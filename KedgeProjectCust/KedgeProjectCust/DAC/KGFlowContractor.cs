using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.PO;
using PX.Objects.GL;
namespace Kedge.DAC
{
    [Serializable]
    public class KGFlowContractor : IBqlTable
    {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        [PXDefault("KGFLOWSUB")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region SubID
        [PXDBIdentity(IsKey = true)]
        public virtual int? SubID { get; set; }
        public abstract class subID : IBqlField { }
        #endregion

        #region SubUID
        [PXDBString(40, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "SubUID")]
        [AutoNumber(typeof(KGFlowContractor.setup), typeof(AccessInfo.businessDate))]
        public virtual string SubUID { get; set; }
        public abstract class subUID : PX.Data.BQL.BqlString.Field<subUID> { }
        #endregion

        #region ApprovalID
        [PXDBInt()]
        public virtual int? ApprovalID { get; set; }
        public abstract class approvalID : IBqlField { }
        #endregion

        #region ProjectName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project Name")]
        public virtual string ProjectName { get; set; }
        public abstract class projectName : PX.Data.BQL.BqlString.Field<projectName> { }
        #endregion

        #region DeptName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Dept Name")]
        public virtual string DeptName { get; set; }
        public abstract class deptName : PX.Data.BQL.BqlString.Field<deptName> { }
        #endregion

        #region ProjectCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project Code")]
        public virtual string ProjectCode { get; set; }
        public abstract class projectCode : PX.Data.BQL.BqlString.Field<projectCode> { }
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

        #region CntTypeName
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cnt Type Name")]
        public virtual string CntTypeName { get; set; }
        public abstract class cntTypeName : PX.Data.BQL.BqlString.Field<cntTypeName> { }
        #endregion

        #region PurchaseOwnerName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Purchase Owner Name")]
        public virtual string PurchaseOwnerName { get; set; }
        public abstract class purchaseOwnerName : PX.Data.BQL.BqlString.Field<purchaseOwnerName> { }
        #endregion

        #region InvNo
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inv No")]
        public virtual string InvNo { get; set; }
        public abstract class invNo : PX.Data.BQL.BqlString.Field<invNo> { }
        #endregion

        #region Title
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Title")]
        public virtual string Title { get; set; }
        public abstract class title : PX.Data.BQL.BqlString.Field<title> { }
        #endregion

        #region GetProjectDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Get Project Date")]
        public virtual DateTime? GetProjectDate { get; set; }
        public abstract class getProjectDate : PX.Data.BQL.BqlDateTime.Field<getProjectDate> { }
        #endregion

        #region Ctotal
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ctotal")]
        public virtual Decimal? Ctotal { get; set; }
        public abstract class ctotal : PX.Data.BQL.BqlDecimal.Field<ctotal> { }
        #endregion

        #region Ctax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ctax")]
        public virtual Decimal? Ctax { get; set; }
        public abstract class ctax : PX.Data.BQL.BqlDecimal.Field<ctax> { }
        #endregion

        #region Camount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Camount")]
        public virtual Decimal? Camount { get; set; }
        public abstract class camount : PX.Data.BQL.BqlDecimal.Field<camount> { }
        #endregion

        #region Message
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Message")]
        public virtual string Message { get; set; }
        public abstract class message : PX.Data.BQL.BqlString.Field<message> { }
        #endregion

        #region PurchaseNo
        [PXDBString(64, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PurchaseNo")]
        public virtual string PurchaseNo { get; set; }
        public abstract class purchaseNo : PX.Data.BQL.BqlString.Field<purchaseNo> { }
        #endregion

        #region MergeNo
        [PXDBString(64, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "MergeNo")]
        public virtual string MergeNo { get; set; }
        public abstract class mergeNo : PX.Data.BQL.BqlString.Field<mergeNo> { }
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
		[Branch(typeof(POOrder.branchID))]
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