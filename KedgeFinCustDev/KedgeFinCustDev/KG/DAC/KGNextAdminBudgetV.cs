using System;
using PX.Data;

namespace Kedge.DAC
{
  [Serializable]
  [PXCacheName("KGNextAdminBudgetV")]
  public class KGNextAdminBudgetV : IBqlTable
  {
    #region BranchID
    [PXDBInt()]
    [PXUIField(DisplayName = "Branch ID")]
    public virtual int? BranchID { get; set; }
    public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
    #endregion

    #region LedgerID
    [PXDBInt()]
    [PXUIField(DisplayName = "Ledger ID")]
    public virtual int? LedgerID { get; set; }
    public abstract class ledgerID : PX.Data.BQL.BqlInt.Field<ledgerID> { }
        #endregion

    #region LedgerCreatedTime
    [PXDBDate()]
    [PXUIField(DisplayName = "Ledger Created Time")]
    public virtual DateTime? LedgerCreatedTime { get; set; }
    public abstract class ledgerCreatedTime : PX.Data.BQL.BqlDateTime.Field<ledgerCreatedTime> { }
    #endregion

    #region FinYear
        [PXDBString(4, IsFixed = true, InputMask = "")]
    [PXUIField(DisplayName = "Fin Year")]
    public virtual string FinYear { get; set; }
    public abstract class finYear : PX.Data.BQL.BqlString.Field<finYear> { }
    #endregion

    #region AccountID
    [PXDBInt()]
    [PXUIField(DisplayName = "Account ID")]
    public virtual int? AccountID { get; set; }
    public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
    #endregion

    #region Subid
    [PXDBInt()]
    [PXUIField(DisplayName = "Subid")]
    public virtual int? Subid { get; set; }
    public abstract class subid : PX.Data.BQL.BqlInt.Field<subid> { }
    #endregion

    #region Description
    [PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Description")]
    public virtual string Description { get; set; }
    public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
    #endregion

    #region BudgetAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Budget Amt")]
    public virtual Decimal? BudgetAmt { get; set; }
    public abstract class budgetAmt : PX.Data.BQL.BqlDecimal.Field<budgetAmt> { }
    #endregion

    #region AllocatedAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Allocated Amt")]
    public virtual Decimal? AllocatedAmt { get; set; }
    public abstract class allocatedAmt : PX.Data.BQL.BqlDecimal.Field<allocatedAmt> { }
    #endregion

    #region IsBudgetCheck
    [PXDBBool()]
    [PXUIField(DisplayName = "Is Budget Check")]
    public virtual bool? IsBudgetCheck { get; set; }
    public abstract class isBudgetCheck : PX.Data.BQL.BqlBool.Field<isBudgetCheck> { }
    #endregion

    #region BudgetCheckType
    [PXDBString(3, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Budget Check Type")]
    public virtual string BudgetCheckType { get; set; }
    public abstract class budgetCheckType : PX.Data.BQL.BqlString.Field<budgetCheckType> { }
    #endregion

    #region FinPeriodID
    [PXDBString(6, IsFixed = true, InputMask = "")]
    [PXUIField(DisplayName = "Fin Period ID")]
    public virtual string FinPeriodID { get; set; }
    public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
    #endregion

    #region MonthlyBudgetAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Monthly Budget Amt")]
    public virtual Decimal? MonthlyBudgetAmt { get; set; }
    public abstract class monthlyBudgetAmt : PX.Data.BQL.BqlDecimal.Field<monthlyBudgetAmt> { }
    #endregion

    #region BatchAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Batch Amt")]
    public virtual Decimal? BatchAmt { get; set; }
    public abstract class batchAmt : PX.Data.BQL.BqlDecimal.Field<batchAmt> { }
    #endregion

    #region EPTranAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "EPTran Amt")]
    public virtual Decimal? EPTranAmt { get; set; }
    public abstract class ePTranAmt : PX.Data.BQL.BqlDecimal.Field<ePTranAmt> { }
    #endregion

    #region APTranAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "APTran Amt")]
    public virtual Decimal? APTranAmt { get; set; }
    public abstract class aPTranAmt : PX.Data.BQL.BqlDecimal.Field<aPTranAmt> { }
    #endregion

    #region Adramt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Adramt")]
    public virtual Decimal? Adramt { get; set; }
    public abstract class adramt : PX.Data.BQL.BqlDecimal.Field<adramt> { }
    #endregion

    #region RemainBudgetAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Remain Budget Amt")]
    public virtual Decimal? RemainBudgetAmt { get; set; }
    public abstract class remainBudgetAmt : PX.Data.BQL.BqlDecimal.Field<remainBudgetAmt> { }
    #endregion
  }
}