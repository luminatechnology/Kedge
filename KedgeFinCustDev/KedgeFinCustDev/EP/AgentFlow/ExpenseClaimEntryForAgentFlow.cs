using EP.Util;
using Fin.DAC;
using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS.DAC;
using PX.Objects.EP;
using GL = PX.Objects.GL;
using PX.Objects.EP.AgentFlow.DAC;
using System;
using static EP.Util.EPStringList;
using static PX.Data.PXAccess;
using PX.Data.BQL.Fluent;
using System.Linq;
using PX.Data.BQL;
using System.Collections.Generic;

namespace KedgeFinCustDev.EP.AgentFlow
{
    public class ExpenseClaimEntryForAgentFlow : PXGraphExtension<ExpenseClaimEntryForWHT, ExpenseClaimEntry_Extension2, ExpenseClaimEntry>
    {
        #region Select Method
        public PXSelect<KGFlowFinBillingAH, Where<KGFlowFinBillingAH.afmNo, Equal<Current<EPExpenseClaim.refNbr>>>> FlowBillingAHs;
        public PXSelect<KGFlowFinBillingL, Where<KGFlowFinBillingL.headerID, Equal<Current<KGFlowFinBillingAH.headerID>>>> FlowBillingLs;
        public PXSelect<KGFlowFinBillingInv, Where<KGFlowFinBillingInv.headerID, Equal<Current<KGFlowFinBillingAH.headerID>>>> FlowBillingInvs;
        public PXSelect<KGFlowFinBillingNote, Where<KGFlowFinBillingNote.headerID, Equal<Current<KGFlowFinBillingAH.headerID>>>> FlowBillingNotes;
        public PXSelect<KGFlowFinBillingWht, Where<KGFlowFinBillingWht.headerID, Equal<Current<KGFlowFinBillingAH.headerID>>>> FlowBillingWhts;
        #endregion

        #region Override Action

        #region Submit
        int? UpCount = 0;
        public delegate void SubmitDelegate();
        [PXOverride]
        public void Submit(SubmitDelegate baseMethod)
        {
            EPExpenseClaim master = Base.ExpenseClaim.Current;
            EPExpenseClaimExt masterExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(master);

            if (master == null) baseMethod();
            else
            {
                if (UpCount == 0)
                {
                    if ((
                        masterExt.UsrDocType == EPStringList.EPDocType.STD//一般請款
                        || masterExt.UsrDocType == EPStringList.EPDocType.GUR//應付保證票
                        || masterExt.UsrDocType == EPStringList.EPDocType.RGU//退回應收保證票
                        || masterExt.UsrDocType == EPStringList.EPDocType.BTN//金融交易
                        )
                        && masterExt.UsrSkipFlowFlag != true)
                    {
                        checkanddeleteKGFlow();
                        createKGFlow();
                    }
                    /**
                    else if(masterExt.UsrDocType == EPStringList.EPDocType.STD && masterExt.UsrSkipFlowFlag == true)
                    {
                        masterExt.UsrApprovalStatus = EPUsrApprovalStatus.Approved;
                        Base.ExpenseClaim.Update(master);
                    }
                    **/
                    UpCount++;
                }
                baseMethod();
            }

        }
        #endregion

        #endregion

        #region Method

        /// <summary>
        /// Create KGFlowFinBillingAH/ L/ Inv/ Note to DB
        /// </summary>
        public void createKGFlow()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                createKGFlowFinBillingAH();
                createKGFlowFinBillingL();
                createKGFlowFinBillingInv();
                createKGFlowFinBillingNote();
                createKGFlowFinBillingWht();

                //EPExpenseClaim master = Base.ExpenseClaim.Current;
                //EPExpenseClaimExt masterExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(master);
                //if (masterExt.UsrSkipFlowFlag != true)
                //    masterExt.UsrApprovalStatus = EPUsrApprovalStatus.PendingApproval;
                //Base.ExpenseClaim.Update(master);
                Base.Persist();

