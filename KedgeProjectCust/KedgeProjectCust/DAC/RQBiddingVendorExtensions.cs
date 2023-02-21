using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.RQ;
using PX.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace PX.Objects.RQ
{
    public class RQBiddingVendorExt : PXCacheExtension<PX.Objects.RQ.RQBiddingVendor>
    {
        #region UsrVendQuoteCost
        [PXDecimal]
        [PXUIField(DisplayName = "Vendor Quote Cost", IsReadOnly = true)]
        [PXFormula(typeof(Current<RQBiddingVendor.curyTotalQuoteExtCost>))]
        public virtual Decimal? UsrVendQuoteCost { get; set; }
        public abstract class usrVendQuoteCost : PX.Data.BQL.BqlDecimal.Field<usrVendQuoteCost> { }
        #endregion
    }
}