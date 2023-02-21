using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.EP;
using System.Collections;
using PX.Objects.IN;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.TM;
using PX.Objects.PO;
using PX.Objects.RQ;
using PX.Objects.CM;
using Kedge.DAC;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.SM;
using System.Web;
using PX.Objects.CR;


//add InsertPOLineDelegate() new parameter for 2020R1 upgrade (modify by louis 20210310)



namespace PX.Objects.RQ
{
    public class RQRequisitionEntry_Extension : PXGraphExtension<RQRequisitionEntry>
    {
        public string PpmUID;

        public readonly string warningMsg = "Payment Percentage Can't Be Negative !";
        public readonly string errorMsg = "付款比例未達100%, 請確認";
        public readonly string checkErrMsg = "The Requisition Hasn't Been Approved From AgentFlow.";
        /*
        [PXDefault(1)]
        [PXInt]
        protected void RQRequisitionLine_Processflag_CacheAttached(PXCache sender)
        { }*/
        #region Select 
        public PXSelect<KGRequisitionDoc,
                        Where<KGRequisitionDoc.reqNbr, Equal<Current<RQRequisition.reqNbr>>>> KGRequistionDoc;

        public PXSelect<KGRequistionPayment,
                        Where<KGRequistionPayment.reqNbr, Equal<Current<RQRequisition.reqNbr>>>> KGReqPayment;

        public PXSelect<KGRequisitionFile,
                        Where<KGRequisitionFile.orderNbr, Equal<Current<RQRequisition.reqNbr>>>> KGReqsFile;

        public PXSelect<KGFlowPurchase, Where<KGFlowPurchase.purchaseProgramNo, Equal<Current<RQRequisition.reqNbr>>>> KGFlowPurch;

        public PXSelect<KGFlowPurchaseBidDocDetail> KGFlowPurchBidDocDetl;

        public PXSelect<KGFlowPurchaseRequisitionDetail> KGFlowPurchReqsDetl;

        public PXSelect<KGFlowUploadFile> KGFlowUploadFile;
        #endregion

        #region Button


