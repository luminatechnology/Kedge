using System;
using System.Collections;
using System.Text.RegularExpressions;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using RCGV.GV.Util;
using RCGV.GV.DAC;
using RC.Util;
using PX.Objects.EP;

namespace RCGV.GV
{
    /**
     * ====2021-03-18:11955 ====Alton
     * 1.移除一定要有發票明細檢核，改檢核表投SalseAmt是否大於0
     * 2.當EPRefnbr 或 Refnbr有值 關閉明細的新增
     * 
     * ====2021-03-09:11974 ====Alton
     * 1.表頭欄位連動
     *   GuiType 預設 21
     *   InvoiceDate 預設Business Date, InvoiceDate異動, 請連動DeclareYear, DeclareMonth
     *   DeclareYear 預設InvoiceDate的年份
     *   DeclareMonth 請改成StringList(1~12月) value(1~12), label(1月~12月), 預設
     *   InvoiceDate的月份
     *   Vendor異動, 請將Vendor對應的AcctName帶到VendorName, 並將Vendor對應的LocationExtAddress.TaxRegistrationID帶到VendorUniformNumber
     *   VendorName可以自行輸入修改
     *   VendorUniformNumber可以自行輸入修改
     *   InvoiceType -->畫面不顯示
     *   TaxRate -->畫面不顯示
     *   VoucherCategory 如果使用者勾選-發票(含稅)則連動InvoiceType＝發票, 其他選項則連動 InvoiceType＝收據
     *   SalesAmt = sum(GVApGuiInvoiceDetail.SalesAmt), readonly
     *   TaxAmt = sum(GVApGuiInvoiceDetail.TaxAmt), readonly
     *   TotalAmt = SalesAmt + TaxAmt, readonly
     * 2.明細欄位連動
     *   APRefNbr, LOV, 帶出APRegister, 狀態為Release的計價單, 使用者挑選後, 請連動帶出APRegister.DocDesc到GVApGuiInvoiceDetail.ItemDesc.將APInvoice.CuryLineTotal帶到GVApGuiInvoiceDetail.UnitPrice及SalesAmt, 將APInvoice.CuryTaxTotal帶到GVApGuiInvoiceDetail.TaxAmt
     *   Qty 預設待1
     *   UOM LOV, 請預設帶"式"
     * 3.APRefNbr，LOV額外條件，VendorID = GVApGuiInvoice.Vendor
     * 4.存檔前檢核明細的APRefNbr對應的APInvoice之vendorID是否與表頭一致
     * 
     * ====2021-03-22:11995 ====Alton
     * 1.GVApGuiInvoice.Hold 預設為 True, Status預設為擱置
     * 2.擱置勾起來, 狀態變為擱置.
     * 3.當狀態為開啟時, 鎖住所有欄位不能修改, 除了GVApGuiInvoice.Remark, GVApGuiInvoice.Hold
     * 4.GVApGuiInvoice.Hold被勾選, Status更新為擱置.
     * 5.Action 作廢, 只在狀態為開啟時Enable, 當使用者按下作廢Action, 顯示Conform Dialoge 請使用者輸入作廢原因 (GVApGuiInvoice.VoidReason), 使用者輸入確定後, 將Status更新為 "Void", 鎖住所有欄位不能修改異動, 並且update VoidDate（BusinessDate）, VoidReason.
     * 6.只有Status為擱置的時候, 可以刪除發票.
     * 
     * ====2021-03-29:11974 ====Alton
     * Tab-other information 的 RefNbr 及 EPRefNbr 欄位請做成hyperlink
     * 
     * ===2021-04-07:11995 ====Alton
     * 1. Action Display Name 改為「退回」，並新增一個對應的status= Return, 其他動作比照上述「作廢」action處理
     * 2. 請將hold 的checkbox及因為chechbox勾選而有狀態改變的相關邏輯拔掉
     * 3. 請多加一個button「確認」，當使用者按下此鈕時，請將GVApGUIinvoice.Status變成Open，
     *    且此時僅有「退回」的button可按，其他地方都是唯讀狀態
     *    
     * ===2021-04-15:11974 ====Alton
     * 修正明細加總
     * 
     * ====2021-05-25: 12057====Alton
     * 當稅務類別為"應稅" 明細輸入未稅金額, 請先預先計算5%的稅額到GVApGuiInvoiceDetail.TaxAmt
     * 
     * ====2021-05-26: 口頭 ====Alton
     * 明細的ArRefNbr要可以選的到表頭的RefNbr
     * 
     * ====2021-06-02:12072 ====Alton
     * 1. 請在 進項發票維護 其他資訊tab中，新增2個unbound欄位並呈現在畫面上,read-only：
     * . APRegister.DocDate
     * . APRegister.UsrIsConfirmby
     * 2.在當有APInvoice資訊時代入(改在Fin那包...)
     * 3.新增一個判斷: Voucher Category = '海關代徵營業稅' and '其他憑證' ，廠商統一編號不用必填(改在DAC)
     * 
     * ====2021-06-17:12097====Alton
     * 1. 請在GVApGuiInvoice多開一個欄位VendorAddress, nvarchar(255), 允許Null=False
     * 2. 將該放到進項發票主檔的畫面上，位置在表頭的「說明」上方
     * 3. 當選擇GVApGuiInvoice.Vendor時，請預設帶出該供應商在主檔內維護的AddressLine1
     * 沒有選擇GVApGuiInvoice.Vendor時，請預設VendorAddress為空
     * 
     * ====2021-11-18:12271====Alton
     * 1. 發票格式檢核條件調整：
     *    (1)原先會用GuiType做檢查，請改為用InvoiceType做檢核依據
     *    當InvoviceType=‘發票’，不須卡兩碼英文+八碼數字，僅許檢查總共長度是10碼即可
     *    當InvoiceType='收據‘時，則不需檢查任何格式
     *    (2) EP 及 AP 兩個發票tab都須作上述修正
     *  
     * 2. 發票格式檢核錯誤訊息調整：
     *    目前在KG傳票確認過帳時，若發票格式錯誤，會顯示英文的錯誤訊息如附圖，請協助將錯誤訊息調整成「發票號碼格式有誤，請至該張計價單- 發票憑證 的區塊做檢查」
     * **/
    public class GVApInvoiceMaint : PXGraph<GVApInvoiceMaint, GVApGuiInvoice>
    {
        public GVApInvoiceMaint()
        {
        }

