using System;
using PX.Data;

namespace PX.Objects.EP.AgentFlow.DAC
{
  [Serializable]
  [PXCacheName("KGFlowFinBillingWht")]
  public class KGFlowFinBillingWht : IBqlTable
  {
    #region BranchID
    [PXDBInt()]
    [PXUIField(DisplayName = "Branch ID")]
    public virtual int? BranchID { get; set; }
    public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
    #endregion

    #region HeaderID
    [PXDBInt()]
    [PXUIField(DisplayName = "Header ID")]
    [PXDBDefault(typeof(KGFlowFinBillingAH.headerID))]
    [PXParent(typeof(Select<KGFlowFinBillingAH,
                                Where<KGFlowFinBillingAH.headerID,
                                Equal<Current<headerID>>>>))]
    public virtual int? HeaderID { get; set; }
    public abstract class headerID : PX.Data.BQL.BqlInt.Field<headerID> { }
        #endregion

    #region WhtID
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "WhtID")]
    public virtual int? WhtID { get; set; }
    public abstract class whtID : PX.Data.BQL.BqlInt.Field<whtID> { }
    #endregion

    #region PersonalID
    [PXDBString(10, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Personal ID")]
    public virtual string PersonalID { get; set; }
    public abstract class personalID : PX.Data.BQL.BqlString.Field<personalID> { }
    #endregion

    #region PayeeName
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Payee Name")]
    public virtual string PayeeName { get; set; }
    public abstract class payeeName : PX.Data.BQL.BqlString.Field<payeeName> { }
    #endregion

    #region EntryYy
    [PXDBString(3, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Entry Yy")]
    public virtual string EntryYy { get; set; }
    public abstract class entryYy : PX.Data.BQL.BqlString.Field<entryYy> { }
    #endregion

    #region EntryMm
    [PXDBString(2, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Entry Mm")]
    public virtual string EntryMm { get; set; }
    public abstract class entryMm : PX.Data.BQL.BqlString.Field<entryMm> { }
    #endregion

    #region Whtfmtcode
    [PXDBString(2, IsFixed = true, InputMask = "")]
    [PXUIField(DisplayName = "Whtfmtcode")]
    public virtual string Whtfmtcode { get; set; }
    public abstract class whtfmtcode : PX.Data.BQL.BqlString.Field<whtfmtcode> { }
    #endregion

    #region DuTypeCode
    [PXDBString(8, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Du Type Code")]
    public virtual string DuTypeCode { get; set; }
    public abstract class duTypeCode : PX.Data.BQL.BqlString.Field<duTypeCode> { }
    #endregion

    #region Tax2Acct
    [PXDBInt()]
    [PXUIField(DisplayName = "Tax2 Acct")]
    public virtual int? Tax2Acct { get; set; }
    public abstract class tax2Acct : PX.Data.BQL.BqlInt.Field<tax2Acct> { }
    #endregion

    #region Whtamt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Whtamt")]
    public virtual Decimal? Whtamt { get; set; }
    public abstract class whtamt : PX.Data.BQL.BqlDecimal.Field<whtamt> { }
    #endregion

    #region Gnhi2amt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Gnhi2amt")]
    public virtual Decimal? Gnhi2amt { get; set; }
    public abstract class gnhi2amt : PX.Data.BQL.BqlDecimal.Field<gnhi2amt> { }
    #endregion

    #region HouseTaxIdType
    [PXDBString(1, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "House Tax Id Type")]
    public virtual string HouseTaxIdType { get; set; }
    public abstract class houseTaxIdType : PX.Data.BQL.BqlString.Field<houseTaxIdType> { }
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

    #region NoteID
    [PXNote()]
    public virtual Guid? NoteID { get; set; }
    public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
    #endregion

    #region Tstamp
    [PXDBTimestamp()]
    [PXUIField(DisplayName = "Tstamp")]
    public virtual byte[] Tstamp { get; set; }
    public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
    #endregion
  }
}