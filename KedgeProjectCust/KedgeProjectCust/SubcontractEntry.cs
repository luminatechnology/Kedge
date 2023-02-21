using System;
using PX.Data;
using PX.Objects.RQ;
using PX.Objects.PM;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.AP;
using System.Collections;
using PX.Objects.CM;
using Kedge.DAC;
using PX.Objects.IN;
using PX.Objects.CS;

namespace PX.Objects.CN.Subcontracts.SC.Graphs
{ 
    public class SubcontractEntry_Extension : PXGraphExtension<SubcontractEntry>
    {
        public class INUnitEntity : PXGraph<INUnitEntity, INUnit>
        {
            public PXSelect<INUnit> inUnits;
            public void doUnitTransform(PMCostBudgetExt pmCost)
            {

                INUnit unit = PXSelect<INUnit, Where<INUnit.inventoryID, Equal<Required<INUnit.inventoryID>>,
                                               And<INUnit.unitType, Equal<Required<INUnit.unitType>>,
                                               And<INUnit.fromUnit, Equal<Required<INUnit.fromUnit>>,
                                               And<INUnit.toUnit, Equal<Required<INUnit.toUnit>>>>>>>.Select(this, pmCost.InventoryID, 1, pmCost.VendorUom, pmCost.UOM);
                if (unit == null)
                {
                    //INUnit newUnit= (INUnit)inUnits.Cache.CreateInstance();
                    INUnit newUnit = new INUnit();
                    newUnit.UnitType = 1;
                    newUnit.FromUnit = pmCost.VendorUom;
                    newUnit.ToUnit = pmCost.UOM;
                    newUnit.UnitRate = 1;
                    newUnit.PriceAdjustmentMultiplier = 1;
                    newUnit.InventoryID = pmCost.InventoryID;

                    if (inUnits == null)
                    {
                        inUnits = new PXSelect<INUnit>(this);

                    }
                    inUnits.Insert(newUnit);
                    //SubcontractEntry_INUnit subcontractEntry_INUnit =   PXGraph.CreateInstance<SubcontractEntry_INUnit>();
                    //subcontractEntry_INUnit.in;
                }
            }
        }
    
        [Serializable]
        [PXProjection(typeof(Select2<PMBudget,
                                LeftJoin<KGVendorPrice, On<KGVendorPrice.inventoryID, Equal<PMBudget.inventoryID>,
                                                    And<KGVendorPrice.vendorID, Equal<CurrentValue<POOrder.vendorID>>,
                                                    And2<Where<KGVendorPrice.areaCode, Equal<CurrentValue<PMProjectCostFilter.areaCode>>, 
                                                        Or<KGVendorPrice.areaCode, Equal<CurrentValue<PMProjectCostFilter.centerAreaCode>>>>,
                                                    And<KGVendorPrice.effectiveDate, LessEqual<CurrentValue<PMProjectCostFilter.date>>,
                                                    And<Where<KGVendorPrice.expirationDate, IsNull,
                                                            Or<KGVendorPrice.expirationDate, GreaterEqual<CurrentValue<PMProjectCostFilter.date>>>>>>>>>>,
                                Where<PMBudget.projectID, Equal<CurrentValue<PMProjectCostFilter.contractID>>,
                                    And<PMBudgetExt.usrInvPrType, Equal<One>, And<PMBudget.type, Equal<E>>>>,
                                OrderBy<Asc<PMBudget.projectTaskID,
                                            Asc<PMBudget.costCodeID,
                                                Asc<PMBudget.accountGroupID>>>>>))]
        public class PMCostBudgetExt : PMBudget
        {       
            #region ProjectID
            [PXDBInt(IsKey = true, BqlField = typeof(PMBudget.projectID))]
            public new virtual int? ProjectID { get; set; }
            public new abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region inventoryID
            [PXDBInt(IsKey = true, BqlField = typeof(PMBudget.inventoryID))]
            [PXSelector(typeof(InventoryItem.inventoryID), SubstituteKey = typeof(InventoryItem.inventoryCD), DescriptionField = typeof(InventoryItem.descr))]
            public new virtual int? InventoryID { get; set; }
            public new abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
            #endregion

            #region CostCodeID
            [PXDBInt(IsKey = true, BqlField = typeof(PMBudget.costCodeID))]
            [PXDimensionSelector("COSTCODE", typeof(Search<PMCostCode.costCodeID>),
                typeof(PMCostCode.costCodeCD), typeof(PMCostCode.description), typeof(PMCostCode.isDefault), DescriptionField = typeof(PMCostCode.description))]
            public new virtual int? CostCodeID { get; set; }
            public new abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }
            #endregion

            #region inventoryID
            [PXDBInt( BqlField = typeof(KGVendorPrice.inventoryID))]
            [PXSelector(typeof(InventoryItem.inventoryID), SubstituteKey = typeof(InventoryItem.inventoryCD),DescriptionField = typeof(InventoryItem.descr))]
            public virtual int? VendorInventoryID { get; set; }
            public abstract class vendorInventoryID : PX.Data.BQL.BqlInt.Field<vendorInventoryID> { }
            #endregion

            #region VendorPriceID
            [PXDBInt(BqlField = typeof(KGVendorPrice.vendorPriceID),IsKey = true)]
            [PXUIField(DisplayName = "Vendor Price ID")]
            public virtual int? VendorPriceID { get; set; }
            public abstract class vendorPriceID : PX.Data.BQL.BqlInt.Field<vendorPriceID> { }
            #endregion

