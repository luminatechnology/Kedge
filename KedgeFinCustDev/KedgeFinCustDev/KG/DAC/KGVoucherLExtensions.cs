using Kedge.DAC;
using PX.Data;
using PX.Objects;
using PX.Objects.GL;
using System;

namespace Kedge.DAC
{
    public class KGVoucherLFinExt : PXCacheExtension<Kedge.DAC.KGVoucherL>
    {
        #region UsrSubID
        [PXDBInt]
        [PXUIField(DisplayName = "SubID",Required = true)]
        [PXSelector(typeof(Search<Sub.subID,
            Where<Sub.active, Equal<True>>>),
            typeof(Sub.subCD),
            typeof(Sub.description),
            typeof(Sub.active),
            SubstituteKey = typeof(Sub.subCD),
            DescriptionField = typeof(Sub.description))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? UsrSubID { get; set; }
        public abstract class usrSubID : PX.Data.BQL.BqlInt.Field<usrSubID> { }
        #endregion

        #region UsrTaskID
        [PXDBInt]
        [PXUIField(DisplayName = "TaskID")]

        public virtual int? UsrTaskID { get; set; }
        public abstract class usrTaskID : PX.Data.BQL.BqlInt.Field<usrTaskID> { }
        #endregion

        #region UsrCostCodeID
        [PXDBInt]
        [PXUIField(DisplayName = "CostCodeID")]

        public virtual int? UsrCostCodeID { get; set; }
        public abstract class usrCostCodeID : PX.Data.BQL.BqlInt.Field<usrCostCodeID> { }
        #endregion

        #region UsrPaymentDate
        [PXDBDate]
        [PXUIField(DisplayName = "Payment Date")]

        public virtual DateTime? UsrPaymentDate { get; set; }
        public abstract class usrPaymentDate : PX.Data.BQL.BqlInt.Field<usrPaymentDate> { }
        #endregion
    }
}