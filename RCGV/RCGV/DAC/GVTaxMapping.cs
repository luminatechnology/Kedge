using System;
using PX.Data;
using PX.Objects.TX;
using RCGV.GV.Util;
using RCGV.GV.Attribute.Selector;

namespace RCGV.GV.DAC
{
	#region GVTaxType
	public static class GVTaxType
	{
		public const string DutyFreeName = "免稅";
		public const string TaxableName = "應稅";
		public const string ZeroTaxName = "零稅";
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(
					new string[] { DutyFree, Taxable, ZeroTax },
					new string[] { DutyFreeName, TaxableName, ZeroTaxName }
				) { }
		}

		public class ListSimpleAttribute : PXStringListAttribute
		{
			public ListSimpleAttribute()
				: base(
				new string[] { DutyFree, Taxable, ZeroTax },
				new string[] { DutyFreeName, TaxableName, ZeroTaxName }) { }
		}

		public const string DutyFree = "F";
		public const string Taxable = "T";
		public const string ZeroTax = "Z";

		public class dutyFree : Constant<string>
		{
			public dutyFree() : base(DutyFree) { }
		}

		public class zeroTax : Constant<string>
		{
			public zeroTax() : base(ZeroTax) { }
		}

		public class taxable : Constant<string>
		{
			public taxable() : base(Taxable) { }
		}
	}
	#endregion

	[Serializable]
	public class GVTaxMapping : IBqlTable
	{
		#region TaxMappingID
		public abstract class taxMappingID : PX.Data.IBqlField
		{
		}
		protected int? _TaxMappingID;
        [PXDBIdentity]
		public virtual int? TaxMappingID
		{
			get
			{
				return this._TaxMappingID;
			}
			set
			{
				this._TaxMappingID = value;
			}
		}
        #endregion
        #region TaxRevisionID
        public abstract class taxRevisionID : PX.Data.IBqlField
        {
        }
        protected int? _TaxRevisionID;
        [PXDBInt(IsKey = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search2<TaxRev.revisionID,
            InnerJoin<Tax, On<Tax.taxID, Equal<TaxRev.taxID>>,
            InnerJoin<TaxBucket, On<TaxBucket.bucketID, Equal<TaxRev.taxBucketID>,
                    And<TaxRev.taxVendorID, Equal<TaxBucket.vendorID>>>>>,
            Where<Tax.taxType, Equal<CSTaxType.vat>, 
                And<Current<GVTaxMapping.today>, 
                    Between<TaxRev.startDate, TaxRev.endDate>>>>),
                    typeof(TaxRev.revisionID),
                    typeof(Tax.taxID),
                    typeof(TaxBucket.name),
                    typeof(TaxRev.taxType)
            )
        ]
        public virtual int? TaxRevisionID
        {
            get
            {
                return this._TaxRevisionID;
            }
            set
            {
                this._TaxRevisionID = value;
            }
        }
        #endregion
        #region TaxID

        public abstract class taxID : PX.Data.IBqlField {}
		protected String _TaxID;

		/// <summary>
		/// The tax ID. This is the key field, which can be specified by the user.
		/// </summary>
		[PXDBString(Tax.taxID.Length, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
        #endregion
        #region GvType

        public abstract class gvType : PX.Data.IBqlField { }
        protected String _GvType;

        /// <summary>
        /// GvType
        /// </summary>
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXDefault("O", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "GvType", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
        [GVLookUpCodeExtAttribute(typeof(GVTaxMapping.gvType), GVLookUpCodeUtil.GvType)]
        public virtual String GvType
        {
            get
            {
                return this._GvType;
            }
            set
            {
                this._GvType = value;
            }
        }
        #endregion
        #region TaxType
        public abstract class taxType : PX.Data.IBqlField
		{
		}
		protected String _TaxType;
		/// <summary>
		/// The type of the tax.
		/// </summary>
		/// <value>
		/// The field can have the following values:
		/// <c>"F"</c>: 零稅
		/// <c>"T"</c>: 應稅
		/// <c>"Z"</c>: 免稅.
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(GVTaxType.Taxable)]
		[GVTaxType.List()]
		[PXUIField(DisplayName = "Tax Type", Required = true, Visibility = PXUIVisibility.Visible)]
		public virtual String TaxType
		{
			get
			{
				return this._TaxType;
			}
			set
			{
				this._TaxType = value;
			}
		}
		#endregion
		#region TaxRate
        protected decimal? _TaxRate;
        [PXDBDecimal(3)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Tax Rate", Required = true)]
        public virtual decimal? TaxRate
        {
            get
            {
                return this._TaxRate;
            }
            set
            {
                this._TaxRate = value;
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
		//[PXDBDate()]
		//[PXUIField(DisplayName = "Created Date Time")]
		//public virtual DateTime? CreatedDateTime { get; set; }
		//public abstract class createdDateTime : IBqlField { }
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

        #region Today
        public abstract class today : PX.Data.IBqlField
        {
        }
        protected DateTime? _Today;
        [PXDate]
        //DateTime.Today,AccessInfo.businessDate
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? Today
        {
            get
            {
                return this._Today;
            }
            set
            {
                this._Today = value;
            }
        }
        #endregion
    }
}