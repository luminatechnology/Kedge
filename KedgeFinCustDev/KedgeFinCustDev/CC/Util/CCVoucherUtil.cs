using System;
using CC.DAC;
using Kedge.DAC;
using NM.DAC;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.PM;

namespace CC.Util
{
    /**
     * ===2021/10/05 : 0012252 & 0012253 === Althea 
     * CCAP & CCAR Add Postage AcctID & SubID
     * 
     * ===2022-03-30=== Jeff
     * Mantis [0012302 & 0012303] Add one more parameter to determine the release automation.
     **/
    public class CCVoucherUtil
    {
        #region Methods
        public static string CreateARVoucher(int AddVoucherType, CCReceivableCheck receivableCheck, bool canBeReleased = false)
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Use Select
            NMPreference setup = getSetUp(entry) ?? throw new Exception("請至'NM偏好設定',完成設定資料!");
            KGPostageSetup postageSetup = getPostageSetup(entry) ?? throw new Exception("請至'KG傳票摘要設定',完成設定資料!");
            PMTask task = getPMTaskisDefault(entry, receivableCheck.ProjectID);
            PMProject project = getProject(entry, receivableCheck.ProjectID);
            //Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();
            batch = entry.BatchModule.Insert(batch);
            batch.Module = "GL";
            batch.BranchID = receivableCheck.BranchID;
            switch (AddVoucherType)
            {
                case CCList.CCARVoucher.RELEASE:
                    //batch.DateEntered = receivableCheck.GuarReleaseDate;
                    //batch.FinPeriodID = FinPeriod((DateTime)receivableCheck.GuarReleaseDate);
                    batch.DateEntered = receivableCheck.DocDate;
                    batch.FinPeriodID = FinPeriod((DateTime)receivableCheck.DocDate);
                    batch.Description = receivableCheck.Description ?? null;
                    break;

                case CCList.CCARVoucher.REVERSE:
                    batch.DateEntered = receivableCheck.GuarReturnDate;
                    batch.FinPeriodID = FinPeriod((DateTime)receivableCheck.GuarReturnDate);
                    batch.Description = receivableCheck.Description;
                    break;
            }
            entry.BatchModule.Update(batch);

            //GLTran 
            //借方      
            GLTran tranD = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranD = entry.GLTranModuleBatNbr.Insert(tranD);
            tranD.BranchID = receivableCheck.BranchID;
            tranD.Module = "GL";
            switch (AddVoucherType)
            {
                case CCList.CCARVoucher.RELEASE:
                    tranD.AccountID = setup.ARGuarAccountID;
                    tranD.SubID = setup.APGuarSubID;
                    tranD.TranDate = receivableCheck.GuarReleaseDate;
                    tranD.TranDesc = receivableCheck.Description ?? null;
                    tranD.CuryDebitAmt = receivableCheck.GuarAmt;
                    break;

                case CCList.CCARVoucher.REVERSE:
                    tranD.AccountID = setup.GuarTicketAccountID;
                    tranD.SubID = setup.GuarTicketSubID;
                    tranD.TranDate = receivableCheck.GuarReturnDate;
                    tranD.TranDesc = receivableCheck.Description ?? null;
                    //2021/10/05 Add Mantis: 0012253
                    //貸方反轉金額改為GuarAmt+ CCPostageAmt
                    tranD.CuryDebitAmt = receivableCheck.GuarAmt + (receivableCheck.CCPostageAmt ?? 0m);
                    break;

            }
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            tranD.ProjectID = receivableCheck.ProjectID;
            if (receivableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                tranD.TaskID = task.TaskID;
            }
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            //2021/10/05 Add Mantis: 0012253 by Althea
            //借方郵資      
            if (receivableCheck.CCPostageAmt > 0 && AddVoucherType == CCList.CCARVoucher.RELEASE)
            {
                GLTran tranDPost = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
                tranDPost = entry.GLTranModuleBatNbr.Insert(tranDPost);
                tranDPost.BranchID = receivableCheck.BranchID;
                tranDPost.Module = "GL";
                tranDPost.AccountID = postageSetup.KGPostageAccountID;
                tranDPost.SubID = postageSetup.KGPostageSubID;
                tranDPost.TranDate = receivableCheck.GuarReleaseDate;
                tranDPost.TranDesc = receivableCheck.Description ?? null;
                tranDPost.CuryDebitAmt = receivableCheck.CCPostageAmt ?? 0m;
                tranDPost = entry.GLTranModuleBatNbr.Update(tranDPost);
                tranDPost.ProjectID = receivableCheck.ProjectID;
                if (receivableCheck.ProjectID != 0)
                {
                    if (task == null)
                        throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                    tranDPost.TaskID = task.TaskID;
                }
                tranDPost = entry.GLTranModuleBatNbr.Update(tranDPost);
            }

