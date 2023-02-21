using PX.Data;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NM;
using NM.DAC;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.AP;
using Kedge.DAC;
using KG.Util;

namespace NM.Util
{
    /**
     * ===2021/07/08  : 0012132 === Althea
     * Create APVoucher Add AddVoucherType:
     * APPayment / Write off Postage
     * 
     * ===2021/07/07 : 0012133 === Althea
     * APPayment & Writeoff Postage Add Prepayment AccountID & SubID
     * 
     * ===2021/07/08 : 0012136 === Althea
     * Create TT Voucher
     * 
     * ===2021/07/08 : 0012137 === Althea
     * APPayment & Writeoff Postage Add Prepayment AccountID & SubID
     * 
     * ===2021/07/09 : 0012134 === Althea
     * Create APVoucher Add AddVoucherType:
     * APPayment  Reverse/ Write off Postage Reverse
     *  ===2021/07/09 : 0012135 === Althea
     * APPayment Reverse & Writeoff Postage Reverse Add Prepayment AccountID & SubID
     * 
     **/
    public class NMVoucherUtil
    {
        /// <summary>
        /// 產生NMAR的GL傳票
        /// </summary>
        /// <param name="AddVoucherType"></param>
        /// 使用NMStringList.NMARVoucher
        /// <param name="receivableCheck"></param>
        /// Current ReceivableCheck
        /// <returns name ="BatchNbr"></returns>
        public static string CreateARVoucher(int AddVoucherType, 
                                             NMReceivableCheck receivableCheck,
                                             string glStageCode)
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Use Select
            NMPreference setup = getSetUp(entry) ?? throw new Exception("請至'NM偏好設定',完成設定資料!");
            NMBankAccount bankAccount = getBankAccount(entry, (receivableCheck.CollBankAccountID ?? receivableCheck.EtdCollBankAccountID)) ?? throw new Exception("沒有此銀行帳號!");
            CashAccount cashAccount = getCashAccount(entry, bankAccount.CashAccountID) ?? throw new Exception("請檢查此現金科目!");
            CashAccount clearCashAccount = getClearCashAccount(entry) ?? throw new Exception("請至現金科目新增現金-票據過渡科目!");
            LocationARAccountSub locationARAccountSub = getCustomerLocation(entry, receivableCheck.CustomerID) ?? throw new Exception("請檢查客戶應收帳款科目是否有維護!");
            PMTask task = getPMTaskisDefault(entry, receivableCheck.ProjectID);
            PMProject project = getProject(entry, receivableCheck.ProjectID);
            //Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();
            batch = entry.BatchModule.Insert(batch);
            batch.Module = "GL";
            batch.BranchID = receivableCheck.BranchID;
            switch (AddVoucherType)
            {
                case NMStringList.NMARVoucher.COLLECTION:
                    batch.DateEntered = receivableCheck.CollCheckDate ?? throw new Exception(Message.NMCollDateisNullError);
                    batch.FinPeriodID = FinPeriod((DateTime)receivableCheck.CollCheckDate);
                    batch.Description = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.CASH:
                    batch.DateEntered = receivableCheck.DepositDate ?? throw new Exception("請確認兌現日期是否有值!");
                    batch.FinPeriodID = FinPeriod((DateTime)receivableCheck.DepositDate);
                    batch.Description = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.RECEIVE:
                    //20220407 by louis 繳款交回支票, ARPayment過帳傳票日期要與ARPayment AdjDate相同, 即receivableCheck.CheckProcessDate
                    batch.DateEntered = receivableCheck.CheckProcessDate;
                    batch.Description = receivableCheck.Description ?? null;
                    break;           

                case NMStringList.NMARVoucher.COLLREVERSE:
                case NMStringList.NMARVoucher.CASHREVERSE:
                case NMStringList.NMARVoucher.RECREVERSE:
                    batch.DateEntered = receivableCheck.ReverseDate ?? throw new Exception("請確認異動日期是否有值!");
                    batch.FinPeriodID = FinPeriod((DateTime)receivableCheck.ReverseDate);
                    batch.Description = receivableCheck.Reason ?? throw new Exception("請確認異動原因是否有值!");
                    break;
            }

            //20220830 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
            BatchExt batchExt = PXCache<Batch>.GetExtension<BatchExt>(batch);
            batchExt.UsrStageCode = glStageCode;
            entry.BatchModule.Update(batch);

