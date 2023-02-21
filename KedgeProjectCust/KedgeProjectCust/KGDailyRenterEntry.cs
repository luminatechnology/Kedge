using System;
using System.Collections.Generic;
using System.Text;
using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.PO;

namespace Kedge
{
    public class KGDailyRenterEntry : PXGraph<KGDailyRenterEntry, KGDailyRenterM>
    {
        public KGDailyRenterEntry()
        {
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.ApproveAction);
            ApproveAction.SetEnabled(false);
        }

        public PXSetup<KGSetUp> KGSetup;

        #region Select
        [PXFilterable]
        public PXSelect<KGDailyRenterM> DailyRenterM;

        public PXSelect<KGDailyRenterVendorM,
                        Where<KGDailyRenterVendorM.dailyRenterID, Equal<Current<KGDailyRenterM.dailyRenterID>>>> DailyRenterVendorM;
        #endregion

        #region Button
        public PXAction<KGDailyRenterM> ActionMenu;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Action")]
        protected void actionMenu() { }
        public PXAction<KGDailyRenterM> ApproveAction;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Approve")]
        protected void approveAction()
        {
            if (!beforeSaveCheck()) { return; }

            Actions.PressSave();

            PXLongOperation.StartOperation(this, delegate()
            {
                KGDailyRenterM kGDailyRenterM = DailyRenterM.Current;

                kGDailyRenterM.Status        = KGDailyRenter.APPROVED;
                kGDailyRenterM.ActQty        = kGDailyRenterM.ReqQty;
                kGDailyRenterM.ActSelfQty    = kGDailyRenterM.ReqSelfQty;
                kGDailyRenterM.ActInsteadQty = kGDailyRenterM.ReqInsteadQty;

                DailyRenterM.Update(kGDailyRenterM);

                Actions.PressSave();
            });
        }
        #endregion

