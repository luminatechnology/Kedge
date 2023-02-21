using System;
using System.Collections;
using Kedge.DAC;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.CR;
using KG.Util;


namespace Kedge
{
    /**
     * ===2021/07/14 :0012148 === Althea
     * Add INV沖銷產生APPayment 過帳之後 要產生對應的GL
     * 
     * ===2021/07/21 :0012166 === Althea
     * Move Action 改開票&改電匯 To NMTrConfirmProcess
     * Add View Condition:
     * KGBillPayment.UsrIsTrConfirm = True
     * 
     * ===2021/08/02 :0012176 === Althea
     * Add : PaymentMethod F(暫付沖銷) the same with C(現金)
     **/
    public class KGWriteOffProcess : PXGraph<KGWriteOffProcess>
    {
        #region Select View
        public PXFilter<KGWriteOffFilter> Filters;
        public PXSelectJoin<KGBillPayment,
            InnerJoin<APRegister,On<APRegister.refNbr,Equal<KGBillPayment.refNbr>>,
            InnerJoin<APInvoice,On<APInvoice.refNbr,Equal<KGBillPayment.refNbr>>,
            InnerJoin<NMBankAccount,On<NMBankAccount.bankAccountID,Equal<KGBillPayment.bankAccountID>>>>>,
                Where<APRegisterFinExt.usrIsConfirm, Equal<True>,
                    And<APRegister.docType, In3<APDocType.invoice,APDocType.prepayment>,
                    And<APRegister.status,Equal<APDocStatus.open>,
                    //2021/07/21 Add Mantis: 0012166 
                    //And<KGBillPaymentExt.usrIsTrConfirm,Equal<True>,
                    And2<Where<KGBillPaymentExt.usrIsWriteOff,Equal<False>,Or<KGBillPaymentExt.usrIsWriteOff,IsNull>>,

                    And2<Where<KGBillPaymentExt.usrIsTrConfirm, Equal<True>,
                            And<Where<KGBillPayment.paymentMethod, In3<PaymentMethod.cash, PaymentMethod.auth>,
                                Or<Where<KGBillPayment.paymentMethod, Equal<PaymentMethod.giftCertificate>,
                                    And<APRegister.projectID, Equal<Zero>>>>>>>,
            //Header Filter
                    And2<Where<KGBillPayment.refNbr,Equal<Current2<KGWriteOffFilter.refNbr>>,
                                Or<Current2<KGWriteOffFilter.refNbr>,IsNull>>,
                    And2<Where<APRegisterFinExt.usrAccConfirmNbr,Equal<Current2<KGWriteOffFilter.accConfrimNbr>>,
                                Or<Current2<KGWriteOffFilter.accConfrimNbr>,IsNull>>,
                    And2<Where<APRegister.projectID,Equal<Current2<KGWriteOffFilter.contractID>>,
                                Or<Current2<KGWriteOffFilter.contractID>,IsNull>>,
                    And2<Where<KGBillPayment.paymentDate,Equal<Current2<KGWriteOffFilter.paymentDate>>,
                                Or<Current2<KGWriteOffFilter.paymentDate>,IsNull>>,
                    And2<Where<APInvoice.dueDate,Equal<Current2<KGWriteOffFilter.dueDate>>,
                                Or<Current2<KGWriteOffFilter.dueDate>,IsNull>>,
                    And2<Where<KGBillPayment.paymentMethod, Equal<Current2<KGWriteOffFilter.paymentMethod>>,
                                Or<Current2<KGWriteOffFilter.paymentMethod>, IsNull>>,
                   And2<Where<KGBillPaymentExt.usrTrConfirmBy, Equal<Current2<KGWriteOffFilter.trConfirmBy>>,
                                Or<Current2<KGWriteOffFilter.trConfirmBy>, IsNull>>,
                   And2<Where<KGBillPaymentExt.usrTrConfirmDate, Equal<Current2<KGWriteOffFilter.trConfirmDate>>,
                                Or<Current2<KGWriteOffFilter.trConfirmDate>, IsNull>>,
                   And2<Where<KGBillPaymentExt.usrTrPaymentType, Equal<Current2<KGWriteOffFilter.trPaymentType>>,
                                Or<Current2<KGWriteOffFilter.trPaymentType>, IsNull>>,
                   And2<Where<KGBillPaymentExt.isPaymentHold, IsNull,
                                Or<KGBillPaymentExt.isPaymentHold, Equal<False>>>,
                   And<Where<KGBillPaymentExt.usrTrConfirmID, Equal<Current2<KGWriteOffFilter.trConfirmID>>,
                                Or<Current2<KGWriteOffFilter.trConfirmID>, IsNull>>>>>>>>>>>>>>>>>>> INVViews;

