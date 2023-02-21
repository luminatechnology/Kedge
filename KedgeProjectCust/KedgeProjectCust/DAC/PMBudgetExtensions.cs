using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System.Collections;
using System;
using Kedge;

namespace PX.Objects.PM
{
      [PXNonInstantiatedExtension]
      public class PM_PMCostBudget_ExistingColumn : PXCacheExtension<PX.Objects.PM.PMCostBudget>
      {
          #region ProjectTaskID  
          [PXMergeAttributes(Method = MergeMethod.Append)]
          public int? ProjectTaskID { get; set; }
          #endregion

      }

      public class PMBudgetExt : PXCacheExtension<PX.Objects.PM.PMBudget>
      {
            [PXDBString(255,IsUnicode =true)]
            [PXUIField(DisplayName = "UsrInventoryDesc")]
            public virtual String UsrInventoryDesc { get; set; }
            public abstract class usrInventoryDesc : IBqlField { }

            [PXDBString(1)]
            [PXUIField(DisplayName = "UsrInvPrType")]
                //[PXStringList(new string[] { "0", "1" }, new string[] { "¤À¥]", "¹s¬P" })]
            [KGLookUpLov(typeof(usrInvPrType), "INVPRTYPE")]
            //CS205000

            public virtual String UsrInvPrType { get; set; }
            public abstract class usrInvPrType : IBqlField { }
    
            [PXDecimal()]
            [PXUIField(DisplayName = "UsrAvail Qty")]
            [PXDefault(TypeCode.Decimal, "0.00",PersistingCheck =PXPersistingCheck.Nothing)]
            //[PXFormula(typeof(Sub<PMBudget.revisedQty, PMBudget.committedQty>))]
            public virtual Decimal? UsrAvailQty { get; set; }
            public abstract class usrAvailQty : IBqlField { }
            #region  UsrAvailAmt
            [PXDecimal()]
            [PXUIField(DisplayName = "UsrAvail Amt")]
            [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
            //[PXFormula(typeof(Sub<PMBudget.revisedQty, PMBudget.committedQty>))]
            public virtual Decimal? UsrAvailAmt { get; set; }
            public abstract class usrAvailAmt : IBqlField { }
            #endregion

    }
}