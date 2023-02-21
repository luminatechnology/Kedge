using PX.Data;


namespace FIN.Util
{
    public class FinStringList
    {
        #region WHTFmtCode
        public class WHTFmtCode : PXStringListAttribute
        {
            public WHTFmtCode() : base(Values, Labels) { }
            public static readonly string[] Values =
        {
            XX,XB
        };
            public static readonly string[] Labels =
            {
            "XX","XB"
        };
            /// <summary>
            /// 
            /// </summary>
            public const string XX = "XX";
            /// <summary>
            /// 
            /// </summary>
            public const string XB = "XB";


            public class xx : PX.Data.BQL.BqlString.Constant<xx> { public xx() : base(XX) {; } }
            public class xb : PX.Data.BQL.BqlString.Constant<xb> { public xb() : base(XB) {; } }
        }
        #endregion

        #region WHTFmtSub
        public class WHTFmtSub : PXStringListAttribute
        {
            public WHTFmtSub() : base(Values, Labels) { }
            public static readonly string[] Values =
        {
            XX,XB
        };
            public static readonly string[] Labels =
            {
            "XX","XB"
        };
            /// <summary>
            /// 
            /// </summary>
            public const string XX = "XX";
            /// <summary>
            /// 
            /// </summary>
            public const string XB = "XB";


            public class xx : PX.Data.BQL.BqlString.Constant<xx> { public xx() : base(XX) {; } }
            public class xb : PX.Data.BQL.BqlString.Constant<xb> { public xb() : base(XB) {; } }
        }

        #endregion

        #region TypeOfIn
        public class TypeOfIn : PXStringListAttribute
        {
            public TypeOfIn() : base(Values, Labels) { }
            public static readonly string[] Values =
        {
            One,Two,Three,Four,Five,Six,Seven,Eight,Nine,A
        };
            public static readonly string[] Labels =
            {
                "所得人為本國個人",
                "所得人為事業團體",
                "所得人為在中華民國境內住滿183天之外僑或大陸地區人",
                "所得人為總機構在中華民國境外之在臺分公司",
                "所得人為在中華民國境內未住滿183天之大陸地區人民",
                "所得人為在大陸地區單位",
                "所得人為在中華民國境內未住滿183天之外僑",
                "所得人為總機構在中華民國境外之法人、團體或其他機構",
                "所得人為非屬居住者之本國個人",
                "所得人為在中華民國國籍漁船工作滿183天且無居留證之外僑或大陸地區人"
        };
            /// <summary>
            /// '1' 所得人為本國個人
            /// </summary>
            public const string One = "1";
            /// <summary>
            /// '2' 所得人為事業團體
            /// </summary>
            public const string Two = "2";
            /// <summary>
            ///   '3' 所得人為在中華民國境內住滿183天之外僑或大陸地區人
            /// </summary>
            public const string Three = "3";
            /// <summary>
            ///   '4' '所得人為總機構在中華民國境外之在臺分公司
            /// </summary>
            public const string Four = "4";
            /// <summary>
            ///    '5' '所得人為在中華民國境內未住滿183天之大陸地區人民
            /// </summary>
            public const string Five = "5";
            /// <summary>
            ///    '6' '所得人為在大陸地區單位
            /// </summary>
            public const string Six = "6";
            /// <summary>
            ///    '7' '所得人為在中華民國境內未住滿183天之外僑
            /// </summary>
            public const string Seven = "7";
            /// <summary>
            ///    '8' '所得人為總機構在中華民國境外之法人、團體或其他機構
            /// </summary>
            public const string Eight = "8";
            /// <summary>
            ///    '9' '所得人為非屬居住者之本國個人
            /// </summary>
            public const string Nine = "9";
            /// <summary>
            ///     'A' '所得人為在中華民國國籍漁船工作滿183天且無居留證之外僑或大陸地區人
            /// </summary>
            public const string A = "A";

            public class one : PX.Data.BQL.BqlString.Constant<one> { public one() : base(One) {; } }
            public class two : PX.Data.BQL.BqlString.Constant<two> { public two() : base(Two) {; } }
            public class three : PX.Data.BQL.BqlString.Constant<three> { public three() : base(Three) {; } }
            public class four : PX.Data.BQL.BqlString.Constant<four> { public four() : base(Four) {; } }
            public class five : PX.Data.BQL.BqlString.Constant<five> { public five() : base(Five) {; } }
            public class six : PX.Data.BQL.BqlString.Constant<six> { public six() : base(Six) {; } }
            public class seven : PX.Data.BQL.BqlString.Constant<seven> { public seven() : base(Seven) {; } }
            public class eight : PX.Data.BQL.BqlString.Constant<eight> { public eight() : base(Eight) {; } }
            public class nine : PX.Data.BQL.BqlString.Constant<nine> { public nine() : base(Nine) {; } }
            public class a : PX.Data.BQL.BqlString.Constant<a> { public a() : base(A) {; } }





        }

        #endregion

