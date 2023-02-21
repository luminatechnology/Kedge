using System;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Objects.FS;
using PX.CS;

namespace Kedge.DAC
{
    [Serializable]
    public class KGVendorPrice : IBqlTable
    {
        #region VendorPriceID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Vendor Price ID")]
        public virtual int? VendorPriceID { get; set; }
        public abstract class vendorPriceID : PX.Data.BQL.BqlInt.Field<vendorPriceID> { }
        #endregion
        /*
        #region VendorPriceCD
        [PXDBString(50, IsUnicode = true, InputMask = "" )]
        [PXUIField(DisplayName = "Vendor Price CD")]
        public virtual string VendorPriceCD { get; set; }
        public abstract class vendorPriceCD : PX.Data.BQL.BqlString.Field<vendorPriceCD> { }
        #endregion*/

        #region InventoryID
        //[PXDBInt(IsKey =true)]
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXSelector(typeof(InventoryItem.inventoryID), SubstituteKey = typeof(InventoryItem.inventoryCD))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion


        #region UnitPrice
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Unit Price")]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        #endregion

        #region AreaCode
        //[PXDBInt(IsKey = true)]
        [PXDBString(IsKey = true,IsUnicode =true)]
        [PXUIField(DisplayName = "Area Code")]
        /*[PXSelector(typeof(Search<INSite.siteID, Where<INSite.active, Equal<True>, And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>>>),
                    typeof(INSite.siteCD),
                    SubstituteKey = typeof(INSite.siteCD))]*/
        //2020/05/08 Althea Modify 改為string ,selector資料來源改為屬性,ID=AREACODE
        [PXSelectorWithCustomOrderBy(typeof(Search<CSAttributeDetail.valueID,
                    Where<CSAttributeDetail.attributeID, Equal<word.aREACODE>>,
                    OrderBy<Asc<CSAttributeDetail.sortOrder>>>),
                    typeof(CSAttributeDetail.sortOrder),
                    typeof(CSAttributeDetail.description),
                    SubstituteKey = typeof(CSAttributeDetail.description))]
        public virtual string AreaCode { get; set; }
        public abstract class areaCode : PX.Data.BQL.BqlString.Field<areaCode> { }
        #endregion

        #region VendorID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Vendor")]
        [PXSelector(typeof(Search<Vendor.bAccountID>)
            , typeof(Vendor.acctCD), typeof(Vendor.acctName), 
            SubstituteKey = typeof(Vendor.acctCD), DescriptionField = typeof(Vendor.acctName), Filterable = true)]
        //[Vendor()]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region ContactName
        [PXDBString(120, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contact Name")]
        public virtual string ContactName { get; set; }
        public abstract class contactName : PX.Data.BQL.BqlString.Field<contactName> { }
        #endregion

        #region ContactPhone
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contact Phone")]
        public virtual string ContactPhone { get; set; }
        public abstract class contactPhone : PX.Data.BQL.BqlString.Field<contactPhone> { }
        #endregion

        #region EffectiveDate
        [PXDBDate(IsKey = true)]
        [PXUIField(DisplayName = "Effective Date")]
        public virtual DateTime? EffectiveDate { get; set; }
        public abstract class effectiveDate : PX.Data.BQL.BqlDateTime.Field<effectiveDate> { }
        #endregion

        #region ExpirationDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Expiration Date", Enabled = false)]
        public virtual DateTime? ExpirationDate { get; set; }
        public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
        #endregion

        #region Item
        [PXDBString(240, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Item")]
        public virtual string Item { get; set; }
        public abstract class item : PX.Data.BQL.BqlString.Field<item> { }
        #endregion

        #region Uom
        [PXDBString(IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Uom")]
        [PXSelector(typeof(Search4<INUnit.fromUnit,
                    Where<INUnit.unitType, Equal<INUnitType.global>>,
                    Aggregate<GroupBy<INUnit.fromUnit>>>))]
        //
        //[INUnit()]
        //[PXSelector(typeof(Search<INItemClass.itemClassID>), Filterable = true)]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : IBqlField { }
        #endregion



        /*
        #region Memo
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Memo { get; set; }
        public abstract class memo : IBqlField { }
        #endregion*/


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
		[Branch()]
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

        //2020/05/12 ADD
        #region IsDelete
        [PXDBBool()]
        [PXUIField(DisplayName = "IsDelete")]
        [PXDefault(false,PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual bool? IsDelete { get; set; }
        public abstract class isDelete : PX.Data.BQL.BqlBool.Field<isDelete> { }
        #endregion

        #region Selected
        [PXBool()]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

    }
}