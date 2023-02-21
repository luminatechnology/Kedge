using System;
using System.Collections;
using NM.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;

namespace NM
{
    public class NMUploadVendorLocation : PXGraph<NMUploadVendorLocation>, PXImportAttribute.IPXPrepareItems
    {

        //public PXSave<DetailsTable> Save;
        public PXCancel<NMUploadVendorLocationTemp> Cancel;

        #region View
        [PXImport(typeof(NMUploadVendorLocationTemp))]
        public PXSelect<NMUploadVendorLocationTemp> DetailsView;
        #endregion

        #region Button
        public PXAction<NMUploadVendorLocationTemp> Import;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "�פJ")]
        protected void import()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (NMUploadVendorLocationTemp item in DetailsView.Select())
                {
                    try
                    {
                        VendorLocationMaint graph = CreateInstance<VendorLocationMaint>();
                        item.Error = "";
                        SetHeader(ref graph, item);
                        SetPaymentDetails(ref graph, item);
                        graph.Persist();
                    }
                    catch (Exception e)
                    {
                        item.Error = e.Message;
                        DetailsView.Cache.RaiseExceptionHandling<NMUploadVendorLocationTemp.vendorID>(
                                    item, item.VendorID, new PXSetPropertyException(e.Message, PXErrorLevel.RowError));
                        throw e;
                    }
                }
                ts.Complete();
            }
        }
        #endregion

        #region Method
        public void SetHeader(ref VendorLocationMaint graph, NMUploadVendorLocationTemp item)
        {
            PXCache catheL = graph.Location.Cache;
            Location l = SearchLocation(item.VendorID, item.VendorLocationCD);
            if (l != null)
            {
                //graph.Location.Current = graph.Location.Search<Location.locationID>(catheL,l.BAccountID, new object[] { l.LocationID});
                graph.Location.Current = l;
            }
            else
            {
                l = new Location();
                l.BAccountID = item.VendorID;
                l.LocationCD = item.VendorLocationCD;
                l = (Location)graph.Location.Insert(l);
                graph.Persist();
                l = (Location)graph.Location.Current;
                //graph.Location.Current = l;
            }
            catheL.SetValueExt<Location.isAPPaymentInfoSameAsMain>(l, item.PaymentSameAsDefault);
            LocationAPPaymentInfo info = graph.APPaymentInfoLocation.Select();
            catheL.SetValueExt<Location.descr>(l, item.VendorLocationName);
            catheL.SetValueExt<Location.taxRegistrationID>(l, item.TaxRegistrationID);
            catheL.SetValueExt<Location.vPaymentMethodID>(l, item.PaymentMethod);
            catheL.SetValueExt<Location.vCashAccountID>(l, null);
            //catheL.SetValueExt<Location.vCashAccountID>(l, item.CashAccount);
            l = (Location)graph.Location.Update(l);
            //l.VCashAccountID = item.CashAccount;
            graph.APPaymentInfoLocation.Cache.SetValueExt<LocationAPPaymentInfo.vPaymentMethodID>(info, item.PaymentMethod);
            graph.APPaymentInfoLocation.Cache.SetValueExt<LocationAPPaymentInfo.vCashAccountID>(info, item.CashAccount);
            graph.APPaymentInfoLocation.Update(info);
            graph.Persist();
        }


        public void SetPaymentDetails(ref VendorLocationMaint graph, NMUploadVendorLocationTemp item)
        {
            PXCache cache = graph.PaymentDetails.Cache;
            Location mainloc = graph.Location.Current;
            foreach (VendorPaymentMethodDetail detail in graph.PaymentDetails.Select(mainloc.BAccountID, mainloc.LocationID, mainloc.VPaymentMethodID))
            {
                switch (detail.DetailID)
                {
                    case "BANK NBR"://�Ȧ�N�X0
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.BankID);
                        break;
                    case "BANK NAME"://�Ȧ�W��1
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.BankName);
                        break;
                    case "BRANCH NBR"://����O2
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.BankBranchID);
                        break;
                    case "BRANCH NAM"://����W��3
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.BankBranchName);
                        break;
                    case "BANK ACCT"://�Ȧ�b��4
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.BankAccount);
                        break;
                    case "ACCT NAME"://�b��W��5
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.BankAccountName);
                        break;
                    case "CATEGORY"://�������O6
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.Category);
                        break;
                    case "CATEGORYID"://���Ҹ��X7
                        cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.TaxRegistrationID);
                        break;
                    case "CHARGE"://����O
                        //cache.SetValueExt<VendorPaymentMethodDetail.detailValue>(detail, item.BankID);
                        break;
                    default:
                        break;
                }
                graph.PaymentDetails.Update(detail);
            }
        }
        #endregion

        #region BQL
        public Location SearchLocation(int? vendorLocationID, string locationCD)
        {
            return PXSelect<Location,
                Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
                And<Location.locationCD, Equal<Required<Location.locationCD>>>>>
                .Select(this, vendorLocationID, locationCD);
        }
        #endregion

        #region Import
        bool preImport = true;
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (preImport)
            {
                foreach (NMUploadVendorLocationTemp item in DetailsView.Select())
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