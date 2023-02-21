using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kedge.DAC
{
    public interface IKG505002
    {
        string BatchID { get; set; }
        int? BatchSeq { get; set; }
        int? BranchID { get; set; }
        string FinPeriodID { get; set; }
        string ContractCD { get; set; }
        string ContractDesc { get; set; }
        string VendorName { get; set; }
        Decimal? CumIncomeLast { get; set; }
        Decimal? CumIncomeThis { get; set; }
        Decimal? RecIncomeThis { get; set; }
        Decimal? CumCostLast { get; set; }
        Decimal? CumCostThis { get; set; }
        Decimal? RecCostThis { get; set; }
        Decimal? CumProfitLast { get; set; }
        Decimal? CumProfitThis { get; set; }
        Decimal? RecProfitThis { get; set; }
        Decimal? IncomeAmt { get; set; }
        Decimal? ProjectProgrees { get; set; }
        Decimal? ProfitRate { get; set; }
        Decimal? ExpenseAmt { get; set; }
        Decimal? Profit { get; set; }
        string LabelThis { get; set; }
        string LabelLast { get; set; }
    }
}
