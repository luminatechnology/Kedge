using System;
using PX.Data;
using Kedge.DAC;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
//using eInvoiceLib;
using System.Text;
using Kedge.Util;
using System.IO;
using PX.Objects.CR;

namespace Kedge
{
    public class KGSafetyHealthInspectionEntry : PXGraph<KGSafetyHealthInspectionEntry, KGSafetyHealthInspection>
    {
        public KGSafetyHealthInspectionEntry()
        {
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.CloseAction);
            this.ReportMenu.MenuAutoOpen = true;
            this.ReportMenu.AddMenuAction(this.PrintAction);
            this.ReportMenu.AddMenuAction(this.DowloadPPT);

        }

        #region Select
        public PXFilter<KG304002Filter> MasterFilter;

        public PXSelect<KGSafetyHealthInspection> SafetyHealthInspectionH;

        public PXSelect<KGSafetyHealthInspectionL,
                Where<KGSafetyHealthInspectionL.safetyHealthInspectionID,
                    Equal<Current<KGSafetyHealthInspection.safetyHealthInspectionID>>>> SafetyHealthInspectionL;

        #endregion

        #region Action
        public PXAction<KGSafetyHealthInspection> ActionMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Action")]
        protected void actionMenu() { }
        public PXAction<KGSafetyHealthInspection> ReportMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Report")]
        protected void reportMenu() { }

        #region Close
        public PXAction<KGSafetyHealthInspection> CloseAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Close")]
        protected void closeAction()
        {
            KGSafetyHealthInspection inspectionH = SafetyHealthInspectionH.Current;
            if (inspectionH.Status == SafetyHealthInspectionStatuses.Open)
            {
                inspectionH.Status = SafetyHealthInspectionStatuses.Close;
                SafetyHealthInspectionH.Update(inspectionH);
            }

        }
        #endregion

        #region Print
        public PXAction<KGSafetyHealthInspection> PrintAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Print")]
        protected void printAction()
        {
            KGSafetyHealthInspection row = SafetyHealthInspectionH.Current;
            Dictionary<string, string> mailParams = new Dictionary<string, string>()
            {
                ["pSafetyHealthInspectionCD"] = row.SafetyHealthInspectionCD
            };
            var requiredException = new PXReportRequiredException
            (mailParams, "KG604002", PXBaseRedirectException.WindowMode.New, "職安衛查核表");
            requiredException.SeparateWindows = true;
            //requiredException.AddSibling("KG601000", mailParams);
            throw new PXRedirectWithReportException(this, requiredException, "Preview");

        }
        #endregion

        #region ShowPhoto
        public PXAction<KG304002Filter> ShowPhoto;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Show Photo")]
        public virtual IEnumerable showPhoto(PXAdapter adapter)
        {
            PhopoView(false);
            return adapter.Get();
        }
        #endregion

