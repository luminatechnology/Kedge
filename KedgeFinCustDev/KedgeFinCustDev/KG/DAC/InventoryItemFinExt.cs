using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.Common.Extensions;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN.Matrix.Attributes;
using PX.Objects.IN.Matrix.Graphs;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System;

namespace PX.Objects.IN
{
    /**
     * 2021/08/10 :0012190 === Althea
     * Add UsrSettledAccountID int & UsrSettledSubID int
    **/
    public class InventoryItemFinExt : PXCacheExtension<PX.Objects.IN.InventoryItem>
    {
        #region UsrSettledAccountID
        [Account(DisplayName = "UsrSettledAccountID", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), AvoidControlAccounts = true)]
        public virtual int? UsrSettledAccountID { get; set; }
        public abstract class usrSettledAccountID : PX.Data.BQL.BqlInt.Field<usrSettledAccountID> { }
        #endregion

        #region UsrSettledSubID
        [SubAccount(typeof(InventoryItem.salesAcctID), DisplayName = "UsrSettledSubID", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]

        public virtual int? UsrSettledSubID { get; set; }
        public abstract class usrSettledSubID : PX.Data.BQL.BqlInt.Field<usrSettledSubID> { }
        #endregion
    }
}