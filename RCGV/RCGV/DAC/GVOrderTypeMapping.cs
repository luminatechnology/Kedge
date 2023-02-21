﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
    using RCGV.GV.Util;

	[System.SerializableAttribute()]
	public class GVOrderTypeMapping : PX.Data.IBqlTable
	{
		#region OrderTypeMappingID
		public abstract class orderTypeMappingID : PX.Data.IBqlField
		{
		}
		protected int? _OrderTypeMappingID;
		[PXDBIdentity()]		
		public virtual int? OrderTypeMappingID
		{
			get
			{
				return this._OrderTypeMappingID;
			}
			set
			{
				this._OrderTypeMappingID = value;
			}
		}
		#endregion
		#region GvType
		public abstract class gvType : PX.Data.IBqlField
		{

		}
		protected string _GvType;
		[PXDBString(1, IsUnicode = true, IsKey = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "GvType", Required = true)]
        [GVLookUpCodeExtAttribute(typeof(GVOrderTypeMapping.gvType), GVLookUpCodeUtil.GvType)]
       
		public virtual string GvType
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
		#region OrderTypeCD
		public abstract class orderTypeCD : PX.Data.IBqlField
		{
		}
		protected string _OrderTypeCD;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "OrderTypeCD", Required = true)]        
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXStringList()]
      
        public virtual string OrderTypeCD
		{
			get
			{
				return this._OrderTypeCD;
			}
			set
			{
				this._OrderTypeCD = value;
			}
		}
		#endregion
		#region GuiSubType
		public abstract class guiSubType : PX.Data.IBqlField
		{
		}
		protected string _GuiSubType;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "GuiSubType", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]        
        [PXStringList()]
        public virtual string GuiSubType
		{
			get
			{
				return this._GuiSubType;
			}
			set
			{
				this._GuiSubType = value;
			}
		}
        #endregion

        #region IsActive
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Active")]
        [PXDefault(true)]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : IBqlField { }
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
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }
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
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : IBqlField { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion

        public static class GVType1
		{
			public const string I = "I";
			public const string O = "O";

		}
		
		public class GUIType
		{

			public const string IInv = GuiSubTypeAttribute.IInv;
			public const string IAlw = GuiSubTypeAttribute.IAlw;
			public const string OInv = GuiSubTypeAttribute.OInv;
			public const string OAlw = GuiSubTypeAttribute.OAlw;

		}

	}

	public static class GuiSubTypeAttribute
	{
		public const string IInv = "IInv";
		public const string IAlw = "IAlw";
		public const string OInv = "OInv";
		public const string OAlw = "OAlw";

		public class iInv : Constant<string>
		{
			public iInv() : base(IInv) { }
		}

		public class iAlw : Constant<string>
		{
			public iAlw() : base(IAlw) { }
		}

		public class oInv : Constant<string>
		{
			public oInv() : base(OInv) { }
		}

		public class oAlw : Constant<string>
		{
			public oAlw() : base(OAlw) { }
		}

	}
    
	public class OrderTypeAttribute : PXStringListAttribute
	{
		public static String[] key = GVLookupCodeMaint.getKeyByDic(GVLookupCodeMaint.getSelection("GvType"));
		public static String[] value = GVLookupCodeMaint.getValueByDic(GVLookupCodeMaint.getSelection("GvType"));
		public OrderTypeAttribute() : base(key, value) { }
	}
	
	
}
