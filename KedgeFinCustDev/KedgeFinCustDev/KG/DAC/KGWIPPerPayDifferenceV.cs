using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.GL;

namespace KG.DAC
{
    [Serializable]
    [PXCacheName("KGWIPPerPayDifferenceV")]
    public class KGWIPPerPayDifferenceV : IBqlTable
    {
        #region LedgerID
        [PXDBInt()]
        [PXUIField(DisplayName = "Ledger ID")]
        [PXSelector(typeof(Ledger.ledgerID),
            SubstituteKey = typeof(Ledger.ledgerCD),
            DescriptionField =typeof(Ledger.descr))]
        public virtual int? LedgerID { get; set; }
        public abstract class ledgerID : PX.Data.BQL.BqlInt.Field<ledgerID> { }
        #endregion

        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "")]
        [FinPeriodIDFormatting]
        [PXUIField(DisplayName = "Fin Period ID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        [PXSelector(typeof(Branch.branchID),
            SubstituteKey = typeof(Branch.branchCD),
            DescriptionField = typeof(Branch.acctName))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region ProjectID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project ID")]
        [PXSelector(typeof(Contract.contractID),
                SubstituteKey = typeof(Contract.contractCD),
                DescriptionField = typeof(Contract.description)
                )]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region WIPBalance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "WIPBalance")]
        public virtual Decimal? WIPBalance { get; set; }
        public abstract class wIPBalance : PX.Data.BQL.BqlDecimal.Field<wIPBalance> { }
        #endregion

        #region PrePayBalance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Pre Pay Balance")]
        public virtual Decimal? PrePayBalance { get; set; }
        public abstract class prePayBalance : PX.Data.BQL.BqlDecimal.Field<prePayBalance> { }
        #endregion
    }
}