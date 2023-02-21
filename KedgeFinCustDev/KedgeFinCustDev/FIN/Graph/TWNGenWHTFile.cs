using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using Fin.DAC;
using FIN.Util;
// Fin.Descriptor;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.GL ;
using static PX.Objects.CS.BranchMaint;
using System.Collections;
using RCGV.GV.DAC;

namespace Fin.Graph
{
    public class TWNGenWHTFile : PXGraph<TWNGenWHTFile>
    {
        #region Filter & Process & Features
        public PXCancel<WHTTranFilter> Cancel;
        public PXFilter<WHTTranFilter> Filter;

        public PXFilteredProcessingJoinGroupBy<TWNWHTTran, WHTTranFilter,
                                    InnerJoin<Branch,On<TWNWHTTran.branchID,Equal<Branch.branchID>>,
                                    InnerJoin<BAccount,On<BAccount.bAccountID,Equal<Branch.bAccountID>>>>,
                                    Where<DatePart<DatePart.year, TWNWHTTran.tranDate>, GreaterEqual<Current<WHTTranFilter.vidsYearFrom>>,
                                        And<DatePart<DatePart.year, TWNWHTTran.tranDate>, LessEqual<Current<WHTTranFilter.vidsYearTo>>,
                                        And<DatePart<DatePart.month,TWNWHTTran.tranDate>,GreaterEqual<Current<WHTTranFilter.payMonthFrom>>,
                                        And<DatePart<DatePart.month,TWNWHTTran.tranDate>, LessEqual<Current<WHTTranFilter.payMonthTo>>,
                                        And<TWNWHTTran.branchID, Equal<Current<WHTTranFilter.branchID>>,
                                        And<TWNWHTTran.tranType, Equal<FinStringList.TranType.wht>>>>>>>,
                                    Aggregate<GroupBy<TWNWHTTran.personalID,
                                                       GroupBy<TWNWHTTran.wHTFmtCode,
                                                           Sum<TWNWHTTran.wHTAmt,
                                                           Sum<TWNWHTTran.gNHI2Amt,
                                                           Sum<TWNWHTTran.netAmt>>>>>>> WHTTranProc;

        //public PXSetup<TWNGUIPreferences> gUIPreferSetup;
        #endregion

        #region Constructor
        public TWNGenWHTFile()
        {
            WHTTranProc.SetProcessCaption("Process All");
            WHTTranProc.SetProcessVisible(false);
            WHTTranProc.SetProcessAllCaption("Process All");
            WHTTranProc.SetProcessAllVisible(true);
            WHTTranProc.SetProcessDelegate(Process);
        }
        #endregion

        #region Events

        #region Filter
        protected void _(Events.RowSelected<WHTTranFilter> e)
        {
            WHTTranFilter filter = e.Row;
            if (filter == null) return;
            filter.SelectedCount = WHTTranProc.Select().Count;
            decimal? TotalWhtAmt = 0m;
            decimal? TotalPayAmt = 0m;
            decimal? TotalGNHI2Amt = 0m;
            foreach (TWNWHTTran tran in WHTTranProc.Select())
            {
                TotalWhtAmt = (tran.WHTAmt ?? 0m) + TotalWhtAmt;
                TotalPayAmt = (tran.PayAmt ?? 0m) + TotalPayAmt;
                TotalGNHI2Amt = (tran.GNHI2Amt ?? 0m) + TotalGNHI2Amt;
            }
            filter.TotalWHTAmt = TotalWhtAmt;
            filter.TotalPayAmt = TotalPayAmt;
            filter.TotalGNHI2Amt = TotalGNHI2Amt;
        }
        protected void _(Events.FieldDefaulting<WHTTranFilter, WHTTranFilter.withHoldingYearFrom> e)
        {
            WHTTranFilter filter = e.Row;
            if (filter == null) return;
            //�NbusinessDate���~���ഫ������~
            filter.WithHoldingYearFrom = TransferYear(1,((DateTime)Accessinfo.BusinessDate).Year);
        }
        protected void _(Events.FieldDefaulting<WHTTranFilter, WHTTranFilter.withHoldingYearTo> e)
        {
            WHTTranFilter filter = e.Row;
            if (filter == null) return;
            //�NbusinessDate���~���ഫ������~
            filter.WithHoldingYearTo = TransferYear(1, ((DateTime)Accessinfo.BusinessDate).Year);
        }
        protected void _(Events.FieldDefaulting<WHTTranFilter, WHTTranFilter.vidsYearFrom> e)
        {
            WHTTranFilter filter = e.Row;
            if (filter == null) return;
            if (filter.WithHoldingYearFrom != null)
                filter.VidsYearFrom = TransferYear(0, (int)filter.WithHoldingYearFrom);
        }
        protected void _(Events.FieldDefaulting<WHTTranFilter, WHTTranFilter.vidsYearTo> e)
        {
            WHTTranFilter filter = e.Row;
            if (filter == null) return;
            if (filter.WithHoldingYearTo != null)
                filter.VidsYearTo = TransferYear(0, (int)filter.WithHoldingYearTo);
        }

