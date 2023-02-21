using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NM.DAC;
using PX.Data;
using PX.Objects.GL;

namespace NM.Util
{
    public class NMAccountAttribute : PXSelectorAttribute
    {
        public NMAccountAttribute()
            : base(
                typeof(Search<Account.accountID>),
                typeof(Account.accountCD),
                typeof(Account.description)            
                )
        {
            SubstituteKey = typeof(Account.accountCD);
            DescriptionField = typeof(Account.description);
        }

    }
}
