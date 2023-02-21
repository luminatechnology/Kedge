using System;
using PX.Data;
using Kedge;
using PX.Objects.IN;
using static EP.Util.EPStringList;
using PX.Objects.GL;
using OEP =PX.Objects.EP;
using PX.Data.ReferentialIntegrity.Attributes;

namespace Kedge.DAC
{
    /**
     *===2021/04/20 Mantis: 0011858 === Althea
     *Add Field: 
     *1) InventoryID /int /Selector
     *2) CreditAccountID /int /Selector
     *3) CreditSubID /int / Selector
     *4)EPDocType /string(3) / StringList(EPDocType)
     *Add Log:
     *if EPDocType = GLB, CreditAccountID&CreditSubID Enable = true & Required
     * 
     *===2021/08/13 Mantis: 0012198 === Althea
     *Inventory Selector Modify:
     *當EPDocType = 其他申請或傳票作業,
     *InventoryID 可以挑到itemType是費用以外的
     **/
    [Serializable]
    [PXCacheName("KGBudApproveLevel")]
    public class KGBudApproveLevel : IBqlTable
    {
        #region BudLevelID
        [PXDBIdentity(IsKey =true)]
        public virtual int? BudLevelID { get; set; }
        public abstract class budLevelID : PX.Data.BQL.BqlInt.Field<budLevelID> { }
        #endregion

