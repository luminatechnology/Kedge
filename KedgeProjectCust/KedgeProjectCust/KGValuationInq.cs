using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CR;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.IN;

namespace Kedge
{
    public class KGValuationInq : PXGraph<KGValuationInq>
    {
        #region Filter
        public PXSelect<KGValuation> Valuations;
        public PXFilter<ValuationFilter> ValuationFilter;
        public PXSelectJoin<KGValuation,
               InnerJoin<KGValuationDetail,
                   On<KGValuation.valuationID,
                   Equal<KGValuationDetail.valuationID>>,
               InnerJoin<POOrder,
               On<POOrder.orderNbr,
               Equal<KGValuationDetail.orderNbr>>>>,
               Where<KGValuation.status, Equal<word.c>>> AllValuation;
        protected virtual IEnumerable allValuation()
        {
            PXSelectJoin<KGValuation,
               InnerJoin<KGValuationDetail,
                   On<KGValuation.valuationID,
                   Equal<KGValuationDetail.valuationID>>,
               InnerJoin<POOrder,
               On<POOrder.orderNbr,
               Equal<KGValuationDetail.orderNbr>>>>,
               Where<KGValuation.status, Equal<word.c>>> query =
            new PXSelectJoin<KGValuation,
               InnerJoin<KGValuationDetail,
                   On<KGValuation.valuationID,
                   Equal<KGValuationDetail.valuationID>>,
               InnerJoin<POOrder,
               On<POOrder.orderNbr,
               Equal<KGValuationDetail.orderNbr>>>>,
               Where<KGValuation.status, Equal<word.c>>>(this);
            // Adding filtering conditions to the query
            ValuationFilter filter = ValuationFilter.Current;
            if (filter.ContractID != null)
                query.WhereAnd<Where<KGValuation.contractID,
                    Equal<Current<ValuationFilter.contractID>>>>();
            if (filter.OrderNbrFrom != null)
                query.WhereAnd<Where<KGValuationDetail.orderNbr,
                    GreaterEqual<Current<ValuationFilter.orderNbrFrom>>>>();
            if (filter.OrderNbrTo != null)
                query.WhereAnd<Where<KGValuationDetail.orderNbr,
                    LessEqual<Current<ValuationFilter.orderNbrTo>>>>();
            if (filter.ValuationDateFrom != null)
                query.WhereAnd<Where<KGValuation.valuationDate,
                    GreaterEqual<Current<ValuationFilter.valuationDateFrom>>>>();
            if (filter.ValuationDateTo != null)
                query.WhereAnd<Where<KGValuation.valuationDate,
                    LessEqual<Current<ValuationFilter.valuationDateTo>>>>();
            if (filter.ValuationType != null)
                query.WhereAnd<Where<KGValuation.valuationType,
                    Equal<Current<ValuationFilter.valuationType>>>>();
            if (filter.PricingType != null)
                query.WhereAnd<Where<KGValuationDetail.pricingType,
                    Equal<Current<ValuationFilter.pricingType>>>>();
            if (filter.Vendor != null)
                query.WhereAnd<Where<POOrder.vendorID,
                    Equal<Current<ValuationFilter.vendor>>>>();
            if (filter.Status != null)
                query.WhereAnd<Where<KGValuationDetail.status,
                    Equal<Current<ValuationFilter.status>>>>();
            return query.Select();
        }
        #endregion

        #region Event
        protected virtual void KGValuation_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGValuation row = AllValuation.Current;
            if (row == null) return;
            PXUIFieldAttribute.SetEnabled(sender, row, false);
            PXResultset<KGValuationDetail> searchdetail = PXSelect<KGValuationDetail,
                           Where<KGValuationDetail.valuationID,
                               Equal<Required<KGValuationDetail.valuationID>>>>
                               .Select(this, row.ValuationID);
            foreach (KGValuationDetail valuationdetail in searchdetail)
            {
                
                PXResultset<POLine> set = PXSelect<POLine,
                               Where<POLine.orderNbr,
                                   Equal<Required<POLine.orderNbr>>>>
                                   .Select(this, valuationdetail.OrderNbr);
                foreach (POLine poline in set)
                {
                    valuationdetail.InvDesc = poline.TranDesc;
                }

                //加上InventoryID+InventoryDesc顯示
                
                PXResultset<InventoryItem> setinventoryitem = PXSelect<InventoryItem,
                    Where<InventoryItem.inventoryID
                    , Equal<Required<InventoryItem.inventoryID>>>>
                    .Select(this, valuationdetail.InventoryID);
                foreach (InventoryItem inventoryItem in setinventoryitem)
                {
                    valuationdetail.InvDesc = inventoryItem.Descr;
                }
                
            }

            
        }
        #endregion

