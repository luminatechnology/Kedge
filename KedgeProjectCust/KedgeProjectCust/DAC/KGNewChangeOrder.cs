using System;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.CR;
using PX.Objects.AP;

namespace Kedge.DAC
{
    [Serializable]
    public class KGNewChangeOrder : IBqlTable
    {
        #region NewChangeOrderID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "New Change Order ID")]
        public virtual int? NewChangeOrderID { get; set; }
        public abstract class newChangeOrderID : PX.Data.BQL.BqlInt.Field<newChangeOrderID> { }
        #endregion

        #region RefNbr
        [PXDBString(30, IsUnicode = true, InputMask = "",IsKey =true)]
        [PXUIField(DisplayName = "Ref Nbr")]
        [PXDBDefault(typeof(PMChangeOrder.refNbr))]
        [PXParent(typeof(Select<PMChangeOrder,
                                Where<PMChangeOrder.refNbr,
                                  Equal<Current<refNbr>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor ID")]
        [PXSelector(typeof(Search2<Vendor.bAccountID,
            InnerJoin<Location, On<Location.bAccountID, Equal<Vendor.bAccountID>>>,
            Where<Vendor.status, Equal<AccountStatusAttributes.a>,
                And<Location.isActive, Equal<True>>>,
            OrderBy<Asc<Vendor.acctCD>>>),
            typeof(Vendor.bAccountID),
            typeof(Vendor.acctCD),
            typeof(Vendor.acctName),
            typeof(Location.taxRegistrationID),
            typeof(Vendor.status),
            SubstituteKey = typeof(Vendor.acctCD),
            DescriptionField = typeof(Vendor.acctName)
        )]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region OrderName
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "OrderName")]
        public virtual string OrderName { get; set; }
        public abstract class orderName : PX.Data.BQL.BqlString.Field<orderName> { }
        #endregion

        #region Description
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount",IsReadOnly = true)]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
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
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Tax Registration ID
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Registration ID", IsReadOnly = true)]
        [PXDBScalar(typeof(Search<Location.taxRegistrationID,Where<Location.bAccountID,Equal<vendorID>>>))]
        public virtual string TaxRegistrationID { get; set; }
        public abstract class taxRegistrationID : IBqlField { }
        #endregion
    }
}