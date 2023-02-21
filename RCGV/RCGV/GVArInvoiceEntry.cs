using System;
using System.Collections;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.AR;
using PX.Objects.CR;
using RCGV.GV.Util;
using RCGV.GV.SYS;
using static RCGV.GV.Util.GVList;

namespace RCGV.GV
{
    /***
     * ====2021-03-27:12004====Alton
     * 表頭預設值及連動
     * 1.RegistrationCD稅籍編號, 預設帶入登入者Branch所屬的HVRegisteration.RegistrationCD
     * 2.InvoiceDate 必填, 預設為BusinessDate
     * 3.GuiBookCD LOV 可挑選到GVGuiBook.StartMonth <= InvoiceDate <= GVGuiBook.EndMonth的發票, 且GVGuiBook還有剩餘發票還未開的發票簿
     * 4.CustomerID LOV, 必填. 參考ARRegister.CustomerID的LOV. 挑選完CustomerID, 請帶出Customer對應的AcctName跟TaxRegistrationID到CustName 及 CustUniformNumber
     * 5.CustName 可修改
     * 6.CustUniformNumber readonly, 必填
     * 7.CustAddress 可修改(Address.City+Address.AddressLine1+Address.AddressLine2)
     * 8.InvoiceType, 預設'I'
     * 9.GuiType, 必填, 請帶入GuiBookCD對應的GVGuiBook.GuiType
     * 10.GuiWordCD, 必填, 請帶入GuiBookCD對應的GVGuiBook.GuiWordCD
     * 11.SalesAmt, 請加總明細的SalesAmt, readonly
     * 12.TaxAmt, 請加總明細的TaxAmt, readonly
     * 13.TotalAmt, SalesAmt+TaxAmt
     * 
     * ====2021-03-29:12004====Alton
     * 表身預設值及連動
     * 1.ARRefNbr LOV 請帶出表頭CustomerID所屬的ARInvoice,請ARInvoice已經過帳. 非必填. 挑選完RefNbr請帶出ARRegister.CuryLineTotal到ARCurLineAmt
     * 2.ARCurLineAmt, unbound欄位
     * 3.ItemDesc freekeyin, 如果有挑選ARInvoice,請帶出ARRSegister.DocDesc
     * 4.ARDocType, 如果有挑選ARInvoice,請帶出ARRSegister.DocType
     * 5.Qty 預設1,
     * 6.UOM lov, 預設'式'
     * 7.SalesAmt, 大於0, 如果有ARRefNbr不能大於ARRegister.CuryLineTotal-SUM(已開立銷項發票金額)
     * 
     * ====2021-03-29:12004====Alton
     * 欄位調整
     * 1.新增一個欄位GuiInvoiceCD, IsKey=true, 系統自動編號, 使用Seqment Key 'GVARINVOICECD'
     * 2.原GuiInvoiceNbr, 發票號碼, 請改到擱置拿掉, 狀態變成Open才從發票簿中取號
     * 3.GuiInvoiceCD請放在原版面設計的GuiInvoiceNbr, lov
     * 4.原 GuiInvoiceNbr請移動到表頭的GuiWordCD位置, readonly
     * 
     * 1.預設狀態為Hold, Hold=true
     * 2.擱置拿掉, 請先提示統一發票即將取號, 請確認的Confirm Dialog, 使用者確認才變更狀態取號
     * 3.狀態為Open後, 除了CustName/CustAddress 以及明細的 ItemDesc 其餘皆不可修改及新增刪除
     * 4.新增Void"作廢" action. 只有在狀態為Open下enable.
     * 5.按下作廢, 跳出Dialog請使用者輸入作廢原因, 確認Status改為Void. 並更新VoidDate, VoidBy, VoidReason. 請鎖住全部欄位.
     * 
     * ====2021-03-31:12004====Alton
     * 1.存檔時, 請檢查如果有ARRefNbr不能大於ARRegister.CuryLineTotal-SUM(已開立銷項發票金額)
     * 2.SUM(已開立銷項發票金額)需要考慮到反轉的狀況, 如果對應的ARInvoice有反轉的借方調整‘已開立銷項發票金額'
     *   需要減掉反轉的金額.反轉對應的ARInvoice判斷邏輯, DocType='CRM' OrigDocType='$原ARinovice的DocType, 
     *   即INV', OrigRefNbr='原ARinoviceh參照號碼', 則此張ARinvoice即為原來Arinvoice的反轉發票.
     *   
     * ====2021-04-09:11998====Alton
     * 之前抓來判斷的ARRegister.CuryLineTotal 改抓 ARRegister.CuryVatTaxableTotal
     * 
     * ====2021-04-29:12030====Alton
     * 複製功能 貼上時不可以連發票號碼一併貼上
     * 
     * ====2021-05-25:12004====Alton
     * 新增以下邏輯：
     * 表頭 稅務類別＝應稅 時，才要計算TaxAmt= SalesAmt*0.05
     * 等於 零稅&免稅 時，TaxAmt皆不需做計算
     * 
     * ====2021-05-27:12066====Alton
     * 	1.進項發票維護新增一個Action, "修改資料"
     * 	2.進項發票狀態為Open時, enable"修改資料"
     * 	3.執行修改資料, 除了狀態欄位, 其他欄位都開啟可以修改, 但不能刪除(表頭/明細)或新增資料(明細)
     * 	4.儲存後, 再把所以欄位鎖定為readonly
     * 
     * ====2021-08-10:12189====Alton
     * 	1.GVArGuiInvoice 新增一個欄位 IsHistorical (是否歷史發票), bits, 預設 false
     * 	2.畫面IsHistorical 欄位放在表頭GuiBookID上面, 型態checkbox
     * 	3.使用者勾選IsHistorical, 開啟發票號碼欄位讓使用者自行輸入, 並且將發票簿改為readonly, 發票簿ID預設 0 (歷史發票簿)
     * 	4.使用者存檔, 需要檢查發票號碼在Table中不能重複(同一個年度, 用InvoiceDate的年為依據)
     * 	5.歷史發票, 一旦存檔, 不能再修改為非歷史發票
     * ====2021-08-11:12196====Alton
     * 在 銷像發票簿設定 內的發票簿，依照user在GV銷項發票/銷項批次開立/銷項折讓/銷項批次折讓 這四支作業中,
     * 所選的InvoiceDate的月份符合以下邏輯，就要在「發票簿號」LOV內顯示出來：
     * -當所選InvoiceDate的月份，在GVPeriod.OutActive = True時，即可開立銷項發票
     * -當所選InvoiceDate的月份，在GVPeriod.OutActive <> True 時，請跳出error message:'發票期別尚未解鎖或沒有設定，無法開立該期別之銷項發票
     * 
     * ====2021-10-13:12254====Alton
     * 1.開放 GVArGuiInvoice.CustUniformNumber 客戶統一編號可以修改.
     * **/

