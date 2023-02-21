﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	using PX.Objects.CR;
	
	[System.SerializableAttribute()]
	public class GVRegistrationBranch : PX.Data.IBqlTable
	{
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected int? _BAccountID;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "AcctCD")]
		//[PXParent(typeof(Select<BAccount,
		//	Where<GVRegistrationBranch.bAccountID, Equal<Current<BAccount.bAccountID>>>>))]
		
		//2020/04/09 ADD SubstituteKey
		[PXSelector(typeof(Search<BAccount.bAccountID,
			Where<BAccount.status, Equal<StatusA.SA>,
			And2<Where<BAccount.type, Equal<BAccountType.organizationType>>,
				Or<BAccount.type, Equal<BAccountType.branchType>>>>>),
			typeof(BAccount.acctCD),
			typeof(BAccount.acctName),
			SubstituteKey =(typeof(BAccount.acctCD))
			)]
        public virtual int? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region RegistrationID
		public abstract class registrationID : PX.Data.IBqlField
		{
		}
		protected int? _RegistrationID;
		[PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(GVRegistration.registrationID))]
        [PXUIField(DisplayName = "RegistrationID")]
		[PXParent(typeof(Select<GVRegistration,
			Where<GVRegistration.registrationID,
			Equal<Current<GVRegistrationBranch.registrationID>>>>))]
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

	public static class StatusA
	{
		public const string A = "A";
		public class SA : Constant<string>
		{
			public SA()
				: base(A)
			{
			}
		}

	}
}
