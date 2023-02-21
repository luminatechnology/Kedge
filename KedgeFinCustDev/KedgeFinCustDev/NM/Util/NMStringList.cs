using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NM.Util
{
    /**
     * ===2021/03/30 Mantis: 0011988 === Althea
     * ADD Status: PendingModify & PendingRefund to NMARCheckStatus And NMAPCheckStatus
     * 
     * ===2021/06/01 Mantis: 0012070 === Althea
     * Add NMTr Details ID Const
     * 
     * ===2021/07/08 Mantis: 0012132 === Althea
     * NM AP Add Voucher Type Add :
     * APPayment & WriteoffPostage
     * 
     * ===2021/07/08 Mantis: 0012136 === Althea
     * Add NM TT Add Voucher Type
     * 
     * ===2021/07/09 Mantis: 0012134 === Althea
     * NM AP Add Voucher Type Add:
     * APPaymentReverse & WriteoffPostageReverse
     * 
     * ===2021/09/23 Mantis: 0012237 === Althea
     * NM AP Source Code Add HistoryCheck
    **/
    public class NMStringList
    {
        #region NM AR Check Status
        public class NMARCheckStatus : PXIntListAttribute
        {
            public NMARCheckStatus() : base(
                new[]
                {
                    Pair(RECEIVE,"已收票"),
                    Pair(COLLECTION,"已託收"),
                    Pair(CASH,"已兌現"),
                    Pair(WITHDRAW,"已撤票"),
                    Pair(REFUND,"已退票"),
                    //2021/03/30 Add by Althea Mantis:0011988
                    Pair(PENDINGMODIFY,"待修改"),
                    Pair(PENDINGREFUND,"待撤退票")
                })
            { }

            //Const
            /// <summary>
            /// 已收票
            /// </summary>
            public const int RECEIVE = 1;
            /// <summary>
            /// 已託收
            /// </summary>
            public const int COLLECTION = 2;
            /// <summary>
            /// 已兌現
            /// </summary>
            public const int CASH = 3;
            /// <summary>
            /// 已徹票
            /// </summary>
            public const int WITHDRAW = 4;
            /// <summary>
            /// 已退票
            /// </summary>
            public const int REFUND = 5;
            /// <summary>
            /// 待修改
            /// </summary>
            public const int PENDINGMODIFY = 6;
            /// <summary>
            /// 待撤退票
            /// </summary>
            public const int PENDINGREFUND = 7;

            /// <summary>
            /// 已收票
            /// </summary>
            public class receive : PX.Data.BQL.BqlInt.Constant<receive> { public receive() : base(RECEIVE) {; } }
            /// <summary>
            /// 已託收
            /// </summary>
            public class collection : PX.Data.BQL.BqlInt.Constant<collection> { public collection() : base(COLLECTION) {; } }
            /// <summary>
            /// 已兌現
            /// </summary>
            public class cash : PX.Data.BQL.BqlInt.Constant<cash> { public cash() : base(CASH) {; } }
            /// <summary>
            /// 已撤票
            /// </summary>
            public class withdraw : PX.Data.BQL.BqlInt.Constant<withdraw> { public withdraw() : base(WITHDRAW) {; } }
            /// <summary>
            /// 已退票
            /// </summary>
            public class refund : PX.Data.BQL.BqlInt.Constant<refund> { public refund() : base(REFUND) {; } }
            /// <summary>
            /// 待修改
            /// </summary>
            public class pendingmodify : PX.Data.BQL.BqlInt.Constant<pendingmodify> { public pendingmodify() : base(PENDINGMODIFY) {; } }
            /// <summary>
            /// 待撤退票
            /// </summary>
            public class pendingrefund : PX.Data.BQL.BqlInt.Constant<pendingrefund> { public pendingrefund() : base(PENDINGREFUND) {; } }


        }
        #endregion
        #region NM AR Add Voucher Type
        public class NMARVoucher
        {
            //Const
            /// <summary>
            /// 收票
            /// </summary>
            public const int RECEIVE = 1;
            /// <summary>
            /// 託收
            /// </summary>
            public const int COLLECTION = 2;
            /// <summary>
            /// 兌現
            /// </summary>
            public const int CASH = 3;
            /// <summary>
            /// 收票退回/徹票
            /// </summary>
            public const int RECREVERSE = 4;
            /// <summary>
            /// 託收退回/徹票
            /// </summary>
            public const int COLLREVERSE = 5;
            /// <summary>
            /// 兌現退回/徹票
            /// </summary>
            public const int CASHREVERSE = 6;

            public class receive : PX.Data.BQL.BqlInt.Constant<receive> { public receive() : base(RECEIVE) {; } }
            public class collection : PX.Data.BQL.BqlInt.Constant<collection> { public collection() : base(COLLECTION) {; } }
            public class cash : PX.Data.BQL.BqlInt.Constant<cash> { public cash() : base(CASH) {; } }
            public class recreverse : PX.Data.BQL.BqlInt.Constant<recreverse> { public recreverse() : base(RECREVERSE) {; } }
            public class collreverse : PX.Data.BQL.BqlInt.Constant<collreverse> { public collreverse() : base(COLLREVERSE) {; } }
            public class cashreverse : PX.Data.BQL.BqlInt.Constant<cashreverse> { public cashreverse() : base(CASHREVERSE) {; } }
        }
        #endregion
        #region NM AP Check Status
        public class NMAPCheckStatus : PXIntListAttribute
        {
            public NMAPCheckStatus() : base(Values, Labels) { }
            public static readonly int[] Values =
        {
            UNCONFIRM,CONFIRM,CASH,INVALID,CANCEL,REPRESENT,
            //2021/03/30 Add By Althea Mantis : 0011988
                PENDINGMODIFY,PENDINGREFUND
        };
            public static readonly string[] Labels =
            {
            "未確認","已確認","已兌現","已作廢","已退票","已代開",
            "待修改","待撤退票"
        };
            /// <summary>
            /// 未確認
            /// </summary>
            public const int UNCONFIRM = 1;
            /// <summary>
            /// 已確認
            /// </summary>
            public const int CONFIRM = 2;
            /// <summary>
            /// 已兌現
            /// </summary>
            public const int CASH = 3;
            /// <summary>
            /// 已作廢
            /// </summary>
            public const int INVALID = 4;
            /// <summary>
            /// 已退票
            /// </summary>
            public const int CANCEL = 5;
            //2021/01/27 說要拔掉
            /*/// <summary>
            /// 已列印
            /// </summary>
            public const int PRINT = 6;*/
            /// <summary>
            /// 已代開
            /// </summary>
            public const int REPRESENT = 7;
            /// <summary>
            /// 待修改
            /// </summary>
            public const int PENDINGMODIFY = 8;
            /// <summary>
            /// 待撤退票
            /// </summary>
            public const int PENDINGREFUND = 9;

            /// <summary>
            /// 未確認
            /// </summary>
            public class unconfirm : PX.Data.BQL.BqlInt.Constant<unconfirm> { public unconfirm() : base(UNCONFIRM) {; } }
            /// <summary>
            /// 已確認
            /// </summary>
            public class confirm : PX.Data.BQL.BqlInt.Constant<confirm> { public confirm() : base(CONFIRM) {; } }
            /// <summary>
            /// 已兌現
            /// </summary>
            public class cash : PX.Data.BQL.BqlInt.Constant<cash> { public cash() : base(CASH) {; } }
            /// <summary>
            /// 已作廢
            /// </summary>
            public class invalid : PX.Data.BQL.BqlInt.Constant<invalid> { public invalid() : base(INVALID) {; } }
            /// <summary>
            /// 已退票
            /// </summary>
            public class cancel : PX.Data.BQL.BqlInt.Constant<cancel> { public cancel() : base(CANCEL) {; } }
            /*public class print : PX.Data.BQL.BqlInt.Constant<print> { public print() : base(PRINT) {; } }*/
            /// <summary>
            /// 已代開
            /// </summary>
            public class represent : PX.Data.BQL.BqlInt.Constant<represent> { public represent() : base(REPRESENT) {; } }
            /// <summary>
            /// 待修改
            /// </summary>
            public class pendingmodify : PX.Data.BQL.BqlInt.Constant<pendingmodify> { public pendingmodify() : base(PENDINGMODIFY) {; } }
            /// <summary>
            /// 待撤退票
            /// </summary>
            public class pendingrefund : PX.Data.BQL.BqlInt.Constant<pendingrefund> { public pendingrefund() : base(PENDINGREFUND) {; } }


        }
        #endregion
        #region NM AP Add Voucher Type
        public class NMAPVoucher
        {
            //Const
            /// <summary>
            /// 確認/代開
            /// </summary>
            public const int CONFIRM = 1;
            /// <summary>
            /// 付款/過帳付款
            /// </summary>
            public const int PAYMENT = 2;
            /// <summary>
            /// 兌現
            /// </summary>
            public const int CASH = 3;
            /// <summary>
            /// 兌現反轉
            /// </summary>
            public const int CASHREVERSE = 4;
            /// <summary>
            /// 確認反轉
            /// </summary>
            public const int CONFIRMREVERSE = 5;
            /// <summary>
            /// 付款過帳反轉
            /// </summary>
            public const int PAYMENTREVERSE = 6;
            /// <summary>
            /// APPayment 
            /// 2021/07/08 Add Mantis:0012132
            /// </summary>
            public const int APPAYMENT = 7;
            /// <summary>
            /// 沖銷郵資
            /// 2021/07/08 Add Mantis:0012132
            /// </summary>
            public const int WRITEOFFPOSTAGE = 8;
            /// <summary>
            /// APPayment反轉
            /// </summary>
            public const int APPAYMENTREVERSE = 9;
            /// <summary>
            /// 沖銷郵資反轉
            /// </summary>
            public const int WRITEOFFPOSTAGEREVERSE = 10;
            /// <summary>
            /// 預付款
            /// </summary>
            public const int PPM = 11;
            /// <summary>
            /// 預付款反轉
            /// </summary>
            public const int PPMREVERSE = 12;


            public class confirm : PX.Data.BQL.BqlInt.Constant<confirm> { public confirm() : base(CONFIRM) {; } }
            public class cash : PX.Data.BQL.BqlInt.Constant<cash> { public cash() : base(CASH) {; } }
            public class payment : PX.Data.BQL.BqlInt.Constant<payment> { public payment() : base(PAYMENT) {; } }
            public class paymentreverse : PX.Data.BQL.BqlInt.Constant<paymentreverse> { public paymentreverse() : base(PAYMENTREVERSE) {; } }
            public class cashreverse : PX.Data.BQL.BqlInt.Constant<cashreverse> { public cashreverse() : base(CASHREVERSE) {; } }
            public class confirmreverse : PX.Data.BQL.BqlInt.Constant<confirmreverse> { public confirmreverse() : base(CONFIRMREVERSE) {; } }
            public class appayment : PX.Data.BQL.BqlInt.Constant<appayment> { public appayment() : base(APPAYMENT) {; } }
            public class writeoffpostage : PX.Data.BQL.BqlInt.Constant<writeoffpostage> { public writeoffpostage() : base(WRITEOFFPOSTAGE) {; } }
            public class appaymentreverse : PX.Data.BQL.BqlInt.Constant<appaymentreverse> { public appaymentreverse() : base(APPAYMENTREVERSE) {; } }
            public class writeoffpostagereverse : PX.Data.BQL.BqlInt.Constant<writeoffpostagereverse> { public writeoffpostagereverse() : base(WRITEOFFPOSTAGEREVERSE) {; } }
            public class ppm : PX.Data.BQL.BqlInt.Constant<ppm> { public ppm() : base(PPM) {; } }
            public class ppmreverse : PX.Data.BQL.BqlInt.Constant<ppmreverse> { public ppmreverse() : base(PPMREVERSE) {; } }

        }
        #endregion
        #region NM AP Deliver Method
        public class NMAPDeliverMethod : PXIntListAttribute
        {
            public NMAPDeliverMethod() : base(Values, Labels) { }

            public static readonly int[] Values =
        {
            SEND,SELFRECEIVE,OTHER
        };
            public static readonly string[] Labels =
            {
            "寄領","廠商自領","其它"
        };

            public const int SEND = 1;
            public const int SELFRECEIVE = 2;
            public const int OTHER = 3;


            public class send : PX.Data.BQL.BqlInt.Constant<send> { public send() : base(SEND) {; } }
            public class selfreceive : PX.Data.BQL.BqlInt.Constant<selfreceive> { public selfreceive() : base(SELFRECEIVE) {; } }
        }
        #endregion
        #region NM AP Source Code
        public class NMAPSourceCode : PXIntListAttribute
        {
            public NMAPSourceCode() : base(new[] {
                Pair(MANUAL, "手動"),
                Pair(AUTO, "批次"),//2021-05-20 edit by Alton 自動→批次
                Pair(REPRESENT, "代開"),
                Pair(HISTORYCHECK,"歷史票據")//2021/09/23 Edit by Althea 
            })
            { }

            public const int MANUAL = 1;
            public const int AUTO = 2;
            public const int REPRESENT = 3;
            public const int HISTORYCHECK = 4;

            public class manual : PX.Data.BQL.BqlInt.Constant<manual> { public manual() : base(MANUAL) {; } }
            public class auto : PX.Data.BQL.BqlInt.Constant<auto> { public auto() : base(AUTO) {; } }
            public class represent : PX.Data.BQL.BqlInt.Constant<represent> { public represent() : base(REPRESENT) {; } }
            public class historycheck : PX.Data.BQL.BqlInt.Constant<historycheck> { public historycheck() : base(HISTORYCHECK) {; } }
        }
        #endregion
        #region NM TT Add Voucher Type
        public class NMTTVoucher
        {
            /// </summary>
            public const int APPAYMENT = 1;
            /// <summary>
            /// 沖銷郵資
            /// 2021/07/08 Add Mantis:0012132
            /// </summary>
            public const int WRITEOFFPOSTAGE = 2;

            public class appayment : PX.Data.BQL.BqlInt.Constant<appayment> { public appayment() : base(APPAYMENT) {; } }
            public class writeoffpostage : PX.Data.BQL.BqlInt.Constant<writeoffpostage> { public writeoffpostage() : base(WRITEOFFPOSTAGE) {; } }

        }
        #endregion
        #region NM AP TeleTransLog Type
        public class NMApTtLogType : PXIntListAttribute
        {
            public NMApTtLogType() : base(
                new[]
                {
                    Pair(TN,"台新"),
                    Pair(GT,"國泰")
                })
            { }
            /// <summary>
            /// 台新
            /// </summary>
            public const int TN = 1;
            /// <summary>
            /// 國泰
            /// </summary>
            public const int GT = 2;

            /// <summary>
            /// 台新
            /// </summary>
            public class tn : PX.Data.BQL.BqlInt.Constant<tn> { public tn() : base(TN) {; } }
            /// <summary>
            /// 國泰
            /// </summary>
            public class gt : PX.Data.BQL.BqlInt.Constant<gt> { public gt() : base(GT) {; } }

        }
        #endregion
        #region NM AP TeleTransLog Status
        public class NMApTtLogStatus : PXIntListAttribute
        {
            public NMApTtLogStatus() : base(
                new[]
                {
                    Pair(CREATED,"產生電匯"),
                    Pair(FEED_BACK,"取得回饋"),
                    Pair(FEED_BACK_SUCCESS,"回饋結果成功")
                })
            { }
            /// <summary>
            /// 產生電匯
            /// </summary>
            public const int CREATED = 1;
            /// <summary>
            /// 取得回饋
            /// </summary>
            public const int FEED_BACK = 2;

            /// <summary>
            /// 回饋結果成功
            /// </summary>
            public const int FEED_BACK_SUCCESS = 3;

            /// <summary>
            /// 產生電匯
            /// </summary>
            public class created : PX.Data.BQL.BqlInt.Constant<created> { public created() : base(CREATED) {; } }
            /// <summary>
            /// 取得回饋
            /// </summary>
            public class feedBack : PX.Data.BQL.BqlInt.Constant<feedBack> { public feedBack() : base(FEED_BACK) {; } }
            /// <summary>
            /// 回饋結果成功
            /// </summary>
            public class feedBackSuccess : PX.Data.BQL.BqlInt.Constant<feedBackSuccess> { public feedBackSuccess() : base(FEED_BACK_SUCCESS) {; } }
        }
        #endregion
        #region NM AP 台新電匯狀態
        public class NMApTnTtStatus : PXStringListAttribute
        {
            public NMApTnTtStatus() : base(
                new[]
                {
                    Pair(_01,"編輯，資料尚未送審"),
                    Pair(_02,"送審，資料已送審，尚未審核"),
                    Pair(_03,"待審核，資料已審核，但未完成所有的審核"),
                    Pair(_04,"待放行，資料已完成所有的審核，等待放行"),
                    Pair(_05,"退件，資料被審核人員或是放行人員退件"),
                    Pair(_06,"等待回應，資料已放行，FEDI Server尚未處理"),
                    Pair(_07,"上傳主機，資料已放行，FEDI Server已驗章完成，等待後續帳務處理"),
                    Pair(_08,"扣帳成功"),
                    Pair(_09,"付款失敗"),
                    Pair(_10,"退帳通知，資料扣帳成功，但入帳時被入帳行退回"),
                    Pair(_11,"預約取消，資料原已預約，被用戶取消付款"),
                    Pair(_14,"退帳通知(人工回沖)，財經公司通知，進行入工退帳")
                })
            { }
            /// <summary>編輯，資料尚未送審</summary>
            public const string _01 = "01";
            /// <summary>送審，資料已送審，尚未審核</summary>
            public const string _02 = "02";
            /// <summary>待審核，資料已審核，但未完成所有的審核</summary>
            public const string _03 = "03";
            /// <summary>待放行，資料已完成所有的審核，等待放行</summary>
            public const string _04 = "04";
            /// <summary>退件，資料被審核人員或是放行人員退件</summary>
            public const string _05 = "05";
            /// <summary>等待回應，資料已放行，FEDI Server尚未處理</summary>
            public const string _06 = "06";
            /// <summary>上傳主機，資料已放行，FEDI Server已驗章完成，等待後續帳務處理</summary>
            public const string _07 = "07";
            /// <summary>扣帳成功</summary>
            public const string _08 = "08";
            /// <summary>付款失敗</summary>
            public const string _09 = "09";
            /// <summary>退帳通知，資料扣帳成功，但入帳時被入帳行退回</summary>
            public const string _10 = "10";
            /// <summary>預約取消，資料原已預約，被用戶取消付款</summary>
            public const string _11 = "11";
            /// <summary>退帳通知(人工回沖)，財經公司通知，進行入工退帳</summary>
            public const string _14 = "14";

            /// <summary>編輯，資料尚未送審</summary>
            public class n01 : PX.Data.BQL.BqlString.Constant<n01> { public n01() : base(_01) {; } }
            /// <summary>送審，資料已送審，尚未審核</summary>
            public class n02 : PX.Data.BQL.BqlString.Constant<n02> { public n02() : base(_02) {; } }
            /// <summary>待審核，資料已審核，但未完成所有的審核</summary>
            public class n03 : PX.Data.BQL.BqlString.Constant<n03> { public n03() : base(_03) {; } }
            /// <summary>待放行，資料已完成所有的審核，等待放行</summary>
            public class n04 : PX.Data.BQL.BqlString.Constant<n04> { public n04() : base(_04) {; } }
            /// <summary>退件，資料被審核人員或是放行人員退件</summary>
            public class n05 : PX.Data.BQL.BqlString.Constant<n05> { public n05() : base(_05) {; } }
            /// <summary>等待回應，資料已放行，FEDI Server尚未處理</summary>
            public class n06 : PX.Data.BQL.BqlString.Constant<n06> { public n06() : base(_06) {; } }
            /// <summary>上傳主機，資料已放行，FEDI Server已驗章完成，等待後續帳務處理</summary>
            public class n07 : PX.Data.BQL.BqlString.Constant<n07> { public n07() : base(_07) {; } }
            /// <summary>扣帳成功</summary>
            public class n08 : PX.Data.BQL.BqlString.Constant<n08> { public n08() : base(_08) {; } }
            /// <summary>付款失敗</summary>
            public class n09 : PX.Data.BQL.BqlString.Constant<n09> { public n09() : base(_09) {; } }
            /// <summary>退帳通知，資料扣帳成功，但入帳時被入帳行退回</summary>
            public class n10 : PX.Data.BQL.BqlString.Constant<n10> { public n10() : base(_10) {; } }
            /// <summary>預約取消，資料原已預約，被用戶取消付款</summary>
            public class n11 : PX.Data.BQL.BqlString.Constant<n11> { public n11() : base(_11) {; } }
            /// <summary>退帳通知(人工回沖)，財經公司通知，進行入工退帳</summary>
            public class n14 : PX.Data.BQL.BqlString.Constant<n14> { public n14() : base(_14) {; } }

        }
        #endregion
        #region NM AP 國泰電匯交易狀態
        public class NMApGtTtStatus : PXStringListAttribute
        {
            public NMApGtTtStatus() : base(
                new[]
                {
                    Pair(_0,"未啟動"),
                    Pair(_1,"交易成功"),
                    Pair(_254,"已刪除"),
                    Pair(_255,"交易失敗")
                })
            { }
            /// <summary>未啟動</summary>
            public const string _0 = "0";
            /// <summary>交易成功</summary>
            public const string _1 = "1";
            /// <summary>已刪除</summary>
            public const string _254 = "254";
            /// <summary>交易失敗</summary>
            public const string _255 = "255";

            /// <summary>未啟動</summary>
            public class g0 : PX.Data.BQL.BqlString.Constant<g0> { public g0() : base(_0) {; } }
            /// <summary>交易成功</summary>
            public class g1 : PX.Data.BQL.BqlString.Constant<g1> { public g1() : base(_1) {; } }
            /// <summary>已刪除</summary>
            public class g254 : PX.Data.BQL.BqlString.Constant<g254> { public g254() : base(_254) {; } }
            /// <summary>交易失敗</summary>
            public class g255 : PX.Data.BQL.BqlString.Constant<g255> { public g255() : base(_255) {; } }
        }
        #endregion
        #region NM AP 國泰電匯錯誤代碼
        public class NMApGtTtErrorCode : PXStringListAttribute
        {
            public NMApGtTtErrorCode() : base(
                new[]
                {
                    Pair(_0000,"成功"),
                    Pair(_M323,"退匯"),
                    Pair(_M911,"存款不足")
                })
            { }
            /// <summary>成功</summary>
            public const string _0000 = "0000";
            /// <summary>退匯</summary>
            public const string _M323 = "M323";
            /// <summary>存款不足</summary>
            public const string _M911 = "M911";

            /// <summary>成功</summary>
            public class g0000 : PX.Data.BQL.BqlString.Constant<g0000>{public g0000() : base(_0000) {; }}
            /// <summary>退匯</summary>
            public class gM323 : PX.Data.BQL.BqlString.Constant<gM323>{public gM323() : base(_M323) {; }}
            /// <summary>存款不足</summary>
            public class gM911 : PX.Data.BQL.BqlString.Constant<gM911>{public gM911() : base(_M911) {; }}
        }
        #endregion
        #region NM AP 國泰電匯退款理由
        public class NMApGtTtReasonCode : PXStringListAttribute
        {
            public NMApGtTtReasonCode() : base(
                new[]
                {
                    Pair(_3,"帳號錯誤"),
                    Pair(_4,"姓名不符"),
                    Pair(_9,"滯納 ")
                })
            { }
            /// <summary>帳號錯誤</summary>
            public const String _3 = "3";
            /// <summary>姓名不符</summary>
            public const String _4 = "4";
            /// <summary>滯納 </summary>
            public const String _9 = "9";
            /// <summary>帳號錯誤</summary>
            public class g3 : PX.Data.BQL.BqlString.Constant<g3>{public g3() : base(_3) {; }}
            /// <summary>姓名不符</summary>
            public class g4 : PX.Data.BQL.BqlString.Constant<g4>{public g4() : base(_4) {; }}
            /// <summary>滯納 </summary>
            public class g9 : PX.Data.BQL.BqlString.Constant<g9>{public g9() : base(_9) {; }}
        }
        #endregion

        #region NM AP Bank Feedback Type
        public class NMApFeedbackType : PXStringListAttribute
        {
            public NMApFeedbackType() : base(
                new[]
                {
                    Pair(TN_BANK_CHECK,"台新銀行代開支票檔"),
                    Pair(TN_TT,"台新銀行電匯"),
                    Pair(GT_TT,"國泰世華銀行電匯")
                })
            { }
            /// <summary>
            /// 台新銀行代開支票檔
            /// </summary>
            public const string TN_BANK_CHECK = "A";
            /// <summary>
            /// 台新銀行電匯
            /// </summary>
            public const string TN_TT = "B";
            /// <summary>
            /// 國泰世華銀行電匯
            /// </summary>
            public const string GT_TT = "C";

            /// <summary>
            /// 台新銀行代開支票檔
            /// </summary>
            public class tnBankCheck : PX.Data.BQL.BqlString.Constant<tnBankCheck> { public tnBankCheck() : base(TN_BANK_CHECK) {; } }
            /// <summary>
            /// 台新銀行電匯
            /// </summary>
            public class tnTT : PX.Data.BQL.BqlString.Constant<tnTT> { public tnTT() : base(TN_TT) {; } }
            /// <summary>
            /// 國泰世華銀行電匯
            /// </summary>
            public class gtTT : PX.Data.BQL.BqlString.Constant<gtTT> { public gtTT() : base(GT_TT) {; } }
        }
        #endregion
        #region NM AP Tn Bank Check Status
        public class NMApTnBankCheckStatus : PXStringListAttribute
        {
            public NMApTnBankCheckStatus() : base(
                new[]
                {
                   Pair(A,"編輯"),
                   Pair(B,"待審核"),
                   Pair(C,"審核中"),
                   Pair(D,"待放行"),
                   Pair(E,"銀行待處理"),
                   Pair(K,"退件"),
                   Pair(R,"銀行處理中"),
                   Pair(S,"處理完畢"),
                   Pair(T,"作廢"),
                   Pair(U,"已兌現")
                })
            { }
            /// <summary> 編輯 </summary>
            public const string A = "A";
            /// <summary> 待審核 </summary>
            public const string B = "B";
            /// <summary> 審核中 </summary>
            public const string C = "C";
            /// <summary> 待放行 </summary>
            public const string D = "D";
            /// <summary> 銀行待處理 </summary>
            public const string E = "E";
            /// <summary> 退件 </summary>
            public const string K = "K";
            /// <summary> 銀行處理中 </summary>
            public const string R = "R";
            /// <summary> 處理完畢 </summary>
            public const string S = "S";
            /// <summary> 作廢 </summary>
            public const string T = "T";
            /// <summary> 已兌現 </summary>
            public const string U = "U";

            /// <summary> 編輯 </summary>
            public class a : PX.Data.BQL.BqlString.Constant<a> { public a() : base(A) {; } }
            /// <summary> 待審核 </summary>
            public class b : PX.Data.BQL.BqlString.Constant<b> { public b() : base(B) {; } }
            /// <summary> 審核中 </summary>	
            public class c : PX.Data.BQL.BqlString.Constant<c> { public c() : base(C) {; } }
            /// <summary> 待放行 </summary>	
            public class d : PX.Data.BQL.BqlString.Constant<d> { public d() : base(D) {; } }
            /// <summary> 銀行待處理 </summary>
            public class e : PX.Data.BQL.BqlString.Constant<e> { public e() : base(E) {; } }
            /// <summary> 退件 </summary>
            public class k : PX.Data.BQL.BqlString.Constant<k> { public k() : base(K) {; } }
            /// <summary> 銀行處理中 </summary>
            public class r : PX.Data.BQL.BqlString.Constant<r> { public r() : base(R) {; } }
            /// <summary> 處理完畢 </summary>
            public class s : PX.Data.BQL.BqlString.Constant<s> { public s() : base(S) {; } }
            /// <summary> 作廢 </summary>
            public class t : PX.Data.BQL.BqlString.Constant<t> { public t() : base(T) {; } }
            /// <summary> 已兌現 </summary>
            public class u : PX.Data.BQL.BqlString.Constant<u> { public u() : base(U) {; } }

        }
        #endregion

        #region NM Book Usage
        public class NMBookUsage : PXStringListAttribute
        {
            public NMBookUsage() : base(
                new[]
                {
                    Pair(CHECK,"支票"),
                    Pair(CASHIERSCHECK,"銀行本票")
                })
            { }

            //Const
            /// <summary>
            /// 支票
            /// </summary>
            public const string CHECK = "CK";
            /// <summary>
            /// 銀行本票
            /// </summary>
            public const string CASHIERSCHECK = "CC";


            public class check : PX.Data.BQL.BqlString.Constant<check> { public check() : base(CHECK) {; } }
            public class cashierscheck : PX.Data.BQL.BqlString.Constant<cashierscheck> { public cashierscheck() : base(CASHIERSCHECK) {; } }

        }
        #endregion

        #region NMTrDetailsID
        public class NMTrDetailsID 
        {

            //Const
            /// <summary>
            /// ACCT NAME
            /// </summary>
            public const string AcctName = "ACCT NAME";
            /// <summary>
            /// BANK ACCT
            /// </summary>
            public const string BankAcct = "BANK ACCT";
            /// <summary>
            /// BANK NAME
            /// </summary>
            public const string BankName = "BANK NAME";
            /// <summary>
            /// BANK NBR
            /// </summary>
            public const string BankNbr = "BANK NBR";
            /// <summary>
            /// BRANCH NAME
            /// </summary>
            public const string BranchName = "BRANCH NAM";
            /// <summary>
            /// BRANCH NBR
            /// </summary>
            public const string BranchNbr = "BRANCH NBR";
            /// <summary>
            /// CATEGORYID
            /// </summary>
            public const string CategoryID = "CATEGORYID";
            /// <summary>
            /// CATEGORY
            /// </summary>
            public const string Category = "CATEGORY";

            public class acctname : PX.Data.BQL.BqlString.Constant<acctname> { public acctname() : base(AcctName) {; } }
            public class bankacct : PX.Data.BQL.BqlString.Constant<bankacct> { public bankacct() : base(BankAcct) {; } }
            public class bankname : PX.Data.BQL.BqlString.Constant<bankname> { public bankname() : base(BankName) {; } }
            public class banknbr : PX.Data.BQL.BqlString.Constant<banknbr> { public banknbr() : base(BankNbr) {; } }
            public class branchname : PX.Data.BQL.BqlString.Constant<branchname> { public branchname() : base(BranchName) {; } }
            public class branchnbr : PX.Data.BQL.BqlString.Constant<branchnbr> { public branchnbr() : base(BranchNbr) {; } }
            public class categoryID : PX.Data.BQL.BqlString.Constant<categoryID> { public categoryID() : base(CategoryID) {; } }
            public class category : PX.Data.BQL.BqlString.Constant<category> { public category() : base(Category) {; } }


        }
        #endregion

        #region NM SettledType
        public class NMSettledType : PXStringListAttribute
        {
            public NMSettledType() : base(
                new[]
                {
                    Pair(CashAccount,"現金帳戶"),
                    Pair(AccountType,"現金帳戶帳戶類型"),
                    Pair(Other,"其他")
                })
            { }

            #region const
            /// <summary>
            /// A.現金帳戶
            /// </summary>
            public const string CashAccount = "A";
            /// <summary>
            /// B.現金帳戶帳戶類型
            /// </summary>
            public const string AccountType = "B";
            /// <summary>
            /// C.其他
            /// </summary>
            public const string Other = "C";
            #endregion

            /// <summary>
            /// A.現金帳戶
            /// </summary>
            public class cashAccount : PX.Data.BQL.BqlString.Constant<cashAccount> { public cashAccount() : base(CashAccount) {; } }
            /// <summary>
            /// B.現金帳戶帳戶類型
            /// </summary>
            public class accountType : PX.Data.BQL.BqlString.Constant<accountType> { public accountType() : base(AccountType) {; } }
            /// <summary>
            /// C.其他
            /// </summary>
            public class other : PX.Data.BQL.BqlString.Constant<other> { public other() : base(Other) {; } }

        }
        #endregion

        #region NM AP Create Paymment Type
        #region NM AP TeleTransLog Status
        public class NMApCreatePaymentType : PXIntListAttribute
        {
            public NMApCreatePaymentType() : base(
                new[]
                {
                    Pair(CHECK,"票據"),
                    Pair(TT,"電匯")
                })
            { }
            /// <summary> 票據 </summary>
            public const int CHECK = 1;
            /// <summary> 電匯 </summary>
            public const int TT = 2;

            /// <summary> 票據 </summary>
            public class check : PX.Data.BQL.BqlInt.Constant<check> { public check() : base(CHECK) {; } }
            /// <summary> 電匯 </summary>
            public class tt : PX.Data.BQL.BqlInt.Constant<tt> { public tt() : base(TT) {; } }
        }
        #endregion
        #endregion

        public const int ZERO = 0;
        public class zero : PX.Data.BQL.BqlInt.Constant<zero> { public zero() : base(ZERO) {; } }

    }
}
