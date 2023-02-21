using System;
using EP.Util;
using PX.Data;
using CC.DAC;
using CC.Util;
using PX.Objects.CR;
using PX.Objects.PM;
using static EP.Util.EPStringList;
using PX.Objects.EP;

namespace EP.DAC
{
    /**
     * ===2021/08/19 : 0012192 === Althea
     * Add PayBatchNbr : data form CCPayable.GuarReleaseNbr
     * RefNbr Selector Add CCPayable.GuarReleaseNbr to show
     **/
    [Serializable]
    [PXCacheName("EPPaymentModiReqCCP")]
    public class EPPaymentModiReqCCP : EPPaymentModiReq
    {
        #region PaymentType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Type")]
        [PXDefault(EPStringList.PaymentType.CCP)]
        public override string PaymentType { get; set; }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "RefNbr", Required = true)]
        [PXSelector(typeof(Search2<CCPayableCheck.guarPayableCD,
            LeftJoin<BAccount2,On<BAccount2.bAccountID,Equal<CCPayableCheck.vendorID>>,
                LeftJoin<PMProject,On<PMProject.contractID,Equal<CCPayableCheck.projectID>>>>,
            Where<CCPayableCheck.status,
                NotIn3<
                    CCList.CCPayableStatus.voided, 
                    CCList.CCPayableStatus.pendingModify,
                    CCList.CCPayableStatus.pendingRefund, 
                    CCList.CCPayableStatus.appliedRTN>>>),
            typeof(CCPayableCheck.guarPayableCD),
            typeof(CCPayableCheck.status),
            typeof(CCPayableCheck.guarAmt),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(PMProject.contractCD),
            //2021/08/19 Add Mantis: 0012192
            typeof(CCPayableCheck.guarReleaseNbr))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public override string RefNbr { get; set; }
        #endregion

        #region Status
        [PXString()]
        [PXUIField(DisplayName = "Status",IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<CCPayableCheck.status,
            Where<CCPayableCheck.guarPayableCD, Equal<Current<refNbr>>>>))]
        [CCList.CCPayableStatus]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt", IsReadOnly = true)]
        [PXFormula(null, typeof(SumCalc<EPExpenseClaimExt.usrAmtCCP>))]
        public override decimal? Amt { get; set; }
        #endregion

        //2021/08/19 Add Mantis: 0012192 
        #region PayBatchNbr
        [PXString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PayBatchNbr", IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<CCPayableCheck.guarReleaseNbr,
            Where<CCPayableCheck.guarPayableCD, Equal<Current<refNbr>>>>)//,
            //PersistingCheck = PXPersistingCheck.NullOrBlank
            )]

        public override string PayBatchNbr { get; set; }
        #endregion

    }
}