using System;
using System.Collections;
using System.Collections.Generic;
using PS.DAC;
using PX.Data;
using PX.Objects.CR;
using static PS.Util.PSStringList;

namespace PS
{
    public class PSUploadPaymentSlip : PXGraph<PSUploadPaymentSlip>, PXImportAttribute.IPXPrepareItems
    {

        public PXCancel<PSUploadPaymentSlipTemp> Cancel;
        #region View
        [PXImport(typeof(PSUploadPaymentSlipTemp))]
        public PXSelect<PSUploadPaymentSlipTemp> DetailsView;
        #endregion


        #region Button
        public PXAction<PSUploadPaymentSlipTemp> Import;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "匯入")]
        protected void import()
        {
            //PXLongOperation.StartOperation(this, delegate ()
            //{
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                Dictionary<String, List<PSUploadPaymentSlipTemp>> group = GetGroupData();
                foreach (string refNbr in group.Keys)
                {
                    try
                    {
                        PSPaymentSlipEntry graph = CreateInstance<PSPaymentSlipEntry>();
                        bool isFirst = true;
                        foreach (PSUploadPaymentSlipTemp item in group[refNbr])
                        {
                            if (isFirst)
                            {
                                SetHeader(ref graph, item);
                                isFirst = false;
                                graph.Persist();
                            }
                            SetDetail1(ref graph, item);
                            graph.Persist();
                            item.RefNbr = graph.PaymentSlips.Current?.RefNbr;
                        }
                        graph.Submit.Press();
                    }
                    catch (Exception e)
                    {
                        foreach (PSUploadPaymentSlipTemp item in group[refNbr])
                        {
                            item.Error = e.Message;
                            DetailsView.Cache.RaiseExceptionHandling<PSUploadPaymentSlipTemp.tempSeq>(
                                        item, item.TempSeq, new PXSetPropertyException(e.Message, PXErrorLevel.RowError));
                        }
                        throw e;

                    }

                }


                //foreach (PSUploadPaymentSlipTemp item in DetailsView.Select())
                //{
                //    try
                //    {
                //        PSPaymentSlipEntry graph = CreateInstance<PSPaymentSlipEntry>();
                //        item.Error = "";
                //        SetHeader(ref graph, item);
                //        graph.Persist();
                //        SetDetail1(ref graph, item);
                //        graph.Persist();
                //        graph.Submit.Press();
                //        //2021-09-10 mark : 使用者說要自己來key資金付款 !@#$%
                //        //SetDetail2(ref graph, item);
                //        //graph.Persist();
                //        item.RefNbr = graph.PaymentSlips.Current?.RefNbr;
                //        //graph.Release.Press();
                //    }
                //    catch (Exception e)
                //    {
                //        item.Error = e.Message;
                //        DetailsView.Cache.RaiseExceptionHandling<PSUploadPaymentSlipTemp.tempSeq>(
                //                    item, item.TempSeq, new PXSetPropertyException(e.Message, PXErrorLevel.RowError));
                //        throw e;
                //    }
                //}
                ts.Complete();
            }
            //});
        }
        #endregion

        #region Method
        public Dictionary<String, List<PSUploadPaymentSlipTemp>> GetGroupData()
        {
            Dictionary<String, List<PSUploadPaymentSlipTemp>> group = new Dictionary<String, List<PSUploadPaymentSlipTemp>>();
            foreach (PSUploadPaymentSlipTemp item in DetailsView.Select())
            {
                if (!group.ContainsKey(item.RefNbr))
                {
                    group.Add(item.RefNbr, new List<PSUploadPaymentSlipTemp>());
                }
                group[item.RefNbr].Add(item);
            }
            return group;
        }

        public void SetHeader(ref PSPaymentSlipEntry graph, PSUploadPaymentSlipTemp item)
        {
            PXCache cache = graph.PaymentSlips.Cache;
            PSPaymentSlip row = null;
            if (!String.IsNullOrEmpty(item.RefNbr))
            {
                row = GetPSPaymentSlip(item.RefNbr);
            }
            if (row == null)
            {
                row = (PSPaymentSlip)cache.CreateInstance();
                row.DocType = item.DocType;
                row.TargetType = item.TargetType;

                row = graph.PaymentSlips.Insert(row);
            }
            else
            {
                graph.PaymentSlips.Current = row;
            }
            cache.SetValueExt<PSPaymentSlip.docDate>(row, item.DocDate);
            cache.SetValueExt<PSPaymentSlip.targetType>(row, item.TargetType);
            BAccount payer = (BAccount)PXSelectorAttribute.Select<PSUploadPaymentSlipTemp.payerID>(DetailsView.Cache, item);
            switch (item.TargetType)
            {
                case PSTargetType.Vendor:
                    //cache.SetValueExt<PSPaymentSlip.vendorID>(row, item.PayerID);
                    row.VendorID = item.PayerID;
                    row.VendorLocationID = item.PayerLocationID ?? payer?.DefLocationID;
                    break;
                case PSTargetType.Customer:
                    //cache.SetValueExt<PSPaymentSlip.customerID>(row, item.PayerID);
                    row.CustomerID = item.PayerID;
                    row.CustomerLocationID = item.PayerLocationID ?? payer?.DefLocationID;
                    break;
                case PSTargetType.Employee:
                    //cache.SetValueExt<PSPaymentSlip.employeeID>(row, item.PayerID);
                    row.EmployeeID = item.PayerID;
                    break;
            }
            //row = graph.PaymentSlips.Update(row);

            cache.SetValueExt<PSPaymentSlip.contractID>(row, item.ContractID);
            cache.SetValueExt<PSPaymentSlip.docDesc>(row, item.DocDesc);
            //row = graph.PaymentSlips.Update(row);
        }

        public void SetDetail1(ref PSPaymentSlipEntry graph, PSUploadPaymentSlipTemp item)
        {
            PXCache cache = graph.PaymentSlipDetails.Cache;
            //PSPaymentSlipDetails row = graph.PaymentSlipDetails.Select();
            //if (row == null)
            //{
            //    row = (PSPaymentSlipDetails)cache.CreateInstance();
            //    row = graph.PaymentSlipDetails.Insert(row);
            //}

            PSPaymentSlipDetails row = (PSPaymentSlipDetails)cache.CreateInstance();
            row = graph.PaymentSlipDetails.Insert(row);

            cache.SetValueExt<PSPaymentSlipDetails.slipDate>(row, item.LineDate);
            cache.SetValueExt<PSPaymentSlipDetails.paymentRefNbr>(row, item.LinePayNbr);
            cache.SetValueExt<PSPaymentSlipDetails.paymentMethodID>(row, item.LinePayMehtod);
            cache.SetValueExt<PSPaymentSlipDetails.inventoryID>(row, item.LineInvID);
            cache.SetValueExt<PSPaymentSlipDetails.tranDesc>(row, item.LineDesc);
            cache.SetValueExt<PSPaymentSlipDetails.qty>(row, item.LineQty);
            cache.SetValueExt<PSPaymentSlipDetails.unitCost>(row, item.LineCost);
            cache.SetValueExt<PSPaymentSlipDetails.checkIssuer>(row, item.LineIssues);
            cache.SetValueExt<PSPaymentSlipDetails.oriBankCode>(row, item.IssueBankCode);
            cache.SetValueExt<PSPaymentSlipDetails.oriBankAccount>(row, item.IssueBankAccount);
            if (item.TargetType == PSTargetType.Employee)
            {
                cache.SetValueExt<PSPaymentSlipDetails.employeeID>(row, item.EmployeeID);
            }
            if (item.DocType == PSDocType.ArGuarCheck)
            {
                cache.SetValueExt<PSPaymentSlipDetails.guarType>(row, item.GuarType);
                cache.SetValueExt<PSPaymentSlipDetails.guarClass>(row, item.GuarClass);
                cache.SetValueExt<PSPaymentSlipDetails.pONbr>(row, item.PONbr);
                cache.SetValueExt<PSPaymentSlipDetails.issueDate>(row, item.IssueDate);
                cache.SetValueExt<PSPaymentSlipDetails.authDate>(row, item.AuthDate);
            }
            row = graph.PaymentSlipDetails.Update(row);
        }

        public void SetDetail2(ref PSPaymentSlipEntry graph, PSUploadPaymentSlipTemp item)
        {
            PXCache cache = graph.PaymentSlipDetails.Cache;
            PSPaymentSlipDetails row = graph.PaymentSlipDetails.Select();
            if (row == null) return;
            cache.SetValueExt<PSPaymentSlipDetails.paymentCategory>(row, item.PaymentCategory);
            cache.SetValueExt<PSPaymentSlipDetails.bankAccountID>(row, item.BankAccountID);
            cache.SetValueExt<PSPaymentSlipDetails.actualDueDate>(row, item.ActualDueDate);
            cache.SetValueExt<PSPaymentSlipDetails.checkDueDate>(row, item.CheckDueDate);
            cache.SetValueExt<PSPaymentSlipDetails.etdDepositDate>(row, item.EtdDepositDate);
            cache.SetValueExt<PSPaymentSlipDetails.processDate>(row, item.ProcessDate);
            row = graph.PaymentSlipDetails.Update(row);
        }

        #endregion

        #region BQL
        private PSPaymentSlip GetPSPaymentSlip(string refNbr)
        {
            return PXSelect<PSPaymentSlip, Where<PSPaymentSlip.refNbr, Equal<Required<PSPaymentSlip.refNbr>>>>
                .Select(this, refNbr);
        }

        #endregion

        #region Import
        bool preImport = true;
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (preImport)
            {
                foreach (PSUploadPaymentSlipTemp item in DetailsView.Select())
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