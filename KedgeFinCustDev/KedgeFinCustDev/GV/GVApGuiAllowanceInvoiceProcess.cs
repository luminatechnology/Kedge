using System;
using System.Collections.Generic;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.GL;
using RCGV.GV;
using RCGV.GV.DAC;
using RCGV.GV.Util;
using RC.Util;
using static RCGV.GV.GVApCmInvoiceEntry;


/**
 * ====2021-04-23:口頭 ====Alton
 * 選擇的發票要是以確認的發票
 * 
 * ====2021-06-04:12077====Alton
 * 資料來源追加
 * - status = balanced
 * - usrAccConfirmNbr is not null
 * **/
namespace GV
{
    public class GVApGuiAllowanceInvoiceProcess : PXGraph<GVApGuiAllowanceInvoiceProcess>
    {
        public GVApGuiAllowanceInvoiceProcess()
        {
            DetailsView.SetProcessCaption("開立折讓單");
            DetailsView.SetProcessAllVisible(false);
        }

        #region Action
        public PXCancel<ParamTable> Cancel;
        #endregion

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXFilteredProcessing<APRegisterV, ParamTable,
            Where<True, Equal<True>,
             And2<Where<Current2<ParamTable.refNbrFrom>, IsNull,
                    Or<Current2<ParamTable.refNbrFrom>, LessEqual<APRegisterV.refNbr>>>,
                And2<Where<Current2<ParamTable.refNbrTo>, IsNull,
                    Or<Current2<ParamTable.refNbrTo>, GreaterEqual<APRegisterV.refNbr>>>,
                And2<Where<Current2<ParamTable.docDateFrom>, IsNull,
                    Or<Current2<ParamTable.docDateFrom>, LessEqual<APRegisterV.docDate>>>,
                And2<Where<Current2<ParamTable.docDateTo>, IsNull,
                    Or<Current2<ParamTable.docDateTo>, GreaterEqual<APRegisterV.docDate>>>,
                And2<Where<Current2<ParamTable.projectID>, IsNull,
                    Or<Current2<ParamTable.projectID>, Equal<APRegisterV.projectID>>>,
                And<Where<Current2<ParamTable.vendorID>, IsNull,
                    Or<Current2<ParamTable.vendorID>, Equal<APRegisterV.vendorID>>>
                    >>>>>>>> DetailsView;
        #endregion

        #region HyperLink
        #region View Invoice
        public PXAction<APRegisterV> ViewInv;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewInv()
        {
            APRegisterV item = DetailsView.Current;
            APInvoice invoice = GetAPInvoiceByKey(item.OriDecutionRefNbr, item.OriDecutionDocType);
            new HyperLinkUtil<APInvoiceEntry>(invoice, true);
        }
        #endregion

        #region View DebitAdj
        public PXAction<APRegisterV> ViewDebitAdj;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewDebitAdj()
        {
            APRegisterV item = DetailsView.Current;
            APInvoice invoice = GetAPInvoiceByKey(item.RefNbr, item.DocType);
            new HyperLinkUtil<APInvoiceEntry>(invoice, true);
        }
        #endregion

        #endregion

        #region Event

        protected virtual void _(Events.RowSelected<ParamTable> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            DetailsView.SetProcessDelegate(list =>
                CreateAllowanceInvoice((GVApGuiAllowanceInvoiceProcess)e.Cache.Graph, list, row.GuiType));
        }

        protected virtual void _(Events.FieldUpdated<ParamTable, ParamTable.guiType> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            if (e.NewValue == null) row.GuiType = (string)e.OldValue;
        }
        #endregion

