using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using Kedge.DAC;
using System.Collections;
using PX.Objects.PM;
using PX.Objects.GL;
using PX.Objects.PO;
using PX.Objects.IN;

//PM208000 TemplateMaint 280
namespace Kedge
{
    public class KGDailyReportEntry : PXGraph<KGDailyReportEntry, KGDailyReport>
    {

        public PXSelect<KGDailyReport> Dailys;
        public PXSetup<KGSetUp> KGSetup;

        //Tab1: KG Daily Note
        public PXSelect<KGDailyNote,
                Where<KGDailyNote.dailyReportID,
                Equal<Current<KGDailyReport.dailyReportID>>>> DailyNotes;

        //Tab2: KG Daily Schedule Task
        //[PXImport(typeof(KGDailyReport))]
        public PXSelectJoin<KGDailyTask,LeftJoin<PMTask,On<PMTask.taskID,Equal<KGDailyTask.projectTaskID>>>,
                Where<KGDailyTask.dailyReportID,
                Equal<Current<KGDailyReport.dailyReportID>>>> DailyTasks;

        //Tab2: KG Daily Schedule Progress
        //[PXImport(typeof(KGDailyReport))]
        public PXSelect<KGDailySchedule,
                Where<KGDailySchedule.projectTaskID, Equal<Optional<KGDailyTask.projectTaskID>>,
                And<KGDailySchedule.dailyReportID, Equal<Optional<KGDailyReport.dailyReportID>>>>> DailyProgress;

        //Tab3: KG Daily Renters
        /*
        public PXSelect<KGDailyRenter,
                Where<KGDailyRenter.contractID,
                Equal<Current<KGDailyReport.contractID>>,
                And<Where<KGDailyRenter.workDate,
                Equal<Current<KGDailyReport.workDate>>>>>> DailyRenters;*/

        //Tab3: KG Daily Renters Vendors
        /*
        public PXSelect<KGDailyRenterVendor,
                Where<KGDailyRenterVendor.dailyRenterID,
                Equal<Current<KGDailyRenter.dailyRenterID>>>> DailyRenterVendors;*/

        //Tab4: KG Daily HumanMaterial
        public PXSelect<KGDailyMaterial,
                Where<KGDailyMaterial.dailyReportID,
                Equal<Current<KGDailyReport.dailyReportID>>>> DailyMaterials;

        //Lines  RQRequestLine


        [PXCopyPasteHiddenView]
        public PXFilter<KGDailyReportFilter> addItemFilter;

        [PXFilterable]
        [PXCopyPasteHiddenView]
        //public PXSelectReadonly<PMCostBudget> pmCostBudget;
        ////[PXStringList(new string[] { "0", "1" }, new string[] { "分包", "零星" })]


        public PXSelect<PMTask,
                    Where<PMTask.projectID, Equal<Current<KGDailyReport.contractID>>,
                        And<PMTask.startDate, LessEqual<Current<KGDailyReport.workDate>>,
                        And<PMTask.status, Equal<ProjectTaskStatus.active>,
                        And<Where<PMTask.endDate, IsNull, Or<PMTask.endDate, GreaterEqual<Current<KGDailyReport.workDate>>>>>>
                        >>> pmTasks;
        /*
        public PXSelect<PMTask,
                    Where<PMTask.projectID, Equal<Current<KGDailyReport.contractID>>,
                        And<PMTask.status, Equal<ProjectTaskStatus.active>>>>
                         pmTasks;*/
        protected virtual IEnumerable PmTasks()
        {
            // Defining a dynamic data view
            PXSelectBase<PMTask> query =
                               new PXSelect<PMTask,
                    Where<PMTask.projectID, Equal<Current<KGDailyReport.contractID>>,
                        And<PMTask.status, Equal<ProjectTaskStatus.active>>>>(this);
            // The current filtering parameters
            KGDailyReportFilter filter = addItemFilter.Current;
            if (filter.WorkDate != null) {
                query.WhereAnd<Where<PMTask.startDate, LessEqual<Current<KGDailyReportFilter.workDate>>>>();
                query.WhereAnd<Where<PMTask.endDate, IsNull, Or<PMTask.endDate, GreaterEqual<Current<KGDailyReportFilter.workDate>>>>>();
            }
            else
            {
                if (filter.DateFrom != null)
                {
                    query.WhereAnd<Where<PMTask.startDate,
                        GreaterEqual<Current<KGDailyReportFilter.dateFrom>>>>();
                }
                if (filter.DateTo != null)
                {
                    query.WhereAnd<Where<PMTask.endDate, LessEqual<Current<KGDailyReportFilter.dateTo>>>>();
                }
            }
            return query.Select();
        }

