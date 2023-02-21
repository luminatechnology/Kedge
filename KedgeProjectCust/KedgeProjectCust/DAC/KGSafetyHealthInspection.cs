using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.PM;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspection : IBqlTable
    {
        #region SafetyHealthInspectionID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Safety Health Inspection ID")]
        public virtual int? SafetyHealthInspectionID { get; set; }
        public abstract class safetyHealthInspectionID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectionID> { }
        #endregion

        #region SafetyHealthInspectionCD
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Inspection CD",Required =true)]
        [PXSelector(typeof(Search<KGSafetyHealthInspection.safetyHealthInspectionCD>),
                typeof(KGSafetyHealthInspection.safetyHealthInspectionCD),
                typeof(KGSafetyHealthInspection.remark),
                typeof(KGSafetyHealthInspection.status))]
        [AutoNumber(typeof(Search<KGSetUp.kGSafetyHealthInspectionNumbering>), typeof(AccessInfo.businessDate))]
        public virtual string SafetyHealthInspectionCD { get; set; }
        public abstract class safetyHealthInspectionCD : PX.Data.BQL.BqlString.Field<safetyHealthInspectionCD> { }
        #endregion

        #region ProjectID
        [PXUIField(DisplayName = "Project ID", Required = true)]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [ProjectBaseAttribute()]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region TemplateHeaderID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspection Template Header ID", Required =true)]
        //2019/11/14 更改Seletor 顯示templateCD+SegmentCD+SegmentDesc
        /*[PXSelector(typeof(Search<KGSafetyHealthInspectionTemplateH.templateHeaderID,
            Where<KGSafetyHealthInspectionTemplateH.status, 
                Equal<SafetyHealthInspectionStatuses.open>>>),
            typeof(KGSafetyHealthInspectionTemplateH.templateCD),
            typeof(KGSafetyHealthInspectionTemplateH.description),
            SubstituteKey =typeof(KGSafetyHealthInspectionTemplateH.templateCD),
            DescriptionField =typeof(KGSafetyHealthInspectionTemplateH.description))]*/
        [PXSelector(typeof(Search<KGSafetyHealthInspectionTemplateH.templateHeaderID,
            Where<KGSafetyHealthInspectionTemplateH.status, Equal<SafetyHealthInspectionStatuses.open>>>),
            typeof(KGSafetyHealthInspectionTemplateH.templateCD),
            typeof(KGSafetyHealthInspectionTemplateH.segmentCD),
            typeof(KGSafetyHealthInspectionTemplateH.segmentDesc),
            SubstituteKey = typeof(KGSafetyHealthInspectionTemplateH.templateCD),
            DescriptionField = typeof(KGSafetyHealthInspectionTemplateH.segmentDesc)
        )]

        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : PX.Data.BQL.BqlInt.Field<templateHeaderID> { }
        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Check Date",Required =true)]
        [PXDefault(typeof(AccessInfo.businessDate),PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? CheckDate { get; set; }
        public abstract class checkDate : PX.Data.BQL.BqlDateTime.Field<checkDate> { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region FinalScore
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Final Score",Enabled =false)]
        public virtual Decimal? FinalScore { get; set; }
        public abstract class finalScore : PX.Data.BQL.BqlDecimal.Field<finalScore> { }
        #endregion

        #region Evaluation
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Evaluation",Enabled =false)]
        [KGSafetyInspectionEvaluations.List()]
        public virtual string Evaluation { get; set; }
        public abstract class evaluation : PX.Data.BQL.BqlString.Field<evaluation> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status",Enabled =false)]
        [PXDefault(SafetyHealthInspectionStatuses.Hold)]
        [SafetyHealthInspectionStatuses.List]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
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

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold")]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        #endregion

        #region SystemManager
        [PXDBInt()]
        [PXUIField(DisplayName = "System Manager")]
        [PXEPEmployeeSelector]

        public virtual int? SystemManager { get; set; }
        public abstract class systemManager : PX.Data.BQL.BqlInt.Field<systemManager> { }
        #endregion

        #region SiteManager
        [PXDBInt()]
        [PXUIField(DisplayName = "Site Manager")]
        [PXEPEmployeeSelector]

        public virtual int? SiteManager { get; set; }
        public abstract class siteManager : PX.Data.BQL.BqlInt.Field<siteManager> { }
        #endregion

        #region EquipmentSupervisor
        [PXDBInt()]
        [PXUIField(DisplayName = "Equipment Supervisor")]
        [PXEPEmployeeSelector]

        public virtual int? EquipmentSupervisor { get; set; }
        public abstract class equipmentSupervisor : PX.Data.BQL.BqlInt.Field<equipmentSupervisor> { }
        #endregion

        #region SafeHealthSupervisor
        [PXDBInt()]
        [PXUIField(DisplayName = "Safe Health Supervisor")]
        [PXEPEmployeeSelector]
        public virtual int? SafeHealthSupervisor { get; set; }
        public abstract class safeHealthSupervisor : PX.Data.BQL.BqlInt.Field<safeHealthSupervisor> { }
        #endregion

        #region InspectByID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inspect By ID")]
        [PXDefault(          
            typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>)            
            , PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXEPEmployeeSelector]
        public virtual int? InspectByID { get; set; }
        public abstract class inspectByID : PX.Data.BQL.BqlInt.Field<inspectByID> { }
        #endregion

        #region Version
        [PXInt()]
        [PXUIField(DisplayName = "Version", Enabled = false)]
        public virtual int? Version { get; set; }
        public abstract class version : IBqlField { }
        #endregion

        //2019/11/14 拿掉
        /*#region SegmentCD
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment CD", Enabled = false)]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.shin_segmentIDPart1>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr), DescriptionField = typeof(SegmentValue.descr)
        )]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }
        #endregion*/

        #region ScoreSetup
        [PXDecimal()]
        [PXUIField(DisplayName = "Score Setup", Enabled = false)]
        public virtual Decimal? ScoreSetup { get; set; }
        public abstract class scoreSetup : IBqlField { }
        #endregion

    }

}
public static class SafetyHealthInspectionStatuses
{

    public const string HoldName = "Hold";
    public const string OpenName = "Open";
    public const string CloseName = "Close";
    public const string Hold = "H";
    public const string Open = "N";
    public const string Close = "C";

    public class open : BqlString.Constant<open>
    {
        public open() : base(Open) { }
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
                new string[] { Hold, Open, Close },
                new string[] { HoldName, OpenName,CloseName })
        {
        }
    }

}
public static class KGSafetyInspectionEvaluations
{
    public const string KMI_A_PLUS = "A+";
    public const string KMI_A = "A";
    public const string KMI_B_PLUS = "B+";
    public const string KMI_B = "B";
    public const string KMI_C = "C";
    public const string KMI_D = "D";
    public const string KMI_A_PLUS_Str = "Ａ﹢：特優!自主管理、籌劃等，整體狀況特優";
    public const string KMI_A_Str = "Ａ ：高於標準!自主管理、籌劃等，整體狀況優良";
    public const string KMI_B_PLUS_Str = "B+ ：符合標準!致力於自主管理，符合公司要求";
    public const string KMI_B_Str = "B ：低於標準!有致力於自主管理，但需充實加強";
    public const string KMI_C_Str = "C ：異常!須提改善計畫，複查後方得繼續施工";
    public const string KMI_D_Str = "D ：重大異常!需緊急重新評估其管理";

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
            },
            new string[] {
                    KMI_A_PLUS_Str, KMI_A_Str,KMI_B_PLUS_Str, KMI_B_Str, KMI_C_Str,KMI_D_Str
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