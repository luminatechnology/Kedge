using System;
using PX.Data;
using NM.Util;
using PX.Objects.GL;

namespace NM.DAC
{
    [Serializable]
    public class NMPreference : IBqlTable
    {
        #region PreferenceID
        [PXDBInt()]
        [PXUIField(DisplayName = "Preference ID")]
        [PXDefault(1)]
        public virtual int? PreferenceID { get; set; }
        public abstract class preferenceID : PX.Data.BQL.BqlInt.Field<preferenceID> { }
        #endregion

        #region ARNoteAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARNote Account ID")]
        [NMAccount()]
        public virtual int? ARNoteAccountID { get; set; }
        public abstract class aRNoteAccountID : PX.Data.BQL.BqlInt.Field<aRNoteAccountID> { }
        #endregion

        #region ARNoteSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARNoteSubID")]
        [NMSub]
        public virtual int? ARNoteSubID { get; set; }
        public abstract class aRNoteSubID : PX.Data.BQL.BqlInt.Field<aRNoteSubID> { }
        #endregion

        #region ARComNoteAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARCom Note Account ID")]
        [NMAccount()]
        public virtual int? ARComNoteAccountID { get; set; }
        public abstract class aRComNoteAccountID : PX.Data.BQL.BqlInt.Field<aRComNoteAccountID> { }
        #endregion

        #region ARComNoteSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARComNoteSubID")]
        [NMSub]
        public virtual int? ARComNoteSubID { get; set; }
        public abstract class aRComNoteSubID : PX.Data.BQL.BqlInt.Field<aRComNoteSubID> { }
        #endregion

        #region GuarNoteAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Guar Note Account ID")]
        [NMAccount()]
        public virtual int? GuarNoteAccountID { get; set; }
        public abstract class guarNoteAccountID : PX.Data.BQL.BqlInt.Field<guarNoteAccountID> { }
        #endregion

        #region GuarNoteSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "Guar Note Sub ID")]
        [NMSub]
        public virtual int? GuarNoteSubID { get; set; }
        public abstract class guarNoteSubID : PX.Data.BQL.BqlInt.Field<guarNoteSubID> { }
        #endregion

        #region ARGuarAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARGuar Account ID")]
        [NMAccount()]
        public virtual int? ARGuarAccountID { get; set; }
        public abstract class aRGuarAccountID : PX.Data.BQL.BqlInt.Field<aRGuarAccountID> { }
        #endregion

        #region ARGuarSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARGuarSubID")]
        [NMSub]
        public virtual int? ARGuarSubID { get; set; }
        public abstract class aRGuarSubID : PX.Data.BQL.BqlInt.Field<aRGuarSubID> { }
        #endregion

        #region APNoteAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "APNote Account ID")]
        [NMAccount()]
        public virtual int? APNoteAccountID { get; set; }
        public abstract class aPNoteAccountID : PX.Data.BQL.BqlInt.Field<aPNoteAccountID> { }
        #endregion

        #region APNoteSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "APNoteSubID")]
        [NMSub]
        public virtual int? APNoteSubID { get; set; }
        public abstract class aPNoteSubID : PX.Data.BQL.BqlInt.Field<aPNoteSubID> { }
        #endregion

        #region APComNoteAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "APCom Note Account ID")]
        [NMAccount()]
        public virtual int? APComNoteAccountID { get; set; }
        public abstract class aPComNoteAccountID : PX.Data.BQL.BqlInt.Field<aPComNoteAccountID> { }
        #endregion

        #region APComNoteSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "APComNoteSubID")]
        [NMSub]
        public virtual int? APComNoteSubID { get; set; }
        public abstract class aPComNoteSubID : PX.Data.BQL.BqlInt.Field<aPComNoteSubID> { }
        #endregion

        #region GuarTicketAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Guar Ticket Account ID")]
        [NMAccount()]
        public virtual int? GuarTicketAccountID { get; set; }
        public abstract class guarTicketAccountID : PX.Data.BQL.BqlInt.Field<guarTicketAccountID> { }
        #endregion

        #region GuarTicketSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "Guar Ticket Sub ID")]
        [NMSub]
        public virtual int? GuarTicketSubID { get; set; }
        public abstract class guarTicketSubID : PX.Data.BQL.BqlInt.Field<guarTicketSubID> { }
        #endregion

        #region APGuarAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "APGuar Account ID")]
        [NMAccount()]
        public virtual int? APGuarAccountID { get; set; }
        public abstract class aPGuarAccountID : PX.Data.BQL.BqlInt.Field<aPGuarAccountID> { }
        #endregion

        #region APGuarSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "APGuarSubID")]
        [NMSub]
        public virtual int? APGuarSubID { get; set; }
        public abstract class aPGuarSubID : PX.Data.BQL.BqlInt.Field<aPGuarSubID> { }
        #endregion

        #region NMBatchDate
        [PXDBDate()]
        public virtual DateTime? NMBatchDate { get; set; }
        public abstract class nmBatchDate : PX.Data.BQL.BqlDateTime.Field<nmBatchDate> { }
        #endregion

        #region NMBatchSeq
        [PXDBInt()]
        public virtual int? NMBatchSeq { get; set; }
        public abstract class nmBatchSeq : PX.Data.BQL.BqlInt.Field<nmBatchSeq> { }
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
        [PXUIField(DisplayName = "Created Date Time")]
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
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteId
        [PXNote()]
        [PXUIField(DisplayName = "NoteId")]
        public virtual Guid? NoteId { get; set; }
        public abstract class noteId : PX.Data.BQL.BqlGuid.Field<noteId> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}