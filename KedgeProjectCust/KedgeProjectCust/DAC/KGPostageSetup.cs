using System;
using PX.Data;
using PX.Objects.GL;

namespace Kedge.DAC
{
    /**
     * ===2021/04/28 Add Fields ===Althea
     * 1. KGTTAccountID int
     * 2. KGTTSubID int
     * 3. KGVenCouponAccountID int
     * 4. KGVenCouponSubID int
     * 5. KGEmpCouponAccountID int
     * 6. KGEmpCouponSubID int
     * 7. KGTTFeeAccountID int
     * 8. KGTTFeeSubID int
     * 
     * ===2021/06/24 : 0012109 === Althea     
     * 1. KGAuthAccountID int
     * 2. KGAuthSubID int
     * 
     * ===2021/07/29 : 0012176 ===Althea
     * 1. KGTempWriteoffAccountID int
     * 2. KGTempWriteoffSubID int
     * **/
    [Serializable]
    public class KGPostageSetup : IBqlTable
    {
        #region SetupID
        [PXDBInt()]
        [PXUIField(DisplayName = "Setup ID")]
        [PXDefault(1)]
        public virtual int? SetupID { get; set; }
        public abstract class setupID : PX.Data.BQL.BqlInt.Field<setupID> { }
        #endregion

        #region CostIntervalEnd
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Interval End")]
        public virtual int? CostIntervalEnd { get; set; }
        public abstract class costIntervalEnd : PX.Data.BQL.BqlInt.Field<costIntervalEnd> { }
        #endregion

        #region CostInterval
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Interval")]
        public virtual int? CostInterval { get; set; }
        public abstract class costInterval : PX.Data.BQL.BqlInt.Field<costInterval> { }
        #endregion

        #region FirstPostCost
        [PXDBInt()]
        [PXUIField(DisplayName = "FirstPostCost")]
        public virtual int? FirstPostCost { get; set; }
        public abstract class firstPostCost : PX.Data.BQL.BqlInt.Field<firstPostCost> { }
        #endregion

        #region CostIntervalPostCost
        [PXDBInt()]
        [PXUIField(DisplayName = "CostIntervalPostCost")]
        public virtual int? CostIntervalPostCost { get; set; }
        public abstract class costIntervalPostCost : PX.Data.BQL.BqlInt.Field<costIntervalPostCost> { }
        #endregion

        #region LongTerm
        [PXDBInt()]
        [PXUIField(DisplayName = "Long Term")]
        public virtual int? LongTerm { get; set; }
        public abstract class longTerm : PX.Data.BQL.BqlInt.Field<longTerm> { }
        #endregion

        //2019/10/24 Move From KGSetup
        //2021/03/04 Add SubID
        //2021/04/28 Add Fields
        #region Account Setting

        #region KGCashAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGCashAccountID")]
        public virtual int? KGCashAccountID { get; set; }
        public abstract class kGCashAccountID : IBqlField { }
        #endregion

        #region KGCashSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGCashSubID")]
        public virtual int? KGCashSubID { get; set; }
        public abstract class kGCashSubID : IBqlField { }
        #endregion

        #region KGCheckAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGCheckAccountID")]
        public virtual int? KGCheckAccountID { get; set; }
        public abstract class kGCheckAccountID : IBqlField { }
        #endregion

        #region KGCheckSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGCheckSubID")]
        public virtual int? KGCheckSubID { get; set; }
        public abstract class kGCheckSubID : IBqlField { }
        #endregion

        #region KGPostageAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGPostageAccountID")]
        public virtual int? KGPostageAccountID { get; set; }
        public abstract class kGPostageAccountID : IBqlField { }
        #endregion

        #region KGPostageSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGPostageSubID")]
        public virtual int? KGPostageSubID { get; set; }
        public abstract class kGPostageSubID : IBqlField { }
        #endregion

        //2021/04/28 Add 
        #region KGTTAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGTTAccountID")]
        public virtual int? KGTTAccountID { get; set; }
        public abstract class kGTTAccountID : IBqlField { }
        #endregion

        #region KGTTSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGTTSubID")]

        public virtual int? KGTTSubID { get; set; }
        public abstract class kGTTSubID : IBqlField { }
        #endregion

        #region KGVenCouponAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGVenCouponAccountID")]
        public virtual int? KGVenCouponAccountID { get; set; }
        public abstract class kGVenCouponAccountID : IBqlField { }
        #endregion

        #region KGVenCouponSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGVenCouponSubID")]

        public virtual int? KGVenCouponSubID { get; set; }
        public abstract class kGVenCouponSubID : IBqlField { }
        #endregion

        #region KGEmpCouponAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGEmpCouponAccountID")]
        public virtual int? KGEmpCouponAccountID { get; set; }
        public abstract class kGEmpCouponAccountID : IBqlField { }
        #endregion

        #region KGEmpCouponSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGEmpCouponSubID")]

        public virtual int? KGEmpCouponSubID { get; set; }
        public abstract class kGEmpCouponSubID : IBqlField { }
        #endregion

        #region KGTTFeeAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGTTFeeAccountID")]
        public virtual int? KGTTFeeAccountID { get; set; }
        public abstract class kGTTFeeAccountID : IBqlField { }
        #endregion

        #region KGTTFeeSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGTTFeeSubID")]

        public virtual int? KGTTFeeSubID { get; set; }
        public abstract class kGTTFeeSubID : IBqlField { }
        #endregion

        //2021/06/24 Add
        #region KGAuthAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGAuthAccountID")]
        public virtual int? KGAuthAccountID { get; set; }
        public abstract class kGAuthAccountID : IBqlField { }
        #endregion

        #region KGAuthSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGAuthSubID")]

        public virtual int? KGAuthSubID { get; set; }
        public abstract class kGAuthSubID : IBqlField { }
        #endregion

        //2021/07/29 Add
        #region KGTempWriteoffAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGTempWriteoffAccountID")]
        public virtual int? KGTempWriteoffAccountID { get; set; }
        public abstract class kGTempWriteoffAccountID : IBqlField { }
        #endregion

        #region KGTempWriteoffSubID
        [PXDBInt()]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXUIField(DisplayName = "KGTempWriteoffSubID")]

        public virtual int? KGTempWriteoffSubID { get; set; }
        public abstract class kGTempWriteoffSubID : IBqlField { }
        #endregion
        #endregion

        #region Created

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

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #endregion
    }
}