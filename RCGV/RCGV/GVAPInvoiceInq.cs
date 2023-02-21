using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.AP;
using RCGV.GV.Attribute.DropDown;


namespace RCGV.GV
{
	public class GVApInvoiceInq : PXGraph<GVApInvoiceInq>
	{
		#region Select
		public PXFilter<GVApGuiInvoiceFilter> Master;
		[PXCopyPasteHiddenFields]
		[PXFilterable]
		public PXSelectReadonly<GVApGuiInvoice> Details;
        // The delegate for the AddItemDetailView data view
        protected virtual IEnumerable details()
        {
            // Creating a dynamic query
            // Data is ordered by the productID (and supplierID)
            PXSelectBase<GVApGuiInvoice> query =
            new PXSelectOrderBy<GVApGuiInvoice,
            OrderBy<Asc<GVApGuiInvoice.guiInvoiceID>>>(this);
            // Adding filtering conditions to the query
            GVApGuiInvoiceFilter filter = Master.Current;
            if (filter.InvoiceNbrFrom != null)
                query.WhereAnd<Where<GVApGuiInvoice.guiInvoiceNbr,
                    GreaterEqual<Current<GVApGuiInvoiceFilter.invoiceNbrFrom>>>>();
            if (filter.InvoiceNbrTo != null)
                query.WhereAnd<Where<GVApGuiInvoice.guiInvoiceNbr,
                    LessEqual<Current<GVApGuiInvoiceFilter.invoiceNbrTo>>>>();
            if (filter.DateFrom != null)
                query.WhereAnd<Where<GVApGuiInvoice.invoiceDate,
                    GreaterEqual<Current<GVApGuiInvoiceFilter.dateFrom>>>>();
            if (filter.DateTo != null)
                query.WhereAnd<Where<GVApGuiInvoice.invoiceDate,
                    LessEqual<Current<GVApGuiInvoiceFilter.dateTo>>>>();
            if (filter.GuiType != null)
                query.WhereAnd<Where<GVApGuiInvoice.guiType,
                    Equal<Current<GVApGuiInvoiceFilter.guiType>>>>();
            if (filter.VendorID != null)
                query.WhereAnd<Where<GVApGuiInvoice.vendor,
                    Equal<Current<GVApGuiInvoiceFilter.vendorID>>>>();
            if (filter.GuiType != null)
                query.WhereAnd<Where<GVApGuiInvoice.guiType,
                    Equal<Current<GVApGuiInvoiceFilter.guiType>>>>();
            return query.Select();
        }

        /*protected virtual IEnumerable details()
		{
			GVApGuiInvoiceFilter filter = Master.Current;
			int startRow = PXView.StartRow;
			int totalRows = 0;
			BqlCommand query = Details.View.BqlSelect;
			FilterInvoices(ref query, filter);
			PXView accountView = new PXView(this, true, query);
			var list = accountView.Select(
				PXView.Currents, PXView.Parameters, PXView.Searches,
				PXView.SortColumns, PXView.Descendings, PXView.Filters,
				ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;
			return list;
		}*/

        /*private void FilterInvoices(ref BqlCommand query, GVApGuiInvoiceFilter filter)
		{
            if(filter.InvoiceNbrFrom==null && filter.InvoiceNbrTo==null&&
               filter.DateFrom ==null && filter.DateTo ==null &&
               filter.VendorID == null && filter.GuiType==null)
            {
                
            }
			// Filter Nbr
			if (filter.InvoiceNbrFrom != null && filter.InvoiceNbrTo != null)
			{
				if (filter.InvoiceNbrFrom.CompareTo(filter.InvoiceNbrTo) < 0)
				{
					query = query.WhereAnd<
						Where<GVApGuiInvoice.guiInvoiceNbr,
							Between<Current<GVApGuiInvoiceFilter.invoiceNbrFrom>, Current<GVApGuiInvoiceFilter.invoiceNbrTo>>>>();
				}
				else
				{
					query = query.WhereAnd<
					 Where<GVApGuiInvoice.guiInvoiceNbr,
						 Between<Current<GVApGuiInvoiceFilter.invoiceNbrTo>, Current<GVApGuiInvoiceFilter.invoiceNbrFrom>>>>();
				}
			}
			else
			{
				if (filter.InvoiceNbrFrom != null)
				{
					query = query.WhereAnd<
						Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Current<GVApGuiInvoiceFilter.invoiceNbrFrom>>>>();
				}
				else
				{
					query = query.WhereAnd<
						Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Current<GVApGuiInvoiceFilter.invoiceNbrTo>>>>();
				}
			}

			// filter date range
			if (filter.DateFrom != null)
			{
				query = query.WhereAnd<
					Where<GVApGuiInvoice.invoiceDate, GreaterEqual<Current<GVApGuiInvoiceFilter.dateFrom>>>>();
			}
			if (filter.DateTo != null)
			{
				query = query.WhereAnd<
					Where<GVApGuiInvoice.invoiceDate, LessEqual<Current<GVApGuiInvoiceFilter.dateTo>>>>();
			}

			if (filter.GuiType != null)
			{
				query = query.WhereAnd<
					Where<GVApGuiInvoice.guiType, Equal<Current<GVApGuiInvoiceFilter.guiType>>>>();
			}

            // filter by Vedor
            if (filter.VendorID != null)
            {
                query = query.WhereAnd<
                        Where<GVApGuiInvoice.vendor, Equal<Current<GVApGuiInvoiceFilter.vendorID>>>>();
            }

        }*/
        #endregion

