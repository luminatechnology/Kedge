using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGContractCategory : IBqlTable
    {

        #region ContractCategoryID
        [PXDBIdentity()]
        public virtual int? ContractCategoryID { get; set; }
        public abstract class contractCategoryID : IBqlField { }
        #endregion

        #region ContractCategoryCD
        [PXDBString(50, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Contract Category CD", Required = true)]
        [PXSelector(typeof(KGContractCategory.contractCategoryCD),
                    typeof(KGContractCategory.contractCategoryCD))]
        public virtual string ContractCategoryCD { get; set; }
        public abstract class contractCategoryCD : IBqlField { }
        #endregion

        #region ContractType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contract Type", Required = true)]
        [PXDefault()]
        [PXStringList(
          new string[]
          {
      "S",
      "C"
          },
          new string[]
          {
      "簡約",
      "合約"
        })]
        public virtual string ContractType { get; set; }
        public abstract class contractType : IBqlField { }
        #endregion

        #region ContractDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contract Desc")]
        public virtual string ContractDesc { get; set; }
        public abstract class contractDesc : IBqlField { }
        #endregion

        #region NoteID
        [PXNote()]
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

        //2020/03/12 ADD 加上提醒欄位 提醒上傳word檔案格式為.docx
        #region Remind
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "提醒",IsReadOnly =true)]
        [PXDefault("上傳Word範本只支援 .docx檔案格式")]
        public virtual string Remind { get; set; }
        public abstract class remind : IBqlField { }
        #endregion
    }
}