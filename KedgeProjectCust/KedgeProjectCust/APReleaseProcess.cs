using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.PM;
using PX.Objects.IN;
using PX.Objects.TX;
using RCGV.GV.DAC;
using RCGV.GV;
using RCGV.GV.Util;

/**
 * ====2021-04-14:11974 ==== Alton
 * 當APInvoice過帳時補上GVGuiInvoice明細資料
 * 
 * ====2021-05-26:口頭====Alton
 * 1.GVGuiInvoice明細資料需補上計價單號
 * 2.過帳時發票直接確認
 * **/
namespace PX.Objects.AP
{
    public class APReleaseProcess_Extension : PXGraphExtension<APReleaseProcess>
    {
        public PXSelect<APRegister> APDocument;

        #region Override Funcation
        public delegate List<APRegister> ReleaseInvoiceDelegate(JournalEntry je, ref APRegister doc,
                                                                PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res,
                                                                Boolean isPrebooking, out List<INRegister> inDocs);
        [PXOverride]
        public List<APRegister> ReleaseInvoice(JournalEntry je, ref APRegister doc,
                                               PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res,
                                               Boolean isPrebooking, out List<INRegister> inDocs,
                                               ReleaseInvoiceDelegate baseMethod)
        {
            String origDocType = null, origRefNbr = null;
            if (doc.IsRetainageDocument == true && checkIsCustomRetainage(doc))
            {
                origDocType = doc.OrigDocType;
                origRefNbr = doc.OrigRefNbr;
                doc.OrigDocType = "";
                doc.OrigRefNbr = "";
            }
            var ret = baseMethod(je, ref doc, res, isPrebooking, out inDocs);

            if (doc.IsRetainageDocument == true && checkIsCustomRetainage(doc))
            {
                doc.OrigDocType = origDocType;
                doc.OrigRefNbr = origRefNbr;
            }
            foreach (GLTran gLTran in je.GLTranModuleBatNbr.Cache.Inserted)
            {
                if (gLTran.ProjectID == ProjectDefaultAttribute.NonProject())
                {
                    gLTran.ProjectID = doc.ProjectID;
                    gLTran.TaskID = Base.APTran_TranType_RefNbr.Current.TaskID;
                }
            }

            #region Retainage part
            //保留款退回追加
            if (doc.IsRetainageDocument == true && checkIsCustomRetainage(doc))
            {
                APInvoice apdoc = res;
                bool isDebit = (apdoc.DrCr == DrCr.Debit);
                Decimal? totalRetainageAmt = 0;
                foreach (APTran apTran in PXSelect<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>,
                                                        And<APTran.refNbr, Equal<Required<APTran.refNbr>>>>>.Select(Base, doc.DocType, doc.RefNbr))
                {

                    APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                    APRegister origRetainageDoc = PXSelect<APInvoice, Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
                                                        And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(Base, apTranExt.UsrOrigDocType, apTranExt.UsrOrigRefNbr);
                    // We should update unreleased retainage amount
                    // for the original retainage bill in the case
                    // when it is a child retainage document.
                    // 
                    if (origRetainageDoc != null)
                    {
                        Decimal? releaseAmt = (apTran.CuryTranAmt + apdoc.CuryRoundDiff) * apdoc.SignAmount;
                        APTaxTran apTaxTran = PXSelectJoin<APTaxTran, LeftJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>,
                                Where<APTaxTran.module, Equal<BatchModule.moduleAP>,
                                    And<APTaxTran.tranType, Equal<Required<APInvoice.docType>>,
                                    And<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>>.Select(Base, apTranExt.UsrOrigDocType, apTranExt.UsrOrigRefNbr);
                        Decimal? taxRate = 0;
                        if (apTaxTran != null)
                        {
                            taxRate = getValue(apTaxTran.TaxRate) / 100;
                        }
                        totalRetainageAmt = totalRetainageAmt + (releaseAmt * (1 + taxRate));
                    }

                }
                Decimal? totalDiff = doc.CuryOrigDocAmt - totalRetainageAmt;
                //
                bool isAddDiff = false;
                foreach (APTran apTran in PXSelect<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>,
                                                        And<APTran.refNbr, Equal<Required<APTran.refNbr>>>>>.Select(Base, doc.DocType, doc.RefNbr))
                {


                    APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                    APRegister origRetainageDoc = PXSelect<APInvoice, Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
                                                        And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(Base, apTranExt.UsrOrigDocType, apTranExt.UsrOrigRefNbr);
                    // We should update unreleased retainage amount
                    // for the original retainage bill in the case
                    // when it is a child retainage document.
                    // 
                    if (origRetainageDoc != null)
                    {
                        //origRetainageDoc.CuryRetainageUnreleasedAmt -= (apTran.CuryTranAmt + apdoc.CuryRoundDiff) * apdoc.SignAmount;
                        Decimal? releaseAmt = (apTran.CuryTranAmt + apdoc.CuryRoundDiff) * apdoc.SignAmount;

                        APTaxTran apTaxTran = PXSelectJoin<APTaxTran, LeftJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>,
                                Where<APTaxTran.module, Equal<BatchModule.moduleAP>,
                                    And<APTaxTran.tranType, Equal<Required<APInvoice.docType>>,
                                    And<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>>.Select(Base, apTranExt.UsrOrigDocType, apTranExt.UsrOrigRefNbr);
                        Decimal? taxRate = 0;
                        if (apTaxTran != null)
                        {
                            taxRate = getValue(apTaxTran.TaxRate) / 100;
                        }
                        releaseAmt = releaseAmt * (1 + taxRate);
                        Decimal? diff = getValue(origRetainageDoc.CuryRetainageTotal) - getValue(origRetainageDoc.CuryRetainageUnreleasedAmt) - releaseAmt;
                        //
                        if (diff != 0 && !isAddDiff)
                        {
                            isAddDiff = true;
                            releaseAmt = releaseAmt - totalDiff;
                        }
                        origRetainageDoc.CuryRetainageUnreleasedAmt -= releaseAmt;
                        origRetainageDoc = APDocument.Update(origRetainageDoc);
                        // mark by louis 20220217
                        /**
                        if (origRetainageDoc.CuryRetainageUnreleasedAmt < 0m)
                        {
                            throw new PXException(Messages.RetainageUnreleasedBalanceNegative);
                        }
                        **/
                    }
                }
            }
            //Base.Actions.PressSave();

            ///<remarks> Since the historical AP bill hasn't been released, user can release it later, and the standard logic will re-update updated retainage fields back. </remarks>
            if (doc.RetainageApply == true && doc.GetExtension<APRegisterExt>().UsrRetainageHistType == RetHistType.Original && 
                doc.CuryRetainageReleased > 0m && doc. CuryRetainageUnreleasedAmt > 0m)
            {
                doc.CuryRetainageReleased      = doc.RetainageReleased = doc.CuryRetainageTotal;
                doc.CuryRetainageUnreleasedAmt = doc.RetainageUnreleasedAmt = 0m;

                Base.APDocument.Update(doc);

                PXTimeStampScope.DuplicatePersisted(Base.APDocument.Cache, doc, typeof(APInvoice));
            }
            #endregion

            #region GVGuiInvoice
            if (doc.DocType != APDocType.VoidCheck 
                && doc.DocType != APDocType.VoidQuickCheck 
                && doc.DocType != APDocType.VoidRefund
                //add by louis 20220222 for DebitAdj don't create Gui Invoice Detail
                && doc.DocType != APDocType.DebitAdj)
            {
                CreateGVGuiInvoiceDetail(doc.RefNbr, doc);
            }
            #endregion

            return ret;
        }

