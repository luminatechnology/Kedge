using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.AP;
using RCGV.GV.DAC;
using RCGV.GV.Attribute.Selector;

namespace RCGV.GV
{
	public class GVApCmInvoiceInq : PXGraph<GVApCmInvoiceInq, GVApGuiCmInvoice>
	{
		#region Select
		public PXFilter<GVApCmFilter> AddItemMasterView;
		/// <summary>
		/// SmartPanel DetailView
		/// </summary>
		[PXCopyPasteHiddenFields]
		[PXFilterable]
		public PXSelectReadonly<GVApGuiCmInvoice> AddItemDetailView;

        // The delegate for the AddItemDetailView data view
        protected virtual IEnumerable addItemDetailView()
        {
            // Creating a dynamic query
            // Data is ordered by the productID (and supplierID)
            PXSelectBase<GVApGuiCmInvoice> query =
            new PXSelectReadonly<GVApGuiCmInvoice>(this);
            // Adding filtering conditions to the query
            GVApCmFilter filter = AddItemMasterView.Current;
            if (filter.APInvDateFrom != null)
                query.WhereAnd<Where<GVApGuiCmInvoice.invoiceDate,
                    GreaterEqual<Current<GVApCmFilter.aPInvDateFrom>>>>();
            if (filter.APInvDateTo != null)
                query.WhereAnd<Where<GVApGuiCmInvoice.invoiceDate,
                    LessEqual<Current<GVApCmFilter.aPInvDateTo>>>>();
            if (filter.GuiCmInvoiceNbrFrom != null)
                query.WhereAnd<Where<GVApGuiCmInvoice.guiCmInvoiceNbr,
                    GreaterEqual<Current<GVApCmFilter.guiCmInvoiceNbrFrom>>>>();
            if (filter.GuiCmInvoiceNbrTo != null)
                query.WhereAnd<Where<GVApGuiCmInvoice.guiCmInvoiceNbr,
                    LessEqual<Current<GVApCmFilter.guiCmInvoiceNbrTo>>>>();
            if (filter.RegistrationCD != null)
                query.WhereAnd<Where<GVApGuiCmInvoice.registrationCD,
                    Equal<Current<GVApCmFilter.registrationCD>>>>();
            if (filter.VendorID != null)
                query.WhereAnd<Where<GVApGuiCmInvoice.vendorID,
                    Equal<Current<GVApCmFilter.vendorID>>>>();

            return query.Select();
        }
        /*protected virtual IEnumerable addItemDetailView()
		{
			GVApCmFilter filter = AddItemMasterView.Current;
			int startRow = PXView.StartRow;
			int totalRows = 0;
			BqlCommand query = AddItemDetailView.View.BqlSelect;
			FilterCmInvoices(ref query, filter);
			PXView accountView = new PXView(this, true, query);
			var list = accountView.Select(
				PXView.Currents, PXView.Parameters, PXView.Searches,
				PXView.SortColumns, PXView.Descendings, PXView.Filters,
				ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;
			return list;
		}*/

        /*private void FilterCmInvoices(ref BqlCommand query, GVApCmFilter filter)
		{
			// filter by register id
			if (filter.RegistrationCD != null)
			{
				query = query.WhereAnd<
						Where<GVApGuiCmInvoice.registrationCD, Equal<Current<GVApCmFilter.registrationCD>>>>();
			}

			// Filter Nbr
			if (filter.GuiCmInvoiceNbrFrom != null)
			{
				query = query.WhereAnd<
						Where<GVApGuiCmInvoice.guiCmInvoiceNbr, Equal<Current<GVApCmFilter.guiCmInvoiceNbrFrom>>>>();
			}
			
			if (filter.GuiCmInvoiceNbrTo != null)
			{
				query = query.WhereAnd<
					Where<GVApGuiCmInvoice.guiCmInvoiceNbr, Equal<Current<GVApCmFilter.guiCmInvoiceNbrTo>>>>();
				
			}

			// filter date range
			if (filter.APInvDateFrom != null)
			{
				query = query.WhereAnd<
					Where<GVApGuiCmInvoice.invoiceDate, GreaterEqual<Current<GVApCmFilter.aPInvDateFrom>>>>();
			}
			if (filter.APInvDateTo != null)
			{
				query = query.WhereAnd<
					Where<GVApGuiCmInvoice.invoiceDate, LessEqual<Current<GVApCmFilter.aPInvDateTo>>>>();
			}

			// filter by Vedor
			if (filter.VendorID != null)
			{
				query = query.WhereAnd<
						Where<GVApGuiCmInvoice.sellerID, Equal<Current<GVApCmFilter.vendorID>>>>();
			}
			
		}*/

