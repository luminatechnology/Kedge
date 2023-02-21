using Kedge.DAC;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.CR;
using System.Collections;
using PX.Objects.TX;
using System;
using PX.Objects.GL;
using PX.Objects.CS;
using System.Collections.Generic;
using PX.Objects.PO;
using PX.Objects.PM;
using Kedge;
using NM.DAC;
using PX.Objects.IN;
using KG.Util;

namespace PX.Objects.AP
{
    /** 
     * 主要為更改APInvoice要產生分錄的程式override
     * 
     * === 2021/03/16 Mantis: 0011984 === Edit By Althea
     * 原本為AP301000產生的單子才可以產生分錄,改為不需檢查
     * 
     * ===2021/03/23 Fix EP === Edit By Althea
     * EP過帳會產生AP 原廠會變成Balance 自動產生KGVouhcer
     * 所以要略過自動產生的KGVoucher
     * 
     * ===2021/04/28 Add Log === Althea
     * 1. 分錄產生KGBillPayment的明細更改為
     * PaymentMethod = A支票：抓KGPostageSetUp.KGCheckAccountID & KGCheckSubID,
     * PaymentMethod = B現金：抓KGPostageSetUp.KGCashAccountID & KGCashSubID,
     * PaymentMethod = C電匯：抓KGPostageSetUp.KGTTAccountID & KGTTSubID,
     * PaymentMethod = D禮券：先搜尋KGBillPayment.Postage是否大於0
     *                                           若>0,抓KGPostageSetUp.KGVenCouponAccountID & KGVenCouponSubID,
     *                                           其他,抓KGPostageSetUp.KGEmpCouponAccountID & KGEmpCouponSubID。
     * 2. 分錄產生
     * 
     * ===2021/06/07 Mantis:0012062 ===Althea
     * Add Button UpdateAPPayment:
     * 若類型為INV且KGBillPayment.PaymentType = C or D(Project ==X && VendorID !=Employee)
     * 產生APPayment 金額帶KGBillPayment.實付金額
     * 若類行為ADR 
     * 找出借方調整的APPayment 塞入apadjust
     * 
     * ===2021/06/24 : 0012109 === Althea
     * PaymentMethod Add E(授扣) 新增相關的分錄明細
     * 
     * ==2021/07/01 : 0012115 ===Althea
     * 遠期票算法多一個條件:
     * 若pricintType = B,則不算遠期日期, 就塞VoucherDate即可
     * 
     * ===2021/06/06 : 0012110===Althea
     * Remove UpdateAPPayment Action
     * 
     * ===2021/07/06 : 0012122 ===Althea
     * Add ADJ Tax to VoucherL
     * 
     * ===2021/07/06 : 0012123 ===Althea
     * 1.分錄的傳票摘要, 有關###廠商名稱###, ###廠商簡稱###, 請改抓KGBillPayment的VendorID對應的廠商名稱及廠商簡稱
     * 2.如果是員工的話, 不分名稱及簡稱, 請都寫入EPEmployee.AcctName
     * 
     * ===2021/07/29 : 0012176 === Althea
     * PaymentMethod Add F(暫付沖銷) , 
     * AccountID = KGPostageSetup.TempWriteoffAcctID,
     * SubID = KGPostageSetup.TempWriteoffSubID
     * 
     * ===2021/08/10 : 0012190 === Althea
     * APTran產生VoucherL時
     * if(此專案的UsrIsSettled(已結算))為true
     * AcctID = Inventory的UsrSettledAccountID
     * SubID = Inventory的UsrSettledSubID
     * 
     * ===2021/08/17 : 0012190 === Althea
     * APTran產生VoucherL時
     * if(此專案的UsrIsSettled(已結算))為true, 回壓APTran 
     * APTran.AccountID = AcctID
     * APTran.SubID = SubID
     * 
     * ===2021/09/14 : 0012229 === Althea
     * ADR PA Add Log:
     * 之前沒有考慮到手動開的借方調整
     * 若ADR不為扣款也不為反轉,
     * PA:
     * 應付帳款科目(APInvoice.APAccountID)
     * 子科目(APInvoice.APSubID)
     * 金額請寫入(APInvoice.CuryLineTotal+APInvoice.CuryTaxTotal)
     * 
     * ===2021/12/13 : 0012279 === Althea
     * Add APT Log:
     * 若來源為EP & projectID = X & EPProjectID !=null
     * 塞入KGVoucherL的專案為EPProjectID
     **/
    public class APInvoiceEntryByVoucherExt : PXGraphExtension<APInvoiceEntry_Extension, APInvoiceEntry>
    {
        #region 分錄 ADD BY Althea

