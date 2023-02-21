using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.AP;
using RCGV.GV.DAC;
using PX.Common;
using PX.Objects.GL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using PX.Objects.CM;
using System.Globalization;
using RCGV.GV.Util;
using System.Text.RegularExpressions;

namespace RCGV.GV
{

    public class GVApInvoiceEntry : PXGraph<GVApInvoiceEntry>
    {

        #region Action
        public PXInsert<GVApGuiInvoice> Insert;
        public PXSave<GVApGuiInvoice> Save;
        public PXCancel<GVApGuiInvoice> Cancel;
        public PXCopyPasteAction<GVApGuiInvoice> CopyPaste;
        public PXDelete<GVApGuiInvoice> Delete;
        public PXFirst<GVApGuiInvoice> First;
        public PXPrevious<GVApGuiInvoice> Previous;
        public PXNext<GVApGuiInvoice> Next;
        public PXLast<GVApGuiInvoice> Last;

        public PXAction<GVApGuiInvoice> VoidButton;
        [PXButton(Tooltip = "Void invoice")]
        [PXUIField(DisplayName = "VOID",
            MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select,
            Enabled = false)]
        protected virtual IEnumerable voidButton(PXAdapter adapter)
        {
            if (VoidInvoicePanel.AskExt(true) == WebDialogResult.OK)
            {
                GVApGuiInvoice invoice = (GVApGuiInvoice)Invoice.Cache.Current;
                GVApInvFilter voidFilter = (GVApInvFilter)VoidInvoicePanel.Cache.Current;
                if (String.IsNullOrEmpty(voidFilter.VoidReason))
                {
                    throw new PXException("Void reason is require.");
                }
                PXLongOperation.StartOperation(this, delegate ()
                {
                    VoidGVApInvoice(invoice, voidFilter);
                });
            }
            return adapter.Get();
        }

        public PXFilter<GVAPInvCriteriaFilter> AddItemMasterView;
        /// <summary>
        /// SmartPanel DetailView
        /// </summary>
        [PXCopyPasteHiddenFields]
        [PXFilterable]

