using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.PO;

namespace Kedge.DAC
{
    [Serializable]
    public class KGDailyRenterM : IBqlTable
    {
        #region DailyRenterID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Daily Renter ID")]
        public virtual int? DailyRenterID { get; set; }
        public abstract class dailyRenterID : PX.Data.BQL.BqlInt.Field<dailyRenterID> { }
        #endregion
    
        #region DailyRenterCD
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Daily Renter CD")]
        [PXSelector(typeof(KGDailyRenterM.dailyRenterCD))]
        [AutoNumber(typeof(KGSetUp.kGDailyRenterNumbering), typeof(KGDailyRenterM.workDate))]
        public virtual string DailyRenterCD { get; set; }
        public abstract class dailyRenterCD : PX.Data.BQL.BqlString.Field<dailyRenterCD> { }
        #endregion
    
        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project")]
        [PXSelector(typeof(Search2<PMProject.contractID,
                                   LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                                   LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID, Equal<PMProject.contractID>>>>,
                                   Where<PMProject.baseType, Equal<CTPRType.project>,
                                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>),
                    typeof(PMProject.contractCD), 
                    typeof(PMProject.description),
                    typeof(Customer.acctName), 
                    typeof(PMProject.status),
                    typeof(PMProject.approverID), 
                    SubstituteKey = typeof(PMProject.contractCD), 
                    ValidateValue = false,
                    DescriptionField = typeof(PMProject.description))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion
    
        #region WorkDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Request Date")]
        public virtual DateTime? WorkDate { get; set; }
        public abstract class workDate : PX.Data.BQL.BqlDateTime.Field<workDate> { }
        #endregion
    
        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Purchase Order")]
        [PXSelector(typeof(Search5<POOrder.orderNbr,
                                   LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
                                     And<Match<Vendor, Current<AccessInfo.userName>>>>,
                                   LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
                           Where<POOrder.status, Equal<KGDailyRenter.Open>, 
                             And<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
                                And<Vendor.bAccountID, IsNotNull, 
                                    And<POLine.projectID, Equal<Current<KGDailyRenterM.contractID>>>>>>,
                           Aggregate<GroupBy<POOrder.orderNbr>>>), 
                    Filterable = true, 
                    DescriptionField = typeof(POOrder.orderDesc))]
        [PXDefault()]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion
    
        #region LineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor Order Line")]
        [PXSelector(typeof(Search<POLine.lineNbr, 
                                  Where<POLine.orderNbr, Equal<Current<KGDailyRenterM.orderNbr>>,
                                    And<POLine.orderType, Equal<POOrderType.regularSubcontract>,
                                       And<POLine.curyUnbilledAmt, Greater<Zero>,
                                           And<POLine.cancelled, Equal<Zero>>>>>>),
                    typeof(POLine.inventoryID),
                    typeof(POLine.tranDesc),
                    typeof(POLine.orderQty),
                    typeof(POLine.uOM),
                    typeof(POLine.curyUnitCost),
                    typeof(POLine.curyExtCost),
                    SubstituteKey = typeof(POLine.tranDesc))]
        [PXDefault()]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        //[PXUIField(DisplayName = "Inventory ID", Visible = false)]
        [PXFormula(typeof(Default<KGDailyRenterM.lineNbr>))]
        [PXDefault(typeof(Search<POLine.inventoryID,
                                 Where<POLine.orderNbr, Equal<Current<KGDailyRenterM.orderNbr>>,
                                     And<POLine.lineNbr, Equal<Current<KGDailyRenterM.lineNbr>>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor", Enabled = false)]
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
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion
    
        #region ReqQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Req. Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ReqQty { get; set; }
        public abstract class reqQty : PX.Data.BQL.BqlDecimal.Field<reqQty> { }
        #endregion
    
        #region ActQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Act. Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ActQty { get; set; }
        public abstract class actQty : PX.Data.BQL.BqlDecimal.Field<actQty> { }
        #endregion
    
        #region ReqSelfQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Req. Self Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ReqSelfQty { get; set; }
        public abstract class reqSelfQty : PX.Data.BQL.BqlDecimal.Field<reqSelfQty> { }
        #endregion
    
        #region ActSelfQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Act. Self Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ActSelfQty { get; set; }
        public abstract class actSelfQty : PX.Data.BQL.BqlDecimal.Field<actSelfQty> { }
        #endregion
    
        #region ReqInsteadQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Req. Instead Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ReqInsteadQty { get; set; }
        public abstract class reqInsteadQty : PX.Data.BQL.BqlDecimal.Field<reqInsteadQty> { }
        #endregion
    
        #region ActInsteadQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Act. Instead Qty")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? ActInsteadQty { get; set; }
        public abstract class actInsteadQty : PX.Data.BQL.BqlDecimal.Field<actInsteadQty> { }
        #endregion
    
        #region ReqWorkContent
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req. Work Content")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ReqWorkContent { get; set; }
        public abstract class reqWorkContent : PX.Data.BQL.BqlString.Field<reqWorkContent> { }
        #endregion
    
        #region ActWorkContent
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Act. Work Content")]
        [PXDefault("", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string ActWorkContent { get; set; }
        public abstract class actWorkContent : PX.Data.BQL.BqlString.Field<actWorkContent> { }
        #endregion
    
        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        [PXDefault("H")]
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
    
        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold")]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
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