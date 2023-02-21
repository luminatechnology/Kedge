using System;
using System.Web;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using Kedge.DAC;
using PX.Objects.CT;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.RQ;

//PM308000
namespace PX.Objects.PM
{
    /***
     * ====2021-03-12: 11978====Alton
     * 1. 當訂單變更狀態為Open, 訂單類別為"專案結算", 請Disable"過帳"action
     * 2. 當訂單變更狀態為Open, 訂單類別不是"專案結算", 簽核狀態(UsrApproveStatus)不是"已核可"('AP'), 請Disable"過帳"action
     * ↑↑以上改為跳錯誤訊息↑↑
     * 
     * ====2021-06-10:12011====Alton
     * 契約新增申請中的Line, 單價請新增field_updated事件, 連動計算金額(KedgeProjectCust)
     * 
     * ====2021-09-14:12230====Alton
     * 1.成本預算變更, 存檔時, 請檢核工料編號(PMChangeOrderCostBudget.InventoryID)為必填, 如果有null或是N/A, 請顯示錯誤訊息"成本預算工料編號不得為空或是N/A"
     * 2.已承諾變更, 存檔時, 請檢核Commitment Nbr.(PMChangeOrderLine.POOrderNbr)為必填, 如果為空的, 請顯示錯誤訊息"已承諾契約編號(Commitment Nbr.)不得為空的"
     * 
     * ====2021-11-16:12268====Alton
     * 1.PMChangeOrderCostBudget 新增一個unbound 欄位UsrCostCodeDesc, 帶出PMChangeOrderCostBudget.CostCodeID的描述. 以便取代系統標準的CostCodeID_description.
     * 
     * ====2022-04-07====Jeff
     * Per spec [Kedge] - 成本預算
     * **/
    public class ChangeOrderEntry_Extension : PXGraphExtension<ChangeOrderEntry>
    {
        public const string  UOM_式 = "式";
        public const string  CannotGreaterThanRemain_Budget = "變更預算不可低於剩餘可發包預算";
        public const string  CannotGreaterThanRemain_Commitment = "變更已承諾不可超過剩餘可計價";
        public const string  BudgetExceeded = "變更預算/已承諾超過限制";
        public const decimal OneNth = 1m/10000; // m => decimal

        public bool isSpecialUOM = false;

        #region Selects
        //public PXSelect<PMChangeOrder> Document;
        //public PXSelectJoin<PMChangeOrderRevenueBudget,
        //    LeftJoin<PMRevenueBudget, On<PMRevenueBudget.projectID, Equal<PMChangeOrderRevenueBudget.projectID>,
        //        And<PMRevenueBudget.projectTaskID, Equal<PMChangeOrderRevenueBudget.projectTaskID>,
        //        And<PMRevenueBudget.accountGroupID, Equal<PMChangeOrderRevenueBudget.accountGroupID>,
        //        And<PMRevenueBudget.inventoryID, Equal<PMChangeOrderRevenueBudget.inventoryID>,
        //        And<PMRevenueBudget.costCodeID, Equal<PMChangeOrderRevenueBudget.costCodeID>>>>>>>,
        //    Where<PMChangeOrderRevenueBudget.refNbr, Equal<Current<PMChangeOrder.refNbr>>,
        //    And<PMChangeOrderRevenueBudget.type, Equal<GL.AccountType.income>>>> RevenueBudget;
        //public PXSelectJoin<PMChangeOrderLine,
        //    LeftJoin<POLinePM, On<POLinePM.orderType, Equal<PMChangeOrderLine.pOOrderType>,
        //        And<POLinePM.orderNbr, Equal<PMChangeOrderLine.pOOrderNbr>,
        //        And<POLinePM.lineNbr, Equal<PMChangeOrderLine.pOLineNbr>>>>>,
        //    Where<PMChangeOrderLine.refNbr, Equal<Current<PMChangeOrder.refNbr>>>> Details;

        //AddItem Dialog
        public PXSelect<PMBudget, Where<PMBudget.projectID, Equal<Current<PMBudget.projectID>>>> KGNewChangeOrderLineDialog;

        public PXSelect<PMChangeOrderBudget,
            Where<PMChangeOrderBudget.refNbr, Equal<Current<PMChangeOrder.refNbr>>,
            And<PMChangeOrderBudget.type, Equal<GL.AccountType.expense>>>> Budget2;

        [PXCopyPasteHiddenView]
        public PXSelect<PMCostBudget> CurrentAvailCostBudget;
        #endregion

        #region Delegate Data Views
        /// <summary>
        /// 已承諾變更明細新增一個按鍵（由預算項新增），讓使用者方便挑選預算項目，視窗中的預算項目要包含相同訂單變更中成本預算變更的數量/複價 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable currentAvailCostBudget()
        {
            HashSet<BudgetKeyTuple> existing = new HashSet<BudgetKeyTuple>();

            foreach (PXResult<PMChangeOrderLine, PMBudget> res in CommitmentBudget())
            {
                PMChangeOrderLine line = res;

                existing.Add(BudgetKeyTuple.Create(new PMChangeOrderBudget() { ProjectID = line.ProjectID, ProjectTaskID = line.TaskID, AccountGroupID = null, InventoryID = line.InventoryID, CostCodeID = line.CostCodeID }));
            }

            foreach (PMBudget budget in Base.GetCostBudget())
            {
                if (budget.Type != GL.AccountType.Expense || budget.InventoryID == 1282) // 1282 = <N/A>
                    continue;

                if (existing.Contains(BudgetKeyTuple.Create(new PMChangeOrderBudget() { ProjectID = budget.ProjectID, ProjectTaskID = budget.TaskID, AccountGroupID = null, InventoryID = budget.InventoryID, CostCodeID = budget.CostCodeID })))
                    budget.Selected = true;

                yield return budget;
            }
        }
        #endregion

        #region 檔案
        public PXSelect<KGChangeOrderFile,
            Where<KGChangeOrderFile.refNbr, Equal<Current<PMChangeOrder.refNbr>>>> KGChangeOrderFiles;
        public PXSelect<KGNewChangeOrder,
            Where<KGNewChangeOrder.refNbr, Equal<Current<PMChangeOrder.refNbr>>>,
            OrderBy<Asc<KGNewChangeOrder.createdDateTime>>> KGNewChangeOrders;
        public PXSelect<KGNewChangeOrderLine,
            Where<KGNewChangeOrderLine.newChangeOrderID, Equal<Current<KGNewChangeOrder.newChangeOrderID>>>,
            OrderBy<Asc<KGNewChangeOrderLine.createdDateTime>>> KGNewChangeOrderLines;
        #endregion

        #region Agent Flow Integration
        public override void Initialize()
        {
            base.Initialize();
            Base.action.AddMenuAction(webServIntegration);
        }
        //CS201010
        public PXSetup<KGSetUp> KGSetup;
        public PXSelect<KGFlowChangeManagement, Where<KGFlowChangeManagement.orderChangeRefNbr, Equal<Current<PMChangeOrder.refNbr>>>> KGFlowChangeManagements;
        public PXSelect<KGFlowBudChgApplyDetail, Where<KGFlowBudChgApplyDetail.bcaUID, Equal<Current<KGFlowChangeManagement.bcaUID>>>> KGFlowSubAccDetails;
        public PXSelect<KGFlowPutRvn, Where<KGFlowPutRvn.bcaUID, Equal<Current<KGFlowPutRvn.bcaUID>>>> KGFlowPutRvns;
        public PXSelect<KGFlowPutChg, Where<KGFlowPutChg.rvnUID, Equal<Current<KGFlowPutChg.rvnUID>>>> KGFlowPutChgs;
        public PXSelect<KGFlowBudChgApplyNew, Where<KGFlowBudChgApplyNew.bcauid, Equal<Current<KGFlowChangeManagement.bcaUID>>>> KGFlowBudChgApplyNews;
        public PXSelect<KGFlowBudChgApplyNewDetail, Where<KGFlowBudChgApplyNewDetail.bcnuid, Equal<Current<KGFlowBudChgApplyNew.bcnuid>>>> KGFlowBudChgApplyNewDetails;

        public PXSelect<KGFlowUploadFile> KGFlowUploadFiles;

