using System;
using PX.Data;

namespace Kedge.Coms.DAC
{
    [Serializable]
    [PXCacheName("KGComsBatch")]
    public class KGComsBatch : IBqlTable
    {
        #region BatchID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Batch ID", Enabled = false)]
        public virtual int? BatchID { get; set; }
        public abstract class batchID : PX.Data.BQL.BqlInt.Field<batchID> { }
        #endregion

        #region Desc
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Desc", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string Desc { get; set; }
        public abstract class desc : PX.Data.BQL.BqlString.Field<desc> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region unbound

        #region UnBatchID
        [PXInt()]
        [PXUIField(DisplayName = "Batch ID", Enabled = false)]
        [PXUnboundDefault(typeof(Switch<
            Case<Where<batchID, Greater<Zero>>, batchID>, Zero
            >))]
        public virtual int? UnBatchID { get; set; }
        public abstract class unBatchID : PX.Data.BQL.BqlInt.Field<unBatchID> { }
        #endregion

        #region VoucherCount
        [PXDBInt]
        [PXUIField(DisplayName = "Voucher Count", Enabled = false)]
        public virtual int? VoucherCount { get; set; }
        public abstract class voucherCount : PX.Data.BQL.BqlInt.Field<voucherCount> { }
        #endregion

        #region VoucherItemCount
        [PXDBInt]
        [PXUIField(DisplayName = "Voucher Item Count", Enabled = false)]
        public virtual int? VoucherItemCount { get; set; }
        public abstract class voucherItemCount : PX.Data.BQL.BqlInt.Field<voucherItemCount> { }
        #endregion

        #region InvoiceCount
        [PXDBInt]
        [PXUIField(DisplayName = "Invoice Count", Enabled = false)]
        public virtual int? InvoiceCount { get; set; }
        public abstract class invoiceCount : PX.Data.BQL.BqlInt.Field<invoiceCount> { }
        #endregion

        #region IsError
        [PXBool]
        [PXUnboundDefault]
        public virtual bool? IsError { get; set; }
        public abstract class isError : PX.Data.BQL.BqlBool.Field<isError> { }
        #endregion

        #endregion
    }
}