            //GLTran 
            //借方      
            GLTran tranD = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranD = entry.GLTranModuleBatNbr.Insert(tranD);
            tranD.BranchID = receivableCheck.BranchID;
            tranD.Module = "GL";
            switch (AddVoucherType)
            {
                case NMStringList.NMARVoucher.RECEIVE:
                    tranD.AccountID = clearCashAccount.AccountID;
                    tranD.SubID = clearCashAccount.SubID;
                    tranD.TranDate = receivableCheck.CheckDate;
                    tranD.TranDesc = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.COLLECTION:
                    tranD.AccountID = setup.ARNoteAccountID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranD.SubID = setup.ARNoteSubID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranD.TranDate = receivableCheck.CollCheckDate;
                    tranD.TranDesc = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.CASH:
                    tranD.AccountID = cashAccount.AccountID ?? throw new Exception("請至現金帳戶維護科目!");
                    tranD.SubID = cashAccount.SubID ?? throw new Exception("請至現金帳戶維護子科目!");
                    tranD.TranDate = receivableCheck.DepositDate;
                    tranD.TranDesc = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.RECREVERSE:
                    tranD.AccountID = cashAccount.AccountID;
                    tranD.SubID = cashAccount.SubID;
                    tranD.TranDate = receivableCheck.CheckDate;
                    tranD.TranDesc = receivableCheck.Description ?? null;
                    break;

                //2020/09/23 改為寫死過度科目NMCLEARING
                case NMStringList.NMARVoucher.COLLREVERSE:
                    tranD.AccountID = clearCashAccount.AccountID;
                    tranD.SubID = clearCashAccount.SubID;
                    tranD.TranDate = receivableCheck.ReverseDate;
                    tranD.TranDesc = receivableCheck.Reason ?? throw new Exception("請確認異動原因是否有值!"); ;
                    break;

                case NMStringList.NMARVoucher.CASHREVERSE:
                    tranD.AccountID = setup.ARNoteAccountID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranD.SubID = setup.ARNoteSubID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranD.TranDate = receivableCheck.ReverseDate;
                    tranD.TranDesc = receivableCheck.Reason ?? throw new Exception("請確認異動原因是否有值!"); ;
                    break;


            }
            tranD.CuryDebitAmt = receivableCheck.OriCuryAmount;
            tranD.RefNbr = receivableCheck.PayRefNbr;
            tranD.ReferenceID = receivableCheck.CustomerID;
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            tranD.ProjectID = receivableCheck.ProjectID;
            if (receivableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format(Message.NonSetProjectTaskIDError, project.ContractCD));
                tranD.TaskID = task.TaskID;
            }

            tranD = entry.GLTranModuleBatNbr.Update(tranD);

            //貸方
            GLTran tranC = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranC = entry.GLTranModuleBatNbr.Insert(tranC);
            tranC.BranchID = receivableCheck.BranchID;
            tranC.Module = "GL";
            switch (AddVoucherType)
            {
                case NMStringList.NMARVoucher.RECEIVE:
                    tranC.AccountID = cashAccount.AccountID;
                    tranC.SubID = cashAccount.SubID;
                    tranC.TranDate = receivableCheck.CheckDate;
                    tranC.TranDesc = receivableCheck.Description ?? null;
                    break;

                //2020/09/23 改為寫死過度科目NMCLEARING
                case NMStringList.NMARVoucher.COLLECTION:
                    tranC.AccountID = clearCashAccount.AccountID;
                    tranC.SubID = clearCashAccount.SubID;
                    tranC.TranDate = receivableCheck.CollCheckDate;
                    tranC.TranDesc = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.CASH:
                    tranC.AccountID = setup.ARNoteAccountID;
                    tranC.SubID = setup.ARNoteSubID;
                    tranC.TranDate = receivableCheck.DepositDate;
                    tranC.TranDesc = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.RECREVERSE:
                    tranC.AccountID = clearCashAccount.AccountID;
                    tranC.SubID = clearCashAccount.SubID;
                    tranC.TranDate = receivableCheck.CheckDate;
                    tranC.TranDesc = receivableCheck.Description ?? null;
                    break;

                case NMStringList.NMARVoucher.COLLREVERSE:
                    tranC.AccountID = setup.ARNoteAccountID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranC.SubID = setup.ARNoteSubID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranC.TranDate = receivableCheck.ReverseDate;
                    tranC.TranDesc = receivableCheck.Reason ?? throw new Exception("請確認異動原因是否有值!"); ;
                    break;
                case NMStringList.NMARVoucher.CASHREVERSE:
                    tranC.AccountID = cashAccount.AccountID ?? throw new Exception("請至現金帳戶維護科目!");
                    tranC.SubID = cashAccount.SubID ?? throw new Exception("請至現金帳戶維護子科目!");
                    tranC.TranDate = receivableCheck.ReverseDate;
                    tranC.TranDesc = receivableCheck.Reason ?? throw new Exception("請確認異動原因是否有值!"); ;
                    break;
            }



