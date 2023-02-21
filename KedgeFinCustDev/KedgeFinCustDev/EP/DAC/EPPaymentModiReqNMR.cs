using System;
using EP.Util;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.PM;
using static EP.Util.EPStringList;

namespace EP.DAC
{
    /**
     * ===2021/08/11 :0012192 === Althea
     * Add PayBatchNbr
     **/
    [Serializable]
    [PXCacheName("EPPaymentModiReqNMR")]
    public class EPPaymentModiReqNMR : EPPaymentModiReq
    {
        #region PaymentType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Type")]
        [PXDefault(EPStringList.PaymentType.NMR)]
        public override string PaymentType { get; set; }
        #endregion

        #region NMID
        [PXDBInt()]
        [PXUIField(DisplayName = "NMID", Required = true)]
        [PXSelector(typeof(Search2<NMReceivableCheck.receivableCheckID,
            LeftJoin<BAccount2, On<NMReceivableCheck.customerID, Equal<BAccount2.bAccountID>>,
            LeftJoin<PMProject, On<NMReceivableCheck.projectID, Equal<PMProject.contractID>>,
            InnerJoin<ARPayment, On<NMReceivableCheck.payRefNbr, Equal<ARPayment.refNbr>>>>>,
            Where<NMReceivableCheck.status,
                NotIn3<
                    //NMStringList.NMARCheckStatus.cash,
                    NMStringList.NMARCheckStatus.refund,
                    NMStringList.NMARCheckStatus.withdraw,
                    NMStringList.NMARCheckStatus.pendingmodify,
                    NMStringList.NMARCheckStatus.pendingrefund>>>),
            typeof(NMReceivableCheck.checkNbr),
            typeof(NMReceivableCheck.status),
            typeof(NMReceivableCheck.oriCuryAmount),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(NMReceivableCheck.etdDepositDate),
            typeof(NMReceivableCheck.checkDate),
            typeof(PMProject.contractCD),
            typeof(NMReceivableCheck.payRefNbr)
            //,SubstituteKey = typeof(NMReceivableCheck.checkNbr)
            )]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public override int? NMID { get; set; }
        #endregion

        #region CheckNbr
        [PXString(15, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Check Nbr", IsReadOnly = true)]
        //20220406 louis MNID連棟帶出支票號碼
        [PXUnboundDefault(typeof(Search<NMReceivableCheck.checkNbr, 
                                 Where<NMReceivableCheck.receivableCheckID, 
                                Equal<Current<EPPaymentModiReqNMR.nmID>>>>), 
                          PersistingCheck = PXPersistingCheck.Nothing)]
        
        
        public virtual string CheckNbr { get; set; }
        public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
        #endregion

        #region Status
        [PXInt()]
        [PXUIField(DisplayName = "Status", IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<NMReceivableCheck.status,
            Where<NMReceivableCheck.receivableCheckID,Equal<Current<nmID>>>>))]
        [NMStringList.NMARCheckStatus]
        public virtual int? Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt", IsReadOnly = true)]
        [PXFormula(null, typeof(SumCalc<EPExpenseClaimExt.usrAmtNMR>))]
        public override decimal? Amt { get; set; }
        #endregion

        //2021/08/11 Add Mantis: 0012192 
        #region PayBatchNbr
        [PXString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PayBatchNbr",IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search2<ARPayment.batchNbr,
            InnerJoin<NMReceivableCheck, On<NMReceivableCheck.payRefNbr,Equal<ARPayment.refNbr>>>,
            Where<NMReceivableCheck.receivableCheckID,Equal<Current<nmID>>>>)//,
            //PersistingCheck = PXPersistingCheck.NullOrBlank
            )]

        public override string PayBatchNbr { get; set; }
        #endregion
    }
}