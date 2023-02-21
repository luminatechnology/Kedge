using System;
using PX.Data;
using RCGV.GV.Util;

namespace RCGV.GV.DAC
{
    /***
     * ====2021-03-23:11997 ====Alton
     * ApGuiInvoiceNbr LOV, 請帶出GVApGuiApInvoice InvoiceType='I', Status='Open', 且VendorUniformNumber = GVApGuiCmInvoice.VendorUniformNumber的進項發票
     * 
     * **/
    [Serializable]
    [PXCacheName("GVApGuiCmInvoiceLine")]
    public class GVApGuiCmInvoiceLine : IBqlTable
    {
        #region GuiCmInvoiceID
        [PXDBInt()]
        [PXDBDefault(typeof(GVApGuiCmInvoice.guiCmInvoiceID))]
        [PXUIField(DisplayName = "Gui CmInvoice ID")]
        [PXParent(
          typeof(Select<GVApGuiCmInvoice,
            Where<GVApGuiCmInvoice.guiCmInvoiceID,
            Equal<Current<guiCmInvoiceID>>>>)
        )]
        public virtual int? GuiCmInvoiceID { get; set; }
        public abstract class guiCmInvoiceID : PX.Data.BQL.BqlInt.Field<guiCmInvoiceID> { }
        #endregion

        #region GuiCmInvoiceLineID
        [PXDBIdentity(IsKey = true)]
        public virtual int? GuiCmInvoiceLineID { get; set; }
        public abstract class guiCmInvoiceLineID : PX.Data.BQL.BqlInt.Field<guiCmInvoiceLineID> { }
        #endregion

        #region ApGuiInvoiceNbr
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "ApGuiInvoice Nbr")]
        [PXSelector(
                typeof(Search<GVApGuiInvoice.guiInvoiceNbr,
                    Where<GVApGuiInvoice.vendorUniformNumber, Equal<Current<GVApGuiCmInvoice.vendorUniformNumber>>,
                        And<GVApGuiInvoice.invoiceType,Equal<GVList.GVGuiInvoiceType.invoice>,
                        And<GVApGuiInvoice.status,In3<GVList.GVStatus.open, GVList.GVStatus.hold>>>>>),
                typeof(GVApGuiInvoice.guiInvoiceNbr),
                typeof(GVApGuiInvoice.vendor),
                typeof(GVApGuiInvoice.invoiceType),
                typeof(GVApGuiInvoice.salesAmt),
                typeof(GVApGuiInvoice.taxAmt),
                    SubstituteKey = typeof(GVApGuiInvoice.guiInvoiceNbr)
            )]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ApGuiInvoiceNbr { get; set; }
        public abstract class apGuiInvoiceNbr : PX.Data.BQL.BqlString.Field<apGuiInvoiceNbr> { }
        #endregion

        #region ItemDesc
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Desc", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ItemDesc { get; set; }
        public abstract class itemDesc : PX.Data.BQL.BqlString.Field<itemDesc> { }
        #endregion

        #region Quantity
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Quantity")]
        public virtual Decimal? Quantity { get; set; }
        public abstract class quantity : PX.Data.BQL.BqlDecimal.Field<quantity> { }
        #endregion

        #region Unit
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Unit")]
        public virtual string Unit { get; set; }
        public abstract class unit : PX.Data.BQL.BqlString.Field<unit> { }
        #endregion

        #region UnitPrice
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "UnitPrice")]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "CM SalesAmt")]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "CM TaxAmt")]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region Remark
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
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

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}
