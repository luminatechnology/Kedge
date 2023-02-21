using PX.Data.EP;
using PX.Data;
using PX.Objects.AP.MigrationMode;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace PX.Objects.PO
{
    public class POOrderAPDocExt : PXCacheExtension<PX.Objects.PO.POOrderAPDoc>
    {
        #region UsrValuationPhase
        [PXInt()]
        [PXUIField(DisplayName = "UsrValuationPhase", Enabled = false)] 
        public virtual int UsrValuationPhase { get; set; }
        public abstract class usrValuationPhase : IBqlField { }
        #endregion
    }
}