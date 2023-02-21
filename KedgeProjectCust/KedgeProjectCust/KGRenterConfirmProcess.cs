using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CR;
using Kedge.DAC;
using PX.Objects.EP;
using System.Linq;
using static Kedge.KGDailyRenterActualEntry;
using PX.SM;
/**
 * ====2021-05-06: ====Alton
 * 將Qty來源改為 ConfirmQty
 * 
 * ===2021/11/03 === Althea
 * Modify 產生加扣款單時,工料編號AD一致
 * **/
namespace Kedge
{
    public class KGRenterConfirmProcess : PXGraph<KGRenterConfirmProcess>
    {
        #region Filter

        //2020/04/23 移除刪除 新增 複製貼上 上下一筆 按鈕
        public PXSave<RenterFilter> Save;
        public PXCancel<RenterFilter> Cancel;

        public PXFilter<RenterFilter> RenterFilter;

        //2020/02/11 ADD 過濾掉代辦數量為0的資料,因為不須確認
        public PXSelectJoin<KGDailyRenterVendor,
                    InnerJoin<KGDailyRenter,
                        On<KGDailyRenter.dailyRenterID,
                        Equal<KGDailyRenterVendor.dailyRenterID>,
                        And<KGDailyRenterVendor.type,Equal<word.a>>>,
                    InnerJoin<Users,
                        On<Users.pKID,
                        Equal<KGDailyRenterVendor.createdByID>>>>,
                    Where<KGDailyRenterVendor.insteadQty,Greater<word.zero>>,
                    OrderBy<Desc<KGDailyRenterVendor.dailyRenterID>>> RenterVendors;
       
        public PXSelect<KGDailyRenter, Where<KGDailyRenter.status, 
            In3<ACCTUALLY, PENDINGONSITE, ONSITE>>> DailyRenters;
        protected virtual IEnumerable renterVendors()
        {
            PXSelectJoin<KGDailyRenterVendor,
                   InnerJoin<KGDailyRenter,
                       On<KGDailyRenter.dailyRenterID,
                       Equal<KGDailyRenterVendor.dailyRenterID>,
                       And<KGDailyRenterVendor.type, Equal<word.a>>>,
                   InnerJoin<Users,
                       On<Users.pKID,
                       Equal<KGDailyRenterVendor.createdByID>>>>,
                   Where<KGDailyRenterVendor.insteadQty, Greater<word.zero>>,
                   OrderBy<Desc<KGDailyRenterVendor.dailyRenterID>>> query =
            new PXSelectJoin<KGDailyRenterVendor,
                    InnerJoin<KGDailyRenter,
                        On<KGDailyRenter.dailyRenterID,
                        Equal<KGDailyRenterVendor.dailyRenterID>,
                        And<KGDailyRenterVendor.type, Equal<word.a>>>,
                    InnerJoin<Users,
                        On<Users.pKID,
                        Equal<KGDailyRenterVendor.createdByID>>>>,
                    Where<KGDailyRenterVendor.insteadQty, Greater<word.zero>>,
                    OrderBy<Desc<KGDailyRenterVendor.dailyRenterID>>>(this);
            // Adding filtering conditions to the query
            RenterFilter filter = RenterFilter.Current;
            if (filter.ContractID != null)
                query.WhereAnd<Where<KGDailyRenterVendor.contractID,
                    Equal<Current<RenterFilter.contractID>>>>();
            if (filter.OrderNbr != null)
                query.WhereAnd<Where<KGDailyRenterVendor.orderNbr,
                    Equal<Current<RenterFilter.orderNbr>>>>();
            if (filter.WorkDateFrom != null)
                query.WhereAnd<Where<KGDailyRenter.workDate,
                    GreaterEqual<Current<RenterFilter.workDateFrom>>>>();
            if (filter.WorkDateTo != null)
                query.WhereAnd<Where<KGDailyRenter.workDate,
                    LessEqual<Current<RenterFilter.workDateTo>>>>();
            if (filter.VendorID != null)
                query.WhereAnd<Where<KGDailyRenterVendor.vendorID,
                    Equal<Current<RenterFilter.vendorID>>>>();
            if (filter.Status != null)
                query.WhereAnd<Where<KGDailyRenterVendor.status,
                    Equal<Current<RenterFilter.status>>>>();
            if (filter.CreatedByID != null)
                query.WhereAnd<Where<Users.username,
                    Equal<Current<RenterFilter.createdByID>>>>();
            return query.Select();
        }

        #endregion

        //2020/07/15 button改成PXLongOperation寫法

        #region Action
        /*public PXAction<RenterFilter> ActionMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Action")]
        protected void actionMenu() { }*/


        ///<summary>會簽按鈕邏輯</summary>
        ///1.存更改的確認數量(DailyRenterVendor)
        ///2.檢核更改的確認數量有沒有超標DailyRenter,
        ///3.更改狀態到已會簽
        ///4.寫入batchNbr標記報表欄位
        ///5..顯示報表
        ///6.只有"已到工"可使用此按鈕
       
