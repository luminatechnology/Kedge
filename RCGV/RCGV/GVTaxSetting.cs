using System;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.TX;

namespace RCGV.GV
{
    public class GVTaxSetting : PXGraph<GVTaxSetting, GVTax>
    {
        DateTime theEndDate = new DateTime(9999, 06, 06);


        #region Selects
        public PXSelect<GVTax> GVTaxHeaders;

        public PXSelect<GVTaxRev,
                Where<GVTaxRev.gvtaxid,
                    Equal<Current<GVTax.gvtaxid>>>> GVTaxLines;


        #endregion


        #region Header Column Event

        protected virtual void GVTax_Taxid_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e) 
        {
            GVTax row = (GVTax)e.Row;


            PXResultset<TaxRev> TaxRevSet = GetTaxRev(row.Taxid);

            foreach (TaxRev S in TaxRevSet)
            {
                GVTaxRev detail = new GVTaxRev();
               
                detail.Gvtaxid = row.Gvtaxid;
                detail.RevisionID = S.RevisionID;
                detail.StartDate = S.StartDate;
                detail.EndDate = S.EndDate;

                detail.GvType = S.TaxType;
               

                detail.TaxRate = S.TaxRate;

                GVTaxLines.Cache.Insert(detail);
            }

        }

        #endregion

        #region Line Row Event
        protected virtual void GVTaxRev_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            GVTaxRev row = (GVTaxRev)e.Row;
            
            if (row.EndDate == null)
            {
                row.EndDate = theEndDate;
                GVTaxLines.Update(row);
            }

        }

       protected virtual void GVTaxRev_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            GVTaxRev row = (GVTaxRev)e.Row;

            if (row.GvTaxLineID > 0)
            {   //已存檔的資料，不可修改這二欄
                PXUIFieldAttribute.SetReadOnly<GVTaxRev.startDate>(GVTaxLines.Cache, GVTaxLines.Current, true);
                PXUIFieldAttribute.SetReadOnly<GVTaxRev.gvType>(GVTaxLines.Cache, GVTaxLines.Current, true);
            }


        }




        #endregion


        #region Save Method
        public override void Persist()
        {
            GVTaxLines.OrderByNew<OrderBy<Asc<GVTaxRev.startDate, Asc<GVTaxRev.gvType>>>>();
            GVTaxRev preData = null;


            foreach (GVTaxRev data in GVTaxLines.Select()) 
            {
                

                if (preData != null && preData.GvType == data.GvType && preData.StartDate < data.StartDate && preData.EndDate > data.StartDate)
                {
                   
                    preData.EndDate = data.StartDate.Value.AddDays(-1);
                    GVTaxLines.Update(preData);
                }
                else 
                {
                    if (data.EndDate != theEndDate) 
                    {
                        data.EndDate = theEndDate;
                        GVTaxLines.Update(data);

                    }

                }

                preData = data;

            }


            base.Persist();

        }

        #endregion

        # region BQL Method
        private PXResultset<TaxRev> GetTaxRev(String taxID)
        {
            return PXSelect<TaxRev,
                    Where<TaxRev.taxID,
                        Equal<Required<TaxRev.taxID>>>>.Select(this, taxID);
        }

        #endregion

    }
}