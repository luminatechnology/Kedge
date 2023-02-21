using System;
using PX.Data;
using PX.Objects.IN;

namespace Kedge.DAC
{
    [Serializable]
    public class KGProjectRenterMaterial : IBqlTable
    {
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region RenterMaterialID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Renter Material ID")]
        public virtual int? RenterMaterialID { get; set; }
        public abstract class renterMaterialID : PX.Data.BQL.BqlInt.Field<renterMaterialID> { }
        #endregion

        #region ProjectStageID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project Stage ID")]
        [PXDBDefault(typeof(KGProjectStage.projectStageID))]
        [PXParent(typeof(Select<KGProjectStage,
                            Where<KGProjectStage.projectStageID,
                            Equal<Current<renterMaterialID>>>>))]
        public virtual int? ProjectStageID { get; set; }
        public abstract class projectStageID : PX.Data.BQL.BqlInt.Field<projectStageID> { }
        #endregion

        #region InventoryID
        [PXDBInt(IsKey =true)]
        [PXUIField(DisplayName = "InventoryID")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
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

        #region NoteId
        [PXNote()]
        [PXUIField(DisplayName = "NoteId")]
        public virtual Guid? NoteId { get; set; }
        public abstract class noteId : PX.Data.BQL.BqlGuid.Field<noteId> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}