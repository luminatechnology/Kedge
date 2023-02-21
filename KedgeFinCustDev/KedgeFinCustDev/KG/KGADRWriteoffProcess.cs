using System;
using PX.Data;
using PX.Objects.AP;

namespace Kedge
{
  public class KGADRWriteoffProcess : PXGraph<KGADRWriteoffProcess>
  {
    public PXCancel<APRegister> Cancel;
        public PXProcessingJoin<APRegister,
           InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<APRegister.refNbr>>>,
               Where<APRegister.docType, Equal<APDocType.debitAdj>,
                   And<APRegister.status, Equal<APDocStatus.open>,
                   And2<Where<APRegisterFinExt.usrIsWriteOff, Equal<False>,
                       Or<APRegisterFinExt.usrIsWriteOff, IsNull>>,
                   And<
                       Where<APRegisterExt.usrIsDeductionDoc, Equal<True>,
                           Or<Where<APRegister.origDocType, Equal<APDocType.invoice>,
                               And<APRegister.origRefNbr, IsNotNull>>>>>>>>> ADRViews;
        #region Constructor

        public KGADRWriteoffProcess()
        {

            ADRViews.SetProcessDelegate(WriteoffPayment);
            ADRViews.SetProcessCaption("¨R¾P¥I´Ú");
            ADRViews.SetProcessAllCaption("¥þ³¡¨R¾P¥I´Ú");
            ADRViews.SetSelected<APRegister.selected>();

        }
        public static void WriteoffPayment(APRegister inv)
        {
            PXGraph.CreateInstance<KGWriteOffProcess>().ADRWriteoff(inv);
        }
        #endregion

        #region Overridden Properties

        public override bool IsDirty => false;

        #endregion
    }
}