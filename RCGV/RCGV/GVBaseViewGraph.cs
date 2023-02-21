using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using System.Threading;
using System.Threading.Tasks;


namespace RCGV.GV.SYS
{
	public class GVBaseViewGraph<TGraph> :PXGraph  where TGraph : PX.Data.PXGraph
	{
		ThreadLocal<Boolean> checkThread = new ThreadLocal<Boolean>();
		//return e.cancel
		public void setCheckThread(Boolean ckeck)
		{
			if (checkThread.Value != null)
			{
				checkThread.Value = checkThread.Value || ckeck;
			}
			else
			{
				checkThread.Value = ckeck;
			}
		}
		public Boolean getCheckThread()
		{
			if (checkThread.Value != null)
			{
				return checkThread.Value;
			}
			else
			{
				return false;
			}
		}
	}
}