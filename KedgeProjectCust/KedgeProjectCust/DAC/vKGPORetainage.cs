using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.PM;
using PX.Objects.PO;

namespace Kedge.DAC
{
    [Serializable]
    [PXCacheName("vKGPORetainage")]
    public class vKGPORetainage : IBqlTable
    {
        #region Selected
        [PXBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region OrderDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Order Date")]
        public virtual DateTime? OrderDate { get; set; }
        public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<orderDate> { }
        #endregion

        #region OrderType
        [PXDBString(2, IsFixed = true, IsKey = true)]
        [PXUIField(DisplayName = "Order Type")]
        [POOrderType.List()]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "Order Nbr")]
        [PXSelector(typeof(SelectFrom<POOrder>.Where<POOrder.orderType.IsEqual<orderType.FromCurrent>>.SearchFor<POOrder.orderNbr>))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region VendorID
        [POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region ProjectID
        [ProjectBase]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region ContractCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project")]
        public virtual string ContractCD { get; set; }
        public abstract class contractCD : PX.Data.BQL.BqlString.Field<contractCD> { }
        #endregion

        #region RetainageTotal
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retainage Total")]
        public virtual decimal? RetainageTotal { get; set; }
        public abstract class retainageTotal : PX.Data.BQL.BqlDecimal.Field<retainageTotal> { }
        #endregion

        #region RetainageReleased
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retainage Released")]
        public virtual decimal? RetainageReleased { get; set; }
        public abstract class retainageReleased : PX.Data.BQL.BqlDecimal.Field<retainageReleased> { }
        #endregion

        #region RetainageRemaining
        [PXDecimal()]
        [PXUIField(DisplayName = "Retainage Remaining")]
        [PXFormula(typeof(Sub<retainageTotal, retainageReleased>))]
        public virtual decimal? RetainageRemaining { get; set; }
        public abstract class retainageRemaining : PX.Data.BQL.BqlDecimal.Field<retainageRemaining> { }
        #endregion  
    }
}