        #region Action
        #region ConfirmBtn
        public PXAction<GVApGuiInvoice> ConfirmBtn;
        [PXButton(Tooltip = "Confirm invoice", CommitChanges = true)]
        [PXUIField(DisplayName = "Confirm")]
        protected virtual IEnumerable confirmBtn(PXAdapter adapter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                Persist();
                PXCache cache = Invoice.Cache;
                GVApGuiInvoice row = Invoice.Current;
                cache.SetValueExt<GVApGuiInvoice.status>(row, GVList.GVStatus.OPEN);
                cache.SetValueExt<GVApGuiInvoice.confirmDate>(row, this.Accessinfo.BusinessDate);
                cache.SetValueExt<GVApGuiInvoice.confirmPerson>(row, this.Accessinfo.UserID);
                Invoice.Update(row);
                Persist();
                ts.Complete();
            }
            return adapter.Get();
        }
        #endregion

        #region VoidBtn
        public PXAction<GVApGuiInvoice> VoidButton;
        [PXButton(Tooltip = "Return invoice", CommitChanges = true)]
        [PXUIField(DisplayName = "Return")]
        protected virtual IEnumerable voidButton(PXAdapter adapter)
        {
            PXResultset<GVApGuiCmInvoiceLine> rs = GetCMInvoiceByInvoiceNbr(Invoice.Current.GuiInvoiceNbr);
            String msg = String.Format("{0}已開立折讓不可退回", Invoice.Current.GuiInvoiceNbr);
            if (rs.Count > 0) throw new PXException(msg);
            VoidInvoicePanel.Cache.IsDirty = true;
            if (VoidInvoicePanel.AskExt(true) == WebDialogResult.OK)
            {
                GVApGuiInvoice invoice = (GVApGuiInvoice)Invoice.Cache.Current;
                GVApInvFilter voidFilter = (GVApInvFilter)VoidInvoicePanel.Cache.Current;
                if (String.IsNullOrEmpty(voidFilter.VoidReason))
                {
                    throw new PXException("Void reason is require.");
                }
                VoidGVApInvoice(invoice, voidFilter);
            }
            return adapter.Get();
        }
        #endregion

