using System;
using System.Collections.Generic;
using Kedge.DAC;
using NM.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using static NM.Util.NMStringList;

namespace NM
{
    public class NMApPaymentProcess : PXGraph<NMApPaymentProcess>
    {

        public PXCancel<KGBillPayment> Cancel;

        #region View
        public PXProcessing<KGBillPayment, Where<KGBillPaymentExt.usrCreatePaymentType, IsNotNull>> ProcessDatas;
        public PXSelect<NMApPaymentProcessLog> Logs;
        #endregion

        public NMApPaymentProcess()
        {
            ProcessDatas.SetProcessCaption("Process");
            ProcessDatas.SetProcessVisible(true);
            ProcessDatas.SetProcessAllCaption("Process All");
            ProcessDatas.SetProcessAllVisible(true);
            NMApPaymentProcess self = this;
            ProcessDatas.SetProcessDelegate(datas => Process(self, datas));
        }

        #region Method
        public static void Process(NMApPaymentProcess self, List<KGBillPayment> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                KGBillPayment data = datas[i];
                try
                {
                    KGBillPaymentExt ext = data.GetExtension<KGBillPaymentExt>();

                    if (ext.UsrCreatePaymentType == NMApCreatePaymentType.CHECK)
                    {
                        self.CreateAPPaymentByCheck(data);
                        PXUpdate<
                            Set<KGBillPaymentExt.usrCreatePaymentType, Null>,
                            KGBillPayment,
                            Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                            .Update(self, data.BillPaymentID);
                        //data.GetExtension<KGBillPaymentExt>().UsrCreatePaymentType = null;
                        //self.ProcessDatas.Update(data);
                    }
                    else if (ext.UsrCreatePaymentType == NMApCreatePaymentType.TT)
                    {
                        NMApTeleTransLog log = self.GetLogByBillPaymentID(data.BillPaymentID);
                        //2022-10-17 Alton APPayment.UsrTTUploadKey 改放 CustNbr
                        self.CreateAPPaymentByTT(data, log.CustNbr);
                        PXUpdate<
                            Set<KGBillPaymentExt.usrCreatePaymentType, Null>,
                            KGBillPayment,
                            Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                            .Update(self, data.BillPaymentID);
                        //data.GetExtension<KGBillPaymentExt>().UsrCreatePaymentType = null;
                        //self.ProcessDatas.Update(data);
                    }
                }
                catch (PXOuterException e)
                {
                    string errorMsg = "";
                    for (int j = 0; j < e.InnerFields.Length; j++) {
                        errorMsg += $"[{e.InnerFields[j]} - {e.InnerMessages[j]}] \r\n";
                    }
                    self.Logs.Insert(new NMApPaymentProcessLog()
                    {
                        RefNbr = data.RefNbr,
                        BillPaymentID = data.BillPaymentID,
                        Message = errorMsg
                    }) ;
                    PXProcessing<KGBillPayment>.SetError(i, $"{e.Message} \r\n {errorMsg}");
                }
                catch (PXException e)
                {
                    self.Logs.Insert(new NMApPaymentProcessLog()
                    {
                        RefNbr = data.RefNbr,
                        BillPaymentID = data.BillPaymentID,
                        Message = e.Message
                    });
                    PXProcessing<KGBillPayment>.SetError(i, e.Message);
                }
            }
            self.Persist();
        }

