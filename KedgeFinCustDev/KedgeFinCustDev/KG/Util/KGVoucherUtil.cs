using PX.Data;
using PX.Objects.GL;
using System;
using PX.Objects.AP;
using Kedge.DAC;
using PX.Objects.PM;
using NM.DAC;
using PX.Objects.CA;


namespace KG.Util
{
    /**
     * ===2021/04/29 :口頭===Althea
     * 更改為:
     * 將產生出來的GL過帳,要到posted的狀態
     * 
     * ===2021/05/07 :0012021 === Althea
     * Add ReferenceID
     * 
     * ===2021/07/14 :0012148 === Althea
     * Add CreateGLFromKGBillPayment 
     * For KGBillPayment CreateAPPayment and after Release
     * 
     * ===2021/07/16 :0012160 === Althea
     * Modify CreateGLFromKGVoucher:
     * 若VoucherL.BillPaymentID不為null, 描述帶抬頭
     * 
     * ===2021/08/03 :0012176 === Althea
     * Add PaymentMethod = F
     * AcctID = KGPostageSetup.TempWriteoffAcctID
     * SubID = KGPostageSetup.TempWriteoffSubID
     * 
     * ===2021/08/24 :0012209 === Althea
     * CreateGLFromKGVoucher Add VoucherType多一個產生反向GL:
     * 內容:
     * C/D交換
     * 
     * ===2021/11/09 : 0012265 === Althea
     * Modify TranDesc = Digest  
     **/
    public class KGVoucherUtil
    {
        /// <summary>
        /// 產生KGVoucher的GL傳票
        /// </summary>
        /// <param name="APInvoice"></param>
        /// Current APInvoice.RefNbr
        /// <returns name ="BatchNbr"></returns>
        public static string CreateGLFromKGVoucher(APRegister register, int VoucherType, string glStageCode)
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Use Select
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            KGVoucherH voucherH = getKGVoucherH(entry, register.RefNbr);
            PXResultset<KGVoucherL> voucherL = getKGVoucherL(entry, voucherH.VoucherHeaderID);
            #region GLHeader : Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();

            batch = entry.BatchModule.Insert(batch);
            batch.Module = "GL";
            batch.BranchID = register.BranchID;
            batch.DateEntered = register.DocDate;
            batch.FinPeriodID = FinPeriod((DateTime)register.DocDate);
            batch.Description = register.DocDesc;

            entry.BatchModule.Update(batch);
            #endregion

            #region GLDetail : GLTran
            foreach (KGVoucherL L in voucherL)
            {
                KGVoucherLFinExt LfinExt = PXCache<KGVoucherL>.GetExtension<KGVoucherLFinExt>(L);
                GLTran tran = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
                tran = entry.GLTranModuleBatNbr.Insert(tran);

                //2021/05/07 Add ReferenceID
                tran.ReferenceID = register.VendorID;

                tran.BranchID = register.BranchID;
                tran.Module = "GL";
                tran.AccountID = L.AccountID;
                tran.SubID = LfinExt.UsrSubID;
                tran.RefNbr = voucherH.RefNbr;
                tran.Qty = 0;

                //2021/07/16 Add Mantis: 0012160 若有BillPaymentID 描述帶KGBIllPayment的抬頭
                //2021/11/09 Delete Mantis: 0012265 TranDesc = Digest 
                tran.TranDesc = L.Digest;
                /*
                if (L.BillPaymentID == null)
                    tran.TranDesc = L.Digest;
                else
                {
                    KGBillPayment billPayment = GetKGBillPayment(entry, L.BillPaymentID);
                    tran.TranDesc = billPayment.CheckTitle;

                }
                */
                //2022/10/06 Alton Mantis:12362 GLTran要押上KGVoucherL的paymentDate&billpaymentID
                GLTranExt tranExt = PXCache<GLTran>.GetExtension<GLTranExt>(tran);
                tranExt.UsrBillPaymentID = L.BillPaymentID;
                tranExt.UsrPaymentDate = LfinExt.UsrPaymentDate;

                //2021/08/24 Add Mantis: 0012209 產生逆向GL(CD交換)
                string CD = "";
                switch (VoucherType)
                {
                    default:
                    case KGVoucherType.CREATEGL:
                        CD = L.Cd;
                        break;
                    case KGVoucherType.REVERSEGL:
                        switch (L.Cd)
                        {
                            default:
                            case "C":
                                CD = "D";
                                break;
                            case "D":
                                CD = "C";
                                break;
                        }
                        break;
                }

                switch (CD)
                {
                    case "D":
                        tran.DebitAmt = L.Amt;
                        tran.CuryDebitAmt = L.Amt;
                        tran.CreditAmt = 0;
                        //tran.TranDate = null;
                        break;
                    case "C":
                        tran.DebitAmt = 0;
                        tran.CreditAmt = L.Amt;
                        tran.CuryCreditAmt = L.Amt;
                        //tran.TranDate = L.DueDate;
                        break;
                }
                tran = entry.GLTranModuleBatNbr.Update(tran);
                //2021/12/15 modify 改成L的專案
                tran.ProjectID = L.ContractID;
                tran.TaskID = LfinExt.UsrTaskID;

                if (LfinExt.UsrCostCodeID != null)
                    tran.CostCodeID = LfinExt.UsrCostCodeID;
                tran = entry.GLTranModuleBatNbr.Update(tran);

            }
            entry.Persist();
            #endregion
            batch = entry.BatchModule.Current;

