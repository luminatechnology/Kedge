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
using PX.Objects.IN;
using PX.SM;

namespace Kedge
{
    /**
     * ===2021/05/25 :Louis口頭 === Althea
     * 優化效能
     * ===2021/06/10 :0012073 === Althea
     * Action的更新資料改為PXUpdate 
     * 因為用update persist他都沒有更新DB
     * ===2021/06/15 :0012094 ===Althea
     * Filter Add : ValuationType = 3(票貼)不需要廠商確認
     **/
    public class KGValuationConfirmProcess : PXGraph<KGValuationConfirmProcess>
    {
        #region Filter

        public PXFilter<ValuationConfirmFilter> ValuationConfirmFilter;

        public PXSelectJoin<KGValuationDetail,
             InnerJoin<KGValuation,
                 On<KGValuation.valuationID,
                 Equal<KGValuationDetail.valuationID>,
                 And<KGValuation.status, Equal<word.c>,
                 And<KGValuationDetail.pricingType, Equal<word.a>,
                 And<KGValuation.dailyRenterCD, IsNull>>>>>> ValuationDetailA;

        public PXSelect<ViewTable, Where<True, Equal<True>,
                        And2<Where<
                              ViewTable.orderNbr, Equal<Current2<ValuationConfirmFilter.orderNbr>>,
                              Or<Current2<ValuationConfirmFilter.orderNbr>, IsNull>>,
                       And2<Where<
                               ViewTable.contractID, Equal<Current2<ValuationConfirmFilter.contractID>>,
                               Or<Current2<ValuationConfirmFilter.contractID>, IsNull>>,
                         And2<Where<
                               ViewTable.vendorID, Equal<Current2<ValuationConfirmFilter.vendorID>>,
                               Or<Current2<ValuationConfirmFilter.vendorID>, IsNull>>,
                         And2<Where<
                               ViewTable.valuationDate, GreaterEqual<Current2<ValuationConfirmFilter.valuationDateFrom>>,
                               Or<Current2<ValuationConfirmFilter.valuationDateFrom>, IsNull>>,
                         And2<Where<
                               ViewTable.valuationDate, LessEqual<Current2<ValuationConfirmFilter.valuationDateTo>>,
                               Or<Current2<ValuationConfirmFilter.valuationDateTo>, IsNull>>,
                         And2<Where<
                               ViewTable.status, GreaterEqual<Current2<ValuationConfirmFilter.status>>,
                               Or<Current2<ValuationConfirmFilter.status>, IsNull>>,
                         And<Where<
                               ViewTable.createdByID, Equal<Current2<ValuationConfirmFilter.createdByID>>,
                               Or<Current2<ValuationConfirmFilter.createdByID>, IsNull>>
                        >>>>>>>>> ValuationDetails;
        public KGValuationConfirmProcess()
            : base()
        {
            ValuationDetails.OrderByNew<OrderBy<Desc<ViewTable.valuationID>>>();
            
        }

        #endregion

        #region Action

        #region SendValuationApprovalAction
        public PXAction<ValuationConfirmFilter> SendValuationApprovalAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Print & Sign")]
        public virtual IEnumerable sendValuationApprovalAction(PXAdapter adapter, [PXString]
            string reportID)
        {
            bool setCheckThread = false;
            //KGValuationDetail row = ValuationDetails.Current;
            //2019/06/18把Selected拿回來 加上BatchNbr以便Report查詢
            KGValuationConfirmProcess graph = PXGraph.CreateInstance<KGValuationConfirmProcess>();
            string batchnbr = graph.Accessinfo.UserName
                + DateTime.Now.ToString("yyyyMMddHHmmss");
            PXResultset<ViewTable> viewTableset = GetSelectedViewTable();
            if (viewTableset.Count == 0) return adapter.Get();

            //For Check
            foreach (ViewTable valuationDetail in viewTableset)
            {
                switch(valuationDetail.Status)
                {
                    case "V":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusV, PXErrorLevel.RowError));
                        break;

                    case "S":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusS, PXErrorLevel.RowError));
                        break;

