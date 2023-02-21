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
using Kedge;
using PX.Objects.IN;

namespace Kedge.DAC
{
    [Serializable]
    public class KGDailyRenterVendorAct : KGDailyRenterVendor
    {
        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "InsteadOrderNbr ")]
        /*[PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<Vendor.bAccountID, IsNotNull,
               And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>,
                   And<POOrder.orderNbr, NotEqual<Current<KGDailyRenter.orderNbr>>,
                        And2<Where<POOrder.status, Equal<Open>,
                            Or<POOrder.status,Equal<Close>>>,
                       And<POOrder.orderType, In3<POOrderType.regularSubcontract, POOrderType.regularOrder>>>>>
                  >,
              Aggregate<GroupBy<POOrder.orderNbr>>>), typeof(POOrder.orderNbr), typeof(POOrder.orderDesc), typeof(POOrder.vendorID), typeof(Vendor.acctName), typeof(POOrder.orderDate),
        typeof(POOrder.status), typeof(Vendor.curyID), typeof(POOrder.vendorRefNbr), Filterable = true, SubstituteKey = typeof(POOrder.orderNbr), DescriptionField = typeof(POOrder.orderDesc))]
        */
        [PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<Vendor.bAccountID, IsNotNull,
               And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>,
                   And<POOrder.orderNbr, NotEqual<Current<KGDailyRenter.orderNbr>>,
                        And<
                            Where2<
                                Where<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
                                    And<Where<POOrder.status, Equal<Open>,Or<POOrder.status, Equal<Complete>>>>>,
                                Or<Where<POOrder.orderType, Equal< POOrderType.regularOrder>,
                                        And<Where<POOrder.status,Equal<Open>,Or<POOrder.status,Equal<Close>,Or<POOrder.status,Equal<Complete>>>>>>>>>>>
                  >,
              Aggregate<GroupBy<POOrder.orderNbr>>>), 
            typeof(POOrder.orderNbr), 
            typeof(POOrder.orderDesc), 
            typeof(POOrder.vendorID), 
            typeof(Vendor.acctName), 
            typeof(POOrder.orderDate),
            typeof(POOrder.status), 
            typeof(Vendor.curyID), 
            typeof(POOrder.vendorRefNbr), 
            Filterable = true, 
            SubstituteKey = typeof(POOrder.orderNbr), 
            DescriptionField = typeof(POOrder.orderDesc))]


        public override string OrderNbr { get; set; }
        public new abstract class orderNbr : IBqlField { }
        #endregion
        #region Type
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Type")]
        [PXDefault("A")]
        public override string Type { get; set; }
        public new abstract class type : IBqlField { }
        #endregion
        #region InsteadQty
        //[PXDBDecimal()]
        [PXUIField(DisplayName = "InsteadQty")]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]

        //[PXFormula(null, typeof(SumCalc<KGDailyRenter.actInsteadQty>))]
        public new virtual Decimal? InsteadQty { get; set; }
        public new abstract class insteadQty : IBqlField { }
        #endregion
        #region InsteadQty
        [PXDecimal()]
        [PXUIField(DisplayName = "InsteadQty")]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXQuantity()]
        /*[PXFormula(typeof(Switch<Case2<Where<KGDailyRenterVendorAct.type, Equal<KGDailyRenterActualEntry.ACCTUALLY>>, 
        Selector < KGDailyRenterVendorAct.dailyRenterVendorID, KGDailyRenterVendorAct.insteadQty>>>), typeof(SumCalc<KGDailyRenter.actInsteadQty>))]*/
        /*[PXFormula(typeof(Switch<Case2<Where<Current<KGDailyRenterVendorAct.type>, Equal<KGDailyRenterActualEntry.ACCTUALLY>>,
         KGDailyRenterVendorAct.insteadQty>, Case2<Where<KGDailyRenterVendorAct.type, Equal<KGDailyRenterActualEntry.DEFAULT>>,
         Zero>>), typeof(SumCalc<KGDailyRenter.actInsteadQty>))]*/

        public virtual Decimal? VirInsteadQty { get; set; }
        public abstract class virInsteadQty : IBqlField { }
        #endregion
        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor", Enabled = false)]
        [PXSelector(typeof(Search<Vendor.bAccountID>)
        , typeof(Vendor.acctCD), typeof(Vendor.acctName), SubstituteKey = typeof(Vendor.acctName), Filterable = true)]
        public new virtual int? VendorID { get; set; }
        public new abstract class vendorID : IBqlField { }
        #endregion

    }
    [Serializable]
      public class KGDailyRenterVendor : IBqlTable
      {
        #region Selected
        public abstract class selected : IBqlField
        { }
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion
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
        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project")]
        /*[PXSelector(typeof(Search5<POOrder.orderNbr,
          LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
          And<Match<Vendor, Current<AccessInfo.userName>>>>
              , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
          Where<POOrder.orderType, Equal<Optional<POOrder.orderType>>, And<POOrder.status, Equal<Open>,
          And<Vendor.bAccountID, IsNotNull, And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>>>>>,
          Aggregate<GroupBy<POOrder.orderNbr>>>),SubstituteKey =typeof(PMProject.contractCD),Filterable = true)]*/
        [PXSelector(typeof(Search<PMProject.contractID,
            Where<PMProject.baseType, Equal<CTPRType.project>>>),
            SubstituteKey = typeof(PMProject.contractCD))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor")]
        [PXSelector(typeof(Search<Vendor.bAccountID>)
        , typeof(Vendor.acctCD), typeof(Vendor.acctName),SubstituteKey = typeof(Vendor.acctName), Filterable = true)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : IBqlField { }
        #endregion

        #region DailyRenterVendorID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Vendor")]
        public virtual int? DailyRenterVendorID { get; set; }
        public abstract class dailyRenterVendorID : IBqlField { }
        #endregion

        #region DailyRenterID
        [PXDBInt()]
        [PXUIField(DisplayName = "Daily Renter ID")]
        [PXDBDefault(typeof(KGDailyRenter.dailyRenterID))]
        [PXParent(typeof(Select<KGDailyRenter,
                                Where<KGDailyRenter.dailyRenterID,
                                Equal<Current<KGDailyRenterVendor.dailyRenterID>>>>))]
        public virtual int? DailyRenterID { get; set; }
        public abstract class dailyRenterID : IBqlField { }
        #endregion

        #region Type
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Type")]
        
        public virtual string Type { get; set; }
        public abstract class type : IBqlField { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "InsteadOrderNbr ")]
        /*[PXSelector(typeof(Search5<POOrder.orderNbr,
          LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
              , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
          Where<Vendor.bAccountID, IsNotNull, 
           And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>,
               And<POOrder.orderNbr, NotEqual<Current<KGDailyRenter.orderNbr>>,
                    And<POOrder.status, Equal<Open>,
                   And<POOrder.orderType, In3<POOrderType.regularSubcontract, POOrderType.regularOrder>>>>
              >>,
          Aggregate<GroupBy<POOrder.orderNbr>>>), typeof(POOrder.orderNbr), 
            typeof(POOrder.orderDesc), typeof(POOrder.vendorID), typeof(Vendor.acctName),
            typeof(POOrder.orderDate), typeof(POOrder.status), typeof(Vendor.curyID), 
            typeof(POOrder.vendorRefNbr), Filterable = true, DescriptionField = typeof(POOrder.orderDesc))]*/

        [PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<Vendor.bAccountID, IsNotNull,
               And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>,
                   And<POOrder.orderNbr, NotEqual<Current<KGDailyRenter.orderNbr>>,
                        And<
                            Where2<
                                Where<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
                                    And<Where<POOrder.status, Equal<Open>, Or<POOrder.status, Equal<Complete>>>>>,
                                Or<Where<POOrder.orderType, Equal<POOrderType.regularOrder>,
                                        And<Where<POOrder.status, Equal<Open>, Or<POOrder.status, Equal<Close>, Or<POOrder.status, Equal<Complete>>>>>>>>>>>
                  >,
              Aggregate<GroupBy<POOrder.orderNbr>>>),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc),
            typeof(POOrder.vendorID),
            typeof(Vendor.acctName),
            typeof(POOrder.orderDate),
            typeof(POOrder.status),
            typeof(Vendor.curyID),
            typeof(POOrder.vendorRefNbr),
            Filterable = true,
            SubstituteKey = typeof(POOrder.orderNbr),
            DescriptionField = typeof(POOrder.orderDesc))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region InsteadQty
        //[PXDBDecimal()]
        [PXUIField(DisplayName = "InsteadQty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        //20200909 计翴碭эъ╰参砞﹚QTY
        [PXDBQuantity()]
        public virtual Decimal? InsteadQty { get; set; }
        public abstract class insteadQty : IBqlField { }
        #endregion

        #region WorkContent
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Work Content")]
        public virtual string WorkContent { get; set; }
        public abstract class workContent : IBqlField { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        [PXDefault("O")]
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
    
        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : IBqlField { }
        #endregion
     
        #region ContractCD
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Project")]
        public virtual string ContractCD { get; set; }
        public abstract class contractCD : IBqlField { }
        #endregion

        #region Contractdesc
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Project Desc")]
        public virtual string Contractdesc { get; set; }
        public abstract class contractdesc : IBqlField { }
        #endregion

        #region DailyRenterCD
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "DailyRenterCD")]
        public virtual string DailyRenterCD { get; set; }
        public abstract class dailyRenterCD : IBqlField { }
        #endregion 

        #region WorkDate
        [PXDate()]
        [PXUIField(DisplayName = "Request Date")]
        public virtual DateTime? WorkDate { get; set; }
        public abstract class workDate : IBqlField { }
        #endregion

        #region 2019/06/17 Althea ADD 
        //2019/07/01 ConfirmQtyDefault△InsteadQty
        #region ConfirmQty
        //[PXDBDecimal()]
        [PXUIField(DisplayName = "ConfirmQty")]
        [PXDBQuantity()]
        [PXDefault(typeof(KGDailyRenterVendor.insteadQty), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? ConfirmQty { get; set; }
        public abstract class confirmQty : IBqlField { }
        #endregion

        #region BatchNbr
        [PXDBString(255)]
        [PXUIField(DisplayName = "BatchNbr")]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : IBqlField { }
        #endregion
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
		[Branch(typeof(KGDailyRenter.branchID))]
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