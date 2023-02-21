using System;
using PX.Data;
using NM.DAC;
using NM.Util;
using PX.Objects.GL;
using PX.Data.Licensing;
using PX.Objects.AP;
using PX.Objects.CM;
using BAccountR = PX.Objects.CR.BAccountR;
using RC.Util;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Kedge.DAC;
using PX.Objects.CS;
using static NM.Util.NMStringList;
using KG.Util;

namespace NM
{
    /**
     * ===2021/03/31 Mantis : 0011988 === Althea 
     * Add Action : ModifyAction
     * 當狀態為PendingModify時可以使用
     * 按下按鈕更改此張票據狀態
     * 若checkNbr == null, status = unconfirm
     * 若checkNbr != null, status = confirm
     * 
     * ====2021-05-20 : 口頭====Alton
     * 銀行帳號簡碼：追加代開資訊
     * 資金確認資訊：作廢&退票不顯示
     * 來源：自動→批次
     * 
     * =====2021-05-25: 12054 ====Alton
     * 未確認、已待開、待修改 (也就是"已確認"前)
     * 須開放NMP主檔 可修改 銀行帳號 BankAccountID
     * 且舉凡有修改 銀行帳號，須一併處理
     * a. 初始化 KGBillPayment.UsrNMBatchNbr (可能已代開後改手開)
     * b. 初始化 NMPayableCheck.BookNbr (可能原本放了銀行帳號，後改 銀行代開)
     * ====2021-06-01:12071====Alton
     * 1.產生 APPayment時, APPayment.CuryOrigDocAmt(實付金額)以及APAdjust.CuryAdjgAmt(支付金額), 原來是寫入應付票據的OriCuryAmount, 請改為OriCuryAmount+ KGBillPayment.PostageAmt(郵資費)
     * ---->改在NMApCheckEntry
     **/
    public class NMApCheckEntry : PXGraph<NMApCheckEntry, NMPayableCheck>
    {

