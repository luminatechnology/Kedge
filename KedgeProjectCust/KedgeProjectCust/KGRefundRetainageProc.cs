using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.PM;
using PX.Objects.PO;
using Kedge.DAC;
using APRegisterAlias = PX.Objects.AP.Standalone.APRegisterAlias;

namespace KedgeProjectCust
{
    public class KGRefundRetainageProc : PXGraph<KGRefundRetainageProc>
    {
        #region Features
        public PXCancel<APRetainageFilter> Cancel;
        public PXFilter<APRetainageFilter> Filter;
        public PXFilteredProcessing<APRegister, APRetainageFilter, Where<APRegister.released, Equal<True>,
                                                                        And<APRegister.docType, Equal<APDocType.invoice>,
                                                                            And<APRegisterExt.usrPONbr, IsNotNull,
                                                                                And<APRegisterExt.usrIsRetainageApply, Equal<True>,
                                                                                    And2<Where<APRegister.projectID, Equal<Current<APRetainageFilter.projectID>>,
                                                                                                Or<Current<APRetainageFilter.projectID>, IsNull>>,
                                                                                            And2<Where<APRegister.vendorID, Equal<Current<APRetainageFilter.vendorID>>,
                                                                                                    Or<Current<APRetainageFilter.vendorID>, IsNull>>,
                                                                                                And2<Where<APRegisterExt.usrPONbr, Equal<Current<APRetainageFilterExt.usrPONbr>>,
                                                                                                            Or<Current<APRetainageFilterExt.usrPONbr>, IsNull>>,
                                                                                                    And<NotExists<Select<APRegisterAlias, Where<APRegisterAlias.origDocType, Equal<APRegister.docType>,
                                                                                                                                                And<APRegisterAlias.origRefNbr, Equal<APRegister.refNbr>>>>>>>>>>>>>,
                                                                  OrderBy<Desc<APRegister.refNbr>>> RefundRetainage;
        #endregion

        #region Ctor
        public KGRefundRetainageProc()
        {
            RefundRetainage.SetProcessAllVisible(false);
            RefundRetainage.SetProcessDelegate(delegate (List<APRegister> lists)
            {
                CreateFefundRetainage(lists);
            });
        }
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault("X")]
        protected virtual void _(Events.CacheAttached<APRetainageFilter.projectID> e) { }
        #endregion

