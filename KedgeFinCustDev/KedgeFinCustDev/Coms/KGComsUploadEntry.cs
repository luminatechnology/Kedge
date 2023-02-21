using System;
using System.Collections;
using System.Collections.Generic;
using Kedge.Coms.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CT;
using PX.Objects.GL;
using RCGV.GV.DAC;
using System.Text;
using PX.Objects.PM;
using PX.Objects.CR;
using Kedge.DAC;
using NM.Util;
using NM.DAC;
using static RCGV.GV.Util.GVList;
using GVGuiType = RCGV.GV.Util.GVList.GVGuiType;
using PX.Common;


// 20211220 by louis heckInvoice() : 修改統編長度檢查不得大於8碼
//                   DoCreateAPInvoice() : 如果VoucherItem.Nametw=null, 寫入'COMS無資料'到KgBillPayment.Checkitle
// 20220103 by louis DOCreateInvoice() if item.Cd == "D" && item.DateDue != null && DocType='INV' 也寫入文件明細
//

namespace Kedge.Coms
{
    public class KGComsUploadEntry : PXGraph<KGComsUploadEntry>, PXImportAttribute.IPXPrepareItems
    {

        #region 
        private const string IMPORT_GRAPH_KEY = "KGComsImportErrorGraph";
        #endregion
        #region View
        //public PXFilter<KGComsFilter> Filter;
        public PXSelect<KGComsBatch> MasterView;

        [PXImport(typeof(KGComsVoucher))]
        public PXSelect<KGComsVoucher, Where<KGComsVoucher.batchID, Equal<Current<KGComsBatch.batchID>>>> Vouchers;

        [PXImport(typeof(KGComsInvoice))]
        public PXSelect<KGComsInvoice, Where<KGComsInvoice.batchID, Equal<Current<KGComsBatch.batchID>>>> Invoices;

        [PXImport(typeof(KGComsVoucherItem))]
        public PXSelect<KGComsVoucherItem, Where<KGComsVoucherItem.batchID, Equal<Current<KGComsBatch.batchID>>>> VoucherItems;
        #endregion

        #region Action
        public PXSave<KGComsBatch> Save;
        public PXCancel<KGComsBatch> Cancel;
        public PXInsert<KGComsBatch> Insert;
        public PXDelete<KGComsBatch> Delete;

        public PXFirst<KGComsBatch> First;
        [PXUIField(DisplayName = ActionsMessages.First, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXFirstButton]
        protected virtual IEnumerable first(PXAdapter adapter)
        {
            ClearErrorGraph();
            return new PXFirst<KGComsBatch>(this, "First").Press(adapter);
        }

        public PXPrevious<KGComsBatch> Prev;
        [PXUIField(DisplayName = ActionsMessages.Previous, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXPreviousButton]
        protected virtual IEnumerable prev(PXAdapter adapter)
        {
            ClearErrorGraph();
            return new PXPrevious<KGComsBatch>(this, "Prev").Press(adapter);
        }
        public PXNext<KGComsBatch> Next;
        [PXUIField(DisplayName = ActionsMessages.Next, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXNextButton]
        protected virtual IEnumerable next(PXAdapter adapter)
        {
            ClearErrorGraph();
            return new PXNext<KGComsBatch>(this, "Next").Press(adapter);
        }

        public PXLast<KGComsBatch> Last;
        [PXUIField(DisplayName = ActionsMessages.Last, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLastButton]
        protected virtual IEnumerable last(PXAdapter adapter)
        {
            ClearErrorGraph();
            return new PXLast<KGComsBatch>(this, "Last").Press(adapter);
        }

        public PXAction<KGComsBatch> ViewGraph;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "查看畫面", Enabled = false)]
        protected void viewGraph()
        {
            throw new PXRedirectRequiredException(GetErrorGraph(), "AP Invoice")
            {
                Mode = PXBaseRedirectException.WindowMode.NewWindow
            };
        }

        public PXAction<KGComsBatch> Verification;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "重新驗證")]
        protected void verification()
        {
            foreach (KGComsVoucher row in Vouchers.Select())
            {
                CheckVoucher(row);
                Vouchers.Update(row);
            }
            foreach (KGComsInvoice row in Invoices.Select())
            {
                CheckInvoice(row);
                Invoices.Update(row);
            }
            foreach (KGComsVoucherItem row in VoucherItems.Select())
            {
                CheckVoucherItem(row);
                VoucherItems.Update(row);
            }
            MasterView.Cache.SetDefaultExt<KGComsBatch.isError>(MasterView.Current);
            base.Persist();
        }

