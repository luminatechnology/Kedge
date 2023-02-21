using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using PX.Data;
using PX.Data.EP;
using PX.Common;
using PX.Objects.AP.BQL;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.CA;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.DR;
using PX.Objects.AP.Overrides.APDocumentRelease;
using PX.Objects.AR;
using PX.Objects.Common.DataIntegrity;
using PX.Objects.Common.Exceptions;
using PX.Objects.PO.LandedCosts;
using PX.Objects.Common.Tools;
using PX.Objects.GL.FinPeriods.TableDefinition;
using Amount = PX.Objects.AR.ARReleaseProcess.Amount;
using PX.Objects.Common.EntityInUse;
using PX.Objects.Common.Extensions;
using PX.Objects;
using PX.Objects.AP;

namespace PX.Objects.AP
{
    public class APDocumentReleaseFinExt : PXGraphExtension<APDocumentRelease>
    {
        
        protected virtual IEnumerable aPDocumentList()
        {
			PXProcessingJoin<
			BalancedAPDocument,
				LeftJoin<APInvoice,
					On<APInvoice.docType, Equal<BalancedAPDocument.docType>,
					And<APInvoice.refNbr, Equal<BalancedAPDocument.refNbr>>>,
				LeftJoin<APPayment,
					On<APPayment.docType, Equal<BalancedAPDocument.docType>,
					And<APPayment.refNbr, Equal<BalancedAPDocument.refNbr>>>,
				InnerJoinSingleTable<Vendor,
					On<Vendor.bAccountID, Equal<BalancedAPDocument.vendorID>>,
				LeftJoin<
					APAdjust, On<APAdjust.adjgDocType, Equal<BalancedAPDocument.docType>,
					And<APAdjust.adjgRefNbr, Equal<BalancedAPDocument.refNbr>,
					And<APAdjust.released, NotEqual<True>,
					And<APAdjust.hold, Equal<boolFalse>>>>>>>>>,
				Where2<
					Match<Vendor, Current<AccessInfo.userName>>,
							 And<APRegister.hold, Equal<boolFalse>,
							 And<APRegister.voided, Equal<boolFalse>,
							 And<APRegister.scheduled, Equal<boolFalse>,
							 And<APRegister.approved, Equal<boolTrue>,
							 And<APRegister.docType, NotEqual<APDocType.check>,
							 And<APRegister.docType, NotEqual<APDocType.quickCheck>,
							 And<APRegisterFinExt.usrIsConfirm,Equal<True>,
					And<Where<
						BalancedAPDocument.released, Equal<boolFalse>,
									Or<BalancedAPDocument.openDoc, Equal<boolTrue>,
						And<APAdjust.adjdRefNbr, IsNotNull>>>>>>>>>>>>>
						query = new PXProcessingJoin<
			BalancedAPDocument,
				LeftJoin<APInvoice,
					On<APInvoice.docType, Equal<BalancedAPDocument.docType>,
					And<APInvoice.refNbr, Equal<BalancedAPDocument.refNbr>>>,
				LeftJoin<APPayment,
					On<APPayment.docType, Equal<BalancedAPDocument.docType>,
					And<APPayment.refNbr, Equal<BalancedAPDocument.refNbr>>>,
				InnerJoinSingleTable<Vendor,
					On<Vendor.bAccountID, Equal<BalancedAPDocument.vendorID>>,
				LeftJoin<
					APAdjust, On<APAdjust.adjgDocType, Equal<BalancedAPDocument.docType>,
					And<APAdjust.adjgRefNbr, Equal<BalancedAPDocument.refNbr>,
					And<APAdjust.released, NotEqual<True>,
					And<APAdjust.hold, Equal<boolFalse>>>>>>>>>,
				Where2<
					Match<Vendor, Current<AccessInfo.userName>>,
							 And<APRegister.hold, Equal<boolFalse>,
							 And<APRegister.voided, Equal<boolFalse>,
							 And<APRegister.scheduled, Equal<boolFalse>,
							 And<APRegister.approved, Equal<boolTrue>,
							 And<APRegister.docType, NotEqual<APDocType.check>,
							 And<APRegister.docType, NotEqual<APDocType.quickCheck>,
							 And<APRegisterFinExt.usrIsConfirm, Equal<True>,
					And<Where<
						BalancedAPDocument.released, Equal<boolFalse>,
									Or<BalancedAPDocument.openDoc, Equal<boolTrue>,
						And<APAdjust.adjdRefNbr, IsNotNull>>>>>>>>>>>>>(Base);

			//query.WhereAnd<Where<APRegisterFinExt.usrIsConfirm,Equal<True>>>();
            return query.Select();
        }

        #region Event Handlers

        #endregion
    }
}