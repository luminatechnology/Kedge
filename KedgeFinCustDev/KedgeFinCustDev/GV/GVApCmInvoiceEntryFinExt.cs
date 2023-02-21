using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.GL;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using System;
using static RCGV.GV.GVApCmInvoiceEntry;
/**
* ====2021-08-09:12172====Alton
* 新增LOV可以查詢到的計價單, 包含已有APRegister.UsrAccConfirmNbr的預付款(DocType=PPM)
* 
* ====2021-08-12:12193===Alton
* GV進項折讓主檔維護作業，明細挑選發票的LOV，再多放上計價單及對應傳票號碼
* **/
namespace RCGV.GV
{
    public class GVApCmInvoiceEntryFinExt : PXGraphExtension<GVApCmInvoiceEntry>
    {
        #region Event
        public virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.accConfirmNbr> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.declareYear>(e.Row);
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.declareMonth>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoice, GVApGuiCmInvoice.declareYear> e, PXFieldDefaulting baseMethod)
        {
            GVApGuiCmInvoice row = e.Row;
            if (row == null) return;
            if (e.Row.AccConfirmNbr != null)
            {
                var batch = GetBatchByAccConfirmNbr(e.Row.AccConfirmNbr);
                if (batch == null) return;
                e.NewValue = batch.DateEntered?.Year;
                return;
            }
            baseMethod(e.Cache, e.Args);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoice, GVApGuiCmInvoice.declareMonth> e, PXFieldDefaulting baseMethod)
        {
            GVApGuiCmInvoice row = e.Row;
            if (row == null) return;
            if (row.AccConfirmNbr != null)
            {
                var batch = GetBatchByAccConfirmNbr(row.AccConfirmNbr);
                if (batch == null) return;
                e.NewValue = batch.DateEntered?.Month;
                return;
            }
            baseMethod(e.Cache, e.Args);
        }


        public void _(Events.FieldDefaulting<APRegisterLOV, APRegisterLOV.isPrepaymentDuct> e)
        {
            if (e.Row == null) return;
            e.NewValue = e.Row.PrepaymentDuctAmt > 0;
        }
        #endregion


