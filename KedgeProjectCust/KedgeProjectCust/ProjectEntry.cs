using System;
using System.Collections;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.IN;

namespace PX.Objects.PM
{
  public class ProjectEntry_Extension : PXGraphExtension<ProjectEntry>
  {
        #region Override DAC - CacheAttached

        #region PMProject - CustomerID 1785: PM301000專案, 請將PMProject.CustomerID調為必填
        //1785: PM301000專案, 請將PMProject.CustomerID調為必填
        [CustomerActive(DescriptionField = typeof(Customer.acctName),Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        protected virtual void PMProject_CustomerID_CacheAttached(PXCache sender){}
        #endregion

        #endregion

        #region Delegate Buttons
        /// <summary>
        /// Since "ProjectEntry" extension can't read the following action, a redefined method is used to satisfy the requirement.
        /// </summary>
        public PXAction<PMProject> viewCostCommitments;
        [PXUIField(DisplayName = Messages.ViewCommitments, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
        public IEnumerable ViewCostCommitments(PXAdapter adapter)
        {
            var curCostBudget = Base.CostBudget.Current;

            if (curCostBudget != null)
            {
                CommitmentInquiry graph = PXGraph.CreateInstance<CommitmentInquiry>();

                graph.Filter.Current.AccountGroupID = curCostBudget.AccountGroupID;
                graph.Filter.Current.ProjectID      = curCostBudget.ProjectID;
                graph.Filter.Current.ProjectTaskID  = curCostBudget.ProjectTaskID;
                graph.Filter.Current.CostCode       = curCostBudget.CostCodeID;
                // Pass the cursor InventoryID to the popup form. on 2023/2/2
                graph.Filter.Current.InventoryID    = curCostBudget.InventoryID;

                throw new PXPopupRedirectException(graph, Messages.CommitmentEntry + " - " + Messages.ViewCommitments, true);
            }

            return Base.viewCostCommitments.Press(adapter);
        }
        #endregion

        #region Actions
        public PXAction<PMProject> addItem;
        [PXUIField(DisplayName = "Re Calculate Schedule", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddItem(PXAdapter adapter)
        {
            //throw new PXException("1");
            foreach (PMTask task in Base.Tasks.Select()) {
                if (task.EndDate != null && task.StartDate != null & ProjectTaskStatus.Active.Equals(task.Status)) {
                    PMTaskExt pmTaskrExt = PXCache<PMTask>.GetExtension<PMTaskExt>(task);
                    DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    Decimal workToToday = toDays(today, task.StartDate);
                    Decimal workToEndDay = toDays(task.EndDate, task.StartDate);
                    Decimal percent = (workToToday / workToEndDay) * 100;
                    if (percent > 100) {
                        percent = 100;
                    } else if (percent < 0) {
                        percent = 0;
                    }
                    pmTaskrExt.UsrSchConstructedPct = percent;
                    Base.Tasks.Cache.Update(task);
                }    
            }

            return adapter.Get();
        }
        #endregion

        public int toDays(DateTime? endDate, DateTime? startDate)
        {
            int day = ((TimeSpan)(endDate - startDate)).Days + 1;
            return day;
        }

        /*
        public PXSelect<PMCostBudget, Where<PMCostBudget.projectID, Equal<Current<PMProject.contractID>>>,
        OrderBy<Asc<PMCostBudget.sortOrder,
        Asc<PMCostBudget.projectID, 
        Asc<PMCostBudget.projectTaskID, 
        Asc<PMCostBudget.inventoryID, 
        Asc<PMCostBudget.costCodeID, 
        Asc<PMCostBudget.accountGroupID>>>>>>>> CostBudget;

        // The following data view declaration will affect standard data view source.
        //public CRAttributeList<InventoryItem> Answers;

        //588
        public virtual IEnumerable costBudget()
        {
            var selectCostBudget = new PXSelect<PMCostBudget, Where<PMCostBudget.projectID, Equal<Current<PMProject.contractID>>, And<PMCostBudget.type, Equal<GL.AccountType.expense>,
              And<Where<Current<ProjectEntry.CostBudgetFilter.projectTaskID>, IsNull, Or<Current<ProjectEntry.CostBudgetFilter.projectTaskID>, Equal<PMCostBudget.projectTaskID>>>>>>,
              OrderBy<Asc<PMCostBudget.projectID, Asc<PMCostBudget.projectTaskID, Asc<PMCostBudget.inventoryID, Asc<PMCostBudget.costCodeID, Asc<PMCostBudget.accountGroupID>>>>>>>(Base);

            PXDelegateResult delResult = new PXDelegateResult();
            delResult.Capacity = 202;
            delResult.IsResultFiltered = false;
            delResult.IsResultSorted = true;
            delResult.IsResultTruncated = false;

            //base.CostBudgetFilter.Current;
            CostBudget.AllowUpdate = true;
            if (IsGroupByCostCode() && !Base.IsCopyPaste)
            {
                //var list = new List<PMCostBudget>(selectCostBudget.Select().RowCast<PMCostBudget>());
                //throw new PXException("1");
                //return AggregateBudget<PMCostBudget>(list);
                //CostBudget.AllowUpdate = false;
                var list = new List<PMCostBudget>(selectCostBudget.Select().RowCast<PMCostBudget>());

                delResult.AddRange(AggregateBudget2<PMCostBudget>(list));

                return delResult;
            }
            else
            {
                return Base.costBudget();
            }

        }*/

        public virtual bool IsGroupByCostCode()
        {       
            CostBudgetFilterExt costBudgetFilterExt = PXCache<ProjectEntry.CostBudgetFilter>.GetExtension<CostBudgetFilterExt>(Base.CostFilter.Current);
            if (Base.CostFilter.Current != null && costBudgetFilterExt.UsrGroupByCostCode == true)
                return true;
    
            return false;
        }

        #region Event Handlers
        /// <summary>
        /// Set the following DAC field visibility to true according to YJ requirements.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="e"></param>
        protected void PMProject_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var row = (PMProject)e.Row;

            PXUIFieldAttribute.SetVisible<PMRevenueBudget.costCodeID>(Base.RevenueBudget.Cache, null, true);
            //PXUIFieldAttribute.SetVisibility<PMRevenueBudget.costCodeID>(Base.RevenueBudget.Cache, null, PXUIVisibility.Visible);
            PXUIFieldAttribute.SetVisible<PMCostBudget.inventoryID>(Base.CostBudget.Cache, null, true);
            PXUIFieldAttribute.SetVisible<PMCostBudget.costCodeID>(Base.CostBudget.Cache, null, true);
        }

        protected void CostBudgetFilter_GroupByTask_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
        {
            var row = (ProjectEntry.CostBudgetFilter)e.Row;
            cache.SetValue<CostBudgetFilterExt.usrGroupByCostCode>(e.Row, null);
        }

        protected void CostBudgetFilter_UsrGroupByCostCode_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
        {
            var row = (ProjectEntry.CostBudgetFilter)e.Row;
            cache.SetValue<ProjectEntry.CostBudgetFilter.groupByTask>(e.Row, null);
        }

        protected void PMCostBudget_InventoryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {    
            var row = (PMCostBudget)e.Row;
            PXResultset<InventoryItem> inventoryItem=
                           PXSelect<InventoryItem,
                           Where<InventoryItem.inventoryID,
                           Equal<Required<InventoryItem.inventoryID>>>>
                           .Select(Base, row.InventoryID);
            foreach (InventoryItem desc in inventoryItem)
            {
                cache.SetValue<PMBudgetExt.usrInventoryDesc>(e.Row, desc.Descr);
        
                PXResultset<CSAnswers> cSAnswerList = PXSelect<CSAnswers, 
                      Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>.Select(Base,desc.NoteID);
                foreach (CSAnswers newAnswer in cSAnswerList)
                {
                    cache.SetValue<PMBudgetExt.usrInvPrType>(e.Row, newAnswer.Value);
                }
            }
            //Base.CostBudget.Cache.Update(row);
        }
      
        //有可能影響效能20190722 By Jerry
        /*
        public virtual List<PMBudget> AggregateBudget2<T>(IList<T> list)
            where T : PMBudget, new()
        {
            Dictionary<int, PMBudget> aggregates = new Dictionary<int, PMBudget>();

            T total = new T();
            total.ProjectID = Base.Project.Current.ContractID;
            total.ProjectTaskID = null;
            total.AccountGroupID = null;
            total.CostCodeID = -CostCodeAttribute.GetDefaultCostCode();
            total.InventoryID = -PMInventorySelectorAttribute.EmptyInventoryID;
            total.Description = Messages.Total;
            total.SortOrder = 1;
            total.UOM = " ";
            foreach (PMBudget budget in list)
            {               
                int key = budget.CostCodeID.Value;
                PMBudget summary = null;
                if (!aggregates.TryGetValue(key, out summary))
                {
                    summary = new T();
                    summary.ProjectID = budget.ProjectID;
                    summary.ProjectTaskID = budget.ProjectTaskID;
                    summary.AccountGroupID = budget.AccountGroupID;
                    //summary.CostCodeID = -budget.CostCodeID;
                    summary.CostCodeID = budget.CostCodeID;
                    summary.InventoryID = -budget.InventoryID;
                    PMTask summaryTask = Base.Tasks.Select().Where(task => task.GetItem<PMTask>().TaskID == budget.ProjectTaskID).FirstOrDefault<PXResult<PMTask>>();
                    summary.Description = summaryTask.Description;
                    
                    if (summary.UOM == null) {
                        summary.Qty = 0;
                        summary.RevisedQty = 0;
                    }

                    aggregates.Add(key, summary);
                }

                Base.AddToSummary(summary, budget);
                Base.AddToSummary(total, budget);
            }
            List<PMBudget> result = new List<PMBudget>();
            result.AddRange(aggregates.Values);
            result.Add(total);

            return result;
        }  */
        #endregion
    }
}