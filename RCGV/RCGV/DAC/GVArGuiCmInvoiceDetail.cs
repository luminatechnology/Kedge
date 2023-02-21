﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	using PX.Objects.AR;
	
	[System.SerializableAttribute()]
	public class GVArGuiCmInvoiceDetail : PX.Data.IBqlTable
	{
		#region GuiCmInvoiceID
		public abstract class guiCmInvoiceID : PX.Data.IBqlField
		{
		}
		protected int? _GuiCmInvoiceID;
		[PXDBInt()]
		[PXDBDefault(typeof(GVArGuiCmInvoice.guiCmInvoiceID))]
		[PXParent(typeof(Select<GVArGuiCmInvoice,
								Where<GVArGuiCmInvoice.guiCmInvoiceID,
								Equal<Current<GVArGuiCmInvoiceDetail.guiCmInvoiceID>>>>))]
		[PXUIField(DisplayName = "GuiCmInvoiceID")]
		public virtual int? GuiCmInvoiceID
		{
			get
			{
				return this._GuiCmInvoiceID;
			}
			set
			{
				this._GuiCmInvoiceID = value;
			}
		}
		#endregion
		#region GuiCmInvoiceDetailID
		public abstract class guiCmInvoiceDetailID : PX.Data.IBqlField
		{
		}
		protected int? _GuiCmInvoiceDetailID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(Enabled = false)]
		public virtual int? GuiCmInvoiceDetailID
		{
			get
			{
				return this._GuiCmInvoiceDetailID;
			}
			set
			{
				this._GuiCmInvoiceDetailID = value;
			}
		}
		#endregion
		#region ArInvoiceNbr
		public abstract class arInvoiceNbr : PX.Data.IBqlField
		{
		}
		protected string _ArInvoiceNbr;
		[PXDBString(40, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "ArInvoiceNbr")]
		public virtual string ArInvoiceNbr
		{
			get
			{
				return this._ArInvoiceNbr;
			}
			set
			{
				this._ArInvoiceNbr = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected string _DocType;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Doc Type")]
		public virtual string DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region ArGuiInvoiceNbr
		public abstract class arGuiInvoiceNbr : PX.Data.IBqlField
		{
		}
		protected string _ArGuiInvoiceNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXSelector(
			typeof(Search<GVArGuiInvoice.guiInvoiceNbr>),
			typeof(GVArGuiInvoice.guiInvoiceNbr),
			typeof(GVArGuiInvoice.customerID),
			typeof(GVArGuiInvoice.invoiceType),
			typeof(GVArGuiInvoice.salesAmt),
			typeof(GVArGuiInvoice.taxAmt),
				SubstituteKey = typeof(GVArGuiInvoice.guiInvoiceNbr)

		)]
		[PXUIField(DisplayName = "ArGuiInvoiceNbr",Enabled = false)]
		public virtual string ArGuiInvoiceNbr
		{
			get
			{
				return this._ArGuiInvoiceNbr;
			}
			set
			{
				this._ArGuiInvoiceNbr = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected string _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected string _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote]
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
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected byte[] _tstamp;
		[PXDBTimestamp()]
		[PXUIField(DisplayName = "ol")]
		public virtual byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion

		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected int? _CustomerID;
		[PXInt()]
		[PXDBScalar(
			typeof(Search<ARInvoice.customerID,
			Where<ARInvoice.refNbr, Equal<GVArGuiCmInvoiceDetail.arInvoiceNbr>>>))]
		[PXUIField(DisplayName = "Customer ID", Enabled = false)]
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


        #region CuryVatTaxableTotal
        public abstract class curyVatTaxableTotal : PX.Data.IBqlField
		{
		}
		[PXDecimal(2)]
		[PXUIField(DisplayName = "Sales Amt", Enabled = false)]
        
		[PXDBScalar(
			typeof(Search<ARInvoice.curyVatTaxableTotal,
			Where<ARInvoice.refNbr, Equal<GVArGuiCmInvoiceDetail.arInvoiceNbr>>>))]
		public virtual decimal? CuryVatTaxableTotal { get; set; }
		#endregion

		#region CuryTaxTotal
		public abstract class curyTaxTotal : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryTaxTotal;
		[PXDecimal(2)]
		[PXUIField(DisplayName = "Tax Amt", Enabled = false)]
		[PXDBScalar(
			typeof(Search<ARInvoice.curyTaxTotal,
			Where<ARInvoice.refNbr, Equal<GVArGuiCmInvoiceDetail.arInvoiceNbr>>>))]
		public virtual decimal? CuryTaxTotal
		{
			get
			{
				return this._CuryTaxTotal;
			}
			set
			{
				this._CuryTaxTotal = value;
			}
		}
		#endregion
	}
}
