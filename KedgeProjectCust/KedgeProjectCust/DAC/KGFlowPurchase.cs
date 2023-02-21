using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CS;
namespace Kedge.DAC
{
    [Serializable]
    public class KGFlowPurchase : IBqlTable
    {
       /*
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWPPM")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion*/

        #region PpmID
        [PXDBIdentity(IsKey =true)]
        public virtual int? PpmID { get; set; }
        public abstract class ppmID : PX.Data.BQL.BqlString.Field<ppmID> { }
        #endregion

        #region PpmUID
        [PXDBString(40, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PpmUID")]
        //[AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string PpmUID { get; set; }
        public abstract class ppmUID : PX.Data.BQL.BqlString.Field<ppmUID> { }
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

        #region CreateUserName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Create User Name")]
        public virtual string CreateUserName { get; set; }
        public abstract class createUserName : PX.Data.BQL.BqlString.Field<createUserName> { }
        #endregion

        #region PurchaseProgramNo
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Purchase Program No")]
        public virtual string PurchaseProgramNo { get; set; }
        public abstract class purchaseProgramNo : PX.Data.BQL.BqlString.Field<purchaseProgramNo> { }
        #endregion

        #region PurchaseProgramName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Purchase Program Name")]
        public virtual string PurchaseProgramName { get; set; }
        public abstract class purchaseProgramName : PX.Data.BQL.BqlString.Field<purchaseProgramName> { }
        #endregion

        #region PayType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pay Type")]
        public virtual string PayType { get; set; }
        public abstract class payType : PX.Data.BQL.BqlString.Field<payType> { }
        #endregion

        #region CostDay1
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Day1")]
        public virtual int? CostDay1 { get; set; }
        public abstract class costDay1 : PX.Data.BQL.BqlInt.Field<costDay1> { }
        #endregion

        #region CostRatio1
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cost Ratio1")]
        public virtual Decimal? CostRatio1 { get; set; }
        public abstract class costRatio1 : PX.Data.BQL.BqlDecimal.Field<costRatio1> { }
        #endregion

        #region CostDay2
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Day2")]
        public virtual int? CostDay2 { get; set; }
        public abstract class costDay2 : PX.Data.BQL.BqlInt.Field<costDay2> { }
        #endregion

        #region CostRatio2
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cost Ratio2")]
        public virtual Decimal? CostRatio2 { get; set; }
        public abstract class costRatio2 : PX.Data.BQL.BqlDecimal.Field<costRatio2> { }
        #endregion

        #region ResPercent
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Res Percent")]
        public virtual Decimal? ResPercent { get; set; }
        public abstract class resPercent : PX.Data.BQL.BqlDecimal.Field<resPercent> { }
        #endregion

        #region PerformanceBondPercent
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Performance Bond Percent")]
        public virtual Decimal? PerformanceBondPercent { get; set; }
        public abstract class performanceBondPercent : PX.Data.BQL.BqlDecimal.Field<performanceBondPercent> { }
        #endregion

        #region WarrantyYearsOf
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Warranty Years Of")]
        public virtual Decimal? WarrantyYearsOf { get; set; }
        public abstract class warrantyYearsOf : PX.Data.BQL.BqlDecimal.Field<warrantyYearsOf> { }
        #endregion

        #region Explain
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Explain")]
        public virtual string Explain { get; set; }
        public abstract class explain : PX.Data.BQL.BqlString.Field<explain> { }
        #endregion

        #region Message
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Message")]
        public virtual string Message { get; set; }
        public abstract class message : PX.Data.BQL.BqlString.Field<message> { }
        #endregion

        #region Stateuid
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stateuid")]
        public virtual string Stateuid { get; set; }
        public abstract class stateuid : PX.Data.BQL.BqlString.Field<stateuid> { }
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
        /*
        #region BranchID
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
		[Branch()]
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