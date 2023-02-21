using PX.Data;
using System;

// UsrRemark add  IsUnicode = true by louis 20210401

namespace PX.Objects.PM
{
    public class PMChangeOrderBudgetExt : PXCacheExtension<PX.Objects.PM.PMChangeOrderBudget>
    {
        #region UsrChangeReason
        [PXDBString(1)]
        [PXUIField(DisplayName = "ChangeReason", Required = true)]
        /*[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]*/
        [PXStringList(new string[] { "1", "2", "3", "4", "5", "6" },
                     new string[] { "��w�⥼�C", "�W����",
                                "�ζq�W��", "����ܧ�",
                                "�Ȥ��ܧ�", "��L" })]
        public virtual string UsrChangeReason { get; set; }
        public abstract class usrChangeReason : PX.Data.BQL.BqlString.Field<usrChangeReason> { }
        #endregion

        #region UsrChangeType
        [PXDBString(1)]
        [PXUIField(DisplayName = "ChangeType")]
        [PXStringList(new string[] { "A", "B" }, new string[] { "��w��l�[", "�s�P�l�[" })]
        public virtual string UsrChangeType { get; set; }
        public abstract class usrChangeType : PX.Data.BQL.BqlString.Field<usrChangeType> { }
        #endregion

        #region UsrChangePeriod
        [PXDBInt]
        [PXUIField(DisplayName = "ChangePeriod")]

        public virtual int? UsrChangePeriod { get; set; }
        public abstract class usrChangePeriod : PX.Data.BQL.BqlInt.Field<usrChangePeriod> { }
        #endregion

        #region UsrBelongKind
        [PXDBString(1)]
        [PXUIField(DisplayName = "BelongKind", Required = true)]
        /*[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]*/
        [PXStringList(new string[] { "A", "B", "C", "D", "E", "F", "G" },
                      new string[] { "�~�ȳ�", "���q��",
                                 "�I�u��", "���ʳ�",
                                 "�~�D(�Ȥ�)", "���q�l���B�l�[�w��", "��L�B���l�[�w��" })]
        public virtual string UsrBelongKind { get; set; }
        public abstract class usrBelongKind : PX.Data.BQL.BqlString.Field<usrBelongKind> { }
        #endregion

        #region UsrIsApprovedByCustomer
        [PXDBBool]
        [PXUIField(DisplayName = "IsApprovedByCustomer")]

        public virtual bool? UsrIsApprovedByCustomer { get; set; }
        public abstract class usrIsApprovedByCustomer : PX.Data.BQL.BqlBool.Field<usrIsApprovedByCustomer> { }
        #endregion

        #region UsrRemark
        [PXDBString(240, IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]

        public virtual string UsrRemark { get; set; }
        public abstract class usrRemark : PX.Data.BQL.BqlString.Field<usrRemark> { }
        #endregion

        #region UsrInventoryDesc
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "UsrInventoryDesc")]
        public virtual String UsrInventoryDesc { get; set; }
        public abstract class usrInventoryDesc : IBqlField { }
        #endregion

        #region UsrIsFreeItem
        [PXDBBool()]
        [PXUIField(DisplayName = "UsrIsFreeItem")]
        [PXDefault(false,PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsFreeItem { get; set; }
        public abstract class usrIsFreeItem : IBqlField { }
        #endregion
    }
}