        #region BranchID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch ID")]
        [PXDefault(typeof(AccessInfo.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region BudgetYear
        [PXDBInt(MinValue = 1999, MaxValue = 9999, IsKey = true)]
        [PXUIField(DisplayName = "Budget Year")]
        public virtual int? BudgetYear { get; set; }
        public abstract class budgetYear : PX.Data.BQL.BqlInt.Field<budgetYear> { }
        #endregion

        #region ApprovalLevelID
        [PXDBInt()]
        [PXUIField(DisplayName = "Approval Level ID",Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ApprovalLevelID { get; set; }
        public abstract class approvalLevelID : PX.Data.BQL.BqlInt.Field<approvalLevelID> { }
        #endregion

        #region CompanyCode
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company Code")]
        
        public virtual string CompanyCode { get; set; }
        public abstract class companyCode : PX.Data.BQL.BqlString.Field<companyCode> { }
        #endregion      

        #region GroupNo
        [PXDBInt()]
        [PXUIField(DisplayName = "Group No",Required = true)]
        [PXSelector(typeof(Search<KGBudGroup.budGroupNO,
            Where<KGBudGroup.branchID,
                Equal<Current<AccessInfo.branchID>>>>),
            typeof(KGBudGroup.budGroupNO),
            typeof(KGBudGroup.budGroupName),
            DescriptionField = typeof(KGBudGroup.budGroupName))]

        public virtual int? GroupNo { get; set; }
        public abstract class groupNo : PX.Data.BQL.BqlInt.Field<groupNo> { }
        #endregion

        #region SubGroupNo
        [PXDBInt()]
        [PXUIField(DisplayName = "Sub Group No",Required = true)]
        public virtual int? SubGroupNo { get; set; }
        public abstract class subGroupNo : PX.Data.BQL.BqlInt.Field<subGroupNo> { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region Level1
        [PXDBBool()]
        [PXUIField(DisplayName = "Level1")]
        [PXDefault(false)]
        public virtual bool? Level1 { get; set; }
        public abstract class level1 : PX.Data.BQL.BqlBool.Field<level1> { }
        #endregion

        #region Level2
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level2")]
        public virtual bool? Level2 { get; set; }
        public abstract class level2 : PX.Data.BQL.BqlBool.Field<level2> { }
        #endregion

        #region Level3
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level3")]
        public virtual bool? Level3 { get; set; }
        public abstract class level3 : PX.Data.BQL.BqlBool.Field<level3> { }
        #endregion

        #region Level4
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level4")]
        public virtual bool? Level4 { get; set; }
        public abstract class level4 : PX.Data.BQL.BqlBool.Field<level4> { }
        #endregion

        #region Level5
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level5")]
        public virtual bool? Level5 { get; set; }
        public abstract class level5 : PX.Data.BQL.BqlBool.Field<level5> { }
        #endregion

        #region Level6
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level6")]
        public virtual bool? Level6 { get; set; }
        public abstract class level6 : PX.Data.BQL.BqlBool.Field<level6> { }
        #endregion

        #region Level7
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level7")]
        public virtual bool? Level7 { get; set; }
        public abstract class level7 : PX.Data.BQL.BqlBool.Field<level7> { }
        #endregion

        #region Level8
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level8")]
        public virtual bool? Level8 { get; set; }
        public abstract class level8 : PX.Data.BQL.BqlBool.Field<level8> { }
        #endregion

        #region Level9
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level9")]
        public virtual bool? Level9 { get; set; }
        public abstract class level9 : PX.Data.BQL.BqlBool.Field<level9> { }
        #endregion

        #region Level10
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level10")]
        public virtual bool? Level10 { get; set; }
        public abstract class level10 : PX.Data.BQL.BqlBool.Field<level10> { }
        #endregion

        #region Level11
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level11")]
        public virtual bool? Level11 { get; set; }
        public abstract class level11 : PX.Data.BQL.BqlBool.Field<level11> { }
        #endregion

        #region Level12
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level12")]
        public virtual bool? Level12 { get; set; }
        public abstract class level12 : PX.Data.BQL.BqlBool.Field<level12> { }
        #endregion

        #region Level13
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level13")]
        public virtual bool? Level13 { get; set; }
        public abstract class level13 : PX.Data.BQL.BqlBool.Field<level13> { }
        #endregion

        #region Level14
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level14")]
        public virtual bool? Level14 { get; set; }
        public abstract class level14 : PX.Data.BQL.BqlBool.Field<level14> { }
        #endregion

        #region Level15
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level15")]
        public virtual bool? Level15 { get; set; }
        public abstract class level15 : PX.Data.BQL.BqlBool.Field<level15> { }
        #endregion

        #region Level16
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level16")]
        public virtual bool? Level16 { get; set; }
        public abstract class level16 : PX.Data.BQL.BqlBool.Field<level16> { }
        #endregion

        #region Level17
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level17")]
        public virtual bool? Level17 { get; set; }
        public abstract class level17 : PX.Data.BQL.BqlBool.Field<level17> { }
        #endregion

        #region Level18
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level18")]
        public virtual bool? Level18 { get; set; }
        public abstract class level18 : PX.Data.BQL.BqlBool.Field<level18> { }
        #endregion

        #region Level19
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level19")]
        public virtual bool? Level19 { get; set; }
        public abstract class level19 : PX.Data.BQL.BqlBool.Field<level19> { }
        #endregion

        #region Level20
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Level20")]
        public virtual bool? Level20 { get; set; }
        public abstract class level20 : PX.Data.BQL.BqlBool.Field<level20> { }
        #endregion

        #region Level1Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level1 Amt")]
        public virtual Decimal? Level1Amt { get; set; }
        public abstract class level1Amt : PX.Data.BQL.BqlDecimal.Field<level1Amt> { }
        #endregion

        #region Level2Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level2 Amt")]
        public virtual Decimal? Level2Amt { get; set; }
        public abstract class level2Amt : PX.Data.BQL.BqlDecimal.Field<level2Amt> { }
        #endregion

        #region Level3Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level3 Amt")]
        public virtual Decimal? Level3Amt { get; set; }
        public abstract class level3Amt : PX.Data.BQL.BqlDecimal.Field<level3Amt> { }
        #endregion

        #region Level4Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level4 Amt")]
        public virtual Decimal? Level4Amt { get; set; }
        public abstract class level4Amt : PX.Data.BQL.BqlDecimal.Field<level4Amt> { }
        #endregion

        #region Level5Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level5 Amt")]
        public virtual Decimal? Level5Amt { get; set; }
        public abstract class level5Amt : PX.Data.BQL.BqlDecimal.Field<level5Amt> { }
        #endregion

        #region Level6Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level6 Amt")]
        public virtual Decimal? Level6Amt { get; set; }
        public abstract class level6Amt : PX.Data.BQL.BqlDecimal.Field<level6Amt> { }
        #endregion

        #region Level7Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level7 Amt")]
        public virtual Decimal? Level7Amt { get; set; }
        public abstract class level7Amt : PX.Data.BQL.BqlDecimal.Field<level7Amt> { }
        #endregion

        #region Level8Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level8 Amt")]
        public virtual Decimal? Level8Amt { get; set; }
        public abstract class level8Amt : PX.Data.BQL.BqlDecimal.Field<level8Amt> { }
        #endregion

        #region Level9Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level9 Amt")]
        public virtual Decimal? Level9Amt { get; set; }
        public abstract class level9Amt : PX.Data.BQL.BqlDecimal.Field<level9Amt> { }
        #endregion

        #region Level10Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level10 Amt")]
        public virtual Decimal? Level10Amt { get; set; }
        public abstract class level10Amt : PX.Data.BQL.BqlDecimal.Field<level10Amt> { }
        #endregion

        #region Level11Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level11 Amt")]
        public virtual Decimal? Level11Amt { get; set; }
        public abstract class level11Amt : PX.Data.BQL.BqlDecimal.Field<level11Amt> { }
        #endregion

        #region Level12Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level12 Amt")]
        public virtual Decimal? Level12Amt { get; set; }
        public abstract class level12Amt : PX.Data.BQL.BqlDecimal.Field<level12Amt> { }
        #endregion

        #region Level13Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level13 Amt")]
        public virtual Decimal? Level13Amt { get; set; }
        public abstract class level13Amt : PX.Data.BQL.BqlDecimal.Field<level13Amt> { }
        #endregion

        #region Level14Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level14 Amt")]
        public virtual Decimal? Level14Amt { get; set; }
        public abstract class level14Amt : PX.Data.BQL.BqlDecimal.Field<level14Amt> { }
        #endregion

        #region Level15Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level15 Amt")]
        public virtual Decimal? Level15Amt { get; set; }
        public abstract class level15Amt : PX.Data.BQL.BqlDecimal.Field<level15Amt> { }
        #endregion

        #region Level16Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level16 Amt")]
        public virtual Decimal? Level16Amt { get; set; }
        public abstract class level16Amt : PX.Data.BQL.BqlDecimal.Field<level16Amt> { }
        #endregion

        #region Level17Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level17 Amt")]
        public virtual Decimal? Level17Amt { get; set; }
        public abstract class level17Amt : PX.Data.BQL.BqlDecimal.Field<level17Amt> { }
        #endregion

        #region Level18Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level18 Amt")]
        public virtual Decimal? Level18Amt { get; set; }
        public abstract class level18Amt : PX.Data.BQL.BqlDecimal.Field<level18Amt> { }
        #endregion

        #region Level19Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level19 Amt")]
        public virtual Decimal? Level19Amt { get; set; }
        public abstract class level19Amt : PX.Data.BQL.BqlDecimal.Field<level19Amt> { }
        #endregion

        #region Level20Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Level20 Amt")]
        public virtual Decimal? Level20Amt { get; set; }
        public abstract class level20Amt : PX.Data.BQL.BqlDecimal.Field<level20Amt> { }
        #endregion  


        #region BillingType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Billing Type")]
        [PXStringList(new string[] { "A","P"}, new string[] { "管理", "專案" })]
        public virtual string BillingType { get; set; }
        public abstract class billingType : PX.Data.BQL.BqlString.Field<billingType> { }
        #endregion

        #region BelongSClass
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong SClass")]
        public virtual string BelongSClass { get; set; }
        public abstract class belongSClass : PX.Data.BQL.BqlString.Field<belongSClass> { }
        #endregion

        #region BelongMClass
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong MClass")]
        public virtual string BelongMClass { get; set; }
        public abstract class belongMClass : PX.Data.BQL.BqlString.Field<belongMClass> { }
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

        //2021/04/20 Add 
        #region InventoryID
        [PXDefault]
        [Inventory(DisplayName = "Expense Item", Required = true)]
        //2021/08/13 Add Mantis: 0012198 
        [PXRestrictor(typeof(Where<Where<Current<ePDocType>, In3<EPDocType.glb, EPDocType.oth>,
            Or<InventoryItem.itemType, Equal<INItemTypes.expenseItem>>>>),
            OEP.Messages.InventoryItemIsNotAnExpenseType)]
        [PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region EPDocType
        [PXDBString( IsUnicode = true)]
        [PXUIField(DisplayName = "EPDocType" , Required =true)]
        [EPDocType]
        public virtual string EPDocType { get; set; }
        public abstract class ePDocType : PX.Data.BQL.BqlString.Field<ePDocType> { }
        #endregion

        #region CreditAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "CreditAccountID")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active,Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIRequired(typeof(Where<ePDocType,Equal<EPDocType.glb>>))]
        [PXUIEnabled(typeof(Where<ePDocType,Equal<EPDocType.glb>>))]
        public virtual int? CreditAccountID { get; set; }
        public abstract class creditAccountID : PX.Data.BQL.BqlInt.Field<creditAccountID> { }
        #endregion

        #region CreditSubID
        [PXDBInt()]
        [PXUIField(DisplayName = "CreditSubID")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active,Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIRequired(typeof(Where<ePDocType, Equal<EPDocType.glb>>))]
        [PXUIEnabled(typeof(Where<ePDocType, Equal<EPDocType.glb>>))]
        public virtual int? CreditSubID { get; set; }
        public abstract class creditSubID : PX.Data.BQL.BqlInt.Field<creditSubID> { }
        #endregion

        //2021/07/09 Add 
        #region LevelDesc
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Level Desc")]

        public virtual string LevelDesc { get; set; }
        public abstract class levelDesc : PX.Data.BQL.BqlString.Field<levelDesc> { }
        #endregion      
    }
}