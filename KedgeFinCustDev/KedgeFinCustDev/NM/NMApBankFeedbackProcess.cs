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
     * 因原先代碼顯示為中文早成比對資料時誤判，改轉為StringList處理
     * 
     *  * ===2021-03-02:11864=====Alton
     * 2.回饋失敗時，不需要將IsCheckIssue改為False
     * 
     * ====2021-07-08:12136====Alton
     * TT APPayment過帳時產生 APPayment的GL傳票 & 郵資費傳票
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

        #region 讀取回饋檔
        public PXAction<ParamTable> FileUpload;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "讀取回饋檔")]
        protected IEnumerable fileUpload(PXAdapter adapter)
        {
            ParamTable p = MasterView.Current;
            String sessionKey = "FeedbackSessionKey";
            if (p.FeedbackType == null) throw new Exception("請選擇回饋類型");


            if (UploadDialog.AskExt() == WebDialogResult.OK)
            {
                //取得上傳檔案資訊
                PX.SM.FileInfo info = PXContext.SessionTyped<PXSessionStatePXData>().FileInfo[sessionKey] as PX.SM.FileInfo;
                if (info == null) throw new Exception("請選擇上傳檔案");
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

                //清除檔案資訊
                sr.Close();
                System.Web.HttpContext.Current.Session.Remove(sessionKey);

            }
            return adapter.Get();
        }
        #endregion

        #region 執行
        public PXAction<ParamTable> Submit;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "執行")]
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
                                    item.RowError = "無代開紀錄";
                                    errorData.Add(item);
                                }
                                else
                                {
                                    SubmitBankCheck(item, checkItem);
                                }
                            }
                            else if (p.NowFeedbackType == NMApFeedbackType.TN_TT || p.NowFeedbackType == NMApFeedbackType.GT_TT)
                            {
                                //檢查是否存在對應的log
                                if (item.OldLog == null)
                                {
                                    item.RowError = "此紀錄已結束，找不到對應紀錄";
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

                //取得清除前的FeedbackType
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
                    //產生APPayment且過帳
                    appayment = NMApCheckEntry.CreateApPayment(apEntry, ref checkItem, false);
                    checkItem.Status = NMAPCheckStatus.CONFIRM;
                    checkItem.ConfirmBatchNbr = NMVoucherUtil.CreateAPVoucher(NMAPVoucher.CONFIRM, checkItem, GLStageCode.NMPConfirmP1);
                }
            }
            checkItem.FeedbackStatus = item.TnBankCheckStatus;
            Checks.Update(checkItem);
            this.Persist();
            #region APPayment過帳
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
            //更新OldLog isNew = False
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
                //20220311 add by louis 新增寫入使用者自訂序號
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
                    + (String.IsNullOrEmpty(reason) ? "" : ("，" + reason));
                if (item.GtTtStatus == NMApGtTtStatus._1)
                {
                    log.Status = NMApTtLogStatus.FEED_BACK_SUCCESS;
                }
            }

            if (log.Status == NMApTtLogStatus.FEED_BACK_SUCCESS)
            {
                //2022-10-17 Alton add 更新usrCreatePaymentType =2 (TT)
                PXUpdate<
                    Set<KGBillPayment.ttActDate, Required<KGBillPayment.ttActDate>,
                    Set<KGBillPaymentExt.usrCreatePaymentType, Required<KGBillPaymentExt.usrCreatePaymentType>>>,
                KGBillPayment,
                Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                .Update(this, item.Date, NMApCreatePaymentType.TT, item.OldLog.BillPaymentID);

                //如果回饋成功則則產生APPayment
                //2022-10-17 Alton 改由 NMApPaymentProcess排程執行
                //CreateAPPaymentByTT(item.OldLog.BillPaymentID, item.Date,int.Parse(item.UploadKey));
                #region 產生APPayment 傳票 & 郵資傳票
                KGBillPayment billPayment = GetBillPayment(item.OldLog.BillPaymentID);
                string t1BatchNbr = NMVoucherUtil.CreateTTVoucher(NMTTVoucher.APPAYMENT, billPayment, item.Date, GLStageCode.NMPTtFeedbackT1);
                //[Jira KED-19] 2022-12-16 Alton : 取消產生T2傳票
                //NMVoucherUtil.CreateTTVoucher(NMTTVoucher.WRITEOFFPOSTAGE, billPayment, item.Date, GLStageCode.NMPTtFeedbackT2, t1BatchNbr);
                #endregion
                //因已完單，清除其餘紀錄
                foreach (NMApTeleTransLog oldLog in GetLogByBillPaymentID(log.BillPaymentID))
                {
                    Logs.Delete(oldLog);
                }
            }
            //else
            //{
            //否則更新KGBillPayment.UsrIsCheckIssue = false 讓使用者可以轉為支票
            //2021-03-02:11864 Mark By Alton：取消更新IsCheckIssue = False，因為已經可以在此狀態下「改開票」
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
        /// 解析台新代開回饋檔
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
                //用戶自訂序號1,7
                item.UploadKey = GetString(ref txt, 7);
                //發票日期8,8
                String date = GetString(ref txt, 8);
                item.Date = DateTime.ParseExact(date, "yyyyMMdd", provider);
                //支票號碼16,9
                item.CheckNbr = GetString(ref txt, 9);
                //狀態25,1
                item.TnBankCheckStatus = GetString(ref txt, 1);
                //發票人帳號26,17
                GetString(ref txt, 17);
                //金額43,18
                String amount = GetString(ref txt, 18);
                item.Amount = GetDecimal(amount);
                //受款人61,60
                item.Receiver = GetString(ref txt, 60);
                //121以下省略
                tempData.Add(item);
            }
            MasterView.Current.TempData = tempData;
        }
        /// <summary>
        /// 解析台新電匯回饋檔
        /// </summary>
        public virtual void ReadTnTT(StreamReader sr)
        {
            String txt;
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            List<DetailsTable> tempData = new List<DetailsTable>();
            while ((txt = sr.ReadLine()) != null)
            {
                DetailsTable item = new DetailsTable();
                //用戶自訂序號1,7
                //20220310 modify by louis 新增顯示用戶自訂序號
                //GetString(ref txt, 7);
                item.CustNbr = GetString(ref txt, 7);
                //付款日期8,8
                String date = GetString(ref txt, 8);
                item.Date = DateTime.ParseExact(date, "yyyyMMdd", provider);
                //付款金額16,18
                String amount = GetString(ref txt, 18);
                item.Amount = GetDecimal(amount);
                //付款帳號34,17
                GetString(ref txt, 17);
                //付款戶名51,60
                GetString(ref txt, 60);
                //收款帳號111,17
                item.RecAccount = GetString(ref txt, 17);
                //收款戶名128,60
                item.Receiver = GetString(ref txt, 60);
                //付款總行188,3
                GetString(ref txt, 3);
                //付款分行191,4
                GetString(ref txt, 4);
                //收款總行195,3
                GetString(ref txt, 3);
                //收款分行198,4
                GetString(ref txt, 4);
                //附言202,100
                GetString(ref txt, 100);
                //收款人識別碼302,17
                GetString(ref txt, 17);
                //收款人代碼識別319,3
                GetString(ref txt, 3);
                //付款人識別碼17
                GetString(ref txt, 17);
                //付款人代碼識別3
                GetString(ref txt, 3);
                //手續費負擔別3
                GetString(ref txt, 3);
                //對帳單key值30
                item.UploadKey = GetString(ref txt, 30);
                //付款聯絡人35
                GetString(ref txt, 35);
                //付款連絡電話25
                GetString(ref txt, 25);
                //付款傳真號碼25
                GetString(ref txt, 25);
                //收款聯絡人35
                GetString(ref txt, 35);
                //收款連絡電話25
                GetString(ref txt, 25);
                //收款傳真號碼25
                GetString(ref txt, 25);
                //收款通知信箱50
                GetString(ref txt, 50);
                //資料狀態595,2
                item.TnTtStatus = GetString(ref txt, 2);
                //item.TnTtStatus =
                //    status == "01" ? "編輯，資料尚未送審" :
                //    status == "02" ? "送審，資料已送審，尚未審核" :
                //    status == "03" ? "待審核，資料已審核，但未完成所有的審核" :
                //    status == "04" ? "待放行，資料已完成所有的審核，等待放行" :
                //    status == "05" ? "退件，資料被審核人員或是放行人員退件" :
                //    status == "06" ? "等待回應，資料已放行，FEDI Server尚未處理" :
                //    status == "07" ? "上傳主機，資料已放行，FEDI Server已驗章完成，等待後續帳務處理" :
                //    status == "08" ? "扣帳成功" :
                //    status == "09" ? "付款失敗" :
                //    status == "10" ? "退帳通知，資料扣帳成功，但入帳時被入帳行退回" :
                //    status == "11" ? "預約取消，資料原已預約，被用戶取消付款" :
                //    status == "14" ? "退帳通知(人工回沖)，財經公司通知，進行入工退帳" : ""
                //    ;

                //取得log
                item.OldLog = GetOldLog(item.UploadKey);
                item.RefNbr = item.OldLog?.TeleTransRefNbr;

                tempData.Add(item);
            }
            MasterView.Current.TempData = tempData;
        }
        /// <summary>
        /// 解析國泰電匯回饋檔
        /// </summary>
        public virtual void ReadGtTT(StreamReader sr)
        {
            String txt;
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            List<DetailsTable> tempData = new List<DetailsTable>();
            while ((txt = sr.ReadLine()) != null)
            {
                DetailsTable item = new DetailsTable();
                //識別代碼,1
                GetString(ref txt, 1);
                //檔案上傳日期,8
                GetString(ref txt, 8);
                //預定交易日期,8
                GetString(ref txt, 8);
                //交易類別,3
                GetString(ref txt, 3);
                //交易編號,10
                item.UploadKey = GetString(ref txt, 10);
                //付款行代碼,7
                GetString(ref txt, 7);
                //付款人帳號,16
                GetString(ref txt, 16);
                //付款人統編,10
                GetString(ref txt, 10);
                //付款人戶名,70
                GetString(ref txt, 70);
                //幣別,3
                GetString(ref txt, 3);
                //金額正負號,1
                GetString(ref txt, 1);
                //金額,14
                GetString(ref txt, 14);
                //收款行代碼,7
                item.RecBankCode = GetString(ref txt, 7);
                //收款人帳號,16?
                item.RecAccount = GetString(ref txt, 16);
                //收款人統編,10
                GetString(ref txt, 10);
                //收款人戶名,70
                item.Receiver = GetString(ref txt, 70);
                //收款人是否電告,1
                GetString(ref txt, 1);
                //電告設備號碼,50
                GetString(ref txt, 50);
                //手續費分攤方式,2
                GetString(ref txt, 2);
                //發票筆數,4
                GetString(ref txt, 4);
                //備註,50
                GetString(ref txt, 50);
                //交易狀態,3
                item.GtTtStatus = GetString(ref txt, 3);
                //錯誤代碼,4
                item.ErrorMsg = GetString(ref txt, 4);
                //item.ErrorMsg =
                //    errorCode == "0000" ? "0000:成功" :
                //    errorCode == "M323" ? "M323:退回" :
                //    errorCode == "M911" ? "M911:存款不足" : "";
                //財金退匯理由,1
                item.Reason = GetString(ref txt, 1);
                //item.Reason =
                //    reasonCode == "3" ? "帳號錯誤" :
                //    reasonCode == "4" ? "姓名不符" :
                //    reasonCode == "9" ? "滯納" : "";
                //財金匯款序號,7
                item.RemittanceNbr = GetString(ref txt, 7);
                //實際交易日期,8
                String date = GetString(ref txt, 8);
                item.TranDate = DateTime.ParseExact(date, "yyyyMMdd", provider);
                //實際交易時間,6
                String time = GetString(ref txt, 6);
                DateTime dt = DateTime.ParseExact(time, "HHmmss", provider);
                item.TranTime = dt.ToString("HH:mm:ss");
                //匯款金額,14
                String amount = GetString(ref txt, 14);
                item.Amount = GetDecimal(amount);
                //匯款手續費,6
                String transFee = GetString(ref txt, 6);
                item.TransFee = GetDecimal(transFee);

                //取得log
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
                //2021-01-20 Edit By Alton:11881 uploadKey改為logID
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
            // 20220310 modify by louis 新增顯示用戶自訂序號
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

        //2022-10-17 Alton 電匯產生APPayment 改由NMApPaymentProcess排程執行
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
        //    //20220325 by louis 修改APPayment Vendor/vendorLocation都要抓APInvoice表頭的
        //    //Location location = GetLocation(billPayment.VendorLocationID, invoice.VendorID);
        //    Location location = GetLocation(invoice.VendorLocationID, invoice.VendorID);
        //    //20220325 by louis 修改APPayment Vendor/vendorLocation都要抓APInvoice表頭的
        //    apPayment.VendorLocationID = location.LocationID; //billPayment.VendorLocationID;
        //    apPayment.PaymentMethodID  = location.VPaymentMethodID;
        //    apPayment.CashAccountID    = location.CashAccountID;

        //    apPayment = entry.Document.Insert(apPayment);

        //    //20220428 by louis APPayment.AdjDate寫入實際的付款日
        //    //20220721 by louis APPayment.AdjDate寫入電匯回饋檔實際付款日期
        //    apPayment.DocDate          = apPayment.AdjDate = ttDate;
        //    apPayment.CuryOrigDocAmt   = billPayment.PaymentAmount;
        //    apPayment.ExtRefNbr        = invoice.RefNbr;
        //    //20221011 by Alton TT銀行電匯檔的UploadKey要麻煩回饋後放到APPayment上擴一個欄位來放這個值 (UsrTTUploadKey) 
        //    apPayment.GetExtension<APPaymentExt>().UsrTTUploadKey = uploadKey;

        //    entry.Document.Update(apPayment);

        //    #region 明細
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
        /// 取得isNew = False之紀錄
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
            /// 用戶自訂序號<br></br>
            /// 台新電匯<br></br>
            /// </summary>
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Cust Nbr", IsReadOnly = true, Visible = false)]
            public virtual string CustNbr { get; set; }
            public abstract class custNbr : PX.Data.BQL.BqlString.Field<custNbr> { }
            #endregion

            #region RefNbr
            /// <summary>
            /// 計價單號<br></br>
            /// 台新電匯<br></br>
            /// 國泰電匯<br></br>
            /// </summary>
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Ref Nbr", IsReadOnly = true, Visible = false)]
            public virtual string RefNbr { get; set; }
            public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
            #endregion

            #region UploadKey
            /// <summary>
            /// 台新代開-用戶自訂序號<br></br>
            /// 台新電匯-對帳單Key值<br></br>
            /// 國泰電匯-交易編號<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "", IsKey = true)]
            [PXUIField(DisplayName = "Upload Key", IsReadOnly = true, Visible = false)]
            public virtual string UploadKey { get; set; }
            public abstract class uploadKey : PX.Data.BQL.BqlString.Field<uploadKey> { }
            #endregion

            #region Date
            /// <summary>
            /// 台新代開-發票日期<br></br>
            /// 台新電匯-付款日期<br></br>
            /// </summary>
            [PXDate()]
            [PXUIField(DisplayName = "Date", IsReadOnly = true, Visible = false)]
            public virtual DateTime? Date { get; set; }
            public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }
            #endregion

            #region CheckNbr
            /// <summary>
            /// 代開-支票號碼
            /// </summary>
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "Check Nbr", IsReadOnly = true, Visible = false)]
            public virtual string CheckNbr { get; set; }
            public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
            #endregion

            #region TnBankCheckStatus
            /// <summary>
            /// 代開-狀態
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Status", IsReadOnly = true, Visible = false)]
            [NMApTnBankCheckStatus]
            public virtual string TnBankCheckStatus { get; set; }
            public abstract class tnBankCheckStatus : PX.Data.BQL.BqlString.Field<tnBankCheckStatus> { }
            #endregion

            #region TnTtStatus
            /// <summary>
            /// 台新電匯-資料狀態<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Status", IsReadOnly = true, Visible = false)]
            [NMApTnTtStatus]
            public virtual string TnTtStatus { get; set; }
            public abstract class tnTtStatus : PX.Data.BQL.BqlString.Field<tnTtStatus> { }
            #endregion

            #region GtTtStatus
            /// <summary>
            /// 國泰電匯-交易狀態<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Status", IsReadOnly = true, Visible = false)]
            [NMApGtTtStatus]
            public virtual string GtTtStatus { get; set; }
            public abstract class gtTtStatus : PX.Data.BQL.BqlString.Field<gtTtStatus> { }
            #endregion

            #region Amount
            /// <summary>
            /// 台新代開-金額<br></br>
            /// 台新電匯-付款金額<br></br>
            /// 國泰電匯-匯款金額<br></br>
            /// </summary>
            [PXDecimal()]
            [PXUIField(DisplayName = "Amount", IsReadOnly = true, Visible = false)]
            public virtual decimal? Amount { get; set; }
            public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
            #endregion

            #region RecBankCode
            /// <summary>
            /// 國泰電匯-收款行代馬<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Receive Bank Code", IsReadOnly = true, Visible = false)]
            public virtual string RecBankCode { get; set; }
            public abstract class recBankCode : PX.Data.BQL.BqlString.Field<recBankCode> { }
            #endregion

            #region RecAccount
            /// <summary>
            /// 台新電匯-收款帳號<br></br>
            /// 國泰電匯-收款人帳號<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Receive Account", IsReadOnly = true, Visible = false)]
            public virtual string RecAccount { get; set; }
            public abstract class recAccount : PX.Data.BQL.BqlString.Field<recAccount> { }
            #endregion

            #region Receiver
            /// <summary>
            /// 台新代開-受款人<br></br>
            /// 台新電匯-收款戶名<br></br>
            /// 國泰電匯-收款人戶名<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Receiver", IsReadOnly = true, Visible = false)]
            public virtual string Receiver { get; set; }
            public abstract class receiver : PX.Data.BQL.BqlString.Field<receiver> { }
            #endregion

            #region ErrorMsg
            /// <summary>
            /// 國泰電匯-錯誤代碼<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Error Message", IsReadOnly = true, Visible = false)]
            [NMApGtTtErrorCode]
            public virtual string ErrorMsg { get; set; }
            public abstract class errorMsg : PX.Data.BQL.BqlString.Field<errorMsg> { }
            #endregion

            #region Reason
            /// <summary>
            /// 國泰電匯-才新退會理由<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Reason", IsReadOnly = true, Visible = false)]
            [NMApGtTtReasonCode]
            public virtual string Reason { get; set; }
            public abstract class reason : PX.Data.BQL.BqlString.Field<reason> { }
            #endregion

            #region TranDate
            /// <summary>
            /// 國泰電匯-實際交易日<br></br>
            /// </summary>
            [PXDate()]
            [PXUIField(DisplayName = "Tran Date", IsReadOnly = true, Visible = false)]
            public virtual DateTime? TranDate { get; set; }
            public abstract class tranDate : PX.Data.BQL.BqlDateTime.Field<tranDate> { }
            #endregion

            #region TranTime
            /// <summary>
            /// 國泰電匯-實際交易時間<br></br>
            /// </summary>
            [PXString()]
            [PXUIField(DisplayName = "Tran Time", IsReadOnly = true, Visible = false)]
            public virtual String TranTime { get; set; }
            public abstract class tranTime : PX.Data.BQL.BqlDateTime.Field<tranTime> { }
            #endregion

            #region RemittanceNbr
            /// <summary>
            /// 國泰電匯-財金匯款序號<br></br>
            /// </summary>
            [PXString(IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Remittance Nbr", IsReadOnly = true, Visible = false)]
            public virtual string RemittanceNbr { get; set; }
            public abstract class remittanceNbr : PX.Data.BQL.BqlString.Field<remittanceNbr> { }
            #endregion

            #region TransFee
            /// <summary>
            /// 國泰電匯-手續費<br></br>
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
