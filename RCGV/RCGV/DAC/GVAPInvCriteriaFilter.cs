using System;
using PX.Data;
using RCGV.GV.Util;
using PX.Objects.AP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using RCGV.GV.Attribute.Selector;

namespace RCGV.GV.DAC
{
    #region GVAPInvCriteriaFilter
    [Serializable]
    public class GVAPInvCriteriaFilter : IBqlTable
    {
        #region APInvNbrFrom
        public abstract class aPInvNbrFrom : PX.Data.IBqlField
        {
        }
        protected string _APInvNbrFrom;
        [PXString(15)]
        [PXDefault()]
        [PXUIField(DisplayName = "InvoiceNbr From")]
        [APInvoiceType.RefNbr(typeof(Search2<APInvoice.refNbr,
            InnerJoinSingleTable<Vendor, On<APInvoice.vendorID, Equal<Vendor.bAccountID>>>,
            Where<APInvoice.released, Equal<True>,
                And<APInvoice.docType, Equal<APDocType.invoice>,
                    And<APRegisterGVExt.usrApGuiInvoiceNbr, IsNull,
                And<Vendor.bAccountID, Equal<Current<GVAPInvCriteriaFilter.vendorID>>>>>>>
            ), Filterable = true
        )]
        
        public virtual string APInvNbrFrom
        {
            get
            {
                return this._APInvNbrFrom;
            }
            set
            {
                this._APInvNbrFrom = value;
            }
        }

        #endregion
        #region APInvNbrTo
        public abstract class aPInvNbrTo : PX.Data.IBqlField
        {
        }
        protected string _APInvNbrTo;
        [PXString(15)]
        [PXDefault()]
        [PXUIField(DisplayName = "InvoiceNbr To")]
        [APInvoiceType.RefNbr(typeof(Search2<APInvoice.refNbr,
            InnerJoinSingleTable<Vendor, On<APInvoice.vendorID, Equal<Vendor.bAccountID>>>,
            Where<APInvoice.released, Equal<True>,
                And<APInvoice.docType, Equal<APDocType.invoice>,
                    And<APRegisterGVExt.usrApGuiInvoiceNbr, IsNull,
                And<Vendor.bAccountID, Equal<Current<GVAPInvCriteriaFilter.vendorID>>>>>>>
            ), Filterable = true
        )]
        public virtual string APInvNbrTo
        {
            get
            {
                return this._APInvNbrTo;
            }
            set
            {
                this._APInvNbrTo = value;
            }
        }

        #endregion
        #region APInvDateFrom
        public abstract class aPInvDateFrom : IBqlField { }
        [PXDate]
        [PXUIField(DisplayName = "Invoice Date From")]
        public virtual DateTime? APInvDateFrom { get; set; }
        #endregion
        #region APInvDateTo
        public abstract class aPInvDateTo : IBqlField { }
        [PXDate]
        [PXUIField(DisplayName = "Invoice Date To")]
        public virtual DateTime? APInvDateTo { get; set; }
        #endregion
        #region Vendor
        public abstract class vendorID : PX.Data.IBqlField
        {
        }
        /// <summary>
        /// Identifier of the <see cref="Vendor"/>, whom the document belongs to.
        /// </summary>
        [VendorActive(
            Visibility = PXUIVisibility.SelectorVisible,
            DescriptionField = typeof(Vendor.acctName),
            CacheGlobal = true,
            Filterable = true)]
        [PXUIField(DisplayName = "Seller", IsReadOnly = true)]
        public virtual int? VendorID
        {
            get;
            set;
        }

        #endregion
    }
    #endregion
}
