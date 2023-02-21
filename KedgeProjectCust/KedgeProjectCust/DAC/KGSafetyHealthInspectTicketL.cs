using System;
using PX.Data;
using PX.Objects.FS;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspectTicketL : IBqlTable
    {
        #region SafetyHealthInspectTicketLineID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket Line ID")]
        public virtual int? SafetyHealthInspectTicketLineID { get; set; }
        public abstract class safetyHealthInspectTicketLineID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectTicketLineID> { }
        #endregion

        #region SafetyHealthInspectTicketID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket ID")]
        [PXDBDefault(typeof(KGSafetyHealthInspectTicket.safetyHealthInspectTicketID))]
        [PXParent(typeof(Select<KGSafetyHealthInspectTicket,
                                Where<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                                Equal<Current<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID>>>>))]
        public virtual int? SafetyHealthInspectTicketID { get; set; }
        public abstract class safetyHealthInspectTicketID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectTicketID> { }
        #endregion

        #region SafetyHealthInspectionID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspection ID")]
        public virtual int? SafetyHealthInspectionID { get; set; }
        public abstract class safetyHealthInspectionID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectionID> { }
        #endregion

        #region SafetyHealthInspectionLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspection Line ID")]
        public virtual int? SafetyHealthInspectionLineID { get; set; }
        public abstract class safetyHealthInspectionLineID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectionLineID> { }
        #endregion

        #region TemplateLineID
        [PXDBInt]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : IBqlField { }
        #endregion

        #region CategoryCD
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Category CD", Required = true)]
        [PXDBStringList(
            typeof(KGSafetyHealthCategoryDeductionSetup),
            typeof(KGSafetyHealthCategoryDeductionSetup.categoryCD),
            typeof(KGSafetyHealthCategoryDeductionSetup.categoryCD))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Null)]
        public virtual string CategoryCD { get; set; }
        public abstract class categoryCD : IBqlField { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region LastRemark
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Last Remark", Enabled = false)]
        public virtual string LastRemark { get; set; }
        public abstract class lastRemark : PX.Data.BQL.BqlString.Field<lastRemark> { }
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
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

       #region CheckItem
        [PXString( IsUnicode = true, InputMask = "")]
        //2019/11/18 Change
        [PXSelectorWithCustomOrderBy(typeof(Search2<KGSafetyHealthInspectionTemplateL.checkItem,
            InnerJoin<KGSafetyHealthInspectionL, On<KGSafetyHealthInspectionL.templateLineID,
                Equal<KGSafetyHealthInspectionTemplateL.templateLineID>>>,
            Where<KGSafetyHealthInspectionL.safetyHealthInspectionID,
                Equal<Current<KGSafetyHealthInspectTicket.safetyHealthInspectionID>>>,
            OrderBy<Asc<KGSafetyHealthInspectionTemplateL.segmentCD>>>),
            typeof(KGSafetyHealthInspectionTemplateL.segmentCDRead),
            typeof(KGSafetyHealthInspectionTemplateL.segmentDescr),
            typeof(KGSafetyHealthInspectionTemplateL.checkItem)
        )]
        /*[PXDBScalar(
            typeof(Search<KGSafetyHealthInspectionTemplateL.checkItem,
                Where<KGSafetyHealthInspectionTemplateL.templateLineID, Equal<templateLineID>>>)
        )]*/
        [PXUIField(DisplayName = "Check Item",Required =true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion

        #region DeductionSetup
        [PXDecimal()]
        [PXUIField(DisplayName = "Safety Health Deduction Setup")]
        [PXDBScalar(
            typeof(Search<KGSafetyHealthCategoryDeductionSetup.deductionSetup,
                Where<KGSafetyHealthCategoryDeductionSetup.categoryCD, Equal<categoryCD>>>)
        )]
        public virtual Decimal? DeductionSetup { get; set; }
        public abstract class deductionSetup : IBqlField { }
        #endregion

        //2019/11/20 ADD 改善建議
        #region ImprovementPlan
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Improvement Plan")]
        public virtual string ImprovementPlan { get; set; }
        public abstract class improvementPlan : PX.Data.BQL.BqlString.Field<improvementPlan> { }
        #endregion

    }
    [Serializable]
    public class KGSafetyHealthInspectTicketLUpdate : KGSafetyHealthInspectTicketL
    {
        #region key

        #region SafetyHealthInspectionID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Inspection ID",TabOrder =1)]
        public override int? SafetyHealthInspectionID { get; set; }
        #endregion

        #region SafetyHealthInspectTicketLineID
        [PXDBIdentity(IsKey =true)]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket Line ID")]
        public override int? SafetyHealthInspectTicketLineID { get; set; }
        #endregion

        #region CategoryCD
        [PXDBString(50, IsUnicode = true, InputMask = "",IsKey =true)]
        [PXUIField(DisplayName = "Safety Health Category CD",TabOrder =0)]
        public override string CategoryCD { get; set; }
        #endregion

        #endregion

        #region SafetyHealthInspectTicketID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket ID")]
        public override int? SafetyHealthInspectTicketID { get; set; }
        
        #endregion

        #region SafetyHealthInspectionLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspection Line ID")]
        public virtual int? SafetyHealthInspectionLineID { get; set; }
        public abstract class safetyHealthInspectionLineID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectionLineID> { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        [PXDefault("無資料")]
        public override string Remark { get; set; }
        #endregion

        #region ViewCategoryCD
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Category CD")]
        public virtual string ViewCategoryCD { get; set; }
        public abstract class viewCategoryCD : IBqlField { }
        #endregion

        #region Revise

        #region ReviseCheckItem
        [PXString(50, IsUnicode = true, InputMask = "")]
        //2019/12/31
        [PXSelectorWithCustomOrderBy(
            typeof(Search2<KGSafetyHealthInspectionTemplateL.checkItem,
                InnerJoin<KGSafetyHealthInspection,
                    On<KGSafetyHealthInspection.templateHeaderID, 
                        Equal<KGSafetyHealthInspectionTemplateL.templateHeaderID>>>,
                Where<KGSafetyHealthInspection.safetyHealthInspectionID, 
                    Equal<Current<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectionID>>>,
                OrderBy<Asc<KGSafetyHealthInspectionTemplateL.segmentCDRead>>>),
            typeof(KGSafetyHealthInspectionTemplateL.segmentCDRead),
            typeof(KGSafetyHealthInspectionTemplateL.segmentDescr),
            typeof(KGSafetyHealthInspectionTemplateL.checkItem), 
            DescriptionField = typeof(KGSafetyHealthInspectionTemplateL.checkItem),
            Filterable = true
        )]
        [PXUIField(DisplayName = "Revise Check Item")]
        public virtual string ReviseCheckItem { get; set; }
        public abstract class reviseCheckItem : IBqlField { }
        #endregion

        #region ReviseCategoryCD
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Revise Safety Health Category CD")]
        [PXDBStringList(
            typeof(KGSafetyHealthCategoryDeductionSetup),
            typeof(KGSafetyHealthCategoryDeductionSetup.categoryCD),
            typeof(KGSafetyHealthCategoryDeductionSetup.categoryCD))]
        public virtual string ReviseCategoryCD { get; set; }
        public abstract class reviseCategoryCD : IBqlField { }
        #endregion
        #endregion

        #region IsDelete
        [PXBool()]
        [PXUIField(DisplayName = "Is Delete")]
        public virtual bool? IsDelete { get; set; }
        public abstract class isDelete : PX.Data.BQL.BqlBool.Field<isDelete> { }
        #endregion

        #region ImageUrl
        [PXDBString(255)]
        [PXUIField(DisplayName = "Image")]
        public virtual string ImageUrl { get; set; }
        public abstract class imageUrl : PX.Data.BQL.BqlString.Field<imageUrl> { }
        #endregion

        #region CheckItem
        [PXString(IsUnicode = true, InputMask = "")]
        //2019/11/18 Change
        [PXSelectorWithCustomOrderBy(typeof(Search2<KGSafetyHealthInspectionTemplateL.checkItem,
            InnerJoin<KGSafetyHealthInspectionL, On<KGSafetyHealthInspectionL.templateLineID,
                Equal<KGSafetyHealthInspectionTemplateL.templateLineID>>>,
            Where<KGSafetyHealthInspectionL.safetyHealthInspectionID,
                Equal<Current<KGSafetyHealthInspectTicket.safetyHealthInspectionID>>>,
            OrderBy<Asc<KGSafetyHealthInspectionTemplateL.segmentCD>>>),
            typeof(KGSafetyHealthInspectionTemplateL.segmentCDRead),
            typeof(KGSafetyHealthInspectionTemplateL.segmentDescr),
            typeof(KGSafetyHealthInspectionTemplateL.checkItem)
        )]
        /*[PXDBScalar(
            typeof(Search<KGSafetyHealthInspectionTemplateL.checkItem,
                Where<KGSafetyHealthInspectionTemplateL.templateLineID, Equal<templateLineID>>>)
        )]*/
        [PXUIField(DisplayName = "Check Item", Required = true)]
        //[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public override string CheckItem { get; set; }
        #endregion
    }

    [Serializable]
    public class KGSafetyHealthInspectTicketLUpdateAll : KGSafetyHealthInspectTicketLUpdate
    {
        #region key
        #region SafetyHealthInspectionLineID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Inspection Line ID",TabOrder =0)]
        public override int? SafetyHealthInspectionLineID { get; set; }
        #endregion

        #region SafetyHealthInspectTicketLineID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket Line ID")]
        public override int? SafetyHealthInspectTicketLineID { get; set; }
        #endregion

        #endregion

        #region SafetyHealthInspectionID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspection ID")]
        public override int? SafetyHealthInspectionID { get; set; }
        #endregion

        #region CategoryCD
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Category CD")]
        public override string CategoryCD { get; set; }
        #endregion
    }
}