using System;
using PX.Data;

namespace PX.Objects.EP.AgentFlow.DAC
{
    [Serializable]
    [PXCacheName("KGFlowFinBillingL")]
    public class KGFlowFinBillingL : IBqlTable
    {
        #region BranchID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region HeaderID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Header ID")]
        [PXDBDefault(typeof(KGFlowFinBillingAH.headerID))]
        [PXParent(typeof(Select<KGFlowFinBillingAH,
                                Where<KGFlowFinBillingAH.headerID,
                                Equal<Current<headerID>>>>))]
        public virtual int? HeaderID { get; set; }
        public abstract class headerID : PX.Data.BQL.BqlInt.Field<headerID> { }
        #endregion

        #region LineID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "LineID")]
        public virtual int? LineID { get; set; }
        public abstract class lineID : PX.Data.BQL.BqlInt.Field<lineID> { }
        #endregion

        #region Company
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company")]
        public virtual string Company { get; set; }
        public abstract class company : PX.Data.BQL.BqlString.Field<company> { }
        #endregion

        #region BillType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Type")]
        public virtual string BillType { get; set; }
        public abstract class billType : PX.Data.BQL.BqlString.Field<billType> { }
        #endregion

        #region GroupNo
        [PXDBInt()]
        [PXUIField(DisplayName = "Group No")]
        public virtual int? GroupNo { get; set; }
        public abstract class groupNo : PX.Data.BQL.BqlInt.Field<groupNo> { }
        #endregion

        #region GroupName
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Group Name")]
        public virtual string GroupName { get; set; }
        public abstract class groupName : PX.Data.BQL.BqlString.Field<groupName> { }
        #endregion

        #region TaxKind
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Kind")]
        public virtual string TaxKind { get; set; }
        public abstract class taxKind : PX.Data.BQL.BqlString.Field<taxKind> { }
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

        #region BelongMClass
        [PXDBString(4, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong MClass")]
        public virtual string BelongMClass { get; set; }
        public abstract class belongMClass : PX.Data.BQL.BqlString.Field<belongMClass> { }
        #endregion

        #region BelongMClassName
        [PXDBString(4000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong MClass Name")]
        public virtual string BelongMClassName { get; set; }
        public abstract class belongMClassName : PX.Data.BQL.BqlString.Field<belongMClassName> { }
        #endregion

        #region BelongSClass
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong SClass")]
        public virtual string BelongSClass { get; set; }
        public abstract class belongSClass : PX.Data.BQL.BqlString.Field<belongSClass> { }
        #endregion

        #region BelongSClassName
        [PXDBString(4000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong SClass Name")]
        public virtual string BelongSClassName { get; set; }
        public abstract class belongSClassName : PX.Data.BQL.BqlString.Field<belongSClassName> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion

        #region TaxAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Amount")]
        public virtual Decimal? TaxAmount { get; set; }
        public abstract class taxAmount : PX.Data.BQL.BqlDecimal.Field<taxAmount> { }
        #endregion

        #region TotalAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Total Amount")]
        public virtual Decimal? TotalAmount { get; set; }
        public abstract class totalAmount : PX.Data.BQL.BqlDecimal.Field<totalAmount> { }
        #endregion

        #region DateDue
        [PXDBDate()]
        [PXUIField(DisplayName = "Date Due")]
        public virtual DateTime? DateDue { get; set; }
        public abstract class dateDue : PX.Data.BQL.BqlDateTime.Field<dateDue> { }
        #endregion

        #region Digest
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Digest")]
        public virtual string Digest { get; set; }
        public abstract class digest : PX.Data.BQL.BqlString.Field<digest> { }
        #endregion

        #region AfmDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Afm Date")]
        public virtual DateTime? AfmDate { get; set; }
        public abstract class afmDate : PX.Data.BQL.BqlDateTime.Field<afmDate> { }
        #endregion

        #region AfmNo
        [PXDBString(9, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Afm No")]
        public virtual string AfmNo { get; set; }
        public abstract class afmNo : PX.Data.BQL.BqlString.Field<afmNo> { }
        #endregion

        #region ApprovalLevelID
        [PXDBInt()]
        [PXUIField(DisplayName = "Approval Level ID")]
        public virtual int? ApprovalLevelID { get; set; }
        public abstract class approvalLevelID : PX.Data.BQL.BqlInt.Field<approvalLevelID> { }
        #endregion

        #region BudgetAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Budget Amount")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? BudgetAmount { get; set; }
        public abstract class budgetAmount : PX.Data.BQL.BqlDecimal.Field<budgetAmount> { }
        #endregion

        #region UseAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Use Amount")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UseAmount { get; set; }
        public abstract class useAmount : PX.Data.BQL.BqlDecimal.Field<useAmount> { }
        #endregion

        #region BudgetPer
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Budget Per")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? BudgetPer { get; set; }
        public abstract class budgetPer : PX.Data.BQL.BqlDecimal.Field<budgetPer> { }
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