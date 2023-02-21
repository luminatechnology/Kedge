using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using RCGV.GV.Attribute.DropDown;
using RCGV.GV.Util;

/**
 * ====2021-04-06:12006====Alton
 * ARRefNbr移除
 * ====2021-08-25:12213====Alton
 * 1.發票明細GVArGuiCmInvoiceLine新增unbound欄位, RefNbr, 請在挑選完ArGuiInvoiceNbr後帶出銷項發票所屬的ARInvoiceNbr.
 * 2.發票明細GVArGuiCmInvoiceLine新增unbound欄位, BatchNbr, 請在挑選完ArGuiInvoiceNbr後帶出銷項發票所屬的ARInvoice.BatchNbr
 * 3.GVArGuiCmInvoiceLine.ArGuiInvoiceNbr lov請多顯示下列欄位, ArGuiInvoiceNbr所屬的ARInvoice.ARInvoiceNbr/ARInvoice.BatchNbr/ARInvoice.CustomerID
 * **/
namespace RCGV.GV.DAC
{
    [Serializable]
    [PXCacheName("GVArGuiCmInvoiceLine")]
    public class GVArGuiCmInvoiceLine : IBqlTable
    {
        #region GuiCmInvoiceID
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(GVArGuiCmInvoice.guiCmInvoiceID))]
        [PXParent(typeof(Select<GVArGuiCmInvoice,
                    Where<GVArGuiCmInvoice.guiCmInvoiceID,
                    Equal<Current<guiCmInvoiceID>>>>))]
        [PXUIField(DisplayName = "GuiCmInvoiceID")]
        public virtual int? GuiCmInvoiceID { get; set; }
        public abstract class guiCmInvoiceID : PX.Data.BQL.BqlInt.Field<guiCmInvoiceID> { }
        #endregion

        #region GuiCmInvoiceLineID
        [PXDBIdentity]
        public virtual int? GuiCmInvoiceLineID { get; set; }
        public abstract class guiCmInvoiceLineID : PX.Data.BQL.BqlInt.Field<guiCmInvoiceLineID> { }
        #endregion

        #region ArGuiInvoiceNbr
        [PXDBString(40, IsUnicode = true, IsKey = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "ArGuiInvoiceNbr", Required = true, Enabled = false)]
        public virtual string ArGuiInvoiceNbr { get; set; }
        public abstract class arGuiInvoiceNbr : PX.Data.BQL.BqlString.Field<arGuiInvoiceNbr> { }
        #endregion

        #region ItemDesc
        [PXDBString(240, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Item", Required = true)]
        public virtual string ItemDesc { get; set; }
        public abstract class itemDesc : PX.Data.BQL.BqlString.Field<itemDesc> { }
        #endregion

        #region Qty
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Qty", Required = true)]
        [PXDefault(TypeCode.Decimal, "1.00", PersistingCheck = PXPersistingCheck.NullOrBlank)]
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
        [PXUIField(DisplayName = "UnitPrice", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : PX.Data.BQL.BqlDecimal.Field<unitPrice> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2, MinValue = 0)]
        [PXUIField(DisplayName = "CM SalesAmt", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXFormula(null, typeof(SumCalc<GVArGuiCmInvoice.salesAmt>))]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2, MinValue = 0)]
        [PXUIField(DisplayName = "CM TaxAmt", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXFormula(
          null,
          typeof(SumCalc<GVArGuiCmInvoice.taxAmt>))]
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

        #region ARRefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ARRefNbr")]
        public virtual string ARRefNbr { get; set; }
        public abstract class arRefNbr : PX.Data.BQL.BqlString.Field<arRefNbr> { }
        #endregion

        #region ARDocType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "ARDoc Type")]
        [ARDocType.List]
        public virtual string ARDocType { get; set; }
        public abstract class arDocType : PX.Data.BQL.BqlString.Field<arDocType> { }
        #endregion

        #region GuiInvoiceDetailID
        [PXDBInt(IsKey = true)]
        public virtual int? GuiInvoiceDetailID { get; set; }
        public abstract class guiInvoiceDetailID : PX.Data.BQL.BqlInt.Field<guiInvoiceDetailID> { }
        #endregion

        #region unbound
        #region InvSalesAmt
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Inv SalesAmt", Enabled = false)]
        [PXUnboundDefault(typeof(Search<GVArGuiInvoiceDetail.salesAmt,
            Where<GVArGuiInvoiceDetail.guiInvoiceDetailID, Equal<Current<guiInvoiceDetailID>>>>))]
        public virtual Decimal? InvSalesAmt { get; set; }
        public abstract class invSalesAmt : PX.Data.BQL.BqlDecimal.Field<invSalesAmt> { }
        #endregion

        #region Balance
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Balance", Enabled = false)]
        [PXUnboundDefault()]
        public virtual Decimal? Balance { get; set; }
        public abstract class balance : PX.Data.BQL.BqlDecimal.Field<balance> { }
        #endregion

        #region RefNbr
        [PXString()]
        [PXUIField(DisplayName = "Ref Nbr.", Enabled = false)]
        [PXUnboundDefault(typeof(Search<GVArGuiInvoiceDetail.arRefNbr,
            Where<GVArGuiInvoiceDetail.guiInvoiceDetailID, Equal<Current<guiInvoiceDetailID>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region BatchNbr
        [PXString()]
        [PXUIField(DisplayName = "Batch Nbr", Enabled = false)]
        [PXUnboundDefault(typeof(Search2<ARRegister.batchNbr,
            InnerJoin<GVArGuiInvoiceDetail,
                On<GVArGuiInvoiceDetail.arRefNbr, Equal<ARRegister.refNbr>,
                And<GVArGuiInvoiceDetail.aRDocType, Equal<ARRegister.docType>>>>,
            Where<GVArGuiInvoiceDetail.guiInvoiceDetailID, Equal<Current<guiInvoiceDetailID>>>>))]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region CustUniformNumber
        [PXString()]
        [PXUIField(DisplayName = "Cust Uniform Number", Enabled = false)]
        [PXUnboundDefault(typeof(Search2<GVArGuiInvoice.custUniformNumber,
            InnerJoin<GVArGuiInvoiceDetail,
                On<GVArGuiInvoiceDetail.guiInvoiceID, Equal<GVArGuiInvoice.guiInvoiceID>>>,
            Where<GVArGuiInvoiceDetail.guiInvoiceDetailID, Equal<Current<guiInvoiceDetailID>>>>))]
        public virtual string CustUniformNumber { get; set; }
        public abstract class custUniformNumber : PX.Data.BQL.BqlString.Field<custUniformNumber> { }
        #endregion
        #endregion

    }
}
