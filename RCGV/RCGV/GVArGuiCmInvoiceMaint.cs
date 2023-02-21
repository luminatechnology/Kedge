using System;
using System.Collections;
using PX.Data;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using PX.Objects.CR;
using RCGV.GV.SYS;
using RC.Util;
using PX.Objects.AR;
using System.Collections.Generic;


/***
 * ====2021-06-17:12101====Alton
 * 	1.明細請新增TaxAmt, 使用者輸入SalesAmt, 請自動計算5%稅額到TaxAmt, 使用者可以微調
 * 	2.標頭的TaxAmt請自動加總明細的TaxAmt
 * 	====2021-10-13:12255====Alton
 * 1.開放 GVArGuiInvoice.CustUniformNumber 客戶統一編號可以修改.
 * 2.選取發票的LOV追加CustUniformNumber 條件
 * 
 * ====2021-12-07:口頭:Alton
 * 折讓單列印判斷取消BatchNbr判斷
 * **/
namespace RCGV.GV
{
    public class GVArGuiCmInvoiceMaint : PXGraph<GVArGuiCmInvoiceMaint, GVArGuiCmInvoice>
    {

        #region View
        public PXSelect<GVArGuiCmInvoice> gvArGuiInvoices;
        public PXSelect<GVArGuiCmInvoiceLine,
                                Where<GVArGuiCmInvoiceLine.guiCmInvoiceID,
                                Equal<Current<GVArGuiCmInvoice.guiCmInvoiceID>>>> gvArGuiInvoiceLines;
        [PXCopyPasteHiddenView]
        public PXFilter<GVARInvFilter> addFilter;

        [PXCopyPasteHiddenView]
        public PXSelect<ARInvoiceNbrLOV,
              Where<ARInvoiceNbrLOV.customerID, Equal<Current<GVArGuiCmInvoice.customerID>>,
                  And<ARInvoiceNbrLOV.custUniformNumber, Equal<Current<GVArGuiCmInvoice.custUniformNumber>>>>> GvArInvoiceItems;
        #endregion

        #region Action
        #region Button
        #region Void
        public PXAction<GVArGuiCmInvoice> VoidBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Void")]
        public virtual IEnumerable voidBtn(PXAdapter adapter)
        {
            GVARInvFilter arFLT = addFilter.Current;
            if (addFilter.AskExt(true) == WebDialogResult.OK)
            {
                if (String.IsNullOrEmpty(arFLT.VoidDesc))
                {
                    throw new PXException("Void reason is require.");
                }
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    GVArGuiCmInvoice master = gvArGuiInvoices.Current;
                    base.Persist();
                    gvArGuiInvoices.Cache.SetValueExt<GVArGuiCmInvoice.status>(master, GVList.GVStatus.VOIDINVOICE);
                    gvArGuiInvoices.Cache.SetValueExt<GVArGuiCmInvoice.voidReason>(master, arFLT.VoidDesc);
                    gvArGuiInvoices.Cache.SetValueExt<GVArGuiCmInvoice.voidBy>(master, Accessinfo.UserID);
                    gvArGuiInvoices.Cache.SetValueExt<GVArGuiCmInvoice.voidDate>(master, Accessinfo.BusinessDate);
                    gvArGuiInvoices.Update(master);
                    base.Persist();
                    ts.Complete();
                }
            }

            return adapter.Get();
        }
        #endregion

        #region Print
        public PXAction<GVArGuiCmInvoice> PrintBtn;
        [PXButton(Tooltip = "折讓單列印", CommitChanges = true)]
        [PXUIField(DisplayName = "折讓單列印")]
        protected virtual IEnumerable printBtn(PXAdapter adapter)
        {
            GVArGuiCmInvoice invoice = (GVArGuiCmInvoice)gvArGuiInvoices.Cache.Current;
            Dictionary<string, string> patams = new Dictionary<string, string>();
            patams["P_GuiCmInvoiceNbrB"] = invoice.GuiCmInvoiceNbr;
            patams["P_GuiCmInvoiceNbrE"] = invoice.GuiCmInvoiceNbr;
            throw new PXReportRequiredException(patams, "GV602001", "Report");
        }
        #endregion

