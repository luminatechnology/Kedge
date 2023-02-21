using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.PM;
using PX.Objects.PO;
using Kedge.DAC;

namespace Kedge
{
    public class KGCreatePORetainageProc : PXGraph<KGCreatePORetainageProc>
    {
        #region Features
        public PXCancel<vKGPORetainage> Cancel;
        public PXProcessing<vKGPORetainage, Where<Exists<Select<APRegister, Where<APRegisterExt.usrPOOrderType, Equal<vKGPORetainage.orderType>,
                                                                                  And<APRegisterExt.usrPONbr, Equal<vKGPORetainage.orderNbr>,
                                                                                      And<APRegister.retainageApply, Equal<True>,
                                                                                          And<APRegister.isRetainageDocument, Equal<False>,
                                                                                              And2<Where<APRegisterExt.usrIsRetainageApply, Equal<False>,
                                                                                                         Or<APRegisterExt.usrIsRetainageApply, IsNull>>,
                                                                                                  And2<Where<APRegisterExt.usrIsRetainageDoc, Equal<False>,
                                                                                                             Or<APRegisterExt.usrIsRetainageDoc, IsNull>>,
                                                                                                       And<APRegisterExt.usrRetainageHistType, IsNull,
                                                                                                           And<IsNull<APRegisterExt.usrIsDeductionDoc, False>, Equal<False>>>>>>>>>>>>,
                                            OrderBy<Desc<vKGPORetainage.orderNbr>>> Retainage;
        #endregion

        #region Delegate Data View
        public IEnumerable retainage()
        {
            // Since RetainageRemaining is an unbound calculated field.
            foreach (var row in new PXView(this, true, Retainage.View.BqlSelect).SelectMulti().Where(w => (w as vKGPORetainage).RetainageRemaining != 0m))
            {
                yield return row;
            }
        }
        #endregion

        #region Ctor
        public KGCreatePORetainageProc()
        {
            Retainage.SetProcessCaption("Create Bill As Selected");
            Retainage.SetProcessAllVisible(false);
            Retainage.SetProcessDelegate(list => CreateAPBill(list));
        }
        #endregion

        #region Static Methods
        public static void CreateAPBill(List<vKGPORetainage> lists)
        {
            KGCreatePORetainageProc proc = PXGraph.CreateInstance<KGCreatePORetainageProc>();

            try
            {
                proc.CreateBillByPO(lists);
            }
            catch (System.Exception e)
            {
                PXProcessing.SetError<vKGPORetainage>(e.Message);
                throw;
            }
        }
        #endregion

