using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.CR;
using PX.Objects.AR;
using RCGV.GV.Descriptor;
using RCGV.GV.SYS;
using RCGV.GV.Util;

namespace RCGV.GV
{
    public class GVArGuiZeroDocEntry : GVBaseGraph<GVArGuiZeroDocEntry, GVArGuiZeroDoc>
    {
        #region Select
        public PXSelect<GVArGuiZeroDoc> gvArGuiZeroDocs;
        public PXSetup<GVSetup> AutoNumSetup;
        public GVArGuiZeroDocEntry()
        {
            GVSetup setup = AutoNumSetup.Current;

        }
        public PXSelect<GVArGuiZeroDocLine,
            Where<GVArGuiZeroDocLine.zeroDocID,
            Equal<Current<GVArGuiZeroDoc.zeroDocID>>>> gvArGuiZeroDocLines;
        #endregion

        #region Event
        protected virtual void GVArGuiZeroDocLine_GuiInvoiceNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVArGuiZeroDoc master = gvArGuiZeroDocs.Current;
            GVArGuiZeroDocLine line = (GVArGuiZeroDocLine)e.Row;
            PXResultset<GVArGuiInvoice> set = PXSelect<GVArGuiInvoice,
                        Where<GVArGuiInvoice.guiInvoiceNbr,
                            Equal<Required<GVArGuiInvoice.guiInvoiceNbr>>>>
                            .Select(this, line.GuiInvoiceNbr);
            foreach (GVArGuiInvoice guiInvNbr in set)
            {
                line.InvoiceDate = guiInvNbr.InvoiceDate;
                line.SalesAmt = guiInvNbr.SalesAmt;
                line.CustomerID = guiInvNbr.CustomerID;
                PXResultset<BAccount> customer = PXSelect<BAccount,
                        Where<BAccount.bAccountID,
                            Equal<Required<BAccount.bAccountID>>>>
                            .Select(this, line.CustomerID);
                foreach (BAccount customercd in customer)
                {
                    line.Customer = customercd.AcctCD;
                }
            }
        }

