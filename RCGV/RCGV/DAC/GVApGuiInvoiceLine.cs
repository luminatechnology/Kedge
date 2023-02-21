using System;
using PX.Data;
using RCGV.GV.DAC;
﻿
namespace RCGV.GV
{
	
	[System.SerializableAttribute()]
	public class GVApGuiInvoiceLine : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{ }
		[PXBool]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected { get; set; }
		#endregion
		#region GuiInvoiceID
		public abstract class guiInvoiceID : PX.Data.IBqlField
		{
		}
		protected int? _GuiInvoiceID;
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(GVApGuiInvoice.guiInvoiceID))]
		[PXUIField(DisplayName = "GuiInvoiceID")]
		[PXParent(typeof(Select<GVApGuiInvoice,
			Where<GVApGuiInvoice.guiInvoiceID,
			Equal<Current<GVApGuiInvoiceLine.guiInvoiceID>>>>))]
		public virtual int? GuiInvoiceID
		{
			get
			{
				return this._GuiInvoiceID;
			}
			set
			{
				this._GuiInvoiceID = value;
			}
		}
		#endregion
		#region GuiInvoiceLineID
		public abstract class guiInvoiceLineID : PX.Data.IBqlField
		{
		}
		protected int? _GuiInvoiceLineID;
		[PXDBIdentity]
		[PXUIField(Enabled = false)]
		public virtual int? GuiInvoiceLineID
		{
			get
			{
				return this._GuiInvoiceLineID;
			}
			set
			{
				this._GuiInvoiceLineID = value;
			}
		}
		#endregion
		#region APInvoiceNbr
		public abstract class aPInvoiceNbr : PX.Data.IBqlField
		{
		}
		protected string _APInvoiceNbr;
		[PXDBString(40, IsKey = true)]
		[PXUIField(Enabled = false)]
		public virtual string APInvoiceNbr
		{
			get
			{
				return this._APInvoiceNbr;
			}
			set
			{
				this._APInvoiceNbr = value;
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
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
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

		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false, IsReadOnly = true)]
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

		public abstract class noteID : IBqlField { }
		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion
		#region tstamp

		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
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
	}
}