        //送審
        public PXAction<PMChangeOrder> webServIntegration;
        [PXUIField(DisplayName = "AgentFlow", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable WebServIntegration(PXAdapter adapter)
        {
            String message = doAgentFlow();
            if (message != null)
            {
                throw new PXException(message);
            }
            return adapter.Get();
        }

        public String doAgentFlow()
        {
            //using (PXTransactionScope ts = new PXTransactionScope())
            //{
            PMChangeOrder master = Base.Document.Current;
            Base.Actions.PressSave();
            deleteAgentFlowTempData(master.RefNbr);
            KGFlowChangeManagement kgFlowChangeManagement = createKGFlowChangeManagement();
            createKGFlowBudChgApplyDetail();
            createKGFlowPutRvn();
            //mark by Alton 20191203 因KGFlowPutRvn可能有多筆，因此改在createKGFlowPutRvn當中執行
            //createKGFlowPutChg(); 
            createKGFlowBudChgApplyNew();
            //createKGFlowUploadFile(kgFlowChangeManagement);

            //mark by Alton 20191210 不再走原廠的approval因此不用這段
            //EPApproval epApproval = updateEPApproval(master, kgFlowChangeManagement);
            Base.Actions.PressSave();
            KGFlowChangeManagement newkgFlowChangeManagement = KGFlowChangeManagements.Current;
            bool ckeck = agentFlow(newkgFlowChangeManagement, null);
            if (message != null)
            {
                return message;
            }

            if (!ckeck)
            {
                return message;
            }

            //ts.Complete();
            //}
            return null;
        }
        public EPApproval updateEPApproval(PMChangeOrder master, KGFlowChangeManagement kgFlowChangeManagement)
        {
            EPApproval epApproval = PXSelect<EPApproval,
                                Where<EPApproval.refNoteID,
                                Equal<Required<EPApproval.refNoteID>>, And<EPApproval.status, Equal<Required<EPApproval.status>>>>>
                                .Select(Base, master.NoteID, EPApprovalStatus.Pending);
            if (epApproval != null)
            {
                kgFlowChangeManagement.ApprovalID = epApproval.ApprovalID;
            }
            KGFlowChangeManagements.Update(kgFlowChangeManagement);
            return epApproval;
        }

        public KGFlowChangeManagement createKGFlowChangeManagement()
        {
            KGFlowChangeManagement kgFlowChangeManagement = (KGFlowChangeManagement)KGFlowChangeManagements.Cache.CreateInstance();
            //KGFlowSubAcc kglowAccs = new KGFlowSubAcc();
            PMChangeOrder master = Base.Document.Current;
            PMChangeOrderExt masterExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(master);

            PX.Objects.GL.Branch branch = PXSelect<PX.Objects.GL.Branch, Where<PX.Objects.GL.Branch.branchID, Equal<Required<AccessInfo.branchID>>>>.Select(Base, Base.Accessinfo.BranchID);

            if (branch != null)
            {
                kgFlowChangeManagement.DeptName = branch.AcctName;
            }
            //kgFlowChangeManagement.DeptName = Base.Accessinfo.CompanyName;
            Contract contract = PXSelect<Contract,
                            Where<Contract.contractID,
                            Equal<Required<Contract.contractID>>>>
                            .Select(Base, master.ProjectID);
            if (contract != null)
            {
                kgFlowChangeManagement.ProjectName = contract.Description;
                kgFlowChangeManagement.ProjectCode = contract.ContractCD;
            }
            kgFlowChangeManagement.OrderChangeRefNbr = master.RefNbr;
            kgFlowChangeManagement.ApplicationUser = getUsrName(master.CreatedByID);
            //kgFlowChangeManagement.ApplicationUser = ;
            kgFlowChangeManagement.ApplicationDate = master.Date;
            //??
            kgFlowChangeManagement.ChangTitle = master.Description;
            //??

            PMChangeOrderExt pmChangeOrderExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(master);
            //20200302 edit by alton EffectReason 改為 PMChangeOrder.Description  & Cmemo 改為 PMChangeOrder.UsrChangeReason
            //kgFlowChangeManagement.EffectReason = pmChangeOrderExt.UsrChangeReason;
            //kgFlowChangeManagement.Cmemo = master.Text;
            kgFlowChangeManagement.EffectReason = master.Description;
            kgFlowChangeManagement.Cmemo = pmChangeOrderExt.UsrChangeReason;

            //變更期別
            kgFlowChangeManagement.Num = masterExt.UsrOrderChangePeriod;

            kgFlowChangeManagement = KGFlowChangeManagements.Insert(kgFlowChangeManagement);
            return kgFlowChangeManagement;

        }
        public void createKGFlowBudChgApplyDetail()
        {
            int seq = 1;
            char pad = '0';
            foreach (PMChangeOrderCostBudget pmChangeOrderCostBudget in Base.CostBudget.Select())
            {
                //20201120 add by Alton(11805) unbound欄位會抓不到值的問題 
                Base.InitCostBudgetFields(pmChangeOrderCostBudget);

                KGFlowBudChgApplyDetail kgFlowBudChgApplyDetail = (KGFlowBudChgApplyDetail)KGFlowSubAccDetails.Cache.CreateInstance();
                InventoryItem inventoryItem = PXSelect<InventoryItem,
                            Where<InventoryItem.inventoryID,
                            Equal<Required<InventoryItem.inventoryID>>>>
                            .Select(Base, pmChangeOrderCostBudget.InventoryID);
                //add 20200210-項次為系統流水號
                //kgFlowBudChgApplyDetail.ItemNo = seq;
                PMCostCode costCode = PXSelect<PMCostCode,
                                         Where<PMCostCode.costCodeID,
                                         Equal<Required<PMCostCode.costCodeID>>>>
                                         .Select(Base, pmChangeOrderCostBudget.CostCodeID);

                PXResultset<Segment> costCodeSeq = PXSelect<Segment,
                                            Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>,
                                            OrderBy<Asc<Segment.segmentID>>>.Select(Base, "COSTCODE");



                kgFlowBudChgApplyDetail.ItemNo = DoSplit(costCodeSeq, costCode.CostCodeCD);
                kgFlowBudChgApplyDetail.PrintNo = (seq++.ToString()).PadLeft(8, pad);
                if (inventoryItem != null)
                {
                    //20200305 mark by alton manits:11540 itemName 改寫入 PMChangeOrderBudget.UsrInventoryDesc
                    //kgFlowBudChgApplyDetail.ItemName = inventoryItem.Descr;
                    kgFlowBudChgApplyDetail.PccesCode = inventoryItem.InventoryCD;
                }
                kgFlowBudChgApplyDetail.ItemUnit = pmChangeOrderCostBudget.UOM;
                PMCostBudget pmCostBudget = getPMCostBudget(pmChangeOrderCostBudget.CostCodeID, pmChangeOrderCostBudget.ProjectTaskID,
                                        pmChangeOrderCostBudget.ProjectID, pmChangeOrderCostBudget.InventoryID, pmChangeOrderCostBudget.AccountGroupID);
                if (pmCostBudget != null)
                {

                    kgFlowBudChgApplyDetail.ItemCost = pmCostBudget.CuryUnitRate;
                    kgFlowBudChgApplyDetail.ItemQty = pmCostBudget.RevisedQty;
                    kgFlowBudChgApplyDetail.ItemAmt = pmCostBudget.RevisedAmount;
                }
                else
                {
                    kgFlowBudChgApplyDetail.ItemNo = "新增";
                }
                //add 20200210 當資料為NULL 塞0
                kgFlowBudChgApplyDetail.ItemCost = kgFlowBudChgApplyDetail.ItemCost ?? 0;
                kgFlowBudChgApplyDetail.ItemQty = kgFlowBudChgApplyDetail.ItemQty ?? 0;
                kgFlowBudChgApplyDetail.ItemAmt = kgFlowBudChgApplyDetail.ItemAmt ?? 0;
                //20200616 louis 如果是一式項, 變更後的金額即為變更後的單價
                if (UOM_式.Equals(kgFlowBudChgApplyDetail.ItemUnit))
                {
                    kgFlowBudChgApplyDetail.ChgCost = pmChangeOrderCostBudget.RevisedAmount;
                }
                else
                {
                    kgFlowBudChgApplyDetail.ChgCost = pmChangeOrderCostBudget.Rate;
                }

                kgFlowBudChgApplyDetail.ChgQty = pmChangeOrderCostBudget.RevisedQty;
                kgFlowBudChgApplyDetail.ChgAmt = pmChangeOrderCostBudget.RevisedAmount;

                PMChangeOrderBudgetExt pmChangeOrderBudgetExt = PXCache<PMChangeOrderBudget>.GetExtension<PMChangeOrderBudgetExt>(pmChangeOrderCostBudget);
                kgFlowBudChgApplyDetail.ReasonKindCode = pmChangeOrderBudgetExt.UsrChangeReason;
                kgFlowBudChgApplyDetail.BelongKindCode = pmChangeOrderBudgetExt.UsrBelongKind;
                //20200305 add by alton manits:11540 itemName 改寫入 PMChangeOrderBudget.UsrInventoryDesc
                kgFlowBudChgApplyDetail.ItemName = pmChangeOrderBudgetExt.UsrInventoryDesc;

                KGFlowSubAccDetails.Insert(kgFlowBudChgApplyDetail);
            }
        }
        public void createKGFlowPutRvn()
        {
            PMChangeOrder master = Base.Document.Current;
            List<string> orderNbrs = new List<string>();
            foreach (PMChangeOrderLine pmChangeOrderLine in Base.Details.Select())
            {
                //避免重複
                if (orderNbrs.Contains(pmChangeOrderLine.POOrderNbr))
                {
                    continue;
                }
                PMChangeOrderLineExt lineExt = PXCache<PMChangeOrderLine>.GetExtension<PMChangeOrderLineExt>(pmChangeOrderLine);
                orderNbrs.Add(pmChangeOrderLine.POOrderNbr);

                POOrder poOrder = getPOOrderInPMChangeOrderLine(pmChangeOrderLine.POOrderType, pmChangeOrderLine.POOrderNbr);
                if (poOrder == null)
                {
                    continue;
                }
                KGFlowPutRvn kgFlowPutRvn = (KGFlowPutRvn)KGFlowPutRvns.Cache.CreateInstance();
                Contract contract = PXSelect<Contract,
                                Where<Contract.contractID,
                                Equal<Required<Contract.contractID>>>>
                                .Select(Base, poOrder.ProjectID);
                kgFlowPutRvn.SubCode = contract.ContractCD.Trim() + "-" + poOrder.OrderNbr;
                kgFlowPutRvn.SubName = poOrder.OrderDesc;
                Location location = PXSelect<Location,
                    Where<Location.bAccountID,
                    Equal<Required<Location.bAccountID>>>>
                    .Select(Base, poOrder.VendorID);
                if (location != null)
                {
                    kgFlowPutRvn.InvUID = location.TaxRegistrationID;
                }

                //modify by louis 20200213 使用Base查不到資料, 改用一般的Graph, 猜測可能是模組限制..
                PXGraph graph = new PXGraph();
                BAccount bAccount = PXSelect<BAccount,
                            Where<BAccount.bAccountID,
                            Equal<Required<BAccount.bAccountID>>>>
                            .Select(graph, poOrder.VendorID);
                if (bAccount != null)
                {
                    kgFlowPutRvn.Title = bAccount.AcctName;
                }
                kgFlowPutRvn.PutRvnNo = master.RefNbr;
                kgFlowPutRvn.InputDate = master.Date;
                kgFlowPutRvn.AddDay = master.DelayDays;

                kgFlowPutRvn.Num = lineExt.UsrChangePeriod;//期別
                string rvnUID = CS.AutoNumberAttribute.GetNextNumber(KGFlowPutRvns.Cache, kgFlowPutRvn, kgFlowPutRvn.Setup, Base.Accessinfo.BusinessDate);
                kgFlowPutRvn.RvnUID = rvnUID;
                KGFlowPutRvns.Insert(kgFlowPutRvn);
                //add by Alton 20191203
                createKGFlowPutChg(pmChangeOrderLine.POOrderNbr, rvnUID);
            }
        }

        public void createKGFlowPutChg(string orderNbr, string rvnUID)
        {
            PMChangeOrder master = Base.Document.Current;
            //int seq = 1;
            foreach (PMChangeOrderLine pmChangeOrderLine in Base.Details.Select())
            {
                PMChangeOrderLineExt pmChangeOrderLineExt = PXCache<PMChangeOrderLine>.GetExtension<PMChangeOrderLineExt>(pmChangeOrderLine);
                if (orderNbr != pmChangeOrderLine.POOrderNbr)
                {
                    continue;
                }
                KGFlowPutChg kgFlowPutChg = (KGFlowPutChg)KGFlowPutChgs.Cache.CreateInstance();
                kgFlowPutChg.RvnUID = rvnUID;
                //20200210-項次改為系統流水號
                //kgFlowPutChg.ItemNo = pmChangeOrderLine.InventoryID;
                //kgFlowPutChg.ItemNo = seq++;
                if (!"U".Equals(pmChangeOrderLine.LineType))
                {
                    kgFlowPutChg.ItemNo = "新增";
                }
                else
                {
                    kgFlowPutChg.ItemNo = Convert.ToString(pmChangeOrderLine.POLineNbr);
                }


                InventoryItem inventoryItem = PXSelect<InventoryItem,
                           Where<InventoryItem.inventoryID,
                           Equal<Required<InventoryItem.inventoryID>>>>
                           .Select(Base, pmChangeOrderLine.InventoryID);
                if (inventoryItem != null)
                {
                    //20200306 mark by alton mantis:11540 kgFlowPutChg.ItemName改寫入改PMChangeOrderLine.Description
                    //kgFlowPutChg.ItemName = inventoryItem.Descr;
                    kgFlowPutChg.PccesCode = inventoryItem.InventoryCD;
                }

                //20200306 add by alton mantis:11540 kgFlowPutChg.ItemName改寫入改PMChangeOrderLine.Description
                //202000629 edit by alton mantis:11621 kgFlowPutChg.ItemName改寫入改PMChangeOrderLine.UsrInventoryDesc
                kgFlowPutChg.ItemName = pmChangeOrderLineExt.UsrInventoryDesc;

                POLine poLine = getPOLine(pmChangeOrderLine.POLineNbr, pmChangeOrderLine.POOrderNbr, pmChangeOrderLine.POOrderType);
                if (poLine != null)
                {
                    //P.S: a = b ?? c 當 b為null 則 a = c 否則 a = b
                    kgFlowPutChg.PreCost = poLine.CuryUnitCost ?? 0;//修改前單價
                    kgFlowPutChg.PreQty = poLine.OrderQty ?? 0;//修改前數量
                    kgFlowPutChg.PreAmt = poLine.CuryLineAmt ?? 0;//修改前復價
                    kgFlowPutChg.ItemUnit = poLine.UOM;//單位
                }
                else
                {
                    kgFlowPutChg.PreCost = 0;//修改前單價
                    kgFlowPutChg.PreQty = 0;//修改前數量
                    kgFlowPutChg.PreAmt = 0;//修改前復價
                }

                //單位如果為空時，抓PMChangeOrderLine
                kgFlowPutChg.ItemUnit = kgFlowPutChg.ItemUnit ?? pmChangeOrderLine.UOM;

                //edit by alton 20191203 修改後應當為 修改前 - 修改

                kgFlowPutChg.ChgQty = (kgFlowPutChg.PreQty ?? 0) + (pmChangeOrderLine.Qty ?? 0);//修改後數量
                kgFlowPutChg.ChgAmt = (kgFlowPutChg.PreAmt ?? 0) + (pmChangeOrderLine.Amount ?? 0);//修改後復價

                //20200616 louis 如果是一式項, 變更後的金額即為變更後的單價
                if (UOM_式.Equals(kgFlowPutChg.ItemUnit))
                {
                    kgFlowPutChg.ChgCost = kgFlowPutChg.ChgAmt;//修改後單價
                }
                else
                {
                    kgFlowPutChg.ChgCost = pmChangeOrderLine.UnitCost ?? 0;//修改後單價
                }


                kgFlowPutChg.ReasonKindCode = pmChangeOrderLineExt.UsrChangeReason;
                kgFlowPutChg.BelongKindCode = pmChangeOrderLineExt.UsrBelongKind;
                KGFlowPutChgs.Insert(kgFlowPutChg);

            }

        }

        public void createKGFlowBudChgApplyNew()
        {
            foreach (KGNewChangeOrder header in KGNewChangeOrders.Select())
            {
                KGFlowBudChgApplyNew item = (KGFlowBudChgApplyNew)KGFlowBudChgApplyNews.Cache.CreateInstance();
                Vendor vendor = PXSelectorAttribute.Select<KGNewChangeOrder.vendorID>(KGNewChangeOrders.Cache, header) as Vendor;

                item.InvNo = header.TaxRegistrationID;//廠商統編
                item.Title = vendor.AcctName;//廠商名稱
                item.ItemName = header.OrderName;//契約名稱
                item.Memo = header.Description;//契約說明
                KGFlowBudChgApplyNews.Insert(item);
                createKGFlowBudChgApplyNewDetail(header.NewChangeOrderID);
            }
        }

        public void createKGFlowBudChgApplyNewDetail(int? newChangeOrderID)
        {
            foreach (KGNewChangeOrderLine line in getKGNewChangeOrderLine(newChangeOrderID))
            {
                KGFlowBudChgApplyNewDetail item = (KGFlowBudChgApplyNewDetail)KGFlowBudChgApplyNewDetails.Cache.CreateInstance();
                InventoryItem inventoryItem = PXSelectorAttribute.Select<KGNewChangeOrderLine.inventoryID>(KGNewChangeOrderLines.Cache, line) as InventoryItem;
                item.ItemNo = line.InventoryID;//項次
                item.ItemName = inventoryItem.Descr;//項目
                item.PccesCode = inventoryItem.InventoryCD;//編碼
                item.ItemUnit = line.Uom;//單位
                item.ItemCost = line.UnitCost ?? 0;//單價
                item.ItemQty = line.Qty ?? 0;//數量
                item.ItemAmt = line.Amount ?? 0;//複價
                KGFlowBudChgApplyNewDetails.Insert(item);
            }
        }

        public static PXResultset<POLine> getPOLine(int? lineNbr, String orderNbr, String orderType)
        {
            PXGraph graph = new PXGraph();
            PXResultset<POLine> set = PXSelect<POLine,
                    Where<POLine.lineNbr, Equal<Required<POLine.lineNbr>>,
                        And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>, And<POLine.orderType, Equal<Required<POLine.orderType>>>>
                        >>.Select(graph, lineNbr, orderNbr, orderType);

            return set;
        }

        //edit by Alton 20191203 PMChangeOrderLine 不一定只有一組Order
        public POOrder getPOOrderInPMChangeOrderLine(string orderType, string orderNbr)
        {
            POOrder poOrder = APInvoiceEntry_Extension.getPOOrder(orderType, orderNbr);
            return poOrder;
        }
        public PMCostBudget getPMCostBudget(int? costCodeID, int? projectTaskID, int? projectID, int? inventoryID, int? accountGroupID)
        {
            return PXSelect<PMCostBudget, Where<PMCostBudget.costCodeID, Equal<Required<PMCostBudget.costCodeID>>
                    , And<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>
                    , And<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                        And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>,
                        And<PMCostBudget.accountGroupID, Equal<Required<PMCostBudget.accountGroupID>>>>>>>>.
                    Select(Base, costCodeID, projectTaskID, projectID, inventoryID, accountGroupID);
        }

        public String getUsrName(Guid? id)
        {
            PXGraph graph = new PXGraph();
            PX.SM.Users user = PXSelect<PX.SM.Users,
                       Where<PX.SM.Users.pKID,
                       Equal<Required<PX.SM.Users.pKID>>>>
                       .Select(graph, id);

            if (user != null)
            {
                return user.FullName;
            }
            else
            {
                return null;
            }
        }
        public void createKGFlowUploadFile(KGFlowChangeManagement kgFlowChangeManagement)
        {
            foreach (KGChangeOrderFile kgChangeOrderFile in KGChangeOrderFiles.Select())
            {
                if (kgChangeOrderFile.NoteID == null)
                {
                    continue;
                }
                NoteDoc notedoc = PXSelect<NoteDoc,
                     Where<NoteDoc.noteID,
                     Equal<Required<NoteDoc.noteID>>>>
                     .Select(Base, kgChangeOrderFile.NoteID);
                if (notedoc != null)
                {

                    KGFlowUploadFile kgFlowUploadFile = (KGFlowUploadFile)KGFlowUploadFiles.Cache.CreateInstance();
                    kgFlowUploadFile.Fileid = notedoc.FileID;
                    //該檔案可能已經新增過了
                    KGFlowUploadFile checkFlowFile = PXSelect<KGFlowUploadFile,
                     Where<KGFlowUploadFile.fileid,
                     Equal<Required<KGFlowUploadFile.fileid>>>>
                     .Select(Base, notedoc.FileID);
                    if (checkFlowFile != null)
                    {
                        continue;
                    }
                    kgFlowUploadFile.FileType = kgChangeOrderFile.FileType;
                    kgFlowUploadFile.RefNo = kgFlowChangeManagement.BcaUID;
                    kgFlowUploadFile.Category = "CHG";
                    UploadFile uploadFile = PXSelect<UploadFile,
                        Where<UploadFile.fileID,
                        Equal<Required<UploadFile.fileID>>>>
                        .Select(Base, notedoc.FileID);
                    kgFlowUploadFile.FileName = uploadFile.Name; ;
                    string buildUtl = PXRedirectToFileException.BuildUrl(notedoc.FileID);
                    string Absouri = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;

                    buildUtl = buildUtl.Substring(1);
                    Absouri = Absouri.Substring(0, Absouri.IndexOf("/M"));
                    kgFlowUploadFile.FileLink = Absouri + buildUtl;
                    KGFlowUploadFiles.Insert(kgFlowUploadFile);
                }
            }
        }
        public bool agentFlow(KGFlowChangeManagement kgFlowChangeManagement, EPApproval epApproval)
        {
            PMChangeOrder master = Base.Document.Current;
            PMChangeOrderExt pmChangeOrderExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(master);
            message = null;
            String status = null;
            AgentFlowWebServiceUtil.createChangeManagementProcessKedgeURL(Base.Accessinfo.UserName, kgFlowChangeManagement.BcaUID, master.ProjectID, ref status, ref message);
            //此時要是錯誤 message 會有值，Status也塞好了
            if (message != null)
            {
                return false;
            }
            if (status != null && status.StartsWith("http"))
            {
                //mark by Alton 20191210 不再走原廠的approval因此不用這段
                //if (epApproval != null)
                //    {
                //        EPApprovalExt epApprovalExt = PXCache<EPApproval>.GetExtension<EPApprovalExt>(epApproval);
                //        epApprovalExt.UsrReturnUrl = status;
                //        Base.Approval.Update(epApproval);
                pmChangeOrderExt.UsrReturnUrl = status;
                pmChangeOrderExt.UsrKGFlowUID = kgFlowChangeManagement.BcaUID;
                pmChangeOrderExt.UsrApprovalStatus = "PA";//PA:送審中 
                master = Base.Document.Update(master);
                try
                {
                    if (AgentFlowWebServiceUtil.getIsAutoApproved(master.ProjectID) == true)
                    {
                        //master.Approved = true;
                        pmChangeOrderExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(master);
                        pmChangeOrderExt.UsrApprovalStatus = ApprovalStatusListAttribute.Approved;
                        Base.Document.Update(master);
                    }
                }
                catch (Exception e)
                {
                    message = "沒設定專案是否自動核准";
                    return false;
                }

                Base.Actions.PressSave();
                //}
            }
            else
            {
                message = "呼叫AgentFlow失敗:" + status;
            }
            return true;
        }

        //不會跑到
        /*
        protected virtual void PMChangeOrder_Status_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            PMChangeOrder master = (PMChangeOrder)e.Row; ;
            String status = (String)e.OldValue;
            if ((master.Hold == false || master.Hold == null)
                && ChangeOrderStatus.PendingApproval.Equals(master.Status) && APDocStatus.Hold.Equals(status))
            {
                
            }
        }*/
        String message = null;
        #endregion

        #region Actions
        #region ViewOriginalDocument
        public PXAction<APInvoice> ViewOriginalDocument;

        [PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        protected virtual IEnumerable viewOriginalDocument(PXAdapter adapter)
        {
            //參考資料
            //https://asiablog.acumatica.com/2017/08/redirecting-to-external-page-from-button.html
            PMChangeOrder master = Base.Document.Current;
            PMChangeOrderExt pmChangeOrderExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(master);
            throw new Exception("Redirect7:" + pmChangeOrderExt.UsrReturnUrl);
            return adapter.Get();
        }
        #endregion

        #region Dialog Buttons

        #region ShowNewChangeOrderDialog
        public PXAction<KGNewChangeOrder> showNewChangeOrderDialog;
        [PXUIField(DisplayName = "AddItem", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable ShowNewChangeOrderDialog(PXAdapter adapter)
        {
            if (KGNewChangeOrders.Current == null)
            {
                return adapter.Get();
            }
            try
            {
                WebDialogResult result = KGNewChangeOrderLineDialog.AskExt(true);
                if (result == WebDialogResult.OK)
                {
                    return DialogAddItem(adapter);
                }
            }
            catch (PXDialogRequiredException e)
            {
                KGNewChangeOrderLineDialog.Cache.Clear();
                KGNewChangeOrderLineDialog.Cache.ClearQueryCache();
                throw e;
            }
            return adapter.Get();
        }

        #region DialogAddItem
        public PXAction<KGNewChangeOrderLine> dialogAddItem;
        [PXUIField(DisplayName = "DialogAddItem", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable DialogAddItem(PXAdapter adapter)
        {
            PMChangeOrder header = Base.Document.Current;
            KGNewChangeOrder main = KGNewChangeOrders.Current;
            KGNewChangeOrderLineDialog.OrderByNew<OrderBy<Desc<PMBudget.selected>>>();
            foreach (PMBudget item in KGNewChangeOrderLineDialog.Select())
            {
                if (item.Selected == true)
                {
                    KGNewChangeOrderLine line = new KGNewChangeOrderLine();
                    line.VendorID = main.VendorID;
                    line.InventoryID = item.InventoryID;
                    line.Uom = item.UOM;
                    line.UnitCost = item.UnitPrice;
                    line.Qty = item.Qty;
                    line.Amount = line.UnitCost * line.Qty;
                    line.CostCodeID = item.CostCodeID;
                    line.ProjectID = header.ProjectID;
                    line.Taskid = item.ProjectTaskID;
                    KGNewChangeOrderLines.Insert(line);
                }
                else
                {
                    KGNewChangeOrderLineDialog.Cache.Clear();
                    KGNewChangeOrderLineDialog.Cache.ClearQueryCache();
                    return adapter.Get();
                }
            }
            KGNewChangeOrderLineDialog.Cache.Clear();
            KGNewChangeOrderLineDialog.Cache.ClearQueryCache();
            return adapter.Get();
        }
        #endregion

        #endregion

        #region AddCurCostBudget
        public PXAction<PMChangeOrder> addCurCostBudget;
        [PXUIField(DisplayName = "Select Budget Lines")]
        [PXButton]
        public IEnumerable AddCurCostBudget(PXAdapter adapter)
        {
            if (CurrentAvailCostBudget.View.AskExt() == WebDialogResult.OK)
            {
                AddSelectedCurCostBudget();
            }

            return adapter.Get();
        }
        #endregion

        #region AppendSelectedCurCostBudget
        public PXAction<PMChangeOrder> appendSelectedCurCostBudget;
        [PXUIField(DisplayName = "Add Lines")]
        [PXButton]
        public IEnumerable AppendSelectedCurCostBudget(PXAdapter adapter)
        {
            AddSelectedCurCostBudget();

            return adapter.Get();
        }
        #endregion

        #endregion
        #endregion

        #region Delegate Methods
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            PMChangeOrder master = Base.Document.Current;
            if (master == null)
            {
                baseMethod();
            }
            else
            {
                if (!beforeSaveCheck())
                {
                    return;
                }
                baseMethod();
            }
        }

        public delegate void ReleaseDocumentDelegate(PMChangeOrder doc);
        [PXOverride]
        public void ReleaseDocument(PMChangeOrder doc, ReleaseDocumentDelegate baseMethod)
        {
            PMChangeOrderExt docExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(doc);
            String message = null;
            if (doc.Status != ChangeOrderStatus.Open)
            {
                message = "狀態須為「開啟」";
            }
            if (doc.ClassID == "")
            {
                message = "「專案結算」不可過帳";
            }
            else
            {
                if (docExt.UsrApprovalStatus != "AP") //AP:已核可
                {
                    message = "簽核狀態須為「已核可」";
                }
            }
            if (!String.IsNullOrEmpty(message))
                throw new PXException(message);
            baseMethod(doc);

            //更新原廠Data，補齊缺少欄位資訊
            updatePMChangeOrderCostBudget();
            //更新工料描述
            updatePMChangeOrderCostBudgetUsrInventoryDesc();
            updatePOLineTranDescByPMChangeOrderLine();
        }
        #endregion

        #region Event Handlers

        #region PMChangeOrder
        protected virtual void PMChangeOrder_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            PMChangeOrder master = (PMChangeOrder)e.Row;

            if (holdCheck == true)
            {
                master.Hold = false;
                master.Status = beforeStatus;
            }
            SetEnable();
            //20201225 add by Alton - 11853: PM308000 訂單變更新增檢核及連動
            SetRequired();
            if (message != null)
            {
                cache.RaiseExceptionHandling<PMChangeOrder.hold>(master, master.Hold, new PXSetPropertyException(message, PXErrorLevel.Error));
            }

            addCurCostBudget.SetEnabled(Base.addPOLines.GetEnabled());

            // 已核可的訂單變更，不能自行把擱置勾起來修改
            PXUIFieldAttribute.SetEnabled<PMChangeOrder.hold>(cache, master, master != null & master.GetExtension<PMChangeOrderExt>().UsrApprovalStatus != ApprovalStatusAttribute.Approved);
        }

        Boolean? holdCheck = null;
        String beforeStatus = null;
        protected virtual void PMChangeOrder_Hold_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
        {
            if (e.Row == null) { return; }

            PMChangeOrder master = (PMChangeOrder)e.Row;
            PMChangeOrderExt pmChangeOrderExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(master);

            if ((bool?)e.NewValue == true && "PA".Equals(pmChangeOrderExt.UsrApprovalStatus))
            {
                cache.RaiseExceptionHandling<PMChangeOrder.hold>(master, master.Hold,
                                          new PXSetPropertyException("此表單送審中, 不可異動狀態", PXErrorLevel.Error));
                e.Cancel = true;
                holdCheck = true;
                beforeStatus = master.Status;
            }
        }

        //add by alton 20191209 hold = true時 將agentFlow資訊清除
        protected virtual void PMChangeOrder_Hold_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e, PXFieldUpdated baseHandler)
        { 
            PMChangeOrder row = (PMChangeOrder)e.Row;

            if ((bool?)e.OldValue == true && row.Hold == false)
            {
                CheckRemainAvailBudget<PMChangeOrderCostBudget, PMChangeOrderCostBudget.qty, PMChangeOrderCostBudget.amount>(Base.CostBudget.Cache);
                CheckRemainAvailCommitment(Base.Details.Cache);
            }

            if (Base.CostBudget.Cache.ForceExceptionHandling == true || Base.Details.Cache.ForceExceptionHandling == true)
            {
                ///<remarks> Since Hold changes and controls don't work as we expected, there is no choice but to reset the value before the raise header exception handling. </remarks>
                row.Hold = (bool?)e.OldValue;
                cache.Update(row);

                SetError<PMChangeOrder.hold>(cache, row, !row.Hold, BudgetExceeded, PXErrorLevel.Error);
            }

            baseHandler?.Invoke(cache, e);

            if (holdCheck == true) { return; }

            PMChangeOrderExt rowExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(row);

            bool isHold = row.Hold == true;

            //add by alton 20200327 如果usrApprovestatus的狀態為已核可(AP), 請保留UsrKGFlowUID, UsrApprovalStatus, UsrReturnUrl
            bool isAP = rowExt.UsrApprovalStatus == "AP";
            if (isHold && !isAP)
            {
                rowExt.UsrReturnUrl = null;
                rowExt.UsrKGFlowUID = null;
                rowExt.UsrApprovalStatus = null;
            }

            //訂單變更擱置移除，自動執行送簽
            if (isHold == false)
            {
                webServIntegration.Press();
            }
        }

        protected virtual void PMChangeOrder_ProjectID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            PMChangeOrder row = (PMChangeOrder)e.Row;
            int? oldProjectID = (int?)e.OldValue;
            if (row == null) return;
            if (row.ProjectID != oldProjectID)
            {
                //寫入變更次別PMChangeOrder
                PMChangeOrderExt masterExt = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(row);
                masterExt.UsrOrderChangePeriod = getChangeOrderPeriodByProject(row.ProjectID);

                //當ProjectID被異動時，清空KGNewChangeOrder & KGNewChangeOrderLine
                foreach (KGNewChangeOrder header in KGNewChangeOrders.Select())
                {
                    foreach (KGNewChangeOrderLine line in getKGNewChangeOrderLine(header.NewChangeOrderID))
                    {
                        KGNewChangeOrderLines.Delete(line);
                    }
                    KGNewChangeOrders.Delete(header);
                }
            }
        }
        #endregion

        #region PMChangeOrderLine
        protected void _(Events.RowSelected<PMChangeOrderLine> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            e.Cache.AllowInsert = false;

            if (e.Row == null) { return; }

            PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.pOLineNbr>(e.Cache, e.Row, false);

            if (e.Row.LineType == ChangeOrderLineType.NewLine)
            {
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.taskID>(e.Cache, e.Row, false);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.costCodeID>(e.Cache, e.Row, false);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.inventoryID>(e.Cache, e.Row, false);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.uOM>(e.Cache, e.Row, false);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.accountID>(e.Cache, e.Row, false);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.vendorID>(e.Cache, e.Row, false);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.curyID>(e.Cache, e.Row, false);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLineExt.usrRemark>(e.Cache, e.Row, false);
            }

            RelationshipTableControl<PMChangeOrderLine>(Base.Details.Cache, e.Row, e.Row.UOM != UOM_式);
        }

        protected virtual void _(Events.RowPersisting<PMChangeOrderLine> e)
        {
            PMChangeOrderLine row = e.Row;
            if (row == null) return;
            if (row.POOrderNbr == null)
            {
                SetError<PMChangeOrderLine.pOOrderNbr>(e.Cache, row, row.POOrderNbr, "已承諾契約編號(Commitment Nbr.)不得為空的", PXErrorLevel.RowError);
            }
        }

        protected void _(Events.FieldUpdated<PMChangeOrderLine.accountID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            ///<remarks>
            ///已承諾原訂單項目為一式項，變更時數量固定為0
            ///已承諾新增訂單項目，自動帶出工料的單位，並將描述帶到變更的行描述
            ///</remarks>
            var row = e.Row as PMChangeOrderLine;

            if (row.InventoryID != null && e.NewValue != null)
            {
                InventoryItem item = InventoryItem.PK.Find(Base, row.InventoryID);

                Base.Details.Cache.SetValue<PMChangeOrderLine.uOM>(row, item.PurchaseUnit);
                //Base.Details.Cache.SetValue<PMChangeOrderLineExt.usrInventoryDesc>(row, item.Descr);
                Base.Details.Cache.SetDefaultExt<PMChangeOrderLineExt.usrInventoryDesc>(row);

                // Commented out as it will give an unknown error refer to mantis [0012313] - (0025845) #3
                // mantis [0012313] - (0025850) #3
                if (row.UOM == UOM_式)
                {
                    e.Cache.SetValue<PMChangeOrderLine.qty>(row, (row.LineType == ChangeOrderLineType.NewLine || string.IsNullOrEmpty(row.POOrderNbr)) ?
                                                                 1m :
                                                                 IsCostBudgetHasOrigBudgetQty(row.ProjectID,
                                                                                              row.TaskID,
                                                                                              Account.PK.Find(Base, (int)e.NewValue)?.AccountGroupID,
                                                                                              row.CostCodeID,
                                                                                              row.InventoryID) ? 0m : 1m);
                }
            }
        }

        protected void _(Events.FieldVerifying<PMChangeOrderLine, PMChangeOrderLine.qty> e, PXFieldVerifying baseHandler)
        {
            // Since the cache is not updated with the latest value before validation, the value has to be specified.
            e.Row.Qty = (decimal?)e.NewValue;
            CheckRemainAvailCommitment(e.Cache);

            if (e.Cache.ForceExceptionHandling != true)
            {
                baseHandler?.Invoke(e.Cache, e.Args);
            }

            var row = (PMChangeOrderLine)e.Row;

            //20200618 louis 檢核變更合約明細一式項變更後數量必須為1
            checkOrderQtyForOneUom(e.Cache, row);
        }

        protected virtual void _(Events.FieldVerifying<PMChangeOrderLine, PMChangeOrderLine.amount> e)
        {
            var row = e.Row as PMChangeOrderLine;

            if (row != null && e.NewValue != null && row.UOM != UOM_式)
            {
                CheckOneNthDifference<PMChangeOrderLine.amount>(e.Cache, row.UnitCost.Value, (decimal)e.NewValue);
            }

            // Since the cache is not updated with the latest value before validation, the value has to be specified.
            e.Row.Amount = (decimal?)e.NewValue;
            CheckRemainAvailCommitment(e.Cache);
        }

        public virtual void _(Events.FieldDefaulting<PMChangeOrderLine, PMChangeOrderLineExt.usrInventoryDesc> e)
        {
            if (e.Row == null) { return; }

            // According to Louis's final testing request.
            var costBudget = GetPMBudget(e.Row.ProjectID, e.Row.TaskID, GetPMAccountGrpID(e.Cache.Graph, e.Row.AccountID), e.Row.CostCodeID, e.Row.InventoryID);

            e.NewValue = costBudget?.GetExtension<PMBudgetExt>()?.UsrInventoryDesc ?? InventoryItem.PK.Find(Base, e.Row.InventoryID)?.Descr;
        }
        #endregion

        #region PMChangeOrderCostBudget
        protected void _(Events.RowSelected<PMChangeOrderCostBudget> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            PMChangeOrderCostBudget line = (PMChangeOrderCostBudget)e.Row;
            if (line == null) return;
            PMChangeOrderBudgetExt lineExt = PXCache<PMChangeOrderBudget>.GetExtension<PMChangeOrderBudgetExt>(line);
            if (line.ProjectTaskID != null && line.AccountGroupID != null && line.InventoryID != null && line.CostCodeID != null
                && (lineExt.UsrInventoryDesc == null || lineExt.UsrInventoryDesc == ""))
            {
                PMCostBudget costBudgetet = getPMCostBudget(line.ProjectTaskID, line.AccountGroupID, line.InventoryID, line.CostCodeID);
                if (costBudgetet != null)
                {
                    PMBudgetExt budgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(costBudgetet);
                    lineExt.UsrInventoryDesc = budgetExt.UsrInventoryDesc;
                }
            }

            RelationshipTableControl<PMChangeOrderCostBudget>(e.Cache, line, line.UOM != UOM_式);
        }

        protected void _(Events.RowPersisting<PMChangeOrderCostBudget> e)
        {
            PMChangeOrderCostBudget row = e.Row;

            if (row == null) return;

            if (row.InventoryID == null || row.InventoryID == PMInventorySelectorAttribute.EmptyInventoryID)
            {
                SetError<PMChangeOrderCostBudget.inventoryID>(e.Cache, row, row.InventoryID, "成本預算工料編號不得為空或是N/A", PXErrorLevel.RowError);
            } 
        }

        protected void _(Events.FieldUpdated<PMChangeOrderCostBudget.inventoryID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            PMChangeOrderCostBudget row = (PMChangeOrderCostBudget)e.Row;
            if (row == null) return;

            InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<PMChangeOrderCostBudget.inventoryID>(Base.CostBudget.Cache, row);

            if (item != null && row.AccountGroupID == null)
            {
                e.Cache.SetValueExt<PMChangeOrderCostBudget.accountGroupID>(row, GetAccount(item.COGSAcctID)?.AccountGroupID);
            }
            e.Cache.SetDefaultExt<PMChangeOrderCostBudgetExt.usrCostCodeDesc>(row);

            if (e.NewValue != null)
            {
                ///<remarks>成本預算新增預算項目時，自動帶出工料的單位，並將描述帶到變更的工料描述</remarks>
                Base.CostBudget.Cache.SetValue<PMChangeOrderBudget.uOM>(row, item.PurchaseUnit);

                e.Cache.SetDefaultExt<PMChangeOrderBudgetExt.usrInventoryDesc>(row);

                if (row.UOM == UOM_式)
                {
                    Base.CostBudget.Cache.SetValue<PMChangeOrderCostBudget.qty>(row, row.GetExtension<PMChangeOrderCostBudgetExt>()?.UsrLineType == ChangeOrderLineType.NewDocument ?
                                                                                     1m :
                                                                                     IsCostBudgetHasOrigBudgetQty(row.ProjectID, row.ProjectTaskID,
                                                                                                                  row.AccountGroupID, row.CostCodeID,
                                                                                                                  row.InventoryID) ? 0m : 1m);
                }
            }
        }

        protected virtual void _(Events.FieldVerifying<PMChangeOrderCostBudget, PMChangeOrderCostBudget.qty> e)
        { 
            //20200619 louis 檢核變更成本預算一式項變更後數量必須為1
            checkCostBudgeQtyForOneUom(e.Cache, (PMChangeOrderCostBudget)e.Row);

            // Since the cache is not updated with the latest value before validation, the value has to be specified.
            e.Row.Qty = (decimal?)e.NewValue;
            CheckRemainAvailBudget<PMChangeOrderCostBudget, PMChangeOrderCostBudget.qty, PMChangeOrderCostBudget.amount>(e.Cache);
        }

        public virtual void _(Events.FieldUpdated<PMChangeOrderCostBudget, PMChangeOrderCostBudget.projectID> e)
        {
            PMChangeOrderCostBudget row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<PMChangeOrderCostBudgetExt.usrCostCodeDesc>(row);
            e.Cache.SetDefaultExt<PMChangeOrderBudgetExt.usrInventoryDesc>(row);
        }

        public void _(Events.FieldUpdated<PMChangeOrderCostBudget, PMChangeOrderCostBudget.projectTaskID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            PMChangeOrderCostBudget row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<PMChangeOrderCostBudgetExt.usrCostCodeDesc>(row);
            e.Cache.SetDefaultExt<PMChangeOrderBudgetExt.usrInventoryDesc>(row);
        }

        public void _(Events.FieldUpdated<PMChangeOrderCostBudget, PMChangeOrderCostBudget.accountGroupID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            PMChangeOrderCostBudget row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<PMChangeOrderCostBudgetExt.usrCostCodeDesc>(row);
            e.Cache.SetDefaultExt<PMChangeOrderBudgetExt.usrInventoryDesc>(row);
        }

        public void _(Events.FieldUpdated<PMChangeOrderCostBudget, PMChangeOrderCostBudget.costCodeID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Row == null) { return; }

            e.Cache.SetDefaultExt<PMChangeOrderBudgetExt.usrInventoryDesc>(e.Row);
        }

        public virtual void _(Events.FieldDefaulting<PMChangeOrderCostBudget, PMChangeOrderCostBudgetExt.usrCostCodeDesc> e)
        {
            PMChangeOrderCostBudget row = e.Row;
            if (row == null) return;
            string desc = "";
            if (row.ProjectID != null && row.ProjectTaskID != null && row.AccountGroupID != null && row.CostCodeID != null && row.InventoryID != null)
            {
                PMBudget budget = GetPMBudget(row.ProjectID, row.ProjectTaskID, row.AccountGroupID, row.CostCodeID, row.InventoryID);
                desc = budget?.Description;
            }
            if (String.IsNullOrEmpty(desc) && row.CostCodeID != null)
            {
                PMCostCode costCode = GetPMCostCode(row.CostCodeID);
                desc = costCode?.Description;
            }
            e.NewValue = desc;
        }

        public virtual void _(Events.FieldDefaulting<PMChangeOrderCostBudget, PMChangeOrderBudgetExt.usrInventoryDesc> e)
        {
            PMChangeOrderBudget row = e.Row;

            if (row == null) { return; }

            // Modified the source based on mantis [0012313] - (0025828) #1
            // Make up for missing condition per mantis [0012313] - (0025840) #5
            var costBudget = GetPMBudget(row.ProjectID, row.ProjectTaskID, row.AccountGroupID, row.CostCodeID, row.InventoryID);

            e.NewValue = costBudget?.GetExtension<PMBudgetExt>()?.UsrInventoryDesc ?? InventoryItem.PK.Find(Base, row.InventoryID)?.Descr;
        }

        protected virtual void _(Events.FieldVerifying<PMChangeOrderCostBudget, PMChangeOrderCostBudget.amount> e)
        {
            var row = e.Row as PMChangeOrderCostBudget;

            if (row != null && e.NewValue != null && row.UOM != UOM_式)
            {
                ///<remarks>成本預算非一式項變更預算輸入複價，要檢核變更後的複價不能超過變更數量*單價的萬分之一</remarks>
                CheckOneNthDifference<PMChangeOrderCostBudget.amount>(e.Cache, row.Rate.Value, (decimal)e.NewValue);
            }

            // Since the cache is not updated with the latest value before validation, the value has to be specified.
            e.Row.Amount = (decimal?)e.NewValue;
            CheckRemainAvailBudget<PMChangeOrderCostBudget, PMChangeOrderCostBudget.qty, PMChangeOrderCostBudget.amount>(e.Cache);
        }
        #endregion

        #region KGNewChangeOrderLine
        protected virtual void KGNewChangeOrder_VendorID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGNewChangeOrder row = (KGNewChangeOrder)e.Row;
            if (row == null) return;
            if (row.VendorID == null)
            {
                row.TaxRegistrationID = null;
            }
            else
            {
                Location location = PXSelect<Location, Where<Location.isActive, Equal<True>, And<Location.bAccountID, Equal<Required<Location.bAccountID>>>>>
                    .Select(Base, row.VendorID);
                if (location != null)
                {
                    row.TaxRegistrationID = location.TaxRegistrationID;
                }
            }
        }

        protected virtual void KGNewChangeOrderLine_Qty_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGNewChangeOrderLine row = (KGNewChangeOrderLine)e.Row;
            if (row == null) return;
            row.Amount = row.Qty * row.UnitCost;
        }

