using System;
using PX.Data;
using PX.Objects.CS.DAC;

namespace KedgeFinCustDev.FIN.DAC
{
    [Serializable]
    [PXCacheName("APARTopNV")]
    public class APARTopNV : IBqlTable
    {
        #region Type
        [PXDBString(2, InputMask = "")]
        [PXUIField(DisplayName = "Type")]
        public virtual string Type { get; set; }
        public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        [PXSelector(typeof(Search<OrganizationBAccount.bAccountID>),
                        typeof(OrganizationBAccount.acctCD),
                        typeof(OrganizationBAccount.acctName),
                      SubstituteKey = typeof(OrganizationBAccount.acctCD), 
                      DescriptionField = typeof(OrganizationBAccount.acctName),
                      ValidateValue = false)]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region CustUniformNumber
        [PXDBString(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cust Uniform Number")]
        public virtual string CustUniformNumber { get; set; }
        public abstract class custUniformNumber : PX.Data.BQL.BqlString.Field<custUniformNumber> { }
        #endregion

        #region GuiInvoiceNbr
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Gui Invoice Nbr")]
        public virtual string GuiInvoiceNbr { get; set; }
        public abstract class guiInvoiceNbr : PX.Data.BQL.BqlString.Field<guiInvoiceNbr> { }
        #endregion

        #region CustName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cust Name")]
        public virtual string CustName { get; set; }
        public abstract class custName : PX.Data.BQL.BqlString.Field<custName> { }
        #endregion

        #region InvDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Inv Date")]
        public virtual DateTime? InvDate { get; set; }
        public abstract class invDate : PX.Data.BQL.BqlDateTime.Field<invDate> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sales Amt")]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region CmAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cm Amt")]
        public virtual Decimal? CmAmt { get; set; }
        public abstract class cmAmt : PX.Data.BQL.BqlDecimal.Field<cmAmt> { }
        #endregion
    }
}