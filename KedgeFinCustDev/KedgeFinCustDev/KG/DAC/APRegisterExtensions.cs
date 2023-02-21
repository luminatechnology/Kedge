using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;
using CRLocation = PX.Objects.CR.Standalone.Location;
using IRegister = PX.Objects.CM.IRegister;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.Common.Abstractions;
using PX.Objects.Common.MigrationMode;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using PX.SM;
using PX.Objects.PO;
using Fin.DAC;
using PX.Objects.EP;
using KG.Util;

namespace PX.Objects.AP
{
    /**
     * ===2021/03/24 Mantis:0011992 ===Althea
     * Add Fields : 
     * 1.UsrWhtAmt(代扣稅總額) decimal(18, 6)
     * 2.UsrGnhi2Amt(代扣二代健保總額) decimal(18, 6)
     * 3.UsrLbiAmt(代扣勞保總額) decimal(18, 6)
     * 4.UsrHliAmt (代扣健保總額) decimal(18, 6)
     * 5.UsrLbpAmt (代扣退休金總額) decimal(18, 6)
     * 
     * ===2021/05/19 :0012042 ===Althea
     * Add UsrAccConfirmNbrForShow
     *
    **/
    public class APRegisterFinExt : PXCacheExtension<PX.Objects.AP.APRegisterExt, PX.Objects.AP.APRegister>
    {
        #region UsrIsConfirm
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrIsConfirm")]

        public virtual bool? UsrIsConfirm { get; set; }
        public abstract class usrIsConfirm : PX.Data.BQL.BqlBool.Field<usrIsConfirm> { }
        #endregion

        #region UsrConfirmBy
        [PXDBGuid]
        [PXUIField(DisplayName = "UsrConfirmBy")]
        [PXSelector(typeof(Search<Users.pKID>),
                typeof(Users.username),
                typeof(Users.firstName),
                typeof(Users.fullName),
                SubstituteKey = typeof(Users.username))]

        public virtual Guid? UsrConfirmBy { get; set; }
        public abstract class usrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrConfirmBy> { }
        #endregion

        #region UsrConfirmDate
        [PXDBDate]
        [PXUIField(DisplayName = "UsrConfirmDate")]

        public virtual DateTime? UsrConfirmDate { get; set; }
        public abstract class usrConfirmDate : PX.Data.BQL.BqlDateTime.Field<usrConfirmDate> { }
        #endregion

        #region UsrOrderDesc
        [PXString]
        [PXUIField(DisplayName = "Usr Order Desc")]
        [PXUnboundDefault(typeof(Search<POOrder.orderDesc,
            Where<POOrder.orderNbr,Equal<Current<APRegisterExt.usrPONbr>>>>))]
     
        public virtual string UsrOrderDesc { get; set; }
        public abstract class usrOrderDesc : PX.Data.BQL.BqlString.Field<usrOrderDesc> { }
        #endregion

        //2021/02/03 Add Mantis:0011939
        #region UsrAccConfirmNbr
        [PXDBString(14,IsUnicode =true)]
        [PXUIField(DisplayName = "UsrAccConfirmNbr",IsReadOnly =true)]

        public virtual string UsrAccConfirmNbr { get; set; }
        public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
        #endregion

        #region UsrDeductionRefNbr
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "UsrDeductionRefNbr")]
        [PXUnboundDefault(typeof(Search<APRegister.refNbr,
            Where<APRegister.docType, Equal<APDocType.debitAdj>,
                And<APRegisterExt.usrIsDeductionDoc, Equal<True>,
                    And<APRegisterExt.usrOriDeductionDocType, Equal<APDocType.invoice>,
                        And<APRegisterExt.usrOriDeductionRefNbr, Equal<Current<APRegister.refNbr>>>>>>>))]
        /*[PXSelector(typeof(Search<APRegister.refNbr,
            Where<APRegister.docType,Equal<APDocType.debitAdj>,
                And<APRegisterExt.usrIsDeductionDoc,Equal<True>,
                    And<APRegisterExt.usrOriDeductionDocType,Equal<APDocType.invoice>,
                        And<APRegisterExt.usrOriDeductionRefNbr,Equal<Current<APRegister.refNbr>>>>>>>))]*/

        public virtual string UsrDeductionRefNbr { get; set; }
        public abstract class usrDeductionRefNbr : PX.Data.BQL.BqlString.Field<usrDeductionRefNbr> { }
        #endregion

        //2021/03/23 Add Mantis:0011992
        #region UsrWhtAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrWhtAmt", IsReadOnly =true)]
        [PXDefault(TypeCode.Decimal,"0", PersistingCheck = PXPersistingCheck.Nothing)]      
        public virtual decimal? UsrWhtAmt { get; set; }
        public abstract class usrWhtAmt : PX.Data.BQL.BqlDecimal.Field<usrWhtAmt> { }
        #endregion

        #region UsrGnhi2Amt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrGnhi2Amt",IsReadOnly =true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual decimal? UsrGnhi2Amt { get; set; }
        public abstract class usrGnhi2Amt : PX.Data.BQL.BqlDecimal.Field<usrGnhi2Amt> { }
        #endregion

        #region UsrLbiAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrLbiAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrLbiAmt { get; set; }
        public abstract class usrLbiAmt : PX.Data.BQL.BqlDecimal.Field<usrLbiAmt> { }
        #endregion

        #region UsrHliAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrHliAmt",IsReadOnly =true)]
        [PXDefault(TypeCode.Decimal, "0",PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual decimal? UsrHliAmt { get; set; }
        public abstract class usrHliAmt : PX.Data.BQL.BqlDecimal.Field<usrHliAmt> { }
        #endregion

        #region UsrLbpAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrLbpAmt",IsReadOnly =true)]
        [PXDefault(TypeCode.Decimal, "0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual decimal? UsrLbpAmt { get; set; }
        public abstract class usrLbpAmt : PX.Data.BQL.BqlDecimal.Field<usrLbpAmt> { }
        #endregion

        //2021/05/19 Mantis:0012042
        #region UsrAccConfirmNbrForShow
        [PXString()]
        [PXUIField(DisplayName = "UsrAccConfirmNbr", IsReadOnly = true)]

        public virtual string UsrAccConfirmNbrForShow { get; set; }
        public abstract class usrAccConfirmNbrForShow : PX.Data.BQL.BqlString.Field<usrAccConfirmNbrForShow> { }
        #endregion

        //2021/06/07 Mantis:0012062
        #region UsrIsWriteOff
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrIsWriteOff")]
        public virtual bool? UsrIsWriteOff { get; set; }
        public abstract class usrIsWriteOff : PX.Data.BQL.BqlBool.Field<usrIsWriteOff> { }
        #endregion


    }
}