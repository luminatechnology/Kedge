using System;
using System.Web;
using System.Collections;
using PX.Data;
using PX.SM;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Objects.IN;
using PX.Objects.AR;
using PX.Objects.PM;
using PX.Objects.CT;
using Kedge.DAC;
using PX.Common;

namespace PX.Objects.RQ
{
    /**
     * ===2021/04/13 Mantis: 0012018 ===Althea
     * PMProjectCostFilter的專案改掉formula為預設
     * **/
    public class RQRequestEntry_Extension : PXGraphExtension<RQRequestEntry>
    {
        public String message, approveMessage;

        //#region Override function
        //public override void Initialize()
        //{
        //    base.Initialize();
        //    //Base.action.AddMenuAction(this.webServIntegration);
        //    //Base.action.AddMenuAction(webServTest);
        //}
        //#endregion

        #region Custom Constant
        public sealed class Zero : Constant<String>
        {
            public Zero() : base("0") { }
        }
        #endregion

        #region Add Cost Item
        public PXSelect<KGRequestFile, Where<KGRequestFile.orderNbr, Equal<Current<RQRequest.orderNbr>>>> KGRequestFiles;

        [PXCopyPasteHiddenView]
        public PXFilter<PMProjectCostFilter> projectCostFilter;

        [PXFilterable]
        [PXCopyPasteHiddenView]
        //public PXSelectReadonly<PMCostBudget> pmCostBudget;
        public PXSelect<PMCostBudget, 
                        Where<PMCostBudget.projectID, Equal<Current<PMProjectCostFilter.contractID>>,
                              And<PMBudgetExt.usrInvPrType, Equal<Zero>>>, 
                        OrderBy<Asc<PMCostBudget.projectTaskID, 
                                    Asc<PMCostBudget.costCodeID,  
                                        Asc<PMCostBudget.accountGroupID>>>>> pmCostBudget;

