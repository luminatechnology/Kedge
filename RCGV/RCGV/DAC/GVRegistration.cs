﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
    using PX.Data.BQL;
    using PX.Objects.CS;
    using RCGV.GV.Util;

	[System.SerializableAttribute()]
	public class GVRegistration : PX.Data.IBqlTable
	{
		#region RegistrationID
		public abstract class registrationID : PX.Data.IBqlField
		{
		}
		protected int? _RegistrationID;
		[PXDBIdentity()]
		public virtual int? RegistrationID
		{
			get
			{
				return this._RegistrationID;
			}
			set
			{
				this._RegistrationID = value;
			}
		}
		#endregion

		#region RegistrationCD
		public abstract class registrationCD : PX.Data.IBqlField
		{
		}
		protected string _RegistrationCD;
		[PXDBString(9, IsUnicode = true, IsKey = true, InputMask = "#########")]
		//[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "RegistrationCD", Required = true)]
		[PXSelector(
			 typeof(Search<GVRegistration.registrationCD>),
			 typeof(GVRegistration.registrationCD),
			 typeof(GVRegistration.govUniformNumber),
			 typeof(GVRegistration.siteNameChinese))]
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

		#region SiteNameChinese
		public abstract class siteNameChinese : PX.Data.IBqlField
		{
		}
		protected string _SiteNameChinese;
		[PXDBString(240, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "SiteNameChinese", Required = true)]
		public virtual string SiteNameChinese
		{
			get
			{
				return this._SiteNameChinese;
			}
			set
			{
				this._SiteNameChinese = value;
			}
		}
		#endregion

		#region SiteNameEnglish
		public abstract class siteNameEnglish : PX.Data.IBqlField
		{
		}
		protected string _SiteNameEnglish;
		[PXDBString(240, IsUnicode = true)]
		[PXUIField(DisplayName = "SiteNameEnglish")]
		public virtual string SiteNameEnglish
		{
			get
			{
				return this._SiteNameEnglish;
			}
			set
			{
				this._SiteNameEnglish = value;
			}
		}
		#endregion

		#region GovUniformNumber
		public abstract class govUniformNumber : PX.Data.IBqlField
		{
		}
		protected string _GovUniformNumber;
		[PXDBString(8, IsUnicode = true, InputMask = "########")]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "GovUniformNumber", Required = true)]
		public virtual string GovUniformNumber
		{
			get
			{
				return this._GovUniformNumber;
			}
			set
			{
				this._GovUniformNumber = value;
			}
		}
		#endregion

		#region TaxPayer
		public abstract class taxPayer : PX.Data.IBqlField
		{
		}
		protected string _TaxPayer;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "TaxPayer", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public virtual string TaxPayer
		{
			get
			{
				return this._TaxPayer;
			}
			set
			{
				this._TaxPayer = value;
			}
		}
		#endregion

		#region TaxCityCode
		public abstract class taxCityCode : PX.Data.IBqlField
		{
		}
		protected string _TaxCityCode;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "TaxCityCode", Required = true)]
		[GVLookUpCodeExtAttribute(typeof(GVRegistration.taxCityCode), GVLookUpCodeUtil.TaxCityCode)]
		public virtual string TaxCityCode
		{
			get
			{
				return this._TaxCityCode;
			}
			set
			{
				this._TaxCityCode = value;
			}
		}
		#endregion

		#region SpecialTaxType
		public abstract class specialTaxType : PX.Data.IBqlField
		{
		}
		protected string _SpecialTaxType;
		[PXDBString(1, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "SpecialTaxType", Required = true)]
		[GVList.GVSpecialTaxType]
		public virtual string SpecialTaxType
		{
			get
			{
				return this._SpecialTaxType;
			}
			set
			{
				this._SpecialTaxType = value;
			}
		}
		#endregion

		#region SiteAddress
		public abstract class siteAddress : PX.Data.IBqlField
		{
		}
		protected string _SiteAddress;
		[PXDBString(240, IsUnicode = true)]
		[PXUIField(DisplayName = "SiteAddress", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public virtual string SiteAddress
		{
			get
			{
				return this._SiteAddress;
			}
			set
			{
				this._SiteAddress = value;
			}
		}
		#endregion

		#region SiteTelephone
		public abstract class siteTelephone : PX.Data.IBqlField
		{
		}
		protected string _SiteTelephone;
		[PXDBString(20, IsUnicode = true, InputMask = "(##) ####-####")]
		[PXUIField(DisplayName = "SiteTelephone", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public virtual string SiteTelephone
		{
			get
			{
				return this._SiteTelephone;
			}
			set
			{
				this._SiteTelephone = value;
			}
		}
		#endregion

		#region TaxAuthority
		public abstract class taxAuthority : PX.Data.IBqlField
		{
		}
		protected string _TaxAuthority;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "TaxAuthority", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXSelector(typeof(Search2<SegmentValue.value,
					 InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
					And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
					Where<Segment.dimensionID, Equal<tWTAXAGENCY>,
					And<Segment.segmentID, Equal<segmentIDPart1>,
					And<SegmentValue.active, Equal<True>>>>>),
                typeof(SegmentValue.value),
                typeof(SegmentValue.descr))]
		public virtual string TaxAuthority
		{
			get
			{
				return this._TaxAuthority;
			}
			set
			{
				this._TaxAuthority = value;
			}
		}
		#endregion

		#region DeclarationMethod
		public abstract class declarationMethod : PX.Data.IBqlField
		{
		}
		protected string _DeclarationMethod;
		[PXDBString(1, IsUnicode = true)]
		[PXUIField(DisplayName = "DeclarationMethod", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[GVLookUpCodeExtAttribute(typeof(GVRegistration.declarationMethod), GVLookUpCodeUtil.DeclareMethod)]
		public virtual string DeclarationMethod
		{
			get
			{
				return this._DeclarationMethod;
			}
			set
			{
				this._DeclarationMethod = value;
			}
		}
		#endregion

		#region DeclarationPayCode
		public abstract class declarationPayCode : PX.Data.IBqlField
		{
		}
		protected string _DeclarationPayCode;
		[PXDBString(1, IsUnicode = true)]
		[PXUIField(DisplayName = "DeclarationPayCode", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[GVLookUpCodeExtAttribute(typeof(GVRegistration.declarationPayCode), GVLookUpCodeUtil.DeclarePayCode)]
		public virtual string DeclarationPayCode
		{
			get
			{
				return this._DeclarationPayCode;
			}
			set
			{
				this._DeclarationPayCode = value;
			}
		}
		#endregion

		#region ParentRegistrationCD
		public abstract class parentRegistrationCD : PX.Data.IBqlField
		{
		}
		protected string _ParentRegistrationCD;
		[PXDBString(9, IsUnicode = true, InputMask = "999999999")]
		[PXUIField(DisplayName = "ParentRegistrationCD")]

		//2020/04/09 add Selector
		[PXSelector(typeof(Search<GVRegistration.registrationCD,
			Where<GVRegistration.registrationCD,NotEqual<Current<GVRegistration.registrationCD>>>>))]
		public virtual string ParentRegistrationCD
		{
			get
			{
				return this._ParentRegistrationCD;
			}
			set
			{
				this._ParentRegistrationCD = value;
			}
		}
		#endregion

		#region QrCodeSeedString
		public abstract class qrCodeSeedString : PX.Data.IBqlField
		{
		}
		protected string _QrCodeSeedString;
		[PXDBString(240, IsUnicode = true)]
		[PXUIField(DisplayName = "QrCodeSeedString", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public virtual string QrCodeSeedString
		{
			get
			{
				return this._QrCodeSeedString;
			}
			set
			{
				this._QrCodeSeedString = value;
			}
		}
		#endregion

		//2020/04/09 刪除EffectiveDate、ExpirationDate
		/*#region EffectiveDate
		public abstract class effectiveDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EffectiveDate;
		[PXDBDate()]
		[PXDefault((typeof(AccessInfo.businessDate)), PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "EffectiveDate")]
		public virtual DateTime? EffectiveDate
		{
			get
			{
				return this._EffectiveDate;
			}
			set
			{
				this._EffectiveDate = value;
			}
		}
		#endregion
		#region ExpirationDate
		public abstract class expirationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpirationDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "ExpirationDate")]
		public virtual DateTime? ExpirationDate
		{
			get
			{
				return this._ExpirationDate;
			}
			set
			{
				this._ExpirationDate = value;
			}
		}
		#endregion*/

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

		//2020/04/09 add IsActive
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected bool? _IsActive;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "IsActive",Required =true)]
		public virtual bool? IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				this._IsActive = value;
			}
		}
        #endregion

        #region SiteFax
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "SiteFax")]
        public virtual string SiteFax { get; set; }
        public abstract class siteFax : PX.Data.BQL.BqlString.Field<siteFax> { }
        #endregion

        public const string TWTAXAGENCY = "TWTAXAGENCY";
		public const string SegmentID_PART1 = "1";

		public class tWTAXAGENCY : Constant<string>
		{
			public tWTAXAGENCY() : base(TWTAXAGENCY) { }
		}
		public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }
	}
}
