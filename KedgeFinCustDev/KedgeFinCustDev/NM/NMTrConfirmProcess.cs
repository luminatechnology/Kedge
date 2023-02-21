using System;
using System.Collections;
using System.Collections.Generic;
using Kedge.DAC;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.PO;
using RC.Util;
using static NM.Util.NMStringList;
using System.Linq;

namespace NM
{
    /**
     * ===2021/05/17 :Tiifany 口頭=== Althea
     * 篩選資料多加一個條件:
     * APInvoice.BactchNbr<>null
     * 
     * ===2021/05/19:Eva 口頭===Althea
     * 做完取消確認要把付款類別 清空、付款批次 回到預設1
     * 
     * ===2021/05/20:Eva 口頭 ===Althea
     * 上面的金額加總改為用扣掉郵資費的實付金額
     * Grid Add EnableIssueByBank to Show Default: NMBankAccount.EnableIssueByBank
     * ViewTable 新增條件KGBillPaymentExt.usrIsCheckIssued<>True
     * 
     * ===2021/05/31: Tiffany 口頭 ===Althea
     * 做完資料確認要把付款類別 清空、付款批次 回到預設1
     * 狀態為已確認不可再用資料確認按鈕
     * 
     * ===2021/06/01: 0012070 ===Althea
     * Add to Show:
     * AcctName/ BankAcct/ BankName/ BankNbr/ BranchAcct/ BranchName
     * Check Title
     * PostalCode
     * 
     * ====2021-07-09:12139====Alton
     * 1.GRID 請加篩 AP Status = Open
     * 2.排除有被開過ADR的單
     * 
     * ===2021/07/20 : 0012165 ===Althea
     * Add  Filter: 
     * AuthCount int Enable = false
     * AuthTotalAmt decimal  Enable = false
     * PaymentMethod Selector (The same with KGBillPayment)
     * BankAccountID Selector (The same with KGBillPayment)
     *  
     * Add Action :
     * 1)付款確認:        將KGBillPayment.UsrIsTrConfirm = True
     * 2)取消付款確認: 將KGBillPayment.UsrIsTrConfirm = False
     * 3)付款異動:       將KGBillPayment.PaymentMethod = Filters.PaymentMethod & 將KGBillPayment.BankAccountID = Filters.BankAccountID
     *                         KGBillPayment.VendorLocationID = DefLocationID
     * 
     * Modify Action Name:
     * 1)資料確認 -->確認分類 
     * 2)取消確認 --> 取消分類
     * 
     *Modify View Condition:
     *NMTrConfirmFilter.Status = KGBillPayment.UsrIsTrConfim
     *
     *===2021/07/27 : 0012165 ===Althea
     *付款異動Method Modify:
     *原先VendorLocationID 回壓是抓 表頭"付款方式"+VendorID對應的defLocationID
     *改為VendorLocationID 回壓式抓 表頭"BankAccountID" + VendorID 對應的LocationID ,若有多筆先抓有UsrIsReserveAcct = True的資料
     *
     *===2021/08/02 : 0012176 === Althea
     *NMTrConfirmProcess View 不包含 PaymentMethod = F(暫付沖銷)
     *
     *===2021/08/04 : 0012182 === Althea
     *ViewTable Join Vendor OrderBy 先看到Vendor.UsrIsAlertVendor = True的資料
     *
     **/
    public class NMTrConfirmProcess : PXGraph<NMTrConfirmProcess>
    {
        #region Data Views
        //public PXSave<NMTrConfirmFilter> Save;
        //public PXCancel<NMTrConfirmFilter> Cancel;

        public PXFilter<NMTrConfirmFilter> Filters;
        public PXSelect<ViewTable, Where<True, Equal<True>,
                        And2<Where<
                              ViewTable.payDate, Equal<Current2<NMTrConfirmFilter.payDate>>,
                              Or<Current2<NMTrConfirmFilter.payDate>, IsNull>>,
                       And2<Where<
                               ViewTable.projectID, Equal<Current2<NMTrConfirmFilter.contractID>>,
                               Or<Current2<NMTrConfirmFilter.contractID>, IsNull>>,
                         And2<Where<
                               ViewTable.vendorID, Equal<Current2<NMTrConfirmFilter.vendor>>,
                               Or<Current2<NMTrConfirmFilter.vendor>, IsNull>>,
                         And2<Where<
                               ViewTable.dueDate, GreaterEqual<Current2<NMTrConfirmFilter.dueDateFrom>>,
                               Or<Current2<NMTrConfirmFilter.dueDateFrom>, IsNull>>,
                         And2<Where<
                               ViewTable.dueDate, LessEqual<Current2<NMTrConfirmFilter.dueDateTo>>,
                               Or<Current2<NMTrConfirmFilter.dueDateTo>, IsNull>>,
                         And2<Where<
                               ViewTable.usrAccConfirmNbr, GreaterEqual<Current2<NMTrConfirmFilter.accConfirmNbrFrom>>,
                               Or<Current2<NMTrConfirmFilter.accConfirmNbrFrom>, IsNull>>,
                         And2<Where<
                               ViewTable.usrAccConfirmNbr, LessEqual<Current2<NMTrConfirmFilter.accConfirmNbrTo>>,
                               Or<Current2<NMTrConfirmFilter.accConfirmNbrTo>, IsNull>>,
                        And2<Where<
                              KGBillPaymentExt.usrTrConfirmBy, Equal<Current2<NMTrConfirmFilter.trConfirmBy>>,
                              Or<Current2<NMTrConfirmFilter.trConfirmBy>, IsNull>>,
                        And<Where2<
                                Where<
                                  Current2<NMTrConfirmFilter.status>, Equal<NMTrConfirmFilter.int_0>,
                                  And<Where<KGBillPaymentExt.usrIsTrConfirm, IsNull,Or<KGBillPaymentExt.usrIsTrConfirm,Equal<False>>>>>,
                                Or<
                                 Where<
                                  Current2<NMTrConfirmFilter.status>, Equal<NMTrConfirmFilter.int_1>,
                                  And<KGBillPaymentExt.usrIsTrConfirm, Equal<True>>>>
                                    >
                        >>>>>>>>>>,
            OrderBy<Desc<ViewTable.usrIsAlertVendor>>> Details;
        #endregion

        #region Actions

        #region 確認分類
        public PXAction<NMTrConfirmFilter> ConfirmClassAction;
        [PXButton(CommitChanges = true, Tooltip = "確認資金付款分類")]
        [PXUIField(DisplayName = "確認分類")]
        protected IEnumerable confirmClassAction(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, ConfirmClass);
            return adapter.Get();
        }

