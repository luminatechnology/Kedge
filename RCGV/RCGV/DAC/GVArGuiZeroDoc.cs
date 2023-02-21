﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	using RCGV.GV.Util;
	using PX.Objects.AR;
	using RCGV.GV;
	using RCGV.GV.Descriptor;
    using RCGV.GV.Attribute.Selector;

    [System.SerializableAttribute()]
    public class GVArGuiZeroDoc : PX.Data.IBqlTable
    {

        #region ZeroDocID
        public abstract class zeroDocID : PX.Data.IBqlField
        {
        }
        protected int? _ZeroDocID;
        [PXDBIdentity(IsKey = true)]
        [PXUIField(Enabled = false)]
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
        #region ZeroDocCD
        public abstract class zeroDocCD : PX.Data.IBqlField
        {
        }
        protected string _ZeroDocCD;
        [PXDBString(13, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "ZeroDocCD")]
        [PXSelector(
            typeof(Search<GVArGuiZeroDoc.zeroDocCD>),
            typeof(GVArGuiZeroDoc.zeroDocCD),
            typeof(GVArGuiZeroDoc.customerID),
            typeof(GVArGuiZeroDoc.registrationCD),
                SubstituteKey = typeof(GVArGuiZeroDoc.zeroDocCD)

        )]

        [AutoNumber(typeof(GVSetup.autoNumbering), typeof(GVSetup.zeroDocCDNbr))]
        public virtual string ZeroDocCD
        {
            get
            {
                return this._ZeroDocCD;
            }
            set
            {
                this._ZeroDocCD = value;
            }
        }
        #endregion
        #region RegistrationCD
        public abstract class registrationCD : PX.Data.IBqlField
        {
        }
        protected string _RegistrationCD;
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "RegistrationCD", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [RegistrationCDAttribute()]
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
        #region DocNbr
        public abstract class docNbr : PX.Data.IBqlField
        {
        }
        protected string _DocNbr;
        [PXDBString(30, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "DocNbr", Required = true)]
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
        #region DocDate
        public abstract class docDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _DocDate;
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "DocDate")]
        public virtual DateTime? DocDate
        {
            get
            {
                return this._DocDate;
            }
            set
            {
                this._DocDate = value;
            }
        }
        #endregion
        #region DeclareYear
        public abstract class declareYear : PX.Data.IBqlField
        {
        }
        protected int? _DeclareYear;
        [PXDBInt()]
        [PXUIField(DisplayName = "DeclareYear", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? DeclareYear
        {
            get
            {
                return this._DeclareYear;
            }
            set
            {
                this._DeclareYear = value;
            }
        }
        #endregion
        #region DeclareMonth
        public abstract class declareMonth : PX.Data.IBqlField
        {
        }
        protected int? _DeclareMonth;
        [PXDBInt()]
        [PXUIField(DisplayName = "DeclareMonth", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? DeclareMonth
        {
            get
            {
                return this._DeclareMonth;
            }
            set
            {
                this._DeclareMonth = value;
            }
        }
        #endregion
        #region DocType
        public abstract class docType : PX.Data.IBqlField
        {
        }
        protected string _DocType;
        [PXDBString(2, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "DocType", Required = true)]
        [GVLookUpCodeExtAttribute(typeof(GVArGuiZeroDoc.docType), GVLookUpCodeUtil.ZeroDocType)]

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
        #region DocTypeCode
        public abstract class docTypeCode : PX.Data.IBqlField
        {
        }
        protected string _DocTypeCode;
        [PXString()]
        [PXFormula(typeof(GVArGuiZeroDoc.docType))]
        [PXUIField(DisplayName = "DocTypeCode")]
        public virtual string DocTypeCode
        {
            get
            {
                return this._DocTypeCode;
            }
            set
            {
                this._DocTypeCode = value;
            }
        }
        #endregion
        #region Remark
        public abstract class remark : PX.Data.IBqlField
        {
        }
        protected string _Remark;
        [PXDBString(240, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Remark", Required = true)]
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
        #region SalesType
        public abstract class salesType : PX.Data.IBqlField
		 {
		 }
		protected string _SalesType;
		[PXDBString(1, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "SalesType", Required = true)]
        [GVLookUpCodeExtAttribute(typeof(GVArGuiZeroDoc.salesType), GVLookUpCodeUtil.ZeroSalesType)]
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
        #region SalesTypeCode
        public abstract class salesTypeCode : PX.Data.IBqlField
        {
        }
        protected string _SalesTypeCode;
        [PXString()]
        [PXUIField(DisplayName = "SalesTypeCode")]
        [PXFormula(typeof(GVArGuiZeroDoc.salesType))]
        public virtual string SalesTypeCode
        {
            get
            {
                return this._SalesTypeCode;
            }
            set
            {
                this._SalesTypeCode = value;
            }
        }
        #endregion
        


        #region DataType
        public abstract class dataType : PX.Data.IBqlField
		{
		}
		protected string _DataType;
		[PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "DataType", Required = true)]
        [GVLookUpCodeExtAttribute(typeof(GVArGuiZeroDoc.dataType), GVLookUpCodeUtil.ZeroDataType)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
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
		#region CustomerID
		 public abstract class customerID : PX.Data.IBqlField
		 {
		 }
		 protected int? _CustomerID;
		//[PXDBInt()]
		 [PXUIField(DisplayName = "CustomerID")]
		 [CustomerActive(
			 Visibility = PXUIVisibility.SelectorVisible,
			 DescriptionField = typeof(Customer.acctName),
			 Filterable = true, TabOrder = 2)]
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
	    #region CustUniformNumber
		 public abstract class custUniformNumber : PX.Data.IBqlField
		 {
		 }
		 protected string _CustUniformNumber;
		 [PXDBString(9, IsUnicode = true)]
		 [PXDefault("")]
		 [PXUIField(DisplayName = "CustUniformNumber",Enabled =false)]
		 public virtual string CustUniformNumber
		 {
			 get
			 {
				 return this._CustUniformNumber;
			 }
			 set
			 {
				 this._CustUniformNumber = value;
			 }
		 }
		 #endregion
		#region SalesAmt
		 public abstract class salesAmt : PX.Data.IBqlField
		 {
		 }
		 protected decimal? _SalesAmt;
		 [PXDBDecimal(2)]
		 [PXDefault(TypeCode.Decimal, "0.0")]
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
		#region OriCurrency
		 public abstract class oriCurrency : PX.Data.IBqlField
		 {
		 }
		 protected string _OriCurrency;
		 [PXDBString(3, IsUnicode = true)]
		 [PXDefault("")]
		 [PXUIField(DisplayName = "OriCurrency")]
		 public virtual string OriCurrency
		 {
			 get
			 {
				 return this._OriCurrency;
			 }
			 set
			 {
				 this._OriCurrency = value;
			 }
		 }
		 #endregion
		#region OriSalesAmt
		 public abstract class oriSalesAmt : PX.Data.IBqlField
		 {
		 }
		 protected decimal? _OriSalesAmt;
		 [PXDBDecimal(2)]
		 [PXDefault(TypeCode.Decimal, "0.0")]
		 [PXUIField(DisplayName = "OriSalesAmt")]
		 public virtual decimal? OriSalesAmt
		 {
			 get
			 {
				 return this._OriSalesAmt;
			 }
			 set
			 {
				 this._OriSalesAmt = value;
			 }
		 }
		 #endregion
		#region Quantity
		 public abstract class quantity : PX.Data.IBqlField
		 {
		 }
		 protected decimal? _Quantity;
		 [PXDBDecimal(2)]
		 [PXDefault(TypeCode.Decimal, "0.0")]
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
		#region Status
		 public abstract class status : PX.Data.IBqlField
		 {
		 }
		 protected string _Status;
		 [PXDBString(1, IsUnicode = true)]
		 [PXUIField(DisplayName = "Status",Enabled =false)]
         [GVLookUpCodeExtAttribute(typeof(GVArGuiInvoice.status), GVLookUpCodeUtil.Status)]
        public virtual string Status
		 {
			 get
			 {
				 return this._Status;
			 }
			 set
			 {
				 this._Status = value;
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
	 }
}
