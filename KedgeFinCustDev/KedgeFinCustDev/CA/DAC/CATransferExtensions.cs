using CAUtil = KedgeFinCustDev.CA.Util;

using PX.Data.EP;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects;

using System.Collections.Generic;
using System;

namespace PX.Objects.CA
{
    public class CATransferExt : PXCacheExtension<PX.Objects.CA.CATransfer>
    {
        #region UsrApproveStatus
        [PXDBString(2, IsFixed = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Approve Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXStringList(
            new String[] 
            {
                CAUtil.ApproveStatusConstants.OnHold,
                CAUtil.ApproveStatusConstants.PendingApproval,
                CAUtil.ApproveStatusConstants.Approved,
                CAUtil.ApproveStatusConstants.Rejected
            }, 
            new String[]
            {
                CAUtil.Messages.OnHold,
                CAUtil.Messages.PendingApproval,
                CAUtil.Messages.Approved,
                CAUtil.Messages.Rejected
            })]

        public virtual string UsrApproveStatus { get; set; }
        public abstract class usrApproveStatus : PX.Data.BQL.BqlString.Field<usrApproveStatus> { }
        #endregion
    }
}