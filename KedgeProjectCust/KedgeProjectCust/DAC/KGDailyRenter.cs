using System;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.RQ;
using PX.Objects.CR;
using PX.SM;
using PX.Objects.IN;

namespace Kedge.DAC
{
    /*
    public class MyAutonumber : AutoNumberAttribute, IPXFieldDefaultingSubscriber
    {
        
        public MyAutonumber(Type doctypeField, Type dateField)
            : base(doctypeField, dateField)
        {
        }

        void IPXFieldDefaultingSubscriber.FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            //getfields(sender, e.Row);
            base.GetType().InvokeMember("getfields", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.InvokeMethod, null, this, new object[] { sender, e.Row });
            e.NewValue = GetNewNumberSymbol();
        }
    }*/


    [Serializable]
    [PXPrimaryGraph(typeof(KGDailyRenterActualEntry))]
    public class KGDailyRenter : IBqlTable
    {
        //public const string Open = "N";
        public sealed class Complete : Constant<String>
        {
            public Complete() : base("M")
            {

            }
        }
        public sealed class Close : Constant<String>
        {
            public Close() : base("C")
            {

            }
        }
        public sealed class Open : Constant<String>
        {
            public Open() : base("N")
            {

            }
        }
        public sealed class RO : Constant<String>
        {
            public RO() : base("RO")
            {

            }
        }


        #region DailyRenterID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Daily Renter ID")]
        public virtual int? DailyRenterID { get; set; }
        public abstract class dailyRenterID : IBqlField { }
        #endregion