        #region Delegate Data View
        public virtual IEnumerable refundRetainage()
        {
            PXView view = new PXView(this, true, RefundRetainage.View.BqlSelect);

            foreach (APRegister row in view.SelectMulti())
            {
                if (row.GetExtension<APRegisterExt>()?.UsrRetainageUnreleasedAmt > 0m)
                {
                    var aPRetained = SelectFrom<APTran>.Where<APTranExt.usrOrigRetainageDocType.IsEqual<@P.AsString>
                                                              .And<APTranExt.usrOrigRetainageRefNbr.IsEqual<@P.AsString>>>.View.Select(this, row.DocType, row.RefNbr).TopFirst;
                    /*SelectFrom<APRegister2>.Where<APRegister2.origRetainageDocType.IsEqual<@P.AsString>
                                                               .And<APRegister2.origRetainageRefNbr.IsEqual<@P.AsString>>>.View.Select(this, row.DocType, row.RefNbr).TopFirst;*/

                    if (aPRetained == null || aPRetained.Released == true)
                    {
                        yield return row;
                    }
                }
            }
        }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowSelected<APRetainageFilter> e) 
        {
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrPONbr>(RefundRetainage.Cache, null, true);


            PXUIFieldAttribute.SetDisplayName<Vendor.acctName>(RefundRetainage.Cache, "Vendor Name");
        }
        #endregion

        #region Static Methods
        public static void CreateFefundRetainage(List<APRegister> lists)
        {
            KGRefundRetainageProc graph = CreateInstance<KGRefundRetainageProc>();

            graph.CreateFefundRetainageInvoice(lists);
        }
        #endregion

        #region Methods
        protected virtual void CreateFefundRetainageInvoice(List<APRegister> lists)
        {
            if (lists == null) { return; }

            try
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    var aggregateLists = lists.GroupBy(g => new { g.ProjectID, g.VendorID, g.GetExtension<APRegisterExt>().UsrPONbr }).Select(s => new { s.Key.ProjectID, s.Key.VendorID, s.Key.UsrPONbr }).ToList();

                    APInvoiceEntry invoiceEntry = CreateInstance<APInvoiceEntry>();

                    KGSetUp setup = SelectFrom<KGSetUp>.View.SelectSingleBound(this, null);

                    for (int i = 0; i < aggregateLists.Count; i++)
                    {
                        APInvoice invoice = invoiceEntry.Document.Cache.CreateInstance() as APInvoice;

                        invoice.DocType   = APDocType.Invoice;
                        invoice.VendorID  = aggregateLists[i].VendorID;
                        invoice.ProjectID = aggregateLists[i].ProjectID;

                        invoice = invoiceEntry.Document.Insert(invoice);

                        string orderDesc = null;

                        var findList = lists.FindAll(e => e.ProjectID == aggregateLists[i].ProjectID && e.VendorID == aggregateLists[i].VendorID && e.GetExtension<APRegisterExt>().UsrPONbr == aggregateLists[i].UsrPONbr);

                        PMCostBudget costBudget = SelectFrom<PMCostBudget>.Where<PMCostBudget.projectID.IsEqual<@P.AsInt>.And<PMCostBudget.inventoryID.IsEqual<@P.AsInt>>>
                                                                          .View.SelectSingleBound(this, null, invoice.ProjectID, setup.KGRetainageReturnInventoryID);

                        for (int j = 0; j < findList.Count; j++)
                        {
                            APRegisterExt registerExt = findList[j].GetExtension<APRegisterExt>();

                            APTran tran = new APTran()
                            {
                                InventoryID = setup.KGRetainageReturnInventoryID,
                                POOrderType = registerExt.UsrPOOrderType,
                                PONbr       = registerExt.UsrPONbr
                            };

                            invoiceEntry.Transactions.Cache.SetValueExt<APTranExt.usrValuationType>(tran, ValuationTypeStringList.R);

                            tran = invoiceEntry.Transactions.Insert(tran);

                            tran.CuryLineAmt = registerExt.UsrRetainageUnreleasedAmt;
                            tran.TaskID      = costBudget?.ProjectTaskID;
                            tran.CostCodeID  = costBudget?.CostCodeID;
                            tran.TranDesc    = string.Format(PX.Objects.AP.Messages.RetainageForTransactionDescription, ValuationTypeStringList.Labels[4], lists[j].RefNbr);
                            tran.TaxCategoryID = "免稅";
                            invoiceEntry.Transactions.Cache.SetValueExt<APTranExt.usrOrigRetainageDocType>(tran, findList[j].DocType);
                            invoiceEntry.Transactions.Cache.SetValueExt<APTranExt.usrOrigRetainageRefNbr>(tran, findList[j].RefNbr);
                            invoiceEntry.Transactions.Cache.SetValueExt<APTranExt.usrOrigRetUnreleasedAmt>(tran, tran.CuryLineAmt);

                            invoiceEntry.Transactions.Update(tran);

                            if (orderDesc == null)
                            {
                                POOrder order = POOrder.PK.Find(this, registerExt.UsrPOOrderType, registerExt.UsrPONbr);

                                invoiceEntry.Document.Cache.SetValueExt<APInvoice.docDesc>(invoice, order.OrderDesc);
                                invoiceEntry.Document.Cache.SetValueExt<APRegisterExt.usrIsRetainageDoc>(invoice, true);
                                invoiceEntry.Document.Update(invoice);

                                
                            }
                        }
                        ///<remarks>產生退保留款計價單, 請同時產生一筆KGBillpayment, PricingType='A', PaymentMethod='A', PaymentPeriod =0, PaymentPct=100</remarks>
                        invoiceEntry.GetExtension<APInvoiceEntry_Extension>().KGBillPaym.Cache.Insert(new KGBillPayment()
                        {
                            PaymentPeriod = 0,
                            PaymentPct = 100
                        });

                        // According to Mantis [0012321] - (0025648)
                        if (APInvoiceEntry_Extension.ckeckPrevStatusIsOpen(invoice.VendorID, invoice.DocDate, invoice.ProjectID, 
                                                                           invoiceEntry.Document.Current.RefNbr, invoiceEntry.GetExtension<APInvoiceEntry_Extension>().getPoNbr()) == false)
                        {
                            throw new Exception(APInvoiceEntry_Extension.Message.NotYetError);
                        }

                        invoiceEntry.Save.Press();
                    }

                    ts.Complete();
                }
            }
            catch (System.Exception e)
            {
                PXProcessing.SetError<APRegister>(e);
                throw;
            }
        }
        #endregion
    }

    #region Extension DAC
    [PXCacheName("AP Retainage Filter Extension")]
    public partial class APRetainageFilterExt : PXCacheExtension<APRetainageFilter>
    {
        #region UsrPONbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PO Nbr.")]
        [PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.approved, Equal<True>>>),
                    typeof(POOrder.orderType),
                    typeof(POOrder.orderNbr),
                    typeof(POOrder.orderDesc),
                    typeof(POOrder.vendorID),
                    typeof(POOrder.projectID))]
        public virtual string UsrPONbr { get; set; }
        public abstract class usrPONbr : PX.Data.BQL.BqlString.Field<usrPONbr> { }
        #endregion
    }
    #endregion
}