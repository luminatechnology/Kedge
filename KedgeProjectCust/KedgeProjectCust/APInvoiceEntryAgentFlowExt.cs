using Kedge.DAC;
using System;
using System.Web;
using PX.SM;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.PO;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PM;
using RCGV.GV.DAC;
using Branch = PX.Objects.GL.Branch;
using static PX.Objects.AP.APInvoiceEntry_Extension;

namespace Kedge
{
    public class APInvoiceEntryAgentFlow : PXGraph<APInvoiceEntryAgentFlow>
    {
        #region View
        public PXSelect<APInvoice> Document;
        public PXSelect<EPApproval> Approval;
        public PXSelect<KGFlowUploadFile> KGFlowUploadFiles;

        //APInvoice
        public PXSelect<KGFlowSubAcc, Where<KGFlowSubAcc.accNo, Equal<Current<APInvoice.refNbr>>>> KGFlowAccs;
        //APTran-計價
        public PXSelect<KGFlowSubAccMfq, Where<KGFlowSubAccMfq.accUID, Equal<Current<KGFlowSubAcc.accUID>>>> KGFlowAccMfqs;
        //KGBillPayment
        public PXSelect<KGFlowSubAccPay, Where<KGFlowSubAccPay.accUID, Equal<Current<KGFlowSubAcc.accUID>>>> KGFlowSubAccPays;
        //KGBillSummary
        public PXSelect<KGFlowSubAccDetail, Where<KGFlowSubAccDetail.accUID, Equal<Current<KGFlowSubAcc.accUID>>>> KGFlowSubAccDetails;
        //KGDeductionAPTran
        public PXSelect<KGFlowSubAccVenDed, Where<KGFlowSubAccVenDed.accUID, Equal<Current<KGFlowSubAcc.accUID>>>> KGFlowSubAccVenDeds;
        //APGuiInvoice
        public PXSelect<KGFlowSubAccInv, Where<KGFlowSubAccInv.accUID, Equal<Current<KGFlowSubAcc.accUID>>>> KGFlowSubAccInvs;
        //APTran-加款
        public PXSelect<KGFlowSubVenAdd, Where<KGFlowSubVenAdd.accUID, Equal<Current<KGFlowSubAcc.accUID>>>> KGFlowSubVenAdds;
        //分包廠商評鑑單
        public PXSelect<KGFlowSubAccVendorEvaluate, Where<KGFlowSubAccVendorEvaluate.accUID, Equal<Current<KGFlowSubAcc.accUID>>>> KGFlowSubAccVendorEvaluates;
        //分包廠商評鑑單明細
        public PXSelect<KGFlowSubAccVendorEvaluateDetail, Where<KGFlowSubAccVendorEvaluateDetail.accUID, Equal<Current<KGFlowSubAcc.accUID>>,
            And<KGFlowSubAccVendorEvaluateDetail.evaluateUID, Equal<Current<KGFlowSubAccVendorEvaluate.evaluateUID>>>>> KGFlowSubAccVendorEvaluateDetails;
        #endregion

        //#region Action
        //public PXAction<APInvoice> ViewOriginalDocument;

        //[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        //[PXLookupButton]
        //protected virtual IEnumerable viewOriginalDocument(PXAdapter adapter)
        //{
        //    //參考資料
        //    //https://asiablog.acumatica.com/2017/08/redirecting-to-external-page-from-button.html
        //    APInvoice master = Base.Document.Current;
        //    APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
        //    if (apRegisterExt.UsrReturnUrl != null)
        //        throw new Exception("Redirect7:" + apRegisterExt.UsrReturnUrl);
        //    return adapter.Get();
        //}

        //public PXAction<APInvoice> webServIntegration;
        //[PXUIField(DisplayName = "AgentFlow", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        //[PXButton(CommitChanges = true)]
        //public virtual IEnumerable WebServIntegration(PXAdapter adapter)
        //{
        //    using (PXTransactionScope ts = new PXTransactionScope())
        //    {
        //        DoAgentFlow(Base.Document.Current);
        //        Base.Actions.PressSave();
        //        ts.Complete();
        //    }
        //    return adapter.Get();
        //}
        //#endregion

        //#region Event
        //public void _(Events.RowPersisting<APInvoice> e)
        //{
        //    APInvoice row = e.Row;
        //    if (!"AP301000".Equals(PXSiteMap.CurrentScreenID)) return;
        //    if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
        //    APInvoice oldRow = (APInvoice)e.Cache.GetOriginal(row);
        //    if (oldRow == null) return;
        //    if (oldRow.Hold == true && APDocStatus.PendingApproval.Equals(row.Status) && APDocStatus.Hold.Equals(oldRow.Status))
        //    {
        //        DoAgentFlow(row);
        //    }
        //}
        //#endregion

        #region Methods
        public void DoAgentFlow(APInvoiceEntry entry, APInvoice master)
        {
            KGFlowSubAcc kglowAccs;
            EPApproval   epApproval;

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                Document.Current = master;
                DeleteAgentFlowTempData();
                // Standard workflow
                epApproval = GetEPApprovalByPending(master.NoteID);
                // Agent flow header
                kglowAccs = CreateKGFlowSubAcc(master, epApproval, entry);
                // 估驗明細
                CreateKGFlowAccMfqs(master);
                // 支票/票據
                CreateKGFlowSubAccPays(entry);
                // 扣款明細
                CreateKGFlowSubAccVenDed(master, entry);
                // 發票憑證
                CreateKGFlowSubAccInv(entry);
                // 加款明細
                CreateKGFlowSubVenAdd(master, entry);
                // 彙總資訊
                CreateKGFlowSubAccDetail(master, entry);
                // 評鑑資訊
                CreateKGFlowSubAccVendorEvaluateAndDetail(master, entry);
                // 附件 -> No-used
                //CreateKGFlowUploadFile(kglowAccs, entry);

                KGFlowAccs.Cache.Clear();
                KGFlowAccMfqs.Cache.Clear();
                KGFlowSubAccPays.Cache.Clear();
                KGFlowSubAccDetails.Cache.Clear();
                KGFlowSubAccVenDeds.Cache.Clear();
                KGFlowSubAccInvs.Cache.Clear();
                KGFlowSubVenAdds.Cache.Clear();
                KGFlowSubAccVendorEvaluates.Cache.Clear();
                KGFlowSubAccVendorEvaluateDetails.Cache.Clear();
                KGFlowUploadFiles.Cache.Clear();

                this.Actions.PressSave();
                ts.Complete();
            }
            //if (kglowAccs == null || epApproval == null) return;
            //string message = AgentFlow(master, kglowAccs, epApproval);
            //if (message != null) throw new PXException(message);
        }