        protected void _(Events.FieldUpdated<WHTTranFilter, WHTTranFilter.withHoldingYearFrom> e)
        {
            WHTTranFilter filter = e.Row;
            if (filter == null) return;
            Filter.Cache.SetDefaultExt<WHTTranFilter.vidsYearFrom>(filter);
        }
        protected void _(Events.FieldUpdated<WHTTranFilter, WHTTranFilter.withHoldingYearTo> e)
        {
            WHTTranFilter filter = e.Row;
            if (filter == null) return;
            Filter.Cache.SetDefaultExt<WHTTranFilter.vidsYearTo>(filter);
        }
 
        #endregion

        #endregion

        #region Variable Defintion
        public char zero = '0';
        public char space = ' ';
        public int fixedLen = 0;
        public string ourTaxNbr = "";
        public string combinStr = "";
        public string bar = "|";
        #endregion

        #region Function
        //TWNGenGUIMediaFile mediaGraph = PXGraph.CreateInstance<TWNGenGUIMediaFile>();

        public void Process(List<TWNWHTTran> wHTTranList)
        {

            try
            {
                //TWNGUIPreferences gUIPreferences = gUIPreferSetup.Current;

                int count = 1;
                string lines = "", fileName = "";

                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
                    {
                        fileName = Filter.Current.TaxRegistrationID + "." + 
                                          Filter.Current.WithHoldingYearFrom.ToString() + "." 
                                          + "U8.txt";
                            //this.Accessinfo.BusinessDate.Value.ToString("yyyyMMdd") + ".txt";

                        foreach (TWNWHTTran row in WHTTranProc.Select())
                        {
                            // �]�x�����N��
                            GVRegistration registration = GetGVRegistration(Filter.Current.BranchID);
                            lines = registration.TaxAuthority + bar;
                            // �y����
                            lines += AutoNumberAttribute.GetNextNumber(
                                            WHTTranProc.Cache, row, "WHTMEDIAID", Accessinfo.BusinessDate)
                                            + bar ; 
                            // ��ú������Q�Ʒ~�Τ@�s��
                            lines += Filter.Current.TaxRegistrationID + bar;
                            // �ұo���O
                            lines += row.WHTFmtSub + bar;
                            // �ұo�榡
                            lines += row.WHTFmtCode + bar;
                            // �ұo�H�Τ@�s(��)��
                            lines += row.PersonalID + bar;
                            // �Ҹ��O
                            lines += row.TypeOfIn + bar;
                            // ��ú�̳浹�I�`�B
                            lines += Math.Round((row.PayAmt ?? 0m), MidpointRounding.AwayFromZero).ToString()  + bar;//GetStrAmt((row.WHTAmt + row.GNHI2Amt + row.NetAmt).Value);
                            // ��ú�̳榩ú�|�B
                            lines += Math.Round((row.WHTAmt ?? 0m), MidpointRounding.AwayFromZero).ToString() + bar;
                            // ��ú�̳浹�I�b�B
                            lines += Math.Round((row.NetAmt ?? 0m), MidpointRounding.AwayFromZero).ToString() + bar;//GetStrAmt((row.GNHI2Amt + row.NetAmt).Value);
                            // �ұo�H�N��(�αb��)�B����Ыε|�y�s���B����~�ȧO�B�Z�O���n�O�ΧO�B��L�ұo���I���إN��
                            if (row.WHTFmtCode == "9A")
                                lines += row.WHTFmtSub + bar;
                            else
                                lines += row.PropertyID + bar;
                            // Blank
                            lines += bar;
                            // Blank
                            lines += bar;
                            // �n����O
                            lines += bar;
                            // ���~���O
                            lines += bar;
                            // �ұo���I�~��
                            lines += TransferYear(1, ((DateTime)row.PaymentDate).Year).ToString() + bar;
                            // �ұo�H�m�W/�W��
                            lines += row.PayeeName + bar;
                            // �ұo�H�a�}
                            lines += row.PayeeAddr + bar;
                            // �ұo���ݴ���
                            /// The format is YYYMMYYYMM (From payment date and To payment date), the Chinese year and month are retrieved from the payment date entered in the selection criteria. 
                            lines += Convert.ToString(Filter.Current.WithHoldingYearFrom) + Convert.ToString(Filter.Current.PayMonthFrom) +
                                         Convert.ToString(Filter.Current.WithHoldingYearTo) + Convert.ToString(Filter.Current.PayMonthTo) + bar;
                            //GetGUILegal(Filter.Current.FromPaymDate.Value) + GetGUILegal(Filter.Current.ToPaymDate.Value);
                            // �̳Ұh���ҩα�¾���h�����Ҧ��@��ú���h����B
                            lines += bar;
                            // Blank
                            lines += bar;
                            // Blank
                            lines += bar;
                            // Blank
                            lines += bar;
                            // Blank
                            lines += bar;
                            //����|�B���O
                            lines += bar;
                            //�̳��o�覡
                            lines += "1";
                            // �O�_��183��
                            /// Default is blank, If �Ҹ��O = 5 or 6 or 7 or 8 or 9 then "N"
                            lines += bar;//(int.Parse(row.TypeOfIn) >= 5) ? 'N' : space;
                            // �~��a��Φa�ϥN�X
                            if(row.TypeOfIn == "3" || row.TypeOfIn =="5" || row.TypeOfIn == "6" || row.TypeOfIn == "7"
                                || row.TypeOfIn == "8" || row.TypeOfIn == "9" || row.TypeOfIn == "A")
                                lines += row.Jurisdiction + bar; //GetVendCountryID(row.DocType, row.RefNbr);
                            else
                                lines += bar;
                            // ���|��w�N�X
                            if (row.TypeOfIn == "5" || row.TypeOfIn == "6" || row.TypeOfIn == "7"
                               || row.TypeOfIn == "8" || row.TypeOfIn == "9")
                                lines += row.TaxationAgreements + bar;//GetVendCountryID(row.DocType, row.RefNbr);
                            else
                                lines += bar;
                            // Blank
                            lines += bar;
                            // �ɮ׻s�@����B�D�~��̩ұo���I���B�M��֭�����B�M�⧹�����
                            lines += this.Accessinfo.BusinessDate.Value.ToString("MMdd") + bar;
                            // �|���ѧO�X(TIN)
                            lines += bar;

                            // Only the last line does not need to be broken.
                            if (count < WHTTranProc.Select().Count)
                            {
                                sw.WriteLine(lines);
                                count++;
                            }
                            else
                            {
                                sw.Write(lines);
                            }
                        }

                        sw.Close();

                        throw new PXRedirectToFileException(new PX.SM.FileInfo(Guid.NewGuid(), fileName,
                                                                               null, stream.ToArray(), string.Empty),
                                                            true);
                    }
                }
            }
            catch (Exception ex)
            {
                PXProcessing<TWNWHTTran>.SetError(ex);
                throw;
            }
        }

