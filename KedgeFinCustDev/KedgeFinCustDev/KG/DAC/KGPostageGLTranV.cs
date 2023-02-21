using System;
using PX.Data;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    [PXCacheName("KGPostageGLTranV")]
    public class KGPostageGLTranV : IBqlTable
    {
        #region Module
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Module")]
        public virtual string Module { get; set; }
        public abstract class module : PX.Data.BQL.BqlString.Field<module> { }
        #endregion

        #region Batchnbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batchnbr")]
        public virtual string Batchnbr { get; set; }
        public abstract class batchnbr : PX.Data.BQL.BqlString.Field<batchnbr> { }
        #endregion

        #region AccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account ID")]
        [PXSelector(typeof(Search<Account.accountID>),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description)
            )]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region SubID
        [PXDBInt()]
        [PXUIField(DisplayName = "SubID")]
        [PXSelector(typeof(Search<Sub.subID>),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description)
            )]
        public virtual int? SubID { get; set; }
        public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        [PXSelector(typeof(Search<Branch.branchID>),
            SubstituteKey = typeof(Branch.branchCD),
            DescriptionField = typeof(Branch.acctName)
            )]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region UsrBillPaymentID
        [PXDBInt()]
        [PXUIField(DisplayName = "Usr Bill Payment ID")]
        public virtual int? UsrBillPaymentID { get; set; }
        public abstract class usrBillPaymentID : PX.Data.BQL.BqlInt.Field<usrBillPaymentID> { }
        #endregion

        #region CuryDebitAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Debit Amt")]
        public virtual Decimal? CuryDebitAmt { get; set; }
        public abstract class curyDebitAmt : PX.Data.BQL.BqlDecimal.Field<curyDebitAmt> { }
        #endregion

        #region CuryCreditAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Credit Amt")]
        public virtual Decimal? CuryCreditAmt { get; set; }
        public abstract class curyCreditAmt : PX.Data.BQL.BqlDecimal.Field<curyCreditAmt> { }
        #endregion

        #region NetAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Net Amount")]
        public virtual Decimal? NetAmount { get; set; }
        public abstract class netAmount : PX.Data.BQL.BqlDecimal.Field<netAmount> { }
        #endregion
    }
}