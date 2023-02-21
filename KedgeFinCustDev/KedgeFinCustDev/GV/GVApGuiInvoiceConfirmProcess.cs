using System;
using System.Collections.Generic;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.PM;
using PX.Objects.CR;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using RCGV.GV;
using RC.Util;

namespace GV
{
    /**
     * ====2021-03-26 : 口頭 ===Alton
     * 修改為可以找的到手開的進項發票
     * 
     * **/
    public class GVApGuiInvoiceConfirmProcess : PXGraph<GVApGuiInvoiceConfirmProcess>
    {
        public GVApGuiInvoiceConfirmProcess()
        {
            DetailsView.SetProcessAllVisible(false);
            DetailsView.SetProcessCaption("批次確認");
            DetailsView.SetProcessDelegate(delegate (List<GVApGuiInvoiceV> list)
            {
                List<GVApGuiInvoice> datas = new List<GVApGuiInvoice>();
                foreach (GVApGuiInvoiceV item in list)
                {
                    datas.Add(item);
                }
                DoConfirm(datas);
            });
        }

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXFilteredProcessing<GVApGuiInvoiceV, ParamTable,
            Where<True, Equal<True>,
                And2<Where<Current2<ParamTable.invoiceDateFrom>, IsNull,
                    Or<Current2<ParamTable.invoiceDateFrom>, LessEqual<GVApGuiInvoiceV.invoiceDate>>>,
                And2<Where<Current2<ParamTable.invoiceDateTo>, IsNull,
                    Or<Current2<ParamTable.invoiceDateTo>, GreaterEqual<GVApGuiInvoiceV.invoiceDate>>>,
                And2<Where<Current2<ParamTable.declareYear>, IsNull,
                    Or<Current2<ParamTable.declareYear>, Equal<GVApGuiInvoiceV.declareYear>>>,
                And2<Where<Current2<ParamTable.declareMonth>, IsNull,
                    Or<Current2<ParamTable.declareMonth>, Equal<GVApGuiInvoiceV.declareMonth>>>,
                And2<Where<Current2<ParamTable.projectID>, IsNull,
                    Or<Current2<ParamTable.projectID>, Equal<GVApGuiInvoiceV.projectID>>>,
                And<Where<Current2<ParamTable.vendorID>, IsNull,
                    Or<Current2<ParamTable.vendorID>, Equal<GVApGuiInvoiceV.vendor>>>
                    >>>>>>>
            > DetailsView;
        #endregion

        #region Action
        public PXCancel<ParamTable> Cancel;

        #region HyperLink
        public PXAction<GVApGuiInvoiceV> ViewGuiInvoice;
        [PXButton(CommitChanges = true, OnClosingPopup = PXSpecialButtonType.Refresh)]
        [PXUIField(Visible = false)]
        protected virtual void viewGuiInvoice()
        {
            GVApGuiInvoiceV item = DetailsView.Current;
            new HyperLinkUtil<GVApInvoiceMaint>(item, true);
        }

        public PXAction<GVApGuiInvoiceV> ViewAPInvoice;
        [PXButton(CommitChanges = true, OnClosingPopup = PXSpecialButtonType.Refresh)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPInvoice()
        {
            GVApGuiInvoiceV item = DetailsView.Current;
            new HyperLinkUtil<APInvoiceEntry>(GetAPInvoice(item.RefNbr,item.DocType), true);
        }
        #endregion

        #endregion

        #region Event
        protected virtual void _(Events.FieldDefaulting<ParamTable, ParamTable.declareYear> e)
        {
            e.NewValue = this.Accessinfo.BusinessDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<ParamTable, ParamTable.declareMonth> e)
        {
            e.NewValue = this.Accessinfo.BusinessDate?.Month;
        }
        #endregion

        #region Method
        public static void DoConfirm(List<GVApGuiInvoice> data)
        {
            GVApInvoiceMaint graph = PXGraph.CreateInstance<GVApInvoiceMaint>();
            for (int i = 0; i < data.Count; i++)
            {
                try
                {
                    graph.Invoice.Current = graph.Invoice.Search<GVApGuiInvoice.guiInvoiceID>(data[i].GuiInvoiceID);
                    graph.ConfirmBtn.Press();
                }
                catch (Exception e)
                {
                    PXProcessing.SetError(i, e);
                }

            }
        }

        #endregion

        #region BQL
        private APInvoice GetAPInvoice(string refNbr, string docType)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APInvoice.docType, Equal<Required<APInvoice.docType>>>>>
                .Select(this, refNbr, docType);
        }
        #endregion