        #region Event Handlers
        public override void Persist()
        {
            KGDailyRenterM  kGDailyRenterM = DailyRenterM.Current;

            if (kGDailyRenterM == null)
            {
                base.Persist();
            }
            else
            {
                if (!beforeSaveCheck()) { return; }

                setInsertData(kGDailyRenterM);

                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    ts.Complete();
                }
            }
        }

        protected void _(Events.FieldUpdated<KGDailyRenterM.contractID> e)
        {
            KGDailyRenterM row = (KGDailyRenterM)e.Row;

            DailyRenterVendorM.Cache.Clear();

            e.Cache.SetValueExt<KGDailyRenterM.vendorID>(e.Row, null);
            e.Cache.SetValueExt<KGDailyRenterM.lineNbr>(e.Row, null);
            e.Cache.SetValueExt<KGDailyRenterM.orderNbr>(e.Row, null);
            e.Cache.SetValueExt<KGDailyRenterM.reqQty>(e.Row, new Decimal(0));
            e.Cache.SetValueExt<KGDailyRenterM.reqInsteadQty>(e.Row, new Decimal(0));
            e.Cache.SetValueExt<KGDailyRenterM.reqSelfQty>(e.Row, new Decimal(0));
            e.Cache.SetValueExt<KGDailyRenterM.reqWorkContent>(e.Row, null);
        }

        protected void _(Events.FieldUpdated<KGDailyRenterM.orderNbr> e)
        {
            KGDailyRenterM row = (KGDailyRenterM)e.Row;

            POOrder order = PXSelectorAttribute.Select<POOrder.orderNbr>(e.Cache, row) as POOrder;

            if (order == null)
            {
                e.Cache.SetValueExt<KGDailyRenterM.vendorID>(e.Row, null);
                e.Cache.SetValueExt<KGDailyRenterM.lineNbr>(e.Row, null);
            }
            else
            {
                e.Cache.SetValueExt<KGDailyRenterM.vendorID>(e.Row, order.VendorID);
            }
        }

        protected void _(Events.FieldUpdated<KGDailyRenterM.hold> e)
        {
            KGDailyRenterM row = (KGDailyRenterM)e.Row;

            if (row.Hold == true)
            {
                row.Status = KGDailyRenter.HOLD;

                foreach (KGDailyRenterVendorM line in DailyRenterVendorM.Cache.Inserted)
                {
                    line.Status = KGDailyRenter.HOLD;
                }
            }
            else
            {
                row.Status = KGDailyRenter.PENDING_APPROVE;

                foreach (KGDailyRenterVendorM line in DailyRenterVendorM.Cache.Inserted)
                {
                    line.Status = KGDailyRenter.PENDING_APPROVE;
                }
            }
        }

        protected void _(Events.FieldVerifying<KGDailyRenterM.workDate> e)
        {
            KGDailyRenterM kGDailyRenterM = DailyRenterM.Current;

            DateTime workDate = (DateTime)e.NewValue;

            if (kGDailyRenterM.DailyRenterID < 0)
            {
                checkWorkDateMaster(workDate);
            }
        }

        protected void _(Events.RowSelected<KGDailyRenterM> e)
        {
            KGDailyRenterM row = (KGDailyRenterM)e.Row;

            if (row.DailyRenterID < 0)
            {
                PXUIFieldAttribute.SetEnabled<KGDailyRenterM.workDate>(DailyRenterM.Cache, DailyRenterM.Current, true);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled<KGDailyRenterM.workDate>(DailyRenterM.Cache, DailyRenterM.Current, false);
            }

            if (row.Status == null || KGDailyRenter.HOLD.Equals(row.Status))
            {
                setEnable(true);
                ApproveAction.SetEnabled(false);
            }
            else if (KGDailyRenter.PENDING_APPROVE.Equals(row.Status))
            {
                setEnable(false);
                ApproveAction.SetEnabled(true);
            }
            else if (KGDailyRenter.APPROVED.Equals(row.Status))
            {
                Save.SetEnabled(false);
                setEnable(false);
                ApproveAction.SetEnabled(false);
                PXUIFieldAttribute.SetEnabled<KGDailyRenterM.hold>(DailyRenterM.Cache, DailyRenterM.Current, false);
            }
            else
            {
                Save.SetEnabled(false);
                setEnable(false);
                ApproveAction.SetEnabled(false);
            }
        }
        #endregion

        #region Functions
        // Not to be used in the future
        /*private void generateVendLookup()
        {
            List<int> value = new List<int>();
            List<string> label = new List<string>();

            foreach (Vendor vendor in PXSelectJoinGroupBy<Vendor,
                                                          LeftJoin<POOrder, On<Vendor.bAccountID, Equal<POOrder.vendorID>>>,
                                                          Where<POOrder.projectID, Equal<Required<KGDailyRenterM.contractID>>,
                                                            And<POOrder.orderNbr, NotEqual<Required<KGDailyRenterM.orderNbr>>>>,
                                                          Aggregate<GroupBy<Vendor.bAccountID,
                                                                    GroupBy<Vendor.acctCD>>>>
                                                          .Select(this, DailyRenterM.Current.ContractID, DailyRenterM.Current.OrderNbr))
            {
                value.Add((int)vendor.BAccountID);

                StringBuilder bld = new StringBuilder(vendor.AcctCD.ToString());
                bld.Append("- ");
                bld.Append(vendor.AcctName);
                label.Add(bld.ToString());
            }

            PXIntListAttribute.SetList<KGDailyRenterVendorM.vendorID>(DailyRenterVendorM.Cache, null, value.ToArray(), label.ToArray());
        }*/

        public bool checkWorkDateMaster(DateTime? verifyDate)
        {
            KGDailyRenterM kGDailyRenterM = DailyRenterM.Current;

            bool ckeck = true;

            if (verifyDate != null)
            {
                DateTime workDate = verifyDate.Value;
                DateTime today    = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                if (workDate < today)
                {
                    DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenterM.workDate>(kGDailyRenterM, workDate, 
                                                                                       new PXSetPropertyException("¤£¥i¥Ó½Ð¤µ¤Ñ«eªºÂI¤uªí"));
                    ckeck = false;
                }
            }
            return ckeck;
        }

        public void setEnable(bool enable)
        {
            Delete.SetEnabled(enable);

            KGDailyRenterM kGDailyRenterM = DailyRenterM.Current;

            PXUIFieldAttribute.SetEnabled<KGDailyRenterM.contractID>(DailyRenterM.Cache, kGDailyRenterM, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterM.orderNbr>(DailyRenterM.Cache, kGDailyRenterM, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterM.lineNbr>(DailyRenterM.Cache, kGDailyRenterM, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterM.reqQty>(DailyRenterM.Cache, kGDailyRenterM, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterM.reqSelfQty>(DailyRenterM.Cache, kGDailyRenterM, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterM.reqInsteadQty>(DailyRenterM.Cache, kGDailyRenterM, enable);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterM.reqWorkContent>(DailyRenterM.Cache, kGDailyRenterM, enable);

            DailyRenterVendorM.Cache.AllowInsert = enable;
            DailyRenterVendorM.Cache.AllowUpdate = enable;
            DailyRenterVendorM.Cache.AllowDelete = enable;
        }

        public void setInsertData(KGDailyRenterM master)
        {
            if (master.DailyRenterID < 0)
            {
                master.Status = KGDailyRenter.HOLD;
                master.ActQty = 0;
                master.ActInsteadQty = 0;
                master.ActSelfQty = 0;
            }
            else
            { }

            foreach (KGDailyRenterVendorM line in DailyRenterVendorM.Cache.Inserted)
            {
                line.Status = KGDailyRenter.HOLD;

                POLine poline = getPOLine(line.LineNbr, line.OrderNbr);

                if (poline == null)
                {
                    line.Amount = line.InsteadQty * 1;
                }
                else
                {
                    line.Amount = line.InsteadQty.Value * poline.CuryUnitCost;
                }

                line.ContractID = master.ContractID;
            }

            foreach (KGDailyRenterVendorM line in DailyRenterVendorM.Cache.Updated)
            {
                POLine poline = getPOLine(line.LineNbr, line.OrderNbr);

                if (poline == null)
                {
                    line.Amount = line.InsteadQty * 1;
                }
                else
                {
                    line.Amount = line.InsteadQty.Value * poline.CuryUnitCost;
                }

                line.ContractID = master.ContractID;
            }
        }

        public bool beforeSaveCheck()
        {
            bool check = true;

            KGDailyRenterM master = DailyRenterM.Current;

            //------------popup-------Exception---------------
            if (DailyRenterVendorM.Select() == null || DailyRenterVendorM.Select().Count == 0)
            {
                throw new PXException("請輸入代辦扣款廠商資訊");
            }

            //-------------------------------------------------------
            if (master.ContractID == null)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.contractID>(
                     master, master.ContractID, new PXSetPropertyException("ContractID can't be null."));

                check = false;
            }
            if (master.OrderNbr == null)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.orderNbr>(
                     master, master.OrderNbr, new PXSetPropertyException("OrderNbr can't be null."));

                check = false;
            }
            if (master.LineNbr == null)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.lineNbr>(
                     master, master.LineNbr, new PXSetPropertyException("LineNbr can't be null."));

                check = false;
            }
            if (master.WorkDate == null)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.workDate>(
                     master, master.WorkDate, new PXSetPropertyException("WorkDate can't be null."));

                check = false;
            }
            if (master.ReqWorkContent == null)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.reqWorkContent>(
                     master, master.ReqWorkContent, new PXSetPropertyException("ReqWorkContent can't be null."));

                check = false;
            }
            if (master.ReqQty == null || master.ReqQty < 0)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.reqQty>(
                     master, master.ReqQty, new PXSetPropertyException("ReqQty can't be null or negative."));

                check = false;
            }
            if (master.ReqSelfQty == null || master.ReqSelfQty < 0)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.reqSelfQty>(
                     master, master.ReqSelfQty, new PXSetPropertyException("ReqSelfQty can't be null or negative."));

                check = false;
            }
            if (master.ReqInsteadQty == null || master.ReqInsteadQty < 0)
            {
                DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.reqInsteadQty>(
                     master, master.ReqInsteadQty, new PXSetPropertyException("ReqInsteadQty can't be null or negative."));

                check = false;
            }
            else
            {
                Decimal sum = new Decimal(0);

                foreach (KGDailyRenterVendorM line in DailyRenterVendorM.Select())
                {
                    if (line.InsteadQty != null && line.InsteadQty >= 0)
                    {
                        sum = sum + line.InsteadQty.Value;
                    }
                    else
                    {
                        DailyRenterVendorM.Cache.RaiseExceptionHandling<KGDailyRenterVendor.insteadQty>(line, line.InsteadQty,
                            new PXSetPropertyException("Vendor InsteadQty can't be null or negative."));
                        check = false;
                    }
                    if (line.WorkContent == null)
                    {
                        DailyRenterVendorM.Cache.RaiseExceptionHandling<KGDailyRenterVendor.workContent>(line, line.WorkContent,
                            new PXSetPropertyException("WorkContent can't be null."));
                        check = false;
                    }
                }
                if (sum != master.ReqInsteadQty)
                {
                    DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.reqInsteadQty>(
                         master, master.ReqInsteadQty, new PXSetPropertyException("InsteadQty is not equal with vendor insteadQty"));
                    check = false;
                }
            }
            if (master.ReqInsteadQty != null && master.ReqSelfQty != null && master.ReqQty != null)
            {
                if (master.ReqQty != (master.ReqInsteadQty + master.ReqSelfQty))
                {
                    DailyRenterM.Cache.RaiseExceptionHandling<KGDailyRenter.reqQty>(
                     master, master.ReqQty, new PXSetPropertyException("ReqQty not equals with selfQty + insteadQty"));

                    check = false;
                }
            }
            if (master.DailyRenterID < 0)
            {
                check = check && checkWorkDateMaster(master.WorkDate);
            }

            return check;
        }
        
        public static PXResultset<POLine> getPOLine(int? lineNbr, String orderNbr)
        {
            PXGraph graph = new PXGraph();

            PXResultset<POLine> set = PXSelect<POLine,
                                               Where<POLine.lineNbr, Equal<Required<POLine.lineNbr>>,
                                                 And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>>>
                                              .Select(graph, lineNbr, orderNbr);
            return set;
        }

        public static PXResultset<BAccount> getBAccount(int? bAccountID)
        {
            PXGraph graph = new PXGraph();

            PXResultset<BAccount> set = PXSelect<BAccount,
                                                 Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                                                .Select(graph, bAccountID);
            return set;
        }
        #endregion
    }
}