        public string AgentFlow(APInvoice master, KGFlowSubAcc kglowAccs, EPApproval epApproval)
        {
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            string message = null;
            string status = null;
            AgentFlowWebServiceUtil.createdoStartURL(Accessinfo.UserName, kglowAccs.AccUID, master.ProjectID, ref status, ref message);
            if (message != null) return message;

            //status is Call Web Servie return value
            if (status != null && status.StartsWith("http"))
            {
                if (epApproval != null)
                {
                    EPApprovalExt epApprovalExt = PXCache<EPApproval>.GetExtension<EPApprovalExt>(epApproval);
                    epApprovalExt.UsrReturnUrl = status;
                    epApproval = Approval.Update(epApproval);
                    apRegisterExt.UsrReturnUrl = status;
                    apRegisterExt.UsrKGFlowUID = kglowAccs.AccUID;
                    //add by louis 202011223 先執行SelectTimeStamp()去更新timestamp, 已解決存檔時會有another process is update...的錯誤
                    this.SelectTimeStamp();
                    kglowAccs.ApprovalID = epApproval.ApprovalID;
                    KGFlowAccs.Update(kglowAccs);
                    Document.Update(master);
                    this.Actions.PressSave();
                    return null;
                }
                return "簽核失敗:" + status;
            }
            return "呼叫AgentFlow失敗:" + status;
        }

        public KGFlowSubAcc CreateKGFlowSubAcc(APInvoice master, EPApproval epApproval, APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
            KGFlowSubAcc kglowAccs = KGFlowAccs.Current ?? (KGFlowSubAcc)KGFlowAccs.Cache.CreateInstance();
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            Branch branch = GetBranch(master.BranchID);
            kglowAccs.DeptName = branch?.AcctName;

            Contract contract = GetContract(master.ProjectID);
            kglowAccs.ProjectName = contract?.Description;
            kglowAccs.ProjectCode = contract?.ContractCD;

            string orderType = entryExt.getPOOrderType();
            string poNbr = entryExt.getPoNbr();
            kglowAccs.SubCode = contract.ContractCD.Trim() + "-" + poNbr;
            POOrder poOrder = GetPOOrder(poNbr, orderType);
            if (poOrder != null)
            {
                kglowAccs.SubName = poOrder.OrderDesc;
                kglowAccs.Ctotal = poOrder.CuryOrderTotal;
                POOrderExt poorderext = PXCache<POOrder>.GetExtension<POOrderExt>(poOrder);
                kglowAccs.SubUID = poorderext.UsrKGFlowUID;
                ApproveValue approveValue = entryExt.getApproveValue(poNbr, orderType, "D", master.RefNbr);
                kglowAccs.SumDedApproveValue = approveValue.SumDedApproveValue;
                kglowAccs.NoDedApproveValue = approveValue.NoDedApproveValue;
                kglowAccs.DedApproveNoAccValue = approveValue.DedApproveNoAccValue;
                kglowAccs.DedApproveAccValue = approveValue.DedApproveAccValue;
            }

            BAccount bAccount = GetBAccount(master.VendorID);
            kglowAccs.Title = bAccount?.AcctName;
            kglowAccs.Num = apRegisterExt.UsrValuationPhase;
            kglowAccs.AccNo = master.RefNbr;
            kglowAccs.Sdate = apRegisterExt.UsrBillDateFrom;
            kglowAccs.Fdate = apRegisterExt.UsrBillDateTo;
            kglowAccs.SubmittalDate = master.DocDate;
            kglowAccs.SubAccCreateDate = master.DocDate;
            kglowAccs.Stateuid = "62000";

            KGBillSummary sum = entryExt.SummaryAmtFilters.Current;
            //暫緩
            if (sum != null)
            {
                kglowAccs.Total = sum.PoValuationAmt;
                kglowAccs.Tax = sum.TaxAmt;
                kglowAccs.Realpay = sum.TotalAmt;
                kglowAccs.Reserve = sum.RetentionAmt;
                kglowAccs.DedApprove = sum.DeductionAmt;
                kglowAccs.AddApprove = sum.AdditionAmt;
                if (APDocType.DebitAdj.Equals(master.DocType)
                    && APDocType.Invoice.Equals(master.OrigDocType)
                    && master.OrigRefNbr != null) {
                    kglowAccs.Total = (kglowAccs.Total??0m)*-1;
                    kglowAccs.Tax = (sum.TaxAmt??0m) * -1;
                    kglowAccs.Realpay = (sum.TotalAmt??0m) * -1;
                    kglowAccs.Reserve = (sum.RetentionAmt??0m) * -1;
                    kglowAccs.DedApprove = (sum.DeductionAmt??0m) * -1;
                    kglowAccs.AddApprove = (sum.AdditionAmt??0m) * -1;
                }
            }
            kglowAccs.ResTax = master.DefRetainagePct;


            kglowAccs.FlowInitiator = GetUsrers(master.CreatedByID)?.FullName;

            //add by Alton 20191118
            string orderNbr = apRegisterExt.UsrPONbr;
            kglowAccs.RvnSumAddAmount = entryExt.getRvnSumAddAmount(orderNbr);//累積追加減金額
            kglowAccs.RvnTotalExTax = entryExt.getRvnTotalExTax(orderNbr);//變更後金額
            //原合約金額 = 變更後金額 - 累績追加減金額
            kglowAccs.Ctotal = (kglowAccs.RvnTotalExTax ?? 0) - (kglowAccs.RvnSumAddAmount ?? 0);
            kglowAccs.BudAmount = entryExt.getBudAmount(orderNbr);//預算金額

            //2020/01/16 追加專案類別
            CSAnswers answers = GetCSAnswers(master.ProjectID);
            kglowAccs.ClassifyCode = answers?.Value;
            //20200117 AccCategory
            switch (apRegisterExt.UsrBillCategory)
            {
                case "5"://零星雜支
                    kglowAccs.AccCategory = "1";
                    break;
                case "1"://零星水電
                    kglowAccs.AccCategory = "2";
                    break;
                case "4"://零星薪資
                    kglowAccs.AccCategory = "3";
                    break;
                case "6"://水電工程
                    kglowAccs.AccCategory = "4";
                    break;
                case "3"://零星庶務
                    kglowAccs.AccCategory = "2";
                    break;
                case "2"://零星差旅類  add by louis
                    kglowAccs.AccCategory = "5";
                    break;
                case "23"://零星雜支二
                    kglowAccs.AccCategory = "6";
                    break;
                default:
                    break;
            }
            //kglowAccs.ApprovalID = epApproval.ApprovalID;
            kglowAccs = (KGFlowSubAcc)KGFlowAccs.Cache.Insert(kglowAccs);
            KGFlowAccs.Cache.PersistInserted(kglowAccs);
            return kglowAccs;

        }

