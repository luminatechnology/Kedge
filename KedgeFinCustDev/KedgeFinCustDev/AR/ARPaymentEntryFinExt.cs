using System;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Objects.Common;
using PX.Objects.AR;
using PS.DAC;
using NM;
using NM.DAC;
using NM.Util;
using PS.Util;
using PS;
using PX.Objects.CR;
using static PS.Util.PSStringList;
using PX.SM;
using PX.Objects.PM;
using KG.Util;
using PX.Objects.GL;
/**
* ====2021-08-23:12201====Alton
* �אּ�@�iAR�|������h�iNM
* �L�b�ɦ�^PSPaymentSlipDetail���o���ڸ��X
* ====2021-08-23 : 12206 ====Alton
* 	�Ш�U�N ARAdjust.AdjdRefNbr LOV �Цh��ܥXARInvoice���G
* 	-ProjectID�θ�ProjectID���y�z
* 	-CustomerID_Description
* ----�j�����----
* 
* **/
namespace PX.Objects.EP
{
    public class ARPaymentEntryFinExt : PXGraphExtension<ARPaymentEntry>
    {
        #region View
        public PXSelect<PSPaymentSlipInfo, Where<PSPaymentSlipInfo.arPaymentRefNbr, Equal<Current<ARPayment.refNbr>>,
            And<ARDocType.payment, Equal<Current<ARPayment.docType>>>>> PaymentSlipInfo;
        public PXSelect<PSPaymentSlipDetails, Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Current<ARPayment.refNbr>>,
            And<ARDocType.payment, Equal<Current<ARPayment.docType>>>>> PaymentSlipDetails;
        #endregion

        #region Action
        public delegate IEnumerable VoidCheckDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable VoidCheck(PXAdapter adapter, VoidCheckDelegate baseMethod)
        {
            ARPayment payment = Base.Document.Current;
            //2022-11-07 12368 alton �ˬd��ARpayment�ϧ_�����s��PSPaymentSlip
            CheckVoidCheck(payment);
            NMReceivableCheck arCheck = GetNMReceivableCheck(payment.RefNbr);
            //��NM�������ڬ� �w�U�� or �w�I�{ �h���i�@�o
            if (arCheck != null && (arCheck.Status == NMStringList.NMARCheckStatus.COLLECTION || arCheck.Status == NMStringList.NMARCheckStatus.CASH))
            {
                bool isCash = arCheck.Status == NMStringList.NMARCheckStatus.CASH;
                String msg = "NM�������� " + (isCash ? "[�w�I�{]" : "[�w�U��]") + " ���i�@�o";
                throw new PXException(msg);
            }
            return baseMethod(adapter);
        }

        public delegate IEnumerable ReleaseDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Release(PXAdapter adapter, ReleaseDelegate baseMethod)
        {
            PXLongOperation.StartOperation(Base, delegate () { OverrideRelease(adapter); });
            return adapter.Get();
        }

        #endregion

        #region HyperLink
        public PXAction<PSPaymentSlipDetails> ViewPSPaymentSlip;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewPSPaymentSlip()
        {
            PSPaymentSlipDetails d = PaymentSlipDetails.Current;
            if (d?.RefNbr == null) return;
            PSPaymentSlipEntry graph = PXGraph.CreateInstance<PSPaymentSlipEntry>();
            graph.PaymentSlips.Current = graph.PaymentSlips.Search<PSPaymentSlip.refNbr>(d.RefNbr);
            if (graph.PaymentSlips.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "PS PaymentSlip")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }

        #endregion

