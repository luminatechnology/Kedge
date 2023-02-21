
using PX.Data;
using Kedge.DAC;
using PX.Objects.CR;
using NM.DAC;
using NM.Util;
using System.Collections;
using PX.Objects.AP;
using System;
using PX.Objects.FS;

namespace PX.Objects.PO
{
    /**
     * ===2021/05/11 :口頭===Althea
     * VendorLocation Selector Add LocationFinExt.usrIsReserveAcct to Show
     * 
     * ===2021/05/20 :0012046 ===Althea
     * KGBillPayment.VendorLocationID LOV畫面請將UsrIsReserveAcct, 
     * 請放在第一個欄位.LOV Order by UsrIsReserveAcct(降冪), LocationCD(升冪), 讓有備償戶的LocationLID在第一筆
     * 
     * ===2021/06/24 : 0012105 === Althea
     * PaymentMethod Add E:Auth(授扣)
     * VendorLocation & BankAccountID Selector Add Filter :Same withe Cash
     * Default Auth same with Cash
     * 
     * ===2021/08/02 :0012176 === Althea
     * PaymentMethod Add F : TempWriteoff (暫付沖銷) same with Cahs
     **/
    public class POOrderEntryExt2 : PXGraphExtension<POOrderEntry_Extension, POOrderEntry>
    {
        #region Event Handlers
        protected virtual void KGPoOrderPayment_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            if (row == null) return;
            //改為用BankAccountID欄位去連動資訊
            if (row.BankAccountID != null)
            {
                NMBankAccount bankAccount = GetBankAccount(row.BankAccountID);
                row.BankAccount = bankAccount?.BankAccount;
                row.BankName = bankAccount?.BankName;
            }
            else
            {
                row.BankAccount = row.BankName = null;
            }
        }
        protected virtual void KGPoOrderPayment_PaymentMethod_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            POOrder order = Base.Document.Current;
            if (row == null) return;
            if (row.PaymentMethod == null)
            {
                cache.SetValue<KGPoOrderPayment.paymentMethod>(row, null);
            }
            if ((string)e.OldValue != row.PaymentMethod)
            {
                if (row.PaymentMethod == PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
                {
                    BAccount baccount = GetBAccount(order.VendorID);
                    cache.SetValueExt<KGPoOrderPayment.vendorLocationID>(row, baccount?.DefLocationID);
                }
                else
                {
                    cache.SetValueExt<KGPoOrderPayment.vendorLocationID>(row,
                        NMLocationUtil.GetDefLocationByPaymentMethod(order.VendorID, row.PaymentMethod));
                }
            }

        }
        protected virtual void KGPoOrderPayment_VendorLocationID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            POOrder order = Base.Document.Current;
            if (row == null) return;
            if (order.VendorID != null)
            {
                if (row.PaymentMethod == PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
                {
                    BAccount baccount = GetBAccount(order.VendorID);
                    cache.SetValueExt<KGPoOrderPayment.vendorLocationID>(row, baccount?.DefLocationID);
                }
                else
                {
                    cache.SetValueExt<KGPoOrderPayment.vendorLocationID>(row,
                        NMLocationUtil.GetDefLocationByPaymentMethod(order.VendorID, row.PaymentMethod));
                }
            }
        }
        protected virtual void KGPoOrderPayment_VendorLocationID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            if (row == null) return;
            if (row.VendorLocationID != null)
            {
                Location location = GetLocation(row.VendorLocationID);
                if (location.VCashAccountID == null) return;
                string PaymentMethodID = "";
                if (row.PaymentMethod == PaymentMethod.A || row.PaymentMethod == PaymentMethod.D)
                {
                    PaymentMethodID = "CHECK";
                }
                else if (row.PaymentMethod == PaymentMethod.B)
                {
                    PaymentMethodID = "TT";
                }
                else if(row.PaymentMethod== PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
                {
                    PaymentMethodID = location.PaymentMethodID;
                }

                NMBankAccount account = GetBankAccount(location.VCashAccountID, PaymentMethodID);
                if (account == null) return;
                cache.SetValueExt<KGPoOrderPayment.bankAccountID>(row, account?.BankAccountID);

            }
            else
            {
                cache.SetValueExt<KGPoOrderPayment.bankAccountID>(row, null);

            }
        }
        protected virtual void KGPoOrderPayment_BankAccountID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            if (row == null) return;
            if (row.VendorLocationID != null)
            {
                Location location = GetLocation(row.VendorLocationID);
                if (location.VCashAccountID == null) return;
                string PaymentMethodID = "";
                if (row.PaymentMethod == PaymentMethod.A || row.PaymentMethod == PaymentMethod.D)
                {
                    PaymentMethodID = "CHECK";
                }
                else if (row.PaymentMethod == PaymentMethod.B)
                {
                    PaymentMethodID = "TT";
                }
                else if(row.PaymentMethod == PaymentMethod.C || row.PaymentMethod == PaymentMethod.E)
                {
                    PaymentMethodID = location.PaymentMethodID;
                }

                NMBankAccount account = GetBankAccount(location.VCashAccountID, PaymentMethodID);             
                e.NewValue = account?.BankAccountID;
            }
            else
                e.NewValue = null;

        }
        protected virtual void KGPoOrderPayment_BankAccountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            KGPoOrderPayment row = (KGPoOrderPayment)e.Row;
            //帶出對應的NMBankAccount資訊
            if (row.BankAccountID != null)
            {
                NMBankAccount bankAccount = GetBankAccount(row.BankAccountID);
                row.BankAccount = bankAccount?.BankAccount;
                row.BankName = bankAccount?.BankName;
            }
            else
            {
                row.BankAccount = row.BankName = null;
            }
        }
        #endregion

        #region Override
        public delegate int? VendorLocationIDDelegate(int? BAccount, string paymentmethod);
        [PXOverride]
        public virtual int? VendorLocationID(int? BAccount, string PaymentMethod)
        {
            return NMLocationUtil.GetDefLocationByPaymentMethod(BAccount, PaymentMethod);
        }
        #endregion

        #region Method
        private BAccount2 GetBAccount(int? BAccount)
        {
            return PXSelect<BAccount2,
                Where<BAccount2.bAccountID, Equal<Required<BAccount2.bAccountID>>>>
                .Select(Base, BAccount);
        }
        private Location GetLocation(int? VendorLocationID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>>>
                .Select(Base, VendorLocationID);
        }
        private NMBankAccount GetBankAccount(int? CashAccount, string PaymentMethod)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID,Equal<Required<NMBankAccount.paymentMethodID>>>>>
                .Select(Base, CashAccount,PaymentMethod);
        }
        private NMBankAccount GetBankAccount(int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
                .Select(Base, BankAccountID);
        }
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Pricing Type", Visible = true,Enabled =false)]
        [PXDefault("A", PersistingCheck = PXPersistingCheck.Nothing)]
        [PricingType.List]
        protected void KGPoOrderPayment_PricingType_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Method", Visible = true)]
        [PXDefault("A", PersistingCheck = PXPersistingCheck.Nothing)]
        [PaymentMethod.List]
        protected void KGPoOrderPayment_PaymentMethod_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor LocationID", Visible = true)]
        [PXSelectorWithCustomOrderBy(typeof(Search2<Location.locationID,
            InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Location.bAccountID>>>,
            Where<Location.bAccountID, Equal<Current<POOrder.vendorID>>,
                And<Where2<
                    Where<Current<KGPoOrderPayment.paymentMethod>, Equal<PaymentMethod.wireTransfer>, 
                        And<Location.vPaymentMethodID, Equal<word.tt>>>,
                    Or2<Where<Current<KGPoOrderPayment.paymentMethod>, 
                        In3<PaymentMethod.check, PaymentMethod.giftCertificate>,
                        And<Location.vPaymentMethodID, Equal<word.check>>>,
                    Or<Where<Current<KGPoOrderPayment.paymentMethod>, 
                        //2021/06/24 Add Auth same with Cash
                        //2021/07/29 Add TempWriteoff same with Cash
                        In3<PaymentMethod.cash,PaymentMethod.auth>>>>
                    >>
                >,
            //2021/05/20 Add Mantis:0012046
            OrderBy<Desc<LocationFinExt.usrIsReserveAcct,
                Asc<Location.locationCD>>>>),
            //2021/05/11 Add to Show
            typeof(LocationFinExt.usrIsReserveAcct),
            typeof(Location.locationCD),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(Location.locationCD))]
        [PXDefault()]
        protected void KGPoOrderPayment_VendorLocationID_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account CD", Visible = true)]
        [PXSelector(typeof(Search<NMBankAccount.bankAccountID,
            Where2<
                Where<Current<KGPoOrderPayment.paymentMethod>, In3<PaymentMethod.check,PaymentMethod.giftCertificate>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.check>>>,
                Or2<Where<Current<KGPoOrderPayment.paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                    And<NMBankAccount.paymentMethodID, Equal<KG.Util.KGConst.tt>>>,
                //2021/06/24 Add Auth same with Cash
                //2021/08/02 Add TempWriteoff same with Cash
                Or<Where<Current<KGPoOrderPayment.paymentMethod>, In3<PaymentMethod.cash,PaymentMethod.auth>>>>>>),
            typeof(NMBankAccount.bankAccountCD),
            typeof(NMBankAccount.bankCode),
            typeof(NMBankAccount.bankName),
            DescriptionField =typeof(NMBankAccount.bankName),
            SubstituteKey = typeof(NMBankAccount.bankAccountCD))]
        [PXUIEnabled(typeof(Where<Current<POOrder.hold>,Equal<True>>))]
        protected void KGPoOrderPayment_BankAccountID_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Bank Name", Visible = true)]
        protected void KGPoOrderPayment_BankName_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Bank Account", Visible = true)]
        protected void KGPoOrderPayment_BankAccount_CacheAttached(PXCache sender)
        {
        }
        #endregion
    }
}