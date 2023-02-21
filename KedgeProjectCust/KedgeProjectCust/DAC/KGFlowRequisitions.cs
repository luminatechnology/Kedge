using System;
using PX.Data;
using PX.Objects.RQ;
using Kedge.DAC;
using PX.Objects.CS;
using PX.Objects.GL;
namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowRequisitions : IBqlTable
      {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWREQ")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region ReqID

        [PXDBIdentity(IsKey = true)]
        public virtual int? ReqID { get; set; }
        public abstract class reqID : IBqlField { }
        #endregion

        #region ApprovalID
        [PXDBInt()]
        public virtual int? ApprovalID { get; set; }
        public abstract class approvalID : IBqlField { }
        #endregion

        #region ReqUID
        [PXDBString(40, IsKey = true, IsUnicode = true, InputMask = "")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Requid")]
        public virtual string ReqUID { get; set; }
        public abstract class reqUID : PX.Data.BQL.BqlString.Field<reqUID> { }
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

        #region CreateUserName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Create User Name")]
        public virtual string CreateUserName { get; set; }
        public abstract class createUserName : PX.Data.BQL.BqlString.Field<createUserName> { }
        #endregion

        #region RequisitionsNo
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Requisitions No")]
        [PXDBDefault(typeof(RQRequest.orderNbr))]
        [PXParent(typeof(Select<RQRequest,
                                Where<RQRequest.orderNbr,
                                Equal<Current<KGFlowRequisitions.requisitionsNo>>>>))]
        public virtual string RequisitionsNo { get; set; }
        public abstract class requisitionsNo : PX.Data.BQL.BqlString.Field<requisitionsNo> { }
        #endregion

        #region UsrDefinedName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Defined Name")]
        public virtual string UsrDefinedName { get; set; }
        public abstract class usrDefinedName : PX.Data.BQL.BqlString.Field<usrDefinedName> { }
        #endregion

        #region NeedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Need Date")]
        public virtual DateTime? NeedDate { get; set; }
        public abstract class needDate : PX.Data.BQL.BqlDateTime.Field<needDate> { }
        #endregion

        #region RequisitionsExplain
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Requisitions Explain")]
        public virtual string RequisitionsExplain { get; set; }
        public abstract class requisitionsExplain : PX.Data.BQL.BqlString.Field<requisitionsExplain> { }
        #endregion

        #region Message
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Message")]
        public virtual string Message { get; set; }
        public abstract class message : PX.Data.BQL.BqlString.Field<message> { }
        #endregion

        #region StateUID
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stateuid")]
        public virtual string StateUID { get; set; }
        public abstract class stateUID : PX.Data.BQL.BqlString.Field<stateUID> { }
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
        [Branch(typeof(RQRequest.branchID))]
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