using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.AR;
using RCGV.GV.Attribute.Selector;
using RCGV.GV.Attribute.DropDown;
using RCGV.GV.Util;

namespace RCGV.GV
{
	public class GVArInvoiceInq : PXGraph<GVArInvoiceInq>
	{
        public PXSelect<GVGuiType, Where<GVGuiType.gvType, Equal<GVTypeO.GVTypeO1>,
            And<GVGuiType.isActive, Equal<True>>>> gvs;
        protected virtual void GVArGuiInvoiceFilter_GuiType_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            GVArGuiInvoiceFilter row = (GVArGuiInvoiceFilter)e.Row;
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (GVGuiType GV in gvs.Select())
            {
                keys.Add(GV.GuiTypeCD);
                values.Add(GV.GuiTypeCD + "-" + GV.GuiTypeDesc);
            }
            //PXStringListAttribute.SetList<GVGuiWord.guiType>
                      // (sender, row, keys.ToArray(), values.ToArray());
        }
        public PXFilter<GVArGuiInvoiceFilter> gVArGuiInvoiceFilters;
		
		[PXFilterable]
		public PXSelectReadonly<GVArGuiInvoice> gvArGuiInvoices;
		protected virtual IEnumerable GvArGuiInvoices()
		{
			// Creating a dynamic query
			// Data is ordered by the productID (and supplierID)
			PXSelectBase<GVArGuiInvoice> query =
			new PXSelectOrderBy<GVArGuiInvoice,
			OrderBy<Asc<GVArGuiInvoice.guiInvoiceID>>>(this);
			// Adding filtering conditions to the query
			GVArGuiInvoiceFilter filter = gVArGuiInvoiceFilters.Current;
            if (filter.GuiInvoiceNbrFrom != null)
                query.WhereAnd<Where<GVArGuiInvoice.guiInvoiceNbr,
                    GreaterEqual<Current<GVArGuiInvoiceFilter.guiInvoiceNbrFrom>>>>();
            if (filter.GuiInvoiceNbrTo != null)
                query.WhereAnd<Where<GVArGuiInvoice.guiInvoiceNbr,
                    LessEqual<Current<GVArGuiInvoiceFilter.guiInvoiceNbrTo>>>>();
            if (filter.DateFrom != null)
				query.WhereAnd<Where<GVArGuiInvoice.invoiceDate,
					GreaterEqual<Current<GVArGuiInvoiceFilter.dateFrom>>>>();
			if (filter.DateTo != null)
				query.WhereAnd<Where<GVArGuiInvoice.invoiceDate,
					LessEqual<Current<GVArGuiInvoiceFilter.dateTo>>>>();           
            if (filter.GuiType != null)
				query.WhereAnd<Where<GVArGuiInvoice.guiType,
					Equal<Current<GVArGuiInvoiceFilter.guiType>>>>();
			if(filter.CustomerID != null)
				query.WhereAnd<Where<GVArGuiInvoice.customerID,
					Equal<Current<GVArGuiInvoiceFilter.customerID>>>>();
			if(filter.RegistrationCD != null)
				query.WhereAnd<Where<GVArGuiInvoice.registrationCD,
					Equal<Current<GVArGuiInvoiceFilter.registrationCD>>>>();
			return query.Select();
		}


		/*public PXAction<GVArGuiInvoice> AddArInvoice;
		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = "ADDARINVOICE")]
		public virtual IEnumerable addArInvoice(PXAdapter adapter)
		{
			return adapter.Get();
		}*/
		public PXAction<GVArGuiInvoiceFilter> ViewGVArGuiInvoice;
		[PXButton]
		protected virtual void viewGVArGuiInvoice()
		{
			GVArGuiInvoice row = gvArGuiInvoices.Current;
			// Creating the instance of the graph
			GVArInvoiceEntry graph = PXGraph.CreateInstance<GVArInvoiceEntry>();
			// Setting the current product for the graph
			graph.gvArGuiInvoices.Current = graph.gvArGuiInvoices.Search<GVArGuiInvoice.guiInvoiceNbr>(
										 row.GuiInvoiceNbr);
			// If the product is found by its ID, throw an exception to open
			// a new window (tab) in the browser
			if (graph.gvArGuiInvoices.Current != null)
			{
				throw new PXRedirectRequiredException(graph, true, "ArGuiInvocie Details");
			}
		}
		
	}
	[Serializable]
	public class GVArGuiInvoiceFilter : IBqlTable
	{		
        #region GuiInvoiceNbrFrom
        public abstract class guiInvoiceNbrFrom : PX.Data.IBqlField
        {
        }
        protected string _GuiInvoiceNbrFrom;
        [PXString(10)]    
        [PXUIField(DisplayName = "Gui Invoice Nbr From")]
        [PXSelector(
            typeof(Search<GVArGuiInvoice.guiInvoiceNbr>),
            typeof(GVArGuiInvoice.guiInvoiceNbr),
            typeof(GVArGuiInvoice.customerID),
            typeof(GVArGuiInvoice.invoiceType),
                SubstituteKey = typeof(GVArGuiInvoice.guiInvoiceNbr)
        // ,ValidateValue =false
        )]

        public virtual string GuiInvoiceNbrFrom
        {
            get { return this._GuiInvoiceNbrFrom; }
            set { this._GuiInvoiceNbrFrom = value; }
        }
        #endregion
        #region GuiInvoiceNbrTo
        public abstract class guiInvoiceNbrTo : PX.Data.IBqlField
        {
        }
        protected string _GuiInvoiceNbrTo;
        [PXString(10)]
        [PXUIField(DisplayName = "Gui Invoice Nbr To")]
        [PXSelector(
            typeof(Search<GVArGuiInvoice.guiInvoiceNbr>),
            typeof(GVArGuiInvoice.guiInvoiceNbr),
            typeof(GVArGuiInvoice.customerID),
            typeof(GVArGuiInvoice.invoiceType),
               SubstituteKey = typeof(GVArGuiInvoice.guiInvoiceNbr)
        // ,ValidateValue =false
        )]

        public virtual string GuiInvoiceNbrTo
        {
            get { return this._GuiInvoiceNbrTo; }
            set { this._GuiInvoiceNbrTo = value; }
        }
        #endregion
        #region DateFrom
        public abstract class dateFrom : PX.Data.IBqlField
		{
		}
		protected DateTime _DateFrom;
		[PXDate]
		[PXUIField(DisplayName = "Gui Invoice Date From")]
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
		[PXUIField(DisplayName = "Gui Invoice Date To")]
		public virtual DateTime? DateTo
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
        [PXString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "RegistrationCD")]       
        [RegistrationCDAttribute()]
        public virtual string RegistrationCD
        {
            get { return this._RegistrationCD; }
            set { this._RegistrationCD = value; }
        }
        #endregion
        #region GuiType
        public abstract class guiType : PX.Data.IBqlField
		{
		}
		[PXString(1, IsFixed = true)]
		[PXUIField(DisplayName = "GuiType")]
        [PXStringList()]
        public virtual string GuiType { get; set; }
		#endregion
	}
}