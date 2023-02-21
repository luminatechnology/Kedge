using System;
using PX.Data;
using PX.Objects.RQ;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGRequisitionFile : IBqlTable
    {
        #region FileID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "File ID")]
        public virtual int? FileID { get; set; }
        public abstract class fileID : PX.Data.BQL.BqlInt.Field<fileID> { }
        #endregion

        #region OrderNbr
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr")]
        [PXDBDefault(typeof(RQRequisition.reqNbr))]
        [PXParent(typeof(Select<RQRequisition,   
                                Where<RQRequisition.reqNbr, Equal<Current<KGRequisitionFile.orderNbr>>>>))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region FileType
        [PXDBString(50, IsFixed = true, IsUnicode = true, InputMask = "")]
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

    public class FileType
    {
        public const string Kin0701081652333750000 = "Kin0701081652333750000";
        public const string Kin0701081655535460001 = "Kin0701081655535460001";
        public const string Kin0701081656147500002 = "Kin0701081656147500002";
        public const string Kin0701081656393900003 = "Kin0701081656393900003";
        public const string Kin0701081657128590004 = "Kin0701081657128590004";
        public const string Kin0701081659491710005 = "Kin0701081659491710005";
        public const string Kin0701081703345310006 = "Kin0701081703345310006";
        public const string Kin0703211046242490000 = "Kin0703211046242490000";
        public const string Kin0703221729323300001 = "Kin0703221729323300001";
        public const string Kin0905100000000000000 = "Kin0905100000000000000";
        public const string Kin0905100000000000001 = "Kin0905100000000000001";

        public static readonly string[] Values =
        {
            Kin0701081652333750000,
            Kin0701081655535460001,
            Kin0701081656147500002,
            Kin0701081656393900003,
            Kin0701081657128590004,
            Kin0701081659491710005,
            Kin0701081703345310006,
            Kin0703211046242490000,
            Kin0703221729323300001,
            Kin0905100000000000000,
            Kin0905100000000000001,
        };

        public static readonly string[] Labels =
        {
            "設計圖",
            "空白標單(xls)",
            "施工規範",
            "採購單",
            "分包契約",
            "招標須知",
            "合約附件",
            "特定條款",
            "發包預算書(xls)",
            "空白標單(xml)",
            "發包預算書(xml)"
        };

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }
    }
}