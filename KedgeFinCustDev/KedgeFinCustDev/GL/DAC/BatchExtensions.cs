using PX.Data;


namespace PX.Objects.GL
{
    /** ======2021/02/23 0011939 Edit By Althea=====
     *  ADD UsrAccConfirmNbr, nvarchar(14), «D¥²¶ñ.
    **/
    public class BatchExt : PXCacheExtension<PX.Objects.GL.Batch>
    {
        #region UsrAccConfirmNbr
        [PXDBString(14)]
        [PXUIField(DisplayName = "UsrAccConfirmNbr",IsReadOnly = true)]
        
        public virtual string UsrAccConfirmNbr { get; set; }
        public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
        #endregion

        #region UsrStageCode
        [PXDBString(2)]
        [PXUIField(DisplayName = "UsrStageCode", IsReadOnly = true)]

        public virtual string UsrStageCode { get; set; }
        public abstract class usrStageCode : PX.Data.BQL.BqlString.Field<usrStageCode> { }
        #endregion

        #region UsrRefBatchNbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "UsrRefBatchNbr", IsReadOnly = true)]

        public virtual string UsrRefBatchNbr { get; set; }
        public abstract class usrRefBatchNbr : PX.Data.BQL.BqlString.Field<usrRefBatchNbr> { }
        #endregion
    }
}