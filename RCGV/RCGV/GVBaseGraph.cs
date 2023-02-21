using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using System.Threading;
using System.Threading.Tasks;


namespace RCGV.GV.SYS
{
	public		class GVBaseGraph<TGraph, TPrimary> : PXGraph<TGraph, TPrimary>
		where TGraph : PX.Data.PXGraph
		where TPrimary : class, PX.Data.IBqlTable, new()
	{

		public GVBaseGraph()
			: base()
		{
		}	
		
		ThreadLocal<Boolean> checkThread = new ThreadLocal<Boolean>();
		//return e.cancel
		public void setCheckThread(Boolean ckeck)
		{
		    checkThread.Value = checkThread.Value || ckeck;
		}
		public Boolean getCheckThread()
		{
            return checkThread.Value;
		}
        //value equals newValue
        public bool checkNullEX(PXCache sender,String column,Object row,Object value) {
            if (value == null) {
                sender.RaiseExceptionHandling(column, row, value, new PXSetPropertyException(column+" can not null"));
                setCheckThread(true);
                return false;
            }
            setCheckThread(false);
            return true;
        }
        public bool checkNullEX<T> (PXCache sender, String column, Object row, Object value) where T : PX.Data.IBqlField
        {
            if (value == null)
            {
                sender.RaiseExceptionHandling<T> (row, value, new PXSetPropertyException(column + " can not null"));
                setCheckThread(true);
                return false;
            }
            setCheckThread(false);
            return true;
        }
        public bool checkNull(params Object[]  parameter) {

            for (int i = 0; i < parameter.Length; i++)
            {
                if (parameter[i] == null) {

                    return false;
                }
            }
            return true;
        }
	}
}