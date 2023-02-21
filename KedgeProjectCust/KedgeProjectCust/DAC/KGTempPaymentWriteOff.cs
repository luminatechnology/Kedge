using System;
using PX.Data;
using PX.Objects.AP;

namespace Kedge.DAC
{
    /**
     * ===2021/11/18 Add For Fin TempPayment use
     **/
    [Serializable]
    [PXCacheName("KGTempPaymentWriteOff")]
    public class KGTempPaymentWriteOff : IBqlTable
    {
        #region TempWriteOffID
        [PXDBIdentity(IsKey = true)]
        public virtual int? TempWriteOffID { get; set; }
        public abstract class tempWriteOffID : PX.Data.BQL.BqlInt.Field<tempWriteOffID> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region DocType
        [PXDBString(3, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Doc Type")]
        [PXDBDefault(typeof(APRegister.docType))]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Ref Nbr")]
        [PXDBDefault(typeof(APRegister.refNbr))]
        [PXParent(typeof(Select<APRegister, Where<APRegister.docType, Equal<Current<docType>>, 
            And<APRegister.refNbr, Equal<Current<refNbr>>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region Status
        [PXString()]
        [PXUIField(DisplayName = "Status")]
        [PXUnboundDefault(typeof(Search<APRegister.status,
            Where<APRegister.refNbr,Equal<Current<KGTempPaymentWriteOff.refNbr>>>>))]
        [APDocStatus.List]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region OrigRefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Orig Ref Nbr",IsReadOnly =true)]
        public virtual string OrigRefNbr { get; set; }
        public abstract class origRefNbr : PX.Data.BQL.BqlString.Field<origRefNbr> { }
        #endregion

        #region OrigDocType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Orig Doc Type", IsReadOnly = true)]
        public virtual string OrigDocType { get; set; }
        public abstract class origDocType : PX.Data.BQL.BqlString.Field<origDocType> { }
        #endregion

        #region OrigLineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Orig Line Nbr", IsReadOnly = true)]
        public virtual int? OrigLineNbr { get; set; }
        public abstract class origLineNbr : PX.Data.BQL.BqlInt.Field<origLineNbr> { }
        #endregion

        #region OrigRemainAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Orig Remain Amt")]
        [PXDefault(TypeCode.Decimal,"0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? OrigRemainAmt { get; set; }
        public abstract class origRemainAmt : PX.Data.BQL.BqlDecimal.Field<origRemainAmt> { }
        #endregion

        #region WriteOffAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Write Off Amt")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIVerify(typeof(Where<writeOffAmt,LessEqual<origRemainAmt>>), 
            PXErrorLevel.Error, "金額不可大於原始剩餘金額!")]
        public virtual Decimal? WriteOffAmt { get; set; }
        public abstract class writeOffAmt : PX.Data.BQL.BqlDecimal.Field<writeOffAmt> { }
        #endregion

        #region UnBound
        #region OrigTranDesc
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Orig Tran Desc", IsReadOnly = true)]
        [PXDefault(typeof(Search<APTran.tranDesc,
                Where<APTran.tranType, Equal<Current<origDocType>>,
                And<APTran.refNbr, Equal<Current<origRefNbr>>,
                And<APTran.lineNbr, Equal<Current<origLineNbr>>>>>>))]
        public virtual string OrigTranDesc { get; set; }
        public abstract class origTranDesc : PX.Data.BQL.BqlString.Field<origTranDesc> { }
        #endregion

        #region OrigTranAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "Orig Tran Amt")]
        [PXDefault(typeof(Search<APTran.curyTranAmt,
                Where<APTran.tranType, Equal<Current<origDocType>>,
                And<APTran.refNbr, Equal<Current<origRefNbr>>,
                And<APTran.lineNbr, Equal<Current<origLineNbr>>>>>>))]
        public virtual Decimal? OrigTranAmt { get; set; }
        public abstract class origTranAmt : PX.Data.BQL.BqlDecimal.Field<origTranAmt> { }
        #endregion

        

        #endregion

        #region Who Column
        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
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

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
        #endregion
    }
}