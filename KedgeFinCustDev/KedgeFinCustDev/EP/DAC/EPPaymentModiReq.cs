using System;
using PX.Data;
using PX.Objects.EP;
using EP.Util;
using static EP.Util.EPStringList;
using PX.Objects.Common;

namespace EP.DAC
{
    /**
     * ===2021/08/11 :0012192 === Althea
     * Add PayBatchNbr
     **/
    [Serializable]
    [PXCacheName("EPPaymentModiReq")]
    public class EPPaymentModiReq : IBqlTable
    {
        #region PaymentModiReqID
        [PXDBIdentity(IsKey = true)]
        public virtual int? PaymentModiReqID { get; set; }
        public abstract class paymentModiReqID : PX.Data.BQL.BqlInt.Field<paymentModiReqID> { }
        #endregion

        #region EPRefNbr
        [PXDBString(10, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "EPRefNbr")]
        [PXDBDefault(typeof(EPExpenseClaim.refNbr))]
        [PXParent(typeof(Select<EPExpenseClaim,
                            Where<EPExpenseClaim.refNbr,
                            Equal<Current<epRefNbr>>>>))]
        public virtual string EPRefNbr { get; set; }
        public abstract class epRefNbr : PX.Data.BQL.BqlString.Field<epRefNbr> { }
        #endregion

        #region PaymentType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Type")]
        [EPStringList.PaymentType]
        public virtual string PaymentType { get; set; }
        public abstract class paymentType : PX.Data.BQL.BqlString.Field<paymentType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "RefNbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region NMID
        [PXDBInt()]
        public virtual int? NMID { get; set; }
        public abstract class nmID : PX.Data.BQL.BqlInt.Field<nmID> { }
        #endregion

        #region ModifyType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Modify Type", Required = true)]
        [EPStringList.ModifyType]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ModifyType { get; set; }
        public abstract class modifyType : PX.Data.BQL.BqlString.Field<modifyType> { }
        #endregion

        #region ModifyContent
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Modify Content", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ModifyContent { get; set; }
        public abstract class modifyContent : PX.Data.BQL.BqlString.Field<modifyContent> { }
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
        [PXUIField(DisplayName = "Created Date Time")]
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
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt", IsReadOnly = true)]
      
        public virtual decimal? Amt { get; set; }
        public abstract class amt : IBqlField { }
        #endregion

        //2021/08/11 Add Mantis: 0012192
        #region PayBatchNbr
        [PXString(10, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PayBatchNbr")]
        public virtual string PayBatchNbr { get; set; }
        public abstract class payBatchNbr : PX.Data.BQL.BqlString.Field<payBatchNbr> { }
        #endregion
    }

}