        #region DowloadPPT
        public PXAction<KGSafetyHealthInspection> DowloadPPT;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Download PPT")]
        public virtual IEnumerable dowloadPPT(PXAdapter adapter)
        {
            KGSafetyHealthInspection row = SafetyHealthInspectionH.Current;
            if (row == null || row.SafetyHealthInspectionID <= 0 || row.SafetyHealthInspectionID == null) return adapter.Get();
            //取得職安衛pptx範本NoteID
            KGInspectionFileTemplate fileTemp = GetFileTemplate("1");//職安衛 : "1"
            if (fileTemp == null) return adapter.Get();
            PX.SM.UploadFileRevision fileR = KGPptTemplateUtil.GetUploadFileRevision(this, fileTemp.NoteID);
            if (fileR == null) return adapter.Get();

            //開始產生ppt
            KGPptTemplateUtil pptUtil = new KGPptTemplateUtil(fileR.Data);
            Contract contract = GetContract(row.ProjectID);
            String companyName = GetCompanyName(contract.DefaultBranchID);


            String projectCD = contract.ContractCD;
            String projectDesc = contract.Description;
            DateTime checkdate = (DateTime)row.CheckDate;
            String checkdateStr = checkdate.ToString("yyyy/MM/dd");
            String checkdateStrforName = checkdate.ToString("yyyyMMdd");

            String safetyCD = row.SafetyHealthInspectionCD;
            //2020/01/14 更改名稱
            String date = checkdate.Year + "年" + checkdate.Month + "月" + checkdate.Day + "日";
            String title = companyName + " " + projectCD.Trim() + "-" + projectDesc + " " + date + "職安衛查核-" + safetyCD;

            int idx = 2;
            KGSafetyHealthInspectTicketL checkTicketL = GetPPTData(row.SafetyHealthInspectionID);
            if (checkTicketL == null)
            {
                throw new Exception("無職安衛檢查單,請確認!");
            }
            foreach (KGSafetyHealthInspectTicketL item in GetPPTData(row.SafetyHealthInspectionID))
            {
                if (idx == 2)
                {//新增畫面
                    pptUtil.NewPage();
                    idx = 0;
                    pptUtil.ReplaceTextTag("{{TitleMsg}}", title);
                }
                int itemIdx = 1 + idx++;

                pptUtil.ReplaceTextTag("{{Date" + itemIdx + "}}", checkdateStr);
                //20200225 edit by alton :因KGSafetyHealthInspectTicketL.CheckItem的PXDBScalar被拔掉，為了降低危害程度，在此取回checkItem
                //pptUtil.ReplaceTextTag("{{CheckItem" + itemIdx + "}}", item.CheckItem);
                pptUtil.ReplaceTextTag("{{CheckItem" + itemIdx + "}}", GetCheckItem(item.TemplateLineID));
                pptUtil.ReplaceTextTag("{{Remark" + itemIdx + "}}", item.Remark);
                //2019/11/22 ADD ImprovementPlan
                pptUtil.ReplaceTextTag("{{ImprovementPlan" + itemIdx + "}}", item.ImprovementPlan);

                //取得圖檔
                PX.SM.UploadFileRevision imageFile = KGPptTemplateUtil.GetUploadFileRevision(this, item.NoteID);
                if (imageFile != null)
                {
                    pptUtil.ReplaceImageTag("Image" + itemIdx, imageFile.Data, "image/jpeg");
                }

            }
            //清除奇數資料所殘留的替代文字
            if (idx == 1)
            {
                pptUtil.ReplaceTextTag("{{Date2}}", "");
                pptUtil.ReplaceTextTag("{{CheckItem2}}", "");
                pptUtil.ReplaceTextTag("{{Remark2}}", "");
                //2019/11/22 ADD ImprovementPlan
                pptUtil.ReplaceTextTag("{{ImprovementPlan2}}", "");
            }

            //ptt產生結束
            byte[] outBytes = pptUtil.Complete();

            //檔案下載
            throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(),
                                                  "SHI_" + safetyCD + ".pptx",
                                                   null, outBytes
                                                   ), true);
        }

        #endregion

        /*#region EGuiPrintTest
        public PXAction<KGSafetyHealthInspection> EGuiPrintTest;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "EGuiPrintTest")]
        public virtual IEnumerable eGuiPrintTest(PXAdapter adapter)
        {
            rp100Print();
            return adapter.Get();
        }

       /* private static void rp100Print()
        {
            ESCPOS pos = new ESCPOS();
            //註冊BIG5編碼
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            RP100 rp100 = new RP100(pos);
            pos.StartLPTPrinter("RP-600(E)", "Test");
            rp100.PrintInvoice(
                "10211YQ123456789999",
                "YQ1234567810211289999000000390000003C2809112123097532A93tZdCfOm3QnQExODfv3A==:**********:3:3:1:愛之味鮮採蕃茄汁:1:35:",
                "**波爾天然礦泉水:1:15:箭牌青箭口香糖:1:10",
                "102年11-12月",
                "YQ-12345678",
                "9999", "60", "89947155", "", false, "", "2014/11/06 12:59:11");

            pos.CutPaper(0x42, 0x00);

            //印明細
            pos.SendTo("---------------------------\n");
            pos.SendTo("銷貨明細表\n");
            pos.SendTo("愛之味鮮採蕃茄汁  x1     35TX\n");
            pos.SendTo("礦泉水       x1     35TX\n");
            pos.SelectCharSize(0x11);//字體大小
            pos.SendTo("測試Abc123\n");

            pos.CutPaper(0x42, 0x00);
            pos.EndLPTPrinter();
        }
        #endregion*/

        #region LinePhoto
        public PXAction<KGSafetyHealthInspection> LinePhoto;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Line Photo")]
        public virtual IEnumerable linePhoto(PXAdapter adapter)
        {
            PhopoView(true);
            return adapter.Get();
        }
        #endregion

        #region ImportResult
        public PXAction<KGSafetyHealthInspection> ImportResult;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "ImportResult")]
        public virtual IEnumerable importResult(PXAdapter adapter)
        {
            //2020/03/11 ADD 按下彙整結果先跳出視窗詢問
            if (SafetyHealthInspectionH.Ask("是否要彙整?", "檢查項目結果將會依照現場檢查單重置, 確認執行匯整結果",
                MessageButtons.YesNo) != WebDialogResult.Yes)
            {
                return adapter.Get();
            }
            KGSafetyHealthInspection master = SafetyHealthInspectionH.Current;
            if (GetTicketL(master.SafetyHealthInspectionID).Count == 0)
            {
                throw new PXException("無職安衛查核現場檢查單資料可彙整");
            }

            //取得0扣分類別
            KGSafetyHealthCategoryDeductionSetup zeroType= GetZeroPointType();
            foreach (KGSafetyHealthInspectionL item in SafetyHealthInspectionL.Select())
            {
                //2020/03/06 ADD 先把L的幾個update欄位清空,再update
                if (item.Remark != null)
                    item.Remark = null;
                if (item.ImprovementPlan != null)
                    item.ImprovementPlan = null;
                //20200807 Alton(11648) 預設為已查核，查核結果為合格，扣分0
                //if (item.IsChecked != null)
                //    item.IsChecked = false;
                //if (item.CategoryCD != null)
                //    item.CategoryCD = null;
                //if (item.Deduction != null)
                //    item.Deduction = null;
                item.IsChecked = true;
                item.CategoryCD = zeroType?.CategoryCD;
                item.Deduction = zeroType?.DeductionSetup;
                SafetyHealthInspectionL.Update(item);

                KGSafetyHealthInspectTicketL badTicket = null;
                foreach (KGSafetyHealthInspectTicketL ticket in GetTicketLByLine(item.SafetyHealthInspectionLineID))
                {
                    if (badTicket == null || badTicket.DeductionSetup > ticket.DeductionSetup)
                    {
                        badTicket = ticket;
                    }
                }
                if (badTicket != null)
                {
                    item.CategoryCD = badTicket.CategoryCD;
                    item.Deduction = badTicket.DeductionSetup;
                    item.Remark = badTicket.Remark;
                    //2019/11/22 ADD ImprovementPlan
                    item.ImprovementPlan = badTicket.ImprovementPlan;
                    //2020/03/06 ADD isCheck = true
                    item.IsChecked = true;
                    SafetyHealthInspectionL.Update(item);
                }

            }
            return adapter.Get();
        }
        #endregion

        #endregion

        #region Event
        #region KGSafetyHealthInspection
        protected virtual void KGSafetyHealthInspection_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspection row = (KGSafetyHealthInspection)e.Row;
            if (row == null) return;
            ControlEnabled();
            if (row.TemplateHeaderID != null)
            {
                KGSafetyHealthInspectionTemplateH TemplateH =
                    PXSelect<KGSafetyHealthInspectionTemplateH,
                    Where<KGSafetyHealthInspectionTemplateH.templateHeaderID,
                    Equal<Required<KGSafetyHealthInspectionTemplateH.templateHeaderID>>>>
                    .Select(this, row.TemplateHeaderID);
                //2019/11/14拿掉SegmentCD
                //row.SegmentCD = TemplateH.SegmentCD;
                row.Version = TemplateH.Version;
            }
        }
        protected virtual void KGSafetyHealthInspection_TemplateHeaderID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspection row = (KGSafetyHealthInspection)e.Row;
            if (row == null) return;

            KGSafetyHealthInspectionTemplateH TemplateH =
                            PXSelect<KGSafetyHealthInspectionTemplateH,
                            Where<KGSafetyHealthInspectionTemplateH.templateHeaderID,
                        Equal<Required<KGSafetyHealthInspection.templateHeaderID>>>>
                        .Select(this, row.TemplateHeaderID);
            //2019/11/14拿掉SegmentCD
            //row.SegmentCD = TemplateH.SegmentCD;
            row.Version = TemplateH.Version;
            //上一期的header
            KGSafetyHealthInspection inspection =
                PXSelectGroupBy<KGSafetyHealthInspection,
                Where<KGSafetyHealthInspection.projectID,
            Equal<Required<KGSafetyHealthInspection.projectID>>>,
                Aggregate<GroupBy<KGSafetyHealthInspection.projectID,
                Max<KGSafetyHealthInspection.safetyHealthInspectionID>>>>
                .Select(this, row.ProjectID);

            //選取範本帶出Detail的檢查項目
            PXResultset<KGSafetyHealthInspectionTemplateL> set =
                PXSelect<KGSafetyHealthInspectionTemplateL,
                Where<KGSafetyHealthInspectionTemplateL.templateHeaderID,
                Equal<Required<KGSafetyHealthInspectionTemplateH.templateHeaderID>>>>
                .Select(this, row.TemplateHeaderID);
            foreach (KGSafetyHealthInspectionTemplateL TemplateL in set)
            {
                KGSafetyHealthInspectionL detail = new KGSafetyHealthInspectionL();
                detail.TemplateLineID = TemplateL.TemplateLineID;
                detail.SegmentCD = TemplateL.SegmentCD;
                detail.CheckItem = TemplateL.CheckItem;
                //detail.CheckPointDesc = TemplateL.CheckPointDesc;

                if (inspection != null)
                {
                    KGSafetyHealthInspectionL inspectionL =
                    PXSelect<KGSafetyHealthInspectionL,
                    Where<KGSafetyHealthInspectionL.safetyHealthInspectionID,
                    Equal<Required<KGSafetyHealthInspectionL.safetyHealthInspectionID>>,
                    And<KGSafetyHealthInspectionL.templateLineID,
                    Equal<Required<KGSafetyHealthInspectionL.templateLineID>>>>>
                    .Select(this, inspection.SafetyHealthInspectionID, TemplateL.TemplateLineID);
                    if (inspectionL != null)
                    {
                        detail.LastRemark = inspectionL.Remark;
                    }

                }

                SafetyHealthInspectionL.Cache.Update(detail);
            }

        }
        protected virtual void KGSafetyHealthInspection_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspection row = (KGSafetyHealthInspection)e.Row;
            if (row == null) return;
            if (row.Hold == false)
            {
                row.Status = "N";
                CalScore();
            }
            if (row.Hold == true)
            {
                row.Status = "H";
            }
            if (row.FinalScore != null)
            {
                row.Evaluation = GetEvaluation((decimal)row.FinalScore);
            }
        }

        protected virtual void KGSafetyHealthInspection_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            KGSafetyHealthInspection row = e.Row as KGSafetyHealthInspection;
            if (row == null) return;
            if (row.SafetyHealthInspectionID < 0 && row.ProjectID != null)
            {
                KGSafetyHealthInspection inspection = PXSelect<KGSafetyHealthInspection,
                     Where<KGSafetyHealthInspection.projectID,
                     Equal<Required<KGSafetyHealthInspection.projectID>>,
                     And<KGSafetyHealthInspection.status,
                     NotEqual<SafetyHealthInspectionStatuses.close>,
                     And<KGSafetyHealthInspection.safetyHealthInspectionID,
                     Greater<Required<KGSafetyHealthInspection.safetyHealthInspectionID>>>>>>
                     .Select(this, row.ProjectID, 0);
                if (inspection != null)
                {
                    throw new Exception("有相同專案尚未關閉的單號,請先關閉職安衛單號!");
                }

            }
        }
        #endregion

        #region KGSafetyHealthInspectionL
        protected virtual void KGSafetyHealthInspectionL_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspectionL row = e.Row as KGSafetyHealthInspectionL;
            if (row == null) return;

            ControlEnabled();
            PXResultset<KGSafetyHealthCategoryDeductionSetup> set =
                PXSelect<KGSafetyHealthCategoryDeductionSetup>.Select(this);
            List<String> key = new List<String>();
            List<String> value = new List<String>();
            if (set == null)
            {
                throw new Exception("請至職安衛類別扣分設定頁面維護完整資料!");
            }
            foreach (KGSafetyHealthCategoryDeductionSetup CategoryCDValue in set)
            {

                key.Add(CategoryCDValue.CategoryCD);
                value.Add(CategoryCDValue.CategoryCD);

            }
            PXStringListAttribute.SetList<KGSafetyHealthInspectionL.categoryCD>(cache, row, key.ToArray(), value.ToArray());


            if (row.SegmentCD != null)
            {
                SegmentValue segmentValue =
                    PXSelect<SegmentValue,
                    Where<SegmentValue.value, Equal<Required<KGSafetyHealthInspectionL.segmentCD>>,
                    And<SegmentValue.dimensionID, Equal<KGInspectionConstant.kgshinsc>>>>
                    .Select(this, row.SegmentCD);

                row.SegmentDesc = segmentValue.Descr;
            }
        }
        protected virtual void KGSafetyHealthInspectionL_CategoryCD_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectionL row = e.Row as KGSafetyHealthInspectionL;
            if (row == null) return;
            if (row.CategoryCD == null || row.CategoryCD == "")
            {
                row.Deduction = null;
                row.IsChecked = false;
            }
            else
            {
                KGSafetyHealthCategoryDeductionSetup setup =
                PXSelect<KGSafetyHealthCategoryDeductionSetup,
                Where<KGSafetyHealthCategoryDeductionSetup.categoryCD,
                Equal<Required<KGSafetyHealthInspectionL.categoryCD>>>>
                .Select(this, row.CategoryCD);
                row.Deduction = setup.DeductionSetup;
                //2020/1/2 ADD 若扣分類別不為Null,查核與否要打勾
                row.IsChecked = true;
            }

        }

        protected virtual void KGSafetyHealthInspectionL_IsChecked_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectionL row = (KGSafetyHealthInspectionL)e.Row;
            //20200807 Alton(11648) IsCheck = false；清除 缺失說明、改善建議、查核與否、扣分類別、扣分
            if (row.IsChecked == false)
            {
                row.Remark = null;
                row.ImprovementPlan = null;
                row.CategoryCD = null;
                row.Deduction = null;
            }

            //20200807 Alton(11648) IsCheck = true；
            if (row.IsChecked == true)
            {
                //取得0扣分類別
                KGSafetyHealthCategoryDeductionSetup zeroType = GetZeroPointType();
                row.CategoryCD = zeroType?.CategoryCD;
                row.Deduction = zeroType?.DeductionSetup;
            }
        }
        #endregion

        #endregion

        #region Method
        public void ControlEnabled()
        {
            KGSafetyHealthInspection header = SafetyHealthInspectionH.Current;
            KGSafetyHealthInspectionL detail = SafetyHealthInspectionL.Current;

            //狀態為開啟或關閉全部為唯讀
            bool editable = header.Status == "H";
            bool HoldEnabled = header.Status != "C";
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.projectID>(SafetyHealthInspectionH.Cache, header, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.siteManager>(SafetyHealthInspectionH.Cache, header, editable);
            //2020/02/26拿掉
            //PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.systemManager>(SafetyHealthInspectionH.Cache, header, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.equipmentSupervisor>(SafetyHealthInspectionH.Cache, header, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.safeHealthSupervisor>(SafetyHealthInspectionH.Cache, header, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.inspectByID>(SafetyHealthInspectionH.Cache, header, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.remark>(SafetyHealthInspectionH.Cache, header, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.checkDate>(SafetyHealthInspectionH.Cache, header, editable);

            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.hold>(SafetyHealthInspectionH.Cache, header, HoldEnabled);
            Delete.SetEnabled(editable);

            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionL.remark>(SafetyHealthInspectionL.Cache, detail, editable);
            //2019/11/22 ADD Improvement Field
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionL.improvementPlan>(SafetyHealthInspectionL.Cache, detail, editable);
            //2020/01/17 查核與否Enable =false
            //2020/02/26 改回可以維護
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionL.isChecked>(SafetyHealthInspectionL.Cache, detail, editable);
            //2020/02/26 改為唯讀
            //PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionL.categoryCD>(SafetyHealthInspectionL.Cache, detail, editable);

            //2020/01/10 移除新增和刪除按鈕
            //SafetyHealthInspectionL.AllowDelete = editable;
            //SafetyHealthInspectionL.AllowInsert = editable;
            SafetyHealthInspectionL.AllowUpdate = editable;
            //2019/11/19 ADD 若擱置拿調 show photo &彙總查核不可以按
            ShowPhoto.SetEnabled(editable);
            ImportResult.SetEnabled(editable);
            //2019/11/28 ADD 擱置狀態不可按列印
            //2020/01/10 PPT按鈕邏輯和列印一致
            PrintAction.SetEnabled(!editable);
            DowloadPPT.SetEnabled(!editable);
            bool status = header.Status == "N";
            CloseAction.SetEnabled(status);

            //存檔過,範本不可更改
            bool Template;
            if (header.SafetyHealthInspectionID == null)
            {
                Template = true;
            }
            else
            {
                if (header.SafetyHealthInspectionID < 0)
                {
                    Template = true;
                }
                else
                {
                    Template = false;
                }
            }

            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspection.templateHeaderID>(SafetyHealthInspectionH.Cache, header, Template);

        }

        public void CalScore()
        {
            KGSafetyHealthInspection header = SafetyHealthInspectionH.Current;
            KGSafetyHealthInspectionL detail = SafetyHealthInspectionL.Current;

            decimal Score = 0;
            PXResultset<KGSafetyHealthInspectionTemplateS> TemplateS =
                PXSelect<KGSafetyHealthInspectionTemplateS,
                Where<KGSafetyHealthInspectionTemplateS.templateHeaderID,
                Equal<Required<KGSafetyHealthInspection.templateHeaderID>>>>
                .Select(this, header.TemplateHeaderID);
            foreach (KGSafetyHealthInspectionTemplateS S in TemplateS)
            {
                decimal SegmentScore = (decimal)S.ScoreSetup;
                foreach (KGSafetyHealthInspectionL line in SafetyHealthInspectionL.Select())
                {
                    if (line == null)
                    {
                        return;
                    }
                    if (line.SegmentCD == S.SegmentCD)
                    {
                        if (line.Deduction == null)
                        {
                            SegmentScore = SegmentScore + 0;
                        }
                        else
                        {
                            SegmentScore = SegmentScore + (decimal)line.Deduction;
                        }

                    }
                }
                if (SegmentScore < 0)
                {
                    SegmentScore = 0;
                }
                Score = Score + SegmentScore;

            }

            header.FinalScore = (decimal)Score;

        }

        private void PhopoView(bool isLine)
        {
            KG304002Filter filter = MasterFilter.Current;
            KGSafetyHealthInspection data = SafetyHealthInspectionH.Current;
            KGSafetyHealthInspectionL detail = SafetyHealthInspectionL.Current;

            bool check = isLine || (!isLine && filter != null && filter.CategoryCD != null);
            if (check && data != null && data.SafetyHealthInspectionID > 0)
            {
                PXGraph target = null;
                if (!isLine)
                {
                    //2020/03/19 ADD先檢核有無檢查單
                    KGSafetyHealthInspectTicketL inspectTicketL =
                    CheckShowPhotoTickerL(filter.CategoryCD, data.SafetyHealthInspectionID);
                    if (inspectTicketL == null)
                    {
                        throw new Exception("無此類型的現場檢查單!");
                    }


                    target = PXGraph.CreateInstance<KGSafetyHealthInspectTicketLFileEntry>();
                    target.Clear(PXClearOption.ClearAll);
                    target.SelectTimeStamp();
                    KGSafetyHealthInspectTicketLFileEntry entry = (KGSafetyHealthInspectTicketLFileEntry)target;
                    entry.SafetyTickectFiles.Current = entry.SafetyTickectFiles.Select(filter.CategoryCD, data.SafetyHealthInspectionID);
                }
                else
                {
                    //2020/03/19 ADD先檢核有無檢查單
                    KGSafetyHealthInspectTicketL inspectTicketL = CheckLinePhotoTicketL(detail);
                    if (inspectTicketL == null)
                    {
                        throw new Exception("無此檢查重點之檢查單!");
                    }


                    target = PXGraph.CreateInstance<KGSafetyHealthInspectTicketLFileAllEntry>();
                    target.Clear(PXClearOption.ClearAll);
                    target.SelectTimeStamp();
                    KGSafetyHealthInspectTicketLFileAllEntry entry = (KGSafetyHealthInspectTicketLFileAllEntry)target;
                    entry.SafetyTickectFiles.Current = entry.SafetyTickectFiles.Select(SafetyHealthInspectionL.Current.SafetyHealthInspectionLineID);
                }
                throw new PXRedirectRequiredException(target, true, filter.CategoryCD)
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
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
            else
            {
                return KGMMonthlyInspectionEvaluations.KMI_D;
            }
        }

        public List<KGSafetyHealthInspectTicketLUpdate> checkTicketL()
        {
            KGSafetyHealthInspection master = SafetyHealthInspectionH.Current;
            List<KGSafetyHealthInspectTicketLUpdate> list = new List<KGSafetyHealthInspectTicketLUpdate>();
            foreach (KGSafetyHealthInspectTicketLUpdate ticketL in PXSelectJoin<KGSafetyHealthInspectTicketLUpdate,
                InnerJoin<KGSafetyHealthInspectTicket, On<KGSafetyHealthInspectTicket.safetyHealthInspectionID,
                Equal<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectTicketID>>>,
                Where<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectionID, Equal<Required<KGSafetyHealthInspectTicketLUpdate.safetyHealthInspectionID>>
                , And<KGSafetyHealthInspectTicket.status, Equal<Required<KGSafetyHealthInspectTicket.status>>>>>
                .Select(this, master.SafetyHealthInspectionID, KGMonthInspectionTicketStatuses.Open))
            {
                list.Add(ticketL);
            }
            return list;
        }

        //CheckTicketL for LinePhoto
        public KGSafetyHealthInspectTicketL CheckLinePhotoTicketL(KGSafetyHealthInspectionL detail)
        {
            return PXSelectJoin<KGSafetyHealthInspectTicketL,
                InnerJoin<KGSafetyHealthInspectTicket,
                On<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                Equal<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID>>>,
                Where<KGSafetyHealthInspectTicketL.safetyHealthInspectionID,
                Equal<Required<KGSafetyHealthInspectTicketL.safetyHealthInspectionID>>,
                And<KGSafetyHealthInspectTicketL.safetyHealthInspectionLineID,
                Equal<Required<KGSafetyHealthInspectTicketL.safetyHealthInspectionLineID>>,
                And<KGSafetyHealthInspectTicket.status, Equal<Required<KGSafetyHealthInspectTicket.status>>>>>>
                .Select(this, detail.SafetyHealthInspectionID, detail.SafetyHealthInspectionLineID, "N");
        }

        //CheckTicketL for ShowPhoto
        public KGSafetyHealthInspectTicketL CheckShowPhotoTickerL(string categoryCD, int? SafetyInspectID)
        {
            return PXSelectJoin<KGSafetyHealthInspectTicketL,
                    InnerJoin<KGSafetyHealthInspectTicket,
                    On<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                    Equal<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID>>>,
                Where<KGSafetyHealthInspectTicket.status,
                Equal<Required<KGSafetyHealthInspectTicket.status>>,
                And<KGSafetyHealthInspectTicket.safetyHealthInspectionID,
                Equal<Required<KGSafetyHealthInspectTicket.safetyHealthInspectionID>>,
                And<KGSafetyHealthInspectTicketL.categoryCD,
                Equal<Required<KGSafetyHealthInspectTicketL.categoryCD>>>>>>
                    .Select(this, "N", SafetyInspectID, categoryCD);
        }
        #region Test DeviceHubTest
        private void printReportToDeviceHubTest()
        {
            Dictionary<string, string> printParams = new Dictionary<string, string>();
            printParams["ProjectCD"] = "061C";
            printParams["Contract.ContractCD"] = "061C";
            //printParams["ARInvoice.RefNbr"] = invoice.RefNbr;
            PXReportRequiredException ex = null;

            ex = PXReportRequiredException.CombineReport(ex, "KG605007", printParams);

            PX.SM.SMPrinter p = PXSelect<PX.SM.SMPrinter,
                Where<PX.SM.SMPrinter.deviceHubID, Equal<Required<PX.SM.SMPrinter.deviceHubID>>>>
                .Select(this, "DESKTOP-N551TH5");

            PX.SM.PrintSettings ps = new PX.SM.PrintSettings();
            ps.PrinterID = p.PrinterID;
            ps.PrintWithDeviceHub = true;
            ps.NumberOfCopies = 1;
            ps.DefinePrinterManually = true;


            PX.SM.SMPrintJobMaint.CreatePrintJobGroup(ps, ex, "KG605007"); //Job1
        }
        #endregion

        #region Test Txt to DeviceHub
        private void printTxttoDeviceHub(PXAdapter adapter)
        {
            ArrayList files = GetTxtTemplate();
            PX.SM.FileInfo fi = (PX.SM.FileInfo)files[0];

            if (PXAccess.FeatureInstalled<FeaturesSet.deviceHub>())
                PX.SM.SMPrintJobMaint.CreatePrintJobForRawFile(adapter,
                    new NotificationUtility(this).SearchPrinter,
                    "Customer",
                    "GV602001",
                    Accessinfo.BranchID,
                    new Dictionary<string, string> { { "FILEID", fi.UID.ToString() } },
                    PXMessages.LocalizeFormatNoPrefix("GV602001", fi.ToString()));

        }
        private ArrayList GetTxtTemplate()
        {
            KGSafetyHealthInspection inspection = SafetyHealthInspectionH.Current;
            if (inspection == null)
            {
                return new ArrayList();
            }
            var Cache = this.Caches[typeof(KGSafetyHealthInspection)];

            Guid[] fileIDs = PXNoteAttribute.GetFileNotes(Cache, inspection);
            ArrayList files = new ArrayList(fileIDs.Length);
            var fm = PXGraph.CreateInstance<PX.SM.UploadFileMaintenance>();

            foreach (Guid fileID in fileIDs)
            {
                PX.SM.FileInfo fi = fm.GetFile(fileID);
                files.Add(fi);
            }
            return files;
        }
        #endregion

        #endregion

        #region BQL
        private KGSafetyHealthCategoryDeductionSetup GetZeroPointType() {
            return PXSelect<KGSafetyHealthCategoryDeductionSetup,
                Where<KGSafetyHealthCategoryDeductionSetup.deductionSetup,Equal<Zero>>>.Select(this);
        }

        private KGInspectionFileTemplate GetFileTemplate(String inspectionType)
        {
            return PXSelect<KGInspectionFileTemplate,
                Where<KGInspectionFileTemplate.inspectionType,
                    Equal<Required<KGInspectionFileTemplate.inspectionType>>>>.Select(this, inspectionType);
        }

        private string GetCompanyName(int? branchID)
        {
            //BAccount b= PXSelectJoin<BAccount, 
            //    InnerJoin<Branch ,On<Branch.bAccountID,Equal<BAccount.bAccountID>>>,
            //    Where<Branch.branchID, Equal<AccessInfo.branchID>>>.Select(this);
            KGSafetyHealthInspectionEntry graph = PXGraph.CreateInstance<KGSafetyHealthInspectionEntry>();
            Branch b = PXSelect<Branch,
                Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(this, branchID);
            return b.AcctName;
        }

        private PXResultset<KGSafetyHealthInspectTicketL> GetPPTData(int? safetyHealthInspectionID)
        {
            return PXSelectJoin<KGSafetyHealthInspectTicketL,
                 InnerJoin<KGSafetyHealthInspectTicket, On<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                             Equal<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID>,
                                And<KGSafetyHealthInspectTicket.status, Equal<KGSafetyHealthInspectTicket.pStatusOpen>>>,
                 InnerJoin<KGSafetyHealthCategoryDeductionSetup, On<KGSafetyHealthCategoryDeductionSetup.categoryCD, Equal<KGSafetyHealthInspectTicketL.categoryCD>>>>,
                Where<KGSafetyHealthInspectTicketL.safetyHealthInspectionID, Equal<Required<KGSafetyHealthInspection.safetyHealthInspectionID>>,
                    And<KGSafetyHealthCategoryDeductionSetup.deductionSetup, Less<Zero>>>,//add by alton 20191120 只抓扣分小於0的資料
                OrderBy<Asc<KGSafetyHealthInspectTicketL.safetyHealthInspectionLineID>>>.Select(this, safetyHealthInspectionID);
        }

        private Contract GetContract(int? projectID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID, Equal<Required<Contract.contractID>>>>.Select(this, projectID);
        }

        private PXResultset<KGSafetyHealthInspectTicketL> GetTicketLByLine(int? safetyHealthInspectionLineID)
        {
            return PXSelectJoin<KGSafetyHealthInspectTicketL,
                        InnerJoin<KGSafetyHealthInspectTicket, On<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                             Equal<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID>,
                                And<KGSafetyHealthInspectTicket.status, Equal<KGSafetyHealthInspectTicket.pStatusOpen>>>>,
                        Where<KGSafetyHealthInspectTicketL.safetyHealthInspectionLineID, Equal<Required<KGSafetyHealthInspectTicketL.safetyHealthInspectionLineID>>>>
                        .Select(this, safetyHealthInspectionLineID);
        }

        private PXResultset<KGSafetyHealthInspectTicketL> GetTicketL(int? safetyHealthInspectionID)
        {
            return PXSelectJoin<KGSafetyHealthInspectTicketL,
                        InnerJoin<KGSafetyHealthInspectTicket, On<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                             Equal<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID>,
                                And<KGSafetyHealthInspectTicket.status, Equal<KGSafetyHealthInspectTicket.pStatusOpen>>>>,
                        Where<KGSafetyHealthInspectTicketL.safetyHealthInspectionID, Equal<Required<KGSafetyHealthInspectTicketL.safetyHealthInspectionID>>>>
                        .Select(this, safetyHealthInspectionID);
        }

        //20200225 add by alton :因KGSafetyHealthInspectTicketL.CheckItem的PXDBScalar被拔掉，為了降低危害程度，在此取回checkItem
        private String GetCheckItem(int? templateLineID)
        {
            KGSafetyHealthInspectionTemplateL tl = PXSelect<KGSafetyHealthInspectionTemplateL,
                Where<KGSafetyHealthInspectionTemplateL.templateLineID, Equal<Required<KGSafetyHealthInspectionTemplateL.templateLineID>>>>
                .Select(this, templateLineID);
            if (tl != null)
            {
                return tl.CheckItem;
            }
            return "";
        }




        #endregion
    }

    #region filter
    [Serializable]
    public class KG304002Filter : IBqlTable
    {
        #region CategoryCD
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Category CD")]
        [PXDBStringList(
            typeof(KGSafetyHealthCategoryDeductionSetup),
            typeof(KGSafetyHealthCategoryDeductionSetup.categoryCD),
            typeof(KGSafetyHealthCategoryDeductionSetup.categoryCD))]
        public virtual string CategoryCD { get; set; }
        public abstract class categoryCD : IBqlField { }
        #endregion
    }
    #endregion

}