            //貸方
            GLTran tranC = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranC = entry.GLTranModuleBatNbr.Insert(tranC);
            tranC.BranchID = receivableCheck.BranchID;
            tranC.Module = "GL";
            switch (AddVoucherType)
            {
                case CCList.CCARVoucher.RELEASE:
                    tranC.AccountID = setup.GuarTicketAccountID;
                    tranC.SubID = setup.GuarTicketSubID;
                    tranC.TranDate = receivableCheck.GuarReleaseDate;
                    tranC.TranDesc = receivableCheck.Description ?? null;
                    tranC.CuryCreditAmt = receivableCheck.GuarAmt + (receivableCheck.CCPostageAmt ?? 0m);
                    break;

                case CCList.CCARVoucher.REVERSE:
                    tranC.AccountID = setup.ARGuarAccountID;
                    tranC.SubID = setup.ARGuarSubID;
                    tranC.TranDate = receivableCheck.GuarReturnDate;
                    tranC.TranDesc = receivableCheck.Description ?? null;
                    tranC.CuryCreditAmt = receivableCheck.GuarAmt;
                    break;
            }
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            tranC.ProjectID = receivableCheck.ProjectID;
            if (receivableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                tranC.TaskID = task.TaskID;
            }
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            //貸方反轉郵資
            if (receivableCheck.CCPostageAmt > 0 && AddVoucherType == CCList.CCARVoucher.REVERSE)
            {
                GLTran tranCPost = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
                tranCPost = entry.GLTranModuleBatNbr.Insert(tranCPost);
                tranCPost.BranchID = receivableCheck.BranchID;
                tranCPost.Module = "GL";
                tranCPost.AccountID = postageSetup.KGPostageAccountID;
                tranCPost.SubID = postageSetup.KGPostageSubID;
                tranCPost.TranDate = receivableCheck.GuarReleaseDate;
                tranCPost.TranDesc = receivableCheck.Description ?? null;
                tranCPost.CuryCreditAmt = receivableCheck.CCPostageAmt ?? 0m;
                tranCPost = entry.GLTranModuleBatNbr.Update(tranCPost);
                tranCPost.ProjectID = receivableCheck.ProjectID;
                if (receivableCheck.ProjectID != 0)
                {
                    if (task == null)
                        throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                    tranCPost.TaskID = task.TaskID;
                }
                tranCPost = entry.GLTranModuleBatNbr.Update(tranCPost);
            }

            //2023-01-17 Alton KED-28:EP過帳時，傳票為balamce狀態，因有排程會自動過帳造成異常，EP過帳時，傳票狀態改為Hold。
            if (canBeReleased == true)
            {
                batch.Hold = false;
                batch.Status = PX.Objects.GL.BatchStatus.Balanced;
            }
            entry.BatchModule.Update(batch);
            entry.Persist();

            if (canBeReleased == true) { entry.release.Press(); }

            batch = entry.BatchModule.Current;
            return batch.BatchNbr;
        }

