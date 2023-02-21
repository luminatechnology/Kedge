using PX.Data;
using NM.DAC;
using System;

namespace NM.Util
{
    public class NMCheckBookUtil
    {

        private static object lockObj = new object();
        /// <summary>
        /// 取得發票號碼
        /// </summary>
        /// <param name="bookCD"></param>
        /// <returns></returns>
        public static string getCheckNbr(string bookCD)
        {
            if (String.IsNullOrEmpty(bookCD)) return null;
            lock (lockObj)
            {
                PXGraph graph = new PXGraph();
                NMCheckBook checkBook = getNMCheckBook(bookCD);
                if (int.Parse(checkBook.EndCheckNbr) == int.Parse(checkBook.CurrentCheckNbr ?? "0"))
                {
                    throw new Exception("支票號碼已使用完：" + bookCD);
                }
                //號碼長度
                int lenght = checkBook.StartCheckNbr.Trim().Length;
                int currentNo = 0;
                if (checkBook.CurrentCheckNbr == null)
                {
                    currentNo = int.Parse(checkBook.StartCheckNbr);
                }
                else {
                    currentNo = int.Parse(checkBook.CurrentCheckNbr)+1;
                }
                //當前取號
                string currentNbr = currentNo.ToString().PadLeft(lenght, '0');

                PXUpdate<
                    Set<NMCheckBook.currentCheckNbr, Required<NMCheckBook.currentCheckNbr>>,
                    NMCheckBook, Where<NMCheckBook.bookID, Equal<Required<NMCheckBook.bookID>>>>
                    .Update(new PXGraph(), currentNbr, checkBook.BookID);



                return checkBook.CheckWord + currentNbr;
            }
        }

        private static NMCheckBook getNMCheckBook(string bookCD)
        {
            NMCheckBook checkBook = PXSelect<NMCheckBook,
                Where<NMCheckBook.bookCD, Equal<Required<NMCheckBook.bookCD>>>>.Select(new PXGraph(), bookCD);
            if (checkBook == null) throw new Exception("找不到此支票簿：" + bookCD);
            return checkBook;
        }
    }
}