        public PXSelectJoin<APInvoice,
            InnerJoin<Vendor, On<APInvoice.vendorID, Equal<Vendor.bAccountID>>,
            InnerJoin<GVOrderTypeMapping, On<GVOrderTypeMapping.orderTypeCD, Equal<APInvoice.docType>,
                And<GVOrderTypeMapping.guiSubType, Equal<GuiSubTypeAttribute.iInv>>>
                >>,
            Where<APInvoice.released, Equal<True>,
                And<APInvoice.docType, Equal<APDocType.invoice>,
                    And<Where<APRegisterGVExt.salesAmtTotal, IsNull,
                        Or<APInvoice.curyOrigDocAmt, Greater<APRegisterGVExt.salesAmtTotal>>>>>>> AddItemDetailView;
        //APInvoice.curyVatTaxableTotal
        /*
        public PXSelectJoinGroupBy<APInvoice,
            InnerJoin<Vendor, On<APInvoice.vendorID, Equal<Vendor.bAccountID>>,
            InnerJoin<GVOrderTypeMapping, On<GVOrderTypeMapping.orderTypeCD, Equal<APInvoice.docType>,
                And<GVOrderTypeMapping.guiSubType, Equal<GuiSubTypeAttribute.iInv>>>,
            LeftJoin<GVApGuiInvoiceDetail ,On<GVApGuiInvoiceDetail.apInvoiceNbr, Equal<APInvoice.refNbr>>>
                >>,
            Where<APInvoice.released, Equal<True>,
                And<APInvoice.docType, Equal<APDocType.invoice>,
                    And<Where<GVApGuiInvoiceDetail.salesAmt,IsNull,Or<APInvoice.curyOrigDocAmt,Greater<GVApGuiInvoiceDetail.salesAmt>>>>>>, 
            Aggregate<GroupBy<GVApGuiInvoiceDetail.apInvoiceNbr, Sum<GVApGuiInvoiceDetail.salesAmt>>>> AddItemDetailView;*/
        // The delegate for the AddItemDetailView data view
        protected virtual IEnumerable addItemDetailView()
        {
            GVAPInvCriteriaFilter filter = AddItemMasterView.Current;
            int startRow = PXView.StartRow;
            int totalRows = 0;
            BqlCommand query = AddItemDetailView.View.BqlSelect;
            FilterInvoices(ref query, filter);
            PXView accountView = new PXView(this, true, query);
            var list = accountView.Select(
                PXView.Currents, PXView.Parameters, PXView.Searches,
                PXView.SortColumns, PXView.Descendings, PXView.Filters,
                ref startRow, PXView.MaximumRows, ref totalRows);
            PXView.StartRow = 0;
            return list;
        }
        private void FilterInvoices(ref BqlCommand query, GVAPInvCriteriaFilter filter)
        {
            // Filter Nbr
            if (filter.APInvNbrFrom != null && filter.APInvNbrTo != null)
            {
                if (filter.APInvNbrFrom.CompareTo(filter.APInvNbrTo) < 0)
                {
                    query = query.WhereAnd<
                        Where<APInvoice.refNbr,
                            Between<Current<GVAPInvCriteriaFilter.aPInvNbrFrom>, Current<GVAPInvCriteriaFilter.aPInvNbrTo>>>>();
                }
                else
                {
                    query = query.WhereAnd<
                     Where<APInvoice.refNbr,
                         Between<Current<GVAPInvCriteriaFilter.aPInvNbrTo>, Current<GVAPInvCriteriaFilter.aPInvNbrFrom>>>>();
                }
            }
            else
            {
                if (filter.APInvNbrFrom != null)
                {
                    query = query.WhereAnd<
                        Where<APInvoice.refNbr, Equal<Current<GVAPInvCriteriaFilter.aPInvNbrFrom>>>>();
                }
                if (filter.APInvNbrTo != null)
                {
                    query = query.WhereAnd<
                        Where<APInvoice.refNbr, Equal<Current<GVAPInvCriteriaFilter.aPInvNbrTo>>>>();
                }
            }

            // filter date range
            if (filter.APInvDateFrom != null)
            {
                query = query.WhereAnd<
                    Where<APInvoice.docDate, GreaterEqual<Current<GVAPInvCriteriaFilter.aPInvDateFrom>>>>();
            }
            if (filter.APInvDateTo != null)
            {
                query = query.WhereAnd<
                    Where<APInvoice.docDate, LessEqual<Current<GVAPInvCriteriaFilter.aPInvDateTo>>>>();
            }

            // filter by Vedor
            if (filter.VendorID != null)
            {
                query = query.WhereAnd<
                        Where<Vendor.bAccountID, Equal<Current<GVAPInvCriteriaFilter.vendorID>>>>();
            }
            // TODO: filter exist InvoiceNrf
        }
        public PXAction<GVApGuiInvoice> AddItem;
        [PXButton(Tooltip = "Add Invoices")]
        [PXUIField(DisplayName = "Add Item", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Visible = true, Enabled = false)]
        public IEnumerable addItem(PXAdapter adapter)
        {
            //your code here
            if (this.AddItemMasterView.AskExt(true) == WebDialogResult.OK)
            {


                foreach (APRegister record in AddItemDetailView.Cache.Cached)
                {
                    if (record.Selected != true)
                    {
                        continue;
                    }

                    PXSelectBase<APTran> query = new PXSelectJoin<APTran,
                    InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<APTran.refNbr>,
                        And<APTran.tranType, Equal<APInvoice.docType>>>,
                    LeftJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>>>,
                    Where<APTran.refNbr, Equal<Required<GVApGuiInvoiceLine.aPInvoiceNbr>>>>(this);
                    APRegisterGVExt registerExt = PXCache<APRegister>.GetExtension<APRegisterGVExt>(record);
                    int i = 0;
                    foreach (PXResult<APTran, APInvoice, CurrencyInfo> tempData in query.Select(record.RefNbr))
                    {
                        i++;
                        APTran apTran = (APTran)tempData;
                        APInvoice apInvoice = (APInvoice)tempData;
                        CurrencyInfo currencyInfo = (CurrencyInfo)tempData;

                        GVApGuiInvoiceDetail detail = new GVApGuiInvoiceDetail();
                        detail.APRefNbr = record.RefNbr;

                        //detail.GuiInvoiceID = guiInvoice.GuiInvoiceID;
                        // ?? sequence ??
                        //detail.LineNumber = i;
                        //detail.OriSalesAmt = apTran.CuryTranAmt;
                        //detail.OriTaxAmt = apTran.CuryTaxAmt;
                        detail.SalesAmt = apTran.CuryTranAmt;
                        detail.TaxAmt = apTran.CuryTaxAmt;
                        detail.Qty = apTran.Qty;
                        detail.Uom = apTran.UOM != null ? apTran.UOM : null;
                        detail.UnitPrice = apTran.CuryUnitCost;
                        detail.ItemDesc = apTran.TranDesc;
                        //detail.OriCurrency = apInvoice.CuryID;
                        //detail.Vendor = apInvoice.VendorID;
                        detail.TotalSalesAmt = apInvoice.CuryOrigDocAmt;
                        detail.SalesAmtTotal = registerExt.SalesAmtTotal;
                        //if (currencyInfo != null)
                        //{
                        //    detail.ExchangeRate = currencyInfo.CuryRate;
                        //}

                        InvoiceDetails.Update(detail);

                    }

                }

                //AddItemDetailView.Cache.Clear();
                //AddItemMasterView.Cache.Clear();*/
            }
            return adapter.Get();
        }

        public PXAction<GVApGuiInvoice> addItemOK;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable AddItemOK(PXAdapter adapter)
        {
            AddItemMasterView.View.Answer = WebDialogResult.OK;
            return addItem(adapter);
        }

        public PXFilter<GVApInvFilter> VoidInvoicePanel;
        #endregion

