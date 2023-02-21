using System;
using PX.Data;

namespace PX.Objects.EP.AgentFlow.DAC
{
    [Serializable]
    [PXCacheName("KGFlowFinBillingInv")]
    public class KGFlowFinBillingInv : IBqlTable
    {
        #region BranchID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region InvoiceID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Invoice ID")]
        public virtual int? InvoiceID { get; set; }
        public abstract class invoiceID : PX.Data.BQL.BqlInt.Field<invoiceID> { }
        #endregion

        #region HeaderID
        [PXDBInt()]
        [PXUIField(DisplayName = "Header ID")]
        [PXDBDefault(typeof(KGFlowFinBillingAH.headerID))]
        [PXParent(typeof(Select<KGFlowFinBillingAH,
                            Where<KGFlowFinBillingAH.headerID,
                            Equal<Current<headerID>>>>))]
        public virtual int? HeaderID { get; set; }
        public abstract class headerID : PX.Data.BQL.BqlInt.Field<headerID> { }
        #endregion

        #region LineID
        [PXDBInt()]
        [PXUIField(DisplayName = "LineID")]
        public virtual int? LineID { get; set; }
        public abstract class lineID : PX.Data.BQL.BqlInt.Field<lineID> { }
        #endregion

        #region BillType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Type")]
        public virtual string BillType { get; set; }
        public abstract class billType : PX.Data.BQL.BqlString.Field<billType> { }
        #endregion

        #region ItemNo
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item No")]
        public virtual string ItemNo { get; set; }
        public abstract class itemNo : PX.Data.BQL.BqlString.Field<itemNo> { }
        #endregion

        #region InvoiceNo
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Invoice No")]
        public virtual string InvoiceNo { get; set; }
        public abstract class invoiceNo : PX.Data.BQL.BqlString.Field<invoiceNo> { }
        #endregion

        #region InvoiceDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Invoice Date")]
        public virtual DateTime? InvoiceDate { get; set; }
        public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
        #endregion

        #region TaxCode
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Code")]
        public virtual string TaxCode { get; set; }
        public abstract class taxCode : PX.Data.BQL.BqlString.Field<taxCode> { }
        #endregion

        #region IvoKind
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ivo Kind")]
        public virtual string IvoKind { get; set; }
        public abstract class ivoKind : PX.Data.BQL.BqlString.Field<ivoKind> { }
        #endregion

        #region NetAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Net Amount")]
        public virtual Decimal? NetAmount { get; set; }
        public abstract class netAmount : PX.Data.BQL.BqlDecimal.Field<netAmount> { }
        #endregion

        #region TaxAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Amount")]
        public virtual Decimal? TaxAmount { get; set; }
        public abstract class taxAmount : PX.Data.BQL.BqlDecimal.Field<taxAmount> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion

        #region UniformNo
        [PXDBString(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform No")]
        public virtual string UniformNo { get; set; }
        public abstract class uniformNo : PX.Data.BQL.BqlString.Field<uniformNo> { }
        #endregion

        #region UniformName
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform Name")]
        public virtual string UniformName { get; set; }
        public abstract class uniformName : PX.Data.BQL.BqlString.Field<uniformName> { }
        #endregion

        #region Digest
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Digest")]
        public virtual string Digest { get; set; }
        public abstract class digest : PX.Data.BQL.BqlString.Field<digest> { }
        #endregion

        #region TaxableExpense
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Taxable Expense")]
        public virtual Decimal? TaxableExpense { get; set; }
        public abstract class taxableExpense : PX.Data.BQL.BqlDecimal.Field<taxableExpense> { }
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
        [PXUIField(DisplayName = "Created Date Time")]
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
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}