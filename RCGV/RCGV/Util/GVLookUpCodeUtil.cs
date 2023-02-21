using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using RCGV.GV.DAC;

namespace RCGV.GV.Util
{
	public	class GVLookUpCodeUtil
	{

		public const String DeclarePeriod = "DeclarePeriod";

        public class DeclarePeriodType : Constant<string>
        {
            public DeclarePeriodType() : base(DeclarePeriod) { }
        }

        public const String GvType = "GvType";
        public const String InvoiceType = "InvoiceType";
        public const String InvoiceType_I = "I";
        
        public const String EgvType = "EgvType";
        public const String EgvType_B2C = "1";
        public const String Status = "Status";
		public const String TaxCode = "TaxCode";
		public const String DeclareMethod = "DeclareMethod";
		public const String DeclarePayCode = "DeclarePayCode";
        public const String InGuiSubType = "InGuiSubType";
        public const String OutGuiSubType = "OutGuiSubType";
        public const String InOrderType = "InOrderType";
        public const String OutOrderType = "OutOrderType";
        public const String TaxCityCode = "TaxCityCode";
	    public const String VoucharCategoryType = "VoucherCategory";
        public const String DeductionCodeType = "DeductionCode";
        public const String GroupRemarkType = "GroupRemark";
        public const String SpecialTaxType = "SpecialTaxType";
        public const String ZeroDocType = "ZeroDocType";
        public const String ZeroDataType = "ZeroDataType";
        public const String ZeroSalesType = "ZeroSalesType";

        public static Dictionary<String, Object> getSelection(String selectionName)
		{
			
			PXGraph graph  =new PXGraph();
			PXResultset<GVLookupCodeValue> set = PXSelectJoin<GVLookupCodeValue,
						InnerJoin<GVLookupCodeType,
							On<GVLookupCodeType.lookupCodeTypeID,
							Equal<GVLookupCodeValue.lookupCodeTypeID>>>,
						Where<GVLookupCodeType.lookupCodeType,
							Equal<Required<GVLookupCodeType.lookupCodeType>>,
							And<GVLookupCodeValue.IsActive,Equal<True>>>>
							.Select(graph, selectionName, graph.Accessinfo.BusinessDate, graph.Accessinfo.BusinessDate);
			
			Dictionary<String, Object> map = new Dictionary<String, Object>();
			List<string> keys = new List<string>();
			List<string> values = new List<string>();
			foreach (GVLookupCodeValue line in set)
			{
				keys.Add(line.LookupCode);
				values.Add(line.LookupCodeValue);
			}
			map.Add("key", keys);
			map.Add("value", values);
			return map;
		}
		public static String[] getKeyByDic(Dictionary<String, Object> dic)
		{
			return ((List<String>)dic["key"]).ToArray();
		}
		public static String[] getValueByDic(Dictionary<String, Object> dic)
		{
			return ((List<String>)dic["value"]).ToArray();
		}
		public static String[] getKey(String type)
		{
			String[] key = GVLookUpCodeUtil.getKeyByDic(GVLookUpCodeUtil.getSelection(type));
			return key;
		}
		public static String[] getValue(String type)
		{
			String[] value = GVLookUpCodeUtil.getValueByDic(GVLookUpCodeUtil.getSelection(type));
			return value;
		}
	}
	/*
	public class GVLookUpCodeAttribute : PXStringListAttribute
	{
		//public  String type{ get; set; }
		public GVLookUpCodeAttribute(String type):base(GVLookUpCodeUtil.getKey(type), GVLookUpCodeUtil.getValue(type))
			
		{
			
		}
	}*/
	public class GVLookUpCodeDefinition : IPrefetchable
	{
		public Dictionary<String, Dictionary<String, String>> dic = new Dictionary<String, Dictionary<String, String>>();
		public Dictionary<String, String> masterDic = new Dictionary<String, String>();
		public void Prefetch()
		{
            /*
			HashSet<GVLookupCodeType> set = 
				(HashSet<GVLookupCodeType>)PXDatabase.SelectMulti<GVLookupCodeType>(dataFeild);*/
			
			dic.Clear();
			masterDic.Clear();
            PXDataField[] dataFeild = {
                                          new PXDataField<GVLookupCodeType.lookupCodeTypeID>(),
                                          new PXDataField<GVLookupCodeType.lookupCodeType>()

                                      };
            foreach (PXDataRecord rec in PXDatabase.SelectMulti<GVLookupCodeType>
				(dataFeild))
			{
				masterDic[rec.GetInt32(0).ToString()] = rec.GetString(1);
			}

			foreach (PXDataRecord rec in PXDatabase.SelectMulti<GVLookupCodeValue>
				(new PXDataField<GVLookupCodeValue.lookupCode>(),
				new PXDataField<GVLookupCodeValue.lookupCodeValue>(),
				new PXDataField<GVLookupCodeValue.lookupCodeTypeID>(),
				new PXDataField<GVLookupCodeValue.IsActive>()
			
			)){
				Boolean? isActive = rec.GetBoolean(3);
				Dictionary<String, String> detailDic = new Dictionary<String, String>();
                String lookupCodeTypeID = rec.GetInt32(2).ToString();
                if (masterDic.ContainsKey(lookupCodeTypeID)) {
                    String lookupCodeType = masterDic[lookupCodeTypeID];
                    if ((isActive != null && isActive==true))
                    {
                        //contian GVLookupCodeType.lookupCodeTypeID
                        if (dic.ContainsKey(lookupCodeType))
                        {
                            detailDic = dic[lookupCodeType];
                            //detailDic<GVLookupCodeValue.lookupCode,GVLookupCodeValue.lookupCodeValue>
                            detailDic[rec.GetString(0)] = rec.GetString(1);
                            dic[lookupCodeType] = detailDic;
                        }
                        else
                        {
                            detailDic[rec.GetString(0)] = rec.GetString(1);
                            dic[lookupCodeType] = detailDic;
                        }
                    }
                }
			}
		}
	}
	public class GVLookUpCodeAttribute : PXStringListAttribute
	{
		//public  String type{ get; set; }
		public GVLookUpCodeAttribute(String type)
			: base()
		{
			this._AllowedLabels = GVLookUpCodeUtil.getValue(type);
			this._AllowedValues = GVLookUpCodeUtil.getKey(type);
		}
	}

