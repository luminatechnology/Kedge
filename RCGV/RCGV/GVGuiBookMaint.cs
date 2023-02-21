using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.DAC;
using RCGV.GV.Util;

namespace RCGV.GV
{
    public class GVGuiBookMaint : PXGraph<GVGuiBookMaint, GVGuiBook>
    {
        public GVGuiBookMaint()
        {
            MasterView.Cache.AllowDelete = false;
        }

        #region Selects
        public PXSelect<GVGuiBook> MasterView;
        #endregion

        #region GV GuiBook Events

        protected virtual void GVGuiBook_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            GVGuiBook book = (GVGuiBook)e.NewRow;
            GVGuiBook originalBook = (GVGuiBook)e.Row;

            if (!sender.ObjectsEqual<GVGuiBook.startNum, GVGuiBook.endNum>(book, originalBook))
            {
                if (book.EndNum != null && book.StartNum != null)
                {
                    int totalNum = (int.Parse(book.EndNum) - int.Parse(book.StartNum)) + 1;

                    if (totalNum % 50 != 0)
                    {
                        sender.RaiseExceptionHandling<GVGuiBook.endNum>(book, originalBook.EndNum,
                               new PXSetPropertyException("Total num must mutiple of 50.", PXErrorLevel.Error));
                        e.Cancel = true;
                    }
                }
            }
        }


        protected void GVGuiBook_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
            {
                return;

            }
            GVGuiBook row = (GVGuiBook)e.Row;

            //MaxEndNum GuiNumberRange 為虛擬欄位需要隨時塞值
            if (row.GuiBookID > 0 &&
                row.GuiBookCD != null &&
                row.RegistrationCD != null &&
                row.GuiNumberDetailID != null)
            {
                // find last book
                GVGuiBook lastBook = PXSelectReadonly<GVGuiBook,
                    Where<GVGuiBook.registrationCD, Equal<Required<GVGuiBook.registrationCD>>,
                        And<GVGuiBook.guiNumberDetailID, Equal<Required<GVGuiBook.guiNumberDetailID>>>>,
                    OrderBy<Desc<GVGuiBook.endNum>>>.Select(this, row.RegistrationCD, row.GuiNumberDetailID);
                GVGuiNumberDetail numberDetail = PXSelectorAttribute.Select<GVGuiBook.guiNumberDetailID>(cache, row) as GVGuiNumberDetail;
                if (numberDetail != null)
                {
                    row.MaxEndNum = numberDetail.EndNumber;
                    row.GuiNumberRange = string.Format("{0} - {1}", numberDetail.StartNumber, numberDetail.EndNumber);
                }
            }
            //------------Enable------------------------------------

