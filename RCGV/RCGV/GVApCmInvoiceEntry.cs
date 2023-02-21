using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.CR;
using RCGV.GV.DAC;
using RCGV.GV.Util;

namespace RCGV.GV
{
    /***
     * ====2021-03-23:11997 ====Alton
     * OtherInformation
     * 1.欄位皆為Resdonly
     * ActionBar
     * 1.新增"作廢"Action
     * 2.只有在狀態為Open時, 才enable
     * 3.使用者按下作廢, 跳出ConfirmDialog, 並要求user填寫VoidReason, 確認後將狀態更新為"Void", VoidDate = BusinessDate, 所以欄位變成Readonly
     * 
     * ====2021-05-25:12058====Alton
     * 稅務類別為 "應稅"時 輸入明細的GVApGuiCmInvoiceLine.SalesAmt, 請自動計算帶出GVApGuiCmInvoiceLine.TaxAmt
     * 
     * ====2022-03-21====Jeff
     * Mantis [12296] Add new tax amount fields.
     * **/
    public class GVApCmInvoiceEntry : PXGraph<GVApCmInvoiceEntry, GVApGuiCmInvoice>
    {
        #region Select
        public PXSelect<GVApGuiCmInvoice> GuiCmInvoice;
        public PXSelect<GVApGuiCmInvoiceLineV, Where<GVApGuiCmInvoiceLineV.guiCmInvoiceID, Equal<Current<GVApGuiCmInvoice.guiCmInvoiceID>>>> GuiCmInvoiceLine;
        public PXFilter<GVAPCMInvFilter> VoidInvoicePanel;

        public GVApCmInvoiceEntry()
        {

        }
        #endregion

