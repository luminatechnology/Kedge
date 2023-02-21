using System;
using PX.Data;
using NM.DAC;
using PX.Objects.CR;
using PX.Objects.EP;
using System.Collections;
using PX.Objects.AR;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using RC.Util;
using NM.Util;
using KG.Util;

namespace NM
{
    /**
     * ===2021/03/31 Mantis : 0011988 === Althea
     * details�u��ܪ��A��PendingRefund
     **/
    public class NMArCheckModifyProcess : PXGraph<NMArCheckModifyProcess>
    {
        //�v���]�w�}��
        public NMArCheckModifyProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_RECEIVABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        #region ToolBar Action
        //public PXSave<NMModifyCheckFilter> Save;
        public PXCancel<NMModifyCheckFilter> Cancel;

        #region �M��
        public PXAction<NMModifyCheckFilter> WithdrawProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "�M���i�Ŀ�h���䲼")]
        [PXUIField(DisplayName = "�M/�h��")]
        protected IEnumerable withdrawProcessAction(PXAdapter adapter)
        {
            NMReceivableCheck receivableCheck = CheckDetails.Current;
            NMModifyCheckFilter filter = CheckFilters.Current;
            PXResultset<NMReceivableCheck> set =
                PXSelect<NMReceivableCheck,
            Where< NMReceivableCheck.status, Equal < NMStringList.NMARCheckStatus.pendingrefund >,
                 And <NMReceivableCheck.selected, Equal<True>>>>.Select(this);
            if (set.Count == 0)
                throw new Exception("�ФĿ�̤֤@�����!");
            checkRequired();

            string dialogMsg = "�������I����\r\n";
            dialogMsg += "���ʤ��:{0}\r\n";
            dialogMsg += "���ʵ���:{1}��\r\n";
            dialogMsg += "���ʲ{���B:{2:#,0.##}��";
            dialogMsg = String.Format(
                dialogMsg,
                filter.ReverseDate?.ToString("yyyy/MM/dd"),
                filter.SelectedCount,
                filter.SelectedOriCuryAmount);
            if (CheckFilters.Ask("�T�{���ʸ�T", dialogMsg, MessageButtons.YesNo) == WebDialogResult.Yes)
            {
                ModifyMethod(NMStringList.NMARCheckStatus.WITHDRAW);
            }
                
            return adapter.Get();
        }
        #endregion

        /*#region �h��
        public PXAction<NMModifyCheckFilter> ReturnProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "�h���i�Ŀ�h���䲼")]
        [PXUIField(DisplayName = "�h��")]
        protected IEnumerable returnProcessAction(PXAdapter adapter)
        {
            NMReceivableCheck receivableCheck = CheckDetails.Current;
            PXResultset<NMReceivableCheck> set =
                 PXSelect<NMReceivableCheck,
                 Where2<Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>,
                 Or<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.cash>>>,
                 And<NMReceivableCheck.selected, Equal<True>>>>.Select(this);
            if (set.Count == 0)
                throw new Exception("�ФĿ�̤֤@�����!");
            checkRequired();
            ModifyMethod(NMStringList.NMARCheckStatus.REFUND);
            return adapter.Get();
        }
        #endregion*/

        #endregion

        #region  Select and Filter
        public PXFilter<NMModifyCheckFilter> CheckFilters;

        //2021/03/31 Modify
        //��אּ�u��ܫݺM�h
        public PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.pendingrefund>>> CheckDetails;
        /*
        //�u���w�����M�I�{�i�H��ܦb�e��
        public PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>,
                Or<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.cash>>>> CheckDetails;
        */

        protected virtual IEnumerable checkDetails()
        {
            NMModifyCheckFilter filter = CheckFilters.Current;
            //2021/05/06 Modify
            //��אּ�u��ܫݺM�h
            PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.pendingrefund>>> query =
            new PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.pendingrefund>>>(this);

            //query
            if (filter.CustomerID != null)
                query.WhereAnd<Where<NMReceivableCheck.customerID,
                    Equal<Current<NMModifyCheckFilter.customerID>>>>();
            if (filter.ProjectID != null)
                query.WhereAnd<Where<NMReceivableCheck.projectID,
                    Equal<Current<NMModifyCheckFilter.projectID>>>>();
            if (filter.CheckNbr != null)
                query.WhereAnd<Where<NMReceivableCheck.checkNbr,
                    Equal<Current<NMModifyCheckFilter.checkNbr>>>>();


            return query.Select();
        }

        #endregion

        #region Event
        protected virtual void NMReceivableCheck_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            if (row == null) return;
            setReadOnly();
        }

        protected virtual void NMReceivableCheck_Selected_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            NMModifyCheckFilter header = CheckFilters.Current;
            if (row == null) return;

            decimal? selectbaseAmount = 0;
            decimal? selectoriAmount = 0;
            int? selectcount = 0;

            PXResultset<NMReceivableCheck> set =
                PXSelect<NMReceivableCheck,
                Where<NMReceivableCheck.selected, Equal<True>,
                    And<Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.receive>,
                Or<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>,
                Or<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.cash>>>>>>>
                .Select(this);
            foreach (NMReceivableCheck receivableCheck in set)
            {
                selectoriAmount = receivableCheck.OriCuryAmount + selectoriAmount;
                selectbaseAmount = receivableCheck.OriCuryAmount + selectbaseAmount;
            }
            selectcount = set.Count;

            header.SelectedOriCuryAmount = selectoriAmount;
            header.SelectedBaseCuryAmount = selectbaseAmount;
            header.SelectedCount = selectcount;



        }

        #endregion

        #region Method
        public void setReadOnly()
        {
            NMReceivableCheck row = CheckDetails.Current;
            PXUIFieldAttribute.SetReadOnly(CheckDetails.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMReceivableCheck.selected>(CheckDetails.Cache, row, false);
            CheckDetails.AllowDelete = false;
            CheckDetails.AllowInsert = false;
        }

        public void checkRequired()
        {
            NMReceivableCheck receivableCheck = CheckDetails.Current;
            NMModifyCheckFilter filter = CheckFilters.Current;
            if (receivableCheck != null)
            {
                if (filter.ModifyCashierID == null || filter.ReverseDate == null ||
                    filter.Reason == null)
                {
                    string errormsg = "�ж񧴥H�U���!";
                    if (filter.ModifyCashierID == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMModifyCheckFilter.modifyCashierID>(
                            filter, filter.ModifyCashierID, new PXSetPropertyException("����쬰����!", PXErrorLevel.Error));
                        //errormsg = errormsg + "���ʧ@�~�H/";
                    }
                    if (filter.ReverseDate == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMModifyCheckFilter.reverseDate>(
                            filter, filter.ReverseDate, new PXSetPropertyException("����쬰����!", PXErrorLevel.Error));
                        // errormsg = errormsg + "���ʤ��/";
                    }
                    if (filter.Reason == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMModifyCheckFilter.reason>(
                            filter, filter.Reason, new PXSetPropertyException("����쬰����!", PXErrorLevel.Error));
                        //errormsg = errormsg + "���ʭ�]";
                    }
                    throw new Exception(errormsg);
                }
            }
        }

        public void ModifyMethod(int ModifyStatus)
        {
            NMModifyCheckFilter filter = CheckFilters.Current;
            PXLongOperation.StartOperation(this, delegate ()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    foreach (NMReceivableCheck check in CheckDetails.Select())
                    {
                        if (check.Selected == true)
                        {
                            //�H�U��T��s���
                            check.ModifyCashierID = filter.ModifyCashierID;
                            check.ReverseDate = filter.ReverseDate;
                            check.Reason = filter.Reason;
                            /**
                            switch (check.Status)
                            {
                                case NMStringList.NMARCheckStatus.COLLECTION:
                                    check.CollReverseBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.COLLREVERSE, check, GLStageCode.NMRVoidRA);                                    
                                    break;

                                case NMStringList.NMARCheckStatus.CASH:
                                    check.CashReverseBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.CASHREVERSE, check, GLStageCode.NMRVoidRE);
                                    check.CollReverseBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.COLLREVERSE, check, GLStageCode.NMRVoidRA);
                                    break;
                            }
                            switch (ModifyStatus)
                            {
                                case NMStringList.NMARCheckStatus.WITHDRAW:
                                    //���A�אּ�w�M��
                                    check.Status = 4;
                                    break;

                                case NMStringList.NMARCheckStatus.REFUND:

                                    //���A�אּ�w�h��
                                    check.Status = 5;
                                    break;
                            }**/
                            //20220907 louis �]���O�ΥӽкM�h���ӽз|�ק��������ڪ��A���ݹ���
                            if (check.CollBatchNbr!=null) {
                                check.CollReverseBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.COLLREVERSE, check, GLStageCode.NMRVoidRA);
                            }
                            if (check.CashBatchNbr!=null) {
                                check.CashReverseBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.CASHREVERSE, check, GLStageCode.NMRVoidRE);
                            }
                            check.Status = 4;
                            CheckDetails.Update(check);
                            base.Persist();
                            VoidArPaymant(check.PayRefNbr);
                        }
                    }

                    ts.Complete();
                }
            });

        }

        public void VoidArPaymant(string payRefnbr)
        {
            if (payRefnbr == null) return; 
            ARPaymentEntry entry = PXGraph.CreateInstance<ARPaymentEntry>();
            entry.Document.Current = entry.Document.Search<ARPayment.refNbr>(payRefnbr);
            ARPayment arPayment = entry.Document.Current;
            if (arPayment == null) return;
            entry.voidCheck.Press();
            entry.Save.Press();
            //�@�o���٭n�L�b
            arPayment = entry.Document.Current;
            arPayment.Hold = false;
            entry.Document.Update(arPayment);
            entry.release.Press();
        }

        #endregion

        [Serializable]
        public class NMModifyCheckFilter : IBqlTable
        {
            //For Search
            #region CustomerID
            [PXInt()]
            [PXUIField(DisplayName = "Customer ID")]
            [CustomerActive(Visibility = PXUIVisibility.SelectorVisible,
                DescriptionField = typeof(Customer.acctName),
                Filterable = true, TabOrder = 2)]
            public virtual int? CustomerID { get; set; }
            public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
            #endregion

            #region ProjectID
            [PXInt()]
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            [PXUIField(DisplayName = "Project ID")]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region CheckNbr
            [PXString(10, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Check Nbr")]
            [PXSelector(typeof(Search<NMReceivableCheck.checkNbr>),
                typeof(NMReceivableCheck.oriBankAccount),
                typeof(NMReceivableCheck.customerID),
                typeof(NMReceivableCheck.etdDepositDate))]
            public virtual string CheckNbr { get; set; }
            public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
            #endregion

            //For Insert Modify Data
            #region ModifyCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Modify Cashier ID", Required = true)]
            [PXEPEmployeeSelector]
            [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
            public virtual int? ModifyCashierID { get; set; }
            public abstract class modifyCashierID : PX.Data.BQL.BqlInt.Field<modifyCashierID> { }
            #endregion

            #region ReverseDate
            [PXDate()]
            [PXUIField(DisplayName = "Reverse Date", Required = true)]
            [PXDefault(typeof(AccessInfo.businessDate))]
            public virtual DateTime? ReverseDate { get; set; }
            public abstract class reverseDate : PX.Data.BQL.BqlDateTime.Field<reverseDate> { }
            #endregion

            #region Reason
            [PXString(255, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Reason", Required = true)]
            public virtual string Reason { get; set; }
            public abstract class reason : PX.Data.BQL.BqlString.Field<reason> { }
            #endregion

            //For Select Cal
            #region SelectedCount
            [PXInt()]
            [PXUIField(DisplayName = "Selected Count", IsReadOnly = true)]
            public virtual int? SelectedCount { get; set; }
            public abstract class selectedCount : PX.Data.BQL.BqlInt.Field<selectedCount> { }
            #endregion

            #region SelectedOriCuryAmount
            [PXDecimal()]
            [PXUIField(DisplayName = "Selected Ori Cury Amount", IsReadOnly = true)]
            public virtual Decimal? SelectedOriCuryAmount { get; set; }
            public abstract class selectedOriCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedOriCuryAmount> { }
            #endregion

            #region SelectedBaseCuryAmount
            [PXDecimal()]
            [PXUIField(DisplayName = "Selected Base Cury Amount", IsReadOnly = true)]
            public virtual Decimal? SelectedBaseCuryAmount { get; set; }
            public abstract class selectedBaseCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedBaseCuryAmount> { }
            #endregion
        }

    }
}