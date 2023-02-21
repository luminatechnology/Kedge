﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	public class GVArGuiInvoiceLine : PX.Data.IBqlTable
	{
		#region GuiInvoiceLineID
		public abstract class guiInvoiceLineID : PX.Data.IBqlField
		{
		}
		protected int? _GuiInvoiceLineID;
		[PXDBIdentity(IsKey = true)]
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
		#region GuiInvoiceID
		public abstract class guiInvoiceID : PX.Data.IBqlField
		{
		}
		protected int? _GuiInvoiceID;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "GuiInvoiceID")]
		[PXDBDefault(typeof(GVArGuiInvoice.guiInvoiceID))]
		[PXParent(typeof(Select<GVArGuiInvoice,
								Where<GVArGuiInvoice.guiInvoiceID,
								Equal<Current<GVArGuiInvoiceLine.guiInvoiceID>>>>))]
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
		#region ArInvoiceNbr
		public abstract class arInvoiceNbr : PX.Data.IBqlField
		{
		}
		protected string _ArInvoiceNbr;
		[PXDBString(40, IsUnicode = true)]
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
        #region SalesAmt
        public abstract class salesAmt : PX.Data.IBqlField
        {
        }
        protected decimal? _SalesAmt;
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "SalesAmt", Enabled = false)]
        public virtual decimal? SalesAmt
        {
            get
            {
                return this._SalesAmt;
            }
            set
            {
                this._SalesAmt = value;
            }
        }
        #endregion
        #region TaxAmt
        public abstract class taxAmt : PX.Data.IBqlField
        {
        }
        protected decimal? _TaxAmt;
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "TaxAmt", Enabled = false)]
        public virtual decimal? TaxAmt
        {
            get
            {
                return this._TaxAmt;
            }
            set
            {
                this._TaxAmt = value;
            }
        }
        #endregion
        #region TotalAmt
        public abstract class totalAmt : PX.Data.IBqlField
        {
        }
        protected decimal? _TotalAmt;
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "TotalAmt", Enabled = false)]
        public virtual decimal? TotalAmt
        {
            get
            {
                return this._TotalAmt;
            }
            set
            {
                this._TotalAmt = value;
            }
        }
        #endregion
        #region Remark
        public abstract class remark : PX.Data.IBqlField
		{
		}
		protected string _Remark;
		[PXDBString(240, IsUnicode = true)]
		[PXUIField(DisplayName = "Remark")]
		public virtual string Remark
		{
			get
			{
				return this._Remark;
			}
			set
			{
				this._Remark = value;
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
		[PXUIField(DisplayName = "Last Modified Date Time")]
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
		[PXUIField(DisplayName = "Noteid")]
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
	}
}
