using PX.Data;

namespace PX.Objects.PM
{
    public class PMChangeOrderCostBudgetExt : PXCacheExtension<PMChangeOrderCostBudget>
    {
        #region Unbound Fields

        #region UsrCostCodeDesc
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Cost Code Desc", Enabled = false)]
        [PXUnboundDefault]
        public virtual string UsrCostCodeDesc { get; set; }
        public abstract class usrCostCodeDesc : IBqlField { }
        #endregion

        #region UsrHasCostBudget
        [PXString()]
        [PXUnboundDefault(typeof(Search<PMCostBudget.type, Where<PMCostBudget.projectID, Equal<Current<PMChangeOrderCostBudget.projectID>>,
                                                                 And<PMCostBudget.projectTaskID, Equal<Current<PMChangeOrderCostBudget.projectTaskID>>,
                                                                    And<PMCostBudget.costCodeID, Equal<Current<PMChangeOrderCostBudget.costCodeID>>,
                                                                        And<PMCostBudget.accountGroupID, Equal<Current<PMChangeOrderCostBudget.accountGroupID>>,
                                                                            And<PMCostBudget.inventoryID, Equal<Current<PMChangeOrderCostBudget.inventoryID>>>>>>>>))]
        // Add a trigger since AccountGroupID won't have a value immediately.
        [PXFormula(typeof(Default<PMChangeOrderBudget.accountGroupID>))]
        [PXFormula(typeof(Default<PMChangeOrderBudget.costCodeID>))]
        [PXFormula(typeof(Default<PMChangeOrderBudget.inventoryID>))]
        public virtual string UsrHasCostBudget { get; set; }
        public abstract class usrHasCostBudget : PX.Data.BQL.BqlString.Field<usrHasCostBudget> { }
        #endregion

        #endregion

        #region UsrLineType
        public class COLineType_New : PX.Data.BQL.BqlString.Constant<COLineType_New>
        {
            public COLineType_New() : base (ChangeOrderLineType.NewDocument) { }
        }
        public class COLineType_Update : PX.Data.BQL.BqlString.Constant<COLineType_Update>
        {
            public COLineType_Update() : base(ChangeOrderLineType.Update) { }
        }

        [PXDBString(1, IsFixed = true)]
        [ChangeOrderLineType.List()]
        [PXUIField(DisplayName = "Budget Type", Enabled = false)]
        [PXFormula(typeof(IIf<Where<usrHasCostBudget, IsNull>, COLineType_New, COLineType_Update>))]
        [PXFormula(typeof(Default<PMChangeOrderCostBudget.inventoryID>))]
        public virtual string UsrLineType { get; set; }
        public abstract class usrLineType : PX.Data.BQL.BqlString.Field<usrLineType> { }
        #endregion

        #region UsrCostBudgetType
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Cost Budget Changed", Required = true)]
        [Kedge.KGLookUpLov(typeof(usrCostBudgetType), "INVPRTYPE")]
        [PXDefault(typeof(Search<PMBudgetExt.usrInvPrType, Where<PMBudget.projectID, Equal<Current<PMChangeOrderBudget.projectID>>,
                                                                 And<PMBudget.projectTaskID, Equal<Current<PMChangeOrderBudget.projectTaskID>>,
                                                                     And<PMBudget.costCodeID, Equal<Current<PMChangeOrderBudget.costCodeID>>,
                                                                         And<PMBudget.inventoryID, Equal<Current<PMChangeOrderCostBudget.inventoryID>>,
                                                                             And<PMBudget.accountGroupID, Equal<Current<PMChangeOrderBudget.accountGroupID>>>>>>>>))]
        [PXFormula(typeof(Default<PMChangeOrderBudget.accountGroupID>))]
        [PXFormula(typeof(Default<PMChangeOrderBudget.costCodeID>))]
        [PXFormula(typeof(Default<PMChangeOrderBudget.inventoryID>))]
        public string UsrCostBudgetType { get; set; }
        public abstract class usrCostBudgetType : PX.Data.BQL.BqlString.Field<usrCostBudgetType> { }
        #endregion
    }
}
