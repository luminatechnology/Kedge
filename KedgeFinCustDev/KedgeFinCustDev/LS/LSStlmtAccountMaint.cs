using PX.Data;
using PX.Data.BQL.Fluent;
using KedgeFinCustDev.LS.DAC;

namespace KedgeFinCustDev.LS
{
    public class LSStlmtAccountMaint : PXGraph<LSStlmtAccountMaint>
    {
        public PXSavePerRow<LSSettlementAccount> Save;
        public PXCancel<LSSettlementAccount> Cancel;
  
        [PXFilterable()]
        [PXImport(typeof(LSSettlementAccount))]
        public SelectFrom<LSSettlementAccount>.View StlmtAccount;
    }
}