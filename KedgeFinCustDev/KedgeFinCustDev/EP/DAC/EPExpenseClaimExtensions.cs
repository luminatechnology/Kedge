using PX.Data;
using PX.Objects.CR;
using PX.Objects.GL;
using System;
using EP.Util;
using static PS.Util.PSStringList;
using static EP.Util.EPStringList;
using PX.Objects.CS;

namespace PX.Objects.EP
{
    /*
     * ===2021/02/18 mantis:0011943=== Althea
     * DocType=STD, UsrFinancialYear為必填
     * 
     * ===2021/03/25 mantis:0011992=== Althea
     * Add Fields : 
     * 1.UsrWhtAmt(代扣稅總額) decimal(18, 6)
     * 2.UsrGnhi2Amt(代扣二代健保總額) decimal(18, 6)
     * 3.UsrLbiAmt(代扣勞保總額) decimal(18, 6)
     * 4.UsrHliAmt (代扣健保總額) decimal(18, 6)
     * 5.UsrLbpAmt (代扣退休金總額) decimal(18, 6)
     * 
     * ===2021/04/21 mantis:0012029=== Althea
     * Add Fields :
     * UsrApprovalStatus nvarchar(2) StringList, 參考RQRequisition.UsrApprovalStatus
     * UsrKGFlowUID nvarchar(40)
     * UsrReturnUrl nvarchar(2048)
     * 
     * ===2021-05-06===Alton
     * UsrFinancialYear LOV 降冪排序
     * 
     * ===2021/06/21 mantis:0012102===Althea
     * Add Fields:
     * UsrSkipFlowFlag bool
     * 
     * ===2021/07/07 :0012130 === Althea
     * UsrAppeovalStatus 改為EPStringList.EPUsrApprovalStatus
     * 
     * ===2021/10/19 Mantis: 0012256 === Althea
     * Add fields: 
     * UsrBankAccountID int
     * UsrBankTxnAmt decimal(19,4)
     * UsrBankTxnAmtForCal unbound decimal (為了轉換負數)
     * 
     * ===2021/11/10 :0012266 === Althea
     * Add fields:
     * UsrIsTmpPayment decimal(19,4)
     */
    public class EPExpenseClaimExt : PXCacheExtension<PX.Objects.EP.EPExpenseClaim>
    {
        #region UsrVendorID
        [PXDBInt]
        [PXUIField(DisplayName = "UsrVendorID")]
        [PXSelector(typeof(Search<BAccount2.bAccountID,
            Where<BAccount2.type,
                In3<BAccountType.vendorType,BAccountType.combinedType>,
            And<BAccount2.status,Equal<EPConst.active>>>>),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(BAccount2.status),
            SubstituteKey =typeof(BAccount2.acctCD),DescriptionField =typeof(BAccount2.acctName))]
        [PXUIVisible(typeof(Where<EPExpenseClaimExt.usrTargetType,Equal<PSTargetType.vendor>>))]
        public virtual int? UsrVendorID { get; set; }
        public abstract class usrVendorID : PX.Data.BQL.BqlInt.Field<usrVendorID> { }
        #endregion

        #region UsrVendorLocationID
        [PXDBInt]
        [PXUIField(DisplayName = "UsrVendorLocationID")]
        [PXSelector(typeof(Search<LocationExtAddress.locationID,
            Where<LocationExtAddress.bAccountID, Equal<Current<usrVendorID>>>>),
            typeof(LocationExtAddress.locationCD),
            typeof(LocationExtAddress.addressLine1),
            typeof(LocationExtAddress.addressLine2),
            SubstituteKey = typeof(LocationExtAddress.locationCD))]
        [PXUIEnabled(typeof(Where<usrVendorID,IsNotNull>))]
        [PXUIVisible(typeof(Where<EPExpenseClaimExt.usrTargetType, Equal<PSTargetType.vendor>>))]


        public virtual int? UsrVendorLocationID { get; set; }
        public abstract class usrVendorLocationID : PX.Data.BQL.BqlInt.Field<usrVendorLocationID> { }
        #endregion