        #region Select GV Gui Invoice Item
        public PXAction<GVArGuiCmInvoice> SelectGuiInvoiceItem;
        [PXButton(CommitChanges = true, OnClosingPopup = PXSpecialButtonType.Refresh, SpecialType = PXSpecialButtonType.Report)]
        [PXUIField(DisplayName = "Add Gui Invoice")]
        public virtual IEnumerable selectGuiInvoiceItem(PXAdapter adapter)
        {
            WebDialogResult result = GvArInvoiceItems.AskExt(true);
            if (result == WebDialogResult.OK)
            {
                return AddGuiInvoice(adapter);
            }
            GvArInvoiceItems.Cache.Clear();
            return adapter.Get();
        }
        #endregion

        #region AddGuiInvoice
        public PXAction<GVArGuiCmInvoice> addGuiInvoice;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.Report)]
        public virtual IEnumerable AddGuiInvoice(PXAdapter adapter)
        {
            foreach (ARInvoiceNbrLOV item in GvArInvoiceItems.Select())
            {
                if (item.Selected != true) continue;
                GVArGuiCmInvoiceLine line = (GVArGuiCmInvoiceLine)gvArGuiInvoiceLines.Cache.CreateInstance();
                line.GuiInvoiceDetailID = item.GuiInvoiceDetailID;
                line.ArGuiInvoiceNbr = item.GuiInvoiceNbr;
                line.SalesAmt = item.SalesAmt;
                line.UnitPrice = item.SalesAmt;
                line.ItemDesc = item.ItemDesc;
                line.BatchNbr = item.BatchNbr;
                line.RefNbr = item.RefNbr;
                gvArGuiInvoiceLines.Update(line);
            }
            GvArInvoiceItems.Cache.Clear();
            return adapter.Get();
        }
        #endregion
        #endregion

        #endregion

        #region HyperLink
        #region ViewGVInvoice
        public PXAction<GVArGuiCmInvoice> ViewGVInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGVInvoice()
        {
            GVArGuiCmInvoiceLine row = gvArGuiInvoiceLines.Current;

            if (row?.ArGuiInvoiceNbr != null) new HyperLinkUtil<GVArInvoiceEntry>(GetGVInvoice(row.ArGuiInvoiceNbr), true);
        }
        #endregion
        #endregion

        #region Event
        #region GVArGuiCmInvoice
        protected virtual void _(Events.RowSelected<GVArGuiCmInvoice> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row != null) SetUI(row);
        }

        protected virtual void _(Events.RowPersisting<GVArGuiCmInvoice> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            GVPeriod gvPeriod = GetGvPeriod(row.RegistrationCD, row.DeclareYear, row.DeclareMonth);
            if (gvPeriod == null)
                SetError<GVArGuiCmInvoice.invoiceDate>(row, row.InvoiceDate, "發票期別尚未解鎖或沒有設定，無法開立該期別之銷項發票/折讓");

        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiCmInvoice, GVArGuiCmInvoice.declareYear> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiCmInvoice, GVArGuiCmInvoice.declareMonth> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            e.NewValue = row.InvoiceDate?.Month;
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiCmInvoice, GVArGuiCmInvoice.declarePeriod> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            e.NewValue = GVList.GVDeclarePeriod.GetDeclarePeriodKey((int)row.DeclareMonth);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiCmInvoice, GVArGuiCmInvoice.hold> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            bool isHold = row.Hold == true;
            if (isHold)
            {
                e.Cache.SetValueExt<GVArGuiCmInvoice.status>(row, GVList.GVStatus.HOLD);
            }
            else
            {
                e.Cache.SetValueExt<GVArGuiCmInvoice.status>(row, GVList.GVStatus.OPEN);
                e.Cache.SetValueExt<GVArGuiCmInvoice.confirmBy>(row, Accessinfo.UserID);
                e.Cache.SetValueExt<GVArGuiCmInvoice.confirmDate>(row, Accessinfo.BusinessDate);
            }
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiCmInvoice, GVArGuiCmInvoice.invoiceDate> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVArGuiCmInvoice.declareYear>(row);
            e.Cache.SetDefaultExt<GVArGuiCmInvoice.declareMonth>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiCmInvoice, GVArGuiCmInvoice.declareMonth> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVArGuiCmInvoice.declarePeriod>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVArGuiCmInvoice, GVArGuiCmInvoice.customerID> e)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            BAccount bAccount = PXSelectorAttribute.Select<GVArGuiCmInvoice.customerID>(e.Cache, row) as BAccount;
            Location location = GetLocation(bAccount?.DefLocationID);
            e.Cache.SetValueExt<GVArGuiCmInvoice.custUniformNumber>(row, location?.TaxRegistrationID);
            e.Cache.SetValueExt<GVArGuiCmInvoice.custName>(row, bAccount?.AcctName);
            //重新觸發檢核
            foreach (GVArGuiCmInvoiceLine item in gvArGuiInvoiceLines.Select())
            {
                gvArGuiInvoiceLines.Update(item);
            }
        }

        #endregion

        #region GVArGuiCmInvoiceLine
        protected virtual void _(Events.RowPersisting<GVArGuiCmInvoiceLine> e)
        {
            GVArGuiCmInvoiceLine row = (GVArGuiCmInvoiceLine)e.Row;
            GVArGuiCmInvoice master = gvArGuiInvoices.Current;
            if (e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            GVArGuiInvoice invoice = GetGVInvoice(row.ArGuiInvoiceNbr);
            if (invoice?.CustomerID != master.CustomerID)
                SetError<GVArGuiCmInvoiceLine.arGuiInvoiceNbr>(row, row.ArGuiInvoiceNbr, "CustomerID is not mapping");
            if (row.SalesAmt > row.Balance)
                SetError<GVArGuiCmInvoiceLine.salesAmt>(row, row.SalesAmt, "SalesAmt不可大於Balance");
            if (row.CustUniformNumber != master.CustUniformNumber)
                SetError<GVArGuiCmInvoiceLine.arGuiInvoiceNbr>(row, row.ArGuiInvoiceNbr, String.Format("明細統編不一致({0})", row.CustUniformNumber));
        }

        protected virtual void _(Events.FieldDefaulting<GVArGuiCmInvoiceLine, GVArGuiCmInvoiceLine.balance> e)
        {
            GVArGuiCmInvoiceLine row = (GVArGuiCmInvoiceLine)e.Row;
            if (row == null) return;
            decimal usingAmt = 0m;
            foreach (GVArGuiCmInvoiceLine item in GetGVArGuiCmInvoiceLine(row.ArGuiInvoiceNbr, row.GuiCmInvoiceLineID, row.GuiInvoiceDetailID))
            {
                usingAmt += (item.SalesAmt ?? 0m);
            }
            e.NewValue = (row.InvSalesAmt ?? 0m) - usingAmt;

        }

        protected virtual void _(Events.FieldUpdated<GVArGuiCmInvoiceLine, GVArGuiCmInvoiceLine.salesAmt> e)
        {
            GVArGuiCmInvoiceLine row = (GVArGuiCmInvoiceLine)e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<GVArGuiCmInvoiceLine.unitPrice>(row, row.SalesAmt);
            e.Cache.SetValueExt<GVArGuiCmInvoiceLine.taxAmt>(row, round((row.SalesAmt ?? 0m) * 0.05m));
        }
        #endregion

        #endregion

        #region Method
        public Decimal? round(Decimal? num)
        {
            return System.Math.Round(num ?? 0m, 0, MidpointRounding.AwayFromZero);
        }

        public void SetUI(GVArGuiCmInvoice row)
        {
            PXCache mCache = gvArGuiInvoices.Cache;
            PXCache dCache = gvArGuiInvoiceLines.Cache;
            bool isInsert = mCache.GetStatus(row) == PXEntryStatus.Inserted;
            bool isOpen = row.Status == GVList.GVStatus.OPEN;
            bool isHold = row.Status == GVList.GVStatus.HOLD;
            bool isVoid = row.Status == GVList.GVStatus.VOIDINVOICE;
            SelectGuiInvoiceItem.SetVisible(isHold);
            VoidBtn.SetVisible(!isInsert && isOpen && !isVoid);
            VoidBtn.SetEnabled(isOpen);
            //2021-12-07: 拿掉batchNbr判斷，手開可以不填傳票號碼( && row.BatchNbr != null)
            PrintBtn.SetEnabled(row.GuiCmInvoiceID > 0 && !isVoid);

            mCache.AllowDelete = isHold;
            PXUIFieldAttribute.SetReadOnly(mCache, row, !isHold);

            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.hold>(mCache, row, isVoid);

            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.guiCmInvoiceNbr>(mCache, row, false);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.voidBy>(mCache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.voidDate>(mCache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.voidReason>(mCache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.confirmBy>(mCache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.confirmDate>(mCache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.salesAmt>(mCache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.taxAmt>(mCache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVArGuiCmInvoice.totalAmt>(mCache, row, true);

            dCache.AllowInsert = isHold;
            dCache.AllowDelete = isHold;
            dCache.AllowUpdate = isHold;
        }

        private bool SetError<Field>(GVArGuiCmInvoice row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            gvArGuiInvoices.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
            return false;
        }

        private bool SetError<Field>(GVArGuiCmInvoiceLine row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            gvArGuiInvoiceLines.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.Error));
            return false;
        }

        #endregion

        #region BQL
        public virtual GVArGuiInvoice GetGVInvoice(int? detailID)
        {
            return PXSelectJoin<GVArGuiInvoice,
                InnerJoin<GVArGuiInvoiceDetail, On<GVArGuiInvoice.guiInvoiceID, Equal<GVArGuiInvoiceDetail.guiInvoiceID>>>,
                Where<GVArGuiInvoiceDetail.guiInvoiceDetailID, Equal<Required<GVArGuiInvoiceDetail.guiInvoiceDetailID>>>>
                .Select(this, detailID);
        }

        public virtual GVArGuiInvoice GetGVInvoice(string invoiceNbr)
        {
            return PXSelect<GVArGuiInvoice,
                Where<GVArGuiInvoice.guiInvoiceNbr, Equal<Required<GVArGuiInvoice.guiInvoiceNbr>>>>
                .Select(this, invoiceNbr);
        }

        public virtual Location GetLocation(int? locationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(this, locationID);
        }

        public GVPeriod GetGvPeriod(String registrationCD, int? year, int? month)
        {
            return PXSelect<GVPeriod,
                Where<GVPeriod.registrationCD, Equal<Required<GVPeriod.registrationCD>>,
                    And<GVPeriod.periodYear, Equal<Required<GVPeriod.periodYear>>,
                    And<GVPeriod.periodMonth, Equal<Required<GVPeriod.periodMonth>>,
                    And<GVPeriod.outActive, Equal<True>>
                    >>>>.Select(this, registrationCD, year, month);
        }

        /// <summary>
        /// 取得所有折讓明細By InvoiceNbr ，且排除自己 與 作廢折讓
        /// </summary>
        /// <param name="invoiceNbr"></param>
        /// <param name="cmInvoiceLineID"></param>
        /// <returns></returns>
        public PXResultset<GVArGuiCmInvoiceLine> GetGVArGuiCmInvoiceLine(string invoiceNbr, int? cmInvoiceLineID, int? detailID)
        {
            return PXSelectJoin<GVArGuiCmInvoiceLine,
                InnerJoin<GVArGuiCmInvoice, On<GVArGuiCmInvoice.guiCmInvoiceID, Equal<GVArGuiCmInvoiceLine.guiCmInvoiceID>>>,
                Where<GVArGuiCmInvoiceLine.arGuiInvoiceNbr, Equal<Required<GVArGuiCmInvoiceLine.arGuiInvoiceNbr>>,
                And<GVArGuiCmInvoiceLine.guiCmInvoiceLineID, NotEqual<Required<GVArGuiCmInvoiceLine.guiCmInvoiceLineID>>,
                And<GVArGuiCmInvoiceLine.guiInvoiceDetailID, Equal<Required<GVArGuiCmInvoiceLine.guiInvoiceDetailID>>,
                And<GVArGuiCmInvoice.status, NotEqual<GVList.GVStatus.voidinvoice>>>>>>
                .Select(new PXGraph(), invoiceNbr, cmInvoiceLineID, detailID);
        }

        #endregion

        #region Table
        [Serializable]
        [PXHidden]
        public class GVARInvFilter : IBqlTable
        {
            #region VoidDesc
            [PXString(240, IsUnicode = true)]
            [PXUIField(DisplayName = "Void Reason", Required = true)]
            public virtual string VoidDesc { get; set; }
            public abstract class voidDesc : PX.Data.BQL.BqlString.Field<voidDesc> { }
            #endregion
        }

        #region ARInvoiceNbrLOV
        [Serializable]
        [PXProjection(typeof(Select2<GVArGuiInvoice,
              InnerJoin<GVArGuiInvoiceDetail,
                  On<GVArGuiInvoiceDetail.guiInvoiceID, Equal<GVArGuiInvoice.guiInvoiceID>>,
              LeftJoin<ARRegister,
                  On<GVArGuiInvoiceDetail.arRefNbr, Equal<ARRegister.refNbr>,
                  And<GVArGuiInvoiceDetail.aRDocType, Equal<ARRegister.docType>>>>>,
              Where<GVArGuiInvoice.status, Equal<GVList.GVStatus.open>>>)
            )]
        [PXHidden]
        public class ARInvoiceNbrLOV : IBqlTable
        {
            #region Selected
            [PXBool()]
            [PXUIField(DisplayName = "Selected")]
            public virtual bool? Selected { get; set; }
            public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
            #endregion

            #region GuiInvoiceNbr
            [PXDBString(BqlField = typeof(GVArGuiInvoice.guiInvoiceNbr), IsKey = true)]
            [PXUIField(DisplayName = "Gui Invoice Nbr", Enabled = false)]
            public virtual string GuiInvoiceNbr { get; set; }
            public abstract class guiInvoiceNbr : PX.Data.BQL.BqlString.Field<guiInvoiceNbr> { }
            #endregion

            #region GuiInvoiceDetailID
            [PXDBInt(BqlField = typeof(GVArGuiInvoiceDetail.guiInvoiceDetailID), IsKey = true)]
            public virtual int? GuiInvoiceDetailID { get; set; }
            public abstract class guiInvoiceDetailID : PX.Data.BQL.BqlInt.Field<guiInvoiceDetailID> { }
            #endregion

            #region CustomerID
            [CustomerActive(BqlField = typeof(GVArGuiInvoice.customerID), Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName), Filterable = true)]
            [PXUIField(DisplayName = "Customer ID", Enabled = false)]
            public virtual int? CustomerID { get; set; }
            public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
            #endregion

            #region CustUniformNumber
            [PXDBString(BqlField = typeof(GVArGuiInvoice.custUniformNumber))]
            [PXUIField(DisplayName = "Cust Uniform Number", Enabled = false)]
            public virtual string CustUniformNumber { get; set; }
            public abstract class custUniformNumber : PX.Data.BQL.BqlString.Field<custUniformNumber> { }
            #endregion

            //#region Status
            //[PXDBString(BqlField = typeof(GVArGuiInvoice.status))]
            //[PXUIField(DisplayName = "Status")]
            //[GVList.GVStatus]
            //public virtual string Status { get; set; }
            //public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
            //#endregion

            #region InvoiceType
            [PXDBString(BqlField = typeof(GVArGuiInvoice.invoiceType))]
            [PXUIField(DisplayName = "InvoiceType", Enabled = false)]
            [GVList.GVGuiInvoiceType]
            public virtual string InvoiceType { get; set; }
            public abstract class invoiceType : PX.Data.BQL.BqlString.Field<invoiceType> { }
            #endregion

            #region Remark
            [PXDBString(BqlField = typeof(GVArGuiInvoice.remark))]
            [PXUIField(DisplayName = "Remark", Enabled = false)]
            public virtual string Remark { get; set; }
            public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
            #endregion

            #region SalesAmt
            [PXDBDecimal(BqlField = typeof(GVArGuiInvoiceDetail.salesAmt))]
            [PXUIField(DisplayName = "SalesAmt", Enabled = false)]
            public virtual Decimal? SalesAmt { get; set; }
            public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
            #endregion

            #region TaxAmt
            [PXDBDecimal(BqlField = typeof(GVArGuiInvoiceDetail.taxAmt))]
            [PXUIField(DisplayName = "TaxAmt", Enabled = false)]
            public virtual Decimal? TaxAmt { get; set; }
            public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
            #endregion

            #region ItemDesc
            [PXDBString(BqlField = typeof(GVArGuiInvoiceDetail.itemDesc))]
            [PXUIField(DisplayName = "Item Desc", Enabled = false)]
            public virtual string ItemDesc { get; set; }
            public abstract class itemDesc : PX.Data.BQL.BqlString.Field<itemDesc> { }
            #endregion

            #region BatchNbr
            [PXDBString(BqlField = typeof(ARRegister.batchNbr))]
            [PXUIField(DisplayName = "Batch Nbr", Enabled = false)]
            public virtual string BatchNbr { get; set; }
            public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
            #endregion

            #region RefNbr
            [PXDBString(BqlField = typeof(ARRegister.refNbr))]
            [PXUIField(DisplayName = "Ref Nbr", Enabled = false)]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

        }
        #endregion
        #endregion
    }

}