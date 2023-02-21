using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.PO;
using PX.Objects.IN;
using PX.Objects.GL;

namespace Kedge.DAC
{
      [Serializable]
      public class KGBillSegPricing : IBqlTable
      {
            #region BillSegPricingID
            [PXDBIdentity(IsKey = true)]
            [PXUIField(DisplayName = "Bill Seg Pricing ID")]
            public virtual int? BillSegPricingID { get; set; }
            public abstract class billSegPricingID : PX.Data.BQL.BqlInt.Field<billSegPricingID> { }
            #endregion
            #region SegPricingID
            [PXDBInt()]
            [PXUIField(DisplayName = "Seg Pricing ID")]
            public virtual int? SegPricingID { get; set; }
            public abstract class segPricingID : PX.Data.BQL.BqlInt.Field<segPricingID> { }
            #endregion
            #region APTranID
            public abstract class aPTranID : PX.Data.BQL.BqlInt.Field<aPTranID>
            {
            }
            protected Int32? _APTranID;
            [PXDBInt()]
            [PXParent(typeof(Select<APTran,
                                Where<APTran.tranID,
                                Equal<Current<KGBillSegPricing.aPTranID>>,
                                And<APTran.refNbr, Equal<Current<KGBillSegPricing.aPRefNbr>>>>>))]
            [PXDBDefault(typeof(APTran.tranID))]
            public virtual Int32? APTranID
            {
                get
                {
                    return this._APTranID;
                }
                set
                {
                    this._APTranID = value;
                }
            }
            #endregion


            #region APRefNbr
            [PXDBString(15, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "APRef Nbr")]
            [PXDBDefault(typeof(APTran.refNbr))]
            public virtual string APRefNbr { get; set; }
            public abstract class aPRefNbr : PX.Data.BQL.BqlString.Field<aPRefNbr> { }
            #endregion
            #region PODocType
            [PXDBString(2, IsFixed = true, InputMask = "")]
            [PXUIField(DisplayName = "PODoc Type")]
            public virtual string PODocType { get; set; }
            public abstract class pODocType : PX.Data.BQL.BqlString.Field<pODocType> { }
            #endregion
            #region POOrderNbr
            [PXDBString(15, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "POOrder Nbr")]
            public virtual string POOrderNbr { get; set; }
            public abstract class pOOrderNbr : PX.Data.BQL.BqlString.Field<pOOrderNbr> { }
            #endregion
            #region POLineNbr
            [PXDBInt()]
            [PXUIField(DisplayName = "POLine Nbr")]
            public virtual int? POLineNbr { get; set; }
            public abstract class pOLineNbr : PX.Data.BQL.BqlInt.Field<pOLineNbr> { }
            #endregion
            #region Poqty
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Poqty", Enabled = false)]
            /*
            [PXDBScalar(typeof(Search<POLine.orderQty,
                                        Where<POLine.orderNbr, Equal<pOOrderNbr>,
                                        And<POLine.lineNbr, Equal<pOLineNbr>>>>))]*/
            public virtual Decimal? Poqty { get; set; }
            public abstract class poqty : PX.Data.BQL.BqlDecimal.Field<poqty> { }
            #endregion
            #region SortOrder
            [PXDBInt()]
            [PXUIField(DisplayName = "Sort Order",Enabled =false)]
            public virtual int? SortOrder { get; set; }
            public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }
            #endregion
            #region SegmentName
            [PXDBString(IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Segment Name", Enabled = false)]
            public virtual string SegmentName { get; set; }
            public abstract class segmentName : PX.Data.BQL.BqlString.Field<segmentName> { }
            #endregion
            #region SegmentPercent
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Segment Percent", Enabled = false)]
            public virtual Decimal? SegmentPercent { get; set; }
            public abstract class segmentPercent : PX.Data.BQL.BqlDecimal.Field<segmentPercent> { }
            #endregion
            #region BillQty
            [PXDBDecimal(4)]
            [PXDefault(TypeCode.Decimal, "0.0")]
            [PXUIField(DisplayName = "Bill Qty")]
            public virtual Decimal? BillQty { get; set; }
            public abstract class billQty : PX.Data.BQL.BqlDecimal.Field<billQty> { }
            #endregion

            #region BillCumulativeQty
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Bill Cumulative Qty", Enabled = false)]
            public virtual Decimal? BillCumulativeQty { get; set; }
            public abstract class billCumulativeQty : PX.Data.BQL.BqlDecimal.Field<billCumulativeQty> { }
            #endregion

            #region UOM
            public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
            protected String _UOM;


            [PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<POLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
            [INUnit(typeof(POLine.inventoryID), DisplayName = "UOM")]
            public virtual String UOM
            {
                get
                {
                    return this._UOM;
                }
                set
                {
                    this._UOM = value;
                }
            }
            #endregion
            #region POUnitCost
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Unit Cost", Enabled = false)]
            public virtual Decimal? POUnitCost { get; set; }
            public abstract class pOUnitCost : PX.Data.BQL.BqlDecimal.Field<pOUnitCost> { }
            #endregion

            #region POAmt
            [PXDBDecimal()]
            [PXUIField(DisplayName = "PO Amt", Enabled = false)]
            [PXFormula(typeof(Mult<poqty, pOUnitCost>))]
            public virtual Decimal? POAmt { get; set; }
            public abstract class pOAmt : PX.Data.BQL.BqlDecimal.Field<pOAmt> { }
            #endregion
            #region BillAmt
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Bill Amt", Enabled = false)]
            [PXFormula(typeof(Mult<billQty, pOUnitCost>))]
            public virtual Decimal? BillAmt { get; set; }
            public abstract class billAmt : PX.Data.BQL.BqlDecimal.Field<billAmt> { }
            #endregion
            #region BillCumulativeAmt
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Bill Cumulative Amt", Enabled = false)]
            [PXFormula(typeof(Mult<billCumulativeQty, pOUnitCost>))]
            public virtual Decimal? BillCumulativeAmt { get; set; }
            public abstract class billCumulativeAmt : PX.Data.BQL.BqlDecimal.Field<billCumulativeAmt> { }
            #endregion

            #region NoteID
            [PXNote()]
                        [PXUIField(DisplayName = "NoteID")]
                        public virtual Guid? NoteID { get; set; }
                        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
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
            #region Tstamp
            [PXDBTimestamp()]
            [PXUIField(DisplayName = "Tstamp")]
            public virtual byte[] Tstamp { get; set; }
            public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
            /*#region BranchID
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
            {
            }
            protected Int32? _BranchID;

            /// <summary>
            /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
            /// </summary>
            /// <value>
            /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
            /// </value>
		    [Branch(typeof(APRegister.branchID))]
            public virtual Int32? BranchID
            {
                get
                {
                    return this._BranchID;
                }
                set
                {
                    this._BranchID = value;
                }
            }
            #endregion*/
    }
}