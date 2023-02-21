using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.PM;
using RC.Util;
using RCGV.GV;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using Branch = PX.Objects.GL.Branch;
/**
 * ====2021-07-30:12177====Alton
 * 	1.如果對應的customer沒有維護統編, 會有null Reference錯誤
 *  2.錯誤訊息請顯示"客戶沒有維護統一編號"
 *  ====2021-08-10:12195====Alton
 * ARInvoice的單據，除了status=void,rejected,reversed以外，都可以看得到
 * ====2021-08-11:12196====Alton
 * 在 銷像發票簿設定 內的發票簿，依照user在GV銷項發票/銷項批次開立/銷項折讓/銷項批次折讓 這四支作業中,
 * 所選的InvoiceDate的月份符合以下邏輯，就要在「發票簿號」LOV內顯示出來：
 * -當所選InvoiceDate的月份，在GVPeriod.OutActive = True時，即可開立銷項發票
 * -當所選InvoiceDate的月份，在GVPeriod.OutActive <> True 時，請跳出error message:'發票期別尚未解鎖或沒有設定，無法開立該期別之銷項發票
 * ====2021-09-24:12242====Alton
 * 請多加一項入到此畫面的資料條件：
 * 請檢查如果有 ARRegister.RefNbr = ARRegister.OrigRefNbr
 * 則該張AR Invoice 不出現在 GV502001的批次作業中
 * **/
namespace GV
{
    public class GVArGuiInvoiceProcess : PXGraph<GVArGuiInvoiceProcess>
    {

        public GVArGuiInvoiceProcess()
        {
            DetailsView.SetProcessCaption("開立發票");
            //DetailsView.SetProcessEnabled(false);
            DetailsView.SetProcessAllVisible(false);
        }

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
            //foreach (ARInvoiceV item in view.SelectMultiBound(new object[] { MasterView.Current },null)) {
            foreach (ARInvoiceV item in view.SelectMulti(null))
            {
                if (item.Balance > 0 && item.OvRefNbr == null)
                    yield return item;
            }
        }
        #endregion

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

        #endregion
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
                GVPeriod gvPeriod = GetGVPeriod(row.RegistrationCD, row.InvoiceDate?.Year, row.InvoiceDate?.Month);
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
            if (row.GuiBookID == null) check = false;

