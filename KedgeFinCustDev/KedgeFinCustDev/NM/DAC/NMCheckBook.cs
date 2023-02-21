using System;
using NM.Util;
using PX.Data;
using PX.Data.BQL;
using static NM.Util.NMStringList;
using CS = PX.Objects.CS;

namespace NM.DAC
{
    [Serializable]
    public class NMCheckBook : IBqlTable
    {
        #region SetUp
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("CHECKBOOK", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Setup")]
        [PXSelector(typeof(CS.Numbering.numberingID))]
        public virtual string Setup { get; set; }
        #endregion

        #region BookID
        [PXDBIdentity()]
        public virtual int? BookID { get; set; }
        public abstract class bookID : PX.Data.BQL.BqlInt.Field<bookID> { }
        #endregion

        #region BookCD
        [PXDBString(50, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Book CD", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search<bookCD>), typeof(bookCD),typeof(description))]
        [CS.AutoNumber(typeof(Search<CS.Numbering.numberingID,
            Where<CS.Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        public virtual string BookCD { get; set; }
        public abstract class bookCD : PX.Data.BQL.BqlString.Field<bookCD> { }
        #endregion

        #region Description
        [PXDBString(255, IsUnicode = true, InputMask = "" )]
        [PXUIField(DisplayName = "Description" )]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region CheckWord
        [PXDBString(5, InputMask = ">LLLLL")]
        [PXUIField(DisplayName = "Check Word",Required = true)]
        public virtual string CheckWord { get; set; }
        public abstract class checkWord : PX.Data.BQL.BqlString.Field<checkWord> { }
        #endregion

        #region StartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Start Date", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? StartDate { get; set; }
        public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
        #endregion

        #region StartCheckNbr
        [PXDBString(12, IsFixed = true, IsUnicode = true, InputMask = ">000000000000")]
        [PXUIField(DisplayName = "Start Check Nbr", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string StartCheckNbr { get; set; }
        public abstract class startCheckNbr : PX.Data.BQL.BqlString.Field<startCheckNbr> { }
        #endregion

        #region EndCheckNbr
        [PXDBString(12, IsFixed = true, IsUnicode = true, InputMask = ">000000000000")]
        [PXUIField(DisplayName = "End Check Nbr", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string EndCheckNbr { get; set; }
        public abstract class endCheckNbr : PX.Data.BQL.BqlString.Field<endCheckNbr> { }
        #endregion

        #region CurrentCheckNbr
        [PXDBString(12, IsFixed = true, IsUnicode = true, InputMask = ">000000000000")]
        [PXUIField(DisplayName = "Current Check Nbr", IsReadOnly = true)]
        public virtual string CurrentCheckNbr { get; set; }
        public abstract class currentCheckNbr : PX.Data.BQL.BqlString.Field<currentCheckNbr> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account ID", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        //[PXSelector(typeof(Search<NMBankAccount.bankAccountID, Where<NMBankAccount.isActive, Equal<True>>>),
        //            typeof(NMBankAccount.bankAccountCD),
        //            typeof(NMBankAccount.bankName),
        //            typeof(NMBankAccount.bankShortName),
        //            typeof(NMBankAccount.bankCode),
        //            typeof(NMBankAccount.bankAccount),
        //            SubstituteKey = typeof(NMBankAccount.bankAccountCD),
        //            DescriptionField = typeof(NMBankAccount.bankName)
        //)]
        [NMBankAccount()]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        [PXDefault(typeof(AccessInfo.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region unbound

        #region BankAccount
        [PXString()]
        [PXUIField(DisplayName = "Bank Account", Enabled =false)]
        public virtual string BankAccount { get; set; }
        public abstract class bankAccount : PX.Data.BQL.BqlString.Field<bankAccount> { }
        #endregion

        #endregion

        //2021/01/07 Althea Add Mantis:0011868
        #region BookUsage
        [PXDBString(2)]
        [PXUIField(DisplayName = "Book Usage",Required = true)]
        [PXDefault(NMBookUsage.CHECK,PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [NMBookUsage]
        public virtual string BookUsage { get; set; }
        public abstract class bookUsage : PX.Data.BQL.BqlString.Field<bookUsage> { }
        #endregion

    }
}