        #region Action
        #region 產生分錄
        public PXAction<APInvoice> AddKGVoucher;
        [PXUIField(DisplayName = "產生分錄",
            MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable addKGVoucher(PXAdapter adapter)
        {
            Base.Persist();
            APInvoice aPInvoice = Base.Document.Current;
            APRegister aPRegister = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            APRegisterExt aPRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(aPRegister);

            //2021/03/23 ADD 為了略過EP產生AP原廠自動Balance所產生的KGVoucher
            KGBillPayment billPayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APRegister.refNbr>>>>.Select(Base, aPRegister.RefNbr);
            if (billPayment == null && aPRegister.OrigModule == KGConst.EP)
                return adapter.Get();


            //2020/05/25 ADD 檢查是否為APInvoice 所產生的單子才要產生分錄
            //2021/03/16 Delete 不需檢查Mantis:0011984
            /*if (aPInvoice.LastModifiedByScreenID == "AP301000")
            {
            if (aPRegisterExt.UsrVoucherNo == null)
            {*/

            //2019/10/25 ADD 產生借方調整
            //2019/11/06 ADD 借方調整刪除前先檢查狀態,不為開啟即可刪除重新新增
            if (CheckDeleteADR(aPInvoice))
            {
                Base1.createReverse();
            }
            DeleteLine();
            Base.Persist();
            CreateKGVoucher();
            Base.Persist();

            //}
            //}
            return adapter.Get();
        }
        #endregion

        //2021/06/07 Add Mantis:0012062
        //2021/07/06 Delete Mantis:0012110
        #region 更新Payment
        public PXAction<APInvoice> UpdateAPPayment;
        //Defining Button Attribute with tooltip
        [PXButton(CommitChanges = true)]
        //Providing title and mapping action access rights
        [PXUIField(DisplayName = "更新Payment",Visible = false)]
        public virtual IEnumerable updateAPPayment(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(Base, UpdateAPPaymentMethod);
            return adapter.Get();
        }
        #endregion
        
        #endregion

        #region Method
        int rowNumber = 1;

        //建立分錄
        public void CreateKGVoucher()
        {
            APInvoice aPInvoice = Base.Document.Current;
            //2021/03/03 Add By Althea 新增 若UsrIsDeductionDoc!= true 才需要產生分錄
            APRegister R = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            APRegisterExt RExt = PXCache<APRegister>.GetExtension<APRegisterExt>(R);

            if (aPInvoice.DocType == "INV" ||
                (aPInvoice.DocType == "ADR" && RExt.UsrIsDeductionDoc != true) ||
                aPInvoice.DocType == "PPM")
            {
                #region KGVoucherH
                KGVoucherH voucherH = (KGVoucherH)Base1.KGVoucherHs.Cache.CreateInstance();
                voucherH.DocType = aPInvoice.DocType;
                voucherH.RefNbr = aPInvoice.RefNbr;
                if (aPInvoice.ProjectID == null)
                {
                    APTran aPTran =
                        PXSelect<APTran, Where<APTran.projectID, IsNotNull,
                        And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                        .Select(Base, aPInvoice.RefNbr);
                    if (aPTran == null) return;
                    Contract contract = GetContract(aPTran.ProjectID);
                    voucherH.ContractID = aPTran.ProjectID;
                    voucherH.ContractCD = contract.ContractCD;
                    voucherH.ContractDesc = contract.Description;
                }
                else
                {
                    Contract contract = GetContract(aPInvoice.ProjectID);
                    voucherH.ContractID = aPInvoice.ProjectID;
                    voucherH.ContractCD = contract.ContractCD;
                    voucherH.ContractDesc = contract.Description;
                }

                BAccount bAccount = GetBAccount(aPInvoice.VendorID);
                //2021/01/04 Althea Fix 出現Type=VC的單子
                //經討論改為如果Type =EP就塞E,其餘都塞V
                // if (bAccount.Type == "VE")
                //{
                //    voucherH.VendorType = "V";
                //} else
                if (bAccount.Type == "EP")
                {
                    voucherH.VendorType = "E";
                }
                else
                {
                    voucherH.VendorType = "V";
                }
                voucherH.VendorID = aPInvoice.VendorID;
                voucherH.VendorCD = bAccount.AcctCD;
                voucherH.Status = "U";
                APTran tran = GetAPTran(aPInvoice.RefNbr);
                //2020/01/03 PONbr會有空值狀況
                if (tran.PONbr != null)
                {
                    voucherH.PONbr = tran.PONbr;
                }

                voucherH = Base1.KGVoucherHs.Insert(voucherH);

                #endregion
                if (aPInvoice.DocType == "INV")
                {
                    CreateValuationOfAPT(voucherH, aPInvoice);//1
                    CreateValuationOfTT(voucherH, aPInvoice);//2
                    CreateValuationOfRR(voucherH, aPInvoice);//3
                    CreateValuationOfDR(voucherH, aPInvoice);//4
                    CreateValuationOfPA(voucherH, aPInvoice);//5
                    // 2021/11/19 ADD
                    // 2021/12/13 Delete
                    //CreateValuationOfPAForTmp(voucherH, aPInvoice);
                    CreateValuationOfKP(voucherH, aPInvoice);//6
                    CreateValuationOfTTF(voucherH, aPInvoice);//10 2021/04/28 ADD(TTFee)
                    CreateValuationOfDT(voucherH, aPInvoice);//7
                    CreateValuationOfDTT(voucherH, aPInvoice);//2019/11/05ADD
                    CreateValuationOfADJ(voucherH, aPInvoice);//8
                    CreateValuationOfADJT(voucherH, aPInvoice);// 2021/07/06 ADD (ADJ TAX)
                }
                else if (aPInvoice.DocType == "ADR")
                {
                    CreateDebitAdjustOfAPT(voucherH, aPInvoice);//1
                    CreateDebitAdjustOfTT(voucherH, aPInvoice);//2
                    CreateDebitAdjustOfRR(voucherH, aPInvoice);//3
                    CreateDebitAdjustOfDR(voucherH, aPInvoice);//4
                    CreateDebitAdjustOfPA(voucherH, aPInvoice);//5
                    CreateDebitAdjustOfKP(voucherH, aPInvoice);//6  
                    //2021/04/28 Add TTFee
                    CreateDebitAdjustOfTTF(voucherH, aPInvoice);//7              
                }
                else if (aPInvoice.DocType == "PPM")
                {
                    CreatePrepaymentsOfAPT(voucherH, aPInvoice);//1
                    CreatePrepaymentsOfTT(voucherH, aPInvoice);//2
                    CreatePrepaymentsOfPA(voucherH, aPInvoice);//3
                    CreatePrepaymentsOfKP(voucherH, aPInvoice);//4
                    //2021/04/28 Add TTFee
                    CreatePrepaymentsOfTTF(voucherH, aPInvoice);//5 
                }

                //2021/11/25 Modify 改為存入DB
                decimal? Camt = 0m;
                decimal? Damt = 0m;
                foreach (KGVoucherL L in Base1.KGVoucherLs.Select())
                {
                    if (L.Cd == "C")
                        Camt = Camt+L.Amt;
                    if (L.Cd == "D")
                        Damt = Damt+L.Amt;
                }
                voucherH.CAmt = Camt;
                voucherH.DAmt = Damt;
                voucherH =Base1.KGVoucherHs.Update(voucherH);
            }

        }

        #region 計價
        //文件明細:(1)APT(假資料Inside)
        public void CreateValuationOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2021/06/09 Add Mantis:0012082 若此張計價單為保留款 RR
            //2022/07/26 Add 新保留款機制 RR
            APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(aPInvoice);
            if (aPInvoice.IsRetainageDocument == true || apRegisterExt.UsrIsRetainageDoc == true) return; 
            //2021/04/28 Add代扣稅新增的項目 健保補充/勞保費/健保費/退休金
            PXResultset<APTran> set = PXSelect<APTran,
                 Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                 And<APTran.tranType, Equal<WordType.iNV>,
                 And<Where<APTranExt.usrValuationType,
                 In3<ValuationTypeStringList.b, //計價
                        ValuationTypeStringList.a, //加款
                        ValuationTypeStringList.withTax,//代扣稅 //20230103 mantis:12386 w -> withTax
                        ValuationTypeStringList.second, //健保補充 //20230103 mantis:12386 s -> second
                        ValuationTypeStringList.l, //勞保費
                        ValuationTypeStringList.h,//健保費
                        ValuationTypeStringList.n>>>>>,//退休金
                 OrderBy<Asc<APTran.lineNbr>>>
                 .Select(Base, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            {
                APTranExt tranFinExt = PXCache<APTran>.GetExtension<APTranExt>(aPTran);
                string CDType = "";
                decimal? LineAmt = 0;
                if (aPTran.CuryLineAmt > 0)
                {
                    CDType = "D";
                    LineAmt = aPTran.CuryLineAmt;
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(aPTran.CuryLineAmt));
                }
                //2021/12/13 Add Mantis: 0012279
                int? ProjectID = aPTran.ProjectID;
                if(aPInvoice.OrigModule == "EP" && aPTran.ProjectID == 0 &&
                    tranFinExt.UsrEPProjectID !=null)
                {
                    ProjectID = tranFinExt.UsrEPProjectID;
                }
                CreateDetailLine(voucherH, ProjectID, aPTran.AccountID,
                    LineAmt, CDType, aPTran.VendorID, "APT", null, null, aPTran.TranDesc,
                    aPTran.SubID, aPTran.TaskID, aPTran.CostCodeID,aPTran.InventoryID,aPTran.TranID);

            }

        }
        //進項稅額:(2)TT
        public void CreateValuationOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2021/06/09 Add Mantis:0012082 若此張計價單為保留款 RR
            if (aPInvoice.IsRetainageDocument == true) return;
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.iNV>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                decimal? TaxAmt = 0;
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "INV");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;

                TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt;

                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    TaxAmt, "D", register.VendorID, "TT", null, null, tax.PurchTaxSubID);
            }
        }
        //退還保留款:(3)RR
        public void CreateValuationOfRR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            KGBillSummary billSummary =
                    PXSelect<KGBillSummary,
                Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
                And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
                .Select(Base, voucherH.DocType, voucherH.RefNbr);

            if (billSummary == null ||
                billSummary.RetentionReturnWithTaxAmt == null ||
                billSummary.RetentionReturnWithTaxAmt == 0) return;

            APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
            Location location = GetLocationAPAccountSub(register);
            CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                billSummary.RetentionReturnWithTaxAmt, "D", register.VendorID, "RR", null, null, location.VRetainageSubID);

        }
        //扣保留款:(4)DR
        public void CreateValuationOfDR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            KGBillSummary billSummary =
                PXSelect<KGBillSummary,
            Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
            And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
            .Select(Base, voucherH.DocType, voucherH.RefNbr);

            if (billSummary == null || billSummary.RetentionAmt == null) return;
            decimal dramt = 0;
            //2019/11/08 保留款金額改為"含稅"保留款金額
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
            InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
            And<APRegister.docType, Equal<WordType.iNV>>>>,
            Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
            .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                dramt = (decimal)(dramt + taxTran.CuryRetainedTaxAmt);
            }
            dramt = dramt + (decimal)billSummary.RetentionAmt;

            if (dramt == 0) return;
            APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
            Location location = GetLocationAPAccountSub(register);
            CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                dramt, "C", register.VendorID, "DR", null, null, location.VRetainageSubID);

        }
        //貸方付款科目:(5)PA
        public void CreateValuationOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                KGPostageSetup setUp = PXSelect<KGPostageSetup>.Select(Base);
                if (setUp == null) throw new Exception(errorKGPostageSetup);

                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                int? SubID;
                decimal? amt;

                //2021/04/28 Modify Log 
                switch (billPayment.PaymentMethod)
                {
                    default:
                    case PaymentMethod.A:
                        AcctID = setUp.KGCheckAccountID ?? throw new Exception(errorAccountmsg); ;
                        SubID = setUp.KGCheckSubID ?? throw new Exception(errorSubmsg);
                        //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                        //amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                        break;
                    case PaymentMethod.B:
                        AcctID = setUp.KGTTAccountID ?? throw new Exception(errorAccountmsg);
                        SubID = setUp.KGTTSubID ?? throw new Exception(errorSubmsg);
                        //2019/12/18 當付款方式為電匯 總金額不用做動作
                        //amt = billPayment.PaymentAmount;
                        break;
                    case PaymentMethod.C:
                        AcctID = setUp.KGCashAccountID ?? throw new Exception(errorAccountmsg);
                        SubID = setUp.KGCashSubID ?? throw new Exception(errorSubmsg);
                        //2019/12/18 當付款方式為現金 總金額不用做動作
                        //amt = billPayment.PaymentAmount;
                        break;
                    case PaymentMethod.D:
                        if (billPayment.PostageAmt > 0)
                        {
                            AcctID = setUp.KGVenCouponAccountID ?? throw new Exception(errorAccountmsg); ;
                            SubID = setUp.KGVenCouponSubID ?? throw new Exception(errorSubmsg);
                        }
                        else
                        {
                            AcctID = setUp.KGEmpCouponAccountID ?? throw new Exception(errorAccountmsg); ;
                            SubID = setUp.KGEmpCouponSubID ?? throw new Exception(errorSubmsg);
                        }
                        //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                        //amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);                   
                        break;
                    //2021/06/24 PaymentMethod Add E(授扣)
                    case PaymentMethod.E:
                        AcctID = setUp.KGAuthAccountID ?? throw new Exception(errorAccountmsg);
                        SubID = setUp.KGAuthSubID ?? throw new Exception(errorSubmsg);
                        break;
                    

                }

                //2021/04/28 Modify 金額直接抓KGBillPayment的金額
                amt = billPayment.ActPayAmt;
                //2021/04/28 Modify 遠期日期要算18/28
                DateTime date;
                //2021/07/01 Add Mantis :0012115 若PricinfType= B 則不算遠期票
                if(billPayment.PricingType == PricingType.A && billPayment.PaymentPeriod >0)
                    date = VoucherUntil.GetLongTremPaymentDate
                     (Base, (DateTime)billPayment.PaymentDate, billPayment.PaymentPeriod);
                else
                    date = (DateTime)billPayment.PaymentDate;


                CreateLine(voucherH, register.ProjectID, AcctID, amt, "C", register.VendorID,
                    "PA", billPayment.BillPaymentID, date, SubID);
            }

        }
        //Delete 2021/12/13 Mantis: 0012267 移至沖銷付款
        //貸方付款科目:PA-TempWriteoff 2021/11/19 Add Mantis:0012267
        public void CreateValuationOfPAForTmp(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            KGBillPayment kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            PXResultset<KGTempPaymentWriteOff> writeoffset =
                PXSelect<KGTempPaymentWriteOff,
                Where<KGTempPaymentWriteOff.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGTempPaymentWriteOff.docType, Equal<Required<APInvoice.docType>>>>>
                .Select(Base, aPInvoice.RefNbr, aPInvoice.DocType);
            foreach (KGTempPaymentWriteOff writeoff in writeoffset)
            {
                if (writeoff == null) return;
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                APTran tran = GetAPTranLine(writeoff.OrigDocType, writeoff.OrigRefNbr, writeoff.OrigLineNbr);
                CreateLine(voucherH, register.ProjectID, tran.AccountID, writeoff.WriteOffAmt ?? 0m, "C", register.VendorID,
                    "PA", null, voucherH.VoucherDate, tran.SubID);
            }

        }
        //郵資費:(6)KP
        public void CreateValuationOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<PaymentMethod.giftCertificate, PaymentMethod.check>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                //2021/04/28 更改為抓KGBillPayment的郵資
                //CalcPostage calcPostage = new CalcPostage();
                //amt = amt + calcPostage.TotalPostageAmt
                // (Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                amt = amt + billPayment.PostageAmt;
            }

            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null) throw new Exception(errorKGPostageSetup);

            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID, amt,
                "C", register.VendorID, "KP", null, null, setUp.KGPostageSubID);
        }
        //扣款頁籤(for 計價): (7)DT
        public void CreateValuationOfDT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APRegister> R = GetDeductionRegister(aPInvoice.RefNbr);

            foreach (APRegister aPRegister in R)
            {
                if (aPRegister == null) return;
                PXResultset<APTran> Set = PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(Base, aPRegister.RefNbr);
                foreach (APTran tran in Set)
                {
                    CreateDetailLine(voucherH, tran.ProjectID, tran.AccountID,
                        tran.CuryLineAmt, "C", tran.VendorID, "DT", null, null,
                        tran.TranDesc, tran.SubID, tran.TaskID, tran.CostCodeID,tran.InventoryID,tran.TranID);
                }
            }
        }
        //扣款頁籤進項稅額(for 計價): (8)DTT
        public void CreateValuationOfDTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //抓借方調整單子的稅額明細(若TaxTran.CuryTaxAmt=0,則不用加這項)
            PXResultset<APRegister> R = GetDeductionRegister(aPInvoice.RefNbr);
            foreach (APRegister aPRegister in R)
            {
                PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.aDR>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPRegister.RefNbr);
                foreach (APTaxTran taxTran in apTaxTran)
                {
                    if (taxTran == null) return;
                    if (taxTran.CuryTaxAmt == 0) return;
                    APRegister register = GetAPRegister(taxTran.RefNbr, "ADR");
                    Tax tax = GetTax(taxTran);
                    if (tax == null) return;
                    CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                        taxTran.CuryTaxAmt, "C", register.VendorID, "DTT", null, null, tax.PurchTaxSubID);
                }
            }

        }
        //退還預付款:(9)ADJ
        public void CreateValuationOfADJ(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            KGBillSummary kGBillSummary = GetKGBillSummary(aPInvoice);
            if (kGBillSummary == null) return;
            if (kGBillSummary.PrepaymentDuctAmt != null)
            {
                APRegister R = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(R);

                //找出預付款單,和此計價單相同的UsrPONbr
                APRegister registerPPM = PXSelect<APRegister,
                Where<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>,
                And<APRegister.docType, Equal<Required<APRegister.docType>>>>>
                .Select(Base, apRegisterExt.UsrPONbr, "PPM");
                if (registerPPM != null)
                {
                    //科目要抓預付款單財務明細.應付帳款的科目
                    CreateLine(voucherH, R.ProjectID, registerPPM.APAccountID,
                        kGBillSummary.PrepaymentDuctAmt, "C", R.VendorID, "ADJ", null, null, registerPPM.APSubID);
                }

            }
        }
        //退還預付款稅額:(11)ADJT
        //2021/07/06 Add Mantis: 0012122
        public void CreateValuationOfADJT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            KGBillSummary kGBillSummary = GetKGBillSummary(aPInvoice);
            if (kGBillSummary == null || kGBillSummary.PrepaymentDuctTaxAmt == null || kGBillSummary.PrepaymentDuctTaxAmt ==0) return;
            if (kGBillSummary.PrepaymentDuctTaxAmt != 0)
            {
                APRegister R = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(R);

                APTaxTran apTaxTran = PXSelectJoin<APTaxTran,
                   InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                   And<APRegister.docType, Equal<WordType.iNV>>>>,
                   Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                   .Select(Base, R.RefNbr);

                if (apTaxTran == null) return;
                Tax tax = GetTax(apTaxTran);
                if (tax == null) return;

                CreateLine(voucherH, R.ProjectID, tax.PurchTaxAcctID,
                    kGBillSummary.PrepaymentDuctTaxAmt, "C", R.VendorID, "ADJT", null, null, tax.PurchTaxSubID);

            }
        }
        //2021/04/28 Add
        //匯款手續費 (10) TTF(TTFee)
        public void CreateValuationOfTTF(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, Equal<PaymentMethod.wireTransfer>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                //2021/04/28 改為直接抓KGBillPayment PostageAmt
                //CalcPostage calcPostage = new CalcPostage();
                //amt = amt + calcPostage.TotalPostageAmt
                //(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                amt = billPayment.PostageAmt;
            }

            KGPostageSetup setUp = PXSelect<KGPostageSetup>.Select(Base);
            if (setUp == null) throw new Exception(errorKGPostageSetup);

            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGTTFeeAccountID, amt,
                "C", register.VendorID, "TTF", null, null, setUp.KGTTFeeSubID);
        }
        #endregion

        #region 借方調整
        //文件明細:(1)APT
        public void CreateDebitAdjustOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2021/04/28 Add代扣稅新增的項目 健保補充/勞保費/健保費/退休金
            PXResultset<APTran> set = PXSelect<APTran,
                 Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                 And<APTran.tranType, Equal<WordType.aDR>,
                 And<Where<APTranExt.usrValuationType,
                 In3<ValuationTypeStringList.b, //計價
                        ValuationTypeStringList.a, //加款
                        ValuationTypeStringList.withTax,//代扣稅 //20230103 mantis:12386 w -> withTax
                        ValuationTypeStringList.second, //健保補充 //20230103 mantis:12386 s -> second
                        ValuationTypeStringList.l, //勞保費
                        ValuationTypeStringList.h,//健保費
                        ValuationTypeStringList.n>>>>>,//退休金
                 OrderBy<Asc<APTran.lineNbr>>>
                 .Select(Base, aPInvoice.RefNbr);
            foreach (APTran aPTran in set)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (aPTran.CuryLineAmt > 0)
                {
                    CDType = "C";
                    LineAmt = aPTran.CuryLineAmt;
                }
                else
                {
                    CDType = "D";
                    LineAmt = (-(aPTran.CuryLineAmt));
                }
                CreateDetailLine(voucherH, aPTran.ProjectID, aPTran.AccountID,
                    LineAmt, CDType, aPTran.VendorID, "APT", null, null, aPTran.TranDesc,
                    aPTran.SubID, aPTran.TaskID, aPTran.CostCodeID,aPTran.InventoryID,aPTran.TranID);
            }
            PXResultset<KGDeductionAPTran> setdeductionAPTran =
                PXSelect<KGDeductionAPTran,
                Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGDeductionAPTran.valuationID, IsNotNull>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGDeductionAPTran tran in setdeductionAPTran)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (tran.CuryLineAmt > 0)
                {
                    CDType = "C";
                    LineAmt = tran.CuryLineAmt;
                }
                else
                {
                    CDType = "D";
                    LineAmt = (-(tran.CuryLineAmt));
                }
                CreateDetailLine(voucherH, tran.ProjectID, tran.AccountID,
                    LineAmt, CDType, tran.VendorID, "APT", null, null, tran.TranDesc,
                    tran.SubID, tran.TaskID, tran.CostCodeID,tran.InventoryID,tran.TranID);
            }
        }
        //進項稅額:(2)TT
        public void CreateDebitAdjustOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.aDR>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "ADR");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;
                decimal? TaxAmt = 0;
                KGDeductionAPTran deductionAPTran = PXSelect<KGDeductionAPTran,
                    Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                    .Select(Base, aPInvoice.RefNbr);
                if (deductionAPTran != null)
                {
                    KGValuation valuation = PXSelect<KGValuation,
                    Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>
                    .Select(Base, deductionAPTran.ValuationID);
                    TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt + valuation.TaxAmt + valuation.ManageFeeTaxAmt;
                }
                else
                {
                    if (taxTran.CuryRetainedTaxAmt != null)
                    {
                        TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt;
                    }
                    else
                    {
                        TaxAmt = taxTran.CuryTaxAmt;
                    }

                }


                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    TaxAmt, "C", register.VendorID, "TT", null, null,
                    tax.PurchTaxSubID);
            }
        }
        //退還保留款:(3)RR
        public void CreateDebitAdjustOfRR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            KGBillSummary billSummary =
                   PXSelect<KGBillSummary,
               Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
               And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
               .Select(Base, voucherH.DocType, voucherH.RefNbr);
            if (billSummary == null ||
                billSummary.RetentionReturnWithTaxAmt == null ||
                billSummary.RetentionReturnWithTaxAmt == 0) return;

            APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
            Location location = GetLocationAPAccountSub(register);
            CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                billSummary.RetentionReturnWithTaxAmt, "C", register.VendorID, "RR",
                null, null, location.VRetainageSubID);

        }
        //扣保留款:(4)DR
        public void CreateDebitAdjustOfDR(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            KGBillSummary billSummary =
                PXSelect<KGBillSummary,
            Where<KGBillSummary.docType, Equal<Required<KGVoucherH.docType>>,
            And<KGBillSummary.refNbr, Equal<Required<KGVoucherH.refNbr>>>>>
            .Select(Base, voucherH.DocType, voucherH.RefNbr);

            if (billSummary == null || billSummary.RetentionAmt == null || billSummary.RetentionAmt == 0) return;
            decimal dramt = 0;
            //2019/11/08 保留款金額改為"含稅"保留款金額
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
            InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
            And<APRegister.docType, Equal<WordType.aDR>>>>,
            Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
            .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                dramt = (decimal)(dramt + taxTran.CuryRetainedTaxAmt);
            }
            dramt = dramt + (decimal)billSummary.RetentionAmt;

            if (dramt == 0) return;

            APRegister register = GetAPRegister(billSummary.RefNbr, billSummary.DocType);
            Location location = GetLocationAPAccountSub(register);
            CreateLine(voucherH, register.ProjectID, location.VRetainageAcctID,
                dramt, "D", register.VendorID, "DR", null, null, location.VRetainageSubID);

        }
        //貸方付款科目:(5)PA
        public void CreateDebitAdjustOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2021/05/06 Modify by Althea 更改為抓原始的APInvoice的KGBillpayment
            //2021/09/14 Add Mantis: 0012229 手動開的借方調整
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            if (registerExt.UsrIsDeductionDoc != true && aPInvoice.OrigRefNbr == null)
            {
                decimal? Amt = aPInvoice.CuryLineTotal + aPInvoice.CuryTaxTotal;
                CreateLine(voucherH, register.ProjectID, aPInvoice.APAccountID,
                        Amt, "D",register.VendorID, "PA", null, aPInvoice.DocDate, aPInvoice.APSubID);
            }
            else
            {
                PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                    Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                    .Select(Base, aPInvoice.OrigRefNbr);
                if (kgbillpayment == null) return;
                foreach (KGBillPayment billPayment in kgbillpayment)
                {
                    if (billPayment == null) return;

                    CalcPostage calcPostage = new CalcPostage();
                    KGPostageSetup setUp = PXSelect<KGPostageSetup,
                        Where<KGPostageSetup.kGCashAccountID, IsNotNull,
                        And<KGPostageSetup.kGCheckAccountID, IsNotNull>>>.Select(Base);
                    if (setUp == null)
                    {
                        throw new Exception("請至KG傳票摘要設定維護資料!");
                    }

                    int? AcctID;
                    int? SubID;
                    decimal? amt;


                    //2021/04/28 Modify Log 
                    switch (billPayment.PaymentMethod)
                    {
                        default:
                        case PaymentMethod.A:
                            AcctID = setUp.KGCheckAccountID ?? throw new Exception(errorAccountmsg); ;
                            SubID = setUp.KGCheckSubID ?? throw new Exception(errorSubmsg);
                            //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                            //amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                            break;
                        case PaymentMethod.B:
                            AcctID = setUp.KGTTAccountID ?? throw new Exception(errorAccountmsg);
                            SubID = setUp.KGTTSubID ?? throw new Exception(errorSubmsg);
                            //2019/12/18 當付款方式為電匯 總金額不用做動作
                            //amt = billPayment.PaymentAmount;
                            break;
                        case PaymentMethod.C:
                            AcctID = setUp.KGCashAccountID ?? throw new Exception(errorAccountmsg);
                            SubID = setUp.KGCashSubID ?? throw new Exception(errorSubmsg);
                            //2019/12/18 當付款方式為現金 總金額不用做動作
                            //amt = billPayment.PaymentAmount;
                            break;
                        case PaymentMethod.D:
                            if (billPayment.PostageAmt > 0)
                            {
                                AcctID = setUp.KGVenCouponAccountID ?? throw new Exception(errorAccountmsg); ;
                                SubID = setUp.KGVenCouponSubID ?? throw new Exception(errorSubmsg);
                            }
                            else
                            {
                                AcctID = setUp.KGEmpCouponAccountID ?? throw new Exception(errorAccountmsg); ;
                                SubID = setUp.KGEmpCouponSubID ?? throw new Exception(errorSubmsg);
                            }
                            //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                            //amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);                   
                            break;
                        //2021/06/24 Add PaymentMethod Add E(授扣)
                        case PaymentMethod.E:
                            AcctID = setUp.KGAuthAccountID ?? throw new Exception(errorAccountmsg); ;
                            SubID = setUp.KGAuthSubID ?? throw new Exception(errorSubmsg);
                            break;
                    }

                    //2021/04/28 Modify 金額直接抓KGBillPayment的金額
                    amt = billPayment.ActPayAmt;
                    //2021/04/28 Modify 遠期日期要算18/28
                    DateTime date;
                    //2021/07/01 Add Mantis :0012115 若PricinfType= B 則不算遠期票
                    if (billPayment.PricingType == PricingType.A && billPayment.PaymentPeriod > 0)
                        date = VoucherUntil.GetLongTremPaymentDate
                         (Base, (DateTime)billPayment.PaymentDate, billPayment.PaymentPeriod);
                    else
                        date = (DateTime)billPayment.PaymentDate;

                    CreateLine(voucherH, register.ProjectID, AcctID, amt, "D",
                        register.VendorID, "PA", billPayment.BillPaymentID, date, SubID);
                }
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(6)KP
        public void CreateDebitAdjustOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2021/05/06 Modify by Althea 更改為抓原始的APInvoice的KGBillpayment
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, 
                In3<PaymentMethod.check, PaymentMethod.giftCertificate>>>>
                .Select(Base, aPInvoice.OrigRefNbr);
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                //2021/04/28 改為抓KGBillPayment的郵資
                //CalcPostage calcPostage = new CalcPostage();
                //amt += amt + calcPostage.TotalPostageAmt
                // (Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                amt = amt + billPayment.PostageAmt;
            }

            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null) throw new Exception(errorKGPostageSetup);
  
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID,
                amt, "D", register.VendorID, "KP", null, null,setUp.KGPostageSubID);
        }
        //匯款手續費:(7)TTF
        public void CreateDebitAdjustOfTTF(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2021/05/06 Modify by Althea 更改為抓原始的APInvoice的KGBillpayment
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod,Equal<PaymentMethod.wireTransfer>>>>
                .Select(Base, aPInvoice.OrigRefNbr);
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                //2021/04/28 改為抓KGBillPayment的郵資
                //CalcPostage calcPostage = new CalcPostage();
                //amt += amt + calcPostage.TotalPostageAmt
                // (Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                amt = billPayment.PostageAmt;
            }

            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null) throw new Exception(errorKGPostageSetup);

            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGTTFeeAccountID,
                amt, "D", register.VendorID, "TTF", null, null, setUp.KGTTFeeSubID);
        }

        #endregion
        //bool isSave = false;
        #region 預付款
        //文件明細:(1)APT
        public void CreatePrepaymentsOfAPT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            //2021/04/28 Add代扣稅新增的項目 健保補充/勞保費/健保費/退休金
            PXResultset<APTran> set = PXSelect<APTran,
                 Where<APTran.refNbr, Equal<Required<APTran.refNbr>>,
                 And<APTran.tranType, Equal<WordType.pPM>,
                 And<Where<APTranExt.usrValuationType,
                 In3<ValuationTypeStringList.b, //計價
                        ValuationTypeStringList.p,//預付款
                        ValuationTypeStringList.a, //加款
                        ValuationTypeStringList.withTax,//代扣稅 //20230103 mantis:12386 w -> withTax
                        ValuationTypeStringList.second, //健保補充 //20230103 mantis:12386 s -> second
                        ValuationTypeStringList.l, //勞保費
                        ValuationTypeStringList.h,//健保費
                        ValuationTypeStringList.n>>>>>,//退休金
                 OrderBy<Asc<APTran.lineNbr>>>
                 .Select(Base, aPInvoice.RefNbr);

            foreach (APTran aPTran in set)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                //2021/05/11 Add 加上預付款類型 金額要抓CuryTranAmt,其餘抓CuryLineAmt
                APTranExt tranExt = PXCache<APTran>.GetExtension<APTranExt>(aPTran);
                if(tranExt.UsrValuationType == ValuationTypeStringList.P)
                {
                    LineAmt = aPTran.CuryTranAmt;
                }
                else
                {
                    LineAmt = aPTran.CuryLineAmt;
                }
                if (LineAmt> 0)
                {
                    CDType = "D";
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(LineAmt));
                }
                APRegister register = GetAPRegister(aPTran.RefNbr, "PPM");
                CreateDetailLine(voucherH, aPTran.ProjectID, register.APAccountID,
                    LineAmt, CDType, aPTran.VendorID, "APT", null, null, aPTran.TranDesc,
                    aPTran.SubID,aPTran.TaskID,aPTran.CostCodeID,aPTran.InventoryID,aPTran.TranID);
            }
            PXResultset<KGDeductionAPTran> setdeductionAPTran =
                PXSelect<KGDeductionAPTran,
                Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGDeductionAPTran.valuationID, IsNotNull>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGDeductionAPTran tran in setdeductionAPTran)
            {
                string CDType = "";
                decimal? LineAmt = 0;
                if (tran.CuryLineAmt > 0)
                {
                    CDType = "D";
                    LineAmt = tran.CuryLineAmt;
                }
                else
                {
                    CDType = "C";
                    LineAmt = (-(tran.CuryLineAmt));
                }
                APRegister register = GetAPRegister(tran.RefNbr, "PPM");
                CreateDetailLine(voucherH, tran.ProjectID, register.APAccountID,
                    LineAmt, CDType, tran.VendorID, "APT", null, null, tran.TranDesc,
                    tran.SubID,tran.TaskID,tran.CostCodeID,tran.InventoryID,tran.TranID);
            }

        }
        //進項稅額:(2)TT
        public void CreatePrepaymentsOfTT(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<APTaxTran> apTaxTran = PXSelectJoin<APTaxTran,
                InnerJoin<APRegister, On<APRegister.refNbr, Equal<APTaxTran.refNbr>,
                And<APRegister.docType, Equal<WordType.pPM>>>>,
                Where<APTaxTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (APTaxTran taxTran in apTaxTran)
            {
                if (taxTran == null) return;
                APRegister register = GetAPRegister(taxTran.RefNbr, "PPM");
                Tax tax = GetTax(taxTran);
                if (tax == null) return;
                decimal? TaxAmt = 0;
                decimal? deTaxAmt = 0;
                decimal? deManageFeeTaxAmt = 0;
                KGDeductionAPTran deductionAPTran = PXSelect<KGDeductionAPTran,
                    Where<KGDeductionAPTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                    .Select(Base, aPInvoice.RefNbr);
                if (deductionAPTran != null)
                {
                    KGValuation valuation = PXSelect<KGValuation,
                        Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>
                        .Select(Base, deductionAPTran.ValuationID);
                    deTaxAmt = valuation.TaxAmt ?? 0;
                    deManageFeeTaxAmt = valuation.ManageFeeTaxAmt ?? 0;
                }

                TaxAmt = taxTran.CuryTaxAmt + taxTran.CuryRetainedTaxAmt + deTaxAmt + deManageFeeTaxAmt;
                CreateLine(voucherH, register.ProjectID, tax.PurchTaxAcctID,
                    TaxAmt, "D", register.VendorID, "TT", null, null,tax.PurchTaxSubID);
            }
        }
        //貸方付款科目:(3)PA
        public void CreatePrepaymentsOfPA(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            PXResultset<KGBillPayment> kgbillpayment = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kgbillpayment == null) return;
            foreach (KGBillPayment billPayment in kgbillpayment)
            {
                if (billPayment == null) return;
                CalcPostage calcPostage = new CalcPostage();
                KGPostageSetup setUp = PXSelect<KGPostageSetup>.Select(Base);
                if (setUp == null)  throw new Exception(errorKGPostageSetup);
  
                APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
                int? AcctID;
                int? SubID;
                decimal? amt = 0;

                //2021/04/28 Modify Log 
                switch (billPayment.PaymentMethod)
                {
                    default:
                    case PaymentMethod.A:
                        AcctID = setUp.KGCheckAccountID ?? throw new Exception(errorAccountmsg); ;
                        SubID = setUp.KGCheckSubID ?? throw new Exception(errorSubmsg);
                        //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                        //amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                        break;
                    case PaymentMethod.B:
                        AcctID = setUp.KGTTAccountID ?? throw new Exception(errorAccountmsg);
                        SubID = setUp.KGTTSubID ?? throw new Exception(errorSubmsg);
                        //2019/12/18 當付款方式為電匯 總金額不用做動作
                        //amt = billPayment.PaymentAmount;
                        break;
                    case PaymentMethod.C:
                        AcctID = setUp.KGCashAccountID ?? throw new Exception(errorAccountmsg);
                        SubID = setUp.KGCashSubID ?? throw new Exception(errorSubmsg);
                        //2019/12/18 當付款方式為現金 總金額不用做動作
                        //amt = billPayment.PaymentAmount;
                        break;
                    case PaymentMethod.D:
                        if (billPayment.PostageAmt > 0)
                        {
                            AcctID = setUp.KGVenCouponAccountID ?? throw new Exception(errorAccountmsg); ;
                            SubID = setUp.KGVenCouponSubID ?? throw new Exception(errorSubmsg);
                        }
                        else
                        {
                            AcctID = setUp.KGEmpCouponAccountID ?? throw new Exception(errorAccountmsg); ;
                            SubID = setUp.KGEmpCouponSubID ?? throw new Exception(errorSubmsg);
                        }
                        //2019/12/18 當付款方式為票據 總金額要扣掉郵資費 再新增一筆郵資費
                        //amt = billPayment.PaymentAmount - calcPostage.TotalPostageAmt(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);                   
                        break;
                    //2021/06/24 PaymentMethod Add E(授扣)
                    case PaymentMethod.E:
                        AcctID = setUp.KGAuthAccountID ?? throw new Exception(errorAccountmsg); ;
                        SubID = setUp.KGAuthSubID ?? throw new Exception(errorSubmsg);
                        break;

                }

                //2021/04/28 Modify 金額直接抓KGBillPayment的金額
                amt = billPayment.ActPayAmt;
                //2021/04/28 Modify 遠期日期要算18/28
                DateTime date;
                //2021/07/01 Add Mantis :0012115 若PricinfType= B 則不算遠期票
                if (billPayment.PricingType == PricingType.A && billPayment.PaymentPeriod > 0)
                    date = VoucherUntil.GetLongTremPaymentDate
                     (Base, (DateTime)billPayment.PaymentDate, billPayment.PaymentPeriod);
                else
                    date = (DateTime)billPayment.PaymentDate;

                CreateLine(voucherH, register.ProjectID, AcctID, amt, "C",
                    register.VendorID, "PA", billPayment.BillPaymentID, date,SubID);
            }
            //CASE KB.PaymentMethod WHEN 'A' THEN KGSetup.KGCheckAccountID ELSE KGSetup.KGCashAccountID
            //(KB.PaymentAmount - calcPostage(KB.PaymentMethod, KB.PaymentAmount))
        }
        //郵資費:(4)KP
        public void CreatePrepaymentsOfKP(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, 
                In3<PaymentMethod.check, PaymentMethod.giftCertificate>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                //2021/04/28 更改為抓KGBillPayment的郵資
                //CalcPostage calcPostage = new CalcPostage();
                //amt += amt + calcPostage.TotalPostageAmt
                //(Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                amt = amt + billPayment.PostageAmt;
            }

            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null) throw new Exception(errorKGPostageSetup);
            
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGPostageAccountID,
                amt, "C", register.VendorID, "KP", null, null,setUp.KGPostageSubID);
        }
        //匯款手續費:(5)TTF
        public void CreatePrepaymentsOfTTF(KGVoucherH voucherH, APInvoice aPInvoice)
        {
            decimal? amt = 0;
            PXResultset<KGBillPayment> set = PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod,
                Equal<PaymentMethod.wireTransfer>>>>
                .Select(Base, aPInvoice.RefNbr);
            foreach (KGBillPayment billPayment in set)
            {
                if (billPayment == null) return;
                //2021/04/28 更改為抓KGBillPayment的郵資
                //CalcPostage calcPostage = new CalcPostage();
                //amt += amt + calcPostage.TotalPostageAmt
                // (Base, billPayment.PaymentPeriod, billPayment.PaymentAmount);
                amt = billPayment.PostageAmt;
            }  
            KGPostageSetup setUp = PXSelect<KGPostageSetup,
                    Where<KGPostageSetup.kGPostageAccountID, IsNotNull>>.Select(Base);
            if (setUp == null) throw new Exception(errorKGPostageSetup);
            
            APRegister register = GetAPRegister(aPInvoice.RefNbr, aPInvoice.DocType);
            CreateLine(voucherH, register.ProjectID, setUp.KGTTFeeAccountID,
                amt, "C", register.VendorID, "TTF", null, null, setUp.KGTTFeeSubID);
        }
        #endregion

        //Line一樣的資料
        public void CreateDetailLine(KGVoucherH voucherH, int? ProjectID,
            int? AcctID, decimal? Amt, string CD, int? VendorID, string kgvouchertype
            , int? PaymentID, DateTime? DueDate, string TranDesc,
            int? SubID,int? TaskID,int? CostCodeID, int? InventoryID,int? tranID)
        {
            KGVoucherL voucherL = new KGVoucherL();
            KGVoucherLFinExt voucherLFinExt = PXCache<KGVoucherL>.GetExtension<KGVoucherLFinExt>(voucherL);
            APInvoice aPInvoice = Base.Document.Current;
            APTran tran = GetAPTranID(tranID);
            APTranExt tranExt = PXCache<APTran>.GetExtension<APTranExt>(tran);

            KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>,
                And<KGVoucherDigestSetup.isProjectCode, Equal<True>>>>
                .Select(Base, AcctID);
            if (setup != null)
            {
                voucherL.ContractID = null;
                voucherL.ContractCD = null;
            }
            else
            {
                Contract contract = GetContract(ProjectID);
                voucherL.ContractID = contract.ContractID;
                voucherL.ContractCD = contract.ContractCD.Trim();
                voucherLFinExt.UsrTaskID = TaskID;
                voucherLFinExt.UsrCostCodeID = CostCodeID;

                //2021/08/10 Add Mantis: 0012190
                ContractFinExt contractFinExt = PXCache<Contract>.GetExtension<ContractFinExt>(contract);
                if(contractFinExt.UsrIsSettled == true)
                {
                    if (InventoryID != null)
                    {
                        InventoryItem item = GetInventory(InventoryID);
                        InventoryItemFinExt itemFinExt = PXCache<InventoryItem>.GetExtension<InventoryItemFinExt>(item);
                        AcctID = itemFinExt.UsrSettledAccountID;
                        SubID = itemFinExt.UsrSettledSubID;
                        PXUpdate<Set<APTran.accountID, Required<APTran.accountID>,
                            Set<APTran.subID, Required<APTran.subID>>>,
                            APTran,
                            Where<APTran.tranID, Equal<Required<APTran.tranID>>>>
                            .Update(Base, AcctID, SubID, tranID);
                    }
                }
            }

            Account account = GetAccount(AcctID);
            voucherL.AccountID = account.AccountID;//AcctID
            voucherLFinExt.UsrSubID = SubID;
            voucherL.AccountCD = account.AccountCD;//AcctID
            voucherL.AccountDesc = account.Description;//AcctID
            voucherL.Cd = CD;//CD
            voucherL.Amt = Amt;//Ant
            BAccount bAccount = GetBAccount(VendorID);
            //2021/01/04 Althea Fix 出現Type=VC的單子
            //經討論改為如果Type =EP就塞E,其餘都塞V
            // if (bAccount.Type == "VE")
            //{
            //    voucherH.VendorType = "V";
            //} else
            if (bAccount.Type == "EP")
            {
                voucherL.VendorType = "E";
            }
            else
            {
                voucherL.VendorType = "V";
            }
            voucherL.VendorID = bAccount.BAccountID;//VendorID
            voucherL.VendorCD = bAccount.AcctCD.Trim();//VendorID
            voucherL.KGVoucherType = kgvouchertype;//kgvouchertype
            voucherL.ItemNo = rowNumber;

            // APTran tran = GetAPTranLine(aPTran.TranType, aPTran.RefNbr, aPTran.LineNbr);
            voucherL.Digest = TranDesc;

            //voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice);

            if (voucherL.KGVoucherType == "PA")
            {
                voucherL.BillPaymentID = PaymentID;
            }
            if(!(tranExt.UsrValuationType == ValuationTypeStringList.WITH_TAX || tranExt.UsrValuationType == ValuationTypeStringList.SECOND || //20230103 mantis:12386 W -> WITH_TAX(T) ,S -> SECOND(2)
                tranExt.UsrValuationType == ValuationTypeStringList.L || tranExt.UsrValuationType == ValuationTypeStringList.H ||
                tranExt.UsrValuationType == ValuationTypeStringList.N || tranExt.UsrValuationType == null))
            {
                if (Amt == 0) return;
            }
        
            voucherL.DueDate = DueDate;
            Base1.KGVoucherLs.Insert(voucherL);
            rowNumber++;
        }
        public void CreateLine(KGVoucherH voucherH, int? ProjectID,
            int? AcctID, decimal? Amt, string CD, int? VendorID, string kgvouchertype
            , int? PaymentID, DateTime? DueDate,
            int? SubID)
        {
            KGVoucherL voucherL = new KGVoucherL();
            KGVoucherLFinExt voucherLFinExt = PXCache<KGVoucherL>.GetExtension<KGVoucherLFinExt>(voucherL);
            APInvoice aPInvoice = Base.Document.Current;
            if (ProjectID == null)
            {
                APTran aPTranContract =
                    PXSelect<APTran, Where<APTran.projectID, IsNotNull,
                    And<APTran.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                    .Select(Base, aPInvoice.RefNbr);
                if (aPTranContract == null) return;
                Contract contract = GetContract(aPTranContract.ProjectID);
                voucherL.ContractID = contract.ContractID;
                voucherL.ContractCD = contract.ContractCD.Trim();


             
            }
            else
            {
                KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>,
                And<KGVoucherDigestSetup.isProjectCode, Equal<True>>>>
                .Select(Base, AcctID);
                if (setup != null)
                {
                    voucherL.ContractID = null;
                    voucherL.ContractCD = null;
                }
                else
                {
                    Contract contract = GetContract(ProjectID);
                    voucherL.ContractID = contract.ContractID;
                    voucherL.ContractCD = contract.ContractCD.Trim();
                }

            }


            PMTask task = getPMTaskisDefault(voucherL.ContractID);
            voucherLFinExt.UsrTaskID = task?.TaskID;

            Account account = GetAccount(AcctID);
            voucherL.AccountID = account.AccountID;//AcctID
            voucherLFinExt.UsrSubID = SubID;
            voucherL.AccountCD = account.AccountCD;//AcctID
            voucherL.AccountDesc = account.Description;//AcctID
            voucherL.Cd = CD;//CD
            voucherL.Amt = Amt;//Ant
            BAccount bAccount = GetBAccount(VendorID);
            //2021/01/04 Althea Fix 出現Type=VC的單子
            //經討論改為如果Type =EP就塞E,其餘都塞V
            // if (bAccount.Type == "VE")
            //{
            //    voucherH.VendorType = "V";
            //} else
            if (bAccount.Type == "EP")
            {
                voucherL.VendorType = "E";
            }
            else
            {
                voucherL.VendorType = "V";
            }
            voucherL.VendorID = bAccount.BAccountID;//VendorID
            voucherL.VendorCD = bAccount.AcctCD.Trim();//VendorID
            voucherL.KGVoucherType = kgvouchertype;//kgvouchertype
            voucherL.ItemNo = rowNumber;

            APInvoiceEntry aPInvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
            APTran aPTran = aPInvoiceEntry.Transactions.Current;
           
            if (voucherL.KGVoucherType == "PA" && PaymentID != null)
            {
                //2021/11/10 Add Mantis: 0012265 當BillPayment的checkTitle有值則帶checkTitle
                KGBillPayment billpayment = GetKGBillPayment(PaymentID);
                voucherL.BillPaymentID = PaymentID;
                voucherLFinExt.UsrPaymentDate = billpayment.PaymentDate;
                if (billpayment.CheckTitle == null)
                    voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice, PaymentID, DueDate);
                else
                    voucherL.Digest = billpayment.CheckTitle;
            }
            else
                voucherL.Digest = GetDigest(AcctID, voucherL.Digest, aPInvoice, null,DueDate);
            if (Amt == 0) return;

            voucherL.DueDate = DueDate;
            Base1.KGVoucherLs.Insert(voucherL);
            rowNumber++;
        }

        //刪除Line
        public void DeleteLine()
        {
            APInvoice aPInvoice = Base.Document.Current;
            KGVoucherH kGVoucherH = PXSelect<KGVoucherH,
                Where<KGVoucherH.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, aPInvoice.RefNbr);
            if (kGVoucherH == null) return;
            PXResultset<KGVoucherL> kgvoucherL =
                PXSelect<KGVoucherL,
                Where<KGVoucherL.voucherHeaderID,
                Equal<Required<KGVoucherH.voucherHeaderID>>>>
                .Select(Base, kGVoucherH.VoucherHeaderID);

            foreach (KGVoucherL voucherL in kgvoucherL)
            {

                if (voucherL == null) return;
                Base1.KGVoucherLs.Delete(voucherL);
            }
            Base1.KGVoucherHs.Delete(kGVoucherH);
            Base.Persist();
        }

        //分錄摘要
        public string GetDigest(int? pAccountID, string Digest, APInvoice aPInvoice, int? billPaymentID, DateTime? Duedate)
        {
            KGVoucherDigestSetup setup = PXSelect<KGVoucherDigestSetup,
                Where<KGVoucherDigestSetup.accountID, Equal<Required<KGVoucherDigestSetup.accountID>>>>
                .Select(Base, pAccountID);
            if (setup == null) return null;

            Digest = setup.OracleDigestRule;
            string Value = "";
            if (aPInvoice.ProjectID == 0)
            {
                Value = "";
            }
            else
            {
                CSAnswers answers = PXSelectJoin<CSAnswers,
                     InnerJoin<Contract,
                     On<Contract.noteID, Equal<CSAnswers.refNoteID>,
                    And<CSAnswers.attributeID, Equal<WordType.a005>,
                    And<Contract.contractID, Equal<Required<APInvoice.projectID>>>>>>>
                    .Select(Base, aPInvoice.ProjectID);
                //2019/11/08 ADD 如果未填專案屬性提醒去設定
                if (answers == null || answers.Value == null)
                {
                    throw new Exception("請至專案的屬性維護完整資料,謝謝!");
                }
                Value = answers.Value;
            }
            BAccount bAccount = null;
            Contact contact = null;
            if (billPaymentID != null)
            {
                KGBillPayment billPayment = PXSelect<KGBillPayment, Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(Base, billPaymentID);
                bAccount = GetBAccount(billPayment.VendorID);
                contact = GetContact(billPayment.VendorID);
            }
            else
            {
                bAccount = GetBAccount(aPInvoice.VendorID);
                contact = GetContact(aPInvoice.VendorID);
            }

            if (setup.OracleDigestRule != null)
            {
                if (setup.OracleDigestRule.Contains("###專案簡稱###"))
                {
                    Digest = Digest.Replace("###專案簡稱###", Value);
                }
                if (setup.OracleDigestRule.Contains("###廠商簡稱###"))
                {
                    //2021/07/06 Add mantis : 0012123
                    if (bAccount.Type == BAccountType.EmployeeType)
                        Digest = Digest.Replace("###廠商簡稱###", bAccount.AcctName);
                    else
                        Digest = Digest.Replace("###廠商簡稱###", contact.FullName);
                }
                if (setup.OracleDigestRule.Contains("###代扣稅率###"))
                {
                    TaxTran taxTran = GetTaxTran(aPInvoice.RefNbr);
                    Digest = Digest.Replace("###代扣稅率###", taxTran.TaxRate.ToString());
                }
                if (setup.OracleDigestRule.Contains("###廠商名稱###"))
                {
                    Digest = Digest.Replace("###廠商名稱###", bAccount.AcctName);
                }
                if (setup.OracleDigestRule.Contains("###到期日###"))
                {
                    if (Duedate != null)
                    {
                        DateTime date = (DateTime)Duedate;
                        Digest = Digest.Replace("###到期日###", date.ToString("yyyy/MM/dd"));
                    }

                }
            }

            return Digest;
        }
        //借方調整的檢核, True=可以產生ADR,False=不可產生ADR&Voucher
        public bool CheckDeleteADR(APInvoice aPInvoice)
        {
            APRegister ADRApinvoice = PXSelect<APRegister,
                Where<APRegister.origRefNbr, Equal<Required<APRegister.origRefNbr>>>>
            .Select(Base, aPInvoice.RefNbr);
            //表示存在ADR
            if (ADRApinvoice != null)
            {
                //若ADR.Status =開啟,則不可以刪除此張借方調整
                if (ADRApinvoice.Status == "N")
                {
                    return false;
                    throw new Exception("此張預付款單狀態為開啟,不可刪除!");
                }
                else
                {
                    APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
                    APInvoiceEntry_Extension apInvoiceEntryExt = graph.GetExtension<APInvoiceEntry_Extension>();
                    // Setting the current product for the graph
                    graph.Document.Current = graph.Document.Search<APInvoice.refNbr>(ADRApinvoice.RefNbr, ADRApinvoice.DocType);
                    //APInvoiceEntry aPInvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                    //aPInvoiceEntry.Document.Current = aPInvoiceEntry.Document.Search<APInvoice.refNbr>(ADRApinvoice.RefNbr);
                    graph.Delete.Press();
                    return true;
                }
            }
            else
            {
                return true;
            }

        }

        public void UpdateAPPaymentMethod()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                APInvoice inv = Base.Document.Current;
                APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
                APPayment payment = null; 
                APRegister register = PXSelect<APRegister, Where<APRegister.refNbr, Equal<Required<APInvoice.refNbr>>>>.Select(Base, inv.RefNbr);
                APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
                string adjdDocType = "";
                string adjdRefNbr = "";

                if (inv.DocType == APDocType.Invoice)
                {
                    PXResultset<KGBillPayment> billPayment = GetBillPaymentMethodCorD(inv.RefNbr);
                    foreach (KGBillPayment kGBillPayment in billPayment)
                    {

                        try
                        {
                            if (kGBillPayment.PaymentMethod == "C" ||
                              (kGBillPayment.PaymentMethod == "D" && inv.ProjectID == 0))
                            {
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
                            }
                            PXUpdate<Set<KGBillPaymentExt.usrIsWriteOff, True>,
                                KGBillPayment,
                                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                                .Update(Base, kGBillPayment.BillPaymentID);
                        }
                        catch (Exception e)
                        {
                            PXUpdate<Set<KGBillPaymentExt.usrIsWriteOff, False>,
                                KGBillPayment,
                                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                                .Update(Base, kGBillPayment.BillPaymentID);
                        }

                    }
                }
                else if (inv.DocType == APDocType.DebitAdj && registerExt.UsrIsDeductionDoc == true)
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
                               .Update(Base, inv.RefNbr);
                    }
                    catch (Exception e)
                    {
                        PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, False>,
                                APRegister,
                                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                                .Update(Base, inv.RefNbr);
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
                                .Update(Base, inv.RefNbr);
                    }
                    catch(Exception e)
                    {
                        PXUpdate<Set<APRegisterFinExt.usrIsWriteOff, False>,
                                APRegister,
                                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                                .Update(Base, inv.RefNbr);
                    }
                }


                

                ts.Complete();
            }
        }
        #endregion

        #region Select Methods
        private APTran GetAPTranLine(string TranType, string RefNbr, int? LineNbr)
        {
            return PXSelect<APTran, Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APTran.tranType, Equal<Required<APInvoice.docType>>,
                And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>.Select(Base, RefNbr, TranType, LineNbr);
        }
        private Contract GetContract(int? ProjectID)
        {
            return PXSelect<Contract,
                    Where<Contract.contractID, Equal<Required<POOrder.projectID>>>>
                    .Select(Base, ProjectID);
        }
        private BAccount GetBAccount(int? VendorID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(Base, VendorID);
        }
        private Account GetAccount(int? AccountID)
        {
            return PXSelect<Account,
                    Where<Account.accountID, Equal<Required<Account.accountID>>>>
                    .Select(Base, AccountID);
        }
        private Location GetLocationAPAccountSub(APRegister register)
        {
            Location location = PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, register.VendorLocationID);

            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, location.VAPAccountLocationID);
        }
        private APRegister GetAPRegister(string RefNbr, string DocType)
        {
            return PXSelect<APRegister,
                Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                .Select(Base, DocType, RefNbr);
        }
        private Tax GetTax(TaxTran taxTran)
        {
            return PXSelect<Tax,
                Where<Tax.taxID, Equal<Required<TaxTran.taxID>>>>
                .Select(Base, taxTran.TaxID);
        }
        private KGBillSummary GetKGBillSummary(APInvoice order)
        {
            return PXSelect<KGBillSummary,
                Where<KGBillSummary.docType, Equal<Required<APInvoice.docType>>,
                And<KGBillSummary.refNbr, Equal<Required<APInvoice.refNbr>>>>>
                .Select(Base, order.DocType, order.RefNbr);
        }
        private KGBillPayment GetKGBillPayment(int? BillPaymentID)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Select(Base, BillPaymentID);
        }
        private APTran GetAPTran(string refNbr)
        {
            return PXSelect<APTran,
                Where<APTran.refNbr, Equal<Required<APTran.refNbr>>>>
                .Select(Base, refNbr);
        }
        private APTran GetAPTranID(int? TranID)
        {
            return PXSelect<APTran,
                Where<APTran.tranID, Equal<Required<APTran.tranID>>>>
                .Select(Base, TranID);
        }
        private APAdjust GetAdjust(string Rdoctype, string Rrefnbr)
        {
            return PXSelect<APAdjust,
                Where<APAdjust.adjdDocType, Equal<Required<APRegister.docType>>,
                And<APAdjust.adjdRefNbr, Equal<Required<APRegister.refNbr>>>>>
                .Select(Base, Rdoctype, Rrefnbr);
        }
        private Contact GetContact(int? VendorID)
        {
            return PXSelectJoin<Contact,
                InnerJoin<BAccount,On<BAccount.defContactID,Equal<Contact.contactID>>>,
                Where<BAccount.bAccountID, Equal<Required<APInvoice.vendorID>>>>
                .Select(Base, VendorID);
        }
        private TaxTran GetTaxTran(string RefNbr)
        {
            return PXSelect<TaxTran,
                Where<TaxTran.refNbr, Equal<Required<TaxTran.refNbr>>>>
                .Select(Base, RefNbr);
        }
        private PXResultset<APRegister> GetDeductionRegister(string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegisterExt.usrIsDeductionDoc, Equal<True>,
                And<APRegisterExt.usrOriDeductionRefNbr, Equal<Required<APInvoice.refNbr>>,
                And<APRegisterExt.usrOriDeductionDocType, Equal<Required<APRegister.docType>>>>>>
                .Select(Base, RefNbr, "INV");
        }
        private PMTask getPMTaskisDefault(int? ProjectID)
        {
            if (ProjectID == 0)
            {
                return null;
            }
            else
            {
                return PXSelect<PMTask,
                    Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
                    And<PMTask.isDefault, Equal<True>>>>
                    .Select(Base, ProjectID);
            }

        }
        private PXResultset<KGBillPayment> GetBillPaymentMethodCorD(string refNbr)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<KGBillPayment.paymentMethod, In3<word.c, word.d>>>>
                .Select(Base, refNbr);
        }
        private NMBankAccount GetBankAccount(int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<KGBillPayment.bankAccountID>>>>
                .Select(Base, BankAccountID);
        }
        private InventoryItem GetInventory(int? InventoryID)
        {
            return PXSelect<InventoryItem,
                Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
                .Select(Base, InventoryID);
        }
        #endregion

        #region Error Msg
        string errorKGPostageSetup = "請至KG傳票摘要設定維護資料!";
        string errorAccountmsg = "請至傳票摘要設定維護科目欄位!";
        string errorSubmsg = "請至傳票摘要設定維護子科目欄位!";
        string errorBillPaymentVendorID = "請填寫付款方式的供應商!";
        #endregion

        #endregion
    }
}
