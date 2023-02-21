using System;
using Fin.Graph;
using FIN.Util;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.GL;

namespace Fin.DAC
{
    /**
     * === 2021/03/23 Mantis:0011992 === Althea 
     * Add WHTTranID int identity
     * Add TranType string Char(3)
     * Add PayAmt decimal(10,2)
     * Add Jurisdiction Char(2)  居住地
     * Add TaxationAgreements Char(2)  租稅協定
     * Rename  PaymDate -> PaymentDate
     * 
     * ===2021/05/04 :口頭 ===Althea
     * Add PaymentDate Default BussinessDate 
     * 
     * ===2021/09/07 :0012225 ===Althea
     * Add BranchID int
     * 
     * ===2022/03/10===Jeff
     * Since users need to upload excel but DAC PK is identity field, it will impact the column must exist in source excel file to avoid existing record only being inserted instead of updated.
    **/

    [Serializable]
    [PXCacheName("Withholding Tax Trans")]
    [PXPrimaryGraph(typeof(TWNWHTInquiry))]
    public class TWNWHTTran : IBqlTable
    {
        #region Selected       
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected", Visible = true, Enabled = false, Visibility = PXUIVisibility.Invisible)]
        public bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region WHTTranID
        [PXDBIdentity(/*IsKey = true*/)]
        [PXUIField(DisplayName = "WHTTran ID", Visible = false)]
        public virtual int? WHTTranID { get; set; }
        public abstract class wHTTranID : PX.Data.BQL.BqlInt.Field<wHTTranID> { }
        #endregion

        #region TranType
        [PXDBString(3, IsUnicode =true ,IsFixed = true)]
        [PXUIField(DisplayName = "Tran Type",Required = true)]
        [FinStringList.TranType]
        public virtual string TranType { get; set; }
        public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
        #endregion

        #region DocType
        [PXDBString(3,IsFixed = true, IsKey = true)]
        [PXUIField(DisplayName = "Doc Type")]
        [PXDBDefault(typeof(APRegister.docType))]
        [APDocType.List()]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion
    
