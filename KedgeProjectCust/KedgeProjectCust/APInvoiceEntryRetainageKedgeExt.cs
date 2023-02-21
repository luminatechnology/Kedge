using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.AP;
using Kedge.DAC;
using static PX.Objects.AP.APInvoiceEntry;
using PX.Objects.PO;
//KG505003
namespace PX.Objects.AP
{
    [TableAndChartDashboardType]
    public class APRetainageReleaseExt : PXGraph<APRetainageReleaseExt>
    {
        public PXFilter<APRetainageFilter> Filter;
        public PXCancel<APRetainageFilter> Cancel;

        [PXFilterable]
        public PXFilteredProcessing<APInvoiceExt, APRetainageFilter> DocumentList;

        public PXSetup<APSetup> APSetup;

        public PXAction<APRetainageFilter> viewDocument;
        [PXButton]
        public virtual IEnumerable ViewDocument(PXAdapter adapter)
        {
            if (DocumentList.Current != null)
            {
                PXRedirectHelper.TryRedirect(DocumentList.Cache, DocumentList.Current, "Document", PXRedirectHelper.WindowMode.NewWindow);
            }
            return adapter.Get();
        }

        protected virtual IEnumerable documentList()
        {
            foreach (APInvoiceExt doc in PXSelect<APInvoiceExt>.Select(this))
            {
                bool hasUnreleasedDocument = false;

                foreach (PXResult<APRetainageInvoice, APTran> res in PXSelectJoin<APRetainageInvoice,
                    LeftJoin<APTran, On<APRetainageInvoice.paymentsByLinesAllowed, Equal<True>,
                        And<APTran.tranType, Equal<APRetainageInvoice.docType>,
                        And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>,
                        And<APTran.origLineNbr, Equal<Required<APTran.origLineNbr>>>>>>>,
                Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                    And<APRetainageInvoice.origDocType, Equal<Required<APInvoice.docType>>,
                    And<APRetainageInvoice.origRefNbr, Equal<Required<APInvoice.refNbr>>,
                        And<APRetainageInvoice.released, NotEqual<True>>>>>>
                    .Select(this, doc.APTranLineNbr, doc.DocType, doc.RefNbr))
                {
                    APRetainageInvoice invoice = res;
                    APTran tran = res;

                    if (invoice.PaymentsByLinesAllowed != true ||
                        tran.LineNbr != null)
                    {
                        hasUnreleasedDocument = true;
                    }
                }

                if (hasUnreleasedDocument == false)
                {
                    foreach (PXResult<APRetainageInvoice, APTran> res in PXSelectJoin<APRetainageInvoice,
                    LeftJoin<APTran,
                        On<APTran.tranType, Equal<APRetainageInvoice.docType>,
                        And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
                Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                    And<APTranExt.usrOrigDocType, Equal<Required<APTranExt.usrOrigDocType>>,
                    And<APTranExt.usrOrigRefNbr, Equal<Required<APTranExt.usrOrigRefNbr>>,
                        And<APRetainageInvoice.released, NotEqual<True>>>>>>
                    .Select(this, doc.DocType, doc.RefNbr))
                    {
                        APRetainageInvoice invoice = res;
                        APTran tran = res;

                        if (invoice.PaymentsByLinesAllowed != true ||
                            tran.LineNbr != null)
                        {
                            hasUnreleasedDocument = true;
                        }
                    }
                }
                if (!hasUnreleasedDocument)
                    yield return doc;
            }
        }

        public APRetainageReleaseExt()
        {
            APSetup setup = APSetup.Current;

            bool isRequireSingleProjectPerDocument = APSetup.Current?.RequireSingleProjectPerDocument == true;

            PXUIFieldAttribute.SetVisible<APRetainageFilter.projectID>(Filter.Cache, null, isRequireSingleProjectPerDocument);
            PXUIFieldAttribute.SetVisible<APInvoiceExt.displayProjectID>(DocumentList.Cache, null,
                isRequireSingleProjectPerDocument || PXAccess.FeatureInstalled<FeaturesSet.paymentsByLines>());
        }

        protected virtual void APRetainageFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            APRetainageFilter filter = e.Row as APRetainageFilter;
            if (filter == null) return;

            bool isAutoRelease = APSetup.Current?.RetainageBillsAutoRelease == true;

            DocumentList.SetProcessDelegate(delegate (List<APInvoiceExt> list)
            {
                APInvoiceEntry graph = CreateInstance<APInvoiceEntry>();
                APInvoiceEntryRetainageKedgeExt retainageExt = graph.GetExtension<APInvoiceEntryRetainageKedgeExt>();

                RetainageOptions retainageOptions = new RetainageOptions();
                retainageOptions.DocDate = filter.DocDate;
                retainageOptions.MasterFinPeriodID = FinPeriodIDAttribute.CalcMasterPeriodID<APRetainageFilter.finPeriodID>(graph.Caches[typeof(APRetainageFilter)], filter);


                retainageExt.ReleaseRetainageProc(list, retainageOptions, isAutoRelease);
            });
        }

        protected virtual void APInvoiceExt_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            APInvoiceExt invoice = e.Row as APInvoiceExt;
            if (invoice == null) return;

            PXUIFieldAttribute.SetEnabled(sender, invoice, false);
            PXUIFieldAttribute.SetEnabled<APInvoiceExt.selected>(sender, invoice, true);
            PXUIFieldAttribute.SetEnabled<APInvoiceExt.retainageReleasePct>(sender, invoice, true);
            PXUIFieldAttribute.SetEnabled<APInvoiceExt.curyRetainageReleasedAmt>(sender, invoice, true);

