using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.PO;
using Kedge.DAC;
using RCGV.GV.DAC;
using CC;
using CC.DAC;
using CC.Util;
using EP.Util;
using EP.DAC;
using Fin.DAC;
using NM.DAC;
using NM.Util;
using static EP.Util.EPStringList;
using static PS.Util.PSStringList;

namespace PX.Objects.EP
{
    /** 	 
     * ======2021/02/09 0011925 Edit By Althea=====
     * 	 1.��EPExpenseClaim.DocType='RGU'�ɡG
     * 	 - ����� 12.�p�GUsrDocType = 'RGU', �L�b��, ��sCCReceivableCheck�����A��'AppliedRTN'�A�אּ�G
     * 	 b. ���u�L�b�v�ɡA��update CCReceivableCheck.Status= Return
     * 	 
     * === 2021/03/23 Fix EP === Edit By Althea
     * Fix EP�L�b�A����KGVoucher
     * 
     * === 2021-05-13 ==== Alton
     * �u�I�ڤ覡�ܧ�v�@�~�覡�אּ�P�u�@�o�B�h�����v�ۦP
     * 
     * ===2021/05/13 === Althea
     * Add EP WHT To AP
     * 
     * ===2021/05/19  :EVA �f�Y === Althea
     * �u�������ܧ�/���Y�ܧ�/��L�~�n�ˬd
     * 
     * ====2021/05/21 louis=======
     *  �h�^�O�Ҳ���, ��s�O�Ҳ����A�� appliedRTN �Ӥ��O returned
     *  
     *  ===2021/07/15 :0012155 === Althea
     *  Add:
     *  �L�b��docType = GUR(�ǲ��@�~),�s�WGL�ǲ�,������APInvoice
     *  
     *  ===2021/07/15 :0012156 === Althea
     *  OTH �L�b�窱�A�N�n
     *  CHG ����쥻��OTH-��L�I���ܧ� �޿�
     *  
     *  ====2021-08-03:Fix===Alton
     *  KGBillsummary ��z�LEntry���o
     *  
     *  ===2021/08/04 :0012180 ===Althea
     *  EP����APTran: Add UsrProjectID ���APTran.UsrEPProjectID
     *  
     *  ===2021/10/05 :0012252 ===Althea
     *  Add EP Create CC:
     *  EPDetail.UsrCCPostageAmt = CC.CCPostageAmt
     *  
     *  ===2021/10/19 :0012256 === Althea
     *  Add EP Relese DocType = BTN
     *  
     *  ===2021/11/10 :0012266 === althea
     *  STD Release Add Log:
     *  APRegister.UsrIsTmpPayment=EPExpenseClaim.UsrIsTmpPayment
     *  APRegister.UsrTmpPaymentTotal=EPExpenseClaim.CuryDocBal
     *  APRegister.UsrTmpPaymentReleased=0
     *  APRegister.UsrTmpPaymentUnReleased=EPExpenseClaim.CuryDocBal
     *  
     *  ===2022/06/27=== Jeff
     *  Fixed an issue where new lines could not be added.
     **/
    public class EPReleaseProcessExt : PXGraphExtension<EPReleaseProcess>
    {
        #region Override Method

        public delegate void ReleaseDocProcDelegate(EPExpenseClaim claim);
        [PXOverride]
        public virtual void ReleaseDocProc(EPExpenseClaim claim, ReleaseDocProcDelegate baseMethod)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
               
                EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
                if (claimExt.UsrDocType == EPStringList.EPDocType.STD)
                {
                    claimExt.UsrApprovalStatus = EPUsrApprovalStatus.Released;
                    baseMethod(claim);
                    //�Y�o�i�ӽг欰�O�Ҳ�,�L�b�ɤ��ݭn����APInvoice����� Mantis:0011814
                    CreateAPInvocie(claim);
                }
                else if(claimExt.UsrDocType == EPStringList.EPDocType.GUR)
                {
                    //�@��detail���ͤ@��CC
                    //PXResultset<EPExpenseClaimDetails> detailset =
                    //    PXSelect<EPExpenseClaimDetails,
                    //    Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>.Select(Base, claim.RefNbr);
                    foreach(EPExpenseClaimDetails details in PXSelect<EPExpenseClaimDetails,
                                                             Where<EPExpenseClaimDetails.refNbr, 
                                                             Equal<Required<EPExpenseClaim.refNbr>>>>
                                                             .Select(Base, claim.RefNbr))
                    {

                        PXUpdate<
                            Set<EPExpenseClaimDetailsExt.usrGuarPayableCD, Required<EPExpenseClaimDetailsExt.usrGuarPayableCD>>,
                            EPExpenseClaimDetails,
                            Where<EPExpenseClaimDetails.claimDetailID, Equal<Required<EPExpenseClaimDetails.claimDetailID>>>>
                            .Update(Base, CreateCCPayable(claim,details), details.ClaimDetailID);

                    }

                    //EP���A�L�b
                    ReleaseEPStatus(claim);
                }
                else if(claimExt.UsrDocType == EPStringList.EPDocType.RGU)
                {
                    //�Ndetail��������CC���� �⪬�A�אּ'AppliedRTN' �w�ӽ�ú�^�O�Ҳ�.
                    //2021/02/09 �אּReturn
                    //PXResultset<EPExpenseClaimDetails> detailset =
                    //    PXSelect<EPExpenseClaimDetails,
                    //    Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                    //    .Select(new PXGraph(), claim.RefNbr);

                    foreach (EPExpenseClaimDetails details in PXSelect<EPExpenseClaimDetails,
                                      Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                                      .Select(Base, claim.RefNbr))
                    {
                        //EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
                        //Base.SelectTimeStamp();
                        CC.CCReceivableEntry.CreateAppliedRTNVoucher(details.GetExtension<EPExpenseClaimDetailsExt>().UsrGuarReceviableCD, details);
                        /**
                        PXUpdate<
                        //Set<CCReceivableCheck.status, CCList.CCReceivableStatus.returned>, --modify by louis for status update to appliedRTN
                        Set<CCReceivableCheck.status, CCList.CCReceivableStatus.appliedRTN>,
                        CCReceivableCheck,
                        Where<CCReceivableCheck.guarReceviableCD,
                        Equal<Required<EPExpenseClaimDetailsExt.usrGuarReceviableCD>>>>
                        .Update(Base, detailsExt.UsrGuarReceviableCD);**/
                    }
                    //EP���A�L�b
                    ReleaseEPStatus(claim);
                }
                //2021/07/15 Mantis: 0012156 OTH --> CHG
                else if(claimExt.UsrDocType == EPStringList.EPDocType.CHG)
                {
                    UpdateCCorNMStatus(claim);
                    //EP���A�L�b
                    ReleaseEPStatus(claim);
                }
                //2021/07/15 Add Mantis: 0012155
                else if (claimExt.UsrDocType == EPStringList.EPDocType.GLB)
                {
                    EPVoucherUtil.CreateEPVoucher(claim);
                    //EP���A�L�b
                    ReleaseEPStatus(claim);
                }
                //2021/07/15 Add Mantis: 0012156
                else if(claimExt.UsrDocType == EPStringList.EPDocType.OTH)
                {
                    ReleaseEPStatus(claim);
                }
                //2021/10/19 Add Mantis : 0012256
                //20220808r���ĥ�������Ͷǲ�
                else if(claimExt.UsrDocType == EPStringList.EPDocType.BTN)
                {
                    //EPVoucherUtil.CreateBTNVoucher(claim);
                    ReleaseEPStatus(claim);
                }

