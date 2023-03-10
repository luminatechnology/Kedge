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
                    Pair(CommercialPaper,"坝セ布"),
                    Pair(BankInsurance,"蝗︽玂靡"),
                    Pair(Deposit,"虫"),
                    Pair(CashiersCheck,"蝗︽セ布"),
                    Pair(PromissoryNote,"セ布"),
                    Pair(Check,"や布"),
                    //Pair(Draft,"蹲布"),
                    Pair(Stock,"布"),
                    Pair(Bond,"杜ㄩ"),
                    Pair(LetterOfCredit,"獺ノ"),
                    Pair(Other,"ㄤ")
                })
        { }

        //Const
        /// <summary>
        /// セ布
        /// </summary>
        public const String PromissoryNote = "PN";
        /// <summary>
        /// や布
        /// </summary>
        public const String Check = "CK";
        /// <summary>
        /// 蹲布
        /// </summary>
        public const String Draft = "DF";
        /// <summary>
        /// 虫
        /// </summary>
        public const String Deposit = "DT";
        /// <summary>
        /// 布
        /// </summary>
        public const String Stock = "SK";
        /// <summary>
        /// 杜ㄩ
        /// </summary>
        public const String Bond = "BD";
        /// <summary>
        /// 獺ノ
        /// </summary>
        public const String LetterOfCredit = "LC";
        /// <summary>
        /// ㄤ
        /// </summary>
        public const String Other = "OT";
        /// <summary>
        /// 坝セ布
        /// </summary>
        public const String CommercialPaper = "CP";
        /// <summary>
        /// 蝗︽セ布
        /// </summary>
        public const String CashiersCheck = "CC";
        /// <summary>
        /// 蝗︽玂靡
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