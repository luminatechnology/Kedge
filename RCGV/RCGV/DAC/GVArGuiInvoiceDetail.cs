namespace RCGV.GV.DAC
{
    using System;
    using PX.Data;
    using PX.Objects.AR;
    using PX.Objects.IN;

    /**
     * ====2021-03-26: 12004 ====
     * 2.欄位調整(GVArGuiInvoiceDetail)
     *   LineNumber --> 移除
     *   ArInvoiceNbr --> ARRefNbr nvarchar(15)
     *   LinkToInvoiceNbr --> 移除
     *   Quantity --> Qty
     *   Unit --> UOM
     *   ExchangeRate --> 移除
     *   OriSalesAmt --> 移除
     *   OriTaxAmt --> 移除
     *   OriCurrency --> 移除
     *   ArInvoiceLineNbr --> 移除
     *   CustomerID --> 移除
     *   DocType --> ARDocType
     *   
     * ====2021-08-10:12195====Alton
     * 	GV銷項發票/批次開立 & GV銷項折讓/批次折讓 四支作業，
     * 	原先會用 ARInvoice.Status過濾資料，經討論先不需要做這個過濾，亦即所有ARInvoice的單據，
     * 	除了status=void,rejected,reversed以外，都可以在 這四支作業看得到
     * **/
    [Serializable]
    [PXCacheName("GVArGuiInvoiceDetail")]
    public class GVArGuiInvoiceDetail : IBqlTable
    {
        #region GuiInvoiceID
        [PXDBInt()]
        [PXDBDefault(typeof(GVArGuiInvoice.guiInvoiceID))]
        [PXParent(typeof(Select<GVArGuiInvoice,
                  Where<GVArGuiInvoice.guiInvoiceID,
                  Equal<Current<GVArGuiInvoiceDetail.guiInvoiceID>>>>))]
        [PXUIField(DisplayName = "Gui Invoice ID")]
        public virtual int? GuiInvoiceID { get; set; }
        public abstract class guiInvoiceID : PX.Data.BQL.BqlInt.Field<guiInvoiceID> { }
        #endregion

        #region GuiInvoiceDetailID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(Enabled = false)]
        public virtual int? GuiInvoiceDetailID { get; set; }
        public abstract class guiInvoiceDetailID : PX.Data.BQL.BqlInt.Field<guiInvoiceDetailID> { }
        #endregion

        #region ARRefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ARRefNbr")]
        [PXSelector(
            typeof(Search<ARInvoice.refNbr,
                        Where<ARInvoice.status, NotIn3<ARDocStatus.rejected, ARDocStatus.voided, ARDocStatus.reserved>,
                        And<ARInvoice.docType,Equal<ARDocType.invoice>,
                        And<ARInvoice.customerID, Equal<Current2<GVArGuiInvoice.customerID>>>
                        >>>),
            DescriptionField = typeof(ARInvoice.docDesc)
            )]
        public virtual string ARRefNbr { get; set; }
        public abstract class arRefNbr : PX.Data.BQL.BqlString.Field<arRefNbr> { }
        #endregion

        #region ItemDesc
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Desc", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ItemDesc { get; set; }
        public abstract class itemDesc : PX.Data.BQL.BqlString.Field<itemDesc> { }
        #endregion

        #region Qty
        [PXDBDecimal(2, MinValue = 0)]
        [PXDefault(TypeCode.Decimal, "1.00", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Qty", Required = true)]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region Uom
        [INUnit(DisplayName = "Uom")]
        [PXDefault("式", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region UnitPrice
        [PXDBDecimal(2, MinValue = 0)]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "UnitPrice", Required = true)]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2, MinValue = 0)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "SalesAmt", Required = true)]
        [PXFormula(
              typeof(Mult<qty,unitPrice>)
            , typeof(SumCalc<GVArGuiInvoice.salesAmt>))]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2, MinValue = 0)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "TaxAmt", Required = true)]
        [PXFormula(null, typeof(SumCalc<GVArGuiInvoice.taxAmt>))]
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

        #region ARDocType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "ARDoc Type")]
        [ARDocType.List]
        public virtual string ARDocType { get; set; }
        public abstract class aRDocType : PX.Data.BQL.BqlString.Field<aRDocType> { }
        #endregion

        #region unbound
        #region ARCurLineAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "ARCur Line Amt", Enabled = false)]
        [PXUnboundDefault]
        public virtual decimal? ARCurLineAmt { get; set; }
        public abstract class arCurLineAmt : PX.Data.BQL.BqlDecimal.Field<arCurLineAmt> { }
        #endregion

        #region CRMAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "CRM Amt", Enabled = false)]
        [PXUnboundDefault]
        public virtual decimal? CRMAmt { get; set; }
        public abstract class crmAmt : PX.Data.BQL.BqlDecimal.Field<crmAmt> { }
        #endregion
        #endregion
    }
}
