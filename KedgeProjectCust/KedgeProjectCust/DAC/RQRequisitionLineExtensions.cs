using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.RQ;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;
using Kedge.DAC;

namespace PX.Objects.RQ
{
  public class RQRequisitionLineExt : PXCacheExtension<PX.Objects.RQ.RQRequisitionLine>
  {
        #region UsrProjectTaskID
        [PXDBInt]
        [PXUIField(DisplayName="Sub Job")]
        [PXDefault(typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<usrContractID>>, And<PMTask.isDefault, Equal<True>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        //會導致RQ301000 不能上一筆下一筆
        //[ActiveOrInPlanningProjectTaskAttribute(typeof(RQRequisitionLineExt.usrContractID), BatchModule.PO, DisplayName = "Project Task")]
        [PXDimensionSelector("PROTASK", typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<usrContractID>>, Or<Where<Current<processflag>, Equal<Zero>>>>>),
                                        typeof(PMTask.taskCD),
                                        typeof(PMTask.description),
                                        typeof(PMTask.status), typeof(PMCostCode.isDefault), 
                             DescriptionField = typeof(PMTask.description))]
        [PXForeignReference(typeof(Field<usrProjectTaskID>.IsRelatedTo<PMTask.taskID>))]
        public virtual int? UsrProjectTaskID { get; set; }
        public abstract class usrProjectTaskID : IBqlField { }
        #endregion

        #region UsrCostCodeID
        [PXDBInt]
        [PXUIField(DisplayName= "Cost Code")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //會導致RQ301000 不能上一筆下一筆
        //[CostCode(typeof(RQRequisitionLine.expenseAcctID), typeof(usrProjectTaskID), GL.AccountType.Expense)]
        /*
        [PXDimensionSelector("COSTCODE", typeof(Search2<PMCostCode.costCodeID, InnerJoin<PMCostBudget, On<PMCostBudget.costCodeID, Equal<PMCostCode.costCodeID>>>,
            Where<PMCostBudget.projectID, Equal<Current<usrContractID>>, And<PMCostBudget.projectTaskID, Equal<Current<usrProjectTaskID>>, 
                Or<Current<processflag>, Equal<Zero>>>>>),
            typeof(PMCostCode.costCodeCD), typeof(PMCostCode.description), typeof(PMCostCode.isDefault))]*/
        [PXDimensionSelector("COSTCODE", typeof(Search5<PMCostCode.costCodeID, 
                                                InnerJoin<PMCostBudget, On<PMCostBudget.costCodeID, Equal<PMCostCode.costCodeID>>>,
                                                Where<Current<processflag>, Equal<Zero>,
                                                      Or<Where<PMCostBudget.projectID, Equal<Current<usrContractID>>, 
                                                      And<PMCostBudget.projectTaskID, Equal<Current<usrProjectTaskID>>>>>>, Aggregate<GroupBy<PMCostCode.costCodeID>>>),
                                         typeof(PMCostCode.costCodeCD), 
                                         typeof(PMCostCode.description), 
                                         typeof(PMCostCode.isDefault),
                             DescriptionField = typeof(PMCostCode.description))]
        [PXForeignReference(typeof(Field<usrCostCodeID>.IsRelatedTo<PMCostCode.costCodeID>))]
        public virtual int? UsrCostCodeID { get; set; }
        public abstract class usrCostCodeID : IBqlField { }
        #endregion

        #region UsrContractID

        //[PXDBInt]
        //[PXUIField(DisplayName="Project")]
        //[POProjectDefault(typeof(POLine.lineType), AccountType = typeof(POLine.expenseAcctID), PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        //[PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        //會導致RQ301000 不能上一筆下一筆
        //[ProjectBaseAttribute()]
        /*
        [PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.status, Equal<ProjectStatus.active>>>>),
                    typeof(PMProject.contractCD),
                    typeof(PMProject.description), 
                    typeof(PMProject.status),
                    SubstituteKey = typeof(PMProject.contractCD),
                    DescriptionField = typeof(PMProject.description))]*/
        [ProjectBaseExt]
        [PXForeignReference(typeof(Field<usrContractID>.IsRelatedTo<PMProject.contractID>))]
        public virtual int? UsrContractID { get; set; }
        public abstract class usrContractID : IBqlField { }
        #endregion

	    #region UsrAccountGroupID
        [PXDBInt]
        [PXUIField(DisplayName = "Account Group ID")]
        //[PXDefault( PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrAccountGroupID { get; set; }
        public abstract class usrAccountGroupID : IBqlField { }
        #endregion

        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXInt()]
        public virtual int? Processflag { get; set; }
        public abstract class processflag : IBqlField { }
    }
}