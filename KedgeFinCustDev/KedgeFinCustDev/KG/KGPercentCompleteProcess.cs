using System;
using PX.Objects;
using PX.Data;
using Kedge.DAC;
using PX.Data.BQL;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.GL.FinPeriods.TableDefinition;
using System.Collections.Generic;
using PX.Objects.CA;
using Branch = PX.Data.PXAccess.Branch;
using KG.Util;

namespace Kedge
{
    public class KGPercentCompleteProcess_Extension : PXGraphExtension<KGPercentCompleteProcess>
    {
        #region Event Handlers

        #endregion

        #region Method
        [PXOverride]
        public virtual DateTime? getFinStartDate(int? ProjectID)
        {
            ContractFinExt contractExt = GetContractExt(ProjectID);
            return contractExt?.UsrFinStartDate;
        }


        [PXOverride]
        public virtual decimal? getFinModsAmt(string modstype, int? projectID, DateTime? date)
        {
            decimal? FinModsAmt = null;
            PXResultset<KGPmBudgetMod> set = PXSelect<KGPmBudgetMod,
                Where<KGPmBudgetMod.projectID, Equal<Required<KGPmBudgetMod.projectID>>,
                And<KGPmBudgetMod.approvedDate, LessEqual<Required<KGPmBudgetMod.approvedDate>>,
                And<KGPmBudgetMod.modifyClass, Equal<Required<KGPmBudgetMod.modifyClass>>>>>>
                .Select(Base, projectID, date, modstype);
            foreach (KGPmBudgetMod budgetMod in set)
            {
                FinModsAmt = (FinModsAmt ?? 0) + budgetMod.Amount;
            }

            return FinModsAmt;
        }


        [PXOverride]
        public virtual decimal GetTotalCuryAmount(int contractID, string type)
        {
            ContractFinExt contractExt = GetContractExt(contractID);
            if ("I" == type) return contractExt?.UsrOriRevenue ?? 0;
            else if ("E" == type) return contractExt?.UsrOriCost ?? 0;
            return 0;
        }

        private ContractFinExt GetContractExt(int? ProjectID)
        {
            Contract contract = PXSelect<Contract,
                Where<Contract.contractID, Equal<Required<Contract.contractID>>>>
                .Select(Base, ProjectID);

            return PXCache<Contract>.GetExtension<ContractFinExt>(contract);
        }

        [PXOverride]
        public virtual bool ValidatorData(PXResult<GLTran, Branch, Contract, Account, BAccount> data)
        {
            //2022-11-30 alton user不想看到已結算專案
            Contract contract = data;
            if (contract.GetExtension<ContractFinExt>().UsrIsSettled == true) {
                return false;
            }
            return true;
        }

        #endregion
    }
}