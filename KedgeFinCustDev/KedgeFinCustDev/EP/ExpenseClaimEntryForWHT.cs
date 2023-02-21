using EP.DAC;
using EP.Util;
using Fin.DAC;
using FIN.Util;
using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.PM;
using System;
using System.Linq;

namespace PX.Objects.EP
{
    /**
     * ===2021/05/13 :口頭 ===Althea
     * Add PXImport Button
     * 
     * ===2021/05/13 :Tiffany 口頭===Althea
     * Add 若文件類型不為STD檢查是否有代扣稅資料,有則全部刪除
     * 
     * ===2021/06/08 :Tiffany 口頭====Althea
     * Modify Log:
     * 代扣稅Tab塞資料到EPDetails原本在一新增就做改為存檔再做
     * 因為複製會出現EPDetails 重複兩筆
     * 
     * ===2021/06/09 :口頭===Althea
     *  因為代扣稅改做在存檔,系統先驗證KGBillPayment再去塞代扣資料到EPDetails,導致金額異動
     *  所以把KGBillPayment檢核改到WHT的graph 先異動完再檢核
     *  
     * ====2021-07-09:12142====Alton
     * 同AP301000 不檢查 PaymentPct加總必須要100, 只檢核PaymentAmount加總要等於KGBillSummary的TotalAmt
     * 
     * ===2021/09/07 :0012225 ===Althea
     *  Add TWNWHTTran BranchID Default = EPClaim.BranchID
     *  
     * ===2021/09/10 :0012226 === Althea
     * Add When Update DocDate ,TWNWHTTran.PaymentDate = Current(EPClaim.DocDate)
     * Mofidy WHTTran.PaymentDate =current DocDate, Enable = false ,viisible = false
     * Add WHTTran.TranDate Default = BusinessDate, Enable = true ,viisible = true
     * 
     * 	===2021/09/23 :0012239 === Althea
     * 	代扣稅WHTAmt金額為0也要加到epDetails
     **/
    public class ExpenseClaimEntryForWHT : PXGraphExtension<ExpenseClaimEntry_Extension2, ExpenseClaimEntry>
    {
        #region Select
        //2021/03/10 代扣稅Add By Althea 
        //2021/03/25 Modify DAC TWNWHT -> TWNWHTTran
        //2021/05/13 Add Import
        [PXImport]
        public PXSelect<TWNWHTTran,
            Where<TWNWHTTran.ePRefNbr, Equal<Current<EPExpenseClaim.refNbr>>>> WHTView;
        
        [PXCopyPasteHiddenView]
        public PXSelect<EPExpenseClaim,
            Where<EPExpenseClaim.refNbr, Equal<Current<EPExpenseClaim.refNbr>>>> WHTHeader;
        #endregion

        #region Override
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                //刪除 連動的TWNWHTTran
                foreach (EPExpenseClaim claim in Base.ExpenseClaim.Cache.Deleted)
                {
                    foreach (TWNWHTTran tWNWHTTran in WHTView.Select())
                    {
                        WHTView.Delete(tWNWHTTran);
                    }
                }
                //CreateDetailsForWHT(Base.ExpenseClaim.Current);
                if (this.WHTView.Cache.Inserted.RowCast<TWNWHTTran>().FirstOrDefault()?.NoteID != null 
                    || this.WHTView.Cache.Updated.RowCast<TWNWHTTran>().FirstOrDefault()?.NoteID != null)
                {
                    CreateDetailsForWHT(Base.ExpenseClaim.Current);
                }

                baseMethod();

