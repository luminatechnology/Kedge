using System;
using PX.Data;

namespace PX.Objects.EP.AgentFlow.DAC
{
    [Serializable]
    [PXCacheName("KGFlowFinBillingAH")]
    public class KGFlowFinBillingAH : IBqlTable
    {
        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region HeaderID
        [PXDBIdentity(IsKey = true)]
        public virtual int? HeaderID { get; set; }
        public abstract class headerID : PX.Data.BQL.BqlInt.Field<headerID> { }
        #endregion

        #region BillType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Type")]
        public virtual string BillType { get; set; }
        public abstract class billType : PX.Data.BQL.BqlString.Field<billType> { }
        #endregion

        #region Company
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company",Required = true)]
        public virtual string Company { get; set; }
        public abstract class company : PX.Data.BQL.BqlString.Field<company> { }
        #endregion

        #region CompanyName
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company Name",Required =true)]
        public virtual string CompanyName { get; set; }
        public abstract class companyName : PX.Data.BQL.BqlString.Field<companyName> { }
        #endregion

        #region BgtYear
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bgt Year")]
        public virtual string BgtYear { get; set; }
        public abstract class bgtYear : PX.Data.BQL.BqlString.Field<bgtYear> { }
        #endregion

        #region AfmNo
        [PXDBString(9, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Afm No",Required =true)]
        public virtual string AfmNo { get; set; }
        public abstract class afmNo : PX.Data.BQL.BqlString.Field<afmNo> { }
        #endregion

        #region VendorType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Type")]
        public virtual string VendorType { get; set; }
        public abstract class vendorType : PX.Data.BQL.BqlString.Field<vendorType> { }
        #endregion

        #region UniformNo
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform No")]
        public virtual string UniformNo { get; set; }
        public abstract class uniformNo : PX.Data.BQL.BqlString.Field<uniformNo> { }
        #endregion

        #region UniformName
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform Name")]
        public virtual string UniformName { get; set; }
        public abstract class uniformName : PX.Data.BQL.BqlString.Field<uniformName> { }
        #endregion

        #region AfmDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Afm Date",Required =true)]
        public virtual DateTime? AfmDate { get; set; }
        public abstract class afmDate : PX.Data.BQL.BqlDateTime.Field<afmDate> { }
        #endregion

        #region Employee
        [PXDBString(12, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Employee",Required =true)]
        public virtual string Employee { get; set; }
        public abstract class employee : PX.Data.BQL.BqlString.Field<employee> { }
        #endregion

        #region Enable
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Enable",Required =true)]
        public virtual string Enable { get; set; }
        public abstract class enable : PX.Data.BQL.BqlString.Field<enable> { }
        #endregion

        #region SumAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sum Amount")]
        public virtual Decimal? SumAmount { get; set; }
        public abstract class sumAmount : PX.Data.BQL.BqlDecimal.Field<sumAmount> { }
        #endregion

        #region TaxExAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Ex Amt")]
        public virtual Decimal? TaxExAmt { get; set; }
        public abstract class taxExAmt : PX.Data.BQL.BqlDecimal.Field<taxExAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Amt")]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region Tax2AmtS
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax2 Amt S")]
        public virtual Decimal? Tax2AmtS { get; set; }
        public abstract class tax2AmtS : PX.Data.BQL.BqlDecimal.Field<tax2AmtS> { }
        #endregion

        #region Nhi2AmtS
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Nhi2 Amt S")]
        public virtual Decimal? Nhi2AmtS { get; set; }
        public abstract class nhi2AmtS : PX.Data.BQL.BqlDecimal.Field<nhi2AmtS> { }
        #endregion

        #region HliAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Hli Amt")]
        public virtual Decimal? HliAmt { get; set; }
        public abstract class hliAmt : PX.Data.BQL.BqlDecimal.Field<hliAmt> { }
        #endregion

        #region LbiAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Lbi Amt")]
        public virtual Decimal? LbiAmt { get; set; }
        public abstract class lbiAmt : PX.Data.BQL.BqlDecimal.Field<lbiAmt> { }
        #endregion

        #region LbpAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Lbp Amt")]
        public virtual Decimal? LbpAmt { get; set; }
        public abstract class lbpAmt : PX.Data.BQL.BqlDecimal.Field<lbpAmt> { }
        #endregion

        #region AgentFlowStatus
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Agent Flow Status")]
        public virtual string AgentFlowStatus { get; set; }
        public abstract class agentFlowStatus : PX.Data.BQL.BqlString.Field<agentFlowStatus> { }
        #endregion

        #region ReturnMsg
        [PXDBString(4000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Return Msg")]
        public virtual string ReturnMsg { get; set; }
        public abstract class returnMsg : PX.Data.BQL.BqlString.Field<returnMsg> { }
        #endregion

        #region SkipFlowFlag
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Skip Flow Flag")]
        public virtual string SkipFlowFlag { get; set; }
        public abstract class skipFlowFlag : PX.Data.BQL.BqlString.Field<skipFlowFlag> { }
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
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}