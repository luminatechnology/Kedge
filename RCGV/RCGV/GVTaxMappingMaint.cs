using System;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.TX;

namespace RCGV.GV
{
	public class GVTaxMappingMaint : PXGraph<GVTaxMappingMaint>
	{
		#region Buttons
		public PXCancel<GVTaxMapping> Cancel;
		//public PXCopyPasteAction<GVTaxMapping> CopyPaste;
		//public PXDelete<GVTaxMapping> Delete;
		//public PXInsert<GVTaxMapping> Insert;
		public PXSave<GVTaxMapping> Save;
		//public PXFirst<GVTaxMapping> First;
		//public PXLast<GVTaxMapping> Last;
		//public PXNext<GVTaxMapping> Next;
		//public PXPrevious<GVTaxMapping> Previous;

		#endregion
        
		#region Selects
		[PXFilterable]
		public PXSelect<GVTaxMapping> MasterView;
        #endregion

        #region GVTaxMapping Events

        protected virtual void GVTaxMapping_TaxRevisionID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            GVTaxMapping row = (GVTaxMapping)e.Row;
            if (row == null) return;
            TaxRev rev = PXSelectorAttribute.Select<GVTaxMapping.taxRevisionID>(sender, row, e.NewValue) as TaxRev;
            if (rev != null)
            {
                row.TaxRate = rev.TaxRate;
                row.TaxID = rev.TaxID;
                if (rev.TaxType.CompareTo("P") == 0)
                {
                    row.GvType = "I";
                }
                else if (rev.TaxType.CompareTo("S") == 0)
                {
                    row.GvType = "O";
                }
            }
        }
        
        #endregion
    }
}