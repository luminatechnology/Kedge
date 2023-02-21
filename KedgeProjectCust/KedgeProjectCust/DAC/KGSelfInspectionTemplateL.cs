using System;
using PX.Data;
using PX.Objects.IN;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSelfInspectionTemplateL : IBqlTable
    {
        
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : IBqlField { }
        #endregion

        #region TemplateHeaderID
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(KGSelfInspectionTemplateH.templateHeaderID))]
        [PXParent(typeof(Select<KGSelfInspectionTemplateH,
                        Where<KGSelfInspectionTemplateH.templateHeaderID,
                          Equal<Current<KGSelfInspectionTemplateL.templateHeaderID>>>>))]
        [PXUIField(DisplayName = "Template Header ID")]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : IBqlField { }
        #endregion

        #region TemplateLineID
        [PXDBIdentity(IsKey = true)]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : IBqlField { }
        #endregion

        #region CheckItem
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Check Item", Enabled = true,Required =true)]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion

        //#region InventoryID
        //[PXDBInt(IsKey = true)]
        //[PXUIField(DisplayName = "Inv ID", Required = true)]
        //[PXSelector(typeof(Search2<InventoryItem.inventoryID,
        //    InnerJoin<INItemClass, On<INItemClass.itemClassID, Equal<InventoryItem.itemClassID>>>,
        //    Where<INItemClass.itemClassCD, Equal<KGInspectionConstant.inItemClassSC>>>),
        //    typeof(InventoryItem.inventoryID),
        //    typeof(InventoryItem.inventoryCD),
        //    typeof(InventoryItem.descr),
        //   SubstituteKey = typeof(InventoryItem.inventoryCD)
        //)]
        //public virtual int? InventoryID { get; set; }
        //public abstract class inventoryID : IBqlField { }
        //#endregion

        //#region InvDesc
        //[PXString(IsUnicode = true, InputMask = "")]
        //[PXUIField(DisplayName = "Inv Descr", Enabled = false, IsReadOnly = true)]
        //[PXDefault(
        //    typeof(Search<InventoryItem.descr,
        //        Where<InventoryItem.inventoryID, Equal<Current<KGSelfInspectionTemplateL.inventoryID>>>>
        //        )
        //)]
        //public virtual string InvDesc { get; set; }
        //public abstract class invDesc : IBqlField { }
        //#endregion

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
    }
}