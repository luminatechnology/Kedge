using System;
using PX.Data;
using System.Collections.Generic;
using RCGV.GV.DAC;
using RCGV.GV.SYS;
using System.Threading;
using System.Threading.Tasks;

namespace RCGV.GV
{
	public class GVLookupCodeMaint : GVBaseGraph<GVLookupCodeMaint, GVLookupCodeType>
  {
	public PXSelect<GVLookupCodeType> CodeMaster;
	[PXImport(typeof(GVLookupCodeValue))]
	public PXSelect<GVLookupCodeValue,
						Where<GVLookupCodeValue.lookupCodeTypeID,
						Equal<Optional<GVLookupCodeType.lookupCodeTypeID>>>> CodeDetails;
	
	public PXSelectJoin<GVLookupCodeValue,
						InnerJoin<GVLookupCodeType, 
							On<GVLookupCodeType.lookupCodeTypeID, 
							Equal<GVLookupCodeValue.lookupCodeTypeID>>>,
						Where<GVLookupCodeType.lookupCodeType,
							Equal<Required<GVLookupCodeType.lookupCodeType>>,And<GVLookupCodeValue.IsActive, Equal<True>>>> searchDetails;

	
	protected virtual void GVLookupCodeValue_LookupCode_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
	{
		GVLookupCodeValue row = (GVLookupCodeValue)e.Row;
		if (row == null ||  e.NewValue == null )
		{
			return;
		}
		setCheckThread(checkLookupCode_D(sender, row, (String)e.NewValue));
	}

	public bool checkLookupCode_D(PXCache sender, GVLookupCodeValue row, String newValue)
	{
		if (newValue == null)
		{
			return	true;
		}
		foreach (GVLookupCodeValue line in CodeDetails.Select())
		{
			if (line.LookupCode == null)
			{
				return true;
			}
			if (line.LookupCode.Equals(newValue))
			{
                    if (row.LookupCodeValueID.Equals(line.LookupCodeValueID) && row.LookupCodeValueID !=null)
                    {
                        continue;
                    }
                    sender.RaiseExceptionHandling<GVLookupCodeValue.lookupCode>(
									row, newValue,
											new PXSetPropertyException("LookupCode is duplecate.", PXErrorLevel.Error));
				return true;
			}
		}
		return false;
	}


	


	public static Dictionary<String, Object> getSelection(String selectionName) {
		GVLookupCodeMaint graph = PXGraph.CreateInstance<GVLookupCodeMaint>();
		
		Dictionary<String, Object> map = new Dictionary<String, Object>();
		List<string> keys = new List<string>();
		List<string> values = new List<string>();
		foreach (GVLookupCodeValue line in graph.searchDetails.Select(selectionName, graph.Accessinfo.BusinessDate, graph.Accessinfo.BusinessDate))
		{
			keys.Add(line.LookupCode);
			values.Add(line.LookupCodeValue);
		}
		map.Add("key",keys);
		map.Add("value", values);
		return map;
	}
	public static String[] getKeyByDic(Dictionary<String, Object> dic) { 
		return  ((List<String>)dic["key"]).ToArray();
	}
	public static String[] getValueByDic(Dictionary<String, Object> dic)
	{
		return ((List<String>)dic["value"]).ToArray();
	}
    
	protected virtual void GVLookupCodeType_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
	{
            e.Cancel = getCheckThread();
	}
	protected virtual void GVLookupCodeValue_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
	{
            GVLookupCodeValue row = (GVLookupCodeValue)e.Row;
            /*
            setCheckThread(checkLookupCode_D(sender, row, row.LookupCode));
            setCheckThread(checkEffectiveDate_D(sender, row, row.EffectiveDate));
            setCheckThread(checkExpirationDate_D(sender, row, row.ExpirationDate));*/
            e.Cancel = getCheckThread();
	}
    public override void Persist()
    {

        foreach (GVLookupCodeValue row in searchDetails.Cache.Inserted) {
                setCheckThread(checkLookupCode_D(searchDetails.Cache, row, row.LookupCode));
        
        }
        foreach (GVLookupCodeValue row in searchDetails.Cache.Updated)
        {
            setCheckThread(checkLookupCode_D(searchDetails.Cache, row, row.LookupCode));
        }
        base.Persist();
        if (getCheckThread())
        {
            throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
        }
    }

    }
}