using System;
using PX.Data;
using PX.Objects.PO;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.IN;

namespace Kedge.DAC
{
  [Serializable]
    public class KGValuationDetailD : KGValuationDetail
    {

        #region PricingType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pricing Type")]
        [PXDefault("D")]
        [PXStringList(new string[]
            {
                "A",
                "D",
            },
            new string[]
            {
                "¥[´Ú",
                "¦©´Ú"
            })]
        public override string PricingType { get; set; }
        public new abstract class pricingType : IBqlField { }
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
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        public override int? InventoryID { get; set; }
        public new abstract class inventoryID : IBqlField { }
        #endregion

        #region UnBilledTotalAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "UnBilled Total Amt",Enabled =false)]
        public virtual decimal? UnBilledTotalAmt { get; set; }
        public abstract class unBilledTotalAmt : IBqlField { }
        #endregion

    }

}