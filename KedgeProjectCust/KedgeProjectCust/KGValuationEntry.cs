using System;
using PX.Data;
using Kedge.DAC;
using System.Collections.Generic;
using PX.Objects.PO;
using PX.Objects.AP;
using System.Collections;
using PX.Data.SQLTree;
using System.Linq;
using PX.Objects.IN;
using PX.Data.Licensing;
using PX.Objects.CT;

namespace Kedge
{
    /**
     * ===2021/06/15 :0012093 ===Althea
     * ValuationType = 3(票貼):
     * 1) 欄位控制如同一般請款
     * 2) 免管理費 = true, 並且 readonly 
     * 3) isTaxFree = true, 免稅
     * 4) DetailD的工料編號為 KGSetUp.DiscountInventoryID
     * 5) UnConfirmAction,若detailD.Status = B才可以使用
     * 
     * ===2021/07/26 : 0012093 === Althea
     * ValuationTyp = 3(票貼):
     * 1) ValuationContent不為必填
     * 2) isTaxFree 改為 false, 非免稅
     **/
    public class KGValuationEntry : PXGraph<KGValuationEntry, KGValuation>
    {
        #region Action

        public PXAction<KGValuation> ActionMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Action")]
        protected void actionMenu() { }

        #region ConfirmAction
        public PXAction<KGValuation> ConfirmAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Confirm")]
        public virtual IEnumerable confirmAction(PXAdapter adapter)
        {
            KGValuation header = Valuations.Current;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            if (header.Hold == false)
            {
                header.Status = "C";
                ConfirmAction.SetEnabled(false);
                Valuations.Update(header);
                if (header.ValuationType ==KGValuationTypeStringList._0)
                {
                    #region 2019/10/29代扣代付confirm後加款狀態=扣款狀態
                    //2019 /05/31代扣代付confirm後加款資料改成狀態為V
                    detailA.Status = "C";//"V";
                    detailD.Status = "C";
                    #endregion
                    AdditionDetails.Update(detailA);
                    DeductionDetails.Update(detailD);
                    base.Persist();
                }
                else if (header.ValuationType == KGValuationTypeStringList._1 || 
                    header.ValuationType == KGValuationTypeStringList._2 ||
                    //2021/07/20 Add 外勞租用扣款
                    header.ValuationType == KGValuationTypeStringList._4 ||
                    //2021/09/29 Add 附買回扣款單
                    header.ValuationType == KGValuationTypeStringList._5)
                {
                    detailD.Status = "C";
                    DeductionDetails.Update(detailD);
                    base.Persist();
                }
                base.Persist();
            }

            return adapter.Get();
        }
        #endregion

