using System;
using PX.Data;
using NM.Util;
using System.Collections;
using PX.Common;
using System.IO;
using System.Collections.Generic;
using NM.DAC;
using static NM.Util.NMStringList;
using PX.Objects.AP;
using PX.Objects.CR;
using Kedge.DAC;
using KG.Util;
using PX.Objects.GL;

namespace NM
{
    /**
     * =======2021-02-24 : 0011864 =====Alton
     * �]����N�X��ܬ����妭������Ʈɻ~�P�A���ରStringList�B�z
     * 
     *  * ===2021-03-02:11864=====Alton
     * 2.�^�X���ѮɡA���ݭn�NIsCheckIssue�אּFalse
     * 
     * ====2021-07-08:12136====Alton
     * TT APPayment�L�b�ɲ��� APPayment��GL�ǲ� & �l��O�ǲ�
     * **/
    public class NMApBankFeedbackProcess : PXGraph<NMApBankFeedbackProcess>
    {
        const String ENCODING = "BIG5";

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXFilter<ParamTable> UploadDialog;
        public PXFilter<DetailsTable> DetailsView;
        public PXSelect<NMApTeleTransLog> Logs;
        public PXSelect<NMPayableCheck> Checks;
        protected virtual IEnumerable detailsView()
        {
            return MasterView.Current.TempData ?? new List<DetailsTable>();
        }
        #endregion

        #region Actions

        #region Ū���^�X��
        public PXAction<ParamTable> FileUpload;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Ū���^�X��")]
        protected IEnumerable fileUpload(PXAdapter adapter)
        {
            ParamTable p = MasterView.Current;
            String sessionKey = "FeedbackSessionKey";
            if (p.FeedbackType == null) throw new Exception("�п�ܦ^�X����");


            if (UploadDialog.AskExt() == WebDialogResult.OK)
            {
                //���o�W���ɮ׸�T
                PX.SM.FileInfo info = PXContext.SessionTyped<PXSessionStatePXData>().FileInfo[sessionKey] as PX.SM.FileInfo;
                if (info == null) throw new Exception("�п�ܤW���ɮ�");
                Byte[] bytes = info.BinData;
                MemoryStream memoryStream = new MemoryStream(bytes);
                StreamReader sr = new StreamReader(memoryStream, System.Text.Encoding.GetEncoding(ENCODING));
                p.NowFeedbackType = p.FeedbackType;
                if (p.FeedbackType == NMApFeedbackType.TN_BANK_CHECK)
                {
                    ReadTnBankCheck(sr);
                }
                else if (p.FeedbackType == NMApFeedbackType.TN_TT)
                {
                    ReadTnTT(sr);
                }
                else if (p.FeedbackType == NMApFeedbackType.GT_TT)
                {
                    ReadGtTT(sr);
                }

                //�M���ɮ׸�T
                sr.Close();
                System.Web.HttpContext.Current.Session.Remove(sessionKey);

            }
            return adapter.Get();
        }
        #endregion

