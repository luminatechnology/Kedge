using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.PM;
using RC.Util;
using RCGV.GV;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using Branch = PX.Objects.GL.Branch;

/**
 * ====2021-08-10:12195====Alton
 * ARInvoice的單據，除了status=void,rejected,reversed以外，都可以看得到
 * ====2021-08-27:12213====Alton
 * 折讓明細需綁上發票明細ID
 * **/
namespace GV
{
    public class GVArGuiAllowanceInvoiceProcess : PXGraph<GVArGuiAllowanceInvoiceProcess>
    {

        public GVArGuiAllowanceInvoiceProcess()
        {
            DetailsView.SetProcessCaption("開立折讓");
            //DetailsView.SetProcessEnabled(false);
            DetailsView.SetProcessAllVisible(false);
        }

        #region Action
        public PXCancel<ParamTable> Cancel;

        #region HyperLink
        #region View Invoice
        public PXAction<ARInvoiceV> ViewInv;
        [PXButton(CommitChanges = true, OnClosingPopup = PXSpecialButtonType.Refresh)]
        [PXUIField(Visible = false)]
        protected virtual void viewInv()
        {
            ARInvoiceV item = DetailsView.Current;
            ARInvoice invoice = GetARInvoiceByKey(item.RefNbr, item.DocType);
            new HyperLinkUtil<ARInvoiceEntry>(invoice, true);
        }
        #endregion

        #region View Ori Invoice
        public PXAction<ARInvoiceV> ViewOriInv;
        [PXButton(CommitChanges = true, OnClosingPopup = PXSpecialButtonType.Refresh)]
        [PXUIField(Visible = false)]
        protected virtual void viewOriInv()
        {
            ARInvoiceV item = DetailsView.Current;
            ARInvoice invoice = GetARInvoiceByKey(item.OrigRefNbr, item.OrigDocType);
            new HyperLinkUtil<ARInvoiceEntry>(invoice, true);
        }
        #endregion

        #endregion
        #endregion

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXFilteredProcessing<ARInvoiceV, ParamTable,
           Where<True, Equal<True>,
            And2<Where<Current2<ParamTable.refNbrFrom>, IsNull,
                   Or<Current2<ParamTable.refNbrFrom>, LessEqual<ARInvoiceV.refNbr>>>,
               And2<Where<Current2<ParamTable.refNbrTo>, IsNull,
                   Or<Current2<ParamTable.refNbrTo>, GreaterEqual<ARInvoiceV.refNbr>>>,
               And2<Where<Current2<ParamTable.docDateFrom>, IsNull,
                   Or<Current2<ParamTable.docDateFrom>, LessEqual<ARInvoiceV.docDate>>>,
               And2<Where<Current2<ParamTable.docDateTo>, IsNull,
                   Or<Current2<ParamTable.docDateTo>, GreaterEqual<ARInvoiceV.docDate>>>,
               And2<Where<Current2<ParamTable.projectID>, IsNull,
                   Or<Current2<ParamTable.projectID>, Equal<ARInvoiceV.projectID>>>,
               And<Where<Current2<ParamTable.customerID>, IsNull,
                   Or<Current2<ParamTable.customerID>, Equal<ARInvoiceV.customerID>>>
                   >>>>>>>> DetailsView;
        public IEnumerable detailsView()
        {
            PXView view = new PXView(this, true, DetailsView.View.BqlSelect);
            foreach (ARInvoiceV item in view.SelectMulti(null))
            {
                PXResultset<GVArGuiInvoice> rs = GetGVArGuiInvoiceByRefNbr(item.OrigRefNbr, item.OrigDocType);
                //只抓取發票比數為1的資料
                if (rs.Count == 1)
                {
                    GVArGuiInvoice invoice = rs;
                    PXResultset<GVArGuiCmInvoice> cmrs = GetGVArGuiCmInvoice(item.RefNbr, item.DocType);
                    //排除ARInvoice(CRM)已開立過折讓之資料
                    if (cmrs.Count == 0)
                        yield return item;
                }
            }
        }
        #endregion

        #region Event
        protected virtual void _(Events.RowSelected<ParamTable> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;

            bool check = true;
            if (row.InvoiceDate == null)
            {
                check = false;
            }
            else
            {
                GVPeriod gvPeriod = GetGVPeriod(GetGVRegistration(this.Accessinfo.BranchID)?.RegistrationCD, row.InvoiceDate?.Year, row.InvoiceDate?.Month);
                if (gvPeriod == null)
                {
                    SetError<ParamTable.invoiceDate>(e.Cache,
                           row, row.InvoiceDate, "發票期別尚未解鎖或沒有設定，無法開立該期別之銷項發票/折讓");
                    check = false;
                }
                else
                {
                    ErrorClear<ParamTable.invoiceDate>(e.Cache, row, row.InvoiceDate);
                }
            }
            DetailsView.SetProcessEnabled(check);
            DetailsView.SetProcessDelegate(list =>
                CreateCmInvoice((GVArGuiAllowanceInvoiceProcess)e.Cache.Graph, list, row));
        }

