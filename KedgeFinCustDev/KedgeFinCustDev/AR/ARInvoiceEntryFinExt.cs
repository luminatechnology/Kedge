using PX.Data;
using PX.Objects.PM;
using RC.Util;
using RCGV.GV;
using RCGV.GV.DAC;
using System.Collections.Generic;

/**
 * ====2021-08-19:12207====Alton
 * 1.新增一個Tab, 顯示相關聯的銷項發票資訊, 欄位如下
 *   參照號碼(GVArGuiInvoice.GuiInvoiceCD)
 *   發票號碼(GVArGuiInvoice.GuiInvoiceNbr)
 *   發票日期(GVArGuiInvoice.InvoiceDate)
 *   客戶名稱(GVArGuiInvoice.CustName)
 *   客戶統編(GVArGuiInvoice.CustUniformNumber)
 *   發票類型(GVArGuiInvoice.GuiType)
 *   描述(GVArGuiInvoice.DocDesc)
 *   發票金額(GVArGuiInvoice.SalesAmt)
 *   發票稅額(GVArGuiInvoice.TaxAmt)
 *   發票總額(GVArGuiInvoice.TotalAmt)
 * 2.此Tab欄位資料皆不可新增/修改/刪除
 * 3.請移除ARInvoice.UsrArGuiInvoiceNbr欄位, 畫面也不顯示
 * 
 * ====2022/07/25====Jeff
 * mantis [#0012341].
 * 
 * **/
namespace PX.Objects.AR
{
    public class ARInvoiceEntryFinExt : PXGraphExtension<ARInvoiceEntry>
    {
        #region View
        public PXSelectJoinGroupBy<GVArGuiInvoice,
            InnerJoin<GVArGuiInvoiceDetail,
                On<GVArGuiInvoice.guiInvoiceID, Equal<GVArGuiInvoiceDetail.guiInvoiceID>>>,
            Where<GVArGuiInvoiceDetail.arRefNbr, Equal<Current<ARInvoice.refNbr>>,
                And<GVArGuiInvoiceDetail.aRDocType, Equal<Current<ARInvoice.docType>>>>,
            Aggregate<GroupBy<GVArGuiInvoice.guiInvoiceCD>>
            > GVArGuiInvoices;
        #endregion

        #region Override Methods
        public override void Initialize()
        {
            base.Initialize();

            Base.report.AddMenuAction(aRVoucherEntryPrevRpt);
        }
        #endregion

        #region Actions
        public PXAction<ARInvoice> aRVoucherEntryPrevRpt;
        [PXButton(), PXUIField(DisplayName = "應收憑單分錄預覽報表", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual void ARVoucherEntryPrevRpt()
        {
            var invoice = Base.Document.Current;

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["DocType"] = invoice.DocType,
                ["RefNbrFrom"] = invoice.RefNbr,
                ["RefNbrTo"] = invoice.RefNbr,
                ["DocDateFrom"] = invoice.DocDate?.ToString("yyyy/MM/dd"),
                ["DocDateTo"] = invoice.DocDate?.ToString("yyyy/MM/dd"),
                ["CreatedByID"] = null,
            };

            throw new PXReportRequiredException(parameters, "KG606003", PXBaseRedirectException.WindowMode.New, null);
        }

        #region HyperLink
        public PXAction<GVArGuiInvoice> ViewGVArInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGVArInvoice()
        {
            GVArGuiInvoice invoice = GVArGuiInvoices.Current;
            new HyperLinkUtil<GVArInvoiceEntry>(invoice, true);
        }
        #endregion

        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<ARInvoice> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            aRVoucherEntryPrevRpt.SetEnabled(e.Row?.Released != true);
            bool isNonProjcet = ProjectDefaultAttribute.IsNonProject(e.Row?.ProjectID);
            PXUIFieldAttribute.SetRequired<ARTran.costCodeID>(Base.Transactions.Cache, !isNonProjcet);
            //2022-12-26 稅務類別加上 * 但不檢核
            PXUIFieldAttribute.SetRequired<ARTran.taxCategoryID>(Base.Transactions.Cache, true);
        }
        #endregion

        }
}
