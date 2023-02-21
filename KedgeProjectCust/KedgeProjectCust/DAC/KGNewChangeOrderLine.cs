using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.IN;
using PX.Objects.PM;


/**
 *  ====2021-06-10:12011====Alton
 *  契約新增申請中的Line, 單價請新增field_updated事件, 連動計算金額(KedgeProjectCust)
 *  移除UnitCost isReadyonly = true
 * 
 * **/
namespace Kedge.DAC
{
    [Serializable]
    public class KGNewChangeOrderLine : IBqlTable
    {
        #region NewChangeOrderLineID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "New Change Order Line ID")]
        public virtual int? NewChangeOrderLineID { get; set; }
        public abstract class newChangeOrderLineID : PX.Data.BQL.BqlInt.Field<newChangeOrderLineID> { }
        #endregion

        #region NewChangeOrderID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "New Change Order ID")]
        [PXDBDefault(typeof(KGNewChangeOrder.newChangeOrderID))]
        [PXParent(typeof(Select<KGNewChangeOrder,
                            Where<KGNewChangeOrder.newChangeOrderID,
                              Equal<Current<newChangeOrderID>>>>))]
        public virtual int? NewChangeOrderID { get; set; }
        public abstract class newChangeOrderID : PX.Data.BQL.BqlInt.Field<newChangeOrderID> { }
        #endregion

        #region LineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region ProjectID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project ID", Required = true, IsReadOnly = true)]
        [PXSelector(typeof(Search<Contract.contractID>),
                    typeof(Contract.contractCD),
                    typeof(Contract.description),
                SubstituteKey = typeof(Contract.contractCD),
                DescriptionField = typeof(Contract.description)
            )]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region Taskid
        [PXDBInt()]
        [PXUIField(DisplayName = "Taskid", Required = true, IsReadOnly = true)]
        [PXSelector(typeof(Search<PMTask.taskID>),
                    typeof(PMTask.taskCD),
                    typeof(PMTask.description),
                SubstituteKey = typeof(PMTask.taskCD),
                DescriptionField = typeof(PMTask.description)
            )]
        public virtual int? Taskid { get; set; }
        public abstract class taskid : PX.Data.BQL.BqlInt.Field<taskid> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID", Required = true, IsReadOnly = true)]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
                    typeof(InventoryItem.inventoryCD),
                    typeof(InventoryItem.descr),
                SubstituteKey = typeof(InventoryItem.inventoryCD),
                DescriptionField = typeof(InventoryItem.descr)
            )]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region CostCodeID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Code ID", Required = true, IsReadOnly = true)]
        [PXSelector(typeof(Search<PMCostCode.costCodeID>),
                    typeof(PMCostCode.costCodeCD),
                    typeof(PMCostCode.description),
                SubstituteKey = typeof(PMCostCode.costCodeCD),
                DescriptionField = typeof(PMCostCode.description)
            )]
        public virtual int? CostCodeID { get; set; }
        public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }
        #endregion

        #region Description
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description", Required = true)]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor ID", Required = true)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty", Required = true)]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region Uom
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uom",IsReadOnly = true)]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region UnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Unit Cost", Required = true)]
        public virtual Decimal? UnitCost { get; set; }
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount", Required = true,IsReadOnly = true)]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
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
        //[PXUIField(DisplayName = "Created Date Time")]
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
        //[PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion
    }
}