using System;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.RQ;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGRequisitionDoc : IBqlTable
    {
        #region RequisitionDocID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Requisition Doc ID")]
        public virtual int? RequisitionDocID { get; set; }
        public abstract class requisitionDocID : PX.Data.BQL.BqlInt.Field<requisitionDocID> { }
        #endregion

        #region ReqNbr
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Req Nbr")]
        [PXDBDefault(typeof(RQRequisition.reqNbr))]
        [PXParent(typeof(Select<KGRequisitionDoc,
                         Where<KGRequisitionDoc.reqNbr, Equal<Current<RQRequisition.reqNbr>>>>))]
        public virtual string ReqNbr { get; set; }
        public abstract class reqNbr : PX.Data.BQL.BqlString.Field<reqNbr> { }
        #endregion

        #region RequisitionClass
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "RequisitionClass", IsReadOnly = true)]
        [PXDefault("A", PersistingCheck =PXPersistingCheck.Nothing)]
        [ReqClass.List]
        public virtual string RequisitionClass { get; set; }
        public abstract class requisitionClass : PX.Data.BQL.BqlString.Field<requisitionClass> { }
        #endregion

        #region DefRetainagePct
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "Default Retainage Pct", Required = true)]
        public virtual Decimal? DefRetainagePct { get; set; }
        public abstract class defRetainagePct : PX.Data.BQL.BqlDecimal.Field<defRetainagePct> { }
        #endregion

        #region PaymentCalculateMethod
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Calculate Method")]
        [PymtCalcMethod.List]
        public virtual string PaymentCalculateMethod { get; set; }
        public abstract class paymentCalculateMethod : PX.Data.BQL.BqlString.Field<paymentCalculateMethod> { }
        #endregion

        #region PerformanceGuaranteePct
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "Performance Guarantee Pct", Required = true)]
        public virtual Decimal? PerformanceGuaranteePct { get; set; }
        public abstract class performanceGuaranteePct : PX.Data.BQL.BqlDecimal.Field<performanceGuaranteePct> { }
        #endregion

        #region WarrantyYear
        [PXDBDecimal(1)]
        [PXUIField(DisplayName = "Warranty Year")]
        [PXDefault(TypeCode.Decimal, "0.00")]
        public virtual Decimal? WarrantyYear { get; set; }
        public abstract class warrantyYear : PX.Data.BQL.BqlInt.Field<warrantyYear> { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "Note")]
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
        [PXUIField(DisplayName = "Tstamp")]
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

    public class PymtCalcMethod
    {
        public const string A = "A";
        public const string B = "B";

        public static readonly string[] Values =
        {
            A, B
        };

        public static readonly string[] Labels =
        {
            "依數量計算法",
            "依百分比計算法"
        };

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }

        public class a : Constant<string>
        {
            public a() : base(A) { }
        }
        public class b : Constant<string>
        {
            public b() : base(B) { }
        }
    }

    public class ReqClass
    {
        public const string A = "A";
        public const string B = "B";

        public static readonly string[] Values =
        {
            A, B
        };

        public static readonly string[] Labels =
        {
            "一般請購",
            "統購不啟單"
        };

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }

        public class a : Constant<string>
        {
            public a() : base(A) { }
        }
        public class b : Constant<string>
        {
            public b() : base(B) { }
        }
    }
}
