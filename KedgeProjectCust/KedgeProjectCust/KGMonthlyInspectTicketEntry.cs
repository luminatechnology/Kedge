using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.CS;

namespace Kedge
{
    public class KGMonthlyInspectTicketEntry : PXGraph<KGMonthlyInspectTicketEntry, KGMonthlyInspectTicket>
    {
        public PXSelect<KGMonthlyInspectTicket> MonthlyTickets;
        public PXSelect<KGMonthlyInspectTicketL, Where<KGMonthlyInspectTicketL.monthlyInspectTicketID,
                        Equal<Current<KGMonthlyInspectTicket.monthlyInspectTicketID>>>> MonthlyTicketLs;

        public PXSetup<KGSetUp> KGSetup;
        public void Initialize()
        {
            CopyPaste.SetEnabled(false);
            CopyPaste.SetVisible(false);

        }
        /*
        protected void KGMonthlyInspectTicketLFile_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            KGMonthlyInspectTicketLFile row = e.Row as KGMonthlyInspectTicketLFile;
            KGMonthlyInspectTicketL kgMonthlyInspectTicketL = MonthlyTicketLs.Current;
            row.MonthlyInspectionLineID = kgMonthlyInspectTicketL.MonthlyInspectionLineID;
        }*/

        //MonthlyInspectionCD
        protected void KGMonthlyInspectTicket_MonthlyInspectionID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspectTicket row = e.Row as KGMonthlyInspectTicket;
            
