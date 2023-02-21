using System;
using PX.Data;
using NM.DAC;
using RC.Util;

namespace NM
{
    public class NMCheckBookMaint : PXGraph<NMCheckBookMaint, NMCheckBook>
    {
        public NMCheckBookMaint()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_RECEIVABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }


        #region Selects
        public PXSelect<NMCheckBook> CheckBooks;


        #endregion

        #region Events
        protected virtual void NMCheckBook_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMCheckBook row = (NMCheckBook)e.Row;
            if (row == null) return;
            setBankAccountID(sender, row);
            setDisabled();
        }

        protected virtual void _(Events.RowPersisting<NMCheckBook> e)
        {
            NMCheckBook row = (NMCheckBook)e.Row;
            if (row == null) return;
            if (CheckBooks.Cache.GetStatus(row) != PXEntryStatus.Deleted)
            {
                e.Cancel = !BeforeCheck();
            }
        }

        //protected virtual void _(Events.RowInserting<NMCheckBook> e)
        //{
        //    NMCheckBook row = (NMCheckBook)e.Row;
        //    PXEntryStatus status=  CheckBooks.Cache.GetStatus(row);
        //    if (row == null) return;
        //    e.Cancel = BeforeCheck();
        //}

        protected virtual void _(Events.RowDeleting<NMCheckBook> e)
        {
            NMCheckBook row = (NMCheckBook)e.Row;
            if (row == null) return;
            bool isUsing = getUsingCount(row.BookCD) != 0;
            if (isUsing)
            {
                String msg = "���䲼ï���w�Q�ϥ�";
                PXSetPropertyException ex = new PXSetPropertyException(msg, PXErrorLevel.RowError);
                CheckBooks.Cache.RaiseExceptionHandling<NMCheckBook.bookCD>(row, row.BookCD, ex);
            }
            e.Cancel = isUsing;
        }





        #endregion

        #region Medhod
        /**
         * <summary>
         * �Ȧ�ѷӸ��X����
         * </summary>
         * **/
        private void setBankAccountID(PXCache sender, NMCheckBook checkBook)
        {
            NMBankAccount bankAccount = (NMBankAccount)PXSelectorAttribute.Select<NMCheckBook.bankAccountID>(sender, checkBook);
            if (bankAccount != null)
            {
                checkBook.BankAccount = bankAccount.BankCode + "-" + bankAccount.BankAccount;
            }
            else
            {
                checkBook.BankAccount = null;
            }
        }
        /// <summary>
        /// ��Ū�]�w
        /// </summary>
        private void setDisabled()
        {
            NMCheckBook row = (NMCheckBook)CheckBooks.Current;
            if (row == null) return;
            //���o��ï�Q�ϥΫh���i�Q�R��& ���b��ID���i����
            bool isUsing = getUsingCount(row.BookCD) != 0;
            Delete.SetEnabled(!isUsing);
            PXUIFieldAttribute.SetEnabled<NMCheckBook.bankAccountID>(CheckBooks.Cache, row, !isUsing);

            bool readyonly = row.CurrentCheckNbr != null;
            PXUIFieldAttribute.SetReadOnly<NMCheckBook.startDate>(CheckBooks.Cache, row, readyonly);
            PXUIFieldAttribute.SetReadOnly<NMCheckBook.bankAccountID>(CheckBooks.Cache, row, readyonly);
            PXUIFieldAttribute.SetReadOnly<NMCheckBook.checkWord>(CheckBooks.Cache, row, readyonly);
            PXUIFieldAttribute.SetReadOnly<NMCheckBook.startCheckNbr>(CheckBooks.Cache, row, readyonly);
            PXUIFieldAttribute.SetReadOnly<NMCheckBook.endCheckNbr>(CheckBooks.Cache, row, readyonly);
            PXUIFieldAttribute.SetReadOnly<NMCheckBook.bookUsage>(CheckBooks.Cache, row, row.BookUsage != null && row.BookID > 0);

        }

        public bool BeforeCheck()
        {
            bool flag = true;
            flag = flag && checkNbrVerifying();
            return flag;
        }

        public bool checkNbrVerifying()
        {
            NMCheckBook row = CheckBooks.Current;
            if (row.EndCheckNbr.Trim().Length != row.StartCheckNbr.Trim().Length)
            {
                String msg = "�r�ƻݬ۵�";
                PXSetPropertyException e = new PXSetPropertyException(msg, PXErrorLevel.RowError);
                CheckBooks.Cache.RaiseExceptionHandling<NMCheckBook.startCheckNbr>(row, row.StartCheckNbr, e);
                CheckBooks.Cache.RaiseExceptionHandling<NMCheckBook.endCheckNbr>(row, row.EndCheckNbr, e);
                return false;
            }
            if (Int64.Parse(row.EndCheckNbr) == Int64.Parse(row.StartCheckNbr))
            {
                String msg = "���X���i�۵�";
                PXSetPropertyException e = new PXSetPropertyException(msg, PXErrorLevel.RowError);
                CheckBooks.Cache.RaiseExceptionHandling<NMCheckBook.startCheckNbr>(row, row.StartCheckNbr, e);
                CheckBooks.Cache.RaiseExceptionHandling<NMCheckBook.endCheckNbr>(row, row.EndCheckNbr, e);
                return false;
            }
            if (Int64.Parse(row.EndCheckNbr) < Int64.Parse(row.StartCheckNbr))
            {
                String msg = "�_�����i�j�󨴸�";
                PXSetPropertyException e = new PXSetPropertyException(msg, PXErrorLevel.RowError);
                CheckBooks.Cache.RaiseExceptionHandling<NMCheckBook.startCheckNbr>(row, row.StartCheckNbr, e);
                CheckBooks.Cache.RaiseExceptionHandling<NMCheckBook.endCheckNbr>(row, row.EndCheckNbr, e);
                return false;
            }
            return true;
        }

        private int getUsingCount(String bookNbr)
        {
            return PXSelect<NMPayableCheck, Where<NMPayableCheck.bookNbr, Equal<Required<NMPayableCheck.bookNbr>>>>
                .Select(this, bookNbr).Count;
        }

        #endregion


    }
}