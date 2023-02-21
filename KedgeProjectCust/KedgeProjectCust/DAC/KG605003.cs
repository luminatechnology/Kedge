using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CT;
using PX.Objects.PM;

namespace Kedge.DAC
{
    [Serializable]
    public class KG605003 : IBqlTable
    {
        #region BatchID
        [PXDBString(200, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch ID")]
        public virtual string BatchID { get; set; }
        public abstract class batchID : PX.Data.BQL.BqlString.Field<batchID> { }
        #endregion

        #region BatchSeq
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Batch Seq")]
        public virtual int? BatchSeq { get; set; }
        public abstract class batchSeq : PX.Data.BQL.BqlInt.Field<batchSeq> { }
        #endregion

        #region OrderType
        [PXDBString(2, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr")]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region CostCodeCD
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cost Code CD")]
        public virtual string CostCodeCD { get; set; }
        public abstract class costCodeCD : PX.Data.BQL.BqlString.Field<costCodeCD> { }
        #endregion

        #region CostCodeDesc
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cost Code Desc")]
        public virtual string CostCodeDesc { get; set; }
        public abstract class costCodeDesc : PX.Data.BQL.BqlString.Field<costCodeDesc> { }
        #endregion

        #region InventoryDesc
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Desc")]
        public virtual string InventoryDesc { get; set; }
        public abstract class inventoryDesc : PX.Data.BQL.BqlString.Field<inventoryDesc> { }
        #endregion

        #region InventoryCD
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory CD")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region AccountCD
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account CD")]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
        #endregion

        #region Uom
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uom")]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region RevisedQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Revised Qty")]
        public virtual Decimal? RevisedQty { get; set; }
        public abstract class revisedQty : PX.Data.BQL.BqlDecimal.Field<revisedQty> { }
        #endregion

        #region CuryUnitRate
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Unit Rate")]
        public virtual Decimal? CuryUnitRate { get; set; }
        public abstract class curyUnitRate : PX.Data.BQL.BqlDecimal.Field<curyUnitRate> { }
        #endregion

        #region CuryRevisedAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Revised Amount")]
        public virtual Decimal? CuryRevisedAmount { get; set; }
        public abstract class curyRevisedAmount : PX.Data.BQL.BqlDecimal.Field<curyRevisedAmount> { }
        #endregion

        #region OrderQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Order Qty")]
        public virtual Decimal? OrderQty { get; set; }
        public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
        #endregion

        #region CuryUnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Unit Cost")]
        public virtual Decimal? CuryUnitCost { get; set; }
        public abstract class curyUnitCost : PX.Data.BQL.BqlDecimal.Field<curyUnitCost> { }
        #endregion

        #region CuryLineAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Line Amt")]
        public virtual Decimal? CuryLineAmt { get; set; }
        public abstract class curyLineAmt : PX.Data.BQL.BqlDecimal.Field<curyLineAmt> { }
        #endregion

        #region BilledQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Billed Qty")]
        public virtual Decimal? BilledQty { get; set; }
        public abstract class billedQty : PX.Data.BQL.BqlDecimal.Field<billedQty> { }
        #endregion

        #region CuryBilledAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Billed Amt")]
        public virtual Decimal? CuryBilledAmt { get; set; }
        public abstract class curyBilledAmt : PX.Data.BQL.BqlDecimal.Field<curyBilledAmt> { }
        #endregion

        #region FinalQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Final Qty")]
        public virtual Decimal? FinalQty { get; set; }
        public abstract class finalQty : PX.Data.BQL.BqlDecimal.Field<finalQty> { }
        #endregion

        #region FinalAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Final Amount")]
        public virtual Decimal? FinalAmount { get; set; }
        public abstract class finalAmount : PX.Data.BQL.BqlDecimal.Field<finalAmount> { }
        #endregion

        #region IsBalance
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "IsBalance")]
        public virtual string IsBalance { get; set; }
        public abstract class isBalance : PX.Data.BQL.BqlString.Field<isBalance> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }
        #endregion

        //2020/01/30 ADD 4­ÓÄæ¦ì
        #region PurchaseBalance
         //±ÄÁÊµ²¾l
        [PXDBDecimal()]
        [PXUIField(DisplayName = "PurchaseBalance")]
        [PXDefault("0")]
        public virtual Decimal? PurchaseBalance { get; set; }
        public abstract class purchaseBalance : PX.Data.BQL.BqlDecimal.Field<purchaseBalance>{ }
        #endregion

        #region RQOrderQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "RQ Order Qty")]
        [PXDefault("0")]
        public virtual Decimal? RQOrderQty { get; set; }
        public abstract class rQOrderQty : PX.Data.BQL.BqlDecimal.Field<rQOrderQty> { }
        #endregion

        #region RQUnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "RQ Unit Cost")]
        [PXDefault("0")]
        public virtual Decimal? RQUnitCost { get; set; }
        public abstract class rQUnitCost : PX.Data.BQL.BqlDecimal.Field<rQUnitCost> { }
        #endregion

        #region RQAmt
        [PXDBDecimal()]
        [PXDefault("0")]
        [PXUIField(DisplayName = "RQ Amt")]
        public virtual Decimal? RQAmt { get; set; }
        public abstract class rQAmt : PX.Data.BQL.BqlDecimal.Field<rQAmt> { }
        #endregion
    }

    [Serializable]
    public class KG605003Filter : IBqlTable
    {

        #region ProjectID
        [PXInt()]
        [PXUIField(DisplayName = "ProjectID")]
        [PXSelector(typeof(Search2<PMProject.contractID,
                LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                Equal<PMProject.contractID>>>>,
                Where<PMProject.baseType, Equal<CTPRType.project>,
                 And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(Customer.acctName), typeof(PMProject.status),
                typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : IBqlField { }
        #endregion

        #region DeadLine
        [PXDate()]
        [PXUIField(DisplayName = "Dead Line")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? DeadLine { get; set; }
        public abstract class deadLine : IBqlField { }
        #endregion

    }
}