        #region UnConfirmAction
        public PXAction<KGValuation> UnConfirmAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "解除確認")]
        public virtual IEnumerable unConfirmAction(PXAdapter adapter)
        {
            KGValuation header = Valuations.Current;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;

            header.Status = "O";
            header.Hold = true;
            //20220824 Louis 只有代扣代付才會有加款廠商
            if ("0".Equals(header.ValuationType))
            {
                detailA.Status = "O";
                AdditionDetails.Update(detailA);
            }
            detailD.Status = "O";
            Valuations.Update(header);
            DeductionDetails.Update(detailD);
            base.Persist();
            return adapter.Get();
        }
        #endregion

        public KGValuationEntry()
        {
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.ConfirmAction);
            this.ActionMenu.AddMenuAction(this.UnConfirmAction);
        }
        #endregion

        #region Select
        public PXSelect<KGValuation> Valuations;
        public PXSetup<KGSetUp> SetUp;
        public PXSelect<KGValuationDetailA,
                 Where<KGValuationDetailA.valuationID,
                     Equal<Current<KGValuation.valuationID>>,
                 And<KGValuationDetailA.pricingType, Equal<word.a>>>> AdditionDetails;
        public PXSelect<KGValuationDetailD,
                 Where<KGValuationDetailD.valuationID,
                   Equal<Current<KGValuation.valuationID>>,
                 And<KGValuationDetailD.pricingType, Equal<word.d>>>> DeductionDetails;
        #endregion

        #region Master Event
        protected virtual void KGValuation_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            PXUIFieldAttribute.SetEnabled<KGValuation.valuationType>(Valuations.Cache, row, true);
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            PXUIFieldAttribute.SetEnabled<KGValuation.qty>(sender, row, false);
            PXUIFieldAttribute.SetEnabled<KGValuation.unitPrice>(sender, row, false);
            #region 2019/05/29把isManageTaxFree&isTaxFree拿掉,加上isManageFeeAmt
            /*PXUIFieldAttribute.SetEnabled<KGValuation.isManageTaxFree>(sender, row, false);
            PXUIFieldAttribute.SetEnabled<KGValuation.isTaxFree>(sender, row, false);*/
            #endregion
            PXUIFieldAttribute.SetEnabled<KGValuation.isFreeManageFeeAmt>(sender, row, false);
            PXUIFieldAttribute.SetEnabled<KGValuation.additionAmt>(sender, row, false);
            PXUIFieldAttribute.SetEnabled<KGValuation.deductionAmt>(sender, row, false);
            PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, false);
            PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, false);
            if (row.Hold == true)
            {
                ConfirmAction.SetEnabled(false);
            }
        }
        protected virtual void KGValuation_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            //2019/06/24 新增存檔前的預算整合
            checkDeductionAmt(Valuations.Cache, row, row.DeductionAmt, detailD.UnBilledTotalAmt);
        }
        protected virtual void KGValuation_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            if (row.ValuationID > 0)
            {
                PXUIFieldAttribute.SetEnabled<KGValuation.valuationType>(Valuations.Cache, row, false);
            }
        }
        protected virtual void KGValuation_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            //KGValuationDetailA detailA = AdditionDetails.Current;
            //KGValuationDetailD detailD = DeductionDetails.Current;
            KGValuationDetailA detailA = PXSelect<KGValuationDetailA,
                Where<KGValuationDetailA.valuationID, Equal<Required<KGValuationDetailA.valuationID>>>>
                .Select(this, row.ValuationID);
            KGValuationDetailD detailD = PXSelect<KGValuationDetailD,
                Where<KGValuationDetailD.valuationID, Equal<Required<KGValuationDetailD.valuationID>>>>
                .Select(this, row.ValuationID);

            //2020/04/21 博偉說不需要isTaxFree Checkbox, 故先隱藏
            //2019/12/18 不管type為何都要顯示免稅checkbox
            //PXUIFieldAttribute.SetVisible<KGValuation.isTaxFree>(Valuations.Cache, row,row.ValuationType =="1");
            PXUIFieldAttribute.SetVisible<KGValuation.isTaxFree>(Valuations.Cache, row,false);

            if (row.ValuationID < 0)
            {
                PXUIFieldAttribute.SetEnabled<KGValuation.valuationType>(Valuations.Cache, row, true);
            }
            if (row.Hold == true)
            {
                ConfirmAction.SetEnabled(false);
                UnConfirmAction.SetEnabled(false);
                if (row.ValuationType == KGValuationTypeStringList._0)
                {
                    
                    PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, true);
                    PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, true);
                    KGValuationDetailA detailA1 = null;
                    PXResultset<KGValuationDetailA> detailAResultSet = AdditionDetails.Select();
                    if (detailAResultSet.Count > 0)
                    {
                        detailA1 = (KGValuationDetailA)detailAResultSet;
                        if (detailA1.OrderNbr != null)
                        {
                            PXUIFieldAttribute.SetEnabled<KGValuation.qty>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.unitPrice>(sender, row, true);
                            
                            //2019 /05/29把isManageTaxFree&isTaxFree拿掉,加上isManageFeeAmt
                            /*PXUIFieldAttribute.SetEnabled<KGValuation.isManageTaxFree>(sender, row, true);*/
                            //2019/12/18 把免稅checkbox拿回來
                            PXUIFieldAttribute.SetEnabled<KGValuation.isTaxFree>(sender, row, true);                          
                            PXUIFieldAttribute.SetEnabled<KGValuation.isFreeManageFeeAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.additionAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.deductionAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.uom>(sender, row, true);
                        }

                    }
                    else
                    {
                        PXUIFieldAttribute.SetEnabled(sender, row, false);

                        PXUIFieldAttribute.SetEnabled<KGValuation.valuationCD>(sender, row, true);
                        PXUIFieldAttribute.SetEnabled<KGValuation.valuationDate>(sender, row, true);
                        PXUIFieldAttribute.SetEnabled<KGValuation.contractID>(sender, row, true);
                        PXUIFieldAttribute.SetEnabled<KGValuation.valuationType>(sender, row, true);
                        PXUIFieldAttribute.SetEnabled<KGValuation.description>(sender, row, true);
                        PXUIFieldAttribute.SetEnabled<KGValuation.uom>(sender, row, true);
                    }
                }
                //2021/06/15 Add ValautionType._3(會計貼票)比照一般扣款辦理
                //2021/07/19 Add ValautionType._4(外勞租用扣款)比照一般扣款辦理
                //2021/09/28 Add ValautionType._5(附買回扣款)比照一般扣款辦理
                else if (row.ValuationType == KGValuationTypeStringList._1 || row.ValuationType == KGValuationTypeStringList._2 ||
                     row.ValuationType == KGValuationTypeStringList._3 || row.ValuationType == KGValuationTypeStringList._4 ||
                     row.ValuationType == KGValuationTypeStringList._5)
                {                 
                    PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, false);
                    PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, true);
                    PXUIFieldAttribute.SetEnabled<KGValuation.additionAmt>(Valuations.Cache, row, false);
                    KGValuationDetailD detailD1 = null;
                    PXResultset<KGValuationDetailD> detailDResultSet = DeductionDetails.Select();
                    if (detailDResultSet.Count > 0)
                    {
                        detailD1 = (KGValuationDetailD)detailDResultSet;
                        if (detailD1.OrderNbr != null)
                        {
                            PXUIFieldAttribute.SetEnabled<KGValuation.qty>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.unitPrice>(sender, row, true);
                            #region 2019/05/29把isManageTaxFree&isTaxFree拿掉,加上isManageFeeAmt
                            /*PXUIFieldAttribute.SetEnabled<KGValuation.isManageTaxFree>(sender, row, true);*/
                            //2019/06/27拿回此功能
                            PXUIFieldAttribute.SetEnabled<KGValuation.isTaxFree>(sender, row, true);
                            #endregion
                            PXUIFieldAttribute.SetEnabled<KGValuation.isFreeManageFeeAmt>(sender, row, row.ValuationType != KGValuationTypeStringList._3);
                            //PXUIFieldAttribute.SetEnabled<KGValuation.additionAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.deductionAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.uom>(sender, row, true);
                        }

                    }
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, false);
                    PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, false);
                    setAmtEnable(false);
                }
                PXUIFieldAttribute.SetEnabled<KGValuation.valuationContent>(sender, row, row.IsFreeManageFeeAmt == true);
            }
            else if (row.Hold == false)
            {
                bool statusComfirm = row.Status =="C";
                PXUIFieldAttribute.SetEnabled(sender, row, false);
                PXUIFieldAttribute.SetEnabled(AdditionDetails.Cache, detailA, false);
                PXUIFieldAttribute.SetEnabled(DeductionDetails.Cache, detailD, false);
                PXUIFieldAttribute.SetEnabled<KGValuation.hold>(sender, row, !statusComfirm);
                PXUIFieldAttribute.SetEnabled<KGValuation.valuationCD>(sender, row, true);
                ConfirmAction.SetEnabled(!statusComfirm);
                Delete.SetEnabled(!statusComfirm);
                if (row.ValuationType == KGValuationTypeStringList._0)
                {
                    if (row.Status == "C" && detailA.Status == "C" && detailD.Status == "C")
                    {
                        UnConfirmAction.SetEnabled(true);
                    }
                    else
                        UnConfirmAction.SetEnabled(false);
                }
                //2021/06/15 Add Mantis: 0012093
                else if(row.ValuationType == KGValuationTypeStringList._3)
                {
                    if (detailD.Status != "B")
                    {
                        UnConfirmAction.SetEnabled(true);
                    }
                    else
                        UnConfirmAction.SetEnabled(false);
                }
                else
                {
                    if (row.Status == "C" && detailD.Status == "C")
                    {
                        UnConfirmAction.SetEnabled(true);
                    }
                    else
                        UnConfirmAction.SetEnabled(false);
                }
            }

        }
        protected virtual void KGValuation_ValuationType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;

            //2020/04/21 博偉說不需要有免稅checkbox
            //2019/12/18 不管什麼type都要顯示免稅checkbox
            //PXUIFieldAttribute.SetVisible<KGValuation.isTaxFree>(Valuations.Cache, row, row.ValuationType=="1");

            if (row.ValuationType == KGValuationTypeStringList._0)
            {
                if (detailA != null)
                {
                    setAmtEnable(false);
                    setDetailAValueToNull();
                    setHeaderValueToNull();
                }
                if (detailD != null)
                {
                    setAmtEnable(false);
                    setDetailDValueToNull();
                    setHeaderValueToNull();
                }
                PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, true);
                PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, true);
            }
            else if (row.ValuationType == KGValuationTypeStringList._1 || row.ValuationType == KGValuationTypeStringList._2 ||
                row.ValuationType == KGValuationTypeStringList._3 || row.ValuationType == KGValuationTypeStringList._4 ||
                row.ValuationType == KGValuationTypeStringList._5)
            {
                if (detailA != null)
                {
                    setAmtEnable(false);
                    setDetailAValueToNull();
                    setHeaderValueToNull();
                }
                if (detailD != null)
                {
                    setAmtEnable(false);
                    setDetailDValueToNull();
                    setHeaderValueToNull();
                }
                PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, false);
                PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, true);
                PXUIFieldAttribute.SetEnabled<KGValuation.additionAmt>(Valuations.Cache, row, false);
               
            }
            //2020/04/21 博偉說不需要有免稅button... Visble = false 一律帶isTaxfree = false
            //2019/07/05 ADD 若為罰款免稅和免管理費打勾
            if (row.ValuationType == KGValuationTypeStringList._1)
            {
                //row.IsTaxFree = true;
                row.IsFreeManageFeeAmt = true;
            }
            //2021//07/26 Add Mantis: 0012093
            //當類型為會計票貼,免管理費為true,免稅改為false
            //2021/06/15 Add Mantis: 0012093
            //當類型為會計票貼,免管理費為true,免稅也為true
            else if (row.ValuationType == KGValuationTypeStringList._3)
            {
                row.IsFreeManageFeeAmt = true;
                row.IsTaxFree = true;
            }
            //2021/09/28 Add Mantis: 0012247
            //當類型為附買回扣款,免管理費為true,免稅也為true
            else if (row.ValuationType == KGValuationTypeStringList._5)
            {
                row.IsFreeManageFeeAmt = true;
                row.IsTaxFree = true;
            }
            else
            {
                row.IsFreeManageFeeAmt = false;
                row.IsTaxFree = false;
            }

        }
        protected virtual void KGValuation_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            //2019/04/26增加預算檢核
            /*checkAmtEnoughOrNot(
                detailD.OrderNbr,detailD.LineNbr,detailD.ContractID,
                row.DeductionAmt,row.Hold);*/

            CheckAll();
            if (setCheckThread == false)
            {
                if (row.Status == "O" || row.Status == "P")
                {
                    if (detailD != null)
                    {
                        //checkDeductionAmt(Valuations.Cache, row, row.DeductionAmt, detailD.UnBilledTotalAmt);
                        if (row.Hold == false)
                        {

                            PXUIFieldAttribute.SetEnabled(sender, row, false);
                            PXUIFieldAttribute.SetEnabled(AdditionDetails.Cache, detailA, false);
                            PXUIFieldAttribute.SetEnabled(DeductionDetails.Cache, detailD, false);
                            PXUIFieldAttribute.SetEnabled<KGValuation.hold>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.valuationCD>(sender, row, true);
                            ConfirmAction.SetEnabled(true);
                            row.Status = "P";

                        }
                        else
                        {
                            PXUIFieldAttribute.SetEnabled(sender, row, false);
                            PXUIFieldAttribute.SetEnabled<KGValuation.hold>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.valuationCD>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.contractID>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.valuationContent>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.qty>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.additionAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.deductionAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.valuationDate>(sender, row, true);
                            #region 2019/05/29把isManageTaxFree&isTaxFree拿掉,加上isManageFeeAmt
                            /*PXUIFieldAttribute.SetEnabled<KGValuation.isManageTaxFree>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.isTaxFree>(sender, row, true);*/
                            #endregion
                            PXUIFieldAttribute.SetEnabled<KGValuation.isFreeManageFeeAmt>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.description>(sender, row, true);
                            PXUIFieldAttribute.SetEnabled<KGValuation.valuationContent>(sender, row, row.IsFreeManageFeeAmt == true);


                            if (row.ValuationID > 0)
                            {
                                PXUIFieldAttribute.SetEnabled<KGValuation.valuationType>(sender, row, false);
                            }
                            else if (row.ValuationID < 0)
                            {
                                PXUIFieldAttribute.SetEnabled<KGValuation.valuationType>(sender, row, true);
                            }
                            row.Status = "O";
                            if (row.ValuationType == KGValuationTypeStringList._0)
                            {
                                PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, true);
                                PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, true);
                            }
                            else if (row.ValuationType == KGValuationTypeStringList._1 || row.ValuationType == KGValuationTypeStringList._2 ||
                                //2021/07/19 add 4:外勞租用扣款 依照 2:一般扣款辦理
                                row.ValuationType == KGValuationTypeStringList._3 || row.ValuationType == KGValuationTypeStringList._4 ||
                                //20210/9/28 add 5:附買回扣款 依照 2:一般扣款辦理
                                row.ValuationType == KGValuationTypeStringList._5)
                            {
                                PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, true);
                            }
                            else
                            {
                                PXUIFieldAttribute.SetEnabled(AdditionDetails.Cache, detailA, false);
                                PXUIFieldAttribute.SetEnabled(DeductionDetails.Cache, detailD, false);
                            }
                        }
                    }

                }
                else if (row.Status == "C")
                {
                    throw new Exception("已經確認");
                }
            }
            else
            {
                row.Hold = true;
            }
            

        }
        protected virtual void KGValuation_Uom_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            if (row.Uom == "式")
            {
                row.Qty = 1;
            }
        }
        protected virtual void KGValuation_Qty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            if (row.ValuationType == KGValuationTypeStringList._0)
            {
                if (detailA != null)
                {
                    if (detailA.OrderNbr != null)
                    {
                        CaclPrice(row.Qty, row.UnitPrice, detailA.OrderNbr, detailA.OrderType);
                    }
                }

            }
            else if (row.ValuationType == KGValuationTypeStringList._1 || row.ValuationType == KGValuationTypeStringList._2 || 
                row.ValuationType == KGValuationTypeStringList._3 ||
                //2021/07/19 add 4:外勞租用扣款 依照 2:一般扣款辦理
                row.ValuationType == KGValuationTypeStringList._4 ||
                //2021/09/28 Add 5:附買回扣款 依照 2:一般扣款辦理 
                row.ValuationType == KGValuationTypeStringList._5
                )
            {
                if (detailD != null)
                {
                    if (detailD.OrderNbr != null)
                    {
                        CaclPrice(row.Qty, row.UnitPrice, detailD.OrderNbr, detailD.OrderType);
                    }
                }
            }
        }
        protected virtual void KGValuation_UnitPrice_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            if (row.ValuationType == KGValuationTypeStringList._0)
            {
                CaclPrice(row.Qty, row.UnitPrice, detailA.OrderNbr, detailA.OrderType);

            }
            else if (row.ValuationType == KGValuationTypeStringList._1 || row.ValuationType == KGValuationTypeStringList._2 ||
                row.ValuationType == KGValuationTypeStringList._3 || row.ValuationType == KGValuationTypeStringList._4 ||
                row.ValuationType == KGValuationTypeStringList._5)
            {

                CaclPrice(row.Qty, row.UnitPrice, detailD.OrderNbr, detailD.OrderType);
            }
        }
        protected virtual void KGValuation_IsTaxFree_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            CaclPrice(row.Qty, row.UnitPrice, detailD.OrderNbr, detailD.OrderType);
        }
        #region 2019/06/24把isTaxFree拿回來,加上特殊功能
        //2019/05/29把isManageTaxFree&isTaxFree拿掉,加上isManageFeeAmt        
        /*protected virtual void KGValuation_IsManageTaxFree_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            if (row.IsManageTaxFree == true)
            {
                PXResultset<KGSetUp> setup = PXSelect<KGSetUp,
                             Where<KGSetUp.kGManageFeeTaxRate,
                                 IsNotNull>>.Select(this);
                foreach (KGSetUp rate in setup)
                {
                    row.ManageFeeTaxAmt
                        = row.ManageFeeAmt * (rate.KGManageFeeTaxRate / 100);
                }

            }
            else
            {
                row.ManageFeeTaxAmt = 0;
            }
            row.ManageFeeTotalAmt = row.ManageFeeAmt + row.ManageFeeTaxAmt;
            row.DeductionAmt = row.TotalAmt + row.ManageFeeTotalAmt;
        }*/
        #endregion
        protected virtual void KGValuation_IsFreeManageFeeAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuation row = (KGValuation)e.Row;
            if (row.IsFreeManageFeeAmt == true)
            {
                row.ManageFeeAmt = 0;
                row.ManageFeeTaxAmt = 0;
                row.ManageFeeTotalAmt = 0;
                row.DeductionAmt = row.TotalAmt + row.ManageFeeTotalAmt;
                PXUIFieldAttribute.SetEnabled<KGValuation.valuationContent>(sender, row, true);
            }
            else
            {
                PXResultset<KGSetUp> setup = PXSelect<KGSetUp,
                           Where<KGSetUp.kGManageFeeTaxRate,
                               IsNotNull>>.Select(this);
                foreach (KGSetUp kgsetup in setup)
                {
                    row.ManageFeeAmt = row.Amount * (kgsetup.KGManageFeeRate / 100);
                    row.ManageFeeTaxAmt
                            = decimal.Round(Convert.ToDecimal(row.ManageFeeAmt) * (Convert.ToDecimal(kgsetup.KGManageFeeTaxRate / 100)));
                }
                row.ManageFeeTotalAmt = row.ManageFeeAmt + row.ManageFeeTaxAmt;
                row.DeductionAmt = row.TotalAmt + row.ManageFeeTotalAmt;
                PXUIFieldAttribute.SetEnabled<KGValuation.valuationContent>(sender, row, false);
                if(row.ValuationContent!=null)
                {
                    row.ValuationContent = null;
                }
            }
        }
        
        #endregion

        #region DetailA Event
        protected virtual void KGValuationDetailA_OrderNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuationDetailA row = (KGValuationDetailA)e.Row;
            KGValuationDetailD detailD = DeductionDetails.Current;
            KGValuation header = Valuations.Current;
            //加款供應商顯示
            PXResultset<POOrder> set = PXSelect<POOrder,
                            Where<POOrder.orderNbr,
                                Equal<Required<POOrder.orderNbr>>>>
                                .Select(this, row.OrderNbr);
            foreach (POOrder poorder in set)
            {
                row.OrderType = poorder.OrderType;
                PXResultset<Vendor> name =
                PXSelectJoin<Vendor,
                LeftJoin<POOrder, On<POOrder.vendorID, Equal<Vendor.bAccountID>>>,
                Where<Vendor.bAccountID,
                Equal<Required<Vendor.bAccountID>>, And<Match<Vendor, Current<AccessInfo.userName>>>>>
                .Select(this, poorder.VendorID);
                foreach (Vendor vendor in name)
                {
                    row.Vendor = vendor.AcctName;
                }

            }

            //加款工料編號抓KGSetup的KGAdditionInventoryCD+顯示工料描述
            PXResultset<KGSetUp> setup = PXSelect<KGSetUp,
                Where<KGSetUp.kGAdditionInventoryID, IsNotNull,
                And<KGSetUp.kGDeductionInventoryID, IsNotNull>>>.Select(this);
            foreach (KGSetUp kgsetup in setup)
            {
                row.InventoryID = kgsetup.KGAdditionInventoryID;
                PXResultset<InventoryItem> setinventoryitem = PXSelect<InventoryItem,
                    Where<InventoryItem.inventoryID
                    , Equal<Required<InventoryItem.inventoryID>>>>
                    .Select(this, kgsetup.KGAdditionInventoryID);
                foreach (InventoryItem inventoryItem in setinventoryitem)
                {
                    row.InvDesc = inventoryItem.Descr;
                }
            }

            if (detailD != null)
            {

                if (detailD.OrderNbr != null)
                {
                    setAmtEnable(true);
                    checkOrderNbrA(AdditionDetails.Cache, row, row.OrderNbr);
                }
            }
            //若ordernbr=null 把下面資料都刪除
            if (row.OrderNbr == null)
            {
                row.Vendor = null;
                row.InvDesc = null;
                row.InventoryID = null;
                setAmtEnable(false);
            }

        }
        protected virtual void KGValuationDetailA_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGValuationDetailA row = (KGValuationDetailA)e.Row;
            KGValuation header = Valuations.Current;
            if (header != null)
            {
                if (header.ValuationType == "0")
                {
                    row.ContractID = header.ContractID;
                    row.Qty = header.Qty;
                    row.UnitPrice = header.UnitPrice;
                    row.Uom = header.Uom;
                    row.Amount = header.Amount;
                    row.TaxAmt = header.TaxAmt;
                    row.TotalAmt = header.TotalAmt;
                    #region 2019/10/29 代扣代付Confirm後加款狀態=扣款狀態
                    //2019 /05/31代扣代付confirm後加款資料改成狀態為V
                    if (header.Status =="C")
                    {
                        row.Status = "C";//"V";
                    }
                    else
                    {
                        row.Status = header.Status;
                    }
                    //row.Status = header.Status;
                    #endregion
                    row.ManageFeeAmt = 0;
                    row.ManageFeeTaxAmt = 0;
                    row.ManageFeeTotalAmt = 0;
                }
                else
                {
                    e.Cancel = true;
                }
            }

        }
        protected virtual void KGValuationDetailA_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGValuationDetailA row = (KGValuationDetailA)e.Row;
            KGValuation header = Valuations.Current;
            if (row == null) return;
            if (row.OrderNbr != null)
            {
                //加款顯示供應商
                PXResultset<POOrder> set = PXSelect<POOrder,
                            Where<POOrder.orderNbr,
                                Equal<Required<POOrder.orderNbr>>>>
                                .Select(this, row.OrderNbr);
                foreach (POOrder poorder in set)
                {
                    PXResultset<Vendor> name =
                    PXSelectJoin<Vendor,
                    LeftJoin<POOrder, On<POOrder.vendorID, Equal<Vendor.bAccountID>>>,
                    Where<Vendor.bAccountID,
                    Equal<Required<Vendor.bAccountID>>, And<Match<Vendor, Current<AccessInfo.userName>>>>>
                    .Select(this, poorder.VendorID);
                    foreach (Vendor vendor in name)
                    {
                        row.Vendor = vendor.AcctName;
                    }
                }

                //加款顯示工料編號+工料描述
                PXResultset<KGSetUp> setup = PXSelect<KGSetUp,
                Where<KGSetUp.kGAdditionInventoryID, IsNotNull,
                And<KGSetUp.kGDeductionInventoryID, IsNotNull>>>.Select(this);
                foreach (KGSetUp kgsetup in setup)
                {
                    row.InventoryID = kgsetup.KGAdditionInventoryID;
                    PXResultset<InventoryItem> setinventoryitem = PXSelect<InventoryItem,
                        Where<InventoryItem.inventoryID
                        , Equal<Required<InventoryItem.inventoryID>>>>
                        .Select(this, kgsetup.KGAdditionInventoryID);
                    foreach (InventoryItem inventoryItem in setinventoryitem)
                    {
                        row.InvDesc = inventoryItem.Descr;
                    }
                }
                setAmtEnable(true);
            }
            else
            {
                setAmtEnable(false);
            }

        }
        #endregion

        #region DetailD Event
        protected virtual void KGValuationDetailD_OrderNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGValuationDetailD row = (KGValuationDetailD)e.Row;
            KGValuationDetailD detailD = DeductionDetails.Current;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuation header = Valuations.Current;
            //扣款供應商顯示+儲存ordertype
            POOrder poorder = PXSelect<POOrder,
                        Where<POOrder.orderNbr,
                            Equal<Required<POOrder.orderNbr>>>>
                            .Select(this, row.OrderNbr);
            row.OrderType = poorder?.OrderType;

            Vendor vendor =
                PXSelect<Vendor,
                Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>
                .Select(this, poorder.VendorID);
            row.Vendor = vendor?.AcctName;


        

            //扣款工料編號抓KGSetup的KGAdditionInventoryCD+顯示工料描述
            KGSetUp setup = PXSelect<KGSetUp,
                Where<KGSetUp.kGAdditionInventoryID, IsNotNull,
                And<KGSetUp.kGDeductionInventoryID, IsNotNull>>>.Select(this);

            //2020/01/21 Add 若加扣款類型為代扣代付,則工料編號帶與加款的工料編號一至
            if (header.ValuationType == KGValuationTypeStringList._0)
                row.InventoryID = setup?.KGAdditionInventoryID ?? null;
            //2021/06/15 Add Mantis: 0012093,類型為會計票貼,工料編號帶DiscountInventoryID
            else if (header.ValuationType == KGValuationTypeStringList._3)
                row.InventoryID = setup?.DiscountInventoryID ?? null;
            //2021/07/19 Add Mantis: 0012158,類型為外勞租用扣款,工料編號帶KGFognWorkerInventoryID
            else if (header.ValuationType == KGValuationTypeStringList._4)
                row.InventoryID = setup?.KGFognWorkerInventoryID ?? null;
            //2021/09/28 Add Mantis: 0012247,類型為附買回扣款,工料標號帶KGRePurchaseInventoryID
            else if (header.ValuationType == KGValuationTypeStringList._5)
                row.InventoryID = setup?.KGRePurchaseInventoryID ?? null;
            else
                row.InventoryID = setup?.KGDeductionInventoryID ?? null;

            if (row.InventoryID == null)
                sender.RaiseExceptionHandling<KGValuationDetailD.inventoryID>(row, row.InventoryID, new PXSetPropertyException("請至KG偏好設定維護資料!"));

            InventoryItem inventoryitem = PXSelect<InventoryItem,
                Where<InventoryItem.inventoryID
                , Equal<Required<InventoryItem.inventoryID>>>>
                .Select(this, row.InventoryID);
            row.InvDesc = inventoryitem?.Descr ?? null;

            if (detailA != null)
            {
                if (detailA.OrderNbr != null)
                {
                    setAmtEnable(true);
                    checkOrderNbrD(DeductionDetails.Cache, row, row.OrderNbr);
                }

            }

            //若ordernbr==null 把下面資料刪除
            if (row.OrderNbr == null)
            {
                row.Vendor = null;
                row.Uom = null;
                row.InvDesc = null;
                row.InventoryID = null;
                setAmtEnable(false);
            }


        }
        protected virtual void KGValuationDetailD_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGValuationDetailD row = (KGValuationDetailD)e.Row;
            KGValuation header = Valuations.Current;
            if (header != null)
            {
                row.ContractID = header.ContractID;
                row.Qty = header.Qty;
                row.Uom = header.Uom;
                row.UnitPrice = header.UnitPrice;
                row.Amount = header.Amount;
                row.TaxAmt = header.TaxAmt;
                row.TotalAmt = header.TotalAmt;
                row.Status = header.Status;
                row.ManageFeeAmt = header.ManageFeeAmt;
                row.ManageFeeTaxAmt = header.ManageFeeTaxAmt;
                row.ManageFeeTotalAmt = header.ManageFeeTotalAmt;

            }


        }
        protected virtual void KGValuationDetailD_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGValuationDetailD row = (KGValuationDetailD)e.Row;
            KGValuation header = Valuations.Current;
            if (row.OrderNbr != null)
            {
                //扣款顯示供應商
               POOrder poorder = 
                    PXSelect<POOrder,Where<POOrder.orderNbr,Equal<Required<POOrder.orderNbr>>>>.Select(this, row.OrderNbr);

                Vendor vendor =
                PXSelect<Vendor,Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>.Select(this, poorder.VendorID);
    
                row.Vendor = vendor.AcctName;
                    
                

                //扣款顯示工料編號+工料描述
               KGSetUp setup = PXSelect<KGSetUp>.Select(this);

                if (header.IsTaxFree == true && header.ValuationType == KGValuationTypeStringList._1)
                {
                    row.InventoryID = setup?.KGDeductionTaxFreeInventoryID ?? throw new Exception("請至KG偏好設定維護資料!");
                    InventoryItem setinventoryitemtax = PXSelect<InventoryItem,
                         Where<InventoryItem.inventoryID
                         , Equal<Required<InventoryItem.inventoryID>>>>
                         .Select(this, setup.KGDeductionTaxFreeInventoryID);

                    row.InvDesc = setinventoryitemtax?.Descr?? null;

                }
                else
                {
                    if (header.ValuationType == KGValuationTypeStringList._0)
                    {
                        row.InventoryID = setup?.KGAdditionInventoryID ?? null;
                    }
                    else if (header.ValuationType == KGValuationTypeStringList._3)
                        row.InventoryID = setup?.DiscountInventoryID ?? null;
                    else if (header.ValuationType == KGValuationTypeStringList._4)
                        row.InventoryID = setup?.KGFognWorkerInventoryID ?? null;
                    else if (header.ValuationType == KGValuationTypeStringList._5)
                        row.InventoryID = setup?.KGRePurchaseInventoryID ?? null;
                    else
                    {
                        row.InventoryID = setup?.KGDeductionInventoryID ?? null;
                    }

                    if (row.InventoryID == null)
                        sender.RaiseExceptionHandling<KGValuationDetailD.inventoryID>(row, row.InventoryID, new PXSetPropertyException("請至KG偏好設定維護資料!"));
                    InventoryItem setinventoryitem = PXSelect<InventoryItem,
                         Where<InventoryItem.inventoryID
                         , Equal<Required<InventoryItem.inventoryID>>>>
                         .Select(this, row.InventoryID);

                    row.InvDesc = setinventoryitem?.Descr??null;

                }


                //UnBilledTotalAmt顯示

                //2019/11/07 改成用方法呼叫
                /*PXResultset<POLine> setPoLine = PXSelectGroupBy<POLine,
               Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>>,
               Aggregate<GroupBy<POLine.orderNbr,
               Sum<POLine.curyUnbilledAmt,
               Sum<POLine.curyRetainageAmt>>>>>.Select(this, row.OrderNbr);
                foreach(POLine pOLine in setPoLine)
                {
                    row.UnBilledTotalAmt = pOLine.CuryUnbilledAmt + pOLine.CuryRetainageAmt;
                }*/
                CalUnBilledAmt calUnBilledAmt = new CalUnBilledAmt();
                row.UnBilledTotalAmt = calUnBilledAmt.TotalUnBilledAmt(this, row.OrderNbr);
                setAmtEnable(true);
            }
            else
            {
                setAmtEnable(false);
            }

        }
        #endregion

        #region Method
        bool setCheckThread = false;
        public void CaclPrice(decimal? qty, decimal? unitprice, string ordernbr, string ordertype)
        {
            KGValuation header = Valuations.Current;
            header.Amount = unitprice * qty;
            #region 2019/05/29把isManageTaxFree&isTaxFree拿掉,加上isManageFeeAmt
            /*if (header.IsTaxFree == true)
            {
                PXResultset<POTaxTran> Ptt = PXSelect<POTaxTran,
                            Where<POTaxTran.orderNbr,
                                Equal<Required<POTaxTran.orderNbr>>,
                            And<POTaxTran.orderType,
                                Equal<Required<POTaxTran.orderType>>>>>
                                .Select(this, ordernbr, ordertype);
                foreach (POTaxTran taxtran in Ptt)
                {
                    header.TaxAmt = decimal.Round(Convert.ToDecimal(header.Amount) * Convert.ToDecimal(taxtran.TaxRate / 100));

                }
            }
            else
            {
                header.TaxAmt = 0;
            }*/
            #endregion
            //2019/06/26把isTaxFree拿回
            if(header.IsTaxFree == true)
            {
                header.TaxAmt = 0;
            }
            else
            {
                PXResultset<POTaxTran> Ptt = PXSelect<POTaxTran,
                            Where<POTaxTran.orderNbr,
                                Equal<Required<POTaxTran.orderNbr>>,
                            And<POTaxTran.orderType,
                                Equal<Required<POTaxTran.orderType>>>>>
                                .Select(this, ordernbr, ordertype);
                foreach (POTaxTran taxtran in Ptt)
                {
                    header.TaxAmt = decimal.Round(Convert.ToDecimal(header.Amount) * Convert.ToDecimal(taxtran.TaxRate / 100), 0, MidpointRounding.AwayFromZero);

                }
            }
            
            header.TotalAmt = header.TaxAmt + header.Amount;
            if (header.IsFreeManageFeeAmt == true)
            {
                header.ManageFeeTotalAmt = 0;
                header.ManageFeeTaxAmt = 0;
                header.ManageFeeAmt = 0;
            }
            else
            {
                PXResultset<KGSetUp> setup = PXSelect<KGSetUp,
                            Where<KGSetUp.kGManageFeeTaxRate,
                                IsNotNull>>.Select(this);
                foreach (KGSetUp kgsetup in setup)
                {
                    header.ManageFeeAmt = header.Amount * (kgsetup.KGManageFeeRate / 100);
                    #region 2019/05/29把isManageTaxFree&isTaxFree拿掉,加上isManageFeeAmt
                    /*
                    if (header.IsManageTaxFree == true)
                    {
                        header.ManageFeeTaxAmt
                            =decimal.Round(Convert.ToDecimal(header.ManageFeeAmt) * (Convert.ToDecimal(kgsetup.KGManageFeeTaxRate / 100)));
                    }
                    else
                    {
                        header.ManageFeeTaxAmt = 0;
                    }
                    */
                    #endregion
                    header.ManageFeeTaxAmt
                                = decimal.Round(Convert.ToDecimal(header.ManageFeeAmt) * (Convert.ToDecimal(kgsetup.KGManageFeeTaxRate / 100)),0, MidpointRounding.AwayFromZero);
                }
                header.ManageFeeTotalAmt = header.ManageFeeAmt + header.ManageFeeTaxAmt;
            }
   
            header.DeductionAmt = header.TotalAmt + header.ManageFeeTotalAmt;
            if(header.ValuationType =="0")
            {
                //2020/04/21 博偉說加款總額不含稅額,加款總額 = Amount(不含稅金額)
                //2019/06/12 加款總額 = TotalAmt(含稅金額)
                header.AdditionAmt = header.Amount;
            }
            else if(header.ValuationType =="1" || header.ValuationType =="2" || header.ValuationType == KGValuationTypeStringList._4 || 
                header.ValuationType == KGValuationTypeStringList._5)
            {
                header.AdditionAmt = 0;
            }
            
        }
        public bool checkOrderNbrD(PXCache sender, KGValuationDetailD row, String newValue)
        {
            KGValuationDetailA detailA = AdditionDetails.Current;
            string ordernbr = (string)newValue;

            if (newValue == detailA.OrderNbr)
            {
                setCheckThread = true;

                DeductionDetails.Cache.RaiseExceptionHandling<KGValuationDetailD.orderNbr>(
                     row, newValue, new PXSetPropertyException("加款、扣款的分包契約不可以相同!"));
                row.Vendor = null;
                row.InvDesc = null;
                row.Uom = null;
                //setCheckThread = true;
                return true;
            }
            return false;
        }
        public bool checkOrderNbrA(PXCache sender, KGValuationDetailA row, String newValue)
        {
            KGValuationDetailD detailD = DeductionDetails.Current;
            string ordernbr = (string)newValue;
            if (newValue == detailD.OrderNbr)
            {
                setCheckThread = true;

                AdditionDetails.Cache.RaiseExceptionHandling<KGValuationDetailA.orderNbr>(
                     row, newValue, new PXSetPropertyException("加款、扣款的分包契約不可以相同!"));

                row.Vendor = null;
                row.InvDesc = null;
                row.Uom = null;
                //setCheckThread = true;
                return true;
            }



            return false;
        }
        public bool checkNullDetailD(PXCache sender, KGValuationDetailD row, String ordernbr)
        {
            if (ordernbr == null)
            {
                DeductionDetails.Cache.RaiseExceptionHandling<KGValuationDetailD.orderNbr>(
                     row, ordernbr, new PXSetPropertyException("分包契約不可為空值!"));
                return true;
            }

            return false;

        }
        public bool checkNullDetailA(PXCache sender, KGValuationDetailA row, String ordernbr)
        {
            if (ordernbr == null)
            {
                AdditionDetails.Cache.RaiseExceptionHandling<KGValuationDetailA.orderNbr>(
                     row, ordernbr, new PXSetPropertyException("分包契約不可為空值!"));
                return true;
            }

            return false;

        }
        public bool checkDeductionAmt(PXCache sender, KGValuation row, decimal? DeductionAmt, decimal? UnBilledTotalAmt)
        {
            KGValuationDetailD detailD = DeductionDetails.Current;
            if (DeductionAmt > UnBilledTotalAmt)
            {
                setCheckThread = true;
                DeductionDetails.Cache.RaiseExceptionHandling<KGValuationDetailD.unBilledTotalAmt>(
                     detailD, detailD.UnBilledTotalAmt, new PXSetPropertyException("「扣款總額」不能大於「可用餘額」!"));
                return true;
            }
            return false;

        }
        public bool checkNullContent(PXCache sender, KGValuation row,String newValue)
        {
            if (newValue == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.valuationContent>(
                     row, newValue, new PXSetPropertyException("原因不可為空值!"));
                return true;
            }
            return false;
        }
        /*public void checkAmtEnoughOrNot(string ordernbr,int? linenbr, int? contractid, decimal? currentamt,bool? hold)
        {
            PXResultset<POLine> PL = PXSelect<POLine,
                            Where<POLine.orderNbr,
                                Equal<Required<POLine.orderNbr>>,
                            And<POLine.projectID,
                                Equal<Required<POLine.projectID>>,
                            And<POLine.lineNbr,
                                Equal<Required<POLine.lineNbr>>,
                            And<POLine.curyUnbilledAmt,
                                Greater<Zero>,
                                And<POLine.cancelled,
                                Equal<False>>>>>>>
                                .Select(this, ordernbr, contractid,linenbr);
            foreach (POLine PoLine in PL)
            {
                PXResultset<KGDailyRenterVendor> rentervendoruse = PXSelect<KGDailyRenterVendor,
                    Where<KGDailyRenterVendor.orderNbr, Equal<Required<KGDailyRenterVendor.orderNbr>>,
                    And<KGDailyRenterVendor.contractID, Equal<Required<KGDailyRenterVendor.contractID>>,
                    And<KGDailyRenterVendor.lineNbr, Equal<Required<KGDailyRenterVendor.lineNbr>>,
                    And<KGDailyRenterVendor.status,Equal<word.o>>>>>>.Select(this,ordernbr, contractid, linenbr);
                
                foreach (KGDailyRenterVendor rentervendor in rentervendoruse)
                {
                    PXResultset<KGValuation> valuationuse = PXSelectJoinGroupBy<KGValuation,
                        InnerJoin<KGValuationDetail,On<KGValuation.valuationID,
                        Equal<KGValuationDetail.valuationID>>>,
                        Where<KGValuation.hold,Equal<True>,
                        And<KGValuation.dailyRenterCD,IsNull,
                        And<KGValuationDetail.pricingType,Equal<word.d>,
                        And<KGValuationDetail.orderType,Equal<word.r>,
                        And<KGValuationDetail.contractID,Equal<Required<KGValuationDetail.contractID>>,
                        And<KGValuationDetail.orderNbr,Equal<Required<KGValuationDetail.orderNbr>>,
                        And<KGValuationDetail.lineNbr,Equal<Required<KGValuationDetail.lineNbr>>>>>>>>>,
                        Aggregate<Sum<KGValuation.deductionAmt>>>
                        .Select(this,contractid,ordernbr,linenbr);
                    foreach(KGValuation valuation in valuationuse)
                    {
                        //POLine未開立發票金額 - 加總點工申請已用掉金額 - 加總扣款已用掉金額 - 現行畫面的金額 = Amt
                        decimal? amt = PoLine.CuryBilledAmt - rentervendor.Amount - valuation.DeductionAmt - currentamt;
                        if(amt<0)
                        {
                            hold = true;
                            throw new Exception("Amt 剩餘金額不夠，請換單選取 或 修改數量!!");
                        }
                        if(amt >=0)
                        {
                            hold = false;
                        }
                    }




                }
            }
        }*/

        public void setHeaderValueToNull()
        {
            KGValuation header = Valuations.Current;
            header.UnitPrice = null;
            header.Qty = null;
            header.Amount = 0;
            header.TaxAmt = 0;
            header.TotalAmt = 0;
            header.ManageFeeAmt = 0;
            header.ManageFeeTaxAmt = 0;
            header.ManageFeeTotalAmt = 0;
            header.AdditionAmt = null;
            header.DeductionAmt = null;
            header.Uom = null;
        }
        public void setDetailAValueToNull()
        {
            KGValuationDetailA detailA = AdditionDetails.Current;
            detailA.OrderNbr = null;
            detailA.OrderType = null;
            detailA.Uom = null;
            detailA.UnitPrice = null;
            detailA.Vendor = null;
            detailA.InvDesc = null;
            detailA.InventoryID = null;
        }
        public void setDetailDValueToNull()
        {
            KGValuationDetailD detailD = DeductionDetails.Current;
            detailD.OrderNbr = null;
            detailD.OrderType = null;
            detailD.Uom = null;
            detailD.UnitPrice = null;
            detailD.Vendor = null;
            detailD.InvDesc = null;
            detailD.InventoryID = null;
            //2019/12/11 ADD
            detailD.UnBilledTotalAmt = null;
        }
        public void setAmtEnable(bool trueorfalse)
        {
            KGValuation row = Valuations.Current;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            //2019/12/11 ADD detailA 只有在狀態是開啟&類型是代扣代付才開啟
            bool detailAEnabled = row.Status == "O" && row.ValuationType == KGValuationTypeStringList._0;
            bool detailDEnabled = row.Status == "O" && row.ValuationType !=null;
            PXUIFieldAttribute.SetEnabled<KGValuation.qty>(Valuations.Cache, row, trueorfalse);
            PXUIFieldAttribute.SetEnabled<KGValuation.unitPrice>(Valuations.Cache, row, trueorfalse);
            PXUIFieldAttribute.SetEnabled<KGValuation.isFreeManageFeeAmt>(Valuations.Cache, row, trueorfalse);
            //博偉說不需要isTaxFree Checkbox
            //PXUIFieldAttribute.SetEnabled<KGValuation.isTaxFree>(Valuations.Cache, row, trueorfalse);
            PXUIFieldAttribute.SetEnabled<KGValuation.additionAmt>(Valuations.Cache, row, trueorfalse);
            PXUIFieldAttribute.SetEnabled<KGValuation.deductionAmt>(Valuations.Cache, row, trueorfalse);
            PXUIFieldAttribute.SetEnabled<KGValuationDetailA.orderNbr>(AdditionDetails.Cache, detailA, detailAEnabled);
            PXUIFieldAttribute.SetEnabled<KGValuationDetailD.orderNbr>(DeductionDetails.Cache, detailD, detailDEnabled);

        }
        public void CheckAll()
        {
            KGValuation header = Valuations.Current;
            KGValuationDetailA detailA = AdditionDetails.Current;
            KGValuationDetailD detailD = DeductionDetails.Current;
            //Check Header
            if(header.ValuationType==null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.valuationType>(
                    header, header.ValuationType, new PXSetPropertyException("類型不可為空值!"));
            }
            if(header.Description == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.description>(
                    header, header.Description, new PXSetPropertyException("描述不可為空值!"));
            }
            if(header.UnitPrice == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.unitPrice>(
                    header, header.UnitPrice, new PXSetPropertyException("單價不可為空值!"));
            }
            if(header.Qty == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.qty>(
                    header, header.Qty, new PXSetPropertyException("數量不可為空值!"));
            }
            if (header.ValuationDate == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.valuationDate>(
                    header, header.ValuationDate, new PXSetPropertyException("發生日期不可為空值!"));
            }
            if(header.Uom == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.uom>(
                    header, header.Uom, new PXSetPropertyException("單位不可為空值!"));
            }
            if(header.ContractID == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.contractID>(
                    header, header.ContractID, new PXSetPropertyException("專案ID不可為空值!"));
            }
            if (header.IsFreeManageFeeAmt == true)
            {
                //2021/07/26 Add Mantis: 0012093
                if (header.ValuationType != KGValuationTypeStringList._3)
                    checkNullContent(Valuations.Cache, header, header.ValuationContent);
            }
            //checkDeductionAmt(Valuations.Cache, header, header.DeductionAmt, detailD.UnBilledTotalAmt);
            if (header.UnitPrice == 0 || header.UnitPrice == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.unitPrice>(
                    header, header.UnitPrice, new PXSetPropertyException("單位價格不可為0!"));
            }
            if (header.Qty == 0 || header.Qty == null)
            {
                setCheckThread = true;
                Valuations.Cache.RaiseExceptionHandling<KGValuation.qty>(
                    header, header.Qty, new PXSetPropertyException("數量不可為0!"));
            }

            //2019/11/08 ADD 扣款檢核
            CalUnBilledAmt calUnBilledAmt = new CalUnBilledAmt();
            checkDeductionAmt
                (Valuations.Cache, header, header.DeductionAmt,calUnBilledAmt.TotalUnBilledAmt(this,detailD.OrderNbr) );
            //CheckDetail
            if (header.ValuationType == "0")
            {
                if (detailA != null && detailD != null)
                {
                    if (detailA.OrderNbr != null && detailA.OrderNbr != null)
                    {
                        checkOrderNbrA(AdditionDetails.Cache, detailA, detailA.OrderNbr);
                        checkOrderNbrD(DeductionDetails.Cache, detailD, detailD.OrderNbr);
                    }
                    checkNullDetailA(AdditionDetails.Cache, detailA, detailA.OrderNbr);
                    checkNullDetailD(DeductionDetails.Cache, detailD, detailD.OrderNbr);

                }
                else if (detailA == null && detailD != null)
                {
                    throw new Exception("請檢查加款合約是否填寫完整!");
                }
                else if (detailA != null && detailD == null)
                {
                    throw new Exception("請檢查扣款合約是否填寫完整!");
                }
                else if (detailA == null && detailD == null)
                {
                    throw new Exception("請檢查加款合約及扣款合約是否填寫完整!");
                }

            }
            else if (header.ValuationType == KGValuationTypeStringList._1 || 
                header.ValuationType == KGValuationTypeStringList._2 ||
                header.ValuationType == KGValuationTypeStringList._3 ||
                header.ValuationType == KGValuationTypeStringList._4 ||
                header.ValuationType == KGValuationTypeStringList._5)
            {
                if (detailD != null)
                {
                    checkNullDetailD(DeductionDetails.Cache, detailD, detailD.OrderNbr);
                }
                else
                {
                    throw new Exception("扣款單請填完整!");
                }

            }


        }
        public override void Persist()
        {
            KGValuation header = Valuations.Current;
            if (header != null)
            {
                CheckAll();
            }
            if (setCheckThread == true)
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            }
            base.Persist();
        }
        #endregion

        /*#region Link
        //ValuationCD超連結

        public PXAction<KGValuation> ViewDailyRenterCD;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "View Valuation", Visible = false)]
        protected virtual void viewDailyRenterCD()
        {
            KGValuation row = Valuations.Current;
            // Creating the instance of the graph
            KGDailyReportEntry graph = PXGraph.CreateInstance<KGDailyReportEntry>();
            // Setting the current product for the graph
            graph.Dailys.Current = graph.Dailys.Search<KGDailyRenter.dailyRenterCD>(
            row.DailyRenterCD);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Dailys.Current != null)
            {
                throw new PXRedirectRequiredException(graph, true, "DailyRenter Details");
            }


        }
#endregion*/
    }

}