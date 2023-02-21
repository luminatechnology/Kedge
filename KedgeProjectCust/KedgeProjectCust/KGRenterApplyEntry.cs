using System;
using System.Collections;
using PX.Data;
using Kedge.DAC;
using PX.Objects.PO;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.SM;



namespace Kedge
{
    public class KGRenterApplyEntry : PXGraph<KGRenterApplyEntry, KGDailyRenter>
    {
        //public PXSelect<KGDailyRenter> DailyRenters;

       
        public PXSelectJoin<KGDailyRenter, InnerJoin<PMProject, On<KGDailyRenter.contractID,
            Equal<PMProject.contractID>>>,
            Where<Match<PMProject, Current<AccessInfo.userName>>>> DailyRenters;
        
        /**
        protected virtual IEnumerable dailyRenters() {
            KGDailyRenter master = DailyRenters.Current;
            PXSelectBase<KGDailyRenter> query =
                                        new PXSelectJoin<KGDailyRenter, 
                                        InnerJoin<PMProject, On<KGDailyRenter.contractID,
                                        Equal<PMProject.contractID>>>,
                                        Where<Match<PMProject, Current<AccessInfo.userName>>>>(this);
                                         query.WhereAnd<Where<Users.pKID,
                    Equal<Current<AccessInfo.userID>>>>();
            return query.Select();
        }*/
        public PXSetup<KGSetUp> KGSetup;
        public PXSelect<KGDailyRenterVendor,
                 Where<KGDailyRenterVendor.dailyRenterID,
                 Equal<Current<KGDailyRenter.dailyRenterID>>,
                 And<KGDailyRenterVendor.type, Equal<KGDailyRenterActualEntry.DEFAULT>>>> DailyRenterVendors;
        public PXSelect<KGDailyRenterVendorAct,
             Where<KGDailyRenterVendorAct.dailyRenterID,
             Equal<Current<KGDailyRenter.dailyRenterID>>,
             And<KGDailyRenterVendorAct.type, Equal<KGDailyRenterActualEntry.ACCTUALLY>>>> DailyRenterActVendors;

        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Status")]
        [KGDailyRenterStatus.List]
        [PXDefault(KGDailyRenterStatus.OPEN)]
        protected void KGDailyRenter_Status_CacheAttached(PXCache sender)
        {

        }
        protected virtual void KGDailyRenterVendor_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            KGDailyRenterVendor row = (KGDailyRenterVendor)e.Row;
            KGDailyRenter master = DailyRenters.Current;
            row.ContractID = master.ContractID;
            row.Type = "D";
        }


