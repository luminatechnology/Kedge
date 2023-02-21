using System;
using PX.Data;
using Kedge.DAC;

namespace Kedge
{
    public class KGContractCategoryMaint : PXGraph<KGContractCategoryMaint, KGContractCategory>
    {

        public PXSelect<KGContractCategory> KGContractCategorys;

        [PXImport(typeof(KGContractCategory))]
        public PXSelect<KGContractTag,
                    Where<KGContractTag.contractCategoryID,
                    Equal<Current<KGContractCategory.contractCategoryID>>>> KGContractTags;


        protected virtual void KGContractTag_Tagcd_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGContractTag row = (KGContractTag)e.Row;

            foreach(KGContractTag item in KGContractTags.Select())
            {
                if (item.Tagcd == null) continue;
                if (item.Tagcd.CompareTo((string)e.NewValue) == 0)
                {
                    // value to the field        
                    sender.RaiseExceptionHandling<KGContractTag.tagcd>(row, e.NewValue,
                       new PXSetPropertyException("Duplicate Tagcd.", PXErrorLevel.Error));
                    e.Cancel = true;
                }
            }
        }

        protected virtual void KGContractTag_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            if (e.Row == null) return;
            KGContractTag row = (KGContractTag)e.Row;


            KGContractDocTag reset = PXSelect<KGContractDocTag,
                Where<KGContractDocTag.tagid, Equal<Required<KGContractDocTag.tagid>>>>.SelectSingleBound(this, null, row.TagID);
            if (reset != null)
            {
                e.Cancel = true;
                throw new PXException(string.Format("{0} has been used can't destroy.", row.TagDesc));
            }
        }

        protected virtual void KGContractCategory_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGContractCategory row = (KGContractCategory)e.Row;
            if(row!=null)
            {
                row.Remind = "上傳Word範本只支援 .docx檔案格式";
            }
        }

    }
}