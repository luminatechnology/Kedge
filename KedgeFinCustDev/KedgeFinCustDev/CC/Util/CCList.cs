using System;
using PX.Data;

namespace CC.Util
{
    /**
     * ===2021/03/30 Mantis : 0011988 === Althea
     * ADD Status: PendingModify & PendingRefund to NMARCheckStatus And NMAPCheckStatus
     **/
    public class CCList
    {

        #region Target Type
        public class CCTargetType : PS.Util.PSStringList.PSTargetType
        {
        }
        #endregion

        #region CCPayableStatus
        public class CCPayableStatus : CCStatus
        {
            public CCPayableStatus() : base(new[]
                                            {
                                                Pair(OnHold, "On Hold"),
                                                Pair(Balanced,"Balanced"),
                                                Pair(Released,"Released"),
                                                Pair(Returned,"Returned"),
                                                Pair(Voided,"Voided"),
                                                Pair(AppliedRTN,"AppliedRTN"),
                                                //2021/03/30 Add Mantis : 0011988
                                                Pair(PendingModify,"PendingModify"),
                                                Pair(PendingRefund,"PendingRefund")
          
                                            }){ }
        }
        #endregion

        #region CCAPVoucher
        public class CCAPVoucher
        {
            //Const
            /// <summary>
            /// 核可
            /// </summary>
            public const int RELEASE = 1;
            /// <summary>
            /// 退回
            /// </summary>
            public const int REVERSE = 2;


            public class release : PX.Data.BQL.BqlInt.Constant<release> { public release() : base(RELEASE) {; } }
            public class reverse : PX.Data.BQL.BqlInt.Constant<reverse> { public reverse() : base(REVERSE) {; } }
        }
        #endregion

        #region CCReceivableStatus
        public class CCReceivableStatus : CCStatus
        {
            public CCReceivableStatus() : base(new[]
                {
                    Pair(Balanced,"Balanced"),
                    Pair(Released,"Released"),
                    Pair(Returned,"Returned"),
                    Pair(AccountReceived,"Account Received"),
                    Pair(Voided,"Voided"),
                    Pair(AppliedRTN,"AppliedRTN"),
                    Pair(PendingModify,"PendingModify"),
                    Pair(PendingRefund,"PendingRefund")
                })
            { }
        }
        #endregion

        #region CCARVoucher
        public class CCARVoucher
        {
            //Const
            /// <summary>
            /// 核可
            /// </summary>
            public const int RELEASE = 1;
            /// <summary>
            /// 退回
            /// </summary>
            public const int REVERSE = 2;


            public class release : PX.Data.BQL.BqlInt.Constant<release> { public release() : base(RELEASE) {; } }
            public class reverse : PX.Data.BQL.BqlInt.Constant<reverse> { public reverse() : base(REVERSE) {; } }
        }
        #endregion

        #region CC Status
        public class CCStatus : PXStringListAttribute
        {
            public CCStatus(Tuple<string, string>[] valuesToLabel) : base(valuesToLabel){ }

            //Const
            /// <summary>
            /// OnHold
            /// </summary>
            public const string OnHold = "H";
            /// <summary>
            /// Balanced
            /// </summary>
            public const string Balanced = "B";
            /// <summary>
            /// Released
            /// </summary>
            public const string Released = "R";
            /// <summary>
            /// Returned
            /// </summary>
            public const string Returned = "T";
            /// <summary>
            /// AccountReceived
            /// </summary>
            public const string AccountReceived = "A";
            /// <summary>
            /// Voided
            /// </summary>
            public const string Voided = "V";
            /// <summary>
            /// AppliedRTN
            /// </summary>
            public const string AppliedRTN = "N";

            //2021/03/30 Add Mantis : 0011988
            /// <summary>
            /// 待修改
            /// </summary>
            public const string PendingModify = "M";
            /// <summary>
            /// 待撤退票
            /// </summary>
            public const string PendingRefund = "F";

            public class onHold : PX.Data.BQL.BqlString.Constant<onHold> { public onHold() : base(OnHold) {; } }
            public class balanced : PX.Data.BQL.BqlString.Constant<balanced> { public balanced() : base(Balanced) {; } }
            public class released : PX.Data.BQL.BqlString.Constant<released> { public released() : base(Released) {; } }
            public class returned : PX.Data.BQL.BqlString.Constant<returned> { public returned() : base(Returned) {; } }
            public class accountReceived : PX.Data.BQL.BqlString.Constant<accountReceived> { public accountReceived() : base(AccountReceived) {; } }
            public class voided : PX.Data.BQL.BqlString.Constant<voided> { public voided() : base(Voided) {; } }
            public class appliedRTN : PX.Data.BQL.BqlString.Constant<appliedRTN> { public appliedRTN() : base(AppliedRTN) {; } }
            public class pendingModify : PX.Data.BQL.BqlString.Constant<pendingModify> { public pendingModify() : base(PendingModify) {; } }
            public class pendingRefund : PX.Data.BQL.BqlString.Constant<pendingRefund> { public pendingRefund() : base(PendingRefund) {; } }
        }
        #endregion

