using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCGV.GV.Util
{
    public class GVList
    {
        #region GVTaxCode
        public class GVTaxCode : PXStringListAttribute
        {
            public GVTaxCode() : base(new[]
                {
                    Pair(TAXABLE, "應稅"),
                    Pair(ZEROTAX, "零稅"),
                    Pair(FREETAX, "免稅")
                })
            { }

            /// <summary> 應稅 </summary>
            public const string TAXABLE = "1";
            /// <summary> 零稅 </summary>
            public const string ZEROTAX = "2";
            /// <summary> 免稅 </summary>
            public const string FREETAX = "3";

            /// <summary> 應稅 </summary>
            public class taxable : PX.Data.BQL.BqlString.Constant<taxable> { public taxable() : base(TAXABLE) {; } }
            /// <summary> 免稅 </summary>
            public class freetax : PX.Data.BQL.BqlString.Constant<freetax> { public freetax() : base(FREETAX) {; } }
            /// <summary> 零稅 </summary>
            public class zerotax : PX.Data.BQL.BqlString.Constant<zerotax> { public zerotax() : base(ZEROTAX) {; } }
        }
        #endregion

        #region GVStatus_APInv
        public class GVStatusAPInv_APInv : GVStatus
        {
            public GVStatusAPInv_APInv() : base(new[]
            {
                Pair(OPEN, "Open"),
                Pair(VOIDINVOICE, "Return"),
                Pair(GENERATEXML, "Generate XML"),
                Pair(TRUNKEYSENDOK, "Trunkey send OK"),
                Pair(TRUNKEYSENDERROR, "Trunkey send error"),
                Pair(TRUNKEYRETURNOK, "Trunkey return OK"),
                Pair(TRUNKEYRETURNERROR, "Trunkey return error"),
                Pair(HOLD, "Hold"),
                Pair(RELEASE, "Release")
            })
            { }
        }
        #endregion

        #region GVStatus
        public class GVStatus : PXStringListAttribute
        {
            public GVStatus() : base(new[]
            {
                Pair(OPEN, "Open"),
                Pair(VOIDINVOICE, "Void"),
                Pair(GENERATEXML, "Generate XML"),
                Pair(TRUNKEYSENDOK, "Trunkey send OK"),
                Pair(TRUNKEYSENDERROR, "Trunkey send error"),
                Pair(TRUNKEYRETURNOK, "Trunkey return OK"),
                Pair(TRUNKEYRETURNERROR, "Trunkey return error"),
                Pair(HOLD, "Hold"),
                Pair(RELEASE, "Release")
            })
            { }
            protected GVStatus(params Tuple<string, string>[] valuesToLabels) : base(valuesToLabels)
            {
            }

            /// <summary> Open </summary>
            public const string OPEN = "1";
            /// <summary> Void/Return </summary>
            public const string VOIDINVOICE = "2";
            /// <summary> Generate XML </summary>
            public const string GENERATEXML = "3";
            /// <summary> Trunkey send OK </summary>
            public const string TRUNKEYSENDOK = "4";
            /// <summary> Trunkey send error </summary>
            public const string TRUNKEYSENDERROR = "5";
            /// <summary> Trunkey return OK </summary>
            public const string TRUNKEYRETURNOK = "6";
            /// <summary> Trunkey return error </summary>
            public const string TRUNKEYRETURNERROR = "7";
            /// <summary> Hold </summary>
            public const string HOLD = "8";
            /// <summary> Release </summary>
            public const string RELEASE = "9";

            /// <summary> Open </summary>
            public class open : PX.Data.BQL.BqlString.Constant<open> { public open() : base(OPEN) {; } }
            /// <summary> Void/Return </summary>
            public class voidinvoice : PX.Data.BQL.BqlString.Constant<voidinvoice> { public voidinvoice() : base(VOIDINVOICE) {; } }
            /// <summary> Generate XML </summary>
            public class generatexml : PX.Data.BQL.BqlString.Constant<generatexml> { public generatexml() : base(GENERATEXML) {; } }
            /// <summary> Trunkey send OK </summary>
            public class trunkeysendok : PX.Data.BQL.BqlString.Constant<trunkeysendok> { public trunkeysendok() : base(TRUNKEYSENDOK) {; } }
            /// <summary> Trunkey send error </summary>
            public class trunkeysenderror : PX.Data.BQL.BqlString.Constant<trunkeysenderror> { public trunkeysenderror() : base(TRUNKEYSENDERROR) {; } }
            /// <summary> Trunkey return OK </summary>
            public class trunkeyreturnok : PX.Data.BQL.BqlString.Constant<trunkeyreturnok> { public trunkeyreturnok() : base(TRUNKEYRETURNOK) {; } }
            /// <summary> Trunkey return error </summary>
            public class trunkeyreturnerror : PX.Data.BQL.BqlString.Constant<trunkeyreturnerror> { public trunkeyreturnerror() : base(TRUNKEYRETURNERROR) {; } }
            /// <summary> Hold </summary>
            public class hold : PX.Data.BQL.BqlString.Constant<hold> { public hold() : base(HOLD) {; } }
            /// <summary> Release </summary>
            public class release : PX.Data.BQL.BqlString.Constant<release> { public release() : base(RELEASE) {; } }

        }
        #endregion

        #region GVGuiInvoiceType
        public class GVGuiInvoiceType : PXStringListAttribute
        {
            public GVGuiInvoiceType() : base(new[]
                {
                    Pair(INVOICE, "發票"),
                    Pair(RECEIPT, "收據")
                })
            { }
            /// <summary> 發票 </summary>
            public const string INVOICE = "I";
            /// <summary> 收據 </summary>
            public const string RECEIPT = "R";

            /// <summary> 發票 </summary>
            public class invoice : PX.Data.BQL.BqlString.Constant<invoice> { public invoice() : base(INVOICE) {; } }
            /// <summary> 收據 </summary>
            public class receipt : PX.Data.BQL.BqlString.Constant<receipt> { public receipt() : base(RECEIPT) {; } }
        }
        #endregion

        #region GVGuiGroupRemark
        public class GVGuiGroupRemark : PXStringListAttribute
        {
            public GVGuiGroupRemark() : base(new[]
            {
                Pair(UNSUMMARY, "非彙加資料"),
                Pair(SUMMARY, "彙加資料"),
                Pair(APPORTIONMENT, "分攤資料")
            })
            { }
            /// <summary> 非彙加資料 </summary>
            public const string UNSUMMARY = "0";
            /// <summary> 彙加資料 </summary>
            public const string SUMMARY = "A";
            /// <summary> 分攤資料 </summary>
            public const string APPORTIONMENT = "B";

            /// <summary> 非彙加資料 </summary>
            public class unsummary : PX.Data.BQL.BqlString.Constant<unsummary> { public unsummary() : base(UNSUMMARY) {; } }
            /// <summary> 彙加資料 </summary>
            public class summary : PX.Data.BQL.BqlString.Constant<summary> { public summary() : base(SUMMARY) {; } }
            /// <summary> 分攤資料 </summary>
            public class apportionment : PX.Data.BQL.BqlString.Constant<apportionment> { public apportionment() : base(APPORTIONMENT) {; } }

        }
        #endregion

        #region GVGuiVoucherCategory
        public class GVGuiVoucherCategory : PXStringListAttribute
        {
            public GVGuiVoucherCategory() : base(new[]
            {
                Pair(CUSTOMPROXYTAX, "海關代徵營業稅繳納證"),
                Pair(TAXABLE, "發票(含稅)"),
                Pair(OTHERCERTIFICATE, "其他憑證"),
                Pair(ZEROTAX, "零稅")
            })
            { }
            /// <summary> 海關代徵營業稅繳納證 </summary>
            public const string CUSTOMPROXYTAX = "C";
            /// <summary> 發票(含稅) </summary>
            public const string TAXABLE = "I";
            /// <summary> 其他憑證 </summary>
            public const string OTHERCERTIFICATE = "R";
            /// <summary> 零稅 </summary>
            public const string ZEROTAX = "Q";

            /// <summary> 海關代徵營業稅繳納證 </summary>
            public class customproxytax : PX.Data.BQL.BqlString.Constant<customproxytax> { public customproxytax() : base(CUSTOMPROXYTAX) {; } }
            /// <summary> 發票(含稅) </summary>
            public class taxable : PX.Data.BQL.BqlString.Constant<taxable> { public taxable() : base(TAXABLE) {; } }
            /// <summary> 其他憑證 </summary>
            public class othercertificate : PX.Data.BQL.BqlString.Constant<othercertificate> { public othercertificate() : base(OTHERCERTIFICATE) {; } }
            /// <summary> 零稅 </summary>
            public class zerotax : PX.Data.BQL.BqlString.Constant<zerotax> { public zerotax() : base(ZEROTAX) {; } }
        }
        #endregion

        #region GVGuiDeductionCode
        public class GVGuiDeductionCode : PXStringListAttribute
        {
            public GVGuiDeductionCode() : base(new[]
            {
                Pair(DEDUCTIONCODE1, "進項可扣抵之進貨及費用"),
                Pair(DEDUCTIONCODE2, "進項可扣抵之固定資產"),
                Pair(DEDUCTIONCODE3, "進項不可扣抵之進貨及費用"),
                Pair(DEDUCTIONCODE4, "進項不可扣抵之固定資產")
            })
            { }
            /// <summary> 進項可扣抵之進貨及費用 </summary>
            public const string DEDUCTIONCODE1 = "1";
            /// <summary> 進項可扣抵之固定資產 </summary>
            public const string DEDUCTIONCODE2 = "2";
            /// <summary> 進項不可扣抵之進貨及費用 </summary>
            public const string DEDUCTIONCODE3 = "3";
            /// <summary> 進項不可扣抵之固定資產 </summary>
            public const string DEDUCTIONCODE4 = "4";

            /// <summary> 進項可扣抵之進貨及費用 </summary>
            public class deductioncode1 : PX.Data.BQL.BqlString.Constant<deductioncode1> { public deductioncode1() : base(DEDUCTIONCODE1) {; } }
            /// <summary> 進項可扣抵之固定資產 </summary>
            public class deductioncode2 : PX.Data.BQL.BqlString.Constant<deductioncode2> { public deductioncode2() : base(DEDUCTIONCODE2) {; } }
            /// <summary> 進項不可扣抵之進貨及費用 </summary>
            public class deductioncode3 : PX.Data.BQL.BqlString.Constant<deductioncode3> { public deductioncode3() : base(DEDUCTIONCODE3) {; } }
            /// <summary> 進項不可扣抵之固定資產 </summary>
            public class deductioncode4 : PX.Data.BQL.BqlString.Constant<deductioncode4> { public deductioncode4() : base(DEDUCTIONCODE4) {; } }
        }
        #endregion

        #region GVGuiType
        public class GVGuiType : PXStringListAttribute
        {
            #region GVGuiType
            public GVGuiType() : base(new[]
                {
                    TGuiType_21,
                    TGuiType_22,
                    TGuiType_23,
                    TGuiType_24,
                    TGuiType_25,
                    TGuiType_26,
                    TGuiType_27,
                    TGuiType_28,
                    TGuiType_29,
                    TGuiType_31,
                    TGuiType_32,
                    TGuiType_33,
                    TGuiType_34,
                    TGuiType_35,
                    TGuiType_36,
                    TGuiType_37,
                    TGuiType_38
                })
            { }
            #endregion

            #region APCmInvoice
            /// <summary>
            /// 折讓-23、24、29
            /// </summary>
            public class APCmInvoice : PXStringListAttribute
            {
                public APCmInvoice() : base(new[]
                {
                    TGuiType_23,
                    TGuiType_24,
                    TGuiType_29
                })
                { }
            }
            #endregion

            #region APInvoice
            /// <summary>
            /// 發票-21、22、25、26、27、28
            /// </summary>
            public class APInvoice : PXStringListAttribute
            {
                public APInvoice() : base(new[]
                {
                    TGuiType_21,
                    TGuiType_22,
                    TGuiType_25,
                    TGuiType_26,
                    TGuiType_27,
                    TGuiType_28
                })
                { }
            }
            #endregion

            #region ARCmInvoice
            /// <summary>
            /// 折讓-33、34、38
            /// </summary>
            public class ARCmInvoice : PXStringListAttribute
            {
                public ARCmInvoice() : base(new[]
                {
                    TGuiType_33,
                    TGuiType_34,
                    TGuiType_38
                })
                { }
            }
            #endregion

            #region ARInvoice
            /// <summary>
            /// 發票-31、32、35、36、37
            /// </summary>
            public class ARInvoice : PXStringListAttribute
            {
                public ARInvoice() : base(new[]
                {
                    TGuiType_31,
                    TGuiType_32,
                    TGuiType_35,
                    TGuiType_36,
                    TGuiType_37
                })
                { }
            }
            #endregion

            #region Tuple
            public static Tuple<String, String> TGuiType_21 = Pair(AP.GuiType_21, "21-三聯式、電子計算機統一發票");
            public static Tuple<String, String> TGuiType_22 = Pair(AP.GuiType_22, "22-二聯式收銀機統一發票、載有稅額之其他憑證");
            public static Tuple<String, String> TGuiType_23 = Pair(AP.GuiType_23, "23-三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之進貨退出或折讓證明單");
            public static Tuple<String, String> TGuiType_24 = Pair(AP.GuiType_24, "24-二聯式收銀機統一發票及載有稅額之其他憑證之進貨退出或折讓證明單");
            public static Tuple<String, String> TGuiType_25 = Pair(AP.GuiType_25, "25-三聯式收銀機統一發票及一般稅額計算之電子發票");
            public static Tuple<String, String> TGuiType_26 = Pair(AP.GuiType_26, "26-彙總登錄每張稅額伍佰元以下之進項三聯式、電子計算機統一發票");
            public static Tuple<String, String> TGuiType_27 = Pair(AP.GuiType_27, "27-彙總登錄每張稅額伍佰元以下之進項二聯式收銀機統一發票、載有稅額之其他憑證");
            public static Tuple<String, String> TGuiType_28 = Pair(AP.GuiType_28, "28-海關代徵營業稅繳納證");
            public static Tuple<String, String> TGuiType_29 = Pair(AP.GuiType_29, "29-海關退還溢繳營業稅申報單");
            public static Tuple<String, String> TGuiType_31 = Pair(AR.GuiType_31, "31-三聯式、電子計算機統一發票");
            public static Tuple<String, String> TGuiType_32 = Pair(AR.GuiType_32, "32-二聯式、二聯式收銀機統一發票");
            public static Tuple<String, String> TGuiType_33 = Pair(AR.GuiType_33, "33-三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之銷貨退回或折讓證明單");
            public static Tuple<String, String> TGuiType_34 = Pair(AR.GuiType_34, "34-二聯式、二聯式收銀機統一發票及銷項免用統一發票之銷貨退回或折讓證明單");
            public static Tuple<String, String> TGuiType_35 = Pair(AR.GuiType_35, "35-三聯式收銀機統一發票及一般稅額計算之電子發票");
            public static Tuple<String, String> TGuiType_36 = Pair(AR.GuiType_36, "36-免用統一發票");
            public static Tuple<String, String> TGuiType_37 = Pair(AR.GuiType_37, "37-特種稅額銷項憑證、特種稅額計算之電子發票");
            public static Tuple<String, String> TGuiType_38 = Pair(AR.GuiType_38, "38-特種稅額銷貨退回或折讓證明單");
            #endregion

            #region const
            #region Ap
            public class AP
            {
                /// <summary> 21-三聯式、電子計算機統一發票 </summary>
                public const string GuiType_21 = "21";
                /// <summary> 22-二聯式收銀機統一發票、載有稅額之其他憑證 </summary>
                public const string GuiType_22 = "22";
                /// <summary> 23-三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之進貨退出或折讓證明單 </summary>
                public const string GuiType_23 = "23";
                /// <summary> 24-二聯式收銀機統一發票及載有稅額之其他憑證之進貨退出或折讓證明單 </summary>
                public const string GuiType_24 = "24";
                /// <summary> 25-三聯式收銀機統一發票及一般稅額計算之電子發票 </summary>
                public const string GuiType_25 = "25";
                /// <summary> 26-彙總登錄每張稅額伍佰元以下之進項三聯式、電子計算機統一發票 </summary>
                public const string GuiType_26 = "26";
                /// <summary> 27-彙總登錄每張稅額伍佰元以下之進項二聯式收銀機統一發票、載有稅額之其他憑證 </summary>
                public const string GuiType_27 = "27";
                /// <summary> 28-海關代徵營業稅繳納證 </summary>
                public const string GuiType_28 = "28";
                /// <summary> 29-海關退還溢繳營業稅申報單 </summary>
                public const string GuiType_29 = "29";

                /// <summary> 21-三聯式、電子計算機統一發票 </summary>
                public class guiType_21 : PX.Data.BQL.BqlString.Constant<guiType_21> { public guiType_21() : base(GuiType_21) {; } }
                /// <summary> 22-二聯式收銀機統一發票、載有稅額之其他憑證 </summary>
                public class guiType_22 : PX.Data.BQL.BqlString.Constant<guiType_22> { public guiType_22() : base(GuiType_22) {; } }
                /// <summary> 23-三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之進貨退出或折讓證明單 </summary>
                public class guiType_23 : PX.Data.BQL.BqlString.Constant<guiType_23> { public guiType_23() : base(GuiType_23) {; } }
                /// <summary> 24-二聯式收銀機統一發票及載有稅額之其他憑證之進貨退出或折讓證明單 </summary>
                public class guiType_24 : PX.Data.BQL.BqlString.Constant<guiType_24> { public guiType_24() : base(GuiType_24) {; } }
                /// <summary> 25-三聯式收銀機統一發票及一般稅額計算之電子發票 </summary>
                public class guiType_25 : PX.Data.BQL.BqlString.Constant<guiType_25> { public guiType_25() : base(GuiType_25) {; } }
                /// <summary> 26-彙總登錄每張稅額伍佰元以下之進項三聯式、電子計算機統一發票 </summary>
                public class guiType_26 : PX.Data.BQL.BqlString.Constant<guiType_26> { public guiType_26() : base(GuiType_26) {; } }
                /// <summary> 27-彙總登錄每張稅額伍佰元以下之進項二聯式收銀機統一發票、載有稅額之其他憑證 </summary>
                public class guiType_27 : PX.Data.BQL.BqlString.Constant<guiType_27> { public guiType_27() : base(GuiType_27) {; } }
                /// <summary> 28-海關代徵營業稅繳納證 </summary>
                public class guiType_28 : PX.Data.BQL.BqlString.Constant<guiType_28> { public guiType_28() : base(GuiType_28) {; } }
                /// <summary> 29-海關退還溢繳營業稅申報單 </summary>
                public class guiType_29 : PX.Data.BQL.BqlString.Constant<guiType_29> { public guiType_29() : base(GuiType_29) {; } }
            }
            #endregion

            #region AR
            public class AR
            {
                /// <summary> 31-三聯式、電子計算機統一發票 </summary>
                public const string GuiType_31 = "31";
                /// <summary> 32-二聯式、二聯式收銀機統一發票 </summary>
                public const string GuiType_32 = "32";
                /// <summary> 33-三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之銷貨退回或折讓證明單 </summary>
                public const string GuiType_33 = "33";
                /// <summary> 34-二聯式、二聯式收銀機統一發票及銷項免用統一發票之銷貨退回或折讓證明單 </summary>
                public const string GuiType_34 = "34";
                /// <summary> 35-三聯式收銀機統一發票及一般稅額計算之電子發票 </summary>
                public const string GuiType_35 = "35";
                /// <summary> 36-免用統一發票 </summary>
                public const string GuiType_36 = "36";
                /// <summary> 37-特種稅額銷項憑證、特種稅額計算之電子發票 </summary>
                public const string GuiType_37 = "37";
                /// <summary> 38-特種稅額銷貨退回或折讓證明單 </summary>
                public const string GuiType_38 = "38";

                /// <summary> 31-三聯式、電子計算機統一發票 </summary>
                public class guiType_31 : PX.Data.BQL.BqlString.Constant<guiType_31> { public guiType_31() : base(GuiType_31) {; } }
                /// <summary> 32-二聯式、二聯式收銀機統一發票 </summary>
                public class guiType_32 : PX.Data.BQL.BqlString.Constant<guiType_32> { public guiType_32() : base(GuiType_32) {; } }
                /// <summary> 33-三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之銷貨退回或折讓證明單 </summary>
                public class guiType_33 : PX.Data.BQL.BqlString.Constant<guiType_33> { public guiType_33() : base(GuiType_33) {; } }
                /// <summary> 34-二聯式、二聯式收銀機統一發票及銷項免用統一發票之銷貨退回或折讓證明單 </summary>
                public class guiType_34 : PX.Data.BQL.BqlString.Constant<guiType_34> { public guiType_34() : base(GuiType_34) {; } }
                /// <summary> 35-三聯式收銀機統一發票及一般稅額計算之電子發票 </summary>
                public class guiType_35 : PX.Data.BQL.BqlString.Constant<guiType_35> { public guiType_35() : base(GuiType_35) {; } }
                /// <summary> 36-免用統一發票 </summary>
                public class guiType_36 : PX.Data.BQL.BqlString.Constant<guiType_36> { public guiType_36() : base(GuiType_36) {; } }
                /// <summary> 37-特種稅額銷項憑證、特種稅額計算之電子發票 </summary>
                public class guiType_37 : PX.Data.BQL.BqlString.Constant<guiType_37> { public guiType_37() : base(GuiType_37) {; } }
                /// <summary> 38-特種稅額銷貨退回或折讓證明單 </summary>
                public class guiType_38 : PX.Data.BQL.BqlString.Constant<guiType_38> { public guiType_38() : base(GuiType_38) {; } }
            }
            #endregion

            #endregion
        }

        #endregion

        #region GVGuiDeclareMonth
        public class GVGuiDeclareMonth : PXIntListAttribute
        {
            public GVGuiDeclareMonth() : base(new[]
            {
                Pair(M1, "1月"),
                Pair(M2, "2月"),
                Pair(M3, "3月"),
                Pair(M4, "4月"),
                Pair(M5, "5月"),
                Pair(M6, "6月"),
                Pair(M7, "7月"),
                Pair(M8, "8月"),
                Pair(M9, "9月"),
                Pair(M10, "10月"),
                Pair(M11, "11月"),
                Pair(M12, "12月")
            })
            { }
            #region const
            /// <summary> 1月 </summary>
            public const int M1 = 1;
            /// <summary> 2月 </summary>
            public const int M2 = 2;
            /// <summary> 3月 </summary>
            public const int M3 = 3;
            /// <summary> 4月 </summary>
            public const int M4 = 4;
            /// <summary> 5月 </summary>
            public const int M5 = 5;
            /// <summary> 6月 </summary>
            public const int M6 = 6;
            /// <summary> 7月 </summary>
            public const int M7 = 7;
            /// <summary> 8月 </summary>
            public const int M8 = 8;
            /// <summary> 9月 </summary>
            public const int M9 = 9;
            /// <summary> 10月 </summary>
            public const int M10 = 10;
            /// <summary> 11月 </summary>
            public const int M11 = 11;
            /// <summary> 12月 </summary>
            public const int M12 = 12;

            /// <summary> 1月 </summary>
            public class m1 : PX.Data.BQL.BqlInt.Constant<m1> { public m1() : base(M1) {; } }
            /// <summary> 2月 </summary>
            public class m2 : PX.Data.BQL.BqlInt.Constant<m2> { public m2() : base(M2) {; } }
            /// <summary> 3月 </summary>
            public class m3 : PX.Data.BQL.BqlInt.Constant<m3> { public m3() : base(M3) {; } }
            /// <summary> 4月 </summary>
            public class m4 : PX.Data.BQL.BqlInt.Constant<m4> { public m4() : base(M4) {; } }
            /// <summary> 5月 </summary>
            public class m5 : PX.Data.BQL.BqlInt.Constant<m5> { public m5() : base(M5) {; } }
            /// <summary> 6月 </summary>
            public class m6 : PX.Data.BQL.BqlInt.Constant<m6> { public m6() : base(M6) {; } }
            /// <summary> 7月 </summary>
            public class m7 : PX.Data.BQL.BqlInt.Constant<m7> { public m7() : base(M7) {; } }
            /// <summary> 8月 </summary>
            public class m8 : PX.Data.BQL.BqlInt.Constant<m8> { public m8() : base(M8) {; } }
            /// <summary> 9月 </summary>
            public class m9 : PX.Data.BQL.BqlInt.Constant<m9> { public m9() : base(M9) {; } }
            /// <summary> 10月 </summary>
            public class m10 : PX.Data.BQL.BqlInt.Constant<m10> { public m10() : base(M10) {; } }
            /// <summary> 11月 </summary>
            public class m11 : PX.Data.BQL.BqlInt.Constant<m11> { public m11() : base(M11) {; } }
            /// <summary> 12月 </summary>
            public class m12 : PX.Data.BQL.BqlInt.Constant<m12> { public m12() : base(M12) {; } }
            #endregion
        }
        #endregion

        #region GVEgvType
        public class GVEgvType : PXStringListAttribute
        {
            public GVEgvType() : base(new[]
            {
               Pair(B2C, "B2C"),
               Pair(B2BC, "B2B交換"),
               Pair(B2BV, "B2B憑證")
            })
            { }
            #region const
            /// <summary> B2C </summary>
            public const string B2C = "1";
            /// <summary> B2B交換 </summary>
            public const string B2BC = "2";
            /// <summary> B2B憑證 </summary>
            public const string B2BV = "3";

            /// <summary> B2C </summary>
            public class b2c : PX.Data.BQL.BqlString.Constant<b2c> { public b2c() : base(B2C) {; } }
            /// <summary> B2B交換 </summary>
            public class b2bC : PX.Data.BQL.BqlString.Constant<b2bC> { public b2bC() : base(B2BC) {; } }
            /// <summary> B2B憑證 </summary>
            public class b2bV : PX.Data.BQL.BqlString.Constant<b2bV> { public b2bV() : base(B2BV) {; } }
            #endregion
        }
        #endregion

        #region GVDeclarePeriod
        public class GVDeclarePeriod : PXStringListAttribute
        {
            public GVDeclarePeriod() : base(new[]
            {
                Pair(M12, "1-2月"),
                Pair(M34, "3-4月"),
                Pair(M56, "5-6月"),
                Pair(M78, "7-8月"),
                Pair(M910, "9-10月"),
                Pair(M1112, "11-12月")
            })
            { }


            public static string GetDeclarePeriodKey(int month)
            {
                int period = (month / 2) + (month % 2 > 0 ? 1 : 0);
                return period.ToString();
            }
            public static int GetMaxMonth(string gvDeclarePeriod)
            {
                return int.Parse(gvDeclarePeriod) * 2;
            }
            public static int GetMinMonth(string gvDeclarePeriod)
            {
                return int.Parse(gvDeclarePeriod);
            }

            #region const
            /// <summary> 1-2月 </summary>
            public const string M12 = "1";
            /// <summary> 3-4月 </summary>
            public const string M34 = "2";
            /// <summary> 5-6月 </summary>
            public const string M56 = "3";
            /// <summary> 7-8月 </summary>
            public const string M78 = "4";
            /// <summary> 9-10月 </summary>
            public const string M910 = "5";
            /// <summary> 11-12月 </summary>
            public const string M1112 = "6";

            /// <summary> 1-2月 </summary>
            public class m12 : PX.Data.BQL.BqlString.Constant<m12> { public m12() : base(M12) {; } }
            /// <summary> 3-4月 </summary>
            public class m34 : PX.Data.BQL.BqlString.Constant<m34> { public m34() : base(M34) {; } }
            /// <summary> 5-6月 </summary>
            public class m56 : PX.Data.BQL.BqlString.Constant<m56> { public m56() : base(M56) {; } }
            /// <summary> 7-8月 </summary>
            public class m78 : PX.Data.BQL.BqlString.Constant<m78> { public m78() : base(M78) {; } }
            /// <summary> 9-10月 </summary>
            public class m910 : PX.Data.BQL.BqlString.Constant<m910> { public m910() : base(M910) {; } }
            /// <summary> 11-12月 </summary>
            public class m1112 : PX.Data.BQL.BqlString.Constant<m1112> { public m1112() : base(M1112) {; } }
            #endregion
        }

        #endregion

        #region GVSpecialTaxType
        public class GVSpecialTaxType : PXStringListAttribute
        {
            public GVSpecialTaxType() : base(new[]
            {
                Pair(SpecialTaxType_0, "非銷項特種稅額"),
                Pair(SpecialTaxType_1, "酒家及有陪侍服務之茶室、咖啡廳、酒吧等之營業稅稅率"),
                Pair(SpecialTaxType_2, "夜總會、有娛樂節目之餐飲店之營業稅稅率"),
                Pair(SpecialTaxType_3, "銀行業、保險業、信託投資業、證券業、期貨業、票券業及典當業之專屬本業收入（不含銀行業、保險  業經營銀行、保險本業收入）營業稅稅率"),
                Pair(SpecialTaxType_4, "保險業之再保費收入之營業稅稅率"),
                Pair(SpecialTaxType_5, "銀行業、保險業、信託投資業、證券業、期貨業、 票券業及典當業之非專屬本業收入營業稅稅率"),
                Pair(SpecialTaxType_6, "銀行業、保險業經營銀行、保險本業收入之營業稅稅率（適用於一百零三年七月以後銷售額）"),
                Pair(SpecialTaxType_7, "銀行業、保險業經營銀行、保險本業收入之營業稅稅率（適用於一百零三年六月以前銷售額）")
            })
            { }

            /// <summary> 非銷項特種稅額 </summary>
            public const string SpecialTaxType_0 = "0";
            /// <summary> 酒家及有陪侍服務之茶室、咖啡廳、酒吧等之營業稅稅率 </summary>
            public const string SpecialTaxType_1 = "1";
            /// <summary> 夜總會、有娛樂節目之餐飲店之營業稅稅率 </summary>
            public const string SpecialTaxType_2 = "2";
            /// <summary> 銀行業、保險業、信託投資業、證券業、期貨業、票券業及典當業之專屬本業收入（不含銀行業、保險 業經營銀行、保 險本業收入）營業稅稅率 </summary>
            public const string SpecialTaxType_3 = "3";
            /// <summary> 保險業之再保費收入之營業稅稅率 </summary>
            public const string SpecialTaxType_4 = "4";
            /// <summary> 銀行業、保險業、信託投資業、證券業、期貨業、 票券業及典當業之非專屬本業收入營業稅稅率 </summary>
            public const string SpecialTaxType_5 = "5";
            /// <summary> 銀行業、保險業經營銀行、保險本業收入之營業稅稅率（適用於一百零三年七月以後銷售額） </summary>
            public const string SpecialTaxType_6 = "6";
            /// <summary> 銀行業、保險業經營銀行、保險本業收入之營業稅稅率（適用於一百零三年六月以前銷售額） </summary>
            public const string SpecialTaxType_7 = "7";

            /// <summary> 非銷項特種稅額 </summary>
            public class specialTaxType_0 : PX.Data.BQL.BqlString.Constant<specialTaxType_0> { public specialTaxType_0() : base(SpecialTaxType_0) {; } }
            /// <summary> 酒家及有陪侍服務之茶室、咖啡廳、酒吧等之營業稅稅率 </summary>
            public class specialTaxType_1 : PX.Data.BQL.BqlString.Constant<specialTaxType_1> { public specialTaxType_1() : base(SpecialTaxType_1) {; } }
            /// <summary> 夜總會、有娛樂節目之餐飲店之營業稅稅率 </summary>
            public class specialTaxType_2 : PX.Data.BQL.BqlString.Constant<specialTaxType_2> { public specialTaxType_2() : base(SpecialTaxType_2) {; } }
            /// <summary> 銀行業、保險業、信託投資業、證券業、期貨業、票券業及典當業之專屬本業收入（不含銀行業、保險 業經營銀行、保 險本業收入）營業稅稅率 </summary>
            public class specialTaxType_3 : PX.Data.BQL.BqlString.Constant<specialTaxType_3> { public specialTaxType_3() : base(SpecialTaxType_3) {; } }
            /// <summary> 保險業之再保費收入之營業稅稅率 </summary>
            public class specialTaxType_4 : PX.Data.BQL.BqlString.Constant<specialTaxType_4> { public specialTaxType_4() : base(SpecialTaxType_4) {; } }
            /// <summary> 銀行業、保險業、信託投資業、證券業、期貨業、 票券業及典當業之非專屬本業收入營業稅稅率 </summary>
            public class specialTaxType_5 : PX.Data.BQL.BqlString.Constant<specialTaxType_5> { public specialTaxType_5() : base(SpecialTaxType_5) {; } }
            /// <summary> 銀行業、保險業經營銀行、保險本業收入之營業稅稅率（適用於一百零三年七月以後銷售額） </summary>
            public class specialTaxType_6 : PX.Data.BQL.BqlString.Constant<specialTaxType_6> { public specialTaxType_6() : base(SpecialTaxType_6) {; } }
            /// <summary> 銀行業、保險業經營銀行、保險本業收入之營業稅稅率（適用於一百零三年六月以前銷售額） </summary>
            public class specialTaxType_7 : PX.Data.BQL.BqlString.Constant<specialTaxType_7> { public specialTaxType_7() : base(SpecialTaxType_7) {; } }
        }
        #endregion
    }
}
