﻿namespace RCGV.GV
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	public class GVArApCmInvoice : PX.Data.IBqlTable
	{
		#region GuiCmInvoiceID
		public abstract class guiCmInvoiceID : PX.Data.IBqlField
		{
		}
		protected int? _GuiCmInvoiceID;
		[PXDBInt(IsKey =true)]
		[PXDefault(0)]
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
		#region GuiCmInvoiceLineID
		public abstract class guiCmInvoiceLineID : PX.Data.IBqlField
		{
		}
		protected int? _GuiCmInvoiceLineID;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "GuiCmInvoiceLineID")]
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
		#region InvoiceDate
		public abstract class invoiceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _InvoiceDate;
		[PXDBDate()]
		[PXDefault(TypeCode.DateTime, "01/01/1900")]
		[PXUIField(DisplayName = "InvoiceDate")]
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
		#region BuyerName
		public abstract class buyerName : PX.Data.IBqlField
		{
		}
		protected string _BuyerName;
		[PXDBString(240, IsUnicode = true)]
		[PXUIField(DisplayName = "BuyerName")]
		public virtual string BuyerName
		{
			get
			{
				return this._BuyerName;
			}
			set
			{
				this._BuyerName = value;
			}
		}
		#endregion
		#region BuyerNum
		public abstract class buyerNum : PX.Data.IBqlField
		{
		}
		protected string _BuyerNum;
		[PXDBString(9, IsUnicode = true)]
		[PXUIField(DisplayName = "BuyerNum")]
		public virtual string BuyerNum
		{
			get
			{
				return this._BuyerNum;
			}
			set
			{
				this._BuyerNum = value;
			}
		}
		#endregion
		#region GuiDate
		public abstract class guiDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _GuiDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "GuiDate")]
		public virtual DateTime? GuiDate
		{
			get
			{
				return this._GuiDate;
			}
			set
			{
				this._GuiDate = value;
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
            typeof(Search4<GVArApCmInvoice.guiInvoiceNbr, Aggregate<GroupBy<GVArApCmInvoice.guiInvoiceNbr>>>),
            typeof(GVArApCmInvoice.guiInvoiceNbr),
            typeof(GVArApCmInvoice.sellerName),
            typeof(GVArApCmInvoice.sellerNum),
            typeof(GVArApCmInvoice.invoiceDate)
            )]
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
        #region GuiType
        public abstract class guiType : PX.Data.IBqlField
        {
        }
        protected string _GuiType;
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "Gui Type", Required = true)]
        public virtual string GuiType
        {
            get
            {
                return this._GuiType;
            }
            set
            {
                this._GuiType = value;
            }
        }
        #endregion
        #region GuiCmInvoiceNbr
        public abstract class guiCmInvoiceNbr : PX.Data.IBqlField
        {
        }
        protected string _GuiCmInvoiceNbr;
        [PXSelector(
            //typeof(Search<GVArApCmInvoice.guiCmInvoiceNbr, Where<Zero ,Equal<Zero>>   >),
            typeof(Search4<GVArApCmInvoice.guiCmInvoiceNbr, Aggregate<GroupBy<GVArApCmInvoice.guiCmInvoiceNbr>>>),
            
            typeof(GVArApCmInvoice.guiCmInvoiceNbr),
            typeof(GVArApCmInvoice.sellerName),
            typeof(GVArApCmInvoice.sellerNum),
            typeof(GVArApCmInvoice.invoiceDate)
            )]
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "GuiCmInvoiceNbr")]
        public virtual string GuiCmInvoiceNbr
        {
            get
            {
                return this._GuiCmInvoiceNbr;
            }
            set
            {
                this._GuiCmInvoiceNbr = value;
            }
        }
        #endregion

        #region SellerName
        public abstract class sellerName : PX.Data.IBqlField
		{
		}
		protected string _SellerName;
		[PXDBString(240, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "SellerName")]
		public virtual string SellerName
		{
			get
			{
				return this._SellerName;
			}
			set
			{
				this._SellerName = value;
			}
		}
		#endregion
		#region SellerNum
		public abstract class sellerNum : PX.Data.IBqlField
		{
		}
		protected string _SellerNum;
		[PXDBString(9, IsUnicode = true)]
		[PXUIField(DisplayName = "SellerNum")]
		public virtual string SellerNum
		{
			get
			{
				return this._SellerNum;
			}
			set
			{
				this._SellerNum = value;
			}
		}
        #endregion
        #region SellTel
        public abstract class sellTel : PX.Data.IBqlField
        {
        }
        protected string _SellTel;
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "SellerNum")]
        public virtual string SellTel
        {
            get
            {
                return this._SellTel;
            }
            set
            {
                this._SellTel = value;
            }
        }
        #endregion
        #region SellAddress
        public abstract class sellAddress : PX.Data.IBqlField
        {
        }
        protected string _SellAddress;
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "SellerNum")]
        public virtual string SellAddress
        {
            get
            {
                return this._SellAddress;
            }
            set
            {
                this._SellAddress = value;
            }
        }
        #endregion
        #region Addr
        public abstract class addr : PX.Data.IBqlField
        {
        }
        protected string _Addr;
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "SellerNum")]
        public virtual string Addr
        {
            get
            {
                return this._Addr;
            }
            set
            {
                this._Addr = value;
            }
        }
        #endregion
        

        #region ItemDesc
        public abstract class itemDesc : PX.Data.IBqlField
		{
		}
		protected string _ItemDesc;
		[PXDBString(240, IsUnicode = true)]
		[PXUIField(DisplayName = "ItemDesc")]
		public virtual string ItemDesc
		{
			get
			{
				return this._ItemDesc;
			}
			set
			{
				this._ItemDesc = value;
			}
		}
		#endregion
		#region Unit
		public abstract class unit : PX.Data.IBqlField
		{
		}
		protected string _Unit;
		[PXDBString(6, IsUnicode = true)]
		[PXUIField(DisplayName = "Unit")]
		public virtual string Unit
		{
			get
			{
				return this._Unit;
			}
			set
			{
				this._Unit = value;
			}
		}
		#endregion
		#region UnitPrice
		public abstract class unitPrice : PX.Data.IBqlField
		{
		}
		protected decimal? _UnitPrice;
		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "UnitPrice")]
		public virtual decimal? UnitPrice
		{
			get
			{
				return this._UnitPrice;
			}
			set
			{
				this._UnitPrice = value;
			}
		}
        #endregion

        #region  Quantity
        public abstract class quantity : PX.Data.IBqlField
        {
        }
        protected decimal? _Quantity;
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Quantity")]
        public virtual decimal? Quantity
        {
            get
            {
                return this._Quantity;
            }
            set
            {
                this._Quantity = value;
            }
        }
        #endregion
        #region SalesAmt
        public abstract class salesAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _SalesAmt;
		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "SalesAmt")]
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
        #region SalesAmtTotal
        public abstract class salesAmtTotal : PX.Data.IBqlField
        {
        }
        protected decimal? _SalesAmtTotal;
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "SalesAmtTotal")]
        public virtual decimal? SalesAmtTotal
        {
            get
            {
                return this._SalesAmtTotal;
            }
            set
            {
                this._SalesAmtTotal = value;
            }
        }
        #endregion
        #region TaxCode
        public abstract class taxCode : PX.Data.IBqlField
		{
		}
		protected string _TaxCode;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "TaxCode")]
		public virtual string TaxCode
		{
			get
			{
				return this._TaxCode;
			}
			set
			{
				this._TaxCode = value;
			}
		}
        #endregion
        #region TaxCodeDesc
        public abstract class taxCodeDesc : PX.Data.IBqlField
        {
        }
        protected string _TaxCodeDesc;
        [PXDBString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "TaxCode")]
        public virtual string TaxCodeDesc
        {
            get
            {
                return this._TaxCodeDesc;
            }
            set
            {
                this._TaxCodeDesc = value;
            }
        }
        #endregion
        #region TaxAmt
        public abstract class taxAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _TaxAmt;
		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "TaxAmt")]
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
        #region TaxAmtTotal
        public abstract class taxAmtTotal : PX.Data.IBqlField
        {
        }
        protected decimal? _TaxAmtTotal;
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "TaxAmtTotal")]
        public virtual decimal? TaxAmtTotal
        {
            get
            {
                return this._TaxAmtTotal;
            }
            set
            {
                this._TaxAmtTotal = value;
            }
        }
        #endregion
        #region CMType
        public abstract class cMType : PX.Data.IBqlField
		{
		}
		protected string _CMType;
		[PXDBString(2, IsKey = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "CMType")]
		public virtual string CMType
		{
			get
			{
				return this._CMType;
			}
			set
			{
				this._CMType = value;
			}
		}
		#endregion
	}
}
