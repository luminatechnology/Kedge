using System;
using PX.Data;
using PX.Objects.IN;
using Kedge.DAC;
using PX.Objects.CS;
using System.Collections;
using System.Collections.Generic;

namespace Kedge
{
    public class KGMonthlyInspectionTemplate : PXGraph<KGMonthlyInspectionTemplate, KGMonthlyInspectionTemplateH> 
        , PXImportAttribute.IPXPrepareItems
    {
        #region Selector
        public PXSelect<KGMonthlyInspectionTemplateH> MonthlyTemplateH;

        //[PXImport(typeof(KGMonthlyInspectionTemplateS))]
        [PXImport(typeof(KGMonthlyInspectionTemplateH))]
        //[PXImport]
        public PXSelect<KGMonthlyInspectionTemplateS,
                          Where<KGMonthlyInspectionTemplateS.templateHeaderID,
                            Equal<Current<KGMonthlyInspectionTemplateH.templateHeaderID>>>> MonthlyTemplateS;

        [PXImport(typeof(KGMonthlyInspectionTemplateH))]
        public PXSelect<KGMonthlyInspectionTemplateL,
                          Where<KGMonthlyInspectionTemplateL.templateHeaderID,
                            Equal<Current<KGMonthlyInspectionTemplateH.templateHeaderID>>>> MonthlyTemplateL;

        public PXSetup<KGSetUp> KGSetup;

        #endregion
        #region
        public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values) {
            if ("MonthlyTemplateL".Equals(viewName)) {
                if (values["segmentCD"] != null) {
                    String segmentCD = values["segmentCD"].ToString();
                    string[] words = segmentCD.Split('-');
                    values["segmentCD"] = words[0];
                } 
            }
            return true;
        }
        public void PrepareItems(string viewName, IEnumerable items) {
        }
        public bool RowImported(string viewName, object row, object oldRow) {
            String a = viewName;
            return true;
        }
        public virtual bool RowImporting(string viewName, object row) {
            return true;
        }
        #endregion

        #region Events
        #region KGMonthlyInspectionTemplateH
        protected virtual void KGMonthlyInspectionTemplateH_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectionTemplateH row = e.Row as KGMonthlyInspectionTemplateH;
            if (row == null) return;

            bool holdEditable = row.Status != KGMonthInspectionStatuses.Close;
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectionTemplateH.hold>(cache, row, holdEditable);
            bool editable = row.Status == KGMonthInspectionStatuses.Hold;
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectionTemplateH.segmentCD>(cache, row, editable);
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectionTemplateH.description>(cache, row, editable);
            

