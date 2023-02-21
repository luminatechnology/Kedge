using System;
using PX.Data;
using PX.Objects.GL;

namespace NM.DAC
{
    [Serializable]
    [PXCacheName("NMSettlementLog")]
    public class NMSettlementLog : IBqlTable
    {
        #region BranchID
        [PXDBInt()]
        [PXSelector(
            typeof(Search<Branch.branchID>)
            , typeof(Branch.acctName)
            , DescriptionField = typeof(Branch.acctName)
            , SubstituteKey = typeof(Branch.branchCD)
            )]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region SettlementID
        [PXDBIdentity(IsKey = true)]
        public virtual int? SettlementID { get; set; }
        public abstract class settlementID : PX.Data.BQL.BqlInt.Field<settlementID> { }
        #endregion

        #region SettlementDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Settlement Date")]
        public virtual DateTime? SettlementDate { get; set; }
        public abstract class settlementDate : PX.Data.BQL.BqlDateTime.Field<settlementDate> { }
        #endregion

        #region SettledBy
        [PXDBGuid()]
        [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]
        [PXUIField(DisplayName = "Settled By")]
        public virtual Guid? SettledBy { get; set; }
        public abstract class settledBy : PX.Data.BQL.BqlGuid.Field<settledBy> { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
    }
}