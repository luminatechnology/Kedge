using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.GL;
using PX.Objects.IN;

namespace Kedge.DAC
{
  [Serializable]
  public class KGDailySchedule : IBqlTable
  {
        #region Selected
        public abstract class selected : IBqlField
        { }
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion
  
        #region DailyScheduleID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Daily Schedule ID")]
        public virtual int? DailyScheduleID { get; set; }
        public abstract class dailyScheduleID : IBqlField { }
        #endregion

        #region DailyReportID
        [PXDBInt()]
        //[PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Daily Report ID")]
        /*
        [PXCheckUnique(Where = typeof(Where<dailyReportID, Equal<Current<dailyReportID>>,
            And<projectTaskID, Equal<Current<projectTaskID>>,
            And<costCodeID, Equal<Current<costCodeID>>>>>))]*/
        [PXParent(typeof(Select<KGDailyTask,
                            Where<KGDailyTask.dailyTaskID,
                            Equal<Current<KGDailySchedule.dailyTaskID>>,
                            And<KGDailyTask.dailyReportID,Equal<Current<KGDailySchedule.dailyReportID>>>>>))]
        [PXDBDefault(typeof(KGDailyTask.dailyReportID))]
        public virtual int? DailyReportID { get; set; }
        public abstract class dailyReportID : IBqlField { }
        #endregion

        #region DailyTaskID
        [PXDBInt()]
        [PXUIField(DisplayName = "Daily Task ID")]
        [PXDBDefault(typeof(KGDailyTask.dailyTaskID))]
        /*
        [PXCheckUnique(Where = typeof(Where<dailyReportID, Equal<Current<dailyReportID>>,
            And<projectTaskID, Equal<Current<projectTaskID>>>>))]*/
        public virtual int? DailyTaskID { get; set; }
        public abstract class dailyTaskID : IBqlField { }
        #endregion  
      
        #region ProjectTaskID
        //[PXDBInt(IsKey = true)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Project Task ID",Enabled =false)]
        [PXDimensionSelector("PROTASK", typeof(Search<PMTask.taskID,
                    Where<PMTask.projectID, Equal<Current<KGDailyReport.contractID>>>>
                        ), typeof(PMTask.taskCD), typeof(PMTask.description),
                typeof(PMTask.status), typeof(PMTask.isDefault), DescriptionField = typeof(PMTask.description))]
        /*
        [PXCheckUnique(Where = typeof(Where<dailyReportID, Equal<Current<dailyReportID>>,
            And<projectTaskID, Equal<Current<projectTaskID>>,
            And<costCodeID, Equal<Current<costCodeID>>>>>))]*/
        public virtual int? ProjectTaskID { get; set; }
        public abstract class projectTaskID : IBqlField { }
        #endregion

        #region CostCodeID
       // [PXDBInt(IsKey = true)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Code ID", Enabled = false)]
        /*
        [PXCheckUnique(Where = typeof(Where<dailyReportID, Equal<Current<dailyReportID>>,
            And<projectTaskID, Equal<Current<projectTaskID>>>>))]*/
        [PXDimensionSelector("COSTCODE", typeof(PMCostCode.costCodeID), typeof(PMCostCode.costCodeCD), 
            typeof(PMCostCode.description), typeof(PMCostCode.isDefault),DescriptionField =typeof(PMCostCode.description))]
        /*
        PXResultset<PMRevenueBudget> set = PXSelect<PMRevenueBudget,
                    Where<PMRevenueBudget.projectTaskID, Equal<Required<PMRevenueBudget.projectTaskID>>,
                        And < PMRevenueBudget.type, Equal <PX.Objects.GL.AccountType.income >>>>.Select(graph, taskID)
        */
        public virtual int? CostCodeID { get; set; }
        public abstract class costCodeID : IBqlField { }
        #endregion
      
        #region ActualQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Actual Qty")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual Decimal? ActualQty { get; set; }
        public abstract class actualQty : IBqlField { }
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

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion
      
        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        [PXSelector(typeof(InventoryItem.inventoryID),SubstituteKey = typeof(InventoryItem.inventoryCD))] 
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : IBqlField { }
        #endregion
      
