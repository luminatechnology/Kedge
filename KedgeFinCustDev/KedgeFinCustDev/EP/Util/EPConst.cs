using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.EP;

namespace PX.Objects.EP
{
    public class EPConst
    {
        public const string VENDOR = "VE";
        public const string ACTIVE = "A";
        public const decimal Negate1 = -1m;
        public class vendor : PX.Data.BQL.BqlString.Constant<vendor> { public vendor() : base(VENDOR) {; } }
        public class active : PX.Data.BQL.BqlString.Constant<active> { public active() : base(ACTIVE) {; } }
        public class negate1 : PX.Data.BQL.BqlDecimal.Constant<negate1> { public negate1() : base(Negate1) {; } }

    }
}