        public PXSelectJoin<APRegister,
            InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<APRegister.refNbr>>>,
                Where<APRegister.docType, Equal<APDocType.debitAdj>,
                    And<APRegister.status,Equal<APDocStatus.open>,
                    And2<Where<APRegisterFinExt.usrIsWriteOff,Equal<False>,
                        Or<APRegisterFinExt.usrIsWriteOff,IsNull>>,
                    And2<
                        Where<APRegisterExt.usrIsDeductionDoc, Equal<True>,
                            Or<Where<APRegister.origDocType, Equal<APDocType.invoice>,
                                And<APRegister.origRefNbr, IsNotNull>>>>,
                    //Header Filter 
                    And2<Where<APRegister.refNbr, Equal<Current2<KGWriteOffFilter.refNbr>>,
                            Or<Current2<KGWriteOffFilter.refNbr>, IsNull>>,
                    And2<Where<APRegisterFinExt.usrAccConfirmNbr, Equal<Current2<KGWriteOffFilter.accConfrimNbr>>,
                            Or<Current2<KGWriteOffFilter.accConfrimNbr>, IsNull>>,
                    And2<Where<APInvoice.projectID, Equal<Current2<KGWriteOffFilter.contractID>>,
                            Or<Current2<KGWriteOffFilter.contractID>, IsNull>>,
                      And<Where<APInvoice.dueDate, Equal<Current2<KGWriteOffFilter.dueDate>>,
                            Or<Current2<KGWriteOffFilter.dueDate>, IsNull>>>>>>>>>>> ADRViews;
        #endregion

        #region ViewTable

        #region INV View
        [Serializable]
        [PXHidden]
        //20220703 新增isPaymentHold欄位, 暫不付款的資料不可以沖銷
        [PXProjection(typeof(Select<KGBillPayment>
                ), Persistent = false)]
        public class INVView : KGBillPayment
        {
            #region Selected
            [PXDBBool()]
            [PXUIField(DisplayName = "Selected")]
            
            public virtual bool? Selected { get; set; }
            public abstract class selected : IBqlField { }
            #endregion
        }
        #endregion

        #endregion

        #region Action

