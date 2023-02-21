using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGContractTag : IBqlTable
    {
        //2019/07/26 ADD
        #region Active     
        [PXDBBool]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        public abstract class active : IBqlField { }
        #endregion

        #region ContractCategoryID
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(KGContractCategory.contractCategoryID))]
        [PXParent(typeof(Select<KGContractCategory,
                            Where<KGContractCategory.contractCategoryID,
                              Equal<Current<KGContractTag.contractCategoryID>>>>))]
        [PXUIField(DisplayName = "Contract Category ID")]

        public virtual int? ContractCategoryID { get; set; }
        public abstract class contractCategoryID : IBqlField { }
        #endregion

        #region TagID
        [PXDBIdentity]
        [PXUIField(DisplayName = "Tagid")]
        public virtual int? TagID { get; set; }
        public abstract class tagid : IBqlField { }
        #endregion

        #region Tagcd
        [PXDBString(50, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Tagcd", Required = true)]
        public virtual string Tagcd { get; set; }
        public abstract class tagcd : IBqlField { }
        #endregion

        #region TagContent
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tag Content")]
        public virtual string TagContent { get; set; }
        public abstract class tagContent : IBqlField { }
        #endregion

        #region TagDesc
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tag Desc", Enabled = false)]
        public virtual string TagDesc
        {
            get
            {
                return "##" + this.Tagcd + "##";
            }
        }
        public abstract class tagDesc : IBqlField { }
        #endregion

        #region NoteID
        [PXNote(ForceFileCorrection = false)]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion
    }
}