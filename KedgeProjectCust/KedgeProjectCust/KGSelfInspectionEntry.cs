using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.AP;
using PX.Objects.PO;
using PX.Objects.CR;
using System.Collections.Generic;

namespace Kedge
{
    public class KGSelfInspectionEntry : PXGraph<KGSelfInspectionEntry, KGSelfInspection>
    {
        #region Construction
        public KGSelfInspectionEntry()
        {
            ActionsMenu.AddMenuAction(this.CloseAction);
            ReportsMenu.AddMenuAction(this.PrintAction);
        }
        #endregion

        #region Buttons

        public PXAction<KGSelfInspection> ActionsMenu;
        [PXButton]
        [PXUIField(DisplayName = "Actions")]
        protected virtual void actionsMenu()
        {
        }

        public PXAction<KGSelfInspection> ReportsMenu;
        [PXButton]
        [PXUIField(DisplayName = "Report")]
        protected virtual void reportsMenu()
        {
        }

        public PXAction<KGSelfInspection> CloseAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Close")]
        protected void closeAction()
        {
            KGSelfInspection item = SelfInspection.Cache.Current as KGSelfInspection;
            item.Status = KGSelfInspectionStatuses.Close;
            SelfInspection.Cache.Update(item);
        }   

        public PXAction<KGSelfInspection> PrintAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Print")]
        protected void printAction()
        {
            String reportID = "KG604001";
            KGSelfInspection row = SelfInspection.Current;
            //�令�A������ScreenID
            if (row != null)
            {
                Dictionary<string, string> reportParams = new Dictionary<string, string>();
                //�U���n��A���d�߰ѼƤ@��
                reportParams["SelfInspectionCD"] = row.SelfInspectionCD;
                throw new PXReportRequiredException(reportParams, reportID, "Report");
            }
        }
        #endregion

        #region Selector
        public PXSelect<KGSelfInspection> SelfInspection;
        public PXSelect<KGSelfInspectionLine,
            Where<KGSelfInspectionLine.selfInspectionID,
                    Equal<Current<KGSelfInspection.selfInspectionID>>>> SelfInspectionLine;
        public PXSelect<KGSelfInspectionReview,
            Where<KGSelfInspectionReview.selfInspectionID,
                    Equal<Current<KGSelfInspection.selfInspectionID>>>> SelfInspectionReview;
        #endregion

        #region Events
        #region KGSelfInspection
        //protected void KGSelfInspection_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        //{
        //    KGSelfInspection row = (KGSelfInspection)e.NewRow;
        //    KGSelfInspection originalRow = (KGSelfInspection)e.Row;
        //if (!sender.ObjectsEqual<KGSelfInspection.hold>(row, originalRow))
        //{
        //    ControlUIFieldsAttribute(row);
        //}
        //}
        public void KGSelfInspection_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGSelfInspection row = e.Row as KGSelfInspection;
            if (row == null) { return; }

            SetKGSelfInspectionTemplateHeader(sender, row);
            SetVendorName(row);
            CloseAction.SetEnabled(false);
            CopyPaste.SetEnabled(false);
            CopyPaste.SetVisible(false);
            ControlUIFieldsAttribute(row);

        }

