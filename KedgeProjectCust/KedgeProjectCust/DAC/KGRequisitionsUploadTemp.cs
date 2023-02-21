using System;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.IN;
using PX.Objects.AP;

namespace Kedge.DAC
{
    [Serializable]
    public class KGRequisitionsUploadTemp : IBqlTable
    {
        #region TempID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "TempID")]
        public virtual int? TempID { get; set; }
        public abstract class tempID : PX.Data.BQL.BqlInt.Field<tempID> { }
        #endregion

        #region RefNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref Nbr", Required = true)]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region Usercd
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usercd", Required = true)]
        public virtual string Usercd { get; set; }
        public abstract class usercd : PX.Data.BQL.BqlString.Field<usercd> { }
        #endregion

        #region ProjectID
        [PXUIField(DisplayName = "Project ID", Required = true)]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        //[PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [ProjectBaseAttribute()]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region Date
        [PXDBDate()]
        [PXUIField(DisplayName = "Date", Required = true)]
        public virtual DateTime? Date { get; set; }
        public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }
        #endregion

        #region Description
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region CostCodeID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cost Code ID")]
        [PXDimensionSelector("COSTCODE",
            typeof(Search<PMCostCode.costCodeID>),
            typeof(PMCostCode.costCodeCD),
            typeof(PMCostCode.description)
            )]
        public virtual int? CostCodeID { get; set; }
        public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }
        #endregion

        #region Taskid
        [PXDBInt()]
        [PXUIField(DisplayName = "TaskID")]
        [PXDimensionSelector("PROTASK",
            typeof(Search<PMTask.taskID, Where<PMTask.projectID, Equal<Current<projectID>>>>),
            typeof(PMTask.taskCD),
            typeof(PMTask.description)
            )]
        public virtual int? TaskID { get; set; }
        public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXSelector(
            typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            SubstituteKey = typeof(InventoryItem.inventoryCD)
            )]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region InventoryDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Desc")]
        public virtual string InventoryDesc { get; set; }
        public abstract class inventoryDesc : PX.Data.BQL.BqlString.Field<inventoryDesc> { }
        #endregion

        #region AccountGroupID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account Group")]
        [PXSelector(
            typeof(Search<PMAccountGroup.groupID>),
            typeof(PMAccountGroup.groupCD),
            typeof(PMAccountGroup.description),
            SubstituteKey = typeof(PMAccountGroup.groupCD)
            )]
        public virtual int? AccountGroupID { get; set; }
        public abstract class accountGroupID : PX.Data.BQL.BqlInt.Field<accountGroupID> { }
        #endregion

        #region IsActive
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Active")]
        public virtual bool? IsActive { get; set; }
        public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
        #endregion

        //#region CountByRefNbr
        //[PXDBInt()]
        //[PXDefault(0)]
        //[PXUIField(DisplayName = "Count By Ref Nbr")]
        //public virtual int? CountByRefNbr { get; set; }
        //public abstract class countByRefNbr : PX.Data.BQL.BqlInt.Field<countByRefNbr> { }
        //#endregion

        #region Uom
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uom", Required = true)]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region Vendor
        [PXDBString(20,IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor", Required = true)]
        public virtual string VendorCD { get; set; }
        public abstract class vendorCD : PX.Data.BQL.BqlString.Field<vendorCD> { }
        #endregion

        #region Area
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Area", Required = true)]
        //[PXDBStringList(typeof(INSite), typeof(INSite.siteCD), typeof(INSite.descr))]
        public virtual string Area { get; set; }
        public abstract class area : PX.Data.BQL.BqlString.Field<area> { }
        #endregion

        #region OrderQty
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Order Qty", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? OrderQty { get; set; }
        public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
        #endregion

        #region UnitCost
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Unit Cost", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? UnitCost { get; set; }
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        #endregion

        #region SubTotalCost
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Sub Total Cost", Required = true)]
        [PXDefault(TypeCode.Decimal, "0")]
        public virtual Decimal? SubTotalCost { get; set; }
        public abstract class subTotalCost : PX.Data.BQL.BqlDecimal.Field<subTotalCost> { }
        #endregion

        #region DefRetainagePct
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Def Retainage Pct", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DefRetainagePct { get; set; }
        public abstract class defRetainagePct : PX.Data.BQL.BqlDecimal.Field<defRetainagePct> { }
        #endregion

        #region PaymentCalculateMethod
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Calculate Method")]
        [PymtCalcMethod.List]
        public virtual string PaymentCalculateMethod { get; set; }
        public abstract class paymentCalculateMethod : PX.Data.BQL.BqlString.Field<paymentCalculateMethod> { }
        #endregion

        #region PerformanceGuaranteePct
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Performance Guarantee Pct", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? PerformanceGuaranteePct { get; set; }
        public abstract class performanceGuaranteePct : PX.Data.BQL.BqlDecimal.Field<performanceGuaranteePct> { }
        #endregion

        #region WarrantyYear
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Warranty Year", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? WarrantyYear { get; set; }
        public abstract class warrantyYear : PX.Data.BQL.BqlDecimal.Field<warrantyYear> { }
        #endregion

        #region PaymentPeriod1
        [PXDBInt()]
        [PXUIField(DisplayName = "Payment Period1", Required = true)]
        public virtual int? PaymentPeriod1 { get; set; }
        public abstract class paymentPeriod1 : PX.Data.BQL.BqlInt.Field<paymentPeriod1> { }
        #endregion

        #region PaymentPct1
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Payment Pct1", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? PaymentPct1 { get; set; }
        public abstract class paymentPct1 : PX.Data.BQL.BqlDecimal.Field<paymentPct1> { }
        #endregion

        #region PaymentPeriod2
        [PXDBInt()]
        [PXUIField(DisplayName = "Payment Period2", Required = true)]
        [PXDefault(0)]
        public virtual int? PaymentPeriod2 { get; set; }
        public abstract class paymentPeriod2 : PX.Data.BQL.BqlInt.Field<paymentPeriod2> { }
        #endregion

        #region PaymentPct2
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Payment Pct2", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? PaymentPct2 { get; set; }
        public abstract class paymentPct2 : PX.Data.BQL.BqlDecimal.Field<paymentPct2> { }
        #endregion

        #region ErrorMsg
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error Msg", IsReadOnly = true)]
        public virtual string ErrorMsg { get; set; }
        public abstract class errorMsg : PX.Data.BQL.BqlString.Field<errorMsg> { }
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

        #region LastModifiedByDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified By Date Time")]
        public virtual DateTime? LastModifiedByDateTime { get; set; }
        public abstract class lastModifiedByDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedByDateTime> { }
        #endregion

        #region CountByRefNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Count By Ref Nbr", IsReadOnly = true)]
        public virtual int? CountByRefNbr { get; set; }
        public abstract class countByRefNbr : PX.Data.BQL.BqlInt.Field<countByRefNbr> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region unbound
        #region Type
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Type", IsReadOnly = true)]
        [PXDBCalced(typeof(Substring<refNbr, IntegerConst.int1, IntegerConst.int1>), typeof(string))]
        public virtual string Type { get; set; }
        public abstract class type : IBqlField { }
        #endregion

        #endregion
    }

    public static class IntegerConst
    {
        public const int Int1 = 1;
        public class int1 : PX.Data.BQL.BqlType<PX.Data.BQL.IBqlInt, int>.Constant<int1> { public int1() : base(Int1) { } }

    }
}