        public PXAction<KGComsBatch> CreateAPInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "產生計價單")]
        protected IEnumerable createAPInvoice(PXAdapter adapter)
        {
            MasterView.Cache.SetDefaultExt<KGComsBatch.isError>(MasterView.Current);
            if (MasterView.Current.IsError == true) return adapter.Get();
            base.Persist();
            PXLongOperation.StartOperation(this, delegate ()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    DoCreateAPInvoice();
                    ts.Complete();
                    ClearErrorGraph();
                }
            });
            return adapter.Get();
        }
        #endregion

        #region Event

        #region KGComsBatch
        public void _(Events.RowSelected<KGComsBatch> e)
        {
            if (e.Row == null) return;
            CreateAPInvoice.SetEnabled((e.Row.IsError ?? false) != true);
            ViewGraph.SetEnabled(GetErrorGraph() != null);
        }

        public void _(Events.RowPersisted<KGComsBatch> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<KGComsBatch.unBatchID>(e.Row);
        }

        public void _(Events.FieldDefaulting<KGComsBatch, KGComsBatch.isError> e)
        {
            if (e.Row == null) return;
            KGComsVoucher c1 = GetKGComsVoucherByError(e.Row.BatchID);
            KGComsInvoice c2 = GetKGComsInvoiceByError(e.Row.BatchID);
            KGComsVoucherItem c3 = GetKGComsVoucherItemByError(e.Row.BatchID);
            if (c1 == null && c2 == null && c3 == null) e.NewValue = false;
            else e.NewValue = true;
        }

        public void _(Events.FieldDefaulting<KGComsBatch.desc> e)
        {
            if (e.Row == null) return;
            e.NewValue = Accessinfo.BusinessDate?.ToString("yyyyMMdd") + "Upload";
        }
        #endregion

        #region KGComsVoucher
        public void _(Events.RowInserted<KGComsVoucher> e)
        {
            if (e.Row == null) return;
            CheckVoucher(e.Row);
        }
        #endregion

        #region KGComsVoucherItem
        public void _(Events.RowInserted<KGComsVoucherItem> e)
        {
            if (e.Row == null) return;
            CheckVoucherItem(e.Row);
        }
        #endregion

        #region KGComsInvoice
        public void _(Events.RowInserted<KGComsInvoice> e)
        {
            if (e.Row == null) return;
            CheckInvoice(e.Row);
        }
        #endregion

        #endregion

        #region Import
        bool preImport = true;
        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (preImport)
            {
                if (Vouchers.View.Name == viewName)
                {
                    foreach (KGComsVoucher item in Vouchers.Select())
                    {
                        Vouchers.Delete(item);
                    }
                }
                else if (Invoices.View.Name == viewName)
                {
                    foreach (KGComsInvoice item in Invoices.Select())
                    {
                        Invoices.Delete(item);
                    }
                }
                else if (VoucherItems.View.Name == viewName)
                {
                    foreach (KGComsVoucherItem item in VoucherItems.Select())
                    {
                        VoucherItems.Delete(item);
                    }
                }
                preImport = false;
            }
            return true;
        }

        public void PrepareItems(string viewName, IEnumerable items)
        {
        }

        public bool RowImported(string viewName, object row, object oldRow)
        {
            return true;
        }

        public bool RowImporting(string viewName, object row)
        {
            return true;
        }

        #endregion

        #region Static Method
        private static void SetErrorGraph(PXGraph graph)
        {
            PXLongOperation.SetCustomInfo(graph, IMPORT_GRAPH_KEY);
        }

        private static PXGraph GetErrorGraph()
        {
            return PXContext.Session.LongOpCustomInfo[IMPORT_GRAPH_KEY] as PXGraph;
        }

        private static void ClearErrorGraph()
        {
            PXContext.Session.Remove(IMPORT_GRAPH_KEY);
        }
        #endregion

        #region Method

        public void DoCreateAPInvoice()
        {
            Dictionary<String, KGComsVoucher> vouchers = new Dictionary<String, KGComsVoucher>();
            Dictionary<String, List<KGComsVoucherItem>> voucherItems = new Dictionary<String, List<KGComsVoucherItem>>();
            Dictionary<String, List<KGComsInvoice>> invoices = new Dictionary<String, List<KGComsInvoice>>();
            GroupVoucher(ref vouchers);
            GroupVoucherItem(vouchers, ref voucherItems);
            GroupInvoice(vouchers, ref invoices);
            foreach (string key in vouchers.Keys)
            {
                APInvoiceEntry entry = PXGraph.CreateInstance<APInvoiceEntry>();
                try
                {
                    KGComsVoucher voucher = vouchers[key];
                    VendorR vendor = GetVendorR(voucher.UniformNo);
                    //Branch branch = GetBranch(voucher.CompanyName);
                    Contract contract = GetContract(voucher.Project);
                    int? nonProject = ProjectDefaultAttribute.NonProject();

                    APInvoiceEntry_Extension entryExt = entry.GetExtension<APInvoiceEntry_Extension>();
                    PXCache docCache = entry.Document.Cache;
                    PXCache tranCache = entry.Transactions.Cache;
                    PXCache paymentCache = entryExt.KGBillPaym.Cache;
                    PXCache guiCache = entryExt.GVApGuiInvoiceRefs.Cache;

                    #region APInvoice
                    APInvoice doc = (APInvoice)docCache.CreateInstance();
                    //當VoucherItem.cd =D 的資料加總大於等於0為INV否則為ADR
                    doc.DocType = voucher.SumAmt >= 0 ? APDocType.Invoice : APDocType.DebitAdj;
                    doc = entry.Document.Insert(doc);
                    DateTime? voucherDate = GetDate(voucher.VoucherDate);
                    docCache.SetValueExt<APInvoice.docDate>(doc, voucherDate ?? voucher.CreateDate);
                    entry.Document.Current = doc;
                    docCache.SetValueExt<APInvoice.vendorID>(doc, vendor?.BAccountID);
                    docCache.SetValueExt<APInvoice.projectID>(doc, contract?.ContractID ?? nonProject);
                    docCache.SetValueExt<APInvoice.invoiceNbr>(doc, voucher.SubCode);
                    docCache.SetValueExt<APInvoice.docDesc>(doc, voucher.SubCode + "(" + voucher.Accuid + ")");
                    //docCache.SetValueExt<APInvoice.branchID>(doc, branch.BranchID);
                    docCache.SetValueExt<APRegisterExt.usrVoucherNo>(doc, voucher.VoucherNo);
                    docCache.SetValueExt<APRegisterExt.usrVoucherKey>(doc, voucher.VoucherKey);
                    docCache.SetValueExt<APRegisterExt.usrVoucherDate>(doc, voucherDate);
                    docCache.SetValueExt<APRegisterExt.usrIsComsData>(doc, true);
                    doc = entry.Document.Update(doc);
                    #endregion

                    #region APTran / KGBillPayment
                    //KGBillPayment需再APTran後執行
                    List<KGComsVoucherItem> paymentList = new List<KGComsVoucherItem>();
                    foreach (KGComsVoucherItem item in voucherItems[key])
                    {
                        //INV && 貸方 && DateDue 不為空才寫入KGBillPayment
                        if (doc.DocType == APDocType.Invoice && item.Cd == "C" && item.DateDue != null)
                        {
                            paymentList.Add(item);
                        }
                        //mark by louis 20220103 if item.Cd == "D" && item.DateDue != null 也寫入文件明細
                        //else if (item.DateDue == null)
                        else if( (item.DateDue == null) || 
                                (doc.DocType == APDocType.Invoice && item.Cd == "D" && item.DateDue != null)
                                )
                        {
                            decimal _base = item.Cd == "D" ? 1m : -1m;
                            decimal _adrBase = doc.DocType == APDocType.Invoice ? 1m : -1m;
                            Contract _contract = GetContract(item.Project);
                            Account _account = GetAccount(item.AccountNo);
                            APTran tran = (APTran)tranCache.CreateInstance();
                            tran = entry.Transactions.Insert(tran);
                            tranCache.SetValueExt<APTranExt.usrValuationType>(tran, "B");
                            tranCache.SetValueExt<APTran.tranDesc>(tran, item.Digest);
                            //20220223 modify by louis 寫入明細的金額如果為負數, 則單價些入正數金額, 數量寫入-1
                            if (((item.Amount ?? 0) * _base * _adrBase) < 0)
                            {
                                tranCache.SetValueExt<APTran.qty>(tran, -1m);
                                tranCache.SetValueExt<APTran.curyUnitCost>(tran, (item.Amount ?? 0) * _base * _adrBase * -1m);
                            }
                            else {
                                tranCache.SetValueExt<APTran.qty>(tran, 1m);
                                tranCache.SetValueExt<APTran.curyUnitCost>(tran, (item.Amount ?? 0) * _base * _adrBase);
                            }
                            
                            tranCache.SetValueExt<APTran.curyLineAmt>(tran, (item.Amount ?? 0) * _base * _adrBase);
                            tranCache.SetValueExt<APTran.taxCategoryID>(tran, "免稅");
                            tranCache.SetValueExt<APTran.accountID>(tran, _account?.AccountID);
                            //改由表頭預設
                            //tranCache.SetValueExt<APTran.projectID>(tran, _contract?.ContractID ?? nonProject);
                            if (tran.ProjectID != nonProject)
                            {
                                PMCostBudget pmBudget = GetPMCostBudget(tran.ProjectID);
                                tranCache.SetValueExt<APTran.taskID>(tran, pmBudget.TaskID);
                                tranCache.SetValueExt<APTran.costCodeID>(tran, pmBudget.CostCodeID);
                            }
                            tran = entry.Transactions.Update(tran);
                        }
                    }
                    //KGBillPayment
                    foreach (KGComsVoucherItem item in paymentList)
                    {
                        KGBillPayment payment = (KGBillPayment)paymentCache.CreateInstance();
                        VendorR _vendor = GetVendorR(item.UniformNo);
                        payment = entryExt.KGBillPaym.Insert(payment);
                        paymentCache.SetValueExt<KGBillPayment.pricingType>(payment, PricingType.A);
                        paymentCache.SetValueExt<KGBillPayment.paymentMethod>(payment, PaymentMethod.A);
                        paymentCache.SetValueExt<KGBillPayment.paymentPeriod>(payment, 0);
                        paymentCache.SetValueExt<KGBillPayment.paymentPct>(payment, 100m);
                        paymentCache.SetValueExt<KGBillPayment.paymentDate>(payment, item.DateDue);
                        paymentCache.SetValueExt<KGBillPayment.vendorID>(payment, _vendor.BAccountID);
                        //modify by louis 20211220 如果Nametw=null, 寫入'COMS無資料'
                        if (item.Nametw != null)
                        {
                            paymentCache.SetValueExt<KGBillPayment.checkTitle>(payment, item.Nametw);
                        }
                        else {
                            paymentCache.SetValueExt<KGBillPayment.checkTitle>(payment, "COMS無資料");
                        }
                        
                        paymentCache.SetValueExt<KGBillPayment.isPostageFree>(payment, true);
                        //paymentCache.SetValueExt<KGBillPayment.postageAmt>(payment, 0);
                        paymentCache.SetValueExt<KGBillPayment.paymentAmount>(payment, item.Amount ?? 0);
                        paymentCache.SetValueExt<KGBillPayment.remark>(payment, item.Digest);
                        payment = entryExt.KGBillPaym.Update(payment);
                    }
                    #endregion

                    #region GuiInvoice
                    if (invoices.ContainsKey(key))
                    {
                        foreach (KGComsInvoice item in invoices[key])
                        {
                            GVApGuiInvoiceRef invoice = (GVApGuiInvoiceRef)guiCache.CreateInstance();
                            invoice = entryExt.GVApGuiInvoiceRefs.Insert(invoice);
                            guiCache.SetValueExt<GVApGuiInvoiceRef.invoiceType>(invoice,
                                item.IvoKind == "1" ? GVGuiInvoiceType.INVOICE : GVGuiInvoiceType.RECEIPT);
                            guiCache.SetValueExt<GVApGuiInvoiceRef.guiInvoiceNbr>(invoice, item.InvoiceNo);
                            guiCache.SetValueExt<GVApGuiInvoiceRef.guiType>(invoice,
                                item.IvoKind == "1" ? GVGuiType.AP.GuiType_21 : GVGuiType.AP.GuiType_22);
                            guiCache.SetValueExt<GVApGuiInvoiceRef.salesAmt>(invoice, item.Amount ?? 0);
                            guiCache.SetValueExt<GVApGuiInvoiceRef.taxAmt>(invoice, item.TaxAmt ?? 0);
                            guiCache.SetValueExt<GVApGuiInvoiceRef.vendor>(invoice, vendor.BAccountID);
                            //guiCache.SetValueExt<GVApGuiInvoiceRef.vendorName>(invoice, );
                            guiCache.SetValueExt<GVApGuiInvoiceRef.vendorUniformNumber>(invoice, item.UniformNo);
                            guiCache.SetValueExt<GVApGuiInvoiceRef.voucherCategory>(invoice,
                                item.IvoKind == "1" ? GVGuiVoucherCategory.TAXABLE : GVGuiVoucherCategory.OTHERCERTIFICATE);
                            invoice = entryExt.GVApGuiInvoiceRefs.Update(invoice);
                        }
                    }
                    #endregion

                    docCache.SetValueExt<APInvoice.hold>(doc, false);
                    doc = entry.Document.Update(doc);
                    entry.Actions.PressSave();
                }
                catch (Exception e)
                {
                    SetErrorGraph(entry);
                    throw new Exception("AccUID:" + key + "; " + e.Message);
                }
            }
        }

        public void GroupVoucher(ref Dictionary<String, KGComsVoucher> vouchers)
        {
            foreach (KGComsVoucher item in Vouchers.Select())
            {
                item.SumAmt = 0;
                string key = item.Accuid;
                if (!vouchers.ContainsKey(key))
                {
                    vouchers.Add(key, item);
                }
                else
                {
                    vouchers[key] = item;
                }
            }
        }

        public void GroupVoucherItem(Dictionary<String, KGComsVoucher> vouchers, ref Dictionary<String, List<KGComsVoucherItem>> voucherItems)
        {
            foreach (KGComsVoucherItem item in VoucherItems.Select())
            {
                string key = item.AccUid;
                if (vouchers.ContainsKey(key))
                {
                    if (!voucherItems.ContainsKey(key))
                    {
                        List<KGComsVoucherItem> list = new List<KGComsVoucherItem>();
                        voucherItems.Add(key, list);
                    }
                    voucherItems[key].Add(item);
                    if (item.DateDue == null)
                        vouchers[key].SumAmt += (item.Amount ?? 0);
                }
            }
        }

        public void GroupInvoice(Dictionary<String, KGComsVoucher> vouchers, ref Dictionary<String, List<KGComsInvoice>> invoices)
        {
            foreach (KGComsInvoice item in Invoices.Select())
            {
                string key = item.Accuid;
                if (vouchers.ContainsKey(key))
                {
                    if (!invoices.ContainsKey(key))
                    {
                        List<KGComsInvoice> list = new List<KGComsInvoice>();
                        invoices.Add(key, list);
                    }
                    invoices[key].Add(item);
                }
            }
        }

        public DateTime? GetDate(string rocDate)
        {
            if (String.IsNullOrEmpty(rocDate)) return null;
            string day = rocDate.Substring(rocDate.Length - 2, 2);
            string month = rocDate.Substring(rocDate.Length - 4, 2);
            int yearLenght = rocDate.Length - 4;
            string rocYear = rocDate.Substring(0, yearLenght);
            int year = int.Parse(rocYear) + 1911;
            return DateTime.ParseExact(year.ToString() + month + day, "yyyyMMdd", null);
        }

        public void CheckVoucher(KGComsVoucher row)
        {
            row.ErrorMsg = null;
            StringBuilder sb = new StringBuilder();
            VendorR vendor = GetVendorR(row.UniformNo);
            Contract contract = GetContract(row.Project, row.CompanyName);
            Branch branch = GetBranch(row.CompanyName);
            
            if (vendor == null)
                sb.Append(row.UniformNo)
                  .Append("(").Append(row.CompanyName).Append(")")
                  .Append("廠商不存在;");
            if (contract == null)
                sb.Append(row.Project)
                    .Append("(").Append(row.CompanyName).Append(")")
                    .Append("專案不存在;");
            if (branch == null)
                sb.Append(row.CompanyName).Append("分公司不存在;");
            //modify by louis for 新增檢核資料中的分公司與使用者登入的分公司不一致
            if (this.Accessinfo.BranchID != branch.BranchID)
                sb.Append(row.CompanyName)
                  .Append("(").Append(row.CompanyName).Append(")")
                  .Append("分公司與使用者登入的分公司不一致;");
            if (String.IsNullOrEmpty(row.Accuid))
                sb.Append("AccUID不得為空;");

            if (sb.Length > 0)
                row.ErrorMsg = sb.ToString();
        }

        public void CheckVoucherItem(KGComsVoucherItem row)
        {
            row.ErrorMsg = null;
            StringBuilder sb = new StringBuilder();
            Account account = GetAccount(row.AccountNo);
            if (row.Cd == "C" && row.DateDue != null)
            {
                VendorR vendor = GetVendorR(row.UniformNo);
                if (vendor == null)
                {
                    sb.Append(row.UniformNo).Append("廠商不存在;");
                }
                else
                {
                    int? locationID = NMLocationUtil.GetDefLocationByPaymentMethod(vendor.BAccountID, PaymentMethod.A);
                    if (locationID == null)
                    {
                        sb.Append(row.UniformNo).Append("無支票的廠商所在地;");
                    }
                    else
                    {
                        Location location = GetLocation(locationID);
                        NMBankAccount bankAccount = GetBankAccount(location.VCashAccountID, "CHECK");
                        if (bankAccount == null)
                            sb.Append(row.UniformNo).Append("(").Append(location.LocationCD).Append(")")
                                .Append("無銀行帳戶主檔資訊;");
                    }
                }
            }
            else
            {
                if (String.IsNullOrEmpty(row.AccountNo))
                    sb.Append("科目不得為空;");
                else if (account == null)
                    sb.Append(row.AccountNo).Append("科目不存在;");
            }

            if (sb.Length > 0)
                row.ErrorMsg = sb.ToString();
        }

        public void CheckInvoice(KGComsInvoice row)
        {
            row.ErrorMsg = null;
            StringBuilder sb = new StringBuilder();
            GVApGuiInvoice guiInvoice = GetGVApGuiInvoice(row.InvoiceNo);
            if (String.IsNullOrEmpty(row.InvoiceNo))
                sb.Append("發票號碼不得為空;");
            if (String.IsNullOrEmpty(row.UniformNo))
                sb.Append("Uniform No不得為空;");
            //mark by louis 20211220 修改統編長度檢查不得大於8碼
            else if (row.UniformNo?.Length < 8)
                sb.Append("Uniform No不得少於8碼;");
            if (guiInvoice != null)
                sb.Append(row.InvoiceNo).Append("發票號碼已存在;");
            if (sb.Length > 0)
                row.ErrorMsg = sb.ToString();
        }



        #endregion

        #region BQL
        private Location GetLocation(int? VendorLocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(this, VendorLocationID);
        }

        private NMBankAccount GetBankAccount(int? CashAccount, string PaymentMethod)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>>>>
                .Select(this, CashAccount, PaymentMethod);
        }

        public PXResultset<KGComsVoucher> GetKGComsVoucherByError(int? batchID)
        {
            return PXSelectReadonly<KGComsVoucher,
                Where<KGComsVoucher.batchID, Equal<Required<KGComsVoucher.batchID>>,
                And<KGComsVoucher.errorMsg, IsNotNull>>>.Select(this, batchID);
        }

        public PXResultset<KGComsInvoice> GetKGComsInvoiceByError(int? batchID)
        {
            return PXSelectReadonly<KGComsInvoice,
                Where<KGComsInvoice.batchID, Equal<Required<KGComsInvoice.batchID>>,
                And<KGComsInvoice.errorMsg, IsNotNull>>>.Select(this, batchID);
        }

        public PXResultset<KGComsVoucherItem> GetKGComsVoucherItemByError(int? batchID)
        {
            return PXSelectReadonly<KGComsVoucherItem,
                Where<KGComsVoucherItem.batchID, Equal<Required<KGComsVoucherItem.batchID>>,
                And<KGComsVoucherItem.errorMsg, IsNotNull>>>.Select(this, batchID);
        }

        public PXResultset<PMCostBudget> GetPMCostBudget(int? projectID)
        {
            return PXSelectReadonly<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>>>
                .Select(this, projectID);
        }


        public Branch GetBranch(string branchCD)
        {
            return PXSelectReadonly<Branch,
                Where<Branch.branchCD, Equal<Required<Branch.branchCD>>>>
                .Select(this, branchCD);
        }

        public VendorR GetVendorR(string acctCD)
        {
            return PXSelectReadonly<VendorR,
                Where<VendorR.acctCD, Equal<Required<VendorR.acctCD>>>>
                .Select(this, acctCD);
        }

        public Account GetAccount(string accountNo)
        {
            return PXSelectReadonly<Account,
                Where<Account.accountCD, Equal<Required<Account.accountCD>>>>
                .Select(this, accountNo);
        }

        public Contract GetContract(string contractCD)
        {
            return PXSelectReadonly<Contract,
                Where<Contract.contractCD, Equal<Required<Contract.contractCD>>>>
                .Select(this, contractCD);
        }

        public Contract GetContract(string contractCD, string branchCD)
        {
            return PXSelectReadonly2<Contract,
                InnerJoin<Branch, On<Contract.defaultBranchID, Equal<Branch.branchID>>>,
                Where<Contract.contractCD, Equal<Required<Contract.contractCD>>,
                And<Branch.branchCD, Equal<Required<Branch.branchCD>>>
                >>
                .Select(this, contractCD, branchCD);
        }

        public GVApGuiInvoice GetGVApGuiInvoice(string invoiceNbr)
        {
            return PXSelectReadonly<GVApGuiInvoice,
                Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Required<GVApGuiInvoice.guiInvoiceNbr>>>>
                .Select(this, invoiceNbr);
        }

        #endregion

        #region Table
        [Serializable]
        public class KGComsFilter : IBqlTable
        {
            public virtual APInvoiceEntry Graph { get; set; }
        }
        #endregion

    }
}