using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCGV.GV.DAC;
using PX.Data;

namespace RCGV.GV.Util
{

    public class GVGuiBookUtil
    {
        private static object lockObj = new object();
        public const String isLockError = "isLockError";
        public const String bookNoNumberError = "bookNoNumberError";
        public const String noDataError = "noDataError";
        public const String dateError = "dateError";
        /// <summary>
        /// 取得InvoiceNbr，且包含支票簿檢核
        /// </summary>
        /// <param name="guiBookID"></param>
        /// <returns></returns>
        public static String GetArInvoiceNumber(int? guiBookID, DateTime? invoiceDate)
        {
            lock (lockObj)
            {
                PXGraph graph = new PXGraph();
                PXResult<GVGuiBook, GVGuiNumberDetail, GVGuiNumber, GVGuiWord> data =
                    (PXResult<GVGuiBook, GVGuiNumberDetail, GVGuiNumber, GVGuiWord>)getGVGuiBook(graph, guiBookID);
                GVGuiBook book = (GVGuiBook)data;
                GVGuiWord word = (GVGuiWord)data;
                //判斷發票日期
                if (book.CurrentGuiDate != null && book.CurrentGuiDate > invoiceDate)
                {
                    String s = String.Format("InvoiceDate have to bigger than {0}", book.CurrentGuiDate?.ToString("yyyy/MM/dd"));
                    throw new PXException(s);
                }
                //判斷使用號碼
                if (book.EndNum.Equals(book.CurrentNum))
                {
                    String s = String.Format("發票號碼已用完");
                    throw new PXException(s);
                }

                //取號
                int addNbr = 0;
                if (book.CurrentNum == null)
                {
                    addNbr = int.Parse(book.StartNum);
                }
                else
                {
                    addNbr = int.Parse(book.CurrentNum);
                    addNbr++;
                }
                String nbrStr = word.GuiWordCD + formatInvoiceNumber(addNbr);

                PXUpdate<
                    Set<GVGuiBook.currentNum, Required<GVGuiBook.currentNum>,
                    Set<GVGuiBook.currentGuiDate, Required<GVGuiBook.currentGuiDate>>>,
                GVGuiBook,
                Where<GVGuiBook.guiBookID, Equal<Required<GVGuiBook.guiBookID>>>>
                .Update(graph, addNbr, invoiceDate, guiBookID);
                return nbrStr;
            }
        }

        public static PXResultset<GVGuiBook> getGVGuiBook(PXGraph graph, int? bookID)
        {
            return PXSelectJoin<GVGuiBook,
                    InnerJoin<GVGuiNumberDetail,
                        On<GVGuiNumberDetail.guiNumberDetailID, Equal<GVGuiBook.guiNumberDetailID>>,
                    InnerJoin<GVGuiNumber,
                        On<GVGuiNumber.guiNumberID, Equal<GVGuiNumberDetail.guiNumberID>>,
                    InnerJoin<GVGuiWord,
                         On<GVGuiWord.guiWordID, Equal<GVGuiNumber.guiWordID>>>>>,
                    Where<GVGuiBook.guiBookID, Equal<Required<GVGuiBook.guiBookID>>>>.Select(graph, bookID);
        }


        public static String formatInvoiceNumber(int invoiceNumber)
        {
            String nbr = Convert.ToString(invoiceNumber);
            nbr = nbr.PadLeft(8, '0');
            return nbr;
        }

        public class GVGuiBookUtilMessage
        {
            //enum ExecuteStatus { OK, isLockError, bookNoNumberError, noDataError};
            public String ArInvoiceNumber { get; set; }
            public String errorType { get; set; }
            public Boolean status { get; set; }
            public String checkDateMessage { get; set; }
            public String getLockMessage()
            {
                Dictionary<String, String> dic = getMessageDic();
                if (dic.ContainsKey(errorType))
                {
                    return dic[errorType];
                }
                return null;
            }

            private Dictionary<String, String> getMessageDic()
            {
                Dictionary<String, String> dic = new Dictionary<String, String>();
                dic.Add(GVGuiBookUtil.bookNoNumberError, "This BookID Gui Number is run out.");
                dic.Add(GVGuiBookUtil.isLockError, "This BookID IS Lock.");
                dic.Add(GVGuiBookUtil.noDataError, "This BookID does not search any data.");
                return dic;
            }
        }
    }
}
