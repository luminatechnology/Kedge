using System;
using PX.Data;
using PX.Objects.CS;
using NM.Util;
using PX.Objects.EP;

namespace Kedge.DAC
{
    [Serializable]
    public class KGBillPaymentExt : PXCacheExtension<KGBillPayment>
    {
        #region UsrPayableCheckID
        [PXDBInt()]
        public virtual int? UsrPayableCheckID { get; set; }
        public abstract class usrPayableCheckID : PX.Data.BQL.BqlInt.Field<usrPayableCheckID> { }
        #endregion

        #region UsrIsCheckIssued
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual bool? UsrIsCheckIssued { get; set; }
        public abstract class usrIsCheckIssued : PX.Data.BQL.BqlBool.Field<usrIsCheckIssued> { }
        #endregion

        #region UsrEPRefNbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "EPRefNbr")]
        [PXDBDefault(typeof(EPExpenseClaim.refNbr), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXParent(typeof(Select<EPExpenseClaim, Where<EPExpenseClaim.refNbr, Equal<Current<usrEPRefNbr>>>>))]
        public virtual string UsrEPRefNbr { get; set; }
        public abstract class usrEPRefNbr : PX.Data.BQL.BqlString.Field<usrEPRefNbr> { }
        #endregion

        #region UsrNMBatchNbr
        [PXDBString(25)]
        [PXUIField(DisplayName = "UsrNMBatchNbr")]
        ///<summary>
        ///NM§å¦¸°õ¦æ¸¹½X
        ///</summary>
        public virtual string UsrNMBatchNbr { get; set; }
        public abstract class usrNMBatchNbr : PX.Data.BQL.BqlString.Field<usrNMBatchNbr> { }
        #endregion

        //2021/04/15 Add Mantis:0012024
        #region UsrTrPaymentType
        [PXDBString(1)]
        [PXUIField(DisplayName = "UsrTrPaymntType")]
        [PXSelector(typeof(Search<SegmentValue.value,
                       Where<SegmentValue.active, Equal<True>,
                           And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                               And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
               typeof(SegmentValue.value),
               typeof(SegmentValue.descr),
            DescriptionField = typeof(SegmentValue.descr))]

        public virtual string UsrTrPaymentType { get; set; }
        public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
        #endregion

        #region UsrTrConfirmID
        [PXDBInt()]
        [PXUIField(DisplayName = "UsrTrConfirmID")]
        public virtual int? UsrTrConfirmID { get; set; }
        public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
        #endregion

        #region UsrTrConfirmDate
        [PXDBDate()]
        [PXUIField(DisplayName = "UsrTrConfirmDate")]
        public virtual DateTime? UsrTrConfirmDate { get; set; }
        public abstract class usrTrConfirmDate : IBqlField { }
        #endregion

        #region UsrTrConfirmBy
        [PXDBGuid]
        [PXUIField(DisplayName = "UsrTrConfirmBy")]
        [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                typeof(PX.SM.Users.username),
                typeof(PX.SM.Users.firstName),
                typeof(PX.SM.Users.fullName),
                SubstituteKey = typeof(PX.SM.Users.username))]

        public virtual Guid? UsrTrConfirmBy { get; set; }
        public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
        #endregion

        //2021/06/07 Add Mantis:0012062
        #region UsrIsWriteOff
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrIsWriteOff")]
        public virtual bool? UsrIsWriteOff { get; set; }
        public abstract class usrIsWriteOff : PX.Data.BQL.BqlBool.Field<usrIsWriteOff> { }
        #endregion

        //2021/06/30 Add By Althea
        #region Selected
        [PXBool()]
        [PXUIField(DisplayName = "Selected")]

        public virtual bool? Selected { get; set; }
        public abstract class selected : IBqlField { }
        #endregion

        //2021/07/21 Add Mantis:0012165
        #region UsrIsTrConfirm
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrIsTrConfirm")]
        public virtual bool? UsrIsTrConfirm { get; set; }
        public abstract class usrIsTrConfirm : PX.Data.BQL.BqlBool.Field<usrIsTrConfirm> { }
        #endregion

        #region IsPaymentHold
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "IsPaymentHold")]
        public virtual bool? IsPaymentHold { get; set; }
        public abstract class isPaymentHold : PX.Data.BQL.BqlBool.Field<isPaymentHold> { }
        #endregion

        #region UsrCreatePaymentType
        [PXDBInt]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "CreatePaymentType")]
        [NMStringList.NMApCreatePaymentType]
        public virtual int? UsrCreatePaymentType { get; set; }
        public abstract class usrCreatePaymentType : PX.Data.BQL.BqlInt.Field<usrCreatePaymentType> { }
        #endregion
    }
}