        public static string CreateAPVoucher(int AddVoucherType, CCPayableCheck payableCheck, bool canBeReleased = false)
        {
            JournalEntry entry = PXGraph.CreateInstance<JournalEntry>();

            //Use Select
            NMPreference setup = getSetUp(entry) ?? throw new Exception("請至'NM偏好設定',完成設定資料!");
            KGPostageSetup postageSetup = getPostageSetup(entry) ?? throw new Exception("請至'KG傳票摘要設定',完成設定資料!");
            PMTask task = getPMTaskisDefault(entry, payableCheck.ProjectID);
            PMProject project = getProject(entry, payableCheck.ProjectID);
            //Batch
            Batch batch = (Batch)entry.BatchModule.Cache.CreateInstance();
            batch = entry.BatchModule.Insert(batch);
            batch.Module = "GL";
            batch.BranchID = payableCheck.BranchID;
            switch (AddVoucherType)
            {
                case CCList.CCAPVoucher.RELEASE:
                    batch.DateEntered = payableCheck.GuarReleaseDate;
                    batch.FinPeriodID = FinPeriod((DateTime)payableCheck.GuarReleaseDate);
                    batch.Description = payableCheck.Description ?? null;
                    break;

                case CCList.CCAPVoucher.REVERSE:
                    batch.DateEntered = payableCheck.GuarVoidDate;
                    batch.FinPeriodID = FinPeriod((DateTime)payableCheck.GuarVoidDate);
                    batch.Description = payableCheck.Description ?? null;
                    break;
            }
            entry.BatchModule.Update(batch);

            //GLTran 
            //借方      
            GLTran tranD = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranD = entry.GLTranModuleBatNbr.Insert(tranD);
            tranD.BranchID = payableCheck.BranchID;
            tranD.Module = "GL";
            switch (AddVoucherType)
            {
                case CCList.CCAPVoucher.RELEASE:
                    tranD.AccountID = setup.GuarNoteAccountID;
                    tranD.SubID = setup.GuarNoteSubID;
                    tranD.TranDate = payableCheck.GuarReleaseDate;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    tranD.CuryDebitAmt = payableCheck.GuarAmt;
                    break;

                case CCList.CCAPVoucher.REVERSE:
                    tranD.AccountID = setup.APGuarAccountID;
                    tranD.SubID = setup.APGuarSubID;
                    tranD.TranDate = payableCheck.GuarVoidDate;
                    tranD.TranDesc = payableCheck.Description ?? null;
                    tranD.CuryDebitAmt = payableCheck.GuarAmt + (payableCheck.CCPostageAmt ?? 0m);
                    break;

            }
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            tranD.ProjectID = payableCheck.ProjectID;
            if (payableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                tranD.TaskID = task.TaskID;
            }
            tranD = entry.GLTranModuleBatNbr.Update(tranD);
            //借方郵資
            if (payableCheck.CCPostageAmt > 0 && AddVoucherType == CCList.CCAPVoucher.RELEASE)
            {
                GLTran tranDPost = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
                tranDPost = entry.GLTranModuleBatNbr.Insert(tranDPost);
                tranDPost.BranchID = payableCheck.BranchID;
                tranDPost.Module = "GL";

                tranDPost.AccountID = postageSetup.KGPostageAccountID;
                tranDPost.SubID = postageSetup.KGPostageSubID;
                tranDPost.TranDate = payableCheck.GuarReleaseDate;
                tranDPost.TranDesc = payableCheck.Description ?? null;
                tranDPost.CuryDebitAmt = payableCheck.CCPostageAmt ?? 0m;
                tranDPost = entry.GLTranModuleBatNbr.Update(tranDPost);
                tranDPost.ProjectID = payableCheck.ProjectID;
                if (payableCheck.ProjectID != 0)
                {
                    if (task == null)
                        throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                    tranDPost.TaskID = task.TaskID;
                }
                tranDPost = entry.GLTranModuleBatNbr.Update(tranDPost);
            }

            //貸方
            GLTran tranC = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
            tranC = entry.GLTranModuleBatNbr.Insert(tranC);
            tranC.BranchID = payableCheck.BranchID;
            tranC.Module = "GL";
            switch (AddVoucherType)
            {
                case CCList.CCARVoucher.RELEASE:
                    tranC.AccountID = setup.APGuarAccountID;
                    tranC.SubID = setup.APGuarSubID;
                    tranC.TranDate = payableCheck.GuarReleaseDate;
                    tranC.TranDesc = payableCheck.Description ?? null;
                    tranC.CuryCreditAmt = payableCheck.GuarAmt + (payableCheck.CCPostageAmt ?? 0m);
                    break;

                case CCList.CCARVoucher.REVERSE:
                    tranC.AccountID = setup.GuarNoteAccountID;
                    tranC.SubID = setup.GuarNoteSubID;
                    tranC.TranDate = payableCheck.GuarVoidDate;
                    tranC.TranDesc = payableCheck.Description ?? null;
                    tranC.CuryCreditAmt = payableCheck.GuarAmt;
                    break;
            }
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            tranC.ProjectID = payableCheck.ProjectID;
            if (payableCheck.ProjectID != 0)
            {
                if (task == null)
                    throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                tranC.TaskID = task.TaskID;
            }
            tranC = entry.GLTranModuleBatNbr.Update(tranC);
            //貸方反轉郵資
            if (payableCheck.CCPostageAmt > 0 && AddVoucherType == CCList.CCAPVoucher.REVERSE)
            {
                GLTran tranCPost = (GLTran)entry.GLTranModuleBatNbr.Cache.CreateInstance();
                tranCPost = entry.GLTranModuleBatNbr.Insert(tranCPost);
                tranCPost.BranchID = payableCheck.BranchID;
                tranCPost.Module = "GL";
                tranCPost.AccountID = postageSetup.KGPostageAccountID;
                tranCPost.SubID = postageSetup.KGPostageSubID;
                tranCPost.TranDate = payableCheck.GuarVoidDate;
                tranCPost.TranDesc = payableCheck.Description ?? null;
                tranCPost.CuryCreditAmt = payableCheck.CCPostageAmt ?? 0m;
                tranCPost = entry.GLTranModuleBatNbr.Update(tranCPost);
                tranCPost.ProjectID = payableCheck.ProjectID;
                if (payableCheck.ProjectID != 0)
                {
                    if (task == null)
                        throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                    tranCPost.TaskID = task.TaskID;
                }
                tranCPost = entry.GLTranModuleBatNbr.Update(tranCPost);
            }

            batch.Hold = false;
            batch.Status = PX.Objects.GL.BatchStatus.Balanced;
            entry.BatchModule.Update(batch);
            entry.Persist();

            if (canBeReleased == true) { entry.release.Press(); }

            batch = entry.BatchModule.Current;
            return batch.BatchNbr;
        }

        public static string FinPeriod(DateTime Date)
        {
            string dateStr = Date.ToString("yyyyMMdd");
            string Period = dateStr.Substring(0, 4) + dateStr.Substring(4, 2);
            return Period;
        }

        public static NMPreference getSetUp(PXGraph graph)
        {
            return PXSelect<NMPreference>.Select(graph);
        }

        public static KGPostageSetup getPostageSetup(PXGraph graph)
        {
            return PXSelect<KGPostageSetup>.Select(graph);
        }

        public static PMProject getProject(PXGraph graph, int? ProjectID)
        {
            return PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>
                .Select(graph, ProjectID);
        }

        public static PMTask getPMTaskisDefault(PXGraph graph, int? ProjectID)
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
                    .Select(graph, ProjectID);
            }
        }
        #endregion
    }
}