        #region CacheAttached
        #region GVApGuiCmInvoice
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(
            typeof(Search<APRegisterLOV.accConfirmNbr, Where<APRegisterLOV.vendorID, Equal<Current<GVApGuiCmInvoice.vendorID>>>>),
            typeof(APRegisterLOV.accConfirmNbr),
            typeof(APRegisterLOV.refNbr),
            typeof(APRegisterLOV.deductionRefNbr),
            typeof(APRegisterLOV.isPrepaymentDuct),
            typeof(APRegisterLOV.projectID),
            typeof(APRegisterLOV.poNbr),
            typeof(APRegisterLOV.vendorID),
            typeof(APRegisterLOV.acctName)
            )]
        public void _(Events.CacheAttached<GVApGuiCmInvoice.accConfirmNbr> e) { }
        #endregion

        #region GVApGuiCmInvoiceLine
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXSelector(
                typeof(Search2<GVApGuiInvoice.guiInvoiceNbr,
                    LeftJoin<APRegister,
                        On<APRegister.refNbr, Equal<GVApGuiInvoice.refNbr>>>,
                    Where<GVApGuiInvoice.vendorUniformNumber, Equal<Current<GVApGuiCmInvoice.vendorUniformNumber>>,
                        And<GVApGuiInvoice.invoiceType, Equal<GVList.GVGuiInvoiceType.invoice>,
                        And<GVApGuiInvoice.status, In3<GVList.GVStatus.open, GVList.GVStatus.hold>>>>>),
                typeof(GVApGuiInvoice.guiInvoiceNbr),
                typeof(GVApGuiInvoice.refNbr),
                typeof(APRegisterFinExt.usrAccConfirmNbr),
                typeof(GVApGuiInvoice.vendor),
                typeof(GVApGuiInvoice.invoiceType),
                typeof(GVApGuiInvoice.salesAmt),
                typeof(GVApGuiInvoice.taxAmt),
                    SubstituteKey = typeof(GVApGuiInvoice.guiInvoiceNbr)
            )]
        public void GVApGuiCmInvoiceLineV_ApGuiInvoiceNbr_CacheAttached(PXCache sender) { }
        #endregion
        #endregion

        #region BQL
        protected virtual Batch GetBatchByAccConfirmNbr(string accConfirmNbr)
        {
            return PXSelectJoin<Batch,
                InnerJoin<APRegister, On<APRegister.batchNbr, Equal<Batch.batchNbr>>>,
                Where<APRegisterFinExt.usrAccConfirmNbr, Equal<Required<APRegisterFinExt.usrAccConfirmNbr>>>>
                .Select(Base, accConfirmNbr); ;

        }
        #endregion

        #region Table
        #region APRegisterLOV
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<APRegister,
                InnerJoin<APInvoice,
                    On<APInvoice.refNbr, Equal<APRegister.refNbr>,
                    And<APInvoice.docType, Equal<APRegister.docType>>>,
                InnerJoin<KGBillSummary,
                    On<KGBillSummary.refNbr, Equal<APRegister.refNbr>,
                    And<KGBillSummary.docType, Equal<APRegister.docType>>>,
                InnerJoin<BAccountR,
                    On<BAccountR.bAccountID, Equal<APRegister.vendorID>>>>>,
                Where<APRegister.docType, In3<APDocType.invoice, APDocType.debitAdj, APDocType.prepayment>,
                  And<APRegisterExt.usrIsDeductionDoc, NotEqual<True>,
                  And<APRegisterFinExt.usrAccConfirmNbr, IsNotNull>>>>), Persistent = false)]
        public partial class APRegisterLOV : IBqlTable
        {

            #region AccConfirmNbr
            [PXDBString(BqlField = typeof(APRegisterFinExt.usrAccConfirmNbr))]
            [PXUIField(DisplayName = "AccConfirm Nbr")]
            public virtual string AccConfirmNbr { get; set; }
            public abstract class accConfirmNbr : PX.Data.BQL.BqlString.Field<accConfirmNbr> { }
            #endregion

            #region RefNbr
            [PXDBString(BqlField = typeof(APRegister.refNbr))]
            [PXUIField(DisplayName = "RefNbr")]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region DeductionRefNbr
            [PXString]
            [PXUIField(DisplayName = "Deduction RefNbr")]
            [PXUnboundDefault(typeof(Search<APRegister.refNbr,
            Where<APRegister.docType, Equal<APDocType.debitAdj>,
                And<APRegisterExt.usrIsDeductionDoc, Equal<True>,
                    And<APRegisterExt.usrOriDeductionDocType, Equal<APDocType.invoice>,
                        And<APRegisterExt.usrOriDeductionRefNbr, Equal<Current<refNbr>>>>>>>))]
            public virtual string DeductionRefNbr { get; set; }
            public abstract class deductionRefNbr : PX.Data.BQL.BqlString.Field<deductionRefNbr> { }
            #endregion

            #region PONbr
            [PXDBString(BqlField = typeof(APRegisterExt.usrPONbr))]
            [PXUIField(DisplayName = "PO Nbr")]
            public virtual string PONbr { get; set; }
            public abstract class poNbr : PX.Data.BQL.BqlString.Field<poNbr> { }
            #endregion

            #region ProjectID
            [APActiveProjectAttibute(BqlField = typeof(APRegister.projectID))]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region VendorID
            [VendorActive(
                BqlField = typeof(APRegister.vendorID),
                Visibility = PXUIVisibility.SelectorVisible,
                DescriptionField = typeof(Vendor.acctName),
                CacheGlobal = true,
                Filterable = true)]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

            #region AcctName
            [PXDBString(BqlField = typeof(BAccountR.acctName))]
            [PXUIField(DisplayName = "Vendor Name")]
            public virtual string AcctName { get; set; }
            public abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }
            #endregion

            #region PrepaymentDuctAmt
            [PXDBDecimal(BqlField = typeof(KGBillSummary.prepaymentDuctAmt))]
            [PXUIField(DisplayName = "Prepayment Duct")]
            public virtual Decimal? PrepaymentDuctAmt { get; set; }
            public abstract class prepaymentDuctAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentDuctAmt> { }
            #endregion

            #region IsPrepaymentDuct
            [PXBool()]
            [PXUIField(DisplayName = "Is Prepayment Duct")]
            [PXUnboundDefault]
            public virtual bool? IsPrepaymentDuct { get; set; }
            public abstract class isPrepaymentDuct : PX.Data.BQL.BqlBool.Field<isPrepaymentDuct> { }
            #endregion

        }
        #endregion

        #region GVApGuiCmInvoiceLineVFinExt
        public class GVApGuiCmInvoiceLineVFinExt : PXCacheExtension<GVApGuiCmInvoiceLineV>
        {
            #region AccConfirmNbr
            [PXString()]
            [PXUIField(DisplayName = "Acc Confirm Nbr", Enabled = false)]
            [PXUnboundDefault(typeof(Search2<
                APRegisterFinExt.usrAccConfirmNbr,
                InnerJoin<GVApGuiInvoice,
                    On<APRegister.refNbr, Equal<GVApGuiInvoice.refNbr>>>,
                Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Current<GVApGuiCmInvoiceLine.apGuiInvoiceNbr>>>>))]
            public virtual string AccConfirmNbr { get; set; }
            public abstract class accConfirmNbr : PX.Data.BQL.BqlString.Field<accConfirmNbr> { }
            #endregion
        }

        #endregion

        #endregion
    }
}
