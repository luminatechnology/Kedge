using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCGV.GV.DAC;
using PX.Data;

namespace RCGV.GV.Util
{

	#region VoucherCategoryType
	public static class VoucherCategoryAttribute
	{
		public const string CustomProxyTaxName = "海關代徵營業稅繳納證";
		public const string TaxableName = "發票(含稅)";
		public const string OtherCertificateName = "其他憑證";
		public const string ZeroTaxName = "零稅";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { CustomProxyTax, Taxable, OtherCertificate, ZeroTax },
					new string[] { CustomProxyTaxName, TaxableName, OtherCertificateName, ZeroTaxName }
					) { }
		}

		public class ListSimpleAttribute : PXStringListAttribute
		{
			public ListSimpleAttribute()
				: base(
				new string[] { CustomProxyTax, Taxable, OtherCertificate, ZeroTax },
				new string[] { CustomProxyTaxName, TaxableName, OtherCertificateName, ZeroTaxName }) { }
		}

		public const string CustomProxyTax = "C";
		public const string Taxable = "I";
		public const string OtherCertificate = "R";
		public const string ZeroTax = "Q";

		public class customProxyTax : Constant<string>
		{
			public customProxyTax() : base(CustomProxyTax) { }
		}

		public class zeroTax : Constant<string>
		{
			public zeroTax() : base(ZeroTax) { }
		}

		public class taxable : Constant<string>
		{
			public taxable() : base(Taxable) { }
		}

		public class otherCertificate : Constant<string>
		{
			public otherCertificate() : base(OtherCertificate) { }
		}
	}
	#endregion

	#region GVTaxCode
	public static class GVTaxCode
	{

		public const string TaxableName = "應稅";
		public const string ZeroTaxName = "零稅";
		public const string DutyFreeName = "免稅";
		public const string VoidInvoiceName = "作癈";
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { Taxable, ZeroTax, DutyFree, VoidInvoice },
					new string[] { TaxableName, ZeroTaxName, DutyFreeName, VoidInvoiceName }
					) { }
		}

		public class ListSimpleAttribute : PXStringListAttribute
		{
			public ListSimpleAttribute()
				: base(
				new string[] { Taxable, ZeroTax, DutyFree, VoidInvoice },
				new string[] { TaxableName, ZeroTaxName, DutyFreeName, VoidInvoiceName }) { }
		}

		public const string Taxable = "1";
		public const string ZeroTax = "2";
		public const string DutyFree = "3";
		public const string VoidInvoice = "4";

		public class dutyFree : Constant<string>
		{
			public dutyFree() : base(DutyFree) { }
		}

		public class zeroTax : Constant<string>
		{
			public zeroTax() : base(ZeroTax) { }
		}

		public class taxable : Constant<string>
		{
			public taxable() : base(Taxable) { }
		}

		public class voidInvoice : Constant<string>
		{
			public voidInvoice() : base(VoidInvoice) { }
		}
	}
	#endregion

	#region DeductionCodeAttribute
	#endregion

	#region GVTaxCode
	public static class APInvoiceStatus
	{

		public const string OpenName = "Open";
		public const string VoidName = "Void";
		public const string GenerateXmlName = "Generate XML";
		public const string TrunkeySendOKName = "Trunkey send OK";
		public const string TrunkeySendErrorName = "Trunkey send error";
		public const string TrunkeyReturnOKName = "Trunkey return OK";
		public const string TrunkeyReturnErrorName = "Trunkey return error";
        //2021-02-20 : 11955
        public const string HoldName = "Hold";
        public const string ReleaseName = "Release";

        public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { 
						Open, VoidInvoice, GenerateXml, 
						TrunkeySendOK, TrunkeySendError, 
						TrunkeyReturnOK, TrunkeyReturnError ,
                        Hold, ReleaseName
					},
					new string[] { 
						OpenName, VoidName, GenerateXmlName, 
						TrunkeySendOKName, TrunkeySendErrorName, 
						TrunkeyReturnOKName, TrunkeyReturnErrorName ,
                        HoldName, ReleaseName
                    }
					) { }
		}

		public class ListSimpleAttribute : PXStringListAttribute
		{
			public ListSimpleAttribute()
				: base(
					new string[] { 
						Open, VoidInvoice, GenerateXml, 
						TrunkeySendOK, TrunkeySendError, 
						TrunkeyReturnOK, TrunkeyReturnError 
					},
					new string[] { 
						OpenName, VoidName, GenerateXmlName, 
						TrunkeySendOKName, TrunkeySendErrorName, 
						TrunkeyReturnOKName, TrunkeyReturnErrorName 
					}
					) { }
		}

		public const string Open = "1";
		public const string VoidInvoice = "2";
		public const string GenerateXml = "3";
		public const string TrunkeySendOK = "4";
		public const string TrunkeySendError = "5";
		public const string TrunkeyReturnOK = "6";
		public const string TrunkeyReturnError = "7";
        public const string Hold = "8";
        public const string Release = "9";

        public class open : Constant<string>
		{
			public open() : base(Open) { }
		}

		public class voidInvoice : Constant<string>
		{
			public voidInvoice() : base(VoidInvoice) { }
		}

		public class generateXml : Constant<string>
		{
			public generateXml() : base(GenerateXml) { }
		}

		public class trunkeySendOK : Constant<string>
		{
			public trunkeySendOK() : base(TrunkeySendOK) { }
		}

		public class trunkeySendError : Constant<string>
		{
			public trunkeySendError() : base(TrunkeySendError) { }
		}

		public class trunkeyReturnOK : Constant<string>
		{
			public trunkeyReturnOK() : base(TrunkeyReturnOK) { }
		}

		public class trunkeyReturnError : Constant<string>
		{
			public trunkeyReturnError() : base(TrunkeyReturnError) { }
		}

        public class hold : PX.Data.BQL.BqlString.Constant<hold> { public hold() : base(Hold) {; } }
        public class release : PX.Data.BQL.BqlString.Constant<release> { public release() : base(Release) {; } }

    }
	#endregion

	#region GroupRemark
	public static class GroupRemarkAttribute
	{
        public const string UnsummaryName = "非彙加資料";
        public const string SummaryName = "彙加資料";
		public const string ApportionmentName = "分攤資料";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { UnsummaryName, Summary, Apportionment },
					new string[] { UnsummaryName, SummaryName, ApportionmentName }
					) { }
		}

		public class ListSimpleAttribute : PXStringListAttribute
		{
			public ListSimpleAttribute()
				: base(
					new string[] { UnsummaryName, Summary, Apportionment },
					new string[] { UnsummaryName, SummaryName, ApportionmentName }
				) { }
		}

        public const string Unsummary = "0";
        public const string Summary = "A";
		public const string Apportionment = "B";

		public class summary : Constant<string>
		{
			public summary() : base(Summary) { }
		}

		public class apportionment : Constant<string>
		{
			public apportionment() : base(Apportionment) { }
		}
        public class unsummary : Constant<string>
        {
            public unsummary() : base(Unsummary) { }
        }

    }

    #endregion

    #region GVTaxType
    public static class GVType
	{
		public const string InputName = "進項";
		public const string OutputName = "銷項";
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { Input, Output, },
					new string[] { InputName, OutputName }) { }
		}

		public class ListSimpleAttribute : PXStringListAttribute
		{
			public ListSimpleAttribute()
				: base(
				new string[] { Input, Output, },
				new string[] { InputName, OutputName }) { }
		}

		public const string Input = "I";
		public const string Output = "O";

		public class inputType : Constant<string>
		{
			public inputType() : base(Input) { }
		}

		public class outputType : Constant<string>
		{
			public outputType() : base(Output) { }
		}
	}
	#endregion

	#region GVGuiType
	public class GVGuiTypeDropdownAttribute : PXStringListAttribute
	{
		private String _type;

		public GVGuiTypeDropdownAttribute(String type)
			: base()
		{
			this._type = type;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			GVGuiTypeMaint graph = PXGraph.CreateInstance<GVGuiTypeMaint>();
			PXSelectBase<GVGuiType> query = new PXSelectReadonly<GVGuiType,
				Where<GVGuiType.gvType, Equal<Required<GVGuiType.gvType>>>>(graph);
			List<string> keys = new List<string>();
			List<string> values = new List<string>();
			foreach (PXResult<GVGuiType> record in query.Select(_type))
			{
				GVGuiType guiType = (GVGuiType) record;
				keys.Add(string.Format("{0}-{1}", guiType.GuiTypeCD, guiType.GuiTypeDesc));
				values.Add(guiType.GuiTypeCD);
			}
			this._AllowedLabels = keys.ToArray();
			this._AllowedValues = values.ToArray();
		}

		private static void FetchGuiTypes(String kind, String type)
		{
			GVGuiTypeMaint graph = PXGraph.CreateInstance<GVGuiTypeMaint>();
			PXSelectBase<GVGuiType> query = new PXSelectReadonly<GVGuiType,
				Where<GVGuiType.gvType, Equal<Required<GVGuiType.gvType>>>>(graph);

			List<string> keys = new List<string>();
			List<string> values = new List<string>();
			foreach (PXResult<GVGuiType> record in query.Select(type))
			{
				GVGuiType guiType = (GVGuiType) record;
				keys.Add(guiType.GuiTypeDesc);
				values.Add(guiType.GuiTypeCD);
			}
		}
	}
	#endregion
}
