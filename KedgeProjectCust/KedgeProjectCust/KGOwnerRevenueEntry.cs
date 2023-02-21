using System;
using System.Collections;
using Kedge.DAC;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.PM;

namespace Kedge
{
    public class KGOwnerRevenueEntry : PXGraph<KGOwnerRevenueEntry>
    {
        public PXSave<FilterTable> Save;
        public PXCancel<FilterTable> Cancel;
        public PXFilter<FilterTable> FilterView;
        [PXImport(typeof(KGOwnerRevenue))]
        public PXSelect<KGOwnerRevenue,
                        Where<KGOwnerRevenue.projectID, Equal<Current<FilterTable.projectID>>>,
                        OrderBy<Asc<KGOwnerRevenue.costCodeID>>> OwnerRevenue;

        #region Delegate Data View
        protected virtual IEnumerable filterView()
        {
            FilterTable filter = FilterView.Current;

            if (filter != null)
            {
                int startRow  = 0;
                int totalRows = 0;

                PXSelectBase<KGOwnerRevenue> cmd = new PXSelectGroupBy<KGOwnerRevenue,
                                                                       Where<KGOwnerRevenue.projectID, Equal<Current<FilterTable.projectID>>>,
                                                                       Aggregate<Sum<KGOwnerRevenue.amount>>>(this);

                foreach (KGOwnerRevenue row in cmd.View.Select(null,
                                                               null,
                                                               PXView.Searches,
                                                               OwnerRevenue.View.GetExternalSorts(),
                                                               OwnerRevenue.View.GetExternalDescendings(),
                                                               OwnerRevenue.View.GetExternalFilters(),
                                                               ref startRow,
                                                               PXView.MaximumRows,
                                                               ref totalRows))
                {
                    filter.Total = row.Amount;
                }
            }

            yield return filter;

            FilterView.Cache.IsDirty = false;
        }
        #endregion 
    }

    #region Filter Table
    [Serializable]
    public partial class FilterTable : IBqlTable
    {
        #region ProjectID
        [ActiveProjectOrContractBaseAttribute(FieldClass = ProjectAttribute.DimensionName)]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region Total
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Total Amount", Enabled = false)]
        public virtual Decimal? Total { get; set; }
        public abstract class total : PX.Data.BQL.BqlDecimal.Field<total> { }
        #endregion
    }
    #endregion
}