using System;
using System.Collections;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using Kedge.DAC;
using System.IO;
using Xceed.Words.NET;
using System.Collections.Generic;
using PX.Objects.RQ;
using PX.Objects.PM;
using PX.Objects.AP;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.CN.Compliance.CL.DAC;
using System.Web;
using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.CS;
using System.Globalization;
using System.Linq;
using Branch = PX.Objects.GL.Branch;
using UploadFile = PX.SM.UploadFile;
using static PX.Objects.FS.SM_ARReleaseProcess;

/**
 * ===2021/04/08 Mantis:0011964 === Althea
 * Add產生工程合約明細表 僅在合約類行為合約時可以使用
 * ===2021/04/13 Mantis: 0012018 === Althea
 * PMProjectCostFilter的專案加上預設為此分包的專案
 * ===2021/04/20 Mantis: 0012020 === Althea
 * KGPoPayment檢查更改: 若無專案也要檢查
 * ===2021/06/09 Mantis: 0012085 ===Althea
 * 產生工程合約明細表的日期條件 原本是塞DocDate改為系統日
 * 
 * ====2021-06-21 : 12117====Alton
 * 1.分包契約使用者執行行動中的"建立預付款”時會開啟新視窗顯示預付款計價單的畫面
 * 2.請將APTran.RetainagePct 設為 0, CuryRetainageAmt設為0, RetainageAmt設為0.
 * 
 * // add POOrder_ProjectID_FieldUpdated() isnull check (add by louis 20210310)
 **/

namespace PX.Objects.PO
{
    public class POOrderEntry_Extension : PXGraphExtension<POOrderEntry>
    {      
        public override void Initialize()
        {
            base.Initialize();
            Base.action.AddMenuAction(this.PrintContract);
            Base.action.AddMenuAction(this.PrintSimpleContract);
            Base.action.AddMenuAction(this.PrintDetailContract);
            //Base.action.AddMenuAction(this.webServIntegration);          
        }

        #region Selects
        public PXSelect<POOrder> POOrders;

        public PXSelect<POLine,
               Where<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>,
               And<POLine.orderType, Equal<Optional<POOrder.orderType>>>>> POLines;

        public PXSelect<POOrderAPDoc,
            Where<POOrderAPDoc.pOOrderType, Equal<Current<POOrder.orderType>>,
                And<POOrderAPDoc.pONbr, Equal<Current<POOrder.orderNbr>>>>> APDocs;

        public PXSelect<KGContractDoc,
                 Where<KGContractDoc.orderNbr, Equal<Current<POOrder.orderNbr>>,
                     And<KGContractDoc.orderType, Equal<Optional<POOrder.orderType>>>>> KGContractDocs;

        public PXSelect<KGContractDocTag,
               Where<KGContractDocTag.orderType, Equal<Current<POOrder.orderType>>,
               And<KGContractDocTag.orderNbr, Equal<Current<POOrder.orderNbr>>>>> KGContractDocTags;

        public PXSelect<KGContractPricingRule,
               Where<KGContractPricingRule.orderType, Equal<Current<POOrder.orderType>>,
               And<KGContractPricingRule.orderNbr, Equal<Current<POOrder.orderNbr>>>>> KGContractPricingRules;

        public PXSelect<KGContractRelatedVendor,
               Where<KGContractRelatedVendor.orderType, Equal<Current<POOrder.orderType>>,
               And<KGContractRelatedVendor.orderNbr, Equal<Current<POOrder.orderNbr>>>>> KGContractRelatedVendors;

        [PXImport(typeof(KGContractSegPricing))]
        public PXSelect<KGContractSegPricing,
                  Where<KGContractSegPricing.orderType, Equal<Current<POOrder.orderType>>,
                  And<KGContractSegPricing.orderNbr, Equal<Current<POOrder.orderNbr>>,
                  And<KGContractSegPricing.lineNbr, Equal<Current<POLine.lineNbr>>>>>> KGContractSegPricings;

        public PXSelect<KGPoOrderFile,
           Where<KGPoOrderFile.orderNbr, Equal<Current<POOrder.orderNbr>>>> KGPoOrderFiles;

        public PXSelect<KGFlowUploadFile> KGFlowUploadFiles;

        public PXSelect<KGFlowContractor,
            Where<KGFlowContractor.subCode, Equal<Current<POOrder.orderNbr>>>> KGFlowContractors;

        public PXSetup<KGSetUp> setup;

        public PXSelect<KGPoOrderPayment,
               Where<KGPoOrderPayment.orderNbr, Equal<Current<POOrder.orderNbr>>>> KGPoOrderPayments;

        [PXCopyPasteHiddenView]
        public PXFilter<PMProjectCostFilter> projectCostFilter;

        [PXFilterable]
        [PXCopyPasteHiddenView]
        public PXSelect<PMCostBudget,
                        Where<PMCostBudget.projectID, Equal<Current<PMProjectCostFilter.contractID>>,
                              And<PMBudgetExt.usrInvPrType, Equal<Zero>>>,
                        OrderBy<Asc<PMCostBudget.projectTaskID, Asc<PMCostBudget.costCodeID, Asc<PMCostBudget.accountGroupID>>>>> pmCostBudget;
        public PXSelect<PMBudget> BudgetView;
        #endregion