        #region 沖銷付款
        public PXAction<KGWriteOffFilter> WriteoffPaymentAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "沖銷付款")]
        protected IEnumerable writeoffPaymentAction(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, WriteoffPayment);
            return adapter.Get();
        }
        #endregion

        //2021/07/21 Mantis: 0012166 Move to NMTrConfirmProcess
        #region 改開票
        public PXAction<KGWriteOffFilter> ChangeCheckAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "改開票",Visible = false)]
        protected IEnumerable changeCheckAction(PXAdapter adapter)
        {
            ChangePaymentMethod(1);
            return adapter.Get();
        }
        #endregion

        //2021/07/21 Mantis: 0012166 Move to NMTrConfirmProcess

        #region 改電匯
        public PXAction<KGWriteOffFilter> ChangeTTAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "改電匯", Visible = false)]
        protected IEnumerable changeTTAction(PXAdapter adapter)
        {
            ChangePaymentMethod(2);
            return adapter.Get();
        }
        #endregion
        #endregion

        #region Event
        protected void _(Events.RowSelected<KGWriteOffFilter> e)
        {
            KGWriteOffFilter filter = e.Row;
            if (filter == null) return;
            PXUIFieldAttribute.SetReadOnly(INVViews.Cache, null, true); 
            PXUIFieldAttribute.SetReadOnly(ADRViews.Cache, null, true);
            if(filter.Target == KGWriteOffTarget.INV)
                PXUIFieldAttribute.SetReadOnly<INVView.selected>(INVViews.Cache, null, false);
            else if(filter.Target == KGWriteOffTarget.ADR)
                PXUIFieldAttribute.SetReadOnly<APRegister.selected>(ADRViews.Cache, null, false);
        }
        #endregion

        #region Method
        public void WriteoffPayment()
        {
            KGWriteOffFilter filter = Filters.Current;
            if (filter.Target == null) throw new Exception("請先選擇目標!");
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                if (filter.Target == KGWriteOffTarget.INV)
                {
                    PXResultset<KGBillPayment> invset = GetSelectINVView();
                    foreach (KGBillPayment kGBillPayment in invset)
                    {
                        APInvoice inv = GetAPInvoice(kGBillPayment.RefNbr);
                        APPayment payment = null;
                        APRegister register = PXSelect<APRegister, Where<APRegister.refNbr, Equal<Required<APInvoice.refNbr>>>>.Select(this, inv.RefNbr);
                        APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
                        string adjdDocType = "";
                        string adjdRefNbr = "";

                        if (kGBillPayment.PaymentMethod == PaymentMethod.C ||
                           (kGBillPayment.PaymentMethod == PaymentMethod.D && inv.ProjectID == 0) ||
                            kGBillPayment.PaymentMethod == PaymentMethod.E)
                        {
                            APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();

                            APPayment apPayment = null;
                            apPayment = (APPayment)entry.Document.Cache.CreateInstance();
                            apPayment.CuryID = inv.CuryID;
                            apPayment.CuryInfoID = inv.CuryInfoID;
                            apPayment.BranchID = inv.BranchID;
                            apPayment.Hold = true;
                            apPayment.DocType = APDocType.Check;
                            apPayment = entry.Document.Insert(apPayment);
                            entry.Document.SetValueExt<APPayment.vendorID>(apPayment, inv.VendorID);
                            entry.Document.SetValueExt<APPayment.vendorLocationID>(apPayment, inv.VendorLocationID);

                            string PaymentMethodID = "";
                            switch (kGBillPayment?.PaymentMethod)
                            {
                                case Kedge.DAC.PaymentMethod.C:
                                case Kedge.DAC.PaymentMethod.E: //2021/06/29 Add E the same with C
                                    PaymentMethodID = "CASH";
                                    break;
                                case Kedge.DAC.PaymentMethod.D:
                                    PaymentMethodID = "COUPON";
                                    break;
                            }
                            entry.Document.SetValueExt<APPayment.paymentMethodID>(apPayment, PaymentMethodID);
                            NMBankAccount bankAccount = GetBankAccount(kGBillPayment?.BankAccountID);
                            entry.Document.SetValueExt<APPayment.cashAccountID>(apPayment, bankAccount?.CashAccountID);
                            entry.Document.SetValueExt<APPayment.docDesc>(apPayment, inv.DocDesc);
                            entry.Document.SetValueExt<APPayment.adjDate>(apPayment, inv.DocDate);
                            entry.Document.SetValueExt<APPayment.curyOrigDocAmt>(apPayment, kGBillPayment?.PaymentAmount);
                            entry.Document.SetValueExt<APPayment.extRefNbr>(apPayment, inv.RefNbr);

                            entry.Save.Press();

                            payment = entry.Document.Current;
                            adjdDocType = inv.DocType;
                            adjdRefNbr = inv.RefNbr;
                            APAdjust adj = new APAdjust();
                            adj.AdjdDocType = adjdDocType;
                            adj.AdjdRefNbr = adjdRefNbr;
                            adj.AdjdLineNbr = 0;
                            //set origamt to zero to apply "full" amounts to invoices.
                            entry.Document.Cache.SetValueExt<APPayment.curyOrigDocAmt>(entry.Document.Current, 0m);
                            adj = PXCache<APAdjust>.CreateCopy(entry.Adjustments.Insert(adj));

                            adj.CuryAdjgAmt = kGBillPayment.PaymentAmount;
                            adj = PXCache<APAdjust>.CreateCopy((APAdjust)entry.Adjustments.Cache.Update(adj));
                            decimal? CuryApplAmt = kGBillPayment.PaymentAmount;

                            APPayment copy = PXCache<APPayment>.CreateCopy(entry.Document.Current);
                            copy.CuryOrigDocAmt = CuryApplAmt;
                            entry.Document.SetValueExt<APPayment.hold>(copy, false);

                            entry.Document.Cache.Update(copy);
                            entry.Save.Press();

                            entry.release.Press();

                            //2021/07/14 Add Mantis: 0012148
                            //產生對應的GL
                            if (kGBillPayment.PaymentMethod == PaymentMethod.C ||
                                kGBillPayment.PaymentMethod == PaymentMethod.D ||
                                kGBillPayment.PaymentMethod == PaymentMethod.E) {
                                KGVoucherUtil.CreateGLFromKGBillPayment(kGBillPayment, GLStageCode.NMPWriteoffB1);
                            }
                            
                        }
                        PXUpdate<Set<KGBillPaymentExt.usrIsWriteOff, True,
                            Set<KGBillPaymentExt.usrIsCheckIssued, True>>,
                            KGBillPayment,
                            Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                            .Update(this, kGBillPayment.BillPaymentID);


                    }
                }
                else if(filter.Target== KGWriteOffTarget.ADR)
                {
                    //PXResultset<APRegister> invset = GetSelectAPInvoice();
                    foreach (APRegister inv in ADRViews.Select())
                    {
                        if (inv.Selected == true)
                        {
                            APPayment payment = null;
                            //APRegister register = GetAPRegister(inv.RefNbr);
                            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(inv);
                            APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
                            string adjdDocType = "";
                            string adjdRefNbr = "";

                            if (inv.DocType == APDocType.DebitAdj && registerExt.UsrIsDeductionDoc == true)
                            {
                                try
                                {
                                    payment = entry.Document.Current = entry.Document.Search<APPayment.refNbr>(inv.RefNbr, inv.DocType);
                                    adjdDocType = registerExt.UsrOriDeductionDocType;
                                    adjdRefNbr = registerExt.UsrOriDeductionRefNbr;
                                    APAdjust adj1 = new APAdjust();
                                    adj1.AdjdDocType = adjdDocType;
                                    adj1.AdjdRefNbr = adjdRefNbr;
                                    adj1.AdjdLineNbr = 0;
                                    //set origamt to zero to apply "full" amounts to invoices.
                                    entry.Document.Cache.SetValueExt<APPayment.curyOrigDocAmt>(entry.Document.Current, 0m);
                                    adj1 = PXCache<APAdjust>.CreateCopy(entry.Adjustments.Insert(adj1));

                                    adj1.CuryAdjgAmt = inv.CuryOrigDocAmt;
                                    adj1 = PXCache<APAdjust>.CreateCopy((APAdjust)entry.Adjustments.Cache.Update(adj1));
                                    decimal? CuryApplAmt = entry.Document.Current.CuryApplAmt;

                                    APPayment copy1 = PXCache<APPayment>.CreateCopy(entry.Document.Current);
                                    copy1.CuryOrigDocAmt = CuryApplAmt;
                                    entry.Document.SetValueExt<APPayment.hold>(copy1, false);
                                    entry.Document.Cache.Update(copy1);
                                    entry.Save.Press();
                                    entry.release.Press();

                                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, True>,
                                           APRegister,
                                           Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                                           .Update(this, inv.RefNbr);
                                }
                                catch (Exception e)
                                {
                                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, False>,
                                            APRegister,
                                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                                            .Update(this, inv.RefNbr);
                                    throw new Exception(e.ToString());
                                }
                            }
                            else if (inv.DocType == APDocType.DebitAdj && inv.OrigDocType != null && inv.OrigRefNbr != null)
                            {
                                try
                                {
                                    payment = entry.Document.Current = entry.Document.Search<APPayment.refNbr>(inv.RefNbr, inv.DocType);
                                    adjdDocType = inv.OrigDocType;
                                    adjdRefNbr = inv.OrigRefNbr;
                                    APAdjust adj2 = new APAdjust();
                                    adj2.AdjdDocType = adjdDocType;
                                    adj2.AdjdRefNbr = adjdRefNbr;
                                    adj2.AdjdLineNbr = 0;
                                    //set origamt to zero to apply "full" amounts to invoices.
                                    entry.Document.Cache.SetValueExt<APPayment.curyOrigDocAmt>(entry.Document.Current, 0m);
                                    adj2 = PXCache<APAdjust>.CreateCopy(entry.Adjustments.Insert(adj2));

                                    adj2.CuryAdjgAmt = inv.CuryOrigDocAmt;
                                    adj2 = PXCache<APAdjust>.CreateCopy((APAdjust)entry.Adjustments.Cache.Update(adj2));
                                    decimal? CuryApplAmt = entry.Document.Current.CuryApplAmt;

                                    APPayment copy2 = PXCache<APPayment>.CreateCopy(entry.Document.Current);
                                    copy2.CuryOrigDocAmt = CuryApplAmt;
                                    entry.Document.SetValueExt<APPayment.hold>(copy2, false);
                                    entry.Document.Cache.Update(copy2);
                                    entry.Save.Press();
                                    entry.release.Press();
                                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, True>,
                                            APRegister,
                                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                                            .Update(this, inv.RefNbr);
                                }
                                catch (Exception e)
                                {
                                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, False>,
                                            APRegister,
                                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                                            .Update(this, inv.RefNbr);
                                    throw new Exception(e.ToString());

                                }
                            }
                        }
                    }
                }

                INVViews.Cache.Clear();
                ADRViews.Cache.Clear();
                INVViews.Cache.ClearQueryCache();
                ADRViews.Cache.ClearQueryCache();

                ts.Complete();
            }
        }
        private void ChangePaymentMethod(int ChangePaymentMethod)
        {
            PXLongOperation.StartOperation(this, delegate()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    string pm = "";
                    string pmID = "";
                    switch (ChangePaymentMethod)
                    {
                        case 1:
                            pm = PaymentMethod.A;
                            pmID = "CHECK";
                            break;
                        case 2:
                            pm = PaymentMethod.B;
                            pmID = "TT";
                            break;
                    }
                    PXResultset<KGBillPayment> invset = GetSelectINVView();
                    foreach (KGBillPayment kgbillPayment in invset)
                    {
                        BAccount bAccount = GetBAccount(kgbillPayment.VendorID);
                        int? VendorLocationID =
                            NMLocationUtil.GetDefLocationByPaymentMethod(kgbillPayment.VendorID, pm)
                                ?? throw new Exception(String.Format("此供應商({0})沒有對應付款方式的所在地!", bAccount.AcctCD));
                        Location location = GetLocation(VendorLocationID);
                        NMBankAccount bankAccount = GetBankAccount(
                                location.VCashAccountID ?? throw new Exception(String.Format("此供應商({0})所在地沒有對應的現金帳號!", bAccount.AcctCD)), pmID);
                        if (bankAccount == null) throw new Exception(String.Format("此供應商({0})所在地沒有對應的NM銀行帳號!", bAccount.AcctCD));
                        PXUpdate<
                            Set<KGBillPayment.paymentMethod, Required<KGBillPayment.paymentMethod>,
                            Set<KGBillPayment.vendorLocationID, Required<KGBillPayment.vendorLocationID>,
                            Set<KGBillPayment.bankAccountID, Required<KGBillPayment.bankAccountID>>>>,
                            KGBillPayment,
                            Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                            .Update(this, pm, VendorLocationID, bankAccount.BankAccountID, kgbillPayment.BillPaymentID);
                    }

                    ts.Complete();
                }
            });
        }
        public void INVWriteoff(KGBillPayment kGBillPayment)
        {
            APInvoice inv = GetAPInvoice(kGBillPayment.RefNbr);
            APPayment payment = null;
            APRegister register = PXSelect<APRegister, Where<APRegister.refNbr, Equal<Required<APInvoice.refNbr>>>>.Select(this, inv.RefNbr);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            string adjdDocType = "";
            string adjdRefNbr = "";

            if (kGBillPayment.PaymentMethod == PaymentMethod.C ||
              (kGBillPayment.PaymentMethod == PaymentMethod.D && inv.ProjectID == 0) ||
                kGBillPayment.PaymentMethod == PaymentMethod.E)
            {
                APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();

                APPayment apPayment = null;
                apPayment = (APPayment)entry.Document.Cache.CreateInstance();
                apPayment.CuryID = inv.CuryID;
                apPayment.CuryInfoID = inv.CuryInfoID;
                apPayment.BranchID = inv.BranchID;
                apPayment.Hold = true;
                apPayment.DocType = APDocType.Check;
                apPayment = entry.Document.Insert(apPayment);
                entry.Document.SetValueExt<APPayment.vendorID>(apPayment, inv.VendorID);
                entry.Document.SetValueExt<APPayment.vendorLocationID>(apPayment, inv.VendorLocationID);

                string PaymentMethodID = "";
                switch (kGBillPayment?.PaymentMethod)
                {
                    case Kedge.DAC.PaymentMethod.C:
                    case Kedge.DAC.PaymentMethod.E: //2021/06/29 Add E the same with C
                        PaymentMethodID = "CASH";
                        break;
                    case Kedge.DAC.PaymentMethod.D:
                        PaymentMethodID = "COUPON";
                        break;
                }
                entry.Document.SetValueExt<APPayment.paymentMethodID>(apPayment, PaymentMethodID);
                NMBankAccount bankAccount = GetBankAccount(kGBillPayment?.BankAccountID);
                entry.Document.SetValueExt<APPayment.cashAccountID>(apPayment, bankAccount?.CashAccountID);
                entry.Document.SetValueExt<APPayment.docDesc>(apPayment, inv.DocDesc);
                entry.Document.SetValueExt<APPayment.adjDate>(apPayment, inv.DocDate);
                entry.Document.SetValueExt<APPayment.curyOrigDocAmt>(apPayment, kGBillPayment?.PaymentAmount);
                entry.Document.SetValueExt<APPayment.extRefNbr>(apPayment, inv.RefNbr);

                entry.Save.Press();

                payment = entry.Document.Current;
                adjdDocType = inv.DocType;
                adjdRefNbr = inv.RefNbr;
                APAdjust adj = new APAdjust();
                adj.AdjdDocType = adjdDocType;
                adj.AdjdRefNbr = adjdRefNbr;
                adj.AdjdLineNbr = 0;
                //set origamt to zero to apply "full" amounts to invoices.
                entry.Document.Cache.SetValueExt<APPayment.curyOrigDocAmt>(entry.Document.Current, 0m);
                adj = PXCache<APAdjust>.CreateCopy(entry.Adjustments.Insert(adj));

                adj.CuryAdjgAmt = kGBillPayment.PaymentAmount;
                adj = PXCache<APAdjust>.CreateCopy((APAdjust)entry.Adjustments.Cache.Update(adj));
                decimal? CuryApplAmt = kGBillPayment.PaymentAmount;

                APPayment copy = PXCache<APPayment>.CreateCopy(entry.Document.Current);
                copy.CuryOrigDocAmt = CuryApplAmt;
                entry.Document.SetValueExt<APPayment.hold>(copy, false);

                entry.Document.Cache.Update(copy);
                entry.Save.Press();

                entry.release.Press();

                //2021/07/14 Add Mantis: 0012148
                //產生對應的GL
                if (kGBillPayment.PaymentMethod == PaymentMethod.C ||
                                kGBillPayment.PaymentMethod == PaymentMethod.D ||
                                kGBillPayment.PaymentMethod == PaymentMethod.E)
                {
                    KGVoucherUtil.CreateGLFromKGBillPayment(kGBillPayment, GLStageCode.NMPWriteoffB1);
                }
            }
            PXUpdate<Set<KGBillPaymentExt.usrIsWriteOff, True,
                Set<KGBillPaymentExt.usrIsCheckIssued, True>>,
                KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Update(this, kGBillPayment.BillPaymentID);
        }

        public void ADRWriteoff(APRegister inv)
        {
            APPayment payment = null;
            //APRegister register = GetAPRegister(inv.RefNbr);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(inv);
            APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
            string adjdDocType = "";
            string adjdRefNbr = "";

            if (inv.DocType == APDocType.DebitAdj && registerExt.UsrIsDeductionDoc == true)
            {
                try
                {
                    payment = entry.Document.Current = entry.Document.Search<APPayment.refNbr>(inv.RefNbr, inv.DocType);
                    adjdDocType = registerExt.UsrOriDeductionDocType;
                    adjdRefNbr = registerExt.UsrOriDeductionRefNbr;
                    APAdjust adj1 = new APAdjust();
                    adj1.AdjdDocType = adjdDocType;
                    adj1.AdjdRefNbr = adjdRefNbr;
                    adj1.AdjdLineNbr = 0;
                    //set origamt to zero to apply "full" amounts to invoices.
                    entry.Document.Cache.SetValueExt<APPayment.curyOrigDocAmt>(entry.Document.Current, 0m);
                    adj1 = PXCache<APAdjust>.CreateCopy(entry.Adjustments.Insert(adj1));

                    adj1.CuryAdjgAmt = inv.CuryOrigDocAmt;
                    adj1 = PXCache<APAdjust>.CreateCopy((APAdjust)entry.Adjustments.Cache.Update(adj1));
                    decimal? CuryApplAmt = entry.Document.Current.CuryApplAmt;

                    APPayment copy1 = PXCache<APPayment>.CreateCopy(entry.Document.Current);
                    copy1.CuryOrigDocAmt = CuryApplAmt;
                    entry.Document.SetValueExt<APPayment.hold>(copy1, false);
                    entry.Document.Cache.Update(copy1);
                    entry.Save.Press();
                    entry.release.Press();

                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, True>,
                           APRegister,
                           Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                           .Update(this, inv.RefNbr);
                }
                catch (Exception e)
                {
                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, False>,
                            APRegister,
                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                            .Update(this, inv.RefNbr);
                    throw new Exception(e.ToString());
                }
            }
            else if (inv.DocType == APDocType.DebitAdj && inv.OrigDocType != null && inv.OrigRefNbr != null)
            {
                try
                {
                    payment = entry.Document.Current = entry.Document.Search<APPayment.refNbr>(inv.RefNbr, inv.DocType);
                    adjdDocType = inv.OrigDocType;
                    adjdRefNbr = inv.OrigRefNbr;
                    APAdjust adj2 = new APAdjust();
                    adj2.AdjdDocType = adjdDocType;
                    adj2.AdjdRefNbr = adjdRefNbr;
                    adj2.AdjdLineNbr = 0;
                    //set origamt to zero to apply "full" amounts to invoices.
                    entry.Document.Cache.SetValueExt<APPayment.curyOrigDocAmt>(entry.Document.Current, 0m);
                    adj2 = PXCache<APAdjust>.CreateCopy(entry.Adjustments.Insert(adj2));

                    adj2.CuryAdjgAmt = inv.CuryOrigDocAmt;
                    adj2 = PXCache<APAdjust>.CreateCopy((APAdjust)entry.Adjustments.Cache.Update(adj2));
                    decimal? CuryApplAmt = entry.Document.Current.CuryApplAmt;

                    APPayment copy2 = PXCache<APPayment>.CreateCopy(entry.Document.Current);
                    copy2.CuryOrigDocAmt = CuryApplAmt;
                    entry.Document.SetValueExt<APPayment.hold>(copy2, false);
                    entry.Document.Cache.Update(copy2);
                    entry.Save.Press();
                    entry.release.Press();
                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, True>,
                            APRegister,
                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                            .Update(this, inv.RefNbr);
                }
                catch (Exception e)
                {
                    PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, False>,
                            APRegister,
                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                            .Update(this, inv.RefNbr);
                    throw new Exception(e.ToString());

                }
            }
        }
        #endregion

        #region Select Method
        private PXResultset<KGBillPayment> GetSelectINVView()
        {
            return PXSelect<KGBillPayment, Where<KGBillPaymentExt.selected, Equal<True>>>.Select(this);
        }
        private APInvoice GetAPInvoice(string RefNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(this, RefNbr);
        }
        private NMBankAccount GetBankAccount(int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<KGBillPayment.bankAccountID>>>>
                .Select(this, BankAccountID);
        }
        private NMBankAccount GetBankAccount(int? CashAccount, string PaymentMethod)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>>>>
                .Select(this, CashAccount, PaymentMethod);
        }
        private Location GetLocation(int? VendorLocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(this, VendorLocationID);
        }
        private BAccount GetBAccount(int? BAccount)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(this, BAccount);
        }
        private PXResultset<APRegister> GetSelectAPInvoice()
        { 
           ADRViews.WhereAnd<Where<APRegister.selected, Equal<True>>>();
            var apdocs = new PXResultset<APRegister>();
            foreach(PXResult<APRegister> ap in ADRViews.Select())
            {
                apdocs.Add(ap);
            }
            return apdocs;
        }
        private APRegister  GetAPRegister(string RefNbr)
        {
            return PXSelect<APRegister, Where<APRegister.refNbr, Equal<Required<APInvoice.refNbr>>>>.Select(this, RefNbr);
        }
        #endregion

        #region KGWriteOffProcess Fliter
        [Serializable]
        public class KGWriteOffFilter : IBqlTable
        {
            #region Target
            [PXInt()]
            [PXUIField(DisplayName = "Target",Required = true)]
            [KGWriteOffTarget]
            [PXUnboundDefault(typeof(KGWriteOffTarget.inv))]
            public virtual int? Target { get; set; }
            public abstract class target : PX.Data.BQL.BqlInt.Field<target> { }
            #endregion 

            #region RefNbr
            [PXString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "RefNbr")]
            [PXSelector(typeof(Search<APInvoice.refNbr,
                Where<APInvoice.status, Equal<APDocStatus.open>>>),
                typeof(APInvoice.docType),
                typeof(APInvoice.refNbr),
                typeof(APInvoice.docDesc),
                typeof(APInvoice.status),
                typeof(APInvoice.docDate),
                typeof(APInvoice.dueDate),
                typeof(APInvoice.tranPeriodID))]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : IBqlField { }
            #endregion

            #region AccConfrimNbr
            [PXString(15, IsUnicode = true)]
            [PXUIField(DisplayName = "AccConfrimNbr")]
            [PXSelector(typeof(Search<APRegisterFinExt.usrAccConfirmNbr,
                Where<APRegister.status, Equal<APDocStatus.open>,
                    And<APRegisterFinExt.usrAccConfirmNbr,IsNotNull>>>),
                typeof(APRegisterFinExt.usrAccConfirmNbr),
                typeof(APInvoice.docType),
                typeof(APInvoice.refNbr),
                typeof(APInvoice.docDesc),
                typeof(APInvoice.status),
                typeof(APInvoice.docDate),
                typeof(APInvoice.dueDate),
                typeof(APInvoice.tranPeriodID))]
            public virtual string AccConfrimNbr { get; set; }
            public abstract class accConfrimNbr : IBqlField { }
            #endregion

            #region ContractID
            [PXInt()]
            [PXUIField(DisplayName = "Project")]
            [PXSelector(typeof(Search2<PMProject.contractID,
                    LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                    LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                    Equal<PMProject.contractID>>>>,
                    Where<PMProject.baseType, Equal<CTPRType.project>,
                     And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                    , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                    typeof(Customer.acctName), typeof(PMProject.status),
                    typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
            public virtual int? ContractID { get; set; }
            public abstract class contractID : IBqlField { }
            #endregion

            #region PaymentDate
            [PXDate()]
            [PXUIField(DisplayName = "PaymentDate")]
            [PXUIVisible(typeof(Where<target,Equal<KGWriteOffTarget.inv>>))]
            public virtual DateTime? PaymentDate { get; set; }
            public abstract class paymentDate : IBqlField { }
            #endregion

            #region DueDate
            [PXDate()]
            [PXUIField(DisplayName = "DueDate")]
            [PXUnboundDefault(typeof(AccessInfo.businessDate))]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : IBqlField { }
            #endregion

            #region TrPaymentType
            [PXString(1)]
            [PXUIField(DisplayName = "PaymentType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]
            [PXUIVisible(typeof(Where<target, Equal<KGWriteOffTarget.inv>>))]

            public virtual string TrPaymentType { get; set; }
            public abstract class trPaymentType : PX.Data.BQL.BqlString.Field<trPaymentType> { }
            #endregion

            #region TrConfirmID
            [PXInt()]
            [PXUIVisible(typeof(Where<target,Equal<KGWriteOffTarget.inv>>))]
            [PXUIField(DisplayName = "TrConfirmID")]
            public virtual int? TrConfirmID { get; set; }
            public abstract class trConfirmID : PX.Data.BQL.BqlInt.Field<trConfirmID> { }
            #endregion

            #region TrConfirmDate
            [PXDate()]
            [PXUIField(DisplayName = "TrConfirmDate")]
            [PXUIVisible(typeof(Where<target,Equal<KGWriteOffTarget.inv>>))]
            public virtual DateTime? TrConfirmDate { get; set; }
            public abstract class trConfirmDate : IBqlField { }
            #endregion

            #region TrConfirmBy
            [PXGuid]
            [PXUIField(DisplayName = "TrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]
            [PXUIVisible(typeof(Where<target, Equal<KGWriteOffTarget.inv>>))]

            public virtual Guid? TrConfirmBy { get; set; }
            public abstract class trConfirmBy : PX.Data.BQL.BqlGuid.Field<trConfirmBy> { }
            #endregion

            #region PaymentMethod
            [PXString(1, IsUnicode = true)]
            [PXUIField(DisplayName = "PaymentMethod")]
            [KGWriteOffPaymentMethod]
            [PXUIVisible(typeof(Where<target,Equal<KGWriteOffTarget.inv>>))]
            public virtual string PaymentMethod { get; set; }
            public abstract class paymentMethod : IBqlField { }
            #endregion 
        }
        #endregion

        #region Dropdown List
        public class KGWriteOffTarget : PXIntListAttribute
        {
            public KGWriteOffTarget() : base(
                new[]
                {
                    Pair(INV,"一般計價"),
                    Pair(ADR,"借方調整")
                })
            { }

            //Const
            /// <summary>
            /// 一般計價
            /// </summary>
            public const int INV = 1;
            /// <summary>
            /// 借方調整
            /// </summary>
            public const int ADR = 2;

            /// <summary>
            /// 一般計價
            /// </summary>
            public class inv : PX.Data.BQL.BqlInt.Constant<inv> { public inv() : base(INV) {; } }
            /// <summary>
            /// 借方調整
            /// </summary>
            public class adr : PX.Data.BQL.BqlInt.Constant<adr> { public adr() : base(ADR) {; } }

        }
        public class KGWriteOffPaymentMethod : PXStringListAttribute
        {
            public KGWriteOffPaymentMethod() : base(
                new[]
                {
                    Pair(Cash,"現金"),
                    Pair(Coupon,"禮券"),
                    Pair(Auth,"授扣")
                })
            { }

            //Const
            /// <summary>
            /// 現金
            /// </summary>
            public const string Cash = "C";
            /// <summary>
            /// 禮券
            /// </summary>
            public const string Coupon = "D";
            /// <summary>
            /// 授扣
            /// </summary>
            public const string Auth = "E";

            /// <summary>
            /// 現金
            /// </summary>
            public class cash : PX.Data.BQL.BqlString.Constant<cash> { public cash() : base(Cash) {; } }
            /// <summary>
            /// 禮券
            /// </summary>
            public class coupon : PX.Data.BQL.BqlString.Constant<coupon> { public coupon() : base(Coupon) {; } }
            /// <summary>
            /// 授扣
            /// </summary>
            public class auth : PX.Data.BQL.BqlString.Constant<auth> { public auth() : base(Auth) {; } }

        }
        #endregion
    }
}