        //2019/06/05錯誤訊息改為紅叉出現在row的前面
        #region SendApprovalAction
        public PXAction<RenterFilter> SendApprovalAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Print & Sign")]
        public virtual IEnumerable sendApprovalAction(PXAdapter adapter, [PXString]
            string reportID)
        {
            //KGDailyRenterVendor RV = RenterVendors.Current;
            //2019/06/17把Selected checkbox拿回來

            //先檢查狀態
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "S")
                        {
                            //2019/06/17檢查ConfirmQty是否為空值
                            checkConfirmQty(RenterVendors.Cache, kGDailyRenterVendor);
                            if (!CreateValuation(kGDailyRenterVendor, "1"))
                            {
                                setCheckThread = true;
                                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                                 kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("請確認有足夠的扣款金額!", PXErrorLevel.RowError));
                                //throw new Exception("請確認有足夠的扣款金額!");
                            }
                        }
                        else if (kGDailyRenterVendor.Status == "W")
                        {
                            setCheckThread = true;

                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已會簽」。" + "\n" + "若要解除，請執行「解除會簽」；" + "\n" + "若要簽認，請執行「簽認及上傳附件」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「已會簽」。" + "\n" + "若要解除，請執行「解除會簽」；" + "\n" + "若要簽認，請執行「簽認及上傳附件」!");

                        }
                        else if (kGDailyRenterVendor.Status == "C")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已簽認」。" + "\n" + "若要解除，請執行「解除簽認」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「已簽認」。" + "\n" + "若要解除，請執行「解除簽認」!");

                        }
                    }
                }
            }
            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {


                    KGRenterConfirmProcess graph = PXGraph.CreateInstance<KGRenterConfirmProcess>();
                    string batchnbr = graph.Accessinfo.UserName
                        + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                    foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                    {
                        if (dailyRenterVendor.Selected == true)
                        {
                            if (dailyRenterVendor.Status == "S")
                            {
                                if (dailyRenterVendor.ConfirmQty == 0)
                                {
                                    dailyRenterVendor.Status = "C";
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }
                                else if (dailyRenterVendor.ConfirmQty > 0)
                                {
                                    dailyRenterVendor.BatchNbr = batchnbr;
                                    dailyRenterVendor.Status = "W";
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }

                                //2020/07/15 廠商確認按鈕才要回壓代辦數量
                                /*KGDailyRenter dailyRenter =
                                                    PXSelect<KGDailyRenter,
                                                    Where<KGDailyRenter.dailyRenterID,
                                                    Equal<Required<KGDailyRenter.dailyRenterID>>>>
                                                    .Select(this, dailyRenterVendor.DailyRenterID);
                                //回壓代辦數量=確認數量 自辦數量=實際數量-代辦數量
                                dailyRenter.ActInsteadQty = dailyRenterVendor.ConfirmQty;
                                dailyRenter.ActSelfQty = dailyRenter.ActQty - dailyRenterVendor.ConfirmQty;
                                KGDailyRenters.Update(dailyRenter);*/

                            }
                        }
                    }
                    base.Persist();
                    KGRenterConfirmProcess pp = CreateInstance<KGRenterConfirmProcess>();
                    NewPageToDisplayReport(pp);
                });

               
            }
      

            #region 2019/05/30改成report開啟新頁面
            /*KGDailyRenterVendor DRV = RenterVendors.Current;
            if (reportID == null) reportID = "KG601000";
            if (DRV != null)
            {
                Dictionary<string, string> mailParams = new Dictionary<string, string>();
                mailParams["DailyRenterVendorID"] = DRV.DailyRenterVendorID.ToString();
                throw new PXReportRequiredException(mailParams, reportID, "Report");
            }*/
            #endregion
            return adapter.Get();
        }
        #endregion

        /// <summary>廠商簽認按鈕邏輯</summary>
        /// 1.把確認數量押回DailyRenter(Header)
        /// 2.產生加扣款單
        /// 3.更改狀態為已簽認
        /// 4.只有"已會簽"可以使用此按鈕
     
        #region VendorConfirmAction
        public PXAction<RenterFilter> VendorConfirmAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Vendor Confirm", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable vendorConfirmAction(PXAdapter adapter)
        {
            //先檢核
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "W")
                        {
                            //2020/09/15 Mantis: 0011686
                            //checkFileLength(RenterVendors.Cache, kGDailyRenterVendor);
                        }
                        else if (kGDailyRenterVendor.Status == "S")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已到工」。" + "\n" + "若要會簽，請執行「列印及會簽」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「未會簽」。" + "\n" + "若要會簽，請執行「列印及會簽」!");
                        }
                        else if (kGDailyRenterVendor.Status == "C")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                             kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已簽認」。" + "\n" + "若要解除，請執行「解除簽認」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「已簽認」。" + "\n" + "若要解除，請執行「解除簽認」!");
                        }
                    }
                }
            }

            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    //2020/04/06 考慮到多筆的dailyRenterVendor 先回壓更新數量後 再更新畫面上的status
                    //2020/11/04 Mantis:0011791  抓出未確認的實際"代辦"數量+已確認過的"確認"數量
                    //2020/11/05 用Transaction包
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                        {
                            if (dailyRenterVendor == null) return;
                            if(dailyRenterVendor.Status =="W" && dailyRenterVendor.Selected == true)
                            {

                                dailyRenterVendor.Status = "C";
                                dailyRenterVendor.Selected = false;                                
                                RenterVendors.Update(dailyRenterVendor);

                                CreateValuation(dailyRenterVendor, "2");

                                base.Persist();
                                //回壓代辦數量
                                decimal? confirmQty = 0;
                                decimal? unconfirmQty = 0;
                                //找出狀態為廠商確認的確認數量
                                KGDailyRenterVendor confirmRenterVendor =
                                    PXSelectGroupBy<KGDailyRenterVendor,
                                    Where<KGDailyRenterVendor.status, Equal<Required<KGDailyRenterVendor.status>>,
                                    And<KGDailyRenterVendor.insteadQty, Greater<word.zero>,
                                    And<KGDailyRenterVendor.type, Equal<word.a>,
                                    And<KGDailyRenterVendor.dailyRenterID, Equal<Required<KGDailyRenterVendor.dailyRenterID>>>>>>,
                                    Aggregate<GroupBy<KGDailyRenterVendor.dailyRenterID,
                                    Sum<KGDailyRenterVendor.confirmQty>>>>.Select(this, "C", dailyRenterVendor.DailyRenterID);
                                if (confirmRenterVendor != null)
                                    confirmQty = confirmRenterVendor.ConfirmQty;
                                else
                                    confirmQty = 0;

                                //找出狀態為已到工或會簽中的實際代辦數量
                                KGDailyRenterVendor unConfirmRenterVendor =
                                    PXSelectGroupBy<KGDailyRenterVendor,
                                    Where<KGDailyRenterVendor.insteadQty, Greater<word.zero>,
                                    And<KGDailyRenterVendor.type, Equal<word.a>,
                                    And<KGDailyRenterVendor.dailyRenterID, Equal<Required<KGDailyRenterVendor.dailyRenterID>>,                          
                                    And<Where<KGDailyRenterVendor.status, Equal<Required<KGDailyRenterVendor.status>>,
                                    Or<KGDailyRenterVendor.status, Equal<Required<KGDailyRenterVendor.status>>>>>>>>,
                                    Aggregate<GroupBy<KGDailyRenterVendor.dailyRenterID,
                                    Sum<KGDailyRenterVendor.insteadQty>>>>.Select(this,dailyRenterVendor.DailyRenterID,"S","W");
                                if (unConfirmRenterVendor != null)
                                    unconfirmQty = unConfirmRenterVendor.InsteadQty;
                                else
                                    unconfirmQty = 0;

                                UpdateDailyRenter(dailyRenterVendor, confirmQty+unconfirmQty);

                            }
                        }
                        ts.Complete();
                    }
                     
                });
        }
            
            return adapter.Get();


        }
        #endregion

        /// <summary>解除會簽按鈕邏輯</summary>
        /// 1.只有狀態為已會簽可以使用此按鈕
        /// 2.狀態改回已到工
        /// 3.清空batchNbr

        #region UnSignAction
        public PXAction<RenterFilter> UnSignAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "UnSign", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable unSignAction(PXAdapter adapter)
        {
            //先檢查狀態
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "S")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已到工」。" + "\n" + "若要會簽，請執行「列印及會簽」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「未會簽」。" + "\n" + "若要會簽，請執行「列印及會簽」!");
                        }
                        else if (kGDailyRenterVendor.Status == "C")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已簽認」。" + "\n" + "若要解除，請執行「解除簽認」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「已簽認」。" + "\n" + "若要解除，請執行「解除簽認」!");
                        }
                    }
                }
            }
            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {

                    foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                    {
                        if (dailyRenterVendor != null)
                        {
                            if (dailyRenterVendor.Selected == true)
                            {
                                if (dailyRenterVendor.Status == "W")
                                {
                                    dailyRenterVendor.Status = "S";
                                    dailyRenterVendor.BatchNbr = null;
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }
                            }
                        }
                    }
                    base.Persist();
                });
            }
            return adapter.Get();
        }
        #endregion

        /// <summary>解除簽認按鈕邏輯</summary>
        /// 1.只有"已簽認"才可以使用此按鈕
        /// 2.刪除加扣款單
        /// 3.更改狀態為"已會簽"
        /// 4.檢核是否為已計價,若計價單已被計價,出錯誤:此張單已被計價,不可解除簽認

        #region UnConfirmAction
        public PXAction<RenterFilter> UnConfirmAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Vendor UnConfirm", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable unConfirmAction(PXAdapter adapter)
        {
            //檢查狀態
            foreach (KGDailyRenterVendor kGDailyRenterVendor in RenterVendors.Select())
            {
                if (kGDailyRenterVendor != null)
                {
                    if (kGDailyRenterVendor.Selected == true)
                    {
                        if (kGDailyRenterVendor.Status == "C")
                        {
                            KGValuationDetail valuationdetail = PXSelectJoin<KGValuationDetail,
                                InnerJoin<KGValuation,
                            On<KGValuation.valuationID,
                            Equal<KGValuationDetail.valuationID>>>,
                                Where<KGValuation.dailyRenterCD,
                                Equal<Required<KGValuation.dailyRenterCD>>>>
                                .Select(this, kGDailyRenterVendor.DailyRenterCD);
                            if (valuationdetail != null)
                            {
                                if (valuationdetail.Status == "B")
                                {
                                    setCheckThread = true;
                                    RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                               kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("該筆資料已經完成計價, 不能取消廠商確認!", PXErrorLevel.RowError));
                                    //throw new Exception("該筆資料已經完成計價, 不能取消廠商確認!");
                                }
                            }

                        }
                        else if (kGDailyRenterVendor.Status == "W")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已會簽」。" + "\n" + "若要解除，請執行「解除會簽」；" + "\n" + "若要簽認，請執行「簽認及上傳附件」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「已會簽」。" + "\n" + "若要解除，請執行「解除會簽」；" + "\n" + "若要簽認，請執行「簽認及上傳附件」!");
                        }
                        else if (kGDailyRenterVendor.Status == "S")
                        {
                            setCheckThread = true;
                            RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
                           kGDailyRenterVendor, kGDailyRenterVendor.Status, new PXSetPropertyException("此狀態為「已到工」。" + "\n" + "若要會簽，請執行「列印及會簽」!", PXErrorLevel.RowError));
                            //throw new Exception("此狀態為「未會簽」。" + "\n" + "若要會簽，請執行「列印及會簽」!");
                        }

                    }
                }
            }
            if (!setCheckThread)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
                    {
                        if (dailyRenterVendor != null)
                        {
                            if (dailyRenterVendor.Selected == true)
                            {
                                if (dailyRenterVendor.Status == "C")
                                {
                                    if (dailyRenterVendor.ConfirmQty != 0)
                                    {
                                        DeleteValuation(dailyRenterVendor);
                                    }
                                    dailyRenterVendor.Status = "W";
                                    dailyRenterVendor.Selected = false;
                                    RenterVendors.Update(dailyRenterVendor);
                                }
                            }
                        }
                    }
                    base.Persist();
                });
            }

            return adapter.Get();
        }
        #endregion



        /*public KGRenterConfirmProcess()
        {
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.SendApprovalAction);
            this.ActionMenu.AddMenuAction(this.VendorConfirmAction);
            this.ActionMenu.AddMenuAction(this.UnSignAction);
            this.ActionMenu.AddMenuAction(this.UnConfirmAction);
        }*/
        #endregion

        #region Event
        protected virtual void KGDailyRenterVendor_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGDailyRenterVendor row = (KGDailyRenterVendor)e.Row;
            if (row == null) return;
            PXUIFieldAttribute.SetEnabled(sender, row, false);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.selected>(sender, row, true);
            PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmQty>(sender, row, row.Status =="S");
            RenterVendors.AllowInsert = false;
            RenterVendors.AllowDelete = false;
            //2019/04/19ConfrimAmt改成ConfirmInstQty->2019/04/08把confirmInsQty拿掉
            //PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmInstQty>(sender, row, true);
            //PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.selected>(sender, row, true);
            //2019/04/22 不加if的檢核會出現null的exception 錯誤
            if (row != null)
            {
                PXResultset<KGDailyRenter> set = PXSelect<KGDailyRenter,
              Where<KGDailyRenter.dailyRenterID,
              Equal<Required<KGDailyRenter.dailyRenterID>>>>
                      .Select(this, row.DailyRenterID);
                foreach (KGDailyRenter kgorder in set)
                {
                    row.DailyRenterCD = kgorder.DailyRenterCD;
                    row.WorkDate = kgorder.WorkDate;
                }
                //2019/05/08畫面加入專案描述在專案旁邊(Unbound)
                PXResultset<PMProject> setpm =
                    PXSelectJoin<PMProject,
                    InnerJoin<KGDailyRenterVendor,
                    On<KGDailyRenterVendor.contractID,
                    Equal<PMProject.contractID>>>,
                    Where<KGDailyRenterVendor.contractID,
                    Equal<Required<KGDailyRenterVendor.contractID>>>>
                    .Select(this, row.ContractID);
                foreach (PMProject pMProject in setpm)
                {
                    row.Contractdesc = pMProject.Description;
                }
            }
            if(row.Status=="S")
            {
                PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmQty>(sender, row, true);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled<KGDailyRenterVendor.confirmQty>(sender, row, false);
            }

        }
        /*Sergey教的方法
         * protected virtual void KGDailyRenterVendor_Selected_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)

        {
            KGDailyRenterVendor row = (KGDailyRenterVendor)e.Row;
            if(e.NewValue==null)
            {
                foreach (KGDailyRenterVendor rentervendor in RenterVendors.Select())
                {
                    if (rentervendor.Selected == true && rentervendor.DailyRenterID !=row.DailyRenterID )

                    {

                        rentervendor.Selected = false;

                        sender.Update(rentervendor);

                    }

                }

            }
            
        }*/
        //2019/07/01代辦數量、自辦數量、Amount都要變
        protected virtual void KGDailyRenterVendor_ConfirmQty_FieldUpdated(PXCache sender ,PXFieldUpdatedEventArgs e)
        {
            KGDailyRenterVendor renterVendor = (KGDailyRenterVendor)e.Row;         
            checkConfirmQty(sender, renterVendor);
            KGDailyRenter renter = PXSelect<KGDailyRenter,
                Where<KGDailyRenter.dailyRenterID, Equal<Required<KGDailyRenterVendor.dailyRenterID>>>>
                .Select(this, renterVendor.DailyRenterID);
            POLine poline = getPOLine(renter.LineNbr, renter.OrderNbr);
            if (poline == null)
            {
                renterVendor.Amount = renterVendor.ConfirmQty * 1;
            }
            else
            {
                renterVendor.Amount = renterVendor.ConfirmQty * poline.CuryUnitCost;
            }
        }
        #endregion

        #region Select
        public PXSelect<KGValuation> Valuations;
        public PXSelect<KGValuationDetail> ValuationDetails;
        public PXSetup<KGSetUp> SetUp;
        public PXSelect<KGValuationDetailA,
                 Where<KGValuationDetailA.valuationID,
                     Equal<Current<KGValuation.valuationID>>,
                 And<KGValuationDetailA.pricingType, Equal<word.a>>>> AdditionDetails;
        public PXSelect<KGValuationDetailD,
                 Where<KGValuationDetailD.valuationID,
                   Equal<Current<KGValuation.valuationID>>,
                 And<KGValuationDetailD.pricingType, Equal<word.d>>>> DeductionDetails;
        public PXSelect<KGDailyRenter,
         Where<KGDailyRenter.dailyRenterID,
         Equal<Current<KGDailyRenterVendor.dailyRenterID>>>> KGDailyRenters;

        #endregion

        #region Method
        //新增加扣款單 
        //2019/11/08 ADD檢核功能
        //checkorCreatetype:1=檢核扣款金額 checkorCreatetype:2=新增加扣款
        public bool CreateValuation(KGDailyRenterVendor DRV,string checkorCreatetype)
        {
             //= RenterVendors.Current;
            KGValuation KVHeader = new KGValuation();
            KGValuationDetailD KVDetailD = new KGValuationDetailD();
            KGValuationDetailA KVDetailA = new KGValuationDetailA();

            KVHeader.ContractID = DRV.ContractID;
            KVHeader.ValuationType = "0";
            KVHeader.Hold = false;
            KVHeader.Description = DRV.WorkContent;
            #region 2019/06/05把加扣款單類型改回代扣代付/單價抓RenterVendor的單價
            /*KVHeader.ValuationType = "2";
              POLine dailyrentervendor =PXSelectJoin<POLine,
                InnerJoin<KGDailyRenterVendor, On<KGDailyRenterVendor.orderNbr, 
                Equal<POLine.orderNbr>>>,
                Where<POLine.lineNbr, 
                Equal<Required<POLine.lineNbr>>>>
                .Select(this,DRV.LineNbr);
            KVHeader.UnitPrice = dailyrentervendor.CuryUnitCost;*/
            #endregion
            KVHeader.Status = "C";
            KVHeader.Uom = "工";
            //Header+DetailA
            PXResultset<KGDailyRenter> set = PXSelect<KGDailyRenter,
                Where<KGDailyRenter.dailyRenterID,
                Equal<Required<KGDailyRenter.dailyRenterID>>>>
                        .Select(this, DRV.DailyRenterID);
            foreach (KGDailyRenter kgorder in set)
            {
                KVHeader.ValuationDate = kgorder.WorkDate;
                KVHeader.DailyRenterCD = kgorder.DailyRenterCD;
                //Header公式
                #region 2019/06/05數量變為代辦數量 /單價變為KGDailyRenter的purchase order Line的單價
                /*PXResultset<POLine> po = PXSelect<POLine,
                            Where<POLine.orderNbr,
                                Equal<Required<POLine.orderNbr>>>>
                                .Select(this, DRV.OrderNbr);
                foreach (POLine poline in po)
                {
                    KVHeader.UnitPrice = poline.CuryUnitCost;
                    KVHeader.Qty = poline.UnbilledQty;*/
                #endregion

                POLine poline = PXSelect<POLine,
                    Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                    And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>
                    .Select(this, kgorder.OrderNbr, kgorder.LineNbr);
                KVHeader.UnitPrice = poline.CuryUnitCost;
                //2021-05-06 edit by Alton  InsteadQty→ConfirmQty
                KVHeader.Qty = DRV.ConfirmQty;
                KVHeader.Amount = KVHeader.Qty * KVHeader.UnitPrice;
                PXResultset<POTaxTran> Ptt = PXSelect<POTaxTran,
                            Where<POTaxTran.orderNbr,
                                Equal<Required<POTaxTran.orderNbr>>>>
                                .Select(this, DRV.OrderNbr);
                foreach (POTaxTran taxrate in Ptt)
                {
                    KVHeader.TaxAmt = KVHeader.Amount * (taxrate.TaxRate / 100);
                }

                KVHeader.TotalAmt = KVHeader.TaxAmt + KVHeader.Amount;
                PXResultset<KGSetUp> setup = PXSelect<KGSetUp,
                            Where<KGSetUp.kGManageFeeTaxRate,
                                IsNotNull>>.Select(this);
                foreach (KGSetUp rate in setup)
                {
                    KVHeader.ManageFeeAmt = KVHeader.Amount * (rate.KGManageFeeRate / 100);
                    KVHeader.ManageFeeTaxAmt
                        = KVHeader.ManageFeeAmt * (rate.KGManageFeeTaxRate / 100);
                }
                KVHeader.ManageFeeTotalAmt = KVHeader.ManageFeeAmt + KVHeader.ManageFeeTaxAmt;

                //}
                //改完代扣代付->2019/06/12加款總額=TotalAmt(含稅金額)
                KVHeader.AdditionAmt = KVHeader.TotalAmt;
                KVHeader.DeductionAmt = KVHeader.TotalAmt + KVHeader.ManageFeeTotalAmt;

                //DetailA 2019/05/07改為一般扣款(A的資料不用填)
                //表頭的type改成一般扣款表身的status改為V表頭的不變
                //DetailA 2019/06/05改回代扣代付(A資料要填/status="V")
                KVDetailA.OrderNbr = kgorder.OrderNbr;
                POOrder orderdetailA = PXSelect<POOrder,
                    Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>
                    .Select(this, KVDetailA.OrderNbr);
                if (orderdetailA.Status == "V")
                {
                    throw new Exception(String.Format("此單號{0},狀態為已拒絕,請選擇別張分包契約單號!", orderdetailA.OrderNbr));
                }
                KVDetailA.OrderType = orderdetailA.OrderType;
                KVDetailA.Uom = "工";
                KVDetailA.Qty = KVHeader.Qty;
                KVDetailA.Amount = KVHeader.Amount;
                KVDetailA.UnitPrice = KVHeader.UnitPrice;
                KVDetailA.TaxAmt = KVHeader.TaxAmt;
                KVDetailA.TotalAmt = KVHeader.TotalAmt;
                KVDetailA.ContractID = KVHeader.ContractID;
                KVDetailA.Status = "V";
                KVDetailA.ManageFeeAmt = 0;
                KVDetailA.ManageFeeTaxAmt = 0;
                KVDetailA.ManageFeeTotalAmt = 0;
            }  
            //DetailD
            KVDetailD.ContractID = KVHeader.ContractID;
            KVDetailD.OrderNbr = DRV.OrderNbr;
            //2020/09/18 Fix POOrder status為已拒絕 或出錯的單
            POOrder orderdetailD = PXSelect<POOrder,
                   Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>
                   .Select(this, KVDetailD.OrderNbr);
            if(orderdetailD.Status =="V")
            {
                throw new Exception(String.Format("此單號{0},狀態為已拒絕,請選擇別張分包契約單號!",orderdetailD.OrderNbr));
            }
            KVDetailD.OrderType = orderdetailD.OrderType;
            KVDetailD.Qty = KVHeader.Qty;
            KVDetailD.Uom = "工";
            KVDetailD.Amount = KVHeader.Amount;
            KVDetailD.TaxAmt = KVHeader.TaxAmt;
            KVDetailD.TotalAmt = KVHeader.TotalAmt;
            KVDetailD.ManageFeeAmt = KVHeader.ManageFeeAmt;
            KVDetailD.ManageFeeTaxAmt = KVHeader.ManageFeeTaxAmt;
            KVDetailD.ManageFeeTotalAmt = KVHeader.ManageFeeTotalAmt;
            KVDetailD.Status = "V";
            KVDetailD.UnitPrice = KVHeader.UnitPrice;
            //新增Detail InventoryID
            PXResultset<KGSetUp> setupkg = PXSelect<KGSetUp,
               Where<KGSetUp.kGAdditionInventoryID, IsNotNull,
               And<KGSetUp.kGDeductionInventoryID, IsNotNull>>>.Select(this);
            foreach (KGSetUp kgsetup in setupkg)
            {
                KVDetailA.InventoryID = kgsetup.KGAdditionInventoryID;
                //2021/11/03 Modify Add 若加扣款類型為代扣代付,則工料編號帶與加款的工料編號一至
                KVDetailD.InventoryID = kgsetup.KGAdditionInventoryID;
            }
            
            if(checkorCreatetype =="1")
            {
                CalUnBilledAmt calUnBilledAmt = new CalUnBilledAmt();
                if(calUnBilledAmt.TotalUnBilledAmt(this,KVDetailD.OrderNbr)-KVHeader.DeductionAmt<0)
                {

                    return false;
                }
            }
            else if(checkorCreatetype=="2")
            {
                Valuations.Insert(KVHeader);
                DeductionDetails.Insert(KVDetailD);
                AdditionDetails.Insert(KVDetailA);
                return true;
            }

            return true;
        }
        //刪除加扣款單
        public void DeleteValuation(KGDailyRenterVendor DRV)
        {
            //KGDailyRenterVendor DRV = RenterVendors.Current;
            PXResultset<KGValuation> SearchDailyRenterID = 
                PXSelect<KGValuation, 
                Where<KGValuation.dailyRenterCD,
                Equal<Required<KGValuation.dailyRenterCD>>>>
                .Select(this, DRV.DailyRenterCD);
            foreach(KGValuation deletevaluation in SearchDailyRenterID )
            {
                if(deletevaluation.DailyRenterCD == DRV.DailyRenterCD)
                {
                    Valuations.Delete(deletevaluation);
                }
                PXResultset<KGValuationDetail> SearchDetail =
                PXSelect<KGValuationDetail,
                Where<KGValuationDetail.valuationID,
                Equal<Required<KGValuationDetail.valuationID>>>>
                .Select(this, deletevaluation.ValuationID);
                foreach(KGValuationDetail deletedetail in SearchDetail)
                {
                    ValuationDetails.Delete(deletedetail);
                }
            }
        }
        //開新頁面到Report
        protected virtual void NewPageToDisplayReport(KGRenterConfirmProcess pp)
        {
            //d[ReportMessages.CheckReportFlag] = ReportMessages.CheckReportFlagValue;
            //var requiredException = new PXReportRequiredException(d, "KG601000", PXBaseRedirectException.WindowMode.New, "廠商確認單");
            KGDailyRenterVendor DRV = RenterVendors.Current;
            if (DRV != null)
            {
                if (DRV.BatchNbr == null)
                {
                    return;
                }
                else
                {
                    Dictionary<string, string> mailParams = new Dictionary<string, string>()
                    {
                        ["BatchNbr"] = DRV.BatchNbr.ToString()
                    };
                    var requiredException = new PXReportRequiredException(mailParams, "KG601000", PXBaseRedirectException.WindowMode.New, "廠商確認單");
                    requiredException.SeparateWindows = true;
                    //requiredException.AddSibling("KG601000", mailParams);
                    throw new PXRedirectWithReportException(pp, requiredException, "Preview");
                }
                
            }


        }
        //檢核ConfirmQty
        bool setCheckThread = false;
        public bool checkConfirmQty(PXCache sender,KGDailyRenterVendor row)
        {
            KGDailyRenter dailyRenter =
                PXSelect<KGDailyRenter, Where<KGDailyRenter.dailyRenterID, 
                Equal<Required<KGDailyRenter.dailyRenterID>>>>
                .Select(this, row.DailyRenterID);
            //檢核不可為空值
            if (row.ConfirmQty == null)
            {
                setCheckThread = true;
                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.confirmQty>(
                        row, row.ConfirmQty, new PXSetPropertyException("不可為空值!"));
                return true;
            }
            //檢核確認數量不可大於實際數量
            if (dailyRenter.ActQty<row.ConfirmQty)
            {
                setCheckThread = true;
                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.confirmQty>(
                        row, row.ConfirmQty, new PXSetPropertyException("確認數量不可以大於實際數量"));
                return true;
            }
            return false;
        }
        public bool checkFileLength(PXCache sender,KGDailyRenterVendor row)
        {
            Guid[] files = PXNoteAttribute.GetFileNotes(RenterVendors.Cache, row);
            if (files.Length == 0)
            {
                setCheckThread = true;
                RenterVendors.Cache.RaiseExceptionHandling<KGDailyRenterVendor.status>(
             row, row.Status, new PXSetPropertyException("請夾檔案!", PXErrorLevel.RowError));
                //throw new PXException("請夾檔案!");
                return true;
            }
            return false;
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
        //2019/11/14改成"廠商確認"才回壓代辦數量、自辦數量、金額
        public virtual void UpdateDailyRenter(KGDailyRenterVendor dailyRenterVendor,decimal? ConfirmQty)
        {
            KGDailyRenter dailyRenter =
                                            PXSelect<KGDailyRenter,
                                            Where<KGDailyRenter.dailyRenterID,
                                            Equal<Required<KGDailyRenter.dailyRenterID>>>>
                                            .Select(this, dailyRenterVendor.DailyRenterID);
            //回壓代辦數量=確認數量 自辦數量=實際數量-代辦數量
            //2020/04/01 有可能有多筆
            dailyRenter.ActInsteadQty = ConfirmQty;
            dailyRenter.ActSelfQty = dailyRenter.ActQty - ConfirmQty;
            KGDailyRenters.Update(dailyRenter);
            base.Persist();
        }
        public override void Persist()
        {
            KGDailyRenterVendor renterVendor = RenterVendors.Current;
            checkConfirmQty(RenterVendors.Cache, renterVendor);
            foreach (KGDailyRenterVendor dailyRenterVendor in RenterVendors.Select())
            {
                if (dailyRenterVendor.Selected == true)
                {
                    if (dailyRenterVendor.Status == "S")
                    {
                        if (dailyRenterVendor.ConfirmQty == 0)
                        {
                            dailyRenterVendor.Status = "C";
                            dailyRenterVendor.Selected = false;
                            RenterVendors.Update(dailyRenterVendor);
                        }
                        else if (dailyRenterVendor.ConfirmQty > 0)
                        {
                            dailyRenterVendor.Selected = false;
                            RenterVendors.Update(dailyRenterVendor);
                        }
                        

                    }
                }
            }
            base.Persist();
        }
        #endregion

        #region Link
        //ValuationCD超連結
        public PXAction<RenterFilter> ViewRenter;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Daily Renter", Visible = false)]
        protected virtual void viewRenter()
        {
            KGDailyRenterVendor row = RenterVendors.Current;
            // Creating the instance of the graph
            KGDailyRenterActualEntry graph = PXGraph.CreateInstance<KGDailyRenterActualEntry>();
            // Setting the current product for the graph
            graph.DailyRenters.Current = graph.DailyRenters.Search<KGDailyRenter.dailyRenterCD>(
            row.DailyRenterCD);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.DailyRenters.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "DailyRenter Details");
            }


        }
        #endregion
    }

    [Serializable]
    public class RenterFilter : IBqlTable
    {
        #region CreatedByID
        [PXString()]
        [PXUIField(DisplayName = "CreatedByID")]
        [PXSelector(typeof(Search5<Users.username,
             InnerJoin<KGDailyRenterVendor, On<KGDailyRenterVendor.createdByID,
                 Equal<Users.pKID>>>,
             Aggregate<GroupBy<Users.username>>>),
             typeof(KGDailyRenterVendor.createdByID),
             typeof(Users.fullName),
             SubstituteKey = typeof(Users.username))]
        public virtual string CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion

        #region ContractID
        [PXInt()]
        [PXUIField(DisplayName = "Project")]
        [PXSelector(typeof(Search2<PMProject.contractID,
                LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                Equal<PMProject.contractID>>>>,
                Where<PMProject.baseType, Equal<CTPRType.project>,
                 And<PMProject.nonProject, Equal<False>,
                     And<PMProject.status,Equal<word.a>,
                 And<Match<Current<AccessInfo.userName>>>>>>>)
                , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(Customer.acctName), typeof(PMProject.status),
                typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region OrderNbr
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Order Nbr")]
        [PXSelector(typeof(Search5<POOrder.orderNbr,
              LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>
                  , LeftJoin<POLine, On<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
              Where<Vendor.bAccountID, IsNotNull,
               And<POLine.projectID, Equal<Current<KGDailyRenter.contractID>>,
                   And<POOrder.orderNbr, NotEqual<Current<KGDailyRenter.orderNbr>>,
                        And<
                            Where2<
                                Where<POOrder.orderType, Equal<POOrderType.regularSubcontract>,
                                    And<Where<POOrder.status, Equal<KGDailyRenterVendor.Open>, Or<POOrder.status, Equal<KGDailyRenterVendor.Complete>>>>>,
                                Or<Where<POOrder.orderType, Equal<POOrderType.regularOrder>,
                                        And<Where<POOrder.status, Equal<KGDailyRenterVendor.Open>, Or<POOrder.status, Equal<KGDailyRenterVendor.Close>, Or<POOrder.status, Equal<KGDailyRenterVendor.Complete>>>>>>>>>>>
                  >,
              Aggregate<GroupBy<POOrder.orderNbr>>>),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc),
            typeof(POOrder.vendorID),
            typeof(Vendor.acctName),
            typeof(POOrder.orderDate),
            typeof(POOrder.status),
            typeof(Vendor.curyID),
            typeof(POOrder.vendorRefNbr),
            Filterable = true,
            SubstituteKey = typeof(POOrder.orderNbr),
            DescriptionField = typeof(POOrder.orderDesc))]

        /*[PO.RefNbr(
            typeof(Search5<POOrder.orderNbr,
            LeftJoin<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
            And<Match<Vendor, Current<AccessInfo.userName>>>>>,
            Where<POOrder.hold, Equal<False>,
            And2<Where<POOrder.status, Equal<word.n>,
                Or<POOrder.status,Equal<word.c>,
                    Or<POOrder.status,Equal<word.m>>>>,
            And<Where<POOrder.orderType, Equal<word.ro>,
                Or<POOrder.orderType, Equal<word.rs>>>>>>,
            Aggregate<GroupBy<POOrder.orderNbr>>,
            OrderBy<Desc<POOrder.orderNbr>>>),
            DescriptionField = typeof(POOrder.orderDesc)
       )]*/
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region VendorID
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        [PXUIField(DisplayName = "Vendor")]
        [PXForeignReference(typeof(Field<RenterFilter.vendorID>.IsRelatedTo<BAccount.bAccountID>))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : IBqlField { }
        #endregion

        #region WorkDateFrom
        [PXDate()]
        [PXUIField(DisplayName = "Work Date From")]
        public virtual DateTime? WorkDateFrom { get; set; }
        public abstract class workDateFrom : IBqlField { }
        #endregion

        #region WorkDateTo
        [PXDate()]
        [PXUIField(DisplayName = "Work Date To")]
        public virtual DateTime? WorkDateTo { get; set; }
        public abstract class workDateTo : IBqlField { }
        #endregion

        #region Status
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Status")]
        [PXStringList(
                    new string[]
                    {
                      "S", "W","C"

                    },
                    new string[]
                    {
                       "ON SITE", "PENDING SIGN","VENDOR CONFIRM"
        })]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
        #endregion

        

    }

    //計算剩餘未計價總金額(PO)
    public class CalUnBilledAmt
    {
        
        //剩餘未計價總金額 = POLine迴圈(未開發票總金額+保留款總金額)+apRegister退回保留款
        public decimal TotalUnBilledAmt(PXGraph graph, string orderNbr)
        {
            //vTotalUnBilledAmt=剩餘未計價總金額
            decimal vTotalUnBilledAmt = 0;

            //已PONbr算出Sum(POLine未開發票總金額)&Sum(POLine保留款總金額)
            PXResultset<POLine> setPoLine = PXSelectGroupBy<POLine,
               Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>,
               Aggregate<GroupBy<POLine.orderNbr,
               Sum<POLine.curyUnbilledAmt,
               Sum<POLine.curyRetainageAmt>>>>>.Select(graph, orderNbr);
            foreach (POLine pOLine in setPoLine)
            {
                //避免空值
                decimal? vCuryUnbilledAmt = pOLine.CuryUnbilledAmt ?? 0;
                decimal? vCuryRetainageAmt = pOLine.CuryRetainageAmt ?? 0;
                 
                vTotalUnBilledAmt = (decimal)(vCuryUnbilledAmt + vCuryRetainageAmt);
            }

            //已PONbr算出退回保留款總金額
            APRegister apRegister = PXSelectGroupBy<APRegister,
                Where<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>>,
                Aggregate<GroupBy<APRegisterExt.usrPONbr,
                Sum<APRegister.curyRetainageUnreleasedAmt>>>>
                .Select(graph, orderNbr);
            //避免空值
            if (apRegister == null) return vTotalUnBilledAmt;
            decimal? vCuryRetainageUnreleasedAmt = apRegister.CuryRetainageUnreleasedAmt ?? 0;
            vTotalUnBilledAmt = vTotalUnBilledAmt + (decimal)vCuryRetainageUnreleasedAmt;

            return vTotalUnBilledAmt;
        }

        ///2019/11/07 ADD 檢查未計價總金額是否夠扣扣款總額
        ///True:可以扣款 False:不可扣款
        public bool CheckUnbilledAmtEnoughToDeduation(decimal deduationAmt, decimal totalUnBilledAmt)
        {
            if(totalUnBilledAmt - deduationAmt<0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}