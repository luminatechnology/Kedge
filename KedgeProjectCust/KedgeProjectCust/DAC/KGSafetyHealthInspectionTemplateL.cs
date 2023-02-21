using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspectionTemplateL : IBqlTable
    {
        #region TemplateHeaderID
        [PXDBInt()]
        [PXDBDefault(typeof(KGSafetyHealthInspectionTemplateH.templateHeaderID))]
        [PXParent(typeof(Select<KGSafetyHealthInspectionTemplateH,
                    Where<KGSafetyHealthInspectionTemplateH.templateHeaderID,
                    Equal<Current<templateHeaderID>>>>))]
        [PXUIField(DisplayName = "Safety Health Template Header ID")]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : PX.Data.BQL.BqlInt.Field<templateHeaderID> { }
        #endregion

        #region TemplateLineID
        [PXDBIdentity(IsKey =true)]
        [PXUIField(DisplayName = "Safety Health Template Line ID")]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : PX.Data.BQL.BqlInt.Field<templateLineID> { }
        #endregion

        #region SegmentCD
        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment CD")]
        [PXStringList()]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : PX.Data.BQL.BqlString.Field<segmentCD> { }
        #endregion

        //2019/11/13 因職安衛與月查核不同,故此欄位拿掉
        /*#region MaxNoMissing
        [PXDBInt()]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Safety Health Max No Missing", Required =true)]
        public virtual int? MaxNoMissing { get; set; }
        public abstract class maxNoMissing : PX.Data.BQL.BqlInt.Field<maxNoMissing> { }
        #endregion*/

        #region CheckItem
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Check Item",Required =true)]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : PX.Data.BQL.BqlString.Field<checkItem> { }
        #endregion

        //2020/03/09 拿掉
        /*#region CheckPointDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Check Point Desc")]
        public virtual string CheckPointDesc { get; set; }
        public abstract class checkPointDesc : PX.Data.BQL.BqlString.Field<checkPointDesc> { }
        #endregion*/

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


        #region SegmentDescr
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXString()]
        [PXUIField(DisplayName = "職安衛類別說明", IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(
               typeof(Search2<SegmentValue.descr,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>,
                And<SegmentValue.value, Equal<KGSafetyHealthInspectionTemplateL.segmentCD>>>>>))]
        public virtual string SegmentDescr { get; set; }
        public abstract class segmentDescr : IBqlField { }
        #endregion
        #region SegmentCDRead
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXString(30)]
        [PXUIField(DisplayName = "SegmentCD Read")]
        //[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXFormula(typeof(segmentCD))]
        public virtual string SegmentCDRead { get; set; }
        public abstract class segmentCDRead : IBqlField { }
        #endregion

        //2020/09/11 ADD By Althea Mantis:0011645
        #region RiskType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Risk Type",Required =true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshrisk>,
                And<Segment.segmentID, Equal<KGInspectionConstant.shin_segmentIDPart1>,
                And<SegmentValue.active, Equal<True>>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr),
            DescriptionField = typeof(SegmentValue.descr)
        )]
        public virtual string RiskType { get; set; }
        public abstract class riskType : PX.Data.BQL.BqlString.Field<riskType> { }
        #endregion

        #region RiskCategory
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Risk Category",Required =true)]
        [PXStringList(new string[]{"A", "B"},new string []{ "不安全環境", "不安全行為" })]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]

        public virtual string RiskCategory { get; set; }
        public abstract class riskCategory : PX.Data.BQL.BqlString.Field<riskCategory> { }
        #endregion
    }
}