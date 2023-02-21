using System;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;

namespace Kedge.DAC
{
  [Serializable]
  public class KGDailyReport : IBqlTable
  {
        #region DailyReportID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Daily Report ID")]
        [PXReferentialIntegrityCheck(CheckPoint = CheckPoint.OnPersisting)]
        public virtual int? DailyReportID { get; set; }
        public abstract class dailyReportID : IBqlField { }
        #endregion
        #region DailyReportCD
        [PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCC")]
        [PXDefault()]
        [PXSelector(typeof(Search2<dailyReportCD,InnerJoin<PMProject,On<KGDailyReport.contractID,Equal<PMProject.contractID>>>>), typeof(dailyReportCD),typeof(workDate), typeof(contractID), typeof(createdByID), typeof(PMProject.description))]
        [AutoNumber(typeof(KGSetUp.kGDailyReportNumbering), typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Daily Report CD")]
        public virtual String DailyReportCD { get; set; }
        public abstract class dailyReportCD : IBqlField { }
        #endregion
        #region ContractID
        /*
        [PXDBInt()]
        
        [PXSelector(typeof(Search2<PMProject.contractID,
            LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
            LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
            Equal<PMProject.contractID>>>>,
            Where<PMProject.baseType, Equal<CTPRType.project>,
             And<PMProject.status, Equal<ProjectStatus.active>,
             And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>>)
            , typeof(PMProject.contractCD), typeof(PMProject.description),
            typeof(Customer.acctName), typeof(PMProject.status),
            typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false,DescriptionField = typeof(PMProject.description))]*/
        [ProjectBaseExt]
        [PXUIField(DisplayName = "Project", Required = true)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region WorkDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Work Date", Required = true)]
        public virtual DateTime? WorkDate { get; set; }
        public abstract class workDate : IBqlField { }
        #endregion

        #region WeatherAM
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Weather AM", Required = true)]
        [PXStringList(
               new string[]
               {
                 WeatherType.Sunny, 
                 WeatherType.Cloudy,
                 WeatherType.Rainy,
                 WeatherType.Typhoon
               },
               new string[]
               {
                 WeatherType.SunnyUI, 
                 WeatherType.CloudyUI,
                 WeatherType.RainyUI,
                 WeatherType.TyphoonUI
               })]  
        public virtual string WeatherAM { get; set; }
        public abstract class weatherAM : IBqlField { }
        #endregion

        #region WeatherPM
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Weather PM", Required = true)]
        [PXStringList(
               new string[]
               {
                 WeatherType.Sunny, 
                 WeatherType.Cloudy,
                 WeatherType.Rainy,
                 WeatherType.Typhoon
               },
               new string[]
               {
                 WeatherType.SunnyUI, 
                 WeatherType.CloudyUI,
                 WeatherType.RainyUI,
                 WeatherType.TyphoonUI
               })]
        public virtual string WeatherPM { get; set; }
        public abstract class weatherPM : IBqlField { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : IBqlField { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : IBqlField { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : IBqlField { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : IBqlField { }
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
        public abstract class tstamp : IBqlField { }
        #endregion

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the document belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
        [PX.Objects.GL.Branch()]
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

    public class WeatherType
  {
    public const string Sunny = "S";
    public const string Cloudy = "C";
    public const string Rainy = "R";
    public const string Typhoon = "T";
    public const string SunnyUI = "Sunny";
    public const string CloudyUI = "Cloudy";
    public const string RainyUI = "Rainy";
    public const string TyphoonUI = "Typhoon";
  }
}