        protected virtual void _(Events.FieldUpdated<KGNewChangeOrderLine.unitCost> e)
        {
            KGNewChangeOrderLine row = (KGNewChangeOrderLine)e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<KGNewChangeOrderLine.amount>(row, (row.Qty ?? 0) * (row.UnitCost ?? 0));
        }
        #endregion

        #endregion

        //Add By Althea
        #region Methods
        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg, PXErrorLevel errorLevel) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue, new PXSetPropertyException(errorMsg, errorLevel));
            return false;
        }

        public void SetUIRequired<T>(PXCache cache) where T : PX.Data.IBqlField
        {
            PXUIFieldAttribute.SetRequired<T>(cache, true);
            PXDefaultAttribute.SetPersistingCheck<T>(cache, null, PXPersistingCheck.NullOrBlank);
        }

        public void SetRequired()
        {
            //收入預算
            SetUIRequired<PMChangeOrderRevenueBudget.costCodeID>(Base.RevenueBudget.Cache);
            SetUIRequired<PMChangeOrderRevenueBudget.uOM>(Base.RevenueBudget.Cache);
            SetUIRequired<PMChangeOrderRevenueBudget.rate>(Base.RevenueBudget.Cache);
            //成本預算
            SetUIRequired<PMChangeOrderCostBudget.costCodeID>(Base.CostBudget.Cache);
            SetUIRequired<PMChangeOrderCostBudget.uOM>(Base.CostBudget.Cache);
            SetUIRequired<PMChangeOrderCostBudget.rate>(Base.CostBudget.Cache);
            //已承諾
            SetUIRequired<PMChangeOrderLine.pOOrderNbr>(Base.Details.Cache);
        }

        public void SetEnable()
        {
            //表頭 PMChangeOrder 
            PMChangeOrder changeOrder = Base.Document.Current;

            bool editable = changeOrder.Status == "H";
            bool isOpen = changeOrder.Status == "O";

            PXUIFieldAttribute.SetEnabled<PMChangeOrderExt.usrChangeReason>(Base.Document.Cache, changeOrder, editable);

            PMChangeOrderBudget budget = Budget2.Current;
            if (budget != null)
            {
                //收入預算Tab
                PMChangeOrderCostBudgetExt costBudgetExt = budget.GetExtension<PMChangeOrderCostBudgetExt>();

                PXUIFieldAttribute.SetEnabled<PMChangeOrderBudgetExt.usrIsApprovedByCustomer>(Base.Budget.Cache, budget, editable);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderBudgetExt.usrRemark>(Base.Budget.Cache, budget, editable);

                //成本預算Tab
                PXUIFieldAttribute.SetEnabled<PMChangeOrderBudgetExt.usrChangeType>(Base.Budget.Cache, budget, editable);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderBudgetExt.usrChangeReason>(Base.Budget.Cache, budget, editable);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderBudgetExt.usrRemark>(Base.Budget.Cache, budget, editable);
            }

            //已承諾Tab
            PMChangeOrderLine changeOrderLine = Base.Details.Current;
            if (changeOrderLine != null)
            {
                PMChangeOrderLineExt changeOrderLineExt = PXCache<PMChangeOrderLine>.GetExtension<PMChangeOrderLineExt>(changeOrderLine);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLineExt.usrBelongKind>(Base.Details.Cache, changeOrderLine, editable);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLineExt.usrChangeReason>(Base.Details.Cache, changeOrderLine, editable);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLineExt.usrChangeSource>(Base.Details.Cache, changeOrderLine, editable);
                PXUIFieldAttribute.SetEnabled<PMChangeOrderLineExt.usrRemark>(Base.Details.Cache, changeOrderLine, editable);
            }

            KGNewChangeOrders.AllowInsert = editable;
            showNewChangeOrderDialog.SetEnabled(editable);

            //送審：status = open & usrApprovalStatus is null
            webServIntegration.SetEnabled(isOpen && changeOrder.GetExtension<PMChangeOrderExt>().UsrApprovalStatus == null);

            PXUIFieldAttribute.SetEnabled(KGNewChangeOrders.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled(KGNewChangeOrderLines.Cache, null, editable);
        }

        //20200618 louis 檢核變更合約明細一式項變更後數量必須為1
        public void checkOrderQtyForOneUom(PXCache sender, PMChangeOrderLine changeOrderLine)
        {
            // mantis [0012313] - (0025840) #6,7
            decimal changeQty = changeOrderLine.Qty.GetValueOrDefault();
            decimal zero = 0;
            decimal minusOne = -1;
            decimal one = 1;
            String lineType = changeOrderLine.LineType, uom = changeOrderLine.UOM;

            if (UOM_式.Equals(uom))
            {
                if (ChangeOrderLineType.Update.Equals(lineType))
                {
                    if (minusOne.CompareTo(changeQty) != 0 && zero.CompareTo(changeQty) != 0)
                    {
                        sender.RaiseExceptionHandling<PMChangeOrderLine.qty>(changeOrderLine, changeOrderLine.Qty,
                          new PXSetPropertyException("變更一式項數量必須為0", PXErrorLevel.Error));
                    }
                }
                else if (ChangeOrderLineType.NewLine.Equals(lineType) ||
                         ChangeOrderLineType.NewDocument.Equals(lineType))
                {
                    if (one.CompareTo(changeQty) != 0)
                    {
                        sender.RaiseExceptionHandling<PMChangeOrderLine.qty>(changeOrderLine, changeOrderLine.Qty,
                         new PXSetPropertyException("新增一式項數量必須為1", PXErrorLevel.Error));
                    }
                }
            }
        }

        //20200619 louis 檢核變更成本預算一式項變更後數量必須為1
        public bool checkCostBudgeQtyForOneUom(PXCache sender, PMChangeOrderCostBudget pmChangeOrderCostBudget)
        {
            //modify by louis for upgrade to 20.114.0020
            //PMBudget budget = this.IsValidKey(pmChangeOrderCostBudget) ? Base.GetOriginalCostBudget(pmChangeOrderCostBudget.GetBudgetKey()) : null;
            PMBudget budget = IsValidKey(pmChangeOrderCostBudget) ? Base.GetOriginalCostBudget(BudgetKeyTuple.Create(pmChangeOrderCostBudget)) : null;
            
            decimal qty = (decimal)(pmChangeOrderCostBudget.Qty ?? 0m);
            string uom = pmChangeOrderCostBudget.UOM;
            if (budget != null)
            {
                decimal reviseQty = (decimal)budget.RevisedQty;

                if (UOM_式.Equals(uom))
                {
                    if ((reviseQty + qty) != 1)
                    {
                        sender.RaiseExceptionHandling<PMChangeOrderCostBudget.qty>(pmChangeOrderCostBudget,
                                          pmChangeOrderCostBudget.Qty,
                              new PXSetPropertyException("一式項預算變更後的數量必須為 1", PXErrorLevel.Error));
                        return false;
                    }
                }
            }
            else
            {
                if (UOM_式.Equals(uom))
                {
                    if (qty != 1)
                    {
                        sender.RaiseExceptionHandling<PMChangeOrderCostBudget.qty>(pmChangeOrderCostBudget,
                                          pmChangeOrderCostBudget.Qty,
                              new PXSetPropertyException("一式項預算變更後的數量必須為 1", PXErrorLevel.Error));
                        return false;
                    }
                }
            }
            return true;
        }

        public bool beforeSaveCheck()
        {
            PMChangeOrder master = Base.Document.Current;
            //寫入變更次別PMChangeOrderLine
            foreach (PMChangeOrderLine line in Base.Details.Select())
            {
                //20200618 louis 檢核變更合約明細一式項變更後數量必須為1
                checkOrderQtyForOneUom(Base.Details.Cache, line);

                PXEntryStatus lineDmlStatus = Base.Details.Cache.GetStatus(line);
                if (lineDmlStatus == PXEntryStatus.Inserted)
                {
                    PMChangeOrderLineExt lineExt = PXCache<PMChangeOrderLine>.GetExtension<PMChangeOrderLineExt>(line);
                    lineExt.UsrChangePeriod = getCommitmentChangePeriodByPONbr(master.RefNbr, line.POOrderNbr);
                }
            }
            bool flag = true;
            //20200619 louis 檢核變更成本預算一式項變更後數量必須為1
            foreach (PMChangeOrderCostBudget costBudge in Base.CostBudget.Select())
            {
                //2021/01/20 Althea Add Mantis:0011853
                PMChangeOrderBudgetExt changeOrderBudgetExt = PXCache<PMChangeOrderBudget>.GetExtension<PMChangeOrderBudgetExt>(costBudge);
                if (changeOrderBudgetExt.UsrIsFreeItem != true)
                {
                    //20201225 Alton 單位費率不得為0
                    if (costBudge.Rate == 0m)
                    {
                        Base.CostBudget.Cache.RaiseExceptionHandling<PMChangeOrderCostBudget.rate>(costBudge,
                                              costBudge.Rate, new PXSetPropertyException("單位費率不得為零", PXErrorLevel.Error));
                        flag = false;
                    }
                }
            }

            //20201225 Alton 單位費率需大於0
            foreach (PMChangeOrderRevenueBudget revenueBudget in Base.RevenueBudget.Select())
            {
                if (revenueBudget.Rate == 0m)
                {
                    Base.RevenueBudget.Cache.RaiseExceptionHandling<PMChangeOrderRevenueBudget.rate>(revenueBudget,
                                          revenueBudget.Rate, new PXSetPropertyException("單位費率不得為零", PXErrorLevel.Error));
                    flag = false;
                }
            }

            //重新滾算流水號 & 重新計算 total Amount
            foreach (KGNewChangeOrder header in KGNewChangeOrders.Select())
            {
                decimal? total = 0;
                int lineNbr = 1;
                foreach (KGNewChangeOrderLine item in getKGNewChangeOrderLine(header.NewChangeOrderID))
                {
                    item.LineNbr = lineNbr++;
                    total += item.Amount;
                    KGNewChangeOrderLines.Update(item);
                }
                header.Amount = total;
                KGNewChangeOrders.Update(header);
            }

            CheckRemainAvailBudget<PMChangeOrderCostBudget, PMChangeOrderCostBudget.qty, PMChangeOrderCostBudget.amount>(Base.CostBudget.Cache, true);
            CheckRemainAvailCommitment(Base.Details.Cache, true);

            return true;
        }

        public virtual void RelationshipTableControl<T>(PXCache cache, T data, bool isSpecficUOM) where T : PX.Data.IBqlTable, new()
        {
            ///<remarks>
            ///成本預算的原預算項目變更時，單價費率/工料描述/計價單元描述不得異動
            ///成本預算原預算項目/新增預算項目為一式項，不能修改
            ///已承諾原訂單項目變更，單價及行描述不得異動。
            ///已承諾原訂單項目為一式項，不能修改。
            /// </remarks>
            bool isOnHold  = Base.Document.Current?.Status == ChangeOrderStatus.OnHold;
            bool isNewType = true;

            switch (typeof(T).Name)
            {
                case nameof(PMChangeOrderCostBudget):
                    isNewType = cache.GetExtension<PMChangeOrderCostBudgetExt>(data).UsrLineType == ChangeOrderLineType.NewDocument;

                    PXUIFieldAttribute.SetEnabled<PMChangeOrderBudget.rate>(cache, data, isOnHold && isNewType);
                    PXUIFieldAttribute.SetEnabled<PMChangeOrderBudget.qty>(cache, data, isOnHold && isSpecficUOM);
                    PXUIFieldAttribute.SetEnabled<PMChangeOrderBudgetExt.usrInventoryDesc>(cache, data, isOnHold && isNewType);
                    PXUIFieldAttribute.SetEnabled<PMChangeOrderCostBudgetExt.usrCostBudgetType>(cache, data, isOnHold && isNewType);
                    break;
                case nameof(PMChangeOrderLine):
                    isNewType = (data as PMChangeOrderLine).LineType == ChangeOrderLineType.NewLine;

                    PXUIFieldAttribute.SetEnabled<PMChangeOrderLineExt.usrInventoryDesc>(cache, data, isOnHold && isNewType);
                    PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.unitCost>(cache, data, isOnHold && isNewType);
                    PXUIFieldAttribute.SetEnabled<PMChangeOrderLine.qty>(cache, data, isOnHold && isSpecficUOM);
                    //PXUIFieldAttribute.SetEnabled<CN.Subcontracts.PM.CacheExtensions.PMChangeOrderLineCLExt.commitmentType>(cache, data, isOnHold && isNewType);
                    break;
            }
        }

        /// <summary>
        /// 剩餘可發包預算＝
        /// 成本預算變更後的複價/數量 (Change order from cost budget)
        /// – 已建單之分包規劃複價/數量（分包）或已建單之零星合約的複價/數量（零星） (Request)
        /// – 已核可之已承諾訂單變更（包含自己）追加減複價/數量 (PO commitments in change order)
        /// </summary>
        // Created a common method for different graphs to use based on spec [Kedge_SASD_請採購系統改善需求規格].
        public void CheckRemainAvailBudget<T, T1, T2>(PXCache cache, bool whenSaving = false) where T : IBqlTable where T1 : IBqlField where T2 : IBqlField
        {
            ValueTuple<int?, int?, int?, int?, int?> values = new ValueTuple<int?, int?, int?, int?, int?>();

            string refNbr = string.Empty, orderNbr = string.Empty;
            foreach (T row in cache.Cached)
            {
                switch (cache.BqlTable.Name)
                {
                    case nameof(PMChangeOrderBudget):
                        var budget   = row as PMChangeOrderCostBudget;
                        values       = ValueTuple.Create(budget.ProjectID, budget.ProjectTaskID, budget.CostCodeID, budget.AccountGroupID, budget.InventoryID);
                        isSpecialUOM = budget.UOM == UOM_式;
                        refNbr       = budget.RefNbr;
                        break;
                    case nameof(RQRequestLine):
                        var line     = row as RQRequestLine;
                        var lineExt  = line.GetExtension<RQRequestLineExt>();
                        values       = ValueTuple.Create(lineExt.UsrContractID, lineExt.UsrProjectTaskID, lineExt.UsrCostCodeID, lineExt.UsrAccountGroupID, line.InventoryID);
                        isSpecialUOM = line.UOM == UOM_式;
                        orderNbr     = line.OrderNbr;
                        break;
                }
                bool fromChangeOrder = string.IsNullOrEmpty(orderNbr);

                decimal? sourceQty = (decimal?)cache.GetValue(row, cache.GetField(typeof(T1)));
                decimal? sourceAmt = (decimal?)cache.GetValue(row, cache.GetField(typeof(T2)));

                (decimal qty, decimal amount) = CalcRemainingAvailValue(cache.Graph, values, refNbr, string.Empty, orderNbr);

                if (Math.Abs(sourceQty.Value) > Math.Abs(qty) && !isSpecialUOM && ((fromChangeOrder == true & sourceQty < 0m) || (fromChangeOrder == false & sourceQty > 0m)))
                {
                    cache.RaiseExceptionHandling<T1>(row, sourceQty, new PXSetPropertyException($"{CannotGreaterThanRemain_Budget}數量 [{qty}]。"));
                    cache.ForceExceptionHandling = true;
                }
                if (Math.Abs(sourceAmt.Value) > Math.Abs(amount) && isSpecialUOM && ((fromChangeOrder == true & sourceAmt < 0m) || (fromChangeOrder == false & sourceAmt > 0m)))
                {
                    cache.RaiseExceptionHandling<T2>(row, sourceAmt, new PXSetPropertyException($"{CannotGreaterThanRemain_Budget}複價 [{amount}]。")); 
                    cache.ForceExceptionHandling = true;
                }
                // Due to Acumatica standard exceptions don't work properly as we expect and it can't raise line's exception handling neither.
                if (whenSaving == true && cache.ForceExceptionHandling == true)
                {
                    throw new Exception(BudgetExceeded);
                }
            }
        }

        ///<summary>
        ///送審時檢核已承諾減少後的數量不可小於剩餘可計價複價/數量
        /// </summary>
        private void CheckRemainAvailCommitment(PXCache cache, bool whenSaving = false)
        {
            string errorMsg = null;
            foreach (PMChangeOrderLine row in cache.Cached)
            {
                isSpecialUOM = row.UOM == UOM_式;

                (decimal qty, decimal amount) = (decimal.MaxValue, decimal.MaxValue);

                if ((!isSpecialUOM && row.Qty < 0) || (isSpecialUOM && row.Amount < 0) )
                {
                    (qty, amount) = CalcRemainingUnbilledValue(cache.Graph, ValueTuple.Create(row.ProjectID, row.TaskID, row.CostCodeID, row.AccountID, row.InventoryID), 
                                                               row.POOrderType, row.POOrderNbr, row.RefNbr);
                    errorMsg = CannotGreaterThanRemain_Commitment;
                }
                else if ((!isSpecialUOM && row.Qty > 0) || (isSpecialUOM && row.Amount > 0) )
                {
                    (qty, amount) = CalcRemainingAvailValue(cache.Graph, ValueTuple.Create(row.ProjectID, row.TaskID, row.CostCodeID, GetPMAccountGrpID(cache.Graph, row.AccountID), row.InventoryID), 
                                                            string.Empty, row.RefNbr, string.Empty);
                    errorMsg = CannotGreaterThanRemain_Budget;
                }

                if (Math.Abs(row.Qty.Value) > Math.Abs(qty) && !isSpecialUOM)
                {
                    cache.RaiseExceptionHandling<PMChangeOrderLine.qty>(row, row.Qty, new PXSetPropertyException($"{errorMsg}數量 [{qty}]。"));
                    cache.ForceExceptionHandling = true;
                }
                if (Math.Abs(row.Amount.Value) > Math.Abs(amount) && isSpecialUOM)
                {
                    cache.RaiseExceptionHandling<PMChangeOrderLine.amount>(row, row.Amount, new PXSetPropertyException($"{errorMsg}複價 [{amount}]。"));
                    cache.ForceExceptionHandling = true;
                }
                // Due to Acumatica standard exceptions don't work properly as we expect and it can't raise line's exception handling neither.
                if (whenSaving == true && cache.ForceExceptionHandling == true)
                {
                    throw new Exception(BudgetExceeded);
                }
            }
        }

        /// <summary>
        /// 成本預算非一式項變更預算輸入複價，要檢核變更後的複價不能超過變更數量*單價的萬分之一
        /// 已承諾非一式項變更訂單項目輸入複價，要檢核不能超過數量*單價的萬分之一
        /// </summary>
        public virtual void CheckOneNthDifference<T>(PXCache cache, decimal amount, decimal newValue) where T : PX.Data.IBqlField
        {
            decimal calcAmt = Math.Round(amount * OneNth, 0, MidpointRounding.AwayFromZero);
            decimal minAmt = amount - calcAmt;
            decimal maxAmt = amount + calcAmt;

            bool hasError = newValue > 0 ? newValue < minAmt || newValue > maxAmt : newValue > minAmt || newValue < maxAmt;

            if (hasError == true)
            {
                string errorMsg = string.Format("追加減金額必須介於{0}與{1}之間", Math.Round(minAmt, 2, MidpointRounding.AwayFromZero),
                                                                                Math.Round(maxAmt, 2, MidpointRounding.AwayFromZero));

                //throw new PXSetPropertyException<T>(errorMsg);
                cache.RaiseExceptionHandling<T>(cache.Current, newValue, new PXSetPropertyException(errorMsg));
            }
        }

        ///<summary>
        ///複寫原廠未填寫資料
        ///</summary>
        private void updatePMChangeOrderCostBudget()
        {
            //成本預算
            foreach (PMChangeOrderCostBudget costBudget in Base.CostBudget.Select())
            {
                PXUpdate<
                        Set<PMBudget.curyUnitRate, Required<PMBudget.curyUnitRate>,
                        Set<PMBudget.rate, Required<PMBudget.rate>,
                        Set<PMBudget.curyUnitPrice, Required<PMBudget.curyUnitPrice>,
                        Set<PMBudget.unitPrice, Required<PMBudget.unitPrice>,
                        Set<PMBudget.changeOrderAmount, Required<PMBudget.changeOrderAmount>,
                        Set<PMBudget.changeOrderQty, Required<PMBudget.changeOrderQty>,
                        Set<PMBudget.curyRevisedAmount, Required<PMBudget.curyRevisedAmount>,
                        Set<PMBudget.revisedAmount, Required<PMBudget.revisedAmount>,
                        Set<PMBudget.revisedQty, Required<PMBudget.revisedQty>,
                        Set<PMBudget.uOM, Required<PMBudget.uOM>
                        >>>>>>>>>>,
                     PMBudget,
                        Where<PMBudget.projectID, Equal<Required<PMBudget.projectID>>,
                            And<PMBudget.projectTaskID, Equal<Required<PMBudget.projectTaskID>>,
                            And<PMBudget.costCodeID, Equal<Required<PMBudget.costCodeID>>,
                            And<PMBudget.inventoryID, Equal<Required<PMBudget.inventoryID>>,
                            And<PMBudget.accountGroupID, Equal<Required<PMBudget.accountGroupID>>,
                            And<PMBudget.type, Equal<PX.Objects.GL.AccountType.expense>,
                            And<PMBudget.uOM, IsNull,
                            And<Where<PMBudget.curyUnitRate, IsNull, Or<PMBudget.curyUnitRate, Equal<Zero>>>>
                            >>>>>>
                    >>.Update(Base,
                        costBudget.Rate, costBudget.Rate, costBudget.Rate, costBudget.Rate,
                        costBudget.Amount, costBudget.Qty,
                        costBudget.Amount, costBudget.Amount, costBudget.Qty,
                        costBudget.UOM,
                        costBudget.ProjectID, costBudget.ProjectTaskID, costBudget.CostCodeID, costBudget.InventoryID, costBudget.AccountGroupID
                        );
            }
            Base.Save.Press();
        }

        private void updatePMChangeOrderCostBudgetUsrInventoryDesc()
        {
            foreach (PMChangeOrderCostBudget costBudget in Base.CostBudget.Select())
            {
                PMChangeOrderBudgetExt pmChangeOrderBudgetExt = PXCache<PMChangeOrderBudget>.GetExtension<PMChangeOrderBudgetExt>(costBudget);
                PXUpdate<
                        Set<PMBudgetExt.usrInventoryDesc, Required<PMBudgetExt.usrInventoryDesc>>,
                     PMBudget,
                        Where<PMBudget.projectID, Equal<Required<PMBudget.projectID>>,
                            And<PMBudget.projectTaskID, Equal<Required<PMBudget.projectTaskID>>,
                            And<PMBudget.costCodeID, Equal<Required<PMBudget.costCodeID>>,
                            And<PMBudget.inventoryID, Equal<Required<PMBudget.inventoryID>>,
                            And<PMBudget.accountGroupID, Equal<Required<PMBudget.accountGroupID>>,
                            And<PMBudget.type, Equal<PX.Objects.GL.AccountType.expense>
                            >>>>>
                    >>.Update(Base, pmChangeOrderBudgetExt.UsrInventoryDesc,
                        costBudget.ProjectID, costBudget.ProjectTaskID, costBudget.CostCodeID, costBudget.InventoryID, costBudget.AccountGroupID
                        );
            }
            Base.Save.Press();
        }

        private void updatePOLineTranDescByPMChangeOrderLine()
        {
            foreach (PMChangeOrderLine line in Base.Details.Select())
            {
                if (line.POOrderNbr != null && line.POOrderType != null && line.POLineNbr != null)
                {
                    PMChangeOrderLineExt lineExt = PXCache<PMChangeOrderLine>.GetExtension<PMChangeOrderLineExt>(line);
                    PXUpdate<
                            Set<POLine.tranDesc, Required<POLine.tranDesc>>,
                         POLine,
                            Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>,
                                And<POLine.orderType, Equal<Required<POLine.orderType>>
                                >>
                        >>.Update(Base, lineExt.UsrInventoryDesc,
                            line.POOrderNbr, line.POLineNbr, line.POOrderType
                            );
                }
            }
            Base.Save.Press();
        }

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

        ///<summary>
        ///刪除AGENTFLOW tempData
        ///</summary>
        private void deleteAgentFlowTempData(string refNbr)
        {
            foreach (KGFlowChangeManagement cm in getKGFlowChangeManagement(refNbr))
            {
                //delete KGFlowBudChgApplyDetail
                foreach (KGFlowBudChgApplyDetail item in getKGFlowBudChgApplyDetail(cm.BcaUID))
                {
                    KGFlowSubAccDetails.Delete(item);
                }
                //delete KGFlowPutRvn
                foreach (KGFlowPutRvn item in getKGFlowPutRvn(cm.BcaUID))
                {
                    //delete KGFlowPutChg
                    foreach (KGFlowPutChg detail in getKGFlowPutChg(item.RvnUID))
                    {
                        KGFlowPutChgs.Delete(detail);
                    }
                    KGFlowPutRvns.Delete(item);
                }

                //delete KGFlowBudChgApplyNew
                foreach (KGFlowBudChgApplyNew item in getKGFlowBudChgApplyNew(cm.BcaUID))
                {
                    //delete KGFlowBudChgApplyNewDetail
                    foreach (KGFlowBudChgApplyNewDetail detail in getKGFlowBudChgApplyNewDetail(item.Bcnuid))
                    {
                        KGFlowBudChgApplyNewDetails.Delete(detail);
                    }
                    KGFlowBudChgApplyNews.Delete(item);
                }
                KGFlowChangeManagements.Delete(cm);
            }
            Base.Actions.PressSave();
        }

        public virtual IEnumerable CommitmentBudget()
        {
            List<PXResult<PMChangeOrderLine, PMBudget>> result = new List<PXResult<PMChangeOrderLine, PMBudget>>();

            var select = new PXSelect<PMChangeOrderLine, Where<PMChangeOrderLine.refNbr, Equal<Current<PMChangeOrder.refNbr>>>>(Base);

            foreach (PMChangeOrderLine record in select.Select())
            {
                PMBudget budget = IsValidKey(record) ? Base.GetOriginalCostBudget(BudgetKeyTuple.Create(new PMChangeOrderBudget() { ProjectID = record.ProjectID, ProjectTaskID = record.TaskID, AccountGroupID = null, InventoryID = record.InventoryID, CostCodeID = record.CostCodeID })) : null;

                if (budget == null) budget = new PMBudget();

                result.Add(new PXResult<PMChangeOrderLine, PMBudget>(record, budget));
            }

            return result;
        }

        public virtual void AddSelectedCurCostBudget()
        {
            List<PMChangeOrderLine> lists = null;

            foreach (PMCostBudget budget in CurrentAvailCostBudget.Cache.Updated)
            {
                if (budget.Type != AccountType.Expense || budget.Selected != true) { continue; }

                PMChangeOrderLine line = new PMChangeOrderLine() 
                { 
                    ProjectID   = budget.ProjectID,
                    TaskID      = budget.ProjectTaskID, 
                    InventoryID = budget.InventoryID, 
                    CostCodeID  = budget.CostCodeID
                };

                line.LineType = ChangeOrderLineType.NewLine;
                line.UnitCost = budget.Rate;

                lists = Base.Details.Cache.Cached.RowCast<PMChangeOrderLine>().ToList();

                if (lists.Exists(e => e.ProjectID == line.ProjectID && e.TaskID == line.TaskID && e.InventoryID == line.InventoryID && e.CostCodeID == line.CostCodeID) == false)
                {
                    Base.Details.Cache.Insert(line);
                }
            }
        }

        /// <summary>
        /// [0] -> ProjectID, [1] -> ProjectTaskID, [2] -> AccountGroupID, [3] -> CostCodeID, [4] -> InventoryID
        /// </summary>
        private bool IsCostBudgetHasOrigBudgetQty(params int?[] keys)
        {
            return GetPMBudget(keys[0].Value, keys[1].Value, keys[2].Value, keys[3].Value, keys[4].Value)?.Qty > 0m;
        }
        #endregion

        #region BQLs
        public PMBudget GetPMBudget(int? projectID, int? taskID, int? groupID, int? costCodeID, int? inventoryID, PXGraph graph = null)
        {
            return PXSelect<PMBudget,
                    Where<PMBudget.projectID, Equal<Required<PMBudget.projectID>>,
                    And<PMBudget.projectTaskID, Equal<Required<PMBudget.projectTaskID>>,
                    And<PMBudget.accountGroupID, Equal<Required<PMBudget.accountGroupID>>,
                    And<PMBudget.costCodeID, Equal<Required<PMBudget.costCodeID>>,
                    And<PMBudget.inventoryID, Equal<Required<PMBudget.inventoryID>>,
                    And<PMBudget.type, Equal<Required<PMBudget.type>>
                >>>>>>>.Select(Base ?? graph, projectID, taskID, groupID, costCodeID, inventoryID, "E");
        }

        public PMCostCode GetPMCostCode(int? costCodeID)
        {
            return PXSelect<PMCostCode,
                Where<PMCostCode.costCodeID, Equal<Required<PMCostCode.costCodeID>>>>
                .Select(Base, costCodeID);
        }

        private Account GetAccount(int? accountID)
        {
            return PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>
                .Select(Base, accountID);
        }

        private POLine getPOLine(string orderNbr, string orderType, int? lineNbr)
        {
            return PXSelect<POLine,
                Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                And<POLine.orderType, Equal<Required<POLine.orderType>>,
                And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>
                .Select(Base, orderNbr, orderType, lineNbr);
        }

        private int getChangeOrderPeriodByProject(int? projectID)
        {
            PMChangeOrder order = PXSelectGroupBy<PMChangeOrder,
                        Where<PMChangeOrder.projectID, Equal<Required<PMChangeOrder.projectID>>,
                            And<PMChangeOrder.classID, NotEqual<Required<PMChangeOrder.classID>>>>,//clissID <> 專案結算
                        Aggregate<Max<PMChangeOrderExt.usrOrderChangePeriod>>>
                        .Select(Base, projectID, "專案結算");
            PMChangeOrderExt ext = PXCache<PMChangeOrder>.GetExtension<PMChangeOrderExt>(order);
            return (ext.UsrOrderChangePeriod ?? 0) + 1;
        }

        private int getCommitmentChangePeriodByPONbr(string refNbr, string poNbr)
        {
            PMChangeOrderLine item = PXSelectGroupBy<PMChangeOrderLine,
                Where<PMChangeOrderLine.refNbr, NotEqual<Required<PMChangeOrderLine.refNbr>>,//<>refNbr
                And<PMChangeOrderLine.pOOrderNbr, Equal<Required<PMChangeOrderLine.pOOrderNbr>>>>,//PoorderNbr
                Aggregate<Max<PMChangeOrderLineExt.usrChangePeriod>>
                >.Select(Base, refNbr, poNbr);

            if (item == null)
            {
                item = new PMChangeOrderLine();
            }
            PMChangeOrderLineExt ext = PXCache<PMChangeOrderLine>.GetExtension<PMChangeOrderLineExt>(item);
            return (ext.UsrChangePeriod ?? 0) + 1;
        }

        private int? GetPMAccountGrpID(PXGraph graph, int? accountID)
        {
            return SelectFrom<PMAccountGroup>.Where<PMAccountGroup.accountID.IsEqual<@P.AsInt>.And<PMAccountGroup.isExpense.IsEqual<True>>>
                                             .View.SelectSingleBound(graph, null, accountID).TopFirst?.GroupID;
        }

        private PXResultset<KGNewChangeOrderLine> getKGNewChangeOrderLine(int? newChangeOrderID)
        {
            return PXSelect<KGNewChangeOrderLine,
                        Where<KGNewChangeOrderLine.newChangeOrderID, Equal<Required<KGNewChangeOrderLine.newChangeOrderID>>>,
                        OrderBy<Asc<KGNewChangeOrderLine.createdDateTime>>>
                        .Select(Base, newChangeOrderID);
        }

        private PXResultset<KGFlowChangeManagement> getKGFlowChangeManagement(string refNbr)
        {
            return PXSelect<KGFlowChangeManagement,
                Where<KGFlowChangeManagement.orderChangeRefNbr, Equal<Required<KGFlowChangeManagement.orderChangeRefNbr>>>>
                        .Select(Base, refNbr);
        }

        private PXResultset<KGFlowBudChgApplyDetail> getKGFlowBudChgApplyDetail(string bcaUID)
        {
            return PXSelect<KGFlowBudChgApplyDetail,
                Where<KGFlowBudChgApplyDetail.bcaUID, Equal<Required<KGFlowBudChgApplyDetail.bcaUID>>>>
                        .Select(Base, bcaUID);
        }

        private PXResultset<KGFlowPutRvn> getKGFlowPutRvn(string bcaUID)
        {
            return PXSelect<KGFlowPutRvn,
                Where<KGFlowPutRvn.bcaUID, Equal<Required<KGFlowPutRvn.bcaUID>>>>
                        .Select(Base, bcaUID);
        }

        private PXResultset<KGFlowPutChg> getKGFlowPutChg(string rvnUID)
        {
            return PXSelect<KGFlowPutChg,
                Where<KGFlowPutChg.rvnUID, Equal<Required<KGFlowPutChg.rvnUID>>>>
                        .Select(Base, rvnUID);
        }

        private PXResultset<KGFlowBudChgApplyNew> getKGFlowBudChgApplyNew(string bcaUID)
        {
            return PXSelect<KGFlowBudChgApplyNew,
                Where<KGFlowBudChgApplyNew.bcauid, Equal<Required<KGFlowBudChgApplyNew.bcauid>>>>
                        .Select(Base, bcaUID);
        }

        private PXResultset<KGFlowBudChgApplyNewDetail> getKGFlowBudChgApplyNewDetail(string bcnuid)
        {
            return PXSelect<KGFlowBudChgApplyNewDetail,
                Where<KGFlowBudChgApplyNewDetail.bcnuid, Equal<Required<KGFlowBudChgApplyNewDetail.bcnuid>>>>
                        .Select(Base, bcnuid);
        }

        private PXResultset<PMCostBudget> getPMCostBudget(int? projectTaskID, int? accountGroupID, int? inventoryID, int? costCodeID)
        {
            return PXSelect<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                    And<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>,
                    And<PMCostBudget.accountGroupID, Equal<Required<PMCostBudget.accountGroupID>>,
                    And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>,
                    And<PMCostBudget.costCodeID, Equal<Required<PMCostBudget.costCodeID>>>>>>
                >>
                        .Select(Base, Base.Document.Current.ProjectID, projectTaskID, accountGroupID, inventoryID, costCodeID);
        }

        private bool IsValidKey(PMChangeOrderBudget record)
        {
            if (record == null)
                return false;

            if (record.CostCodeID == null)
                return false;

            if (record.InventoryID == null)
                return false;

            if (record.AccountGroupID == null)
                return false;

            if (record.TaskID == null)
                return false;

            if (record.ProjectID == null)
                return false;

            return true;
        }

        private bool IsValidKey(PMChangeOrderLine record)
        {
            return !(record == null || record.CostCodeID == null || record.InventoryID == null || record.TaskID == null || record.ProjectID == null);
        }

        /// <summary>
        /// ValueTuple : P1 = ProjectID, P2 = ProjectTaskID, P3 = CostCodeID, P4 = AccountGroupID, P5 = InventoryID
        /// curCORefNbr -> Can't think about itself in calculation.
        /// </summary>
        public virtual (decimal qty, decimal amount) CalcRemainingAvailValue(PXGraph graph, ValueTuple<int?, int?, int?, int?, int?> valueTuple, string budCORefNbr, string comCORefNbr, string reqOrderNbr)
        {
            PMBudget budget = GetPMBudget(valueTuple.Item1, valueTuple.Item2, valueTuple.Item4, valueTuple.Item3, valueTuple.Item5, graph);

            decimal? qty = budget?.RevisedQty ?? 0m;
            decimal? amt = budget?.RevisedAmount ?? 0m;

            List<PMChangeOrderCostBudget> budgets = new List<PMChangeOrderCostBudget>();

            if (string.IsNullOrEmpty(budCORefNbr))
            {
                budgets = graph.Caches[typeof(PMChangeOrderCostBudget)].Inserted.RowCast<PMChangeOrderCostBudget>().Where(w => w.ProjectID == valueTuple.Item1 && w.ProjectTaskID == valueTuple.Item2 &&
                                                                                                                               w.CostCodeID == valueTuple.Item3 && w.AccountGroupID == valueTuple.Item4 &&
                                                                                                                               w.InventoryID == valueTuple.Item5).ToList();
                qty += budgets.Sum(s => s.Qty) ?? 0m;
                amt += budgets.Sum(s => s.Amount) ?? 0m;
            }

            budgets = SelectFrom<PMChangeOrderCostBudget>//.InnerJoin<PMChangeOrder>.On<PMChangeOrder.refNbr.IsEqual<PMChangeOrderCostBudget.refNbr>>
                                                         //.And<PMChangeOrder.released.IsEqual<False>
                                                         //    .And<PMChangeOrderExt.usrApprovalStatus.IsEqual<CR.ApprovalStatusAttribute.approved>>>>
                                                        .Where<PMChangeOrderCostBudget.projectID.IsEqual<@P.AsInt>
                                                        .And<PMChangeOrderCostBudget.projectTaskID.IsEqual<@P.AsInt>
                                                            .And<PMChangeOrderCostBudget.costCodeID.IsEqual<@P.AsInt>>
                                                                    .And<PMChangeOrderCostBudget.accountGroupID.IsEqual<@P.AsInt>
                                                                        .And<PMChangeOrderCostBudget.inventoryID.IsEqual<@P.AsInt>
                                                                            .And<PMChangeOrderCostBudget.refNbr.IsNotLike<@P.AsString>>>>>>
                                                        .View.Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5, budCORefNbr).RowCast<PMChangeOrderCostBudget>().ToList();

            qty += budgets.Sum(s => s.Qty) ?? 0m;
            amt += budgets.Sum(s => s.Amount) ?? 0m;

            if (budget?.GetExtension<PMBudgetExt>()?.UsrInvPrType == "1") // It's from Attributes [INVPRTYPE] which means 零星
            {
                POLine line = SelectFrom<POLine>.Where<POLine.projectID.IsEqual<@P.AsInt>
                                                       .And<POLine.taskID.IsEqual<@P.AsInt>
                                                            .And<POLine.costCodeID.IsEqual<@P.AsInt>
                                                                 .And<POLine.expenseAcctID.IsEqual<@P.AsInt>
                                                                      .And<POLine.inventoryID.IsEqual<@P.AsInt>
                                                                           .And<POLine.cancelled.IsEqual<False>
                                                                                .And<POLine.orderType.IsEqual<POOrderType.regularSubcontract>>>>>>>>
                                                .AggregateTo<Sum<POLine.orderQty,
                                                                Sum<POLine.curyExtCost>>>.View
                                                .Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5);

                qty -= line?.OrderQty ?? 0m;
                amt -= line?.CuryExtCost ?? 0m;
            }
            else
            {
                RQRequestLine reqLine = SelectFrom<RQRequestLine>.Where<RQRequestLineExt.usrContractID.IsEqual<@P.AsInt>
                                                                        .And<RQRequestLineExt.usrProjectTaskID.IsEqual<@P.AsInt>
                                                                             .And<RQRequestLineExt.usrCostCodeID.IsEqual<@P.AsInt>
                                                                                  .And<RQRequestLineExt.usrAccountGroupID.IsEqual<@P.AsInt>
                                                                                       .And<RQRequestLine.inventoryID.IsEqual<@P.AsInt>
                                                                                            .And<RQRequestLine.cancelled.IsEqual<False>
                                                                                                 .And<RQRequestLine.orderNbr.IsNotEqual<@P.AsString>>>>>>>>
                                                                 .AggregateTo<Sum<RQRequestLine.orderQty,
                                                                                  Sum<RQRequestLine.curyEstExtCost>>>.View
                                                                 .Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5, reqOrderNbr);

                qty -= reqLine?.OrderQty ?? 0m;
                amt -= reqLine?.CuryEstExtCost ?? 0m;
            }

            PMChangeOrderLine cOrdLine = SelectFrom<PMChangeOrderLine>.InnerJoin<PMAccountGroup>.On<PMAccountGroup.accountID.IsEqual<PMChangeOrderLine.accountID>
                                                                                                    .And<PMAccountGroup.isExpense.IsEqual<True>>>
                                                                      .InnerJoin<PMChangeOrder>.On<PMChangeOrder.refNbr.IsEqual<PMChangeOrderLine.refNbr>>
                                                                                                   //.And<PMChangeOrder.released.IsEqual<False>
                                                                                                   //     .And<PMChangeOrderExt.usrApprovalStatus.IsEqual<CR.ApprovalStatusAttribute.approved>>>>
                                                                      .Where<PMChangeOrderLine.projectID.IsEqual<@P.AsInt>
                                                                             .And<PMChangeOrderLine.taskID.IsEqual<@P.AsInt>
                                                                                  .And<PMChangeOrderLine.costCodeID.IsEqual<@P.AsInt>
                                                                                       .And<PMAccountGroup.groupID.IsEqual<@P.AsInt>
                                                                                            .And<PMChangeOrderLine.inventoryID.IsEqual<@P.AsInt>>
                                                                                                 .And<PMChangeOrderLine.refNbr.IsNotEqual<@P.AsString>>>>>>
                                                                      .AggregateTo<Sum<PMChangeOrderLine.qty,
                                                                                       Sum<PMChangeOrderLine.amount>>>.View
                                                                      .Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5, comCORefNbr);
            qty -= cOrdLine?.Qty ?? 0m;
            amt -= cOrdLine?.Amount ?? 0m;

            if (string.IsNullOrEmpty(comCORefNbr))
            {
                List<PMChangeOrderLine> commiments = new List<PMChangeOrderLine>();

                commiments = graph.Caches[typeof(PMChangeOrderLine)].Inserted.RowCast<PMChangeOrderLine>().Where(w => w.ProjectID == valueTuple.Item1 && w.TaskID == valueTuple.Item2 &&
                                                                                                                      w.CostCodeID == valueTuple.Item3 && GetPMAccountGrpID(graph, w.AccountID) == valueTuple.Item4 &&
                                                                                                                      w.InventoryID == valueTuple.Item5).ToList();
                qty -= commiments.Sum(s => s.Qty) ?? 0m;
                amt -= commiments.Sum(s => s.Amount) ?? 0m;
            }

            return (qty.Value, amt.Value);
        }

        /// <summary>
        /// ValueTuple : P1 = ProjectID, P2 = ProjectTaskID, P3 = CostCodeID, P4 = AccountID, P5 = InventoryID
        /// 剩餘可發包預算＝成本預算變更後的複價/數量+成本預算已核可未過帳的修改單複價/數量–已建單之分包規劃複價/數量（分包）或已建單之零星合約的複價/數量（零星）–已核可未過帳之已承諾訂單變更（包含自己）追加減複價/數量。
        /// curCORefNbr -> Can't think about itself in calculation.
        /// </summary>
        public virtual (decimal qty, decimal amount) CalcRemainingUnbilledValue(PXGraph graph, ValueTuple<int?, int?, int?, int?, int?> valueTuple, string pOOrdType, string pONbr, string comCORefNbr)
        {
            POLine line = SelectFrom<POLine>.Where<POLine.projectID.IsEqual<@P.AsInt>
                                                   .And<POLine.taskID.IsEqual<@P.AsInt>
                                                        .And<POLine.costCodeID.IsEqual<@P.AsInt>
                                                             .And<POLine.expenseAcctID.IsEqual<@P.AsInt>
                                                                  .And<POLine.inventoryID.IsEqual<@P.AsInt>
                                                                       .And<POLine.cancelled.IsEqual<False>
                                                                            .And<POLine.orderType.IsEqual<@P.AsString>
                                                                                 .And<POLine.orderNbr.IsEqual<@P.AsString>>>>>>>>>
                                            .AggregateTo<Sum<POLine.orderQty,
                                                            Sum<POLine.curyExtCost>>>.View
                                            .Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5, pOOrdType, pONbr);

            PMChangeOrderLine cOrdLine = SelectFrom<PMChangeOrderLine>//.InnerJoin<PMChangeOrder>.On<PMChangeOrder.refNbr.IsEqual<PMChangeOrderLine.refNbr>>
                                                                                                   //.And<PMChangeOrder.released.IsEqual<False>
                                                                                                   //     .And<PMChangeOrderExt.usrApprovalStatus.IsEqual<CR.ApprovalStatusAttribute.approved>>>>
                                                                      .Where<PMChangeOrderLine.projectID.IsEqual<@P.AsInt>
                                                                             .And<PMChangeOrderLine.taskID.IsEqual<@P.AsInt>
                                                                                  .And<PMChangeOrderLine.costCodeID.IsEqual<@P.AsInt>
                                                                                       .And<PMChangeOrderLine.accountID.IsEqual<@P.AsInt>
                                                                                            .And<PMChangeOrderLine.inventoryID.IsEqual<@P.AsInt>>
                                                                                                 .And<PMChangeOrderLine.refNbr.IsNotEqual<@P.AsString>
                                                                                                      .And<PMChangeOrderLine.pOOrderType.IsEqual<@P.AsString>
                                                                                                           .And<PMChangeOrderLine.pOOrderNbr.IsEqual<@P.AsString>>>>>>>>
                                                                      .AggregateTo<Sum<PMChangeOrderLine.qty,
                                                                                       Sum<PMChangeOrderLine.amount>>>.View
                                                                      .Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5, comCORefNbr, pOOrdType, pONbr);

            APTran invTran = SelectFrom<APTran>.Where<APTran.projectID.IsEqual<@P.AsInt>
                                                      .And<APTran.taskID.IsEqual<@P.AsInt>
                                                           .And<APTran.costCodeID.IsEqual<@P.AsInt>
                                                                .And<APTran.accountID.IsEqual<@P.AsInt>
                                                                     .And<APTran.inventoryID.IsEqual<@P.AsInt>
                                                                          .And<APTran.tranType.IsIn<APDocType.invoice, APDocType.prepayment>
                                                                               .And<APTranExt.usrValuationType.IsEqual<ValuationTypeStringList.b>
                                                                                    .And<APTran.pOOrderType.IsEqual<@P.AsString>
                                                                                         .And<APTran.pONbr.IsEqual<@P.AsString>>>>>>>>>>
                                               .AggregateTo<Sum<APTran.qty,
                                                               Sum<APTran.curyTranAmt>>>.View
                                               .Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5, pOOrdType, pONbr);

            APTran adjTran = SelectFrom<APTran>.Where<APTran.projectID.IsEqual<@P.AsInt>
                                                      .And<APTran.taskID.IsEqual<@P.AsInt>
                                                           .And<APTran.costCodeID.IsEqual<@P.AsInt>
                                                                .And<APTran.accountID.IsEqual<@P.AsInt>
                                                                     .And<APTran.inventoryID.IsEqual<@P.AsInt>
                                                                          .And<APTran.tranType.IsEqual<APDocType.debitAdj>
                                                                               .And<APTranExt.usrValuationType.IsEqual<ValuationTypeStringList.b>
                                                                                    .And<APTran.pOOrderType.IsEqual<@P.AsString>
                                                                                         .And<APTran.pONbr.IsEqual<@P.AsString>>>>>>>>>>
                                               .AggregateTo<Sum<APTran.qty,
                                                               Sum<APTran.curyTranAmt>>>.View
                                               .Select(graph, valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4, valueTuple.Item5, pOOrdType, pONbr);

            return ( (line?.OrderQty ?? 0m) + (cOrdLine?.Qty ?? 0m) - (invTran?.Qty ?? 0m) + (adjTran?.Qty ?? 0m),
                     (line?.CuryExtCost ?? 0m) + (cOrdLine?.Amount ?? 0m) - (invTran?.CuryTranAmt ?? 0m) + (adjTran?.CuryTranAmt ?? 0m) );
        }
        #endregion
    }
}
