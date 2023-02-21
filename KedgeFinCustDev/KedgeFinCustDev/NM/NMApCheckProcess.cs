using System;
using PX.Data;
using NM.DAC;
using NM.Util;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using System.Collections;
using PX.Objects.AP;
using PX.Objects.CS;
using System.Collections.Generic;
using PX.Objects.PO;
using PX.Objects.CA;
using RC.Util;
using Kedge.DAC;
using PX.Objects.AP.MigrationMode;
using PaymentMethod = Kedge.DAC.PaymentMethod;
using static NM.Util.NMStringList;


/**
 * ====2021-04-19:12015 ====Alton
 * NM批次開票 及 TT電匯 的OriCuryAmount，都請改抓KGBillPayment的ActPayAmt
 * 
 * ====2021-04-23:12016 ====Alton
 * 4.因應KGBillPayment產生APPayment/電匯檔/應付票據如果有依據KGBillpayment.VendorLocationID去寫入Vendor相關欄位,
 *   請依照APRegister.VendorID為準, KGBillPayment.VendorID是會套用到電匯對付款對象, 票據的支票抬頭(票據主檔的Vendor還是抓APRegister.Vendor)
 * - NMPayableCheck.VendorID = APInvoice.VendorID
 * - NMPayableCheck.VendorLocationID = APInvoice.VendorLocationID
 * - 銀行帳戶相關資訊來自於KGBillPayment.VendorLocationID
 * 
 * ====2021-04-29:12025 ====Alton
 * 1.GRID與Filter加入 UsrTrPaymntType & UsrTrConfirmID
 * 2.BatchNbr的顯示改為UsrAccConfirmNbr
 * 
 * ====2021-05-28:12069 ====Alton
 * 1.NM502000 應付票據批次開票, NM502007銀行電匯檔作業, 不要顯示計價單已反轉的資料
 * 2.判斷計價單是否已反轉, 就是只要找到有DocType = 'ADR' and OrigRefNbr = 原計價單號 and OrigDocType = 'INV', 則該計價單就有做過反轉
 * 
 * ====2021-06-23:口頭====Alton
 * 禮卷判斷改為判斷不可為nonProject
 * 
 * ====2021-07-06:12119====Alton
 * fix Vendor HyperLink 
 * 
 * ====2021-07-22:12168====Alton
 * 入到這隻作業的條件請多加 KGBillPayment.UsrIsTrConfirm 要等於True
 * **/
namespace NM
{
    public class NMApCheckProcess : PXGraph<NMApCheckProcess>
    {

