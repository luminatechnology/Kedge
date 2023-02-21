using System;
using PX.Data;
using RCGV.GV.DAC;
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.Licensing;
using PX.Objects.IN;

namespace RCGV.GV.DAC
{
    /**
     * ===2021-03-11:11974 === Alton
     * [GuiInvoiceLineID] -->移除
     * [LineNumber] -->移除
     * [Quantity] --> rename 'Qty'
     * [Unit] --> rename 'UOM'
     * [OriSalesAmt] -->移除
     * [OriTaxAmt] -->移除
     * [OriCurrency] -->移除
     * [ExchangeRate] -->移除
     * [ApInvoiceNbr]--> rename 'APRefNbr'
     * [ApInvoiceLineNo] -->移除
     * [Remark] -->移除
     * [DocType] --> rename 'APDocType'
     * [Vendor] --> 移除
     * 
     * ===2021-03-17:11974 === Alton
     * 1.明細必填欄位
     *   ItemDesc
     *   Qty
     *   UnitPrice
     *   SalesAmt
     *   TaxAmt
     * 
     * **/
    [System.SerializableAttribute()]
    [PXCacheName("GVApGuiInvoiceDetail")]
    public class GVApGuiInvoiceDetail : PX.Data.IBqlTable
    {
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region GuiInvoiceID
        [PXDBInt()]
        [PXDBDefault(typeof(GVApGuiInvoice.guiInvoiceID))]
        [PXUIField(DisplayName = "GuiInvoiceID")]
        [PXParent(typeof(Select<GVApGuiInvoice,
            Where<GVApGuiInvoice.guiInvoiceID, Equal<Current<guiInvoiceID>>>>))]
        public virtual int? GuiInvoiceID { get; set; }
        public abstract class guiInvoiceID : PX.Data.BQL.BqlInt.Field<guiInvoiceID> { }
        #endregion

        #region GuiInvoiceDetailID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(Enabled = false)]
        public virtual int? GuiInvoiceDetailID { get; set; }
        public abstract class guiInvoiceDetailID : PX.Data.BQL.BqlInt.Field<guiInvoiceDetailID> { }
        #endregion

        #region APRefNbr
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "AP RefNbr")]
        [PXSelector(
            typeof(Search<APInvoice.refNbr,
                Where<APInvoice.docType, In3<APDocType.invoice, APDocType.prepayment>,
                    And<
                       Where2<
                           Where<APInvoice.vendorID, Equal<Current<GVApGuiInvoice.vendor>>,
                               And<APInvoice.status, Equal<APDocStatus.open>,
                               And<APInvoice.released, Equal<True>>>>,
                        Or<APInvoice.refNbr, Equal<Current2<GVApGuiInvoice.refNbr>>>>>>>))]
        public virtual string APRefNbr { get; set; }
        public abstract class apRefNbr : PX.Data.BQL.BqlString.Field<apRefNbr> { }
        #endregion

        #region APDocType
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "AP Doc Type")]
        public virtual string APDocType { get; set; }
        public abstract class apDocType : PX.Data.BQL.BqlString.Field<apDocType> { }
        #endregion

        #region ItemDesc
        [PXDBString(240, IsUnicode = true)]
        [PXUIField(DisplayName = "ItemDesc", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ItemDesc { get; set; }
        public abstract class itemDesc : PX.Data.BQL.BqlString.Field<itemDesc> { }
        #endregion

        #region Qty
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Qty", Required = true)]
        public virtual decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region Uom
        [INUnit(DisplayName = "Uom")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region UnitPrice
        [PXDBDecimal(3)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "UnitPrice", Required = true)]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXFormula(typeof(Mult<IsNull<qty, Zero>, IsNull<unitPrice, Zero>>), typeof(SumCalc<GVApGuiInvoice.salesAmt>))]
        [PXUIField(DisplayName = "SalesAmt", Required = true)]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXFormula(null, typeof(SumCalc<GVApGuiInvoice.taxAmt>))]
        [PXUIField(DisplayName = "TaxAmt", Required = true)]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
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
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region Unbound

        #region 這兩個傢伙是怎樣???
        #region SalesAmtTotal
        [PXDecimal(2)]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "SalesAmtTotal", Enabled = false)]
        public virtual Decimal? SalesAmtTotal { get; set; }
        public abstract class salesAmtTotal : PX.Data.BQL.BqlDecimal.Field<salesAmtTotal> { }
        #endregion

        #region TotalSalesAmt
        [PXDecimal(2)]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "TotalSalesAmt", Enabled = false)]
        public virtual Decimal? TotalSalesAmt { get; set; }
        public abstract class totalSalesAmt : PX.Data.BQL.BqlDecimal.Field<totalSalesAmt> { }
        #endregion
        #endregion

        #endregion
    }
}
