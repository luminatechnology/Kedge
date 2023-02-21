using NM.DAC;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NM.Util
{
    //NMBankAccount 所有有維護的銀行
    public class NMBankAccountAttribute : PXSelectorAttribute
    {
        public NMBankAccountAttribute()
            : base(
                typeof(Search<NMBankAccount.bankAccountID, Where<NMBankAccount.isSettlement, Equal<False>>>),
                typeof(NMBankAccount.bankAccountCD),
                typeof(NMBankAccount.bankName),
                typeof(NMBankAccount.bankShortName),
                typeof(NMBankAccount.bankCode),
                typeof(NMBankAccount.bankAccount),
                typeof(NMBankAccount.enableIssueByBank)
                )
        {
            SubstituteKey = typeof(NMBankAccount.bankAccountCD);
            DescriptionField = typeof(NMBankAccount.bankName);
        }

    }

}
