using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KedgeFinCustDev.CA.Util
{
    // Constants for Approve Status.
    public static class ApproveStatusConstants
    {
        public const string OnHold = "OH";          // 擱置 (On Hold)
        public const string PendingApproval = "PA"; // 待簽核 (Pending Approval)
        public const string Approved = "AP";        // 已核可 (Approved)
        public const string Rejected = "RJ";        // 已拒絕 (Rejected)
    }
}
