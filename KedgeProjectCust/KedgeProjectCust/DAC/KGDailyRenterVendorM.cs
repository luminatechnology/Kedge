using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.PO;

namespace Kedge.DAC
{
    [Serializable]
    public class KGDailyRenterVendorM : IBqlTable
    {
        #region DailyRenterVendorID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Daily Renter Vendor ID")]
        public virtual int? DailyRenterVendorID { get; set; }
        public abstract class dailyRenterVendorID : PX.Data.BQL.BqlInt.Field<dailyRenterVendorID> { }
        #endregion
    
        #region DailyRenterID
        [PXDBInt()]
        [PXUIField(DisplayName = "Daily Renter ID")]
        [PXDBDefault(typeof(KGDailyRenterM.dailyRenterID))]
        [PXParent(typeof(Select<KGDailyRenterM,
                                Where<KGDailyRenterM.dailyRenterID, Equal<Current<KGDailyRenterVendorM.dailyRenterID>>>>))]
        public virtual int? DailyRenterID { get; set; }
        public abstract class dailyRenterID : PX.Data.BQL.BqlInt.Field<dailyRenterID> { }
        #endregion
    
        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project")]
        [PXSelector(typeof(Search<PMProject.contractID,
                                  Where<PMProject.baseType, Equal<CTPRType.project>>>),
                    SubstituteKey = typeof(PMProject.contractCD))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor")]      
        [PXSelector(typeof(Search2<BAccount2.bAccountID,
                           LeftJoin<Vendor,
                                     On<Vendor.bAccountID, Equal<BAccount2.bAccountID>,
                                      And<Vendor.type, Equal<BAccountType.vendorType>>>>,
                           Where<BAccount2.type, Equal<BAccountType.vendorType>>>),
                    typeof(BAccount2.acctCD),
                    typeof(BAccount2.acctName),
                    typeof(BAccount2.type),
                    SubstituteKey = typeof(BAccount2.acctCD),
                    DescriptionField = typeof(BAccount2.acctName))]
        //[PXIntList(new int[] { 0 }, new string[] { "undefined" })]
        //[KGVendorLookUp(typeof(Current<KGDailyRenterM.contractID>), typeof(Current<KGDailyRenterM.orderNbr>))]
        public virtual Int32? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Purchase order")]
        [PXSelector(typeof(Search5<POOrder.orderNbr,
                                   LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
                                     And<Match<Vendor, Current<AccessInfo.userName>>>>,
                                   LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
                                   Where<Vendor.bAccountID, IsNotNull,
                                     And<POLine.projectID, Equal<Current<KGDailyRenterM.contractID>>,
                                        And<POOrder.orderNbr, NotEqual<Current<KGDailyRenterM.orderNbr>>,
                                            And<POOrder.status, Equal<KGDailyRenter.Open>,
                                                And<POOrder.orderType, In3<POOrderType.regularSubcontract, POOrderType.regularOrder>>>>>>,
                                   Aggregate<GroupBy<POOrder.orderNbr>>>), 
                     Filterable = true, 
                     DescriptionField = typeof(POOrder.orderDesc))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion
    
        #region LineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Line Nbr")]
        [PXSelector(typeof(Search<POLine.lineNbr, 
                                  Where<POLine.orderNbr, Equal<Current<KGDailyRenterVendorM.orderNbr>>,
                                    And<POLine.orderType, Equal<KGDailyRenter.RO>,
                                    And<POLine.curyUnbilledAmt, Greater<Zero>,
                                    And<POLine.cancelled, Equal<Zero>>>>>>),
                    typeof(POLine.inventoryID),
                    typeof(POLine.tranDesc),
                    typeof(POLine.orderQty),
                    typeof(POLine.uOM),
                    typeof(POLine.curyUnitCost),
                    typeof(POLine.curyExtCost),
                    SubstituteKey = typeof(POLine.tranDesc))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion
    
        #region InsteadQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Instead Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? InsteadQty { get; set; }
        public abstract class insteadQty : PX.Data.BQL.BqlDecimal.Field<insteadQty> { }
        #endregion
    
        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion
    
        #region WorkContent
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Work Content")]
        public virtual string WorkContent { get; set; }
        public abstract class workContent : PX.Data.BQL.BqlString.Field<workContent> { }
        #endregion
    
        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXStringList(new string[]
                      {
                        "H", "P","A","E","S", "W","C"
                      },
                      new string[]
                      {
                        "HOLD", "PENDING APPROVE"," APPROVED", "PENDING ON SITE",
                        "ON SITE", "PENDING SIGN","VENDOR CONFIRM"
                      }
                     )]
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
    
        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    
        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}