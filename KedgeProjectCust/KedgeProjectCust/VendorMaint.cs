using System;
using System.Collections.Generic;
using PX.Data;
using Branch = PX.SM.Branch;
using Kedge.DAC;
using PX.Objects.PO;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Objects.RQ;
using PX.Objects.CR;

namespace PX.Objects.AP
{
    /**
     * ====2021-02-09 : 11947 ====Alton
     * 1.新增一個Tab(OrderHistory), Grid, 條列該供應商相關的合約, Order by POOrder.OrderDate降冪排列
     * 2.欄位如下(全部readonly)
     *    POOrder.ProjectID
     *    POOrder.OrderTyope
     *    POOrder.OrderNbr --> HyperLink to PO301000/SC301000
     *    POOrder.OrderDate
     *    POOrder.OrderDesc
     *    POOrder.Status
     *    POOrder.CuryOrderTotal
     *    KGContractEvaluation.Score --> 第一次評鑑分數 KGContractEvaluation.EvalPhase = 1, hyperlink 到 kg303002(readonly), 可參考計價單KGContractEvaluation.ContractEvaluationCD的hyperlink
     *    KGContractEvaluation.Score --> 第二次評鑑分數 KGContractEvaluation.EvalPhase = 2, hyperlink 到 kg303002(readonly)\
     * 3.Grid新增一個Action(ViewBidding),使用者可以highlight某一個分包契約, 按下ViewBidding, 開啟新視窗(RQ503000), 帶出該分包契約對應請購單的投標資料. 如果點選的是零星合約, 則直接跳出alert dialog, 顯示"零星合約, 無投標資料"
     *
     * =====2021-04-07 ======= Louis
     * 1. change FirstScore/SecondScore to Decimal
     * **/
    public class VendorMaint_Extension : PXGraphExtension<VendorMaint>
    {
        public PXSelectReadonly<KGContractEvaluation,
                                Where<KGContractEvaluation.vendorID, Equal<Current<Vendor.bAccountID>>>> ContractEval;

        public PXSelectReadonly<OrderHistory,
                                Where<OrderHistory.vendorID, Equal<Current<Vendor.bAccountID>>>> OrderHistorys;
        #region Events
        protected virtual void _(Events.RowSelected<LocationExtAddress> e)
        {
            LocationExtAddress row = e.Row;
            PXUIFieldAttribute.SetRequired<LocationExtAddress.taxRegistrationID>(Base.BaseLocations.Cache, true);
        }
        protected virtual void _(Events.RowPersisting<LocationExtAddress> e)
        {
            LocationExtAddress row = e.Row;
            if (row == null) return;
            if (row.TaxRegistrationID == null)
                Base.BaseLocations.Cache.RaiseExceptionHandling<LocationExtAddress.taxRegistrationID>(e.Row, row.TaxRegistrationID, new PXSetPropertyException("此欄位不可為空!"));


        }
        #endregion

        #region Button
        public PXAction<OrderHistory> ViewBidding;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewBidding()
        {
            POOrder d = OrderHistorys.Current;
            if (d == null || d.RQReqNbr == null) return;

            RQBiddingProcess graph = PXGraph.CreateInstance<RQBiddingProcess>();
            graph.Document.Current = graph.Document.Search<RQRequisition.reqNbr>(d.RQReqNbr);
            if (graph.Document.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "RQ Bidding Process")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }

        }

        #endregion

        #region HyperLink
        public PXAction<OrderHistory> ViewOrder;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewOrder()
        {
            POOrder d = OrderHistorys.Current;
            if (d == null) return;
            POOrderEntry graph;
            String message = "";
            if (d.OrderType == POOrderType.RegularSubcontract)
            {
                graph = PXGraph.CreateInstance<SubcontractEntry>();
                message = "Subcontract Entry";
            }
            else
            {
                graph = PXGraph.CreateInstance<POOrderEntry>();
                message = "PO Order Entry";
            }
            graph.Document.Current = graph.Document.Search<POOrder.orderNbr>(d.OrderNbr, new object[] { d.OrderType });
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Document.Current != null)
            {
                throw new PXRedirectRequiredException(graph, message)
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }

        #endregion

        #region TableView - OrderHistory
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select<POOrder>), Persistent = false)]
        public class OrderHistory : POOrder
        {
            #region FirstScore
            [PXDecimal(2)]
            [PXUIField(DisplayName = "First Score")]
            [PXUnboundDefault(
                typeof(Search<KGContractEvaluation.score,
                    Where<KGContractEvaluation.orderNbr, Equal<Current<POOrder.orderNbr>>,
                        And<KGContractEvaluation.orderType, Equal<Current<POOrder.orderType>>,
                            And<IsNull<KGContractEvaluation.evalPhase, int_1>, Equal<int_1>>>>>)
                )]
            public virtual decimal? FirstScore { get; set; }
            public abstract class firstScore : PX.Data.BQL.BqlDecimal.Field<firstScore> { }
            #endregion

            #region SecondScore
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Second Score")]
            [PXUnboundDefault(
                typeof(Search<KGContractEvaluation.score,
                    Where<KGContractEvaluation.orderNbr, Equal<Current<orderNbr>>,
                        And<KGContractEvaluation.orderType, Equal<Current<orderType>>,
                            And<KGContractEvaluation.evalPhase, Equal<int_2>>>>>)
                )]
            public virtual decimal? SecondScore { get; set; }
            public abstract class secondScore : PX.Data.BQL.BqlDecimal.Field<secondScore> { }
            #endregion

            #region const
            public const int Int_1 = 1;
            public const int Int_2 = 2;
            public class int_1 : PX.Data.BQL.BqlInt.Constant<int_1> { public int_1() : base(Int_1) {; } }
            public class int_2 : PX.Data.BQL.BqlInt.Constant<int_2> { public int_2() : base(Int_2) {; } }
            #endregion
        }
        #endregion
    }
}