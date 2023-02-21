using System;
using PX.Data;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGFlowPurchaseBidDocDetail : IBqlTable
    {
        #region PpbID
        [PXDBIdentity(IsKey = true)]
        public virtual int? PpbID { get; set; }
        public abstract class ppbID : PX.Data.BQL.BqlString.Field<ppbID> { }
        #endregion

        #region PpbUID
        [PXDBString(40, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PpbUID")]
        public virtual string PpbUID { get; set; }
        public abstract class ppbUID : PX.Data.BQL.BqlString.Field<ppbUID> { }
        #endregion

        #region PpmUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PpmUID")]
        public virtual string PpmUID { get; set; }
        public abstract class ppmUID : PX.Data.BQL.BqlString.Field<ppmUID> { }
        #endregion

        #region PppUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PppUID")]
        public virtual string PppUID { get; set; }
        public abstract class pppUID : PX.Data.BQL.BqlString.Field<pppUID> { }
        #endregion

        #region InvNo
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inv No")]
        public virtual string InvNo { get; set; }
        public abstract class invNo : PX.Data.BQL.BqlString.Field<invNo> { }
        #endregion

        #region CompanyName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company Name")]
        public virtual string CompanyName { get; set; }
        public abstract class companyName : PX.Data.BQL.BqlString.Field<companyName> { }
        #endregion

        #region ContactPhone
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contact Phone")]
        public virtual string ContactPhone { get; set; }
        public abstract class contactPhone : PX.Data.BQL.BqlString.Field<contactPhone> { }
        #endregion

        #region Contactor
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contactor")]
        public virtual string Contactor { get; set; }
        public abstract class contactor : PX.Data.BQL.BqlString.Field<contactor> { }
        #endregion

        #region FinalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Final Amt")]
        public virtual Decimal? FinalAmt { get; set; }
        public abstract class finalAmt : PX.Data.BQL.BqlDecimal.Field<finalAmt> { }
        #endregion

        #region BudTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Bud Total Amt")]
        public virtual Decimal? BudTotalAmt { get; set; }
        public abstract class budTotalAmt : PX.Data.BQL.BqlDecimal.Field<budTotalAmt> { }
        #endregion

        #region Adderss
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Address")]
        public virtual string Adderss { get; set; }
        public abstract class adderss : PX.Data.BQL.BqlString.Field<adderss> { }
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

        #region CostDay1
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Day1")]
        public virtual int? CostDay1 { get; set; }
        public abstract class costDay1 : PX.Data.BQL.BqlInt.Field<costDay1> { }
        #endregion

        #region CostRatio1
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cost Ratio1")]
        public virtual Decimal? CostRatio1 { get; set; }
        public abstract class costRatio1 : PX.Data.BQL.BqlDecimal.Field<costRatio1> { }
        #endregion

        #region CostDay2
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Day2")]
        public virtual int? CostDay2 { get; set; }
        public abstract class costDay2 : PX.Data.BQL.BqlInt.Field<costDay2> { }
        #endregion

        #region CostRatio2
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cost Ratio2")]
        public virtual Decimal? CostRatio2 { get; set; }
        public abstract class costRatio2 : PX.Data.BQL.BqlDecimal.Field<costRatio2> { }
        #endregion


        #region BudFinalPercent
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Bud Final Percent")]
        public virtual Decimal? BudFinalPercent { get; set; }
        public abstract class budFinalPercent : PX.Data.BQL.BqlDecimal.Field<budFinalPercent> { }
        #endregion

        #region ResPercent
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Res Percent")]
        public virtual Decimal? ResPercent { get; set; }
        public abstract class resPercent : PX.Data.BQL.BqlDecimal.Field<resPercent> { }
        #endregion

        #region PerformanceBondPercent
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Performance Bond Percent")]
        public virtual Decimal? PerformanceBondPercent { get; set; }
        public abstract class performanceBondPercent : PX.Data.BQL.BqlDecimal.Field<performanceBondPercent> { }
        #endregion

        #region WarrantyYearsOf
        [PXDBInt()]
        [PXUIField(DisplayName = "Warranty Years Of")]
        public virtual int? WarrantyYearsOf { get; set; }
        public abstract class warrantyYearsOf : PX.Data.BQL.BqlInt.Field<warrantyYearsOf> { }
        #endregion
        /*
        #region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch()]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion*/
    }
}