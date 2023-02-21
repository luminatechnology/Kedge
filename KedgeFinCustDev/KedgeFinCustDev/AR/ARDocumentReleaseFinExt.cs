using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.Common;
using PX.Objects.AR;
using PS.DAC;
using NM;
using NM.DAC;
using NM.Util;
using PS;
using KG.Util;
using PX.Objects.GL;
using PX.Objects.PM;

namespace PX.Objects.EP
{
    public class ARDocumentReleaseFinExt : PXGraphExtension<ARDocumentRelease>
    {
        public override void Initialize()
        {
            base.Initialize();
            #region override ARDocumentRelease() AR501000
            ARSetup setup = Base.arsetup.Current;
            Base.ARDocumentList.SetProcessDelegate(
                delegate (List<BalancedARDocument> list)
                {
                    FinProcess(list);
                }
            );
            Base.ARDocumentList.SetProcessCaption(Messages.Release);
            Base.ARDocumentList.SetProcessAllCaption(Messages.ReleaseAll);
            #endregion
        }

        #region HyperLink
        public PXAction<BalancedARDocument> ViewPSPaymentSlip;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewPSPaymentSlip()
        {
            BalancedARDocument d = Base.ARDocumentList.Current;
            BalancedARDocumentFinExt ext = PXCache<BalancedARDocument>.GetExtension<BalancedARDocumentFinExt>(d);
            if (d?.RefNbr == null) return;
            PSPaymentSlipEntry graph = PXGraph.CreateInstance<PSPaymentSlipEntry>();
            graph.PaymentSlips.Current = graph.PaymentSlips.Search<PSPaymentSlip.refNbr>(ext.PSPaymentSlipNbr);
            if (graph.PaymentSlips.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "PS PaymentSlip")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }

        #endregion


