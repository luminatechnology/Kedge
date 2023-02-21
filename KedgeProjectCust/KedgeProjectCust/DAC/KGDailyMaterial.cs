using System;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.GL;

namespace Kedge.DAC
{
      


    [Serializable]
    public class KGDailyMaterial : IBqlTable
    {

        public sealed class NONSTOCK : Constant<String>
        {
            public NONSTOCK() : base("NONSTOCK")
            {

            }
        }
        public sealed class ACTIVE : Constant<String>
        {
            public ACTIVE() : base("AC")
            {

            }
        }
        public sealed class INVAUDIT : Constant<String>
        {
            public INVAUDIT() : base("INVAUDIT")
            {

            }
        }
        public sealed class One : Constant<String>
        {
            public One() : base("1")
            {

            }
        }

        #region Selected
        public abstract class selected : IBqlField
        { }
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region DailyMatID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Daily Mat ID")]
        public virtual int? DailyMatID { get; set; }
        public abstract class dailyMatID : IBqlField { }
        #endregion

        #region DailyReportID
        [PXDBInt()]
        [PXUIField(DisplayName = "Daily Report ID")]
        [PXDBDefault(typeof(KGDailyReport.dailyReportID))]
        [PXParent(typeof(Select<KGDailyReport,
                                Where<KGDailyReport.dailyReportID,
                                Equal<Current<KGDailyMaterial.dailyReportID>>>>))]
        public virtual int? DailyReportID { get; set; }
        public abstract class dailyReportID : IBqlField { }
        #endregion

        #region Qty
        [PXDBDecimal(2,MinValue =0,MaxValue =9.99)]
        [PXUIField(DisplayName = "Qty")]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : IBqlField { }
        #endregion
        //IN201000-品項類別，IN202000工料品項 ，CS205000屬性
        #region MaterialID
        [PXDBInt()]
        [PXUIField(DisplayName = "Material ID")]
        [PXCheckUnique(Where = typeof(Where<dailyReportID, Equal<Current<dailyReportID>>,
                         And<materialID, Equal<Current<materialID>>>>))]
        [PXSelector(typeof(Search2<InventoryItem.inventoryID,
            LeftJoin<INItemClass, On<INItemClass.itemClassID, Equal<InventoryItem.itemClassID>>,
            LeftJoin<CSAnswers, On<CSAnswers.refNoteID, Equal<InventoryItem.noteID>,And<CSAnswers.attributeID,Equal<INVAUDIT>>>>>,
                Where<INItemClass.itemClassCD,Equal<NONSTOCK>,And<InventoryItem.itemStatus,Equal<ACTIVE>,And<CSAnswers.value, Equal<One>>>>>), SubstituteKey = typeof(InventoryItem.inventoryCD),DescriptionField = typeof(InventoryItem.descr))]
        //NONSTOCK
        public virtual int? MaterialID { get; set; }
        public abstract class materialID : IBqlField { }
        #endregion

        #region UOM
        public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
        protected String _UOM;


        //[PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<POLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        //[INUnit(typeof(POLine.inventoryID), DisplayName = "UOM")]
        [PXString(10)]
        [PXUIField(DisplayName = "UOM",Enabled =false)]
        [PXDBScalar(typeof(Search<InventoryItem.baseUnit,
            Where<InventoryItem.inventoryID, Equal<materialID>
            >>))]
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
		[Branch(typeof(KGDailyReport.branchID))]
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