using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.RQ;
using PX.Objects.CR;
using PX.Data.BQL;
namespace Kedge
{
    //GI000117 Test
    public class KGDailyRenterActualEntry : PXGraph<KGDailyRenterActualEntry, KGDailyRenter>
    {

        public class OPEN : Constant<String>
        {
            public OPEN()
            : base("O")
            {
            }
        }
        public class DEFAULT : Constant<String>
        {
            public DEFAULT()
            : base("D")
            {
            }
        }
        public class ACCTUALLY : Constant<String>
        {
            public ACCTUALLY()
            : base("A")
            {
            }
        }
        public class PENDINGONSITE : Constant<String>
        {
            public PENDINGONSITE()
            : base("E")
            {
            }
        }
        public class ONSITE : Constant<String>
        {
            public ONSITE()
            : base("S")
            {
            }
        }
        public KGDailyRenterActualEntry()
        {
            DailyRenterReqVendors.AllowInsert = DailyRenterReqVendors.AllowUpdate = DailyRenterReqVendors.AllowDelete = false;
            //DailyRenters.AllowInsert = DailyRenters.AllowDelete = false;
            //Delete.SetEnabled(false);
            Insert.SetEnabled(false);
            ConfirmAction.SetEnabled(false);
        }


        public PXAction<KGDailyRenter> ConfirmAction;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Confirm")]
        protected void confirmAction()
        {
            WebDialogResult result = DailyRenters.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("點工單確認後, 將無法修改!"),
                MessageButtons.YesNo, MessageIcon.Warning, true);
            //checking answer	
            if (result != WebDialogResult.Yes) return;
            if (!beforeSaveCheck())
            {
                return;
            }
            Actions.PressSave();
            //PXLongOperation.StartOperation(this, delegate ()
            //{
            KGDailyRenter master = DailyRenters.Current;
            master.Status = KGDailyRenter.ON_SITE;
            DailyRenters.Update(master);
            foreach (KGDailyRenterVendorAct line in DailyRenterActVendors.Select())
            {
                line.Status = KGDailyRenter.ON_SITE;
                DailyRenterActVendors.Update(line);
            }
            Actions.PressSave();
            //});
        }

        public sealed class Zero : BqlType<IBqlDecimal, Decimal>.Constant<Zero>
        {
            public Zero() : base(new decimal(0))
            {
            }
        }
        //public PXSelect<KGDailyRenter, Where<KGDailyRenter.status, In3<OPEN, PENDINGONSITE, ONSITE>, And<KGDailyRenter.reqInsteadQty, Greater<Zero>>>> DailyRenters;
        //public PXSelect<KGDailyRenter, Where<KGDailyRenter.status, In3<OPEN, PENDINGONSITE, ONSITE>>> DailyRenters;
        public PXSelectJoin<KGDailyRenter, InnerJoin<PMProject, On<KGDailyRenter.contractID,
            Equal<PMProject.contractID>>>,
            Where <KGDailyRenter.status, In3<OPEN, PENDINGONSITE, ONSITE>,
            And <Match<PMProject, Current<AccessInfo.userName>>>
            >> DailyRenters;


        public PXSelect<KGDailyRenterVendor,
                 Where<KGDailyRenterVendor.dailyRenterID,
                 Equal<Current<KGDailyRenter.dailyRenterID>>,
                 And<KGDailyRenterVendor.type, Equal<DEFAULT>>>> DailyRenterReqVendors;