        //Lines  RQRequestLine
        public PXAction<KGDailyReport> addItem;
        [PXUIField(DisplayName = "Add Schedule Task", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddItem(PXAdapter adapter)
        {
            KGDailyReport master = Dailys.Current;
            try
            {
                WebDialogResult result = pmTasks.AskExt(true);
                if (result == WebDialogResult.OK)
                {
                    return AddItems(adapter);
                    //Base.Transactions  POLine
                }
                else if (result == WebDialogResult.Cancel)
                {
                    pmTasks.Cache.Clear();
                    pmTasks.Cache.ClearQueryCache();
                    //projectCostFilter.Cache.Clear();
                    //projectCostFilter.Cache.ClearQueryCache(); 
                }
                else
                {
                    pmTasks.Cache.Clear();
                    pmTasks.Cache.ClearQueryCache();
                    // projectCostFilter.Cache.Clear();
                }
            }
            catch (Exception e)
            {
                pmTasks.Cache.Clear();
                pmTasks.Cache.ClearQueryCache();
                //addItemFilter.Current.DateFrom = master.WorkDate;
                //addItemFilter.Current.DateTo = master.WorkDate;
                addItemFilter.Current.WorkDate = master.WorkDate;
                //projectCostFilter.Cache.Clear(); ;
                throw e;
            }
            return adapter.Get();
        }
        public int toDays(DateTime? endDate, DateTime? startDate)
        {
            int day = ((TimeSpan)(endDate - startDate)).Days + 1;
            return day;
        }

        
        

        public PXAction<KGDailyReport> addItems;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select,
                      MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddItems(PXAdapter adapter)
        {
            //throw new PXException("123");
            foreach (PMTask pmTask in pmTasks.Cache.Cached)
            {
                if (pmTask.Selected != true)
                {
                    continue;
                }
                if (!checkLine(pmTask))
                {
                    continue;
                }
                KGDailyReport master = Dailys.Current;
                //HashSet<int> taskset = new HashSet<int>();
                KGDailyTask kgDailyTask = new KGDailyTask();
                kgDailyTask.ProjectTaskID = pmTask.TaskID;
                DateTime?    ScheduleStartDate = pmTask.StartDate;
                DateTime?   ScheduleEndDate = pmTask.EndDate;
                PMTaskExt pmTaskExt = pmTasks.Cache.GetExtension<PMTaskExt>(pmTask);
                kgDailyTask.ActualStartDate = pmTaskExt.UsrActualStartDate;
                kgDailyTask.ActualEndDate = pmTaskExt.UsrActualEndDate;
                //kgDailyTask.SchedulePercent = pmTask.CompletedPercent;
                if (ScheduleEndDate == null || ScheduleStartDate == null || master.WorkDate == null) {
                    kgDailyTask.SchedulePercent = 0;
                }
                else {
                    Decimal workToToday = toDays(master.WorkDate, ScheduleStartDate);
                    Decimal workToEndDay = toDays(ScheduleEndDate, ScheduleStartDate);
                    Decimal percent    = (workToToday /workToEndDay )* 100;
                    if (percent > 100) {
                        percent = 100;
                    } else if (percent < 0) {
                        percent = 0;
                    }
                    kgDailyTask.SchedulePercent = percent;


                }
                //taskset.Add(pmTask.TaskID.Value);
                DailyTasks.Update(kgDailyTask);
                foreach (PMRevenueBudget process in getKGDailySchedule(pmTask.TaskID.Value))
                {
                    KGDailySchedule schedule =(KGDailySchedule)DailyProgress.Cache.CreateInstance();
                    schedule.ProjectTaskID = process.TaskID;
                    schedule.CostCodeID = process.CostCodeID;
                    schedule.InventoryID = process.InventoryID;
                    schedule.AccountGroupID = process.AccountGroupID;
                    schedule.Qty = process.Qty;
                    schedule.ChangeOrderQty = process.ChangeOrderQty;
                    schedule.RevisedQty = process.RevisedQty;
                    //PMBudgetExt pmBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(process);
                    //schedule.InventoryDesc = pmBudgetExt.UsrInventoryDesc;
                    schedule.InventoryDesc = process.Description;
                    schedule.ContractID = master.ContractID;
                    schedule.Rate = process.CuryUnitRate;
                    KGDailySchedule accSchedule = getPreviousAccQty(master, schedule);
                    if (accSchedule == null)
                    {
                        schedule.AccumulateQty = 0;
                    }
                    else {
                        schedule.AccumulateQty = accSchedule.ActualQty;
                    }
                     DailyProgress.Update(schedule);
                }
                Decimal actualPercent = getActualPercent(kgDailyTask.ProjectTaskID,false);
                DailyTasks.Cache.SetValue<KGDailyTask.actualPercent>(kgDailyTask, actualPercent);

            }
            return adapter.Get();

        }
        public PXResultset<KGDailySchedule> getPreviousAccQty(KGDailyReport master, KGDailySchedule schedule) {
            PXGraph graph = new PXGraph();
            PXResultset<KGDailySchedule> set = PXSelectJoinGroupBy<KGDailySchedule,
                InnerJoin<KGDailyReport, On<KGDailyReport.dailyReportID, Equal<KGDailySchedule.dailyReportID>>,
                InnerJoin<KGDailyTask, On<KGDailyTask.dailyTaskID, Equal<KGDailySchedule.dailyTaskID>>>
            >, Where<KGDailyReport.workDate, Less<Required<KGDailyReport.workDate>>,
                And<KGDailySchedule.costCodeID, Equal<Required<KGDailySchedule.costCodeID>>,
                And<KGDailySchedule.projectTaskID, Equal<Required<KGDailySchedule.projectTaskID>>,
                And < KGDailySchedule.accountGroupID, Equal<Required<KGDailySchedule.accountGroupID>>,
                And<KGDailySchedule.inventoryID, Equal<Required<KGDailySchedule.inventoryID>>,
                And <KGDailyReport.contractID, Equal<Required<KGDailyReport.contractID>>>>>>>>,
            Aggregate<GroupBy<KGDailySchedule.projectTaskID, GroupBy<KGDailySchedule.costCodeID, GroupBy<KGDailyReport.contractID, Sum<KGDailySchedule.actualQty>>>>>
            >.Select(graph, master.WorkDate, schedule.CostCodeID, schedule.ProjectTaskID, schedule.AccountGroupID, schedule.InventoryID, master.ContractID);
            return set;
        }

        public static PXResultset<PMRevenueBudget> getRate(int costCodeID, int projectTaskID, int projectID,int inventoryID,int accountGroupID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<PMRevenueBudget> set = PXSelect<PMRevenueBudget, Where<PMRevenueBudget.costCodeID, Equal<Required<KGDailySchedule.costCodeID>>

                    , And<PMRevenueBudget.projectTaskID, Equal<Required<KGDailySchedule.projectTaskID>>
                    , And<PMRevenueBudget.projectID, Equal<Required<KGDailySchedule.contractID>>,
                        And<PMRevenueBudget.inventoryID, Equal< Required < KGDailySchedule.inventoryID>>,
                        And < PMRevenueBudget.accountGroupID, Equal < Required < KGDailySchedule.accountGroupID >>>>>>>> .
                    Select(graph, costCodeID, projectTaskID, projectID, inventoryID, accountGroupID);
             return set;
        }

        public bool checkLine(PMTask pmTask)
        {
            foreach (KGDailyTask allline in DailyTasks.Select())
            {

                if (pmTask.TaskID.Equals(allline.ProjectTaskID))
                {
                    return false;
                }
            }
            return true;
        }


        //暫時用不到
        private void setAll() {
            KGDailyReport master = Dailys.Current;
            if (master.ContractID != null && master.WorkDate != null)
            {
                HashSet<int> taskset = new HashSet<int>();

                foreach (PMTask pmTask in getKGDailyTask(master.ContractID.Value, master.WorkDate.Value))
                {
                    KGDailyTask kgDailyTask = new KGDailyTask();
                    kgDailyTask.ProjectTaskID = pmTask.TaskID;
                    DateTime? ScheduleStartDate = pmTask.StartDate;
                    DateTime? ScheduleEndDate = pmTask.EndDate;
                    kgDailyTask.SchedulePercent = pmTask.CompletedPercent;
                    taskset.Add(pmTask.TaskID.Value);

                    //實際percent已後說
                    DailyTasks.Update(kgDailyTask);
                    foreach (PMRevenueBudget process in getKGDailySchedule(pmTask.TaskID.Value))
                    {
                        KGDailySchedule schedule = new KGDailySchedule();
                        schedule.ProjectTaskID = process.TaskID;
                        schedule.CostCodeID = process.CostCodeID;
                        schedule.InventoryID = process.InventoryID;
                        schedule.Qty = process.Qty;
                        schedule.ChangeOrderQty = process.ChangeOrderQty;
                        schedule.RevisedQty = process.RevisedQty;
                        schedule.ContractID = master.ContractID;
                        schedule.Rate = process.CuryUnitRate;
                        DailyProgress.Update(schedule);
                    }
                }
            }
        }

        //AR303000
        private PXResultset<PMTask> getKGDailyTask(int contractID , DateTime workDate)
        {
            PXGraph graph = new PXGraph();
            PXResultset<PMTask> set = PXSelect<PMTask,
                    Where<PMTask.projectID, Equal<Required<KGDailyReport.contractID>>,
                        And<PMTask.startDate, LessEqual<Required<KGDailyReport.workDate>>,
                        And<Where<PMTask.endDate,IsNull,Or<PMTask.endDate,GreaterEqual<Required<KGDailyReport.workDate>>>>>
                        >>>.Select(graph, contractID, workDate);
            return set;
        }
        private PXResultset<KGDailyReport> getKGDailyReport(int contractID, DateTime workDate)
        {
            PXGraph graph = new PXGraph();
            KGDailyReport master = Dailys.Current;
            PXResultset<KGDailyReport> set = PXSelect<KGDailyReport,
                    Where<KGDailyReport.contractID, Equal<Required<KGDailyReport.contractID>>,
                        And< KGDailyReport.workDate, Equal<Required<KGDailyReport.workDate>>,
                        And< KGDailyReport.dailyReportID ,NotEqual<Required<KGDailyReport.dailyReportID>>>
                        >>>.Select(graph, contractID, workDate, master.DailyReportID);
            return set;
        }
        
        private PXResultset<PMRevenueBudget> getKGDailySchedule(int taskID)
        {
            PXGraph graph = new PXGraph();

            PXResultset<PMRevenueBudget> set = PXSelect<PMRevenueBudget,
                    Where<PMRevenueBudget.projectTaskID, Equal<Required<PMRevenueBudget.projectTaskID>>,
                        And < PMRevenueBudget.type, Equal <PX.Objects.GL.AccountType.income >>>>.Select(graph, taskID);
            return set;
        }

        /*
        private PXResultset<PMRevenueBudget> getKGDailySchedule(int contractID)
        {
            PXGraph graph = new PXGraph();

            PXResultset<PMRevenueBudget> set = PXSelect<PMRevenueBudget,
                    Where<PMRevenueBudget.projectID, Equal<Required<PMRevenueBudget.projectID>>>
                        >.Select(graph, contractID);
            return set;
        }*/


        protected virtual void KGDailyReport_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGDailyReport master = Dailys.Current;
            if (master.DailyReportID > 0)
            {
                PXUIFieldAttribute.SetEnabled<KGDailyReport.workDate>(Dailys.Cache, Dailys.Current, false);
            }
            else {
                PXUIFieldAttribute.SetEnabled<KGDailyReport.workDate>(Dailys.Cache, Dailys.Current, true);
            }
            
            if (DailyNotes.Current == null)
            {
                DailyNotes.Current =(KGDailyNote) DailyNotes.Cache.CreateInstance();
            }
                

        }
        protected virtual void KGDailySchedule_RowInserted(PXCache sender,PXRowInsertedEventArgs e)
        {
            if (!e.ExternalCall) {
                return;
            }
            KGDailySchedule detail = (KGDailySchedule)e.Row;
            //KGDailyReport master =    Dailys
            KGDailyTask line = DailyTasks.Current;
            if (line == null) {
                return;
            }
            detail.ProjectTaskID = line.ProjectTaskID;
            DailyTasks.Update(line);
        }
        
        protected void KGDailyReport_WorkDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            
            KGDailyReport master = Dailys.Current;
            if (master.ContractID != null && master.WorkDate != null) {
                KGDailyReport temp = getKGDailyReport(master.ContractID.Value, master.WorkDate.Value);
                if (temp != null) {
                      Dailys.Cache.RaiseExceptionHandling<KGDailyReport.workDate>(
                             master, master.WorkDate, new PXSetPropertyException("日報表同一個專案同一天不能件兩個日報"));
                }  
            }   
            /*
            foreach (KGDailyTask task in DailyTasks.Select()) {
                DailyTasks.Delete(task);
            }*/

        }
        protected void KGDailyReport_ContractID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            KGDailyReport master = Dailys.Current;
            foreach (KGDailyTask task in DailyTasks.Select()) {
                DailyTasks.Delete(task);
            }
            foreach (KGDailySchedule kgDailySchedule in DailyProgress.Select())
            {
                DailyProgress.Delete(kgDailySchedule);
            }
            foreach (KGDailyMaterial kgDailyMaterial in DailyMaterials.Select())
            {
                DailyMaterials.Delete(kgDailyMaterial);
            }
            /*
            foreach (KGDailyNote kgDailyNote in DailyNotes.Select())
            {
                DailyNotes.Delete(kgDailyNote);
            }*/


