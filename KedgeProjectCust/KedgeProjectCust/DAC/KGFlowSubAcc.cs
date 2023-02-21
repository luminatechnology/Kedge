using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;

namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowSubAcc : IBqlTable
      {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXUnboundDefault("KGFLOWACC")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region AccID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "AccID")]
        public virtual int? AccID { get; set; }
        public abstract class accID : PX.Data.BQL.BqlInt.Field<accID> { }
        #endregion

        #region AccUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccUID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string AccUID { get; set; }
        public abstract class accUID : PX.Data.BQL.BqlString.Field<accUID> { }
        #endregion

        #region ApprovalID
        [PXDBInt()]
        public virtual int? ApprovalID { get; set; }
        public abstract class approvalID : IBqlField { }
        #endregion

        #region DeptName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Dept Name")]
        public virtual string DeptName { get; set; }
        public abstract class deptName : PX.Data.BQL.BqlString.Field<deptName> { }
        #endregion

        #region ProjectName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project Name")]
        public virtual string ProjectName { get; set; }
        public abstract class projectName : PX.Data.BQL.BqlString.Field<projectName> { }
        #endregion

        #region ProjectCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project Code")]
        public virtual string ProjectCode { get; set; }
        public abstract class projectCode : PX.Data.BQL.BqlString.Field<projectCode> { }
        #endregion

        #region Subuid
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Subuid")]
        public virtual string SubUID { get; set; }
        public abstract class subUID : PX.Data.BQL.BqlString.Field<subUID> { }
        #endregion

        #region SubCode
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sub Code")]
        public virtual string SubCode { get; set; }
        public abstract class subCode : PX.Data.BQL.BqlString.Field<subCode> { }
        #endregion

        #region SubName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sub Name")]
        public virtual string SubName { get; set; }
        public abstract class subName : PX.Data.BQL.BqlString.Field<subName> { }
        #endregion

        #region Title
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Title")]
        public virtual string Title { get; set; }
        public abstract class title : PX.Data.BQL.BqlString.Field<title> { }
        #endregion

        #region Num
        [PXDBInt()]
        [PXUIField(DisplayName = "Num")]
        public virtual int? Num { get; set; }
        public abstract class num : PX.Data.BQL.BqlInt.Field<num> { }
        #endregion

        #region AccNo
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acc No")]
        [PXDBDefault(typeof(APInvoice.refNbr))]
        [PXParent(typeof(Select<APInvoice,
                                Where<APInvoice.refNbr,
                                Equal<Current<KGFlowSubAcc.accNo>>>>))]
        public virtual string AccNo { get; set; }
        public abstract class accNo : PX.Data.BQL.BqlString.Field<accNo> { }
        #endregion

        #region Sdate
        [PXDBDate()]
        [PXUIField(DisplayName = "Sdate")]
        public virtual DateTime? Sdate { get; set; }
        public abstract class sdate : PX.Data.BQL.BqlDateTime.Field<sdate> { }
        #endregion

        #region Fdate
        [PXDBDate()]
        [PXUIField(DisplayName = "Fdate")]
        public virtual DateTime? Fdate { get; set; }
        public abstract class fdate : PX.Data.BQL.BqlDateTime.Field<fdate> { }
        #endregion

        #region Stateuid
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stateuid")]
        public virtual string Stateuid { get; set; }
        public abstract class stateuid : PX.Data.BQL.BqlString.Field<stateuid> { }
        #endregion

        #region Message
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Message")]
        public virtual string Message { get; set; }
        public abstract class message : PX.Data.BQL.BqlString.Field<message> { }
        #endregion

        #region SumDedApproveValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sum Ded Approve Value")]
        [PXDefault(TypeCode.Decimal,"0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? SumDedApproveValue { get; set; }
        public abstract class sumDedApproveValue : PX.Data.BQL.BqlDecimal.Field<sumDedApproveValue> { }
        #endregion

        #region NoDedApproveValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "No Ded Approve Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? NoDedApproveValue { get; set; }
        public abstract class noDedApproveValue : PX.Data.BQL.BqlDecimal.Field<noDedApproveValue> { }
        #endregion

        #region DedApproveNoAccValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ded Approve No Acc Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DedApproveNoAccValue { get; set; }
        public abstract class dedApproveNoAccValue : PX.Data.BQL.BqlDecimal.Field<dedApproveNoAccValue> { }
        #endregion

        #region DedApproveAccValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ded Approve Acc Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DedApproveAccValue { get; set; }
        public abstract class dedApproveAccValue : PX.Data.BQL.BqlDecimal.Field<dedApproveAccValue> { }
        #endregion

        #region Ctotal
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ctotal")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Ctotal { get; set; }
        public abstract class ctotal : PX.Data.BQL.BqlDecimal.Field<ctotal> { }
        #endregion

        #region SubAccCreateDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Sub Acc Create Date")]
        public virtual DateTime? SubAccCreateDate { get; set; }
        public abstract class subAccCreateDate : PX.Data.BQL.BqlDateTime.Field<subAccCreateDate> { }
        #endregion
        #region SubmittalDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Sub mittal Date")]
        public virtual DateTime? SubmittalDate { get; set; }
        public abstract class submittalDate : PX.Data.BQL.BqlDateTime.Field<submittalDate> { }
        #endregion
        


        #region IsProjectSubmittal
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Project Submittal")]
        public virtual bool? IsProjectSubmittal { get; set; }
        public abstract class isProjectSubmittal : PX.Data.BQL.BqlBool.Field<isProjectSubmittal> { }
        #endregion

        #region Total
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Total")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Total { get; set; }
        public abstract class total : PX.Data.BQL.BqlDecimal.Field<total> { }
        #endregion

        #region Tax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Tax { get; set; }
        public abstract class tax : PX.Data.BQL.BqlDecimal.Field<tax> { }
        #endregion

        #region Realpay
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Realpay")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Realpay { get; set; }
        public abstract class realpay : PX.Data.BQL.BqlDecimal.Field<realpay> { }
        #endregion

        #region Reserve
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Reserve")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Reserve { get; set; }
        public abstract class reserve : PX.Data.BQL.BqlDecimal.Field<reserve> { }
        #endregion

        #region DedApprove
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ded Approve")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DedApprove { get; set; }
        public abstract class dedApprove : PX.Data.BQL.BqlDecimal.Field<dedApprove> { }
        #endregion

        #region AddApprove
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Add Approve")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AddApprove { get; set; }
        public abstract class addApprove : PX.Data.BQL.BqlDecimal.Field<addApprove> { }
        #endregion

        #region PaymentProcessMode
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Process Mode")]
        public virtual string PaymentProcessMode { get; set; }
        public abstract class paymentProcessMode : PX.Data.BQL.BqlString.Field<paymentProcessMode> { }
        #endregion

        #region ResTax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Res Tax")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? ResTax { get; set; }
        public abstract class resTax : PX.Data.BQL.BqlDecimal.Field<resTax> { }
        #endregion

        #region AccCategory
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acc Category")]
        public virtual string AccCategory { get; set; }
        public abstract class accCategory : PX.Data.BQL.BqlString.Field<accCategory> { }
        #endregion

        #region Score1
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score1")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Score1 { get; set; }
        public abstract class score1 : PX.Data.BQL.BqlDecimal.Field<score1> { }
        #endregion

        #region Score2
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score2")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Score2 { get; set; }
        public abstract class score2 : PX.Data.BQL.BqlDecimal.Field<score2> { }
        #endregion

        #region Score3
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score3")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Score3 { get; set; }
        public abstract class score3 : PX.Data.BQL.BqlDecimal.Field<score3> { }
        #endregion

        #region Comment1
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Comment1")]
        public virtual string Comment1 { get; set; }
        public abstract class comment1 : PX.Data.BQL.BqlString.Field<comment1> { }
        #endregion

        #region Comment2
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Comment2")]
        public virtual string Comment2 { get; set; }
        public abstract class comment2 : PX.Data.BQL.BqlString.Field<comment2> { }
        #endregion

        #region Comment3
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Comment3")]
        public virtual string Comment3 { get; set; }
        public abstract class comment3 : PX.Data.BQL.BqlString.Field<comment3> { }
        #endregion

        #region Level
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Level")]
        public virtual string Level { get; set; }
        public abstract class level : PX.Data.BQL.BqlString.Field<level> { }
        #endregion

        #region SumScore
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sum Score")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? SumScore { get; set; }
        public abstract class sumScore : PX.Data.BQL.BqlDecimal.Field<sumScore> { }
        #endregion

        #region IsSettlementAcc
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Settlement Acc")]
        public virtual bool? IsSettlementAcc { get; set; }
        public abstract class isSettlementAcc : PX.Data.BQL.BqlBool.Field<isSettlementAcc> { }
        #endregion

        #region FlowInitiator
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Flow Initiator")]
        public virtual string FlowInitiator { get; set; }
        public abstract class flowInitiator : PX.Data.BQL.BqlString.Field<flowInitiator> { }
        #endregion

        #region FlowPaymentCategory
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Flow Payment Category")]
        public virtual string FlowPaymentCategory { get; set; }
        public abstract class flowPaymentCategory : PX.Data.BQL.BqlString.Field<flowPaymentCategory> { }
        #endregion

        #region ClassifyCode
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ClassifyCode")]
        public virtual string ClassifyCode { get; set; }
        public abstract class classifyCode : PX.Data.BQL.BqlString.Field<classifyCode> { }
        #endregion

        #region RvnSumAddAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Rvn Sum Add Amount")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RvnSumAddAmount { get; set; }
        public abstract class rvnSumAddAmount : PX.Data.BQL.BqlDecimal.Field<rvnSumAddAmount> { }
        #endregion

        #region RvnTotalExTax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Rvn Total Ex Tax")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RvnTotalExTax { get; set; }
        public abstract class rvnTotalExTax : PX.Data.BQL.BqlDecimal.Field<rvnTotalExTax> { }
        #endregion

        #region BudAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Bud Amount")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? BudAmount { get; set; }
        public abstract class budAmount : PX.Data.BQL.BqlDecimal.Field<budAmount> { }
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
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
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