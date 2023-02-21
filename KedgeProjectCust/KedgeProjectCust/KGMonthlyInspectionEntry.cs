using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.IN;
using Kedge.DAC;
using Kedge.Util;
using PX.Objects.CT;

using System.Collections;

namespace Kedge
{
    public class KGMonthlyInspectTicketLOne : KGMonthlyInspectTicketL {

    }

    public class KGMonthlyInspectionEntry : PXGraph<KGMonthlyInspectionEntry, KGMonthlyInspection>
    {
        public KGMonthlyInspectionEntry()
        {
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.CloseAction);
            this.ReportMenu.MenuAutoOpen = true;
            this.ReportMenu.AddMenuAction(this.PrintAction);
            this.ReportMenu.AddMenuAction(this.DowloadPPT);

        }

        public sealed class StatusO : Constant<String>
        {
            public StatusO() : base("O")
            { }
        }


        #region Selectors
        //add by louis
        public PXSelect<KGMonthlyInspection> MonthlyInspectionH;
        public PXSelect<KGMonthlyInspectionL,
            Where<KGMonthlyInspectionL.monthlyInspectionID,
                Equal<Current<KGMonthlyInspection.monthlyInspectionID>>>> MonthlyInspectionL;
        public PXSelect<KGMonthlyInspectionSign,
            Where<KGMonthlyInspectionSign.monthlyInspectionID,
                Equal<Current<KGMonthlyInspection.monthlyInspectionID>>>> KGMonthlyInspectionSigns;
        


        #endregion


        #region Button
        public PXAction<KGMonthlyInspection> ActionMenu;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Action")]
        protected void actionMenu() { }
        public PXAction<KGMonthlyInspection> ReportMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Report")]
        protected void reportMenu() { }



        public PXAction<KGMonthlyInspection> CloseAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Close")]
        protected void closeAction()
        {
            KGMonthlyInspection item = MonthlyInspectionH.Cache.Current as KGMonthlyInspection;
            item.Status = KGMonthInspectionStatuses.Close;
            MonthlyInspectionH.Cache.Update(item);
        }

        public PXAction<KGMonthlyInspection> PrintAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Print")]
        protected void printAction()
        {
            Actions.PressSave();
            String reportID = "KG604000";
            KGMonthlyInspection row = MonthlyInspectionH.Current;
            //改成你的報表ScreenID
            if (row != null)
            {
                Dictionary<string, string> reportParams = new Dictionary<string, string>();
                //下面要跟你的查詢參數一樣
                reportParams["MonthlyInspectionCD"] = row.MonthlyInspectionCD;
                throw new PXReportRequiredException(reportParams, reportID, "Report");
            }
        }

