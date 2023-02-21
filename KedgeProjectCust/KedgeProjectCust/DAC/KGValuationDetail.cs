using System;
using PX.Data;
using PX.Objects.PO;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.IN;

namespace Kedge.DAC
{
  [Serializable]
  public class KGValuationDetail : IBqlTable
  {
        #region Selected
        public abstract class selected : IBqlField
        { }
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region ValuationDetailID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Valuation Detail ID")]
        public virtual int? ValuationDetailID { get; set; }
        public abstract class valuationDetailID : IBqlField { }
        #endregion        

        #region ValuationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Valuation CD", Required = true)]
        [PXDBDefault(typeof(KGValuation.valuationID))]
        [PXParent(typeof(Select<KGValuation,
                            Where<KGValuation.valuationID,
                            Equal<Current<KGValuationDetail.valuationID>>>>))]
        [PXSelector(typeof(Search<KGValuation.valuationID>),
            typeof(KGValuation.valuationCD),
            typeof(KGValuation.contractID),
            SubstituteKey =(typeof(KGValuation.valuationCD)))]
        public virtual int? ValuationID { get; set; }
        public abstract class valuationID : IBqlField { }
        #endregion

        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Contract ID", Required = true)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region PricingType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pricing Type")]
        [PXStringList(new string[]
            {
                "A",
                "D",
            },
            new string[]
            {
                "加款",
                "扣款"
            })]
        public virtual string PricingType { get; set; }
        public abstract class pricingType : IBqlField { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Purchase Order", Required = true,Enabled =false)]
        [PO.RefNbr(
            typeof(Search5<POOrder.orderNbr,
            LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
            And<Match<Vendor, Current<AccessInfo.userName>>>>,
            LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
            Where<POOrder.hold, Equal<False>,
            And2<Where<POOrder.status, Equal<word.n>,
                Or<POOrder.status,Equal<word.c>,
                    Or<POOrder.status,Equal<word.m>>>>,
            And<POLine.projectID, Equal<Current<KGValuation.contractID>>,
            And<Where<POOrder.orderType, Equal<word.ro>,
                Or<POOrder.orderType, Equal<word.rs>>>>>>>,
            Aggregate<GroupBy<POOrder.orderNbr>>,
            OrderBy<Desc<POOrder.orderNbr>>>),
            DescriptionField = typeof(POOrder.orderDesc)
       )]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region OrderType
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "Order Type", Required = true)]
        public virtual string OrderType { get; set; }
        public abstract class orderType : IBqlField { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "Inventory ID",Enabled =false)]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : IBqlField { }
        #endregion

        #region InvDesc
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Inv Desc", Enabled = false)]
        [PXDefault(typeof(Search<InventoryItem.descr,
            Where<InventoryItem.inventoryID, Equal<Current<inventoryID>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string InvDesc { get; set; }
        public abstract class invDesc : IBqlField { }
        #endregion

        #region Uom
        [PXDBString(6, IsUnicode = true)]
        [PXUIField(DisplayName = "UOM")]
        [PXStringList(new string[] { "工", "式" }, new string[] { "工", "式" })]
        public virtual string Uom { get; set; }
        public abstract class uom : IBqlField { }
        #endregion

        #region UnitPrice
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Unit Price")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : IBqlField { }
        #endregion

        #region Qty
        //[PXDBDecimal()]
        [PXUIField(DisplayName = "Qty")]
        //20200909 小數點幾位改為抓系統的設定QTY
        [PXDBQuantity()]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : IBqlField { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : IBqlField { }
        #endregion

        #region ManageFeeAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Manage Fee Amt")]
        public virtual Decimal? ManageFeeAmt { get; set; }
        public abstract class manageFeeAmt : IBqlField { }
        #endregion

        #region ManageFeeTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Manage Fee Tax Amt")]
        public virtual Decimal? ManageFeeTaxAmt { get; set; }
        public abstract class manageFeeTaxAmt : IBqlField { }
        #endregion

        #region ManageFeeTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Manage Fee Total Amt")]
        public virtual Decimal? ManageFeeTotalAmt { get; set; }
        public abstract class manageFeeTotalAmt : IBqlField { }
        #endregion

        #region TaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Amt")]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : IBqlField { }
        #endregion

        #region TotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Total Amt", Enabled = false)]
        public virtual Decimal? TotalAmt { get; set; }
        public abstract class totalAmt : IBqlField { }
        #endregion

        #region APInvoiceNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "APInvoice Nbr", Enabled = false)]
        public virtual string APInvoiceNbr { get; set; }
        public abstract class aPInvoiceNbr : IBqlField { }
        #endregion

        #region APInvoiceDate
        [PXDBDate()]
        [PXUIField(DisplayName = "APInvoice Date", Enabled = false)]
        public virtual DateTime? APInvoiceDate { get; set; }
        public abstract class aPInvoiceDate : IBqlField { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Status")]
        [PXStringList(new string[] { "O", "P", "C", "S", "V", "B" },
            new string[] { "OPEN", "PENDING CONFIRM", "CONFIRM", "PENDING SIGN", "VENDOR CONFIRM", "BILLED" })]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
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

        #region Vendor
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor", Enabled = false)]
        public virtual string Vendor { get; set; }
        public abstract class vendor : IBqlField { }
        #endregion

        #region LineNbr
        [PXInt()]
        [PXUIField(DisplayName = "LineNbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : IBqlField { }
        #endregion

        //2019/06/18加上BatchNbr用於Report
        #region BatchNbr
        [PXDBString(255)]
        [PXUIField(DisplayName = "BatchNbr")]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : IBqlField { }
        #endregion
    }
}