        #region GNHI2Code
        public class GNHI2Code : PXStringListAttribute
        {
            public GNHI2Code() : base(Values, Labels) { }
            public static readonly string[] Values =
        {
            XX,XB
        };
            public static readonly string[] Labels =
            {
            "XX","XB"
        };
            /// <summary>
            /// 
            /// </summary>
            public const string XX = "XX";
            /// <summary>
            /// 
            /// </summary>
            public const string XB = "XB";


            public class xx : PX.Data.BQL.BqlString.Constant<xx> { public xx() : base(XX) {; } }
            public class xb : PX.Data.BQL.BqlString.Constant<xb> { public xb() : base(XB) {; } }
        }
        #endregion

        #region TranType
        public class TranType : PXStringListAttribute
        {
            public TranType() : base(Values, Labels) { }
            public static readonly string[] Values =
        {
            WHT,LBI,HLI,LBP
        };
            public static readonly string[] Labels =
            {
            "WithHoldingTax","LaborInsurance","HealthInsurance","LaborPension"
        };
            /// <summary>
            /// 代扣稅
            /// </summary>
            public const string WHT = "WHT";
            /// <summary>
            /// 勞保
            /// </summary>
            public const string LBI = "LBI";
            /// <summary>
            /// 健保
            /// </summary>
            public const string HLI = "HLI";
            /// <summary>
            /// 勞動養老金
            /// </summary>
            public const string LBP = "LBP";
            /// <summary>
            /// 健保補充
            /// </summary>
            public const string GHI = "GHI";

            public class wht : PX.Data.BQL.BqlString.Constant<wht> { public wht() : base(WHT) {; } }
            public class lbi : PX.Data.BQL.BqlString.Constant<lbi> { public lbi() : base(LBI) {; } }
            public class hli : PX.Data.BQL.BqlString.Constant<hli> { public hli() : base(HLI) {; } }
            public class lbp : PX.Data.BQL.BqlString.Constant<lbp> { public lbp() : base(LBP) {; } }
            public class ghi : PX.Data.BQL.BqlString.Constant<ghi> { public ghi() : base(GHI) {; } }

        }
        #endregion

        #region Month
        public class Month : PXIntListAttribute
        {
            public Month() : base(Values, Labels) { }
            public static readonly int[] Values =
        {
            Jan,Feb,Mar,Apr,May,June,
            July,Aug,Sept,Oct,Nov,Dec
        };
            public static readonly string[] Labels =
            {
            "1","2","3","4","5","6",
            "7","8","9","10","11","12"
        };
            /// <summary>
            /// 1月
            /// </summary>
            public const int Jan = 1;
            /// <summary>
            /// 2月
            /// </summary>
            public const int Feb = 2;
            /// <summary>
            /// 3月
            /// </summary>
            public const int Mar = 3;
            /// <summary>
            /// 4月
            /// </summary>
            public const int Apr = 4;
            /// <summary>
            /// 5月
            /// </summary>
            public const int May = 5;
            /// <summary>
            /// 6月
            /// </summary>
            public const int June = 6;
            /// <summary>
            /// 7月
            /// </summary>
            public const int July = 7;
            /// <summary>
            /// 8月
            /// </summary>
            public const int Aug = 8;
            /// <summary>
            /// 9月
            /// </summary>
            public const int Sept = 9;
            /// <summary>
            /// 10月
            /// </summary>
            public const int Oct = 10;
            /// <summary>
            /// 11月
            /// </summary>
            public const int Nov = 11;
            /// <summary>
            /// 12月
            /// </summary>
            public const int Dec = 12;

            public class jan : PX.Data.BQL.BqlInt.Constant<jan> { public jan() : base(Jan) {; } }
            public class feb : PX.Data.BQL.BqlInt.Constant<feb> { public feb() : base(Feb) {; } }
            public class mar : PX.Data.BQL.BqlInt.Constant<mar> { public mar() : base(Mar) {; } }
            public class apr : PX.Data.BQL.BqlInt.Constant<apr> { public apr() : base(Apr) {; } }
            public class may : PX.Data.BQL.BqlInt.Constant<may> { public may() : base(May) {; } }
            public class june : PX.Data.BQL.BqlInt.Constant<june> { public june() : base(June) {; } }
            public class july : PX.Data.BQL.BqlInt.Constant<july> { public july() : base(July) {; } }
            public class aug : PX.Data.BQL.BqlInt.Constant<aug> { public aug() : base(Aug) {; } }
            public class sept : PX.Data.BQL.BqlInt.Constant<sept> { public sept() : base(Sept) {; } }
            public class oct : PX.Data.BQL.BqlInt.Constant<oct> { public oct() : base(Oct) {; } }
            public class nov : PX.Data.BQL.BqlInt.Constant<nov> { public nov() : base(Nov) {; } }
            public class dec : PX.Data.BQL.BqlInt.Constant<dec> { public dec() : base(Dec) {; } }

        }
        #endregion
    }
}
