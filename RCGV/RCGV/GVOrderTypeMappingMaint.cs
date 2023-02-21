using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using RCGV.GV.SYS;
using System.Threading;
using System.Threading.Tasks;
using RCGV.GV.Util;
using PX.Objects.CR;

namespace RCGV.GV
{
    public class GVOrderTypeMappingMaint : GVBaseViewGraph<GVOrderTypeMappingMaint>
    {
        public PXSave<GVOrderTypeMapping> Save;
        public PXSelect<GVOrderTypeMapping> OrderTypes;
       

        protected virtual void GVOrderTypeMapping_RowSelected(PXCache sender ,PXRowSelectedEventArgs e)
        {
            GVOrderTypeMapping row = (GVOrderTypeMapping)e.Row;

            if (row == null)
            {
                return;

            }
            GVLookUpCodeDefinition def = GetLookUpCodeDefinition();
            //2020/04/09 ADD 若生效日未勾起,出現error
            CheckLookUpCode();



            if (row.GvType == GVOrderTypeMapping.GVType1.I)
            {
                Dictionary<String, String> listordertype = def.dic[GVLookUpCodeUtil.InOrderType];

                string[] keysordertype = new string[listordertype.Keys.Count];
                listordertype.Keys.CopyTo(keysordertype, 0);
                string[] valuesordertype = new string[listordertype.Values.Count];
                listordertype.Values.CopyTo(valuesordertype, 0);
                PXStringListAttribute.SetList<GVOrderTypeMapping.orderTypeCD>(sender, row, keysordertype, valuesordertype);

                Dictionary<String, String> list = def.dic[GVLookUpCodeUtil.InGuiSubType];
                string[] keys = new string[list.Keys.Count];
                list.Keys.CopyTo(keys, 0);
                string[] values = new string[list.Values.Count];
                list.Values.CopyTo(values, 0);
                PXStringListAttribute.SetList<GVOrderTypeMapping.guiSubType>(sender, row, keys, values);
            }
            else if (row.GvType == GVOrderTypeMapping.GVType1.O)
            {


                Dictionary<String, String> listordertype = def.dic[GVLookUpCodeUtil.OutOrderType];
                string[] keys = new string[listordertype.Keys.Count];
                listordertype.Keys.CopyTo(keys, 0);
                string[] values = new string[listordertype.Values.Count];
                listordertype.Values.CopyTo(values, 0);
                PXStringListAttribute.SetList<GVOrderTypeMapping.orderTypeCD>(sender, row, keys, values);

                Dictionary<String, String> listguisubtype = def.dic[GVLookUpCodeUtil.OutGuiSubType];
                string[] keysgui = new string[listguisubtype.Keys.Count];
                listguisubtype.Keys.CopyTo(keysgui, 0);
                string[] valuesgui = new string[listguisubtype.Values.Count];
                listguisubtype.Values.CopyTo(valuesgui, 0);
                PXStringListAttribute.SetList<GVOrderTypeMapping.guiSubType>(sender, row, keysgui, valuesgui);
            }
            else if (row.GvType == null || row.GvType == "")
            {
                Dictionary<String, String> listInordertype = def.dic[GVLookUpCodeUtil.InOrderType];
                Dictionary<String, String> listOutordertype = def.dic[GVLookUpCodeUtil.OutOrderType];
                string[] keysordertype = new string[listInordertype.Keys.Count + listOutordertype.Keys.Count];
                listInordertype.Keys.CopyTo(keysordertype, 0);
                listOutordertype.Keys.CopyTo(keysordertype, 4);
                string[] valuesordertype = new string[listInordertype.Values.Count + listOutordertype.Values.Count];
                listInordertype.Values.CopyTo(valuesordertype, 0);
                listOutordertype.Values.CopyTo(valuesordertype, 4);
                PXStringListAttribute.SetList<GVOrderTypeMapping.orderTypeCD>(sender, row, keysordertype, valuesordertype);

                Dictionary<String, String> listInGuiSubtype = def.dic[GVLookUpCodeUtil.InGuiSubType];
                Dictionary<String, String> listOutGuiSubtype = def.dic[GVLookUpCodeUtil.OutGuiSubType];

                string[] keys = new string[listInGuiSubtype.Keys.Count + listOutGuiSubtype.Keys.Count];
                listInGuiSubtype.Keys.CopyTo(keys, 0);
                listOutGuiSubtype.Keys.CopyTo(keys, 2);
                string[] values = new string[listInGuiSubtype.Values.Count + listOutGuiSubtype.Values.Count];
                listInGuiSubtype.Values.CopyTo(values, 0);
                listOutGuiSubtype.Values.CopyTo(values, 2);
                PXStringListAttribute.SetList<GVOrderTypeMapping.guiSubType>(sender, row, keys, values);
            }
        }

