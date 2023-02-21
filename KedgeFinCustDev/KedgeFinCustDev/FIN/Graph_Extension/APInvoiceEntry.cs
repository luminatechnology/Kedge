using PX.Data;
using PX.Data.BQL.Fluent;
using Fin.DAC;
using RC.Util;
using NM.Util;
using PX.Objects.CR;
using Kedge.DAC;
using NM.DAC;

namespace PX.Objects.AP
{
    //2021/03/10 Althea Take Out
    public class APInvoiceEntry_Extension2 : PXGraphExtension<APInvoiceEntry_Extension, APInvoiceEntryRetainage, APInvoiceEntry>
    {
        /*#region Selects
        public SelectFrom<TWNWHT>.Where<TWNWHT.docType.IsEqual<APInvoice.docType.FromCurrent>
                                        .And<TWNWHT.refNbr.IsEqual<APInvoice.refNbr.FromCurrent>>>.View WHTView;

        #endregion

        #region Global Variables
        private bool runable = false;
        private bool activWHT = false;
        #endregion

        #region Override Function
        public override void Initialize()
        {
            runable = RCFeaturesSetUtil.IsActive(Base);
            activWHT = RCFeaturesSetUtil.IsActiveOP(Base, RCFeaturesSetProperties.WITHHOLDING_TAXES);
        }

        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<APInvoice> e, PXRowSelected invokeBaseHandler)
        {
            invokeBaseHandler?.Invoke(e.Cache, e.Args);

            WHTView.Cache.AllowSelect = runable && activWHT;
        }

        protected void _(Events.FieldUpdated<APInvoice.vendorID> e, PXFieldUpdated invokeBaseHandler)
        {
            invokeBaseHandler?.Invoke(e.Cache, e.Args);

            if (runable && activWHT &&
                Base.Document.Current != null && Base.Document.Current.DocType.Equals(APDocType.Invoice))
            {
                TWNWHT wNWHT = new TWNWHT()
                {
                    DocType = Base.Document.Current.DocType
                };

                WHTView.Cache.SetDefaultExt<TWNWHT.personalID>(wNWHT);
                WHTView.Cache.SetDefaultExt<TWNWHT.propertyID>(wNWHT);
                WHTView.Cache.SetDefaultExt<TWNWHT.typeOfIn>(wNWHT);
                WHTView.Cache.SetDefaultExt<TWNWHT.wHTFmtCode>(wNWHT);
                WHTView.Cache.SetDefaultExt<TWNWHT.wHTFmtSub>(wNWHT);
                WHTView.Cache.SetDefaultExt<TWNWHT.wHTTaxPct>(wNWHT);
                WHTView.Cache.SetDefaultExt<TWNWHT.gNHI2Code>(wNWHT);

                WHTView.Cache.Insert(wNWHT);
            }
        }
        #endregion*/
    }
}