	public class GVLookUpCodeExtAttribute : PXStringListAttribute, IPXRowSelectedSubscriber
	{
		//public  String type{ get; set; }
		public Type _Module;
		public String typeName;
		public GVLookUpCodeExtAttribute(Type module, String typeName)
			: base()
		{
			this._AllowedLabels = GVLookUpCodeUtil.getValue(typeName);
			this._AllowedValues = GVLookUpCodeUtil.getKey(typeName);
			_Module = module;
            this.typeName = typeName;

        }
		public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			Object row = e.Row as Object;
			String module = (String)sender.GetValue(row, _Module.Name);

			if (row != null  /* && module != null*/)
			{

				GVLookUpCodeDefinition def = PXDatabase.GetSlot<GVLookUpCodeDefinition>(
					typeof(GVLookUpCodeDefinition).Name, typeof(GVLookupCodeType), typeof(GVLookupCodeValue));
                if (def.dic.ContainsKey(typeName)) {
                    Dictionary<String, String> list = def.dic[typeName];
                    PXStringListAttribute.SetList(sender, row, _FieldName, list.Keys.ToArray(), list.Values.ToArray());
                }
			}
		}
	}
	/*
	#region XTLLPaymenDocTypeAttribute
	public class XTLLPaymenDocTypeAttribute : PXStringListAttribute, IPXRowSelectedSubscriber
	{
		public Type _Module;
		public class OrderTypesDefinition : IPrefetchable
		{
			public Dictionary<String, String> list = new Dictionary<String, String>();

			public void Prefetch()
			{
				list.Clear();
				
				foreach (PXDataRecord rec in PXDatabase.SelectMulti<SOOrderType>(new PXDataField<SOOrderType.orderType>(), new PXDataField<SOOrderType.descr>()))
				{
					list[rec.GetString(0)] = rec.GetString(1);
				}
			}
		}

		public XTLLPaymenDocTypeAttribute(Type module)
			: base()
		{
			_Module = module;
		}

		public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			Object row = e.Row as Object;
			String module = (String)sender.GetValue(row, _Module.Name);

			if (row != null && module != null)
			{
				if (module == arModule.value)
				{
					PXStringListAttribute.SetList(sender, row, _FieldName, new ARDocType.ListAttribute());
				}
				if (module == soModule.value)
				{
					OrderTypesDefinition def = PXDatabase.GetSlot<OrderTypesDefinition>(typeof(OrderTypesDefinition).Name, typeof(SOOrderType));
					PXStringListAttribute.SetList(sender, row, _FieldName, def.list.Keys.ToArray(), def.list.Values.ToArray());
				}
			}
		}
	}
	#endregion*/
}
