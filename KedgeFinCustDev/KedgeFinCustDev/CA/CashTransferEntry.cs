using CAUtil = KedgeFinCustDev.CA.Util;

using PX.Data.BQL.Fluent;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.Common;
using PX.Objects.Common.Extensions;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using PX.Objects.Common.Bql;
using PX.Objects;
using PX.Objects.CA;

namespace PX.Objects.CA
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class CashTransferEntry_Extension : PXGraphExtension<CashTransferEntry>
    {
        private bool isApprovalClicked = false;
        private bool isRejectionClicked = false;

        public SelectFrom<CATransfer>.View TransferDataView;

        public override void Initialize()
        {
            base.Initialize();

            this.Action.MenuAutoOpen = true;

            // To Add "Approve" and "Reject" Buttons into Action Menu Folders. 
            this.Action.AddMenuAction(Approve);
            this.Action.AddMenuAction(Reject);
        }

        #region Build Action Buttons
        public PXAction<CATransfer> Approve;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Approve", Enabled = false)]
        protected virtual void approve()
        {
            var caTransfer = Base.Transfer.Current;
            var caTransferExt = PXCache<CATransfer>.GetExtension<CATransferExt>(caTransfer);

            caTransferExt.UsrApproveStatus = CAUtil.ApproveStatusConstants.Approved;            
            TransferDataView.Update(caTransfer);
            Base.Actions.PressSave();

            isApprovalClicked = true;
        }

        public PXAction<CATransfer> Reject;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Reject", Enabled = false)]
        protected virtual void reject()
        {
            var caTransfer = Base.Transfer.Current;
            var caTransferExt = PXCache<CATransfer>.GetExtension<CATransferExt>(caTransfer);

            caTransferExt.UsrApproveStatus = CAUtil.ApproveStatusConstants.Rejected;                       
            TransferDataView.Update(caTransfer);

            Base.Actions.PressSave();

            isRejectionClicked = true;
        }

        public PXAction<CATransfer> Action;
        [PXUIField(DisplayName = "Actions",  MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(SpecialType = PXSpecialButtonType.ActionsFolder)]        
        protected void action()
        {   
        }
        #endregion


        #region Event Handlers
        

        protected virtual void _(Events.RowSelected<CATransfer> e)
        {
            var transfer = e.Row as CATransfer;

            if (transfer == null)
            {
                return;
            }
                        
            var transferExt = PXCache<CATransfer>.GetExtension<CATransferExt>(transfer);
                        
            if (transfer.Status.ToLower() == "b")
            {
                PXUIFieldAttribute.SetReadOnly(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.descr>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.transferNbr>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.outAccountID>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.outDate>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.outExtRefNbr>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.curyTranOut>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.inAccountID>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.inDate>(e.Cache, transfer, true);
                PXUIFieldAttribute.SetReadOnly<CATransfer.inExtRefNbr>(e.Cache, transfer, true);

                Base.Expenses.AllowInsert = false;
                Base.Expenses.AllowUpdate = false;
                Base.Expenses.AllowDelete = false;
            }
            else
            {
                PXUIFieldAttribute.SetReadOnly<CATransfer.transferNbr>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.descr>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.outAccountID>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.outDate>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.outExtRefNbr>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.curyTranOut>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.inAccountID>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.inDate>(e.Cache, transfer, false);
                PXUIFieldAttribute.SetReadOnly<CATransfer.inExtRefNbr>(e.Cache, transfer, false);

                Base.Expenses.AllowInsert = true;
                Base.Expenses.AllowUpdate = true;
                Base.Expenses.AllowDelete = true;
            }

            if (isApprovalClicked)
            {
                transferExt.UsrApproveStatus = CAUtil.ApproveStatusConstants.Approved;
                PXUIFieldAttribute.SetReadOnly<CATransfer.hold>(e.Cache, transfer, false);
                Base.Release.SetEnabled(true);
                isApprovalClicked = false;
            }

            if (isRejectionClicked)
            {
                transferExt.UsrApproveStatus = CAUtil.ApproveStatusConstants.Rejected;
                PXUIFieldAttribute.SetReadOnly<CATransfer.hold>(e.Cache, transfer, true);
                Base.Release.SetEnabled(false);
                isRejectionClicked = false;
            }
            
            if (!isApprovalClicked && !isRejectionClicked)
            {
                if (transfer.TransferNbr.ToUpper().StartsWith("CA"))    // Update.
                {
                    if (!transfer.Hold.Value)   // Un Hold.
                    {
                        Approve.SetEnabled(true);
                        Reject.SetEnabled(true);

                        if (transferExt.UsrApproveStatus == CAUtil.ApproveStatusConstants.Approved)
                        {
                            PXUIFieldAttribute.SetReadOnly<CATransfer.hold>(e.Cache, transfer, true);
                            Base.Release.SetEnabled(true);
                        }
                        else if (transferExt.UsrApproveStatus == CAUtil.ApproveStatusConstants.Rejected)
                        {
                            PXUIFieldAttribute.SetReadOnly<CATransfer.hold>(e.Cache, transfer, false);
                            Base.Release.SetEnabled(false);
                        }
                        else if (transferExt.UsrApproveStatus == CAUtil.ApproveStatusConstants.PendingApproval)
                        {
                            PXUIFieldAttribute.SetReadOnly<CATransfer.hold>(e.Cache, transfer, true);
                            Base.Release.SetEnabled(false);
                        }
                        else
                        {
                            transferExt.UsrApproveStatus = CAUtil.ApproveStatusConstants.PendingApproval;
                            PXUIFieldAttribute.SetReadOnly<CATransfer.hold>(e.Cache, transfer, true);
                            transfer.Hold = false;
                        }
                    }
                    else
                    {
                        Approve.SetEnabled(false);
                        Reject.SetEnabled(false);
                        transferExt.UsrApproveStatus = null;
                    }
                }
                else    // New.
                {
                    Base.Release.SetEnabled(false);
                    Approve.SetEnabled(false);
                    Reject.SetEnabled(false);
                    PXUIFieldAttribute.SetReadOnly<CATransfer.hold>(e.Cache, transfer, false);

                    if (!transfer.Hold.Value)    // Un Hold.
                    {
                        transferExt.UsrApproveStatus = CAUtil.ApproveStatusConstants.PendingApproval;
                    }
                    else
                    {
                        transferExt.UsrApproveStatus = null;
                    }
                }
            }
        }

        #endregion

    }
}