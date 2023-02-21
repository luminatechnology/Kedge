using System;
using PX.Data;
using Kedge.DAC;
using PX.Objects.CS;
using System.Collections.Generic;

namespace Kedge
{
    public class KGSafetyHealthInspectionTemplate : PXGraph<KGSafetyHealthInspectionTemplate, KGSafetyHealthInspectionTemplateH>
    {
        #region Select
        public PXSelect<KGSafetyHealthInspectionTemplateH> SafetyHealthTemplateH;

        [PXImport(typeof(KGSafetyHealthInspectionTemplateL))]
        public PXSelect<KGSafetyHealthInspectionTemplateL,
            Where<KGSafetyHealthInspectionTemplateL.templateHeaderID,
                Equal<Current<KGSafetyHealthInspectionTemplateH.templateHeaderID>>>> SafetyHealthTemplateL;

        [PXImport(typeof(KGSafetyHealthInspectionTemplateS))]
        public PXSelect<KGSafetyHealthInspectionTemplateS,
            Where<KGSafetyHealthInspectionTemplateS.templateHeaderID,
                Equal<Current<KGSafetyHealthInspectionTemplateH.templateHeaderID>>>> SafetyHealthTemplateS;

        public PXSetup<KGSetUp> KGSetup;
        #endregion

        #region Event

        #region SafetyHealthTemplateH
        protected virtual void KGSafetyHealthInspectionTemplateH_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateH templateH = (KGSafetyHealthInspectionTemplateH)e.Row;
            if (templateH == null) return;
            EnableControl();
            //�YStatus������->���i�R��
            if (templateH.Status == "C")
            {
                Delete.SetEnabled(false);
            }
            //�YStatus���}��,�]�S�Q¾�w�èϥιL���d��->�i�R��
            //�YStatus���}��,�Q¾�w�èϥιL���d��->���i�R��
            if (templateH.Status == "N")
            {

                Delete.SetEnabled(true);

            }

