using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using RCGV.GV.DAC;
using PX.Data;

namespace RCGV.GV.Util
{
    public class QREncrypter
    {

        public static String AESEncrypt(GVArGuiInvoice master, PXResultset<GVArGuiInvoiceDetail> detail)
        {
            DateTime date = DateTime.Parse(master.InvoiceDate.ToString());
            string twdate = Convert.ToInt16(date.AddYears(-1911).Year).ToString() +
                date.ToString("MMdd");
            int salesamt = Convert.ToInt32(master.SalesAmt);
            int totalamt = Convert.ToInt32(master.TotalAmt);
            string salesamt16 = salesamt.ToString("X8");
            string totalamt16 = totalamt.ToString("X8");
            PXGraph graph = new PXGraph();
            PXResultset<GVRegistration> set = PXSelect<GVRegistration,
                    Where<GVRegistration.registrationCD,
                        Equal<Required<GVRegistration.registrationCD>>>>
                        .Select(graph, master.RegistrationCD);
            string govuniformnbr = "";
            string stringseedkey = "";
            string custuniformnbr = "";
            foreach (GVRegistration uniformNumber in set)
            {
                govuniformnbr = uniformNumber.GovUniformNumber;
                stringseedkey = uniformNumber.QrCodeSeedString;
            }

            if (master.CustUniformNumber == null || master.CustUniformNumber.Length != 8)
            {
                custuniformnbr = "00000000";
            }
            else {
                custuniformnbr = master.CustUniformNumber;
            }
            String[][] strArr = new String[detail.Count][]; ;
            /*
            for (int i=0; i< detail.Count;i++)
            {
                GVArGuiInvoiceDetail gvdetail = detail[i];
                strArr[i] = new String[] { gvdetail.ItemDesc ,gvdetail.Quantity.ToString() , gvdetail.UnitPrice.ToString() } ;
            }*/
            string detailcount = detail.Count.ToString();
            String str =
                QRCodeINV(master.GuiInvoiceNbr, twdate, "000000", master.RandonNumber, (decimal)master.SalesAmt, (decimal)master.TaxAmt,
                    (decimal)master.TotalAmt, custuniformnbr, custuniformnbr, govuniformnbr, govuniformnbr, stringseedkey);
            str = str + ":**********:" + detailcount + ":" + detailcount + ":1";
            StringBuilder sb = new StringBuilder(str);
            string iteamdesc = "";
            string quantity = "";
            string unitprice = "";
            foreach (GVArGuiInvoiceDetail gvdetail in detail)
            {
                iteamdesc = gvdetail.ItemDesc;
                quantity = gvdetail.Qty.ToString();
                unitprice = gvdetail.UnitPrice.ToString();
                sb.Append(":" + iteamdesc);
                sb.Append(":" + quantity);
                sb.Append(":" + unitprice);
            }

            return sb.ToString();
        }