        #region Select
        public PXSelect<GVApGuiInvoice> Invoice;

        public PXSelect<APRegister, Where<APRegister.refNbr, Equal<Null>>> _HiddenRegister;
        public PXSelect<Vendor, Where<Vendor.acctCD, Equal<Null>>> _HiddenVendor;
        /*
        public PXSelectJoin<GVApGuiInvoiceLine,
            InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<GVApGuiInvoiceLine.aPInvoiceNbr>>,
            InnerJoin<Vendor, On<APInvoice.vendorID, Equal<Vendor.bAccountID>>>>,
            Where<GVApGuiInvoiceLine.guiInvoiceID, Equal<Current<GVApGuiInvoice.guiInvoiceID>>>,
            OrderBy<Asc<APInvoice.refNbr>>> InvoiceLines;*/

        // 不可以與 Line 關聯,會造成在 Persist 丟例外時 Detail 不見
        public PXSelect<GVApGuiInvoiceDetail,
            Where<GVApGuiInvoiceDetail.guiInvoiceID,
                Equal<Current<GVApGuiInvoice.guiInvoiceID>>>> InvoiceDetails;

        public GVApInvoiceEntry()
        {
            //Invoice.AllowInsert = false;
            //Invoice.AllowDelete = false;
        }
        #endregion

        #region Events