            #region UsrInventoryDesc
            [PXDBString(BqlField = typeof(PMBudgetExt.usrInventoryDesc))]
            [PXUIField(DisplayName = "UsrInventoryDesc")]
            public virtual String UsrInventoryDescExt { get; set; }
            public abstract class usrInventoryDescExt : IBqlField { }
            #endregion

            #region VProjectTaskID
            public abstract class vProjectTaskID : PX.Data.BQL.BqlInt.Field<vProjectTaskID>
            {
            }
            [PXInt()]
            [PXDimensionSelector("PROTASK", typeof(Search<PMTask.taskID>),
                                        typeof(PMTask.taskCD),
                                        typeof(PMTask.description),
                                        typeof(PMTask.status), typeof(PMCostCode.isDefault),
                             DescriptionField = typeof(PMTask.description))]
            [PXUIField(DisplayName = "Sub Job")]
            //[PXFormula(typeof(Current<PMBudget.projectTaskID>))]
            public  virtual Int32? VProjectTaskID
            {
                get { return ProjectTaskID; }
                set { ProjectTaskID = VProjectTaskID; }
            }
            #endregion

            #region UnitPrice
            [PXDBDecimal(BqlField = typeof(KGVendorPrice.unitPrice))]
            [PXUIField(DisplayName = "Unit Price")]
            public virtual Decimal? VendorUnitPrice { get; set; }
            public abstract class vendorUnitPrice : PX.Data.BQL.BqlDecimal.Field<vendorUnitPrice> { }
            #endregion

            #region AreaCode
            //[PXDBInt(IsKey = true)]
            [PXDBString(BqlField = typeof(KGVendorPrice.areaCode))]
            [PXUIField(DisplayName = "Area Code")]
            /*[PXSelector(typeof(Search<INSite.siteID, Where<INSite.active, Equal<True>, And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>>>),
                        typeof(INSite.siteCD),
                        SubstituteKey = typeof(INSite.siteCD))]*/
            [PXSelector(typeof(Search<CSAttributeDetail.valueID,
            Where<CSAttributeDetail.attributeID, Equal<word.aREACODE>>,
            OrderBy<Asc<CSAttributeDetail.sortOrder>>>),
            typeof(CSAttributeDetail.description),
            DescriptionField = typeof(CSAttributeDetail.description))]
            public virtual string AreaCode { get; set; }
            public abstract class areaCode : PX.Data.BQL.BqlString.Field<areaCode> { }
            #endregion

            #region VendorID
            [PXDBInt(BqlField = typeof(KGVendorPrice.vendorID))]
            [PXUIField(DisplayName = "Vendor")]
            [PXSelector(typeof(Search<Vendor.bAccountID>)
                , typeof(Vendor.acctCD), typeof(Vendor.acctName), SubstituteKey = typeof(Vendor.acctCD), DescriptionField = typeof(Vendor.acctName), Filterable = true)]
            //[Vendor()]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

            #region EffectiveDate
            [PXDBDate(BqlField = typeof(KGVendorPrice.effectiveDate))]
            [PXUIField(DisplayName = "Effective Date")]
            public virtual DateTime? EffectiveDate { get; set; }
            public abstract class effectiveDate : PX.Data.BQL.BqlDateTime.Field<effectiveDate> { }
            #endregion

            #region ExpirationDate
            [PXDBDate(BqlField = typeof(KGVendorPrice.expirationDate))]
            [PXUIField(DisplayName = "Expiration Date", Enabled = false)]
            public virtual DateTime? ExpirationDate { get; set; }
            public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
            #endregion

            #region Item
            [PXDBString(BqlField = typeof(KGVendorPrice.item))]
            [PXUIField(DisplayName = "Item")]
            public virtual string Item { get; set; }
            public abstract class item : PX.Data.BQL.BqlString.Field<item> { }
            #endregion

            #region Uom
            [PXDBString(BqlField = typeof(KGVendorPrice.uom))]
            [PXUIField(DisplayName = "Uom")]
            [PXSelector(typeof(Search4<INUnit.fromUnit,
                        Where<INUnit.unitType, Equal<INUnitType.global>>,
                        Aggregate<GroupBy<INUnit.fromUnit>>>))]
            public virtual string VendorUom { get; set; }
            public abstract class vendorUom : PX.Data.BQL.BqlString.Field<vendorUom> { }
            #endregion

            #region Remark
            [PXDBString(BqlField = typeof(KGVendorPrice.remark))]
            [PXUIField(DisplayName = "Remark")]
            public virtual string Remark { get; set; }
            public abstract class remark : IBqlField { }
            #endregion
        }

        #region Definition
        public PMCostBudget pmCost = null;

        public readonly string errorMsg_Qty  = "訂單數量不可大於預算可發包數量。";
        public readonly string errorMsg_Amt  = "訂單金額不可大於預算可發包金額。";
        public readonly string errorMsg_Prc  = "單位成本不能大於供應商價格之價格。";
        public readonly string errMsg_NegAmt = "金額不能為負數";

        public sealed class E : Constant<String>
        {
            public E() : base("E") { }
        }

        public sealed class One : Constant<String>
        {
            public One() : base("1") { }
        }
        public sealed class Type : Constant<String>
        {
            public Type() : base(type) { }
        }
        public  static readonly string type = "式";
        #endregion

        #region Select/Filter
        public PXSelect<CurrencyInfo> currencyinfo;

        [PXCopyPasteHiddenView]
        public PXFilter<PMProjectCostFilter> projectCostFilter;
        //Add By Jerry  20190723 PMCostBudget.uOM, Equal<Type>
        [PXFilterable]
        [PXCopyPasteHiddenView]
        public PXSelect<PMCostBudgetExt> pmCostBudget;
        