        #region Link
        //ValuationCD超連結
        public PXAction<ValuationFilter> ViewValuation;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Valuation", Visible =false)]
        protected virtual void viewValuation()
        {
            KGValuation row = AllValuation.Current;
                // Creating the instance of the graph
                KGValuationEntry graph = PXGraph.CreateInstance<KGValuationEntry>();
                // Setting the current product for the graph
                graph.Valuations.Current = graph.Valuations.Search<KGValuation.valuationCD>(
                row.ValuationCD);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Valuations.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "Valuation Details");
            }
            
            
        }
        #endregion
    }

    [Serializable]
    public class ValuationFilter : IBqlTable
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
                typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region OrderNbrFrom
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Purchase Order From")]
        [PXSelector(typeof(Search5<KGValuationDetail.orderNbr,
            InnerJoin<POOrder, On<POOrder.orderNbr,
                Equal<KGValuationDetail.orderNbr>>,
            InnerJoin<KGValuation, On<KGValuation.valuationID,
                Equal<KGValuationDetail.valuationID>>>>,
            Where<KGValuation.status, Equal<word.c>>,
            Aggregate<GroupBy<KGValuationDetail.orderNbr>>>),
            typeof(POOrder.orderType),
            typeof(KGValuationDetail.orderNbr),
            typeof(POOrder.orderDate),
            typeof(POOrder.status),
            typeof(POOrder.vendorID),
            typeof(POOrder.vendorID_Vendor_acctName),
            typeof(POOrder.orderDesc)//,
                                     //DescriptionField = typeof(POOrder.orderDesc)
       )]
        public virtual string OrderNbrFrom { get; set; }
        public abstract class orderNbrFrom : IBqlField { }
        #endregion

        #region OrderNbrTo
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Purchase Order To")]
        [PXSelector(typeof(Search5<KGValuationDetail.orderNbr,
            InnerJoin<POOrder, On<POOrder.orderNbr,
                Equal<KGValuationDetail.orderNbr>>,
            InnerJoin<KGValuation, On<KGValuation.valuationID,
                Equal<KGValuationDetail.valuationID>>>>,
            Where<KGValuation.status, Equal<word.c>>,
            Aggregate<GroupBy<KGValuationDetail.orderNbr>>>),
            typeof(POOrder.orderType),
            typeof(KGValuationDetail.orderNbr),
            typeof(POOrder.orderDate),
            typeof(POOrder.status),
            typeof(POOrder.vendorID),
            typeof(POOrder.vendorID_Vendor_acctName),
            typeof(POOrder.orderDesc)//,
                                     //DescriptionField = typeof(POOrder.orderDesc)
       )]
        public virtual string OrderNbrTo { get; set; }
        public abstract class orderNbrTo : IBqlField { }
        #endregion

        #region ValuationType
        [PXString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Valuation Type")]
        [PXStringList(
            new string[]
            {
                "0",
                "1",
                "2"
            },
            new string[]
            {
                "代扣代付",
                "罰款",
                "一般扣款"
            })]
        public virtual string ValuationType { get; set; }
        public abstract class valuationType : IBqlField { }
        #endregion

        #region PricingType
        [PXString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Pricing Type")]
        [PXStringList(new string[]
            {
                "A",
                "D",
            },
            new string[]
            {
                "加款",
                "扣款"
            })]
        public virtual string PricingType { get; set; }
        public abstract class pricingType : IBqlField { }
        #endregion

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

        #region Vendor
        [PXInt()]
        [PXUIField(DisplayName = "Vendor")]
        [POVendor(Visibility = PXUIVisibility.SelectorVisible,
            DescriptionField = typeof(Vendor.acctName), 
            CacheGlobal = true, Filterable = true)]
        public virtual int? Vendor { get; set; }
        public abstract class vendor : IBqlField { }
        #endregion

        #region Status
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Status")]
        [PXStringList(new string[] {  "C", "S", "V", "B" },
            new string[] { "CONFIRM", "PENDING SIGN", "VENDOR CONFIRM", "BILLED" })]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
        #endregion

    }
}