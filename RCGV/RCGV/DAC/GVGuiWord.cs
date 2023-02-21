﻿namespace RCGV.GV.DAC
{
	using System;
	using PX.Data;
    using RCGV.GV.Util;

    [System.SerializableAttribute()]
    public class GVGuiWord : PX.Data.IBqlTable
    {


        #region DeclareYear
        public abstract class declareYear : PX.Data.IBqlField
        {
        }
        protected int? _DeclareYear;
        [PXDBInt(IsKey = true, MinValue = 1911, MaxValue = 9999)]
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
        #region GuiWordID
        public abstract class guiWordID : PX.Data.IBqlField
        {
        }
        protected int? _GuiWordID;
        [PXDBIdentity()]
        [PXUIField(Enabled = false)]
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
        #region GuiWordCD
        public abstract class guiWordCD : PX.Data.IBqlField
        {
        }
        protected string _GuiWordCD;
        [PXDBString(2, IsKey = true, IsUnicode = true, InputMask = ">LL")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "GuiWordCD", Required = true)]
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

        #region DeclarePeriod
        public abstract class declarePeriod : PX.Data.IBqlField
        {
        }
        protected string _DeclarePeriod;
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "DeclarePeriod", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVLookUpCodeExtAttribute(typeof(GVGuiWord.declarePeriod), GVLookUpCodeUtil.DeclarePeriod)]
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
        
        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }

}
