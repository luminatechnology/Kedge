using System;
using PX.Objects;
using PX.Data;
using Kedge.DAC;
using PX.Objects.AP;
using System.Collections.Generic;
using static PX.Data.PXBaseRedirectException;
using PX.Data.Licensing;
using GL =PX.Objects.GL; 
using CR =PX.Objects.CR;
using NM.Util;
using PX.Objects.PM;

namespace Kedge
{
    /*
     * ===2021/04/20 Mantis: 0012027 === Althea
     * Add Action: Voucher Report
     * 請呼叫report KG606001, 請傳入參照號碼, 開啟KG分錄預覽報表
     * ===2021/06/09 :Louis 口頭 ===Althea
     * Add Filter:OrgBAccountID = APInvoice.BranchID
     * 
     * ===2021/11/09 Mantis: 0012265 === althea
     * VoucherL.Digest 開放維護
     * 
     * ===2021/11/22 Mantis: 0012269 === Althea
     * 開放KGVoucherL.AccountID, SubID, 金額, 借貸欄位可以編輯, 並且可以新增跟修改
     * 存檔時檢核借貸總金額是否和Header一致
     * 
     * ===2021/12/13 Mantis: 0012265 === Althea
     * 對應的計價單APregister.DocType='INV' & APregiser.UsrIsTmpPayment=1,
     * 或是APregister.DocType='PPM', 
     * 或是APregister.DocType='INV' ＆ APregister.IsRetainageDocument=1, 
     * 則只能修改摘要, 不能修改其他欄位或是新增刪除.
     */
    public class KGVoucherMaint_Extension : PXGraphExtension<KGVoucherMaint>
    {

        #region Event Handlers
        protected void _(Events.RowSelected<KGVoucherH> e)
        {
            KGVoucherH voucherH = (KGVoucherH)e.Row;
            if (voucherH == null) return;
            SetUI(voucherH);
            if (voucherH.RefNbr!= null)
            {
                APRegister register = PXSelect<APRegister,
                    Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                    .Select(Base, voucherH.RefNbr);
                APRegisterFinExt finExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
                voucherH.DocDate = register.DocDate;
                voucherH.APStatus = register.Status;
                voucherH.UsrAccConfirmNbr = finExt.UsrAccConfirmNbr;
            }
        }
        protected void _(Events.RowPersisting<KGVoucherH> e)
        {
            KGVoucherH row = e.Row;
            if (row == null) return;
            CheckAmt((row.CAmt??0m), (row.DAmt??0m));
            CheckSubID();
        }
        protected void _(Events.FieldUpdated<KGVoucherL, KGVoucherL.accountID> e)
        {
            KGVoucherL row = e.Row;
            if (row == null) return;
            Base.VoucherL.Cache.SetDefaultExt<KGVoucherL.accountDesc>(row);
            Base.VoucherL.Cache.SetDefaultExt<KGVoucherL.accountCD>(row);
        }
        protected void _(Events.FieldUpdated<KGVoucherL, KGVoucherL.contractID> e)
        {
            KGVoucherL row = e.Row;
            if (row == null) return;
            Base.VoucherL.Cache.SetDefaultExt<KGVoucherL.contractCD>(row);
        }
        protected void _(Events.FieldUpdated<KGVoucherL, KGVoucherL.vendorID> e)
        {
            KGVoucherL row = e.Row;
            if (row == null) return;
            Base.VoucherL.Cache.SetDefaultExt<KGVoucherL.vendorCD>(row);
            CR.BAccount bAccount = getBAccount(row.VendorID);
            if (bAccount.Type == "EP")
            {
                row.VendorType = "E";
            }
            else
            {
                row.VendorType = "V";
            }
        }
        #endregion

