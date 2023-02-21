using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.GL;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.IN;
using System.Runtime;
using PX.Objects.CS;
using RCGV.GV;
using RCGV.GV.DAC;
using PX.Objects.TX;
using PX.Data.BQL;

namespace Kedge
{
    public class KGVoucherProcess : PXGraph<KGVoucherProcess>
    {
        #region Filter

        public PXFilter<VoucherFilter> VoucherFilters;

        public PXSelect<APRegister, Where<APRegister.refNbr,
            Equal<Current<KGVoucherH.refNbr>>>> APRegisters;

        public PXSelectJoin<KGVoucherH,
            InnerJoin<APRegister,
                On<APRegister.docType, Equal<KGVoucherH.docType>,
                    And<APRegister.refNbr, Equal<KGVoucherH.refNbr>>>,
                LeftJoin<APInvoice,
                    On<APInvoice.docType, Equal<KGVoucherH.docType>,
                        And<APInvoice.refNbr, Equal<KGVoucherH.refNbr>>>,
                    LeftJoin<BAccount,
                        On<BAccount.bAccountID, Equal<APRegister.vendorID>>,
                        LeftJoin<KGBillSummary,
                            On<KGBillSummary.docType, Equal<KGVoucherH.docType>,
                                And<KGBillSummary.refNbr, Equal<KGVoucherH.refNbr>>>>>>>> AllVouchers;

        /*public PXSelectJoin<APInvoice,
             LeftJoin<KGVoucherH,
                 On<KGVoucherH.docType, Equal<APInvoice.docType>,
                     And<KGVoucherH.refNbr, Equal<APInvoice.refNbr>>>,
             LeftJoin<KGBillSummary,
                 On<KGBillSummary.docType, Equal<APInvoice.docType>,
                     And<KGBillSummary.refNbr, Equal<APInvoice.refNbr>>>,
             LeftJoin<BAccount,
                 On<BAccount.bAccountID, Equal<APInvoice.vendorID>>>>>,
             Where<APInvoice.status, Equal<VoucherStatuses.balance>,
                 Or<APInvoice.status, Equal<VoucherStatuses.open>>>> AllVouchers;*/

        /*public PXSelect<KGVoucherH> KGVoucherHs;*/

        public PXSelect<KGVoucherL,Where<KGVoucherL.voucherHeaderID,
            Equal<Current<KGVoucherH.voucherHeaderID>>>> KGVoucherLs;

        protected virtual IEnumerable allVouchers()
        {
            PXSelectJoin<KGVoucherH,
            InnerJoin<APRegister,
                On<APRegister.docType, Equal<KGVoucherH.docType>,
                    And<APRegister.refNbr, Equal<KGVoucherH.refNbr>>>,
                LeftJoin<APInvoice,
                    On<APInvoice.docType, Equal<KGVoucherH.docType>,
                        And<APInvoice.refNbr, Equal<KGVoucherH.refNbr>>>,
                    LeftJoin<BAccount,
                        On<BAccount.bAccountID, Equal<APRegister.vendorID>>,
                        LeftJoin<KGBillSummary,
                            On<KGBillSummary.docType, Equal<KGVoucherH.docType>,
                                And<KGBillSummary.refNbr, Equal<KGVoucherH.refNbr>>>>>>>>
            query =
                 new PXSelectJoin<KGVoucherH,
            InnerJoin<APRegister,
                On<APRegister.docType, Equal<KGVoucherH.docType>,
                    And<APRegister.refNbr, Equal<KGVoucherH.refNbr>>>,
                LeftJoin<APInvoice,
                    On<APInvoice.docType, Equal<KGVoucherH.docType>,
                        And<APInvoice.refNbr, Equal<KGVoucherH.refNbr>>>,
                    LeftJoin<BAccount,
                        On<BAccount.bAccountID, Equal<APRegister.vendorID>>,
                        LeftJoin<KGBillSummary,
                            On<KGBillSummary.docType, Equal<KGVoucherH.docType>,
                                And<KGBillSummary.refNbr, Equal<KGVoucherH.refNbr>>>>>>>>(this);


            // Adding filtering conditions to the query
            VoucherFilter filter = VoucherFilters.Current;

            if (filter.ContractID != null)
                query.WhereAnd<Where<KGVoucherH.contractID,
                    Equal<Current<VoucherFilter.contractID>>>>();
       
            if (filter.DocType != null)
                query.WhereAnd<Where<KGVoucherH.docType,
                    Equal<Current<VoucherFilter.docType>>>>();
            if (filter.RefNbr != null)
                query.WhereAnd<Where<KGVoucherH.refNbr,
                    Equal<Current<VoucherFilter.refNbr>>>>();
            if (filter.Vendor != null)
                query.WhereAnd<Where<KGVoucherH.vendorID,
                    Equal<Current<VoucherFilter.vendor>>>>();
            /*if (filter.Status != null)
                query.WhereAnd<Where<APInvoice.status,
                    Equal<Current<VoucherFilter.status>>>>();
            if (filter.DueDateFrom != null)
                query.WhereAnd<Where<APInvoice.dueDate,
                    GreaterEqual<Current<VoucherFilter.dueDateFrom>>>>();
            if (filter.DueDateTo != null)
                query.WhereAnd<Where<APInvoice.dueDate,
                    LessEqual<Current<VoucherFilter.dueDateTo>>>>();*/
            if (filter.PONbr != null)
                query.WhereAnd<Where<KGVoucherH.pONbr,
                    Equal<Current<VoucherFilter.pONbr>>>>();
            /*if (filter.IsVoucher == "0")
                query.WhereAnd<Where<KGVoucherH.refNbr,
                    IsNotNull>>();
            if (filter.IsVoucher == "1")
                query.WhereAnd<Where<KGVoucherH.refNbr,
                    IsNull>>();*/
            if (filter.VoucherDateFrom != null)
                query.WhereAnd<Where<KGVoucherH.voucherDate,
                    GreaterEqual<Current<VoucherFilter.voucherDateFrom>>>>();
            if (filter.VoucherDateTo != null)
                query.WhereAnd<Where<KGVoucherH.voucherDate,
                    LessEqual<Current<VoucherFilter.voucherDateTo>>>>();
            if (filter.VoucherStatus != null)
                query.WhereAnd<Where<KGVoucherH.status,
                    Equal<Current<VoucherFilter.voucherstatus>>>>();
            /*if (filter.VoucherStatus == "U")
                query.WhereAnd<Where<KGVoucherH.status, IsNull,
                    Or<KGVoucherH.status,Equal<VoucherStatuses.voucherU>>>>();
            if (filter.VoucherStatus == "C")
                query.WhereAnd<Where<KGVoucherH.status, 
                    Equal<VoucherStatuses.voucherC>>>();
                query.WhereAnd<Where<APInvoice.origRefNbr,IsNull>>();*/
            return query.Select();
        }
        #endregion

        #region Buttons  

