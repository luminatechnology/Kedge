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
                      new string[] { "��w�⥼�C", "�W����",
                                 "�ζq�W��", "����ܧ�",
                                 "�Ȥ��ܧ�", "��L" })]
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
                      new string[] { "�~�ȳ�", "���q��",
                                 "�I�u��", "���ʳ�",
                                 "�~�D(�Ȥ�)", "���q�l���B�l�[�w��", "��L�B���l�[�w��" })]
        public virtual string UsrBelongKind { get; set; }
        public abstract class usrBelongKind : PX.Data.BQL.BqlString.Field<usrBelongKind> { }
        #endregion

        #region UsrChangeSource
        [PXDBString(1)]
        [PXUIField(DisplayName = "ChangeSource")]
        [PXStringList(new string[] { "A", "B" },
                      new string[] { "���]�W��", "�ǳƪ�" })]
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