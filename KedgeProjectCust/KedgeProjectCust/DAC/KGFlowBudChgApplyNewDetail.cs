using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
    /*
     * Since the original PK is identity, the header numbering sequence field cannot be correctly brought into the line related field, 
     * so keep the identity and add another related field in the PK.
    */
    [Serializable]
    public class KGFlowBudChgApplyNewDetail : IBqlTable
    {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWBND", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region Bndid
        // Acuminator disable once PX1055 DacKeyFieldsWithIdentityKeyField [Justification]
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Bndid")]
        public virtual int? Bndid { get; set; }
        public abstract class bndid : PX.Data.BQL.BqlInt.Field<bndid> { }
        #endregion

        #region Bnduid
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Bnduid")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string Bnduid { get; set; }
        public abstract class bnduid : PX.Data.BQL.BqlString.Field<bnduid> { }
        #endregion

        #region Bcnuid
        // Acuminator disable once PX1055 DacKeyFieldsWithIdentityKeyField [Justification]
        [PXDBString(40, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "Bcnuid")]
        [PXDBDefault(typeof(KGFlowBudChgApplyNew.bcnuid))]
        [PXParent(typeof(Select<KGFlowBudChgApplyNew, Where<KGFlowBudChgApplyNew.bcnuid, Equal<Current<bcnuid>>>>))]
        public virtual string Bcnuid { get; set; }
        public abstract class bcnuid : PX.Data.BQL.BqlString.Field<bcnuid> { }
        #endregion

        #region ItemNo
        [PXDBInt()]
        [PXUIField(DisplayName = "Item No")]
        public virtual int? ItemNo { get; set; }
        public abstract class itemNo : PX.Data.BQL.BqlInt.Field<itemNo> { }
        #endregion

        #region ItemName
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Name")]
        public virtual string ItemName { get; set; }
        public abstract class itemName : PX.Data.BQL.BqlString.Field<itemName> { }
        #endregion

        #region PccesCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pcces Code")]
        public virtual string PccesCode { get; set; }
        public abstract class pccesCode : PX.Data.BQL.BqlString.Field<pccesCode> { }
        #endregion

        #region ItemUnit
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Unit")]
        public virtual string ItemUnit { get; set; }
        public abstract class itemUnit : PX.Data.BQL.BqlString.Field<itemUnit> { }
        #endregion

        #region ItemCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Cost")]
        public virtual Decimal? ItemCost { get; set; }
        public abstract class itemCost : PX.Data.BQL.BqlDecimal.Field<itemCost> { }
        #endregion

        #region ItemQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Qty")]
        public virtual Decimal? ItemQty { get; set; }
        public abstract class itemQty : PX.Data.BQL.BqlDecimal.Field<itemQty> { }
        #endregion

        #region ItemAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Amt")]
        public virtual Decimal? ItemAmt { get; set; }
        public abstract class itemAmt : PX.Data.BQL.BqlDecimal.Field<itemAmt> { }
        #endregion

        #region EmergencyAccQty 
        [PXDBDecimal()] [PXUIField(DisplayName = "Emergency Acc Qty")] 
        public virtual Decimal? EmergencyAccQty { get; set; }
        public abstract class emergencyAccQty : PX.Data.BQL.BqlDecimal.Field<emergencyAccQty> { }
        #endregion

        #region EmergencyAccCost 
        [PXDBDecimal()] [PXUIField(DisplayName = "Emergency Acc Cost")] 
        public virtual Decimal? EmergencyAccCost { get; set; }
        public abstract class emergencyAccCost : PX.Data.BQL.BqlDecimal.Field<emergencyAccCost> { }
        #endregion

        #region EmergencyAccAmt 
        [PXDBDecimal()] [PXUIField(DisplayName = "Emergency Acc Amt")] 
        public virtual Decimal? EmergencyAccAmt { get; set; }
        public abstract class emergencyAccAmt : PX.Data.BQL.BqlDecimal.Field<emergencyAccAmt> { }
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
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}