        public override void Persist()
        {
            Boolean invalid = true;
            int insertCount = 0;
            GVApGuiInvoice record = (GVApGuiInvoice)Invoice.Current;
            PXEntryStatus status = Invoice.Cache.GetStatus(record);

            if (status == PXEntryStatus.Inserted)
            {
                if (!ValidateGVApGuiInvoice(Invoice.Cache, record))
                {
                    invalid = invalid && false;
                }
                foreach (GVApGuiInvoiceDetail detail in InvoiceDetails.Cache.Inserted)
                {
                    insertCount += 1;
                    if (!ValidateGVApGuiInvoiceDetail(InvoiceDetails.Cache, detail))
                    {
                        invalid = invalid && false;
                    }
                }

                if (!invalid)
                {
                    throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
                }
                if (insertCount == 0)
                {
                    throw new PXSetPropertyException("Please input at least one line!");
                }
            }

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                if (record != null && status == PXEntryStatus.Inserted)
                {

                    if (record.GroupRemark == GroupRemarkAttribute.Summary && record.GroupCnt == 0)
                    {
                        throw new PXException(string.Format("Group count can't be zero when group remark equal to {0}", GroupRemarkAttribute.SummaryName));
                    }
                    //補Line刪除
                    /*
                    foreach (GVApGuiInvoiceDetail detail in InvoiceDetails.Cache.Inserted)
                    {
                        GVApGuiInvoiceLine li = new GVApGuiInvoiceLine();
                        li.APInvoiceNbr = detail.ApInvoiceNbr;
                        li.GuiInvoiceID = record.GuiInvoiceID;

                        InvoiceLines.Insert(li);
                        //只要帶入 GuiInvoiceID 就會有 LineID
                        detail.GuiInvoiceLineID = li.GuiInvoiceID;
                    }*/
                    // ? count Detail or Line
                    base.Persist();
                    foreach (GVApGuiInvoiceDetail detail in InvoiceDetails.Select())
                    {
                        UpdateApRegister(detail,false);
                    }
                    ts.Complete();
                }
            }
            /*
            if (status == PXEntryStatus.Inserted) { 
                base.Persist();
            }*/
        }
        private const string open = "1";
        public virtual void GVApGuiInvoice_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row == null) return;

            PXEntryStatus status = Invoice.Cache.GetStatus(row);

            PXUIFieldAttribute.SetRequired<GVApGuiInvoice.groupCnt>(sender, row.GroupRemark == GroupRemarkAttribute.Summary);

            UIReadOnly(sender, row, status != PXEntryStatus.Inserted);

            if (status == PXEntryStatus.Inserted ||
                row.VoidDate != null)
            {
                VoidButton.SetVisible(false); 
            }
            else
            {
                VoidButton.SetVisible(true);
            }

            if (!(row.Status == null || open.Equals(row.Status)))
            {
                VoidButton.SetEnabled(false);
            }
            else
            {
                VoidButton.SetEnabled(true);
            }
        }
        protected virtual void GVApGuiInvoice_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row.GuiInvoiceID < 0)
            {
                row.GuiInvoiceNbr = "<NEW>";

            }

        }
        protected virtual void GVApGuiInvoice_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            GVApGuiInvoice invoice = (GVApGuiInvoice)e.Row;
            if (invoice == null) return;
        }


        protected virtual void GVApGuiInvoice_DeclareYear_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            DateTime dt = (DateTime)row.InvoiceDate;
            cache.SetValue<GVApGuiInvoice.declareYear>(e.Row, dt.Year);
            cache.SetValue<GVApGuiInvoice.declareMonth>(e.Row, dt.Month);
        }

        protected virtual void GVApGuiInvoice_DeclareYear_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVApGuiInvoice row = e.Row as GVApGuiInvoice;
            if (e.NewValue == null) return;
            int year = (int)e.NewValue;
            CheckDeclareYearMonth(sender, row, year, (int)row.DeclareMonth);
        }

        protected virtual void GVApGuiInvoice_DeclareMonth_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVApGuiInvoice row = e.Row as GVApGuiInvoice;
            if (e.NewValue == null) return;
            int month = (int)e.NewValue;

            CheckDeclareYearMonth(sender, row, (int)row.DeclareYear, month);
        }

        protected virtual void GVApGuiInvoice_Vendor_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row.Vendor == null) return;
            Location location = PXSelectReadonly<Location,
                Where<Location.bAccountID, Equal<Required<GVApGuiInvoice.vendor>>>,
                OrderBy<Desc<GVGuiBook.endNum>>>.Select(this, row.Vendor);

            if (location != null)
            {
                row.VendorUniformNumber = location.TaxRegistrationID;
            }
        }

        protected virtual void GVApGuiInvoice_InvoiceDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVApGuiInvoice row = (GVApGuiInvoice)e.Row;
            if (row.InvoiceDate == null) return;

            // DeclareYear and DeclareMonth are dependent to InvoiceDate
            DateTime invoiceDate = (DateTime)row.InvoiceDate;
            row.DeclareYear = invoiceDate.Year;
            row.DeclareMonth = invoiceDate.Month;
        }

        protected virtual void GVApGuiInvoice_GroupRemark_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVApGuiInvoice row = e.Row as GVApGuiInvoice;
            if (row.GroupRemark == null) return;
            PXUIFieldAttribute.SetRequired<GVApGuiInvoice.groupCnt>(sender, row.GroupRemark == GroupRemarkAttribute.Summary);
        }

        //protected virtual void GVApGuiInvoice_LatestLineNo_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        //{
        //    GVApGuiInvoice row = e.Row as GVApGuiInvoice;

        //    if (row.LatestLineNo == null)
        //    {
        //        row.LatestLineNo = 0;
        //    }
        //}

        protected virtual void GVApGuiInvoiceDetail_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            GVApGuiInvoiceDetail detail = (GVApGuiInvoiceDetail)e.Row;
            GVApGuiInvoice row = Invoice.Current;


            Console.WriteLine(row.LatestLineNo);
            //Invoice.Cache.Update(row);
        }

        protected virtual void GVApGuiInvoiceDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            GVApGuiInvoiceDetail detail = (GVApGuiInvoiceDetail)e.Row;
            GVApGuiInvoice row = Invoice.Current;

            //GVApGuiInvoiceLine li = new GVApGuiInvoiceLine();
            //li.APInvoiceNbr = detail.ApInvoiceNbr;
            //li.GuiInvoiceID = row.GuiInvoiceID;

            //InvoiceLines.Insert(li);
            //InvoiceLines.Cache.Current = li;
            ////只要帶入 GuiInvoiceID 就會有 LineID
            //detail.GuiInvoiceLineID = li.GuiInvoiceID;

            //row.LatestLineNo += 1;
            //Invoice.Cache.Update(row);
        }

        protected virtual void GVApGuiInvoiceDetail_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            GVApGuiInvoiceDetail line = (GVApGuiInvoiceDetail)e.Row;
            // Checking whether the deletion has been initiated from the UI
            if (!e.ExternalCall) return;
            if (InvoiceDetails.Ask("Confirm Delete", "Are you sure?", MessageButtons.YesNo) != WebDialogResult.Yes)
            {
                e.Cancel = true;
            }

            GVApGuiInvoiceDetail detail = (GVApGuiInvoiceDetail)e.Row;
            GVApGuiInvoice order = Invoice.Current;
            PXEntryStatus orderStatus = Invoice.Cache.GetStatus(order);
            bool isDeleted = orderStatus == PXEntryStatus.InsertedDeleted ||
                             orderStatus == PXEntryStatus.Deleted;
            if (isDeleted) return;

            // Asking for confirmation on an attempt to delete a
            // shipment line other than the gift card line


            PXResultset<GVApGuiInvoiceDetail> set = PXSelect<GVApGuiInvoiceDetail,
                    Where<GVApGuiInvoiceDetail.apRefNbr,
                    Equal<Required<GVApGuiInvoiceDetail.apRefNbr>>>>
                    .Select(this, detail.APRefNbr);
            foreach (GVApGuiInvoiceDetail nbr in set)
            {
                if (nbr.APRefNbr == detail.APRefNbr)
                {
                    InvoiceDetails.Delete(nbr);
                    InvoiceDetails.View.RequestRefresh();
                }
            }
            bool isUpdated = false;
            if (detail.SalesAmt != null)
            {
                order.SalesAmt -= detail.SalesAmt;
                isUpdated = true;
            }
            if (detail.TaxAmt != null)
            {
                order.TaxAmt -= detail.TaxAmt;
                isUpdated = true;
            }

            if (isUpdated) { Invoice.Update(order); }
            //CaclAmountTotal();
        }

        protected virtual void GVApGuiInvoiceDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            GVApGuiInvoiceDetail detail = (GVApGuiInvoiceDetail)e.Row;
            GVApGuiInvoice row = Invoice.Current;

            //row.LatestLineNo -= 1;
            //Invoice.Cache.Update(row);
        }
        protected virtual void GVApGuiInvoiceDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            Console.WriteLine("GVApGuiInvoiceDetail_RowSelected: {0}", row.APRefNbr);
            //PXEntryStatus status = InvoiceDetails.Cache.GetStatus(row);
        }

        protected virtual void GVApGuiInvoiceDetail_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            GVApGuiInvoiceDetail detail = (GVApGuiInvoiceDetail)e.NewRow;
            GVApGuiInvoiceDetail originalDetail = (GVApGuiInvoiceDetail)e.Row;
            if (!sender.ObjectsEqual<GVApGuiInvoiceDetail.unitPrice, GVApGuiInvoiceDetail.qty, GVApGuiInvoiceDetail.taxAmt>(detail, originalDetail))
            {
                if (detail.UnitPrice != null & detail.Qty != null)
                {
                    detail.SalesAmt = (decimal)detail.UnitPrice * detail.Qty;
                }
                CaclAmountTotal();
            }
        }

        protected virtual void GVApGuiInvoiceDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            Console.WriteLine("GVApGuiInvoiceDetail_RowSelected: {0}", row.APRefNbr);
        }

        protected virtual void GVApGuiInvoiceDetail_LineNo_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            GVApGuiInvoice invoice = (GVApGuiInvoice)Invoice.Cache.Current;
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            //row.LineNo = invoice.LatestLineNo + 1;
        }


        protected virtual void GVApGuiInvoiceDetail_ApInvoiceNbr_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            GVApGuiInvoice guiInvoice = (GVApGuiInvoice)Invoice.Cache.Current;
            GVApGuiInvoiceDetail row = (GVApGuiInvoiceDetail)e.Row;
            if (e.NewValue == null) return;


            PXSelectBase<APTran> query = new PXSelectJoin<APTran,
                    InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTran.refNbr>,
                        And<APTran.tranType, Equal<APRegister.docType>>>,
                    InnerJoin<Vendor, On<APRegister.vendorID, Equal<Vendor.bAccountID>>,
                    LeftJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APRegister.curyInfoID>>>>>,
                    Where<APTran.refNbr, Equal<Required<GVApGuiInvoiceDetail.apRefNbr>>,
                    And<APRegister.docType, Equal<APDocType.invoice>>>>(this);

            PXResult<APTran, APRegister, Vendor, CurrencyInfo> record = (PXResult<APTran, APRegister, Vendor, CurrencyInfo>)query.Select(e.NewValue);

            if (record != null)
            {
                
                APTran apTran = (APTran)record;
                APRegister register = (APRegister)record;
                //APInvoice invoice = (APInvoice)record;
                Vendor vendor = (Vendor)record;
                CurrencyInfo currencyInfo = (CurrencyInfo)record;
                // 取得客制化欄位
                APRegisterGVExt registerExt = PXCache<APRegister>.GetExtension<APRegisterGVExt>(register);

                if (registerExt.SalesAmtTotal != null && registerExt.SalesAmtTotal >= register.CuryOrigDocAmt) {
                    sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.apRefNbr>(row, row.APRefNbr,
                                   new PXSetPropertyException("APInvoiceNbr is used and don't have any overage, please check!", PXErrorLevel.Error));
                    e.Cancel = true;
                    return;
                }

                if (guiInvoice.Vendor != vendor.BAccountID)
                {
                    sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.apRefNbr>(row, row.APRefNbr,
                                   new PXSetPropertyException("Vendor not match APInvoice vendor, please check!", PXErrorLevel.Error));
                    e.Cancel = true;
                    return;
                }
               
                //row.OriSalesAmt = apTran.CuryTranAmt;
                row.TotalSalesAmt = register.CuryOrigDocAmt;
                row.SalesAmtTotal = registerExt.SalesAmtTotal;
                //row.OriTaxAmt = apTran.CuryTaxAmt;
                row.SalesAmt = apTran.CuryTranAmt;
                row.TaxAmt = apTran.CuryTaxAmt;
                row.Qty = apTran.Qty;
                row.Uom = apTran.UOM != null ? apTran.UOM : null;
                row.UnitPrice = apTran.CuryUnitCost;
                row.ItemDesc = apTran.TranDesc;
                //row.OriCurrency = register.CuryID;

                //if (currencyInfo != null)
                //{
                //    row.ExchangeRate = currencyInfo.CuryRate;
                //}
                //InvoiceDetails.Update(row);
                PXUIFieldAttribute.SetReadOnly<GVApGuiInvoiceDetail.unitPrice>(sender, row, true);
                PXUIFieldAttribute.SetReadOnly<GVApGuiInvoiceDetail.qty>(sender, row, true);
                PXUIFieldAttribute.SetReadOnly<GVApGuiInvoiceDetail.itemDesc>(sender, row, true);
                PXUIFieldAttribute.SetReadOnly<GVApGuiInvoiceDetail.salesAmt>(sender, row, true);
                PXUIFieldAttribute.SetReadOnly<GVApGuiInvoiceDetail.taxAmt>(sender, row, true);
                PXUIFieldAttribute.SetReadOnly<GVApGuiInvoiceDetail.uom>(sender, row, true);
            }
        }
        #endregion

        #region AddItemMasterView Events

        protected virtual void GVAPInvCriteriaFilter_VendorID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            GVAPInvCriteriaFilter row = e.Row as GVAPInvCriteriaFilter;
            GVApGuiInvoice invoice = (GVApGuiInvoice)Invoice.Cache.Current;
            if (invoice == null) return;
            if (invoice.Vendor == null) return;
            sender.SetValue<GVAPInvCriteriaFilter.vendorID>(e.Row, invoice.Vendor);
        }

        protected virtual void GVAPInvCriteriaFilter_APInvDateFrom_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            GVAPInvCriteriaFilter row = e.Row as GVAPInvCriteriaFilter;
            row.APInvDateFrom = DateTime.Now.AddDays(-(DateTime.Now.Day - 1));
        }

        protected virtual void GVAPInvCriteriaFilter_APInvDateTo_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            GVAPInvCriteriaFilter row = e.Row as GVAPInvCriteriaFilter;
            row.APInvDateTo = DateTime.Now;
        }
        protected virtual void GVAPInvCriteriaFilter_APInvDateFrom_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVAPInvCriteriaFilter row = e.Row as GVAPInvCriteriaFilter;
            if (row.APInvDateTo == null) return;
            if (e.NewValue == null) return;

            DateTime dateStart = (DateTime)e.NewValue;
            if (dateStart > row.APInvDateTo)
                // Throwing an exception to show the error on the field
                throw new PXSetPropertyException("Start date must be less than or equal to End Date.");
        }

        protected virtual void GVAPInvCriteriaFilter_APInvDateTo_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVAPInvCriteriaFilter row = e.Row as GVAPInvCriteriaFilter;
            if (row.APInvDateFrom == null) return;
            if (e.NewValue == null) return;

            DateTime dateEnd = (DateTime)e.NewValue;
            if (dateEnd < row.APInvDateFrom)
                // Throwing an exception to show the error on the field
                throw new PXSetPropertyException("End date must be greater than or equal to Start Date.");

        }
        #endregion

        #region GVApInvFilter Events
        protected virtual void GVApInvFilter_VoidReason_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            GVApInvFilter row = (GVApInvFilter)e.Row;
            if (e.NewValue == null) return;
            if (row == null) return;
        }
        #endregion

        #region Methods

        private void UIReadOnly(PXCache sender, GVApGuiInvoice row, bool isReadOnly)
        {
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.registrationCD>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.vendor>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.invoiceDate>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.voucherCategory>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.taxCode>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.deductionCode>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.invoiceType>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.guiType>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.groupCnt>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.groupRemark>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.remark>(sender, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<GVApGuiInvoice.taxAmt>(sender, row, isReadOnly);
            Invoice.Cache.AllowDelete = !isReadOnly;

            InvoiceDetails.Cache.AllowInsert = !isReadOnly;
            InvoiceDetails.Cache.AllowDelete = !isReadOnly;
            InvoiceDetails.Cache.AllowUpdate = !isReadOnly;
        }

        /// <summary>
		/// Update APRegister
		/// </summary>
		private void UpdateApRegister(GVApGuiInvoiceDetail guiDetail,bool isvoid)
        {
            PXResultset<APRegister> resultSet = PXSelect<APRegister,
                                                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                                                .Select(this, guiDetail.APRefNbr);
            GVApGuiInvoice master = Invoice.Current as GVApGuiInvoice;
            foreach (APRegister register in resultSet)
            {

                
                String usrApGuiInvoiceNbrStr = reWriteAPRegisterStr(guiDetail.APRefNbr, isvoid) ;

                PXUpdate<
                Set<APRegisterGVExt.usrApGuiInvoiceNbr, Required<APRegisterGVExt.usrApGuiInvoiceNbr>>,
                APRegister,
                    Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>,
                    And<APRegister.docType, Equal<APDocType.invoice>>>>
                    .Update(this, usrApGuiInvoiceNbrStr, register.RefNbr);
            }
        }
        public String reWriteAPRegisterStr(String refNbr,bool isVoid)
        {
            PXSelectBase<GVApGuiInvoiceDetail> query = new PXSelectJoin<GVApGuiInvoiceDetail,
                    InnerJoin<GVApGuiInvoice, On<GVApGuiInvoiceDetail.guiInvoiceID, Equal<GVApGuiInvoice.guiInvoiceID>>>,
                    Where<GVApGuiInvoiceDetail.apRefNbr, Equal<Required<GVApGuiInvoiceDetail.apRefNbr>>>
                    >(this);
            GVApGuiInvoice master = Invoice.Current as GVApGuiInvoice;
            HashSet<String> set = new HashSet<String>();
            //set.Add(master.GuiInvoiceNbr);
            foreach (PXResult<GVApGuiInvoiceDetail, GVApGuiInvoice> tempData in query.Select(refNbr))
            {
                GVApGuiInvoice tempMaster = (GVApGuiInvoice)tempData;
                set.Add(tempMaster.GuiInvoiceNbr);
            }
            String usrApGuiInvoiceNbrStr = "";
            if (isVoid)
            {
                usrApGuiInvoiceNbrStr = "";

            }
            
            
            foreach (String tempData in set)
            {
                if ("".Equals(usrApGuiInvoiceNbrStr)) {
                    usrApGuiInvoiceNbrStr = tempData;
                }
                else {
                    usrApGuiInvoiceNbrStr = usrApGuiInvoiceNbrStr + "/" + tempData;
                } 
            }
            return usrApGuiInvoiceNbrStr;
        }


        public void CaclAmountTotal()
        {
            PXEntryStatus status = Invoice.Cache.GetStatus(Invoice.Current);

            if (status == PXEntryStatus.Inserted)
            {

                GVApGuiInvoice master = Invoice.Current as GVApGuiInvoice;
                Decimal salesAmount = 0;
                Decimal taxAmount = 0;

                foreach (GVApGuiInvoiceDetail detail in InvoiceDetails.Cache.Inserted)
                {
                    salesAmount += (Decimal)detail.SalesAmt;
                    taxAmount += (Decimal)detail.TaxAmt;
                }
                master.SalesAmt = salesAmount;
                master.TaxAmt = taxAmount;
                Invoice.Update(master);
            }
        }

        private void CheckDeclareYearMonth(PXCache sender, GVApGuiInvoice row, int year, int month)
        {
            if (year == 0)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoice.declareYear>(row, row.DeclareYear,
                       new PXSetPropertyException("Year can't be zero.", PXErrorLevel.Error));
                return;
            }

            if (month == 0)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoice.declareMonth>(row, row.DeclareMonth,
                       new PXSetPropertyException("Month can't be zero.", PXErrorLevel.Error));
                return;
            }

            DateTime invoiceDT = (DateTime)row.InvoiceDate;
            if (invoiceDT.Year > year)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoice.declareYear>(row, row.DeclareYear,
                       new PXSetPropertyException(string.Format("Year can not less than invoice year {0:d}.", invoiceDT.Year), PXErrorLevel.Error));
                return;
            }
            DateTime dateTime = DateTime.ParseExact(string.Format("{0:d2}/{1:d2}/{2}", invoiceDT.Day, month, year),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture);

            if (invoiceDT.CompareTo(dateTime) > 0)
            {
                // Throwing an exception to show the error on the field
                sender.RaiseExceptionHandling<GVApGuiInvoice.declareMonth>(row, row.DeclareMonth,
                       new PXSetPropertyException(string.Format("Month can not less than invoice month {0:d}.", invoiceDT.Month), PXErrorLevel.Error));
            }
        }

        /// <summary>
        /// 驗證 GVApGuiInvoice
        /// </summary>
        /// <param name="sender"><see cref="PX.Data.PXCache"/></param>
        /// <param name="row"><see cref="RCGV.GV.DAC.GVApGuiInvoice"/></param>
        /// <returns>Boolean</returns>
        private Boolean ValidateGVApGuiInvoice(PXCache sender, GVApGuiInvoice row)
        {
            Boolean valid = true;

            if (row.InvoiceType == "I")
            {
                if (row.GuiInvoiceNbr.Length != 10 ||
                    !Regex.Match(row.GuiInvoiceNbr, "^[A-Z]{2}[0-9]{8}").Success)
                {
                    sender.RaiseExceptionHandling<GVApGuiInvoice.guiInvoiceNbr>(row, row.GuiInvoiceNbr,
                        new PXSetPropertyException("GuiInvoiceNbr format error!", PXErrorLevel.Error));
                    valid = false;
                }
            }
            if (row.GroupRemark == GroupRemarkAttribute.Summary && row.GroupCnt == 0)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoice.groupCnt>(row, row.GroupCnt,
                        new PXSetPropertyException(string.Format("Group count can't be zero when group remark equal to {0}", GroupRemarkAttribute.SummaryName), PXErrorLevel.Error));
                valid = false;
            }

            if (row.VendorUniformNumber == null)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoice.vendorUniformNumber>(row, row.VendorUniformNumber,
                               new PXSetPropertyException("VendorUniformNumber can not be empty!", PXErrorLevel.Error));
                valid = false;
            }
            else if (row.VendorUniformNumber.Length != 8)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoice.vendorUniformNumber>(row, row.VendorUniformNumber,
                               new PXSetPropertyException("VendorUniformNumber format error!", PXErrorLevel.Error));
                valid = false;
            }
         
                return valid;
        }

        private Boolean ValidateGVApGuiInvoiceDetail(PXCache sender, GVApGuiInvoiceDetail row)
        {
            GVApGuiInvoice master = Invoice.Current as GVApGuiInvoice;
            Boolean valid = true;
            if (String.IsNullOrWhiteSpace(row.ItemDesc))
            {
                sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.itemDesc>(
                        row, row.ItemDesc,
                          new PXSetPropertyException(
                          "ItemDesc can't be blank",
                          PXErrorLevel.Error));
                valid = false;
            }
            if (String.IsNullOrWhiteSpace(row.Uom))
            {
                sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.uom>(
                        row, row.Uom,
                          new PXSetPropertyException(
                          "Unit can't be blank",
                          PXErrorLevel.Error));
                valid = false;
            }

            if (row.Qty <= 0)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.qty>(
                        row, row.Qty,
                          new PXSetPropertyException(
                          "Quantity can't be zero",
                          PXErrorLevel.Error));
                valid = false;
            }

            if (row.UnitPrice <= 0)
            {
                sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.unitPrice>(
                        row, row.UnitPrice,
                          new PXSetPropertyException(
                          "UnitPrice can't be zero",
                          PXErrorLevel.Error));
                valid = false;
            }
            //1 就是應稅
            if (master.TaxCode.Equals("1"))
            {
                if (row.TaxAmt <= 0)
                {
                    sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.taxAmt>(
                            row, row.TaxAmt,
                              new PXSetPropertyException(
                              "TaxAmt can't be zero",
                              PXErrorLevel.Error));
                    valid = false;
                }
            }
            //if (row.APRefNbr != null && master.Vendor != null)
            //{
            //    //ARInvoice arInvoice = getARInvoice(line.ArInvoiceNbr);
            //    if (!master.Vendor.Equals(row.Vendor))
            //    {
            //        sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.vendor>(row, row.Vendor, new PXSetPropertyException("Vendor is not equal with ArInvoice, please check!"));
            //        valid = false;
            //    }
            //}
            //if (row.APRefNbr != null && row.SalesAmt > row.OriSalesAmt)
            //{
            //    sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.salesAmt>(row, row.SalesAmt, new PXSetPropertyException("SalesAmt have to smaller than oriSalesAmt from ApInvoice."));
            //    valid = false;
            //}
            //檢查金額 -Begin

            //curyOrigDocAmt
            decimal currentSalesAmt = 0;
            decimal groupbySalesAmt = 0;
            foreach (GVApGuiInvoiceDetail detail in InvoiceDetails.Cache.Inserted)
            {
                if (row.APRefNbr == detail.APRefNbr)
                {
                    currentSalesAmt = currentSalesAmt + (decimal)detail.SalesAmt;
                }
            }
            if (row.SalesAmtTotal == null){
                groupbySalesAmt = 0;
            }
            else {
                groupbySalesAmt =(decimal) row.SalesAmtTotal;
            }

            if ((groupbySalesAmt + currentSalesAmt) > row.TotalSalesAmt) {
                sender.RaiseExceptionHandling<GVApGuiInvoiceDetail.apRefNbr>(row, row.APRefNbr, new PXSetPropertyException("SalesAmt have to smaller than total oriSalesAmt from ApInvoice."));
                valid = false;
            }

            //檢查金額 -End
            return valid;
        }

        public void basePersist()
        {
            base.Persist();
        }
        private void VoidGVApInvoice(GVApGuiInvoice gvApInvoice, GVApInvFilter voidFilter)
        {
            GVApGuiInvoice master = Invoice.Current as GVApGuiInvoice;
            Console.WriteLine(gvApInvoice.VoidReason);
            GVApInvoiceEntry graph = PXGraph.CreateInstance<GVApInvoiceEntry>();
            //using (PXTransactionScope ts = new PXTransactionScope())
            //{
            gvApInvoice.VoidDate = DateTime.Now;
            //gvApInvoice.TaxCode = "F";
            gvApInvoice.Status = "2";
            gvApInvoice.VoidReason = voidFilter.VoidReason;
            graph.Invoice.Update(gvApInvoice);
            graph.Invoice.Current = gvApInvoice;
            graph.basePersist();
            foreach (GVApGuiInvoiceDetail detail in InvoiceDetails.Select())
            {
                UpdateApRegister(detail ,true);
            }
            base.Persist();
            //    ts.Complete();
            //}
        }
        #endregion
    }
    [Serializable]
    public class GVApInvFilter : IBqlTable
    {
        #region VoidDesc
        public abstract class voidDesc : PX.Data.IBqlField
        {
        }
        protected string _VoidReason;
        [PXString(240, IsUnicode = true)]
        [PXUIField(DisplayName = "Reason", Required = true)]
        public virtual string VoidReason
        {
            get
            {
                return this._VoidReason;
            }
            set
            {
                this._VoidReason = value;
            }
        }
        #endregion
    }


}