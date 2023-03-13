using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using RC.Util;
using System;

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
            var refNbr = row.RefNbr;
            Guid? noteID = null;
            if (refNbr.StartsWith("AP") || refNbr.StartsWith("PA"))
            {
                APRegister register = PXSelect<APRegister,
                    Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                    .Select(Base, refNbr);
                noteID = register?.NoteID;
            }
            else if (refNbr.StartsWith("AR"))
            {
                ARRegister register = PXSelect<ARRegister,
                    Where<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>>>
                    .Select(Base, refNbr);
                noteID = register?.NoteID;
            }
            if (noteID == null) return;
            EntityHelper helper = new EntityHelper(Base);
            helper.NavigateToRow(noteID.Value, PXRedirectHelper.WindowMode.NewWindow);
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