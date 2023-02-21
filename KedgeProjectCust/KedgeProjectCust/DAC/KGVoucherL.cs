using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;

namespace Kedge.DAC
{
    [Serializable]
    public class KGVoucherL : IBqlTable
    {
        #region VoucherLineID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "VoucherLineID")]
        public virtual int? VoucherLineID { get; set; }
        public abstract class voucherLineID : IBqlField { }
        #endregion

        #region VoucherHeaderID
        [PXDBInt()]
        [PXUIField(DisplayName = "VoucherHeaderID", Required = true)]
        [PXDBDefault(typeof(KGVoucherH.voucherHeaderID))]
        [PXParent(typeof(Select<KGVoucherH,
                            Where<KGVoucherH.voucherHeaderID,
                            Equal<Current<KGVoucherL.voucherHeaderID>>>>))]
        public virtual int? VoucherHeaderID { get; set; }
        public abstract class voucherHeaderID : IBqlField { }
        #endregion

        #region ItemNo
        [PXDBInt()]
        [PXUIField(DisplayName = "Item No")]
        public virtual int? ItemNo { get; set; }
        public abstract class itemNo : PX.Data.BQL.BqlInt.Field<itemNo> { }
        #endregion

        #region KGVoucherType
        [PXDBString(10)]
        [PXUIField(DisplayName = "KG Voucher Type")]
        public virtual string KGVoucherType { get; set; }
        public abstract class kGVoucherType : PX.Data.BQL.BqlString.Field<kGVoucherType> { }
        #endregion

        #region VoucherKey
        [PXDBString(11, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Voucher Key")]
        public virtual string VoucherKey { get; set; }
        public abstract class voucherKey : PX.Data.BQL.BqlString.Field<voucherKey> { }
        #endregion

        #region VoucherNo
        [PXDBString(11, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Voucher No")]
        public virtual string VoucherNo { get; set; }
        public abstract class voucherNo : PX.Data.BQL.BqlString.Field<voucherNo> { }
        #endregion

        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project ID", Required = true)]
        [PXSelector(typeof(Search<Contract.contractID>),
            SubstituteKey = typeof(Contract.contractCD),
            DescriptionField = typeof(Contract.description))]
        [PXDefault(typeof(Current<KGVoucherH.contractID>))]

        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region ContractCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contract CD")]
        [PXDefault(typeof(Search<Contract.contractCD,
            Where<Contract.contractID,Equal<Current<contractID>>>>))]
        public virtual string ContractCD { get; set; }
        public abstract class contractCD : PX.Data.BQL.BqlString.Field<contractCD> { }
        #endregion

        #region AccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account ID")]
        [PXSelector(typeof(Search<Account.accountID>),
                typeof(Account.accountCD),
                typeof(Account.description),
            DescriptionField = typeof(Account.description),
            SubstituteKey = typeof(Account.accountCD))]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region AccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account CD")]
        [PXDefault(typeof(Search<Account.accountCD,
            Where<Account.accountID,Equal<Current<accountID>>>>))]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
        #endregion

        #region AccountDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Desc")]
        [PXDefault(typeof(Search<Account.description,
            Where<Account.accountID,Equal<Current<accountID>>>>))]
        public virtual string AccountDesc { get; set; }
        public abstract class accountDesc : PX.Data.BQL.BqlString.Field<accountDesc> { }
        #endregion

        #region Digest
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Digest")]
        public virtual string Digest { get; set; }
        public abstract class digest : PX.Data.BQL.BqlString.Field<digest> { }
        #endregion

        #region Cd
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cd")]
        [PXStringList(new string[] {"C","D" },
            new string[] { "C.¶U", "D.­É" })]
        public virtual string Cd { get; set; }
        public abstract class cd : PX.Data.BQL.BqlString.Field<cd> { }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt")]
        public virtual Decimal? Amt { get; set; }
        public abstract class amt : PX.Data.BQL.BqlDecimal.Field<amt> { }
        #endregion

        #region VendorType
        [PXDBString(2, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Type")]
        public virtual string VendorType { get; set; }
        public abstract class vendorType : PX.Data.BQL.BqlString.Field<vendorType> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor ID")]
        [PXDefault(typeof(Current<KGVoucherH.vendorID>))]
        [PXSelector(typeof(Search<BAccount.bAccountID>),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(BAccount.acctCD),
            DescriptionField = typeof(BAccount.acctName))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor CD")]
        [PXSelector(typeof(Search<BAccount.acctCD>),
            DescriptionField = typeof(BAccount.acctName))]
        public virtual string VendorCD { get; set; }
        public abstract class vendorCD : PX.Data.BQL.BqlString.Field<vendorCD> { }
        #endregion

        #region DueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Due Date")]
        public virtual DateTime? DueDate { get; set; }
        public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        //2019/07/15 ADD
        #region BillPaymentID
        [PXDBInt()]
        [PXUIField(DisplayName = "BillPaymentID")]
        public virtual int? BillPaymentID { get; set; }
        public abstract class billPaymentID : PX.Data.BQL.BqlInt.Field<billPaymentID> { }
        #endregion

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch(typeof(APRegister.branchID))]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion*/
    }
}