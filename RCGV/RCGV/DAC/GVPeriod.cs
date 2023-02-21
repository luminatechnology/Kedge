using System;
using PX.Data;
using PX.Objects.TX;
using PX.Objects.GL;
using RCGV.GV.Util;
using RCGV.GV.Attribute.Selector;

namespace RCGV.GV.DAC
{
	[Serializable]
	public class GVPeriod : IBqlTable
	{
		#region GvPeriodID
		public abstract class gvPeriodID : PX.Data.IBqlField
		{
		}
		protected int? _GvPeriodID;
		[PXDBIdentity]
		public virtual int? GvPeriodID
		{
			get
			{
				return this._GvPeriodID;
			}
			set
			{
				this._GvPeriodID = value;
			}
		}
        #endregion
        #region RegistrationCD
        public abstract class registrationCD : PX.Data.IBqlField
        {
        }
        protected string _RegistrationCD;
        [PXDBString(9, IsUnicode = true, InputMask = "",IsKey =true)]
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
        #region PeriodYear
        public abstract class periodYear : PX.Data.IBqlField
		{
		}
		protected int? _PeriodYear;
		[PXDBInt(IsKey = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Period Year", Enabled = false, Required = true, Visibility = PXUIVisibility.Visible)]
		public virtual int? PeriodYear
		{
			get { return this._PeriodYear; }
			set { this._PeriodYear = value; }
		}
		#endregion
		#region PeriodMonth
		public abstract class periodMonth : PX.Data.IBqlField
		{
		}
		protected int? _PeriodMonth;
		[PXDBInt(IsKey = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Period Month", Enabled = false, Required = true, Visibility = PXUIVisibility.Visible)]
		public virtual int? PeriodMonth
		{
			get { return this._PeriodMonth; }
			set { this._PeriodMonth = value; }
		}
		#endregion
		#region InActive
		public abstract class inActive : PX.Data.IBqlField
		{
		}
		protected bool? _InActive;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "In Active")]
		public virtual bool? InActive
		{
			get
			{
				return this._InActive;
			}
			set
			{
				this._InActive = value;
			}
		}
		#endregion
		#region OutActive
		public abstract class outActive : PX.Data.IBqlField
		{
		}
		protected bool? _OutActive;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Out Active")]
		public virtual bool? OutActive
		{
			get
			{
				return this._OutActive;
			}
			set
			{
				this._OutActive = value;
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
	}
}