        #region Method
        public static void CreateAllowanceInvoice(GVApGuiAllowanceInvoiceProcess graph, List<APRegisterV> datas, string guiType)
        {
            Dictionary<String, bool> checks = graph.CheckCmAmt(datas);
            for (int i = 0; i < datas.Count; i++)
            {
                APRegisterV data = datas[i];
                if (!checks[data.GuiInvoiceNbr])
                {
                    PXProcessing.SetError(i, "折讓金額超過發票金額");
                    continue;
                }
                try
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        GVApCmInvoiceEntry entry = PXGraph.CreateInstance<GVApCmInvoiceEntry>();
                        graph.CreateGvApGuiCmInvoice(entry, data, guiType);
                        graph.CreateGvApGuiCmInvoiceDetail(entry, data);
                        entry.Persist();
                        GVApGuiCmInvoice cmInvoice = entry.GuiCmInvoice.Current;
                        PXUpdate<Set<APRegisterGVExt.usrApGuiCmInvoiceNbr, Required<APRegisterGVExt.usrApGuiCmInvoiceNbr>>,
                            APRegister,
                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>,
                            And<APRegister.docType, Equal<Required<APRegister.docType>>>>>
                            .Update(new PXGraph(), cmInvoice.GuiCmInvoiceNbr, data.RefNbr, data.DocType);
                        ts.Complete();
                        PXProcessing.SetInfo(i, "Process Success");
                    }
                }
                catch (Exception e)
                {
                    PXProcessing.SetError(i, e);
                }
            }
        }

        public void CreateGvApGuiCmInvoice(GVApCmInvoiceEntry entry, APRegisterV data, string guiType)
        {
            APRegisterExt registerExt = PXCache<APRegister>.GetExtension<APRegisterExt>(data);
            APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(data);
            PXCache hCache = entry.GuiCmInvoice.Cache;
            GVApGuiCmInvoice header = (GVApGuiCmInvoice)hCache.CreateInstance();
            header = entry.GuiCmInvoice.Insert(header);
            hCache.SetValueExt<GVApGuiCmInvoice.registrationCD>(header, GetGVRegistrationCD(data.BranchID));
            hCache.SetValueExt<GVApGuiCmInvoice.guiType>(header, guiType);
            hCache.SetValueExt<GVApGuiCmInvoice.vendorID>(header, data.VendorID);
            Location l = (Location)PXSelectorAttribute.Select<APRegisterV.vendorLocationID>(DetailsView.Cache, data);
            hCache.SetValueExt<GVApGuiCmInvoice.vendorUniformNumber>(header, l?.TaxRegistrationID);
            hCache.SetValueExt<GVApGuiCmInvoice.vendorAddress>(header, data.VendorAddress);
            hCache.SetValueExt<GVApGuiCmInvoice.taxCode>(header, GVList.GVTaxCode.TAXABLE);
            hCache.SetValueExt<GVApGuiCmInvoice.salesAmt>(header, data.CuryLineTotal);
            hCache.SetValueExt<GVApGuiCmInvoice.taxAmt>(header, data.CuryTaxTotal);
            hCache.SetValueExt<GVApGuiCmInvoice.totalAmt>(header, data.CuryOrigDocAmt);
            hCache.SetValueExt<GVApGuiCmInvoice.invoiceDate>(header, data.DocDate);
            //hCache.SetValueExt<GVApGuiCmInvoice.declareYear>(header, data.DocDate?.Year);
            //hCache.SetValueExt<GVApGuiCmInvoice.declareMonth>(header, data.DocDate?.Month);
            hCache.SetValueExt<GVApGuiCmInvoice.hold>(header, false);
            //hCache.SetValueExt<GVApGuiCmInvoice.status>(header, GVList.GVStatus.OPEN);
            hCache.SetValueExt<GVApGuiCmInvoice.remark>(header, data.DocDesc);

            String accConfirmNbr = "";
            //扣款請將該借方調整的APRegister.UsrAccConfirmNbr去掉結尾的‘D‘放在此欄位上
            accConfirmNbr = registerFinExt?.UsrAccConfirmNbr;
            if (registerExt.UsrIsDeductionDoc == true)
            {
                accConfirmNbr = accConfirmNbr?.Replace("D", "");
            }
            hCache.SetValueExt<GVApGuiCmInvoice.accConfirmNbr>(header, accConfirmNbr);

            entry.GuiCmInvoice.Update(header);
        }

        public void CreateGvApGuiCmInvoiceDetail(GVApCmInvoiceEntry entry, APRegisterV data)
        {
            PXCache lCache = entry.GuiCmInvoiceLine.Cache;
            GVApGuiCmInvoiceLineV line = (GVApGuiCmInvoiceLineV)lCache.CreateInstance();
            //line = entry.GuiCmInvoiceLine.Insert(line);
            lCache.SetValueExt<GVApGuiCmInvoiceLineV.apGuiInvoiceNbr>(line, data.GuiInvoiceNbr);
            lCache.SetValueExt<GVApGuiCmInvoiceLineV.itemDesc>(line, data.DocDesc);
            lCache.SetValueExt<GVApGuiCmInvoiceLineV.unitPrice>(line, data.CuryLineTotal);
            lCache.SetValueExt<GVApGuiCmInvoiceLineV.quantity>(line, 1m);
            lCache.SetValueExt<GVApGuiCmInvoiceLineV.salesAmt>(line, data.CuryLineTotal);
            lCache.SetValueExt<GVApGuiCmInvoiceLineV.taxAmt>(line, data.CuryTaxTotal);
            line = entry.GuiCmInvoiceLine.Update(line);
        }

        private Dictionary<String, bool> CheckCmAmt(List<APRegisterV> datas)
        {
            Dictionary<String, bool> checks = new Dictionary<String, bool>();
            Dictionary<String, decimal> selectTotalAmt = new Dictionary<String, decimal>();
            Dictionary<String, decimal> invoiceAmt = new Dictionary<String, decimal>();
            //整理Selected By GuiInvoiceNbr
            foreach (APRegisterV item in datas)
            {
                if (selectTotalAmt.ContainsKey(item.GuiInvoiceNbr))
                {
                    selectTotalAmt[item.GuiInvoiceNbr] += item.CuryLineTotal ?? 0m;
                }
                else
                {
                    //取得現存的折讓金額
                    decimal total = 0;
                    foreach (GVApGuiCmInvoiceLine line in GetCmInvioceLineByGuiInvoiceNbr(item.GuiInvoiceNbr))
                    {
                        total += line.SalesAmt ?? 0;
                    }
                    //添加所選取的折讓金額
                    total += item.CuryLineTotal ?? 0m;
                    selectTotalAmt.Add(item.GuiInvoiceNbr, total);
                    //取得發票金額
                    foreach (GVApGuiInvoice invoice in GetGuiInvioceLineByGuiInvoiceNbr(item.GuiInvoiceNbr))
                    {
                        invoiceAmt.Add(invoice.GuiInvoiceNbr, invoice.SalesAmt ?? 0m);
                    }
                }

            }
            foreach (string key in selectTotalAmt.Keys)
            {
                //折讓金額是否小於等於發票金額
                checks.Add(key, invoiceAmt[key] >= selectTotalAmt[key]);
            }
            return checks;
        }

        #endregion

        #region BQL
        private APInvoice GetAPInvoiceByKey(string refNbr, string docType)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>
                , And<APInvoice.docType, Equal<Required<APInvoice.docType>>>>>
                .Select(this, refNbr, docType);
        }

        private APTaxTran GetAPTaxTran(string refNbr, string docType)
        {
            return PXSelect<APTaxTran,
                Where<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>,
                And<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>>>>
                .Select(this, refNbr, docType);
        }

        private string GetGVRegistrationCD(int? branchID)
        {
            GVRegistration r = PXSelectJoin<GVRegistration,
                    InnerJoin<GVRegistrationBranch, On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>,
                    InnerJoin<Branch, On<GVRegistrationBranch.bAccountID, Equal<Branch.bAccountID>>>>,
                Where<Branch.branchID, Equal<Required<Branch.branchID>>>
                >.Select(this, branchID);
            return r?.RegistrationCD;
        }

        private PXResultset<GVApGuiCmInvoiceLine> GetCmInvioceLineByGuiInvoiceNbr(string guiInvoiceNbr)
        {
            return PXSelect<GVApGuiCmInvoiceLine,
                Where<GVApGuiCmInvoiceLine.apGuiInvoiceNbr, Equal<Required<GVApGuiCmInvoiceLine.apGuiInvoiceNbr>>>>
                .Select(this, guiInvoiceNbr);
        }

        private PXResultset<GVApGuiInvoice> GetGuiInvioceLineByGuiInvoiceNbr(string guiInvoiceNbr)
        {
            return PXSelect<GVApGuiInvoice,
                Where<GVApGuiInvoice.guiInvoiceNbr, Equal<Required<GVApGuiInvoice.guiInvoiceNbr>>>>
                .Select(this, guiInvoiceNbr);
        }
        #endregion

        #region Table
        #region ParamTable
        [Serializable]
        public class ParamTable : IBqlTable
        {
            #region Process Info
            #region GuiType
            [PXString()]
            [PXUIField(DisplayName = "Gui Type", Required = true)]
            [PXUnboundDefault(typeof(guiType23), PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [PXSelector(
                typeof(Search<GVGuiType.guiTypeCD, Where<GVGuiType.guiTypeCD, In3<guiType23, guiType24, guiType29>>>),
                typeof(GVGuiType.guiTypeCD),
                typeof(GVGuiType.guiTypeDesc),
                DescriptionField = typeof(GVGuiType.guiTypeDesc)
            )]
            public virtual string GuiType { get; set; }
            public abstract class guiType : PX.Data.BQL.BqlString.Field<guiType> { }
            #endregion

            #endregion
            #region Filter
            #region RefNbrFrom
            [PXString()]
            [PXUIField(DisplayName = "From RefNbr")]
            public virtual string RefNbrFrom { get; set; }
            public abstract class refNbrFrom : PX.Data.BQL.BqlString.Field<refNbrFrom> { }
            #endregion

            #region RefNbrTo
            [PXString()]
            [PXUIField(DisplayName = "To RefNbr")]
            public virtual string RefNbrTo { get; set; }
            public abstract class refNbrTo : PX.Data.BQL.BqlString.Field<refNbrTo> { }
            #endregion

            #region DocDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "From DocDate")]
            public virtual DateTime? DocDateFrom { get; set; }
            public abstract class docDateFrom : PX.Data.BQL.BqlDateTime.Field<docDateFrom> { }
            #endregion

            #region DocDateTo
            [PXDate()]
            [PXUIField(DisplayName = "To DocDate")]
            public virtual DateTime? DocDateTo { get; set; }
            public abstract class docDateTo : PX.Data.BQL.BqlDateTime.Field<docDateTo> { }
            #endregion

            #region ProjectID
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            [PXUIField(DisplayName = "Project ID")]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region Vendor
            [VendorActive(
               Visibility = PXUIVisibility.SelectorVisible,
               DescriptionField = typeof(PX.Objects.AP.Vendor.acctName),
               CacheGlobal = true,
               Filterable = true,
               Required = false
               )]
            [PXDefault]
            [PXUIField(DisplayName = "Vendor")]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion
            #endregion

            #region Const
            /// <summary> 三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之進貨退出或折讓證明單 </summary>
            public const String GuiType23 = "23";

            /// <summary> 二聯式收銀機統一發票及載有稅額之其他憑證之進貨退出或折讓證明單 </summary>
            public const String GuiType24 = "24";

            /// <summary> 海關退還溢繳營業稅申報單 </summary>
            public const String GuiType29 = "29";
            /// <summary> 三聯式、電子計算機、三聯式收銀機統一發票及一般稅額計算之電子發票之進貨退出或折讓證明單 </summary>
            public class guiType23 : PX.Data.BQL.BqlString.Constant<guiType23> { public guiType23() : base(GuiType23) {; } }
            /// <summary> 二聯式收銀機統一發票及載有稅額之其他憑證之進貨退出或折讓證明單 </summary>
            public class guiType24 : PX.Data.BQL.BqlString.Constant<guiType24> { public guiType24() : base(GuiType24) {; } }
            /// <summary> 海關退還溢繳營業稅申報單 </summary>
            public class guiType29 : PX.Data.BQL.BqlString.Constant<guiType29> { public guiType29() : base(GuiType29) {; } }
            #endregion
        }
        #endregion

        #region APRegisterV
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(
            Select5<APRegister,
                InnerJoin<APInvoice,
                    On<APInvoice.refNbr, Equal<APRegister.refNbr>,
                        And<APInvoice.docType, Equal<APRegister.docType>>>,
                InnerJoin<GVApGuiInvoice,
                    On<IsNull<APRegisterExt.usrOriDeductionRefNbr, APRegister.origRefNbr>, Equal<GVApGuiInvoice.refNbr>,
                        And<GVApGuiInvoice.invoiceType, Equal<invoice>,
                        And<GVApGuiInvoice.status, In3<GVList.GVStatus.open, GVList.GVStatus.hold>>>>>
                    >,
                Where<APRegister.docType, Equal<APDocType.debitAdj>,
                  And<APRegister.status, In3<APDocStatus.balanced, APDocStatus.open>,
                  And<APRegisterFinExt.usrIsConfirm, Equal<True>,
                  And<APRegisterFinExt.usrAccConfirmNbr, IsNotNull,
                  And<APRegisterGVExt.usrApGuiCmInvoiceNbr, IsNull>>>>>,
                Aggregate<GroupBy<APRegister.refNbr>>
                >)
            , Persistent = false)]
        public class APRegisterV : APRegister
        {
            #region CuryTaxTotal
            [PXDBDecimal(BqlField = typeof(APInvoice.curyTaxTotal))]
            [PXUIField(DisplayName = "Cury Tax Total")]
            public virtual decimal? CuryTaxTotal { get; set; }
            public abstract class curyTaxTotal : PX.Data.BQL.BqlDecimal.Field<curyTaxTotal> { }
            #endregion

            #region CuryLineTotal
            [PXDBDecimal(BqlField = typeof(APInvoice.curyLineTotal))]
            [PXUIField(DisplayName = "Cury Line Total")]
            public virtual decimal? CuryLineTotal { get; set; }
            public abstract class curyLineTotal : PX.Data.BQL.BqlDecimal.Field<curyLineTotal> { }
            #endregion

            #region unbound
            #region OriDecutionRefNbr
            [PXString()]
            [PXUIField(DisplayName = "Ori Deduction RefNbr")]
            [PXFormula(typeof(IsNull<APRegisterExt.usrOriDeductionRefNbr, APRegister.origRefNbr>
                ))]
            public virtual string OriDecutionRefNbr { get; set; }
            public abstract class oriDecutionRefNbr : PX.Data.BQL.BqlString.Field<oriDecutionRefNbr> { }
            #endregion

            #region OriDecutionDocType
            [PXString()]
            [PXUIField(DisplayName = "Ori Decution Doc Type")]
            [PXFormula(typeof(IsNull<APRegisterExt.usrOriDeductionDocType, APRegister.origDocType>
                ))]
            public virtual string OriDecutionDocType { get; set; }
            public abstract class oriDecutionDocType : PX.Data.BQL.BqlString.Field<oriDecutionDocType> { }
            #endregion
            #endregion

            #region GuiInvoiceID
            //[PXString()]
            [PXDBInt(BqlField = typeof(GVApGuiInvoice.guiInvoiceID))]
            [PXUIField(DisplayName = "Gui Invoice Nbr")]
            public virtual int? GuiInvoiceID { get; set; }
            public abstract class guiInvoiceID : PX.Data.BQL.BqlInt.Field<guiInvoiceID> { }
            #endregion

            #region GuiInvoiceNbr
            [PXString()]
            //[PXDBString(BqlField = typeof(GVApGuiInvoice.guiInvoiceNbr))]
            [PXUIField(DisplayName = "Gui Invoice Nbr")]
            [PXUnboundDefault(typeof(Search<GVApGuiInvoice.guiInvoiceNbr, Where<GVApGuiInvoice.guiInvoiceID, Equal<Current<guiInvoiceID>>>>))]
            public virtual string GuiInvoiceNbr { get; set; }
            public abstract class guiInvoiceNbr : PX.Data.BQL.BqlString.Field<guiInvoiceNbr> { }
            #endregion

            #region TotalAmt
            //[PXDBDecimal(BqlField = typeof(GVApGuiInvoice.totalAmt))]
            [PXDecimal]
            [PXUnboundDefault(typeof(Search<GVApGuiInvoice.totalAmt, Where<GVApGuiInvoice.guiInvoiceID, Equal<Current<guiInvoiceID>>>>))]
            [PXUIField(DisplayName = "Total Amount")]
            public virtual decimal? TotalAmt { get; set; }
            public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt> { }
            #endregion

            #region VendorAddress
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Vendor Address")]
            [PXUnboundDefault(typeof(Search<GVApGuiInvoice.vendorAddress, Where<GVApGuiInvoice.guiInvoiceID, Equal<Current<guiInvoiceID>>>>))]
            public virtual string VendorAddress { get; set; }
            public abstract class vendorAddress : PX.Data.BQL.BqlString.Field<vendorAddress> { }
            #endregion

            #region Const
            /// <summary> 支票 </summary>
            public const String Invoice = "I";
            /// <summary> 支票 </summary>
            public class invoice : PX.Data.BQL.BqlString.Constant<invoice> { public invoice() : base(Invoice) {; } }
            #endregion
        }
        #endregion
        #endregion


    }
}