            batch.Hold = false;
            batch.Status = PX.Objects.GL.BatchStatus.Balanced;
            entry.BatchModule.Update(batch);
            entry.Persist();
            entry.release.Press();

            batch = entry.BatchModule.Current;

            //2021/11/25 Add Mantis: 0012272 狀態不要過帳
            //ReleaseGLtoPosted(batch.BatchNbr);
            PXUpdate<Set<BatchExt.usrAccConfirmNbr, Required<APRegisterFinExt.usrAccConfirmNbr>,
                Set<BatchExt.usrStageCode, Required<BatchExt.usrStageCode>>>,
                Batch,
                Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>
                .Update(entry, registerFinExt.UsrAccConfirmNbr, glStageCode, batch.BatchNbr);


            return batch.BatchNbr;
        }

        /// <summary>
        /// 產生KGBillPayment PaymentMethodID = C/D/E (現金/禮券/授扣)的GL傳票
        /// </summary>
        /// <param name="KGBillPayment"></param>
        /// Current APInvoice.RefNbr
        /// <returns name ="BatchNbr"></returns>
        public static string CreateGLFromKGBillPayment(KGBillPayment billPayment,
                                                       string glStageCode)
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();
            APRegister register = getAPRegister(entry, billPayment.RefNbr);
            //Use Select
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            KGPostageSetup postageSetup = getPostageSetup(entry) ?? throw new Exception(Message.PostageSetupisNull);

            #region GLHeader : Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();

            batch = entry.BatchModule.Insert(batch);
            batch.Module = "GL";
            batch.BranchID = register.BranchID;
            batch.DateEntered = billPayment.PaymentDate;
            batch.FinPeriodID = FinPeriod((DateTime)billPayment.PaymentDate);
            batch.Description = register.DocDesc;
            entry.BatchModule.Update(batch);
            //20220829 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
            BatchExt batchExt = PXCache<Batch>.GetExtension<BatchExt>(batch);
            batchExt.UsrStageCode = glStageCode;
            #endregion

            #region GLDetail : GLTran

            #region TranD
            GLTran tranD = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranD = entry.GLTranModuleBatNbr.Insert(tranD);
            tranD.BranchID = register.BranchID;
            tranD.Module = "GL";
            int? AcctID;
            int? SubID;
            switch (billPayment.PaymentMethod)
            {
                default:
                    AcctID = null;
                    SubID = null;
                    break;
                case Kedge.DAC.PaymentMethod.C:
                    AcctID = postageSetup.KGCashAccountID ?? throw new Exception(Message.PostageSetupAcctIDisNull);
                    SubID = postageSetup.KGCashSubID ?? throw new Exception(Message.PostageSetupSubIDisNull);
                    break;
                case Kedge.DAC.PaymentMethod.D:
                    AcctID = postageSetup.KGEmpCouponAccountID ?? throw new Exception(Message.PostageSetupAcctIDisNull);
                    SubID = postageSetup.KGEmpCouponSubID ?? throw new Exception(Message.PostageSetupSubIDisNull);
                    break;
                case Kedge.DAC.PaymentMethod.E:
                    AcctID = postageSetup.KGAuthAccountID ?? throw new Exception(Message.PostageSetupAcctIDisNull);
                    SubID = postageSetup.KGAuthSubID ?? throw new Exception(Message.PostageSetupSubIDisNull);
                    break;
            }
            tranD.AccountID = AcctID;
            tranD.SubID = SubID;
            tranD.RefNbr = register.RefNbr;
            tranD.Qty = 0;
            tranD.TranDesc = register.DocDesc;
            tranD.DebitAmt = billPayment.PaymentAmount;
            tranD.CuryDebitAmt = billPayment.PaymentAmount;
            tranD.CreditAmt = 0;
            tranD.CuryCreditAmt = 0;
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            tranD.ProjectID = register.ProjectID;
            tranD = entry.GLTranModuleBatNbr.Update(tranD);

