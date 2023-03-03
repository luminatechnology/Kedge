using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Discount;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.AR;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Objects.RQ;
using Branch = PX.Objects.GL.Branch;
using Kedge.DAC;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using Kedge;
using Kedge.Util;

/**
    * ======2021/02/09 0011940 Edit By Althea=====
    *  AP301000計價彙整 付款方法調整
    *  請一併檢查KGBillPayment.PaymentAmount要等於KGBillSummary.TotalAmt
    * ======2021/03/03 0011971 Edit By Althea=====
    *  Update Enable
    *  原本邏輯是origRefNbr改為UsrIsDeductionDoc != true & DocType= "ADR" 
    *  CreateADRVoucher前也新增邏輯:
    *  判斷:若UsrIsDeductionDoc == true && DocType = ADR，直接跳過createADRVoucher。
    *  Update INV DT&DTT
    *  抓扣款資訊原本用origRefNbr改用
    *  UsrIsDeductionDoc == true & 
    *  UsrIsDeductionRefNbr= Current APInvoice.RefNbr & 
    *  UsrIsDeductionDocType = "INV"
    *  Modify INV/ADR/PPM PA&KP
    *  PaymentMethod有新增D禮券的選項
    *  D(禮券)的做法如同A(支票)
    *  ======2021-02-21: 11955 =====Alton
    *  如果InvoiceType = 'R', 不檢查GVApGuiInvoice.GuiInvoiceNbr是否重複
    * ======2021-03-02 :11970 ======Alton
    * 修改舊有邏輯問題
    * 1.發票重覆應該僅比對InvoiceType = 'I'(改在ProjectCust)
    * 2.統編應抓取主檔location
    * 
    * =====2021-03-15 : ????=====Alton
    * add CacheAttached ProjcetCD 可以選X
    *     defBranchID is null
    *     
    *  ===2021/04/06 Mantis:0011992===Althea
    *  Modify KGBillSummary.OriTotal
    *  KGBillSummary.TotalAmt 計算需要扣掉WhtAmt / Gnhi2Amt / HliAmt / LbiAmt / LbpAmt 金額.
    *  
    * ====2021-04-20:12002 ====Alton
    * 修正畫面控制KGBillpayment_RowSelected → APInvoice_RowSelected
    * 
    * ====2021-04-27 :測試需求====Alton
    * 1.檢核APTran的PONbr是否一致
    * 2.當沒PONbr時 仍要計算 KGBillSummary.PoValuationAmt
    * 3.當沒有PONbr或對應不到廠商評鑑時，廠商評鑑不塞值(避免出錯)
    * 
    * ====2021-04-28 :測試需求====Alton
    * 1.存檔檢核不再卡CreateScreen
    * 2.發票號碼重複檢核改由Row存檔檢和
    * 
    * ===2021/04/28: 口頭===Althea
    * KGBillSummary的保留款退回金額邏輯更改為:
    * 若apinvoice.IsRetainageDocument = true,
    * 則計算Sum(CuryLineAmt)
    * 
    * ===2021/05/25: Tiffany口頭 ===Althea
    * 產生借方調整的稅額改為用SUM(可算稅金額)*稅率(不要一筆算一次稅額)
    * 
    * ===2021/06/16: 0012096 ===Althea
    * 扣款的dialog 去掉會計票貼的資料
    * 
    * ====2021-06-28:12113====Alton
    * 1.扣款Tab中, 使用者點選新增扣款, 彈跳出來的Dialog, 請開啟DeductionsFilter.OrderNbr, 讓使用者可以選到相同專案, 但是相同廠商的其他合約, 預設為目前計價對應的合約
    * 
    * ====2021-07-06:12127====Alton
    * 扣款Tab中, 使用者點選新增扣款OrderNbr Lov追加條件：當前APInvoice的供應商要相同
    * 
    * ===2021/07/06 :0012123 ===Althea
    * 1.分錄的傳票摘要, 有關###廠商名稱###, ###廠商簡稱###, 請改抓KGBillPayment的VendorID對應的廠商名稱及廠商簡稱
    * 2.如果是員工的話, 不分名稱及簡稱, 請都寫入EPEmployee.AcctName
    * 
    * ====2021-07-14:12150====Alton
    * 目前在AP計價單做保留款退回,將擱置勾掉時,會跳出需要做廠商評鑑的視窗
    * 已與Louis確認此種計價情形無需做廠商評鑑
    * 故請協助將 APRegister.DocType='INV' && APRegister.IsRetainageDocument='1'的APBill
    * 跳過廠商評鑑的動作
    * 
    * ====2021-07-15:12147====Alton
    * PricingType='其他-不重計付款日'不更新PaymentDate
    * 
    *  ====2021-07-16:12126====Alton
    * 1.請不要檢查 PaymentPct加總必須要100, 只要檢核PaymentAmount加總要等於KGBillSummary的TotalAmt
    * 
    * ====2021-07-20:12162====Alton
    * 	1.分包契約產生應付帳款計價, 分包契約付款方法中的電匯銀行帳號簡碼設定為02600T, 但產生到計價單, 付款方法電匯的銀行帳號簡碼變成 03500T
    * 	
    * ====2021-07-26:12170 ====Alton
    * 	1.這個事件目的在給沖銷支付金額的預設金額, 預設金額來自分包契約計價參數設定中的預付款扣回比例KGContractPricingRule.DeductionRatioPeriod
    * 	2.給預付款扣回金額預設值請只在APRegister.DocTYpe為INV, 且APRegister.UsrPOOrderNbr跟UsrPOOrderType不為空的狀態下執行
    * 	3.在取的POOrderPrepayment的BQL要判斷POOrderPrepayment對應APRegNbr的Status, 必須不為"作廢"(我在追一下系統沖銷的邏輯), 且取得的 POOrderPrepayment.RefNbr = APAdjust.adjdRefNbr and POOrderPrepayment.RefDocTYpe = APAdjust.adjdRefDocTYpe
    * 
    * ====2021-08-02:12179 ====Alton
    * 1.KGBillpayment.paymentAmount的預設值只有在PaymentPct異動時重新計算該筆金額
    * 2.移除掉原先PaymentAmount的滾動計算
    * 
    * ====2021-08-05:12183 ====Alton
    * 期別抽離至APInvoiceValuationPhaseUtil處理
    * 除了扣款借方調整(UsrIsDeductionDoc == true)是依據原單單號回傳
    * 其他皆依據ProjectID & PONbr處理
    * 
    * ====2021-08-09:12186 ====Alton
    * 繼12179資金說不要滾算.....但工地想要滾算
    * 追加:當KGBillSummary.TotalAmt異動時全部重新滾算(不計差額)
    * 
    * ====2021-08-18:12186 ====Louis
    * createKGFlowSubAcc 針對零星差旅費, KGFlowSubAcc.AccCategory 寫入"5"
    * 
    * ====2021-09-15:12233====Alton
    * 	1.發票憑證使用者輸入InvoiceType為發票, 請連動VoucherCategory為"發票(憑證)", 如果InvoiceType為收據,則連動VoucherCategory為"其他憑證"
    * 	2.使用者輸入未稅金額, 請連動計算5%的稅額, 放到TaxAmt, 使用者還可以修改TaxAmt
    * 	
    * ====2021-12-09:12277====Alton
    * 計價單的DocType不是'INV'的時候, 請Disable發票憑證那個Tab, 不能輸入
    * 
    * ====2021/12/17 :Louis Alton口頭===Althea
    * 刪掉KGBillSummary.TotalAmt 的 updated事件
    * 	
    * 修改BeforeSave邏輯, 只有從PO301000及SC301000才執行 by louis 20210219
    * 修改APInvoice_Hold_FieldUpdating()邏輯, 只針對專案(非X)檢查實付金額必須要跟付款總金額一致 by louis 20210318
    * 修改APInvoice_RowSelected()邏輯, 讓已過帳的計價單也可以產生分錄 by louis 20210318
    * 修改APInvoice_Hold_FieldUpdating()邏輯, 針對COMS來的資料不檢查實付金額必須要跟付款總金額一致 by louis 20211223
    * 
    * ====2022/03/03 Per Spec [Kedge_SASD_計價系統改善需求規格] - 1.1 欄位控制調整==== Jeff
    * Adjust two fields enable control (UsrBillDateFrom, UsrBillDataTo)
    * Add new delegate event (APInvoice_RowDeleting).
   **/

//加扣款KG303000
namespace PX.Objects.AP
{
    #region Inherited / Projection DACs
    //AP5010000 APInvoiceEntryRetainage
    [Serializable]
    [PXProjection(typeof(Select2<APInvoice, InnerJoin<APRegister, On<APRegister.docType, Equal<APInvoice.docType>,
                                                                     And<APRegister.refNbr, Equal<APInvoice.refNbr>>>,
                                            LeftJoin<APTran, On<APRegister.paymentsByLinesAllowed, Equal<True>,
                                                                And<APTran.tranType, Equal<APInvoice.docType>,
                                                                    And<APTran.refNbr, Equal<APInvoice.refNbr>,
                                                                        And<APTran.curyRetainageBal, Greater<decimal0>,
                                                                            And<APTran.curyRetainageAmt, Greater<decimal0>>>>>>>>,
                                            Where<APRegister.curyRetainageUnreleasedAmt, Greater<decimal0>,
                                                  And<APRegister.curyRetainageTotal, Greater<decimal0>,
                                                      And<APRegister.docType, Equal<APDocType.invoice>,
                                                          And<APRegister.retainageApply, Equal<True>,
                                                              And<APRegister.released, Equal<True>,
                                                                  And<APRegister.vendorID, Equal<CurrentValue<APInvoice.vendorID>>,
                                                                      And<APRegister.docDate, LessEqual<CurrentValue<APInvoice.docDate>>,
                                                                          And<APInvoice.refNbr, NotEqual<CurrentValue<APInvoice.refNbr>>>>>>>>>>,
                                            OrderBy<Asc<APRegister.refNbr>>>))]
    public partial class APInvoiceKedgeExt : APInvoice
    {
        #region Key fields

        #region DocType
        public new abstract class docType : IBqlField { }

        [PXDBString(3,
            IsKey = true,
            IsFixed = true,
            BqlField = typeof(APInvoice.docType))]
        [APInvoiceType.List]
        [PXUIField(DisplayName = "Type")]
        public override string DocType
        {
            get
            {
                return _DocType;
            }
            set
            {
                _DocType = value;
            }
        }
        #endregion
        #region RefNbr
        public new abstract class refNbr : IBqlField { }

        [PXDBString(15,
            IsKey = true,
            IsUnicode = true,
            InputMask = ">CCCCCCCCCCCCCCC",
            BqlField = typeof(APInvoice.refNbr))]
        [PXUIField(DisplayName = "Reference Nbr.")]
        [PXSelector(typeof(APInvoiceKedgeExt.refNbr))]
        public override string RefNbr
        {
            get
            {
                return _RefNbr;
            }
            set
            {
                _RefNbr = value;
            }
        }
        #endregion
        #region LineNbr2
        public abstract class lineNbr : IBqlField { }

        [PXInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.",
            Enabled = false,
            FieldClass = nameof(FeaturesSet.PaymentsByLines))]
        [PXFormula(typeof(IsNull<APInvoiceKedgeExt.aPTranLineNbr, int0>))]
        public virtual int? LineNbr2
        {
            get;
            set;
        }
        #endregion

        #endregion

