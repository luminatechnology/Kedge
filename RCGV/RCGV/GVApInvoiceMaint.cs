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
     * 1.�����@�w�n���o�������ˮ֡A���ˮ֪��SalseAmt�O�_�j��0
     * 2.��EPRefnbr �� Refnbr���� �������Ӫ��s�W
     * 
     * ====2021-03-09:11974 ====Alton
     * 1.���Y���s��
     *   GuiType �w�] 21
     *   InvoiceDate �w�]Business Date, InvoiceDate����, �гs��DeclareYear, DeclareMonth
     *   DeclareYear �w�]InvoiceDate���~��
     *   DeclareMonth �Ч令StringList(1~12��) value(1~12), label(1��~12��), �w�]
     *   InvoiceDate�����
     *   Vendor����, �бNVendor������AcctName�a��VendorName, �ñNVendor������LocationExtAddress.TaxRegistrationID�a��VendorUniformNumber
     *   VendorName�i�H�ۦ��J�ק�
     *   VendorUniformNumber�i�H�ۦ��J�ק�
     *   InvoiceType -->�e�������
     *   TaxRate -->�e�������
     *   VoucherCategory �p�G�ϥΪ̤Ŀ�-�o��(�t�|)�h�s��InvoiceType�׵o��, ��L�ﶵ�h�s�� InvoiceType�צ���
     *   SalesAmt = sum(GVApGuiInvoiceDetail.SalesAmt), readonly
     *   TaxAmt = sum(GVApGuiInvoiceDetail.TaxAmt), readonly
     *   TotalAmt = SalesAmt + TaxAmt, readonly
     * 2.�������s��
     *   APRefNbr, LOV, �a�XAPRegister, ���A��Release���p����, �ϥΪ̬D���, �гs�ʱa�XAPRegister.DocDesc��GVApGuiInvoiceDetail.ItemDesc.�NAPInvoice.CuryLineTotal�a��GVApGuiInvoiceDetail.UnitPrice��SalesAmt, �NAPInvoice.CuryTaxTotal�a��GVApGuiInvoiceDetail.TaxAmt
     *   Qty �w�]��1
     *   UOM LOV, �йw�]�a"��"
     * 3.APRefNbr�ALOV�B�~����AVendorID = GVApGuiInvoice.Vendor
     * 4.�s�ɫe�ˮ֩��Ӫ�APRefNbr������APInvoice��vendorID�O�_�P���Y�@�P
     * 
     * ====2021-03-22:11995 ====Alton
     * 1.GVApGuiInvoice.Hold �w�]�� True, Status�w�]�����m
     * 2.���m�İ_��, ���A�ܬ����m.
     * 3.���A���}�Ү�, ���Ҧ���줣��ק�, ���FGVApGuiInvoice.Remark, GVApGuiInvoice.Hold
     * 4.GVApGuiInvoice.Hold�Q�Ŀ�, Status��s�����m.
     * 5.Action �@�o, �u�b���A���}�Ү�Enable, ��ϥΪ̫��U�@�oAction, ���Conform Dialoge �ШϥΪ̿�J�@�o��] (GVApGuiInvoice.VoidReason), �ϥΪ̿�J�T�w��, �NStatus��s�� "Void", ���Ҧ���줣��קﲧ��, �åBupdate VoidDate�]BusinessDate�^, VoidReason.
     * 6.�u��Status�����m���ɭ�, �i�H�R���o��.
     * 
     * ====2021-03-29:11974 ====Alton
     * Tab-other information �� RefNbr �� EPRefNbr ���а���hyperlink
     * 
     * ===2021-04-07:11995 ====Alton
     * 1. Action Display Name �אּ�u�h�^�v�A�÷s�W�@�ӹ�����status= Return, ��L�ʧ@��ӤW�z�u�@�o�vaction�B�z
     * 2. �бNhold ��checkbox�Φ]��chechbox�Ŀ�Ӧ����A���ܪ������޿�ޱ�
     * 3. �Цh�[�@��button�u�T�{�v�A��ϥΪ̫��U���s�ɡA�бNGVApGUIinvoice.Status�ܦ�Open�A
     *    �B���ɶȦ��u�h�^�v��button�i���A��L�a�賣�O��Ū���A
     *    
     * ===2021-04-15:11974 ====Alton
     * �ץ����ӥ[�`
     * 
     * ====2021-05-25: 12057====Alton
     * ��|�����O��"���|" ���ӿ�J���|���B, �Х��w���p��5%���|�B��GVApGuiInvoiceDetail.TaxAmt
     * 
     * ====2021-05-26: �f�Y ====Alton
     * ���Ӫ�ArRefNbr�n�i�H�諸����Y��RefNbr
     * 
     * ====2021-06-02:12072 ====Alton
     * 1. �Цb �i���o�����@ ��L��Ttab���A�s�W2��unbound���çe�{�b�e���W,read-only�G
     * . APRegister.DocDate
     * . APRegister.UsrIsConfirmby
     * 2.�b��APInvoice��T�ɥN�J(��bFin���]...)
     * 3.�s�W�@�ӧP�_: Voucher Category = '�����N�x��~�|' and '��L����' �A�t�ӲΤ@�s�����Υ���(��bDAC)
     * 
     * ====2021-06-17:12097====Alton
     * 1. �ЦbGVApGuiInvoice�h�}�@�����VendorAddress, nvarchar(255), ���\Null=False
     * 2. �N�ө��i���o���D�ɪ��e���W�A��m�b���Y���u�����v�W��
     * 3. ����GVApGuiInvoice.Vendor�ɡA�йw�]�a�X�Ө����Ӧb�D�ɤ����@��AddressLine1
     * �S�����GVApGuiInvoice.Vendor�ɡA�йw�]VendorAddress����
     * 
     * ====2021-11-18:12271====Alton
     * 1. �o���榡�ˮֱ���վ�G
     *    (1)����|��GuiType���ˬd�A�Чאּ��InvoiceType���ˮ֨̾�
     *    ��InvoviceType=���o�����A�����d��X�^��+�K�X�Ʀr�A�ȳ\�ˬd�`�@���׬O10�X�Y�i
     *    ��InvoiceType='���ڡ��ɡA�h�����ˬd����榡
     *    (2) EP �� AP ��ӵo��tab�����@�W�z�ץ�
     *  
     * 2. �o���榡�ˮֿ��~�T���վ�G
     *    �ثe�bKG�ǲ��T�{�L�b�ɡA�Y�o���榡���~�A�|��ܭ^�媺���~�T���p���ϡA�Ш�U�N���~�T���վ㦨�u�o�����X�榡���~�A�Цܸӱi�p����- �o������ ���϶����ˬd�v
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
            String msg = String.Format("{0}�w�}�ߧ������i�h�^", Invoice.Current.GuiInvoiceNbr);
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
        [PXButton(Tooltip = "�}�����ק�", CommitChanges = true)]
        [PXUIField(DisplayName = "�ק���")]
        protected virtual IEnumerable editBtn(PXAdapter adapter)
        {
            PXResultset<GVApGuiCmInvoiceLine> rs = GetCMInvoiceByInvoiceNbr(Invoice.Current.GuiInvoiceNbr);
            String msg = String.Format("{0}�w�}�ߧ������i�ק�", Invoice.Current.GuiInvoiceNbr);
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
            //�ˮ֦~
            if (row.DeclareYear == 0)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareYear, "Year can't be zero.");
            else if (row.InvoiceDate?.Year > row.DeclareYear)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareYear, String.Format("Year can not less than invoice year {0:d}.", row.InvoiceDate?.Year));
            //�ˮ֤�
            if (row.DeclareMonth == 0)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareMonth, "Month can't be zero.");
            else if (row.InvoiceDate?.Year >= row.DeclareYear && row.InvoiceDate?.Month > row.DeclareMonth)
                check = SetError<GVApGuiInvoice.declareYear>(row, row.DeclareMonth, String.Format("Year can not less than invoice month {0:d}.", row.InvoiceDate?.Month));
            //�ˮ�GroupCnt
            if (row.GroupRemark == GVList.GVGuiGroupRemark.SUMMARY && row.GroupCnt == 0)
                check = SetError<GVApGuiInvoice.groupCnt>(row, row.GroupCnt, String.Format("Group count can't be zero when group remark equal to {0}", GroupRemarkAttribute.SummaryName));

            if (row.InvoiceType == GVList.GVGuiInvoiceType.INVOICE)
            {
                //�ˮֵo�����X�榡
                //if (!Regex.Match(row.GuiInvoiceNbr, "^[A-Z]{2}[0-9]{8}").Success)
                if (row.GuiInvoiceNbr?.Length < 10)
                    check = SetError<GVApGuiInvoice.guiInvoiceNbr>(row, row.GuiInvoiceNbr, "�o�����X�榡���~!");
                //�ˮֵo�����X�O�_����
                if (IsUsingInvoiceNbr(row.GuiInvoiceNbr, row.GuiInvoiceID))
                    check = SetError<GVApGuiInvoice.guiInvoiceNbr>(row, row.GuiInvoiceNbr, "�ӵo�����X�w�Q�ϥ�");
            }
            //�ˮ�SalesAmt
            //20220222 modify by Louis ���|���B�i�H�� 0
            if (row.SalesAmt < 0)
                check = SetError<GVApGuiInvoice.salesAmt>(row, row.SalesAmt, "�o�����B���o���t��.");
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
            //���sĲ�o�ˮ�
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
            //�ˮ�Qty
            if ((row.Qty ?? 0) <= 0)
                check = SetError<GVApGuiInvoiceDetail.qty>(row, row.Qty, "Qty can't be zero");
            //�ˮ�unitPrice
            //20220222 modify by louis ����i�H��0, ���ର�t��
            if ((row.UnitPrice ?? 0) < 0)
                check = SetError<GVApGuiInvoiceDetail.unitPrice>(row, row.UnitPrice, "UnitPrice can't be negative");
            //�ˮ�TaxAmt
            if (master.TaxCode == GVList.GVTaxCode.TAXABLE && (row.TaxAmt ?? 0) <= 0)
                check = SetError<GVApGuiInvoiceDetail.taxAmt>(row, row.TaxAmt, "TaxAmt can't be zero");
            //�ˮ�APRefNbr����Vendor�O�_�P���Y�ۦP
            if (!String.IsNullOrEmpty(row.APRefNbr))
            {
                APInvoice invoice = (APInvoice)PXSelectorAttribute.Select<GVApGuiInvoiceDetail.apRefNbr>(e.Cache, row);
                //2021-05-26 add by alton ����RefNbr�n�i�H�諸����Y��Refnbr
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
            e.Cache.SetValueExt<GVApGuiInvoiceDetail.uom>(row, "��");
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
        /// ���s�u����Y�[�`(��APInvoice�L�b��)
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
            PXUIFieldAttribute.SetVisible<GVApGuiInvoiceInfo.printCnt>(InvoiceInfo.Cache, InvoiceInfo.Current, false);//�Ȯɤ����
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