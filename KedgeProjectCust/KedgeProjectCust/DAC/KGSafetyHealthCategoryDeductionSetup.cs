using System;
using PX.Data;

namespace Kedge.DAC
{
  [Serializable]
  public class KGSafetyHealthCategoryDeductionSetup : IBqlTable
  {
    #region Selected
    [PXBool]
    [PXUIField(DisplayName = "Selected")]
    public virtual bool? Selected { get; set; }
    public abstract class selected : IBqlField { }
    #endregion
    
    #region CategoryCD
    [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Safety Health Category CD", Required = true)]
    public virtual string CategoryCD { get; set; }
    public abstract class categoryCD : IBqlField { }
    #endregion

    #region DeductionSetup
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Safety Health Deduction Setup", Required = true)]
    public virtual Decimal? DeductionSetup { get; set; }
    public abstract class deductionSetup : IBqlField { }
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