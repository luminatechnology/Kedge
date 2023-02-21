using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowChangeManagement : IBqlTable
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

        #region BcaID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "BcaID")]
        public virtual int? BcaID { get; set; }
        public abstract class bcaID : PX.Data.BQL.BqlInt.Field<bcaID> { }
        #endregion

        #region Bcauid
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "BcaUID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string BcaUID { get; set; }
        public abstract class bcaUID : PX.Data.BQL.BqlString.Field<bcaUID> { }
        #endregion

        #region ApprovalID
        [PXDBInt()]
        public virtual int? ApprovalID { get; set; }
        public abstract class approvalID : IBqlField { }
        #endregion

        #region DeptName
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "OrderChangeRefNbr")]
        public virtual string OrderChangeRefNbr { get; set; }
        public abstract class orderChangeRefNbr : PX.Data.BQL.BqlString.Field<orderChangeRefNbr> { }
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

        #region ApplicationUser
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Application User")]
        public virtual string ApplicationUser { get; set; }
        public abstract class applicationUser : PX.Data.BQL.BqlString.Field<applicationUser> { }
        #endregion

        #region ApplicationDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Application Date")]
        public virtual DateTime? ApplicationDate { get; set; }
        public abstract class applicationDate : PX.Data.BQL.BqlDateTime.Field<applicationDate> { }
        #endregion

        #region Num
        [PXDBInt()]
        [PXUIField(DisplayName = "Num")]
        public virtual int? Num { get; set; }
        public abstract class num : PX.Data.BQL.BqlInt.Field<num> { }
        #endregion

        #region ChangTitle
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Chang Title")]
        public virtual string ChangTitle { get; set; }
        public abstract class changTitle : PX.Data.BQL.BqlString.Field<changTitle> { }
        #endregion

        #region EffectReason
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Effect Reason")]
        public virtual string EffectReason { get; set; }
        public abstract class effectReason : PX.Data.BQL.BqlString.Field<effectReason> { }
        #endregion

        #region Cmemo
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cmemo")]
        public virtual string Cmemo { get; set; }
        public abstract class cmemo : PX.Data.BQL.BqlString.Field<cmemo> { }
        #endregion

        #region StateUID
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "State UID")]
        public virtual string StateUID { get; set; }
        public abstract class stateUID : PX.Data.BQL.BqlString.Field<stateUID> { }
        #endregion

        #region Message
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Message")]
        public virtual string Message { get; set; }
        public abstract class message : PX.Data.BQL.BqlString.Field<message> { }
        #endregion

        #region CostControlUser
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CostControlUser")]
        public virtual string CostControlUser { get; set; }
        public abstract class costControlUser : PX.Data.BQL.BqlString.Field<costControlUser> { }
        #endregion

        #region AccountUser
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccountUser")]
        public virtual string AccountUser { get; set; }
        public abstract class accountUser : PX.Data.BQL.BqlString.Field<accountUser> { }
        #endregion

        #region InitialProfitMargins
        [PXDBDecimal()]
        [PXUIField(DisplayName = "InitialProfitMargins")]
        public virtual Decimal? InitialProfitMargins { get; set; }
        public abstract class initialProfitMargins : PX.Data.BQL.BqlDecimal.Field<initialProfitMargins> { }
        #endregion

        #region CurrentProfitMargins
        [PXDBDecimal()]
        [PXUIField(DisplayName = "CurrentProfitMargins")]
        public virtual Decimal? CurrentProfitMargins { get; set; }
        public abstract class currentProfitMargins : PX.Data.BQL.BqlDecimal.Field<currentProfitMargins> { }
        #endregion

        #region ForecastProfitMargins
        [PXDBDecimal()]
        [PXUIField(DisplayName = "ForecastProfitMargins")]
        public virtual Decimal? ForecastProfitMargins { get; set; }
        public abstract class forecastProfitMargins : PX.Data.BQL.BqlDecimal.Field<forecastProfitMargins> { }
        #endregion

        #region QualityImprovement
        [PXDBBool()]
        [PXUIField(DisplayName = "Quality Improvement")]
        public virtual bool? QualityImprovement { get; set; }
        public abstract class qualityImprovement : PX.Data.BQL.BqlBool.Field<qualityImprovement> { }
        #endregion

        #region ClassID
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Class ID")]
        public virtual string ClassID { get; set; }
        public abstract class classID : PX.Data.BQL.BqlString.Field<classID> { }
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
        [PXNote(ForceFileCorrection = false)]
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
      }
}