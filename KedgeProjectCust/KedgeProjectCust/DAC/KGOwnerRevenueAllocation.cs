using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.PM;

namespace Kedge.DAC
{
    [Serializable]
    public class KGOwnerRevenueAllocation : IBqlTable
    {
        #region ProjectID
        [ProjectDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [ActiveProjectOrContractBaseAttribute(FieldClass = ProjectAttribute.DimensionName, IsKey = true)]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region TaskID
        [ProjectTask(typeof(projectID), IsKey = true, AlwaysEnabled = true)]
        public virtual int? TaskID { get; set; }
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
        #endregion

        #region CostCodeID
        [CostCode(null, typeof(taskID), null, IsKey = true, Filterable = false, SkipVerification = true)]
        public virtual int? CostCodeID { get; set; }
        public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }
        #endregion

        #region CostCodeDesc
        //[PXDBLocalizableString(250, IsUnicode = true)]
        [PXDBString(250, IsUnicode = true)]
        [PXUIField(DisplayName = "Cost Code Desc", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String CostCodeDesc { get; set; }
        public abstract class costCodeDesc : PX.Data.BQL.BqlString.Field<costCodeDesc> { }
        #endregion

        #region Qty
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty")]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region Uom
        [PMUnit(typeof(PMBudget.inventoryID))]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region Rate
        [PXDBPriceCost()]
        [PXUIField(DisplayName = "Rate", Enabled = false)]
        public virtual Decimal? Rate { get; set; }
        public abstract class rate : PX.Data.BQL.BqlDecimal.Field<rate> { }
        #endregion

        #region Amount
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Amount", Enabled = false)]
        [PXFormula(typeof(Mult<qty, rate>))]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
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