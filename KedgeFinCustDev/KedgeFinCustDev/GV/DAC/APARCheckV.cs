using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.GL;

namespace GV.DAC
{
    [Serializable]
    [PXCacheName("APARCheckV")]
    public class APARCheckV : IBqlTable
    {
        #region BranchCD
        [PXDBString(30, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Branch CD")]
        [PXSelector(typeof(Branch.branchCD))]
        public virtual string BranchCD { get; set; }
        public abstract class branchCD : PX.Data.BQL.BqlString.Field<branchCD> { }
        #endregion

        #region DataType
        [PXDBString(2, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Data Type")]
        public virtual string DataType { get; set; }
        public abstract class dataType : PX.Data.BQL.BqlString.Field<dataType> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Batch Nbr")]
        [PXSelector(typeof(Batch.batchNbr))]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region UsrAccConfirmNbr
        [PXDBString(14, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Acc Confirm Nbr")]
        public virtual string UsrAccConfirmNbr { get; set; }
        public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
        #endregion

        #region DateEntered
        [PXDBDate()]
        [PXUIField(DisplayName = "Date Entered")]
        public virtual DateTime? DateEntered { get; set; }
        public abstract class dateEntered : PX.Data.BQL.BqlDateTime.Field<dateEntered> { }
        #endregion

        #region AccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account CD")]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
        #endregion

        #region AcctDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acct Desc")]
        public virtual string AcctDesc { get; set; }
        public abstract class acctDesc : PX.Data.BQL.BqlString.Field<acctDesc> { }
        #endregion

        #region ContractCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contract CD")]
        [PXSelector(typeof(Contract.contractCD))]
        public virtual string ContractCD { get; set; }
        public abstract class contractCD : PX.Data.BQL.BqlString.Field<contractCD> { }
        #endregion

        #region ProjectDesc
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project Desc")]
        public virtual string ProjectDesc { get; set; }
        public abstract class projectDesc : PX.Data.BQL.BqlString.Field<projectDesc> { }
        #endregion

        #region Acctcd
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acctcd")]
        public virtual string Acctcd { get; set; }
        public abstract class acctcd : PX.Data.BQL.BqlString.Field<acctcd> { }
        #endregion

        #region AcctName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acct Name")]
        public virtual string AcctName { get; set; }
        public abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }
        #endregion

        #region TranDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tran Desc")]
        public virtual string TranDesc { get; set; }
        public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }
        #endregion

        #region Debit
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Debit")]
        public virtual Decimal? Debit { get; set; }
        public abstract class debit : PX.Data.BQL.BqlDecimal.Field<debit> { }
        #endregion

        #region Inv
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inv")]
        public virtual string Inv { get; set; }
        public abstract class inv : PX.Data.BQL.BqlString.Field<inv> { }
        #endregion

        #region GVInvTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "GVInv Tax Amt")]
        public virtual Decimal? GVInvTaxAmt { get; set; }
        public abstract class gVInvTaxAmt : PX.Data.BQL.BqlDecimal.Field<gVInvTaxAmt> { }
        #endregion

        #region Credit
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Credit")]
        public virtual Decimal? Credit { get; set; }
        public abstract class credit : PX.Data.BQL.BqlDecimal.Field<credit> { }
        #endregion

        #region Gvcmtaxamt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Gvcmtaxamt")]
        public virtual Decimal? Gvcmtaxamt { get; set; }
        public abstract class gvcmtaxamt : PX.Data.BQL.BqlDecimal.Field<gvcmtaxamt> { }
        #endregion
    }
}