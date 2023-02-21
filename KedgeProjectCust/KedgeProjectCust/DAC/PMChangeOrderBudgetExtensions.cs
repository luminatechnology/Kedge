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
                     new string[] { "原預算未列", "規格更改",
                                "用量增減", "單價變更",
                                "客戶變更", "其他" })]
        public virtual string UsrChangeReason { get; set; }
        public abstract class usrChangeReason : PX.Data.BQL.BqlString.Field<usrChangeReason> { }
        #endregion

        #region UsrChangeType
        [PXDBString(1)]
        [PXUIField(DisplayName = "ChangeType")]
        [PXStringList(new string[] { "A", "B" }, new string[] { "原預算追加", "零星追加" })]
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
                      new string[] { "業務部", "機電部",
                                 "施工部", "採購部",
                                 "業主(客戶)", "公司吸收且追加預算", "其他且不追加預算" })]
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