        protected virtual IEnumerable PmCostBudget()
        {

            PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMProjectCostFilter.contractID>>>>.Select(Base);
            ContractExt contractExt = null;
            if (project != null)
            {
                contractExt = PXCache<Contract>.GetExtension<ContractExt>(project);
            }
            else {

                yield break;
            }
            POOrder master = Base.Document.Current;
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (master.OrderDate != null)
            {
                today =(DateTime) master.OrderDate;
            }
            PXSelectBase <PMCostBudgetExt> query = new PXSelect<PMCostBudgetExt>(Base);
            PMProjectCostFilter pmProjectCostFilter = projectCostFilter.Current;
            string centerArea = getCenterAreaCodeID();

            foreach (PXResult<PMCostBudgetExt> record in query.Select()){
                PMCostBudgetExt pmCostBudget =(PMCostBudgetExt)record;
                PMBudgetExt pmBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCostBudget);
                if (centerArea !=null && centerArea.Equals(pmCostBudget.AreaCode)){
                    KGVendorPrice kgVendorPrice  = getKGVendorPrice(pmCostBudget.InventoryID, pmCostBudget.VendorID, pmProjectCostFilter.AreaCode, pmCostBudget.Item, pmCostBudget.VendorUom, today);
                    
                    if (kgVendorPrice != null) {
                        continue;
                    }
                }


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



                
                PMCostBudget pmCostBudgetExt = RQRequestEntry_Extension.getPMCostBudget(pmCostBudget.CostCodeID.Value, pmCostBudget.TaskID.Value,
                                                    pmCostBudget.ProjectID.Value, pmCostBudget.InventoryID.Value,
                                                    pmCostBudget.AccountGroupID.Value);
                if (pmCostBudget != null)
                {
                    PMBudgetExt ext = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCostBudgetExt);
                    pmBudgetExt.UsrInventoryDesc = ext.UsrInventoryDesc;
                }

                


