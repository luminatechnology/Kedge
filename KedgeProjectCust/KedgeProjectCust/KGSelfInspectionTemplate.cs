using System;
using System.Collections;
using PX.Data;
using Kedge.DAC;
using PX.Objects.CS;

namespace Kedge
{
    public class KGSelfInspectionTemplate : PXGraph<KGSelfInspectionTemplate, KGSelfInspectionTemplateH>
    {
        #region Selector
        public PXSelect<KGSelfInspectionTemplateH> TemplateH;
        [PXImport(typeof(KGSelfInspectionTemplateH))]
        public PXSelect<KGSelfInspectionTemplateL,
            Where<KGSelfInspectionTemplateL.templateHeaderID,
                    Equal<Current<KGSelfInspectionTemplateH.templateHeaderID>>>> TemplateL;

        #endregion

        #region Events
        #region KGInspectionTemplateH
        protected void KGSelfInspectionTemplateH_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGSelfInspectionTemplateH row = e.Row as KGSelfInspectionTemplateH;
            if (row == null) return;
            //Hold 可維護全部欄位；Open & Close 全部欄位Disable
            bool editable = row.Status == KGSelfInspectionStatuses.Hold;

            //處理別名
            if (row.SegmentCD != null)
            {
                row.SegmentDesc = this.GetSegmentValueDescrs(row.SegmentCD);
            }
            FieldDeiabled(cache, row);//處理欄位disabled

            Delete.SetEnabled(editable);
            TemplateL.AllowInsert = editable;
            TemplateL.AllowDelete = editable;
            TemplateL.AllowUpdate = editable;


        }

        protected virtual void KGSelfInspectionTemplateH_SegmentCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSelfInspectionTemplateH row = e.Row as KGSelfInspectionTemplateH;
            if (row == null) return;

