using System;
using PX.Data;
using RCGV.GV;
using RCGV.GV.Util;

namespace RCGV.GV.DAC
{
  [Serializable]
  public class GVGuiType : IBqlTable
  {
    #region GuiTypeID
	[PXDBIdentity()]
    [PXUIField(DisplayName = "Gui Type ID")]
    public virtual int? GuiTypeID { get; set; }
    public abstract class guiTypeID : IBqlField { }
    #endregion

    #region GuiTypeCD
	[PXDBString(2, IsUnicode = true, InputMask = "", IsKey = true)]
    [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
    [PXUIField(DisplayName = "Gui Type CD", Required = true)]
    public virtual string GuiTypeCD { get; set; }
    public abstract class guiTypeCD : IBqlField { }
    #endregion


    #region GvType
    [PXDBString(1, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Gv Type", Required = true)]
    [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
    [GVLookUpCodeExtAttribute(typeof(GVGuiType.gvType), GVLookUpCodeUtil.GvType)]
    public virtual string GvType { get; set; }
    public abstract class gvType : IBqlField { }
    #endregion

    #region GuiTypeDesc
    [PXDBString(240, IsUnicode = true, InputMask = "")]
    [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
    [PXUIField(DisplayName = "Gui Type Desc", Required = true)]
    public virtual string GuiTypeDesc { get; set; }
    public abstract class guiTypeDesc : IBqlField { }
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

    }
}