using PX.Data;
using System;

namespace PX.Objects.CN.Compliance.CL.DAC
{

    public class ComplianceDocumentExt : PXCacheExtension<PX.Objects.CN.Compliance.CL.DAC.ComplianceDocument>
    {
        #region UsrGuarClass
        [PXDBString(2)]
        [PXUIField(DisplayName = "UsrGuarClass")]
        [GuarClassForProdList]
        public virtual string UsrGuarClass { get; set; }
        public abstract class usrGuarClass : PX.Data.BQL.BqlString.Field<usrGuarClass> { }
        #endregion
    }
    public class GuarClassForProdList : PXStringListAttribute
    {
        public GuarClassForProdList() : base(new[]
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

        public class bankInsurance : PX.Data.BQL.BqlString.Constant<cashiersCheck> { public bankInsurance() : base(BankInsurance) {; } }
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
}