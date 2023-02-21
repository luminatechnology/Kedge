using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.PM;

namespace KedgeFinCustDev.FIN.DAC
{
    [Serializable]
    [PXCacheName("KG Project History By Period")]
    public class KGProjHistoryByPeriod : IBqlTable
    {
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected", Visible = false)]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region AccountID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Account")]
        [PXSelector(typeof(Account.accountID), SubstituteKey = typeof(Account.accountCD), DescriptionField = typeof(Account.description), ValidateValue = false)]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region BranchID
        [Branch(IsKey = true, ValidateValue = false)]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region ProjectID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Project")]
        [PXSelector(typeof(PMProject.contractID), SubstituteKey = typeof(PMProject.contractCD), DescriptionField = typeof(PMProject.description), ValidateValue = false)]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region LedgerID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Ledger")]
        [PXSelector(typeof(Ledger.ledgerID), SubstituteKey = typeof(Ledger.ledgerCD), DescriptionField = typeof(Ledger.descr), ValidateValue = false)]
        public virtual int? LedgerID { get; set; }
        public abstract class ledgerID : PX.Data.BQL.BqlInt.Field<ledgerID> { }
        #endregion

        #region SubID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Sub")]
        [PXSelector(typeof(Sub.subID), SubstituteKey = typeof(Sub.subCD), DescriptionField = typeof(Sub.description), ValidateValue = false)]
        public virtual int? SubID { get; set; }
        public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
        #endregion

        #region FinPeriodID
        [FinPeriodID(branchSourceType: typeof(branchID),
                     headerMasterFinPeriodIDType: typeof(Batch.tranPeriodID), IsKey = true)]
        [PXUIField(DisplayName = "Fin Period")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region FinYear
        [PXDBString(4, IsFixed = true)]
        public virtual string FinYear { get; set; }
        public abstract class finYear : PX.Data.BQL.BqlString.Field<finYear> { }
        #endregion

        #region FinYtdBalance
        [PXDBBaseCury(typeof(ledgerID))]
        [PXUIField(DisplayName = "Fin Ytd Balance")]
        public virtual decimal? FinYtdBalance { get; set; }
        public abstract class finYtdBalance : PX.Data.BQL.BqlDecimal.Field<finYtdBalance> { }
        #endregion

        #region AcctType
        [PXDBString(1, IsFixed = true)]
        [AccountType.List()]
        [PXUIField(DisplayName = "Account Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string AcctType { get; set; }
        public abstract class acctType : PX.Data.BQL.BqlString.Field<acctType> { }
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
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    
        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}