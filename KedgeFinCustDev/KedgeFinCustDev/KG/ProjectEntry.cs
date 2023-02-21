using System;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Objects.CS;
using System.Collections;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.Common;
using PX.SM;
using PX.Objects.CA;
using System.Linq;
using System.Diagnostics;
using PX.Api;
using PX.Data.DependencyInjection;
//using PX.LicensePolicy;
using PX.Objects.GL.FinPeriods;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects;
using PX.Objects.PM;
using Kedge.DAC;
using KG.Util;


namespace PX.Objects.PM
{
    public class ProjectEntry_Extension : PXGraphExtension<ProjectEntry>
    {
        #region Select
        public PXSelect<KGProjectStage,
            Where<KGProjectStage.projectID, Equal<Current<PMProject.contractID>>>> ProjectStages;
        public PXSelect<KGProjectRenterMaterial,
            Where<KGProjectRenterMaterial.projectStageID, Equal<Current<KGProjectStage.projectStageID>>>> ProjectRenterMaterials;

        //Revenue
        public PXSelect<KGPmBudgetModR,
            Where<KGPmBudgetModR.projectID, Equal<Current<PMProject.contractID>>,
                And<KGPmBudgetModR.modifyClass, Equal<KGConst.revenue>>>> BudgetModRs;

        //Cost
        public PXSelect<KGPmBudgetModC,
            Where<KGPmBudgetModC.projectID, Equal<Current<PMProject.contractID>>,
                And<KGPmBudgetModC.modifyClass, Equal<KGConst.cost>>>> BudgetModCs;
        
        #endregion

        protected IEnumerable projectStages()
        {
            KGProjectStage current = 
                (KGProjectStage)PXSelect<KGProjectStage,
                Where<KGProjectStage.projectID,Equal<Current<PMProject.contractID>>>>
                .SelectWindowed(Base, 0, 1)?? ProjectStages.Insert();
            yield return current;
        }

        #region Event Handlers
        protected virtual void PMProject_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PMProject row = (PMProject)e.Row;
            if (row == null) return;
            Contract contract = PXSelect<Contract,
                Where<Contract.contractID, Equal<Required<PMProject.contractID>>>>
                .Select(Base, row.ContractID);
            ContractFinExt contractExt = PXCache<Contract>.GetExtension<ContractFinExt>(contract);

            decimal? UsrModRevenueTotal=0;
            decimal? UsrModCostTotal = 0;

            PXResultset<KGPmBudgetModR> ModRset =
                PXSelect<KGPmBudgetModR,
                Where<KGPmBudgetModR.projectID, Equal<Required<PMProject.contractID>>,
                And < KGPmBudgetModR.modifyClass, Equal < KGConst.revenue >>>>
                .Select(Base, row.ContractID);
            if(ModRset!=null)
            {
                foreach (KGPmBudgetModR getRSum in ModRset)
                {
                    UsrModRevenueTotal = UsrModRevenueTotal + getRSum.Amount;
                }
            }
            PXResultset<KGPmBudgetModC> ModCset =
                PXSelect<KGPmBudgetModC,
                Where<KGPmBudgetModC.projectID, Equal<Required<PMProject.contractID>>,
                And < KGPmBudgetModC.modifyClass, Equal < KGConst.cost >>>>
                .Select(Base, row.ContractID);
            if (ModCset != null)
            {
                foreach (KGPmBudgetModC getCSum in ModCset)
                {
                    UsrModCostTotal = UsrModCostTotal + getCSum.Amount;
                }
            }


            contractExt.UsrModRevenueTotal = UsrModRevenueTotal;
            contractExt.UsrModCostTotal = UsrModCostTotal;
            contractExt.UsrFinalRevenue = (contractExt.UsrOriRevenue??0 )+ UsrModRevenueTotal;
            contractExt.UsrFinalCost = (contractExt.UsrOriCost??0 )+ UsrModCostTotal;

        }

        

