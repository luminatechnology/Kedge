using System;
using PX.Data;
using RCGV.GV;

namespace RCGV.GV.DAC
{
  [Serializable]
  public class GVLookupCodeValue : IBqlTable
  {
    #region LookupCodeValueID

    [PXUIField(DisplayName = "Lookup Code Value ID")]
	[PXDBIdentity(IsKey = true)]
    public virtual int? LookupCodeValueID { get; set; }
    public abstract class lookupCodeValueID : IBqlField { }
    #endregion

    #region LookupCodeTypeID
	[PXDBInt()]
    [PXUIField(DisplayName = "Lookup Code Type ID")]
	[PXDBDefault(typeof(GVLookupCodeType.lookupCodeTypeID))]
	[PXParent(typeof(Select<GVLookupCodeType,
							Where<GVLookupCodeType.lookupCodeTypeID,
							Equal<Current<GVLookupCodeValue.lookupCodeTypeID>>>>))]
	public virtual int? LookupCodeTypeID { get; set; }
    public abstract class lookupCodeTypeID : IBqlField { }
    #endregion

    #region LookupCode
	[PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
    [PXUIField(DisplayName = "Lookup Code" ,Required =true)]

    public virtual string LookupCode { get; set; }
    public abstract class lookupCode : IBqlField { }
    #endregion

    #region LookupCodeValue
    [PXDBString(240, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Lookup Code Value", Required = true)]
    [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
    public virtual string LookupCodeValue { get; set; }
    public abstract class lookupCodeValue : IBqlField { }
        #endregion

    #region IsActive
    public abstract class IsActive : PX.Data.IBqlField
    {
    }
    protected bool? _IsActive;
    [PXDBBool()]
    [PXDefault(true)]
    [PXUIField(DisplayName = "IsActive")]
    public virtual bool? isActive
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