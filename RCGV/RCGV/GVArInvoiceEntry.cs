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
     * ���Y�w�]�Ȥγs��
     * 1.RegistrationCD�|�y�s��, �w�]�a�J�n�J��Branch���ݪ�HVRegisteration.RegistrationCD
     * 2.InvoiceDate ����, �w�]��BusinessDate
     * 3.GuiBookCD LOV �i�D���GVGuiBook.StartMonth <= InvoiceDate <= GVGuiBook.EndMonth���o��, �BGVGuiBook�٦��Ѿl�o���٥��}���o��ï
     * 4.CustomerID LOV, ����. �Ѧ�ARRegister.CustomerID��LOV. �D�粒CustomerID, �бa�XCustomer������AcctName��TaxRegistrationID��CustName �� CustUniformNumber
     * 5.CustName �i�ק�
     * 6.CustUniformNumber readonly, ����
     * 7.CustAddress �i�ק�(Address.City+Address.AddressLine1+Address.AddressLine2)
     * 8.InvoiceType, �w�]'I'
     * 9.GuiType, ����, �бa�JGuiBookCD������GVGuiBook.GuiType
     * 10.GuiWordCD, ����, �бa�JGuiBookCD������GVGuiBook.GuiWordCD
     * 11.SalesAmt, �Х[�`���Ӫ�SalesAmt, readonly
     * 12.TaxAmt, �Х[�`���Ӫ�TaxAmt, readonly
     * 13.TotalAmt, SalesAmt+TaxAmt
     * 
     * ====2021-03-29:12004====Alton
     * ���w�]�Ȥγs��
     * 1.ARRefNbr LOV �бa�X���YCustomerID���ݪ�ARInvoice,��ARInvoice�w�g�L�b. �D����. �D�粒RefNbr�бa�XARRegister.CuryLineTotal��ARCurLineAmt
     * 2.ARCurLineAmt, unbound���
     * 3.ItemDesc freekeyin, �p�G���D��ARInvoice,�бa�XARRSegister.DocDesc
     * 4.ARDocType, �p�G���D��ARInvoice,�бa�XARRSegister.DocType
     * 5.Qty �w�]1,
     * 6.UOM lov, �w�]'��'
     * 7.SalesAmt, �j��0, �p�G��ARRefNbr����j��ARRegister.CuryLineTotal-SUM(�w�}�߾P���o�����B)
     * 
     * ====2021-03-29:12004====Alton
     * ���վ�
     * 1.�s�W�@�����GuiInvoiceCD, IsKey=true, �t�Φ۰ʽs��, �ϥ�Seqment Key 'GVARINVOICECD'
     * 2.��GuiInvoiceNbr, �o�����X, �Ч�����m����, ���A�ܦ�Open�~�q�o��ï������
     * 3.GuiInvoiceCD�Щ�b�쪩���]�p��GuiInvoiceNbr, lov
     * 4.�� GuiInvoiceNbr�в��ʨ���Y��GuiWordCD��m, readonly
     * 
     * 1.�w�]���A��Hold, Hold=true
     * 2.���m����, �Х����ܲΤ@�o���Y�N����, �нT�{��Confirm Dialog, �ϥΪ̽T�{�~�ܧ󪬺A����
     * 3.���A��Open��, ���FCustName/CustAddress �H�Ω��Ӫ� ItemDesc ��l�Ҥ��i�ק�ηs�W�R��
     * 4.�s�WVoid"�@�o" action. �u���b���A��Open�Uenable.
     * 5.���U�@�o, ���XDialog�ШϥΪ̿�J�@�o��], �T�{Status�אּVoid. �ç�sVoidDate, VoidBy, VoidReason. �����������.
     * 
     * ====2021-03-31:12004====Alton
     * 1.�s�ɮ�, ���ˬd�p�G��ARRefNbr����j��ARRegister.CuryLineTotal-SUM(�w�}�߾P���o�����B)
     * 2.SUM(�w�}�߾P���o�����B)�ݭn�Ҽ{����઺���p, �p�G������ARInvoice�����઺�ɤ�վ㡥�w�}�߾P���o�����B'
     *   �ݭn����઺���B.���������ARInvoice�P�_�޿�, DocType='CRM' OrigDocType='$��ARinovice��DocType, 
     *   �YINV', OrigRefNbr='��ARinoviceh�ѷӸ��X', �h���iARinvoice�Y�����Arinvoice������o��.
     *   
     * ====2021-04-09:11998====Alton
     * ���e��ӧP�_��ARRegister.CuryLineTotal ��� ARRegister.CuryVatTaxableTotal
     * 
     * ====2021-04-29:12030====Alton
     * �ƻs�\�� �K�W�ɤ��i�H�s�o�����X�@�ֶK�W
     * 
     * ====2021-05-25:12004====Alton
     * �s�W�H�U�޿�G
     * ���Y �|�����O�����| �ɡA�~�n�p��TaxAmt= SalesAmt*0.05
     * ���� �s�|&�K�| �ɡATaxAmt�Ҥ��ݰ��p��
     * 
     * ====2021-05-27:12066====Alton
     * 	1.�i���o�����@�s�W�@��Action, "�ק���"
     * 	2.�i���o�����A��Open��, enable"�ק���"
     * 	3.����ק���, ���F���A���, ��L��쳣�}�ҥi�H�ק�, ������R��(���Y/����)�ηs�W���(����)
     * 	4.�x�s��, �A��ҥH�����w��readonly
     * 
     * ====2021-08-10:12189====Alton
     * 	1.GVArGuiInvoice �s�W�@����� IsHistorical (�O�_���v�o��), bits, �w�] false
     * 	2.�e��IsHistorical ����b���YGuiBookID�W��, ���Acheckbox
     * 	3.�ϥΪ̤Ŀ�IsHistorical, �}�ҵo�����X������ϥΪ̦ۦ��J, �åB�N�o��ï�אּreadonly, �o��ïID�w�] 0 (���v�o��ï)
     * 	4.�ϥΪ̦s��, �ݭn�ˬd�o�����X�bTable�����୫��(�P�@�Ӧ~��, ��InvoiceDate���~���̾�)
     * 	5.���v�o��, �@���s��, ����A�קאּ�D���v�o��
     * ====2021-08-11:12196====Alton
     * �b �P���o��ï�]�w �����o��ï�A�̷�user�bGV�P���o��/�P���妸�}��/�P������/�P���妸���� �o�|��@�~��,
     * �ҿ諸InvoiceDate������ŦX�H�U�޿�A�N�n�b�u�o��ï���vLOV����ܥX�ӡG
     * -��ҿ�InvoiceDate������A�bGVPeriod.OutActive = True�ɡA�Y�i�}�߾P���o��
     * -��ҿ�InvoiceDate������A�bGVPeriod.OutActive <> True �ɡA�и��Xerror message:'�o�����O�|������ΨS���]�w�A�L�k�}�߸Ӵ��O���P���o��
     * 
     * ====2021-10-13:12254====Alton
     * 1.�}�� GVArGuiInvoice.CustUniformNumber �Ȥ�Τ@�s���i�H�ק�.
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
        [PXUIField(DisplayName = "�T�{�P����")]
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
            //2021-03-30 mark by alton �ڰ�ثe���ݭn
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
            //���sĲ�odetail�ˮ�
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
                //�ˮ֤��
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
        //�ˬdInvoiceDate
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
                       row, row.InvoiceDate, "�o�����O�|������ΨS���]�w�A�L�k�}�߸Ӵ��O���P���o��/����");

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
                           row, row.GuiInvoiceNbr, "�п�J�o�����X!");
            }

            if (row.GuiInvoiceNbr != null)
            {
                int? year = row.InvoiceDate?.Year;
                PXResultset<GVArGuiInvoice> rs = GetGVInvoiceByGVNbr(row.GuiInvoiceNbr, row.GuiInvoiceID);
                //�p�G�~���̼˫h����
                foreach (GVArGuiInvoice item in rs)
                {
                    int? _year = item.InvoiceDate?.Year;
                    if (year == _year)
                        SetError<GVArGuiInvoice.guiInvoiceNbr>(
                           row, row.GuiInvoiceNbr, "�o�����X���i����!");
                }
            }
        }


        #endregion

        #region Verifying Detail

        /// <summary>
        /// �ˮֻPARInvoice������T
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
                    row, row.SalesAmt, String.Format("�w�W�L{0}�i�}�ߪ��B(�l�B�G{1:0.00})", row.ARRefNbr, row.ARCurLineAmt - total));
                else if ((row.ARCurLineAmt - row.CRMAmt) < (total + (row.SalesAmt ?? 0)))
                    SetWarning<GVArGuiInvoiceDetail.salesAmt>(
                    row, row.SalesAmt, String.Format("�w�W�L{0}�������B(�l�B�G{1:0.00})", row.ARRefNbr, row.ARCurLineAmt - total - row.CRMAmt));
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

            #region �T�w
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
            //add by alton �קK�����W�U���ɨS���Q�}�����
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
        /// ���oARInvoice���U���Ҧ��o���A�B�ư��@�o�o���P�ۤv
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