            tranC.CuryCreditAmt = receivableCheck.OriCuryAmount;
            tranC.RefNbr = receivableCheck.PayRefNbr;
            tranC.ReferenceID = receivableCheck.CustomerID;
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            tranC.ProjectID = receivableCheck.ProjectID;
            if (receivableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format(Message.NonSetProjectTaskIDError, project.ContractCD));
                tranC.TaskID = task.TaskID;
            }
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            //entry.Persist();


            batch.Hold = false;
            batch.Status = PX.Objects.GL.BatchStatus.Balanced;
            entry.BatchModule.Update(batch);
            entry.Persist();
            entry.release.Press();

            batch = entry.BatchModule.Current;
            return batch.BatchNbr;
        }

        /// <summary>
        /// 產生NMAP的GL傳票
        /// </summary>
        /// <param name="AddVoucherType"></param>
        /// 使用NMStringList.NMAPVoucher
        /// <param name="payableCheck" ></param>
        /// Current PayableCheck
        /// <returns name ="BatchNbr"></returns>
        /// 20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
        public static string CreateAPVoucher(int AddVoucherType, NMPayableCheck payableCheck, string glStageCode)
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Use Select
            NMPreference setup = getSetUp(entry) ?? throw new Exception(Message.NMSettingisNull);
            NMBankAccount bankAccount = getBankAccount(entry, payableCheck.BankAccountID) ?? throw new Exception("沒有此銀行帳號!");
            CashAccount cashAccount = getCashAccount(entry, bankAccount.CashAccountID) ?? throw new Exception("請檢查此現金科目!");
            LocationARAccountSub locationARAccountSub = getCustomerLocation(entry, payableCheck.VendorID) ?? throw new Exception("請檢查客戶應收帳款科目是否有維護!");
            CashAccount clearCashAccount = getClearCashAccount(entry) ?? throw new Exception("請至現金科目新增現金-票據過渡科目!");
            PMTask task = getPMTaskisDefault(entry, payableCheck.ProjectID);
            PMProject project = getProject(entry, payableCheck.ProjectID);
            APInvoice invoice = getAPInvoice(entry, payableCheck.RefNbr);
            KGBillPayment billPayment = getKGBillPayment(entry, payableCheck.BillPaymentID);
            Vendor vendor = getVendor(entry, payableCheck.VendorID?? throw new Exception("此資料未維護供應商!"));
            KGPostageSetup postageSetup = getPostageSetUp(entry) ?? throw new Exception(Message.PostageSetupisNull);

            //Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();
            batch = entry.BatchModule.Insert(batch);
            batch.BranchID = payableCheck.BranchID;
            BatchExt batchExt = PXCache<Batch>.GetExtension<BatchExt>(batch);
            //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
            batchExt.UsrStageCode = glStageCode;

            //2022-11-02 by alton 12369
            if (batchExt.UsrStageCode == GLStageCode.NMPConfirmP4) {
                batchExt.UsrRefBatchNbr = payableCheck.PaymentBatchNbr;//NMPConfirmP2
            }
            else if (batchExt.UsrStageCode == GLStageCode.NMPVoidPD) {
                batchExt.UsrRefBatchNbr = payableCheck.PaymentReverseBatchNbr;//NMPVoidPB
            }
            switch (AddVoucherType)
            {              
                case NMStringList.NMAPVoucher.CONFIRM:
                case NMStringList.NMAPVoucher.APPAYMENT:
                case NMStringList.NMAPVoucher.WRITEOFFPOSTAGE:
                case NMStringList.NMAPVoucher.PPM:
                    batch.DateEntered = payableCheck.CheckDate ?? throw new Exception(Message.NMCheckDateisNullError);
                    batch.FinPeriodID = FinPeriod((DateTime)payableCheck.CheckDate);
                    batch.Description = payableCheck.Description ?? null;
                    break;

                case NMStringList.NMAPVoucher.PAYMENT:
                    batch.DateEntered = payableCheck.CheckDate;
                    batch.FinPeriodID = FinPeriod((DateTime)payableCheck.CheckDate);
                    batch.Description = payableCheck.Description ?? null;
                    break;

                case NMStringList.NMAPVoucher.CASH:
                    batch.DateEntered = payableCheck.DepositDate ?? throw new Exception(Message.NMCollDateisNullError);
                    batch.FinPeriodID = FinPeriod((DateTime)payableCheck.DepositDate);
                    batch.Description = payableCheck.Description ?? null;
                    break;

                //20220930 modify by louis 針對撤退票產生的傳票都要用兌現日當傳票日
                case NMStringList.NMAPVoucher.CASHREVERSE:
                    batch.DateEntered = payableCheck.DepositDate ?? throw new Exception(Message.NMCollDateisNullError);
                    batch.FinPeriodID = FinPeriod((DateTime)payableCheck.DepositDate);
                    break;
                case NMStringList.NMAPVoucher.PAYMENTREVERSE:
                case NMStringList.NMAPVoucher.CONFIRMREVERSE:
                case NMStringList.NMAPVoucher.PPMREVERSE:
                //20220503 modify by louis 針對撤退票產生的傳票都要用異動日當傳票日
                case NMStringList.NMAPVoucher.APPAYMENTREVERSE:
                case NMStringList.NMAPVoucher.WRITEOFFPOSTAGEREVERSE:
                    batch.DateEntered = payableCheck.ModifyDate;
                    batch.FinPeriodID = FinPeriod((DateTime)payableCheck.ModifyDate);
                    batch.Description = payableCheck.ModifyReason;
                    break;

            }
            entry.BatchModule.Update(batch);

            //GLTran 
            //借方      
            GLTran tranD = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranD = entry.GLTranModuleBatNbr.Insert(tranD);
            tranD.BranchID = payableCheck.BranchID;
            switch (AddVoucherType)
            {
                //2020/11/12 Mantis:0011789

                //確認&代開
                case NMStringList.NMAPVoucher.CONFIRM:
                    tranD.AccountID = clearCashAccount.AccountID;
                    tranD.SubID = clearCashAccount.SubID;
                    tranD.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;

                //兌現
                case NMStringList.NMAPVoucher.CASH:
                    tranD.AccountID = setup.APNoteAccountID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranD.SubID = setup.APNoteSubID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranD.TranDate = payableCheck.DepositDate;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;

                //付款/過帳付款
                case NMStringList.NMAPVoucher.PAYMENT:
                    tranD.AccountID = cashAccount.AccountID;
                    tranD.SubID = cashAccount.SubID;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;

                //反轉確認
                case NMStringList.NMAPVoucher.CONFIRMREVERSE:
                    tranD.AccountID = setup.APNoteAccountID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranD.SubID = setup.APNoteSubID ?? throw new Exception(Message.NMSettingSubIDisNull);
                    tranD.TranDate = payableCheck.ModifyDate;
                    tranD.TranDesc = payableCheck.ModifyReason ?? null;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;

                //反轉兌現
                case NMStringList.NMAPVoucher.CASHREVERSE:
                    tranD.AccountID = cashAccount.AccountID;
                    tranD.SubID = cashAccount.SubID;
                    tranD.TranDate = payableCheck.ModifyDate;
                    tranD.TranDesc = payableCheck.ModifyReason ?? null;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;

                //反轉付款過帳
                case NMStringList.NMAPVoucher.PAYMENTREVERSE:
                    tranD.AccountID = clearCashAccount.AccountID;
                    tranD.SubID = clearCashAccount.SubID;
                    tranD.TranDate = payableCheck.ModifyDate;
                    tranD.TranDesc = payableCheck.ModifyReason ?? null;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;

                //2021/07/08 Add Mantis: 0012132/0012133
                //APPayment
                case NMStringList.NMAPVoucher.APPAYMENT:
                    tranD.AccountID = postageSetup.KGCheckAccountID ?? throw new Exception(Message.PostageAcctIDisNull);
                    tranD.SubID = postageSetup.KGCheckSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranD.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description;
                    //20221230 Alton 改抓ActPayAmt
                    tranD.CuryDebitAmt = billPayment.ActPayAmt;
                    //tranD.CuryDebitAmt = billPayment.PaymentAmount;
                    break;

                //Write off Postage
                case NMStringList.NMAPVoucher.WRITEOFFPOSTAGE:
                    tranD.AccountID = cashAccount.AccountID;
                    tranD.SubID = cashAccount.SubID;
                    tranD.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description;
                    tranD.CuryDebitAmt = billPayment.PostageAmt;
                    break;

                //PPM
                case NMStringList.NMAPVoucher.PPM:
                    tranD.AccountID = postageSetup.KGCheckAccountID ?? throw new Exception(Message.PostageAcctIDisNull);
                    tranD.SubID = postageSetup.KGCheckSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranD.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;

                //APPayment Reverse
                case NMStringList.NMAPVoucher.APPAYMENTREVERSE:
                    tranD.AccountID = cashAccount.AccountID;
                    tranD.SubID = cashAccount.SubID;
                    tranD.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    //20221230 Alton 改抓ActPayAmt
                    tranD.CuryDebitAmt = billPayment.ActPayAmt;
                    //tranD.CuryDebitAmt = billPayment.PaymentAmount;
                    break;

                //Writeoff Postage Reverse
                case NMStringList.NMAPVoucher.WRITEOFFPOSTAGEREVERSE:
                    tranD.AccountID = postageSetup.KGCheckAccountID ?? throw new Exception(Message.PostageAcctIDisNull);
                    tranD.SubID = postageSetup.KGCheckSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranD.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    tranD.CuryDebitAmt = billPayment.PostageAmt;
                    break;

                //PPM Reverse
                case NMStringList.NMAPVoucher.PPMREVERSE:
                    tranD.AccountID = cashAccount.AccountID;
                    tranD.SubID = cashAccount.SubID;
                    tranD.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    tranD.CuryDebitAmt = payableCheck.OriCuryAmount;
                    break;
            }
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            tranD.ProjectID = payableCheck.ProjectID;
            if (payableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format(Message.NonSetProjectTaskIDError, project.ContractCD));
                tranD.TaskID = task.TaskID;
            }

            tranD = entry.GLTranModuleBatNbr.Update(tranD);




            //貸方
            GLTran tranC = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranC = entry.GLTranModuleBatNbr.Insert(tranC);
            tranC.BranchID = payableCheck.BranchID;
            switch (AddVoucherType)
            {

                case NMStringList.NMAPVoucher.CONFIRM:
                    tranC.AccountID = setup.APNoteAccountID ?? throw new Exception(Message.NMSettingAcctIDisNull);
                    tranC.SubID = setup.APNoteSubID ?? throw new Exception(Message.NMSettingSubIDisNull);
                    tranC.TranDate = payableCheck.CheckDate;
                    tranD.TranDesc = payableCheck.Description;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;

                case NMStringList.NMAPVoucher.CASH:
                    tranC.AccountID = cashAccount.AccountID;
                    tranC.SubID = cashAccount.SubID;
                    tranC.TranDate = payableCheck.DepositDate;
                    tranC.TranDesc = payableCheck.Description ?? null;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;

                case NMStringList.NMAPVoucher.PAYMENT:
                    tranC.AccountID = clearCashAccount.AccountID;
                    tranC.SubID = clearCashAccount.SubID;
                    tranC.TranDesc = payableCheck.Description ?? null;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;

                case NMStringList.NMAPVoucher.CONFIRMREVERSE:
                    tranC.AccountID = clearCashAccount.AccountID;
                    tranC.SubID = clearCashAccount.SubID;
                    tranC.TranDate = payableCheck.ModifyDate;
                    tranC.TranDesc = payableCheck.ModifyReason ?? null;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;

                case NMStringList.NMAPVoucher.CASHREVERSE:
                    tranC.AccountID = setup.APNoteAccountID;
                    tranC.SubID = setup.APNoteSubID;
                    tranC.TranDate = payableCheck.ModifyDate;
                    tranC.TranDesc = payableCheck.ModifyReason ?? null;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;

                case NMStringList.NMAPVoucher.PAYMENTREVERSE:
                    tranC.AccountID = cashAccount.AccountID;
                    tranC.SubID = cashAccount.SubID;
                    tranC.TranDate = payableCheck.ModifyDate;
                    tranC.TranDesc = payableCheck.ModifyReason ?? null;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;

                //2021/07/08 Add Mantis: 0012132
                case NMStringList.NMAPVoucher.APPAYMENT:
                    tranC.AccountID = cashAccount.AccountID;
                    tranC.SubID = cashAccount.SubID;
                    tranC.TranDate = payableCheck.CheckDate;
                    tranC.TranDesc = payableCheck.Description ?? null;
                    //20221230 Alton 改抓ActPayAmt
                    tranC.CuryCreditAmt = billPayment.ActPayAmt;
                    //tranC.CuryCreditAmt = billPayment.PaymentAmount;
                    break;

                case NMStringList.NMAPVoucher.WRITEOFFPOSTAGE:
                    tranC.AccountID = postageSetup.KGCheckAccountID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranC.SubID = postageSetup.KGCheckSubID ?? throw new Exception(Message.PostageSubIDisNull);         
                    tranC.TranDate = payableCheck.CheckDate;
                    tranC.TranDesc = payableCheck.Description ?? null;
                    tranC.CuryCreditAmt = billPayment.PostageAmt;
                    break;

                //PPM
                case NMStringList.NMAPVoucher.PPM:
                    tranC.AccountID = cashAccount.AccountID;
                    tranC.SubID = cashAccount.SubID;
                    tranC.TranDate = payableCheck.CheckDate;
                    tranC.TranDesc = payableCheck.Description;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;

                //APPayment Reverse
                case NMStringList.NMAPVoucher.APPAYMENTREVERSE:
                    tranC.AccountID = postageSetup.KGCheckAccountID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranC.SubID = postageSetup.KGCheckSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranC.TranDate = payableCheck.CheckDate;
                    tranC.TranDesc = payableCheck.Description;
                    //20221230 Alton 改抓ActPayAmt
                    tranC.CuryCreditAmt = billPayment.ActPayAmt;
                    //tranC.CuryCreditAmt = billPayment.PaymentAmount;
                    break;

                //Write off Postage Reverse
                case NMStringList.NMAPVoucher.WRITEOFFPOSTAGEREVERSE:
                    tranC.AccountID = cashAccount.AccountID;
                    tranC.SubID = cashAccount.SubID;
                    tranC.TranDate = payableCheck.CheckDate;
                    tranC.TranDesc = payableCheck.Description;
                    tranC.CuryCreditAmt = billPayment.PostageAmt;
                    break;

                //PPM Reverse
                case NMStringList.NMAPVoucher.PPMREVERSE:
                    tranC.AccountID = postageSetup.KGCheckAccountID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranC.SubID = postageSetup.KGCheckSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    tranC.TranDate = payableCheck.CheckDate;
                    tranC.TranDesc = payableCheck.Description ?? null;
                    tranC.CuryCreditAmt = payableCheck.OriCuryAmount;
                    break;
            }
            // tranC.RefNbr = payableCheck.PayRefNbr;
            //tranC.ReferenceID = payableCheck.CustomerID;
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            tranC.ProjectID = payableCheck.ProjectID;
            if (payableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format(Message.NonSetProjectTaskIDError, project.ContractCD));
                tranC.TaskID = task.TaskID;
            }
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            //entry.Persist();


            batch.Hold = false;
            batch.Status = PX.Objects.GL.BatchStatus.Balanced;
            entry.BatchModule.Update(batch);
            entry.Persist();
            entry.release.Press();

            batch = entry.BatchModule.Current;

            //2022-11-04 Alton 因效能更改順序，改為P2產生後去回寫P4 UsrRefBatchNbr
            if (batchExt.UsrStageCode == GLStageCode.NMPConfirmP2)
            {
                PXUpdate<
                    Set<BatchExt.usrRefBatchNbr,Required<BatchExt.usrRefBatchNbr>>,
                    Batch,
                    Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>
                    .Update(entry,
                    batch.BatchNbr, //usrRefBatchNbr = P2BatchNbr
                    payableCheck.ConfirmWriteoffPostageBatchNbr);//Where P4 batchNbr
            }

            return batch.BatchNbr;
        }

        /// <summary>
        /// 產生TT的GL傳票
        /// </summary>
        /// <param name="AddVoucherType"></param>
        /// 使用NMStringList.NMTTVoucher
        /// <param name="billPayment"></param>
        /// Current KGBillPayment
        /// <returns></returns>
        public static string CreateTTVoucher(int AddVoucherType, 
                                             KGBillPayment billPayment, 
                                             DateTime? ttDate,
                                             string glStageCode,
                                             string refBatchNbr = null
                                             )
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Use Select
            APInvoice invoice = getAPInvoice(entry, billPayment.RefNbr);
            NMPreference setup = getSetUp(entry) ?? throw new Exception("請至'NM偏好設定',完成設定資料!");
            NMBankAccount bankAccount = getBankAccount(entry, billPayment.BankAccountID) ?? throw new Exception("沒有此銀行帳號!");
            CashAccount cashAccount = getCashAccount(entry, bankAccount.CashAccountID) ?? throw new Exception("請檢查此現金科目!");
            PMTask task = getPMTaskisDefault(entry, invoice.ProjectID);
            PMProject project = getProject(entry, invoice.ProjectID);
            //2022-12-22 Alton 移除用不到的髒Code
            //Vendor vendor = getVendor(entry, billPayment.VendorID ?? throw new Exception("此資料未維護供應商!"));
            KGPostageSetup postageSetup = getPostageSetUp(entry) ?? throw new Exception(Message.PostageSetupisNull);
            //2022-12-22 Alton 移除用不到的髒Code
            //Location location = getLocation(entry, billPayment.VendorLocationID);
            //CashAccount billcashAccount = getCashAccount(entry, location.CashAccountID) ?? throw new Exception("請檢查此現金科目!");
            //Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();

            batch = entry.BatchModule.Insert(batch);
            batch.BranchID =invoice.BranchID;
            //batch.DateEntered = billPayment.PaymentDate ?? throw new Exception(Message.NMCheckDateisNullError);
            //batch.FinPeriodID = FinPeriod((DateTime)billPayment.PaymentDate);
            //20220721 by louis 傳票日期改為寫入回饋檔實際付款日期
            batch.DateEntered = ttDate;
            batch.FinPeriodID = FinPeriod((DateTime)ttDate);
            batch.Description = invoice.DocDesc ?? null;
            //20220829 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
            BatchExt batchExt = PXCache<Batch>.GetExtension<BatchExt>(batch);
            batchExt.UsrStageCode = glStageCode;
            //2022-11-02 by alton 12369
            batchExt.UsrRefBatchNbr = refBatchNbr;
            entry.BatchModule.Update(batch);

            //GLTran 
            //借方      
            GLTran tranD = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranD = entry.GLTranModuleBatNbr.Insert(tranD);
            tranD.BranchID = invoice.BranchID;
            switch (AddVoucherType)
            {
                //APPayment
                case NMStringList.NMTTVoucher.APPAYMENT:
                    //20220421 by louis 不論是否預付款,皆寫入應付帳款
                    /**
                    if (invoice.DocType == APDocType.Prepayment)
                    {
                        tranD.AccountID = vendor.PrepaymentAcctID ?? throw new Exception(Message.VendorPPMAcctIDisNull);
                        tranD.SubID = vendor.PrepaymentSubID ?? throw new Exception(Message.VendorPPMSubIDisNull);
                    }
                    else
                    {**/
                        tranD.AccountID = postageSetup.KGTTAccountID ?? throw new Exception(Message.PostageAcctIDisNull);
                        tranD.SubID = postageSetup.KGTTSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    // }
                    //[Jira KED-19] 2022-12-16 Alton : 真正的付款金額是扣除郵資後的付款總額
                    //tranD.CuryDebitAmt = billPayment.PaymentAmount;
                    tranD.CuryDebitAmt = billPayment.ActPayAmt;
                    break;

                //Write off Postage
                case NMStringList.NMTTVoucher.WRITEOFFPOSTAGE:
                    //20220421 by louis 不論是否預付款, 皆寫入銀行帳戶
                    /**
                    if (invoice.DocType == APDocType.Prepayment)
                    {
                        tranD.AccountID = postageSetup.KGTTAccountID ?? throw new Exception(Message.PostageAcctIDisNull);
                        tranD.SubID = postageSetup.KGTTSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    }
                    else
                    {**/
                        tranD.AccountID = cashAccount.AccountID;
                        tranD.SubID = cashAccount.SubID;
                    //}
                    tranD.CuryDebitAmt = billPayment.PostageAmt;
                    break;
            }
            tranD.TranDate = ttDate;
            tranD.TranDesc = invoice.DocDesc??null;
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            tranD.ProjectID = invoice.ProjectID;
            if (invoice.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format(Message.NonSetProjectTaskIDError, project.ContractCD));
                tranD.TaskID = task.TaskID;
            }
            // Store the source field value as suggested by Louis for future auditing.
            entry.GLTranModuleBatNbr.Cache.SetValueExt<GLTranExt.usrBillPaymentID>(tranD, billPayment.BillPaymentID);
            entry.GLTranModuleBatNbr.Update(tranD);

            //貸方
            GLTran tranC = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranC = entry.GLTranModuleBatNbr.Insert(tranC);
            tranC.BranchID = invoice.BranchID;
            switch (AddVoucherType)
            {
                case NMStringList.NMTTVoucher.APPAYMENT:
                    //20220421 by louis 不論是否預付款, 皆寫入銀行帳戶
                    /**
                    if (invoice.DocType == APDocType.Prepayment)
                    {
                        tranC.AccountID = postageSetup.KGTTAccountID ?? throw new Exception(Message.PostageAcctIDisNull);
                        tranC.SubID = postageSetup.KGTTSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    }
                    else
                    {**/
                        tranC.AccountID = cashAccount.AccountID;
                        tranC.SubID = cashAccount.SubID;
                    //}
                    //[Jira KED-19] 2022-12-19 Alton : 真正的付款金額是扣除郵資後的付款總額
                    //tranC.CuryCreditAmt = billPayment.PaymentAmount;
                    tranC.CuryCreditAmt = billPayment.ActPayAmt;
                    break;

                case NMStringList.NMTTVoucher.WRITEOFFPOSTAGE:
                    //20220421 by louis 不論是否預付款,皆寫入應付帳款
                    /**
                    if (invoice.DocType == APDocType.Prepayment)
                    {
                        tranC.AccountID = billcashAccount.AccountID;
                        tranC.SubID = billcashAccount.SubID;
                    }
                    else
                    {**/
                        tranC.AccountID = postageSetup.KGTTAccountID ?? throw new Exception(Message.PostageAcctIDisNull);
                        tranC.SubID = postageSetup.KGTTSubID ?? throw new Exception(Message.PostageSubIDisNull);
                    //}
                    tranC.CuryCreditAmt = billPayment.PostageAmt;
                    break;
            }
            tranC.TranDate = ttDate;
            tranC.TranDesc = invoice.DocDesc ?? null;
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            tranC.ProjectID = invoice.ProjectID;
            if (invoice.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format(Message.NonSetProjectTaskIDError, project.ContractCD));
                tranC.TaskID = task.TaskID;
            }
            // Store the source field value as suggested by Louis for future auditing.
            entry.GLTranModuleBatNbr.Cache.SetValueExt<GLTranExt.usrBillPaymentID>(tranC, billPayment.BillPaymentID);
            entry.GLTranModuleBatNbr.Update(tranC);
            //entry.Persist();

            batch.Hold = false;
            batch.Status = PX.Objects.GL.BatchStatus.Balanced;
            entry.BatchModule.Update(batch);
            entry.Persist();
            entry.release.Press();

            batch = entry.BatchModule.Current;

            return batch.BatchNbr;
        }

        #region Method
        public static string FinPeriod(DateTime Date)
        {
            string dateStr = Date.ToString("yyyyMMdd");
            string Period = dateStr.Substring(0, 4) + dateStr.Substring(4, 2);
            return Period;

        }

        public static NMPreference getSetUp(PXGraph graph)
        {
            return PXSelect<NMPreference>.Select(graph);
        }
        public static NMBankAccount getBankAccount(PXGraph graph, int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMReceivableCheck.collBankAccountID>>>>
                .Select(graph, BankAccountID);
        }
        public static CashAccount getCashAccount(PXGraph graph, int? CashAccountID)
        {
            return PXSelect<CashAccount,
                Where<CashAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>>>
                .Select(graph, CashAccountID);
        }
        public static CashAccount getClearCashAccount(PXGraph graph)
        {
            return PXSelect<CashAccount,
                Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>
                .Select(graph, "NMCLEARING");
        }
        public static LocationARAccountSub getCustomerLocation(PXGraph graph, int? BAccountID)
        {
            return PXSelect<LocationARAccountSub,
                Where<LocationARAccountSub.bAccountID, Equal<Required<NMReceivableCheck.customerID>>>>
                .Select(graph, BAccountID);
        }
        public static PMTask getPMTaskisDefault(PXGraph graph, int? ProjectID)
        {
            if (ProjectID == 0)
            {
                return null;
            }
            else
            {
                return PXSelect<PMTask,
                    Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
                    And<PMTask.isDefault, Equal<True>>>>
                    .Select(graph, ProjectID);
            }

        }
        public static PMProject getProject(PXGraph graph, int? ProjectID)
        {
            return PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>
                .Select(graph, ProjectID);
        }
        public static APInvoice getAPInvoice(PXGraph graph, string RefNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<NMPayableCheck.refNbr>>>>
                .Select(graph, RefNbr);
        }
        public static KGBillPayment getKGBillPayment(PXGraph graph, int? BillPaymentID)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.billPaymentID,Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(graph, BillPaymentID);
        }
        public static Vendor getVendor(PXGraph graph,int? BAccountID)
        {
            return PXSelect<Vendor,
                Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>
                .Select(graph, BAccountID);
        }
        public static KGPostageSetup getPostageSetUp(PXGraph graph)
        {
            return PXSelect<KGPostageSetup>.Select(graph);
        }
        public static Location getLocation(PXGraph graph, int? locationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(graph, locationID);
        }
        #endregion

        #region Error Msg
        public class Message
        {
            public static String NMSettingisNull = "請至NM偏好設定維護資料!";
            public static String NMSettingAcctIDisNull = "請至NM偏好設定,維護科目!";
            public static String NMSettingSubIDisNull = "請至NM偏好設定,維護子科目!";
            public static String NonSetProjectTaskIDError = "請至專案''{0}''預設任務!";
            public static String NMCheckDateisNullError="請確認開票日期是否有值!";
            public static String NMCollDateisNullError="請確認託收日期是否有值!";
            public static String VendorNotSetPrepaymentError = "請維護此供應商的科目與子科目!";
            public static String PostageSetupisNull = "請至摘要設定維護資料!";
            public static String PostageAcctIDisNull = "請至摘要設定維護科目!";
            public static String PostageSubIDisNull = "請至摘要設定維護子科目!";
            public static String VendorPPMSubIDisNull = "請至此員工維護預付款子科目!";
            public static String VendorPPMAcctIDisNull = "請至此員工維護預付款科目!";
        }

        #endregion
    }
}
