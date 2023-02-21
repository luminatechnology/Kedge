using System;
using PX.Data;
using PX.Objects.PO;
using PX.Objects.CR;

namespace Kedge.DAC
{
    /**
     * ======2021/02/08 0011942  Edit By Althea=====
     * PO301000 分包契約付款條件調整
     * 	1.付款方法的KGPoOrderPayment.PaymentPct請改成可以輸入小數點兩位
     * 	2.PaymentPct加總要等於100 ((已做過
    **/
    [Serializable]
    public class KGPoOrderPayment : IBqlTable
    {
        #region PoOrderPaymentID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Po Order Payment ID")]
        public virtual int? PoOrderPaymentID { get; set; }
        public abstract class poOrderPaymentID : PX.Data.BQL.BqlInt.Field<poOrderPaymentID> { }
        #endregion

        #region OrderNbr
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXDBDefault(typeof(POOrder.orderNbr))]
        [PXParent(typeof(Select<POOrder,
                         Where<POOrder.orderNbr, Equal<Current<KGPoOrderPayment.orderNbr>>>>))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region PricingType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pricing Type",Visible =false)]
        [PXDefault("A",PersistingCheck =PXPersistingCheck.Nothing)]
        [PricingType.List]
        public virtual string PricingType { get; set; }
        public abstract class pricingType : PX.Data.BQL.BqlString.Field<pricingType> { }
        #endregion

        #region PaymentMethod
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Method",Visible =false)]
        [PXDefault("A", PersistingCheck = PXPersistingCheck.Nothing)]
        [PaymentMethod.List]
        public virtual string PaymentMethod { get; set; }
        public abstract class paymentMethod : PX.Data.BQL.BqlString.Field<paymentMethod> { }
        #endregion

        #region PaymentPeriod
        [PXDBInt()]
        [PXUIField(DisplayName = "Payment Period")]
        public virtual int? PaymentPeriod { get; set; }
        public abstract class paymentPeriod : PX.Data.BQL.BqlInt.Field<paymentPeriod> { }
        #endregion

        #region PaymentPct
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Payment Pct")]
        public virtual Decimal? PaymentPct { get; set; }
        public abstract class paymentPct : PX.Data.BQL.BqlDecimal.Field<paymentPct> { }
        #endregion

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch(typeof(POOrder.branchID))]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion*/

        //2020/12/18 ADD
        #region VendorLocationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor LocationID",Visible =false)]
        [PXSelector(typeof(Search2<Location.locationID,
            InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Location.bAccountID>>>,
            Where<Location.bAccountID, Equal<Current<POOrder.vendorID>>,
                And<Where2<
                    Where<Current<KGPoOrderPayment.paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                        And<Location.vPaymentMethodID, Equal<word.tt>>>,
                    Or<Where<Current<KGPoOrderPayment.paymentMethod>, Equal<PaymentMethod.check>,
                        And<Location.vPaymentMethodID, Equal<word.check>>>>
                    >>
                >>),
            typeof(Location.locationCD),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(Location.locationCD))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account CD",Visible =false)]
        [PXSelector(typeof(Search<bankAccountID,
            Where<bankAccountID, IsNotNull>>))]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region BankAccount
        [PXString()]
        [PXUIField(DisplayName = "Bank Account",Visible =false, Enabled = false)]
        public virtual string BankAccount { get; set; }
        public abstract class bankAccount : PX.Data.BQL.BqlString.Field<bankAccount> { }
        #endregion

        #region BankName
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "BankName",Visible =false, Enabled = false)]
        public virtual string BankName { get; set; }
        public abstract class bankName : PX.Data.BQL.BqlString.Field<bankName> { }
        #endregion




    }
}