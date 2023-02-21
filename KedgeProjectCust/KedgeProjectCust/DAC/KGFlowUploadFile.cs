using System;
using PX.Data;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGFlowUploadFile : IBqlTable
    {
        #region Fileid
        [PXDBGuid(IsKey =true)]
        [PXUIField(DisplayName = "Fileid")]
        public virtual Guid? Fileid { get; set; }
        public abstract class fileid : PX.Data.BQL.BqlGuid.Field<fileid> { }
        #endregion

        #region FileType
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "File Type")]
        public virtual string FileType { get; set; }
        public abstract class fileType : PX.Data.BQL.BqlString.Field<fileType> { }
        #endregion

        #region FileName
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "File Name")]
        public virtual string FileName { get; set; }
        public abstract class fileName : PX.Data.BQL.BqlString.Field<fileName> { }
        #endregion

        #region FileLink
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "File Link")]
        public virtual string FileLink { get; set; }
        public abstract class fileLink : PX.Data.BQL.BqlString.Field<fileLink> { }
        #endregion

        #region RefNo
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref No")]
        public virtual string RefNo { get; set; }
        public abstract class refNo : PX.Data.BQL.BqlString.Field<refNo> { }
        #endregion

        #region Category
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Category")]
        public virtual string Category { get; set; }
        public abstract class category : PX.Data.BQL.BqlString.Field<category> { }
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
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch()]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion*/
    }
}