using System;
using PX.Data;
using PX.Objects.RQ;
using Kedge.DAC;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;

namespace Kedge.DAC
{
      [Serializable]
      public class KGRequestFile : IBqlTable
      {
            #region Fileid
            //[PXDBInt(IsKey = true)]
            [PXDBIdentity(IsKey = true)]
            [PXUIField(DisplayName = "Fileid",Enabled =false)]
            //[AutoNumber(typeof(KGSetUp.kGRequestNumbering), typeof(AccessInfo.businessDate))]
            public virtual int? Fileid { get; set; }
            public abstract class fileid : PX.Data.BQL.BqlInt.Field<fileid> { }
            #endregion

            #region OrderNbr
            [PXDBString(20, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Order Nbr")]
            [PXDBDefault(typeof(RQRequest.orderNbr))]
            [PXParent(typeof(Select<RQRequest,
                                    Where<RQRequest.orderNbr,
                                    Equal<Current<KGRequestFile.orderNbr>>>>))]
            public virtual string OrderNbr { get; set; }
            public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
            #endregion

            #region FileType
            [PXDBString(50, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "File Type")]
            [FileType.List]
            public virtual string FileType { get; set; }
            public abstract class fileType : PX.Data.BQL.BqlString.Field<fileType> { }
            #endregion

            #region FIleDesc
            [PXDBString(240, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "FIle Desc")]
            public virtual string FIleDesc { get; set; }
            public abstract class fIleDesc : PX.Data.BQL.BqlString.Field<fIleDesc> { }
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
		    [Branch(typeof(RQRequest.branchID))]
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