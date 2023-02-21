using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;
using CRLocation = PX.Objects.CR.Standalone.Location;
using IRegister = PX.Objects.CM.IRegister;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.Common.Abstractions;
using PX.Objects.Common.MigrationMode;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PX.Objects.PM
{
    public    class PMChangeOrderExt : PXCacheExtension<PX.Objects.PM.PMChangeOrder>
    {
        #region  UsrRedirect
        [PXString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "電子簽核")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String UsrRedirect
        {
            get
            {
                if (UsrReturnUrl != null)
                {
                    return "電子簽核";
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }
        public abstract class usrRedirect : PX.Data.BQL.BqlString.Field<usrRedirect> { }
        #endregion
        #region UsrReturnUrl
        [PXDBString(2048)]
        [PXUIField(DisplayName = "ReturnUrl")]

        public virtual string UsrReturnUrl { get; set; }
        public abstract class usrReturnUrl : PX.Data.BQL.BqlString.Field<usrReturnUrl> { }
        #endregion

        #region UsrKGFlowUID
        [PXDBString(40)]
        [PXUIField(DisplayName = "UsrKGFlowUID")]
        public virtual string UsrKGFlowUID { get; set; }
        public abstract class usrKGFlowUID : PX.Data.BQL.BqlString.Field<usrKGFlowUID> { }
        #endregion

        #region UsrChangeReason
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "ChangeReason")]
       
        public virtual string UsrChangeReason { get; set; }
        public abstract class usrChangeReason : PX.Data.BQL.BqlString.Field<usrChangeReason> { }
        #endregion

        #region UsrOrderChangePeriod
        [PXDBInt]
        [PXDefault(1, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "OrderChangePeriod", IsReadOnly = true)]

        public virtual int? UsrOrderChangePeriod { get; set; }
        public abstract class usrOrderChangePeriod : PX.Data.BQL.BqlInt.Field<usrOrderChangePeriod> { }
        #endregion

        #region UsrApprovalStatus
        [PXDBString(2)]
        [PXUIField(DisplayName = "ApprovalStatus",IsReadOnly =true)]
        [ApprovalStatusList]
        public virtual string UsrApprovalStatus { get; set; }
        public abstract class usrApprovalStatus : PX.Data.BQL.BqlString.Field<usrApprovalStatus> { }
        #endregion
    }
}