        /*#region 產生分錄
        public PXAction<VoucherFilter> AddKGVoucher;
        [PXUIField(DisplayName = "產生分錄",
            MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true,Tooltip ="可勾選多筆產生分錄單")]
        public virtual IEnumerable addKGVoucher(PXAdapter adapter)
        {
            foreach (APInvoice row in AllVouchers.Select())
            {
                if (row.Selected == true)
                {
                    APRegister aPRegister = GetAPRegister(row.RefNbr, row.DocType);
                    APRegisterExt aPRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(aPRegister);
                    if (aPRegisterExt.UsrVoucherNo == null)
                    {

                        DeleteLine(row);
                        //base.Persist();
                        CreateKGVoucher(row);
                        //base.Persist();
                    }
                    else
                    {
                        throw new Exception("此單號不可產生分錄");
                    }
                    
                }
            }
            base.Persist();
            return adapter.Get();

        }
        #endregion*/

        #region 拋轉
        public PXAction<VoucherFilter> ProcessAction;
        [PXButton(CommitChanges = true,Tooltip = "可勾選多筆拋轉分錄")]
        [PXUIField(DisplayName = "拋轉")]
        protected IEnumerable processAction(PXAdapter adapter)
        {
            foreach (KGVoucherH row in AllVouchers.Select())
            {
                if (row.Selected == true)
                {
                    VoucherFilter filter = VoucherFilters.Current;
                    CheckStatusP();
                    if (!setCheckThread)
                    {
                        if (filter.VoucherDate == null)
                        {
                            throw new Exception("傳票日期不能為空值");
                        }
                        /*ProcessingBaseforKG processing = new ProcessingBaseforKG();
                        var list = processing.GetSelectedItems(this.AllVouchers.Cache, this.AllVouchers.Cache.Cached);

                        foreach (APInvoice row in list)
                        {*/
                        //VoucherUntil.testconnection(filter.VoucherDate);
                        /*foreach (APInvoice row in AllVouchers.Select())
                    {
                        if (row.Selected == true)
                        {*/
                        KGVoucherH voucherH = GetKGVoucherH(row.RefNbr);
                        //KGVoucherMaint graph = PXGraph.CreateInstance<KGVoucherMaint>();
                        //VoucherUntil.testconnection(filter.VoucherDate, voucherH);
                        voucherH.VoucherDate = filter.VoucherDate;

                        DateTime voucherDate = (DateTime)voucherH.VoucherDate;
                        PXResultset<KGVoucherL> set = PXSelect<KGVoucherL,
                            Where<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherH.voucherHeaderID>>,
                            And<KGVoucherL.kGVoucherType, Equal<WordType.pA>>>>
                            .Select(this, voucherH.VoucherHeaderID);
                        foreach (KGVoucherL voucherL in set)
                        {
                            KGBillPayment billPayment = PXSelect<KGBillPayment,
                                Where<KGBillPayment.billPaymentID, Equal<Required<KGVoucherL.billPaymentID>>>>
                                .Select(this, voucherL.BillPaymentID);
                            if (billPayment != null)
                            {
                                /*double period = (double)billPayment.PaymentPeriod;
                                voucherL.DueDate = voucherDate.AddDays(period);*/
                                if (billPayment.PaymentPeriod == 0)
                                {
                                    voucherL.DueDate = filter.VoucherDate;
                                }
                                else
                                {
                                    voucherL.DueDate =
                                        VoucherUntil.GetLongTremPaymentDate
                                        (this, (DateTime)filter.VoucherDate, billPayment.PaymentPeriod);
                                }
                                KGVoucherLs.Update(voucherL);
                                base.Persist();
                            }

                        }

                        BAccount bAccount = GetBAccount(voucherH.VendorID);
                        Contact contact = GetContact(voucherH.VendorID);
                        //2019/12/18 ADD Company改為此專案所綁的公司 而不是當前登入公司
                        Contract contract = GetContract(voucherH.ContractID);
                        CSAnswers cSAnswers = GetCSAnswers(voucherH.VendorID);
                        Address address = GetAddress(voucherH.VendorID);
                        PXResultset<GVApGuiInvoice> gVApGuiInvoice = GetGVApGuiInvoice(voucherH.RefNbr);
                        PXResultset<KGVoucherL> setvoucherL = PXSelect<KGVoucherL,
                            Where<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherH.voucherHeaderID>>>>
                            .Select(this, voucherH.VoucherHeaderID);
                        int? branchID = contract.DefaultBranchID;
                        string UserName = AgentFlowWebServiceUtil.getName(Accessinfo.UserName).ToUpper();
                        Branch branch = GetBranch(branchID);
                        VoucherUntil.InsertToOracle(filter.VoucherDate, voucherH, setvoucherL,
                            contact, bAccount, address, cSAnswers, gVApGuiInvoice, branch.BranchCD.Trim(), UserName);

                        //之後要加上
                        AllVouchers.Update(voucherH);
                        base.Persist();
                        PXUpdate<Set<APRegisterExt.usrVoucherDate, Required<APRegisterExt.usrVoucherDate>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(this, filter.VoucherDate, voucherH.RefNbr);
                        PXUpdate<Set<APRegisterExt.usrVoucherKey, Required<APRegisterExt.usrVoucherKey>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(this, voucherH.VoucherKey, voucherH.RefNbr);
                        PXUpdate<Set<APRegisterExt.usrVoucherNo, Required<APRegisterExt.usrVoucherNo>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(this, voucherH.VoucherNo, voucherH.RefNbr);
                        //}

                    }
                    base.Persist();
                }
            }
            
            return adapter.Get();
        }


        #endregion