            DetailsView.SetProcessEnabled(check);
            DetailsView.SetProcessDelegate(list =>
                CreateInvoice((GVArGuiInvoiceProcess)e.Cache.Graph, list, row));
        }

        protected virtual void _(Events.FieldUpdated<ParamTable, ParamTable.invoiceDate> e)
        {
            ParamTable row = e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<ParamTable.guiBookID>(row, null);
        }

        #endregion

        #region Method
        public static void CreateInvoice(GVArGuiInvoiceProcess graph, List<ARInvoiceV> datas, ParamTable param)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                ARInvoiceV data = datas[i];
                //檢核
                if (!graph.Check(data, i)) continue;
                try
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        GVArInvoiceEntry entry = PXGraph.CreateInstance<GVArInvoiceEntry>();
                        graph.CreateGvArGuiInvoice(entry, data, param);
                        graph.CreateGvArGuiInvoiceDetail(entry, data);
                        entry.Save.Press();
                        if (param.AutoConfirm == true)
                        {
                            entry.ConfirmBtn.Press();
                        }
                        ts.Complete();
                        PXProcessing.SetInfo(i, "Process Success");
                    }
                }
                catch (Exception e)
                {
                    PXProcessing.SetError(i, e);
                }

            }

        }

        public virtual bool Check(ARInvoiceV data, int i)
        {
            //if (String.IsNullOrEmpty(data.DocDesc))
            //{
            //    PXProcessing.SetError(i, "DocDesc can not be null");
            //    return false;
            //}
            BAccount ba = GetBAccount(data.CustomerID);
            Location location = GetLocation(ba?.DefLocationID);
            if (String.IsNullOrEmpty(location?.TaxRegistrationID))
            {
                PXProcessing.SetError(i, "客戶沒有維護統一編號");
                return false;
            }
            return true;
        }

        public virtual void CreateGvArGuiInvoice(GVArInvoiceEntry entry, ARInvoiceV data, ParamTable param)
        {
            PXCache cache = entry.gvArGuiInvoices.Cache;
            GVArGuiInvoice item = (GVArGuiInvoice)cache.CreateInstance();
            item = entry.gvArGuiInvoices.Insert(item);
            cache.SetValueExt<GVArGuiInvoice.invoiceDate>(item, param.InvoiceDate);
            cache.SetValueExt<GVArGuiInvoice.customerID>(item, data.CustomerID);
            cache.SetValueExt<GVArGuiInvoice.taxCode>(item, GVList.GVTaxCode.TAXABLE);
            cache.SetValueExt<GVArGuiInvoice.guiBookID>(item, param.GuiBookID);
            cache.SetValueExt<GVArGuiInvoice.remark>(item, data.DocDesc);
            item = entry.gvArGuiInvoices.Update(item);
        }

        public virtual void CreateGvArGuiInvoiceDetail(GVArInvoiceEntry entry, ARInvoiceV data)
        {
            PXCache cache = entry.gvArGuiInvoiceDetails.Cache;
            GVArGuiInvoiceDetail item = (GVArGuiInvoiceDetail)cache.CreateInstance();
            PMProject pmProject = GetPMProject(data.ProjectID);
            item = entry.gvArGuiInvoiceDetails.Insert(item);
            cache.SetValueExt<GVArGuiInvoiceDetail.arRefNbr>(item, data.RefNbr);
            cache.SetValueExt<GVArGuiInvoiceDetail.aRDocType>(item, data.DocType);
            cache.SetValueExt<GVArGuiInvoiceDetail.remark>(item, data.DocDesc ?? (pmProject.Description + "款項"));
            cache.SetValueExt<GVArGuiInvoiceDetail.unitPrice>(item, data.Balance);
            cache.SetValueExt<GVArGuiInvoiceDetail.taxAmt>(item, (data.Balance ?? 0m) * 0.05m);
            item = entry.gvArGuiInvoiceDetails.Update(item);
        }

        private void ErrorClear<Field>(PXCache cache, object row, object newValue) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue, null);
        }

        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }
        #endregion

        #region BQL
        private PMProject GetPMProject(int? projectID)
        {
            return PXSelect<PMProject,
                Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>
                .Select(new PXGraph(), projectID);
        }

        private ARInvoice GetARInvoiceByKey(string refNbr, string docType)
        {
            return PXSelect<ARInvoice,
                Where<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>
                , And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>>>>
                .Select(this, refNbr, docType);
        }

        private BAccount GetBAccount(int? baccountID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>
                >>
                .Select(new PXGraph(), baccountID);
        }

        private Location GetLocation(int? locationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(new PXGraph(), locationID);
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
            #region InvoiceDate
            [PXDate()]
            [PXUnboundDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [PXUIField(DisplayName = "Invoice Date", Required = true)]
            public virtual DateTime? InvoiceDate { get; set; }
            public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
            #endregion

            #region GuiBookID
            [PXInt()]
            [PXUnboundDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [PXUIField(DisplayName = "Gui Book ID", Required = true)]
            [PXSelector(
                   typeof(Search<GVGuiBook_GVPeriod.guiBookID,
                       Where<GVGuiBook_GVPeriod.registrationCD, Equal<Current<registrationCD>>,
                        //And<GVPeriod.outActive, Equal<True>,
                        And2<Where<GVGuiBook_GVPeriod.currentNum, NotEqual<GVGuiBook_GVPeriod.endNum>, Or<GVGuiBook_GVPeriod.currentNum, IsNull>>,
                        And<GVGuiBook_GVPeriod.startMonth, LessEqual<DatePart<DatePart.month, Current<invoiceDate>>>,
                        And<GVGuiBook_GVPeriod.endMonth, GreaterEqual<DatePart<DatePart.month, Current<invoiceDate>>>,
                        And<GVGuiBook_GVPeriod.declareYear, Equal<DatePart<DatePart.year, Current<invoiceDate>>>
                        >>>>>>),
                    typeof(GVGuiBook_GVPeriod.guiBookCD),
                    typeof(GVGuiBook_GVPeriod.declareYear),
                    typeof(GVGuiBook_GVPeriod.startMonth),
                    typeof(GVGuiBook_GVPeriod.endMonth),
                    typeof(GVGuiBook_GVPeriod.currentNum),
                    typeof(GVGuiBook_GVPeriod.endNum),
                   SubstituteKey = typeof(GVGuiBook_GVPeriod.guiBookCD)
                )]
            public virtual int? GuiBookID { get; set; }
            public abstract class guiBookID : PX.Data.BQL.BqlInt.Field<guiBookID> { }
            #endregion

            #region RegistrationCD
            [PXString(9, IsUnicode = true)]
            //[PXUIField(DisplayName = "RegistrationCD", Required = true)]
            //[RegistrationCD()]
            [PXUnboundDefault(typeof(
                Search2<GVRegistration.registrationCD,
                    InnerJoin<GVRegistrationBranch, On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>,
                    InnerJoin<Branch, On<Branch.bAccountID, Equal<GVRegistrationBranch.bAccountID>>>>,
                    Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>),
                PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual string RegistrationCD { get; set; }
            public abstract class registrationCD : PX.Data.BQL.BqlString.Field<registrationCD> { }
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
                On<ARInvoice.docType, Equal<ARRegister.docType>,
                And<ARInvoice.refNbr, Equal<ARRegister.refNbr>>>>,
            Where<ARInvoice.docType, Equal<ARDocType.invoice>,
                And<ARRegister.status, NotIn3<ARDocStatus.voided, ARDocStatus.rejected, ARDocStatus.reserved>
               >>
           >), Persistent = false)]
        [PXHidden]
        //[PXCacheName("ARInvoice")]
        public class ARInvoiceV : ARInvoice
        {
            #region UsingAmt
            [PXDecimal(2)]
            [PXUIField(DisplayName = "UsingAmt", Enabled = false)]
            [PXUnboundDefault(TypeCode.Decimal, "0")]
            [PXDBScalar(typeof(Search5<GVArGuiInvoiceDetail.salesAmt,
                InnerJoin<GVArGuiInvoice, On<GVArGuiInvoiceDetail.guiInvoiceID, Equal<GVArGuiInvoice.guiInvoiceID>>>,
                Where<GVArGuiInvoiceDetail.arRefNbr, Equal<refNbr>,
                  And<GVArGuiInvoiceDetail.aRDocType, Equal<docType>
                  , And<GVArGuiInvoice.status, NotEqual<GVList.GVStatus.voidinvoice>>
                      >>,
                Aggregate<Sum<GVArGuiInvoiceDetail.salesAmt>>>))]

            public virtual Decimal? UsingAmt { get; set; }
            public abstract class usingAmt : PX.Data.BQL.BqlDecimal.Field<usingAmt> { }
            #endregion

            #region Balance
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Balance", Enabled = false)]
            [PXUnboundDefault(TypeCode.Decimal, "0")]
            [PXFormula(typeof(Sub<IsNull<curyVatTaxableTotal, Zero>, IsNull<usingAmt, Zero>>))]
            public virtual Decimal? Balance { get; set; }
            public abstract class balance : PX.Data.BQL.BqlDecimal.Field<balance> { }
            #endregion

            //20220318 by louis 新增查詢反轉ARInvoice條件新增docType=creditMemo
            #region OvRefNbr
            [PXString]
            [PXDBScalar(typeof(Search<ARRegister.refNbr,
                Where<ARRegister.origRefNbr, Equal<refNbr>,
                    And<ARRegister.origDocType, Equal<docType>,
                    And<ARRegister.docType, Equal<ARDocType.creditMemo>>>>>))]
            public virtual string OvRefNbr { get; set; }
            public abstract class ovRefNbr : PX.Data.BQL.BqlString.Field<ovRefNbr> { }
            #endregion
        }
        #endregion

        #endregion
    }
}