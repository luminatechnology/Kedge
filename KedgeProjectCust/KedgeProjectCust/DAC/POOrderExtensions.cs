using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.Common.Bql;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects;
using PX.SM;
using PX.TM;
using System.Collections.Generic;
using System;
using Kedge.DAC;

namespace PX.Objects.PO
{
    public class POOrderExt : PXCacheExtension<PX.Objects.PO.POOrder>
    {
        #region UsrKGFlowUID
        [PXDBString(40)]
        [PXUIField(DisplayName = "UsrKGFlowUID")]
        public virtual string UsrKGFlowUID { get; set; }
        public abstract class usrKGFlowUID : PX.Data.BQL.BqlString.Field<usrKGFlowUID> { }
        #endregion

        #region DefRetainagePct

        public abstract class defRetainagePct : PX.Data.BQL.BqlDecimal.Field<defRetainagePct> { }
        [PXDBDecimal(0, MinValue = 0, MaxValue = 100)]
        [PXDefault(TypeCode.Decimal,"0")]
        [PXUIField(DisplayName = "Retainage Percent", FieldClass = nameof(FeaturesSet.Retainage))]
        [PXFormula(typeof(Selector<POOrder.vendorID, Vendor.retainagePct>))]
        public virtual decimal? DefRetainagePct
        {
            get;
            set;
        }
        #endregion

        #region  UsrRedirect
        [PXString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "電子簽核")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String UsrRedirect
        {
            get
            {
                if (UsrReturnUrl != null)
                {
                    return "電子簽核";
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }
        public abstract class usrRedirect : PX.Data.BQL.BqlString.Field<usrRedirect> { }
        #endregion

        #region UsrReturnUrl
        [PXDBString(2048)]
        [PXUIField(DisplayName = "Return Url")]
        public virtual string UsrReturnUrl { get; set; }
        public abstract class usrReturnUrl : PX.Data.BQL.BqlString.Field<usrReturnUrl> { }
        #endregion

        #region UsrContractEvalID
        [PXString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Contract Evalution ID")]
        [PXDBScalar(typeof(Search<KGContractEvaluation.contractEvaluationCD,
                                  Where<KGContractEvaluation.orderNbr, Equal<POOrder.orderNbr>,
                                        And<KGContractEvaluation.orderType, Equal<POOrder.orderType>>>>))]
        [PXSelector(typeof(KGContractEvaluation.contractEvaluationCD))]
        public virtual string UsrContractEvalID { get; set; }
        public abstract class usrContractEvalID : PX.Data.BQL.BqlInt.Field<usrContractEvalID> { }
        #endregion

        #region UsrBillCategory
        [PXDBString(2)]
        [PXUIField(DisplayName = "BillCategory")]
        [PXStringList(new string[]
                      {
                          "1", "2", "3", "4", "5", "23", "6", "7","8","9","10",
                          "11", "12", "13", "14", "15", "16", "17","18","19","20",
                          "21", "22"
                      },
                      new string[]
                      {
                          "(零星-水電)","(零星-差旅費)", "(零星-庶務)", "(零星-薪資)", "(零星-雜支)","(零星-雜支二)",
                          "水電工程","玄關門工程", "石材工程", "防火門工程", "防火玻璃工程",
                          "泥作工程","油漆工程", "混凝土壓送", "連續壁", "搭吊工程",
                          "預拌混凝土材料","磁磚材料", "輕隔間工程", "模板工程", "鋼筋材料",
                          "鋼筋綁紮","鷹架工程"
        })]
        public virtual string UsrBillCategory { get; set; }
        public abstract class usrBillCategory : PX.Data.BQL.BqlString.Field<usrBillCategory> { }
        #endregion
    }
}
