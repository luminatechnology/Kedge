using System;
using PX.Data;
using Kedge.DAC;

namespace Kedge
{
    public class KGSafetyHealthInspectTicketEntry : PXGraph<KGSafetyHealthInspectTicketEntry, KGSafetyHealthInspectTicket>
    {

        #region Select
        public PXSelect<KGSafetyHealthInspectTicket> SafetyTickets;
        public PXSelect<KGSafetyHealthInspectTicketL,
            Where<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID,
                Equal<Current<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID>>>> SafetyTicketLs;
        #endregion

        #region Event

        public void KGSafetyHealthInspectTicket_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SetDisabled();
        }

        public void KGSafetyHealthInspectTicket_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectTicket row = SafetyTickets.Current;
            if (row == null) return;
            if ((bool)row.Hold)
            {
                row.Status = "H";//Hold
            }
            else
            {
                row.Status = "N";//Open
            }
        }

        public void KGSafetyHealthInspectTicket_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectTicket row = SafetyTickets.Current;
            if (row == null) return;
            row.SafetyHealthInspectionID = null;
            DeleteDetail();
        }

        public void KGSafetyHealthInspectTicket_SafetyHealthInspectionID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectTicket row = SafetyTickets.Current;
            if (row == null) return;
            KGSafetyHealthInspectionTemplateH kgst =
                PXSelectJoin<KGSafetyHealthInspectionTemplateH,
                InnerJoin<KGSafetyHealthInspection, On<KGSafetyHealthInspectionTemplateH.templateHeaderID, Equal<KGSafetyHealthInspection.templateHeaderID>>>,
                Where<KGSafetyHealthInspection.safetyHealthInspectionID, Equal<Required<KGSafetyHealthInspection.safetyHealthInspectionID>>>>
                .Select(this, row.SafetyHealthInspectionID);
            if (kgst != null)
            {
                row.SegmentCD = kgst.SegmentCD;
            }
            DeleteDetail();
        }

        public void KGSafetyHealthInspectTicketL_CheckItem_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectTicket rowh = SafetyTickets.Current;
            KGSafetyHealthInspectTicketL row = SafetyTicketLs.Current;
            if (rowh == null || row == null) return;
            KGSafetyHealthInspectionTemplateL tempL = (KGSafetyHealthInspectionTemplateL)PXSelectorAttribute.Select<KGSafetyHealthInspectionTemplateL.checkItem>(sender, row);
            if (tempL != null)
            {
                KGSafetyHealthInspectionL kgsl = PXSelectJoin<KGSafetyHealthInspectionL,
                    InnerJoin<KGSafetyHealthInspectionTemplateL, On<KGSafetyHealthInspectionL.templateLineID, Equal<KGSafetyHealthInspectionTemplateL.templateLineID>>>,
                    Where<KGSafetyHealthInspectionL.safetyHealthInspectionID, Equal<Required<KGSafetyHealthInspectionL.safetyHealthInspectionID>>,
                    And<KGSafetyHealthInspectionL.templateLineID, Equal<Required<KGSafetyHealthInspectionL.templateLineID>>>>
                    >.Select(this, rowh.SafetyHealthInspectionID, tempL.TemplateLineID);
                row.SafetyHealthInspectionID = kgsl.SafetyHealthInspectionID;
                row.TemplateLineID = kgsl.TemplateLineID;
                row.LastRemark = kgsl.LastRemark;
                row.SafetyHealthInspectionLineID = kgsl.SafetyHealthInspectionLineID;
            }
        }

        public void KGSafetyHealthInspectTicketL_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspectTicketL ticketL = SafetyTicketLs.Current;
            if (ticketL == null) return;
            if (ticketL.TemplateLineID != null)
            {
                KGSafetyHealthInspectionTemplateL inspectionTemplateL =
                    PXSelect<KGSafetyHealthInspectionTemplateL,
                    Where<KGSafetyHealthInspectionTemplateL.templateLineID,
                Equal<Required<KGSafetyHealthInspectTicketL.templateLineID>>>>
                    .Select(this, ticketL.TemplateLineID);
                ticketL.CheckItem = inspectionTemplateL.CheckItem;
            }
        }
        #endregion

        #region Method

        private void DeleteDetail()
        {
            foreach (KGSafetyHealthInspectTicketL item in SafetyTicketLs.Select())
            {
                SafetyTicketLs.Delete(item);
            }
        }

        private void SetDisabled()
        {
            KGSafetyHealthInspectTicket row = SafetyTickets.Current;
            KGSafetyHealthInspectTicketL detail = SafetyTicketLs.Current;

            if (row == null) return;
            bool isNewRow = row.SafetyHealthInspectTicketID < 0;
            bool isHold = row.Status == "H";
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicket.safetyHealthInspectionID>(SafetyTickets.Cache, row, isHold && isNewRow);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicket.projectID>(SafetyTickets.Cache, row, isHold && isNewRow);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicket.hold>(SafetyTickets.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicket.checkDate>(SafetyTickets.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectTicket.inspectByID>(SafetyTickets.Cache, row, isHold);

            if (!isHold)
            {
                PXUIFieldAttribute.SetEnabled(SafetyTicketLs.Cache, null, isHold);
                Delete.SetEnabled(false);
                SafetyTicketLs.AllowInsert = false;
                SafetyTicketLs.AllowDelete = false;
                SafetyTicketLs.AllowUpdate = false;

            }

            #endregion


        }

    }
}