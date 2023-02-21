using System;
using Kedge.DAC;
using PX.Data;

namespace Kedge
{
    public class KGSafetyHealthInspectTicketLFileAllEntry : PXGraph<KGSafetyHealthInspectTicketLFileAllEntry>
    {
        #region Action
        //2019/11/21 檢視照片不可更改(全部畫面Enabled = false)
        //public PXSave<KGSafetyHealthInspectTicketLUpdateAll> Save;
        public PXFirst<KGSafetyHealthInspectTicketLUpdateAll> First;
        public PXPrevious<KGSafetyHealthInspectTicketLUpdateAll> Previous;
        public PXNext<KGSafetyHealthInspectTicketLUpdateAll> Next;
        public PXLast<KGSafetyHealthInspectTicketLUpdateAll> Last;
        #endregion

        #region selector
        public PXSelectJoin<KGSafetyHealthInspectTicketLUpdateAll,
            InnerJoin<KGSafetyHealthInspectTicket,On<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                Equal<KGSafetyHealthInspectTicketLUpdateAll.safetyHealthInspectTicketID>,
                And<KGSafetyHealthInspectTicket.status,Equal<KGSafetyHealthInspectTicket.pStatusOpen>>>>,
             Where<KGSafetyHealthInspectTicketLUpdateAll.safetyHealthInspectionLineID,
                 Equal<Optional<KGSafetyHealthInspectTicketLUpdateAll.safetyHealthInspectionLineID>>>> SafetyTickectFiles;
        #endregion

        #region event

        protected virtual void KGSafetyHealthInspectTicketLUpdateAll_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspectTicketLUpdateAll row = (KGSafetyHealthInspectTicketLUpdateAll)e.Row;
            if (SafetyTickectFiles.Current == null) return;
            if (row.ViewCategoryCD == null) row.ViewCategoryCD = row.CategoryCD;
            SetDisabled(row);

            //2020/03/09 ADD 因為之前把PXDBScalar拿掉 所以新增這一段
            if (row.TemplateLineID != null)
            {
                KGSafetyHealthInspectionTemplateL inspectionTemplateL =
                    PXSelect<KGSafetyHealthInspectionTemplateL,
                    Where<KGSafetyHealthInspectionTemplateL.templateLineID,
                    Equal<Required<KGSafetyHealthInspectTicketLUpdate.templateLineID>>>>
                    .Select(this, row.TemplateLineID);
                row.CheckItem = inspectionTemplateL.CheckItem;
            }
        }

        public override void Persist()
        {
            KGSafetyHealthInspectTicketLUpdateAll master = SafetyTickectFiles.Current;
            if (master == null) return;
            base.Persist();

            if (master.IsDelete == true)
            {
                PXDatabase.Delete<NoteDoc>(
                            new PXDataFieldRestrict<NoteDoc.noteID>(PXDbType.UniqueIdentifier, master.NoteID)
                        );

                PXDatabase.Delete<KGSafetyHealthInspectTicketL>(
                            new PXDataFieldRestrict<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketLineID>
                            (PXDbType.Int, 4, master.SafetyHealthInspectTicketLineID, PXComp.EQ)
                        );
                //清空畫面
                master.ReviseCategoryCD = null;
                master.ReviseCheckItem = null;
                master.Remark = "已刪除";
                master.SafetyHealthInspectTicketLineID = null;
                master.SafetyHealthInspectTicketID = null;
                master.NoteID = null;
                master.ImageUrl = null;
                master.CreatedByID = null;
                master.CheckItem = null;
                master.IsDelete = false;
                SetDisabled(master);
            }
            else
            {
                //預計更新之參數(預設為原值)
                int? _safetyHealthInspectionLineID = master.SafetyHealthInspectionLineID;
                int? _templateLineID = master.TemplateLineID;
                string _categoryCD = master.CategoryCD;
                bool doUpdate = false;

                if (master.ReviseCheckItem != null)
                {
                    KGSafetyHealthInspectionTemplateL tempL = (KGSafetyHealthInspectionTemplateL)PXSelectorAttribute.Select<KGSafetyHealthInspectTicketLUpdateAll.reviseCheckItem>(SafetyTickectFiles.Cache, master);
                    if (tempL != null)
                    {
                        KGSafetyHealthInspectionL kgsl = PXSelectJoin<KGSafetyHealthInspectionL,
                        InnerJoin<KGSafetyHealthInspectionTemplateL, On<KGSafetyHealthInspectionL.templateLineID, Equal<KGSafetyHealthInspectionTemplateL.templateLineID>>>,
                        Where<KGSafetyHealthInspectionL.safetyHealthInspectionID, Equal<Required<KGSafetyHealthInspectionL.safetyHealthInspectionID>>,
                        And<KGSafetyHealthInspectionL.templateLineID, Equal<Required<KGSafetyHealthInspectionL.templateLineID>>>>
                        >.Select(this, master.SafetyHealthInspectionID, tempL.TemplateLineID);

                        doUpdate = true;
                        _safetyHealthInspectionLineID = kgsl.SafetyHealthInspectionLineID;
                        _templateLineID = kgsl.TemplateLineID;
                        //修改畫面顯示資訊
                        master.CheckItem = kgsl.CheckItem;
                        master.ReviseCheckItem = null;
                    }
                }
                if (master.ReviseCategoryCD != null)
                {
                    doUpdate = true;
                    _categoryCD = master.ReviseCategoryCD;
                    //修改畫面顯示資訊
                    master.ViewCategoryCD = _categoryCD;
                    master.ReviseCategoryCD = null;
                }

                if (doUpdate)
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        PXUpdate<
                            Set<KGSafetyHealthInspectTicketL.safetyHealthInspectionLineID, Required<KGSafetyHealthInspectTicketL.safetyHealthInspectionLineID>,
                            Set<KGSafetyHealthInspectTicketL.templateLineID, Required<KGSafetyHealthInspectTicketL.templateLineID>,
                            Set<KGSafetyHealthInspectTicketL.categoryCD, Required<KGSafetyHealthInspectTicketL.categoryCD>>>>,
                            KGSafetyHealthInspectTicketL,
                            Where<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketLineID, Equal<Required<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketLineID>>>>
                            .Update(this, _safetyHealthInspectionLineID, _templateLineID, _categoryCD, master.SafetyHealthInspectTicketLineID);
                        ts.Complete();
                    }
                }
            }
        }

        #endregion

        #region Method
        private void SetDisabled(KGSafetyHealthInspectTicketL row)
        {
            /* 2019/11/21 檢視照片不可更改(全部畫面Enabled = false)
            //KGSafetyHealthInspectTicketLUpdateAll row = SafetyTickectFiles.Current;
            bool isInser = row.SafetyHealthInspectTicketLineID < 0 || row.SafetyHealthInspectTicketLineID == null;
            SafetyTickectFiles.AllowInsert = !isInser;
            SafetyTickectFiles.AllowUpdate = !isInser;
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdateAll.categoryCD>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdateAll.checkItem>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdateAll.viewCategoryCD>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdateAll.reviseCategoryCD>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdateAll.reviseCheckItem>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdateAll.isDelete>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdateAll.remark>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            */
            SafetyTickectFiles.AllowInsert = false;
            SafetyTickectFiles.AllowUpdate = false;
            PXUIFieldAttribute.SetEnabled(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, false);
        }
        #endregion

    }
}