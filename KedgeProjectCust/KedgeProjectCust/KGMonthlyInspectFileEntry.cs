using System;
using PX.Data;
using Kedge.DAC;
using System.Collections;
using System.Collections.Generic;

namespace Kedge
{
    public class KGMonthlyInspectFileEntry : PXGraph<KGMonthlyInspectFileEntry>
    {
        public PXSave<KGMonthlyInspectTicketLUpdate> Save;
        public PXFirst<KGMonthlyInspectTicketLUpdate> First;
        public PXPrevious<KGMonthlyInspectTicketLUpdate> Previous;
        public PXNext<KGMonthlyInspectTicketLUpdate> Next;
        public PXLast<KGMonthlyInspectTicketLUpdate> Last;

        public PXSelectJoin<KGMonthlyInspectTicketLUpdate,
                        LeftJoin<KGMonthlyInspectTicket, On<KGMonthlyInspectTicketLUpdate.monthlyInspectTicketID,
                            Equal<KGMonthlyInspectTicket.monthlyInspectTicketID>>>,
          Where<KGMonthlyInspectTicketLUpdate.testResult,
              Equal<Optional<KGMonthlyInspectTicketLUpdate.testResult>>,
              And<KGMonthlyInspectTicketLUpdate.monthlyInspectionID,
                  Equal<Optional<KGMonthlyInspectTicketLUpdate.monthlyInspectionID>>, 
                  And<KGMonthlyInspectTicket.status, Equal<KGMonthInspectionTicketStatuses.open>>>>> KGMonthlyInspectTicketLFiles;

        public KGMonthlyInspectFileEntry()
        {
        }
        
        protected void KGMonthlyInspectTicketLUpdate_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            //KGMonthlyInspectTicketLFiles.AllowUpdate = false;

            KGMonthlyInspectTicketLUpdate row = (KGMonthlyInspectTicketLUpdate)e.Row;
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketLUpdate.checkItem>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            /*
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketL.testResult>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketL.checkItem>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketL.remark>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, true);
            */

            if (KGMonthlyInspectTicketLFiles.Current != null && KGMonthlyInspectTicketLFiles.Current.MonthlyInspectTicketLineID > 0)
            {
                if (KGMonthlyInspectTicketLFiles.Current.CheckItem == null)
                {
                    KGMonthlyInspectTicketL ticketL = PXSelect<KGMonthlyInspectTicketL,
                    Where<KGMonthlyInspectTicketL.monthlyInspectTicketLineID, Equal<Required<KGMonthlyInspectTicketLFile.monthlyInspectTicketLineID>>>>.Select(this, KGMonthlyInspectTicketLFiles.Current.MonthlyInspectTicketLineID);
                    if (ticketL != null)
                    {
                        KGMonthlyInspectTicketLFiles.Current.CheckItem = ticketL.CheckItem;
                    }
                }
            }
            if (row == null) {
                return;
            }
            if (KGMonthlyInspectTicketLFiles.Current != null && ( KGMonthlyInspectTicketLFiles.Current.MonthlyInspectTicketLineID < 0 || KGMonthlyInspectTicketLFiles.Current.MonthlyInspectTicketLineID ==null))
            {
                row.Remark = "無資料";
                KGMonthlyInspectTicketLFiles.AllowUpdate = false;
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketLUpdate.testResult>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
                
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketLUpdate.remark>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, false);
            }
            else {
                KGMonthlyInspectTicketLFiles.AllowUpdate = true;
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketLUpdate.testResult>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, true);
                
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspectTicketLUpdate.remark>(KGMonthlyInspectTicketLFiles.Cache, KGMonthlyInspectTicketLFiles.Current, true);
            }
            if (row.ViewTestResult == null) {
                row.ViewTestResult = row.TestResult;
            } 
            
        }

        
        public override void Persist()
        {
            KGMonthlyInspectTicketLUpdate master = KGMonthlyInspectTicketLFiles.Current;
            //刪除就返回
            if (master == null) return;
            if (master.IsDelete == true) {
                base.Persist();
                PXDatabase.Delete<KGMonthlyInspectTicketL>(
                        new PXDataFieldRestrict("MonthlyInspectTicketLineID", PXDbType.Int, 4, master.MonthlyInspectTicketLineID, PXComp.EQ)
                        );

                //清空畫面
                master.ReviseTestResult = null;
                master.ReviseMonthlyInspectionLineID = null;
                master.Remark = "已刪除";
    
                master.NoteID= null;
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
                //setInsertData(master);
                if (master.ReviseMonthlyInspectionLineID != null)
                {
                    KGMonthlyInspectionTemplateL templateL=   PXSelectJoin<KGMonthlyInspectionTemplateL,
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

                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    if (master.ReviseTestResult != null)
                    {
                        PXUpdate<Set<KGMonthlyInspectTicketL.testResult, Required<KGMonthlyInspectTicketL.testResult>>, KGMonthlyInspectTicketL
                        , Where<KGMonthlyInspectTicketL.monthlyInspectTicketLineID,
                        Equal<Required<KGMonthlyInspectTicketL.monthlyInspectTicketLineID>>>>.
                        Update(this, master.ReviseTestResult, master.MonthlyInspectTicketLineID);
                        //master.TestResult = null;
                        master.ViewTestResult = master.ReviseTestResult;
                        master.ReviseTestResult = null;
                    }
                    ts.Complete();
                }
                //回塞View
            }
            
        }
        public bool beforeSaveCheck()
        {
            bool check = true;
            return check;
        }
    }
}