        #endregion

        #region 取消分類
        public PXAction<NMTrConfirmFilter> CancelClassAction;
        [PXButton(CommitChanges = true, Tooltip = "取消資金付款分類")]
        [PXUIField(DisplayName = "取消分類")]
        protected IEnumerable cancelClassAction(PXAdapter adapter)
        {
            NMTrConfirmFilter filter = Filters.Current;
            PXLongOperation.StartOperation(this, CancelClass);
            return adapter.Get();
        }


        #endregion

        //2021/07/21 Add Mantis: 0012165
        #region 付款確認
        public PXAction<NMTrConfirmFilter> TrConfirmClassAction;
        [PXButton(CommitChanges = true, Tooltip = "確認資金付款")]
        [PXUIField(DisplayName = "付款確認")]
        protected IEnumerable trConfirmClassAction(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, TrConfirm);
            return adapter.Get();
        }

        #endregion

        //2021/07/21 Add Mantis: 0012165a
        #region 取消付款確認
        public PXAction<NMTrConfirmFilter> CancelTrConfirmAction;
        [PXButton(CommitChanges = true, Tooltip = "取消已確認的資金付款")]
        [PXUIField(DisplayName = "取消付款確認")]
        protected IEnumerable cancelTrConfirmAction(PXAdapter adapter)
        {
            NMTrConfirmFilter filter = Filters.Current;
            PXLongOperation.StartOperation(this, CancelTrConfirm);
            return adapter.Get();
        }


        #endregion

        //2021/07/21 Add Mantis : 0012165
        #region 付款異動
        public PXAction<NMTrConfirmFilter> ModifyPaymentAction;
        [PXButton(CommitChanges = true, Tooltip = "異動付款資料")]
        [PXUIField(DisplayName = "付款異動")]
        protected IEnumerable modifyPaymentAction(PXAdapter adapter)
        {
            ///<remarks>Asynchronous threads will refresh the graph to release memory after the job ends, and this mechanism is not needed for the time being.</remarks>
            //PXLongOperation.StartOperation(this, ModifyPayment);
            ModifyPayment();

            return adapter.Get();
        }
        #endregion
        //2022/03/25 Add Mantis : 0012165

        #region 所在地異動
        public PXAction<NMTrConfirmFilter> ModifyLocationAction;
        [PXButton(CommitChanges = true, Tooltip = "異動所在地")]
        [PXUIField(DisplayName = "所在地異動")]
        protected IEnumerable modifyLocationAction(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, ModifyLocation);
            return adapter.Get();
        }
        #endregion

        #region  toggleSource
        public PXAction<NMTrConfirmFilter> ToggleSource;
        [PXButton()]
        [PXUIField(DisplayName = "全選/取消全選", MapEnableRights = PXCacheRights.Select)]
        public IEnumerable toggleSource(PXAdapter adapter)
        {
            bool hasSelected = !Details.View.SelectMulti().ToList().RowCast<ViewTable>().First().ISSelected.GetValueOrDefault();

            foreach (ViewTable detail in Details.View.SelectMulti().RowCast<ViewTable>())
            {
                detail.ISSelected = hasSelected;

                Details.Cache.Update(detail);
            }

            return adapter.Get();
        }
        #endregion

        #endregion

        #region HyperLink
        public PXAction<APInvoice> ViewGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGL()
        {
            ViewTable row = Details.Current;
            APRegister register = GetAPRegister(row.RefNbr);
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
            string BatchNbr = "";
            if (register.DocType == APDocType.DebitAdj)
                BatchNbr = registerFinExt.UsrAccConfirmNbr.Remove(12);
            else
                BatchNbr = registerFinExt.UsrAccConfirmNbr;
            Batch batch = PXSelect<Batch,
                Where<BatchExt.usrAccConfirmNbr, Equal<Required<APRegisterFinExt.usrAccConfirmNbrForShow>>>>
                .Select(this, BatchNbr);
            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
            graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(batch.BatchNbr);
            if (graph.BatchModule.Current == null) return;
            new HyperLinkUtil<JournalEntry>(graph.BatchModule.Current, true);
        }
        #endregion

        #region Event Handlers   
        protected void _(Events.RowSelected<NMTrConfirmFilter> e)
        {
            NMTrConfirmFilter row = e.Row as NMTrConfirmFilter;
            if (row == null) return;
            setDetailsUIEnabled();
            ViewTable viewTable = getSelectedViewTable();
            if (viewTable == null)
            {
                row.TTCount = row.CashCount = row.CheckCount = row.GiftCount = row.AuthCount = 0;
                row.TTCountTotalAmt = row.GiftCountTotalAmt = row.CheckCountTotalAmt = row.CashCountTotalAmt = row.AuthCountTotalAmt = 0m;
            }
        }

        /*
        protected void _(Events.RowSelected<ViewTable> e)
        {
            ViewTable row = e.Row ;
            if (row == null) return;
            setDetailsUIEnabled();
        }
        */

        protected void _(Events.FieldDefaulting<ViewTable.checkDueType> e)
        {
            ViewTable row = e.Row as ViewTable;
            if (row == null) return;
            if (row.PaymentPeriod == 0)
                e.NewValue = 1;
            else
                e.NewValue = 0;

            //塞default
            foreach(VendorPaymentMethodDetail details in 
                GetVendorPayment(row.VPaymentMethodID,row.LocationID))
            {
                switch(details.DetailID)
                {
                    case NMStringList.NMTrDetailsID.BankAcct:
                        row.BankAcct = details.DetailValue;
                        break;
                    case NMStringList.NMTrDetailsID.BankName:
                        row.BankName = details.DetailValue;
                        break;
                    case NMStringList.NMTrDetailsID.BankNbr:
                        row.BankNbr = details.DetailValue;
                        break;
                    case NMStringList.NMTrDetailsID.BranchName:
                        row.BranchName = details.DetailValue;
                        break;
                    case NMStringList.NMTrDetailsID.BranchNbr:
                        row.BranchNbr = details.DetailValue;
                        break;
                    case NMStringList.NMTrDetailsID.Category:
                        row.Category = details.DetailValue;
                        break;
                    case NMStringList.NMTrDetailsID.CategoryID:
                        row.CategoryID = details.DetailValue;
                        break;
                }
            }
        }
        
        protected void _(Events.FieldUpdated<ViewTable.isSelected> e)
        {
            ViewTable row = e.Row as ViewTable;
            NMTrConfirmFilter filter = Filters.Current;
            if (row == null) return;
            bool selected = (bool)e.NewValue;
            
            if (selected)
            {
                PXUIFieldAttribute.SetReadOnly<ViewTable.locationID>(Details.Cache, null, false);
            }
            else {
                PXUIFieldAttribute.SetReadOnly<ViewTable.locationID>(Details.Cache, null, true);
            }

            //2021/05/20 Eva口頭:金額改抓已扣郵資費的實付金額
            decimal Amt = row.ActPayAmt ?? 0;
            int Count = 1;
            if (!selected)
            {
                Amt = (-1m) * Amt;
                Count = -1;
            }

            switch (row.PaymentMethod)
            {
                case Kedge.DAC.PaymentMethod.A:
                    filter.CheckCount = (filter.CheckCount ?? 0) + Count;
                    filter.CheckCountTotalAmt = (filter.CheckCountTotalAmt ?? 0) + Amt;
                    break;
                case Kedge.DAC.PaymentMethod.B:
                    filter.TTCount = (filter.TTCount ?? 0) + Count;
                    filter.TTCountTotalAmt = (filter.TTCountTotalAmt ?? 0) + Amt;
                    break;
                case Kedge.DAC.PaymentMethod.C:
                    filter.CashCount = (filter.CashCount ?? 0) + Count;
                    filter.CashCountTotalAmt = (filter.CashCountTotalAmt ?? 0) + Amt;
                    break;
                case Kedge.DAC.PaymentMethod.D:
                    filter.GiftCount = (filter.GiftCount ?? 0) + Count;
                    filter.GiftCountTotalAmt = (filter.GiftCountTotalAmt ?? 0) + Amt;
                    break;
                case Kedge.DAC.PaymentMethod.E:
                    filter.AuthCount = (filter.AuthCount ?? 0) + Count;
                    filter.AuthCountTotalAmt = (filter.AuthCountTotalAmt ?? 0) + Amt;
                    break;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 1) 將details設為readonly,只有seleted可以用
        /// 2) details不可新增/刪除
        /// 3) 若篩選的狀態為未確認,取消確認按鈕不可用
        /// </summary>
        private void setDetailsUIEnabled()
        {
            NMTrConfirmFilter filter = Filters.Current;
            //ViewTable row = Details.Current;
            //2021/07/21 狀態改為用UsrIsTrConfirm
            //若狀態為未確認取消按鈕不可用
            //2021/05/31 Tiffany口頭: 狀態為已確認不可再用資料確認按鈕
            if (filter.Status == 1)
            {
                TrConfirmClassAction.SetEnabled(false);
                CancelTrConfirmAction.SetEnabled(true);
            }
            else
            {
                TrConfirmClassAction.SetEnabled(true);
                CancelTrConfirmAction.SetEnabled(false);
            }

            Details.AllowInsert = false;
            Details.AllowDelete = false;
            PXUIFieldAttribute.SetReadOnly(Details.Cache, null, true);
            PXUIFieldAttribute.SetReadOnly<ViewTable.isSelected>(Details.Cache, null, false);
        }

        /// <summary>
        /// 確認分類
        /// Log:
        /// 1)將四個欄位的資料押回KGBillPayment的對應欄位
        ///     1.'付款類型' , 2.'確認批次' , 3.'確認日期' , 4.'確認人員'
        /// </summary>
        private void ConfirmClass()
        {
            NMTrConfirmFilter filter = Filters.Current;
            if (filter.TrPaymentType == null || filter.TrConfirmID == null)
                throw new Exception("請將分類資料填妥!");

            //將確認資料押回對應欄位
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXResultset<ViewTable> SelectedViewTable = getSelectedViewTable();
                foreach (ViewTable viewTable in SelectedViewTable)
                {
                    //因為graph太多雜七雜八 會導致update語法有問題 所以new一個乾淨的
                    PXGraph graph = new PXGraph();
                    PXUpdate<Set<KGBillPaymentExt.usrTrConfirmBy, Required<KGBillPaymentExt.usrTrConfirmBy>,
                        Set<KGBillPaymentExt.usrTrConfirmDate, Required<KGBillPaymentExt.usrTrConfirmDate>,
                        Set<KGBillPaymentExt.usrTrConfirmID, Required<KGBillPaymentExt.usrTrConfirmID>,
                        Set<KGBillPaymentExt.usrTrPaymentType, Required<KGBillPaymentExt.usrTrPaymentType>>>>>,
                        KGBillPayment,
                        Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>
                        , And<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                        >
                        .Update(graph, graph.Accessinfo.UserID, graph.Accessinfo.BusinessDate,
                        filter.TrConfirmID, filter.TrPaymentType, viewTable.RefNbr, viewTable.BillPaymentID);
                }

                ts.Complete();
            }

            //2021/05/31 Tiffany 口頭Add 做完取消確認要把付款類別 清空、付款批次 回到預設1
            filter.TrPaymentType = null;
            filter.TrConfirmID = 1;
            Filters.Update(filter);
        }

        /// <summary>
        /// 取消分類
        /// Log: 
        /// 1)清空KGBillPayment的四個欄位(確認時壓的對應欄位)
        ///     1.'付款類型' , 2.'確認批次' , 3.'確認日期' , 4.'確認人員'
        /// </summary>
        private void CancelClass()
        {
            NMTrConfirmFilter filter = Filters.Current;
            PXResultset<ViewTable> SelectedViewTable = getSelectedViewTable();
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXGraph graph = new PXGraph();
                foreach (ViewTable viewTable in SelectedViewTable)
                {
                    PXUpdate<Set<KGBillPaymentExt.usrTrConfirmBy, Required<KGBillPaymentExt.usrTrConfirmBy>,
                        Set<KGBillPaymentExt.usrTrConfirmDate, Required<KGBillPaymentExt.usrTrConfirmDate>,
                        Set<KGBillPaymentExt.usrTrConfirmID, Required<KGBillPaymentExt.usrTrConfirmID>,
                        Set<KGBillPaymentExt.usrTrPaymentType, Required<KGBillPaymentExt.usrTrPaymentType>>>>>,
                        KGBillPayment,
                        Where<KGBillPayment.billPaymentID, Equal<Required<ViewTable.billPaymentID>>>>
                        .Update(graph, null, null, null, null, viewTable.BillPaymentID);
                }
                ts.Complete();
            }

            //2021/05/19 Eva 口頭Add 做完取消確認要把付款類別 清空、付款批次 回到預設1
            filter.TrPaymentType = null;
            filter.TrConfirmID = 1;
            Filters.Update(filter);

        }

        /// <summary>
        /// 檢查是否已經付款
        /// </summary>
        /// <param name="BillPaymentID"></param>
        /// <returns>
        /// True : 已經付款
        /// False: 尚未付款
        /// </returns>
        private bool CheckisnotCashed(int? BillPaymentID)
        {
            KGBillPayment billPayment = GetBillPayment(BillPaymentID);
            KGBillPaymentExt billPaymentExt = PXCache<KGBillPayment>.GetExtension<KGBillPaymentExt>(billPayment);
            switch (billPayment.PaymentMethod)
            {
                default:
                    return false;
                case Kedge.DAC.PaymentMethod.B:
                    NMApTeleTransLog log = GetTransLog(BillPaymentID);
                    if (log != null)
                        return true;
                    else
                        return false;
                case Kedge.DAC.PaymentMethod.A:
                case Kedge.DAC.PaymentMethod.C:
                case Kedge.DAC.PaymentMethod.D:
                case Kedge.DAC.PaymentMethod.E:
                    if (billPaymentExt.UsrIsCheckIssued ?? false)
                        return true;
                    else
                        return false;
            }
        }

        /// <summary>
        /// 付款確認
        /// Log:
        /// 將KGBillPayment.UsrIsTrConfirm = True
        /// </summary>
        private void TrConfirm()
        {
            NMTrConfirmFilter filter = Filters.Current;

            //將KGBIllPayment.UsrIsConfirm = True
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXResultset<ViewTable> SelectedViewTable = getSelectedViewTable();
                foreach (ViewTable viewTable in SelectedViewTable)
                {
                    //因為graph太多雜七雜八 會導致update語法有問題 所以new一個乾淨的
                    PXGraph graph = new PXGraph();
                    PXUpdate<
                        Set<KGBillPaymentExt.usrIsTrConfirm, True>,
                        KGBillPayment,
                        Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>,
                        And<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>>
                        .Update(graph, viewTable.RefNbr, viewTable.BillPaymentID);
                }

                ts.Complete();
            }

            filter.PaymentMethod = null;
            filter.BankAccountID = null;
            Filters.Update(filter);
        }

        /// <summary>
        /// 取消付款確認
        /// Log:
        /// 1) 檢查所選的KGBillPayment是否已經完成付款,若已完成付款,則跳錯誤
        /// 2) 將KGBillPayment.UsrIsTrConfirm = False
        /// </summary>
        private void CancelTrConfirm()
        {
            PXResultset<ViewTable> SelectedViewTable = getSelectedViewTable();

            //先檢查
            List<string> checkCashed = new List<string>();
            foreach (ViewTable checktable in SelectedViewTable)
            {
                //Check is not Cashed
                if (CheckisnotCashed(checktable.BillPaymentID))
                {
                    checkCashed.Add(checktable.RefNbr);
                }
            }
            if (checkCashed.Count != 0)
            {
                string result = string.Join(",", checkCashed);
                throw new Exception("以下計價單已經完成付款!" + result);
            }

            //將KGBIllPayment.UsrIsConfirm = True
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (ViewTable viewTable in SelectedViewTable)
                {
                    //因為graph太多雜七雜八 會導致update語法有問題 所以new一個乾淨的
                    PXGraph graph = new PXGraph();
                    PXUpdate<
                        Set<KGBillPaymentExt.usrIsTrConfirm, False>,
                        KGBillPayment,
                        Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>,
                        And<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>>
                        .Update(graph, viewTable.RefNbr, viewTable.BillPaymentID);
                }

                ts.Complete();
            }
        }

        /// <summary>
        /// 異動付款資料
        /// Log:
        /// 將Header的PaymentMethod &BankAccountID 無條件壓掉對應欄位
        /// 塞對應的DefLocation 到KGBillPayment.VendorLocationID 
        /// </summary>
        private void ModifyPayment()
        {
            NMTrConfirmFilter header = Filters.Current;

            if (header == null|| header.PaymentMethod == null || header.BankAccountID == null)
                throw new Exception("請將異動資料填寫完整!");

            //將KGBIllPayment.PaymentMethod & BankAccountID壓掉
            //using (PXTransactionScope ts = new PXTransactionScope())
            //{
                PXResultset<ViewTable> SelectedViewTable = getSelectedViewTable();

                foreach (ViewTable viewTable in SelectedViewTable)
                {
                    //2021/07/27 Mantis: 0012165 LocationID 改為用NMBankAccountID的付款方法對應供應商的付款方法找出的LocationID
                    //20220329 modify by louis LocationID預設用KGBillpayment付款方法去串該廠商對應的LocationID
                    #region 用付款方法的預設LocationID 

                    //int? VendorLocationID;
                    BAccount bAccount = GetBAccount(viewTable.VendorID);
                    //if (header.PaymentMethod == PaymentMethod.C || header.PaymentMethod == PaymentMethod.E)
                    //{
                    //Location location = GetLocation(bAccount.DefLocationID);
                    //VendorLocationID = location.LocationID;
                    //}
                    //else
                    //{
                    int? VendorLocationID = NMLocationUtil.GetDefLocationByPaymentMethod(viewTable.VendorID, header.PaymentMethod);
                    //?? throw new Exception(String.Format("此供應商({0})沒有對應付款方式的所在地!", bAccount.AcctCD));

                    // Added and modifed the operation according to Mantis [0012301]
                    if (VendorLocationID == null)
                    {
                        Details.Cache.RaiseExceptionHandling<ViewTable.locationID>(viewTable, viewTable.LocationID, new PXSetPropertyException($"此供應商({bAccount.AcctCD})沒有對應付款方式的所在地!", PXErrorLevel.RowError));
                    }
                    //}
                    #endregion
                    else
                    {
                        ///<remarks> 
                        ///Because [ViewTable] is an projection unbound DAC is also Readonly, 
                        ///the original update statement which updates the data to DB directly, so adding the cache SetValue makes the UI display the same as DB.
                        ///</remarks>
                        Details.Cache.SetValue<KGBillPayment.paymentMethod>(viewTable, header.PaymentMethod);
                        Details.Cache.SetValue<KGBillPayment.bankAccountID>(viewTable, header.BankAccountID);
                        Details.Cache.SetValue<KGBillPayment.vendorLocationID>(viewTable, VendorLocationID);

                        PXUpdate<Set<KGBillPayment.paymentMethod, Required<KGBillPayment.paymentMethod>,
                                     Set<KGBillPayment.bankAccountID, Required<KGBillPayment.bankAccountID>,
                                         Set<KGBillPayment.vendorLocationID, Required<KGBillPayment.vendorLocationID>>>>,
                                 KGBillPayment,
                                 Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>,
                                       And<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>>
                                         // 因為graph太多雜七雜八 會導致update語法有問題 所以new一個乾淨的
                                 .Update(new PXGraph(), header.PaymentMethod, header.BankAccountID, VendorLocationID, viewTable.RefNbr, viewTable.BillPaymentID);

                        Details.Cache.RaiseExceptionHandling<ViewTable.locationID>(viewTable, viewTable.LocationID, new PXSetPropertyException("Updated successfully.", PXErrorLevel.RowInfo));
                    }
                }
            //  ts.Complete();
            //}
        }

        /// <summary>
        /// 異動所在地資料
        /// Log:
        /// 
        /// 允許資金修改付款供應商所在地 
        /// </summary>
        private void ModifyLocation()
        {
            NMTrConfirmFilter header = Filters.Current;
            

            //將KGBIllPayment.VendorLocationID壓掉
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXResultset<ViewTable> SelectedViewTable = getSelectedViewTable();
                foreach (ViewTable viewTable in SelectedViewTable)
                {

                    PXGraph graph = new PXGraph();
                    PXUpdate<
                        Set<KGBillPayment.vendorLocationID, Required<KGBillPayment.vendorLocationID>>,
                        KGBillPayment,
                        Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>,
                        And<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>>
                        .Update(graph, viewTable.LocationID, viewTable.RefNbr, viewTable.BillPaymentID);
                }

                ts.Complete();
            }

        }
        #endregion

        #region Select Method
        private PXResultset<ViewTable> getSelectedViewTable()
        {
            return PXSelect<ViewTable, Where<ViewTable.isSelected, Equal<True>>>
                .Select(this);
        }
        private KGBillPayment GetBillPayment(int? BillPaymentID)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(this, BillPaymentID);
        }
        private NMApTeleTransLog GetTransLog(int? BillPaymentID)
        {
            return PXSelect<NMApTeleTransLog,
                Where<NMApTeleTransLog.isNew, Equal<True>,
                And<NMApTeleTransLog.status, Equal<NMApTtLogStatus.feedBackSuccess>,
                And<NMApTeleTransLog.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>>>
                .Select(this, BillPaymentID);
        }
        private Location GetLocation(int? VendorLocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(this, VendorLocationID);
        }
        private BAccount GetBAccount(int? BAccount)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(this, BAccount);
        }
        private Location GetNMBAVendorLocation(int? BankAccountID, int? VendorID)
        {
            return PXSelectJoin<Location,
                InnerJoin<NMBankAccount, On<NMBankAccount.paymentMethodID, Equal<Location.vPaymentMethodID>>>,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>,
                And<Location.bAccountID, Equal<Required<Location.bAccountID>>>>,
                OrderBy<Desc<LocationFinExt.usrIsReserveAcct>>>
                .Select(this, BankAccountID, VendorID);
        }
        private APRegister GetAPRegister(string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>.Select(this, RefNbr);
        }
        private PXResultset<VendorPaymentMethodDetail> GetVendorPayment(string vPMID, int? locationID)
        {

            return PXSelect<VendorPaymentMethodDetail,
            Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Required<ViewTable.vPaymentMethodID>>,
            And<VendorPaymentMethodDetail.locationID, Equal<Required<ViewTable.locationID>>>>>
            .Select(this, vPMID, locationID);
        }
        #endregion

        #region View Table
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<KGBillPayment, InnerJoin<APRegister, On<APRegister.refNbr, Equal<KGBillPayment.refNbr>,
                                                              And<APRegisterFinExt.usrIsConfirm, Equal<True>>>,
                                                    InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<KGBillPayment.refNbr>>,
                                                    LeftJoin<Location, On<Location.locationID,Equal<KGBillPayment.vendorLocationID>,
                                                                          And<Location.bAccountID,Equal<KGBillPayment.vendorID>>>,
                                                    //add 2021-06-09 :12139
                                                    LeftJoin<APInvoiceDebit, On<APInvoiceDebit.origRefNbr, Equal<APRegister.refNbr>,
                                                                                And<APInvoiceDebit.origDocType, Equal<APRegister.docType>>>,
                                                    //2021/08/04 Add Mantis: 0012182
                                                    LeftJoin<Vendor, On<Vendor.bAccountID,Equal<KGBillPayment.vendorID>>,
                                                    LeftJoin<Address, On<Address.addressID, Equal<Location.defAddressID>>,
                                                    LeftJoin<NMBankAccount, On<NMBankAccount.bankAccountID, Equal<KGBillPayment.bankAccountID>>>>>>>>>,
            Where<
                //2021/05/17 Tiffany口頭 Add
                //2021/07/23 Tiffany Takeoff
                //APInvoice.batchNbr, IsNotNull,
                APRegister.status,Equal<APDocStatus.open>,//add 2021-06-09 :12139
                And<APInvoiceDebit.refNbr, IsNull,
                //2022/07/03 Louis 新增暫時不付款條件
                //And<KGBillPayment.isPaymentHold, NotEqual<True>,
                //And<Where2<KGBillPayment.isPaymentHold, IsNull, Or<KGBillPayment.isPaymentHold, Equal<True>>>>,
                //2021/05/20 Eva口頭 Add
                And2<Where<KGBillPaymentExt.usrIsCheckIssued, IsNull, Or<KGBillPaymentExt.usrIsCheckIssued, Equal<False>>>,
                And<Where<KGBillPaymentExt.isPaymentHold, IsNull, Or<KGBillPaymentExt.isPaymentHold, Equal<False>>>>>
                    >>,//add 2021-06-09 :12139
            
            OrderBy<
                //2021/08/04 Add Mantis: 0012182
                Desc<VendorExt.usrIsAlertVendor,
                     Desc<KGBillPayment.refNbr>>>>), Persistent = false)]
        public class ViewTable : KGBillPayment
        {
            #region ISSelected
            [PXDBBool(BqlField = typeof(APRegister.selected))]
            [PXUIField(DisplayName = "ISSelected")]
            public virtual bool? ISSelected { get; set; }
            public abstract class isSelected : PX.Data.BQL.BqlBool.Field<isSelected> { }
            #endregion

            //APInvoice Shows
            #region UsrAccConfirmNbr
            [PXDBString(BqlField = typeof(APRegisterFinExt.usrAccConfirmNbr))]
            [PXUIField(DisplayName = "UsrAccConfirmNbr")]
            public virtual string UsrAccConfirmNbr { get; set; }
            public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
            #endregion        

            #region DueDate
            [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
            [PXUIField(DisplayName = "Due Date")]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : IBqlField { }
            #endregion

            #region ProjectID
            [PXDBInt(BqlField = typeof(APInvoice.projectID))]
            [PXUIField(DisplayName = "ProjectID")]
            [PXSelector(typeof(Search<PMProject.contractID>),
                typeof(PMProject.contractCD),
                typeof(PMProject.description),
                SubstituteKey = typeof(PMProject.contractCD),
                DescriptionField = typeof(PMProject.description))]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : IBqlField { }
            #endregion

            #region BranchID
            [PXDBInt(BqlField = typeof(APInvoice.branchID))]
            [PXUIField(DisplayName = "BranchID")]
            [PXSelector(typeof(Search<Branch.branchID>),
                typeof(Branch.branchCD),
                typeof(Branch.acctName),
                SubstituteKey = typeof(Branch.branchCD),
                DescriptionField = typeof(Branch.acctName))]

            public virtual int? BranchID { get; set; }
            public abstract class branchID : IBqlField { }
            #endregion

            #region PayDate
            [PXDBDate(BqlField = typeof(APInvoice.payDate))]
            [PXUIField(DisplayName = "Pay Date")]
            public virtual DateTime? PayDate { get; set; }
            public abstract class payDate : IBqlField { }
            #endregion

            //VendorLocation Shows
            //2021/06/01 Mantis: 0012070 Add 
            //AcctName/BankAcct/ BankName / BankNbr/BranchName/ BranchNbr
            #region LocationID
            [PXDBInt(BqlField = typeof(Location.locationID))]
            [PXUIField(DisplayName = "LocationID")]
            [PXSelector(typeof(Search2<Location.locationID,
            InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Location.bAccountID>>>,
            Where<Location.bAccountID, Equal<Current<KGBillPayment.vendorID>>,
                And<Location.isActive,Equal<True>,
                And<Where2<
                    Where<Current<paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                        And<Location.vPaymentMethodID, Equal<word.tt>>>,
                    Or<Where<Current<paymentMethod>, Equal<PaymentMethod.check>,
                        And<Location.vPaymentMethodID, Equal<word.check>>>>
                    >>
                >>>),
            typeof(Location.locationCD),
            typeof(Location.descr),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(Location.locationCD))]
            public virtual int? LocationID { get; set; }
            public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
            #endregion

            #region vPaymentMethodID
            [PXDBString(BqlField = typeof(Location.vPaymentMethodID))]
            [PXUIField(DisplayName = "vPaymentMethodID")]
            public virtual string VPaymentMethodID { get; set; }
            public abstract class vPaymentMethodID : PX.Data.BQL.BqlString.Field<vPaymentMethodID> { }
            #endregion

            #region LocationCD
            [PXDBString(BqlField = typeof(Location.locationCD))]
            [PXUIField(DisplayName = "LocationCD")]               
            public virtual string LocationCD { get; set; }
            public abstract class locationCD : PX.Data.BQL.BqlString.Field<locationCD> { }
            #endregion

            #region LocationDescr
            [PXDBString(BqlField = typeof(Location.descr))]
            [PXUIField(DisplayName = "Location Descr")]                    
            public virtual string LocationDescr { get; set; }
            public abstract class locationDescr : PX.Data.BQL.BqlString.Field<locationDescr> { }
            #endregion

            #region CheckDueType
            [PXInt()]
            [PXUIField(DisplayName = "CheckDueType")]
            [PXUnboundDefault()]
            [PXIntList(new int[] { 0, 1 }, new string[] { "遠期", "即期" })]
            public virtual int? CheckDueType { get; set; }
            public abstract class checkDueType : PX.Data.BQL.BqlInt.Field<checkDueType> { }
            #endregion

            #region AcctName
            [PXString()]
            [PXUIField(DisplayName = "AcctName")]         
            [PXUnboundDefault(typeof(Search2<VendorPaymentMethodDetail.detailValue,
                InnerJoin<PX.Objects.CA.PaymentMethodDetail,
                    On<PX.Objects.CA.PaymentMethodDetail.paymentMethodID, Equal<VendorPaymentMethodDetail.paymentMethodID>,
                        And<PX.Objects.CA.PaymentMethodDetail.detailID, Equal<VendorPaymentMethodDetail.detailID>>>>,
                Where<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.acctname>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<ViewTable.vendorLocationID>>,
                        And<VendorPaymentMethodDetail.bAccountID, Equal<Current<ViewTable.vendorID>>>>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            
            public virtual string AcctName { get; set; }
            public abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }
            #endregion

            #region BankAcct
            [PXString()]
            [PXUIField(DisplayName = "BankAcct")]
            /*
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                    Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Current2<ViewTable.vPaymentMethodID>>,
                        And<VendorPaymentMethodDetail.locationID, Equal<Current2<ViewTable.locationID>>,
                        And<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.bankacct>
                            >>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            */
            public virtual string BankAcct { get; set; }
            public abstract class bankAcct : PX.Data.BQL.BqlString.Field<bankAcct> { }
            #endregion

            #region BankName
            [PXString()]
            [PXUIField(DisplayName = "BankName")]
            /*
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                    Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Current2<ViewTable.vPaymentMethodID>>,
                        And<VendorPaymentMethodDetail.locationID, Equal<Current2<ViewTable.locationID>>,
                        And<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.bankname>
                            >>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            */
            public virtual string BankName { get; set; }
            public abstract class bankName : PX.Data.BQL.BqlString.Field<bankName> { }
            #endregion

            #region BankNbr
            [PXString()]
            [PXUIField(DisplayName = "BankNbr")]
            /*
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                    Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Current2<ViewTable.vPaymentMethodID>>,
                        And<VendorPaymentMethodDetail.locationID, Equal<Current2<ViewTable.locationID>>,
                        And<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.banknbr>
                            >>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            */
            public virtual string BankNbr { get; set; }
            public abstract class bankNbr : PX.Data.BQL.BqlString.Field<bankNbr> { }
            #endregion

            #region BranchName
            [PXString()]
            [PXUIField(DisplayName = "BranchName")]
            /*
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                    Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Current2<ViewTable.vPaymentMethodID>>,
                        And<VendorPaymentMethodDetail.locationID, Equal<Current2<ViewTable.locationID>>,
                        And<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.branchname>
                            >>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            */
            public virtual string BranchName { get; set; }
            public abstract class branchName : PX.Data.BQL.BqlString.Field<branchName> { }
            #endregion

            #region BranchNbr
            [PXString()]
            [PXUIField(DisplayName = "BranchNbr")]
            /*
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                    Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Current2<ViewTable.vPaymentMethodID>>,
                        And<VendorPaymentMethodDetail.locationID, Equal<Current2<ViewTable.locationID>>,
                        And<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.branchnbr>
                            >>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            */
            public virtual string BranchNbr { get; set; }
            public abstract class branchNbr : PX.Data.BQL.BqlString.Field<branchNbr> { }
            #endregion

            #region CategoryID
            [PXString()]
            [PXUIField(DisplayName = "CategoryID")]
            /*
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                    Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Current2<ViewTable.vPaymentMethodID>>,
                        And<VendorPaymentMethodDetail.locationID, Equal<Current2<ViewTable.locationID>>,
                        And<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.categoryID>
                            >>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            */
            public virtual string CategoryID { get; set; }
            public abstract class categoryID : PX.Data.BQL.BqlString.Field<categoryID> { }
            #endregion

            #region Category
            [PXString()]
            [PXUIField(DisplayName = "Category")]
            /*
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                    Where<VendorPaymentMethodDetail.paymentMethodID, Equal<Current2<ViewTable.vPaymentMethodID>>,
                        And<VendorPaymentMethodDetail.locationID, Equal<Current2<ViewTable.locationID>>,
                        And<VendorPaymentMethodDetail.detailID, Equal<NMStringList.NMTrDetailsID.category>
                            >>>>),
                PersistingCheck = PXPersistingCheck.Nothing)]
            */
            public virtual string Category { get; set; }
            public abstract class category : PX.Data.BQL.BqlString.Field<category> { }
            #endregion

            //Address Shows
            //2021/06/01 Mantis: 0012070 Add PostalCode
            #region PostalCode
            [PXDBString(BqlField =typeof(Address.postalCode))]
            [PXUIField(DisplayName = "PostalCode")]
         
            public virtual string PostalCode { get; set; }
            public abstract class postalCode : PX.Data.BQL.BqlString.Field<postalCode> { }
            #endregion

            #region addrLine1
            [PXDBString(BqlField = typeof(Address.addressLine1))]
            [PXUIField(DisplayName = "AddrLine1")]
            /*
            [PXUnboundDefault(typeof(Search2<Address.addressLine1,
                InnerJoin<Location, On<Location.defAddressID, Equal<Address.addressID>>>,
                Where<Location.locationID, Equal<Current<ViewTable.vendorLocationID>>>>))]
            */
            public virtual string AddrLine1 { get; set; }
            public abstract class addrLine1 : PX.Data.BQL.BqlString.Field<addrLine1> { }
            #endregion

            //NMBankAccount Shows     
            #region BankShortName
            [PXDBString(BqlField = typeof(NMBankAccount.bankShortName))]
            [PXUIField(DisplayName = "BankShortName")]
            /*
            [PXUnboundDefault(typeof(Search<NMBankAccount.bankShortName,
                Where<NMBankAccount.bankAccountID, Equal<Current<ViewTable.bankAccountID>>>>))]
            */
            public virtual string BankShortName { get; set; }
            public abstract class bankShortName : PX.Data.BQL.BqlString.Field<bankShortName> { }
            #endregion

            #region BankAccountCD
            [PXDBString(BqlField =typeof(NMBankAccount.bankAccountCD))]
            [PXUIField(DisplayName = "BankAccountCD")]
            /*
            [PXUnboundDefault(typeof(Search<NMBankAccount.bankAccountCD,
                Where<NMBankAccount.bankAccountID, Equal<Current<ViewTable.bankAccountID>>>>))]
          */
            public virtual string BankAccountCD { get; set; }
            public abstract class bankAccountCD : PX.Data.BQL.BqlString.Field<bankAccountCD> { }
            #endregion

            //2021/05/20 Eva口頭: Add 
            #region EnableIssueByBank
            [PXDBBool(BqlField = typeof(NMBankAccount.enableIssueByBank))]
            /*
            [PXUnboundDefault(typeof(Search<NMBankAccount.enableIssueByBank,
                Where<NMBankAccount.bankAccountID, Equal<Current<ViewTable.bankAccountID>>>>))]
            */
            [PXUIField(DisplayName = "Enable Issue By Bank")]
            public virtual bool? EnableIssueByBank { get; set; }
            public abstract class enableIssueByBank : PX.Data.BQL.BqlBool.Field<enableIssueByBank> { }
            #endregion

            //2021/08/04 Add Mantis: 0012182
            #region UsrIsAlertVendor
            [PXDBBool(BqlField = typeof(VendorExt.usrIsAlertVendor))]
            /*
            [PXUnboundDefault(typeof(Search<VendorExt.usrIsAlertVendor,
                Where<Vendor.bAccountID, Equal<Current<ViewTable.vendorID>>>>))]
            */
            [PXUIField(DisplayName = "usrIsAlertVendor")]
            public virtual bool? UsrIsAlertVendor { get; set; }
            public abstract class usrIsAlertVendor : PX.Data.BQL.BqlBool.Field<usrIsAlertVendor> { }
            #endregion

            // Added an overide parent field and modified the existing properties.
            #region BankAccountID
            [PXDBInt(BqlField = typeof(NMBankAccount.bankAccountID))]
            [PXUIField(DisplayName = "Bank Account", Visible = true)]
            [PXSelector(typeof(Search<NMBankAccount.bankAccountID>), SubstituteKey = typeof(NMBankAccount.bankAccountCD))]
            public override int? BankAccountID { get; set; }
            public new abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion
        }
        #endregion

        #region Unbound DACs
        [Serializable]
        public class NMTrConfirmFilter : IBqlTable
        {
            //Filter 
            #region DueDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Due Date From")]
            //[PXUnboundDefault(typeof(AccessInfo.businessDate),PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual DateTime? DueDateFrom { get; set; }
            public abstract class dueDateFrom : IBqlField { }
            #endregion

            #region DueDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Due Date To")]
            [PXUnboundDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual DateTime? DueDateTo { get; set; }
            public abstract class dueDateTo : IBqlField { }
            #endregion

            #region PayDate
            [PXDate()]
            [PXUIField(DisplayName = "Pay Date")]
            public virtual DateTime? PayDate { get; set; }
            public abstract class payDate : IBqlField { }
            #endregion

            #region ConfirmDate
            [PXDate()]
            [PXUIField(DisplayName = "Confirm Date")]
            public virtual DateTime? ConfirmDate { get; set; }
            public abstract class confirmDate : IBqlField { }
            #endregion

            #region ContractID
            [PXInt()]
            [PXUIField(DisplayName = "Project")]
            [PXSelector(typeof(Search2<PMProject.contractID,
                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                    Equal<PMProject.contractID>>>>,
                    Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                    , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
            public virtual int? ContractID { get; set; }
            public abstract class contractID : IBqlField { }
            #endregion

            #region AccConfirmNbrFrom
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "AccConfirmNbr From")]

            public virtual string AccConfirmNbrFrom { get; set; }
            public abstract class accConfirmNbrFrom : IBqlField { }
            #endregion

            #region AccConfirmNbrTo
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "AccConfirmNbr To")]

            public virtual string AccConfirmNbrTo { get; set; }
            public abstract class accConfirmNbrTo : IBqlField { }
            #endregion

            #region Vendor
            [PXInt()]
            [PXUIField(DisplayName = "Vendor")]
            [POVendor(Visibility = PXUIVisibility.SelectorVisible,
                DescriptionField = typeof(Vendor.acctName),
                CacheGlobal = true, Filterable = true)]
            public virtual int? Vendor { get; set; }
            public abstract class vendor : IBqlField { }
            #endregion

            #region Status
            [PXInt]
            [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
            [PXUIField(DisplayName = "Status")]
            [PXIntList(new int[] { 0, 1 }, new string[] { "未確認", "已確認" })]
            public virtual int? Status { get; set; }
            public abstract class status : IBqlField { }
            #endregion

            #region Const
            public const int Int_0 = 0;
            public const int Int_1 = 1;
            public class int_0 : PX.Data.BQL.BqlInt.Constant<int_0> { public int_0() : base(Int_0) {; } }
            public class int_1 : PX.Data.BQL.BqlInt.Constant<int_1> { public int_1() : base(Int_1) {; } }
            #endregion

            #region TrConfirmBy
            [PXGuid]
            [PXUIField(DisplayName = "TrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? TrConfirmBy { get; set; }
            public abstract class trConfirmBy : PX.Data.BQL.BqlGuid.Field<trConfirmBy> { }
            #endregion

            //Insert Data
            #region TrPaymentType
            [PXString(1)]
            [PXUIField(DisplayName = "PaymentType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string TrPaymentType { get; set; }
            public abstract class trPaymentType : PX.Data.BQL.BqlString.Field<trPaymentType> { }
            #endregion

            #region TrConfirmID
            [PXInt()]
            [PXUIField(DisplayName = "TrConfirmID")]
            /*[PXUnboundDefault(1)]
            */
            public virtual int? TrConfirmID { get; set; }
            public abstract class trConfirmID : PX.Data.BQL.BqlInt.Field<trConfirmID> { }
            #endregion

            //Count Data
            #region TTCount
            [PXInt()]
            [PXUIField(DisplayName = "TTCount", IsReadOnly = true)]
            public virtual int? TTCount { get; set; }
            public abstract class ttCount : PX.Data.BQL.BqlInt.Field<ttCount> { }
            #endregion

            #region CashCount
            [PXInt()]
            [PXUIField(DisplayName = "CashCount", IsReadOnly = true)]
            public virtual int? CashCount { get; set; }
            public abstract class cashCount : PX.Data.BQL.BqlInt.Field<cashCount> { }
            #endregion

            #region CheckCount
            [PXInt()]
            [PXUIField(DisplayName = "CheckCount", IsReadOnly = true)]
            public virtual int? CheckCount { get; set; }
            public abstract class checkCount : PX.Data.BQL.BqlInt.Field<checkCount> { }
            #endregion

            #region GiftCount
            [PXInt()]
            [PXUIField(DisplayName = "GiftCount", IsReadOnly = true)]
            public virtual int? GiftCount { get; set; }
            public abstract class giftCount : PX.Data.BQL.BqlInt.Field<giftCount> { }
            #endregion

            #region AuthCount
            [PXInt()]
            [PXUIField(DisplayName = "AuthCount", IsReadOnly = true)]
            public virtual int? AuthCount { get; set; }
            public abstract class authCount : PX.Data.BQL.BqlInt.Field<authCount> { }
            #endregion

            #region TTCountTotalAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "TTCountTotalAmt", IsReadOnly = true)]
            public virtual decimal? TTCountTotalAmt { get; set; }
            public abstract class ttCountTotalAmt : PX.Data.BQL.BqlDecimal.Field<ttCountTotalAmt> { }
            #endregion

            #region CashCountTotalAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "CashCountTotalAmt", IsReadOnly = true)]
            public virtual decimal? CashCountTotalAmt { get; set; }
            public abstract class cashCountTotalAmt : PX.Data.BQL.BqlDecimal.Field<cashCountTotalAmt> { }
            #endregion

            #region CheckCountTotalAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "CheckCountTotalAmt", IsReadOnly = true)]
            public virtual decimal? CheckCountTotalAmt { get; set; }
            public abstract class checkCountTotalAmt : PX.Data.BQL.BqlDecimal.Field<checkCountTotalAmt> { }
            #endregion

            #region GiftCountTotalAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "GiftCountTotalAmt", IsReadOnly = true)]
            public virtual decimal? GiftCountTotalAmt { get; set; }
            public abstract class giftCountTotalAmt : PX.Data.BQL.BqlDecimal.Field<giftCountTotalAmt> { }
            #endregion

            #region AuthCountTotalAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "AuthCountTotalAmt", IsReadOnly = true)]
            public virtual decimal? AuthCountTotalAmt { get; set; }
            public abstract class authCountTotalAmt : PX.Data.BQL.BqlDecimal.Field<authCountTotalAmt> { }
            #endregion

            //KGBillPayment
            #region PaymentMethod
            [PXString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Payment Method")]
            [Kedge.DAC.PaymentMethod.List]
            public virtual string PaymentMethod { get; set; }
            public abstract class paymentMethod : PX.Data.BQL.BqlString.Field<paymentMethod> { }
            #endregion

            #region BankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Bank Account CD")]
            [PXSelector(typeof(Search2<NMBankAccount.bankAccountID,
            InnerJoin<PX.Objects.CA.CashAccount, On<PX.Objects.CA.CashAccount.cashAccountID, Equal<NMBankAccount.cashAccountID>>>,
            Where2<
                Where<Current<paymentMethod>, In3<PaymentMethod.check, PaymentMethod.giftCertificate>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.check>>>,
                Or2<Where<Current<paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.tt>>>,
                Or<Where<Current<paymentMethod>,
                    //2021/06/24 Add Auth same with Cash
                    In3<PaymentMethod.cash, PaymentMethod.auth>>>>>>),
            typeof(NMBankAccount.bankAccountCD),
            typeof(PX.Objects.CA.CashAccount.descr),
            typeof(NMBankAccount.bankCode),
            typeof(NMBankAccount.bankName),
            DescriptionField = typeof(NMBankAccount.bankName),
            SubstituteKey = typeof(NMBankAccount.bankAccountCD))]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion
        }

        #region APInvoiceDebit - 2021-06-09 :12139
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