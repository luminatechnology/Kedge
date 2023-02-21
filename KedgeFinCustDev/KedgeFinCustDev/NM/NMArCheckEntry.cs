using System;
using PX.Data;
using NM.DAC;
using RC.Util;
using NM.Util;
using PX.Objects.PM;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.AR;
using PX.Objects.CA;
using System.Collections;
using PX.Objects.GL;
using PS;
using PS.DAC;
using PX.Objects.AP;
using PX.Objects.EP;

namespace NM
{
    /**
     * ===2021/03/31 Mantis : 0011988 === Althea
     * Add Action : ModifyAction
     * 當狀態為PendingModify時可以使用
     * 按下按鈕更改此張票據狀態=receive
     * 
     * ===2021/08/10 Mantis: 0012191 ===Althea
     * Add ARPaymentBatchNbr HyperLink
     **/
    public class NMArCheckEntry : PXGraph<NMArCheckEntry, NMReceivableCheck>
    {

        public NMArCheckEntry()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_RECEIVABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        #region View
        public PXSelect<NMReceivableCheck> Checks;
        public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<NMReceivableCheck.curyInfoID>>>> currencyinfo;

        #endregion

        #region Action
        public ToggleCurrency<NMReceivableCheck> CurrencyView;

        //2021/03/31 Mantis:0011988 Althea
        #region 完成修改
        public PXAction<NMReceivableCheck> ModifiedAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "完成修改")]
        protected IEnumerable modifiedAction(PXAdapter adapter)
        {
            NMReceivableCheck check = Checks.Current;
            PXLongOperation.StartOperation(this, ModifiedMethod);
            return adapter.Get();
        }
        #endregion

        #endregion

        #region HyperLink

        #region Coll BatchNbr
        public PXAction<NMReceivableCheck> ViewCollBatchNbr;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual IEnumerable viewCollBatchNbr(PXAdapter adapter)
        {
            NMReceivableCheck row = Checks.Current;
            ViewBatch(NMStringList.NMARVoucher.COLLECTION, row);
            return adapter.Get();
        }
        #endregion

        #region Cash BatchNbr
        public PXAction<NMReceivableCheck> ViewCashBatchNbr;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual IEnumerable viewCashBatchNbr(PXAdapter adapter)
        {
            NMReceivableCheck row = Checks.Current;
            ViewBatch(NMStringList.NMARVoucher.CASH, row);
            return adapter.Get();
        }
        #endregion       

        #region CollReverse BatchNbr
        public PXAction<NMReceivableCheck> ViewCollReverseBatchNbr;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual IEnumerable viewCollReverseBatchNbr(PXAdapter adapter)
        {
            NMReceivableCheck row = Checks.Current;
            ViewBatch(NMStringList.NMARVoucher.COLLREVERSE, row);
            return adapter.Get();
        }
        #endregion

        #region CashReverse BatchNbr
        public PXAction<NMReceivableCheck> ViewCashReverseBatchNbr;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual IEnumerable viewCashReverseBatchNbr(PXAdapter adapter)
        {
            NMReceivableCheck row = Checks.Current;
            ViewBatch(NMStringList.NMARVoucher.CASHREVERSE, row);
            return adapter.Get();
        }
        #endregion

        #region ArPayment
        public PXAction<NMReceivableCheck> ViewArPayment;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual IEnumerable viewArPayment(PXAdapter adapter)
        {
            NMReceivableCheck row = Checks.Current;
            ARPaymentEntry entry = PXGraph.CreateInstance<ARPaymentEntry>();
            entry.Document.Current = entry.Document.Search<ARPayment.refNbr>(row.PayRefNbr);
            if (entry.Document.Current != null)
            {
                throw new PXRedirectRequiredException(entry, "AR Payment")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
            return adapter.Get();
        }
        #endregion

        #region PSpaymentSlip
        public PXAction<NMReceivableCheck> ViewPsPaymentSlip;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual IEnumerable viewPsPaymentSlip(PXAdapter adapter)
        {
            NMReceivableCheck row = Checks.Current;
            PSPaymentSlipEntry entry = PXGraph.CreateInstance<PSPaymentSlipEntry>();
            entry.PaymentSlips.Current = entry.PaymentSlips.Search<PSPaymentSlip.refNbr>(row.ReceiptNbr);
            if (entry.PaymentSlips.Current != null)
            {
                throw new PXRedirectRequiredException(entry, "PS Payment Slip Entry")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
            return adapter.Get();
        }
        #endregion

        #region View ARPayment GL
        public PXAction<NMReceivableCheck> ViewARGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual IEnumerable viewARGL(PXAdapter adapter)
        {
            NMReceivableCheck row = Checks.Current;
            if (row.PayRefNbr == null) return adapter.Get();
            ARPayment payment = GetARPayment(row.PayRefNbr);
            if (payment?.BatchNbr == null) return adapter.Get();

            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
            graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(payment.BatchNbr, BatchModule.AR);
            if (graph.BatchModule.Current == null) return adapter.Get();
            new HyperLinkUtil<JournalEntry>(graph.BatchModule.Current, true);

            return adapter.Get();
        }
        #endregion

        #endregion

        #region Event

        #region 20200921 11696: NM301000 應收票據收票 存檔時, 請不要產生收款及沖賬(AR302000)
        //public override void Persist()
        //{
        //    using (PXTransactionScope ts = new PXTransactionScope())
        //    {
        //        NMReceivableCheck item = Checks.Current;
        //        if (Checks.Cache.GetStatus(item) == PXEntryStatus.Inserted)
        //        {

        //            base.Persist();//為了取得CuryInfoID
        //            item.PayRefNbr = CreateArPayment(item, true);
        //            Checks.Update(item);

        //        }
        //        else if (Checks.Cache.GetStatus(item) == PXEntryStatus.Updated && item.PayRefNbr != null)
        //        {
        //            CreateArPayment(item, false);
        //        }
        //        else
        //        {
        //            foreach (NMReceivableCheck deleteItem in Checks.Cache.Deleted)
        //            {
        //                DeleteArPayment(deleteItem);
        //            }
        //        }
        //        base.Persist();
        //        ts.Complete();
        //    }
        //}
        #endregion

        protected virtual void NMReceivableCheck_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            if (row == null) return;
            setReadOnly();
            if (row.CollBankAccountID != null)
            {
                NMBankAccount bankAccount = getNMBank(row.CollBankAccountID);
                row.BankAccount = bankAccount.BankAccount;
            }
        }

        protected virtual void NMReceivableCheck_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            if (row == null) return;
            if (row.ProjectID != null || row.ProjectID != 0)
            {
                PMProject project = getPMProject(row.ProjectID);
                if (project != null)
                {
                    Checks.Cache.SetValueExt<NMReceivableCheck.customerID>(row, project.CustomerID);
                }
            }
        }

        protected virtual void NMReceivableCheck_IsByElse_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            if (row == null) return;
            if (row.IsByElse == false)
            {
                row.CheckIssuer = null;
            }
        }

        protected virtual void NMReceivableCheck_CheckDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            if (row == null) return;
            setDueDate(row);
        }

        protected virtual void NMReceivableCheck_CheckInDays_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            if (row == null) return;
            setDueDate(row);
        }

        protected virtual void NMReceivableCheck_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            int? customerLocationID = null;
            if (row == null) return;
            if (row.CustomerID != null)
            {
                //Customer v = getCustomer(row.CustomerID);
                customerLocationID = getDefLocation(row.CustomerID);
                //BAccountR v = (BAccountR)PXSelectorAttribute.Select<NMReceivableCheck.customerID>(sender, row);
                //customerLocationID = v.DefLocationID;
            }
            row.CustomerLocationID = customerLocationID;
        }


        #endregion

        #region Method
        public static void DeleteArPayment(NMReceivableCheck item)
        {
            ARPaymentEntry entry = PXGraph.CreateInstance<ARPaymentEntry>();
            entry.Document.Current = entry.Document.Search<ARPayment.refNbr>(item.PayRefNbr);
            ARPayment arPayment = entry.Document.Current;
            if (arPayment == null) return;
            if (arPayment.Status == ARDocStatus.Balanced)
            {
                entry.Document.Delete(arPayment);
                entry.Persist();
            }
            if (arPayment.Status == ARDocStatus.Open)
            {
                entry.voidCheck.Press();
                entry.Save.Press();
                //作廢後還要過帳
                arPayment = entry.Document.Current;
                arPayment.Hold = false;
                entry.Document.Update(arPayment);
                entry.release.Press();
            }

        }

        public virtual string CreateArPayment(NMReceivableCheck item, bool isInsert)
        {
            ARPaymentEntry entry = PXGraph.CreateInstance<ARPaymentEntry>();
            ARPayment arPayment = null;

            //判斷是否為新增
            if (isInsert)
            {
                arPayment = (ARPayment)entry.Document.Cache.CreateInstance();
                arPayment.CuryID = item.CuryID;
                arPayment.CuryInfoID = item.CuryInfoID;
                arPayment = entry.Document.Insert(arPayment);
                arPayment.Hold = false;
            }
            else
            {
                entry.Document.Current = entry.Document.Search<ARPayment.refNbr>(item.PayRefNbr);
                arPayment = entry.Document.Current;
                if (arPayment == null) return null;
            }
            arPayment.DocType = ARDocType.Payment;
            arPayment.BranchID = item.BranchID;
            arPayment.AdjDate = item.CheckProcessDate;
            arPayment.CustomerID = item.CustomerID;
            arPayment.CustomerLocationID = item.CustomerLocationID;
            Customer c = getCustomer(item.CustomerID);
            if (c != null)
            {
                arPayment.PaymentMethodID = c.DefPaymentMethodID;
                PaymentMethodAccount pma = getPaymentMethodAccount(c.DefPaymentMethodID);
                if (pma != null) arPayment.CashAccountID = pma.CashAccountID;
                if (c.DefPaymentMethodID == null || arPayment.CashAccountID == null)
                {
                    String msg = "未維護客戶預設付款方式及現金科目, 請先至客戶主檔維護";
                    PXSetPropertyException e = new PXSetPropertyException(msg, PXErrorLevel.RowError);
                    Checks.Cache.RaiseExceptionHandling<NMReceivableCheck.customerID>(item, item.CustomerID, e);
                    throw e;
                }
            }
            arPayment.DocDesc = item.Description;

            arPayment.ExtRefNbr = item.CheckNbr;
            arPayment.CuryID = item.CuryID;
            arPayment.CuryInfoID = item.CuryInfoID;
            arPayment.CuryOrigDocAmt = item.OriCuryAmount;
            //arPayment.OrigDocAmt = item.BaseCuryAmount;

            arPayment = entry.Document.Update(arPayment);
            entry.Save.Press();
            arPayment = entry.Document.Current;
            return arPayment.RefNbr;
        }

        public static void ViewBatch(int BatchType, NMReceivableCheck row)
        {
            string batchNbr = "";
            switch (BatchType)
            {
                case NMStringList.NMARVoucher.COLLECTION:
                    batchNbr = row.CollBatchNbr;
                    break;
                case NMStringList.NMARVoucher.CASH:
                    batchNbr = row.CashBatchNbr;
                    break;
                case NMStringList.NMARVoucher.COLLREVERSE:
                    batchNbr = row.CollReverseBatchNbr;
                    break;
                case NMStringList.NMARVoucher.CASHREVERSE:
                    batchNbr = row.CashReverseBatchNbr;
                    break;
            }
            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();

            graph.BatchModule.Current = PXSelect<Batch, Where<Batch.batchNbr,
                Equal<Required<Batch.batchNbr>>>>.Select(graph, batchNbr);
            //graph.ContractEval.Search<KGContractEvaluation.contractEvaluationCD>(
            //row.ContractEvaluationCD);
            //throw new PXPopupRedirectException(graph, string.Empty, true);
            if (graph.BatchModule.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "Batchl")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }

        public void setReadOnly()
        {
            NMReceivableCheck row = Checks.Current;

            #region ReadOnly Group Area        
            //2021/03/21 將PENDINGMODIFY的模式與RECEIVE相同
            bool Status = row.Status != NMStringList.NMARCheckStatus.RECEIVE && 
                                 row.Status != NMStringList.NMARCheckStatus.PENDINGMODIFY;
            Insert.SetEnabled(false);
            if (Status)
            {
                PXUIFieldAttribute.SetReadOnly(Checks.Cache, row, Status);
                Delete.SetEnabled(false);
            }
            else
            {
                Delete.SetEnabled(true);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.collBankAccountID>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.collCashierID>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.collCheckDate>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.cashCashierID>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.depositDate>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.modifyCashierID>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.reverseDate>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetEnabled<NMReceivableCheck.reason>(Checks.Cache, row, false);
                PXUIFieldAttribute.SetReadOnly<NMReceivableCheck.checkIssuer>(Checks.Cache, row, row.IsByElse == false);
            }
            #endregion

            ModifiedAction.SetEnabled(row.Status == NMStringList.NMARCheckStatus.PENDINGMODIFY);
        }

        /**
         * 重新計算 到期日 與 預計兌現日
         * **/
        private void setDueDate(NMReceivableCheck row)
        {
            if (row.CheckDate == null || row.CheckInDays == null) return;
            DateTime checkDate = row.CheckDate ?? new DateTime();
            int checkInDays = row.CheckInDays ?? 0;

            DateTime date = checkDate.AddDays(checkInDays);
            row.DueDate = date;
            row.EtdDepositDate = date;

        }

        /// <summary>
        /// 完成修改按鈕,更改狀態 = RECEIVE
        /// </summary>

        private void ModifiedMethod()
        {
            NMReceivableCheck check = Checks.Current;
            check.Status = NMStringList.NMARCheckStatus.RECEIVE;
            Checks.Update(check);
            base.Persist();
        }

        #endregion

        #region Select Method
        private PMProject getPMProject(int? projectID)
        {
            return PXSelect<PMProject,
                Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>
                .Select(this, projectID);
        }

        public NMBankAccount getNMBank(int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
                .Select(this, BankAccountID);
        }

        public Customer getCustomer(int? customerID)
        {
            return PXSelect<Customer,
                Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>
            .Select(new PXGraph(), customerID);
        }

        public int? getDefLocation(int? baccountID)
        {
            int? defLocationID = null;
            BAccount bAccount = PXSelect<BAccount,
                                      Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                                      .Select(new PXGraph(), baccountID);
            if (bAccount.Type.Equals(BAccountType.CustomerType)) {
                Customer customer = PXSelect<Customer,
                                      Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>
                                      .Select(new PXGraph(), baccountID);
                defLocationID = customer?.DefLocationID;
            } else if (bAccount.Type.Equals(BAccountType.VendorType) || bAccount.Type.Equals(BAccountType.CombinedType)) {
                Vendor vendor = PXSelect<Vendor,
                                      Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>
                                      .Select(new PXGraph(), baccountID);
                defLocationID = vendor?.DefLocationID;
            } else if (bAccount.Type.Equals(BAccountType.EmployeeType)) {
                EPEmployee employee = PXSelect<EPEmployee,
                                          Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>
                                          .Select(new PXGraph(), baccountID);
                defLocationID = employee?.DefLocationID;
            }
            
            return defLocationID;
        }

        public PaymentMethodAccount getPaymentMethodAccount(string paymentMethodID)
        {
            return PXSelect<PaymentMethodAccount,
                Where<PaymentMethodAccount.paymentMethodID, Equal<Required<PaymentMethodAccount.paymentMethodID>>>>
                .Select(this, paymentMethodID);
        }

        private ARPayment GetARPayment(string RefNbr)
        {
            return PXSelect<ARPayment,
                Where<ARPayment.refNbr, Equal<Required<NMReceivableCheck.payRefNbr>>>>
                .Select(this, RefNbr);
        }
        #endregion
    }
}