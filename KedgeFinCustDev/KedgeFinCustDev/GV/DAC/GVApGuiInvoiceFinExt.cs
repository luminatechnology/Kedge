using PX.Data;
using PX.Objects.AP;
using PX.SM;
using System;

namespace RCGV.GV.DAC
{
    public class GVApGuiInvoiceFinExt: PXCacheExtension<GVApGuiInvoice>
    {

        #region Unbound
        #region DocDate
        [PXDate]
        [PXUIField(DisplayName = "DocDate",IsReadOnly = true)]
        [PXUnboundDefault(typeof(
            Search<APRegister.docDate,
            Where<APRegister.refNbr, Equal<Current2<GVApGuiInvoice.refNbr>>>>))]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region UsrConfirmBy
        [PXGuid]
        [PXUIField(DisplayName = "UsrConfirmBy",IsReadOnly = true)]
        [PXUnboundDefault(typeof(
            Search<APRegisterFinExt.usrConfirmBy,
            Where<APRegister.refNbr,Equal<Current2<GVApGuiInvoice.refNbr>>>>))]
        [PXSelector(typeof(Search<Users.pKID>),
                typeof(Users.username),
                typeof(Users.firstName),
                typeof(Users.fullName),
                SubstituteKey = typeof(Users.username))]

        public virtual Guid? UsrConfirmBy { get; set; }
        public abstract class usrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrConfirmBy> { }
        #endregion
        #endregion
    }
}
