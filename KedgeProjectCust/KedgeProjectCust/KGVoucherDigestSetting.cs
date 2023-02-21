using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using Kedge.DAC;
using PX.Objects.GL;

namespace Kedge
{
    public class KGVoucherDigestSetting : PXGraph<KGVoucherDigestSetting>
    {
        public PXSave<KGPostageSetup> Save;
        public PXCancel<KGPostageSetup> Cancel;

        public PXSelect<KGPostageSetup> PostageSetups;
        [PXImport(typeof(KGVoucherDigestSetup))]
        public PXSelect<KGVoucherDigestSetup,
            Where<KGVoucherDigestSetup.setupID,
                Equal<Current<KGPostageSetup.setupID>>>> VoucherDigestSetups;

        #region Event
        protected virtual void KGVoucherDigestSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGVoucherDigestSetup row = (KGVoucherDigestSetup)e.Row;
            if(row.AccountID!=null)
            {
                Account account = GetAccount(row.AccountID);
                row.AccountCD = account.Description;
            }
        }
        protected virtual void KGVoucherDigestSetup_AccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGVoucherDigestSetup row = (KGVoucherDigestSetup)e.Row;
            if (row.AccountID != null)
            {
                Account account = GetAccount(row.AccountID);
                row.AccountCD = account.Description;
            }
        }
        #endregion

        #region Method
        private Account GetAccount(int? AccountID)
        {
            return PXSelect<Account,
                Where<Account.accountID,Equal<Required<Account.accountID>>>>
                .Select(this, AccountID);
        }
        #endregion
    }
}