        #region Action
        #region Void
        public PXAction<GVApGuiCmInvoice> VoidButton;
        [PXButton(Tooltip = "Void invoice", CommitChanges = true)]
        [PXUIField(DisplayName = "VOID",
            MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select,
            Enabled = false)]
        protected virtual IEnumerable voidButton(PXAdapter adapter)
        {
            if (VoidInvoicePanel.AskExt(true) == WebDialogResult.OK)
            {
                GVApGuiCmInvoice invoice = (GVApGuiCmInvoice)GuiCmInvoice.Cache.Current;
                GVAPCMInvFilter voidFilter = (GVAPCMInvFilter)VoidInvoicePanel.Cache.Current;
                if (String.IsNullOrEmpty(voidFilter.VoidReason))
                {
                    throw new PXException("Void reason is require.");
                }
                PXLongOperation.StartOperation(this, delegate ()
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        invoice.VoidDate = this.Accessinfo.BusinessDate;
                        invoice.VoidReason = voidFilter.VoidReason;
                        invoice.Status = GVList.GVStatus.VOIDINVOICE;
                        GuiCmInvoice.Update(invoice);
                        Persist();
                        ts.Complete();
                    }
                });
            }
            return adapter.Get();
        }
        #endregion

        #region Print
        public PXAction<GVApGuiCmInvoice> PrintBtn;
        [PXButton(Tooltip = "折讓單列印", CommitChanges = true)]
        [PXUIField(DisplayName = "折讓單列印")]
        protected virtual IEnumerable printBtn(PXAdapter adapter)
        {
            GVApGuiCmInvoice invoice = (GVApGuiCmInvoice)GuiCmInvoice.Cache.Current;
            Dictionary<string, string> patams = new Dictionary<string, string>();
            patams["P_GuiCmInvoiceNbrB"] = invoice.GuiCmInvoiceNbr;
            patams["P_GuiCmInvoiceNbrE"] = invoice.GuiCmInvoiceNbr;
            throw new PXReportRequiredException(patams, "GV601001", "Report");
        }
        #endregion
        #endregion

        #region Events

        #region GVApGuiCmInvoice
        protected virtual void _(Events.RowSelected<GVApGuiCmInvoice> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null) return;
            SetEnabled(row);
        }

        protected virtual void _(Events.RowPersisting<GVApGuiCmInvoice> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            bool check = true;
            //檢核年
            if (row.DeclareYear == 0)
            {
                check = false;
                SetError<GVApGuiCmInvoice.declareYear>(row, row.DeclareYear, "Year can't be zero.");
            }
            else if (row?.InvoiceDate?.Year > row.DeclareYear)
            {
                check = false;
                SetError<GVApGuiCmInvoice.declareYear>(row, row.DeclareYear,
                    String.Format("Year can not less than invoice year {0:d}.", row.InvoiceDate?.Year));
            }
            //檢核月
            if (row.DeclareMonth == 0)
            {
                check = false;
                SetError<GVApGuiCmInvoice.declareMonth>(row, row.DeclareMonth, "Month can't be zero.");
            }
            else if (row?.InvoiceDate?.Year >= row.DeclareYear && row?.InvoiceDate?.Month > row.DeclareMonth)
            {
                check = false;
                SetError<GVApGuiCmInvoice.declareMonth>(row, row.DeclareMonth,
                    String.Format("Year can not less than invoice month {0:d}.", row.InvoiceDate?.Month));
            }


            //檢核Period
            GVPeriod gvPeriod = getGvPeriod(row.RegistrationCD, row.DeclareYear, row.DeclareMonth);
            if (gvPeriod == null)
            {
                check = false;
                SetError<GVApGuiCmInvoice.registrationCD>(row, row.RegistrationCD, "Declare Period had closed, please check!");
            }
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoice, GVApGuiCmInvoice.totalAmt> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null) return;
            e.NewValue = row.SalesAmt + row.TaxAmt;
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.hold> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null) return;
            if (row.Hold == true)
            {
                e.Cache.SetValueExt<GVApGuiCmInvoice.status>(row, GVList.GVStatus.HOLD);
            }
            else
            {
                e.Cache.SetValueExt<GVApGuiCmInvoice.status>(row, GVList.GVStatus.OPEN);
                e.Cache.SetValueExt<GVApGuiCmInvoice.confirmDate>(row, this.Accessinfo.BusinessDate);
                e.Cache.SetValueExt<GVApGuiCmInvoice.confirmPerson>(row, this.Accessinfo.UserID);
            }
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.salesAmt> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.totalAmt>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.taxAmt> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.totalAmt>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.vendorID> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.vendorName>(row);
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.vendorUniformNumber>(row);
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.vendorAddress>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoice, GVApGuiCmInvoice.vendorUniformNumber> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row == null) return;
            e.NewValue = GetDefLocation(row.VendorID)?.TaxRegistrationID;
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.vendorUniformNumber> e)
        {
            if (e.Row == null) return;
            //重新觸發Line檢核
            foreach (GVApGuiCmInvoiceLineV item in GuiCmInvoiceLine.Cache.Cached)
            {
                GuiCmInvoiceLine.Update(item);
            }
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoice, GVApGuiCmInvoice.vendorName> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            if (row.VendorID == null) return;
            BAccountR b = (BAccountR)PXSelectorAttribute.Select<GVApGuiCmInvoice.vendorID>(e.Cache, row);
            e.NewValue = b?.AcctName;
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.invoiceDate> e)
        {
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.declareYear>(e.Row);
            e.Cache.SetDefaultExt<GVApGuiCmInvoice.declareMonth>(e.Row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoice, GVApGuiCmInvoice.declareYear> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            e.NewValue = row?.InvoiceDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoice, GVApGuiCmInvoice.declareMonth> e)
        {
            GVApGuiCmInvoice row = (GVApGuiCmInvoice)e.Row;
            e.NewValue = row?.InvoiceDate?.Month;
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoice, GVApGuiCmInvoice.taxCode> e)
        {
            foreach (GVApGuiCmInvoiceLineV item in GuiCmInvoiceLine.Select())
            {
                GuiCmInvoiceLine.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.taxAmt>(item);
            }
        }

        #endregion

        #region GVApGuiCmInvoiceLineV
        protected virtual void _(Events.RowPersisting<GVApGuiCmInvoiceLineV> e)
        {
            GVApGuiCmInvoice master = GuiCmInvoice.Current;
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            bool check = true;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            GVApGuiInvoice apInvoice = PXSelectorAttribute.Select<GVApGuiCmInvoiceLineV.apGuiInvoiceNbr>(e.Cache, row) as GVApGuiInvoice;

            //檢查金額
            if (apInvoice?.SalesAmt < row.SalesAmt || row.Balance < row.SalesAmt)
            {
                check = false;
                SetError<GVApGuiCmInvoiceLineV.salesAmt>(row, row.SalesAmt, "Discount amount over current amount");
            }

            //檢查VendorUniformNumber是否與支票一致
            if (master.VendorUniformNumber != apInvoice?.VendorUniformNumber)
            {
                check = false;
                SetError<GVApGuiCmInvoiceLineV.apGuiInvoiceNbr>(row, row.ApGuiInvoiceNbr,
                    String.Format("[{0}]VendorUniformNumber is not mapping", row.ApGuiInvoiceNbr));
            }
        }

        protected virtual void _(Events.RowDeleted<GVApGuiCmInvoiceLineV> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            GVApGuiCmInvoice master = this.GuiCmInvoice.Current;
            this.GuiCmInvoice.Cache.SetValueExt<GVApGuiCmInvoice.salesAmt>(master, (master.SalesAmt ?? 0m) - (row.SalesAmt ?? 0m));
            this.GuiCmInvoice.Cache.SetValueExt<GVApGuiCmInvoice.taxAmt>(master, (master.TaxAmt ?? 0m) - (row.TaxAmt ?? 0m));
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.salesAmt> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            e.NewValue = (row.Quantity ?? 0) * (row.UnitPrice ?? 0);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.salesAmt> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            GVApGuiCmInvoice master = this.GuiCmInvoice.Current;
            if (master == null) return;
            this.GuiCmInvoice.Cache.SetValueExt<GVApGuiCmInvoice.salesAmt>(master, (master.SalesAmt ?? 0m) - (decimal)(e.OldValue ?? 0m) + (decimal)(e.NewValue ?? 0m));
            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.taxAmt>(row);
            //重新滾算已使用量
            //e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.usedAmt>(row);
            foreach (GVApGuiCmInvoiceLineV item in GuiCmInvoiceLine.Select())
            {
                if (item.ApGuiInvoiceNbr == row.ApGuiInvoiceNbr && item.GuiCmInvoiceLineID != row.GuiCmInvoiceLineID)
                {
                    e.Cache.SetValueExt<GVApGuiCmInvoiceLineV.usedAmt>(item, GetUsedAmtByInvoiceNbr(this, item.ApGuiInvoiceNbr, item.GuiCmInvoiceLineID));
                }
            }
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.taxAmt> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            GVApGuiCmInvoice master = this.GuiCmInvoice.Current;
            decimal taxRate = 0m;
            if (master.TaxCode == GVList.GVTaxCode.TAXABLE)
                taxRate = 0.05m;
            e.NewValue = round((row.SalesAmt ?? 0m) * taxRate);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.taxAmt> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            GVApGuiCmInvoice master = this.GuiCmInvoice.Current;
            this.GuiCmInvoice.Cache.SetValueExt<GVApGuiCmInvoice.taxAmt>(master, (master.TaxAmt ?? 0m) - (decimal)(e.OldValue ?? 0m) + (decimal)(e.NewValue ?? 0m));
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.quantity> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.salesAmt>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.unitPrice> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.salesAmt>(row);
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.apGuiInvoiceNbr> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;

            if (row == null) return;

            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.refNbr>(row);
            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.invSalesAmt>(row);
            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.invTaxAmt>(row);
            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.usedAmt>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.usedAmt> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;

            if (row == null) return;

            (decimal salesAmt, decimal taxAmt) = GetUsedAmtByInvoiceNbr(new PXGraph(), row.ApGuiInvoiceNbr, row.GuiCmInvoiceLineID);

            e.NewValue = salesAmt;
            ///<remarks>Since I don't wanna do the same calculation in the second FieldDefaulting event, write the update here.</remarks>
            row.UsedTaxAmt = taxAmt;
        }

        protected virtual void _(Events.FieldUpdated<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.usedAmt> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<GVApGuiCmInvoiceLineV.balance>(row);
        }

        protected virtual void _(Events.FieldDefaulting<GVApGuiCmInvoiceLineV, GVApGuiCmInvoiceLineV.balance> e)
        {
            GVApGuiCmInvoiceLineV row = (GVApGuiCmInvoiceLineV)e.Row;

            if (row != null)
            {
                e.NewValue     = (row.InvSalesAmt ?? 0) - (row.UsedAmt ?? 0);
                ///<remarks>Since I don't wanna do the same calculation in the second FieldDefaulting event, write the update here.</remarks>
                row.TaxBalance = (row.InvTaxAmt ?? 0) - (row.UsedTaxAmt ?? 0);
            }
        }
        #endregion

        #endregion

        #region Methods
        private void SetError<Field>(GVApGuiCmInvoice row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            GuiCmInvoice.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
        }

        private void SetError<Field>(GVApGuiCmInvoiceLine row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            GuiCmInvoiceLine.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
        }

        private void SetEnabled(GVApGuiCmInvoice row)
        {
            bool isHold = row.Status == GVList.GVStatus.HOLD;
            bool isOpen = row.Status == GVList.GVStatus.OPEN;
            bool isVoid = row.Status == GVList.GVStatus.VOIDINVOICE;
            PXUIFieldAttribute.SetVisible<GVApGuiCmInvoice.printCount>(GuiCmInvoice.Cache, row, false);//暫時不顯示

            PXUIFieldAttribute.SetReadOnly(GuiCmInvoice.Cache, row, !isHold);
            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.remark>(GuiCmInvoice.Cache, row, isVoid);
            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.hold>(GuiCmInvoice.Cache, row, isVoid);

            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.guiCmInvoiceNbr>(GuiCmInvoice.Cache, row, false);
            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.confirmDate>(GuiCmInvoice.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.confirmPerson>(GuiCmInvoice.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.voidDate>(GuiCmInvoice.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.voidReason>(GuiCmInvoice.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<GVApGuiCmInvoice.printCount>(GuiCmInvoice.Cache, row, true);
            VoidButton.SetEnabled(isOpen);
            PrintBtn.SetEnabled(row.GuiCmInvoiceID > 0 && !isVoid);

            GuiCmInvoice.Cache.AllowDelete = isHold;
            GuiCmInvoiceLine.Cache.AllowInsert = isHold;
            GuiCmInvoiceLine.Cache.AllowUpdate = isHold;
            GuiCmInvoiceLine.Cache.AllowDelete = isHold;
        }

        public Decimal? round(Decimal? num)
        {
            return System.Math.Round(num ?? 0m, 0, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region BQL
        public PXResultset<GVPeriod> getGvPeriod(String registrationCD, int? year, int? month)
        {
            PXResultset<GVPeriod> set = PXSelect<GVPeriod,
                    Where<GVPeriod.registrationCD, Equal<Required<GVPeriod.registrationCD>>,
                        And<GVPeriod.periodYear, Equal<Required<GVPeriod.periodYear>>,
                        And<GVPeriod.periodMonth, Equal<Required<GVPeriod.periodMonth>>,
                        And<GVPeriod.outActive, Equal<True>>
                        >>>>.Select(this, registrationCD, year, month);

            return set;

        }

        public Location GetDefLocation(int? baccountID)
        {
            return PXSelectJoin<Location,
                    InnerJoin<BAccount, On<BAccount.defLocationID, Equal<Location.locationID>>>,
                    Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                    .Select(new PXGraph(), baccountID);

        }

        public (decimal salesAmt, decimal taxAmt) GetUsedAmtByInvoiceNbr(PXGraph graph, string guiInvoiceNbr, int? notLineID)
        {
            decimal total = 0m, taxTotal = 0m;

            foreach (GVApGuiCmInvoiceLine item in PXSelectJoin<GVApGuiCmInvoiceLine, InnerJoin<GVApGuiInvoice, On<GVApGuiInvoice.guiInvoiceNbr, Equal<GVApGuiCmInvoiceLine.apGuiInvoiceNbr>>>,
                                                                                     Where<GVApGuiCmInvoiceLine.apGuiInvoiceNbr, Equal<Required<GVApGuiCmInvoiceLine.apGuiInvoiceNbr>>,
                                                                                           And<GVApGuiCmInvoiceLine.guiCmInvoiceLineID, NotEqual<Required<GVApGuiCmInvoiceLine.guiCmInvoiceLineID>>>>>
                                                                                     .Select(graph, guiInvoiceNbr, notLineID))
            {
                total += (item.SalesAmt ?? 0m);
                taxTotal += (item.TaxAmt ?? 0m);
            }

            return (total, taxTotal);
        }
        #endregion

        #region Table
        [Serializable]
        [PXHidden]
        public class GVAPCMInvFilter : IBqlTable
        {
            #region VoidReason
            [PXString(240, IsUnicode = true)]
            [PXUIField(DisplayName = "Reason", Required = true)]
            public virtual string VoidReason { get; set; }
            public abstract class voidReason : PX.Data.BQL.BqlString.Field<voidReason> { }
            #endregion
        }

        #region Extended DAC
        [Serializable]
        [PXCacheName("AP GUI CM Invoice Line")]
        public class GVApGuiCmInvoiceLineV : GVApGuiCmInvoiceLine
        {
            #region RefNbr
            [PXString()]
            [PXUIField(DisplayName = "Reference Nbr.", Enabled = false)]
            [PXUnboundDefault(typeof(Search<GVApGuiInvoice.refNbr, Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Current<apGuiInvoiceNbr>>>>))]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region InvSalesAmt
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Inv Sales Amount", Enabled = false)]
            [PXUnboundDefault(typeof(Search<GVApGuiInvoice.salesAmt, Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Current<apGuiInvoiceNbr>>>>))]
            public virtual decimal? InvSalesAmt { get; set; }
            public abstract class invSalesAmt : PX.Data.BQL.BqlDecimal.Field<invSalesAmt> { }
            #endregion

            #region InvTaxAmt
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Inv. Tax Amount", Enabled = false)]
            [PXUnboundDefault(typeof(Search<GVApGuiInvoice.taxAmt, Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Current<apGuiInvoiceNbr>>>>))]
            public virtual decimal? InvTaxAmt { get; set; }
            public abstract class invTaxAmt : PX.Data.BQL.BqlDecimal.Field<invTaxAmt> { }
            #endregion

            #region UsedAmt
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Used Amt", Enabled = false)]
            [PXUnboundDefault()]
            public virtual decimal? UsedAmt { get; set; }
            public abstract class usedAmt : PX.Data.BQL.BqlDecimal.Field<usedAmt> { }
            #endregion

            #region UsedTaxAmt
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Used Tax Amt", Enabled = false)]
            public virtual decimal? UsedTaxAmt { get; set; }
            public abstract class usedTaxAmt : PX.Data.BQL.BqlDecimal.Field<usedTaxAmt> { }
            #endregion

            #region Balance
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Balance", Enabled = false)]
            [PXUnboundDefault()]
            public virtual decimal? Balance { get; set; }
            public abstract class balance : PX.Data.BQL.BqlDecimal.Field<balance> { }
            #endregion

            #region TaxBalance
            [PXDecimal(2)]
            [PXUIField(DisplayName = "Tax Balance", Enabled = false)]
            public virtual decimal? TaxBalance { get; set; }
            public abstract class taxBalance : PX.Data.BQL.BqlDecimal.Field<taxBalance> { }
            #endregion
        }
        #endregion
        #endregion
    }
}