        #region Action
        public PXAction<GVApGuiInvoiceFilter> ViewGVApGuiInvoice;
        [PXButton]
        protected virtual void viewGVApGuiInvoice()
        {
            GVApGuiInvoice row = Details.Current;
            // Creating the instance of the graph
            GVApInvoiceEntry graph = PXGraph.CreateInstance<GVApInvoiceEntry>();
            // Setting the current product for the graph
            graph.Invoice.Current = graph.Invoice.Search<GVApGuiInvoice.guiInvoiceNbr>(
                                         row.GuiInvoiceNbr);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Invoice.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "ApGuiInvocie Details");
            }
        }
        #endregion
    }

    [Serializable]
	public class GVApGuiInvoiceFilter : IBqlTable
	{
		#region InvoiceNbrFrom
		public abstract class invoiceNbrFrom : PX.Data.IBqlField
		{
		}
		protected string _InvoiceNbrFrom;
		[PXString(40)]
		[PXDefault]
		[PXUIField(DisplayName = "InvoiceNbr From")]
		[PXSelector(
			typeof(Search<GVApGuiInvoice.guiInvoiceNbr>),
			typeof(GVApGuiInvoice.guiInvoiceNbr),
			typeof(GVApGuiInvoice.deductionCode),
			typeof(GVApGuiInvoice.guiType)
		)]
		public virtual string InvoiceNbrFrom
		{
			get
			{
				return this._InvoiceNbrFrom;
			}
			set
			{
				this._InvoiceNbrFrom = value;
			}
		}
		#endregion
		#region InvoiceNbrTo
		public abstract class invoiceNbrTo : PX.Data.IBqlField
		{
		}
		protected string _InvoiceNbrTo;
		[PXString(40)]
		[PXDefault]
		[PXUIField(DisplayName = "InvoiceNbr To")]
		[PXSelector(
			typeof(Search<GVApGuiInvoice.guiInvoiceNbr>),
			typeof(GVApGuiInvoice.guiInvoiceNbr),
			typeof(GVApGuiInvoice.deductionCode),
			typeof(GVApGuiInvoice.guiType)
		)]
		public virtual string InvoiceNbrTo
		{
			get
			{
				return this._InvoiceNbrTo;
			}
			set
			{
				this._InvoiceNbrTo = value;
			}
		}
		#endregion
		#region DateFrom
		public abstract class dateFrom : PX.Data.IBqlField
		{
		}
		protected DateTime _DateFrom;
		[PXDate]
		[PXUIField(DisplayName = "Invoice Date From")]
		public virtual DateTime? DateFrom
		{
			get;
			set;
		}
		#endregion
		#region DateTo
		public abstract class dateTo : PX.Data.IBqlField
		{
		}
		protected DateTime _DateTo;
		[PXDate]
		[PXUIField(DisplayName = "Invoice Date To")]
		public virtual DateTime? DateTo
		{
			get;
			set;
		}
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
        [PXUIField(DisplayName = "Seller")]
        public virtual int? VendorID
		{
			get;
			set;
		}

		#endregion
		#region GuiType
		public abstract class guiType : PX.Data.IBqlField
		{
		}
		[PXString(1, IsFixed = true)]
		[PXUIField(DisplayName = "GUI Type")]
        [GuiType(GuiTypeAttribute.Input)]
        public virtual string GuiType { get; set; }
		#endregion
	}
}