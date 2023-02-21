using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.PO;
using PX.Objects.PM;
using PX.Objects.AR;
using PX.Objects.CT;
using PX.Objects.CS;
using PX.Objects.GL;
using KG.Util;
using Kedge.DAC;
using RC.Util;
using RCGV.GV.DAC;
using Fin.DAC;

namespace Kedge
{
    /**
     *  ======2021/02/23 0011939 Edit By Althea=====
     *  1.Update�p���T�{:
     *      1.1 ��s�Ŀ諸APInvoice.PayDate = FilterForAP.VoucherDate
     *      1.2 ��s�Ŀ諸APInvoice������ADR(�ɤ�վ�) Invoice ��APInvoice.PayDate = FilterForAP.VoucherDate
     *      1.3 �[�ˮ�: ���ͥN�ǲ����X���e���ˮ֥N�ǲ����X�O�_����,�Y���h�����~�T��
     *          ErrorMsg: �w���ͥN�ǲ����X, ���୫�s�i��p���T�{!"
     *      1.4 �[�޿�: �Y�Ŀ諸APInvoice��KGBillPayment.PricingType = 'B'(��L�p��),�hPaymemtDate�������s�p��    
     *  2.Remove�p���w��
     *  3.Update���ͥN�ǲ����X:
     *      3.1 ��sDisplayName:���ͥN�ǲ����X
     *  4.Update���Y:
     *      4.1 �s�W�|�p�T�{�H���d�߱���,LOV
     *  5.ADD�����N�ǲ����X:
     *      5.1 ADD Action RemoveAccConfirmNbr
     *      5.2 �����X���ܸ߰ݽT�w�n�����N�ǲ����X�H
     *      5.3 �ˮ֤Ŀ諸APInvoice�O�_���͹L�N�ǲ����X,�Y�L�h���Error:�бN�H�U�p���沣�ͥN�ǲ����X!
     *      5.4 �M��APInvoice��UsrAccConfirmNbr���
     *      5.5 �N��APInvoice������ADR Invoice(�ɤ�վ�)��UsrAccConfirmNbr���M��
     *  6.ADD�L�b
     *      6.1 ADD Action Release
     *      6.2 �ˮ֤Ŀ諸APInvoice�O�_���͹L�N�ǲ����X,�Y�L�h���Error:�бN�H�U�p���沣�ͥN�ǲ����X!
     *      6.3 �N��APInvoice&����ADR Invoice(�ɤ�վ�)�@�_�L�b
     *      6.4 �NUsrAccConfirmNbr��s��Batch.UsrAccConfirmNbr
     *  
     *  ======2021/03/18 0011981 Edit By Althea=====
     *  1. Delete �����N�ǲ����X���s
     *  2. Merge �X�֭p���T�{&���ͥN�ǲ����X
     *  3. Delete ���s������
     *  4. ADD ���Ʋ��ͥN�ǲ����X,�n��log��DB/ Table: KGAccConfirmLog
     *  
     *  ===2021/05/14 :�f�Y ===Althea
     *  ���ͽT�{���X��method�̦h�[ �^�������KGVoucherL
     *  
     *  ===2021/05/31 :0012062 ===Althea
     *  �L���b�NDocDate��s �o��������M�~��
     *  
     *  ===2021/06/11 :0012062 ===Althea
     *  �����e�fAdd Log:
     *  �Y���i�p����O�qEP�Ӫ�,�����e�f�� ���F�����m�Ħ^
     *  �s�W�H�U�޿�:
     *  ��sAPInvoice.OrigRefNbr = null/ OrigDocType = Null/ OrigMoudle = AP
     *  ��sEPClaim.UsrIsRejectByAcct = True
     *  
     *  ===2021/07/01 : 0012116 === Althea
     *  �����p��I�ڤ�����޿貧�ʧאּ:
     *  �YBillPayment.PricingType ="B" , ����VocherDate
     *  �YBillPayment.PricingType = "A" , 0�� : ��VoucherDate,�j��0�� : ��GetLongTremPaymentDate(VoucherDate)
     *  
     *  ===2021/07/02 : 0012062 === Althea
     *  �����e�fAdd Log:
     *  �Y���i�p����Ӧ�EP
     *  �s�W�H�U�޿�:
     *  Update GVAPGuiInvoice:RefNbr = Null
     *  Update GVAPGuiInvoiceDetail:APRefNbr = Null/APDocType = Null
     *  Update TWNWHTTran:RefNbr = Null/DocType = Null
     *  Update KGBillPayment:RefNbr = Null
     *  Delete APInvoice
     *  Update EPClaim:UsrIsRejectByAcct = True/Release = False/Status = "R"(ReleasedStatus)/UsrApprovalStatus = "RJ"
     *  Update EPExpenseClaimDetails:Release = False /APDocType = Null/APRefNbr = Null/Status = "A" (ApprovedStatus)
     *  
     *  ===2021/09/13 :0012228 === Althea
     *  Add Logic:
     *  �p���T�{btn�s�W�N���|��paymentdate = Filter.voucherDate
     *  
     *  ===2021/09/28 :0012246 === Althea
     *  APReturn BTN Add Logic:
     *  if(AP���������ڭɤ�վ㤣��null)
     *  ��s���i�����ɤ�վ㪺Hold=true/ Status: Hold
    **/
    public class KGVoucherProcess_Extension : PXGraphExtension<KGVoucherProcess>
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.ProcessAction.SetVisible(false);
            Base.ERPVoid.SetVisible(false);
            Base.VoucherPreviewAction.SetVisible(false);
        }

        #region Select 
        public PXFilter<FilterForAP> Filters;

        public PXSelect<APInvoice, Where<True, Equal<True>>> apinovices;

        public PXSelect<ViewTableFin, Where<APInvoice.status, Equal<KGConst.balance>,//True, Equal<True>,
                     And2<Where<
                          ViewTableFin.projectID, Equal<Current2<FilterForAP.contractID>>,
                          Or<Current2<FilterForAP.contractID>, IsNull>>,
                    And2<Where<
                          ViewTableFin.docType, Equal<Current2<FilterForAP.docType>>,
                          Or<Current2<FilterForAP.docType>, IsNull>>,
                    And2<Where<
                          ViewTableFin.refNbr, Equal<Current2<FilterForAP.refNbr>>,
                          Or<Current2<FilterForAP.refNbr>, IsNull>>,
                    And2<Where<
                          ViewTableFin.vendorID, Equal<Current2<FilterForAP.vendor>>,
                          Or<Current2<FilterForAP.vendor>, IsNull>>,
                    And2<Where<
                          ViewTableFin.dueDate, GreaterEqual<Current2<FilterForAP.dueDateFrom>>,
                          Or<Current2<FilterForAP.dueDateFrom>, IsNull>>,
                    And2<Where<
                          ViewTableFin.dueDate, LessEqual<Current2<FilterForAP.dueDateTo>>,
                          Or<Current2<FilterForAP.dueDateTo>, IsNull>>,
                    And2<Where<
                          APRegisterExt.usrPONbr, Equal<Current2<FilterForAP.pONbr>>,
                          Or<Current2<FilterForAP.pONbr>, IsNull>>,
                    And2<Where<
                          APRegisterFinExt.usrConfirmBy, Equal<Current2<FilterForAP.usrConfirmBy>>,
                          Or<Current2<FilterForAP.usrConfirmBy>, IsNull>>,
                    And2<Where<
                          APRegisterExt.usrVoucherNo, GreaterEqual<Current2<FilterForAP.usrVoucherNoFrom>>,
                          Or<Current2<FilterForAP.usrVoucherNoFrom>, IsNull>>,
                    And2<Where<
                          APRegisterExt.usrVoucherNo, LessEqual<Current2<FilterForAP.usrVoucherNoTo>>,
                          Or<Current2<FilterForAP.usrVoucherNoTo>, IsNull>>,
                    And<Where2<
                            Where<
                              Current2<FilterForAP.usrIsConfirm>, Equal<FilterForAP.int_0>,
                              And<IsNull<APRegisterFinExt.usrIsConfirm, False>, Equal<False>>>,
                            Or<
                             Where<
                              Current2<FilterForAP.usrIsConfirm>, Equal<FilterForAP.int_1>,
                              And<APRegisterFinExt.usrIsConfirm, Equal<True>>>
                                >
                    >>>>>>>>>>>>>> Details;

        public PXSelect<KGBillPayment> Billpayments;

        public PXSelectJoin<KGVoucherH,
            InnerJoin<APRegister,
                On<APRegister.docType, Equal<KGVoucherH.docType>,
                And<APRegister.refNbr, Equal<KGVoucherH.refNbr>>>,
                LeftJoin<APInvoice,
                    On<APInvoice.docType, Equal<KGVoucherH.docType>,
                        And<APInvoice.refNbr, Equal<KGVoucherH.refNbr>>>,
                    LeftJoin<BAccount,
                        On<BAccount.bAccountID, Equal<APRegister.vendorID>>,
                        LeftJoin<KGBillSummary,
                            On<KGBillSummary.docType, Equal<KGVoucherH.docType>,
                                And<KGBillSummary.refNbr, Equal<KGVoucherH.refNbr>>>>>>>> AllVouchers;

        public PXSelect<KGAcctConfirmLog> Logs;
        #endregion

        #region Actions

        #region �p���T�{
        public PXAction<FilterForAP> ConfirmAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "�p���T�{")]
        protected IEnumerable confirmAction(PXAdapter adapter)
        {
            FilterForAP filter = Filters.Current;
            if (filter.VoucherDate == null)
                throw new Exception("�ж�g�ǲ����!");
            CheckMethod(1);
            CheckVoucherSubID();
            PXLongOperation.StartOperation(Base, APConfirm);
            return adapter.Get();
        }


        #endregion

        //2021/03/17 UnVisible Merge with Confirm mantis:0011981
        //2021/05/27 Add Mantis:0012062
        #region �ǲ�����
        public PXAction<FilterForAP> CreateAccConfirmNbr;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "�ǲ�����")]
        protected IEnumerable createAccConfirmNbr(PXAdapter adapter)
        {
            FilterForAP filter = Filters.Current;
            CheckMethod(2);
            PXLongOperation.StartOperation(Base, ToCreateAccConfirmNbr);
            return adapter.Get();
        }
        #endregion

        //2021/02/22 Add Mantis:0011939
        #region �L�b
        public PXAction<FilterForAP> Release;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "�L�b")]
        protected IEnumerable release(PXAdapter adapter)
        {
            FilterForAP filter = Filters.Current;
            if (filter.VoucherDate == null)
                throw new Exception("�ж�g�ǲ����!");
            CheckMethod(6);
            PXLongOperation.StartOperation(Base, APRelease);
            return adapter.Get();
        }


        #endregion

        //2021/02/19 Add Mantis:0011939
        //2021/03/17 Delete Mantis:0011981
        //2021/05/24 Add Mantis:0012052
        //2021/05/27 Modify Mantis:0012062
        #region ��������
        public PXAction<FilterForAP> RemoveAccConfirmNbr;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "��������")]
        protected IEnumerable removeAccConfirmNbr(PXAdapter adapter)
        {
            FilterForAP filter = Filters.Current;
            CheckMethod(3);
            WebDialogResult result = Filters.Ask(ActionsMessages.Warning,
                   PXMessages.LocalizeFormatNoPrefix("�T�w�n���������ܡH"),
                   MessageButtons.OKCancel, MessageIcon.Warning, true);
            //checking answer	
            if (result != WebDialogResult.OK) return adapter.Get();
            PXLongOperation.StartOperation(Base, ToRemoveAccConfirmNbr);
            return adapter.Get();
        }
        #endregion

        //2021/05/28 Add Mantis:0012062
        #region �����T�{
        public PXAction<FilterForAP> CancelConfrim;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "�����T�{")]
        protected IEnumerable cancelConfrim(PXAdapter adapter)
        {
            FilterForAP filter = Filters.Current;
            CheckMethod(4);
            WebDialogResult result = Filters.Ask(ActionsMessages.Warning,
                   PXMessages.LocalizeFormatNoPrefix("�T�w�n�����T�{�X?"),
                   MessageButtons.OKCancel, MessageIcon.Warning, true);
            //checking answer	
            if (result != WebDialogResult.OK) return adapter.Get();

            PXLongOperation.StartOperation(Base, APCancel);
            return adapter.Get();
        }
        #endregion

        #region �����e�f
        public PXAction<FilterForAP> ReturnSubmit;
        //Defining Button Attribute with tooltip
        [PXButton(Tooltip = "�i�Ŀ�h�������e�f,�Цܭp���վ���", CommitChanges = true)]
        //Providing title and mapping action access rights
        [PXUIField(DisplayName = "�����e�f")]
        public virtual IEnumerable returnSubmit(PXAdapter adapter)
        {
            //2020/07/24���i�����e
            //2020/09/26�P�H�e�ǲ������e�f�޿�̼˥u�O���ݭnKGVoucher�o�q
            CheckMethod(5);
            WebDialogResult result = Filters.Ask(ActionsMessages.Warning,
                   PXMessages.LocalizeFormatNoPrefix("�T�w�n�����e�f��?"),
                   MessageButtons.OKCancel, MessageIcon.Warning, true);
            //checking answer	
            if (result != WebDialogResult.OK) return adapter.Get();

            PXLongOperation.StartOperation(Base, APReturn);
            return adapter.Get();
        }
        #endregion

        #region �ǲ��w��
        public PXAction<FilterForAP> ViewVoucherAction;
        [PXButton(CommitChanges = true, Tooltip = "����@���w������")]
        //[PXLookupButton]
        [PXUIField(DisplayName = "�����w��")]
        protected IEnumerable viewVoucherAction(PXAdapter adapter)
        {
            ViewTableFin invoice = Details.Current;
            KGVoucherH row = PXSelect<KGVoucherH,
                Where<KGVoucherH.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, invoice.RefNbr);
            KGVoucherMaint graph = PXGraph.CreateInstance<KGVoucherMaint>();
            // Setting the current product for the graph
            if (row == null)
            {
                throw new Exception("���渹�|�����ͤ���!");
            }
            graph.VoucherH.Current = graph.VoucherH.Search<KGVoucherH.refNbr>(row.RefNbr);

            if (graph.VoucherH.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "Open Voucher Maint")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }

            return adapter.Get();
        }
        #endregion

        #region NO Need
        //Visable = fale
        #region ����
        public PXAction<FilterForAP> ProcessFinAction;
        [PXButton(CommitChanges = true, Tooltip = "�i�Ŀ�h���������")]
        [PXUIField(DisplayName = "����", Visible = false)]
        protected IEnumerable processFinAction(PXAdapter adapter)
        {

            PXLongOperation.StartOperation(Base, KGVoucherProcess);
            return adapter.Get();
        }


        #endregion

        //2021/02/19 Delete Mantis:0011939
        #region �p���w��
        /*public PXAction<FilterForAP> APInvoicePreviewAction;
        [PXButton(CommitChanges = true, Tooltip = "����@���w���p��")]
        [PXLookupButton]
        [PXUIField(DisplayName = "�p���w��")]
        public virtual IEnumerable aPInvoicePreviewAction(PXAdapter adapter)
        {
            APInvoice invoice = Details.Current;
            // Creating the instance of the graph
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            // Setting the current product for the graph
            APInvoice aPInvoice = PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>,
                And<APInvoice.docType, Equal<Required<APInvoice.docType>>>>>
                .Select(Base, invoice.RefNbr, invoice.DocType);
            graph.Document.Current = aPInvoice;

            if (graph.Document.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "Open APInvoice Entry")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };

            }
            return adapter.Get();
        }*/
        #endregion
        #endregion

        #endregion

        #region Event Handlers
        public virtual void ViewTableFin_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ViewTableFin row = (ViewTableFin)e.Row;
            if (row == null) return;
            setEnabled();

        }
        /*
         public virtual void _(Events.FieldDefaulting<ViewTableFin, ViewTableFin.usrEPRefNbr> e)
         {
             ViewTableFin row = e.Row;
             if (row == null) return;
             setDefaultValue(row);
         }
         */
        public virtual void FilterForAP_VoucherDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            FilterForAP filter = (FilterForAP)e.Row;

            //2021/11/03 Add Mantis: 0012258 �w�]���a2021/08/30(only for UAT)
            //DateTime date = new DateTime(2021, 8, 31, 00, 00, 00);
            //filter.VoucherDate = date;
            
            DateTime today = (DateTime)Base.Accessinfo.BusinessDate;
            int date = Int32.Parse(today.Day.ToString());
            if (date < 28)
            {
                filter.VoucherDate = today.AddDays(28 - date);
            }
            if (date > 28)
            {
                filter.VoucherDate = today.AddMonths(1).AddDays(28 - date);
            }
            
        }
        public virtual void FilterForAP_UsrIsConfirm_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            FilterForAP master = (FilterForAP)e.Row;
            if (master.UsrIsConfirm == 0)
                master.UsrConfirmBy = null;

        }
        #endregion

        #region Methods
        public void setEnabled()
        {
            FilterForAP header = Filters.Current;
            ViewTableFin row = Details.Current;
            PXUIFieldAttribute.SetEnabled(Details.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<ViewTableFin.selected>(Details.Cache, null, true);

            //CreateAccConfirmNbr.SetEnabled(header.UsrIsConfirm==FilterForAP.Int_1);
            //RemoveAccConfirmNbr.SetEnabled(header.UsrIsConfirm == FilterForAP.Int_1);
        }

        public void setDefaultValue(ViewTableFin row)
        {
            if (row.OrigModule == KGConst.EP && row.DocType == APDocType.Invoice)
            {
                //APRegisterFinExt detailsFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(row);
                row.UsrEPRefNbr = row.OrigRefNbr;
                EPExpenseClaim claim = GetEPClaim(row.UsrEPRefNbr);
                //row.UsrEPDesc = claim?.DocDesc;
            }
        }

        protected virtual void NewPageToDisplayReport(KGVoucherProcess pp)
        {
            //d[ReportMessages.CheckReportFlag] = ReportMessages.CheckReportFlagValue;
            //var requiredException = new PXReportRequiredException(d, "KG601000", PXBaseRedirectException.WindowMode.New, "�t�ӽT�{��");
            ViewTableFin invoice = Details.Current;
            if (invoice != null)
            {
                string periodyear = invoice.FinPeriodID.Substring(0, 4);
                string periodmonth = invoice.FinPeriodID.Substring(4, 2);
                Dictionary<string, string> mailParams = new Dictionary<string, string>()
                {
                    ["PeriodFrom"] = periodmonth + periodyear,
                    ["PeriodTo"] = periodmonth + periodyear,

                    ["RefNbr"] = invoice.RefNbr,
                    ["DocType"] = invoice.DocType
                };
                var requiredException = new PXReportRequiredException(mailParams, "AP610500", PXBaseRedirectException.WindowMode.New, "");
                requiredException.SeparateWindows = true;
                //requiredException.AddSibling("KG601000", mailParams);
                throw new PXRedirectWithReportException(pp, requiredException, "Preview");
            }
        }

        /// <summary>
        /// �p���T�{
        /// </summary>
        private void APConfirm()
        {
            //��T�{���
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                FilterForAP filter = Filters.Current;
                PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
                //A.�t��update�ϥΪ̤Ŀ諸AP Bill����APInvoice.DocDate, APInvoice.DueDate��FilterForAP.VoucherDate.
                //B.�T�{��s�᪺APInvoice.DocDate�n�PAPInvoice.FinPeriodID���P�@�Ӥ��, �p�G���O��update APInvoice.FinPeriodID.
                //C.���s�p��KGBillPayment.PaymentDate, �w��s�᪺APInvoice.DueDate�[�WKGBillPayment.PaymentPeriod�����ѼƬ���ǭp��()

                foreach (ViewTableFin invoice in viewt)
                {
                    #region Update APInvoice
                    PXUpdate<Set<APRegister.docDate, Required<APRegister.docDate>>,
                    APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                    .Update(Base, filter.VoucherDate, invoice.RefNbr);

                    ////Mantis:0011939
                    //�Ф]��sAPInvoice.PayDate��FilterForAP.VoucherDate.
                    PXUpdate<Set<APInvoice.dueDate, Required<APInvoice.dueDate>,
                        Set<APInvoice.payDate, Required<APInvoice.payDate>>>,
                   APInvoice, Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                   .Update(Base, filter.VoucherDate, filter.VoucherDate, invoice.RefNbr);

                    //Mantis:0011939
                    //�Ф]��sAPInvoice.PayDate��FilterForAP.VoucherDate.
                    /*PXUpdate<Set<APInvoice.payDate, Required<APInvoice.payDate>>,
                   APInvoice, Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                   .Update(Base, filter.VoucherDate, invoice.RefNbr);*/

                    string period = ((DateTime)filter.VoucherDate).ToString("yyyy") + ((DateTime)filter.VoucherDate).ToString("MM");
                    if (period != invoice.FinPeriodID)
                    {
                        PXUpdate<Set<APRegister.finPeriodID, Required<APRegister.finPeriodID>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                        .Update(Base, period, invoice.RefNbr);
                        PXUpdate<Set<APRegister.tranPeriodID, Required<APRegister.tranPeriodID>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                        .Update(Base, period, invoice.RefNbr);
                    }

                    //2020/08/31 ADD ��user���U�p���T�{��,
                    //�бNUsrIsConfirm�]��1, UsrConfirmBy�g�J����ϥΪ�, UsrConfirmDate�g�J business date
                    PXUpdate<
                        Set<APRegisterFinExt.usrIsConfirm, True,
                        Set<APRegisterFinExt.usrConfirmBy, Required<APRegisterFinExt.usrConfirmBy>,
                        Set<APRegisterFinExt.usrConfirmDate, Required<APRegisterFinExt.usrConfirmDate>>>>,
                    APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                     .Update(Base, Base.Accessinfo.UserID, Base.Accessinfo.BusinessDate, invoice.RefNbr);
                    #endregion

                    #region Update KGBillPayment
                    PXResultset<KGBillPayment> set = PXSelect<KGBillPayment, Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                                                              .Select(Base, invoice.RefNbr);
                    if (set != null)
                    {
                        APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();

                        graph.Document.Current = invoice;

                        APInvoiceEntryFinExt graphExt = graph.GetExtension<APInvoiceEntryFinExt>();

                        foreach (KGBillPayment billPayment in set)
                        {
                            ///<remarks> Added the validation from custom graph according to mantis [0012339] request. </remarks>
                            graphExt.CheckKGBillPayment(billPayment, true);

                            var invHold = graph.Document.Cache.GetStateExt<APInvoice.hold>(invoice) as PXFieldState;
                            if (invHold.ErrorLevel != PXErrorLevel.Undefined)
                            {
                                throw new Exception(invHold.Error);
                            }

                            var pymtAmt = graph.GetExtension<APInvoiceEntry_Extension>().KGBillPaym.Cache.GetStateExt<KGBillPayment.paymentAmount>(billPayment) as PXFieldState;
                            if (pymtAmt.ErrorLevel != PXErrorLevel.Undefined)
                            {
                                throw new Exception(pymtAmt.Error);
                            }

                            //2021/07/01 Mantis: 0012116
                            //�޿�: 
                            //�YBillPayment.PricingType ="B" , ����VocherDate
                            //�YBillPayment.PricingType = "A" , 
                            //  0�� : ��VoucherDate,
                            //  �j��0�� : ��GetLongTremPaymentDate(VoucherDate)
                            if (billPayment.PaymentPeriod == null)
                            {
                                throw new Exception("�ж񧴲����Ѽ����!");
                            }
                            //else if (billPayment.PricingType ==PricingType.B)
                            //{
                            //    //�����ʧ@
                            //}
                            else if (billPayment.PaymentPeriod == 0)
                            {
                                PXUpdate<
                                    Set<KGBillPayment.paymentDate, Required<KGBillPayment.paymentDate>>,
                                    KGBillPayment,
                                    Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                                    .Update(Base, filter.VoucherDate, billPayment.BillPaymentID);
                                //[KED-20] 2022-12-19 Alton 
                                PXUpdate<
                                    Set<KGVoucherLFinExt.usrPaymentDate, Required<KGVoucherLFinExt.usrPaymentDate>>,
                                    KGVoucherL,
                                    Where<KGVoucherL.billPaymentID, Equal<Required<KGVoucherL.billPaymentID>>>>
                                    .Update(Base, filter.VoucherDate, billPayment.BillPaymentID);
                            }
                            else if (billPayment.PaymentPeriod > 0)
                            {
                                PXUpdate<
                                    Set<KGBillPayment.paymentDate, Required<KGBillPayment.paymentDate>>,
                                    KGBillPayment,
                                    Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                                    .Update(Base, VoucherUntil.GetLongTremPaymentDate
                                     (Base, (DateTime)filter.VoucherDate, billPayment.PaymentPeriod), billPayment.BillPaymentID);
                                //[KED-20] 2022-12-19 Alton 
                                PXUpdate<
                                    Set<KGVoucherLFinExt.usrPaymentDate, Required<KGVoucherLFinExt.usrPaymentDate>>,
                                    KGVoucherL,
                                    Where<KGVoucherL.billPaymentID, Equal<Required<KGVoucherL.billPaymentID>>>>
                                    .Update(Base, VoucherUntil.GetLongTremPaymentDate
                                     (Base, (DateTime)filter.VoucherDate, billPayment.PaymentPeriod), billPayment.BillPaymentID);
                            }
                        }
                    }
                    #endregion

                    //2021/09/13 Add Mantis: 0012228
                    #region Update WHTTran
                    TWNWHTTran tWNWHTTran = GetTWNWHTTran(invoice.RefNbr);
                    if (tWNWHTTran != null)
                        PXUpdate<
                            Set<TWNWHTTran.paymentDate, Required<FilterForAP.voucherDate>>,
                            TWNWHTTran,
                            Where<TWNWHTTran.refNbr, Equal<Required<APInvoice.refNbr>>>>
                            .Update(Base, filter.VoucherDate, invoice.RefNbr);
                    #endregion

                    //2021/05/28 Move From  ToCreateAccConfirmNbr Mantis: 0012062
                    #region Update KGVoucher
                    KGVoucherH voucherH = Base.GetKGVoucherH(invoice.RefNbr);
                    foreach (KGVoucherL voucherL in PXSelect<KGVoucherL, Where<KGVoucherL.voucherHeaderID,
                        Equal<Required<KGVoucherH.voucherHeaderID>>>>.Select(Base, voucherH.VoucherHeaderID))
                    {

                        if (voucherL.KGVoucherType?.ToString().Trim() == "PA")
                        {
                            KGBillPayment kgbillpayment = PXSelect<KGBillPayment,
                                Where<KGBillPayment.billPaymentID,
                                Equal<Required<KGBillPayment.billPaymentID>>>>
                                .Select(Base, voucherL.BillPaymentID);
                            DateTime date = (DateTime)filter.VoucherDate;
                            DateTime? DueDate;
                            //add by louis for no KGBillPayment in APInvoice 20211228
                            if (kgbillpayment != null)
                            {
                                //2021/07/01 Add Mantis: 0012116
                                if (kgbillpayment.PricingType == PricingType.B)
                                {
                                    DueDate = kgbillpayment.PaymentDate;
                                }
                                else if (kgbillpayment.PaymentPeriod == 0)
                                {
                                    DueDate = filter.VoucherDate;
                                }
                                else if (kgbillpayment.PaymentPeriod > 0)
                                {
                                    DueDate =
                                        VoucherUntil.GetLongTremPaymentDate
                                        (Base, (DateTime)filter.VoucherDate, kgbillpayment.PaymentPeriod);
                                }
                                else
                                {
                                    DueDate = kgbillpayment.PaymentDate;
                                }
                                //2021/11/25 delete Louis said
                                //Digest = Base.GetDigest(voucherL.AccountID, voucherL.Digest, voucherH.ContractID,
                                // voucherH.VendorID, DueDate);

                                /*
                                 * [KED-20] 2022-12-19 Alton �]���ɪ�kgBillPayment��Ƭ��¸��
                                 * �]��KGVoucherLFinExt.usrPaymentDate = kgbillpayment.PaymentDate(X)
                                */
                                PXUpdate<Set<KGVoucherL.dueDate, Required<KGVoucherL.dueDate>>,
                                    KGVoucherL,
                                    Where<KGVoucherL.voucherLineID, Equal<Required<KGVoucherL.voucherLineID>>>>
                                    .Update(Base, DueDate, voucherL.VoucherLineID);
                            }

                        }
                    }
                    #endregion

                    #region Update ADRInvoice
                    //2021/02/03 Add mantis: 0011939
                    //��X���i�p����������ɤ�վ�,
                    //��s�ɤ�վ㪺DocDate, FinPeriodID,PayDate,�⪬�A�אּbalance
                    APRegister adrInvoice = getDeductionADR(invoice.RefNbr);
                    if (adrInvoice != null)
                    {
                        string periodMMYYYY = ((DateTime)filter.VoucherDate).ToString("MM") + ((DateTime)filter.VoucherDate).ToString("yyyy");
                        APInvoiceEntry invoiceADREntry = PXGraph.CreateInstance<APInvoiceEntry>();
                        invoiceADREntry.Document.Current = invoiceADREntry.Document.Search<APInvoice.refNbr>(adrInvoice.RefNbr, adrInvoice.DocType);
                        invoiceADREntry.Document.SetValueExt<APInvoice.docDate>(invoiceADREntry.Document.Current, filter.VoucherDate);
                        invoiceADREntry.Document.SetValueExt<APInvoice.finPeriodID>(invoiceADREntry.Document.Current, periodMMYYYY);
                        invoiceADREntry.Document.SetValueExt<APInvoice.tranPeriodID>(invoiceADREntry.Document.Current, periodMMYYYY);
                        invoiceADREntry.Document.SetValueExt<APRegisterFinExt.usrIsConfirm>(invoiceADREntry.Document.Current, true);
                        invoiceADREntry.Document.SetValueExt<APRegisterFinExt.usrConfirmBy>(invoiceADREntry.Document.Current, Base.Accessinfo.UserID);
                        invoiceADREntry.Document.SetValueExt<APRegisterFinExt.usrConfirmDate>(invoiceADREntry.Document.Current, Base.Accessinfo.BusinessDate);
                        //2021/02/19 Add Mantis:0011939
                        invoiceADREntry.Document.SetValueExt<APInvoice.payDate>(invoiceADREntry.Document.Current, filter.VoucherDate);
                        invoiceADREntry.Actions.PressSave();

                        if (invoiceADREntry.Document.Current.Status != APDocStatus.Balanced &&
                            invoiceADREntry.Document.Current.Hold == true)
                        {
                            invoiceADREntry.Document.SetValueExt<APInvoice.hold>(invoiceADREntry.Document.Current, false);
                        }
                        //invoiceADREntry.Persist();

                        //2021/09/30 ���setvalue
                        /*
                        PXUpdate<Set<APInvoice.docDate, Required<APInvoice.docDate>>,
                            APInvoice, Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                            .Update(Base, filter.VoucherDate, adrInvoice.RefNbr);
                        if (period != adrInvoice.FinPeriodID)
                        {

                            PXUpdate<Set<APRegister.finPeriodID, Required<APRegister.finPeriodID>>,
                            APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                            .Update(Base, period, adrInvoice.RefNbr);
                            PXUpdate<Set<APRegister.tranPeriodID, Required<APRegister.tranPeriodID>>,
                            APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                            .Update(Base, period, adrInvoice.RefNbr);
                        }
                        PXUpdate<
                        Set<APRegisterFinExt.usrIsConfirm, True,
                        Set<APRegister.hold, False,
                        Set<APRegister.status, APDocStatus.balanced,
                        Set<APRegisterFinExt.usrConfirmBy, Required<APRegisterFinExt.usrConfirmBy>,
                        Set<APRegisterFinExt.usrConfirmDate, Required<APRegisterFinExt.usrConfirmDate>>>>>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                        .Update(Base, Base.Accessinfo.UserID, Base.Accessinfo.BusinessDate, adrInvoice.RefNbr);

                        //2021/02/19 Add Mantis:0011939
                        PXUpdate<Set<APInvoice.payDate, Required<APInvoice.payDate>>,
                        APInvoice, Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                        .Update(Base, filter.VoucherDate, adrInvoice.RefNbr);

                        */

                    }
                    #endregion
                }

                //2021/03/17 Add Mantis:0011981
                //2021/05/27 Move to Indepandent Action Mantis:0012062
                //���Ͷǲ����X
                //ToCreateAccConfirmNbr();

                ts.Complete();
            }
            //AutoRefreash
            Details.Cache.Clear();
            Details.Cache.ClearQueryCache();
        }

        /// <summary>
        /// �ǲ�����
        /// </summary>
        private static object lockObj1 = new object();
        private void ToCreateAccConfirmNbr()
        {
            lock (lockObj1)
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    FilterForAP filter = Filters.Current;
                    PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();

                    int Nbr = 0, foreachNbr = 0;
                    foreach (ViewTableFin invoice in viewt)
                    {
                        foreachNbr++;
                        APRegister register = PXSelect<APRegister,
                        Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                        .Select(Base, invoice.RefNbr);
                        APRegisterFinExt registerExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
                        APInvoice newinvoice = PXSelect<APInvoice,
                                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                                .Select(Base, invoice.RefNbr);
                        if (foreachNbr == 1)
                        {
                            string batchNbr = KGBatchNbrUtil.GetLastBatchNbr(Base, newinvoice.DocDate);
                            if (batchNbr != null)
                                Nbr = Int16.Parse(batchNbr.Substring(8));
                        }
                        Nbr++;
                        string usrAccConfirmNbr = newinvoice.DocDate?.ToString("yyyyMMdd") + Nbr.ToString().PadLeft(4, '0');

                        PXUpdate<Set<APRegisterFinExt.usrAccConfirmNbr, Required<APRegisterFinExt.usrAccConfirmNbr>>,
                                 APRegister,
                                 Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>.Update(Base, usrAccConfirmNbr, invoice.RefNbr);

                        if (registerExt.UsrAccConfirmNbr != null)
                        {
                            //Add Log
                            CreateConfirmLog(registerExt.UsrConfirmBy, usrAccConfirmNbr,
                                             registerExt.UsrAccConfirmNbr, register.RefNbr, register.DocType);
                        }

                        APRegister ADR = getDeductionADR(invoice.RefNbr);
                        if (ADR != null)
                        {
                            APRegisterFinExt ADRExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(ADR);
                            if (ADRExt.UsrAccConfirmNbr != null)
                            {
                                //Add Log
                                CreateConfirmLog(ADRExt.UsrConfirmBy, usrAccConfirmNbr + "D",
                                ADRExt.UsrAccConfirmNbr, ADR.RefNbr, ADR.DocType);
                            }

                            PXUpdate<
                            Set<APRegisterFinExt.usrAccConfirmNbr, Required<APRegisterFinExt.usrAccConfirmNbr>>,
                            APRegister,
                            Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                            And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                            .Update(Base, usrAccConfirmNbr + "D", ADR.DocType, ADR.RefNbr);
                        }

                        //2021/05/14 Add KGVoucherL������]�n��
                        //2021/05/28 Move to APConfirm           
                        //KGVoucherH voucherH = Base.GetKGVoucherH(invoice.RefNbr);
                        //foreach (KGVoucherL voucherL in PXSelect<KGVoucherL, Where<KGVoucherL.voucherHeaderID,
                        //    Equal<Required<KGVoucherH.voucherHeaderID>>>>.Select(Base, voucherH.VoucherHeaderID))
                        //{

                        //    if (voucherL.KGVoucherType.ToString().Trim() == "PA")
                        //    {
                        //        KGBillPayment kgbillpayment = PXSelect<KGBillPayment,
                        //            Where<KGBillPayment.billPaymentID,
                        //            Equal<Required<KGBillPayment.billPaymentID>>>>
                        //            .Select(Base, voucherL.BillPaymentID);
                        //        DateTime date = (DateTime)filter.VoucherDate;
                        //        DateTime? DueDate;
                        //        string Digest = "";
                        //        if (kgbillpayment.PaymentPeriod == 0)
                        //        {
                        //            DueDate = filter.VoucherDate;
                        //        }
                        //        else
                        //        {
                        //            DueDate =
                        //                VoucherUntil.GetLongTremPaymentDate
                        //                (Base, (DateTime)filter.VoucherDate, kgbillpayment.PaymentPeriod);
                        //        }
                        //        Digest = Base.GetDigest(voucherL.AccountID, voucherL.Digest, voucherH.ContractID,
                        //            voucherH.VendorID, voucherL.DueDate);

                        //        PXUpdate<Set<KGVoucherL.dueDate, Required<KGVoucherL.dueDate>,
                        //            Set<KGVoucherL.digest, Required<KGVoucherL.digest>>>,
                        //            KGVoucherL,
                        //            Where<KGVoucherL.voucherLineID, Equal<Required<KGVoucherL.voucherLineID>>>>
                        //            .Update(Base, DueDate, Digest, voucherL.VoucherLineID);
                        //    }
                        //}
                    }
                    ts.Complete();
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void ToRemoveAccConfirmNbr()
        {
            PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (ViewTableFin invoice in viewt)
                {
                    APRegister register = GetRegister(invoice.RefNbr);
                    APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
                    //20210/05/28 Add Create Log newAccConfirmNbr = Null
                    CreateConfirmLog(Base.Accessinfo.UserID, null, registerFinExt.UsrAccConfirmNbr ?? null, register.RefNbr, register.DocType);
                    //�M�Ŧ��iAPInvoice&����ADR�� usrAccConfirmNbr
                    PXUpdate<
                        Set<APRegisterFinExt.usrAccConfirmNbr, Null>,
                        APRegister,
                        Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                        .Update(Base, invoice.RefNbr);

                    APRegister ADR = getDeductionADR(invoice.RefNbr);
                    if (ADR != null)
                    {
                        APRegisterFinExt ADRFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(ADR);
                        CreateConfirmLog(Base.Accessinfo.UserID, null, ADRFinExt.UsrAccConfirmNbr ?? null, ADR.RefNbr, ADR.DocType);
                        PXUpdate<
                                Set<APRegisterFinExt.usrAccConfirmNbr, Null>,
                                APRegister,
                                Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                                And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                                .Update(Base, ADR.DocType, ADR.RefNbr);

                    }
                    //2021/05/28 Move to APCancel()
                    //2021/05/24 Modify Mantis: 0012052
                    /*
                    PXUpdate<
                        Set<APRegisterFinExt.usrIsConfirm, False,
                        Set<APRegisterFinExt.usrAccConfirmNbr, Null,
                        Set<APRegisterFinExt.usrConfirmBy, Null,
                        Set<APRegisterFinExt.usrConfirmDate, Null>>>>,
                        APRegister,
                        Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                        .Update(Base, invoice.RefNbr);
                    APRegister ADR = getADR(invoice.RefNbr);
                    if (ADR != null)
                    {
                        PXUpdate<
                            Set<APRegisterFinExt.usrIsConfirm, False,
                            Set<APRegisterFinExt.usrAccConfirmNbr, Null,
                            Set<APRegisterFinExt.usrConfirmBy, Null,
                            Set<APRegisterFinExt.usrConfirmDate, Null>>>>,
                            APRegister,
                            Where<APRegister.docType, Equal<Required<APRegister.docType>>,
                            And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
                            .Update(Base, ADR.DocType, ADR.RefNbr);
                    }
                    */
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// �����T�{
        /// </summary>
        private void APCancel()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
                foreach (ViewTableFin row in viewt)
                {
                    //2021/05/28 Delete ���ݭn���APInvoice�����A
                    //���ܭp���檬�A�����m+�M�����ǲ��������
                    /*
                     * PXUpdate<
                        Set<APRegister.hold, True,
                        Set<APRegister.status, Required<APRegister.status>>>,
                        APRegister,
                        Where<APRegister.refNbr,
                        Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(Base, "H", row.RefNbr);
                    */
                    PXUpdate<
                        Set<APRegisterFinExt.usrIsConfirm, False,
                        Set<APRegisterFinExt.usrConfirmDate, Null,
                        Set<APRegisterFinExt.usrConfirmBy, Null>>>,
                        APRegister,
                        Where<APRegister.refNbr,
                        Equal<Required<APRegister.refNbr>>>>
                        .Update(Base, row.RefNbr);


                }
                ts.Complete();
            }
            //AutoRefreash
            Details.Cache.Clear();
            Details.Cache.ClearQueryCache();
        }

        /// <summary>
        /// 5. �����e�f
        /// </summary>
        private void APReturn()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
                foreach (ViewTableFin row in viewt)
                {
                    APRegisterFinExt detailsFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(row);
                    //���ܭp���檬�A�����m       
                    string EPRefNbr = row.UsrEPRefNbr;
                    APInvoiceEntry invoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                    invoiceEntry.Document.Current = invoiceEntry.Document.Search<APInvoice.refNbr>(row.RefNbr, row.DocType);
                    invoiceEntry.Document.Current.Hold = true;
                    invoiceEntry.Document.Update(invoiceEntry.Document.Current);
                    invoiceEntry.Persist();

                    //2021/09/28 Add Mantis: 0012246 
                    APInvoiceEntry ADRInvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                    APRegister ADRRegister = getDeductionADR(row.RefNbr);
                    if (ADRRegister != null)
                    {
                        ADRInvoiceEntry.Document.Current = invoiceEntry.Document.Search<APInvoice.refNbr>(ADRRegister.RefNbr, ADRRegister.DocType);
                        ADRInvoiceEntry.Document.Current.Hold = true;
                        ADRInvoiceEntry.Document.Update(ADRInvoiceEntry.Document.Current);
                        ADRInvoiceEntry.Persist();
                    }
                    /*
                    PXUpdate<
                        Set<APRegister.hold, True,
                        Set<APRegister.status, APDocStatus.hold>>,
                        APRegister,
                        Where<APRegister.refNbr,Equal<Required<APRegister.refNbr>>>>
                        .Update(Base, row.RefNbr);
                    */

                    //2021/05/31 Mantis: 0012062
                    //2021/06/11 Mantis: 0012062
                    //2021/11/25 :Louis�f�Y,��l���i��q�妸�Ӫ� �P�_�אּ��OrigModule
                    if (row.OrigModule == KGConst.EP)
                    {
                        PXUpdate<
                            Set<APRegister.origModule, Required<APRegister.origModule>,
                            Set<APRegister.origDocType, Null,
                            Set<APRegister.origRefNbr, Null>>>,
                            APRegister,
                            Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>,
                            And<APRegister.docType, Equal<Required<APRegister.docType>>>>>
                            .Update(Base, "AP", row.RefNbr, row.DocType);
                        //2021/07/02 Add Mantis: 0012062 Desc :Louis 07/01
                        PXUpdate<
                            Set<KGBillPayment.refNbr, Null>,
                            KGBillPayment,
                            Where<KGBillPayment.refNbr, Equal<Required<KGBillPayment.refNbr>>>>
                            .Update(Base, row.RefNbr);
                        PXUpdate<
                            Set<GVApGuiInvoice.refNbr, Null>,
                            GVApGuiInvoice,
                            Where<GVApGuiInvoice.refNbr, Equal<Required<GVApGuiInvoice.refNbr>>>>
                            .Update(Base, row.RefNbr);
                        PXUpdate<
                            Set<GVApGuiInvoiceDetail.apRefNbr, Null,
                            Set<GVApGuiInvoiceDetail.apDocType, Null>>,
                            GVApGuiInvoiceDetail,
                            Where<GVApGuiInvoiceDetail.apRefNbr, Equal<Required<GVApGuiInvoiceDetail.apRefNbr>>,
                            And<GVApGuiInvoiceDetail.apDocType, Equal<Required<GVApGuiInvoiceDetail.apDocType>>>>>
                            .Update(Base, row.RefNbr, row.DocType);
                        PXUpdate<
                            Set<TWNWHTTran.refNbr, Null,
                            Set<TWNWHTTran.docType, Null>>,
                            TWNWHTTran,
                            Where<TWNWHTTran.refNbr, Equal<Required<TWNWHTTran.refNbr>>>>
                            .Update(Base, row.RefNbr);

                        APInvoiceEntry newinvoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                        newinvoiceEntry.Document.Current = newinvoiceEntry.Document.Search<APInvoice.refNbr>(row.RefNbr, row.DocType);
                        newinvoiceEntry.DeleteButton.Press();

                        PXUpdate<
                            Set<EPExpenseClaimExt.usrIsRejectByAcct, True,
                            Set<EPExpenseClaim.released, False,
                            Set<EPExpenseClaim.approved, False,
                            Set<EPExpenseClaim.hold, True,
                            Set<EPExpenseClaim.approveDate, Null,
                            Set<EPExpenseClaim.status, Required<EPExpenseClaim.status>,
                            Set<EPExpenseClaimExt.usrApprovalStatus, Required<EPExpenseClaimExt.usrApprovalStatus>>>>>>>>,
                            EPExpenseClaim,
                            Where<EPExpenseClaim.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                            .Update(Base, EPExpenseClaimStatus.HoldStatus, "RJ", EPRefNbr);

                        PXUpdate<
                            Set<EPExpenseClaimDetails.released, False,
                            Set<EPExpenseClaimDetails.aPDocType, Null,
                            Set<EPExpenseClaimDetails.aPRefNbr, Null,
                            Set<EPExpenseClaimDetails.hold, False,
                            Set<EPExpenseClaimDetails.status, Required<EPExpenseClaimDetails.status>>>>>>,
                            EPExpenseClaimDetails,
                            Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                            .Update(Base, EPExpenseClaimDetailsStatus.ApprovedStatus, EPRefNbr);

                    }


                }
                ts.Complete();
            }
            //AutoRefreash
            Details.Cache.Clear();
            Details.Cache.ClearQueryCache();
        }

        /// <summary>
        /// 6. �L�b
        /// </summary>
        private void APRelease()
        {
            PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (ViewTableFin invoice in viewt)
                {
                    #region ��lAPInvoice�L�b
                    APInvoiceEntry invoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
                    invoiceEntry.Document.Current = PXSelect<APInvoice,
                        Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                        .Select(Base, invoice.RefNbr, invoice.DocType);
                    invoiceEntry.release.Press();
                    /*
                     APInvoice aPInvoice = invoiceEntry.Document.Current;
                    APRegister register = GetRegister(aPInvoice.RefNbr);
                    APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
                    PXUpdate<
                        Set<BatchExt.usrAccConfirmNbr, Required<APRegisterFinExt.usrAccConfirmNbr>>,
                        Batch,
                        Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>
                        .Update(Base, registerFinExt.UsrAccConfirmNbr, aPInvoice.BatchNbr);
                    */
                    //2021/05/31 Mantis:0012062
                    GVApGuiInvoice gvAPInvoice = GetGVInvoice(invoice.RefNbr);
                    if (gvAPInvoice != null)
                        PXUpdate<Set<GVApGuiInvoice.declareYear, Required<GVApGuiInvoice.declareYear>,
                            Set<GVApGuiInvoice.declareMonth, Required<GVApGuiInvoice.declareMonth>>>,
                            GVApGuiInvoice,
                            Where<GVApGuiInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                            .Update(Base, ((DateTime)invoice.DocDate).ToString("yyyy"), ((DateTime)invoice.DocDate).ToString("MM"), invoice.RefNbr);
                    #endregion

                    #region ������ADR Invoice �L�b
                    APInvoiceEntry invoiceADREntry = PXGraph.CreateInstance<APInvoiceEntry>();
                    APRegister ADRregister = getDeductionADR(invoice.RefNbr);
                    if (ADRregister != null)
                    {


                        invoiceADREntry.Document.Current =
                            invoiceADREntry.Document.Search<APInvoice.refNbr>(ADRregister.RefNbr, ADRregister.DocType);
                        if (invoiceADREntry.Document.Current.Status != APDocStatus.Balanced &&
                            invoiceADREntry.Document.Current.Hold == true)
                        {
                            invoiceADREntry.Document.SetValueExt<APInvoice.hold>(invoiceADREntry.Document.Current, false);
                        }

                        invoiceADREntry.release.Press();
                        /*
                        APInvoice ADRInvoice = invoiceADREntry.Document.Current;
                        APRegisterFinExt registerADRFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(ADRregister);
                        PXUpdate<
                            Set<BatchExt.usrAccConfirmNbr, Required<APRegisterFinExt.usrAccConfirmNbr>>,
                            Batch,
                            Where<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>
                            .Update(Base, registerADRFinExt.UsrAccConfirmNbr, ADRInvoice.BatchNbr);
                        */


                    }
                    #endregion
                }
                ts.Complete();
            }
        }

        bool setCheckThread = false;
        public void CheckStatusP()
        {
            foreach (KGVoucherH row in AllVouchers.Select())
            {
                if (row.Selected == true)
                {
                    if (row.Status == "P")
                    {
                        setCheckThread = true;
                        AllVouchers.Cache.RaiseExceptionHandling<KGVoucherH.status>(
                        row, row.Status, new PXSetPropertyException("���A���u�w����v�C���i�Φ����s", PXErrorLevel.RowError));
                    }
                }
            }
        }

        /// <summary>
        /// Check UsrIsConfirm/ UsrAccComfirmNbr/ KGVoucherH
        /// </summary>
        /// <param name="CheckType">
        /// 1.�p���T�{
        /// 2.�ǲ�����
        /// 3.��������
        /// 4.�����T�{
        /// 5.�����e�f
        /// 6.�L�b
        /// </param>
        private void CheckMethod(int? CheckType)
        {
            List<string> listVoucher = new List<string>();
            List<string> listAccConfirmNbr = new List<string>();
            List<string> listConfirm = new List<string>();
            PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
            foreach (ViewTableFin invoice in viewt)
            {
                APRegister register = GetRegister(invoice.RefNbr);
                APRegisterFinExt registerFinExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(register);
                KGVoucherH voucehrH = GetVoucherH(invoice.RefNbr);

                switch (CheckType)
                {
                    case 1: //�p���T�{
                        if (registerFinExt.UsrIsConfirm == true)
                            listConfirm.Add(invoice.RefNbr);
                        if (voucehrH == null)
                            listVoucher.Add(invoice.RefNbr);
                        break;
                    case 2: //�ǲ�����
                        if (registerFinExt.UsrIsConfirm != true)
                            listConfirm.Add(invoice.RefNbr);
                        if (registerFinExt.UsrAccConfirmNbr != null)
                            listAccConfirmNbr.Add(invoice.RefNbr);
                        break;
                    case 3: //��������
                        if (registerFinExt.UsrIsConfirm != true)
                            listConfirm.Add(invoice.RefNbr);
                        if (registerFinExt.UsrAccConfirmNbr == null)
                            listAccConfirmNbr.Add(invoice.RefNbr);
                        break;
                    case 4: //�����T�{
                        if (registerFinExt.UsrIsConfirm != true)
                            listConfirm.Add(invoice.RefNbr);
                        if (registerFinExt.UsrAccConfirmNbr != null)
                            listAccConfirmNbr.Add(invoice.RefNbr);
                        break;
                    case 5: //�h�^�e�f
                        if (registerFinExt.UsrIsConfirm == true)
                            listConfirm.Add(invoice.RefNbr);
                        if (registerFinExt.UsrAccConfirmNbr != null)
                            listAccConfirmNbr.Add(invoice.RefNbr);
                        break;
                    case 6: //�L�b
                        if (registerFinExt.UsrIsConfirm != true)
                            listConfirm.Add(invoice.RefNbr);
                        if (registerFinExt.UsrAccConfirmNbr == null)
                            listAccConfirmNbr.Add(invoice.RefNbr);
                        if (voucehrH == null)
                            listVoucher.Add(invoice.RefNbr);
                        break;
                }

            }

            string ErrorMsg = "";

            if (listConfirm.Count != 0 || listVoucher.Count != 0 || listAccConfirmNbr.Count != 0)
            {

                switch (CheckType)
                {
                    case 1: //�p���T�{
                        ErrorMsg = "�p����w�g�T�{ / �w�g����, �L�k�T�{!";
                        break;
                    case 2: //�ǲ�����
                        ErrorMsg = "�p����|���T�{/�w�g����, �L�k����!";
                        break;
                    case 3: //��������
                        ErrorMsg = "�p����|������, �L�k��������!";
                        break;
                    case 4: //�����T�{
                        ErrorMsg = "�p����|���T�{/�w�g����, �L�k�����T�{!";
                        break;
                    case 5: //�h�^�e�f
                        ErrorMsg = "�p����w�g�T�{/�w�g����, �L�k�����e�f!";
                        break;
                    case 6: //�L�b
                        ErrorMsg = "�p����|������, �L�k�L�b!";
                        break;
                }
                if (listVoucher.Count != 0)
                {
                    ErrorMsg = ErrorMsg + "�Х��N�H�U�p���沣�ͤ���!";
                }
                throw new Exception(ErrorMsg);
            }

        }

        /// <summary>
        /// Add Confrim Log
        /// </summary>
        private void CreateConfirmLog(Guid? ConfrimBy, string newAccNbr, string oldAccNbr, string refNbr, string DocType)
        {
            PXDatabase.Insert<KGAcctConfirmLog>(
                new PXDataFieldAssign("ConfirmBy", ConfrimBy),
                new PXDataFieldAssign("ConfirmDate", Base.Accessinfo.BusinessDate),
                new PXDataFieldAssign("NewAccConfirmNbr", newAccNbr),
                new PXDataFieldAssign("OrigAccConfirmNbr", oldAccNbr),
                new PXDataFieldAssign("RefNbr", refNbr),
                new PXDataFieldAssign("DocType", DocType),
                new PXDataFieldAssign("Module", "AP"),
                new PXDataFieldAssign("CreatedByID", Base.Accessinfo.UserID),
                new PXDataFieldAssign("CreatedByScreenID", "KG505001"),
                new PXDataFieldAssign("CreatedDateTime", Base.Accessinfo.BusinessDate),
                new PXDataFieldAssign("LastModifiedByID", Base.Accessinfo.UserID),
                new PXDataFieldAssign("LastModifiedByScreenID", "KG505001"),
                new PXDataFieldAssign("LastModifiedDateTime", Base.Accessinfo.BusinessDate));

        }
        /// <summary>
        /// For ����
        /// </summary>
        private void KGVoucherProcess()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
                foreach (ViewTableFin invoice in viewt)
                {
                    VoucherFilter filter = Base.VoucherFilters.Current;
                    CheckStatusP();
                    if (!setCheckThread)
                    {
                        //2021/01/06 �ǲ�������אּAPinvoice.DocDate�@��
                        KGVoucherH voucherH = Base.GetKGVoucherH(invoice.RefNbr);
                        //voucherH.VoucherDate = filter.VoucherDate;
                        voucherH.VoucherDate = invoice.DocDate;

                        DateTime voucherDate = (DateTime)voucherH.VoucherDate;
                        PXResultset<KGVoucherL> set = PXSelect<KGVoucherL,
                            Where<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherH.voucherHeaderID>>,
                            And<KGVoucherL.kGVoucherType, Equal<WordType.pA>>>>
                            .Select(Base, voucherH.VoucherHeaderID);
                        foreach (KGVoucherL voucherL in set)
                        {
                            /*KGBillPayment billPayment = PXSelect<KGBillPayment,
                                Where<KGBillPayment.billPaymentID, Equal<Required<KGVoucherL.billPaymentID>>>>
                                .Select(Base, voucherL.BillPaymentID);
                            if (billPayment != null)
                            {
                                if (billPayment.PaymentPeriod == 0)
                                {
                                    voucherL.DueDate = filter.VoucherDate;
                                }
                                else
                                {
                                    voucherL.DueDate = 
                                        VoucherUntil.GetLongTremPaymentDate
                                        (Base, (DateTime)filter.VoucherDate, billPayment.PaymentPeriod);
                                }

                            }*/
                            voucherL.DueDate = invoice.DocDate;
                            Base.KGVoucherLs.Update(voucherL);
                            Base.Persist();

                        }

                        BAccount bAccount = Base.GetBAccount(voucherH.VendorID);
                        Contact contact = Base.GetContact(voucherH.VendorID);
                        //2019/12/18 ADD Company�אּ���M�שҸj�����q �Ӥ��O��e�n�J���q
                        Contract contract = Base.GetContract(voucherH.ContractID);
                        CSAnswers cSAnswers = Base.GetCSAnswers(voucherH.VendorID);
                        Address address = Base.GetAddress(voucherH.VendorID);
                        PXResultset<GVApGuiInvoice> gVApGuiInvoice = Base.GetGVApGuiInvoice(voucherH.RefNbr);
                        PXResultset<KGVoucherL> setvoucherL = PXSelect<KGVoucherL,
                            Where<KGVoucherL.voucherHeaderID, Equal<Required<KGVoucherH.voucherHeaderID>>>>
                            .Select(Base, voucherH.VoucherHeaderID);
                        int? branchID = contract.DefaultBranchID;
                        string UserName = AgentFlowWebServiceUtil.getName(Base.Accessinfo.UserName).ToUpper();
                        Branch branch = Base.GetBranch(branchID);
                        VoucherUntil.InsertToOracle(filter.VoucherDate, voucherH, setvoucherL,
                            contact, bAccount, address, cSAnswers, gVApGuiInvoice, branch.BranchCD.Trim(), UserName);

                        //����n�[�W
                        AllVouchers.Update(voucherH);
                        Base.Persist();
                        PXUpdate<Set<APRegisterExt.usrVoucherDate, Required<APRegisterExt.usrVoucherDate>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(Base, invoice.DocDate, voucherH.RefNbr);
                        PXUpdate<Set<APRegisterExt.usrVoucherKey, Required<APRegisterExt.usrVoucherKey>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(Base, voucherH.VoucherKey, voucherH.RefNbr);
                        PXUpdate<Set<APRegisterExt.usrVoucherNo, Required<APRegisterExt.usrVoucherNo>>,
                        APRegister, Where<APRegister.refNbr, Equal<Required<KGVoucherH.refNbr>>>>
                        .Update(Base, voucherH.VoucherNo, voucherH.RefNbr);
                        //}

                    }
                    Base.Persist();
                }
                ts.Complete();
            }
        }

        protected virtual void CheckVoucherSubID()
        {
            bool flag = true;
            PXResultset<ViewTableFin> viewt = getSelectedViewTableFin();
            foreach (ViewTableFin invoice in viewt)
            {
                KGVoucherH voucherH = GetVoucherH(invoice.RefNbr);
                foreach (KGVoucherL voucherL in PXSelect<KGVoucherL, Where<KGVoucherL.voucherHeaderID,
                        Equal<Required<KGVoucherH.voucherHeaderID>>>>.Select(Base, voucherH.VoucherHeaderID))
                {
                    if (voucherL.GetExtension<KGVoucherLFinExt>().UsrSubID == null)
                    {
                        Details.Cache.RaiseExceptionHandling<ViewTableFin.refNbr>
                            (invoice, invoice.RefNbr, new PXSetPropertyException($"{invoice.RefNbr}:�����ʤ֤l���", PXErrorLevel.RowError));
                        flag = false;
                        break;
                    }
                }
            }
            if (!flag)
            {
                throw new PXException($"�нT�{�����l���");
            }
        }

        #endregion

        #region Select Methods
        private PXResultset<ViewTableFin> getSelectedViewTableFin()
        {
            return PXSelect<ViewTableFin,
                Where<APRegister.selected, Equal<True>>,
                OrderBy<Asc<APRegister.refNbr>>>
                .Select(Base);
        }
        private KGVoucherH GetVoucherH(string refNbr)
        {
            return PXSelect<KGVoucherH,
                Where<KGVoucherH.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(Base, refNbr);
        }
        private APRegister GetRegister(string refNbr)
        {
            PXGraph graph = new PXGraph();
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(graph, refNbr);
        }
        private APRegister getDeductionADR(string refNbr)
        {
            PXGraph graph = new PXGraph();
            return PXSelect<APRegister,
                Where<APRegister.docType, Equal<APDocType.debitAdj>,
                And<APRegisterExt.usrIsDeductionDoc, Equal<True>,
                And<APRegisterExt.usrOriDeductionDocType, Equal<APDocType.invoice>,
                And<APRegisterExt.usrOriDeductionRefNbr, Equal<Required<APRegister.refNbr>>>>>>>
                .Select(graph, refNbr);
        }
        private APRegister getOrigInvoice(string DeductionRefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegisterFinExt.usrDeductionRefNbr, Equal<Required<APRegisterFinExt.usrDeductionRefNbr>>>>
                .Select(Base, DeductionRefNbr);
        }
        private GVApGuiInvoice GetGVInvoice(string refNbr)
        {
            return PXSelect<GVApGuiInvoice,
                Where<GVApGuiInvoice.refNbr, Equal<Required<GVApGuiInvoice.refNbr>>>>
                .Select(Base, refNbr);
        }
        private TWNWHTTran GetTWNWHTTran(string refNbr)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.refNbr, Equal<Required<TWNWHTTran.refNbr>>>>
                .Select(Base, refNbr);
        }
        private EPExpenseClaim GetEPClaim(string refNbr)
        {
            return PXSelect<EPExpenseClaim,
                Where<EPExpenseClaim.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>
                .Select(Base, refNbr);
        }

        #endregion

        #region HyperLink
        public PXAction<FilterForAP> ViewAP;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAP()
        {
            new HyperLinkUtil<APInvoiceEntry>(Details.Current, true);
        }

        public PXAction<FilterForAP> ViewAPADR;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPADR()
        {
            APInvoice inv = Details.Current;
            APRegisterFinExt finExt = PXCache<APRegister>.GetExtension<APRegisterFinExt>(inv);
            if (finExt.UsrDeductionRefNbr == null) return;
            // Creating the instance of the graph
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            graph.Document.Current = graph.Document.Search<APInvoice.refNbr>
                (finExt.UsrDeductionRefNbr, new object[] { APDocType.DebitAdj });

            new HyperLinkUtil<APInvoiceEntry>(graph.Document.Current, true);
        }

        public PXAction<FilterForAP> ViewEP;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewEP()
        {
            ViewTableFin details = Details.Current;
            if (details.OrigModule == KGConst.EP && details.DocType == APDocType.Invoice)
            {
                if (details.UsrEPRefNbr == null) return;
                ExpenseClaimEntry graph = PXGraph.CreateInstance<ExpenseClaimEntry>();
                graph.ExpenseClaim.Current = graph.ExpenseClaim.Search<EPExpenseClaim.refNbr>(details.UsrEPRefNbr);

                new HyperLinkUtil<ExpenseClaimEntry>(graph.ExpenseClaim.Current, true);
            }
            else
            {
                APInvoiceEntry apgraph = PXGraph.CreateInstance<APInvoiceEntry>();
                apgraph.Document.Current = apgraph.Document.Search<APInvoice.refNbr>(details.OrigRefNbr, details.OrigDocType);
                new HyperLinkUtil<APInvoiceEntry>(apgraph.Document.Current, true);
            }
        }

        #endregion
    }

    #region ViewTableFin
    [Serializable]
    [PXHidden]
    [PXProjection(typeof(Select2<APRegister,
        InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<APRegister.refNbr>>>,
            Where<APRegisterExt.usrIsDeductionDoc, Equal<False>,
            Or<APRegisterExt.usrIsDeductionDoc, IsNull>>>
        ), Persistent = false)]
    public class ViewTableFin : APInvoice
    {
        //For Show
        #region UsrEPRefNbr
        [PXDBString(BqlField = typeof(APInvoice.origRefNbr))]
        [PXUIField(DisplayName = "UsrEPRefNbr")]
        [PXSelector(typeof(Search<EPExpenseClaim.refNbr>),
            typeof(EPExpenseClaim.refNbr),
            typeof(EPExpenseClaim.docDate),
            DescriptionField = typeof(EPExpenseClaim.docDesc))]
        public virtual string UsrEPRefNbr { get; set; }
        public abstract class usrEPRefNbr : PX.Data.BQL.BqlString.Field<usrEPRefNbr> { }
        #endregion
    }
    #endregion

    #region FilterForAP Table
    [Serializable]
    public class FilterForAP : IBqlTable
    {
        #region ContractID
        [PXInt()]
        [PXUIField(DisplayName = "Project")]
        [PXSelector(typeof(Search2<PMProject.contractID,
                LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>,
                LeftJoin<ContractBillingSchedule, On<ContractBillingSchedule.contractID,
                Equal<PMProject.contractID>>>>,
                Where<PMProject.baseType, Equal<CTPRType.project>,
                 And<PMProject.nonProject, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>>)
                , typeof(PMProject.contractID), typeof(PMProject.contractCD), typeof(PMProject.description),
                typeof(Customer.acctName), typeof(PMProject.status),
                typeof(PMProject.approverID), SubstituteKey = typeof(PMProject.contractCD), ValidateValue = false)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region DocType
        [PXString(3, IsUnicode = true)]
        [PXUIField(DisplayName = "Doc Type")]
        [PXSelector(typeof(Search4<KGVoucherH.docType,
            Aggregate<GroupBy<KGVoucherH.docType>>>))]
        public virtual string DocType { get; set; }
        public abstract class docType : IBqlField { }
        #endregion

        #region RefNbr
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "RefNbr")]
        [PXSelector(typeof(Search<APInvoice.refNbr,
            Where<APInvoice.status, Equal<KGConst.balance>>>),
            typeof(APInvoice.docType),
            typeof(APInvoice.refNbr),
            typeof(APInvoice.docDesc),
            typeof(APInvoice.status),
            typeof(APInvoice.docDate),
            typeof(APInvoice.dueDate),
            typeof(APInvoice.tranPeriodID))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : IBqlField { }
        #endregion

        #region Vendor
        [PXInt()]
        [PXUIField(DisplayName = "Vendor")]
        [POVendor(Visibility = PXUIVisibility.SelectorVisible,
            DescriptionField = typeof(Vendor.acctName),
            CacheGlobal = true, Filterable = true)]
        public virtual int? Vendor { get; set; }
        public abstract class vendor : IBqlField { }
        #endregion

        #region DueDateFrom
        [PXDate()]
        [PXUIField(DisplayName = "Due Date From")]
        public virtual DateTime? DueDateFrom { get; set; }
        public abstract class dueDateFrom : IBqlField { }
        #endregion

        #region DueDateTo
        [PXDate()]
        [PXUIField(DisplayName = "Due Date To")]
        public virtual DateTime? DueDateTo { get; set; }
        public abstract class dueDateTo : IBqlField { }
        #endregion

        #region PONbr
        [PXString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PONbr")]
        [PXSelector(typeof(Search5<APRegisterExt.usrPONbr,
            InnerJoin<POOrder,
                On<POOrder.orderNbr, Equal<APRegisterExt.usrPONbr>>>,
                Where<APRegisterExt.usrPONbr, IsNotNull>,
            Aggregate<GroupBy<APRegisterExt.usrPONbr>>,
            OrderBy<Desc<APRegisterExt.usrPONbr>>>),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc))]
        public virtual string PONbr { get; set; }
        public abstract class pONbr : IBqlField { }
        #endregion

        #region VoucherDate
        [PXDate()]
        [PXUIField(DisplayName = "Voucher Date")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? VoucherDate { get; set; }
        public abstract class voucherDate : IBqlField { }
        #endregion

        #region UsrIsConfirm
        [PXInt]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrIsConfirm")]
        [PXIntList(new int[] { 0, 1 }, new string[] { "���T�{", "�w�T�{" })]

        public virtual int? UsrIsConfirm { get; set; }
        public abstract class usrIsConfirm : IBqlField { }
        #endregion

        #region Const
        public const int Int_0 = 0;
        public const int Int_1 = 1;
        public class int_0 : PX.Data.BQL.BqlInt.Constant<int_0> { public int_0() : base(Int_0) {; } }
        public class int_1 : PX.Data.BQL.BqlInt.Constant<int_1> { public int_1() : base(Int_1) {; } }
        #endregion

        //2021/02/19 Add Mantis:0011939
        #region UsrConfirmBy
        [PXGuid]
        [PXUIField(DisplayName = "UsrConfirmBy")]
        [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                typeof(PX.SM.Users.username),
                typeof(PX.SM.Users.firstName),
                typeof(PX.SM.Users.fullName),
                SubstituteKey = typeof(PX.SM.Users.username))]
        [PXUIEnabled(typeof(Where<usrIsConfirm, Equal<int_1>>))]

        public virtual Guid? UsrConfirmBy { get; set; }
        public abstract class usrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrConfirmBy> { }
        #endregion

        //2021/09/02 Add Mantis:0012222
        #region UsrVoucherNoFrom
        [PXString]
        [PXUIField(DisplayName = "UsrVoucherNo From")]
        public virtual string UsrVoucherNoFrom { get; set; }
        public abstract class usrVoucherNoFrom : PX.Data.BQL.BqlString.Field<usrVoucherNoFrom> { }
        #endregion

        #region UsrVoucherNoTo
        [PXString]
        [PXUIField(DisplayName = "UsrVoucherNo To")]
        public virtual string UsrVoucherNoTo { get; set; }
        public abstract class usrVoucherNoTo : PX.Data.BQL.BqlString.Field<usrVoucherNoTo> { }
        #endregion
    }
    #endregion
}