        protected virtual IEnumerable PmCostBudget()
        {
            //PXResultset<PMCostBudget> query = pmCostBudget.Select();
            foreach (PMCostBudget pmCostBudget in PXSelect<PMCostBudget, 
                        Where<PMCostBudget.projectID, Equal<Current<PMProjectCostFilter.contractID>>,
                              And<PMBudgetExt.usrInvPrType, Equal<Zero>>>, 
                        OrderBy < Asc < PMCostBudget.projectTaskID, 
                                    Asc < PMCostBudget.costCodeID,  
                                        Asc < PMCostBudget.accountGroupID >>>>>.Select(Base)) {
                PMBudgetExt pmBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCostBudget);
                
                if (SubcontractEntry_Extension.type.Equals(pmCostBudget.UOM))
                {
                    pmBudgetExt.UsrAvailQty = getValue(pmCostBudget.RevisedQty);
                }
                else
                {
                    Decimal? commitQty = getCommitQty(Base, pmCostBudget.CostCodeID.Value, pmCostBudget.TaskID.Value,
                                                        pmCostBudget.ProjectID.Value, pmCostBudget.InventoryID.Value,
                                                        pmCostBudget.AccountGroupID.Value);
                    pmBudgetExt.UsrAvailQty = getValue(pmCostBudget.RevisedQty) - commitQty;

                }
                Decimal? commitAmt = getCommitAmt(Base, pmCostBudget.CostCodeID.Value, pmCostBudget.TaskID.Value,
                                                        pmCostBudget.ProjectID.Value, pmCostBudget.InventoryID.Value,
                                                        pmCostBudget.AccountGroupID.Value);
                pmBudgetExt.UsrAvailAmt = getValue(pmCostBudget.RevisedAmount) - commitAmt;

                yield return pmCostBudget;
            }
        }
        public virtual void PMCostBudget_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            PMCostBudget pmCostBudget = (PMCostBudget)e.Row;
            PMBudgetExt pmBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCostBudget);
            if (pmBudgetExt.UsrAvailQty == null) {
                if (SubcontractEntry_Extension.type.Equals(pmCostBudget.UOM)) {
                    pmBudgetExt.UsrAvailQty = getValue(pmCostBudget.RevisedQty);
                }
                else {
                    Decimal? commitQty = getCommitQty(Base, pmCostBudget.CostCodeID.Value, pmCostBudget.TaskID.Value,
                                                                          pmCostBudget.ProjectID.Value, pmCostBudget.InventoryID.Value,
                                                                          pmCostBudget.AccountGroupID.Value);
                    

                    pmBudgetExt.UsrAvailQty = getValue(pmCostBudget.RevisedQty) - commitQty;
                    
                }
            }
            if (pmBudgetExt.UsrAvailAmt == null)
            {
                Decimal? commitAmt = getCommitAmt(Base, pmCostBudget.CostCodeID.Value, pmCostBudget.TaskID.Value,
                                                                          pmCostBudget.ProjectID.Value, pmCostBudget.InventoryID.Value,
                                                                          pmCostBudget.AccountGroupID.Value);
                pmBudgetExt.UsrAvailAmt = getValue(pmCostBudget.RevisedAmount) - commitAmt;
            }
        }

        //Lines  RQRequestLine
        public PXAction<RQRequest> addCostItem;
        [PXUIField(DisplayName = "Add Cost Item", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddCostItem(PXAdapter adapter)
        {
            try
            {
                WebDialogResult result = pmCostBudget.AskExt(true);
                if (result == WebDialogResult.OK)
                {
                    return AddItems(adapter);
                    //Base.Transactions  POLine
                }
                else if (result == WebDialogResult.Cancel)
                {
                    pmCostBudget.Cache.Clear();
                    pmCostBudget.Cache.ClearQueryCache();
                }
                else
                {
                    pmCostBudget.Cache.Clear();
                    pmCostBudget.Cache.ClearQueryCache();
                }
            }
            catch (Exception e)
            {
                pmCostBudget.Cache.Clear();
                pmCostBudget.Cache.ClearQueryCache();
                //projectCostFilter.Cache.Clear(); ;
                throw e;
            }
            return adapter.Get();
        }

        public PXAction<RQRequest> addItems;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddItems(PXAdapter adapter)
        {
            foreach (PMCostBudget pmCost in pmCostBudget.Cache.Cached)
            {
                if (pmCost.Selected != true)
                {
                    continue;
                }
                /* remove by louis for allow user add the same cost line multi time on 2019 / 6 / 3
                if (!checkLine(pmCost)) {
                     continue;
                }
                */
                PXCache contactCache = Base.Lines.Cache;
                RQRequestLine line = (RQRequestLine)Base.Lines.Cache.CreateInstance();
                PMBudgetExt pmBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCost);
                //PMAccountGroup group = getPMAccountGroup(pmCost.AccountGroupID);
                //line.Description = group.Description;
                //line.Description = pmCost.Description;
                line.Description = pmBudgetExt.UsrInventoryDesc;
                line.InventoryID = pmCost.InventoryID;
                //Qty
                line.UOM = pmCost.UOM;
                // line.CuryEstUnitCost = pmCost.UnitPrice;
                //line.EstUnitCost = pmCost.UnitPrice;
                //line.CuryEstUnitCost = pmCost.
                line.CuryEstUnitCost = pmCost.CuryUnitRate;
                //line.EstUnitCost = 0;

                Decimal revisedQty = pmCost.RevisedQty.Value;
                Decimal committedOpenQty = pmCost.CommittedOpenQty.Value;
                Decimal actualQty = pmCost.ActualQty.Value;
                Decimal committedQty = pmCost.CommittedQty.Value;

                Decimal? newCommittedQty = pmBudgetExt.UsrAvailQty;
                ///原本計算方式   修訂後預算數量-已承諾未結數量-實際數量
                ///
                if (SubcontractEntry_Extension.type.Equals(pmCost.UOM))
                {
                    line.OrderQty = revisedQty;
                }
                else
                {
                    line.OrderQty =  getValue(newCommittedQty);
                    //line.OrderQty = revisedQty - committedOpenQty - actualQty;
                }
                //現在計算方式  修訂後預算數量-修訂已承諾數量
                //line.OrderQty = revisedQty - committedQty;
                line=Base.Lines.Insert(line);
                contactCache.SetValue<RQRequestLineExt.usrContractID>(line, pmCost.ProjectID);
                contactCache.SetValue<RQRequestLineExt.usrProjectTaskID>(line, pmCost.ProjectTaskID);
                contactCache.SetValue<RQRequestLineExt.usrCostCodeID>(line, pmCost.CostCodeID);
          
                if (SubcontractEntry_Extension.type.Equals(pmCost.UOM))
                {
                    contactCache.SetValue<RQRequestLineExt.usrAvailQty>(line, getValue(pmCost.RevisedQty));
                }
                else
                {
                    contactCache.SetValue<RQRequestLineExt.usrAccountGroupID>(line, pmCost.AccountGroupID);
                    
                    Decimal? commitQty = getPreviousCommitQty(Base, pmCost.CostCodeID.Value, pmCost.TaskID.Value,
                                                          pmCost.ProjectID.Value, pmCost.InventoryID.Value,
                                                          pmCost.AccountGroupID.Value);
                    contactCache.SetValue<RQRequestLineExt.usrAvailQty>(line, getValue(pmCost.RevisedQty) - commitQty);
                }
                Decimal? commitAmt = getPreviousCommitAmt(Base, pmCost.CostCodeID.Value, pmCost.TaskID.Value,
                                                          pmCost.ProjectID.Value, pmCost.InventoryID.Value,
                                                          pmCost.AccountGroupID.Value);
                contactCache.SetValue<RQRequestLineExt.usrAvailAmt>(line, getValue(pmCost.RevisedAmount) - commitAmt);
                contactCache.SetValue<RQRequestLineExt.usrAccountGroupID>(line, pmCost.AccountGroupID);
               
                //RQRequestLineExt rqRequestLineExt = contactCache.GetExtension<RQRequestLineExt>(line);
            }
            pmCostBudget.Cache.Clear();
            pmCostBudget.Cache.ClearQueryCache();
            //projectCostFilter.Cache.Clear();
            return adapter.Get();
        }
        #endregion

        #region Persist
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            RQRequest master = Base.Document.Current;
            //代表刪除
            if (master == null)
            {
                baseMethod();
            }
            else
            {
                if (!beforeSaveCheck())
                {
                    return;
                }
                baseMethod();
            }
        }

        public bool beforeSaveCheck()
        {
            bool check = true;

            //OrderQty > ChangeOrderQty -CommittedQty
            foreach (RQRequestLine line in Base.Lines.Select())
            {
                RQRequestLineExt reqLineExt = PXCache<RQRequestLine>.GetExtension<RQRequestLineExt>(line);

                if (reqLineExt.UsrContractID != null && reqLineExt.UsrProjectTaskID != null && reqLineExt.UsrCostCodeID != null &&
                    reqLineExt.UsrAccountGroupID != null && line.InventoryID != null)
                {
                    //Decimal? commitQty = getCommitQty(Base, reqLineExt.UsrCostCodeID.Value, reqLineExt.UsrProjectTaskID.Value,
                    //                                        reqLineExt.UsrContractID.Value, line.InventoryID.Value,
                    //                                        reqLineExt.UsrAccountGroupID.Value);
                    //Decimal? commitAmt = getCommitAmt(Base, reqLineExt.UsrCostCodeID.Value, reqLineExt.UsrProjectTaskID.Value,
                    //                                        reqLineExt.UsrContractID.Value, line.InventoryID.Value,
                    //                                        reqLineExt.UsrAccountGroupID.Value);

                    PMCostBudget pmCost = RQRequestEntry_Extension.getPMCostBudget(reqLineExt.UsrCostCodeID.Value, reqLineExt.UsrProjectTaskID.Value,
                                                                                   reqLineExt.UsrContractID.Value, line.InventoryID.Value, reqLineExt.UsrAccountGroupID.Value);

                    if (pmCost == null)
                    {
                        Base.Lines.Cache.RaiseExceptionHandling<RQRequestLine.orderQty>(line, line.OrderQty, new PXSetPropertyException("無此預算", PXErrorLevel.RowError));
                        check = false;
                        if (!"RQ301000".Equals(PXSiteMap.CurrentScreenID))
                        {
                            throw new PXException("無此預算");
                        }
                    }
                    //else
                    //{
                    //    //非一式項檢查
                    //    if (!SubcontractEntry_Extension.type.Equals(line.UOM))
                    //    {
                    //        if ((getValue(pmCost.RevisedQty.Value) - commitQty) < 0)
                    //        {
                    //            Base.Lines.Cache.RaiseExceptionHandling<RQRequestLine.orderQty>(line, line.OrderQty, new PXSetPropertyException("訂單數量不可大於預算可發包數量"));
                    //            check = false;
                    //            if (!"RQ301000".Equals(PXSiteMap.CurrentScreenID))
                    //            {
                    //                throw new PXException("訂單數量不可大於預算可發包數量");
                    //            }
                    //        }
                    //    }
                    //    if ((getValue(pmCost.CuryRevisedAmount.Value) - commitAmt) < 0)
                    //    {
                    //        Base.Lines.Cache.RaiseExceptionHandling<RQRequestLine.curyEstExtCost>(line, line.CuryEstExtCost, new PXSetPropertyException("訂單金額不可大於預算可發包金額"));

                    //        check = false;
                    //        if (!"RQ301000".Equals(PXSiteMap.CurrentScreenID))
                    //        {
                    //            throw new PXException("訂單金額不可大於預算可發包金額");
                    //        }
                    //    }
                    //}

                    // Added a verification based on spec [Kedge_SASD_請採購系統改善需求規格].
                    if (Base.Document.Current?.Hold == false)
                    {
                        new ChangeOrderEntry_Extension().CheckRemainAvailBudget<RQRequestLine, RQRequestLine.orderQty, RQRequestLine.curyEstExtCost>(Base.Lines.Cache, true);
                    }
                }
            }

            return check;
        }

        public bool checkLine(PMCostBudget pmCost)
        {
            foreach (RQRequestLine allline in Base.Lines.Select())
            {
                RQRequestLineExt alllineExt = PXCache<RQRequestLine>.GetExtension<RQRequestLineExt>(allline);

                int inventoryID = 0;
                int usrContractID = 0;
                int usrProjectTaskID = 0;
                int usrCostCodeID = 0;
                if (allline.InventoryID != null)
                {
                    inventoryID = allline.InventoryID.Value;
                }
                if (alllineExt.UsrContractID != null)
                {
                    usrContractID = alllineExt.UsrContractID.Value;
                }
                if (alllineExt.UsrProjectTaskID != null)
                {
                    usrProjectTaskID = alllineExt.UsrProjectTaskID.Value;
                }
                if (alllineExt.UsrCostCodeID != null)
                {
                    usrCostCodeID = alllineExt.UsrCostCodeID.Value;
                }

                if (inventoryID.Equals(pmCost.InventoryID) &&
                    usrContractID.Equals(pmCost.ProjectID) &&
                    usrProjectTaskID.Equals(pmCost.ProjectTaskID) &&
                    usrCostCodeID.Equals(pmCost.CostCodeID))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Action
        public PXAction<APInvoice> ViewOriginalDocument;

        [PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        protected virtual IEnumerable viewOriginalDocument(PXAdapter adapter)
        {
            //參考資料
            //https://asiablog.acumatica.com/2017/08/redirecting-to-external-page-from-button.html
            RQRequest master = Base.Document.Current;
            RQRequestExt rqRequestExt = PXCache<RQRequest>.GetExtension<RQRequestExt>(master);
            throw new Exception("Redirect7:" + rqRequestExt.UsrReturnUrl);
            return adapter.Get();
        }
        #endregion

        #region Event
        protected virtual void RQRequest_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(cache, e);

            RQRequest master = Base.Document.Current;

            addCostItem.SetEnabled(master.Hold == true);
            KGRequestFiles.AllowInsert = KGRequestFiles.AllowUpdate = KGRequestFiles.AllowDelete = master.Hold == true;

            RQRequestExt rqRequestExt = PXCache<RQRequest>.GetExtension<RQRequestExt>(master);

            if (message != null)
            {
                cache.RaiseExceptionHandling<RQRequest.hold>(master, master.Hold,
                          new PXSetPropertyException(message, PXErrorLevel.Error));
                //throw new  PXException(message);
            }
            if (approveMessage != null)
            {
                cache.RaiseExceptionHandling<RQRequest.hold>(master, master.Hold,
                          new PXSetPropertyException(approveMessage, PXErrorLevel.Warning));
                //throw new  PXException(message);
            }

            if (master.Hold == true)
            {
                rqRequestExt.UsrKGFlowUID = null;
            }

            if (master.Status.IsIn(RQRequestStatus.Rejected, RQRequestStatus.Hold))
            {
                rqRequestExt.UsrReturnUrl = null;
            }
        }

        protected virtual void RQRequestLine_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            RQRequestLine line = (RQRequestLine)e.Row;
            if (line != null)
            {

                PXUIFieldAttribute.SetEnabled<RQRequestLineExt.usrAvailAmt>(cache, line, false);

                RQRequestLineExt rqRequestLineExt = PXCache<RQRequestLine>.GetExtension<RQRequestLineExt>(line);
                PMCostBudget pmCost = null;
                if (rqRequestLineExt.UsrAvailQty == null)
                {

                    if (rqRequestLineExt.UsrContractID != null && rqRequestLineExt.UsrProjectTaskID != null && rqRequestLineExt.UsrCostCodeID != null &&
                        rqRequestLineExt.UsrAccountGroupID != null && line.InventoryID != null)
                    {

                        pmCost = getPMCostBudget(rqRequestLineExt.UsrCostCodeID.Value, rqRequestLineExt.UsrProjectTaskID.Value,
                                                 rqRequestLineExt.UsrContractID.Value, line.InventoryID.Value,
                                                 rqRequestLineExt.UsrAccountGroupID.Value);
                        if (SubcontractEntry_Extension.type.Equals(line.UOM))
                        {
                            if (pmCost != null)
                            {
                                rqRequestLineExt.UsrAvailQty = getValue(pmCost.RevisedQty.Value);
                            }
                            else
                            {
                                rqRequestLineExt.UsrAvailQty = 0;
                            }

                        }
                        else
                        {
                            Decimal? commitQty = getPreviousCommitQty(Base, rqRequestLineExt.UsrCostCodeID.Value, rqRequestLineExt.UsrProjectTaskID.Value,
                                                                            rqRequestLineExt.UsrContractID.Value, line.InventoryID.Value,
                                                                            rqRequestLineExt.UsrAccountGroupID.Value);
                            if (pmCost != null)
                            {
                                rqRequestLineExt.UsrAvailQty = getValue(pmCost.RevisedQty.Value) - commitQty;
                            }
                            else
                            {
                                rqRequestLineExt.UsrAvailQty = 0 - commitQty;
                            }

                        }

                    }
                }

                if (rqRequestLineExt.UsrAvailAmt == null)
                {
                    if (rqRequestLineExt.UsrContractID != null && rqRequestLineExt.UsrProjectTaskID != null && rqRequestLineExt.UsrCostCodeID != null &&
                        rqRequestLineExt.UsrAccountGroupID != null && line.InventoryID != null)
                    {
                        if (pmCost == null)
                        {
                            pmCost = getPMCostBudget(rqRequestLineExt.UsrCostCodeID.Value, rqRequestLineExt.UsrProjectTaskID.Value,
                                                     rqRequestLineExt.UsrContractID.Value, line.InventoryID.Value,
                                                     rqRequestLineExt.UsrAccountGroupID.Value);
                        }


                        Decimal? commitAmt = getPreviousCommitAmt(Base, rqRequestLineExt.UsrCostCodeID.Value, rqRequestLineExt.UsrProjectTaskID.Value,
                                                              rqRequestLineExt.UsrContractID.Value, line.InventoryID.Value,
                                                              rqRequestLineExt.UsrAccountGroupID.Value);
                        if (pmCost != null)
                        {
                            rqRequestLineExt.UsrAvailAmt = getValue(pmCost.RevisedAmount.Value) - commitAmt;
                        }
                        else
                        {
                            rqRequestLineExt.UsrAvailAmt = 0 - commitAmt;
                        }

                    }
                }
            }
        }
        #endregion

        //#region CacheAttached
        //20201021 - mark by Alton 此CacheAttached造成ProjectTaskID isKey=true 遺失，且考慮到此修改無意義，因此註解
        //[PXDBInt()]
        //[PXSelector(typeof(Search<PMTask.taskID,
        //                          Where<PMTask.projectID, Equal<Current<PMProjectCostFilter.contractID>>>>),
        //            SubstituteKey = typeof(PMTask.taskCD),
        //            DescriptionField = typeof(PMTask.description))]
        //[PXDimension(ProjectTaskAttribute.DimensionName)]
        //protected void PMCostBudget_ProjectTaskID_CacheAttached(PXCache sender) { }
        //#endregion

        #region Search
        //public PXSelect<RQRequest, Where<RQRequest.orderNbr, Equal<Current<RQRequest.orderNbr>>, And<RQRequest.reqClassID, Equal<Current<RQRequest.reqClassID>>>>> CurrentDocument;

        public static PXResultset<PMAccountGroup> getPMAccountGroup(int? groupID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<PMAccountGroup> set = PXSelect<PMAccountGroup,
                    Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(graph, groupID);
            return set;
        }

        public static PXResultset<PMCostBudget> getPMCostBudget(int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<PMCostBudget> set = PXSelect<PMCostBudget, Where<PMCostBudget.costCodeID, Equal<Required<PMCostBudget.costCodeID>>
                    , And<PMCostBudget.projectTaskID, Equal<Required<PMCostBudget.projectTaskID>>
                    , And<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                        And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>,
                        And<PMCostBudget.accountGroupID, Equal<Required<PMCostBudget.accountGroupID>>>>>>>>.
                    Select(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID);
            return set;
        }
        
        public  Decimal? getCommitQty(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID) {
            PXResultset<RQRequestLine> lines = getRQRequestLine(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);
            Decimal? sum = 0;
            foreach (RQRequestLine line in lines)
            {
                RQRequest rqRequest = PXSelect<RQRequest, Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>.
               Select(graph, line.OrderNbr);
                //noPurchaseOrder
                //統購不起單也要檢查預算 20200828金榮通知
                //if (!noPurchaseOrder.Equals(rqRequest.ReqClassID) && !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                if ( !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                {
                    sum = sum + getValue(line.OrderQty);
                }
            }
            return sum;
        }
               
        public Decimal? getCommitAmt(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            PXResultset<RQRequestLine> lines = getRQRequestLine(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);
            Decimal? sum = 0;
            
            foreach (RQRequestLine line in lines)
            {
                RQRequest rqRequest = PXSelect<RQRequest, Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>.
               Select(graph, line.OrderNbr);
                //noPurchaseOrder
                //統購不起單也要檢查預算 20200828r金榮通知
                //if (!noPurchaseOrder.Equals(rqRequest.ReqClassID) && !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                if ( !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                {
                    sum = sum + getValue(line.CuryEstExtCost);
                }
            }
            return sum;
        }

        //包含本單子
        public Decimal? getPreviousCommitQty(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            RQRequest master = Base.Document.Current;
            PXResultset<RQRequestLine> lines = getRQRequestLine(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);

            Decimal? sum = 0;
            foreach (RQRequestLine line in lines)
            {
                RQRequest rqRequest = PXSelect<RQRequest, Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>.
               Select(graph, line.OrderNbr);
                if (line.OrderNbr != null && !line.OrderNbr.Equals(master.OrderNbr))
                {
                    //noPurchaseOrder
                    //統購不起單也要檢查預算 20200828金榮通知
                    //if (!noPurchaseOrder.Equals(rqRequest.ReqClassID) && !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                    if ( !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                    {
                        sum = sum + getValue(line.OrderQty);
                    }
                }
            }

            return sum;
        }
        //包含本單子
        public Decimal? getPreviousCommitAmt(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            RQRequest master = Base.Document.Current;
            PXResultset<RQRequestLine> lines = getRQRequestLine(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);

            Decimal? sum = 0;
            foreach (RQRequestLine line in lines)
            {
                RQRequest rqRequest = PXSelect<RQRequest, Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>.
               Select(graph, line.OrderNbr);
                //noPurchaseOrder
                //統購不起單也要檢查預算 20200828金榮通知
                //if (!noPurchaseOrder.Equals(rqRequest.ReqClassID) && !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                if ( !RQRequestStatus.Canceled.Equals(rqRequest.Status))
                {
                    if (line.OrderNbr != null && !line.OrderNbr.Equals(master.OrderNbr))
                    {
                        sum = sum + getValue(line.CuryEstExtCost);
                    }
                }
            }
            return sum;
        }

        public static PXResultset<RQRequestLine> getRQRequestLine(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID,String orderNbr)
        {
            PXResultset<RQRequestLine> set = null;
            if (orderNbr == null)
            {
                set = PXSelect<RQRequestLine, Where<RQRequestLineExt.usrCostCodeID, Equal<Required<RQRequestLineExt.usrCostCodeID>>

                    , And<RQRequestLineExt.usrProjectTaskID, Equal<Required<RQRequestLineExt.usrProjectTaskID>>
                    , And<RQRequestLineExt.usrContractID, Equal<Required<RQRequestLineExt.usrContractID>>,
                        And<RQRequestLine.inventoryID, Equal<Required<RQRequestLine.inventoryID>>,
                        And<RQRequestLineExt.usrAccountGroupID, Equal<Required<RQRequestLineExt.usrAccountGroupID>>>>>>>>.
                    Select(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID);
            }
            else {
                set = PXSelect<RQRequestLine, Where<RQRequestLineExt.usrCostCodeID, Equal<Required<RQRequestLineExt.usrCostCodeID>>

                    , And<RQRequestLineExt.usrProjectTaskID, Equal<Required<RQRequestLineExt.usrProjectTaskID>>
                    , And<RQRequestLineExt.usrContractID, Equal<Required<RQRequestLineExt.usrContractID>>,
                        And<RQRequestLine.inventoryID, Equal<Required<RQRequestLine.inventoryID>>,
                        And<RQRequestLineExt.usrAccountGroupID, Equal<Required<RQRequestLineExt.usrAccountGroupID>>,
                        And<RQRequestLine.orderNbr, NotEqual<Required<RQRequestLine.orderNbr>>>>>>>>>.
                    Select(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, orderNbr);
            }
            return set;
        }

        #endregion

        #region Agent Flow
        //CS201010
        public PXSetup<KGSetUp> KGSetup;
        public PXSelect<KGFlowRequisitions, Where<KGFlowRequisitions.requisitionsNo, Equal<Current<RQRequest.orderNbr>>>> KGFlowReqs;
        public PXSelect<KGFlowRequisitionsDetail, Where<KGFlowRequisitionsDetail.reqUID, Equal<Current<KGFlowRequisitions.reqUID>>>> KGFlowReqsDetail;
        public PXSelect<KGFlowUploadFile, Where<KGFlowUploadFile.refNo, Equal<Current<RQRequest.orderNbr>>>> KGFlowReqsFiles;
        public PXAction<RQRequestLine> webServIntegration;
        [PXUIField(DisplayName = "AgentFlow", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable WebServIntegration(PXAdapter adapter)
        {
            RQRequest master = Base.Document.Current;
            RQRequestExt rqRequestExt = PXCache<RQRequest>.GetExtension<RQRequestExt>(master);
            String message = createAgentFlow();
            message = null;
            if (message != null)
            {
                throw new PXException(message);
            }

            return adapter.Get();
        }

        public String createAgentFlow()
        {
            //刪除既有資料
            foreach (KGFlowRequisitions kgFlowRequisitions   in  KGFlowReqs.Select()) {
                 PXResultset<KGFlowRequisitionsDetail>    set =      PXSelect<KGFlowRequisitionsDetail, 
                                Where<KGFlowRequisitionsDetail.reqUID, Equal<Required<KGFlowRequisitions.reqUID>>>>.Select(Base, kgFlowRequisitions.ReqUID);
                foreach (KGFlowRequisitionsDetail kgFlowRequisitionsDetail in set) {
                    KGFlowReqsDetail.Delete(kgFlowRequisitionsDetail);
                }
                 KGFlowReqs.Delete(kgFlowRequisitions);
            }
            Base.Actions.PressSave();

            if (Base.Lines.Select().Count > 0)
            {
                KGFlowRequisitions kgFlowRequisitions = (KGFlowRequisitions)KGFlowReqs.Cache.CreateInstance();
                RQRequest master = Base.Document.Current;
                //PX.Data.Update.PXInstanceHelper.CurrentCompany;
                //add by louis to get branch name
                PX.Objects.GL.Branch branch = PXSelect<PX.Objects.GL.Branch, Where<PX.Objects.GL.Branch.branchID, Equal<Required<RQRequest.branchID>>>>.Select(Base, master.BranchID);
                //kgFlowRequisitions.DeptName = Base.Accessinfo.CompanyName;
                //add by louis to get branch acctname for Dept Name
                if (branch != null) {
                    kgFlowRequisitions.DeptName = branch.AcctName;
                }
                RQRequestLine rqRequestLine = Base.Lines.Select();
                RQRequestLineExt rqRequestLineExt = Base.Lines.Cache.GetExtension<RQRequestLineExt>(rqRequestLine);
                RQRequestExt rqRequestExt = PXCache<RQRequest>.GetExtension<RQRequestExt>(master);
                Contract contract = PXSelect<Contract,
                        Where<Contract.contractID,
                        Equal<Required<Contract.contractID>>>>
                        .Select(Base, rqRequestLineExt.UsrContractID);
                kgFlowRequisitions.ProjectName = contract.Description;
                kgFlowRequisitions.ProjectCode = contract.ContractCD;
                BAccount bAccount = PXSelect<BAccount,
                    Where<BAccount.bAccountID,
                    Equal<Required<BAccount.bAccountID>>>>
                    .Select(Base, master.EmployeeID);
                kgFlowRequisitions.CreateUserName = bAccount.AcctName;
                kgFlowRequisitions.RequisitionsNo = master.OrderNbr;
                kgFlowRequisitions.UsrDefinedName = master.Description;
                //modify by louis 20190724
                kgFlowRequisitions.NeedDate = master.OrderDate;
                kgFlowRequisitions.RequisitionsExplain = rqRequestExt.UsrExplain;
                //待處理 非空
                kgFlowRequisitions.StateUID = "";
                kgFlowRequisitions.Message = "";
                kgFlowRequisitions = KGFlowReqs.Insert(kgFlowRequisitions);

                foreach (RQRequestLine line in Base.Lines.Select())
                {
                    KGFlowRequisitionsDetail kgFlowRequisitionsDetail = (KGFlowRequisitionsDetail)KGFlowReqsDetail.Cache.CreateInstance();
                    InventoryItem inventoryItem = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryID,
                        Equal<Required<InventoryItem.inventoryID>>>>
                        .Select(Base, line.InventoryID);

                    //kgFlowRequisitionsDetail.reqUID = new DateTime().Ticks+"";
                    kgFlowRequisitionsDetail.PccesCode = inventoryItem.InventoryCD;
                    kgFlowRequisitionsDetail.ItemName = line.Description;
                    kgFlowRequisitionsDetail.ItemQty = line.OpenQty;
                    kgFlowRequisitionsDetail.ItemUnit = line.UOM;
                    kgFlowRequisitionsDetail.ItemCost = line.CuryEstUnitCost;
                    kgFlowRequisitionsDetail.ItemAmt = line.CuryEstExtCost;
                    KGFlowReqsDetail.Insert(kgFlowRequisitionsDetail);
                }
                //using (PXTransactionScope ts = new PXTransactionScope())
                //{
                    Base.Actions.PressSave();
                    EPApproval epApproval = PXSelect<EPApproval,
                                Where<EPApproval.refNoteID,
                                Equal<Required<EPApproval.refNoteID>>,And<EPApproval.status,Equal<Required<EPApproval.status>>>>>
                                .Select(Base, master.NoteID, EPApprovalStatus.Pending);
                    if (epApproval != null)
                    {
                        kgFlowRequisitions.ApprovalID = epApproval.ApprovalID;
                    }
                    KGFlowReqs.Update(kgFlowRequisitions);

                    foreach (KGRequestFile kgRequestFile in KGRequestFiles.Select())
                    {
                        if (kgRequestFile.NoteID == null)
                        {
                            continue;
                        }
                        NoteDoc notedoc = PXSelect<NoteDoc,
                             Where<NoteDoc.noteID,
                             Equal<Required<NoteDoc.noteID>>>>
                             .Select(Base, kgRequestFile.NoteID);
                        if (notedoc != null)
                        {
                            KGFlowUploadFile kgFlowUploadFile = (KGFlowUploadFile)KGFlowReqsFiles.Cache.CreateInstance();
                            kgFlowUploadFile.Fileid = notedoc.FileID;
                            //該檔案可能已經新增過了
                            KGFlowUploadFile checkFlowFile = PXSelect<KGFlowUploadFile,
                             Where<KGFlowUploadFile.fileid,
                             Equal<Required<KGFlowUploadFile.fileid>>>>
                             .Select(Base, notedoc.FileID);
                            if (checkFlowFile != null)
                            {
                                continue;
                            }
                            kgFlowUploadFile.FileType = kgRequestFile.FileType;
                            kgFlowUploadFile.RefNo = kgFlowRequisitions.ReqUID;
                            kgFlowUploadFile.Category = "REQ";
                            UploadFile uploadFile = PXSelect<UploadFile,
                                Where<UploadFile.fileID,
                                Equal<Required<UploadFile.fileID>>>>
                                .Select(Base, notedoc.FileID);
                            kgFlowUploadFile.FileName = uploadFile.Name; ;
                            string buildUtl = PXRedirectToFileException.BuildUrl(notedoc.FileID);
                            string Absouri = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;

                            buildUtl = buildUtl.Substring(1);
                            Absouri = Absouri.Substring(0, Absouri.IndexOf("/M"));
                            kgFlowUploadFile.FileLink = Absouri + buildUtl;
                            KGFlowReqsFiles.Insert(kgFlowUploadFile);
                        }
                    }
                    
                    rqRequestExt.UsrKGFlowUID = kgFlowRequisitions.ReqUID;
                    master=Base.Document.Update(master);
                    //Base.Document.Cache.SetValueExt<RQRequestExt.usrKGFlowUid>();
                    Base.Actions.PressSave();
                    bool ckeck=    callAgentFlow(kgFlowRequisitions.ReqUID, epApproval);
                    master = Base.Document.Current;
                    if (message != null) {
                    //throw new PXException(rqRequestExt.Message);
                        master.Hold = true;
                        master.Status = RQRequestStatus.Hold;
                        Base.Document.Update(master);
                        //Base.Actions.PressSave();
                        return message;
                    }
                    
                    if (!ckeck) {
                        return message;
                    }
                    //ts.Complete(); 
                //}
            }
            return null;
        }

        public bool callAgentFlow(String ReqUID, EPApproval epApproval)
        {
            RQRequest master = Base.Document.Current;
            RQRequestExt rqRequestExt = PXCache<RQRequest>.GetExtension<RQRequestExt>(master);
            message = null;
            String status = null;
            AgentFlowWebServiceUtil.createROProcessKedgeURL(Base.Accessinfo.UserName, ReqUID, rqRequestExt.UsrProjectID, ref status, ref message);
            //此時要是錯誤 message 會有值，Status也塞好了
            if (message != null) {
                return false;
            }
            //status is Call Web Servie return value
            if (status != null && status.StartsWith("http"))
            {
                if (epApproval != null)
                {
                    EPApprovalExt epApprovalExt = PXCache<EPApproval>.GetExtension<EPApprovalExt>(epApproval);
                    epApprovalExt.UsrReturnUrl = status;
                    rqRequestExt.UsrReturnUrl = status;
                    //Base.Approval.Update(epApproval);
                    master=Base.Document.Update(master);
                    Base.Actions.PressSave();
                    try
                    {
                        if (AgentFlowWebServiceUtil.getIsAutoApproved(rqRequestExt.UsrProjectID)==true) {
                            //master.Approved = true;
                            //Base.Document.Update(master);
                            ApprovalUtil.Approve(epApproval.ApprovalID,ref approveMessage);
                            //callAutoApproval();
                        }
                    }
                    catch (Exception e)
                    {
                        approveMessage= "自動核准失敗";
                        //return false;
                    }
                }
                //Base.Document.Cache.RaiseExceptionHandling<RQRequest.status>(master, master.Status, new PXSetPropertyException("AgentFlowµLªk¼g¤JcreateROProcess"));
            }
            else
            {
                message = "呼叫AgentFlow失敗:" + status;
                return false;
            }
            return true;
        }

        //"統購不啟單"
        public const String noPurchaseOrder = "30";
        public static Decimal? getValue(Decimal? value)
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

        protected virtual void RQRequest_Status_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            RQRequest master = (RQRequest)e.Row; ;
            String status = (String)e.OldValue;

            if ((master.Hold == false || master.Hold == null)
                && RQRequestStatus.PendingApproval.Equals(master.Status) && RQRequestStatus.Hold.Equals(status))
            {

                if (noPurchaseOrder.Equals(master.ReqClassID))
                {
                    master.Status = RQRequestStatus.Open;
                    master.Approved = true;
                    return;
                }

                if (RQRequestStatus.PendingApproval.Equals(master.Status))
                {
                    RQRequestExt rqRequestExt = PXCache<RQRequest>.GetExtension<RQRequestExt>(master);
                    //rqRequestExt.Message = "錯誤";
                    createAgentFlow();
                    //Base.Document.Cache.RaiseExceptionHandling<RQRequest.status>(master, master.Status, new PXSetPropertyException("AgentFlow¼g¤JcreateROProcess²§±`"));
                    //webServIntegration.Press();
                }
            };
        }
        #endregion
    }

    #region Custom Filter
    [Serializable]
    public class PMProjectCostFilter : IBqlTable
    {
        /// <summary>
        /// Get or sets ProjectID
        /// </summary>
        [PXInt]
        [PXDimensionSelectorAttribute(ProjectAttribute.DimensionName,
                        typeof(Search2<PMProject.contractID,
                        LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>>,
                        Where<PMProject.baseType, Equal<CTPRType.project>, 
                            And<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>,
                        And<PMProject.status, Equal<ProjectStatus.active>,
                        And2<Match<Current<AccessInfo.userName>>, Or<PMProject.nonProject, Equal<True>>>>>>>)
                        , typeof(PMProject.contractCD), typeof(PMProject.contractCD), typeof(PMProject.description),
                        typeof(PMProject.status), typeof(PMProject.customerID), typeof(Customer.acctName), typeof(PMProject.curyID))]
        
        /*
        [PXSelector(typeof(Search2<PMProject.contractID,
                                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID, Equal<PMProject.contractID>>>>,
                                    Where<PMProject.baseType, Equal<CTPRType.project>,
                                        And<PMProject.status, Equal<ProjectStatus.active>,
                                        And<PMProject.nonProject, Equal<False>, 
                                        And<Match<Current<AccessInfo.userName>>>>>>>),
                    typeof(PMProject.contractCD),
                    typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID),
                    SubstituteKey = typeof(PMProject.contractCD),
                    DescriptionField = typeof(PMProject.description))]*/
        [PXUIField(DisplayName = "Project ID")]
        [PXDefault(typeof(Current<RQRequestExt.usrProjectID>))]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
    }
    #endregion 
}
