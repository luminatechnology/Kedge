using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;

/**
 * ====2021-05-18====
 * 當BAccount為員工時，回傳DefLocationID
 * 
 * **/
namespace NM.Util
{
    public class NMLocationUtil
    {
        public enum PaymentMethodID
        {
            TT = 'B',
            CHECK = 'A',
            GIFT = 'D',
            CASH = 'C',
            AUTH = 'E',
            WRITEOFF = 'F'
        }
        /// <summary>
        /// 取得預設LocationID
        /// </summary>
        /// <param name="baccountID">供應商 或 員工</param>
        /// <param name="paymentMethod">付款方式(Kedge.DAC.PaymentMethod)</param>
        /// <returns></returns>
        public static int? GetDefLocationByPaymentMethod(int? baccountID, string paymentMethod)
        {
            try
            {
                return GetDefLocationByPaymentMethod(baccountID, GetPaymentMethodID(paymentMethod));
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// 取得預設LocationID
        /// </summary>
        /// <param name="baccountID">供應商 或 員工</param>
        /// <param name="paymentMethodID">付款方式"TT" || "CHECK"</param>
        /// <returns></returns>
        public static int? GetDefLocationByPaymentMethod(int? baccountID, PaymentMethodID paymentMethodID)
        {
            string _paymentMethodID = null;
            switch (paymentMethodID)
            {
                case PaymentMethodID.TT:
                    _paymentMethodID = Kedge.DAC.word.TT;
                    break;
                case PaymentMethodID.CHECK:
                case PaymentMethodID.GIFT:
                case PaymentMethodID.AUTH:
                case PaymentMethodID.WRITEOFF:
                    _paymentMethodID = Kedge.DAC.word.CHECK;
                    break;
                default:
                    return null;
            }

            PXGraph graph = new PXGraph();
            BAccount bAcount = GetBAccount(graph, baccountID);
            if (bAcount?.Type == BAccountType.EmployeeType)
            {
                return bAcount?.DefLocationID;
            }
            else
            {
                Location defLocation = Location.PK.Find(graph, bAcount.BAccountID, bAcount.DefLocationID);
                if (defLocation?.PaymentMethodID != _paymentMethodID)
                {
                    Location location = GetLocation(graph, baccountID, _paymentMethodID);
                    if (location != null)
                    {
                        return location?.LocationID;
                    }
                    else
                    {
                        BAccount vendorBAcount = GetBAccount(graph, baccountID);
                        return vendorBAcount?.DefLocationID;
                    }
                }
                else
                {
                    return defLocation.LocationID;
                }

            }

        }

        public static void CheckKGBillPaymentLocation(KGBillPayment payment)
        {
            PXGraph graph = new PXGraph();
            if (PaymentMethodID.TT == GetPaymentMethodID(payment.PaymentMethod))
            {
                Location location = Location.PK.Find(graph, payment.VendorID, payment.VendorLocationID);
                if (location.VPaymentMethodID != "TT")
                {
                    throw new PXException("{0} 付款方式非TT", location.LocationCD);
                }
                foreach (VendorPaymentMethodDetail paymentMethodDetail in GetVendorPaymentMethodDetail(graph, location.BAccountID, location.LocationID, location.PaymentMethodID))
                {
                    if (paymentMethodDetail.DetailValue == null || paymentMethodDetail.DetailValue == "") {
                        throw new PXException("{0} 電匯所在地沒有維護匯款資訊!", location.LocationCD);
                    }
                }
            }
        }


        /// <summary>
        /// Kedge.DAC.PaymentMethod > NM.Util.NMLocationUtil.PaymentMethodID
        /// </summary>
        /// <param name="paymentMethod">付款方式(Kedge.DAC.PaymentMethod)</param>
        /// <returns></returns>
        private static PaymentMethodID GetPaymentMethodID(string paymentMethod)
        {
            Type eType = typeof(PaymentMethodID);
            Char payment = char.Parse(paymentMethod);
            return
            (PaymentMethodID)Enum.Parse(eType, Enum.GetName(eType, payment));
        }

        #region BQL
        private static BAccount GetBAccount(PXGraph graph, int? baccountID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(graph, baccountID);
        }

        private static Location GetLocation(PXGraph graph, int? baccountID, string paymentMehtodID)
        {

            return PXSelect<Location,
                Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
                    And<Location.vPaymentMethodID, Equal<Required<Location.vPaymentMethodID>>>>>
                    .Select(graph, baccountID, paymentMehtodID);
        }

        private static PXResultset<VendorPaymentMethodDetail> GetVendorPaymentMethodDetail(PXGraph graph, int? baccountID, int? locationID, string paymentMethodID)
        {
            return PXSelect<VendorPaymentMethodDetail,
                Where<
                    VendorPaymentMethodDetail.bAccountID, Equal<Required<VendorPaymentMethodDetail.bAccountID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Required<VendorPaymentMethodDetail.locationID>>,
                    And<VendorPaymentMethodDetail.paymentMethodID, Equal<Required<VendorPaymentMethodDetail.paymentMethodID>>>>>>
                .Select(graph, baccountID, locationID, paymentMethodID);
        }
        #endregion
    }
}