        #region UsrFinancialYear
        [PXDBString(4)]
        [PXUIField(DisplayName = "UsrFinancialYear")]
        [PXSelector(typeof(Search4<GLBudgetLine.finYear,
            Where<GLBudgetLine.branchID,Equal<Current<AccessInfo.branchID>>>,
            Aggregate<GroupBy<GLBudgetLine.finYear>>,
            OrderBy<Desc<GLBudgetLine.finYear>>>),
            typeof(GLBudgetLine.finYear))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<Current<usrDocType>,Equal<EPStringList.EPDocType.std>>))]
        public virtual string UsrFinancialYear { get; set; }
        public abstract class usrFinancialYear : PX.Data.BQL.BqlString.Field<usrFinancialYear> { }
        #endregion

        //2020/09/23 討論後取消這個控制,直接顯示供應商把客戶隱藏
        #region UsrTargetType
        [PXDBString()]
        [PSTargetType()]
        [PXDefault("V",PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrTargetType")]
        [PXUIVisible(typeof(Where<Current<EPExpenseClaimExt.usrDocType>,In3<EPStringList.EPDocType.gur, EPStringList.EPDocType.rgu>>))]
        public virtual string UsrTargetType { get; set; }
        public abstract class usrTargetType : PX.Data.BQL.BqlString.Field<usrTargetType> { }
        #endregion

        //2021/01/29 Takeoff Mantis:0011925
        //2020/11/27 Add Mantis:0011814
        /*#region UsrIsGuarantee
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Guarantee Ticket")]
        public virtual bool? UsrIsGuarantee { get; set; }
        public abstract class usrIsGuarantee : PX.Data.BQL.BqlBool.Field<usrIsGuarantee> { }
        #endregion*/
        //2021/01/29 Take off Mantis:0011925
        //2020/12/14 Add Mantis:001834
        /*#region UsrGuarPayableCD
        [PXDBString(15,IsUnicode =true)]
        [PXUIField(DisplayName = "UsrGuarPayableCD",Enabled =false)]
        [PXUIVisible(typeof(Where<usrIsGuarantee,Equal<True>>))]
        public virtual string UsrGuarPayableCD { get; set; }
        public abstract class usrGuarPayableCD : PX.Data.BQL.BqlString.Field<usrGuarPayableCD> { }
        #endregion*/

        //2021/01/29 Add Mantis:0011925
        #region UsrDocType
        [PXDBString(3,IsUnicode =true)]
        [PXUIField(DisplayName = "UsrDocType")]
        [EPDocType]
        [PXDefault(EPDocType.STD)]
        public virtual string UsrDocType { get; set; }
        public abstract class usrDocType : PX.Data.BQL.BqlString.Field<usrDocType> { }
        #endregion

        //2021/03/25 Add Mantis:0011992
        #region UsrWhtAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrWhtAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrWhtAmt { get; set; }
        public abstract class usrWhtAmt : PX.Data.BQL.BqlDecimal.Field<usrWhtAmt> { }
        #endregion

        #region UsrGnhi2Amt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrGnhi2Amt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? UsrGnhi2Amt { get; set; }
        public abstract class usrGnhi2Amt : PX.Data.BQL.BqlDecimal.Field<usrGnhi2Amt> { }
        #endregion

        #region UsrLbiAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrLbiAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrLbiAmt { get; set; }
        public abstract class usrLbiAmt : PX.Data.BQL.BqlDecimal.Field<usrLbiAmt> { }
        #endregion

        #region UsrHliAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrHliAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? UsrHliAmt { get; set; }
        public abstract class usrHliAmt : PX.Data.BQL.BqlDecimal.Field<usrHliAmt> { }
        #endregion

        #region UsrLbpAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrLbpAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? UsrLbpAmt { get; set; }
        public abstract class usrLbpAmt : PX.Data.BQL.BqlDecimal.Field<usrLbpAmt> { }
        #endregion

        //2021/04/21 Add Mantis:0012029
        #region UsrApprovalStatus
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "Approval Status", IsReadOnly = true)]
        [EPUsrApprovalStatus]
        [PXDefault(EPUsrApprovalStatus.Hold,PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrApprovalStatus { get; set; }
        public abstract class usrApprovalStatus : PX.Data.BQL.BqlString.Field<usrApprovalStatus> { }
        #endregion

        #region UsrKGFlowUID
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "KGFlow UID")]
        public virtual string UsrKGFlowUID { get; set; }
        public abstract class usrKGFlowUID : PX.Data.BQL.BqlString.Field<usrKGFlowUID> { }
        #endregion

        #region UsrReturnUrl
        [PXDBString(2048)]
        [PXUIField(DisplayName = "ReturnUrl")]
        public virtual string UsrReturnUrl { get; set; }
        public abstract class usrReturnUrl : PX.Data.BQL.BqlString.Field<usrReturnUrl> { }
        #endregion

        //2021/04/26
        #region IsNMR
        [PXBool()]
        [PXUIField(DisplayName = "IsNMR", IsReadOnly = true)]
        public virtual bool? IsNMR { get; set; }
        public abstract class isNMR : IBqlField { }
        #endregion

        #region IsNMP
        [PXBool()]
        [PXUIField(DisplayName = "IsNMP", IsReadOnly = true)]
        public virtual bool? IsNMP { get; set; }
        public abstract class isNMP : IBqlField { }
        #endregion

        #region IsCCP
        [PXBool()]
        [PXUIField(DisplayName = "IsCCP", IsReadOnly = true)]
        public virtual bool? IsCCP { get; set; }
        public abstract class isCCP : IBqlField { }
        #endregion

        #region IsCCR
        [PXBool()]
        [PXUIField(DisplayName = "IsCCR", IsReadOnly = true)]
        public virtual bool? IsCCR { get; set; }
        public abstract class isCCR : IBqlField { }
        #endregion

        #region IsTT
        [PXBool()]
        [PXUIField(DisplayName = "IsTT", IsReadOnly = true)]
        public virtual bool? IsTT { get; set; }
        public abstract class isTT : IBqlField { }
        #endregion

        #region UsrAmtNMR
        [PXDBDecimal()]
        [PXUIField(DisplayName = "UsrAmtNMR", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrAmtNMR { get; set; }
        public abstract class usrAmtNMR : IBqlField { }
        #endregion

        #region UsrAmtNMP
        [PXDBDecimal()]
        [PXUIField(DisplayName = "UsrAmtNMP", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrAmtNMP { get; set; }
        public abstract class usrAmtNMP : IBqlField { }
        #endregion

        #region UsrAmtCCR
        [PXDBDecimal()]
        [PXUIField(DisplayName = "UsrAmtCCR", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrAmtCCR { get; set; }
        public abstract class usrAmtCCR : IBqlField { }
        #endregion

        #region UsrAmtCCP
        [PXDBDecimal()]
        [PXUIField(DisplayName = "UsrAmtCCP", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrAmtCCP { get; set; }
        public abstract class usrAmtCCP : IBqlField { }
        #endregion

        #region UsrAmtTT
        [PXDBDecimal()]
        [PXUIField(DisplayName = "UsrAmtTT", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrAmtTT { get; set; }
        public abstract class usrAmtTT : IBqlField { }
        #endregion

        //2021/05/31 Add Mantis: 0012062
        //2021/05/31 待討論
        //2021/06/11 Add Msntis: 0012062 (原本是UsrisVoided)
        #region UsrIsRejectByAcct
        [PXDBBool()]
        [PXUIField(DisplayName = "UsrIsRejectByAcct", IsReadOnly = true)]
        [PXDefault(false,PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsRejectByAcct { get; set; }
        public abstract class usrIsRejectByAcct : IBqlField { }
        #endregion

        //2021/06/21 Add Mantis: 0012102
        //20220314 modify by louis 新增其他收付款變更也可以免電簽
        #region UsrSkipFlowFlag
        [PXDBBool()]
        [PXUIField(DisplayName = "UsrSkipFlowFlag")]
        [PXDefault(false,PersistingCheck =PXPersistingCheck.Nothing)]
        //[PXUIVisible(typeof(Where<usrDocType,Equal<EPStringList.EPDocType.std>>))]
        [PXUIVisible(typeof(Where<Current<EPExpenseClaimExt.usrDocType>,
                     In3<EPStringList.EPDocType.std, EPStringList.EPDocType.chg, 
                         EPStringList.EPDocType.rgu, EPStringList.EPDocType.glb>>))]
        public virtual bool? UsrSkipFlowFlag { get; set; }
        public abstract class usrSkipFlowFlag : IBqlField { }
        #endregion

        //2021/10/22 delete討論後不需要
        //2021/10/18 Add Mantis: 0012256
        /*
        #region UsrBankAccountID
        [PXDBInt]
        [PXUIField(DisplayName = "BankAccountCD")]
        [NMBankAccount]
        [PXDefault()]
        [PXUIRequired(typeof(Where<usrDocType, Equal<EPDocType.btn>>))]
        [PXUIVisible(typeof(Where<usrDocType,Equal<EPStringList.EPDocType.btn>>))]
        public virtual int? UsrBankAccountID { get; set; }
        public abstract class usrBankAccountID : PX.Data.BQL.BqlInt.Field<usrBankAccountID> { }
        #endregion
        */

        //2021/10/18 Add Mantis: 0012256
        #region UsrBankTxnAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "UsrBankTxnAmt",IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIVisible(typeof(Where<EPExpenseClaimExt.usrDocType, Equal<EPStringList.EPDocType.btn>>))]
        //[PXFormula(typeof(Mult<usrBankTxnAmtForCal, EPConst.negate1>))]
        public virtual decimal? UsrBankTxnAmt { get; set; }
        public abstract class usrBankTxnAmt : PX.Data.BQL.BqlDecimal.Field<usrBankTxnAmt> { }
        #endregion

        #region UsrBankTxnAmtForCal
        [PXDecimal()]
        [PXUIField(DisplayName = "UsrBankTxnAmtForCal", IsReadOnly = true,Visible = false)]
        public virtual decimal? UsrBankTxnAmtForCal { get; set; }
        public abstract class usrBankTxnAmtForCal : PX.Data.BQL.BqlDecimal.Field<usrBankTxnAmtForCal> { }
        #endregion

        //2021/11/10 Add Mantis: 0012266
        #region UsrIsTmpPayment
        [PXDBBool]
        [PXUIField(DisplayName = "UsrIsTmpPayment")]
        [PXDefault(false,PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIVisible(typeof(Where<usrDocType,Equal<EPStringList.EPDocType.std>>))]
        public virtual bool? UsrIsTmpPayment { get; set; }
        public abstract class usrIsTmpPayment : PX.Data.BQL.BqlBool.Field<usrIsTmpPayment> { }
        #endregion

        #region UsrLineCntr
        [PXDBInt()]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrLineCntr { get; set; }
        public abstract class usrLineCntr : PX.Data.BQL.BqlInt.Field<usrLineCntr> { }
        #endregion

        #region UsrStageCode
        [PXDBString(2, IsUnicode = true)]
        [PXUIField(DisplayName = "Stage Code")]
        public virtual string UsrStageCode { get; set; }
        public abstract class usrStageCode : PX.Data.BQL.BqlString.Field<usrStageCode> { }
        #endregion

        #region unbound
        #region CCCheckTabCtrl
        [PXBool()]
        [PXUIField(DisplayName = "CC Check TabCtrl", Visible = false)]
        [PXFormula(typeof(Where<usrDocType,Equal<EPDocType.gur>,Or<usrDocType, Equal<EPDocType.rgu>>>))]
        public virtual bool? CCCheckTabCtrl { get; set; }
        public abstract class cCCheckTabCtrl : PX.Data.BQL.BqlBool.Field<cCCheckTabCtrl> { }
        #endregion
        #endregion
    }
}