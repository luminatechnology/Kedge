using System;
using System.Collections;
using System.Collections.Generic;
using Kedge.DAC;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.PM;
using PX.Web.UI;

namespace Kedge
{
    public class KGOwnerRevenueAllocEntry : PXGraph<KGOwnerRevenueAllocEntry>
    {
        public PXSave<FilterTable> Save;
        public PXCancel<FilterTable> Cancel;
        public PXFilter<FilterTable> FilterView;

        public PXSelect<ProjectTask> Tasks;
        public PXSelect<KGOwnerRevenue> OwnerRevenue;
        public PXSelectReadonly<PMTask,
                                Where<PMTask.taskID, Equal<Current<ProjectTask.taskID>>>> CurrentTask;
        [PXImport()]
        public PXSelect<KGOwnerRevenueAllocation,
                        Where<KGOwnerRevenueAllocation.projectID, Equal<Current<FilterTable.projectID>>>> OwnerRevenueAlloc;

        #region Lookup Button
        public PXAction<KGOwnerRevenue> addOwnerReve;
        [PXUIField(DisplayName = "Add Owner Revenue", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton()]
        public virtual IEnumerable AddOwnerReve(PXAdapter adapter)
        {
            try
            {
                if (OwnerRevenue.AskExt() == WebDialogResult.OK)
                {
                    foreach (KGOwnerRevenue revenue in OwnerRevenue.Cache.Updated)
                    {
                        AddOwnReve(revenue);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            OwnerRevenue.Cache.Clear();
            OwnerRevenue.Cache.ClearQueryCache();

            return adapter.Get();
        }

        public PXAction<KGOwnerRevenue> add;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable Add(PXAdapter adapter)
        {
            foreach (KGOwnerRevenue revenue in OwnerRevenue.Cache.Updated)
            {
                AddOwnReve(revenue);
            }

            OwnerRevenue.Cache.Clear();

            return adapter.Get();
        }
        #endregion

        #region Delegate Data View
        protected virtual IEnumerable filterView()
        {
            FilterTable filter = FilterView.Current;

            if (filter != null)
            {
                int startRow  = 0;
                int totalRows = 0;

                PXSelectBase<KGOwnerRevenueAllocation> cmd = new PXSelectGroupBy<KGOwnerRevenueAllocation,
                                                                                 Where<KGOwnerRevenueAllocation.projectID, Equal<Current<FilterTable.projectID>>>,
                                                                                 Aggregate<Sum<KGOwnerRevenueAllocation.amount>>>(this);

                foreach (KGOwnerRevenueAllocation row in cmd.View.Select(null,
                                                                         null,
                                                                         PXView.Searches,
                                                                         OwnerRevenueAlloc.View.GetExternalSorts(),
                                                                         OwnerRevenueAlloc.View.GetExternalDescendings(),
                                                                         OwnerRevenueAlloc.View.GetExternalFilters(),
                                                                         ref startRow,
                                                                         PXView.MaximumRows,
                                                                         ref totalRows))
                {
                    filter.TotalAmtByProject = row.Amount;
                }

                cmd.WhereAnd<Where<KGOwnerRevenueAllocation.taskID, Equal<Current<ProjectTask.taskID>>>>();
                cmd.View.BqlSelect.AggregateNew<Aggregate<Sum<KGOwnerRevenueAllocation.amount>>>();

                foreach (KGOwnerRevenueAllocation row in cmd.View.Select(null,
                                                                         null,
                                                                         PXView.Searches,
                                                                         OwnerRevenueAlloc.View.GetExternalSorts(),
                                                                         OwnerRevenueAlloc.View.GetExternalDescendings(),
                                                                         OwnerRevenueAlloc.View.GetExternalFilters(),
                                                                         ref startRow,
                                                                         PXView.MaximumRows,
                                                                         ref totalRows))
                {
                    filter.TotalAmtByTask = row.Amount;
                }
            }

            yield return filter;

            FilterView.Cache.IsDirty = false;
        }

        protected virtual IEnumerable ownerRevenue()
        {
            HashSet<string> existing = new HashSet<string>();
            
            foreach (KGOwnerRevenueAllocation res in OwnerRevenueAlloc.Cache.Inserted)
            {
                existing.Add(res.ProjectID.ToString() + res.CostCodeID.ToString());
            }

            //int startRow = PXView.StartRow;
            //int totalRows = 0;

            //foreach (
            //    var res in
            //        cmd.View.Select(PXView.Currents, new object[] { Folders.Current.CategoryID }, PXView.Searches, PXView.SortColumns, PXView.Descendings,
            //            PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows))
            //{
            //    yield return res;
            //    PXView.StartRow = 0;
            //}

            foreach (KGOwnerRevenue row in GetOwnerRevenue())
            {
                KGOwnerRevenueAllocation reveAlloc = PXSelectGroupBy<KGOwnerRevenueAllocation,
                                                                     Where<KGOwnerRevenueAllocation.projectID, Equal<Required<KGOwnerRevenueAllocation.projectID>>,
                                                                           And<KGOwnerRevenueAllocation.costCodeID, Equal<Required<KGOwnerRevenueAllocation.costCodeID>>,
                                                                               And<KGOwnerRevenueAllocation.taskID, IsNotNull>>>,
                                                                     Aggregate<Sum<KGOwnerRevenueAllocation.qty>>>
                                                                     .Select(this, row.ProjectID, row.CostCodeID);
                row.RemainQty = row.Qty - (reveAlloc.Qty ?? 0);

                if (row.RemainQty > 0 && !existing.Contains(row.ProjectID.ToString() + row.CostCodeID.ToString())) { yield return row; }
            }
        }

        protected virtual IEnumerable tasks([PXDBInt] Int32? taskID)
        {
            List<ProjectTask> projTasks = new List<ProjectTask>();

            if (taskID == null)
            {
                projTasks.Add(new ProjectTask() { Description = PXSiteMap.RootNode.Title,
                                                  Icon = Sprite.Main.GetFullUrl(PX.Web.UI.Sprite.Main.Folder),
                                                  ParentID = FilterView.Current.ProjectID,
                                                  TaskID = 0 });     
            }
            else
            {
                FilterTable filter = FilterView.Current;

                if (filter != null && filter.ProjectID.HasValue)
                {
                    PXSelectBase<PMTask> cmd = new PXSelect<PMTask,
                                                            Where<PMTask.projectID, Equal<Required<PMTask.projectID>>>,
                                                            OrderBy<Asc<PMTask.taskCD>>>(this);

                    foreach (PMTask task in cmd.SelectWindowed(0, cmd.Select().Count, new object[] { filter.ProjectID }))
                    {
                        if (taskID == 0)
                        {
                            projTasks.Add(new ProjectTask()
                            {
                                TaskID      = task.TaskID,
                                Description = (task.TaskCD + "-" + task.Description),
                                ParentID    = taskID
                            });
                        }
                    }
                }
            }

            return projTasks;
        }

        protected virtual IEnumerable ownerRevenueAlloc([PXDBInt] Int32? taskID)
        {
            PMTask task = PXSelectReadonly<PMTask, 
                                           Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, taskID);

            if (task != null)
            {
                foreach (KGOwnerRevenueAllocation revenueAlloc in PXSelect<KGOwnerRevenueAllocation, 
                                                                           Where<KGOwnerRevenueAllocation.projectID, Equal<Required<KGOwnerRevenueAllocation.projectID>>, 
                                                                                 And<KGOwnerRevenueAllocation.taskID, Equal<Required<KGOwnerRevenueAllocation.taskID>>>>>
                                                                           .Select(this, new object[] { task.ProjectID, task.TaskID }))
                {
                    yield return revenueAlloc;
                }
            }
        }
        #endregion

        #region Function
        public virtual void AddOwnReve(KGOwnerRevenue revenue)
        {
            KGOwnerRevenueAllocation revenueAlloc = new KGOwnerRevenueAllocation();

            revenueAlloc.ProjectID    = revenue.ProjectID;
            revenueAlloc.CostCodeID   = revenue.CostCodeID;
            revenueAlloc.CostCodeDesc = revenue.CostCodeDesc;
            revenueAlloc.TaskID       = Tasks.Current.TaskID;
            revenueAlloc.Qty          = revenue.RemainQty;
            revenueAlloc.Rate         = revenue.Rate;
            revenueAlloc.Uom          = revenue.Uom;
            revenueAlloc.Amount       = revenue.Amount;

            OwnerRevenueAlloc.Cache.Insert(revenueAlloc);
        }

        public virtual ICollection<KGOwnerRevenue> GetOwnerRevenue()
        {
            if (ownerRevenues == null)
            {
                ownerRevenues = BuildOwnerReveLookup();
            }

            return ownerRevenues.Values;
        }

        protected Dictionary<int, KGOwnerRevenue> ownerRevenues;
        public virtual Dictionary<int, KGOwnerRevenue> BuildOwnerReveLookup()
        {
            Dictionary<int, KGOwnerRevenue> result = new Dictionary<int, KGOwnerRevenue>();

            var select = new PXSelect<KGOwnerRevenue,
                                      Where<KGOwnerRevenue.projectID, Equal<Current<FilterTable.projectID>>>,
                                      OrderBy<Asc<KGOwnerRevenue.costCodeID>>>(this);

            foreach (KGOwnerRevenue record in select.Select())
            {
                result.Add(record.CostCodeID.Value, record);
            }

            return result;
        }

        #endregion

        #region External Table
        [Serializable]
        public class ProjectTask : IBqlTable
        {
            #region ParentID       
            [PXInt()]
            public virtual Int32? ParentID { get; set; }
            public abstract class parentID : IBqlField { }
            #endregion

            #region TaskID
            [PXInt()]
            public virtual Int32? TaskID { get; set; }
            public abstract class taskID : IBqlField { }
            #endregion

            #region Description        
            [PXDBString(100, IsUnicode = true)]
            [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual string Description { get; set; }
            public abstract class description : IBqlField { }
            #endregion

            #region Icon
            [PXString(250)]
            public virtual string Icon { get; set; }
            public abstract class icon : IBqlField { }
            #endregion
        }

        [Serializable]
        public class FilterTable : IBqlTable
        {
            #region ProjectID
            [PXDefault()]
            [ActiveProjectOrContractBaseAttribute(FieldClass = ProjectAttribute.DimensionName)]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region TotalAmtByProject
            [PXDBBaseCury()]
            [PXDefault(TypeCode.Decimal, "0.0")]
            [PXUIField(DisplayName = "Total Amount By Project", Enabled = false)]
            public virtual Decimal? TotalAmtByProject { get; set; }
            public abstract class totalAmtByProject : PX.Data.BQL.BqlDecimal.Field<totalAmtByProject> { }
            #endregion

            #region TotalAmtByTask
            [PXDBBaseCury()]
            [PXDefault(TypeCode.Decimal, "0.0")]
            [PXUIField(DisplayName = "Total Amount By Task", Enabled = false)]
            public virtual Decimal? TotalAmtByTask { get; set; }
            public abstract class totalAmtByTask : PX.Data.BQL.BqlDecimal.Field<totalAmtByTask> { }
            #endregion
        }
        #endregion
    }
}