        #region DowloadPDF
        public PXAction<KGSafetyHealthInspection> DowloadPPT;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Dowload PPT")]
        public virtual IEnumerable dowloadPPT(PXAdapter adapter)
        {

            KGMonthlyInspection row = MonthlyInspectionH.Current;
            if (row == null || row.MonthlyInspectionID <= 0 || row.MonthlyInspectionID == null) return adapter.Get();

            //取得職安衛pptx範本NoteID
            KGInspectionFileTemplate fileTemp = GetFileTemplate("0");//職安衛 : "1" 月查核0
             
            if (fileTemp == null) return adapter.Get();
            PX.SM.UploadFileRevision fileR = KGPptTemplateUtil.GetUploadFileRevision(this, fileTemp.NoteID);
            if (fileR == null) return adapter.Get();
            Contract contract = PXSelect<Contract,
                            Where<Contract.contractID,
                            Equal<Required<Contract.contractID>>>>
                            .Select(this, row.ProjectID);

           
            //開始產生ppt
            KGPptTemplateUtil pptUtil = new KGPptTemplateUtil(fileR.Data);
            //Accessinfo.CompanyName
            String companyName = "根基營造股份有限公司";
            String projectCD = "";
            String projectDesc = "";
            if (contract !=null){
                projectCD = contract.ContractCD;
                projectDesc = contract.Description;
            }

            DateTime checkdate = (DateTime)row.CheckDate;
            String checkdateStr = checkdate.ToString("yyyy/MM/dd");
            String dayMonth = checkdate.Year + "年" + checkdate.Month + "月";
            String title = companyName + " " + projectCD + "-" + projectDesc + " " + dayMonth + " 月查核";
            PXResultset<KGMonthlyInspectTicketLUpdate> set = PXSelect<KGMonthlyInspectTicketLUpdate,
                        Where<KGMonthlyInspectTicketLUpdate.monthlyInspectionID,
                            Equal<Required<KGMonthlyInspectTicketLUpdate.monthlyInspectionID>>,
                         And<Where<KGMonthlyInspectTicketLUpdate.testResult,
                                Equal<Required<KGMonthlyInspectTicketLUpdate.testResult>>,
                             Or<KGMonthlyInspectTicketLUpdate.testResult,
                                Equal<Required<KGMonthlyInspectTicketLUpdate.testResult>>>>>>,
                        OrderBy<Desc<KGMonthlyInspectTicketLUpdate.testResult,
                               Desc<KGMonthlyInspectTicketLUpdate.checkItem>>>>.
                                 Select(this, row.MonthlyInspectionID, KGMonthInspectionResults.Defect,
                                        KGMonthInspectionResults.Observation);

            int count = 0;
            int totalCount = set.Count;
            int idx = 2;
            foreach (KGMonthlyInspectTicketLUpdate item in set)
            {
                if (idx == 2)
                {//新增畫面
                    pptUtil.NewPage();
                    idx = 0;
                    pptUtil.ReplaceTextTag("{{TitleMsg}}", title);
                }
                count = count + 1;
                int itemIdx = 1 + idx++;

                if (count == totalCount  && (totalCount%2)==1)
                {
                    pptUtil.ReplaceTextTag("{{Description2}}", "");
                    pptUtil.ReplaceTextTag("{{Date2}}", "");
                    pptUtil.ReplaceTextTag("{{CheckItem2}}", "");
                    pptUtil.ReplaceTextTag("{{Remark2}}", "");
                    if (KGMonthInspectionResults.Observation.Equals(item.TestResult))
                    {
                        pptUtil.ReplaceTextTag("{{Description1}}", "觀察事項");
                    }
                    else
                    {
                        pptUtil.ReplaceTextTag("{{Description1}}", "缺失說明");
                    }
                }
                else {
                    if (KGMonthInspectionResults.Observation.Equals(item.TestResult))
                    {
                        pptUtil.ReplaceTextTag("{{Description" + itemIdx + "}}", "觀察事項");
                    }
                    else
                    {
                        pptUtil.ReplaceTextTag("{{Description" + itemIdx + "}}", "缺失說明");
                    }
                }

                

                pptUtil.ReplaceTextTag("{{Date" + itemIdx + "}}", checkdateStr);
                pptUtil.ReplaceTextTag("{{CheckItem" + itemIdx + "}}", item.CheckItem);
                pptUtil.ReplaceTextTag("{{Remark" + itemIdx + "}}", item.Remark);

                //取得圖檔
                PX.SM.UploadFileRevision imageFile = KGPptTemplateUtil.GetUploadFileRevision(this, item.NoteID);
                if (imageFile != null)
                {
                    pptUtil.ReplaceImageTag("Image" + itemIdx, imageFile.Data, "image/jpeg");
                }

            }
            //ptt產生結束
            byte[] outBytes = pptUtil.Complete();

            //檔案下載
            throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(),
                                                   dayMonth + "月查核表.pptx",
                                                   null, outBytes
                                                   ), true);
                                                   
            return null;
        }
        private KGInspectionFileTemplate GetFileTemplate(String inspectionType)
        {
            return PXSelect<KGInspectionFileTemplate,
                Where<KGInspectionFileTemplate.inspectionType,
                    Equal<Required<KGInspectionFileTemplate.inspectionType>>>>.Select(this, inspectionType);
        }

        #endregion
        #endregion

        public void controlEnable() {
            KGMonthlyInspection row = MonthlyInspectionH.Current;
            bool holdEditable = row.Status != KGMonthInspectionStatuses.Close;
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.hold>(MonthlyInspectionH.Cache, row, holdEditable);
            bool editable = row.Status == KGMonthInspectionStatuses.Hold;
            Delete.SetEnabled(editable);
            qualifiedPhoto.SetEnabled(editable);
            importTicketResult.SetEnabled(editable);
            observedPhoto.SetEnabled(editable);
            flawPhoto.SetEnabled(editable);

            CloseAction.SetEnabled(false);
            PrintAction.SetEnabled(false);
            DowloadPPT.SetEnabled(false);
            PXEntryStatus status = MonthlyInspectionH.Cache.GetStatus(row);

            //按鈕亮暗重新控制

            if (row.Status == KGMonthInspectionStatuses.Hold)
            {
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.hold>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, true);
                setEnableColumn(true);
                PrintAction.SetEnabled(false);
                DowloadPPT.SetEnabled(false);
                CloseAction.SetEnabled(false);
            }
            else if (row.Status == KGMonthInspectionStatuses.Open)
            {
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.hold>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, true);
                setEnableColumn(false);
                PrintAction.SetEnabled(true);
                DowloadPPT.SetEnabled(true);
                CloseAction.SetEnabled(true);
            }
            else if (row.Status == KGMonthInspectionStatuses.Close)
            {
                setEnableColumn(false);
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.hold>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, false);
                PrintAction.SetEnabled(true);
                DowloadPPT.SetEnabled(true);
                CloseAction.SetEnabled(false);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.hold>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, false);
                setEnableColumn(false);
                PrintAction.SetEnabled(false);
                DowloadPPT.SetEnabled(false);
                CloseAction.SetEnabled(false);
            }
        }


        #region Events
        #region KGMonthlyInspection
        protected void KGMonthlyInspection_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            
            KGMonthlyInspection row = (KGMonthlyInspection)e.Row;
            if (row == null) return;
            controlEnable();
            if (row.Version == null && row.TemplateHeaderID != null)
            {
                KGMonthlyInspectionTemplateH template = PXSelectorAttribute.Select<KGMonthlyInspectionTemplateH.templateHeaderID>(sender, row) as KGMonthlyInspectionTemplateH;
                if (template != null)
                {
                    //row.SegmentCD = template.SegmentCD;
                    //row.SegmentDesc = template.SegmentDesc;
                    row.Version = template.Version;
                    //row.ScoreSetup = templateS.ScoreSetup;
                }
            }
            DateTime today=(DateTime) this.Accessinfo.BusinessDate;
            if (row.CheckMonth == null) {
                row.CheckMonth = today.Month;
            }
            if (row.CheckYear==null) {
                row.CheckYear = today.Year;
            }
        }

        public void setEnableColumn(bool enable)
        {
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.checkDate>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.projectID>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.remark>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.overallRating>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.templateHeaderID>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.checkYear>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.checkMonth>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.systemManager>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.equipmentSupervisor>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.siteManager>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.constructionStage>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);
            //PXUIFieldAttribute.SetEnabled<KGMonthlyInspection.safeHealthSupervisor>(MonthlyInspectionH.Cache, MonthlyInspectionH.Current, enable);

            MonthlyInspectionL.AllowDelete = enable;
            MonthlyInspectionL.AllowInsert = enable;
            MonthlyInspectionL.AllowUpdate = enable;
        }

        protected virtual void KGMonthlyInspection_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            PXGraph graph = new PXGraph();
            KGMonthlyInspection row = (KGMonthlyInspection)e.Row;
            KGMonthlyInspectTicket ticket = PXSelect<KGMonthlyInspectTicket,
                                                                          Where< KGMonthlyInspectTicket.monthlyInspectionID, Equal<Required<KGMonthlyInspectTicket.monthlyInspectionID>>>>

                                                                           .Select(graph, row.MonthlyInspectionID);
            if (ticket!=null)
            {
                throw new PXException("已開立現場檢查單, 不可刪除月查核單");

            }
        }


        protected void KGMonthlyInspection_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            KGMonthlyInspection row = (KGMonthlyInspection)e.NewRow;
            KGMonthlyInspection originalRow = (KGMonthlyInspection)e.Row;
            if (!sender.ObjectsEqual<KGMonthlyInspection.templateHeaderID>(row, originalRow))
            {
                if (row.TemplateHeaderID != null)
                {
                    AddKGMonthlyInspectionTemplateLines((int)row.TemplateHeaderID);
                }
                else
                {
                    RemoveAllKGMonthlyInspectionTemplateLines();
                }
            }
        }


        protected void KGMonthlyInspection_FinalScore_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspection row = e.Row as KGMonthlyInspection;
            if (row == null) return;

            row.Evaluation = GetEvaluation((decimal)row.FinalScore);
            row.EvaluationDesc = GetEvaluationDesc(row.Evaluation);
        }


        public static KGMonthlyInspectionTemplateS getTemplateS(KGMonthlyInspectionTemplateH template, KGMonthlyInspectionTemplateL item)
        {
            PXGraph graph = new PXGraph();
            KGMonthlyInspectionTemplateS templateS = PXSelect<KGMonthlyInspectionTemplateS,
                                                                          Where<KGMonthlyInspectionTemplateS.templateHeaderID, Equal<Required<KGMonthlyInspectionTemplateS.templateHeaderID>>
                                                                          , And<KGMonthlyInspectionTemplateS.segmentCD, Equal<Required<KGMonthlyInspectionTemplateS.segmentCD>>>>>

                                                                           .Select(graph, template.TemplateHeaderID, item.SegmentCD);
            return templateS;
        }
        public static KGMonthlyInspectionTemplateS getTemplateS(int? templateHeaderID, String segmentCD)
        {
            PXGraph graph = new PXGraph();
            KGMonthlyInspectionTemplateS templateS = PXSelect<KGMonthlyInspectionTemplateS,
                                                                          Where<KGMonthlyInspectionTemplateS.templateHeaderID, Equal<Required<KGMonthlyInspectionTemplateS.templateHeaderID>>
                                                                          , And<KGMonthlyInspectionTemplateS.segmentCD, Equal<Required<KGMonthlyInspectionTemplateS.segmentCD>>>>>

                                                                           .Select(graph, templateHeaderID, segmentCD);
            return templateS;
        }

        protected void KGMonthlyInspection_TemplateHeaderID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {

            KGMonthlyInspection row = e.Row as KGMonthlyInspection;
            if (row == null) return;

            if (row.TemplateHeaderID != null)
            {
                KGMonthlyInspectionTemplateH template = PXSelectorAttribute.Select<KGMonthlyInspectionTemplateH.templateHeaderID>(sender, row) as KGMonthlyInspectionTemplateH;

                if (template != null)
                {
                    //row.SegmentCD = template.SegmentCD;
                    //row.SegmentDesc = template.SegmentDesc;
                    row.Version = template.Version;
                    //row.ScoreSetup = templateS.ScoreSetup;
                }
            }
            else
            {
                //row.SegmentCD = null;
                //row.SegmentDesc = null;
                row.Version = null;
                //row.ScoreSetup = null;
            }
        }
        #endregion
        #region KGMonthlyInspectionL
        protected void KGMonthlyInspectionL_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectionL row = e.Row as KGMonthlyInspectionL;
            if (row == null) return;
            controlEnable();
            KGMonthlyInspection header = MonthlyInspectionH.Current as KGMonthlyInspection;
            if (header == null) return;

            if (row.EveryMissingBuckle == null || row.ScoreSetupLine == null) {
                KGMonthlyInspection master = MonthlyInspectionH.Current;
                KGMonthlyInspectionTemplateH template = this.GetKGMonthlyInspectionTemplateHeader((int)master.TemplateHeaderID);
                //PXResultset<KGMonthlyInspectionL> set = PXSelect<KGMonthlyInspectionL,
                //        Where<KGMonthlyInspectionL.segmentCD, Equal<Required<KGMonthlyInspectionL.segmentCD>>
                //        , And<KGMonthlyInspectionL.isChecked, Equal<True>>
                //        >>.Select(this, row.SegmentCD);
                PXResultset<KGMonthlyInspectionL> set = PXSelect<KGMonthlyInspectionL,
                        Where<KGMonthlyInspectionL.segmentCD, Equal<Required<KGMonthlyInspectionL.segmentCD>>
                        , And<KGMonthlyInspectionL.monthlyInspectionID, Equal<Required<KGMonthlyInspection.monthlyInspectionID>>
                        , And<KGMonthlyInspectionL.isChecked, Equal<True>>
                         >>>.Select(this, row.SegmentCD, master.MonthlyInspectionID);

                KGMonthlyInspectionTemplateS templateS = getTemplateS(template.TemplateHeaderID, row.SegmentCD);
                if (row.IsChecked == true)
                {
                    if (set.Count != 0)
                    {
                        row.ScoreSetupLine = templateS.ScoreSetup / set.Count;
                    }
                    else
                    {
                        row.ScoreSetupLine = 0;
                    }
                    if (row.MaxNoMissing != 0)
                    {
                        row.EveryMissingBuckle = (decimal)(row.ScoreSetupLine / row.MaxNoMissing);
                    }
                    else
                    {
                        row.EveryMissingBuckle = 0;
                    }

                    if (KGMonthInspectionResults.Defect.Equals(row.TestResult))
                    {
                        row.Deduction = row.MissingNum * row.EveryMissingBuckle;
                        if (row.Deduction > row.ScoreSetupLine)
                        {
                            row.Deduction = row.ScoreSetupLine;
                        }

                        row.Score = row.ScoreSetupLine - row.Deduction;
                    }
                    else
                    {
                        row.Deduction = 0;

                        row.Score = row.ScoreSetupLine - row.Deduction;
                    }

                }
                else {
                    row.ScoreSetupLine = 0;
                    row.EveryMissingBuckle = 0;
                }

            }
                

                // remark By Jerry 20191005
                /*
                KGMonthlyInspectionTemplateH template = this.GetKGMonthlyInspectionTemplateHeader((int)header.TemplateHeaderID);
                PXResultset<KGMonthlyInspectionTemplateL> templateLinesForTempS = GetKGMonthlyInspectionTemplateLines((int)header.TemplateHeaderID, row.SegmentCD);
                KGMonthlyInspectionTemplateS templateS = getTemplateS(template, templateLinesForTempS);
                PXResultset<KGMonthlyInspectionTemplateL> templateLines = GetKGMonthlyInspectionTemplateLines((int)header.TemplateHeaderID, row.SegmentCD);
                //templateLinesForTempS 不知道為何用過就不能count?
                if (templateLines.Count != 0)
                {
                    row.ScoreSetupLine = templateS.ScoreSetup / templateLines.Count;
                }
                KGMonthlyInspectionTemplateL templateLine = FilterKGMonthlyInspectionTemplateLine(row, templateLines);
                if (templateLine != null)
                {
                    row.MaxNoMissing = templateLine.MaxNoMissing;
                    if (row.ScoreSetupLine != null && row.MaxNoMissing != null && row.MaxNoMissing != 0)
                    {
                        row.EveryMissingBuckle = (decimal)(row.ScoreSetupLine / row.MaxNoMissing);
                    }

                }*/
            }
        /*
        protected void KGMonthlyInspectionL_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            calculateTotal();
        }
        protected void KGMonthlyInspectionL_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            calculateTotal();
        }
        protected void KGMonthlyInspectionL_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            calculateTotal();
        }*/
        public void calculateTotal() {
            Decimal sum = 0;
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            foreach (KGMonthlyInspectionL kgMonthlyInspectionL in MonthlyInspectionL.Select()) {
                if (kgMonthlyInspectionL.Deduction != null) {
                    sum = sum + kgMonthlyInspectionL.Deduction.Value;
                }
            }
            MonthlyInspectionH.Current.FinalScore = 100-sum;
            master.Evaluation = GetEvaluation((decimal)master.FinalScore);
            master.EvaluationDesc=GetEvaluationDesc(master.Evaluation);
            
        }


        protected void KGMonthlyInspectionL_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            KGMonthlyInspectionL oldLine = (KGMonthlyInspectionL)e.Row;
            KGMonthlyInspectionL line = (KGMonthlyInspectionL)e.NewRow;
            KGMonthlyInspection row = MonthlyInspectionH.Current as KGMonthlyInspection;

            // If the Deduction value has changed, adjust the TotalQty value    // accordingly 
            //remark by jerry 20191005
            /*
            if (!sender.ObjectsEqual<KGMonthlyInspectionL.missingNum>(line, oldLine))
            {
                if (line.MissingNum <= line.MaxNoMissing)
                {
                    line.Deduction = line.MissingNum * line.EveryMissingBuckle;
                    line.Score = line.ScoreSetupLine - line.Deduction;
                }
            }*/
        }
        public PXAction<KGMonthlyInspection> isCheckedAct;
        [PXUIField(DisplayName = "IsChecked",Visible =false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable IsCheckedAct(PXAdapter adapter)
        {
            foreach (KGMonthlyInspectionL row in MonthlyInspectionL.Select())
            {
                KGMonthlyInspection master = MonthlyInspectionH.Current;
                KGMonthlyInspectionTemplateH template = this.GetKGMonthlyInspectionTemplateHeader((int)master.TemplateHeaderID);
                //PXResultset<KGMonthlyInspectionL> set = PXSelect<KGMonthlyInspectionL,
                //        Where<KGMonthlyInspectionL.segmentCD, Equal<Required<KGMonthlyInspectionL.segmentCD>>
                //        , And<KGMonthlyInspectionL.isChecked, Equal<True>>>>.Select(this, row.SegmentCD);
                PXResultset<KGMonthlyInspectionL> set = PXSelect < KGMonthlyInspectionL,
                        Where< KGMonthlyInspectionL.segmentCD, Equal<Required<KGMonthlyInspectionL.segmentCD>>
                        , And < KGMonthlyInspectionL.monthlyInspectionID, Equal<Required<KGMonthlyInspection.monthlyInspectionID>>
                        , And < KGMonthlyInspectionL.isChecked, Equal < True >>
                         >>>.Select(this, row.SegmentCD, master.MonthlyInspectionID);

                KGMonthlyInspectionTemplateS templateS = getTemplateS(template.TemplateHeaderID, row.SegmentCD);
                if (row.IsChecked == true)
                {
                    if (set.Count != 0)
                    {
                        row.ScoreSetupLine = templateS.ScoreSetup / set.Count;
                    }
                    else
                    {
                        row.ScoreSetupLine = 0;
                    }
                    if (row.MaxNoMissing != 0)
                    {
                        row.EveryMissingBuckle = (decimal)(row.ScoreSetupLine / row.MaxNoMissing);
                    }
                    else
                    {
                        row.EveryMissingBuckle = 0;
                    }

                    if (KGMonthInspectionResults.Defect.Equals(row.TestResult))
                    {
                        row.Deduction = row.MissingNum * row.EveryMissingBuckle;
                        if (row.Deduction > row.ScoreSetupLine)
                        {
                            row.Deduction = row.ScoreSetupLine;
                        }

                        row.Score = row.ScoreSetupLine - row.Deduction;
                    }
                    else
                    {
                        row.Deduction = 0;

                        row.Score = row.ScoreSetupLine - row.Deduction;
                    }

                   

                }
                else
                {
                    row.ScoreSetupLine = 0;
                    row.EveryMissingBuckle = 0;
                    row.MissingNum = 0;
                    row.Score = 0;
                    row.EveryMissingBuckle = 0;
                    row.Deduction = 0;
                    //佩分
                    row.ScoreSetupLine = 0;
                    row.Remark = "";
                    row.TestResult = null;
                }
                MonthlyInspectionL.Update(row);
               
            }
            MonthlyInspectionL.View.RequestRefresh();
            return adapter.Get();
        }


        protected void KGMonthlyInspectionL_IsChecked_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!e.ExternalCall) {
                return;
            }
            
            KGMonthlyInspectionL kgMonthlyInspectionL = (KGMonthlyInspectionL)e.Row;
            isCheckedAct.Press();
        
        
        }
        protected void KGMonthlyInspectionL_TestResult_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (!e.ExternalCall)
            {
                return;
            }

            String originTestResult = e.OldValue as String;
            KGMonthlyInspectionL row = e.Row as KGMonthlyInspectionL;
            if (originTestResult == null && row.TestResult != null) {
                row.IsChecked = true;
                isCheckedAct.Press();
            }
            if (originTestResult != null && row.TestResult == null) {
                row.IsChecked = false;
                isCheckedAct.Press();
            }


        }
        


        protected void KGMonthlyInspectionL_MissingNum_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspectionL kgMonthlyInspectionL = (KGMonthlyInspectionL)e.Row;
            if (!e.ExternalCall) {
                return;
            }
            if (kgMonthlyInspectionL.IsChecked == true)
            {
                if (KGMonthInspectionResults.Defect.Equals(kgMonthlyInspectionL.TestResult))
                {
                    kgMonthlyInspectionL.Deduction = kgMonthlyInspectionL.MissingNum * kgMonthlyInspectionL.EveryMissingBuckle;
                    if (kgMonthlyInspectionL.Deduction > kgMonthlyInspectionL.ScoreSetupLine)
                    {
                        kgMonthlyInspectionL.Deduction = kgMonthlyInspectionL.ScoreSetupLine;
                    }
                    kgMonthlyInspectionL.Score = kgMonthlyInspectionL.ScoreSetupLine - kgMonthlyInspectionL.Deduction;
                }
                else {
                    kgMonthlyInspectionL.Deduction = 0;

                    kgMonthlyInspectionL.Score = kgMonthlyInspectionL.ScoreSetupLine - kgMonthlyInspectionL.Deduction;
                }
            }
            else
            {
                kgMonthlyInspectionL.MissingNum = 0;
                kgMonthlyInspectionL.Score = 0;
                kgMonthlyInspectionL.EveryMissingBuckle = 0;
                kgMonthlyInspectionL.Deduction = 0;
                //佩分
                kgMonthlyInspectionL.ScoreSetupLine = 0;
            }

            /*
            if (isRun == false) {
               
                computeTotal();
            }*/

        }
        protected void KGMonthlyInspection_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspection row = e.Row as KGMonthlyInspection;
            if (row == null) return;
            if (row.Hold == true)
            {
                row.Status = KGMonthInspectionStatuses.Hold;
                MonthlyInspectionH.Current.FinalScore = 0;
                row.Evaluation = null;
                row.EvaluationDesc = null;
            }
            else
            {
                computeTotal();
                calculateTotal();
                row.Status = KGMonthInspectionStatuses.Open;
            }
        }
        protected void KGMonthlyInspection_CheckMonth_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspection row = e.Row as KGMonthlyInspection;
            if (row == null || row.ProjectID == null) return;

            foreach (KGMonthlyInspectionL line in  MonthlyInspectionL.Select()) {
                line.LastMonthRemark = getLastMonthInspactionRemark(line);
                MonthlyInspectionL.Cache.Update(line);
            }
            
        }
        protected void KGMonthlyInspection_CheckYear_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspection row = e.Row as KGMonthlyInspection;
            if (row == null || row.ProjectID == null) return;
            foreach (KGMonthlyInspectionL line in MonthlyInspectionL.Select())
            {
                line.LastMonthRemark = getLastMonthInspactionRemark(line);
                MonthlyInspectionL.Cache.Update(line);
            }
        }


        protected void KGMonthlyInspectionL_MissingNum_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGMonthlyInspectionL row = e.Row as KGMonthlyInspectionL;

            int num = int.Parse(e.NewValue.ToString());

            if (num < 0)
            {
                sender.RaiseExceptionHandling<KGMonthlyInspectionL.missingNum>(row, null,
                       new PXSetPropertyException("缺失數不可小於 0", PXErrorLevel.Error));
            }
            
            if (num > row.MaxNoMissing)
            {
                sender.RaiseExceptionHandling<KGMonthlyInspectionL.missingNum>(row, null,
                  new PXSetPropertyException("MissingNum can't greater than MaxNoMissing.",
                     PXErrorLevel.Error));
                e.Cancel = true;
            }
        }
        protected void KGMonthlyInspectionL_ScoreSetupLine_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGMonthlyInspectionL row = e.Row as KGMonthlyInspectionL;
            if (row.ScoreSetupLine == null) return;

            int score = int.Parse(e.NewValue.ToString());
            if (score < 0 || score > 100)
            {
                sender.RaiseExceptionHandling<KGMonthlyInspectionL.scoreSetupLine>(row, row.ScoreSetupLine,
                  new PXSetPropertyException("ScoreSetupLine must between 0 ~ 100.",
                     PXErrorLevel.Warning));
                e.Cancel = true;
            }
        }

        protected void KGMonthlyInspectionL_Score_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGMonthlyInspectionL row = e.Row as KGMonthlyInspectionL;

        }

        protected void KGMonthlyInspectionL_Deduction_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGMonthlyInspectionL row = e.Row as KGMonthlyInspectionL;
            if (row.Deduction == null) return;

            int score = int.Parse(e.NewValue.ToString());
            if (score < 0 || score > 100)
            {
                sender.RaiseExceptionHandling<KGMonthlyInspectionL.deduction>(row, row.Deduction,
                  new PXSetPropertyException("Deduction must between 0 ~ 100.",
                     PXErrorLevel.Warning));
                e.Cancel = true;
            }
        }
        #endregion

        public override void Persist()
        {
            KGMonthlyInspection master = MonthlyInspectionH.Current;
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
                //setInsertData(master);
                //新增時塞資料
                if (master.MonthlyInspectionID < 0 || master.MonthlyInspectionID == null)
                {
                    KGMonthlyInspectionSign sign =(KGMonthlyInspectionSign) KGMonthlyInspectionSigns.Cache.CreateInstance();
                    sign.SeqNo = 1;
                    sign.SignBy = "工地主管";
                    KGMonthlyInspectionSigns.Insert(sign);
                    KGMonthlyInspectionSign sign2 = (KGMonthlyInspectionSign)KGMonthlyInspectionSigns.Cache.CreateInstance();
                    sign2.SeqNo = 2;
                    sign2.SignBy = "設備承辦人";
                    KGMonthlyInspectionSigns.Insert(sign2);
                    /*
                    sign3 = (KGMonthlyInspectionSign)KGMonthlyInspectionSigns.Cache.CreateInstance();
                    sign3.SeqNo = 3;
                    sign3.SignBy = "安衛承辦人";
                    KGMonthlyInspectionSigns.Insert(sign3);
                    */
                }
                    
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    base.Persist();
                    ts.Complete();
                }
            }
        }
        public bool beforeSaveCheck()
        {
            bool check = true;
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            //新增時機點
            if (master.MonthlyInspectionID < 0 || master.MonthlyInspectionID == null) {

                //檢查該專案下上個月是否有非關閉狀態 KGMonthInspectionStatuses.Close
                PXGraph graph = new PXGraph();
                KGMonthlyInspection kgMonthlyInspection = PXSelect<KGMonthlyInspection, 
                    Where<KGMonthlyInspection.projectID,
                    Equal<Required<KGMonthlyInspection.projectID>>>,
                    OrderBy<Desc<KGMonthlyInspection.checkYear, Desc<KGMonthlyInspection.checkMonth>>>>.Select(graph, master.ProjectID);
                if (kgMonthlyInspection != null && !KGMonthInspectionStatuses.Close.Equals(kgMonthlyInspection.Status)) {
                    check = false;
                    throw new PXException("上一期尚未關閉");
                    
                }
                if (kgMonthlyInspection != null && kgMonthlyInspection.CheckMonth.Equals(master.CheckMonth) && kgMonthlyInspection.CheckYear.Equals(master.CheckYear))
                {
                    check = false;
                    throw new PXException("同專案同一期不能開兩張");

                }
                //當月是否兩張
            }

            check = DAC.Utils.RaiseNullCheck<KGMonthlyInspection.systemManager>(MonthlyInspectionH.Cache, master, master.SystemManager, "SystemManager") && check;
            check = DAC.Utils.RaiseNullCheck<KGMonthlyInspection.siteManager>(MonthlyInspectionH.Cache, master, master.SiteManager, "Site Manager") && check;
            check = DAC.Utils.RaiseNullCheck<KGMonthlyInspection.equipmentSupervisor>(MonthlyInspectionH.Cache, master, master.EquipmentSupervisor, "Equipment Supervisor") && check;
            //check = DAC.Utils.RaiseNullCheck<KGMonthlyInspection.safeHealthSupervisor>(MonthlyInspectionH.Cache, master, master.SafeHealthSupervisor, "SafeHealth Supervisor") && check;

            foreach (KGMonthlyInspectionL row in MonthlyInspectionL.Select())
            {
                if (row.MissingNum != null) {
                    int num = int.Parse(row.MissingNum.ToString());

                    if (num < 0)
                    {
                        check = false;
                        MonthlyInspectionL.Cache.RaiseExceptionHandling<KGMonthlyInspectionL.missingNum>(row, row.MissingNum,
                               new PXSetPropertyException("缺失數不可小於 0", PXErrorLevel.Error));
                    }

                    if (num > row.MaxNoMissing)
                    {
                        check = false;
                        MonthlyInspectionL.Cache.RaiseExceptionHandling<KGMonthlyInspectionL.missingNum>(row, row.MissingNum,
                          new PXSetPropertyException("MissingNum can't greater than MaxNoMissing.",
                             PXErrorLevel.Error));

                    }
                    if (KGMonthInspectionResults.Defect.Equals(row.TestResult) && num==0) {
                        check = false;
                        MonthlyInspectionL.Cache.RaiseExceptionHandling<KGMonthlyInspectionL.missingNum>(row, row.MissingNum,
                          new PXSetPropertyException("檢查結果是有缺失須改正,缺失數要大於0.",
                             PXErrorLevel.Error));
                    }
                    else if (KGMonthInspectionResults.Observation.Equals(row.TestResult) && num >0)
                    {
                        row.MissingNum = 0;
                        MonthlyInspectionL.Update(row);
                    }
                    else if(KGMonthInspectionResults.Qualified.Equals(row.TestResult) && num > 0)
                    {
                        row.MissingNum = 0;
                        MonthlyInspectionL.Update(row);
                    }
                }
                else {
                    if (KGMonthInspectionResults.Defect.Equals(row.TestResult))
                    {
                        check = false;
                        MonthlyInspectionL.Cache.RaiseExceptionHandling<KGMonthlyInspectionL.missingNum>(row, row.MissingNum,
                          new PXSetPropertyException("檢查結果是有缺失須改正,缺失數要大於0且不能為空白",
                             PXErrorLevel.Error));
                    }
                }

                //檢查已核可沒有檢查結果
                if (row.TestResult == null && row.IsChecked == true) {
                    check = false;
                    MonthlyInspectionL.Cache.RaiseExceptionHandling<KGMonthlyInspectionL.testResult>(row, row.TestResult,
                      new PXSetPropertyException("檢查結果是有缺失須改正,缺失數要大於0且不能為空白",
                         PXErrorLevel.Error));
                }


                
            }
            

            return check;
        }


        #endregion

        #region Methods

        public String getLastMonthInspactionRemark(KGMonthlyInspectionL kgMonthlyInspectL)
        {
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            int month = Convert.ToInt32(master.CheckMonth) - 1;
            int year = Convert.ToInt32(master.CheckYear);
            if (month == 0)
            {
                month = 12;
                year = year - 1;
            }
            String remark = null;
            foreach (PXResult<KGMonthlyInspectionL, KGMonthlyInspection> result in PXSelectJoin<KGMonthlyInspectionL,
                InnerJoin<KGMonthlyInspection, On<KGMonthlyInspection.monthlyInspectionID, Equal<KGMonthlyInspectionL.monthlyInspectionID>>,
                InnerJoin<KGMonthlyInspectionTemplateL, On<KGMonthlyInspectionTemplateL.templateLineID, Equal<KGMonthlyInspectionL.templateLineID>>>>,
                Where<KGMonthlyInspection.checkMonth, LessEqual<Required<KGMonthlyInspection.checkMonth>>,
                    And<KGMonthlyInspection.checkYear, LessEqual<Required<KGMonthlyInspection.checkYear>>,
                    And<KGMonthlyInspectionL.segmentCD, Equal<Required<KGMonthlyInspectionL.segmentCD>>,
                    And<KGMonthlyInspectionTemplateL.checkItem, Equal<Required<KGMonthlyInspectionTemplateL.checkItem>>,
                    And<KGMonthlyInspection.projectID, Equal<Required<KGMonthlyInspection.projectID>>>>>>>,
                    OrderBy<Desc<KGMonthlyInspection.checkYear,
                            Desc<KGMonthlyInspection.checkMonth,
                            Desc<KGMonthlyInspection.checkDate>>>>>.
                            Select(this, month, year, kgMonthlyInspectL.SegmentCD, kgMonthlyInspectL.CheckItem, master.ProjectID))
                            
            {

                KGMonthlyInspectionL kgMonthlyInspectionL = (KGMonthlyInspectionL)result;
                KGMonthlyInspection kgMonthlyInspection = (KGMonthlyInspection)result;
                remark = kgMonthlyInspectionL.Remark;
                break;
            }
            return remark;
        }

       
        private void AddKGMonthlyInspectionTemplateLines(int templateHeaderID)
        {
            RemoveAllKGMonthlyInspectionTemplateLines();
            KGMonthlyInspection row = MonthlyInspectionH.Cache.Current as KGMonthlyInspection;
            PXResultset<KGMonthlyInspectionTemplateL> set = GetKGMonthlyInspectionTemplateLines(templateHeaderID);
            KGMonthlyInspectionTemplateH template = this.GetKGMonthlyInspectionTemplateHeader((int)row.TemplateHeaderID);
            row.FinalScore = 0;
            row.Evaluation = null;
            row.EvaluationDesc = null;
            
            foreach (KGMonthlyInspectionTemplateL item in set)
            {

                KGMonthlyInspectionTemplateS templateS = getTemplateS(template, item);
                KGMonthlyInspectionL line = new KGMonthlyInspectionL();
                //20180816
                line.TemplateLineID = item.TemplateLineID;
                line.CheckPointDesc = item.CheckPointDesc;
                line.CheckItem = item.CheckItem;
                line.SegmentCD = item.SegmentCD;
                line.MaxNoMissing = item.MaxNoMissing;
                //20191005 Jerry
                /*
                PXResultset<KGMonthlyInspectionTemplateL> set2 = GetKGMonthlyInspectionTemplateLines(templateHeaderID, item.SegmentCD);
                if (set.Count != 0)
                {
                    line.ScoreSetupLine = templateS.ScoreSetup / set2.Count;
                }
                line.EveryMissingBuckle = (decimal)(line.ScoreSetupLine / line.MaxNoMissing);
                line.Deduction = line.MissingNum * line.EveryMissingBuckle;
                line.Score = line.ScoreSetupLine - line.Deduction;*/
                line.Score = 0;
                line.EveryMissingBuckle = 0;
                line.Deduction = 0;
                //佩分
                line.ScoreSetupLine = 0;

                //20191005 by Jerry
                line.LastMonthRemark = getLastMonthInspactionRemark(line);

                MonthlyInspectionL.Cache.Update(line);
            }
        }

        private void RemoveAllKGMonthlyInspectionTemplateLines()
        {
            foreach (KGMonthlyInspectionL item in MonthlyInspectionL.Select())
            {
                MonthlyInspectionL.Cache.Delete(item);
            }
        }

        private KGMonthlyInspectionTemplateH GetKGMonthlyInspectionTemplateHeader(int templateHeaderId)
        {
            return PXSelect<KGMonthlyInspectionTemplateH,
                                         Where<KGMonthlyInspectionTemplateH.templateHeaderID,
                                                         Equal<Required<KGMonthlyInspectionTemplateH.templateHeaderID>>>>.Select(this, templateHeaderId);
        }

        private PXResultset<KGMonthlyInspectionTemplateL> GetKGMonthlyInspectionTemplateLines(int templateHeaderId)
        {
            return PXSelect<KGMonthlyInspectionTemplateL,
                                        Where<KGMonthlyInspectionTemplateL.templateHeaderID,
                                                        Equal<Required<KGMonthlyInspectionTemplateL.templateHeaderID>>>>.Select(this, templateHeaderId);
        }
        private PXResultset<KGMonthlyInspectionTemplateL> GetKGMonthlyInspectionTemplateLines(int templateHeaderId, String segmentCD)
        {
            return PXSelect<KGMonthlyInspectionTemplateL,
                                        Where<KGMonthlyInspectionTemplateL.templateHeaderID,
                                                        Equal<Required<KGMonthlyInspectionTemplateL.templateHeaderID>>,
                                             And<KGMonthlyInspectionTemplateL.segmentCD,
                                                        Equal<Required<KGMonthlyInspectionTemplateL.segmentCD>>>>>.Select(this, templateHeaderId, segmentCD);
        }

        private KGMonthlyInspectionTemplateL FilterKGMonthlyInspectionTemplateLine(KGMonthlyInspectionL row, PXResultset<KGMonthlyInspectionTemplateL> collection)
        {
            foreach (KGMonthlyInspectionTemplateL item in collection)
            {
                if (item.TemplateLineID == row.TemplateLineID)
                {
                    return item;
                }
            }
            return null;
        }

        private String GetEvaluation(decimal score)
        {
            if (score >= 95)
            {
                return KGMMonthlyInspectionEvaluations.KMI_A_PLUS;
            }
            else if (score >= 90 && score < 95)
            {
                return KGMMonthlyInspectionEvaluations.KMI_A;
            }
            else if (score >= 85 && score < 90)
            {
                return KGMMonthlyInspectionEvaluations.KMI_B_PLUS;
            }
            else if (score >= 80 && score < 85)
            {
                return KGMMonthlyInspectionEvaluations.KMI_B;
            }
            else if (score >= 70 && score < 80)
            {
                return KGMMonthlyInspectionEvaluations.KMI_C;
            }
            else if (score < 70)
            {
                return KGMMonthlyInspectionEvaluations.KMI_D;
            }
            else {
                return KGMMonthlyInspectionEvaluations.KMI_D;
            }
        }
        #endregion
        private String GetEvaluationDesc(String evaluation)
        {
            if (KGMMonthlyInspectionEvaluations.KMI_A_PLUS.Equals(evaluation))
            {
                return KGMMonthlyInspectionEvaluations.KMI_A_PLUS_Str;
            }
            else if (KGMMonthlyInspectionEvaluations.KMI_A.Equals(evaluation))
            {
                return KGMMonthlyInspectionEvaluations.KMI_A_Str;
            }
            else if (KGMMonthlyInspectionEvaluations.KMI_B_PLUS.Equals(evaluation))
            {
                return KGMMonthlyInspectionEvaluations.KMI_B_PLUS_Str;
            }
            else if (KGMMonthlyInspectionEvaluations.KMI_B.Equals(evaluation))
            {
                return KGMMonthlyInspectionEvaluations.KMI_B_Str;
            }
            else if (KGMMonthlyInspectionEvaluations.KMI_C.Equals(evaluation))
            {
                return KGMMonthlyInspectionEvaluations.KMI_C_Str;
            }
            else if (KGMMonthlyInspectionEvaluations.KMI_D.Equals(evaluation))
            {
                return KGMMonthlyInspectionEvaluations.KMI_D_Str;
            }
            else return null;
        }



        public void Redirect(String type) {
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            
            //有缺失
            if (type != null)
            {
                KGMonthlyInspectFileEntry target = PXGraph.CreateInstance<KGMonthlyInspectFileEntry>(); 
                target.Clear(PXClearOption.ClearAll);
                target.SelectTimeStamp();
                target.KGMonthlyInspectTicketLFiles.Current = PXSelectJoin<KGMonthlyInspectTicketLUpdate,
                        InnerJoin<KGMonthlyInspectTicket,On<KGMonthlyInspectTicketLUpdate.monthlyInspectTicketID,
                            Equal<KGMonthlyInspectTicket.monthlyInspectTicketID>>>,
                Where<KGMonthlyInspectTicketLUpdate.testResult, Equal<Required<KGMonthlyInspectTicketLUpdate.testResult>>,
                And<KGMonthlyInspectTicketLUpdate.monthlyInspectionID, Equal<Required<KGMonthlyInspectTicketLUpdate.monthlyInspectionID>>
                ,And<KGMonthlyInspectTicket.status,Equal<Required<KGMonthlyInspectTicket.status>>>>>>.Select(this, type, master.MonthlyInspectionID, KGMonthInspectionTicketStatuses.Open);
                throw new PXRedirectRequiredException(target, true, "View KGMonthlyInspectTicket") {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
            else {
                KGMonthlyInspectFileAllEntry target = PXGraph.CreateInstance<KGMonthlyInspectFileAllEntry>();
                target.Clear(PXClearOption.ClearAll);
                target.SelectTimeStamp();
                target.KGMonthlyInspectTicketLFiles.Current = PXSelectJoin<KGMonthlyInspectTicketLUpdateAll,
                        InnerJoin<KGMonthlyInspectTicket, On<KGMonthlyInspectTicketLUpdateAll.monthlyInspectTicketID,
                            Equal<KGMonthlyInspectTicket.monthlyInspectTicketID>>>,
                Where<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionID, 
                Equal<Required<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionID>>
                ,And<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionLineID,
                Equal<Required<KGMonthlyInspectTicketLUpdateAll.monthlyInspectionLineID>>,
                And < KGMonthlyInspectTicket.status,Equal < Required < KGMonthlyInspectTicket.status >>>>>>.
                Select(this, master.MonthlyInspectionID, MonthlyInspectionL.Current.MonthlyInspectionLineID, KGMonthInspectionTicketStatuses.Open);
                throw new PXRedirectRequiredException(target, true, "View KGMonthlyInspectTicket") {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            
        }
        /*
        public  List<KGMonthlyInspectTicketLFileUpdate> getTicketFile(String type,KGMonthlyInspectionL kgMonthlyInspectionL) {
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            List<KGMonthlyInspectTicketLFileUpdate> list = new List<KGMonthlyInspectTicketLFileUpdate>();
            foreach (KGMonthlyInspectTicketLFileUpdate ticketFile  in PXSelect<KGMonthlyInspectTicketLFileUpdate,
                Where<KGMonthlyInspectTicketLFileUpdate.testResult, Equal<Required<KGMonthlyInspectTicketLFileUpdate.testResult>>,
                And<KGMonthlyInspectTicketLFileUpdate.monthlyInspectionID, Equal<Required<KGMonthlyInspectTicketLFileUpdate.monthlyInspectionID>>>>>.Select(this, type, master.MonthlyInspectionID)) {
                if (kgMonthlyInspectionL.MonthlyInspectionLineID.Equals(ticketFile.MonthlyInspectionLineID)) {
                    list.Add(ticketFile);
                }
            }
            return list;
        }*/
        public List<KGMonthlyInspectTicketLUpdate> getTicketL(String type, KGMonthlyInspectionL kgMonthlyInspectionL)
        {
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            List<KGMonthlyInspectTicketLUpdate> list = new List<KGMonthlyInspectTicketLUpdate>();
            foreach (KGMonthlyInspectTicketLUpdate ticketL in PXSelectJoin<KGMonthlyInspectTicketLUpdate,
                InnerJoin<KGMonthlyInspectTicket,On<KGMonthlyInspectTicket.monthlyInspectTicketID,Equal<KGMonthlyInspectTicketLUpdate.monthlyInspectTicketID>>>,
                Where<KGMonthlyInspectTicketLUpdate.testResult, Equal<Required<KGMonthlyInspectTicketLUpdate.testResult>>,
                And<KGMonthlyInspectTicketLUpdate.monthlyInspectionID, Equal<Required<KGMonthlyInspectTicketLUpdate.monthlyInspectionID>>
                ,And<KGMonthlyInspectTicket.status,Equal<Required<KGMonthlyInspectTicket.status>>>>>>.Select(this, type, master.MonthlyInspectionID, KGMonthInspectionTicketStatuses.Open))
            {
                if (kgMonthlyInspectionL.MonthlyInspectionLineID.Equals(ticketL.MonthlyInspectionLineID))
                {
                    list.Add(ticketL);
                }
            }
            return list;
        }
        public List<KGMonthlyInspectTicketLUpdate> checkTicketL()
        {
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            List<KGMonthlyInspectTicketLUpdate> list = new List<KGMonthlyInspectTicketLUpdate>();
            foreach (KGMonthlyInspectTicketLUpdate ticketL in PXSelectJoin<KGMonthlyInspectTicketLUpdate,
                InnerJoin<KGMonthlyInspectTicket, On<KGMonthlyInspectTicket.monthlyInspectTicketID,
                Equal<KGMonthlyInspectTicketLUpdate.monthlyInspectTicketID>>>,
                Where<KGMonthlyInspectTicketLUpdate.monthlyInspectionID, Equal<Required<KGMonthlyInspectTicketLUpdate.monthlyInspectionID>>
                , And<KGMonthlyInspectTicket.status, Equal<Required<KGMonthlyInspectTicket.status>>>>>
                .Select(this, master.MonthlyInspectionID, KGMonthInspectionTicketStatuses.Open))
            {
                list.Add(ticketL);
            }
            return list;
        }


        public PXAction<KGMonthlyInspection> flawPhoto;
        [PXUIField(DisplayName = "FlawPhoto", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable FlawPhoto(PXAdapter adapter)
        {
            Redirect(KGMonthInspectionResults.Defect);
            return adapter.Get();
        }
        public PXAction<KGMonthlyInspection> observedPhoto;
        [PXUIField(DisplayName = "ObservedPhoto", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable ObservedPhoto(PXAdapter adapter)
        {
            Redirect(KGMonthInspectionResults.Observation);
            return adapter.Get();
        }
        public PXAction<KGMonthlyInspection> qualifiedPhoto;
        [PXUIField(DisplayName = "QualifiedPhoto", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable QualifiedPhoto(PXAdapter adapter)
        {
            Redirect(KGMonthInspectionResults.Qualified);
            return adapter.Get();
        }
        public PXAction<KGMonthlyInspection> importTicketResult;
        [PXUIField(DisplayName = "ImportResult", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable ImportTicketResult(PXAdapter adapter)
        {
            if (MonthlyInspectionH.Ask("是否要彙整?", "檢查項目結果將會依照現場檢查單重置, 確認執行匯整結果", MessageButtons.YesNo) != WebDialogResult.Yes)
            {
                return adapter.Get();
            }

            

            KGMonthlyInspection master = MonthlyInspectionH.Current;
            List<KGMonthlyInspectTicketLUpdate> allList= checkTicketL();

            if (allList.Count == 0) {
                throw new PXException("無月查核現場檢查單資料可彙整");
            }


            foreach (KGMonthlyInspectionL kgMonthlyInspectionL   in   MonthlyInspectionL.Select()) {
                
                List<KGMonthlyInspectTicketLUpdate> list = getTicketL(KGMonthInspectionResults.Defect, kgMonthlyInspectionL);
                kgMonthlyInspectionL.MissingNum = list.Count;
                KGMonthlyInspectionTemplateL templateL = PXSelect<KGMonthlyInspectionTemplateL,
                    Where<KGMonthlyInspectionTemplateL.templateLineID,
                    Equal<Required<KGMonthlyInspectionTemplateL.templateLineID>>>>.Select(this, kgMonthlyInspectionL.TemplateLineID);
                if (templateL != null && list.Count> templateL.MaxNoMissing) {
                    kgMonthlyInspectionL.MissingNum = templateL.MaxNoMissing;
                }
 
                kgMonthlyInspectionL.Remark = getRemark(list);
                kgMonthlyInspectionL.IsChecked = getIsCheck(kgMonthlyInspectionL);
                if (kgMonthlyInspectionL.IsChecked == true)
                {
                    if (kgMonthlyInspectionL.MissingNum > 0)
                    {
                        kgMonthlyInspectionL.TestResult = KGMonthInspectionResults.Defect;
                    }
                    else if (getTicketL(KGMonthInspectionResults.Observation, kgMonthlyInspectionL).Count > 0) {
                        kgMonthlyInspectionL.TestResult = KGMonthInspectionResults.Observation;
                    }
                    else 
                    {
                        kgMonthlyInspectionL.TestResult = KGMonthInspectionResults.Qualified;
                    }
                }
                else {
                    kgMonthlyInspectionL.TestResult = null;
                }
                
                //XX
                MonthlyInspectionL.Update(kgMonthlyInspectionL);
            }
            computeTotal();
            return adapter.Get();
        }

        public void caculate(bool isNew) {
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            foreach (KGMonthlyInspectionL kgMonthlyInspectionL in MonthlyInspectionL.Select())
            {

                List<KGMonthlyInspectTicketLUpdate> list = getTicketL(KGMonthInspectionResults.Defect, kgMonthlyInspectionL);
                kgMonthlyInspectionL.MissingNum = list.Count;
                kgMonthlyInspectionL.Remark = getRemark(list);
                if (isNew == true) {
                    kgMonthlyInspectionL.IsChecked = getIsCheck(kgMonthlyInspectionL);
                }
                if (kgMonthlyInspectionL.IsChecked == true)
                {
                    if (kgMonthlyInspectionL.MissingNum > 0)
                    {
                        kgMonthlyInspectionL.TestResult = KGMonthInspectionResults.Defect;
                    }
                    else if (getTicketL(KGMonthInspectionResults.Observation, kgMonthlyInspectionL).Count > 0)
                    {
                        kgMonthlyInspectionL.TestResult = KGMonthInspectionResults.Observation;
                    }
                    else
                    {
                        kgMonthlyInspectionL.TestResult = KGMonthInspectionResults.Qualified;
                    }
                }
                else
                {
                    kgMonthlyInspectionL.TestResult = null;
                }

                //XX
                MonthlyInspectionL.Update(kgMonthlyInspectionL);
            }
            computeTotal();
        }



        public PXAction<KGMonthlyInspection> viewTicketFile;
        [PXUIField(DisplayName = "ViewTicketFile", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXLookupButton(CommitChanges = true)]
        public virtual IEnumerable ViewTicketFile(PXAdapter adapter)
        {
            Redirect(null);
            return adapter.Get();
        }

        public void computeTotal() {
            
            
            KGMonthlyInspection master = MonthlyInspectionH.Current;
            KGMonthlyInspectionTemplateH template = this.GetKGMonthlyInspectionTemplateHeader((int)master.TemplateHeaderID);

            foreach (KGMonthlyInspectionL kgMonthlyInspectionL in MonthlyInspectionL.Select())
            {
                //PXResultset<KGMonthlyInspectionL> set = PXSelect<KGMonthlyInspectionL,
                //    Where<KGMonthlyInspectionL.segmentCD,Equal<Required<KGMonthlyInspectionL.segmentCD>>
                //    ,And<KGMonthlyInspectionL.isChecked,Equal<True>>>>.Select(this, kgMonthlyInspectionL.SegmentCD);
                PXResultset<KGMonthlyInspectionL> set = PXSelect<KGMonthlyInspectionL,
                        Where<KGMonthlyInspectionL.segmentCD, Equal<Required<KGMonthlyInspectionL.segmentCD>>
                        , And<KGMonthlyInspectionL.monthlyInspectionID, Equal<Required<KGMonthlyInspection.monthlyInspectionID>>
                        , And<KGMonthlyInspectionL.isChecked, Equal<True>>
                         >>>.Select(this, kgMonthlyInspectionL.SegmentCD, master.MonthlyInspectionID);

                KGMonthlyInspectionTemplateS templateS = getTemplateS(template.TemplateHeaderID, kgMonthlyInspectionL.SegmentCD);
                if (kgMonthlyInspectionL.IsChecked == true)
                {
                    if (set.Count != 0)
                    {
                        kgMonthlyInspectionL.ScoreSetupLine = templateS.ScoreSetup / set.Count;
                    }
                    else
                    {
                        kgMonthlyInspectionL.ScoreSetupLine = 0;
                    }
                    if (kgMonthlyInspectionL.MaxNoMissing != 0)
                    {
                        kgMonthlyInspectionL.EveryMissingBuckle = (decimal)(kgMonthlyInspectionL.ScoreSetupLine / kgMonthlyInspectionL.MaxNoMissing);
                    }
                    else
                    {
                        kgMonthlyInspectionL.EveryMissingBuckle = 0;
                    }


                    kgMonthlyInspectionL.Deduction = kgMonthlyInspectionL.MissingNum * kgMonthlyInspectionL.EveryMissingBuckle;
                    if (kgMonthlyInspectionL.Deduction > kgMonthlyInspectionL.ScoreSetupLine) {
                        kgMonthlyInspectionL.Deduction = kgMonthlyInspectionL.ScoreSetupLine;
                    }

                    kgMonthlyInspectionL.Score = kgMonthlyInspectionL.ScoreSetupLine - kgMonthlyInspectionL.Deduction;
                }
                else {
                    kgMonthlyInspectionL.Score = 0;
                    kgMonthlyInspectionL.EveryMissingBuckle = 0;
                    kgMonthlyInspectionL.Deduction = 0;
                    //佩分
                    kgMonthlyInspectionL.ScoreSetupLine = 0;
                }
                MonthlyInspectionL.Update(kgMonthlyInspectionL);
            }
        }


        public string getRemark(List<KGMonthlyInspectTicketLUpdate> list) {
            String remark = null;
            foreach (KGMonthlyInspectTicketLUpdate ticketFile in list) {
                if (remark == null && ticketFile.Remark != null) {
                    remark = ticketFile.Remark;
                } else if (ticketFile.Remark != null) {
                    //20200806 11649: KG304000 月查核表彙整結果, 合併不同現場檢查單的缺失描述(Remark)請用 "; "隔開
                    remark = remark +"; "+ ticketFile.Remark;
                }
            }
            return remark;
        }
        public bool getIsCheck(KGMonthlyInspectionL kgMonthlyInspectionL) {
            
            PXResultset<KGMonthlyInspectTicketL> set = PXSelect<KGMonthlyInspectTicketL,
                Where<KGMonthlyInspectTicketL.monthlyInspectionLineID,
                Equal<Required<KGMonthlyInspectTicketL.monthlyInspectionLineID>>>>
                .Select(this, kgMonthlyInspectionL.MonthlyInspectionLineID);
            foreach (KGMonthlyInspectTicketL ticketL in set) {
                return true;
            }
            return false;
        }

    }
}