                ts.Complete();
            }
        }

        #endregion

        #region Event Handlers

        #endregion

        #region Method
        /// <summary>
        /// Create new CC Payable ,Data from Tab:CC
        /// </summary>
        /// <param name="header">Current EP</param>
        /// <param name="item">details</param>
        /// <returns></returns>
        public String CreateCCPayable(EPExpenseClaim header,EPExpenseClaimDetails item)
        {
            /*EPExpenseClaimDetails item = PXSelect<EPExpenseClaimDetails,
                          Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                                  .Select(Base, header.RefNbr);*/
            EPExpenseClaimExt headerExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(header);
            EPExpenseClaimDetailsExt itemExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(item);

            CCPayableEntry entry = PXGraph.CreateInstance<CCPayableEntry>();
            PXCache cache = entry.PayableChecks.Cache;
            CCPayableCheck check = (CCPayableCheck)cache.CreateInstance();
            check = entry.PayableChecks.Insert(check);
            //cache.SetValueExt<CCReceivableCheck.docDate>(check, null);//�w�]
            //cache.SetValueExt<CCReceivableCheck.status>(check, null);//�w�]
            //cache.SetValueExt<CCPayableCheck.guarNbr>(check, item.ExpenseRefNbr);
            cache.SetValueExt<CCPayableCheck.issueDate>(check, itemExt.UsrIssueDate??null);
            cache.SetValueExt<CCPayableCheck.authDate>(check, itemExt.UsrAuthDate??null);
            cache.SetValueExt<CCPayableCheck.guarType>(check, itemExt.UsrGuarType);
            cache.SetValueExt<CCPayableCheck.guarClass>(check, itemExt.UsrGuarClass);
            cache.SetValueExt<CCPayableCheck.projectID>(check, item.ContractID);
            cache.SetValueExt<CCPayableCheck.description>(check, header.DocDesc);
            cache.SetValueExt<CCPayableCheck.dueDate>(check, itemExt.UsrDueDate??null);
            EPEmployee employee = GetEmployee();
            cache.SetValueExt<CCPayableCheck.contractorID>(check, employee.BAccountID);
            cache.SetValueExt<CCPayableCheck.depID>(check, header.DepartmentID);
            cache.SetValueExt<CCPayableCheck.cashierID>(check, header.EmployeeID);
            cache.SetValueExt<CCPayableCheck.branchID>(check, item.BranchID);
            cache.SetValueExt<CCPayableCheck.targetType>(check, headerExt.UsrTargetType);
            cache.SetValueExt<CCPayableCheck.guarAmt>(check, item.CuryUnitCost);
            //2021/10/05 Add Mantis: 0012252
            cache.SetValueExt<CCPayableCheck.ccPostageAmt>(check, itemExt.UsrCCPostageAmt);

            bool isCashierCheck = itemExt.UsrGuarClass == CCList.GuarClassList.CashiersCheck;
            if (headerExt.UsrTargetType == PSTargetType.Vendor)
            {
                BAccount2 bAccount = GetBAccount2(headerExt.UsrVendorID);
                cache.SetValueExt<CCPayableCheck.guarTitle>(check, bAccount.AcctName);
                check.VendorID = headerExt.UsrVendorID;
                check.VendorLocationID = headerExt.UsrVendorLocationID;
                LocationExtAddress location = GetLocation(headerExt.UsrVendorLocationID);
                NMBankAccount bankAccount = GetBankAccount(location.VCashAccountID, "CHECK");
                if (bankAccount == null)
                    throw new Exception("�г]�w�{���b��!");
                cache.SetValueExt<CCPayableCheck.bankAccountID>(check, bankAccount.BankAccountID);

                //check.BankAccountID = bankAccount.BankAccountID;
                //check.BankCode = bankAccount.BankCode;
                //Mantis: 001834 Add 
                //Mantis:0011872
                /*int BankAccoutID;
                if (isCashierCheck)
                {
                    //BankAccoutID = itemExt.UsrBankAccountID?? throw new Exception("�к��@�Ȧ�b��²�X���!");
                    //check.BankAccountID = BankAccoutID;
                    //NMBankAccount bankAccout = GetBankAccount(BankAccoutID);
                    //check.BankCode = bankAccout.BankCode;
                    //check.BookCD = itemExt.UsrBookCD;
                }
                else
                {
                    LocationExtAddress location = GetLocation(headerExt.UsrVendorLocationID);
                    NMBankAccount bankAccount = GetBankAccount(location.VCashAccountID,"CHECK");
                    if (bankAccount == null)
                        throw new Exception("�г]�w�{���b��!");
                    check.BankAccountID = bankAccount.BankAccountID;
                    check.BankCode = bankAccount.BankCode;
                }*/



                check.PONbr = itemExt.UsrPONbr ?? null;
                if (itemExt.UsrPONbr != null)
                {
                    POOrder order = GetPOOrder(itemExt.UsrPONbr);
                    check.POOrderType = order.OrderType ?? null;
                }
            }
            else if (headerExt.UsrTargetType == PSTargetType.Customer)
            {
                BAccount2 bAccount = GetBAccount2(header.CustomerID);
              
                //Mantis: 001834 Add 
                cache.SetValueExt<CCPayableCheck.guarTitle>(check, bAccount.AcctName);
                cache.SetValueExt<CCPayableCheck.customerID>(check, item.CustomerID ?? null);
                cache.SetValueExt<CCPayableCheck.customerLocationID>(check, item.CustomerLocationID ?? null);

                LocationExtAddress location = GetLocation(header.CustomerID);
                NMBankAccount bankAccount = GetBankAccount(location.VCashAccountID, "CHECK");
                cache.SetValueExt<CCPayableCheck.bankAccountID>(check, bankAccount.BankAccountID);
                
                //Mantis:0011872
                /*int BankAccountID;
                if(isCashierCheck)
                {
                    //BankAccountID = itemExt.UsrBankAccountID ?? throw new Exception("�к��@�Ȧ�²�X!");
                    //NMBankAccount bankAccount = GetBankAccount(BankAccountID);
                    //cache.SetValueExt<CCPayableCheck.bankAccountID>(check, BankAccountID);
                    //cache.SetValueExt<CCPayableCheck.bankCode>(check, bankAccount.BankCode);
                    //cache.SetValueExt<CCPayableCheck.bookCD>(check, itemExt.UsrBookCD);

                }
                else
                {
                    LocationExtAddress location = GetLocation(header.CustomerID);
                    NMBankAccount bankAccount = GetBankAccount(location.VCashAccountID, "CHECK");
                    cache.SetValueExt<CCPayableCheck.bankAccountID>(check, bankAccount.BankAccountID);
                    cache.SetValueExt<CCPayableCheck.bankCode>(check, bankAccount.BankCode);
                }*/

            }




            entry.Persist();

            return entry.PayableChecks.Current.GuarPayableCD;
        }

        /// <summary>
        /// Update AP Invoice some Fields and Set APRefNbr to GV & KGBillPayment,
        /// and create KGVoucher
        /// </summary>
        /// <param name="claim">Current EP</param>
        public void CreateAPInvocie(EPExpenseClaim claim)
        {
            //ExpenseClaimEntry_Extension2 expenseClaimGraph2 = expenseClaimGraph.GetExtension<ExpenseClaimEntry_Extension2>();
            // baseMethod(claim);
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            EPExpenseClaimDetails detail = PXSelect<EPExpenseClaimDetails,
                       Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                               .Select(Base, claim.RefNbr);

            //��gAPInvoice Vendor���
            //��t�L�b�|��APInvoice�������Ӷ�J���uID
            //�Ȼs���YEP���������, APInvoice�������ӫh�a�JEP��������,�Y�S��N�ӭ�t
            //�Ψ�����ID���oPayType & PayAccount
            string PayTypeID;
            int? PayAccountID;
            if (claimExt.UsrVendorID != null)
            {
                LocationExtAddress locationExtAddress =
                    PXSelect<LocationExtAddress,
                    Where<LocationExtAddress.bAccountID, Equal<Required<EPExpenseClaimExt.usrVendorID>>>>
                    .Select(Base, claimExt.UsrVendorID);
                PayTypeID = locationExtAddress.VPaymentMethodID ?? throw new Exception("�Цܨ����ӳ]�w�I�ڤ�k!");
                PayAccountID = locationExtAddress.VCashAccountID ?? throw new Exception("�Цܨ����ӳ]�w�{�����!");
            }
            else
            {
                Location location =
                    PXSelect<Location,
                    Where<Location.bAccountID, Equal<Required<EPExpenseClaim.employeeID>>>>
                    .Select(Base, claim.EmployeeID);
                PayTypeID = location.VPaymentMethodID ?? throw new Exception("�Цܭ��u�]�w�I�ڤ�k!");
                PayAccountID = location.VCashAccountID ?? throw new Exception("�Цܭ��u�]�w�{�����!");

            }

            DateTime BussinessDate = (DateTime)(Base.Accessinfo.BusinessDate);
            DateTime? PayDate;
            KGBillPayment payperiod =
                PXSelectGroupBy<KGBillPayment,
                Where<KGBillPaymentExt.usrEPRefNbr,
                    Equal<Required<KGBillPaymentExt.usrEPRefNbr>>>,
                Aggregate<
                    Min<KGBillPayment.paymentPeriod,
                    GroupBy<KGBillPaymentExt.usrEPRefNbr>>>>.Select(Base, claim.RefNbr);
            if (payperiod != null)
                PayDate = Kedge.VoucherUntil.GetLongTremPaymentDate(Base, BussinessDate, payperiod.PaymentPeriod);
            else
                PayDate = Kedge.VoucherUntil.GetLongTremPaymentDate(Base, BussinessDate, 0);
            //�^��APInvoice ��� 
            var invoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();

            APInvoice invoice = PXSelect<APInvoice, Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APInvoice.docType,Equal<Required<APInvoice.docType>>>>>.Select(invoiceEntry, detail.APRefNbr,detail.APDocType);

            invoiceEntry.Document.Current = invoice;
            invoiceEntry.Document.SetValueExt<APInvoice.projectID>(invoice, detail.ContractID);
            invoiceEntry.Document.SetValueExt<APInvoice.docDate>(invoice, BussinessDate);
            invoiceEntry.Document.SetValueExt<APInvoice.payTypeID>(invoice, PayTypeID);
            invoiceEntry.Document.SetValueExt<APInvoice.payAccountID>(invoice, PayAccountID);
            invoiceEntry.Document.SetValueExt<APInvoice.payDate>(invoice, PayDate);
            invoiceEntry.Document.SetValueExt<APInvoice.dueDate>(invoice, PayDate);
            //2021/11/10 Add Mantis: 0012266
            invoiceEntry.Document.SetValueExt<APRegisterExt.usrIsTmpPayment>(invoice, claimExt.UsrIsTmpPayment);
            invoiceEntry.Document.SetValueExt<APRegisterExt.usrTmpPaymentTotal>(invoice, claim.CuryDocBal);
            invoiceEntry.Document.SetValueExt<APRegisterExt.usrTmpPaymentReleased>(invoice, 0m);
            invoiceEntry.Document.SetValueExt<APRegisterExt.usrTmpPaymentUnReleased>(invoice, claim.CuryDocBal);

            if (claimExt.UsrVendorID != null)
            {
                invoiceEntry.Document.Cache.SetValue<APInvoice.vendorID>(invoice, claimExt.UsrVendorID);
                invoiceEntry.Document.Cache.SetValue<APInvoice.vendorLocationID>(invoice, claimExt.UsrVendorLocationID);

                invoiceEntry.Transactions.Cache.SetValue<APInvoice.vendorID>(invoiceEntry.Transactions.Current, claimExt.UsrVendorID);
                invoiceEntry.Transactions.UpdateCurrent();
            }
            invoice =invoiceEntry.Document.Update(invoice);
            invoiceEntry.Actions.PressSave();

            int LineNbr =1;
            //2021/06/04 Add Mantis: 0012080
            //�|�B���D
            //��APTran.UsrValuationType
            foreach(EPExpenseClaimDetails detailsEP in PXSelect<EPExpenseClaimDetails,
                       Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>,
                       OrderBy<Asc<EPExpenseClaimDetails.claimDetailID>>>
                               .Select(Base, claim.RefNbr))
            {             
                APTran aPTran = PXSelect<APTran, Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                    And<APTran.lineNbr, Equal<Required<EPExpenseClaimDetails.aPLineNbr>>>>>
                    .Select(Base, invoice.RefNbr, LineNbr);
                EPExpenseClaimDetailsExt detailsEPExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(detailsEP);
                invoiceEntry.Transactions.SetValueExt<APTran.curyTaxAmt>(aPTran, detailsEP.CuryTaxTotal);
                invoiceEntry.Transactions.SetValueExt<APTranExt.usrValuationType>(aPTran, detailsEPExt.UsrValuationType);
                //2021/08/04 Add Mantis: 0012180
                invoiceEntry.Transactions.SetValueExt<APTranExt.usrEPProjectID>(aPTran, detailsEPExt.UsrProjectID);        
                invoiceEntry.Transactions.Update(aPTran);
                LineNbr++;
            }
            invoiceEntry.Document.SetValueExt<APInvoice.curyTaxAmt>(invoice, claim.CuryTaxTotal);
            invoiceEntry.Document.SetValueExt<APInvoice.curyTaxTotal>(invoice, claim.CuryTaxTotal);
            invoiceEntry.Document.SetValueExt<APInvoice.curyDocBal>(invoice, claim.CuryDocBal);
            invoiceEntry.Document.Update(invoice);

            //2021-08-03 Alton ���Entry�ۤv��
            foreach (APTaxTran item in invoiceEntry.Taxes.Select()) {
                if (item.Module == BatchModule.AP && item.TranType == invoice.DocType && item.RefNbr == invoice.RefNbr) {
                    invoiceEntry.Taxes.Cache.SetValueExt<APTaxTran.curyTaxAmt>(item, claim.CuryTaxTotal);
                    invoiceEntry.Taxes.Update(item);
                }
            }
            /* -- 2021-08-03 Alton ���Entry�ۤv��
            //       APTaxTran taxtran = PXSelect<APTaxTran, Where< APTaxTran.module, Equal<BatchModule.moduleAP>,
            //And < APTaxTran.tranType, Equal<Required<APInvoice.docType>>,
            //And < APTaxTran.refNbr, Equal <Required< APInvoice.refNbr >>>>>>.Select(Base, invoice.DocType,invoice.RefNbr);
            //       invoiceEntry.Taxes.SetValueExt<APTaxTran.curyTaxAmt>(taxtran, claim.CuryTaxTotal);
            //       invoiceEntry.Taxes.Update(taxtran);
            */

            //2021/05/13 Add Add WHT From EP to AP
            ///<remarks>Mantis [0012312] - EP��J�N���|�p�G���h��, �L�b���u�|���Ĥ@���ҫO�O, ��L�X�����|�a��AP Bill.</remarks>
            //var invoiceEntryFinExt = invoiceEntry.GetExtension<APInvoiceEntryFinExt>();
            //PXResultset<TWNWHTTran> whttran = GetTWWHTTran(claim.RefNbr);
            ExpenseClaimEntry expenseClaimGraph = PXGraph.CreateInstance<ExpenseClaimEntry>();
            int count = 0, lineCntr = 0;
            foreach (TWNWHTTran tran in PXSelect<TWNWHTTran, Where<TWNWHTTran.ePRefNbr, Equal<Required<EPExpenseClaim.refNbr>>>>.Select(expenseClaimGraph, claim.RefNbr))
            {
                //expenseClaimGraph.ExpenseClaimCurrent.Current = claim;
                
                var wHTView = expenseClaimGraph.GetExtension<ExpenseClaimEntryForWHT>().WHTView;

                wHTView.Current = tran;

                wHTView.Cache.SetValue<TWNWHTTran.docType>(tran, detail.APDocType);
                wHTView.Cache.SetValue<TWNWHTTran.refNbr>(tran, detail.APRefNbr);
                wHTView.Cache.SetValue<TWNWHTTran.lineNbr>(tran, tran.LineNbr + count++);
                wHTView.Cache.MarkUpdated(tran);
                lineCntr = count;
                //invoiceEntryFinExt.WHTView.SetValueExt<TWNWHTTran.docType>(tran, detail.APDocType);
                //invoiceEntryFinExt.WHTView.SetValueExt<TWNWHTTran.refNbr>(tran, detail.APRefNbr);
                //invoiceEntryFinExt.WHTView.Update(tran);
            }
            expenseClaimGraph.Save.Press();

            invoiceEntry.Document.SetValueExt<APRegisterFinExt.usrWhtAmt>(invoice, claimExt.UsrWhtAmt ?? 0m);
            invoiceEntry.Document.SetValueExt<APRegisterFinExt.usrGnhi2Amt>(invoice, claimExt.UsrGnhi2Amt ?? 0m);
            invoiceEntry.Document.SetValueExt<APRegisterFinExt.usrHliAmt>(invoice, claimExt.UsrHliAmt ?? 0m);
            invoiceEntry.Document.SetValueExt<APRegisterFinExt.usrLbiAmt>(invoice, claimExt.UsrLbiAmt ?? 0m);
            invoiceEntry.Document.SetValueExt<APRegisterFinExt.usrLbpAmt>(invoice, claimExt.UsrLbpAmt ?? 0m);

            var invoiceEntryExt = invoiceEntry.GetExtension<APInvoiceEntry_Extension>();

            //2021-08-03 Alton ���Entry�ۤv��
            //KGBillSummary billS = GetBillSummary(detail.APRefNbr);
            KGBillSummary billS = invoiceEntryExt.getKGBillSummary();
            invoiceEntryExt.SummaryAmtFilters.SetValueExt<KGBillSummary.whtAmt>(billS, claimExt.UsrWhtAmt ?? 0m);
            invoiceEntryExt.SummaryAmtFilters.SetValueExt<KGBillSummary.gnhi2Amt>(billS, claimExt.UsrGnhi2Amt ?? 0m);
            invoiceEntryExt.SummaryAmtFilters.SetValueExt<KGBillSummary.hliAmt>(billS, claimExt.UsrHliAmt ?? 0m);
            invoiceEntryExt.SummaryAmtFilters.SetValueExt < KGBillSummary.lbiAmt>(billS, claimExt.UsrLbiAmt ?? 0m);
            invoiceEntryExt.SummaryAmtFilters.SetValueExt<KGBillSummary.lbpAmt>(billS, claimExt.UsrLbpAmt ?? 0m);
            invoiceEntryExt.SummaryAmtFilters.Update(billS);

            invoiceEntry.Actions.PressSave();

            //2020/10/29 Mantis:0011776 �令���bEP���@,�A�L�b���ɭԶ�APRefNbr��KGBillPayment 
            count = 0;
            foreach (KGBillPayment payment in PXSelect<KGBillPayment, Where<KGBillPaymentExt.usrEPRefNbr,Equal<Required<EPExpenseClaim.refNbr>>>>.Select(expenseClaimGraph, claim.RefNbr))
            {
                DateTime? paymentDate = payment.PaymentDate;
                //2021-07-20:12147 PricingType='��L-�����p�I�ڤ�'��, �N���p�ʧ�sPaymentDate
                if (payment.PricingType == PricingType.A) {
                    paymentDate = Kedge.VoucherUntil.GetLongTremPaymentDate(Base, BussinessDate, payment.PaymentPeriod);
                }

                ///<remarks>Because DAC PK needs to be modified in different graphs.</remarks>
                PXUpdate<Set<KGBillPayment.refNbr, Required<APInvoice.refNbr>,
                             Set<KGBillPayment.docType, Required<APInvoice.docType>,
                                 Set<KGBillPayment.lineNbr, Required<KGBillPayment.lineNbr>,
                                     Set<KGBillPayment.paymentDate, Required<KGBillPayment.paymentDate>>>>>,
                         KGBillPayment,
                         Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                         .Update(Base, detail.APRefNbr, detail.APDocType, (payment.LineNbr ?? 0m) + count++, paymentDate, payment.BillPaymentID);
                lineCntr = Math.Max(lineCntr, count);
            }

            #region KGBillPayment Insert Old Methed. 
            /*BAccount bAccount = PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<EPExpenseClaimExt.usrVendorID>>>>
                .Select(Base, baccountID);
            KGBillPayment billPayment = new KGBillPayment();
            billPayment = invoiceEntryExt.KGBillPaym.Insert();
            var insertParams = new List<PXDataFieldAssign>();
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.refNbr>(aPInvoice.RefNbr));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.pricingType>("A"));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.paymentMethod>("A"));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.paymentPeriod>(0));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.paymentPct>(100));
            //2020/10/28 Mantis:0011775
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.paymentDate>(Base.Accessinfo.BusinessDate));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.paymentAmount>(invoiceEntry.Document.Current.CuryDocBal));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.checkTitle>(bAccount.AcctName));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.createdByID>(billPayment.CreatedByID));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.createdByScreenID>(billPayment.CreatedByScreenID));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.createdDateTime>(billPayment.CreatedDateTime));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.lastModifiedByID>(billPayment.LastModifiedByID));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.lastModifiedByScreenID>(billPayment.LastModifiedByScreenID));
            insertParams.Add(new PXDataFieldAssign<KGBillPayment.lastModifiedDateTime>(billPayment.LastModifiedDateTime));
            PXDatabase.Insert<KGBillPayment>(insertParams.ToArray());*/
            #endregion

            //�o��tab
            PXResultset<GVApGuiInvoiceFinRef> set = PXSelect<GVApGuiInvoiceFinRef, Where<GVApGuiInvoiceFinRef.ePRefNbr, Equal<Required<GVApGuiInvoiceFinRef.ePRefNbr>>>>
                                                                                   .Select(Base, claim.RefNbr);
            count = 0;
            foreach (GVApGuiInvoiceFinRef gvapinvoice in set)
            {
                ///<remarks>Because DAC PK needs to be modified in different graphs.</remarks>
                PXUpdate<Set<GVApGuiInvoice.refNbr, Required<APInvoice.refNbr>,
                             Set<GVApGuiInvoice.docType, Required<APInvoice.docType>,
                                 Set<GVApGuiInvoice.lineNbr, Required<GVApGuiInvoice.lineNbr>>>>,
                         GVApGuiInvoice,
                         Where<GVApGuiInvoice.ePRefNbr, Equal<Required<EPExpenseClaim.refNbr>>,
                               And<GVApGuiInvoice.guiInvoiceID, Equal<Required<GVApGuiInvoice.guiInvoiceID>>>>>
                         .Update(Base, detail.APRefNbr, detail.APDocType, (gvapinvoice.LineNbr ?? 0m) + count++, claim.RefNbr, gvapinvoice.GuiInvoiceID);
                lineCntr = Math.Max(lineCntr, count);
            }

            //����KGVoucher
            invoiceEntry.Document.Current = invoice;
            ///<remarks> Since the details line nbr is manually updated, the header line nbr control field also has to be updated.</remarks>
            invoiceEntry.Document.Cache.SetValue<APInvoice.lineCntr>(invoice, lineCntr);
            invoiceEntry.Document.Update(invoice);
            APInvoiceEntryByVoucherExt byVoucherExt = invoiceEntry.GetExtension<APInvoiceEntryByVoucherExt>();
            byVoucherExt.AddKGVoucher.Press();
        }

        /// <summary>
        /// Just update EP Status to Release.
        /// </summary>
        /// <param name="claim">Current EP</param>
        public void ReleaseEPStatus(EPExpenseClaim claim)
        {
            
            PXUpdate<Set<EPExpenseClaim.status, Required<EPExpenseClaim.status>,
                        Set<EPExpenseClaim.released, Required<EPExpenseClaim.released>,
                        Set<EPExpenseClaimExt.usrApprovalStatus, Required<EPExpenseClaimExt.usrApprovalStatus>>>>,
                    EPExpenseClaim,
                    Where<EPExpenseClaim.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                    .Update(Base, EPExpenseClaimStatus.ReleasedStatus, true, EPUsrApprovalStatus.Released, claim.RefNbr);
        }

        /// <summary>
        /// When Ep.Doctype = CHG and Release, 
        /// update Status ,Which row in Tab ModiReq Link to CC or NM 
        /// </summary>
        /// <param name="claim">Current claim</param>
        public void UpdateCCorNMStatus(EPExpenseClaim claim)
        {
            PXResultset<EPPaymentModiReq> req = getPaymentMofiReq(claim.RefNbr);

            //Check Status
            List<string> ErrorMsg1 = new List<string>();
            List<string> ErrorMsg2 = new List<string>();
            foreach (EPPaymentModiReq checkmodiReq in req)
            {            
                //2021/05/19 EVA �f�Y:�u�������ܧ�/���Y�ܧ�/��L�~�n�ˬd
                switch(checkmodiReq.ModifyType)
                {
                    case EPStringList.ModifyType.DAT:
                    case EPStringList.ModifyType.TIT:
                    case EPStringList.ModifyType.OTH:
                        switch (checkmodiReq.PaymentType)
                        {
                            case PaymentType.NMP:
                                NMPayableCheck nmpcheck = GetNMPayableCheck(checkmodiReq.NMID);
                                switch (nmpcheck.Status)
                                {
                                    case NMStringList.NMAPCheckStatus.PENDINGMODIFY:
                                    case NMStringList.NMAPCheckStatus.PENDINGREFUND:
                                        ErrorMsg1.Add("NMp-" + nmpcheck.CheckNbr);
                                        //throw new Exception("�Ӳ���/�q�� �ثe���b�s��Ϋݭק襤�A���pô�������f�T�{");
                                        break;
                                    case NMStringList.NMAPCheckStatus.CASH:
                                    case NMStringList.NMAPCheckStatus.CANCEL:
                                    case NMStringList.NMAPCheckStatus.INVALID:
                                        ErrorMsg2.Add("NMP-" + nmpcheck.CheckNbr);
                                        //throw new Exception("�������ڷ�e���A���i�ק�A�нT�{�çR���ӵ������᭫�s�e�f");
                                        break;
                                }
                                break;
                            case PaymentType.NMR:
                                NMReceivableCheck nmrcheck = GetNMReceivableCheck(checkmodiReq.NMID);
                                switch (nmrcheck.Status)
                                {
                                    case NMStringList.NMARCheckStatus.PENDINGMODIFY:
                                    case NMStringList.NMARCheckStatus.PENDINGREFUND:
                                        ErrorMsg1.Add("NMR-" + nmrcheck.CheckNbr);
                                        //throw new Exception("�Ӳ���/�q�� �ثe���b�s��Ϋݭק襤�A���pô�������f�T�{");
                                        break;
                                    case NMStringList.NMARCheckStatus.CASH:
                                    case NMStringList.NMARCheckStatus.REFUND:
                                    case NMStringList.NMARCheckStatus.WITHDRAW:
                                    case NMStringList.NMARCheckStatus.COLLECTION:
                                        ErrorMsg2.Add("NMR-" + nmrcheck.CheckNbr);
                                        //throw new Exception("�������ڷ�e���A���i�ק�A�нT�{�çR���ӵ������᭫�s�e�f");
                                        break;
                                }
                                break;
                            case PaymentType.CCP:
                                CCPayableCheck ccpcheck = GetCCPayableCheck(checkmodiReq.RefNbr);
                                switch (ccpcheck.Status)
                                {
                                    case CCList.CCStatus.PendingModify:
                                    case CCList.CCStatus.PendingRefund:
                                        ErrorMsg1.Add("CCP-" + ccpcheck.GuarPayableCD);
                                        break;
                                    case CCList.CCStatus.AccountReceived:
                                    case CCList.CCStatus.AppliedRTN:
                                    case CCList.CCStatus.Released:
                                    case CCList.CCStatus.Voided:
                                    case CCList.CCStatus.Returned:
                                        ErrorMsg2.Add("CCP-" + ccpcheck.GuarPayableCD);
                                        break;
                                }
                                break;
                            case PaymentType.CCR:
                                CCReceivableCheck ccrcheck = GetCCReceivableCheck(checkmodiReq.RefNbr);
                                switch (ccrcheck.Status)
                                {
                                    case CCList.CCStatus.PendingModify:
                                    case CCList.CCStatus.PendingRefund:
                                        ErrorMsg1.Add("CCR-" + ccrcheck.GuarReceviableCD);
                                        break;
                                    case CCList.CCStatus.AccountReceived:
                                    case CCList.CCStatus.AppliedRTN:
                                    case CCList.CCStatus.Released:
                                    case CCList.CCStatus.Voided:
                                    case CCList.CCStatus.Returned:
                                        ErrorMsg2.Add("CCR-" + ccrcheck.GuarReceviableCD);
                                        break;
                                }
                                break;
                        }
                        break;
                }
                    
            }
            string ErrMsg = "";
            if (ErrorMsg1.Count != 0 && ErrorMsg2.Count != 0)
            {
               ErrMsg = string.Join(",", ErrorMsg1.ToArray())+ Environment.NewLine+
                    "�H�W����/�q�� �ثe���b�s��Ϋݭק襤�A���pô�������f�T�{!" + Environment.NewLine+
                    string.Join(",", ErrorMsg2.ToArray()) +Environment.NewLine+
                    "�H�W���ڷ�e���A���i�ק�A�нT�{�çR���ӵ������᭫�s�e�f!";
                throw new Exception(ErrMsg);
            }
            else if(ErrorMsg1.Count != 0)
            {
                ErrMsg = string.Join(",", ErrorMsg1.ToArray()) + Environment.NewLine +
                    "�H�W����/�q�� �ثe���b�s��Ϋݭק襤�A���pô�������f�T�{!";
                throw new Exception(ErrMsg);
            }
            else if(ErrorMsg2.Count!=0)
            {
                ErrMsg = 
                    string.Join(",", ErrorMsg2.ToArray()) + Environment.NewLine +
                    "�H�W���ڷ�e���A���i�ק�A�нT�{�çR���ӵ������᭫�s�e�f!";
                throw new Exception(ErrMsg);
            }

            //Update Status
            foreach(EPPaymentModiReq modiReq in req)
            {               


                //Update Status
                switch(modiReq.ModifyType)
                {
                    case ModifyType.DAT:
                    case ModifyType.OTH:
                    case ModifyType.TIT:
                        //Status = PendingModify
                        switch(modiReq.PaymentType)
                        {
                            case PaymentType.NMP:
                                PXUpdate<Set<NMPayableCheck.status, NMStringList.NMAPCheckStatus.pendingmodify>,
                                    NMPayableCheck,
                                    Where<NMPayableCheck.payableCheckID, Equal<Required<EPPaymentModiReq.nmID>>>>
                                    .Update(Base, modiReq.NMID);                              
                                break;
                            case PaymentType.NMR:
                                PXUpdate<Set<NMReceivableCheck.status, NMStringList.NMARCheckStatus.pendingmodify>,
                                    NMReceivableCheck,
                                    Where<NMReceivableCheck.receivableCheckID, Equal<Required<EPPaymentModiReq.nmID>>>>
                                    .Update(Base, modiReq.NMID);                               
                                break;
                            case PaymentType.CCP:
                                PXUpdate<Set<CCPayableCheck.status, CCList.CCStatus.pendingModify>,
                                    CCPayableCheck,
                                    Where<CCPayableCheck.guarPayableCD, Equal<Required<EPPaymentModiReq.refNbr>>>>
                                    .Update(Base, modiReq.RefNbr);
                                break;
                            case PaymentType.CCR:
                                PXUpdate<Set<CCReceivableCheck.status, CCList.CCStatus.pendingModify>,
                                    CCReceivableCheck,
                                    Where<CCReceivableCheck.guarReceviableCD, Equal<Required<EPPaymentModiReq.refNbr>>>>
                                    .Update(Base, modiReq.RefNbr);
                                break;
                        }
                        break;
                    case ModifyType.REV:
                    case ModifyType.PMT:
                        //Status = PendingRefund
                        switch (modiReq.PaymentType)
                        {
                            case PaymentType.NMP:
                                PXUpdate<Set<NMPayableCheck.status, NMStringList.NMAPCheckStatus.pendingrefund>,
                                    NMPayableCheck,
                                    Where<NMPayableCheck.payableCheckID, Equal<Required<EPPaymentModiReq.nmID>>>>
                                    .Update(Base, modiReq.NMID);
                                break;
                            case PaymentType.NMR:
                                PXUpdate<Set<NMReceivableCheck.status, NMStringList.NMARCheckStatus.pendingrefund>,
                                    NMReceivableCheck,
                                    Where<NMReceivableCheck.receivableCheckID, Equal<Required<EPPaymentModiReq.nmID>>>>
                                    .Update(Base, modiReq.NMID);
                                break;
                            case PaymentType.CCP:
                                PXUpdate<Set<CCPayableCheck.status, CCList.CCStatus.pendingRefund>,
                                    CCPayableCheck,
                                    Where<CCPayableCheck.guarPayableCD, Equal<Required<EPPaymentModiReq.refNbr>>>>
                                    .Update(Base, modiReq.RefNbr);
                                break;
                            case PaymentType.CCR:
                                PXUpdate<Set<CCReceivableCheck.status, CCList.CCStatus.pendingRefund>,
                                    CCReceivableCheck,
                                    Where<CCReceivableCheck.guarReceviableCD, Equal<Required<EPPaymentModiReq.refNbr>>>>
                                    .Update(Base, modiReq.RefNbr);
                                break;
                        }
                        break;                  
                }
            }
        }
        #endregion

        #region Select Methods
        public POOrder GetPOOrder(string PONbr)
        {
            return PXSelect<POOrder,
                Where<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>
                .Select(Base, PONbr);
        }
        public BAccount2 GetBAccount2(int? BAccountID)
        {
            return PXSelect<BAccount2,
                Where<BAccount2.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, BAccountID);
        }
        private LocationExtAddress GetLocation(int? LocationID)
        {
            return PXSelect<LocationExtAddress,
                Where<LocationExtAddress.locationID, Equal<Required<LocationExtAddress.locationID>>>>
                .Select(Base, LocationID);
        }
        private NMBankAccount GetBankAccount(int? VCashAccountID,string Check)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID,Equal<NMBankAccount.paymentMethodID>>>>
                .Select(Base, VCashAccountID, Check);
        }
        private EPEmployee GetEmployee()
        {
            return PXSelect<EPEmployee,
                Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>
                .Select(Base);
        }
        private PXResultset<EPPaymentModiReq> getPaymentMofiReq(string EPRefNbr)
        {
            return PXSelect<EPPaymentModiReq,
                Where<EPPaymentModiReq.epRefNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                .Select(Base, EPRefNbr);
        }
        private NMPayableCheck GetNMPayableCheck(int? PayableCheckID)
        {
            return PXSelect<NMPayableCheck,
                Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>>
                .Select(Base, PayableCheckID);

        }
        private NMReceivableCheck GetNMReceivableCheck(int? ReceivableCheckID)
        {
            return PXSelect<NMReceivableCheck,
                Where<NMReceivableCheck.receivableCheckID, Equal<Required<NMReceivableCheck.receivableCheckID>>>>
                .Select(Base, ReceivableCheckID);
        }
        private CCPayableCheck GetCCPayableCheck(string PayableCheckCD)
        {
            return PXSelect<CCPayableCheck,
                Where<CCPayableCheck.guarPayableCD, Equal<Required<CCPayableCheck.guarPayableCD>>>>
                .Select(Base, PayableCheckCD);
        }
        private CCReceivableCheck GetCCReceivableCheck(string ReceivableCheckCD)
        {
            return PXSelect<CCReceivableCheck,
                Where<CCReceivableCheck.guarReceviableCD, Equal<Required<CCReceivableCheck.guarReceviableCD>>>>
                .Select(Base, ReceivableCheckCD);
        }
        //private NMBankAccount GetBankAccount(int? BankAccountID)
        //{
        //    return PXSelect<NMBankAccount,
        //        Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
        //        .Select(Base, BankAccountID);
        //}
        //private PXResultset<TWNWHTTran> GetTWWHTTran(string EPRefNbr)
        //{
        //    return PXSelect<TWNWHTTran,
        //        Where<TWNWHTTran.ePRefNbr,
        //        Equal<Required<EPExpenseClaim.refNbr>>>>.Select(Base, EPRefNbr);
        //}
        //private KGBillSummary GetBillSummary(string APRefNbr)
        //{
        //    return PXSelect<KGBillSummary,
        //        Where<KGBillSummary.refNbr, Equal<Required<EPExpenseClaimDetails.aPRefNbr>>>>
        //        .Select(Base, APRefNbr);
        //}
        #endregion
    }
}