        #region Method
        public void FinProcess(List<BalancedARDocument> list)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                List<ARRegister> newlist = new List<ARRegister>(list.Count);
                List<ARRegister> nmlist = new List<ARRegister>(list.Count);
                foreach (BalancedARDocument doc in list)
                {
                    newlist.Add(doc);
                    nmlist.Add(doc);
                }
                ARDocumentRelease.ReleaseDoc(newlist, true);
                OverrideRelease(nmlist);
                ts.Complete();
            }
        }

        public void OverrideRelease(List<ARRegister> list)
        {
            //Stage1. 原廠先過帳，且產生傳票
            foreach (ARRegister arr in list)
            {
                ARPayment arp = GetARPayment(arr.RefNbr, arr.DocType);
                if (arp == null) continue;
                if (arp.DocType != ARDocType.Payment && arp.DocType != ARDocType.VoidPayment) continue;
                bool isVoid = arp.DocType == ARDocType.VoidPayment;
                NMReceivableCheck item;
                //當付款方式為 CHECK 票據 或 票據公司時額外處理
                if (arp.PaymentMethodID == "CHECK")
                {
                    if (isVoid)
                    {
                        //Stage2. 當付款方式為票據且狀態為作廢時取得NM應收票據
                        item = GetNMReceivableCheck(arp.RefNbr);
                        if (item != null)
                        {
                            //Stage3. 對原廠傳票沖帳-沖銷類別：收票 退回/徹票
                            item.RecReverseBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.RECREVERSE, item, GLStageCode.NMRVoidRB);
                            //Stage4. 如果NM應收票據是已收票，則更改狀態為已撤票
                            if (item.Status == NMStringList.NMARCheckStatus.RECEIVE)
                            {
                                item.Status = NMStringList.NMARCheckStatus.WITHDRAW;
                            }
                            PXUpdate<
                                Set<NMReceivableCheck.status, Required<NMReceivableCheck.status>,
                                Set<NMReceivableCheck.recReverseBatchNbr, Required<NMReceivableCheck.recReverseBatchNbr>>>,
                                NMReceivableCheck,
                                Where<NMReceivableCheck.receivableCheckID, Equal<Required<NMReceivableCheck.receivableCheckID>>>>
                                .Update(new PXGraph(), item.Status, item.RecReverseBatchNbr, item.ReceivableCheckID);
                        }

                        PXUpdate<
                                Set<BatchExt.usrStageCode, Required<BatchExt.usrStageCode>>,
                                Batch,
                                Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>.Update(Base, GLStageCode.NMRVoidRC, arp.BatchNbr);
                    }
                    else
                    {
                        //Stage2. 當付款方式為票據時產生NM應收票據
                        item = CreateNMArCheck(arp);
                        //Stage3. 對原廠傳票沖帳-沖銷類別：收票
                        item.RecBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.RECEIVE, item, GLStageCode.NMRReceiveR2);
                        PXUpdate<
                            Set<NMReceivableCheck.status, Required<NMReceivableCheck.status>,
                            Set<NMReceivableCheck.recBatchNbr, Required<NMReceivableCheck.recBatchNbr>>>,
                            NMReceivableCheck,
                            Where<NMReceivableCheck.receivableCheckID, Equal<Required<NMReceivableCheck.receivableCheckID>>>>
                            .Update(new PXGraph(), item.Status, item.RecBatchNbr, item.ReceivableCheckID);
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
                SetBatchStageCode(arp);
                SetGLTranProjectID(arp);
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


        public NMReceivableCheck CreateNMArCheck(ARPayment data)
        {
            PSPaymentSlipDetails psd = GetPSPaymentSlipDetails(data.RefNbr);
            NMBankAccount bk = GetNMBankAccount(psd.BankAccountID);

            NMArCheckEntry entry = PXGraph.CreateInstance<NMArCheckEntry>();
            NMReceivableCheck item = (NMReceivableCheck)entry.Checks.Cache.CreateInstance();
            item = entry.Checks.Insert(item);
            item.CheckNbr = data.ExtRefNbr;
            item.BranchID = data.BranchID;
            item.OriBankCode = psd.OriBankCode;
            item.OriBankAccount = psd.OriBankAccount;
            item.CheckProcessDate = data.AdjDate;
            item.CheckDate = psd.CheckDueDate;
            item.DueDate = psd.ActualDueDate;
            item.EtdDepositDate = psd.EtdDepositDate;
            item.Description = data.DocDesc;
            item.CuryID = data.CuryID;
            item.CuryInfoID = data.CuryInfoID;
            item.OriCuryAmount = data.CuryOrigDocAmt;
            item.ProjectID = psd.ContractID;
            item.ProjectPeriod = 0;
            //2023-01-10 修正customer被Project覆蓋
            //item.CustomerID = data.CustomerID;
            //item.CustomerLocationID = data.CustomerLocationID;
            item.EtdCollBankAccountID = psd.BankAccountID;
            item.PayRefNbr = data.RefNbr;
            //item.CollEmployeeID = psd.CheckIssuer;
            item = entry.Checks.Update(item);

            //2023-01-10 修正customer被Project覆蓋
            entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerID>(item,data.CustomerID);
            entry.Checks.Cache.SetValueExt<NMReceivableCheck.customerLocationID>(item, data.CustomerLocationID);
            item = entry.Checks.Update(item);
            entry.Save.Press();
            return item;
        }

        public void SetGLTranProjectID(ARPayment data)
        {
            if (ProjectDefaultAttribute.IsNonProject(data.ProjectID)) return;
            var task = GetDefTask(data.ProjectID);
            if (task == null)
            {
                var prject = PMProject.PK.Find(Base, data.ProjectID);
                throw new PXException($"{0} 請設定預設專案任務", prject.ContractCD);
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
        public ARPayment GetARPayment(string refNbr, string docType)
        {
            return PXSelect<ARPayment,
                Where<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>,
                And<ARPayment.docType, Equal<Required<ARPayment.docType>>>>>
                .Select(Base, refNbr, docType);
        }

        public PSPaymentSlipDetails GetPSPaymentSlipDetails(string arPaymentRefNbr)
        {
            return PXSelect<PSPaymentSlipDetails,
                Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Required<PSPaymentSlipDetails.arPaymentRefNbr>>>>
                .Select(Base, arPaymentRefNbr);
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

        public PMTask GetDefTask(int? projectID)
        {
            return PXSelect<PMTask,
                Where<PMTask.isDefault, Equal<True>,
                And<PMTask.projectID, Equal<Required<PMTask.projectID>>>>>
                .Select(Base, projectID);
        }
        #endregion

        #region Table
        public class BalancedARDocumentFinExt : PXCacheExtension<BalancedARDocument>
        {
            #region PSPaymentSlipNbr
            [PXString(15, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "PS Payment Slip Nbr", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<PSPaymentSlipDetails.refNbr,
                Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Current<BalancedARDocument.refNbr>>>>))]
            public virtual string PSPaymentSlipNbr { get; set; }
            public abstract class psPaymentSlipNbr : PX.Data.BQL.BqlString.Field<psPaymentSlipNbr> { }
            #endregion
        }
        #endregion

    }
}