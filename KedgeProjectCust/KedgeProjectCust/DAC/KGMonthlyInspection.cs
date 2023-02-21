using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.PM;
using PX.Data.BQL;
using PX.Objects.EP;
using PX.Objects.CR;

namespace Kedge.DAC
{
    public static class KGMonthInspectionStatuses
    {

        public const string HoldName = "Hold";
        public const string OpenName = "Open";
        public const string PendingName = "Pending";
        public const string CloseName = "Close";
        public const string Hold = "H";
        public const string Open = "N";
        public const string Pending = "P";
        public const string Close = "C";

        public class open : BqlString.Constant<open>
        {
            public open() : base(Open) { }
        }
        public class pending : BqlString.Constant<pending>
        {
            public pending() : base(Pending) { }
        }

        public class close : BqlString.Constant<close>
        {
            public close() : base(Close) { }
        }
        public class hold : BqlString.Constant<hold>
        {
            public hold() : base(Hold) { }
        }
        public class ShortListAttribute : PXStringListAttribute
        {
            public ShortListAttribute()
                : base(
                    new string[] { Hold, Open, Close },
                    new string[] { HoldName, OpenName, CloseName })
            {
            }
        }
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                    new string[] { Hold, Open, Pending, Close },
                    new string[] { HoldName, OpenName, Pending, CloseName })
            {
            }
        }

    }



    public class MonthOfYear
    {

        public static readonly int[] Values =
        {
            1,2,3,4,5,6,7,8,9,10,11,12
        };
        public static readonly string[] Labels =
        {
            "1","2","3","4","5","6","7","8","9","10","11","12"
        };
        public class ListAttribute : PXIntListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }
    }


    public static class KGMMonthlyInspectionEvaluations
    {
        public const string KMI_A_PLUS = "A+";
        public const string KMI_A = "A";
        public const string KMI_B_PLUS = "B+";
        public const string KMI_B = "B";
        public const string KMI_C = "C";
        public const string KMI_D = "D";
        public const string KMI_A_PLUS_Str = "特優!自主管理、籌劃等，整體狀況特優";
        public const string KMI_A_Str = "高於標準!自主管理、籌劃等，整體狀況優良";
        public const string KMI_B_PLUS_Str = "符合標準!致力於自主管理，符合公司要求";
        public const string KMI_B_Str = "低於標準!有致力於自主管理，但需充實加強";
        public const string KMI_C_Str = "異常!須提改善計畫，複查後方得繼續施工";
        public const string KMI_D_Str = "重大異常!需緊急重新評估其管理";

        public class kmiAPlus : BqlString.Constant<kmiAPlus>
        {
            public kmiAPlus() : base(KMI_A_PLUS) { }
        }
        public class kmiA : BqlString.Constant<kmiA>
        {
            public kmiA() : base(KMI_A) { }
        }
        public class kmiBPlus : BqlString.Constant<kmiBPlus>
        {
            public kmiBPlus() : base(KMI_B_PLUS) { }
        }
        public class kmiB : BqlString.Constant<kmiB>
        {
            public kmiB() : base(KMI_B) { }
        }

        public class kmiC : BqlString.Constant<kmiC>
        {
            public kmiC() : base(KMI_C) { }
        }
        public class kmiD : BqlString.Constant<kmiD>
        {
            public kmiD() : base(KMI_D) { }
        }
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                    new string[] {
                    KMI_A_PLUS, KMI_A,KMI_B_PLUS, KMI_B, KMI_C,KMI_D
                },/*
                new string[] {
                    KMI_A_PLUS_Str, KMI_A_Str,KMI_B_PLUS_Str, KMI_B_Str, KMI_C_Str,KMI_D_Str
                }*/
                new string[] {
                    KMI_A_PLUS, KMI_A,KMI_B_PLUS, KMI_B, KMI_C,KMI_D
                })
            { }
        }

        public class ListSimpleAttribute : PXStringListAttribute
        {
            public ListSimpleAttribute()
                : base(
                new string[] {
                     KMI_A_PLUS, KMI_A,KMI_B_PLUS, KMI_B, KMI_C,KMI_D
                },
                new string[] {
                   KMI_A_PLUS_Str, KMI_A_Str,KMI_B_PLUS_Str, KMI_B_Str, KMI_C_Str,KMI_D_Str
                })
            { }
        }
    }

    public class RecentYear
    {
        public class ListAttribute : PXIntListAttribute
        {
            public ListAttribute() : base() {
                DateTime date = (DateTime)DateTime.Now;
                DateTime nextYear = date.AddYears(1);
                DateTime lastYear= date.AddYears(-1);
                string[] Labels =
                {
                   lastYear.Year.ToString(),date.Year.ToString(),nextYear.Year.ToString()
                };
                int[] Values =
                {
                    lastYear.Year,date.Year,nextYear.Year
                };
                base._AllowedValues = Values;
                base._AllowedLabels = Labels;
            }
        }
    }


    public class kgcomststage : BqlString.Constant<kgcomststage> { public  kgcomststage() : base("KGCONSTSTAGE") { } }
    [Serializable]
    public class KGMonthlyInspection : IBqlTable
    {
        public sealed class OneHundred : Constant<Decimal>
        {
            public OneHundred() : base(100)
            { }
        }

        #region MonthlyInspectionID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Monthly Inspection ID")]
        public virtual int? MonthlyInspectionID { get; set; }
        public abstract class monthlyInspectionID : IBqlField { }
        #endregion

        #region MonthlyInspectionCD

        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Monthly Inspection CD", Required = true)]
        [PXSelector(typeof(KGMonthlyInspection.monthlyInspectionCD),
                    typeof(KGMonthlyInspection.monthlyInspectionCD),
                    typeof(KGMonthlyInspection.checkDate)
                   )]
        [AutoNumber(typeof(Search<KGSetUp.kGMonthlyInspectionNumbering>), typeof(AccessInfo.businessDate))]
        public virtual string MonthlyInspectionCD { get; set; }
        public abstract class monthlyInspectionCD : IBqlField { }
        #endregion


        #region ProjectID
        
        //[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        //[PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        //[PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [ProjectBaseExt()]
        [PXUIField(DisplayName = "Project ID", Required = true)]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : IBqlField { }
        #endregion
        
        #region TemplateHeaderID
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Monthly Template Header ID", Required = true)]
        [PXSelector(typeof(Search<KGMonthlyInspectionTemplateH.templateHeaderID,
            Where<KGMonthlyInspectionTemplateH.status, Equal<KGMonthInspectionStatuses.open>>>),
            typeof(KGMonthlyInspectionTemplateH.templateCD),
            typeof(KGMonthlyInspectionTemplateH.segmentCD),
            typeof(KGMonthlyInspectionTemplateH.segmentDesc),
            SubstituteKey = typeof(KGMonthlyInspectionTemplateH.templateCD),
            DescriptionField = typeof(KGMonthlyInspectionTemplateH.segmentDesc)
            
        )]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : IBqlField { }
        #endregion

        #region Version
        [PXInt()]
        [PXUIField(DisplayName = "Version", Enabled = false)]
        public virtual int? Version { get; set; }
        public abstract class version : IBqlField { }
        #endregion

        #region ScoreSetup
        [PXDecimal()]
        [PXUIField(DisplayName = "Score Setup", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXDBScalar(
        //    typeof(Search<KGMonthlyInspectionTemplateH.scoreSetup,
        //    Where<KGMonthlyInspectionTemplateH.templateHeaderID, Equal<KGMonthlyInspection.templateHeaderID>>>)
        //)]
        public virtual Decimal? ScoreSetup { get; set; }
        public abstract class scoreSetup : IBqlField { }
        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate),PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Check Date", Required = true)]
        public virtual DateTime? CheckDate { get; set; }
        public abstract class checkDate : IBqlField { }
        #endregion


        #region ConstructionStage
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ConstructionStage")]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<kgcomststage>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart1>,
                    And<SegmentValue.active, Equal<True>>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr),
            DescriptionField = typeof(SegmentValue.descr)
        )]
        public virtual string ConstructionStage { get; set; }
        public abstract class constructionStage : IBqlField { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : IBqlField { }
        #endregion

        #region OverallRating
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Overall Rating", Visible = false)]
        public virtual string OverallRating { get; set; }
        public abstract class overallRating : IBqlField { }
        #endregion

        #region FinalScore
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0",PersistingCheck =PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Final Score", Enabled = false, IsReadOnly = true)]
        //[PXUnboundFormula(typeof(Sub<OneHundred, KGMonthlyInspection.allDeductionScore>),null)]
        public virtual Decimal? FinalScore { get; set; }
        public abstract class finalScore : IBqlField { }
        #endregion

        #region Evaluation
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Evaluation", Enabled = false, IsReadOnly = true)]
        //[PXDefault(KGMMonthlyInspectionEvaluations.KMI_D)]
        [KGMMonthlyInspectionEvaluations.List()]
        public virtual string Evaluation { get; set; }
        public abstract class evaluation : IBqlField { }
        #endregion

        #region EvaluationDesc
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "EvaluationDesc", Enabled = false, IsReadOnly = true)]
        public virtual string EvaluationDesc { get; set; }
        public abstract class evaluationDesc : IBqlField { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false, Required = true, IsReadOnly = true)]
        [PXDefault(KGMonthInspectionStatuses.Hold)]
        [KGMonthInspectionStatuses.List()]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
        [PXDefault(true)]
        public virtual Boolean? Hold { get; set; }
        public abstract class hold : IBqlField { }
        #endregion


        #region CheckYear
        [PXDBInt()]
        [PXUIField(DisplayName = "CheckYear",Required =true)]
        [PXDefault()]
        [RecentYear.List]
        public virtual int? CheckYear { get; set; }
        public abstract class checkYear : IBqlField { }
        #endregion

        #region CheckMonth
        [PXDBInt()]
        [PXUIField(DisplayName = "CheckMonth", Required = true)]
        [PXDefault()]
        [MonthOfYear.List]
        public virtual int? CheckMonth { get; set; }
        public abstract class checkMonth : IBqlField { }
        #endregion

        #region SystemManager
        [PXDBInt()]
        /*
        [PXSelector(typeof(Search2<BAccount.bAccountID,
            InnerJoin<EPEmployeeContract, On<EPEmployeeContract.employeeID, Equal<BAccount.bAccountID>>>,
            Where<EPEmployeeContract.contractID, Equal<Current<KGMonthlyInspection.projectID>>>>),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(BAccount.acctCD),
            DescriptionField = typeof(BAccount.acctName))]*/
        [PXEPEmployeeSelector]
        [PXUIField(DisplayName = "System Manager", Visibility = PXUIVisibility.SelectorVisible,Required =true)]
      
        public virtual int? SystemManager { get; set; }
        public abstract class systemManager : IBqlField { }
        #endregion

        #region SiteManager
        [PXDBInt()]
        /*
        [PXSelector(typeof(Search2<BAccount.bAccountID,
            InnerJoin<EPEmployeeContract, On<EPEmployeeContract.employeeID, Equal<BAccount.bAccountID>>>,
            Where<EPEmployeeContract.contractID, Equal<Current<KGMonthlyInspection.projectID>>>>),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(BAccount.acctCD),
            DescriptionField = typeof(BAccount.acctName))]*/
        [PXEPEmployeeSelector]
        [PXUIField(DisplayName = "Site Manager", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        public virtual int? SiteManager { get; set; }
        public abstract class siteManager : IBqlField { }
        #endregion

        #region EquipmentSupervisor
        [PXDBInt()]
        /*
        [PXSelector(typeof(Search2<BAccount.bAccountID,
            InnerJoin<EPEmployeeContract, On<EPEmployeeContract.employeeID, Equal<BAccount.bAccountID>>>,
            Where<EPEmployeeContract.contractID, Equal<Current<KGMonthlyInspection.projectID>>>>),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(BAccount.acctCD),
            DescriptionField = typeof(BAccount.acctName))]*/
        [PXEPEmployeeSelector]
        [PXUIField(DisplayName = "Equipment Supervisor", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        public virtual int? EquipmentSupervisor { get; set; }
        public abstract class equipmentSupervisor : IBqlField { }
        #endregion

        /*
        #region SafeHealthSupervisor
        [PXDBInt()]
        [PXEPEmployeeSelector]
        [PXUIField(DisplayName = "SafeHealth Supervisor", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
        public virtual int? SafeHealthSupervisor { get; set; }
        public abstract class safeHealthSupervisor : IBqlField { }
        #endregion
        */

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
}