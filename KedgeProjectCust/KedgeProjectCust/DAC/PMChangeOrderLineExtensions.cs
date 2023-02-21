using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using PX.Objects.CM;

namespace PX.Objects.PM
{
    public class PMChangeOrderLineExt : PXCacheExtension<PX.Objects.PM.PMChangeOrderLine>
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

        #region UsrChangeSource
        [PXDBString(1)]
        [PXUIField(DisplayName = "ChangeSource")]
        [PXStringList(new string[] { "A", "B" },
                      new string[] { "分包規劃", "準備金" })]
        public virtual string UsrChangeSource { get; set; }
        public abstract class usrChangeSource : PX.Data.BQL.BqlString.Field<usrChangeSource> { }
        #endregion

        #region UsrRemark
        [PXDBString(240)]
        [PXUIField(DisplayName = "Remark")]

        public virtual string UsrRemark { get; set; }
        public abstract class usrRemark : PX.Data.BQL.BqlString.Field<usrRemark> { }
        #endregion

        #region UsrInventoryDesc
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "UsrInventoryDesc",Required = true)]

        public virtual string UsrInventoryDesc { get; set; }
        public abstract class usrInventoryDesc : PX.Data.BQL.BqlString.Field<usrInventoryDesc> { }
        #endregion
       
        #region UnitCost
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        //remove Base Currreny attri, add PriceCost Attri
        [PXRemoveBaseAttribute(typeof(PXDBBaseCuryAttribute))]
        [PXDBPriceCost]
        public virtual Decimal? UnitCost
        {
            get;
            set;
        }
        #endregion
    }
}