            if (row.MonthlyInspectionID == null)
            {
                row.SegmentCD = null;
                row.CheckYear = null;
                row.CheckMonth = null;
                //deleteLine();
            }
            else
            {
                //deleteLine();
                doMonthInspect(row);
            }
        }
        /*
        public void  deleteLine() {
            foreach (KGMonthlyInspectTicketL kgMonthlyInspectTicketL in MonthlyTicketLs.Select()) {
                MonthlyTicketLs.Delete(kgMonthlyInspectTicketL);
            }
        }
        */
        protected void KGMonthlyInspectTicket_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspectTicket row = e.Row as KGMonthlyInspectTicket;
            if (row.Hold == true)
            {
                row.Status = KGMonthInspectionTicketStatuses.Hold;
            }
            else
            {
                row.Status = KGMonthInspectionTicketStatuses.Open;
            }

        }
        protected virtual void KGMonthlyInspectTicket_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectTicket row = e.Row as KGMonthlyInspectTicket;
            if (row.MonthlyInspectionID != null && row.SegmentCD == null)
            {
                doMonthInspectMaster(row);
            }
            setEnable();
            
        }
        protected virtual void KGMonthlyInspectTicketL_CheckItem_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspectTicket master = MonthlyTickets.Current;
            KGMonthlyInspectTicketL row = e.Row as KGMonthlyInspectTicketL;
            if (row.CheckItem == null)
            {
                row.TemplateLineID = null;
                row.MonthlyInspectionLineID = null;
                row.LastMonthRemark = null;
                row.SegmentCD = null;
            }
            else {
                KGMonthlyInspectionTemplateL templateL = (KGMonthlyInspectionTemplateL)PXSelectorAttribute.Select<KGMonthlyInspectTicketL.checkItem>(sender, row, row.CheckItem);
                if (templateL != null)
                {
                    row.TemplateLineID = templateL.TemplateLineID;
                    KGMonthlyInspectionL kgMonthlyInspectionL =
                        PXSelect<KGMonthlyInspectionL, Where<KGMonthlyInspectionL.templateLineID,
                             Equal<Required<KGMonthlyInspectionL.templateLineID>>,
                          And<KGMonthlyInspectionL.monthlyInspectionID
                            , Equal<Required<KGMonthlyInspectionL.monthlyInspectionID>>>>>.Select(this, row.TemplateLineID, master.MonthlyInspectionID);
                    row.MonthlyInspectionLineID = kgMonthlyInspectionL.MonthlyInspectionLineID;
                    row.SegmentCD = templateL.SegmentCD;
                }
                row.LastMonthRemark=     getLastMonthInspactionRemark(row);
            }

        }
        protected virtual void KGMonthlyInspectTicketL_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectTicket master = MonthlyTickets.Current;
            KGMonthlyInspectTicketL row = e.Row as KGMonthlyInspectTicketL;
            if (row == null) {
                return;
            }
            setEnable();
            if (master.MonthlyInspectionID != null && row.MonthlyInspectionLineID != null && row.CheckItem == null)
            {
                KGMonthlyInspectionL kgMonthlyInspectionL = PXSelect<KGMonthlyInspectionL, Where<KGMonthlyInspectionL.monthlyInspectionLineID,
                    Equal<Required<KGMonthlyInspectionL.monthlyInspectionLineID>>>>.Select(this, row.MonthlyInspectionLineID);

                //row.SegmentCD = kgMonthlyInspectionL.SegmentCD;
                KGMonthlyInspectionTemplateL kgMonthlyInspectionTemplateL = PXSelect<KGMonthlyInspectionTemplateL, Where<KGMonthlyInspectionTemplateL.templateLineID,
                Equal<Required<KGMonthlyInspectionTemplateL.templateLineID>>>>.Select(this, kgMonthlyInspectionL.TemplateLineID);
                row.CheckItem = kgMonthlyInspectionTemplateL.CheckItem;
                row.CheckPointDesc = kgMonthlyInspectionTemplateL.CheckPointDesc;
               
            }

        }


        public void setEnable()
        {
            KGMonthlyInspectTicket master = MonthlyTickets.Current;
            bool enable = true;
            if (master.Hold == true)
            {
                enable = true;
            }
            else
            {
                enable = false;
            }
            
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicket.checkDate>(MonthlyTickets.Cache, master, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicket.inspectByID>(MonthlyTickets.Cache, master, enable);
            PXEntryStatus status = MonthlyTickets.Cache.GetStatus(master);
            //會有漏洞保存當下不會換狀態
            //if (status == PXEntryStatus.Inserted)
            if (master.MonthlyInspectTicketID<0)
            {
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicket.contractID>(MonthlyTickets.Cache, master, true);
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicket.monthlyInspectionID>(MonthlyTickets.Cache, master, true);
            }
            else {
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicket.contractID>(MonthlyTickets.Cache, master, false);
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicket.monthlyInspectionID>(MonthlyTickets.Cache, master, false);
            }
            CopyPaste.SetEnabled(false);
            CopyPaste.SetVisible(false);

            MonthlyTicketLs.AllowInsert= enable;
            MonthlyTicketLs.AllowUpdate = enable;
            MonthlyTicketLs.AllowDelete = enable;
            Delete.SetEnabled(enable);


        }

        public void doMonthInspect(KGMonthlyInspectTicket row)
        {
            doMonthInspectMaster(row);
            //doMonthInspectDetails(row);
        }
        public void doMonthInspectMaster(KGMonthlyInspectTicket row)
        {
            KGMonthlyInspection kgMonthlyInspection = PXSelect<KGMonthlyInspection, Where<KGMonthlyInspection.monthlyInspectionID,
               Equal<Required<KGMonthlyInspectTicket.monthlyInspectionID>>>>.Select(this, row.MonthlyInspectionID);
            if (kgMonthlyInspection != null)
            {
                KGMonthlyInspectionTemplateH kgMonthlyInspectionTemplateH = PXSelect<KGMonthlyInspectionTemplateH, Where<KGMonthlyInspectionTemplateH.templateHeaderID,
                    Equal<Required<KGMonthlyInspectionTemplateH.templateHeaderID>>>>.Select(this, kgMonthlyInspection.TemplateHeaderID);
                if (kgMonthlyInspectionTemplateH != null)
                {
                    row.SegmentCD = kgMonthlyInspectionTemplateH.SegmentCD;
                    row.CheckYear = kgMonthlyInspection.CheckYear;
                    row.CheckMonth = kgMonthlyInspection.CheckMonth;
                }

            }
        }
  

        public String getLastMonthInspactionRemark(KGMonthlyInspectTicketL kgMonthlyInspectTicketL)
        {
            KGMonthlyInspectTicket master = MonthlyTickets.Current;
            int month = Convert.ToInt32(master.CheckMonth)-1;
            int year = Convert.ToInt32(master.CheckYear);
            if (month == 0) {
                month = 12;
                year = year - 1;
            }
            String remark = null;
            foreach (PXResult<KGMonthlyInspectionL, KGMonthlyInspection>  result in  PXSelectJoin< KGMonthlyInspectionL,
                InnerJoin<KGMonthlyInspection, On<KGMonthlyInspection.monthlyInspectionID, Equal<KGMonthlyInspectionL.monthlyInspectionID>>, 
                InnerJoin<KGMonthlyInspectionTemplateL, On<KGMonthlyInspectionTemplateL.templateLineID, Equal<KGMonthlyInspectionL.templateLineID>>>>,
                Where<KGMonthlyInspection.checkMonth, LessEqual<Required<KGMonthlyInspection.checkMonth>>,
                    And<KGMonthlyInspection.checkYear, LessEqual<Required<KGMonthlyInspection.checkYear>>,
                    And<KGMonthlyInspectionL.segmentCD, Equal<Required<KGMonthlyInspectionL.segmentCD>>,
                    And<KGMonthlyInspectionTemplateL.checkItem,Equal<Required<KGMonthlyInspectionTemplateL.checkItem>>,
                    And<KGMonthlyInspection.projectID,Equal<Required<KGMonthlyInspection.projectID>>>>>>>,
                    OrderBy<Desc<KGMonthlyInspection.checkYear,
                            Desc<KGMonthlyInspection.checkMonth,
                            Desc<KGMonthlyInspection.checkDate>>>>>.
                            Select(this, month, year, kgMonthlyInspectTicketL.SegmentCD, kgMonthlyInspectTicketL.CheckItem, master.ContractID)) {

                KGMonthlyInspectionL kgMonthlyInspectionL = (KGMonthlyInspectionL)result;
                KGMonthlyInspection kgMonthlyInspection = (KGMonthlyInspection)result;
                remark= kgMonthlyInspectionL.Remark;
                break;
            }
            return remark;
        }
        public override void Persist()
        {
            KGMonthlyInspectTicket master = MonthlyTickets.Current;
            //代表刪除
            if (master == null)
            {
                base.Persist();
            }
            else
            {

                if (!beforeSaveCheck())
                {
                    return;
                }
                /*
                foreach (KGMonthlyInspectTicketL kgMonthlyInspectTicketL in MonthlyTicketLs.Select()) {
                    PXResultset<KGMonthlyInspectTicketLFile> set = PXSelect<KGMonthlyInspectTicketLFile, Where<KGMonthlyInspectTicketLFile.monthlyInspectTicketLineID,
                             Equal<Required<KGMonthlyInspectTicketL.monthlyInspectTicketLineID>>>>.Select(this, kgMonthlyInspectTicketL.MonthlyInspectTicketLineID);

                    if (set.Count > 0)
                    {
                        kgMonthlyInspectTicketL.IsChecked = true;
                    }
                    else {
                        kgMonthlyInspectTicketL.IsChecked = false;
                    }
                   
                    MonthlyTicketLs.Update(kgMonthlyInspectTicketL);
                }*/
                


                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    //throw new Exception();
                    ts.Complete();
                }
                setEnable();
            }
        }
        public bool beforeSaveCheck()
        {
            bool check = true;
            KGMonthlyInspectTicket master = MonthlyTickets.Current;

            //新增才卡
            if (master.MonthlyInspectTicketID < 0 || master.MonthlyInspectTicketID == null)
            {
                PXGraph graph = new PXGraph();
                
               KGMonthlyInspectTicket ticket= PXSelect < KGMonthlyInspectTicket, 
                    Where<KGMonthlyInspectTicket.monthlyInspectionID, 
                Equal<Required<KGMonthlyInspectTicket.monthlyInspectionID>>
                        , And<KGMonthlyInspectTicket.inspectByID, 
                Equal<Required<KGMonthlyInspectTicket.inspectByID>>>>>.Select(graph, master.MonthlyInspectionID, master.InspectByID);
                if (ticket != null) {
                    throw new PXException("同一檢查人員不能檢查兩次");
                }

                //master.MonthlyInspectionID 
                //master.InspectByID
            }


            foreach (KGMonthlyInspectTicketL ticketFile in MonthlyTicketLs.Cache.Inserted)
            {
                Guid[] files = PXNoteAttribute.GetFileNotes(MonthlyTicketLs.Cache, ticketFile);
                if (files.Length > 1)
                {
                    MonthlyTicketLs.Cache.RaiseExceptionHandling<KGMonthlyInspectTicketL.testResult>(
                                ticketFile, ticketFile.TestResult, new PXSetPropertyException("只能夾帶一個檔案", PXErrorLevel.RowError));
                    check = false;
                }
            }
            return check;
        }

          
    }
}