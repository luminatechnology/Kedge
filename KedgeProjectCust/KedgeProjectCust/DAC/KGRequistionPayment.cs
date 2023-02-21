using System;
using PX.Data;
using PX.Objects.RQ;
using PX.Objects.GL;
using System.Collections.Generic;

// 20210208 PricingType 新增"其他計價" by louis
// 20210624 PaymentMethod 新增"授扣" by althea
// 20210729 PaymentMethod 新增"暫付沖銷" by althea

namespace Kedge.DAC
{
    [Serializable]
    public class KGRequistionPayment : IBqlTable
    {
        #region RequisitionPaymentID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Requisition Payment ID")]
        public virtual int? RequisitionPaymentID { get; set; }
        public abstract class requisitionPaymentID : PX.Data.BQL.BqlInt.Field<requisitionPaymentID> { }
        #endregion
    
        #region ReqNbr
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr")]
        [PXDBDefault(typeof(RQRequisition.reqNbr))]
        [PXParent(typeof(Select<RQRequisition,
                         Where<RQRequisition.reqNbr, Equal<Current<KGRequistionPayment.reqNbr>>>>))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion
    
        #region PricingType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pricing Type")]
        [PricingType.List]
        public virtual string PricingType { get; set; }
        public abstract class pricingType : PX.Data.BQL.BqlString.Field<pricingType> { }
        #endregion
    
        #region PaymentMethod
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Method")]
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
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Payment Pct")]
        public virtual Decimal? PaymentPct { get; set; }
        public abstract class paymentPct : PX.Data.BQL.BqlDecimal.Field<paymentPct> { }
        #endregion
    
        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
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
		[Branch()]
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

    public class PricingType
    {
        public const string A = "A";
        public const string B = "B";
        /// <summary>
        /// 一般計價
        /// </summary>
        public class general : PX.Data.BQL.BqlString.Constant<general> { public general() : base(A) {; } }
        /// <summary>
        /// 其他-不重計付款日
        /// </summary>
        public class other : PX.Data.BQL.BqlString.Constant<other> { public other() : base(B) {; } }

        public static readonly string[] Values =
        {
            A, B
        };

        public static readonly string[] Labels =
        {
            "一般計價", "其他-不重計付款日"
        };

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }
    }

    public class PaymentMethod
    {
        /// <summary>
        /// 支票
        /// </summary>
        public const string A = "A";
        /// <summary>
        /// 電匯
        /// </summary>
        public const string B = "B";
        /// <summary>
        /// 現金
        /// </summary>
        public const string C = "C";
        /// <summary>
        /// 禮券
        /// </summary>
        public const string D = "D";
        /// <summary>
        /// 授扣
        /// </summary>
        public const string E = "E";
        /// <summary>
        /// 暫付沖銷
        /// </summary>
       // public const string F = "F";

        /// <summary>
        /// 支票
        /// </summary>
        public class check : PX.Data.BQL.BqlString.Constant<check> { public check() : base(A) {; } }
        /// <summary>
        /// 電匯
        /// </summary>
        public class wireTransfer : PX.Data.BQL.BqlString.Constant<wireTransfer> { public wireTransfer() : base(B) {; } }
        /// <summary>
        /// 現金
        /// </summary>
        public class cash : PX.Data.BQL.BqlString.Constant<cash> { public cash() : base(C) {; } }
        /// <summary>
        /// 禮券
        /// </summary>
        public class giftCertificate  : PX.Data.BQL.BqlString.Constant<giftCertificate> { public giftCertificate() : base(D) {; } }
        /// <summary>
        /// 授扣
        /// </summary>
        public class auth : PX.Data.BQL.BqlString.Constant<auth> { public auth() : base(E) {; } }
        /// <summary>
        /// 暫付沖銷
        /// </summary>
       // public class tempwriteoff : PX.Data.BQL.BqlString.Constant<tempwriteoff> { public tempwriteoff() : base(F) {; } }

        public static readonly string[] Values =
        {
            A,  B, C, D, E//, F
        };

        public static readonly string[] Labels =
        {
            "支票",  "電匯", "現金", "禮券","授扣" //,"暫付沖銷"
        };

        public static String getLabel(String key) {
            if (key == null) {
                return null;
            }
            Dictionary<String, String> dic = new Dictionary<String, String>();
            for(int i=0;i<Values.Length;i++) {
                dic[Values[i]] = Labels[i];
            }
            return dic[key];
        }

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }
    }
}