using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
    [Serializable]
    public class KGFlowBudChgApplyNew : IBqlTable
    {

        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWBCN")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region Bcnid
        [PXDBIdentity(/*IsKey = true*/)]
        [PXUIField(DisplayName = "Bcnid")]
        public virtual int? Bcnid { get; set; }
        public abstract class bcnid : PX.Data.BQL.BqlInt.Field<bcnid> { }
        #endregion

        #region Bcnuid
        [PXDBString(40, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Bcnuid")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string Bcnuid { get; set; }
        public abstract class bcnuid : PX.Data.BQL.BqlString.Field<bcnuid> { }
        #endregion
        
        #region Bcauid
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bcauid")]
        [PXDBDefault(typeof(KGFlowChangeManagement.bcaUID))]
        [PXParent(typeof(Select<KGFlowChangeManagement,
                                        Where<KGFlowChangeManagement.bcaUID,
                                        Equal<Current<bcauid>>>>))]
        public virtual string Bcauid { get; set; }
        public abstract class bcauid : PX.Data.BQL.BqlString.Field<bcauid> { }
        #endregion

        #region InvNo
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inv No")]
        public virtual string InvNo { get; set; }
        public abstract class invNo : PX.Data.BQL.BqlString.Field<invNo> { }
        #endregion

        #region Title
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Title")]
        public virtual string Title { get; set; }
        public abstract class title : PX.Data.BQL.BqlString.Field<title> { }
        #endregion

        #region ItemName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Name")]
        public virtual string ItemName { get; set; }
        public abstract class itemName : PX.Data.BQL.BqlString.Field<itemName> { }
        #endregion

        #region Memo
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Memo")]
        public virtual string Memo { get; set; }
        public abstract class memo : PX.Data.BQL.BqlString.Field<memo> { }
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
        [PXUIField(DisplayName = "NoteID")]
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