        public NMApCheckProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_PAYABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.CreateCheck);
            this.ActionMenu.AddMenuAction(this.ChangeToTT);
        }

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXSelect<NMPayableCheckV, Where<True, Equal<True>,
                    And2<Where<NMPayableCheckV.vendorID, Equal<Current2<ParamTable.vendorID>>,
                        Or<Current2<ParamTable.vendorID>, IsNull>>,
                    And2<Where<Current2<ParamTable.projectID>, IsNull,
                        Or<NMPayableCheckV.projectID, Equal<Current2<ParamTable.projectID>>>>,
                    And2<Where<NMPayableCheckV.paymentMethodID, Equal<Current2<ParamTable.paymentMethodID>>,
                        Or<Current2<ParamTable.paymentMethodID>, IsNull>>,
                    And2<Where<NMPayableCheckV.cashAccountID, Equal<Current2<ParamTable.cashAccountID>>,
                        Or<Current2<ParamTable.cashAccountID>, IsNull>>,
                    And2<Where<NMPayableCheckV.bankAccountID, Equal<Current2<ParamTable.bankAccountID>>,
                        Or<Current2<ParamTable.bankAccountID>, IsNull>>,
                    And2<Where<Current2<ParamTable.dueDate>, IsNull,
                        Or<NMPayableCheckV.dueDate, Equal<Current2<ParamTable.dueDate>>>>,
                    And2<Where<Current2<ParamTable.etdDepositDate>, IsNull,
                        Or<NMPayableCheckV.etdDepositDate, LessEqual<Current2<ParamTable.etdDepositDate>>>>,
                    And2<Where<Current2<ParamTable.usrTrConfirmBy>, IsNull,
                        Or<NMPayableCheckV.usrTrConfirmBy, Equal<Current2<ParamTable.usrTrConfirmBy>>>>,
                    And2<Where<Current2<ParamTable.usrTrConfirmDate>, IsNull,
                        Or<NMPayableCheckV.usrTrConfirmDate, Equal<Current2<ParamTable.usrTrConfirmDate>>>>,
                    And2<Where<Current2<ParamTable.usrTrConfirmID>, IsNull,
                        Or<NMPayableCheckV.usrTrConfirmID, Equal<Current2<ParamTable.usrTrConfirmID>>>>,
                    And<Where<Current2<ParamTable.usrTrPaymentType>, IsNull,
                        Or<NMPayableCheckV.usrTrPaymentType, Equal<Current2<ParamTable.usrTrPaymentType>>>>
                        >>>>>>>>>>>>> DetailView;
        #endregion

        #region Action

        public PXCancel<ParamTable> Cancel;

        #region Menu
        public PXAction<ParamTable> ActionMenu;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "行動")]
        protected void actionMenu() { }
        #endregion

        #region 批次開票
        public PXAction<ParamTable> CreateCheck;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "批次開票")]
        protected IEnumerable createCheck(PXAdapter adapter)
        {
            List<NMPayableCheckV> items = new List<NMPayableCheckV>();
            PXSelectBase<NMPayableCheckV> cmd = new PXSelect<NMPayableCheckV, Where<NMPayableCheckV.selected, Equal<True>>>(this);
            PXResultset<NMPayableCheckV> rs = cmd.Select();
            foreach (NMPayableCheckV item in rs)
            {
                String error = null;
                NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableCheckV.bankAccountID>(DetailView.Cache, item);
                //2021-05-14 mark by alton KGBillpayment.Location在批次開票已經沒有任何的意義，不需檢查VendorLocationID & PaymentMethodID & CashAccountID
                //if (item.VendorLocationID == null)
                //{
                //    error = "請維護供應商所在地資訊";
                //}
                //else if (item.PaymentMethodID != "CHECK")
                //{
                //    error = "付款方式須為CHECK";
                //}
                //else if (item.CashAccountID == null)
                //{
                //    error = "請維護現金科目";
                //}
                //else 
                if (item.BankAccountID == null)
                {
                    error = "請維護銀行帳戶";
                }
                else if (nba?.EnableIssueByBank != true && String.IsNullOrEmpty(item.BookNbr))
                {
                    error = "支票簿號";
                }

                if (error != null)
                {
                    DetailView.Cache.RaiseExceptionHandling<NMPayableCheckV.refNbr>(
                                    item, item.RefNbr, new PXSetPropertyException(error, PXErrorLevel.RowError));
                }
                else
                {
                    items.Add(item);
                }
            }

            if (rs.Count != items.Count)
            {
                String error = "資料有誤";
                throw new PXException(error);
            }
            PXLongOperation.StartOperation(this, delegate () { CreateNMPaymentCheck(items); });
            return adapter.Get();
        }
        #endregion

        #region 改電匯
        public PXAction<ParamTable> ChangeToTT;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "改電匯")]
        protected IEnumerable changeToTT(PXAdapter adapter)
        {
            if (MasterView.Ask("確定改為電匯", "確定是否將勾選資訊轉至電匯作業？", MessageButtons.YesNo) == WebDialogResult.Yes)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    PXSelectBase<NMPayableCheckV> cmd = new PXSelect<NMPayableCheckV, Where<NMPayableCheckV.selected, Equal<True>>>(this);
                    PXResultset<NMPayableCheckV> rs = cmd.Select();
                    foreach (NMPayableCheckV item in rs)
                    {
                        PXUpdate<
                            Set<KGBillPayment.paymentMethod, Kedge.DAC.PaymentMethod.wireTransfer,
                            Set<KGBillPayment.vendorLocationID, Required<KGBillPayment.vendorLocationID>>>,
                        KGBillPayment,
                        Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                        .Update(this,
                            NMLocationUtil.GetDefLocationByPaymentMethod(item.VendorID, NMLocationUtil.PaymentMethodID.TT),
                            item.BillPaymentID);
                    }
                    DetailView.Cache.Clear();
                    MasterView.Cache.Clear();
                });
            }

            return adapter.Get();
        }
        #endregion

        #region HyperLink
        public PXAction<NMPayableCheckV> ViewVendor;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewVendor()
        {
            NMPayableCheckV d = DetailView.Current;
            BAccount2 ba2 = PXSelectorAttribute.Select<NMPayableCheckV.vendorID>(DetailView.Cache, d) as BAccount2;
            HyperLinkVendor(ba2);
        }

        public PXAction<NMPayableCheckV> ViewAPVendor;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPVendor()
        {
            NMPayableCheckV d = DetailView.Current;
            BAccount2 ba2 = PXSelectorAttribute.Select<NMPayableCheckV.apVendorID>(DetailView.Cache, d) as BAccount2;
            HyperLinkVendor(ba2);
        }

        public PXAction<NMPayableCheckV> ViewBatch;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewBatch()
        {
            //Batch batch = GetConfirmBranch(DetailView.Current.UsrAccConfirmNbr);
            Batch batch = (Batch)PXSelectorAttribute.Select<NMPayableCheckV.usrAccConfirmNbr>(DetailView.Cache, DetailView.Current);
            new HyperLinkUtil<JournalEntry>(batch, true);
        }
        #endregion
        #endregion

        #region Event

        #region ParamTable

        protected virtual void _(Events.FieldUpdated<ParamTable.bankAccountID> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            //e.Cache.SetDefaultExt<ParamTable.cashAccountID>(row);
            //e.Cache.SetDefaultExt<ParamTable.paymentMethodID>(row);
        }

        //protected virtual void _(Events.FieldUpdated<ParamTable.branchID> e)
        //{
        //    ParamTable row = (ParamTable)e.Row;
        //    if (row == null) return;
        //    foreach (NMPayableCheckV item in PXSelect<NMPayableCheckV>.Select(this))
        //    {
        //        DetailView.Cache.SetDefaultExt<NMPayableCheckV.branchID>(item);
        //    }
        //}

        protected virtual void _(Events.FieldUpdated<ParamTable.checkDate> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            foreach (NMPayableCheckV item in PXSelect<NMPayableCheckV>.Select(this))
            {
                DetailView.Cache.SetDefaultExt<NMPayableCheckV.checkDate>(item);
            }
        }

        protected virtual void _(Events.FieldUpdated<ParamTable.cashierID> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            foreach (NMPayableCheckV item in PXSelect<NMPayableCheckV>.Select(this))
            {
                DetailView.Cache.SetDefaultExt<NMPayableCheckV.payableCashierID>(item);
            }
        }

        protected virtual void _(Events.FieldUpdated<ParamTable.description> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            foreach (NMPayableCheckV item in PXSelect<NMPayableCheckV>.Select(this))
            {
                DetailView.Cache.SetDefaultExt<NMPayableCheckV.description>(item);
            }
        }
        #endregion

        #region NMPayableCheckV
        protected virtual void NMPayableCheckV_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMPayableCheckV row = (NMPayableCheckV)e.Row;
            if (row == null) return;
            setReadOnly();
        }

        //protected virtual void _(Events.FieldUpdated<NMPayableCheckV.selected> e)
        //{
        //    NMPayableCheckV row = (NMPayableCheckV)e.Row;
        //    if (row == null) return;
        //    DetailView.Cache.SetValueExt<NMPayableCheckV.oriCuryAmount>(row, row.OriCuryAmount);
        //    //SetSelectedInformation();
        //}

        protected virtual void _(Events.FieldUpdated<NMPayableCheckV.vendorLocationID> e)
        {
            NMPayableCheckV row = (NMPayableCheckV)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<NMPayableCheckV.cashAccountID>(row);
            e.Cache.SetDefaultExt<NMPayableCheckV.paymentMethodID>(row);
            //e.Cache.SetDefaultExt<NMPayableCheckV.bankAccountID>(row);
            //e.Cache.SetDefaultExt<NMPayableCheckV.bookNbr>(row);
            //e.Cache.SetDefaultExt<NMPayableCheckV.enableIssueByBank>(row);
        }

        //protected virtual void _(Events.FieldDefaulting<NMPayableCheckV.etdDepositDate> e)
        //{
        //    NMPayableCheckV row = (NMPayableCheckV)e.Row;
        //    e.NewValue = row?.PaymentDate;
        //}

        //protected virtual void _(Events.FieldDefaulting<NMPayableCheckV.bankAccountID> e)
        //{
        //    NMPayableCheckV row = (NMPayableCheckV)e.Row;
        //    if (row == null) return;
        //    NMBankAccount nba = GetBankAccount(row.CashAccountID, row.PaymentMethodID);
        //    e.NewValue = nba?.BankAccountID;
        //}

        protected virtual void _(Events.FieldUpdated<NMPayableCheckV.bankAccountID> e)
        {
            NMPayableCheckV row = (NMPayableCheckV)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<NMPayableCheckV.bookNbr>(row);
            e.Cache.SetDefaultExt<NMPayableCheckV.enableIssueByBank>(row);
        }

        protected virtual void _(Events.FieldDefaulting<NMPayableCheckV.bookNbr> e)
        {
            NMPayableCheckV row = (NMPayableCheckV)e.Row;
            if (row == null) return;
            NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableCheckV.bankAccountID>(e.Cache, row);
            e.NewValue = null;
            if (nba?.EnableIssueByBank != true)
            {
                NMCheckBook ncb = GetCheckBook(row.BankAccountID);
                e.NewValue = ncb?.BookCD;
            }
        }

        protected virtual void _(Events.FieldDefaulting<NMPayableCheckV.enableIssueByBank> e)
        {
            NMPayableCheckV row = (NMPayableCheckV)e.Row;
            if (row == null) return;
            NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableCheckV.bankAccountID>(e.Cache, row);
            e.NewValue = nba?.EnableIssueByBank;
        }
        #endregion

        #endregion

        #region Method
        private void HyperLinkVendor(BAccount2 ba2)
        {
            if (ba2 == null) return;
            if (ba2.Type == BAccountType.VendorType)
            {
                VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
                graph.BAccount.Current = graph.BAccount.Search<BAccount.bAccountID>(ba2.BAccountID);
                if (graph.BAccount.Current != null)
                {
                    throw new PXRedirectRequiredException(graph, "VendorMaint")
                    {
                        Mode = PXBaseRedirectException.WindowMode.NewWindow
                    };
                }
            }
            else if (ba2.Type == BAccountType.EmployeeType)
            {
                EmployeeMaint graph = PXGraph.CreateInstance<EmployeeMaint>();
                graph.BAccount.Current = graph.BAccount.Search<BAccount.bAccountID>(ba2.BAccountID);

                if (graph.BAccount.Current != null)
                {
                    throw new PXRedirectRequiredException(graph, "EmployeeMaint")
                    {
                        Mode = PXBaseRedirectException.WindowMode.NewWindow
                    };
                }
            }
        }

        //private void SetSelectedInformation()
        //{
        //    ParamTable p = MasterView.Current;
        //    int count = 0;
        //    decimal oriAmt = 0m;
        //    decimal basAmt = 0m;
        //    foreach (NMPayableCheckV item in PXSelect<NMPayableCheckV, Where<NMPayableCheckV.selected, Equal<True>>>.Select(this))
        //    {
        //        count += 1;
        //        oriAmt += item.OriCuryAmount ?? 0m;
        //        basAmt += item.BaseCuryAmount ?? 0m;
        //    }
        //    p.SelectedCount = count;
        //    p.SelectedOriCuryAmount = oriAmt;
        //    p.SelectedBaseCuryAmount = basAmt;

        //}

        /// <summary>
        /// 開票
        /// </summary>
        private void CreateNMPaymentCheck(List<NMPayableCheckV> items)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                NMApCheckEntry entry = PXGraph.CreateInstance<NMApCheckEntry>();
                foreach (NMPayableCheckV item in items)
                {
                    CreateNMPaymentCheck(item, entry);
                }
                ts.Complete();
                //SetSelectedInformation();
            }
            DetailView.Cache.Clear();
            MasterView.Cache.Clear();
        }

        public virtual void CreateNMPaymentCheck(NMPayableCheckV item, NMApCheckEntry entry)
        {
            NMPayableCheck newItem = (NMPayableCheck)entry.PayableChecks.Cache.CreateInstance();
            PXCache cache = entry.PayableChecks.Cache;
            newItem = entry.PayableChecks.Insert(newItem);
            cache.SetValueExt<NMPayableCheck.refNbr>(newItem, item.RefNbr);
            cache.SetValueExt<NMPayableCheck.projectID>(newItem, item.ProjectID);
            cache.SetValueExt<NMPayableCheck.projectPeriod>(newItem, item.ProjectPeriod);
            cache.SetValueExt<NMPayableCheck.sourceCode>(newItem, item.SourceCode);
            cache.SetValueExt<NMPayableCheck.curyID>(newItem, item.CuryID);
            cache.SetValueExt<NMPayableCheck.curyInfoID>(newItem, item.CuryInfoID);
            cache.SetValueExt<NMPayableCheck.oriCuryAmount>(newItem, item.OriCuryAmount);
            //cache.SetValueExt<NMPayableCheck.baseCuryAmount>(newItem,);
            cache.SetValueExt<NMPayableCheck.branchID>(newItem, item.BranchID);
            cache.SetValueExt<NMPayableCheck.checkDate>(newItem, item.CheckDate);
            cache.SetValueExt<NMPayableCheck.etdDepositDate>(newItem, item.EtdDepositDate);
            //mark by louis 20220622 修改產生應付票據的廠商跟所在地改寫入KGBillpayment的廠商跟所在地
            //cache.SetValueExt<NMPayableCheck.vendorID>(newItem, item.APVendorID);
            //cache.SetValueExt<NMPayableCheck.vendorLocationID>(newItem, item.APVendorLocationID);
            cache.SetValueExt<NMPayableCheck.vendorID>(newItem, item.VendorID);
            cache.SetValueExt<NMPayableCheck.vendorLocationID>(newItem, item.VendorLocationID);

            cache.SetValueExt<NMPayableCheck.title>(newItem, item.CheckTitle);
            cache.SetValueExt<NMPayableCheck.bankAccountID>(newItem, item.BankAccountID);
            cache.SetValueExt<NMPayableCheck.bookNbr>(newItem, item.BookNbr);
            cache.SetValueExt<NMPayableCheck.payableCashierID>(newItem, item.PayableCashierID);
            cache.SetValueExt<NMPayableCheck.description>(newItem, item.Description);
            cache.SetValueExt<NMPayableCheck.billPaymentID>(newItem, item.BillPaymentID);
            entry.PayableChecks.Update(newItem);
            entry.Persist();

            //更新KGBillPaymentExt.UsrIsCheckIssued = true (代表已被使用)
            //更新KGBillPayment.VendorLocationID
            PXUpdate<
                Set<KGBillPaymentExt.usrIsCheckIssued, True
                //Set<KGBillPayment.vendorLocationID, Required<KGBillPayment.vendorLocationID>>
                >, KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Update(this,
                //item.VendorLocationID,
                item.BillPaymentID);
        }

        public void setReadOnly()
        {
            NMPayableCheckV row = DetailView.Current;
            PXUIFieldAttribute.SetEnabled(DetailView.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<NMPayableCheckV.selected>(DetailView.Cache, row, true);
            PXUIFieldAttribute.SetEnabled<NMPayableCheckV.vendorLocationID>(DetailView.Cache, row, true);
            PXUIFieldAttribute.SetEnabled<NMPayableCheckV.bookNbr>(DetailView.Cache, row, true);
            PXUIFieldAttribute.SetEnabled<NMPayableCheckV.bankAccountID>(DetailView.Cache, row, true);
            DetailView.AllowDelete = false;
            DetailView.AllowInsert = false;
        }

        #endregion

        #region BQL
        //public Batch GetConfirmBranch(string accConfirmNbr) {
        //    return PXSelect<Batch,
        //        Where<BatchExt.usrAccConfirmNbr, Equal<Required<BatchExt.usrAccConfirmNbr>>,
        //        And<Batch.module,Equal< BatchModule.moduleGL>>>>
        //        .Select(this, accConfirmNbr);
        //}

        public NMBankAccount GetBankAccount(int? cashAccountID, string payMethodID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.isSettlement, Equal<False>,
                And<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>>>>>
                .Select(this, cashAccountID, payMethodID);
        }

        public NMCheckBook GetCheckBook(int? bankAccountID)
        {
            return PXSelect<NMCheckBook,
                Where<NMCheckBook.bankAccountID, Equal<Required<NMCheckBook.bankAccountID>>,
                    And<IsNull<NMCheckBook.bookUsage, NMBookUsage.check>, Equal<NMBookUsage.check>,
                    And<Where<NMCheckBook.currentCheckNbr, NotEqual<NMCheckBook.endCheckNbr>, Or<NMCheckBook.currentCheckNbr, IsNull>>>>>>
                .Select(this, bankAccountID);
        }

        #endregion

        #region Table
        #region ParamTable
        [Serializable]
        public class ParamTable : IBqlTable
        {
            #region summary Area
            //#region BranchID
            //[PXInt()]
            //[PXUIField(DisplayName = "Branch ID")]
            //[PXUnboundDefault(typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>))]
            //[PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            //public virtual int? BranchID { get; set; }
            //public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            //#endregion

            #region CheckDate
            [PXDate()]
            [PXUIField(DisplayName = "Check Date")]
            [PXUnboundDefault(typeof(AccessInfo.businessDate))]
            public virtual DateTime? CheckDate { get; set; }
            public abstract class checkDate : PX.Data.BQL.BqlDateTime.Field<checkDate> { }
            #endregion

            #region CashierID
            [PXInt()]
            [PXUIField(DisplayName = "Cashier")]
            [PXEPEmployeeSelector]
            [PXUnboundDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
            public virtual int? CashierID { get; set; }
            public abstract class cashierID : PX.Data.BQL.BqlInt.Field<cashierID> { }
            #endregion

            #region Description
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Description")]
            public virtual string Description { get; set; }
            public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
            #endregion
            #endregion

            #region Selection Area
            #region BankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Bank Account ID")]
            [NMBankAccount]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor ID")]
            [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>,
                Or<BAccount2.type, Equal<BAccountType.combinedType>,
                Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>>),
                typeof(BAccount2.acctCD),
                typeof(BAccount2.acctName),
                typeof(BAccount2.status),
                typeof(BAccount2.defAddressID),
                typeof(BAccount2.defContactID),
                typeof(BAccount2.defLocationID),
                        SubstituteKey = typeof(BAccount.acctCD),
                        DescriptionField = typeof(BAccount.acctName))]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

            #region ProjectID
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            [PXUIField(DisplayName = "Project ID")]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region PayDate
            [PXDate()]
            [PXUIField(DisplayName = "Etd Deposit Date")]
            public virtual DateTime? EtdDepositDate { get; set; }
            public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
            #endregion

            #region DueDate
            [PXDate()]
            [PXUIField(DisplayName = "Due Date")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>), PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
            #endregion

            #region PaymentMethodID
            [PXString()]
            [PXUIField(DisplayName = "Payment Method ID", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<NMBankAccount.paymentMethodID,
                Where<NMBankAccount.bankAccountID, Equal<Current<bankAccountID>>>>))]
            public virtual string PaymentMethodID { get; set; }
            public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
            #endregion

            #region CashAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Cash Account ID", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<NMBankAccount.cashAccountID,
                Where<NMBankAccount.bankAccountID, Equal<Current<bankAccountID>>>>))]
            public virtual int? CashAccountID { get; set; }
            public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
            #endregion

            #endregion

            #region Selected Information
            #region UnBranchID - 計算加總用
            [PXInt()]
            [PXUnboundDefault(typeof(Current<AccessInfo.branchID>))]
            public virtual int? UnBranchID { get; set; }
            public abstract class unBranchID : PX.Data.BQL.BqlInt.Field<unBranchID> { }
            #endregion

            #region SelectedCount
            [PXInt()]
            [PXUIField(DisplayName = "Selected Count", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Int32, "0")]
            public virtual int? SelectedCount { get; set; }
            public abstract class selectedCount : PX.Data.BQL.BqlInt.Field<selectedCount> { }
            #endregion

            #region SelectedOriCuryAmount
            [PXDecimal(4)]
            [PXUIField(DisplayName = "Selected Ori Cury Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SelectedOriCuryAmount { get; set; }
            public abstract class selectedOriCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedOriCuryAmount> { }
            #endregion

            //#region SelectedBaseCuryAmount
            //[PXDecimal(4)]
            //[PXUIField(DisplayName = "Selected Base Cury Amount", IsReadOnly = true)]
            //[PXUnboundDefault(TypeCode.Decimal, "0.0")]
            //public virtual Decimal? SelectedBaseCuryAmount { get; set; }
            //public abstract class selectedBaseCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedBaseCuryAmount> { }
            //#endregion

            #region UsrTrPaymentType
            [PXString()]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string UsrTrPaymentType { get; set; }
            public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
            #endregion

            #region UsrTrConfirmID
            [PXInt()]
            [PXUIField(DisplayName = "UsrTrConfirmID")]
            public virtual int? UsrTrConfirmID { get; set; }
            public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
            #endregion

            #region UsrTrConfirmDate
            [PXDate()]
            [PXUIField(DisplayName = "UsrTrConfirmDate")]
            public virtual DateTime? UsrTrConfirmDate { get; set; }
            public abstract class usrTrConfirmDate : IBqlField { }
            #endregion

            #region UsrTrConfirmBy
            [PXGuid()]
            [PXUIField(DisplayName = "UsrTrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? UsrTrConfirmBy { get; set; }
            public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
            #endregion

            #endregion
        }
        #endregion

        #region NMPayableCheckV
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<APInvoice,
            InnerJoin<KGBillPayment,
                On<KGBillPayment.refNbr, Equal<APInvoice.refNbr>>,
            InnerJoin<APRegister,
                On<APRegister.refNbr, Equal<APInvoice.refNbr>,
                And<APRegister.docType, Equal<APInvoice.docType>>>,
             LeftJoin<Location,
                 On<Location.locationID, Equal<KGBillPayment.vendorLocationID>,
                 And<Location.bAccountID, Equal<KGBillPayment.vendorID>>>,
              LeftJoin<APInvoiceDebit,
                 On<APInvoiceDebit.origRefNbr, Equal<APRegister.refNbr>,
                 And<APInvoiceDebit.origDocType, Equal<APRegister.docType>>>>
                 >>>,
             Where<
                    APInvoice.openDoc, Equal<True>,
                    And<APRegisterFinExt.usrIsConfirm, Equal<True>,
                    And2<Where<
                        APInvoice.released, Equal<True>,
                        Or<APInvoice.prebooked, Equal<True>>>,
                    And2<Where<
                        APInvoice.docType, Equal<APDocType.invoice>,//計價
                        Or<APInvoice.docType, Equal<APDocType.creditAdj>,//貸方調整
                        Or<APInvoice.docType, Equal<APDocType.prepayment>>>>,//預付款
                    And<IsNull<KGBillPaymentExt.usrIsCheckIssued, False>, Equal<False>,
                    And<APInvoice.status, Equal<APDocStatus.open>,
                    And<APInvoice.curyDocBal, Greater<decimal0>,
                    And<KGBillPayment.paymentAmount, Greater<decimal0>,
                    //And<KGBillPayment.postageAmt, Greater<decimal0>,//郵資費要大於0，(為了禮券
                    And2<
                        Where<KGBillPayment.paymentMethod, Equal<PaymentMethod.check>,//付款方式：支票
                            Or<
                                Where<KGBillPayment.paymentMethod, Equal<PaymentMethod.giftCertificate>,
                                    And<APRegister.projectID, NotEqual<Zero>>>>>,//付款方式：禮卷，Project != nonProject
                    And<APInvoiceDebit.refNbr, IsNull,
                    And<KGBillPaymentExt.usrIsTrConfirm, Equal<True>>>//12168:KGBillPayment.UsrIsTrConfirm 要等於True
                        >>>>>>>>>
            >), Persistent = false)]
        public partial class NMPayableCheckV : IBqlTable
        {

            #region Selected
            [PXDBBool]
            [PXUIField(DisplayName = "Selected")]
            public virtual bool? Selected { get; set; }
            public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
            #endregion

            #region BranchID
            [PXDBInt(BqlField = typeof(APInvoice.branchID))]
            [PXUIField(DisplayName = "Branch ID")]
            //[PXUnboundDefault(typeof(Current<ParamTable.branchID>))]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            public virtual int? BranchID { get; set; }
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            #endregion

            #region PayableCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Payable Cashier ID", Required = true)]
            [PXUnboundDefault(typeof(Current<ParamTable.cashierID>), PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [PXEPEmployeeSelector]
            public virtual int? PayableCashierID { get; set; }
            public abstract class payableCashierID : PX.Data.BQL.BqlInt.Field<payableCashierID> { }
            #endregion

            #region CheckDate
            [PXDate()]
            [PXUIField(DisplayName = "Check Date", Required = true)]
            [PXUnboundDefault(typeof(Current<ParamTable.checkDate>), PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual DateTime? CheckDate { get; set; }
            public abstract class checkDate : PX.Data.BQL.BqlDateTime.Field<checkDate> { }
            #endregion

            #region Description
            [PXString(255, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Description")]
            [PXUnboundDefault(typeof(Current<ParamTable.description>))]
            public virtual string Description { get; set; }
            public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
            #endregion

            //#region PaymentDate
            //[PXDBDate(BqlField = typeof(KGBillPayment.paymentDate))]
            //[PXUIField(DisplayName = "Payment Date")]
            //public virtual DateTime? PaymentDate { get; set; }
            //public abstract class paymentDate : PX.Data.BQL.BqlDateTime.Field<paymentDate> { }
            //#endregion

            //#region BatchNbr
            //[PXDBString(InputMask = "", BqlField = typeof(APInvoice.batchNbr))]
            //[PXUIField(DisplayName = "Batch Nbr")]
            //[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleAP>>>))]
            //public virtual string BatchNbr { get; set; }
            //public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
            //#endregion

            #region UsrAccConfirmNbr
            [PXDBString(BqlField = typeof(APRegisterFinExt.usrAccConfirmNbr))]
            [PXSelector(typeof(Search<BatchExt.usrAccConfirmNbr, Where<Batch.module, Equal<BatchModule.moduleGL>>>))]
            [PXUIField(DisplayName = "UsrAccConfirmNbr")]

            public virtual string UsrAccConfirmNbr { get; set; }
            public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
            #endregion

            #region EtdDepositDate
            //edit 11829 20201207 - 「預計兌現日」＝「付款日」( EtdDepositDate = PaymentDate )
            [PXDBDate(BqlField = typeof(KGBillPayment.paymentDate))]
            [PXUIField(DisplayName = "Etd Deposit Date")]
            //[PXUnboundDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual DateTime? EtdDepositDate { get; set; }
            public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
            #endregion

            #region CuryID
            [PXDBString(IsUnicode = true, BqlField = typeof(APInvoice.curyID))]
            [PXUIField(DisplayName = "CuryID", Required = true)]
            [PXSelector(typeof(Currency.curyID))]
            public virtual string CuryID { get; set; }
            public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
            #endregion

            #region CuryInfoID
            [PXDBLong(BqlField = typeof(APInvoice.curyInfoID))]
            [CurrencyInfo()]
            public virtual Int64? CuryInfoID { get; set; }
            public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
            protected Int64? _CuryInfoID;
            #endregion

            #region OriCuryAmount
            [PXDBCurrency(typeof(curyInfoID), typeof(baseCuryAmount), BqlField = typeof(KGBillPayment.actPayAmt))]
            [PXUIField(DisplayName = "Ori Cury Amount", Required = true)]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual Decimal? OriCuryAmount { get; set; }
            public abstract class oriCuryAmount : PX.Data.BQL.BqlDecimal.Field<oriCuryAmount> { }
            #endregion

            #region BaseCuryAmount
            [PXBaseCury]
            [PXUIField(DisplayName = "Base Cury Amount", IsReadOnly = true)]
            public virtual Decimal? BaseCuryAmount { get; set; }
            public abstract class baseCuryAmount : PX.Data.BQL.BqlDecimal.Field<baseCuryAmount> { }
            #endregion

            #region SourceCode
            [PXInt()]
            [PXUIField(DisplayName = "Source Code", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.String, "2")]
            [NMStringList.NMAPSourceCode]
            public virtual int? SourceCode { get; set; }
            public abstract class sourceCode : PX.Data.BQL.BqlInt.Field<sourceCode> { }
            #endregion

            #region VendorID
            [PXDBInt(BqlField = typeof(KGBillPayment.vendorID))]
            [PXUIField(DisplayName = "Vendor ID", Required = true)]
            [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>,
                Or<BAccount2.type, Equal<BAccountType.combinedType>,
                Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>>),
                typeof(BAccount2.acctCD),
                typeof(BAccount2.acctName),
                typeof(BAccount2.status),
                typeof(BAccount2.defAddressID),
                typeof(BAccount2.defContactID),
                typeof(BAccount2.defLocationID),
            SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

            #region VendorLocationID
            [PXDBInt(BqlField = typeof(KGBillPayment.vendorLocationID))]
            [PXUIField(DisplayName = "Vendor Location ID", IsReadOnly = true)]
            [PXSelector(typeof(Search2<Location.locationID,
                InnerJoin<BAccount2,
                    On<BAccount2.bAccountID, Equal<Location.bAccountID>>>,
                Where2<
                    Where<BAccount2.type, Equal<BAccountType.employeeType>, Or<Location.vPaymentMethodID, Equal<word.check>>>,
                    And<Location.bAccountID, Equal<Current<vendorID>>>>>),
                typeof(Location.locationCD),
                typeof(Location.descr),
                typeof(Location.paymentMethodID),
                typeof(Location.cashAccountID),
                SubstituteKey = typeof(Location.locationCD))]
            public virtual int? VendorLocationID { get; set; }
            public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
            #endregion

            #region ProjectID
            [ProjectBase(BqlField = typeof(APInvoice.projectID))]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            [PXUIField(DisplayName = "Project ID", Required = true)]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [ProjectDefault()]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region ProjectPeriod
            [PXDBInt(BqlField = typeof(APRegisterExt.usrValuationPhase))]
            [PXUIField(DisplayName = "Project Period")]
            public virtual int? ProjectPeriod { get; set; }
            public abstract class projectPeriod : PX.Data.BQL.BqlInt.Field<projectPeriod> { }
            #endregion

            #region RefNbr
            [PXDBString(15, InputMask = "", BqlField = typeof(APInvoice.refNbr))]
            [PXUIField(DisplayName = "RefN br")]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region DocType
            [PXDBString(3, IsFixed = true, BqlField = typeof(APInvoice.docType))]
            [PXDefault]
            [APMigrationModeDependentInvoiceTypeList]
            [PXUIField(DisplayName = "Doc Type")]
            public virtual string DocType { get; set; }
            public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
            #endregion

            #region BillPaymentID
            [PXDBInt(BqlField = typeof(KGBillPayment.billPaymentID), IsKey = true)]
            public virtual int? BillPaymentID { get; set; }
            public abstract class billPaymentID : PX.Data.BQL.BqlInt.Field<billPaymentID> { }
            #endregion

            #region DueDate
            [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
            [PXUIField(DisplayName = "Due Date")]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
            #endregion

            #region CashAccountID
            [PXDBInt(BqlField = typeof(Location.cashAccountID), FieldOrdinal = 1)]
            [PXUIField(DisplayName = "Cash Account ID", IsReadOnly = true)]
            [PXDefault(typeof(Search<Location.cashAccountID,
              Where<Location.locationID, Equal<Current<vendorLocationID>>,
                  And<Location.bAccountID, Equal<Current<vendorID>>>>>))]
            [PXSelector(typeof(Search<CashAccount.cashAccountID, Where<CashAccount.active, Equal<True>>>),
                      typeof(CashAccount.cashAccountCD),
                      typeof(CashAccount.descr),
                      typeof(CashAccount.accountID),
                      typeof(CashAccount.subID),
                      SubstituteKey = typeof(CashAccount.cashAccountCD),
                      DescriptionField = typeof(CashAccount.descr)
                     )]
            public virtual int? CashAccountID { get; set; }
            public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
            #endregion

            #region PaymentMethodID
            [PXDBString(BqlField = typeof(Location.vPaymentMethodID))]
            [PXUIField(DisplayName = "Payment Method ID", IsReadOnly = true)]
            [PXSelector(typeof(Search<PaymentMethodAccount.paymentMethodID>),
                typeof(PaymentMethodAccount.paymentMethodID))]
            [PXDefault(typeof(Search<Location.vPaymentMethodID,
                Where<Location.locationID, Equal<Current<vendorLocationID>>,
                    And<Location.bAccountID, Equal<Current<vendorID>>>>>))]
            public virtual string PaymentMethodID { get; set; }
            public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
            #endregion

            #region BankAccountID
            [PXDBInt(BqlField = typeof(KGBillPayment.bankAccountID))]
            [PXUIField(DisplayName = "Bank Account ID", Required = true)]
            [PXSelector(typeof(Search<NMBankAccount.bankAccountID,
                Where<NMBankAccount.isSettlement, Equal<False>,
                And<NMBankAccount.paymentMethodID, Equal<word.check>>>
                    >),
                typeof(NMBankAccount.bankAccountCD),
                typeof(NMBankAccount.bankName),
                typeof(NMBankAccount.bankShortName),
                typeof(NMBankAccount.bankCode),
                typeof(NMBankAccount.bankAccount),
                SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                DescriptionField = typeof(NMBankAccount.bankName))]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

            #region BookNbr
            [PXString(50, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Book Nbr", Required = true)]
            [PXSelector(
                typeof(Search<NMCheckBook.bookCD,
                    Where<NMCheckBook.bankAccountID, Equal<Current<NMPayableCheckV.bankAccountID>>,
                        And<IsNull<NMCheckBook.bookUsage, NMBookUsage.check>, Equal<NMBookUsage.check>,
                        And<Where<NMCheckBook.currentCheckNbr, NotEqual<NMCheckBook.endCheckNbr>,
                            Or<NMCheckBook.currentCheckNbr, IsNull>>>>>>),
                typeof(NMCheckBook.bookCD),
                typeof(NMCheckBook.startCheckNbr),
                typeof(NMCheckBook.currentCheckNbr),
                typeof(NMCheckBook.endCheckNbr),
                typeof(NMCheckBook.startDate),
                typeof(NMCheckBook.description),
                DescriptionField = typeof(NMCheckBook.description)
                )]
            [PXUnboundDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual string BookNbr { get; set; }
            public abstract class bookNbr : PX.Data.BQL.BqlString.Field<bookNbr> { }
            #endregion

            #region EnableIssueByBank
            [PXBool()]
            [PXUnboundDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
            [PXUIField(DisplayName = "Enable Issue By Bank")]
            public virtual bool? EnableIssueByBank { get; set; }
            public abstract class enableIssueByBank : PX.Data.BQL.BqlBool.Field<enableIssueByBank> { }
            #endregion

            #region BookNbr
            [PXDBString(BqlField = typeof(KGBillPayment.checkTitle), IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Check Title")]
            public virtual string CheckTitle { get; set; }
            public abstract class checkTitle : PX.Data.BQL.BqlString.Field<checkTitle> { }
            #endregion
            //#region BillPaymentMethod
            //[PXDBString(BqlField =typeof(KGBillPayment.paymentMethod))]
            //[PXUIField(DisplayName = "Bill Payment Method")]
            //[PaymentMethod.List]
            //public virtual string BillPaymentMethod { get; set; }
            //public abstract class billPaymentMethod : PX.Data.BQL.BqlString.Field<billPaymentMethod> { }
            //#endregion

            #region APVendorID
            [PXDBInt(BqlField = typeof(APRegister.vendorID))]
            [PXUIField(DisplayName = "AP Vendor ID")]
            [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>,
                Or<BAccount2.type, Equal<BAccountType.combinedType>,
                Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>>),
                typeof(BAccount2.acctCD),
                typeof(BAccount2.acctName),
                typeof(BAccount2.status),
                typeof(BAccount2.defAddressID),
                typeof(BAccount2.defContactID),
                typeof(BAccount2.defLocationID),
            SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
            public virtual int? APVendorID { get; set; }
            public abstract class apVendorID : PX.Data.BQL.BqlInt.Field<apVendorID> { }
            #endregion

            #region APVendorLocationID
            [PXDBInt(BqlField = typeof(APRegister.vendorLocationID))]
            [PXUIField(DisplayName = "AP Vendor Location ID")]
            [PXSelector(typeof(Search2<Location.locationID,
                InnerJoin<BAccount2,
                    On<BAccount2.bAccountID, Equal<Location.bAccountID>>>,
                Where<Location.bAccountID, Equal<Current<apVendorID>>>>),
                typeof(Location.locationCD),
                typeof(Location.descr),
                typeof(Location.paymentMethodID),
                typeof(Location.cashAccountID),
                SubstituteKey = typeof(Location.locationCD))]
            public virtual int? APVendorLocationID { get; set; }
            public abstract class apVendorLocationID : PX.Data.BQL.BqlInt.Field<apVendorLocationID> { }
            #endregion

            #region UsrTrPaymentType
            [PXDBString(BqlField = typeof(KGBillPaymentExt.usrTrPaymentType))]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string UsrTrPaymentType { get; set; }
            public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
            #endregion

            #region UsrTrConfirmID
            [PXDBInt(BqlField = typeof(KGBillPaymentExt.usrTrConfirmID))]
            [PXUIField(DisplayName = "UsrTrConfirmID")]
            public virtual int? UsrTrConfirmID { get; set; }
            public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
            #endregion

            #region UsrTrConfirmDate
            [PXDBDate(BqlField = typeof(KGBillPaymentExt.usrTrConfirmDate))]
            [PXUIField(DisplayName = "UsrTrConfirmDate")]
            public virtual DateTime? UsrTrConfirmDate { get; set; }
            public abstract class usrTrConfirmDate : IBqlField { }
            #endregion

            #region UsrTrConfirmBy
            [PXDBGuid(BqlField = typeof(KGBillPaymentExt.usrTrConfirmBy))]
            [PXUIField(DisplayName = "UsrTrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? UsrTrConfirmBy { get; set; }
            public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
            #endregion

            #region 計算加總用
            #region UnBranchID
            [PXDBInt(BqlField = typeof(APInvoice.branchID))]
            [PXUnboundDefault(typeof(Current<AccessInfo.branchID>))]
            [PXParent(typeof(Select<ParamTable,
                            Where<ParamTable.unBranchID,
                            Equal<Current<unBranchID>>>>))]
            public virtual int? UnBranchID { get; set; }
            public abstract class unBranchID : PX.Data.BQL.BqlInt.Field<unBranchID> { }
            #endregion

            #region UnOriCuryAmount
            [PXDecimal]
            [PXFormula(typeof(Switch<
                Case<Where<selected, Equal<True>>, IsNull<oriCuryAmount, decimal0>,
                Case<Where<selected, Equal<False>>, decimal0>>
                >), typeof(SumCalc<ParamTable.selectedOriCuryAmount>))]
            public virtual Decimal? UnOriCuryAmount { get; set; }
            public abstract class unOriCuryAmount : PX.Data.BQL.BqlDecimal.Field<unOriCuryAmount> { }
            #endregion

            #region UnCount
            [PXInt]
            [PXFormula(typeof(Switch<
                Case<Where<selected, Equal<True>>, int1,
                Case<Where<selected, Equal<False>>, int0>>
                >), typeof(SumCalc<ParamTable.selectedCount>))]
            public virtual int? UnCount { get; set; }
            public abstract class unCount : PX.Data.BQL.BqlInt.Field<unCount> { }
            #endregion

            #endregion

        }
        #endregion

        #region APInvoiceDebit
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select<APInvoice,
            Where<APInvoice.docType, Equal<APDocType.debitAdj>,
                And<APRegisterExt.usrIsDeductionDoc, NotEqual<True>>>
            >), Persistent = false)]
        public partial class APInvoiceDebit : IBqlTable
        {

            #region RefNbr
            [PXDBString(15, InputMask = "", BqlField = typeof(APInvoice.refNbr))]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region DocType
            [PXDBString(3, IsFixed = true, BqlField = typeof(APInvoice.docType))]
            public virtual string DocType { get; set; }
            public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
            #endregion

            #region OrigRefNbr
            [PXDBString(15, InputMask = "", BqlField = typeof(APInvoice.origRefNbr))]
            public virtual string OrigRefNbr { get; set; }
            public abstract class origRefNbr : PX.Data.BQL.BqlString.Field<origRefNbr> { }
            #endregion

            #region OrigDocType
            [PXDBString(3, IsFixed = true, BqlField = typeof(APInvoice.origDocType))]
            public virtual string OrigDocType { get; set; }
            public abstract class origDocType : PX.Data.BQL.BqlString.Field<origDocType> { }
            #endregion
        }
        #endregion
        #endregion

    }

}