using NM.DAC;
using PX.Data;
using System;
using System.Collections;
using static NM.Util.NMStringList;

namespace NM
{
    public class NMUploadHistorical : PXGraph<NMUploadHistorical>, PXImportAttribute.IPXPrepareItems
    {


        public PXCancel<NMUploadHistoricalTempData> Cancel;
        #region View
        [PXImport(typeof(NMUploadHistoricalTempData))]
        public PXSelect<NMUploadHistoricalTempData> DetailsView;
        #endregion

        #region Button
        public PXAction<NMUploadHistoricalTempData> Import;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "¶×¤J")]
        protected void import()
        {

            foreach (NMUploadHistoricalTempData item in DetailsView.Select())
            {
                try
                {
                    NMApCheckEntry graph = CreateInstance<NMApCheckEntry>();
                    CreateCheck(ref graph, item);
                    graph.Persist();
                    item.PayableCheckCD = graph.PayableChecks.Current?.PayableCheckCD;
                }
                catch (Exception e)
                {
                    item.Error = e.Message;
                    DetailsView.Cache.RaiseExceptionHandling<NMUploadHistoricalTempData.tempSeq>(
                                item, item.TempSeq, new PXSetPropertyException(e.Message, PXErrorLevel.RowError));
                    throw e;

                }
            }

        }
        #endregion

        #region Method
        public virtual void CreateCheck(ref NMApCheckEntry entry, NMUploadHistoricalTempData item)
        {
            NMPayableCheck newItem = (NMPayableCheck)entry.PayableChecks.Cache.CreateInstance();
            PXCache cache = entry.PayableChecks.Cache;
            newItem = entry.PayableChecks.Insert(newItem);
            //cache.SetValueExt<NMPayableCheck.refNbr>(newItem, item.RefNbr);
            cache.SetValueExt<NMPayableCheck.projectID>(newItem, item.ProjectID);
            cache.SetValueExt<NMPayableCheck.projectPeriod>(newItem, item.ProjectPeriod ?? 0);
            cache.SetValueExt<NMPayableCheck.sourceCode>(newItem, NMAPSourceCode.HISTORYCHECK);
            //cache.SetValueExt<NMPayableCheck.curyID>(newItem, item.CuryID);
            //cache.SetValueExt<NMPayableCheck.curyInfoID>(newItem, item.CuryInfoID);
            cache.SetValueExt<NMPayableCheck.oriCuryAmount>(newItem, item.OriCuryAmount);
            //cache.SetValueExt<NMPayableCheck.baseCuryAmount>(newItem,);
            cache.SetValueExt<NMPayableCheck.branchID>(newItem, entry.Accessinfo.BranchID);
            cache.SetValueExt<NMPayableCheck.checkDate>(newItem, item.CheckDate);
            cache.SetValueExt<NMPayableCheck.etdDepositDate>(newItem, item.EtdDepositDate);
            cache.SetValueExt<NMPayableCheck.vendorID>(newItem, item.VendorID);
            cache.SetValueExt<NMPayableCheck.vendorLocationID>(newItem, item.VendorLocationID);
            cache.SetValueExt<NMPayableCheck.title>(newItem, item.CheckTitle);
            cache.SetValueExt<NMPayableCheck.bankAccountID>(newItem, item.BankAccountID);
            cache.SetValueExt<NMPayableCheck.checkNbr>(newItem, item.CheckNbr);
            //cache.SetValueExt<NMPayableCheck.bookNbr>(newItem, item.BookNbr);
            cache.SetValueExt<NMPayableCheck.payableCashierID>(newItem, item.PayableCashierID);
            cache.SetValueExt<NMPayableCheck.description>(newItem, item.Description);
            newItem = entry.PayableChecks.Update(newItem);
            cache.SetValueExt<NMPayableCheck.status>(newItem, NMAPCheckStatus.CONFIRM);
            //cache.SetValueExt<NMPayableCheck.billPaymentID>(newItem, item.BillPaymentID);
            entry.PayableChecks.Update(newItem);
            entry.Persist();
        }
        #endregion

        #region Import
        bool preImport = true;
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (preImport)
            {
                foreach (NMUploadHistoricalTempData item in DetailsView.Select())
                {
                    DetailsView.Delete(item);
                }
                preImport = false;
            }
            return true;
        }

        public bool RowImporting(string viewName, object row)
        {
            return true;
        }

        public bool RowImported(string viewName, object row, object oldRow)
        {
            return true;
        }

        public void PrepareItems(string viewName, IEnumerable items)
        {
        }
        #endregion


    }
}