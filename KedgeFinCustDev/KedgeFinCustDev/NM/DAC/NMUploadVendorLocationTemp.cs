using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CA;
using PX.Objects.GL;

namespace NM.DAC
{
    [Serializable]
    [PXCacheName("NMUploadVendorLocationTemp")]
    public class NMUploadVendorLocationTemp : IBqlTable
    {

        #region TempSeq
        [PXDBIdentity(IsKey = true)]
        public virtual int? TempSeq { get; set; }
        public abstract class tempSeq : PX.Data.BQL.BqlInt.Field<tempSeq> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor ID")]
        [PXSelector(typeof(Search<Vendor.bAccountID>),
                    typeof(Vendor.acctCD),
                    typeof(Vendor.acctName),
                SubstituteKey = typeof(Vendor.acctCD)
            )]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        //#region VendorLocationID
        //[PXDBInt()]
        //[PXUIField(DisplayName = "Vendor Location ID")]
        //public virtual int? VendorLocationID { get; set; }
        //public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        //#endregion

        #region VendorLocation
        [PXDBString(255,IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Location")]
        public virtual string VendorLocationCD { get; set; }
        public abstract class vendorLocationCD : PX.Data.BQL.BqlString.Field<vendorLocationCD> { }
        #endregion

        #region VendorLocationName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Location Name")]
        public virtual string VendorLocationName { get; set; }
        public abstract class vendorLocationName : PX.Data.BQL.BqlString.Field<vendorLocationName> { }
        #endregion

        #region PaymentSameAsDefault
        [PXDBBool()]
        [PXUIField(DisplayName = "Payment Same As Default")]
        public virtual bool? PaymentSameAsDefault { get; set; }
        public abstract class paymentSameAsDefault : PX.Data.BQL.BqlBool.Field<paymentSameAsDefault> { }
        #endregion

        #region PaymentMethod
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Method")]
        [PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
                            Where<PaymentMethod.useForAP, Equal<True>,
                            And<PaymentMethod.isActive, Equal<True>>>>),
                            DescriptionField = typeof(PaymentMethod.descr))]
        public virtual string PaymentMethod { get; set; }
        public abstract class paymentMethod : PX.Data.BQL.BqlString.Field<paymentMethod> { }
        #endregion

        #region CashAccount
        //[PXDBInt()]
        [PXUIField(DisplayName = "Cash Account")]
        [CashAccount(typeof(Search2<CashAccount.cashAccountID,
                        InnerJoin<PaymentMethodAccount,
                            On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>>>,
                        Where2<Match<Current<AccessInfo.userName>>,
                            And<CashAccount.clearingAccount, Equal<False>,
                            And<PaymentMethodAccount.paymentMethodID, Equal<Current<paymentMethod>>,
                            And<PaymentMethodAccount.useForAP, Equal<True>>>>>>),
                            Visibility = PXUIVisibility.Visible)]
        public virtual int? CashAccount { get; set; }
        public abstract class cashAccount : PX.Data.BQL.BqlInt.Field<cashAccount> { }
        #endregion

        #region Tax Registration ID
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Registration ID")]
        public virtual string TaxRegistrationID { get; set; }
        public abstract class taxRegistrationID : PX.Data.BQL.BqlString.Field<taxRegistrationID> { }
        #endregion

        #region BankID
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank ID")]
        public virtual string BankID { get; set; }
        public abstract class bankID : PX.Data.BQL.BqlString.Field<bankID> { }
        #endregion

        #region BankName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Name")]
        public virtual string BankName { get; set; }
        public abstract class bankName : PX.Data.BQL.BqlString.Field<bankName> { }
        #endregion

        #region BankBranchID
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Branch ID")]
        public virtual string BankBranchID { get; set; }
        public abstract class bankBranchID : PX.Data.BQL.BqlString.Field<bankBranchID> { }
        #endregion

        #region BankBranchName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Branch Name")]
        public virtual string BankBranchName { get; set; }
        public abstract class bankBranchName : PX.Data.BQL.BqlString.Field<bankBranchName> { }
        #endregion

        #region BankAccount
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Account")]
        public virtual string BankAccount { get; set; }
        public abstract class bankAccount : PX.Data.BQL.BqlString.Field<bankAccount> { }
        #endregion

        #region BankAccountName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Account Name")]
        public virtual string BankAccountName { get; set; }
        public abstract class bankAccountName : PX.Data.BQL.BqlString.Field<bankAccountName> { }
        #endregion

        #region Category
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Category")]
        public virtual string Category { get; set; }
        public abstract class category : PX.Data.BQL.BqlString.Field<category> { }
        #endregion

        #region CategoryMappingID
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Category Mapping ID")]
        public virtual string CategoryMappingID { get; set; }
        public abstract class categoryMappingID : PX.Data.BQL.BqlString.Field<categoryMappingID> { }
        #endregion

        #region Error
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error")]
        public virtual string Error { get; set; }
        public abstract class error : PX.Data.BQL.BqlString.Field<error> { }
        #endregion
    }
}