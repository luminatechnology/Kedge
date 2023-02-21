using System;
using System.Collections.Generic;
using PS.DAC;
using PS.Util;
using PX.Data;
using PX.Objects.CR;
using RC.Util;

namespace PS
{
    public class PSPaymentSlipProcess : PXGraph<PSPaymentSlipProcess>
    {

        public PSPaymentSlipProcess()
        {
            DetailsView.SetProcessDelegate(delegate (List<PSPaymentSlipV> list)
            {
                List<PSPaymentSlip> datas = new List<PSPaymentSlip>();
                foreach (PSPaymentSlipV item in list)
                {
                    datas.Add(item);
                }
                Release(datas);
            });
            DetailsView.SetProcessCaption("Release");
            DetailsView.SetProcessAllCaption("Release All");

        }

        #region Action
        public PXCancel<PSPaymentSlipV> Cancel;
        #endregion

        #region HyperLink
        public PXAction<PSPaymentSlipV> ViewPaymentSlip;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewPaymentSlip()
        {
            new HyperLinkUtil<PSPaymentSlipEntry>(
                PXSelectorAttribute.Select<PSPaymentSlipV.refNbr>(DetailsView.Cache, DetailsView.Current),
                true
                );
        }
        #endregion

        #region View
        [PXFilterable]
        public PXProcessing<PSPaymentSlipV> DetailsView;
        #endregion

        #region Static Method - Release
        public static void Release(List<PSPaymentSlip> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                try
                {
                    Release(datas[i]);
                }
                catch (Exception e)
                {
                    PXProcessing.SetError(i, e);
                }
            }
        }

        public static void Release(PSPaymentSlip item)
        {
            PSPaymentSlipEntry entry = PXGraph.CreateInstance<PSPaymentSlipEntry>();
            entry.PaymentSlips.Current = item;
            entry.Release.Press();
        }
        #endregion


        #region Table
        [Serializable]
        [PXProjection(typeof(
                Select<PSPaymentSlip,
                    Where<PSPaymentSlip.status, Equal<PSStringList.PSStatus.open>>>
                ))]
        [PXHidden]
        public class PSPaymentSlipV : PSPaymentSlip
        {
            #region Selected
            [PXBool]
            [PXUIField(DisplayName = "Selected")]
            public virtual bool? Selected { get; set; }
            public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
            #endregion

            #region CustomerVendorCD
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Customer / Vendor", Enabled = false)]
            [PXUnboundDefault(typeof(Search<BAccount2.acctCD,
                Where<BAccount2.bAccountID, Equal<IsNull<Current2<customerID>, Current2<vendorID>>>>>))]
            [PXDBScalar(typeof(Search<BAccount2.acctCD,
                Where<BAccount2.bAccountID, Equal<IsNull<customerID, vendorID>>>>))]
            public virtual string CustomerVendorCD { get; set; }
            public abstract class customerVendorCD : PX.Data.BQL.BqlString.Field<customerVendorCD> { }
            #endregion

            #region CustomerVendorName
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Customer / Vendor Name", Enabled = false)]
            [PXUnboundDefault(typeof(Search<BAccount2.acctName,
                Where<BAccount2.bAccountID, Equal<IsNull<Current2<customerID>, Current2<vendorID>>>>>))]
            [PXDBScalar(typeof(Search<BAccount2.acctName,
                Where<BAccount2.bAccountID, Equal<IsNull<customerID, vendorID>>>>))]
            public virtual string CustomerVendorName { get; set; }
            public abstract class customerVendorName : PX.Data.BQL.BqlString.Field<customerVendorName> { }
            #endregion
        }

        [Serializable]
        public class XXX : IBqlTable
        {
        }

        #endregion



    }
}