using Kedge.DAC;
using PX.Data;
using System;

namespace Kedge.Util
{
    public class CalcPostageUtil
    {

        public static decimal GetPostageAmt(PXGraph graph, int? paymentDay, decimal? paymentAmount)
        {
            KGPostageSetup setup = PXSelect<KGPostageSetup,
                Where<KGPostageSetup.longTerm, IsNotNull,
                And<KGPostageSetup.firstPostCost, IsNotNull,
                And<KGPostageSetup.costIntervalEnd, IsNotNull,
                And<KGPostageSetup.costIntervalEnd, NotEqual<Zero>,
                And<KGPostageSetup.costInterval, IsNotNull,
                And<KGPostageSetup.costIntervalPostCost, IsNotNull>>>>>>>.Select(graph);
            if (setup == null)
            {
                throw new PXException("請至[KG傳票摘要設定]維護[郵資費設定]");
            }
            decimal _totalPoatageAmt = 0m;

            if (paymentDay != 0)//遠期
            {
                //遠期郵資費
                _totalPoatageAmt = (int)setup.LongTerm;//40;
            }
            else//即期
            {
                //基本郵資費用
                _totalPoatageAmt = setup.FirstPostCost ?? 0m;
                //基本額度
                int baseCost = setup.CostIntervalEnd ?? 1;
                //超額費用
                int costIntervalPostCost = setup.CostIntervalPostCost ?? 0;
                //超額費率
                int costInterval = setup.CostInterval ?? 0;
                //餘額 = 金額 - 基礎額度
                decimal balance = (paymentAmount ?? 0m) - baseCost;
                //當餘額大於0 (超額)
                if (balance > 0m)
                {
                    int magn = Decimal.ToInt32(balance / costInterval + (balance % costInterval > 0 ? 1 : 0));
                    _totalPoatageAmt += (costIntervalPostCost * magn);
                }
            }
            return _totalPoatageAmt;
        }
    }
}
