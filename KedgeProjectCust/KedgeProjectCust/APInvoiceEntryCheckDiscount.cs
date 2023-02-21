using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Kedge
{
    public class APInvoiceEntryCheckDiscountExt : PXGraphExtension<APInvoiceEntryBillSummaryExt,APInvoiceEntry_Extension, APInvoiceEntryRetainage, APInvoiceEntry>
    {
        #region Select View 
        [PXCopyPasteHiddenView]
        public PXFilter<CheckDiscountFilter> CheckDiscountFilters;
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public PXSelectJoin<CheckDiscountValuationDetail,
            InnerJoin<KGValuation,On<KGValuation.valuationID,Equal<CheckDiscountValuationDetail.valuationID>>>,
            Where<CheckDiscountValuationDetail.orderNbr, Equal<Current<CheckDiscountFilter.orderNbr>>,
                And<CheckDiscountValuationDetail.contractID, Equal<Current<CheckDiscountFilter.contractID>>,
                        And<CheckDiscountValuationDetail.aPInvoiceNbr,IsNull,
                            And<CheckDiscountValuationDetail.aPInvoiceDate,IsNull,
                                And<CheckDiscountValuationDetail.pricingType,Equal<word.d>,
                                    //2021/11/03 add mantis :0012258 只能抓到票貼
                                    And<KGValuation.valuationType,Equal<KGValuationTypeStringList.three>>>>>>>> CheckDiscounts;
        #endregion

        #region Action
        public PXAction<APInvoice> addCheckDiscount;
        [PXUIField(DisplayName = "Add Check Discount", MapEnableRights = PXCacheRights.Select,
                   MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddCheckDiscount(PXAdapter adapter)
        {
            APInvoice master = Base.Document.Current;
            try
            {
                CheckDiscountFilter filter = CheckDiscountFilters.Current;
                filter.VendorID = master.VendorID;
                filter.ContractID = Base2.getProjectID();
                CheckDiscountFilters.Update(CheckDiscountFilters.Current);
                WebDialogResult result = this.CheckDiscounts.AskExt(true);
                if (result == WebDialogResult.OK)
                {
                    return AddCheckDiscountLines(adapter);
                }
                else if (result == WebDialogResult.Cancel)
                {
                    Base2.deductionDetails.Cache.Clear();
                    Base2.deductionDetails.Cache.ClearQueryCache();
                }
                else
                {
                    Base2.deductionDetails.Cache.Clear();
                    Base2.deductionDetails.Cache.ClearQueryCache();
                }
            }
            catch (Exception e)
            {
                Base2.deductionDetails.Cache.Clear();
                Base2.deductionDetails.Cache.ClearQueryCache();
                CheckDiscountFilter filter = CheckDiscountFilters.Current;
                filter.VendorID = master.VendorID;
                filter.ContractID = Base2.getProjectID();
                filter.OrderNbr = Base2.getPoNbr();
                CheckDiscountFilters.Update(CheckDiscountFilters.Current);
                throw e;
            }
            return adapter.Get();
        }

        public PXAction<APInvoice> addCheckDiscountLines;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select,
                              MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddCheckDiscountLines(PXAdapter adapter)
        {
            foreach (CheckDiscountValuationDetail checkDiscountLine in CheckDiscounts.Select())
            {
                if (checkDiscountLine.Selected != true)
                {
                    continue;
                }

                if (!checkDeductionLine(checkDiscountLine))
                {
                    continue;
                }
                PXGraph graph = new PXGraph();
                KGValuation kgValuation = PXSelectReadonly<KGValuation,
                           Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>.Select(graph, checkDiscountLine.ValuationID);
                KGDeductionAPTran dedBillLine = (KGDeductionAPTran)createKGDeductionAPTranbyDis(checkDiscountLine);
                dedBillLine.Qty = checkDiscountLine.Qty;
                dedBillLine.UnitCost = checkDiscountLine.UnitPrice;
                dedBillLine.CuryUnitCost = checkDiscountLine.UnitPrice;

                dedBillLine.CuryLineAmt = checkDiscountLine.Amount;
                dedBillLine.CuryTranAmt = checkDiscountLine.Amount;
                dedBillLine.IsManageFee = false;
                dedBillLine.TranDesc = kgValuation.ValuationCD + "-" + Base2.getKGValuationType(kgValuation.ValuationType) + "-" + kgValuation.Description;
                //採購收獲號碼
                //dedBillLine.ReceiptNbr = kgValuation.ValuationCD;

                PMCostBudget pmCostBudget = Base2.getPMCostBudget(dedBillLine.ProjectID, dedBillLine.InventoryID);
                if (pmCostBudget != null)
                {
                    dedBillLine.TaskID = pmCostBudget.TaskID;
                    dedBillLine.CostCodeID = pmCostBudget.CostCodeID;
                }

                Base2.deductionAPTranDetails.Insert(dedBillLine);
                if (checkDiscountLine.ManageFeeAmt != null && checkDiscountLine.ManageFeeAmt > 0)
                {
                    KGDeductionAPTran manageFree = (KGDeductionAPTran)createKGDeductionAPTranbyDis(checkDiscountLine);
                    manageFree.Qty = 1;
                    manageFree.TranDesc = kgValuation.ValuationCD + "-" + Base2.getKGValuationType(kgValuation.ValuationType) + "-" + "Management fee" + "-" + kgValuation.Description;
                    manageFree.UnitCost = checkDiscountLine.ManageFeeAmt;
                    manageFree.CuryLineAmt = checkDiscountLine.ManageFeeAmt;
                    manageFree.CuryTranAmt = checkDiscountLine.ManageFeeAmt;
                    manageFree.CuryUnitCost = checkDiscountLine.ManageFeeAmt;

                    manageFree.CuryTaxableAmt = checkDiscountLine.ManageFeeAmt;
                    manageFree.TaxableAmt = checkDiscountLine.ManageFeeAmt;
                    manageFree.CuryTaxAmt = checkDiscountLine.ManageFeeTaxAmt;
                    manageFree.TaxAmt = checkDiscountLine.ManageFeeTaxAmt;
                    manageFree.IsManageFee = true;

                    //manageFree.ReceiptNbr = kgValuation.ValuationCD;
                    KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
                    if (setUp != null)
                    {
                        //modify by louis 20200527
                        manageFree.InventoryID = setUp.KGManageInventoryID;
                        pmCostBudget = Base2.getPMCostBudget(manageFree.ProjectID, setUp.KGManageInventoryID);
                    }
                    else
                    {
                        pmCostBudget = null;
                    }

                    if (pmCostBudget != null)
                    {
                        manageFree.TaskID = pmCostBudget.TaskID;
                        manageFree.CostCodeID = pmCostBudget.CostCodeID;
                    }

                    manageFree = Base2.deductionAPTranDetails.Insert(manageFree);
                }
            }
            CheckDiscounts.Cache.Clear();
            CheckDiscounts.Cache.ClearQueryCache();
            return adapter.Get();
        }
        #endregion

        #region Event 
        protected virtual void _(Events.RowSelected<APInvoice> e)
        {
            APInvoice row = e.Row;
            if (row == null) return;
            addCheckDiscount.SetVisible(false);
        }
        #endregion

        #region Method
        public KGDeductionAPTran createKGDeductionAPTranbyDis(CheckDiscountValuationDetail checkDiscountLine)
        {
            PXCache deductionCache = Base2.deductionAPTranDetails.Cache;
            KGDeductionAPTran dedBillLine = (KGDeductionAPTran)deductionCache.CreateInstance();
            //get PO Order Line DAC
            PXGraph graph = new PXGraph();
            /*
            POLine poLine = PXSelectReadonly<POLine,
                            Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                            And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>.Select(graph, checkDiscountLine.OrderNbr, 1);*/
            //checkDiscountLine.LineNbr;
            dedBillLine.PONbr = checkDiscountLine.OrderNbr;
            //checkDiscountLine.LineNbr
            //dedBillLine.POLineNbr = 1;
            dedBillLine.POOrderType = checkDiscountLine.OrderType;
            dedBillLine.InventoryID = checkDiscountLine.InventoryID;
            //dedBillLine.TranDesc = poLine.TranDesc;

            //dedBillLine.AccountID = poLine.ExpenseAcctID;
            //ExpenseAcctID-DESCR
            //dedBillLine.SubID = poLine.ExpenseSubID;
            dedBillLine.ProjectID = checkDiscountLine.ContractID;
            // dedBillLine.TaskID = poLine.TaskID;
            // dedBillLine.CostCodeID = poLine.CostCodeID;
            //dedBillLine.TaxCategoryID = poLine.TaxCategoryID;
            dedBillLine.ValuationID = checkDiscountLine.ValuationID;
            dedBillLine.ValuationType = "C";
            dedBillLine.CuryTaxableAmt = checkDiscountLine.Amount;
            dedBillLine.CuryTaxAmt = checkDiscountLine.TaxAmt;
            dedBillLine.CuryTranAmt = checkDiscountLine.Amount;
            dedBillLine.RetainagePct = 0;
            dedBillLine.CuryRetainageAmt = 0;
            dedBillLine.UOM = checkDiscountLine.Uom;
            return dedBillLine;

        }
        public bool checkDeductionLine(CheckDiscountValuationDetail checkDiscountLine)
        {
            foreach (KGDeductionAPTran dedLine in Base2.deductionAPTranDetails.Select())
            {
                int valuationID = 0;
                if (dedLine.ValuationID != null)
                {
                    valuationID = dedLine.ValuationID.Value;
                }
                if (valuationID.Equals(checkDiscountLine.ValuationID))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region CheckDiscountValuationDetail View
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<KGValuationDetail,
            InnerJoin<KGValuation, On<KGValuation.valuationID, Equal<KGValuationDetail.valuationID>>>,
                Where<KGValuation.valuationType, Equal<KGValuationTypeStringList.three>,
                Or<KGValuation.status, Equal<KGValuationStatusStringList.c>>>>
            ), Persistent = false)]
        public class CheckDiscountValuationDetail : KGValuationDetail
        {
        }
        #endregion

        #region FilterTable
        [Serializable]
        public class CheckDiscountFilter : IBqlTable
        {
            #region ContractID
            [PXInt()]
            [PXUIField(DisplayName = "Project")]
            [PXSelector(typeof(Search2<PMProject.contractID,
                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                    Equal<PMProject.contractID>>>>,
                    Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                    , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false, DescriptionField = typeof(PMProject.description))]
            public virtual int? ContractID { get; set; }
            public abstract class contractID : IBqlField { }
            #endregion

            #region OrderNbr
            [PXString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "Purchase Order", Enabled = false)]
            [PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
              And<Match<Vendor, Current<AccessInfo.userName>>>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<POOrder.orderType, Equal<Optional<POOrder.orderType>>,
                 And<Vendor.bAccountID, IsNotNull, And<POLine.projectID, Equal<Current<CheckDiscountFilter.contractID>>,
                 And<POOrder.orderType, In3<POOrderType.regularSubcontract, POOrderType.regularOrder>>>>>,
              Aggregate<GroupBy<POOrder.orderNbr, GroupBy<POOrder.orderType>>>>), Filterable = true, DescriptionField = typeof(POOrder.orderDesc))]
            public virtual string OrderNbr { get; set; }
            public abstract class orderNbr : IBqlField { }
            #endregion

            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor")]
            [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]

            public virtual int? VendorID { get; set; }
            public abstract class vendorID : IBqlField { }
            #endregion

            /*  
            #region ValuationType
            [PXString(15)]
            [PXUIField(DisplayName = "Valuation Type")]
            [PXStringList(new string[]{"0","1", "2" },new string[] { "代扣代付","罰款", "一般扣款"})]
            public virtual string ValuationType { get; set; }
            public abstract class valuationType : IBqlField { }
            #endregion
            */

            #region ValuationDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Valuation Date From")]
            public virtual DateTime? ValuationDateFrom { get; set; }
            public abstract class valuationDateFrom : IBqlField { }
            #endregion

            #region ValuationDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Valuation Date To")]
            public virtual DateTime? ValuationDateTo { get; set; }
            public abstract class valuationDateTo : IBqlField { }
            #endregion
        }
        #endregion
    }
}
