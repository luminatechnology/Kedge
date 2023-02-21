using System;
using PX.Data;
using PX.Objects.CS;
namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowBudChgApplyDetail : IBqlTable
      {
            #region Setup
            public abstract class setup : IBqlField
            { }
            [PXString()]
            //CS201010
            [PXDefault("KGFLOWCHG")]
            [PXUIField(DisplayName = "Setup")]
            public virtual string Setup { get; set; }
            #endregion

            #region ChgID
            [PXDBIdentity(IsKey = true)]
            [PXUIField(DisplayName = "ChgID")]
            public virtual int? ChgID { get; set; }
            public abstract class chgID : PX.Data.BQL.BqlInt.Field<chgID> { }
            #endregion
            #region ChgUID
            [PXDBString(40, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "ChgUID")]
            [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
            public virtual string ChgUID { get; set; }
            public abstract class chgUID : PX.Data.BQL.BqlString.Field<chgUID> { }
            #endregion
            #region BcaUID
            [PXDBString(40, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "BcaUID")]
            [PXDBDefault(typeof(KGFlowChangeManagement.bcaUID))]
            [PXParent(typeof(Select<KGFlowChangeManagement,
                                Where<KGFlowChangeManagement.bcaUID,
                                Equal<Current<KGFlowBudChgApplyDetail.bcaUID>>>>))]
            public virtual string BcaUID { get; set; }
            public abstract class bcaUID : PX.Data.BQL.BqlString.Field<bcaUID> { }
        #endregion
            /*
            #region ItemNo
            [PXDBInt()]
            [PXUIField(DisplayName = "Item No")]
            public virtual int? ItemNo { get; set; }
            public abstract class itemNo : PX.Data.BQL.BqlInt.Field<itemNo> { }
            #endregion
            */
            #region ItemNo
            [PXDBString(40, IsUnicode = true)]
            [PXUIField(DisplayName = "Item No")]
            public virtual string ItemNo { get; set; }
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
            #region ChgCost
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Chg Cost")]
            public virtual Decimal? ChgCost { get; set; }
            public abstract class chgCost : PX.Data.BQL.BqlDecimal.Field<chgCost> { }
            #endregion
            #region ChgQty
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Chg Qty")]
            public virtual Decimal? ChgQty { get; set; }
            public abstract class chgQty : PX.Data.BQL.BqlDecimal.Field<chgQty> { }
            #endregion
            #region ChgAmt
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Chg Amt")]
            public virtual Decimal? ChgAmt { get; set; }
            public abstract class chgAmt : PX.Data.BQL.BqlDecimal.Field<chgAmt> { }
        #endregion
        #region PrintNo
        [PXDBString(128, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Print No")]
        public virtual string PrintNo { get; set; }
        public abstract class printNo : PX.Data.BQL.BqlString.Field<printNo> { }
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
            public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID>
            {
            }
            protected Guid? _NoteID;

            /// <summary>
            /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the line.
            /// </summary>
            /// <value>
            /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
            /// </value>
	        [PXNote()]
            public virtual Guid? NoteID
            {
                get
                {
                    return this._NoteID;
                }
                set
                {
                    this._NoteID = value;
                }
            }
            #endregion
            #region Tstamp
            [PXDBTimestamp()]
            [PXUIField(DisplayName = "Tstamp")]
            public virtual byte[] Tstamp { get; set; }
            public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
        #region ReasonKindCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ReasonKindCode")]
        public virtual string ReasonKindCode { get; set; }
        public abstract class reasonKindCode : PX.Data.BQL.BqlString.Field<reasonKindCode> { }
        #endregion
        #region BelongKindCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "BelongKindCode")]
        public virtual string BelongKindCode { get; set; }
        public abstract class belongKindCode : PX.Data.BQL.BqlString.Field<belongKindCode> { }
        #endregion
    }
}