                if (pmBudgetExt.UsrAvailQty > 0 )
                {
                    yield return record;
                }
            }

        }

        


        public virtual void PMCostBudgetExt_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            PMCostBudgetExt pmCostBudget = (PMCostBudgetExt)e.Row;
            //KGVendorPrice kgVendorPrice = (KGVendorPrice)e.Row;
            PMBudgetExt pmBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCostBudget);
            if (pmBudgetExt.UsrAvailQty == null || pmBudgetExt.UsrAvailQty ==0)
            {

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
                
            }
            if (pmBudgetExt.UsrAvailAmt == null || pmBudgetExt.UsrAvailAmt==0)
            {
                Decimal? commitAmt = getCommitAmt(Base, pmCostBudget.CostCodeID.Value, pmCostBudget.TaskID.Value,
                                                                            pmCostBudget.ProjectID.Value, pmCostBudget.InventoryID.Value,
                                                                            pmCostBudget.AccountGroupID.Value);
                pmBudgetExt.UsrAvailAmt = getValue(pmCostBudget.RevisedAmount) - commitAmt;
            }

            PMCostBudget pmCostBudgetExt = RQRequestEntry_Extension.getPMCostBudget(pmCostBudget.CostCodeID.Value, pmCostBudget.TaskID.Value,
                                                    pmCostBudget.ProjectID.Value, pmCostBudget.InventoryID.Value,
                                                    pmCostBudget.AccountGroupID.Value);
            if (pmCostBudget != null)
            {
                PMBudgetExt ext = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCostBudgetExt);
                pmBudgetExt.UsrInventoryDesc = ext.UsrInventoryDesc;
            }


        }
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
        #region method
        public Decimal? getCommitQty(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            PXResultset<POLine> lines = getPOLines(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);

            Decimal? sum = 0;
            foreach (POLine line in lines)
            {
                sum = sum + getValue(line.OrderQty);

            }
            return sum;
        }
        public Decimal? getCommitAmt(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            PXResultset<POLine> lines = getPOLines(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);
            Decimal? sum = 0;
            foreach (POLine line in lines)
            {
                sum = sum + getValue(line.CuryExtCost);
            }
            return sum;
        }
        //包含本單子
        public Decimal? getPreviousCommitQty(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            POOrder master = Base.Document.Current;
            PXResultset<POLine> lines = getPOLines(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);
            Decimal? sum = 0;
            foreach (POLine line in lines)
            {
                if (line.OrderNbr != null && !line.OrderNbr.Equals(master.OrderNbr))
                {
                    sum = sum + getValue(line.OrderQty);
                }
            }
            return sum;
        }
        //包含本單子
        public Decimal? getPreviousCommitAmt(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID)
        {
            POOrder master = Base.Document.Current;
            PXResultset<POLine> lines = getPOLines(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, null);
            Decimal? sum = 0;
            foreach (POLine line in lines)
            {
                if (line.OrderNbr != null && !line.OrderNbr.Equals(master.OrderNbr))
                {
                    sum = sum + getValue(line.CuryExtCost);
                }
            }
            return sum;
        }
        public static PXResultset<POLine> getPOLines(PXGraph graph, int costCodeID, int projectTaskID, int projectID, int inventoryID, int baAccountGroupID, String orderNbr)
        {
            PXResultset<POLine> set = null;
            if (orderNbr == null)
            {
                set = PXSelect<POLine, Where<POLine.costCodeID, Equal<Required<POLine.costCodeID>>

                    , And<POLine.taskID, Equal<Required<POLine.taskID>>
                    , And<POLine.projectID, Equal<Required<POLine.projectID>>,
                        And<POLine.inventoryID, Equal<Required<POLine.inventoryID>>,
                        And<POLineContractExt.usrAccountGroupID, Equal<Required<POLineContractExt.usrAccountGroupID>>,
                        And<POLine.orderType,Equal<Required<POLine.orderType>>>>>>>>>.
                    Select(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID,"RS");
            }
            else
            {
                set = PXSelect< POLine, Where<POLine.costCodeID, Equal<Required<POLine.costCodeID>>

                     , And<POLine.taskID, Equal<Required<POLine.taskID>>
                     , And<POLine.projectID, Equal<Required<POLine.projectID>>,
                         And<POLine.inventoryID, Equal<Required<POLine.inventoryID>>,
                         And<POLineContractExt.usrAccountGroupID, Equal<Required<POLineContractExt.usrAccountGroupID>>,
                        And<POLine.orderNbr, NotEqual<Required<POLine.orderNbr>>,
                        And<POLine.orderType, Equal<Required<POLine.orderType>>>>>>>>>>.
                    Select(graph, costCodeID, projectTaskID, projectID, inventoryID, baAccountGroupID, orderNbr, "RS");
            }
            return set;
        }
        #endregion

        #endregion

        #region CacheAttached

        //注意大幅版更需要確認該DAC是否增加屬性
        //20200130 調整專案條件
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>), PX.Objects.PM.Messages.InactiveContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [ProjectBaseExt]
        protected void POOrder_ProjectID_CacheAttached(PXCache sender) { }

        [PXRemoveBaseAttribute(typeof(PXUIVerifyAttribute))]
        protected void _(Events.CacheAttached<POLine.curyExtCost> e) { }
        
        #endregion

        #region Button
        public PXAction<POOrder> addCostItem;
        [PXUIField(DisplayName = "Add Cost Item", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddCostItem(PXAdapter adapter)
        {
            //graph.Document.Cache.RaiseFieldUpdated<SOOrder.salesPersonID>(newOrder, null);
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
                    //projectCostFilter.Cache.Clear();
                    //projectCostFilter.Cache.ClearQueryCache(); 
                }
            }
            catch (Exception e)
            {
                pmCostBudget.Cache.Clear();
                pmCostBudget.Cache.ClearQueryCache();
                PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMProjectCostFilter.contractID>>>>.Select(Base);
                ContractExt contractExt = null;
                if (project != null)
                {
                    contractExt = PXCache<Contract>.GetExtension<ContractExt>(project);
                }
                else
                {
                    throw e;
                }
                POOrder master = Base.Document.Current;
                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                if (master.OrderDate != null)
                {
                    today = (DateTime)master.OrderDate;
                }
                PMProjectCostFilter pmProjectCostFilter = projectCostFilter.Current;
                pmProjectCostFilter.AreaCode = contractExt.UsrSiteArea;
                pmProjectCostFilter.CenterAreaCode = getCenterAreaCodeID();
                pmProjectCostFilter.Date = today;
                projectCostFilter.Update(pmProjectCostFilter);
                throw e;
            }

            return adapter.Get();
        }

        public PXAction<POOrder> addItems;
        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select,
                      MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable AddItems(PXAdapter adapter)
        {
            POOrder master = Base.Document.Current;

            //foreach (PXResult<PMCostBudget, KGVendorPrice> record in pmCostBudget.Cache.Cached)
            foreach (PXResult<PMCostBudgetExt> record in pmCostBudget.Select())
            {
                PMCostBudgetExt pmCost = (PMCostBudgetExt)record;


                if (pmCost.Selected != true)
                {
                    continue;
                }
                /* remove by louis for allow user add the same cost line multi time on 2019/6/3
                if (!checkLine(pmCost))
                {
                    continue;
                } */
                PMBudgetExt pmBudgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCost);
                POLine line = new POLine();
                line.ProjectID = pmCost.ProjectID;
                line.TaskID = pmCost.ProjectTaskID;
                line.CostCodeID = pmCost.CostCodeID;
                line.InventoryID = pmCost.InventoryID;
                /*
                line.CuryUnitCost = 1;
                line.OrderQty = 1;
                Base.Transactions.Update(line);*/

                //PMAccountGroup group = getPMAccountGroup(pmCost.AccountGroupID);
                //line.TranDesc = group.Description;
                //line.TranDesc = pmCost.Description;
                // Obtain custom field value as required by YJ.
                line.TranDesc = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCost).UsrInventoryDesc;

                //2020/05/08 Althea Modify 因為UsrSiteArea轉為string
                //20200514 Alton Mark  因為我們已經不抓warehouse
                //line.SiteID =Int32.Parse( PXCache<Contract>.GetExtension<ContractExt>(getPMProject(pmCost.ProjectID.Value)).UsrSiteArea);

                Decimal revisedQty = pmCost.RevisedQty.Value;
                Decimal committedOpenQty = pmCost.CommittedOpenQty.Value;
                Decimal actualQty = pmCost.ActualQty.Value;
                Decimal committedQty = pmCost.CommittedQty.Value;
                Decimal? newCommittedQty = pmBudgetExt.UsrAvailQty;
                
                if (type.Equals(pmCost.UOM)) {
                    line.OrderQty = revisedQty;
                }
                else {
                    //line.OrderQty = revisedQty - committedOpenQty - actualQty;
                    line.OrderQty = newCommittedQty;
                }

                //line.CuryUnitCost = pmCost.Rate.Value;
                //這段不妥20190907沒考慮到KGVendorPrice.Item
                Decimal? unitPrice=null;
                //Decimal? unitPrice = getUnitPrice(line.InventoryID, master.VendorID, line.ProjectID, master.OrderDate.Value, pmCost);
                if (pmCost.VendorPriceID == null) {
                    unitPrice = pmCost.CuryUnitRate;
                }
                else {
                    unitPrice = pmCost.VendorUnitPrice;
                }

                if (unitPrice == null)
                {
                    line.CuryUnitCost = 0;
                }
                else
                {
                    line.CuryUnitCost = unitPrice;
                }

                line.CuryLineAmt = line.OrderQty * line.CuryUnitCost;

                //line.OrderQty = 1;
                //line.CuryUnitCost = 1;
                //line.CuryLineAmt = 1;
                //line.UnitCost = pmCost.UnitPrice;
                //line.UnitCost = 1;
                //line.CuryRetainageAmt = line.CuryLineAmt;
                //line.RetainagePct = 100;
                initValue(line);

                POLine updateLine = Base.Transactions.Update(line);

                if (unitPrice == null)
                {
                    Base.Transactions.Cache.RaiseExceptionHandling<POLine.curyUnitCost>(updateLine, updateLine.CuryUnitCost,
                                                                                        new PXSetPropertyException("Unit Cost must has value.",
                                                                                                                   PXErrorLevel.Warning));
                }
                Base.Transactions.Cache.SetValue<POLineContractExt.usrOriCuryUnitCost>(updateLine,unitPrice);
                Base.Transactions.Cache.SetValue<POLineContractExt.usrAccountGroupID>(updateLine, pmCost.AccountGroupID);
                // Subcontract detail line UOM be brought in from project cost budget when the record comes from "Add Cost Item".
                
                
                if (pmCost.VendorPriceID == null)
                {
                    Base.Transactions.Cache.SetValue<POLine.uOM>(updateLine, pmCost.UOM);
                    PMBudgetExt budgetExt = PXCache<PMBudget>.GetExtension<PMBudgetExt>(pmCostBudget.Current);
                    Base.Transactions.Cache.SetValue<POLineContractExt.inventDescription>(updateLine, budgetExt.UsrInventoryDesc);
                    Base.Transactions.Cache.SetValue<POLineContractExt.budgetInventDescription>(updateLine, pmCost.Item);
                }
                else {
                    Base.Transactions.Cache.SetValue<POLine.uOM>(updateLine, pmCost.VendorUom);
                    Base.Transactions.Cache.SetValue<POLineContractExt.inventDescription>(updateLine, pmCost.Item);
                    Base.Transactions.Cache.SetValue<POLineContractExt.usrVendorPriceID>(updateLine, pmCost.VendorPriceID);
                    Base.Transactions.Cache.SetValue<POLineContractExt.pricingInventDescription>(updateLine, pmCost.Item);
                    
                    //設定單位轉換20190903
                    doUnitTransform(pmCost);
                }

                if (SubcontractEntry_Extension.type.Equals(updateLine.UOM))
                {
                    Base.Transactions.Cache.SetValue<POLineContractExt.usrAvailQty>(updateLine, getValue(pmCost.RevisedQty) );
                }
                else
                {
                    Decimal? commitQty = getPreviousCommitQty(Base, pmCost.CostCodeID.Value, pmCost.TaskID.Value,
                                                     pmCost.ProjectID.Value, pmCost.InventoryID.Value,
                                                     pmCost.AccountGroupID.Value);
                    Base.Transactions.Cache.SetValue<POLineContractExt.usrAvailQty>(updateLine, getValue(pmCost.RevisedQty) - commitQty);
                }
                Decimal? commitAmt = getPreviousCommitAmt(Base, pmCost.CostCodeID.Value, pmCost.TaskID.Value,
                                                     pmCost.ProjectID.Value, pmCost.InventoryID.Value,
                                                     pmCost.AccountGroupID.Value);
                Base.Transactions.Cache.SetValue<POLineContractExt.usrAvailAmt>(updateLine, getValue(pmCost.RevisedAmount) - commitAmt);

                Base.Transactions.Cache.SetDefaultExt<POLine.curyUnitCost>(updateLine);
                Base.Transactions.Cache.SetValueExt<POLine.curyUnitCost>(updateLine, getValue(unitPrice));

                //Base.Transactions.Cache.SetValue<POLine.orderQty>(Base.Transactions.Current, revisedQty - committedOpenQty - actualQty);
                //Base.Transactions.Cache.SetValue<POLine.curyLineAmt>(Base.Transactions.Current, line.OrderQty * line.CuryUnitCost);
            }
            pmCostBudget.Cache.Clear();
            pmCostBudget.Cache.ClearQueryCache();
            //projectCostFilter.Cache.Clear();
            return adapter.Get();
        }
        #endregion
   
        #region Event Handlers
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            POOrder master = Base.Document.Current;
            //¥Nªí§R°£
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

        protected void POOrder_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            POOrder row = (POOrder)e.Row;
            addCostItem.SetEnabled((row.VendorID != null || row.OrderDate != null) && row.Status == POOrderStatus.Hold);
            if (Base.Transactions != null && Base.Transactions.Select().Count > 0) {
                PXUIFieldAttribute.SetEnabled<POOrder.orderDate>(Base.Document.Cache, row, false);
            }
            else {
                PXUIFieldAttribute.SetEnabled<POOrder.orderDate>(Base.Document.Cache, row, true);
            }
           
        }
       
        protected void POLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            POLine line = (POLine)e.Row;
           
            if (line != null)
            {
                PXUIFieldAttribute.SetEnabled<POLineContractExt.usrAvailQty>(Base.Transactions.Cache, line, false);
                PXUIFieldAttribute.SetEnabled<POLineContractExt.usrAvailAmt>(Base.Transactions.Cache, line, false);
                POLineContractExt poLineExt = PXCache<POLine>.GetExtension<POLineContractExt>(line);
              
                PMCostBudget pmCost = null;
                if (line.ProjectID != null && line.TaskID != null && line.CostCodeID != null &&
                    poLineExt.UsrAccountGroupID != null && line.InventoryID != null)
                {
                    pmCost = RQRequestEntry_Extension.getPMCostBudget(line.CostCodeID.Value, line.TaskID.Value,
                                                            line.ProjectID.Value, line.InventoryID.Value,
                                                            poLineExt.UsrAccountGroupID.Value);
                    if (pmCost!=null) {
                        if (SubcontractEntry_Extension.type.Equals(line.UOM))
                        {
                            poLineExt.UsrAvailQty = getValue(pmCost.RevisedQty.Value);
                        }
                        else
                        {
                            Decimal? commitQty = getPreviousCommitQty(Base, line.CostCodeID.Value, line.TaskID.Value,
                                                                line.ProjectID.Value, line.InventoryID.Value,
                                                                poLineExt.UsrAccountGroupID.Value);
                            poLineExt.UsrAvailQty = getValue(pmCost.RevisedQty.Value) - commitQty;
                        }
                    }

                }
                
                

                if (line.ProjectID != null && line.TaskID != null && line.CostCodeID != null &&
                    poLineExt.UsrAccountGroupID != null && line.InventoryID != null)
                {
                    if (pmCost == null)
                    {
                        pmCost = RQRequestEntry_Extension.getPMCostBudget(line.CostCodeID.Value, line.TaskID.Value,
                                                            line.ProjectID.Value, line.InventoryID.Value,
                                                            poLineExt.UsrAccountGroupID.Value);
                    }
                    Decimal? commitAmt = getPreviousCommitAmt(Base, line.CostCodeID.Value, line.TaskID.Value,
                                                            line.ProjectID.Value, line.InventoryID.Value,
                                                            poLineExt.UsrAccountGroupID.Value);
                    poLineExt.UsrAvailAmt = getValue(pmCost.RevisedAmount) - commitAmt;
                }
            }

        }
        //移除父層事件
        protected void POLine_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
        {
            POLine row = (POLine)e.Row;
            row.UnitCost = row.UnitCost;
        }

        public virtual void POLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            POLine row = (POLine)e.Row;
            row.UnitCost = row.UnitCost;
        }

        public virtual void POLine_OrderQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            POLine row = (POLine)e.Row;
            row.UnitCost = row.UnitCost;
        }

        //此種類型為主動控制是否call父層事件的方法
        protected void POLine_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e, PXFieldDefaulting del)
        {
            POLine row = (POLine)e.Row;
            if (row == null) { return; }
            if (row.CuryUnitCost == null || row.CuryUnitCost == 0)
            {
                del.Invoke(sender, e);
            }
            else {
                e.NewValue = row.CuryUnitCost;
            }
        }
        //為了不被標準訂單影響因此加了這個
        protected virtual void POOrder_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated del)
        {

        }

        //protected virtual void _(Events.FieldVerifying<POLine.curyExtCost> e)
        //{
        //    POLine allline = e.Row as POLine;

        //    POLineContractExt poLineExt = PXCache<POLine>.GetExtension<POLineContractExt>(allline);

        //    decimal? commitAmt = getCommitAmt(Base, allline.CostCodeID.Value, allline.TaskID.Value,
        //                                            allline.ProjectID.Value, allline.InventoryID.Value,
        //                                            poLineExt.UsrAccountGroupID.Value);

        //    PMCostBudget pmCost = RQRequestEntry_Extension.getPMCostBudget(allline.CostCodeID.Value, allline.TaskID.Value,
        //                                                                   allline.ProjectID.Value, allline.InventoryID.Value,
        //                                                                    poLineExt.UsrAccountGroupID.Value);

        //    if ((getValue(pmCost.CuryRevisedAmount.Value) - commitAmt) < 0)
        //    {
        //        e.Cache.RaiseExceptionHandling<POLine.curyExtCost>(allline, allline.CuryExtCost, new PXSetPropertyException(errorMsg_Amt, PXErrorLevel.RowError));
        //    }

        //    if (allline.CuryExtCost < 0)
        //    {
        //        e.Cache.RaiseExceptionHandling<POLine.curyExtCost>(allline, allline.CuryExtCost, new PXSetPropertyException(errMsg_NegAmt));
        //    }
        //}
        #endregion

        #region Functions
        public void doUnitTransform(PMCostBudgetExt pmCost)
        {
            INUnitEntity graph = PXGraph.CreateInstance<INUnitEntity>();
            graph.doUnitTransform(pmCost);
            graph.Persist();
        }

        public void initValue(POLine line)
        {

            line.CompletePOLine = "A";
            line.DocumentDiscountRate = 1;
            line.GroupDiscountRate = 1;
            line.LineType = "SV";
            line.POAccrualType = "O";
            line.RcptQtyAction = "W";
            line.RcptQtyAction = "W";
            line.RcptQtyMax = 100;
            line.RcptQtyThreshold = 100;
            //line.TaxCategoryID = "À³µ|";
            line.ReceiptStatus = "";
            line.VendorID = Base.Document.Current.VendorID;
            //line.UnitCost = 1;
            /*
            line.DiscAmt = 1;
            line.CuryUnbilledAmt = 1;
            line.CuryRetainageAmt = 1;
            line.CuryReceivedCost = 1;
            line.CuryLineAmt = 1;
            line.OpenQty = 1;
            line.RcptQtyMin = 1;
            line.RcptQtyMax = 100000000;
            line.RetainageAmt = 100;
            line.RetainagePct = 100;
            line.UnbilledAmt = 1;
            line.UnbilledQty = 1;
            
            line.UnitVolume = 1;
            line.UnitWeight = 1;*/
        }

        //暫時用不到可加同一種
        public bool checkLine(PMCostBudget pmCost)
        {
            foreach (POLine allline in Base.Transactions.Select())
            {
                //RQRequestLineExt alllineExt = PXCache<RQRequestLine>.GetExtension<RQRequestLineExt>(allline);
                int inventoryID = 0;
                int usrContractID = 0;
                int usrProjectTaskID = 0;
                int usrCostCodeID = 0;
                if (allline.InventoryID != null)
                {
                    inventoryID = allline.InventoryID.Value;
                }
                if (allline.ProjectID != null)
                {
                    usrContractID = allline.ProjectID.Value;
                }
                if (allline.TaskID != null)
                {
                    usrProjectTaskID = allline.TaskID.Value;
                }
                if (allline.CostCodeID != null)
                {
                    usrCostCodeID = allline.CostCodeID.Value;
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

        public class PMCostBudgetValue
        {
            public PMCostBudgetValue()
            {
                orderQty = 0;
                poLineOrdQty = 0;
                poLineExtCost = 0;
                oriOrderQty = 0;
                oriExtCost = 0;
                varianceAmt = 0;
            }
            public decimal? orderQty { get; set; }
            public decimal? poLineOrdQty { get; set; }
            public decimal? poLineExtCost { get; set; }
            public decimal? oriOrderQty { get; set; }
            public decimal? oriExtCost { get; set; }
            public decimal? varianceAmt { get; set; }

        }

        public bool beforeSaveCheck()
        {
            bool check = true;
           
            foreach (POLine allline in Base.Transactions.Select())
            {
                POLineContractExt poLineExt = PXCache<POLine>.GetExtension<POLineContractExt>(allline);

                if (allline.ProjectID != null && allline.TaskID != null && allline.CostCodeID != null &&
                    poLineExt.UsrAccountGroupID != null && allline.InventoryID != null)
                {
                    
                    decimal? commitQty = getCommitQty(Base, allline.CostCodeID.Value, allline.TaskID.Value,
                                                              allline.ProjectID.Value, allline.InventoryID.Value,
                                                              poLineExt.UsrAccountGroupID.Value);
                    decimal? commitAmt = getCommitAmt(Base, allline.CostCodeID.Value, allline.TaskID.Value,
                                                              allline.ProjectID.Value, allline.InventoryID.Value,
                                                              poLineExt.UsrAccountGroupID.Value);

                    PMCostBudget pmCost = RQRequestEntry_Extension.getPMCostBudget(allline.CostCodeID.Value, allline.TaskID.Value,
                                                              allline.ProjectID.Value, allline.InventoryID.Value,
                                                              poLineExt.UsrAccountGroupID.Value);

                    //非一式項檢查
                    //經Cheer建議一式項應該是檢查預算的Uom才是對的
                    if (!SubcontractEntry_Extension.type.Equals(pmCost.UOM))
                    {
                        if ((getValue(pmCost.RevisedQty.Value) - commitQty) < 0)
                        {
                            check = Base.Transactions.Cache.RaiseExceptionHandling<POLine.orderQty>(allline, allline.OrderQty, 
                                                                                                    new PXSetPropertyException(errorMsg_Qty)); 
                        }
                    }

                    if ((getValue(pmCost.CuryRevisedAmount.Value) - commitAmt) < 0)
                    {
                        check = Base.Transactions.Cache.RaiseExceptionHandling<POLine.curyExtCost>(allline, allline.CuryExtCost,
                                                                                                   new PXSetPropertyException(errorMsg_Amt));
                    }

                    if (check == true && allline.CuryExtCost < 0)
                    {
                        check = Base.Transactions.Cache.RaiseExceptionHandling<POLine.curyExtCost>(allline, allline.CuryExtCost,
                                                                                                   new PXSetPropertyException(errMsg_NegAmt));
                    }
                }
                
                if (allline.ProjectID != null &&
                    allline.CuryUnitCost != null &&
                    poLineExt.UsrOriCuryUnitCost != null &&
                    Convert.ToDecimal(allline.CuryUnitCost.Value) > Convert.ToDecimal(poLineExt.UsrOriCuryUnitCost.Value))
                    //allline.CuryUnitCost.Value > poLineExt.UsrOriCuryUnitCost.Value)
                {
                    check = Base.Transactions.Cache.RaiseExceptionHandling<POLine.curyUnitCost>(allline, allline.CuryUnitCost,
                                                                                                new PXSetPropertyException(errorMsg_Prc));
                    break;
                }
            }

            return check;
        }

        public static PXResultset<PMProject> getPMProject(int projectID)
        {
            PXGraph graph = new PXGraph();
            PXResultset<PMProject> set = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.
                    Select(graph, projectID);
            return set;
        }

        public static string getCenterAreaCodeID() {
            PXGraph graph = new PXGraph();
            /*INSite inSite = PXSelect< INSite, Where<INSite.active, Equal<True>, 
                    And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>,
                    And< INSite.siteCD,Equal<Required<INSite.siteCD>>>>>>.
                    Select(graph, "全省");*/

            CSAttributeDetail cSAttributeDetail = PXSelect<CSAttributeDetail, 
                Where<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>,
                And<CSAttributeDetail.description,Equal<Required<CSAttributeDetail.description>>>>>.Select(graph, "AREACODE", "全省");

            return cSAttributeDetail.ValueID;
        }

        //2020/05/08 Althea Modify siteArea改為string
        public static PXResultset<KGVendorPrice> getKGVendorPrice(int? inventoryID, int? vendorID, string siteArea,String item,String Uom, DateTime orderDate)
        {
            
            PXGraph graph = new PXGraph();
            PXResultset<KGVendorPrice> set = PXSelect<KGVendorPrice, Where<KGVendorPrice.inventoryID, Equal<Required<KGVendorPrice.inventoryID>>,
                And<KGVendorPrice.vendorID, Equal<Required<KGVendorPrice.vendorID>>,
                And<KGVendorPrice.areaCode, Equal<Required<KGVendorPrice.areaCode>>,
                And < KGVendorPrice.item, Equal<Required<KGVendorPrice.item>>,
                And < KGVendorPrice.uom, Equal<Required<KGVendorPrice.uom>>,
                And <KGVendorPrice.effectiveDate, LessEqual<Required<KGVendorPrice.effectiveDate>>,
                And<Where<KGVendorPrice.expirationDate, IsNull, Or<KGVendorPrice.expirationDate, GreaterEqual<Required<KGVendorPrice.expirationDate>>>>>>>>>>>>.
                    Select(graph, inventoryID, vendorID, siteArea, item, Uom, orderDate, orderDate);
            return set;
        }

        public static PXResultset<PMAccountGroup> getPMAccountGroup(int? groupID)
        {
            PXGraph graph = new PXGraph();

            PXResultset<PMAccountGroup> set = PXSelect<PMAccountGroup,
                                                       Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>
                                                       .Select(graph, groupID);

            return set;
        }
        #endregion
    }

    #region Filter DAC
    [Serializable]
    public class PMProjectCostFilter : IBqlTable
    {
        #region   
        /// <summary>
        /// Get or sets ProjectID
        /// </summary>   
        protected Int32? _ContractID;
        [PXInt]
        /*
        [PXSelector(typeof(Search2<PMProject.contractID,
                           LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                           LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID, Equal<PMProject.contractID>>>>,
                           Where<PMProject.baseType, Equal<CTPRType.project>,
                             And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>),
                    //typeof(PMProject.contractID),  // Not to be used.
                    typeof(PMProject.contractCD),
                    typeof(PMProject.description),
                    typeof(Customer.acctName),
                    typeof(PMProject.status),
                    typeof(PMProject.approverID),
                    SubstituteKey = typeof(PMProject.contractCD),
                    ValidateValue = false)]*/
        [PXSelector(typeof(Search2<PMProject.contractID,
                           LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                           LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID, Equal<PMProject.contractID>>>>,
                           Where<PMProject.baseType, Equal<CTPRType.project>, 
                               And<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>,
                             And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>>),
                    //typeof(PMProject.contractID),  // Not to be used.
                    typeof(PMProject.contractCD),
                    typeof(PMProject.description),
                    typeof(Customer.acctName),
                    typeof(PMProject.status),
                    typeof(PMProject.approverID),
                    SubstituteKey = typeof(PMProject.contractCD),
                    ValidateValue = false)]
        [PXFormula(typeof(Current<POOrder.projectID>))]
        [PXUIField(DisplayName = "Project ID")]
        public virtual Int32? ContractID
        {
            get
            {
                return this._ContractID;
            }
            set
            {
                this._ContractID = value;
            }
        }
        public abstract class contractID : PX.Data.IBqlField { }
        #endregion

        //2020/05/08 Althea Modify 因為UsrSiteArea改為String Selector 資料來源改為屬性抓ID=AREACODE
        #region areaCode
        [PXString()]
        [PXUIField(DisplayName = "Area Code")]
        /*[PXSelector(typeof(Search<INSite.siteID, Where<INSite.active, Equal<True>, And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>>>),
                    typeof(INSite.siteCD),
                    SubstituteKey = typeof(INSite.siteCD))]*/
        [PXSelector(typeof(Search<CSAttributeDetail.valueID,
            Where<CSAttributeDetail.attributeID, Equal<word.aREACODE>>,
            OrderBy<Asc<CSAttributeDetail.sortOrder>>>),
            typeof(CSAttributeDetail.description),
            DescriptionField = typeof(CSAttributeDetail.description))]
        public virtual string AreaCode { get; set; }
        public abstract class areaCode : PX.Data.BQL.BqlString.Field<areaCode> { }
        #endregion
        #region centerAreaCode
        [PXString()]
        [PXUIField(DisplayName = "Center Area Code")]
        /*[PXSelector(typeof(Search<INSite.siteID, Where<INSite.active, Equal<True>, And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>>>),
                    typeof(INSite.siteCD),
                    SubstituteKey = typeof(INSite.siteCD))]*/
        [PXSelector(typeof(Search<CSAttributeDetail.valueID,
            Where<CSAttributeDetail.attributeID, Equal<word.aREACODE>>,
            OrderBy<Asc<CSAttributeDetail.sortOrder>>>),
            typeof(CSAttributeDetail.description),
            DescriptionField = typeof(CSAttributeDetail.description))]
        public virtual string CenterAreaCode { get; set; }
        public abstract class centerAreaCode : PX.Data.BQL.BqlString.Field<centerAreaCode> { }
        #endregion

        #region date
        [PXDate()]
        [PXUIField(DisplayName = "Date")]
        public virtual DateTime? Date { get; set; }
        public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }
        #endregion
       

    }
    #endregion

    #region DAC Extension
    public class POOrderAPDocExt : PXCacheExtension<POOrderAPDoc>
    {
        #region ValuationPhase
        [PXDBInt(BqlField = typeof(APRegisterExt.usrValuationPhase))]
        [PXUIField(DisplayName = "Valuation Phase")]
        public virtual int? ValuationPhase { get; set; }
        public abstract class valuationPhase : IBqlField { }
        #endregion
    }
    #endregion
}
 