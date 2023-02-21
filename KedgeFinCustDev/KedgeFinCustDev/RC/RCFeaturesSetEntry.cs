using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.SiteMap.DAC;
using PX.Web.UI.Frameset.Model.DAC;
using RC.DAC;

namespace RC
{
    public class RCFeaturesSetEntry : PXGraph<RCFeaturesSetEntry>
    {
        public Guid defaultGuid = new Guid("00000000-0000-0000-0000-000000000000");

        //public PXSave<MasterTable> Save;
        //public PXCancel<MasterTable> Cancel;

        #region View
        public SelectFrom<SiteMap>.View SiteMapView;

        public PXSelect<RCFeaturesSet> Setings;
        protected IEnumerable setings()
        {
            RCFeaturesSet current = (RCFeaturesSet)PXSelect<RCFeaturesSet,
                                  Where<True, Equal<True>>,
                                  OrderBy<Desc<RCFeaturesSet.status>>>
                                  .SelectWindowed(this, 0, 1) ?? Setings.Insert();
            yield return current;
        }
        #endregion

        #region Action

        public PXAction<RCFeaturesSet> ModifyAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Modify", MapEnableRights = PXCacheRights.Insert, MapViewRights = PXCacheRights.Select)]
        public IEnumerable modifyAction(PXAdapter adapter)
        {
            Setings.Current.Status = 0;
            return adapter.Get();
        }
        public PXAction<RCFeaturesSet> EnableAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Enable")]
        public IEnumerable enableAction(PXAdapter adapter)
        {
            try
            {
                Setings.Current.Status = 1;
                base.Persist();
                PX.Common.PXPageCacheUtils.InvalidateCachedPages();

                UpdateSiteMap();
            }
            catch (Exception e) {
                Setings.Current.Status = 0;
                throw e;
            }
            return adapter.Get();
        }
        #endregion

        #region Event
        protected virtual void RCFeaturesSet_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            SetEnable();
        }

        #endregion

        #region Method
        private void SetEnable()
        {
            RCFeaturesSet row = Setings.Current;
            if (row == null) return;
            bool isEnable = (row.Status ?? 0) == 0;

            ModifyAction.SetEnabled(!isEnable);
            EnableAction.SetEnabled(isEnable);
            PXUIFieldAttribute.SetEnabled(Setings.Cache, Setings.Current, isEnable);
            PXUIFieldAttribute.SetEnabled<RCFeaturesSet.status>(Setings.Cache, Setings.Current, false);

        }

        private void UpdateSiteMap()
        {
            string workspace = "None";

            RCFeaturesSet set = Setings.Current;

            SiteMapView.WhereNew<Where<SiteMap.screenID.IsLike<@P.AsString>
                                       .Or<SiteMap.screenID.IsLike<@P.AsString>
                                           .Or<SiteMap.screenID.IsLike<@P.AsString>>>>>();

            foreach (SiteMap siteMap in SiteMapView.Select(new object[] { "LS%", "TW%", "NM%" }))
            {
                switch (siteMap.ScreenID)
                {
                    // NM
                    case "NM200000":
                    case "NM200001":
                    case "NM301000":
                    case "NM501000":
                    case "NM501001":
                    case "NM301002":
                        break;
                    // TW
                    case "TW101000":
                    case "TW201000":
                    case "TW202000":
                    case "TW203000":
                    case "TW401000":
                        workspace = (set.GuiPreference == true) ? "Taxes" : string.Empty;
                        break;
                    case "TW301000":
                        workspace = (set.ApInvoice == true) ? "Taxes" : string.Empty;
                        break;
                    case "TW302000":
                    case "TW601000":
                        workspace = (set.ArInvoice == true) ? "Taxes" : string.Empty;
                        break;
                    case "TW501000":
                    case "TW502000":
                        workspace = (set.DeclareGovInvoice == true) ? "Taxes" : string.Empty;
                        break;
                    case "TW503000":
                    case "TW504000":
                        workspace = (set.BankProIntegration == true) ? "Taxes" : string.Empty;
                        break;
                    // WH
                    case "TW402000":
                    case "TW505000":
                        workspace = (set.WithholdingTaxes == true) ? "Taxes" : string.Empty;
                        break;
                    // LS
                    case "LS201000":
                    case "LS301000":
                    case "LS401000":
                    case "LS601000":
                        workspace = (set.LedgerSettlement == true) ? "Finance" : string.Empty;
                        break;
                }

                Guid workspaceID = string.IsNullOrEmpty(workspace) ? Guid.Empty : GetWorkspace(workspace).WorkspaceID.Value;

                PXUpdate<Set<MUIScreen.workspaceID, Required<MUIScreen.workspaceID>>, MUIScreen,
                         Where<MUIScreen.nodeID, Equal<Required<SiteMap.nodeID>>>>.Update(this, workspaceID, siteMap.NodeID);
            }

            Redirector.Refresh(System.Web.HttpContext.Current);
        }

        protected virtual MUIWorkspace GetWorkspace(string title)
        {
            return SelectFrom<MUIWorkspace>.Where<MUIWorkspace.title.IsEqual<@P.AsString>>.View.ReadOnly.SelectSingleBound(this, null, title);
        }
        #endregion
    }
}