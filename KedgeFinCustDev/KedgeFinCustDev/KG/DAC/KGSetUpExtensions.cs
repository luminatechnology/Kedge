using Kedge.DAC;
using PX.Data;
using PX.Objects;
using PX.Objects.IN;
using System;

namespace Kedge.DAC
{
    public class KGSetUpExt : PXCacheExtension<Kedge.DAC.KGSetUp>
    {
        #region UsrKGPrjManageInventoryID
        [PXDBInt]
        [PXUIField(DisplayName = "UsrKGPrjManageInventoryID")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        public virtual int? UsrKGPrjManageInventoryID { get; set; }
        public abstract class usrKGPrjManageInventoryID : PX.Data.BQL.BqlInt.Field<usrKGPrjManageInventoryID> { }
        #endregion
    }
}