            if (invoice.Selected ?? true)
            {
                Dictionary<System.String, System.String> errors = PXUIFieldAttribute.GetErrors(sender, invoice, PXErrorLevel.Error);
                if (errors.Count > 0)
                {
                    invoice.Selected = false;
                    DocumentList.Cache.SetStatus(invoice, PXEntryStatus.Updated);
                    sender.RaiseExceptionHandling<APInvoiceExt.selected>(
                        invoice,
                        null,
                        new PXSetPropertyException(Messages.ErrorRaised, PXErrorLevel.RowError));

                    PXUIFieldAttribute.SetEnabled<APInvoiceExt.selected>(sender, invoice, false);
                }
            }
        }

        public override bool IsDirty => false;
    }



    public class APInvoiceEntryRetainageKedgeExt : PXGraphExtension<APInvoiceEntry>
    //public class APInvoiceEntryRetainageKedgeExt  : PXGraph<APInvoiceEntryRetainageKedgeExt, APInvoice>
    {
        /*
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.retainage>();
        }*/

        public override void Initialize()
        {
            base.Initialize();

            RetainageOptions releaseRetainageOptions = ReleaseRetainageOptions.Current;

            PXAction action = Base.Actions["action"];
            if (action != null)
            {
                action.AddMenuAction(releaseRetainageExt);
            }
        }
        protected virtual void APInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            APInvoice document = e.Row as APInvoice;
            if (document == null) return;

            bool isDocumentReleasedOrPrebooked = document.Released == true || document.Prebooked == true;
            bool isDocumentVoided = document.Voided == true;
            bool isDocumentInvoice = document.DocType == APDocType.Invoice;
            bool retainageApply = document.RetainageApply == true;

            releaseRetainageExt.SetEnabled(false);

            if (isDocumentReleasedOrPrebooked || isDocumentVoided)
            {
                releaseRetainageExt.SetEnabled(isDocumentInvoice &&
                    document.Released == true &&
                    retainageApply &&
                    document.CuryRetainageUnreleasedAmt > 0m);
            }
        }

        protected virtual void ClearCurrentDocumentDiscountDetails()
        {
            Base.DiscountDetails
                    .Select()
                    .RowCast<APInvoiceDiscountDetail>()
                    .ForEach(discountDetail => Base.DiscountDetails.Cache.Delete(discountDetail));

            Base.Discount_Row
                .Select()
                .RowCast<APTran>()
                .ForEach(tran => Base.Discount_Row.Cache.Delete(tran));
        }


        //[PXReadOnlyView]
        //[PXCopyPasteHiddenView]
        // APRetainageInvoice class is a APRegister class alias
        // because only APRegister part is affecting by the release process
        // and only this way we can get a proper behavior for the QueryCache mechanism.
        /*
        public PXSelectJoin<APRetainageInvoice,
            InnerJoinSingleTable<APInvoice, On<APInvoice.docType, Equal<APRetainageInvoice.docType>,
                And<APInvoice.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
            Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                And<APRetainageInvoice.origDocType, Equal<Optional<APInvoice.docType>>,
                And<APRetainageInvoice.origRefNbr, Equal<Optional<APInvoice.refNbr>>>>>> RetainageDocuments;*/
        /*
        public PXSelectJoin<APRetainageInvoice,
            LeftJoin<APTran, On<APTran.tranType, Equal<APRetainageInvoice.docType>,
                And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
            Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                And<APTranExt.usrOrigDocType, Equal<Optional<APInvoice.docType>>,
                And<APTranExt.usrOrigRefNbr, Equal<Optional<APInvoice.refNbr>>>>>> RetainageDocuments;*/

        [PXCopyPasteHiddenView]
        public PXFilter<RetainageOptions> ReleaseRetainageOptions;

        public PXAction<APInvoice> releaseRetainageExt;

        [PXUIField(
            DisplayName = "Custom Release Retainage ",
            MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Update)]
        [PXProcessButton]
        [APMigrationModeDependentActionRestriction(
            restrictInMigrationMode: true,
            restrictForRegularDocumentInMigrationMode: true,
            restrictForUnreleasedMigratedDocumentInNormalMode: true)]
        public virtual IEnumerable ReleaseRetainageExt(PXAdapter adapter)
        {
            APInvoice doc = Base.Document.Current;

            if (doc != null &&
                doc.DocType == APDocType.Invoice &&
                doc.RetainageApply == true &&
                doc.CuryRetainageUnreleasedAmt > 0m)
            {
                APRegister reversingDoc;
                if (Base.CheckReversingRetainageDocumentAlreadyExists(Base.Document.Current, out reversingDoc))
                {
                    throw new PXException(
                        Messages.ReleaseRetainageReversingDocumentExists,
                        PXMessages.LocalizeNoPrefix(APDocTypeDict[doc.DocType]),
                        PXMessages.LocalizeNoPrefix(APDocTypeDict[reversingDoc.DocType]),
                        reversingDoc.RefNbr);
                }

                Base.Save.Press();

                APRetainageReleaseExt retainageGraph = PXGraph.CreateInstance<APRetainageReleaseExt>();

                retainageGraph.Filter.Current.DocDate = doc.DocDate;
                retainageGraph.Filter.Current.FinPeriodID = doc.FinPeriodID;
                retainageGraph.Filter.Current.BranchID = doc.BranchID;
                retainageGraph.Filter.Current.VendorID = doc.VendorID;
                retainageGraph.Filter.Current.RefNbr = doc.RefNbr;
                retainageGraph.Filter.Current.ShowBillsWithOpenBalance = doc.OpenDoc == true;

                APInvoiceExt retainageDocToRelease = retainageGraph.DocumentList.SelectSingle();
                if (retainageDocToRelease == null)
                {
                    APRetainageInvoice retainageDoc = PXSelectJoin<APRetainageInvoice,
                        LeftJoin<APTran, On<APTran.tranType, Equal<APRetainageInvoice.docType>,
                            And<APTran.refNbr, Equal<APRetainageInvoice.refNbr>>>>,
                        Where<APRetainageInvoice.isRetainageDocument, Equal<True>,
                            And<APTranExt.usrOrigDocType, Equal<Optional<APInvoice.docType>>,
                            And<APTranExt.usrOrigRefNbr, Equal<Optional<APInvoice.refNbr>>>>>>
                        .Select(Base)
                        .RowCast<APRetainageInvoice>()
                        .FirstOrDefault(row => row.Released != true);

                    throw new PXException(
                        Messages.ReleaseRetainageNotReleasedDocument,
                        PXMessages.LocalizeNoPrefix(APDocTypeDict[retainageDoc.DocType]),
                        retainageDoc.RefNbr,
                        PXMessages.LocalizeNoPrefix(APDocTypeDict[doc.DocType]));
                }

                throw new PXRedirectRequiredException(retainageGraph, nameof(releaseRetainageExt));
            }

            return adapter.Get();
        }

        public virtual void ReleaseRetainageProc(List<APInvoiceExt> list, RetainageOptions retainageOpts, bool isAutoRelease = false)
        {
            bool failed = false;
            List<APInvoice> result = new List<APInvoice>();
            var group = list.GroupBy(row => new { row.DocType, row.RefNbr }).First();
            //Remark By Jerry
            //foreach (var group in list.GroupBy(row => new { row.DocType, row.RefNbr }))
            //{
            APInvoiceExt doc = group.First();
            PXProcessing<APInvoiceExt>.SetCurrentItem(doc);
            decimal curyRetainageSum = group.Sum(row => row.CuryRetainageReleasedAmt ?? 0m);

            try
            {
                Base.Clear(PXClearOption.ClearAll);
                PXUIFieldAttribute.SetError(Base.Document.Cache, null, null, null);

                APTran tranMax = null;
                TaxCalc oldTaxCalc = TaxBaseAttribute.GetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null);

                Base.Clear(PXClearOption.PreserveTimeStamp);

                if (doc.CuryRetainageReleasedAmt <= 0 || doc.CuryRetainageReleasedAmt > doc.CuryRetainageBal)
                {
                    throw new PXException(Messages.IncorrectRetainageAmount);
                }

                // Magic. We need to prevent rewriting of CurrencyInfo.IsReadOnly 
                // by true in CurrencyInfoView
                // 
                Base.CurrentDocument.Cache.AllowUpdate = true;

                PXResult<APInvoice, CurrencyInfo, Terms, Vendor> resultDoc =
                    APInvoice_CurrencyInfo_Terms_Vendor
                        .SelectSingleBound(Base, null, doc.DocType, doc.RefNbr, doc.VendorID).AsEnumerable()
                        .Cast<PXResult<APInvoice, CurrencyInfo, Terms, Vendor>>()
                        .First();

                CurrencyInfo info = resultDoc;
                APInvoice origInvoice = resultDoc;
                Vendor vendor = resultDoc;

                CurrencyInfo new_info = PXCache<CurrencyInfo>.CreateCopy(info);
                new_info.CuryInfoID = null;
                new_info.IsReadOnly = false;
                new_info = PXCache<CurrencyInfo>.CreateCopy(Base.currencyinfo.Insert(new_info));

                APInvoice invoice = PXCache<APInvoice>.CreateCopy(origInvoice);
                invoice.CuryInfoID = new_info.CuryInfoID;
                invoice.DocType = APDocType.Invoice;
                invoice.RefNbr = null;
                invoice.LineCntr = null;
                invoice.InvoiceNbr = origInvoice.InvoiceNbr;

                // Must be set for _RowSelected event handler
                // 
                invoice.OpenDoc = true;
                invoice.Released = false;

                Base.Document.Cache.SetDefaultExt<APInvoice.isMigratedRecord>(invoice);
                invoice.BatchNbr = null;
                invoice.PrebookBatchNbr = null;
                invoice.Prebooked = false;
                invoice.ScheduleID = null;
                invoice.Scheduled = false;
                invoice.NoteID = null;

                invoice.DueDate = null;
                invoice.DiscDate = null;
                invoice.CuryOrigDiscAmt = 0m;
                //塞空字串不能塞Null
                //invoice.OrigDocType = "";
                //invoice.OrigRefNbr = "";
                invoice.OrigDocType = origInvoice.DocType;
                invoice.OrigRefNbr = origInvoice.RefNbr;
                System.String origDocType = origInvoice.DocType;
                System.String OrigRefNbr = origInvoice.RefNbr;
                invoice.OrigDocDate = origInvoice.DocDate;

                invoice.PaySel = false;
                invoice.CuryLineTotal = 0m;
                invoice.IsTaxPosted = false;
                invoice.IsTaxValid = false;
                invoice.CuryVatTaxableTotal = 0m;
                invoice.CuryVatExemptTotal = 0m;

                invoice.CuryDocBal = 0m;
                invoice.CuryOrigDocAmt = curyRetainageSum;
                invoice.Hold = !isAutoRelease && Base.apsetup.Current.HoldEntry == true || Base.IsApprovalRequired(invoice, Base.Document.Cache);

                invoice.DocDate = retainageOpts.DocDate;
                FinPeriodIDAttribute.SetPeriodsByMaster<APInvoice.finPeriodID>(Base.Document.Cache, invoice, retainageOpts.MasterFinPeriodID);

                Base.ClearRetainageSummary(invoice);
                invoice.RetainageApply = false;
                invoice.IsRetainageDocument = true;

                invoice = Base.Document.Insert(invoice);

                if (new_info != null)
                {
                    CurrencyInfo b_info = (CurrencyInfo)PXSelect<CurrencyInfo,
                        Where<CurrencyInfo.curyInfoID, Equal<Current<APInvoice.curyInfoID>>>>.Select(Base);

                    b_info.CuryID = new_info.CuryID;
                    b_info.CuryEffDate = new_info.CuryEffDate;
                    b_info.CuryRateTypeID = new_info.CuryRateTypeID;
                    b_info.CuryRate = new_info.CuryRate;
                    b_info.RecipRate = new_info.RecipRate;
                    b_info.CuryMultDiv = new_info.CuryMultDiv;
                    Base.currencyinfo.Update(b_info);
                }

                //----------追加付款方式----------------------
                //新增一筆KGBillPayment 注意要放在APTran之前  因為實付金額
                APInvoiceEntry_Extension apInvoiceEntryExt = Base.GetExtension<APInvoiceEntry_Extension>();
                //依照分包契約的設定, 也產生付款方式的資料
                KGBillPayment billPayment = (KGBillPayment)apInvoiceEntryExt.KGBillPaym.Cache.CreateInstance();

                APRegisterExt apRegExt = PXCache<APRegister>.GetExtension<APRegisterExt>(Base.Document.Current);
                foreach (KGPoOrderPayment segPricing in PXSelect <KGPoOrderPayment,
                  Where <KGPoOrderPayment.orderNbr, Equal<Required<POOrder.orderNbr>>>>.Select(Base, apRegExt.UsrPONbr)) {
                    billPayment.PricingType = PricingType.A;
                    //CheckTitle
                    BAccount bAccount = GetBAccount(invoice.VendorID);
                    if (bAccount != null)
                    {
                        billPayment.CheckTitle = bAccount.AcctName;
                    }
                    //支票
                    billPayment.PaymentMethod = Kedge.DAC.PaymentMethod.A;
                    billPayment.PaymentPeriod = segPricing.PaymentPeriod;
                    billPayment.PaymentPct = segPricing.PaymentPct;
                    apInvoiceEntryExt.KGBillPaym.Cache.Insert(billPayment);
                }
                if (apInvoiceEntryExt.KGBillPaym.Select().Count == 0) {
                    billPayment.PricingType = PricingType.A;
                    //CheckTitle
                    BAccount bAccount = GetBAccount(invoice.VendorID);
                    if (bAccount != null)
                    {
                        billPayment.CheckTitle = bAccount.AcctName;
                    }
                    //支票
                    billPayment.PaymentMethod = Kedge.DAC.PaymentMethod.A;
                    billPayment.PaymentPeriod = 0;
                    billPayment.PaymentPct = 100;
                    apInvoiceEntryExt.KGBillPaym.Cache.Insert(billPayment);
                }

                //一般計價
                
                //----------追加付款方式----------------------

                bool isRetainageByLines = doc.LineNbr != 0;
                bool isFinalRetainageDoc = !isRetainageByLines && doc.CuryRetainageUnreleasedCalcAmt == 0m;
                //Dictionary<APTranKey, APTranValue> retainageDetails = new Dictionary<APTranKey, APTranValue>();
                var retainageDetails = new Dictionary<(string TranType, string RefNbr, int? LineNbr), APTranRetainageData>();
                //Add By Jerry
                foreach (var group2 in list.GroupBy(row => new { row.DocType, row.RefNbr }))
                {
                    APInvoiceExt doc2 = group2.First();
                    PXResult<APInvoice, CurrencyInfo, Terms, Vendor> resultDoc2=
                   APInvoice_CurrencyInfo_Terms_Vendor
                       .SelectSingleBound(Base, null, doc2.DocType, doc2.RefNbr, doc2.VendorID).AsEnumerable()
                       .Cast<PXResult<APInvoice, CurrencyInfo, Terms, Vendor>>()
                       .First();

                
                    APInvoice origInvoiceExt = resultDoc2;

                    foreach (APInvoiceExt docLine in group2)
                    {
                        PXProcessing<APInvoiceExt>.SetCurrentItem(docLine);

                        PXResultset<APTran> details = isRetainageByLines
                            ? PXSelect<APTran,
                                Where<APTran.tranType, Equal<Required<APTran.tranType>>,
                                    And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                                    And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>
                                .SelectSingleBound(Base, null, docLine.DocType, docLine.RefNbr, docLine.LineNbr)
                            : PXSelectGroupBy<APTran,
                                Where<APTran.tranType, Equal<Required<APTran.tranType>>,
                                    And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                                    And<APTran.curyRetainageAmt, NotEqual<decimal0>>>>,
                                Aggregate<
                                    GroupBy<APTran.taxCategoryID,
                    Sum<APTran.curyRetainageAmt>>>,
                OrderBy<Asc<APTran.taxCategoryID>>>
                                .Select(Base, docLine.DocType, docLine.RefNbr);

                        TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, TaxCalc.ManualCalc);

                        foreach (APTran detail in details)
                        {
                            // Create APTran record for chosen retainage amount, 
                            // clear all required fields to prevent tax calculation,
                            // discount calculation and retainage calculation.
                            // CuryUnitCost = 0m and CuryLineAmt = 0m here to prevent their 
                            // FieldDefaulting events, because in our case default value 
                            // should be equal to zero.
                            //
                            APTran tranNew =new APTran { 
                            
                                CuryUnitCost = 0m,
                                CuryLineAmt = 0m
                            };

                            tranNew.BranchID = origInvoiceExt.BranchID;
                            tranNew.TaxCategoryID = detail.TaxCategoryID;
                            tranNew.AccountID = origInvoiceExt.RetainageAcctID;
                            tranNew.SubID = origInvoiceExt.RetainageSubID;
                            tranNew.ProjectID = ProjectDefaultAttribute.NonProject();

                            tranNew.Qty = 0m;
                            tranNew.CuryUnitCost = 0m;
                            tranNew.ManualDisc = true;
                            tranNew.DiscPct = 0m;
                            tranNew.CuryDiscAmt = 0m;
                            tranNew.RetainagePct = 0m;
                            tranNew.CuryRetainageAmt = 0m;
                            tranNew.CuryTaxableAmt = 0m;
                            tranNew.CuryTaxAmt = 0;
                            tranNew.CuryExpenseAmt = 0m;
                            tranNew.GroupDiscountRate = 1m;
                            tranNew.DocumentDiscountRate = 1m;

                            tranNew.OrigLineNbr = docLine.LineNbr;
                            
                            using (new PXLocaleScope(vendor.LocaleName))
                            {
                                tranNew.TranDesc = PXMessages.LocalizeFormatNoPrefix(
                                    Messages.RetainageForTransactionDescription,
                                    APDocTypeDict[origInvoiceExt.DocType],
                                    origInvoiceExt.RefNbr);
                            }

                            decimal curyLineAmt = 0m;
                            bool isFinalRetainageDetail = docLine.CuryRetainageUnreleasedCalcAmt == 0m;

                            if (isFinalRetainageDetail)
                            {
                                PXResultset<APTran> detailsRetainage = isRetainageByLines
                                    ? PXSelectJoin<APTran,
                                        InnerJoin<APRegister, On<APRegister.docType, Equal<APTran.tranType>,
                                            And<APRegister.refNbr, Equal<APTran.refNbr>>>>,
                                        Where<APRegister.isRetainageDocument, Equal<True>,
                                            And<APRegister.released, Equal<True>,
                                            And<APRegister.origDocType, Equal<Required<APRegister.origDocType>>,
                                            And<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>,
                                            And<APTran.origLineNbr, Equal<Required<APTran.origLineNbr>>>>>>>>
                                        .Select(Base, docLine.DocType, docLine.RefNbr, docLine.LineNbr)
                                    : PXSelectJoin<APTran,
                                        InnerJoin<APRegister, On<APRegister.docType, Equal<APTran.tranType>,
                                            And<APRegister.refNbr, Equal<APTran.refNbr>>>>,
                                        Where<APRegister.isRetainageDocument, Equal<True>,
                                            And<APRegister.released, Equal<True>,
                                            And<APRegister.origDocType, Equal<Required<APRegister.origDocType>>,
                                            And<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>,
                                            And<Where<APTran.taxCategoryID, Equal<Required<APTran.taxCategoryID>>,
                                                Or<Required<APTran.taxCategoryID>, IsNull>>>>>>>>
                                        .Select(Base, docLine.DocType, docLine.RefNbr, detail.TaxCategoryID, detail.TaxCategoryID);

                                decimal detailsRetainageSum = 0m;
                                foreach (PXResult<APTran, APRegister> res in detailsRetainage)
                                {
                                    APTran detailRetainage = res;
                                    APRegister docRetainage = res;
                                    detailsRetainageSum += (detailRetainage.CuryTranAmt ?? 0m) * (docRetainage.SignAmount ?? 0m);
                                }

                                curyLineAmt = (detail.CuryRetainageAmt ?? 0m) - detailsRetainageSum;
                            }
                            else
                            {
                                decimal retainagePercent = (decimal)(docLine.CuryRetainageReleasedAmt /
                                    (isRetainageByLines ? detail.CuryOrigRetainageAmt : doc2.CuryRetainageTotal));
                                curyLineAmt = PXCurrencyAttribute.RoundCury(Base.Transactions.Cache, tranNew, (detail.CuryRetainageAmt ?? 0m) * retainagePercent);
                            }

                            tranNew.CuryLineAmt = curyLineAmt;
                            //------------Detail 調整By Jerry Begin---------------------
                            KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
                            PMCostBudget pmCostBudget = null;
                            if (setUp != null)
                            {
                                pmCostBudget = getPMCostBudget(invoice.ProjectID, setUp.KGRetainageReturnInventoryID);
                            }
                            else
                            {
                                pmCostBudget = null;
                            }
                            APInvoice master = Base.Document.Current;
                            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
                            tranNew.PONbr = apRegisterExt.UsrPONbr;
                            tranNew.POOrderType = apRegisterExt.UsrPOOrderType;
                            //tranNew.AccountID = pmCostBudget.AccountGroupID;
                            //tranNew.ProjectID = ProjectDefaultAttribute.NonProject();
                            tranNew.ProjectID = master.ProjectID;

                            APTranExt apTranExt = PXCache<APTran>.GetExtension<APTranExt>(tranNew);
                            apTranExt.UsrOrigDocType = origInvoiceExt.DocType;
                            apTranExt.UsrOrigRefNbr = origInvoiceExt.RefNbr;
                            tranNew = Base.Transactions.Update(tranNew);
                            //mark by louis 20210718
                            //Base.Transactions.Cache.SetValue<APTranExt.usrValuationType>(tranNew, "R");
                            Base.Transactions.Cache.SetValue<APTranExt.usrValuationType>(tranNew, "B");



                            if (isRetainageByLines)
                            {
                                //retainageDetails.Add(new APTranKey(tranNew),
                                    //new APTranValue(tranNew, docLine.CuryRetainageReleasedAmt - tranNew.CuryLineAmt, isFinalRetainageDetail));
                                retainageDetails.Add(
                                    (tranNew.TranType, tranNew.RefNbr, tranNew.LineNbr),
                                    new APTranRetainageData()
                                    {
                                        Detail = tranNew,
                                        RemainAmt = docLine.CuryRetainageReleasedAmt - tranNew.CuryLineAmt,
                                        IsFinal = isFinalRetainageDetail
                                    });
                            }
                            else if (tranMax == null || System.Math.Abs(tranMax.CuryLineAmt ?? 0m) < System.Math.Abs(tranNew.CuryLineAmt ?? 0m))
                            {
                                tranMax = tranNew;
                            }

                            if (pmCostBudget != null)
                            {
                                Base.Transactions.Cache.SetValueExt<APTran.inventoryID>(tranNew, pmCostBudget.InventoryID);
                                //Base.Transactions.Cache.SetValueExt<APTran.projectID>(tranNew, master.ProjectID);
                                Base.Transactions.Cache.SetValueExt<APTran.taskID>(tranNew, pmCostBudget.TaskID);
                                Base.Transactions.Cache.SetValueExt<APTran.costCodeID>(tranNew, pmCostBudget.CostCodeID);
                                //Base.Transactions.Cache.SetValueExt<APTran.projectID>(tranNew, master.ProjectID);
                            }



                            //------------Detail 調整By Jerry End---------------------
                        }

                        PXProcessing<APInvoiceExt>.SetProcessed();
                    }
                }

                ClearCurrentDocumentDiscountDetails();

                // We should copy all taxes from the original document
                // because it is possible to add or delete them.
                // 

                //-----------------稅處理Begin-------------------------------
                var taxes = PXSelectJoin<APTaxTran,
                LeftJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>,
                Where<APTaxTran.module, Equal<BatchModule.moduleAP>,
                    And<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>,
                    And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>,
                    And<APTaxTran.curyRetainedTaxAmt, NotEqual<decimal0>>>>>>
                .Select(Base, group.Key.DocType, group.Key.RefNbr);

                // Insert taxes first and only after that copy 
                // all needed values to prevent tax recalculation
                // during the next tax insertion.
                // 
                Dictionary<string, APTaxTran> insertedTaxes = null;
                insertedTaxes = new Dictionary<string, APTaxTran>();
                taxes.RowCast<APTaxTran>().ForEach(tax => insertedTaxes.Add(tax.TaxID, Base.Taxes.Insert(new APTaxTran() { TaxID = tax.TaxID })));
                
                decimal? taxAmt = getTaxAmt(list, retainageDetails);
                foreach (PXResult<APTaxTran, Tax> res in taxes)
                {
                    APTaxTran aptaxtran = res;
                    Tax tax = res;

                    APTaxTran new_aptaxtran = insertedTaxes[aptaxtran.TaxID];
                    if (new_aptaxtran == null) continue;

                    APReleaseProcess.AdjustTaxCalculationLevelForNetGrossEntryMode(invoice, null, ref tax);
                    decimal curyTaxAmt = 0m;

                    if (isRetainageByLines)
                    {
                        foreach (APTax aptax in Base.Tax_Rows.Select()
                            .RowCast<APTax>()
                            .Where(row => row.TaxID == aptaxtran.TaxID))
                        {
                            //APTranKey key = new APTranKey(aptax);
                            //APTranValue retainageDetail = retainageDetails[key];
                            APTranRetainageData retainageDetail = retainageDetails[(aptax.TranType, aptax.RefNbr, aptax.LineNbr)];
                            decimal detailCuryTaxAmt = 0m;

                            PXResult<APTax, Tax> origAPTaxRes = PXSelectJoin<APTax,
                                InnerJoin<Tax, On<Tax.taxID, Equal<APTax.taxID>>>,
                                Where<APTax.tranType, Equal<Required<APTax.tranType>>,
                                    And<APTax.refNbr, Equal<Required<APTax.refNbr>>,
                                    And<APTax.lineNbr, Equal<Required<APTax.lineNbr>>,
                                    And<APTax.taxID, Equal<Required<APTax.taxID>>>>>>>
                                .SelectSingleBound(Base, null, group.Key.DocType, group.Key.RefNbr, retainageDetail.Detail.OrigLineNbr, aptax.TaxID)
                                .Cast<PXResult<APTax, Tax>>()
                                .First();

                            APTax origAPTax = origAPTaxRes;
                            Tax origTax = origAPTaxRes;

                            if (retainageDetail.IsFinal)
                            {
                                PXResultset<APTax> taxDetailsRetainage = PXSelectJoin<APTax,
                                    InnerJoin<APRegister, On<APRegister.docType, Equal<APTax.tranType>,
                                        And<APRegister.refNbr, Equal<APTax.refNbr>>>,
                                    InnerJoin<APTran, On<APTran.tranType, Equal<APTax.tranType>,
                                        And<APTran.refNbr, Equal<APTax.refNbr>,
                                        And<APTran.lineNbr, Equal<APTax.lineNbr>>>>>>,
                                    Where<APRegister.isRetainageDocument, Equal<True>,
                                        And<APRegister.released, Equal<True>,
                                        And<APRegister.origDocType, Equal<Required<APRegister.origDocType>>,
                                        And<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>,
                                        And<APTran.origLineNbr, Equal<Required<APTran.origLineNbr>>,
                                        And<APTax.taxID, Equal<Required<APTax.taxID>>>>>>>>>
                                    .Select(Base, origDocType, OrigRefNbr, retainageDetail.Detail.OrigLineNbr, aptax.TaxID);

                                decimal taxDetailsRetainageSum = 0m;
                                foreach (PXResult<APTax, APRegister> resTaxDetailsRetainage in taxDetailsRetainage)
                                {
                                    APTax taxDetailRetainage = resTaxDetailsRetainage;
                                    APRegister docRetainage = resTaxDetailsRetainage;
                                    taxDetailsRetainageSum += ((taxDetailRetainage.CuryTaxAmt ?? 0m) + (taxDetailRetainage.CuryExpenseAmt ?? 0m)) * (docRetainage.SignAmount ?? 0m);
                                }

                                detailCuryTaxAmt = (origAPTax.CuryRetainedTaxAmt ?? 0m) - taxDetailsRetainageSum;
                            }
                            else
                            {
                                decimal retainedPercent = (decimal)origAPTax.CuryRetainedTaxAmt / (decimal)origAPTax.CuryRetainedTaxableAmt;
                                detailCuryTaxAmt = PXCurrencyAttribute.RoundCury(Base.Tax_Rows.Cache, aptax, (decimal)aptax.CuryTaxableAmt * retainedPercent);
                            }

                            curyTaxAmt += detailCuryTaxAmt;

                            APTax new_aptax = PXCache<APTax>.CreateCopy(aptax);
                            decimal detailDeductiblePercent = 100m - (new_aptax.NonDeductibleTaxRate ?? 100m);
                            new_aptax.CuryExpenseAmt = PXCurrencyAttribute.RoundCury(Base.Tax_Rows.Cache, new_aptax, detailCuryTaxAmt * detailDeductiblePercent / 100m);
                            new_aptax.CuryTaxAmt = detailCuryTaxAmt - new_aptax.CuryExpenseAmt;
                            new_aptax = Base.Tax_Rows.Update(new_aptax);

                            //if (origTax.IsIncludedInBalance())
                            if (APReleaseProcess.IncludeTaxInLineBalance(origTax))
                            {
                                retainageDetail.RemainAmt -= detailCuryTaxAmt;
                            }
                        }
                    }
                    else
                    {
                        if (isFinalRetainageDoc)
                        {
                            PXResultset<APTaxTran> taxDetailsRetainage = PXSelectJoin<APTaxTran,
                                InnerJoin<APRegister, On<APRegister.docType, Equal<APTaxTran.tranType>,
                                    And<APRegister.refNbr, Equal<APTaxTran.refNbr>>>>,
                                Where<APRegister.isRetainageDocument, Equal<True>,
                                    And<APRegister.released, Equal<True>,
                                    And<APRegister.origDocType, Equal<Required<APRegister.origDocType>>,
                                    And<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>,
                                    And<APTaxTran.taxID, Equal<Required<APTaxTran.taxID>>>>>>>>
                                .Select(Base, aptaxtran.TranType, aptaxtran.RefNbr, aptaxtran.TaxID);

                            decimal taxDetailsRetainageSum = 0m;
                            foreach (PXResult<APTaxTran, APRegister> resTaxDetailsRetainage in taxDetailsRetainage)
                            {
                                APTaxTran taxDetailRetainage = resTaxDetailsRetainage;
                                APRegister docRetainage = resTaxDetailsRetainage;
                                taxDetailsRetainageSum += ((taxDetailRetainage.CuryTaxAmt ?? 0m) + (taxDetailRetainage.CuryExpenseAmt ?? 0m)) * (docRetainage.SignAmount ?? 0m);
                            }

                            curyTaxAmt = (aptaxtran.CuryRetainedTaxAmt ?? 0m) - taxDetailsRetainageSum;
                        }
                        else
                        {
                            APTax retainedTaxableSum = PXSelectGroupBy<APTax,
                                Where<APTax.tranType, Equal<Required<APTax.tranType>>,
                                    And<APTax.refNbr, Equal<Required<APTax.refNbr>>,
                                    And<APTax.taxID, Equal<Required<APTax.taxID>>>>>,
                                Aggregate<
                                    GroupBy<APTax.tranType,
                                    GroupBy<APTax.refNbr,
                                    GroupBy<APTax.taxID,
                                    Sum<APTax.curyRetainedTaxableAmt>>>>>>
                                .SelectSingleBound(Base, null, aptaxtran.TranType, aptaxtran.RefNbr, aptaxtran.TaxID);

                            decimal retainedPercent = (decimal)aptaxtran.CuryRetainedTaxAmt / (decimal)retainedTaxableSum.CuryRetainedTaxableAmt;
                            curyTaxAmt = PXCurrencyAttribute.RoundCury(Base.Taxes.Cache, new_aptaxtran, (decimal)new_aptaxtran.CuryTaxableAmt * retainedPercent);
                        }
                    }

                    new_aptaxtran = PXCache<APTaxTran>.CreateCopy(new_aptaxtran);

                    // We should adjust APTax taxable amount for inclusive tax, 
                    // because it used during the release process to post correct 
                    // amount on Expense account for each APTran record. 
                    // See APReleaseProcess.GetExpensePostingAmount method for details.
                    // 
                    decimal taxDiff = (new_aptaxtran.CuryTaxAmt ?? 0m) + (new_aptaxtran.CuryExpenseAmt ?? 0m) - curyTaxAmt;
                    if (tax?.IsRegularInclusiveTax() == true && taxDiff != 0m)
                    {
                        new_aptaxtran.CuryTaxableAmt += taxDiff;

                        foreach (APTax roundAPTax in Base.Tax_Rows.Select()
                            .AsEnumerable().RowCast<APTax>()
                            .Where(row => row.TaxID == new_aptaxtran.TaxID))
                        {
                            APTax roundTaxDetail = PXCache<APTax>.CreateCopy(roundAPTax);
                            roundTaxDetail.CuryTaxableAmt += taxDiff;
                            roundTaxDetail = Base.Tax_Rows.Update(roundTaxDetail);

                            foreach (APTax lineAPTax in Base.Tax_Rows.Select()
                                .AsEnumerable().RowCast<APTax>()
                                .Where(row => row.TaxID != roundAPTax.TaxID && row.LineNbr == roundAPTax.LineNbr))
                            {
                                APTaxTran lineAPTaxTran = insertedTaxes[lineAPTax.TaxID];
                                lineAPTaxTran.CuryTaxableAmt += taxDiff;
                                lineAPTaxTran = Base.Taxes.Update(lineAPTaxTran);

                                APTax lineTaxDetail = PXCache<APTax>.CreateCopy(lineAPTax);
                                lineTaxDetail.CuryTaxableAmt += taxDiff;
                                lineTaxDetail = Base.Tax_Rows.Update(lineTaxDetail);
                            }
                        }
                    }

                    new_aptaxtran.TaxRate = aptaxtran.TaxRate;
                    decimal deductiblePercent = 100m - (new_aptaxtran.NonDeductibleTaxRate ?? 100m);
                    if (curyTaxAmt == null)
                    {
                        new_aptaxtran.CuryExpenseAmt = 0;
                    }
                    else {
                        new_aptaxtran.CuryExpenseAmt = PXCurrencyAttribute.RoundCury(Base.Taxes.Cache, new_aptaxtran, curyTaxAmt * deductiblePercent / 100m);
                        //new_aptaxtran.CuryTaxAmt = taxAmt;
                    }
                    //new_aptaxtran.CuryTaxAmt = new_aptaxtran.CuryTaxAmt;
                    //new_aptaxtran.CuryTaxAmt = curyTaxAmt - new_aptaxtran.CuryExpenseAmt;
                    //根據研究其實new_aptaxtran.CuryTaxAmt會自動算沒必要多此一舉反而可能錯
                    //new_aptaxtran.CuryTaxAmt = taxAmt;
                    new_aptaxtran = Base.Taxes.Update(new_aptaxtran);
                }
                //-----------------稅處理End-------------------------------


                //有疑慮影響APTran金額可能導致金額莫名歸0
                /*
                if (isRetainageByLines)
                {
                    retainageDetails.Values
                        .Where(value => value.RemainAmt != 0m)
                        .ForEach(value => ProcessRoundingDiff(value.RemainAmt ?? 0m, value.Detail));
                }
                else if (tranMax != null)
                {
                    decimal diff = curyRetainageSum - (invoice.CuryDocBal ?? 0m);
                    if (diff != 0m)
                    {
                        ProcessRoundingDiff(diff, tranMax);
                    }
                }*/
                
                if (invoice.CuryTaxAmt != invoice.CuryTaxTotal)
                {
                    invoice.CuryTaxAmt = invoice.CuryTaxTotal;
                    invoice = Base.Document.Update(invoice);
                }

                TaxBaseAttribute.SetTaxCalc<APTran.taxCategoryID>(Base.Transactions.Cache, null, oldTaxCalc);
                //--------------------------------
                //invoice.OrigDocType = "";
                //invoice.OrigRefNbr = "";
                //--------------------------------

                Base.Save.Press();
                //Add By Jerry
                //foreach (var group2 in list.GroupBy(row => new { row.DocType, row.RefNbr }))
                //{
                    if (isAutoRelease && invoice.Hold != true)
                    {
                        using (new PXTimeStampScope(null))
                        {
                            APDocumentRelease.ReleaseDoc(new List<APRegister> { invoice }, false);
                        }
                    }
                //}
            }
            catch (PXException exc)
            {
                PXProcessing<APInvoiceExt>.SetError(exc);
                failed = true;
            }
        
   

            if (failed)
            {
                throw new PXOperationCompletedWithErrorException(GL.Messages.DocumentsNotReleased);
            }
        }
        private BAccount GetBAccount(int? VendorID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, VendorID);
        }
        public PXResultset<PMCostBudget> getPMCostBudget(int? projectID, int? inventoryID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<PMCostBudget> set = PXSelect<PMCostBudget, Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>>>>.Select(graph, projectID, inventoryID);
            return set;
        }
        public decimal? getTaxAmt(List<APInvoiceExt> list, Dictionary<(string TranType, string RefNbr, int? LineNbr), APTranRetainageData> retainageDetails) {

            decimal? totalCuryTaxAmt = 0;
            foreach (var group2 in list.GroupBy(row => new { row.DocType, row.RefNbr }))
            {
                APInvoiceExt doc2 = group2.First();
                bool isRetainageByLines = doc2.LineNbr != 0;
                bool isFinalRetainageDoc = !isRetainageByLines && doc2.CuryRetainageUnreleasedCalcAmt == 0m;
                PXResult<APInvoice, CurrencyInfo, Terms, Vendor> resultDoc2 =
               APInvoice_CurrencyInfo_Terms_Vendor
                   .SelectSingleBound(Base, null, doc2.DocType, doc2.RefNbr, doc2.VendorID).AsEnumerable()
                   .Cast<PXResult<APInvoice, CurrencyInfo, Terms, Vendor>>()
                   .First();


                APInvoice origInvoiceExt = resultDoc2;

                var taxes = PXSelectJoin<APTaxTran,
                LeftJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>,
                Where<APTaxTran.module, Equal<BatchModule.moduleAP>,
                    And<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>,
                    And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>,
                    And<APTaxTran.curyRetainedTaxAmt, NotEqual<decimal0>>>>>>
                .Select(Base, group2.Key.DocType, group2.Key.RefNbr);

                // Insert taxes first and only after that copy 
                // all needed values to prevent tax recalculation
                // during the next tax insertion.
                // 
                Dictionary<string, APTaxTran> insertedTaxes = null;
                insertedTaxes = new Dictionary<string, APTaxTran>();
                taxes.RowCast<APTaxTran>().ForEach(tax => insertedTaxes.Add(tax.TaxID, new APTaxTran() { TaxID = tax.TaxID }));

                foreach (PXResult<APTaxTran, Tax> res in taxes)
                {
                    APTaxTran aptaxtran = res;
                    Tax tax = res;

                    APTaxTran new_aptaxtran = insertedTaxes[aptaxtran.TaxID];
                    if (new_aptaxtran == null) continue;

                    decimal curyTaxAmt = 0m;

                    if (isRetainageByLines)
                    {
                        foreach (APTax aptax in Base.Tax_Rows.Select()
                            .RowCast<APTax>()
                            .Where(row => row.TaxID == aptaxtran.TaxID))
                        {
                            //APTranKey key = new APTranKey(aptax);
                            //APTranValue retainageDetail = retainageDetails[key];
                            APTranRetainageData retainageDetail = retainageDetails[(aptax.TranType, aptax.RefNbr, aptax.LineNbr)];
                           
                            decimal detailCuryTaxAmt = 0m;

                            PXResult<APTax, Tax> origAPTaxRes = PXSelectJoin<APTax,
                                InnerJoin<Tax, On<Tax.taxID, Equal<APTax.taxID>>>,
                                Where<APTax.tranType, Equal<Required<APTax.tranType>>,
                                    And<APTax.refNbr, Equal<Required<APTax.refNbr>>,
                                    And<APTax.lineNbr, Equal<Required<APTax.lineNbr>>,
                                    And<APTax.taxID, Equal<Required<APTax.taxID>>>>>>>
                                .SelectSingleBound(Base, null, group2.Key.DocType, group2.Key.RefNbr, retainageDetail.Detail.OrigLineNbr, aptax.TaxID)
                                .Cast<PXResult<APTax, Tax>>()
                                .First();

                            APTax origAPTax = origAPTaxRes;
                            Tax origTax = origAPTaxRes;

                            if (retainageDetail.IsFinal)
                            {
                                PXResultset<APTax> taxDetailsRetainage = PXSelectJoin<APTax,
                                    InnerJoin<APRegister, On<APRegister.docType, Equal<APTax.tranType>,
                                        And<APRegister.refNbr, Equal<APTax.refNbr>>>,
                                    InnerJoin<APTran, On<APTran.tranType, Equal<APTax.tranType>,
                                        And<APTran.refNbr, Equal<APTax.refNbr>,
                                        And<APTran.lineNbr, Equal<APTax.lineNbr>>>>>>,
                                    Where<APRegister.isRetainageDocument, Equal<True>,
                                        And<APRegister.released, Equal<True>,
                                        And<APRegister.origDocType, Equal<Required<APRegister.origDocType>>,
                                        And<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>,
                                        And<APTran.origLineNbr, Equal<Required<APTran.origLineNbr>>,
                                        And<APTax.taxID, Equal<Required<APTax.taxID>>>>>>>>>
                                    .Select(Base, origInvoiceExt.DocType, origInvoiceExt.RefNbr, retainageDetail.Detail.OrigLineNbr, aptax.TaxID);

                                decimal taxDetailsRetainageSum = 0m;
                                foreach (PXResult<APTax, APRegister> resTaxDetailsRetainage in taxDetailsRetainage)
                                {
                                    APTax taxDetailRetainage = resTaxDetailsRetainage;
                                    APRegister docRetainage = resTaxDetailsRetainage;
                                    taxDetailsRetainageSum += ((taxDetailRetainage.CuryTaxAmt ?? 0m) + (taxDetailRetainage.CuryExpenseAmt ?? 0m)) * (docRetainage.SignAmount ?? 0m);
                                }

                                detailCuryTaxAmt = (origAPTax.CuryRetainedTaxAmt ?? 0m) - taxDetailsRetainageSum;
                            }
                            else
                            {
                                decimal retainedPercent = (decimal)origAPTax.CuryRetainedTaxAmt / (decimal)origAPTax.CuryRetainedTaxableAmt;
                                detailCuryTaxAmt = PXCurrencyAttribute.RoundCury(Base.Tax_Rows.Cache, aptax, (decimal)aptax.CuryTaxableAmt * retainedPercent);
                            }

                            curyTaxAmt += detailCuryTaxAmt;

                            APTax new_aptax = PXCache<APTax>.CreateCopy(aptax);
                            decimal detailDeductiblePercent = 100m - (new_aptax.NonDeductibleTaxRate ?? 100m);
                            new_aptax.CuryExpenseAmt = PXCurrencyAttribute.RoundCury(Base.Tax_Rows.Cache, new_aptax, detailCuryTaxAmt * detailDeductiblePercent / 100m);
                            new_aptax.CuryTaxAmt = detailCuryTaxAmt - new_aptax.CuryExpenseAmt;
                            //if (origTax.IsIncludedInBalance())
                            if(APReleaseProcess.IncludeTaxInLineBalance(origTax))
                            {
                                retainageDetail.RemainAmt -= detailCuryTaxAmt;
                            }
                        }
                    }
                    else
                    {
                        if (isFinalRetainageDoc)
                        {
                            PXResultset<APTaxTran> taxDetailsRetainage = PXSelectJoin<APTaxTran,
                                InnerJoin<APRegister, On<APRegister.docType, Equal<APTaxTran.tranType>,
                                    And<APRegister.refNbr, Equal<APTaxTran.refNbr>>>>,
                                Where<APRegister.isRetainageDocument, Equal<True>,
                                    And<APRegister.released, Equal<True>,
                                    And<APRegister.origDocType, Equal<Required<APRegister.origDocType>>,
                                    And<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>,
                                    And<APTaxTran.taxID, Equal<Required<APTaxTran.taxID>>>>>>>>
                                .Select(Base, aptaxtran.TranType, aptaxtran.RefNbr, aptaxtran.TaxID);

                            decimal taxDetailsRetainageSum = 0m;
                            foreach (PXResult<APTaxTran, APRegister> resTaxDetailsRetainage in taxDetailsRetainage)
                            {
                                APTaxTran taxDetailRetainage = resTaxDetailsRetainage;
                                APRegister docRetainage = resTaxDetailsRetainage;
                                taxDetailsRetainageSum += ((taxDetailRetainage.CuryTaxAmt ?? 0m) + (taxDetailRetainage.CuryExpenseAmt ?? 0m)) * (docRetainage.SignAmount ?? 0m);
                            }

                            curyTaxAmt = (aptaxtran.CuryRetainedTaxAmt ?? 0m) - taxDetailsRetainageSum;
                        }
                        else
                        {
                            APTax retainedTaxableSum = PXSelectGroupBy<APTax,
                                Where<APTax.tranType, Equal<Required<APTax.tranType>>,
                                    And<APTax.refNbr, Equal<Required<APTax.refNbr>>,
                                    And<APTax.taxID, Equal<Required<APTax.taxID>>>>>,
                                Aggregate<
                                    GroupBy<APTax.tranType,
                                    GroupBy<APTax.refNbr,
                                    GroupBy<APTax.taxID,
                                    Sum<APTax.curyRetainedTaxableAmt>>>>>>
                                .SelectSingleBound(Base, null, aptaxtran.TranType, aptaxtran.RefNbr, aptaxtran.TaxID);

                            decimal retainedPercent = (decimal)aptaxtran.CuryRetainedTaxAmt / (decimal)retainedTaxableSum.CuryRetainedTaxableAmt;
                            if (new_aptaxtran.CuryTaxableAmt == null)
                            {
                                curyTaxAmt = 0;
                            }
                            else {
                                curyTaxAmt = PXCurrencyAttribute.RoundCury(Base.Taxes.Cache, new_aptaxtran, (decimal)new_aptaxtran.CuryTaxableAmt * retainedPercent);
                            }
                        }
                    }

                    new_aptaxtran = PXCache<APTaxTran>.CreateCopy(new_aptaxtran);

                    // We should adjust APTax taxable amount for inclusive tax, 
                    // because it used during the release process to post correct 
                    // amount on Expense account for each APTran record. 
                    // See APReleaseProcess.GetExpensePostingAmount method for details.
                    // 
                    decimal taxDiff = (new_aptaxtran.CuryTaxAmt ?? 0m) + (new_aptaxtran.CuryExpenseAmt ?? 0m) - curyTaxAmt;
                    if (tax?.IsRegularInclusiveTax() == true && taxDiff != 0m)
                    {
                        new_aptaxtran.CuryTaxableAmt += taxDiff;

                        foreach (APTax roundAPTax in Base.Tax_Rows.Select()
                            .AsEnumerable().RowCast<APTax>()
                            .Where(row => row.TaxID == new_aptaxtran.TaxID))
                        {
                            APTax roundTaxDetail = PXCache<APTax>.CreateCopy(roundAPTax);
                            roundTaxDetail.CuryTaxableAmt += taxDiff;


                            foreach (APTax lineAPTax in Base.Tax_Rows.Select()
                                .AsEnumerable().RowCast<APTax>()
                                .Where(row => row.TaxID != roundAPTax.TaxID && row.LineNbr == roundAPTax.LineNbr))
                            {
                                APTaxTran lineAPTaxTran = insertedTaxes[lineAPTax.TaxID];
                                lineAPTaxTran.CuryTaxableAmt += taxDiff;

                                APTax lineTaxDetail = PXCache<APTax>.CreateCopy(lineAPTax);
                                lineTaxDetail.CuryTaxableAmt += taxDiff;
                            }
                        }
                    }

                    new_aptaxtran.TaxRate = aptaxtran.TaxRate;
                    decimal deductiblePercent = 100m - (new_aptaxtran.NonDeductibleTaxRate ?? 100m);
                    new_aptaxtran.CuryExpenseAmt = PXCurrencyAttribute.RoundCury(Base.Taxes.Cache, new_aptaxtran, curyTaxAmt * deductiblePercent / 100m);
                    totalCuryTaxAmt = totalCuryTaxAmt+(curyTaxAmt - new_aptaxtran.CuryExpenseAmt);

                }
            }
            return totalCuryTaxAmt;
        }
        //add by louis 20200926
        public class APTranRetainageData
        {
            public APTran Detail;
            public decimal? RemainAmt;
            public bool IsFinal;
        }

        private void ProcessRoundingDiff(decimal diff, APTran tran)
        {
            tran.CuryLineAmt += diff;
            tran = Base.Transactions.Update(tran);

            foreach (var group in Base.Tax_Rows.Select()
                .AsEnumerable().RowCast<APTax>()
                .Where(row => row.LineNbr == tran.LineNbr)
                .GroupBy(row => new { row.TranType, row.RefNbr, row.TaxID }))
            {
                foreach (APTax taxDetail in group)
                {
                    APTax newTaxDetail = PXCache<APTax>.CreateCopy(taxDetail);
                    newTaxDetail.CuryTaxableAmt += diff;
                    newTaxDetail = Base.Tax_Rows.Update(newTaxDetail);
                }

                APTaxTran taxSum = PXSelect<APTaxTran,
                    Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>,
                        And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>,
                        And<APTaxTran.taxID, Equal<Required<APTaxTran.taxID>>>>>>
                    .SelectSingleBound(Base, null, group.Key.TranType, group.Key.RefNbr, group.Key.TaxID);
                if (taxSum != null)
                {
                    APTaxTran newTaxSum = PXCache<APTaxTran>.CreateCopy(taxSum);
                    newTaxSum.CuryTaxableAmt += diff;
                    newTaxSum = Base.Taxes.Update(newTaxSum);
                }
            }
        }
        /*
        public PXAction<APInvoice> ViewRetainageDocument;

        [PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        protected virtual IEnumerable viewRetainageDocument(PXAdapter adapter)
        {
            RedirectionToOrigDoc.TryRedirect(RetainageDocuments.Current.DocType, RetainageDocuments.Current.RefNbr, RetainageDocuments.Current.OrigModule);
            return adapter.Get();
        }*/




    }
}