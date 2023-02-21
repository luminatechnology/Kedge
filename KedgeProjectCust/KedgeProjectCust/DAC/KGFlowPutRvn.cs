using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowPutRvn : IBqlTable
      {

        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get { return "KGFLOWRVN"; }  }
        #endregion

        #region RvnID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Rvnid")]
        public virtual int? RvnID { get; set; }
        public abstract class rvnID : PX.Data.BQL.BqlInt.Field<rvnID> { }
        #endregion

        #region RvnUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "RvnUID")]
        //[AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string RvnUID { get; set; }
        public abstract class rvnUID : PX.Data.BQL.BqlString.Field<rvnUID> { }
        #endregion

        #region BcaUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "BcaUID")]
        [PXDBDefault(typeof(KGFlowChangeManagement.bcaUID))]
        [PXParent(typeof(Select<KGFlowChangeManagement,
                                    Where<KGFlowChangeManagement.bcaUID,
                                    Equal<Current<KGFlowPutRvn.bcaUID>>>>))]
        public virtual string BcaUID { get; set; }
        public abstract class bcaUID : PX.Data.BQL.BqlString.Field<bcaUID> { }
        #endregion

        #region SubCode
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sub Code")]
        public virtual string SubCode { get; set; }
        public abstract class subCode : PX.Data.BQL.BqlString.Field<subCode> { }
        #endregion
         
        #region SubName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sub Name")]
        public virtual string SubName { get; set; }
        public abstract class subName : PX.Data.BQL.BqlString.Field<subName> { }
        #endregion

        #region InvUID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "InvUID")]
        public virtual string InvUID { get; set; }
        public abstract class invUID : PX.Data.BQL.BqlString.Field<invUID> { }
        #endregion

        #region Title
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Title")]
        public virtual string Title { get; set; }
        public abstract class title : PX.Data.BQL.BqlString.Field<title> { }
        #endregion

        #region Num
        [PXDBInt()]
        [PXUIField(DisplayName = "Num")]
        public virtual int? Num { get; set; }
        public abstract class num : PX.Data.BQL.BqlInt.Field<num> { }
        #endregion

        #region PutRvnNo
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Put Rvn No")]
        public virtual string PutRvnNo { get; set; }
        public abstract class putRvnNo : PX.Data.BQL.BqlString.Field<putRvnNo> { }
        #endregion

        #region InputDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Input Date")]
        public virtual DateTime? InputDate { get; set; }
        public abstract class inputDate : PX.Data.BQL.BqlDateTime.Field<inputDate> { }
        #endregion

        #region AddDay
        [PXDBInt()]
        [PXUIField(DisplayName = "Add Day")]
        public virtual int? AddDay { get; set; }
        public abstract class addDay : PX.Data.BQL.BqlInt.Field<addDay> { }
        #endregion

        #region FinishDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Finish Date")]
        public virtual DateTime? FinishDate { get; set; }
        public abstract class finishDate : PX.Data.BQL.BqlDateTime.Field<finishDate> { }
        #endregion

        #region Cmemo
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cmemo")]
        public virtual string Cmemo { get; set; }
        public abstract class cmemo : PX.Data.BQL.BqlString.Field<cmemo> { }
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
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID>
        {
        }
        protected Guid? _NoteID;

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
        [PXNote()]
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
      }
}