        public NMApCheckEntry()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_PAYABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }
        }


        #region View
        public PXSelect<NMPayableCheck> PayableChecks;
        public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<NMPayableCheck.curyInfoID>>>> currencyinfo;
        public PXSelect<KGBillPaymentInfomation, Where<KGBillPaymentInfomation.payableCheckCD, Equal<Current<NMPayableCheck.payableCheckCD>>>> KGBillPaymentInfo;
        public PXSelect<KGBillPayment, Where<KGBillPayment.billPaymentID, Equal<Current<NMPayableCheck.billPaymentID>>>> KGBillPaymentCurrent;
        #endregion

        #region Action
        public ToggleCurrency<NMPayableCheck> CurrencyView;
        #region 確認
        public PXAction<NMPayableCheck> Confirm;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "確認", Enabled = true, Visible = false)]
        protected void confirm()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                NMPayableCheck item = PayableChecks.Current;
                int? oldStatus = item.Status;
                try
                {
                    this.Persist();
                    item.Status = NMStringList.NMAPCheckStatus.CONFIRM;
                    item.CheckNbr = NMCheckBookUtil.getCheckNbr(item.BookNbr);
                    //產生傳票並過帳,回傳傳票號碼
                    //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                    item.ConfirmBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.CONFIRM, item, GLStageCode.NMPConfirmP1);
                    PayableChecks.Update(item);
                    this.Persist();
                    ts.Complete();
                }
                catch (Exception e)
                {
                    item.Status = oldStatus;
                    item.CheckNbr = null;
                    throw e;
                }
            }
        }
        #endregion

        //2020/11/09 Althea Add
        #region 列印支票
        public PXAction<NMPayableCheck> PrintCheck;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "列印支票")]
        protected void printCheck()
        {
            NMPayableCheck check = PayableChecks.Current;
            PXLongOperation.StartOperation(this, PrintMethod);
        }
        #endregion

        #region 重印支票
        public PXAction<NMPayableCheck> RePrintCheck;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "重印支票")]
        protected void rePrintCheck()
        {
            NMPayableCheck check = PayableChecks.Current;
            PXLongOperation.StartOperation(this, PrintMethod);
        }
        #endregion

        //2021/03/31 Mantis:0011988 Althea
        #region 完成修改
        public PXAction<NMPayableCheck> ModifiedAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "完成修改")]
        protected void modifiedAction()
        {
            NMPayableCheck check = PayableChecks.Current;
            PXLongOperation.StartOperation(this, ModifiedMethod);
        }


        #endregion

        #region 手動給號
        public PXAction<NMPayableCheck> ManualAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "手動給號", Visible = false)]
        protected void manualAction()
        {
            NMPayableCheck check = PayableChecks.Current;
            PayableChecks.Cache.SetValueExt<NMPayableCheck.showManual>(check, true);
            PayableChecks.Cache.SetValueExt<NMPayableCheck.isManualCheckNbr>(check, true);
            PayableChecks.Update(check);
            //PayableChecks.Cache.SetStatus(check,PXEntryStatus.Updated);
        }
        #endregion

        #region 取消手動給號
        public PXAction<NMPayableCheck> UnManualAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "取消手動給號", Visible = false)]
        protected void unManualAction()
        {
            NMPayableCheck check = PayableChecks.Current;
            PayableChecks.Cache.SetValueExt<NMPayableCheck.showManual>(check, false);
            PayableChecks.Cache.SetValueExt<NMPayableCheck.isManualCheckNbr>(check, false);
            PayableChecks.Cache.SetValueExt<NMPayableCheck.checkNbr>(check, null);
            Save.Press();
        }
        #endregion

        #endregion

        #region HyperLink
        #region APPayment
        public PXAction<NMPayableCheck> ViewAPPayment;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPPayment()
        {
            NMPayableCheck d = PayableChecks.Current;
            if (d.PaymentDocNo == null) return;
            // Creating the instance of the graph
            APPaymentEntry graph = PXGraph.CreateInstance<APPaymentEntry>();
            // Setting the current product for the graph
            graph.Document.Current = graph.Document.Search<APPayment.refNbr>(d.PaymentDocNo);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Document.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "APPayment")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }
        #endregion

        #region APReleaseGLBatchNbr
        public PXAction<NMPayableCheck> ViewAPReleaseGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPReleaseGL()
        {

            Batch batch = (Batch)PXSelectorAttribute.Select<NMPayableCheck.aPInvBatchNbr>
                (PayableChecks.Cache, PayableChecks.Current);
            new HyperLinkUtil<JournalEntry>(batch, true);
        }
        #endregion

        #region APInvoice
        public PXAction<NMPayableCheck> ViewAPInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPInvoice()
        {
            NMPayableCheck d = PayableChecks.Current;
            if (d.RefNbr == null) return;
            // Creating the instance of the graph
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            // Setting the current product for the graph
            APInvoice aPInvoice = getAPInvoice(d.RefNbr);
            if (aPInvoice == null) return;
            graph.Document.Current = graph.Document.Search<APInvoice.refNbr>(aPInvoice.RefNbr, new object[] { aPInvoice.DocType });
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Document.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "APInvoice")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }
        #endregion

        #region GL Voucher
        public PXAction<NMPayableCheck> ViewGLVoucherByConfirm;
        public PXAction<NMPayableCheck> ViewGLVoucherByConfirmRe;
        public PXAction<NMPayableCheck> ViewGLVoucherByCash;
        public PXAction<NMPayableCheck> ViewGLVoucherByCashRe;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByConfirm()
        {
            NMPayableCheck d = PayableChecks.Current;
            if (d.ConfirmBatchNbr == null) return;
            LinkVoucher(d.ConfirmBatchNbr);
        }
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByConfirmRe()
        {
            NMPayableCheck d = PayableChecks.Current;
            if (d.ConfirmReverseBatchNbr == null) return;
            LinkVoucher(d.ConfirmReverseBatchNbr);
        }
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByCash()
        {
            NMPayableCheck d = PayableChecks.Current;
            if (d.CashBatchNbr == null) return;
            LinkVoucher(d.CashBatchNbr);
        }
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByCashRe()
        {
            NMPayableCheck d = PayableChecks.Current;
            if (d.CashReverseBatchNbr == null) return;
            LinkVoucher(d.CashReverseBatchNbr);
        }
        #endregion

        #endregion

        #region Event
        public override void Persist()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {

                NMPayableCheck item = PayableChecks.Current;
                if (PayableChecks.Cache.GetStatus(item) == PXEntryStatus.Inserted)
                {
                    base.Persist();//為了取得CuryInfoID
                    //mark 因APPayment挪至確認後才產生且過帳
                    //APPayment ap = CreateApPayment(item, true);
                    //item.PaymentDocNo = ap.RefNbr;
                    //item.PaymentDocDate = ap.AdjDate;
                    //PayableChecks.Update(item);
                }
                else if (PayableChecks.Cache.GetStatus(item) == PXEntryStatus.Updated && item.PaymentDocNo != null)
                {
                    //mark 因APPayment挪至確認後才產生且過帳
                    //APPayment ap = CreateApPayment(item, false);
                    //item.PaymentDocDate = ap.AdjDate;
                    //PayableChecks.Update(item);

                    if (item.ShowManual ?? false)
                    {
                        if (item.ManualCheckNbr == null) {
                            SetError<NMPayableCheck.manualCheckNbr>(PayableChecks.Cache, item, item.ManualCheckNbr, "Check Nbr can't be empty.");
                            return;
                        }
                        var checkItem = getNMPayableCheckByCheckNbr(item.PayableCheckID, item.ManualCheckNbr);
                        if (checkItem != null)
                        {
                            SetError<NMPayableCheck.manualCheckNbr>(PayableChecks.Cache, item, item.ManualCheckNbr, "Check Nbr 重複");
                            return;
                        }
                    }
                }
                else
                {
                    foreach (NMPayableCheck deleteItem in PayableChecks.Cache.Deleted)
                    {
                        PXUpdate<
                            Set<KGBillPaymentExt.usrIsCheckIssued, False,
                            Set<KGBillPaymentExt.usrPayableCheckID, Null>>,
                            KGBillPayment,
                            Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                            .Update(this, deleteItem.BillPaymentID);
                        //mark 因APPayment挪至確認後才產生且過帳，因此不再此作刪除
                        //DeleteApPayment(deleteItem);
                    }
                }
                base.Persist();
                ts.Complete();
                PayableChecks.Cache.SetValueExt<NMPayableCheck.showManual>(item, false);
                //刷新ManualUI
                setManual(PayableChecks.Cache,item);
            }
        }

        protected virtual void NMPayableCheck_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMPayableCheck row = (NMPayableCheck)e.Row;
            if (row == null) return;
            setReadOnly(row);
            setUnbound(row);
            setManual(sender, row);
        }

        protected virtual void NMPayableCheck_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMPayableCheck row = (NMPayableCheck)e.Row;
            if (row == null) return;
            int? vendorLocationID = null;
            if (row.VendorID != null)
            {
                Contact contact = getContact(row.VendorID);
                PX.Objects.CR.BAccount2 v = (PX.Objects.CR.BAccount2)PXSelectorAttribute.Select<NMPayableCheck.vendorID>(sender, row);
                row.Title = v.AcctName;
                vendorLocationID = v.DefLocationID;
            }
            row.VendorLocationID = vendorLocationID;
        }

        protected virtual void NMPayableCheck_BankAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMPayableCheck row = (NMPayableCheck)e.Row;
            if (row == null) return;
            NMBankAccount nmBA = (NMBankAccount)PXSelectorAttribute.Select<NMPayableCheck.bankAccountID>(sender, row);
            setUnbound(row);
            KGBillPayment kgBillPayment = KGBillPaymentCurrent.Select();
            KGBillPaymentCurrent.Cache.SetValueExt<KGBillPaymentExt.usrNMBatchNbr>(kgBillPayment, null);
            KGBillPaymentCurrent.Update(kgBillPayment);
            sender.SetValueExt<NMPayableCheck.bookNbr>(row, null);
            sender.SetValueExt<NMPayableCheck.status>(row, NMAPCheckStatus.UNCONFIRM);
            sender.SetValueExt<NMPayableCheck.isIssuedByBank>(row, nmBA.EnableIssueByBank);

        }

        protected virtual void _(Events.FieldUpdated<NMPayableCheck, NMPayableCheck.manualCheckNbr> e)
        {
            var row = e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<NMPayableCheck.checkNbr>(row, e.NewValue);
            NMPayableCheck orgItem = (NMPayableCheck)PXSelectorAttribute.Select<NMPayableCheck.manualCheckNbr>(e.Cache, row);
            e.Cache.SetValueExt<NMPayableCheck.bookNbr>(row, orgItem?.BookNbr);
        }
        #endregion

        #region Method
        private void LinkVoucher(String batchNbr)
        {
            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
            graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(batchNbr);
            if (graph.BatchModule.Current == null) return;
            throw new PXRedirectRequiredException(graph, "GL Voucher")
            {
                Mode = PXBaseRedirectException.WindowMode.NewWindow
            };
        }

        /// <summary>
        /// 產生APPayment，且自動將對應欄位寫入NMPayableCheck row
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="row"></param>
        /// <param name="autoRelease">是否自動過帳</param>
        /// <returns></returns>
        public static APPayment CreateApPayment(APPaymentEntry entry, ref NMPayableCheck row, bool autoRelease)
        {
            APPayment apPayment = null;
            //APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
            apPayment = (APPayment)entry.Document.Cache.CreateInstance();
            apPayment.CuryID = row.CuryID;
            apPayment.CuryInfoID = row.CuryInfoID;
            apPayment.BranchID = row.BranchID;
            apPayment.Hold = true;
            apPayment.DocType = APDocType.Check;
            apPayment = entry.Document.Insert(apPayment);

            APInvoice origAPInvoice = getAPInvoice(entry, row.RefNbr);
            apPayment.VendorID = origAPInvoice.VendorID;
            apPayment.VendorLocationID = origAPInvoice.VendorLocationID;
            apPayment.PaymentMethodID = row.PaymentMethodID;
            apPayment.CashAccountID = row.CashAccountID;
            apPayment = entry.Document.Update(apPayment);
            entry.Document.SetValueExt<APPayment.docDesc>(apPayment, row.Description);
            entry.Document.SetValueExt<APPayment.adjDate>(apPayment, row.CheckDate);
            entry.Document.SetValueExt<APPayment.extRefNbr>(apPayment, row.CheckNbr ?? "N/A");

            //2021-06-01 APPayment金額須排除郵資，改抓KGBillPayment的實付金額
            KGBillPayment billPayment = GetKGBillPayment(entry, row.BillPaymentID);
            //entry.Document.SetValueExt<APPayment.curyOrigDocAmt>(apPayment, row.OriCuryAmount);
            entry.Document.SetValueExt<APPayment.curyOrigDocAmt>(apPayment, billPayment?.PaymentAmount);

            //apPayment = entry.Document.Update(apPayment);
            entry.Save.Press();
            apPayment = entry.Document.Current;
            #region 明細
            APAdjust detail = (APAdjust)entry.Adjustments.Cache.CreateInstance();
            //APInvoice api = getAPInvoice(entry, row.RefNbr);
            detail.AdjdDocType = origAPInvoice.DocType;
            detail.AdjdRefNbr = origAPInvoice.RefNbr;
            detail = entry.Adjustments.Insert(detail);

            //2021-06-01 APPayment金額須排除郵資，改抓KGBillPayment的實付金額
            //entry.Adjustments.Cache.SetValueExt<APAdjust.curyAdjgAmt>(detail, row.OriCuryAmount);
            entry.Adjustments.Cache.SetValueExt<APAdjust.curyAdjgAmt>(detail, billPayment?.PaymentAmount);
            entry.Adjustments.Update(detail);
            #endregion
            entry.Save.Press();
            row.PaymentDocNo = apPayment.RefNbr;
            row.PaymentDocDate = apPayment.AdjDate;
            PXUpdate<
                Set<NMPayableCheck.paymentDocNo, Required<NMPayableCheck.paymentDocNo>,
                Set<NMPayableCheck.paymentDocDate, Required<NMPayableCheck.paymentDocDate>>>,
                NMPayableCheck, 
            Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>
                >.Update(entry, row.PaymentDocNo, row.PaymentDocDate, row.PayableCheckID);

            #region 過帳
            if (autoRelease)
            {
                entry.Document.SetValueExt<APPayment.hold>(apPayment, false);
                apPayment = entry.Document.Update(apPayment);
                entry.Save.Press();
                entry.release.Press();
            }
            #endregion

            return apPayment;
        }
        private void DeleteApPayment(NMPayableCheck row)
        {
            APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
            entry.Document.Current = entry.Document.Search<APPayment.refNbr>(row.PaymentDocNo);
            APPayment apPayment = entry.Document.Current;
            if (apPayment == null) return;
            entry.Document.Delete(apPayment);
            entry.Persist();
        }

        //2020/11/09 Althea Add
        public void PrintMethod()
        {
            string printBatchNbr = getPrintBatchNbr();
            NMPayableCheck check = PayableChecks.Current;
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                check.PrintDate = this.Accessinfo.BusinessDate;
                check.PrintUser = this.Accessinfo.UserID;
                check.PrintCount = (check.PrintCount ?? 0) + 1;
                check.PrintBatchNbr = printBatchNbr;
                check.PrintChineseAmt = NMPrintableProcess.GetChineseNum("單價",
                    string.Format("{0:###0.#}", check.OriCuryAmount ?? 0));

                PayableChecks.Update(check);

                base.Persist();
                ts.Complete();
            }
            NMApCheckEntry pp = CreateInstance<NMApCheckEntry>();
            NewPageToDisplayReport(pp, printBatchNbr);
        }

        //2021/03/31 Add Mantis: 0011988
        /// <summary>
        /// 完成修改按鈕,更改狀態
        /// 若PaymentDocNo為null,狀態改為UNCONFIRM
        /// 若PaymentDocNo不為null,狀態改為CONFIRM
        /// </summary>
        public void ModifiedMethod()
        {
            NMPayableCheck check = PayableChecks.Current;
            if (check.PaymentDocNo == null)
            {
                check.Status = NMStringList.NMAPCheckStatus.UNCONFIRM;
            }
            else
            {
                check.Status = NMStringList.NMAPCheckStatus.CONFIRM;
            }
            PayableChecks.Update(check);
            base.Persist();
        }

        protected virtual void NewPageToDisplayReport(NMApCheckEntry pp, string printBatchNbr)
        {
            NMPayableCheck check = PayableChecks.Current;
            if (check == null) return;

            Dictionary<string, string> mailParams = new Dictionary<string, string>()
            {
                ["PrintBatchNbr"] = printBatchNbr
            };
            var requiredException = new PXReportRequiredException(mailParams, "NM600200", PXBaseRedirectException.WindowMode.New, "支票列印");
            requiredException.SeparateWindows = true;
            throw new PXRedirectWithReportException(pp, requiredException, "Preview");
        }

        private void setReadOnly(NMPayableCheck row)
        {
            bool isInsert = PayableChecks.Cache.GetStatus(row) == PXEntryStatus.Inserted;
            Confirm.SetEnabled(true);
            //Save.SetEnabled(false);
            Insert.SetEnabled(false);
            //2021/03/31 Althea add 
            bool befConfrim = In(row.Status, NMAPCheckStatus.UNCONFIRM, NMAPCheckStatus.REPRESENT, NMAPCheckStatus.PENDINGMODIFY) && row.PaymentDocNo == null;

            if (row.Status != NMStringList.NMAPCheckStatus.UNCONFIRM && row.Status != NMStringList.NMAPCheckStatus.REPRESENT)
            {
                Confirm.SetEnabled(false);
                Delete.SetEnabled(false);
                PXUIFieldAttribute.SetReadOnly(PayableChecks.Cache, row, true);
                PXUIFieldAttribute.SetReadOnly<NMPayableCheck.payableCheckCD>(PayableChecks.Cache, row, false);
            }

            if (In(row.Status, NMAPCheckStatus.CONFIRM, NMAPCheckStatus.CASH, NMAPCheckStatus.PENDINGMODIFY))
            {
                PXUIFieldAttribute.SetReadOnly<NMPayableCheck.checkDate>(PayableChecks.Cache, row, false);
                PXUIFieldAttribute.SetReadOnly<NMPayableCheck.etdDepositDate>(PayableChecks.Cache, row, false);
                PXUIFieldAttribute.SetReadOnly<NMPayableCheck.title>(PayableChecks.Cache, row, false);
                PXUIFieldAttribute.SetReadOnly<NMPayableCheck.description>(PayableChecks.Cache, row, false);
            }

            //2021-05-25 add by Alton 
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.bankAccountID>(PayableChecks.Cache, row, !befConfrim);

            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.payableCashierID>(PayableChecks.Cache, row, !isInsert);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.vendorID>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.vendorLocationID>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.projectID>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.projectPeriod>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.oriCuryAmount>(PayableChecks.Cache, row, true);

            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.branchID>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.paymentDocNo>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.paymentDocDate>(PayableChecks.Cache, row, true);

            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.deliverMethod>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.receiver>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.deliverDate>(PayableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.sendCashierID>(PayableChecks.Cache, row, true);

            NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableCheck.bankAccountID>(PayableChecks.Cache, row);
            SetUIRequired<NMPayableCheck.bookNbr>(PayableChecks.Cache, nba?.EnableIssueByBank != true && row.SourceCode != NMAPSourceCode.HISTORYCHECK);
            if (row.Status == NMStringList.NMAPCheckStatus.PENDINGMODIFY && nba?.EnableIssueByBank != true && row.SourceCode != NMAPSourceCode.HISTORYCHECK)
            {
                PXUIFieldAttribute.SetEnabled<NMPayableCheck.bookNbr>(PayableChecks.Cache, row, true);
            }

            #region ReadyOnly Group Area
            PXUIFieldAttribute.SetEnabled<NMPayableCheck.cashCashierID>(PayableChecks.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<NMPayableCheck.depositDate>(PayableChecks.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<NMPayableCheck.modifyCashierID>(PayableChecks.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<NMPayableCheck.modifyDate>(PayableChecks.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<NMPayableCheck.modifyReason>(PayableChecks.Cache, row, false);


            #endregion

            bool isRePrint = (row.PrintCount ?? 0) > 0;
            //Althea Add 
            PrintCheck.SetEnabled(!isRePrint &&
                row.Status != NMStringList.NMAPCheckStatus.PENDINGMODIFY &&
                row.Status != NMStringList.NMAPCheckStatus.PENDINGREFUND);
            RePrintCheck.SetEnabled(isRePrint &&
                row.Status != NMStringList.NMAPCheckStatus.PENDINGMODIFY &&
                row.Status != NMStringList.NMAPCheckStatus.PENDINGREFUND);
            //2021/03/31 Althea add Mantis: 0011988
            ModifiedAction.SetEnabled(row.Status == NMStringList.NMAPCheckStatus.PENDINGMODIFY);
            KGBillPaymentInfo.Cache.AllowUpdate = false;
            KGBillPaymentInfo.Cache.AllowInsert = false;
        }

        public bool In(object baseVal, params object[] paramVals)
        {
            bool flag = false;
            foreach (object paramVal in paramVals)
            {
                flag = flag || (paramVal.Equals(baseVal));
            }
            return flag;
        }

        public void SetUIRequired<T>(PXCache cache, bool isRequired) where T : PX.Data.IBqlField
        {
            PXUIFieldAttribute.SetRequired<T>(cache, isRequired);
            PXDefaultAttribute.SetPersistingCheck<T>(cache, null, isRequired ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
        }

        private void setUnbound(NMPayableCheck row)
        {
            if (row.BankAccountID != null)
            {
                NMBankAccount bankAccount = getBankAccount(row.BankAccountID);
                row.CashAccountID = bankAccount.CashAccountID;
                row.PaymentMethodID = bankAccount.PaymentMethodID;
            }
        }

        public string getPrintBatchNbr()
        {
            return this.Accessinfo.UserName
                         + System.DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private void setManual(PXCache cache, NMPayableCheck row)
        {
            var isShowManual = row.ShowManual ?? false;
            var isUnConfirm = row.Status == NMStringList.NMAPCheckStatus.UNCONFIRM;
            var isManualCheckNbr = row.IsManualCheckNbr ?? false;

            ManualAction.SetVisible(isUnConfirm && !isManualCheckNbr);
            UnManualAction.SetVisible(isUnConfirm && isManualCheckNbr);

            PXUIFieldAttribute.SetVisible<NMPayableCheck.checkNbr>(cache, row, !isShowManual);
            PXUIFieldAttribute.SetVisible<NMPayableCheck.manualCheckNbr>(cache, row, isShowManual);
            SetUIRequired<NMPayableCheck.manualCheckNbr>(cache, isShowManual);
        }

        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }

        #endregion

        #region Select Method
        private Contact getContact(int? vendorID)
        {
            return PXSelect<Contact,
                Where<Contact.bAccountID, Equal<Required<Contact.bAccountID>>>>
                .Select(new PXGraph(), vendorID);
        }

        private NMBankAccount getBankAccount(int? bankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID,
                Equal<Required<NMBankAccount.bankAccountID>>>>
                .Select(this, bankAccountID);
        }

        private APInvoice getAPInvoice(string refNbr)
        {
            return getAPInvoice(this, refNbr);
        }

        private NMPayableCheck getNMPayableCheckByCheckNbr(int? payableCheckID, string checkNbr)
        {
            return PXSelect<NMPayableCheck,
                Where<NMPayableCheck.status, NotIn3<NMStringList.NMAPCheckStatus.cancel, NMStringList.NMAPCheckStatus.invalid>,
                And<NMPayableCheck.payableCheckID, NotEqual<Required<NMPayableCheck.payableCheckID>>,
                And<NMPayableCheck.checkNbr, Equal<Required<NMPayableCheck.checkNbr>>>>>>
                .Select(new PXGraph(), payableCheckID, checkNbr);
        }

        private static APInvoice getAPInvoice(PXGraph graph, string refNbr)
        {
            return PXSelect<APInvoice, Where<APInvoice.refNbr,
                Equal<Required<APInvoice.refNbr>>,
                And<Where<APInvoice.docType, Equal<APDocType.invoice>,//計價
                        Or<APInvoice.docType, Equal<APDocType.creditAdj>,//貸方調整
                        Or<APInvoice.docType, Equal<APDocType.prepayment>>>>>>>.Select(graph, refNbr);
        }

        private static KGBillPayment GetKGBillPayment(PXGraph graph, int? billPaymentID)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(graph, billPaymentID);
        }


        #endregion

        #region Table
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<KGBillPayment,
            InnerJoin<NMPayableCheck,
                On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>>
            >), Persistent = false)]
        public partial class KGBillPaymentInfomation : IBqlTable
        {

            #region PayableCheckCD
            [PXDBString(BqlField = typeof(NMPayableCheck.payableCheckCD), InputMask = "", IsKey = true)]
            public virtual string PayableCheckCD { get; set; }
            public abstract class payableCheckCD : PX.Data.BQL.BqlString.Field<payableCheckCD> { }
            #endregion

            #region Status
            [PXDBInt(BqlField = typeof(NMPayableCheck.status))]
            public virtual int? Status { get; set; }
            public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
            #endregion

            #region TrPaymentType
            [PXDBString(BqlField = typeof(KGBillPaymentExt.usrTrPaymentType))]
            public virtual string TrPaymentType { get; set; }
            public abstract class trPaymentType : PX.Data.BQL.BqlString.Field<trPaymentType> { }
            #endregion

            #region TrConfirmID
            [PXDBInt(BqlField = typeof(KGBillPaymentExt.usrTrConfirmID))]
            public virtual int? TrConfirmID { get; set; }
            public abstract class trConfirmID : PX.Data.BQL.BqlInt.Field<trConfirmID> { }
            #endregion

            #region TrConfirmDate
            [PXDBDate(BqlField = typeof(KGBillPaymentExt.usrTrConfirmDate))]
            public virtual DateTime? TrConfirmDate { get; set; }
            public abstract class trConfirmDate : PX.Data.BQL.BqlDateTime.Field<trConfirmDate> { }
            #endregion

            #region TrConfirmBy
            [PXDBGuid(BqlField = typeof(KGBillPaymentExt.usrTrConfirmBy))]
            public virtual Guid? TrConfirmBy { get; set; }
            public abstract class trConfirmBy : PX.Data.BQL.BqlGuid.Field<trConfirmBy> { }
            #endregion


            #region unbound
            #region UsrTrPaymentType
            [PXString(1)]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]
            [PXFormula(typeof(Switch<
                Case<Where<status, In3<NMAPCheckStatus.cancel, NMAPCheckStatus.invalid>>, Null>, trPaymentType
                >))]

            public virtual string UsrTrPaymentType { get; set; }
            public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
            #endregion

            #region UsrTrConfirmID
            [PXInt()]
            [PXUIField(DisplayName = "UsrTrConfirmID")]
            [PXFormula(typeof(Switch<
                Case<Where<status, In3<NMAPCheckStatus.cancel, NMAPCheckStatus.invalid>>, Null>, trConfirmID
                >))]
            public virtual int? UsrTrConfirmID { get; set; }
            public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
            #endregion

            #region UsrTrConfirmDate
            [PXDate()]
            [PXUIField(DisplayName = "UsrTrConfirmDate")]
            [PXFormula(typeof(Switch<
                Case<Where<status, In3<NMAPCheckStatus.cancel, NMAPCheckStatus.invalid>>, Null>, trConfirmDate
                >))]
            public virtual DateTime? UsrTrConfirmDate { get; set; }
            public abstract class usrTrConfirmDate : IBqlField { }
            #endregion

            #region UsrTrConfirmBy
            [PXGuid]
            [PXUIField(DisplayName = "UsrTrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]
            [PXFormula(typeof(Switch<
                Case<Where<status, In3<NMAPCheckStatus.cancel, NMAPCheckStatus.invalid>>, Null>, trConfirmBy
                >))]
            public virtual Guid? UsrTrConfirmBy { get; set; }
            public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
            #endregion
            #endregion
        }
        #endregion
    }
}