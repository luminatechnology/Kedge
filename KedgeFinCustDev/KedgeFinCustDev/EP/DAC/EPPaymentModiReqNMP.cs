using System;
using EP.Util;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.PM;
using static EP.Util.EPStringList;

namespace EP.DAC
{
    /**
     * ===2021/05/26 :Eva 口頭 === Althea
     * NMID 多一個APRefNbr的顯示
     * 
     * ===2021/08/11 :0012192 === Althea
     * Add PayBatchNbr
     **/

    [Serializable]
    [PXCacheName("EPPaymentModiReqNMP")]
    public class EPPaymentModiReqNMP : EPPaymentModiReq
    {
        #region PaymentType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Type")]
        [PXDefault(EPStringList.PaymentType.NMP)]
        public override string PaymentType { get; set; }
        #endregion

        #region NMID
        [PXDBInt()]
        [PXUIField(DisplayName = "NMID", Required = true)]
        [PXSelector(typeof(Search2<NMPayableCheck.payableCheckID,
             InnerJoin<BAccount2, On<NMPayableCheck.vendorID, Equal<BAccount2.bAccountID>>,
                 InnerJoin<Contract, On<NMPayableCheck.projectID, Equal<Contract.contractID>>>>,
             Where<NMPayableCheck.status,
                 NotIn3<
                     //20220425 by louis 已代開, 已兌現可以申請異動
                     //NMStringList.NMAPCheckStatus.cash,
                     NMStringList.NMAPCheckStatus.invalid,
                     //NMStringList.NMAPCheckStatus.represent,
                     NMStringList.NMAPCheckStatus.pendingmodify,
                     NMStringList.NMAPCheckStatus.pendingrefund>>>),
             typeof(NMPayableCheck.payableCheckCD),
             typeof(NMPayableCheck.checkNbr),
             typeof(NMPayableCheck.status),
             typeof(NMPayableCheck.oriCuryAmount),
             typeof(BAccount2.acctCD),
             typeof(BAccount2.acctName),
             typeof(NMPayableCheck.etdDepositDate),
             typeof(NMPayableCheck.checkDate),
             typeof(Contract.contractCD),
            typeof(NMPayableCheck.refNbr),
            typeof(NMPayableCheck.aPInvBatchNbr)
             //,SubstituteKey = typeof(NMPayableCheck.payableCheckCD)
            )]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]

        public override int? NMID { get; set; }
        #endregion

        #region CheckNbr
        [PXString(15, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Check Nbr", IsReadOnly = true)]
        //20220406 louis MNID連棟帶出支票號碼
        [PXUnboundDefault(typeof(Search<NMPayableCheck.checkNbr,
                                 Where<NMPayableCheck.payableCheckID,
                                Equal<Current<EPPaymentModiReqNMP.nmID>>>>),
                          PersistingCheck = PXPersistingCheck.Nothing)]


        public virtual string CheckNbr { get; set; }
        public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
        #endregion

        #region Status
        [PXInt()]
        [PXUIField(DisplayName = "Status", IsReadOnly = true)]
        [NMStringList.NMAPCheckStatus]
        [PXUnboundDefault(typeof(Search<NMPayableCheck.status,
            Where<NMPayableCheck.payableCheckID, Equal<Current<nmID>>>>))]
        public virtual int? Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt", IsReadOnly = true)]
        [PXFormula(null,typeof(SumCalc<EPExpenseClaimExt.usrAmtNMP>))]
        public override decimal? Amt { get; set; }
        #endregion

        //2021/08/11 Add Mantis: 0012192 
        #region PayBatchNbr
        [PXString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PayBatchNbr",  IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<NMPayableCheck.aPInvBatchNbr,
            Where<NMPayableCheck.payableCheckID,Equal<Current<nmID>>>>)//,
            //PersistingCheck = PXPersistingCheck.NullOrBlank
            )]
        
        public override string PayBatchNbr { get; set; }
        #endregion
    }
}