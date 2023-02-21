using System;
using EP.DAC;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.PM;

namespace EP.Util
{
    public class EPVoucherUtil
    {
        //2020/08/20 原先用EPDetail當來源 改成 KGExpenseVoucher
        /// <summary>
        /// 產生EP DocType = GUR (傳票作業) 的GL傳票
        /// </summary>
        /// <param name="claim"></param>
        /// Current EPExpenseClaim
        /// <returns name ="BatchNbr"></returns>
        public static string CreateEPVoucher(EPExpenseClaim claim)
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();
            batch = entry.BatchModule.Insert(batch);
            batch.Module = "GL";
            batch.BranchID = claim.BranchID;
            batch.DateEntered = claim.DocDate;
            batch.FinPeriodID = FinPeriod((DateTime)claim.DocDate);
            batch.Description = claim.DocDesc ?? null;
            entry.BatchModule.Update(batch);

            //PXResultset<KGExpenseVoucher> voucher = getVoucher(entry, claim.RefNbr);
            foreach (KGExpenseVoucher details in getVoucher(entry, claim.RefNbr))
            {
                #region GLTran
                //GLTran 
                //借方      
                GLTran tranBankTxn = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
                tranBankTxn = entry.GLTranModuleBatNbr.Insert(tranBankTxn);
                tranBankTxn.BranchID = details.BranchID;
                tranBankTxn.Module = "GL";
                tranBankTxn.AccountID = details.AccountID;
                tranBankTxn.SubID = details.SubID;
                tranBankTxn.TranDate = details.TranDate;
                tranBankTxn.TranDesc = details.TranDesc ?? null;
                tranBankTxn.CuryDebitAmt = details.DebitAmt;
                tranBankTxn.CuryCreditAmt = details.CreditAmt;
                tranBankTxn.RefNbr = details.EPRefNbr;
                //tranBankTxn.ReferenceID = details.CustomerID;
                tranBankTxn = entry.GLTranModuleBatNbr.Update(tranBankTxn);
                tranBankTxn.ProjectID = details.ProjectID;
                if (tranBankTxn.ProjectID != 0)
                {
                    PMTask task = getTask(entry, tranBankTxn.ProjectID);
                    tranBankTxn.TaskID = task?.TaskID;
                }
                entry.GLTranModuleBatNbr.Update(tranBankTxn);
                #endregion
            }

            batch.Hold = false;
            batch.Status = BatchStatus.Balanced;
            // According to mantis [0012358]-(0025831) #4
            entry.BatchModule.Cache.SetValue<BatchExt.usrStageCode>(batch, claim.GetExtension<EPExpenseClaimExt>().UsrStageCode);
            entry.BatchModule.Update(batch);
            entry.Persist();
            entry.release.Press();

            batch = entry.BatchModule.Current;

            return batch.BatchNbr;
        }

