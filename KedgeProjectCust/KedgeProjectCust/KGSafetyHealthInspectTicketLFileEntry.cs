using System;
using PX.Data;
using Kedge.DAC;
using System.Collections;
using System.Collections.Generic;

namespace Kedge
{
    public class KGSafetyHealthInspectTicketLFileEntry : PXGraph<KGSafetyHealthInspectTicketLFileEntry>
    {
        #region Action
        public PXSave<KGSafetyHealthInspectTicketLUpdate> Save;
        public PXFirst<KGSafetyHealthInspectTicketLUpdate> First;
        public PXPrevious<KGSafetyHealthInspectTicketLUpdate> Previous;
        public PXNext<KGSafetyHealthInspectTicketLUpdate> Next;
        public PXLast<KGSafetyHealthInspectTicketLUpdate> Last;
        #endregion

        #region selector
        public PXSelectJoin<KGSafetyHealthInspectTicketLUpdate,
            LeftJoin<KGSafetyHealthInspectTicket, On<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                Equal<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectTicketID>,
                And<KGSafetyHealthInspectTicket.status, Equal<KGSafetyHealthInspectTicket.pStatusOpen>>>>,
            Where<KGSafetyHealthInspectTicketLUpdate.categoryCD,
                Equal<Optional<KGSafetyHealthInspectTicketLUpdate.categoryCD>>,
                And<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectionID,
                    Equal<Optional<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectionID>>>>> SafetyTickectFiles;

        #endregion

        #region Event

        protected virtual void KGSafetyHealthInspectTicketLUpdate_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspectTicketLUpdate row = (KGSafetyHealthInspectTicketLUpdate)e.Row;
            if (SafetyTickectFiles.Current == null) return;
            if (row.ViewCategoryCD == null) row.ViewCategoryCD = row.CategoryCD;
            //2019/12/31 �]����PXDBScalar���� �ҥH�n�[�W�o�q
            if (row.TemplateLineID != null)
            {
                KGSafetyHealthInspectionTemplateL inspectionTemplateL =
                    PXSelect<KGSafetyHealthInspectionTemplateL,
                    Where<KGSafetyHealthInspectionTemplateL.templateLineID,
                    Equal<Required<KGSafetyHealthInspectTicketLUpdate.templateLineID>>>>
                    .Select(this, row.TemplateLineID);
                row.CheckItem = inspectionTemplateL.CheckItem;
            }
            SetDisabled(row);
            if (SafetyTickectFiles.Current != null && (SafetyTickectFiles.Current.SafetyHealthInspectTicketLineID < 0 || SafetyTickectFiles.Current.SafetyHealthInspectTicketLineID == null))
            {
                row.Remark = "�L���";
                SafetyTickectFiles.AllowUpdate = false;
            }
            else
            {
                SafetyTickectFiles.AllowUpdate = true;
            }
        }

        public override void Persist()
        {
            KGSafetyHealthInspectTicketLUpdate master = SafetyTickectFiles.Current;
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
                //�M�ŵe��
                master.ReviseCategoryCD = null;
                master.ReviseCheckItem = null;
                master.Remark = "�w�R��";
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
                //�w�p��s���Ѽ�(�w�]�����)
                int? _safetyHealthInspectionLineID = master.SafetyHealthInspectionLineID;
                int? _templateLineID = master.TemplateLineID;
                string _categoryCD = master.CategoryCD;
                bool doUpdate = false;

                if (master.ReviseCheckItem != null)
                {
                    KGSafetyHealthInspectionTemplateL tempL = (KGSafetyHealthInspectionTemplateL)PXSelectorAttribute.Select<KGSafetyHealthInspectTicketLUpdate.reviseCheckItem>(SafetyTickectFiles.Cache, master);
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
                        //�ק�e����ܸ�T
                        master.CheckItem = kgsl.CheckItem;
                        master.ReviseCheckItem = null;
                    }
                }
                if (master.ReviseCategoryCD != null)
                {
                    doUpdate = true;
                    _categoryCD = master.ReviseCategoryCD;
                    //�ק�e����ܸ�T
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

            bool isInser = row.SafetyHealthInspectTicketLineID < 0 || row.SafetyHealthInspectTicketLineID == null;
            SafetyTickectFiles.AllowInsert = !isInser;
            SafetyTickectFiles.AllowUpdate = !isInser;

            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.categoryCD>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.checkItem>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.viewCategoryCD>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.reviseCategoryCD>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.reviseCheckItem>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.isDelete>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.remark>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicketLUpdate.improvementPlan>(SafetyTickectFiles.Cache, SafetyTickectFiles.Current, !isInser);
            /*
            //2020/03/13 ADD �ѨMNext���s�X�{�ǲ��{�H
            //�j�X��e���ƬO�_���Ĥ@���M�̫�@��
            KGSafetyHealthInspectTicketLUpdate  currentTicketUpdate= SafetyTickectFiles.Current;
            List<KGSafetyHealthInspectTicketLUpdate> list = new List<KGSafetyHealthInspectTicketLUpdate>();
            foreach (KGSafetyHealthInspectTicketLUpdate ticketL in SafetyTickectFiles.Select())
            {
                list.Add(ticketL);
            }
            KGSafetyHealthInspectTicketLUpdate ticketUpdateDBSelect =
                   PXSelect<KGSafetyHealthInspectTicketLUpdate,
                   Where<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectTicketLineID,
               Equal<Required<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectTicketLineID>>>>
               .Select(this, currentTicketUpdate.SafetyHealthInspectTicketLineID);

            if(ticketUpdateDBSelect.CategoryCD!=currentTicketUpdate.CategoryCD)
            {
                Last.SetEnabled(true);
                First.SetEnabled(true);
                Next.SetEnabled(true);
                Previous.SetEnabled(true);
            }
            else
            {
                int listCount = list.Count;
                //�Y�u���@�� �������s���_��
                if (listCount == 1)
                {
                    Last.SetEnabled(false);
                    First.SetEnabled(false);
                    Next.SetEnabled(false);
                    Previous.SetEnabled(false);
                }
                //�Y��e���̫�@�� ��U�@��&�̫�@�������s ���_��
                else if (row.SafetyHealthInspectTicketLineID == list[list.Count - 1].SafetyHealthInspectTicketLineID)
                {
                    Next.SetEnabled(false);
                    Last.SetEnabled(false);

                    Previous.SetEnabled(true);
                    First.SetEnabled(true);
                }
                //�Y��e���Ĥ@�� ��W�@��&�Ĥ@�������s ���_��
                else if (row.SafetyHealthInspectTicketLineID == list[0].SafetyHealthInspectTicketLineID)
                {
                    Next.SetEnabled(true);
                    Last.SetEnabled(true);

                    Previous.SetEnabled(false);
                    First.SetEnabled(false);
                }
                //��l�����}
                else
                {
                    Last.SetEnabled(true);
                    First.SetEnabled(true);
                    Next.SetEnabled(true);
                    Previous.SetEnabled(true);
                }
            }
            */


        }
        #endregion
    }

}