using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.RQ;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;
using Kedge.DAC;

namespace PX.Objects.RQ
{
    public class RQRequestExt : PXCacheExtension<PX.Objects.RQ.RQRequest>
    {
        #region UsrKGFlowUid
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "usrKGFlowUid")]
        public virtual String UsrKGFlowUID { get; set; }
        public abstract class usrKGFlowUID : PX.Data.BQL.BqlString.Field<usrKGFlowUID> { }
        #endregion

        #region UsrProjectID               
        [PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>),
                      PM.Messages.InactiveContract,
                      typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>,
                                Or<PMProject.nonProject, Equal<True>>>),
                      PM.Messages.ProjectInvisibleInModule,
                      typeof(PMProject.contractCD))]
        [ProjectBaseExt]
        [PXDefault()]
        public virtual int? UsrProjectID { get; set; }
        public abstract class usrProjectID : PX.Data.BQL.BqlInt.Field<usrProjectID> { }
        #endregion

        #region  UsrRedirect
        [PXString(50 ,IsUnicode = true)]
        [PXUIField(DisplayName = "電子簽核")]
        [PXDefault(PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual String UsrRedirect {
            get {
                if (UsrReturnUrl != null)
                {
                    return "電子簽核";
                }
                else {
                    return null;
                }
            }
            set {
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

        #region UsrExplain
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Explain")]
        public virtual string UsrExplain { get; set; }
        public abstract class usrExplain : PX.Data.BQL.BqlString.Field<usrExplain> { }
        #endregion
    }
}