        #region DailyRenterCD
        [PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCC")]
        [PXUIField(DisplayName = "Daily Renter CD")]
        [PXDefault()]
        [PXSelector(typeof(Search2<KGDailyRenter.dailyRenterCD, InnerJoin<PMProject, On<KGDailyRenter.contractID, 
            Equal<PMProject.contractID>>>,
            Where<Match<PMProject, Current<AccessInfo.userName>>>>),
            typeof(KGDailyRenter.dailyRenterCD), typeof(KGDailyRenter.workDate),
            typeof(PMProject.contractCD), typeof(PMProject.description))]
        /**
        [PXSelector(typeof(Search2<KGDailyRenter.dailyRenterCD, InnerJoin<PMProject, On<KGDailyRenter.contractID, Equal<PMProject.contractID>>
            ,InnerJoin<Users,On<Users.pKID,Equal<KGDailyRenter.createdByID>>>>, Where<Users.pKID,
                    Equal<Current<AccessInfo.userID>>>>),
            typeof(KGDailyRenter.dailyRenterCD), typeof(KGDailyRenter.workDate), 
            typeof(PMProject.contractCD), typeof(PMProject.description))]
        */
        /*[PXSelector(typeof(Search2<KGDailyRenter.dailyRenterCD, InnerJoin<PMProject, On<KGDailyRenter.contractID, Equal<PMProject.contractID>>>>), typeof(KGDailyRenter.dailyRenterCD), typeof(KGDailyRenter.workDate), typeof(PMProject.contractCD), typeof(PMProject.description))]*/
        //[PXSelector(typeof(Search<KGDailyRenter.dailyRenterCD>), typeof(KGDailyRenter.dailyRenterCD), typeof(KGDailyRenter.workDate), typeof(PMProject.contractCD), typeof(PMProject.description))]*
        //CS201010 Numbering Sequences 

        //[AutoNumber("DAILYREN", typeof(AccessInfo.businessDate))]
        //[MyAutonumber(typeof(APSetupExt.usrDailyRenterID), typeof(KGDailyRenter.workDate))]
        [AutoNumber(typeof(KGSetUp.kGDailyRenterNumbering), typeof(KGDailyRenter.workDate))]
        //[AutoNumber(typeof(GLSetup.allocationNumberingID), typeof(AccessInfo.businessDate))]
        public virtual string DailyRenterCD { get; set; }
        public abstract class dailyRenterCD : IBqlField { }
        #endregion

        #region ContractID
        /*
        [PXDBInt()]
        [PXUIField(DisplayName = "Project")]
        [PXSelector(typeof(Search2<PMProject.contractID,
                LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                Equal<PMProject.contractID>>>>,
                Where<PMProject.baseType, Equal<CTPRType.project>,
                 And<PMProject.nonProject, Equal<False>,
                     And<PMProject.status, Equal<ProjectStatus.active>, And<Match<Current<AccessInfo.userName>>>>>>>)
                , typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(Customer.acctName), typeof(PMProject.status),
                typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), DescriptionField = typeof(PMProject.description), ValidateValue = false)]*/
        [ProjectBaseExt]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor")]
        [PXSelector(typeof(Search<Vendor.bAccountID>)
        , typeof(Vendor.acctCD), typeof(Vendor.acctName), SubstituteKey = typeof(Vendor.acctName), Filterable = true)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : IBqlField { }
        #endregion

        #region WorkDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Request Date")]
        public virtual DateTime? WorkDate { get; set; }
        public abstract class workDate : IBqlField { }
        #endregion

        #region OrderNbr
        //POOrderType.RegularSubcontract
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Purchase Order")]
        [PXSelector(typeof(Search5<POOrder.orderNbr,
                           LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>,
                           LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>,
                           LeftJoin<POContact, On<POOrder.remitContactID, Equal<POContact.contactID>>>>>,
                           Where<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
                              And2<Where<POOrder.status, Equal<Open>,
                                  Or<POOrder.status,Equal<Complete>>>,
                                And<Vendor.bAccountID, IsNotNull,
                                  And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>>>>>,
                           Aggregate<GroupBy<POOrder.orderNbr>>>),
                    typeof(POOrder.orderDesc),
                    typeof(POOrder.vendorID),
                    typeof(Vendor.acctName),
                    typeof(POOrder.orderDate),
                    typeof(POOrder.status),
                    typeof(Vendor.curyID),
                    typeof(POContact.fullName),
                    Filterable = true,
                    DescriptionField = typeof(POOrder.orderDesc))]
        /*[PXSelector(typeof(Search5<POOrder.orderNbr, LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
              , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>,
                  LeftJoin<BAccount, On<POOrder.vendorID, Equal<BAccount.bAccountID>>>>>,
          Where<POOrder.status, Equal<Open>, And<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
          And<Vendor.bAccountID, IsNotNull, And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>>>>>,
          Aggregate<GroupBy<POOrder.orderNbr>>>), typeof(POOrder.orderNbr), typeof(POOrder.orderDesc), typeof(POOrder.vendorID), typeof(Vendor.acctName), typeof(POOrder.orderDate), typeof(POOrder.status), typeof(Vendor.curyID), typeof(POOrder.vendorRefNbr), Filterable = true, DescriptionField = typeof(POOrder.orderDesc))]*/
        /*[PXSelector(typeof(Search5<POOrder.orderNbr,
          InnerJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
          And<Match<Vendor, Current<AccessInfo.userName>>>>
              , InnerJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>,
                  InnerJoin<BAccount,On<Vendor.bAccountID,Equal<BAccount.bAccountID>>>>>,
          Where<POOrder.status, Equal<Open>, And<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
          And<Vendor.bAccountID, IsNotNull, And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>>>>>,
          Aggregate<GroupBy<POOrder.orderNbr>>>),typeof(POOrder.orderNbr), typeof(POOrder.orderDesc), typeof(POOrder.vendorID), typeof(Vendor.acctName), typeof(BAccount.acctCD), typeof(BAccount.acctName), typeof(POOrder.orderDate), typeof(POOrder.status), typeof(Vendor.curyID), typeof(POOrder.vendorRefNbr),  Filterable = true, DescriptionField = typeof(POOrder.orderDesc))]*/
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region LineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Order Line")]
        [PXSelector(typeof(Search<POLine.lineNbr, Where<POLine.orderNbr, Equal<Current<KGDailyRenter.orderNbr>>,
            And<POLine.orderType, Equal<POOrderType.regularSubcontract>,
            And<POLine.curyUnbilledAmt, Greater<Zero>>>>>),
                //,
                //And<POLine.cancelled, Equal<Zero>>>>>>),
                    typeof(POLine.inventoryID),
                    typeof(POLine.tranDesc),
                    typeof(POLine.orderQty),
                    typeof(POLine.uOM),
                    typeof(POLine.curyUnitCost),
                    typeof(POLine.curyExtCost),
                    SubstituteKey = typeof(POLine.tranDesc))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : IBqlField { }
        #endregion

        #region ReqQty
        //[PXDBDecimal()]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]
        [PXUIField(DisplayName = "Req Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ReqQty { get; set; }
        public abstract class reqQty : IBqlField { }
        #endregion

        #region ActQty
        //[PXDBDecimal()]
        [PXUIField(DisplayName = "Act Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]
        public virtual Decimal? ActQty { get; set; }
        public abstract class actQty : IBqlField { }
        #endregion

        #region ReqSelfQty
        //[PXDBDecimal()]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]
        [PXUIField(DisplayName = "Req Self Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ReqSelfQty { get; set; }
        public abstract class reqSelfQty : IBqlField { }
        #endregion

        #region ActSelfQty
        //[PXDBDecimal()]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]
        [PXUIField(DisplayName = "Act Self Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ActSelfQty { get; set; }
        public abstract class actSelfQty : IBqlField { }

        #endregion

        #region ReqInsteadQty
        //[PXDBDecimal()]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]
        [PXUIField(DisplayName = "Req Instead Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ReqInsteadQty { get; set; }
        public abstract class reqInsteadQty : IBqlField { }
        #endregion

        #region ActInsteadQty
        //[PXDBDecimal()]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]
        [PXUIField(DisplayName = "Act Instead Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ActInsteadQty { get; set; }
        public abstract class actInsteadQty : IBqlField { }
        #endregion

        #region ReqWorkContent
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Req. Work Content")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string ReqWorkContent { get; set; }
        public abstract class reqWorkContent : IBqlField { }
        #endregion

        #region ActWorkContent
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Act. Work Content")]
        public virtual string ActWorkContent { get; set; }
        public abstract class actWorkContent : IBqlField { }
        #endregion
        public static String HOLD = "H";
        public static String PENDING_APPROVE = "P";
        public static String APPROVED = "A";
        public static String OPEN = "O";
        public static String PENDING_ON_SITE = "E";
        public static String ON_SITE = "S";
        public static String PENDING_SIGN = "W";
        public static String VENDOR_CONFIRM = "C";
        #region Status

        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Status")]
        [PXStringList(
                    new string[]
                    {
                      "O","E","S", "W","C"

                    },
                    new string[]
                    {
                       "OPEN","PENDING ON SITE","ON SITE", "PENDING SIGN","VENDOR CONFIRM"
        })]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
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
        /* 
       #region Vendor
       [PXString(IsUnicode = true)]
       [PXUIField(DisplayName = "Vendor")]
           /*
           [PXDBScalar(
               typeof(Search2<BAccount.acctName,
                           InnerJoin<POOrder, On<POOrder.vendorID, Equal<BAccount.bAccountID>>, 
                           InnerJoin<KGDailyRenter,On<KGDailyRenter.orderNbr, Equal<POOrder.orderNbr>>>>,
                           Where<KGDailyRenter.orderNbr ,Equal<Current<KGDailyRenter.orderNbr>>>>))]*/
        /*[PXDBScalar(
        typeof(Search2<BAccount.acctName,
                    InnerJoin<POOrder, On<POOrder.vendorID, Equal<BAccount.bAccountID>>,
                    InnerJoin<KGDailyRenter, On<KGDailyRenter.orderNbr, Equal<POOrder.orderNbr>>>>>))]
        public virtual string Vendor { get; set; }
        public abstract class vendor : IBqlField { }
        #endregion*/

        #region Hold
        //既⊿ノ逆
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
        [PXDefault(true)]
        public virtual Boolean? Hold { get; set; }
        public abstract class hold : IBqlField { }
        #endregion

        #region UpdateProject
        //既⊿ノ逆
        [PXBool()]
        [PXUIField(DisplayName = "Update Project", Visibility = PXUIVisibility.Visible)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Boolean? UpdateProject { get; set; }
        public abstract class updateProject : IBqlField { }
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
    public class KGDailyRenterStatus{
        public const string OPEN = "O";
        public const string PENDING_ON_SITE = "E";
        public const string ON_SITE = "S";
        public const string PENDING_SIGN = "W";
        public const string VENDOR_CONFIRM = "C";
        public static readonly string[] Values =
        {
            OPEN,
            PENDING_ON_SITE,
            ON_SITE,
            PENDING_SIGN,
            VENDOR_CONFIRM
        };
        public static readonly string[] Labels =
        {
            "OPEN",
            "PENDING ON SITE",
            "ON SITE",
            "PENDING SIGN",
            "VENDOR CONFIRM"
        };
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }
    }
}