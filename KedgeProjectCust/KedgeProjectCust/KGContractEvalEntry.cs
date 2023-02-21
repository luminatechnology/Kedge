using Kedge.DAC;
using PX.Data;
using System.Collections;

namespace Kedge
{
    /**
     * ======2021/02/09 0011948 Edit By Althea=====
     * AP301000 估驗計價廠商評鑑, 
     * 點選KGContractEvaluation.ContractEvaluationCD開啟的畫面, 請disable delete action
     * 
     * =======20210324 Edit By Louis=====
     * modify contractEval() for change FinalScore to decmail 20210324
     **/
    public class KGContractEvalEntry : PXGraph<KGContractEvalEntry, KGContractEvaluation>
    {
        public const string verifyErrMsg = "The Evaluation Score Must Be Between 1 To 100.";
        public const string checkErrMsg  = "There is Evaluation Score Hasn't Been Filled Out.";

        public PXSelect<KGContractEvaluation> ContractEval;
        public PXSelect<KGContractEvaluationL,
                        Where<KGContractEvaluationL.contractEvaluationID, Equal<Current<KGContractEvaluation.contractEvaluationID>>>,
                        OrderBy<Asc<KGContractEvaluationL.questSeq>>> ContractEvalLine;

        public PXSetup<KGSetUp> KGSetup;

        #region Override Method
        public override void Persist()
        {
            foreach (KGContractEvaluationL row in ContractEvalLine.Cache.Inserted)
            {
                if (row.Score == null)
                {
                    throw new PXException(checkErrMsg);
                }
            }

            base.Persist();
        }
        #endregion

        #region Delegate Data View
        protected virtual IEnumerable contractEval()
        {
            KGContractEvaluation header = ContractEval.Current;

            if (header != null)
            {
                //int i = 0;
                decimal i = 0;

                foreach (KGContractEvaluationL row in ContractEvalLine.Cache.Cached)
                {
                    if (row.FinalScore == null) { continue; }
                    //modify by louis for change FinalScore to decmail 20210324
                    //i += decimal.ToInt32(row.FinalScore.Value);
                    i += row.FinalScore.Value;
                }

                header.Score = i;
            }

            yield return header;
        }
        #endregion

        #region Event Handlers
        protected void _(Events.FieldVerifying<KGContractEvaluationL.score> e)
        {
            if ((decimal)e.NewValue < 1 || (decimal)e.NewValue > 100)
            {
                throw new PXSetPropertyException(verifyErrMsg);
            }
        }

        protected void _(Events.RowSelected<KGContractEvaluation> e)
        {
            KGContractEvaluation row = e.Row as KGContractEvaluation;
            if (row == null) return;
            this.Delete.SetEnabled(false);
        }
        #endregion
    }
}