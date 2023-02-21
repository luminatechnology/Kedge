using PX.Data;
using PX.Objects.AP;
using RC.Util;

namespace PX.Objects.GL
{
    public class JournalEntry_Extension : PXGraphExtension<JournalEntry>
    {
        #region HyperLink
        public PXAction<Batch> ViewAP;
        [PXButton(CommitChanges = true)]
        [PXUIField()]
        protected virtual void viewAP()
        {
            GLTran row = Base.GLTranModuleBatNbr.Current;
            APRegister register = PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(Base, row.RefNbr);
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            graph.Document.Current = graph.Document.Search<APInvoice.refNbr>(register.RefNbr, new object[] { register.DocType });
            if (graph.Document.Current == null) return;
            new HyperLinkUtil<APInvoiceEntry>(graph.Document.Current, true);
        }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowSelected<Batch> e)
        {
            Batch batch = e.Row;
            if (batch == null) return;
            ViewAP.SetEnabled(true);
        }
        #endregion
    }
}