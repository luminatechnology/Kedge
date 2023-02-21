using System;
using PX.Data;
using PX.Objects.CA;
using NM.DAC;
using RC.Util;

namespace NM
{
    public class NMBankAccountMaint : PXGraph<NMBankAccountMaint, NMBankAccount>
    {

        public NMBankAccountMaint()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_RECEIVABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        #region Selects
        public PXSelect<NMBankAccount> BankAccounts;


        #endregion


        #region Events
        protected virtual void NMBankAccount_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMBankAccount bank = (NMBankAccount)e.Row;

            CashAccount cashAccount = PXSelectorAttribute.Select<NMBankAccount.cashAccountID>(sender, bank) as CashAccount;

            if (bank.CashAccountID != null)
            {
                if (bank.PaymentMethodID != null)
                {
                    bank.PaymentMethodID = null;
                }
                bank.AccountID = cashAccount.AccountID;
                bank.SubID = cashAccount.SubID;
                bank.CuryID = cashAccount.CuryID;
            }
            else
            {
                bank.AccountID = null;
                bank.SubID = null;
                bank.CuryID = null;
                bank.PaymentMethodID = null;
            }
        }
        protected void _(Events.RowPersisting<NMBankAccount> e)
        {
            NMBankAccount row = (NMBankAccount)e.Row;
            if (row == null) return;
            if (BankAccounts.Cache.GetStatus(row) != PXEntryStatus.Inserted) return;
            NMBankAccount _ba = GetBankAccountByPC(row.PaymentMethodID, row.CashAccountID,row.BankAccountID);
            if (_ba != null)
            {
                CashAccount ca = (CashAccount)PXSelectorAttribute.Select<NMBankAccount.cashAccountID>(BankAccounts.Cache, row);
                e.Cancel = true;
                String error = String.Format(
                    "付款方式：{0} & 現金帳戶：{1}，已存在於 {2}"
                    , row.PaymentMethodID, ca.CashAccountCD, _ba.BankAccountCD);
                BankAccounts.Cache.RaiseExceptionHandling<NMBankAccount.paymentMethodID>(
                            row, row.PaymentMethodID, new PXSetPropertyException(error, PXErrorLevel.Error));
                BankAccounts.Cache.RaiseExceptionHandling<NMBankAccount.cashAccountID>(
                            row, row.CashAccountID, new PXSetPropertyException(error, PXErrorLevel.Error));
            }
        }


        protected void _(Events.RowSelected<NMBankAccount> e)
        {
            //此功能不得刪除
            Delete.SetEnabled(false);
            NMBankAccount row = e.Row as NMBankAccount;

            if (row == null) return;
            //必填欄位只能在insert的時候可輸入
            bool isInsert = BankAccounts.Cache.GetStatus(row) == PXEntryStatus.Inserted;
            //PXUIFieldAttribute.SetEnabled<NMBankAccount.bankName>(BankAccounts.Cache, row, isInsert);
            //PXUIFieldAttribute.SetEnabled<NMBankAccount.bankShortName>(BankAccounts.Cache, row, isInsert);
            PXUIFieldAttribute.SetEnabled<NMBankAccount.activationDate>(BankAccounts.Cache, row, isInsert);
            //PXUIFieldAttribute.SetEnabled<NMBankAccount.accountType>(BankAccounts.Cache, row, isInsert);
            //PXUIFieldAttribute.SetEnabled<NMBankAccount.bankCode>(BankAccounts.Cache, row, isInsert);
            //PXUIFieldAttribute.SetEnabled<NMBankAccount.bankAccount>(BankAccounts.Cache, row, isInsert);
            //PXUIFieldAttribute.SetEnabled<NMBankAccount.accountName>(BankAccounts.Cache, row, isInsert);
            PXUIFieldAttribute.SetEnabled<NMBankAccount.cashAccountID>(BankAccounts.Cache, row, isInsert);
            PXUIFieldAttribute.SetEnabled<NMBankAccount.paymentMethodID>(BankAccounts.Cache, row, isInsert);
            //PXUIFieldAttribute.SetEnabled<NMBankAccount.curyID>(BankAccounts.Cache, row, isInsert);
        }
        protected void _(Events.FieldUpdated<NMBankAccount, NMBankAccount.isSettlement> e)
        {
            NMBankAccount row = e.Row as NMBankAccount;
            if (row == null) return;
            row.SettlementDate = (row.IsSettlement ?? false) ? this.Accessinfo.BusinessDate : null;
        }

        #endregion

        #region Mathod
        public virtual NMBankAccount GetBankAccountByPC(string paymentMethodID, int? cashAccountID, int? notEqID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>,
                And<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.bankAccountID, NotEqual<Required<NMBankAccount.bankAccountID>>>>>>
                .Select(this, paymentMethodID, cashAccountID, notEqID);
        }

        #endregion

    }
}