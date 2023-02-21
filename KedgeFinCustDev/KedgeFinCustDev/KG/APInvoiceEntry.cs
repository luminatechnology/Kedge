using PX.Data;
using PX.Objects.CR;
using NM.Util;
using Kedge.DAC;
using Kedge.Util;
using NM.DAC;
using System.Collections;
using System;
using RCGV.GV.Util;
using KG.Util;
using Fin.DAC;
using RCGV.GV.DAC;
using FIN.Util;
using PX.Objects.PM;
using NM;
using PX.Objects.GL;
using RC.Util;
using PX.Objects.EP;
using Kedge;
using PX.Objects.FS;
using System.Linq;

namespace PX.Objects.AP
{
    /**
     * ======2021-02-19 :11955 ======Alton
     * 1.將GVApGuiInvoice.InvoiceType放到發票憑證畫面上, 必填.
     * 2.InvoiceType = 'I', 請連動將GVApGuiInvoice.GuiType設為 '21'
     * 2.InvoiceType = 'I', GVApGuiInvoice.GuiInvoiceNbr長度必須為10碼, 兩碼為英文字母, 後八碼為數字
     * 3.InvoiceType = 'R', 請連動將GVApGuiInvoice.GuiType設為 '27'
     * 4.如果InvoiceType = 'R', 不檢查GVApGuiInvoice.GuiInvoiceNbr是否重複
     * 5.如果InvoiceType = 'R', GVApGuiInvoice.GuiInvoiceNbr不預設長度(最長不可超過DB欄位長度40).
     * 6.GVApGuiInvoice新增一個欄位VendorName, nvarchar(255)-->直接加在RCGV那包
     * 7.請將Vendor改成非必填.
     * 8.將GVApGuiInvoice.VendorName放到發票憑證畫面上, 必填, free keyin. 如果使用者挑選Vendor, 請將Vendor對應的BAccount.AcctName放到GVApGuiInvoice.VendorName.
     * 9.請將GVApGuiInvoice.VendorUniformNumber開放編輯, 只能輸入八碼數字. 原從Vendor連動帶出統編邏輯不變, 但是使用者可以不輸入vendor, 自行輸入GVApGuiInvoice.VendorName及GVApGuiInvoice.VendorUniformNumber
     * 
     * ======2021-02-20 :11955 ======Alton
     * 1.存檔寫入GVApGuiInvoice時：
     *   VoucherCategory : 如果InvoiceType = 'R', 請寫入 "R". 如果InvoiceType = 'I', 請寫入"I"
     *   DeductionCode : 請預設寫入 "1"
     *   Remark : APRegister.DocDesc
     * 2.GVApGuiInvoice.TaxCode, 如果InvoiceType = 'R', 請寫入 "3". 如果InvoiceType = 'I', 請寫入"1"
     * 3.GVApGuiInvoice.Status, 請新增兩個個狀態"Hold"擱置, "Release"已放行
     * 4.計價單寫入GVApGuiInvoice主檔時, 請寫入Status＝"Hold"
     * 
     * ======2021-02-26 :0011940 ======Althea
     * 1.輸入完PaymentPct, 請自動計算PaymentAmount
     * 2.當APinvoice.Status=Open但UsrIsCheckIssued=0時，請將KGBillPayment.PaymentPct及PaymentAmount 兩個欄位設為read-only
     * 3.AP invoice 過帳時，請再對KGBillPayment所有欄位做一次 status＝hold存檔時會做的檢核
     * 4.承接第3點，如果KGBillPayment的PaymentMethod=CHECK 的時候，才需要檢查CheckTitle為必填欄位
     * 
     * ======2021-03-02 :11970 ======Alton
     * 修改舊有邏輯問題
     * 1.declareYear & declareMonth 應該在InvoiceDate異動時重新給予
     * 2.發票重覆應該僅比對InvoiceType = 'I'(改在ProjectCust)
     * 
     * ======2021-03-04 :0011971 ======Althea
     * Override Release:
     * 過帳後產生GL
     * 
     * ======2021-03-10 :11970 ======Alton
     * 1.InvoiceType = 'R'時，VendorUniformNumber改為非必填，請一併修改AP301000的發票憑證tab
     * 2.GVApGuiInvoice.TaxCode 改用GVList.TaxCode
     * 
     * ======2021-03-10 : 代扣稅======Althea
     * 1.ADD: 代扣稅Tab DAC:TWNWHT
     * 2.ADD Cache Attached: TWNWHT.APRefNbr && TWNWHT.APDocType Default Current APInvoice
     * 3.ADD Cache Attached: TWNWHT.UsrPersonalID Default =Current APInvoice.Vendor.UsrPersonalID
     * 
     * =====2021-03-17 : 11955 =======Alton
     * 1.APInvoice存檔時, GVApGuiInvoice.OriCurrencyAmt 請寫入 GVApGuiInvoice.SalesAmt+GVApGuiInvoice.TaxAmt
     * 2.APInvoice存檔時, 請寫入GVApGuiInvoice.Hold = 1
     * 3.當GVApGuiInvoice.InvoiceDate 異動時, 請連動異動GVApGuiInvoice.DeclareYear, GVApGuiInvoice.DeclareMonth
     * 4.APInvoice存檔時, 請寫入GVApGuiInvoiceDetail下列欄位
     *   ItemDesc = APInvoice.DocDesc
     *   Qty = 1
     *   UnitPrice = GVApGuiInvoice.SalesAmt
     *   SalesAmt = GVApGuiInvoice.SalesAmt
     *   TaxAmt = GVApGuiInvoice.TaxAmt
     *   APRefNbr = APRegister.RefNbr
     *   APDocType = APRegister.DocType
     * 
     * ====2021-03-22 : 11996====Alton
     * AP301000 發票憑證, 選擇Vendor, 除了帶出Vendor Name外, 也請把Vendor對應的LocationExtAddress.TaxRegistrationID, 如果Vendor沒有維護TaxRegistrationID, 也請清空GVApGuiInvoiceRef.VendorUniformNumber
     * 
     * ===2021/03/24 : 11992 === Althea
     * 1. Modify DAC : TWNWHT change DAC : TWNWHTTran
     * 2.Add Persist:存檔時,APTran檢查是否有此ValuationType,若無則新增一筆,若有更新Amt即可
     * 3.Add 當TWNWHTTran更新WHTAmt,計算sum到header用trantype區別的amt欄位
     * 
     * ===2021/03/26 ===Althea
     * 1. CostcodeID/TaskID抓專案的成本預算相同工料編號
     * 
     * ===2021/04/06 : 0012002 === Althea
     * APInvoice狀態在closed以前都可以更改/新增/刪除KGBillPayment
     * 若此KGBillPayment以產生過NM票據主檔或回饋檔回傳成功時,將此全部改為readonly
     * 
     * ====2021-04-16 : 12002 ===Alton
     * 請將這段原先寫在 KGBillPayment_row selected 的控制，搬到 APinv_RowSelected 內
     * 
     * ====2021-04-16 : 12015 ===Alton
     * 1. KGBillPayment新增兩個欄位，並顯示在畫面上：
     *     (1) PostageAmt, Decimal (18,6) :
     *               a. 請抓 KGBillPayment.PaymentAmount 做為Base的計算金額
     *               b. 計算邏輯請follow KG傳票摘要設定(KG105001)中的「郵資費設定」Group
     *     (2) ActPayAmt, Decimal (18,6):
     *                 a. ActPayAmt = KGBillPayment.PaymentAmount- PostageAmt
     * 2. 當KGBillPayment.PaymentMethod= TT or CHECK時，才需要計算item 1的兩個欄位，
     *       其他付款方式請直接塞零、不需做運算
     *       
     * ====2021-04-20 : 12002 ===Alton
     * 改為由IsCheckIssued判斷單筆是否能異動 & 刪除
     * 
     * ====2021-04-23 : 12016 ===Alton
     * 	1.KGBillPayment新增VendorID欄位, int
     * 	2.預設放APRegister/EPExpenseClaim表頭的VendorID
     * 	3.使用者異動時, 請連動改變VendorLocationID的lov, 並帶入VendorID對應的AcctName到CheckTitle
     * 	
     * ====2021-04-26 : 11974 ====Alton
     * AP inv過帳完成後，EP/AP的發票資訊變唯讀，需要修改請至進項發票主檔
     * 
     * ====2021-04-26: 口頭 ====Alton
     * 1.ActPayAmt任何情況下都要計算  KGBillPayment.PaymentAmount- PostageAmt
     * 2.PostageAmt追加 禮券也要取得郵資費
     * 
     * ====2021-04-27 ====Alton
     * 當KGBillpayment的付款方式為禮券 & vendorID為員工 則 locationID 為非必填
     * 
     * ====2021-04-28 : 口頭=====Alton
     * 1.郵資費改在該筆存檔時計算
     * 2.禮券的郵資費要判斷是否有專案+是否為供應商，才當支票計算，否則當現金處理
     * 
     * ===2021/04/28 :Alton口頭 ===Althea
     * Add RowUpdated
     * 
     * ===2021/05/03 : 口頭===Althea
     * 1. 代扣稅刪除/更新：(原本在persist寫的)
         * 1. 要重新滾算APTran/EPDetails的對應明細的金額
         * 2. 若明細金額為0要把APTran/EPDetails的那筆刪掉
     * 2. 代扣稅檢核：
	        1. 全部金額都要>0，計算出的netAmt也要>0
	        2. 金額不能為0
     * 3. APTran刪除代扣稅資料：
	        1. 代扣稅的Tab要刪掉對應的代扣稅明細
     * 4. 代扣稅的更動時機：
            1. 只有hold可以更改。
     * 5. 測試代扣稅的Tab有沒有加到APTran/EPDetails
     * 6. Jeff寫的代扣税Action先藏起來（有兩個
     * 7. 二代健保的(總額)金額怪怪的
     * 
     * ===2021/05/05 :口頭 ===Althea
     * APTran刪除代扣稅相關的邏輯改為:
     * 1. APTran.UsrValuationType 若為代扣稅類型或加款,APTran欄位全部ReadOnly
     * 2.刪除代扣稅類型,若金額不為0不可刪除
     * 
     * ====2021-05-06:口頭====Alton
     * 借方調整不檢查 不檢查KGBillPayment
     * 
     * ===2021/05/07 : 0012022 === Althea
     * Add HyperLink :UsrAccConfirmNbr to GL
     * 
     * ===2021/05/11 :口頭===Althea
     * VendorLocation Selector Add LocationFinExt.usrIsReserveAcct to Show
     * 
     * ===2021/05/13 :口頭===Althea
     * Add Import Button
     * 
     * ===2021-05-19 :12044===Alton
     * 	1.請打開發票憑證的稅額, 讓使用者可以微調
     *  2.使用者輸入未稅金額, 自動算出5%稅額當預設值, 但是用者可以微調稅額.
     *  
     * ===2021/05/19 :0012042 ===Althea
     * 1.財務明細下的傳票號碼, APRegister.UsrAccConfirmNbr 請在計價單過完帳後才顯使傳票號碼及hyperlink
     * 
     * ===2021/05/20 :0012047 ===Althea
     *  KGBillPayment.VendorLocationID LOV畫面請將UsrIsReserveAcct, 
     * 請放在第一個欄位.LOV Order by UsrIsReserveAcct(降冪), LocationCD(升冪), 讓有備償戶的LocationLID在第一筆
     * 
     * ===2021/05/31 :0012062 ===Althea
     * Delete:(待討論)
     * 若是從EP產生的AP刪除時要回壓EP.UsrIsVoided = True
     * 
     * ====2021-06-08:口頭 ====Alton
     * 原因：使用者不按順序輸入欄位值，在那邊哀哀叫說稅額被蓋掉了
     * 修正：
     *  1.InvoiceType異動時不觸發taxAmt計算
     *  2.InvoiceType = 收據(R)，TaxCode = 應稅(TAXABLE)
     *  3.開放憑證類別，稅務類別欄位給他們改
     *  
     *  ===2021/06/08: 口頭 ===Althea
     *  代扣稅的tab資訊塞到APTran時機改為在存檔時再做
     * 
     * ====2021-06-11:12090 ====Alton
     * Fix 計價單刪除時會有error
     * 
     * ===2021/06/18 :0012096 ===Althea
     * AddCheckDiscount Btn Visalbe = true
     * 
     * ===2021/06/24 : 0012105 ===Althea
     * PaymentMethod Add E:Auth(授扣)
     * VendorLocation & BankAccountID Selector Add Filter :Same withe Cash
     * Default Auth same with Cash
     * 
     * ====2021-07-06:12126====Alton
     * 1.請不要檢查 PaymentPct加總必須要100, 只要檢核PaymentAmount加總要等於KGBillSummary的TotalAmt
     * 
     * ====2021-08-03:12179====Alton
     * 1.KGBillpayment.paymentAmount的預設值只有在PaymentPct異動時重新計算該筆金額
     * 2.移除掉原先PaymentAmount的滾動計算
     * 
     * === 2021/08/02 :0012176 === Althea
     * Add PaymentMethod = F  , same with Cash
     * 
     * ===2021/08.24 :0012210 === Althea
     * Add if DocType = ppm, Readonly Tab: Deduction
     * 
     * ===2021/09/07 :0012225 === Althea
     * Add TWNWHTTran BranchID Default = APRegister.BranchID
     * 
     * ===2021/09/10 :0012226 === Althea
     * Add When Update DocDate ,TWNWHTTran.PaymentDate = Current(APRegister.DocDate)
     * Mofidy WHTTran.PaymentDate =current DocDate, Enable = false ,viisible = false
     * Add WHTTran.TranDate Default = BusinessDate, Enable = true ,viisible = true
     * 
     *  ====2021-09-15:12233====Alton
     * 	1.發票憑證使用者輸入InvoiceType為發票, 請連動VoucherCategory為"發票(憑證)", 如果InvoiceType為收據,則連動VoucherCategory為"其他憑證"
     * 	2.使用者輸入未稅金額, 請連動計算5%的稅額, 放到TaxAmt, 使用者還可以修改TaxAmt
     * 	----↑相同邏輯般至ProjectCust↑----
     * 	
     * 	===2021/09/23 :0012239 === Althea
     * 	代扣稅WHTAmt金額為0也要加到apTran
     * 	
     *====2021-11-18:12271====Alton
     * 1. 發票格式檢核條件調整：
     *    (1)原先會用GuiType做檢查，請改為用InvoiceType做檢核依據
     *    當InvoviceType=‘發票’，不須卡兩碼英文+八碼數字，僅許檢查總共長度是10碼即可
     *    當InvoiceType='收據‘時，則不需檢查任何格式
     *    (2) EP 及 AP 兩個發票tab都須作上述修正
     *  
     * 2. 發票格式檢核錯誤訊息調整：
     *    目前在KG傳票確認過帳時，若發票格式錯誤，會顯示英文的錯誤訊息如附圖，請協助將錯誤訊息調整成「發票號碼格式有誤，請至該張計價單- 發票憑證 的區塊做檢查」
     * 
     * ====2021-12-22:14:55====Jeff
     * KG bill payment
     * 實付金額和小計檢核錯誤訊息未正確停止和顯示。將該檢查method從Persist()移至RowPersisting<KGBillPayment>()
     * **/
    public class APInvoiceEntryFinExt : PXGraphExtension<APInvoiceEntryCheckDiscountExt, APInvoiceEntry_Extension, APInvoiceEntry>
    {
        #region Button
        #region RefreshPostageAmt
        public PXAction<APInvoice> RefreshPostageAmt;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "重算郵資")]
        protected IEnumerable refreshPostageAmt(PXAdapter adapter)
        {
            foreach (KGBillPayment row in Base1.KGBillPaym.Select())
            {
                SetPostageAmt(row);
                Base1.KGBillPaym.Update(row);
            }
            return adapter.Get();
        }
        #endregion
        #endregion

        #region Override Function
        public delegate int? VendorLocationIDDelegate(int? LocationID);
        [PXOverride]
        public virtual int? VendorLocationID(int? LocationID, VendorLocationIDDelegate basemethod)
        {
            return LocationID;
        }
        public delegate int? VendorLocationIDDelegate1(int? BAccount, string paymentmethod);
        [PXOverride]
        public virtual int? VendorLocationID(int? BAccount, string paymentmethod, VendorLocationIDDelegate1 basemethod)
        {
            return NMLocationUtil.GetDefLocationByPaymentMethod(BAccount, paymentmethod);
        }

        public delegate IEnumerable ReleaseDelegate(PXAdapter adapter);
        //因為不知道為何呼叫baseMethod 會使baseMethod以上的動作做兩次,再做兩次baseMethod,
        //再做basemethod以下的動作,所以用此全域變數避免兩次.....
        int upCount = 0;
        int downCount = 0;
        [PXOverride]
        public IEnumerable Release(PXAdapter adapter, ReleaseDelegate basemethod)
        {
            APRegister register = Base.Document.Current;
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            basemethod(adapter);
            if (downCount == 0)
            {
                //若APDocType = ADR && UsrIsDeductionDoc = true,則沒有分錄也不用產生GL
                //20220418 by louis KGVoucherUtil.CreateGLFromKGVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                if (!(APDocType.DebitAdj.Equals(register.DocType) && registerExt.UsrIsDeductionDoc == true))
                    KGVoucherUtil.CreateGLFromKGVoucher(register, KGVoucherType.CREATEGL, GLStageCode.APBillReleaseA0);
                downCount = downCount + 1;

                //2021/05/06 Add 若計價類型為ADR & OriRefNbr的代扣稅和發票有資料則刪除
                if (APDocType.DebitAdj.Equals(register.DocType) && register.OrigRefNbr != null)
                {
                    PXResultset<GVApGuiInvoiceRef> gui = PXSelect<GVApGuiInvoiceRef,
                        Where<GVApGuiInvoiceRef.refNbr, Equal<Required<APInvoice.refNbr>>>>
                        .Select(Base, register.OrigRefNbr);
                    foreach (GVApGuiInvoiceRef invoiceRef in gui)
                    {
                        PXDatabase.Delete<GVApGuiInvoice>(new PXDataFieldRestrict[]
                    {
                        new PXDataFieldRestrict("RefNbr",invoiceRef.RefNbr),
                    });
                    }

                    PXResultset<TWNWHTTran> tran = PXSelect<TWNWHTTran,
                        Where<TWNWHTTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                        .Select(Base, register.OrigRefNbr);
                    foreach (TWNWHTTran whttran in tran)
                    {
                        PXDatabase.Delete<TWNWHTTran>(new PXDataFieldRestrict[]
                    {
                        new PXDataFieldRestrict("RefNbr",whttran.RefNbr),
                    });
                        //WHTView.Delete(whttran);
                    }
                    //Base.Save.Press();
                }

            }
            return adapter.Get();
        }

        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                // Move the following method to RowPersisting<KGBillPayment>, because this method will conduct many times.
                //CheckKGBillPayment();
                APInvoice invoice = Base.Document.Current;

                //2021/05/06 Add 刪除的連動
                foreach (APRegister register in Base.Document.Cache.Deleted)
                {
                    foreach (TWNWHTTran whttran in WHTView.Cache.Cached)
                    {
                        WHTView.Delete(whttran);
                    }
                }
                //createAPTranForWHT(invoice);
                if (this.WHTView.Cache.Inserted.RowCast<TWNWHTTran>().FirstOrDefault()?.NoteID != null
                    || this.WHTView.Cache.Updated.RowCast<TWNWHTTran>().FirstOrDefault()?.NoteID != null)
                {
                    createAPTranForWHT(invoice);
                }
                baseMethod();
                ts.Complete();
            }
        }
        #endregion

        #region View Select
        //For 代扣稅
        //2021/05/13 Add Import
        [PXImport]
        public PXSelect<TWNWHTTran,
            Where<TWNWHTTran.docType, Equal<Current<APInvoice.docType>>,
                And<TWNWHTTran.refNbr, Equal<Current<APInvoice.refNbr>>>>> WHTView;
        [PXCopyPasteHiddenView]
        public PXSelect<APInvoice,
            Where<APInvoice.docType, Equal<Current<APInvoice.docType>>,
                And<APInvoice.refNbr, Equal<Current<APInvoice.refNbr>>>>> WHTHeader;

        //For Temp暫付款
        public PXSelectReadonly<APRegister,
            Where<APRegister.refNbr, Equal<Current<APInvoice.refNbr>>,
                And<APRegister.docType, Equal<Current<APInvoice.docType>>>>> TempAP;
        public PXSelectGroupBy<KGTempPaymentWriteOff,
            Where<KGTempPaymentWriteOff.origRefNbr, Equal<Current<APInvoice.refNbr>>,
                And<KGTempPaymentWriteOff.docType, Equal<Current<APInvoice.docType>>>>,
            Aggregate<
                GroupBy<KGTempPaymentWriteOff.refNbr,
                GroupBy<KGTempPaymentWriteOff.docType,
                    Sum<KGTempPaymentWriteOff.writeOffAmt>>>>> TempWriteoffTab;


        #endregion

        #region HyperLink
        public PXAction<APInvoice> ViewGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGL()
        {
            APInvoice master = Base.Document.Current;
            APRegister register = GetAPRegister(master.RefNbr);
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            string BatchNbr = "";
            if (register.DocType == APDocType.DebitAdj)
            {
                if (registerExt.UsrIsComsData != true)
                {
                    BatchNbr = registerFinExt.UsrAccConfirmNbr.Remove(12);
                }
                else
                {
                    BatchNbr = registerFinExt.UsrAccConfirmNbr;
                }
            }
            else
            {
                BatchNbr = registerFinExt.UsrAccConfirmNbr;
            }
            Batch batch = PXSelect<Batch,
                Where<BatchExt.usrAccConfirmNbr, Equal<Required<APRegisterFinExt.usrAccConfirmNbrForShow>>>>
                .Select(Base, BatchNbr);
            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
            graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(batch.BatchNbr);
            if (graph.BatchModule.Current == null) return;
            new HyperLinkUtil<JournalEntry>(graph.BatchModule.Current, true);
        }
        #endregion

        #region Event Handlers
        #region APInvoice
        protected void _(Events.RowSelected<APInvoice> e)
        {
            APRegister register = Base.Document.Current;
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            if (register != null)
            {
                if (registerFinExt.UsrIsConfirm == false || registerFinExt.UsrIsConfirm == null)
                    Base.release.SetEnabled(false);
            }
            setBillPayUI();
            SetGuiInvoiceUI();
            SetDeductionUI(register);
            bool isHold = register.Hold ?? false;
            WHTView.AllowDelete = isHold;
            WHTView.AllowInsert = isHold;
            WHTView.AllowUpdate = isHold;

            //2021/05/19 Mantis: 0012042
            //過帳後才顯示UsrAccConfirmNbrForShow即hyperlink
            if (register.Status != APDocStatus.Balanced &&
                register.Status != APDocStatus.Hold &&
                register.Status != APDocStatus.PendingApproval)
            {
                registerFinExt.UsrAccConfirmNbrForShow = registerFinExt.UsrAccConfirmNbr;
            }
            else
            {
                registerFinExt.UsrAccConfirmNbrForShow = null;
            }

            //Temp Write off Tab
            bool IsTmpPayment = registerExt.UsrIsTmpPayment ?? false;
            TempAP.AllowSelect = IsTmpPayment;
            TempWriteoffTab.AllowSelect = IsTmpPayment;
            TempWriteoffTab.AllowInsert = false;
            TempWriteoffTab.AllowDelete = false;
            PXUIFieldAttribute.SetReadOnly(TempWriteoffTab.Cache, null);

            //UsrIsTmpPayment Control Mantis:0012267
            bool istmpCanEdit = !(register.OrigModule == "EP" || register.OrigModule == "PO") && register.Hold == true;
            PXUIFieldAttribute.SetReadOnly<APRegisterExt.usrIsTmpPayment>(Base.Document.Cache, register, !istmpCanEdit);
            SetUsrValuationTypeList();
        }
        protected void _(Events.RowInserted<APInvoice> e)
        {
            APInvoice row = e.Row;
            //APInvoice產生的借方調整要清空以下欄位

            APRegisterExt rExt = e.Cache.GetExtension<APRegisterExt>(row);

            APRegisterFinExt rFinExt = e.Cache.GetExtension<APRegisterFinExt>(row);

            //所有的新單都要清空以下欄位
            rFinExt.UsrConfirmBy = null;
            rFinExt.UsrConfirmDate = null;
            rFinExt.UsrAccConfirmNbr = null;
            rFinExt.UsrIsConfirm = null;

            //類型為INV+ 保留款退回 和 類型為ADR + 是扣款文件 清空代扣欄位
            if ((row.DocType == APDocType.Invoice && row.IsRetainageDocument == true)
                || (row.DocType == APDocType.DebitAdj && rExt.UsrIsDeductionDoc == true))
            {
                rFinExt.UsrLbiAmt = 0;
                rFinExt.UsrWhtAmt = 0;
                rFinExt.UsrGnhi2Amt = 0;
                rFinExt.UsrLbpAmt = 0;
                rFinExt.UsrHliAmt = 0;
            }
        }
        protected void _(Events.RowPersisting<APInvoice> e)
        {

            APInvoice row = e.Row;
            //2021-06-11 add by Alton 添加刪除判斷
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            if (row == null) return;
            APRegister register = GetAPRegister(row.RefNbr);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            //2021/11/17 Add Mantis: 0012267 壓暫付款金額
            if (registerExt.UsrIsTmpPayment == true)
            {
                registerExt.UsrTmpPaymentTotal = row.CuryDocBal;
            }

            //2022-10-28 12363 alton 計價單在擱置移除送審前, 檢核KGBillPayment付款資訊
            //2022-10-31 12363 alton 改為存檔皆檢核
            if (!PreHoldCheckBillPayment())
            {
                throw new PXException("請檢查付款方式!");
            }
        }

        protected void createAPTranForWHT(APInvoice row)
        {
            //APInvoice row = e.Row;
            //2021-06-11 add by Alton 添加刪除判斷
            //if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            if (row == null) return;
            APRegister register = GetAPRegister(row.RefNbr);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            decimal UsrWhtAmt = registerFinExt.UsrWhtAmt ?? 0;
            decimal UsrGnhi2Amt = registerFinExt.UsrGnhi2Amt ?? 0;
            decimal UsrLbiAmt = registerFinExt.UsrLbiAmt ?? 0;
            decimal UsrLbpAmt = registerFinExt.UsrLbpAmt ?? 0;
            decimal UsrHliAmt = registerFinExt.UsrHliAmt ?? 0;
            //2021/09/23 Mantis: 0012239 金額是0也要塞到APTran
            //檢查TWNWHTTran是否存在各類型的資料
            TWNWHTTran exsitWHTTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.WHT);
            TWNWHTTran exsitLbiTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.LBI);
            TWNWHTTran exsitLbpTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.LBP);
            TWNWHTTran exsitHliTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.HLI);

            APTran oldWHTTran = GetAPTranofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.WHT));
            APTran oldGnhi2Tran = GetAPTranofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.GHI));
            APTran oldLbiTran = GetAPTranofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.LBI));
            APTran oldLbpTran = GetAPTranofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.LBP));
            APTran oldHliTran = GetAPTranofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.HLI));

            if (exsitWHTTran != null)
            {
                if (oldWHTTran != null)
                    UpdateAPTranAmt(FinStringList.TranType.WHT, UsrWhtAmt);
                else
                    CreateAPTranFromWHTTab(FinStringList.TranType.WHT, UsrWhtAmt);
            }
            else
            {
                if (oldWHTTran != null)
                {
                    UpdateAPTranAmt(FinStringList.TranType.WHT, 0m);
                    Base.Transactions.Delete(oldWHTTran);
                }
            }
            if (exsitWHTTran != null)
            {
                if (oldGnhi2Tran != null)
                    UpdateAPTranAmt(FinStringList.TranType.GHI, UsrGnhi2Amt);
                else
                    CreateAPTranFromWHTTab(FinStringList.TranType.GHI, UsrGnhi2Amt);
            }
            else
            {
                if (oldGnhi2Tran != null)
                {
                    UpdateAPTranAmt(FinStringList.TranType.GHI, 0m);
                    Base.Transactions.Delete(oldGnhi2Tran);
                }
            }
            if (exsitLbiTran != null)
            {
                if (oldLbiTran != null)
                    UpdateAPTranAmt(FinStringList.TranType.LBI, UsrLbiAmt);
                else
                    CreateAPTranFromWHTTab(FinStringList.TranType.LBI, UsrLbiAmt);
            }
            else
            {
                if (oldLbiTran != null)
                {
                    UpdateAPTranAmt(FinStringList.TranType.LBI, 0m);
                    Base.Transactions.Delete(oldLbiTran);
                }
            }
            if (exsitLbpTran != null)
            {
                if (oldLbpTran != null)
                    UpdateAPTranAmt(FinStringList.TranType.LBP, UsrLbpAmt);
                else
                    CreateAPTranFromWHTTab(FinStringList.TranType.LBP, UsrLbpAmt);
            }
            else
            {
                if (oldLbpTran != null)
                {
                    UpdateAPTranAmt(FinStringList.TranType.LBP, 0m);
                    Base.Transactions.Delete(oldLbpTran);
                }
            }
            if (exsitHliTran != null)
            {
                if (oldHliTran != null)
                    UpdateAPTranAmt(FinStringList.TranType.HLI, UsrHliAmt);
                else
                    CreateAPTranFromWHTTab(FinStringList.TranType.HLI, UsrHliAmt);
            }
            else
            {
                if (oldHliTran != null)
                {
                    UpdateAPTranAmt(FinStringList.TranType.HLI, 0m);
                    Base.Transactions.Delete(oldHliTran);
                }
            }
            //2021/10/06 Delete Louis said edit by Althea
            //Delete APTran, When copy, system add APTran but UsrValuationType = B (not WHT Type)
            /*
            foreach (APTran tran in Base.Transactions.Cache.Cached)
            {
                APTranExt tranExt = PXCache<APTran>.GetExtension<APTranExt>(tran);
                KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
                if (tranExt.UsrValuationType == ValuationTypeStringList.B &&
                    (tran.InventoryID == setUp.HealthInsInventoryID ||
                    tran.InventoryID == setUp.KGIndividualTaxInventoryID ||
                    tran.InventoryID == setUp.KGSupplementaryTaxInventoryID ||
                    tran.InventoryID == setUp.LaborInsInventoryID ||
                    tran.InventoryID == setUp.PensionInventoryID))
                {
                    Base.Transactions.Delete(tran);
                }
            }
            */

        }
        protected void _(Events.FieldUpdated<APInvoice, APInvoice.hold> e, PXFieldUpdated baseMethod)
        {
            APInvoice row = e.Row;
            if (row == null || row.Hold == null) return;
            //2022-10-28 12363 alton 計價單在擱置移除送審前, 檢核KGBillPayment付款資訊
            if (PreHoldCheckBillPayment())
            {
                baseMethod.Invoke(e.Cache, e.Args);
            }
            else
            {
                throw new PXException("請檢查付款方式!");
            }

            APRegisterFinExt registerFinExt = e.Cache.GetExtension<APRegisterFinExt>(row);
            if (row.Hold == true)
            {
                registerFinExt.UsrIsConfirm = false;
                registerFinExt.UsrConfirmBy = null;
                registerFinExt.UsrConfirmDate = null;
                registerFinExt.UsrAccConfirmNbr = null;
                registerFinExt.UsrAccConfirmNbrForShow = null;
            }
        }
        protected void _(Events.FieldUpdated<APInvoice, APInvoice.docDate> e)
        {
            APInvoice row = e.Row;
            if (row == null) return;
            foreach (TWNWHTTran whttran in WHTView.Select())
            {
                whttran.PaymentDate = row.DocDate;
                WHTView.Update(whttran);
            }
        }

        /*待討論
        protected void _(Events.RowDeleted<APInvoice> e)
        {
            APInvoice master = e.Row;
            if (master == null) return;
            //2021/05/31 Add Mantis:0012062 
            if (master.CreatedByScreenID == "EP301000")
            {
                EPExpenseClaim claim = GetEPClaimFromAPRefNbr(master.RefNbr);
                if (claim == null) return;
                PXUpdate<Set<EPExpenseClaimExt.usrIsVoided, True>,
                    EPExpenseClaim,
                    Where<EPExpenseClaim.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                    .Update(Base, claim.RefNbr);
            }
        }
        */
        #endregion

        #region APTran
        protected virtual void _(Events.RowSelected<APTran> e)
        {
            APTran row = e.Row as APTran;
            if (row == null) return;
            SetAPTranUI(row);
            APTranExt rowFinExt = PXCache<APTran>.GetExtension<APTranExt>(row);

        }
        protected virtual void _(Events.RowDeleting<APTran> e)
        {
            APTran row = e.Row as APTran;
            APTranExt rowExt = PXCache<APTran>.GetExtension<APTranExt>(row);
            APInvoice master = Base.Document.Current;
            if (row == null) return;
            if (Base.Document.Cache.GetStatus(master) != PXEntryStatus.Deleted)
            {
                //若型態為代扣稅類型,金額為0才可以刪除
                if (rowExt.UsrValuationType == ValuationTypeStringList.WITH_TAX ||//20230103 mantis:12386 w -> withTax
                    rowExt.UsrValuationType == ValuationTypeStringList.H ||
                    rowExt.UsrValuationType == ValuationTypeStringList.L ||
                    rowExt.UsrValuationType == ValuationTypeStringList.N ||
                    rowExt.UsrValuationType == ValuationTypeStringList.SECOND)//20230103 mantis:12386 w -> second
                {
                    if (row.CuryLineAmt != 0) throw new Exception("此資料不可刪除!");
                }
            }

        }
        #endregion

        #region KGBillPayment

        protected virtual void _(Events.RowPersisting<KGBillPayment> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            //20220516 只有在Hold跟Balanced下才重新計算郵資
            //20220622 狀態Open下也要重新計算郵資
            APRegister register = Base.Document.Current;
            if (register.Status.Equals(APDocStatus.Hold)
                || register.Status.Equals(APDocStatus.Balanced)
                || register.Status.Equals(APDocStatus.Open))
            {
                SetPostageAmt(row);
            }

            // Add the validation from Persist().
            CheckKGBillPayment(row);
        }

        protected virtual void _(Events.RowSelected<KGBillPayment> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            //20220316 add by louis 開啟PaymentDate在資金作業前可以編輯
            APRegister register = Base.Document.Current;
            APRegisterFinExt registerfINExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            KGBillPaymentExt rowExt = PXCache<KGBillPayment>.GetExtension<KGBillPaymentExt>(row);
            if (rowExt.UsrIsCheckIssued != true && registerfINExt.UsrIsConfirm == true)
            {
                PXUIFieldAttribute.SetEnabled<KGBillPayment.paymentDate>(e.Cache, row, true);
            }

            BAccount2 ba2 = (BAccount2)PXSelectorAttribute.Select<KGBillPayment.vendorID>(e.Cache, row);
            String type = (ba2?.Type ?? BAccountType.VendorType);
            bool isVendor = type == BAccountType.VendorType;
            bool isGif = row.PaymentMethod == PaymentMethod.D;//禮卷
            SetUIRequired<KGBillPayment.vendorLocationID>(e.Cache, row, isVendor || !isGif);

        }

        protected void _(Events.RowDeleting<KGBillPayment> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            APRegister register = Base.Document.Current;
            if (row == null) return;
            KGBillPaymentExt rowExt = e.Cache.GetExtension<KGBillPaymentExt>(row);
            if (rowExt.UsrIsCheckIssued == true)
            {
                e.Cancel = true;
                throw new PXException("已產生電匯或票據資訊，不可刪除");
                //add by louis 20220624已過帳且有郵資的付款方法不能刪除
            }
            else if (row.IsPostageFree == false && register.Status.Equals(APDocStatus.Open))
            {
                e.Cancel = true;
                throw new PXException("有郵資且已過帳，不可刪除");
            }

        }

        protected void _(Events.RowInserting<KGBillPayment> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            APRegister register = Base.Document.Current;
            if (register.Status.Equals(APDocStatus.Open))
            {
                row.IsPostageFree = true;
            }
        }

        protected virtual void KGBillPayment_PaymentMethod_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;

            if (row != null && (string)e.OldValue != row.PaymentMethod)
            {
                ///<remarks> Mantis [0012324]</remarks>
                if (row.PaymentMethod == PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
                {
                    //cache.SetValueExt<KGBillPayment.vendorLocationID>(row, GetBAccount(row.VendorID)?.DefLocationID);
                    cache.SetValueExt<KGBillPayment.vendorLocationID>(row, NMLocationUtil.GetDefLocationByPaymentMethod(row.VendorID, "A"));
                }
                else
                {
                    cache.SetValueExt<KGBillPayment.vendorLocationID>(row, NMLocationUtil.GetDefLocationByPaymentMethod(row.VendorID, row.PaymentMethod));
                }
            }
        }

        protected virtual void KGBillPayment_VendorLocationID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            if (row.PaymentMethod == PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
            {
                //BAccount baccount = GetBAccount(row.VendorID);
                e.NewValue = NMLocationUtil.GetDefLocationByPaymentMethod(row.VendorID, "A");
                //e.NewValue = baccount?.DefLocationID;
            }
            else
            {
                e.NewValue = NMLocationUtil.GetDefLocationByPaymentMethod(row.VendorID, row.PaymentMethod);
            }
        }

        protected virtual void KGBillPayment_VendorLocationID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            cache.SetDefaultExt<KGBillPayment.bankAccountID>(row);
        }

        protected virtual void KGBillPayment_BankAccountID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            if (row.VendorLocationID != null)
            {
                Location location = GetLocation(row.VendorLocationID);
                if (location.VCashAccountID == null) return;
                string PaymentMethodID = "";
                if (row.PaymentMethod == PaymentMethod.A || row.PaymentMethod == PaymentMethod.D)
                {
                    PaymentMethodID = "CHECK";
                }
                else if (row.PaymentMethod == PaymentMethod.B)
                {
                    PaymentMethodID = "TT";
                }
                else if (row.PaymentMethod == PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
                {
                    PaymentMethodID = location.PaymentMethodID;
                }

                NMBankAccount account = GetBankAccount(location.VCashAccountID, PaymentMethodID);
                e.NewValue = account?.BankAccountID;
            }
            else
                e.NewValue = null;

        }

        protected virtual void KGBillPayment_PaymentPct_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            KGBillSummary sum = Base1.getKGBillSummary();
            decimal paymentAmt = (sum?.TotalAmt ?? 0) * (row.PaymentPct ?? 0) / 100;
            paymentAmt = System.Math.Round(paymentAmt, 0, MidpointRounding.AwayFromZero);
            cache.SetValueExt<KGBillPayment.paymentAmount>(row, paymentAmt);
        }

        protected virtual void _(Events.FieldUpdated<KGBillPayment, KGBillPayment.vendorID> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<KGBillPayment.vendorLocationID>(row);
            e.Cache.SetDefaultExt<KGBillPayment.checkTitle>(row);
        }

        protected virtual void _(Events.FieldDefaulting<KGBillPayment, KGBillPayment.checkTitle> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            BAccount2 b2 = (BAccount2)PXSelectorAttribute.Select<KGBillPayment.vendorID>(e.Cache, row);
            e.NewValue = b2?.AcctName;
        }

        protected virtual void _(Events.FieldUpdated<KGBillPayment, KGBillPayment.isPostageFree> e)
        {
            KGBillPayment row = (KGBillPayment)e.Row;
            if (row == null) return;
            SetPostageAmt(row);
        }
        #endregion

        #region GVApGuiInvoiceRef
        protected virtual void _(Events.RowPersisting<GVApGuiInvoiceRef> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            //2021-04-07 mark by alton Hold移除
            //if (row.Hold == null) row.Hold = true;
            row.TotalAmt = row.SalesAmt + row.TaxAmt;
            //檢核
            if (!checkGVApGuiInvoiceRef(row))
                throw new PXException("GVApGuiInvoiceRef has error.");
        }


        //protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceRef, GVApGuiInvoiceRef.vendor> e)
        //{
        //    GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
        //    if (row == null) return;
        //    e.Cache.SetDefaultExt<GVApGuiInvoiceRef.vendorName>(row);
        //}

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceRef, GVApGuiInvoiceRef.vendorName> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            PXCache cache = Base1.GVApGuiInvoiceRefs.Cache;
            BAccount b = (BAccount)PXSelectorAttribute.Select<GVApGuiInvoiceRef.vendor>(cache, row);
            e.NewValue = b?.AcctName;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceRef, GVApGuiInvoiceRef.declareYear> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceRef, GVApGuiInvoiceRef.declareMonth> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Month;
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceRef, GVApGuiInvoiceRef.invoiceDate> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiInvoiceRef.declareYear>(row);
            e.Cache.SetDefaultExt<GVApGuiInvoiceRef.declareMonth>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceRef.remark> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            e.NewValue = Base.Document.Current.DocDesc;
        }

        #endregion

        #region TWNWHTTran
        protected virtual void _(Events.RowSelected<TWNWHTTran> e)
        {
            TWNWHTTran row = e.Row as TWNWHTTran;
            if (row == null) return;
            setWHTTranUI();
        }
        protected virtual void _(Events.RowDeleted<TWNWHTTran> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            APRegister register = Base.Document.Current;
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            //APTran tran = GetAPTranofValuationType(register.RefNbr, ValuationType(row.TranType));
            //if (tran == null) return;

            KGBillSummary billSummary = Base1.SummaryAmtFilters.Current;
            switch (row.TranType)
            {
                case FinStringList.TranType.WHT:
                    registerFinExt.UsrWhtAmt = (registerFinExt.UsrWhtAmt ?? 0) - row.WHTAmt ?? 0;
                    billSummary.WhtAmt = registerFinExt.UsrWhtAmt;
                    //Base.Transactions.SetValueExt<APTran.curyUnitCost>(tran, registerFinExt.UsrWhtAmt);
                    if (row.GNHI2Amt != null)
                    {
                        registerFinExt.UsrGnhi2Amt = (registerFinExt.UsrGnhi2Amt ?? 0) - row.GNHI2Amt ?? 0;
                        billSummary.Gnhi2Amt = registerFinExt.UsrGnhi2Amt;

                        /*
                        APTran tranGNHI2 = GetAPTranofValuationType(register.RefNbr, ValuationType(FinStringList.TranType.GHI));
                        Base.Transactions.Cache.SetValueExt<APTran.curyLineAmt>(tranGNHI2, registerFinExt.UsrGnhi2Amt);
                        if (registerFinExt.UsrGnhi2Amt == 0)
                        {
                            Base.Transactions.Cache.Update(tranGNHI2);
                            Base.Transactions.Cache.Delete(tranGNHI2);
                        }
                        */
                    }
                    break;
                case FinStringList.TranType.LBI:
                    registerFinExt.UsrLbiAmt = (registerFinExt.UsrLbiAmt ?? 0) - row.WHTAmt ?? 0;
                    billSummary.LbiAmt = registerFinExt.UsrLbiAmt ?? 0;
                    //Base.Transactions.Cache.SetValueExt<APTran.curyUnitCost>(tran, registerFinExt.UsrLbiAmt ?? 0);
                    break;
                case FinStringList.TranType.HLI:
                    registerFinExt.UsrHliAmt = (registerFinExt.UsrHliAmt ?? 0) - row.WHTAmt ?? 0;
                    billSummary.HliAmt = registerFinExt.UsrHliAmt ?? 0;
                    //Base.Transactions.Cache.SetValueExt<APTran.curyUnitCost>(tran, registerFinExt.UsrHliAmt);
                    break;
                case FinStringList.TranType.LBP:
                    registerFinExt.UsrLbpAmt = (registerFinExt.UsrLbpAmt ?? 0) - row.WHTAmt ?? 0;
                    billSummary.LbpAmt = registerFinExt.UsrLbpAmt ?? 0;
                    //Base.Transactions.Cache.SetValueExt<APTran.curyUnitCost>(tran, registerFinExt.UsrLbpAmt);
                    break;
            }
            /*
            Base.Transactions.Cache.Update(tran);
            if (tran.CuryLineAmt == 0)
            {
                Base.Transactions.Cache.Delete(tran);
            }
            */
        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran.tranType> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            APRegister register = Base.Document.Current;
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            //處裡舊資料 APTran刪除或更新金額
            //若變更TranType,清空有些WHT才需填的欄位
            try
            {
                if ((string)e.OldValue != null)
                {
                    decimal? OldAmt = 0m;
                    switch ((string)e.OldValue)
                    {
                        case FinStringList.TranType.WHT:
                            registerFinExt.UsrWhtAmt = (registerFinExt.UsrWhtAmt ?? 0) - row.WHTAmt;
                            OldAmt = registerFinExt.UsrWhtAmt;
                            break;
                        case FinStringList.TranType.LBI:
                            registerFinExt.UsrLbiAmt = (registerFinExt.UsrLbiAmt ?? 0) - row.WHTAmt;
                            OldAmt = registerFinExt.UsrLbiAmt;
                            break;
                        case FinStringList.TranType.HLI:
                            registerFinExt.UsrHliAmt = (registerFinExt.UsrHliAmt ?? 0) - row.WHTAmt;
                            OldAmt = registerFinExt.UsrHliAmt;
                            break;
                        case FinStringList.TranType.LBP:
                            registerFinExt.UsrLbpAmt = (registerFinExt.UsrLbpAmt ?? 0) - row.WHTAmt;
                            OldAmt = registerFinExt.UsrLbpAmt;
                            break;
                    }
                    //20210/06/08 Take Off Move to Persist
                    /*
                    APTran oldtran = GetAPTranofValuationType(register.RefNbr, ValuationType((string)e.OldValue));
                    UpdateAPTranAmt((string)e.OldValue, OldAmt);
                    if (oldtran != null && OldAmt == 0)
                        Base.Transactions.Cache.Delete(oldtran);

                    WHTView.Cache.SetValueExt<TWNWHTTran.gNHI2Amt>(row, 0m);
                    APTran oldGNHI2tran = GetAPTranofValuationType(register.RefNbr, ValuationType(FinStringList.TranType.GHI));
                    if (oldGNHI2tran != null && oldGNHI2tran.CuryLineAmt == 0)
                        Base.Transactions.Cache.Delete(oldGNHI2tran);
                    */
                }

                // 處理新的TranType APTran新增或更新金額
                //先檢查APTran是否有相同valuationType的資料,若為null 則新增一筆,若有資料則update amt
                decimal? Amt = 0;
                switch (row.TranType)
                {
                    case FinStringList.TranType.WHT:
                        Amt = (registerFinExt.UsrWhtAmt ?? 0);
                        break;
                    case FinStringList.TranType.LBI:
                        Amt = (registerFinExt.UsrLbiAmt ?? 0);
                        break;
                    case FinStringList.TranType.HLI:
                        Amt = (registerFinExt.UsrHliAmt ?? 0);
                        break;
                    case FinStringList.TranType.LBP:
                        Amt = (registerFinExt.UsrLbpAmt ?? 0);
                        break;
                }
                //20210/06/08 Take Off Move to Persist
                /*
                APTran tran = GetAPTranofValuationType(register.RefNbr, ValuationType(row.TranType));
                if (tran == null)
                    CreateAPTranFromWHTTab(row.TranType, Amt);
                else
                    UpdateAPTranAmt(row.TranType, Amt);
                if (row.TranType == FinStringList.TranType.WHT && row.GNHI2Amt != null)
                {
                    APTran GHI2tran = GetAPTranofValuationType(register.RefNbr, ValuationType(FinStringList.TranType.WHT));
                    if (GHI2tran == null)
                        CreateAPTranFromWHTTab(row.TranType, registerFinExt.UsrGnhi2Amt);
                    else
                        UpdateAPTranAmt(FinStringList.TranType.WHT, registerFinExt.UsrGnhi2Amt);
                }
                */
            }
            catch (Exception a)
            {
                row.TranType = null;
                throw a;
            }


            //Default
            
            if (row.TranType == FinStringList.TranType.WHT)
            {
                /**
                if (register.VendorID != null)
                {
                    //2021/05/12 Add
                    BAccount2 bAccount = GetBAccount(register.VendorID);
                    if (bAccount.Type == BAccountType.EmployeeType)
                    {
                        row.PayeeName = bAccount.AcctName;
                        VendorPaymentMethodDetail vendorPaymentMethodDetai = GetVendorPaymentMethodDetail(register.VendorID);
                        row.PersonalID = vendorPaymentMethodDetai?.DetailValue ?? "";
                        Address address = GetAddress(bAccount.DefAddressID);
                        row.PayeeAddr = address?.City ?? "" + address?.AddressLine1 ?? "" + address?.AddressLine2 ?? "";
                    }
                    else
                    {
                        Vendor vendor = GetVendor(register.VendorID);
                        VendorExt vendorExt = PXCache<Vendor>.GetExtension<VendorExt>(vendor);
                        Contact contact = GetContact(vendor?.DefContactID);
                        row.PayeeName = contact?.Attention ?? "";
                        row.PersonalID = vendorExt?.UsrPersonalID ?? "";
                        Address address = GetAddress(vendor?.DefAddressID);
                        row.PayeeAddr = address?.City ?? "" + address?.AddressLine1 ?? "" + address?.AddressLine2 ?? "";
                    }
                }
                **/

                row.TypeOfIn = "1";
                row.WHTFmtCode = "50";
                row.WHTFmtSub = "0";
                row.PaymentDate = register.DocDate;//Base.Accessinfo.BusinessDate;
                row.TranDate = Base.Accessinfo.BusinessDate;
                row.Jurisdiction = "TW";
                row.TaxationAgreements = "0";
            }
            else
            {
                row.PersonalID = null;
                row.PayeeName = null;
                row.PayeeAddr = null;
                row.PropertyID = null;
                row.TypeOfIn = null;
                row.WHTFmtCode = null;
                row.WHTFmtSub = null;
                row.PayAmt = null;
                row.NetAmt = null;
                row.Jurisdiction = null;
                row.TaxationAgreements = null;
                row.GNHI2Amt = null;
            }
            row.WHTAmt = 0;


        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran.payAmt> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            //2021/05/06 Add Check
            CheckAmt(row, 2, row.PayAmt ?? 0m);

            if (e.NewValue != null)
            {
                row.NetAmt = (decimal?)e.NewValue - (row.WHTAmt ?? 0m);
            }
        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran.wHTAmt> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            //2021/05/06 Add Check
            if (row.WHTAmt == null) row.WHTAmt = 0m;
            CheckAmt(row, 1, row.WHTAmt ?? 0m);
            if (row.TranType == null)
                WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.tranType>(row, row.TranType,
                    new PXSetPropertyException(ErrorTranType, PXErrorLevel.RowError));

            APRegister register = Base.Document.Current;
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            KGBillSummary billSummary = Base1.SummaryAmtFilters.Current;
            if (billSummary == null)
            {
                billSummary = Base1.SummaryAmtFilters.Insert();
            }
            if (row.WHTAmt != null)
            {
                decimal? OldWHTAmt = (decimal?)e.OldValue ?? 0;
                decimal? NewWHTAmt = (decimal?)row.WHTAmt ?? 0;
                decimal? TotalGNHI2Amt = 0;
                //For APTran CuryUnicost
                decimal? Amt = 0;

                if (OldWHTAmt != row.WHTAmt)
                    TotalGNHI2Amt = TotalGNHI2Amt - OldWHTAmt + NewWHTAmt;
                switch (row.TranType)
                {
                    case FinStringList.TranType.WHT:
                        registerFinExt.UsrWhtAmt = (registerFinExt.UsrWhtAmt ?? 0) + TotalGNHI2Amt;
                        billSummary.WhtAmt = registerFinExt.UsrWhtAmt ?? 0;
                        row.NetAmt = (row.PayAmt ?? 0) - (row.WHTAmt ?? 0);
                        Amt = registerFinExt.UsrWhtAmt ?? 0;
                        break;
                    case FinStringList.TranType.LBI:
                        registerFinExt.UsrLbiAmt = (registerFinExt.UsrLbiAmt ?? 0) + TotalGNHI2Amt;
                        billSummary.LbiAmt = registerFinExt.UsrLbiAmt ?? 0;
                        Amt = registerFinExt.UsrLbiAmt ?? 0;
                        break;
                    case FinStringList.TranType.HLI:
                        registerFinExt.UsrHliAmt = (registerFinExt.UsrHliAmt ?? 0) + TotalGNHI2Amt;
                        billSummary.HliAmt = registerFinExt.UsrHliAmt ?? 0;
                        Amt = registerFinExt.UsrHliAmt ?? 0;
                        break;
                    case FinStringList.TranType.LBP:
                        registerFinExt.UsrLbpAmt = (registerFinExt.UsrLbpAmt ?? 0) + TotalGNHI2Amt;
                        billSummary.LbpAmt = registerFinExt.UsrLbpAmt ?? 0;
                        Amt = registerFinExt.UsrLbpAmt ?? 0;
                        break;
                }

                //2021/04/29 Add
                //20210/06/08 Take Off Move to Persist
                /*
                APTran tran = GetAPTranofValuationType(register.RefNbr, ValuationType(row.TranType));
                Base.Transactions.Cache.SetValueExt<APTran.curyUnitCost>(tran, Amt);
                Base.Transactions.Cache.SetDefaultExt<APTran.curyLineAmt>(tran);
                Base.Transactions.Cache.Update(tran);
                */
            }
        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran.gNHI2Amt> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            if (row.GNHI2Amt == null) row.GNHI2Amt = 0m;
            //2021/05/06 Add check
            CheckAmt(row, 4, row.GNHI2Amt ?? 0);

            APRegister register = Base.Document.Current;
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            KGBillSummary billSummary = Base1.SummaryAmtFilters.Current;

            if (row.GNHI2Amt != null)
            {
                decimal? OldGNHI2Amt = (decimal?)e.OldValue ?? 0;
                decimal? NewGNHI2Amt = (decimal?)row.GNHI2Amt ?? 0;
                decimal? TotalGNHI2Amt = 0;
                if (OldGNHI2Amt != row.GNHI2Amt)
                    TotalGNHI2Amt = (registerFinExt.UsrGnhi2Amt ?? 0) - OldGNHI2Amt + NewGNHI2Amt;
                registerFinExt.UsrGnhi2Amt = TotalGNHI2Amt;
                billSummary.Gnhi2Amt = TotalGNHI2Amt;

                //20210/06/08 Take Off Move to Persist
                /*
                APTran tran = GetAPTranofValuationType(register.RefNbr, ValuationType(FinStringList.TranType.GHI));
                if (tran == null)
                    CreateAPTranFromWHTTab(FinStringList.TranType.GHI, row.GNHI2Amt);
                else
                {
                    Base.Transactions.Cache.SetValueExt<APTran.curyUnitCost>(tran, TotalGNHI2Amt);
                    Base.Transactions.Cache.SetDefaultExt<APTran.curyLineAmt>(tran);
                    Base.Transactions.Cache.Update(tran);
                }
                */

                //Base.Transactions.SetValueExt<APTran.curyUnitCost>(tran, TotalGNHI2Amt);
            }
        }

        //2022-11-16 Alon 12375
        //2022-12-23 Louis 修正Baccount== null錯誤, 及住址只抓address2 戶籍地址
        protected virtual void _(Events.FieldUpdated<TWNWHTTran, TWNWHTTran.personalID> e)
        {
            if (e.Row == null) return;
            if (e.Row.PersonalID != null)
            {
                //透過CD抓
                BAccount baccount = GetBAccountByCD(e.Row.PersonalID) ?? GetBAccountByCategoryID(e.Row.PersonalID);
                if (baccount != null)
                {
                    Address addr = Address.PK.Find(Base, baccount.DefAddressID);
                    e.Cache.SetValueExt<TWNWHTTran.payeeName>(e.Row, baccount.AcctName);
                    //修正只抓address2 戶籍地址
                    //e.Cache.SetValueExt<TWNWHTTran.payeeAddr>(e.Row, (addr?.AddressLine1 ?? "") + (addr?.AddressLine2 ?? ""));
                    e.Cache.SetValueExt<TWNWHTTran.payeeAddr>(e.Row, (addr?.AddressLine2 ?? ""));
                }
            }
        }
        protected virtual void _(Events.RowPersisted<TWNWHTTran> e)
        {
            TWNWHTTran row = e.Row;
            if (row == null) return;

            if (CheckAmt(row, 1, row.WHTAmt ?? 0))
                throw new Exception("代扣稅" + ErrorAmt);

            if (row.TranType == FinStringList.TranType.WHT)
            {
                if (CheckAmt(row, 2, row.PayAmt ?? 0))
                    throw new Exception("代扣稅" + ErrorAmt);
                if (CheckAmt(row, 3, row.NetAmt ?? 0))
                    throw new Exception("代扣稅" + ErrorAmt);
                if (CheckAmt(row, 4, row.GNHI2Amt ?? 0))
                    throw new Exception("代扣稅" + ErrorAmt);
            }
        }
        #endregion

        #endregion

        #region Method
        protected virtual bool PreHoldCheckBillPayment()
        {
            bool check = true;
            foreach (KGBillPayment payment in Base1.KGBillPaym.Select())
            {
                try
                {
                    NMLocationUtil.CheckKGBillPaymentLocation(payment);
                }
                catch (Exception e)
                {
                    check = false;
                    Base1.KGBillPaym.Cache.RaiseExceptionHandling<KGBillPayment.vendorLocationID>(payment, payment.VendorLocationID,
                        new PXSetPropertyException(e.Message, PXErrorLevel.RowError));
                }
            }
            return check;
        }


        /// <summary>
        /// Cache exception handling requires the correct data to return exception.
        /// </summary>
        /// <param name="row"></param>
        public void CheckKGBillPayment(KGBillPayment row, bool isComs = false)
        {
            APRegister register = Base.Document.Current;
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            //借方調整不檢查
            if (register == null ||
                Base.Document.Cache.GetStatus(register) == PXEntryStatus.Deleted ||
                register.Status == APDocStatus.Hold ||
                register.DocType == APDocType.DebitAdj ||
                //modify by louis 20211221 修改用OrigModule判斷計價單來源是不是費用申請
                //register.OrigModule == "EP" ||
                //modify by louis 20211221 新增來自Coms資料不要檢查付款方法
                (registerExt.UsrIsComsData == true && isComs == false)
                )
                return;
            Decimal? sumPaymentPct = 0;
            Decimal? sumPaymenyAmount = 0;
            //KGBillPayment payments = null;
            KGBillSummary sum = Base1.getKGBillSummary();
            //modify by louis 20211216修改因為db中的資料就不相同, 會一直檢核不過
            foreach (KGBillPayment kgBillPayment in Base1.KGBillPaym.Select())
            //foreach (KGBillPayment kgBillPayment in Base1.KGBillPaym.Cache.Cached)
            {
                sumPaymentPct += (kgBillPayment.PaymentPct ?? 0);
                sumPaymenyAmount += (kgBillPayment.PaymentAmount ?? 0);
                //payments = kgBillPayment;
            }
            //2021-07-06 mark By Alton :12126   
            //if (sumPaymentPct != 100)
            //{
            if (row == null)
            {
                if (isComs == false)
                {
                    Base.Document.Cache.RaiseExceptionHandling<APInvoice.hold>(register, register.Hold,
                          new PXSetPropertyException("必須要有付款方式", PXErrorLevel.RowError));
                }
                else
                {
                    Base.Document.Cache.RaiseExceptionHandling<APInvoice.hold>(register, register.Hold,
                              new PXSetPropertyException("計價單: " + row.RefNbr + " 必須要有付款方式", PXErrorLevel.RowError));
                }

                //return;
            }
            //2021-07-06 mark By Alton :12126   
            //    else
            //    {
            //        Base1.KGBillPaym.Cache.RaiseExceptionHandling<KGBillPayment.paymentPct>(payments, payments.PaymentPct,
            //              new PXSetPropertyException("付款方式-付款比例加總要為100", PXErrorLevel.RowError));
            //        return;
            //    }
            //}
            if (row != null && sumPaymenyAmount != sum.TotalAmt)
            {
                if (isComs == false)
                {
                    Base1.KGBillPaym.Cache.RaiseExceptionHandling<KGBillPayment.paymentAmount>(row, row.PaymentAmount,
                                                                                           new PXSetPropertyException("付款金額加總要與實付金額-小計相同!", PXErrorLevel.RowError));
                }
                else
                {
                    Base1.KGBillPaym.Cache.RaiseExceptionHandling<KGBillPayment.paymentAmount>(row, row.PaymentAmount,
                                                                                               new PXSetPropertyException("計價單: " + row.RefNbr + " 付款金額加總要與實付金額-小計相同!", PXErrorLevel.RowError));
                }

                //return;
            }
        }

        /// <summary>
        /// 計算郵資費
        /// </summary>
        /// <param name="row"></param>
        public void SetPostageAmt(KGBillPayment row)
        {
            int? projectID = Base.Document.Current.ProjectID;
            PXCache cache = Base1.KGBillPaym.Cache;
            decimal postageAmt = 0m;
            if ((row.IsPostageFree ?? false) == false)
            {
                switch (row.PaymentMethod)
                {
                    case PaymentMethod.A://支票
                        postageAmt = CalcPostageUtil.GetPostageAmt(Base, 1, row.PaymentAmount);
                        break;
                    case PaymentMethod.B://電匯
                        postageAmt = CalcPostageUtil.GetPostageAmt(Base, 0, row.PaymentAmount);
                        break;
                    case PaymentMethod.D://禮券
                        BAccount2 ba2 = (BAccount2)PXSelectorAttribute.Select<KGBillPayment.vendorID>(Base1.KGBillPaym.Cache, row);
                        //為供應商且有專案，當支票計算
                        if (ba2.Type == BAccountType.VendorType && projectID != null && projectID != 0)
                            postageAmt = CalcPostageUtil.GetPostageAmt(Base, 1, row.PaymentAmount);
                        else
                            postageAmt = 0m;
                        break;
                    default:
                        postageAmt = 0m;
                        break;
                }
            }
            cache.SetValueExt<KGBillPayment.postageAmt>(row, postageAmt);
            cache.SetValueExt<KGBillPayment.actPayAmt>(row, (row.PaymentAmount ?? 0m) - (row.PostageAmt ?? 0m));
        }

        public void SetUIRequired<T>(PXCache cache, object row, bool isRequired) where T : PX.Data.IBqlField
        {
            PXDefaultAttribute.SetPersistingCheck<T>(cache, row, isRequired ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
        }

        public bool checkGVApGuiInvoiceRef(GVApGuiInvoiceRef row)
        {

            bool check = true;
            if (row.InvoiceType == GVList.GVGuiInvoiceType.INVOICE)
            {
                //Regex rgx = new Regex(@"^[A-Z]{2}[0-9]{8}$");
                //if (!rgx.IsMatch(row.GuiInvoiceNbr.ToString()))
                if (row.GuiInvoiceNbr?.Length < 10)
                {
                    check = false;
                    String error = "發票號碼格式有誤，請至該張計價單- 發票憑證 的區塊做檢查";
                    Base1.GVApGuiInvoiceRefs.Cache.RaiseExceptionHandling<GVApGuiInvoiceRef.guiInvoiceNbr>(
                            row, row.GuiInvoiceNbr, new PXSetPropertyException(error, PXErrorLevel.Error));
                }
            }
            return check;
        }

        /// <summary>
        /// Control KGBillPayment UI Enabled or Readonly
        /// </summary>
        /// <param name="billPayment">
        /// Current KGBillPayment
        /// </param>
        public void setBillPayUI()
        {
            APInvoice invoice = Base.Document.Current;
            #region 2021/04/06 Take off and Add new logic for setUIEnabled Mantis:0012002
            /*
             bool canEdit = invoice.Status != APDocStatus.PendingApproval || invoice.Status != APDocStatus.Closed ||
                invoice.Status != APDocStatus.Open;
            if (invoice.Status == APDocStatus.Open && billPaymentExt.UsrIsCheckIssued != true)
            {
                PXUIFieldAttribute.SetEnabled(Base1.KGBillPaym.Cache, null, true);
                PXUIFieldAttribute.SetEnabled<KGBillPayment.paymentPct>(Base1.KGBillPaym.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<KGBillPayment.paymentAmount>(Base1.KGBillPaym.Cache, null, false);
            }
            else
                PXUIFieldAttribute.SetEnabled(Base1.KGBillPaym.Cache, null, canEdit);

            //Base1.KGBillPaym.AllowUpdate = true;
            */
            #endregion
            bool canEdit = true;
            //AP狀態為closed以前都可以編輯
            switch (invoice.Status)
            {

                case APDocStatus.Hold:
                case APDocStatus.Balanced:
                case APDocStatus.Open:
                    canEdit = true;
                    break;

                case APDocStatus.Closed:
                default:
                    canEdit = false;
                    break;
            }
            Base1.KGBillPaym.Cache.AllowDelete = canEdit;
            Base1.KGBillPaym.Cache.AllowInsert = canEdit;
            Base1.KGBillPaym.Cache.AllowUpdate = canEdit;
            if (canEdit)
            {
                PXUIFieldAttribute.SetEnabled(Base1.KGBillPaym.Cache, null, canEdit);
                foreach (KGBillPayment billPayment in Base1.KGBillPaym.Select())
                {
                    KGBillPaymentExt billPaymentExt = Base1.KGBillPaym.Cache.GetExtension<KGBillPaymentExt>(billPayment);
                    //NMPayableCheck check = GetPayableCheck(billPayment.BillPaymentID);
                    //NMApTeleTransLog transLog = GetMApTeleTransLog(billPayment.BillPaymentID);
                    //if (check != null || (transLog != null && transLog.Status == NMApTtLogStatus.FEED_BACK_SUCCESS))
                    if (billPaymentExt.UsrIsCheckIssued == true)
                    {
                        PXUIFieldAttribute.SetEnabled(Base1.KGBillPaym.Cache, billPayment, false);
                    }
                }
            }
            //固定唯讀
            PXUIFieldAttribute.SetEnabled<KGBillPayment.actPayAmt>(Base1.KGBillPaym.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<KGBillPayment.postageAmt>(Base1.KGBillPaym.Cache, null, false);
            PXUIFieldAttribute.SetVisible<KGBillPayment.actPayAmt>(Base1.KGBillPaym.Cache, null, true);
            PXUIFieldAttribute.SetVisible<KGBillPayment.postageAmt>(Base1.KGBillPaym.Cache, null, true);
            //過帳號免郵資不能修改
            if (!invoice.Status.Equals(APDocStatus.Hold) && !invoice.Status.Equals(APDocStatus.Balanced))
            {
                PXUIFieldAttribute.SetEnabled<KGBillPayment.isPostageFree>(Base1.KGBillPaym.Cache, null, false);
            }
        }

        /// <summary>
        /// Control GuiInvoice UI Enabled or Readonly
        /// </summary>
        public void SetGuiInvoiceUI()
        {
            APInvoice invoice = Base.Document.Current;
            if (invoice.Released == true)
            {
                Base1.GVApGuiInvoiceRefs.AllowInsert = false;
                Base1.GVApGuiInvoiceRefs.AllowUpdate = false;
                Base1.GVApGuiInvoiceRefs.AllowDelete = false;
            }
        }

        public void SetDeductionUI(APRegister inv)
        {
            //2021/08/24 Add Mantis: 0012210
            bool isPPM = inv.DocType == APDocType.Prepayment;
            bool isHold = inv.Hold == true;
            Base1.deductionAPTranDetails.AllowDelete = !isPPM && isHold;
            Base1.deductionAPTranDetails.AllowInsert = !isPPM && isHold;
            Base1.deductionAPTranDetails.AllowUpdate = !isPPM && isHold;
            Base1.addDeduction.SetEnabled(!isPPM && isHold);
            Base2.addCheckDiscount.SetVisible(true);
            Base2.addCheckDiscount.SetEnabled(!isPPM && isHold);

            //2021/05/03 把扣款Tab的扣代扣稅/扣健保補充的action隱藏
            Base1.addSupPerm.SetVisible(false);
            Base1.addWHT.SetVisible(false);
        }

        /// <summary>
        /// 若UsrValuationType 是代扣稅類型或加款,全部欄位則ReadOnly
        /// </summary>
        /// <param name="tran"></param>
        public void SetAPTranUI(APTran tran)
        {
            APTranExt tranExt = PXCache<APTran>.GetExtension<APTranExt>(tran);
            bool isReadOnly = false;
            if (tranExt.UsrValuationType == ValuationTypeStringList.WITH_TAX ||//20230103 mantis:12386 w -> withTax
                tranExt.UsrValuationType == ValuationTypeStringList.L ||
                tranExt.UsrValuationType == ValuationTypeStringList.N ||
                tranExt.UsrValuationType == ValuationTypeStringList.H ||
                tranExt.UsrValuationType == ValuationTypeStringList.SECOND ||//20230103 mantis:12386 s -> second
                tranExt.UsrValuationType == ValuationTypeStringList.A)
            {
                isReadOnly = true;
            }
            if (isReadOnly)
            {
                PXUIFieldAttribute.SetReadOnly(Base.Transactions.Cache, tran, isReadOnly);
                //20220224 add by louis for addition can change Tax Category 
                if (tranExt.UsrValuationType == ValuationTypeStringList.A)
                {
                    PXUIFieldAttribute.SetReadOnly<APTran.taxCategoryID>(Base.Transactions.Cache, tran, false);
                }

            }
        }
        /*---mark 2021-08-02:12179
        /// <summary>
        /// Method of Cal Payment Amount
        /// </summary>
        public void setPaymentAmount()
        {
            //KGBillSummary sum = getKGBillSummary();
            KGBillSummary sum = Base1.getKGBillSummary();
            Decimal total = 0;
            decimal? pct = 0;
            foreach (KGBillPayment kgBillPayment in Base1.KGBillPaym.Select())
            {
                decimal paymentAmt = (sum.TotalAmt ?? 0) * (kgBillPayment.PaymentPct ?? 0) / 100;
                paymentAmt = System.Math.Round(paymentAmt, 0, MidpointRounding.AwayFromZero);
                total = total + paymentAmt;
                pct = kgBillPayment.PaymentPct + pct;

                if (!(sum.TotalAmt ?? 0).Equals(total) && pct == 100)
                {
                    if (kgBillPayment != null)
                    {
                        paymentAmt = paymentAmt - (total - (sum.TotalAmt ?? 0m));
                    }
                }
                Base1.KGBillPaym.Cache.SetValueExt<KGBillPayment.paymentAmount>(kgBillPayment, paymentAmt);
            }

        }
        */
        /// <summary>
        /// Control WHTTran UI Enabled or Readonly
        /// </summary>
        /// <param name="tran"></param>
        public void setWHTTranUI()
        {
            TWNWHTTran tran = WHTView.Current;
            APInvoice invoice = Base.Document.Current;
            if (invoice == null) return;
            bool isHold = invoice.Hold ?? false;
            bool isWHT = tran.TranType == FinStringList.TranType.WHT;
            PXUIFieldAttribute.SetReadOnly(WHTView.Cache, tran, !isHold || !isWHT);

            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.tranType>(WHTView.Cache, null, !isHold);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.wHTAmt>(WHTView.Cache, null, !isHold);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.tranDate>(WHTView.Cache, null, !isHold);

            //永遠唯獨
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.netAmt>(WHTView.Cache, null, true);
        }

        /// <summary>
        /// Transfer TranType to ValuationType
        /// </summary>
        /// <param name="TranType"></param>
        /// <returns>ValuationType</returns>
        public string ValuationType(string TranType)
        {
            string VType = "";
            switch (TranType)
            {
                //代扣稅
                case FinStringList.TranType.WHT:
                    VType = ValuationTypeStringList.WITH_TAX;//20230103 mantis:12386 w -> withTax
                    break;
                //代扣勞保
                case FinStringList.TranType.LBI:
                    VType = ValuationTypeStringList.L;
                    break;
                //代扣健保
                case FinStringList.TranType.HLI:
                    VType = ValuationTypeStringList.H;
                    break;
                //代扣退休金
                case FinStringList.TranType.LBP:
                    VType = ValuationTypeStringList.N;
                    break;
                //代扣二代健保
                case FinStringList.TranType.GHI:
                    VType = ValuationTypeStringList.SECOND;//20230103 mantis:12386 s -> second
                    break;
            }
            return VType;
        }

        /// <summary>
        /// Transfer ValuationType to TranType
        /// </summary>
        /// <param name="ValuationType"></param>
        /// <returns>TranType</returns>
        public string TranType(string ValuationType)
        {
            string VType = "";
            switch (ValuationType)
            {
                //代扣稅
                case ValuationTypeStringList.WITH_TAX://20230103 mantis:12386 w -> withTax
                    VType = FinStringList.TranType.WHT;
                    break;
                //代扣勞保
                case ValuationTypeStringList.L:
                    VType = FinStringList.TranType.LBI;
                    break;
                //代扣健保
                case ValuationTypeStringList.H:
                    VType = FinStringList.TranType.HLI;
                    break;
                //代扣退休金
                case ValuationTypeStringList.N:
                    VType = FinStringList.TranType.LBP;
                    break;
                //代扣二代健保
                case ValuationTypeStringList.SECOND://20230103 mantis:12386 second -> Second
                    VType = FinStringList.TranType.GHI;
                    break;
            }
            return VType;
        }


        /// <summary>
        /// Create a tran detail for Without Holding Tax From WHTTran Tab
        /// </summary>
        /// <param name="TranType">WHTTran.TranType</param>
        /// <param name="Amt">the same TranType的Sum(WHTAmt)</param>
        private void CreateAPTranFromWHTTab(string TranType, decimal? Amt)
        {
            APInvoice invoice = Base.Document.Current;
            KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
            APTran tran = Base.Transactions.Cache.CreateInstance() as APTran;
            APTranExt tranExt = PXCache<APTran>.GetExtension<APTranExt>(tran);
            int? InventoryID = null;
            switch (TranType)
            {
                case FinStringList.TranType.WHT:
                    InventoryID = setUp.KGIndividualTaxInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.GHI:
                    InventoryID = setUp.KGSupplementaryTaxInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.HLI:
                    InventoryID = setUp.HealthInsInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.LBI:
                    InventoryID = setUp.LaborInsInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.LBP:
                    InventoryID = setUp.PensionInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
            }
            Base.CurrentDocument.Cache.SetValueExt<APInvoice.paymentsByLinesAllowed>(Base.CurrentDocument.Current, false);

            //塞APTran
            Base.Transactions.Cache.SetValueExt<APTran.inventoryID>(tran, InventoryID);
            Base.Transactions.Cache.SetValueExt<APTran.projectID>(tran, invoice.ProjectID);
            if (invoice.ProjectID != 0)
            {
                PMCostBudget costBudget = GetCostBudget(invoice.ProjectID, InventoryID);
                if (costBudget == null)
                    throw new Exception(string.Format("請至專案({0})設定工料編號的成本任務!", invoice.ProjectID));
                Base.Transactions.Cache.SetValueExt<APTran.costCodeID>(tran, costBudget.CostCodeID);
                Base.Transactions.Cache.SetValueExt<APTran.taskID>(tran, costBudget.TaskID);
            }

            Base.Transactions.Cache.SetValueExt<APTran.qty>(tran, -1m);
            Base.Transactions.Cache.SetValueExt<APTran.curyUnitCost>(tran, Amt);
            Base.Transactions.Cache.SetDefaultExt<APTran.curyLineAmt>(tran);

            PXCache<APTran>.GetExtension<APTranExt>(tran).UsrValuationType = ValuationType(TranType);

            Base.Transactions.Cache.Insert(tran);
        }

        /// <summary>
        /// Update Amt which the tran detail of the same UsrValuationtype
        /// </summary>
        /// <param name="TranType">WHTTran.TranType</param>
        /// <param name="Amt">the same TranType的Sum(WHTAmt)</param>
        private void UpdateAPTranAmt(string TranType, decimal? Amt)
        {
            APInvoice invocie = Base.Document.Current;
            string VType = ValuationType(TranType);
            APTran tran = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTranExt.usrValuationType, Equal<Required<APTranExt.usrValuationType>>>>>
                .Select(Base, invocie.RefNbr, VType);
            Base.Transactions.SetValueExt<APTran.curyLineAmt>(tran, (-1 * Amt));
            Base.Transactions.SetValueExt<APTran.curyUnitCost>(tran, Amt);
            Base.Transactions.SetValueExt<APTran.curyTranAmt>(tran, (-1 * Amt));
            /*tran.CuryLineAmt = (-1 * Amt);
            tran.CuryUnitCost = Amt;
            tran.CuryTranAmt = (-1 * Amt);*/
            Base.Transactions.Cache.Update(tran);

        }

        /// <summary>
        /// Check Amt >=0
        /// </summary>
        /// <param name="tran">
        /// Current TWNWHTTran
        /// </param>
        /// <param name="Field">
        /// 1. WHTAmt
        /// 2. PayAmt
        /// 3. NetAmt
        /// 4. GNHI2Amt
        /// </param>
        /// <param name="Amt"></param>
        /// <returns>
        /// True : 金額<0要出錯!
        /// False :金額>=0
        /// </returns>
        private bool CheckAmt(TWNWHTTran tran, int? Field, decimal? Amt)
        {
            if (Amt >= 0) return false;
            switch (Field)
            {
                case 1:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.wHTAmt>(tran, Amt,
                        new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
                case 2:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.payAmt>(tran, Amt,
                        new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
                case 3:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.netAmt>(tran, Amt,
                        new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
                case 4:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.gNHI2Amt>(tran, Amt,
                       new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
            }
            return true;
        }

        private void SetUsrValuationTypeList()
        {
            PXStringListAttribute.SetList<APTranExt.usrValuationType>(Base.Transactions.Cache, null,
                new string[] {
                    ValuationTypeStringList.A, ValuationTypeStringList.D,
                    ValuationTypeStringList.R, ValuationTypeStringList.P,
                    ValuationTypeStringList.B, ValuationTypeStringList.WITH_TAX,//20230103 mantis:12386 W -> WITH_TAX(T)
                    ValuationTypeStringList.SECOND, ValuationTypeStringList.L, //20230103 mantis:12386 S -> SECOND(2)
                    ValuationTypeStringList.H, ValuationTypeStringList.N,
                    ValuationTypeStringList.C },
                new string[] { "加款", "扣款", "保留款退回", "預付款", "計價", "代扣稅", "健保補充", "勞保費", "健保費", "退休金", "票貼" }
                );
        }

        #endregion

        #region Select Method
        private BAccount2 GetBAccount(int? BAccount)
        {
            return PXSelect<BAccount2,
                Where<BAccount2.bAccountID, Equal<Required<BAccount2.bAccountID>>>>
                .Select(Base, BAccount);
        }
        private Location GetLocation(int? VendorLocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, VendorLocationID);
        }
        private NMBankAccount GetBankAccount(int? CashAccount, string PaymentMethod)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>>>>
                .Select(Base, CashAccount, PaymentMethod);
        }
        private NMBankAccount GetBankAccount(int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
                .Select(Base, BankAccountID);
        }
        private PMCostBudget GetCostBudget(int? ProjectID, int? InventoryID)
        {
            return PXSelect<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>>>>
                .Select(Base, ProjectID, InventoryID);
        }
        private NMPayableCheck GetPayableCheck(int? BillPaymentID)
        {
            return PXSelect<NMPayableCheck,
                Where<NMPayableCheck.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(Base, BillPaymentID);
        }
        private NMApTeleTransLog GetMApTeleTransLog(int? BillPaymentID)
        {
            return PXSelect<NMApTeleTransLog,
                Where<NMApTeleTransLog.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(Base, BillPaymentID);
        }
        private APTran GetAPTranofValuationType(string RefNbr, string VauationType)
        {
            return PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTranExt.usrValuationType, Equal<Required<APTranExt.usrValuationType>>>>>
                .Select(Base, RefNbr, VauationType);
        }
        private APTran GetAPTran(string RefNbr, string DocType, int? LineNbr)
        {
            return PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                    And<APTran.tranType, Equal<Required<APTran.tranType>>,
                    And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>
                    .Select(Base, RefNbr, DocType, LineNbr);

        }
        private TWNWHTTran GetWHTTranofTranType(string RefNbr, string TranType)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<TWNWHTTran.tranType, Equal<Required<TWNWHTTran.tranType>>>>>
                .Select(Base, RefNbr, TranType);
        }
        private PXResultset<TWNWHTTran> getWHT(string TranType)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.tranType, Equal<Required<TWNWHTTran.tranType>>>>
                .Select(Base, TranType);
        }
        private APRegister GetAPRegister(string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, RefNbr);
        }
        private VendorPaymentMethodDetail GetVendorPaymentMethodDetail(int? BAccountID)
        {
            return PXSelect<VendorPaymentMethodDetail,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Required<APInvoice.vendorID>>,
                And<VendorPaymentMethodDetail.detailID, Equal<Required<VendorPaymentMethodDetail.detailID>>>>>
                .Select(Base, BAccountID, "CATEGORYID");
        }
        private Address GetAddress(int? DefAddressID)
        {
            return PXSelect<Address,
                Where<Address.addressID, Equal<Required<BAccount.defAddressID>>>>
                .Select(Base, DefAddressID);
        }
        private Contact GetContact(int? DefContactID)
        {
            return PXSelect<Contact,
                Where<Contact.contactID, Equal<Required<BAccount.defContactID>>>>
                .Select(Base, DefContactID);
        }
        private Vendor GetVendor(int? BAccountID)
        {
            return PXSelect<Vendor,
                Where<Vendor.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, BAccountID);
        }
        private EPExpenseClaim GetEPClaimFromAPRefNbr(string APRefNbr)
        {
            return PXSelectJoin<EPExpenseClaim,
                InnerJoin<EPExpenseClaimDetails, On<EPExpenseClaimDetails.refNbr, Equal<EPExpenseClaim.refNbr>>>,
                Where<EPExpenseClaimDetails.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(Base, APRefNbr);
        }

        private BAccount GetBAccountByCD(string acctCD)
        {
            return PXSelect<BAccount,
                Where<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>
                .Select(new PXGraph(), acctCD);
        }

        private BAccount GetBAccountByCategoryID(string categoryID)
        {
            return PXSelectJoin<BAccount,
                InnerJoin<VendorPaymentMethodDetail, On<VendorPaymentMethodDetail.bAccountID, Equal<BAccount.bAccountID>>>,
                Where<VendorPaymentMethodDetail.detailID, Equal<Required<VendorPaymentMethodDetail.detailID>>,
                    And<VendorPaymentMethodDetail.detailValue, Equal<Required<VendorPaymentMethodDetail.detailValue>>>>>
                .Select(new PXGraph(), "CATEGORYID", categoryID);
        }
        #endregion

        #region Cache Attached

        #region KGBillPayment
        #region VendorID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "VendorID", Required = true)]
        [PXDefault(typeof(Current<APInvoice.vendorID>), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        protected void KGBillPayment_VendorID_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region VendorLocationID
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor LocationID", Visible = true)]
        [PXSelectorWithCustomOrderBy(typeof(Search2<Location.locationID, InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Location.bAccountID>>>,
                                            Where<True, Equal<True>,
                                                 And<Current<KGBillPayment.vendorID>, Equal<Location.bAccountID>,
                                                     And<Where2<Where<BAccount.type, NotEqual<BAccountType.employeeType>,
                                                                      And<Where<Current<KGBillPayment.paymentMethod>,
                                                                                //2021/06/24 Add Auth same with Cash
                                                                                //2021/08/02 Add TempWriteoff same with Cash
                                                                                In3<PaymentMethod.cash, PaymentMethod.auth>,
                                                                                Or2<Where<Current<KGBillPayment.paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                                                                                          And<Location.vPaymentMethodID, Equal<word.tt>>>,
                                                                      Or<Where<Current<KGBillPayment.paymentMethod>,
                                                                               In3<PaymentMethod.check, PaymentMethod.giftCertificate>,
                                                                                   And<Location.vPaymentMethodID, Equal<word.check>>>>>>>>,
                                                    Or<Where<BAccount.type, Equal<BAccountType.employeeType>>>>>>>,
            //2021/05/20 Add Mantis:0012047
            OrderBy<Desc<LocationFinExt.usrIsReserveAcct,
                         Asc<Location.locationCD>>>>),
            //2021/05/11 Add to Show
            typeof(LocationFinExt.usrIsReserveAcct),
            typeof(Location.locationCD),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            //2021/05/11 Add to Show
            typeof(LocationFinExt.usrIsReserveAcct),
            SubstituteKey = typeof(Location.locationCD))]
        [PXDefault()]
        protected void KGBillPayment_VendorLocationID_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region BankAccountID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account CD", Visible = true)]
        [PXSelector(typeof(Search<NMBankAccount.bankAccountID,
            Where2<
                Where<Current<KGBillPayment.paymentMethod>, In3<PaymentMethod.check, PaymentMethod.giftCertificate>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.check>>>,
                Or2<Where<Current<KGBillPayment.paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.tt>>>,
                Or<Where<Current<KGBillPayment.paymentMethod>,
                    //2021/06/24 Add Auth same with Cash
                    //2021/08/02 Add TempWriteoff same with Cash
                    In3<PaymentMethod.cash, PaymentMethod.auth>>>>>>),
            typeof(NMBankAccount.bankAccountCD),
            typeof(NMBankAccount.bankCode),
            typeof(NMBankAccount.bankName),
            DescriptionField = typeof(NMBankAccount.bankName),
            SubstituteKey = typeof(NMBankAccount.bankAccountCD))]

        protected void KGBillPayment_BankAccountID_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region CheckTitle
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault()]
        [PXUIRequired(typeof(Where<Current<KGBillPayment.paymentMethod>, Equal<PaymentMethod.check>>))]

        protected void KGBillPayment_CheckTitle_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region PstageAmt
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIVisible(typeof(True))]

        protected void KGBillPayment_PostageAmt_CacheAttached(PXCache sender) { }
        #endregion

        #region ActPayAmt
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIVisible(typeof(True))]

        protected void KGBillPayment_ActPayAmt_CacheAttached(PXCache sender) { }
        #endregion

        #region IsPostageFree
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "IsPostageFree")]
        protected void KGBillPayment_IsPostageFree_CacheAttached(PXCache sender) { }
        #endregion
        #endregion

        #region GVApGuiInvoiceRef
        #region Remark 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        protected void GVApGuiInvoiceRef_Remark_CacheAttached(PXCache sender) { }
        #endregion

        #region DeductionCode 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault("1")]
        protected void GVApGuiInvoiceRef_DeductionCode_CacheAttached(PXCache sender) { }
        #endregion

        #region Status
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(APInvoiceStatus.Hold)]
        protected void GVApGuiInvoiceRef_Status_CacheAttached(PXCache sender) { }
        #endregion

        #region InvoiceType
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Invoice Type", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        protected void GVApGuiInvoiceRef_InvoiceType_CacheAttached(PXCache sender) { }
        #endregion

        #region VendorName
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Vendor Name", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        protected void GVApGuiInvoiceRef_VendorName_CacheAttached(PXCache sender) { }
        #endregion

        #region VendorUniformNumber
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(InputMask = "00000000")]
        [PXUIField(DisplayName = "VendorUniformNumber")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<GVApGuiInvoiceRef.invoiceType, Equal<GVList.GVGuiInvoiceType.invoice>>))]
        protected void GVApGuiInvoiceRef_VendorUniformNumber_CacheAttached(PXCache sender) { }
        #endregion

        #region Vendor
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIRequired(typeof(False))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        protected void GVApGuiInvoiceRef_Vendor_CacheAttached(PXCache sender) { }
        #endregion

        #region GuiInvoiceNbr
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(14, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCC")]
        protected void GVApGuiInvoiceRef_GuiInvoiceNbr_CacheAttached(PXCache sender) { }
        #endregion

        #region Hold 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(true)]
        protected void GVApGuiInvoiceRef_Hold_CacheAttached(PXCache sender) { }
        #endregion

        #region SalesAmt
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "SalesAmt", Enabled = true)]
        protected void GVApGuiInvoiceRef_SalesAmt_CacheAttached(PXCache sender) { }
        #endregion

        #region TaxAmt
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "TaxAmt")]
        protected void GVApGuiInvoiceRef_TaxAmt_CacheAttached(PXCache sender) { }
        #endregion
        #endregion

        #region TWNWHTTran

        #region RefNbr
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBDefault(typeof(APInvoice.refNbr))]
        [PXParent(typeof(Select<APInvoice,
                            Where<APInvoice.docType, Equal<Current<TWNWHTTran.docType>>,
                                And<APInvoice.refNbr, Equal<Current<TWNWHTTran.refNbr>>>>>))]
        protected void TWNWHTTran_RefNbr_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region DocType
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBDefault(typeof(APInvoice.docType))]
        protected void TWNWHTTran_DocType_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region PersonalID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Personal ID", Required = true)]
        [PXDefault(typeof(Search<VendorExt.usrPersonalID,
            Where<Vendor.bAccountID, Equal<Current<APInvoice.vendorID>>>>))]
        protected void TWNWHTTran_PersonalID_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region BranchID 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(Current<APRegister.branchID>))]
        protected void TWNWHTTran_BranchID_CacheAttached(PXCache sender) { }
        #endregion

        #region PaymentDate
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(Current<APInvoice.docDate>))]
        protected void _(Events.CacheAttached<TWNWHTTran.paymentDate> e) { }
        #endregion 
        #endregion

        #endregion


        #region Error Msg 
        string ErrorAmt = "金額必須大於0!";
        string ErrorTranType = "請先填寫代扣類型!";
        #endregion
    }
}