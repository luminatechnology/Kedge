using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.GL;

namespace Kedge.DAC
{
    /**
     * ======2021/02/08 0011946 Edit By Althea=====
     * AP301000 KGBillPayment CheckTitle設為必填  
     * ======2021/02/08 0011940 Edit By Althea=====
     *  AP301000計價彙整 付款方法調整
     *  1.付款方法的KGBillPayment.PaymentPct請改成可以輸入小數點兩位
     *  2.PaymentPct加總要等於100 ((已做過
     *  ======2021/02/09 0011946 Edit By Althea=====
     * CheckTitle設為必填,帶入的值為 VendorR.AcctName
     * ====2021-04-16 :12015 ====Alton
     * 1. KGBillPayment新增兩個欄位，並顯示在畫面上：
     *    PostageAmt, Decimal (18,6)
     *    ActPayAmt, Decimal (18,6)
     * ===2021/08/19 :0012194 ===Althea
     * BillPaymentID visible for process KG505005
     * 
     * ===2022/03/10===Jeff
     * Since users need to upload excel but DAC PK is identity field, it will impact the column must exist in source excel file to avoid existing record only being inserted instead of updated.
     * But we still keep identity field be DB one of PK and change the PK conidtion on DAC.
    **/

    [Serializable]
    public class KGBillPayment : IBqlTable
    {
        #region BillPaymentID
        [PXDBIdentity(/*IsKey = true*/)]
        [PXUIField(DisplayName = "Bill Payment ID", Visible = false)]
        public virtual int? BillPaymentID { get; set; }
        public abstract class billPaymentID : PX.Data.BQL.BqlInt.Field<billPaymentID> { }
        #endregion

        #region DocType
        [PXDBString(3, IsKey = true, IsFixed = true)]
        [PXDBDefault(typeof(APRegister.docType))]
        [APDocType.List()]
        [PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(30, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "Ref Nbr")]
        [PXDBDefault(typeof(APRegister.refNbr))]
        [PXParent(typeof(Select<APRegister, Where<APRegister.refNbr, Equal<Current<refNbr>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region LineNbr
		[PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
        [PXLineNbr(typeof(APRegister.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region PricingType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pricing Type")]
        [PXDefault("A", PersistingCheck = PXPersistingCheck.Nothing)]
        [PricingType.List]
        public virtual string PricingType { get; set; }
        public abstract class pricingType : PX.Data.BQL.BqlString.Field<pricingType> { }
        #endregion

        #region PaymentMethod
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Method")]
        [PXDefault("A")]
        [PaymentMethod.List]
        public virtual string PaymentMethod { get; set; }
        public abstract class paymentMethod : PX.Data.BQL.BqlString.Field<paymentMethod> { }
        #endregion

        #region PaymentPeriod
        [PXDBInt()]
        [PXUIField(DisplayName = "Payment Period", Required = true)]
        [PXDefault()]
        public virtual int? PaymentPeriod { get; set; }
        public abstract class paymentPeriod : PX.Data.BQL.BqlInt.Field<paymentPeriod> { }
        #endregion

        #region PaymentPct
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Payment Pct", Required = true)]
        [PXDefault]
        public virtual Decimal? PaymentPct { get; set; }
        public abstract class paymentPct : PX.Data.BQL.BqlDecimal.Field<paymentPct> { }
        #endregion

        #region PaymentDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Payment Date", Enabled = false)]
        public virtual DateTime? PaymentDate { get; set; }
        public abstract class paymentDate : PX.Data.BQL.BqlDateTime.Field<paymentDate> { }
        #endregion

        #region PaymentAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Payment Amount")]
        public virtual Decimal? PaymentAmount { get; set; }
        public abstract class paymentAmount : PX.Data.BQL.BqlDecimal.Field<paymentAmount> { }
        #endregion

        #region VendorID
        [PXDBInt]
        [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<BAccount.status.active>,
              //And<Where<BAccount2.type, Equal<BAccountType.vendorType>, Or<BAccount2.type, Equal<BAccountType.employeeType>>
                  //>>
            And<Where<BAccount2.type, In3<BAccountType.vendorType, BAccountType.combinedType, 
                                                                  BAccountType.employeeType>>>
            >>),
                  typeof(BAccount2.acctCD),
                  typeof(BAccount2.acctName),
                  typeof(BAccount2.status),
                  typeof(BAccount2.defAddressID),
                  typeof(BAccount2.defContactID),
                  typeof(BAccount2.defLocationID),
              SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
        [PXUIField(DisplayName = "VendorID", Visible = false)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region CheckTitle
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Title", Required = true)]
        //2021/02/09 Modify Mantis:0011946
        [PXDefault(typeof(Search<BAccount.acctName,
            Where<BAccount.bAccountID, Equal<Current<APInvoice.vendorID>>>>))]

        public virtual string CheckTitle { get; set; }
        public abstract class checkTitle : PX.Data.BQL.BqlString.Field<checkTitle> { }
        #endregion

        #region Remark
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
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
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        //2020/11/30 Add
        #region VendorLocationID
        [PXDBInt()]
        [PXUIField(DisplayName = "VendorLocationID", Visible = false)]
        [PXSelector(typeof(Search2<Location.locationID,
            InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Location.bAccountID>>>,
            Where<Location.bAccountID, Equal<Current<APInvoice.vendorID>>,
                And<Location.isActive, Equal<True>,
                And<Where2<
                    Where<Current<paymentMethod>, Equal<PaymentMethod.wireTransfer>,
                        And<Location.vPaymentMethodID, Equal<word.tt>>>,
                    Or<Where<Current<paymentMethod>, Equal<PaymentMethod.check>,
                        And<Location.vPaymentMethodID, Equal<word.check>>>>
                    >>
                >>>),
            typeof(Location.locationCD),
            typeof(Location.descr),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(Location.locationCD))]


        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account CD", Visible = false, Enabled = false)]
        [PXSelector(typeof(Search<bankAccountID,
            Where<bankAccountID, IsNotNull>>))]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        //2021-04-21 add
        #region PostageAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "PostageAmt", Visible = false)]
        public virtual decimal? PostageAmt { get; set; }
        public abstract class postageAmt : PX.Data.BQL.BqlDecimal.Field<postageAmt> { }
        #endregion

        #region ActPayAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "ActPayAmt", Visible = false)]
        public virtual decimal? ActPayAmt { get; set; }
        public abstract class actPayAmt : PX.Data.BQL.BqlDecimal.Field<actPayAmt> { }
        #endregion

        #region IsPostageFree
        [PXDBBool]
        [PXUIField(DisplayName = "IsPostageFree", Visible = false)]
        [PXDefault(typeof(False), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsPostageFree { get; set; }
        public abstract class isPostageFree : PX.Data.BQL.BqlBool.Field<isPostageFree> { }
        #endregion

        #region IsPaymentHold
        [PXDBBool]
        [PXUIField(DisplayName = "IsPaymentHold", Visible = false)]
        [PXDefault(typeof(False), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsPaymentHold { get; set; }
        public abstract class isPaymentHold : PX.Data.BQL.BqlBool.Field<isPaymentHold> { }
        #endregion

        #region TtActDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Act TT Date")]
        public virtual DateTime? TtActDate { get; set; }
        public abstract class ttActDate : PX.Data.BQL.BqlDateTime.Field<ttActDate> { }
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
		[Branch(typeof(APRegister.branchID))]
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
    }
}