using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using RCGV.GV.SYS;
using System.Threading;
using System.Threading.Tasks;

namespace RCGV.GV
{
    
	public class GVGuiWordMaint : GVBaseViewGraph<GVGuiWordMaint>
	{

        #region Action Bar
        public PXSave<GVGuiWord> Save;
        public PXCancel<GVGuiWord> Cancel;
        public PXInsert<GVGuiWord> Insert;
        public PXCopyPasteAction<GVGuiWord> CopyPaste;
        public PXDelete<GVGuiWord> Delete;
        public PXFirst<GVGuiWord> First;
        public PXPrevious<GVGuiWord> Previous;
        public PXNext<GVGuiWord> Next;
        public PXLast<GVGuiWord> Last;
        #endregion

        #region Select
        [PXImport(typeof(GVGuiWord))]
		public PXSelect<GVGuiWord> guiWords;
       
        #endregion

        #region Event Handler
        
		
        //Verify Delete Number Table have GuiWord
        protected virtual void GVGuiWord_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            GVGuiWord row = (GVGuiWord)e.Row;
            
            
            PXResultset<GVGuiNumber> setNumber = PXSelect<GVGuiNumber,
                      Where<GVGuiNumber.guiWordID,
                          Equal<Required<GVGuiNumber.guiWordID>>>>
                              .Select(this, row.GuiWordID);
           if(setNumber.Count >0)
            {
                guiWords.Ask("ERROR", "GuiWord aleady in used, can not be deleted!", MessageButtons.OK);
                guiWords.Cache.IsDirty = true;
                e.Cancel = true;
            }
            else if (sender.GetStatus(row) != PXEntryStatus.InsertedDeleted)
            {
                if (guiWords.Ask("Confirm Delete", "Are you sure?", MessageButtons.YesNo) != WebDialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }

        }
        #endregion

        
        
    }
    public static class GVTypeO
	{
		public const string O = "O";
		public class GVTypeO1 : Constant<string>
		{
			public GVTypeO1()
				: base(O)
			{
			}
		}

	}
}