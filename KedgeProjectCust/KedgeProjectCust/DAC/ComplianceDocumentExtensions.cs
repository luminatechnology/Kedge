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
                    Pair(CommercialPaper,"�u�ӥ���"),
                    Pair(BankInsurance,"�Ȧ�O�Ү�"),
                    Pair(Deposit,"�s��"),
                    Pair(CashiersCheck,"�Ȧ楻��"),
                    Pair(PromissoryNote,"�j����"),
                    Pair(Check,"�䲼"),
                    //Pair(Draft,"�ײ�"),
                    Pair(Stock,"�Ѳ�"),
                    Pair(Bond,"�Ũ�"),
                    Pair(LetterOfCredit,"�H�Ϊ�"),
                    Pair(Other,"��L")
                })
        { }

        //Const
        /// <summary>
        /// �j����
        /// </summary>
        public const String PromissoryNote = "PN";
        /// <summary>
        /// �䲼
        /// </summary>
        public const String Check = "CK";
        /// <summary>
        /// �ײ�
        /// </summary>
        public const String Draft = "DF";
        /// <summary>
        /// �s��
        /// </summary>
        public const String Deposit = "DT";
        /// <summary>
        /// �Ѳ�
        /// </summary>
        public const String Stock = "SK";
        /// <summary>
        /// �Ũ�
        /// </summary>
        public const String Bond = "BD";
        /// <summary>
        /// �H�Ϊ�
        /// </summary>
        public const String LetterOfCredit = "LC";
        /// <summary>
        /// ��L
        /// </summary>
        public const String Other = "OT";
        /// <summary>
        /// �u�ӥ���
        /// </summary>
        public const String CommercialPaper = "CP";
        /// <summary>
        /// �Ȧ楻��
        /// </summary>
        public const String CashiersCheck = "CC";
        /// <summary>
        /// �Ȧ�O�Ү�
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