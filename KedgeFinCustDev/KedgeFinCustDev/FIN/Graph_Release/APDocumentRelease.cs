using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using Fin.DAC;
using PX.Objects.CR;
using RC.Util;

namespace PX.Objects.AP
{
    public class APReleaseProcess_Extension2 : PXGraphExtension<APReleaseProcess_Extension, APReleaseProcess>
    {
        #region Selects & Setup
        public SelectFrom<TWNWHTTran>.Where<TWNWHTTran.docType.IsEqual<APRegister.docType.FromCurrent>
                                            .And<TWNWHTTran.refNbr.IsEqual<APRegister.refNbr.FromCurrent>>>.View WHTTranView;
        #endregion

        #region Global Variables
        private bool runable = false;
        private bool activWHT = false;
        #endregion

        #region Override Function
        public override void Initialize()
        {
            runable  = RCFeaturesSetUtil.IsActive(Base);
            activWHT = RCFeaturesSetUtil.IsActiveOP(Base, RCFeaturesSetProperties.WITHHOLDING_TAXES);
        }
        #endregion

        #region Delegate Funcation
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            baseMethod();

            if (runable && activWHT)
            {
                APRegister register = Base.APDocument.Current;
                TWNWHT tWNWHT = SelectFrom<TWNWHT>.Where<TWNWHT.aPDocType.IsEqual<@P.AsString>
                                                                .And<TWNWHT.aPRefNbr.IsEqual<@P.AsString>>>.View.Select(Base, register.DocType, register.RefNbr);

                if (register != null &&
                    register.Released.Equals(true) &&
                    register.DocType.Equals(APDocType.Invoice) &&
                    tWNWHT != null)
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        TWNWHTTran wHTTran = new TWNWHTTran();

                        foreach (APTran row in Base.APTran_TranType_RefNbr.Cache.Cached)
                        {
                            APTranExt tranExt = PXCacheEx.GetExtension<APTranExt>(row);

                            if (tranExt.UsrValuationType.Equals("W")) // W = ¥N¦©µ|
                            {
                                wHTTran.DocType = register.DocType;
                                wHTTran.RefNbr = register.RefNbr;

                                wHTTran = WHTTranView.Cache.Insert(wHTTran) as TWNWHTTran;

                                wHTTran.BatchNbr   = register.BatchNbr;
                                wHTTran.TranDate   = register.DocDate;
                                //wHTTran.PaymDate  = register.pay;
                                wHTTran.PersonalID = tWNWHT.PersonalID;
                                wHTTran.PropertyID = tWNWHT.PropertyID;
                                wHTTran.TypeOfIn   = tWNWHT.TypeOfIn;
                                wHTTran.WHTFmtCode = tWNWHT.WHTFmtCode;
                                wHTTran.WHTFmtSub  = tWNWHT.WHTFmtSub;
                                wHTTran.PayeeName  = (string)PXSelectorAttribute.GetField(Base.APDocument.Cache, register,
                                                                                          nameof(register.VendorID), register.VendorID, nameof(Vendor.acctName));
                                wHTTran.PayeeAddr = SelectAddress(Base, register.VendorID).AddressLine1;
                                wHTTran.WHTTaxPct = tWNWHT.WHTTaxPct;
                                wHTTran.WHTAmt    = row.CuryLineAmt * -1;
                                wHTTran.NetAmt    = register.CuryOrigDocAmt;

                                WHTTranView.Cache.Update(wHTTran);
                            }
                            if (tranExt.UsrValuationType.Equals("S")) // S = °·«O¸É¥R
                            {
                                Kedge.DAC.KGSetUp kGSetUp = SelectFrom<Kedge.DAC.KGSetUp>.View.ReadOnly.Select(Base);

                                wHTTran.GNHI2Pct  = kGSetUp.KGSupplPremiumRate;
                                wHTTran.GNHI2Code = tWNWHT.GNHI2Code;
                                wHTTran.GNHI2Amt  = row.CuryLineAmt * -1;

                                WHTTranView.Cache.Update(wHTTran);
                            }
                        }
                        // Manually Saving as base code will not call base graph persis.
                        WHTTranView.Cache.Persist(PXDBOperation.Insert);
                        WHTTranView.Cache.Persist(PXDBOperation.Update);

                        ts.Complete(Base);

                        // Triggering after save events.
                        WHTTranView.Cache.Persisted(false);
                    }
                }
            } 
        }
        #endregion

        #region Static Methods
        public static Address SelectAddress(PXGraph graph, int? vendorID)
        {
            return SelectFrom<Address>.InnerJoin<Vendor>.On<Vendor.bAccountID.IsEqual<Address.bAccountID>
                                                            .And<Vendor.defAddressID.IsEqual<Address.addressID>>>
                                      .Where<Vendor.bAccountID.IsEqual<@P.AsInt>>.View.ReadOnly.SelectSingleBound(graph, null, vendorID);
        }
        #endregion
    }
}