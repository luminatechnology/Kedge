using PX.Common;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;
using Kedge.DAC;

namespace PX.Objects.PO
{
  public class POLineContractExt : PXCacheExtension<PX.Objects.PO.POLine>
  {
        #region UsrSegPricingFlag
        [PXDBBool]
        //[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName="分段計價", IsReadOnly = true, Enabled = true)]

        public virtual bool? UsrSegPricingFlag { get; set; }
        public abstract class usrSegPricingFlag : PX.Data.BQL.BqlBool.Field<usrSegPricingFlag> { }
        #endregion

        #region UsrAccountGroupID
        [PXDBInt]
        [PXUIField(DisplayName = "Account Group ID")]
        [PXSelector(typeof(Search<PMAccountGroup.groupID
            //, Where<PMAccountGroup.isActive, Equal<True>> // 專案預算可選到 isActive = false
            >),
            typeof(PMAccountGroup.groupCD),
            typeof(PMAccountGroup.description),
            typeof(PMAccountGroup.type),
            SubstituteKey = typeof(PMAccountGroup.groupCD),
            DescriptionField = typeof(PMAccountGroup.description))]
        public virtual int? UsrAccountGroupID { get; set; }
        public abstract class usrAccountGroupID : IBqlField { }
        #endregion

        #region UsrOriCuryUnitCost
        [PXDecimal]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "OriCuryUnitCost")]
        public virtual Decimal? UsrOriCuryUnitCost { get; set; }
        public abstract class usrOriCuryUnitCost : IBqlField { }
        #endregion

        #region InventDescription
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Material Description")]
        public virtual String InventDescription {
            get {
                if (UsrVendorPriceID == null)
                {
                    return BudgetInventDescription;
                }
                else {
                    return PricingInventDescription;
                }
            }
            set {
            }
        }
        public abstract class inventDescription : PX.Data.BQL.BqlString.Field<inventDescription> { }
        #endregion


        #region BudgetInventDescription
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Material Description")]
        [PXDBScalar(typeof(Search<PMBudgetExt.usrInventoryDesc,
                                 Where<PMBudget.projectID, Equal<POLine.projectID>,
                                    And<PMBudget.costCodeID, Equal<POLine.costCodeID>,
                                        And<PMBudget.inventoryID, Equal<POLine.inventoryID>,
                                            And<PMBudget.projectTaskID, Equal<POLine.taskID>>>>>>))]
        public virtual String BudgetInventDescription { get; set; }
        public abstract class budgetInventDescription : PX.Data.BQL.BqlString.Field<budgetInventDescription> { }
        #endregion

        #region PricingInventDescription
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Material Description")]
        [PXDBScalar(typeof(Search<KGVendorPrice.item,
                                 Where<KGVendorPrice.vendorPriceID, Equal<POLineContractExt.usrVendorPriceID>>>))]
        public virtual String PricingInventDescription { get; set; }
        public abstract class pricingInventDescription : PX.Data.BQL.BqlString.Field<pricingInventDescription> { }
        #endregion



        #region  UsrAvailQty
        [PXDecimal()]
        [PXUIField(DisplayName = "UsrAvail Qty", Enabled = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrAvailQty { get; set; }
        public abstract class usrAvailQty : IBqlField { }
        #endregion

        #region  UsrAvailAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "UsrAvail Amt")]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXFormula(typeof(Sub<PMBudget.revisedQty, PMBudget.committedQty>))]
        public virtual Decimal? UsrAvailAmt { get; set; }
        public abstract class usrAvailAmt : IBqlField { }
        #endregion

        //KGVendorPrice's ID
        #region  VendorPriceID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor Price ID")]
        public virtual int? UsrVendorPriceID { get; set; }
        public abstract class usrVendorPriceID : PX.Data.BQL.BqlInt.Field<usrVendorPriceID> { }
        #endregion
    }
}