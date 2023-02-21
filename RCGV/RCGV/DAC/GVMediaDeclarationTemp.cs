using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;

namespace RCGV.GV.DAC
{
    [Serializable]
    [PXCacheName("GVMediaDeclarationTemp")]
    public class GVMediaDeclarationTemp : IBqlTable
    {
        public class PK : PrimaryKeyOf<GVMediaDeclarationTemp>.By<declareYear, period, registrationCD>
        {
            public static GVMediaDeclarationTemp Find(PXGraph graph, int? declareYear, int? period, string registrationCD) =>
                FindBy(graph, declareYear, period, registrationCD);
        }

        #region DeclareYear
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Declare Year")]
        public virtual int? DeclareYear { get; set; }
        public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
        #endregion

        #region Period
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Period")]
        public virtual int? Period { get; set; }
        public abstract class period : PX.Data.BQL.BqlInt.Field<period> { }
        #endregion

        #region RegistrationCD
        [PXDBString(9, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Registration CD")]
        public virtual string RegistrationCD { get; set; }
        public abstract class registrationCD : PX.Data.BQL.BqlString.Field<registrationCD> { }
        #endregion

        #region DeclareBatchNbr
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Declare Batch Nbr")]
        public virtual string DeclareBatchNbr { get; set; }
        public abstract class declareBatchNbr : PX.Data.BQL.BqlString.Field<declareBatchNbr> { }
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
    }
}