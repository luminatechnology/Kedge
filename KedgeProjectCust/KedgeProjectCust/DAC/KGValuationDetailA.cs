using System;
using PX.Data;
using PX.Objects.PO;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.IN;

namespace Kedge.DAC
{
    [Serializable]
    public class KGValuationDetailA : KGValuationDetail
    {

        #region PricingType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pricing Type")]
        [PXDefault("A")]
        [PXStringList(new string[]
            {
                "A",
                "D",
            },
            new string[]
            {
                "加款",
                "扣款"
            })]
        public override string PricingType { get; set; }
        //public new abstract class pricingType : IBqlField { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        public override int? InventoryID { get; set; }
        //public new abstract class inventoryID : IBqlField { }
        #endregion

    }
    public static class word
    {
        public const string RO = "RO";
        public const string RS = "RS";
        public const string C = "C";
        public const string N = "N";
        public const string M = "M";

        public const string A = "A";
        public const string B = "B";
        public const string D = "D";
        public const string O = "O";
        public const string OWNER = "OWNER";
        public const string AREACODE = "AREACODE";

        public const string G = "G";
        public const string PE = "專案結算";
        public const string StateD = "63000";
        public const decimal Zero = 0;
        public const string E = "E";
        public const string TT = "TT";
        public const string CHECK = "CHECK";
        public class check : Constant<string>
        {
            public check() : base(CHECK) { }
        }
        public class tt : Constant<string>
        {
            public tt() : base(TT) { }
        }
        public class m : Constant<string>
        {
            public m() : base(M) { }
        }
        public class e : Constant<string>
        {
            public e() : base(E) { }
        }
        public class zero : Constant<decimal>
        {
            public zero() : base(Zero) { }
        }
        public class stateD : Constant<string>
        {
            public stateD() : base(StateD) { }
        }
        public class pE : Constant<string>
        {
            public pE() : base(PE) { }
        }
        public class g : Constant<string>
        {
            public g() : base(G) { }
        }
        public class oWNER : Constant<string>
        {
            public oWNER() : base(OWNER) { }
        }
        public class aREACODE : Constant<string>
        {
            public aREACODE() : base(AREACODE) { }
        }
        public class ro : Constant<string>
        {
            public ro() : base(RO) { }
        }
        public class rs : Constant<string>
        {
            public rs() : base(RS) { }
        }
        public class c : Constant<string>
        {
            public c() : base(C) { }
        }
        public class n : Constant<string>
        {
            public n() : base(N) { }
        }
        public class a : Constant<string>
        {
            public a() : base(A) { }
        }
        public class b : Constant<string>
        {
            public b() : base(B) { }
        }
        public class d : Constant<string>
        {
            public d() : base(D) { }
        }
        public class o : Constant<string>
        {
            public o() : base(O) { }
        }

    }

}