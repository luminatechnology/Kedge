using System;
using Kedge.DAC;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.PM;
using PX.Objects.GL;
using PX.Objects.FS;
using System.Collections;
using System.Collections.Generic;
using RC.Util;
using PX.Data.EP;
using PX.Objects.AP.MigrationMode;
using PaymentMethod = Kedge.DAC.PaymentMethod;

namespace NM
{
    /**
     * ===20210113===
     * 1.���͹q�שһݪ��Ȧ�b��²�X���ParamTable�M�w�A��ƤW���Ȧ�b��²�X�ȨѰѦ�
     * 2.VendorLocationID���A�P�B�^KGBillPayment
     * 3.Log�B�~������e��VendorLocationID & BankAccountID
     * 
     * ===2021-02-20:11958=====Alton
     * TT�q���ɲ��ͧ@�~ ��IsCheckIssue=True�ɡA���i�H�i�桥��}�����@�~
     * 
     * ===2021-03-02:11864=====Alton
     * 1.��}���ɡA�R���Ҧ���NMApTeleTransLog
     * 
     * ====2021-04-19:12015 ====Alton
     * NM�妸�}�� �� TT�q�� ��OriCuryAmount�A���Ч��KGBillPayment��ActPayAmt
     * 
     * ====2021-04-23:12016 ====Alton
     * 4.�]��KGBillPayment����APPayment/�q����/���I���ڦp�G���̾�KGBillpayment.VendorLocationID�h�g�JVendor�������,
     *   �Ш̷�APRegister.VendorID����, KGBillPayment.VendorID�O�|�M�Ψ�q�׹�I�ڹ�H, ���ڪ��䲼���Y(���ڥD�ɪ�Vendor�٬O��APRegister.Vendor)
     * - �Ȧ�b�������T�Ӧ۩�KGBillPayment.VendorLocationID
     * 
     * ====2021-04-29:12025 ====Alton
     * 1.GRID�PFilter�[�J UsrTrPaymntType & UsrTrConfirmID
     * 2.BatchNbr����ܧאּUsrAccConfirmNbr
     * 
     * ====2021-05-19:12049 ====Alton
     * 1. �w��z�LVendorlocation�a�X�I�ڱb����VendorLocationID LOV�e���зs�W�@�����UsrIsReserveAcct, ���Acheckbox, �Щ�b�Ĥ@�����.LOV Order by UsrIsReserveAcct(����), LocationCD(�ɾ�)
     * 2.�г·б��@�U�U��screen, �u�n�w��t�өάO���u(�I�ڹ�H)��LOV�վ�
     * 
     * ====2021-05-28:12069 ====Alton
     * 1.NM502000 ���I���ڧ妸�}��, NM502007�Ȧ�q���ɧ@�~, ���n��ܭp����w���઺���
     * 2.�P�_�p����O�_�w����, �N�O�u�n��즳DocType = 'ADR' and OrigRefNbr = ��p���渹 and OrigDocType = 'INV', �h�ӭp����N�����L����
     * 
     * ====2021-06-07:�f�Y====Alton
     * �ɦW��^��
     * ====2021-07-22:12167====Alton
     * �J��o���@�~������Цh�[ KGBillPayment.UsrIsTrConfirm �n����True
     * 
     * ====2021-07-28:12174====Alton
     * 1.�Цb���Y�u�q�׸�ơvgroup���A�s�W�u�I�ڤ���v���A�޿�P�ۦ���u�Ȧ�b��²�X�v���A��ϥΪ̬D��I�ڤ���ɡA�бN�U��grid���ҿ��ƪ��I�ڤ�����������A���ʧ@������Ф@�_�g�b�u���͹q���ɡv��button��
     * 2.�u�I�ڤ���v��줣�ݶ�J�w�]�ȡA�����P�_����즳�ȮɡA�~�ݭn����W�z�����I�ڤ骺�ʧ@
     * 3.����u���͹q���ɡv�ɪ�����dialog��r�A�ץ��p�U�G
     *   ��I�ڤ���S���ȮɡA�к����u�T�w�O�_�N�Ŀ諸��Ʋ��͹q���ɡH�v
     *   ��I�ڤ�����ȮɡA�Чאּ�u�T�w�O�_�N�Ŀ諸��ơu���͹q���ɡv�áu���ʥI�ڤ���v�H�v
     *   
     * ====2021-12-15:Eva�f�z====Alton
     * �q�פ��ݤ���O�]����b
     * **/
    public class NMApTeleTransProcess : PXGraph<NMApTeleTransProcess>
    {
        public PXCancel<ParamTable> Cancel;
        public NMApTeleTransProcess()
        {
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.CreateTTBtn);
            this.ActionMenu.AddMenuAction(this.ChangeToCheckBtn);
        }

        #region Const Message
        public const String BankAccountCanNotBeNull = "�п�J�Ȧ�b��²�X";
        public const String TheBankCanNotCreateTT = "�ӻȦ�L�k����q�ק@�~";
        #endregion

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXSelect<NMPayableTTV, Where<IsNull<NMPayableTTV.isCheckIssue, False>, Equal<Current2<ParamTable.isCheckIssue>>,
                    And2<Where<
                        NMPayableTTV.vendorID, Equal<Current2<ParamTable.vendorID>>,
                        Or<Current2<ParamTable.vendorID>, IsNull>>,
                    And2<Where<
                        NMPayableTTV.projectID, Equal<Current2<ParamTable.projectID>>,
                        Or<Current2<ParamTable.projectID>, IsNull>>,
                    And2<Where<
                        NMPayableTTV.paymentMethodID, Equal<Current2<ParamTable.paymentMethodID>>,
                        Or<Current2<ParamTable.paymentMethodID>, IsNull>>,
                    And2<Where<
                        NMPayableTTV.cashAccountID, Equal<Current2<ParamTable.cashAccountID>>,
                        Or<Current2<ParamTable.cashAccountID>, IsNull>>,
                    And2<Where<Current2<ParamTable.payDate>, IsNull,
                        Or<NMPayableTTV.paymentDate, LessEqual<Current2<ParamTable.payDate>>>>,
                    And2<Where<Current2<ParamTable.dueDate>, IsNull,
                        Or<NMPayableTTV.dueDate, Equal<Current2<ParamTable.dueDate>>>>,
                    And2<Where<Current2<ParamTable.usrTrConfirmBy>, IsNull,
                        Or<NMPayableTTV.usrTrConfirmBy, Equal<Current2<ParamTable.usrTrConfirmBy>>>>,
                    And2<Where<Current2<ParamTable.usrTrConfirmDate>, IsNull,
                        Or<NMPayableTTV.usrTrConfirmDate, Equal<Current2<ParamTable.usrTrConfirmDate>>>>,
                    And2<Where<Current2<ParamTable.usrTrConfirmID>, IsNull,
                        Or<NMPayableTTV.usrTrConfirmID, Equal<Current2<ParamTable.usrTrConfirmID>>>>,
                    And<Where<Current2<ParamTable.usrTrPaymentType>, IsNull,
                        Or<NMPayableTTV.usrTrPaymentType, Equal<Current2<ParamTable.usrTrPaymentType>>>>
                    >>>>>>>>>>>> DetailsView;
        public PXSelect<NMApTeleTransLog> Logs;
        #endregion

