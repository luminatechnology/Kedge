using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;

namespace RCGV
{
	public class GVSetupMaint : PXGraph<GVSetupMaint>
	{
		public PXSave<GVSetup> Save;
		public PXCancel<GVSetup> Cancel;
		public PXSelect<GVSetup> LastNumbers;
	}
}