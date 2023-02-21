using System;
using PX.Data;
using NM.DAC;
using PX.Objects.CR;
using PX.Objects.PO;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using NM.Util;
using PX.Objects.AP;
using PX.Objects.EP;
using System.Collections;
using System.Collections.Generic;
using RC.Util;
using PX.Objects.GL;

namespace NM
{
    public class NMPrintableProcess : PXGraph<NMPrintableProcess>
    {
        #region Select
        public PXCancel<NMPrintableFilter> Cancel;
        public PXFilter<NMPrintableFilter> Filters;
        /*public PXSelectJoin<NMPayableCheck,
                InnerJoin<NMBankAccount, On<NMBankAccount.bankAccountID,
                    Equal<NMPayableCheck.bankAccountID>>>,
                Where<NMBankAccount.isCheckPrintAble, Equal<True>,
                    And2<Where<NMPayableCheck.printCount, Equal<NMStringList.zero>,
                        Or<NMPayableCheck.printCount,IsNull>>,
                        And<Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.cash>,
                Or<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>>>>>>> DetailViews;*/
        public  PXSelectJoin<NMPayableCheck,
                 InnerJoin<NMBankAccount, On<NMBankAccount.bankAccountID,
                     Equal<NMPayableCheck.bankAccountID>>>,
                 Where2<Where<
                     NMPayableCheck.printCount, Equal<NMStringList.zero>,
                         Or<NMPayableCheck.printCount, IsNull>>,
                         And<Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.cash>,
                 Or<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>>>>>> DetailViews;
        protected virtual IEnumerable detailViews()
        {
            NMPrintableFilter filter = Filters.Current;
            PXSelectJoin<NMPayableCheck,
                 InnerJoin<NMBankAccount, On<NMBankAccount.bankAccountID,
                     Equal<NMPayableCheck.bankAccountID>>>,
                 Where2<Where<
                     NMPayableCheck.printCount, Equal<NMStringList.zero>,
                         Or<NMPayableCheck.printCount, IsNull>>,
                         And<Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.cash>,
                 Or<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>>>>>> query =
            new PXSelectJoin<NMPayableCheck,
                 InnerJoin<NMBankAccount, On<NMBankAccount.bankAccountID,
                     Equal<NMPayableCheck.bankAccountID>>>,
                 Where2<Where<
                     NMPayableCheck.printCount, Equal<NMStringList.zero>,
                         Or<NMPayableCheck.printCount, IsNull>>,
                         And<Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.cash>,
                 Or<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>>>>>>(this);

            //query

            if (filter.PayableCashierID != null)
                query.WhereAnd<Where<NMPayableCheck.payableCashierID,
                    Equal<Current<NMPrintableFilter.payableCashierID>>>>();
            if (filter.BankAccountID != null)
                query.WhereAnd<Where<NMPayableCheck.bankAccountID,
                    Equal<Current<NMPrintableFilter.bankAccountID>>>>();
            if (filter.CheckNbrFrom != null)
                query.WhereAnd<Where<NMPayableCheck.checkNbr,
                    GreaterEqual<Current<NMPrintableFilter.checkNbrFrom>>>>();
            if (filter.CheckNbrTo != null)
                query.WhereAnd<Where<NMPayableCheck.checkNbr,
                    LessEqual<Current<NMPrintableFilter.checkNbrTo>>>>();
            if (filter.CheckDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.checkDate,
                    GreaterEqual<Current<NMPrintableFilter.checkDateFrom>>>>();
            if (filter.CheckDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.checkDate,
                    LessEqual<Current<NMPrintableFilter.checkDateTo>>>>();
            if (filter.DueDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.dueDate,
                    GreaterEqual<Current<NMPrintableFilter.dueDateFrom>>>>();
            if (filter.DueDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.dueDate,
                    LessEqual<Current<NMPrintableFilter.dueDateTo>>>>();
            if (filter.EtdDepositDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.etdDepositDate,
                    GreaterEqual<Current<NMPrintableFilter.etdDepositDateFrom>>>>();
            if (filter.EtdDepositDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.etdDepositDate,
                    LessEqual<Current<NMPrintableFilter.etdDepositDateTo>>>>();
            if (filter.VendorID != null)
                query.WhereAnd<Where<NMPayableCheck.vendorID,
                    Equal<Current<NMPrintableFilter.vendorID>>>>();
            if (filter.VendorLocationID != null)
                query.WhereAnd<Where<NMPayableCheck.vendorLocationID,
                    Equal<Current<NMPrintableFilter.vendorLocationID>>>>();
            if (filter.ProjectID != null)
                query.WhereAnd<Where<NMPayableCheck.projectID,
                    Equal<Current<NMPrintableFilter.projectID>>>>();
            if (filter.ProjectPeriod != null)
                query.WhereAnd<Where<NMPayableCheck.projectPeriod,
                    Equal<Current<NMPrintableFilter.projectPeriod>>>>();

            //return query.Select();
            //設Default
           foreach (NMPayableCheck payableCheck in query.Select())
            {
                if (payableCheck.Receiver == null)
                {
                    Contact contact = GetContact(payableCheck.VendorID);
                    if (contact != null)
                        payableCheck.Receiver = contact.Attention ?? "";
                }
                //if (payableCheck.RefNbr != null)
                //{
                //    APInvoice invoice = GetInvoice(payableCheck.RefNbr);
                //    if (invoice != null)
                //        payableCheck.APInvBatchNbr = invoice.BatchNbr ?? null;
                //}
                yield return payableCheck;
            }


        }
        #endregion

        #region Event
        protected virtual void NMPayableCheck_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMPayableCheck row = (NMPayableCheck)e.Row;
            if (row == null) return;
            setReadOnly();
        }
        #endregion

        #region Action

        #region 列印支票
        public PXAction<NMPrintableFilter> PrintCheck;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "列印支票")]
        protected IEnumerable printCheck(PXAdapter adapter)
        {
            NMPayableCheck payableCheck = DetailViews.Current;
            if (payableCheck == null) return adapter.Get();
            
            PXLongOperation.StartOperation(this, PrintMethod) ;
            
            return adapter.Get();
        }

        #endregion


        #endregion

        #region Method
        private Contact GetContact(int? vendorID)
        {
            return PXSelect<Contact,
                Where<Contact.bAccountID, Equal<Required<Contact.bAccountID>>>>
                .Select(this, vendorID);
        }
        private APInvoice GetInvoice(string RefNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, RefNbr);
        }

        public void setReadOnly()
        {
            NMPayableCheck row = DetailViews.Current;
            PXUIFieldAttribute.SetReadOnly(DetailViews.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.selected>(DetailViews.Cache, row, false);
            DetailViews.AllowDelete = false;
            DetailViews.AllowInsert = false;
        }


        public  void PrintMethod()
        {
            string printBatchNbr = getPrintBatchNbr();
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach(NMPayableCheck check in DetailViews.Select())
                {
                    if(check.Selected==true)
                    {
                        //check.Status = NMStringList.NMAPCheckStatus.PRINT;
                        check.PrintDate = this.Accessinfo.BusinessDate;
                        check.PrintUser = this.Accessinfo.UserID;
                        check.PrintCount = (check.PrintCount ?? 0) + 1;
                        check.PrintBatchNbr = printBatchNbr;
                        check.PrintChineseAmt = GetChineseNum("單價", 
                            string.Format("{0:###0.#}", check.OriCuryAmount ?? 0));
                        DetailViews.Update(check);
                    }
                }
                base.Persist();
                ts.Complete();
            }
            NMPrintableProcess pp = CreateInstance<NMPrintableProcess>();
            NewPageToDisplayReport(pp,printBatchNbr);

        }

        
        protected virtual void NewPageToDisplayReport(NMPrintableProcess pp,string printBatchNbr)
        {
            NMPayableCheck check = DetailViews.Current;
            if (check == null) return;

            Dictionary<string, string> mailParams = new Dictionary<string, string>()
            {
                ["PrintBatchNbr"] = printBatchNbr
            };
            var requiredException = new PXReportRequiredException(mailParams, "NM600200", PXBaseRedirectException.WindowMode.New, "支票列印");
            requiredException.SeparateWindows = true;
            throw new PXRedirectWithReportException(pp, requiredException, "Preview");
        }

        //Get Print Nbr For Report Use
        public string getPrintBatchNbr()
        {
            return this.Accessinfo.UserName
                         + System.DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        //數字轉成中文大寫
        /// <summary>
        /// 數字轉大寫
        /// </summary>
        /// <param name="type">單價/數量</param>
        /// <param name="Num">數字</param>
        /// <returns></returns>
        public static string GetChineseNum(string type, string Num)
        {
            #region
            try
            {
                string m_1, m_2, m_3, m_4, m_5, m_6, m_7, m_8, m_9;
                m_1 = Num;
                string numNum = "0123456789.";
                string numChina = "零壹貳叁肆伍陸柒捌玖點";
                string numChinaWeigh = "個拾佰仟萬拾佰仟億拾佰仟萬";
                if (Num.Substring(0, 1) == "0")//0123-->123
                    Num = Num.Substring(1, Num.Length - 1);
                /*if (!Num.Contains(‘.‘))
                    Num += ".00";
                else//123.234  123.23 123.2
                    Num = Num.Substring(0, Num.IndexOf(‘.‘) + 1 + (Num.Split(‘.‘)[1].Length > 2 ? 3 : Num.Split(‘.‘)[1].Length));*/
                m_1 = Num;
                m_2 = m_1;
                m_3 = m_4 = "";
                //m_2:1234-> 壹貳叁肆
                for (int i = 0; i < 11; i++)
                {
                    m_2 = m_2.Replace(numNum.Substring(i, 1), numChina.Substring(i, 1));
                }
                //m_3:佰拾萬仟佰拾個
                int iLen = m_1.Length;
                if (m_1.IndexOf(".") > 0)
                    iLen = m_1.IndexOf(".");//獲取整數位數
                for (int j = iLen; j >= 1; j--)
                    m_3 += numChinaWeigh.Substring(j - 1, 1);
                //m_4:2行+3行
                for (int i = 0; i < m_3.Length; i++)
                    m_4 += m_2.Substring(i, 1) + m_3.Substring(i, 1);
                //m_5:4行去"0"後拾佰仟
                m_5 = m_4;
                m_5 = m_5.Replace("零拾", "零");
                m_5 = m_5.Replace("零佰", "零");
                m_5 = m_5.Replace("零仟", "零");
                //m_6:00-> 0,000-> 0
                m_6 = m_5;
                for (int i = 0; i < iLen; i++)
                    m_6 = m_6.Replace("零零", "零");
                //m_7:6行去億,萬,個位"0"
                m_7 = m_6;
                m_7 = m_7.Replace("億零萬零", "億零");
                m_7 = m_7.Replace("億零萬", "億零");
                m_7 = m_7.Replace("零億", "億");
                m_7 = m_7.Replace("零萬", "萬");
                if (m_7.Length > 2)
                    m_7 = m_7.Replace("零個", "個");
                //m_8:7行+2行小數-> 數目
                m_8 = m_7;
                m_8 = m_8.Replace("個", "");
                //if (m_2.Substring(m_2.Length - 3, 3) != "點零零")
                //    m_8 += m_2.Substring(m_2.Length - 3, 3);
                //m_9:7行+2行小數-> 價格
                m_9 = m_7;
                m_9 = m_9.Replace("個", "元");
                /*if (m_2.Substring(m_2.Length - 3, 3) != "點零零")
                {
                    m_9 += m_2.Substring(m_2.Length - 2, 2);
                    m_9 = m_9.Insert(m_9.Length - 1, "角");
                    m_9 += "分";
                }
                else*/
                m_9 += "整";
                if (m_9 != "零元整")
                    m_9 = m_9.Replace("零元", "");
                // m_9 = m_9.Replace("零分", "整");
                if (type == "數量")
                    return m_8;
                else
                    return m_9;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            #endregion
        }
        #endregion

        #region HyperLink

        #region PayableCD
        public PXAction<NMPayableCheck> ViewPayableCD;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewPayableCD()
        {
            NMPayableCheck check = (NMPayableCheck)PXSelectorAttribute.Select<NMPayableCheck.payableCheckCD>
                (DetailViews.Cache, DetailViews.Current);
            new HyperLinkUtil<NMApCheckEntry>(check, true);
        }
        #endregion

        #region APReleaseGLBatchNbr
        public PXAction<NMPayableCheck> ViewAPReleaseGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPReleaseGL()
        {

            Batch batch = (Batch)PXSelectorAttribute.Select<NMPayableCheck.aPInvBatchNbr>
                (DetailViews.Cache, DetailViews.Current);
            new HyperLinkUtil<JournalEntry>(batch, true);
        }
        #endregion

        #endregion

        [Serializable]
        public class NMPrintableFilter : IBqlTable
        {
            //For Search

            //PayableCashier
            #region PayableCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Payable Cashier ID")]
            [PXEPEmployeeSelector]
            public virtual int? PayableCashierID { get; set; }
            public abstract class payableCashierID : PX.Data.BQL.BqlInt.Field<payableCashierID> { }
            #endregion

            //CheckNbr From To
            #region CheckNbrFrom
            [PXString(12, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Check Nbr From")]
            public virtual string CheckNbrFrom { get; set; }
            public abstract class checkNbrFrom : PX.Data.BQL.BqlString.Field<checkNbrFrom> { }
            #endregion

            #region CheckNbrFrom
            [PXString(12, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Check Nbr To")]
            public virtual string CheckNbrTo { get; set; }
            public abstract class checkNbrTo : PX.Data.BQL.BqlString.Field<checkNbrTo> { }
            #endregion

            //Check Date From To
            #region CheckDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Check Date From")]
            public virtual DateTime? CheckDateFrom { get; set; }
            public abstract class checkDateFrom : PX.Data.BQL.BqlDateTime.Field<checkDateFrom> { }
            #endregion

            #region CheckDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Check Date To")]
            public virtual DateTime? CheckDateTo { get; set; }
            public abstract class checkDateTo : PX.Data.BQL.BqlDateTime.Field<checkDateTo> { }
            #endregion

            //Due Date
            #region DueDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Due Date From")]
            public virtual DateTime? DueDateFrom { get; set; }
            public abstract class dueDateFrom : PX.Data.BQL.BqlDateTime.Field<dueDateFrom> { }
            #endregion

            #region DueDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Due Date To")]
            public virtual DateTime? DueDateTo { get; set; }
            public abstract class dueDateTo : PX.Data.BQL.BqlDateTime.Field<dueDateTo> { }
            #endregion

            //Etd Deposit Date
            #region EtdDepositDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Etd Deposit Date From")]
            public virtual DateTime? EtdDepositDateFrom { get; set; }
            public abstract class etdDepositDateFrom : PX.Data.BQL.BqlDateTime.Field<etdDepositDateFrom> { }
            #endregion

            #region EtdDepositDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Etd Deposit Date To")]
            public virtual DateTime? EtdDepositDateTo { get; set; }
            public abstract class etdDepositDateTo : PX.Data.BQL.BqlDateTime.Field<etdDepositDateTo> { }
            #endregion

            //Vendor ID
            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor ID")]
            [POVendor(Visibility = PXUIVisibility.SelectorVisible,
                DescriptionField = typeof(Vendor.acctName),
                CacheGlobal = true, Filterable = true)]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

            //VendorLocationID
            #region VendorLocationID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor Location ID")]
            [PXSelector(typeof(Search<LocationExtAddress.locationID,
                Where<LocationExtAddress.bAccountID, Equal<Current<vendorID>>>>),
                typeof(LocationExtAddress.locationCD),
                SubstituteKey = typeof(LocationExtAddress.addressLine1))]
            public virtual int? VendorLocationID { get; set; }
            public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
            #endregion

            //ProjectID
            #region ProjectID
            [PXInt()]
            [PXUIField(DisplayName = "Project ID")]
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region ProjectPeriod
            [PXInt()]
            [PXUIField(DisplayName = "Project Period")]
            public virtual int? ProjectPeriod { get; set; }
            public abstract class projectPeriod : PX.Data.BQL.BqlInt.Field<projectPeriod> { }
            #endregion

            //Bank Account ID
            #region BankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Bank Account ID")]
            [NMBankAccount(SubstituteKey = typeof(NMBankAccount.bankAccountCD))]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

        }


    }
}