        #region Table
        #region ParamTable
        [Serializable]
        public class ParamTable : IBqlTable
        {
            #region InvoiceDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "InvoiceDateFrom")]
            public virtual DateTime? InvoiceDateFrom { get; set; }
            public abstract class invoiceDateFrom : PX.Data.BQL.BqlDateTime.Field<invoiceDateFrom> { }
            #endregion

            #region InvoiceDateTo
            [PXDate()]
            [PXUIField(DisplayName = "InvoiceDateTo")]
            public virtual DateTime? InvoiceDateTo { get; set; }
            public abstract class invoiceDateTo : PX.Data.BQL.BqlDateTime.Field<invoiceDateTo> { }
            #endregion

            #region DeclareYear
            [PXInt()]
            [PXUIField(DisplayName = "DeclareYear")]
            [PXUnboundDefault]
            public virtual int? DeclareYear { get; set; }
            public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
            #endregion

            #region DeclareMonth
            [PXInt()]
            [PXUIField(DisplayName = "DeclareMonth")]
            [PXIntList(
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 },
                new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" }
                )]
            [PXUnboundDefault]
            public virtual int? DeclareMonth { get; set; }
            public abstract class declareMonth : PX.Data.BQL.BqlInt.Field<declareMonth> { }
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

            #region Vendor
            [VendorActive(
               Visibility = PXUIVisibility.SelectorVisible,
               DescriptionField = typeof(PX.Objects.AP.Vendor.acctName),
               CacheGlobal = true,
               Filterable = true,
               Required = false
               )]
            [PXDefault]
            [PXUIField(DisplayName = "Vendor")]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

        }
        #endregion

        #region GVApGuiInvoiceV
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<GVApGuiInvoice,
            LeftJoin<APRegister,
                On<GVApGuiInvoice.refNbr, Equal<APRegister.refNbr>,
                And<APRegister.docType, NotIn3<APDocType.voidCheck, APDocType.voidQuickCheck, APDocType.voidRefund>,
                And<APRegister.released,Equal<True>,
                And<APRegister.status,Equal<APDocStatus.open>>>>>>,
            Where<GVApGuiInvoice.status, Equal<GVList.GVStatus.hold>,
                And<
                    Where<GVApGuiInvoice.ePRefNbr, IsNull,
                        Or<Where<GVApGuiInvoice.ePRefNbr, IsNotNull, And<GVApGuiInvoice.refNbr, IsNotNull>>>>>
           >>), Persistent = false)]
        public class GVApGuiInvoiceV : GVApGuiInvoice
        {
            #region Selected
            [PXBool()]
            [PXUIField(DisplayName = "Selected")]
            public virtual bool? Selected { get; set; }
            public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
            #endregion

            #region UsrAccConfirmNbr
            [PXDBString(BqlField = typeof(APRegisterFinExt.usrAccConfirmNbr))]
            [PXUIField(DisplayName = "UsrAccConfirmNbr")]
            public virtual string UsrAccConfirmNbr { get; set; }
            public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
            #endregion

            #region ProjectID
            [ProjectBase(BqlField = typeof(APRegister.projectID))]
            [PXUIField(DisplayName = "Project ID")]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region DocType
            [PXDBString(BqlField = typeof(APRegister.docType))]
            [PXUIField(DisplayName = "DocType")]
            public virtual string DocType { get; set; }
            public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
            #endregion
        }

        #endregion
        #endregion


    }
}