using PX.Data;
using System;

namespace RCGV.GV.DAC
{
    [Serializable]
    public class GVTaxRev : IBqlTable
    {
        
        #region Gvtaxid
        [PXDBInt()]
        [PXUIField(DisplayName = "Gvtaxid")]
        [PXDBDefault(typeof(GVTax.gvtaxid))]
        [PXParent(typeof(Select<GVTax,
                               Where<GVTax.gvtaxid,
                               Equal<Current<GVTaxRev.gvtaxid>>>>))]
        public virtual int? Gvtaxid { get; set; }
        public abstract class gvtaxid : PX.Data.BQL.BqlInt.Field<gvtaxid> { }
        #endregion

        #region GvTaxLineID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Gv Tax Line ID")]
        public virtual int? GvTaxLineID { get; set; }
        public abstract class gvTaxLineID : PX.Data.BQL.BqlInt.Field<gvTaxLineID> { }
        #endregion

        #region RevisionID
        [PXDBInt()]
        [PXUIField(DisplayName = "Revision ID")]
        public virtual int? RevisionID { get; set; }
        public abstract class revisionID : PX.Data.BQL.BqlInt.Field<revisionID> { }
        #endregion

        #region StartDate
        [PXDBDate(IsKey = true)]       
        [PXUIField(DisplayName = "Start Date", Required = true)]
        public virtual DateTime? StartDate { get; set; }
        public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
        #endregion

        #region EndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "End Date")]
        public virtual DateTime? EndDate { get; set; }
        public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
        #endregion

        #region GvType
        [PXDBString(1, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Gv Type", Required = true)]
        [PXDefault("S")]
        [PXStringList(new string[] { "S", "P" },
                      new string[] { "¾P¶µ", "¶i¶µ" })]
        public virtual string GvType { get; set; }
        public abstract class gvType : PX.Data.BQL.BqlString.Field<gvType> { }
        #endregion

        #region TaxType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXDefault(GVTaxType.Taxable)]
        [GVTaxType.List()]
        [PXUIField(DisplayName = "Tax Type")]
        public virtual string TaxType { get; set; }
        public abstract class taxType : PX.Data.BQL.BqlString.Field<taxType> { }
        #endregion

        #region TaxRate
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tax Rate", Required = true)]
        public virtual Decimal? TaxRate { get; set; }
        public abstract class taxRate : PX.Data.BQL.BqlDecimal.Field<taxRate> { }
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
        [PXUIField(DisplayName = "Noteid")]
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