        #endregion

        #region Method
        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }

        private void ErrorClear<Field>(PXCache cache, object row, object newValue) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue, null);
        }

        public static void CreateCmInvoice(GVArGuiAllowanceInvoiceProcess graph, List<ARInvoiceV> datas, ParamTable param)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                ARInvoiceV item = datas[i];
                //檢核
                if (!graph.Check(item, i)) continue;
                try
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        GVArGuiCmInvoiceMaint entry = PXGraph.CreateInstance<GVArGuiCmInvoiceMaint>();
                        graph.CreateGVArGuiCmInvoice(entry, item, param);
                        graph.CreateGVArGuiCmInvoiceLine(entry, item);
                        entry.Save.Press();
                        if (param.AutoConfirm == true)
                        {
                            GVArGuiCmInvoice row = entry.gvArGuiInvoices.Current;
                            entry.gvArGuiInvoices.Cache.SetValueExt<GVArGuiCmInvoice.hold>(row, false);
                            entry.gvArGuiInvoices.Update(row);
                            entry.Save.Press();
                        }
                        ts.Complete();
                    }
                }
                catch (Exception e)
                {
                    PXProcessing.SetError(i, e);
                }
            }
        }

        public virtual void CreateGVArGuiCmInvoice(GVArGuiCmInvoiceMaint entry, ARInvoiceV item, ParamTable param)
        {
            PXCache cache = entry.gvArGuiInvoices.Cache;
            GVArGuiCmInvoice row = (GVArGuiCmInvoice)cache.CreateInstance();
            row = entry.gvArGuiInvoices.Insert(row);
            cache.SetValueExt<GVArGuiCmInvoice.invoiceDate>(row, param.InvoiceDate);
            cache.SetValueExt<GVArGuiCmInvoice.customerID>(row, item.CustomerID);
            cache.SetValueExt<GVArGuiCmInvoice.taxCode>(row, GVList.GVTaxCode.TAXABLE);
            cache.SetValueExt<GVArGuiCmInvoice.guiType>(row, param.GuiType);
            cache.SetValueExt<GVArGuiCmInvoice.batchNbr>(row, item.BatchNbr);
            cache.SetValueExt<GVArGuiCmInvoice.remark>(row, item.DocDesc);
            row = entry.gvArGuiInvoices.Update(row);
        }

        public virtual void CreateGVArGuiCmInvoiceLine(GVArGuiCmInvoiceMaint entry, ARInvoiceV item)
        {
            PXCache cache = entry.gvArGuiInvoiceLines.Cache;
            GVArGuiCmInvoiceLine row = (GVArGuiCmInvoiceLine)cache.CreateInstance();
            cache.SetValueExt<GVArGuiCmInvoiceLine.arGuiInvoiceNbr>(row, item.ArGuiInvoiceNbr);
            cache.SetValueExt<GVArGuiCmInvoiceLine.guiInvoiceDetailID>(row, item.GuiInvoiceDetailID);
            row = entry.gvArGuiInvoiceLines.Insert(row);
            cache.SetValueExt<GVArGuiCmInvoiceLine.salesAmt>(row, item.CuryVatTaxableTotal);
            cache.SetValueExt<GVArGuiCmInvoiceLine.itemDesc>(row, item.DocDesc);
            cache.SetValueExt<GVArGuiCmInvoiceLine.arRefNbr>(row, item.RefNbr);
            cache.SetValueExt<GVArGuiCmInvoiceLine.arDocType>(row, item.DocType);
            row = entry.gvArGuiInvoiceLines.Update(row);
        }

        public virtual bool Check(ARInvoiceV data, int i)
        {
            if (String.IsNullOrEmpty(data.DocDesc))
            {
                PXProcessing.SetError(i, "DocDesc cannot be null");
                return false;
            }
            return true;
        }
        #endregion

        #region BQL

        /// <summary>
        /// 取得該ARInvoice底下的發票
        /// </summary>
        /// <param name="refNbr"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        private PXResultset<GVArGuiInvoice> GetGVArGuiInvoiceByRefNbr(string refNbr, string docType)
        {
            return PXSelectJoinGroupBy<GVArGuiInvoice,
                InnerJoin<GVArGuiInvoiceDetail,
                    On<GVArGuiInvoice.guiInvoiceID, Equal<GVArGuiInvoiceDetail.guiInvoiceID>>>,
                Where<GVArGuiInvoiceDetail.arRefNbr, Equal<Required<GVArGuiInvoiceDetail.arRefNbr>>,
                    And<GVArGuiInvoiceDetail.aRDocType, Equal<Required<GVArGuiInvoiceDetail.aRDocType>>,
                    And<GVArGuiInvoice.status, NotEqual<GVList.GVStatus.voidinvoice>>>>,
                Aggregate<GroupBy<GVArGuiInvoice.guiInvoiceID>>>
                .Select(this, refNbr, docType);
        }

        /// <summary>
        /// 取得銷項折讓
        /// </summary>
        /// <param name="refNbr"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        private PXResultset<GVArGuiCmInvoice> GetGVArGuiCmInvoice(string refNbr, string docType)
        {
            return PXSelectJoin<GVArGuiCmInvoice,
                InnerJoin<GVArGuiCmInvoiceLine,
                    On<GVArGuiCmInvoice.guiCmInvoiceID, Equal<GVArGuiCmInvoiceLine.guiCmInvoiceID>>>,
                Where<GVArGuiCmInvoiceLine.arRefNbr, Equal<Required<GVArGuiCmInvoiceLine.arRefNbr>>,
                    And<GVArGuiCmInvoiceLine.arDocType, Equal<Required<GVArGuiCmInvoiceLine.arDocType>>,
                    And<GVArGuiCmInvoice.status, NotEqual<GVList.GVStatus.voidinvoice>>>>>
                .Select(this, refNbr, docType);
        }

        private ARInvoice GetARInvoiceByKey(string refNbr, string docType)
        {
            return PXSelect<ARInvoice,
                Where<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>
                , And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>>>>
                .Select(new PXGraph(), refNbr, docType);
        }

        private GVPeriod GetGVPeriod(string registrationCD, int? year, int? month)
        {
            return PXSelect<GVPeriod,
                Where<GVPeriod.registrationCD, Equal<Required<GVPeriod.registrationCD>>,
                And<GVPeriod.periodYear, Equal<Required<GVPeriod.periodYear>>,
                And<GVPeriod.periodMonth, Equal<Required<GVPeriod.periodMonth>>,
                And<GVPeriod.outActive, Equal<True>>>>>>
                .Select(this, registrationCD, year, month);
        }

        private GVRegistration GetGVRegistration(int? branchID)
        {
            return PXSelectJoin<GVRegistration,
                    InnerJoin<GVRegistrationBranch, On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>,
                    InnerJoin<Branch, On<Branch.bAccountID, Equal<GVRegistrationBranch.bAccountID>>>>,
                    Where<Branch.branchID, Equal<Required<Branch.branchID>>>>
                    .Select(this, branchID);
        }
        #endregion

        #region Table
        #region ParamTable
        [Serializable]
        [PXHidden]
        public class ParamTable : IBqlTable
        {
            #region Filter
            #region DocDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "DocDateFrom")]
            public virtual DateTime? DocDateFrom { get; set; }
            public abstract class docDateFrom : PX.Data.BQL.BqlDateTime.Field<docDateFrom> { }
            #endregion

            #region DocDateTo
            [PXDate()]
            [PXUIField(DisplayName = "DocDateTo")]
            public virtual DateTime? DocDateTo { get; set; }
            public abstract class docDateTo : PX.Data.BQL.BqlDateTime.Field<docDateTo> { }
            #endregion

            #region RefNbrFrom  
            [PXString()]
            [PXUIField(DisplayName = "RefNbrFrom")]
            [PXUnboundDefault]
            public virtual string RefNbrFrom { get; set; }
            public abstract class refNbrFrom : PX.Data.BQL.BqlString.Field<refNbrFrom> { }
            #endregion

            #region RefNbrTo  
            [PXString()]
            [PXUIField(DisplayName = "RefNbrTo")]
            [PXUnboundDefault]
            public virtual string RefNbrTo { get; set; }
            public abstract class refNbrTo : PX.Data.BQL.BqlString.Field<refNbrTo> { }
            #endregion

            #region ProjectID
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            [PXUIField(DisplayName = "Project ID")]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region Customer
            [CustomerActive(DescriptionField = typeof(Customer.acctName), Required = false)]
            [PXDefault]
            public virtual int? CustomerID { get; set; }
            public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
            #endregion
            #endregion

            #region Invoice Info

            //#region RegistrationCD
            //[PXString(9, IsUnicode = true)]
            ////[PXUIField(DisplayName = "RegistrationCD", Required = true)]
            ////[RegistrationCD()]
            //[PXUnboundDefault(typeof(
            //    Search2<GVRegistration.registrationCD,
            //        InnerJoin<GVRegistrationBranch, On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>,
            //        InnerJoin<Branch, On<Branch.bAccountID, Equal<GVRegistrationBranch.bAccountID>>>>,
            //        Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>),
            //    PersistingCheck = PXPersistingCheck.NullOrBlank)]
            //public virtual string RegistrationCD { get; set; }
            //public abstract class registrationCD : PX.Data.BQL.BqlString.Field<registrationCD> { }
            //#endregion

            #region InvoiceDate
            [PXDate()]
            [PXUnboundDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [PXUIField(DisplayName = "Invoice Date", Required = true)]
            public virtual DateTime? InvoiceDate { get; set; }
            public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
            #endregion

            #region GuiType
            [PXString(2, IsFixed = true, IsUnicode = true)]
            [PXUIField(DisplayName = "Gui Type", Required = true)]
            [PXUnboundDefault(GVList.GVGuiType.AR.GuiType_33)]
            [GVList.GVGuiType.ARCmInvoice]
            public virtual string GuiType { get; set; }
            public abstract class guiType : PX.Data.BQL.BqlString.Field<guiType> { }
            #endregion

            #region AutoConfirm
            [PXBool()]
            [PXUIField(DisplayName = "Auto Confirm")]
            [PXUnboundDefault(false)]
            public virtual bool? AutoConfirm { get; set; }
            public abstract class autoConfirm : PX.Data.BQL.BqlBool.Field<autoConfirm> { }
            #endregion

            #endregion
        }
        #endregion

        #region ARInvoiceV
        [Serializable]
        [PXProjection(typeof(Select2<ARInvoice,
            InnerJoin<ARRegister,
                On<ARRegister.docType, Equal<ARInvoice.docType>,
                And<ARRegister.refNbr, Equal<ARInvoice.refNbr>>>,
           InnerJoin<GVArGuiInvoiceDetail,
                On<GVArGuiInvoiceDetail.arRefNbr, Equal<ARRegister.origRefNbr>,
                 And<GVArGuiInvoiceDetail.aRDocType, Equal<ARRegister.origDocType>>>,
           InnerJoin<GVArGuiInvoice, On<GVArGuiInvoice.guiInvoiceID, Equal<GVArGuiInvoiceDetail.guiInvoiceID>>>>>,
            Where<ARInvoice.docType, Equal<ARDocType.creditMemo>,
                And<ARRegister.status, NotIn3<ARDocStatus.voided, ARDocStatus.rejected, ARDocStatus.reserved>
               >>
           >), Persistent = false)]
        [PXHidden]
        public class ARInvoiceV : ARInvoice
        {
            #region ArGuiInvoiceNbr
            [PXString(40, IsUnicode = true)]
            [PXUnboundDefault(
                typeof(Search2<GVArGuiInvoice.guiInvoiceNbr,
                    InnerJoin<GVArGuiInvoiceDetail,
                        On<GVArGuiInvoice.guiInvoiceID, Equal<GVArGuiInvoiceDetail.guiInvoiceID>>>,
                    Where<GVArGuiInvoiceDetail.arRefNbr, Equal<Current<origRefNbr>>,
                        And<GVArGuiInvoiceDetail.aRDocType, Equal<Current<origDocType>>>>
                    >)
                )]
            [PXUIField(DisplayName = "ArGuiInvoiceNbr")]
            public virtual string ArGuiInvoiceNbr { get; set; }
            public abstract class arGuiInvoiceNbr : PX.Data.BQL.BqlString.Field<arGuiInvoiceNbr> { }
            #endregion

            #region GuiInvoiceDetailID
            [PXInt()]
            [PXUnboundDefault(
                typeof(Search<GVArGuiInvoiceDetail.guiInvoiceDetailID,
                    Where<GVArGuiInvoiceDetail.arRefNbr, Equal<Current<origRefNbr>>,
                        And<GVArGuiInvoiceDetail.aRDocType, Equal<Current<origDocType>>>>
                    >)
                )]
            public virtual int? GuiInvoiceDetailID { get; set; }
            public abstract class guiInvoiceDetailID : PX.Data.BQL.BqlInt.Field<guiInvoiceDetailID> { }
            #endregion

            #region OrigInvoiceSalesAmt
            [PXDBDecimal(BqlField = typeof(GVArGuiInvoice.salesAmt))]
            [PXUIField(DisplayName = "Orig Invoice Sales Amt")]
            public virtual decimal? OrigInvoiceSalesAmt { get; set; }
            public abstract class origInvoiceSalesAmt : PX.Data.BQL.BqlString.Field<origInvoiceSalesAmt> { }
            #endregion
        }
        #endregion

        #endregion


    }
}