        #region Methods
        public virtual void CreateBillByPO(List<vKGPORetainage> lists)
        {
            if (lists.Count > 0)
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    APInvoiceEntry graph = CreateInstance<APInvoiceEntry>();

                    var SummaryAmtView = graph.GetExtension<APInvoiceEntry_Extension>().SummaryAmtFilters;

                    for (int i = 0; i < lists.Count; i++)
                    {
                        APInvoice invoice = new APInvoice()
                        {
                            DocType = APDocType.Invoice,
                            ProjectID = lists[i].ProjectID,
                            DocDesc = "¾ú¥v·JÁ`«O¯d´Ú"
                        };

                        invoice = graph.Document.Insert(invoice);

                        graph.Document.SetValueExt<APRegister.vendorID>(invoice, lists[i].VendorID);
                        graph.Document.SetValueExt<APRegisterExt.usrIsRetainageApply>(invoice, true);
                        graph.Document.SetValueExt<APRegisterExt.usrPOOrderType>(invoice, lists[i].OrderType);
                        graph.Document.SetValueExt<APRegisterExt.usrPONbr>(invoice, lists[i].OrderNbr);
                        graph.Document.SetValueExt<APRegisterExt.usrRetainageAmt>(invoice, lists[i].RetainageRemaining);

                        graph.Document.Update(invoice);

                        PMCostBudget costBudget = SelectFrom<PMCostBudget>.Where<PMCostBudget.projectID.IsEqual<@P.AsInt>>.View.SelectSingleBound(graph, null, lists[i].ProjectID);

                        Dictionary<string, POOrderAPDoc> dic = new Dictionary<string, POOrderAPDoc>();

                        APInvoice origInv = new APInvoice();

                        foreach (POOrderAPDoc row in SelectFrom<POOrderAPDoc>.Where<POOrderAPDoc.pOOrderType.IsEqual<@P.AsString>
                                                                                    .And<POOrderAPDoc.pONbr.IsEqual<@P.AsString>>>.View.Select(graph, lists[i].OrderType, lists[i].OrderNbr))
                        {
                            origInv = APInvoice.PK.Find(graph, row.DocType, row.RefNbr);

                            var invExt = origInv.GetExtension<APRegisterExt>();

                            if ((row.DocType == APDocType.Invoice ||
                                (row.DocType == APDocType.DebitAdj && invExt.UsrIsDeductionDoc != true && string.IsNullOrEmpty(origInv.OrigRefNbr) ) ) &&
                                row.Status.IsIn(APDocStatus.Balanced, APDocStatus.Open, APDocStatus.Closed) &&
                                invExt.UsrRetainageHistType == null
                               )
                            {
                                dic.Add(row.RefNbr, row);
                            }

                            if (origInv.IsRetainageDocument != true && origInv.DocType != APDocType.Invoice)
                            {
                                dic.Remove(origInv.OrigRefNbr ?? string.Empty);
                            }
                        }

                        foreach (var key in dic.Keys)
                        {
                            dic.TryGetValue(key, out POOrderAPDoc pOAPDoc);

                            origInv = APInvoice.PK.Find(graph, pOAPDoc.DocType, pOAPDoc.RefNbr);

                            PXDatabase.Update<APRegister>(new PXDataFieldAssign<APRegisterExt.usrRetainageHistType>(PXDbType.NVarChar, RetHistType.Original),
                                                          new PXDataFieldAssign<APRegister.curyRetainageReleased>(PXDbType.Decimal, origInv.CuryRetainageTotal),
                                                          new PXDataFieldAssign<APRegister.retainageReleased>(PXDbType.Decimal, origInv.RetainageTotal),
                                                          new PXDataFieldAssign<APRegister.curyRetainageUnreleasedAmt>(PXDbType.Decimal, 0m),
                                                          new PXDataFieldAssign<APRegister.retainageUnreleasedAmt>(PXDbType.Decimal, 0m),
                                                          new PXDataFieldRestrict<APRegister.docType>(PXDbType.Char, origInv.DocType),
                                                          new PXDataFieldRestrict<APRegister.refNbr>(PXDbType.NVarChar, origInv.RefNbr));

                            APTran tran = new APTran()
                            {
                                TranDesc   = $"Orig. {APDocType.ListAttribute.GetLocalizedLabel<APInvoice.docType>(graph.Document.Cache, invoice, pOAPDoc.DocType)} [{pOAPDoc.RefNbr}] - {origInv.CuryRetainageUnreleasedAmt}",
                                TaskID     = costBudget.TaskID,
                                CostCodeID = costBudget.CostCodeID,
                                AccountID  = graph.location.Current?.VRetainageAcctID,
                                SubID      = graph.location.Current?.VRetainageSubID
                            };

                            tran = graph.Transactions.Insert(tran);

                            tran.POOrderType = pOAPDoc.POOrderType;
                            tran.PONbr       = pOAPDoc.PONbr;

                            graph.Transactions.Update(tran);
                        }

                        SummaryAmtView.SetValueExt<KGBillSummary.totalAmt>(SummaryAmtView.Current, 0m);
                        SummaryAmtView.UpdateCurrent();

                        graph.Document.Cache.SetValue<APRegisterExt.usrRetainageHistType>(invoice, RetHistType.History);
                        graph.Document.Cache.SetValue<APRegister.hold>(invoice, false);
                        graph.Document.Cache.SetValue<APRegister.openDoc>(invoice, false);
                        graph.Document.Cache.SetValue<APRegister.released>(invoice, true);
                        graph.Document.Cache.SetValue<APRegister.status>(invoice, APDocStatus.Closed);
                        
                        graph.Document.Update(invoice);

                        graph.Save.Press();

                        ///<remarks> 
                        /// Since this bill invoice is just a historical data, there is no need to release and generate journal voucher, 
                        /// the following field will be overwritten by standard or customized code when saving so use PXUpdateAttribute.
                        ///</remarks>
                        PXUpdate<Set<APRegisterExt.usrValuationPhase, Required<APRegisterExt.usrValuationPhase>>,
                                 APRegister,
                                 Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                                       And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                                 .Update(graph, -1, invoice.DocType, invoice.RefNbr);

                        PXProcessing.SetInfo<vKGPORetainage>($"{invoice.RefNbr} Has Been Created.");
                    }

                    ts.Complete();
                }
            }
        }
        #endregion
    }
}