        #region EditBtn
        public PXAction<GVApGuiInvoice> EditBtn;
        [PXButton(Tooltip = "開啟欄位修改", CommitChanges = true)]
        [PXUIField(DisplayName = "修改資料")]
        protected virtual IEnumerable editBtn(PXAdapter adapter)
        {
            PXResultset<GVApGuiCmInvoiceLine> rs = GetCMInvoiceByInvoiceNbr(Invoice.Current.GuiInvoiceNbr);
            String msg = String.Format("{0}已開立折讓不可修改", Invoice.Current.GuiInvoiceNbr);
            if (rs.Count > 0) throw new PXException(msg);
            Setup.Current.IsEdit = true;
            return adapter.Get();
        }
        #endregion

        #region HyperLink
        #region ViewAPInvoice
        public PXAction<GVApGuiInvoice> ViewAPInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPInvoice()
        {
            GVApGuiInvoice row = Invoice.Current;
            if (row?.RefNbr != null) new HyperLinkUtil<APInvoiceEntry>(GetAPInvoice(row.RefNbr), true);
        }
        #endregion

        #region ViewEPExpenseClaim
        public PXAction<GVApGuiInvoice> ViewEPExpenseClaim;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewEPExpenseClaim()
        {
            GVApGuiInvoice row = Invoice.Current;
            if (row?.EPRefNbr != null) new HyperLinkUtil<ExpenseClaimEntry>(GetEPExpenseClaim(row.EPRefNbr), true);
        }
        #endregion

        #endregion


        #endregion

        #region Select
        public PXFilter<GVApInvFilter> VoidInvoicePanel;
        public PXSelect<GVApGuiInvoice> Invoice;
        public PXSelect<GVApGuiInvoiceInfo,
            Where<GVApGuiInvoiceInfo.guiInvoiceID, Equal<Current<GVApGuiInvoiceInfo.guiInvoiceID>>>> InvoiceInfo;
        public PXSelect<GVApGuiInvoiceDetail,
            Where<GVApGuiInvoiceDetail.guiInvoiceID,
                Equal<Current<GVApGuiInvoice.guiInvoiceID>>>> InvoiceDetails;
        public PXFilter<GVApInvoiceSetup> Setup;

        #endregion

        #region Override
        [PXOverride]
        public override void Persist()
        {
            Setup.Current.IsEdit = false;
            base.Persist();
        }
        #endregion

        #region Events