                    case "B":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusB, PXErrorLevel.RowError));
                        break;
                }
               
            }

            #region 2019/05/30改成report開啟新頁面
            /*if (reportID == null) reportID = "KG603000";
            if (row != null)
            {
                Dictionary<string, string> mailParams = new Dictionary<string, string>();
                mailParams["ValuationDetailID"] = row.ValuationDetailID.ToString();
                throw new PXReportRequiredException(mailParams, reportID, "Report");
            }*/
            #endregion

            if (setCheckThread) throw new Exception(ErrorSelectStatus);


            foreach (ViewTable kgvaluationDetail in viewTableset)
            {

                if (kgvaluationDetail.Status == "C")
                {
                    PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>,
                        Set<KGValuationDetail.batchNbr, Required<KGValuationDetail.batchNbr>>>,
                        KGValuationDetail,
                        Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                        .Update(this,"S", batchnbr,kgvaluationDetail.ValuationDetailID);
                    
                    /*kgvaluationDetail.Status = "S";
                    kgvaluationDetail.BatchNbr = batchnbr;*/
                    //2019/10/29 DetailA& DetailD 狀態要一致
                    KGValuationDetail detailA = GetDetailA(kgvaluationDetail.ValuationID);
                    if (detailA != null)
                    {
                        PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>>,
                        KGValuationDetail,
                        Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                        .Update(this, "S", detailA.ValuationDetailID);
                        /*
                        detailA.Status = "S";
                        ValuationDetailA.Update(detailA);
                        */
                    }

                    kgvaluationDetail.Selected = false;
                    ValuationDetails.Update(kgvaluationDetail);
                }             
            }
            this.Persist();
            ValuationDetails.View.RequestRefresh();
            ValuationDetails.Cache.Clear();
            ValuationDetails.Cache.ClearQueryCache();
            NewPageToDisplayReport(batchnbr);
            return adapter.Get();
        }
        #endregion

        #region valuationVendorConfirmAction
        public PXAction<ValuationConfirmFilter> ValuationVendorConfirmAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Vendor Confirm", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable valuationVendorConfirmAction(PXAdapter adapter)
        {
            bool setCheckThread = false;
            PXResultset<ViewTable> viewtableset = GetSelectedViewTable();
            if (viewtableset.Count == 0) return adapter.Get();
            //KGValuationDetail row = ValuationDetails.Current;

            //For Check
            foreach (ViewTable valuationDetail in viewtableset)
            {
                //mark by louis for not checking File when Vendor Confirm
                /**
                if (valuationDetail.Status == "S")
                {

                    Guid[] files = PXNoteAttribute.GetFileNotes(ValuationDetails.Cache, valuationDetail);
                    if (files.Length == 0)
                    {
                        //base.Clear();
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                     valuationDetail, valuationDetail.Status, new PXSetPropertyException("請夾檔案!", PXErrorLevel.RowError));
                        //throw new PXException("請夾檔案!");
                    }
                    
                }else **/
                switch(valuationDetail.Status)
                {
                    case "C":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusC, PXErrorLevel.RowError));
                        break;
                    case "V":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusV, PXErrorLevel.RowError));
                        break;
                    case "B":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusB, PXErrorLevel.RowError));
                        break;

                }

            }

            if (setCheckThread) throw new Exception(ErrorSelectStatus);

            foreach (ViewTable kgvaluationDetail in viewtableset)
            {
                if (kgvaluationDetail != null)
                {
                    if (kgvaluationDetail.Selected == true)
                    {
                        if (kgvaluationDetail.Status == "S")
                        {
                            
                            PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>>,
                               KGValuationDetail,
                               Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                               .Update(this, "V", kgvaluationDetail.ValuationDetailID);
                            //kgvaluationDetail.Status = "V";
                            //2019/10/29 DetailA& DetailD 狀態要一致
                            KGValuationDetail detailA = GetDetailA(kgvaluationDetail.ValuationID);
                            if (detailA != null)
                            {
                                PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>>,
                               KGValuationDetail,
                               Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                               .Update(this, "V", detailA.ValuationDetailID);
                                /*
                                detailA.Status = "V";
                                ValuationDetailA.Update(detailA);
                                */
                            }

                            kgvaluationDetail.Selected = false;
                            ValuationDetails.Update(kgvaluationDetail);
                        }
                    }
                }

                base.Persist();
            }
            ValuationDetails.View.RequestRefresh();
            ValuationDetails.Cache.Clear();
            ValuationDetails.Cache.ClearQueryCache();

            return adapter.Get();
        }
        #endregion

        #region UnSignValuationAction
        public PXAction<ValuationConfirmFilter> UnSignValuationAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "UnSign", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable unSignValuationAction(PXAdapter adapter)
        {
            bool setCheckThread = false;
            PXResultset<ViewTable> viewtableset = GetSelectedViewTable();

            //For Check
            foreach (ViewTable valuationDetail in viewtableset)
            {
                //2020/06/15 ADD 檢核DetailA狀態為以計價,應該要擋住不給更改狀態
                checkDetailAStatus(valuationDetail.ValuationID, 1);

                switch (valuationDetail.Status)
                {
                    case "V":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusV, PXErrorLevel.RowError));
                        break;

                    case "C":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusC, PXErrorLevel.RowError));
                        break;

                    case "B":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusB, PXErrorLevel.RowError));
                        break;
                }

            }
            if (setCheckThread) throw new Exception(ErrorSelectStatus);

            foreach (ViewTable kGValuationDetail in viewtableset)
            {

                if (kGValuationDetail.Status == "S")
                {
                    PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>>,
                           KGValuationDetail,
                           Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                           .Update(this, "C", kGValuationDetail.ValuationDetailID);
                    /*
                    kGValuationDetail.Selected = false;
                    kGValuationDetail.Status = "C";
                    */
                    //2019/10/29 DetailA& DetailD 狀態要一致
                    KGValuationDetail detailA = GetDetailA(kGValuationDetail.ValuationID);
                    if (detailA != null)
                    {
                        PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>>,
                           KGValuationDetail,
                           Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                           .Update(this, "C", detailA.ValuationDetailID);
                        /*
                        detailA.Status = "C";
                        ValuationDetailA.Update(detailA);
                        */
                    }
                    kGValuationDetail.Selected = false;
                    ValuationDetails.Update(kGValuationDetail);

                }
         
            }
            base.Persist();
            ValuationDetails.View.RequestRefresh();
            ValuationDetails.Cache.Clear();
            ValuationDetails.Cache.ClearQueryCache();
            return adapter.Get();
        }
        #endregion

        #region UnConfirmValuationAction
        public PXAction<ValuationConfirmFilter> UnConfirmValuationAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Vendor UnConfirm", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable unConfirmValuationAction(PXAdapter adapter)
        {
            bool setCheckThread = false;
            PXResultset<ViewTable> viewtableset = GetSelectedViewTable();

            foreach (ViewTable valuationDetail in viewtableset)
            {
                //2020/06/15 ADD 檢核DetailA狀態為以計價,應該要擋住不給更改狀態
                checkDetailAStatus(valuationDetail.ValuationID, 2);

                switch (valuationDetail.Status)
                {
                    case "C":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusC, PXErrorLevel.RowError));
                        break;

                    case "S":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusS, PXErrorLevel.RowError));
                        break;

                    case "B":
                        setCheckThread = true;
                        ValuationDetails.Cache.RaiseExceptionHandling<KGValuationDetail.status>(
                         valuationDetail, valuationDetail.Status, new PXSetPropertyException(ErrorStatusB, PXErrorLevel.RowError));
                        break;
                }

            }
                
            
            if(setCheckThread) throw new Exception(ErrorSelectStatus);

            foreach (ViewTable kGValuationDetail in viewtableset)
            {
                if (kGValuationDetail != null)
                {
                    if (kGValuationDetail.Selected == true)
                    {
                        if (kGValuationDetail.Status == "V")
                        {
                            PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>>,
                               KGValuationDetail,
                               Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                               .Update(this, "S", kGValuationDetail.ValuationDetailID);
                            /*
                            kGValuationDetail.Selected = false;
                            kGValuationDetail.Status = "S";
                            */
                            //2019/10/29 DetailA& DetailD 狀態要一致
                            KGValuationDetail detailA = GetDetailA(kGValuationDetail.ValuationID);
                            if (detailA != null)
                            {
                                PXUpdate<Set<KGValuationDetail.status, Required<KGValuationDetail.status>>,
                                   KGValuationDetail,
                                   Where<KGValuationDetail.valuationDetailID, Equal<Required<KGValuationDetail.valuationDetailID>>>>
                                   .Update(this, "S", detailA.ValuationDetailID);
                                /*
                                detailA.Status = "S";
                                ValuationDetailA.Update(detailA);
                                */
                            }

                            kGValuationDetail.Selected = false;
                            ValuationDetails.Update(kGValuationDetail);
                        }
                    }
                }
            }
            base.Persist();
            ValuationDetails.View.RequestRefresh();
            ValuationDetails.Cache.Clear();
            ValuationDetails.Cache.ClearQueryCache();
            return adapter.Get();
        }
        #endregion

        #endregion

        #region Link
        public PXAction<ValuationConfirmFilter> ViewValuationConfirm;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Confirm Valuation", Visible = false)]
        protected virtual void viewValuationConfirm()
        {
            KGValuationDetail row = ValuationDetails.Current;
            // Creating the instance of the graph
            KGValuationEntry graph = PXGraph.CreateInstance<KGValuationEntry>();
            // Setting the current product for the graph
            graph.Valuations.Current = graph.Valuations.Search<KGValuation.valuationID>(
            row.ValuationID);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Valuations.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "Valuation Details");
            }

        }
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
        protected void _(Events.RowSelected<ViewTable> e)
        {
            ViewTable row = e.Row;
            if (row == null) return;
            setUIEnabled();
        }
        #endregion

        #region Method
        private void setUIEnabled()
        {
            PXUIFieldAttribute.SetReadOnly(ValuationDetails.Cache, null,true);
            PXUIFieldAttribute.SetReadOnly<ViewTable.selected>(ValuationDetails.Cache, null, false);
            ValuationDetails.AllowInsert = false;
            ValuationDetails.AllowDelete = false;
        }
        protected virtual void NewPageToDisplayReport(string BatchNbr)
        {
     
            if (BatchNbr != null)
            {
                Dictionary<string, string> mailParams = new Dictionary<string, string>()
                {
                    ["BatchNbr"] =BatchNbr
                };
                var requiredException = new PXReportRequiredException(mailParams, "KG603000", PXBaseRedirectException.WindowMode.New, "廠商確認單");
                requiredException.SeparateWindows = true;
                //requiredException.AddSibling("KG601000", mailParams);
                throw new PXRedirectWithReportException(this, requiredException, "Preview");
            }


        }
        
        private KGValuationDetail GetDetailA(int? ValuationID)
        {
            return PXSelect<KGValuationDetail,
                Where<KGValuationDetail.valuationID, Equal<Required<KGValuation.valuationID>>,
                And<KGValuationDetail.pricingType, Equal<word.a>>>>
                .Select(this, ValuationID);
        }

        public void checkDetailAStatus(int? valuationID, int cancelbutton)
        {
            KGValuationDetailA detailA =
                PXSelect<KGValuationDetailA,
                Where< KGValuationDetailA.valuationID,
                Equal<Required<KGValuationDetailA.valuationID>>>>
                .Select(this, valuationID);
            if(detailA ==null)
            {
                return;
            }
            //cancelbutton 1.取消會簽 2.取消簽認
            string ErrorMsg = "此加款單已被計價,";
            if(detailA.Status == "B")
            {
                if(cancelbutton ==1 )
                {
                    ErrorMsg = ErrorMsg + "不可取消會簽!";
                }
                if(cancelbutton ==2)
                {
                    ErrorMsg = ErrorMsg + "不可取消簽認!";
                }
                throw new Exception(ErrorMsg);
            }
        }
        #endregion

        #region Select Method
        private PXResultset<ViewTable> GetSelectedViewTable()
        {
            return PXSelect<ViewTable, Where<ViewTable.selected, Equal<True>>>.Select(this);
        }
        #endregion

        #region Error Msg 
        /// <summary>
        /// 此狀態為「未會簽」。
        /// </summary>
        string ErrorStatusC = "此狀態為「未會簽」。" + "\n" + "若要會簽，請執行「列印及會簽」";
        /// <summary>
        /// 此狀態為「已簽認」。
        /// </summary>
        string ErrorStatusV = "此狀態為「已簽認」。" + "\n" + "若要解除，請執行「解除簽認」!";
        /// <summary>
        /// 此狀態為「已會簽」。
        /// </summary>
        string ErrorStatusS = "此狀態為「已會簽」。" + "\n" + "若要解除，請執行「解除會簽」；" + "\n" + "若要簽認，請執行「簽認及上傳附件」!";
        /// <summary>
        /// 此狀態已完成計價!
        /// </summary>
        string ErrorStatusB = "此狀態已完成計價! 不可變更。";
        /// <summary>
        /// 請確認勾選資料的狀態!
        /// </summary>
        string ErrorSelectStatus = "請確認勾選資料的狀態!";

        #endregion
    }

    #region ViewTable
    [Serializable]
    [PXHidden]
    [PXProjection(typeof(Select2<KGValuationDetail,
                    InnerJoin<KGValuation,
                        On<KGValuation.valuationID,Equal<KGValuationDetail.valuationID>>,
                    InnerJoin<POOrder,
                        On<POOrder.orderNbr,Equal<KGValuationDetail.orderNbr>>>>,
                    Where<KGValuation.status, Equal<word.c>,
                        //2021/06/15 Mantis: 0012094 會計票貼不用廠商確認
                        And<KGValuation.valuationType,NotEqual<KGValuationTypeStringList.three>,
                        And<KGValuationDetail.pricingType, Equal<word.d>,
                        And<KGValuation.dailyRenterCD, IsNull>>>>,
                    OrderBy<Desc<KGValuationDetail.valuationID>>>
        ), Persistent = false)]
    public class ViewTable : KGValuationDetail
    {
        //Valuation

        #region ValuationDate
        [PXDBDate(BqlField = typeof(KGValuation.valuationDate))]
        [PXUIField(DisplayName = "ValuationDate")]
        public virtual DateTime? ValuationDate { get; set; }
        public abstract class valuationDate : PX.Data.BQL.BqlDateTime.Field<valuationDate> { }
        #endregion

        #region ValuationDescr
        [PXDBString(BqlField = typeof(KGValuation.description))]
        [PXUIField(DisplayName = "ValuationDescr")]
        public virtual string ValuationDescr { get; set; }
        public abstract class valuationDescr : PX.Data.BQL.BqlString.Field<valuationDescr> { }
        #endregion



        //Users Show 
        /*
        #region CreatedByID
        [PXDBInt(BqlField = typeof(KGValuationDetail.createdByID))]
        [PXUIField(DisplayName = "CreateByID")]
        [PXSelector(typeof(Search<Users.pKID>))]

        public virtual Guid? CreateByID { get; set; }
        public abstract class createByID : PX.Data.BQL.BqlGuid.Field<createByID> { }
        #endregion
        */
        //Contract Show
        #region ContractCD
        [PXString()]
        [PXUIField(DisplayName = "ContractCD")]
        [PXUnboundDefault(typeof(Search<Contract.contractCD,
            Where<Contract.contractID,Equal<Current<ViewTable.contractID>>>>))]

        public virtual string ContractCD { get; set; }
        public abstract class contractCD : PX.Data.BQL.BqlString.Field<contractCD> { }
        #endregion

        #region ContractDescr
        [PXString()]
        [PXUIField(DisplayName = "Contract Descr")]
        [PXUnboundDefault(typeof(Search<Contract.description,
            Where<Contract.contractID, Equal<Current<ViewTable.contractID>>>>))]

        public virtual string ContractDescr { get; set; }
        public abstract class contractDescr : PX.Data.BQL.BqlString.Field<contractDescr> { }
        #endregion

        //POOrder Show
        #region VendorID
        [PXDBInt(BqlField = typeof(POOrder.vendorID))]
        [PXUIField(DisplayName = "VendorID")]
        [PXSelector(typeof(Search<BAccount.bAccountID>),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(BAccount.acctCD))]
 
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorAcctName
        [PXString()]
        [PXUIField(DisplayName = "AcctName")]
        [PXUnboundDefault(typeof(Search<BAccount.acctName,
            Where<BAccount.bAccountID,Equal<Current<vendorID>>>>))]

        public virtual string VendorAcctName { get; set; }
        public abstract class vendorAcctName : PX.Data.BQL.BqlString.Field<vendorAcctName> { }
        #endregion

    }
    #endregion

    [Serializable]
    public class ValuationConfirmFilter : IBqlTable
    {
        #region ContractID
        [PXInt()]
        [PXUIField(DisplayName = "Project", Required = true)]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [ProjectBaseAttribute()]
        [PXForeignReference(typeof(Field<contractID>.IsRelatedTo<PMProject.contractID>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region OrderNbr
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Purchase Order")]
        [PXSelector(typeof(Search5<KGValuationDetail.orderNbr,
            InnerJoin<POOrder, On<POOrder.orderNbr,
                Equal<KGValuationDetail.orderNbr>>,
            InnerJoin<KGValuation, On<KGValuation.valuationID,
                Equal<KGValuationDetail.valuationID>>>>,
            Where<KGValuation.status,Equal<word.c>>,
            Aggregate<GroupBy<KGValuationDetail.orderNbr>>>),
            typeof(POOrder.orderType),
            typeof(KGValuationDetail.orderNbr),
            typeof(POOrder.orderDate),
            typeof(POOrder.status),
            typeof(POOrder.vendorID),
            typeof(POOrder.vendorID_Vendor_acctName),
            typeof(POOrder.orderDesc)//,
            //DescriptionField = typeof(POOrder.orderDesc)
       )]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : IBqlField { }
        #endregion

        #region VendorID
        [PXInt()]
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, 
            DescriptionField = typeof(Vendor.acctName), 
            CacheGlobal = true, 
            Filterable = true)]
        [PXUIField(DisplayName = "Vendor")]
        [PXForeignReference(typeof(Field<ValuationConfirmFilter.vendorID>.IsRelatedTo<BAccount.bAccountID>))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : IBqlField { }
        #endregion

        #region ValuationDateFrom
        [PXDate()]
        [PXUIField(DisplayName = "Valuation Date From")]
        public virtual DateTime? ValuationDateFrom { get; set; }
        public abstract class valuationDateFrom : IBqlField { }
        #endregion

        #region ValuationDateTo
        [PXDate()]
        [PXUIField(DisplayName = "Valuation Date To")]
        public virtual DateTime? ValuationDateTo { get; set; }
        public abstract class valuationDateTo : IBqlField { }
        #endregion

        #region Status
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Status")]
        [PXStringList(new string[] { "C", "S", "V", "B" },
            new string[] { "CONFIRM", "PENDING SIGN", "VENDOR CONFIRM", "BILLED" })]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
        #endregion

        /* #region CreatedByID
         [PXGuid()]
         [PXSelector(typeof(Search5<KGValuationDetail.createdByID,
             InnerJoin<Users,On<Users.pKID,Equal<KGValuationDetail.createdByID>>>,
             Aggregate<GroupBy<KGValuationDetail.createdByID>>>),
             typeof(KGValuationDetail.createdByID),
             typeof(Users.fullName),
             SubstituteKey =typeof(Users.username))]
         public virtual Guid? CreatedByID { get; set; }
         public abstract class createdByID : IBqlField { }
         #endregion
         */
        //先找username在找guid

        #region CreatedByID
        [PXGuid()]
        [PXUIField(DisplayName = "CreatedByID")]
        [PXSelector(typeof(Search<Users.pKID>),
            typeof(Users.username),
             typeof(Users.displayName),
             typeof(Users.fullName),
             SubstituteKey = typeof(Users.username))]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion
       
    }
}