using System;
using NM.DAC;
using NM.Util;
using PX.Data;
using RC.Util;
using PX.Objects.GL;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using KG.Util;
using Kedge.DAC;
using PX.Objects.AP;
using PX.Objects.CT;
using PX.Objects.CS;


/**
 * ====2021-05-20====Alton
 * 追APInvoice加到期日條件
 * ====2021-06-07:User 上傳出錯====Alton
 * 上傳失敗!ORA-01400:cannot insert NULL into ("VAB","UPLD2_DETL","KEY_VALE")
 * Fix：補上對帳單key值(AP單號)
 * **/
namespace NM
{
    public class NMApCheckBankProcess : PXGraph<NMApCheckBankProcess>
    {

        public NMApCheckBankProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_PAYABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }
        }
        #region const

        public const string TN_BANKCODE = "812";
        public const string BankIsNotTN = "銀行非台新";
        public const string CreateFailed = "產生失敗";
        public const string PlzSelectRow = "Please at least select one record!";
        #endregion

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXSelect<DetailTable, Where<DetailTable.status, Equal<Current2<ParamTable.checkStatus>>,
               And2<Where<
                   DetailTable.payableCashierID, Equal<Current2<ParamTable.cashierID>>,
                   Or<Current2<ParamTable.cashierID>, IsNull>>,
               And2<Where<
                   DetailTable.bankCode, Equal<Current2<ParamTable.bankCode>>,
                   Or<Current2<ParamTable.bankCode>, IsNull>>,
               And2<Where<
                   DetailTable.vendorID, Equal<Current2<ParamTable.vendorID>>,
                   Or<Current2<ParamTable.vendorID>, IsNull>>,
               And2<Where<
                   DetailTable.vendorLocationID, Equal<Current2<ParamTable.vendorLocationID>>,
                   Or<Current2<ParamTable.vendorLocationID>, IsNull>>,
               And2<Where<
                   DetailTable.projectPeriod, GreaterEqual<Current2<ParamTable.projectStartPeriod>>,
                   Or<Current2<ParamTable.projectStartPeriod>, IsNull>>,
               And2<Where<
                   DetailTable.projectPeriod, LessEqual<Current2<ParamTable.projectEndPeriod>>,
                   Or<Current2<ParamTable.projectEndPeriod>, IsNull>>,
               And2<Where<
                   DetailTable.apDueDate, Equal<Current2<ParamTable.dueDate>>,
                   Or<Current2<ParamTable.dueDate>, IsNull>>,
               And2<Where<
                   DetailTable.checkDate, GreaterEqual<Current2<ParamTable.checkStartDate>>,
                   Or<Current2<ParamTable.checkStartDate>, IsNull>>,
               And2<Where<
                   DetailTable.checkDate, LessEqual<Current2<ParamTable.checkEndDate>>,
                   Or<Current2<ParamTable.checkEndDate>, IsNull>>,
               And2<Where<DetailTable.projectID, Equal<Current2<ParamTable.projectID>>,
                   Or<Current2<ParamTable.projectID>, IsNull>>,
               And2<Where<Current2<ParamTable.usrTrConfirmBy>, IsNull,
                   Or<DetailTable.usrTrConfirmBy, Equal<Current2<ParamTable.usrTrConfirmBy>>>>,
               And2<Where<Current2<ParamTable.usrTrConfirmDate>, IsNull,
                   Or<DetailTable.usrTrConfirmDate, Equal<Current2<ParamTable.usrTrConfirmDate>>>>,
               And2<Where<Current2<ParamTable.usrTrConfirmID>, IsNull,
                   Or<DetailTable.usrTrConfirmID, Equal<Current2<ParamTable.usrTrConfirmID>>>>,
               And<Where<Current2<ParamTable.usrTrPaymentType>, IsNull,
                   Or<DetailTable.usrTrPaymentType, Equal<Current2<ParamTable.usrTrPaymentType>>>>
                   >>>>>>>>>>>>>>>
            > DetailsView;
        #endregion

        #region Action
        #region 產生台新支票檔
        public PXAction<ParamTable> DowloadEzCheck;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "產生台新代開支票檔")]
        protected IEnumerable dowloadEzCheck(PXAdapter adapter)
        {
            List<NMPayableCheck> list = new List<NMPayableCheck>();
            bool hasError = false;
            foreach (NMPayableCheck item in DetailsView.Select())
            {
                if (item.Selected != true) continue;
                NMBankAccount ba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableCheck.bankAccountID>(DetailsView.Cache, item);
                if (ba.BankCode.Substring(0, 3) != "812")
                {
                    DetailsView.Cache.RaiseExceptionHandling<NMPayableCheck.bankAccountID>(
                            item, item.RefNbr, new PXSetPropertyException(BankIsNotTN, PXErrorLevel.RowError));
                    hasError = true;
                }
                list.Add(item);
            }
            if (list.Count == 0) throw new PXException(PlzSelectRow);
            if (hasError) throw new PXException(CreateFailed);

            PXLongOperation.StartOperation(this, delegate () { dowloadTXT(list); });
            return adapter.Get();
        }
        #endregion

        #region HyperLink
        public PXAction<DetailTable> ViewCheck;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewCheck()
        {
            NMPayableCheck check = (NMPayableCheck)PXSelectorAttribute.Select<DetailTable.payableCheckCD>(DetailsView.Cache, DetailsView.Current);
            new HyperLinkUtil<NMApCheckEntry>(check, true);
        }

        #region APReleaseGLBatchNbr
        public PXAction<DetailTable> ViewAPReleaseGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPReleaseGL()
        {
            Batch batch = (Batch)PXSelectorAttribute.Select<DetailTable.aPInvBatchNbr>(DetailsView.Cache, DetailsView.Current);
            new HyperLinkUtil<JournalEntry>(batch, true);
        }
        #endregion


        #endregion

        #endregion

        #region Method
        public virtual void dowloadTXT(List<NMPayableCheck> list)
        {

            using (System.IO.MemoryStream zipMs = new System.IO.MemoryStream())
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    PX.DbServices.ZipArchive zip = new PX.DbServices.ZipArchive(zipMs);
                    //2021-01-29 Add By Alton : 11919
                    String batchNbr = NMBatchNbrUtil.GetNextBatchNbr(this);
                    String fileName = "Kedge_ezCheck_" + batchNbr;
                    RCEleTxtFileUtil fileUtil_H = new RCEleTxtFileUtil(fileName, "BIG5");
                    RCEleTxtFileUtil fileUtil_D = new RCEleTxtFileUtil(fileName + "-1", "BIG5");
                    foreach (NMPayableCheck item in list)
                    {
                        PXUpdate<Set<NMPayableCheck.status, NMStringList.NMAPCheckStatus.represent,
                            Set<NMPayableCheck.sourceCode, NMStringList.NMAPSourceCode.represent,
                            Set<NMPayableCheck.isIssuedByBank, True,
                            Set<NMPayableCheck.bankAccountID, Required<NMPayableCheck.bankAccountID>,
                            Set<NMPayableCheck.bookNbr, Null>>>>>,
                        NMPayableCheck,
                        Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>
                        >.Update(this, item.BankAccountID, item.PayableCheckID);
                        //2021-01-29 Add By Alton : 11919 更新KGBillPayment.batchNbr
                        NMBatchNbrUtil.UpdateBillPaymentBatchNbr(this, batchNbr, item.BillPaymentID);

                        inputTXT(item, fileUtil_H);
                        inputTXT_D(item, fileUtil_D);
                    }
                    byte[] bytesH = fileUtil_H.Close();
                    byte[] bytesD = fileUtil_D.Close();

                    using (System.IO.Stream zipStream = zip.OpenWrite(fileUtil_H.FILENAME))
                    {
                        zipStream.Write(bytesH, 0, bytesH.Length);
                    }
                    using (System.IO.Stream zipStream = zip.OpenWrite(fileUtil_D.FILENAME))
                    {
                        zipStream.Write(bytesD, 0, bytesD.Length);
                    }

                    PX.SM.FileInfo file = new PX.SM.FileInfo(fileName + ".zip", null, zipMs.ToArray());
                    ts.Complete();
                    //檔案下載
                    //fileUtil_H.Dowload();
                    throw new PXRedirectToFileException(file, true);
                    //SetSelectedInformation();
                }
            }
        }

        public virtual void inputTXT(NMPayableCheck item, RCEleTxtFileUtil fileUtil)
        {
            //用戶序號(7)
            String seq = RCEleTxtFileUtil.GetDataStrByByte(item.PayableCheckCD, 7, false, fileUtil.ENCODING);
            //發票日期(8) - yyyyMMdd 
            //2021-02-02 Alton Mantis:11760 發票日期改抓EtdDepositDate
            //String date = RCEleTxtFileUtil.GetDataStrByByte(item.CheckDate?.ToString("yyyyMMdd"), 8, false, fileUtil.ENCODING);
            String date = RCEleTxtFileUtil.GetDataStrByByte(item.EtdDepositDate?.ToString("yyyyMMdd"), 8, false, fileUtil.ENCODING);
            //發票人帳號(17) 
            NMBankAccount ba = (NMBankAccount)PXSelectorAttribute.Select<NMPayableCheck.bankAccountID>(DetailsView.Cache, item);
            String bankAccount = RCEleTxtFileUtil.GetDataStrByByte(ba.BankAccount, 17, false, fileUtil.ENCODING);
            //金額(18) -含兩位小數 左補0
            String amount = RCEleTxtFileUtil.GetDataStrByByte(item.OriCuryAmount?.ToString("0.00").Replace(".", ""), 18, true, '0', fileUtil.ENCODING);
            //受款人(60)- 右補空白
            String title = RCEleTxtFileUtil.GetDataStrByByte(item.Title, 60, false, fileUtil.ENCODING);
            //受款人地址(100)- 右補空白
            Address ad = GetAddressByLocation(item.VendorLocationID);
            String address = (ad?.City ?? "") + (ad?.AddressLine1 ?? ad?.AddressLine2 ?? "");
            address = RCEleTxtFileUtil.GetDataStrByByte(address, 100, false, fileUtil.ENCODING);
            //禁止背書轉讓(1) - Y
            String noTransfer = "Y";
            //對帳單key值(30) - 右補空白
            String key = RCEleTxtFileUtil.GetDataStrByByte(item.RefNbr, 30, false, fileUtil.ENCODING);
            fileUtil.InputLine(
                    seq +
                    date +
                    bankAccount +
                    amount +
                    title +
                    address +
                    noTransfer +
                    key
                );
        }

        public virtual void inputTXT_D(NMPayableCheck item, RCEleTxtFileUtil fileUtil)
        {

            BAccount baccount = GetBaccount(item.VendorID);
            GVApGuiInvoiceRef gv = GetGV(item.RefNbr);
            Contract contract = GetContract(item.ProjectID);
            //廠商統編(10)
            String taxID = RCEleTxtFileUtil.GetDataStrByByte(baccount.TaxRegistrationID, 10, false, fileUtil.ENCODING);
            //到期日(8) - yyyyMMdd 
            String date = RCEleTxtFileUtil.GetDataStrByByte(item.EtdDepositDate?.ToString("yyyyMMdd"), 8, false, fileUtil.ENCODING);
            //發票日期(8) 
            String invoiceDate = RCEleTxtFileUtil.GetDataStrByByte(gv?.InvoiceDate?.ToString("yyyyMMdd"), 8, false, fileUtil.ENCODING);
            //發票號碼(10)
            String invoiceNbr = RCEleTxtFileUtil.GetDataStrByByte(gv?.GuiInvoiceNbr, 10, false, fileUtil.ENCODING);
            //發票金額(13)
            String invoiceAmt = RCEleTxtFileUtil.GetDataStrByByte(item.OriCuryAmount?.ToString("0.00").Replace(".", ""), 13, true, '0', fileUtil.ENCODING);
            //對帳單key值(11)
            String key = RCEleTxtFileUtil.GetDataStrByByte(item.RefNbr, 11, false, fileUtil.ENCODING);
            String remark = RCEleTxtFileUtil.GetDataStrByByte(contract.ContractCD, 8, false, fileUtil.ENCODING);
            fileUtil.InputLine(
                   taxID +
                   date +
                   invoiceDate +
                   invoiceNbr +
                   invoiceAmt +
                   key +
                   remark
                );
        }

        #endregion

        #region Event
        protected virtual void _(Events.RowSelected<DetailTable> e)
        {
            if (e.Row == null) return;
            DetailsView.AllowInsert = false;
            DetailsView.AllowDelete = false;
            PXUIFieldAttribute.SetEnabled(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<DetailTable.selected>(e.Cache, e.Row, true);
            PXUIFieldAttribute.SetEnabled<DetailTable.bankAccountID>(e.Cache, e.Row, true);
        }

        protected virtual void _(Events.FieldUpdated<DetailTable.selected> e)
        {
            DetailTable row = (DetailTable)e.Row;
            if (row == null) return;
            SetSelectedInformation();
        }
        #endregion

        #region Method
        private void SetSelectedInformation()
        {
            ParamTable p = MasterView.Current;
            int count = 0;
            decimal oriAmt = 0m;
            decimal basAmt = 0m;
            foreach (NMPayableCheck item in DetailsView.Select())
            {
                if (item.Selected != true) continue;
                count += 1;
                oriAmt += item.OriCuryAmount ?? 0m;
                basAmt += item.BaseCuryAmount ?? 0m;
            }
            p.SelectedCount = count;
            p.SelectedOriCuryAmount = oriAmt;
            p.SelectedBaseCuryAmount = basAmt;

        }
        #endregion

        #region BQL
        public Address GetAddressByLocation(int? locationID)
        {
            return PXSelectJoin<Address,
                InnerJoin<Location, On<Location.defAddressID, Equal<Address.addressID>>>,
            Where<Location.locationID, Equal<Required<Location.locationID>>>>
            .Select(this, locationID);
        }

        public BAccount GetBaccount(int? baccountID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(new PXGraph(), baccountID);
        }

        public GVApGuiInvoiceRef GetGV(string refNbr)
        {
            return PXSelect<GVApGuiInvoiceRef,
                Where<GVApGuiInvoiceRef.refNbr,
                Equal<Required<GVApGuiInvoiceRef.refNbr>>>>
                .Select(this, refNbr);

        }

        public Contract GetContract(int? projectID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID,
                Equal<Required<Contract.contractID>>>>
                .Select(this, projectID);

        }
        #endregion

        #region Table
        [PXHidden]
        public class ParamTable : IBqlTable
        {
            #region BranchID
            [PXInt()]
            [PXUIField(DisplayName = "Branch ID")]
            [PXDefault(
                typeof(Search<Branch.branchID,
                    Where<Branch.active, Equal<True>,
                        And<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>)
                , PersistingCheck = PXPersistingCheck.Nothing
                )]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            public virtual int? BranchID { get; set; }
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            #endregion

            #region CashierID
            [PXInt()]
            [PXUIField(DisplayName = "Cashier")]
            [PXEPEmployeeSelector]
            [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual int? CashierID { get; set; }
            public abstract class cashierID : PX.Data.BQL.BqlInt.Field<cashierID> { }
            #endregion

            #region CheckStartDate
            [PXDate()]
            [PXUIField(DisplayName = "Check Start Date")]
            public virtual DateTime? CheckStartDate { get; set; }
            public abstract class checkStartDate : PX.Data.BQL.BqlDateTime.Field<checkStartDate> { }
            #endregion

            #region CheckEndDate
            [PXDate()]
            [PXUIField(DisplayName = "Check End Date")]
            public virtual DateTime? CheckEndDate { get; set; }
            public abstract class checkEndDate : PX.Data.BQL.BqlDateTime.Field<checkEndDate> { }
            #endregion

            #region DeliverStartDate
            [PXDate()]
            [PXUIField(DisplayName = "Deliver Start Date")]
            public virtual DateTime? DeliverStartDate { get; set; }
            public abstract class deliverStartDate : PX.Data.BQL.BqlDateTime.Field<deliverStartDate> { }
            #endregion

            #region DeliverEndDate
            [PXDate()]
            [PXUIField(DisplayName = "Deliver End Date")]
            public virtual DateTime? DeliverEndDate { get; set; }
            public abstract class deliverEndDate : PX.Data.BQL.BqlDateTime.Field<deliverEndDate> { }
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

            #region VendorLocationID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor Location ID")]
            [PXSelector(typeof(Search<LocationExtAddress.locationID,
            Where<LocationExtAddress.bAccountID, Equal<Current<vendorID>>>>),
            typeof(LocationExtAddress.locationCD),
            SubstituteKey = typeof(LocationExtAddress.addressLine1))]
            public virtual int? VendorLocationID { get; set; }
            public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
            #endregion

            #region ProjectID
            //[PXInt()]
            [PXUIField(DisplayName = "Project ID")]
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region ProjectPeriodStart
            [PXInt()]
            [PXUIField(DisplayName = "Project Start Period", Visible = false)]
            public virtual int? ProjectStartPeriod { get; set; }
            public abstract class projectStartPeriod : PX.Data.BQL.BqlInt.Field<projectStartPeriod> { }
            #endregion

            #region ProjectEndPeriod
            [PXInt()]
            [PXUIField(DisplayName = "Project End Period", Visible = false)]
            public virtual int? ProjectEndPeriod { get; set; }
            public abstract class projectEndPeriod : PX.Data.BQL.BqlInt.Field<projectEndPeriod> { }
            #endregion

            #region BankCode
            [PXString(7, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Bank Code")]
            [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
            [NMBankCode]
            public virtual string BankCode { get; set; }
            public abstract class bankCode : PX.Data.BQL.BqlString.Field<bankCode> { }
            #endregion

            #region CheckStatus
            [PXInt()]
            [PXUIField(DisplayName = "Check Status")]
            [PXUnboundDefault(typeof(NMStringList.NMAPCheckStatus.unconfirm))]
            [PXIntList(
                new int[] { NMStringList.NMAPCheckStatus.UNCONFIRM, NMStringList.NMAPCheckStatus.REPRESENT },
                new string[] { "未確認", "已代開" }
                )]
            public virtual int? CheckStatus { get; set; }
            public abstract class checkStatus : PX.Data.BQL.BqlInt.Field<checkStatus> { }
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

            #region DueDate
            [PXDate()]
            [PXUIField(DisplayName = "Due Date")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>), PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
            #endregion

            #region Selected Information

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

            #region SelectedBaseCuryAmount
            [PXDecimal(4)]
            [PXUIField(DisplayName = "Selected Base Cury Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SelectedBaseCuryAmount { get; set; }
            public abstract class selectedBaseCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedBaseCuryAmount> { }
            #endregion

            #endregion


        }


        #region Detail
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(Select2<NMPayableCheck,
           InnerJoin<NMBankAccount, On<NMBankAccount.bankAccountID, Equal<NMPayableCheck.bankAccountID>>,
           InnerJoin<KGBillPayment, On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>,
           InnerJoin<APInvoice,On<APInvoice.refNbr,Equal<NMPayableCheck.refNbr>>>>>,
           Where<NMBankAccount.enableIssueByBank, Equal<True>,
               And<Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.unconfirm>,
                   Or<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.represent>>>>>>
            )
            , Persistent = false)]
        public class DetailTable : NMPayableCheck
        {
            #region BankAccountID
            [PXDBInt(BqlField = typeof(NMPayableCheck.bankAccountID))]
            [PXUIField(DisplayName = "Bank Account ID", Required = true)]
            [NMBankAccount(
                        SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                        DescriptionField = typeof(NMBankAccount.bankName)
            )]
            [PXRestrictor(typeof(Where<NMBankAccount.enableIssueByBank, Equal<True>>), "Bank Account Not Found", typeof(NMBankAccount.enableIssueByBank))]
            [PXRestrictor(typeof(Where<NMBankAccount.paymentMethodID, Equal<KGConst.check>>), "Bank Account Not Found", typeof(NMBankAccount.enableIssueByBank))]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public override int? BankAccountID { get; set; }
            #endregion

            #region BankCode
            [PXDBString(7, IsFixed = true, IsUnicode = true, InputMask = "", BqlField = typeof(NMBankAccount.bankCode))]
            [PXUIField(DisplayName = "Bank Code", Required = true)]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            [NMBankCode]
            public virtual string BankCode { get; set; }
            public abstract class bankCode : PX.Data.BQL.BqlString.Field<bankCode> { }
            #endregion

            #region NMBatchNbr
            [PXDBString(IsFixed = true, IsUnicode = true, InputMask = "", BqlField = typeof(KGBillPaymentExt.usrNMBatchNbr))]
            [PXUIField(DisplayName = "NM Batch Nbr", IsReadOnly = true)]
            public virtual string NMBatchNbr { get; set; }
            public abstract class nmBatchNbr : PX.Data.BQL.BqlString.Field<nmBatchNbr> { }
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

            #region DueDate
            [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
            [PXUIField(DisplayName = "Due Date")]
            public virtual DateTime? APDueDate { get; set; }
            public abstract class apDueDate : PX.Data.BQL.BqlDateTime.Field<apDueDate> { }
            #endregion

        }
        #endregion
        #endregion


    }
}