        public PXSelect<KGDailyRenterVendorAct,
             Where<KGDailyRenterVendorAct.dailyRenterID,
             Equal<Current<KGDailyRenter.dailyRenterID>>,
             And<KGDailyRenterVendorAct.type, Equal<KGDailyRenterActualEntry.ACCTUALLY>>>> DailyRenterActVendors;

        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "Daily Renter CD")]
        [PXDefault()]
        //ACCTUALLY=APPROVED
        [PXSelector(typeof(Search2<KGDailyRenter.dailyRenterCD, InnerJoin<PMProject, On<KGDailyRenter.contractID,
            Equal<PMProject.contractID>>>,
            Where<KGDailyRenter.status, In3<OPEN, PENDINGONSITE, ONSITE>,
            And<Match<PMProject, Current<AccessInfo.userName>>>>>)
        , typeof(KGDailyRenter.workDate), typeof(KGDailyRenter.dailyRenterCD),
        typeof(KGDailyRenter.orderNbr), typeof(KGDailyRenter.lineNbr), typeof(KGDailyRenter.reqWorkContent))]
        protected void KGDailyRenter_DailyRenterCD_CacheAttached(PXCache sender)
        {

        }
        [PXDBDate()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Request Date")]
        protected void KGDailyRenter_WorkDate_CacheAttached(PXCache sender)
        {

        }
        //動態調整數字

        protected virtual void KGDailyRenterVendorAct_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            doSum();
        }
        /*
        protected virtual void KGDailyRenterVendorAct_RowUpdated(PXCache sender,PXRowUpdatedEventArgs e)
        {
            doSum();
        }*/
        protected virtual void KGDailyRenterVendorAct_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            doSum();
        }
        public void doSum()
        {
            Decimal? sum = 0;
            foreach (KGDailyRenterVendorAct line in DailyRenterActVendors.Select())
            {
                sum = sum + line.InsteadQty;
            }
            KGDailyRenter master = DailyRenters.Current;
            master.ActInsteadQty = sum;
            master.ActSelfQty = master.ActQty - master.ActInsteadQty;
            DailyRenters.Update(master);
        }

        protected virtual void KGDailyRenter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGDailyRenter row = (KGDailyRenter)e.Row;
            if (row == null) {
                return;
            }

            if (row.Status == null)
            {
                ConfirmAction.SetEnabled(false);
                setEnable(false);
            }
            else if (KGDailyRenter.OPEN.Equals(row.Status))
            {
                ConfirmAction.SetEnabled(true);
                setEnable(true);

                if (DailyRenterActVendors.Select().Count == 0 && PXEntryStatus.Notchanged == DailyRenters.Cache.GetStatus(DailyRenters.Current))
                {
                    foreach (KGDailyRenterVendor line in DailyRenterReqVendors.Select())
                    {
                        KGDailyRenterVendorAct vendor = (KGDailyRenterVendorAct)DailyRenterActVendors.Cache.CreateInstance();
                        vendor.Type = "A";
                        vendor.OrderNbr = line.OrderNbr;
                        vendor.ContractID = line.ContractID;
                        //vendor.LineNbr = line.LineNbr;
                        vendor.VendorID = line.VendorID;
                        vendor.WorkContent = line.WorkContent;
                        vendor.InsteadQty = line.InsteadQty;
                        line.Status = KGDailyRenter.PENDING_ON_SITE;
                        vendor.Status = line.Status;
                        //DailyRenters.Cache.SetStatus(DailyRenters.Current, PXEntryStatus.Modified);
                        DailyRenterActVendors.Insert(vendor);

                    }
                }

            }
            else if (KGDailyRenter.PENDING_ON_SITE.Equals(row.Status))
            {
                setEnable(true);
                ConfirmAction.SetEnabled(true);
            }
            else if (KGDailyRenter.ON_SITE.Equals(row.Status))
            {
                setEnable(false);
                ConfirmAction.SetEnabled(false);
            }
            else
            {
                ConfirmAction.SetEnabled(false);
            }

            if (row.ReqInsteadQty > 0)
            {
                if (DailyRenterActVendors.Select() == null || DailyRenterActVendors.Select().Count == 0)
                {
                    ConfirmAction.SetEnabled(false);
                }
            }

            //2020/09/15 Mantis: 0011685
            Delete.SetEnabled(row.Status == "O" || row.Status == "E");
            //Delete.SetEnabled(false);
        }
        public void setEnable(bool enable)
        {
            Delete.SetEnabled(enable);
            KGDailyRenter master = DailyRenters.Current;
            //PXUIFieldAttribute.SetEnabled<KGDailyRenter.actQty>(DailyRenters.Cache, master, enable);

            //PXUIFieldAttribute.SetEnabled<KGDailyRenter.actSelfQty>(DailyRenters.Cache, master, enable);
            //PXUIFieldAttribute.SetEnabled<KGDailyRenter.actInsteadQty>(DailyRenters.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenter.actWorkContent>(DailyRenters.Cache, master, enable);
            DailyRenterActVendors.Cache.AllowInsert = enable;
            DailyRenterActVendors.Cache.AllowUpdate = enable;
            DailyRenterActVendors.Cache.AllowDelete = enable;
        }


        protected virtual void KGDailyRenterVendorAct_InsteadQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            doSum();
        }
        protected virtual void KGDailyRenterVendorAct_OrderNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGDailyRenterVendorAct row = (KGDailyRenterVendorAct)e.Row;
            POOrder order = PXSelectorAttribute.Select<POOrder.orderNbr>(sender, row) as POOrder;

            //bAccount.AcctName;
            if (order == null)
            {
                sender.SetValueExt<KGDailyRenterVendorAct.vendorID>(e.Row, null);
                //sender.SetValueExt<KGDailyRenterVendorAct.lineNbr>(e.Row, null);
            }
            else
            {
                sender.SetValueExt<KGDailyRenterVendorAct.vendorID>(e.Row, order.VendorID);
                //一筆塞POLine的功能
                /*PXResultset<POLine> set = KGRenterApplyEntry.getPOLine(order.OrderNbr);
                POLine poLine = set;
                //set.RowCount
                if (poLine != null)
                {
                    //int a=       poLine.LineNbr.Value;
                    //set.Count 莫名的跳例外......
                    if (KGRenterApplyEntry.getPOLine(order.OrderNbr).Count == 1)
                    {
                        sender.SetValueExt<KGDailyRenterVendorAct.lineNbr>(e.Row, poLine.LineNbr);
                    }

                }*/
                //bAccount.AcctName;
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
                    ts.Complete();
                }
            }
        }

        public void setInsertData(KGDailyRenter master)
        {
            if (master.DailyRenterID < 0)
            {


            }
            else
            {

                if (master.Status != KGDailyRenter.ON_SITE)
                {
                    master.Status = KGDailyRenter.PENDING_ON_SITE;
                }
                foreach (KGDailyRenterVendorAct line in DailyRenterActVendors.Cache.Inserted)
                {
                    line.Type = "A";
                    line.Status = KGDailyRenter.PENDING_ON_SITE;
                    POLine poline = KGRenterApplyEntry.getPOLine(master.LineNbr, master.OrderNbr);
                    line.Amount = line.InsteadQty.Value * poline.CuryUnitCost;
                    //add by louis for default confirmQty
                    line.ConfirmQty = line.InsteadQty;
                    line.ContractID = master.ContractID;
                }

                foreach (KGDailyRenterVendorAct line in DailyRenterActVendors.Cache.Updated)
                {
                    POLine poline = KGRenterApplyEntry.getPOLine(master.LineNbr, master.OrderNbr);
                    line.Amount = line.InsteadQty.Value * poline.CuryUnitCost;
                    //add by louis for default confirmQty
                    line.ConfirmQty = line.InsteadQty;
                    line.ContractID = master.ContractID;
                }
            }
        }

        public bool beforeSaveCheck()
        {
            bool check = true;
            KGDailyRenter master = DailyRenters.Current;
            if (master.ReqInsteadQty > 0) {
                if (DailyRenterActVendors.Select() == null || DailyRenterActVendors.Select().Count == 0)
                {
                    throw new PXException("請輸入實際代辦扣款廠商資訊");
                }
            }
            if (master.ActWorkContent == null)
            {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.actWorkContent>(
                     master, master.ActWorkContent, new PXSetPropertyException("ActWorkContent can't be null."));

                check = false;
            }
            if (master.ActQty == null || master.ActQty < 0)
            {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.actQty>(
                     master, master.ActQty, new PXSetPropertyException("ActQty can't be null or negative"));

                check = false;
            }
            if (master.ActSelfQty == null || master.ActSelfQty < 0)
            {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.actInsteadQty>(
                     master, master.ActInsteadQty, new PXSetPropertyException("實際代辦數量加總超過需求數量"));

                check = false;
            }
            if (master.ActInsteadQty == null || master.ActInsteadQty < 0)
            {
                DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.actInsteadQty>(
                     master, master.ActInsteadQty, new PXSetPropertyException("ActInsteadQty can't be null or negative"));

                check = false;
            }
            else
            {
                //檢查加總KGDailyRenter.actInsteadQty
                Decimal sum = new Decimal(0);
                foreach (KGDailyRenterVendorAct line in DailyRenterActVendors.Select())
                {
                    if (line.InsteadQty != null && line.InsteadQty >= 0)
                    {
                        sum = sum + line.InsteadQty.Value;
                    }
                    else
                    {
                        DailyRenterActVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendorAct.insteadQty>(line, line.InsteadQty,
                            new PXSetPropertyException("Vendor InsteadQty can't be null or negative"));
                        check = false;
                    }


                }

                if (sum != master.ActInsteadQty)
                {
                    DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.actInsteadQty>(
                         master, master.ActInsteadQty, new PXSetPropertyException("ActInsteadQty is not equal with vendor insteadQty"));
                    check = false;
                }
            }


            foreach (KGDailyRenterVendorAct line in DailyRenterActVendors.Select())
            {
                if (line.OrderNbr == null)
                {
                    DailyRenterActVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendorAct.orderNbr>(line, line.OrderNbr,
                        new PXSetPropertyException("實際到工廠商合約不能為空值"));
                    check = false;
                }
                if (line.InsteadQty == null)
                {
                    /* DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.actInsteadQty>(
                         master, master.ActInsteadQty, new PXSetPropertyException("ActInsteadQty is not equal with vendor insteadQty"));*/
                    DailyRenterActVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendorAct.insteadQty>(line, line.InsteadQty,
                   new PXSetPropertyException("InsteadQty can't be null."));
                    check = false;
                }
                if (line.WorkContent == null)
                {
                    DailyRenterActVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendorAct.workContent>(line, line.WorkContent,
                       new PXSetPropertyException("WorkContent can't be null."));
                    check = false;
                }
            }
            PXResultset<KGDailyRenterVendor> RenterVendor =
                PXSelect<KGDailyRenterVendor,
                Where<KGDailyRenterVendor.dailyRenterID, Equal<Required<KGDailyRenter.dailyRenterID>>,
                And<KGDailyRenterVendor.type,Equal<ACCTUALLY>>>>
                .Select(this, master.DailyRenterID);
            foreach(KGDailyRenterVendor vendor in DailyRenterActVendors.Select())
            {
                //2019/11/29 Add By Althea 加入未計價金額檢核
                KGRenterConfirmProcess confirmProcess = new KGRenterConfirmProcess();
                if(!confirmProcess.CreateValuation(vendor, "1"))
                {
                    check = false;
                    throw new Exception("請確認" + vendor.OrderNbr + "有足夠的扣款金額!");
                   
                }
      
            }

            if (master.ActInsteadQty != null && master.ActSelfQty != null && master.ActQty != null)
            {
                if (master.ActQty != (master.ActInsteadQty + master.ActSelfQty))
                {
                    DailyRenters.Cache.RaiseExceptionHandling<KGDailyRenter.actInsteadQty>(
                        master, master.ActQty, new PXSetPropertyException("ActQty not equals with ActselfQty + ActinsteadQty"));

                    check = false;
                }
            }

            return check;
        }


    }
}