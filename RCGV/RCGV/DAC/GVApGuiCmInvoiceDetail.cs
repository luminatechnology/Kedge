using System;
using PX.Data;
﻿
namespace RCGV.GV.DAC
{

	[System.SerializableAttribute()]
	public class GVApGuiCmInvoiceDetail : PX.Data.IBqlTable
	{
		#region GuiCmInvoiceID
		public abstract class guiCmInvoiceID : PX.Data.IBqlField
		{
		}
		protected int? _GuiCmInvoiceID;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(GVApGuiCmInvoice.guiCmInvoiceID))]
		[PXUIField(DisplayName = "GuiCmInvoice ID")]
		[PXParent(typeof(Select<GVApGuiCmInvoice,
					Where<GVApGuiCmInvoice.guiCmInvoiceID,
					Equal<Current<GVApGuiCmInvoiceDetail.guiCmInvoiceID>>>>))]
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
		#region GuiCmInvoiceLineID
		public abstract class guiCmInvoiceLineID : PX.Data.IBqlField
		{
		}
		protected int? _GuiCmInvoiceLineID;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(GVApGuiCmInvoiceLine.guiCmInvoiceLineID))]
		[PXUIField(DisplayName = "GuiCmInvoiceLine ID")]
		[PXParent(typeof(Select<GVApGuiCmInvoiceLine,
					Where<GVApGuiCmInvoiceLine.guiCmInvoiceLineID,
					Equal<Current<GVApGuiCmInvoiceDetail.guiCmInvoiceLineID>>>>))]

		public virtual int? GuiCmInvoiceLineID
		{
			get
			{
				return this._GuiCmInvoiceLineID;
			}
			set
			{
				this._GuiCmInvoiceLineID = value;
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected string _RefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "RefNbr")]
		public virtual string RefNbr
        {
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
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