        #region Action
        public PXAction<KGVoucherH> VoucherReport;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "分錄報表")]
        protected void voucherReport()
        {
            String reportID = "KG606001";
            if(Base.VoucherH.Current != null)
            {
                //APInvoice invocice = PXSelect<APInvoice, 
                //    Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>.Select(Base, Base.VoucherH.Current.RefNbr);
                //PX.Objects.CR.BAccount bAccount = PXSelectJoin<PX.Objects.CR.BAccount, 
                //    InnerJoin<GL.Branch,On<Branch.bAccountID,Equal<PX.Objects.CR.BAccount.bAccountID>>>,
                //    Where<GL.Branch.branchID, Equal<Required<APInvoice.branchID>>>>
                //    .Select(Base, invocice.BranchID);
                APRegister register = APRegister.PK.Find(Base, Base.VoucherH.Current.DocType, Base.VoucherH.Current.RefNbr);
                APRegisterFinExt registerFinExt = register.GetExtension<APRegisterFinExt>();
                Dictionary<string, string> mailParams = new Dictionary<string, string>() { 
                    ["UsrAccConfirmNbrFrom"]= registerFinExt.UsrAccConfirmNbr,
                    ["UsrAccConfirmNbrTo"] = registerFinExt.UsrAccConfirmNbr,
                    ["DateFrom"] = register.DocDate?.ToString("yyyy/MM/dd"),
                    ["DateTo"] = register.DocDate?.ToString("yyyy/MM/dd"),
                    ["CreateByID"] = null
                };
                //下面要跟你的查詢參數一樣
                //mailParams["OrgBAccountID"] = bAccount?.AcctCD?? null;
                //mailParams["RefNbrFrom"] = Base.VoucherH.Current.RefNbr;             
                //mailParams["RefNbrTo"] = Base.VoucherH.Current.RefNbr;
                throw new PXReportRequiredException(mailParams, reportID , WindowMode.NewWindow , "Report");
            }
        }
        #endregion

        #region Method
         private void SetUI(KGVoucherH header)
        {
            APRegister register = getAPRegister(header.RefNbr);
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(register);
            bool justCanEditDigest =
                (register.DocType == APDocType.Invoice &&
                    (registerExt.UsrIsTmpPayment == true || register.IsRetainageDocument == true)) ||
                (register.DocType == APDocType.Prepayment);
            bool canEdit = !(register.Status == APDocStatus.Balanced);
            if (justCanEditDigest)
            {
                PXUIFieldAttribute.SetReadOnly(Base.VoucherL.Cache, null, true);
                PXUIFieldAttribute.SetReadOnly<KGVoucherL.digest>(Base.VoucherL.Cache, null, canEdit);
                Base.VoucherL.AllowDelete = false;
                Base.VoucherL.AllowInsert = false;
            }
            else
            {
                PXUIFieldAttribute.SetReadOnly(Base.VoucherL.Cache, null, true);
                PXUIFieldAttribute.SetReadOnly<KGVoucherL.digest>(Base.VoucherL.Cache, null, canEdit);
                //2021/11/22 Mantis: 0012269 以下欄位開放維護/新增刪除修改都開放
                PXUIFieldAttribute.SetReadOnly<KGVoucherL.accountID>(Base.VoucherL.Cache, null, canEdit);
                PXUIFieldAttribute.SetReadOnly<KGVoucherLFinExt.usrSubID>(Base.VoucherL.Cache, null, canEdit);
                PXUIFieldAttribute.SetReadOnly<KGVoucherL.amt>(Base.VoucherL.Cache, null, canEdit);
                PXUIFieldAttribute.SetReadOnly<KGVoucherL.cd>(Base.VoucherL.Cache, null, canEdit);
                //modify by louis 20211216 開放ContractID可以修改
                PXUIFieldAttribute.SetReadOnly<KGVoucherL.contractID>(Base.VoucherL.Cache, null, canEdit);
                Base.VoucherL.AllowDelete = !canEdit;
                Base.VoucherL.AllowUpdate = !canEdit;
                Base.VoucherL.AllowInsert = !canEdit;
            }


            //2022-11-15 alton 當有UsrAccConfirmNbr 才可列印報表，且資訊都不可異動
            var registerFinExt = register.GetExtension<APRegisterFinExt>();
            bool hasAccConfirmNbr = registerFinExt.UsrAccConfirmNbr != null;
            VoucherReport.SetEnabled(hasAccConfirmNbr);
            Base.VoucherL.AllowDelete = Base.VoucherL.AllowInsert = Base.VoucherL.AllowUpdate = !hasAccConfirmNbr;

        }

        protected virtual void CheckSubID() {
            foreach (KGVoucherL voucherL in Base.VoucherL.Select()) {
                if (Base.VoucherL.Cache.GetStatus(voucherL) == PXEntryStatus.Deleted) continue;
                var voucherLExt = voucherL.GetExtension<KGVoucherLFinExt>();
                if (voucherLExt.UsrSubID == null)
                {
                    Base.VoucherL.Cache.RaiseExceptionHandling<KGVoucherLFinExt.usrSubID>
                        (voucherL, voucherLExt.UsrSubID, new PXSetPropertyException($"'子科目' cannot be empty.", PXErrorLevel.Error));
                }
            }
        }

        private CR.BAccount getBAccount(int? BAccountID)
        {
            return PXSelect<CR.BAccount,
                Where<CR.BAccount.bAccountID, Equal<Required<CR.BAccount.bAccountID>>>>
                .Select(Base, BAccountID);
        }
        #endregion

        #region Select Method
        private APRegister getAPRegister(string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(Base, RefNbr);
        }
        /// <summary>
        /// 檢核異動過後借貸總金額有無相符
        /// </summary>
        /// <param name="CAmt">KGVoucherH.CAmt</param>
        /// <param name="DAmt">KGVoucherH.DAmt</param>
        private void CheckAmt(decimal? CAmt, decimal? DAmt)
        {
            decimal creditamt = 0m;
            decimal debitamt = 0m;

            foreach (KGVoucherL voucherL in Base.VoucherL.Select())
            {
                KGVoucherLFinExt voucherLExt = PXCache<KGVoucherL>.GetExtension<KGVoucherLFinExt>(voucherL);
                if (voucherL.ContractID !=0 ) {
                    PMTask task = PXSelect<PMTask,
                            Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
                            And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>
                            .Select(Base, voucherL.ContractID, voucherLExt.UsrTaskID);
                    if (task == null)
                    {
                        PMTask newTask = PXSelect<PMTask,
                                         Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
                                         And<PMTask.isDefault, Equal<True>>>>
                                         .Select(Base, voucherL.ContractID);
                        if (newTask == null)
                        {
                            throw new Exception(voucherL.ContractCD.Trim() + "專案沒有設定預設任務!");
                        }
                        else
                        {
                            voucherLExt.UsrTaskID = newTask.TaskID;
                        }

                    }
                }
                

                if (voucherL.Cd == "C")
                    creditamt = creditamt + voucherL.Amt??0m;
                if (voucherL.Cd == "D")
                    debitamt = debitamt + voucherL.Amt ?? 0m;
            }

            if (CAmt != creditamt || DAmt != debitamt)
                throw new Exception("異動後的分錄借貸金額與計價單原借貸總金額不符!");
        }
        #endregion

       

    }
}