            String descr = GetSegmentValueDescrs(row.SegmentCD);
            row.SegmentDesc = descr;
        }

        /**
         Hold 欄位更新
             */
        protected virtual void KGSelfInspectionTemplateH_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSelfInspectionTemplateH row = e.Row as KGSelfInspectionTemplateH;
            if (row == null) return;
            if (row.Status == KGSelfInspectionStatuses.Close) return;



            //查詢Open狀態下，該範本是否已被使用，已被使用則不可再修改Hold，並且不可刪除
            if (row.Status == KGSelfInspectionStatuses.Open)
            {
                PXResultset<KGSelfInspection> set = PXSelect<KGSelfInspection, Where<KGSelfInspection.templateHeaderID, Equal<Required<KGSelfInspection.templateHeaderID>>>>.Select(this, row.TemplateHeaderID);
                if (set.Count > 0)
                {
                    row.Hold = false;
                    String msg = "已有自主檢查單, 如需修改資料, 請建立新版範本";
                    sender.RaiseExceptionHandling<KGSelfInspectionTemplateH.hold>(row, row.Hold,
                             new PXSetPropertyException(msg, PXErrorLevel.Error));
                    return;
                };
            }

            if (row.Hold == false)
            {
                row.Status = KGSelfInspectionStatuses.Open;
            }
            else if (row.Hold == true)
            {
                row.Status = KGSelfInspectionStatuses.Hold;
            }
        }

        public override void Persist()
        {

            if (CheckCheckItem())
            {
                KGSelfInspectionTemplateH record = (KGSelfInspectionTemplateH)TemplateH.Current;
                PXEntryStatus status = TemplateH.Cache.GetStatus(record);

                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    if (record != null)
                    {
                        if (status == PXEntryStatus.Inserted)
                        {
                            KGSelfInspectionTemplateH latestItem = FetchLastKGSelfInspectionTemplateH(record.SegmentCD);
                            if (latestItem != null && latestItem.TemplateHeaderID > 0)
                            {
                                record.Version = latestItem.Version + 1;
                                TemplateH.Cache.Update(record);
                            }
                        }
                        if (record.Status == KGSelfInspectionStatuses.Open && (status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated))
                        {
                            //當前狀態為OPEN時才去關閉其他單
                            //status→close & hold→false
                            PXUpdate<
                                   Set<KGSelfInspectionTemplateH.status, Required<KGSelfInspectionTemplateH.status>,
                                   Set<KGSelfInspectionTemplateH.hold, Required<KGSelfInspectionTemplateH.hold>>
                                   >,
                               KGSelfInspectionTemplateH,
                                   Where<KGSelfInspectionTemplateH.segmentCD, Equal<Required<KGSelfInspectionTemplateH.segmentCD>>,
                                  And<
                                    Where<
                                        KGSelfInspectionTemplateH.status, NotEqual<Required<KGSelfInspectionTemplateH.status>>,
                                        And<
                                            KGSelfInspectionTemplateH.version, Less<Required<KGSelfInspectionTemplateH.version>>
                                        >
                                    >
                                >>>
                                   .Update(this, KGSelfInspectionStatuses.Close, false, record.SegmentCD, KGSelfInspectionStatuses.Close, record.Version);
                        }
                    }

                    ts.Complete();
                }
            }
            base.Persist();
        }
        #endregion
        #endregion

        #region Methods

        /**
         * 欄位是否可修改
         * Status = Hold 全部欄位可維護
         * Status = Open 僅提供Hold欄位可維護
        **/
        private void FieldDeiabled(PXCache cache, KGSelfInspectionTemplateH row)
        {
            bool isNew = row.TemplateHeaderID < 0;
            bool isHold = row.Status == KGSelfInspectionStatuses.Hold;
            bool isOpen = row.Status == KGSelfInspectionStatuses.Open;
            //存檔後，「自主檢查類別」要變唯讀。
            PXUIFieldAttribute.SetEnabled<KGSelfInspectionTemplateH.segmentCD>(cache, row, isNew);
            PXUIFieldAttribute.SetEnabled<KGSelfInspectionTemplateH.description>(cache, row, isHold);
            PXUIFieldAttribute.SetEnabled<KGSelfInspectionTemplateH.hold>(cache, row, isHold || isOpen);
            PXUIFieldAttribute.SetEnabled(TemplateL.Cache, null, isHold);
            Delete.SetEnabled(true);

        }

        private KGSelfInspectionTemplateH FetchLastKGSelfInspectionTemplateH(string segmentCD)
        {
            KGSelfInspectionTemplateH item = PXSelect<KGSelfInspectionTemplateH,
                Where<KGSelfInspectionTemplateH.segmentCD, Equal<Required<KGSelfInspectionTemplateH.segmentCD>>>,
                OrderBy<Desc<KGSelfInspectionTemplateH.templateCD>>>.SelectSingleBound(this, null, segmentCD);
            return item;
        }

        private int[] GetSegementsLength(string dimensionID)
        {
            PXResultset<Segment> set = GetSegements(dimensionID);
            ArrayList lengths = new ArrayList();
            foreach (Segment item in set)
            {
                lengths.Add(item.Length);

            }
            return lengths.ToArray(typeof(int)) as int[];
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

        private string GetSegmentValueDescrs(string segmentCD)
        {
            PXResultset<Segment> items = GetSegements(KGSelfInspectionTemplateH.DimensionName);

            ArrayList descrs = new ArrayList();
            if (segmentCD != null)
            {
                int startPos = 0;
                foreach (Segment seg in items)
                {
                    int len = (Int32)seg.Length;
                    string value = segmentCD.Substring(startPos, (Int32)seg.Length);
                    SegmentValue item = GetSegmentValue(KGSelfInspectionTemplateH.DimensionName, (short)seg.SegmentID, value);
                    if (item != null)
                    {
                        descrs.Add(item.Descr);
                    }
                    startPos += len;
                }
            }

            if (descrs.Count == 0)
            {
                return null;
            }
            return string.Join(",", (string[])descrs.ToArray(Type.GetType("System.String")));
        }

        /**
         <summary>檢核CheckItem是否重複</summary>
             */
        private bool CheckCheckItem()
        {
            foreach (KGSelfInspectionTemplateL d1 in TemplateL.Select())
            {
                String msg = null;
                foreach (KGSelfInspectionTemplateL d2 in TemplateL.Select())
                {
                    if (d1.CheckItem == d2.CheckItem && d1.TemplateLineID != d2.TemplateLineID)
                    {
                        msg = "不得重複";
                        TemplateL.Cache.RaiseExceptionHandling<KGSelfInspectionTemplateL.checkItem>(d2, d2.CheckItem,
                                 new PXSetPropertyException(msg, PXErrorLevel.Error));
                    }
                }
                if (msg != null)
                {
                    TemplateL.Cache.RaiseExceptionHandling<KGSelfInspectionTemplateL.checkItem>(d1, d1.CheckItem,
                                 new PXSetPropertyException(msg, PXErrorLevel.Error));
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}