    public class GVArInvoiceEntry : PXGraph<GVArInvoiceEntry, GVArGuiInvoice>
    {
        #region Select
        public PXSelect<GVArGuiInvoice> gvArGuiInvoices;
        public PXSelect<GVArGuiInvoiceDetail,
            Where<GVArGuiInvoiceDetail.guiInvoiceID, Equal<Current<GVArGuiInvoice.guiInvoiceID>>>> gvArGuiInvoiceDetails;

        [PXCopyPasteHiddenView]
        public PXFilter<GVARInvFilter> addFilter;
        #endregion

        #region Button
        #region Confirm
        [PXFilterable]
        public PXAction<GVArGuiInvoice> ConfirmBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "確認與取號")]
        public virtual IEnumerable confirmBtn(PXAdapter adapter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                if (DoConfirm()) ts.Complete();
            }
            return adapter.Get();
        }
        #endregion

        #region Void
        [PXFilterable]
        public PXAction<GVArGuiInvoice> VoidBtn;
        [PXButton(Tooltip = "Void invoice", CommitChanges = true)]
        [PXUIField(DisplayName = "Void")]
        protected virtual IEnumerable voidBtn(PXAdapter adapter)
        {
            GVArGuiInvoice master = gvArGuiInvoices.Current;
            if (addFilter.AskExt(true) == WebDialogResult.OK)
            {
                GVArGuiInvoice invoice = gvArGuiInvoices.Current;
                GVARInvFilter arFLT = addFilter.Current;
                if (String.IsNullOrEmpty(arFLT.VoidReason))
                {
                    throw new PXException("Void reason is require.");
                }
                VoidGVArInvoice(invoice, arFLT);
            }
            else
            {
                addFilter.Cache.Clear();
            }

            return adapter.Get();
        }

        private void VoidGVArInvoice(GVArGuiInvoice gvArInvoice, GVARInvFilter voidFilter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.status>(gvArInvoice, GVList.GVStatus.VOIDINVOICE);
                gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.voidReason>(gvArInvoice, voidFilter.VoidReason);
                gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.voidBy>(gvArInvoice, this.Accessinfo.UserID);
                gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.voidDate>(gvArInvoice, this.Accessinfo.BusinessDate);
                gvArGuiInvoices.Update(gvArInvoice);
                base.Persist();
                ts.Complete();
            }
        }

        #endregion
        #endregion

        #region Event
        #region GVArGuiInvoice

        protected virtual void _(Events.RowPersisting<GVArGuiInvoice> e)
        {
            GVArGuiInvoice row = (GVArGuiInvoice)e.Row;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            checkInvoiceDate(row);
            checkCustUniform(row);
            checkInvoiceNbr(row);
            if (gvArGuiInvoiceDetails.Select().Count == 0)
                SetError<GVArGuiInvoice.guiInvoiceCD>(row, row.GuiInvoiceCD, "At least One ArInvoice");

            if (row.IsHistorical == true)
            {
                int month = (int)row.InvoiceDate?.Month;
                e.Cache.SetValueExt<GVArGuiInvoice.declarePeriod>(row, GVDeclarePeriod.GetDeclarePeriodKey(month));
            }
            //2021-03-30 mark by alton 根基目前不需要
            //if (!String.IsNullOrEmpty(row.GuiInvoiceNbr))
            //{
            //    e.Cache.SetValueExt<GVArGuiInvoice.qrcodeStr1>(row, QREncrypter.AESEncrypt(row, gvArGuiInvoiceDetails.Select()));
            //    e.Cache.SetValueExt<GVArGuiInvoice.qrcodeStr1>(row, "**");
            //}
        }

        protected virtual void _(Events.RowSelected<GVArGuiInvoice> e)
        {
            GVArGuiInvoice row = (GVArGuiInvoice)e.Row;
            if (row == null) return;
            SetUI(row);
        }

        protected virtual void _(Events.FieldUpdating<GVArGuiInvoice, GVArGuiInvoice.voidBy> e)
        {
            if (e.Row == null) return;
            if (IsCopyPasteContext)
                e.NewValue = null;
        }

        protected virtual void _(Events.FieldUpdating<GVArGuiInvoice, GVArGuiInvoice.voidDate> e)
        {
            if (e.Row == null) return;
            if (IsCopyPasteContext)
                e.NewValue = null;
        }

        protected virtual void _(Events.FieldUpdating<GVArGuiInvoice, GVArGuiInvoice.voidReason> e)
        {
            if (e.Row == null) return;
            if (IsCopyPasteContext)
                e.NewValue = null;
        }

        protected virtual void _(Events.FieldUpdating<GVArGuiInvoice, GVArGuiInvoice.guiInvoiceNbr> e)
        {
            if (e.Row == null) return;
            if (IsCopyPasteContext)
                e.NewValue = null;
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiInvoice, GVArGuiInvoice.randonNumber> e)
        {
            GVArGuiInvoice row = e.Row;
            if (row == null) return;
            e.NewValue = new Random().Next(0, 9999).ToString().PadLeft(4, '0');
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoice, GVArGuiInvoice.customerID> e)
        {
            GVArGuiInvoice row = e.Row;
            if (row == null) return;
            BAccount bAccount = PXSelectorAttribute.Select<GVArGuiInvoice.customerID>(e.Cache, row) as BAccount;
            Location location = GetLocation(bAccount?.DefLocationID);
            Address address = GetAddress(bAccount?.DefAddressID);
            e.Cache.SetValueExt<GVArGuiInvoice.custName>(row, bAccount?.AcctName);
            e.Cache.SetValueExt<GVArGuiInvoice.custUniformNumber>(row, location?.TaxRegistrationID);
            e.Cache.SetValueExt<GVArGuiInvoice.custAddress>(row, address?.AddressLine1 + address?.AddressLine2);
            //重新觸發detail檢核
            foreach (GVArGuiInvoiceDetail item in gvArGuiInvoiceDetails.Select())
            {
                gvArGuiInvoiceDetails.Update(item);
            }
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoice, GVArGuiInvoice.taxCode> e)
        {
            foreach (GVArGuiInvoiceDetail item in gvArGuiInvoiceDetails.Select())
            {
                gvArGuiInvoiceDetails.Cache.SetDefaultExt<GVArGuiInvoiceDetail.taxAmt>(item);
                gvArGuiInvoiceDetails.Update(item);
            }
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoice, GVArGuiInvoice.guiBookID> e)
        {
            GVArGuiInvoice row = (GVArGuiInvoice)e.Row;
            if (row == null) return;
            GVGuiBook book = (GVGuiBook)PXSelectorAttribute.Select<GVArGuiInvoice.guiBookID>(e.Cache, row);
            e.Cache.SetValueExt<GVArGuiInvoice.guiWordCD>(row, book?.GuiWordCD);
            e.Cache.SetValueExt<GVArGuiInvoice.guiType>(row, book?.GuiType);
            e.Cache.SetValueExt<GVArGuiInvoice.declarePeriod>(row, book?.DeclarePeriod);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoice, GVArGuiInvoice.invoiceDate> e)
        {
            GVArGuiInvoice row = e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<GVArGuiInvoice.guiBookID>(row, null);
            e.Cache.SetDefaultExt<GVArGuiInvoice.declareYear>(row);
            e.Cache.SetDefaultExt<GVArGuiInvoice.declareMonth>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiInvoice, GVArGuiInvoice.declareYear> e)
        {
            GVArGuiInvoice row = e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiInvoice, GVArGuiInvoice.declareMonth> e)
        {
            GVArGuiInvoice row = e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Month;
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoice, GVArGuiInvoice.isHistorical> e)
        {
            GVArGuiInvoice row = e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<GVArGuiInvoice.guiBookID>(row, null);
            e.Cache.SetValueExt<GVArGuiInvoice.guiInvoiceNbr>(row, null);
        }

        #endregion

        #region GVArGuiInvoiceDetail
        protected virtual void _(Events.RowPersisting<GVArGuiInvoiceDetail> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            CheckByArInvoice(row);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.arRefNbr> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            ARInvoice invoice = (ARInvoice)PXSelectorAttribute.Select<GVArGuiInvoiceDetail.arRefNbr>(e.Cache, row);
            e.Cache.SetValueExt<GVArGuiInvoiceDetail.aRDocType>(row, invoice?.DocType);
            e.Cache.SetValueExt<GVArGuiInvoiceDetail.itemDesc>(row, invoice?.DocDesc);
            e.Cache.SetValueExt<GVArGuiInvoiceDetail.unitPrice>(row, invoice?.CuryVatTaxableTotal);
            e.Cache.SetDefaultExt<GVArGuiInvoiceDetail.arCurLineAmt>(row);
            e.Cache.SetDefaultExt<GVArGuiInvoiceDetail.crmAmt>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.qty> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVArGuiInvoiceDetail.salesAmt>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.unitPrice> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVArGuiInvoiceDetail.salesAmt>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.salesAmt> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            e.NewValue = (row.Qty ?? 0) * (row.UnitPrice ?? 0);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.salesAmt> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVArGuiInvoiceDetail.taxAmt>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.taxAmt> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            GVArGuiInvoice master = gvArGuiInvoices.Current;
            decimal taxRate = 0m;
            if (master.TaxCode == GVList.GVTaxCode.TAXABLE)
                taxRate = 0.05m;
            e.NewValue = round((row.SalesAmt ?? 0m) * taxRate);
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.arCurLineAmt> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            ARInvoice invoice = GetArInvoice(row.ARRefNbr, row.ARDocType);
            e.NewValue = invoice?.CuryVatTaxableTotal;
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiInvoiceDetail, GVArGuiInvoiceDetail.crmAmt> e)
        {
            GVArGuiInvoiceDetail row = (GVArGuiInvoiceDetail)e.Row;
            if (row == null) return;
            decimal crmAmt = 0m;
            foreach (ARInvoice crmInvoice in GetCRMArInvoice(row.ARRefNbr, row.ARDocType))
            {
                crmAmt += crmInvoice?.CuryVatTaxableTotal ?? 0m;
            }
            ;
            e.NewValue = crmAmt;
        }

        #endregion
        #endregion

        #region Method
        private bool SetError<Field>(GVArGuiInvoice row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            gvArGuiInvoices.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }

        private bool SetError<Field>(GVArGuiInvoiceDetail row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            gvArGuiInvoiceDetails.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.Error));
            return false;
        }

        private bool SetWarning<Field>(GVArGuiInvoiceDetail row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            gvArGuiInvoiceDetails.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.Warning));
            return false;
        }

        private bool DoConfirm()
        {
            GVArGuiInvoice row = gvArGuiInvoices.Current;
            try
            {
                Persist();
                //檢核日期
                if ((row.IsHistorical ?? false) != true)
                {
                    gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.guiInvoiceNbr>(row, GVGuiBookUtil.GetArInvoiceNumber(row.GuiBookID, row.InvoiceDate));
                }
                gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.status>(row, GVList.GVStatus.OPEN);
                gvArGuiInvoices.Update(row);
                Persist();
            }
            catch (Exception e)
            {
                SetError<GVArGuiInvoice.guiBookID>(row, row.GuiBookID, e.Message);
                gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.guiInvoiceNbr>(row, null);
                gvArGuiInvoices.Cache.SetValueExt<GVArGuiInvoice.status>(row, GVList.GVStatus.HOLD);
                gvArGuiInvoices.Update(row);
                return false;
            }
            return true;
        }

        #region Verfying master
        //檢查InvoiceDate
        public void checkInvoiceDate(GVArGuiInvoice row)
        {
            if (row.IsHistorical == true) return;
            GVGuiBook book = GetGuiBook(row.GuiBookID);
            int? declareYear = book.DeclareYear;
            int? startMonth = book.StartMonth;
            int? endMonth = book.EndMonth;
            if (!(row.DeclareMonth >= startMonth && row.DeclareMonth <= endMonth))
            {
                SetError<GVArGuiInvoice.invoiceDate>(
                    row, row.InvoiceDate, String.Format("Invoice Date must between {0} and {1}",
                                                         declareYear + "/" + startMonth,
                                                         declareYear + "/" + endMonth));
            }
            GVPeriod gvPeriod = GetGVPeriod(row.RegistrationCD, row.InvoiceDate?.Year, row.InvoiceDate?.Month);
            if (gvPeriod == null)
                SetError<GVArGuiInvoice.invoiceDate>(
                       row, row.InvoiceDate, "發票期別尚未解鎖或沒有設定，無法開立該期別之銷項發票/折讓");

        }

        public void checkCustUniform(GVArGuiInvoice row)
        {
            if (row.CustUniformNumber == null) return;
            int custValueLength = row.CustUniformNumber.Length;
            if (custValueLength != 8)
            {
                SetError<GVArGuiInvoice.custUniformNumber>(
                       row, row.CustUniformNumber, "Customer uniform number must be 8 digits number!");
            }
        }

        public void checkInvoiceNbr(GVArGuiInvoice row)
        {
            if (row.IsHistorical == true && row.GuiInvoiceNbr == null)
            {
                SetError<GVArGuiInvoice.guiInvoiceNbr>(
                           row, row.GuiInvoiceNbr, "請輸入發票號碼!");
            }

            if (row.GuiInvoiceNbr != null)
            {
                int? year = row.InvoiceDate?.Year;
                PXResultset<GVArGuiInvoice> rs = GetGVInvoiceByGVNbr(row.GuiInvoiceNbr, row.GuiInvoiceID);
                //如果年份依樣則跳錯
                foreach (GVArGuiInvoice item in rs)
                {
                    int? _year = item.InvoiceDate?.Year;
                    if (year == _year)
                        SetError<GVArGuiInvoice.guiInvoiceNbr>(
                           row, row.GuiInvoiceNbr, "發票號碼不可重複!");
                }
            }
        }


        #endregion

        #region Verifying Detail

        /// <summary>
        /// 檢核與ARInvoice相關資訊
        /// </summary>
        /// <param name="row"></param>
        public void CheckByArInvoice(GVArGuiInvoiceDetail row)
        {
            GVArGuiInvoice master = gvArGuiInvoices.Current;
            if (row.ARRefNbr == null) return;
            ARInvoice arInvoice = GetArInvoice(row.ARRefNbr, row.ARDocType);
            if (arInvoice.CustomerID != master.CustomerID)
            {
                SetError<GVArGuiInvoiceDetail.arRefNbr>(
                    row, row.ARRefNbr, "This ARRefNbr do not belong to the same customer!");
            }
            else
            {
                decimal total = 0m;
                foreach (GVArGuiInvoiceDetail item in GetAllDetailByArRefNbr(row.ARRefNbr, row.ARDocType, row.GuiInvoiceDetailID))
                {
                    total += (item.SalesAmt ?? 0);
                }
                if (row.ARCurLineAmt < (total + (row.SalesAmt ?? 0)))
                    SetError<GVArGuiInvoiceDetail.salesAmt>(
                    row, row.SalesAmt, String.Format("已超過{0}可開立金額(餘額：{1:0.00})", row.ARRefNbr, row.ARCurLineAmt - total));
                else if ((row.ARCurLineAmt - row.CRMAmt) < (total + (row.SalesAmt ?? 0)))
                    SetWarning<GVArGuiInvoiceDetail.salesAmt>(
                    row, row.SalesAmt, String.Format("已超過{0}反轉後金額(餘額：{1:0.00})", row.ARRefNbr, row.ARCurLineAmt - total - row.CRMAmt));
            }
        }

        #endregion

        private void SetUI(GVArGuiInvoice row)
        {
            bool isInsert = gvArGuiInvoices.Cache.GetStatus(row) == PXEntryStatus.Inserted;
            bool isVoid = row.Status == GVList.GVStatus.VOIDINVOICE;
            bool isHold = row.Status == GVList.GVStatus.HOLD;
            bool isOpen = row.Status == GVList.GVStatus.OPEN;
            //bool hasInvNbr = !String.IsNullOrEmpty(row.GuiInvoiceNbr) && !isInsert;
            bool hasCMInv = HasAllowInvoice(row.GuiInvoiceNbr);
            bool isHistorical = (row.IsHistorical ?? false) == true;

            #region GVArGuiInvoice
            gvArGuiInvoices.Cache.AllowDelete = isHold;
            PXUIFieldAttribute.SetReadOnly(gvArGuiInvoices.Cache, row, isVoid || isOpen);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.guiInvoiceNbr>(gvArGuiInvoices.Cache, row, !isHistorical || isVoid || isOpen);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.guiType>(gvArGuiInvoices.Cache, row, !isHistorical || isVoid || isOpen);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.guiBookID>(gvArGuiInvoices.Cache, row, isHistorical || isVoid || isOpen);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.isHistorical>(gvArGuiInvoices.Cache, row, !isInsert);

            if (isOpen && !isVoid)
            {
                PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.custName>(gvArGuiInvoices.Cache, row, false);
                PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.custAddress>(gvArGuiInvoices.Cache, row, false);
                PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.custUniformNumber>(gvArGuiInvoices.Cache, row, false);
            }

            #region 固定
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.guiInvoiceCD>(gvArGuiInvoices.Cache, row, false);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.voidReason>(gvArGuiInvoices.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.voidDate>(gvArGuiInvoices.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoice.voidBy>(gvArGuiInvoices.Cache, row, true);
            

            #endregion
            #endregion

            #region GVArGuiInvoiceDetail
            gvArGuiInvoiceDetails.Cache.AllowDelete = isHold;
            gvArGuiInvoiceDetails.Cache.AllowInsert = isHold;
            gvArGuiInvoiceDetails.Cache.AllowUpdate = !isVoid;
            //add by alton 避免切換上下筆時沒有被開啟欄位
            PXUIFieldAttribute.SetReadOnly(gvArGuiInvoiceDetails.Cache, null, false);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoiceDetail.arCurLineAmt>(gvArGuiInvoiceDetails.Cache, null, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiInvoiceDetail.crmAmt>(gvArGuiInvoiceDetails.Cache, null, true);
            if (isOpen && !isVoid)
            {
                PXUIFieldAttribute.SetReadOnly(gvArGuiInvoiceDetails.Cache, null, true);
                PXUIFieldAttribute.SetReadOnly<GVArGuiInvoiceDetail.itemDesc>(gvArGuiInvoiceDetails.Cache, null, false);
            }

            #endregion

            VoidBtn.SetEnabled(!hasCMInv && isOpen);
            //VoidBtn.SetVisible(!hasCMInv && !isInsert && hasInvNbr && !isVoid);
            ConfirmBtn.SetEnabled(!isVoid && !isOpen && !isInsert);
            ConfirmBtn.SetVisible(!isVoid && !isOpen && !isInsert);
        }
        public Decimal? round(Decimal? num)
        {
            return System.Math.Round(num ?? 0m, 0, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region BQL
        private GVPeriod GetGVPeriod(string registrationCD, int? year, int? month)
        {
            return PXSelect<GVPeriod,
                Where<GVPeriod.registrationCD, Equal<Required<GVPeriod.registrationCD>>,
                And<GVPeriod.periodYear, Equal<Required<GVPeriod.periodYear>>,
                And<GVPeriod.periodMonth, Equal<Required<GVPeriod.periodMonth>>,
                And<GVPeriod.outActive,Equal<True>>>>>>
                .Select(this, registrationCD, year, month);
        }

        private PXResultset<GVArGuiInvoice> GetGVInvoiceByGVNbr(string invoiceNbr, int? notGuiInvoiceID)
        {
            return PXSelect<GVArGuiInvoice,
                 Where<GVArGuiInvoice.guiInvoiceNbr, Equal<Required<GVArGuiInvoice.guiInvoiceNbr>>,
                 And<GVArGuiInvoice.guiInvoiceID, NotEqual<Required<GVArGuiInvoice.guiInvoiceID>>>>>
                 .Select(this, invoiceNbr, notGuiInvoiceID);
        }

        private ARInvoice GetArInvoice(string arRefNbr, string arDocType)
        {
            return PXSelect<ARInvoice,
                Where<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>,
                And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>>>>
                .Select(this, arRefNbr, arDocType);
        }

        private PXResultset<ARInvoice> GetCRMArInvoice(string origRefNbr, string origDocType)
        {
            return PXSelect<ARInvoice,
                Where<ARInvoice.origRefNbr, Equal<Required<ARInvoice.origRefNbr>>,
                And<ARInvoice.origDocType, Equal<Required<ARInvoice.origDocType>>,
                And<ARInvoice.docType, Equal<ARDocType.creditMemo>>>>>
                .Select(this, origRefNbr, origDocType);
        }

        /// <summary>
        /// 取得ARInvoice底下的所有發票，且排除作廢發票與自己
        /// </summary>
        /// <param name="arRefNbr"></param>
        /// <param name="arDocType"></param>
        /// <returns></returns>
        private PXResultset<GVArGuiInvoiceDetail> GetAllDetailByArRefNbr(string arRefNbr, string arDocType, int? detailID)
        {
            return PXSelectJoin<GVArGuiInvoiceDetail,
                InnerJoin<GVArGuiInvoice, On<GVArGuiInvoice.guiInvoiceID, Equal<GVArGuiInvoiceDetail.guiInvoiceID>>>,
                Where<GVArGuiInvoiceDetail.arRefNbr, Equal<Required<GVArGuiInvoiceDetail.arRefNbr>>,
                And<GVArGuiInvoiceDetail.aRDocType, Equal<Required<GVArGuiInvoiceDetail.aRDocType>>,
                And<GVArGuiInvoiceDetail.guiInvoiceDetailID, NotEqual<Required<GVArGuiInvoiceDetail.guiInvoiceDetailID>>,
                And<GVArGuiInvoice.status, NotEqual<GVList.GVStatus.voidinvoice>>>>>>
                .Select(this, arRefNbr, arDocType, detailID);
        }

        private bool HasAllowInvoice(string guiInvoiceNbr)
        {
            GVArGuiCmInvoiceLine line = PXSelect<GVArGuiCmInvoiceLine,
                             Where<GVArGuiCmInvoiceLine.arGuiInvoiceNbr,
                             Equal<Required<GVArGuiCmInvoiceLine.arGuiInvoiceNbr>>>>
                             .Select(this, guiInvoiceNbr);
            return line != null;
        }

        private Location GetLocation(int? locationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(this, locationID);
        }

        private Address GetAddress(int? addressID)
        {
            return PXSelect<Address,
                Where<Address.addressID, Equal<Required<Address.addressID>>>>
                .Select(this, addressID);
        }

        private GVGuiBook GetGuiBook(int? guiBookID)
        {
            return PXSelect<GVGuiBook,
                Where<GVGuiBook.guiBookID, Equal<Required<GVGuiBook.guiBookID>>>>
                .Select(this, guiBookID);
        }
        #endregion

        #region Table
        [Serializable]
        [PXHidden]
        public class GVARInvFilter : IBqlTable
        {
            #region VoidReason
            [PXString(240, IsUnicode = true)]
            [PXUIField(DisplayName = "Void Reason", Required = true)]
            public virtual string VoidReason { get; set; }
            public abstract class voidReason : PX.Data.BQL.BqlString.Field<voidReason> { }
            #endregion
        }
        #endregion
    }
}