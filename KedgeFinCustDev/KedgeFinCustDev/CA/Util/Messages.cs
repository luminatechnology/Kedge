using PX.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KedgeFinCustDev.CA.Util
{
    [PXLocalizable()]
    public static class Messages
    {
        // Approve Status.
        public const string OnHold = "On Hold";                     // 擱置 (Hold)
        public const string PendingApproval = "Pending Approval";   // 待簽核 (Pending Approval)
        public const string Approved = "Approved";                  // 已核可 (Approved)
        public const string Rejected = "Rejected";                  // 已拒絕 (Rejected)
    }
}
