using System;
using PX.Data;

namespace RCGV.GV.DAC
{
  [Serializable]
  public class GVLookupCodeType : IBqlTable
  {
    #region LookupCodeTypeID
 
    [PXUIField(DisplayName = "Lookup Code Type ID")]
	[PXDBIdentity()]
    public virtual int? LookupCodeTypeID { get; set; }
    public abstract class lookupCodeTypeID : IBqlField { }
    #endregion

    #region LookupCodeType
	[PXDBString(60, IsUnicode = true, InputMask = "", IsKey = true)]
    [PXUIField(DisplayName = "Lookup Code Type", Required = true)]
    [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
    [PXSelector(
			typeof(Search<GVLookupCodeType.lookupCodeType>),
			typeof(GVLookupCodeType.lookupCodeType),
			typeof(GVLookupCodeType.lookupCodeTypeDesc)
	
			)]
    public virtual string LookupCodeType { get; set; }
    public abstract class lookupCodeType : IBqlField { }
    #endregion

    #region LookupCodeTypeDesc
    [PXDBString(240, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Lookup Code Type Desc")]
    public virtual string LookupCodeTypeDesc { get; set; }
    public abstract class lookupCodeTypeDesc : IBqlField { }
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
    [PXUIField(DisplayName = "Created Date Time")]
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

    #region Noteid
    [PXNote]
    [PXUIField(DisplayName = "Noteid")]
    public virtual Guid? Noteid { get; set; }
    public abstract class noteid : IBqlField { }
    #endregion

    #region Tstamp
    [PXDBTimestamp()]
    [PXUIField(DisplayName = "Tstamp")]
    public virtual byte[] Tstamp { get; set; }
    public abstract class tstamp : IBqlField { }
    #endregion
  }
}