using System;
using PX.Data;

namespace Kedge.DAC
{
  [Serializable]
  public class KG505002Month : IBqlTable,IKG505002
  {
    #region BatchID
    [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Batch ID")]
    public virtual string BatchID { get; set; }
    public abstract class batchID : PX.Data.BQL.BqlString.Field<batchID> { }
    #endregion

    #region BatchSeq
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Batch Seq")]
    public virtual int? BatchSeq { get; set; }
    public abstract class batchSeq : PX.Data.BQL.BqlInt.Field<batchSeq> { }
    #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region FinPeriodID
        [PXDBString(6, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "FinPeriodID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

    #region ContractCD
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Contract CD")]
    public virtual string ContractCD { get; set; }
    public abstract class contractCD : PX.Data.BQL.BqlString.Field<contractCD> { }
    #endregion

    #region ContractDesc
    [PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Contract Desc")]
    public virtual string ContractDesc { get; set; }
    public abstract class contractDesc : PX.Data.BQL.BqlString.Field<contractDesc> { }
    #endregion

    #region VendorName
    [PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Vendor Name")]
    public virtual string VendorName { get; set; }
    public abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
    #endregion

    #region CumIncomeLast
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Cum Income Last")]
    public virtual Decimal? CumIncomeLast { get; set; }
    public abstract class cumIncomeLast : PX.Data.BQL.BqlDecimal.Field<cumIncomeLast> { }
    #endregion

    #region CumIncomeThis
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Cum Income This")]
    public virtual Decimal? CumIncomeThis { get; set; }
    public abstract class cumIncomeThis : PX.Data.BQL.BqlDecimal.Field<cumIncomeThis> { }
    #endregion

    #region RecIncomeThis
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Rec Income This")]
    public virtual Decimal? RecIncomeThis { get; set; }
    public abstract class recIncomeThis : PX.Data.BQL.BqlDecimal.Field<recIncomeThis> { }
    #endregion

    #region CumCostLast
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Cum Cost Last")]
    public virtual Decimal? CumCostLast { get; set; }
    public abstract class cumCostLast : PX.Data.BQL.BqlDecimal.Field<cumCostLast> { }
    #endregion

    #region CumCostThis
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Cum Cost This")]
    public virtual Decimal? CumCostThis { get; set; }
    public abstract class cumCostThis : PX.Data.BQL.BqlDecimal.Field<cumCostThis> { }
    #endregion

    #region RecCostThis
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Rec Cost This")]
    public virtual Decimal? RecCostThis { get; set; }
    public abstract class recCostThis : PX.Data.BQL.BqlDecimal.Field<recCostThis> { }
    #endregion

    #region CumProfitLast
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Cum Profit Last")]
    public virtual Decimal? CumProfitLast { get; set; }
    public abstract class cumProfitLast : PX.Data.BQL.BqlDecimal.Field<cumProfitLast> { }
    #endregion

    #region CumProfitThis
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Cum Profit This")]
    public virtual Decimal? CumProfitThis { get; set; }
    public abstract class cumProfitThis : PX.Data.BQL.BqlDecimal.Field<cumProfitThis> { }
    #endregion

    #region RecProfitThis
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Rec Profit This")]
    public virtual Decimal? RecProfitThis { get; set; }
    public abstract class recProfitThis : PX.Data.BQL.BqlDecimal.Field<recProfitThis> { }
    #endregion

    #region IncomeAmt
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Income Amt")]
    public virtual Decimal? IncomeAmt { get; set; }
    public abstract class incomeAmt : PX.Data.BQL.BqlDecimal.Field<incomeAmt> { }
    #endregion

    #region ProjectProgrees
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Project Progrees")]
    public virtual Decimal? ProjectProgrees { get; set; }
    public abstract class projectProgrees : PX.Data.BQL.BqlDecimal.Field<projectProgrees> { }
    #endregion

    #region ProfitRate
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Profit Rate")]
    public virtual Decimal? ProfitRate { get; set; }
    public abstract class profitRate : PX.Data.BQL.BqlDecimal.Field<profitRate> { }
        #endregion

        #region ExpenseAmt
        [PXDBDecimal()]
    [PXUIField(DisplayName = "Expense Amt")]
    public virtual Decimal? ExpenseAmt { get; set; }
    public abstract class expenseAmt : PX.Data.BQL.BqlDecimal.Field<expenseAmt> { }
    #endregion

    #region Profit
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Profit")]
    public virtual Decimal? Profit { get; set; }
    public abstract class profit : PX.Data.BQL.BqlDecimal.Field<profit> { }
        #endregion

        #region LabelThis
        [PXDBString(10, IsFixed = true, IsUnicode = true, InputMask = "")]
        public virtual string LabelThis { get; set; }
        public abstract class labelThis : PX.Data.BQL.BqlString.Field<labelThis> { }
        #endregion

        #region LabelLast
        [PXDBString(10, IsFixed = true, IsUnicode = true, InputMask = "")]
        public virtual string LabelLast { get; set; }
        public abstract class labelLast : PX.Data.BQL.BqlString.Field<labelLast> { }
        #endregion


    }
}