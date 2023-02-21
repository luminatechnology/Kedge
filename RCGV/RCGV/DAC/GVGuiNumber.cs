﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	using RCGV.GV.Util;
    using RCGV.GV.Attribute.Selector;

    [System.SerializableAttribute()]
	public class GVGuiNumber : PX.Data.IBqlTable
	{
		public bool isFirst = true;
        #region Today
        public abstract class today : PX.Data.IBqlField
        {
        }
        protected DateTime? _Today;
        [PXDate]
        //DateTime.Today,AccessInfo.businessDate
        [PXDefault(typeof(AccessInfo.businessDate),PersistingCheck = PXPersistingCheck.Nothing)]
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
        #region RegistrationCD
        public abstract class registrationCD : PX.Data.IBqlField
		{
		}
		protected string _RegistrationCD;
		[PXDBString(9, IsUnicode = true,InputMask = "")]
		[PXUIField(DisplayName = "Registration CD", Required = true)]
        [RegistrationCDAttribute()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
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
        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
        [PXDefault(true)]
        public virtual Boolean? Hold { get; set; }
        public abstract class hold : IBqlField { }
        #endregion
        #region IsUse
        [PXBool()]
        [PXUIField(DisplayName = "IsUse", Visibility = PXUIVisibility.Visible)]
        [PXDefault(PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual Boolean? IsUse { get; set; }
        public abstract class isUse : IBqlField { }
        #endregion

        #region GuiNumberID
        public abstract class guiNumberID : PX.Data.IBqlField
		{
		}
		protected int? _GuiNumberID;
		[PXDBIdentity(IsKey =true)]
		[PXUIField(DisplayName = "Gui Number ID")]
		public virtual int? GuiNumberID
		{
			get
			{
				return this._GuiNumberID;
			}
			set
			{
				this._GuiNumberID = value;
			}
		}
		#endregion
		#region DeclareYear
		public abstract class declareYear : PX.Data.IBqlField
		{
		}
		protected int? _DeclareYear;
		[PXDBInt(MaxValue=9999,MinValue=0000)]
		[PXDefault(typeof(DatePart<DatePart.year,Current<AccessInfo.businessDate>>), PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "DeclareYear", Required = true)]
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
		#region DeclarePeriod
		public abstract class declarePeriod : PX.Data.IBqlField
		{
		}
		protected string _DeclarePeriod;
		[PXDBString(1, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVLookUpCodeExtAttribute(typeof(GVGuiNumber.declarePeriod), GVLookUpCodeUtil.DeclarePeriod)]
        [PXUIField(DisplayName = "Declare Period", Required = true)]
        public virtual string DeclarePeriod
		{
			get
			{
				return this._DeclarePeriod;
			}
			set
			{
				this._DeclarePeriod = value;
			}
		}
		#endregion
		#region GuiWordID
		public abstract class guiWordID : PX.Data.IBqlField
		{
		}
		protected int? _GuiWordID;
		[PXDBInt()]
        /*
		[PXSelector(
			typeof(Search<GVGuiWord.guiWordID,
							Where<GVGuiWord.declareYear, Equal<Current<GVGuiNumber.declareYear>>
							  , And<GVGuiWord.declarePeriod, Equal<Current<GVGuiNumber.declarePeriod>>>>,
                                  OrderBy<Desc<GVGuiWord.declareYear, Desc<GVGuiWord.declarePeriod, Desc<GVGuiWord.guiWordCD>>>
                                  >>)*/
        [PXSelector(
            typeof(Search<GVGuiWord.guiWordID,
                            Where<GVGuiWord.declareYear, Equal<Current<GVGuiNumber.declareYear>>>,
                                  OrderBy<Desc<GVGuiWord.declareYear, Desc<GVGuiWord.declarePeriod, Desc<GVGuiWord.guiWordCD>>>
                                  >>),

            typeof(GVGuiWord.guiWordCD),
            typeof(GVGuiWord.declareYear),
            typeof(GVGuiWord.declarePeriod),
                SubstituteKey = typeof(GVGuiWord.guiWordCD)

        )]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "GUI Word ID", Required = true)]
		public virtual int? GuiWordID
		{
			get
			{
				return this._GuiWordID;
			}
			set
			{
				this._GuiWordID = value;
			}
		}
		#endregion
		#region ApplyStartNumber
		public abstract class applyStartNumber : PX.Data.IBqlField
		{
		}
		protected string _ApplyStartNumber;
		[PXDBString(8, IsUnicode = true, InputMask = "00000000")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Apply Start Number", Required = true)]
		public virtual string ApplyStartNumber
		{
			get
			{
				return this._ApplyStartNumber;
			}
			set
			{
				this._ApplyStartNumber = value;
			}
		}
		#endregion
		#region ApplyEndNumber
		public abstract class applyEndNumber : PX.Data.IBqlField
		{
		}
		protected string _ApplyEndNumber;
		[PXDBString(8, IsUnicode = true, InputMask = "00000000")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Apply End Number",Required=true)]
		public virtual string ApplyEndNumber
		{
			get
			{
				return this._ApplyEndNumber;
			}
			set
			{
				this._ApplyEndNumber = value;
			}
		}
		#endregion
		#region CurrentNumber
		public abstract class currentNumber : PX.Data.IBqlField
		{
		}
		protected string _CurrentNumber;
		[PXDBString(8, IsUnicode = true)]
		[PXUIField(DisplayName = "CurrentNumber")]
		public virtual string CurrentNumber
		{
			get
			{
				return this._CurrentNumber;
			}
			set
			{
				this._CurrentNumber = value;
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
		[PXUIField(DisplayName = "Tstamp")]
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
