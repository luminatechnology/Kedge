using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.EP;
using System.Collections;
using PX.Objects.IN;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.TM;
using PX.Objects.PO;
using PX.Objects;
using PX.Objects.RQ;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CT;
using static PX.Objects.RQ.RQRequestProcess;

namespace PX.Objects.RQ
{
    #region Extension DAC
    [Serializable]
    public partial class RQRequestLineOwnedExt : PXCacheExtension<RQRequestLineOwned>
    {
        #region ReqDescription       
        [PXDBString(60, IsUnicode = true, BqlField = typeof(RQRequest.description))]
        [PXUIField(DisplayName = "Req Description", Enabled = false)]
        public virtual string ReqDescription { get; set; }
        public abstract class reqDescription : IBqlField { }
        #endregion

        #region UsrContractID
        public abstract class usrContractID : PX.Data.BQL.BqlString.Field<usrContractID> { }
        #endregion
    }

    [Serializable]
    public partial class RQRequestSelectionExt : PXCacheExtension<RQRequestSelection>
    {
        #region ProjectID
        [PXInt()]
        [PXUIField(DisplayName = "Project")]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), 
                      PM.Messages.CancelledContract, 
                      typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), 
                      PM.Messages.ProjectInvisibleInModule, 
                      typeof(PMProject.contractCD))]
        [PXSelector(typeof(Search<PMProject.contractID, 
                                  Where<PMProject.baseType, Equal<CTPRType.project>,
                                    And<PMProject.status, Equal<ProjectStatus.active>>>>),
                    typeof(PMProject.contractCD), 
                    typeof(PMProject.description), 
                    typeof(PMProject.status), 
                    SubstituteKey = typeof(PMProject.contractCD), 
                    DescriptionField = typeof(PMProject.description))]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : IBqlField { }
        #endregion

        #region FromRefNbr  
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "From Ref.Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelectorAttribute(typeof(Search<RQRequest.orderNbr,
                                           Where<RQRequest.status, Equal<RQRequestStatus.open>>>),
                             typeof(RQRequest.orderNbr),
                             typeof(RQRequest.orderDate),
                             typeof(RQRequest.status),
                             typeof(RQRequest.employeeID),
                             typeof(RQRequest.departmentID),
                             typeof(RQRequest.locationID),
                             Filterable = true,
                             DescriptionField = typeof(RQRequest.description))]       
        public virtual string FromRefNbr { get; set; }
        public abstract class fromRefNbr : IBqlField { }
        #endregion

        #region ToRefNbr  
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "To Ref.Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelectorAttribute(typeof(Search<RQRequest.orderNbr,
                                           Where<RQRequest.status, Equal<RQRequestStatus.open>>>),
                             typeof(RQRequest.orderNbr),
                             typeof(RQRequest.orderDate),
                             typeof(RQRequest.status),
                             typeof(RQRequest.employeeID),
                             typeof(RQRequest.departmentID),
                             typeof(RQRequest.locationID),
                             Filterable = true,
                             DescriptionField = typeof(RQRequest.description))]     
        public virtual string ToRefNbr { get; set; }
        public abstract class toRefNbr : IBqlField { }
        #endregion
    }
    #endregion

    public class RQRequestProcess_Extension : PXGraphExtension<RQRequestProcess>
    {
        #region New Processing
        /*public class RQRequestProcessingExt :
            PXFilteredProcessingJoin<RQRequestLineOwned,
                                     RQRequestSelection,
                                     LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<RQRequestLineOwned.employeeID>>>,
                                     Where2<Where<Current<RQRequestSelection.selectedPriority>, Equal<AllPriority>,
                                                  Or<RQRequestLineOwned.priority, Equal<Current<RQRequestSelection.selectedPriority>>>>,
            // Start filter condistion
            And2<Where<Customer.bAccountID, IsNull,
                        Or<Match<Customer, Current<AccessInfo.userName>>>>,
                 And2<Where<RQRequestLineOwnedExt.usrContractID, Equal<Current<RQRequestSelectionExt.projectID>>>,
                           //And2<Where<RQRequestLineOwnedExt.reqDescription, Like<RQRequestSelection.description>>,
                           And2<Where<RQRequestLineOwned.orderNbr, GreaterEqual<Current<RQRequestSelectionExt.fromRefNbr>>>,
                                And<RQRequestLineOwned.orderNbr, LessEqual<Current<RQRequestSelectionExt.toRefNbr>>>>>>>,
                                      // End filter condition                      
                                      OrderBy<Desc<RQRequestLineOwned.priority,
                                                   Asc<RQRequestLineOwned.orderNbr,
                                                       Asc<RQRequestLineOwned.lineNbr>>>>>
        {

            public RQRequestProcessingExt(PXGraph graph) : base(graph)
            {
                base._OuterView.WhereAndCurrent<RQRequestSelection>(typeof(RQRequestSelection.workGroupID).Name, typeof(RQRequestSelection.ownerID).Name);
            }

            public RQRequestProcessingExt(PXGraph graph, Delegate handler) : base(graph, handler)
            {
                base._OuterView.WhereAndCurrent<RQRequestSelection>(typeof(RQRequestSelection.workGroupID).Name, typeof(RQRequestSelection.ownerID).Name);
            }

            [PXUIField(DisplayName = "Process", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
            [PXProcessButton]
            protected override IEnumerable Process(PXAdapter adapter)
            {
                return !CheckCustomer(adapter, true) ? adapter.Get() : base.Process(adapter);
            }

            [PXUIField(DisplayName = "Process All", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
            [PXProcessButton]
            protected override IEnumerable ProcessAll(PXAdapter adapter)
            {
                return !CheckCustomer(adapter, false) ? adapter.Get() : base.ProcessAll(adapter);
            }

            private bool CheckCustomer(PXAdapter adapter, bool onlySelected)
            {
                try
                {
                    if (System.Web.HttpContext.Current == null)
                        return true;
                }
                catch (Exception)
                {
                    return true;
                }

                RQRequestLineOwned prev = null;
                bool isCustomerDiffer = false;
                var list = onlySelected ? GetSelectedItems(this.View.Cache, this.View.Cache.Cached) : this.View.SelectMulti().RowCast<RQRequestLineOwned>();

                foreach (RQRequestLineOwned line in list)
                {
                    if (!(prev == null ||
                            (prev.CustomerRequest == true &&
                                prev.EmployeeID == line.EmployeeID &&
                                prev.LocationID == line.LocationID) ||
                            (prev.CustomerRequest == line.CustomerRequest && line.CustomerRequest != true)))
                    {
                        isCustomerDiffer = true;
                        break;
                    }
                    prev = line;
                }
                return !isCustomerDiffer ||
                    this.Ask(CR.Messages.Confirmation, Messages.RequisitionCreationConfirmation, MessageButtons.YesNo) == WebDialogResult.Yes;
            }
        }*/
        #endregion

        #region Delegate Dataview
        protected virtual IEnumerable records()
        {
            RQRequestSelection    filter    = Base.Filter.Current;
            RQRequestSelectionExt filterExt = PXCache<RQRequestSelection>.GetExtension<RQRequestSelectionExt>(filter);

            BqlCommand cmd = Base.Records.View.BqlSelect;

            if (filter != null && filterExt.ProjectID != null)
            {
                cmd = cmd.WhereAnd<Where<RQRequestLineOwnedExt.usrContractID, Equal<Current<RQRequestSelectionExt.projectID>>>>();
            }
            if (filter != null && filter.Description != null)
            {
                cmd = cmd.WhereAnd<Where<RQRequestLineOwnedExt.reqDescription, Contains<Current<RQRequestSelection.description>>>>();
            }
            if (filter != null && filterExt.FromRefNbr != null)
            {
                cmd = cmd.WhereAnd<Where<RQRequestLineOwned.orderNbr, GreaterEqual<Current<RQRequestSelectionExt.fromRefNbr>>>>();
            }
            if (filter != null && filterExt.ToRefNbr != null)
            {
                cmd = cmd.WhereAnd<Where<RQRequestLineOwned.orderNbr, LessEqual<Current<RQRequestSelectionExt.toRefNbr>>>>();
            }
            if (filter != null && filter.EmployeeID != null)
            {
                cmd = cmd.WhereAnd<Where<RQRequestLineOwned.employeeID, Equal<Current<RQRequestSelection.employeeID>>>>();
            }
            if (filter != null && filter.ReqClassID != null)
            {
                cmd = cmd.WhereAnd<Where<RQRequestLineOwned.reqClassID, Equal<Current<RQRequestSelection.reqClassID>>>>();
            }

            int totalRows = 0;
            int startRow = PXView.StartRow;
            PXView view = new PXView(Base, false, cmd);
            view.Clear();

            List<object> result = view.Select(PXView.Currents, PXView.Parameters,
                                              PXView.Searches, PXView.SortColumns,
                                              PXView.Descendings, PXView.Filters,
                                              ref startRow, PXView.MaximumRows, ref totalRows);

            return result;
            //foreach (PXResult<RQRequestLineOwned, Customer> item in view.Select(PXView.Currents, PXView.Parameters,
            //                                                          PXView.Searches, PXView.SortColumns,
            //                                                          PXView.Descendings, PXView.Filters,
            //                                                          ref startRow, PXView.MaximumRows, ref totalRows))
            //{
            //    yield return item;
            //}
        }
        #endregion

        #region Event Handlers
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref. Nbr.")]
        [PXSelector(typeof(Search<RQRequest.orderNbr>))]
        protected void _(Events.CacheAttached<RQRequestLineOwned.orderNbr> e) { }

        protected void RQRequestSelection_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if (InvokeBaseHandler != null)
                InvokeBaseHandler(cache, e);

            RQRequestSelection filter = Base.Filter.Current;
            Base.Records.SetProcessDelegate(list => GenRequisition(filter, list));
        }
        #endregion

        #region Function
        private void GenRequisition(RQRequestSelection filter, List<RQRequestLineOwned> lines)
        {
            RQRequisitionEntry graph = PXGraph.CreateInstance<RQRequisitionEntry>();

            RQRequisition requisition = (RQRequisition)graph.Document.Cache.CreateInstance();
            graph.Document.Insert(requisition);
            requisition.ShipDestType = null;

            // Start Add some variables
            int rowCount = 1;
            string reqNbr = "";
            string combinDesc = "";
            // End
            bool isCustomerSet = true;
            bool isVendorSet = true;
            bool isShipToSet = true;
            int? shipContactID = null;
            int? shipAddressID = null;
            var vendors = new HashSet<VendorRef>();

            foreach (RQRequestLine line in lines)
            {
                PXResult<RQRequest, RQRequestClass> e =
                    (PXResult<RQRequest, RQRequestClass>)
                    PXSelectJoin<RQRequest,
                        InnerJoin<RQRequestClass,
                            On<RQRequestClass.reqClassID, Equal<RQRequest.reqClassID>>>,
                        Where<RQRequest.orderNbr, Equal<Required<RQRequest.orderNbr>>>>
                        .Select(graph, line.OrderNbr);

                RQRequest req = e;
                RQRequestClass reqclass = e;

                requisition = PXCache<RQRequisition>.CreateCopy(graph.Document.Current);

                if (reqclass.CustomerRequest == true && isCustomerSet)
                {
                    if (requisition.CustomerID == null)
                    {
                        requisition.CustomerID = req.EmployeeID;
                        requisition.CustomerLocationID = req.LocationID;
                    }
                    else if (requisition.CustomerID != req.EmployeeID || requisition.CustomerLocationID != req.LocationID)
                    {
                        isCustomerSet = false;
                    }
                }
                else
                    isCustomerSet = false;

                if (isShipToSet)
                {
                    if (shipContactID == null && shipAddressID == null)
                    {
                        requisition.ShipDestType = req.ShipDestType;
                        requisition.ShipToBAccountID = req.ShipToBAccountID;
                        requisition.ShipToLocationID = req.ShipToLocationID;
                        shipContactID = req.ShipContactID;
                        shipAddressID = req.ShipAddressID;
                    }
                    else if (requisition.ShipDestType != req.ShipDestType ||
                            requisition.ShipToBAccountID != req.ShipToBAccountID ||
                            requisition.ShipToLocationID != req.ShipToLocationID)
                    {
                        isShipToSet = false;
                    }
                }

                if (line.VendorID != null && line.VendorLocationID != null)
                {
                    VendorRef vendor = new VendorRef()
                    {
                        VendorID = line.VendorID.Value,
                        LocationID = line.VendorLocationID.Value
                    };

                    vendors.Add(vendor);

                    if (isVendorSet)
                    {
                        if (requisition.VendorID == null)
                        {
                            requisition.VendorID = line.VendorID;
                            requisition.VendorLocationID = line.VendorLocationID;
                        }
                        else if (requisition.VendorID != line.VendorID ||
                                 requisition.VendorLocationID != line.VendorLocationID)
                            isVendorSet = false;
                    }
                }
                else
                    isVendorSet = false;

                if (!isCustomerSet)
                {
                    requisition.CustomerID = null;
                    requisition.CustomerLocationID = null;
                }

                if (!isVendorSet)
                {
                    requisition.VendorID = null;
                    requisition.VendorLocationID = null;
                    requisition.RemitAddressID = null;
                    requisition.RemitContactID = null;
                }
                else if (requisition.VendorID == req.VendorID && requisition.VendorLocationID == req.VendorLocationID)
                {
                    requisition.RemitAddressID = req.RemitAddressID;
                    requisition.RemitContactID = req.RemitContactID;
                }

                if (!isShipToSet)
                {
                    requisition.ShipDestType = PX.Objects.PO.POShippingDestination.CompanyLocation;
                    graph.Document.Cache.SetDefaultExt<RQRequisition.shipToBAccountID>(requisition);
                }

                // Start『分包規劃』上方的描述帶到『請購單』上方的描述，如果 多筆勾選成一筆請購，那描述就帶全部的描述，中間用/隔開。
                if (line.OrderNbr != reqNbr)
                {
                    combinDesc += Base.Records.Cache.GetExtension<RQRequestLineOwnedExt>(line).ReqDescription + "/";
                    reqNbr = line.OrderNbr;
                }

                if (lines.Count == rowCount)
                {
                    combinDesc = combinDesc.Substring(0, combinDesc.Length - 1);
                }

                requisition.Description = combinDesc;
                graph.CurrentDocument.Cache.GetExtension<RQRequisitionExt>(requisition).UsrProjectID = PXCache<RQRequest>.GetExtension<RQRequestExt>(req).UsrProjectID;
                rowCount++;
                // End 

                graph.Document.Update(requisition);

                if (line.OpenQty > 0)
                {
                    if (!graph.Lines.Cache.IsDirty && req.CuryID != requisition.CuryID)
                    {
                        requisition = PXCache<RQRequisition>.CreateCopy(graph.Document.Current);
                        requisition.CuryID = req.CuryID;
                        graph.Document.Update(requisition);
                    }

                    graph.InsertRequestLine(line, line.OpenQty.GetValueOrDefault(), filter.AddExists == true);
                }
            }

            if (isShipToSet)
            {
                foreach (PXResult<POAddress, POContact> res in
                            PXSelectJoin<POAddress,
                                CrossJoin<POContact>,
                                Where<POAddress.addressID, Equal<Required<RQRequisition.shipAddressID>>,
                                    And<POContact.contactID, Equal<Required<RQRequisition.shipContactID>>>>>
                                .Select(graph, shipAddressID, shipContactID))
                {
                    AddressAttribute.CopyRecord<RQRequisition.shipAddressID>(graph.Document.Cache, graph.Document.Current, (POAddress)res, true);
                    AddressAttribute.CopyRecord<RQRequisition.shipContactID>(graph.Document.Cache, graph.Document.Current, (POContact)res, true);
                }
            }

            if (requisition.VendorID == null && vendors.Count > 0)
            {
                foreach (var vendor in vendors)
                {
                    RQBiddingVendor bid = PXCache<RQBiddingVendor>.CreateCopy(graph.Vendors.Insert());
                    bid.VendorID = vendor.VendorID;
                    bid.VendorLocationID = vendor.LocationID;
                    graph.Vendors.Update(bid);
                }
            }

            if (graph.Lines.Cache.IsDirty)
            {
                graph.Save.Press();
                throw new PXRedirectRequiredException(graph, string.Format(Messages.RequisitionCreated, graph.Document.Current.ReqNbr));
            }

            for (int i = 0; i < lines.Count; i++)
            {
                PXProcessing<RQRequestLine>.SetInfo(i, PXMessages.LocalizeFormatNoPrefixNLA(Messages.RequisitionCreated, graph.Document.Current.ReqNbr));
            }
        }
        #endregion
    }
}