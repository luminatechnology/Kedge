using System;
using System.Collections;
using Kedge.DAC;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;

namespace Kedge
{
    public class KGDailyMaterialReport : PXGraph<KGDailyMaterialReport>
    {

        #region View
        public PXFilter<FilterTable> MasterView;
        public PXFilter<DetailsTable> DetailsView;
        protected virtual IEnumerable detailsView()
        {
            POLine line;
            KGDailyRenter kgdr;
            DateTime? date;
            DateTime? extDate = MasterView.Current.ExecutionTime;
            setLabel(MasterView.Current);
            PXSelectBase<DetailsTable> cmd = new PXSelect<DetailsTable>(this);
            String sql = cmd.ToString();
            foreach (DetailsTable dt in cmd.Select())
            {
                dt.BudgetAmount = getBudget(dt.ProjectID, dt.InventoryID) ?? 0;//預算

                #region Stage one
                date = dt.StageOneDate;
                foreach (PXResult<POLine, KGDailyRenter> obj in getPOLine(dt.InventoryID, date))
                {
                    line = (POLine)obj;
                    kgdr = (KGDailyRenter)obj;
                    dt.SOneQty = kgdr?.ActSelfQty ?? 0;//到工數量
                    dt.SOneAmt = dt.SOneQty * (line?.CuryUnitCost ?? 0);//到工金額
                    dt.SOneIncAmount = getIncAmount(dt.ProjectID, date, dt.InventoryID);//發生費用
                    dt.SOnePerBudget = "0%";
                    if (dt.BudgetAmount != Decimal.Zero)
                    {
                        //佔預算% =到工金額/預算
                        decimal? perBudget = dt.SOneIncAmount / dt.BudgetAmount;
                        dt.SOnePerBudget = perBudget?.ToString("P");
                    }
                }
                #endregion
                #region Stage Two
                date = dt.StageTwoDate;
                foreach (PXResult<POLine, KGDailyRenter> obj in getPOLine(dt.InventoryID, date))
                {
                    line = (POLine)obj;
                    kgdr = (KGDailyRenter)obj;
                    dt.STwoQty = kgdr?.ActSelfQty ?? 0;//到工數量
                    dt.STwoAmt = dt.STwoQty * (line?.CuryUnitCost ?? 0);//到工金額
                    dt.STwoIncAmount = getIncAmount(dt.ProjectID, date, dt.InventoryID);//發生費用
                    dt.STwoPerBudget = "0%";
                    if (dt.BudgetAmount != Decimal.Zero)
                    {
                        //佔預算% =到工金額/預算
                        decimal? perBudget = dt.STwoIncAmount / dt.BudgetAmount;
                        dt.STwoPerBudget = perBudget?.ToString("P");
                    }
                }
                #endregion
                #region Stage Three
                date = dt.StageThreeDate;
                foreach (PXResult<POLine, KGDailyRenter> obj in getPOLine(dt.InventoryID, date))
                {
                    line = (POLine)obj;
                    kgdr = (KGDailyRenter)obj;
                    dt.SThreeQty = kgdr?.ActSelfQty ?? 0;//到工數量
                    dt.SThreeAmt = dt.SThreeQty * (line?.CuryUnitCost ?? 0);//到工金額
                    dt.SThreeIncAmount = getIncAmount(dt.ProjectID, date, dt.InventoryID);//發生費用
                    dt.SThreePerBudget = "0%";
                    if (dt.BudgetAmount != Decimal.Zero)
                    {
                        //佔預算% =到工金額/預算
                        decimal? perBudget = dt.SThreeIncAmount / dt.BudgetAmount;
                        dt.SThreePerBudget = perBudget?.ToString("P");
                    }
                }
                #endregion
                decimal? remainingPer = 1m;
                decimal? remainingInc = dt.BudgetAmount;
                #region Now
                date = extDate;
                foreach (PXResult<POLine, KGDailyRenter> obj in getPOLine(dt.InventoryID, date))
                {
                    line = (POLine)obj;
                    kgdr = (KGDailyRenter)obj;
                    dt.NowQty = kgdr?.ActSelfQty ?? 0;//到工數量
                    dt.NowAmt = dt.NowQty * (line?.CuryUnitCost ?? 0);//到工金額
                    dt.NowIncAmount = getIncAmount(dt.ProjectID, date, dt.InventoryID);//發生費用
                    dt.NowPerBudget = "0%";
                    remainingInc -= dt.NowIncAmount;
                    if (dt.BudgetAmount != Decimal.Zero)
                    {
                        //佔預算% =到工金額/預算
                        decimal? perBudget = dt.NowIncAmount / dt.BudgetAmount;
                        remainingPer -= perBudget;
                        dt.NowPerBudget = perBudget?.ToString("P");
                    }
                }
                #endregion
                #region Remaining
                dt.RemainingCost = remainingInc;
                dt.RePerBudget = remainingPer?.ToString("P");
                #endregion
                yield return dt;
            }
        }
        #endregion

