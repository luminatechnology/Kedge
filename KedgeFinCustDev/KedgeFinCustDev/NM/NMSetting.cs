using System;
using PX.Data;
using NM.DAC;

namespace NM
{
  public class NMSetting : PXGraph<NMSetting>
  {

    public PXSave<NMPreference> Save;
    public PXCancel<NMPreference> Cancel;


    public PXSelect<NMPreference> SetUps;

  }
}