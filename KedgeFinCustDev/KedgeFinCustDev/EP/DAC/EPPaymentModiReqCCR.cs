using System;
using EP.Util;
using PX.Data;
using CC.DAC;
using CC.Util;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.EP;

namespace EP.DAC
{
    /**
     * ===2021/05/21 :Eva口頭===Althea
     * CCR的RefNbr Selector 條件改為狀態只有balanced才能挑選
     * 
     * ===2021/08/19 : 0012192 ===Althea
     * Add PayBatcheNbr : data from CCReceivableCheck.BatchNbr
     * RefNbr Selector Add CCReceivableCheck.BatchNbr  to show
     **/
    [Serializable]
    [PXCacheName("EPPaymentModiReqCCR")]
    public class EPPaymentModiReqCCR : EPPaymentModiReq
    {
        #region PaymentType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Type")]
        [PXDefault(EPStringList.PaymentType.CCR)]
        public override string PaymentType { get; set; }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "RefNbr", Required = true)]
        [PXSelector(typeof(Search2<CCReceivableCheck.guarReceviableCD,
            LeftJoin<BAccount2,On<BAccount2.bAccountID,Equal<CCReceivableCheck.vendorID>>,
                LeftJoin<PMProject,On<PMProject.contractID,Equal<CCReceivableCheck.projectID>>>>,
            Where<CCReceivableCheck.status,
                //2021/05/21 Eva口頭狀態改為只有balance才能選
                Equal<CCList.CCReceivableStatus.balanced>>>),
            typeof(CCReceivableCheck.guarReceviableCD),
            typeof(CCReceivableCheck.status),
            typeof(CCReceivableCheck.guarAmt),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(PMProject.contractCD),
            //2021/08/19 Add Mantis: 0012192
            typeof(CCReceivableCheck.batchNbr))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public override string RefNbr { get; set; }
        #endregion

        #region Status
        [PXString()]
        [PXUIField(DisplayName = "Status",IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<CCReceivableCheck.status,
            Where<CCReceivableCheck.guarReceviableCD, Equal<Current<refNbr>>>>))]
        [CCList.CCReceivableStatus]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region ModifyType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Modify Type", Required = true)]
        [EPStringList.ModifyType_CCR]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public override string ModifyType { get; set; }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt", IsReadOnly = true)]
        [PXFormula(null, typeof(SumCalc<EPExpenseClaimExt.usrAmtCCR>))]
        public override decimal? Amt { get; set; }
        #endregion

        //2021/08/19 Add Mantis: 0012192 
        #region PayBatchNbr
        [PXString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PayBatchNbr", IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<CCReceivableCheck.batchNbr,
            Where<CCReceivableCheck.guarReceviableCD, Equal<Current<refNbr>>>>)//,
            //PersistingCheck = PXPersistingCheck.NullOrBlank
            )]

        public override string PayBatchNbr { get; set; }
        #endregion
    }
}