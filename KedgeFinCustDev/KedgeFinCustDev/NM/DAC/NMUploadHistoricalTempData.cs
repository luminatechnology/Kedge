using System;
using Kedge.DAC;
using NM.Util;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.PM;

namespace NM.DAC
{
    [Serializable]
    [PXCacheName("NMUploadHistoricalTempData")]
    public class NMUploadHistoricalTempData : IBqlTable
    {
        #region TempSeq
        [PXDBIdentity(IsKey = true)]
        public virtual int? TempSeq { get; set; }
        public abstract class tempSeq : PX.Data.BQL.BqlInt.Field<tempSeq> { }
        #endregion

        #region Error
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error")]
        public virtual string Error { get; set; }
        public abstract class error : PX.Data.BQL.BqlString.Field<error> { }
        #endregion

        #region PayableCheckCD
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payable Check CD")]
        public virtual string PayableCheckCD { get; set; }
        public abstract class payableCheckCD : PX.Data.BQL.BqlString.Field<payableCheckCD> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account ID")]
        [NMBankAccount(
                    SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                    DescriptionField = typeof(NMBankAccount.bankNameByIssueBank)
        )]
        [PXRestrictor(typeof(Where<NMBankAccount.paymentMethodID, Equal<word.check>>), "銀行{0}的付款方式須為CHECK", typeof(NMBankAccount.bankAccountCD))]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region CheckNbr
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Nbr")]
        public virtual string CheckNbr { get; set; }
        public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Check Date")]
        public virtual DateTime? CheckDate { get; set; }
        public abstract class checkDate : PX.Data.BQL.BqlDateTime.Field<checkDate> { }
        #endregion

        #region EtdDepositDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Etd Deposit Date")]
        public virtual DateTime? EtdDepositDate { get; set; }
        public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
        #endregion

        #region PayableCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Payable Cashier ID", Required = true)]
        [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXEPEmployeeSelector]
        public virtual int? PayableCashierID { get; set; }
        public abstract class payableCashierID : PX.Data.BQL.BqlInt.Field<payableCashierID> { }
        #endregion

        #region OriCuryAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ori Cury Amount")]
        public virtual Decimal? OriCuryAmount { get; set; }
        public abstract class oriCuryAmount : PX.Data.BQL.BqlDecimal.Field<oriCuryAmount> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor ID")]
        [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>,
                Or<BAccount2.type, Equal<BAccountType.combinedType>,
                Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>>),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(BAccount2.status),
            typeof(BAccount2.defAddressID),
            typeof(BAccount2.defContactID),
            typeof(BAccount2.defLocationID),
            SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorLocationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor Location ID")]
        [PXSelector(typeof(Search<Location.locationID,
            Where<Location.bAccountID, Equal<Current<vendorID>>>>),
            typeof(Location.locationCD),
            typeof(Location.descr),
            SubstituteKey = typeof(Location.locationCD))]
        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion

        #region CheckTitle
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Title")]
        public virtual string CheckTitle { get; set; }
        public abstract class checkTitle : PX.Data.BQL.BqlString.Field<checkTitle> { }
        #endregion

        #region ProjectID
        [ProjectBase()]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        //[PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region ProjectPeriod
        [PXDBInt()]
        [PXUIField(DisplayName = "Project Period")]
        public virtual int? ProjectPeriod { get; set; }
        public abstract class projectPeriod : PX.Data.BQL.BqlInt.Field<projectPeriod> { }
        #endregion

        #region Description
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion
    }
}