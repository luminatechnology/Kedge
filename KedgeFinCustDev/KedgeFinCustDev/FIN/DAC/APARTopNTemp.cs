using System;
using PX.Data;

namespace KedgeFinCustDev.FIN.DAC
{
    [Serializable]
    [PXCacheName("APARTopNTemp")]
    public class APARTopNTemp : IBqlTable
    {
        #region BatchNbr
        [PXDBString(14, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch Nbr")]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region Seq
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Seq")]
        public virtual int? Seq { get; set; }
        public abstract class seq : PX.Data.BQL.BqlInt.Field<seq> { }
        #endregion

        #region UniformNumber
        [PXDBString(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform Number")]
        public virtual string UniformNumber { get; set; }
        public abstract class uniformNumber : PX.Data.BQL.BqlString.Field<uniformNumber> { }
        #endregion

        #region Name
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Name")]
        public virtual string Name { get; set; }
        public abstract class name : PX.Data.BQL.BqlString.Field<name> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion

        #region BuildTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Build Total Amt")]
        public virtual Decimal? BuildTotalAmt { get; set; }
        public abstract class buildTotalAmt : PX.Data.BQL.BqlDecimal.Field<buildTotalAmt> { }
        #endregion

        #region Type
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Type")]
        public virtual string Type { get; set; }
        public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
    }
}