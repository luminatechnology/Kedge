using System;
using PX.Data;
using Kedge.DAC;
using System.Collections;
using System.Collections.Generic;

namespace Kedge
{
    public class KGMonthlyInspectFileAllEntry : PXGraph<KGMonthlyInspectFileEntry>
    {
        public PXSave<KGMonthlyInspectTicketLUpdateAll> Save;
        public PXFirst<KGMonthlyInspectTicketLUpdateAll> First;
        public PXPrevious<KGMonthlyInspectTicketLUpdateAll> Previous;
        public PXNext<KGMonthlyInspectTicketLUpdateAll> Next;
        public PXLast<KGMonthlyInspectTicketLUpdateAll> Last;
        /*
         public PXSelect<KGMonthlyInspectTicketLUpdateAll,
                    Equal<Optional<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionID>>
                ,And<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionLineID,
                    Equal<Optional<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionLineID>>>>> KGMonthlyInspectTicketLFiles;
     
        */
        public PXSelectJoin<KGMonthlyInspectTicketLUpdateAll,
                        LeftJoin<KGMonthlyInspectTicket, On<KGMonthlyInspectTicketLUpdateAll.monthlyInspectTicketID,
                            Equal<KGMonthlyInspectTicket.monthlyInspectTicketID>>>,
            Where<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionID,
                    Equal<Optional<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionID>>
                ,And<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionLineID,
                    Equal<Optional<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionLineID>>,
                And<KGMonthlyInspectTicket.status, Equal<KGMonthInspectionTicketStatuses.open>>>>> KGMonthlyInspectTicketLFiles;
     

        public KGMonthlyInspectFileAllEntry()
        {
            //KGMonthlyInspectTicketLFiles.AllowInsert = false;
            //KGMonthlyInspectTicketLFiles.AllowUpdate = false;

        }
        
        protected void KGMonthlyInspectTicketLUpdateAll_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectTicketLUpdateAll row = (KGMonthlyInspectTicketLUpdateAll)e.Row;
            if (row == null)
            {
                KGMonthlyInspectTicketLFiles.Cache.AllowUpdate = false;
                return;
            }
            else {
                KGMonthlyInspectTicketLFiles.Cache.AllowUpdate = true;
            }
            if (KGMonthlyInspectTicketLFiles.Current != null && KGMonthlyInspectTicketLFiles.Current.MonthlyInspectTicketLineID < 0)
            {
                KGMonthlyInspectTicketLFiles.Current.Remark = "無資料";
            }
            if (row.ViewTestResult == null)
            {
                row.ViewTestResult = row.TestResult;
            }
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketL.testResult>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketL.checkItem>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketL.remark>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            //PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketLFile.issueDesc>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            if (KGMonthlyInspectTicketLFiles.Current != null && KGMonthlyInspectTicketLFiles.Current.MonthlyInspectTicketLineID > 0)
            {
                if (KGMonthlyInspectTicketLFiles.Current.CheckItem == null)
                {
                    KGMonthlyInspectTicketL ticketL = PXSelect<KGMonthlyInspectTicketL,
                    Where<KGMonthlyInspectTicketL.monthlyInspectTicketLineID, Equal<Required<KGMonthlyInspectTicketLFile.monthlyInspectTicketLineID>>>>.Select(this, KGMonthlyInspectTicketLFiles.Current.MonthlyInspectTicketLineID);
                    if (ticketL!= null)
                    {
                        KGMonthlyInspectTicketLFiles.Current.CheckItem = ticketL.CheckItem;
                    }
                }
            }

            
        }

        
        public override void Persist()
        {
            KGMonthlyInspectTicketLUpdateAll master = KGMonthlyInspectTicketLFiles.Current;
            //刪除就返回
            if (master == null) return;
            if (master.IsDelete == true)
            {
                base.Persist();
                PXDatabase.Delete<KGMonthlyInspectTicketL>(
                        new PXDataFieldRestrict("MonthlyInspectTicketLineID", PXDbType.Int, 4, master.MonthlyInspectTicketLineID, PXComp.EQ)
                        );
                //清空畫面
                master.ReviseTestResult = null;
                master.ReviseMonthlyInspectionLineID = null;
                master.Remark = "已刪除";

                master.NoteID = null;
                master.ImageUrl = null;
                master.CreatedByID = null;
                master.CheckItem = null;
                master.IsDelete = false;
                return;
            }
            else
            {
                if (!beforeSaveCheck())
                {
                    return;
                }
                if (master.ReviseMonthlyInspectionLineID != null)
                {
                    KGMonthlyInspectionTemplateL templateL = PXSelectJoin<KGMonthlyInspectionTemplateL,
                        InnerJoin<KGMonthlyInspectionL, On<KGMonthlyInspectionL.templateLineID,
                            Equal<KGMonthlyInspectionTemplateL.templateLineID>>>,
                        Where<KGMonthlyInspectionL.monthlyInspectionLineID,
                            Equal<Required<KGMonthlyInspectionL.monthlyInspectionLineID>>>,
                        OrderBy<Asc<KGMonthlyInspectionTemplateL.templateLineID>>>.Select(this, master.ReviseMonthlyInspectionLineID);
                    master.MonthlyInspectionLineID = master.ReviseMonthlyInspectionLineID;
                    master.TemplateLineID = templateL.TemplateLineID;
                    master.CheckItem = templateL.CheckItem;
                    master.ReviseMonthlyInspectionLineID = null;
                }
                //setInsertData(master);
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    if (master.ReviseTestResult != null)
                    {

                        PXUpdate<Set<KGMonthlyInspectTicketL.testResult, Required<KGMonthlyInspectTicketL.testResult>>, KGMonthlyInspectTicketL
                        , Where<KGMonthlyInspectTicketL.monthlyInspectTicketLineID,
                        Equal<Required<KGMonthlyInspectTicketL.monthlyInspectTicketLineID>>>>.
                        Update(this, master.ReviseTestResult, master.MonthlyInspectTicketLineID);
                        master.ViewTestResult = master.ReviseTestResult;
                        master.ReviseTestResult = null;
                    }

                    ts.Complete();
                }

            }
        }
        public bool beforeSaveCheck()
        {
            bool check = true;
            return check;
        }
    }
}