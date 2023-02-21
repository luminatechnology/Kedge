using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.CT;

namespace Kedge
{
    public class KGSetting : PXGraph<KGSetting, KGSetUp>
    {
        public PXSelect<KGSetUp> setup;
        //add by louis for ProjectApproveSetup
        public PXSelect<KGApproveSetup,
            Where<KGApproveSetup.setupID,Equal<Current<KGSetUp.setupID>>>> ApproveSetup;
        public PXSelect<KGFinIntegrationSetup,
           Where<KGFinIntegrationSetup.setupID, Equal<Current<KGSetUp.setupID>>>> FinIntegrationSetup;
        public PXSelect<KGInspectionFileTemplate,
           Where<KGInspectionFileTemplate.setupID, Equal<Current<KGSetUp.setupID>>>> InspectionFileTemplates;

        #region KGApproveSetUp
        protected virtual void KGApproveSetup_ContractID_FieldVerifying(PXCache sender,PXFieldVerifyingEventArgs e)
        {
            KGApproveSetup row = (KGApproveSetup)e.Row;
            if (row == null) return;

            PXResultset<KGApproveSetup> set = PXSelect<KGApproveSetup>.Select(this);
            foreach (KGApproveSetup approveSetup in set)
            {
                if ((int?)e.NewValue == approveSetup.ContractID)
                {
                    ApproveSetup.Cache.RaiseExceptionHandling<KGApproveSetup.contractID>(
                     row, (int?)e.NewValue, new PXSetPropertyException("專案不可以重複!"));
                }
            }
        }
        protected virtual void KGApproveSetup_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGApproveSetup row = (KGApproveSetup)e.Row;
            if (row == null) return;
            PXResultset<KGApproveSetup> set = PXSelect<KGApproveSetup,
                Where<KGApproveSetup.contractID,Equal<Required<KGApproveSetup.contractID>>>>
                .Select(this,row.ContractID);
            if (set == null) return;
            
            if (set.Count > 1)
            {
                ApproveSetup.Cache.RaiseExceptionHandling<KGApproveSetup.contractID>(
                        row, row.ContractID, new PXSetPropertyException("專案不可以重複!"));

            }
            
        }
        #endregion

        #region KGFinIntegrationSetup
        protected virtual void KGFinIntegrationSetup_ContractID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGFinIntegrationSetup row = (KGFinIntegrationSetup)e.Row;
            if (row == null) return;

            PXResultset<KGFinIntegrationSetup> set = PXSelect<KGFinIntegrationSetup>.Select(this);
            foreach (KGFinIntegrationSetup finIntegrationSetup in set)
            {
                if ((int?)e.NewValue == finIntegrationSetup.ContractID)
                {
                    FinIntegrationSetup.Cache.RaiseExceptionHandling<KGFinIntegrationSetup.contractID>(
                     row, (int?)e.NewValue, new PXSetPropertyException("專案不可以重複!"));
                }
            }
        }
        protected virtual void KGFinIntegrationSetup_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGFinIntegrationSetup row = (KGFinIntegrationSetup)e.Row;
            if (row == null) return;
            PXResultset<KGFinIntegrationSetup> set = PXSelect<KGFinIntegrationSetup,
                Where<KGFinIntegrationSetup.contractID, Equal<Required<KGFinIntegrationSetup.contractID>>>>
                .Select(this, row.ContractID);
            if (set == null) return;

            if (set.Count > 1)
            {
                FinIntegrationSetup.Cache.RaiseExceptionHandling<KGFinIntegrationSetup.contractID>(
                        row, row.ContractID, new PXSetPropertyException("專案不可以重複!"));

            }

        }
        #endregion

        #region KGInspectionFileTemplate
        protected virtual void KGInspectionFileTemplate_InspectionType_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            KGInspectionFileTemplate row = (KGInspectionFileTemplate)e.Row;
            if (row == null) return;

            PXResultset<KGInspectionFileTemplate> set = PXSelect<KGInspectionFileTemplate>.Select(this);
            foreach (KGInspectionFileTemplate finIntegrationSetup in set)
            {
                if (e.NewValue.ToString() == finIntegrationSetup.InspectionType)
                {
                   InspectionFileTemplates.Cache.RaiseExceptionHandling<KGInspectionFileTemplate.inspectionType>(
                     row, e.NewValue.ToString(), new PXSetPropertyException("查核類型不可以重複!"));
                }
            }
        }
        protected virtual void KGInspectionFileTemplate_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGInspectionFileTemplate row = (KGInspectionFileTemplate)e.Row;
            if (row == null) return;
            PXResultset<KGInspectionFileTemplate> set = PXSelect<KGInspectionFileTemplate,
                Where<KGInspectionFileTemplate.inspectionType, Equal<Required<KGInspectionFileTemplate.inspectionType>>>>
                .Select(this, row.InspectionType);
            if (set == null) return;

            if (set.Count > 1)
            {
                InspectionFileTemplates.Cache.RaiseExceptionHandling<KGInspectionFileTemplate.inspectionType>(
                        row, row.InspectionType, new PXSetPropertyException("查核類型不可以重複!"));

            }

        }
        #endregion

        public override void Persist()
        {
            KGSetUp master = setup.Current;
            //代表刪除
            if (master == null)
            {
                base.Persist();
            }
            else
            {

                if (!beforeSaveCheck())
                {
                    return;        
                }
                
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    //throw new Exception();
                    ts.Complete();
                }
            }
        }

        public bool beforeSaveCheck()
        {
            bool check = true;
            KGSetUp master = setup.Current;
            KGApproveSetup approveSetup = ApproveSetup.Current;

            if (master.KGDailyRenterDateLimit == null || master.KGDailyRenterDateLimit < 0)
            {
                setup.Cache.RaiseExceptionHandling<KGSetUp.kGDailyRenterDateLimit>(
                     master, master.KGDailyRenterDateLimit, new PXSetPropertyException("點工日期限制必須大於等於0"));

                check = false;
            }
            
            return check;
        }

        
        
    }
}