        #region GVApGuiInvoice
        protected virtual void _(Events.RowSelected<GVApGuiInvoice> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null) return;
            SetUI(row);
        }
        protected virtual void _(Events.RowPersisting<GVApGuiInvoice> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            bool check = true;
            //檢核年
            if (row.DeclareYear == 0)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareYear, "Year can't be zero.");
            else if (row.InvoiceDate?.Year > row.DeclareYear)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareYear, String.Format("Year can not less than invoice year {0:d}.", row.InvoiceDate?.Year));
            //檢核月
            if (row.DeclareMonth == 0)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareMonth, "Month can't be zero.");
            else if (row.InvoiceDate?.Year >= row.DeclareYear && row.InvoiceDate?.Month > row.DeclareMonth)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareMonth, String.Format("Year can not less than invoice month {0:d}.", row.InvoiceDate?.Month));
            //檢核GroupCnt
            if (row.GroupRemark == GVList.GVGuiGroupRemark.SUMMARY && row.GroupCnt == 0)
                check = SetError<GVApGuiInvoice.groupCnt>(row, row.GroupCnt, String.Format("Group count can't be zero when group remark equal to {0}", GroupRemarkAttribute.SummaryName));

            if (row.InvoiceType == GVList.GVGuiInvoiceType.INVOICE)
            {
                //檢核發票號碼格式
                //if (!Regex.Match(row.GuiInvoiceNbr, "^[A-Z]{2}[0-9]{8}").Success)
                if (row.GuiInvoiceNbr?.Length < 10)
                    check = SetError<GVApGuiInvoice.guiInvoiceNbr>(row, row.GuiInvoiceNbr, "發票號碼格式有誤!");
                //檢核發票號碼是否重複
                if (IsUsingInvoiceNbr(row.GuiInvoiceNbr, row.GuiInvoiceID))
                    check = SetError<GVApGuiInvoice.guiInvoiceNbr>(row, row.GuiInvoiceNbr, "該發票號碼已被使用");
            }
            //檢核SalesAmt
            //20220222 modify by Louis 未稅金額可以為 0
            if (row.SalesAmt < 0)
                check = SetError<GVApGuiInvoice.salesAmt>(row, row.SalesAmt, "發票金額不得為負數.");
        }
        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoice, GVApGuiInvoice.declareYear> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Year;
        }
        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoice, GVApGuiInvoice.declareMonth> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Month;
        }
        protected virtual void _(Events.FieldUpdated<GVApGuiInvoice, GVApGuiInvoice.vendor> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiInvoice.vendorUniformNumber>(row);
            e.Cache.SetDefaultExt<GVApGuiInvoice.vendorName>(row);
            e.Cache.SetDefaultExt<GVApGuiInvoice.vendorAddress>(row);
        }

        //protected virtual void _(Events.FieldUpdated<GVApGuiInvoice, GVApGuiInvoice.hold> e)
        //{
        //    GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
        //    if (row == null) return;
        //    if (row.Hold == true)
        //    {
        //        e.Cache.SetValueExt<GVApGuiInvoice.status>(row, GVList.GVStatus.HOLD);
        //    }
        //    else
        //    {
        //        e.Cache.SetValueExt<GVApGuiInvoice.status>(row, GVList.GVStatus.OPEN);
        //        e.Cache.SetValueExt<GVApGuiInvoice.confirmDate>(row, this.Accessinfo.BusinessDate);
        //        e.Cache.SetValueExt<GVApGuiInvoice.confirmPerson>(row, this.Accessinfo.UserID);
        //    }

        //}
        protected virtual void _(Events.FieldUpdated<GVApGuiInvoice, GVApGuiInvoice.voucherCategory> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            e.Cache.SetDefaultExt<GVApGuiInvoice.invoiceType>(row);
        }
        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoice, GVApGuiInvoice.invoiceType> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null) return;
            if (row.VoucherCategory == GVList.GVGuiVoucherCategory.TAXABLE)
                e.NewValue = GVList.GVGuiInvoiceType.INVOICE;
            else
                e.NewValue = GVList.GVGuiInvoiceType.RECEIPT;
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiInvoice, GVApGuiInvoice.taxCode> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null) return;
            //重新觸發檢核
            foreach (GVApGuiInvoiceDetail item in InvoiceDetails.Select())
            {
                InvoiceDetails.Cache.SetDefaultExt<GVApGuiInvoiceDetail.taxAmt>(item);
                InvoiceDetails.Update(item);
            }
        }
        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoice, GVApGuiInvoice.vendorUniformNumber> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            e.NewValue = GetDefLocation(row.Vendor)?.TaxRegistrationID;
        }
        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoice, GVApGuiInvoice.vendorName> e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            BAccountR v = (BAccountR)PXSelectorAttribute.Select<GVApGuiInvoice.vendor>(e.Cache, row);
            e.NewValue = v?.AcctName;
        }
        protected virtual void _(Events.FieldUpdated<GVApGuiInvoice, GVApGuiInvoice.invoiceDate> e)
        {
            e.Cache.SetDefaultExt<GVApGuiInvoice.declareYear>(e.Row);
            e.Cache.SetDefaultExt<GVApGuiInvoice.declareMonth>(e.Row);
        }

        #endregion

        #region GVApGuiInvoiceDetail
        protected virtual void _(Events.RowPersisting<GVApGuiInvoiceDetail> e)
        {
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            GVApGuiInvoice master = Invoice.Current;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            bool check = true;
            //檢核Qty
            if ((row.Qty ?? 0) <= 0)
                check = SetError<GVApGuiInvoiceDetail.qty>(row, row.Qty, "Qty can't be zero");
            //檢核unitPrice
            //20220222 modify by louis 單價可以為0, 不能為負數
            if ((row.UnitPrice ?? 0) < 0)
                check = SetError<GVApGuiInvoiceDetail.unitPrice>(row, row.UnitPrice, "UnitPrice can't be negative");
            //檢核TaxAmt
            if (master.TaxCode == GVList.GVTaxCode.TAXABLE && (row.TaxAmt ?? 0) <= 0)
                check = SetError<GVApGuiInvoiceDetail.taxAmt>(row, row.TaxAmt, "TaxAmt can't be zero");
            //檢核APRefNbr對應Vendor是否與表頭相同
            if (!String.IsNullOrEmpty(row.APRefNbr))
            {
                APInvoice invoice = (APInvoice)PXSelectorAttribute.Select<GVApGuiInvoiceDetail.apRefNbr>(e.Cache, row);
                //2021-05-26 add by alton 明細RefNbr要可以選的到表頭的Refnbr
                if (master.Vendor != invoice?.VendorID && master.RefNbr != row.APRefNbr)
                    check = SetError<GVApGuiInvoiceDetail.apRefNbr>(row, row.APRefNbr, "RefNbr not found.");
            }
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceDetail, GVApGuiInvoiceDetail.apRefNbr> e)
        {
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            if (row == null || String.IsNullOrEmpty((string)e.NewValue)) return;
            APInvoice r = (APInvoice)PXSelectorAttribute.Select<GVApGuiInvoiceDetail.apRefNbr>(e.Cache, row);
            if (r == null) return;
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.apDocType>(row, r.DocType);
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.itemDesc>(row, r.DocDesc);
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.unitPrice>(row, r.CuryLineTotal);
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.salesAmt>(row, r.CuryLineTotal);
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.taxAmt>(row, r.CuryTaxTotal);
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.qty>(row, 1m);
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.uom>(row, "式");
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiInvoiceDetail, GVApGuiInvoiceDetail.salesAmt> e)
        {
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiInvoiceDetail.taxAmt>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiInvoiceDetail, GVApGuiInvoiceDetail.taxAmt> e)
        {
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            if (row == null) return;
            GVApGuiInvoice master = Invoice.Current;
            decimal taxRate = 0m;
            if (master.TaxCode == GVList.GVTaxCode.TAXABLE)
                taxRate = 0.05m;
            e.NewValue = round((row.SalesAmt ?? 0m) * taxRate);
        }
        #endregion
        #endregion

        #region Methods

        /// <summary>
        /// 重新滾算表頭加總(給APInvoice過帳用)
        /// </summary>
        public void SetTotal()
        {
            GVApGuiInvoice master = Invoice.Current;
            decimal salesAmt = 0m;
            decimal taxAmt = 0m;
            foreach (GVApGuiInvoiceDetail detail in InvoiceDetails.Select())
            {
                salesAmt += detail.SalesAmt ?? 0m;
                taxAmt += detail.TaxAmt ?? 0m;
            }
            Invoice.Cache.SetValueExt<GVApGuiInvoice.salesAmt>(master, salesAmt);
            Invoice.Cache.SetValueExt<GVApGuiInvoice.taxAmt>(master, taxAmt);
            Invoice.Update(master);
        }

        private bool SetError<Field>(GVApGuiInvoice row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            Invoice.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }

        private bool SetError<Field>(GVApGuiInvoiceDetail row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            InvoiceDetails.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }

        private void SetUI(GVApGuiInvoice row)
        {
            PXCache sender = Invoice.Cache;
            bool isHold = row.Status == GVList.GVStatus.HOLD;
            bool isOpen = row.Status == GVList.GVStatus.OPEN;
            bool isVoid = row.Status == GVList.GVStatus.VOIDINVOICE;
            bool isSetupEdit = Setup.Current.IsEdit ?? false;
            bool isEpAp = !String.IsNullOrEmpty(row.RefNbr) || !String.IsNullOrEmpty(row.EPRefNbr);
            bool isApReleased = row.APReleased ?? false;
            bool hasDetail = InvoiceDetails.Select().Count > 0;

            #region GVApGuiInvoice
            PXUIFieldAttribute.SetVisible<GVApGuiInvoice.guiInvoiceID>(sender, row, false);
            

            if (isSetupEdit)
                PXUIFieldAttribute.SetReadOnly(sender, row, false);
            else
                PXUIFieldAttribute.SetReadOnly(sender, row, !isHold || (isEpAp && !isApReleased));

            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.remark>(sender, row, isVoid);
            //PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.hold>(sender, row, isVoid);

            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.taxAmt>(sender, row, true);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.salesAmt>(sender, row, true);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.totalAmt>(sender, row, true);

            PXUIFieldAttribute.SetRequired<GVApGuiInvoice.groupCnt>(sender, row.GroupRemark == GroupRemarkAttribute.Summary);
            #endregion

            #region GVApGuiInvoiceInfo
            PXUIFieldAttribute.SetReadOnly(InvoiceInfo.Cache, InvoiceInfo.Current, true);
            PXUIFieldAttribute.SetVisible<GVApGuiInvoiceInfo.printCnt>(InvoiceInfo.Cache, InvoiceInfo.Current, false);//暫時不顯示
            #endregion

            #region Action
            Invoice.Cache.AllowDelete = isHold;
            if (isEpAp)
            {
                if (isApReleased) InvoiceDetails.Cache.AllowInsert = isHold && !hasDetail;
                else InvoiceDetails.Cache.AllowInsert = false;
            }
            else
            {
                InvoiceDetails.Cache.AllowInsert = isHold;
            }
            InvoiceDetails.Cache.AllowDelete = isHold && !isEpAp;
            InvoiceDetails.Cache.AllowUpdate = isHold || isSetupEdit;
            VoidButton.SetEnabled(!isHold && row.VoidDate == null);
            ConfirmBtn.SetEnabled(isHold && (!isEpAp || isApReleased));
            EditBtn.SetEnabled(isOpen && !isSetupEdit);
            #endregion
        }

        private void VoidGVApInvoice(GVApGuiInvoice gvApInvoice, GVApInvFilter voidFilter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                gvApInvoice.VoidDate = this.Accessinfo.BusinessDate;
                gvApInvoice.VoidReason = voidFilter.VoidReason;
                gvApInvoice.Status = GVList.GVStatus.VOIDINVOICE;
                Invoice.Update(gvApInvoice);
                Save.Press();
                ts.Complete();
            }
        }

        public Decimal? round(Decimal? num)
        {
            return System.Math.Round(num ?? 0m, 0, MidpointRounding.AwayFromZero);
        }
        #endregion

        #region BQL
        public APInvoice GetAPInvoice(string refNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, refNbr);
        }

        public EPExpenseClaim GetEPExpenseClaim(string refNbr)
        {
            return PXSelect<EPExpenseClaim,
                Where<EPExpenseClaim.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                .Select(this, refNbr);
        }

        public bool IsUsingInvoiceNbr(string invoiceNbr, int? notGuiInvoiceID)
        {
            GVApGuiInvoice item = PXSelect<GVApGuiInvoice,
                Where<GVApGuiInvoice.invoiceType, Equal<GVList.GVGuiInvoiceType.invoice>,
                And<GVApGuiInvoice.guiInvoiceNbr, Equal<Required<GVApGuiInvoice.guiInvoiceNbr>>,
                And<GVApGuiInvoice.guiInvoiceID, NotEqual<Required<GVApGuiInvoice.guiInvoiceID>>>>>>
                .Select(this, invoiceNbr, notGuiInvoiceID);
            return item != null;
        }

        public Location GetDefLocation(int? baccountID)
        {
            return PXSelectJoin<Location,
                    InnerJoin<BAccount, On<BAccount.defLocationID, Equal<Location.locationID>>>,
                    Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                    .Select(new PXGraph(), baccountID);

        }

        public PXResultset<GVApGuiCmInvoiceLine> GetCMInvoiceByInvoiceNbr(string invoiceNbr)
        {
            return PXSelectJoin<GVApGuiCmInvoiceLine,
                    InnerJoin<GVApGuiCmInvoice,On<GVApGuiCmInvoice.guiCmInvoiceID,Equal<GVApGuiCmInvoiceLine.guiCmInvoiceID>>>,
                     Where<GVApGuiCmInvoiceLine.apGuiInvoiceNbr, Equal<Required<GVApGuiCmInvoiceLine.apGuiInvoiceNbr>>,
                     And<GVApGuiCmInvoice.status,NotEqual<GVList.GVStatus.voidinvoice>>>>
                     .Select(this, invoiceNbr);
        }
        #endregion

        #region Table
        [Serializable]
        [PXHidden]
        public class GVApInvFilter : IBqlTable
        {
            #region VoidReason
            [PXString(240, IsUnicode = true)]
            [PXUIField(DisplayName = "Reason", Required = true)]
            public virtual string VoidReason { get; set; }
            public abstract class voidReason : PX.Data.BQL.BqlString.Field<voidReason> { }
            #endregion
        }

        [Serializable]
        [PXHidden]
        public class GVApInvoiceSetup : IBqlTable
        {
            #region VoidReason
            [PXBool()]
            [PXUnboundDefault(typeof(False))]
            public virtual bool? IsEdit { get; set; }
            public abstract class isEdit : PX.Data.BQL.BqlString.Field<isEdit> { }
            #endregion
        }

        [Serializable]
        [PXHidden]
        [PXProjection(typeof(
            Select<GVApGuiInvoice>
            ), Persistent = false)]
        public class GVApGuiInvoiceInfo : GVApGuiInvoice
        {
        }
        #endregion
    }

}