using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;

namespace Kedge.DAC
{
  [Serializable]
  public class KGFlowSubAccDetail : IBqlTable
  {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWADU")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region Adid
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Adid")]
        public virtual int? AdID { get; set; }
        public abstract class adID : PX.Data.BQL.BqlInt.Field<adID> { }
        #endregion

        #region AdUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AdUID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string AdUID { get; set; }
        public abstract class adUID : PX.Data.BQL.BqlString.Field<adUID> { }
        #endregion

        #region AccUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccUID")]
        [PXDBDefault(typeof(KGFlowSubAcc.accUID))]
        [PXParent(typeof(Select<KGFlowSubAcc,
                            Where<KGFlowSubAcc.accUID,
                            Equal<Current<KGFlowSubAccDetail.accUID>>>>))]
        public virtual string AccUID { get; set; }
        public abstract class accUID : PX.Data.BQL.BqlString.Field<accUID> { }
        #endregion

        #region Sno
        [PXDBInt()]
        [PXUIField(DisplayName = "Sno")]
        public virtual int? Sno { get; set; }
        public abstract class sno : PX.Data.BQL.BqlInt.Field<sno> { }
        #endregion

        #region FundsName
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Funds Name")]
        public virtual string FundsName { get; set; }
        public abstract class fundsName : PX.Data.BQL.BqlString.Field<fundsName> { }
        #endregion

        #region ReserveDedReturnReserve
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Reserve Ded Return Reserve")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? ReserveDedReturnReserve { get; set; }
        public abstract class reserveDedReturnReserve : PX.Data.BQL.BqlDecimal.Field<reserveDedReturnReserve> { }
        #endregion

        #region DedReturnPrePay
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ded Return Pre Pay")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DedReturnPrePay { get; set; }
        public abstract class dedReturnPrePay : PX.Data.BQL.BqlDecimal.Field<dedReturnPrePay> { }
        #endregion

        #region Ven2Value
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ven2 Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Ven2Value { get; set; }
        public abstract class ven2Value : PX.Data.BQL.BqlDecimal.Field<ven2Value> { }
        #endregion

        #region DedValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ded Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DedValue { get; set; }
        public abstract class dedValue : PX.Data.BQL.BqlDecimal.Field<dedValue> { }
        #endregion

        #region AddValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Add Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AddValue { get; set; }
        public abstract class addValue : PX.Data.BQL.BqlDecimal.Field<addValue> { }
        #endregion

        #region TaxValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? TaxValue { get; set; }
        public abstract class taxValue : PX.Data.BQL.BqlDecimal.Field<taxValue> { }
        #endregion

        #region RealPayValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Real Pay Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RealPayValue { get; set; }
        public abstract class realPayValue : PX.Data.BQL.BqlDecimal.Field<realPayValue> { }
        #endregion

        #region InvoiceValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Invoice Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? InvoiceValue { get; set; }
        public abstract class invoiceValue : PX.Data.BQL.BqlDecimal.Field<invoiceValue> { }
        #endregion

        #region WageReport
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Wage Report")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? WageReport { get; set; }
        public abstract class wageReport : PX.Data.BQL.BqlDecimal.Field<wageReport> { }
        #endregion

        #region WithholdingTax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Withholding Tax")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? WithholdingTax { get; set; }
        public abstract class withholdingTax : PX.Data.BQL.BqlDecimal.Field<withholdingTax> { }
        #endregion

        #region WarrantyCash
        [PXDBBool()]
        [PXUIField(DisplayName = "Warranty Cash")]
        public virtual bool? WarrantyCash { get; set; }
        public abstract class warrantyCash : PX.Data.BQL.BqlBool.Field<warrantyCash> { }
        #endregion

        #region WarrantyTicket
        [PXDBBool()]
        [PXUIField(DisplayName = "Warranty Ticket")]
        public virtual bool? WarrantyTicket { get; set; }
        public abstract class warrantyTicket : PX.Data.BQL.BqlBool.Field<warrantyTicket> { }
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

        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID>
        {
        }
        protected Guid? _NoteID;

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
	    [PXNote()]
        public virtual Guid? NoteID
        {
            get
            {
                return this._NoteID;
            }
            set
            {
                this._NoteID = value;
            }
        }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
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