        public PXAction<RQRequestLine> webServIntegration;
        [PXUIField(DisplayName = "AgentFlow", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable WebServIntegration(PXAdapter adapter)
        {
            if (getKGRequisitionDoc() == null)
            {
                throw new PXException("契約資訊中的計價條件需要填寫");
            }
            if (KGReqPayment.Select().Count == 0)
            {
                throw new PXException("契約資訊中的付款條件需要填寫");
            }
            decimal? totalPaymPct = 0;
            foreach (KGRequistionPayment kGReqPaym in KGReqPayment.Select())
            {
                if (kGReqPaym.PaymentPct < 0m)
                {
                    KGReqPayment.Cache.RaiseExceptionHandling<KGRequistionPayment.paymentPct>(kGReqPaym,
                                                                                              null,
                                                                                              new PXSetPropertyException<KGRequistionPayment.paymentPct>
                                                                                                  (warningMsg, PXErrorLevel.Warning));
                    return adapter.Get();
                }

                totalPaymPct += kGReqPaym.PaymentPct;
            }

            if (totalPaymPct != 100m)
            {
                throw new PXException(errorMsg);
            }


            //if (Base.Document.Current.Status == Messages.PendingApproval)
            //{
            //using (PXTransactionScope ts = new PXTransactionScope())
            //{

            //刪除已存在資料
            foreach (KGFlowPurchase kgFlowPurchase in KGFlowPurch.Select())
            {
                PXResultset<KGFlowPurchaseBidDocDetail> set = PXSelect<KGFlowPurchaseBidDocDetail,
                                Where<KGFlowPurchaseBidDocDetail.ppmUID, Equal<Required<KGFlowPurchase.ppmUID>>>>.Select(Base, kgFlowPurchase.PpmUID);

                foreach (KGFlowPurchaseBidDocDetail kgFlowPurchaseBidDocDetail in set)
                {
                    KGFlowPurchBidDocDetl.Delete(kgFlowPurchaseBidDocDetail);
                }
                PXResultset<KGFlowPurchaseRequisitionDetail> set2 = PXSelect<KGFlowPurchaseRequisitionDetail,
                               Where<KGFlowPurchaseRequisitionDetail.ppmUID, Equal<Required<KGFlowPurchase.ppmUID>>>>.Select(Base, kgFlowPurchase.PpmUID);
                foreach (KGFlowPurchaseRequisitionDetail kgFlowPurchaseRequisitionDetail in set2)
                {
                    KGFlowPurchReqsDetl.Delete(kgFlowPurchaseRequisitionDetail);
                }
                KGFlowPurch.Delete(kgFlowPurchase);
            }
            //後面需要用到ApproveID，而ApproveID需要用到NoteID因此需要先保存
            Base.Save.Press();

            this.InsertIntoKGFlowPurch();
            this.InsertIntoKGFlowPurchBidDocDetl();
            this.InsertIntoKGFlowPurchReqsDetl();
            this.CreateKGUploadFile();
            RQRequisition requisition = Base.CurrentDocument.Current;
            requisition.GetExtension<RQRequisitionExt>().UsrApprovalStatus = "PA";

            Base.Save.Press();
            this.ConnectAgentFlowWeb();
            Base.Save.Press();
            // ts.Complete();
            //}

            //}

            return adapter.Get();
        }

        public PXAction<RQRequestLine> resetStatus;
        [PXUIField(DisplayName = "Reset Status", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable ResetStatus(PXAdapter adapter)
        {
            RQRequisition master = Base.Document.Current;
            if (RQRequisitionStatus.Released.Equals(master.Status) && Base.POOrders.Select().Count == 0)
            {
                Base.Save.Press();
                master.Status = RQRequisitionStatus.Open;
                master.Released = false;
                Base.Document.Update(master);
                Base.Save.Press();
            }
            return adapter.Get();
        }
        #endregion

        #region Action
        public PXAction<APInvoice> ViewOriginalDocument;

        [PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        protected virtual IEnumerable viewOriginalDocument(PXAdapter adapter)
        {
            //參考資料
            //https://asiablog.acumatica.com/2017/08/redirecting-to-external-page-from-button.html
            RQRequisition master = Base.Document.Current;
            RQRequisitionExt rqRequestExt = PXCache<RQRequisition>.GetExtension<RQRequisitionExt>(master);
            throw new Exception("Redirect7:" + rqRequestExt.UsrReturnUrl);
            return adapter.Get();
        }
        #endregion
        #region Event

        
        protected virtual void RQRequisition_Status_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }
            RQRequisition master = (RQRequisition)e.Row;
            RQRequisitionExt requisitionExt = PXCache<RQRequisition>.GetExtension<RQRequisitionExt>(master);
            //bool? hold = (bool?)e.NewValue;
            //bool? hold = master.Status;
            if (master.Status.Equals(RQRequisitionStatus.Open))
            {
                if ("B".Equals(KGRequistionDoc.Current?.RequisitionClass))
                {
                    requisitionExt.UsrApprovalStatus = ApprovalStatusListAttribute.Approved;
                }

            }
            
        }
        #endregion

        #region Override-Function
        public override void Initialize()
        {
            base.Initialize();

            Base.action.AddMenuAction(this.webServIntegration);
            Base.action.AddMenuAction(resetStatus);
        }
        //add new parameter for 2020R1 upgrage by louis 20210310
        public delegate POLine InsertPOLineDelegate(POOrderEntry graph, RQRequisitionLine line, decimal? qty, decimal? unitCost, IBqlTable costOriginDac, string lineType, DateTime? bidPromisedDate);
        [PXOverride]
        public virtual POLine InsertPOLine(POOrderEntry graph, RQRequisitionLine line, decimal? qty, decimal? unitCost, IBqlTable costOriginDac, string lineType, DateTime? bidPromisedDate, InsertPOLineDelegate baseMethod)
        {
            POLine poLine = baseMethod(graph, line, qty, unitCost, costOriginDac, lineType, bidPromisedDate);
            RQRequisitionExt requisitionExt = PXCache<RQRequisition>.GetExtension<RQRequisitionExt>(Base.Document.Current);
            RQRequisitionLineExt requisitionLineExt = PXCache<RQRequisitionLine>.GetExtension<RQRequisitionLineExt>(line);
            //graph.Document.Current.ProjectID = requisitionExt.UsrProjectID;

            if (poLine == null || requisitionLineExt == null)
            { }
            else
            {
                poLine.ProjectID = requisitionLineExt.UsrContractID;
                poLine.TaskID = requisitionLineExt.UsrProjectTaskID;
                poLine.CostCodeID = requisitionLineExt.UsrCostCodeID;
                POOrderEntry_Extension pOOrderEntry_Ext = graph.GetExtension<POOrderEntry_Extension>();
                if (pOOrderEntry_Ext.KGContractDocs.Select().Count == 0)
                {
                    CreateKGContractDoc(pOOrderEntry_Ext);

                    RQSetup rqSetup = PXSelect<RQSetup>.Select(Base);
                    if (rqSetup.RequisitionApproval == true)
                    {
                        graph.Document.Current.Hold = true;
                        graph.Document.Current.Status = POOrderStatus.Hold;
                        graph.Document.Update(graph.Document.Current);
                    }

                    //請注意前提是KGContractDoc資料一定要有
                    //mark by louis 20190717
                    //pOOrderEntry_Ext.ImportSampleTags.Press();
                }


                //poLine.CostCodeID = requisitionLineExt.UsrCostCodeID;
                try
                {
                    poLine = graph.Transactions.Update(poLine);
                }
                catch (Exception e)
                {

                    String s = e.StackTrace;
                    throw e;

                }
                graph.Document.Current.ProjectID = poLine.ProjectID;
                graph.Document.Update(graph.Document.Current);
                graph.Transactions.Cache.SetValueExt<POLine.taskID>(poLine, requisitionLineExt.UsrProjectTaskID);
                graph.Transactions.Cache.SetValueExt<POLine.costCodeID>(poLine, requisitionLineExt.UsrCostCodeID);
                //poLine.TaskID = requisitionLineExt.UsrProjectTaskID;
                //poLine.CostCodeID = requisitionLineExt.UsrCostCodeID;

                CreateKGPOPayment(pOOrderEntry_Ext);
                CreateKGContractPrcRule(pOOrderEntry_Ext);

                //Add By Althea 2019/10/16
                KGRequisitionDoc kgRequistionDoc = getKGRequisitionDoc();
                if (kgRequistionDoc.DefRetainagePct == null || kgRequistionDoc.DefRetainagePct == 0)
                {
                    poLine.RetainagePct = 0;
                }
                else
                {
                    poLine.RetainagePct = kgRequistionDoc.DefRetainagePct;
                    graph.Transactions.Update(poLine);
                }

            }

            return poLine;
        }

        [PXOverride]
        public virtual RQRequisitionLine CreateNewRequisitionLineFromRequestLine(RQRequest request, RQRequestLine requestLine)
        {
            RQRequisitionLine requisitionLine = new RQRequisitionLine
            {
                ReqNbr = Base.Document.Current.ReqNbr,
                InventoryID = requestLine.InventoryID,
                SubItemID = requestLine.SubItemID,
                Description = requestLine.Description,
                UOM = requestLine.UOM,
                OrderQty = 0m,
                ManualPrice = requestLine.ManualPrice,
                ExpenseAcctID = requestLine.ExpenseAcctID,
                ExpenseSubID = requestLine.ExpenseSubID,
                RequestedDate = requestLine.RequestedDate,
                PromisedDate = requestLine.PromisedDate,

                ByRequest = true
            };

            RQRequestLineExt requestLineExt = PXCache<RQRequestLine>.GetExtension<RQRequestLineExt>(requestLine);
            RQRequisitionLineExt requisitionLineExt = PXCache<RQRequisitionLine>.GetExtension<RQRequisitionLineExt>(requisitionLine);
            //requisitionLineExt.Processflag = 0;
            requisitionLineExt.UsrContractID = requestLineExt.UsrContractID;
            requisitionLineExt.UsrProjectTaskID = requestLineExt.UsrProjectTaskID;
            requisitionLineExt.UsrCostCodeID = requestLineExt.UsrCostCodeID;
            requisitionLineExt.UsrAccountGroupID = requestLineExt.UsrAccountGroupID;
            if (Base.Document.Current.CuryID == request.CuryID)
            {
                requisitionLine.CuryEstUnitCost = requestLine.CuryEstUnitCost;
            }
            else
            {
                decimal unitCost;
                PXCurrencyAttribute.CuryConvCury<RQRequisitionLine.curyInfoID>(Base.Lines.Cache, requestLine, requestLine.EstUnitCost.GetValueOrDefault(), out unitCost);
                requisitionLine.CuryEstUnitCost = unitCost;
            }
            //mark by louis 20200521 for not using
            /*
            if (Base.Lines.Current != null && Base.Lines.Current.LineNbr == 1) { 
                this.InitKGReqsDoc();
            }*/

            return requisitionLine;
        }

        public delegate IEnumerable CreatePOOrderDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable CreatePOOrder(PXAdapter adapter, CreatePOOrderDelegate baseMethod)
        {
            CheckApprovalStatus();
            VerifyKGReqDoc(this.KGRequistionDoc.Select());
            return baseMethod(adapter);
        }
        #endregion

        #region Function
        /// <summary>
        /// Verify the data and fields must be filled.
        /// </summary>
        /// <param name="kGReqDoc"></param>
        private void VerifyKGReqDoc(KGRequisitionDoc kGReqDoc)
        {
            if (kGReqDoc == null)
            {
                throw new PXException("Requisition information must be filled.");
            }

            if (kGReqDoc.DefRetainagePct == null)
            {
                this.KGRequistionDoc.Cache.RaiseExceptionHandling<KGRequisitionDoc.defRetainagePct>(kGReqDoc,
                                                                                                    kGReqDoc.DefRetainagePct,
                                                                                                    new PXSetPropertyException<KGRequisitionDoc.defRetainagePct>
                                                                                                    (MyMessages.MandatoryField, PXErrorLevel.Error));
            }
            if (kGReqDoc.PaymentCalculateMethod == null)
            {
                this.KGRequistionDoc.Cache.RaiseExceptionHandling<KGRequisitionDoc.paymentCalculateMethod>(kGReqDoc,
                                                                                                           kGReqDoc.PaymentCalculateMethod,
                                                                                                           new PXSetPropertyException<KGRequisitionDoc.paymentCalculateMethod>
                                                                                                           (MyMessages.MandatoryField, PXErrorLevel.Error));
            }
            if (kGReqDoc.PerformanceGuaranteePct == null)
            {
                this.KGRequistionDoc.Cache.RaiseExceptionHandling<KGRequisitionDoc.performanceGuaranteePct>(kGReqDoc,
                                                                                                            kGReqDoc.PerformanceGuaranteePct,
                                                                                                            new PXSetPropertyException<KGRequisitionDoc.performanceGuaranteePct>
                                                                                                            (MyMessages.MandatoryField, PXErrorLevel.Error));
            }
        }

        /// <summary>
        /// Chekc the custom field status to ensure the external flow has been approved.
        /// </summary>
        private void CheckApprovalStatus()
        {
            RQRequisition requisition = Base.CurrentDocument.Current;

            if (requisition.Hold != true && requisition.Status == RQRequisitionStatus.Open &&
                requisition.GetExtension<RQRequisitionExt>().UsrApprovalStatus != ApprovalStatusAttribute.Approved)
            {
                throw new PXException(checkErrMsg);
            }
        }

        /// <summary>
        /// Bring the default value by request class ID.
        /// </summary>
        protected void InitKGReqsDoc()
        {
            KGRequisitionDoc requisitionDoc = KGRequistionDoc.Cache.CreateInstance() as KGRequisitionDoc;

            KGRequistionDoc.Cache.Insert(requisitionDoc);
        }

        // Not used for the time being.
        //protected void InitKGReqPayment()
        //{
        //    decimal? paymtPct = 0;

        //    KGRequistionPayment requistionPayment = KGReqPayment.Cache.CreateInstance() as KGRequistionPayment;

        //    for (int i = 1; i <= 2; i++)
        //    {
        //        requistionPayment.PricingType   = PricingType.A;
        //        requistionPayment.PaymentMethod = PaymentMethod.A;
        //        requistionPayment.PaymentPeriod = 0;
        //        requistionPayment.PaymentPct    = 100 - paymtPct;

        //        paymtPct = requistionPayment.PaymentPct;

        //        KGReqPayment.Cache.Insert(requistionPayment);
        //    }
        //}

        /// <summary>
        /// Use POOrderEntry extension to provide reference data for the relational table when creating PO order.
        /// </summary>
        /// <param name="graphExt"></param>
        protected void CreateKGContractDoc(POOrderEntry_Extension graphExt)
        {

            KGContractDoc kGContractDoc = graphExt.KGContractDocs.Cache.CreateInstance() as KGContractDoc;
            KGRequisitionDoc kgRequistionDoc = getKGRequisitionDoc();
            KGContractCategory kGContractCategory = PXSelect<KGContractCategory, Where<KGContractCategory.contractType, Equal<ReqCategoryType.categoryType>>>.Select(Base);
            KGVendorEvaluation kGVendorEvaluation = PXSelect<KGVendorEvaluation, Where<KGVendorEvaluation.status, Equal<EvaStatus.evaStatus>>>.Select(Base);
            kGContractDoc.EvaluationID = kGVendorEvaluation.EvaluationID;
            kGContractDoc.ContractCategoryID = kGContractCategory.ContractCategoryID;
            //kGContractDoc.EvaluationScore = 90;
            kGContractDoc.ContractDocDate = Base.Accessinfo.BusinessDate;
            kGContractDoc.ActualStartDate = Base.Accessinfo.BusinessDate;
            kGContractDoc.ExpectDays = 0;
            kGContractDoc.RemainingDays = 0;
            //add by louis 20200522 copy RequisitionClass to OrderClass
            kGContractDoc.OrderClass = kgRequistionDoc.RequisitionClass;

            graphExt.KGContractDocs.Cache.Insert(kGContractDoc);
        }

        /// <summary>
        /// Use POOrderEntry extension to provide reference data for the relational table when creating PO order.
        /// </summary>
        /// <param name="graphExt"></param>
        protected void CreateKGPOPayment(POOrderEntry_Extension graphExt)
        {
            KGPoOrderPayment poOrderPayment = (KGPoOrderPayment)graphExt.KGPoOrderPayments.Cache.CreateInstance();

            if (graphExt.KGPoOrderPayments.Select().Count > 0) { return; }

            foreach (KGRequistionPayment kGReqPayment in KGReqPayment.Select())
            {
                poOrderPayment.PricingType = kGReqPayment.PricingType;
                poOrderPayment.PaymentMethod = kGReqPayment.PaymentMethod;
                poOrderPayment.PaymentPeriod = kGReqPayment.PaymentPeriod;
                poOrderPayment.PaymentPct = kGReqPayment.PaymentPct;

                graphExt.KGPoOrderPayments.Insert(poOrderPayment);
            }
        }

        public KGRequisitionDoc getKGRequisitionDoc()
        {
            KGRequisitionDoc kgRequistionDoc = KGRequistionDoc.Current;
            if (kgRequistionDoc == null)
            {
                kgRequistionDoc = PXSelect<KGRequisitionDoc,
                        Where<KGRequisitionDoc.reqNbr, Equal<Required<RQRequisition.reqNbr>>>>.Select(Base, Base.Document.Current.ReqNbr);
            }
            return kgRequistionDoc;
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
        /// <summary>
        /// Use POOrderEntry extension to get the default record and set other field values.
        /// </summary>
        /// <param name="graphExt"></param>
        protected void CreateKGContractPrcRule(POOrderEntry_Extension graphExt)
        {
            KGRequisitionDoc kgRequistionDoc = getKGRequisitionDoc();
            if (graphExt.KGContractPricingRules.Current == null || kgRequistionDoc == null) { return; }
            KGContractPricingRule contractPrcRule = graphExt.KGContractPricingRules.Current;
            contractPrcRule.DefRetainagePct = getValue(kgRequistionDoc.DefRetainagePct);
            contractPrcRule.RetainageApply = (contractPrcRule.DefRetainagePct > 0);
            contractPrcRule.WarrantyRatioUntaxed = getValue(kgRequistionDoc.PerformanceGuaranteePct);
            contractPrcRule.WarrantyYear = getValue(kgRequistionDoc.WarrantyYear);
        }

        /// <summary>
        /// Create function for store some information for external system (AgentFlow)
        /// </summary>
        protected void InsertIntoKGFlowPurch()
        {
            Boolean firstRec = true;

            KGFlowPurchase flowPurchase = KGFlowPurch.Cache.CreateInstance() as KGFlowPurchase;
            RQRequisition master = Base.Document.Current;
            EPApproval epApproval = PXSelect<EPApproval,
                                Where<EPApproval.refNoteID,
                                Equal<Required<EPApproval.refNoteID>>, And<EPApproval.status, Equal<Required<EPApproval.status>>>>>
                                .Select(Base, master.NoteID, EPApprovalStatus.Pending);
            if (epApproval != null)
            {
                flowPurchase.ApprovalID = epApproval.ApprovalID;
            }
            //KGFlowPurch.Update(kgFlowRequisitions);

            Contract contract = PXSelectReadonly<Contract,
                                                 Where<Contract.contractID, Equal<Required<Contract.contractID>>>>
                                                 .Select(Base, Base.Lines.Cache.GetExtension<RQRequisitionLineExt>(Base.Lines.Current).UsrContractID);
            //flowPurchase= (KGFlowPurchase)KGFlowPurch.Insert(flowPurchase);
            flowPurchase.PpmUID = AutoNumberAttribute.GetNextNumber(KGFlowPurch.Cache, flowPurchase, "KGFLOWPPM", Base.Accessinfo.BusinessDate);
            //flowPurchase.DeptName = Base.Accessinfo.CompanyName;
            PX.Objects.GL.Branch branch = PXSelect<PX.Objects.GL.Branch, Where<PX.Objects.GL.Branch.branchID, Equal<Required<RQRequisition.branchID>>>>.Select(Base, master.BranchID);
            //add by louis to get branch acctname for Dept Name
            if (branch != null)
            {
                flowPurchase.DeptName = branch.AcctName;
            }
            if (contract != null)
            {
                flowPurchase.ProjectName = contract.Description;
                flowPurchase.ProjectCode = contract.ContractCD;
            }
            KGRequisitionDoc kgRequistionDoc = getKGRequisitionDoc();
            //modify by louis change flowPurchase.createUserName to employyee full name
            BAccount bAccount = PXSelect<BAccount,
                    Where<BAccount.bAccountID,
                    Equal<Required<BAccount.bAccountID>>>>
                    .Select(Base, master.EmployeeID);
            flowPurchase.CreateUserName = bAccount.AcctName;
            flowPurchase.PurchaseProgramNo = Base.Document.Current.ReqNbr;
            flowPurchase.PurchaseProgramName = Base.Document.Current.Description;
            flowPurchase.Explain = kgRequistionDoc.Remark;
            flowPurchase.ResPercent = kgRequistionDoc.DefRetainagePct;
            flowPurchase.PayType = kgRequistionDoc.PaymentCalculateMethod;
            flowPurchase.PerformanceBondPercent = kgRequistionDoc.PerformanceGuaranteePct;
            flowPurchase.WarrantyYearsOf = kgRequistionDoc.WarrantyYear;

            foreach (PXResult<KGRequistionPayment> result in PXSelectReadonly<KGRequistionPayment,
                                                                              Where<KGRequistionPayment.reqNbr, Equal<Required<RQRequisition.reqNbr>>>>
                                                                              .Select(Base, Base.Document.Current.ReqNbr))
            {
                KGRequistionPayment requistionPayment = result;

                if (firstRec == true)
                {
                    flowPurchase.CostDay1 = requistionPayment.PaymentPeriod;
                    flowPurchase.CostRatio1 = requistionPayment.PaymentPct;

                    firstRec = false;
                }
                else
                {
                    flowPurchase.CostDay2 = requistionPayment.PaymentPeriod;
                    flowPurchase.CostRatio2 = requistionPayment.PaymentPct;
                }
            }

            flowPurchase = (KGFlowPurchase)KGFlowPurch.Update(flowPurchase);

            PpmUID = flowPurchase.PpmUID;
            //Base.Document.Cache.SetValueExt<RQRequisitionExt.usrKGFlowUID>(Base.Document.Current, PpmUID);
            Base.Document.Cache.Update(Base.Document.Current);
        }

        /// <summary>
        /// Create function for store some information for external system (AgentFlow)
        /// </summary>
        protected void InsertIntoKGFlowPurchBidDocDetl()
        {
            KGFlowPurchaseBidDocDetail flowPurchBidDocDetl = KGFlowPurchBidDocDetl.Cache.CreateInstance() as KGFlowPurchaseBidDocDetail;
            LocationExtAddress locationExtAddress = PXSelectReadonly<LocationExtAddress, Where<LocationExtAddress.bAccountID, Equal<Required<LocationExtAddress.bAccountID>>>>.Select(Base, Base.vendor.Current.BAccountID);
            flowPurchBidDocDetl.PpbUID = AutoNumberAttribute.GetNextNumber(KGFlowPurchBidDocDetl.Cache, flowPurchBidDocDetl, "KGFLOWPPB", Base.Accessinfo.BusinessDate);
            flowPurchBidDocDetl.PpmUID = PpmUID;
            if (locationExtAddress != null)
            {
                flowPurchBidDocDetl.InvNo = locationExtAddress.TaxRegistrationID;
            }

            /**
            if (Base.location.Current != null)
            {
                flowPurchBidDocDetl.InvNo = Base.location.Current.TaxRegistrationID;
            }*/
            if (Base.vendor.Current != null)
            {
                flowPurchBidDocDetl.CompanyName = Base.vendor.Current.AcctName;
            }
            //modify by louis for PORemitContact/PORemitAddress 抓錯bug 20200820
            PORemitContact vendorContract = Base.Remit_Contact.Select();
            if (vendorContract != null)
            {
                flowPurchBidDocDetl.ContactPhone = vendorContract.Phone1;
                flowPurchBidDocDetl.Contactor = vendorContract.Attention;
            }
            PORemitAddress vendorAddress = Base.Remit_Address.Select();
            if (vendorAddress != null)
            {
                flowPurchBidDocDetl.Adderss = vendorAddress.AddressLine1;
            }
            /**
            if (Base.Remit_Contact.Current != null)
            {
                flowPurchBidDocDetl.ContactPhone = Base.Remit_Contact.Current.Phone1;
                flowPurchBidDocDetl.Contactor = Base.Remit_Contact.Current.Attention;
            }
            if (Base.Remit_Address.Current != null)
            {
                flowPurchBidDocDetl.Adderss = Base.Remit_Address.Current.AddressLine1;
            }**/

            //Jerry新增20191008
            RQRequisition currentDocument = Base.CurrentDocument.Select();

            foreach (RQBiddingVendor vendor in Base.Vendors.Select())
            {
                if (currentDocument != null && vendor.VendorID.Equals(currentDocument.VendorID))
                {
                    flowPurchBidDocDetl.FinalAmt = Base.Vendors.Cache.GetExtension<RQBiddingVendorExt>(vendor).UsrVendQuoteCost;
                }

            }
            //RQBiddingVendor rqBiddingVendor = Base.Vendors.Select();


            //分子flowPurchBidDocDetl.FinalAmt
            Decimal? budTotalAmt = 0;
            foreach (RQRequisitionLine line in Base.Lines.Select())
            {
                #region mark 20200620-mantis:11618
                //mark 20200620-mantis:11618 by alton :改抓分包規劃
                //RQRequisitionLineExt rqRequisitionLineExt = PXCache<RQRequisitionLine>.GetExtension<RQRequisitionLineExt>(line);
                //if (rqRequisitionLineExt.UsrCostCodeID != null && rqRequisitionLineExt.UsrProjectTaskID != null && rqRequisitionLineExt.UsrContractID != null && line.InventoryID != null && rqRequisitionLineExt.UsrAccountGroupID != null)
                //{
                //    PMCostBudget pmCost = RQRequestEntry_Extension.getPMCostBudget(rqRequisitionLineExt.UsrCostCodeID.Value, rqRequisitionLineExt.UsrProjectTaskID.Value,
                //                                                              rqRequisitionLineExt.UsrContractID.Value, line.InventoryID.Value,
                //                                                              rqRequisitionLineExt.UsrAccountGroupID.Value);
                //    if (pmCost != null)
                //    {
                //        budTotalAmt = budTotalAmt + getValue(pmCost.RevisedAmount);
                //    }

                //}
                #endregion
                #region add 20200620-mantis:11618
                PXResultset<RQRequestLine> rs = PXSelectJoin<RQRequestLine,
                    InnerJoin<RQRequisitionContent,
                    On<RQRequisitionContent.orderNbr, Equal<RQRequestLine.orderNbr>,
                    And<RQRequisitionContent.lineNbr, Equal<RQRequestLine.lineNbr>>>>,
                    Where<RQRequisitionContent.reqNbr, Equal<Required<RQRequisitionContent.reqNbr>>,
                    And<RQRequisitionContent.reqLineNbr, Equal<Required<RQRequisitionContent.reqLineNbr>>>>>
                    .Select(Base, line.ReqNbr, line.LineNbr);
                foreach (RQRequestLine oLine in rs)
                {
                    budTotalAmt += (oLine.CuryEstExtCost ?? 0);
                }
                #endregion

            }
            flowPurchBidDocDetl.BudTotalAmt = budTotalAmt;
            if (budTotalAmt == 0 || flowPurchBidDocDetl.FinalAmt == null || flowPurchBidDocDetl.FinalAmt == 0)
            {
                flowPurchBidDocDetl.BudFinalPercent = 0;
            }
            else
            {
                flowPurchBidDocDetl.BudFinalPercent = (getValue(flowPurchBidDocDetl.FinalAmt) / budTotalAmt) * 100;
            }

            Boolean firstRec = true;
            foreach (PXResult<KGRequistionPayment> result in PXSelectReadonly<KGRequistionPayment,
                                                                              Where<KGRequistionPayment.reqNbr, Equal<Required<RQRequisition.reqNbr>>>>
                                                                              .Select(Base, Base.Document.Current.ReqNbr))
            {
                KGRequistionPayment requistionPayment = result;

                if (firstRec == true)
                {
                    flowPurchBidDocDetl.CostDay1 = requistionPayment.PaymentPeriod;
                    flowPurchBidDocDetl.CostRatio1 = requistionPayment.PaymentPct;

                    firstRec = false;
                }
                else
                {
                    flowPurchBidDocDetl.CostDay2 = requistionPayment.PaymentPeriod;
                    flowPurchBidDocDetl.CostRatio2 = requistionPayment.PaymentPct;
                }
            }
            KGRequisitionDoc kgRequistionDoc = getKGRequisitionDoc();
            flowPurchBidDocDetl.ResPercent = kgRequistionDoc.DefRetainagePct;
            flowPurchBidDocDetl.PerformanceBondPercent = kgRequistionDoc.PerformanceGuaranteePct;
            if (kgRequistionDoc.WarrantyYear != null)
            {
                flowPurchBidDocDetl.WarrantyYearsOf = Decimal.ToInt32((Decimal)kgRequistionDoc.WarrantyYear);
            }


            KGFlowPurchBidDocDetl.Cache.Insert(flowPurchBidDocDetl);
        }

        /// <summary>
        /// Create function for store some information for external system (AgentFlow)
        /// </summary>
        protected void InsertIntoKGFlowPurchReqsDetl()
        {
            KGFlowPurchaseRequisitionDetail flowPurchReqsDetl = KGFlowPurchReqsDetl.Cache.CreateInstance() as KGFlowPurchaseRequisitionDetail;
            RQRequisitionContent rQRequisitionContent = PXSelectReadonly<RQRequisitionContent,
                                                                        Where<RQRequisitionContent.reqNbr,
                                                                        Equal<Required<RQRequisitionContent.reqNbr>>>>.Select(Base, Base.Document.Current.ReqNbr);
            RQRequest reqDetail = PXSelectReadonly<RQRequest,
                                                 Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>
                                                 .Select(Base, rQRequisitionContent.OrderNbr);

            RQRequestExt reqDetailExt = PXCache<RQRequest>.GetExtension<RQRequestExt>(reqDetail);
            flowPurchReqsDetl.PprUID = AutoNumberAttribute.GetNextNumber(KGFlowPurchReqsDetl.Cache, flowPurchReqsDetl, "KGFLOWPPR", Base.Accessinfo.BusinessDate);
            flowPurchReqsDetl.PpmUID = PpmUID;
            flowPurchReqsDetl.PurchaseProgramNo = Base.Document.Current.ReqNbr;
            flowPurchReqsDetl.ReqUID = reqDetailExt.UsrKGFlowUID;
            flowPurchReqsDetl.RequisitionsNo = reqDetail.OrderNbr;
            /*
            if (Base.request.Current != null)
            {
                flowPurchReqsDetl.RequisitionsNo = Base.request.Current.OrderNbr;
            }*/

            KGFlowPurchReqsDetl.Cache.Insert(flowPurchReqsDetl);
        }

        /// <summary>
        /// Create some records using Acumatica file external link information.
        /// </summary>
        protected void CreateKGUploadFile()
        {
            foreach (KGRequisitionFile requisitionFile in KGReqsFile.Select())
            {
                if (requisitionFile.NoteID == null) { continue; }

                NoteDoc notedoc = PXSelect<NoteDoc,
                                           Where<NoteDoc.noteID, Equal<Required<NoteDoc.noteID>>>>
                                           .Select(Base, requisitionFile.NoteID);
                if (notedoc != null)
                {
                    UploadFile uploadFile = PXSelect<UploadFile,
                                                     Where<UploadFile.fileID, Equal<Required<UploadFile.fileID>>>>
                                                     .Select(Base, notedoc.FileID);

                    if (uploadFile == null) { return; }

                    string buildUtl = PXRedirectToFileException.BuildUrl(notedoc.FileID);
                    string Absouri = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;

                    buildUtl = buildUtl.Substring(1);
                    Absouri = Absouri.Substring(0, Absouri.IndexOf("/M"));

                    KGFlowUploadFile kGFlowUploadFile = KGFlowUploadFile.Cache.CreateInstance() as KGFlowUploadFile;

                    kGFlowUploadFile.Fileid = notedoc.FileID;
                    kGFlowUploadFile.FileName = uploadFile.Name;
                    kGFlowUploadFile.FileLink = Absouri + buildUtl;
                    kGFlowUploadFile.Category = "PR";
                    kGFlowUploadFile.RefNo = PpmUID;
                    kGFlowUploadFile.FileType = requisitionFile.FileType;

                    KGFlowUploadFile.Cache.Insert(kGFlowUploadFile);
                }
            }
        }
        public String message;
        /// <summary>
        /// External web service (AgentFlow) referrnce template.
        /// </summary>
        protected void ConnectAgentFlowWeb()
        {
            string ID = Base.Accessinfo.UserName;
            RQRequisition master = Base.Document.Current;
            RQRequisitionExt requisitionExt = PXCache<RQRequisition>.GetExtension<RQRequisitionExt>(master);

            //using (Kedge.ws.AgentflowWeb.AgentflowWebService soapClient = new Kedge.ws.AgentflowWeb.AgentflowWebService())

            String status = null;
            AgentFlowWebServiceUtil.createPOProcessKedgeURL(ID, PpmUID, requisitionExt.UsrProjectID, ref status, ref message);
            //此時要是錯誤 message 會有值，Status也塞好了
            if (message != null)
            {
                return;
            }
            //status is Call Web Servie return value
            if (status != null && status.StartsWith("http"))
            {
                requisitionExt.UsrReturnUrl = status;
                requisitionExt.UsrKGFlowUID = PpmUID;
                //Base.Document.Cache.SetValueExt<RQRequisitionExt.usrKGFlowUID>(Base.Document.Current, PpmUID);
                //Base.Document.Cache.Update(Base.Document.Current);
                master = Base.Document.Update(master);
                try
                {
                    if (AgentFlowWebServiceUtil.getIsAutoApproved(requisitionExt.UsrProjectID) == true)
                    {
                        requisitionExt = PXCache<RQRequisition>.GetExtension<RQRequisitionExt>(master);
                        //master.Approved = true;
                        requisitionExt.UsrApprovalStatus = ApprovalStatusListAttribute.Approved;
                        Base.Document.Update(master);
                    }
                }
                catch (Exception e)
                {
                    message = "沒設定專案是否自動核准";
                }
            }
        }
        #endregion

        #region Event Handlers
        protected virtual void RQRequisitionLine_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            RQRequisitionLine row = (RQRequisitionLine)e.Row;

            RQRequisitionLineExt requisitionLineExt = PXCache<RQRequisitionLine>.GetExtension<RQRequisitionLineExt>(row);
            requisitionLineExt.Processflag = 1;
        }

        /// <summary>
        /// Remove the initialization event logic according to Louis's request.
        /// </summary>
        /// <param name="e"></param>
        //protected void _(Events.FieldUpdated<KGRequisitionDoc.contractCategoryID> e)
        //{
        //    if (PXSelectGroupBy<KGRequistionPayment,
        //                        Where<KGRequistionPayment.reqNbr, Equal<Required<RQRequisition.reqNbr>>>,
        //                        Aggregate<Count>>.Select(Base, Base.Document.Current.ReqNbr).Count >= 2)
        //    { return; }
        //    this.InitKGReqPayment();
        //}

        /// <summary>
        /// Use status to control action button and data view are readonly.
        /// </summary>
        /// <param name="e"></param>
        protected void _(Events.RowSelected<RQRequisition> e)
        {
            if (e.Row == null)
            {
                return;
            }

            var row = (RQRequisition)e.Row;

            bool isHold = row.Status == RQRequisitionStatus.Hold;
            bool isOpen = row.Status == RQRequisitionStatus.Open;

            RQRequisitionExt requisitionExt = PXCache<RQRequisition>.GetExtension<RQRequisitionExt>(row);
            if (row.Hold == true && requisitionExt.UsrApprovalStatus != ApprovalStatusListAttribute.Approved)
            {
                requisitionExt.UsrReturnUrl = null;
                requisitionExt.UsrApprovalStatus = null;
                requisitionExt.UsrKGFlowUID = null;
            }
            if (ApprovalStatusListAttribute.Approved.Equals(requisitionExt.UsrApprovalStatus))
            {
                this.webServIntegration.SetEnabled(false);
            }
            else
            {
                this.webServIntegration.SetEnabled(isOpen);
            }


            PXUIFieldAttribute.SetEnabled(KGRequistionDoc.Cache, KGRequistionDoc.Cache.Current, isHold);
            PXUIFieldAttribute.SetEnabled(KGReqPayment.Cache, KGReqPayment.Cache.Current, isHold);
            PXUIFieldAttribute.SetEnabled(KGReqsFile.Cache, KGReqsFile.Cache.Current, isHold);

            KGReqPayment.Cache.AllowInsert = KGReqPayment.Cache.AllowDelete = isHold;
            KGReqsFile.Cache.AllowInsert = KGReqsFile.Cache.AllowDelete = isHold;

            RQRequisition master = Base.Document.Current;
            if (RQRequisitionStatus.Released.Equals(master.Status) && Base.POOrders.Select().Count == 0)
            {
                resetStatus.SetEnabled(true);
            }
            else
            {
                resetStatus.SetEnabled(false);
            }




        }


        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            beforeSaveCheck();
            baseMethod();
        }
        /// <summary>
        /// Verify the summary payment percentage must be 100%.
        /// </summary>
        /// <param name="e"></param>
        protected void beforeSaveCheck()
        {
            decimal? totalPaymPct = 0;
            foreach (KGRequistionPayment kGReqPaym in KGReqPayment.Select())
            {
                if (kGReqPaym.PaymentPct < 0m)
                {
                    KGReqPayment.Cache.RaiseExceptionHandling<KGRequistionPayment.paymentPct>(kGReqPaym,
                                                                                              null,
                                                                                              new PXSetPropertyException<KGRequistionPayment.paymentPct>
                                                                                                  (warningMsg, PXErrorLevel.Error));
                    throw new PXException("契約資訊中的付款條件需要修正");
                }

                totalPaymPct += kGReqPaym.PaymentPct;
            }

            if (KGReqPayment.Current != null && totalPaymPct != 100m)
            {
                KGReqPayment.Cache.RaiseExceptionHandling<KGRequistionPayment.paymentPct>(KGReqPayment.Current,
                                                                                              KGReqPayment.Current.PaymentPct,
                                                                                              new PXSetPropertyException<KGRequistionPayment.paymentPct>
                                                                                                  (errorMsg, PXErrorLevel.Error));
                //throw new PXException(errorMsg);
                throw new PXException("契約資訊中的付款條件需要修正");

            }
        }
        #endregion
    }

    public static class ReqCategoryType
    {
        public const string CType = "C";
        public class categoryType : Constant<string>
        {
            public categoryType() : base(CType) { }
        }

    }

    public static class EvaStatus
    {
        public const string Open = "N";
        public class evaStatus : Constant<string>
        {
            public evaStatus() : base(Open) { }
        }

    }
}