            if (row.Hold == true)
            {
                PXUIFieldAttribute.SetEnabled(MasterView.Cache, MasterView.Current, true);
                Delete.SetEnabled(true);
                if (row.GuiBookID > 0 && row.GuiBookCD != null && row.RegistrationCD != null && row.GuiNumberDetailID != null)
                {
                    // find last book
                    GVGuiBook lastBook = PXSelectReadonly<GVGuiBook,
                        Where<GVGuiBook.registrationCD, Equal<Required<GVGuiBook.registrationCD>>,
                            And<GVGuiBook.guiNumberDetailID, Equal<Required<GVGuiBook.guiNumberDetailID>>>>,
                        OrderBy<Desc<GVGuiBook.endNum>>>.Select(this, row.RegistrationCD, row.GuiNumberDetailID);

                    if (lastBook != null)
                    {
                        // if not last row can't edit endNum
                        PXUIFieldAttribute.SetEnabled<GVGuiBook.endNum>(cache, row, lastBook.GuiBookCD == row.GuiBookCD);
                    }
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<GVGuiBook.endNum>(cache, row, row.GuiNumberDetailID != null);
                }
                if (PXEntryStatus.Inserted == cache.GetStatus(row))
                {
                    PXUIFieldAttribute.SetEnabled<GVGuiBook.declareYear>(cache, row, true);
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<GVGuiBook.declareYear>(cache, row, false);
                }
            }
            else
            {
                PXUIFieldAttribute.SetEnabled(MasterView.Cache, MasterView.Current, false);
                PXUIFieldAttribute.SetEnabled<GVGuiBook.guiBookCD>(MasterView.Cache, MasterView.Current, true);

                Delete.SetEnabled(false);

                bool isUse = (row.CurrentNum != null);
                PXUIFieldAttribute.SetEnabled<GVGuiBook.hold>(MasterView.Cache, MasterView.Current, !isUse);
            }








        }

        protected void GVGuiBook_DeclareYear_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            GVGuiBook row = (GVGuiBook)e.Row;
            cache.SetValue<GVGuiBook.declareYear>(e.Row, DateTime.Now.Year);
        }

        protected virtual void GVGuiBook_StartMonth_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVGuiBook row = e.Row as GVGuiBook;
            if (row.EndMonth == null) return;
            if (e.NewValue == null) return;

            int startMonth = (int)e.NewValue;
            int maxMonth = GVList.GVDeclarePeriod.GetMaxMonth(row.DeclarePeriod);
            if (startMonth < maxMonth - 1 ||
                startMonth > maxMonth)
                // Throwing an exception to show the error on the field
                throw new PXSetPropertyException(string.Format("Start month must between {0} and {1}.", maxMonth - 1, maxMonth));

            if (startMonth > row.EndMonth)
            {
                throw new PXSetPropertyException("Start month must less than end month.");
            }
        }

        protected virtual void GVGuiBook_EndMonth_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVGuiBook row = e.Row as GVGuiBook;
            if (row.StartMonth == null) return;
            if (e.NewValue == null) return;

            int endMonth = (int)e.NewValue;
            int maxMonth = GVList.GVDeclarePeriod.GetMaxMonth(row.DeclarePeriod);
            if (endMonth < maxMonth - 1 ||
                endMonth > maxMonth)
                // Throwing an exception to show the error on the field
                throw new PXSetPropertyException(string.Format("End month must between {0} and {1}.", maxMonth - 1, maxMonth));

            if (endMonth < row.StartMonth)
            {
                throw new PXSetPropertyException("End month must greater than start month.");
            }
        }

        protected virtual void GVGuiBook_RegistrationCD_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVGuiBook row = (GVGuiBook)e.Row;
            GVRegistration registration = null;

            if (row.RegistrationCD != null)
            {
                registration = PXSelectorAttribute.Select<GVGuiBook.registrationCD>(sender, row) as GVRegistration;
                if (registration != null)
                {
                    row.GovUniformNumber = registration.GovUniformNumber;
                }
            }
            if (registration == null)
            {
                row.GovUniformNumber = null;
            }
        }

        protected virtual void GVGuiBook_GuiNumberDetailID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            GVGuiBook row = (GVGuiBook)e.Row;
            GVGuiNumberDetail numberDetail = null;

            if (row.GuiNumberDetailID != null)
            {
                numberDetail = PXSelectorAttribute.Select<GVGuiBook.guiNumberDetailID>(sender, row) as GVGuiNumberDetail;
                if (numberDetail != null)
                {
                    row.GuiWordCD = numberDetail.GuiWordCD;
                    row.GuiNumberRange = string.Format("{0} - {1}", numberDetail.StartNumber, numberDetail.EndNumber);
                    GVGuiNumber numberRow = PXSelect<GVGuiNumber, Where<GVGuiNumber.guiNumberID, Equal<Required<GVGuiNumber.guiNumberID>>>>.Select(this, numberDetail.GuiNumberID);
                    if (numberRow != null)
                    {
                        row.DeclarePeriod = numberRow.DeclarePeriod;

                        row.EndMonth = GVList.GVDeclarePeriod.GetMaxMonth(row.DeclarePeriod);
                        row.StartMonth = row.EndMonth - 1;
                    }

                    // find last book
                    GVGuiBook lastBook = PXSelectReadonly<GVGuiBook,
                        Where<GVGuiBook.registrationCD, Equal<Required<GVGuiBook.registrationCD>>,
                            And<GVGuiBook.guiNumberDetailID, Equal<Required<GVGuiBook.guiNumberDetailID>>>>,
                        OrderBy<Desc<GVGuiBook.endNum>>>.Select(this, row.RegistrationCD, row.GuiNumberDetailID);
                    if (lastBook != null)
                    {
                        row.StartNum = (int.Parse(lastBook.EndNum) + 1).ToString().PadLeft(8, '0');
                    }
                    else
                    {
                        row.StartNum = numberDetail.StartNumber;
                    }
                    row.MaxEndNum = numberDetail.EndNumber;
                    row.EndNum = null;
                }
                else
                {
                    row.StartNum = null;
                    row.EndNum = null;
                    row.GuiWordCD = null;
                    row.DeclarePeriod = null;
                    row.DeclareYear = null;
                    row.GuiNumberRange = null;
                }
            }
            else
            {
                row.StartNum = null;
                row.EndNum = null;
                row.GuiWordCD = null;
                row.DeclarePeriod = null;
                row.DeclareYear = null;
                row.GuiNumberRange = null;
            }
        }

        protected virtual void GVGuiBook_EndNum_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            GVGuiBook row = e.Row as GVGuiBook;
            if (row.MaxEndNum == null) return;
            if (e.NewValue == null) return;

            String newEndNum = (string)e.NewValue;
            //if ((int.Parse(newEndNum) % 50) != 0)
            //{
            //	// Throwing an exception to show the error on the field
            //	throw new PXSetPropertyException("End num must mutiple of 50.");
            //}
            if (row.StartNum.CompareTo(newEndNum) > 0)
            {
                // Throwing an exception to show the error on the field
                throw new PXSetPropertyException("End num must be greater than or equal to start num.");

            }
            if (newEndNum.CompareTo(row.MaxEndNum) > 0)
            {
                // Throwing an exception to show the error on the field
                throw new PXSetPropertyException(string.Format("End num must be less than or equal to {0}.", row.MaxEndNum));
            }
            row.RemainCount = int.Parse(newEndNum) - int.Parse(row.StartNum);
        }

        #endregion

        #region Misc
        private Boolean ValidDateRange(int periodYear, int declarePeriod, DateTime dt)
        {
            int maxMonth = declarePeriod;
            int minMonth = declarePeriod - 1;
            DateTime minDT = new DateTime(periodYear, minMonth, 1, 0, 0, 0);
            DateTime maxDT = new DateTime(periodYear, maxMonth, DateTime.DaysInMonth(periodYear, maxMonth), 23, 59, 59);

            if (minDT.CompareTo(dt) > 0 ||
                maxDT.CompareTo(dt) < 0)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}