        #region Action
        #region Menu
        public PXAction<ParamTable> ActionMenu;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "���")]
        protected void actionMenu() { }
        #endregion

        #region Button
        public PXAction<ParamTable> ChangeToCheckBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "��}��")]
        protected IEnumerable changeToCheckBtn(PXAdapter adapter)
        {
            if (MasterView.Ask("�T�w�אּ�}��", "�T�w�O�_�N�Ŀ��T��ܶ}���@�~�H", MessageButtons.YesNo) == WebDialogResult.Yes)
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    PXSelectBase<NMPayableTTV> cmd = new PXSelect<NMPayableTTV, Where<NMPayableTTV.selected, Equal<True>>>(this);
                    PXResultset<NMPayableTTV> rs = cmd.Select();
                    foreach (NMPayableTTV item in rs)
                    {
                        PXUpdate<
                            Set<KGBillPaymentExt.usrIsCheckIssued, False,
                            Set<KGBillPayment.paymentMethod, Kedge.DAC.PaymentMethod.check,
                            Set<KGBillPayment.vendorLocationID, Required<KGBillPayment.vendorLocationID>>>>,
                        KGBillPayment,
                        Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                        .Update(this,
                            NMLocationUtil.GetDefLocationByPaymentMethod(item.VendorID, NMLocationUtil.PaymentMethodID.CHECK),
                            item.BillPaymentID);
                        NMBatchNbrUtil.UpdateBillPaymentBatchNbr(this, "", item.BillPaymentID);

                        //add by altion :11864 �R���Ҧ�log����
                        foreach (NMApTeleTransLog log in GetLogByBillpaymentID(item.BillPaymentID)) 
                        {
                            Logs.Delete(log);
                        }
                    }
                    DetailsView.Cache.Clear();
                    MasterView.Cache.Clear();
                });
            }
            return adapter.Get();
        }
        public PXAction<ParamTable> CreateTTBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "���͹q����")]
        protected IEnumerable createTTBtn(PXAdapter adapter)
        {
            ParamTable p = MasterView.Current;

            if (p.BankAccountID == null)
            {
                MasterView.Cache.RaiseExceptionHandling<ParamTable.bankAccountID>(
                                    p, null,
                                    new PXSetPropertyException(BankAccountCanNotBeNull, PXErrorLevel.Error));
                throw new PXException(BankAccountCanNotBeNull);
            }
            if (p.PaymentDate != null)
            {
                if (MasterView.Ask("���͹q��", "�T�w�O�_�N�Ŀ諸��ơu���͹q���ɡv�áu���ʥI�ڤ���v�H", MessageButtons.YesNo) == WebDialogResult.Yes)
                {
                    DoCreateTT(p);
                }
            }
            else {
                if (MasterView.Ask("���͹q��", "�T�w�O�_�N�Ŀ諸��Ʋ��͹q���ɡH", MessageButtons.YesNo) == WebDialogResult.Yes)
                {
                    DoCreateTT(p);
                }
            }
            return adapter.Get();
        }


        //public PXAction<ParamTable> CreateTTByTNBtn;
        //[PXButton(CommitChanges = true)]
        //[PXUIField(DisplayName = "���ͥx�s�q����")]
        //protected IEnumerable createTTByTNBtn(PXAdapter adapter)
        //{
        //    if (MasterView.Ask("���ͥx�s�q��", "�T�w�O�_�N�Ŀ�겣�͡u�x�s�q���ɡv�H", MessageButtons.YesNo) == WebDialogResult.Yes)
        //    {
        //        TTClass ttClass = TTClass.TN;
        //        List<NMPayableTTV> items = GetSelectedItem(ttClass);
        //        PXLongOperation.StartOperation(this, delegate () { dowloadTXT(ttClass, items); });
        //    }
        //    return adapter.Get();
        //}

        //public PXAction<ParamTable> CreateTTByGTBtn;
        //[PXButton(CommitChanges = true)]
        //[PXUIField(DisplayName = "���Ͱ���q����")]
        //protected IEnumerable createTTByGTBtn(PXAdapter adapter)
        //{
        //    if (MasterView.Ask("���ͥx�s�q��", "�T�w�O�_�N�Ŀ�겣�͡u����q���ɡv�H", MessageButtons.YesNo) == WebDialogResult.Yes)
        //    {
        //        TTClass ttClass = TTClass.GT;
        //        List<NMPayableTTV> items = GetSelectedItem(ttClass);
        //        PXLongOperation.StartOperation(this, delegate () { dowloadTXT(ttClass, items); });
        //    }
        //    return adapter.Get();
        //}


        #endregion

        #region HyperLink
        public PXAction<NMPayableTTV> ViewBatch;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewBatch()
        {
            //Batch batch = GetConfirmBranch(DetailsView.Current.UsrAccConfirmNbr);
            Batch batch = (Batch)PXSelectorAttribute.Select<NMPayableTTV.usrAccConfirmNbr>(DetailsView.Cache, DetailsView.Current);
            new HyperLinkUtil<JournalEntry>(batch, true);
        }


        //private void HyperLink<Graph>(object current, bool isNewWindow) where Graph : PXGraph, new()
        //{
        //    Graph graph = PXGraph.CreateInstance<Graph>();
        //    string viewName = graph.PrimaryView;
        //    graph.GetPrimaryCache().Current = current;
        //    if (graph.GetPrimaryCache().Current != null)
        //    {
        //        if (isNewWindow)
        //            throw new PXRedirectRequiredException(graph, typeof(Graph).Name)
        //            {
        //                Mode = PXBaseRedirectException.WindowMode.NewWindow
        //            };
        //        else throw new PXRedirectRequiredException(graph, typeof(Graph).Name);
        //    }
        //}
        #endregion


        #endregion

        #region Event
        protected virtual void _(Events.RowSelected<ParamTable> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            //ChangeToCheckBtn.SetEnabled((row.IsCheckIssue ?? false) == false);
        }

        protected virtual void _(Events.FieldUpdated<ParamTable.selectBankAccountID> e)
        {
            ParamTable row = (ParamTable)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<ParamTable.paymentMethodID>(row);
            e.Cache.SetDefaultExt<ParamTable.cashAccountID>(row);
        }

        protected virtual void _(Events.RowSelected<NMPayableTTV> e)
        {
            NMPayableTTV row = (NMPayableTTV)e.Row;
            if (row == null) return;
            setReadOnly();
        }

        protected virtual void _(Events.FieldUpdated<NMPayableTTV.vendorLocationID> e)
        {
            NMPayableTTV row = (NMPayableTTV)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<NMPayableTTV.cashAccountID>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.paymentMethodID>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.acctName>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.bankAcct>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.bankName>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.bankNbr>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.branchNam>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.branchNbr>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.categoryID>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.category>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.charge>(row);
            e.Cache.SetDefaultExt<NMPayableTTV.bankAccountID>(row);
        }

        //protected virtual void _(Events.FieldDefaulting<NMPayableTTV.bankAccountID> e)
        //{
        //    NMPayableTTV row = (NMPayableTTV)e.Row;
        //    if (row == null) return;
        //    NMBankAccount nba = GetBankAccount(row.CashAccountID, row.PaymentMethodID);
        //    e.NewValue = nba?.BankAccountID;
        //}

        #endregion

        #region Method
        public void DoCreateTT(ParamTable p) {
            TTClass ttClass;
            NMBankAccount ba = (NMBankAccount)PXSelectorAttribute.Select<ParamTable.bankAccountID>(MasterView.Cache, p);
            switch (ba.BankCode.Substring(0, 3))
            {
                case ParamTable.TN:
                    ttClass = TTClass.TN;
                    break;
                case ParamTable.GT:
                    ttClass = TTClass.GT;
                    break;
                default:
                    MasterView.Cache.RaiseExceptionHandling<ParamTable.bankAccountID>(
                                p, ba.BankAccountCD,
                                new PXSetPropertyException(TheBankCanNotCreateTT, PXErrorLevel.Error));
                    throw new PXException(TheBankCanNotCreateTT);
            }

            List<NMPayableTTV> items = GetSelectedItem(ttClass);
            PXLongOperation.StartOperation(this, delegate () { dowloadTXT(ttClass, items); });
        }

        /// <summary>
        /// ���o���Ҧ��\�᪺�Q������
        /// </summary>
        /// <returns></returns>
        public virtual List<NMPayableTTV> GetSelectedItem(TTClass ttClass)
        {
            ParamTable p = MasterView.Current;
            List<NMPayableTTV> items = new List<NMPayableTTV>();
            PXSelectBase<NMPayableTTV> cmd = new PXSelect<NMPayableTTV, Where<NMPayableTTV.selected, Equal<True>>>(this);
            PXResultset<NMPayableTTV> set = cmd.Select();
            //���ҹq�׸�T
            foreach (NMPayableTTV item in set)
            {
                String error = null;
                if (item.PaymentMethodID != NMPayableTTV.TT)
                {
                    error = "�I�ڤ覡�D�q��(" + item.RefNbr + ")";
                }
                else if (String.IsNullOrEmpty(item.BankAcct)
                  /*--2021-12-15 : �q�פ��ݤ���O�]����b
                  || String.IsNullOrEmpty(item.BankName)
                  || String.IsNullOrEmpty(item.BranchNbr)
                  || String.IsNullOrEmpty(item.BranchNam)
                  */
                  || String.IsNullOrEmpty(item.Category)
                  || String.IsNullOrEmpty(item.CategoryID)
                  //|| String.IsNullOrEmpty(item.Charge)
                  )
                {
                    error = "�к��@�q�׸�T(" + item.RefNbr + ")";
                }
                else
                {
                    items.Add(item);
                }
                if (error != null)
                {
                    DetailsView.Cache.RaiseExceptionHandling<NMPayableTTV.refNbr>(
                                    item, item.RefNbr, new PXSetPropertyException(error, PXErrorLevel.RowError));
                }
            }
            if (set.Count != items.Count)
            {
                String error = "���ͥ���";
                throw new PXException(error);
            }
            return items;
        }

        /// <summary>
        /// �U���q����
        /// </summary>
        /// <param name="ttClass"></param>
        /// <param name="items"></param>
        public virtual void dowloadTXT(TTClass ttClass, List<NMPayableTTV> items)
        {

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (NMPayableTTV item in items)
                {
                    //Stage 1.��sKGBillPaymentExt.UsrIsCheckIssued = true (�N��w�Q�ϥ�)
                    PXUpdate<
                        Set<KGBillPaymentExt.usrIsCheckIssued, True>,
                        KGBillPayment,
                    Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                    .Update(this, item.BillPaymentID);
                    //Stage 2.����APPayment
                    //mantis:11864 �אּ�^�X�צ^�ӽT�{�ɲ���
                    //CreateAPPayment(item);
                    //Stage 3.��s�¬��� & �g�J�O����
                    item.LogID = CreateTeleTransLog(item, ttClass);
                }

                RCEleTxtFileUtil util = null;
                //Stage 4.���͹q����
                switch (ttClass)
                {
                    case TTClass.TN:
                        util = doTN(items);
                        break;
                    case TTClass.GT:
                        util = doGT(items);
                        break;
                }
                if (util != null)
                {
                    ts.Complete();
                    util.Dowload();
                }
                DetailsView.Cache.Clear();
                MasterView.Cache.Clear();
            }
        }


        public virtual int? CreateTeleTransLog(NMPayableTTV item, TTClass ttClass)
        {
            ParamTable P = MasterView.Current;
            //��sNMApTeleTransLog �N isNew = true�אּfalse
            PXUpdate<Set<NMApTeleTransLog.isNew, False>,
                NMApTeleTransLog,
                Where<NMApTeleTransLog.billPaymentID, Equal<Required<NMApTeleTransLog.billPaymentID>>,
                And<NMApTeleTransLog.isNew, Equal<True>>>>
                .Update(this, item.BillPaymentID);
            //�s�WNMApTeleTransLog
            NMApTeleTransLog log = new NMApTeleTransLog();
            log.BillPaymentID = item.BillPaymentID;
            log.TeleTransRefNbr = item.RefNbr;
            log.VendorLocationID = item.VendorLocationID;
            log.BankAccountID = P.BankAccountID;
            if (ttClass == TTClass.TN)
            {
                log.Type = NMStringList.NMApTtLogType.TN;
            }
            else if (ttClass == TTClass.GT)
            {
                log.Type = NMStringList.NMApTtLogType.GT;
            }
            log.Status = NMStringList.NMApTtLogStatus.CREATED;
            log.IsNew = true;
            log = Logs.Insert(log);
            this.Persist();
            return Logs.Current.LogID;
        }

        public virtual RCEleTxtFileUtil doTN(List<NMPayableTTV> items)
        {
            //String mmdd = this.Accessinfo.BusinessDate?.ToString("MMdd");
            //2021-01-29 Add By Alton : 11918
            String batchNbr = NMBatchNbrUtil.GetNextBatchNbr(this);
            String branchCD = GetBranch(this.Accessinfo.BranchID)?.BranchCD ?? "";
            ParamTable param = MasterView.Current;
            RCEleTxtFileUtil util = new RCEleTxtFileUtil("Taishin_TT_" + branchCD.Trim() + "_" + batchNbr, "BIG5");
            NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<ParamTable.bankAccountID>(MasterView.Cache, param);
            
            foreach (NMPayableTTV item in items)
            {
                DateTime? _paymentDate = param.PaymentDate ?? item.PaymentDate;
                //2021-01-29 Add By Alton : 11918 ��sKGBillPayment.batchNbr
                NMBatchNbrUtil.UpdateBillPaymentBatchNbr(this, batchNbr, item.BillPaymentID);
                //NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableTTV.bankAccountID>(DetailsView.Cache, item);
                Branch branch = (Branch)PXSelectorAttribute.Select<NMPayableTTV.branchID>(DetailsView.Cache, item);
                BAccount branchBAccount = GetBaccount(branch.BAccountID);
                //�Τ�ۭq�Ǹ�(7) modify by louis 20220330 �]���x�s�W�Ǧ���쥲���O�ߤ@��, �ҥH����έp���渹, ���Numbering 'NMTELCUST'

                //String custNbr = Guid.NewGuid().ToString().Substring(0, 7);
                String custNbr = AutoNumberAttribute.GetNextNumber(DetailsView.Cache, item, "NMTELCUST", this.Accessinfo.BusinessDate);
                //String custNbr = RCEleTxtFileUtil.GetDataStrByByte(item.RefNbr.Replace("AP", ""), 7, false, util.ENCODING);
                //�I�ڤ��(8)
                String payDate = RCEleTxtFileUtil.GetDataStrByByte(_paymentDate?.ToString("yyyyMMdd"), 8, false, util.ENCODING);
                //�I�ڪ��B(18)
                String payAmt = RCEleTxtFileUtil.GetDataStrByByte(item.OriCuryAmount?.ToString("0.00").Replace(".", ""), 18, true, '0', util.ENCODING);
                //�I�ڱb��(17)
                String payBankAcct = RCEleTxtFileUtil.GetDataStrByByte(nba.BankAccount, 17, false, util.ENCODING);
                //�I�ڤ�W(60)
                String payBankAcctName = RCEleTxtFileUtil.GetDataStrByByte(nba.AccountName, 60, false, util.ENCODING);
                //���ڱb��(17)--�Ȧ�b��
                String recBankAccot = RCEleTxtFileUtil.GetDataStrByByte(item.BankAcct, 17, false, util.ENCODING);
                //���ڤ�W(60)--�b��W��
                String recAccName = RCEleTxtFileUtil.GetDataStrByByte(item.AcctName, 60, false, util.ENCODING);
                //�I���`��(3)
                String payBankCode = RCEleTxtFileUtil.GetDataStrByByte(nba.BankCode.Substring(0, 3), 3, false, util.ENCODING);
                //�I�ڤ���(4)
                String payBranchCode = RCEleTxtFileUtil.GetDataStrByByte(nba.BankCode.Substring(3, 4), 4, false, util.ENCODING);
                //�����`��(3)--�Ȧ�N�X
                String recBankCode = RCEleTxtFileUtil.GetDataStrByByte(item.BankNbr, 3, false, util.ENCODING);
                //���ڤ���(4)--�Ȧ�O
                String recBranchCode = RCEleTxtFileUtil.GetDataStrByByte(item.BranchNbr, 4, false, util.ENCODING);
                //����(100)
                String desc = RCEleTxtFileUtil.GetDataStrByByte("", 100, false, util.ENCODING);
                //���ڤH�ѧO�X(17)--���Ҹ��X
                String recIDNbr = RCEleTxtFileUtil.GetDataStrByByte(item.CategoryID, 17, false, util.ENCODING);
                //���ڤH�N�X�ѧO(3)--�������O 53:�@��/�����b���B58�G�Τ@�s���B174�G�����Ҧr��
                String recIDNbrType = RCEleTxtFileUtil.GetDataStrByByte(
                    item.Category == "1" ? "58" :
                    item.Category == "2" ? "174" :
                    item.Category == "3" ? "53" : ""
                    , 3, false, util.ENCODING);
                //�I�ڤH�ѧO�X(17)
                String payIDNbr = RCEleTxtFileUtil.GetDataStrByByte(branchBAccount.TaxRegistrationID, 17, false, util.ENCODING);
                //�w�ڤH�N�X�ѧO(3)
                String payIDNbrType = RCEleTxtFileUtil.GetDataStrByByte("58", 3, false, util.ENCODING);
                //����O�t��O(3) 13�G���ڤH�t��B15�G�I�ڤH�t��
                String transFeeType = RCEleTxtFileUtil.GetDataStrByByte(item.Charge, 3, false, util.ENCODING);
                //Key��30 --2021-01-14��slogID
                String key = RCEleTxtFileUtil.GetDataStrByByte(item.LogID.ToString(), 30, false, util.ENCODING);
                //�I���p���H(35)
                String payCon = RCEleTxtFileUtil.GetDataStrByByte("", 35, false, util.ENCODING);
                //�I���p���q��(25)
                String payTel = RCEleTxtFileUtil.GetDataStrByByte("", 25, false, util.ENCODING);
                //�I�ڶǯu���X(25)
                String payFax = RCEleTxtFileUtil.GetDataStrByByte("", 25, false, util.ENCODING);
                //�����p���H(35)
                String recCon = RCEleTxtFileUtil.GetDataStrByByte("", 35, false, util.ENCODING);
                //���ڳs���q��(25)
                String recTel = RCEleTxtFileUtil.GetDataStrByByte("", 25, false, util.ENCODING);
                //���ڶǯu���X(25)
                String recFax = RCEleTxtFileUtil.GetDataStrByByte("", 25, false, util.ENCODING);
                //���ڳq��email(50)
                String recEmail = RCEleTxtFileUtil.GetDataStrByByte("", 50, false, util.ENCODING);

                util.InputLine(
                      custNbr            //�Τ�ۭq�Ǹ�
                    + payDate            //�I�ڤ��
                    + payAmt             //�I�ڪ��B
                    + payBankAcct        //�I�ڱb��
                    + payBankAcctName    //�I�ڤ�W
                    + recBankAccot       //���ڱb��
                    + recAccName         //���ڤ�W
                    + payBankCode        //�I���`��
                    + payBranchCode      //�I�ڤ���
                    + recBankCode        //�����`��
                    + recBranchCode      //���ڤ���
                    + desc               //����
                    + recIDNbr           //���ڤH�ѧO�X
                    + recIDNbrType       //���ڤH�N�X�ѧO
                    + payIDNbr           //�I�ڤH�ѧO�X
                    + payIDNbrType       //�I�ڤH�N�X�ѧO
                    + transFeeType       //����O�t��O
                    + key                //��b��Key��
                    + payCon             //�I���p���H
                    + payTel             //�I���p���q��
                    + payFax             //�I�ڶǯu���X
                    + recCon             //�����p���H
                    + recTel             //�����p���q��
                    + recFax             //���ڶǯu���X
                    + recEmail           //����email
                    );
            }
            //util.Dowload();
            return util;
        }

        public virtual RCEleTxtFileUtil doGT(List<NMPayableTTV> items)
        {
            //String mmdd = this.Accessinfo.BusinessDate?.ToString("MMdd");
            //2021-01-29 Add By Alton : 11918
            String batchNbr = NMBatchNbrUtil.GetNextBatchNbr(this);
            String branchCD = GetBranch(this.Accessinfo.BranchID)?.BranchCD ?? "";
            ParamTable param = MasterView.Current;
            RCEleTxtFileUtil util = new RCEleTxtFileUtil("Cathay_TT_" + branchCD.Trim() + "_" + batchNbr, "BIG5");
            NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<ParamTable.bankAccountID>(MasterView.Cache, param);
            foreach (NMPayableTTV item in items)
            {
                DateTime? _paymentDate = param.PaymentDate ?? item.PaymentDate;
                //2021-01-29 Add By Alton : 11918 ��sKGBillPayment.batchNbr
                NMBatchNbrUtil.UpdateBillPaymentBatchNbr(this, batchNbr, item.BillPaymentID);
                //NMBankAccount nba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableTTV.bankAccountID>(DetailsView.Cache, item);
                Branch branch = (Branch)PXSelectorAttribute.Select<NMPayableTTV.branchID>(DetailsView.Cache, item);
                BAccount branchBAccount = GetBaccount(branch.BAccountID);
                //�ѧO�N�X(1) 2
                String code = "2";
                //�ɮפW�Ǥ��(8)
                String uploadDate = RCEleTxtFileUtil.GetDataStrByByte(this.Accessinfo.BusinessDate?.ToString("yyyyMMdd"), 8, false, util.ENCODING);
                //�w�w������(8)
                String paymentDate = RCEleTxtFileUtil.GetDataStrByByte(_paymentDate?.ToString("yyyyMMdd"), 8, false, util.ENCODING);
                //������O(3)
                String tranType = "SPU";
                //����s��(10) -- 2021-01-14 ��sLogID
                String tranNbr = RCEleTxtFileUtil.GetDataStrByByte(item.LogID.ToString(), 10, false, util.ENCODING);
                //�I�ڦ�N�X(7)
                String payCode = RCEleTxtFileUtil.GetDataStrByByte(nba.BankCode, 7, false, util.ENCODING);
                //�I�ڤH�b��(16)
                String payBankAcct = RCEleTxtFileUtil.GetDataStrByByte(nba.BankAccount, 7, false, util.ENCODING);
                //�I�ڤH�νs(10)
                String paTaxIDNbr = RCEleTxtFileUtil.GetDataStrByByte(branchBAccount.TaxRegistrationID, 10, false, util.ENCODING);
                //�I�ڤH��W(70)
                String paAcctName = RCEleTxtFileUtil.GetDataStrByByte(nba.AccountName, 70, false, util.ENCODING);
                //���O(3)
                String curyID = "TWD";
                //���B���t��(1)
                String amtSign = "+";
                //���B(14)
                String amount = RCEleTxtFileUtil.GetDataStrByByte(item.OriCuryAmount?.ToString("0.00").Replace(".", ""), 14, true, '0', util.ENCODING);
                //���ڦ�N�X(7) -- �Ȧ�N�X+�Ȧ�O
                String recBankCode = RCEleTxtFileUtil.GetDataStrByByte(item.BankNbr + item.BranchNbr, 7, false, util.ENCODING);
                //���ڤH�b��(16) -- �Ȧ�b��
                String recBankAccot = RCEleTxtFileUtil.GetDataStrByByte(item.BankAcct, 16, false, util.ENCODING);
                //���ڤH�νs(10) -- ���Ҹ��X
                String recTaxIDNbr = RCEleTxtFileUtil.GetDataStrByByte((item.Category == "1" ? item.CategoryID : ""), 10, false, util.ENCODING);
                //���ڤH��W(70) -- �b��W��
                String recName = RCEleTxtFileUtil.GetDataStrByByte(item.AcctName, 70, false, util.ENCODING);
                //���ڤH�O�_�q�i(1)  ���G���q���B���G��-mail�B���GFAX�B���GShort Message
                String recEMsgType = "0";
                //�q�i�]�Ƹ��X(50)
                String eMsgEquNbr = RCEleTxtFileUtil.GetDataStrByByte("", 50, false, util.ENCODING);
                //����O���u�覡(2) 15�G�׶O�~�[�B13�G�׶O����
                String transFeeType = RCEleTxtFileUtil.GetDataStrByByte(item.Charge, 2, false, util.ENCODING);
                //�o������(4)
                String invoiceCount = "0000";
                //�Ƶ�(50)
                String desc = RCEleTxtFileUtil.GetDataStrByByte("�@", 50, false, util.ENCODING);

                util.InputLine(
                          code             //�ѧO�N�X
                        + uploadDate       //�ɮפW�Ǥ��
                        + paymentDate      //�w�w������
                        + tranType         //������O
                        + tranNbr          //����s��
                        + payCode          //�I�ڦ�N�X
                        + payBankAcct      //�I�ڤH�b��
                        + paTaxIDNbr       //�I�ڤH�νs
                        + paAcctName       //�I�ڤH��W
                        + curyID           //���O
                        + amtSign          //���B���t��
                        + amount           //���B
                        + recBankCode      //���ڦ�N�X
                        + recBankAccot     //���ڤH�b��
                        + recTaxIDNbr      //���ڤH�νs
                        + recName          //���ڤH��W
                        + recEMsgType      //���ڤH�O�_�q�i
                        + eMsgEquNbr       //�q�i�]�Ƹ��X
                        + transFeeType     //����O���u�覡
                        + invoiceCount     //�o������
                        + desc             //�Ƶ�
                    );
            }
            //util.Dowload();
            return util;
        }

        public void setReadOnly()
        {
            ParamTable p = MasterView.Current;
            NMPayableTTV row = DetailsView.Current;
            PXUIFieldAttribute.SetEnabled(DetailsView.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<NMPayableTTV.selected>(DetailsView.Cache, row, true);
            PXUIFieldAttribute.SetEnabled<NMPayableTTV.vendorLocationID>(DetailsView.Cache, row, true);
            DetailsView.AllowDelete = false;
            DetailsView.AllowInsert = false;
        }
        #endregion

        #region BQL
        public PXResultset<NMApTeleTransLog> GetLogByBillpaymentID(int? billpaymentID)
        {
            return PXSelect<NMApTeleTransLog,
            Where<NMApTeleTransLog.billPaymentID, Equal<Required<NMApTeleTransLog.billPaymentID>>>>
               .Select(this, billpaymentID);
        }

        public NMBankAccount GetBankAccount(int? cashAccountID, string payMethodID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.isSettlement, Equal<False>,
                And<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>,
                And<NMBankAccount.paymentMethodID, Equal<Required<NMBankAccount.paymentMethodID>>,
                And<Where<
                    NMBankAccount.bankCode, Like<ParamTable.likeTN>,
                    Or<NMBankAccount.bankCode, Like<ParamTable.likeGT>>>>
                >>>>
                .Select(this, cashAccountID, payMethodID);
        }

        public BAccount GetBaccount(int? baccountID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(new PXGraph(), baccountID);
        }

        public Branch GetBranch(int? branchID)
        {
            return PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>
                .Select(this, branchID);
        }

        #endregion

        #region enum
        public enum TTClass
        {
            /// <summary>
            /// �x�s
            /// </summary>
            TN,
            /// <summary>
            /// ���
            /// </summary>
            GT
        }
        #endregion

        #region Table

        #region ParamTable
        [Serializable]
        [PXHidden]
        public class ParamTable : IBqlTable
        {
            #region TT Info
            #region BankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Bank Account ID")]
            [NMBankAccount]
            [PXRestrictor(typeof(Where<NMBankAccount.paymentMethodID, Equal<NMPayableTTV.tt>>), "PaymentMethod Not TT.", typeof(NMBankAccount.paymentMethodID))]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

            #region PaymentDate
            [PXDate()]
            [PXUIField(DisplayName = "Payment Date")]
            public virtual DateTime? PaymentDate { get; set; }
            public abstract class paymentDate : PX.Data.BQL.BqlDateTime.Field<paymentDate> { }
            #endregion
            #endregion

            #region Selection Area
            #region SelectBankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Bank Account ID")]
            [NMBankAccount]
            //[PXRestrictor(typeof(Where<NMBankAccount.bankCode, Like<likeTN>, Or<NMBankAccount.bankCode, Like<likeGT>>>), "Bank Code Not Found.", typeof(NMBankAccount.bankCode))]
            public virtual int? SelectBankAccountID { get; set; }
            public abstract class selectBankAccountID : PX.Data.BQL.BqlInt.Field<selectBankAccountID> { }
            #endregion

            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor ID")]
            [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>, Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>),
                typeof(BAccount2.acctCD),
                typeof(BAccount2.acctName),
                typeof(BAccount2.status),
                typeof(BAccount2.defAddressID),
                typeof(BAccount2.defContactID),
                typeof(BAccount2.defLocationID),
                        SubstituteKey = typeof(BAccount.acctCD),
                        DescriptionField = typeof(BAccount.acctName))]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
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

            #region PayDate
            [PXDate()]
            [PXUIField(DisplayName = "Pay Date")]
            public virtual DateTime? PayDate { get; set; }
            public abstract class payDate : PX.Data.BQL.BqlDateTime.Field<payDate> { }
            #endregion

            #region DueDate
            [PXDate()]
            [PXUIField(DisplayName = "Due Date")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>))]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
            #endregion

            #region PaymentMethodID
            [PXString()]
            [PXUIField(DisplayName = "Payment Method ID", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<NMBankAccount.paymentMethodID,
                Where<NMBankAccount.bankAccountID, Equal<Current<selectBankAccountID>>>>))]
            public virtual string PaymentMethodID { get; set; }
            public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
            #endregion

            #region CashAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Cash Account ID", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<NMBankAccount.cashAccountID,
                Where<NMBankAccount.bankAccountID, Equal<Current<selectBankAccountID>>>>))]
            public virtual int? CashAccountID { get; set; }
            public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
            #endregion

            #region IsCheckIssue
            [PXBool()]
            [PXUIField(DisplayName = "Is Check Issue")]
            [PXUnboundDefault(false)]
            public virtual bool? IsCheckIssue { get; set; }
            public abstract class isCheckIssue : PX.Data.BQL.BqlBool.Field<isCheckIssue> { }
            #endregion

            #region UsrTrPaymentType
            [PXString()]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string UsrTrPaymentType { get; set; }
            public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
            #endregion

            #region UsrTrConfirmID
            [PXInt()]
            [PXUIField(DisplayName = "UsrTrConfirmID")]
            public virtual int? UsrTrConfirmID { get; set; }
            public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
            #endregion

            #region UsrTrConfirmDate
            [PXDate()]
            [PXUIField(DisplayName = "UsrTrConfirmDate")]
            public virtual DateTime? UsrTrConfirmDate { get; set; }
            public abstract class usrTrConfirmDate : IBqlField { }
            #endregion

            #region UsrTrConfirmBy
            [PXGuid()]
            [PXUIField(DisplayName = "UsrTrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? UsrTrConfirmBy { get; set; }
            public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
            #endregion

            #endregion

            #region Selected Information

            #region UnBranchID - �p��[�`��
            [PXInt()]
            [PXUnboundDefault(typeof(Current<AccessInfo.branchID>))]
            public virtual int? UnBranchID { get; set; }
            public abstract class unBranchID : PX.Data.BQL.BqlInt.Field<unBranchID> { }
            #endregion

            #region SelectedCount
            [PXInt()]
            [PXUIField(DisplayName = "Selected Count", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Int32, "0")]
            public virtual int? SelectedCount { get; set; }
            public abstract class selectedCount : PX.Data.BQL.BqlInt.Field<selectedCount> { }
            #endregion

            #region SelectedOriCuryAmount
            [PXDecimal(4)]
            [PXUIField(DisplayName = "Selected Ori Cury Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SelectedOriCuryAmount { get; set; }
            public abstract class selectedOriCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedOriCuryAmount> { }
            #endregion

            #endregion

            #region Const
            #region BankCode(like)
            /**�x�s**/
            public const string TN = "812";
            /**���**/
            public const string GT = "013";

            public class likeTN : PX.Data.BQL.BqlString.Constant<likeTN> { public likeTN() : base(TN + "%") {; } }
            public class likeGT : PX.Data.BQL.BqlString.Constant<likeGT> { public likeGT() : base(GT + "%") {; } }

            #endregion
            #endregion
        }
        #endregion

        #region NMPayableTTV
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<APInvoice,
            InnerJoin<KGBillPayment,
                On<KGBillPayment.refNbr, Equal<APInvoice.refNbr>>,
            InnerJoin<APRegister,
                On<APRegister.refNbr, Equal<APInvoice.refNbr>,
                And<APRegister.docType, Equal<APInvoice.docType>>>,
            LeftJoin<Location, On<Location.bAccountID, Equal<KGBillPayment.vendorID>,
                And<Location.locationID, Equal<KGBillPayment.vendorLocationID>>>,
            LeftJoin<NMApTeleTransLog, On<NMApTeleTransLog.billPaymentID, Equal<KGBillPayment.billPaymentID>,
                And<NMApTeleTransLog.isNew, Equal<True>>>,
             LeftJoin<APInvoiceDebit,
                 On<APInvoiceDebit.origRefNbr, Equal<APRegister.refNbr>,
                 And<APInvoiceDebit.origDocType, Equal<APRegister.docType>>>>
                >>>>,
             Where<
                    APInvoice.openDoc, Equal<True>,
                    And<APRegisterFinExt.usrIsConfirm, Equal<True>,
                    And2<Where<
                        APInvoice.docType, Equal<APDocType.invoice>,//�p��
                        Or<APInvoice.docType, Equal<APDocType.creditAdj>,//�U��վ�
                        Or<APInvoice.docType, Equal<APDocType.prepayment>>>>,//�w�I��
                    And2<Where<
                        IsNull<KGBillPaymentExt.usrIsCheckIssued, False>, Equal<False>,
                        Or<
                            Where<IsNull<KGBillPaymentExt.usrIsCheckIssued, False>, Equal<True>,
                            And<NMApTeleTransLog.status, NotEqual<NMStringList.NMApTtLogStatus.feedBackSuccess>>>>
                        >,
                    And<APInvoice.status, Equal<APDocStatus.open>,
                    And<APInvoice.curyDocBal, Greater<decimal0>,
                    And<KGBillPayment.paymentAmount, Greater<decimal0>,
                    And<KGBillPayment.paymentMethod, Equal<PaymentMethod.wireTransfer>,
                    And<APInvoiceDebit.refNbr, IsNull,
                    And<KGBillPaymentExt.usrIsTrConfirm, Equal<True>>>>//12168:KGBillPayment.UsrIsTrConfirm �n����True>>
                                                                      //And<KGBillPayment.paymentPeriod, Equal<Zero>>
                        >>>>//�����Ѽƭn����0
             >>>
            >), Persistent = false)]
        public partial class NMPayableTTV : IBqlTable
        {
            #region Selected
            [PXDBBool]
            [PXUIField(DisplayName = "Selected")]
            public virtual bool? Selected { get; set; }
            public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
            #endregion

            #region BranchID
            [PXDBInt(BqlField = typeof(APInvoice.branchID))]
            [PXUIField(DisplayName = "Branch ID")]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            public virtual int? BranchID { get; set; }
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            #endregion

            #region ProjectID
            [ProjectBase(BqlField = typeof(APInvoice.projectID))]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            [PXUIField(DisplayName = "Project ID", Required = true)]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [ProjectDefault()]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region RefNbr
            [PXDBString(15, InputMask = "", BqlField = typeof(APInvoice.refNbr))]
            [PXUIField(DisplayName = "RefN br")]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            //#region BatchNbr
            //[PXDBString(InputMask = "", BqlField = typeof(APInvoice.batchNbr))]
            //[PXUIField(DisplayName = "Batch Nbr")]
            //[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<BatchModule.moduleAP>>>))]
            //public virtual string BatchNbr { get; set; }
            //public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
            //#endregion

            #region UsrAccConfirmNbr
            [PXDBString(BqlField = typeof(APRegisterFinExt.usrAccConfirmNbr))]
            [PXSelector(typeof(Search<BatchExt.usrAccConfirmNbr, Where<Batch.module, Equal<BatchModule.moduleGL>>>))]
            [PXUIField(DisplayName = "UsrAccConfirmNbr")]

            public virtual string UsrAccConfirmNbr { get; set; }
            public abstract class usrAccConfirmNbr : PX.Data.BQL.BqlString.Field<usrAccConfirmNbr> { }
            #endregion

            #region DocType
            [PXDBString(3, IsFixed = true, BqlField = typeof(APInvoice.docType))]
            [PXDefault]
            [APMigrationModeDependentInvoiceTypeList]
            [PXUIField(DisplayName = "Doc Type")]
            [PXFieldDescription]
            public virtual string DocType { get; set; }
            public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
            #endregion

            #region PaymentDate
            [PXDBDate(BqlField = typeof(KGBillPayment.paymentDate))]
            [PXUIField(DisplayName = "Payment Date")]
            public virtual DateTime? PaymentDate { get; set; }
            public abstract class paymentDate : PX.Data.BQL.BqlDateTime.Field<paymentDate> { }
            #endregion

            #region DueDate
            [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
            [PXUIField(DisplayName = "Due Date")]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
            #endregion

            #region VendorID
            [PXDBInt(BqlField = typeof(KGBillPayment.vendorID))]
            [PXUIField(DisplayName = "Vendor ID", Required = true)]
            [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>, Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>),
                typeof(BAccount2.acctCD),
                typeof(BAccount2.acctName),
                typeof(BAccount2.status),
                typeof(BAccount2.defAddressID),
                typeof(BAccount2.defContactID),
                typeof(BAccount2.defLocationID),
            SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

            #region VendorLocationID
            [PXDBInt(BqlField = typeof(KGBillPayment.vendorLocationID))]
            [PXUIField(DisplayName = "Vendor Location ID")]
            [PXSelectorWithCustomOrderBy(typeof(Search<Location.locationID,
                Where<Location.bAccountID, Equal<Current<vendorID>>,
                    And<Location.vPaymentMethodID, Equal<word.tt>>>,
                OrderBy<
                    Desc<LocationFinExt.usrIsReserveAcct,
                    Asc<Location.locationCD>>>>),
                typeof(LocationFinExt.usrIsReserveAcct),
                typeof(Location.locationCD),
                typeof(Location.descr),
                typeof(Location.paymentMethodID),
                typeof(Location.cashAccountID),
                SubstituteKey = typeof(Location.locationCD))]
            public virtual int? VendorLocationID { get; set; }
            public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
            #endregion

            #region ProjectPeriod
            [PXDBInt(BqlField = typeof(APRegisterExt.usrValuationPhase))]
            [PXUIField(DisplayName = "Project Period")]
            public virtual int? ProjectPeriod { get; set; }
            public abstract class projectPeriod : PX.Data.BQL.BqlInt.Field<projectPeriod> { }
            #endregion

            #region CuryID
            [PXDBString(IsUnicode = true, BqlField = typeof(APInvoice.curyID))]
            [PXUIField(DisplayName = "CuryID", Required = true)]
            [PXSelector(typeof(Currency.curyID))]
            public virtual string CuryID { get; set; }
            public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
            #endregion

            #region CuryInfoID
            [PXDBLong(BqlField = typeof(APInvoice.curyInfoID))]
            [CurrencyInfo()]
            public virtual Int64? CuryInfoID { get; set; }
            public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
            protected Int64? _CuryInfoID;
            #endregion

            #region OriCuryAmount
            [PXDBCurrency(typeof(curyInfoID), typeof(baseCuryAmount), BqlField = typeof(KGBillPayment.actPayAmt))]
            [PXUIField(DisplayName = "Ori Cury Amount", Required = true)]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual Decimal? OriCuryAmount { get; set; }
            public abstract class oriCuryAmount : PX.Data.BQL.BqlDecimal.Field<oriCuryAmount> { }
            #endregion

            #region BaseCuryAmount
            [PXBaseCury()]
            [PXUIField(DisplayName = "Base Cury Amount", IsReadOnly = true)]
            public virtual Decimal? BaseCuryAmount { get; set; }
            public abstract class baseCuryAmount : PX.Data.BQL.BqlDecimal.Field<baseCuryAmount> { }
            #endregion

            #region PaymentMethodID
            [PXDBString(BqlField = typeof(Location.vPaymentMethodID))]
            [PXUIField(DisplayName = "Payment Method ID", IsReadOnly = true)]
            [PXSelector(typeof(Search<PaymentMethodAccount.paymentMethodID>),
                typeof(PaymentMethodAccount.paymentMethodID))]
            [PXDefault(typeof(Search<Location.vPaymentMethodID,
                Where<Location.locationID, Equal<Current<vendorLocationID>>,
                    And<Location.bAccountID, Equal<Current<vendorID>>>>>))]
            public virtual string PaymentMethodID { get; set; }
            public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
            #endregion

            #region BillPaymentID
            [PXDBInt(BqlField = typeof(KGBillPayment.billPaymentID), IsKey = true)]
            public virtual int? BillPaymentID { get; set; }
            public abstract class billPaymentID : PX.Data.BQL.BqlInt.Field<billPaymentID> { }
            #endregion

            #region CashAccountID
            [PXDBInt(BqlField = typeof(Location.cashAccountID))]
            [PXUIField(DisplayName = "Cash Account ID", IsReadOnly = true)]
            [PXDefault(typeof(Search<Location.cashAccountID,
                Where<Location.locationID, Equal<Current<vendorLocationID>>,
                    And<Location.bAccountID, Equal<Current<vendorID>>>>>))]
            [PXSelector(typeof(Search<CashAccount.cashAccountID, Where<CashAccount.active, Equal<True>>>),
                        typeof(CashAccount.cashAccountCD),
                        typeof(CashAccount.descr),
                        typeof(CashAccount.accountID),
                        typeof(CashAccount.subID),
                        SubstituteKey = typeof(CashAccount.cashAccountCD),
                        DescriptionField = typeof(CashAccount.descr)
                       )]
            public virtual int? CashAccountID { get; set; }
            public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
            #endregion

            #region IsCheckIssue
            [PXDBBool(BqlField = typeof(KGBillPaymentExt.usrIsCheckIssued))]
            [PXUIField(DisplayName = "Is Check Issue")]
            public virtual bool? IsCheckIssue { get; set; }
            public abstract class isCheckIssue : PX.Data.BQL.BqlBool.Field<isCheckIssue> { }
            #endregion

            #region NMBatchNbr
            [PXDBString(IsFixed = true, IsUnicode = true, InputMask = "", BqlField = typeof(KGBillPaymentExt.usrNMBatchNbr))]
            [PXUIField(DisplayName = "NM Batch Nbr", IsReadOnly = true)]
            public virtual string NMBatchNbr { get; set; }
            public abstract class nmBatchNbr : PX.Data.BQL.BqlString.Field<nmBatchNbr> { }
            #endregion

            #region APVendorID
            [PXDBInt(BqlField = typeof(APInvoice.vendorID))]
            [PXUIField(DisplayName = "AP Vendor ID")]
            [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>, Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>),
                typeof(BAccount2.acctCD),
                typeof(BAccount2.acctName),
                typeof(BAccount2.status),
                typeof(BAccount2.defAddressID),
                typeof(BAccount2.defContactID),
                typeof(BAccount2.defLocationID),
            SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
            public virtual int? APVendorID { get; set; }
            public abstract class apVendorID : PX.Data.BQL.BqlInt.Field<apVendorID> { }
            #endregion

            #region APVendorLocationID
            [PXDBInt(BqlField = typeof(APInvoice.vendorLocationID))]
            [PXUIField(DisplayName = "AP Vendor Location ID")]
            [PXSelector(typeof(Search<Location.locationID>),
                typeof(Location.locationCD),
                typeof(Location.descr),
                typeof(Location.paymentMethodID),
                typeof(Location.cashAccountID),
                SubstituteKey = typeof(Location.locationCD))]
            public virtual int? APVendorLocationID { get; set; }
            public abstract class apVendorLocationID : PX.Data.BQL.BqlInt.Field<apVendorLocationID> { }
            #endregion

            #region UsrTrPaymentType
            [PXDBString(BqlField = typeof(KGBillPaymentExt.usrTrPaymentType))]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string UsrTrPaymentType { get; set; }
            public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
            #endregion

            #region UsrTrConfirmID
            [PXDBInt(BqlField = typeof(KGBillPaymentExt.usrTrConfirmID))]
            [PXUIField(DisplayName = "UsrTrConfirmID")]
            public virtual int? UsrTrConfirmID { get; set; }
            public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
            #endregion

            #region UsrTrConfirmDate
            [PXDBDate(BqlField = typeof(KGBillPaymentExt.usrTrConfirmDate))]
            [PXUIField(DisplayName = "UsrTrConfirmDate")]
            public virtual DateTime? UsrTrConfirmDate { get; set; }
            public abstract class usrTrConfirmDate : IBqlField { }
            #endregion

            #region UsrTrConfirmBy
            [PXDBGuid(BqlField = typeof(KGBillPaymentExt.usrTrConfirmBy))]
            [PXUIField(DisplayName = "UsrTrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? UsrTrConfirmBy { get; set; }
            public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
            #endregion


            #region �p��[�`��
            #region UnBranchID
            [PXDBInt(BqlField = typeof(APInvoice.branchID))]
            [PXUnboundDefault(typeof(Current<AccessInfo.branchID>))]
            [PXParent(typeof(Select<ParamTable,
                            Where<ParamTable.unBranchID,
                            Equal<Current<unBranchID>>>>))]
            public virtual int? UnBranchID { get; set; }
            public abstract class unBranchID : PX.Data.BQL.BqlInt.Field<unBranchID> { }
            #endregion

            #region UnOriCuryAmount
            [PXDecimal]
            [PXFormula(typeof(Switch<
                Case<Where<selected, Equal<True>>, IsNull<oriCuryAmount, decimal0>,
                Case<Where<selected, Equal<False>>, decimal0>>
                >), typeof(SumCalc<ParamTable.selectedOriCuryAmount>))]
            public virtual Decimal? UnOriCuryAmount { get; set; }
            public abstract class unOriCuryAmount : PX.Data.BQL.BqlDecimal.Field<unOriCuryAmount> { }
            #endregion

            #region UnCount
            [PXInt]
            [PXFormula(typeof(Switch<
                Case<Where<selected, Equal<True>>, int1,
                Case<Where<selected, Equal<False>>, int0>>
                >), typeof(SumCalc<ParamTable.selectedCount>))]
            public virtual int? UnCount { get; set; }
            public abstract class unCount : PX.Data.BQL.BqlInt.Field<unCount> { }
            #endregion

            #endregion

            #region unbound
            #region BankAccountID
            [PXDBInt(BqlField = typeof(KGBillPayment.bankAccountID))]
            [PXUIField(DisplayName = "Bank Account ID", Required = true)]
            [PXSelector(typeof(Search<NMBankAccount.bankAccountID,
                Where<NMBankAccount.isSettlement, Equal<False>,
                    And<NMBankAccount.cashAccountID, Equal<Current<cashAccountID>>,
                        And<NMBankAccount.paymentMethodID, Equal<Current<paymentMethodID>>>>
                    >>),
                typeof(NMBankAccount.bankAccountCD),
                typeof(NMBankAccount.bankName),
                typeof(NMBankAccount.bankShortName),
                typeof(NMBankAccount.bankCode),
                typeof(NMBankAccount.bankAccount),
                SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                DescriptionField = typeof(NMBankAccount.bankName))]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

            #region AcctName
            [PXString()]
            [PXUIField(DisplayName = "Acct Name", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_acctName>>>>>>))]
            public virtual string AcctName { get; set; }
            public abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }
            #endregion

            #region BankAcct
            [PXString()]
            [PXUIField(DisplayName = "Bank Acct", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_bankAcct>>>>>>))]
            public virtual string BankAcct { get; set; }
            public abstract class bankAcct : PX.Data.BQL.BqlString.Field<bankAcct> { }
            #endregion

            #region BankName
            [PXString()]
            [PXUIField(DisplayName = "Bank Name", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_bankName>>>>>>))]
            public virtual string BankName { get; set; }
            public abstract class bankName : PX.Data.BQL.BqlString.Field<bankName> { }
            #endregion

            #region BankNbr
            [PXString()]
            [PXUIField(DisplayName = "Bank Nbr", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_bankNbr>>>>>>))]
            public virtual string BankNbr { get; set; }
            public abstract class bankNbr : PX.Data.BQL.BqlString.Field<bankNbr> { }
            #endregion

            #region BranchNam
            [PXString()]
            [PXUIField(DisplayName = "Branch Nam", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_branchNam>>>>>>))]
            public virtual string BranchNam { get; set; }
            public abstract class branchNam : PX.Data.BQL.BqlString.Field<branchNam> { }
            #endregion

            #region BranchNbr
            [PXString()]
            [PXUIField(DisplayName = "Branch Nbr", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_branchNbr>>>>>>))]
            public virtual string BranchNbr { get; set; }
            public abstract class branchNbr : PX.Data.BQL.BqlString.Field<branchNbr> { }
            #endregion

            #region Charge
            [PXString()]
            [PXUIField(DisplayName = "Charge", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_charge>>>>>>))]
            public virtual string Charge { get; set; }
            public abstract class charge : PX.Data.BQL.BqlString.Field<charge> { }
            #endregion

            #region Category
            [PXString()]
            [PXUIField(DisplayName = "Category", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_category>>>>>>))]
            public virtual string Category { get; set; }
            public abstract class category : PX.Data.BQL.BqlString.Field<category> { }
            #endregion

            #region CategoryID
            [PXString()]
            [PXUIField(DisplayName = "CategoryID", IsReadOnly = true)]
            [PXUnboundDefault(typeof(Search<VendorPaymentMethodDetail.detailValue,
                Where<VendorPaymentMethodDetail.bAccountID, Equal<Current<vendorID>>,
                    And<VendorPaymentMethodDetail.locationID, Equal<Current<vendorLocationID>>,
                        And<VendorPaymentMethodDetail.paymentMethodID, Equal<tt>,
                            And<VendorPaymentMethodDetail.detailID, Equal<_categoryID>>>>>>))]
            public virtual string CategoryID { get; set; }
            public abstract class categoryID : PX.Data.BQL.BqlString.Field<categoryID> { }
            #endregion

            #region LogID
            [PXInt()]
            public virtual int? LogID { get; set; }
            public abstract class logID : PX.Data.BQL.BqlInt.Field<logID> { }
            #endregion


            #endregion

            #region Const
            #region PaymentMethodDetail
            public const string ACCT_NAME = "ACCT NAME";
            public const string BANK_ACCT = "BANK ACCT";
            public const string BANK_NAME = "BANK NAME";
            public const string BANK_NBR = "BANK NBR";
            public const string BRANCH_NAM = "BRANCH NAM";
            public const string BRANCH_NBR = "BRANCH NBR";
            public const string CATEGORY = "CATEGORY";
            public const string CATEGORYID = "CATEGORYID";
            public const string CHARGE = "CHARGE";
            public const string TT = "TT";
            public class tt : PX.Data.BQL.BqlString.Constant<tt> { public tt() : base(TT) {; } }

            public class _acctName : PX.Data.BQL.BqlString.Constant<_acctName> { public _acctName() : base(ACCT_NAME) {; } }
            public class _bankAcct : PX.Data.BQL.BqlString.Constant<_bankAcct> { public _bankAcct() : base(BANK_ACCT) {; } }
            public class _bankName : PX.Data.BQL.BqlString.Constant<_bankName> { public _bankName() : base(BANK_NAME) {; } }
            public class _bankNbr : PX.Data.BQL.BqlString.Constant<_bankNbr> { public _bankNbr() : base(BANK_NBR) {; } }
            public class _branchNam : PX.Data.BQL.BqlString.Constant<_branchNam> { public _branchNam() : base(BRANCH_NAM) {; } }
            public class _branchNbr : PX.Data.BQL.BqlString.Constant<_branchNbr> { public _branchNbr() : base(BRANCH_NBR) {; } }
            public class _category : PX.Data.BQL.BqlString.Constant<_category> { public _category() : base(CATEGORY) {; } }
            public class _categoryID : PX.Data.BQL.BqlString.Constant<_categoryID> { public _categoryID() : base(CATEGORYID) {; } }
            public class _charge : PX.Data.BQL.BqlString.Constant<_charge> { public _charge() : base(CHARGE) {; } }
            #endregion

            #endregion

        }
        #endregion

        #region APInvoiceDebit
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select<APInvoice,
            Where<APInvoice.docType, Equal<APDocType.debitAdj>,
                And<APRegisterExt.usrIsDeductionDoc, NotEqual<True>>>
            >), Persistent = false)]
        public partial class APInvoiceDebit : IBqlTable
        {

            #region RefNbr
            [PXDBString(15, InputMask = "", BqlField = typeof(APInvoice.refNbr))]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region DocType
            [PXDBString(3, IsFixed = true, BqlField = typeof(APInvoice.docType))]
            public virtual string DocType { get; set; }
            public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
            #endregion

            #region OrigRefNbr
            [PXDBString(15, InputMask = "", BqlField = typeof(APInvoice.origRefNbr))]
            public virtual string OrigRefNbr { get; set; }
            public abstract class origRefNbr : PX.Data.BQL.BqlString.Field<origRefNbr> { }
            #endregion

            #region OrigDocType
            [PXDBString(3, IsFixed = true, BqlField = typeof(APInvoice.origDocType))]
            public virtual string OrigDocType { get; set; }
            public abstract class origDocType : PX.Data.BQL.BqlString.Field<origDocType> { }
            #endregion
        }
        #endregion
        #endregion
    }
}