                ts.Complete();
            }
        }
        public void createKGFlowFinBillingAH()
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            Branch branch = GetBranch(claim.BranchID);
            //PX.Objects.CR.BAccount companyBA = GetBAccount(branch?.BAccountID);
            Organization companyBA = GetCompanyBAccount(branch.BranchCD);
            //PX.Objects.CR.BAccount empolyeeBA = GetBAccount(claim.EmployeeID);
            PX.Objects.EP.EPEmployee empolyeeBA = GetEmpBAccount(claim.EmployeeID);


            KGFlowFinBillingAH billingAH = (KGFlowFinBillingAH)FlowBillingAHs.Cache.CreateInstance();
            billingAH.BillType = "A";//寫死管理
            billingAH.BranchID = branch?.BranchID;
            billingAH.Company = branch?.BranchCD;
            billingAH.CompanyName = companyBA?.OrganizationName;
            billingAH.BgtYear = (Convert.ToInt32(claimExt.UsrFinancialYear) - 1911).ToString();
            billingAH.AfmNo = claim.RefNbr;
            if (claimExt.UsrVendorID != null)
            {
                PX.Objects.CR.BAccount vendorBA = GetBAccount(claimExt.UsrVendorID);
                billingAH.VendorType = "V";
                billingAH.UniformNo = vendorBA?.AcctCD;
                billingAH.UniformName = vendorBA?.AcctName;
            }
            else
            {
                billingAH.VendorType = "E";
                billingAH.UniformNo = empolyeeBA?.AcctCD;
                billingAH.UniformName = empolyeeBA?.AcctName;
            }
            billingAH.AfmDate = claim.DocDate;
            billingAH.Employee = empolyeeBA?.AcctName;
            billingAH.Enable = ""; //待提供
            billingAH.SumAmount = claim.CuryDocBal;
            billingAH.TaxExAmt = claim.CuryVatTaxableTotal + claim.CuryVatExemptTotal;
            //billingAH.TaxAmt = claim.CuryVatExemptTotal;
            billingAH.TaxAmt = claim.CuryTaxTotal;
            billingAH.Tax2AmtS = claimExt.UsrWhtAmt;
            billingAH.Nhi2AmtS = claimExt.UsrGnhi2Amt;
            billingAH.HliAmt = claimExt.UsrHliAmt;
            billingAH.LbiAmt = claimExt.UsrLbiAmt;
            billingAH.LbpAmt = claimExt.UsrLbpAmt;
            if (claimExt.UsrSkipFlowFlag == true)
                billingAH.SkipFlowFlag = "Y";
            else
                billingAH.SkipFlowFlag = "N";

            FlowBillingAHs.Insert(billingAH);
        }
        public void createKGFlowFinBillingL()
        {
            int? LineNbr = 0;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            foreach (EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Select())
            {
                EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
                if ("B".Equals(detailsExt.UsrValuationType))
                {
                    LineNbr++;
                    KGFlowFinBillingL billingL = (KGFlowFinBillingL)FlowBillingLs.Cache.CreateInstance();
                    Branch branch = GetBranch(details.BranchID);
                    KGBudApproveLevel approveLevel = GetKGBudApproveLevel(detailsExt.UsrApprovalLevelID);
                    KGBudGroup budGroup = GetKGBudGroup(approveLevel?.GroupNo, approveLevel?.BranchID);
                    Note note = GetNote(details.NoteID);
                    billingL.BillType = "A"; //寫死管理
                    billingL.BranchID = branch?.BranchID;
                    billingL.LineID = LineNbr;
                    billingL.Company = branch?.BranchCD;
                    //billingL.BillType = approveLevel?.BillingType;
                    billingL.GroupNo = approveLevel?.GroupNo;
                    billingL.GroupName = budGroup?.BudGroupName;
                    billingL.TaxKind = getVoucherType(detailsExt.UsrVoucherType);
                    if (claimExt.UsrVendorID != null)
                    {
                        PX.Objects.CR.BAccount vendorBA = GetBAccount(claimExt.UsrVendorID);
                        Location vendorLC = GetLocation(vendorBA?.DefLocationID);
                        billingL.VendorType = "V";
                        billingL.UniformNo = vendorLC?.TaxRegistrationID ?? null;
                        billingL.UniformName = vendorBA?.AcctName ?? null;
                    }
                    else
                    {
                        PX.Objects.EP.EPEmployee employeeBA = GetEmpBAccount(claim.EmployeeID);
                        VendorPaymentMethodDetail employeeVD = GetEmpolyeeTaxRegisteration(employeeBA?.DefLocationID);
                        billingL.VendorType = "E";
                        billingL.UniformNo = employeeVD?.DetailValue;
                        billingL.UniformName = employeeBA?.AcctName;
                    }
                    //billingL.UniformName = ""; //待提供
                    billingL.BelongMClass = ""; //待提供
                    billingL.BelongMClassName = ""; //待提供
                    billingL.BelongSClass = ""; //待提供
                    billingL.BelongSClassName = ""; //待提供
                    billingL.Amount = details.CuryExtCost;
                    billingL.TaxAmount = details.CuryTaxTotal;
                    billingL.TotalAmount = details.CuryTranAmtWithTaxes;
                    billingL.DateDue = null; //目前在付款方式中, 需跟會計確認
                    billingL.Digest = note?.NoteText ?? null;
                    billingL.AfmNo = details.RefNbr;
                    billingL.AfmDate = details.ExpenseDate;
                    billingL.ApprovalLevelID = detailsExt.UsrApprovalLevelID;

                    #region 預算金額 & 累計執行數
                    if (claimExt.UsrDocType == EPStringList.EPDocType.GUR//應付保證票
                    || claimExt.UsrDocType == EPStringList.EPDocType.RGU//退回應收保證票
                    || claimExt.UsrDocType == EPStringList.EPDocType.BTN//金融交易
                      )
                    {
                        //預算金額
                        decimal budget = GetBudgetByFinYear(details.BranchID, claimExt.UsrFinancialYear, details.ExpenseAccountID, details.ExpenseSubID);
                        var usedSum = GetUsed(details.BranchID, claimExt.UsrFinancialYear, details.ExpenseAccountID, details.ExpenseSubID);
                        decimal used = (usedSum?.CreditAmt ?? 0m) - (usedSum?.DebitAmt ?? 0m);
                        billingL.BudgetAmount = budget;//預算金額
                        billingL.UseAmount = used;//累計執行數
                    }
                    #endregion

                    FlowBillingLs.Insert(billingL);
                }
            }
        }
        public void createKGFlowFinBillingInv()
        {
            int? LineNbr = 0;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            foreach (GVApGuiInvoiceFinRef inv in Base1.GVApGuiInvoiceRefs.Select())
            {
                LineNbr++;
                KGFlowFinBillingInv billingInv = (KGFlowFinBillingInv)FlowBillingInvs.Cache.CreateInstance();
                billingInv.BranchID = claim.BranchID;
                billingInv.InvoiceID = inv.GuiInvoiceID;
                billingInv.LineID = LineNbr;
                billingInv.BillType = "A";
                billingInv.ItemNo = "";//Louis要再想一下
                billingInv.InvoiceNo = inv.GuiInvoiceNbr;
                billingInv.InvoiceDate = inv.InvoiceDate;
                billingInv.TaxCode = inv.TaxCode;
                switch (inv.GuiType)
                {
                    case "21":
                        billingInv.IvoKind = "1";
                        break;
                    case "22":
                        billingInv.IvoKind = "3";
                        break;
                    case "25":
                        billingInv.IvoKind = "2";
                        break;
                    case "26":
                        billingInv.IvoKind = "1";
                        break;
                    case "27":
                        billingInv.IvoKind = "3";
                        break;
                    default:
                        billingInv.IvoKind = "0";
                        break;
                }
                //billingInv.IvoKind = inv.InvoiceType;
                billingInv.NetAmount = inv.SalesAmt;
                billingInv.TaxAmount = inv.TaxAmt;
                billingInv.Amount = inv.SalesAmt + inv.TaxAmt;
                billingInv.UniformNo = inv.VendorUniformNumber;
                billingInv.UniformName = inv.VendorName;
                billingInv.Digest = ""; //Louis要再想一下
                billingInv.TaxableExpense = inv.SalesAmt;

                FlowBillingInvs.Insert(billingInv);

            }
        }
        public void createKGFlowFinBillingNote()
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            foreach (KGBillPayment billPayment in Base1.KGBillPayments.Select())
            {
                KGFlowFinBillingNote billingNote = (KGFlowFinBillingNote)FlowBillingNotes.Cache.CreateInstance();
                billingNote.BillType = "A"; //寫死管理
                billingNote.BranchID = claim.BranchID;
                billingNote.BillingNoteID = billPayment.BillPaymentID;
                //billingNote.BillType = "";//BillPayment沒有相關欄位
                if (claimExt.UsrVendorID != null)
                {
                    PX.Objects.CR.BAccount vendorBA = GetBAccount(claimExt.UsrVendorID);
                    billingNote.VendorType = "V";
                    billingNote.UniformNo = vendorBA?.AcctCD;
                    billingNote.UniformName = vendorBA?.AcctName;
                }
                else
                {
                    PX.Objects.EP.EPEmployee employeeBA = GetEmpBAccount(claim.EmployeeID);
                    billingNote.VendorType = "E";
                    billingNote.UniformNo = employeeBA?.AcctCD;
                    billingNote.UniformName = employeeBA?.AcctName;
                }
                billingNote.NetAmount = billPayment.ActPayAmt;
                billingNote.DateDue = billPayment.PaymentDate;
                billingNote.PostageAmt = billPayment.PostageAmt;
                billingNote.Remark = billPayment.Remark;

                FlowBillingNotes.Insert(billingNote);
            }
        }
        public void createKGFlowFinBillingWht()
        {
            int? LineNbr = 0;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            foreach (TWNWHTTran whtTrans in Base2.WHTView.Select())
            {
                LineNbr++;
                KGFlowFinBillingWht billingWht = (KGFlowFinBillingWht)FlowBillingWhts.Cache.CreateInstance();
                Branch branch = GetBranch(claim.BranchID);
                billingWht.BranchID = branch?.BranchID;
                billingWht.WhtID = whtTrans.WHTTranID;
                billingWht.PersonalID = whtTrans.PersonalID;
                billingWht.PayeeName = whtTrans.PayeeName;

                billingWht.EntryYy = (whtTrans.PaymentDate.Value.Year - 1911).ToString();
                billingWht.EntryMm = int.Parse(whtTrans.PaymentDate.Value.Month.ToString()).ToString("00");
                billingWht.Whtfmtcode = whtTrans.WHTFmtCode;
                billingWht.DuTypeCode = whtTrans.DuTypeCode;
                billingWht.Tax2Acct = 0;
                billingWht.Whtamt = whtTrans.WHTAmt;
                billingWht.Gnhi2amt = whtTrans.GNHI2Amt;
                billingWht.HouseTaxIdType = "";
                FlowBillingWhts.Insert(billingWht);

            }
        }

        /// <summary>
        /// Check KGFlowBillingAH wheater Exist 
        /// if Yes, Delete KGFlowBillingAH/L/Inv/Note
        /// </summary>
        public void checkanddeleteKGFlow()
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            KGFlowFinBillingAH billingAH = GetFinBillingAH(claim.RefNbr);
            if (billingAH == null) return;
            FlowBillingAHs.Delete(billingAH);
            foreach (KGFlowFinBillingL billingL in FlowBillingLs.Select())
            {
                FlowBillingLs.Delete(billingL);
            }
            foreach (KGFlowFinBillingInv billingInv in FlowBillingInvs.Select())
            {
                FlowBillingInvs.Delete(billingInv);
            }
            foreach (KGFlowFinBillingNote billingNote in FlowBillingNotes.Select())
            {
                FlowBillingNotes.Delete(billingNote);
            }
            Base.Persist();
        }

        /// <summary>
        /// Transfer UsrVoucherType to Cons VoucherType
        /// </summary>
        /// <param name="UsrVoucherType">
        /// Acumatica VoucherType (Our define)</param>
        /// <returns>VoucherType (Cons Define)</returns>
        private string getVoucherType(string UsrVoucherType)
        {
            string VoucherType = "";
            switch (UsrVoucherType)
            {
                case EPStringList.EPVoucherType.A:
                    VoucherType = "N";
                    break;
                case EPStringList.EPVoucherType.G:
                    VoucherType = "I";
                    break;
                case EPStringList.EPVoucherType.S:
                    VoucherType = "S";
                    break;
            }
            return VoucherType;
        }
        #endregion

        #region Select Method
        private Branch GetBranch(int? BranchID)
        {
            return PXSelect<Branch,
                Where<Branch.branchID, Equal<Required<Branch.branchID>>>>
                .Select(Base, BranchID);
        }

        private Organization GetCompanyBAccount(string BranchCD)
        {
            return PXSelect<Organization,
                Where<Organization.organizationCD, Equal<Required<Organization.organizationCD>>>>
                .Select(Base, BranchCD);
        }

        private PX.Objects.EP.EPEmployee GetEmpBAccount(int? BAccountID)
        {
            return PXSelect<PX.Objects.EP.EPEmployee,
                Where<PX.Objects.EP.EPEmployee.bAccountID, Equal<Required<PX.Objects.EP.EPEmployee.bAccountID>>>>
                .Select(Base, BAccountID);
        }
        private PX.Objects.CR.BAccount GetBAccount(int? BAccountID)
        {
            return PXSelect<PX.Objects.CR.BAccount,
                Where<PX.Objects.CR.BAccount.bAccountID, Equal<Required<PX.Objects.CR.BAccount.bAccountID>>>>
                .Select(Base, BAccountID);
        }
        private KGBudApproveLevel GetKGBudApproveLevel(int? budLevelID)
        {
            return PXSelect<KGBudApproveLevel,
                Where<KGBudApproveLevel.budLevelID, Equal<Required<KGBudApproveLevel.budLevelID>>>>
                .Select(Base, budLevelID);
        }
        private KGBudGroup GetKGBudGroup(int? GroupNo, int? BranchID)
        {
            return PXSelect<KGBudGroup,
                Where<KGBudGroup.branchID, Equal<Required<KGBudGroup.branchID>>,
                And<KGBudGroup.budGroupNO, Equal<Required<KGBudGroup.budGroupNO>>>>>
                .Select(Base, BranchID, GroupNo);
        }
        private Note GetNote(Guid? NoteID)
        {
            return PXSelect<Note,
                Where<Note.noteID, Equal<Required<Note.noteID>>>>
                .Select(Base, NoteID);
        }
        private Location GetLocation(int? LocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, LocationID);
        }
        public VendorPaymentMethodDetail GetEmpolyeeTaxRegisteration(int? LocationID)
        {
            return PXSelect<VendorPaymentMethodDetail,
                Where<VendorPaymentMethodDetail.locationID, Equal<Required<VendorPaymentMethodDetail.locationID>>,
                And<VendorPaymentMethodDetail.detailID, Equal<Required<VendorPaymentMethodDetail.detailID>>>>>
                .Select(Base, LocationID, "CATEGORYID");
        }
        public KGFlowFinBillingAH GetFinBillingAH(string RefNbr)
        {
            return PXSelect<KGFlowFinBillingAH,
                Where<KGFlowFinBillingAH.afmNo, Equal<Required<EPExpenseClaim.refNbr>>>>
                .Select(Base, RefNbr);
        }

        /// <summary>
        /// 取得年度預算
        /// </summary>
        /// <param name="branchID"></param>
        /// <param name="finYear"></param>
        /// <param name="accountID"></param>
        /// <param name="subID"></param>
        /// <returns></returns>
        public decimal GetBudgetByFinYear(int? branchID, string finYear, int? accountID, int? subID)
        {
            GL.Ledger budgetLedger = GL.Ledger.UK.Find(Base, "BUDGET");
            return SelectFrom<GL.GLBudgetLine>
                .Where<GL.GLBudgetLine.released.IsEqual<True>
                .And<GL.GLBudgetLine.branchID.IsEqual<@P.AsInt>
                .And<GL.GLBudgetLine.ledgerID.IsEqual<@P.AsInt>
                .And<GL.GLBudgetLine.finYear.IsEqual<@P.AsString>
                .And<GL.GLBudgetLine.accountID.IsEqual<@P.AsInt>
                .And<GL.GLBudgetLine.subID.IsEqual<@P.AsInt>>>>>>>
                .AggregateTo<Sum<GL.GLBudgetLine.amount>>
                .View.Select(Base, branchID, budgetLedger?.LedgerID, finYear, accountID, subID)
                .RowCast<GL.GLBudgetLine>()?.First()?.Amount ?? 0m;
        }

        /// <summary>
        /// 取得累計使用
        /// </summary>
        /// <param name="branchID"></param>
        /// <param name="finYear"></param>
        /// <param name="accountID"></param>
        /// <param name="subID"></param>
        /// <returns></returns>
        public GL.GLTran GetUsed(int? branchID, string finYear, int? accountID, int? subID)
        {
            GL.Ledger budgetLedger = GL.Ledger.UK.Find(Base, "ACTUAL");
            return SelectFrom<GL.GLTran>
                 .InnerJoin<GL.Batch>
                    .On<GL.GLTran.batchNbr.IsEqual<GL.Batch.batchNbr>>
                 .Where<GL.Batch.branchID.IsEqual<@P.AsInt>
                 .And<GL.GLTran.ledgerID.IsEqual<@P.AsInt>
                 .And<GL.Batch.finPeriodID.IsLike<@P.AsString>
                 .And<GL.GLTran.accountID.IsEqual<@P.AsInt>
                 .And<GL.GLTran.subID.IsEqual<@P.AsInt>>>>>>
                 .AggregateTo<
                     Sum<GL.GLTran.creditAmt>,
                     Sum<GL.GLTran.debitAmt>>
                 .View.Select(Base, branchID, budgetLedger?.LedgerID, $"{finYear}%", accountID, subID)
                 .RowCast<GL.GLTran>()?.First();
        }
        #endregion
    }
}
