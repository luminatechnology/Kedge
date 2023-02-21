using System;
using Fin.DAC;
using PX.Data;

namespace Fin
{
    //Screen ID : TW30.10.02
  public class TWNWHTMaint : PXGraph<TWNWHTMaint>
  {
        public PXSave<TWNWHTTran> Save;
        [PXImport]
        public PXSelect<TWNWHTTran> WHTTrans;

  }
}