            #endregion

            #region TranC
            //2021/12/13 Add mantis: 0012278
            GLTran tranC = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranC = entry.GLTranModuleBatNbr.Insert(tranC);
            tranC.BranchID = register.BranchID;
            tranC.Module = "GL";

            NMBankAccount bankAccount = getNMBank(entry, billPayment.BankAccountID);
            CashAccount cashAccount = getCashAccount(entry, bankAccount.CashAccountID);
            tranC.AccountID = cashAccount.AccountID;
            tranC.SubID = cashAccount.SubID;
            tranC.RefNbr = register.RefNbr;
            tranC.Qty = 0;
            tranC.TranDesc = register.DocDesc;
            tranC.CreditAmt = billPayment.PaymentAmount;
            tranC.CuryCreditAmt = billPayment.PaymentAmount;
            tranC.DebitAmt = 0;
            tranC.CuryDebitAmt = 0;
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            tranC.ProjectID = register.ProjectID;
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            #endregion

            entry.Persist();
            #endregion
            batch = entry.BatchModule.Current;

            batch.Hold = false;
            batch.Status = PX.Objects.GL.BatchStatus.Balanced;
            entry.BatchModule.Update(batch);
            entry.Persist();
            entry.release.Press();

            batch = entry.BatchModule.Current;

            //20220324 modify by louis release傳票後, 不再執行Post, 因為已設定系統release後自動Post
            //ReleaseGLtoPosted(batch.BatchNbr);


            return batch.BatchNbr;
        }

        /// <summary>
        /// 將GL過帳(Posted)
        /// </summary>
        /// <param name="batchNbr"></param>
        public static void ReleaseGLtoPosted(string batchNbr)
        {
            PostGraph postGraph = PXGraph.CreateInstance<PostGraph>();
            Batch batch = PXSelect<Batch, Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>.Select(postGraph, batchNbr);
            postGraph.PostBatchProc(batch);
        }

        #region Method
        public static string FinPeriod(DateTime Date)
        {
            string dateStr = Date.ToString("yyyyMMdd");
            string Period = dateStr.Substring(0, 4) + dateStr.Substring(4, 2);
            return Period;

        }
        public static KGVoucherH getKGVoucherH(PXGraph graph, string RefNbr)
        {
            return PXSelect<KGVoucherH,
                Where<KGVoucherH.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                .Select(graph, RefNbr);
        }
        public static PXResultset<KGVoucherL> getKGVoucherL(PXGraph graph, int? VoucherID)
        {
            return PXSelect<KGVoucherL,
                Where<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherH.voucherHeaderID>>>>
                .Select(graph, VoucherID);
        }
        public static APRegister getAPRegister(PXGraph graph, string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(graph, RefNbr);
        }
        public static APTran GetAPTran(PXGraph graph, string DocType, string RefNbr, int? LineNbr)
        {
            return PXSelect<APTran,
                Where<APTran.tranType, Equal<Required<KGTempPaymentWriteOff.origDocType>>,
                And<APTran.refNbr, Equal<Required<KGTempPaymentWriteOff.origRefNbr>>,
                And<APTran.lineNbr, Equal<Required<KGTempPaymentWriteOff.origLineNbr>>>>>>
                .Select(graph, DocType, RefNbr, LineNbr);
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
        public static NMBankAccount getNMBank(PXGraph graph, int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
                .Select(graph, BankAccountID);
        }
        public static CashAccount getCashAccount(PXGraph graph, int? CashAccountID)
        {
            return PXSelect<CashAccount,
                Where<CashAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>>>
                .Select(graph, CashAccountID);
        }
        public static KGPostageSetup getPostageSetup(PXGraph graph)
        {
            return PXSelect<KGPostageSetup>.Select(graph);
        }
        public static KGBillPayment GetKGBillPayment(PXGraph graph, int? BillPaymentID)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(graph, BillPaymentID);
        }
        #endregion

        #region Message
        public class Message
        {
            public static String PostageSetupisNull = "請至摘要設定維護資料!";
            public static String PostageSetupAcctIDisNull = "請至摘要設定維護科目資料!";
            public static String PostageSetupSubIDisNull = "請至摘要設定維護子科目資料!";


        }

        #endregion

    }
}