                ts.Complete();
            }

        }
        #endregion

        #region Events
        protected void _(Events.RowSelected<EPExpenseClaim> e)
        {
            EPExpenseClaim row = e.Row as EPExpenseClaim;
            setEnabledUI();
        }
        protected void _(Events.FieldUpdated<EPExpenseClaim,EPExpenseClaimExt.usrDocType> e)
        {
            EPExpenseClaim row = e.Row;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);
            if(rowExt.UsrDocType != EPStringList.EPDocType.STD)
            {
                TWNWHTTran whttran = WHTView.Select();
                if(whttran != null)
                {
                    foreach(TWNWHTTran tran in WHTView.Select())
                    {
                        WHTView.Delete(tran);
                    }
                }
            }
        }
        protected void _(Events.FieldUpdated<EPExpenseClaim, EPExpenseClaim.docDate> e)
        {
            EPExpenseClaim row = e.Row;
            if (row == null) return;
            foreach(TWNWHTTran tran in WHTView.Select())
            {
                tran.PaymentDate = row.DocDate;
                WHTView.Update(tran);
            }
        }
        //protected void _(Events.RowPersisting<EPExpenseClaim> e)
        protected void CreateDetailsForWHT(EPExpenseClaim row)
        {
            //EPExpenseClaim row = e.Row;
            if (row == null) return;
            EPExpenseClaimExt rowExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);
            decimal UsrWhtAmt = rowExt.UsrWhtAmt ?? 0;
            decimal UsrGnhi2Amt = rowExt.UsrGnhi2Amt ?? 0;
            decimal UsrLbiAmt = rowExt.UsrLbiAmt ?? 0;
            decimal UsrLbpAmt = rowExt.UsrLbpAmt ?? 0;
            decimal UsrHliAmt = rowExt.UsrHliAmt ?? 0;
            //2021/09/23 Mantis: 0012239 金額是0也要塞到APTran
            //檢查TWNWHTTran是否存在各類型的資料
            TWNWHTTran exsitWHTTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.WHT);        
            TWNWHTTran exsitLbiTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.LBI);
            TWNWHTTran exsitLbpTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.LBP);
            TWNWHTTran exsitHliTran = GetWHTTranofTranType(row.RefNbr, FinStringList.TranType.HLI);
           
            EPExpenseClaimDetails oldWHTTran = GetEPDetailsofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.WHT));
            EPExpenseClaimDetails oldGnhi2Tran = GetEPDetailsofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.GHI));
            EPExpenseClaimDetails oldLbiTran = GetEPDetailsofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.LBI));
            EPExpenseClaimDetails oldLbpTran = GetEPDetailsofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.LBP));
            EPExpenseClaimDetails oldHliTran = GetEPDetailsofValuationType(row.RefNbr, ValuationType(FinStringList.TranType.HLI));
            if (exsitWHTTran != null)
            {                
                if (oldWHTTran != null)                
                    UpdateEPDetailAmt(FinStringList.TranType.WHT, UsrWhtAmt);             
                else
                    CreateEPDetailFromWHTTab(FinStringList.TranType.WHT, UsrWhtAmt);
            }
            else
            {
                if (oldWHTTran != null)           
                    Base.ExpenseClaimDetails.Delete(oldWHTTran);                
            }
            if (exsitWHTTran != null)
            {     
                if (oldGnhi2Tran != null)
                    UpdateEPDetailAmt(FinStringList.TranType.GHI, UsrGnhi2Amt);
                else
                    CreateEPDetailFromWHTTab(FinStringList.TranType.GHI, UsrGnhi2Amt);
            }
            else
            {
                if (oldGnhi2Tran != null)
                    Base.ExpenseClaimDetails.Delete(oldGnhi2Tran);
            }
            if (exsitLbiTran != null)
            {
                if (oldLbiTran != null)
                    UpdateEPDetailAmt(FinStringList.TranType.LBI, UsrLbiAmt);
                else
                    CreateEPDetailFromWHTTab(FinStringList.TranType.LBI, UsrLbiAmt);
            }
            else
            {
                if (oldLbiTran != null)
                    Base.ExpenseClaimDetails.Delete(oldLbiTran);
            }
            if (exsitLbpTran != null)
            {
                if (oldLbpTran != null)
                    UpdateEPDetailAmt(FinStringList.TranType.LBP, UsrLbpAmt);
                else
                    CreateEPDetailFromWHTTab(FinStringList.TranType.LBP, UsrLbpAmt);
            }
            else
            {
                if (oldLbpTran != null)
                    Base.ExpenseClaimDetails.Delete(oldLbpTran);
            }
            if (exsitHliTran != null)
            {
                if (oldHliTran != null)
                    UpdateEPDetailAmt(FinStringList.TranType.HLI, UsrHliAmt);
                else
                    CreateEPDetailFromWHTTab(FinStringList.TranType.HLI, UsrHliAmt);
            }
            else
            {
                if (oldHliTran != null)
                    Base.ExpenseClaimDetails.Delete(oldHliTran);
            }

            //2021/10/06 Delete Louis said edit by althea
            /*
            foreach(EPExpenseClaimDetails details in Base.ExpenseClaimDetails.Cache.Cached)
            {
                EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);
                KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
                if (detailsExt.UsrValuationType == ValuationTypeStringList.B &&
                    (details.InventoryID == setUp.HealthInsInventoryID ||
                    details.InventoryID == setUp.KGIndividualTaxInventoryID ||
                    details.InventoryID == setUp.KGSupplementaryTaxInventoryID ||
                    details.InventoryID == setUp.LaborInsInventoryID ||
                    details.InventoryID == setUp.PensionInventoryID))
                {
                    Base.ExpenseClaimDetails.Delete(details);
                }
            }
            */
            //EPExpenseClaim master = Base.ExpenseClaim.Current;
            EPExpenseClaimExt masterExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(row);
            if (masterExt.UsrDocType == EPStringList.EPDocType.STD)
            {
                KGBillPayment billPayment = Base1.KGBillPayments.Current;
                //KGBillPayment Check
                if (billPayment != null)
                {
                    decimal? PaymentPct = 0;
                    //2021/02/09 Add Mantis:0011941
                    decimal? PaymentAmount = 0;
                    foreach (KGBillPayment epBillPayment in Base1.KGBillPayments.Select())
                    {
                        PaymentPct = epBillPayment.PaymentPct + PaymentPct;
                        PaymentAmount = PaymentAmount + epBillPayment.PaymentAmount;
                    }
                    //20210709 mark by alton:12142 
                    //if (PaymentPct != 100)
                    //    Base1.KGBillPayments.Cache.RaiseExceptionHandling<KGBillPayment.paymentPct>
                    //        (billPayment, billPayment.PaymentPct, new Exception("請確認付款比率為100%!"));
                    //throw new Exception("請確認付款比率為100%!");

                    //20220418 by louis nodify by decimal compare error
                    if (Convert.ToDecimal(PaymentAmount) != Convert.ToDecimal(row.CuryDocBal))
                    {
                        Base1.KGBillPayments.Cache.RaiseExceptionHandling<KGBillPayment.paymentAmount>
                                                                         (billPayment, billPayment.PaymentAmount, 
                                                                          new PXSetPropertyException("請確認付款金額與申請金額相符!"));
                        throw new PXException("請確認付款金額與申請金額相符!");
                    }
                }
            }
        }
        protected void _(Events.RowDeleting<EPExpenseClaimDetails> e)
        {
            EPExpenseClaimDetails row = e.Row ;
            EPExpenseClaimDetailsExt rowExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(row);
            EPExpenseClaim master = Base.ExpenseClaim.Current;
            if (row == null) return;
            if (Base.ExpenseClaim.Cache.GetStatus(master) != PXEntryStatus.Deleted)
            {
                //若型態為代扣稅類型,金額為0才可以刪除
                if (rowExt.UsrValuationType == ValuationTypeStringList.WITH_TAX || //20230103 mantis:12386 w -> WITH_TAX
                    rowExt.UsrValuationType == ValuationTypeStringList.H ||
                    rowExt.UsrValuationType == ValuationTypeStringList.L ||
                    rowExt.UsrValuationType == ValuationTypeStringList.N ||
                    rowExt.UsrValuationType == ValuationTypeStringList.SECOND)//20230103 mantis:12386 S -> SECOND
                {
                    if (row.CuryExtCost != 0) throw new Exception("此資料不可刪除!");
                }
            }

        }

        #region TWNWHTTran
        protected virtual void _(Events.RowSelected<TWNWHTTran> e)
        {
            TWNWHTTran row = e.Row as TWNWHTTran;
            if (row == null) return;
            setWHTTranUI(row);
        }
        protected virtual void _(Events.RowDeleted<TWNWHTTran> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            EPExpenseClaimDetails details = GetEPDetailsofValuationType(claim.RefNbr, ValuationType(row.TranType));
            //if (details == null) return;

            switch (row.TranType)
            {
                case FinStringList.TranType.WHT:
                    claimExt.UsrWhtAmt = (claimExt.UsrWhtAmt ?? 0) - row.WHTAmt;
                    if (details != null)
                        Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, claimExt.UsrWhtAmt);
                    if (row.GNHI2Amt != null)
                    {
                        claimExt.UsrGnhi2Amt = (claimExt.UsrGnhi2Amt ?? 0) - row.GNHI2Amt;

                        EPExpenseClaimDetails detailsGNHI2 = GetEPDetailsofValuationType(claim.RefNbr, ValuationType(FinStringList.TranType.GHI));
                        Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(detailsGNHI2, claimExt.UsrGnhi2Amt);
                        if (claimExt.UsrGnhi2Amt == 0)
                        {
                            Base.ExpenseClaimDetails.Cache.Update(detailsGNHI2);
                            Base.ExpenseClaimDetails.Cache.Delete(detailsGNHI2);

                        }
                    }
                    break;
                case FinStringList.TranType.LBI:
                    claimExt.UsrLbiAmt = (claimExt.UsrLbiAmt ?? 0) - row.WHTAmt;
                    if (details != null)
                        Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, claimExt.UsrLbiAmt);
                    break;
                case FinStringList.TranType.HLI:
                    claimExt.UsrHliAmt = (claimExt.UsrHliAmt ?? 0) - row.WHTAmt;
                    if (details != null)
                        Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, claimExt.UsrHliAmt);
                    break;
                case FinStringList.TranType.LBP:
                    claimExt.UsrLbpAmt = (claimExt.UsrLbpAmt ?? 0) - row.WHTAmt;
                    if (details != null)
                        Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, claimExt.UsrLbpAmt);
                    break;
            }
            if (details != null)
            {
                Base.ExpenseClaimDetails.Cache.Update(details);
                if (details?.CuryUnitCost == 0)
                {
                    Base.ExpenseClaimDetails.Cache.Delete(details);
                }
            }

        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran.tranType> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            if (claim == null) return;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            //處裡舊資料 EPExpenseClaimDetails刪除或更新金額
            //若變更TranType,清空有些WHT才需填的欄位

            if ((string)e.OldValue != null)
            {
                decimal? OldAmt = 0m;
                switch ((string)e.OldValue)
                {
                    case FinStringList.TranType.WHT:
                        claimExt.UsrWhtAmt = (claimExt.UsrWhtAmt ?? 0) - row.WHTAmt;
                        OldAmt = claimExt.UsrWhtAmt;
                        break;
                    case FinStringList.TranType.LBI:
                        claimExt.UsrLbiAmt = (claimExt.UsrLbiAmt ?? 0) - row.WHTAmt;
                        OldAmt = claimExt.UsrLbiAmt;
                        break;
                    case FinStringList.TranType.HLI:
                        claimExt.UsrHliAmt = (claimExt.UsrHliAmt ?? 0) - row.WHTAmt;
                        OldAmt = claimExt.UsrHliAmt;
                        break;
                    case FinStringList.TranType.LBP:
                        claimExt.UsrLbpAmt = (claimExt.UsrLbpAmt ?? 0) - row.WHTAmt;
                        OldAmt = claimExt.UsrLbpAmt;
                        break;
                }
                WHTView.Cache.SetValueExt<TWNWHTTran.gNHI2Amt>(row, 0m);
                //2021/06/08 Take off Move to Persist
                /*
                EPExpenseClaimDetails oldtran = GetEPDetailsofValuationType(claim.RefNbr, ValuationType((string)e.OldValue));
                UpdateEPDetailAmt((string)e.OldValue, OldAmt);
                if (oldtran != null && OldAmt == 0)
                    Base.ExpenseClaimDetails.Cache.Delete(oldtran);
                WHTView.Cache.SetValueExt<TWNWHTTran.gNHI2Amt>(row, 0m);
                EPExpenseClaimDetails oldGNHI2tran = GetEPDetailsofValuationType(claim.RefNbr, ValuationType(FinStringList.TranType.GHI));
                if (oldGNHI2tran != null && oldGNHI2tran.CuryUnitCost == 0)
                    Base.ExpenseClaimDetails.Cache.Delete(oldGNHI2tran);
                */
            }

            // 處理新的TranType EPExpenseClaimDetails新增或更新金額
            //先檢查EPExpenseClaimDetails是否有相同valuationType的資料,若為null 則新增一筆,若有資料則update amt
            decimal? Amt = 0;
            switch (row.TranType)
            {
                case FinStringList.TranType.WHT:
                    Amt = (claimExt.UsrWhtAmt ?? 0);
                    break;
                case FinStringList.TranType.LBI:
                    Amt = (claimExt.UsrLbiAmt ?? 0);
                    break;
                case FinStringList.TranType.HLI:
                    Amt = (claimExt.UsrHliAmt ?? 0);
                    break;
                case FinStringList.TranType.LBP:
                    Amt = (claimExt.UsrLbpAmt ?? 0);
                    break;
            }
            //2021/06/08 Take off Move to Persist
            /*
            EPExpenseClaimDetails details = GetEPDetailsofValuationType(claim.RefNbr, ValuationType(row.TranType));
            if (details == null)
                CreateEPDetailFromWHTTab(row.TranType, Amt);
            else
                UpdateEPDetailAmt(row.TranType, Amt);
            if (row.TranType == FinStringList.TranType.WHT && row.GNHI2Amt != null)
            {
                EPExpenseClaimDetails GHI2tran = GetEPDetailsofValuationType(claim.RefNbr, ValuationType(FinStringList.TranType.WHT));
                if (GHI2tran == null)
                    CreateEPDetailFromWHTTab(row.TranType, claimExt.UsrGnhi2Amt);
                else
                    UpdateEPDetailAmt(FinStringList.TranType.WHT, claimExt.UsrGnhi2Amt);
            }
            */

            //Default
            if (row.TranType == FinStringList.TranType.WHT)
            {
                //2021/05/12 Add
                /**
                if(claimExt.UsrVendorID == null && claim.EmployeeID==null)
                {
                    throw new Exception("請先填員工或供應商!");
                }
                BAccount2 bAccount = GetBAccount(claimExt.UsrVendorID?? claim.EmployeeID);
                if (claimExt.UsrVendorID==null)
                {
                    row.PayeeName = bAccount.AcctName;
                    VendorPaymentMethodDetail vendorPaymentMethodDetai = GetVendorPaymentMethodDetail(claim.EmployeeID);
                    row.PersonalID = vendorPaymentMethodDetai?.DetailValue ?? "";
                    Address address = GetAddress(bAccount.DefAddressID);
                    row.PayeeAddr = address?.City ?? "" + address?.AddressLine1 ?? "" + address?.AddressLine2 ?? "";
                }
                else
                {
                    Vendor vendor = GetVendor(claimExt.UsrVendorID);
                    VendorExt vendorExt = PXCache<Vendor>.GetExtension<VendorExt>(vendor);
                    Contact contact = GetContact(vendor?.DefContactID);
                    row.PayeeName = contact?.Attention ?? "";
                    row.PersonalID = vendorExt?.UsrPersonalID ?? "";
                    Address address = GetAddress(vendor?.DefAddressID);
                    row.PayeeAddr = address?.City ?? "" + address?.AddressLine1 ?? "" + address?.AddressLine2 ?? "";
                }**/

                row.TypeOfIn = "1";
                row.WHTFmtCode = "50";
                row.WHTFmtSub = "0";
                row.PaymentDate = claim.DocDate;//Base.Accessinfo.BusinessDate;
                row.TranDate = Base.Accessinfo.BusinessDate;
                row.Jurisdiction = "TW";
                row.TaxationAgreements = "0";
            }
            else
            {
                row.PersonalID = null;
                row.PayeeName = null;
                row.PayeeAddr = null;
                row.PropertyID = null;
                row.TypeOfIn = null;
                row.WHTFmtCode = null;
                row.WHTFmtSub = null;
                row.PayAmt = null;
                row.NetAmt = null;
                row.Jurisdiction = null;
                row.TaxationAgreements = null;
                row.GNHI2Amt = null;
            }
            row.WHTAmt = 0m;
        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran, TWNWHTTran.payAmt> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            CheckAmt(row, 2, row.PayAmt??0m);
            row.NetAmt = row.PayAmt - (row.WHTAmt ?? 0m);

        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran, TWNWHTTran.wHTAmt> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            if (row.WHTAmt == null) row.WHTAmt = 0m;
            //2021/05/06 Add Check
            CheckAmt(row, 1, row.WHTAmt);
            if (row.TranType == null)
                WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.tranType>(row, row.TranType,
                    new PXSetPropertyException(ErrorTranType, PXErrorLevel.RowError));

            if (row.WHTAmt != null)
            {
                decimal? OldWHTAmt = (decimal?)e.OldValue ?? 0;
                decimal? NewWHTAmt = (decimal?)row.WHTAmt ?? 0;
                decimal? TotalGNHI2Amt = 0;
                //For EPExpenseClaimDetails CuryUnicost
                decimal? Amt = 0;

                if (OldWHTAmt != row.WHTAmt)
                    TotalGNHI2Amt = TotalGNHI2Amt - OldWHTAmt + NewWHTAmt;
                switch (row.TranType)
                {
                    case FinStringList.TranType.WHT:
                        claimExt.UsrWhtAmt = (claimExt.UsrWhtAmt ?? 0) + TotalGNHI2Amt;
                        row.NetAmt = (row.PayAmt ?? 0) - (row.WHTAmt ?? 0);
                        Amt = claimExt.UsrWhtAmt ?? 0;
                        break;
                    case FinStringList.TranType.LBI:
                        claimExt.UsrLbiAmt = (claimExt.UsrLbiAmt ?? 0) + TotalGNHI2Amt;
                        Amt = claimExt.UsrLbiAmt ?? 0;
                        break;
                    case FinStringList.TranType.HLI:
                        claimExt.UsrHliAmt = (claimExt.UsrHliAmt ?? 0) + TotalGNHI2Amt;
                        Amt = claimExt.UsrHliAmt ?? 0;
                        break;
                    case FinStringList.TranType.LBP:
                        claimExt.UsrLbpAmt = (claimExt.UsrLbpAmt ?? 0) + TotalGNHI2Amt;
                        Amt = claimExt.UsrLbpAmt ?? 0;
                        break;
                }

                //2021/04/29 Add
                //2021/06/08 Take off Move to Persist
                /*
                EPExpenseClaimDetails details = GetEPDetailsofValuationType(claim.RefNbr, ValuationType(row.TranType));
                Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, Amt);
                Base.ExpenseClaimDetails.Cache.Update(details);
                */
            }
        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran, TWNWHTTran.gNHI2Amt> e)
        {
            TWNWHTTran row = (TWNWHTTran)e.Row;
            if (row == null) return;
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            if (row.GNHI2Amt == null) row.GNHI2Amt = 0m;
            //2021/05/07 Add Check
            CheckAmt(row, 4, row.GNHI2Amt);
            if (row.GNHI2Amt != null)
            {
                decimal? OldGNHI2Amt = (decimal?)e.OldValue ?? 0m;
                decimal? NewGNHI2Amt = (decimal?)row.GNHI2Amt ?? 0m;
                decimal? TotalGNHI2Amt = 0;
                if (OldGNHI2Amt != row.GNHI2Amt)
                    TotalGNHI2Amt = (claimExt.UsrGnhi2Amt ?? 0m) - OldGNHI2Amt + NewGNHI2Amt;
                claimExt.UsrGnhi2Amt = TotalGNHI2Amt;

                //2021/06/08 Take off move to Persist
                /*
                EPExpenseClaimDetails details = GetEPDetailsofValuationType(claim.RefNbr, ValuationType(FinStringList.TranType.GHI));
                if (details == null)
                    CreateEPDetailFromWHTTab(FinStringList.TranType.GHI, row.GNHI2Amt);
                else
                {
                    Base.ExpenseClaimDetails.Cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, TotalGNHI2Amt);
                    Base.ExpenseClaimDetails.Cache.Update(details);
                }
                */
            }
        }
        protected virtual void _(Events.FieldUpdated<TWNWHTTran, TWNWHTTran.netAmt> e)
        {
            TWNWHTTran row = e.Row;
            if (row == null) return;
            CheckAmt(row, 3, row.NetAmt??0);
        }

        //2022-11-16 Alon 12374
        //2022-12-23 Louis 修正Baccount== null錯誤, 及住址只抓address2 戶籍地址
        protected virtual void _(Events.FieldUpdated<TWNWHTTran, TWNWHTTran.personalID> e)
        {
            if (e.Row == null) return;
            if (e.Row.PersonalID != null)
            {
                //透過CD抓
                BAccount baccount = GetBAccountByCD(e.Row.PersonalID) ?? GetBAccountByCategoryID(e.Row.PersonalID);
                if (baccount != null)
                {
                    Address addr = Address.PK.Find(Base, baccount.DefAddressID);
                    e.Cache.SetValueExt<TWNWHTTran.payeeName>(e.Row, baccount.AcctName);
                    //修正只抓address2 戶籍地址
                    //e.Cache.SetValueExt<TWNWHTTran.payeeAddr>(e.Row, (addr?.AddressLine1 ?? "") + (addr?.AddressLine2 ?? ""));
                    e.Cache.SetValueExt<TWNWHTTran.payeeAddr>(e.Row, (addr?.AddressLine2 ?? ""));
                }
            }
        }

        protected virtual void _(Events.RowPersisted<TWNWHTTran> e)
        {
            TWNWHTTran row = e.Row;
            if (row == null) return;

            if (CheckAmt(row, 1, row.WHTAmt ?? 0))
                throw new Exception("代扣稅" + ErrorAmt);

            if (row.TranType == FinStringList.TranType.WHT)
            {
                if (CheckAmt(row, 2, row.PayAmt ?? 0))
                    throw new Exception("代扣稅" + ErrorAmt);
                if (CheckAmt(row, 3, row.NetAmt ?? 0))
                    throw new Exception("代扣稅" + ErrorAmt);
                if (CheckAmt(row, 4, row.GNHI2Amt ?? 0))
                    throw new Exception("代扣稅" + ErrorAmt);
            }

        }
        #endregion
        #endregion

        #region Method
        private void setEnabledUI()
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            EPExpenseClaimExt claimExt = PXCache<EPExpenseClaim>.GetExtension<EPExpenseClaimExt>(claim);
            bool isCHG = claimExt.UsrDocType == EPStringList.EPDocType.CHG;
            //WHTTran
            WHTView.Cache.AllowInsert = !isCHG;
            WHTView.Cache.AllowUpdate = !isCHG;
            WHTView.Cache.AllowDelete = !isCHG;
        }
        /// <summary>
        /// Control WHTTran UI Enabled or Readonly
        /// </summary>
        /// <param name="tran"></param>
        private void setWHTTranUI(TWNWHTTran tran)
        {
            bool isWHT = tran.TranType == FinStringList.TranType.WHT;
            PXUIFieldAttribute.SetReadOnly(WHTView.Cache, null, !isWHT);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.tranType>(WHTView.Cache, null, false);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.wHTAmt>(WHTView.Cache, null, false);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.tranDate>(WHTView.Cache, null, false);
            //Net Amt Always ReadOnly
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.netAmt>(WHTView.Cache, null, true);

        }

        /// <summary>
        /// Transfer TranType to ValuationType
        /// </summary>
        /// <param name="TranType"></param>
        /// <returns>ValuationType</returns>
        public string ValuationType(string TranType)
        {
            string VType = "";
            switch (TranType)
            {
                //代扣稅
                case FinStringList.TranType.WHT:
                    VType = ValuationTypeStringList.WITH_TAX;//20230103 mantis:12386 W -> WITH_TAX(T)
                    break;
                //代扣勞保
                case FinStringList.TranType.LBI:
                    VType = ValuationTypeStringList.L;
                    break;
                //代扣健保
                case FinStringList.TranType.HLI:
                    VType = ValuationTypeStringList.H;
                    break;
                //代扣退休金
                case FinStringList.TranType.LBP:
                    VType = ValuationTypeStringList.N;
                    break;
                //代扣二代健保
                case FinStringList.TranType.GHI:
                    VType = ValuationTypeStringList.SECOND;//20230103 mantis:12386 S -> SECOND(2)
                    break;
            }
            return VType;
        }

        /// <summary>
        /// Create a EP detail for Without Holding Tax From WHTTran Tab
        /// </summary>
        /// <param name="TranType">WHTTran.TranType</param>
        /// <param name="Amt">the same TranType的Sum(WHTAmt)</param>
        private void CreateEPDetailFromWHTTab(string TranType, decimal? Amt)
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            KGSetUp setUp = PXSelect<KGSetUp>.Select(Base);
            EPExpenseClaimDetails currentdetails = Base.ExpenseClaimDetails.Current;
            EPExpenseClaimDetails details = Base.ExpenseClaimDetails.Cache.CreateInstance() as EPExpenseClaimDetails;

            //EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details);

            int? InventoryID = null;
            int? ContractID = null;
            
            switch (TranType)
            {
                case FinStringList.TranType.WHT:
                    InventoryID = setUp.KGIndividualTaxInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.GHI:
                    InventoryID = setUp.KGSupplementaryTaxInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.HLI:
                    InventoryID = setUp.HealthInsInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.LBI:
                    InventoryID = setUp.LaborInsInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
                case FinStringList.TranType.LBP:
                    InventoryID = setUp.PensionInventoryID ?? throw new Exception("請至KG偏好設定維護工料編號!");
                    break;
            }
            Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.inventoryID>(details, InventoryID);
            Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.qty>(details, (decimal)(-1));
            if (currentdetails == null)
                ContractID = 0;
            else
                ContractID = currentdetails.ContractID;
            Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.contractID>(details, ContractID);
            if (ContractID != 0)
            {
                PMCostBudget costBudget = GetPMCostBudget(currentdetails.ContractID, InventoryID);
                if (costBudget == null)
                    throw new Exception(string.Format("請至專案({0})設定工料編號的成本任務!", currentdetails.ContractID));
                Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.costCodeID>(details, costBudget.CostCodeID);
                Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.taskID>(details, costBudget.TaskID);
            }
            Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.curyExtCost>(details, (-1 * Amt));
            Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(details, Amt);
            Base.ExpenseClaimDetails.SetValueExt<EPExpenseClaimDetails.curyTranAmt>(details, (-1 * Amt));

            /*
            //tran.LineNbr = row.LineNbr;
            details.InventoryID = InventoryID;
            //tran.TranDesc = row.TranDesc;
            details.Qty = (-1);
            //tran.UOM = row.UOM;
            details.ContractID = currentdetails.ContractID;
            //tran.TaskID = row.TaskID;
            //tran.CostCodeID = row.CostCodeID;
            details.CuryExtCost = (-1 * Amt);
            details.CuryUnitCost = Amt;
            details.CuryTranAmt = (-1 * Amt);
            //tran.CuryRetainageAmt = Amt;
            //Base.Transactions.Cache.RaiseFieldUpdated<APTran.curyLineAmt>(aPTran, null);
            */
            PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(details).UsrValuationType = ValuationType(TranType);
            Base.ExpenseClaimDetails.Cache.Insert(details);
        }

        /// <summary>
        /// Update Amt which the EP detail of the same UsrValuationtype
        /// </summary>
        /// <param name="TranType">WHTTran.TranType</param>
        /// <param name="Amt">the same TranType的Sum(WHTAmt)</param>
        private void UpdateEPDetailAmt(string TranType, decimal? Amt)
        {
            EPExpenseClaim claim = Base.ExpenseClaim.Current;
            string VType = ValuationType(TranType);
            EPExpenseClaimDetails details =
                PXSelect<EPExpenseClaimDetails,
                Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>,
                And<EPExpenseClaimDetailsExt.usrValuationType, Equal<Required<EPExpenseClaimDetailsExt.usrValuationType>>>>>
                .Select(Base, claim.RefNbr, VType);

            details.CuryExtCost = (-1 * Amt);
            details.CuryUnitCost = Amt;
            details.CuryTranAmt = (-1 * Amt);
            Base.ExpenseClaimDetails.Cache.Update(details);

        }

        /// <summary>
        /// Check Amt >=0
        /// </summary>
        /// <param name="tran">
        /// Current TWNWHTTran
        /// </param>
        /// <param name="Field">
        /// 1. WHTAmt
        /// 2. PayAmt
        /// 3. NetAmt
        /// 4. GNHI2Amt
        /// </param>
        /// <param name="Amt"></param>
        /// <returns>
        /// True : 金額<0要出錯!
        /// False :金額>=0
        /// </returns>
        private bool CheckAmt(TWNWHTTran tran, int? Field, decimal? Amt)
        {
            if (Amt >= 0) return false;
            switch (Field)
            {
                case 1:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.wHTAmt>(tran, Amt,
                        new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
                case 2:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.payAmt>(tran, Amt,
                        new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
                case 3:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.netAmt>(tran, Amt,
                        new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
                case 4:
                    WHTView.Cache.RaiseExceptionHandling<TWNWHTTran.gNHI2Amt>(tran, Amt,
                       new PXSetPropertyException(ErrorAmt, PXErrorLevel.RowError));
                    break;
            }
            return true;
        }

        #endregion

        #region Select Method
        private PMCostBudget GetPMCostBudget(int? ProjectID, int? InventoryID)
        {
            return PXSelect<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>,
                    And<PMCostBudget.inventoryID, Equal<Required<PMCostBudget.inventoryID>>>>>
                    .Select(Base, ProjectID, InventoryID);
        }
        private EPExpenseClaimDetails GetEPDetailsofValuationType(string RefNbr, string VauationType)
        {
            return PXSelect<EPExpenseClaimDetails,
                Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>,
                And<EPExpenseClaimDetailsExt.usrValuationType, Equal<Required<EPExpenseClaimDetailsExt.usrValuationType>>>>>
                .Select(Base, RefNbr, VauationType);
        }
        private TWNWHTTran GetWHTTranofTranType(string RefNbr, string TranType)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.ePRefNbr, Equal<Required<EPExpenseClaim.refNbr>>,
                And<TWNWHTTran.tranType, Equal<Required<TWNWHTTran.tranType>>>>>
                .Select(Base, RefNbr, TranType);
        }
        private PXResultset<TWNWHTTran> getWHT(string TranType)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.tranType, Equal<Required<TWNWHTTran.tranType>>>>
                .Select(Base, TranType);
        }
        private APRegister GetAPRegister(string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, RefNbr);
        }
        private VendorPaymentMethodDetail GetVendorPaymentMethodDetail(int? BAccountID)
        {
            return PXSelect<VendorPaymentMethodDetail,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Required<APInvoice.vendorID>>,
                And<VendorPaymentMethodDetail.detailID, Equal<Required<VendorPaymentMethodDetail.detailID>>>>>
                .Select(Base, BAccountID, "CATEGORYID");
        }
        private Address GetAddress(int? DefAddressID)
        {
            return PXSelect<Address,
                Where<Address.addressID, Equal<Required<BAccount.defAddressID>>>>
                .Select(Base, DefAddressID);
        }
        private Contact GetContact(int? DefContactID)
        {
            return PXSelect<Contact,
                Where<Contact.contactID, Equal<Required<BAccount.defContactID>>>>
                .Select(Base, DefContactID);
        }
        private Vendor GetVendor(int? BAccountID)
        {
            PXGraph graph = new PXGraph();
            return PXSelect<Vendor,
                Where<Vendor.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(graph, BAccountID);
        }
        private BAccount2 GetBAccount(int? BAccount)
        {
            return PXSelect<BAccount2,
                Where<BAccount2.bAccountID, Equal<Required<BAccount2.bAccountID>>>>
                .Select(Base, BAccount);
        }
        private BAccount GetBAccountByCD(string acctCD)
        {
            return PXSelect<BAccount,
                Where<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>
                .Select(new PXGraph(), acctCD);
        }

        private BAccount GetBAccountByCategoryID(string categoryID)
        {
            return PXSelectJoin<BAccount,
                InnerJoin<VendorPaymentMethodDetail, On<VendorPaymentMethodDetail.bAccountID, Equal<BAccount.bAccountID>>>,
                Where<VendorPaymentMethodDetail.detailID, Equal<Required<VendorPaymentMethodDetail.detailID>>,
                    And<VendorPaymentMethodDetail.detailValue, Equal<Required<VendorPaymentMethodDetail.detailValue>>>>>
                .Select(new PXGraph(), "CATEGORYID", categoryID);
        }
        #endregion

        #region Cache Attached

        #region TWNWHTTran

        #region EPRefNbr
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBDefault(typeof(EPExpenseClaim.refNbr))]
        [PXParent(typeof(Select<EPExpenseClaim,
                            Where<EPExpenseClaim.refNbr,
                            Equal<Current<TWNWHTTran.ePRefNbr>>>>))]
        protected void TWNWHTTran_EPRefNbr_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region PersonalID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Personal ID", Required = true)]
        [PXDefault(typeof(Search<VendorExt.usrPersonalID,
            Where<Vendor.bAccountID, Equal<Current<APInvoice.vendorID>>>>))]
        protected void TWNWHTTran_PersonalID_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region BranchID 
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(Current<EPExpenseClaim.branchID>))]
        protected void TWNWHTTran_BranchID_CacheAttached(PXCache sender) { }
        #endregion

        #region PaymentDate
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDefault(typeof(Current<EPExpenseClaim.docDate>))]
        protected void _(Events.CacheAttached<TWNWHTTran.paymentDate> e) { }
        #endregion

        /// <summary>
        /// Enable the primary key definition as DB.
        /// </summary>
        #region WHTTranID
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBIdentity(IsKey = true)]
        protected virtual void _(Events.CacheAttached<TWNWHTTran.wHTTranID> e) { }
        #endregion

        /// <summary>
        /// Remove one of PK field conditions.
        /// </summary>
        #region RefNbr
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(15, IsUnicode = true)]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<TWNWHTTran.refNbr> e) { }
        #endregion

        /// <summary>
        /// Remove one of PK field conditions.
        /// </summary>
        #region DocType
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(3, IsFixed = true)]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        protected virtual void _(Events.CacheAttached<TWNWHTTran.docType> e) { }
        #endregion

        /// <summary>
        /// Remove one of PK field conditions.
        /// </summary>
        #region LineNbr
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBInt()]
        protected virtual void _(Events.CacheAttached<TWNWHTTran.lineNbr> e) { }
        #endregion

        #endregion

        #endregion

        #region Error Msg 
        string ErrorAmt = "金額必須大於0!";
        string ErrorTranType = "請先填寫代扣類型!";
        #endregion
    }
}
