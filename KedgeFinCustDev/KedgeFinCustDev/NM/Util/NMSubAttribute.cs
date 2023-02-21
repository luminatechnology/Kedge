using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.GL;

namespace NM.Util
{
    public class NMSubAttribute : PXSelectorAttribute
    {
        public NMSubAttribute()
            : base(
                typeof(Search<Sub.subID>),
                typeof(Sub.subCD),
                typeof(Sub.description)
                )
        {
            SubstituteKey = typeof(Sub.subCD);
            DescriptionField = typeof(Sub.description);
        }

    }
}