        public string GetStrAmt(decimal amount)
        {
            fixedLen  = 10;
            combinStr = amount.ToString();
            
            return combinStr.PadLeft(fixedLen, zero);
        }

        public string GetGUILegal(DateTime dateTime)
        {
            var tWCalendar = new System.Globalization.TaiwanCalendar();

            int year = tWCalendar.GetYear(dateTime);
            string month = DateTime.Parse(dateTime.ToString()).ToString("MM");

            return string.Format("{0}{1}", year, month);
        }


        /// <summary>
        /// If TWN_WhtTrans.FormatCode ='9A', then TWN_WhtTrans.FormatSubCode 
        /// else(TWN_WhtTrans.PropertyID if TWN_WhtTrans.PropertyID is not blank)
        /// </summary>
        public string GetFormatNbr(TWNWHTTran wHTTran)
        {
            if (wHTTran.WHTFmtCode == "9A")
            {
                return wHTTran.WHTFmtSub;
            }
            else
            {
                return string.IsNullOrEmpty(wHTTran.PropertyID) ? new string(space, 12) : wHTTran.PropertyID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type">
        /// Transfer Year To //
        /// 0 : �褸
        /// 1 : ����
        /// </param>
        /// <param name="Year">
        /// Type = 0:�褸, �ж����~,
        /// Type = 1:����, �ж�褸�~
        /// </param>
        /// <returns> �褸�~/����~</returns>
        public int? TransferYear(int Type ,int Year)
        {
            int Years = 0;
            if (Type == 0)
                Years = Year + 1911;
            if (Type == 1)
                Years = Year - 1911;
            return Years;
        }
        #endregion

        #region Search Method
        private string GetVendCountryID(string docType, string refNbr)
        {
            Address address= SelectFrom<Address>.LeftJoin<BAccount>.On<BAccount.bAccountID.IsEqual<Address.bAccountID>
                                                                       .And<BAccount.defAddressID.IsEqual<Address.addressID>>>
                                                .LeftJoin<APRegister>.On<APRegister.vendorID.IsEqual<BAccount.bAccountID>>
                                                .Where<APRegister.docType.IsEqual<@P.AsString>
                                                      .And<APRegister.refNbr.IsEqual<@P.AsString>>>.View.ReadOnly.Select(this, docType, refNbr);

            return address != null ? address.CountryID : new string(space, 2);
        }
        private GVRegistration GetGVRegistration(int? BranchID)
        {
            return PXSelectJoin<GVRegistration,
                InnerJoin<GVRegistrationBranch, On<GVRegistrationBranch.registrationID, Equal<GVRegistration.registrationID>>,
                InnerJoin<Branch, On<Branch.bAccountID, Equal<GVRegistrationBranch.bAccountID>>>>,
                Where<Branch.branchID, Equal<Required<Branch.branchID>>>>
                .Select(this, BranchID);
        }
        #endregion

    }



    #region Filter DAC
    [Serializable]
    [PXCacheName("WHT Transaction Filter")]
    public class WHTTranFilter : IBqlTable
    {
        /* 2021/09/06 Delete
        #region FromDate
        [PXDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "From Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? FromDate { get; set; }
        public abstract class fromDate : PX.Data.BQL.BqlDateTime.Field<fromDate> { }
        #endregion

        #region ToDate
        [PXDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "To Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? ToDate { get; set; }
        public abstract class toDate : PX.Data.BQL.BqlDateTime.Field<toDate> { }
        #endregion 
        

        #region FromPaymDate
        [PXDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "From Payment Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? FromPaymDate { get; set; }
        public abstract class fromPaymDate : PX.Data.BQL.BqlDateTime.Field<fromPaymDate> { }
        #endregion

        #region ToPaymDate
        [PXDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "To Payment Date", Visibility = PXUIVisibility.Visible)]
        public virtual DateTime? ToPaymDate { get; set; }
        public abstract class toPaymDate : PX.Data.BQL.BqlDateTime.Field<toPaymDate> { }
        #endregion
        */
       

        //2021/09/06 Add Mantis: 0012223
        //Filter Data
        #region BranchID
        [PXInt()]
        [PXUIField(DisplayName = "BranchID",IsReadOnly =true)]
        [PXSelector(typeof(Search<Branch.branchID>),
            typeof(Branch.branchCD),
            typeof(Branch.acctName),
            SubstituteKey = typeof(Branch.branchCD),
            DescriptionField = typeof(Branch.acctName))]
        [PXUnboundDefault(typeof(AccessInfo.branchID))]

        public virtual int? BranchID { get; set; }
        public abstract class branchID : IBqlField { }
        #endregion

        #region TaxRegistrationID
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "TaxRegistrationID", IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search2<BAccount.taxRegistrationID,
            InnerJoin<Branch,On<Branch.bAccountID,Equal<BAccount.bAccountID>>>,
            Where<Branch.branchID,Equal<Current<branchID>>>>))]
        public virtual string TaxRegistrationID { get; set; }
        public abstract class taxRegistrationID : PX.Data.BQL.BqlString.Field<taxRegistrationID> { }
        #endregion

        #region WithHoldingYearFrom
        [PXInt(MaxValue =999)]
        [PXUIField(DisplayName = "WithHolding Year From")]     
        public virtual int? WithHoldingYearFrom { get; set; }
        public abstract class withHoldingYearFrom : PX.Data.BQL.BqlInt.Field<withHoldingYearFrom> { }
        #endregion

        #region WithHoldingYearTo
        [PXInt(MaxValue = 999)]
        [PXUIField(DisplayName = "WithHolding Year To")]
        public virtual int? WithHoldingYearTo { get; set; }
        public abstract class withHoldingYearTo : PX.Data.BQL.BqlInt.Field<withHoldingYearTo> { }
        #endregion

        #region VidsYearFrom
        [PXInt()]
        [PXUIField(DisplayName = "Vids Year From")]
        public virtual int? VidsYearFrom { get; set; }
        public abstract class vidsYearFrom : PX.Data.BQL.BqlInt.Field<vidsYearFrom> { }
        #endregion

        #region VidsYearTo
        [PXInt()]
        [PXUIField(DisplayName = "Vids Year To")]
        public virtual int? VidsYearTo { get; set; }
        public abstract class vidsYearTo : PX.Data.BQL.BqlInt.Field<vidsYearTo> { }
        #endregion

        #region PayMonthFrom
        [PXInt()]
        [PXUIField(DisplayName = "Pay Month From ")]
        [FinStringList.Month]
        [PXUnboundDefault(typeof(FinStringList.Month.jan))]
        public virtual int? PayMonthFrom { get; set; }
        public abstract class payMonthFrom : PX.Data.BQL.BqlInt.Field<payMonthFrom> { }
        #endregion

        #region PayMonthTo
        [PXInt()]
        [PXUIField(DisplayName = "Pay Month To ")]
        [FinStringList.Month]
        [PXUnboundDefault(typeof(FinStringList.Month.dec))]
        public virtual int? PayMonthTo { get; set; }
        public abstract class payMonthTo : PX.Data.BQL.BqlInt.Field<payMonthTo> { }
        #endregion

        //Total Count
        #region SelectedCount
        [PXInt()]
        [PXUIField(DisplayName = "Selected Count",IsReadOnly = true)]
        [PXDefault]
        public virtual int? SelectedCount { get; set; }
        public abstract class selectedCount : PX.Data.BQL.BqlInt.Field<selectedCount> { }
        #endregion

        #region TotalWHTAmt ��ú�|�B
        [PXDecimal()]
        [PXUIField(DisplayName = "Total WHT Amount", IsReadOnly = true)]
        [PXDefault]
        public virtual decimal? TotalWHTAmt { get; set; }
        public abstract class totalWHTAmt : PX.Data.BQL.BqlDecimal.Field<totalWHTAmt> { }
        #endregion

        #region TotalGNHI2Amt �G�N���O�|�B
        [PXDecimal()]
        [PXUIField(DisplayName = "Total GNHI2 Amount", IsReadOnly = true)]
        [PXDefault]
        public virtual decimal? TotalGNHI2Amt { get; set; }
        public abstract class totalGNHI2Amt : PX.Data.BQL.BqlDecimal.Field<totalGNHI2Amt> { }
        #endregion

        #region TotalPayAmt ���I�`�B
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Pay Amount", IsReadOnly = true)]
        [PXDefault]
        public virtual decimal? TotalPayAmt { get; set; }
        public abstract class totalPayAmt : PX.Data.BQL.BqlDecimal.Field<totalPayAmt> { }
        #endregion
    }
    #endregion  
}