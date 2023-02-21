using System;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.PO;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.EP;
using PX.Objects.CR;


namespace Kedge.DAC
{

    [Serializable]
    public class KGSelfInspection : IBqlTable
    {
        #region SelfInspectionID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Self Inspection ID")]
        public virtual int? SelfInspectionID { get; set; }
        public abstract class selfInspectionID : IBqlField { }
        #endregion

        #region SelfInspectionCD
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "SelfInspection CD", Required = true)]
        [PXSelector(typeof(KGSelfInspection.selfInspectionCD),
                    typeof(KGSelfInspection.selfInspectionCD),
                    typeof(KGSelfInspection.checkDate)
                   )]
        [AutoNumber(typeof(Search<KGSetUp.kGSelfInspectionNumbering>), typeof(AccessInfo.businessDate))]
        public virtual string SelfInspectionCD { get; set; }
        public abstract class selfInspectionCD : IBqlField { }
        #endregion

        #region ProjectID
        //[PXDBInt()]
        [PXUIField(DisplayName = "Project", Required = true)]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [ProjectBaseAttribute()]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : IBqlField { }
        #endregion

        #region TemplateHeaderID
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Template Header ID", Required = true)]
        [PXSelector(typeof(Search<KGSelfInspectionTemplateH.templateHeaderID,
            Where<KGSelfInspectionTemplateH.status, Equal<KGSelfInspectionStatuses.open>>>),
                    typeof(KGSelfInspectionTemplateH.templateCD),
            typeof(KGSelfInspectionTemplateH.segmentCD),
            typeof(KGSelfInspectionTemplateH.version),
            typeof(KGSelfInspectionTemplateH.segmentDesc),
            SubstituteKey = typeof(KGSelfInspectionTemplateH.templateCD)
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

        #region SegmentCD
        [PXDimension(KGSelfInspectionTemplateH.DimensionName)]
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Segment CD", Enabled = false)]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }
        #endregion

        #region SegmentDesc
        [PXString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Segment Desc", Enabled = false)]
        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : IBqlField { }
        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Check Date", Required = true)]
        public virtual DateTime? CheckDate { get; set; }
        public abstract class checkDate : IBqlField { }
        #endregion

        #region CheckPosition
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Position")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string CheckPosition { get; set; }
        public abstract class checkPosition : IBqlField { }
        #endregion

        #region OrderType
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string OrderType { get; set; }
        public abstract class orderType : IBqlField { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search5<POOrder.orderNbr,
            LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
            And<Match<Vendor, Current<AccessInfo.userName>>>>,
            LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
            Where<POOrder.hold, Equal<False>,
            And<POOrder.orderType, In3<POOrderType.regularSubcontract, POOrderType.regularOrder>,
            And<POOrder.status, Equal<KGInspectionConstant.Word.N>,
                And<POLine.projectID, Equal<Current<KGSelfInspection.projectID>>>>>>,
            Aggregate<GroupBy<POOrder.orderNbr>>,
            OrderBy<Desc<POOrder.orderNbr>>>),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc), typeof(POOrder.vendorID), typeof(Vendor.acctName),
            typeof(POOrder.orderDate), typeof(POOrder.status)
            , DescriptionField = typeof(POOrder.orderDesc))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region VendorName
        [PXString(IsUnicode = true, InputMask = "")]
        //[PXDBScalar(typeof(
        //Search2<Vendor.acctName, InnerJoin<POOrder, On<POOrder.orderNbr, Equal<KGSelfInspection.orderNbr>
        //                        , And<POOrder.orderType, Equal<KGSelfInspection.orderType>>>>,
        //            Where<Vendor.bAccountID, Equal<POOrder.vendorID>>>
        //    ))]
        [PXUIField(DisplayName = "Vendor Name", Enabled = false, IsReadOnly = true)]
        public virtual string VendorName { get; set; }
        public abstract class vendorName : IBqlField { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : IBqlField { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Enabled = true)]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : IBqlField { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false, Required = true, IsReadOnly = true)]
        [PXDefault(KGSelfInspectionStatuses.Hold)]
        [KGSelfInspectionStatuses.ListAttribute4Self]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
        #endregion

        #region InspectByID
        [PXDBInt()]
        [PXUIField(DisplayName = "InspectByID" , Required = true)]
        [PXDefault(
            //typeof(AccessInfo.userID.FromCurrent)
            typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>)
            //typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.bAccountID, Equal<Current<AccessInfo.userID>>>>)
            , PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXEPEmployeeSelector]
        //[PXSelector(typeof(Search2<BAccount.bAccountID,
        //    InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<BAccount.bAccountID>>>>),
        //    typeof(BAccount.acctCD),
        //    typeof(BAccount.acctName),
        //    typeof(BAccount.status),
        //    typeof(EPEmployee.departmentID),
        //    SubstituteKey = typeof(BAccount.acctCD),
        //    DescriptionField = typeof(BAccount.acctName))]
        public virtual int? InspectByID { get; set; }
        public abstract class inspectByID : IBqlField { }
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
    }
}