        #region Event
        protected virtual void _(Events.FieldUpdated<FilterTable.projectID> e)
        {
            FilterTable row = e.Row as FilterTable;
            if (row == null) return;
            KGProjectStage ps = getProjectStage(row.ProjectID);
            if (ps != null)
            {
                row.StageOneDate = ps.StageOneDate;
                row.StageTwoDate = ps.StageTwoDate;
                row.StageThreeDate = ps.StageThreeDate;
                row.StageOneDesc = ps.StageOneDesc ?? "Stage One";
                row.StageTwoDesc = ps.StageTwoDesc ?? "Stage Two";
                row.StageThreeDesc = ps.StageThreeDesc ?? "Stage Three";
            }
            else
            {
                row.StageOneDate = null;
                row.StageTwoDate = null;
                row.StageThreeDate = null;
                row.StageOneDesc = "Stage One";
                row.StageTwoDesc = "Stage Two";
                row.StageThreeDesc = "Stage Three";
            }

        }
        #endregion

        #region Method
        private void setLabel(FilterTable dt)
        {
            String qtyL = "_到工數量";
            String amtL = "_到工金額";
            String budgetL = "_佔預算%";
            String incAmtL = "_發生費用";
            String remainingL = "剩餘";
            String nowL = "目前累計";
            String stageL = "";
            #region Stage One Label
            stageL = dt.StageOneDesc ?? "Stage One";
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sOneQty>(DetailsView.Cache, stageL + qtyL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sOneAmt>(DetailsView.Cache, stageL + amtL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sOnePerBudget>(DetailsView.Cache, stageL + budgetL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sOneIncAmount>(DetailsView.Cache, stageL + incAmtL);
            #endregion
            #region Stage Two Label
            stageL = dt.StageTwoDesc ?? "Stage Two";
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sTwoQty>(DetailsView.Cache, stageL + qtyL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sTwoAmt>(DetailsView.Cache, stageL + amtL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sTwoPerBudget>(DetailsView.Cache, stageL + budgetL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sTwoIncAmount>(DetailsView.Cache, stageL + incAmtL);
            #endregion
            #region Stage Three Label
            stageL = dt.StageThreeDesc ?? "Stage Three";
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sThreeQty>(DetailsView.Cache, stageL + qtyL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sThreeAmt>(DetailsView.Cache, stageL + amtL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sThreePerBudget>(DetailsView.Cache, stageL + budgetL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.sThreeIncAmount>(DetailsView.Cache, stageL + incAmtL);
            #endregion
            #region Now Label
            PXUIFieldAttribute.SetDisplayName<DetailsTable.nowQty>(DetailsView.Cache, nowL + qtyL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.nowAmt>(DetailsView.Cache, nowL + amtL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.nowPerBudget>(DetailsView.Cache, nowL + budgetL);
            PXUIFieldAttribute.SetDisplayName<DetailsTable.nowIncAmount>(DetailsView.Cache, nowL + incAmtL);
            #endregion
            #region Remaining Label
            PXUIFieldAttribute.SetDisplayName<DetailsTable.rePerBudget>(DetailsView.Cache, remainingL + "預算%");
            PXUIFieldAttribute.SetDisplayName<DetailsTable.remainingCost>(DetailsView.Cache, remainingL + "費用");
            #endregion
        }

        #endregion

        #region BQL
        private KGProjectStage getProjectStage(int? projectID)
        {
            return PXSelect<KGProjectStage,
                Where<KGProjectStage.projectID, Equal<Required<KGProjectStage.projectID>>>>
                .Select(this, projectID);
        }

        /// <summary>
        /// 取得工料預算
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        private decimal? getBudget(int? projectID, int? inventoryID)
        {
            PMCostBudget item = PXSelectGroupBy<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>>
                >,
                Aggregate<Sum<PMCostBudget.curyRevisedAmount>>
                >.Select(this, projectID, inventoryID);
            return item?.CuryRevisedAmount;
        }

        /// <summary>
        /// 取得數量(KGDailyRenter.ActSelfQty) 單價(POLine.CuryUnitCOst)
        /// </summary>
        /// <param name="inventoryID"></param>
        /// <param name="workDate"></param>
        /// <returns></returns>
        private PXResultset<POLine> getPOLine(int? inventoryID, DateTime? workDate)
        {
            return PXSelectJoinGroupBy<POLine,
                InnerJoin<KGDailyRenter,
                    On<POLine.orderNbr, Equal<KGDailyRenter.orderNbr>,
                    And<POLine.lineNbr, Equal<KGDailyRenter.lineNbr>>>>,
                Where<POLine.inventoryID, Equal<Required<POLine.inventoryID>>,
                And<KGDailyRenter.workDate, LessEqual<Required<KGDailyRenter.workDate>>>>,
                Aggregate<GroupBy<POLine.curyUnitCost, Sum<KGDailyRenter.actSelfQty>>>
                >.Select(this, inventoryID, workDate);
        }

        private decimal getIncAmount(int? projectID, DateTime? date, int? inventoryID)
        {
            String[] inStatus = new string[] {
                APDocStatus.Balanced, APDocStatus.Reserved,
                APDocStatus.Open, APDocStatus.Closed, APDocStatus.Prebooked };
            APTran apTran = PXSelectJoinGroupBy<APTran,
                InnerJoin<APRegister, On<APTran.refNbr, Equal<APRegister.refNbr>>>,
                Where<APRegister.projectID, Equal<Required<APRegister.projectID>>,//projectID
                And<APRegister.status, In<Required<APRegister.status>>,//inStatus
                And<APRegister.createdDateTime, LessEqual<Required<APRegister.createdDateTime>>,//date
                And<APRegister.docType, Equal<APDocType.invoice>,
                And<APTran.inventoryID, Equal<Required<APTran.inventoryID>>>>>>>,//inventoryID
                Aggregate<Sum<APTran.curyLineAmt>>>
                .Select(this, projectID, inStatus, date, inventoryID);
            return apTran?.CuryLineAmt ?? 0m;
        }
        #endregion

        #region Table
        #region FilterTable
        [Serializable]
        public class FilterTable : IBqlTable
        {
            #region ProjectID
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            [PXUIField(DisplayName = "Project ID", Required = true)]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [ProjectDefault()]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region ExecutionTime
            [PXDate()]
            [PXUIField(DisplayName = "Execution Time", Required = true)]
            [PXUnboundDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual DateTime? ExecutionTime { get; set; }
            public abstract class executionTime : PX.Data.BQL.BqlDateTime.Field<executionTime> { }
            #endregion

            #region unbound
            #region StageOneDate
            [PXDate()]
            [PXUIField(DisplayName = "Stage One Date", IsReadOnly = true)]
            public virtual DateTime? StageOneDate { get; set; }
            public abstract class stageOneDate : PX.Data.BQL.BqlDateTime.Field<stageOneDate> { }
            #endregion

            #region StageOneDesc
            [PXString(60, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Stage One Desc", IsReadOnly = true)]
            public virtual string StageOneDesc { get; set; }
            public abstract class stageOneDesc : PX.Data.BQL.BqlString.Field<stageOneDesc> { }
            #endregion

            #region StageTwoDate
            [PXDate()]
            [PXUIField(DisplayName = "Stage Two Date", IsReadOnly = true)]

            public virtual DateTime? StageTwoDate { get; set; }
            public abstract class stageTwoDate : PX.Data.BQL.BqlDateTime.Field<stageTwoDate> { }
            #endregion

            #region StageTwoDesc
            [PXString(60, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Stage Two Desc", IsReadOnly = true)]
            public virtual string StageTwoDesc { get; set; }
            public abstract class stageTwoDesc : PX.Data.BQL.BqlString.Field<stageTwoDesc> { }
            #endregion

            #region StageThreeDate
            [PXDate()]
            [PXUIField(DisplayName = "Stage Three Date", IsReadOnly = true)]
            public virtual DateTime? StageThreeDate { get; set; }
            public abstract class stageThreeDate : PX.Data.BQL.BqlDateTime.Field<stageThreeDate> { }
            #endregion

            #region StageThreeDesc
            [PXString(60, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Stage Three Desc", IsReadOnly = true)]
            public virtual string StageThreeDesc { get; set; }
            public abstract class stageThreeDesc : PX.Data.BQL.BqlString.Field<stageThreeDesc> { }
            #endregion
            #endregion

        }
        #endregion

        #region DetailsTable
        [Serializable]
        [PXProjection(typeof(
            Select2<KGProjectStage,
                InnerJoin<KGProjectRenterMaterial, On<KGProjectStage.projectStageID, Equal<KGProjectRenterMaterial.projectStageID>>,
                InnerJoin<InventoryItem, On<KGProjectRenterMaterial.inventoryID, Equal<InventoryItem.inventoryID>>>>,
            Where<KGProjectStage.projectID, Equal<CurrentValue<FilterTable.projectID>>>
            >), Persistent = false)]
        public class DetailsTable : IBqlTable
        {
            #region InventoryID
            [PXDBInt(IsKey = true, BqlField = typeof(KGProjectRenterMaterial.renterMaterialID))]
            public virtual int? RenterMaterialID { get; set; }
            public abstract class renterMaterialID : PX.Data.BQL.BqlInt.Field<renterMaterialID> { }
            #endregion

            #region StageOneDate
            [PXDBDate(BqlField = typeof(KGProjectStage.stageOneDate))]
            [PXUIField(DisplayName = "StageOneDate")]
            public virtual DateTime? StageOneDate { get; set; }
            public abstract class stageOneDate : PX.Data.BQL.BqlDateTime.Field<stageOneDate> { }
            #endregion

            #region StageOneDesc
            [PXDBString(60, IsUnicode = true, InputMask = "",
                BqlField = typeof(KGProjectStage.stageOneDesc))]
            public virtual string StageOneDesc { get; set; }
            public abstract class stageOneDesc : PX.Data.BQL.BqlString.Field<stageOneDesc> { }
            #endregion

            #region StageTwoDate
            [PXDBDate(BqlField = typeof(KGProjectStage.stageTwoDate))]
            [PXUIField(DisplayName = "StageTwoDate")]

            public virtual DateTime? StageTwoDate { get; set; }
            public abstract class stageTwoDate : PX.Data.BQL.BqlDateTime.Field<stageTwoDate> { }
            #endregion

            #region StageTwoDesc
            [PXDBString(60, IsUnicode = true, InputMask = "",
                BqlField = typeof(KGProjectStage.stageTwoDesc))]
            public virtual string StageTwoDesc { get; set; }
            public abstract class stageTwoDesc : PX.Data.BQL.BqlString.Field<stageTwoDesc> { }
            #endregion

            #region StageThreeDate
            [PXDBDate(BqlField = typeof(KGProjectStage.stageThreeDate))]
            [PXUIField(DisplayName = "StageThreeDate")]
            public virtual DateTime? StageThreeDate { get; set; }
            public abstract class stageThreeDate : PX.Data.BQL.BqlDateTime.Field<stageThreeDate> { }
            #endregion

            #region StageThreeDesc
            [PXDBString(60, IsUnicode = true, InputMask = "",
                BqlField = typeof(KGProjectStage.stageThreeDesc))]
            public virtual string StageThreeDesc { get; set; }
            public abstract class stageThreeDesc : PX.Data.BQL.BqlString.Field<stageThreeDesc> { }
            #endregion

            #region ProjectID
            [PXDBInt(BqlField = typeof(KGProjectStage.projectID))]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region InventoryID
            [PXDBInt(BqlField = typeof(InventoryItem.inventoryID))]
            public virtual int? InventoryID { get; set; }
            public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
            #endregion

            #region InventoryCD
            [PXDBString(IsUnicode = true, InputMask = "",
                BqlField = typeof(InventoryItem.inventoryCD))]
            [PXUIField(DisplayName = "Inventory CD", IsReadOnly = true)]
            public virtual string InventoryCD { get; set; }
            public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
            #endregion

            #region InventoryDesc
            [PXDBString(IsUnicode = true, InputMask = "",
                BqlField = typeof(InventoryItem.descr))]
            [PXUIField(DisplayName = "Inventory Desc", IsReadOnly = true)]
            public virtual string InventoryDesc { get; set; }
            public abstract class inventoryDesc : PX.Data.BQL.BqlString.Field<inventoryDesc> { }
            #endregion

            #region unbound

            #region Budget Amount
            [PXDecimal()]
            [PXUIField(DisplayName = "Budget Amount", IsReadOnly = true)]
            public virtual Decimal? BudgetAmount { get; set; }
            public abstract class budgetAmount : PX.Data.BQL.BqlDecimal.Field<budgetAmount> { }
            #endregion

            #region Stage One
            #region SOneQty
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage One Qty", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal,"0.0")]
            public virtual Decimal? SOneQty { get; set; }
            public abstract class sOneQty : PX.Data.BQL.BqlDecimal.Field<sOneQty> { }
            #endregion

            #region SOneAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage One Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SOneAmt { get; set; }
            public abstract class sOneAmt : PX.Data.BQL.BqlDecimal.Field<sOneAmt> { }
            #endregion

            #region SOnePerBudget
            [PXString()]
            [PXUIField(DisplayName = "Stage One Per Budget", IsReadOnly = true)]
            [PXUnboundDefault("0%")]
            public virtual string SOnePerBudget { get; set; }
            public abstract class sOnePerBudget : PX.Data.BQL.BqlString.Field<sOnePerBudget> { }
            #endregion

            #region SOneIncAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage One Inc. Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SOneIncAmount { get; set; }
            public abstract class sOneIncAmount : PX.Data.BQL.BqlDecimal.Field<sOneIncAmount> { }
            #endregion
            #endregion

            #region Stage Two
            #region STwoQty
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage Two Qty", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? STwoQty { get; set; }
            public abstract class sTwoQty : PX.Data.BQL.BqlDecimal.Field<sTwoQty> { }
            #endregion

            #region STwoAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage Two Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? STwoAmt { get; set; }
            public abstract class sTwoAmt : PX.Data.BQL.BqlDecimal.Field<sTwoAmt> { }
            #endregion

            #region STwoPerBudget
            [PXString()]
            [PXUIField(DisplayName = "Stage Two Per Budget", IsReadOnly = true)]
            [PXUnboundDefault("0%")]
            public virtual string STwoPerBudget { get; set; }
            public abstract class sTwoPerBudget : PX.Data.BQL.BqlString.Field<sTwoPerBudget> { }
            #endregion

            #region STwoIncAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage Two Inc. Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? STwoIncAmount { get; set; }
            public abstract class sTwoIncAmount : PX.Data.BQL.BqlDecimal.Field<sTwoIncAmount> { }
            #endregion
            #endregion

            #region Stage Three
            #region SThreeQty
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage Three Qty", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SThreeQty { get; set; }
            public abstract class sThreeQty : PX.Data.BQL.BqlDecimal.Field<sThreeQty> { }
            #endregion

            #region SThreeAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage Three Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SThreeAmt { get; set; }
            public abstract class sThreeAmt : PX.Data.BQL.BqlDecimal.Field<sThreeAmt> { }
            #endregion

            #region SThreePerBudget
            [PXString()]
            [PXUIField(DisplayName = "Stage Three Per Budget", IsReadOnly = true)]
            [PXUnboundDefault("0%")]
            public virtual string SThreePerBudget { get; set; }
            public abstract class sThreePerBudget : PX.Data.BQL.BqlString.Field<sThreePerBudget> { }
            #endregion

            #region SThreeIncAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Stage Three Inc. Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SThreeIncAmount { get; set; }
            public abstract class sThreeIncAmount : PX.Data.BQL.BqlDecimal.Field<sThreeIncAmount> { }
            #endregion
            #endregion

            #region Now
            #region NowQty
            [PXDecimal()]
            [PXUIField(DisplayName = "Now Qty", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? NowQty { get; set; }
            public abstract class nowQty : PX.Data.BQL.BqlDecimal.Field<nowQty> { }
            #endregion

            #region NowAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Now Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? NowAmt { get; set; }
            public abstract class nowAmt : PX.Data.BQL.BqlDecimal.Field<nowAmt> { }
            #endregion

            #region NowPerBudget
            [PXString()]
            [PXUIField(DisplayName = "Now Per Budget", IsReadOnly = true)]
            [PXUnboundDefault("0%")]
            public virtual string NowPerBudget { get; set; }
            public abstract class nowPerBudget : PX.Data.BQL.BqlString.Field<nowPerBudget> { }
            #endregion

            #region NowIncAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "Now Inc. Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? NowIncAmount { get; set; }
            public abstract class nowIncAmount : PX.Data.BQL.BqlDecimal.Field<nowIncAmount> { }
            #endregion
            #endregion

            #region Remaining
            #region RePerBudget
            [PXString(IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Budget(%)", IsReadOnly = true)]
            [PXUnboundDefault("0%")]
            public virtual string RePerBudget { get; set; }
            public abstract class rePerBudget : PX.Data.BQL.BqlString.Field<rePerBudget> { }
            #endregion

            #region Remaining Cost
            [PXDecimal()]
            [PXUIField(DisplayName = "Remaining Cost", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? RemainingCost { get; set; }
            public abstract class remainingCost : PX.Data.BQL.BqlDecimal.Field<remainingCost> { }
            #endregion
            #endregion

            #endregion

        }
        #endregion
        #endregion

    }
}