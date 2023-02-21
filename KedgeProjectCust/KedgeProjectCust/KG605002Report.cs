using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.PO;
using PX.Objects.PM;
using PX.Objects.IN;
using PX.Objects.GL;
using System.Collections.Generic;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AP;


/***
 * ====2021-06-21 :12103 ====Alton 
 * 1.結算完工成本的數量, 請不要計算分包契約原始數量, 只要累計訂單變更(類型為專案結算)結算的變更數量.
 * 2.結算完工成本的複價, 請不要計算分包契約原始金額, 只要累計訂單變更(類型為專案結算)結算的變更金額.
 * ====2021-06-22:口頭====Alton
 * 估驗數量&復價由最後一筆改成累計
 * ====2021-06-28 :12112 ====Alton
 * 1.報表最前面麻煩新增預算專案任務(PMCostBudget.ProjectTaskID)及專案任務描述(PMCostBudget.ProjectTaskID_description)
 * 2.結算數量/複價, 請判斷如果有結算的訂單變更, 請顯示結算的數量/金額, 如果沒有進行過結算, 請顯示分報契約的數量/複價
 * 3.如果該筆分包契約找到多筆結算的訂單變更, 請依照訂單變更RefNbr排序, 用最大(如CO00001, CO00002, 請只抓CO00002)的那筆結算訂單變更, 不要累加. 報表訂單變更編號也請只顯示(CO00002)
 * 4.訂單變更編號在報表的label請改成結算單號.
 * 5.如果結算訂單變更狀態還不是Open, 請不要在報表中呈現, 現在會有是否結算是'N', 但是有訂單變更編號
 * 
 * **/
namespace Kedge
{
    /**
     * <summary>
     * KG605001預算發包請款一覽表(reportID=KG605005)
     * KG605002預算發包請款結算一覽表(reportID=KG605006)
     * </summary>
     * */
    public class KG605002Report : PXGraph<KG605002Report>
    {

        //public PXSave<MasterTable> Save;
        //public PXCancel<MasterTable> Cancel;

        #region view
        public PXFilter<KG605002Filter> MasterView;
        public PXSelect<KG605002> DetailsView;
        #endregion

        #region Button