        #region ERP作廢
        //Defining action DataView
        public PXAction<VoucherFilter> ERPVoid;
        //Defining Button Attribute with tooltip
        [PXButton(Tooltip = "可勾選多筆ERP作廢", CommitChanges = true)]
        //Providing title and mapping action access rights
        [PXUIField(DisplayName = "ERP作廢")]
        protected void eRPVoid()
        {
            VoucherFilter filter = VoucherFilters.Current;
            CheckStatusC();
            if (!setCheckThread)
            {
                WebDialogResult result = VoucherFilters.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("確定要ERP作廢嗎?"),
               MessageButtons.OKCancel, MessageIcon.Warning, true);
                //checking answer	
                if (result != WebDialogResult.OK) return;
                if (result == WebDialogResult.OK)
                {
                    foreach (KGVoucherH row in AllVouchers.Select())
                    {
                        if (row.Selected == true)
                        {
                            //刪除VoucherH
                            PXResultset<KGVoucherH> setH = PXSelect<KGVoucherH,
                                Where<KGVoucherH.refNbr, Equal<Required<APInvoice.refNbr>>>>.
                                Select(this, row.RefNbr);
                            foreach (KGVoucherH VoucherH in setH)
                            {
                                VoucherH.Status = "C";                  
                                AllVouchers.Update(VoucherH);                            
                            }
                            //2019/12/05清除計價單的三個傳票欄位
                            //清除欄位             
                            PXUpdate<Set<APRegisterExt.usrVoucherDate, Required<APRegisterExt.usrVoucherDate>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, null, row.RefNbr);
                            PXUpdate<Set<APRegisterExt.usrVoucherKey, Required<APRegisterExt.usrVoucherKey>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, null, row.RefNbr);
                            PXUpdate<Set<APRegisterExt.usrVoucherNo, Required<APRegisterExt.usrVoucherNo>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, null, row.RefNbr);
                        }
                    }

                }
            }
            base.Persist();
        }
        #endregion

        #region 取消送審
        //Defining action DataView
        public PXAction<VoucherFilter> CancelSubmit;
        //Defining Button Attribute with tooltip
        [PXButton(Tooltip = "可勾選多筆取消送審,請至計價調整更改",CommitChanges = true)]
        //Providing title and mapping action access rights
        [PXUIField(DisplayName = "取消送審")]
        protected void cancelSubmit()
        {
            VoucherFilter filter = VoucherFilters.Current;
            CheckStatusP();
            if (!setCheckThread)
            {
                WebDialogResult result = VoucherFilters.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("確定要取消送審嗎?"),
               MessageButtons.OKCancel, MessageIcon.Warning, true);
                //checking answer	
                if (result != WebDialogResult.OK) return;
                if (result == WebDialogResult.OK)
                {
                    foreach (KGVoucherH row in AllVouchers.Select())
                    {
                        if (row.Selected == true)
                        {

                            //刪除VoucherH
                            PXResultset<KGVoucherH> setH = PXSelect<KGVoucherH,
                                Where<KGVoucherH.refNbr, Equal<Required<APInvoice.refNbr>>>>.
                                Select(this, row.RefNbr);
                            foreach (KGVoucherH deleteH in setH)
                            {
                                AllVouchers.Delete(deleteH);
                                //刪除VoucherL
                                PXResultset<KGVoucherL> setL = PXSelect<KGVoucherL,
                                   Where<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherL.voucherHeaderID>>>>.
                                   Select(this, deleteH.VoucherHeaderID);
                                foreach (KGVoucherL deleteL in setL)
                                {
                                    KGVoucherLs.Delete(deleteL);
                                }
                            }
                            base.Persist();
                            //清除欄位        
                            //2019/12/05改為在ERP作廢就清空計價單傳票欄位
                            /*PXUpdate<Set<APRegisterExt.usrVoucherDate, Required<APRegisterExt.usrVoucherDate>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, null, row.RefNbr);
                            PXUpdate<Set<APRegisterExt.usrVoucherKey, Required<APRegisterExt.usrVoucherKey>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, null, row.RefNbr);
                            PXUpdate<Set<APRegisterExt.usrVoucherNo, Required<APRegisterExt.usrVoucherNo>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, null, row.RefNbr);*/
                            //改變計價單狀態為擱置
                            APInvoiceEntry invoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                            invoiceEntry.Document.Current = invoiceEntry.Document.Search<APInvoice.refNbr>(row.RefNbr);
                            invoiceEntry.Document.Current.Hold = true;
                            invoiceEntry.Document.Update(invoiceEntry.Document.Current);
                            invoiceEntry.Persist();
                            /*
                            PXUpdate<Set<APRegister.hold, Required<APRegister.hold>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, true, row.RefNbr);
                            PXUpdate<Set<APRegister.status, Required<APRegister.status>>,
                                APRegister,
                                Where<APRegister.refNbr,
                                Equal<Required<KGVoucherH.refNbr>>>>
                                .Update(this, "H", row.RefNbr);
                            */
                        }
                    }

                }
            }
            base.Persist();

            //Do usefull stuff.
        }
        #endregion

        #region Link
        public PXAction<VoucherFilter> VoucherPreviewAction;
        [PXButton(CommitChanges = true,Tooltip = "選取一筆預覽分錄")]
        //[PXLookupButton]
        [PXUIField(DisplayName = "分錄預覽")]
        protected IEnumerable voucherPreviewAction(PXAdapter adapter)
        {
            KGVoucherH row = AllVouchers.Current;
            VoucherFilter filter = VoucherFilters.Current;
            // Creating the instance of the graph
            if(filter.VoucherDate == null)
            {
                throw new Exception("請填傳票日期!");
            }
            //更改到期日抓傳票日期 2019/09/03 ADD
            KGVoucherH voucherH = GetKGVoucherH(row.RefNbr);
            //KGVoucherL detail = KGVoucherLs.Current;
            foreach(KGVoucherL voucherL in KGVoucherLs.Select())
            {
                if(voucherH.Status !="P")
                {
                    if (voucherL.AccountCD.ToString().Trim() == "2102")
                    {
                        KGBillPayment kgbillpayment = PXSelect<KGBillPayment,
                            Where<KGBillPayment.billPaymentID, 
                            Equal<Required<KGBillPayment.billPaymentID>>>>
                            .Select(this, voucherL.BillPaymentID);
                        DateTime date = (DateTime)filter.VoucherDate;

                        //voucherL.DueDate = date.AddDays((double)kgbillpayment.PaymentPeriod);
                        if (kgbillpayment.PaymentPeriod == 0)
                        {
                            voucherL.DueDate = filter.VoucherDate;
                        }
                        else
                        {
                            voucherL.DueDate =
                                VoucherUntil.GetLongTremPaymentDate
                                (this, (DateTime)filter.VoucherDate, kgbillpayment.PaymentPeriod);
                        }
                        voucherL.Digest = GetDigest(voucherL.AccountID, voucherL.Digest, voucherH.ContractID,
                            voucherH.VendorID, voucherL.DueDate);
                        KGVoucherLs.Update(voucherL);
                        base.Persist();
                    }
                }
                
            }
            base.Persist();
           
            KGVoucherMaint graph = PXGraph.CreateInstance<KGVoucherMaint>();
            // Setting the current product for the graph
            if(voucherH == null)
            {
                throw new Exception("此單號尚未產生分錄!");
            }
            graph.VoucherH.Current = 
                graph.VoucherH.Search<KGVoucherH.refNbr>(voucherH.RefNbr);
                // If the product is found by its ID, throw an exception to open
                // a new window (tab) in the browser
            /*if (graph.VoucherH.Current != null)
            {

                throw new PXRedirectRequiredException(graph, true, "VoucherMaint");
            }*/

            if (graph.VoucherH.Current != null)
            {
                throw new PXRedirectRequiredException(graph, false,"Open Voucher Maint")
                {
                    //Mode = PXBaseRedirectException.WindowMode.New
                };
            }

            return adapter.Get();
        }

        public PXAction<VoucherFilter> APInvoicePreviewAction;
        [PXButton(CommitChanges = true,Tooltip = "選取一筆預覽計價")]
        [PXLookupButton]
        [PXUIField(DisplayName = "計價預覽")]
        protected IEnumerable aPInvoicePreviewAction(PXAdapter adapter)
        {
            KGVoucherH row = AllVouchers.Current;
            // Creating the instance of the graph
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            // Setting the current product for the graph
            APInvoice aPInvoice = PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APInvoice.docType,Equal<Required<APInvoice.docType>>>>>
                .Select(this, row.RefNbr,row.DocType);
            graph.Document.Current = aPInvoice;

            if (graph.Document.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "Open APInvoice Entry")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
               
            }
            
            return adapter.Get();
        }
        #endregion      
        
        #endregion

        #region Event
        protected virtual void KGVoucherH_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGVoucherH row = (KGVoucherH)e.Row;
            if (row == null) return;
            PXUIFieldAttribute.SetEnabled(sender, row, false);
            PXUIFieldAttribute.SetEnabled<APInvoice.selected>(sender, row, true);
            
            /*APTran aPTran = PXSelect<APTran, 
                Where<APTran.refNbr,Equal<Required<APInvoice.refNbr>>>>
                .Select(this, row.RefNbr);
            if (aPTran == null) return;
            APRegister aPRegister = PXSelect<APRegister,
                Where<APRegister.docType,Equal<Required<APInvoice.docType>>,
                And<APRegister.refNbr,Equal<Required<APInvoice.refNbr>>>>>
                .Select(this, row.DocType, row.RefNbr);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(aPRegister);
            registerExt.UsrPONbrforUnbound = aPTran.PONbr;*/
        }
        #endregion

        #region Methods
        bool setCheckThread = false;
        public void CheckStatusP()
        {
            foreach (KGVoucherH row in AllVouchers.Select())
            {
                //KGVoucherH voucherH = GetKGVoucherH(row.RefNbr);
                if (row.Selected == true)
                {
                    if (row.Status == "P")
                    {
                        setCheckThread = true;
                        AllVouchers.Cache.RaiseExceptionHandling<KGVoucherH.status>(
                        row, row.Status, new PXSetPropertyException("狀態為「已拋轉」。不可用此按鈕", PXErrorLevel.RowError));
                    }
                }
            }
        }
        public void CheckStatusC()
        {
            foreach (KGVoucherH row in AllVouchers.Select())
            {
                //KGVoucherH voucherH = GetKGVoucherH(row.RefNbr);
                if (row.Selected == true)
                {
                    if (row.Status != "P")
                    {
                        setCheckThread = true;
                        AllVouchers.Cache.RaiseExceptionHandling<KGVoucherH.status>(
                        row, row.Status, new PXSetPropertyException("狀態需為「已拋轉」。不可用此按鈕", PXErrorLevel.RowError));
                    }
                }
            }
        }

        #region Select Methods
        public Branch GetBranch(int? BranchID)
        {
            return PXSelect<Branch,
                Where<Branch.branchID, Equal<Required<POOrder.branchID>>>>
                .Select(this, BranchID);
        }

        public BAccount GetBAccount(int? VendorID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<KGVoucherH.vendorID>>>>
                .Select(this, VendorID);
        }

        public Contract GetContract(int? ProjectID)
        {
            return PXSelect<Contract,
                    Where<Contract.contractID, Equal<Required<POOrder.projectID>>>>
                    .Select(this, ProjectID);
        }

        public Contact GetContact(int? VendorID)
        {
            return PXSelectJoin<Contact,
                    InnerJoin<BAccount, On<BAccount.defContactID, Equal<Contact.contactID>>>,
                    Where<BAccount.bAccountID, Equal<Required<APInvoice.vendorID>>>>
                    .Select(this, VendorID);
        }

        public Address GetAddress(int? VendorID)
        {
            return PXSelect<Address,
                Where<Address.bAccountID, Equal<Required<KGVoucherH.vendorID>>>>
                .Select(this, VendorID);
        }

        public CSAnswers GetCSAnswers(int? VendorID)
        {
            return PXSelectJoin<CSAnswers,
                InnerJoin<Vendor,
                On<Vendor.noteID, Equal<CSAnswers.refNoteID>,
                And<CSAnswers.attributeID, Equal<word.oWNER>,
                And<Vendor.bAccountID, Equal<Required<KGVoucherH.vendorID>>>>>>>
                .Select(this, VendorID);
        }

        public PXResultset<GVApGuiInvoice> GetGVApGuiInvoice(string RefNbr)
        {
            return PXSelect<GVApGuiInvoice,
                Where<GVApGuiInvoice.refNbr,Equal<Required<KGVoucherH.refNbr>>>>
                .Select(this, RefNbr);         
        }

        public string GetDigest(int? pAccountID, string Digest, int? ProjectID
            ,int? VendorID,DateTime? Duedate)
        {
            KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>>>
                .Select(this, pAccountID);
            if (setup == null) return null;

            Digest = setup.OracleDigestRule;
            CSAnswers answers = PXSelectJoin<CSAnswers,
            InnerJoin<Contract,
            On<Contract.noteID, Equal<CSAnswers.refNoteID>,
            And<CSAnswers.attributeID, Equal<WordType.a005>,
            And<Contract.contractID, Equal<Required<APInvoice.projectID>>>>>>>
            .Select(this, ProjectID);
            if (setup.OracleDigestRule != null)
            {      
                if (setup.OracleDigestRule.Contains("###廠商簡稱###"))
                {
                    Contact contact = GetContact(VendorID);
                    Digest = Digest.Replace("###廠商簡稱###", contact.FullName);
                }
                
                if (setup.OracleDigestRule.Contains("###廠商名稱###"))
                {
                    BAccount bAccount = GetBAccount(VendorID);
                    Digest = Digest.Replace("###廠商名稱###", bAccount.AcctName);
                }

                if (setup.OracleDigestRule.Contains("###到期日###"))
                {
                    if (Duedate != null)
                    {
                        DateTime date = (DateTime)Duedate;
                        Digest = Digest.Replace("###到期日###", date.ToString("yyyy/MM/dd"));
                    }

                }
            }


            return Digest;

        }

        private TaxTran GetTaxTran(string RefNbr)
        {
            return PXSelect<TaxTran,
                Where<TaxTran.refNbr, Equal<Required<TaxTran.refNbr>>>>
                .Select(this, RefNbr);
        }

        private APRegister GetAPRegister(string RefNbr, string DocType)
        {
            return PXSelect<APRegister,
                Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                .Select(this, DocType, RefNbr);
        }

        private Account GetAccount(int? AccountID)
        {
            return PXSelect<Account,
                    Where<Account.accountID, Equal<Required<Account.accountID>>>>
                    .Select(this, AccountID);
        }

        private APTran GetAPTran(string refNbr)
        {
            return PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>>>
                .Select(this, refNbr);
        }

        private APAdjust GetAdjust(string Rdoctype, string Rrefnbr)
        {
            return PXSelect<APAdjust,
                Where<APAdjust.adjdDocType, Equal<Required<APRegister.docType>>,
                And<APAdjust.adjdRefNbr, Equal<Required<APRegister.refNbr>>>>>
                .Select(this, Rdoctype, Rrefnbr);
        }

        private Tax GetTax(TaxTran taxTran)
        {
            return PXSelect<Tax,
                Where<Tax.taxID, Equal<Required<TaxTran.taxID>>>>
                .Select(this, taxTran.TaxID);
        }

        private Location GetAPRegisterLocation(APRegister register)
        {
            return PXSelect<Location,
                    Where<Location.bAccountID, Equal<Required<Location.bAccountID>>>>
                    .Select(this, register.VendorID);
        }

        private APTran GetAPTranLine(string TranType, string RefNbr, int? LineNbr)
        {
            return PXSelect<APTran, Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTran.tranType, Equal<Required<APInvoice.docType>>,
                And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>
                .Select(this, RefNbr, TranType, LineNbr);
        }

        public KGVoucherH GetKGVoucherH(string RefNbr)
        {
            return PXSelect<KGVoucherH,
                Where<KGVoucherH.refNbr,
                Equal<Required<KGVoucherH.refNbr>>>>
                .Select(this, RefNbr);
        }
        
        #endregion

        /*#region Voucher Methods
        int rowNumber = 1;
        //建立分錄
        public void CreateKGVoucher(APInvoice aPInvoice)
        {
            
            #region KGVoucherH
            KGVoucherMaint graph = PXGraph.CreateInstance<KGVoucherMaint>();
            KGVoucherH voucherH = new KGVoucherH();
            voucherH.DocType = aPInvoice.DocType;
            voucherH.RefNbr = aPInvoice.RefNbr;
            if (aPInvoice.ProjectID == null)
            {
                APTran aPTran =
                    PXSelect<APTran, Where<APTran.projectID, IsNotNull,
                    And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                    .Select(this, aPInvoice.RefNbr);
                if (aPTran == null) return;
                Contract contract = GetContract(aPTran.ProjectID);
                voucherH.ContractID = aPTran.ProjectID;
                voucherH.ContractCD = contract.ContractCD;
                voucherH.ContractDesc = contract.Description;
            }
            else
            {
                Contract contract = GetContract(aPInvoice.ProjectID);
                voucherH.ContractID = aPInvoice.ProjectID;
                voucherH.ContractCD = contract.ContractCD;
                voucherH.ContractDesc = contract.Description;
            }
            BAccount bAccount = GetBAccount(aPInvoice.VendorID);
            if (bAccount.Type == "VE")
            {
                voucherH.VendorType = "V";
            }
            else if (bAccount.Type == "EP")
            {
                voucherH.VendorType = "E";
            }
            voucherH.VendorID = aPInvoice.VendorID;
            voucherH.VendorCD = bAccount.AcctCD;
            voucherH.Status = "U";
            APTran tran = GetAPTran(aPInvoice.RefNbr);
            voucherH.PONbr = tran.PONbr;
            AllVouchers.Insert(voucherH);

            #endregion         
            if (aPInvoice.DocType == "INV")
            {
                CreateValuationOfAPT(voucherH, aPInvoice);//1
                CreateValuationOfTT(voucherH, aPInvoice);//2
                CreateValuationOfRR(voucherH, aPInvoice);//3
                CreateValuationOfDR(voucherH, aPInvoice);//4
                CreateValuationOfPA(voucherH, aPInvoice);//5
                CreateValuationOfKP(voucherH, aPInvoice);//6
                CreateValuationOfDT(voucherH, aPInvoice);//7
                CreateValuationOfADJ(voucherH, aPInvoice);//8
            }
            else if (aPInvoice.DocType == "ADR")
            {
                CreateDebitAdjustOfAPT(voucherH, aPInvoice);//1
                CreateDebitAdjustOfTT(voucherH, aPInvoice);//2
                CreateDebitAdjustOfRR(voucherH, aPInvoice);//3
                CreateDebitAdjustOfDR(voucherH, aPInvoice);//4
                CreateDebitAdjustOfPA(voucherH, aPInvoice);//5
                CreateDebitAdjustOfKP(voucherH, aPInvoice);//6
            }
            else if (aPInvoice.DocType == "PPM")
            {
                CreatePrepaymentsOfAPT(voucherH, aPInvoice);//1
                CreatePrepaymentsOfTT(voucherH, aPInvoice);//2
                CreatePrepaymentsOfPA(voucherH, aPInvoice);//3
                CreatePrepaymentsOfKP(voucherH, aPInvoice);//4
            }
        }

        #region 計價
        //文件明細:(1)APT(假資料Inside)
        public void CreateValuationOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTran> set = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                And<APTran.tranType, Equal<WordType.iNV>,
                And<Where<APTranExt.usrValuationType, Equal<WordType.b>,
                Or<APTranExt.usrValuationType, Equal<WordType.a>>>>>>,
                OrderBy<Asc<APTran.lineNbr>>>
                .Select(this, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            { 
                string CDType = "";
                decimal? LineAmt = 0;
                if(aPTran.CuryLineAmt>0)
                {
                    CDType = "D";
                    LineAmt = aPTran.CuryLineAmt;
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(aPTran.CuryLineAmt));
                }
                CreateDetailLine(voucherH, aPTran.ProjectID, aPTran.AccountID,
                    LineAmt, CDType, aPTran.VendorID, "APT", null, null, aPTran);
            }
            #region 假資料
            /*KGVoucherL typeC = new KGVoucherL();
            APTran APTranSumforC = PXSelectGroupBy<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                And<Where<APTranExt.usrValuationType, Equal<WordType.b>,
                Or<APTranExt.usrValuationType, Equal<WordType.a>>>>>,
                Aggregate<Sum<APTran.curyLineAmt>>>.Select(this, aPInvoice.RefNbr);
            Contract contract = GetAPInvoiceContract(aPInvoice);
            BAccount bAccount = GetAPInvoiceBAccount(aPInvoice);
            typeC.ContractID = aPInvoice.ProjectID;
            typeC.ContractCD = contract.ContractCD;
            //string CD = "2012";
            Account accountC = PXSelect<Account,
                Where<Account.accountCD, Equal<Required<Account.accountCD>>>>
                .Select(this, "2102");
            typeC.AccountCD = accountC.AccountCD;
            typeC.AccountID = APTranSumforC.AccountID;

            typeC.AccountDesc = accountC.Description;
            typeC.Cd = "C";
            typeC.Amt = APTranSumforC.CuryLineAmt;
            if (bAccount.Type == "VE")
            {
                typeC.VendorType = "V";
            }
            else if (bAccount.Type == "EP")
            {
                typeC.VendorType = "E";
            }
            typeC.VendorCD = bAccount.AcctCD;
            typeC.ItemNo = rowNumber;
            rowNumber++;
            KGVoucherLs.Insert(typeC);
            #endregion
        }
        //進項稅額:(2)TT
        public void CreateValuationOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.iNV>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "INV");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;
                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    taxTran.CuryTaxAmt+taxTran.CuryRetainedTaxAmt, "D", register.VendorID, "TT", null, null);
            }
        }
        //退還保留款:(3)RR
        public void CreateValuationOfRR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                   PXSelect<KGBillSummary,
               Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
               And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
               .Select(this, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;
                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetAPRegisterLocation(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    billSummary.RetentionReturnAmt, "D", register.VendorID, "RR", null, null);
            }
        }
        //扣保留款:(4)DR
        public void CreateValuationOfDR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                PXSelect<KGBillSummary,
            Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
            And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
            .Select(this, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;
                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetAPRegisterLocation(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    billSummary.RetentionAmt, "C", register.VendorID, "DR", null, null);
            }
        }
        //貸方付款科目:(5)PA
        public void CreateValuationOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                KGSetUp setUp = PXSelect<KGSetUp>.Select(this);
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                if (billPayment.PaymentMethod == "A")
                {
                    AcctID = setUp.KGCheckAccountID;
                }
                else
                {
                    AcctID = setUp.KGCashAccountID;
                }
                decimal? amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(billPayment.PaymentMethod, billPayment.PaymentAmount);
                DateTime date = (DateTime)billPayment.PaymentDate;
                date.AddDays((double)billPayment.PaymentPeriod);
                CreateLine(voucherH, register.ProjectID, AcctID, amt, "C", register.VendorID,
                    "PA", billPayment.BillPaymentID, date);
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(6)KP
        public void CreateValuationOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, Equal<WordType.a>>>>
                .Select(this, aPInvoice.RefNbr);
            //if (set.Count == 0) return;
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                amt = amt + calcPostage.TotalPostageAmt
                    (billPayment.PaymentMethod, billPayment.PaymentAmount);
            }
            KGBillPayment kGBillPayment = PXSelectGroupBy<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, Equal<WordType.a>>>,
                Aggregate<GroupBy<KGBillPayment.refNbr>>>
                .Select(this, aPInvoice.RefNbr);
            if (kGBillPayment == null) return;
            KGSetUp setUp = PXSelect<KGSetUp>.Select(this);
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID, amt,
                "C", register.VendorID, "KP", null, null);
        }
        //扣款頁籤(for 計價): (7)DT
        public void CreateValuationOfDT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APRegister> R = PXSelect<APRegister,
                Where<APRegister.origRefNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);

            foreach (APRegister aPRegister in R)
            {
                if (aPRegister == null) return;
                PXResultset<APTran> Set = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(this, aPRegister.RefNbr);
                foreach (APTran tran in Set)
                {
                    CreateLine(voucherH, tran.ProjectID, tran.AccountID,
                        tran.CuryLineAmt, "C", tran.VendorID, "DT", null, null);
                }
            }
        }
        //退還預付款:(8)ADJ
        public void CreateValuationOfADJ(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            APRegister R = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            APAdjust adjust = GetAdjust(R.DocType, R.RefNbr);
            if (R == null) return;
            if (adjust == null) return;
            PXResultset<APRegister> register = PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(this, adjust.AdjgRefNbr);
            foreach (APRegister R2 in register)
            {
                if (R2 == null) return;
                CreateLine(voucherH, R.ProjectID, R2.APAccountID,
                    adjust.CuryAdjdAmt, "C", R.VendorID, "ADJ", null, null);
            }
        }
        #endregion

        #region 借方調整
        //文件明細:(1)APT
        public void CreateDebitAdjustOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTran> set = PXSelect<APTran,
               Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
               And<APTran.tranType, Equal<WordType.aDR>,
               And<Where<APTranExt.usrValuationType, Equal<WordType.b>,
               Or<APTranExt.usrValuationType, Equal<WordType.a>>>>>>,
               OrderBy<Asc<APTran.lineNbr>>>
               .Select(this, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            {
                CreateDetailLine(voucherH, aPTran.ProjectID, aPTran.AccountID,
                    aPTran.CuryLineAmt, "C", aPTran.VendorID, "APT", null, null, aPTran);
            }
        }
        //進項稅額:(2)TT
        public void CreateDebitAdjustOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.aDR>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "ADR");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;
                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    taxTran.CuryTaxAmt+taxTran.CuryRetainedTaxAmt, "C", register.VendorID, "TT", null, null);
            }
        }
        //退還保留款:(3)RR
        public void CreateDebitAdjustOfRR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                   PXSelect<KGBillSummary,
               Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
               And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
               .Select(this, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;
                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetAPRegisterLocation(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    billSummary.RetentionReturnAmt, "C", register.VendorID, "RR",
                    null, null);
            }
        }
        //扣保留款:(4)DR
        public void CreateDebitAdjustOfDR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillSummary> kgBillSummary =
                PXSelect<KGBillSummary,
            Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
            And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
            .Select(this, voucherH.DocType, voucherH.RefNbr);
            foreach (KGBillSummary billSummary in kgBillSummary)
            {
                if (billSummary == null) return;

                APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
                Location location = GetAPRegisterLocation(register);
                CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                    billSummary.RetentionAmt, "D", register.VendorID, "DR", null, null);
            }
        }
        //貸方付款科目:(5)PA
        public void CreateDebitAdjustOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;

                CalcPostage calcPostage = new CalcPostage();
                KGSetUp setUp = PXSelect<KGSetUp>.Select(this);
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                if (billPayment.PaymentMethod == "A")
                {
                    AcctID = setUp.KGCheckAccountID;
                }
                else
                {
                    AcctID = setUp.KGCashAccountID;
                }
                decimal? amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(billPayment.PaymentMethod, billPayment.PaymentAmount);
                DateTime date = (DateTime)billPayment.PaymentDate;
                date.AddDays((double)billPayment.PaymentPeriod);
                CreateLine(voucherH, register.ProjectID, AcctID, amt, "D",
                    register.VendorID, "PA", billPayment.BillPaymentID, date);
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(6)KP
        public void CreateDebitAdjustOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, Equal<WordType.a>>>>
                .Select(this, aPInvoice.RefNbr);
            if (set.Count == 0) return;
            foreach (KGBillPayment billPayment in set)
            {
                CalcPostage calcPostage = new CalcPostage();
                amt += amt + calcPostage.TotalPostageAmt
                    (billPayment.PaymentMethod, billPayment.PaymentAmount);
            }
            KGBillPayment kGBillPayment = PXSelectGroupBy<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, Equal<WordType.a>>>,
                Aggregate<GroupBy<KGBillPayment.refNbr>>>
                .Select(this, aPInvoice.RefNbr);
            KGSetUp setUp = PXSelect<KGSetUp>.Select(this);
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID,
                amt, "D", register.VendorID, "KP", null, null);
        }
        #endregion

        #region 預付款
        //文件明細:(1)APT
        public void CreatePrepaymentsOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTran> set = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                And<APTran.tranType, Equal<WordType.pPM>,
                And<APTranExt.usrValuationType, Equal<WordType.p>>>>,
                OrderBy<Asc<APTran.lineNbr>>>
                .Select(this, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            {
                APRegister register = GetAPRegister(aPTran.RefNbr, "PPM");
                CreateDetailLine(voucherH, aPTran.ProjectID, register.APAccountID,
                    aPTran.CuryLineAmt, "D", aPTran.VendorID, "APT", null, null, aPTran);
            }
        }
        //進項稅額:(2)TT
        public void CreatePrepaymentsOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.pPM>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "PPM");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;
                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    taxTran.CuryTaxAmt+taxTran.CuryRetainedTaxAmt, "D", register.VendorID, "TT", null, null);
            }
        }
        //貸方付款科目:(3)PA
        public void CreatePrepaymentsOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                KGSetUp setUp = PXSelect<KGSetUp>.Select(this);
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                if (billPayment.PaymentMethod == "A")
                {
                    AcctID = setUp.KGCheckAccountID;
                }
                else
                {
                    AcctID = setUp.KGCashAccountID;
                }
                decimal? amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(billPayment.PaymentMethod, billPayment.PaymentAmount);
                DateTime date = (DateTime)billPayment.PaymentDate;
                date.AddDays((double)billPayment.PaymentPeriod);
                CreateLine(voucherH, register.ProjectID, AcctID, amt, "C",
                    register.VendorID, "PA", billPayment.BillPaymentID, date);
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(4)KP
        public void CreatePrepaymentsOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, Equal<WordType.a>>>>
                .Select(this, aPInvoice.RefNbr);
            if (set.Count == 0) return;
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                amt += amt + calcPostage.TotalPostageAmt
                    (billPayment.PaymentMethod, billPayment.PaymentAmount);
            }
            KGBillPayment kGBillPayment = PXSelectGroupBy<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, Equal<WordType.a>>>,
                Aggregate<GroupBy<KGBillPayment.refNbr>>>
                .Select(this, aPInvoice.RefNbr);
            KGSetUp setUp = PXSelect<KGSetUp>.Select(this);
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID,
                amt, "C", register.VendorID, "KP", null, null);
        }
        #endregion

        //Line一樣的資料
        public void CreateDetailLine(KGVoucherH voucherH, int? ProjectID,
            int? AcctID, decimal? Amt, string CD, int? VendorID, string kgvouchertype
            , int? PaymentID, DateTime? DueDate, APTran aPTran)
        {
            KGVoucherL voucherL = new KGVoucherL();
            //APInvoice aPInvoice = AllVouchers.Current;
            Contract contract = GetContract(ProjectID);
            voucherL.ContractID = contract.ContractID;
            voucherL.ContractCD = contract.ContractCD.Trim();
            Account account = GetAccount(AcctID);
            voucherL.AccountID = account.AccountID;//AcctID
            voucherL.AccountCD = account.AccountCD;//AcctID
            voucherL.AccountDesc = account.Description;//AcctID
            voucherL.Cd = CD;//CD
            voucherL.Amt = Amt;//Ant
            BAccount bAccount = GetBAccount(VendorID);
            if (bAccount.Type == "VE")
            {
                voucherL.VendorType = "V";
            }//VendorID
            else if (bAccount.Type == "EP")
            {
                voucherL.VendorType = "E";
            }//VendorID
            voucherL.VendorID = bAccount.BAccountID;//VendorID
            voucherL.VendorCD = bAccount.AcctCD.Trim();//VendorID
            voucherL.KGVoucherType = kgvouchertype;//kgvouchertype
            voucherL.ItemNo = rowNumber;

            APTran tran = GetAPTranLine(aPTran.TranType, aPTran.RefNbr, aPTran.LineNbr);
            voucherL.Digest = tran.TranDesc;

            //voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice);

            if (voucherL.KGVoucherType == "PA")
            {
                voucherL.BillPaymentID = PaymentID;
            }
            if (Amt == 0) return;

            voucherL.DueDate = DueDate;
            KGVoucherMaint graph = PXGraph.CreateInstance<KGVoucherMaint>();
            KGVoucherLs.Insert(voucherL);
            rowNumber++;
        }
        public void CreateLine(KGVoucherH voucherH, int? ProjectID,
            int? AcctID, decimal? Amt, string CD, int? VendorID, string kgvouchertype
            , int? PaymentID, DateTime? DueDate)
        {
            
            KGVoucherL voucherL = new KGVoucherL();
            APInvoice aPInvoice = PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>,
            And<APInvoice.docType, Equal<Required<APInvoice.docType>>>>>
                .Select(this, voucherH.RefNbr, voucherH.DocType);
            //APInvoice aPInvoice = AllVouchers.Current;
            if (ProjectID == null)
            {
                APTran aPTranContract =
                    PXSelect<APTran, Where<APTran.projectID, IsNotNull,
                    And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                    .Select(this, voucherH.RefNbr);
                if (aPTranContract == null) return;
                Contract contract = GetContract(aPTranContract.ProjectID);
                voucherL.ContractID = contract.ContractID;
                voucherL.ContractCD = contract.ContractCD.Trim();
            }
            else
            {
                Contract contract = GetContract(ProjectID);
                voucherL.ContractID = contract.ContractID;
                voucherL.ContractCD = contract.ContractCD.Trim();
            }

            Account account = GetAccount(AcctID);
            voucherL.AccountID = account.AccountID;//AcctID
            voucherL.AccountCD = account.AccountCD;//AcctID
            voucherL.AccountDesc = account.Description;//AcctID
            voucherL.Cd = CD;//CD
            voucherL.Amt = Amt;//Ant
            BAccount bAccount = GetBAccount(VendorID);
            if (bAccount.Type == "VE")
            {
                voucherL.VendorType = "V";
            }//VendorID
            else if (bAccount.Type == "EP")
            {
                voucherL.VendorType = "E";
            }//VendorID
            voucherL.VendorID = bAccount.BAccountID;//VendorID
            voucherL.VendorCD = bAccount.AcctCD.Trim();//VendorID
            voucherL.KGVoucherType = kgvouchertype;//kgvouchertype
            voucherL.ItemNo = rowNumber;

            APInvoiceEntry aPInvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
            APTran aPTran = aPInvoiceEntry.Transactions.Current;
            voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice, DueDate);


            if (voucherL.KGVoucherType == "PA")
            {
                voucherL.BillPaymentID = PaymentID;
            }
            if (Amt == 0) return;

            voucherL.DueDate = DueDate;
            KGVoucherMaint graph = PXGraph.CreateInstance<KGVoucherMaint>();
            KGVoucherLs.Insert(voucherL);
            rowNumber++;
        }

        //刪除Line
        public void DeleteLine(APInvoice aPInvoice)
        {
            
            KGVoucherH kGVoucherH = PXSelect<KGVoucherH,
                Where<KGVoucherH.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, aPInvoice.RefNbr);
            if (kGVoucherH == null) return;
            KGVoucherMaint graph = PXGraph.CreateInstance<KGVoucherMaint>();
            PXResultset<KGVoucherL> kgvoucherL =
                PXSelect<KGVoucherL,
                Where<KGVoucherL.voucherHeaderID,
                Equal<Required<KGVoucherH.voucherHeaderID>>>>
                .Select(this, kGVoucherH.VoucherHeaderID);

            foreach (KGVoucherL voucherL in kgvoucherL)
            {
                

                if (voucherL == null) return;
                KGVoucherLs.Delete(voucherL);
            }
            AllVouchers.Delete(kGVoucherH);
            this.Persist();
        }

        //分錄摘要
        public string GetDigest(int? pAccountID, string Digest, APInvoice aPInvoice, DateTime? Duedate)
        {
            KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>>>
                .Select(this, pAccountID);
            if (setup == null) return null;

            Digest = setup.OracleDigestRule;
            CSAnswers answers = PXSelectJoin<CSAnswers,
            InnerJoin<Contract,
            On<Contract.noteID, Equal<CSAnswers.refNoteID>,
            And<CSAnswers.attributeID, Equal<WordType.a005>,
            And<Contract.contractID, Equal<Required<APInvoice.projectID>>>>>>>
            .Select(this, aPInvoice.ProjectID);
            if (setup.OracleDigestRule != null)
            {
                if (setup.OracleDigestRule.Contains("###專案簡稱###"))
                {
                    Digest = Digest.Replace("###專案簡稱###", answers.Value);
                }
                if (setup.OracleDigestRule.Contains("###廠商簡稱###"))
                {
                    Contact contact = GetContact(aPInvoice.VendorID);
                    Digest = Digest.Replace("###廠商簡稱###", contact.FullName);
                }
                if (setup.OracleDigestRule.Contains("###代扣稅率###"))
                {
                    TaxTran taxTran = GetTaxTran(aPInvoice.RefNbr);
                    Digest = Digest.Replace("###代扣稅率###", taxTran.TaxRate.ToString());
                }
                if (setup.OracleDigestRule.Contains("###廠商名稱###"))
                {
                    BAccount bAccount = GetBAccount(aPInvoice.VendorID);
                    Digest = Digest.Replace("###廠商名稱###", bAccount.AcctName);
                }
                if (setup.OracleDigestRule.Contains("###到期日###"))
                {
                    if (Duedate != null)
                    {
                        DateTime date = (DateTime)Duedate;
                        Digest = Digest.Replace("###到期日###", date.ToString("yyyy/MM/dd"));
                    }

                }
            }

            return Digest;
        }
        #endregion*/


        #endregion

    }
    [Serializable]
    public class VoucherFilter : IBqlTable
    {
        
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

        #region DocType
        [PXString(3, IsUnicode = true)]
        [PXUIField(DisplayName = "Doc Type")]
        [PXSelector(typeof(Search4<KGVoucherH.docType,
            Aggregate<GroupBy<KGVoucherH.docType>>>))]
        public virtual string DocType { get; set; }
        public abstract class docType : IBqlField { }
        #endregion

        #region RefNbr
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "RefNbr")]

        [PXSelector(typeof(Search2<KGVoucherH.refNbr,
        InnerJoinSingleTable<APInvoice,
            On<APInvoice.docType, Equal<KGVoucherH.docType>,
            And<APInvoice.refNbr, Equal<KGVoucherH.refNbr>>>>>),
            typeof(APInvoice.docType),
            typeof(KGVoucherH.refNbr),
            typeof(APInvoice.docDesc),
            typeof(APInvoice.status),
            typeof(APInvoice.docDate),
            typeof(APInvoice.dueDate),
            typeof(APInvoice.tranPeriodID)
            )]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : IBqlField { }
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

        #region DueDateFrom
        [PXDate()]
        [PXUIField(DisplayName = "Due Date From")]
        public virtual DateTime? DueDateFrom { get; set; }
        public abstract class dueDateFrom : IBqlField { }
        #endregion

        #region DueDateTo
        [PXDate()]
        [PXUIField(DisplayName = "Due Date To")]
        public virtual DateTime? DueDateTo { get; set; }
        public abstract class dueDateTo : IBqlField { }
        #endregion

        #region PONbr
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PONbr")]
        [PXSelector(typeof(Search5<KGVoucherH.pONbr,
            InnerJoin<APRegister,
                On<APRegister.refNbr,Equal<KGVoucherH.refNbr>>>,
            Where<KGVoucherH.docType,Equal<APRegister.docType>>,
            Aggregate<GroupBy<KGVoucherH.pONbr>>,
            OrderBy<Desc<KGVoucherH.pONbr>>>),
            typeof(KGVoucherH.pONbr),
            typeof(KGVoucherH.refNbr))]
        public virtual string PONbr { get; set; }
        public abstract class pONbr : IBqlField { }
        #endregion      

        #region VoucherStatus
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "VoucherStatus")]
        [PXStringList(new string[] { "U", "P", "C" },
            new string[] { "未拋轉", "已拋轉", "ERP作廢" })]
        [PXDefault("U")]
        public virtual string VoucherStatus { get; set; }
        public abstract class voucherstatus : IBqlField { }
        #endregion

        #region VoucherDateFrom
        [PXDate()]
        [PXUIField(DisplayName = "Voucher Date From")]
        public virtual DateTime? VoucherDateFrom { get; set; }
        public abstract class voucherDateFrom : IBqlField { }
        #endregion

        #region VoucherDateTo
        [PXDate()]
        [PXUIField(DisplayName = "Voucher Date To")]
        public virtual DateTime? VoucherDateTo { get; set; }
        public abstract class voucherDateTo : IBqlField { }
        #endregion

        #region VoucherDate
        [PXDate()]
        [PXUIField(DisplayName = "Voucher Date")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? VoucherDate { get; set; }
        public abstract class voucherDate : IBqlField { }
        #endregion

        #region UsrIsConfirm
        [PXInt]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrIsConfirm")]
        [PXIntList(new int[] { 0, 1 }, new string[] { "未確認", "已確認" })]
        public virtual int? UsrIsConfirm { get; set; }
        public abstract class usrIsConfirm : IBqlField { }
        #endregion

    }

    public static class VoucherStatuses
    {

        public const string HoldName = "Hold";
        public const string OpenName = "Open";
        public const string CloseName = "Close";
        public const string BalanceName = "Balance";

        public const string Hold = "H";
        public const string Open = "N";
        public const string Close = "C";
        public const string Balance = "B";

        public const string VoucherC = "C";
        public const string VoucherP = "P";
        public const string VoucherU = "U";


        public class open : BqlString.Constant<open>
        {
            public open() : base(Open) { }
        }

        public class close : BqlString.Constant<close>
        {
            public close() : base(Close) { }
        }
        public class hold : BqlString.Constant<hold>
        {
            public hold() : base(Hold) { }
        }
        public class balance : BqlString.Constant<balance>
        {
            public balance() : base(Balance) { }
        }

        public class voucherU : BqlString.Constant<voucherU>
        {
            public voucherU() : base(VoucherU) { }
        }

        public class voucherC : BqlString.Constant<voucherC>
        {
            public voucherC() : base(VoucherC) { }
        }
        public class voucherP : BqlString.Constant<voucherP>
        {
            public voucherP() : base(VoucherP) { }
        }
    }
}