        #region Guar Class
        public class GuarClassList : PXStringListAttribute
        {
            public GuarClassList() : base(new[]
                {
                    Pair(CommercialPaper,"工商本票"),
                    Pair(BankInsurance,"銀行保證書"),
                    Pair(Deposit,"存單"),
                    Pair(CashiersCheck,"銀行本票"),
                    Pair(PromissoryNote,"大本票"),
                    Pair(Check,"支票"),
                    //Pair(Draft,"匯票"),
                    Pair(Stock,"股票"),
                    Pair(Bond,"債券"),
                    Pair(LetterOfCredit,"信用狀"),
                    Pair(Other,"其他")
                })
            { }

            //Const
            /// <summary>
            /// 大本票
            /// </summary>
            public const String PromissoryNote = "PN";
            /// <summary>
            /// 支票
            /// </summary>
            public const String Check = "CK";
            /// <summary>
            /// 匯票
            /// </summary>
            public const String Draft = "DF";
            /// <summary>
            /// 存單
            /// </summary>
            public const String Deposit = "DT";
            /// <summary>
            /// 股票
            /// </summary>
            public const String Stock = "SK";
            /// <summary>
            /// 債券
            /// </summary>
            public const String Bond = "BD";
            /// <summary>
            /// 信用狀
            /// </summary>
            public const String LetterOfCredit = "LC";
            /// <summary>
            /// 其他
            /// </summary>
            public const String Other = "OT";
            /// <summary>
            /// 工商本票
            /// </summary>
            public const String CommercialPaper = "CP";
            /// <summary>
            /// 銀行本票
            /// </summary>
            public const String CashiersCheck = "CC";
            /// <summary>
            /// 銀行保證書
            /// </summary>
            public const String BankInsurance = "BI";

            public class bankInsurance : PX.Data.BQL.BqlString.Constant<bankInsurance> { public bankInsurance() : base(BankInsurance) {; } }
            public class cashiersCheck : PX.Data.BQL.BqlString.Constant<cashiersCheck> { public cashiersCheck() : base(CashiersCheck) {; } }
            public class commercialPaper : PX.Data.BQL.BqlString.Constant<commercialPaper> { public commercialPaper() : base(CommercialPaper) {; } }
            public class promissoryNote : PX.Data.BQL.BqlString.Constant<promissoryNote> { public promissoryNote() : base(PromissoryNote) {; } }
            public class check : PX.Data.BQL.BqlString.Constant<check> { public check() : base(Check) {; } }
            public class draft : PX.Data.BQL.BqlString.Constant<draft> { public draft() : base(Draft) {; } }
            public class deposit : PX.Data.BQL.BqlString.Constant<deposit> { public deposit() : base(Deposit) {; } }
            public class stock : PX.Data.BQL.BqlString.Constant<stock> { public stock() : base(Stock) {; } }
            public class bond : PX.Data.BQL.BqlString.Constant<bond> { public bond() : base(Bond) {; } }
            public class letterOfCredit : PX.Data.BQL.BqlString.Constant<letterOfCredit> { public letterOfCredit() : base(LetterOfCredit) {; } }
            public class other : PX.Data.BQL.BqlString.Constant<other> { public other() : base(Other) {; } }
        }
        #endregion

        #region Guar Type
        public class GuarTypeList : PXStringListAttribute
        {
            public GuarTypeList() : base(new[]
                {
                    Pair(Escrow,"履約"),
                    Pair(Warranty,"保固"),
                    Pair(PrePayment,"預付款"),
                    Pair(Other,"其他")
                })
            { }

            //Const
            /// <summary>
            /// 履約
            /// </summary>
            public const String Escrow = "E";
            /// <summary>
            /// 保固
            /// </summary>
            public const String Warranty = "W";
            /// <summary>
            /// 其他
            /// </summary>
            public const String Other = "O";
            /// <summary>
            /// 預付款
            /// </summary>
            public const String PrePayment = "P";

            public class prepayment : PX.Data.BQL.BqlString.Constant<prepayment> { public prepayment() : base(PrePayment) {; } }
            public class escrow : PX.Data.BQL.BqlString.Constant<escrow> { public escrow() : base(Escrow) {; } }
            public class warranty : PX.Data.BQL.BqlString.Constant<warranty> { public warranty() : base(Warranty) {; } }
            public class other : PX.Data.BQL.BqlString.Constant<other> { public other() : base(Other) {; } }
        }
        #endregion
    }
}