        #region CuryID
        public new abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }

        /// <summary>
        /// Code of the <see cref="PX.Objects.CM.Currency">Currency</see> of the document.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="Company.BaseCuryID">company's base currency</see>.
        /// </value>
        [PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(APRegister.curyID))]
        [PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, FieldClass = nameof(FeaturesSet.Multicurrency))]
        [PXDefault(typeof(Search<Company.baseCuryID>))]
        [PXSelector(typeof(Currency.curyID))]
        public override string CuryID
        {
            get;
            set;
        }

        #endregion
        #region DisplayProjectID
        public abstract class displayProjectID : IBqlField { }

        [PXInt]
        [PXUIField(DisplayName = "Project", Enabled = false)]
        [PXSelector(typeof(PMProject.contractID),
            SubstituteKey = typeof(PMProject.contractCD),
            ValidateValue = false)]
        [PXFormula(typeof(Switch<Case<Where<APInvoiceKedgeExt.paymentsByLinesAllowed, Equal<True>>, APInvoiceKedgeExt.aPTranProjectID>, APInvoiceKedgeExt.projectID>))]
        public virtual int? DisplayProjectID
        {
            get;
            set;
        }
        #endregion
        #region CuryRetainageBal
        public abstract class curyRetainageBal : IBqlField { }

        [PXCurrency(typeof(APInvoiceKedgeExt.curyInfoID), typeof(APInvoiceKedgeExt.retainageBal), BaseCalc = false)]
        [PXFormula(typeof(IsNull<APInvoiceKedgeExt.aPTranCuryRetainageBal, APInvoiceKedgeExt.curyRetainageUnreleasedAmt>))]
        public virtual decimal? CuryRetainageBal
        {
            get;
            set;
        }
        #endregion
        #region RetainageBal
        public abstract class retainageBal : IBqlField { }

        [PXBaseCury]
        [PXFormula(typeof(IsNull<APInvoiceKedgeExt.aPTranRetainageBal, APInvoiceKedgeExt.retainageUnreleasedAmt>))]
        public virtual decimal? RetainageBal
        {
            get;
            set;
        }
        #endregion
        #region CuryOrigDocAmtWithRetainageTotal
        public new abstract class curyOrigDocAmtWithRetainageTotal : IBqlField { }

        [PXCurrency(typeof(APRegister.curyInfoID), typeof(APRegister.origDocAmtWithRetainageTotal), BaseCalc = false)]
        [PXUIField(DisplayName = "Total Amount", FieldClass = nameof(FeaturesSet.Retainage))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(IsNull<Add<APInvoiceKedgeExt.aPTranCuryOrigRetainageAmt, APInvoiceKedgeExt.aPTranCuryOrigTranAmt>,
            Add<APRegister.curyOrigDocAmt, APRegister.curyRetainageTotal>>))]
        public override decimal? CuryOrigDocAmtWithRetainageTotal
        {
            get;
            set;
        }
        #endregion
        #region OrigDocAmtWithRetainageTotal
        public new abstract class origDocAmtWithRetainageTotal : IBqlField { }

        [PXBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Total Amount", FieldClass = nameof(FeaturesSet.Retainage))]
        [PXFormula(typeof(IsNull<Add<APInvoiceKedgeExt.aPTranOrigRetainageAmt, APInvoiceKedgeExt.aPTranOrigTranAmt>,
            Add<APRegister.curyOrigDocAmt, APRegister.curyRetainageTotal>>))]
        public override decimal? OrigDocAmtWithRetainageTotal
        {
            get;
            set;
        }
        #endregion
        #region RetainageReleasePct
        public abstract class retainageReleasePct : PX.Data.BQL.BqlDecimal.Field<retainageReleasePct> { }

        [UnboundRetainagePercent(
            typeof(True),
            typeof(decimal100),
            typeof(APInvoiceKedgeExt.curyRetainageBal),
            typeof(APInvoiceKedgeExt.curyRetainageReleasedAmt),
            typeof(APInvoiceKedgeExt.retainageReleasePct),
            DisplayName = "Percent to Release")]
        public virtual decimal? RetainageReleasePct
        {
            get;
            set;
        }
        #endregion
        #region CuryRetainageReleasedAmt
        public abstract class curyRetainageReleasedAmt : PX.Data.BQL.BqlDecimal.Field<curyRetainageReleasedAmt> { }

        [UnboundRetainageAmount(
            typeof(APInvoiceKedgeExt.curyInfoID),
            typeof(APInvoiceKedgeExt.curyRetainageBal),
            typeof(APInvoiceKedgeExt.curyRetainageReleasedAmt),
            typeof(APInvoiceKedgeExt.retainageReleasedAmt),
            typeof(APInvoiceKedgeExt.retainageReleasePct),
            DisplayName = "Retainage to Release")]
        public virtual decimal? CuryRetainageReleasedAmt
        {
            get;
            set;
        }
        #endregion
        #region RetainageReleasedAmt
        public abstract class retainageReleasedAmt : PX.Data.BQL.BqlDecimal.Field<retainageReleasedAmt> { }
        [PXBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? RetainageReleasedAmt
        {
            get;
            set;
        }
        #endregion
        #region CuryRetainageUnreleasedCalcAmt
        public abstract class curyRetainageUnreleasedCalcAmt : PX.Data.BQL.BqlDecimal.Field<curyRetainageUnreleasedCalcAmt> { }

        [PXCurrency(typeof(APInvoiceKedgeExt.curyInfoID), typeof(APInvoiceKedgeExt.retainageUnreleasedCalcAmt))]
        [PXUIField(DisplayName = "Unreleased Retainage")]
        [PXFormula(typeof(Sub<APInvoiceKedgeExt.curyRetainageBal, APInvoiceKedgeExt.curyRetainageReleasedAmt>))]
        public virtual decimal? CuryRetainageUnreleasedCalcAmt
        {
            get;
            set;
        }
        #endregion
        #region RetainageUnreleasedCalcAmt
        public abstract class retainageUnreleasedCalcAmt : PX.Data.BQL.BqlDecimal.Field<retainageUnreleasedCalcAmt> { }

        [PXBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? RetainageUnreleasedCalcAmt
        {
            get;
            set;
        }
        #endregion
        #region RetainageVendorRef
        public abstract class retainageVendorRef : PX.Data.BQL.BqlString.Field<retainageVendorRef> { }
        [PXString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Retainage Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
        [APVendorRefNbr]
        public virtual string RetainageVendorRef
        {
            get;
            set;
        }
        #endregion

        #region APTran fields

        #region APTranLineNbr
        public abstract class aPTranLineNbr : IBqlField { }

        [PXDBInt(BqlField = typeof(APTran.lineNbr))]
        public virtual int? APTranLineNbr
        {
            get;
            set;
        }
        #endregion
        #region APTranInventoryID
        public abstract class aPTranInventoryID : IBqlField { }

        [PXDBInt(BqlField = typeof(APTran.inventoryID))]
        [PXUIField(DisplayName = "Inventory ID",
            Enabled = false,
            FieldClass = nameof(FeaturesSet.PaymentsByLines))]
        [PXSelector(typeof(InventoryItem.inventoryID),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            ValidateValue = false)]
        public virtual int? APTranInventoryID
        {
            get;
            set;
        }
        #endregion
        #region APTranProjectID
        public abstract class aPTranProjectID : IBqlField { }

        [PXDBInt(BqlField = typeof(APTran.projectID))]
        [PXUIField(DisplayName = "Project",
            Enabled = false,
            FieldClass = nameof(FeaturesSet.PaymentsByLines))]
        [PXSelector(typeof(PMProject.contractID),
            SubstituteKey = typeof(PMProject.contractCD),
            ValidateValue = false)]
        public virtual int? APTranProjectID
        {
            get;
            set;
        }
        #endregion
        #region APTranTaskID
        public abstract class aPTranTaskID : IBqlField { }

        [PXDBInt(BqlField = typeof(APTran.taskID))]
        [PXUIField(DisplayName = "Project Task",
            Enabled = false,
            FieldClass = nameof(FeaturesSet.PaymentsByLines))]
        [PXSelector(typeof(PMTask.taskID),
            SubstituteKey = typeof(PMTask.taskCD),
            ValidateValue = false)]
        public virtual int? APTranTaskID
        {
            get;
            set;
        }
        #endregion
        #region APTranCostCodeID
        public abstract class aPTranCostCodeID : IBqlField { }

        [PXDBInt(BqlField = typeof(APTran.costCodeID))]
        [PXUIField(DisplayName = "Cost Code",
            Enabled = false,
            FieldClass = nameof(FeaturesSet.PaymentsByLines))]
        [PXSelector(typeof(PMCostCode.costCodeID),
            SubstituteKey = typeof(PMCostCode.costCodeCD),
            ValidateValue = false)]
        public virtual int? APTranCostCodeID
        {
            get;
            set;
        }
        #endregion
        #region APTranAccountID
        public abstract class aPTranAccountID : IBqlField { }

        [PXDBInt(BqlField = typeof(APTran.accountID))]
        [PXUIField(DisplayName = "Account",
            Enabled = false,
            FieldClass = nameof(FeaturesSet.PaymentsByLines))]
        [PXSelector(typeof(Account.accountID),
            SubstituteKey = typeof(Account.accountCD),
            ValidateValue = false)]
        public virtual int? APTranAccountID
        {
            get;
            set;
        }
        #endregion
        #region APTranCuryOrigRetainageAmt
        public abstract class aPTranCuryOrigRetainageAmt : IBqlField { }

        [PXDBDecimal(BqlField = typeof(APTran.curyOrigRetainageAmt))]
        public virtual decimal? APTranCuryOrigRetainageAmt
        {
            get;
            set;
        }
        #endregion
        #region APTranOrigRetainageAmt
        public abstract class aPTranOrigRetainageAmt : IBqlField { }

        [PXDBDecimal(BqlField = typeof(APTran.origRetainageAmt))]
        public virtual decimal? APTranOrigRetainageAmt
        {
            get;
            set;
        }
        #endregion
        #region APTranCuryRetainageBal
        public abstract class aPTranCuryRetainageBal : IBqlField { }

        [PXDBDecimal(BqlField = typeof(APTran.curyRetainageBal))]
        public virtual decimal? APTranCuryRetainageBal
        {
            get;
            set;
        }
        #endregion
        #region APTranRetainageBal
        public abstract class aPTranRetainageBal : IBqlField { }

        [PXDBDecimal(BqlField = typeof(APTran.retainageBal))]
        public virtual decimal? APTranRetainageBal
        {
            get;
            set;
        }
        #endregion
        #region APTranCuryOrigTranAmt
        public abstract class aPTranCuryOrigTranAmt : IBqlField { }

        [PXDBDecimal(BqlField = typeof(APTran.curyOrigTranAmt))]
        public virtual decimal? APTranCuryOrigTranAmt
        {
            get;
            set;
        }
        #endregion
        #region APTranOrigTranAmt
        public abstract class aPTranOrigTranAmt : IBqlField { }

        [PXDBDecimal(BqlField = typeof(APTran.origTranAmt))]
        public virtual decimal? APTranOrigTranAmt
        {
            get;
            set;
        }
        #endregion

        #endregion
        #region PONbr
        public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr>
        {
        }
        protected String _PONbr;
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PO Number", Enabled = false, IsReadOnly = true)]
        [PXSelector(typeof(Search<POOrder.orderNbr>), DescriptionField = typeof(POOrder.orderDesc))]
        //[PXDBScalar(typeof(Search4<APTran.pONbr, Where<APTran.refNbr, Equal<refNbr>,And<APTran.tranType, Equal<docType>,And<APTran.pONbr,IsNotNull>>>, Aggregate<GroupBy<APInvoice.refNbr>>>))]
        public virtual String PONbr
        {
            get
            {
                return this._PONbr;
            }
            set
            {
                this._PONbr = value;
            }
        }
        #endregion
    }

    [Serializable]
    [PXProjection(typeof(Select2<APRegister, LeftJoin<APTran, On<APTran.tranType, Equal<APRegister.docType>,
                                                                 And<APTran.refNbr, Equal<APRegister.refNbr>>>>,
                                             Where<APRegisterExt.usrIsRetainageDoc, Equal<True>>,
                                             OrderBy<Asc<APRegister.refNbr>>>))]
    public partial class APRegister2 : PX.Data.IBqlTable
    {
        #region DocType
        [PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APRegister.docType))]
        [APInvoiceType.List]
        [PXUIField(DisplayName = "Type")]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC",  BqlField = typeof(APRegister.refNbr))]
        [PXUIField(DisplayName = "Reference Nbr.")]
        [PXSelector(typeof(APRegister.refNbr))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region DocDate
        [PXDBDate(BqlField = typeof(APRegister.docDate))]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, BqlField = typeof(APRegister.status))]
        [PXDefault(APDocStatus.Hold)]
        [PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [APDocStatus.List]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region Released
        [PXDBBool(BqlField = typeof(APRegister.released))]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Released", Visible = false)]
        public virtual bool? Released { get; set; }
        public abstract class released : PX.Data.BQL.BqlBool.Field<released> { }
        #endregion

        #region CuryDocBal
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(APRegister.curyInfoID), typeof(APRegister.docBal), BaseCalc = false, BqlField = typeof(APRegister.curyDocBal))]
        [PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual decimal? CuryDocBal { get; set; }
        public abstract class curyDocBal : PX.Data.BQL.BqlDecimal.Field<curyDocBal> { }
        #endregion

        #region OrigRetainageDocType
        [PXDBString(3, IsUnicode = true, BqlField = typeof(APTranExt.usrOrigRetainageDocType))]
        [PXUIField(DisplayName = "Orig. Retainage Doc. Type", Enabled = false)]
        public virtual string OrigRetainageDocType { get; set; }
        public abstract class origRetainageDocType : PX.Data.BQL.BqlString.Field<origRetainageDocType> { }
        #endregion

        #region OrigRetainageRefNbr
        [PXDBString(50, IsUnicode = true, BqlField = typeof(APTranExt.usrOrigRetainageRefNbr))]
        [PXUIField(DisplayName = "Orig. Retainage Ref. Nbr.", Enabled = false)]
        public virtual string OrigRetainageRefNbr { get; set; }
        public abstract class origRetainageRefNbr : PX.Data.BQL.BqlString.Field<origRetainageRefNbr> { }
        #endregion

        #region ValuationPhase
        [PXDBInt(BqlField = typeof(APRegisterExt.usrValuationPhase))]
        [PXUIField(DisplayName = "Valuation Phase")]
        public virtual int? ValuationPhase { get; set; }
        public abstract class valuationPhase : PX.Data.BQL.BqlInt.Field<valuationPhase> { }
        #endregion
    }

    [Serializable]
    public class KGValuationDetailDeduction : KGValuationDetail { }

    #region ApGuIInvoice
    [Serializable]
    public class GVApGuiInvoiceRef : GVApGuiInvoice
    {
        #region GuiInvoiceNbr
        public new abstract class guiInvoiceNbr : PX.Data.IBqlField
        {
        }
        protected new string _GuiInvoiceNbr;
        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "GuiInvoiceNbr")]
        public new virtual string GuiInvoiceNbr
        {
            get
            {
                return this._GuiInvoiceNbr;
            }
            set
            {
                this._GuiInvoiceNbr = value;
            }
        }
        #endregion

        #region InvoiceType
        public abstract new class invoiceType : PX.Data.IBqlField
        {
        }
        protected new String _InvoiceType;
        [PXDBString(1)]
        [PXUIField(DisplayName = "InvoiceType", Visible = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [GVList.GVGuiInvoiceType]
        public new virtual String InvoiceType
        {
            get
            {
                return this._InvoiceType;
            }
            set
            {
                this._InvoiceType = value;
            }
        }
        #endregion

        #region VendorName
        [PXDBString(255, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Vendor Name", Visible = false)]
        public new virtual string VendorName { get; set; }
        public new abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
        #endregion

        #region GuiInvoiceID
        [PXDBIdentity(IsKey = false)]
        [PXUIField(Visible = false)]
        public override int? GuiInvoiceID { get; set; }
        #endregion

        #region DocType
        [PXDBString(3, IsKey = true, IsFixed = true)]
        [PXDBDefault(typeof(APRegister.docType), PersistingCheck = PXPersistingCheck.Nothing)]
        [APDocType.List()]
        [PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
        public override string DocType { get; set; }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "Reference Nbr.")]
        [PXDBDefault(typeof(APRegister.refNbr), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXParent(typeof(Select<APRegister, Where<APRegister.refNbr, Equal<Current<GVApGuiInvoiceRef.refNbr>>,
                                                  And<APRegister.docType, Equal<Current<GVApGuiInvoiceRef.docType>>>>>))]
        public override string RefNbr { get; set; }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
        [PXLineNbr(typeof(APRegister.lineCntr), ReuseGaps = true)]
        public override int? LineNbr { get; set; }
        #endregion
    }
    #endregion

    #endregion

    public class APInvoiceEntry_Extension : PXGraphExtension<APInvoiceEntryRetainage, APInvoiceEntry>
    {
        #region Selects
        public PXSetup<OrganizationFinPeriod>.Where<OrganizationFinPeriod.finPeriodID.IsEqual<Current<APInvoice.finPeriodID>>
                                                   .And<EqualToOrganizationOfBranch<OrganizationFinPeriod.organizationID, Current<APInvoice.branchID>>>> finperiod;

        //add by louis for Vendor Evaluation
        //2021/10/18 Orderby抓最後一次的評鑑 by althea
        public PXSelect<KGContractEvaluation,
                        Where<KGContractEvaluation.aPDocType, Equal<Current<APInvoice.docType>>,
                              And<KGContractEvaluation.aPRefNbr, Equal<Current<APInvoice.refNbr>>>>,
                        OrderBy<Desc<KGContractEvaluation.contractEvaluationID, Desc<KGContractEvaluation.evalPhase>>>> ContractEvaluation;
        public IEnumerable contractEvaluation()
        {
            APInvoice master = Base.Document.Current;
            if (master != null)
            {
                KGContractEvaluation CE = PXSelect<KGContractEvaluation,
                            Where<KGContractEvaluation.aPDocType, Equal<Required<KGContractEvaluation.aPDocType>>,
                                  And<KGContractEvaluation.aPRefNbr, Equal<Required<KGContractEvaluation.aPRefNbr>>>>,
                            OrderBy<Desc<KGContractEvaluation.evalPhase>>>.Select(Base, master.DocType, master.RefNbr);
                yield return CE;
            }
        }
        public PXSelect<KGContractEvaluationL,
                        Where<KGContractEvaluationL.contractEvaluationID, Equal<Current<KGContractEvaluation.contractEvaluationID>>>,
                        OrderBy<Asc<KGContractEvaluationL.questSeq>>> ContractEvaluationL;

        [PXImport(typeof(KGBillPayment))]
        public PXSelect<KGBillPayment, Where<KGBillPayment.refNbr, Equal<Current<APRegister.refNbr>>>> KGBillPaym;

        [PXViewName("Custom Retainage")]
        [PXCopyPasteHiddenView()]
        public PXSelectReadonly<APRegister2, Where<APRegister2.origRetainageDocType, Equal<Current<APInvoice.docType>>,
                                                   And<APRegister2.origRetainageRefNbr, Equal<Current<APInvoice.refNbr>>>>> CustomRetainageList;
        #endregion

        #region Override Methods
        //public override void Initialize()
        //{
            //base.Initialize();
            //Base.action.AddMenuAction(webServIntegration);
            //setHoldEnable();
        //}
        //20190826
        public void setHoldEnable()
        {
            APInvoice master = Base.Document.Current;
            //int companyId = PX.Data.Update.PXInstanceHelper.CurrentCompany;
            if (master != null)
            {
                if (master.Hold == true)
                {
                    var invoiceState = Base.GetDocumentState(Base.Document.Cache, master);
                    bool commitDate = checkCommitDate();
                    if (invoiceState.IsDocumentReleasedOrPrebooked || invoiceState.IsDocumentVoided)
                    {
                        //整個單頭都鎖住了
                    }
                    else if (invoiceState.IsDocumentRejectedOrPendingApproval || invoiceState.IsDocumentApprovedBalanced)
                    {
                        //整個單頭都鎖住了
                        //此情況原本是true
                        setEnableOfHold(commitDate);
                    }
                    else if (invoiceState.IsRetainageDebAdj)
                    {
                        //此情況原本是true
                        setEnableOfHold(commitDate);
                    }
                    else
                    {
                        if (master.Scheduled == false)
                        {
                            setEnableOfHold(commitDate);
                        }
                    }
                    PXUIFieldAttribute.SetEnabled<APRegisterExt.usrIsOverdueSubmit>(Base.Document.Cache, master, true);
                }
                //20200525 Alton mantis:11603
                bool isHold = master.Status == APDocStatus.Hold;
                PXUIFieldAttribute.SetEnabled<KGBillSegPricing.billQty>(segmentList.Cache, null, isHold);

                // Mantis [0012320]
                Base.Delete.SetEnabled(isHold);
            }
        }

        public void setEnableOfHold(bool commitDate)
        {
            APInvoice master = Base.Document.Current;
            //20200904 add by alton 11674: AP301000計價單預期送審的檢核請只限制計價單有效
            if (master.DocType != APDocType.Invoice) return;
            if (commitDate)
            {
                PXUIFieldAttribute.SetEnabled<APInvoice.hold>(Base.Document.Cache, master, true);
            }
            else
            {
                APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(Base.Document.Current);
                if (apRegisterExt.UsrIsOverdueSubmit == true)
                {
                    PXUIFieldAttribute.SetEnabled<APInvoice.hold>(Base.Document.Cache, master, true);
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<APInvoice.hold>(Base.Document.Cache, master, false);
                }
            }
        }

        public bool checkCommitDate()
        {
            KGSetUp kgSetup = KGSetup.Current;
            //if (kgSetup == null) {
            kgSetup = PXSelect<KGSetUp>.Select(Base);
            //}
            //KGSetup.Current = kgSetup;
            int days = ((DateTime)Base.Accessinfo.BusinessDate).Day;
            if (days >= kgSetup.KGBillStartDate && days <= kgSetup.KGBillEndDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //由訂單產生計價
        public delegate void InvoicePOOrderDelegate(POOrder order, Boolean createNew, Boolean keepOrderTaxes);
        [PXOverride]
        public void InvoicePOOrder(POOrder order, Boolean createNew, Boolean keepOrderTaxes, InvoicePOOrderDelegate baseMethod)
        {
            baseMethod(order, createNew, keepOrderTaxes);

            APInvoice invoice = Base.Document.Current;

            //20190722追加儲存PONbr
            Base.Document.Cache.SetValueExt<APRegisterExt.usrPONbr>(invoice, order.OrderNbr);
            Base.Document.Cache.SetValueExt<APRegisterExt.usrPOOrderType>(invoice, order.OrderType);
            //20220322追加儲存
            KGContractPricingRule kGContractPricingRule = PXSelect<KGContractPricingRule,
                        Where<KGContractPricingRule.orderNbr, Equal<Required<KGContractPricingRule.orderNbr>>,
                          And<KGContractPricingRule.orderType, Equal<Required<KGContractPricingRule.orderType>>>>,
                        OrderBy<Desc<APRegister.origRefNbr>>>.Select(Base, order.OrderNbr, order.OrderType);

            if (kGContractPricingRule != null && kGContractPricingRule.RetainageApply!= null && kGContractPricingRule.DefRetainagePct != null && invoice.RetainageApply != true)
            {
                Base.Document.Cache.SetValueExt<APRegisterExt.usrIsRetainageApply>(invoice, kGContractPricingRule.RetainageApply);
                Base.Document.Cache.SetValueExt<APRegisterExt.usrRetainagePct>(invoice, kGContractPricingRule.DefRetainagePct);
            }
            
            Base.Document.Cache.SetValueExt<APRegisterExt.usrBillCategory>(invoice, PXCache<POOrder>.GetExtension<POOrderExt>(order).UsrBillCategory);

            if (createNew == true)
            {
                CreateKGBillPayment(order);
            }
        }

        /// <summary>
        /// YJ's request.
        /// </summary>
        public delegate void ProcessPOOrderLinesDelegate(POOrder order, HashSet<APTran> duplicates, bool addBilled, bool keepOrderTaxes = false);
        [PXOverride]
        public virtual void ProcessPOOrderLines(POOrder order, HashSet<APTran> duplicates, bool addBilled, bool keepOrderTaxes, ProcessPOOrderLinesDelegate baseMethod)
        {
            PXSelectBase<POLineS> cmd = new PXSelectReadonly<POLineS,
                                                             Where<POLineS.orderType, Equal<Required<POLineS.orderType>>,
                                                                   And<POLineS.orderNbr, Equal<Required<POLineS.orderNbr>>,
                                                                       And<POLineS.cancelled, Equal<False>,
                                                                           And<POLineS.closed, Equal<False>,
                                                                               And<POLineS.pOAccrualType, Equal<POAccrualType.order>>>>>>,
                                                             OrderBy<Asc<POLineS.sortOrder>>>(Base);
            if (!addBilled)
            {
                cmd.Join<LeftJoin<APTran, On<APTran.pOAccrualRefNoteID, Equal<POLineS.orderNoteID>, And<APTran.pOAccrualLineNbr, Equal<POLineS.lineNbr>,
                    And<APTran.released, Equal<False>>>>>>();
                //cmd.WhereAnd<Where<APTran.refNbr, IsNull>>();
                cmd.WhereAnd<Where<POLineS.billed, Equal<False>>>();
            }

            Base.ProcessPOOrderLines(cmd.Select(order.OrderType, order.OrderNbr).RowCast<POLineS>(),
                                     duplicates,
                                     keepOrderTaxes);
        }

        /// <summary>
        /// Copy the logic from removed Adjust RowUpdated event.
        /// </summary>
        public delegate void AttachPrepaymentDelegate(List<POOrder> orders);
        [PXOverride]
        public virtual void AttachPrepayment(List<POOrder> orders, AttachPrepaymentDelegate baseMethod)
        {
            baseMethod(orders);

            APAdjust adjust = Base.Adjustments.Current;
            KGContractPricingRule cPricRule = GetContractPricingRule(Base.Transactions.Current.POOrderType, Base.Transactions.Current.PONbr);

            if (adjust!= null && cPricRule != null && cPricRule.DeductionRatioPeriod > 0m)
            {
                POOrderPrepayment orderPrepay = PXSelectJoin<POOrderPrepayment, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<POOrderPrepayment.curyInfoID>>>,
                                                                                Where<POOrderPrepayment.aPDocType, Equal<Required<APPayment.docType>>,
                                                                                      And<POOrderPrepayment.aPRefNbr, Equal<Required<APPayment.refNbr>>>>>
                                                                                .Select(Base, adjust.AdjgDocType, adjust.AdjgRefNbr);

                decimal? prepAdjAmt = cPricRule.DeductionRatioPeriod / 100 * (orderPrepay?.CuryAppliedAmt ?? 1m);
                /// If remaining amount is less than deduction ratio amount, then use remaining amount.
                adjust.AdjAmt = adjust.CuryAdjgAmt = adjust.CuryAdjdAmt = new[] { prepAdjAmt, adjust.CuryDocBal }.Min();
                // Per Louis's request, 四捨五入
                adjust.AdjAmt = Math.Round(adjust.AdjAmt.Value, 0);
            }
        }
        #endregion

        #region ApGuIInvoice
        [PXImport(typeof(GVApGuiInvoiceRef))]
        public PXSelect<GVApGuiInvoiceRef, Where<GVApGuiInvoiceRef.refNbr, Equal<Current<APInvoice.refNbr>>>> GVApGuiInvoiceRefs;

        protected virtual void GVApGuiInvoiceRef_Vendor_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            //2021-03-03 edit by Alton 統編改透過主檔location抓取
            //Location location = PXSelectReadonly<Location,
            //    Where<Location.bAccountID, Equal<Required<GVApGuiInvoice.vendor>>>,
            //    OrderBy<Desc<GVGuiBook.endNum>>>.Select(Base, row.Vendor);
            //if (location != null)
            //{
            //    row.VendorUniformNumber = location.TaxRegistrationID;
            //}
            BAccount b = (BAccount)PXSelectorAttribute.Select<GVApGuiInvoiceRef.vendor>(sender, row);
            Location location = GetLocationByID(b?.DefLocationID);
            sender.SetValueExt<GVApGuiInvoiceRef.vendorUniformNumber>(row, location?.TaxRegistrationID);
            sender.SetValueExt<GVApGuiInvoiceRef.vendorName>(row, b?.AcctName);
            sender.SetValueExt<GVApGuiInvoiceRef.vendorAddress>(row, b?.AcctName);
        }

        protected virtual void GVApGuiInvoiceRef_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            APInvoice master = Base.Document.Current;
            GVApGuiInvoiceRefs.Cache.SetValueExt<GVApGuiInvoiceRef.vendor>(row, row.Vendor ?? master.VendorID);
        }

        protected void _(Events.FieldDefaulting<GVApGuiInvoiceRef, GVApGuiInvoiceRef.vendor> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            APInvoice master = Base.Document.Current;
            e.NewValue = master.VendorID;
        }

        protected void _(Events.RowPersisting<GVApGuiInvoiceRef> e)
        {
            GVApGuiInvoiceRef row = e.Row;
            if (row == null || row.InvoiceType == "R") return;
            PXResultset<GVApGuiInvoiceRef> set = PXSelect<GVApGuiInvoiceRef, Where<GVApGuiInvoiceRef.guiInvoiceNbr, Equal<Required<GVApGuiInvoiceRef.guiInvoiceNbr>>
                , And<GVApGuiInvoiceRef.guiInvoiceID, NotEqual<Required<GVApGuiInvoiceRef.guiInvoiceID>>>>>
                .Select(Base, row.GuiInvoiceNbr, row.GuiInvoiceID);
            if (set.Count > 0)
            {
                e.Cache.RaiseExceptionHandling<GVApGuiInvoiceRef.guiInvoiceNbr>(
                        row, row.GuiInvoiceNbr, new PXSetPropertyException(Message.GVApGuiInvoiceDuplicateError, PXErrorLevel.RowError));
            }
        }
        #endregion

        #region 檔案
        public PXSelect<KGBillFile, Where<KGBillFile.refNbr, Equal<Current<APInvoice.refNbr>>>> KGBillFiles;
        #endregion

        #region 保留款
        [PXDBDecimal(0, MinValue = 0, MaxValue = 100)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Default Retainage Percent", FieldClass = "Retainage")]
        protected void APInvoice_DefRetainagePct_CacheAttached(PXCache sender) { }

        [PXDBDecimal(0, MinValue = 0, MaxValue = 100)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Default Retainage Percent", Visibility = PXUIVisibility.Invisible, FieldClass = "Retainage")]
        [RetainagePercentAttribute(
            typeof(APInvoice.retainageApply),
            typeof(APInvoice.defRetainagePct),
            typeof(Sub<Current<APTran.curyLineAmt>, Current<APTran.curyDiscAmt>>),
            typeof(APTran.curyRetainageAmt),
            typeof(APTran.retainagePct))]
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        protected void APTran_RetainagePct_CacheAttached(PXCache sender) { }
        //APTran用這屬性DBRetainagePercentAttribute 但會有Error可能是小數點不對
        /*
        public class DBRetainagePercentAttribute : RetainagePercentAttribute
        {
            public DBRetainagePercentAttribute(
                Type retainageApplyField,
                Type defRetainagePctField,
                Type retainedAmtFormula,
                Type curyRetainageAmtField,
                Type retainagePctField)
                : base(retainageApplyField, defRetainagePctField, retainedAmtFormula, curyRetainageAmtField, retainagePctField)
            {
                _Attributes.Add(new PXDBDecimalAttribute(0) { MinValue = 0, MaxValue = 100 });
                _Attributes.Add(new PXDefaultAttribute(TypeCode.Decimal, "0.0", defaultType) { PersistingCheck = PXPersistingCheck.Nothing });
                _Attributes.Add(new PXUIVerifyAttribute(verifyType, PXErrorLevel.Error, AP.Messages.IncorrectRetainagePercent));
                //_Attributes.Add(new UndefaultFormulaAttribute(formulaType));
            }
        }*/
        #endregion 

        #region Agent Flow Integration
        //CS201010
        public PXSetup<KGSetUp> KGSetup;

        #region BQL by Alton

        /**
         * <summary>
         * 累績追加減金額
         * </summary>
         * **/
        public decimal? getRvnSumAddAmount(string pOOrderNbr)
        {
            decimal? rvnSumAddAmount = 0;
            PMChangeOrderLine item = PXSelectJoinGroupBy<PMChangeOrderLine,
                    InnerJoin<POLine, On<PMChangeOrderLine.pOOrderNbr, Equal<POLine.orderNbr>,
                        And<PMChangeOrderLine.pOOrderType, Equal<POLine.orderType>,
                        And<PMChangeOrderLine.pOLineNbr, Equal<POLine.lineNbr>>>
                    >,
                    InnerJoin<PMChangeOrder, On<PMChangeOrder.refNbr, Equal<PMChangeOrderLine.refNbr>>>>,
                    Where<PMChangeOrderLine.pOOrderNbr, Equal<Required<PMChangeOrderLine.pOOrderNbr>>,
                        And<PMChangeOrderLine.released, Equal<Required<PMChangeOrderLine.released>>,
                        And<PMChangeOrder.classID, NotEqual<Required<PMChangeOrder.classID>>>>
                    >,
                    Aggregate<Sum<PMChangeOrderLine.amount>>
                >.Select(Base, pOOrderNbr, true, "專案結算");
            if (item != null)
            {
                rvnSumAddAmount = item.Amount;
            }
            return rvnSumAddAmount;
        }

        /**
         * <summary>
         * 變更後金額
         * </summary>
         * **/
        public decimal? getRvnTotalExTax(string pOOrderNbr)
        {
            decimal? rvnTotalExTax = 0;
            POLine item = PXSelectGroupBy<POLine,
                    Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>,
                    Aggregate<Sum<POLine.curyLineAmt>>
                >.Select(Base, pOOrderNbr);
            if (item != null)
            {
                rvnTotalExTax = item.CuryLineAmt;
            }
            return rvnTotalExTax;
        }

        /**
         * <summary>
         * 預算金額
         * </summary>
         * **/
        public decimal? getBudAmount(string pOOrderNbr)
        {
            decimal? budAmount = 0;
            PMBudget item = PXSelectJoinGroupBy<PMBudget,
                    InnerJoin<POLine, On<PMBudget.projectID, Equal<POLine.projectID>,
                        And<PMBudget.projectTaskID, Equal<POLine.taskID>,
                        And<PMBudget.costCodeID, Equal<POLine.costCodeID>,
                        And<PMBudget.inventoryID, Equal<POLine.inventoryID>>>>
                    >>,
                    Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                    And<PMBudget.type, Equal<Required<PMBudget.type>>>
                    >,
                    Aggregate<Sum<PMBudget.curyRevisedAmount>>
                >.Select(Base, pOOrderNbr, "E");
            if (item != null)
            {
                budAmount = item.CuryRevisedAmount;
            }
            return budAmount;
        }

        #endregion

        //3種status V-DedApproveNoAccValue ,B-DedApproveAccValue,NoDedApproveValue , status != ('V' , 'B')
        //pricingType-D
        //此方法經Jerry詳細思考後認為可能有問題20191014
        public PXResultset<KGValuationDetail> getKGValuationDetail(String orderNbr, String orderType, String pricingType)
        {
            //isTaxFree 1免稅0要稅
            PXGraph graph = new PXGraph();
            PXResultset<KGValuationDetail> set = PXSelectJoin<KGValuationDetail,
                                InnerJoin<KGValuation, On<KGValuation.valuationID, Equal<KGValuationDetail.valuationID>>,
                                LeftJoin<POOrder, On<POOrder.orderNbr, Equal<KGValuationDetail.orderNbr>
                                    //, And<POOrder.orderType, Equal<POOrder.orderType>>>>>,    //modify by louis 20201207
                                    , And<POOrder.orderType, Equal<KGValuationDetail.orderType>>>>>,
                                Where<KGValuationDetail.orderNbr, Equal<Required<KGValuationDetail.orderNbr>>,
                                      And<KGValuationDetail.orderType, Equal<Required<KGValuationDetail.orderType>>,
                                      And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>>>>
                                      .Select(graph, orderNbr, orderType, pricingType);
            return set;
        }

        public PXResultset<KGValuationDetail> getKGValuationDetail(int? valuationID, String pricingType)
        {

            PXGraph graph = new PXGraph();
            PXResultset<KGValuationDetail> set = PXSelectJoin<KGValuationDetail,
                                InnerJoin<KGValuation, On<KGValuation.valuationID, Equal<KGValuationDetail.valuationID>>,
                                LeftJoin<POOrder, On<POOrder.orderNbr, Equal<KGValuationDetail.orderNbr>
                                    //, And<POOrder.orderType, Equal<POOrder.orderType>>>>>,  //modify by louis 20201207
                                    , And<POOrder.orderType, Equal<KGValuationDetail.orderType>>>>>,
                                Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                                      //And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>>>//mark by alton 20191120 
                                      And<KGValuationDetail.pricingType, Equal<Optional<KGValuationDetail.pricingType>>>>>//add by alton 20191120 加扣款類型非必要條件
                                      .Select(graph, valuationID, pricingType);
            return set;
        }

        public PXResultset<KGDailyRenterVendor> getKGDailyRenterVendor(string orderNbr)
        {
            string[] status = { "S", "W" };
            PXResultset<KGDailyRenterVendor> set = PXSelect<KGDailyRenterVendor,
                               Where<KGDailyRenterVendor.type, Equal<Required<KGDailyRenterVendor.type>>, //type
                               And<KGDailyRenterVendor.status, In<Required<KGDailyRenterVendor.status>>,//status
                               And<KGDailyRenterVendor.orderNbr, Equal<Required<KGDailyRenterVendor.orderNbr>>>>>>//orderNbr
                               .Select(Base, "A", status, orderNbr);
            return set;
        }

        public decimal getPOTaxTranTaxRate(string orderNbr)
        {
            POTaxTran poTaxTran = PXSelect<POTaxTran,
                            Where<POTaxTran.orderNbr,
                                Equal<Required<POTaxTran.orderNbr>>>>
                                .Select(Base, orderNbr);
            if (poTaxTran == null)
            {
                return 0;
            }
            return (poTaxTran.TaxRate ?? 0) / 100;
        }

        public ApproveValue getApproveValue(String orderNbr, String orderType, String pricingType, string refNbr)
        {
            PXResultset<KGValuationDetail> kgValuationDetails = getKGValuationDetail(orderNbr, orderType, pricingType);
            ApproveValue approveValue = new ApproveValue();
            decimal? sumDedApproveValue = 0;
            decimal? noDedApproveValue = 0;
            decimal? dedApproveNoAccValue = 0;
            decimal? dedApproveAccValue = 0;
            decimal? sumDedApproveValueWithTax = 0;
            decimal? noDedApproveValueWithTax = 0;
            decimal? dedApproveNoAccValueWithTax = 0;
            decimal? dedApproveAccValueWithTax = 0;
            decimal? taxRateContainSelf = (1 + getTaxRate(refNbr));

            foreach (KGValuationDetail detail in kgValuationDetails)
            {
                KGValuation kgValuation = PXSelectReadonly<KGValuation,
                           Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>.Select(Base, detail.ValuationID);

                //免稅
                if (kgValuation != null && kgValuation.IsTaxFree == true)
                {
                    sumDedApproveValue = sumDedApproveValue + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);
                    if (!"V".Equals(detail.Status) && !"B".Equals(detail.Status))
                    {
                        noDedApproveValue = noDedApproveValue + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);
                    }
                    if ("V".Equals(detail.Status))
                    {
                        dedApproveNoAccValue = dedApproveNoAccValue + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);
                    }
                    if ("B".Equals(detail.Status))
                    {
                        dedApproveAccValue = dedApproveAccValue + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);

                    }
                }
                //要稅
                else
                {
                    sumDedApproveValueWithTax = sumDedApproveValueWithTax + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);
                    if (!"V".Equals(detail.Status) && !"B".Equals(detail.Status))
                    {
                        noDedApproveValueWithTax = noDedApproveValueWithTax + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);
                    }
                    if ("V".Equals(detail.Status))
                    {
                        dedApproveNoAccValueWithTax = dedApproveNoAccValueWithTax + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);
                    }
                    if ("B".Equals(detail.Status))
                    {
                        dedApproveAccValueWithTax = dedApproveAccValueWithTax + getValue(detail.Amount) + getValue(detail.ManageFeeAmt);
                    }
                }


            }

            PXResultset<KGDailyRenterVendor> KGDailyRenterVendors = getKGDailyRenterVendor(orderNbr);
            decimal? _noDedApproveValue = 0;//點工扣墊款
            decimal? taxRate = getPOTaxTranTaxRate(orderNbr);
            decimal? manageFeeRate = (KGSetup.Current.KGManageFeeRate ?? 0) / 100;
            decimal? manageFeeTaxRate = (KGSetup.Current.KGManageFeeTaxRate ?? 0) / 100;
            foreach (KGDailyRenterVendor item in KGDailyRenterVendors)
            {
                decimal? amount = item.Amount ?? 0;//點工扣墊款
                decimal? amountTax = amount * taxRate;//扣款金額稅金 = 點工扣墊款 * 稅率
                decimal? amountMF = amount * manageFeeRate;//點工扣款管理費 = 點工扣墊款 * 管理費費率
                decimal? amountMFTax = amountMF * manageFeeTaxRate;//點工扣款管理費稅金 = 點工扣款管理費 * 管理費稅率

                _noDedApproveValue += (amount
                                      + amountTax
                                      + amountMF
                                      + amountMFTax
                                      );
            }

            approveValue.SumDedApproveValue = sumDedApproveValue + (sumDedApproveValueWithTax * taxRateContainSelf);
            approveValue.NoDedApproveValue = noDedApproveValue + (noDedApproveValueWithTax * taxRateContainSelf) + _noDedApproveValue;//add by Alton 20191216 mantis:11443
            approveValue.DedApproveNoAccValue = dedApproveNoAccValue + (dedApproveNoAccValueWithTax * taxRateContainSelf);
            approveValue.DedApproveAccValue = dedApproveAccValue + (dedApproveAccValueWithTax * taxRateContainSelf);

            return approveValue;
        }
        
        public class ApproveValue
        {
            public decimal? SumDedApproveValue { get; set; }
            public decimal? NoDedApproveValue { get; set; }
            public decimal? DedApproveNoAccValue { get; set; }
            public decimal? DedApproveAccValue { get; set; }
        }

        protected void APInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            APInvoice master = Base.Document.Current;
            //APInvoice row = e.Row as APInvoice;
            //ADD BY Althea
            if (master == null) return;

            //保留款退回客製追加
            PXUIFieldAttribute.SetVisible<APInvoice.curyRetainageUnpaidTotal>(cache, master, false);

            if (master != null)
            {
                retainageList.AllowInsert = retainageList.AllowDelete = false;
                //int? brachID = Base.Accessinfo.BranchID;
                APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(Base.Document.Current);
                //2差旅費
                if ("2".Equals(apRegisterExt.UsrBillCategory))
                {
                    travelExpenses.SetEnabled(true);
                }
                else
                {
                    travelExpenses.SetEnabled(false);
                }

                if (travelMessage != null)
                {
                    master.Hold = true;
                    master.Status = APDocStatus.Hold;
                    cache.RaiseExceptionHandling<APInvoice.hold>(master, master.Hold, new PXSetPropertyException(travelMessage, PXErrorLevel.Error));
                }
                if (holdCheck == false)
                {
                    master.Hold = true;
                    master.Status = APDocStatus.Hold;
                }
                //20200305
                if (master.Hold == true)
                {
                    apRegisterExt.UsrKGFlowUID = null;
                    apRegisterExt.UsrReturnUrl = null;
                }

                //設定Vible
                PXUIFieldAttribute.SetVisible(cache, "IsJointPayees", false);

                // Spec [Kedge_SASD_計價系統改善需求規格] - 客製退保留款計價單只能加入扣款，計價明細也不能新增新的計價明細，也不能進行加款
                addAddition.SetEnabled(apRegisterExt.UsrIsRetainageDoc != true);

                Base.Transactions.Cache.AllowInsert = apRegisterExt.UsrIsRetainageDoc != true;
            }
            
            //符合以下條件才能按產生分錄
            //master.Status =="B"
            //2021/03/03 Althea Modify 原本是origRefNbr改為UsrIsDeductionDoc != true & DocType= "ADR" 
            //20210318 louis  新增已過帳還可以產生分錄  
            APRegisterExt APRExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);

            AddKGVoucher.SetEnabled((master.Status == "N" || master.Status == "B" || master.Status == "C") &&
                (master.DocType == "INV" || master.DocType == "PPM" ||
                (master.DocType == "ADR" && APRExt.UsrIsDeductionDoc != true)) &&
                APRExt.UsrVoucherKey == null && APRExt.UsrVoucherNo == null
                );
            //if (tempEpApprovalExt != null && Base.Approval.Current != null)
            //{
            //    EPApprovalExt newEpApprovalExt = PXCache<EPApproval>.GetExtension<EPApprovalExt>(Base.Approval.Current);
            //    newEpApprovalExt.UsrReturnUrl = tempEpApprovalExt.UsrReturnUrl;
            //    Base.Approval.Cache.IsDirty = false;
            //}
            setHoldEnable();

            // YJ's reuqest
            PXUIFieldAttribute.SetVisible<APInvoice.projectID>          (cache, master, true);

            PXUIFieldAttribute.SetEnabled<APInvoice.projectID>          (cache, master, true);
            PXUIFieldAttribute.SetEnabled<APRegisterExt.usrFlowStatus>  (cache, master, false);
            PXUIFieldAttribute.SetEnabled<APRegisterExt.usrBillCategory>(cache, master, false);
            PXUIFieldAttribute.SetEnabled<APRegisterExt.usrBillDateFrom>(cache, master, master.Status.IsIn(ARDocStatus.Hold, ARDocStatus.Balanced));
            PXUIFieldAttribute.SetEnabled<APRegisterExt.usrBillDateTo>  (cache, master, master.Status.IsIn(ARDocStatus.Hold, ARDocStatus.Balanced));
            //PXDBStringAttribute.SetInputMask<APInvoice.docDesc>(Base.Document.Cache, ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");

            // These data views are readonly.
            ContractEvaluation.Cache.AllowUpdate = ContractEvaluationL.Cache.AllowInsert = ContractEvaluationL.Cache.AllowUpdate = ContractEvaluationL.Cache.AllowDelete = false;

            //2021/01/15 搬到KGBillPayment_Rowselected
            //2021-04-20 搬回來....
            KGBillPaym.Cache.AllowInsert = KGBillPaym.Cache.AllowDelete = (master.Hold ?? false);

            //2021-12-09:12277: DocType不是'INV'的時候, 請Disable發票憑證那個Tab, 不能輸入
            GVApGuiInvoiceRefs.Cache.AllowInsert = GVApGuiInvoiceRefs.Cache.AllowUpdate = master.DocType == APDocType.Invoice;

            // According to Louis's request on 2022/04/14 Jeff
            bool stdRetainageAppy = master.RetainageApply == true;
            bool retainageApplied = APRExt.UsrIsRetainageApply == true;
            bool isRetainageReDoc = APRExt.UsrIsRetainageDoc == true;

            PXUIFieldAttribute.SetVisible<APRegisterExt.usrTotalRetainageAmt>(cache, master, retainageApplied && !stdRetainageAppy);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrRetainagePct>(cache, master, retainageApplied && !stdRetainageAppy);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrRetainageAmt>(cache, master, retainageApplied && !stdRetainageAppy);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrRetainageReleased>(cache, master, retainageApplied && !stdRetainageAppy);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrRetainageUnreleasedAmt>(cache, master, retainageApplied && !stdRetainageAppy);

            // According to Mantis [0012321] - (0025647) # 2
            PXUIFieldAttribute.SetVisible<APTranExt.usrRetainagePct>(Base.Transactions.Cache, null, isRetainageReDoc);
            PXUIFieldAttribute.SetVisible<APTranExt.usrOrigRetUnreleasedAmt>(Base.Transactions.Cache, null, isRetainageReDoc);
            PXUIFieldAttribute.SetVisible<APTran.qty>(Base.Transactions.Cache, null, !isRetainageReDoc);
            PXUIFieldAttribute.SetVisible<APTran.curyUnitCost>(Base.Transactions.Cache, null, !isRetainageReDoc);

            CustomRetainageList.Cache.AllowSelect = retainageApplied && !stdRetainageAppy;

            PXUIFieldAttribute.SetEnabled<APInvoice.retainageApply>(cache, master, !isRetainageReDoc && retainageApplied == false && master.Hold == true);
            PXUIFieldAttribute.SetEnabled<APRegisterExt.usrIsTmpPayment>(cache, master, !isRetainageReDoc);

            bool isReversed = master.DocType == APDocType.DebitAdj && master.OrigDocType == APDocType.Invoice;

            Base.Transactions.Cache.AllowDelete = Base.Transactions.Cache.AllowInsert = Base.Transactions.Cache.AllowUpdate = !isReversed;
            PXUIFieldAttribute.SetEnabled<APRegisterExt.usrRetainageAmt>(cache, master, !isReversed && master.Status == APDocStatus.Hold);

            if (APRExt.UsrRetainageHistType == RetHistType.History)
            {
                Base.Transactions.Cache.AllowDelete = Base.Transactions.Cache.AllowInsert = Base.Transactions.Cache.AllowUpdate = false;
                PXUIFieldAttribute.SetEnabled<APRegisterExt.usrRetainageAmt>(cache, master, false);
            }
        }

        protected void _(Events.RowDeleting<APInvoice> e, PXRowDeleting baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
			
			if (e.Row.DocType != APDocType.Invoice) { return; }

            const string errMsg = "下一期計價單已建立，而無法被刪除。";

            var row    = e.Row as APInvoice;
            var rowExt = row.GetExtension<APRegisterExt>();

            bool hasNextPhase = SelectFrom<APRegister>.Where<APRegister.docType.IsEqual<APDocType.invoice>
                                                            .And<APRegisterExt.usrPOOrderType.IsEqual<@P.AsString>
                                                                 .And<APRegisterExt.usrPONbr.IsEqual<@P.AsString>
                                                                      .And<APRegisterExt.usrValuationPhase.IsGreater<@P.AsInt>>>>>.View
                                                      .SelectSingleBound(Base, null, rowExt.UsrPOOrderType, rowExt.UsrPONbr, rowExt.UsrValuationPhase).Count > 0;

            if (row.Status == ARDocStatus.Hold && hasNextPhase == true)
            {
                throw new PXException(errMsg);
            }
        }

        bool? holdCheck = null;
        protected virtual void APInvoice_Hold_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.Row == null) { return; }

            APInvoice master = (APInvoice)e.Row;
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(Base.Document.Current);

            bool? hold = (bool?)e.NewValue;

            if (hold == false || hold == null)
            {
                if (master != null && master.ProjectID != null)
                {
                    if (!APDocType.DebitAdj.Equals(master.DocType) && apRegisterExt.UsrIsComsData != true)
                    {
                        Contract contract = GetContract(master.ProjectID);
                        if (contract != null && contract.NonProject != true)
                        {
                            Decimal? sumPaymentPct = 0;
                            //2021/02/09 Add Mantis:0011940
                            Decimal? sumPaymenyAmount = 0;
                            KGBillPayment payments = null;
                            KGBillSummary sum = getKGBillSummary();
                            foreach (KGBillPayment kgBillPayment in KGBillPaym.Select())
                            {

                                sumPaymentPct = sumPaymentPct + getValue(kgBillPayment.PaymentPct);
                                sumPaymenyAmount = sumPaymenyAmount + (kgBillPayment.PaymentAmount ?? 0);
                                payments = kgBillPayment;
                            }
                            if (payments == null)
                            {
                                sender.RaiseExceptionHandling<APInvoice.hold>(master, master.Hold,
                                      new PXSetPropertyException("必須要有付款方式", PXErrorLevel.Error));
                                holdCheck = false;
                                e.Cancel = true;
                                return;
                            }
                            //2021-07-16 mark by alton : 12126
                            //else if (sumPaymentPct != 100)
                            //{

                            //    KGBillPaym.Cache.RaiseExceptionHandling<KGBillPayment.paymentPct>(payments, payments.PaymentPct,
                            //          new PXSetPropertyException("付款方式加總要為100", PXErrorLevel.Error));

                            //    //master.Status = APDocStatus.Hold;
                            //    holdCheck = false;
                            //    e.Cancel = true;
                            //    return;

                            //}
                            else if (sumPaymenyAmount != sum.TotalAmt)
                            {
                                //只針對專案(非X)檢查實付金額必須要跟付款粽金額一致 20200318 louis
                                if (master.ProjectID > 0)
                                {
                                    KGBillPaym.Cache.RaiseExceptionHandling<KGBillPayment.paymentAmount>(payments, payments.PaymentAmount,
                                              new PXSetPropertyException("付款金額加總要與實付金額-小計相同!", PXErrorLevel.Error));
                                    holdCheck = false;
                                    e.Cancel = true;
                                    return;
                                }
                            }
                        }
                    }
                }

                //add 保留款退回不做廠商評鑑 by louis 20210716
                if (master.DocType == APDocType.Invoice && master.IsRetainageDocument != true && Base.Document.Cache.GetStatus(master) != PXEntryStatus.Deleted)
                {
                    if (SummaryAmtFilters.Current != null && master.Status != APDocStatus.Balanced
                        && master.Status != APDocStatus.Open && master.Status != APDocStatus.Closed)
                    {
                        KGContractDoc contractDoc = PXSelect<KGContractDoc, Where<KGContractDoc.orderNbr, Equal<Required<APRegisterExt.usrPONbr>>>>.Select(Base, apRegisterExt.UsrPONbr);
                        
                        if (contractDoc != null)
                        {
                            if ((SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.EvaluationScore ||
                                (SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.EvaluationScore &&
                                SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.SecEvaluationScore)) &&
                                ContractEvalExisted(apRegisterExt.UsrPOOrderType, apRegisterExt.UsrPONbr) == 0 &&
                                apRegisterExt.UsrPOOrderType == POOrderType.RegularOrder)

                            {
                                sender.RaiseExceptionHandling<APInvoice.hold>(master, master.Hold, new PXSetPropertyException("請填寫第一次評鑑", PXErrorLevel.Error));
                                holdCheck = false;
                                e.Cancel = true;
                                return;
                            }
                            else if (SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.SecEvaluationScore &&
                                     ContractEvalExisted(apRegisterExt.UsrPOOrderType, apRegisterExt.UsrPONbr) == 1 &&
                                     APContractEvalExisted(apRegisterExt.UsrPOOrderType, apRegisterExt.UsrPONbr, master.RefNbr) <= 0 &&
                                     apRegisterExt.UsrPOOrderType == POOrderType.RegularOrder)
                            {
                                sender.RaiseExceptionHandling<APInvoice.hold>(master, master.Hold, new PXSetPropertyException("請填寫第二次評鑑", PXErrorLevel.Error));
                                holdCheck = false;
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void APInvoice_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!e.ExternalCall)
            {
                return;
            }
        }

        string travelMessage = null;
        public virtual void APInvoice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            APInvoice master = (APInvoice)e.Row; ;
            APInvoice oldRow = (APInvoice)e.OldRow;
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(Base.Document.Current);
            //其他程式的保存檢查
            if (master.CuryOrigDocAmt != master.CuryDocBal)
            {
                return;
            }
            if (!e.ExternalCall)
            {
                return;
            }
            if (holdCheck == false)
            {
                return;
            }

            //其他程式的保存檢查End
            String screenID = PXSiteMap.CurrentScreenID;
            travelMessage = null;
            String status = null;
            if ("AP301000".Equals(PXSiteMap.CurrentScreenID) && (master.Hold == false || master.Hold == null) && oldRow.Hold == true
                && APDocStatus.PendingApproval.Equals(master.Status) && APDocStatus.Hold.Equals(oldRow.Status))
            {
                if (APDocStatus.PendingApproval.Equals(master.Status))
                {
                    //2差旅費

                    if ("2".Equals(apRegisterExt.UsrBillCategory))
                    {
                        Decimal? sum = 0;
                        Decimal? amount = 0;
                        foreach (APTran apTran in Base.Transactions.Select())
                        {
                            sum = sum + getValue(apTran.CuryTranAmt);
                        }

                        //sum = sum * (1 + getTaxRate());
                        sum = sum + getTaxAmt();
                        //AgentFlowWebServiceUtil.getEngTravleAmount( master.RefNbr, master.ProjectID, ref status, ref travelMessage);
                        AgentFlowWebServiceUtil.getEngTravleAmount(master.RefNbr, master.ProjectID, ref status, ref travelMessage);
                        if (travelMessage != null)
                        {
                            return;
                        }
                        if (status == null || "-1".Equals(status))
                        {
                            travelMessage = "找不到差旅費資料";
                            return;
                        }
                        else
                        {
                            amount = Convert.ToDecimal(status);
                        }
                        if (sum != amount)
                        {
                            travelMessage = "差旅報告單的金額 (" + amount + ") 與計價的金額含稅加總 (" + sum + ")不一致";
                        }

                    }
                    //throw new PXException("GG");
                    //doAgentFlow();
                    if (apRegisterExt.UsrIsComsData!=true) {
                        PXGraph.CreateInstance<APInvoiceEntryAgentFlow>().DoAgentFlow(Base, master);
                    }
                    
                    //Base.Actions.PressSave();
                    //Base.Document.Cache.RaiseExceptionHandling<RQRequest.status>(master, master.Status, new PXSetPropertyException("AgentFlow寫入createROProcess異常"));
                    //webServIntegration.Press();
                }
            };


        }

        protected void APInvoice_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            //BAccount bAccount = GetBAccount(VendorID);
            APInvoice master = (APInvoice)e.Row;
            BAccount bAccount = GetBAccount(master.VendorID);
            if (bAccount != null)
            {
                foreach (KGBillPayment kgBillPayment in KGBillPaym.Select())
                {
                    kgBillPayment.CheckTitle = bAccount.AcctName;
                    KGBillPaym.Update(kgBillPayment);
                }
            }
            
        }

        protected virtual void _(Events.RowInserted<APInvoice> e)
        {
            //Base.Document.Cache.SetValue<APRegisterExt.usrValuationPhase>(e.Row, APInvoiceValuationPhaseUtil.GetUsrValuationPhase(Base));
            if (e.Row.DocType == APDocType.DebitAdj && e.Row.GetExtension<APRegisterExt>().UsrIsDeductionDoc != true && !string.IsNullOrEmpty(e.Row.OrigRefNbr) && e.Row.OrigDocType == APDocType.Invoice)
            {
                Base.Document.Cache.SetValue<APRegisterExt.usrValuationPhase>(e.Row, APInvoiceValuationPhaseUtil.GetUsrValuationPhase(Base));
                Base.GetExtension<APInvoiceEntryBillSummaryExt>().SetCumAndTotalSummary(e.Row);
                //new APInvoiceEntryBillSummaryExt().SetCumAndTotalSummary(e.Row);
            }
        }
        #endregion

        #region 分段計價
        [PXImport(typeof(KGBillSegPricing))]
        public PXSelect<KGBillSegPricing,
                Where<KGBillSegPricing.aPRefNbr, Equal<Optional<APInvoice.refNbr>>,
                And<KGBillSegPricing.aPTranID, Equal<Optional<APTran.tranID>>>>> segmentList;
        public PXAction<APInvoice> segmentPricing;
        [PXUIField(DisplayName = "Segment Pricing", MapEnableRights = PXCacheRights.Select,
                MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable SegmentPricing(PXAdapter adapter)
        {
            APInvoice master = Base.Document.Current;
            try
            {
                APTran aptran = Base.Transactions.Current;
                PXResultset<KGContractSegPricing> set = PXSelect<KGContractSegPricing, Where<KGContractSegPricing.orderNbr, Equal<Current<APTran.pONbr>>,
                                              And<KGContractSegPricing.lineNbr, Equal<Current<APTran.pOLineNbr>>,
                                              And<KGContractSegPricing.orderType, Equal<Current<APTran.pOOrderType>>>>>>.Select(Base);

                if (set.Count == 0)
                {
                    throw new PXException(Message.NoSegPricing);
                }
                if (segmentList.Select().Count == 0)
                {
                    POLine poLine = null;
                    if (set.Count > 0)
                    {
                        KGContractSegPricing segPricing = set;
                        PXGraph graph = new PXGraph();

                        poLine = PXSelectReadonly<POLine,
                                        Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                        And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>,
                                        And<POLine.orderType, Equal<Required<POLine.orderType>>>>>>.Select(graph, segPricing.OrderNbr, segPricing.LineNbr, segPricing.OrderType);
                    }
                    foreach (KGContractSegPricing kgContractSegPricing in set)
                    {
                        KGBillSegPricing segPricing = (KGBillSegPricing)segmentList.Cache.CreateInstance();
                        segPricing.SegPricingID = kgContractSegPricing.SegPricingID;
                        segPricing.PODocType = kgContractSegPricing.OrderType;
                        segPricing.POOrderNbr = kgContractSegPricing.OrderNbr;
                        segPricing.POLineNbr = kgContractSegPricing.LineNbr;
                        segPricing.SortOrder = kgContractSegPricing.SortOrder;
                        segPricing.SegmentName = kgContractSegPricing.SegmentName;
                        segPricing.SegmentPercent = kgContractSegPricing.SegmentPercent;
                        segPricing.Poqty = poLine.OrderQty;
                        segPricing.UOM = poLine.UOM;
                        segPricing.POUnitCost = poLine.CuryUnitCost * segPricing.SegmentPercent / 100;
                        segPricing.BillCumulativeQty = getBillCumulativeQty(segPricing.SegPricingID, segPricing.BillSegPricingID);
                        //segPricing.RetainagePct = 
                        segmentList.Insert(segPricing);
                    }
                }
                /*
                int a = segmentList.Select().Count;
               PXResultset<APSegmentPricing>   set=    PXSelect<APSegmentPricing, Where<APSegmentPricing.orderNbr, Equal<Required<APTran.pONbr>>,
                         And<APSegmentPricing.lineNbr, Equal<Required<APTran.pOLineNbr>>,
                    And<APSegmentPricing.orderType, Equal<Required<APTran.pOOrderType>>>>>>.Select(Base, aptran.PONbr, aptran.POLineNbr,aptran.POOrderType);
                int b = set.Count;*/
                WebDialogResult result = this.segmentList.AskExt(true);
                if (result == WebDialogResult.OK)
                {
                }
                else if (result == WebDialogResult.Cancel)
                {
                    //this.segmentList.Cache.Clear();
                    //this.segmentList.Cache.ClearQueryCache();
                }
                else
                {
                    //this.segmentList.Cache.Clear();
                    //this.segmentList.Cache.ClearQueryCache();
                }
            }
            catch (Exception e)
            {
                //this.segmentList.Cache.Clear();
                //this.segmentList.Cache.ClearQueryCache();
                throw e;
            }
            return adapter.Get();
        }

        public PXAction<POOrder> cancelSegmentPercent;
        [PXUIField(DisplayName = "Cancel", Enabled = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable CancelSegmentPercent(PXAdapter adapter)
        {
            APInvoice master = Base.Document.Current;
            if (PXEntryStatus.Inserted == Base.Document.Cache.GetStatus(master))
            {
                setQtyZero();
            }
            else
            {
                APTran aptran = Base.Transactions.Current;
                if (aptran.TranID < 0)
                {
                    setQtyZero();
                }
                else
                {
                    PXGraph graph = new PXGraph();
                    APTran dbAPtran = PXSelect<APTran,
                            Where<APTran.tranID, Equal<Required<APTran.tranID>>>>
                            .Select(graph, aptran.TranID);
                    aptran.Qty = dbAPtran.Qty;
                    Base.Transactions.Cache.SetValueExt<APTran.qty>(aptran, aptran.Qty);
                    foreach (KGBillSegPricing apSegmentPricing in segmentList.Select())
                    {
                        PXGraph graph2 = new PXGraph();
                        KGBillSegPricing kgBillSegPricing = PXSelect<KGBillSegPricing,
                            Where<KGBillSegPricing.billSegPricingID, Equal<Required<KGBillSegPricing.billSegPricingID>>>>
                            .Select(graph2, apSegmentPricing.BillSegPricingID);
                        if (kgBillSegPricing == null)
                        {
                            segmentList.Delete(apSegmentPricing);
                        }
                        else
                        {
                            apSegmentPricing.BillQty = kgBillSegPricing.BillQty;
                            segmentList.Update(apSegmentPricing);
                        }
                    }
                }
            }
            return adapter.Get();
        }
        public void setQtyZero()
        {
            APTran aptran = Base.Transactions.Current;
            foreach (KGBillSegPricing apSegmentPricing in segmentList.Select())
            {
                apSegmentPricing.BillQty = 0;
                segmentList.Update(apSegmentPricing);
            }
            aptran.Qty = 0;
            Base.Transactions.Cache.SetValueExt<APTran.qty>(aptran, aptran.Qty);
        }

        public PXAction<APInvoice> checkSegmentPercent;
        [PXUIField(DisplayName = "OK", Enabled = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable CheckSegmentPercent(PXAdapter adapter)
        {
            APTran aptran = Base.Transactions.Current;
            Decimal sum = 0;
            foreach (KGBillSegPricing apSegmentPricing in segmentList.Select())
            {
                if (apSegmentPricing.BillQty != null && apSegmentPricing.SegmentPercent != null)
                {
                    sum = sum + (apSegmentPricing.BillQty.Value * (apSegmentPricing.SegmentPercent.Value / 100));
                }
            }

            aptran.Qty = sum;
            Base.Transactions.Cache.SetValueExt<APTran.qty>(aptran, aptran.Qty);
            Base.Transactions.Update(aptran);
            return adapter.Get();
        }

        protected virtual void KGBillSegPricing_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            APTran apTran = Base.Transactions.Current;
            if (apTran == null)
            {
                return;
            }

            KGBillSegPricing kgBillSegPricing = (KGBillSegPricing)e.Row;
            if (kgBillSegPricing != null)
            {
                bool ckeckPricings = ckeckPricing(apTran);
                checkSegmentPercent.SetEnabled(ckeckPricings);
            }

        }
        protected virtual void KGBillSegPricing_BillQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGBillSegPricing kgBillSegPricing = (KGBillSegPricing)e.Row;
            APTran apTran = Base.Transactions.Current;
            if (kgBillSegPricing.BillQty < 0)
            {
                segmentList.Cache.RaiseExceptionHandling<KGBillSegPricing.billQty>(
                     kgBillSegPricing, kgBillSegPricing.BillQty, new PXSetPropertyException(Message.PoQtyNagativeError));
                Base.Transactions.Cache.RaiseExceptionHandling<APTran.qty>(
                 apTran, apTran.Qty, new PXSetPropertyException(Message.PoQtyNagativeError));

            }
            if (kgBillSegPricing != null)
            {
                if ((((getValue(kgBillSegPricing.BillQty) + getValue(kgBillSegPricing.BillCumulativeQty))) * (kgBillSegPricing.SegmentPercent / 100))
                    > getValue(kgBillSegPricing.Poqty))
                {

                    segmentList.Cache.RaiseExceptionHandling<KGBillSegPricing.billQty>(
                            kgBillSegPricing, kgBillSegPricing.BillQty, new PXSetPropertyException(Message.PoQtySmallError));
                    Base.Transactions.Cache.RaiseExceptionHandling<APTran.qty>(
                        apTran, apTran.Qty, new PXSetPropertyException(Message.PoQtySmallError));
                }
            }
        }

        #endregion

        #region 加款
        [PXCopyPasteHiddenView]
        public PXFilter<AdditionsFilter> additionFilters;

        [PXFilterable]
        [PXCopyPasteHiddenView]
        public PXSelectJoin<KGValuationDetail,
                            InnerJoin<KGValuation,
                                      On<KGValuation.valuationID, Equal<KGValuationDetail.valuationID>>,
                                      LeftJoin<POOrder,
                                        On<POOrder.orderNbr, Equal<KGValuationDetail.orderNbr>>,
                                        LeftJoin<BAccount,
                                                 On<BAccount.bAccountID, Equal<POOrder.vendorID>>>>>,
                            Where<KGValuationDetail.pricingType, Equal<PricingTypeAddition>,
                                And<KGValuationDetail.aPInvoiceNbr, IsNull,
                                    And<KGValuationDetail.aPInvoiceDate, IsNull,
                                    And<KGValuationDetail.status, Equal<StatusV>>>>>> additionDetails;

        protected virtual IEnumerable AdditionDetails()
        {
            PXSelectBase<KGValuationDetail> query = new PXSelectJoin<KGValuationDetail,
                            InnerJoin<KGValuation,
                                      On<KGValuation.valuationID, Equal<KGValuationDetail.valuationID>>,
                                      LeftJoin<POOrder,
                                        On<POOrder.orderNbr, Equal<KGValuationDetail.orderNbr>>,
                                        LeftJoin<BAccount,
                                                 On<BAccount.bAccountID, Equal<POOrder.vendorID>>>>>,
                            Where<KGValuationDetail.pricingType, Equal<PricingTypeAddition>,
                                And<KGValuationDetail.aPInvoiceNbr, IsNull,
                                    And<KGValuationDetail.aPInvoiceDate, IsNull,
                                    And<KGValuationDetail.status, Equal<StatusV>>>>>>(Base);

            AdditionsFilter filter = additionFilters.Current;
            if (filter.ContractID != null)
            {
                query.WhereAnd<Where<KGValuationDetail.contractID, Equal<Current<AdditionsFilter.contractID>>>>();
            }
            if (filter.OrderNbr != null)
            {
                query.WhereAnd<Where<KGValuationDetail.orderNbr, Equal<Current<AdditionsFilter.orderNbr>>>>();
            }
            if (filter.VendorID != null)
            {
                query.WhereAnd<Where<BAccount.bAccountID, Equal<Current<AdditionsFilter.vendorID>>>>();
            }
            if (filter.ValuationDateFrom != null)
            {
                query.WhereAnd<Where<KGValuation.valuationDate, GreaterEqual<Current<AdditionsFilter.valuationDateFrom>>>>();
            }
            if (filter.ValuationDateTo != null)
            {
                query.WhereAnd<Where<KGValuation.valuationDate, LessEqual<Current<AdditionsFilter.valuationDateTo>>>>();
            }
            return query.Select();
        }

        public PXAction<APInvoice> addAddition;
        [PXUIField(DisplayName = "Add Addition", MapEnableRights = PXCacheRights.Select,
                MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddAddition(PXAdapter adapter)
        {
            APInvoice master = Base.Document.Current;
            try
            {
                //additionFilters.Current.VendorID = master.VendorID;
                //additionFilters.Current.ContractID = getProjectID();
                //additionFilters.Cache.Clear();
                AdditionsFilter filter = additionFilters.Current;
                filter.VendorID = master.VendorID;
                filter.ContractID = getProjectID();
                additionFilters.Update(additionFilters.Current);
                WebDialogResult result = this.additionDetails.AskExt(true);

                if (result == WebDialogResult.OK)
                {
                    return AddAdditionLines(adapter);
                }
                else if (result == WebDialogResult.Cancel)
                {
                    this.additionDetails.Cache.Clear();
                    this.additionDetails.Cache.ClearQueryCache();
                }
                else
                {
                    this.additionDetails.Cache.Clear();
                    this.additionDetails.Cache.ClearQueryCache();
                }
            }
            catch (Exception e)
            {
                this.additionDetails.Cache.Clear();
                this.additionDetails.Cache.ClearQueryCache();
                additionFilters.Current.VendorID = master.VendorID;
                additionFilters.Current.ContractID = getProjectID();
                additionFilters.Current.OrderNbr = getPoNbr();
                //additionFilters.Current.ContractID = getProjectID();
                additionFilters.Update(additionFilters.Current);
                throw e;
            }
            return adapter.Get();
        }
        public PXAction<APInvoice> addAdditionLines;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select,
                          MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddAdditionLines(PXAdapter adapter)
        {
            foreach (KGValuationDetail additionLine in additionDetails.Select())
            {
                if (additionLine.Selected != true)
                {
                    continue;
                }

                if (!checkLine(additionLine))
                {
                    continue;
                }
                PXGraph graph = new PXGraph();
                PXCache valuationCache = Base.Transactions.Cache;
                KGValuation kgValuation = PXSelectReadonly<KGValuation,
                            Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>.Select(graph, additionLine.ValuationID);

                /*
                POLine poLine = PXSelectReadonly<POLine,
                                Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>.Select(graph, additionLine.OrderNbr, 1);*/



                //additionLine.LineNbr
                APTran billLine = CreateAPTran(additionLine, kgValuation);

                billLine.Qty = additionLine.Qty;
                billLine.UnitCost = additionLine.UnitPrice;
                billLine.CuryUnitCost = additionLine.UnitPrice;
                billLine.CuryLineAmt = additionLine.Amount;
                billLine.CuryTranAmt = additionLine.Amount;
                billLine.TranDesc = kgValuation.ValuationCD + "-" + getKGValuationType(kgValuation.ValuationType) + "-" + kgValuation.Description;
                //billLine.ReceiptNbr = kgValuation.ValuationCD;

                //billLine = Base.Transactions.Insert(billLine);
                valuationCache.SetValue<APTranExt.usrValuationID>(billLine, additionLine.ValuationID);
                valuationCache.SetValue<APTranExt.usrValuationType>(billLine, "A");
                billLine = Base.Transactions.Insert(billLine);
                /*
                if (additionLine.ManageFeeAmt != null && additionLine.ManageFeeAmt > 0) {
                    APTran manageFree = CreateAPTran(additionLine, kgValuation);

                    manageFree.Qty = 1;
                    manageFree.UnitCost = additionLine.ManageFeeAmt;
                    manageFree.CuryLineAmt = additionLine.ManageFeeAmt;
                    manageFree.CuryTranAmt = additionLine.ManageFeeAmt;
                    manageFree.TranDesc = kgValuation.ValuationCD + "-" + kgValuation.ValuationType+"-"+ "Management fee";

                    manageFree = Base.Transactions.Insert(manageFree);
                    valuationCache.SetValue<APTranExt.usrValuationID>(manageFree, additionLine.ValuationID);
                    valuationCache.SetValue<APTranExt.usrValuationType>(manageFree, "A");
                }*/
            }
            additionDetails.Cache.Clear();
            additionDetails.Cache.ClearQueryCache();
            return adapter.Get();
        }

        #endregion

        #region 扣款
        [PXCopyPasteHiddenView]
        public PXFilter<DeductionsFilter> deductionFilters;
        public PXSelect<KGDeductionAPTran, Where<KGDeductionAPTran.refNbr, Equal<Current<APInvoice.refNbr>>>> deductionAPTranDetails;
        /// <summary>
        /// Define new data view with specific conditions.
        /// </summary>
        public PXSelect<KGDeductionAPTran,
                        Where<KGDeductionAPTran.refNbr, Equal<Current<APInvoice.refNbr>>,
                              And<KGDeductionAPTran.inventoryID, NotEqual<Current<KGSetUp.kGIndividualTaxInventoryID>>,
                                  And<KGDeductionAPTran.inventoryID, NotEqual<Current<KGSetUp.kGSupplementaryTaxInventoryID>>>>>> DeduAPTranView;
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public PXSelectJoin<KGValuationDetailDeduction,
                                InnerJoin<KGValuation,
                                          On<KGValuation.valuationID, Equal<KGValuationDetailDeduction.valuationID>>,
                                          LeftJoin<POOrder,
                                            On<POOrder.orderNbr, Equal<KGValuationDetailDeduction.orderNbr>>,
                                            LeftJoin<BAccount,
                                                     On<BAccount.bAccountID, Equal<POOrder.vendorID>>>>>,
                                Where<KGValuationDetailDeduction.pricingType, Equal<PricingTypeDeduction>,
                                    And<KGValuationDetailDeduction.aPInvoiceNbr, IsNull,
                                    And<KGValuationDetailDeduction.aPInvoiceDate, IsNull,
                                    And<KGValuationDetail.status, Equal<StatusV>,
                                    //2021/06/16 Add 去掉會計票貼
                                    And<KGValuation.valuationType, NotEqual<KGValuationTypeStringList.three>>>>>>> deductionDetails;

        protected virtual IEnumerable DeductionDetails()
        {
            PXSelectBase<KGValuationDetailDeduction> query = new PXSelectJoin<KGValuationDetailDeduction,
                                InnerJoin<KGValuation,
                                          On<KGValuation.valuationID, Equal<KGValuationDetailDeduction.valuationID>>,
                                          LeftJoin<POOrder,
                                            On<POOrder.orderNbr, Equal<KGValuationDetailDeduction.orderNbr>>,
                                            LeftJoin<BAccount,
                                                     On<BAccount.bAccountID, Equal<POOrder.vendorID>>>>>,
                                Where<KGValuationDetailDeduction.pricingType, Equal<PricingTypeDeduction>,
                                    And<KGValuationDetailDeduction.aPInvoiceNbr, IsNull,
                                    And<KGValuationDetailDeduction.aPInvoiceDate, IsNull,
                                    And<KGValuationDetail.status, Equal<StatusV>>>>>>(Base);

            DeductionsFilter filter = deductionFilters.Current;
            if (filter.ContractID != null)
            {
                query.WhereAnd<Where<KGValuationDetailDeduction.contractID, Equal<Current<DeductionsFilter.contractID>>>>();
            }
            if (filter.OrderNbr != null)
            {
                query.WhereAnd<Where<KGValuationDetailDeduction.orderNbr, Equal<Current<DeductionsFilter.orderNbr>>>>();
            }
            if (filter.VendorID != null)
            {
                query.WhereAnd<Where<BAccount.bAccountID, Equal<Current<DeductionsFilter.vendorID>>>>();
            }
            if (filter.ValuationDateFrom != null)
            {
                query.WhereAnd<Where<KGValuation.valuationDate, GreaterEqual<Current<DeductionsFilter.valuationDateFrom>>>>();
            }
            if (filter.ValuationDateTo != null)
            {
                query.WhereAnd<Where<KGValuation.valuationDate, LessEqual<Current<DeductionsFilter.valuationDateTo>>>>();
            }
            return query.Select();
        }

        public PXAction<APInvoice> addDeduction;
        [PXUIField(DisplayName = "Add Deduction", MapEnableRights = PXCacheRights.Select,
                   MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddDeduction(PXAdapter adapter)
        {
            APInvoice master = Base.Document.Current;
            try
            {
                DeductionsFilter filter = deductionFilters.Current;
                filter.VendorID = master.VendorID;
                filter.ContractID = getProjectID();
                deductionFilters.Update(deductionFilters.Current);
                WebDialogResult result = this.deductionDetails.AskExt(true);
                if (result == WebDialogResult.OK)
                {
                    return AddDeductionLines(adapter);
                }
                else if (result == WebDialogResult.Cancel)
                {
                    this.deductionDetails.Cache.Clear();
                    this.deductionDetails.Cache.ClearQueryCache();
                }
                else
                {
                    this.deductionDetails.Cache.Clear();
                    this.deductionDetails.Cache.ClearQueryCache();
                }
            }
            catch (Exception e)
            {
                this.deductionDetails.Cache.Clear();
                this.deductionDetails.Cache.ClearQueryCache();
                DeductionsFilter filter = deductionFilters.Current;
                filter.VendorID = master.VendorID;
                filter.ContractID = getProjectID();
                filter.OrderNbr = getPoNbr();
                deductionFilters.Update(deductionFilters.Current);
                throw e;
            }
            return adapter.Get();
        }

        public PXAction<APInvoice> addDeductionLines;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select,
                              MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddDeductionLines(PXAdapter adapter)
        {
            foreach (KGValuationDetailDeduction deductionLine in deductionDetails.Select())
            {
                if (deductionLine.Selected != true)
                {
                    continue;
                }

                if (!checkDeductionLine(deductionLine))
                {
                    continue;
                }
                PXGraph graph = new PXGraph();
                KGValuation kgValuation = PXSelectReadonly<KGValuation,
                           Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>.Select(graph, deductionLine.ValuationID);
                KGDeductionAPTran dedBillLine = (KGDeductionAPTran)createKGDeductionAPTran(deductionLine);
                dedBillLine.Qty = deductionLine.Qty;
                dedBillLine.UnitCost = deductionLine.UnitPrice;
                dedBillLine.CuryUnitCost = deductionLine.UnitPrice;

                dedBillLine.CuryLineAmt = deductionLine.Amount;
                dedBillLine.CuryTranAmt = deductionLine.Amount;
                dedBillLine.IsManageFee = false;
                dedBillLine.TranDesc = kgValuation.ValuationCD + "-" + getKGValuationType(kgValuation.ValuationType) + "-" + kgValuation.Description;
                //採購收獲號碼
                //dedBillLine.ReceiptNbr = kgValuation.ValuationCD;

                PMCostBudget pmCostBudget = getPMCostBudget(dedBillLine.ProjectID, dedBillLine.InventoryID);
                if (pmCostBudget != null)
                {
                    dedBillLine.TaskID = pmCostBudget.TaskID;
                    dedBillLine.CostCodeID = pmCostBudget.CostCodeID;
                }

                this.deductionAPTranDetails.Insert(dedBillLine);
                if (deductionLine.ManageFeeAmt != null && deductionLine.ManageFeeAmt > 0)
                {
                    KGDeductionAPTran manageFree = (KGDeductionAPTran)createKGDeductionAPTran(deductionLine);
                    manageFree.Qty = 1;
                    manageFree.TranDesc = kgValuation.ValuationCD + "-" + getKGValuationType(kgValuation.ValuationType) + "-" + "Management fee" + "-" + kgValuation.Description;
                    manageFree.UnitCost = deductionLine.ManageFeeAmt;
                    manageFree.CuryLineAmt = deductionLine.ManageFeeAmt;
                    manageFree.CuryTranAmt = deductionLine.ManageFeeAmt;
                    manageFree.CuryUnitCost = deductionLine.ManageFeeAmt;

                    manageFree.CuryTaxableAmt = deductionLine.ManageFeeAmt;
                    manageFree.TaxableAmt = deductionLine.ManageFeeAmt;
                    manageFree.CuryTaxAmt = deductionLine.ManageFeeTaxAmt;
                    manageFree.TaxAmt = deductionLine.ManageFeeTaxAmt;
                    manageFree.IsManageFee = true;

                    //manageFree.ReceiptNbr = kgValuation.ValuationCD;
                    KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
                    if (setUp != null)
                    {
                        //modify by louis 20200527
                        manageFree.InventoryID = setUp.KGManageInventoryID;
                        pmCostBudget = getPMCostBudget(manageFree.ProjectID, setUp.KGManageInventoryID);
                    }
                    else
                    {
                        pmCostBudget = null;
                    }

                    if (pmCostBudget != null)
                    {
                        manageFree.TaskID = pmCostBudget.TaskID;
                        manageFree.CostCodeID = pmCostBudget.CostCodeID;
                    }

                    manageFree = this.deductionAPTranDetails.Insert(manageFree);
                }
            }
            deductionDetails.Cache.Clear();
            deductionDetails.Cache.ClearQueryCache();
            return adapter.Get();
        }
        //暫時需求先將某些欄位ReadOnly
        protected virtual void KGDeductionAPTran_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.selected>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.branchID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.inventoryID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.tranDesc>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.qty>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.uOM>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.curyUnitCost>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.curyDiscAmt>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.curyTranAmt>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.accountID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.subID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.projectID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.taskID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.costCodeID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.nonBillable>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.taxCategoryID>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.date>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.pONbr>(cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.receiptNbr>(cache, e.Row, false);
            APInvoice master = Base.Document.Current;
            if (master.Hold == false)
            {
                PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.curyLineAmt>(cache, e.Row, false);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled<KGDeductionAPTran.curyLineAmt>(cache, e.Row, true);
            }
        }

        /// <summary>
        /// Add a button to generate a withholding tax line.
        /// </summary>
        public PXAction<APInvoice> addWHT;
        [PXUIField(DisplayName = "Add Withholding Tax", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable AddWHT(PXAdapter adapter)
        {
            try
            {
                CreateDeductionDetails(KGSetup.Current.KGIndividualTaxInventoryID);
            }
            catch (Exception e)
            {
                throw e;
            }

            return adapter.Get();
        }

        /// <summary>
        /// Add a button to generate a supplementary premium line.
        /// </summary>
        public PXAction<APInvoice> addSupPerm;
        [PXUIField(DisplayName = "Add Supplementary Premium", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable AddSupPerm(PXAdapter adapter)
        {
            try
            {
                CreateDeductionDetails(KGSetup.Current.KGSupplementaryTaxInventoryID);
            }
            catch (Exception e)
            {
                throw e;
            }

            return adapter.Get();
        }

        /// <summary>
        /// Create one deduction line.
        /// </summary>
        /// <param name="inventoryID"></param>
        private void CreateDeductionDetails(int? inventoryID)
        {
            APTran aPTran = Base.Transactions.Current;
            KGDeductionAPTran row = deductionAPTranDetails.Cache.CreateInstance() as KGDeductionAPTran;

            row.InventoryID = inventoryID;
            row.Qty = 1;
            row.ProjectID = Base.CurrentDocument.Current.ProjectID;

            if (inventoryID == KGSetup.Current.KGIndividualTaxInventoryID)
            {
                row.ValuationType = "W";
                // In AP Bill, user will select tax zone for withholding tax 'WH10 or WH20 or WH05'
                // From the tax zone you can get TAX ID.  From the TAX ID, you can get TAX percentage
                // 在"扣款"頁簽 產生 代扣稅時 CurrencyUnitCode (單位成本) = KGBillsummary.PoValuationAmt (估驗金額) * TAX Rate
                TaxRev taxRev = PXSelectReadonly<TaxRev,
                                                 Where<TaxRev.taxID, Equal<Required<APInvoice.taxZoneID>>>>
                                                 .Select(Base, Base.Document.Current.TaxZoneID);

                row.CuryUnitCost = SummaryAmtFilters.Current.PoValuationAmt * (taxRev.TaxRate / 100);
            }
            else
            {
                row.ValuationType = "S";
                // 在"扣款"頁簽 產生 健保補充保費時 CurrencyUnitCode (單位成本) = KGBillsummary.PoValuationAmt (估驗金額) * 健保補充保費費率
                row.CuryUnitCost = SummaryAmtFilters.Current.PoValuationAmt * (KGSetup.Current.KGSupplPremiumRate / 100);
            }

            PMCostBudget costBudget = getPMCostBudget(row.ProjectID, row.InventoryID);

            if (costBudget != null)
            {
                row.TaskID = costBudget.TaskID;
                row.CostCodeID = costBudget.CostCodeID;
            }

            InventoryItem inventItem = (InventoryItem)PXSelectorAttribute.Select(deductionAPTranDetails.Cache,
                                                                                 row,
                                                                                 deductionAPTranDetails.Cache.GetField(typeof(KGDeductionAPTran.inventoryID)));

            row.TranDesc = inventItem.InventoryCD + "-" +
                                PXStringListAttribute.GetLocalizedLabel<APTranExt.usrValuationType>(Base.Transactions.Cache,
                                                                                                    aPTran, row.ValuationType) + "-" +
                                inventItem.Descr;
            row.PONbr = aPTran.PONbr;
            row.POOrderType = aPTran.POOrderType;
            row.VendorID = aPTran.VendorID;

            deductionAPTranDetails.Cache.Insert(row);
        }

        public KGDeductionAPTran createKGDeductionAPTran(KGValuationDetailDeduction deductionLine)
        {
            PXCache deductionCache = this.deductionAPTranDetails.Cache;
            KGDeductionAPTran dedBillLine = (KGDeductionAPTran)deductionCache.CreateInstance();
            //get PO Order Line DAC
            PXGraph graph = new PXGraph();
            /*
            POLine poLine = PXSelectReadonly<POLine,
                            Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                            And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>.Select(graph, deductionLine.OrderNbr, 1);*/
            //deductionLine.LineNbr;
            dedBillLine.PONbr = deductionLine.OrderNbr;
            //deductionLine.LineNbr
            //dedBillLine.POLineNbr = 1;
            dedBillLine.POOrderType = deductionLine.OrderType;
            dedBillLine.InventoryID = deductionLine.InventoryID;
            //dedBillLine.TranDesc = poLine.TranDesc;

            //dedBillLine.AccountID = poLine.ExpenseAcctID;
            //ExpenseAcctID-DESCR
            //dedBillLine.SubID = poLine.ExpenseSubID;
            dedBillLine.ProjectID = deductionLine.ContractID;
            // dedBillLine.TaskID = poLine.TaskID;
            // dedBillLine.CostCodeID = poLine.CostCodeID;
            //dedBillLine.TaxCategoryID = poLine.TaxCategoryID;
            dedBillLine.ValuationID = deductionLine.ValuationID;
            dedBillLine.ValuationType = "D";
            dedBillLine.CuryTaxableAmt = deductionLine.Amount;
            dedBillLine.CuryTaxAmt = deductionLine.TaxAmt;
            dedBillLine.CuryTranAmt = deductionLine.Amount;
            dedBillLine.RetainagePct = 0;
            dedBillLine.CuryRetainageAmt = 0;
            dedBillLine.UOM = deductionLine.Uom;

            return dedBillLine;
        }

        public String getKGValuationType(String type)
        {
            //代扣代付
            if ("0".Equals(type))
            {
                return Message.KGValuationType_0;
            }
            //罰款
            else if ("1".Equals(type))
            {
                return Message.KGValuationType_1;
            }
            //一般扣款
            else if ("2".Equals(type))
            {
                return Message.KGValuationType_2;
            }
            //2021/06/16 Add 加扣款多一類型 票貼
            else if (KGValuationTypeStringList._3 == type)
            {
                return Message.KGValuationType_3;
            }
            else if (KGValuationTypeStringList._4 == type)
            {
                return KGValuationTypeStringList._4;
            }
            else if (KGValuationTypeStringList._5 == type)
            {
                return KGValuationTypeStringList._5;
            }
            return "";
        }
        #endregion

        #region 保留款退回
        public PXSelect<APInvoiceKedgeExt, Where<APRegisterExt.usrPONbr, IsNotNull>> retainageList;
        protected virtual IEnumerable RetainageList()
        {
            foreach (APInvoiceKedgeExt doc in PXSelect<APInvoiceKedgeExt>.Select(Base))
            {
                bool hasUnreleasedDocument = false;

                foreach (PXResult<APRetainageInvoice, APTran> res in PXSelectJoin<APRetainageInvoice,
                    LeftJoin<APTran, On<APRetainageInvoice.paymentsByLinesAllowed, Equal<True>,
                        And<APTran.tranType, Equal<APRetainageInvoice.docType>,
                        And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>,
                        And<APTran.origLineNbr, Equal<Required<APTran.origLineNbr>>>>>>>,
                Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                    And<APRetainageInvoice.origDocType, Equal<Required<APInvoice.docType>>,
                    And<APRetainageInvoice.origRefNbr, Equal<Required<APInvoice.refNbr>>,
                        And<APRetainageInvoice.released, NotEqual<True>>>>>>
                    .Select(Base, doc.APTranLineNbr, doc.DocType, doc.RefNbr))
                {
                    APRetainageInvoice invoice = res;
                    APTran tran = res;

                    if (invoice.PaymentsByLinesAllowed != true || tran.LineNbr != null)
                    {
                        hasUnreleasedDocument = true;
                    }
                }

                if (!hasUnreleasedDocument)
                {
                    yield return doc;
                }
            }
        }

        public PXAction<APInvoice> returnRetainage;
        [PXUIField(DisplayName = "Return Retainage", MapEnableRights = PXCacheRights.Select,
               MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable ReturnRetainage(PXAdapter adapter)
        {
            APInvoice curDoc = (APInvoice)this.Base.Document.Current;
            foreach (APInvoiceKedgeExt retainageLine in retainageList.Select())
            {

                /*PXResult<APInvoice, CurrencyInfo, Terms, Vendor> resultDoc =
                APInvoice_CurrencyInfo_Terms_Vendor
                    .SelectSingleBound(this.Base, null, curDoc.DocType, curDoc.RefNbr, curDoc.VendorID)
                    .Cast<PXResult<APInvoice, CurrencyInfo, Terms, Vendor>>()
                    .First();*/

                if (retainageLine.Selected != true)
                {
                    continue;
                }
                APInvoice master = Base.Document.Current;
                //CurrencyInfo info = resultDoc;
                //APInvoice origInvoice = resultDoc;
                Vendor vendor = null;

                if (retainageLine.VendorID != null)
                {
                    vendor = getVendor(retainageLine.VendorID.Value);
                }
                APTran retainBillLine = (APTran)Base.Transactions.Cache.CreateInstance();
                retainBillLine.BranchID = curDoc.BranchID;
                //retainBillLine.TaxCategoryID = retainageLine.TaxCategoryID;
                retainBillLine.AccountID = curDoc.RetainageAcctID;
                retainBillLine.SubID = curDoc.RetainageSubID;
                //retainBillLine.ProjectID = master.ProjectID;

                retainBillLine.CuryUnitCost = 0m;
                retainBillLine.ManualDisc = true;
                retainBillLine.DiscPct = 0m;
                retainBillLine.CuryDiscAmt = 0m;
                retainBillLine.RetainagePct = 0m;
                retainBillLine.CuryRetainageAmt = 0m;
                retainBillLine.CuryTaxableAmt = 0m;
                retainBillLine.CuryTaxAmt = 0;
                retainBillLine.CuryExpenseAmt = 0m;
                retainBillLine.GroupDiscountRate = 1m;
                retainBillLine.DocumentDiscountRate = 1m;

                //retainBillLine.ProjectID = retainageLine.ProjectID;
                //retainBillLine.TaskID = retainageLine.APTranTaskID;
                //retainBillLine.CostCodeID = retainageLine.APTranCostCodeID;
                //retainBillLine.ProjectID = master.ProjectID;
                using (new PXLocaleScope(vendor.LocaleName))
                {
                    retainBillLine.TranDesc = PXMessages.LocalizeFormatNoPrefix(
                        Messages.RetainageForTransactionDescription,
                        APDocTypeDict[curDoc.DocType],
                        retainageLine.RefNbr);
                }

                // prevCuryTotal = (retainageLine.CuryRetainageAmt ?? 0m) - (this.Base.Document.Current.CuryDocBal ?? 0m);
                //retainageLine.CuryLineAmt = PXCurrencyAttribute.RoundCury(this.Base.Transactions.Cache, tranNew, (detail.CuryRetainageAmt ?? 0m) * retainagePercent);
                retainBillLine.RetainagePct = 0;
                retainBillLine.CuryRetainageAmt = 0;

                KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
                PMCostBudget pmCostBudget = null;
                if (setUp != null)
                {
                    pmCostBudget = getPMCostBudget(master.ProjectID, setUp.KGRetainageReturnInventoryID);
                }
                else
                {
                    pmCostBudget = null;
                }
                retainBillLine.Qty = 1;

                retainBillLine.ProjectID = ProjectDefaultAttribute.NonProject();
                APTran tran = Base.Transactions.Insert(retainBillLine);
                Base.Transactions.Cache.SetValue<APTranExt.usrValuationType>(tran, "R");

                if (pmCostBudget != null)
                {

                    Base.Transactions.Cache.SetValueExt<APTran.inventoryID>(tran, pmCostBudget.InventoryID);
                    Base.Transactions.Cache.SetValueExt<APTran.taskID>(tran, pmCostBudget.TaskID);
                    Base.Transactions.Cache.SetValueExt<APTran.costCodeID>(tran, pmCostBudget.CostCodeID);

                    /*retainBillLine.TaskID = pmCostBudget.TaskID;
                    retainBillLine.CostCodeID = pmCostBudget.CostCodeID;
                    retainBillLine.InventoryID = pmCostBudget.InventoryID;*/
                }
                tran.CuryLineAmt = retainageLine.CuryRetainageReleasedAmt;
                tran.CuryTranAmt = retainageLine.CuryRetainageReleasedAmt;

                tran.PONbr = retainageLine.PONbr;
                tran.POOrderType = getPOOrderType();
                tran.CuryUnitCost = retainageLine.CuryRetainageReleasedAmt;
                tran = Base.Transactions.Update(tran);
            }

            //setValuationALL();
            return adapter.Get();
        }

        protected virtual void APInvoiceKedgeExt_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            APInvoiceKedgeExt apInvoiceKedgeExt = (APInvoiceKedgeExt)e.Row;
            if (apInvoiceKedgeExt == null)
            {
                return;
            }
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.branchID>(cache, apInvoiceKedgeExt, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.docType>(cache, apInvoiceKedgeExt, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.refNbr>(cache, apInvoiceKedgeExt, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.pONbr>(cache, apInvoiceKedgeExt, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.vendorID>(cache, apInvoiceKedgeExt, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.curyRetainageUnreleasedCalcAmt>(cache, apInvoiceKedgeExt, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.retainageVendorRef>(cache, apInvoiceKedgeExt, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceKedgeExt.docDate>(cache, apInvoiceKedgeExt, false);
            PXGraph graph = new PXGraph();
            if (apInvoiceKedgeExt.RefNbr != null)
            {
                if (apInvoiceKedgeExt.PONbr == null)
                {
                    APTran apTran = PXSelectGroupBy<APTran, Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                    And<APTran.tranType, Equal<Required<APTran.tranType>>, And<APTran.pONbr, IsNotNull>>>,
                    Aggregate<GroupBy<APTran.refNbr>>>.Select(graph, apInvoiceKedgeExt.RefNbr, apInvoiceKedgeExt.DocType);
                    if (apTran != null)
                    {
                        apInvoiceKedgeExt.PONbr = apTran.PONbr;
                    }
                }
            }
        }

        //參考至 APInvoiceEntryRetainage  338
        /*
         PXSelectJoin<APRetainageInvoice,
			InnerJoinSingleTable<APInvoice, On<APInvoice.docType, Equal<APRetainageInvoice.docType>,
				And<APInvoice.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
			Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
				And<APRetainageInvoice.origDocType, Equal<Optional<APInvoice.docType>>,
				And<APRetainageInvoice.origRefNbr, Equal<Optional<APInvoice.refNbr>>>>>>
        
       */
        [PXReadOnlyView]
        [PXCopyPasteHiddenView]
        public PXSelectJoin<APRetainageInvoice,
            LeftJoin<APTran, On<APTran.tranType, Equal<APRetainageInvoice.docType>,
                And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
            Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                And<APTranExt.usrOrigDocType, Equal<Optional<APInvoice.docType>>,
                And<APTranExt.usrOrigRefNbr, Equal<Optional<APInvoice.refNbr>>>>>> RetainageDocuments;

        protected virtual IEnumerable retainageDocuments()
        {
            APInvoice master = Base.Document.Current;
            if (master != null)
            {
                return PXSelectJoin<APRetainageInvoice,
                    LeftJoin<APTran, On<APTran.tranType, Equal<APRetainageInvoice.docType>,
                        And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
                    Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                        And<APTranExt.usrOrigDocType, Equal<Required<APInvoice.docType>>,
                        And<APTranExt.usrOrigRefNbr, Equal<Required<APInvoice.refNbr>>>>>>.Select(Base, master.DocType, master.RefNbr);
            }
            else
            {
                return PXSelectJoin<APRetainageInvoice,
                    LeftJoin<APTran, On<APTran.tranType, Equal<APRetainageInvoice.docType>,
                        And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
                    Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                        And<APTranExt.usrOrigDocType, Equal<Optional<APInvoice.docType>>,
                        And<APTranExt.usrOrigRefNbr, Equal<Optional<APInvoice.refNbr>>>>>>.Select(Base);
            }
        }
        #endregion

        #region 加總
        public PXSelect<KGBillSummary, Where<KGBillSummary.docType, Equal<Current<APInvoice.docType>>,
                    And<KGBillSummary.refNbr, Equal<Current<APInvoice.refNbr>>>>> SummaryAmtFilters;
        Decimal getBillCumulativeQty(int? segPricingID, int? billSegPricingID)
        {
            if (segPricingID == null)
            {
                return 0;
            }
            PXResultset<KGBillSegPricing> set =
                PXSelect<KGBillSegPricing, Where<KGBillSegPricing.segPricingID, Equal<Required<KGBillSegPricing.segPricingID>>,
                                                  And<KGBillSegPricing.billSegPricingID, NotEqual<Current<KGBillSegPricing.billSegPricingID>>>>>.
                                                  Select(Base, segPricingID.Value, billSegPricingID);
            if (set.Count == 0)
            {
                return 0;
            }
            else
            {
                Decimal sum = 0;
                foreach (KGBillSegPricing segPricing in set)
                {
                    if (segPricing.BillQty != null)
                    {
                        sum = sum + segPricing.BillQty.Value;
                    }
                }
                return sum;
            }
        }
        public KGBillSummary getKGBillSummary()
        {
            KGBillSummary sum = SummaryAmtFilters.Current;
            APInvoice maste = Base.Document.Current;
            if (maste == null) return null;
            if (sum == null)
            {
                if (SummaryAmtFilters.Select().Count == 0)
                {
                    //SummaryAmtFilters.Insert();
                    SummaryAmtFilters.Current = SummaryAmtFilters.Insert();
                    //SummaryAmtFilters.Current = (KGBillSummary)SummaryAmtFilters.Cache.CreateInstance();
                    sum = SummaryAmtFilters.Current;
                }
                else
                {
                    SummaryAmtFilters.Current = SummaryAmtFilters.Select();
                    sum = SummaryAmtFilters.Select();
                }
            }
            return sum;
        }

        public Decimal? getTaxRate(string refNbr)
        {
            Decimal? taxRate = 0;
            APTaxTran apTaxTran = Base.Taxes.Select();

            if (apTaxTran == null)
            {
                apTaxTran = SelectFrom<APTaxTran>.Where<APTaxTran.module.IsEqual<GL.BatchModule.moduleAP>.
                                                       And<APTaxTran.refNbr.IsEqual<P.AsString>>>.
                                                       View.Select(Base, refNbr);
                 ;
            }
            if (apTaxTran!=null) {
                taxRate = getValue(apTaxTran.TaxRate) / 100;
            }
            
            return taxRate;
        }

        public Decimal? getTaxRate()
        {
            Decimal? taxRate = 0;
            APTaxTran apTaxTran = Base.Taxes.Select();

            if (apTaxTran != null)
            {
                taxRate = getValue(apTaxTran.TaxRate) / 100;
            }
            
            return taxRate;
        }

        public Decimal? getTaxAmt()
        {
            Decimal? taxAmt = 0;
            APTaxTran apTaxTran = Base.Taxes.Select();
            if (apTaxTran != null)
            {
                taxAmt = getValue(apTaxTran.CuryTaxAmt);
            }
            return taxAmt;
        }

        //期數影響-做Summary前
        public decimal? GetPreviousWithholdingTax(APInvoice master)
        {
            decimal? sum = 0;
            PXResultset<APInvoice> set = getInvoiceFromVendor(master, getUsrValuationPhase());
            foreach (APInvoice apInvoice in set)
            {
                PXResultset<APTran> set2 = getAPTran(apInvoice, master.ProjectID);
                foreach (APTran apTran in set2)
                {
                    APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                    if (("W".Equals(apTranExt.UsrValuationType) || "S".Equals(apTranExt.UsrValuationType)))
                    {
                        sum += apTran.CuryLineAmt ?? 0;
                    }
                }
            }
            return sum;
        }

        //期數影響
        public Dictionary<String, decimal> getPoAcumAmtAndQty(APTran row)
        {
            PXGraph graph = new PXGraph();
            Dictionary<String, decimal> dic = new Dictionary<String, decimal>();
            APInvoice master = Base.Document.Current;
            decimal sum = 0;
            decimal qty = 0;
            dic["sum"] = 0;
            dic["qty"] = 0;

            PXResultset<APTran> set2 = PXSelectJoin<APTran, InnerJoin<APRegister, On<APTran.tranType, Equal<APRegister.docType>,
                    And<APTran.refNbr, Equal<APRegister.refNbr>,
                    And<APTran.lineType, NotEqual<SOLineType.discount>>>>>,
                        Where<APRegister.vendorID, Equal<Required<APRegister.vendorID>>,
                    And<APRegisterExt.usrValuationPhase, Less<Required<APRegisterExt.usrValuationPhase>>,
                    And<APRegister.docType, In3<APDocType.invoice, APDocType.debitAdj>,
                    And<APTran.pOLineNbr, Equal<Required<APTran.pOLineNbr>>,
                    And<APTran.pONbr, Equal<Required<APTran.pONbr>>>>>>>>.
                    Select(graph, master.VendorID, getUsrValuationPhase(), row.POLineNbr, row.PONbr);
            if (set2.Count > 0)
            {
                Decimal sumPoValuationAmt = 0;
                Decimal sumPoValuationQty = 0;
                foreach (APTran apTran in set2)
                {
                    APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                    if (ckeckUsrValuationTypeisNull(apTranExt) && apTran.CuryLineAmt != null)
                    {
                        if (APDocType.DebitAdj.Equals(apTran.TranType))
                        {
                            sumPoValuationAmt = sumPoValuationAmt - apTran.CuryLineAmt.Value;
                        }
                        else
                        {
                            sumPoValuationAmt = sumPoValuationAmt + apTran.CuryLineAmt.Value;
                        }
                    }

                    if (ckeckUsrValuationTypeisNull(apTranExt) && apTran.Qty != null)
                    {
                        if (APDocType.DebitAdj.Equals(apTran.TranType))
                        {
                            sumPoValuationQty = sumPoValuationQty - apTran.Qty.Value;
                        }
                        else
                        {
                            sumPoValuationQty = sumPoValuationQty + apTran.Qty.Value;
                        }
                    }

                }
                sum = sumPoValuationAmt;
                qty = sumPoValuationQty;
                dic["sum"] = sum;
                dic["qty"] = qty;
            }

            return dic;
        }
        #endregion

        #region Seach & Static Methods
        private PXResultset<Vendor> getVendor(int vendorID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<Vendor> set = PXSelect<Vendor,
                    Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>.Select(graph, vendorID);
            return set;
        }

        public PXResultset<PMCostBudget> getPMCostBudget(int? projectID, int? inventoryID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<PMCostBudget> set = PXSelect<PMCostBudget, Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>>>>.Select(graph, projectID, inventoryID);
            return set;
        }
        //搜尋所有計價的累積金額
        public static PXResultset<APInvoice> getInvoiceFromVendor(APInvoice master, int? usrValuationPhase)
        {
            PXGraph graph = new PXGraph();

            PXResultset<APInvoice> set = PXSelect<APInvoice, Where<APInvoice.vendorID, Equal<Required<APInvoice.vendorID>>,
            And<APRegisterExt.usrValuationPhase, Less<Required<APRegisterExt.usrValuationPhase>>,
            And<APInvoice.docType, Equal<Required<APInvoice.docType>>>>>, OrderBy<Desc<APInvoice.docDate>>>.
            Select(graph, master.VendorID, usrValuationPhase, APDocType.Invoice);
            return set;
        }
        //當下無期數只能用日期
        public static PXResultset<APInvoice> getInvoiceFromVendor(APInvoice master)
        {
            PXGraph graph = new PXGraph();
            PXResultset<APInvoice> set = PXSelect<APInvoice, Where<APInvoice.vendorID, Equal<Required<APInvoice.vendorID>>,

            And<APInvoice.docDate, LessEqual<Required<APInvoice.docDate>>,
            And<APInvoice.docType, Equal<Required<APInvoice.docType>>,
            And<APInvoice.refNbr, NotEqual<Required<APInvoice.refNbr>>>>>>, OrderBy<Desc<APInvoice.docDate>>>.
            Select(graph, master.VendorID, master.DocDate, APDocType.Invoice, master.RefNbr);
            return set;
        }

        public static PXResultset<APTran> getAPTranFromVendor(APInvoice apInvoice, String poNbr)
        {
            PXGraph graph = new PXGraph();
            //新版不docType借方與計價都要入累加
            PXResultset<APTran> set2 = PXSelect<APTran, Where<APTran.tranType, Equal<Required<APInvoice.docType>>,
                And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTran.lineType, NotEqual<SOLineType.discount>,
                And<APTran.pONbr, Equal<Required<APTran.pONbr>>>>>>>.
            Select(graph, apInvoice.DocType, apInvoice.RefNbr, poNbr);
            return set2;
        }

        public static PXResultset<APTran> getAPTran(APInvoice apInvoice, int? projectID)
        {
            PXGraph graph = new PXGraph();
            //新版不docType借方與計價都要入累加
            return PXSelect<APTran,
                Where<APTran.tranType, Equal<Required<APInvoice.docType>>,
                And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTran.projectID, Equal<Required<APTran.projectID>>>>>>.
            Select(graph, apInvoice.DocType, apInvoice.RefNbr, projectID);
        }

        public List<APTran> getDebitAPTranFromVendor(APInvoice master, String poNbr, int? usrValuationPhase)
        {
            PXGraph graph = new PXGraph();
            List<APTran> list = new List<APTran>();
            foreach (PXResult<APTran, APRegister> result in PXSelectJoin<APTran, InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTran.refNbr>>>,
               Where<APTran.tranType, Equal<Required<APRegister.docType>>, And<APTran.lineType, NotEqual<SOLineType.discount>,
                   And<APTran.pONbr, Equal<Required<APTran.pONbr>>,
                   And<APRegister.vendorID, Equal<Required<APRegister.vendorID>>,
                   And<APRegisterExt.usrValuationPhase, Less<Required<APRegisterExt.usrValuationPhase>>,
                   And<APRegister.docType, Equal<Required<APRegister.docType>>>>>>>>,
               OrderBy<Desc<APRegister.docDate>>>.
               Select(graph, APDocType.DebitAdj, poNbr, master.VendorID
               , usrValuationPhase, APDocType.DebitAdj))
            {
                APTran apTran = (APTran)result;
                APRegister apInvoice = (APRegister)result;
                if (apInvoice.ProjectID == getProjectID())
                {
                    list.Add(apTran);
                }
            }

            return list;
        }
        //用在set Bill from to 已經有期數也就是已經保存過才能呼叫此方法
        public APInvoice getLastAPInvoiceByAPTran(APInvoice apInvoice, String poNbr)
        {
            PXGraph graph = new PXGraph();
            //新版不docType借方與計價都要入累加
            foreach (PXResult<APTran, APInvoice> result in PXSelectJoin<APTran, InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<APTran.refNbr>>>,
                Where<APTran.tranType, Equal<Required<APInvoice.docType>>, And<APTran.lineType, NotEqual<SOLineType.discount>,
                    And<APTran.pONbr, Equal<Required<APTran.pONbr>>,
                    And<APInvoice.vendorID, Equal<Required<APInvoice.vendorID>>,
                    And<APInvoice.docDate, LessEqual<Required<APInvoice.docDate>>,
                    And<APInvoice.docType, Equal<Required<APInvoice.docType>>,
                    And<APInvoice.refNbr, NotEqual<Required<APInvoice.refNbr>>>>>>>>>,
                OrderBy<Desc<APInvoice.docDate>>>.
                Select(graph, apInvoice.DocType, poNbr, apInvoice.VendorID
                , apInvoice.DocDate, APDocType.Invoice, apInvoice.RefNbr))
            {
                APTran apTran = (APTran)result;
                if (apInvoice.ProjectID == getProjectID())
                {
                    return (APInvoice)result;
                }

            }
            return null;
        }

        public static PXResultset<POOrder> getPOOrder(String orderType, String orderNbr)
        {
            PXGraph graph = new PXGraph();
            PXResultset<POOrder> poOrders = PXSelect<POOrder,
                           Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                                               And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(graph, orderType, orderNbr);
            return poOrders;
        }

        public static PXResultset<POLine> getPOLines(String orderType, String orderNbr)
        {
            PXGraph graph = new PXGraph();
            PXResultset<POLine> poLines = PXSelect<POLine,
                           Where<POLine.orderType, Equal<Required<POLine.orderType>>,
                                               And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>>>.Select(graph, orderType, orderNbr);
            return poLines;
        }

        public static PXResultset<POLine> getPOLine(String orderType, String orderNbr, int? lineNbr)
        {
            PXGraph graph = new PXGraph();
            PXResultset<POLine> poLines = PXSelect<POLine,
                           Where<POLine.orderType, Equal<Required<POLine.orderType>>,
                                               And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                                               And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>>.Select(graph, orderType, orderNbr, lineNbr);
            return poLines;
        }

        /// <summary>
        /// Using static label to select table buffer (KGContractPricingRule).
        /// </summary>
        /// <param name="pOOrderType"></param>
        /// <param name="pONbr"></param>
        /// <returns></returns>
        public static KGContractPricingRule GetContractPricingRule(string pOOrderType, string pONbr)
        {
            return PXSelect<KGContractPricingRule,
                            Where<KGContractPricingRule.orderType, Equal<Required<APTran.pOOrderType>>,
                                  And<KGContractPricingRule.orderNbr, Equal<Required<APTran.pONbr>>>>>
                            .Select(new PXGraph(), pOOrderType, pONbr);
        }

        /// <summary>
        /// Using static label to select table buffer (KGContractDoc).
        /// </summary>
        /// <param name="pOOrderType"></param>
        /// <param name="pONbr"></param>
        /// <returns></returns>
        public static KGContractDoc GetContractDoc(string pOOrderType, string pONbr)
        {
            return PXSelectReadonly<KGContractDoc,
                                    Where<KGContractDoc.orderNbr, Equal<Required<APTran.pONbr>>,
                                          And<KGContractDoc.orderType, Equal<Required<APTran.pOOrderType>>>>>
                                    .Select(new PXGraph(), pONbr, pOOrderType);
        }

        public static KGSetUp GetSetUp()
        {
            return PXSelect<KGSetUp>.Select(new PXGraph());
        }

        /// <summary>
        /// Calcualte refund retainage released amount and consider debit adjustment.
        /// It's used in APRegisterExtension class.
        /// </summary>
        public static decimal? CalcRetainageReleasedAmt(bool? isRetApply, string docType, string refNbr)
        {
            if (isRetApply != true) { return 0m; }

            decimal? retReleased = 0m;
            foreach (APTran tran in SelectFrom<APTran>.Where<APTranExt.usrOrigRetainageDocType.IsEqual<@P.AsString>
                                                             .And<APTranExt.usrOrigRetainageRefNbr.IsEqual<@P.AsString>
                                                                  .And<APTran.released.IsEqual<True>>>>.View.Select(new PXGraph(), docType, refNbr))
            {
                retReleased += (tran.TranType == APDocType.DebitAdj ? -tran.CuryLineAmt : tran.CuryLineAmt);
            }

            return retReleased;
        }
        #endregion

        #region Event Handlers
        #region APTran
        //add 2021-04-27 需判斷APTran的PONbr是否一致
        protected virtual void _(Events.RowPersisting<APTran> e)
        {
            APTran row = e.Row as APTran;
            if (row != null && row.PONbr != null)
            {
                APTran item = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                And<APTran.pONbr, NotEqual<Required<APTran.pONbr>>,
                And<APTran.pONbr, IsNotNull>>>>.Select(Base, row.RefNbr, row.PONbr);
                //當有不一致的資料時跳錯
                if (item != null)
                {
                    String error = "PONbr需一致";
                    Base.Transactions.Cache.RaiseExceptionHandling<APTran.pONbr>(row, row.PONbr,
                        new PXSetPropertyException(error, PXErrorLevel.RowError));
                }
            }
            //return;
            //抓取ponbr不一致的資料
        }

        //add by louis for default qty to 0 whern create invoice from PO/SC 20210112
        protected void APTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            APTran row = e.Row as APTran;
            APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(row);
            APInvoice doc = Base.Document.Current;
            if (APDocType.Invoice.Equals(doc.DocType)
                && doc.IsRetainageDocument == false
                && "B".Equals(apTranExt.UsrValuationType)
                && (doc.CreatedByScreenID.Equals("PO301000")
                || doc.CreatedByScreenID.Equals("SC301000")))
            {
                row.BaseQty = 0;
                row.Qty = 0;
                row.CuryTranAmt = 0;
                row.TranAmt = 0;
                row.CuryLineAmt = 0;
                row.LineAmt = 0;
                row.CuryRetainageAmt = 0;
                row.RetainageAmt = 0;
            }

        }

        protected void APTran_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            APTran row = e.Row as APTran;
            Dictionary<String, decimal> dic = getPoAcumAmtAndQty(row);

            APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(row);
            if (ckeckUsrValuationTypeisNull(apTranExt))
            {
                if (APDocType.DebitAdj.Equals(row.TranType))
                {
                    apTranExt.UsrTotalAccumulateAmt = apTranExt.UsrAccumulateAmt - row.CuryLineAmt;
                    apTranExt.UsrTotalAccumulateQty = apTranExt.UsrAccumulateQty - row.Qty;
                }
                else
                {
                    apTranExt.UsrAccumulateQty = dic["qty"];
                    apTranExt.UsrAccumulateAmt = dic["sum"];
                }
            }
            else
            {
                apTranExt.UsrAccumulateQty = 0;
                apTranExt.UsrAccumulateAmt = 0;
                apTranExt.UsrTotalAccumulateAmt = 0;
                apTranExt.UsrTotalAccumulateQty = 0;
            }
            apTranExt.UsrSegPricingFlag = true;
            PXResultset<KGContractSegPricing> set = PXSelect<KGContractSegPricing, Where<KGContractSegPricing.orderNbr, Equal<Required<APTran.pONbr>>,
                                      And<KGContractSegPricing.lineNbr, Equal<Required<APTran.pOLineNbr>>,
                                      And<KGContractSegPricing.orderType, Equal<Required<APTran.pOOrderType>>>>>>.Select(Base, row.PONbr, row.POLineNbr, row.POOrderType);

            if (set.Count == 0)
            {
                apTranExt.UsrSegPricingFlag = false;
            }
            else
            {
                apTranExt.UsrSegPricingFlag = true;
            }

            // YJ's request
            if (Base.CurrentDocument.Current.ProjectID == null)
            {
                //add by louis for Bill without PONbr
                if (row.PONbr != null)
                {
                    Base.CurrentDocument.Cache.SetValueExt<APInvoice.projectID>(Base.CurrentDocument.Current,
                                                                                                getCurrentPOOrder(row.POOrderType, row.PONbr).ProjectID);
                }
            }
        }

        protected void APTran_CuryLineAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            APTran row = e.Row as APTran;
            if (row == null) return;
            checkCuryUnitCost(row);
        }

        protected void APInvoice_CuryDocBal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            APInvoice row = e.Row as APInvoice;

            if (row != null && row.CuryDocBal > 0m
                && row.GetExtension<APRegisterExt>().UsrRetainagePct != null
                && row.GetExtension<APRegisterExt>().UsrRetainagePct > 0m)
            {
                Base.Document.Cache.SetValueExt<APRegisterExt.usrRetainageAmt>(row, CalcRetainageAmt(sender, row));
            }
        }

        protected void APTran_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            APTran row = e.Row as APTran;
            if (row == null) return;
            
            APInvoice doc = Base.Document.Current;
            //註解掉不需要的程式碼 20210713 by louis
            /**
            bool isPrebookedNotCompleted = (doc != null) && (doc.Prebooked == true && doc.Released == false && doc.Voided == false);
            bool isPOReceiptRelated = !string.IsNullOrEmpty(row.ReceiptNbr);
            bool isPOOrderRelated = !string.IsNullOrEmpty(row.PONbr);
            bool isPOOrderBasedBilling = isPOOrderRelated && (row.POAccrualType == POAccrualType.Order);
            bool isReverseTran = (row.OrigLineNbr != null);
            bool isLCBasedTranAP = false;
            bool is1099Enabled = Base.vendor.Current?.Vendor1099 == true && doc != null && doc.Voided != true;
            bool isDocumentReleased = doc?.Released == true;
            bool isDocumentDebitAdjustment = doc.DocType == APDocType.DebitAdj;


            if (row.LCDocType != null && row.LCRefNbr != null)
            {
                isLCBasedTranAP = true;
            }

            bool isStockItem = false;

            // When migration mode is activated in AP module,
            // we should process stock items the same way as nonstock
            // items, because we should allow entering both types of
            // inventory items without any additional links to PO.
            // 
            if (APSetup.Current?.MigrationMode != true && row.InventoryID != null)
            {
                InventoryItem item = PXSelect<InventoryItem,
                    Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, row.InventoryID);
                isStockItem = item?.StkItem == true;
            }

            if (isStockItem &&
                row.TranType != APDocType.Prepayment &&
                row.POAccrualType != POAccrualType.Order &&
                !isPOReceiptRelated)
            {
                cache.RaiseExceptionHandling<APTran.inventoryID>(row, row.InventoryID,
                    new PXSetPropertyException(Messages.NoLinkedtoReceipt, PXErrorLevel.Warning));
            }

            if (!isLCBasedTranAP)
            {
                if (isPOOrderRelated || isPOReceiptRelated)
                {
                    PXUIFieldAttribute.SetEnabled<APTran.inventoryID>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<APTran.uOM>(cache, row, isPOOrderBasedBilling && !isReverseTran);
                    if (isPOOrderBasedBilling && isReverseTran)
                    {
                        // do not allow changing the original qty and amount because otherwise it won't be able to revert the PPV amount
                        PXUIFieldAttribute.SetEnabled<APTran.qty>(cache, row, false);
                        PXUIFieldAttribute.SetEnabled<APTran.curyUnitCost>(cache, row, false);
                        PXUIFieldAttribute.SetEnabled<APTran.curyLineAmt>(cache, row, false);
                        PXUIFieldAttribute.SetEnabled<APTran.discPct>(cache, row, false);
                        PXUIFieldAttribute.SetEnabled<APTran.curyDiscAmt>(cache, row, false);
                        PXUIFieldAttribute.SetEnabled<APTran.manualDisc>(cache, row, false);
                        PXUIFieldAttribute.SetEnabled<APTran.discountID>(cache, row, false);
                    }
                }

                bool allowEdit = (doc != null) && (doc.Prebooked == false && doc.Released == false && doc.Voided == false);
                PXUIFieldAttribute.SetEnabled<APTran.defScheduleID>(cache, row, allowEdit && row.TranType == APDocType.DebitAdj);
                PXUIFieldAttribute.SetEnabled<APTran.deferredCode>(cache, row, allowEdit && row.DefScheduleID == null);

                if (doc.Released == false && doc.Prebooked == false)
                {
                    bool currencyChanged = false;

                    if (!string.IsNullOrEmpty(row.PONbr))
                    {
                        POOrder sourceDoc = PXSelect<POOrder,
                            Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                                                And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(Base, row.POOrderType, row.PONbr);
                        if (!string.IsNullOrEmpty(sourceDoc?.OrderNbr))
                        {
                            currencyChanged = (doc.CuryID != sourceDoc.CuryID);
                        }
                    }
                    if (currencyChanged)
                    {
                        cache.RaiseExceptionHandling<APTran.curyLineAmt>(row, row.CuryLineAmt,
                        new PXSetPropertyException(Messages.APDocumentCurrencyDiffersFromSourceDocument, PXErrorLevel.Warning));
                        cache.RaiseExceptionHandling<APTran.curyUnitCost>(row, row.CuryUnitCost,
                        new PXSetPropertyException(Messages.APDocumentCurrencyDiffersFromSourceDocument, PXErrorLevel.Warning));
                    }
                }

                if (isPOOrderRelated || isPOReceiptRelated)
                {
                    bool isPOPrepayment = (row.TranType == APDocType.Prepayment);
                    bool allowChangingAccount = !isDocumentReleased && !isPrebookedNotCompleted
                        && (!POLineType.UsePOAccrual(row.LineType) || isPOPrepayment);
                    PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, allowChangingAccount);
                    PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, allowChangingAccount);
                    PXUIFieldAttribute.SetEnabled<APTran.branchID>(cache, row, allowChangingAccount);
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, !isStockItem && (!isDocumentReleased || !is1099Enabled));
                    PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, !isStockItem && (!isDocumentReleased || !is1099Enabled));
                }
            }
            else
            {
                PXUIFieldAttribute.SetEnabled<APTran.qty>(cache, row, false);
                PXUIFieldAttribute.SetEnabled<APTran.accountID>(cache, row, false);
                PXUIFieldAttribute.SetEnabled<APTran.subID>(cache, row, false);
                PXUIFieldAttribute.SetEnabled<APTran.uOM>(cache, row, false);

                if (isDocumentDebitAdjustment && row.OrigLineNbr.HasValue)
                {
                    PXUIFieldAttribute.SetEnabled<APTran.curyUnitCost>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<APTran.curyLineAmt>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<APTran.discPct>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<APTran.curyDiscAmt>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<APTran.manualDisc>(cache, row, false);
                    PXUIFieldAttribute.SetEnabled<APTran.taxCategoryID>(cache, row, false);
                }
                //PXUIFieldAttribute.SetEnabled(cache, row, false);
            }

            bool isProjectEditable = !isPOOrderRelated;

            InventoryItem ns = (InventoryItem)PXSelectorAttribute.Select(cache, e.Row, cache.GetField(typeof(APTran.inventoryID)));
            if (ns != null && ns.StkItem != true && ns.NonStockReceipt != true)
            {
                isProjectEditable = true;
            }

            isProjectEditable = isProjectEditable && (!isDocumentReleased || !is1099Enabled);

            PXUIFieldAttribute.SetEnabled<APTran.projectID>(cache, row, isProjectEditable && APSetup.Current.RequireSingleProjectPerDocument != true);
            PXUIFieldAttribute.SetEnabled<APTran.taskID>(cache, row, isProjectEditable);

            PXUIFieldAttribute.SetEnabled<APTran.lCDocType>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTran.lCRefNbr>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTran.lCLineNbr>(cache, row, false);

            #region Migration Mode Settings

            if (doc != null &&
                doc.IsMigratedRecord == true &&
                doc.Released != true)
            {
                PXUIFieldAttribute.SetEnabled<APTran.defScheduleID>(Base.Transactions.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<APTran.deferredCode>(Base.Transactions.Cache, null, false);
            }
          
            #endregion
            **/
            //Coding By Jerry
            APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(row);
            if (apTranExt.InitFlag == null || apTranExt.InitFlag == true)
            {
                if (row.PONbr != null && row.POLineNbr != null)
                {
                    PXGraph graph = new PXGraph();
                    PXResultset<KGContractSegPricing> set = PXSelect<KGContractSegPricing, Where<KGContractSegPricing.orderNbr, Equal<Required<APTran.pONbr>>,
                                      And<KGContractSegPricing.lineNbr, Equal<Required<APTran.pOLineNbr>>,
                                      And<KGContractSegPricing.orderType, Equal<Required<APTran.pOOrderType>>>>>>.Select(graph, row.PONbr, row.POLineNbr, row.POOrderType);
                    if (set.Count == 0)
                    {
                        apTranExt.UsrSegPricingFlag = false;
                        //PXUIFieldAttribute.SetEnabled<APTran.qty>(cache, row, true);
                    }
                    else
                    {
                        apTranExt.UsrSegPricingFlag = true;
                    }
                    apTranExt.InitFlag = false;
                }
                else
                {
                    apTranExt.UsrSegPricingFlag = false;
                    //PXUIFieldAttribute.SetEnabled<APTran.qty>(cache, row, true);
                }
            }
            PXUIFieldAttribute.SetEnabled<APTran.qty>(cache, row, !apTranExt.UsrSegPricingFlag.Value);
            PXUIFieldAttribute.SetEnabled<APTranExt.usrValuationType>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTranExt.usrTotalAccumulateAmt>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTranExt.usrTotalAccumulateQty>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTranExt.usrAccumulateAmt>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTranExt.usrAccumulateQty>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTranExt.usrSegPricingFlag>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTranExt.usrSegPricingFlag>(cache, row, false);
            PXUIFieldAttribute.SetEnabled<APTran.curyUnitCost>(cache, row, false);
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(doc);

            if (apRegisterExt.UsrPONbr == null)
            {
                Base.Document.Cache.SetValueExt<APRegisterExt.usrPONbr>(Base.Document.Current, getPoNbr());
                Base.Document.Cache.SetValueExt<APRegisterExt.usrPOOrderType>(Base.Document.Current, getPOOrderType());
            }
            //整體累積金額要經常重算20191003

            Dictionary<String, decimal> dic = getPoAcumAmtAndQty(row);
            if (ckeckUsrValuationTypeisNull(apTranExt))
            {
                apTranExt.UsrAccumulateQty = dic["qty"];
                apTranExt.UsrAccumulateAmt = dic["sum"];
            }
            if (APDocType.DebitAdj.Equals(row.TranType))
            {
                apTranExt.UsrTotalAccumulateAmt = apTranExt.UsrAccumulateAmt - row.CuryLineAmt;
                apTranExt.UsrTotalAccumulateQty = apTranExt.UsrAccumulateQty - row.Qty;
            }
            else
            {
                apTranExt.UsrTotalAccumulateAmt = apTranExt.UsrAccumulateAmt + row.CuryLineAmt;
                apTranExt.UsrTotalAccumulateQty = apTranExt.UsrAccumulateQty + row.Qty;
            }

            if (doc.Hold == true && "A".Equals(apTranExt.UsrValuationType))
            {
                PXUIFieldAttribute.SetEnabled<APTran.qty>(cache, row, false);
            }
            //PXUIFieldAttribute.SetEnabled<APTran.qty>(cache, row, false);
        }

        protected void _(Events.FieldVerifying<APTran.curyLineAmt> e, PXFieldVerifying baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            APTranExt tranExt = (e.Row as APTran).GetExtension<APTranExt>();

            // This field needs to be validated on modification and the invoice is a refund retainage.
            if (!string.IsNullOrEmpty(tranExt.UsrOrigRetainageDocType) & e.NewValue != null && (decimal)e.NewValue > (decimal)e.OldValue)
            {
                APRegisterExt regisExt = APInvoice.PK.Find(Base, tranExt.UsrOrigRetainageDocType, tranExt.UsrOrigRetainageRefNbr)?.GetExtension<APRegisterExt>();

                // APRegisterExt.UsrRetainageUnreleasedAmt is an unbound with formula field and needs to be calculated manually.
                decimal? retUnreleasedAmt = regisExt.UsrRetainageAmt - regisExt.UsrRetainageReleased.Value;

                if ((decimal)e.NewValue > retUnreleasedAmt)
                {
                    e.Cache.RaiseExceptionHandling<APTran.curyLineAmt>(e.Row, e.NewValue, new PXSetPropertyException(string.Format(Message.ExtCostCannotGtrRetUnreleased, retUnreleasedAmt)));
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<APTranExt.usrRetainagePct> e)
        {
            // According to Mantis [0012321] - (0025647) # 1
            if (e.NewValue != null && Base.Document.Current?.GetExtension<APRegisterExt>().UsrIsRetainageDoc == true)
            {
                e.Cache.SetValueExt<APTran.curyLineAmt>(e.Row, (int)e.NewValue * (e.Row as APTran).GetExtension<APTranExt>().UsrOrigRetUnreleasedAmt / 100m);
            }
        }
        #endregion

        //protected virtual void AdditionsFilter_ContractID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        //{
        //    APInvoice master = Base.Document.Current;
        //    AdditionsFilter row = (AdditionsFilter)e.Row;
        //}

        protected virtual void KGDeductionAPTran_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            if (!isReduction())
            {
                throw new PXException(Message.NoRededuction);
            }
        }

        protected virtual void _(Events.RowSelected<KGContractEvaluation> e)
        {
            createContractEval.SetEnabled(e.Row == null);
        }

        #region GVApGuiInvoiceRef
        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceRef, GVApGuiInvoiceRef.invoiceType> e)
        {
            GVApGuiInvoiceRef row = (GVApGuiInvoiceRef)e.Row;
            if (row == null) return;
            PXCache cache = GVApGuiInvoiceRefs.Cache;
            if (row.InvoiceType == GVList.GVGuiInvoiceType.INVOICE)
            {
                cache.SetValueExt<GVApGuiInvoiceRef.guiType>(row, GVList.GVGuiType.AP.GuiType_21);
                cache.SetValueExt<GVApGuiInvoiceRef.voucherCategory>(row, GVList.GVGuiVoucherCategory.TAXABLE);
                cache.SetValueExt<GVApGuiInvoiceRef.taxCode>(row, GVList.GVTaxCode.TAXABLE);
            }
            else if (row.InvoiceType == GVList.GVGuiInvoiceType.RECEIPT)
            {
                cache.SetValueExt<GVApGuiInvoiceRef.guiType>(row, GVList.GVGuiType.AP.GuiType_27);
                cache.SetValueExt<GVApGuiInvoiceRef.voucherCategory>(row, GVList.GVGuiVoucherCategory.OTHERCERTIFICATE);
                cache.SetValueExt<GVApGuiInvoiceRef.taxCode>(row, GVList.GVTaxCode.TAXABLE);
            }
            //e.Cache.SetDefaultExt<GVApGuiInvoiceRef.taxAmt>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceRef, GVApGuiInvoiceRef.salesAmt> e)
        {
            GVApGuiInvoiceRef row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiInvoiceRef.taxAmt>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceRef, GVApGuiInvoiceRef.taxAmt> e)
        {
            GVApGuiInvoiceRef row = e.Row;
            if (row == null) return;
            decimal taxRate = row.TaxCode == GVList.GVTaxCode.TAXABLE ? 0.05m : 0m;
            e.NewValue = round((row.SalesAmt ?? 0) * taxRate);
        }
        #endregion
        #endregion

        #region CacheAttached
        [APActiveProjectAttibute]
        [ProjectDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        //20200130 
        //[ProjectBaseExt]
        protected void APInvoice_ProjectID_CacheAttached(PXCache sender) { }
        #endregion

        #region  Persist
        public virtual void KGValuationDetail_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }
        
        public virtual void KGValuationDetailDeduction_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }

        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            APInvoice master = Base.Document.Current;
            //代表刪除
            if (master == null)
            {
                baseMethod();
            }
            else
            {
                if (!beforeSaveCheck())
                {
                    throw new PXException("Error: Updating record raised at least one error. Please review the errors.");
                }

                PopupReminderDialog(master);

                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    //電子發票 塞日期
                    foreach (GVApGuiInvoiceRef gvApGuiInvoiceRef in GVApGuiInvoiceRefs.Select())
                    {
                        if (gvApGuiInvoiceRef.DeclareYear == null && gvApGuiInvoiceRef.InvoiceDate != null)
                        {
                            gvApGuiInvoiceRef.DeclareYear = ((DateTime)gvApGuiInvoiceRef.InvoiceDate).Year;

                        }
                        if (gvApGuiInvoiceRef.DeclareMonth == null && gvApGuiInvoiceRef.InvoiceDate != null)
                        {
                            gvApGuiInvoiceRef.DeclareMonth = ((DateTime)gvApGuiInvoiceRef.InvoiceDate).Month;
                        }
                        if (gvApGuiInvoiceRef.RegistrationCD == null)
                        {
                            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, Base.Accessinfo.BranchID);
                            GVRegistrationBranch gvRegistrationBranch = PXSelect<GVRegistrationBranch, Where<GVRegistrationBranch.bAccountID, Equal<Required<GVRegistrationBranch.bAccountID>>>>.Select(Base, branch.BAccountID);
                            if (gvRegistrationBranch != null)
                            {
                                GVRegistration gvRegistration = PXSelect<GVRegistration, Where<GVRegistration.registrationID, Equal<Required<GVRegistration.registrationID>>>>.Select(Base, gvRegistrationBranch.RegistrationID);
                                gvApGuiInvoiceRef.RegistrationCD = gvRegistration.RegistrationCD;
                            }
                        }
                    }

                    //if (APDocType.Invoice.Equals(master.DocType)) {
                    
                    if (PXEntryStatus.Inserted == Base.Document.Cache.GetStatus(master))
                    {
                        Base.Document.Cache.SetValue<APRegisterExt.usrValuationPhase>(master, APInvoiceValuationPhaseUtil.GetUsrValuationPhase(Base));
                        //設定BillDateFromTo
                        setBillDateFromTo();
                    }

                    
                    /*---2021-08-02:12179 
                    //付款金額及付款日期
                    bool checkPaymentAmount = true;
                    foreach (KGBillPayment kgBillPayment in KGBillPaym.Select())
                    {
                        if (kgBillPayment.PaymentAmount == null || kgBillPayment.PaymentAmount == 0)
                        {
                            checkPaymentAmount = false;
                        }
                    }
                    if (!checkPaymentAmount)
                    {
                        setPayment();
                    }*/
                    //setPayment();
                    //UsrRevDeductionNbr
                    //

                    //刪除 連動的KGValuationDetail
                    foreach (APTran apTran in Base.Transactions.Cache.Deleted)
                    {
                        APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                        if (APDocType.Invoice.Equals(master.DocType) && "A".Equals(apTranExt.UsrValuationType))
                        {
                            PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                           Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                           Set<KGValuationDetailDeduction.status, Required<KGValuationDetailDeduction.status>>>>,
                               KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                               And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                               >>.Update(Base, null, null, "V", apTranExt.UsrValuationID, "A");
                        }
                        else if (APDocType.DebitAdj.Equals(master.DocType) && "D".Equals(apTranExt.UsrValuationType))
                        {
                            string revDeductionNbr = PXCache<APRegister>.GetExtension<APRegisterExt>(Base.Document.Current).UsrRevDeductionNbr;

                            if (revDeductionNbr == null)
                            {
                                revDeductionNbr = master.RefNbr;
                            }
                            PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                               Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                               Set<KGValuationDetail.status, Required<KGValuationDetail.status>>>>,
                                   KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                                   And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                                   >>.Update(Base, null, null, "V", apTranExt.UsrValuationID, "D");
                        }
                    }
                    //假設扣款刪除也要回壓
                    foreach (KGDeductionAPTran dedLine in this.deductionAPTranDetails.Cache.Deleted)
                    {
                        //2021/06/17 Add 刪除若是票貼 也要把aPInvoiceNbr/aPInvoiceDate設成空
                        if (dedLine.ValuationType == "C")
                        {
                            PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                            Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                            Set<KGValuationDetail.status, Required<KGValuationDetail.status>>>>,
                                KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                                And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                                >>.Update(Base, null, null, "C", dedLine.ValuationID, "D");
                        }
                        else if (dedLine.IsManageFee == false)
                        {
                            PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                            Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                            Set<KGValuationDetail.status, Required<KGValuationDetail.status>>>>,
                                KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                                And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                                >>.Update(Base, null, null, "V", dedLine.ValuationID, "D");
                        }
                    }
                    //2020/09/11 Althea Add 刪除發票關聯
                    foreach (APRegister register in Base.Document.Cache.Deleted)
                    {
                        //一張計價單底下有可能為多張發票
                        foreach (GVApGuiInvoiceRef invoice in this.GVApGuiInvoiceRefs.Select())
                        {
                            //2020/10/08 直接把GVAPGuiInovice 刪掉
                            GVApGuiInvoiceRefs.Delete(invoice);
                        }
                    }
                    //刪除 連動的KGValuationDetail End

                    baseMethod();
                    //throw new Exception();
                    foreach (APTran apTran in Base.Transactions.Select())
                    {
                        APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                        if (APDocType.Invoice.Equals(master.DocType) && "A".Equals(apTranExt.UsrValuationType))
                        {
                            PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                           Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                           Set<KGValuationDetailDeduction.status, Required<KGValuationDetailDeduction.status>>>>,
                               KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                               And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                               >>.Update(Base, master.RefNbr, master.DocDate, "B", apTranExt.UsrValuationID, "A");
                        }

                        //這段程式代表 反置時間回寫扣款
                        /*
                        else if (APDocType.DebitAdj.Equals(master.DocType) && "D".Equals(apTranExt.UsrValuationType))
                        {
                            String revDeductionNbr = apRegisterExt.UsrRevDeductionNbr;
                            if (revDeductionNbr == null)
                            {
                                revDeductionNbr = master.RefNbr;
                            }
                            PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                               Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                               Set<KGValuationDetail.status, Required<KGValuationDetail.status>>>>,
                                   KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                                   And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                                   >>.Update(Base, revDeductionNbr, master.DocDate, "B", apTranExt.UsrValuationID, "D");
                        }*/
                    }
                    //20191018 By Jerry
                    if (APDocType.Invoice.Equals(master.DocType))
                    {
                        foreach (KGDeductionAPTran row in deductionAPTranDetails.Select())
                        {
                            //2021/06/17 Mantis: 0012096
                            if (row.ValuationType == "C" || row.IsManageFee == false)
                            {
                                PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                               Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                               Set<KGValuationDetail.status, Required<KGValuationDetail.status>>>>,
                                   KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                                   And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                                   >>.Update(Base, master.RefNbr, master.DocDate, "B", row.ValuationID, "D");
                            }
                        }
                    }
                    // Per YJ's request to add a feature.
                    AddSpecificDeductionTrans();

                    // mantis [0012321] - 0025758 #1
                    List<KGBillPayment> lists = KGBillPaym.Cache.Cached.RowCast<KGBillPayment>().ToList();

                    if (master.DocType == APDocType.DebitAdj && KGBillPaym.Select().Count <= 0)
                    {
                        for (int i = 0; i < lists.Count; i++)
                        {
                            KGBillPayment origBillPaym = lists[i];
                            KGBillPayment newBillPaym  = new KGBillPayment();

                            newBillPaym = KGBillPaym.Insert(new KGBillPayment()/* { DocType = master.DocType, RefNbr = master.RefNbr }*/);

                            newBillPaym.ActPayAmt     = origBillPaym.ActPayAmt;
                            newBillPaym.BankAccountID = origBillPaym.BankAccountID;
                            newBillPaym.CheckTitle    = origBillPaym.CheckTitle;
                            newBillPaym.IsPaymentHold = origBillPaym.IsPaymentHold;
                            newBillPaym.IsPostageFree = origBillPaym.IsPostageFree;
                            newBillPaym.PaymentAmount = origBillPaym.PaymentAmount;
                            newBillPaym.PaymentDate   = origBillPaym.PaymentDate;
                            newBillPaym.PaymentMethod = origBillPaym.PaymentMethod;
                            newBillPaym.PaymentPct    = origBillPaym.PaymentPct;
                            newBillPaym.PaymentPeriod = origBillPaym.PaymentPeriod;
                            newBillPaym.PostageAmt    = origBillPaym.PostageAmt;
                            newBillPaym.PricingType   = origBillPaym.PricingType;
                            newBillPaym.Remark        = origBillPaym.Remark;
                            newBillPaym.TtActDate     = origBillPaym.TtActDate;
                            newBillPaym.VendorID      = origBillPaym.VendorID;

                            KGBillPaym.Update(newBillPaym);
                        }
                        // call standard save function
                        baseMethod();
                    }

                    //createReverse();
                    ts.Complete();
                }
            }

            if (downCount == 0)
            {
                downCount++;
                if (!"AP301000".Equals(PXSiteMap.CurrentScreenID)) return;
                if (master == null || Base.Document.Cache.GetStatus(master) == PXEntryStatus.Deleted) return;
                if (APDocStatus.PendingApproval.Equals(master.Status))
                {
                    APInvoiceEntryAgentFlow entry = PXGraph.CreateInstance<APInvoiceEntryAgentFlow>();
                    entry.Document.Current = master;
                    entry.KGFlowAccs.Current = entry.KGFlowAccs.Select();
                    string message = entry.AgentFlow(master, entry.KGFlowAccs.Current, entry.GetEPApprovalByPending(master.NoteID));
                    if (message != null) throw new PXException(message);
                }
            }
        }

        public int? getUsrValuationPhase() => APInvoiceValuationPhaseUtil.GetUsrValuationPhase(Base);

        public bool beforeSaveCheck()
        {
            bool check = true;
            APInvoice master = Base.Document.Current;
            //modify by louis for bill without project
            if ("PO301000".Equals(master.CreatedByScreenID) || "SC301000".Equals(master.CreatedByScreenID))
            {
                if (master.ProjectID != null && master.ProjectID != 0)
                {
                    if (APDocType.Invoice.Equals(master.DocType))
                    {
                        if (getPoNbr() == null)
                        {
                            throw new PXException(Message.MustHavePoNbrError);
                        }
                    }

                    //add by louis for 借方調整不檢查前期是否已經balance
                    if (PXEntryStatus.Inserted == Base.Document.Cache.GetStatus(master)
                        && APDocType.Invoice.Equals(master.DocType)
                        && master.IsRetainageDocument != true)
                    //if (PXEntryStatus.Inserted == Base.Document.Cache.GetStatus(master))
                    {
                        bool ckeckPrevStatus = ckeckPrevStatusIsOpen(master.VendorID, master.DocDate, master.ProjectID, master.RefNbr, getPoNbr());
                        if (ckeckPrevStatus == false)
                        {
                            check = ckeckPrevStatus;
                            throw new PXException(Message.NotYetError);
                        }
                    }
                    //----------Check Segment Pricing------------------------
                    check = check && ckeckAllPricing();
                    foreach (APTran apTran in Base.Transactions.Select())
                    {
                        APTranExt apLineExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                        //Check Segment Pricing
                        PXResultset<KGContractSegPricing> set = PXSelect<KGContractSegPricing, Where<KGContractSegPricing.orderNbr, Equal<Current<APTran.pONbr>>,
                                              And<KGContractSegPricing.lineNbr, Equal<Required<APTran.pOLineNbr>>,
                                              And<KGContractSegPricing.orderType, Equal<Required<APTran.pOOrderType>>>>>>.Select(Base, apTran.POLineNbr, apTran.POOrderType);
                        if (set.Count > 0)
                        {
                            PXResultset<KGBillSegPricing> kgContractSegPricings = PXSelect<KGBillSegPricing, Where<KGBillSegPricing.aPRefNbr,
                            Equal<Current<APInvoice.refNbr>>, And<KGBillSegPricing.aPTranID, Equal<Required<APTran.tranID>>>>>.Select(Base, apTran.TranID);
                            foreach (KGBillSegPricing kgBillSegPricing in kgContractSegPricings)
                            {
                                if (kgBillSegPricing.BillQty < 0 || kgBillSegPricing.BillQty == null)
                                {
                                    segmentList.Cache.RaiseExceptionHandling<KGBillSegPricing.billQty>(
                                         kgBillSegPricing, kgBillSegPricing.BillQty, new PXSetPropertyException(Message.PoQtyNullAndNagativeError));
                                    Base.Transactions.Cache.RaiseExceptionHandling<APTran.qty>(
                                     apTran, apTran.Qty, new PXSetPropertyException(Message.PoQtyNullAndNagativeError));

                                    check = false;
                                }
                            }
                        }

                        //檢查合約的累計數量不能超過合約的總數量
                        if (APDocType.Invoice.Equals(master.DocType))
                        {
                            if (!APDocStatus.Balanced.Equals(master.Status) && !APDocStatus.Open.Equals(master.Status))
                            {
                                if (apTran.PONbr != null && apTran.POOrderType != null)
                                {
                                    POLine poLine = getPOLine(apTran.POOrderType, apTran.PONbr, apTran.POLineNbr);
                                    if (poLine != null)
                                    {

                                        //非一式
                                        if (!SubcontractEntry_Extension.type.Equals(apTran.UOM))
                                        {
                                            POLineContractExt lineExt = PXCache<POLine>.GetExtension<POLineContractExt>(poLine);
                                            PMCostBudget pmCostBudget = null;
                                            if (apTran.CostCodeID != null && apTran.TaskID != null && apTran.ProjectID != null && apTran.ProjectID != null && lineExt.UsrAccountGroupID != null)
                                            {
                                                pmCostBudget = RQRequestEntry_Extension.getPMCostBudget(apTran.CostCodeID.Value, apTran.TaskID.Value,
                                                                         apTran.ProjectID.Value, apTran.InventoryID.Value,
                                                                         lineExt.UsrAccountGroupID.Value);
                                            }
                                            //一式不處理 等同於預算的一式
                                            if (pmCostBudget != null && SubcontractEntry_Extension.type.Equals(pmCostBudget.UOM))
                                            {

                                            }
                                            else
                                            {
                                                if (apLineExt.UsrTotalAccumulateQty > poLine.OrderQty)
                                                {
                                                    Base.Transactions.Cache.RaiseExceptionHandling<APTran.qty>(
                                                        apTran, apTran.Qty, new PXSetPropertyException(Message.APTranQtyError));
                                                    check = false;
                                                }
                                            }
                                        }
                                        //都處理

                                        if (apLineExt.UsrTotalAccumulateAmt > poLine.CuryLineAmt)
                                        {
                                            Base.Transactions.Cache.RaiseExceptionHandling<APTran.curyLineAmt>(
                                                apTran, apTran.CuryLineAmt, new PXSetPropertyException(Message.APTranAmtError));
                                            check = false;
                                        }
                                        //XX
                                    }
                                }
                            }
                        }

                        //2020/06/29 Add CuryUnitCost Check by althea
                        if (!checkCuryUnitCost(apTran))
                        {
                            check = false;
                        }
                    }
                    //-----------Check Segment Pricing End----------------------
                    //-----------Check AP GUI INVOICE END-----------------------
                    APRegisterExt aPRegExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);

                    // YJ's request
                    if (aPRegExt.UsrPOOrderType == POOrderType.RegularOrder)
                    {
                        if (master.DocType == APDocType.Prepayment &&
                            master.CuryLineTotal > GetContractPricingRule(aPRegExt.UsrPOOrderType, aPRegExt.UsrPONbr).PrepaymentsAmtUntaxed)
                        {
                            throw new PXException(Message.InvPrepayAmtCannotGreaterPOPrepayAmt);
                        }
                        // Louis's request
                        if (GetContractDoc(aPRegExt.UsrPOOrderType, aPRegExt.UsrPONbr) == null)
                        {
                            throw new PXException(Message.ContEvaluationDidNotFilled);
                        }
                    }

                    //20220324 保留款金額檢核
                    if (aPRegExt != null && aPRegExt.UsrRetainagePct != null)
                    {
                        // Mantis [0012321] #2
                        if (aPRegExt.UsrRetainagePct > 0m)
                        {
                            decimal calcAmt = CalcRetainageAmt(Base.Document.Cache, master);

                            string errorMsg = aPRegExt.UsrRetainageAmt < calcAmt ? $"保留款金額需大於等於保留款預設比例的金額 {calcAmt}。" :
                                                                                   aPRegExt.UsrRetainageAmt > aPRegExt.UsrTotalRetainageAmt ? Message.RetainageAmtCannotGreaterInvAmt : null;

                            if (!string.IsNullOrEmpty(errorMsg))
                            {
                                Base.Document.Cache.RaiseExceptionHandling<APRegisterExt.usrRetainageAmt>(master, aPRegExt.UsrRetainageAmt,
                                                                                                          new PXSetPropertyException(errorMsg));
                            }
                        }

                    }
                }
            }

            ///<remarks> To avoid overcomplicated calculation logic, we do not allow users to reverse invoice with refund reservations.</remarks>
            if (APInvoice.PK.Find(Base, master.OrigDocType, master.OrigRefNbr)?.GetExtension<APRegisterExt>().UsrRetainageReleased > 0 && master.DocType == APDocType.DebitAdj)
            {
                throw new PXException(Message.CannotReverseBillWithRefundRet);
            }

            return check;
        }

        //請注意先後順序呼叫之前要確定已經有期數，找到上一期的計價然後再處理
        public void setBillDateFromTo()
        {
            APInvoice master = Base.Document.Current;
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            if (APDocType.Invoice.Equals(master.DocType))
            {
                if (apRegisterExt.UsrValuationPhase == 0 || apRegisterExt.UsrValuationPhase == 1)
                {
                    setZeroOneValuationPhase(apRegisterExt);
                }
                else
                {
                    APInvoice lastInvoice = getLastAPInvoiceByAPTran(master, getPoNbr());
                    if (lastInvoice != null)
                    {
                        APRegisterExt lastApRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(lastInvoice);
                        DateTime? tomorrow = null;
                        if (lastApRegisterExt.UsrBillDateTo != null)
                        {
                            DateTime UsrBillDateFrom = (DateTime)lastApRegisterExt.UsrBillDateTo;
                            tomorrow = UsrBillDateFrom.AddDays(1);
                        }
                        apRegisterExt.UsrBillDateFrom = tomorrow;
                        apRegisterExt.UsrBillDateTo = master.DocDate;
                    }
                }
            }
            else if (APDocType.Prepayment.Equals(master.DocType))
            {
                //預付款期別絕對是0或1
                setZeroOneValuationPhase(apRegisterExt);

            }
            else if (APDocType.DebitAdj.Equals(master.DocType))
            {
                PXGraph graph = new PXGraph();
                APInvoice lastInvoice = PXSelect<APInvoice, Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>,
                                    And<APInvoice.docType, Equal<APDocType.invoice>>>>.Select(graph, apRegisterExt.UsrRevDeductionNbr);
                if (lastInvoice != null)
                {
                    APRegisterExt lastApRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(lastInvoice);
                    apRegisterExt.UsrBillDateFrom = lastApRegisterExt.UsrBillDateFrom;
                    apRegisterExt.UsrBillDateTo = lastApRegisterExt.UsrBillDateTo;
                }
            }
            Base.Document.Update(master);
        }

        protected virtual void APInvoice_DueDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            APInvoice master = (APInvoice)e.Row;
            foreach (KGBillPayment kgBillPayment in KGBillPaym.Select())
            {
                //2021-07-15 : 12147 PricingType='其他-不重計付款日'不更新PaymentDate
                if (master.DueDate != null && kgBillPayment.PaymentPeriod != null && kgBillPayment.PricingType == PricingType.A)
                {
                    kgBillPayment.PaymentDate = ((DateTime)master.DueDate).AddDays((Double)kgBillPayment.PaymentPeriod);
                    KGBillPaym.Update(kgBillPayment);
                }
            }
        }

        protected virtual void KGBillPayment_PaymentPeriod_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            APInvoice master = Base.Document.Current;
            KGBillPayment kgBillPayment = (KGBillPayment)e.Row;
            if (master.DueDate != null && kgBillPayment.PaymentPeriod != null && kgBillPayment.PricingType == PricingType.A)
            {
                kgBillPayment.PaymentDate = ((DateTime)master.DueDate).AddDays((Double)kgBillPayment.PaymentPeriod);
            }
        }

        public virtual void _(Events.RowSelected<KGBillPayment> e)
        {
            KGBillPayment row = e.Row;
            if (row == null) return;
            PXUIFieldAttribute.SetEnabled<KGBillPayment.paymentDate>(e.Cache, row, row.PricingType == PricingType.B);
        }

        protected virtual void _(Events.FieldUpdated<KGBillPayment, KGBillPayment.pricingType> e)
        {
            APInvoice master = Base.Document.Current;
            KGBillPayment kgBillPayment = e.Row;
            if (master?.DueDate != null && kgBillPayment.PaymentPeriod != null && kgBillPayment.PricingType == PricingType.A)
            {
                kgBillPayment.PaymentDate = ((DateTime)master.DueDate).AddDays((Double)kgBillPayment.PaymentPeriod);
            }
        }

        protected virtual void _(Events.FieldUpdated<KGBillPayment, KGBillPayment.paymentPct> e)
        {
            KGBillPayment row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<KGBillPayment.paymentAmount>(row);
        }

        protected virtual void _(Events.FieldDefaulting<KGBillPayment, KGBillPayment.paymentAmount> e)
        {
            KGBillPayment row = e.Row;
            KGBillSummary sum = getKGBillSummary();
            if (row == null) return;
            e.NewValue = round((sum?.TotalAmt ?? 0m) * (row.PaymentPct ?? 0m) / 100);
        }

        //2021/12/17 Althea Delete 刪掉煩人的連動
        
        protected virtual void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.totalAmt> e)
        {
            KGBillSummary row = e.Row;
            //if (row == null && e.OldValue==e.NewValue) return;
            //20220317 modify by louis 修正因為decimal小數點不一致造成比較錯誤
            if (row == null || Convert.ToDecimal(e.OldValue) == Convert.ToDecimal(e.NewValue)) return;

            foreach (KGBillPayment item in KGBillPaym.Select())
            {
                KGBillPaym.Cache.SetDefaultExt<KGBillPayment.paymentAmount>(item);
                KGBillPaym.Update(item);
            }
        }
        
        public Decimal? round(Decimal? num)
        {
            if (num == null)
            {
                return 0;
            }
            return System.Math.Round((Decimal)getValue(num), 0, MidpointRounding.AwayFromZero);
        }

        public APRegisterExt setZeroOneValuationPhase(APRegisterExt apRegisterExt)
        {
            POOrder pOOrder = getCurrentPOOrder();
            APInvoice master = Base.Document.Current;
            if (pOOrder != null)
            {
                apRegisterExt.UsrBillDateFrom = pOOrder.OrderDate;
                apRegisterExt.UsrBillDateTo = master.DocDate;
            }
            return apRegisterExt;
        }
        #endregion

        #region Contract Evalation
        /// <summary>
        /// Create a virtual action button to pop up new form and bring default value.
        /// </summary>
        public PXAction<APInvoice> createContractEval;
        [PXUIField(DisplayName = "Add Contract Evaluation")]
        [PXButton(CommitChanges = true, OnClosingPopup = PXSpecialButtonType.Save)]
        protected virtual void CreateContractEval()
        {
            APRegister register = Base.Document.Current;
            APRegisterExt regisExt = register.GetExtension<APRegisterExt>();

            //2020/12/17 Add By Althea
            //跳出廠商評鑑邏輯
            //計價(類型為INV)存檔時, 狀態不為"Balance", "OPEN", 'CLOSE'. 
            //請不是在執行反轉,刪除時, 請檢查估驗計價比例. 
            //如果比例達到或是超過第一次評鑑時機, 且尚未填寫過第一次評鑑時, 請讓使用者填寫評鑑問卷. 
            //如果達到第二次評鑑時機, 請讓使用者填寫第二次評鑑問卷.
            //2021-07-14 add by Alton
            //保留款退回不做廠商評鑑
            bool isRetainageDocument = register.IsRetainageDocument ?? false == true;

            if (register.DocType == APDocType.Invoice && isRetainageDocument != true && Base.Document.Cache.GetStatus(register) != PXEntryStatus.Deleted)
            {
                // 當使用者成功儲存計價單(AP301000)後，請檢查計價累計比例，若大於或等於分包契約設定的評鑑時機(90%), 且該合約尚無”廠商評鑑”，
                // 該合約尚無”廠商評鑑”的檢查是到KGContractEvaluation找不到該PO號碼(KGContractEvaluation.ordernbr)。由於扣款不加總因此這行有可能不跑到

                //20201221 Edit by Alton ：狀態為Balanced不處理...避免automation無法觸發
                if (SummaryAmtFilters.Current != null && register.Status != APDocStatus.Balanced
                    && register.Status != APDocStatus.Open && register.Status != APDocStatus.Closed)
                {
                    KGContractDoc contractDoc = PXSelect<KGContractDoc, Where<KGContractDoc.orderNbr, Equal<Required<APRegisterExt.usrPONbr>>>>.Select(Base, regisExt.UsrPONbr);

                    if (contractDoc != null)
                    {
                        int  times = 0;

                        //2021/05/10 Mantis: 0011827 by Althea
                        if ((SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.EvaluationScore ||
                            (SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.EvaluationScore &&
                            SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.SecEvaluationScore)) &&
                            ContractEvalExisted(regisExt.UsrPOOrderType, regisExt.UsrPONbr) == 0 &&
                            regisExt.UsrPOOrderType == POOrderType.RegularOrder)
                        {
                            times = 1;
                        }
                        else if (SummaryAmtFilters.Current.PoValuationPercent >= contractDoc.SecEvaluationScore &&
                                 ContractEvalExisted(regisExt.UsrPOOrderType, regisExt.UsrPONbr) == 1 &&
                                 //add by louis 20210729 一次計價只做一次評鑑
                                 APContractEvalExisted(regisExt.UsrPOOrderType, regisExt.UsrPONbr, register.RefNbr) <= 0 &&
                                 PXCache<APRegister>.GetExtension<APRegisterExt>(Base.CurrentDocument.Current).UsrPOOrderType == POOrderType.RegularOrder)
                        {
                            times = 2;
                        }

                        if (times > 0)
                        {
                            KGContractEvalEntry target = PXGraph.CreateInstance<KGContractEvalEntry>();

                            CreateContractEvalLine(ref target, times);

                            PXRedirectHelper.TryRedirect(target, PXRedirectHelper.WindowMode.Popup);
                            //throw new PXPopupRedirectException(target, string.Empty, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When the form is opening, then insert default key values.
        /// </summary>
        /// <param name="graph"></param>
        private void CreateContractEvalLine(ref KGContractEvalEntry graph, int times)
        {
            APTran aPTran = Base.Transactions.Current;
            KGContractDoc contractDoc = GetContractDoc(aPTran.POOrderType, aPTran.PONbr);
            
            //當contractDoc為空的時候不做下面的事情
            if (contractDoc == null) return;

            graph.ContractEval.Insert();

            graph.ContractEval.SetValueExt<KGContractEvaluation.evaluationID>(graph.ContractEval.Current, contractDoc?.EvaluationID);
            graph.ContractEval.SetValueExt<KGContractEvaluation.orderNbr>(graph.ContractEval.Current, aPTran.PONbr);
            graph.ContractEval.SetValueExt<KGContractEvaluation.orderType>(graph.ContractEval.Current, aPTran.POOrderType);
            graph.ContractEval.SetValueExt<KGContractEvaluation.aPRefNbr>(graph.ContractEval.Current, aPTran.RefNbr);
            graph.ContractEval.SetValueExt<KGContractEvaluation.aPDocType>(graph.ContractEval.Current, aPTran.TranType);
            graph.ContractEval.SetValueExt<KGContractEvaluation.vendorID>(graph.ContractEval.Current, Base.CurrentDocument.Current.VendorID);
            graph.ContractEval.SetValueExt<KGContractEvaluation.evalPhase>(graph.ContractEval.Current, times);

            KGContractEvaluationL contractEvalL = graph.ContractEvalLine.Cache.CreateInstance() as KGContractEvaluationL;

            foreach (KGVendorEvaluationQuest row in PXSelectReadonly<KGVendorEvaluationQuest, Where<KGVendorEvaluationQuest.evaluationID, Equal<Required<KGContractEvaluation.evaluationID>>>>
                                                                     .Select(Base, contractDoc?.EvaluationID))
            {
                contractEvalL.QuestSeq    = row.QuestSeq;
                contractEvalL.Quest       = row.Quest;
                contractEvalL.WeightScore = row.Score;

                graph.ContractEvalLine.Cache.Insert(contractEvalL);
            }
        }

        /// <summary>
        /// Check the PO has completed contract evaluation.
        /// </summary>
        /// <returns></returns>
        public int ContractEvalExisted(string pOOrderType, string pONbr)
        {
            var row = PXSelectReadonly<KGContractEvaluation,
                                       Where<KGContractEvaluation.orderType, Equal<Required<APTran.pOOrderType>>,
                                             And<KGContractEvaluation.orderNbr, Equal<Required<APTran.pONbr>>>>>
                                       .Select(Base, pOOrderType, pONbr);
            return row.Count;
        }

        public int APContractEvalExisted(string pOOrderType, string pONbr, string apRefNbr)
        {
            var row = PXSelectReadonly<KGContractEvaluation,
                                       Where<KGContractEvaluation.orderType, Equal<Required<APTran.pOOrderType>>,
                                             And<KGContractEvaluation.orderNbr, Equal<Required<APTran.pONbr>>,
                                             And<KGContractEvaluation.aPRefNbr, Equal<Required<APTran.refNbr>>
                                             >>>>
                                       .Select(Base, pOOrderType, pONbr, apRefNbr);
            return row.Count;
        }

        #region hyperLink
        public PXAction<APInvoice> ViewContractEval;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewContractEval()
        {
            APInvoice aPInvoice = Base.Document.Current;
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(aPInvoice);
            KGContractEvaluation evaluation = ContractEvaluation.Current;
            KGContractEvalEntry graph = PXGraph.CreateInstance<KGContractEvalEntry>();
            graph.ContractEval.Current = PXSelect<KGContractEvaluation,
                                                            Where<KGContractEvaluation.contractEvaluationCD, Equal<Required<KGContractEvaluation.contractEvaluationCD>>>>
                                                            .Select(Base, evaluation.ContractEvaluationCD);
            if (graph.ContractEval.Current != null)
            {
                //throw new PXRedirectRequiredException(graph, "Contract Eval")
                //{
                //    Mode = PXBaseRedirectException.WindowMode.NewWindow
                //};
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.Popup);
            }
        }
        #endregion
        #endregion

        #region 保留款
        public PXAction<APRetainageFilter> viewRetainageDoc;
        [PXButton]
        public virtual IEnumerable ViewRetainageDoc(PXAdapter adapter)
        {
            if (retainageList.Current != null)
            {
                PXRedirectHelper.TryRedirect(retainageList.Cache, retainageList.Current, "Document", PXRedirectHelper.WindowMode.NewWindow);
            }
            return adapter.Get();
        }
        #endregion

        #region 差旅費
        public PXAction<APInvoice> travelExpenses;
        [PXUIField(
            DisplayName = "openEngTravleAPEForm")]
        [PXLookupButton(CommitChanges = true)]

        public virtual IEnumerable TravelExpenses(PXAdapter adapter)
        {
            APInvoice master = Base.Document.Current;
            if (master.RefNbr == null)
            {
                throw new PXException("請先保存");
            }
            String tempMessage = null;
            String status = null;
            if (master.Hold == true)
            {
                AgentFlowWebServiceUtil.createOpenEngTravleAPEForm(Base.Accessinfo.UserName, master.RefNbr, "Y", master.ProjectID, ref status, ref tempMessage);
            }
            else
            {

                AgentFlowWebServiceUtil.createOpenEngTravleAPEForm(Base.Accessinfo.UserName, master.RefNbr, "N", master.ProjectID, ref status, ref tempMessage);
            }

            if (tempMessage != null)
            {
                throw new PXException(tempMessage);
            }

            if (status.StartsWith("http") == false)
            {
                throw new PXException("webService錯誤" + status);
            }
            else
            {
                throw new Exception("Redirect7:" + status);

            }




            return adapter.Get();
        }

        #endregion

        #region Reverse
        #region 扣款
        public virtual void ReverseDeductionProc(APRegister doc)
        {
            AR.DuplicateFilter dfilter = PXCache<AR.DuplicateFilter>.CreateCopy(Base.duplicatefilter.Current);
            WebDialogResult dialogRes = Base.duplicatefilter.View.Answer;

            Base.Clear(PXClearOption.PreserveTimeStamp);

            //Magic. We need to prevent rewriting of CurrencyInfo.IsReadOnly by true in CurrencyInfoView
            Base.CurrentDocument.Cache.AllowUpdate = true;

            foreach (PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res in APInvoice_CurrencyInfo_Terms_Vendor.Select(Base, (object)doc.DocType, doc.RefNbr, doc.VendorID))
            {
                CurrencyInfo info = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
                info.CuryInfoID = null;
                info.IsReadOnly = false;
                info = PXCache<CurrencyInfo>.CreateCopy(Base.currencyinfo.Insert(info));

                APInvoice origInvoice = res;

                APInvoice invoice = PXCache<APInvoice>.CreateCopy(origInvoice);
                invoice.CuryInfoID = info.CuryInfoID;
                invoice.DocType = APDocType.DebitAdj;
                invoice.RefNbr = null;

                //must set for _RowSelected
                invoice.OpenDoc = true;
                invoice.Released = false;

                Base.Document.Cache.SetDefaultExt<APInvoice.isMigratedRecord>(invoice);
                invoice.BatchNbr = null;
                invoice.PrebookBatchNbr = null;
                invoice.Prebooked = false;
                invoice.ScheduleID = null;
                invoice.Scheduled = false;
                invoice.NoteID = null;

                invoice.TermsID = null;
                invoice.InstallmentCntr = null;
                invoice.InstallmentNbr = null;
                invoice.DueDate = null;
                invoice.DiscDate = null;
                invoice.CuryOrigDiscAmt = 0m;
                invoice.FinPeriodID = doc.FinPeriodID;
                invoice.OrigDocDate = invoice.DocDate;

                //if (doc.IsChildRetainageDocument())
                //{
                //invoice.OrigDocType = doc.OrigDocType;
                //invoice.OrigRefNbr = doc.OrigRefNbr;
                //}
                //else
                //{
                #region 20201218 edit by Alton (11822: AP301000估驗計價APRegister新增三個欄位UsrIsDeductionDoc, UsrOriDeductionRefNbr, UsrOriDeductionDocType)
                //invoice.OrigDocType = doc.DocType;
                //invoice.OrigRefNbr = doc.RefNbr;
                APRegister register = invoice;
                Base.Document.Cache.SetValueExt<APRegisterExt.usrOriDeductionRefNbr>(register, doc.RefNbr);
                Base.Document.Cache.SetValueExt<APRegisterExt.usrOriDeductionDocType>(register, doc.DocType);
                Base.Document.Cache.SetValueExt<APRegisterExt.usrIsDeductionDoc>(register, true);
                #endregion
                //}

                invoice.PaySel = false;
                //PaySel does not affect these fields
                //invoice.PayTypeID = null;
                //invoice.PayDate = null;
                //invoice.PayAccountID = null;
                //invoice.CuryDocBal = invoice.CuryOrigDocAmt;
                invoice.CuryOrigDocAmt = 0m;
                invoice.CuryLineTotal = 0m;
                invoice.IsTaxPosted = false;
                invoice.IsTaxValid = false;
                invoice.CuryVatTaxableTotal = 0m;
                invoice.CuryVatExemptTotal = 0m;
                invoice.PrebookAcctID = PXAccess.FeatureInstalled<FeaturesSet.prebooking>() ? origInvoice.PrebookAcctID : null;
                invoice.PrebookSubID = PXAccess.FeatureInstalled<FeaturesSet.prebooking>() ? origInvoice.PrebookSubID : null;
                invoice.Hold = (Base.apsetup.Current.HoldEntry ?? false) || Base.IsApprovalRequired(invoice, Base.Document.Cache);
                invoice.PendingPPD = false;

                Base.ClearRetainageSummary(invoice);
                using (var validationScope = new DisableSelectorValidationScope(Base.Document.Cache, typeof(APRegister.employeeID)))
                {
                    Base.Document.Cache.SetDefaultExt<APInvoice.employeeID>(invoice);
                    invoice = Base.Document.Insert(invoice);
                }

                if (invoice.RefNbr == null)
                {
                    //manual numbering, check for occasional duplicate
                    APInvoice duplicate = PXSelect<APInvoice>.Search<APInvoice.docType, APInvoice.refNbr>(Base, invoice.DocType, invoice.OrigRefNbr);
                    if (duplicate != null)
                    {
                        PXCache<AR.DuplicateFilter>.RestoreCopy(Base.duplicatefilter.Current, dfilter);
                        Base.duplicatefilter.View.Answer = dialogRes;
                        if (Base.duplicatefilter.AskExt() == WebDialogResult.OK)
                        {
                            Base.duplicatefilter.Cache.Clear();
                            if (Base.duplicatefilter.Current.RefNbr == null)
                                throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(AR.DuplicateFilter.refNbr).Name);
                            duplicate = PXSelect<APInvoice>.Search<APInvoice.docType, APInvoice.refNbr>(Base, invoice.DocType, Base.duplicatefilter.Current.RefNbr);
                            if (duplicate != null)
                                throw new PXException(ErrorMessages.RecordExists);
                            invoice.RefNbr = Base.duplicatefilter.Current.RefNbr;
                        }
                    }
                    else
                        invoice.RefNbr = invoice.OrigRefNbr;
                    Base.Document.Cache.Normalize();
                    invoice = Base.Document.Update(invoice);
                }

                if (info != null)
                {
                    CurrencyInfo b_info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<APInvoice.curyInfoID>>>>.Select(Base, null);
                    b_info.CuryID = info.CuryID;
                    b_info.CuryEffDate = info.CuryEffDate;
                    b_info.CuryRateTypeID = info.CuryRateTypeID;
                    b_info.CuryRate = info.CuryRate;
                    b_info.RecipRate = info.RecipRate;
                    b_info.CuryMultDiv = info.CuryMultDiv;
                    Base.currencyinfo.Update(b_info);
                }
            }

            TaxAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, TaxCalc.ManualCalc);
            // Add additional conditions to filter two special deduction types.
            foreach (KGDeductionAPTran srcTran in PXSelect<KGDeductionAPTran, Where<KGDeductionAPTran.refNbr, Equal<Required<KGDeductionAPTran.refNbr>>,
                                                                                    And<KGDeductionAPTran.inventoryID, NotEqual<Required<KGDeductionAPTran.inventoryID>>,
                                                                                        And<KGDeductionAPTran.inventoryID, NotEqual<Required<KGDeductionAPTran.inventoryID>>>>>>
                                                           .Select(Base, doc.RefNbr, KGSetup.Current.KGSupplementaryTaxInventoryID, KGSetup.Current.KGIndividualTaxInventoryID))
            {
                //TODO Create new APTran and explicitly fill the required fields
                APTran tran = new APTran();
                tran.TranType = null;
                tran.RefNbr = null;
                tran.LineNbr = srcTran.LineNbr;
                tran.SortOrder = srcTran.SortOrder;
                tran.BranchID = srcTran.BranchID;
                tran.TranID = null;
                //2019R2
                //tran.BatchNbr = srcTran.BatchNbr;
                tran.AccountID = srcTran.AccountID;
                tran.SubID = srcTran.SubID;
                tran.CuryInfoID = null;
                tran.CuryTranAmt = srcTran.CuryTranAmt;
                tran.TranAmt = srcTran.TranAmt;
                tran.CuryTaxableAmt = srcTran.CuryTaxableAmt;
                tran.TaxableAmt = srcTran.TaxableAmt;
                tran.CuryTaxAmt = srcTran.CuryTaxAmt;
                tran.TaxAmt = srcTran.TaxAmt;
                tran.CuryExpenseAmt = srcTran.CuryExpenseAmt;
                tran.ExpenseAmt = srcTran.ExpenseAmt;
                tran.Qty = srcTran.Qty;
                tran.BaseQty = srcTran.BaseQty;
                tran.CuryUnitCost = srcTran.CuryUnitCost;
                tran.ManualPrice = srcTran.ManualPrice;
                tran.ManualDisc = true;
                tran.CuryLineAmt = srcTran.CuryLineAmt;
                tran.LineAmt = srcTran.LineAmt;
                tran.DiscountsAppliedToLine = srcTran.DiscountsAppliedToLine;
                tran.OrigLineNbr = srcTran.LineNbr;
                tran.OrigGroupDiscountRate = srcTran.OrigGroupDiscountRate;
                tran.OrigDocumentDiscountRate = srcTran.OrigDocumentDiscountRate;
                tran.GroupDiscountRate = srcTran.GroupDiscountRate;
                tran.DocumentDiscountRate = srcTran.DocumentDiscountRate;
                tran.UnitCost = srcTran.UnitCost;
                tran.TranClass = srcTran.TranClass;
                tran.DrCr = null;
                tran.TranDate = srcTran.TranDate;
                tran.TranDesc = srcTran.TranDesc;
                tran.Released = null;
                tran.NonBillable = srcTran.NonBillable;
                tran.POOrderType = srcTran.POOrderType;
                tran.PONbr = srcTran.PONbr;
                tran.ReceiptType = srcTran.ReceiptType;
                tran.ReceiptNbr = srcTran.ReceiptNbr;
                tran.POLineNbr = srcTran.POLineNbr;
                tran.ReceiptLineNbr = srcTran.ReceiptLineNbr;
                tran.LCDocType = srcTran.LCDocType;
                tran.LCRefNbr = srcTran.LCRefNbr;
                tran.LCLineNbr = srcTran.LCLineNbr;
                tran.PPVDocType = null;
                tran.PPVRefNbr = null;
                tran.InventoryID = srcTran.InventoryID;
                tran.SiteID = srcTran.SiteID;
                tran.UOM = srcTran.UOM;
                tran.LineType = srcTran.LineType;
                tran.TaxCategoryID = srcTran.TaxCategoryID;
                tran.Box1099 = srcTran.Box1099;
                tran.FinPeriodID = srcTran.FinPeriodID;
                tran.DeferredCode = srcTran.DeferredCode;
                //tran.ScheduleID = srcTran.ScheduleID;
                tran.DefScheduleID = srcTran.DefScheduleID;
                tran.VendorID = srcTran.VendorID;
                tran.Date = srcTran.Date;
                tran.ProjectID = srcTran.ProjectID;
                tran.TaskID = srcTran.TaskID;
                tran.CostCodeID = srcTran.CostCodeID;
                //tran.LCTranID = srcTran.LCTranID;
                tran.LandedCostCodeID = srcTran.LandedCostCodeID;
                tran.TaxID = srcTran.TaxID;
                tran.DiscountID = srcTran.DiscountID;
                tran.DiscountSequenceID = srcTran.DiscountSequenceID;
                tran.DiscAmt = srcTran.DiscAmt;
                tran.CuryDiscAmt = srcTran.CuryDiscAmt;
                tran.DiscPct = srcTran.DiscPct;
                tran.POPPVAmt = 0m;
                tran.EmployeeID = srcTran.EmployeeID;
                tran.RetainagePct = srcTran.RetainagePct;
                tran.CuryRetainageAmt = srcTran.CuryRetainageAmt;
                tran.RetainageAmt = srcTran.RetainageAmt;
                tran.POAccrualType = srcTran.POAccrualType;
                tran.POAccrualRefNoteID = srcTran.POAccrualRefNoteID;
                tran.POAccrualLineNbr = srcTran.POAccrualLineNbr;
                tran.UnreceivedQty = srcTran.UnreceivedQty;
                tran.BaseUnreceivedQty = srcTran.BaseUnreceivedQty;
                Base.Transactions.Cache.SetValue<APTranExt.usrValuationID>(tran, srcTran.ValuationID);
                Base.Transactions.Cache.SetValue<APTranExt.usrValuationType>(tran, srcTran.ValuationType);
                string origDrCr = tran.DrCr;
                tran.NoteID = null;
                /*
                if (!string.IsNullOrEmpty(tran.DeferredCode))
                {
                    DRSchedule schedule = PXSelect<DRSchedule,
                        Where<DRSchedule.module, Equal<moduleAP>,
                        And<DRSchedule.docType, Equal<Required<DRSchedule.docType>>,
                        And<DRSchedule.refNbr, Equal<Required<DRSchedule.refNbr>>,
                        And<DRSchedule.lineNbr, Equal<Required<DRSchedule.lineNbr>>>>>>>.Select(this, doc.DocType, doc.RefNbr, tran.LineNbr);

                    if (schedule != null)
                    {
                        tran.DefScheduleID = schedule.ScheduleID;
                    }
                }
                */
                Decimal? curyTranAmt = tran.CuryTranAmt;
                APTran tranNew = Base.Transactions.Insert(tran);
                //PXNoteAttribute.CopyNoteAndFiles(Base.Transactions.Cache, srcTran, Base.Transactions.Cache, tranNew);

                if (tranNew != null && tranNew.CuryTranAmt != curyTranAmt)
                {
                    tranNew.CuryTranAmt = curyTranAmt;
                    tranNew = (APTran)Base.Transactions.Cache.Update(tranNew);
                }

                if (tranNew.LineType == SOLineType.Discount)
                {
                    tranNew.DrCr = (origDrCr == DrCr.Debit) ? DrCr.Credit : DrCr.Debit;
                    tranNew.FreezeManualDisc = true;
                    tranNew.TaxCategoryID = null;
                    Base.Transactions.Update(tranNew);
                }
            }

            foreach (APInvoiceDiscountDetail discountDetail in PXSelect<APInvoiceDiscountDetail, Where<APInvoiceDiscountDetail.docType, Equal<Required<APInvoice.docType>>, And<APInvoiceDiscountDetail.refNbr, Equal<Required<APInvoice.refNbr>>>>, OrderBy<Asc<APInvoiceDiscountDetail.docType, Asc<APInvoiceDiscountDetail.refNbr>>>>.Select(Base, doc.DocType, doc.RefNbr))
            {
                APInvoiceDiscountDetail newDiscountDetail = PXCache<APInvoiceDiscountDetail>.CreateCopy(discountDetail);

                newDiscountDetail.DocType = Base.Document.Current.DocType;
                newDiscountDetail.RefNbr = Base.Document.Current.RefNbr;
                newDiscountDetail.IsManual = true;
                _discountEngine.UpdateDiscountDetail(Base.DiscountDetails.Cache, Base.DiscountDetails, newDiscountDetail);
            }
            if (!Base.IsExternalTax(Base.Document.Current.TaxZoneID))
            {
                bool disableTaxCalculation = doc.PendingPPD == true && doc.DocType == APDocType.DebitAdj;
                Decimal? curyTaxableAmt = 0;
                //2021/05/24 稅額改成 SUM(CuryTaxableAmt)*稅率% 
                Decimal? curyTaxAmt = 0;
                foreach (KGDeductionAPTran srcTran in PXSelect<KGDeductionAPTran, Where<KGDeductionAPTran.refNbr, Equal<Required<KGDeductionAPTran.refNbr>>>>.Select(Base, doc.RefNbr))
                //foreach (APTaxTran tax in PXSelect<APTaxTran, Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>>>>.Select(Base, doc.DocType, doc.RefNbr))
                {
                    //add 2021-07-20 by Alton 12163 暫時寫死應稅才計算稅額(到時候大改透過原廠處理)
                    if (srcTran.TaxCategoryID == "應稅")
                    {
                        curyTaxableAmt = curyTaxableAmt + getValue(srcTran.CuryTaxableAmt);
                    }
                }
                curyTaxAmt = curyTaxAmt + Math.Round((curyTaxableAmt ?? 0) * 0.05m, MidpointRounding.AwayFromZero);

                //設定稅
                APTaxTran new_aptax = new APTaxTran();
                new_aptax.TaxID = "VAT05";
                //new_aptax.TaxID = "WH05";
                if (disableTaxCalculation)
                {
                    TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, TaxCalc.NoCalc);
                }
                new_aptax = Base.Taxes.Insert(new_aptax);

                if (new_aptax != null)
                {
                    new_aptax = PXCache<APTaxTran>.CreateCopy(new_aptax);
                    new_aptax.TaxRate = 5;
                    new_aptax.CuryTaxableAmt = curyTaxableAmt;
                    new_aptax.CuryTaxAmt = curyTaxAmt;
                    if (disableTaxCalculation)
                    {
                        TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, TaxCalc.ManualCalc);
                    }
                    //new_aptax.CuryRetainedTaxableAmt = tax.CuryRetainedTaxableAmt;
                    //new_aptax.CuryRetainedTaxAmt = tax.CuryRetainedTaxAmt;
                    new_aptax = Base.Taxes.Update(new_aptax);
                }
                else
                {
                    APTaxTran aptax = PXSelect<APTaxTran,
                    Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>,
                        And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>
                        , And<APTaxTran.taxID, Equal<Required<APTaxTran.taxID>>>>>>
                    .Select(Base, doc.DocType, doc.RefNbr, "VAT05");
                    aptax.TaxRate = 5;
                    aptax.CuryTaxableAmt = curyTaxableAmt;
                    aptax.CuryTaxAmt = curyTaxAmt;
                    if (disableTaxCalculation)
                    {
                        TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, TaxCalc.ManualCalc);
                    }
                    //new_aptax.CuryRetainedTaxableAmt = tax.CuryRetainedTaxableAmt;
                    //new_aptax.CuryRetainedTaxAmt = tax.CuryRetainedTaxAmt;
                    new_aptax = Base.Taxes.Update(aptax);
                }
            }
            //該程式碼不會累加金額TaxID相同狀況下無法Insert
            /*
            if (!Base.IsExternalTax(Base.Document.Current.TaxZoneID))
            {
                bool disableTaxCalculation = doc.PendingPPD == true && doc.DocType == APDocType.DebitAdj;
                foreach (KGDeductionAPTran srcTran in PXSelect<KGDeductionAPTran, Where<KGDeductionAPTran.refNbr, Equal<Required<KGDeductionAPTran.refNbr>>>>.Select(Base, doc.RefNbr))
                //foreach (APTaxTran tax in PXSelect<APTaxTran, Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>>>>.Select(Base, doc.DocType, doc.RefNbr))
                {
                    APTaxTran new_aptax = new APTaxTran();
                    new_aptax.TaxID = "VAT05";
                    if (disableTaxCalculation)
                    {
                        TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, TaxCalc.NoCalc);
                    }
                    new_aptax = Base.Taxes.Insert(new_aptax);

                    if (new_aptax != null)
                    {
                        new_aptax = PXCache<APTaxTran>.CreateCopy(new_aptax);
                        new_aptax.TaxRate = 5;
                        new_aptax.CuryTaxableAmt = srcTran.CuryTaxableAmt;
                        new_aptax.CuryTaxAmt = srcTran.CuryTaxAmt;
                        if (disableTaxCalculation)
                        {
                            TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, TaxCalc.ManualCalc);
                        }
                        //new_aptax.CuryRetainedTaxableAmt = tax.CuryRetainedTaxableAmt;
                        //new_aptax.CuryRetainedTaxAmt = tax.CuryRetainedTaxAmt;
                        new_aptax = Base.Taxes.Update(new_aptax);
                    }
                }
            }*/
        }

        public virtual void ReleaseRetainageProc(APInvoice doc, RetainageOptions retainageOpts, bool isAutoRelease = false)
        {
            Base.Clear(PXClearOption.PreserveTimeStamp);

            if (retainageOpts.CuryRetainageAmt <= 0 || retainageOpts.CuryRetainageAmt > doc.CuryRetainageUnreleasedAmt)
            {
                throw new PXException(Messages.IncorrectRetainageAmount);
            }

            // Magic. We need to prevent rewriting of CurrencyInfo.IsReadOnly 
            // by true in CurrencyInfoView
            // 
            this.Base.CurrentDocument.Cache.AllowUpdate = true;

            PXResult<APInvoice, CurrencyInfo, Terms, Vendor> resultDoc =
                APInvoice_CurrencyInfo_Terms_Vendor
                    .SelectSingleBound(this.Base, null, doc.DocType, doc.RefNbr, doc.VendorID)
                    .Cast<PXResult<APInvoice, CurrencyInfo, Terms, Vendor>>()
                    .First();

            CurrencyInfo info = resultDoc;
            APInvoice origInvoice = resultDoc;
            Vendor vendor = resultDoc;

            decimal retainagePercent = (decimal)(retainageOpts.CuryRetainageAmt / doc.CuryRetainageTotal);

            PXResultset<APTran> details = PXSelectGroupBy<APTran,
                Where<APTran.tranType, Equal<Required<APTran.tranType>>,
                    And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                    And<APTran.curyRetainageAmt, NotEqual<decimal0>>>>,
                Aggregate<
                    GroupBy<APTran.taxCategoryID,
                    Sum<APTran.curyRetainageAmt>>>>
                .Select(this.Base, doc.DocType, doc.RefNbr);

            APTran tranNew = null;
            decimal prevCuryTotal = 0m;

            TaxCalc oldTaxCalc = TaxBaseAttribute.GetTaxCalc<APTran.taxCategoryID>(this.Base.Transactions.Cache, null);
            TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(this.Base.Transactions.Cache, null, TaxCalc.ManualCalc);

            foreach (APTran detail in details)
            {

                tranNew = new APTran();
                tranNew.BranchID = this.Base.Document.Current.BranchID;
                tranNew.TaxCategoryID = detail.TaxCategoryID;
                tranNew.AccountID = this.Base.Document.Current.RetainageAcctID;
                tranNew.SubID = this.Base.Document.Current.RetainageSubID;
                tranNew.ProjectID = ProjectDefaultAttribute.NonProject();

                tranNew.Qty = 0m;
                tranNew.CuryUnitCost = 0m;
                tranNew.ManualDisc = true;
                tranNew.DiscPct = 0m;
                tranNew.CuryDiscAmt = 0m;
                tranNew.RetainagePct = 0m;
                tranNew.CuryRetainageAmt = 0m;
                tranNew.CuryTaxableAmt = 0m;
                tranNew.CuryTaxAmt = 0;
                tranNew.CuryExpenseAmt = 0m;
                tranNew.GroupDiscountRate = 1m;
                tranNew.DocumentDiscountRate = 1m;

                using (new PXLocaleScope(vendor.LocaleName))
                {
                    tranNew.TranDesc = PXMessages.LocalizeFormatNoPrefix(
                        Messages.RetainageForTransactionDescription,
                        APDocTypeDict[origInvoice.DocType],
                        this.Base.Document.Current.RefNbr);
                }

                prevCuryTotal = (retainageOpts.CuryRetainageAmt ?? 0m) - (this.Base.Document.Current.CuryDocBal ?? 0m);
                tranNew.CuryLineAmt = PXCurrencyAttribute.RoundCury(this.Base.Transactions.Cache, tranNew, (detail.CuryRetainageAmt ?? 0m) * retainagePercent);
                //tranNew = this.Base.Transactions.Update(tranNew);
                Base.Transactions.Insert(tranNew);
            }

            //ClearCurrentDocumentDiscountDetails();

            // We should copy all taxes from the original document
            // because it is possible to add or delete them.
            // 
            foreach (APTaxTran aptaxtran in PXSelect<APTaxTran,
                Where<APTaxTran.module, Equal<BatchModule.moduleAP>,
                    And<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>,
                    And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>>>>>
                .Select(this.Base, this.Base.Document.Current.DocType, this.Base.Document.Current.RefNbr)
                .RowCast<APTaxTran>()
                .Where(row => row.CuryRetainedTaxAmt != 0m))
            {
                APTaxTran new_aptaxtran = this.Base.Taxes.Insert(new APTaxTran
                {
                    TaxID = aptaxtran.TaxID
                });

                if (new_aptaxtran != null)
                {
                    new_aptaxtran = PXCache<APTaxTran>.CreateCopy(new_aptaxtran);
                    new_aptaxtran.TaxRate = aptaxtran.TaxRate;
                    new_aptaxtran = Base.Taxes.Update(new_aptaxtran);
                }
            }

            TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(this.Base.Transactions.Cache, null, oldTaxCalc);
            decimal diff = (retainageOpts.CuryRetainageAmt ?? 0m) - (Base.Document.Current.CuryDocBal ?? 0m);

            if (tranNew != null && diff != 0m)
            {
                HashSet<string> taxList = PXSelectJoin<APTax,
                    InnerJoin<Tax, On<Tax.taxID, Equal<APTax.taxID>>>,
                    Where<APTax.tranType, Equal<Required<APTax.tranType>>,
                        And<APTax.refNbr, Equal<Required<APTax.refNbr>>,
                        And<APTax.lineNbr, Equal<Required<APTax.lineNbr>>,
                        And<Tax.taxType, NotEqual<CSTaxType.use>>>>>>
                    .Select(this.Base, tranNew.TranType, tranNew.RefNbr, tranNew.LineNbr)
                    .RowCast<APTax>()
                    .Select(row => row.TaxID)
                    .ToHashSet();

                // To guarantee correct document total amount 
                // we should calculate last line total, 
                // including its taxes.
                //
                TaxAttribute.CalcTaxable calcClass = new TaxAttribute.CalcTaxable(false, TaxAttribute.TaxCalcLevelEnforcing.None);
                decimal curyLineAmt = calcClass.CalcTaxableFromTotalAmount(
                    this.Base.Transactions.Cache,
                    tranNew,
                    taxList,
                    this.Base.Document.Current.DocDate.Value,
                    prevCuryTotal);

                tranNew.CuryLineAmt = curyLineAmt;
                tranNew = Base.Transactions.Update(tranNew);
            }

            APVendorRefNbrAttribute aPVendorRefNbrAttribute = this.Base.Document.Cache.GetAttributesReadonly<APInvoice.invoiceNbr>()
                .OfType<APVendorRefNbrAttribute>().FirstOrDefault();
            if (aPVendorRefNbrAttribute != null)
            {
                var args = new PXFieldVerifyingEventArgs(this.Base.Document.Current, this.Base.Document.Current.InvoiceNbr, true);
                aPVendorRefNbrAttribute.FieldVerifying(this.Base.Document.Cache, args);
            }
        }

        public void createReverse()
        {
            APInvoice master = Base.Document.Current;

            //doReverseDeduction();

            if (APDocType.Invoice.Equals(master.DocType) && DeduAPTranView.Select().Count /*deductionAPTranDetails.Select().Count*/ > 0 && isReduction())
            {
                // Creating the instance of the graph
                APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
                APInvoiceEntry_Extension apInvoiceEntryExt = graph.GetExtension<APInvoiceEntry_Extension>();
                // Setting the current product for the graph
                graph.Document.Current = graph.Document.Search<APInvoice.refNbr>(master.RefNbr, master.DocType);
                //apInvoiceEntryExt.reverseDeduction.Press();
                apInvoiceEntryExt.doReverseDeduction();
                //graph.Document.Current.InvoiceNbr = master.InvoiceNbr + "_D";
                //供應商參考InvoiceNbr
                graph.Document.Current.InvoiceNbr = master.RefNbr;
                graph.Document.Cache.SetValue<APRegisterExt.usrRevDeductionNbr>(graph.Document.Current, master.RefNbr);
                APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(graph.Document.Current);
                apRegisterExt.UsrRevDeductionNbr = master.RefNbr;
                graph.Document.Update(graph.Document.Current);
                //throw new PXRedirectRequiredException(graph, true, "APInvoice");
                graph.Actions.PressSave();
                //graph.Save.Press();
            }
        }

        public void doReverseDeduction()
        {
            APInvoice origDoc = Base.Document.Current;
            if (origDoc != null && (origDoc.DocType == APDocType.Invoice || origDoc.DocType == APDocType.CreditAdj))
            {
                /** 註解調不需要的程式碼 by louis 20210713
                if (origDoc.InstallmentNbr != null && string.IsNullOrEmpty(origDoc.MasterRefNbr) == false)
                {
                    throw new PXSetPropertyException(Messages.Multiply_Installments_Cannot_be_Reversed, origDoc.MasterRefNbr);
                }

                if (origDoc.IsOriginalRetainageDocument() ||
                    origDoc.IsChildRetainageDocument())
                {
                    // Verify the case when unreleased retainage
                    // document exists.
                    // 
                    APRetainageInvoice retainageDoc = Base.RetainageDocuments
                        .Select()
                        .RowCast<APRetainageInvoice>()
                        .FirstOrDefault(row => row.Released != true);

                    if (retainageDoc != null)
                    {
                        throw new PXException(
                            Messages.ReverseRetainageNotReleasedDocument,
                            PXMessages.LocalizeNoPrefix(APDocTypeDict[retainageDoc.DocType]),
                            retainageDoc.RefNbr,
                            PXMessages.LocalizeNoPrefix(APDocTypeDict[origDoc.DocType]));
                    }

                    // Verify the case when released retainage
                    // document exists or payments applied.
                    // 
                    APAdjust adj =
                        PXSelect<APAdjust,
                        Where<APAdjust.adjdDocType, Equal<Current<APInvoice.docType>>,
                            And<APAdjust.adjdRefNbr, Equal<Current<APInvoice.refNbr>>,
                            And<APAdjust.voided, Equal<False>>>>>
                        .SelectSingleBound(Base, null);

                    bool hasPaymentsApplied = adj != null;

                    if (origDoc.IsOriginalRetainageDocument() &&
                        origDoc.CuryRetainageTotal != origDoc.CuryRetainageUnreleasedAmt ||
                        hasPaymentsApplied)
                    {
                        throw new PXException(
                            Messages.HasPaymentsOrDebAdjCannotBeReversed,
                            PXMessages.LocalizeNoPrefix(APDocTypeDict[origDoc.DocType]),
                            origDoc.RefNbr);
                    }

                    // Verify the case when reversing retainage
                    // document exists.
                    // 
                    APRegister reversingDoc;
                    if (Base.CheckReversingRetainageDocumentAlreadyExists(origDoc, out reversingDoc))
                    {
                        throw new PXException(
                            Messages.ReversingRetainageDocumentExists,
                            PXMessages.LocalizeNoPrefix(APDocTypeDict[origDoc.DocType]),
                            origDoc.RefNbr,
                            PXMessages.LocalizeNoPrefix(APDocTypeDict[reversingDoc.DocType]),
                            reversingDoc.RefNbr);
                    }
                } **/

                //Base.Save.Press();

                APInvoice doc = PXCache<APInvoice>.CreateCopy(Base.Document.Current);
                _finPeriodUtils.VerifyAndSetFirstOpenedFinPeriod<APInvoice.finPeriodID, APInvoice.branchID>(Base.Document.Cache, doc, finperiod, typeof(OrganizationFinPeriod.aPClosed));

                try
                {
                    Base.IsReverseContext = true;

                    this.ReverseDeductionProc(doc);

                    Base.Document.Cache.RaiseExceptionHandling<APInvoice.finPeriodID>(Base.Document.Current, Base.Document.Current.FinPeriodID, null);

                    //return new List<APInvoice> { Base.Document.Current };
                }
                catch (PXException)
                {
                    Base.Clear(PXClearOption.PreserveTimeStamp);
                    Base.Document.Current = doc;
                    throw;
                }
                finally
                {
                    Base.IsReverseContext = false;
                }
            }
        }

        /// <summary>
        /// Verify that existing records are not inserted.
        /// Create one/multiple lines with a withholding tax or supplementary premium.
        /// </summary>
        public void AddSpecificDeductionTrans()
        {
            KGSetUp setUp = KGSetup.Current;
            APTran aPTran = Base.Transactions.Cache.CreateInstance() as APTran;

            Base.CurrentDocument.SetValueExt<APInvoice.paymentsByLinesAllowed>(Base.CurrentDocument.Current, false);

            foreach (KGDeductionAPTran row in deductionAPTranDetails.Select())
            {
                if ((row.InventoryID == setUp.KGIndividualTaxInventoryID || row.InventoryID == setUp.KGSupplementaryTaxInventoryID) &&
                    deductionAPTranDetails.Cache.GetStatus(row) == PXEntryStatus.Notchanged)
                {
                    aPTran.LineNbr = row.LineNbr;
                    aPTran.InventoryID = row.InventoryID;
                    aPTran.TranDesc = row.TranDesc;
                    aPTran.Qty = (-1 * row.Qty);
                    aPTran.UOM = row.UOM;
                    aPTran.ProjectID = row.ProjectID;
                    aPTran.TaskID = row.TaskID;
                    aPTran.CostCodeID = row.CostCodeID;
                    aPTran.CuryLineAmt = (-1 * row.CuryLineAmt);
                    aPTran.CuryUnitCost = row.CuryUnitCost;
                    aPTran.CuryTranAmt = (-1 * row.CuryTranAmt);
                    aPTran.CuryRetainageAmt = row.CuryRetainageAmt;
                    //Base.Transactions.Cache.RaiseFieldUpdated<APTran.curyLineAmt>(aPTran, null);

                    PXCache<APTran>.GetExtension<APTranExt>(aPTran).UsrValuationType = row.ValuationType;

                    Base.Transactions.Cache.Insert(aPTran);
                }
            }
        }
        #endregion

        #region 一般
        int? downCount = 0;
        public delegate void ReverseInvoiceProcDelegate(APRegister doc);
        [PXOverride]
        public virtual void ReverseInvoiceProc(APRegister doc, ReverseInvoiceProcDelegate baseMethod)
        {
            APInvoice master = Base.Document.Current;

            baseMethod(doc);
            releaseAddAndDeduc(master);
            //2020/09/11 Althea Add 一併刪除APInvoice與GVAPGuiInvoice關聯
            //一張計價單底下有可能為多張發票
            //2020/10/12 直接刪除GVAPGuiInvoice的紀錄
            PXResultset<GVApGuiInvoice> guiinvoice =
                PXSelect<GVApGuiInvoice,
                Where<GVApGuiInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, master.RefNbr);
            if (guiinvoice != null)
            {
                foreach (GVApGuiInvoiceRef invoice in guiinvoice)
                {
                    PXDatabase.Delete<GVApGuiInvoice>(
                        new PXDataFieldRestrict<GVApGuiInvoice.guiInvoiceID>(PXDbType.Int, invoice.GuiInvoiceID));

                }
            }
            /**
            APInvoice aPInvoicecurrent = Base.Document.Current;
            KGBillSummary billSummaryINV =
                PXSelect<KGBillSummary,
                Where<KGBillSummary.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, master.RefNbr);
            KGBillSummary sum = getKGBillSummary();
            sum.AdditionAmt = billSummaryINV.AdditionAmt;
            sum.AdditionCumAmt = billSummaryINV.AdditionCumAmt;
            sum.AdditionTaxAmt = billSummaryINV.AdditionTaxAmt;
            sum.AdditionTotalAmt = billSummaryINV.AdditionTotalAmt;
            sum.AdditionWithTaxAmt = billSummaryINV.AdditionWithTaxAmt;
            sum.OriPOTotalAmt = billSummaryINV.OriPOTotalAmt;
            sum.DeductionAmt = billSummaryINV.DeductionAmt;
            sum.DeductionTaxAmt = billSummaryINV.DeductionTaxAmt;
            sum.DeductionWithTaxCumAmt = billSummaryINV.DeductionWithTaxCumAmt;
            sum.DeductionWithTaxTotalAmt = billSummaryINV.DeductionWithTaxTotalAmt;
            sum.GvInvWithTaxAmt = billSummaryINV.GvInvWithTaxAmt;
            sum.InsDeductionAmt = billSummaryINV.InsDeductionAmt;
            sum.OriTotalAmt = billSummaryINV.OriTotalAmt;
            sum.PoCumulativeAmt = billSummaryINV.PoCumulativeAmt;
            sum.PoTotalAmt = billSummaryINV.PoTotalAmt;
            sum.PoValuationAmt = billSummaryINV.PoValuationAmt;
            sum.PoValuationPercent = billSummaryINV.PoValuationPercent;
            sum.PrepaymentAmt = billSummaryINV.PrepaymentAmt;
            sum.PrepaymentCumAmt = billSummaryINV.PrepaymentCumAmt;
            sum.PrepaymentDuctAmt = billSummaryINV.PrepaymentDuctAmt;
            sum.PrepaymentDuctCumAmt = billSummaryINV.PrepaymentDuctCumAmt;
            sum.PrepaymentDuctTaxAmt = billSummaryINV.PrepaymentDuctTaxAmt;
            sum.PrepaymentDuctTotalAmt = billSummaryINV.PrepaymentDuctTotalAmt;
            sum.PrepaymentDuctWithTaxAmt = billSummaryINV.PrepaymentDuctWithTaxAmt;
            sum.PrepaymentTaxAmt = billSummaryINV.PrepaymentTaxAmt;
            sum.PrepaymentTotalAmt = billSummaryINV.PrepaymentTotalAmt;
            sum.PrepaymentWithTaxAmt = billSummaryINV.PrepaymentWithTaxAmt;
            sum.RetentionAmt = billSummaryINV.RetentionAmt;
            sum.RetentionReturnAmt = billSummaryINV.RetentionReturnAmt;
            sum.RetentionReturnTaxAmt = billSummaryINV.RetentionReturnTaxAmt;
            sum.RetentionReturnWithTaxAmt = billSummaryINV.RetentionReturnWithTaxAmt;
            sum.RetentionReturnWithTaxCumAmt = billSummaryINV.RetentionReturnWithTaxCumAmt;
            sum.RetentionReturnWithTaxTotalAmt = billSummaryINV.RetentionReturnWithTaxTotalAmt;
            sum.RetentionTaxAmt = billSummaryINV.RetentionTaxAmt;
            sum.RetentionWithTaxAmt = billSummaryINV.RetentionWithTaxAmt;
            sum.RetentionWithTaxCumAmt = billSummaryINV.RetentionWithTaxCumAmt;
            sum.RetentionWithTaxTotalAmt = billSummaryINV.RetentionWithTaxTotalAmt;
            sum.TaxAmt = billSummaryINV.TaxAmt;
            sum.TotalAmt = billSummaryINV.TotalAmt;

            //setCumAndTotalSummary(sum);
            SummaryAmtFilters.Update(sum);
            **/
            if (downCount == 0)
            {
                KGBillPayment billPayment = KGBillPaym.Insert();
                Location location = GetDefLocation(master.VendorID);
                if (location == null) throw new Exception("請先至此供應商設定預設所在地!");
                string PaymentMethodID = "";
                switch (location.PaymentMethodID)
                {
                    case "CHECK":
                        PaymentMethodID = Kedge.DAC.PaymentMethod.A;
                        break;
                    case "TT":
                        PaymentMethodID = Kedge.DAC.PaymentMethod.B;
                        break;
                    case "CASH":
                        PaymentMethodID = Kedge.DAC.PaymentMethod.C;
                        break;
                    case "COUPON":
                        PaymentMethodID = Kedge.DAC.PaymentMethod.D;
                        break;
                }
                KGBillPaym.SetValueExt<KGBillPayment.paymentMethod>(billPayment, PaymentMethodID);
                KGBillPaym.SetValueExt<KGBillPayment.paymentPeriod>(billPayment, 0);
                KGBillPaym.SetValueExt<KGBillPayment.paymentPct>(billPayment, 100m);
                KGBillPaym.SetValueExt<KGBillPayment.vendorLocationID>(billPayment, location.LocationID);
                //KGBillPaym.SetValueExt<KGBillPayment.paymentPeriod>(billPayment, 0);

                downCount++;
            }
        }
        #endregion

        #endregion

        #region 刪除
        protected virtual void APInvoice_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            APInvoice master = (APInvoice)e.Row;

            PopupReminderDialog(master);

            releaseAddAndDeducForDel(master);
            //2020/09/26 Mantis: 0011704
            //deleteAP(master);
        }
        //釋放加扣款，刪除用
        public void releaseAddAndDeducForDel(APInvoice master)
        {
            foreach (APTran apTran in Base.Transactions.Cache.Deleted)
            {
                APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                if (APDocType.Invoice.Equals(master.DocType) && "A".Equals(apTranExt.UsrValuationType))
                {
                    PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                   Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                   Set<KGValuationDetailDeduction.status, Required<KGValuationDetailDeduction.status>>>>,
                       KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                       And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                       >>.Update(Base, null, null, "V", apTranExt.UsrValuationID, "A");
                }
            }
            //edit 20200526 alton mantis:11605 Cache.Deleted會無資料
            foreach (KGDeductionAPTran dedLine in this.deductionAPTranDetails.Select())
            {
                if (dedLine.IsManageFee == false)
                {
                    PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                    Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                    Set<KGValuationDetail.status, Required<KGValuationDetail.status>>>>,
                        KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                        And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                        >>.Update(Base, null, null, "V", dedLine.ValuationID, "D");
                }
            }
        }
        //釋放加扣款，反轉用
        public void releaseAddAndDeduc(APInvoice master)
        {
            foreach (APTran apTran in Base.Transactions.View.SelectMultiBound(new object[] { master }, new object[] { }))
            {
                APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                if (APDocType.Invoice.Equals(master.DocType) && "A".Equals(apTranExt.UsrValuationType))
                {
                    PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                   Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                   Set<KGValuationDetailDeduction.status, Required<KGValuationDetailDeduction.status>>>>,
                       KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                       And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                       >>.Update(Base, null, null, "V", apTranExt.UsrValuationID, "A");

                }
            }
            //2021-01-11 mark by Alton 11716 ，因在BaseMethod之後的資料會是反轉後的資料
            //foreach (KGDeductionAPTran dedLine in this.deductionAPTranDetails.Select())
            foreach (KGDeductionAPTran dedLine in this.deductionAPTranDetails.View.SelectMultiBound(new object[] { master }, new object[] { }))
            {
                if (dedLine.IsManageFee == false)
                {
                    PXUpdate<Set<KGValuationDetail.aPInvoiceNbr, Required<KGValuationDetail.aPInvoiceNbr>,
                    Set<KGValuationDetail.aPInvoiceDate, Required<KGValuationDetail.aPInvoiceDate>,
                    Set<KGValuationDetail.status, Required<KGValuationDetail.status>>>>,
                        KGValuationDetail, Where<KGValuationDetail.valuationID, Equal<Required<KGValuationDetail.valuationID>>,
                        And<KGValuationDetail.pricingType, Equal<Required<KGValuationDetail.pricingType>>>
                        >>.Update(Base, null, null, "V", dedLine.ValuationID, "D");
                }
            }
        }

        //計價單刪除,將加扣款單回到VendorConfirm狀態
        /*public void deleteAP(APInvoice master)
        {
            //加款
            PXResultset<APTran> tran =
                PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTranExt.usrValuationType, Equal<WordType.a>>>>
                .Select(Base, master.RefNbr);
            if (tran != null)
            {
                foreach (APTran aPTran in tran)
                {
                    APTranExt aPTranExt = PXCache<APTran>.GetExtension<APTranExt>(aPTran);
                    PXUpdate<
                        Set<KGValuationDetailA.status, WordType.v,
                        Set<KGValuationDetailA.aPInvoiceDate, Null,
                        Set<KGValuationDetailA.aPInvoiceNbr, Null>>>,
                          KGValuationDetailA,
                          Where<KGValuationDetailA.valuationID, Equal<Required<APTranExt.usrValuationID>>>>
                           .Update(Base, aPTranExt.UsrValuationID);
                }

            }
            //扣款
            PXResultset<KGDeductionAPTran> detran =
                PXSelect<KGDeductionAPTran,
                Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, master.RefNbr);
            if (detran != null)
            {
                foreach (KGDeductionAPTran deAptran in detran)
                {
                    foreach(KGValuationDetailD detailD in PXSelect<KGValuationDetailD,
                        Where<KGValuationDetailD.valuationID,Equal<Required< KGDeductionAPTran.valuationID>>>>
                        .Select(Base,deAptran.ValuationID))
                    {
                        PXUpdate<
                       Set<KGValuationDetail.status, WordType.v,
                       Set<KGValuationDetail.aPInvoiceDate, Null,
                       Set<KGValuationDetail.aPInvoiceNbr, Null>>>,
                         KGValuationDetail,
                         Where<KGValuationDetail.valuationID, Equal<Required<KGDeductionAPTran.valuationID>>,
                         And<KGValuationDetail.status,Equal<Required<KGValuationDetail.status>>>>>
                          .Update(Base, deAptran.ValuationID,"D");
                    }          
                }
            }
        }*/
        #endregion

        #region 常用方法
        public int? getProjectID()
        {
            APInvoice master = Base.Document.Current;
            if (master != null)
            {
                return master.ProjectID;
            }
            foreach (APTran apTran in Base.Transactions.Select())
            {
                return apTran.ProjectID;
            }

            return null;
        }
        public APTran CreateAPTran(KGValuationDetail additionLine, KGValuation kgValuation)
        {
            APTran billLine = (APTran)Base.Transactions.Cache.CreateInstance();
            //get PO Order Line DAC

            billLine.PONbr = additionLine.OrderNbr;
            //billLine.POLineNbr = poLine.POLineNbr;
            billLine.POOrderType = additionLine.OrderType;
            billLine.InventoryID = additionLine.InventoryID;
            //billLine.TranDesc = poLine.TranDesc;
            billLine.UOM = additionLine.Uom;
            //billLine.AccountID = poLine.ExpenseAcctID;
            //ExpenseAcctID-DESCR
            //billLine.SubID = poLine.ExpenseSubID;
            billLine.ProjectID = additionLine.ContractID;

            PMCostBudget pmCostBudget = getPMCostBudget(billLine.ProjectID, billLine.InventoryID);
            if (pmCostBudget != null)
            {
                billLine.TaskID = pmCostBudget.TaskID;
                billLine.CostCodeID = pmCostBudget.CostCodeID;
            }
            //billLine.TaskID = poLine.TaskID;
            //billLine.CostCodeID = poLine.CostCodeID;
            //billLine.TaxCategoryID = poLine.TaxCategoryID;
            billLine.RetainagePct = 0;
            billLine.CuryRetainageAmt = 0;

            return billLine;
        }
        public bool checkLine(KGValuationDetail additionLine)
        {
            foreach (APTran apLine in Base.Transactions.Select())
            {
                APTranExt apLineExt = PXCache<APTran>.GetExtension<APTranExt>(apLine);
                int valuationID = 0;
                if (apLineExt.UsrValuationID != null)
                {
                    valuationID = apLineExt.UsrValuationID.Value;
                }
                if (valuationID.Equals(additionLine.ValuationID))
                {
                    return false;
                }
            }
            return true;
        }
        public bool checkDeductionLine(KGValuationDetailDeduction deductionLine)
        {
            foreach (KGDeductionAPTran dedLine in this.deductionAPTranDetails.Select())
            {
                int valuationID = 0;
                if (dedLine.ValuationID != null)
                {
                    valuationID = dedLine.ValuationID.Value;
                }
                if (valuationID.Equals(deductionLine.ValuationID))
                {
                    return false;
                }
            }
            return true;
        }
        public bool isReduction()
        {
            APInvoice master = Base.Document.Current;
            PXResultset<APInvoice> set2 = PXSelect<APInvoice, Where<APRegisterExt.usrRevDeductionNbr, Equal<Required<APRegisterExt.usrRevDeductionNbr>>>>.Select(Base, master.RefNbr);
            if (set2.Count > 0)
            {
                return false;
            }
            return true;
        }
        public Decimal? getValue(Decimal? value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value;
            }
        }
        public String getPoNbr()
        {
            APInvoice master = Base.Document.Current;
            return getPoNbr(master, Base.Transactions.Select());
        }
        public String getPoNbr(APInvoice master, PXResultset<APTran> set)
        {
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            if (apRegisterExt.UsrPONbr != null)
            {
                return apRegisterExt.UsrPONbr;
            }

            foreach (APTran apTran in set)
            {
                APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                /*if (ckeckUsrValuationTypeisNull(apTranExt))
                {
                    return apTran.PONbr;
                }*/
                if (apTran.PONbr != null)
                {
                    return apTran.PONbr;
                }
            }
            return null;
        }
        public String getPOOrderType()
        {
            APInvoice master = Base.Document.Current;
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            if (apRegisterExt.UsrPOOrderType != null)
            {
                return apRegisterExt.UsrPOOrderType;
            }
            foreach (APTran apTran in Base.Transactions.Select())
            {
                APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                //if (ckeckUsrValuationTypeisNull(apTranExt))
                //{
                if (apTran.POOrderType != null)
                {
                    return apTran.POOrderType;
                }
                //}
            }
            return null;
        }
        public POOrder getCurrentPOOrder()
        {
            String subCode = getPoNbr();
            String orderType = getPOOrderType();
            POOrder poOrder = getPOOrder(orderType, subCode);
            return poOrder;
        }
        public POOrder getCurrentPOOrder(String orderType, String orderNbr)
        {
            POOrder poOrder = getPOOrder(orderType, orderNbr);
            return poOrder;
        }
        public bool ckeckUsrValuationTypeisNull(APTranExt apTranExt)
        {
            if (apTranExt.UsrValuationType == null || "B".Equals(apTranExt.UsrValuationType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ckeckAllPricing()
        {
            bool check = true;
            foreach (APTran apTran in Base.Transactions.Select())
            {
                check = check && ckeckPricing(apTran);
            }
            return check;
        }

        public bool ckeckPricing(APTran apTran)
        {
            bool check = true;
            APInvoice master = Base.Document.Current;
            //Check Segment Pricing
            PXResultset<KGContractSegPricing> set = PXSelect<KGContractSegPricing, Where<KGContractSegPricing.orderNbr, Equal<Current<APTran.pONbr>>,
                                    And<KGContractSegPricing.lineNbr, Equal<Required<APTran.pOLineNbr>>,
                                    And<KGContractSegPricing.orderType, Equal<Required<APTran.pOOrderType>>>>>>.Select(Base, apTran.POLineNbr, apTran.POOrderType);
            if (set.Count > 0)
            {
                PXResultset<KGBillSegPricing> kgContractSegPricings = PXSelect<KGBillSegPricing, Where<KGBillSegPricing.aPRefNbr,
                Equal<Current<APInvoice.refNbr>>, And<KGBillSegPricing.aPTranID, Equal<Required<APTran.tranID>>>>>.Select(Base, apTran.TranID);
                foreach (KGBillSegPricing kgBillSegPricing in kgContractSegPricings)
                {

                    if (kgBillSegPricing.BillQty < 0)
                    {

                        segmentList.Cache.RaiseExceptionHandling<KGBillSegPricing.billQty>(
                                kgBillSegPricing, kgBillSegPricing.BillQty, new PXSetPropertyException(Message.PoQtyNullAndNagativeError));
                        Base.Transactions.Cache.RaiseExceptionHandling<APTran.qty>(
                            apTran, apTran.Qty, new PXSetPropertyException(Message.PoQtyNagativeError));
                        check = false;

                    }
                    if (kgBillSegPricing.BillQty == null)
                    {
                        check = false;
                    }

                    if (APDocType.Invoice.Equals(master.DocType))
                    {
                        if (((getValue(kgBillSegPricing.BillQty) + getValue(kgBillSegPricing.BillCumulativeQty))) * (kgBillSegPricing.SegmentPercent / 100)
                            > getValue(kgBillSegPricing.Poqty))
                        {

                            segmentList.Cache.RaiseExceptionHandling<KGBillSegPricing.billQty>(
                                    kgBillSegPricing, kgBillSegPricing.BillQty, new PXSetPropertyException(Message.PoQtySmallError));
                            Base.Transactions.Cache.RaiseExceptionHandling<APTran.qty>(
                                apTran, apTran.Qty, new PXSetPropertyException(Message.PoQtySmallError));

                            check = false;
                        }
                    }
                }
            }

            return check;
        }
        //期數影響
        public static bool ckeckPrevStatusIsOpen(int? vendorID, DateTime? docDate, int? projectID, String refNbr, String poNbr)
        {
            PXGraph graph = new PXGraph();
            //只有計價才檢查

            PXResultset<APInvoice> set = PXSelect<APInvoice, Where<APInvoice.vendorID, Equal<Required<APInvoice.vendorID>>,
                   And<APInvoice.docType, Equal<Required<APInvoice.docType>>,
                   And<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>,
                   And<APInvoice.projectID, Equal<Required<APInvoice.projectID>>>>>>>.
                   Select(graph, vendorID, APDocType.Invoice, poNbr, projectID);
            /*
            PXResultset<APInvoice> set = PXSelect<APInvoice, Where<APInvoice.vendorID, Equal<Required<APInvoice.vendorID>>,
           And<APInvoice.docType, Equal<Required<APInvoice.docType>>,
           And<APInvoice.refNbr, NotEqual<Required<APInvoice.refNbr>>,
           And<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>>>>>>.
           Select(graph, vendorID, APDocType.Invoice, refNbr, poNbr);*/
            /*
             PXResultset<APRegister> set = PXSelect<APRegister, Where<APRegister.vendorID, Equal<Required<APRegister.vendorID>>,
             And<APRegister.docType, Equal<Required<APRegister.docType>>,
             And<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>>>>>.
             Select(graph, vendorID, APDocType.Invoice, poNbr);*/

            foreach (APInvoice apInvoice in set)
            {

                if (projectID == apInvoice.ProjectID && refNbr != apInvoice.RefNbr)
                {
                    if (APDocStatus.Hold.Equals(apInvoice.Status) ||
                        APDocStatus.PendingApproval.Equals(apInvoice.Status) ||
                        APDocStatus.Rejected.Equals(apInvoice.Status))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected void CreateKGBillPayment(POOrder pOOrder)
        {
            APInvoice master = Base.Document.Current;
            //2021/01/14 add 為了不要讓他跑兩次....
            var billpaymentCache = KGBillPaym.Select();
            if (billpaymentCache.Count != 0) return;

            if (POOrderType.RegularSubcontract.Equals(pOOrder.OrderType))
            {
                {
                    //新增一筆KGBillPayment 注意要放在APTran之前
                    KGBillPayment billPayment = (KGBillPayment)KGBillPaym.Cache.CreateInstance();
                    //一般計價
                    billPayment.PricingType = PricingType.A;
                    billPayment = KGBillPaym.Insert(billPayment);
                    //CheckTitle
                    //BAccount bAccount = GetBAccount(master.VendorID);
                    //if (bAccount != null)
                    //{
                    //    billPayment.CheckTitle = bAccount.AcctName;
                    //}
                    //支票
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.paymentMethod>(billPayment, Kedge.DAC.PaymentMethod.A);
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.paymentPeriod>(billPayment, 0);
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.paymentPct>(billPayment, 100m);
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.vendorID>(billPayment, master.VendorID);
                    //billPayment.PaymentMethod = Kedge.DAC.PaymentMethod.A;
                    //billPayment.PaymentPeriod = 0;
                    //billPayment.PaymentPct = 100;
                    //billPayment.VendorLocationID = VendorLocationID(master.VendorID, billPayment.PaymentMethod) ?? null;
                    KGBillPaym.Update(billPayment);
                }
            }
            else
            {
                foreach (KGPoOrderPayment orderPayment in PXSelect<KGPoOrderPayment,
                                                                   Where<KGPoOrderPayment.orderNbr, Equal<Required<KGPoOrderPayment.orderNbr>>>>
                                                                   .Select(Base, pOOrder.OrderNbr))
                {
                    KGBillPayment billPayment = (KGBillPayment)KGBillPaym.Cache.CreateInstance();
                    billPayment.PricingType = PricingType.A;
                    billPayment = KGBillPaym.Insert(billPayment);
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.paymentMethod>(billPayment, orderPayment.PaymentMethod);
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.paymentPeriod>(billPayment, orderPayment.PaymentPeriod);
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.paymentPct>(billPayment, orderPayment.PaymentPct);
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.vendorID>(billPayment, master.VendorID);
                    //add 2021-07-20 by Alton :12162
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.vendorLocationID>(billPayment, orderPayment.VendorLocationID ?? GetDefLocation(master.VendorID).LocationID );
                    KGBillPaym.Cache.SetValueExt<KGBillPayment.bankAccountID>(billPayment, orderPayment.BankAccountID);
                    //billPayment.PaymentMethod = orderPayment.PaymentMethod;
                    //BAccount bAccount = GetBAccount(master.VendorID);
                    //if (bAccount != null)
                    //{
                    //    billPayment.CheckTitle = bAccount.AcctName;
                    //}
                    //billPayment.PaymentPeriod = orderPayment.PaymentPeriod;
                    //billPayment.PaymentPct = orderPayment.PaymentPct;
                    //billPayment.VendorLocationID = VendorLocationID(orderPayment.VendorLocationID) ?? null;
                    KGBillPaym.Update(billPayment);
                }
            }
        }

        //2020/12/24 Althea Add 供Fin override AP 塞值進去
        public virtual int? VendorLocationID(int? LocationID)
        {
            return null;
        }

        public virtual int? VendorLocationID(int? BAccount, string paymentmethod)
        {
            return null;
        }

        //2021/05/27 
        private Location GetDefLocation(int? VendorID)
        {
            return PXSelectJoin<Location,
                InnerJoin<BAccount, On<BAccount.defLocationID, Equal<Location.locationID>>>,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, VendorID);
        }

        //2020/06/29 成本小計欄位新增檢核
        public bool checkCuryUnitCost(APTran aPTran)
        {
            //需檢核條件 
            //此張計價單型態為計價,此明細的valuationType為計價,成本小計不為0或空值
            //檢核邏輯
            //((APTran.Qty * APTram.CuryUnitCost)-KGSetup.BillAdjustAmtLimit)<=APTran.CuryLineAmt <= 
            //((APTran.Qty * APTram.CuryUnitCost)+KGSetup.BillAdjustAmtLimit)
            APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(aPTran);
            APInvoice aPInvoice = PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APTran.refNbr>>>>.Select(Base, aPTran.RefNbr);
            if (apTranExt.UsrValuationType == "B" && aPInvoice.DocType == "INV"
                && aPTran.CuryLineAmt != Decimal.Zero && aPTran.CuryLineAmt != null)
            {
                KGSetUp setUp = GetSetUp();
                if (setUp.BillAdjustAmtLimit != Decimal.Zero && setUp.BillAdjustAmtLimit != null)
                {
                    decimal? CuryLineAmt = aPTran.Qty * aPTran.CuryUnitCost;
                    if (!(CuryLineAmt - setUp.BillAdjustAmtLimit <= aPTran.CuryLineAmt &&
                        aPTran.CuryLineAmt <= CuryLineAmt + setUp.BillAdjustAmtLimit))
                    {

                        Base.Transactions.Cache.RaiseExceptionHandling<APTran.curyLineAmt>(
                                            aPTran, aPTran.CuryLineAmt, new PXSetPropertyException(
                                                String.Format("成本小記調整, 必須介於(數量*成本小計)上下容許差額{0}之間",
                                     setUp.BillAdjustAmtLimit)));

                        return false;
                    }
                }
            }
            return true;

        }

        /// <summary>
        /// Total retainage amount (unbound) * retainage percent (bound).
        /// </summary>
        private decimal CalcRetainageAmt(PXCache cache, APInvoice invoice)
        {
            // The field value comes from attribute event.
            var totalRetAmt = (PXDecimalState)cache.GetValueExt<APRegisterExt.usrTotalRetainageAmt>(invoice);
            
            return Math.Round((decimal)(totalRetAmt.Value ?? 1m) * (invoice.GetExtension<APRegisterExt>().UsrRetainagePct ?? 0m) / 100m, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// This method is only for historical retainage conversion and manully adjust incorrect values.
        /// </summary>
        public virtual void PopupReminderDialog(APInvoice invoice)
        {
            if (invoice.RetainageApply == true && invoice.GetExtension<APRegisterExt>().UsrRetainageHistType == RetHistType.Original)
            {
                Base.Document.Ask(CR.Messages.Notifications, Message.NotifyKedgeIT, MessageButtons.OK, MessageIcon.Warning);
            }
        }
        #endregion

        //#region 期數
        //期數影響
        //public int getApInvoiceNum()
        //{
        //    PXGraph graph = new PXGraph();
        //    APInvoice master = Base.Document.Current;
        //    int count = 0;
        //    if (APDocType.Invoice.Equals(master.DocType) || APDocType.DebitAdj.Equals(master.DocType))
        //    {
        //        //
        //        PXResultset<APInvoice> set = getInvoiceFromVendor(master);
        //        foreach (APInvoice apInvoice in set)
        //        {
        //            if (apInvoice.RefNbr == master.RefNbr)
        //            {
        //                continue;
        //            }
        //            PXResultset<APTran> set2 = getAPTranFromVendor(apInvoice, getPoNbr());
        //            if (set2.Count > 0)
        //            {
        //                APTran apTran = set2;
        //                if (apTran.ProjectID == getProjectID())
        //                {
        //                    count = count + 1;
        //                }
        //            }
        //        }
        //    }
        //    //getProjectID()
        //    return count;
        //}
        //#endregion

        #region Message
        [PXLocalizable()]
        public class Message
        {
            public static String MustHavePoNbrError = "計價單必須最少要有一筆估驗計價明細";
            public static String PoQtySmallError = "PoQty have to bigger than BillCumulativeQty+ BillQty";
            public static String PoQtyNagativeError = "SegPricing's BillQty can't be negative.";
            public static String PoQtyNullAndNagativeError = "SegPricing's BillQty can't be null or negative.";
            public static String GVApGuiInvoiceDuplicateError = "GVApGuiInvoiceRef can't  duplicate";
            public static String APTranQtyError = "累計數量不能超過合約的總數量";
            public static String APTranAmtError = "累計金額不能超過合約的總金額";
            public static String NotYetError = "前面幾期尚未結束";
            public static String NoRededuction = "扣款單已產生無法扣款";
            public static String KGValuationType_0 = "代扣代付";
            public static String KGValuationType_1 = "罰款";
            public static String KGValuationType_2 = "一般扣款";
            //2021/06/15 Add 加扣款多一類型-會計票貼
            public static String KGValuationType_3 = "票貼";
            public static String NoSegPricing = "此筆資料不能分段計價";
            public static String CreateROProcessError = "AgentFlow寫入createROProcess異常";
            public static String LoginError = "AgentFlow登入異常";
            public static String LoginFailError = "AgentFlow登入失敗";
            public const string InvPrepayAmtCannotGreaterPOPrepayAmt = "The Prepayment Invoice can't Greater Than PO Prepayment Amount.";
            public const string ContEvaluationDidNotFilled = "分包契約未設定廠商評鑑問卷及評鑑時機 !!";
            public const string RetainageAmtCannotGreaterInvAmt = "保留款金額不能超過計價總金額。";
            public const string CannotReverseBillWithRefundRet = "已有保留款退回計價單, 不能進行反轉。";
            public const string ExtCostCannotGtrRetUnreleased = "單價不能超過保留款剩餘金額 {0}。";
            public const string NotifyKedgeIT = "此計價單已做過歷史保留款轉換，此計價單異動後請與IT聯繫處理。";
        }
        #endregion

        public static readonly Dictionary<string, string> APDocTypeDict = new APDocType.ListAttribute().ValueLabelDic;
        private DiscountEngine<APTran, APInvoiceDiscountDetail> _discountEngine => DiscountEngineProvider.GetEngineFor<APTran, APInvoiceDiscountDetail>();

        [InjectDependency]
        protected IFinPeriodUtils _finPeriodUtils { get; set; }

        [Serializable]
        public class AdditionsFilter : IBqlTable
        {
            public sealed class Open : Constant<String>
            {
                public Open() : base("N") { }
            }

            #region ContractID
            [PXInt()]
            [PXUIField(DisplayName = "Project")]
            [PXSelector(typeof(Search2<PMProject.contractID,
                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                    Equal<PMProject.contractID>>>>,
                    Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                    , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false, DescriptionField = typeof(PMProject.description))]
            public virtual int? ContractID { get; set; }
            public abstract class contractID : IBqlField { }
            #endregion

            #region OrderNbr
            [PXString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "Purchase Order", Enabled = false)]
            [PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
              And<Match<Vendor, Current<AccessInfo.userName>>>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<POOrder.orderType, Equal<Optional<POOrder.orderType>>,
              And<Vendor.bAccountID, IsNotNull, And<POLine.projectID, Equal<Current<AdditionsFilter.contractID>>,
                And<POOrder.status, Equal<Open>,
                    And<POOrder.orderType, In3<POOrderType.regularSubcontract, POOrderType.regularOrder>>>>>>,
              Aggregate<GroupBy<POOrder.orderNbr, GroupBy<POOrder.orderType>>>>), Filterable = true, DescriptionField = typeof(POOrder.orderDesc))]
            public virtual string OrderNbr { get; set; }
            public abstract class orderNbr : IBqlField { }
            #endregion

            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor")]
            [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : IBqlField { }
            #endregion

            #region ValuationDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Valuation Date From")]
            public virtual DateTime? ValuationDateFrom { get; set; }
            public abstract class valuationDateFrom : IBqlField { }
            #endregion

            #region ValuationDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Valuation Date To")]
            public virtual DateTime? ValuationDateTo { get; set; }
            public abstract class valuationDateTo : IBqlField { }
            #endregion
        }

        [Serializable]
        public class DeductionsFilter : IBqlTable
        {
            #region ContractID
            [PXInt()]
            [PXUIField(DisplayName = "Project")]
            [PXSelector(typeof(Search2<PMProject.contractID,
                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                    Equal<PMProject.contractID>>>>,
                    Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                    , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false, DescriptionField = typeof(PMProject.description))]
            public virtual int? ContractID { get; set; }
            public abstract class contractID : IBqlField { }
            #endregion

            #region OrderNbr
            [PXString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "Purchase Order", IsReadOnly = true)]
            [PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
              And<Match<Vendor, Current<AccessInfo.userName>>>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<POOrder.orderType, Equal<Optional<POOrder.orderType>>,
                 And<Vendor.bAccountID, IsNotNull,
                 And<Vendor.bAccountID, Equal<Current<APInvoice.vendorID>>,
                 And<POLine.projectID, Equal<Current<DeductionsFilter.contractID>>,
                 And<POOrder.orderType, In3<POOrderType.regularSubcontract, POOrderType.regularOrder>>>>>>,
              Aggregate<GroupBy<POOrder.orderNbr, GroupBy<POOrder.orderType>>>>), Filterable = true, DescriptionField = typeof(POOrder.orderDesc))]
            public virtual string OrderNbr { get; set; }
            public abstract class orderNbr : IBqlField { }
            #endregion

            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor")]
            [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]

            public virtual int? VendorID { get; set; }
            public abstract class vendorID : IBqlField { }
            #endregion

            #region ValuationDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Valuation Date From")]
            public virtual DateTime? ValuationDateFrom { get; set; }
            public abstract class valuationDateFrom : IBqlField { }
            #endregion

            #region ValuationDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Valuation Date To")]
            public virtual DateTime? ValuationDateTo { get; set; }
            public abstract class valuationDateTo : IBqlField { }
            #endregion
        }

        public sealed class StatusV : Constant<String>
        {
            public StatusV() : base("V") { }
        }

        public sealed class PricingTypeAddition : Constant<String>
        {
            public PricingTypeAddition() : base("A") { }
        }

        public sealed class PricingTypeDeduction : Constant<String>
        {
            public PricingTypeDeduction() : base("D") { }
        }

        #region 分錄 ADD BY Althea
        #region Select
        [PXCopyPasteHiddenFields(typeof(APInvoice.invoiceNbr), FieldsToShowInSimpleImport = new[] { typeof(APInvoice.invoiceNbr) })]
        [PXViewName(Messages.APInvoice)]
        public PXSelectJoin<
            APInvoice,
                    LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<APInvoice.vendorID>>>,
            Where<
                APInvoice.docType, Equal<Optional<APInvoice.docType>>,
                And2<Where<
                    APInvoice.origModule, NotEqual<BatchModule.moduleTX>,
                    Or<APInvoice.released, Equal<True>>>,
                And<Where<
                    Vendor.bAccountID, IsNull,
                    Or<Match<Vendor, Current<AccessInfo.userName>>>>>>>> Document;

        public PXSelect<KGVoucherH, Where<KGVoucherH.refNbr, Equal<Current<APInvoice.refNbr>>>> KGVoucherHs;
        public PXSelect<KGVoucherL, Where<KGVoucherL.voucherHeaderID, Equal<Current<KGVoucherH.voucherHeaderID>>>> KGVoucherLs;
        #endregion

        #region Action
        #region 產生分錄
        public PXAction<APInvoice> AddKGVoucher;
        [PXUIField(DisplayName = "產生分錄", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable addKGVoucher(PXAdapter adapter)
        {
            APInvoice aPInvoice = Document.Current;
            APRegister aPRegister = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            APRegisterExt aPRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(aPRegister);
            Base.Persist();
            //2020/05/25 ADD 檢查是否為APInvoice 所產生的單子才要產生分錄
            //if (aPInvoice.LastModifiedByScreenID == "AP301000" ||
            //    aPInvoice.LastModifiedByScreenID == "EP503010")
           // {
                if (aPRegisterExt.UsrVoucherNo == null)
                {

                    //2019/10/25 ADD 產生借方調整
                    //2019/11/06 ADD 借方調整刪除前先檢查狀態,不為開啟即可刪除重新新增
                    if (CheckDeleteADR(aPInvoice))
                    {
                        createReverse();
                    }
                    DeleteLine();
                    Base.Persist();
                    CreateKGVoucher();
                    Base.Persist();

                }
            //}


            //2019/10/22更改
            //因為改為自動化步驟所以不需要show Error 直接return
            /*else
            {
                throw new Exception("此單號不可產生分錄");
            }*/



            return adapter.Get();
        }
        #endregion

        #region 刪除Action
        public PXAction<APInvoice> DeleteVoucher;
        [PXUIField(DisplayName = "DeleteVoucher", Visible = false)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable deleteVoucher(PXAdapter adapter)
        {
            APInvoice aPInvoice = Document.Current;
            KGVoucherH voucherH = PXSelect<KGVoucherH, Where<KGVoucherH.refNbr,
                Equal<Required<APInvoice.refNbr>>,
                And<KGVoucherH.docType, Equal<Required<APInvoice.docType>>>>>.
                Select(Base, aPInvoice.RefNbr, aPInvoice.DocType);
            //刪除usrVoucherKey usrVoucherNo usrVoucehrDate
            PXUpdate<Set<APRegisterExt.usrVoucherDate, Required<APRegisterExt.usrVoucherDate>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(Base, null, voucherH.RefNbr);
            PXUpdate<Set<APRegisterExt.usrVoucherKey, Required<APRegisterExt.usrVoucherKey>>,
            APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
            .Update(Base, null, voucherH.RefNbr);
            PXUpdate<Set<APRegisterExt.usrVoucherNo, Required<APRegisterExt.usrVoucherNo>>,
            APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
            .Update(Base, null, voucherH.RefNbr);

            //VoucherH狀態改為P
            voucherH.Status = "P";
            KGVoucherHs.Update(voucherH);
            Base.Persist();
            return adapter.Get();
        }
        #endregion

        #endregion

        #region Method
        int rowNumber = 1;

        //建立分錄
        public void CreateKGVoucher()
        {
            APInvoice aPInvoice = Document.Current;
            //2021/03/03 Add By Althea 新增 若UsrIsDeductionDoc!= true 才需要產生分錄
            APRegister R = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            APRegisterExt RExt = PXCache<APRegister>.GetExtension<APRegisterExt>(R);

            if (aPInvoice.DocType == "INV" ||
                (aPInvoice.DocType == "ADR" && RExt.UsrIsDeductionDoc != true) ||
                aPInvoice.DocType == "PPM")
            {
                #region KGVoucherH
                KGVoucherH voucherH = (KGVoucherH)KGVoucherHs.Cache.CreateInstance();
                voucherH.DocType = aPInvoice.DocType;
                voucherH.RefNbr = aPInvoice.RefNbr;
                if (aPInvoice.ProjectID == null)
                {
                    APTran aPTran =
                        PXSelect<APTran, Where<APTran.projectID, IsNotNull,
                        And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                        .Select(Base, aPInvoice.RefNbr);
                    if (aPTran == null) return;
                    Contract contract = GetContract(aPTran.ProjectID);
                    voucherH.ContractID = aPTran.ProjectID;
                    voucherH.ContractCD = contract.ContractCD;
                    voucherH.ContractDesc = contract.Description;
                }
                else
                {
                    Contract contract = GetContract(aPInvoice.ProjectID);
                    voucherH.ContractID = aPInvoice.ProjectID;
                    voucherH.ContractCD = contract.ContractCD;
                    voucherH.ContractDesc = contract.Description;
                }

                BAccount bAccount = GetBAccount(aPInvoice.VendorID);
                //2021/01/04 Althea Fix 出現Type=VC的單子
                //經討論改為如果Type =EP就塞E,其餘都塞V
                // if (bAccount.Type == "VE")
                //{
                //    voucherH.VendorType = "V";
                //} else
                if (bAccount.Type == "EP")
                {
                    voucherH.VendorType = "E";
                }
                else
                {
                    voucherH.VendorType = "V";
                }
                voucherH.VendorID = aPInvoice.VendorID;
                voucherH.VendorCD = bAccount.AcctCD;
                voucherH.Status = "U";
                APTran tran = GetAPTran(aPInvoice.RefNbr);
                //2020/01/03 PONbr會有空值狀況
                if (tran.PONbr != null)
                {
                    voucherH.PONbr = tran.PONbr;
                }

                KGVoucherHs.Insert(voucherH);

                #endregion
                if (aPInvoice.DocType == "INV")
                {
                    CreateValuationOfAPT(voucherH, aPInvoice);//1
                    CreateValuationOfTT(voucherH, aPInvoice);//2
                    //CreateValuationOfRR(voucherH, aPInvoice);//3
                    CreateValuationOfDR(voucherH, aPInvoice);//4
                    CreateValuationOfPA(voucherH, aPInvoice);//5
                    CreateValuationOfKP(voucherH, aPInvoice);//6
                    CreateValuationOfDT(voucherH, aPInvoice);//7
                    CreateValuationOfDTT(voucherH, aPInvoice);//2019/11/05ADD
                    CreateValuationOfADJ(voucherH, aPInvoice);//8
                }
                else if (aPInvoice.DocType == "ADR")
                {
                    CreateDebitAdjustOfAPT(voucherH, aPInvoice);//1
                    CreateDebitAdjustOfTT(voucherH, aPInvoice);//2
                    CreateDebitAdjustOfRR(voucherH, aPInvoice);//3
                    CreateDebitAdjustOfDR(voucherH, aPInvoice);//4
                    CreateDebitAdjustOfPA(voucherH, aPInvoice);//5
                    CreateDebitAdjustOfKP(voucherH, aPInvoice);//6              
                }
                else if (aPInvoice.DocType == "PPM")
                {
                    CreatePrepaymentsOfAPT(voucherH, aPInvoice);//1
                    CreatePrepaymentsOfTT(voucherH, aPInvoice);//2
                    CreatePrepaymentsOfPA(voucherH, aPInvoice);//3
                    CreatePrepaymentsOfKP(voucherH, aPInvoice);//4
                }
            }

        }

        #region 計價
        //文件明細:(1)APT(假資料Inside)
        public void CreateValuationOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTran> set = PXSelect<APTran,
                 Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                 And<APTran.tranType, Equal<WordType.iNV>,
                 And<Where<APTranExt.usrValuationType, Equal<WordType.b>,
                 Or<APTranExt.usrValuationType, Equal<WordType.a>,
                 Or<APTranExt.usrValuationType, Equal<WordType.w>>>>>>>,
                 OrderBy<Asc<APTran.lineNbr>>>
                 .Select(Base, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (aPTran.CuryLineAmt > 0)
                {
                    CDType = "D";
                    LineAmt = aPTran.CuryLineAmt;
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(aPTran.CuryLineAmt));
                }
                CreateDetailLine(voucherH, aPTran.ProjectID, aPTran.AccountID,
                    LineAmt, CDType, aPTran.VendorID, "APT", null, null, aPTran.TranDesc);
                /* CreateDetailLine(voucherH, aPTran.ProjectID, aPTran.AccountID,
                     aPTran.CuryLineAmt, "D", aPTran.VendorID, "APT", null, null, aPTran);*/
            }


            //2019/11/05 Remove
            /*PXResultset<KGDeductionAPTran> setdeductionAPTran =
                PXSelect<KGDeductionAPTran,
                Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGDeductionAPTran.valuationID,IsNotNull>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach(KGDeductionAPTran tran in setdeductionAPTran)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (tran.CuryLineAmt > 0)
                {
                    CDType = "D";
                    LineAmt = tran.CuryLineAmt;
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(tran.CuryLineAmt));
                }
                CreateDetailLine(voucherH, tran.ProjectID, tran.AccountID,
                    LineAmt,CDType, tran.VendorID, "APT", null, null, tran.TranDesc);
            }*/
            #region 假資料
            /*KGVoucherL typeC = new KGVoucherL();
            APTran APTranSumforC = PXSelectGroupBy<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                And<Where<APTranExt.usrValuationType, Equal<WordType.b>,
                Or<APTranExt.usrValuationType, Equal<WordType.a>>>>>,
                Aggregate<Sum<APTran.curyLineAmt>>>.Select(Base, aPInvoice.RefNbr);
            Contract contract = GetAPInvoiceContract(aPInvoice);
            BAccount bAccount = GetAPInvoiceBAccount(aPInvoice);
            typeC.ContractID = aPInvoice.ProjectID;
            typeC.ContractCD = contract.ContractCD;
            //string CD = "2012";
            Account accountC = PXSelect<Account,
                Where<Account.accountCD, Equal<Required<Account.accountCD>>>>
                .Select(Base, "2102");
            typeC.AccountCD = accountC.AccountCD;
            typeC.AccountID = APTranSumforC.AccountID;

            typeC.AccountDesc = accountC.Description;
            typeC.Cd = "C";
            typeC.Amt = APTranSumforC.CuryLineAmt;
            if (bAccount.Type == "VE")
            {
                typeC.VendorType = "V";
            }
            else if (bAccount.Type == "EP")
            {
                typeC.VendorType = "E";
            }
            typeC.VendorCD = bAccount.AcctCD;
            typeC.ItemNo = rowNumber;
            rowNumber++;
            KGVoucherLs.Insert(typeC);*/
            #endregion
        }
        //進項稅額:(2)TT
        public void CreateValuationOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.iNV>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                decimal? TaxAmt = 0;
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "INV");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;

                TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt;
                //2019/11/05 扣款稅額改到C"貸"
                /*KGDeductionAPTran deductionAPTran = PXSelect<KGDeductionAPTran,
                    Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                    And<KGDeductionAPTran.valuationID, IsNotNull>>>
                    .Select(Base, aPInvoice.RefNbr);
                if (deductionAPTran == null)
                {
                    TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt;
                }
                else
                {
                    KGValuation valuation = PXSelect<KGValuation,
                   Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>
                   .Select(Base, deductionAPTran.ValuationID);
                    TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt + valuation.TaxAmt + valuation.ManageFeeTaxAmt;
                }*/
                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    TaxAmt, "D", register.VendorID, "TT", null, null);
            }
        }
        //退還保留款:(3)RR
        public void CreateValuationOfRR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                   PXSelect<KGBillSummary,
               Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
               And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
               .Select(Base, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;
                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetLocationAPAccountSub(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    billSummary.RetentionReturnAmt, "D", register.VendorID, "RR", null, null);
            }
        }
        //扣保留款:(4)DR
        public void CreateValuationOfDR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                PXSelect<KGBillSummary,
            Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
            And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
            .Select(Base, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;
                decimal dramt = 0;
                //2019/11/08 保留款金額改為"含稅"保留款金額
                PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.iNV>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
                foreach (APTaxTran taxTran in apTaxTran)
                {
                    if (taxTran == null) return;
                    dramt = (decimal)(dramt + taxTran.CuryRetainedTaxAmt);
                }
                dramt = dramt + (decimal)billSummary.RetentionAmt;

                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetLocationAPAccountSub(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    dramt, "C", register.VendorID, "DR", null, null);
            }
        }
        //貸方付款科目:(5)PA
        public void CreateValuationOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGCheckAccountID, IsNotNull,
                    And<KGPostageSetup.kGCashAccountID, IsNotNull>>>.Select(Base);
                if (setUp == null)
                {
                    throw new Exception("請至KG財務偏好設定維護資料!");
                }
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                decimal? amt;
                if (billPayment.PaymentMethod == "A" || billPayment.PaymentMethod == "D")
                {
                    AcctID = setUp.KGCheckAccountID;
                    //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                    amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                }
                else
                {
                    AcctID = setUp.KGCashAccountID;
                    //2019/12/18 當付款方式為現金 總金額不用做動作
                    amt = billPayment.PaymentAmount;
                }
                //decimal? amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);

                DateTime date = (DateTime)billPayment.PaymentDate;
                date.AddDays((double)billPayment.PaymentPeriod);
                CreateLine(voucherH, register.ProjectID, AcctID, amt, "C", register.VendorID,
                    "PA", billPayment.BillPaymentID, date);
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(6)KP
        public void CreateValuationOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<WordType.a, WordType.d>>>>
                .Select(Base, aPInvoice.RefNbr);
            //if (set.Count == 0) return;
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                amt = amt + calcPostage.TotalPostageAmt
                    (Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
            }
            KGBillPayment kGBillPayment = PXSelectGroupBy<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<WordType.a, WordType.d>>>,
                Aggregate<GroupBy<KGBillPayment.refNbr>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kGBillPayment == null) return;
            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null)
            {
                throw new Exception("請至KG財務偏好設定維護資料!");
            }
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID, amt,
                "C", register.VendorID, "KP", null, null);
        }
        //扣款頁籤(for 計價): (7)DT
        public void CreateValuationOfDT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APRegister> R = GetDeductionRegister(aPInvoice.RefNbr);

            foreach (APRegister aPRegister in R)
            {
                if (aPRegister == null) return;
                PXResultset<APTran> Set = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(Base, aPRegister.RefNbr);
                foreach (APTran tran in Set)
                {
                    CreateLine(voucherH, tran.ProjectID, tran.AccountID,
                        tran.CuryLineAmt, "C", tran.VendorID, "DT", null, null);
                }
            }
        }
        //扣款頁籤進項稅額(for 計價): (8)DTT
        public void CreateValuationOfDTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //抓借方調整單子的稅額明細(若TaxTran.CuryTaxAmt=0,則不用加這項)
            PXResultset<APRegister> R = GetDeductionRegister(aPInvoice.RefNbr);
            foreach (APRegister aPRegister in R)
            {
                PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.aDR>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPRegister.RefNbr);
                foreach (APTaxTran taxTran in apTaxTran)
                {
                    if (taxTran == null) return;
                    if (taxTran.CuryTaxAmt == 0) return;
                    APRegister register = GetAPRegister(taxTran.RefNbr, "ADR");
                    Tax tax = GetTax(taxTran);
                    if (tax == null) return;
                    CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                        taxTran.CuryTaxAmt, "C", register.VendorID, "DTT", null, null);
                }
            }

        }
        //退還預付款:(9)ADJ
        public void CreateValuationOfADJ(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2019/11/13 預付款改為抓估驗計價的預付款扣回金額 科目: UsrPONbr,類型:PPM,找財物明細應付帳款的科目
            /*APRegister R = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            APAdjust adjust = GetAdjust(R.DocType, R.RefNbr);
            if (R == null) return;
            if (adjust == null) return;
            PXResultset<APRegister> register = PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(Base, adjust.AdjgRefNbr);
            foreach (APRegister R2 in register)
            {
                if (R2 == null) return;
                CreateLine(voucherH, R.ProjectID, R2.APAccountID,
                    adjust.CuryAdjdAmt, "C", R.VendorID, "ADJ", null, null);
            }*/

            KGBillSummary kGBillSummary = GetKGBillSummary(aPInvoice);
            if (kGBillSummary == null) return;
            if (kGBillSummary.PrepaymentDuctAmt != null)
            {
                APRegister R = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(R);

                //找出預付款單,和此計價單相同的UsrPONbr
                APRegister registerPPM = PXSelect<APRegister,
                Where<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>,
                And<APRegister.docType, Equal<Required<APRegister.docType>>>>>
                .Select(Base, apRegisterExt.UsrPONbr, "PPM");
                if (registerPPM != null)
                {
                    //科目要抓預付款單財務明細.應付帳款的科目
                    CreateLine(voucherH, R.ProjectID, registerPPM.APAccountID,
                        kGBillSummary.PrepaymentDuctAmt, "C", R.VendorID, "ADJ", null, null);
                }

            }
        }
        #endregion

        #region 借方調整
        //文件明細:(1)APT
        public void CreateDebitAdjustOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTran> set = PXSelect<APTran,
               Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
               And<APTran.tranType, Equal<WordType.aDR>,
               And<Where<APTranExt.usrValuationType, Equal<WordType.b>,
               Or<APTranExt.usrValuationType, Equal<WordType.a>>>>>>,
               OrderBy<Asc<APTran.lineNbr>>>
               .Select(Base, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (aPTran.CuryLineAmt > 0)
                {
                    CDType = "C";
                    LineAmt = aPTran.CuryLineAmt;
                }
                else
                {
                    CDType = "D";
                    LineAmt = (-(aPTran.CuryLineAmt));
                }
                CreateDetailLine(voucherH, aPTran.ProjectID, aPTran.AccountID,
                    aPTran.CuryLineAmt, CDType, aPTran.VendorID, "APT", null, null, aPTran.TranDesc);
            }
            PXResultset<KGDeductionAPTran> setdeductionAPTran =
                PXSelect<KGDeductionAPTran,
                Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGDeductionAPTran.valuationID, IsNotNull>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGDeductionAPTran tran in setdeductionAPTran)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (tran.CuryLineAmt > 0)
                {
                    CDType = "C";
                    LineAmt = tran.CuryLineAmt;
                }
                else
                {
                    CDType = "D";
                    LineAmt = (-(tran.CuryLineAmt));
                }
                CreateDetailLine(voucherH, tran.ProjectID, tran.AccountID,
                    LineAmt, CDType, tran.VendorID, "APT", null, null, tran.TranDesc);
            }
        }
        //進項稅額:(2)TT
        public void CreateDebitAdjustOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.aDR>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "ADR");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;
                decimal? TaxAmt = 0;
                KGDeductionAPTran deductionAPTran = PXSelect<KGDeductionAPTran,
                    Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                    .Select(Base, aPInvoice.RefNbr);
                if (deductionAPTran != null)
                {
                    KGValuation valuation = PXSelect<KGValuation,
                    Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>
                    .Select(Base, deductionAPTran.ValuationID);
                    TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt + valuation.TaxAmt + valuation.ManageFeeTaxAmt;
                }
                else
                {
                    if (taxTran.CuryRetainedTaxAmt != null)
                    {
                        TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt;
                    }
                    else
                    {
                        TaxAmt = taxTran.CuryTaxAmt;
                    }

                }


                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    TaxAmt, "C", register.VendorID, "TT", null, null);
            }
        }
        //退還保留款:(3)RR
        public void CreateDebitAdjustOfRR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                   PXSelect<KGBillSummary,
               Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
               And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
               .Select(Base, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;
                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetLocationAPAccountSub(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    billSummary.RetentionReturnAmt, "C", register.VendorID, "RR",
                    null, null);
            }
        }
        //扣保留款:(4)DR
        public void CreateDebitAdjustOfDR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                PXSelect<KGBillSummary,
            Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
            And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
            .Select(Base, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;

                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetLocationAPAccountSub(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    billSummary.RetentionAmt, "D", register.VendorID, "DR", null, null);
            }
        }
        //貸方付款科目:(5)PA
        public void CreateDebitAdjustOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;

                CalcPostage calcPostage = new CalcPostage();
                KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGCashAccountID, IsNotNull,
                    And<KGPostageSetup.kGCheckAccountID, IsNotNull>>>.Select(Base);
                if (setUp == null)
                {
                    throw new Exception("請至KG財務偏好設定維護資料!");
                }
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                decimal? amt;
                if (billPayment.PaymentMethod == "A" || billPayment.PaymentMethod == "D")
                {
                    AcctID = setUp.KGCheckAccountID;
                    //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                    amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                }
                else
                {
                    AcctID = setUp.KGCashAccountID;
                    //2019/12/18 當付款方式為現金 總金額不用做動作
                    amt = billPayment.PaymentAmount;
                }
                //decimal? amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                DateTime date = (DateTime)billPayment.PaymentDate;
                date.AddDays((double)billPayment.PaymentPeriod);
                CreateLine(voucherH, register.ProjectID, AcctID, amt, "D",
                    register.VendorID, "PA", billPayment.BillPaymentID, date);
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(6)KP
        public void CreateDebitAdjustOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<WordType.a, WordType.d>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (set.Count == 0) return;
            foreach (KGBillPayment billPayment in set)
            {
                CalcPostage calcPostage = new CalcPostage();
                amt += amt + calcPostage.TotalPostageAmt
                    (Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
            }
            KGBillPayment kGBillPayment = PXSelectGroupBy<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<WordType.a, WordType.d>>>,
                Aggregate<GroupBy<KGBillPayment.refNbr>>>
                .Select(Base, aPInvoice.RefNbr);
            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null)
            {
                throw new Exception("請至KG財務偏好設定維護資料!");
            }
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID,
                amt, "D", register.VendorID, "KP", null, null);
        }
        #endregion
        //bool isSave = false;
        #region 預付款
        //文件明細:(1)APT
        public void CreatePrepaymentsOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTran> set = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                And<APTran.tranType, Equal<WordType.pPM>,
                And<APTranExt.usrValuationType, Equal<WordType.p>>>>,
                OrderBy<Asc<APTran.lineNbr>>>
                .Select(Base, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (aPTran.CuryLineAmt > 0)
                {
                    CDType = "D";
                    LineAmt = aPTran.CuryLineAmt;
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(aPTran.CuryLineAmt));
                }
                APRegister register = GetAPRegister(aPTran.RefNbr, "PPM");
                CreateDetailLine(voucherH, aPTran.ProjectID, register.APAccountID,
                    LineAmt, CDType, aPTran.VendorID, "APT", null, null, aPTran.TranDesc);
            }
            PXResultset<KGDeductionAPTran> setdeductionAPTran =
                PXSelect<KGDeductionAPTran,
                Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGDeductionAPTran.valuationID, IsNotNull>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGDeductionAPTran tran in setdeductionAPTran)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (tran.CuryLineAmt > 0)
                {
                    CDType = "D";
                    LineAmt = tran.CuryLineAmt;
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(tran.CuryLineAmt));
                }
                APRegister register = GetAPRegister(tran.RefNbr, "PPM");
                CreateDetailLine(voucherH, tran.ProjectID, register.APAccountID,
                    LineAmt, CDType, tran.VendorID, "APT", null, null, tran.TranDesc);
            }

        }
        //進項稅額:(2)TT
        public void CreatePrepaymentsOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.pPM>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "PPM");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;
                decimal? TaxAmt = 0;
                decimal? deTaxAmt = 0;
                decimal? deManageFeeTaxAmt = 0;
                KGDeductionAPTran deductionAPTran = PXSelect<KGDeductionAPTran,
                    Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                    .Select(Base, aPInvoice.RefNbr);
                if (deductionAPTran != null)
                {
                    KGValuation valuation = PXSelect<KGValuation,
                        Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>
                        .Select(Base, deductionAPTran.ValuationID);
                    deTaxAmt = valuation.TaxAmt ?? 0;
                    deManageFeeTaxAmt = valuation.ManageFeeTaxAmt ?? 0;
                }

                TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt + deTaxAmt + deManageFeeTaxAmt;
                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    TaxAmt, "D", register.VendorID, "TT", null, null);
            }
        }
        //貸方付款科目:(3)PA
        public void CreatePrepaymentsOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                KGPostageSetup setUp = PXSelect<KGPostageSetup,
                     Where<KGPostageSetup.kGCashAccountID, IsNotNull,
                     And<KGPostageSetup.kGCheckAccountID, IsNotNull>>>.Select(Base);
                if (setUp == null)
                {
                    throw new Exception("請至KG財務偏好設定維護資料!");
                }
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                if (billPayment.PaymentMethod == "A" || billPayment.PaymentMethod == "D")
                {
                    AcctID = setUp.KGCheckAccountID;
                }
                else
                {
                    AcctID = setUp.KGCashAccountID;
                }
                decimal? amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                DateTime date = (DateTime)billPayment.PaymentDate;
                date.AddDays((double)billPayment.PaymentPeriod);
                CreateLine(voucherH, register.ProjectID, AcctID, amt, "C",
                    register.VendorID, "PA", billPayment.BillPaymentID, date);
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(4)KP
        public void CreatePrepaymentsOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<WordType.a, WordType.d>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (set.Count == 0) return;
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();

                amt += amt + calcPostage.TotalPostageAmt
                    (Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
            }
            KGBillPayment kGBillPayment = PXSelectGroupBy<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<WordType.a, WordType.d>>>,
                Aggregate<GroupBy<KGBillPayment.refNbr>>>
                .Select(Base, aPInvoice.RefNbr);
            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null)
            {
                throw new Exception("請至KG財務偏好設定維護資料!");
            }
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID,
                amt, "C", register.VendorID, "KP", null, null);
        }
        #endregion

        //Line一樣的資料
        public void CreateDetailLine(KGVoucherH voucherH, int? ProjectID,
            int? AcctID, decimal? Amt, string CD, int? VendorID, string kgvouchertype
            , int? PaymentID, DateTime? DueDate, string TranDesc)
        {
            KGVoucherL voucherL = new KGVoucherL();
            APInvoice aPInvoice = Document.Current;

            KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>,
                And<KGVoucherDigestSetup.isProjectCode, Equal<True>>>>
                .Select(Base, AcctID);
            if (setup != null)
            {
                voucherL.ContractID = null;
                voucherL.ContractCD = null;
            }
            else
            {
                Contract contract = GetContract(ProjectID);
                voucherL.ContractID = contract.ContractID;
                voucherL.ContractCD = contract.ContractCD.Trim();
            }

            Account account = GetAccount(AcctID);
            voucherL.AccountID = account.AccountID;//AcctID
            voucherL.AccountCD = account.AccountCD;//AcctID
            voucherL.AccountDesc = account.Description;//AcctID
            voucherL.Cd = CD;//CD
            voucherL.Amt = Amt;//Ant
            BAccount bAccount = GetBAccount(VendorID);
            //2021/01/04 Althea Fix 出現Type=VC的單子
            //經討論改為如果Type =EP就塞E,其餘都塞V
            // if (bAccount.Type == "VE")
            //{
            //    voucherH.VendorType = "V";
            //} else
            if (bAccount.Type == "EP")
            {
                voucherL.VendorType = "E";
            }
            else
            {
                voucherL.VendorType = "V";
            }
            voucherL.VendorID = bAccount.BAccountID;//VendorID
            voucherL.VendorCD = bAccount.AcctCD.Trim();//VendorID
            voucherL.KGVoucherType = kgvouchertype;//kgvouchertype
            voucherL.ItemNo = rowNumber;

            // APTran tran = GetAPTranLine(aPTran.TranType, aPTran.RefNbr, aPTran.LineNbr);
            voucherL.Digest = TranDesc;

            //voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice);

            if (voucherL.KGVoucherType == "PA")
            {
                voucherL.BillPaymentID = PaymentID;
            }
            if (Amt == 0) return;

            voucherL.DueDate = DueDate;
            KGVoucherLs.Insert(voucherL);
            rowNumber++;
        }
        public void CreateLine(KGVoucherH voucherH, int? ProjectID,
            int? AcctID, decimal? Amt, string CD, int? VendorID, string kgvouchertype
            , int? PaymentID, DateTime? DueDate)
        {
            KGVoucherL voucherL = new KGVoucherL();
            APInvoice aPInvoice = Document.Current;
            if (ProjectID == null)
            {
                APTran aPTranContract =
                    PXSelect<APTran, Where<APTran.projectID, IsNotNull,
                    And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                    .Select(Base, aPInvoice.RefNbr);
                if (aPTranContract == null) return;
                Contract contract = GetContract(aPTranContract.ProjectID);
                voucherL.ContractID = contract.ContractID;
                voucherL.ContractCD = contract.ContractCD.Trim();
            }
            else
            {
                KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>,
                And<KGVoucherDigestSetup.isProjectCode, Equal<True>>>>
                .Select(Base, AcctID);
                if (setup != null)
                {
                    voucherL.ContractID = null;
                    voucherL.ContractCD = null;
                }
                else
                {
                    Contract contract = GetContract(ProjectID);
                    voucherL.ContractID = contract.ContractID;
                    voucherL.ContractCD = contract.ContractCD.Trim();
                }

            }

            Account account = GetAccount(AcctID);
            voucherL.AccountID = account.AccountID;//AcctID
            voucherL.AccountCD = account.AccountCD;//AcctID
            voucherL.AccountDesc = account.Description;//AcctID
            voucherL.Cd = CD;//CD
            voucherL.Amt = Amt;//Ant
            BAccount bAccount = GetBAccount(VendorID);
            //2021/01/04 Althea Fix 出現Type=VC的單子
            //經討論改為如果Type =EP就塞E,其餘都塞V
            // if (bAccount.Type == "VE")
            //{
            //    voucherH.VendorType = "V";
            //} else
            if (bAccount.Type == "EP")
            {
                voucherL.VendorType = "E";
            }
            else
            {
                voucherL.VendorType = "V";
            }
            voucherL.VendorID = bAccount.BAccountID;//VendorID
            voucherL.VendorCD = bAccount.AcctCD.Trim();//VendorID
            voucherL.KGVoucherType = kgvouchertype;//kgvouchertype
            voucherL.ItemNo = rowNumber;

            APInvoiceEntry aPInvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
            APTran aPTran = aPInvoiceEntry.Transactions.Current;


            if (voucherL.KGVoucherType == "PA")
            {
                voucherL.BillPaymentID = PaymentID;
                voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice, PaymentID, DueDate);
            }
            else
                voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice, null, DueDate);


            if (Amt == 0) return;

            voucherL.DueDate = DueDate;
            KGVoucherLs.Insert(voucherL);
            rowNumber++;
        }

        //刪除Line
        public void DeleteLine()
        {
            APInvoice aPInvoice = Document.Current;
            KGVoucherH kGVoucherH = PXSelect<KGVoucherH,
                Where<KGVoucherH.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kGVoucherH == null) return;
            PXResultset<KGVoucherL> kgvoucherL =
                PXSelect<KGVoucherL,
                Where<KGVoucherL.voucherHeaderID,
                Equal<Required<KGVoucherH.voucherHeaderID>>>>
                .Select(Base, kGVoucherH.VoucherHeaderID);

            foreach (KGVoucherL voucherL in kgvoucherL)
            {

                if (voucherL == null) return;
                KGVoucherLs.Delete(voucherL);
            }
            KGVoucherHs.Delete(kGVoucherH);
            Base.Persist();
        }

        //分錄摘要
        public string GetDigest(int? pAccountID, string Digest, APInvoice aPInvoice, int? billPaymentID, DateTime? Duedate)
        {
            KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>>>
                .Select(Base, pAccountID);
            if (setup == null) return null;

            Digest = setup.OracleDigestRule;
            string Value = "";
            if (aPInvoice.ProjectID == 0)
            {
                Value = "";
            }
            else
            {
                CSAnswers answers = PXSelectJoin<CSAnswers,
                     InnerJoin<Contract,
                     On<Contract.noteID, Equal<CSAnswers.refNoteID>,
                    And<CSAnswers.attributeID, Equal<WordType.a005>,
                    And<Contract.contractID, Equal<Required<APInvoice.projectID>>>>>>>
                    .Select(Base, aPInvoice.ProjectID);
                //2019/11/08 ADD 如果未填專案屬性提醒去設定
                if (answers == null || answers.Value == null)
                {
                    throw new Exception("請至專案的屬性維護完整資料,謝謝!");
                }
                Value = answers.Value;
            }
            BAccount bAccount = null;
            Contact contact = null;
            if (billPaymentID != null)
            {
                KGBillPayment billPayment = PXSelect<KGBillPayment, Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(Base, billPaymentID);
                bAccount = GetBAccount(billPayment.VendorID ?? aPInvoice.VendorID);
                contact = GetContact(billPayment.VendorID ?? aPInvoice.VendorID);
            }
            else
            {
                bAccount = GetBAccount(aPInvoice.VendorID);
                contact = GetContact(aPInvoice.VendorID);
            }

            if (setup.OracleDigestRule != null)
            {
                if (setup.OracleDigestRule.Contains("###專案簡稱###"))
                {
                    Digest = Digest.Replace("###專案簡稱###", Value);
                }
                if (setup.OracleDigestRule.Contains("###廠商簡稱###"))
                {
                    //2021/07/06 Add mantis : 0012123
                    if (bAccount.Type == BAccountType.EmployeeType)
                        Digest = Digest.Replace("###廠商簡稱###", bAccount.AcctName);
                    else
                        Digest = Digest.Replace("###廠商簡稱###", contact.FullName);
                }
                if (setup.OracleDigestRule.Contains("###代扣稅率###"))
                {
                    TaxTran taxTran = GetTaxTran(aPInvoice.RefNbr);
                    Digest = Digest.Replace("###代扣稅率###", taxTran.TaxRate.ToString());
                }
                if (setup.OracleDigestRule.Contains("###廠商名稱###"))
                {
                    Digest = Digest.Replace("###廠商名稱###", bAccount.AcctName);
                }
                if (setup.OracleDigestRule.Contains("###到期日###"))
                {
                    if (Duedate != null)
                    {
                        DateTime date = (DateTime)Duedate;
                        Digest = Digest.Replace("###到期日###", date.ToString("yyyy/MM/dd"));
                    }

                }
            }

            return Digest;
        }

        //借方調整的檢核, True=可以產生ADR,False=不可產生ADR&Voucher
        public bool CheckDeleteADR(APInvoice aPInvoice)
        {
            APRegister ADRApinvoice = PXSelect<APRegister,
                Where<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>>>
            .Select(Base, aPInvoice.RefNbr);
            //表示存在ADR
            if (ADRApinvoice != null)
            {
                //若ADR.Status =開啟,則不可以刪除此張借方調整
                if (ADRApinvoice.Status == "N")
                {
                    return false;
                    throw new Exception("此張預付款單狀態為開啟,不可刪除!");
                }
                else
                {
                    APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
                    // Setting the current product for the graph
                    graph.Document.Current = graph.Document.Search<APInvoice.refNbr>(ADRApinvoice.RefNbr, ADRApinvoice.DocType);
                    //APInvoiceEntry aPInvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                    //aPInvoiceEntry.Document.Current = aPInvoiceEntry.Document.Search<APInvoice.refNbr>(ADRApinvoice.RefNbr);
                    graph.Delete.Press();
                    return true;
                }
            }
            else
            {
                return true;
            }

        }
        #endregion

        #region Select Methods
        private APTran GetAPTranLine(string TranType, string RefNbr, int? LineNbr)
        {
            return PXSelect<APTran, Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTran.tranType, Equal<Required<APInvoice.docType>>,
                And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>.Select(Base, RefNbr, TranType, LineNbr);
        }
        private Contract GetContract(int? ProjectID)
        {
            return PXSelect<Contract,
                    Where<Contract.contractID, Equal<Required<POOrder.projectID>>>>
                    .Select(Base, ProjectID);
        }
        private BAccount GetBAccount(int? VendorID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, VendorID);
        }
        private Account GetAccount(int? AccountID)
        {
            return PXSelect<Account,
                    Where<Account.accountID, Equal<Required<Account.accountID>>>>
                    .Select(Base, AccountID);
        }
        private Location GetLocationAPAccountSub(APRegister register)
        {
            Location location = PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, register.VendorLocationID);

            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, location.VAPAccountLocationID);
        }
        private APRegister GetAPRegister(string RefNbr, string DocType)
        {
            return PXSelect<APRegister,
                Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                .Select(Base, DocType, RefNbr);
        }
        private Tax GetTax(TaxTran taxTran)
        {
            return PXSelect<Tax,
                Where<Tax.taxID, Equal<Required<TaxTran.taxID>>>>
                .Select(Base, taxTran.TaxID);
        }
        private KGBillSummary GetKGBillSummary(APInvoice order)
        {
            return PXSelect<KGBillSummary,
                Where<KGBillSummary.docType, Equal<Required<APInvoice.docType>>,
                And<KGBillSummary.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                .Select(Base, order.DocType, order.RefNbr);
        }
        private APTran GetAPTran(string refNbr)
        {
            return PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>>>
                .Select(Base, refNbr);
        }
        private APAdjust GetAdjust(string Rdoctype, string Rrefnbr)
        {
            return PXSelect<APAdjust,
                Where<APAdjust.adjdDocType, Equal<Required<APRegister.docType>>,
                And<APAdjust.adjdRefNbr, Equal<Required<APRegister.refNbr>>>>>
                .Select(Base, Rdoctype, Rrefnbr);
        }
        private Contact GetContact(int? VendorID)
        {
            return PXSelect<Contact,
                Where<Contact.bAccountID, Equal<Required<APInvoice.vendorID>>>>
                .Select(Base, VendorID);
        }
        private TaxTran GetTaxTran(string RefNbr)
        {
            return PXSelect<TaxTran,
                Where<TaxTran.refNbr, Equal<Required<TaxTran.refNbr>>>>
                .Select(Base, RefNbr);
        }
        private PXResultset<APRegister> GetDeductionRegister(string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegisterExt.usrIsDeductionDoc, Equal<True>,
                And<APRegisterExt.usrOriDeductionRefNbr, Equal<Required<APInvoice.refNbr>>,
                And<APRegisterExt.usrOriDeductionDocType, Equal<Required<APRegister.docType>>>>>>
                .Select(Base, RefNbr, "INV");
        }

        private Location GetLocationByID(int? locationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, locationID);
        }
        #endregion
        #endregion    
    }

    #region WordType string Constant
    public static class WordType
    {
        public const string B = "B";
        public const string A = "A";
        public const string W = "W";
        public const string V = "V";

        public const string D = "D";
        public const string PPM = "PPM";
        public const string P = "P";
        public const string INV = "INV";
        public const string ADR = "ADR";
        public const string A004 = "A004";
        public const string A005 = "A005";
        public const string PA = "PA";
        public class a004 : Constant<string>
        {
            public a004() : base(A004) { }
        }
        public class a005 : Constant<string>
        {
            public a005() : base(A005) { }
        }
        public class pA : Constant<string>
        {
            public pA() : base(PA) { }
        }
        public class v : Constant<string>
        {
            public v() : base(V) { }
        }
        public class d : Constant<string>
        {
            public d() : base(D) { }
        }
        public class aDR : Constant<string>
        {
            public aDR() : base(ADR) { }
        }
        public class iNV : Constant<string>
        {
            public iNV() : base(INV) { }
        }
        public class pPM : Constant<string>
        {
            public pPM() : base(PPM) { }
        }
        public class b : Constant<string>
        {
            public b() : base(B) { }
        }
        public class p : Constant<string>
        {
            public p() : base(P) { }
        }
        public class w : Constant<string>
        {
            public w() : base(W) { }
        }
        public class a : Constant<string>
        {
            public a() : base(A) { }
        }
    }
    #endregion

    #region Funcation Class
    public class CalcPostage
    {
        //PaymentDay:即期(S),遠期(L)
        public decimal TotalPostageAmt(PXGraph graph, int? PaymentDay, decimal? PaymentAmount)
        {
            KGPostageSetup setup = PXSelect<KGPostageSetup,
                Where<KGPostageSetup.longTerm, IsNotNull,
                And<KGPostageSetup.firstPostCost, IsNotNull,
                And<KGPostageSetup.costIntervalEnd, IsNotNull,
                And<KGPostageSetup.costInterval, IsNotNull,
                And<KGPostageSetup.costIntervalPostCost, IsNotNull>>>>>>.Select(graph);
            if (setup == null)
            {
                throw new Exception("請至KG財務偏好設定維護資料!");
            }
            decimal vTotalPoatageAmt = 0;
            int vPaymentAmt = (int)PaymentAmount;
            if (PaymentDay != 0)//遠期
            {
                vTotalPoatageAmt = (int)setup.LongTerm;//40;
            }
            else//即期
            {
                if (PaymentAmount <= setup.CostIntervalEnd)
                {
                    vTotalPoatageAmt = (decimal)setup.FirstPostCost;
                }
                else
                {
                    vTotalPoatageAmt = (decimal)setup.FirstPostCost;
                    int j = (int)setup.CostInterval;

                    for (int i = vPaymentAmt - (int)setup.CostIntervalEnd; i > 0;)
                    {
                        i = i - j;
                        vTotalPoatageAmt = vTotalPoatageAmt + (int)setup.CostIntervalPostCost;
                    }
                }
            }
            return vTotalPoatageAmt;
        }
    }
    #endregion
}