        /// <summary>
        /// 將發票資訊文字加密成驗證文字
        /// </summary>
        /// <param name="plainText">發票資訊</param>
        /// <param name="AESKey">種子密碼(QRcode)</param>
        /// <returns>加密後的HEX字串</returns>
        public  static string AESEncrypt(string plainText, string AESKey)
        {
            byte[] bytes = Encoding.Default.GetBytes(plainText);
            ICryptoTransform transform = new RijndaelManaged
            {
                KeySize = 0x80,
                Key = convertHexToByte(AESKey),
                BlockSize = 0x80,
                IV = Convert.FromBase64String("Dt8lyToo17X/XkXaQvihuA==")
            }.CreateEncryptor();
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream2.Close();
            return Convert.ToBase64String(stream.ToArray());
        }
        /// <summary>
        /// 轉換HEX值為 Binaries
        /// </summary>
        /// <param name="hexString">HEX字串</param>
        /// <returns>Binaries值</returns>
        private static byte[] convertHexToByte(string hexString)
        {
            byte[] buffer = new byte[hexString.Length / 2];
            int index = 0;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                int num3 = Convert.ToInt32(hexString.Substring(i, 2), 0x10);
                buffer[index] = BitConverter.GetBytes(num3)[0];
                index++;
            }
            return buffer;
        }
        /// <summary>
        /// 檢查發票輸入資訊
        /// </summary>
        /// <param name="InvoiceNumber">發票字軌號碼共 10 碼</param>
        /// <param name="InvoiceDate">發票開立年月日(中華民國年份月份日期)共 7 碼</param>
        /// <param name="InvoiceTime">發票開立時間 (24 小時制) 共 6 碼</param>
        /// <param name="RandomNumber">4碼隨機碼</param>
        /// <param name="SalesAmount">以整數方式載入銷售額 (未稅)，若無法分離稅項則記載為0</param>
        /// <param name="TaxAmount">以整數方式載入稅額，若無法分離稅項則記載為0</param>
        /// <param name="TotalAmount">整數方式載入總計金額(含稅)</param>
        /// <param name="BuyerIdentifier">買受人統一編號，若買受人為一般消費者，請填入 00000000 8位字串</param>
        /// <param name="RepresentIdentifier">代表店統一編號，電子發票證明聯二維條碼規格已不使用代表店，請填入00000000 8位字串</param>
        /// <param name="SellerIdentifier">銷售店統一編號</param>
        /// <param name="BusinessIdentifier">總機構統一編號，如無總機構請填入銷售店統一編號</param>
        /// <param name="productArray">單項商品資訊</param>
        /// <param name="AESKey">加解密金鑰(QR種子密碼)</param>
        private static void inputValidate(string InvoiceNumber,
            string InvoiceDate,
            string InvoiceTime,
            string RandomNumber,
            decimal SalesAmount,
            decimal TaxAmount,
            decimal TotalAmount,
            string BuyerIdentifier,
            string RepresentIdentifier,
            string SellerIdentifier,
            string BusinessIdentifier,
            string AESKey)
        {
            if (string.IsNullOrEmpty(InvoiceNumber) || (InvoiceNumber.Length != 10))
            {
                throw new Exception("Invaild InvoiceNumber: " + InvoiceNumber);
            }
            if (string.IsNullOrEmpty(InvoiceDate) || (InvoiceDate.Length != 7))
            {
                throw new Exception("Invaild InvoiceDate: " + InvoiceDate);
            }
            try
            {
                long num = long.Parse(InvoiceDate);
                int num2 = int.Parse(InvoiceDate.Substring(3, 2));
                int num3 = int.Parse(InvoiceDate.Substring(5));
                if ((num2 < 1) || (num2 > 12))
                {
                    throw new Exception();
                }
                if ((num3 < 1) || (num3 > 0x1f))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception("Invaild InvoiceDate: " + InvoiceDate);
            }
            if (string.IsNullOrEmpty(InvoiceTime))
            {
                throw new Exception("Invaild InvoiceTime: " + InvoiceTime);
            }
            if (string.IsNullOrEmpty(RandomNumber) || (RandomNumber.Length != 4))
            {
                throw new Exception("Invaild RandomNumber: " + RandomNumber);
            }
            if (SalesAmount < 0M)
            {
                throw new Exception("Invaild SalesAmount: " + SalesAmount);
            }
            if (TotalAmount < 0M)
            {
                throw new Exception("Invaild TotalAmount: " + TotalAmount);
            }
            if (string.IsNullOrEmpty(BuyerIdentifier) || (BuyerIdentifier.Length != 8))
            {
                throw new Exception("Invaild BuyerIdentifier: " + BuyerIdentifier);
            }
            if (string.IsNullOrEmpty(RepresentIdentifier))
            {
                throw new Exception("Invaild RepresentIdentifier: " + RepresentIdentifier);
            }
            if (string.IsNullOrEmpty(SellerIdentifier) || (SellerIdentifier.Length != 8))
            {
                throw new Exception("Invaild SellerIdentifier: " + SellerIdentifier);
            }
            if (string.IsNullOrEmpty(BusinessIdentifier))
            {
                throw new Exception("Invaild BusinessIdentifier: " + BusinessIdentifier);
            }
            if (string.IsNullOrEmpty(AESKey))
            {
                throw new Exception("Invaild AESKey");
            }
        }
        /// <summary>
        /// 產生發票左邊QR碼
        /// </summary>
        /// <param name="InvoiceNumber">發票字軌號碼共 10 碼</param>
        /// <param name="InvoiceDate">發票開立年月日(中華民國年份月份日期)共 7 碼</param>
        /// <param name="InvoiceTime">發票開立時間 (24 小時制) 共 6 碼</param>
        /// <param name="RandomNumber">4碼隨機碼</param>
        /// <param name="SalesAmount">以整數方式載入銷售額 (未稅)，若無法分離稅項則記載為0</param>
        /// <param name="TaxAmount">以整數方式載入稅額，若無法分離稅項則記載為0</param>
        /// <param name="TotalAmount">整數方式載入總計金額(含稅)</param>
        /// <param name="BuyerIdentifier">買受人統一編號，若買受人為一般消費者，請填入 00000000 8位字串</param>
        /// <param name="RepresentIdentifier">代表店統一編號，電子發票證明聯二維條碼規格已不使用代表店，請填入00000000 8位字串</param>
        /// <param name="SellerIdentifier">銷售店統一編號</param>
        /// <param name="BusinessIdentifier">總機構統一編號，如無總機構請填入銷售店統一編號</param>
        /// <param name="productArray">單項商品資訊</param>
        /// <param name="AESKey">加解密金鑰(QR種子密碼)</param>
        /// <returns></returns>
        public static string QRCodeINV(string InvoiceNumber,
            string InvoiceDate,
            string InvoiceTime,
            string RandomNumber,
            decimal SalesAmount,
            decimal TaxAmount,
            decimal TotalAmount,
            string BuyerIdentifier,
            string RepresentIdentifier,
            string SellerIdentifier,
            string BusinessIdentifier,
            string AESKey)
        {
            try
            {
                inputValidate(InvoiceNumber,
                    InvoiceDate,
                    InvoiceTime,
                    RandomNumber,
                    SalesAmount,
                    TaxAmount,
                    TotalAmount,
                    BuyerIdentifier,
                    RepresentIdentifier,
                    SellerIdentifier,
                    BusinessIdentifier,
                    AESKey);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return ((InvoiceNumber +
                InvoiceDate +
                RandomNumber +
                Convert.ToInt32(SalesAmount).ToString("x8") +
                Convert.ToInt32(TotalAmount).ToString("x8") +
                BuyerIdentifier + SellerIdentifier) +
                AESEncrypt(InvoiceNumber + RandomNumber, AESKey).PadRight(0x18));
        }


    }
}
