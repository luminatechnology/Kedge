using System;
using PX.Data;
using RCGV.GV.DAC;
using RCGV.GV.SYS;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;


namespace RCGV.GV
{
	public class GVGuiTypeMaint : GVBaseViewGraph<GVGuiTypeMaint>

    {
        [PXImport(typeof(GVGuiType))]
        public PXSelect<GVGuiType> CodeMaster;
		public PXSave<GVGuiType> Save;
		public PXCancel<GVGuiType> Cancel;


        #region Event Handlers
        protected virtual void GVGuiType_GuiTypeCD_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GVGuiType row = (GVGuiType)e.Row;
			// null not check 
			if (row == null )
			{
				return;
			}
			setCheckThread(checkGuiTypeCD_M(sender, row, (String)e.NewValue));
		}
		public bool checkGuiTypeCD_M(PXCache sender, GVGuiType row, String newValue)
		{
			// no modify
			if (newValue == null )
			{
				return	true;
			}
			foreach (GVGuiType line in CodeMaster.Select())
			{
				if (line.GuiTypeCD == null)
				{
					return true;
				}

				if (line.GuiTypeCD.Equals(newValue))
				{
					if (row.GuiTypeID != null)
					{
						if (row.GuiTypeID.Equals(line.GuiTypeID))
						{
							continue;
						}
					}
					else
					{
						GVGuiType tempLine = CodeMaster.Current;
						if (tempLine != null &&  tempLine.GuiTypeID.Equals(line.GuiTypeID))
						{
							continue;
						}
					}

					sender.RaiseExceptionHandling<GVGuiType.guiTypeCD>(
										row, newValue,
												new PXSetPropertyException("GuiTypeCD is duplecate.", PXErrorLevel.Error));

					return true;
				}
			}
			return false;
		}
		
		
        #endregion
        protected virtual void GVGuiType_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
            GVGuiType row = (GVGuiType)e.Row;
            setCheckThread(checkGuiTypeCD_M(sender, row, row.GuiTypeCD));            
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
        public static Dictionary<String, Object> getGuiType(String type)
		{

			PXGraph graph = new PXGraph();
			PXResultset<GVGuiType> set = PXSelect<GVGuiType,

						Where<GVGuiType.gvType,
							Equal<Required<GVGuiType.gvType>>

							>>.Select(graph, type);

			Dictionary<String, Object> map = new Dictionary<String, Object>();
			List<string> keys = new List<string>();
			List<string> values = new List<string>();
			foreach (GVGuiType line in set)
			{
				keys.Add(line.GuiTypeCD);
                //values.Add(line.GuiTypeCD+"-"+line.GuiTypeDesc);
                values.Add(line.GuiTypeDesc);
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
	}


   
}