        public void CreateKGFlowAccMfqs(APInvoice master)
        {
            //add by Alton 20191114 需抓所有PO
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);

            foreach (PXResult<POLine, APRegister, InventoryItem, APTran> data in GetKGFlowAccMfqsData(master.RefNbr))
            {
                InventoryItem inventoryItem = data;
                POLine poLine = data;

                KGFlowSubAccMfq kgFlowSubAccMfq = (KGFlowSubAccMfq)KGFlowAccMfqs.Cache.CreateInstance();
                //kgFlowSubAccMfq.ItemName = inventoryItem.Descr;//工程項目名稱
                //改抓Poline
                kgFlowSubAccMfq.ItemName = poLine?.TranDesc;//工程項目名稱
                kgFlowSubAccMfq.PccesCode = inventoryItem?.InventoryCD;//工料號碼
                kgFlowSubAccMfq.ItemUnit = poLine?.UOM;//單位

                PMChangeOrderLine changeLine = GetChangeOrderLine(poLine.OrderNbr, poLine.OrderType, poLine.LineNbr);
                decimal? changeAmt = changeLine?.Amount ?? 0;
                //decimal? changeQty = changeLine?.Qty ?? 0;

                //20200226 mark by Alton mantis:11532，預算要是訂單變更後的
                //kgFlowSubAccMfq.SubItemQty = getValue(poLine.OrderQty) - changeQty;//合約數量
                //kgFlowSubAccMfq.SubItemAmt = getValue(poLine.CuryLineAmt) - changeAmt;//合約復價
                kgFlowSubAccMfq.SubItemQty = poLine?.OrderQty ?? 0;//合約數量
                //modify by louis 20200820 for 合約單價改顯示合約修改前的複價
                kgFlowSubAccMfq.SubItemAmt = (poLine?.CuryLineAmt ?? 0) - changeAmt;//合約復價
                kgFlowSubAccMfq.SubItemCost = poLine?.CuryUnitCost ?? 0;//合約單價
                kgFlowSubAccMfq.AddAmt = changeAmt;//追加金額

                //透過refNbr+Po.LineNbr抓取APTran相關資料，當APTran無資料則透過Po.Line sum APTran取得前期&累計 ( 本期單價帶合約單價，數量與復價為0)
                APTran apTran = data;
                if (apTran.RefNbr != null)
                {
                    APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                    kgFlowSubAccMfq.PreQty = apTranExt.UsrAccumulateQty ?? 0;//前期數量
                    kgFlowSubAccMfq.PreAmt = apTranExt.UsrAccumulateAmt ?? 0;//前期復價

                    
                    kgFlowSubAccMfq.ItemCost = apTran.CuryUnitCost ?? 0;//本期單價
                    //借方調整計價單（負估驗）寫入中介Table的本期復價請轉成負數
                    if (master.DocType == APDocType.DebitAdj)
                    {
                        kgFlowSubAccMfq.ItemQty = -1 * apTran.Qty ?? 0;//本期數量
                        kgFlowSubAccMfq.ItemAmt = -1 * apTran.CuryLineAmt ?? 0;//本期復價
                    }
                    else {
                        kgFlowSubAccMfq.ItemQty = apTran.Qty ?? 0;//本期數量
                        kgFlowSubAccMfq.ItemAmt = apTran.CuryLineAmt ?? 0;//本期復價
                    }
                        

                    kgFlowSubAccMfq.AccQty = apTranExt.UsrTotalAccumulateQty ?? 0;//累計數量
                    kgFlowSubAccMfq.AccAmt = apTranExt.UsrTotalAccumulateAmt ?? 0;//累計復價

                    //add by alton 20200121 如果為一式項，數量需依據復價計算 數量 = 復價/合約復價
                    // NEW per spec [Kedge_SASD_計價系統改善需求規格] -> 一式項數量寫入中介Table固定寫入1
                    if (apTran.UOM == "式")
                    {
                        //if (kgFlowSubAccMfq.SubItemAmt != 0)
                        //{
                        //    kgFlowSubAccMfq.PreQty = kgFlowSubAccMfq.PreAmt / kgFlowSubAccMfq.SubItemAmt;//前期數量
                        //    kgFlowSubAccMfq.ItemQty = kgFlowSubAccMfq.ItemAmt / kgFlowSubAccMfq.SubItemAmt;//本期數量
                        //    kgFlowSubAccMfq.AccQty = kgFlowSubAccMfq.AccAmt / kgFlowSubAccMfq.SubItemAmt;//累計數量
                        //}
                        ////add by alton:12131 2021-07-07 原本沒有此合約項目，透過訂單變更新增走這段
                        //else if (kgFlowSubAccMfq.AddAmt != 0)
                        //{
                        //    kgFlowSubAccMfq.PreQty = kgFlowSubAccMfq.PreAmt / kgFlowSubAccMfq.AddAmt;//前期數量
                        //    kgFlowSubAccMfq.ItemQty = kgFlowSubAccMfq.ItemAmt / kgFlowSubAccMfq.AddAmt;//本期數量
                        //    kgFlowSubAccMfq.AccQty = kgFlowSubAccMfq.AccAmt / kgFlowSubAccMfq.AddAmt;//累計數量
                        //}
                        //else
                        //{
                        //    kgFlowSubAccMfq.PreQty = 0;//前期數量
                        //    kgFlowSubAccMfq.ItemQty = 0;//本期數量
                        //    kgFlowSubAccMfq.AccQty = 0;//累計數量
                        //}

                        //前期數量, 本期數量, 累計數量, 如果本期有計價金額, 則一式項數量固定寫入1
                        if (kgFlowSubAccMfq.PreAmt > 0)
                        {
                            kgFlowSubAccMfq.PreQty = 1;
                        }
                        if (kgFlowSubAccMfq.ItemAmt > 0)
                        {
                            kgFlowSubAccMfq.ItemQty = 1;
                            //一式項本期單價等於本期複價
                            kgFlowSubAccMfq.ItemCost = apTran.CuryLineAmt ?? 0;
                        }
                        if (kgFlowSubAccMfq.AccAmt > 0)
                        {
                            kgFlowSubAccMfq.AccQty = 1;
                        }

                    }
                }
                else
                {
                    APTran poAPTran = GetAPTranSumByPO(poLine.OrderNbr, poLine.LineNbr, poLine.OrderType, apRegisterExt.UsrValuationPhase);
                    kgFlowSubAccMfq.PreQty = poAPTran?.Qty ?? 0;//前期數量
                    kgFlowSubAccMfq.PreAmt = poAPTran?.CuryLineAmt ?? 0;//前期復價
                    //在此無本期數量，因此本期復價與數量皆為0，本期單價同合約單價
                    kgFlowSubAccMfq.ItemQty = 0;//本期數量
                    kgFlowSubAccMfq.ItemCost = kgFlowSubAccMfq.SubItemCost;//本期單價
                    kgFlowSubAccMfq.ItemAmt = 0;//本期復價
                    kgFlowSubAccMfq.AccQty = (kgFlowSubAccMfq.PreQty ?? 0) + kgFlowSubAccMfq.ItemQty;//累計數量
                    kgFlowSubAccMfq.AccAmt = (kgFlowSubAccMfq.PreAmt ?? 0) + kgFlowSubAccMfq.ItemAmt;//累計復價
                }

                // 預付款 & 保留款退回計價單寫入中介Table的本期計價數量、金額寫入零
                if (master.DocType == APDocType.Prepayment || master.IsRetainageDocument == true)
                {
                    // 本期數量, 本期復價
                    kgFlowSubAccMfq.ItemQty = kgFlowSubAccMfq.ItemAmt = 0;
                }

                kgFlowSubAccMfq.Percentage = 0;
                //合約復價=(合約付價+追加金額)
                decimal? poAmt = ((kgFlowSubAccMfq.SubItemAmt ?? 0) + (kgFlowSubAccMfq.AddAmt ?? 0));
                //百分比=累計復價/合約復價
                if (kgFlowSubAccMfq.AccAmt != null && kgFlowSubAccMfq.SubItemAmt != null && poAmt != 0)
                {
                    //modify by louis for kgFlowSubAccMfq.Percentage didn't include changeAmt
                    //20201023 midify by Alton 累計百分比調整為 累計付價/(合約付價+追加金額)
                    kgFlowSubAccMfq.Percentage = kgFlowSubAccMfq.AccAmt / poAmt;

                }

                // 借方調整計價單（負估驗 / 扣款）寫入中介Table的累計金額請轉成負數
                /**
                if (master.DocType == APDocType.DebitAdj)
                {
                    kgFlowSubAccMfq.AccAmt = -1 * kgFlowSubAccMfq.AccAmt;
                }**/

                kgFlowSubAccMfq = (KGFlowSubAccMfq)KGFlowAccMfqs.Cache.Insert(kgFlowSubAccMfq);
                KGFlowAccMfqs.Cache.PersistInserted(kgFlowSubAccMfq);
            }
        }

        public void CreateKGFlowSubAccPays(APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();

            bool isDebitAdj = entry.Document.Current?.DocType == APDocType.DebitAdj;

            foreach (KGBillPayment kgBillPayment in entryExt.KGBillPaym.Select())
            {
                KGFlowSubAccPay kgFlowSubAccPay = (KGFlowSubAccPay)KGFlowSubAccPays.Cache.CreateInstance();
                //要看到Label   CostName XX
                kgFlowSubAccPay.CostName = Kedge.DAC.PaymentMethod.getLabel(kgBillPayment.PaymentMethod);
                //20191022
                kgFlowSubAccPay.PayName  = kgBillPayment.PaymentPct + "% " + kgBillPayment.PaymentPeriod + "天";
                // 借方調整計價單付款方法寫入中介table的金額請轉成負數
                kgFlowSubAccPay.PayAmt   = isDebitAdj == false ? kgBillPayment.PaymentAmount : -1 * kgBillPayment.PaymentAmount;
                kgFlowSubAccPay.Title    = kgBillPayment.CheckTitle;
                kgFlowSubAccPay.CashDate = kgBillPayment.PaymentDate;
                kgFlowSubAccPay.AccMemo  = kgBillPayment.Remark;

                kgFlowSubAccPay = (KGFlowSubAccPay)KGFlowSubAccPays.Cache.Insert(kgFlowSubAccPay);
                KGFlowSubAccPays.Cache.PersistInserted(kgFlowSubAccPay);
            }
        }

        public void CreateKGFlowSubAccVenDed(APInvoice master, APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
            foreach (KGDeductionAPTran dedLine in entryExt.deductionAPTranDetails.Select())
            {
                if (dedLine.IsManageFee == false || dedLine.IsManageFee == null)
                {
                    KGFlowSubAccVenDed kgFlowSubAccVenDedy = (KGFlowSubAccVenDed)KGFlowSubAccVenDeds.Cache.CreateInstance();
                    KGValuation kgValuation = GetKGValuation(dedLine.ValuationID);
                    if (kgValuation != null)
                    {
                        kgFlowSubAccVenDedy.VenName = kgValuation.Description;
                        kgFlowSubAccVenDedy.VenUnit = kgValuation.Uom;
                        //kgFlowSubAccVenDedy.DedValue = kgValuation.TotalAmt;
                        kgFlowSubAccVenDedy.NoManagefareDesc = kgValuation.ValuationContent;
                        kgFlowSubAccVenDedy.VenDate = kgValuation.ValuationDate;

                        KGValuationDetail kgValuationDetailD = GetKGValuationDetail(kgValuation.ValuationID, true);
                        KGValuationDetail kgValuationDetailA = GetKGValuationDetail(kgValuation.ValuationID, false);

                        //KGValuationDetail kgValuationDetails = getKGValuationDetail(kgValuation.ValuationID, "D");//mark by alton 20191120 需抓加款與扣款

                        if (kgValuationDetailD != null)
                        {

                            kgFlowSubAccVenDedy.DedValue = (kgValuationDetailD.TotalAmt ?? 0) + (kgValuationDetailD.ManageFeeTotalAmt ?? 0);
                            kgFlowSubAccVenDedy.Managefare = kgValuationDetailD.ManageFeeAmt ?? 0;
                            kgFlowSubAccVenDedy.ManagefarTax = kgValuationDetailD.ManageFeeTaxAmt ?? 0;
                            //20191022
                            Contract contract = GetContract(master.ProjectID);
                            if (contract != null && kgValuationDetailA != null)
                            {
                                kgFlowSubAccVenDedy.AddSubCode = contract.ContractCD.Trim() + "-" + kgValuationDetailA.OrderNbr;//edit by alton 20191120 合約編號orderNbr 抓加款的
                            }
                        }
                        kgFlowSubAccVenDedy = (KGFlowSubAccVenDed)KGFlowSubAccVenDeds.Cache.Insert(kgFlowSubAccVenDedy);
                        KGFlowSubAccVenDeds.Cache.PersistInserted(kgFlowSubAccVenDedy);
                    }
                }
            }
        }

        public void CreateKGFlowSubAccInv(APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
            foreach (GVApGuiInvoiceRef gvApGuiInvoice in entryExt.GVApGuiInvoiceRefs.Select())
            {
                KGFlowSubAccInv kgFlowSubAccInv = (KGFlowSubAccInv)KGFlowSubAccInvs.Cache.CreateInstance();
                kgFlowSubAccInv.InvNo = gvApGuiInvoice.GuiInvoiceNbr;
                kgFlowSubAccInv.InvAmt = gvApGuiInvoice.SalesAmt ?? 0;
                kgFlowSubAccInv.InvTax = gvApGuiInvoice.TaxAmt ?? 0;
                kgFlowSubAccInv.InvTotal = kgFlowSubAccInv.InvAmt + kgFlowSubAccInv.InvTax;
                kgFlowSubAccInv.InvDate = gvApGuiInvoice.InvoiceDate;
                if (gvApGuiInvoice.GuiType != null)
                {
                    GVGuiType guiType = GetGVGuiType(gvApGuiInvoice.GuiType);
                    if (guiType != null)
                    {
                        kgFlowSubAccInv.KindName = gvApGuiInvoice.GuiType + "." + guiType.GuiTypeDesc;
                    }
                }
                kgFlowSubAccInv.InvoiceNo = gvApGuiInvoice.VendorUniformNumber;
                kgFlowSubAccInv.Memo = gvApGuiInvoice.Remark;
                kgFlowSubAccInv = (KGFlowSubAccInv)KGFlowSubAccInvs.Cache.Insert(kgFlowSubAccInv);
                KGFlowSubAccInvs.Cache.PersistInserted(kgFlowSubAccInv);
            }
        }

        public void CreateKGFlowSubVenAdd(APInvoice master, APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            foreach (APTran apTran in entry.Transactions.Select())
            {
                APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                //加款
                if ("A".Equals(apTranExt.UsrValuationType))
                {
                    KGFlowSubVenAdd kgFlowSubVenAdd = (KGFlowSubVenAdd)KGFlowSubVenAdds.Cache.CreateInstance();
                    KGValuation kgValuation = GetKGValuation(apTranExt.UsrValuationID);
                    if (kgValuation != null)
                    {
                        kgFlowSubVenAdd.VenName = kgValuation.Description;
                        kgFlowSubVenAdd.VenUnit = kgValuation.Uom;

                        KGValuationDetail kgValuationDetails = entryExt.getKGValuationDetail(apTranExt.UsrValuationID, "A");
                        kgFlowSubVenAdd.AddSubCode = kgValuationDetails?.OrderNbr;
                        kgFlowSubVenAdd.AddValue = kgValuationDetails?.TotalAmt ?? 0;

                        kgFlowSubVenAdd.AddNum = apRegisterExt.UsrValuationPhase;
                        kgFlowSubVenAdd.VenDate = master.DocDate;
                    }
                    kgFlowSubVenAdd = (KGFlowSubVenAdd)KGFlowSubVenAdds.Cache.Insert(kgFlowSubVenAdd);
                    KGFlowSubVenAdds.Cache.PersistInserted(kgFlowSubVenAdd);
                }
            }
        }

        public void CreateKGFlowSubAccDetail(APInvoice master, APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
            APInvoiceEntryBillSummaryExt entryBillExt = entry.GetExtension<APInvoiceEntryBillSummaryExt>();
            APRegisterExt masterExt = entry.Document.Cache.GetExtension<APRegisterExt>(master);
            KGBillSummary thisSum = entryExt.getKGBillSummary();
            decimal? taxRate = entryExt.getTaxRate(master.RefNbr) + 1;

            #region 前期累計
            KGFlowSubAccDetail previousDetail = (KGFlowSubAccDetail)KGFlowSubAccDetails.Cache.CreateInstance();
            //2021-11-09 邏輯改用KGBillSummary
            decimal? retentionWithTaxCumAmt = thisSum.RetentionWithTaxCumAmt ?? 0;
            decimal? retentionReturnWithTaxCumAmt = thisSum.RetentionReturnWithTaxCumAmt ?? 0;
            previousDetail.ReserveDedReturnReserve = retentionWithTaxCumAmt - retentionReturnWithTaxCumAmt;
            decimal? prepaymentCumAmt = thisSum.PrepaymentCumAmt ?? 0;
            decimal? prepaymentDuctCumAmt = thisSum.PrepaymentDuctCumAmt ?? 0;
            previousDetail.DedReturnPrePay = prepaymentCumAmt - prepaymentDuctCumAmt;
            previousDetail.AddValue = thisSum.AdditionCumAmt ?? 0;

            decimal? ven2CumValue = 0;
            decimal? dedCumValue = 0;
            decimal? ven2CumTax = 0;
            decimal? dedCumTax = 0;

            foreach (KGBillSummary cumSum in entryBillExt.GetCumKGBillSummary(thisSum, master, masterExt))
            {
                //Edit by Alton 扣墊 & 扣款 含稅
                ven2CumValue += cumSum.InsDeductionAmt ?? 0;
                dedCumValue += cumSum.StdDeductionAmt ?? 0;
                ven2CumTax += GetThisTaxValue(cumSum.RefNbr, true, entryExt);
                dedCumTax += GetThisTaxValue(cumSum.RefNbr, false, entryExt);
                //modify by louis 20211223 for 前期累計金額為0的問題
                previousDetail.TaxValue = (previousDetail.TaxValue??0) + cumSum.TaxAmt ?? 0;
                previousDetail.RealPayValue = (previousDetail.RealPayValue ?? 0)+cumSum.TotalAmt;
                previousDetail.InvoiceValue = (previousDetail.InvoiceValue ?? 0)+cumSum.GvInvWithTaxAmt ?? 0;
            }
            previousDetail.Ven2Value = entryExt.round(ven2CumValue + ven2CumTax);
            previousDetail.DedValue = entryExt.round(dedCumValue + dedCumTax);
            previousDetail.Sno = 1;
            previousDetail.FundsName = "前期累計";
            //金額是負的要再轉一次
            previousDetail.WithholdingTax = -entryExt.GetPreviousWithholdingTax(master);
            previousDetail = (KGFlowSubAccDetail)KGFlowSubAccDetails.Cache.Insert(previousDetail);
            KGFlowSubAccDetails.Cache.PersistInserted(previousDetail);
            #endregion

            #region 本期累計
            KGFlowSubAccDetail detail = (KGFlowSubAccDetail)KGFlowSubAccDetails.Cache.CreateInstance();
            //detail.ReserveDedReturnReserve = (thisSum.RetentionWithTaxAmt ?? 0) - (thisSum.RetentionReturnWithTaxAmt ?? 0);
            detail.DedReturnPrePay = (thisSum.PrepaymentAmt ?? 0) - (thisSum.PrepaymentDuctAmt ?? 0);

            //Edit by Alton 扣墊 & 扣款 含稅
            detail.Ven2Value = entryExt.round((thisSum.InsDeductionAmt ?? 0) + GetThisTaxValue(thisSum.RefNbr, true, entryExt));
            detail.DedValue = entryExt.round((thisSum.StdDeductionAmt ?? 0) + GetThisTaxValue(thisSum.RefNbr, false, entryExt));

            detail.AddValue = thisSum.AdditionAmt ?? 0;
            //detail.TaxValue = thisSum.TaxAmt ?? 0;

            // 借方調整計價單（負估驗 / 扣款）寫入中介Table的實付金額 / 累計金額請轉成負數
            detail.ReserveDedReturnReserve = (thisSum.RetentionWithTaxAmt ?? 0m) - (thisSum.RetentionReturnWithTaxAmt ?? 0m);
            detail.ReserveDedReturnReserve = master.DocType != APDocType.DebitAdj ? detail.ReserveDedReturnReserve : -1 * detail.ReserveDedReturnReserve;
            detail.TaxValue = master.DocType != APDocType.DebitAdj ? thisSum.TaxAmt ?? 0m : -1 * (thisSum.TaxAmt ?? 0m);
            detail.RealPayValue = master.DocType != APDocType.DebitAdj ? thisSum.TotalAmt ?? 0m : -1 * (thisSum.TotalAmt ?? 0m);
            detail.InvoiceValue = master.DocType != APDocType.DebitAdj ? thisSum.GvInvWithTaxAmt ?? 0m : -1 * (thisSum.GvInvWithTaxAmt ?? 0m);
            //detail.InvoiceValue = thisSum.GvInvWithTaxAmt ?? 0;
            detail.Sno = 2;
            detail.FundsName = "本期款項";
            //-----------------計算------------------------------
            Decimal? sumWithholdingTax = 0;
            foreach (APTran apTran in entry.Transactions.Select())
            {
                APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);

                if ("W".Equals(apTranExt.UsrValuationType) || "S".Equals(apTranExt.UsrValuationType))
                {
                    sumWithholdingTax += (apTran.CuryLineAmt ?? 0);
                }
            }
            //金額是負的要再轉一次
            detail.WithholdingTax = -sumWithholdingTax;
            detail = (KGFlowSubAccDetail)KGFlowSubAccDetails.Cache.Insert(detail);
            KGFlowSubAccDetails.Cache.PersistInserted(detail);
            #endregion

            #region 總累計累計
            KGFlowSubAccDetail totelDetail = (KGFlowSubAccDetail)KGFlowSubAccDetails.Cache.CreateInstance();
            totelDetail.ReserveDedReturnReserve = previousDetail.ReserveDedReturnReserve + detail.ReserveDedReturnReserve;
            totelDetail.DedReturnPrePay = previousDetail.DedReturnPrePay + detail.DedReturnPrePay;
            totelDetail.Ven2Value = previousDetail.Ven2Value + detail.Ven2Value;
            totelDetail.DedValue = previousDetail.DedValue + detail.DedValue;
            totelDetail.AddValue = previousDetail.AddValue + detail.AddValue;
            totelDetail.TaxValue = previousDetail.TaxValue + detail.TaxValue;
            totelDetail.RealPayValue = previousDetail.RealPayValue + detail.RealPayValue;
            totelDetail.InvoiceValue = previousDetail.InvoiceValue + detail.InvoiceValue;
            totelDetail.Sno = 3;
            totelDetail.FundsName = "累計款項";
            totelDetail.WithholdingTax = previousDetail.WithholdingTax + detail.WithholdingTax;

            totelDetail = (KGFlowSubAccDetail)KGFlowSubAccDetails.Cache.Insert(totelDetail);
            KGFlowSubAccDetails.Cache.PersistInserted(totelDetail);
            #endregion
        }

        public void CreateKGFlowSubAccVendorEvaluateAndDetail(APInvoice master, APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            KGContractEvaluation kgContractEvaluation = GetKGContractEvaluation(master.RefNbr, master.DocType);
            if (kgContractEvaluation != null)
            {
                KGFlowSubAccVendorEvaluate evaluate = (KGFlowSubAccVendorEvaluate)KGFlowSubAccVendorEvaluates.Cache.CreateInstance();
                Contract contract = GetContract(master.ProjectID);
                string poNbr = entryExt.getPoNbr();
                string orderType = entryExt.getPOOrderType();
                evaluate.SubCode = contract.ContractCD.Trim() + "-" + poNbr;

                POOrder poOrder = GetPOOrder(poNbr, orderType);
                evaluate.SubName = poOrder?.OrderDesc;

                BAccount bAccount = GetBAccount(master.VendorID);
                evaluate.Title = bAccount?.AcctName;

                KGVendorEvaluation kgVendorEvaluation = GetKGVendorEvaluation(kgContractEvaluation.EvaluationID);
                evaluate.EvaluationCode = kgVendorEvaluation?.EvaluationName;
                evaluate.QuestionDocCode = kgContractEvaluation.ContractEvaluationCD;
                evaluate.EvaluateDate = kgContractEvaluation.EvaluationDate;
                evaluate.WeightScore = kgContractEvaluation.Score;
                evaluate = (KGFlowSubAccVendorEvaluate)KGFlowSubAccVendorEvaluates.Cache.Insert(evaluate);
                KGFlowSubAccVendorEvaluates.Cache.PersistInserted(evaluate);
                foreach (KGContractEvaluationL evaluationL in GetKGContractEvaluationL(kgContractEvaluation.ContractEvaluationID))
                {
                    KGFlowSubAccVendorEvaluateDetail evaluateDetail =
                        (KGFlowSubAccVendorEvaluateDetail)KGFlowSubAccVendorEvaluateDetails.Cache.CreateInstance();

                    evaluateDetail.QuestionNo = evaluationL.QuestSeq;
                    evaluateDetail.Content = evaluationL.Quest;
                    //evaluateDetail.Notes = evaluationL.Quest;
                    evaluateDetail.Score = evaluationL.Score;
                    evaluateDetail.Memo = evaluationL.Remark;
                    evaluateDetail.Weight = evaluationL.WeightScore;
                    evaluateDetail.WeightScore = evaluationL.FinalScore;

                    evaluateDetail = (KGFlowSubAccVendorEvaluateDetail)KGFlowSubAccVendorEvaluateDetails.Cache.Insert(evaluateDetail);
                    KGFlowSubAccVendorEvaluateDetails.Cache.PersistInserted(evaluateDetail);
                }
            }
        }

        public void CreateKGFlowUploadFile(KGFlowSubAcc kglowAccs, APInvoiceEntry entry)
        {
            APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
            foreach (KGBillFile kgBillFile in entryExt.KGBillFiles.Select())
            {
                if (kgBillFile.NoteID == null) continue;

                NoteDoc notedoc = GetNoteDoc(kgBillFile.NoteID);
                if (notedoc != null)
                {
                    //該檔案可能已經新增過了
                    if (GetKGFlowUploadFile(notedoc.FileID) != null) continue;

                    KGFlowUploadFile kgFlowUploadFile = (KGFlowUploadFile)KGFlowUploadFiles.Cache.CreateInstance();
                    kgFlowUploadFile.Fileid = notedoc.FileID;

                    kgFlowUploadFile.FileType = kgBillFile.FileType;
                    kgFlowUploadFile.RefNo = kglowAccs.AccUID;
                    kgFlowUploadFile.Category = "INV";
                    UploadFile uploadFile = GetUploadFile(notedoc.FileID);
                    kgFlowUploadFile.FileName = uploadFile.Name; ;
                    string buildUtl = PXRedirectToFileException.BuildUrl(notedoc.FileID);
                    string Absouri = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;

                    buildUtl = buildUtl.Substring(1);
                    Absouri = Absouri.Substring(0, Absouri.IndexOf("/M"));
                    kgFlowUploadFile.FileLink = Absouri + buildUtl;
                    kgFlowUploadFile = (KGFlowUploadFile)KGFlowUploadFiles.Cache.Insert(kgFlowUploadFile);
                    KGFlowUploadFiles.Cache.PersistInserted(kgFlowUploadFile);
                }
            }
        }

        public void DeleteAgentFlowTempData()
        {
            foreach (KGFlowSubAcc acc in KGFlowAccs.Select())
            {
                //KGFlowAccs.Delete(acc);
                KGFlowAccs.Cache.PersistDeleted(acc);
            }
        }

        /**
        * <summary>
        * 取得當期 扣墊 or 扣款 稅額
        * </summary>
        * **/
        private decimal GetThisTaxValue(string refNbr, bool isVen2ValueTax, APInvoiceEntry_Extension entryExt)
        {

            decimal tax = 0;
            decimal taxRate = entryExt.getTaxRate(refNbr) ?? 0;
            PXResultset<KGDeductionAPTran> rs = GetKGDeductionAPTranByRefNbr(refNbr);
            foreach (PXResult<KGDeductionAPTran, KGValuation> data in rs)
            {
                KGValuation valuation = (KGValuation)data;
                KGDeductionAPTran deduction = (KGDeductionAPTran)data;
                //只計算應稅 isTaxFree = false
                if (valuation != null && !(valuation.IsTaxFree ?? false))
                {
                    if (
                        (isVen2ValueTax && valuation.ValuationType == "0") //扣墊稅額 條件
                        || (!isVen2ValueTax && valuation.ValuationType != "0")  //扣款稅額 條件
                        )
                    {
                        tax += ((deduction.CuryLineAmt ?? 0) * taxRate);
                    }
                }
            }
            return tax;
        }
        #endregion

        #region BQL
        public UploadFile GetUploadFile(Guid? fileID)
        {
            return PXSelect<UploadFile,
                        Where<UploadFile.fileID, Equal<Required<UploadFile.fileID>>>>
                        .Select(this, fileID);
        }

        public KGFlowUploadFile GetKGFlowUploadFile(Guid? fileID)
        {
            return PXSelect<KGFlowUploadFile,
                         Where<KGFlowUploadFile.fileid, Equal<Required<KGFlowUploadFile.fileid>>>>
                         .Select(this, fileID);
        }

        public NoteDoc GetNoteDoc(Guid? noteID)
        {
            return PXSelect<NoteDoc,
                     Where<NoteDoc.noteID, Equal<Required<NoteDoc.noteID>>>>
                     .Select(this, noteID);
        }

        public KGVendorEvaluation GetKGVendorEvaluation(int? evaluationID)
        {
            return PXSelect<KGVendorEvaluation,
                Where<KGVendorEvaluation.evaluationID,
                Equal<Required<KGVendorEvaluation.evaluationID>>>>
                .Select(this, evaluationID);
        }

        public PXResultset<KGContractEvaluationL> GetKGContractEvaluationL(int? contractEvaluationID)
        {
            return PXSelect<KGContractEvaluationL,
                Where<KGContractEvaluationL.contractEvaluationID, Equal<Required<KGContractEvaluationL.contractEvaluationID>>>>
                .Select(this, contractEvaluationID);
        }

        public KGContractEvaluation GetKGContractEvaluation(string refNbr, string docType)
        {
            return PXSelect<KGContractEvaluation,
                Where<KGContractEvaluation.aPRefNbr, Equal<Required<KGContractEvaluation.aPRefNbr>>,
                And<KGContractEvaluation.aPDocType, Equal<Required<KGContractEvaluation.aPDocType>>>>>
                .Select(this, refNbr, docType);
        }
        
    public PXResultset<KGDeductionAPTran> GetKGDeductionAPTranByRefNbr(string refNbr)
        {
            return PXSelectJoin<KGDeductionAPTran,
                    LeftJoin<KGValuation, On<KGDeductionAPTran.valuationID, Equal<KGValuation.valuationID>>>,
                        Where<KGDeductionAPTran.refNbr, Equal<Required<KGDeductionAPTran.refNbr>>,
                        And<KGDeductionAPTran.valuationType, NotIn3<ValuationTypeStringList.w, 
                        ValuationTypeStringList.s>>
                             >
                    >.Select(this, refNbr);
        }

        public GVGuiType GetGVGuiType(string guiType)
        {
            return PXSelect<GVGuiType,
                 Where<GVGuiType.guiTypeCD, Equal<Required<GVGuiType.guiTypeCD>>
                 >>.Select(this, guiType);
        }

        /**
         * <summary>
         * 修訂原getKGValuationDetail(int? valuationID , String pricingType) 的BQL<br/>
         * 移除 POOrder 與 KGValuation 不必要之條件
         * </summary>
         * **/
        public PXResultset<KGValuationDetail> GetKGValuationDetail(int? valuationID, bool isDeduction)
        {
            return PXSelect<KGValuationDetail,
                                Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                                      And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>>>
                                      .Select(this, valuationID, isDeduction ? "D" : "A");
        }

        public KGValuation GetKGValuation(int? valuationID)
        {
            return PXSelectReadonly<KGValuation,
                     Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>
                     .Select(this, valuationID);
        }

        /**
         * <summary>
         * 依據PO與期別，取得前期數量與復價
         * </summary>
         * **/
        private PXResultset<APTran> GetAPTranSumByPO(string orderNbr, int? poLineNbr, string orderType, int? usrValuationPhase)
        {
            return PXSelectJoinGroupBy<APTran,
                    InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTran.refNbr>>>,
                    Where<APTran.pONbr, Equal<Required<APTran.pONbr>>,//pONbr
                        And<APTran.pOLineNbr, Equal<Required<APTran.pOLineNbr>>,//pOLineNbr
                        And<APTran.pOOrderType, Equal<Required<APTran.pOOrderType>>,//pOOrderType
                        And<APRegisterExt.usrValuationPhase, Less<Required<APRegisterExt.usrValuationPhase>>,//usrValuationPhase
                        And<APRegister.docType, In3<APDocType.invoice, APDocType.debitAdj>>>>>>,
                    Aggregate<GroupBy<APTran.pOLineNbr,
                            Sum<APTran.curyLineAmt,
                            Sum<APTran.qty>>
                    >>>
                .Select(this, orderNbr, poLineNbr, orderType, usrValuationPhase);
        }

        /**
        * <summary>
        * 取得訂單變更金額
        * </summary>
        * **/
        private PXResultset<PMChangeOrderLine> GetChangeOrderLine(string orderNbr, string orderType, int? lineNbr)
        {
            return PXSelectJoinGroupBy<PMChangeOrderLine,
                    InnerJoin<PMChangeOrder, On<PMChangeOrder.refNbr, Equal<PMChangeOrderLine.refNbr>>>,
                    Where<PMChangeOrderLine.pOOrderNbr, Equal<Required<PMChangeOrderLine.pOOrderNbr>>,//POOrderNbr
                        And<PMChangeOrderLine.pOOrderType, Equal<Required<PMChangeOrderLine.pOOrderType>>,//POOrderType
                        And<PMChangeOrderLine.pOLineNbr, Equal<Required<PMChangeOrderLine.pOLineNbr>>,//POLineNbr
                        And<PMChangeOrderLine.released, Equal<Required<PMChangeOrderLine.released>>,//released
                        And<PMChangeOrder.classID, NotEqual<Required<PMChangeOrder.classID>>>//classID
                        >>>
                    >,
                    Aggregate<Sum<PMChangeOrderLine.amount, Sum<PMChangeOrderLine.qty>>>
                >.Select(this, orderNbr, orderType, lineNbr, true, "專案結算");
        }

        /**
       * <summary>
       * 透過RefNbr取得對應OrderNbr底下的POLine
       * </summary>
       * **/
        private PXResultset<POLine> GetKGFlowAccMfqsData(string refNbr)
        {
            return PXSelectJoin<POLine,
                    InnerJoin<APRegister, On<APRegisterExt.usrPONbr, Equal<POLine.orderNbr>,
                        And<APRegisterExt.usrPOOrderType, Equal<POLine.orderType>>>,
                    LeftJoin<InventoryItem, On<POLine.inventoryID, Equal<InventoryItem.inventoryID>>,
                    LeftJoin<APTran, On<POLine.inventoryID, Equal<APTran.inventoryID>,
                        And<POLine.orderNbr, Equal<APTran.pONbr>,
                        And<POLine.lineNbr, Equal<APTran.pOLineNbr>,
                        And<POLine.orderType, Equal<APTran.pOOrderType>,
                        And<APRegister.refNbr, Equal<APTran.refNbr>>>>>
                    >>>
                  >,
                    Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>//refNbr
                >.Select(this, refNbr);
        }

        public CSAnswers GetCSAnswers(int? projectID)
        {
            return PXSelectJoin<CSAnswers,
                InnerJoin<Contract,
                    On<Contract.noteID, Equal<CSAnswers.refNoteID>,
                    And<CSAnswers.attributeID, Equal<WordType.a004>,
                    And<Contract.contractID, Equal<Required<Contract.contractID>>>>>>>
            .Select(this, projectID);
        }

        public PX.SM.Users GetUsrers(Guid? id)
        {
            return PXSelect<PX.SM.Users,
                Where<PX.SM.Users.pKID, Equal<Required<PX.SM.Users.pKID>>>>
                .Select(this, id);
        }

        public BAccount GetBAccount(int? bAccountID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(this, bAccountID);
        }


        public POOrder GetPOOrder(string poNbr, string orderType)
        {
            return PXSelect<POOrder,
                Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>
                .Select(this, orderType, poNbr);
        }

        public Contract GetContract(int? projectID)
        {
            return PXSelect<Contract,
                   Where<Contract.contractID, Equal<Required<Contract.contractID>>>>
                   .Select(this, projectID);
        }

        public Branch GetBranch(int? branchID)
        {
            return PXSelect<Branch,
                Where<Branch.branchID, Equal<Required<Branch.branchID>>>>
                .Select(this, branchID);
        }

        public EPApproval GetEPApprovalByPending(Guid? noteID)
        {
            return PXSelect<EPApproval,
                    Where<EPApproval.refNoteID, Equal<Required<EPApproval.refNoteID>>,
                    And<EPApproval.status, Equal<EPApprovalStatus.pending>>>>
                    .Select(this, noteID);
        }
        #endregion
    }
}