        #region RefNbr
        [PXDBString(15, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "Ref. Nbr")]
        [PXSelector(typeof(Search<APRegister.refNbr,
                                  Where<APRegister.docType, Equal<Current<docType>>,
                                        And<APRegister.refNbr, Equal<Current<refNbr>>>>>))]
        [PXDBDefault(typeof(APRegister.refNbr))]
        [PXParent(typeof(Select<APRegister, Where<APRegister.refNbr, Equal<Current<refNbr>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region LineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
        [PXLineNbr(typeof(APRegister.lineCntr))]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch Nbr")]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion
    
        #region TranDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Tran Date")]
        [PXDefault(typeof(AccessInfo.businessDate),
            PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? TranDate { get; set; }
        public abstract class tranDate : PX.Data.BQL.BqlDateTime.Field<tranDate> { }
        #endregion

        #region PaymentDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Payment Date")]
        [PXDefault(typeof(AccessInfo.businessDate),
            PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual DateTime? PaymentDate { get; set; }
        public abstract class paymentDate : PX.Data.BQL.BqlDateTime.Field<paymentDate> { }
        #endregion
    
        #region PersonalID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Personal Tax ID")]
        [PXUIRequired(typeof(Where<tranType,Equal<FinStringList.TranType.wht>>))]
        [PXDefault()]
        
        public virtual string PersonalID { get; set; }
        public abstract class personalID : PX.Data.BQL.BqlString.Field<personalID> { }
        #endregion
    
        #region PropertyID
        [PXDBString(12, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Property ID")]
        public virtual string PropertyID { get; set; }
        public abstract class propertyID : PX.Data.BQL.BqlString.Field<propertyID> { }
        #endregion
    
        #region TypeOfIn
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Type Of Income")]
        [FinStringList.TypeOfIn]
        [PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]
        [PXDefault()]

        public virtual string TypeOfIn { get; set; }
        public abstract class typeOfIn : PX.Data.BQL.BqlString.Field<typeOfIn> { }
        #endregion
    
        #region WHTFmtCode
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Format Code")]
        [WHTFMTCODESelector]
        [PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]
        [PXDefault()]
        public virtual string WHTFmtCode { get; set; }
        public abstract class wHTFmtCode : PX.Data.BQL.BqlString.Field<wHTFmtCode> { }
        #endregion
    
        #region WHTFmtSub
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Format Sub Code")]
        [WHTFMTSUBCODESelector]
        [PXDefault()]
        [PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]

        public virtual string WHTFmtSub { get; set; }
        public abstract class wHTFmtSub : PX.Data.BQL.BqlString.Field<wHTFmtSub> { }
        #endregion
    
        #region PayeeName
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payee Name")]
        [PXDefault()]
        [PXUIRequired(typeof(Where<tranType,Equal<FinStringList.TranType.wht>>))]
        public virtual string PayeeName { get; set; }
        public abstract class payeeName : PX.Data.BQL.BqlString.Field<payeeName> { }
        #endregion
    
        #region PayeeAddr
        [PXDBString(70, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payee Address")]
        [PXDefault()]
        [PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]

        public virtual string PayeeAddr { get; set; }
        public abstract class payeeAddr : PX.Data.BQL.BqlString.Field<payeeAddr> { }
        #endregion
    
        #region GNHI2Pct
        [PXDBDecimal()]
        [PXUIField(DisplayName = "2GNHI %")]
        public virtual decimal? GNHI2Pct { get; set; }
        public abstract class gNHI2Pct : PX.Data.BQL.BqlDecimal.Field<gNHI2Pct > { }
        #endregion
    
        #region GNHI2Code
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "2GNHI Code")]
        [GNHI2CodeSelector]
        public virtual string GNHI2Code { get; set; }
        public abstract class gNHI2Code : PX.Data.BQL.BqlString.Field<gNHI2Code> { }
        #endregion
    
        #region GNHI2Amt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "2GNHI Amount")]
        public virtual decimal? GNHI2Amt { get; set; }
        public abstract class gNHI2Amt : PX.Data.BQL.BqlDecimal.Field<gNHI2Amt> { }
        #endregion
   
        #region WHTTaxPct
        [PXDBString(5, IsUnicode = true)]
        [PXUIField(DisplayName = "WHT Tax %")]
        public virtual string WHTTaxPct { get; set; }
        public abstract class wHTTaxPct: PX.Data.BQL.BqlDecimal.Field<wHTTaxPct> { }
        #endregion

        #region WHTAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "WHT Amount")]
        public virtual decimal? WHTAmt { get; set; }
        public abstract class wHTAmt : PX.Data.BQL.BqlDecimal.Field<wHTAmt > { }
        #endregion

        #region NetAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Net Amount",IsReadOnly = true)]        
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? NetAmt { get; set; }
        public abstract class netAmt : PX.Data.BQL.BqlDecimal.Field<netAmt> { }
        #endregion

        #region PayAmt
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Pay Amount")]
        [PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing )]
        public virtual decimal? PayAmt { get; set; }
        public abstract class payAmt : PX.Data.BQL.BqlDecimal.Field<payAmt> { }
        #endregion

        #region Jurisdiction
        [PXDBString(2,IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Jurisdiction")]
        [PXDefault()]
        [PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]
        [WHTJURISDICTIONSelector]
        public virtual string Jurisdiction { get; set; }
        public abstract class jurisdiction : PX.Data.BQL.BqlString.Field<jurisdiction> { }
        #endregion

        #region TaxationAgreements
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "TaxationAgreements")]
        [PXDefault()]
        [PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]
        [WHTTAXAGREEMENTSelector]
        public virtual string TaxationAgreements { get; set; }
        public abstract class taxationAgreements : PX.Data.BQL.BqlString.Field<taxationAgreements> { }
        #endregion

        #region EPRefNbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "EP Ref. Nbr")]

        public virtual string EPRefNbr { get; set; }
        public abstract class ePRefNbr : PX.Data.BQL.BqlString.Field<ePRefNbr> { }
        #endregion

        //2021/09/06 Add Mantis: 0012225
        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "BranchID")]
        [PXSelector(typeof(Search<Branch.branchID>),
            typeof(Branch.branchCD),
            typeof(Branch.acctName),
            SubstituteKey = typeof(Branch.branchCD),
            DescriptionField = typeof(Branch.acctName))]
        [PXDefault(typeof(AccessInfo.branchID))]

        public virtual int? BranchID { get; set; }
        public abstract class branchID : IBqlField { }
        #endregion

        #region DuTypeCode
        [PXDBString(8)]
        [PXUIField(DisplayName = "DuTypeCode")]
        [WHTDUTYPECODESelector]
        //[PXUIRequired(typeof(Where<tranType, Equal<FinStringList.TranType.wht>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string DuTypeCode { get; set; }
        public abstract class duTypeCode : PX.Data.BQL.BqlString.Field<duTypeCode> { }
        #endregion

        #region Who Column

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
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    
        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #endregion
    }
}