        #endregion

        #region Method
        public void CreateGVGuiInvoiceDetail(string refNbr, APRegister doc)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (GVApGuiInvoice invoice in GetGVApGuiInvoice(refNbr))
                {
                    GVApInvoiceMaint entry = PXGraph.CreateInstance<GVApInvoiceMaint>();
                    entry.Invoice.Current = invoice;
                    decimal salesAmt = invoice.SalesAmt ?? 0m;
                    decimal taxAmt = invoice.TaxAmt ?? 0m;
                    GVApGuiInvoice master = entry.Invoice.Current;

                    
                    //產生明細
                    GVApGuiInvoiceDetail detail = (GVApGuiInvoiceDetail)entry.InvoiceDetails.Cache.CreateInstance();
                    detail = entry.InvoiceDetails.Insert(detail);
                    PMProject pmProject = GetPMProject(doc.ProjectID);
                    //2021-05-26 add by Alton 明細要補上計價單號
                    entry.InvoiceDetails.Cache.SetValueExt<GVApGuiInvoiceDetail.apRefNbr>(detail, invoice.RefNbr);
                    entry.InvoiceDetails.Cache.SetValueExt<GVApGuiInvoiceDetail.itemDesc>(detail, invoice.Remark ?? doc.DocDesc ?? (pmProject.Description + "款項"));
                    entry.InvoiceDetails.Cache.SetValueExt<GVApGuiInvoiceDetail.qty>(detail, 1m);
                    entry.InvoiceDetails.Cache.SetValueExt<GVApGuiInvoiceDetail.uom>(detail, "式");
                    entry.InvoiceDetails.Cache.SetValueExt<GVApGuiInvoiceDetail.unitPrice>(detail, salesAmt);
                    entry.InvoiceDetails.Cache.SetValueExt<GVApGuiInvoiceDetail.salesAmt>(detail, salesAmt);
                    entry.InvoiceDetails.Cache.SetValueExt<GVApGuiInvoiceDetail.taxAmt>(detail, taxAmt);
                    detail = entry.InvoiceDetails.Update(detail);
                    entry.Save.Press();

                    //重新填寫表頭金額
                    entry.SetTotal();
                    entry.Save.Press();
                    //2021-05-26 add by Alton 過帳時直接確認
                    entry.ConfirmBtn.Press();
                }
                ts.Complete();
            }
        }

        public Decimal? getValue(Decimal? value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value;
            }
        }


        public bool checkIsCustomRetainage(APRegister doc)
        {
            bool isCustom = false;
            foreach (APTran apTran in PXSelect<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>,
                                                    And<APTran.refNbr, Equal<Required<APTran.refNbr>>>>>.Select(Base, doc.DocType, doc.RefNbr))
            {

                APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(apTran);
                APRegister origRetainageDoc = PXSelect<APInvoice, Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
                                                    And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(Base, apTranExt.UsrOrigDocType, apTranExt.UsrOrigRefNbr);
                // We should update unreleased retainage amount
                // for the original retainage bill in the case
                // when it is a child retainage document.
                // 
                if (origRetainageDoc != null)
                {
                    isCustom = true;
                }
            }
            return isCustom;
        }
        #endregion

        #region BQL
        private PMProject GetPMProject(int? projectID)
        {
            return PXSelect<PMProject,
                Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>
                .Select(new PXGraph(), projectID);
        }

        public PXResultset<GVApGuiInvoice> GetGVApGuiInvoice(string refNbr)
        {
            return PXSelect<GVApGuiInvoice,
                Where<GVApGuiInvoice.status, Equal<GVList.GVStatus.hold>,
                And<GVApGuiInvoice.refNbr, Equal<Required<GVApGuiInvoice.refNbr>>>>>
                .Select(Base, refNbr);
        }
        #endregion
    }
}