            PXUIFieldAttribute.SetEnabled(MonthlyTemplateL.Cache, null, editable);
            PXUIFieldAttribute.SetEnabled(MonthlyTemplateS.Cache, null, editable);
            MonthlyTemplateL.AllowInsert = editable;
            MonthlyTemplateL.AllowDelete = editable;
            MonthlyTemplateL.AllowUpdate = editable;
            MonthlyTemplateS.AllowInsert = editable;
            MonthlyTemplateS.AllowDelete = editable;
            MonthlyTemplateS.AllowUpdate = editable;
            Delete.SetEnabled(editable);
         
        }

        protected void KGMonthlyInspectionTemplateH_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspectionTemplateH row = e.Row as KGMonthlyInspectionTemplateH;
            if (row.Hold == true)
            {
                row.Status = KGMonthInspectionStatuses.Hold;
            }
            else
            {
                row.Status = KGMonthInspectionStatuses.Open;
            }

            Boolean? status = null;
            if (e.OldValue == null)
            {
               status = false;
            }
            else {
                status = (Boolean)e.OldValue;
            }
           
            if (row == null) return;
            if (status == false && row.Hold == true)
            {

                row.Status = KGMonthInspectionStatuses.Hold;
                KGMonthlyInspection kgMonthlyInspection = null;
                if (row.TemplateHeaderID > 0) {
                    kgMonthlyInspection= PXSelect<KGMonthlyInspection, Where<KGMonthlyInspection.templateHeaderID, 
                        Equal<Required<KGMonthlyInspectionTemplateH.templateHeaderID>>>>.Select(this, row.TemplateHeaderID);
                }

                if (kgMonthlyInspection != null)
                {
                    row.Hold = false;
                    row.Status = KGMonthInspectionStatuses.Open;
                    MonthlyTemplateH.Update(row);
                    sender.RaiseExceptionHandling<KGMonthlyInspectionTemplateH.hold>(row, row.Hold,
                              new PXSetPropertyException("已有月查核檢查單, 如需修改資料, 請建立新版範本", PXErrorLevel.Error));
                    
                }
                else {
                    row.Status = KGMonthInspectionStatuses.Hold;

                }
            
            }
            else {
                /*
                if (MonthlyTemplateH.Ask("開啟確認", "範本開啟後就不能再修改，請問是否要開啟?", MessageButtons.YesNo) == WebDialogResult.Yes)
                {
                    row.Status = open;
                }
                else {
                    row.Hold = false;
                }*/
            }

            

        }
        protected virtual void KGMonthlyInspectionTemplateH_SegmentCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspectionTemplateH row = e.Row as KGMonthlyInspectionTemplateH;
            if (row == null) return;

            SegmentValue segmentValue = PXSelectJoin<SegmentValue,
                    InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart1>,
                And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>.Select(this, row.SegmentCD);
            if (segmentValue != null)
            {
                row.SegmentDesc = segmentValue.Descr;
            }
            else {
                row.SegmentDesc = null;
            }
            //順便塞個Version
            KGMonthlyInspectionTemplateH latestItem = FetchLastKGMonthlyInspectionTemplateH(row.SegmentCD);
            if (latestItem != null && latestItem.TemplateCD.CompareTo(KGInspectionConstant.NewName) != 0)
            {
                row.Version = latestItem.Version + 1;
            }
            else {
                row.Version = 1;
            }

        }
        #endregion

        #region KGMonthlyInspectionTemplateL

        protected virtual void KGMonthlyInspectionTemplateL_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectionTemplateL row = e.Row as KGMonthlyInspectionTemplateL;
            if (row == null) return;

            /*
            HashSet<String> hashSet = new HashSet<String>();
            foreach (KGMonthlyInspectionTemplateS templateS in MonthlyTemplateS.Select())
            {
                hashSet.Add(templateS.SegmentCD);
            }*/
             //PXStringListAttribute.SetList<KGMonthlyInspectionTemplateL.segmentCD>(cache, row, new string[] { "A00" }, new string[] { "A00-施工品質" });
            
            /*
            PXResultset<SegmentValue> set = PXSelectJoin<SegmentValue,
        InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
            And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
        Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
            And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>>>>.Select(this);
            List<String> key = new List<String>();
            List<String> value = new List<String>();
            foreach (SegmentValue segmentValue in set)
            {
                if (hashSet.Contains(segmentValue.Value))
                {
                    key.Add(segmentValue.Value);
                    value.Add(segmentValue.Value + "-" + segmentValue.Descr);
                }
            }
            PXStringListAttribute.SetList<KGMonthlyInspectionTemplateL.segmentCD>(cache, row, key.ToArray(), value.ToArray());*/
        }


        /*
        public virtual void KGMonthlyInspectionTemplateL_SegmentCD_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if (e.Row != null)
            {
                KGMonthlyInspectionTemplateL row = e.Row as KGMonthlyInspectionTemplateL;
                HashSet< String> hashSet = new HashSet< String>();
                foreach (KGMonthlyInspectionTemplateS templateS in MonthlyTemplateS.Select())
                {
                    hashSet.Add(templateS.SegmentCD);
                }

                PXResultset<SegmentValue> set = PXSelectJoin<SegmentValue,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>>>>.Select(this);
                List<String> key = new List<String>();
                List<String> value = new List<String>();
                foreach (SegmentValue segmentValue in set) {
                    if (hashSet.Contains(segmentValue.Value)) {
                        key.Add(segmentValue.Value);
                        value.Add(segmentValue.Value+"-"+ segmentValue.Descr);
                    }
                }
                PXStringListAttribute.SetList<KGMonthlyInspectionTemplateL.segmentCD>(sender, row, key.ToArray(), value.ToArray());
            }
        }*/
        protected virtual void KGMonthlyInspectionTemplateL_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            if (e.Row != null)
            {
                KGMonthlyInspectionTemplateL row = e.Row as KGMonthlyInspectionTemplateL;
                HashSet<String> hashSet = new HashSet<String>();
                foreach (KGMonthlyInspectionTemplateS templateS in MonthlyTemplateS.Select())
                {
                    hashSet.Add(templateS.SegmentCD);
                }

                PXResultset<SegmentValue> set = PXSelectJoin<SegmentValue,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>>>>.Select(this);
                List<String> key = new List<String>();
                List<String> value = new List<String>();
                foreach (SegmentValue segmentValue in set)
                {
                    if (hashSet.Contains(segmentValue.Value))
                    {
                        key.Add(segmentValue.Value);
                        value.Add(segmentValue.Value + "-" + segmentValue.Descr);
                    }
                }
                PXStringListAttribute.SetList<KGMonthlyInspectionTemplateL.segmentCD>(sender, row, key.ToArray(), value.ToArray());
            }
        }

        protected virtual void KGMonthlyInspectionTemplateL_MaxNoMissing_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGMonthlyInspectionTemplateL row = e.Row as KGMonthlyInspectionTemplateL;
            if (row == null) return;
            int maxNoMissing = 0;
            if (e.NewValue != null)
            {
                if (typeof(int).IsInstanceOfType(e.NewValue))
                {
                    maxNoMissing = (int)e.NewValue;
                }
            }
            if (maxNoMissing < 0)
            {
                sender.RaiseExceptionHandling<KGMonthlyInspectionTemplateL.maxNoMissing>(row, 0,
                       new PXSetPropertyException("缺失數上限需大於等於0", PXErrorLevel.Error));
            }
        }
        #endregion

        #region KGMonthlyInspectionTemplateS
        protected virtual void KGMonthlyInspectionTemplateS_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectionTemplateS row = e.Row as KGMonthlyInspectionTemplateS;
            PXUIFieldAttribute.SetEnabled<KGMonthlyInspectionTemplateS.segmentDesc>(cache, row, false);

        }
        
        protected virtual void KGMonthlyInspectionTemplateS_ScoreSetup_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGMonthlyInspectionTemplateS row = e.Row as KGMonthlyInspectionTemplateS;
            if (row == null) return;
            decimal scoreSetup = 0;
            if (e.NewValue != null) {
                if (typeof(Decimal).IsInstanceOfType(e.NewValue)) {
                    scoreSetup = (decimal)e.NewValue;
                }
                
            }
            if (scoreSetup < 0 || scoreSetup > 100)
            {
                sender.RaiseExceptionHandling<KGMonthlyInspectionTemplateS.scoreSetup>(row, 0,
                       new PXSetPropertyException("項次配分設定需介於 0 ~ 100", PXErrorLevel.Error));
            }
        }
        protected virtual void KGMonthlyInspectionTemplateS_SegmentCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGMonthlyInspectionTemplateS row = e.Row as KGMonthlyInspectionTemplateS;
            if (row == null  || row.SegmentCD == null) {
                if (row!=null) {
                    row.SegmentDesc = null;
                }
                return;
            } 
            SegmentValue segmentValue = PXSelectorAttribute.Select<KGMonthlyInspectionTemplateS.segmentCD>(MonthlyTemplateS.Cache, row) as SegmentValue;
            if (segmentValue != null) {
                row.SegmentDesc = segmentValue.Descr;
            }
            
        }
        #endregion
        public bool beforeSaveCheck()
        {
            bool check = true;
            Dictionary<String, KGMonthlyInspectionTemplateL> dic = new Dictionary<String, KGMonthlyInspectionTemplateL>();
            foreach (KGMonthlyInspectionTemplateL allline in MonthlyTemplateL.Select())
            {

                if (allline.MaxNoMissing == null || allline.MaxNoMissing <=0)
                {
                    check = false;

                    MonthlyTemplateL.Cache.RaiseExceptionHandling<KGMonthlyInspectionTemplateL.maxNoMissing>(allline,
                                                                                                allline.MaxNoMissing,
                                                                                                new PXSetPropertyException("缺失數必須大於0"));
                }
                String key = allline.SegmentCD + "-" + allline.CheckItem;
                if (dic.ContainsKey(key))
                {
                    check = false;

                    MonthlyTemplateL.Cache.RaiseExceptionHandling<KGMonthlyInspectionTemplateL.segmentCD>(allline,
                                                                                                allline.SegmentCD,
                                                                                                new PXSetPropertyException("相同範本單號下，不可有相同的「類別+檢查項目」重複出現", PXErrorLevel.RowError));
                }
                else {
                    dic.Add(key, allline);
                }


            }
            return check;
        }


        #region Global
        public override void Persist()
        {
            KGMonthlyInspectionTemplateH record = (KGMonthlyInspectionTemplateH)MonthlyTemplateH.Current;
            if (record == null)
            {
                base.Persist();
            }
            else {

                if (!ValidScoreSetupPercentage())
                {
                    throw new Exception("項次配分設定比率未達100%");
                }
                if (MonthlyTemplateL.Select().Count == 0) {
                    throw new Exception("請設定檢查項目");
                }

                /*
                MonthlyTemplateL.Cache.RaiseExceptionHandling<KGMonthlyInspectionTemplateL.checkItem>(MonthlyTemplateL.Current, null,
                             new PXSetPropertyException("已有月查核檢查單, 如需修改資料, 請建立新版範本", PXErrorLevel.Error));*/
          
                if (!beforeSaveCheck())
                {
                    return;
                }

                PXEntryStatus status = MonthlyTemplateH.Cache.GetStatus(record);
                foreach (KGMonthlyInspectionTemplateS tempateS in  MonthlyTemplateS.Select()) {
                    String a = "";
                }


                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    if (record != null && status == PXEntryStatus.Inserted)
                    {
                        KGMonthlyInspectionTemplateH latestItem = FetchLastKGMonthlyInspectionTemplateH(record.SegmentCD);
                        if (latestItem != null && latestItem.TemplateCD.CompareTo(KGInspectionConstant.NewName) != 0)
                        {
                            record.Version = latestItem.Version + 1;
                            MonthlyTemplateH.Cache.Update(record);
                        }
                        else
                        {
                            record.Version = 1;
                        }
                        /*
                        PXUpdate<
                               Set<KGMonthlyInspectionTemplateH.status, Required<KGMonthlyInspectionTemplateH.status>>,
                           KGMonthlyInspectionTemplateH,
                               Where<KGMonthlyInspectionTemplateH.segmentCD, Equal<Required<KGMonthlyInspectionTemplateH.segmentCD>>,
                               And<KGMonthlyInspectionTemplateH.status, Equal<Required<KGMonthlyInspectionTemplateH.status>>>>>
                               .Update(this, "C", record.SegmentCD, "O");*/
                    }
                    if (record.Status == KGMonthInspectionStatuses.Open) {
                        //Object[] UpdateStatus = new string[] { KGMonthInspectionStatuses.Open, KGMonthInspectionStatuses.Hold };
                        PXUpdate<Set<KGMonthlyInspectionTemplateH.status, Required<KGMonthlyInspectionTemplateH.status>,
                            Set<KGMonthlyInspectionTemplateH.hold, Required<KGMonthlyInspectionTemplateH.hold>>>,
                            KGMonthlyInspectionTemplateH,
                            Where<KGMonthlyInspectionTemplateH.segmentCD, Equal<Required<KGMonthlyInspectionTemplateH.segmentCD>>,
                                And<KGMonthlyInspectionTemplateH.version, Less<Required<KGMonthlyInspectionTemplateH.version>>,
                                 And <Where 
                                   < KGMonthlyInspectionTemplateH.status, Equal<Required<KGMonthlyInspectionTemplateH.status>>,
                                   Or<KGMonthlyInspectionTemplateH.status, Equal<Required<KGMonthlyInspectionTemplateH.status>>>>>>>>
                                  .Update(this, KGMonthInspectionStatuses.Close, false, record.SegmentCD, record.Version, KGMonthInspectionStatuses.Open, KGMonthInspectionStatuses.Hold);
                        /*
                        PXUpdate<Set<KGMonthlyInspectionTemplateH.status, Required<KGMonthlyInspectionTemplateH.status>,
                              Set<KGMonthlyInspectionTemplateH.hold, Required<KGMonthlyInspectionTemplateH.hold>>>,
                              KGMonthlyInspectionTemplateH,
                              Where<KGMonthlyInspectionTemplateH.segmentCD, Equal<Required<KGMonthlyInspectionTemplateH.segmentCD>>,
                                   And<
                                     Where<KGMonthlyInspectionTemplateH.version, Less<Required<KGMonthlyInspectionTemplateH.version>>> >>>
                                    .Update(this, KGMonthInspectionStatuses.Close, false, record.SegmentCD, record.Version);*/
                    }
                    base.Persist();
                    ts.Complete();
                }
            }

            
        }
        #endregion
        #endregion

        #region Methods

        private KGMonthlyInspectionTemplateH FetchLastKGMonthlyInspectionTemplateH(string segmentCD)
        {
            PXGraph graph = new PXGraph();
            KGMonthlyInspectionTemplateH item = PXSelect<KGMonthlyInspectionTemplateH,
                Where<KGMonthlyInspectionTemplateH.segmentCD, Equal<Required<KGMonthlyInspectionTemplateH.segmentCD>>>,
                OrderBy<Desc<KGMonthlyInspectionTemplateH.templateCD>>>.SelectSingleBound(graph, null, segmentCD);
            return item;
        }

        private PXResultset<Segment> GetSegements(string dimensionID)
        {
            PXResultset<Segment> set = PXSelect<Segment, Where<Segment.dimensionID,
                                                    Equal<Required<Segment.dimensionID>>>,
                                                    OrderBy<Asc<Segment.dimensionID>>>.Select(this, dimensionID);
            return set;
        }

        private SegmentValue GetSegmentValue(string dimensionID, short segmentID, string value)
        {

            SegmentValue item = PXSelect<SegmentValue, Where<SegmentValue.dimensionID,
                                                        Equal<Required<SegmentValue.dimensionID>>,
                                                    And<SegmentValue.segmentID, Equal<Required<SegmentValue.segmentID>>,
                                                    And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>.Select(this, dimensionID, segmentID, value);
            return item;
        }
        private bool ValidScoreSetupPercentage()
        {
            bool valid = false;
            decimal totalPercentage = 0;
            foreach(KGMonthlyInspectionTemplateS row in MonthlyTemplateS.Select())
            {
                totalPercentage += (decimal)row.ScoreSetup;
            }

            if (totalPercentage == 100)
            {
                valid = true;
            }
            return valid;
        }

        private string GetSegmentValueDescrs(string segmentCD)
        {
            PXResultset<Segment> items = GetSegements(KGInspectionConstant.KGMNINSC);

            ArrayList descrs = new ArrayList();
            int startPos = 0;
            foreach (Segment seg in items)
            {
                int len = (Int32)seg.Length;
                string value = segmentCD.Substring(startPos, (Int32)seg.Length-1);
                SegmentValue item = GetSegmentValue(KGInspectionConstant.KGMNINSC, (short)seg.SegmentID, value);
                if (item != null)
                {
                    descrs.Add(item.Descr);
                }
                startPos += len;
            }

            if (descrs.Count == 0)
            {
                return null;
            }
            return string.Join(",", (string[])descrs.ToArray(Type.GetType("System.String")));
        }
        #endregion
    }
}