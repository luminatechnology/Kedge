using System;
using Kedge.DAC;
using PX.Data;

namespace Kedge
{
    public class KGVenEvaluationEntry : PXGraph<KGVenEvaluationEntry, KGVendorEvaluation>
    {

        #region Selector
        public PXSelect<KGVendorEvaluation> VendorEvaluation;

        [PXImport(typeof(KGVendorEvaluationQuest))]
        public PXSelect<KGVendorEvaluationQuest,
                        Where<KGVendorEvaluationQuest.evaluationID,
                          Equal<Current<KGVendorEvaluation.evaluationID>>>> VendorEvaluationQuest;
        
        #endregion

        #region Event
        #region TitleEvent
        public override void Persist()
        {
            KGVendorEvaluation record = (KGVendorEvaluation)VendorEvaluation.Current;
            PXEntryStatus status = VendorEvaluation.Cache.GetStatus(record);
            if (status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated) {
                
                int totalScore = 0;//加權總值，需等於100
                int seq = 0;//題目序號
                foreach (KGVendorEvaluationQuest item in VendorEvaluationQuest.Select())
                {
                    totalScore = totalScore + (int)item.Score;
                    seq = seq + 1;
                    item.QuestSeq = seq;
                };
                if (totalScore != 100)
                {
                    throw new Exception("加權總值有誤：" + totalScore + "/100");
                }
            }
            base.Persist();
        }
        #endregion

        #region KGVendorEvaluation
        public void KGVendorEvaluation_RowSelected(PXCache sender, PXRowSelectedEventArgs e) {
            KGVendorEvaluation row = e.Row as KGVendorEvaluation;
            FieldDisabled(row);
        }

       public void KGVendorEvaluation_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e) { 
            KGVendorEvaluation row = e.Row as KGVendorEvaluation;
            if (row == null) { return; }
            bool hold = (bool)row.Hold;
            row.Status = (hold ? "H" : "N");
        }

        #endregion

        #endregion

        #region Method
        private void FieldDisabled(KGVendorEvaluation row) {
            bool isHold = row.Hold == true;
            //header
            PXUIFieldAttribute.SetEnabled<KGVendorEvaluation.evaluationName>(VendorEvaluation.Cache, row, isHold);

            //detail
            PXUIFieldAttribute.SetEnabled(VendorEvaluationQuest.Cache, null, isHold);

            Delete.SetEnabled(isHold);
            VendorEvaluationQuest.AllowDelete = isHold;
            VendorEvaluationQuest.AllowInsert = isHold;
            VendorEvaluationQuest.AllowUpdate = isHold;
        }
        #endregion

    }
}