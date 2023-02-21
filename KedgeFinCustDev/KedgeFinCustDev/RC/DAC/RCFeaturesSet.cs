using System;
using PX.Data;
using PX.Objects.CS;
using RC.Util;

namespace RC.DAC
{
    [Serializable]
    public class RCFeaturesSet : IBqlTable
    {
        #region Status
        [PXDBInt()]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXDefault(1)]
        [PXIntList(
            new int[] { 0, 1 },
            new string[] { "等待啟用中", "啟用" }
        )]
        public virtual int? Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
        #endregion

        #region ValidUntill
        [PXDBDate()]
        [PXUIField(DisplayName = "ValidUntill")]
        public virtual DateTime? ValidUntill { get; set; }
        public abstract class validUntill : PX.Data.BQL.BqlDateTime.Field<validUntill> { }
        #endregion

        #region ValidationCode
        [PXDBString(500, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ValidationCode")]
        public virtual string ValidationCode { get; set; }
        public abstract class validationCode : PX.Data.BQL.BqlString.Field<validationCode> { }
        #endregion

        #region IsTestEnv
        [RCFeature(false, null, DisplayName = "IsTestEnv")]
        public virtual bool? IsTestEnv { get; set; }
        public abstract class isTestEnv : PX.Data.BQL.BqlBool.Field<isTestEnv> { }
        #endregion

        #region GovUniformInvoice
        [RCFeature(false, null, DisplayName = "GovUniformInvoice")]
        public virtual bool? GovUniformInvoice { get { return this._GovUniformInvoice; } set { this._GovUniformInvoice = value; } }
        protected bool? _GovUniformInvoice;
        public abstract class govUniformInvoice : PX.Data.BQL.BqlBool.Field<govUniformInvoice> { }
        #endregion

        #region GuiPreference
        [RCFeature(false, typeof(RCFeaturesSet.govUniformInvoice), DisplayName = "GUIPreference", SyncToParent = false)]
        public virtual bool? GuiPreference { get; set; }
        public abstract class guiPreference : PX.Data.BQL.BqlBool.Field<guiPreference> { }
        #endregion

        #region ApInvoice
        [RCFeature(false, typeof(RCFeaturesSet.govUniformInvoice), DisplayName = "ApInvoice", SyncToParent = false)]
        public virtual bool? ApInvoice { get; set; }
        public abstract class apInvoice : PX.Data.BQL.BqlBool.Field<apInvoice> { }
        #endregion

        #region ArInvoice
        [RCFeature(false, typeof(RCFeaturesSet.govUniformInvoice), DisplayName = "ArInvoice", SyncToParent = false)]
        public virtual bool? ArInvoice { get; set; }
        public abstract class arInvoice : PX.Data.BQL.BqlBool.Field<arInvoice> { }
        #endregion

        #region DeclareGovInvoice
        [RCFeature(false, typeof(RCFeaturesSet.govUniformInvoice), DisplayName = "DeclareGovInvoice", SyncToParent = false)]
        public virtual bool? DeclareGovInvoice { get; set; }
        public abstract class declareGovInvoice : PX.Data.BQL.BqlBool.Field<declareGovInvoice> { }
        #endregion

        #region BankProIntegration
        [RCFeature(false, typeof(RCFeaturesSet.govUniformInvoice), DisplayName = "BankProIntegration", SyncToParent = false)]
        public virtual bool? BankProIntegration { get; set; }
        public abstract class bankProIntegration : PX.Data.BQL.BqlBool.Field<bankProIntegration> { }
        #endregion

        #region WithholdingTaxes
        [RCFeature(false, null, DisplayName = "WithholdingTaxes")]
        public virtual bool? WithholdingTaxes { get; set; }
        public abstract class withholdingTaxes : PX.Data.BQL.BqlBool.Field<withholdingTaxes> { }
        #endregion

        #region NotesManagement
        [RCFeature(false, null, DisplayName = "NotesManagement")]
        public virtual bool? NotesManagement { get; set; }
        public abstract class notesManagement : PX.Data.BQL.BqlBool.Field<notesManagement> { }
        #endregion

        #region NotesPreference
        [RCFeature(false, typeof(RCFeaturesSet.notesManagement), DisplayName = "NotesPreference", SyncToParent = false)]
        public virtual bool? NotesPreference { get; set; }
        public abstract class notesPreference : PX.Data.BQL.BqlBool.Field<notesPreference> { }
        #endregion

        #region NotesReceivable
        [RCFeature(false, typeof(RCFeaturesSet.notesManagement), DisplayName = "NotesReceivable", SyncToParent = false)]
        public virtual bool? NotesReceivable { get; set; }
        public abstract class notesReceivable : PX.Data.BQL.BqlBool.Field<notesReceivable> { }
        #endregion

        #region NotesPayable
        [RCFeature(false, typeof(RCFeaturesSet.notesManagement), DisplayName = "NotesPayable", SyncToParent = false)]
        public virtual bool? NotesPayable { get; set; }
        public abstract class notesPayable : PX.Data.BQL.BqlBool.Field<notesPayable> { }
        #endregion

        #region Check
        [RCFeature(false, typeof(RCFeaturesSet.notesManagement), DisplayName = "Check", SyncToParent = false)]
        public virtual bool? Check { get; set; }
        public abstract class check : PX.Data.BQL.BqlBool.Field<check> { }
        #endregion

        #region LedgerSettlement
        [RCFeature(false, typeof(RCFeaturesSet.notesManagement), DisplayName = "LedgerSettlement", SyncToParent = false)]
        public virtual bool? LedgerSettlement { get; set; }
        public abstract class ledgerSettlement : PX.Data.BQL.BqlBool.Field<ledgerSettlement> { }
        #endregion

        #region PaymentSlip 
        [RCFeature(false, null, DisplayName = "Payment Slip", SyncToParent = false)] 
        public virtual bool? PaymentSlip { get; set; }
        public abstract class paymentSlip : PX.Data.BQL.BqlBool.Field<paymentSlip> { }
        #endregion

        #region ExpenseClaim 
        [RCFeature(false,null, DisplayName = "Expense Claim", SyncToParent = false)] 
        public virtual bool? ExpenseClaim { get; set; }
        public abstract class expenseClaim : PX.Data.BQL.BqlBool.Field<expenseClaim> { }
        #endregion

        #region CertificateManagement 
        [RCFeature(false, null, DisplayName = "Certificate Management", SyncToParent = false)] 
        public virtual bool? CertificateManagement { get; set; }
        public abstract class certificateManagement : PX.Data.BQL.BqlBool.Field<certificateManagement> { }
        #endregion

        #region CCPayableCheck 
        [RCFeature(false, typeof(RCFeaturesSet.certificateManagement), DisplayName = "CC Payable Check", SyncToParent = false)] 
        public virtual bool? CCPayableCheck { get; set; }
        public abstract class cCPayableCheck : PX.Data.BQL.BqlBool.Field<cCPayableCheck> { }
        #endregion

        #region CCReceivableChecck 
        [RCFeature(false, typeof(RCFeaturesSet.certificateManagement), DisplayName = "CC Receivable Checck", SyncToParent = false)] 
        public virtual bool? CCReceivableChecck { get; set; }
        public abstract class cCReceivableChecck : PX.Data.BQL.BqlBool.Field<cCReceivableChecck> { }
        #endregion
    }
}