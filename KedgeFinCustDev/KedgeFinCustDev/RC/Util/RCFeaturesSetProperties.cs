using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC.Util
{
    public class RCFeaturesSetProperties
    {
        /// <summary>
        /// 此專案是否為測試環境
        /// </summary>
        public const bool IS_TEST_ENV = true;

        /// <summary>
        /// 統一發票
        /// </summary>
        public const string GOV_UNIFROM_INVOICE = "GovUniformInvoice";
        /// <summary>
        /// 發票偏好設定
        /// </summary>
        public const string GUI_PREFERENCE = "GuiPreference";
        /// <summary>
        /// 進項發票
        /// </summary>
        public const string AP_INVOICE = "ApInvoice";
        /// <summary>
        /// 銷項發票
        /// </summary>
        public const string AR_INVOICE = "ArInvoice";
        /// <summary>
        /// 申報作業
        /// </summary>
        public const string DECLARE_GOV_INVOICE = "DeclareGovInvoice";
        /// <summary>
        /// 金財通整合
        /// </summary>
        public const string BANK_PRO_INTEGRATION = "BankProIntegration";
        /// <summary>
        /// 代扣稅作業
        /// </summary>
        public const string WITHHOLDING_TAXES = "WithholdingTaxes";
        /// <summary>
        /// NotesManagement
        /// </summary>
        public const string NOTES_MANAGEMENT = "NotesManagement";
        /// <summary>
        /// NotesPreference
        /// </summary>
        public const string NOTES_PREFERENCE = "NotesPreference";
        /// <summary>
        /// NotesReceivable
        /// </summary>
        public const string NOTES_RECEIVABLE = "NotesReceivable";
        /// <summary>
        /// NotesPayable
        /// </summary>
        public const string NOTES_PAYABLE = "NotesPayable";
        /// <summary>
        /// LedgerSettlement
        /// </summary>
        public const string LEDGER_SETTLEMENT = "LedgerSettlement";
        /// <summary>
        /// Check
        /// </summary>
        public const string CHECK = "Check";
        /// <summary>
        /// 繳款單
        /// </summary>
        public const string PAYMENT_SLIP = "PaymentSlip";
        /// <summary>
        /// 費用申請
        /// </summary>
        public const string EXPENSE_CLAIM = "ExpenseClaim";
        /// <summary>
        /// 保證票作業
        /// </summary>
        public const string CERTIFICATE_MANAGEMENT = "CertificateManagement";
        /// <summary>
        /// 應付保證票
        /// </summary>
        public const string CC_PAYABLE_CHECK = "CCPayableCheck";
        /// <summary>
        /// 應收保證票
        /// </summary>
        public const string CC_RECEIVABLE_CHECCK = "CCReceivableChecck";


    }
}
