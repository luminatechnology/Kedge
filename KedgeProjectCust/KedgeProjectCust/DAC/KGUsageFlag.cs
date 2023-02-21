using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kedge.DAC
{
    public class KGUsageFlag
    {
        /// <summary>
        /// a.建立新分包契約（啟動電子簽核)
        /// </summary>
        public const string A = "A";

        /// <summary>
        /// b.統購案件(不啟動電子簽核)
        /// </summary>
        public const string B = "B";

        public static readonly string[] Values =
        {
            A, B
        };

        public static readonly string[] Labels =
        {
            "建立新分包契約（啟動電子簽核)",
            "統購案件(不啟動電子簽核)"
        };

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }

        public class a : Constant<string>
        {
            public a() : base(A) { }
        }
        public class b : Constant<string>
        {
            public b() : base(B) { }
        }
    }
}
