using System;
using System.Collections;
using PX.Data;
using NM.DAC;
using NM.Util;
using RC.Util;
using PX.Objects.PM;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.PO;
using PX.Objects.CS;
using CC.DAC;
using CC.Util;
using static CC.Util.CCList;

namespace CC
{
    /* ======2021/02/09 0011925 Edit By Althea=====
     * 1.CCPayableCheck.Status='AppliedRTN'�ɡA�бN�Ҧ�����ܦ�read-only
     * ======2021/02/26 0011961  Edit By Althea=====
     * 1.Add Action: Return(�h�^)
     *      1.1 ���A�令Retuned
     * 2.Update ���A��Retuned�ɤ~�i�H���@�o
     * 3.Update �֥i��,��guarNbr���^ExpenseRefNbr
     * 
     * ===2021/03/31 Mantis : 0011988 === Althea 
     * Add Action : ModifyAction
     * ���A��PendingModify�ɥi�H�ϥ�
     * ���U���s��惡�i���ڪ��A = balanced
     * 
     * ===2021-05-17:�f�Y===Alton
     * 1.�Ȧ楻�����z�LbookCD����
     * 2.����GuarNbr�אּ����
     * 
     * ===2021-06-07:12081===Alton
     * 	1.�� CCPayableCheck. GuarClass='�Ȧ楻��'�ɡA
     * 	  �Х�CCPayableCheck. BankAccountID������NMCheckBook. BankAccountID���A
     * 	  NMCheckBook. BookUsage='�Ȧ楻��'���䲼ï �i�����
     * 	2.�P'�u�ӥ���'�A�ЦbCC���I�O�Ҳ����֥i���ɨ���
     * 	3.�Y�b���U'�֥i'�ɡA�䤣��ӻȦ�b���������䲼ï���A ��pop-up���ܵ����G�u�Ш�NM�䲼ï�D�ɫإߡu�Ȧ楻���v�ϥΪ��䲼ï�v
     * 	4.�p�G�����O�Ȧ楻���A���ˬd�p�G�Ȧ�b��²�X=null,�Цh���@��msg: �п�ܭn�}�߻Ȧ楻�����Ȧ�b��²�X
     * 	
     * 	===2022-03-09=== Jeff
     * 	Mantis[0012287] EP301000 �O�ΥӽС����I�O�Ҳ����L�b��, ���ͪ����I�O�Ҳ����A�אּ"���m"
     * 	
     * 	===2022-03-30=== Jeff
     * 	Mantis [0012302] CC���I�O�Ҳ��]CC302001�^�t�X�ǲ��y�{�վ�
     */
    public class CCPayableEntry : PXGraph<CCPayableEntry, CCPayableCheck>
    {
        #region Ctor
        public CCPayableEntry()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.CC_PAYABLE_CHECK))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(AccountConfirm);
            this.ActionMenu.AddMenuAction(this.ReleaseBtn);
            this.ActionMenu.AddMenuAction(this.ReturnBtn);
            this.ActionMenu.AddMenuAction(this.VoidBtn);
        }
        #endregion

        #region Select
        public PXSelect<CCPayableCheck> PayableChecks;
        #endregion

        #region Actions
        #region Menu
        public PXAction<CCPayableCheck> ActionMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Actions")]
        protected void actionMenu() { }
        #endregion

        #region Accounting Confirmation
        public PXAction<CCPayableCheck> AccountConfirm;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Accounting Confirmation")]
        protected virtual IEnumerable accountConfirm(PXAdapter adapter)
        {
            foreach (CCPayableCheck check in adapter.Get().RowCast<CCPayableCheck>() )
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    check.GuarReleaseDate = this.Accessinfo.BusinessDate;
                    check.GuarReleaseNbr  = CCVoucherUtil.CreateAPVoucher(CCList.CCAPVoucher.RELEASE, check);

                    ModifiedMethod();
                });
            }

            return adapter.Get();
        }
        #endregion

        #region �֥i
        public PXAction<CCPayableCheck> ReleaseBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "�֥i")]
        protected IEnumerable releaseBtn(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, releasemethod);

            return adapter.Get();
        }
        #endregion

        #region �h�^
        //2021/02/25 Add Mantis:0011961
        public PXAction<CCPayableCheck> ReturnBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "�h�^")]
        protected IEnumerable returnBtn(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, returnmethod);

            return adapter.Get();
        }
        #endregion

        #region �@�o
        public PXAction<CCPayableCheck> VoidBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "�@�o")]
        protected IEnumerable voidBtn(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, voidmethod);

            return adapter.Get();
        }
        #endregion

        //2021/03/31 Add Mantis : 0011988
        #region �����ק�
        public PXAction<CCPayableCheck> ModifiedAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "�����ק�")]
        protected IEnumerable modifiedAction(PXAdapter adapter)
        {
            CCPayableCheck check = PayableChecks.Current;
            PXLongOperation.StartOperation(this, ModifiedMethod);
            return adapter.Get();
        }
        #endregion

        #endregion

        #region HyperLink

        #region GL Voucher
        public PXAction<CCPayableCheck> ViewGLVoucherByRelease;
        public PXAction<CCPayableCheck> ViewGLVoucherByVoid;

        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByRelease()
        {
            CCPayableCheck check = PayableChecks.Current;
            if (check.GuarReleaseNbr == null) return;
            LinkVoucher(check.GuarReleaseNbr);
        }

        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByVoid()
        {
            CCPayableCheck check = PayableChecks.Current;
            if (check.GuarVoidNbr == null) return;
            LinkVoucher(check.GuarVoidNbr);
        }

        #endregion

        #endregion

        #region Event
        protected virtual void _(Events.RowSelected<CCPayableCheck> e)
        {
            if (e.Row != null)
            {
                SetEnabled(e.Row);
            }
        }

        protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.targetType> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
            if (row == null) return;
            e.Cache.SetValueExt<CCPayableCheck.customerID>(row, null);
            e.Cache.SetValueExt<CCPayableCheck.vendorID>(row, null);
            if (row.TargetType == CCTargetType.Customer)
            {
                e.Cache.SetValueExt<CCPayableCheck.projectID>(row, row.ProjectID);
            }
        }

        protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.customerID> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
            if (row == null) return;
            if (row.CustomerID == null)
            {
                row.CustomerLocationID = null;
                row.GuarTitle = null;
            }
            else
            {
                Customer customer = GetCustomer(row.CustomerID);
                row.GuarTitle = customer.AcctName ?? null;
            }
        }

        protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.projectID> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
            if (row == null && row.TargetType == CCTargetType.Vendor) return;
            PMProject c = (PMProject)PXSelectorAttribute.Select<CCPayableCheck.projectID>(e.Cache, row);
            e.Cache.SetValueExt<CCPayableCheck.customerID>(row, c?.CustomerID);
        }

        protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.vendorID> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
            if (row == null) return;
            if (row.VendorID == null)
            {
                row.VendorLocationID = null;
                row.GuarTitle = null;
            }
            else
            {
                Vendor vendor = GetVendor(row.VendorID);
                row.GuarTitle = vendor.AcctName ?? null;

            }
        }

        //2021/01/22 �令���b�֥i���s
        /*protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.guarClass> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
  
            if(row.GuarClass == GuarClassList.CommercialPaper)
            {
                row.GuarNbr =
                    AutoNumberAttribute.GetNextNumber(PayableChecks.Cache, row,"COMPAPER",this.Accessinfo.BusinessDate);
            }
            else
            {
                row.GuarNbr = null;
            }
        }*/

        //2021/01/22 �令���b�֥i���s
        /*protected virtual void _(Events.RowInserting<CCPayableCheck> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;

            if (row.GuarClass == GuarClassList.CommercialPaper && row.GuarNbr ==null)
            {
               row.GuarNbr =
                    AutoNumberAttribute.GetNextNumber(PayableChecks.Cache, row, "COMPAPER", this.Accessinfo.BusinessDate);
            }
        
        }*/

        //2021/01/22 �令���b�֥i���s
        /*protected virtual void _(Events.RowPersisting<CCPayableCheck> e)
        {
            CCPayableCheck check = e.Row as CCPayableCheck;
            if(PayableChecks.Cache.GetStatus(check) == PXEntryStatus.Inserted)
            {
                if(check.GuarClass !=null &&check.GuarClass == CCList.GuarClassList.CashiersCheck)
                {
                    check.GuarNbr = NMCheckBookUtil.getCheckNbr(check.BookCD);
                }
            }
        }*/
        //2020/10/30 Althea Add Mantis:0011813
        protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.pONbr> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
            if (row == null) return;
            if (row.PONbr != null)
            {
                POOrder order = GetPOOrder(row.PONbr);
                row.POOrderType = order.OrderType;
                row.Description = order.OrderDesc;
            }
        }

        protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.bankAccountID> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
            if (row == null) return;
            NMBankAccount ba = (NMBankAccount)PXSelectorAttribute.Select<CCPayableCheck.bankAccountID>(e.Cache, row);
            row.BankCode = ba?.BankCode;
            e.Cache.SetDefaultExt<CCPayableCheck.bookCD>(row);
        }

        protected virtual void _(Events.FieldUpdated<CCPayableCheck, CCPayableCheck.contractorID> e)
        {
            CCPayableCheck row = e.Row as CCPayableCheck;
            if (row == null) return;
            if (row.ContractorID != null)
            {
                EPEmployee employee = GetPEmployee(row.ContractorID);
                row.DepID = employee.DepartmentID;
            }
        }
        #endregion

        #region Methods
        public void releasemethod()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                this.Persist();
                CCPayableCheck check = PayableChecks.Current;
                if (check == null) return;
                check.Status = CCPayableStatus.Released;

                //2021/01/22 Mantis: 0011898
                if (check.GuarClass == GuarClassList.CommercialPaper)
                {
                    check.GuarNbr = AutoNumberAttribute.GetNextNumber(PayableChecks.Cache, check, "COMPAPER", this.Accessinfo.BusinessDate);
                }
                else if (check.GuarClass == GuarClassList.CashiersCheck)
                {
                    if (check.BankAccountID == null) throw new PXException("�ж�g�Ȧ�b��²�X");
                    NMBankAccount ba = (NMBankAccount)PXSelectorAttribute.Select<CCPayableCheck.bankAccountID>(PayableChecks.Cache,check);
                    //2021-05-17 mark by Alton �Ȯɤ��z�LBookCD������
                    //2021-06-07 Edit by Alton �S�n�z�L�䲼ï���
                    if (check.BookCD == null) throw new PXException("�ж�{0}������BookNbr(�Ȧ楻��)", ba.BankAccountCD);
                    check.GuarNbr = NMCheckBookUtil.getCheckNbr(check.BookCD);
                }
                PayableChecks.Update(check);
                this.Persist();

                //2021/02/25 add Mantis:0011961
                if (check.GuarNbr != null)
                {
                    PXUpdate<Set<EPExpenseClaimDetails.expenseRefNbr, Required<CCPayableCheck.guarNbr>>,
                             EPExpenseClaimDetails,
                             Where<EPExpenseClaimDetailsExt.usrGuarPayableCD, Equal<Required<CCPayableCheck.guarPayableCD>>>>
                             .Update(this, check.GuarNbr, check.GuarPayableCD);
                }

                ts.Complete();
            }
        }

        public void voidmethod()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                this.Persist();
                CCPayableCheck check = PayableChecks.Current;
                if (check == null) return;
                check.Status = CCPayableStatus.Voided;
                check.GuarVoidDate = this.Accessinfo.BusinessDate;
                check.GuarVoidNbr = CCVoucherUtil.CreateAPVoucher(CCList.CCAPVoucher.REVERSE, check, true);
                PayableChecks.Update(check);
                this.Persist();

                ts.Complete();
            }

        }

        public void returnmethod()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                this.Persist();
                CCPayableCheck check = PayableChecks.Current;
                if (check == null) return;
                check.Status = CCPayableStatus.Returned;
                PayableChecks.Update(check);
                this.Persist();

                ts.Complete();
            }

        }

        public void SetEnabled(CCPayableCheck check)
        {
            bool isReleased      = check.Status == CCPayableStatus.Released;
            bool isReturned      = check.Status == CCPayableStatus.Returned;
            bool isAppliedRTN    = check.Status == CCPayableStatus.AppliedRTN;
            bool isBalanced      = check.Status == CCPayableStatus.Balanced;
            bool isPendingModify = check.Status == CCPayableStatus.PendingModify;
            bool isPendingRefund = check.Status == CCPayableStatus.PendingRefund;
            bool isOnHold        = check.Status == CCPayableStatus.OnHold;

            ReleaseBtn.SetEnabled(isBalanced);
            //2021/02/25 Add Mantis:0011961
            ReturnBtn.SetEnabled(isAppliedRTN);
            VoidBtn.SetEnabled(isPendingRefund || isReturned);
            //2021/03/31 Add Mantis:0011988
            ModifiedAction.SetEnabled(isPendingModify);
            // Mantis [0012302] The status is only Hold, the new button can be clicked.
            AccountConfirm.SetEnabled(isOnHold);

            //Mantis:2021/02/09 Mantis:0011925
            PXUIFieldAttribute.SetReadOnly(PayableChecks.Cache, check, true);
            PXUIFieldAttribute.SetReadOnly<CCPayableCheck.guarPayableCD>(PayableChecks.Cache, check, false);
            PXUIFieldAttribute.SetReadOnly<CCPayableCheck.bankCode>(PayableChecks.Cache, check, true);
            if (isBalanced || isPendingModify || isOnHold)
            {
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.bankAccountID>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.guarType>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.guarClass>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.bookCD>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.guarNbr>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.guarTitle>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.dueDate>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.issueDate>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.authDate>(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.description>(PayableChecks.Cache, check, false);
            }
            //2021/05/19 Phill�f�YAdd
            PXUIFieldAttribute.SetReadOnly<CCPayableCheck.cashierID>(PayableChecks.Cache, check, !(isOnHold || isBalanced));
            PXUIFieldAttribute.SetReadOnly<CCPayableCheck.guarNbr>(PayableChecks.Cache, check, check.GuarClass == GuarClassList.CashiersCheck);
            // Mantis[#0012287]-2022/03/09
            PXUIFieldAttribute.SetReadOnly<CCPayableCheck.hold>(PayableChecks.Cache, check, !(isOnHold || isBalanced) );

            PayableChecks.Cache.AllowDelete = check.Status != CCPayableStatus.Released & check.Status != CCPayableStatus.Voided;
            //2021/01/22 Take off Mantis:0011898
            /*if(isExsitEPRefNbr(check.GuarPayableCD))
            {
                PXUIFieldAttribute.SetEnabled(PayableChecks.Cache, check, false);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.dueDate>(PayableChecks.Cache, check, true);
                PXUIFieldAttribute.SetReadOnly<CCPayableCheck.description>(PayableChecks.Cache, check, true);
            }*/
        }

        /*public bool isExsitEPRefNbr(string GuarPayableCD)
        {
            if (GuarPayableCD.Contains("NEW"))
            {
                return false;
            }
            else
            {
                EPExpenseClaim claim = PXSelect<EPExpenseClaim,
              Where<EPExpenseClaimExt.usrGuarPayableCD, Equal<Required<EPExpenseClaimExt.usrGuarPayableCD>>>>
              .Select(this, GuarPayableCD);
                if (claim != null) return true;
                else return false;

            }

        }*/

        /// <summary>
        /// Link to GLVoucher
        /// </summary>
        /// <param name="batchNbr">GL Batch Nbr</param>
        private void LinkVoucher(String batchNbr)
        {
            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
            graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(batchNbr);
            if (graph.BatchModule.Current == null) return;
            throw new PXRedirectRequiredException(graph, "GL Voucher")
            {
                Mode = PXBaseRedirectException.WindowMode.NewWindow
            };
        }

        /// <summary>
        /// update status to balance
        /// </summary>
        private void ModifiedMethod()
        {
            CCPayableCheck check = PayableChecks.Current;
            check.Status = CCList.CCStatus.Balanced;
            PayableChecks.Update(check);
            base.Persist();
        }
        #endregion

        #region Select Method
        public Customer GetCustomer(int? CustomerID)
        {
            return PXSelect<Customer,
                Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>
                .Select(this, CustomerID);
        }

        public Vendor GetVendor(int? VendorID)
        {
            return PXSelect<Vendor,
                Where<Vendor.bAccountID, Equal<Required<Vendor.bAccountID>>>>
                .Select(this, VendorID);
        }

        public EPEmployee GetPEmployee(int? EmployeeID)
        {
            return PXSelect<EPEmployee,
                Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>
                .Select(this, EmployeeID);
        }

        public POOrder GetPOOrder(string POOrderNbr)
        {
            return PXSelect<POOrder,
                Where<POOrder.orderNbr, Equal<Required<CCReceivableCheck.pONbr>>>>
                .Select(this, POOrderNbr);
        }



        #endregion
    }
}