        protected virtual void PMProject_UsrOriCost_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            Contract contract = (Contract)e.Row;
            ContractFinExt contractExt = PXCache<Contract>.GetExtension<ContractFinExt>(contract);
            if (contractExt == null) return;
            //sender.SetDefaultExt<ContractFinExt.usrFinalCost>(contract);
            
            if (contractExt.UsrOriCost != null)
            {
                contractExt.UsrFinalCost = (contractExt.UsrOriCost ?? 0 )+ contractExt.UsrModCostTotal;

            }
        }
        

        protected virtual void PMProject_UsrOriRevenue_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            Contract contract = (Contract)e.Row;
            ContractFinExt contractExt = PXCache<Contract>.GetExtension<ContractFinExt>(contract);
            if (contractExt == null) return;
            
            if (contractExt.UsrOriRevenue != null)
            {
                contractExt.UsrFinalRevenue = (contractExt.UsrOriRevenue??0) + contractExt.UsrModRevenueTotal;

            }


        }

        //20220815 by louis 新增異動是否結算時, 自動更新結算日期
        protected virtual void PMProject_UsrIsSettled_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            Contract contract = (Contract)e.Row;
            ContractFinExt contractExt = PXCache<Contract>.GetExtension<ContractFinExt>(contract);
            if (contractExt == null) return;

            if (contractExt.UsrIsSettled != null && contractExt.UsrIsSettled == true)
            {
                contractExt.UsrFinSettleDate = Base.Accessinfo.BusinessDate;
            }
            else {
                contractExt.UsrFinSettleDate = null;
            }
        }

        protected virtual void KGProjectStage_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGProjectStage row = (KGProjectStage)e.Row;
            if (row == null) return;
            if (row.StageOneDate != null)
            {
                if (row.StageTwoDate != null)
                {
                    if (row.StageTwoDate < row.StageOneDate)
                        throw new Exception("請檢查日期,階段二的日期須大於階段一的日期!");

                    if (row.StageThreeDate != null)
                    {
                        if (row.StageThreeDate < row.StageTwoDate)
                            throw new Exception("請檢查日期,階段三的日期須大於階段二的日期!");
                    }
                }
            }
            if (row.StageOneDate == null && row.StageTwoDate != null)
            {
                throw new Exception("請填第一階段的日期!");
            }
            if (row.StageTwoDate == null && row.StageThreeDate != null)
            {
                throw new Exception("請填第二階段的日期!");
            }
        }
        /*
        protected virtual void KGProjectRenterMaterial_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGProjectRenterMaterial details = (KGProjectRenterMaterial)e.Row;
            if (details == null) return;
            /*checkInventoryID(sender, details.InventoryID);
            if(hasError)
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            }
        }*/
        #endregion
        /*protected virtual void KGProjectRenterMaterial_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGProjectRenterMaterial row = (KGProjectRenterMaterial)e.Row;
            if (row == null) return;
            checkInventoryID(sender, row.InventoryID);
        }*/
        #region Method
        //bool hasError = false;
        /*public bool checkInventoryID(PXCache sender,int? NewInventoryID)
        {
            KGProjectStage stage = ProjectStages.Current;
            KGProjectRenterMaterial row =ProjectRenterMaterials.Current;
            foreach (KGProjectRenterMaterial details in ProjectRenterMaterials.Cache.Cached)
            {

                if (details.InventoryID == null || NewInventoryID ==null)
                {
                    continue;
                }
                if (details.InventoryID.Equals(NewInventoryID))
                {
                    hasError = true;
                    sender.RaiseExceptionHandling<KGProjectRenterMaterial.inventoryID>(
                                        row, NewInventoryID,
                                                new PXSetPropertyException("工料編號不可重複!", PXErrorLevel.Error));
                    return true;
                }
            }
            return false;
        }*/
        #endregion
    }
}