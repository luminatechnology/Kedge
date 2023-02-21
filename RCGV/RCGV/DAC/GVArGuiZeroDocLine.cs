﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	public class GVArGuiZeroDocLine : PX.Data.IBqlTable
	{
		#region ZeroDocLineID
		public abstract class zeroDocLineID : PX.Data.IBqlField
		{
		}
		protected int? _ZeroDocLineID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(Enabled = false)]
		public virtual int? ZeroDocLineID
		{
			get
			{
				return this._ZeroDocLineID;
			}
			set
			{
				this._ZeroDocLineID = value;
			}
		}
		#endregion
		#region ZeroDocID
		public abstract class zeroDocID : PX.Data.IBqlField
		{
		}
		protected int? _ZeroDocID;
		[PXDBInt()]
		[PXUIField(DisplayName = "ZeroDocID")]
        [PXDBDefault(typeof(GVArGuiZeroDoc.zeroDocID))]
        [PXParent(typeof(Select<GVArGuiZeroDoc,
                            Where<GVArGuiZeroDoc.zeroDocID,
                            Equal<Current<GVArGuiZeroDocLine.zeroDocID>>>>))]
        public virtual int? ZeroDocID
		{
			get
			{
				return this._ZeroDocID;
			}
			set
			{
				this._ZeroDocID = value;
			}
		}
		#endregion
		#region GuiInvoiceNbr
		public abstract class guiInvoiceNbr : PX.Data.IBqlField
		{
		}
		protected string _GuiInvoiceNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "GuiInvoiceNbr")]
		[PXSelector(
			typeof(Search<GVArGuiInvoice.guiInvoiceNbr,
			Where<GVArGuiInvoice.taxCode, Equal<LookUpCode.lookUpCode2>,
            And<Where<GVArGuiInvoice.customerID,Equal<Current<GVArGuiZeroDoc.customerID>>,
            And<Where<GVArGuiInvoice.registrationCD,Equal<Current<GVArGuiZeroDoc.registrationCD>>>>>>>>),
			typeof(GVArGuiInvoice.guiInvoiceNbr),
			typeof(GVArGuiInvoice.customerID))]
		public virtual string GuiInvoiceNbr
		{
			get
			{
				return this._GuiInvoiceNbr;
			}
			set
			{
				this._GuiInvoiceNbr = value;
			}
		}
		#endregion
		#region InvoiceDate
		public abstract class invoiceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _InvoiceDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "InvoiceDate", Enabled = false)]
		public virtual DateTime? InvoiceDate
		{
			get
			{
				return this._InvoiceDate;
			}
			set
			{
				this._InvoiceDate = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected int? _CustomerID;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "CustomerID")]
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
		#region SalesAmt
		public abstract class salesAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _SalesAmt;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "SalesAmt",Enabled=false)]
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
		#region Customer
		public abstract class customer : PX.Data.IBqlField
		{
		}
		protected string _Customer;
		[PXDBString(240, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Customer", Enabled = false)]
		public virtual string Customer
		{
			get
			{
				return this._Customer;
			}
			set
			{
				this._Customer = value;
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

		public static class LookUpCode
		{
			public const string LookUpCode2 = "2";
			public class lookUpCode2 : Constant<string>
			{
				public lookUpCode2()
					: base(LookUpCode2)
				{
				}
			}

		}
	}
}
