using System;
using PX.Data;

namespace PX.Objects.EP.AgentFlow.DAC
{
    [Serializable]
    [PXCacheName("KGFlowFinBillingNote")]
    public class KGFlowFinBillingNote : IBqlTable
    {
        #region BranchID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region BillingNoteID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Billing Note ID")]
        public virtual int? BillingNoteID { get; set; }
        public abstract class billingNoteID : PX.Data.BQL.BqlInt.Field<billingNoteID> { }
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

        #region BillType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Type")]
        public virtual string BillType { get; set; }
        public abstract class billType : PX.Data.BQL.BqlString.Field<billType> { }
        #endregion

        #region VendorType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Type")]
        public virtual string VendorType { get; set; }
        public abstract class vendorType : PX.Data.BQL.BqlString.Field<vendorType> { }
        #endregion

        #region UniformNo
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform No")]
        public virtual string UniformNo { get; set; }
        public abstract class uniformNo : PX.Data.BQL.BqlString.Field<uniformNo> { }
        #endregion

        #region UniformName
        [PXDBString(4000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform Name")]
        public virtual string UniformName { get; set; }
        public abstract class uniformName : PX.Data.BQL.BqlString.Field<uniformName> { }
        #endregion

        #region NetAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Net Amount")]
        public virtual Decimal? NetAmount { get; set; }
        public abstract class netAmount : PX.Data.BQL.BqlDecimal.Field<netAmount> { }
        #endregion

        #region DateDue
        [PXDBDate()]
        [PXUIField(DisplayName = "Date Due")]
        public virtual DateTime? DateDue { get; set; }
        public abstract class dateDue : PX.Data.BQL.BqlDateTime.Field<dateDue> { }
        #endregion

        #region PostageAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Postage Amt")]
        public virtual Decimal? PostageAmt { get; set; }
        public abstract class postageAmt : PX.Data.BQL.BqlDecimal.Field<postageAmt> { }
        #endregion

        #region Remark
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
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