        #region 執行報表
        public PXAction<KG605002Filter> ProcessAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "執行報表")]
        protected void processAction()
        {
            KG605002Report graph = PXGraph.CreateInstance<KG605002Report>();
            String screenID = graph.Accessinfo.ScreenID;
            bool isFinalRp = "KG.60.50.02" == screenID;

            //結算ReportID:KG605006
            //無結算ReportID:KG605005
            String reportID = isFinalRp ? "KG605006" : "KG605005";

            //刪除7天前的舊資料
            DateTime businessDate = (DateTime)graph.Accessinfo.BusinessDate;
            DeleteOldData(businessDate, 7);

            //開始產生報表
            DateTime now = System.DateTime.Now;
            string BatchID = graph.Accessinfo.UserName + now.ToString("yyyyMMddHHmmss") + reportID;
            KG605002Filter filter = MasterView.Current;
            if (filter != null)
            {
                if (filter.ProjectID == null)
                {
                    String msg = "專案不可為空值!";
                    MasterView.Cache.RaiseExceptionHandling<KG605002Filter.projectID>(
                        filter, filter.ProjectID, new PXSetPropertyException(msg));
                    return;
                }
                if (filter.DeadLine == null)
                {
                    String msg = "到期日不可為空值!";
                    MasterView.Cache.RaiseExceptionHandling<KG605002Filter.deadLine>(
                        filter, filter.DeadLine, new PXSetPropertyException(msg));
                    return;
                }

                Dictionary<string, string> patams = new Dictionary<string, string>();
                DateTime date = (DateTime)filter.DeadLine;


                Contract contract = GetContract(filter.ProjectID);
                //取得Title資訊
                string p_contractCD = contract.ContractCD;
                string p_description = contract.Description;
                //string p_printDate = now.ToString("yyyy/MM/dd HH:mm:ss");


                //產生ReportData
                string p_totalAmt = CreateReportData(BatchID, filter.ProjectID, filter.DeadLine, isFinalRp).ToString();
                //傳入報表參數
                patams["P_BatchID"] = BatchID;
                patams["P_ContractCD"] = p_contractCD;
                patams["P_Description"] = p_description;
                patams["P_DeadLine"] = date.ToString("yyyy/MM/dd");
                patams["P_TotalAmt"] = p_totalAmt;
                throw new PXReportRequiredException(patams, reportID, "Report");
            }
        }
        #endregion
        #endregion

        #region Method

        /**
         * <summary>
         * 取得Contract資訊
         * </summary>
         */
        private Contract GetContract(int? ProjectID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID, Equal<Required<POOrder.projectID>>>>
                .Select(this, ProjectID);
        }

        /**
         * <summary>
         * 產生ReportData
         * </summary>
         */
        private decimal? CreateReportData(String BatchID, int? projectID, DateTime? deadLine, bool isFinalRp)
        {
            decimal? totalAmt = 0;
            int seq = 0;
            PXResultset<Segment> costCodeSplit = GetSegment("COSTCODE");
            PXResultset<Segment> taskSplit = GetSegment("PROTASK");
            KG605002 header = null;
            foreach (PXResult<PMBudget, PMTask, PMCostCode, InventoryItem, Account, CSAttributeDetail> data in GetDetailHeader(projectID))
            {
                //subDetail參數
                int? costCodeID = ((PMBudget)data).CostCodeID;
                int? inventoryID = ((PMBudget)data).InventoryID;
                int? taskID = ((PMBudget)data).ProjectTaskID;
                bool isNewGroup = true;
                //取得分包契約
                PXResultset<POLine> po_rs = GetSubDetailByPO(deadLine, projectID, taskID, costCodeID, inventoryID);
                //當分包契約有資料時，採用分包契約資訊
                if (po_rs.Count > 0)
                {
                    foreach (PXResult<POLine, POOrder, Contract, Vendor> poData in po_rs)
                    {
                        KG605002 item = NewKG605002(data, BatchID, seq++, isNewGroup, costCodeSplit, taskSplit);
                        //紀錄header資訊
                        if (isNewGroup)
                        {
                            header = item;
                        }
                        isNewGroup = false;
                        item.ProjectCode = ((Contract)poData).ContractCD;
                        //發包合約編號
                        item.OrderNbr = ((POOrder)poData).OrderNbr;
                        //發包合約項目
                        item.OrderDesc = ((POOrder)poData).OrderDesc;
                        //發包承攬商
                        item.VendorName = ((Vendor)poData).AcctName;
                        //發包數量
                        item.OrderQty = ((POLine)poData).OrderQty;
                        //發包單價
                        item.CuryUnitCost = ((POLine)poData).CuryUnitCost;
                        //發包復價
                        item.CuryLineAmt = ((POLine)poData).CuryLineAmt;
                        //行描述
                        item.TranDesc = ((POLine)poData).TranDesc;

                        /* 改抓計價單 20200121
                        //估驗數量
                        item.BilledQty = ((POLine)poData).BilledQty;
                        //估驗復價
                        item.CuryBilledAmt = ((POLine)poData).CuryBilledAmt;
                        */
                        //計價單參數
                        string orderNbr = ((POOrder)poData).OrderNbr;
                        string orderType = ((POOrder)poData).OrderType;
                        int? branchID = ((POOrder)poData).BranchID; ;
                        int? vendorID = ((POOrder)poData).VendorID; ;
                        int? vendorLocationID = ((POOrder)poData).VendorLocationID;
                        int? lineNbr = ((POLine)poData).LineNbr;
                        PXResultset<APTran> ap_t =
                            GetAPInvoice(orderNbr, orderType, branchID, vendorID, vendorLocationID, projectID, inventoryID, costCodeID, taskID, lineNbr);

                        //2021-06-22 add by alton 估驗數量&復價由最後一筆改成累計
                        decimal billedQty = 0m;
                        decimal curyBilledAmt = 0m;
                        foreach (PXResult<APTran, APRegister> apData in ap_t)
                        {
                            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>((APRegister)apData);
                            if (((APRegister)apData).Hold == false && apRegisterExt.UsrFlowStatus == "D")
                            {
                                //估驗數量
                                //item.BilledQty = ((APTran)apData).Qty;
                                billedQty += ((APTran)apData)?.Qty ?? 0m;
                                //估驗復價
                                //item.CuryBilledAmt = ((APTran)apData).CuryLineAmt;
                                curyBilledAmt += ((APTran)apData)?.CuryLineAmt ?? 0m;
                            }
                        }
                        item.BilledQty = billedQty;
                        item.CuryBilledAmt = curyBilledAmt;

                        if (isFinalRp)
                        {
                            //取得分包契約專案結算
                            PXResultset<PMChangeOrderLine> poc_rs = GetSubDetailByPOC(orderNbr, orderType, lineNbr);
                            //Mark by Alton mantis:12103 user做專案結算時的分包契約僅拿來做紀錄，因此不計算原始金額數量
                            //2021-06-28 結算數量/複價, 請判斷如果有結算的訂單變更, 請顯示結算的數量/金額, 如果沒有進行過結算, 請顯示分報契約的數量/複價
                            decimal? finalQty = item.OrderQty ?? 0;//起始值為：<發包契約>數量
                            decimal? finalAmount = item.CuryLineAmt ?? 0;//起始值為：<發包契約>復價
                            String isFinal = "N";
                            String refNbr = "";
                            if (poc_rs.Count > 0)
                            {
                                //2021-06-28 如果該筆分包契約找到多筆結算的訂單變更, 請依照訂單變更RefNbr排序, 用最大(如CO00001, CO00002, 請只抓CO00002)的那筆結算訂單變更, 不要累加. 報表訂單變更編號也請只顯示(CO00002)
                                //foreach (PMChangeOrderLine pmcol in poc_rs)
                                //{
                                //    refNbr = pmcol.RefNbr;
                                //    finalQty += (pmcol.Qty == null ? 0 : pmcol.Qty);
                                //    finalAmount += (pmcol.Amount == null ? 0 : pmcol.Amount);
                                //    isFinal = "Y";
                                //}
                                //只抓一筆
                                PMChangeOrderLine pmcol = poc_rs;
                                refNbr = pmcol.RefNbr;
                                finalQty = pmcol.Qty ?? 0;
                                finalAmount = pmcol.Amount ?? 0;
                                isFinal = "Y";
                            }

                            item.IsFinal = isFinal;
                            item.RefNbr = refNbr;
                            item.FinalQty = finalQty;
                            item.FinalAmount = finalAmount;
                            //計算加總
                            totalAmt += item.FinalAmount;
                        }
                        //計算餘額
                        SetBalance(header, item);
                        //寫入DB
                        DetailsView.Update(item);
                    }
                    //重新更新header
                }
                //否則改用成本預算
                else
                {
                    KG605002 item = NewKG605002(data, BatchID, seq++, isNewGroup, costCodeSplit, taskSplit);
                    if (isNewGroup)
                    {
                        header = item;
                    }
                    isNewGroup = false;
                    if (isFinalRp)
                    {
                        //edit alton 20191114 當[是否結算] 為N時 完工數量&復價 需為0
                        decimal? finalQty = 0;
                        decimal? finalAmount = 0;
                        PXResultset<PMChangeOrderBudget> pmcob_rs = GetSubDetailByPOB(projectID, taskID, costCodeID, inventoryID);
                        String isFinal = "N";
                        foreach (PMChangeOrderBudget pmcob in pmcob_rs)
                        {

                            if (isFinal == "N")
                            {//edit alton 20191114 當[是否結算] 為N時 完工數量&復價 需為0
                                finalQty = (item.RevisedQty == null ? 0 : item.RevisedQty);//起始值為：<預算>數量
                                finalAmount = (item.CuryRevisedAmount == null ? 0 : item.CuryRevisedAmount);//起始值為：<預算>復價
                            }

                            finalQty += (pmcob.Qty == null ? 0 : pmcob.Qty);
                            finalAmount += (pmcob.Amount == null ? 0 : pmcob.Amount);
                            isFinal = "Y";
                        }
                        item.IsFinal = isFinal;
                        item.FinalQty = finalQty;
                        item.FinalAmount = finalAmount;
                        //計算加總
                        totalAmt += item.FinalAmount;
                    }
                    //計算餘額
                    SetBalance(header, item);
                    //寫入DB
                    DetailsView.Update(item);
                }
                //重新更新Header資訊
                if (header != null)
                {
                    DetailsView.Update(header);
                }
            }

            base.Persist();
            return totalAmt;
        }

        /**
         * <summary>
         * 表頭資訊
         * </summary>
         * */
        private KG605002 NewKG605002(PXResult<PMBudget, PMTask, PMCostCode, InventoryItem, Account, CSAttributeDetail> data, String BatchID, int seq, bool isNewGroup, PXResultset<Segment> costCodeSplit, PXResultset<Segment> taskSplit)
        {
            PMCostCode pMCostCode = (PMCostCode)data;
            InventoryItem inventoryItem = (InventoryItem)data;
            CSAttributeDetail cSAttributeDetail = (CSAttributeDetail)data;
            Account account = (Account)data;
            PMBudget pMBudget = (PMBudget)data;
            PMTask pMTask = (PMTask)data;

            KG605002 item = new KG605002();
            item.BatchID = BatchID;
            item.BatchSeq = seq;
            item.IsFinal = "N";
            if (isNewGroup)
            {
                PMBudgetExt pmbe = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pMBudget);
                //專案任務
                item.ProjectTaskCD = DoSplit(taskSplit, pMTask.TaskCD);
                //專案任務描述
                item.ProjectTaskDesc = pMTask.Description;
                //計價單元
                item.CostCodeCD = DoSplit(costCodeSplit, pMCostCode.CostCodeCD);
                //計價單元描述
                item.CostCodeDesc = pMCostCode.Description;
                //工項代碼
                item.InventoryCD = inventoryItem.InventoryCD;
                //工項代碼
                item.InvetoryDesc = pmbe.UsrInventoryDesc;
                //會計科目
                item.AccountCD = account.AccountCD;
                //單位
                item.Uom = pMBudget.UOM;
                //發包類型
                //item.UsrInvPrTypeName = pmbe.UsrInvPrType;
                item.UsrInvPrTypeName = cSAttributeDetail.Description;
                //預算數量
                item.RevisedQty = pMBudget.RevisedQty;
                //預算單價
                item.CuryUnitRate = pMBudget.CuryUnitRate;
                //預算復價
                item.CuryRevisedAmount = pMBudget.CuryRevisedAmount;
                //預算餘額起始值為預算復價
                item.Balance = item.CuryRevisedAmount;
            }
            return item;
        }

        /**
         * <summary>
         * 計算餘額
         * </summary>
         * <param name="header">header資訊</param>
         * <param name="item">明細資訊</param>
         */
        private void SetBalance(KG605002 header, KG605002 item)
        {
            if (item.OrderNbr != null)
            {//發包契約
                if (header.Uom == "式")
                {//單位="式"：預算餘額-發包契約複價
                    header.Balance -= item.CuryLineAmt;
                }
                else
                {//單位!="式"：預算餘額-(預算單價 * 發包數量)
                    header.Balance -= (header.CuryUnitRate * item.OrderQty);
                }
            }
            else
            {//非發包
                if (item.IsFinal == "Y")
                {//已結算
                    //預算餘額-結算復價
                    header.Balance -= item.FinalAmount;
                }
            }
        }

        /**
         * <summary>
         * 斷點處理
         * </summary>
         * **/
        private String DoSplit(PXResultset<Segment> splits, string strVal)
        {
            String returnVal = "";
            String endSplit = "";
            int index = 0;
            if (strVal != null)
            {
                foreach (Segment split in splits)
                {
                    returnVal += endSplit + strVal.Substring(index, (int)split.Length);
                    endSplit = split.Separator;
                    index += (int)split.Length;
                    if (index > strVal.Length)
                    {
                        index = strVal.Length;
                    }
                }
                return returnVal;
            }
            return strVal;
        }


        /**
         * <summary>
         * 取得Segment
         * </summary>
         */
        private PXResultset<Segment> GetSegment(string dimensionID)
        {
            return
                PXSelect<Segment,
                    Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>,
                    OrderBy<Asc<Segment.segmentID>>
                >.Select(this, dimensionID);
        }

        /**
         * <summary>
         * 取得DetailHeader資訊
         * </summary>
         */
        private PXResultset<PMBudget> GetDetailHeader(int? ProjectID)
        {
            return
                PXSelectJoin<
                    PMBudget,
                    InnerJoin<PMTask, On<PMBudget.projectTaskID, Equal<PMTask.taskID>, And<PMBudget.projectID, Equal<PMTask.projectID>>>,
                    InnerJoin<PMCostCode, On<PMBudget.costCodeID, Equal<PMCostCode.costCodeID>>,
                    InnerJoin<InventoryItem, On<PMBudget.inventoryID, Equal<InventoryItem.inventoryID>>,
                    InnerJoin<Account, On<PMBudget.accountGroupID, Equal<Account.accountGroupID>>,
                    LeftJoin<CSAttributeDetail, On<PMBudgetExt.usrInvPrType, Equal<CSAttributeDetail.valueID>, And<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>>>>
                    >>>>,
                    Where<PMBudget.projectID, Equal<Required<PMBudget.projectID>>,
                    And<PMBudget.type, Equal<Required<PMBudget.type>>>
                    >,
                    OrderBy<Asc<PMCostCode.costCodeCD, Asc<PMBudget.inventoryID, Asc<Account.accountCD>>>>

              >
                .Select(this, "INVPRTYPE", ProjectID, "E");
        }

        /**
         * <summary>
         * 取得SubDetail-分包契約資訊
         * </summary>
         */
        private PXResultset<POLine> GetSubDetailByPO(DateTime? deadLine, int? projectID, int? taskID, int? costCodeID, int? inventoryID)
        {
            return
                PXSelectJoin<
                    POLine,
                    InnerJoin<POOrder, On<POLine.orderNbr, Equal<POOrder.orderNbr>, And<POLine.orderType, Equal<POOrder.orderType>>>,
                    InnerJoin<Contract, On<POLine.projectID, Equal<Contract.contractID>>,
                    LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
                    >>>,
                    Where<POOrder.orderDate, LessEqual<Required<POOrder.orderDate>>,//deadLine
                    And<POLine.projectID, Equal<Required<POLine.projectID>>,//projectID
                    And<POLine.taskID, Equal<Required<POLine.taskID>>,//taskID
                    And<POLine.inventoryID, Equal<Required<POLine.inventoryID>>,//inventoryID
                    And<POLine.costCodeID, Equal<Required<POLine.costCodeID>>//costCodeID
                    >>>>>,
                    OrderBy<Asc<Contract.contractCD, Asc<POOrder.orderNbr>>>

              >
                .Select(this, deadLine, projectID, taskID, inventoryID, costCodeID);
        }

        /**
         * <summary>
         * 取得SubDetail-分包契約-專案結算資訊
         * </summary>
         */
        private PXResultset<PMChangeOrderLine> GetSubDetailByPOC(string orderNbr, string orderType, int? lineNbr)
        {
            return
                PXSelectJoin<
                    PMChangeOrderLine,
                    InnerJoin<PMChangeOrder, On<PMChangeOrderLine.refNbr, Equal<PMChangeOrder.refNbr>>>,
                    Where<PMChangeOrderLine.pOOrderNbr, Equal<Required<PMChangeOrderLine.pOOrderNbr>>,//orderNbr
                    And<PMChangeOrderLine.pOOrderType, Equal<Required<PMChangeOrderLine.pOOrderType>>,//orderType
                    And<PMChangeOrderLine.pOLineNbr, Equal<Required<PMChangeOrderLine.pOLineNbr>>,//lineNbr
                    And<PMChangeOrder.classID, Equal<Required<PMChangeOrder.classID>>,//'專案結算'
                    And<PMChangeOrder.status, Equal<ChangeOrderStatus.open>//'O' -Open //20210628 要判斷status //20191112 status 不判斷
                    >>>>>,
                    OrderBy<Desc<PMChangeOrderLine.refNbr>>
              >
                .Select(this, orderNbr, orderType, lineNbr, "專案結算");
        }

        /**
         * <summary>
         * 取得SubDetail- 成本預算-專案結算
         * </summary>
         */
        private PXResultset<PMChangeOrderBudget> GetSubDetailByPOB(int? projectID, int? taskID, int? costCodeID, int? inventoryID)
        {
            return
                PXSelectJoin<
                    PMChangeOrderBudget,
                    InnerJoin<PMChangeOrder, On<PMChangeOrderBudget.refNbr, Equal<PMChangeOrder.refNbr>>>,
                    Where<PMChangeOrderBudget.projectID, Equal<Required<PMChangeOrderBudget.projectID>>,//projectID
                    And<PMChangeOrderBudget.projectTaskID, Equal<Required<PMChangeOrderBudget.projectTaskID>>,//taskID
                    And<PMChangeOrderBudget.inventoryID, Equal<Required<PMChangeOrderBudget.inventoryID>>,//inventoryID
                    And<PMChangeOrderBudget.costCodeID, Equal<Required<PMChangeOrderBudget.costCodeID>>,//costCodeID
                    And<PMChangeOrder.classID, Equal<Required<PMChangeOrder.classID>>,//'專案結算'
                    And<PMChangeOrder.status, Equal<Required<PMChangeOrder.status>>//'O' -Open
                    >>>>>>
              >
                .Select(this, projectID, taskID, inventoryID, costCodeID, "專案結算", "O");
        }

        /**
         * <summary>
         * 取得計價單明細的 數量、成本小計
         * </summary>
         */
        private PXResultset<APTran> GetAPInvoice(string orderNbr, string orderType, int? branchID, int? vendorID, int? vendorLocationID, int? projectID, int? inventoryID, int? costCodeID, int? taskID, int? poLineNbr)
        {
            return
                PXSelectJoin<
                    APTran,
                    InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTran.refNbr>>>,
                    Where<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>,//OrderNbr
                    And<APRegisterExt.usrPOOrderType, Equal<Required<APRegisterExt.usrPOOrderType>>,//OrderType
                    And<APRegister.branchID, Equal<Required<APRegister.branchID>>,//branchID
                    And<APRegister.vendorID, Equal<Required<APRegister.vendorID>>,//vendorID
                    And<APRegister.vendorLocationID, Equal<Required<APRegister.vendorLocationID>>,//vendorLocationID
                    And<APRegister.docType, Equal<Required<APRegister.docType>>,// = 'INV'
                    And<APTranExt.usrValuationType, Equal<Required<APTranExt.usrValuationType>>,// = 'B'
                    And<APTran.projectID, Equal<Required<APTran.projectID>>,//projectID
                    And<APTran.inventoryID, Equal<Required<APTran.inventoryID>>,//inventoryID
                    And<APTran.costCodeID, Equal<Required<APTran.costCodeID>>,//costCodeID
                    And<APTran.taskID, Equal<Required<APTran.taskID>>,//taskID
                    And<APTran.pOLineNbr, Equal<Required<APTran.pOLineNbr>>//poLineNbr
                    >>>>>>>>>>>>
              >
                .Select(this, orderNbr, orderType, branchID, vendorID, vendorLocationID, "INV", "B"
                    , projectID, inventoryID, costCodeID, taskID, poLineNbr);
        }

        /**
         * <summary>
         * 刪除舊資料
         * </summary>
         * <param name="date">當前時間</param>
         * <param name="day">預刪除幾天前的資料</param>
         */
        private void DeleteOldData(DateTime date, int day)
        {
            DateTime deadLine = date.AddDays(-day);
            PXResultset<KG605002> set = PXSelect<KG605002, Where<KG605002.createdDateTime, LessEqual<Required<KG605002.createdDateTime>>>>.Select(this, deadLine);
            foreach (KG605002 old in set)
            {
                DetailsView.Delete(old);
            }
            base.Persist();
        }

        #endregion

        #region filter
        [Serializable]
        public class KG605002Filter : IBqlTable
        {
            #region ProjectID
            [PXInt()]
            [PXUIField(DisplayName = "ProjectID", Required = true)]

            [PXSelector(typeof(Search2<PMProject.contractID,
                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                    Equal<PMProject.contractID>>>>,
                    Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                    , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : IBqlField { }
            #endregion

            #region DeadLine
            [PXDate()]
            [PXUIField(DisplayName = "Dead Line", Required = true)]
            [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual DateTime? DeadLine { get; set; }
            public abstract class deadLine : IBqlField { }
            #endregion
        }
        #endregion

    }
}