        #region InventoryDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        //[PXUIField(DisplayName = "UsrInventoryDesc", Enabled = false)]
        [PXUIField(DisplayName = "CostCodeDesc", Enabled = false)]
        public virtual string InventoryDesc { get; set; }
        public abstract class inventoryDesc : IBqlField { }
        #endregion
      
        #region RevisedQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Revised Budgeted Quantity",Enabled =false)]
        public virtual Decimal? RevisedQty { get; set; }
        public abstract class  revisedQty : IBqlField { }
        #endregion
      
        #region AccumulateQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Accumulate Qty", Enabled = false)]
        /*
        [PXDBScalar(typeof(Search5<KGDailySchedule.actualQty,
            InnerJoin<KGDailyReport,On<KGDailyReport.dailyReportID,Equal<KGDailySchedule.dailyReportID>>,
                InnerJoin<KGDailyTask,On<KGDailyTask.dailyTaskID,Equal<KGDailySchedule.dailyTaskID>>>
            >, Where<KGDailyReport.workDate, Less<Current<KGDailyReport.workDate>>,
                And<KGDailySchedule.costCodeID,Equal<KGDailySchedule.costCodeID>,
                And<KGDailySchedule.projectTaskID ,Equal<KGDailySchedule.projectTaskID>,
                And<KGDailySchedule.contractID, Equal<KGDailySchedule.contractID>,
                And<KGDailySchedule.inventoryID, Equal<KGDailySchedule.inventoryID>>>>>>,
            Aggregate<GroupBy<KGDailySchedule.projectTaskID,GroupBy<KGDailySchedule.costCodeID,
                GroupBy<KGDailyReport.contractID,Sum<KGDailySchedule.actualQty>>>>>
            >))]*/
        public virtual Decimal? AccumulateQty { get; set; }
        public abstract class accumulateQty : IBqlField { }
        #endregion

        #region TotalAccumulateQty
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Accumulate Qty", Enabled = false)]
        [PXFormula(typeof(Add<accumulateQty, actualQty>))]
        //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? TotalAccumulateQty { get; set; }
        public abstract class totalAccumulateQty : IBqlField { }
        #endregion

        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Original Budgeted Quantity", Enabled = false)]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : IBqlField { }
        #endregion
      
        #region ChangeOrderQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Budgeted CO Quantity", Enabled = false)]
        public virtual Decimal? ChangeOrderQty { get; set; }
        public abstract class changeOrderQty : IBqlField { }
        #endregion


        #region Rate

        [PXDecimal()]
        
        [PXDBScalar(typeof(Search<PMRevenueBudget.curyUnitRate, Where<PMRevenueBudget.costCodeID, Equal<KGDailySchedule.costCodeID>
            
            ,And<PMRevenueBudget.projectTaskID,Equal<KGDailySchedule.projectTaskID>
            ,And<PMRevenueBudget.projectID,Equal<KGDailySchedule.contractID>,
                And<PMRevenueBudget.inventoryID,Equal<KGDailySchedule.inventoryID>>>>>>))]
        [PXUIField(DisplayName = "Rate")]
        public virtual Decimal? Rate { get; set; }
        public abstract class rate : IBqlField { }
        #endregion
        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "ProjectCD", Required = true)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region AccountGroupID
        [PXDBInt]
        [PXUIField(DisplayName = "Account Group ID")]

        public virtual int? AccountGroupID { get; set; }
        public abstract class accountGroupID : IBqlField { }
        #endregion

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch(typeof(KGDailyReport.branchID))]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion*/

        public static void UpdateKeyFields(PXGraph graph, int? oldProjectTaskID, int? oldDailyReportID, int? newProjectTaskID, int? newDailyReportID)
        {
         
            PXResultset<KGDailySchedule> res = PXSelect<KGDailySchedule,
                        Where<KGDailySchedule.projectTaskID, Equal<Optional<KGDailyTask.projectTaskID>>,
                        And<KGDailySchedule.dailyReportID, Equal<Optional<KGDailyReport.dailyReportID>>>>>
                        .Select(graph, oldProjectTaskID, oldDailyReportID);
            foreach (KGDailySchedule kgDailySchedule in res)
            {
                kgDailySchedule.ProjectTaskID = newProjectTaskID;
                kgDailySchedule.DailyReportID = newDailyReportID;
                graph.Caches[typeof(KGDailySchedule)].Update(kgDailySchedule);
            }
        }
       

    }
}