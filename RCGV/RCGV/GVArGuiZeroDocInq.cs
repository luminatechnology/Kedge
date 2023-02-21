using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using RCGV.GV.Util;


namespace RCGV.GV
{
	public class GVArGuiZeroDocInq : PXGraph<GVArGuiZeroDocInq>
	{
		public PXFilter<GVArGuiZeroDocFilter> gVArGuiZeroDocFilters;


		[PXFilterable]
		public PXSelectReadonly<GVArGuiZeroDoc> gvArGuiZeroDocs;
		protected virtual IEnumerable GVArGuiZeroDocs()
		{
			// Creating a dynamic query
			// Data is ordered by the productID (and supplierID)
			PXSelectBase<GVArGuiZeroDoc> query =
			new PXSelectOrderBy<GVArGuiZeroDoc,
			OrderBy<Asc<GVArGuiZeroDoc.zeroDocCD>>>(this);
			// Adding filtering conditions to the query
			GVArGuiZeroDocFilter filter = gVArGuiZeroDocFilters.Current;
			if (filter.DocDateFrom != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.docDate,
					GreaterEqual<Current<GVArGuiZeroDocFilter.docdateFrom>>>>();
			if (filter.DocDateTo != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.docDate,
					LessEqual<Current<GVArGuiZeroDocFilter.docdateTo>>>>();
			if (filter.ZeroDocCDFrom != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.zeroDocCD,
					GreaterEqual<Current<GVArGuiZeroDocFilter.zeroDocCDFrom>>>>();
			if (filter.ZeroDocCDTo != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.zeroDocCD,
					LessEqual<Current<GVArGuiZeroDocFilter.zeroDocCDTo>>>>();
			if (filter.DocType != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.docType,
					Equal<Current<GVArGuiZeroDocFilter.docType>>>>();
			if (filter.SalesType != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.salesType,
					Equal<Current<GVArGuiZeroDocFilter.salesType>>>>();
			if (filter.DataType != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.dataType,
					Equal<Current<GVArGuiZeroDocFilter.dataType>>>>();
			if (filter.DocNbr != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.docNbr,
					Equal<Current<GVArGuiZeroDocFilter.docNbr>>>>();
			if (filter.RegistrationCD != null)
				query.WhereAnd<Where<GVArGuiZeroDoc.registrationCD,
					Equal<Current<GVArGuiZeroDocFilter.registrationCD>>>>();
			return query.Select();
		}


		public PXAction<GVArGuiZeroDocFilter> ViewGVArGuiZeroDoc;
		[PXButton]
		protected virtual void viewGVArGuiZeroDoc()
		{
			GVArGuiZeroDoc row = gvArGuiZeroDocs.Current;
			// Creating the instance of the graph
			GVArGuiZeroDocEntry graph = PXGraph.CreateInstance<GVArGuiZeroDocEntry>();
			// Setting the current product for the graph
			graph.gvArGuiZeroDocs.Current = graph.gvArGuiZeroDocs.Search<GVArGuiZeroDoc.zeroDocCD>(
			row.ZeroDocCD);
			// If the product is found by its ID, throw an exception to open
			// a new window (tab) in the browser
			if (graph.gvArGuiZeroDocs.Current != null)
			{
				throw new PXRedirectRequiredException(graph, true, "ZeroDoc Details");
			}
		}

	}
	[Serializable]
	public class GVArGuiZeroDocFilter : IBqlTable
	{
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
		#region ZeroDocCDFrom
		public abstract class zeroDocCDFrom : PX.Data.IBqlField
		{
		}
		[PXString(13)]
		[PXUIField(DisplayName = "ZeroDocCDFrom")]
        [PXSelector(
            typeof(Search<GVArGuiZeroDoc.zeroDocCD>),
            typeof(GVArGuiZeroDoc.registrationCD),
            typeof(GVArGuiZeroDoc.customerID))]
        public virtual string ZeroDocCDFrom { get; set; }
		#endregion
		#region ZeroDocCDTo
		public abstract class zeroDocCDTo : PX.Data.IBqlField
		{
		}
		[PXString(13)]
		[PXUIField(DisplayName = "ZeroDocCDTo")]
        [PXSelector(
            typeof(Search<GVArGuiZeroDoc.zeroDocCD>),
            typeof(GVArGuiZeroDoc.registrationCD),
            typeof(GVArGuiZeroDoc.customerID))]
        public virtual string ZeroDocCDTo { get; set; }
		#endregion
		#region DocDateFrom
		public abstract class docdateFrom : PX.Data.IBqlField
		{
		}
		protected DateTime _DocDateFrom;
		[PXDate]
		[PXUIField(DisplayName = "DocDateFrom")]
		public virtual DateTime? DocDateFrom
		{
			get;
			set;
		}
		#endregion
		#region DocDateTo
		public abstract class docdateTo : PX.Data.IBqlField
		{
		}
		protected DateTime _DocDateTo;
		[PXDate]
		[PXUIField(DisplayName = "DocDateTo")]
		public virtual DateTime? DocDateTo
		{
			get;
			set;
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		[PXString(2, IsFixed = true)]
		[PXUIField(DisplayName = "DocType")]
		//[GVLookUpCodeAttribute(GVLookUpCodeUtil.ZeroDocType)]
		[PXStringList(
			 new string[]
			{
				"G3","F5","F4",
				"D5","B2","B9",
				"B8","G5","D1",
				"B1","99","98"
			}, new string[]
			{
				"外貨復出口","自由港區貨物出口","自由港區與其他自由港區、課稅區對自由港區移運",
				"保稅倉貨物出口","保稅廠相互交易或售與保","產品出口",
				"保稅廠進口貨物","國貨出口","課稅區售與或退回保稅倉",
				"課稅區售與保稅廠","買匯水單","賣匯水單"
			})]
		public virtual string DocType { get; set; }
		#endregion
		#region DocNbr
		public abstract class docNbr : PX.Data.IBqlField
		{
		}
		protected string _DocNbr;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "DocNbr")]
		[PXSelector(
			typeof(Search<GVArGuiZeroDoc.docNbr>),
			typeof(GVArGuiZeroDoc.docNbr))]
		public virtual string DocNbr
		{
			get
			{
				return this._DocNbr;
			}
			set
			{
				this._DocNbr = value;
			}
		}
		#endregion
		#region SalesType
		public abstract class salesType : PX.Data.IBqlField
		{
		}
		protected string _SalesType;
		[PXUIField(DisplayName = "SalesType")]
		//[GVLookUpCodeAttribute(GVLookUpCodeUtil.ZeroSalesType)]
		[PXStringList(new string[]
			{
				"1",
				"2"
			}, new string[] 
			{
				"外銷貨物",
				"與外銷有關之勞務,或在國內提供國外使用之勞務"
			}
			)]
		public virtual string SalesType
		{
			get
			{
				return this._SalesType;
			}
			set
			{
				this._SalesType = value;
			}
		}
		#endregion
		#region DataType
		public abstract class dataType : PX.Data.IBqlField
		{
		}
		protected string _DataType;
		[PXUIField(DisplayName = "DataType")]
		//[GVLookUpCodeAttribute(GVLookUpCodeUtil.ZeroDataType)]
		[PXStringList(new string[]
			{
				"1",
				"2"
			}, new string[]
			{
				"非經海關出口應附證明文件",
				"經海關出口免附證明文件"
			})]
		public virtual string DataType
		{
			get
			{
				return this._DataType;
			}
			set
			{
				this._DataType = value;
			}
		}
		#endregion
	}
}