		#endregion

		#region Events

		public virtual void GVApCmFilter_RegistrationCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			GVApCmFilter row = (GVApCmFilter)e.Row;
			GVRegistration registration = null;

			if (row.RegistrationCD != null)
			{
				registration = PXSelectorAttribute.Select<GVApCmFilter.registrationCD>(sender, row) as GVRegistration;
				if (registration != null)
				{
					row.GovUniformNumber = registration.GovUniformNumber;
				}
			}
			if (registration == null)
			{
				row.GovUniformNumber = null;
			}
		}
        #endregion

        #region Action
        public PXAction<GVApCmFilter> ViewGVApGuiCmInvoice;
        [PXButton]
        protected virtual void viewGVApGuiCmInvoice()
        {
            GVApGuiCmInvoice row = AddItemDetailView.Current;
            // Creating the instance of the graph
            GVApCmInvoiceEntry graph = PXGraph.CreateInstance<GVApCmInvoiceEntry>();
            // Setting the current product for the graph
            graph.GuiCmInvoice.Current = graph.GuiCmInvoice.Search<GVApGuiCmInvoice.guiCmInvoiceNbr>(
                                         row.GuiCmInvoiceNbr);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.GuiCmInvoice.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "ApCmInvocie Details");
            }
        }
        #endregion
    }

    #region GVApCmFilter
    [Serializable]
	public class GVApCmFilter : IBqlTable
	{
        #region RegistrationCD
        public abstract class registrationCD : PX.Data.IBqlField
        {
        }
        protected string _RegistrationCD;
        [PXDBString(9, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Registration CD", Required = true)]
        [RegistrationCDAttribute()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string RegistrationCD
        {
            get
            {
                return this._RegistrationCD;
            }
            set
            {
                this._RegistrationCD = value;
            }
        }
        #endregion
        #region GovUniformNumber
        public abstract class govUniformNumber : PX.Data.IBqlField
		{
		}
		protected string _GovUniformNumber;
		[PXDBString(8, IsUnicode = true)]
		[PXUIField(DisplayName = "GovUniformNumber", Enabled = false)]
		public virtual string GovUniformNumber
		{
			get
			{
				return this._GovUniformNumber;
			}
			set
			{
				this._GovUniformNumber = value;
			}
		}
		#endregion
		#region GuiCmInvoiceNbrFrom
		public abstract class guiCmInvoiceNbrFrom : PX.Data.IBqlField
		{
		}
		protected string _GuiCmInvoiceNbrFrom;
		[PXString(15)]
		[PXDefault()]
		[PXUIField(DisplayName = "CmInvoiceNbr From")]
		[PXSelector(
			typeof(Search<GVApGuiCmInvoice.guiCmInvoiceNbr>),
			typeof(GVApGuiCmInvoice.guiCmInvoiceNbr),
			typeof(GVApGuiCmInvoice.guiType),
			typeof(GVApGuiCmInvoice.salesAmt),
			typeof(GVApGuiCmInvoice.taxAmt)
		)]
		public virtual string GuiCmInvoiceNbrFrom
		{
			get
			{
				return this._GuiCmInvoiceNbrFrom;
			}
			set
			{
				this._GuiCmInvoiceNbrFrom = value;
			}
		}

		#endregion
		#region GuiCmInvoiceNbrTo
		public abstract class guiCmInvoiceNbrTo : PX.Data.IBqlField
		{
		}
		protected string _GuiCmInvoiceNbrTo;
		[PXString(15)]
		[PXDefault()]
		[PXUIField(DisplayName = "CmInvoiceNbr To")]
		[PXSelector(
			typeof(Search<GVApGuiCmInvoice.guiCmInvoiceNbr>),
			typeof(GVApGuiCmInvoice.guiCmInvoiceNbr),
			typeof(GVApGuiCmInvoice.guiType),
			typeof(GVApGuiCmInvoice.salesAmt),
			typeof(GVApGuiCmInvoice.taxAmt)
		)]
		public virtual string GuiCmInvoiceNbrTo
		{
			get
			{
				return this._GuiCmInvoiceNbrTo;
			}
			set
			{
				this._GuiCmInvoiceNbrTo = value;
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
        [PXDefault]
        [PXUIField(DisplayName = "Seller")]
        //[PXForeignReference(typeof(Field<GVApCmFilter.vendorID>.IsRelatedTo<BAccount.bAccountID>))]
        public virtual int? VendorID
		{
			get;
			set;
		}

		#endregion
	}
	#endregion
}