        public virtual void CreateAPPaymentByCheck(KGBillPayment data)
        {
            APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                var check = GetCheckByBillPaymentID(data.BillPaymentID);
                APPayment apPayment = NMApCheckEntry.CreateApPayment(entry, ref check, false);
                entry.Document.SetValueExt<APPayment.hold>(apPayment, false);
                apPayment = entry.Document.Update(apPayment);
                entry.Save.Press();
                entry.release.Press();
                ts.Complete(entry);
            }
        }

        public virtual void CreateAPPaymentByTT(KGBillPayment data, string uploadKey)
        {
            APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                APPayment apPayment = (APPayment)entry.Document.Cache.CreateInstance();
                KGBillPayment billPayment = data;
                APInvoice invoice = GetAPInvoiceByRefNbr(entry, billPayment.RefNbr);

                apPayment.CuryID = invoice.CuryID;
                apPayment.CuryInfoID = invoice.CuryInfoID;
                apPayment.BranchID = invoice.BranchID;
                apPayment.Hold = true;
                apPayment.DocType = APDocType.Check;
                apPayment.VendorID = invoice.VendorID;
                //20220325 by louis 修改APPayment Vendor/vendorLocation都要抓APInvoice表頭的
                //Location location = GetLocation(billPayment.VendorLocationID, invoice.VendorID);
                Location location = Location.PK.Find(entry, invoice.VendorID, invoice.VendorLocationID);

                //20220325 by louis 修改APPayment Vendor/vendorLocation都要抓APInvoice表頭的
                apPayment.VendorLocationID = location.LocationID; //billPayment.VendorLocationID;
                apPayment.PaymentMethodID = location.VPaymentMethodID;
                apPayment.CashAccountID = location.CashAccountID;

                apPayment = entry.Document.Insert(apPayment);

                //20220428 by louis APPayment.AdjDate寫入實際的付款日
                //20220721 by louis APPayment.AdjDate寫入電匯回饋檔實際付款日期
                apPayment.DocDate = apPayment.AdjDate = data.TtActDate;
                apPayment.CuryOrigDocAmt = billPayment.PaymentAmount;
                apPayment.ExtRefNbr = invoice.RefNbr;
                //20221011 by Alton TT銀行電匯檔的UploadKey要麻煩回饋後放到APPayment上擴一個欄位來放這個值 (UsrTTUploadKey) 
                apPayment.GetExtension<APPaymentExt>().UsrTTUploadKey = uploadKey;

                entry.Document.Update(apPayment);

                #region 明細
                APAdjust detail = (APAdjust)entry.Adjustments.Cache.CreateInstance();
                detail.AdjdDocType = invoice.DocType;
                detail.AdjdRefNbr = invoice.RefNbr;
                detail = entry.Adjustments.Insert(detail);
                entry.Adjustments.Cache.SetValueExt<APAdjust.curyAdjgAmt>(detail, billPayment.PaymentAmount);
                entry.Adjustments.Update(detail);
                #endregion

                entry.Document.Cache.SetValueExt<APPayment.hold>(apPayment, false);
                // Store the source field value as suggested by Louis for future auditing.
                entry.Document.Cache.SetValueExt<APPaymentExt.usrBillPaymentID>(apPayment, billPayment.BillPaymentID);
                entry.Document.Update(apPayment);
                entry.Save.Press();
                entry.release.Press();

                ts.Complete(entry);
            }
        }
        #endregion


        #region BQL
        protected virtual PXResultset<APInvoice> GetAPInvoiceByRefNbr(PXGraph graph, string refNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                 .Select(graph, refNbr);
        }

        //protected virtual PXResultset<KGBillPayment> GetBillPayment(PXGraph graph, int? billPaymentID)
        //{
        //    return PXSelect<KGBillPayment,
        //        Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
        //         .Select(graph, billPaymentID);
        //}

        protected virtual NMApTeleTransLog GetLogByBillPaymentID(int? billPaymentID)
        {
            return PXSelect<NMApTeleTransLog,
                 Where<NMApTeleTransLog.billPaymentID, Equal<Required<NMApTeleTransLog.billPaymentID>>,
                    And<NMApTeleTransLog.status, Equal<NMApTtLogStatus.feedBackSuccess>>
                 >>
                  .Select(this, billPaymentID);
        }

        protected virtual NMPayableCheck GetCheckByBillPaymentID(int? billPaymentID)
        {
            return PXSelect<NMPayableCheck,
                 Where<NMPayableCheck.billPaymentID, Equal<Required<NMPayableCheck.billPaymentID>>>
                 >.Select(this, billPaymentID);
        }

        #endregion
    }
}