            //2019/11/13 ����
            /*if(templateH.SegmentCD !=null)
             {
                 SegmentValue segmentValue = GetSegmentValue(KGInspectionConstant.SegmentID_PART1, templateH.SegmentCD);
                 templateH.SegmentDesc = segmentValue.Descr;
             }*/
        }
        protected virtual void KGSafetyHealthInspectionTemplateH_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateH templateH = (KGSafetyHealthInspectionTemplateH)e.Row;
            if (templateH == null) return;
            //2019/11/13 ADD �s�L��,���O����Ū
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionTemplateH.segmentCD>(SafetyHealthTemplateH.Cache, templateH, templateH.TemplateHeaderID < 0);

        }
        protected virtual void KGSafetyHealthInspectionTemplateH_SegmentCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateH templateH = (KGSafetyHealthInspectionTemplateH)e.Row;
            if (templateH == null) return;
            //2019/11/13 ����
           /* if(templateH.SegmentCD !=null)
            {
                SegmentValue segmentValue = GetSegmentValue(KGInspectionConstant.SegmentID_PART1, templateH.SegmentCD);
                templateH.SegmentDesc = segmentValue.Descr;
            }*/
        }
        protected virtual void KGSafetyHealthInspectionTemplateH_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateH row = e.Row as KGSafetyHealthInspectionTemplateH;
            if (row.Hold == true)
            {
                row.Status = "H";
            }
            else
            {
                row.Status = "N";
            }

            Boolean? status = null;
            if (e.OldValue == null)
            {
                status = false;
            }
            else
            {
                status = (Boolean)e.OldValue;
            }

            if (row == null) return;
            if (status == false && row.Hold == true)
            {

                row.Status = "H";
                KGSafetyHealthInspection kGSafetyHealthInspection = null;
                if (row.TemplateHeaderID > 0)
                {
                    kGSafetyHealthInspection = PXSelect<KGSafetyHealthInspection, Where<KGSafetyHealthInspection.templateHeaderID,
                        Equal<Required<KGSafetyHealthInspection.templateHeaderID>>>>.Select(this, row.TemplateHeaderID);
                }

                if (kGSafetyHealthInspection != null)
                {
                    row.Hold = false;
                    row.Status = "N";
                    SafetyHealthTemplateH.Update(row);
                    sender.RaiseExceptionHandling<KGSafetyHealthInspectionTemplateH.hold>(row, row.Hold,
                              new PXSetPropertyException("�w��¾�w���ˬd��, �p�ݭק���, �Ыإ߷s���d��", PXErrorLevel.Error));

                }
                else
                {
                    row.Status = "H";

                }

            }
        }
        
        #endregion

        #region SafetyHealthTemplateS
        protected virtual void KGSafetyHealthInspectionTemplateS_ScoreSetup_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null) return;
            KGSafetyHealthInspectionTemplateS row = e.Row as KGSafetyHealthInspectionTemplateS;
            if (row == null) return;
            decimal scoreSetup = 0;
            if (e.NewValue != null)
            {
                if (typeof(Decimal).IsInstanceOfType(e.NewValue))
                {
                    scoreSetup = (decimal)e.NewValue;
                }

            }
            if (scoreSetup < 0 || scoreSetup > 100)
            {
                sender.RaiseExceptionHandling<KGSafetyHealthInspectionTemplateS.scoreSetup>(row, 0,
                       new PXSetPropertyException("�����t���]�w�ݤ��� 0 ~ 100", PXErrorLevel.Error));
            }
        }
        protected virtual void KGSafetyHealthInspectionTemplateS_SegmentCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateS templateS = (KGSafetyHealthInspectionTemplateS)e.Row;
            if (templateS == null) return;
            if (templateS.SegmentCD != null)
            {
                /*SegmentValue segmentValue = PXSelectJoin<SegmentValue,
                    InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>,
                And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>
                .Select(this, templateS.SegmentCD);*/
                SegmentValue segmentValue = GetSegmentValue(KGInspectionConstant.SegmentID_PART2, templateS.SegmentCD);
                templateS.SegmentDesc = segmentValue.Descr;
            }
            //2019/11/13 �Y��null �M��¾�w�����O�y�z
            else
            {
                templateS.SegmentDesc = null;
            }
        }
        protected virtual void KGSafetyHealthInspectionTemplateS_RowSelected(PXCache sender,PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateS templateS = e.Row as KGSafetyHealthInspectionTemplateS;
            if (templateS == null) return;
            if (templateS.SegmentCD!=null)
            {
                SegmentValue segmentValue = GetSegmentValue(KGInspectionConstant.SegmentID_PART2, templateS.SegmentCD);
                templateS.SegmentDesc = segmentValue.Descr;
            }
        }
        #endregion

        #region SafetyHealthTemplateL
        protected virtual void KGSafetyHealthInspectionTemplateL_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateL row = e.Row as KGSafetyHealthInspectionTemplateL;
            if (row == null) return;
  
            HashSet<String> hashSet = new HashSet<String>();
            foreach (KGSafetyHealthInspectionTemplateS templateS in SafetyHealthTemplateS.Select())
            {
                hashSet.Add(templateS.SegmentCD);
            }

            PXResultset<SegmentValue> set = PXSelectJoin<SegmentValue,
        InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
            And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
        Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
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
            PXStringListAttribute.SetList<KGSafetyHealthInspectionTemplateL.segmentCD>(cache, row, key.ToArray(), value.ToArray());
        }
        protected virtual void KGSafetyHealthInspectionTemplateL_SegmentCD_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGSafetyHealthInspectionTemplateL row = e.Row as KGSafetyHealthInspectionTemplateL;
            int Index = row.SegmentCD.IndexOf("-");
            if (Index == -1) return;
            row.SegmentCD = row.SegmentCD.Remove(Index);
        }
        #endregion

        #endregion

        #region Method
        private void EnableControl()
        {
            KGSafetyHealthInspectionTemplateH templateH = SafetyHealthTemplateH.Current;
            KGSafetyHealthInspectionTemplateS templateS = SafetyHealthTemplateS.Current;
            KGSafetyHealthInspectionTemplateL templateL = SafetyHealthTemplateL.Current;

            //�Y���A!=Hold ��줣�i���@ ���F�i�H��d���渹
            bool editable = templateH.Status == "H";
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionTemplateH.hold>(SafetyHealthTemplateH.Cache, templateH, templateH.Status =="H"||templateH.Status=="N");
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionTemplateH.segmentCD>(SafetyHealthTemplateH.Cache, templateH, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionTemplateH.description>(SafetyHealthTemplateH.Cache, templateH, editable);

            //2019/12/11 ADD TemplateS �y�z������Ū,��L��۪��A�}��
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionTemplateS.segmentCD>(SafetyHealthTemplateS.Cache, templateS, editable);
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionTemplateS.scoreSetup>(SafetyHealthTemplateS.Cache, templateS, editable);

            PXUIFieldAttribute.SetEnabled(SafetyHealthTemplateL.Cache, templateL, editable);
            SafetyHealthTemplateL.AllowInsert = editable;
            SafetyHealthTemplateL.AllowUpdate = editable;
            SafetyHealthTemplateL.AllowDelete = editable;
            SafetyHealthTemplateS.AllowInsert = editable;
            SafetyHealthTemplateS.AllowUpdate = editable;
            SafetyHealthTemplateS.AllowDelete = editable;

            //2019/11/13 ADD �s�L��,���O����Ū
            PXUIFieldAttribute.SetEnabled<KGSafetyHealthInspectionTemplateH.segmentCD>(SafetyHealthTemplateH.Cache, templateH, templateH.TemplateHeaderID<0);
        }

        //�ۦP���ODB�̫�@���d��
        private KGSafetyHealthInspectionTemplateH LastTemplate(string segmentCD)
        {
            KGSafetyHealthInspectionTemplateH item = PXSelect<KGSafetyHealthInspectionTemplateH,
                Where<KGSafetyHealthInspectionTemplateH.segmentCD, Equal<Required<KGSafetyHealthInspectionTemplateH.segmentCD>>>,
                OrderBy<Desc<KGSafetyHealthInspectionTemplateH.templateCD>>>.SelectSingleBound(this, null, segmentCD);
            return item;
        }

        //�j�MSegmentValue
        private SegmentValue GetSegmentValue(string part,string SegmentCD)
        {
            return PXSelectJoin<SegmentValue,
                    InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
                And<Segment.segmentID, Equal<Required<Segment.segmentID>>,
                And<SegmentValue.value, Equal<Required<SegmentValue.value>>>>>>
                .Select(this, part,SegmentCD);
        }

        public override void Persist()
        {
            KGSafetyHealthInspectionTemplateH templateH = SafetyHealthTemplateH.Current;
            if (templateH == null) return;
            #region Check
            if (!ValidScoreSetupPercentage())
            {
                throw new Exception("�нT�w�����t���]�w�`��v��100%!");
            }
            #endregion

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                KGSafetyHealthInspectionTemplateH record = (KGSafetyHealthInspectionTemplateH)SafetyHealthTemplateH.Current;
                PXEntryStatus status = SafetyHealthTemplateH.Cache.GetStatus(record);
                if (record != null && status == PXEntryStatus.Inserted)
                {
                    //����+1
                    KGSafetyHealthInspectionTemplateH set =
                        PXSelectGroupBy<KGSafetyHealthInspectionTemplateH,
                        Where<KGSafetyHealthInspectionTemplateH.segmentCD,
                        Equal<Required<KGSafetyHealthInspectionTemplateH.segmentCD>>>,
                        Aggregate<GroupBy<KGSafetyHealthInspectionTemplateH.segmentCD,
                        Max<KGSafetyHealthInspectionTemplateH.version>>>>
                        .Select(this, templateH.SegmentCD);
                    if (set == null)
                    {
                        templateH.Version = 1;
                    }
                    else
                    {
                        templateH.Version = set.Version + 1;
                    }

                    //�ˬd���L�P���O,�Y���⤧�e���d�����A�令close
                    PXUpdate<
                              Set<KGSafetyHealthInspectionTemplateH.status, Required<KGSafetyHealthInspectionTemplateH.status>>,
                          KGSafetyHealthInspectionTemplateH,
                              Where<KGSafetyHealthInspectionTemplateH.segmentCD, Equal<Required<KGSafetyHealthInspectionTemplateH.segmentCD>>>>
                              .Update(this, "C", record.SegmentCD);
                    PXUpdate<
                              Set<KGSafetyHealthInspectionTemplateH.hold, Required<KGSafetyHealthInspectionTemplateH.hold>>,
                          KGSafetyHealthInspectionTemplateH,
                              Where<KGSafetyHealthInspectionTemplateH.segmentCD, Equal<Required<KGSafetyHealthInspectionTemplateH.segmentCD>>>>
                              .Update(this, false, record.SegmentCD);

                }
                base.Persist();
                ts.Complete();
            }
        }

        private bool ValidScoreSetupPercentage()
        {
            bool valid = false;
            decimal totalPercentage = 0;
            foreach (KGSafetyHealthInspectionTemplateS row in SafetyHealthTemplateS.Select())
            {
                totalPercentage += (decimal)row.ScoreSetup;
            }

            if (totalPercentage == 100)
            {
                valid = true;
            }
            return valid;
        }
        #endregion

    }
}