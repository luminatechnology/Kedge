using System;
using PX.Data;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    [PXCacheName("KGBudApproveName")]
    public class KGBudApproveName : IBqlTable
    {
        #region Branch
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch", IsReadOnly = true)]
        [PXSelector(typeof(Search<Branch.branchID>),
            typeof(Branch.branchCD),
            typeof(Branch.acctName),
            SubstituteKey = typeof(Branch.branchCD),
            DescriptionField = typeof(Branch.acctName)
            )]
        [PXDefault(typeof(AccessInfo.branchID))]
        public virtual int? Branch { get; set; }
        public abstract class branch : PX.Data.BQL.BqlInt.Field<branch> { }
        #endregion

        #region CompanyName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company Name", IsReadOnly =true)]
        [PXDefault(typeof(Search<Branch.acctName,
            Where<Branch.branchID,Equal<Current<KGBudApproveName.branch>>>>))]
        //[PXFormula(typeof(Search<Branch.acctName,
        //    Where<Branch.branchID, Equal<KGBudApproveName.branch>>>))]
        public virtual string CompanyName { get; set; }
        public abstract class companyName : PX.Data.BQL.BqlString.Field<companyName> { }
        #endregion

        #region CompanyType
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company Type",IsReadOnly =true)]
        [PXDefault(typeof(Search<Branch.branchCD,
            Where<Branch.branchID, Equal<Current<KGBudApproveName.branch>>>>))]
        //[PXFormula(typeof(Search<Branch.branchCD,
        //    Where<Branch.branchID, Equal<KGBudApproveName.branch>>>))]
        public virtual string CompanyType { get; set; }
        public abstract class companyType : PX.Data.BQL.BqlString.Field<companyType> { }
        #endregion

        #region Stage1Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage1 Name")]
        public virtual string Stage1Name { get; set; }
        public abstract class stage1Name : PX.Data.BQL.BqlString.Field<stage1Name> { }
        #endregion

        #region Stage2Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage2 Name")]
        public virtual string Stage2Name { get; set; }
        public abstract class stage2Name : PX.Data.BQL.BqlString.Field<stage2Name> { }
        #endregion

        #region Stage3Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage3 Name")]
        public virtual string Stage3Name { get; set; }
        public abstract class stage3Name : PX.Data.BQL.BqlString.Field<stage3Name> { }
        #endregion

        #region Stage4Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage4 Name")]
        public virtual string Stage4Name { get; set; }
        public abstract class stage4Name : PX.Data.BQL.BqlString.Field<stage4Name> { }
        #endregion

        #region Stage5Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage5 Name")]
        public virtual string Stage5Name { get; set; }
        public abstract class stage5Name : PX.Data.BQL.BqlString.Field<stage5Name> { }
        #endregion

        #region Stage6Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage6 Name")]
        public virtual string Stage6Name { get; set; }
        public abstract class stage6Name : PX.Data.BQL.BqlString.Field<stage6Name> { }
        #endregion

        #region Stage7Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage7 Name")]
        public virtual string Stage7Name { get; set; }
        public abstract class stage7Name : PX.Data.BQL.BqlString.Field<stage7Name> { }
        #endregion

        #region Stage8Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage8 Name")]
        public virtual string Stage8Name { get; set; }
        public abstract class stage8Name : PX.Data.BQL.BqlString.Field<stage8Name> { }
        #endregion

        #region Stage9Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage9 Name")]
        public virtual string Stage9Name { get; set; }
        public abstract class stage9Name : PX.Data.BQL.BqlString.Field<stage9Name> { }
        #endregion

        #region Stage10Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage10 Name")]
        public virtual string Stage10Name { get; set; }
        public abstract class stage10Name : PX.Data.BQL.BqlString.Field<stage10Name> { }
        #endregion

        #region Stage11Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage11 Name")]
        public virtual string Stage11Name { get; set; }
        public abstract class stage11Name : PX.Data.BQL.BqlString.Field<stage11Name> { }
        #endregion

        #region Stage12Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage12 Name")]
        public virtual string Stage12Name { get; set; }
        public abstract class stage12Name : PX.Data.BQL.BqlString.Field<stage12Name> { }
        #endregion

        #region Stage13Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage13 Name")]
        public virtual string Stage13Name { get; set; }
        public abstract class stage13Name : PX.Data.BQL.BqlString.Field<stage13Name> { }
        #endregion

        #region Stage14Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage14 Name")]
        public virtual string Stage14Name { get; set; }
        public abstract class stage14Name : PX.Data.BQL.BqlString.Field<stage14Name> { }
        #endregion

        #region Stage15Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage15 Name")]
        public virtual string Stage15Name { get; set; }
        public abstract class stage15Name : PX.Data.BQL.BqlString.Field<stage15Name> { }
        #endregion

        #region Stage16Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage16 Name")]
        public virtual string Stage16Name { get; set; }
        public abstract class stage16Name : PX.Data.BQL.BqlString.Field<stage16Name> { }
        #endregion

        #region Stage17Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage17 Name")]
        public virtual string Stage17Name { get; set; }
        public abstract class stage17Name : PX.Data.BQL.BqlString.Field<stage17Name> { }
        #endregion

        #region Stage18Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage18 Name")]
        public virtual string Stage18Name { get; set; }
        public abstract class stage18Name : PX.Data.BQL.BqlString.Field<stage18Name> { }
        #endregion

        #region Stage19Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage19 Name")]
        public virtual string Stage19Name { get; set; }
        public abstract class stage19Name : PX.Data.BQL.BqlString.Field<stage19Name> { }
        #endregion

        #region Stage20Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Stage20 Name")]
        public virtual string Stage20Name { get; set; }
        public abstract class stage20Name : PX.Data.BQL.BqlString.Field<stage20Name> { }
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