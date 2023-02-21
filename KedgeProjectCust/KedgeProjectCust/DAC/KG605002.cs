using System;
using PX.Data;
using PX.Objects.PM;

namespace Kedge.DAC
{
    [Serializable]
    public class KG605002 : IBqlTable
    {
        #region BatchID
        [PXDBString(200, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch ID")]
        public virtual string BatchID { get; set; }
        public abstract class batchID : PX.Data.BQL.BqlString.Field<batchID> { }
        #endregion

        #region CostCodeCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cost Code CD")]
        public virtual string CostCodeCD { get; set; }
        public abstract class costCodeCD : PX.Data.BQL.BqlString.Field<costCodeCD> { }
        #endregion

        #region BatchSeq
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Batch Seq")]
        public virtual int? BatchSeq { get; set; }
        public abstract class batchSeq : PX.Data.BQL.BqlInt.Field<batchSeq> { }
        #endregion


        #region CostCodeDesc
        [PXDBString(250, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cost Code Desc")]
        public virtual string CostCodeDesc { get; set; }
        public abstract class costCodeDesc : PX.Data.BQL.BqlString.Field<costCodeDesc> { }
        #endregion

        #region InventoryCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory CD")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region InvetoryDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Invetory Desc")]
        public virtual string InvetoryDesc { get; set; }
        public abstract class invetoryDesc : PX.Data.BQL.BqlString.Field<invetoryDesc> { }
        #endregion

        #region AccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account CD")]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
        #endregion

        #region Uom
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uom")]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region UsrInvPrTypeName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Inv Pr Type Name")]
        public virtual string UsrInvPrTypeName { get; set; }
        public abstract class usrInvPrTypeName : PX.Data.BQL.BqlString.Field<usrInvPrTypeName> { }
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

        #region ProjectCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project Code")]
        public virtual string ProjectCode { get; set; }
        public abstract class projectCode : PX.Data.BQL.BqlString.Field<projectCode> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr")]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region OrderDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Desc")]
        public virtual string OrderDesc { get; set; }
        public abstract class orderDesc : PX.Data.BQL.BqlString.Field<orderDesc> { }
        #endregion

        #region VendorName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Name")]
        public virtual string VendorName { get; set; }
        public abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
        #endregion

        #region Balance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Balance")]
        public virtual Decimal? Balance { get; set; }
        public abstract class balance : PX.Data.BQL.BqlDecimal.Field<balance> { }
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

        #region IsFinal
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Is Final")]
        public virtual string IsFinal { get; set; }
        public abstract class isFinal : PX.Data.BQL.BqlString.Field<isFinal> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref Nbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region TranDesc
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tran Desc")]
        public virtual string TranDesc { get; set; }
        public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }
        #endregion

        #region ProjectTaskCD
        [PXDBString(30,IsUnicode =true)]
        [PXUIField(DisplayName = "Project Task CD")]
        public virtual string ProjectTaskCD { get; set; }
        public abstract class projectTaskCD : PX.Data.BQL.BqlString.Field<projectTaskCD> { }
        #endregion

        #region ProjectTaskDesc
        [PXDBString(250, IsUnicode = true)]
        [PXUIField(DisplayName = "Project Task Desc")]
        public virtual string ProjectTaskDesc { get; set; }
        public abstract class projectTaskDesc : PX.Data.BQL.BqlString.Field<projectTaskDesc> { }
        #endregion
    }
}