using System;
using Kedge.DAC;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using EP.Util;

namespace EP.DAC
{
    [Serializable]
    [PXCacheName("KGExpenseVoucher")]
    public class KGExpenseVoucher : IBqlTable
    {
        #region VoucherID
        [PXDBIdentity(/*IsKey = true*/)]
        public virtual int? VoucherID { get; set; }
        public abstract class voucherID : PX.Data.BQL.BqlInt.Field<voucherID> { }
        #endregion

        #region BranchID
        [Branch(typeof(EPExpenseClaim.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region EPRefNbr
        [PXDBString(15, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "EPRefNbr")]
        [PXDBDefault(typeof(EPExpenseClaim.refNbr))]
        [PXParent(typeof(Select<EPExpenseClaim,
                            Where<EPExpenseClaim.refNbr,
                            Equal<Current<epRefNbr>>>>))]
        public virtual string EPRefNbr { get; set; }
        public abstract class epRefNbr : PX.Data.BQL.BqlString.Field<epRefNbr> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(EPExpenseClaimExt.usrLineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region ApprovalLevelID
        [PXDBInt()]
        [PXUIField(DisplayName = "Approval Level ID", Required = true)]
        [PXSelector(typeof(Search2<KGBudApproveLevel.budLevelID,
            InnerJoin<KGBudGroup,
                On<KGBudGroup.budGroupNO, Equal<KGBudApproveLevel.groupNo>,
                And<KGBudGroup.branchID, Equal<KGBudApproveLevel.branchID>>>>,
            Where<KGBudApproveLevel.branchID, Equal<Current<AccessInfo.branchID>>,
                And<KGBudApproveLevel.budgetYear, Equal<Current<EPExpenseClaimExt.usrFinancialYear>>,
                And<KGBudApproveLevel.ePDocType, Equal<Current<EPExpenseClaimExt.usrDocType>>>>>>),
            typeof(KGBudApproveLevel.billingType),
            typeof(KGBudGroup.budGroupName),
            typeof(KGBudApproveLevel.subGroupNo),
            typeof(KGBudApproveLevel.levelDesc),
            typeof(KGBudApproveLevel.remark),
            SubstituteKey = typeof(KGBudApproveLevel.approvalLevelID))]
        public virtual int? ApprovalLevelID { get; set; }
        public abstract class approvalLevelID : PX.Data.BQL.BqlInt.Field<approvalLevelID> { }
        #endregion

        #region AccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account ID", Required = true)]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]

        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region SubID
        [SubAccount(typeof(accountID), DisplayName = "SubID", Visibility = PXUIVisibility.Visible, Required = true)]
        [PXUIRequired(typeof(Where<Current<EPExpenseClaimExt.usrDocType>, Equal<EPStringList.EPDocType.glb>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? SubID { get; set; }
        public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
        #endregion

        #region TranDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Tran Date", Required = true)]
        [PXDefault(typeof(Current<EPExpenseClaim.docDate>))]
        public virtual DateTime? TranDate { get; set; }
        public abstract class tranDate : PX.Data.BQL.BqlDateTime.Field<tranDate> { }
        #endregion

        #region DebitAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Debit Amt")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DebitAmt { get; set; }
        public abstract class debitAmt : PX.Data.BQL.BqlDecimal.Field<debitAmt> { }
        #endregion

        #region CreditAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Credit Amt")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CreditAmt { get; set; }
        public abstract class creditAmt : PX.Data.BQL.BqlDecimal.Field<creditAmt> { }
        #endregion

        #region TranDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tran Desc", Required = true)]
        [PXDefault]
        public virtual string TranDesc { get; set; }
        public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }
        #endregion

        #region ProjectID

        [ActiveProjectOrContractForGLAttribute(AccountFieldType = typeof(accountID))]

        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        [ProjectDefault()]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region IsSendFlow
        [PXDBBool()]
        [PXUIField(DisplayName = "IsSendFlow")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsSendFlow { get; set; }
        public abstract class isSendFlow : IBqlField { }
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