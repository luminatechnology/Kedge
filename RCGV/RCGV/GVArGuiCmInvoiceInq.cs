using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.AR;

namespace RCGV.GV
{
	public class GVArGuiCmInvoiceInq : PXGraph<GVArGuiCmInvoiceInq>
	{
		public PXFilter<GVArGuiCmInvoiceFilter> gVArGuiCmInvoiceFilters;


		[PXFilterable]
		public PXSelectReadonly<GVArGuiCmInvoice> gVArGuiCmInvoices;
		protected virtual IEnumerable GVArGuiCmInvoices()
		{
			// Creating a dynamic query
			// Data is ordered by the productID (and supplierID)
			PXSelectBase<GVArGuiCmInvoice> query =
			new PXSelectReadonly<GVArGuiCmInvoice>(this);
			// Adding filtering conditions to the query
			GVArGuiCmInvoiceFilter filter = gVArGuiCmInvoiceFilters.Current;
			if (filter.CMInvoiceDateFrom != null)
				query.WhereAnd<Where<GVArGuiCmInvoice.invoiceDate,
					GreaterEqual<Current<GVArGuiCmInvoiceFilter.cMInvoiceDateFrom>>>>();
			if (filter.CMInvoiceDateTo != null)
				query.WhereAnd<Where<GVArGuiCmInvoice.invoiceDate,
					LessEqual<Current<GVArGuiCmInvoiceFilter.cMInvoicedateTo>>>>();
            if (filter.GuiCmInvoiceNbrFrom != null)
                query.WhereAnd<Where<GVArGuiCmInvoice.guiCmInvoiceNbr,
                    GreaterEqual<Current<GVArGuiCmInvoiceFilter.guiCmInvoiceNbrFrom>>>>();
            if (filter.GuiCmInvoiceNbrTo != null)
                query.WhereAnd<Where<GVArGuiCmInvoice.guiCmInvoiceNbr,
                    LessEqual<Current<GVArGuiCmInvoiceFilter.guiCmInvoiceNbrTo>>>>();
            if (filter.RegistrationCD != null)
				query.WhereAnd<Where<GVArGuiCmInvoice.registrationCD,
					Equal<Current<GVArGuiCmInvoiceFilter.registrationCD>>>>();
			if (filter.CustomerID != null)
				query.WhereAnd<Where<GVArGuiCmInvoice.customerID,
					Equal<Current<GVArGuiCmInvoiceFilter.customerID>>>>();
			
			return query.Select();
		}
		public PXAction<GVArGuiCmInvoiceFilter> ViewGVArGuiCmInvoice;
		[PXButton]
		protected virtual void viewGVArGuiCmInvoice()
		{
			GVArGuiCmInvoice row = gVArGuiCmInvoices.Current;
			// Creating the instance of the graph
			GVArGuiCmInvoiceMaint graph = PXGraph.CreateInstance<GVArGuiCmInvoiceMaint>();
			// Setting the current product for the graph
			graph.gvArGuiInvoices.Current = graph.gvArGuiInvoices.Search<GVArGuiCmInvoice.guiCmInvoiceNbr>(
										 row.GuiCmInvoiceNbr);
			// If the product is found by its ID, throw an exception to open
			// a new window (tab) in the browser
			if (graph.gvArGuiInvoices.Current != null)
			{
				throw new PXRedirectRequiredException(graph, true, "ArGuiCmInvocie Details");
			}
		}
	}
	[Serializable]
	public class GVArGuiCmInvoiceFilter : IBqlTable
	{
		
        #region GuiCmInvoiceNbrFrom
        public abstract class guiCmInvoiceNbrFrom : PX.Data.IBqlField
        {
        }
        protected string _GuiCmInvoiceNbrFrom;
        [PXSelector(
            typeof(Search<GVArGuiCmInvoice.guiCmInvoiceNbr>),
            typeof(GVArGuiCmInvoice.guiCmInvoiceNbr),
            typeof(GVArGuiCmInvoice.customerID),
            typeof(GVArGuiCmInvoice.registrationCD),
                SubstituteKey = typeof(GVArGuiCmInvoice.guiCmInvoiceNbr)

        )]
        [PXString(40)]
        [PXUIField(DisplayName = "Gui Cm Invoice Nbr From")]
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
        [PXSelector(
            typeof(Search<GVArGuiCmInvoice.guiCmInvoiceNbr>),
            typeof(GVArGuiCmInvoice.guiCmInvoiceNbr),
            typeof(GVArGuiCmInvoice.customerID),
            typeof(GVArGuiCmInvoice.registrationCD),
                SubstituteKey = typeof(GVArGuiCmInvoice.guiCmInvoiceNbr)

        )]
        [PXString(40)]
        [PXUIField(DisplayName = "Gui Cm Invoice Nbr To")]
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
		#region CMInvoiceDateFrom
		public abstract class cMInvoiceDateFrom : PX.Data.IBqlField
		{
		}
		protected DateTime _CMInvoiceDateFrom;
		[PXDate]
		[PXUIField(DisplayName = "CM Invoice Date From")]
		public virtual DateTime? CMInvoiceDateFrom
		{
			get;
			set;
		}
		#endregion
		#region CMInvoiceDateTo
		public abstract class cMInvoicedateTo : PX.Data.IBqlField
		{
		}
		protected DateTime _CMInvoiceDateTo;
		[PXDate]
		[PXUIField(DisplayName = "CM Invoice Date To")]
		public virtual DateTime? CMInvoiceDateTo
		{
			get;
			set;
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected int? _CustomerID;
		//[PXDBInt()]
		//[PXUIField(DisplayName = "CustomerID")]
		[CustomerActive(Visibility = PXUIVisibility.SelectorVisible,
			DescriptionField = typeof(Customer.acctName), Filterable = true, TabOrder = 2)]
		public virtual int? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region RegistrationCD
		public abstract class registrationCD : PX.Data.IBqlField
		{
		}
		protected string _RegistrationCD;
		[PXDBString(9, IsUnicode = true)]
		[PXUIField(DisplayName = "RegistrationCD")]
		[PXSelector(
			typeof(Search<GVRegistration.registrationCD>),
			typeof(GVRegistration.registrationCD))]
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
	}
}