        //2020/10/19 Add Mantis: 0012256 DocType = BTN產生的傳票
        /// <summary>
        /// 產生EP DocType = BTN(金融交易) 的GL傳票
        /// </summary>
        /// <param name="claim"></param>
        /// Current EPExpenseClaim
        /// <returns name ="BatchNbr"></returns>
        public static string CreateBTNVoucher(EPExpenseClaim claim)
        {
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Use Select

            //Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();
            batch = entry.BatchModule.Insert(batch);
            batch.Module = "GL";
            batch.BranchID = claim.BranchID;
            batch.DateEntered = claim.DocDate;
            batch.FinPeriodID = FinPeriod((DateTime)claim.DocDate);
            batch.Description = claim.DocDesc ?? null;
            entry.BatchModule.Update(batch);

            //GLTran
            //一筆detail一筆GLTran
            PXResultset<EPExpenseClaimDetails> voucher = getEPDetails(entry, claim.RefNbr);
            foreach (EPExpenseClaimDetails details in voucher)
            {
                #region GL Tran
                //借方      
                GLTran tran = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
                tran = entry.GLTranModuleBatNbr.Insert(tran);
                tran.BranchID = details.BranchID;
                tran.Module = "GL";
                tran.AccountID = details.ExpenseAccountID;
                tran.SubID = details.ExpenseSubID;
                tran.TranDate = details.ExpenseDate;
                tran.TranDesc = details.TranDesc ?? null;
                tran.Qty = details.Qty;
                tran.UOM = details.UOM;
                if (details.CuryTranAmtWithTaxes >= 0)
                {
                    tran.CuryDebitAmt = details.CuryTranAmtWithTaxes;
                    tran.CuryCreditAmt = 0m;
                }
                else
                {
                    tran.CuryDebitAmt = 0m;
                    tran.CuryCreditAmt = -1m *details.CuryTranAmtWithTaxes;
                }
                tran.RefNbr = details.RefNbr;
                tran.NonBillable = false;
                //tran.ReferenceID = details.CustomerID;
                tran = entry.GLTranModuleBatNbr.Update(tran);
                tran.ProjectID = 0;
                tran = entry.GLTranModuleBatNbr.Update(tran);
                #endregion
            }

            //2021/10/22 Delete Phil&Tiffany討論不需要這筆
            //針對BankTxnAmt 新增一筆GLTran 
            /*
            #region GL Tran BankTxn  
            CashAccount cashAccount = GetCashAccountByNMBank(entry,claimExt.UsrBankAccountID);
            GLTran tranBankTxn = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranBankTxn = entry.GLTranModuleBatNbr.Insert(tranBankTxn);
            tranBankTxn.BranchID = claim.BranchID;
            tranBankTxn.Module = "GL";
            tranBankTxn.AccountID = cashAccount.AccountID;
            tranBankTxn.SubID = cashAccount.SubID;
            tranBankTxn.TranDate = claim.DocDate;
            tranBankTxn.TranDesc = claim.DocDesc ?? null;
            tranBankTxn.Qty = 1m;
            if (claimExt.UsrBankTxnAmt >= 0)
            {
                tranBankTxn.CuryDebitAmt = claimExt.UsrBankTxnAmt;
                tranBankTxn.CuryCreditAmt = 0m;
            }
            else
            {
                tranBankTxn.CuryDebitAmt = 0m;
                tranBankTxn.CuryCreditAmt = -1m *claimExt.UsrBankTxnAmt;
            }
            tranBankTxn.RefNbr = claim.RefNbr;
            tranBankTxn.NonBillable = false;
            //tranBankTxn.ReferenceID = details.CustomerID;
            tranBankTxn = entry.GLTranModuleBatNbr.Update(tranBankTxn);
            tranBankTxn.ProjectID = 0;
            tranBankTxn = entry.GLTranModuleBatNbr.Update(tranBankTxn);
            #endregion
            */

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
        public static PXResultset<EPExpenseClaimDetails> getEPDetails(PXGraph graph, string RefNbr)
        {
            return PXSelect<EPExpenseClaimDetails,
                Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                .Select(graph, RefNbr);
        }
        #endregion

        #region Select Method 
        private static PXResultset<KGExpenseVoucher> getVoucher(PXGraph graph, string RefNbr)
        {
            return PXSelect<KGExpenseVoucher,
                Where<KGExpenseVoucher.epRefNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                .Select(graph, RefNbr);
        }
        private  static PMTask getTask(PXGraph graph, int? ProjectID)
        {
            return PXSelect<PMTask,
                Where<PMTask.projectID, Equal<Required<KGExpenseVoucher.projectID>>>>
                .Select(graph, ProjectID);
        }
        /*
        private static CashAccount GetCashAccountByNMBank(PXGraph graph, int? NMBankAccountID)
        {
            return PXSelectJoin<CashAccount,
                InnerJoin<NMBankAccount, On<NMBankAccount.cashAccountID, Equal<CashAccount.cashAccountID>>>,
                Where<NMBankAccount.bankAccountID, Equal<Required<EPExpenseClaimExt.usrBankAccountID>>>>
                .Select(graph, NMBankAccountID);
;        }
        */
        #endregion
    }
}
