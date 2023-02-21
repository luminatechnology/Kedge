using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.GL;
using System.Collections;
using static EP.Util.EPStringList;

namespace Kedge
{
    /**
     * ===2021/04/20 Mantis: 0011858 === Althea
     * Add Log :
     * if EPDocType != GLB, CreditAccountID & CreditSubID = null
     * 
     * 
     **/
    public class KGBudApproveLevelEntry : PXGraph<KGBudApproveLevelEntry>
    {
        #region View
        public PXSave<KGBudApproveLevel> Save;
        public PXCancel<KGBudApproveLevel> Cancel;

        public PXFilter<KGBudApprovelLevelFilter> Filters;

        [PXImport(typeof(KGBudApproveLevel))]
        public PXSelect<KGBudApproveLevel,
            Where<KGBudApproveLevel.budgetYear,Equal<Current<KGBudApprovelLevelFilter.budgetYear>>>,
            OrderBy<Asc<KGBudApproveLevel.approvalLevelID>>
            > BudApproveLevels;
        #endregion

        #region Action
        #region 複製到下個年度
        public PXAction<KGBudApproveLevel> CopyToNextYear;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "複製到下個年度")]
        protected IEnumerable copyToNextYear(PXAdapter adapter)
        {
            KGBudApprovelLevelFilter filter = Filters.Current;
            //先檢查下個年度是否有資料 若有則跳錯誤
            CheckNextYearisExsit();
            PXLongOperation.StartOperation(this, CopytoNextYearMethod);
            return adapter.Get();
        }
        #endregion

        #endregion

        #region Event
        protected virtual void _(Events.FieldDefaulting<KGBudApprovelLevelFilter, KGBudApprovelLevelFilter.budgetYear> e)
        {
            KGBudApprovelLevelFilter row = (KGBudApprovelLevelFilter)e.Row;
            e.NewValue= this.Accessinfo.BusinessDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<KGBudApproveLevel, KGBudApproveLevel.budgetYear> e)
        {
            KGBudApproveLevel row = (KGBudApproveLevel)e.Row;
            KGBudApprovelLevelFilter filter = Filters.Current;
            e.NewValue = filter.BudgetYear;
        }

        protected virtual void _(Events.FieldDefaulting<KGBudApproveLevel, KGBudApproveLevel.companyCode> e)
        {
            KGBudApproveLevel row = (KGBudApproveLevel)e.Row;
            //e.NewValue = this.Accessinfo.CompanyName;
            Branch branch = PXSelect<Branch,
            Where<Branch.branchID, Equal<Required<Branch.branchID>>>>
            .Select(this, this.Accessinfo.BranchID);
            e.NewValue = branch?.BranchCD;
        }

        /*
        protected virtual void _(Events.FieldUpdated<KGBudApproveLevel, KGBudApproveLevel.subGroupNo> e)
        {
            KGBudApproveLevel row = (KGBudApproveLevel)e.Row;
            if (row == null) return;
            foreach(KGBudApproveLevel level in BudApproveLevels.Cache.Cached)
            {
                if (level.BudgetYear == row.BudgetYear &&
                    level.GroupNo == row.GroupNo &&
                    level.SubGroupNo == (int?)e.NewValue &&
                    level.BudLevelID != row.BudLevelID)
                {
                    throw new Exception("同一個GroupNo不能重複!");
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<KGBudApproveLevel, KGBudApproveLevel.approvalLevelID> e)
        {
            KGBudApproveLevel row = (KGBudApproveLevel)e.Row;
            if (row == null) return;
            foreach (KGBudApproveLevel level in BudApproveLevels.Cache.Cached)
            {
                if (row.BudLevelID == level.BudLevelID) continue; 
                if (level.BudgetYear == row.BudgetYear &&
                    level.BranchID == row.BranchID &&
                    level.ApprovalLevelID == (int?)e.NewValue)
                {
                    throw new Exception("同一個ApprovalLevelID不能重複!");
                }
            }
        }
        */
        protected virtual void _(Events.RowSelected<KGBudApprovelLevelFilter> e)
        {
            KGBudApprovelLevelFilter filter = (KGBudApprovelLevelFilter)e.Row;
            if (filter == null) return;
            //KGBudApprovelLevelFilter filter = Filters.Current;
            if(filter!= null)
            {
                KGBudApproveName approveName = PXSelect<KGBudApproveName,
                    Where<KGBudApproveName.branch, Equal<Required<KGBudApprovelLevelFilter.branch>>>>
                    .Select(this, filter.Branch);
                if(approveName != null)
                {
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level1>
                        (BudApproveLevels.Cache,"第一關-"+(approveName.Stage1Name??""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level2>
                        (BudApproveLevels.Cache, "第二關-" + (approveName.Stage2Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level3>
                        (BudApproveLevels.Cache, "第三關-" + (approveName.Stage3Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level4>
                        (BudApproveLevels.Cache, "第四關-" + (approveName.Stage4Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level5>
                        (BudApproveLevels.Cache, "第五關-" + (approveName.Stage5Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level6>
                       (BudApproveLevels.Cache, "第六關-" + (approveName.Stage6Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level7>
                        (BudApproveLevels.Cache, "第七關-" + (approveName.Stage7Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level8>
                        (BudApproveLevels.Cache, "第八關-" + (approveName.Stage8Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level9>
                        (BudApproveLevels.Cache, "第九關-" + (approveName.Stage9Name ?? ""));
                     PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level10>
                        (BudApproveLevels.Cache, "第十關-" + (approveName.Stage10Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level11>
                        (BudApproveLevels.Cache, "第十一關-" + (approveName.Stage11Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level12>
                        (BudApproveLevels.Cache, "第十二關-" + (approveName.Stage12Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level13>
                        (BudApproveLevels.Cache, "第十三關-" + (approveName.Stage13Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level14>
                        (BudApproveLevels.Cache, "第十四關-" + (approveName.Stage14Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level15>
                        (BudApproveLevels.Cache, "第十五關-" + (approveName.Stage15Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level16>
                       (BudApproveLevels.Cache, "第十六關-" + (approveName.Stage16Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level17>                 
                        (BudApproveLevels.Cache, "第十七關-" + (approveName.Stage17Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level18>                 
                        (BudApproveLevels.Cache, "第十八關-" + (approveName.Stage18Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level19>
                        (BudApproveLevels.Cache, "第十九關-" + (approveName.Stage19Name ?? ""));
                    PXUIFieldAttribute.SetDisplayName<KGBudApproveLevel.level20>                
                       (BudApproveLevels.Cache, "第二十關-" + (approveName.Stage20Name ?? ""));
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<KGBudApproveLevel.ePDocType> e)
        {
            KGBudApproveLevel row = e.Row as KGBudApproveLevel;
            if (row == null) return;
            if(row.EPDocType != EPDocType.GLB || row.EPDocType == null)
            {
                row.CreditAccountID = null;
                row.CreditSubID = null;
            }
        }

        protected virtual void _(Events.RowPersisting<KGBudApproveLevel> e)
        {
            KGBudApproveLevel row = e.Row;
            KGBudApprovelLevelFilter filter = Filters.Current;
            if (row == null) return;
            PXResultset<KGBudApproveLevel> levelset =
                PXSelect<KGBudApproveLevel,
                Where<KGBudApproveLevel.budgetYear, Equal<Required<KGBudApproveLevel.budgetYear>>,
                And<KGBudApproveLevel.branchID, Equal<Required<KGBudApproveLevel.branchID>>>>>
                .Select(this,filter.BudgetYear,filter.Branch);
            foreach (KGBudApproveLevel level in levelset)
            {
                if (level.BudgetYear == row.BudgetYear &&
                    level.GroupNo == row.GroupNo &&
                    level.SubGroupNo == row.SubGroupNo &&
                    level.BudLevelID != row.BudLevelID)
                {
                    BudApproveLevels.Cache.RaiseExceptionHandling<KGBudApproveLevel.subGroupNo>(row, row.SubGroupNo,
                        new PXSetPropertyException("同一個GroupNo不能重複!"));
                }
            }
            foreach (KGBudApproveLevel level in levelset)
            {
                if (row.BudLevelID == level.BudLevelID) continue;
                if (level.BudgetYear == row.BudgetYear &&
                    level.BranchID == row.BranchID &&
                    level.ApprovalLevelID == row.ApprovalLevelID)
                {
                    BudApproveLevels.Cache.RaiseExceptionHandling<KGBudApproveLevel.approvalLevelID>(row, row.ApprovalLevelID,
                        new PXSetPropertyException("同一個ApprovalLevelID不能重複!"));
                }
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 檢查是否有下個年度的資料存在
        /// </summary>
        private void CheckNextYearisExsit()
        {
            KGBudApprovelLevelFilter filter = Filters.Current;
            if (filter.BudgetYear == null) throw new Exception("請填寫年度!");
            int? budgetyear = filter.BudgetYear+ 1;
            if (budgetyear != 1)
            {
                KGBudApproveLevel approveLevel = GetApproveLevel(budgetyear);
                if(approveLevel != null)
                {
                    throw new Exception("下年度資料已經存在");
                }
            }
        }
        private void CopytoNextYearMethod()
        {
            this.Save.Press();
            KGBudApprovelLevelFilter filter = Filters.Current;
            int? budgetyear = filter.BudgetYear+ 1;
            PXResultset<KGBudApproveLevel> approveLevelset =
                PXSelect<KGBudApproveLevel,
                Where<KGBudApproveLevel.budgetYear, Equal<Required<KGBudApproveLevel.budgetYear>>>>
                .Select(this, filter.BudgetYear);
            foreach (KGBudApproveLevel approveLevel in approveLevelset)
            {
                KGBudApproveLevel nextyearlevel = new KGBudApproveLevel();
                nextyearlevel.BudgetYear = budgetyear;
                nextyearlevel= BudApproveLevels.Insert(nextyearlevel);
                nextyearlevel.BranchID = approveLevel.BranchID;
                nextyearlevel.CompanyCode = approveLevel.CompanyCode;
                nextyearlevel.GroupNo = approveLevel.GroupNo;
                nextyearlevel.Remark = approveLevel.Remark;
                nextyearlevel.Level1 = approveLevel.Level1;
                nextyearlevel.Level1Amt = approveLevel.Level1Amt;
                nextyearlevel.Level2 = approveLevel.Level2;
                nextyearlevel.Level2Amt = approveLevel.Level2Amt;
                nextyearlevel.Level3 = approveLevel.Level3;
                nextyearlevel.Level3Amt = approveLevel.Level3Amt;
                nextyearlevel.Level4 = approveLevel.Level4;
                nextyearlevel.Level4Amt = approveLevel.Level4Amt;
                nextyearlevel.Level5 = approveLevel.Level5;
                nextyearlevel.Level5Amt = approveLevel.Level5Amt;
                nextyearlevel.Level6 = approveLevel.Level6;
                nextyearlevel.Level6Amt = approveLevel.Level6Amt;
                nextyearlevel.Level7 = approveLevel.Level7;
                nextyearlevel.Level7Amt = approveLevel.Level7Amt;
                nextyearlevel.Level8 = approveLevel.Level8;
                nextyearlevel.Level8Amt = approveLevel.Level8Amt;
                nextyearlevel.Level9 = approveLevel.Level9;
                nextyearlevel.Level9Amt = approveLevel.Level9Amt;
                nextyearlevel.Level10 = approveLevel.Level10;
                nextyearlevel.Level10Amt = approveLevel.Level10Amt;
                nextyearlevel.Level11 = approveLevel.Level11;
                nextyearlevel.Level11Amt = approveLevel.Level11Amt;
                nextyearlevel.Level12 = approveLevel.Level12;
                nextyearlevel.Level12Amt = approveLevel.Level12Amt;
                nextyearlevel.Level13 = approveLevel.Level13;
                nextyearlevel.Level13Amt = approveLevel.Level13Amt;
                nextyearlevel.Level14 = approveLevel.Level14;
                nextyearlevel.Level14Amt = approveLevel.Level14Amt;
                nextyearlevel.Level15 = approveLevel.Level15;
                nextyearlevel.Level15Amt = approveLevel.Level15Amt;
                nextyearlevel.Level16 = approveLevel.Level16;
                nextyearlevel.Level16Amt = approveLevel.Level16Amt;
                nextyearlevel.Level17 = approveLevel.Level17;
                nextyearlevel.Level17Amt = approveLevel.Level17Amt;
                nextyearlevel.Level18 = approveLevel.Level18;
                nextyearlevel.Level18Amt = approveLevel.Level18Amt;
                nextyearlevel.Level19 = approveLevel.Level19;
                nextyearlevel.Level19Amt = approveLevel.Level19Amt;
                nextyearlevel.Level20 = approveLevel.Level20;
                nextyearlevel.Level20Amt = approveLevel.Level20Amt;
                nextyearlevel.ApprovalLevelID = approveLevel.ApprovalLevelID;
                nextyearlevel.SubGroupNo = approveLevel.SubGroupNo;
                nextyearlevel.BillingType = approveLevel.BillingType;
                nextyearlevel.BelongMClass = approveLevel.BelongMClass;
                nextyearlevel.BelongSClass = approveLevel.BelongSClass;
                nextyearlevel.CreditAccountID = approveLevel.CreditAccountID;
                nextyearlevel.CreditSubID = approveLevel.CreditSubID;
                nextyearlevel.InventoryID = approveLevel.InventoryID;
                nextyearlevel.EPDocType = approveLevel.EPDocType;
                nextyearlevel = BudApproveLevels.Update(nextyearlevel);
            }
            Save.Press();
            filter.BudgetYear = budgetyear;
            //Filters.Update(filter);
            Filters.Current = filter;
        }
        #endregion

        #region Select Method
        private KGBudApproveLevel GetApproveLevel(int? BudgetYear)
        {
            return PXSelect<KGBudApproveLevel,
                Where<KGBudApproveLevel.budgetYear, Equal<Required<KGBudApproveLevel.budgetYear>>>>
                .Select(this,BudgetYear);
        }
        #endregion

        #region Filter Table
        [Serializable]
        public class KGBudApprovelLevelFilter : IBqlTable
        {
            #region BudgetYear
            [PXInt(MinValue = 1999, MaxValue = 9999)]
            [PXUIField(DisplayName = "Budget Year")]
            public virtual int? BudgetYear { get; set; }
            public abstract class budgetYear : PX.Data.BQL.BqlInt.Field<budgetYear> { }
            #endregion

            #region Branch
            [PXInt()]
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
        }
        #endregion
    }
}