        #region Event

        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (ARPayment deleteItem in Base.Document.Cache.Deleted)
                {
                    if (deleteItem.DocType != ARDocType.Payment) continue;
                    //Stage1.���oPSPaymentSlipDetails �è��oPSPaymentSlip
                    PSPaymentSlipDetails detail = GetPSPaymentSlipDetails(deleteItem.RefNbr);
                    if (detail == null) continue;
                    PSPaymentSlip header = GetPSPaymentSlip(detail.RefNbr);
                    //Stage2.��s������PSPymentSlipDetsils�A�����s�����åBIsVoid = True
                    PXUpdate<
                        Set<PSPaymentSlipDetails.arPaymentRefNbr, Null,
                        Set<PSPaymentSlipDetails.isVoid, True>>,
                        PSPaymentSlipDetails,
                        Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Required<PSPaymentSlipDetails.arPaymentRefNbr>>>>
                        .Update(Base, detail.ArPaymentRefNbr);
                    //Stage3.�P�_PSPaymentSlip���U��PSPaymentSlipDetials�O�_�٦s�b���@�o��
                    int count = CountPSPaymentSlipDetailsByIsNotVoid(header.RefNbr);
                    if (count == 0)
                    {
                        //Stage4.���s�b���p�h�@�oPSPaymentSlip
                        PXUpdate<
                            Set<PSPaymentSlip.status, PSStringList.PSStatus.voided,
                            Set<PSPaymentSlip.voidedBy, Required<PSPaymentSlip.voidedBy>,
                            Set<PSPaymentSlip.voidedDate, Required<PSPaymentSlip.voidedDate>>>>
                        , PSPaymentSlip, Where<PSPaymentSlip.refNbr, Equal<Required<PSPaymentSlip.refNbr>>>>
                        .Update(Base, Base.Accessinfo.UserID, Base.Accessinfo.BusinessDate, header.RefNbr);
                    }
                }
                baseMethod();
                ts.Complete();
            }
        }


        protected virtual void _(Events.RowSelected<ARPayment> e)
        {
            if (e.Row == null) return;
            SetReadOnly();

        }
        #endregion

        #region CacheAttached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        //2021-08-23 : 12206 override ARInvoiceType.AdjdRefNbr
        [PXSelector(typeof(Search2<ARInvoice.refNbr,
            LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
                And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
                And<ARAdjust.released, NotEqual<True>,
                And<ARAdjust.voided, NotEqual<True>,
                And<Where<ARAdjust.adjgDocType, NotEqual<Current<ARRegister.docType>>,
                    Or<ARAdjust.adjgRefNbr, NotEqual<Current<ARRegister.refNbr>>>>>>>>>,
            LeftJoin<ARAdjust2, On<ARAdjust2.adjgDocType, Equal<ARInvoice.docType>,
                And<ARAdjust2.adjgRefNbr, Equal<ARInvoice.refNbr>,
                And<ARAdjust2.released, NotEqual<True>,
                And<ARAdjust2.voided, NotEqual<True>>>>>,
            LeftJoin<Customer, On<ARInvoice.customerID, Equal<Customer.bAccountID>>,
            LeftJoin<PMProject, On<ARInvoice.projectID, Equal<PMProject.contractID>>>>>>,
            Where<ARInvoice.docType, Equal<Optional<ARAdjust.adjdDocType>>,
                And<ARInvoice.released, Equal<True>,
                And<ARInvoice.openDoc, Equal<True>,
                And<ARAdjust.adjgRefNbr, IsNull,
                And<ARAdjust2.adjdRefNbr, IsNull,
                And<ARInvoice.customerID, In2<Search<AR.Override.BAccount.bAccountID,
                    Where<AR.Override.BAccount.bAccountID, Equal<Optional<ARRegister.customerID>>,
                        Or<AR.Override.BAccount.consolidatingBAccountID, Equal<Optional<ARRegister.customerID>>>>>>,
                And2<Where<ARInvoice.pendingPPD, NotEqual<True>,
                    Or<Current<ARRegister.pendingPPD>, Equal<True>>>,
                And<Where<
                    Current<ARSetup.migrationMode>, NotEqual<True>,
                    Or<ARInvoice.isMigratedRecord, Equal<Current<ARRegister.isMigratedRecord>>>>>>>>>>>>>),
            typeof(ARRegister.branchID),
            typeof(ARRegister.refNbr),
            typeof(ARRegister.docDate),
            //typeof(ARRegister.finPeriodID),//mark 2021-08-25 
            typeof(PMProject.contractCD),//2021-08-23 : 12206  add column
            typeof(PMProject.description),//2021-08-23 : 12206  add column
            typeof(ARRegister.customerID),
            typeof(Customer.acctName),//2021-08-23 : 12206  add column
                                      //typeof(ARRegister.customerLocationID), ,//mark 2021-08-25 :12206
                                      //typeof(ARRegister.curyID), ,//mark 2021-08-25 :12206
            typeof(ARRegister.curyOrigDocAmt),
            typeof(ARRegister.curyDocBal),
            typeof(ARRegister.status),
            typeof(ARRegister.dueDate),
            typeof(ARAdjust.ARInvoice.invoiceNbr),
            typeof(ARRegister.docDesc),

            Filterable = true)]
        //2022-12-26 Alton [Jira:KED-25] Fix Error: The multi-part identifier "SOInvoice.HasLegacyCCTran" could not be bound.
        [PXRemoveBaseAttribute(typeof(PXRestrictorAttribute))]
        public void ARAdjust_AdjdRefNbr_CacheAttached(PXCache cache) { }
        #endregion

        #region Method
        public void SetReadOnly()
        {
            PXUIFieldAttribute.SetReadOnly(PaymentSlipDetails.Cache, PaymentSlipDetails.Current, true);

            #region PSPaymentInfo
            PSPaymentSlipInfo info = PaymentSlipInfo.Current;
            bool isEmployee = (info?.TargetType ?? PSTargetType.Employee) == PSTargetType.Employee;
            PXUIFieldAttribute.SetVisible<PSPaymentSlipInfo.createdByID>(PaymentSlipInfo.Cache, info, !isEmployee);
            PXUIFieldAttribute.SetVisible<PSPaymentSlipInfo.payerID>(PaymentSlipInfo.Cache, info, isEmployee);
            #endregion
        }

        /// <summary>
        /// �������ڹL�b����
        /// </summary>
        /// <param name="adapter"></param>
        public void OverrideRelease(PXAdapter adapter)
        {
            //Stage1. ��t���L�b�A�B���Ͷǲ�
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                List<ARRegister> list = BaseRelease(adapter);
                foreach (ARRegister arr in list)
                {
                    if (arr.DocType != ARDocType.Payment && arr.DocType != ARDocType.VoidPayment) continue;
                    ARPayment arp = GetARPayment(arr.RefNbr, arr.DocType);
                    if (arp == null) continue;
                    bool isVoid = arp.DocType == ARDocType.VoidPayment;
                    NMReceivableCheck item;
                    //��I�ڤ覡�� CHECK ���� �� ���ڤ��q���B�~�B�z
                    if (arp.PaymentMethodID == "CHECK")
                    {
                        if (isVoid)
                        {
                            //Stage2. ��I�ڤ覡�����ڥB���A���@�o�ɨ��oNM��������
                            item = GetNMReceivableCheck(arp.RefNbr);
                            if (item != null) {
                                //Stage3. ���t�ǲ��R�b-�R�P���O�G���� �h�^/����
                                item.RecReverseBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.RECREVERSE,
                                                                                        item,
                                                                                        GLStageCode.NMRVoidRB);
                                //Stage4. �p�GNM�������ڬO�w�����A�h��窱�A���w�M��
                                if (item.Status == NMStringList.NMARCheckStatus.RECEIVE)
                                {
                                    item.Status = NMStringList.NMARCheckStatus.WITHDRAW;
                                }
                                PXUpdate<
                                    Set<NMReceivableCheck.status, Required<NMReceivableCheck.status>,
                                    Set<NMReceivableCheck.recReverseBatchNbr, Required<NMReceivableCheck.recReverseBatchNbr>>>,
                                    NMReceivableCheck,
                                    Where<NMReceivableCheck.receivableCheckID, Equal<Required<NMReceivableCheck.receivableCheckID>>>>
                                    .Update(Base, item.Status, item.RecReverseBatchNbr, item.ReceivableCheckID);
                            }
                            
                            PXUpdate<
                                Set<BatchExt.usrStageCode, Required<BatchExt.usrStageCode>>,
                                Batch,
                                Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>.Update(Base, GLStageCode.NMRVoidRC, arp.BatchNbr);
                        }
                        else
                        {

                            //���oPSPaymentSlipDetail
                            foreach (PSPaymentSlipDetails psd in GetPSPaymentDetail(arp.RefNbr, arp.PaymentMethodID, arp.CashAccountID))
                            {
                                //Stage2. ��I�ڤ覡�����ڮɲ���NM��������
                                item = CreateNMArCheck(arp, psd.PaymentRefNbr, psd.TranAmt, psd.TranDesc);
                                //Stage3. ���t�ǲ��R�b-�R�P���O�G����
                                item.RecBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.RECEIVE, item,
                                                                               GLStageCode.NMRReceiveR2);
                                PXUpdate<
                                    Set<NMReceivableCheck.status, Required<NMReceivableCheck.status>,
                                    Set<NMReceivableCheck.recBatchNbr, Required<NMReceivableCheck.recBatchNbr>>>,
                                    NMReceivableCheck,
                                    Where<NMReceivableCheck.receivableCheckID, Equal<Required<NMReceivableCheck.receivableCheckID>>>>
                                    .Update(Base, item.Status, item.RecBatchNbr, item.ReceivableCheckID);
                            }

                            PXUpdate<
                                Set<BatchExt.usrStageCode, Required<BatchExt.usrStageCode>>,
                                Batch,
                                Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>.Update(Base, GLStageCode.NMRPaymentReleaseR1, arp.BatchNbr);


                        }
                    }
                    else
                    {
                        if (isVoid)
                        {
                            PXUpdate<
                                    Set<BatchExt.usrStageCode, Required<BatchExt.usrStageCode>>,
                                    Batch,
                                    Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>.Update(Base,
                                                                                         GLStageCode.NMRVoidRD,
                                                                                         arp.BatchNbr);
                        }
                        else
                        {
                            PXUpdate<
                                    Set<BatchExt.usrStageCode, Required<BatchExt.usrStageCode>>,
                                    Batch,
                                    Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>.Update(Base,
                                                                                         GLStageCode.NMRPaymentReleaseR0,
                                                                                         arp.BatchNbr);
                        }
                    }
                    if (isVoid)
                    {
                        //Stage1. �N������PSPaymentSlipDetails �� IsVoid����True
                        PXUpdate<
                            Set<PSPaymentSlipDetails.isVoid, True>,
                            PSPaymentSlipDetails,
                            Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Required<PSPaymentSlipDetails.arPaymentRefNbr>>>
                            >.Update(Base, arp.RefNbr);
                        //Stage2. PSPaymentSlipDetails������PSPaymentSlip���U������isVoid�Ҭ�true�ɡA���Y���A��ۧ@�o
                        PSPaymentSlipDetails psd = GetPSPaymentSlipDetails(arp.RefNbr);
                        if (psd != null)
                        {
                            int count = CountPSPaymentSlipDetailsByIsNotVoid(psd.RefNbr);
                            if (count == 0)
                            {
                                PXUpdate<
                                    Set<PSPaymentSlip.status, PSStringList.PSStatus.voided,
                                    Set<PSPaymentSlip.voidedBy, Required<PSPaymentSlip.voidedBy>,
                                    Set<PSPaymentSlip.voidedDate, Required<PSPaymentSlip.voidedDate>>>>,
                                    PSPaymentSlip,
                                    Where<PSPaymentSlip.refNbr, Equal<Required<PSPaymentSlip.refNbr>>>
                                    >.Update(Base, Base.Accessinfo.UserID, Base.Accessinfo.BusinessDate, psd.RefNbr);
                            }
                        }
                    }
                    SetBatchStageCode(arp);
                    SetGLTranProjectID(arp);
                }
                ts.Complete();
            }
        }
        /// <summary>
        /// ��tAPPaymentEntry Release
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public List<ARRegister> BaseRelease(PXAdapter adapter)
        {
            PXCache cache = Base.Document.Cache;
            List<ARRegister> list = new List<ARRegister>();
            foreach (ARPayment ardoc in adapter.Get<ARPayment>())
            {
                if (!(bool)ardoc.Hold)
                {
                    cache.Update(ardoc);
                    list.Add(ardoc);
                }
            }
            if (list.Count == 0)
            {
                throw new PXException(Messages.Document_Status_Invalid);
            }
            Base.Save.Press();
            //�אּ�b�~���B�z
            //PXLongOperation.StartOperation(this, delegate () { ARDocumentRelease.ReleaseDoc(list, false); });
            ARDocumentRelease.ReleaseDoc(list, false);
            return list;
        }

        /// <summary>
        /// ����NM���I����
        /// </summary>
        /// <param name="data"></param>
        public NMReceivableCheck CreateNMArCheck(ARPayment data, string checkNbr, decimal? amount, string desc)
        {
            PSPaymentSlipDetails psd = GetPSPaymentSlipDetails(data.RefNbr);
            PSPaymentSlip ps = GetPSPaymentSlip(psd.RefNbr);
            //NMBankAccount bk = GetNMBankAccount(psd.BankAccountID);

            NMArCheckEntry entry = PXGraph.CreateInstance<NMArCheckEntry>();
            NMReceivableCheck item = (NMReceivableCheck)entry.Checks.Cache.CreateInstance();
            item = entry.Checks.Insert(item);
            //item.CheckNbr = data.ExtRefNbr;//2021-08-23
            item.CheckNbr = checkNbr;
            item.BranchID = data.BranchID;
            item.OriBankCode = psd.OriBankCode;
            item.OriBankAccount = psd.OriBankAccount;
            item.CheckProcessDate = data.AdjDate;
            item.CheckDate = psd.CheckDueDate;
            item.DueDate = psd.ActualDueDate;
            item.EtdDepositDate = psd.EtdDepositDate;
            //item.Description = data.DocDesc;//2021-08-23
            item.Description = desc;
            item.CuryID = data.CuryID;
            item.CuryInfoID = data.CuryInfoID;
            //item.OriCuryAmount = data.CuryOrigDocAmt;//2021-08-23
            item.OriCuryAmount = amount;
            item.ProjectID = psd.ContractID;
            item.ProjectPeriod = 0;
            item = entry.Checks.Update(item);
            if ("V".Equals(ps.TargetType))
            {
                //2023-01-10 �ץ�customer�QProject�л\
                entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerID>(item, ps.VendorID);
                entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerLocationID>(item, ps.VendorLocationID);
            }
            else if ("C".Equals(ps.TargetType))
            {
                //2023-01-10 �ץ�customer�QProject�л\
                entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerID>(item, ps.CustomerID);
                entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerLocationID>(item, ps.CustomerLocationID);
            }
            else if ("E".Equals(ps.TargetType))
            {
                //2023-01-10 �ץ�customer�QProject�л\
                entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerID>(item, ps.EmployeeID);
                entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerLocationID>(item, ps.LocationID);
            }
            //item.CustomerID = data.CustomerID;
            //item.CustomerLocationID = data.CustomerLocationID;=
            item.EtdCollBankAccountID = psd.BankAccountID;
            item.PayRefNbr = data.RefNbr;
            //item.CollEmployeeID = psd.CheckIssuer;

            item = entry.Checks.Update(item);
            entry.Save.Press();
            return item;
        }

        public void CheckVoidCheck(ARPayment payment)
        {
            const string THIS_SCREEN_ID = "AR.30.20.00";
            if (payment.DocType == ARDocType.Payment && Base.Accessinfo.ScreenID == THIS_SCREEN_ID)
            {
                var ps = GetPSPaymentSlipDetails(payment.RefNbr);
                if (ps != null && ps.PaymentMethodID == "CHECK") {
                    throw new PXException("�����p��������, �ݳz�z�L�������ڲ��ʧ@�~�i��M�h��!");
                }
            }
        }

        public void SetBatchStageCode(ARPayment payment)
        {
            bool isVoid = payment.DocType == ARDocType.VoidPayment;
            const string THIS_SCREEN_ID = "AR302000";
            if (payment.CreatedByScreenID == THIS_SCREEN_ID)
            {
                string stageCode = null;
                if (!isVoid)
                {
                    stageCode = GLStageCode.NMRPaymentReleaseR5;
                }
                else
                {
                    stageCode = GLStageCode.NMPaymentVoidRF;
                }
                PXUpdate<
                     Set<BatchExt.usrStageCode, Required<BatchExt.usrStageCode>>,
                     Batch,
                     Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>
                     .Update(Base, stageCode, payment.BatchNbr);
            }

        }

        public void SetGLTranProjectID(ARPayment data)
        {
            if (ProjectDefaultAttribute.IsNonProject(data.ProjectID)) return;
            var task = GetDefTask(data.ProjectID);
            if (task == null)
            {
                var prject = PMProject.PK.Find(Base, data.ProjectID);
                throw new PXException($"{0} �г]�w�w�]�M�ץ���", prject.ContractCD);
            }
            PXUpdate<
               Set<GLTran.projectID, Required<GLTran.projectID>,
               Set<GLTran.taskID, Required<GLTran.taskID>>>,
               GLTran,
               Where<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
               And<GLTran.projectID, Equal<Required<GLTran.projectID>>>>>
               .Update(Base,
                       data.ProjectID,
                       task.TaskID,
                       data.BatchNbr,
                       ProjectDefaultAttribute.NonProject()
                       );
        }

        #endregion

        #region BQL
        public int CountPSPaymentSlipDetailsByIsNotVoid(string refNbr)
        {
            return PXSelect<PSPaymentSlipDetails,
                Where<PSPaymentSlipDetails.refNbr, Equal<Required<PSPaymentSlipDetails.refNbr>>,
                And<IsNull<PSPaymentSlipDetails.isVoid, False>, NotEqual<True>>>
                >.Select(Base, refNbr).Count;
        }

        public ARPayment GetARPayment(string refNbr, string docType)
        {
            return PXSelect<ARPayment,
                Where<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>,
                And<ARPayment.docType, Equal<Required<ARPayment.docType>>>>>
                .Select(Base, refNbr, docType);
        }

        public PSPaymentSlip GetPSPaymentSlip(string refNbr)
        {
            return PXSelect<PSPaymentSlip,
                Where<PSPaymentSlip.refNbr, Equal<Required<PSPaymentSlip.refNbr>>>>
                .Select(Base, refNbr);
        }

        public PSPaymentSlipDetails GetPSPaymentSlipDetails(string arPaymentRefNbr)
        {
            return PXSelect<PSPaymentSlipDetails,
                Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Required<PSPaymentSlipDetails.arPaymentRefNbr>>>>
                .Select(Base, arPaymentRefNbr);
        }

        public int CountPSPaymentSlipDetailsRef(string refNbr)
        {
            return PXSelect<PSPaymentSlipDetails,
                Where<PSPaymentSlipDetails.arPaymentRefNbr, IsNotNull,
                And<PSPaymentSlipDetails.refNbr, Equal<Required<PSPaymentSlipDetails.refNbr>>>>>
                .Select(Base, refNbr)
                .Count;
        }

        public NMBankAccount GetNMBankAccount(int? bankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
                .Select(Base, bankAccountID);
        }

        public NMReceivableCheck GetNMReceivableCheck(string refNbr)
        {
            return PXSelect<NMReceivableCheck,
                Where<NMReceivableCheck.payRefNbr, Equal<Required<NMReceivableCheck.payRefNbr>>>>
                .Select(Base, refNbr);
        }

        public PXResultset<PSPaymentSlipDetails> GetPSPaymentDetail(string refNbr, string paymentMethodID, int? cashAccountID)
        {
            return PXSelectJoin<PSPaymentSlipDetails,
                InnerJoin<NMBankAccount,
                    On<PSPaymentSlipDetails.bankAccountID, Equal<NMBankAccount.bankAccountID>>>,
                Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Required<PSPaymentSlipDetails.arPaymentRefNbr>>,
                    And<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                    And<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>>>>>
                .Select(Base, refNbr, cashAccountID, paymentMethodID);
        }

        public PMTask GetDefTask(int? projectID)
        {
            return PXSelect<PMTask,
                Where<PMTask.isDefault, Equal<True>,
                And<PMTask.projectID, Equal<Required<PMTask.projectID>>>>>
                .Select(Base, projectID);
        }
        #endregion

        #region Table - PSPaymentSlipInfo

        [Serializable]
        [PXCacheName("PSPaymentSlipInfo")]
        [PXProjection(typeof(Select5<PSPaymentSlip,
            InnerJoin<PSPaymentSlipDetails, On<PSPaymentSlip.refNbr, Equal<PSPaymentSlipDetails.refNbr>>,
            LeftJoin<EPEmployee, On<PSPaymentSlip.createdByID, Equal<EPEmployee.userID>>>>,
            Aggregate<GroupBy<PSPaymentSlipDetails.paymentRefNbr>>
           >), Persistent = false)]
        public partial class PSPaymentSlipInfo : IBqlTable
        {
            #region ARPaymentRefNbr
            [PXDBString(BqlField = typeof(PSPaymentSlipDetails.arPaymentRefNbr), IsKey = true)]
            [PXUIField(DisplayName = "Payment Ref Nbr", IsReadOnly = true)]
            public virtual string ARPaymentRefNbr { get; set; }
            public abstract class arPaymentRefNbr : PX.Data.BQL.BqlString.Field<arPaymentRefNbr> { }
            #endregion

            #region RefNbr
            [PXDBString(BqlField = typeof(PSPaymentSlip.refNbr))]
            [PXUIField(DisplayName = "Ref Nbr", IsReadOnly = true)]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region PayerID
            [PXInt()]
            [PXUIField(DisplayName = "Payer", IsReadOnly = true)]
            [PXSelector(
                typeof(Search<BAccount2.bAccountID, Where<BAccount2.type, In3<BAccountType.vendorType, BAccountType.customerType, BAccountType.employeeType>>>),
                typeof(BAccount2.acctCD),
                typeof(BAccount2.acctName),
                DescriptionField = typeof(BAccount2.acctName),
                SubstituteKey = typeof(BAccount2.acctCD)
                )]
            [PXFormula(typeof(
                Switch<
                    Case<Where<targetType, Equal<PSTargetType.employee>>, employeeID>,
                    createEmpID
                    >
                ))]
            public virtual int? PayerID { get; set; }
            public abstract class payerID : PX.Data.BQL.BqlInt.Field<payerID> { }
            #endregion

            #region TargetType
            [PXDBString(BqlField = typeof(PSPaymentSlip.targetType))]
            public virtual string TargetType { get; set; }
            public abstract class targetType : PX.Data.BQL.BqlString.Field<targetType> { }
            #endregion

            #region EmployeeID
            [PXDBInt(BqlField = typeof(PSPaymentSlip.employeeID))]
            [PXUIField(DisplayName = "Payer", IsReadOnly = true)]
            //[PXSelector(
            //    typeof(Search<BAccount2.bAccountID, Where<BAccount2.type, In3<BAccountType.vendorType, BAccountType.customerType, BAccountType.employeeType>>>),
            //    typeof(BAccount2.acctCD),
            //    typeof(BAccount2.acctName),
            //    DescriptionField = typeof(BAccount2.acctName),
            //    SubstituteKey = typeof(BAccount2.acctCD)
            //    )]
            public virtual int? EmployeeID { get; set; }
            public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
            #endregion


            #region CreateEmpID
            [PXDBInt(BqlField = typeof(EPEmployee.bAccountID))]
            [PXUIField(DisplayName = "Payer", IsReadOnly = true)]
            public virtual int? CreateEmpID { get; set; }
            public abstract class createEmpID : PX.Data.BQL.BqlInt.Field<createEmpID> { }
            #endregion

            #region CreatedByID
            [PXDBCreatedByID(DisplayName = "Payer", BqlField = typeof(PSPaymentSlip.createdByID))]
            public virtual Guid? CreatedByID { get; set; }
            public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
            #endregion

            //#region CustomerID
            //[PXDBInt(BqlField = typeof(PSPaymentSlip.customerID))]
            //public virtual int? CustomerID { get; set; }
            //public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
            //#endregion

            //#region VendorID
            //[PXDBInt(BqlField = typeof(PSPaymentSlip.vendorID))]
            //public virtual int? VendorID { get; set; }
            //public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            //#endregion
        }
        #endregion
    }
}