        protected virtual void KGDailyRenter_ContractID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGDailyRenter row = (KGDailyRenter)e.Row;
            KGDailyRenter master = DailyRenters.Current;
            master.UpdateProject = true;
        }
        protected virtual void KGDailyRenter_OrderNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGDailyRenter row = (KGDailyRenter)e.Row;

            POOrder order = PXSelectorAttribute.Select<POOrder.orderNbr>(sender, row) as POOrder;
            /*BAccount bAccount = getBAccount(order.VendorID);*/

            if (order == null)
            {
                sender.SetValueExt<KGDailyRenter.vendorID>(e.Row, null);
                sender.SetValueExt<KGDailyRenter.lineNbr>(e.Row, null);
            }
            else
            {
                sender.SetValueExt<KGDailyRenter.vendorID>(e.Row, order.VendorID);
                PXResultset<POLine> set = KGRenterApplyEntry.getPOLine(order.OrderNbr);
                POLine poLine = set;
                //set.RowCount
                if (poLine != null) {
                    //int a=       poLine.LineNbr.Value;
                    //set.Count 莫名的跳例外......
                    if (KGRenterApplyEntry.getPOLine(order.OrderNbr).Count == 1) {
                        sender.SetValueExt<KGDailyRenter.lineNbr>(e.Row, poLine.LineNbr);
                    }
                }

                //bAccount.AcctName;
            }
        }
        public static PXResultset<POLine> getPOLine(String orderNbr) {

            PXGraph graph = new PXGraph();

            PXResultset<POLine> set = PXSelect<POLine, Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                And<POLine.orderType, Equal<POOrderType.regularSubcontract>,
                And<POLine.curyUnbilledAmt, Greater<Zero>,
                And<POLine.cancelled, Equal<Zero>>>>>>.Select(graph, orderNbr);

            //PXResultset<POLine> set = PXSelect<POLine, Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>>.Select(graph, orderNbr);
            return set;
        }
        protected virtual void KGDailyRenter_WorkDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGDailyRenter master = DailyRenters.Current;
            DateTime workDate = (DateTime)e.NewValue;
            if (master.DailyRenterID < 0)
            {
                checkWorkDateMaster(workDate,true);
            }

        }


        public bool checkWorkDateMaster(DateTime? verifyDate,bool showErr) {

            KGDailyRenter master = DailyRenters.Current;
            bool ckeck = true;
            if (verifyDate != null) {
                DateTime workDate = verifyDate.Value;
                DateTime sysDate = (DateTime)this.Accessinfo.BusinessDate;
                DateTime today = new DateTime(sysDate.Year, sysDate.Month, sysDate.Day);
                KGSetUp kgSetup = KGSetup.Select();
                if (kgSetup.KGDailyRenterDateLimit == 0 || kgSetup.KGDailyRenterDateLimit == null)
                {
                    return true;
                }
                else {
                    Double limitDay = 0 - (Double)kgSetup.KGDailyRenterDateLimit;
                    DateTime fromDay = today.AddDays(limitDay);
                    if (workDate < fromDay || workDate > today)
                    {
                        if (showErr) {
                            String limitDayErr = String.Format("只允許建立系統日及系統日{0}的點工單", limitDay);
                            DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.workDate>(
                                 master, workDate, new PXSetPropertyException(limitDayErr));
                        }
                        ckeck = false;
                    }
                }


            }
            return ckeck;
        }


        // MatrixMode="True"
        //229 T200
        protected virtual void KGDailyRenter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGDailyRenter row = (KGDailyRenter)e.Row;
            KGDailyRenter master = DailyRenters.Current;
            //PXEntryStatus status=   DailyRenters.Cache.GetStatus(row);
            //no change 一開始
            if (row.DailyRenterID < 0)
            {
                PXUIFieldAttribute.SetEnabled<KGDailyRenter.workDate>(DailyRenters.Cache, DailyRenters.Current, true);
            }
            else {
                PXUIFieldAttribute.SetEnabled<KGDailyRenter.workDate>(DailyRenters.Cache, DailyRenters.Current, false);
            }
            

            if (row.Status == null || KGDailyRenter.OPEN.Equals(row.Status)) {
                bool editable=     checkWorkDateMaster(master.WorkDate, false);
                setEnable(editable);
            }
            else {
                setEnable(false);
            }
            master.UpdateProject = false;

        }

        public void setEnable(bool enable) {

            Delete.SetEnabled(enable);
            KGDailyRenter master = DailyRenters.Current;
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.contractID>(DailyRenters.Cache, master, enable);
            //PXUIFieldAttribute.SetEnabled<KGDailyRenter.workDate>(DailyRenters.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.orderNbr>(DailyRenters.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.lineNbr>(DailyRenters.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.reqQty>(DailyRenters.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.reqSelfQty>(DailyRenters.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.reqInsteadQty>(DailyRenters.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.reqWorkContent>(DailyRenters.Cache, master, enable);
            DailyRenterVendors.Cache.AllowInsert = enable;
            DailyRenterVendors.Cache.AllowUpdate = enable;
            DailyRenterVendors.Cache.AllowDelete = enable;

            if (master.UpdateProject != true || master.UpdateProject == null)
            {
                DailyRenterVendors.Cache.AllowInsert = enable;
                DailyRenterActVendors.Cache.AllowUpdate = enable;
                DailyRenterActVendors.Cache.AllowDelete = enable;
            }
            else {
                DailyRenters.Cache.SetValueExt<KGDailyRenter.vendorID>(master, null);
                DailyRenters.Cache.SetValueExt<KGDailyRenter.lineNbr>(master, null);
                DailyRenters.Cache.SetValueExt<KGDailyRenter.orderNbr>(master, null);
                DailyRenters.Cache.SetValueExt<KGDailyRenter.reqQty>(master, new Decimal(0));
                DailyRenters.Cache.SetValueExt<KGDailyRenter.reqInsteadQty>(master, new Decimal(0));
                DailyRenters.Cache.SetValueExt<KGDailyRenter.reqSelfQty>(master, new Decimal(0));
                DailyRenters.Cache.SetValueExt<KGDailyRenter.reqWorkContent>(master, null);
                foreach (KGDailyRenterVendor vendor in DailyRenterVendors.Select())
                {
                    DailyRenterVendors.Delete(vendor);
                    //Actions.Remove(vendor);
                }
            }

        }
        public override void Persist()
        {
            KGDailyRenter master = DailyRenters.Current;
            //代表刪除
            if (master == null)
            {
                base.Persist();
            }
            else
            {

                if (!beforeSaveCheck())
                {
                    return;
                }
                setInsertData(master);
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    //throw new Exception();
                    DailyRenters.Update(DailyRenters.Current);
                    DailyRenters.Cache.SetStatus(DailyRenters.Current,PXEntryStatus.Notchanged);
                    DailyRenters.Cache.IsDirty = false;
                    //PXUIFieldAttribute.SetEnabled<KGDailyRenter.workDate>(DailyRenters.Cache, DailyRenters.Current, false);

                    ts.Complete();
                }
            }
        }
        public void setInsertData(KGDailyRenter master)
        {
            //if (master.DailyRenterID < 0)
            //{
                if (master.Status == null) {
                    master.Status = KGDailyRenter.OPEN;
                }
                //Act實際-調整By移除Approve
                master.ActQty = master.ReqQty;
                master.ActInsteadQty = master.ReqInsteadQty;
                master.ActSelfQty = master.ReqSelfQty;
            //}
            //else {
           // }

           

            /*
            foreach (KGDailyRenterVendor line in DailyRenterVendors.Select())
            {
                KGDailyRenterVendorAct vendor = (KGDailyRenterVendorAct)DailyRenterActVendors.Cache.CreateInstance();
                vendor.Type = "A";
                vendor.OrderNbr = line.OrderNbr;
                vendor.ContractID = line.ContractID;
                vendor.LineNbr = line.LineNbr;
                vendor.VendorID = line.VendorID;
                vendor.WorkContent = line.WorkContent;
                vendor.InsteadQty = line.InsteadQty;
                line.Status = KGDailyRenter.APPROVED;
                vendor.Status = line.Status;
                DailyRenterActVendors.Insert(vendor);
                DailyRenterVendors.Update(line);
            }*/

            foreach (KGDailyRenterVendor line in DailyRenterVendors.Select())
            {
                line.Type = "D";
                if (line.Status == null) {
                    line.Status = KGDailyRenter.OPEN;
                }
                POLine poline = getPOLine(master.LineNbr, master.OrderNbr);
                line.Amount = line.InsteadQty.Value * poline.CuryUnitCost;
                //add by louis for default confirmQty
                line.ConfirmQty = line.InsteadQty;
                line.ContractID = master.ContractID;

            }


        }
        public bool beforeSaveCheck()
        {
            bool check = true;
            KGDailyRenter master = DailyRenters.Current;
            //------------popup-------Exception---------------
            /*
            if (DailyRenterVendors.Select() == null || DailyRenterVendors.Select().Count == 0) {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.reqQty>(
                    master, master.ReqQty, new PXSetPropertyException("請輸入代辦扣款廠商資訊且需求數量必須大於0"));
                check = false;
            }*/
            //-------------------------------------------------------

            check = DAC.Utils.RaiseNullCheck<KGDailyRenter.contractID>(DailyRenters.Cache, master, master.ContractID, "ContractID") && check;
            check = DAC.Utils.RaiseNullCheck<KGDailyRenter.orderNbr>(DailyRenters.Cache, master, master.OrderNbr, "OrderNbr") && check;
            check = DAC.Utils.RaiseNullCheck<KGDailyRenter.lineNbr>(DailyRenters.Cache, master, master.LineNbr, "LineNbr") && check;
            check = DAC.Utils.RaiseNullCheck<KGDailyRenter.workDate>(DailyRenters.Cache, master, master.WorkDate, "WorkDate") && check;
            check = check && DAC.Utils.RaiseNullCheck<KGDailyRenter.reqWorkContent>(DailyRenters.Cache, master, master.ReqWorkContent, "ReqWorkContent") && check; ;
            if (master.ReqQty == null || master.ReqQty <= 0)
            {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.reqQty>(
                     master, master.ReqQty, new PXSetPropertyException(Message.ReqQtyNumberError));

                check = false;
            }
            if (master.ReqSelfQty == null || master.ReqSelfQty < 0)
            {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.reqSelfQty>(
                     master, master.ReqSelfQty, new PXSetPropertyException(Message.ReqSelfQtyNullOrNegativeError));

                check = false;
            }
            if (master.ReqInsteadQty == null || master.ReqInsteadQty < 0)
            {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.reqInsteadQty>(
                     master, master.ReqInsteadQty, new PXSetPropertyException(Message.ReqInsteadQtyNullOrNegativeError));

                check = false;
            }
            else {
                //檢查加總KGDailyRenter.reqInsteadQty
                Decimal sum = new Decimal(0);
                foreach (KGDailyRenterVendor line in DailyRenterVendors.Select())
                {
                    if (line.InsteadQty != null && line.InsteadQty >= 0)
                    {
                        sum = sum + line.InsteadQty.Value;
                    }
                }
                //加總比較
                if (sum != master.ReqInsteadQty)
                {
                    DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.reqInsteadQty>(
                         master, master.ReqInsteadQty, new PXSetPropertyException(Message.InsteadQtyNullOrNegativeError));
                    check = false;
                }
            }

            foreach (KGDailyRenterVendor line in DailyRenterVendors.Select())
            {
                check = DAC.Utils.RaiseNullCheck<KGDailyRenterVendor.vendorID>(DailyRenterVendors.Cache, line, line.VendorID, "Vendor") && check;
                check = DAC.Utils.RaiseNullCheck<KGDailyRenterVendor.workContent>(DailyRenterVendors.Cache, line, line.WorkContent, "WorkContent") && check;
                if (line.InsteadQty == null || line.InsteadQty < 0)
                {
                    DailyRenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.insteadQty>(line, line.InsteadQty,
                       new PXSetPropertyException(Message.VendorInsteadQtyNullOrNegativeError));
                    check = false;
                }
            }

            if (master.ReqInsteadQty != null && master.ReqSelfQty != null && master.ReqQty != null) {
                if (master.ReqQty != (master.ReqInsteadQty + master.ReqSelfQty)) {
                    DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.reqQty>(
                     master, master.ReqQty, new PXSetPropertyException(Message.WorkDateError));

                    check = false;
                }
            }
            //代表新增才跑
            if (master.DailyRenterID < 0)
            {
                check = check && checkWorkDateMaster(master.WorkDate, true);

                PXGraph tempGraph = new PXGraph();
                KGDailyRenter renter = PXSelect<KGDailyRenter, Where<KGDailyRenter.contractID, Equal<Required<KGDailyRenter.contractID>>,
                    And<KGDailyRenter.orderNbr, Equal<Required<KGDailyRenter.orderNbr>>,
                    And<KGDailyRenter.lineNbr, Equal<Required<KGDailyRenter.lineNbr>>,
                And<KGDailyRenter.workDate, Equal<Required<KGDailyRenter.workDate>>>>>>>.Select(tempGraph, master.ContractID, master.OrderNbr, master.LineNbr, master.WorkDate);
                if (renter != null) {
                    DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.workDate>(
                           master, master.WorkDate, new PXSetPropertyException(Message.WorkDateError));
                    check = false;
                }

            }
            return check;
        }

        public static PXResultset<POLine> getPOLine(int? lineNbr, String orderNbr)
        {
            PXGraph graph = new PXGraph();
            PXResultset<POLine> set = PXSelect<POLine,
                    Where<POLine.lineNbr, Equal<Required<POLine.lineNbr>>,
                        And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>
                        >>.Select(graph, lineNbr, orderNbr);

            return set;
        }
        public static PXResultset<BAccount> getBAccount(int? bAccountID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<BAccount> set = PXSelect<BAccount,
                    Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>
                        >>.Select(graph, bAccountID);

            return set;
        }
        public class Message{
            public static String WorkDateError = "該專案同廠商合約項目下日期重覆";
            public static String ReqQtyError = "ReqQty not equals with selfQty + insteadQty";
            public static String VendorInsteadQtyNullOrNegativeError = "Vendor InsteadQty can't be null or negative.";
            public static String InsteadQtyNullOrNegativeError = "InsteadQty is not equal with vendor insteadQty";
            public static String ReqInsteadQtyNullOrNegativeError = "ReqInsteadQty can't be null or negative.";
            public static String ReqSelfQtyNullOrNegativeError = "ReqSelfQty can't be null or negative.";
            public static String ReqQtyNumberError = "需求數量必須大於0";
        }

    }
}