        public void KGSelfInspection_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSelfInspection row = e.Row as KGSelfInspection;
            KGSelfInspection oldRow = e.OldValue as KGSelfInspection;
            if (row == null) { return; }
            if (!sender.ObjectsEqual<KGSelfInspection.projectID>(row, oldRow)) {
                //�M��OrderNbr������T
                row.OrderNbr = null;
                row.VendorName = null;
                row.OrderType = null;
                //�M���ˬd�H��
                //row.InspectByID = null;
            }
        }

        public void KGSelfInspection_TemplateHeaderID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSelfInspection row = e.Row as KGSelfInspection;
            if (row == null) { return; }
            PXEntryStatus status = SelfInspection.Cache.GetStatus(row);
            
            KGSelfInspectionTemplateH template = SetKGSelfInspectionTemplateHeader(sender, row);
            //�M��Detail��T
            foreach (KGSelfInspectionLine item in SelfInspectionLine.Select())
            {
                SelfInspectionLine.Delete(item);
            }
            //�M��Detail��T
            foreach (KGSelfInspectionReview item in SelfInspectionReview.Select())
            {
                SelfInspectionReview.Delete(item);
            }
            if (template != null)
            {
                PXResultset<KGSelfInspectionTemplateL> templateLines = this.GetKGSelfInspectionTemplateLines((int)row.TemplateHeaderID);
                foreach (KGSelfInspectionTemplateL templateItem in templateLines)
                {
                    KGSelfInspectionLine item = new KGSelfInspectionLine();
                    item.TemplateLineID = templateItem.TemplateLineID;
                    item.CheckItem = templateItem.CheckItem;
                    SelfInspectionLine.Cache.Insert(item);
                }
            }
        }
        public void KGSelfInspection_OrderNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSelfInspection row = e.Row as KGSelfInspection;
            if (row == null) { return; }

            if (row.OrderNbr == null) {
                row.VendorName = null;
                row.OrderType = null;
                return;
            }

            POOrder pOOrder = PXSelectorAttribute.Select<KGSelfInspection.orderNbr>(sender, row) as POOrder;
            if (pOOrder != null)
            {
                row.OrderType = pOOrder.OrderType;
                row.VendorName = GetVendorName(pOOrder.VendorID);
            }
        }

        public void KGSelfInspection_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSelfInspection row = e.Row as KGSelfInspection;
            if (row == null) { return; }

            bool hold = (bool)row.Hold;
            if (hold)
            {
                if (row.Status != KGSelfInspectionStatuses.Hold)
                {
                    row.Status = KGSelfInspectionStatuses.Hold;
                }
            }
            else
            {
                if (row.Status != KGSelfInspectionStatuses.Open)
                {
                    row.Status = KGSelfInspectionStatuses.Open;
                }
            }
        }
        #endregion

        #region KGSelfInspectionLine

        protected void KGSelfInspectionLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            KGSelfInspectionLine line = e.Row as KGSelfInspectionLine;
            if (line.TemplateLineID == null) {
                SelfInspectionLine.Delete(line);
            }
        }

            protected void KGSelfInspectionLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            KGSelfInspectionLine line = e.Row as KGSelfInspectionLine;
            KGSelfInspectionLine oleLine = e.OldRow as KGSelfInspectionLine;
            if (line == null) return;
            
            if (!sender.ObjectsEqual<KGSelfInspectionLine.testResult>(line, oleLine))
            {

                if (line.TestResult == KGSelfInspectionTestResults.Qualified ||
                    line.TestResult == KGSelfInspectionTestResults.NoItem)
                {
                    foreach (KGSelfInspectionReview item in SelfInspectionReview.Select())
                    {
                        if (line.TemplateLineID == item.TemplateLineID)
                        {
                            SelfInspectionReview.Cache.Delete(item);
                        }
                    }
                }
                else if (line.TestResult == KGSelfInspectionTestResults.Defect)
                {
                    KGSelfInspectionReview newReview = new KGSelfInspectionReview();
                    newReview.CheckItem = line.CheckItem;
                    newReview.TemplateLineID = line.TemplateLineID;
                    newReview.TestResult = line.TestResult;
                    newReview.ReviewResult = "1";
                    SelfInspectionReview.Cache.Insert(newReview);
                }
            }
        }

        #endregion

        #region KGSelfInspectionReview
        protected void KGSelfInspectionReview_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            KGSelfInspectionReview line = e.Row as KGSelfInspectionReview;
            if (line.TemplateLineID == null)
            {
                SelfInspectionReview.Delete(line);
            }
        }
        #endregion

        #endregion

        #region Methods

        private PXResultset<KGSelfInspectionTemplateL> GetKGSelfInspectionTemplateLines(int templateHeaderID)
        {
            PXResultset<KGSelfInspectionTemplateL> set = PXSelect<KGSelfInspectionTemplateL, Where<KGSelfInspectionTemplateL.templateHeaderID,
                                                    Equal<Required<KGSelfInspectionTemplateL.templateHeaderID>>>>.Select(this, templateHeaderID);
            return set;
        }

        private void SetVendorName(KGSelfInspection row) {
            //POOrder pOOrder = PXSelectorAttribute.Select<KGSelfInspection.orderNbr>(sender, row) as POOrder;
            POOrder pOOrder = PXSelect<POOrder, 
                Where<POOrder.orderNbr,Equal<Required<POOrder.orderNbr>>,
                And<POOrder.orderType,Equal<Required<POOrder.orderType>>>>>
                .Select(this,row.OrderNbr,row.OrderType);
            if (pOOrder != null)
            {
                row.VendorName = GetVendorName(pOOrder.VendorID);
            }
        }

        /**
         �z�LvendorID���ovendorName
             */
        private string GetVendorName(int? vendorID)
        {

            string vendorName = null ;
            if (vendorID !=null) {
                BAccount bAccount = PXSelect<BAccount, Where<BAccount.bAccountID,
                                                        Equal<Required<BAccount.bAccountID>>>>.Select(this, vendorID);
                if (bAccount!=null) {
                    vendorName = bAccount.AcctName;
                }
            }
            return vendorName;
        }

        private KGSelfInspectionTemplateH SetKGSelfInspectionTemplateHeader(PXCache sender, KGSelfInspection row)
        {
            if (row.TemplateHeaderID == null) {
                row.SegmentCD = null;
                row.SegmentDesc = null;
                row.Version = null;
                return null;
            };

            //SegmentCD & SegmentDesc & Version��L�����A�a�X
            //KGSelfInspectionTemplateH template = PXSelectorAttribute.Select<KGSelfInspectionTemplateH.templateHeaderID>(sender, row) as KGSelfInspectionTemplateH;
            KGSelfInspectionTemplateH template = PXSelect<KGSelfInspectionTemplateH, 
                Where<KGSelfInspectionTemplateH.templateHeaderID,Equal<Required<KGSelfInspectionTemplateH.templateHeaderID>>>>.Select(this, row.TemplateHeaderID);
            if (template == null) return null;

            row.SegmentCD = template.SegmentCD;
            row.SegmentDesc = template.SegmentDesc;
            row.Version = template.Version;

            return template;
        }

        /**
         * 
         * 1.Hold=1�BStatus= O ���O�@�}�l���w�]��
         * 2.Hold=0�ɡA�NStatus �ܦ��uP�v(Pending)
         *  2.1. ��Status=P�ɡA[����/�C�L] �P [���/����] Button�~��Enable
         *  2.2. ��Status=P�ɡA�N�i�H���@����쳣Disable
         *    (���FHold��user���i��Ϯ��S�^�ӭn���� �M
         *    �ۥD�ˬd�渹���]for�d�ߥ�)
         * 3.����U[����Button]��A�NStatus �ܦ��uC�v(Closed)
         *   3.1. ��Status=C�ɡA�N�i�H���@�����+Hold��Disable(���F�ۥD�ˬd�渹���]for�d�ߥ�)
         *   3.2. ��Status=C�ɡA[����]Button�]�nDisable
         *  (�]���S���@�i�����h����...)
         * 
         */
        private void ControlUIFieldsAttribute(KGSelfInspection row)
        {
            PXEntryStatus status = SelfInspection.Cache.GetStatus(row);
            bool isHold = row.Status == KGSelfInspectionStatuses.Hold;
            bool isClose = row.Status == KGSelfInspectionStatuses.Close;
            //Status='H' �� ���\�R�� Status = 'L' or 'C' �������\�R��
            Delete.SetEnabled(isHold);

            PXUIFieldAttribute.SetEnabled<KGSelfInspection.hold>(SelfInspection.Cache, row, row.Status != KGSelfInspectionStatuses.Close);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.projectID>(SelfInspection.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.templateHeaderID>(SelfInspection.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.checkDate>(SelfInspection.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.inspectByID>(SelfInspection.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.orderNbr>(SelfInspection.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.status>(SelfInspection.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.checkPosition>(SelfInspection.Cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspection.remark>(SelfInspection.Cache, row, isHold);

            PXUIFieldAttribute.SetEnabled(SelfInspectionLine.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled(SelfInspectionReview.Cache, null, !isClose);

            SelfInspectionLine.AllowDelete = false;
            SelfInspectionLine.AllowInsert = false;
            SelfInspectionReview.AllowDelete = false;
            SelfInspectionReview.AllowInsert = false;
            
            if (status != PXEntryStatus.Inserted)
            {
                PXUIFieldAttribute.SetEnabled<KGSelfInspection.templateHeaderID>(SelfInspection.Cache, row, false);
                PrintAction.SetEnabled(!isHold);
                CloseAction.SetEnabled(row.Status == KGSelfInspectionStatuses.Open);
            }
        }
        #endregion
    }
}