        protected virtual void GVArGuiZeroDoc_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            GVArGuiZeroDoc doc = (GVArGuiZeroDoc)e.Row;
            if (sender.GetStatus(doc) == PXEntryStatus.Inserted)
            {
                if (doc.Status == null)
                {

                    doc.Status = GVList.GVStatus.RELEASE;

                }
            }
        }
        protected virtual void GVArGuiZeroDoc_DeclareYear_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            GVArGuiZeroDoc row = (GVArGuiZeroDoc)e.Row;
            if (row.DeclareYear == null || e.NewValue == null)
            {
                DateTime systemdate = DateTime.Today;
                int year = systemdate.Year;
                e.NewValue = year;
            }
            if (row.DeclareYear != null || e.NewValue == null)
            {
                e.NewValue = true;
                e.Cancel = true;
            }
        }
        protected virtual void GVArGuiZeroDoc_DeclareMonth_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            GVArGuiZeroDoc row = (GVArGuiZeroDoc)e.Row;
            if (row.DeclareMonth == null)
            {
                DateTime systemdate = DateTime.Today;
                int month = systemdate.Month;
                e.NewValue = month;
            }
            if (row.DeclareMonth != null)
            {
                e.NewValue = true;
                e.Cancel = true;
            }
        }
        protected virtual void GVArGuiZeroDoc_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            GVArGuiZeroDoc row = (GVArGuiZeroDoc)e.Row;
            BAccount bAccount = PXSelectorAttribute.Select<GVArGuiInvoice.customerID>(sender, row) as BAccount;
            int? locationID = bAccount.DefLocationID;
            row.CustUniformNumber = bAccount.TaxRegistrationID;
            PXResultset<LocationExtAddress> set = PXSelect<LocationExtAddress, Where<LocationExtAddress.locationID,
                                                    Equal<Required<LocationExtAddress.locationID>>>>.Select(this, locationID);
            LocationExtAddress loction = set;
            if (loction != null)
            {
                row.CustUniformNumber = loction.TaxRegistrationID;
            }
        }
        protected virtual void GVArGuiZeroDoc_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            GVArGuiZeroDoc doc = (GVArGuiZeroDoc)e.Row;
            setCheckThread(checkDocNbr_M(sender, doc, doc.DocNbr));
            e.Cancel = getCheckThread();
            if (sender.GetStatus(doc) == PXEntryStatus.Inserted)
            {
                /*PXResultset<GVArGuiZeroDoc> nbr = PXSelect<GVArGuiZeroDoc,
						Where<GVArGuiZeroDoc.zeroDocCD,
							Equal<Required<GVArGuiZeroDoc.zeroDocCD>>>>
							.Select(this);
				foreach (GVArGuiZeroDoc uniformNumber in nbr)
				{
					string checkNumber = "Z" + DateTime.Now.ToString("yyyyMMdd") + "0001";
					if (uniformNumber.ZeroDocCD != checkNumber)
					{
						GVSetupMaint graph = PXGraph.CreateInstance<GVSetupMaint>();
						GVSetup setup = new GVSetup();
						setup.ZeroDocCDNbr = "0000";
						graph.LastNumbers.Update(setup);
					}

				}*/
                AutoNumberAttribute.SetLastNumberField<GVArGuiZeroDoc.zeroDocCD>(
                    sender, doc,
                                typeof(GVSetup.zeroDocCDNbr));
                AutoNumberAttribute.SetPrefix<GVArGuiZeroDoc.zeroDocCD>(
                    sender, doc, "Z");
            }
        }
        #endregion

        #region Verify
        protected virtual void GVArGuiZeroDoc_DocNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVArGuiZeroDoc row = (GVArGuiZeroDoc)e.Row;
            if (row == null)
            {
                return;
            }
            if (row.DocNbr != null)
            {
                setCheckThread(checkDocNbr_M(sender, row, (String)row.DocNbr));
            }
        }

        public bool checkDocNbr_M(PXCache sender, GVArGuiZeroDoc row, String newValue)
        {
            string DocNbrs = (string)newValue;
            if (row.DocNbr == null || DocNbrs == null)
            {
                sender.RaiseExceptionHandling<GVArGuiZeroDoc.docNbr>(
                        row, newValue,
                          new PXSetPropertyException(
                          "DocNbr cannot be null",
                          PXErrorLevel.Error));
                return true;
            }
 
            int intDocNbrs = DocNbrs.Length;
            if (intDocNbrs < 14)
            {

                sender.RaiseExceptionHandling<GVArGuiZeroDoc.docNbr>(
                        row, newValue,
                          new PXSetPropertyException(
                          "DocNbr is too short!",
                          PXErrorLevel.Error));
                return true;
            }
            else if (intDocNbrs > 14)
            {

                sender.RaiseExceptionHandling<GVArGuiZeroDoc.docNbr>(
                        row, newValue,
                          new PXSetPropertyException(
                          "DocNbr is too long!",
                          PXErrorLevel.Error));
                return true;
            }
            return false;
        }
        #endregion

        #region Method
        public override void Persist()
        {
            GVArGuiZeroDoc row = gvArGuiZeroDocs.Current;
            setCheckThread(checkDocNbr_M(gvArGuiZeroDocs.Cache, row, row.DocNbr));
            
            foreach (GVArGuiZeroDocLine details in gvArGuiZeroDocLines.Cache.Inserted)
            {
                //setCheckThread(checkBAccountID_M(baccounts.Cache, details, details.BAccountID));
            }
            foreach (GVArGuiZeroDocLine details in gvArGuiZeroDocLines.Cache.Updated)
            {
                //setCheckThread(checkBAccountID_M(baccounts.Cache, details, details.BAccountID));
            }
            
            if (getCheckThread())
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            }
            base.Persist();

        }

        #endregion
    }



}