            if (DailyNotes.Current != null) {
                DailyNotes.Current.Remark = null;
                DailyNotes.Update(DailyNotes.Current);
            }
         
        }
       


        protected void KGDailyMaterial_MaterialID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGDailyReport master = Dailys.Current;
            KGDailyMaterial row =(KGDailyMaterial) e.Row;

            if (row.MaterialID == null) {
                row.UOM = null;
            }

            InventoryItem inventoryItem = PXSelectorAttribute.Select<KGDailyMaterial.materialID>(sender, row) as InventoryItem;
            if (inventoryItem != null) {

                row.UOM = inventoryItem.BaseUnit;
            }

        }
        protected void KGDailySchedule_ActualQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGDailyReport master = Dailys.Current;
            KGDailySchedule row = (KGDailySchedule)e.Row;

               

            if (getNum(row.Qty) < (getNum(row.AccumulateQty)+ getNum(row.ActualQty))) {
               
                DailyProgress.Cache.RaiseExceptionHandling<KGDailySchedule.actualQty>(
                     row, row.ActualQty, new PXSetPropertyException("QTY have to bigger than ActualQty+ AccumulateQty"));
                DailyTasks.Cache.RaiseExceptionHandling<KGDailyTask.actualPercent>(
                     DailyTasks.Current, DailyTasks.Current.ActualPercent, new PXSetPropertyException("ActualQty Error , please check KGDailySchedule 's actualQty"));
                return;
            }
            

            Decimal actualPercent = getActualPercent(DailyTasks.Current.ProjectTaskID, false);
            DailyTasks.Cache.SetValue<KGDailyTask.actualPercent>(DailyTasks.Current, actualPercent);
            if ((actualPercent > 0 ) && DailyTasks.Current.ActualStartDate == null) {
                DailyTasks.Cache.SetValue<KGDailyTask.actualStartDate>(DailyTasks.Current, master.WorkDate);
            }
            if ((actualPercent == 100) && DailyTasks.Current.ActualEndDate == null) {
                DailyTasks.Cache.SetValue<KGDailyTask.actualEndDate>(DailyTasks.Current, master.WorkDate);
            }
            if ((actualPercent < 100) && DailyTasks.Current.ActualEndDate != null)
            {
                DailyTasks.Cache.SetValue<KGDailyTask.actualEndDate>(DailyTasks.Current, null);
            }
            DailyTasks.Cache.Update(DailyTasks.Current);

            if (row.ActualQty < 0) {
                DailyTasks.Cache.RaiseExceptionHandling<KGDailyTask.actualPercent>(DailyTasks.Current, DailyTasks.Current.ActualPercent,
                    new PXSetPropertyException("Schedule's ActualQty can't be null or negative."));
                DailyProgress.Cache.RaiseExceptionHandling<KGDailySchedule.actualQty>(
                 row, row.ActualQty, new PXSetPropertyException("Schedule's ActualQty can't be null or negative."));
            }

        }

        public Decimal getActualPercent(int? ProjectTaskID,bool isPersist) {
            KGDailyReport master = Dailys.Current;
            Decimal totalAmount = 0;
            Decimal actualAmount = 0;

            PXResultset<KGDailySchedule> res = PXSelect<KGDailySchedule,
                        Where<KGDailySchedule.projectTaskID, Equal<Optional<KGDailyTask.projectTaskID>>,
                        And<KGDailySchedule.dailyReportID, Equal<Optional<KGDailyReport.dailyReportID>>>>>
                        .Select(this, ProjectTaskID, master.DailyReportID);
            foreach (KGDailySchedule schedule in res)
            {
                //避免搜到其他的Task
                if (schedule.ProjectTaskID != null && schedule.ProjectTaskID.Equals(ProjectTaskID))
                {
                    Decimal rate = getNum(schedule.Rate);

                    if (schedule.Rate == null)
                    {
                        PMRevenueBudget buget = getRate(schedule.CostCodeID.Value, schedule.ProjectTaskID.Value, 
                            schedule.ContractID.Value, schedule.InventoryID.Value, schedule.AccountGroupID.Value);
                        rate = buget.CuryUnitRate.Value;
                        schedule.Rate = rate;
                    }
                    Decimal accumulateQty = 0;
                    Decimal acctQty = 0;
                    if (isPersist)
                    {
                        KGDailySchedule sche = getAllAccQty(master, schedule);
                        accumulateQty = getNum(sche.ActualQty);
                        acctQty = 0;
                    }
                    else
                    {
                        accumulateQty = getNum(schedule.AccumulateQty);
                        acctQty = getNum(schedule.ActualQty);
                    }
                    Decimal qty = getNum(schedule.Qty);
                    totalAmount = totalAmount + (qty * rate);
                    actualAmount = actualAmount + ((accumulateQty + acctQty) * rate);
                }

            }
            if (totalAmount == 0)
            {
                return 0;
            }
            else {
                Decimal percent = (actualAmount / totalAmount) * 100;
                if (percent > 100)
                {
                    percent = 100;
                }
                else if (percent < 0)
                {
                    percent = 0;
                }
                return percent;
            }
            
        }

        public Decimal getNum(Decimal? num) {
            if (num == null)
            {
                return 0;
            }
            else {
                return num.Value;
            }

        }
        /*
        protected virtual void KGDailyTask_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            KGDailyTask  oldRow = (KGDailyTask)e.OldRow;
            KGDailyTask newRow = (KGDailyTask)e.Row;
            if (oldRow == null)
                return;
             KGDailySchedule.UpdateKeyFields(this, oldRow.ProjectTaskID, oldRow.DailyReportID, newRow.ProjectTaskID, newRow.DailyReportID);
        }*/

        public override void Persist()
        {
            KGDailyReport master = Dailys.Current;
            //代表刪除
            if (master == null)
            {
                base.Persist();
            }
            else {
                if (!beforeSaveCheck())
                {
                    return;
                }
                //setInsertData(master);
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    //throw new Exception();
                    foreach (KGDailyTask task in DailyTasks.Select())
                    {
                        Decimal accPercent = getActualPercent(task.ProjectTaskID, true);

                        PXUpdate<Set<PMTaskExt.usrConstructedPct, Required<PMTask.completedPercent>,
                            Set< PMTaskExt.usrActualStartDate, Required<PMTaskExt.usrActualStartDate>,
                            Set<PMTaskExt.usrActualEndDate, Required<PMTaskExt.usrActualEndDate>>>>,
                                PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Update(this, accPercent, task.ActualStartDate, task.ActualEndDate, task.ProjectTaskID);
                    }

                    ts.Complete();
                }
            }           
        }
        public bool beforeSaveCheck()
        {
            bool check = true;
            KGDailyReport master = Dailys.Current;

            if (master.WeatherAM ==null)
            {
                Dailys.Cache.RaiseExceptionHandling<KGDailyReport.weatherAM>(
                     master, master.WeatherAM, new PXSetPropertyException("WeatherAM can't be null"));

                check = false;
            }
            if (master.WeatherPM == null)
            {
                Dailys.Cache.RaiseExceptionHandling<KGDailyReport.weatherPM>(
                     master, master.WeatherPM, new PXSetPropertyException("WeatherPM can't be null"));

                check = false;
            }

            if (master.WorkDate == null)
            {
                Dailys.Cache.RaiseExceptionHandling<KGDailyReport.workDate>(
                     master, master.WorkDate, new PXSetPropertyException("WorkDate can't be null"));

                check = false;
            }

            if (master.ContractID == null)
            {
                Dailys.Cache.RaiseExceptionHandling<KGDailyReport.contractID>(
                     master, master.ContractID, new PXSetPropertyException("Project can't be null"));

                check = false;
            }

            if (master.ContractID != null && master.WorkDate != null)
            {
                KGDailyReport temp = getKGDailyReport(master.ContractID.Value, master.WorkDate.Value);
                if (temp != null)
                {
                    //KGDailyReportEntry graph = PXGraph.CreateInstance<KGDailyReportEntry>();
                    //graph.Dailys.Current = temp;
                    //throw new PXRedirectRequiredException(graph, true, "Daily Report");
                    //Dailys.Cache.Clear();
                    //Dailys.Update(temp);

                     Dailys.Cache.RaiseExceptionHandling<KGDailyReport.workDate>(
                             master, master.WorkDate, new PXSetPropertyException("日報表同一個專案同一天不能件兩個日報"));

                     check = false;
                  
                }
            }


            /*
            List<KGDailySchedule> list = new List<KGDailySchedule>(DailyProgress.Cache.Inserted.RowCast<KGDailySchedule>());
            list.AddRange((IEnumerable<KGDailySchedule>) DailyProgress.Cache.Updated);
            foreach (KGDailySchedule schedule in DailyProgress.Cache.Inserted) {


            }*/
            foreach (KGDailyTask task in DailyTasks.Select())
            {
                PXResultset<KGDailySchedule> res = PXSelect<KGDailySchedule,
                        Where<KGDailySchedule.projectTaskID, Equal<Optional<KGDailyTask.projectTaskID>>,
                        And<KGDailySchedule.dailyReportID, Equal<Optional<KGDailyReport.dailyReportID>>>>>
                        .Select(this, task.ProjectTaskID, master.DailyReportID);
                foreach (KGDailySchedule schedule in res)
                {
                    if (getNum(schedule.Qty) < (getNum(schedule.AccumulateQty) + getNum(schedule.ActualQty)))
                    {
                        DailyProgress.Cache.RaiseExceptionHandling<KGDailySchedule.actualQty>(
                             schedule, schedule.ActualQty, new PXSetPropertyException("總累計數量不可大於修訂後預算數量"));
                        
                        DailyTasks.Cache.RaiseExceptionHandling<KGDailyTask.actualPercent>(
                             DailyTasks.Current, task, new PXSetPropertyException("ActualQty Error , please check KGDailySchedule 's actualQty"));
                        return false;
                    }
                    if (schedule.ActualQty < 0)
                    {
                        DailyTasks.Cache.RaiseExceptionHandling<KGDailyTask.actualPercent>(DailyTasks.Current, DailyTasks.Current.ActualPercent,
                            new PXSetPropertyException("Schedule's ActualQty can't be null or negative."));
                        DailyProgress.Cache.RaiseExceptionHandling<KGDailySchedule.actualQty>(
                         schedule, schedule.ActualQty, new PXSetPropertyException("Schedule's ActualQty can't be null or negative."));
                    }
                }
            }

            return check;
        }
        public PXResultset<KGDailySchedule> getAllAccQty(KGDailyReport master, KGDailySchedule schedule)
        {
            PXGraph graph = new PXGraph();
            PXResultset<KGDailySchedule> set = PXSelectJoinGroupBy<KGDailySchedule,
                InnerJoin<KGDailyReport, On<KGDailyReport.dailyReportID, Equal<KGDailySchedule.dailyReportID>>,
                InnerJoin<KGDailyTask, On<KGDailyTask.dailyTaskID, Equal<KGDailySchedule.dailyTaskID>>>
            >, Where<KGDailySchedule.costCodeID, Equal<Required<KGDailySchedule.costCodeID>>,
                And<KGDailySchedule.projectTaskID, Equal<Required<KGDailySchedule.projectTaskID>>,
                And<KGDailyReport.contractID, Equal<Required<KGDailyReport.contractID>>,
                And<KGDailySchedule.accountGroupID, Equal<Required<KGDailySchedule.accountGroupID>>,
                And<KGDailySchedule.inventoryID, Equal<Required<KGDailySchedule.inventoryID>>>>>>>,
            Aggregate<GroupBy<KGDailySchedule.projectTaskID, GroupBy<KGDailySchedule.costCodeID, GroupBy<KGDailyReport.contractID, Sum<KGDailySchedule.actualQty>>>>>
            >.Select(graph,  schedule.CostCodeID, schedule.ProjectTaskID, master.ContractID, schedule.AccountGroupID, schedule.InventoryID);
            return set;
        }


        /*
         * PXUpdate<Set<ARRegisterExtendsion.usrArGuiInvoiceNbr, Null>,
                            ARRegisterExtendsion,
                            Where<ARRegisterExtendsion.refNbr, Equal<Required<ARRegisterExtendsion.refNbr>>,
                            And<ARRegisterExtendsion.docType, Equal<Required<ARRegisterExtendsion.docType>>>>>
                            .Update(this, INVNBR.RefNbr, INVNBR.DocType);

         * */

        [Serializable]
        public class KGDailyReportFilter : IBqlTable
        {

           
            #region DateFrom
            public abstract class dateFrom : PX.Data.IBqlField
            {
            }
            protected DateTime? _DateFrom;
            [PXDate()]
            [PXDefault()]
            [PXUIField(DisplayName = "Task Date From")]

            public virtual DateTime? DateFrom
            {
                get
                {
                    return this._DateFrom;
                }
                set
                {
                    this._DateFrom = value;
                }
            }

            #endregion

            #region DateTo
            public abstract class dateTo : PX.Data.IBqlField
            {
            }
            protected DateTime? _DateTo;
            [PXDate()]
            [PXDefault()]
            [PXUIField(DisplayName = "Task Date To")]

            public virtual DateTime? DateTo
            {
                get
                {
                    return this._DateTo;
                }
                set
                {
                    this._DateTo = value;
                }
            }

            #endregion

            #region WorkDate
            [PXDate()]
            [PXUIField(DisplayName = "Work Date")]
            public virtual DateTime? WorkDate { get; set; }
            public abstract class workDate : IBqlField { }
            #endregion

        }
    }
}