        #region ����
        public PXAction<ParamTable> Submit;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "����")]
        protected IEnumerable submit(PXAdapter adapter)
        {
            ParamTable p = MasterView.Current;

            List<DetailsTable> errorData = new List<DetailsTable>();

            ///<remarks> To avoid database postback errors, the scope rolls back all operations in the async thread. </remarks>
            //using (PXTransactionScope ts = new PXTransactionScope())
            //{
            ///<remarks> 
            /// Use Async to avoid the Release feature from not working properly, as that trigger time falls on custom GL voucher creation and releasing, and there will always be an unexpected "null reference" issue.
            ///</remarks>
            PXLongOperation.StartOperation(this, delegate ()
            {
                foreach (DetailsTable item in DetailsView.Select())
                {
                    using (PXTransactionScope ts2 = new PXTransactionScope())
                    {
                        try
                        {
                            if (p.NowFeedbackType == NMApFeedbackType.TN_BANK_CHECK)
                            {
                                NMPayableCheck checkItem = GetPayableCheck(item.UploadKey);

                                if (checkItem == null)
                                {
                                    item.RowError = "�L�N�}����";
                                    errorData.Add(item);
                                }
                                else
                                {
                                    SubmitBankCheck(item, checkItem);
                                }
                            }
                            else if (p.NowFeedbackType == NMApFeedbackType.TN_TT || p.NowFeedbackType == NMApFeedbackType.GT_TT)
                            {
                                //�ˬd�O�_�s�b������log
                                if (item.OldLog == null)
                                {
                                    item.RowError = "�������w�����A�䤣���������";
                                    errorData.Add(item);
                                }
                                else
                                {
                                    SubmitTT(item, p.NowFeedbackType);
                                }
                            }
                            ts2.Complete();
                        }
                        catch (Exception e)
                        {
                            item.RowError = e.Message;
                            if (!errorData.Contains(item))
                            {
                                errorData.Add(item);
                            }
                            //throw;
                        }
                    }
                }

                //���o�M���e��FeedbackType
                MasterView.Current.FeedbackType = MasterView.Current.FeedbackType;
                MasterView.Current.TempData = errorData;
            });

            //    ts.Complete();
            //}

            return adapter.Get();
        }
        #endregion

        #endregion

        #region Event
        private void _(Events.RowSelected<ParamTable> e)
        {
            ParamTable row = e.Row;
            if (row == null) return;
            SetVisabled(row.FeedbackType);
            Submit.SetEnabled(row.NowFeedbackType != null);

            ///<remarks> 
            /// Mantis [0012329]
            /// Because the use of asynchronous thread bypasses unforeseen problems, but does not raise exceptions handling to display error message.
            ///</remarks>
            if (row.TempData != null)
            {
                foreach (DetailsTable errorRow in this.MasterView.Current.TempData)
                {
                    if (string.IsNullOrEmpty(errorRow.RowError)) { continue; }

                    DetailsView.Cache.RaiseExceptionHandling<DetailsTable.uploadKey>(errorRow, errorRow.UploadKey, new PXSetPropertyException(errorRow.RowError, PXErrorLevel.RowError));
                }
            }
        }
        #endregion

        #region Methods
        public virtual void SubmitBankCheck(DetailsTable item, NMPayableCheck checkItem)
        {
            APPaymentEntry apEntry = null;
            APPayment appayment = null;
            if (!String.IsNullOrEmpty(item.CheckNbr) && item.TnBankCheckStatus == "S")
            {
                checkItem.CheckNbr = item.CheckNbr;
                if (!String.IsNullOrEmpty(checkItem.PaymentDocNo))
                {
                    PXUpdate<
                        Set<APPayment.extRefNbr, Required<APPayment.extRefNbr>>,
                    APPayment,
                    Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>
                    .Update(this, item.CheckNbr, checkItem.PaymentDocNo);
                }
                else if (checkItem.Status == NMAPCheckStatus.REPRESENT)
                {
                    apEntry = PXGraph.CreateInstance<APPaymentEntry>();
                    //����APPayment�B�L�b
                    appayment = NMApCheckEntry.CreateApPayment(apEntry, ref checkItem, false);
                    checkItem.Status = NMAPCheckStatus.CONFIRM;
                    checkItem.ConfirmBatchNbr = NMVoucherUtil.CreateAPVoucher(NMAPVoucher.CONFIRM, checkItem, GLStageCode.NMPConfirmP1);
                }
            }
            checkItem.FeedbackStatus = item.TnBankCheckStatus;
            Checks.Update(checkItem);
            this.Persist();
            #region APPayment�L�b
            if (apEntry != null && appayment != null)
            {
                apEntry.Document.SetValueExt<APPayment.hold>(appayment, false);
                appayment = apEntry.Document.Update(appayment);
                apEntry.Save.Press();
                apEntry.release.Press();
            }
            #endregion
        }

        public virtual void SubmitTT(DetailsTable item, String feedbackType)
        {
            //��sOldLog isNew = False
            PXUpdate<Set<NMApTeleTransLog.isNew, False>,
               NMApTeleTransLog,
               Where<NMApTeleTransLog.billPaymentID, Equal<Required<NMApTeleTransLog.billPaymentID>>,
               And<NMApTeleTransLog.isNew, Equal<True>>>>
               .Update(this, item.OldLog.BillPaymentID);

            NMApTeleTransLog log = new NMApTeleTransLog();
            log.BillPaymentID = item.OldLog.BillPaymentID;
            log.TeleTransRefNbr = item.OldLog.TeleTransRefNbr;
            log.Status = NMApTtLogStatus.FEED_BACK;
            log.IsNew = true;
            if (feedbackType == NMApFeedbackType.TN_TT)
            {
                log.Type = NMApTtLogType.TN;
                //20220311 add by louis �s�W�g�J�ϥΪ̦ۭq�Ǹ�
                log.CustNbr = item.CustNbr;
                log.ReturnMsg = new NMApTnTtStatus().ValueLabelDic[item.TnTtStatus];
                if (item.TnTtStatus == NMApTnTtStatus._08)
                {
                    log.Status = NMApTtLogStatus.FEED_BACK_SUCCESS;
                }
            }
            else if (feedbackType == NMApFeedbackType.GT_TT)
            {
                log.Type = NMApTtLogType.GT;
                String GtTtStatus = new NMApGtTtStatus().ValueLabelDic[item.GtTtStatus];
                String errorMsg = new NMApGtTtErrorCode().ValueLabelDic[item.ErrorMsg];
                String reason = new NMApGtTtReasonCode().ValueLabelDic[item.Reason];
                log.ReturnMsg =
                    item.GtTtStatus + "(" + errorMsg + ")"
                    + (String.IsNullOrEmpty(reason) ? "" : ("�A" + reason));
                if (item.GtTtStatus == NMApGtTtStatus._1)
                {
                    log.Status = NMApTtLogStatus.FEED_BACK_SUCCESS;
                }
            }

            if (log.Status == NMApTtLogStatus.FEED_BACK_SUCCESS)
            {
                //2022-10-17 Alton add ��susrCreatePaymentType =2 (TT)
                PXUpdate<
                    Set<KGBillPayment.ttActDate, Required<KGBillPayment.ttActDate>,
                    Set<KGBillPaymentExt.usrCreatePaymentType, Required<KGBillPaymentExt.usrCreatePaymentType>>>,
                KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Update(this, item.Date, NMApCreatePaymentType.TT, item.OldLog.BillPaymentID);

                //�p�G�^�X���\�h�h����APPayment
                //2022-10-17 Alton ��� NMApPaymentProcess�Ƶ{����
                //CreateAPPaymentByTT(item.OldLog.BillPaymentID, item.Date,int.Parse(item.UploadKey));
                #region ����APPayment �ǲ� & �l��ǲ�
                KGBillPayment billPayment = GetBillPayment(item.OldLog.BillPaymentID);
                string t1BatchNbr = NMVoucherUtil.CreateTTVoucher(NMTTVoucher.APPAYMENT, billPayment, item.Date, GLStageCode.NMPTtFeedbackT1);
                //[Jira KED-19] 2022-12-16 Alton : ��������T2�ǲ�
                //NMVoucherUtil.CreateTTVoucher(NMTTVoucher.WRITEOFFPOSTAGE, billPayment, item.Date, GLStageCode.NMPTtFeedbackT2, t1BatchNbr);
                #endregion
                //�]�w����A�M����l����
                foreach (NMApTeleTransLog oldLog in GetLogByBillPaymentID(log.BillPaymentID))
                {
                    Logs.Delete(oldLog);
                }
            }
            //else
            //{
            //�_�h��sKGBillPayment.UsrIsCheckIssue = false ���ϥΪ̥i�H�ର�䲼
            //2021-03-02:11864 Mark By Alton�G������sIsCheckIssue = False�A�]���w�g�i�H�b�����A�U�u��}���v
            //PXUpdate<
            //        Set<KGBillPaymentExt.usrIsCheckIssued, False>,
            //        KGBillPayment,
            //        Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>
            //    >.Update(this, log.BillPaymentID);
            //}

            Logs.Insert(log);

            this.Persist();
        }
        /// <summary>
        /// �ѪR�x�s�N�}�^�X��
        /// </summary>
        public virtual void ReadTnBankCheck(StreamReader sr)
        {
            String txt;
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            PXCache cache = DetailsView.Cache;
            List<DetailsTable> tempData = new List<DetailsTable>();
            while ((txt = sr.ReadLine()) != null)
            {
                DetailsTable item = new DetailsTable();
                //�Τ�ۭq�Ǹ�1,7
                item.UploadKey = GetString(ref txt, 7);
                //�o�����8,8
                String date = GetString(ref txt, 8);
                item.Date = DateTime.ParseExact(date, "yyyyMMdd", provider);
                //�䲼���X16,9
                item.CheckNbr = GetString(ref txt, 9);
                //���A25,1
                item.TnBankCheckStatus = GetString(ref txt, 1);
                //�o���H�b��26,17
                GetString(ref txt, 17);
                //���B43,18
                String amount = GetString(ref txt, 18);
                item.Amount = GetDecimal(amount);
                //���ڤH61,60
                item.Receiver = GetString(ref txt, 60);
                //121�H�U�ٲ�
                tempData.Add(item);
            }
            MasterView.Current.TempData = tempData;
        }
        /// <summary>
        /// �ѪR�x�s�q�צ^�X��
        /// </summary>
        public virtual void ReadTnTT(StreamReader sr)
        {
            String txt;
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            List<DetailsTable> tempData = new List<DetailsTable>();
            while ((txt = sr.ReadLine()) != null)
            {
                DetailsTable item = new DetailsTable();
                //�Τ�ۭq�Ǹ�1,7
                //20220310 modify by louis �s�W��ܥΤ�ۭq�Ǹ�
                //GetString(ref txt, 7);
                item.CustNbr = GetString(ref txt, 7);
                //�I�ڤ��8,8
                String date = GetString(ref txt, 8);
                item.Date = DateTime.ParseExact(date, "yyyyMMdd", provider);
                //�I�ڪ��B16,18
                String amount = GetString(ref txt, 18);
                item.Amount = GetDecimal(amount);
                //�I�ڱb��34,17
                GetString(ref txt, 17);
                //�I�ڤ�W51,60
                GetString(ref txt, 60);
                //���ڱb��111,17
                item.RecAccount = GetString(ref txt, 17);
                //���ڤ�W128,60
                item.Receiver = GetString(ref txt, 60);
                //�I���`��188,3
                GetString(ref txt, 3);
                //�I�ڤ���191,4
                GetString(ref txt, 4);
                //�����`��195,3
                GetString(ref txt, 3);
                //���ڤ���198,4
                GetString(ref txt, 4);
                //����202,100
                GetString(ref txt, 100);
                //���ڤH�ѧO�X302,17
                GetString(ref txt, 17);
                //���ڤH�N�X�ѧO319,3
                GetString(ref txt, 3);
                //�I�ڤH�ѧO�X17
                GetString(ref txt, 17);
                //�I�ڤH�N�X�ѧO3
                GetString(ref txt, 3);
                //����O�t��O3
                GetString(ref txt, 3);
                //��b��key��30
                item.UploadKey = GetString(ref txt, 30);
                //�I���p���H35
                GetString(ref txt, 35);
                //�I�ڳs���q��25
                GetString(ref txt, 25);
                //�I�ڶǯu���X25
                GetString(ref txt, 25);
                //�����p���H35
                GetString(ref txt, 35);
                //���ڳs���q��25
                GetString(ref txt, 25);
                //���ڶǯu���X25
                GetString(ref txt, 25);
                //���ڳq���H�c50
                GetString(ref txt, 50);
                //��ƪ��A595,2
                item.TnTtStatus = GetString(ref txt, 2);
                //item.TnTtStatus =
                //    status == "01" ? "�s��A��Ʃ|���e�f" :
                //    status == "02" ? "�e�f�A��Ƥw�e�f�A�|���f��" :
                //    status == "03" ? "�ݼf�֡A��Ƥw�f�֡A���������Ҧ����f��" :
                //    status == "04" ? "�ݩ��A��Ƥw�����Ҧ����f�֡A���ݩ��" :
                //    status == "05" ? "�h��A��ƳQ�f�֤H���άO���H���h��" :
                //    status == "06" ? "���ݦ^���A��Ƥw���AFEDI Server�|���B�z" :
                //    status == "07" ? "�W�ǥD���A��Ƥw���AFEDI Server�w�糹�����A���ݫ���b�ȳB�z" :
                //    status == "08" ? "���b���\" :
                //    status == "09" ? "�I�ڥ���" :
                //    status == "10" ? "�h�b�q���A��Ʀ��b���\�A���J�b�ɳQ�J�b��h�^" :
                //    status == "11" ? "�w�������A��ƭ�w�w���A�Q�Τ�����I��" :
                //    status == "14" ? "�h�b�q��(�H�u�^�R)�A�]�g���q�q���A�i��J�u�h�b" : ""
                //    ;

                //���olog
                item.OldLog = GetOldLog(item.UploadKey);
                item.RefNbr = item.OldLog?.TeleTransRefNbr;

                tempData.Add(item);
            }
            MasterView.Current.TempData = tempData;
        }
        /// <summary>
        /// �ѪR����q�צ^�X��
        /// </summary>
        public virtual void ReadGtTT(StreamReader sr)
        {
            String txt;
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            List<DetailsTable> tempData = new List<DetailsTable>();
            while ((txt = sr.ReadLine()) != null)
            {
                DetailsTable item = new DetailsTable();
                //�ѧO�N�X,1
                GetString(ref txt, 1);
                //�ɮפW�Ǥ��,8
                GetString(ref txt, 8);
                //�w�w������,8
                GetString(ref txt, 8);
                //������O,3
                GetString(ref txt, 3);
                //����s��,10
                item.UploadKey = GetString(ref txt, 10);
                //�I�ڦ�N�X,7
                GetString(ref txt, 7);
                //�I�ڤH�b��,16
                GetString(ref txt, 16);
                //�I�ڤH�νs,10
                GetString(ref txt, 10);
                //�I�ڤH��W,70
                GetString(ref txt, 70);
                //���O,3
                GetString(ref txt, 3);
                //���B���t��,1
                GetString(ref txt, 1);
                //���B,14
                GetString(ref txt, 14);
                //���ڦ�N�X,7
                item.RecBankCode = GetString(ref txt, 7);
                //���ڤH�b��,16?
                item.RecAccount = GetString(ref txt, 16);
                //���ڤH�νs,10
                GetString(ref txt, 10);
                //���ڤH��W,70
                item.Receiver = GetString(ref txt, 70);
                //���ڤH�O�_�q�i,1
                GetString(ref txt, 1);
                //�q�i�]�Ƹ��X,50
                GetString(ref txt, 50);
                //����O���u�覡,2
                GetString(ref txt, 2);
                //�o������,4
                GetString(ref txt, 4);
                //�Ƶ�,50
                GetString(ref txt, 50);
                //������A,3
                item.GtTtStatus = GetString(ref txt, 3);
                //���~�N�X,4
                item.ErrorMsg = GetString(ref txt, 4);
                //item.ErrorMsg =
                //    errorCode == "0000" ? "0000:���\" :
                //    errorCode == "M323" ? "M323:�h�^" :
                //    errorCode == "M911" ? "M911:�s�ڤ���" : "";
                //�]���h�ײz��,1
                item.Reason = GetString(ref txt, 1);
                //item.Reason =
                //    reasonCode == "3" ? "�b�����~" :
                //    reasonCode == "4" ? "�m�W����" :
                //    reasonCode == "9" ? "����" : "";
                //�]���״ڧǸ�,7
                item.RemittanceNbr = GetString(ref txt, 7);
                //��ڥ�����,8
                String date = GetString(ref txt, 8);
                item.TranDate = DateTime.ParseExact(date, "yyyyMMdd", provider);
                //��ڥ���ɶ�,6
                String time = GetString(ref txt, 6);
                DateTime dt = DateTime.ParseExact(time, "HHmmss", provider);
                item.TranTime = dt.ToString("HH:mm:ss");
                //�״ڪ��B,14
                String amount = GetString(ref txt, 14);
                item.Amount = GetDecimal(amount);
                //�״ڤ���O,6
                String transFee = GetString(ref txt, 6);
                item.TransFee = GetDecimal(transFee);

                //���olog
                item.OldLog = GetOldLog(item.UploadKey);
                item.RefNbr = item.OldLog?.TeleTransRefNbr;

                tempData.Add(item);
            }
            MasterView.Current.TempData = tempData;
        }

        private NMApTeleTransLog GetOldLog(String uploadKey)
        {
            try
            {
                //2021-01-20 Edit By Alton:11881 uploadKey�אּlogID
                int? logID = int.Parse(uploadKey);
                return GetLogByID(logID);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private void SetVisabled(String feedbackType)
        {
            bool isTnankCheck = feedbackType == NMApFeedbackType.TN_BANK_CHECK;
            bool isTnTT = feedbackType == NMApFeedbackType.TN_TT;
            bool isGtTT = feedbackType == NMApFeedbackType.GT_TT;
            PXCache cache = DetailsView.Cache;
            // 20220310 modify by louis �s�W��ܥΤ�ۭq�Ǹ�
            PXUIFieldAttribute.SetVisible<DetailsTable.custNbr>(cache, null, isTnTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.refNbr>(cache, null, isTnTT || isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.uploadKey>(cache, null, true);
            PXUIFieldAttribute.SetVisible<DetailsTable.date>(cache, null, isTnankCheck || isTnTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.checkNbr>(cache, null, isTnankCheck);
            PXUIFieldAttribute.SetVisible<DetailsTable.tnBankCheckStatus>(cache, null, isTnankCheck);
            PXUIFieldAttribute.SetVisible<DetailsTable.tnTtStatus>(cache, null, isTnTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.gtTtStatus>(cache, null, isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.amount>(cache, null, true);
            PXUIFieldAttribute.SetVisible<DetailsTable.recBankCode>(cache, null, isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.recAccount>(cache, null, isTnTT || isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.receiver>(cache, null, true);
            PXUIFieldAttribute.SetVisible<DetailsTable.errorMsg>(cache, null, isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.reason>(cache, null, isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.tranDate>(cache, null, isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.tranTime>(cache, null, isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.remittanceNbr>(cache, null, isGtTT);
            PXUIFieldAttribute.SetVisible<DetailsTable.transFee>(cache, null, isGtTT);
        }

        //2022-10-17 Alton �q�ײ���APPayment ���NMApPaymentProcess�Ƶ{����
        //public virtual void CreateAPPaymentByTT(int? billPaymentID, DateTime? ttDate,int uploadKey)
        //{
        //    APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
        //    APPayment apPayment = (APPayment)entry.Document.Cache.CreateInstance();
        //    KGBillPayment billPayment = GetBillPayment(billPaymentID);
        //    APInvoice invoice = GetAPInvoiceByRefNbr(billPayment.RefNbr);

        //    apPayment.CuryID           = invoice.CuryID;
        //    apPayment.CuryInfoID       = invoice.CuryInfoID;
        //    apPayment.BranchID         = invoice.BranchID;
        //    apPayment.Hold             = true;
        //    apPayment.DocType          = APDocType.Check;
        //    apPayment.VendorID         = invoice.VendorID;
        //    //20220325 by louis �ק�APPayment Vendor/vendorLocation���n��APInvoice���Y��
        //    //Location location = GetLocation(billPayment.VendorLocationID, invoice.VendorID);
        //    Location location = GetLocation(invoice.VendorLocationID, invoice.VendorID);
        //    //20220325 by louis �ק�APPayment Vendor/vendorLocation���n��APInvoice���Y��
        //    apPayment.VendorLocationID = location.LocationID; //billPayment.VendorLocationID;
        //    apPayment.PaymentMethodID  = location.VPaymentMethodID;
        //    apPayment.CashAccountID    = location.CashAccountID;

        //    apPayment = entry.Document.Insert(apPayment);

        //    //20220428 by louis APPayment.AdjDate�g�J��ڪ��I�ڤ�
        //    //20220721 by louis APPayment.AdjDate�g�J�q�צ^�X�ɹ�ڥI�ڤ��
        //    apPayment.DocDate          = apPayment.AdjDate = ttDate;
        //    apPayment.CuryOrigDocAmt   = billPayment.PaymentAmount;
        //    apPayment.ExtRefNbr        = invoice.RefNbr;
        //    //20221011 by Alton TT�Ȧ�q���ɪ�UploadKey�n�·Ц^�X����APPayment�W�X�@�����ө�o�ӭ� (UsrTTUploadKey) 
        //    apPayment.GetExtension<APPaymentExt>().UsrTTUploadKey = uploadKey;

        //    entry.Document.Update(apPayment);

        //    #region ����
        //    APAdjust detail = (APAdjust)entry.Adjustments.Cache.CreateInstance();
        //    detail.AdjdDocType = invoice.DocType;
        //    detail.AdjdRefNbr = invoice.RefNbr;
        //    detail = entry.Adjustments.Insert(detail);
        //    entry.Adjustments.Cache.SetValueExt<APAdjust.curyAdjgAmt>(detail, billPayment.PaymentAmount);
        //    entry.Adjustments.Update(detail);
        //    #endregion

        //    entry.Document.Cache.SetValueExt<APPayment.hold>(apPayment, false);
        //    // Store the source field value as suggested by Louis for future auditing.
        //    entry.Document.Cache.SetValueExt<APPaymentExt.usrBillPaymentID>(apPayment, billPayment.BillPaymentID);
        //    entry.Document.Update(apPayment);

        //    entry.Save.Press();
        //    entry.release.Press();
        //}

        private String GetString(ref String s, int lenght)
        {
            String str = "";
            int startIdx = 0;
            int _lenght = 0;
            CharEnumerator ce = s.GetEnumerator();
            while (ce.MoveNext() && _lenght < lenght)
            {
                char c = ce.Current;
                Byte[] bytes = System.Text.Encoding.GetEncoding(ENCODING).GetBytes(c.ToString());
                _lenght += bytes.Length;
                str += c;
                startIdx++;
            }
            s = s.Substring(startIdx);
            return str.Trim();
        }

        private Decimal GetDecimal(String s)
        {
            String str = s.TrimStart('0');
            String num = str.Substring(0, str.Length - 2) + "." + str.Substring(str.Length - 2, 2);
            return Decimal.Parse(num);
        }
        #endregion

        #region BQL
        private PXResultset<Location> GetLocation(int? locationID, int? baccountID)
        {
            return PXSelect<Location,
                Where<Location.locationID, Equal<Required<Location.locationID>>,
                And<Location.bAccountID, Equal<Required<Location.bAccountID>>>>>
                 .Select(this, locationID, baccountID);
        }

        private PXResultset<APInvoice> GetAPInvoiceByRefNbr(string refNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                 .Select(this, refNbr);
        }

        private PXResultset<KGBillPayment> GetBillPayment(int? billPaymentID)
        {
            return PXSelect<KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                 .Select(this, billPaymentID);
        }
        private PXResultset<NMApTeleTransLog> GetLogByID(int? logID)
        {
            return PXSelect<NMApTeleTransLog,
                 Where<NMApTeleTransLog.logID, Equal<Required<NMApTeleTransLog.logID>>>>
                    .Select(this, logID);
        }

        /// <summary>
        /// ���oisNew = False������
        /// </summary>
        /// <param name="billPaymentID"></param>
        /// <returns></returns>
        private PXResultset<NMApTeleTransLog> GetLogByBillPaymentID(int? billPaymentID)
        {
            return PXSelect<NMApTeleTransLog,
                 Where<NMApTeleTransLog.billPaymentID, Equal<Required<NMApTeleTransLog.billPaymentID>>,
                    And<NMApTeleTransLog.isNew, Equal<False>>
                 >>
                  .Select(this, billPaymentID);
        }

        private NMPayableCheck GetPayableCheck(string payableCD)
        {
            return PXSelect<NMPayableCheck,
                Where<NMPayableCheck.payableCheckCD, Equal<Required<NMPayableCheck.payableCheckCD>>>>
                .Select(this, payableCD);
        }

        #endregion

        #region Tables

        #region ParamTable
        [Serializable]
        public class ParamTable : IBqlTable
        {
            #region FeedbackType
            [PXString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Feedback Type", Required = true)]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [NMApFeedbackType]
            public virtual string FeedbackType { get; set; }
            public abstract class feedbackType : PX.Data.BQL.BqlString.Field<feedbackType> { }
            #endregion

            #region NowFeedbackType
            [PXString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
            public virtual string NowFeedbackType { get; set; }
            public abstract class nowFeedbackType : PX.Data.BQL.BqlString.Field<nowFeedbackType> { }
            #endregion

            #region TempData
            public List<DetailsTable> TempData { get; set; }
            #endregion
        }
        #endregion

        #region DetailsTable
        [Serializable]
        public class DetailsTable : IBqlTable
        {
            #region NMApTeleTransLog
            public NMApTeleTransLog OldLog { get; set; }
            #endregion

            #region CustNbr
            /// <summary>
            /// �Τ�ۭq�Ǹ�<br></br>
            /// �x�s�q��<br></br>
            /// </summary>
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Cust Nbr", IsReadOnly = true, Visible = false)]
            public virtual string CustNbr { get; set; }
            public abstract class custNbr : PX.Data.BQL.BqlString.Field<custNbr> { }
            #endregion

            #region RefNbr
            /// <summary>
            /// �p���渹<br></br>
            /// �x�s�q��<br></br>
            /// ����q��<br></br>
            /// </summary>
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Ref Nbr", IsReadOnly = true, Visible = false)]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region UploadKey
            /// <summary>
            /// �x�s�N�}-�Τ�ۭq�Ǹ�<br></br>
            /// �x�s�q��-��b��Key��<br></br>
            /// ����q��-����s��<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "", IsKey = true)]
            [PXUIField(DisplayName = "Upload Key", IsReadOnly = true, Visible = false)]
            public virtual string UploadKey { get; set; }
            public abstract class uploadKey : PX.Data.BQL.BqlString.Field<uploadKey> { }
            #endregion

            #region Date
            /// <summary>
            /// �x�s�N�}-�o�����<br></br>
            /// �x�s�q��-�I�ڤ��<br></br>
            /// </summary>
            [PXDate()]
            [PXUIField(DisplayName = "Date", IsReadOnly = true, Visible = false)]
            public virtual DateTime? Date { get; set; }
            public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }
            #endregion

            #region CheckNbr
            /// <summary>
            /// �N�}-�䲼���X
            /// </summary>
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Check Nbr", IsReadOnly = true, Visible = false)]
            public virtual string CheckNbr { get; set; }
            public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
            #endregion

            #region TnBankCheckStatus
            /// <summary>
            /// �N�}-���A
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Status", IsReadOnly = true, Visible = false)]
            [NMApTnBankCheckStatus]
            public virtual string TnBankCheckStatus { get; set; }
            public abstract class tnBankCheckStatus : PX.Data.BQL.BqlString.Field<tnBankCheckStatus> { }
            #endregion

            #region TnTtStatus
            /// <summary>
            /// �x�s�q��-��ƪ��A<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Status", IsReadOnly = true, Visible = false)]
            [NMApTnTtStatus]
            public virtual string TnTtStatus { get; set; }
            public abstract class tnTtStatus : PX.Data.BQL.BqlString.Field<tnTtStatus> { }
            #endregion

            #region GtTtStatus
            /// <summary>
            /// ����q��-������A<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Status", IsReadOnly = true, Visible = false)]
            [NMApGtTtStatus]
            public virtual string GtTtStatus { get; set; }
            public abstract class gtTtStatus : PX.Data.BQL.BqlString.Field<gtTtStatus> { }
            #endregion

            #region Amount
            /// <summary>
            /// �x�s�N�}-���B<br></br>
            /// �x�s�q��-�I�ڪ��B<br></br>
            /// ����q��-�״ڪ��B<br></br>
            /// </summary>
            [PXDecimal()]
            [PXUIField(DisplayName = "Amount", IsReadOnly = true, Visible = false)]
            public virtual decimal? Amount { get; set; }
            public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
            #endregion

            #region RecBankCode
            /// <summary>
            /// ����q��-���ڦ�N��<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Receive Bank Code", IsReadOnly = true, Visible = false)]
            public virtual string RecBankCode { get; set; }
            public abstract class recBankCode : PX.Data.BQL.BqlString.Field<recBankCode> { }
            #endregion

            #region RecAccount
            /// <summary>
            /// �x�s�q��-���ڱb��<br></br>
            /// ����q��-���ڤH�b��<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Receive Account", IsReadOnly = true, Visible = false)]
            public virtual string RecAccount { get; set; }
            public abstract class recAccount : PX.Data.BQL.BqlString.Field<recAccount> { }
            #endregion

            #region Receiver
            /// <summary>
            /// �x�s�N�}-���ڤH<br></br>
            /// �x�s�q��-���ڤ�W<br></br>
            /// ����q��-���ڤH��W<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Receiver", IsReadOnly = true, Visible = false)]
            public virtual string Receiver { get; set; }
            public abstract class receiver : PX.Data.BQL.BqlString.Field<receiver> { }
            #endregion

            #region ErrorMsg
            /// <summary>
            /// ����q��-���~�N�X<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Error Message", IsReadOnly = true, Visible = false)]
            [NMApGtTtErrorCode]
            public virtual string ErrorMsg { get; set; }
            public abstract class errorMsg : PX.Data.BQL.BqlString.Field<errorMsg> { }
            #endregion

            #region Reason
            /// <summary>
            /// ����q��-�~�s�h�|�z��<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Reason", IsReadOnly = true, Visible = false)]
            [NMApGtTtReasonCode]
            public virtual string Reason { get; set; }
            public abstract class reason : PX.Data.BQL.BqlString.Field<reason> { }
            #endregion

            #region TranDate
            /// <summary>
            /// ����q��-��ڥ����<br></br>
            /// </summary>
            [PXDate()]
            [PXUIField(DisplayName = "Tran Date", IsReadOnly = true, Visible = false)]
            public virtual DateTime? TranDate { get; set; }
            public abstract class tranDate : PX.Data.BQL.BqlDateTime.Field<tranDate> { }
            #endregion

            #region TranTime
            /// <summary>
            /// ����q��-��ڥ���ɶ�<br></br>
            /// </summary>
            [PXString()]
            [PXUIField(DisplayName = "Tran Time", IsReadOnly = true, Visible = false)]
            public virtual String TranTime { get; set; }
            public abstract class tranTime : PX.Data.BQL.BqlDateTime.Field<tranTime> { }
            #endregion

            #region RemittanceNbr
            /// <summary>
            /// ����q��-�]���״ڧǸ�<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Remittance Nbr", IsReadOnly = true, Visible = false)]
            public virtual string RemittanceNbr { get; set; }
            public abstract class remittanceNbr : PX.Data.BQL.BqlString.Field<remittanceNbr> { }
            #endregion

            #region TransFee
            /// <summary>
            /// ����q��-����O<br></br>
            /// </summary>
            [PXDecimal()]
            [PXUIField(DisplayName = "Trans Fee", IsReadOnly = true, Visible = false)]
            public virtual decimal? TransFee { get; set; }
            public abstract class transFee : PX.Data.BQL.BqlDecimal.Field<transFee> { }
            #endregion

            #region RowError
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            public virtual string RowError { get; set; }
            public abstract class rowError : PX.Data.BQL.BqlString.Field<rowError> { }
            #endregion
        }
        #endregion

        #endregion
    }
}
