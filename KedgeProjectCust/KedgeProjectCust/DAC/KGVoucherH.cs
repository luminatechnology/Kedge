using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.Objects.GL;

namespace Kedge.DAC
{
    /**
     * ===2021/11/22 :0012269 === Althea
     * CAmt & DAmt 改為存DB decimal(19,4)
     **/
    [Serializable]
    public class KGVoucherH : IBqlTable
    {
        #region Selected
        public abstract class selected : IBqlField
        { }
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region VoucherHeaderID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "VoucherHeaderID")]
        public virtual int? VoucherHeaderID { get; set; }
        public abstract class voucherHeaderID : IBqlField { }
        #endregion

        #region DocType
        [PXDBString(3, IsKey = true, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Doc Type")]
        [PXStringList(
            new string[] {"INV", "PPM", "ADR" },
            new string[] { "計價", "預付款", "借方調整"})]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "RefNbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region VoucherKey
        [PXDBString(11, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Voucher Key")]
        public virtual string VoucherKey { get; set; }
        public abstract class voucherKey : PX.Data.BQL.BqlString.Field<voucherKey> { }
        #endregion

        #region VoucherDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Voucher Date")]
        public virtual DateTime? VoucherDate { get; set; }
        public abstract class voucherDate : PX.Data.BQL.BqlString.Field<voucherDate> { }
        #endregion

        #region VoucherNo
        [PXDBString(11, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Voucher No")]
        public virtual string VoucherNo { get; set; }
        public abstract class voucherNo : PX.Data.BQL.BqlString.Field<voucherNo> { }
        #endregion

        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Contract ID")]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlString.Field<contractID> { }
        #endregion

        #region ContractCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<Contract.contractCD>),
            DescriptionField =typeof(Contract.description))]
        [PXUIField(DisplayName = "Contract CD")]
        public virtual string ContractCD { get; set; }
        public abstract class contractCD : PX.Data.BQL.BqlString.Field<contractCD> { }
        #endregion

        #region ContractDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contract Desc")]
        public virtual string ContractDesc { get; set; }
        public abstract class contractDesc : PX.Data.BQL.BqlString.Field<contractDesc> { }
        #endregion

        #region VendorType
        [PXDBString(2, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Type")]
        [PXStringList(new string[] {"E","V" },
            new string[] { "員工", "供應商" })]
        public virtual string VendorType { get; set; }
        public abstract class vendorType : PX.Data.BQL.BqlString.Field<vendorType> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor ID")]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlString.Field<vendorID> { }
        #endregion

        #region VendorCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor CD")]
        [PXSelector(typeof(Search<BAccount.acctCD>),
            DescriptionField = typeof(BAccount.acctName))]
        public virtual string VendorCD { get; set; }
        public abstract class vendorCD : PX.Data.BQL.BqlString.Field<vendorCD> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        [PXStringList(new string[] { "C","U", "P" },
            new string[] { "ERP作廢","未拋轉", "已拋轉" })]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
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

        //2019/07/17 ADD UnBound
        //2021/11/22 改為入DB
        #region CAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "C Amt",IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal,"0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? CAmt { get; set; }
        public abstract class cAmt : PX.Data.BQL.BqlDecimal.Field<cAmt> { }
        #endregion

        #region DAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "D Amt",IsReadOnly =true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DAmt { get; set; }
        public abstract class dAmt : PX.Data.BQL.BqlDecimal.Field<dAmt> { }
        #endregion

        //2019/07/18 ADD UnBound
        #region PONbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "PONbr")]
        public virtual string PONbr { get; set; }
        public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr> { }
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

        //Unbound
        #region APStatus 
        [PXString()]
        [PXUIField(DisplayName = "APStatus")]
        [PXDefault(typeof(Search<APRegister.status,
            Where<APRegister.refNbr,Equal<Current<refNbr>>>>),
            PersistingCheck =PXPersistingCheck.Nothing)]
        [APDocStatus.List]
        public virtual string APStatus { get; set; }
        public abstract class aPStatus : PX.Data.BQL.BqlString.Field<aPStatus> { }
        #endregion

        #region DocDate 
        [PXDate()]
        [PXUIField(DisplayName = "DocDate")]
        [PXDefault(typeof(Search<APRegister.docDate,
            Where<APRegister.refNbr,Equal<Current<refNbr>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region UsrAccConfirmNbr
        [PXString]
        [PXUIField(DisplayName = "UsrAccConfirmNbr")]
        public virtual string UsrAccConfirmNbr { get; set; }
        public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
        #endregion

    }
}