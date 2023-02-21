using PX.Data;
using PX.Objects.GL;
using RCGV.GV.DAC;

namespace RCGV.GV
{
    public class GVArGuiCmInvoiceMaintFinExt : PXGraphExtension<GVArGuiCmInvoiceMaint>
    {

        #region Event
        //mark by louis 20230107 銷項折讓單不使用傳票日期為預設的申報年月
        /**
        public virtual void _(Events.FieldUpdated<GVArGuiCmInvoice, GVArGuiCmInvoice.batchNbr> e)
        {
            if (e.Row == null) return;
            e.Cache.SetDefaultExt<GVArGuiCmInvoice.declareYear>(e.Row);
            e.Cache.SetDefaultExt<GVArGuiCmInvoice.declareMonth>(e.Row);
        }
        /**
        
        /**
        protected virtual void _(Events.FieldDefaulting<GVArGuiCmInvoice, GVArGuiCmInvoice.declareYear> e, PXFieldDefaulting baseMethod)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            
            if (row.BatchNbr != null)
            {
                var batch = GetBatch(e.Row.BatchNbr);
                if (batch == null) return;
                e.NewValue = batch.DateEntered?.Year;
                return;
            }
            
        baseMethod(e.Cache, e.Args);
        }
        **/

        //mark by louis 20230107 銷項折讓單不使用傳票日期為預設的申報年月
        /**
        protected virtual void _(Events.FieldDefaulting<GVArGuiCmInvoice, GVArGuiCmInvoice.declareMonth> e, PXFieldDefaulting baseMethod)
        {
            GVArGuiCmInvoice row = e.Row;
            if (row == null) return;
            if (row.BatchNbr != null)
            {
                var batch = GetBatch(e.Row.BatchNbr);
                if (batch == null) return;
                e.NewValue = batch.DateEntered?.Month;
                return;
            }
            baseMethod(e.Cache, e.Args);
        }
        **/
        #endregion

        #region BQL
        protected virtual Batch GetBatch(string batchNbr)
        {
            return PXSelect<Batch,
                Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>
                .Select(Base, batchNbr);
        }
        #endregion
    }
}
