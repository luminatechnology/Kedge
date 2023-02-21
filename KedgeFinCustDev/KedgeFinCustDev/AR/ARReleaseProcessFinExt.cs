using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.AR
{
    public class ARReleaseProcessFinExt : PXGraphExtension<ARReleaseProcess>
    {

        #region Override

        public delegate void PersistDelegate();
        [PXOverride]
        public virtual void Persist(PersistDelegate baseMethod)
        {
            baseMethod();
            //2022-12-12 YJ Issue :AR302000(ARPayment) 過帳的傳票要綁上ProjectID
            SetGLTranProject();
        }
        #endregion

        #region Method
        protected virtual void SetGLTranProject()
        {
            ARRegister doc = Base.ARDocument.Current;
            ARPayment payment = ARPayment.PK.Find(Base, doc.DocType, doc.RefNbr);
            if (payment != null)
            {
                foreach (GLTran item in GetGLTran(payment.BatchNbr)) {
                    item.ProjectID = payment.ProjectID;
                    item.TaskID = payment.TaskID;
                    PXCache<GLTran>.Update(Base,item);
                }
            }
        }
        #endregion

        #region BQL
        protected virtual PXResultset<GLTran> GetGLTran(string batchNbr) {
            return PXSelect<GLTran, 
                Where<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>>>
                .Select(Base, batchNbr);
        }
        #endregion
    }
}