        #region Buttons Event
        public PXAction<PX.Objects.PO.POOrder> PrintContract;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "產生合約文件", Visibility = PXUIVisibility.Invisible, Visible = false)]
        protected IEnumerable printContract(PXAdapter adapter)
        {
            DownloadContract();
            Base.Document.View.RequestRefresh();
            return adapter.Get();
        }

        public PXAction<PX.Objects.PO.POOrder> PrintSimpleContract;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "產生簡約文件")]
        protected void printSimpleContract()
        {
            String reportID = "KG602000";
            POOrder order = POOrders.Current;
            if (order.Status == "D")
            {
                order.Status = "N";
                POOrders.Update(order);
                Base.Persist();
            }
            //改成你的報表ScreenID
            if (order != null)
            {
                Dictionary<string, string> mailParams = new Dictionary<string, string>();
                KGContractDoc contractDoc = PXSelect<KGContractDoc,
                    Where<KGContractDoc.orderNbr, Equal<Required<POOrder.orderNbr>>,
                And<KGContractDoc.orderType, Equal<Required<POOrder.orderType>>>>>
                    .Select(Base, order.OrderNbr, order.OrderType);
                DateTime docdate = (DateTime)contractDoc.ContractDocDate;
                //下面要跟你的查詢參數一樣
                mailParams["OrderType"] = order.OrderType;
                mailParams["OrderNbr"] = order.OrderNbr;
                mailParams["RunningType"] = "AUTO";
                //mailParams["CompletionDate"] = String.Format("{0:MM/dd/yyyy}", contractDoc.ContractDocDate);
                mailParams["CompletionDate"] = docdate.ToString("yyyy/MM/dd");
                throw new PXReportRequiredException(mailParams, reportID, "Report");
            }
        }

        //2019/07/30 ADD
        public PXAction<PX.Objects.PO.POOrder> PrintDetailContract;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "產生工程合約明細表")]
        protected void printDetailContract()
        {
            String reportID = "KG602000";
            POOrder order = POOrders.Current;         
            //改成你的報表ScreenID
            if (order != null)
            {
                Dictionary<string, string> mailParams = new Dictionary<string, string>();
                KGContractDoc contractDoc = PXSelect<KGContractDoc,
                    Where<KGContractDoc.orderNbr, Equal<Required<POOrder.orderNbr>>,
                And<KGContractDoc.orderType, Equal<Required<POOrder.orderType>>>>>
                    .Select(Base, order.OrderNbr, order.OrderType);
                //下面要跟你的查詢參數一樣
                DateTime docdate = (DateTime)contractDoc.ContractDocDate;
                mailParams["OrderType"] = order.OrderType;
                mailParams["OrderNbr"] = order.OrderNbr;
                mailParams["RunningType"] = "MANUAL";
                //2021/06/09 Louis說DocDate改為bussinessDate
                mailParams["CompletionDate"] = ((DateTime)Base.Accessinfo.BusinessDate).ToString("yyyy/MM/dd");
                throw new PXReportRequiredException(mailParams, reportID, "Report");
            }
        }
        public PXAction<PX.Objects.PO.POOrder> SegmentPricing;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "分段計價")]
        protected void segmentPricing()
        {
            this.checkSegmentPercent.SetEnabled(false);
        }

        public PXAction<POOrder> cancelSegmentPercent;
        [PXUIField(DisplayName = "Cancel", Enabled = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable CancelSegmentPercent(PXAdapter adapter)
        {
            return this.CheckSegmentPercent(adapter);
        }

        public PXAction<POOrder> checkSegmentPercent;
        [PXUIField(DisplayName = "OK", Enabled = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable CheckSegmentPercent(PXAdapter adapter)
        {

            decimal percentage = (decimal)GetKGContractSegPricingTotalPercentage();

            POLine line = POLines.Current as POLine;
            if (line != null)
            {
                PXCache sender = Base.Transactions.Cache;
                POLineContractExt lineExt = sender.GetExtension<POLineContractExt>(line);
                lineExt.UsrSegPricingFlag = percentage == 100;
                sender.SetStatus(line, PXEntryStatus.Modified);
                sender.Update(line);
                sender.IsDirty = true;
            }

            return adapter.Get();
        }

        public PXAction<PX.Objects.PO.POOrder> ImportSampleTags;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "匯入範本標籤")]
        protected void importSampleTags()
        {
            POOrder order = (POOrder)POOrders.Current;
            KGContractDoc row = (KGContractDoc)KGContractDocs.Current;

            if (row == null || row.ContractCategoryID == null) {
                throw new PXException("契約資訊相關 契約類型尚未選擇");
            }

            PXResultset<KGContractTag> dataSet = PXSelect<KGContractTag,
                       Where<KGContractTag.contractCategoryID,
                           Equal<Required<KGContractTag.contractCategoryID>>>>.Select(Base, row.ContractCategoryID);

            foreach (KGContractTag tag in dataSet)
            {
                KGContractDocTag docTag = new KGContractDocTag();
                docTag.Tagid = tag.TagID;
                docTag.OrderNbr = order.OrderNbr;
                docTag.OrderType = order.OrderType;
                //2019/07/26 ADD
                docTag.Active = tag.Active;
                if (tag.Tagcd.Equals("工程款付款票期說明"))
                {
                    PXResultset<KGPoOrderPayment> poorderpayment = PXSelect<KGPoOrderPayment,
                        Where<KGPoOrderPayment.orderNbr, Equal<Required<POOrder.orderNbr>>>>
                        .Select(Base, order.OrderNbr);
                    string content = string.Empty;
                    foreach (KGPoOrderPayment orderPayment in poorderpayment)
                    {
                        if (orderPayment == null) return;
                        string paymentMethod = string.Empty;
                        if (orderPayment.PaymentMethod == "A")
                        {
                            paymentMethod = "支票";
                        }
                        else if (orderPayment.PaymentMethod == "B")
                        {
                            paymentMethod = "現金票";
                        }
                        else if (orderPayment.PaymentMethod == "C")
                        {
                            paymentMethod = "現金";
                        }
                        content += paymentMethod +
                             orderPayment.PaymentPct + "%" +
                             orderPayment.PaymentPeriod + "天" + "、";
                    }
                    docTag.TagContent = content.TrimEnd('、');//把最後一筆的"、刪掉"
                }
                else if (tag.Tagcd.Equals("分包契約工程地點"))
                {
                    //2019/07/11 改成抓KGContractDoc.SiteAddress
                    //PMProject project = GetPMProject(order);
                    KGContractDoc contractDoc = GetKGContractDoc(order);
                    docTag.TagContent = contractDoc != null ? contractDoc.SiteAddress : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("分包契約名稱"))
                {
                    docTag.TagContent = string.IsNullOrEmpty(order.OrderDesc) ? tag.TagContent : order.OrderDesc;
                }
                else if (tag.Tagcd.Equals("分包契約訂約日期"))
                {
                    KGContractDoc contractDoc = GetKGContractDoc(order);
                    if (contractDoc == null)
                        continue;
                    DateTime date = (DateTime)contractDoc.ContractDocDate;
                    //民國XXX年XX月XX號
                    if(date!=null)
                    {
                        var formatDate = date.ToFullTaiwanDate();
                        docTag.TagContent = formatDate.ToString();
                    }
                    
                }
                else if (tag.Tagcd.Equals("分包契約編號"))
                {
                    Contract contract = GetContract(order.ProjectID);
                    docTag.TagContent = contract.ContractCD.Trim()+ "-" + order.OrderNbr;
                }
                else if (tag.Tagcd.Equals("承攬廠商公司名稱"))
                {
                    Vendor vendor = GetPOVendor(order);
                    docTag.TagContent = vendor != null ? vendor.AcctName : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("承攬廠商統一編號"))
                {
                    Location location = GetPORegistration(order);
                    docTag.TagContent = location != null ? location.TaxRegistrationID : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("業主工程名稱"))
                {
                    if (order.ProjectID != 0)
                    {
                        Contract contract = GetPOContract(order);
                        docTag.TagContent = contract != null ? contract.Description : tag.TagContent;
                    }
                    else
                    {
                        docTag.TagContent = tag.TagContent;
                    }
                }
                else if (tag.Tagcd.Equals("業主工程簡稱"))
                {
                    if (order.ProjectID != 0)
                    {
                        Contract contract = GetPOContract(order);
                        docTag.TagContent = contract != null ? contract.Description : tag.TagContent;
                    }
                    else
                    {
                        docTag.TagContent = tag.TagContent;
                    }
                }
                else if (tag.Tagcd.Equals("保固年限"))
                {
                    KGContractPricingRule kGContractPricingRule = GetKGContractPricingRule(order);
                    docTag.TagContent = kGContractPricingRule != null ? kGContractPricingRule.WarrantyYear.ToString() : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("保固金比率"))
                {
                    KGContractPricingRule contractPricingRule = GetKGContractPricingRule(order);
                    if (contractPricingRule == null) continue;
                    string pct = string.Format("{0:###0.#}", contractPricingRule.WarrantyRatioUntaxed??0);
                    docTag.TagContent = pct; //+ "%";(2019/07/26發現文件裡後面有加%了
                }
                else if (tag.Tagcd.Equals("分包契約訂約日期"))
                {
                    KGContractDoc kGContractDoc = GetKGContractDoc(order);
                    docTag.TagContent = kGContractDoc != null ? string.Format("{0:yyyy/MM/dd}", kGContractDoc.ContractDocDate) : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約承攬廠商公司名稱"))
                {
                    Vendor vendor = GetPOVendor(order);
                    docTag.TagContent = vendor != null ? vendor.AcctName : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約承攬廠商負責人"))
                {
                    CSAnswers cSAnswers = GetCSAnswers(order);
                    docTag.TagContent = cSAnswers != null ? cSAnswers.Value : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約承攬廠商統一編號"))
                {
                    Location location = GetPORegistration(order);
                    docTag.TagContent = location != null ? location.TaxRegistrationID : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約承攬廠商地址"))
                {
                    Address address = GetAddress(order);
                    docTag.TagContent = address != null ? address.AddressLine1 : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約承攬廠商負責人電話"))
                {
                    Contact contact = GetContact(order);
                    docTag.TagContent = contact != null ? contact.Phone1 : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約保証廠商公司名稱"))
                {
                    KGContractRelatedVendor contractRelatedVendor = GetKGContractRelatedVendor(order);
                    if (contractRelatedVendor != null)
                    {
                        Vendor vendor = GetKGContractrelatedVendorVendor(contractRelatedVendor);
                        docTag.TagContent = vendor != null ? vendor.AcctName : tag.TagContent;
                    }

                }
                else if (tag.Tagcd.Equals("合約保証廠商地址"))
                {
                    KGContractRelatedVendor contractRelatedVendor = GetKGContractRelatedVendor(order);
                    if (contractRelatedVendor != null)
                    {
                        Address address = GetKGContractRelatedVendorAddress(contractRelatedVendor);
                        docTag.TagContent = address != null ? address.AddressLine1 : tag.TagContent;
                    }

                }
                else if (tag.Tagcd.Equals("合約保証廠商負責人"))
                {
                    KGContractRelatedVendor contractRelatedVendor = GetKGContractRelatedVendor(order);
                    if (contractRelatedVendor == null) continue;
                    CSAnswers answers = GetKGContractRelatedVendorCSAnswers(contractRelatedVendor);
                    docTag.TagContent = answers != null ? answers.Value : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約保証廠商負責人電話"))
                {
                    KGContractRelatedVendor contractRelatedVendor = GetKGContractRelatedVendor(order);
                    if (contractRelatedVendor == null) continue;
                    Contact contact = GetKGContractRelatedVendorContact(contractRelatedVendor);
                    docTag.TagContent = contact != null ? contact.Phone1 : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約保証廠商統一編號"))
                {
                    KGContractRelatedVendor contractRelatedVendor = GetKGContractRelatedVendor(order);
                    if(contractRelatedVendor == null) continue;
                    Location location = GetKGContractRelatedVendorLocation(contractRelatedVendor);
                    docTag.TagContent = location != null ? location.TaxRegistrationID : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("合約營造廠公司名稱"))
                {
                    Branch branch = GetBranch(order.BranchID);
                    if (branch == null) continue;
                    docTag.TagContent = branch.AcctName;
                }
                else if (tag.Tagcd.Equals("原分包契約工程總價(未稅)(國字)"))
                {
                    //edit 2020-04-08 by alton mantis: 11571
                    //string Amount = GetChineseNum("單價", string.Format("{0:###0.#}", order.CuryLineTotal));
                    string Amount = GetChineseNum("單價", string.Format("{0:###0.#}", row.TotalAmt??0));
                    docTag.TagContent = order != null ? Amount : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("原分包契約工程總價(營業稅)(國字)"))
                {
                    //edit 2020-04-08 by alton mantis: 11571
                    //string Amount = GetChineseNum("單價", string.Format("{0:###0.#}", order.CuryTaxTotal));
                    string Amount = GetChineseNum("單價", string.Format("{0:###0.#}", row.TotalTaxAmt??0));
                    docTag.TagContent = order != null ? Amount : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("原分包契約工程總價(含稅)(國字)"))
                {
                    string Amount = GetChineseNum("單價", string.Format("{0:###0.#}", order.CuryOrderTotal??0));
                    docTag.TagContent = order != null ? Amount : tag.TagContent;
                }
                else if (tag.Tagcd.Equals("保留款比率"))
                {
                    KGContractPricingRule kGContractPricingRule = GetKGContractPricingRule(order);
                    if (kGContractPricingRule == null) continue;
                    string pct = string.Format("{0:###0.#}", kGContractPricingRule.DefRetainagePct??0);
                    docTag.TagContent = pct != null ? pct + "%" : tag.TagContent;
                }
                else
                {
                    docTag.TagContent = tag.TagContent;
                }

                if (!IsExistKGContractDocTag(docTag))
                {
                    KGContractDocTags.Cache.Insert(docTag);
                }
                //2019/06/18若doctag已有資料,要先刪除doctag之後再加一筆
                else if(IsExistKGContractDocTag(docTag))
                {   
                    KGContractDocTags.Cache.Delete(docTag);
                    KGContractDocTags.Cache.Insert(docTag);
                }
                
            }
        }


        //Lines  RQRequestLine
        public PXAction<PX.Objects.PO.POOrder> AddCostItem;
        [PXUIField(DisplayName = "Add Cost Item", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable addCostItem(PXAdapter adapter)
        {
            try
            {
                WebDialogResult result = pmCostBudget.AskExt(true);
                if (result == WebDialogResult.OK)
                {
                    return AddItems(adapter);
                }
                else if (result == WebDialogResult.Cancel)
                {
                    pmCostBudget.Cache.Clear();
                    pmCostBudget.Cache.ClearQueryCache();
                }
                else
                {
                    pmCostBudget.Cache.Clear();
                    pmCostBudget.Cache.ClearQueryCache();
                }
            }
            catch (Exception e)
            {
                pmCostBudget.Cache.Clear();
                pmCostBudget.Cache.ClearQueryCache();
                throw e;
            }
            return adapter.Get();
        }

        public PXAction<PX.Objects.PO.POOrder> addItems;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select,
                      MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddItems(PXAdapter adapter)
        {
            foreach (PMCostBudget pmCost in pmCostBudget.Cache.Cached)
            {
                if (!(bool)pmCost.Selected)
                {
                    continue;
                }

                if (checkLineExist(pmCost))
                {
                    continue;
                }

                PXCache sender = Base.Transactions.Cache;

                POLine line = (POLine)Base.Transactions.Cache.CreateInstance();

                line.ProjectID = pmCost.ProjectID;
                line.InventoryID = pmCost.InventoryID;
                line.TaskID = pmCost.ProjectTaskID;
                line.CostCodeID = pmCost.CostCodeID;


                //Qty
                line.UOM = pmCost.UOM;
                line.CuryUnitCost = pmCost.CuryUnitRate;

                POLines.Cache.Update(line);
                //contactCache.SetValue<RQRequestLineExt.usrContractID>(Base.Lines.Current, pmCost.ProjectID);
                //contactCache.SetValue<RQRequestLineExt.usrProjectTaskID>(Base.Lines.Current, pmCost.ProjectTaskID);
                //contactCache.SetValue<RQRequestLineExt.usrCostCodeID>(Base.Lines.Current, pmCost.CostCodeID);
                //contactCache.SetValue<RQRequestLineExt.usrAccountGroupID>(Base.Lines.Current, pmCost.AccountGroupID);
                //RQRequestLineExt rqRequestLineExt = contactCache.GetExtension<RQRequestLineExt>(line);
            }
            pmCostBudget.Cache.Clear();
            pmCostBudget.Cache.ClearQueryCache();

            return adapter.Get();
        }

        #region WebService
        public PXAction<RQRequestLine> webServIntegration;
        [PXUIField(DisplayName = "AgentFlow", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable WebServIntegration(PXAdapter adapter)
        {
            String message = CreatePoOrderRequestFile();
            message = null;
            if (message != null)
            {
                throw new PXException(message);
            }
            Base.Document.View.RequestRefresh();
            return adapter.Get();
        }
        #endregion

        #endregion

        #region Action
        public PXAction<APInvoice> ViewOriginalDocument;
        [PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        protected virtual IEnumerable viewOriginalDocument(PXAdapter adapter)
        {
            //參考資料
            //https://asiablog.acumatica.com/2017/08/redirecting-to-external-page-from-button.html

            POOrder poOrder = POOrders.Current;
            POOrderExt poorderext = PXCache<POOrder>.GetExtension<POOrderExt>(poOrder);
            throw new Exception("Redirect7:" + poorderext.UsrReturnUrl);
            return adapter.Get();
        }

        //public PXAction<POOrder> updateCommitments;
        //[PXUIField(DisplayName = "Update Commit",
        //           MapEnableRights = PXCacheRights.Select,
        //           MapViewRights = PXCacheRights.Select)]
        //[PXButton]
        //public virtual IEnumerable UpdateCommitments(PXAdapter adapter)
        //{
        //    decimal committedQty = 0, committedAmt = 0;

        //    PMCommitment commitment = PXSelectReadonly<PMCommitment,
        //                                               Where<PMCommitment.commitmentID, Equal<Required<POLine.commitmentID>>>>
        //                                               .Select(Base, Base.Transactions.Current.CommitmentID);

        //    PMBudget budget = PXSelect<PMBudget,
        //                               Where<PMBudget.projectID, Equal<Required<PMBudget.projectID>>,
        //                                     And<PMBudget.projectTaskID, Equal<Required<PMBudget.projectTaskID>>,
        //                                         And<PMBudget.costCodeID, Equal<Required<PMBudget.costCodeID>>,
        //                                             And<PMBudget.inventoryID, Equal<Required<PMBudget.inventoryID>>,
        //                                                 And<PMBudget.accountGroupID, Equal<Required<PMBudget.accountGroupID>>>>>>>>
        //                               .Select(Base, commitment.ProjectID, commitment.TaskID, commitment.CostCodeID,
        //                                       commitment.InventoryID, commitment.AccountGroupID);
        //    if (budget != null)
        //    {
        //        committedQty = (budget.CommittedOrigQty + commitment.Qty).Value;
        //        committedAmt = (budget.CuryCommittedOrigAmount + commitment.Amount).Value;

        //        budget.CommittedQty = budget.CommittedOpenQty = budget.CommittedOrigQty =
        //        /*(committedQty > budget.Qty) ? budget.Qty :*/ committedQty;

        //        if (budget.Qty == budget.CommittedOrigQty)
        //        {
        //            commitment = PXSelectGroupBy<PMCommitment,
        //                                         Where<PMCommitment.projectID, Equal<Required<PMCommitment.projectID>>,
        //                                               And<PMCommitment.projectTaskID, Equal<Required<PMCommitment.projectTaskID>>,
        //                                                   And<PMCommitment.costCodeID, Equal<Required<PMCommitment.costCodeID>>,
        //                                                       And<PMCommitment.inventoryID, Equal<Required<PMCommitment.inventoryID>>,
        //                                                           And<PMCommitment.accountGroupID, Equal<Required<PMCommitment.accountGroupID>>>>>>>,
        //                                         Aggregate<Sum<PMCommitment.amount>>>
        //                                         .Select(Base, commitment.ProjectID, commitment.TaskID, commitment.CostCodeID,
        //                                                 commitment.InventoryID, commitment.AccountGroupID);

        //            committedAmt = commitment.Amount.Value;
        //        }

        //        budget.CuryCommittedAmount = budget.CommittedAmount =
        //        budget.CommittedOrigAmount = budget.CuryCommittedOpenAmount =
        //        budget.CommittedOpenAmount = budget.CuryCommittedOrigAmount =
        //        /*(committedAmt > budget.CuryAmount) ? budget.CuryAmount :*/ committedAmt;

        //        BudgetView.Cache.Update(budget);
        //    }

        //    Base.Save.Press();

        //    return adapter.Get();
        //}

        //public PXAction<POOrder> updateCommitment2;
        //[PXUIField(DisplayName = "Update Commit_Old",
        //           MapEnableRights = PXCacheRights.Select,
        //           MapViewRights = PXCacheRights.Select)]
        //[PXButton]
        //public virtual IEnumerable UpdateCommitment2(PXAdapter adapter)
        //{
        //    decimal committedQty = 0, committedAmt = 0;

        //    PMCommitment commitment = PXSelectReadonly<PMCommitment,
        //                                               Where<PMCommitment.commitmentID, Equal<Required<POLine.commitmentID>>>>
        //                                               .Select(Base, Base.Transactions.Current.CommitmentID);

        //    PMBudget budget = PXSelect<PMBudget,
        //                               Where<PMBudget.projectID, Equal<Required<PMBudget.projectID>>,
        //                                     And<PMBudget.projectTaskID, Equal<Required<PMBudget.projectTaskID>>,
        //                                         And<PMBudget.costCodeID, Equal<Required<PMBudget.costCodeID>>,
        //                                             And<PMBudget.inventoryID, Equal<Required<PMBudget.inventoryID>>,
        //                                                 And<PMBudget.accountGroupID, Equal<Required<PMBudget.accountGroupID>>>>>>>>
        //                               .Select(Base, commitment.ProjectID, commitment.TaskID, commitment.CostCodeID,
        //                                       commitment.InventoryID, commitment.AccountGroupID);
        //    if (budget != null)
        //    {
        //        committedQty = (budget.CommittedOrigQty + commitment.Qty).Value;
        //        committedAmt = (budget.CuryCommittedOrigAmount + commitment.Amount).Value;

        //        budget.CommittedQty = budget.CommittedOpenQty = budget.CommittedOrigQty =
        //        (committedQty > budget.Qty) ? budget.Qty : committedQty;

        //        if (budget.Qty == budget.CommittedOrigQty)
        //        {
        //            commitment = PXSelectGroupBy<PMCommitment,
        //                                         Where<PMCommitment.projectID, Equal<Required<PMCommitment.projectID>>,
        //                                               And<PMCommitment.projectTaskID, Equal<Required<PMCommitment.projectTaskID>>,
        //                                                   And<PMCommitment.costCodeID, Equal<Required<PMCommitment.costCodeID>>,
        //                                                       And<PMCommitment.inventoryID, Equal<Required<PMCommitment.inventoryID>>,
        //                                                           And<PMCommitment.accountGroupID, Equal<Required<PMCommitment.accountGroupID>>>>>>>,
        //                                         Aggregate<Sum<PMCommitment.amount>>>
        //                                         .Select(Base, commitment.ProjectID, commitment.TaskID, commitment.CostCodeID,
        //                                                 commitment.InventoryID, commitment.AccountGroupID);

        //            committedAmt = commitment.Amount.Value;
        //        }

        //        budget.CuryCommittedAmount = budget.CommittedAmount =
        //        budget.CommittedOrigAmount = budget.CuryCommittedOpenAmount =
        //        budget.CommittedOpenAmount = budget.CuryCommittedOrigAmount =
        //        (committedAmt > budget.CuryAmount) ? budget.CuryAmount : committedAmt;

        //        BudgetView.Cache.Update(budget);
        //    }

        //    Base.Save.Press();

        //    return adapter.Get();
        //}
        #endregion

        #region Event Handlers

        #region POOrder
        protected virtual void POOrder_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
        {
            // 預先建立關聯的 契約相關資訊
            if (KGContractDocs.Current == null &&
                PXSiteMap.CurrentScreenID == "PO301000")
            {
                // 不會啟用 Save
                PXCache sender = KGContractDocs.Cache;
                KGContractDoc doc = sender.CreateInstance() as KGContractDoc;
                sender.Update(doc);
                sender.SetStatus(doc, PXEntryStatus.Notchanged);
                sender.IsDirty = false;

                //// Save button 會啟用
                //KGContractDoc doc = new KGContractDoc();
                //KGContractDocs.Cache.Update(doc);
            }
        }

        //20200130
        [ProjectDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>), PM.Messages.InactiveContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [ProjectBaseExt]
        protected void POOrder_ProjectID_CacheAttached(PXCache sender) { }
        
        protected virtual void POOrder_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            POOrder order = e.Row as POOrder;

            if (order == null)
            {
                return;
            }
            //零星不跑Enable
            if (POOrderType.RegularSubcontract.Equals(order.OrderType))
            {

            }
            else {
                ControlsSetEnabled(order);
            }
            if (message != null)
            {
                //order.Hold = true;
                //order.Status = POOrderStatus.Hold;
                //POOrders.Cache.SetValueExt<POOrder.hold>(POOrders.Current, true);
                cache.RaiseExceptionHandling<POOrder.hold>(order, order.Hold,
                          new PXSetPropertyException(message, PXErrorLevel.Error));
                //throw new PXException(message);

            }
            //2020-04-08 add by alton mantis:11571
            setKGContractDocTotal();
        }

        protected virtual void POOrder_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            POOrder poOrder = POOrders.Current;
            KGContractPricingRule rule = (KGContractPricingRule)KGContractPricingRules.Current;
            // Add " && poOrder != null" to avoid exception error when delete subcontract.
            /* 2022/03/22 Cancel overall system's field
            if (rule != null && poOrder != null)
            {
                poOrder.RetainageApply = rule.RetainageApply;
                poOrder.DefRetainagePct = rule.DefRetainagePct;
            }
            */

            if (poOrder != null && poOrder.ProjectID != null && POOrderType.RegularOrder.Equals(poOrder.OrderType))
            {
                //Contract contract = GetContract(poOrder.ProjectID);
                //2021/04/20 Mantis: 0012020 無專案也要檢查
                /*
                 * if (contract != null && contract.NonProject != true)
                 * {
                */
     
                    if (KGPoOrderPayments.Select().Count == 0)
                    {
                        throw new PXException("必須要有付款方式");

                    }
                // }
            }


        }

        protected virtual void POOrder_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            POOrder newOrder = (POOrder)e.Row;
            POOrder oldOrder = (POOrder)e.OldRow;
            POOrder row = POOrders.Current;

            // Vendor Id 改變時 計價參數的應用保留款要取預設值
            if (!sender.ObjectsEqual<POOrder.vendorID>(newOrder, oldOrder))
            {
                Vendor vendor = PXSelect<Vendor,
                    Where<Vendor.bAccountID, Equal<Required<POOrder.vendorID>>>>
                    .Select(Base, row.VendorID);
                //Vendor vendor = PXSelectorAttribute.Select<POOrder.vendorID>(sender, row) as Vendor;
                if (vendor != null)
                {
                    KGContractPricingRule rule = (KGContractPricingRule)KGContractPricingRules.Current;
                    if (rule == null)
                    {
                        rule = new KGContractPricingRule();
                    }
                    rule.RetainageApply = vendor.RetainageApply;
                    rule.DefRetainagePct = vendor.RetainagePct;
                    KGContractPricingRules.Update(rule);
                }
            }

            // Per Louis's request
            // When purchase order is created from requisition, then set the order is holding.
            if (row != null && row.Hold == false && row.RQReqNbr != "" && row.Status == POOrderStatus.Open &&
                row.LastModifiedByScreenID.StartsWith("RQ"))
            {
                RQSetup rqSetup = PXSelect<RQSetup>.Select(Base);

                if (rqSetup.RequisitionApproval == true)
                {
                    row.Hold = true;
                    row.Status = POOrderStatus.Hold;
                }
            }
        }

        Boolean isFirst = true;
        protected virtual void POOrder_Status_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            POOrder master = (POOrder)e.Row; ;
            //PXSiteMap.CurrentScreenID == "PO301000"
            String screenID = PXSiteMap.CurrentScreenID;
            String status = (String)e.OldValue;
            //POOrderStatus.Balanced 中文為待簽核
            if (PXSiteMap.CurrentScreenID == "PO301000" && (master.Hold == false || master.Hold == null)
                && POOrderStatus.Balanced.Equals(master.Status) && POOrderStatus.Hold.Equals(status))
            {
                CreatePoOrderRequestFile();
            }
            //將請購單弄過來的改回擱置20190819
            if (master != null && master.Hold == false && master.RQReqNbr != "" &&
               screenID.StartsWith("RQ") && isFirst)
            {
                isFirst = false;
                sender.SetValueExt<POOrder.hold>(master, true);
                EPApproval epApproval = Base.Approval.Select();
                if (epApproval != null)
                {
                    Base.Approval.Delete(epApproval);
                }
                //master.Hold = true;
                //master.Status = POOrderStatus.Hold;
            }
        }

        protected virtual void POOrder_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            POOrder master = (POOrder)e.Row; ;
            KGContractDoc contractDoc = KGContractDocs.Current;
            //check IsNull add by louis 20210310
            if (contractDoc==null) {
                contractDoc = new KGContractDoc();
            }
            if (master.ProjectID == null) return;
            PMProject project = PXSelect<PMProject,
                Where<PMProject.contractID, Equal<Required<KGContractDoc.contractDocID>>>>
                .Select(Base, master.ProjectID);
            //check IsNull add by louis 20210310
            if (project != null)
            {
                contractDoc.SiteAddress = project.SiteAddress;
            }
            
            
        }
        
        protected virtual void _(Events.FieldUpdating<POOrder.hold> e)
        {
            if (Convert.ToBoolean(e.NewValue) == false)
            {
                ResetStandardRetainageApply(e.Cache.Graph, false);
            }
        }

        protected virtual void POOrder_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(sender, e);

            POOrder poOrder = e.Row as POOrder;

            //2020/03/02 Add 擱置勾起來, 如果電子簽核欄位有值, 請將URL & UKGFlowUID清空
            if (poOrder.Hold == true)
            {
                POOrderExt poorderext = PXCache<POOrder>.GetExtension<POOrderExt>(poOrder);
                if (poorderext.UsrKGFlowUID != null)
                {
                    poorderext.UsrKGFlowUID = null;
                }
                if(poorderext.UsrReturnUrl !=null)
                {
                    poorderext.UsrReturnUrl = null;
                }
            }
        }
        #endregion

        #region POLine
        protected virtual void POLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            POOrder poOrder = POOrders.Current;
            POLine row = (POLine)e.Row;
            PXResultset<KGContractSegPricing> deleteSegPrices =
                PXSelect<KGContractSegPricing,
                          Where<KGContractSegPricing.orderNbr, Equal<Required<KGContractSegPricing.orderNbr>>,
                          And<KGContractSegPricing.orderType, Equal<Required<KGContractSegPricing.orderType>>,
                          And<KGContractSegPricing.lineNbr, Equal<Required<KGContractSegPricing.lineNbr>>>>>>
                            .Select(Base, poOrder.OrderNbr, poOrder.OrderType, row.LineNbr);
            foreach (KGContractSegPricing item in deleteSegPrices)
            {
                KGContractSegPricings.Delete(item);
            }
        }
        protected virtual void POLine_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            POLine row = POLines.Current;
            ValidPOLineSegPricing(cache);     
        }
        #endregion

        #region POOrderAPDoc
        protected virtual void POOrderAPDoc_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            POOrderAPDoc row = (POOrderAPDoc)e.Row;
            PXResultset<APRegister> apregister = PXSelect<APRegister,
                    Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                    And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                    .Select(Base, row.DocType, row.RefNbr);
            foreach (APRegister register in apregister)
            {

                APRegisterExt aPRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
                if (aPRegisterExt.UsrValuationPhase != null)
                {
                    sender.SetValue<POOrderAPDocExt.usrValuationPhase>(Base.APDocs.Current, aPRegisterExt.UsrValuationPhase);
                }

            }


        }
        #endregion

        #region KGContractRelatedVendor

        protected virtual void KGContractRelatedVendor_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            var row = (KGContractRelatedVendor)e.Row;
            if (row == null) return;

            var item = (PXResult<BAccount, Location>)
                    PXSelectJoin<BAccount,
                        InnerJoin<Location, On<Location.bAccountID, Equal<BAccount.bAccountID>>>,
                    Where<BAccount.bAccountID, Equal<Required<KGContractRelatedVendor.relatedVendorID>>,
                        And<Location.isActive, Equal<True>>>>
                    .Select(Base, row.RelatedVendorID);

            if (item != null)
            {
                if (((BAccount)item).AcctName != null)
                {
                    row.RelatedVendorName = ((BAccount)item).AcctName;
                }
                if (((Location)item).TaxRegistrationID != null)
                {
                    row.RelatedVendorTaxRegistration = ((Location)item).TaxRegistrationID;
                }
            }
        }


        protected virtual void KGContractRelatedVendor_RelatedVendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractRelatedVendor row = (KGContractRelatedVendor)e.Row;

            BAccount bAccount = PXSelectorAttribute.Select<KGContractRelatedVendor.relatedVendorID>(sender, row) as BAccount;
            if (bAccount == null) return;
            int? locationID = bAccount.DefLocationID;

            PXResultset<LocationExtAddress> set = PXSelect<LocationExtAddress, Where<LocationExtAddress.locationID,
                                                    Equal<Required<LocationExtAddress.locationID>>>>.Select(Base, locationID);
            LocationExtAddress loction = set;
            if (loction != null)
            {
                row.RelatedVendorTaxRegistration = loction.TaxRegistrationID;
            }
            row.RelatedVendorName = bAccount.AcctName;
        }

        #endregion

        #region KGContractSegPricing
        protected virtual void KGContractSegPricing_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            // Per Louis's request to add the control by PO order status.
            KGContractSegPricings.AllowDelete = KGContractSegPricings.AllowInsert = KGContractSegPricings.AllowUpdate = 
                                                                                    (Base.CurrentDocument.Current.Status == POOrderStatus.Hold);

            KGContractSegPricing row = (KGContractSegPricing)e.Row;
            if (row == null) return;

            this.checkSegmentPercent.SetEnabled(ValidKGContractSegPricingTotalPercentage());         
        }
        protected virtual void KGContractSegPricing_RowUpdating(PXCache cache, PXRowUpdatingEventArgs e)
        {
            KGContractSegPricing row = (KGContractSegPricing)e.Row;
            if (row == null) return;

            this.checkSegmentPercent.SetEnabled(ValidKGContractSegPricingTotalPercentage());
        }

        /*protected virtual void KGContractSegPricing_SortOrder_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            KGContractSegPricing row = (KGContractSegPricing)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            int sortorder = 0;
            if (e.NewValue != null)
            {
                if (typeof(int).IsInstanceOfType(e.NewValue))
                {
                    sortorder = (int)e.NewValue;
                }
            }
                                                         // Change object 指標轉型 to 轉型
            if (isDuplicateKGContractSegPricingSortOrder(Convert.ToInt32(e.NewValue.ToString()))) //(int)e.NewValue))
            {
                sender.RaiseExceptionHandling<KGContractSegPricing.sortOrder>(row, row.SortOrder,
                          new PXSetPropertyException("Sort Order duplicate.", PXErrorLevel.Error));
                e.Cancel = true;
            }
        }
        */
        protected virtual void KGContractSegPricing_SortOrder_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractSegPricing row = (KGContractSegPricing)e.Row;
            if (row == null) return;
            int sortorder = 0;
            if(row.SortOrder!=null)
            {
                if (typeof(int).IsInstanceOfType(row.SortOrder))
                {
                    sortorder = (int)row.SortOrder;
                }
                if (isDuplicateKGContractSegPricingSortOrder(Convert.ToInt32(row.SortOrder.ToString()))) //(int)e.NewValue))
                {
                    sender.RaiseExceptionHandling<KGContractSegPricing.sortOrder>(row, row.SortOrder,
                              new PXSetPropertyException("Sort Order duplicate.", PXErrorLevel.Error));
                    //e.Cancel = true;
                }
            }
        }
        /*protected virtual void KGContractSegPricing_SegmentName_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            KGContractSegPricing row = (KGContractSegPricing)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;

            if (isDuplicateKGContractSegPricingSegmentName((string)e.NewValue))
            {
                sender.RaiseExceptionHandling<KGContractSegPricing.segmentName>(row, row.SegmentName,
                      new PXSetPropertyException("階段計價名稱重覆.", PXErrorLevel.Error));
                e.Cancel = true;
            }
        }*/

        /*protected virtual void KGContractSegPricing_SegmentName_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractSegPricing row = (KGContractSegPricing)e.Row;
            if (row == null) return;
            if (isDuplicateKGContractSegPricingSegmentName(row.SegmentName))
            {
                sender.RaiseExceptionHandling<KGContractSegPricing.segmentName>(row, row.SegmentName,
                      new PXSetPropertyException("階段計價名稱重覆.", PXErrorLevel.Error));
            }
        }*/

        protected virtual void KGContractSegPricing_SegmentPercent_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractSegPricing row = (KGContractSegPricing)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;

            Decimal percent = (Decimal)e.NewValue;
            if (percent <= 0 || percent > 100)
            {
                sender.RaiseExceptionHandling<KGContractSegPricing.segmentPercent>(row, percent,
                       new PXSetPropertyException("Segment Percent incorrect.", PXErrorLevel.Error));
                //throw new PXSetPropertyException<KGContractSegPricing.segmentPercent>("Segment Percent incorrect.);
            }
        }

        protected virtual void KGContractSegPricing_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            KGContractSegPricing segPricing = (KGContractSegPricing)e.Row;
            if (segPricing == null) return;

            POOrder poOrder = POOrders.Current;
            segPricing.SegPricingID = GetNextSegPricingID();
            segPricing.LineNbr = POLines.Current.LineNbr;
            this.checkSegmentPercent.SetEnabled(false);
        }

        protected virtual void KGContractSegPricing_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            this.checkSegmentPercent.SetEnabled(false);
        }

        #endregion

        #region KGContractDoc
        protected virtual void KGContractDoc_SiteAddress_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            KGContractDoc row = (KGContractDoc)e.Row;
            POOrder order = POOrders.Current;
            if (row == null) return;
            if(order.ProjectID !=null)
            {
                PMProject project =
                    PXSelect<PMProject,
                    Where<PMProject.contractID, Equal<Required<POOrder.projectID>>>>
                    .Select(Base, order.ProjectID);
                row.SiteAddress = project.SiteAddress; 
            }
        }
        protected virtual void KGContractDoc_ContractCategoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractDoc row = (KGContractDoc)e.Row;
            if (row == null) return;

            if (row.ContractCategoryID != null)
            {
                KGContractCategory category = PXSelectorAttribute.Select<KGContractCategory.contractCategoryID>(KGContractDocs.Cache, row) as KGContractCategory;
                if (category != null && category.ContractType.CompareTo("S") == 0)
                {
                    row.ContractDesc = category.ContractDesc;
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<KGContractDoc.contractDesc>(sender, row, false);
                    row.ContractDesc = "";
                }
            }
        }
        /*
        protected virtual void KGContractDoc_RemainingDays_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractDoc row = (KGContractDoc)e.Row;
            if (row == null) return;
            if (row.RemainingDays < 0)
            {
                sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<KGContractDoc.remainingDays>(e.Row),
                        new PXSetPropertyException("剩餘工期 需大於等於0", PXErrorLevel.Error));
            }
        }
        */

        protected virtual void KGContractDoc_EvaluationScore_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            KGContractDoc row = (KGContractDoc)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;

            int evaluationScore = (int)e.NewValue;
            if (evaluationScore < 0 || evaluationScore > 100)
            {
                sender.RaiseExceptionHandling<KGContractDoc.evaluationScore>(row, evaluationScore,
                        new PXSetPropertyException("評鑑時機計價達 需大於等於0(0~100)", PXErrorLevel.Error));
                e.Cancel = true;
            }
        }

        protected virtual void KGContractDoc_SecEvaluationScore_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            KGContractDoc row = (KGContractDoc)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;

            int secEvaluationScore = (int)e.NewValue;
            if (secEvaluationScore < 0 || secEvaluationScore > 100)
            {
                sender.RaiseExceptionHandling<KGContractDoc.secEvaluationScore>(row, secEvaluationScore,
                        new PXSetPropertyException("第二次評鑑時機計價達 需大於等於0(0~100)", PXErrorLevel.Error));
                e.Cancel = true;
            }
        }

        protected virtual void KGContractDoc_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            KGContractDoc doc = (KGContractDoc)e.NewRow;
            KGContractDoc originalDoc = (KGContractDoc)e.Row;
            if (!sender.ObjectsEqual<KGContractDoc.contractCategoryID>(doc, originalDoc))
            {
                foreach (KGContractDocTag item in KGContractDocTags.Select())
                {
                    KGContractDocTags.Delete(item);
                    KGContractDocTags.Cache.Delete(item);
                }
            }
        }
        
        protected virtual void KGContractDoc_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGContractDoc row = (KGContractDoc)e.Row;
            if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete) return;

            POOrder poOrder = POOrders.Current;
            POLine poLine = POLines.Current;

            int? evaluationScore = row.EvaluationScore;
            if (evaluationScore == null || evaluationScore < 0 || evaluationScore > 100)
            {
                sender.RaiseExceptionHandling<KGContractDoc.evaluationScore>(row, evaluationScore,
                        new PXSetPropertyException("評鑑時機計價達 需大於等於0(0~100)", PXErrorLevel.Error));
                e.Cancel = true;
            }
            int? secEvaluationScore = row.SecEvaluationScore;
            if (secEvaluationScore == null || secEvaluationScore < 0 || secEvaluationScore > 100)
            {
                sender.RaiseExceptionHandling<KGContractDoc.secEvaluationScore>(row, secEvaluationScore,
                        new PXSetPropertyException("第二次評鑑時機計價達 需大於等於0(0~100)", PXErrorLevel.Error));
                e.Cancel = true;
            }
            /*
            int? remainingDays = row.RemainingDays;
            if (remainingDays == null || remainingDays < 0)
            {
                sender.RaiseExceptionHandling<KGContractDoc.remainingDays>(row, remainingDays,
                        new PXSetPropertyException("剩餘工期 需大於等於0", PXErrorLevel.Error));
                e.Cancel = true;
            }

            int? expectDays = row.ExpectDays;
            if (expectDays == null || expectDays < 0)
            {
                sender.RaiseExceptionHandling<KGContractDoc.expectDays>(row, expectDays,
                        new PXSetPropertyException("約定工期 需大於等於0", PXErrorLevel.Error));
                e.Cancel = true;
            }
            */
            //row.OrderType = poOrder.OrderType;
            //row.OrderNbr = poOrder.OrderNbr;

            // 檢查 ContractDocCD 不行重覆
            /*PXGraph graph = new PXGraph();
            PXResultset<KGContractDoc> docSet =
                        PXSelect<KGContractDoc,
                    Where<KGContractDoc.orderNbr,
                        Equal<Required<KGContractDoc.contractDocCD>>,
                        And<KGContractDoc.orderNbr, NotEqual<Required<KGContractDoc.orderNbr>>,
                        And<KGContractDoc.orderType, NotEqual<Required<KGContractDoc.orderType>>>>>>.Select(graph, row.ContractDocCD, row.OrderNbr, row.OrderType);

            if (docSet.Count > 0)
            {
                sender.RaiseExceptionHandling<KGContractDoc.contractDocCD>(row, row.ContractDocCD,
                       new PXSetPropertyException("分包契約編號已存在", PXErrorLevel.Error));
                e.Cancel = true;
            }
            */
            //KGContractDocs.Cache.Update(row);
        }
        #endregion

        #region KGContractPricingRule

        protected virtual void KGContractPricingRule_EstimatedAmtPeriod_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal estimatedAmtPeriod = (decimal)e.NewValue;
            if (estimatedAmtPeriod < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.estimatedAmtPeriod>(row, estimatedAmtPeriod,
                       new PXSetPropertyException("依估驗金額每期扣回比率 需大於等於0", PXErrorLevel.Error));
            }
        }

        protected virtual void KGContractPricingRule_PrepaymentRatioUntaxed_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal prepaymentRatioUntaxed = (decimal)e.NewValue;
            if (prepaymentRatioUntaxed < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.prepaymentRatioUntaxed>(row, prepaymentRatioUntaxed,
                       new PXSetPropertyException("預付款比率(未稅) 需大於等於0", PXErrorLevel.Error));
            }
            if(prepaymentRatioUntaxed >100)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.prepaymentRatioUntaxed>(row, prepaymentRatioUntaxed,
                       new PXSetPropertyException("預付款比率(未稅) 需小於等於100", PXErrorLevel.Error));
            }
        }

        //2019/10/07改為唯讀
        /*protected virtual void KGContractPricingRule_PrepaymentsAmtUntaxed_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal prepaymentsAmtUntaxed = (decimal)e.NewValue;
            if (prepaymentsAmtUntaxed < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.prepaymentsAmtUntaxed>(row, prepaymentsAmtUntaxed,
                       new PXSetPropertyException("預付款金額(未稅) 需大於等於0", PXErrorLevel.Error));
            }
        }*/

        protected virtual void KGContractPricingRule_DeductionRatioPeriod_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal deductionRatioPeriod = (decimal)e.NewValue;
            if (deductionRatioPeriod < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.deductionRatioPeriod>(row, deductionRatioPeriod,
                       new PXSetPropertyException("每期扣回比率 需大於等於0", PXErrorLevel.Error));
            }
            if(deductionRatioPeriod >100)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.deductionRatioPeriod>(row, deductionRatioPeriod,
                       new PXSetPropertyException("每期扣回比率 需小於等於100", PXErrorLevel.Error));
            }
        }

        protected virtual void KGContractPricingRule_WarrantyRatioUntaxed_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal warrantyRatioUntaxed = (decimal)e.NewValue;
            if (warrantyRatioUntaxed < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.warrantyRatioUntaxed>(row, warrantyRatioUntaxed,
                       new PXSetPropertyException("保固款計算方式-依比率(未稅) 需大於等於0", PXErrorLevel.Error));
            }
            if(warrantyRatioUntaxed > 100)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.warrantyRatioUntaxed>(row, warrantyRatioUntaxed,
                       new PXSetPropertyException("保固款計算方式-依比率(未稅) 需小於等於100", PXErrorLevel.Error));
            }
        }
       
        protected virtual void KGContractPricingRule_WarrantyYear_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal warrantyYear = (decimal)e.NewValue;
            if (warrantyYear < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.warrantyYear>(row, warrantyYear,
                       new PXSetPropertyException("保固時間(年) 需大於等於0", PXErrorLevel.Error));
            }
        }

        protected virtual void KGContractPricingRule_WarrantyGuaranteeTicketRatio_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal WarrantyGuaranteeTicketRatio = (decimal)e.NewValue;
            if (WarrantyGuaranteeTicketRatio < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeTicketRatio>(row, WarrantyGuaranteeTicketRatio,
                       new PXSetPropertyException("現金佔比(%) 需大於等於0", PXErrorLevel.Error));
            }

            if (WarrantyGuaranteeTicketRatio > 100)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeTicketRatio>(row, WarrantyGuaranteeTicketRatio,
                       new PXSetPropertyException("現金佔比(%) 需小於等於100", PXErrorLevel.Error));
            }
        }

        protected virtual void KGContractPricingRule_WarrantyGuaranteeCashRatio_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal WarrantyGuaranteeCashRatio = (decimal)e.NewValue;
            if (WarrantyGuaranteeCashRatio < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeCashRatio>(row, WarrantyGuaranteeCashRatio,
                       new PXSetPropertyException("保固票佔比(%) 需大於等於0", PXErrorLevel.Error));
            }
            if (WarrantyGuaranteeCashRatio > 100)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeCashRatio>(row, WarrantyGuaranteeCashRatio,
                       new PXSetPropertyException("保固票佔比(%) 需小於等於100", PXErrorLevel.Error));
            }
        }

        //2019/09/19 ADD 檢核 輸入%數 需在0~100範圍之間
        protected virtual void KGContractPricingRule_DefRetainagePct_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            decimal DefRetainagePct = (decimal)e.NewValue;
            if (DefRetainagePct < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.defRetainagePct>(row, DefRetainagePct,
                       new PXSetPropertyException("保留款百分比(%) 需大於等於0", PXErrorLevel.Error));
            }
            if (DefRetainagePct > 100)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.defRetainagePct>(row, DefRetainagePct,
                       new PXSetPropertyException("保留款百分比(%) 需小於等於100", PXErrorLevel.Error));
            }
        }

        protected virtual void KGContractPricingRule_IsPrepayments_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;

            if (row == null) return;
            if (row.IsPrepayments == null) return;

            if (!row.IsPrepayments.Value)
            {
                row.PrepaymentsAmtUntaxed = 0;
                row.DeductionRatioPeriod = 0;
                row.PrepaymentRatioUntaxed = 0;
            }
            //2019/10/07改為唯讀
            //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.prepaymentsAmtUntaxed>(sender, row, row.IsPrepayments.Value);
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.deductionRatioPeriod>(sender, row, row.IsPrepayments.Value);
            //2019/09/19有預付款打開此欄位維護
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.prepaymentRatioUntaxed>(sender, row, row.IsPrepayments.Value);
        }

        //2019/09/09 ADD IsWarranty = True打開欄位維護, 反之清空欄位設為唯讀
        protected virtual void KGContractPricingRule_IsWarranty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;

            if (row == null) return;
            if (row.IsWarranty == null) return;

            if (!row.IsWarranty.Value)
            {
                row.WarrantyYear = 0;
                row.WarrantyCalcuMethod = null;
                row.WarrantyRatioUntaxed = 0;
                //row.FixedAmtUntaxed = 0;
                row.WarrantyGuaranteeCashRatio = 0;
                row.WarrantyGuaranteeTicketRatio = 0;

            }

            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyYear>(sender, row, true);
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyCalcuMethod>(sender, row, true);
            //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.fixedAmtUntaxed>(sender, row, row.WarrantyCalUntaxedRB == "A");
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyRatioUntaxed>(sender, row, true);
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeCashRatio>(sender, row, true);
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeTicketRatio>(sender, row, true);
        }

        //2019/09/19 ADD 填預付款比例自動算出預付款金額
        //2019/10/07 ADD 更新預付款比例帶到供應商的資訊的prepayment percent
        protected virtual void KGContractPricingRule_PrepaymentRatioUntaxed_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            POOrder order = POOrders.Current as POOrder;
            if (row == null) return;
            if (row.PrepaymentRatioUntaxed == null) return;

            row.PrepaymentsAmtUntaxed = (decimal?)System.Math.Round
                ((double)((order.CuryLineTotal + order.CuryLineRetainageTotal) * row.PrepaymentRatioUntaxed / 100), 0,
                MidpointRounding.AwayFromZero);
            order.PrepaymentPct = row.PrepaymentRatioUntaxed;
            POOrders.Update(order);
        }

        protected virtual void KGContractPricingRule_RetainageApply_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;

            //if (row.RetainageApply == null) return;
            //POOrder poOrder = POOrders.Current;
            if (row != null && row.RetainageApply == false)
            {
                row.DefRetainagePct = 0;
                //    poOrder.DefRetainagePct = 0;
                //    PXResultset<POLine> Poline = PXSelect<POLine,
                //               Where<POLine.orderNbr,
                //                   Equal<Required<POLine.orderNbr>>,
                //               And<POLine.orderType,
                //                   Equal<Required<POLine.orderType>>>>>
                //                   .Select(Base, row.OrderNbr, row.OrderType);
                //    foreach (POLine poline in Poline)
                //    {
                //        poline.RetainagePct = 0;
                //        POLines.Update(poline);
                //    }
                //            
                //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.defRetainagePct>(sender, row, row.RetainageApply == true);

                //poOrder.RetainageApply = row.RetainageApply;
            }
        }

        //protected virtual void KGContractPricingRule_DefRetainagePct_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        //{
        //    KGContractPricingRule row = (KGContractPricingRule)e.Row;
        //    PXResultset<POLine> Poline = PXSelect<POLine,
        //                   Where<POLine.orderNbr,
        //                       Equal<Required<POLine.orderNbr>>,
        //                   And<POLine.orderType,
        //                       Equal<Required<POLine.orderType>>>>>
        //                       .Select(Base, row.OrderNbr, row.OrderType);
        //    if (row == null) return;
        //    POOrder poOrder = POOrders.Current;
        //    //零星不跑
        //    if (POOrderType.RegularSubcontract.Equals(poOrder.OrderType))
        //    {
        //    }
        //    else {
        //        poOrder.DefRetainagePct = row.DefRetainagePct;

        //        foreach (POLine poline in Poline)
        //        {
        //            poline.RetainagePct = row.DefRetainagePct;
        //            POLines.Update(poline);
        //        }
        //    }
        //}

        protected virtual void KGContractPricingRule_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            POOrder order = POOrders.Current;
            if (row == null) return;
            if (order.Status == POOrderStatus.Hold)
            {
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.defRetainagePct>(cache, row, row.RetainageApply == true);
                //2019/10/07改為唯讀
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.prepaymentsAmtUntaxed>(cache, row, false);
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.deductionRatioPeriod>(cache, row, row.IsPrepayments == true);
                //2019/09/19 ADD有預付款打開此欄位維護
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.prepaymentRatioUntaxed>(cache, row, row.IsPrepayments == true);

                //2019/09/09 ADD 若IsWarranty=True 打開欄位維護, 反之欄位為唯讀
                if (row.IsWarranty == true)
                {
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyYear>(cache, row, true);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyCalcuMethod>(cache, row, true);
                    //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.fixedAmtUntaxed>(cache, row, row.WarrantyCalUntaxedRB == "A");
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyRatioUntaxed>(cache, row, true);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeCashRatio>(cache, row, true);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeTicketRatio>(cache, row, true);
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyYear>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyCalcuMethod>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyRatioUntaxed>(cache, row, false);
                    //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.fixedAmtUntaxed>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeCashRatio>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeTicketRatio>(cache, row, false);
                }
            }
        }

        protected virtual void KGContractPricingRule_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;

            if(row.DefRetainagePct !=null ||row.DefRetainagePct!=0)
            {
                checkDefRetainage(cache, row);
            }

            if (row.IsPrepayments == true)
            {
                checkPrepayment(cache, row);
            }
            
            if (row.IsWarranty == true)
            {
                //if (!((row.FixedAmtUntaxed == 0 || row.FixedAmtUntaxed == null)&& 
                if (!(row.WarrantyRatioUntaxed == 0 || row.WarrantyRatioUntaxed == null))
                {

                    checkWarranty(cache, row);
                    checkWarrantynegative(cache, row);
                }
            }
            //mark by louis for not checcking Compliance Document 20201103
            //if (row.IsPerformanceGuarantee ==true)
            //{
                //checkComplianceDocument(cache, row);
            //}

            if (setCheckThread == true)
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            }
            POOrder poOrder = POOrders.Current;
            if (poOrder == null) return;
            //poOrder.RetainageApply = row.RetainageApply;
            //poOrder.DefRetainagePct = row.DefRetainagePct;
            poOrder.PrepaymentPct = row.PrepaymentRatioUntaxed;
            POOrders.Update(poOrder);
        }

        //2019/07/04 ADD WarrantyCalUntaxedRB(RadioButton)
        //2019/09/19 不需要RadioButton了
        /*protected virtual void KGContractPricingRule_WarrantyCalUntaxedRB_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = KGContractPricingRules.Current;
            if (row == null)return;
            if (row.WarrantyCalUntaxedRB == "A")
            {
                row.WarrantyRatioUntaxed = 0;
            }
            else if (row.WarrantyCalUntaxedRB == "R")
            {
                row.FixedAmtUntaxed = 0;
            }
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.fixedAmtUntaxed>(sender, row, row.WarrantyCalUntaxedRB == "A");
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyRatioUntaxed>(sender, row, row.WarrantyCalUntaxedRB == "R");       
        }
        */

        //2019/09/19 FixedAmtUntaxed 拿掉了
        /*protected virtual void KGContractPricingRule_WarrantyRatioUntaxed_FieldUpdated(PXCache sender,PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            POOrder order = POOrders.Current;
            if (row == null) return;
            //固定金額四捨五入到整數
            row.FixedAmtUntaxed = 
                (decimal?)System.Math.Round
                ((double)(order.CuryLineTotal * row.WarrantyRatioUntaxed / 100), 0,
                MidpointRounding.AwayFromZero);
        }

        protected virtual void KGContractPricingRule_FixedAmtUntaxed_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            POOrder order = POOrders.Current;
            if (row == null) return;
            //固定金額四捨五入到小整2位
            row.WarrantyRatioUntaxed =
                (decimal?)System.Math.Round
                ((double)(row.FixedAmtUntaxed/order.CuryLineTotal * 100), 2,
                MidpointRounding.AwayFromZero);
        }*/

        //2019/09/19 把FixedAmtUntaxes拿掉
        /*protected virtual void KGContractPricingRule_FixedAmtUntaxed_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;
            if (e.NewValue == null) return;
            POOrder order = POOrders.Current;
            decimal fixedAmtUntaxed = (decimal)e.NewValue;
            if (fixedAmtUntaxed < 0)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.fixedAmtUntaxed>(row, fixedAmtUntaxed,
                       new PXSetPropertyException("保固款計算方式-固定金額(未稅)", PXErrorLevel.Error));
            }
            if(fixedAmtUntaxed>order.CuryLineTotal)
            {
                sender.RaiseExceptionHandling<KGContractPricingRule.fixedAmtUntaxed>(row, fixedAmtUntaxed,
                       new PXSetPropertyException("保固款計算方式-固定金額(未稅)不可大於行總計", PXErrorLevel.Error));
            }
        }*/

        //2019/09/19取消輸入預付款金額帶出預付款比例的機制
        /*protected virtual void KGContractPricingRule_PrepaymentsAmtUntaxed_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGContractPricingRule row = (KGContractPricingRule)e.Row;
            if (row == null) return;

            if (row.PrepaymentsAmtUntaxed >= 0)
            {
                POOrder order = POOrders.Current as POOrder;
                if (order.CuryLineTotal + order.CuryRetainageTotal != 0)
                {
                    row.PrepaymentRatioUntaxed = (row.PrepaymentsAmtUntaxed / (order.CuryLineTotal + order.CuryRetainageTotal)) * 100;
                }
            }
            checkPrepaymentsAmtUntaxed(sender, row, row.PrepaymentsAmtUntaxed);
        }*/
       
        #endregion

        #region KGContractDocTag

        protected virtual void KGContractDocTag_Tagid_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            KGContractDocTag row = (KGContractDocTag)e.Row;

            if (row == null) return;
            if (row.Tagid == null) return;

            KGContractTag tag = PXSelect<KGContractTag,
                    Where<KGContractTag.tagid, Equal<Required<KGContractTag.tagid>>>>
                    .Select(Base, row.Tagid);

            if (tag != null)
            {
                row.TagContent = tag.TagContent;
            }
        }

        #endregion

        #region PMCostBudget
        [PXSelector(typeof(Search<PMTask.taskID>), SubstituteKey = typeof(PMTask.taskCD))]
        [PXDBInt()]
        [PXDimension(ProjectTaskAttribute.DimensionName)]
        protected void PMCostBudget_ProjectTaskID_CacheAttached(PXCache sender) { }

        //2021/04/13 Add Mantis:0012018
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUnboundDefault(typeof(Current<POOrder.projectID>))]

        protected virtual void PMProjectCostFilter_ContractID_CacheAttached(PXCache sender) { }

        #endregion

        #region KGAgentFlow
        protected virtual void KGPoOrderFile_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGPoOrderFile row = KGPoOrderFiles.Current;
            POOrder pOOrder = POOrders.Current;
            row.OrderNbr = pOOrder.OrderNbr;

            KGPoOrderFiles.Update(row);
            //Base.Persist();


        }
        #endregion

        #region KGPoOrderpayment
        public const string warningMsg = "付款比例(%)不可為負數 !";
        public const string errorMsg = "付款比例(%)加總需等於100(%)";

        protected virtual void KGPoOrderPayment_PaymentPct_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            if (row == null) return;
            
            checkKGPoOrderpaymentnegative(sender, row);
        }
        protected virtual void KGPoOrderPayment_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            POOrder pOOrder = POOrders.Current;  
            
            checkKGPoOrderpaymentnegative(sender, row);
            checkKGPoOrderpayment(sender, row);
            checkPaymentPct(sender, row);
            
            if (setCheckThread == true)
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            }
        }

        #endregion

        #endregion

        #region Methods
        //WebService
        public String CreatePoOrderRequestFile()
        {
            //AgentFlow登入
            
            POOrder pOOrder = POOrders.Current;

            KGContractDoc kGContractDoc = PXSelect<KGContractDoc,
               Where<KGContractDoc.orderNbr,
               Equal<Required<KGContractDoc.orderNbr>>,
               And<KGContractDoc.orderType,
               Equal<Required<KGContractDoc.orderType>>>>>
               .Select(Base, pOOrder.OrderNbr, pOOrder.OrderType);
            if (kGContractDoc == null)
            {
                throw new PXException("kGContractDoc 不能為空");
            }

            //2020/02/27 ADD 若KGFlowContractor有相同的單號請先刪除再新增。
            KGFlowContractor contractor = PXSelect<KGFlowContractor,
                Where<KGFlowContractor.purchaseNo, Equal<Required<POOrder.orderNbr>>>>
                .Select(Base, pOOrder.OrderNbr);
            if (contractor != null)
            {
                KGFlowContractors.Delete(contractor);
                Base.Persist();
            }

            

            KGFlowContractor kGFlowContractor = new KGFlowContractor();
            Contract contract = PXSelect<Contract,
                Where<Contract.contractID,
                Equal<Required<Contract.contractID>>>>
                .Select(Base, pOOrder.ProjectID);
            PX.Objects.GL.Branch branch = PXSelect<PX.Objects.GL.Branch, Where<PX.Objects.GL.Branch.branchID, Equal<Required<POOrder.branchID>>>>.Select(Base, pOOrder.BranchID);
            
            if (branch != null)
            {
                kGFlowContractor.DeptName = branch.AcctName;
            }
            kGFlowContractor.ProjectName = contract.Description;
            kGFlowContractor.ProjectCode = contract.ContractCD;
            kGFlowContractor.SubCode = contract.ContractCD.Trim() + "-" + pOOrder.OrderNbr;
            kGFlowContractor.PurchaseNo = pOOrder.OrderNbr;
            kGFlowContractor.SubName = pOOrder.OrderDesc;
            KGContractCategory kGContractCategory = PXSelectReadonly<KGContractCategory,
                Where<KGContractCategory.contractCategoryID, 
                Equal<Required<KGContractCategory.contractCategoryID>>>>
                .Select(Base, kGContractDoc.ContractCategoryID);

            kGFlowContractor.CntTypeName = kGContractCategory.ContractCategoryCD;
            BAccount bAccountemployee = PXSelect<BAccount,
                Where<BAccount.bAccountID,
                Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, pOOrder.EmployeeID);
            kGFlowContractor.PurchaseOwnerName = bAccountemployee.AcctName;
            BAccount bAccountvendor = PXSelect<BAccount,
                Where<BAccount.bAccountID,
                Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, pOOrder.VendorID);
            kGFlowContractor.Title = bAccountvendor.AcctName;
            Location location = PXSelect<Location,
                Where<Location.bAccountID,
                Equal<Required<Location.bAccountID>>>>
                .Select(Base, pOOrder.VendorID);
            kGFlowContractor.InvNo = location.TaxRegistrationID;
           
            kGFlowContractor.GetProjectDate = kGContractDoc.ContractDocDate;
            //modify by louis 20200312 agentFlow未稅金額,  包含LineTotal + CuryRetainageTotal;
            //kGFlowContractor.Ctotal = pOOrder.LineTotal;

            //edit by alton 20200430 manits:11592
            //kGFlowContractor.Ctotal = pOOrder.LineTotal + pOOrder.CuryRetainageTotal;
            //kGFlowContractor.Ctax = pOOrder.CuryTaxTotal;
            kGFlowContractor.Ctotal = kGContractDoc.TotalAmt;
            kGFlowContractor.Ctax = kGContractDoc.TotalTaxAmt;

            kGFlowContractor.Camount = pOOrder.CuryOrderTotal;
            //revice By Jerry 20190724
            kGFlowContractor=KGFlowContractors.Insert(kGFlowContractor);

            //using (PXTransactionScope ts = new PXTransactionScope())
            //{

                Base.Actions.PressSave();
                //2019/06/12POOrder加上客製欄位/新增SubUID到客製欄位裡
                KGFlowContractor kGFlow = KGFlowContractors.Current;
                POOrderExt poorderext = PXCache<POOrder>.GetExtension<POOrderExt>(pOOrder);
                //改時機點ByJerry 改在回寫ReturURL那
                /*
                PXUpdate<Set<POOrderExt.usrKGFlowUID, Required<POOrderExt.usrKGFlowUID>>,
                    POOrder,
                    Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>,
                    And<POOrder.orderType, Equal<Required<POOrder.orderType>>>>>
                    .Update(Base, kGFlow.SubUID, pOOrder.OrderNbr, pOOrder.OrderType);*/

                //建立kguploadFile
                foreach (KGPoOrderFile poOrderFile in KGPoOrderFiles.Select())
                {
                    if (poOrderFile.NoteID == null)
                    {
                        continue;
                    }
                    // KGPoOrderFile row = KGPoOrderFiles.Current;
                    NoteDoc notedoc = PXSelect<NoteDoc,
                         Where<NoteDoc.noteID,
                         Equal<Required<NoteDoc.noteID>>>>
                         .Select(Base, poOrderFile.NoteID);
                    if (notedoc != null)
                    {

                        KGFlowUploadFile kGFlowUploadFile = new KGFlowUploadFile();
                        kGFlowUploadFile.Fileid = notedoc.FileID;
                        UploadFile uploadFile = PXSelect<UploadFile,
                            Where<UploadFile.fileID,
                            Equal<Required<UploadFile.fileID>>>>
                            .Select(Base, notedoc.FileID);

                        if (uploadFile == null) { return null; }
                        kGFlowUploadFile.FileName = uploadFile.Name;
                        string buildUtl = PXRedirectToFileException.BuildUrl(notedoc.FileID);
                        string Absouri = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;

                        buildUtl = buildUtl.Substring(1);
                        Absouri = Absouri.Substring(0, Absouri.IndexOf("/M"));

                        kGFlowUploadFile.FileLink = Absouri + buildUtl;
                        kGFlowUploadFile.Category = "PO";
                        //2019/06/14 Uploadfile.RefNo = kGFlowContractor.subuid(不是orderNbr)
                        kGFlowUploadFile.RefNo = kGFlow.SubUID;
                        //kGFlowUploadFile.RefNo = pOOrder.OrderNbr;
                        kGFlowUploadFile.FileType = poOrderFile.FileType;
                        KGFlowUploadFiles.Insert(kGFlowUploadFile);
                        //Base.Actions.PressSave();
                    }
                }
              
                EPApproval epApproval = PXSelect<EPApproval,
                                Where<EPApproval.refNoteID,
                                Equal<Required<EPApproval.refNoteID>>, And<EPApproval.status, Equal<Required<EPApproval.status>>>>>
                                .Select(Base, pOOrder.NoteID, EPApprovalStatus.Pending);
                if (epApproval != null)
                {
                    kGFlowContractor.ApprovalID = epApproval.ApprovalID;
                }
                kGFlowContractor = KGFlowContractors.Update(kGFlowContractor);
         
                Base.Actions.PressSave();
                bool ckeck = agentFlow(kGFlowContractor, epApproval);
                if (message != null)
                {
                    return message;
                }

                if (!ckeck)
                {
                    return message;
                }

                /*
                foreach (KGRequestFile kgRequestFile in KGRequestFiles.Select()) {
                    KGFlowUploadFile kgFlowUploadFile = (KGFlowUploadFile)KGFlowReqsFiles.Cache.CreateInstance();
                    kgFlowUploadFile.FileType = kgRequestFile.FileType;
                    kgFlowUploadFile.RefNo = master.OrderNbr;
                    kgFlowUploadFile.FileName = kgRequestFile.FIleDesc;
                    KGFlowReqsFiles.Insert(kgFlowUploadFile);

                }*/
                //Kedge.ws.AgentflowWeb.AgentflowWebService soapClient =        new Kedge.ws.AgentflowWeb.AgentflowWebService();

                 //ts.Complete();
            //}
            return null;
        }
        
        String message = null;
        public bool agentFlow(KGFlowContractor kGFlowContractor, EPApproval epApproval)
        {
            POOrder master = Base.Document.Current;
            POOrderExt poorderext = PXCache<POOrder>.GetExtension<POOrderExt>(master);
            message = null;
            String status = null;
            AgentFlowWebServiceUtil.createContractProcessKedgeURL(Base.Accessinfo.UserName, kGFlowContractor.SubUID, master.ProjectID, ref status, ref message);
            //此時要是錯誤 message 會有值，Status也塞好了
            if (message != null)
            {
                return false;
            }
            //status is Call Web Servie return value
            if (status != null && status.StartsWith("http"))
            {
                if (epApproval != null)
                {
                    EPApprovalExt epApprovalExt = PXCache<EPApproval>.GetExtension<EPApprovalExt>(epApproval);
                    epApprovalExt.UsrReturnUrl = status;
                    poorderext.UsrReturnUrl = status;
                    poorderext.UsrKGFlowUID = kGFlowContractor.SubUID;
                    Base.Approval.Update(epApproval);
                    
                    Base.Document.Update(master);
                    Base.Actions.PressSave();

                }
                //Base.Document.Cache.RaiseExceptionHandling<RQRequest.status>(master, master.Status, new PXSetPropertyException("AgentFlowµLªk¼g¤JcreateROProcess"));
            }
            else
            {
                message = "呼叫AgentFlow失敗:" + status;
                return false;
            }

            return true;
            

        }
        // serial number for SegPricingID  
        protected int GetNextSegPricingID()
        {
            KGContractSegPricing row = PXSelectOrderBy<KGContractSegPricing, OrderBy<Desc<KGContractSegPricing.segPricingID>>>.SelectSingleBound(Base, null, new byte[0]);
            if (row == null) { return 1; }
            return (int)row.SegPricingID + 1;
        }

        //2019/06/10加上預付款金額不可大於行總計檢核
        bool setCheckThread = false;
        //2019/10/07改為唯讀不須檢核
        /*public bool checkPrepaymentsAmtUntaxed(PXCache sender, KGContractPricingRule row, decimal? newValue)
        {
            POOrder pOOrder = POOrders.Current;
            //add by louis for newValue null exception 20190725
            if (newValue!=null) { 
                if (newValue > pOOrder.CuryLineTotal)
                {
                    setCheckThread = true;

                    KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.prepaymentsAmtUntaxed>(
                         row, newValue, new PXSetPropertyException("預付款金額(未稅)不可大於行總計!"));

                    return true;
                }
            }
            return false;
        }*/

        protected bool ValidKGContractSegPricingTotalPercentage()
        {
            decimal? totalPercentage = 0;
            bool inValid = false;
            foreach (KGContractSegPricing segPrice in KGContractSegPricings.Select())
            {
                PXEntryStatus status = KGContractSegPricings.Cache.GetStatus(segPrice);

                if (status != PXEntryStatus.Deleted ||
                    status != PXEntryStatus.InsertedDeleted)
                {
                    totalPercentage += segPrice.SegmentPercent;
                }

                if (segPrice.SortOrder == null || segPrice.SegmentName == null)
                {
                    inValid = true;
                } 
            }
            return !inValid && (totalPercentage == 100 || totalPercentage == 0);
        }
        protected bool ValidPOLineSegPricing(PXCache sender)
        {
            foreach (POLine line in POLines.Select())
            {
                // must set current POLine
                POLines.Current = line;

                if (!ValidKGContractSegPricingTotalPercentage())
                {

                    sender.RaiseExceptionHandling<POLine.inventoryID>(line, line.InventoryID,
                       new PXSetPropertyException("分段計價的計價比率未達100%", PXErrorLevel.RowError));
                    return false;
                }
            }
            return true;
        }

        protected decimal? GetKGContractSegPricingTotalPercentage()
        {
            decimal? totalPercentage = 0;

            foreach (KGContractSegPricing segPrice in KGContractSegPricings.Select())
            {
                PXEntryStatus status = KGContractSegPricings.Cache.GetStatus(segPrice);
                
                if (status != PXEntryStatus.Deleted ||
                    status != PXEntryStatus.InsertedDeleted)
                {
                    totalPercentage += segPrice.SegmentPercent;
                }
            }
            return totalPercentage;
        }

        private void DownloadContract()
        {           
            // 先取出合約範本的附件
            ArrayList files = GetKGContractTagTemplate();
            if (files.Count == 0)
            {
                throw new Exception("there is no template file");
            }
            if (files.Count != 1)
            {
                throw new Exception("there are multiple template files, please check which one is you want.");
            }

            /*PX.SM.FileInfo fi = (PX.SM.FileInfo)files[0];
            byte[] result = ReplaceTemplateTags((PX.SM.FileInfo)files[0]);
            MemoryStream stream = new MemoryStream(result);
            HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=\"根基.docx\"");
                HttpContext.Current.Response.AppendHeader("Content-Length", stream.ToArray().Length.ToString());
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();*/
            try
            {
                POOrder row = POOrders.Current;
                PMProject project =
                    PXSelect<PMProject, 
                    Where<PMProject.contractID, Equal<Required<POOrder.orderNbr>>>>
                    .Select(Base, row.ProjectID);
                if (row.Status == "D")
                {
                    row.Status = "N";
                    POOrders.Update(row);
                    Base.Save.Press();
                }
                PX.SM.FileInfo fi = (PX.SM.FileInfo)files[0];
                byte[] result = ReplaceTemplateTags((PX.SM.FileInfo)files[0]);

                throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(),
                                                                        //fi.OriginalName,
                                                                       project.ContractCD.Trim() + "_"+row.OrderNbr+".docx",
                                                                       null,
                                                                       result), true);
            }
            catch
            {
                PX.SM.FileInfo fi = (PX.SM.FileInfo)files[0];
                byte[] result = ReplaceTemplateTags((PX.SM.FileInfo)files[0]);

                POOrder row = POOrders.Current;
                PMProject project =
                    PXSelect<PMProject,
                    Where<PMProject.contractID, Equal<Required<POOrder.orderNbr>>>>
                    .Select(Base, row.ProjectID);

                throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(),
                                                                       //fi.OriginalName,
                                                                       project.ContractCD.Trim() + "_" + row.OrderNbr + ".docx",
                                                                       null,
                                                                       result), true);
            }
            finally
            {
                POOrder row = POOrders.Current;
                if (row.Status == "D")
                {
                    row.Status = "N";
                    POOrders.Update(row);
                    Base.Save.Press();
                }
            }
        }

        private byte[] ReplaceTemplateTags(PX.SM.FileInfo fi)
        {          
            MemoryStream ms = new MemoryStream(fi.BinData);
            using (DocX document = DocX.Load(ms))
            {
                // Check if all the replace patterns are used in the loaded document.
           
                foreach (KGContractDocTag item in KGContractDocTags.Select())
                {
                    if (item.Active == null || !(bool)item.Active ||
                        item.TagContent == null || item.TagContent.Empty_())
                    {
                        //2019/07/24 ADD若為空值就顯示空
                          item.TagContent = " ";
                    }
                    
                    KGContractTag contractTag = PXSelect<KGContractTag,
                        Where<KGContractTag.tagid, Equal<Required<KGContractTag.tagid>>>>
                        .Select(Base, item.Tagid);
                    if (contractTag == null)
                    {
                        continue;
                    }
                 
                    document.ReplaceText(contractTag.TagDesc, item.TagContent);
                }

                using (MemoryStream msOut = new MemoryStream())
                {
                    document.SaveAs(msOut);
                    return msOut.ToArray();
                }
            }            
        }
      
        private ArrayList GetKGContractTagTemplate()
        {
            // 先取出合約範本的附件
            KGContractCategory contract = PXSelect<KGContractCategory,
                                            Where<KGContractCategory.contractCategoryID, Equal<Required<KGContractTag.contractCategoryID>>>>
                                                .Select(Base, KGContractDocs.Current.ContractCategoryID);

            if (contract == null) {
                return new ArrayList();
            }

            //2020/03/12 ADD 下載檔案前先檢查上傳檔案是否為.docx檔
            NoteDoc noteDoc = PXSelect<NoteDoc, Where<NoteDoc.noteID,
            Equal<Required<NoteDoc.noteID>>>>.Select(Base, contract.NoteID);

            if(noteDoc == null)
            {
                throw new Exception("請至合約範本上傳合約範本檔案!");
            }

            UploadFile uploadFile = PXSelect<UploadFile, Where<UploadFile.fileID,
            Equal<Required<UploadFile.fileID>>>>.Select(Base, noteDoc.FileID);

            if(uploadFile.Name.LastIndexOf(".docx")==-1 || uploadFile.Name.Length-5!=uploadFile.Name.LastIndexOf(".docx"))
            {
                throw new Exception("上傳Word檔案格式需為'.docx'檔,請至KG合約範本檢查上傳Word檔案格式!");
            }

            var contractCache = Base.Caches[typeof(KGContractCategory)];

            Guid[] fileIDs = PXNoteAttribute.GetFileNotes(contractCache, contract);
            ArrayList files = new ArrayList(fileIDs.Length);
            var fm = PXGraph.CreateInstance<PX.SM.UploadFileMaintenance>();

            foreach (Guid fileID in fileIDs)
            {
                PX.SM.FileInfo fi = fm.GetFile(fileID);
                files.Add(fi);
            }
            return files;
        }

        private bool IsExistKGContractDocTag(KGContractDocTag newTag)
        {
            foreach(KGContractDocTag tag in KGContractDocTags.Select())
            {
                if (tag.Tagid == newTag.Tagid)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isDuplicateKGContractSegPricingSortOrder(int sortOrder)
        {
            foreach (KGContractSegPricing item in KGContractSegPricings.Select())
            {
                if (item.SortOrder == null) continue;
                if (item.SortOrder == sortOrder)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isDuplicateKGContractSegPricingSegmentName(string segmentName)
        {
            foreach (KGContractSegPricing item in KGContractSegPricings.Select())
            {
                if (item.SegmentName == null) continue;
                if (item.SegmentName.CompareTo(segmentName) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void ControlsSetEnabled(POOrder order)
        {
            bool editable = order.Status == POOrderStatus.Hold;
            //// Order 不是開放的狀態時就不能更新
            PXUIFieldAttribute.SetEnabled(KGContractDocs.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled(KGContractPricingRules.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled(KGContractRelatedVendors.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled(KGContractDocTags.Cache, null, editable);

            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.paymentMethod>(KGContractPricingRules.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.priceCalculationMethod>(KGContractPricingRules.Cache, null, false);

            //2019/10/07改為唯讀
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.prepaymentsAmtUntaxed>(KGContractPricingRules.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.deductionRatioPeriod>(KGContractPricingRules.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<KGContractPricingRule.defRetainagePct>(KGContractPricingRules.Cache, null, false);

            KGContractPricingRule rule = (KGContractPricingRule)KGContractPricingRules.Current;
            if (rule == null)
            {
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.defRetainagePct>(KGContractPricingRules.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyRatioUntaxed>(KGContractPricingRules.Cache, null, false);
                //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.fixedAmtUntaxed>(KGContractPricingRules.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyCalcuMethod>(KGContractPricingRules.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeTicketRatio>(KGContractPricingRules.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeCashRatio>(KGContractPricingRules.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyYear>(KGContractPricingRules.Cache, null, false);
            }
            else
            {
                if (editable)
                {
                    bool isWarranty=false;
                    if(rule.IsWarranty == null)
                    {
                        isWarranty = false;
                    }
                    else
                    {
                        isWarranty = (bool)rule.IsWarranty;
                    }
                    //2019/10/07改為唯讀
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.prepaymentsAmtUntaxed>(KGContractPricingRules.Cache, null, false);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.deductionRatioPeriod>(KGContractPricingRules.Cache, null, rule.IsPrepayments == true);
                    //2019/09/19 ADD 有預付款此欄位打開維護
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.prepaymentRatioUntaxed>(KGContractPricingRules.Cache, null, rule.IsPrepayments == true);


                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.defRetainagePct>(KGContractPricingRules.Cache, null, rule.RetainageApply == true);
                    //2019/07/05 ADD UnTaxed的開關
                    //2019/09/19 Take OFF
                    //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyRatioUntaxed>(KGContractPricingRules.Cache, null, rule.WarrantyCalUntaxedRB == "R");
                    //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.fixedAmtUntaxed>(KGContractPricingRules.Cache, null, rule.WarrantyCalUntaxedRB == "A");
                    //2019/09/09 ADD isWarranty的開關
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyRatioUntaxed>(KGContractPricingRules.Cache, null, isWarranty);
                    //PXUIFieldAttribute.SetEnabled<KGContractPricingRule.fixedAmtUntaxed>(KGContractPricingRules.Cache, null, isWarranty);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyCalcuMethod>(KGContractPricingRules.Cache, null, isWarranty);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeTicketRatio>(KGContractPricingRules.Cache, null, isWarranty);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyGuaranteeCashRatio>(KGContractPricingRules.Cache, null, isWarranty);
                    PXUIFieldAttribute.SetEnabled<KGContractPricingRule.warrantyYear>(KGContractPricingRules.Cache, null, isWarranty);

                }
            }

            PXUIFieldAttribute.SetEnabled<KGContractDocTag.active>(KGContractDocTags.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled<KGContractDocTag.tagid>(KGContractDocTags.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled<KGContractDocTag.tagContent>(KGContractDocTags.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled<KGContractDocTag.tagDesc>(KGContractDocTags.Cache, null, editable);
            KGContractDocTags.AllowInsert = false;
            KGContractDocTags.AllowDelete = false;

            // Move the following controls to [KGContractSegPricing_RowSelected] event.
            //PXUIFieldAttribute.SetEnabled(KGContractSegPricings.Cache, null, editable);
            //KGContractSegPricings.AllowDelete = editable;
            //KGContractSegPricings.AllowInsert = editable;

            PXUIFieldAttribute.SetEnabled<KGContractRelatedVendor.relatedType>(KGContractRelatedVendors.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled<KGContractRelatedVendor.relatedVendorID>(KGContractRelatedVendors.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled<KGContractRelatedVendor.relatedVendorName>(KGContractRelatedVendors.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<KGContractRelatedVendor.relatedVendorTaxRegistration>(KGContractRelatedVendors.Cache, null, false);
            KGContractRelatedVendors.AllowInsert = editable;
            KGContractRelatedVendors.AllowDelete = editable;

            //SegmentPricing.SetEnabled(editable);

            ImportSampleTags.SetEnabled(editable);

            // prevent doc not reload
            KGContractDoc doc = KGContractDocs.Current as KGContractDoc;
            if (doc == null)
            {
                doc = PXSelect<KGContractDoc,
                    Where<KGContractDoc.orderNbr, Equal<Required<POOrder.orderNbr>>,
                    And<KGContractDoc.orderType, Equal<Required<POOrder.orderType>>>>>
                    .Select(Base, order.OrderNbr, order.OrderType);
                if (doc != null)
                {
                    KGContractDocs.Current = doc;
                }
            }
            
            if (doc != null)
            {
                KGContractCategory category = PXSelectorAttribute.Select<KGContractCategory.contractCategoryID>(KGContractDocs.Cache, doc) as KGContractCategory;
                if (category != null && category.ContractType != null)
                {
                    PXUIFieldAttribute.SetEnabled<KGContractDoc.contractDesc>(KGContractDocs.Cache,doc,category.ContractType=="S" && editable);
                    PrintContract.SetEnabled(category.ContractType.Equals("C"));
                    PrintSimpleContract.SetEnabled(category.ContractType.Equals("S"));
                    //2021/04/08 Add 產生合約文件僅在類行為合約才能使用
                    PrintDetailContract.SetEnabled(category.ContractType.Equals("C"));
                } else
                {
                    PXUIFieldAttribute.SetEnabled<KGContractDoc.contractDesc>(KGContractDocs.Cache, doc, false);
                    PrintContract.SetEnabled(false);
                    PrintSimpleContract.SetEnabled(false);
                    PrintDetailContract.SetEnabled(false);

                }
            }
            else
            {
                PrintContract.SetEnabled(false);
                PrintSimpleContract.SetEnabled(false);
            }
            //2019/06/24 加上KGPoOrderPayment的開關
            KGPoOrderPayment kGPoOrderPayment = KGPoOrderPayments.Current;
            PXUIFieldAttribute.SetEnabled(KGPoOrderPayments.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled<KGPoOrderPayment.bankAccount>(KGPoOrderPayments.Cache, kGPoOrderPayment, false);
            //PXUIFieldAttribute.SetEnabled<KGPoOrderPayment.bankAccountID>(KGPoOrderPayments.Cache, kGPoOrderPayment, false);
            PXUIFieldAttribute.SetEnabled<KGPoOrderPayment.bankName>(KGPoOrderPayments.Cache, kGPoOrderPayment, false);
            KGPoOrderPayments.Cache.AllowInsert = editable;
            KGPoOrderPayments.Cache.AllowDelete = editable;

            PXUIFieldAttribute.SetEnabled(KGPoOrderFiles.Cache, KGPoOrderFiles.Cache.Current, editable);
            KGPoOrderFiles.Cache.AllowInsert = editable;
            KGPoOrderFiles.Cache.AllowDelete = editable;
        }

        private bool checkLineExist(PMCostBudget pmCost)
        {
            foreach (POLine line in Base.Transactions.Select())
            {
                if (line.ProjectID == pmCost.ProjectID &&
                    line.InventoryID == pmCost.InventoryID)
                {
                    return true;
                }
            }
            return false;
        }
       
        ///<summary>
        /// 計算[契約相關資訊] unbound column total
        ///</summary>
        private void setKGContractDocTotal() {
            KGContractDoc row = KGContractDocs.Current;
            if (row == null) return;
            //契約工程總價(未稅) = 稅額明細Tab中的(可課稅金額+保留款應稅) : POTaxTran.CuryTaxableAmt + POTaxTran.CuryRetainedTaxableAmt
            decimal? totalAmt = 0;
            //契約工程總價(營業稅) = 稅額明細Tab中的(稅額+保留稅額) : POTaxTran.CuryTaxAmt + POTaxTran.CuryRetainedTaxAmt
            decimal? totalTaxAmt = 0;
            foreach (POTaxTran item in Base.Taxes.Select()) {
                totalAmt += ((item.CuryTaxableAmt ?? 0) + (item.CuryRetainedTaxableAmt ?? 0));
                totalTaxAmt += ((item.CuryTaxAmt ?? 0) + (item.CuryRetainedTaxAmt ?? 0));
            }
            
            KGContractDocs.Current.TotalAmt = totalAmt;
            KGContractDocs.Current.TotalTaxAmt = totalTaxAmt;
        }

        private Vendor GetPOVendor(POOrder order)
        {
            return PXSelect<Vendor,
                    Where<Vendor.bAccountID, Equal<Required<POOrder.vendorID>>>>
                    .Select(Base, order.VendorID);
        }

        private Vendor GetKGContractrelatedVendorVendor(KGContractRelatedVendor ContractRelatedVendor)
        {
            if(ContractRelatedVendor!=null)
            {
                return PXSelect<Vendor,
                    Where<Vendor.bAccountID, Equal<Required<POOrder.vendorID>>>>
                    .Select(Base, ContractRelatedVendor.RelatedVendorID);
            }            
            else
            {
                return null;
            }
        }

        private Contract GetPOContract(POOrder order)
        {
            return PXSelect<Contract,
                    Where<Contract.contractID, Equal<Required<POOrder.projectID>>>>
                    .Select(Base, order.ProjectID);
        }

        private Location GetPORegistration(POOrder order)
        {
            return PXSelect<Location,
                    Where<Location.bAccountID, Equal<Required<POOrder.vendorID>>>>
                    .Select(Base, order.VendorID);
        }

        private Location GetKGContractRelatedVendorLocation(KGContractRelatedVendor contractRelatedVendor)
        {
            return PXSelect<Location,
                    Where<Location.bAccountID, Equal<Required<POOrder.vendorID>>>>
                    .Select(Base, contractRelatedVendor.RelatedVendorID);
        }

        private KGContractPricingRule GetKGContractPricingRule(POOrder order)
        {
            return PXSelect<KGContractPricingRule,
                Where<KGContractPricingRule.orderNbr, Equal<Required<POOrder.orderNbr>>,
                And<KGContractPricingRule.orderType, Equal<Required<POOrder.orderType>>>>>
                .Select(Base,order.OrderNbr,order.OrderType);
        }

        private KGContractDoc GetKGContractDoc(POOrder order)
        {
            return PXSelect<KGContractDoc,
                Where<KGContractDoc.orderType, Equal<Required<POOrder.orderType>>,
                And<KGContractDoc.orderNbr, Equal<Required<POOrder.orderNbr>>>>>
                .Select(Base, order.OrderType, order.OrderNbr);
        }

        private CSAnswers GetCSAnswers(POOrder order)
        {
            return PXSelectJoin<CSAnswers,
                InnerJoin<Vendor,
                On<Vendor.noteID, Equal<CSAnswers.refNoteID>,
                And<CSAnswers.attributeID, Equal<word.oWNER>,
                And<Vendor.bAccountID,Equal<Required<POOrder.vendorID>>>>>>>
                .Select(Base , order.VendorID);
        }

        private CSAnswers GetKGContractRelatedVendorCSAnswers(KGContractRelatedVendor contractRelatedVendor)
        {
            return PXSelectJoin<CSAnswers,
                InnerJoin<Vendor,
                On<Vendor.noteID, Equal<CSAnswers.refNoteID>,
                And<CSAnswers.attributeID, Equal<word.oWNER>,
                And<Vendor.bAccountID, Equal<Required<POOrder.vendorID>>>>>>>
                .Select(Base, contractRelatedVendor.RelatedVendorID);
        }

        private Address GetAddress(POOrder order)
        {
            return PXSelect<Address,
                Where<Address.bAccountID, Equal<Required<POOrder.vendorID>>>>
                .Select(Base, order.VendorID);
        }

        private Address GetKGContractRelatedVendorAddress (KGContractRelatedVendor contractRelatedVendor)
        {
            return PXSelect<Address,
                Where<Address.bAccountID, Equal<Required<POOrder.vendorID>>>>
                .Select(Base, contractRelatedVendor.RelatedVendorID);
        }

        private Contact GetContact(POOrder order)
        {
            return PXSelect<Contact,
                Where<Contact.contactID, Equal<Required<POOrder.projectID>>>>
                .Select(Base, order.ProjectID);
        }

        private Contact GetKGContractRelatedVendorContact(KGContractRelatedVendor contractRelatedVendor)
        {
            return PXSelect<Contact,
                Where<Contact.bAccountID, Equal<Required<POOrder.vendorID>>>>
                .Select(Base, contractRelatedVendor.RelatedVendorID);
        }

        private PMProject GetPMProject(POOrder order)
        {
            return PXSelect<PMProject,
                Where<PMProject.contractID, Equal<Required<POOrder.projectID>>>>
                .Select(Base, order.ProjectID);
        }

        private KGContractRelatedVendor GetKGContractRelatedVendor(POOrder order)
        {
            KGContractRelatedVendor contractRelatedVendor = PXSelect<KGContractRelatedVendor,
                Where<KGContractRelatedVendor.orderType, Equal<Required<POOrder.orderType>>,
                And<KGContractRelatedVendor.orderNbr, Equal<Required<KGContractRelatedVendor.orderNbr>>,
                And<KGContractRelatedVendor.relatedType, Equal<word.g>>>>>
                .Select(Base, order.OrderType, order.OrderNbr);
            if (contractRelatedVendor != null)
            {
                return PXSelect<KGContractRelatedVendor,
                Where<KGContractRelatedVendor.orderType, Equal<Required<POOrder.orderType>>,
                And<KGContractRelatedVendor.orderNbr, Equal<Required<KGContractRelatedVendor.orderNbr>>,
                And<KGContractRelatedVendor.relatedType, Equal<word.g>>>>>
                .Select(Base, order.OrderType, order.OrderNbr);
            }
            else
            {
                return null;
            }
                

        }

        private Branch GetBranch(int? BranchID)
        {
            return PXSelect<Branch,
                Where<Branch.branchID, Equal<Required<POOrder.branchID>>>>
                .Select(Base, BranchID);
        }

        private Contract GetContract(int? ContractID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID, Equal<Required<POOrder.projectID>>>>
                .Select(Base, ContractID);
        }

        //數字轉成中文大寫
        /// <summary>
        /// 數字轉大寫
        /// </summary>
        /// <param name="type">單價/數量</param>
        /// <param name="Num">數字</param>
        /// <returns></returns>
        public static string GetChineseNum(string type, string Num)
        {
            #region
            try
            {
                string m_1, m_2, m_3, m_4, m_5, m_6, m_7, m_8, m_9;
                m_1 = Num;
                string numNum = "0123456789.";
                string numChina = "零壹貳叁肆伍陸柒捌玖點";
                string numChinaWeigh = "個拾佰仟萬拾佰仟億拾佰仟萬";
                if (Num.Substring(0, 1) == "0")//0123-->123
                    Num = Num.Substring(1, Num.Length - 1);
                /*if (!Num.Contains(‘.‘))
                    Num += ".00";
                else//123.234  123.23 123.2
                    Num = Num.Substring(0, Num.IndexOf(‘.‘) + 1 + (Num.Split(‘.‘)[1].Length > 2 ? 3 : Num.Split(‘.‘)[1].Length));*/
                m_1 = Num;
                m_2 = m_1;
                m_3 = m_4 = "";
                //m_2:1234-> 壹貳叁肆
                for (int i = 0; i < 11; i++)
                {
                    m_2 = m_2.Replace(numNum.Substring(i, 1), numChina.Substring(i, 1));
                }
                //m_3:佰拾萬仟佰拾個
                int iLen = m_1.Length;
                if (m_1.IndexOf(".") > 0)
                    iLen = m_1.IndexOf(".");//獲取整數位數
                for (int j = iLen; j >= 1; j--)
                    m_3 += numChinaWeigh.Substring(j - 1, 1);
                //m_4:2行+3行
                for (int i = 0; i < m_3.Length; i++)
                    m_4 += m_2.Substring(i, 1) + m_3.Substring(i, 1);
                //m_5:4行去"0"後拾佰仟
                m_5 = m_4;
                m_5 = m_5.Replace("零拾", "零");
                m_5 = m_5.Replace("零佰", "零");
                m_5 = m_5.Replace("零仟", "零");
                //m_6:00-> 0,000-> 0
                m_6 = m_5;
                for (int i = 0; i < iLen; i++)
                    m_6 = m_6.Replace("零零", "零");
                //m_7:6行去億,萬,個位"0"
                m_7 = m_6;
                m_7 = m_7.Replace("億零萬零", "億零");
                m_7 = m_7.Replace("億零萬", "億零");
                m_7 = m_7.Replace("零億", "億");
                m_7 = m_7.Replace("零萬", "萬");
                if (m_7.Length > 2)
                    m_7 = m_7.Replace("零個", "個");
                //m_8:7行+2行小數-> 數目
                m_8 = m_7;
                m_8 = m_8.Replace("個", "");
                //if (m_2.Substring(m_2.Length - 3, 3) != "點零零")
                //    m_8 += m_2.Substring(m_2.Length - 3, 3);
                //m_9:7行+2行小數-> 價格
                m_9 = m_7;
                m_9 = m_9.Replace("個", "元");
                /*if (m_2.Substring(m_2.Length - 3, 3) != "點零零")
                {
                    m_9 += m_2.Substring(m_2.Length - 2, 2);
                    m_9 = m_9.Insert(m_9.Length - 1, "角");
                    m_9 += "分";
                }
                else*/ m_9 += "整";
                if (m_9 != "零元整")
                    m_9 = m_9.Replace("零元", "");
               // m_9 = m_9.Replace("零分", "整");
                if (type == "數量")
                    return m_8;
                else
                    return m_9;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            #endregion
        }

        //Check KGPoOrderpayment
        public bool checkKGPoOrderpayment(PXCache sender, KGPoOrderPayment row)
        {
            decimal? totalPaymPct = 0;
            PXEntryStatus status = KGPoOrderPayments.Cache.GetStatus(row);
            if (status == PXEntryStatus.Updated)
            {
                foreach (KGPoOrderPayment kGPOOPaym in KGPoOrderPayments.Select())
                {                    
                    totalPaymPct += kGPOOPaym.PaymentPct;
                }

                if (totalPaymPct != 100)
                {
                    setCheckThread = true;
                    KGPoOrderPayments.Cache.RaiseExceptionHandling<KGPoOrderPayment.paymentPct>(
                            row, row.PaymentPct, new PXSetPropertyException(errorMsg));
                    return true;
                }
            }
            return false;
        }
   
        public bool checkKGPoOrderpaymentnegative(PXCache sender, KGPoOrderPayment row)
        {
            foreach (KGPoOrderPayment kGPOOPaym in KGPoOrderPayments.Select())
            {
                if (kGPOOPaym.PaymentPct < 0)
                {
                    setCheckThread = true;

                    KGPoOrderPayments.Cache.RaiseExceptionHandling<KGPoOrderPayment.paymentPct>(
                         row, row.PaymentPct, new PXSetPropertyException(warningMsg));
                    return true;
                }                    
            }
            return false;
        }
        public bool checkPaymentPct(PXCache sender, KGPoOrderPayment row)
        {
            POOrder master = Base.Document.Current;
            if (master != null && master.ProjectID != null && POOrderType.RegularOrder.Equals(master.OrderType) && PXSiteMap.CurrentScreenID == "PO301000")
            {
                Contract contract = GetContract(master.ProjectID);
                if (contract != null && contract.NonProject != true)
                {
                    Decimal? sumPaymentPct = 0;
                    KGPoOrderPayment payments = null;
                    foreach (KGPoOrderPayment kGPOOPaym in KGPoOrderPayments.Cache.Cached)
                    {

                        sumPaymentPct = sumPaymentPct + getValue(kGPOOPaym.PaymentPct);
                        payments = kGPOOPaym;
                    }
                    if (sumPaymentPct != 100)
                    {
                       
                        if (payments == null)
                        {
                            throw new PXException("必須要有付款方式");

                        }
                        else
                        {
                            setCheckThread = true;
                            KGPoOrderPayments.Cache.RaiseExceptionHandling<KGPoOrderPayment.paymentPct>(
                                    row, row.PaymentPct, new PXSetPropertyException("付款方式加總要為100", PXErrorLevel.Error));
                            return true;
                        }

                            
                    }
                }
            }
            return false;
        }
        public Decimal? getValue(Decimal? value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value;
            }
        }
        public bool checkWarranty(PXCache sender, KGContractPricingRule row)
        {
            decimal? totalPaymPct = 0;
            PXEntryStatus status = KGContractPricingRules.Cache.GetStatus(row);
            /*if (status == PXEntryStatus.Updated)
            {*/
                totalPaymPct = row.WarrantyGuaranteeTicketRatio + row.WarrantyGuaranteeCashRatio;
            if (totalPaymPct != 100)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeCashRatio>(
                        row, row.WarrantyGuaranteeCashRatio, new PXSetPropertyException("繳交保固保證比例(%)加總需等於100(%)", PXErrorLevel.Error));
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeTicketRatio>(
                        row, row.WarrantyGuaranteeTicketRatio, new PXSetPropertyException("繳交保固保證比例(%)加總需等於100(%)", PXErrorLevel.Error));
                return true;
            }
            if (row.WarrantyRatioUntaxed > 100)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.warrantyRatioUntaxed>(
                            row, row.WarrantyRatioUntaxed, new PXSetPropertyException("依比率(未稅)(%)需小於等於100", PXErrorLevel.Error));
                setCheckThread = true;
            }
            //}
            return false;
        }

        public bool checkWarrantynegative(PXCache sender, KGContractPricingRule row)
        {
            POOrder order = POOrders.Current;
            if (row.WarrantyGuaranteeCashRatio < 0)
            {
                setCheckThread = true;

                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeCashRatio>(
                     row, row.WarrantyGuaranteeCashRatio, new PXSetPropertyException("繳交保固保證 保固票佔比 需大於等於0"));
                return true;
            }
            
            if (row.WarrantyGuaranteeTicketRatio < 0)
            {
                setCheckThread = true;

                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.warrantyGuaranteeTicketRatio>(
                     row, row.WarrantyGuaranteeTicketRatio, new PXSetPropertyException("繳交保固保證 現金佔比 需大於等於0"));
                return true;
            }

            if (row.WarrantyRatioUntaxed < 0)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.warrantyRatioUntaxed>(
                            row, row.WarrantyRatioUntaxed, new PXSetPropertyException("依比率(未稅)(%)需大於等於0", PXErrorLevel.Error));
                setCheckThread = true;
            }
            //2019/09/19 FixedAntUntaxed拿掉了
            /*if (row.FixedAmtUntaxed >order.CuryLineTotal)
            {
                setCheckThread = true;

                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.fixedAmtUntaxed>(
                     row, row.FixedAmtUntaxed, new PXSetPropertyException("保固款計算方式-固定金額(未稅)不可大於行總計",PXErrorLevel.Error));
                return true;
            }*/
            return false;
        }

        //mark by louis for upgrade to 20.114.0020
        //2019/09/26 ADD 檢核法規遵循資料
        /**
        public bool checkComplianceDocument(PXCache sender, KGContractPricingRule row)
        {
            POOrder order = POOrders.Current;
            ComplianceDocument compliance =
                PXSelectJoin<ComplianceDocument,
                InnerJoin<ComplianceDocumentReference, 
                On<ComplianceDocumentReference.complianceDocumentReferenceId,
                Equal<ComplianceDocument.purchaseOrder>>,
                InnerJoin<CSAnswers,On<CSAnswers.refNoteID,
                Equal<ComplianceDocument.noteId>>>>,
                Where<ComplianceDocumentReference.referenceNumber,Equal<Required<POOrder.orderNbr>>,
                And<ComplianceDocumentReference.type,Equal<Required<POOrder.orderType>>>>>
                .Select(Base,order.OrderNbr,order.OrderType);
            if (compliance == null)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.isPerformanceGuarantee>(
                            row, row.IsPerformanceGuarantee, new PXSetPropertyException("請維護法規遵循!", PXErrorLevel.Error));
            }
           
            return false;
        }
        **/

        //2019/09/19 ADD 檢核預付款
        public bool checkPrepayment(PXCache sender, KGContractPricingRule row)
        {
            //檢核預付款金額不可大於行總計檢核
            //2019/10/07改為唯讀不須檢核
            //checkPrepaymentsAmtUntaxed(sender, row, row.PrepaymentsAmtUntaxed);
            if (row.DeductionRatioPeriod > 100)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.deductionRatioPeriod>(
                            row, row.DeductionRatioPeriod, new PXSetPropertyException("每期扣回比率(%)需小於等於100", PXErrorLevel.Error));
                return true;
            }
            if (row.DeductionRatioPeriod < 0)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.deductionRatioPeriod>(
                            row, row.DeductionRatioPeriod, new PXSetPropertyException("每期扣回比率(%)需大於等於0", PXErrorLevel.Error));
                return true;
            }
            if (row.PrepaymentRatioUntaxed < 0)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.prepaymentRatioUntaxed>(
                            row, row.PrepaymentRatioUntaxed, new PXSetPropertyException("預付款比率(未稅)(%)需大於等於0", PXErrorLevel.Error));
                return true;
            }
            if (row.PrepaymentRatioUntaxed > 100)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.prepaymentRatioUntaxed>(
                            row, row.PrepaymentRatioUntaxed, new PXSetPropertyException("預付款比率(未稅)(%)需小於等於100", PXErrorLevel.Error));
                return true;
            }
            return false;
        }

        //2019/09/19 ADD 檢核保留款
        public bool checkDefRetainage(PXCache sender, KGContractPricingRule row)
        {
            if (row.DefRetainagePct > 100)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.defRetainagePct>(
                            row, row.DefRetainagePct, new PXSetPropertyException("保留款百分比(%)需小於等於100", PXErrorLevel.Error));
                return true;
            }
            if (row.DefRetainagePct < 0)
            {
                setCheckThread = true;
                KGContractPricingRules.Cache.RaiseExceptionHandling<KGContractPricingRule.defRetainagePct>(
                            row, row.DefRetainagePct, new PXSetPropertyException("保留款百分比(%)需大於等於0", PXErrorLevel.Error));
                return true;
            }
            return false;
        }

        /// <param name="graph"> POOrderEnty </param>
        protected virtual void ResetStandardRetainageApply(PXGraph graph, bool createAP)
        {
            PXCache cache_POOrd = graph.Caches[typeof(POOrder)];
            POOrder order = cache_POOrd.Current as POOrder;

            if (order?.RetainageApply == true)
            {
                var pricingRule = GetKGContractPricingRule(order);

                if (pricingRule?.RetainageApply != true)
                {
                    pricingRule.RetainageApply  = order.RetainageApply;
                    pricingRule.DefRetainagePct = order.DefRetainagePct;

                    graph.Caches[typeof(KGContractPricingRule)].Update(pricingRule);
                }

                cache_POOrd.SetValueExt<POOrder.retainageApply>(order, false);
                cache_POOrd.MarkUpdated(order);

                if (createAP == true)
                {
                    graph.Actions.PressSave();
                }
            }
        }
        #endregion

        #region Cache Override
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(DBRetainagePercentAttribute))]
        [CustDBRetainagePercent(
             typeof(POOrder.retainageApply),
            typeof(POOrder.defRetainagePct),
             typeof(Sub<Current<POLine.curyLineAmt>, Current<POLine.curyDiscAmt>>),
             typeof(POLine.curyRetainageAmt),
            typeof(POLine.retainagePct))]      
        
        protected virtual void POLine_RetainagePct_CacheAttached(PXCache sender) { }

        public class CustDBRetainagePercentAttribute : RetainagePercentAttribute
        {
            //[PXOverride]
            public CustDBRetainagePercentAttribute(
                Type retainageApplyField,
                Type defRetainagePctField,
                Type retainedAmtFormula,
                Type curyRetainageAmtField,
                Type retainagePctField)
                : base(retainageApplyField, defRetainagePctField, retainedAmtFormula, curyRetainageAmtField, retainagePctField)
            {
                _Attributes.Add(new PXDBDecimalAttribute(0) { MinValue = 0, MaxValue = 100 });
                _Attributes.Add(new PXDefaultAttribute(TypeCode.Decimal, "0", defaultType) { PersistingCheck = PXPersistingCheck.Nothing });
                _Attributes.Add(new PXUIVerifyAttribute(verifyType, PXErrorLevel.Error, AP.Messages.IncorrectRetainagePercent));
                _Attributes.Add(new UndefaultFormulaAttribute(formulaType));
            }
        }


        #endregion

        #region Delegate Method
        /// <summary>
        /// YJ's request
        /// </summary>
        /// <returns></returns>
        public delegate bool NeedsAPInvoiceDelegate();
        [PXOverride]
        public virtual bool NeedsAPInvoice(NeedsAPInvoiceDelegate baseMethod)
        {
            foreach (POLine insertedLine in Base.Transactions.Cache.Inserted)
            {
                if (Base.NeedsAPInvoice(insertedLine, true, Base.POSetup.Current))
                {
                    return true;
                }
            }

            foreach (POLine persistedLine in PXSelectReadonly2<POLine,
                                                               LeftJoin<APTran, On<APTran.pOAccrualRefNoteID, Equal<POLine.orderNoteID>,
                                                                                   And<APTran.pOAccrualLineNbr, Equal<POLine.lineNbr>,
                                                                                       And<APTran.released, Equal<False>>>>>,
                                                               Where<POLine.orderType, Equal<Current<POOrder.orderType>>,
                                                                     And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>,
                                                                         And<POLine.pOAccrualType, Equal<POAccrualType.order>>>>>
                                                                .Select(Base))
            {
                if (Base.NeedsAPInvoice(persistedLine, true, Base.POSetup.Current) && Base.Transactions.Cache.GetStatus(persistedLine) != PXEntryStatus.Deleted)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        //Codding by Jerry 2019/05/10
        //POOrderEntry 1447
        public virtual IEnumerable CreateAPInvoice(PXAdapter adapter)
        {
            POOrder poOrder = Base.Document.Current;

            if (poOrder != null)
            {
                if (poOrder.RetainageApply == true)
                {
                    poOrder.DefRetainagePct = poOrder.DefRetainagePct;
                    foreach (POLine poline in Base.Transactions.Select())
                    {
                        if (poOrder.DefRetainagePct != poline.RetainagePct)
                        {
                            poline.RetainagePct = poOrder.DefRetainagePct;
                            Base.Transactions.Update(poline);
                        }
                    }
                }
                else
                {
                    poOrder.DefRetainagePct = 0;
                    foreach (POLine poline in Base.Transactions.Select())
                    {
                        poline.RetainagePct = 0;
                        Base.Transactions.Update(poline);
                    }
                }

                ResetStandardRetainageApply(Base, true);
            }

            if (APInvoiceEntry_Extension.ckeckPrevStatusIsOpen(poOrder.VendorID, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), poOrder.ProjectID, null, poOrder.OrderNbr) == false)
            {
                throw new PXException("前面幾期尚未結束");
            }

            return Base.CreateAPInvoice(adapter);
        }

        //Codding by Jerry 2019/05/10
        //POOrderEntry 1132
        /*
        [PXUIField(DisplayName = "Create Prepayment", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXButton(OnClosingPopup = PXSpecialButtonType.Cancel)]
        public virtual void CreatePrepayment()
        {
            POOrder order = Base.Document.Current;
            if (Base.Document.Current != null)
            {
                
                Base.Save.Press();
                APInvoiceEntry target = PXGraph.CreateInstance<APInvoiceEntry>();
                Decimal? totalAmt = 0;
                
                APInvoiceEntry_Extension apInvoiceEntryExt = target.GetExtension<APInvoiceEntry_Extension>();

                if (Base.Document.Current.PrepaymentRefNbr == null)
                {
                    APInvoice aPInvoice =        target.Document.Insert(new APInvoice { DocType = APDocType.Prepayment, ProjectID = Base.Document.Current.ProjectID});
                    target.Document.Current = aPInvoice;
                    Base.CreatePrePaymentProc(target);

                    APTaxTran apTaxTranMaster = target.Taxes.Select();
                    KGContractPricingRule kgContractPricingRule = KGContractPricingRules.Select();
                    if (kgContractPricingRule.PrepaymentsAmtUntaxed == null)
                    {
                        kgContractPricingRule.PrepaymentsAmtUntaxed = 0;
                    }
                    if (kgContractPricingRule != null)
                    {
                        foreach (APTaxTran apTaxTran in target.Taxes.Select())
                        {

                            target.Taxes.Cache.SetValueExt<APTaxTran.curyTaxableAmt>(apTaxTran, kgContractPricingRule.PrepaymentsAmtUntaxed);
                            target.Taxes.Cache.SetValueExt<APTaxTran.curyTaxAmt>(apTaxTran, apTaxTran.CuryTaxableAmt * (apTaxTran.TaxRate / 100));
                        } 
                        //apTaxTran.CuryTaxableAmt = kgContractPricingRule.PrepaymentsAmtUntaxed;
                        //apTaxTran.CuryTaxAmt = apTaxTran.CuryTaxableAmt * (apTaxTran.TaxRate / 100);
                        //target.Taxes.Cache.Update(apTaxTran);
                    }
                    
                    int i = 0;
                    foreach (APTran line in target.Transactions.Select())
                    {
                        if (kgContractPricingRule == null)
                        {
                            line.CuryLineAmt = 0;
                        }
                        else
                        {
                            line.Qty = 1;
                            line.CuryUnitCost = kgContractPricingRule.PrepaymentsAmtUntaxed;
                            line.CuryLineAmt = kgContractPricingRule.PrepaymentsAmtUntaxed;
                            line.CuryTranAmt = kgContractPricingRule.PrepaymentsAmtUntaxed;
                            totalAmt = kgContractPricingRule.PrepaymentsAmtUntaxed;
                            target.Transactions.Cache.SetValue<APTranExt.usrValuationType>(line, "P");
                            
                        }
                        target.Transactions.Update(line);
                        i = i + 1;
                    }
                    int j = 0;
                    foreach (APTran line in target.Transactions.Select())
                    {
                        if (j != 0)
                        {
                            target.Transactions.Delete(line);
                        }
                        j = j + 1;
                    }
                    //APRegisterExt arRegisterExt = PXCache<APInvoice>.GetExtension<APRegisterExt>(target.Document.Current);
                    if (kgContractPricingRule != null) {
                        target.Document.Current.CuryTaxTotal = apTaxTranMaster.CuryTaxAmt;
                        target.Document.Update(target.Document.Current);
                    }
                    target.Document.Cache.SetValue<APRegisterExt.usrValuationPhase>(target.Document.Current, 0);
                    //20190722追加儲存PONbr
                    target.Document.Cache.SetValueExt<APRegisterExt.usrPONbr>(target.Document.Current, order.OrderNbr);
                    target.Document.Cache.SetValueExt<APRegisterExt.usrPOOrderType>(target.Document.Current, order.OrderType);
                    apInvoiceEntryExt.setValuationSummaryByPrepayment();

                    //新增一筆KGBillPayment
                    KGBillPayment billPayment = (KGBillPayment)apInvoiceEntryExt.KGBillPaym.Cache.CreateInstance();
                    //一般計價
                    billPayment.PricingType = "A";
                    //支票
                    billPayment.PaymentMethod = "A";
                    billPayment.PaymentPeriod = 0;
                    billPayment.PaymentPct = 100;
                    billPayment.PaymentDate = aPInvoice.DueDate;
                    billPayment.PaymentAmount=totalAmt ;
                    apInvoiceEntryExt.KGBillPaym.Insert(billPayment);

                    throw new PXPopupRedirectException(target, "New Prepayment", true);
                
                }
                else
                {
                    throw new InvalidOperationException(PXMessages.LocalizeNoPrefix(PX.Objects.PO.Messages.PrepaymentAlreadyExists));
                }
            }
        }*/
        [PXUIField(DisplayName = "Create Prepayment", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXButton(OnClosingPopup = PXSpecialButtonType.Cancel)]
        public virtual void CreatePrepayment()
        {
            POOrder order = Base.Document.Current;
            if (Base.Document.Current != null)
            {
                Base.Save.Press();
                var target = PXGraph.CreateInstance<APInvoiceEntry>();
                var prepaymentExt = target.GetExtension<PX.Objects.PO.GraphExtensions.APInvoiceEntryExt.Prepayments>();
                //modify by louis for upgrade to 20.114.0020
                //prepaymentExt.AddPOOrder(Base.Document.Current, true);
                prepaymentExt.AddPOOrderProc(Base.Document.Current, true);
                APInvoice aPInvoice = target.Document.Current;
                Decimal? totalAmt = 0;
                APInvoiceEntry_Extension apInvoiceEntryExt = target.GetExtension<APInvoiceEntry_Extension>();
                APTaxTran apTaxTranMaster = target.Taxes.Select();
                KGContractPricingRule kgContractPricingRule = KGContractPricingRules.Select();
                if (kgContractPricingRule.PrepaymentsAmtUntaxed == null)
                {
                    kgContractPricingRule.PrepaymentsAmtUntaxed = 0;
                }
                //新增一筆KGBillPayment 注意要放在APTran之前
                KGBillPayment billPayment = (KGBillPayment)apInvoiceEntryExt.KGBillPaym.Cache.CreateInstance();
                //一般計價
                billPayment.PricingType = "A";
                //支票
                billPayment.PaymentMethod = "A";
                billPayment.PaymentPeriod = 0;
                billPayment.PaymentPct = 100;
                billPayment.PaymentDate = aPInvoice.DueDate;
                billPayment.PaymentAmount = totalAmt;
                BAccount bAccount = GetBAccount(order.VendorID);
                billPayment.VendorLocationID = VendorLocationID(order.VendorID,billPayment.PaymentMethod);
                if (bAccount != null)
                {
                    billPayment.CheckTitle = bAccount.AcctName;
                }
                apInvoiceEntryExt.KGBillPaym.Insert(billPayment);
                //
                foreach (APTran line in target.Transactions.Select())
                {
                    target.Transactions.Cache.SetValue<APTranExt.usrValuationType>(line, "P");
                    //12117: PO301000 分契約建立預付款調整
                    target.Transactions.Cache.SetValue<APTran.retainagePct>(line, 0m);
                    //不知道為啥沒有ProjectID
                    //line.ProjectID = aPInvoice.ProjectID;
                    target.Transactions.Update(line);
                }
                POOrderExt poorderext = PXCache<POOrder>.GetExtension<POOrderExt>(order);

                target.Document.Cache.SetValue<APRegisterExt.usrValuationPhase>(target.Document.Current, 0);
                //20190722追加儲存PONbr
                target.Document.Cache.SetValueExt<APRegisterExt.usrPONbr>(target.Document.Current, order.OrderNbr);
                target.Document.Cache.SetValueExt<APRegisterExt.usrPOOrderType>(target.Document.Current, order.OrderType);
                target.Document.Cache.SetValueExt<APRegisterExt.usrBillCategory>(target.Document.Current, poorderext.UsrBillCategory);
                //apInvoiceEntryExt.setValuationSummaryByPrepayment();
                /*---2021-08-02:12179 
                apInvoiceEntryExt.setPayment();
                */
                throw new PXPopupRedirectException(target, "New Prepayment", true);
            }
        }
        private BAccount GetBAccount(int? VendorID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, VendorID);
        }

        //2020/12/24 Althea Add 供FIN override 塞VendorLocationID的值到預付款
        public virtual int? VendorLocationID(int? BAccount,string PaymentMethod)
        {
            return null;
        }
    }
    [Serializable]
    public class ReportFilterPOOrder : IBqlTable
    {

        #region OrderType
        [PXString()]
        [PXUIField(DisplayName = "OrderType")]      
        public virtual string OrderType { get; set; }
        public abstract class orderType : IBqlField { }
        #endregion

        #region RunningType
        [PXString()]
        [PXUIField(DisplayName = "RunningType")]
        [PXStringList(new string[] { "AUTO","MANUAL"},
            new string[] { "AUTO", "MANUAL"})]
        public virtual string RunningType { get; set; }
        public abstract class runningType : IBqlField { }
        #endregion

        #region CompletionDate
        [PXDate()]
        [PXUIField(DisplayName = "CompletionDate")]
        public virtual DateTime? CompletionDate { get; set; }
        public abstract class completionDate : IBqlField { }
        #endregion

        #region OrderNbr
        [PXString(15, IsUnicode = true)]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion
    }

    public static class DateTimeExtensions
    {
        /// <summary>
        /// To the full taiwan date.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToFullTaiwanDate(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return string.Format("{0} 年 {1} 月 {2} 日",
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }

        /// <summary>
        /// To the simple taiwan date.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToSimpleTaiwanDate(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return string.Format("{0}/{1}/{2}",
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }
        public static string To8VarCharTaiwanDate(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return string.Format("{0}{1}{2}",
                taiwanCalendar.GetYear(datetime),
                datetime.ToString("MM"),
                datetime.ToString("dd"));
        }
    }
}