        #region Verifying
        //OrderType cannot duplecate
        protected virtual void GVOrderTypeMapping_OrderTypeCD_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVOrderTypeMapping row = (GVOrderTypeMapping)e.Row;
            // null not check 
            if (row == null)
            {
                return;
            }
            setCheckThread(checkOrderTypeCD_M(sender, row, (String)e.NewValue));
        }
        public bool checkOrderTypeCD_M(PXCache sender, GVOrderTypeMapping row, String newValue)
        {
            PXResultset<GVOrderTypeMapping> set = PXSelect<GVOrderTypeMapping,
                       Where<GVOrderTypeMapping.orderTypeCD,
                           Equal<Required<GVOrderTypeMapping.orderTypeCD>>,
                           And<GVOrderTypeMapping.gvType,
                           Equal<Required<GVOrderTypeMapping.gvType>>>>>
                               .Select(this, newValue, row.GvType);
            foreach (GVOrderTypeMapping gvline in set)
            {
    
                if (gvline.OrderTypeCD == null || "".Equals(gvline.OrderTypeCD))
                {
                    continue;
                }
                if (gvline.OrderTypeCD.Equals(newValue) && gvline.GvType.Equals(row.GvType))
                {
                    if(gvline.OrderTypeMappingID.Equals(row.OrderTypeMappingID))
                    {
                        continue;
                    }
                    sender.RaiseExceptionHandling<GVOrderTypeMapping.orderTypeCD>(
                                        row, newValue,
                                                new PXSetPropertyException("OrderTypeCD is duplicate.", PXErrorLevel.Error));
                    return true;
                }
            }
            return false;
        }

        //mark by emily 2020/03/25
        //Date Verifying
        /*protected virtual void GVOrderTypeMapping_EffectiveDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVOrderTypeMapping row = (GVOrderTypeMapping)e.Row;
            if (row == null || e.NewValue == null)
            {
                return;
            }
            setCheckThread(checkEffectiveDate_M(sender, row, (DateTime)e.NewValue));
        }
        public bool checkEffectiveDate_M(PXCache sender, GVOrderTypeMapping row, DateTime? newValue)
        {
            DateTime? startDate = newValue;
            DateTime? endDate = row.ExpirationDate;
            if (startDate == null)
            {
                return false;
            }
            if (endDate == null)
            {
                return false;
            }
            if (((DateTime)startDate).CompareTo(endDate) > 0)
            {
                sender.RaiseExceptionHandling<GVOrderTypeMapping.effectiveDate>(
                                        row, startDate,
                                                new PXSetPropertyException("EffectiveDate have to bigger than ExpirationDate. ", PXErrorLevel.Error));
                sender.RaiseExceptionHandling<GVOrderTypeMapping.expirationDate>(
                                        row, endDate,
                                                new PXSetPropertyException("ExpirationDate have to smaller than EffectiveDate.", PXErrorLevel.Error));
                return true;
            }
            return false;
        }
        protected virtual void GVOrderTypeMapping_ExpirationDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVOrderTypeMapping row = (GVOrderTypeMapping)e.Row;
            if (row == null || e.NewValue == null)
            {
                return;
            }
            setCheckThread(checkExpirationDate_M(sender, row, (DateTime)e.NewValue));
        }
        public bool checkExpirationDate_M(PXCache sender, GVOrderTypeMapping row, DateTime? newValue)
        {
            //DateTime startDate = (DateTime)sender.GetValue<GVLookupCodeValue.effectiveDate>(row);
            DateTime? startDate = row.EffectiveDate;
            DateTime? endDate = (DateTime?)newValue;
            if (startDate == null)
            {
                return false;
            }
            if (endDate == null)
            {
                return false;
            }
            if (((DateTime)startDate).CompareTo(endDate) > 0)
            {
                sender.RaiseExceptionHandling<GVOrderTypeMapping.effectiveDate>(
                                        row, startDate,
                                                new PXSetPropertyException("EffectiveDate have to bigger than ExpirationDate. ", PXErrorLevel.Error));
                sender.RaiseExceptionHandling<GVOrderTypeMapping.expirationDate>(
                                        row, endDate,
                                                new PXSetPropertyException("ExpirationDate have to smaller than EffectiveDate.", PXErrorLevel.Error));
                return true;
            }
            return false;
        }*/
        #endregion 

        protected virtual void GVOrderTypeMapping_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            GVOrderTypeMapping row = (GVOrderTypeMapping)e.Row;
            setCheckThread(checkOrderTypeCD_M(sender, row, row.OrderTypeCD));
            // setCheckThread(checkEffectiveDate_M(sender, row, row.EffectiveDate)); mark by emily 2020/03/25
            // setCheckThread(checkExpirationDate_M(sender, row, row.ExpirationDate)); mark by emily 2020/03/25
            e.Cancel = getCheckThread();
        }
        public override void Persist()
        {

            base.Persist();
            if (getCheckThread())
            {
                throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
            }
        }

        #region Method 
        private GVLookUpCodeDefinition GetLookUpCodeDefinition()
        {
            return PXDatabase.GetSlot<GVLookUpCodeDefinition>(
                    typeof(GVLookUpCodeDefinition).Name, typeof(GVLookupCodeType), typeof(GVLookupCodeValue));
        }

        public virtual void CheckLookUpCode()
        {
            ArrayList getError = new ArrayList();
            string getErrortxt = "";
            GVLookUpCodeDefinition def = GetLookUpCodeDefinition();
            if(!def.dic.ContainsKey("InOrderType"))
            {
                getError.Add("InOrderType");
                getErrortxt = getErrortxt+"InOrderType";
            }
            if(!def.dic.ContainsKey("InGuiSubType"))
            {
                getError.Add("InGuiSubType");
                getErrortxt = getErrortxt + ",InGuiSubType";
            }
            if(!def.dic.ContainsKey("OutOrderType"))
            {
                getError.Add("OutOrderType");
                getErrortxt = getErrortxt+ ",OutOrderType";
            }
            if (!def.dic.ContainsKey("OutGuiSubType"))
            {
                getError.Add("OutGuiSubType");
                getErrortxt = getErrortxt + ",OutGuiSubType";

            }
            if (getError.Count!=0)
            {

                throw new Exception(string.Format("請至GVLookUpCode設定, 設定{0}的生失效!", getErrortxt));
            }

        }
        #endregion
    }
}


