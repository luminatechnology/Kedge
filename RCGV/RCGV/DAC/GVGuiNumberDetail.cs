﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	public class GVGuiNumberDetail : PX.Data.IBqlTable
	{
		#region GuiNumberID
		public abstract class guiNumberID : PX.Data.IBqlField
		{
		}
		protected int? _GuiNumberID;
		[PXDBInt( )]
		[PXUIField(DisplayName = "GuiNumberID")]
		[PXDBDefault(typeof(GVGuiNumber.guiNumberID))]
		[PXParent(typeof(Select<GVGuiNumber,
								Where<GVGuiNumber.guiNumberID,
								Equal<Current<GVGuiNumberDetail.guiNumberID>>>>))]
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
		#region GuiNumberDetailID
		public abstract class guiNumberDetailID : PX.Data.IBqlField
		{
		}
		protected int? _GuiNumberDetailID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(Enabled = false)]
		public virtual int? GuiNumberDetailID
		{
			get
			{
				return this._GuiNumberDetailID;
			}
			set
			{
				this._GuiNumberDetailID = value;
			}
		}
		#endregion
		#region LineID
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt()]
		[PXDefault(1)]
		[PXUIField(DisplayName = "LineNbr", Enabled = false, Required = true)]
        [PXLineNbr(typeof(GVGuiNumber))]

        public virtual int? LineNbr
        {
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region StartNumber
		public abstract class startNumber : PX.Data.IBqlField
		{
		}
		protected string _StartNumber;
		[PXDBString(8, IsUnicode = true)]
		[PXUIField(DisplayName = "StartNumber", Enabled = false, Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string StartNumber
		{
			get
			{
				return this._StartNumber;
			}
			set
			{
				this._StartNumber = value;
			}
		}
		#endregion
		#region EndNumber
		public abstract class endNumber : PX.Data.IBqlField
		{
		}
		protected string _EndNumber;
		[PXDBString(8, IsUnicode = true, InputMask = "00000000")]
		[PXUIField(DisplayName = "EndNumber", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string EndNumber
		{
			get
			{
				return this._EndNumber;
			}
			set
			{
				this._EndNumber = value;
			}
		}
		#endregion
		#region GuiWordCD
		public abstract class guiWordCD : PX.Data.IBqlField
		{
		}
		protected string _GuiWordCD;
		[PXString()]
		[PXDBScalar(
			typeof(Search2<GVGuiWord.guiWordCD,
				InnerJoin<GVGuiNumber, 
                    On<GVGuiWord.guiWordID, Equal<GVGuiNumber.guiWordID>>>,
                    Where<GVGuiNumber.guiNumberID, Equal < GVGuiNumberDetail.guiNumberID >>>
				)
		)]
		public virtual string GuiWordCD
		{
			get
			{
				return this._GuiWordCD;
			}
			set
			{
				this._GuiWordCD = value;
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
