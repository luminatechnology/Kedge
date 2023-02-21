using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.PM;
using System;
using Kedge.DAC;

namespace PX.Objects.RQ
{
  public class RQRequestLineExt : PXCacheExtension<PX.Objects.RQ.RQRequestLine>
  {
        #region UsrContractID
        /*[PXDBInt]
        [PXUIField(DisplayName="Project")]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.status, Equal<ProjectStatus.active>>>>),
            typeof(PMProject.contractCD),typeof(PMProject.description), typeof(PMProject.status),SubstituteKey = typeof(PMProject.contractCD), DescriptionField = typeof(PMProject.description))]
        [PXForeignReference(typeof(Field<usrContractID>.IsRelatedTo<PMProject.contractID>))]*/
        [ProjectBaseExtAttribute()]
        public virtual int? UsrContractID { get; set; }
        public abstract class usrContractID : PX.Data.BQL.BqlInt.Field<usrContractID> { }
        #endregion

        #region UsrCostCodeID
        [PXDBInt]
        [PXUIField(DisplayName= "Cost Code")]
        //會導致RQ301000 不能上一筆下一筆
        //[CostCode(typeof(RQRequisitionLine.expenseAcctID), typeof(usrProjectTaskID), GL.AccountType.Expense)]
        //[PXSelector(typeof(PMCostCode.costCodeID), typeof(PMCostCode.costCodeCD), typeof(PMCostCode.description), typeof(PMCostCode.isDefault),SubstituteKey = typeof(PMCostCode.costCodeCD))]
        /*[PXDimensionSelector("COSTCODE", typeof(Search2<PMCostCode.costCodeID,InnerJoin<PMCostBudget,On<PMCostBudget.costCodeID,Equal<PMCostCode.costCodeID>>>,
            Where<PMCostBudget.projectID, Equal<Current<usrContractID>>,And<PMCostBudget.projectTaskID,Equal<Current<usrProjectTaskID>>>>>),
            typeof(PMCostCode.costCodeCD), typeof(PMCostCode.description), typeof(PMCostCode.isDefault), DescriptionField = typeof(PMCostCode.description))]*/
            
        [PXDimensionSelector("COSTCODE", typeof(Search5<PMCostCode.costCodeID, InnerJoin<PMCostBudget, On<PMCostBudget.costCodeID, Equal<PMCostCode.costCodeID>>>,
        Where<PMCostBudget.projectID, Equal<Current<usrContractID>>, And<PMCostBudget.projectTaskID, Equal<Current<usrProjectTaskID>>>>,Aggregate<GroupBy<PMCostCode.costCodeID>>>),
        typeof(PMCostCode.costCodeCD), typeof(PMCostCode.description), typeof(PMCostCode.isDefault), DescriptionField = typeof(PMCostCode.description))]    
        public virtual int? UsrCostCodeID { get; set; }
        public abstract class usrCostCodeID : PX.Data.BQL.BqlInt.Field<usrCostCodeID> { }
        #endregion

        #region UsrProjectTaskID
        [PXDBInt]
        [PXUIField(DisplayName="Sub Job")]
        [PXDefault(typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<usrContractID>>, And<PMTask.isDefault, Equal<True>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        //會導致RQ301000 不能上一筆下一筆
        //[ActiveOrInPlanningProjectTaskAttribute(typeof(RQRequisitionLineExt.usrContractID), BatchModule.PO, DisplayName = "Project Task")]
        //[PXSelector(typeof(PMTask.taskID), typeof(PMTask.taskCD), typeof(PMTask.description), typeof(PMTask.status),SubstituteKey = typeof(PMTask.taskCD))]
        [PXDimensionSelector("PROTASK", typeof(Search<PMTask.taskID,Where<PMTask.projectID, Equal<Current<usrContractID>>>>), typeof(PMTask.taskCD), typeof(PMTask.description), 
                  typeof(PMTask.status), typeof(PMCostCode.isDefault),DescriptionField =typeof(PMTask.description))]
        /*[PXDimensionSelector("PROTASK", typeof(Search<PMTask.taskID>),
                typeof(PMTask.taskID), typeof(PMTask.taskCD), typeof(PMTask.description),
                typeof(PMTask.status), typeof(PMCostCode.isDefault))]*/
        //[ProjectTask(typeof(RQRequestLineExt.usrProjectTaskID), AlwaysEnabled = true, DirtyRead = true)]
        [PXForeignReference(typeof(Field<usrProjectTaskID>.IsRelatedTo<PMTask.taskID>))]
        public virtual int? UsrProjectTaskID { get; set; }
        public abstract class usrProjectTaskID : PX.Data.BQL.BqlInt.Field<usrProjectTaskID> { }
            #endregion

        #region UsrAccountGroupID
        [PXDBInt]
        [PXUIField(DisplayName = "Account Group ID")]
        [PXSelector(typeof(Search<PMAccountGroup.groupID
            //, Where<PMAccountGroup.isActive, Equal<True>> //專案預算可選到 isActive = false
            >),
            typeof(PMAccountGroup.groupCD), 
            typeof(PMAccountGroup.description), 
            typeof(PMAccountGroup.type),
            SubstituteKey = typeof(PMAccountGroup.groupCD),
            DescriptionField = typeof(PMAccountGroup.description))]
        public virtual int? UsrAccountGroupID { get; set; }
        public abstract class usrAccountGroupID : PX.Data.BQL.BqlInt.Field<usrAccountGroupID> { }
        #endregion

        #region  UsrAvailQty
        [PXDecimal()]
        [PXUIField(DisplayName = "UsrAvail Qty",Enabled =false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrAvailQty { get; set; }
        public abstract class usrAvailQty : IBqlField { }
        #endregion

        #region  UsrAvailAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "UsrAvail Amt")]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXFormula(typeof(Sub<PMBudget.revisedQty, PMBudget.committedQty>))]
        public virtual Decimal? UsrAvailAmt { get; set; }
        public abstract class usrAvailAmt : IBqlField { }
        #endregion
    }
}