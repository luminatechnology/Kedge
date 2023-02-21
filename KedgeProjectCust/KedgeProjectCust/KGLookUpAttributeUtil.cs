using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CS;

namespace Kedge
{
    public class KGLookUpAttributeUtil
    {

    }

    public class KGLookUpCodeDefinition : IPrefetchable
    {
        public Dictionary<String, Dictionary<String, String>> dic = new Dictionary<String, Dictionary<String, String>>();
        public Dictionary<String, String> masterDic = new Dictionary<String, String>();
        public void Prefetch()
        {
            dic.Clear();
            masterDic.Clear();
            PXDataField[] dataFeild = {
                                          new PXDataField<CSAttribute.attributeID>(),
                                          new PXDataField<CSAttribute.controlType>(),
                                          new PXDataField<CSAttribute.description>()

                                      };
            foreach (PXDataRecord rec in PXDatabase.SelectMulti<CSAttribute>
                (dataFeild))
            {
                
                String type = rec.GetInt32(1).ToString();

                // Text = 1; Combo = 2; CheckBox = 4; Datetime = 5; MultiSelectCombo = 6;
                if ("2".Equals(type)) {
                    masterDic[rec.GetString(0)] = rec.GetString(2);
                }
                
            }

            foreach (PXDataRecord rec in PXDatabase.SelectMulti<CSAttributeDetail>
                (new PXDataField<CSAttributeDetail.attributeID>(),
                new PXDataField<CSAttributeDetail.valueID>(),
                new PXDataField<CSAttributeDetail.description>()

            ))
            {
                Dictionary<String, String> detailDic = new Dictionary<String, String>();
                String attributeID = rec.GetString(0);
                if (masterDic.ContainsKey(attributeID))
                {

                        //contian GVLookupCodeType.lookupCodeTypeID
                        if (dic.ContainsKey(attributeID))
                        {
                            detailDic = dic[attributeID];
                            //detailDic<GVLookupCodeValue.lookupCode,GVLookupCodeValue.lookupCodeValue>
                            detailDic[rec.GetString(1)] = rec.GetString(2);
                            dic[attributeID] = detailDic;
                        }
                        else
                        {
                            detailDic[rec.GetString(1)] = rec.GetString(2);
                            dic[attributeID] = detailDic;
                        }
                    
                }
            }
        }

    }

    public class KGLookUpLovAttribute : PXStringListAttribute, IPXRowSelectedSubscriber
    {
        //public  String type{ get; set; }
        public Type _Module;
        public String typeName;
        public KGLookUpLovAttribute(Type module, String typeName)
            : base()
        {
            //this._AllowedLabels = GVLookUpCodeUtil.getValue(typeName);
            //this._AllowedValues = GVLookUpCodeUtil.getKey(typeName);


            _Module = module;
            this.typeName = typeName;
            initStringList();

        }
        private void initStringList()
        {
            KGLookUpCodeDefinition def = PXDatabase.GetSlot<KGLookUpCodeDefinition>(
                    typeof(KGLookUpCodeDefinition).Name, typeof(CSAttribute), typeof(CSAttributeDetail));
            if (def.dic.ContainsKey(typeName))
            {
                Dictionary<String, String> list = def.dic[typeName];
                this._AllowedLabels = list.Values.ToArray();
                this._AllowedValues = list.Keys.ToArray();
            }
        }


        public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            Object row = e.Row as Object;
            String module = (String)sender.GetValue(row, _Module.Name);

            if (row != null  /* && module != null*/)
            {

                KGLookUpCodeDefinition def = PXDatabase.GetSlot<KGLookUpCodeDefinition>(
                    typeof(KGLookUpCodeDefinition).Name, typeof(CSAttribute), typeof(CSAttributeDetail));
                if (def.dic.ContainsKey(typeName))
                {
                    Dictionary<String, String> list = def.dic[typeName];
                    PXStringListAttribute.SetList(sender, row, _FieldName, list.Keys.ToArray(), list.Values.ToArray());
                }
            }
        }
    }
}
