using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.PO;
using Kedge.DAC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.AP
{
    /**
     * === 2021/11/18 : 0012267 === Althea
     * Add Fields: 
     * UsrIsTmpPayment bool for Fin TempPayment
     * UsrTmpPaymentTotal unbound for show (APInvoice.CuryLineTotal + APInvoice.CuryTaxTotal)
     * UsrTmpPaymentReleased unbound for show (Sum(APTran.UsrTmpPaymentReleased))
     * UsrTmpPaymentUnReleased unbound for show (UsrTmpPaymentTotal - UsrTmpPaymentReleased)
     **/
    public class APRegisterExt : PXCacheExtension<PX.Objects.AP.APRegister>
    {
        #region UsrValuationPhase
        [PXDBInt]
        [PXUIField(DisplayName = "Valuation Phase")]
        public virtual int? UsrValuationPhase { get; set; }
        public abstract class usrValuationPhase : PX.Data.BQL.BqlInt.Field<usrValuationPhase> { }
        #endregion

        #region UsrRevDeductionNbr
        [PXDBString]
        [PXUIField(DisplayName = "RevDeduction Nbr")]

        public virtual String UsrRevDeductionNbr { get; set; }
        public abstract class usrRevDeductionNbr : IBqlField { }
        #endregion

        #region UsrBillDateFrom
        [PXDBDate]
        [PXUIField(DisplayName = "Bill Date From")]
        public virtual DateTime? UsrBillDateFrom { get; set; }
        public abstract class usrBillDateFrom : PX.Data.BQL.BqlDateTime.Field<usrBillDateFrom> { }
        #endregion

        #region UsrBillDateTo
        [PXDBDate]
        [PXUIField(DisplayName = "Bill Date To")]
        public virtual DateTime? UsrBillDateTo { get; set; }
        public abstract class usrBillDateTo : PX.Data.BQL.BqlDateTime.Field<usrBillDateTo> { }
        #endregion

        #region UsrVoucherKey
        [PXDBString(11)]
        [PXUIField(DisplayName = "Voucher Key")]

        public virtual string UsrVoucherKey { get; set; }
        public abstract class usrVoucherKey : PX.Data.BQL.BqlString.Field<usrVoucherKey> { }
        #endregion

        #region UsrVoucherNo
        [PXDBString(11)]
        [PXUIField(DisplayName = "VoucherNo")]

        public virtual string UsrVoucherNo { get; set; }
        public abstract class usrVoucherNo : PX.Data.BQL.BqlString.Field<usrVoucherNo> { }
        #endregion

        #region UsrVoucherDate
        [PXDBDate]
        [PXUIField(DisplayName = "VoucherDate")]

        public virtual DateTime? UsrVoucherDate { get; set; }
        public abstract class usrVoucherDate : PX.Data.BQL.BqlDateTime.Field<usrVoucherDate> { }
        #endregion

        #region UsrVoucherClass
        [PXDBString(60)]
        [PXUIField(DisplayName = "VoucherClass")]

        public virtual string UsrVoucherClass { get; set; }
        public abstract class usrVoucherClass : PX.Data.BQL.BqlString.Field<usrVoucherClass> { }
        #endregion

        #region UsrPONbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "PO Nbr.", Visible = false)]
        [PXSelector(typeof(Search<POOrder.orderNbr>),
                    typeof(POOrder.orderNbr),
                    typeof(POOrder.orderDesc))]
        public virtual string UsrPONbr { get; set; }
        public abstract class usrPONbr : PX.Data.BQL.BqlString.Field<usrPONbr> { }
        #endregion

        #region UsrPOOrderType
        [PXDBString(15)]
        [PXUIField(DisplayName = "POOrderType")]

        public virtual string UsrPOOrderType { get; set; }
        public abstract class usrPOOrderType : PX.Data.BQL.BqlString.Field<usrPOOrderType> { }
        #endregion

        #region UsrIsOverdueSubmit
        [PXDBBool]
        [PXUIField(DisplayName = "IsOverdueSubmit")]

        public virtual bool? UsrIsOverdueSubmit { get; set; }
        public abstract class usrIsOverdueSubmit : PX.Data.BQL.BqlBool.Field<usrIsOverdueSubmit> { }
        #endregion

        #region  UsrRedirect
        [PXString(50, IsUnicode = true)]
        //2021-09-15:12234 visable = false
        [PXUIField(DisplayName = "電子簽核",Visible = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String UsrRedirect
        {
            get
            {
                if (Base.Hold == false && UsrReturnUrl != null)
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
        [PXUIField(DisplayName = "ReturnUrl")]

        public virtual string UsrReturnUrl { get; set; }
        public abstract class usrReturnUrl : PX.Data.BQL.BqlString.Field<usrReturnUrl> { }
        #endregion

        #region UsrFlowStatus
        [PXString(1)]
        [PXUIField(DisplayName = "FlowStatus", Enabled = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXStringList(
                    new string[]
                    {
                      "A","B","C", "D"

                    },
                    new string[]
                    {
                       "未送審","送審中","核可中", "已核可"
        })]
        public virtual string UsrFlowStatus
        {
            get
            {
                if (Base.Hold == true)
                {
                    return "A";
                }
                else if ("63000".Equals(Stateuid))
                {
                    return "D";
                }
                else if ("62500".Equals(Stateuid))
                {
                    return "C";
                }
                else if ("62000".Equals(Stateuid))
                {
                    return "B";
                }
                else
                {
                    return "A";
                }
            }
            set
            {

            }
        }
        public abstract class usrFlowStatus : PX.Data.BQL.BqlString.Field<usrFlowStatus> { }
        #endregion

        #region Stateuid
        [PXString(5, IsUnicode = true, InputMask = "")]
        [PXDBScalar(
            typeof(Search<KGFlowSubAcc.stateuid,
            Where<KGFlowSubAcc.accUID, Equal<APRegisterExt.usrKGFlowUID>>>)
        )]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Stateuid")]

        public virtual string Stateuid { get; set; }
        public abstract class stateuid : PX.Data.BQL.BqlString.Field<stateuid> { }
        #endregion

        #region CuryRetainageUnpaidTotal
        public abstract class usrCuryRetainageUnpaidTotal : PX.Data.BQL.BqlDecimal.Field<usrCuryRetainageUnpaidTotal> { }

        [PXDecimal(0)]
        [PXUIField(DisplayName = "Unpaid Retainage")]
        [PXFormula(typeof(Sub<APRegister.curyRetainageTotal, APRegister.curyRetainageReleased>))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrCuryRetainageUnpaidTotal
        {
            get;
            set;
        }
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
                          "(零星-水電)","(零星-差旅費)", "(零星-庶務)", "(零星-薪資)", "(零星-雜支)", "(零星-雜支二)",
                          "水電工程","玄關門工程", "石材工程", "防火門工程", "防火玻璃工程",
                          "泥作工程","油漆工程", "混凝土壓送", "連續壁", "搭吊工程",
                          "預拌混凝土材料","磁磚材料", "輕隔間工程", "模板工程", "鋼筋材料",
                          "鋼筋綁紮","鷹架工程"
        })]
        public virtual string UsrBillCategory { get; set; }
        public abstract class usrBillCategory : PX.Data.BQL.BqlString.Field<usrBillCategory> { }
        #endregion

        #region UsrKGFlowUID
        [PXDBString(40)]
        [PXUIField(DisplayName = "UsrKGFlowUID")]
        public virtual string UsrKGFlowUID { get; set; }
        public abstract class usrKGFlowUID : PX.Data.BQL.BqlString.Field<usrKGFlowUID> { }
        #endregion

        #region UsrIsDeductionDoc
        [PXDBBool]
        [PXUIField(DisplayName = "IsDeductionDoc")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual bool? UsrIsDeductionDoc { get; set; }
        public abstract class usrIsDeductionDoc : PX.Data.BQL.BqlBool.Field<usrIsDeductionDoc> { }
        #endregion

        #region UsrOriDeductionRefNbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "OriDeductionRefNbr")]
        public virtual string UsrOriDeductionRefNbr { get; set; }
        public abstract class usrOriDeductionRefNbr : PX.Data.BQL.BqlString.Field<usrOriDeductionRefNbr> { }
        #endregion

        #region UsrOriDeductionDocType
        [PXDBString(3)]
        [PXUIField(DisplayName = "OriDeductionDocType", Visible = false)]
        public virtual string UsrOriDeductionDocType { get; set; }
        public abstract class usrOriDeductionDocType : PX.Data.BQL.BqlString.Field<usrOriDeductionDocType> { }
        #endregion

        #region UsrSkipFlowFlag
        [PXDBBool]
        [PXUIField(DisplayName = "Skip Flow")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrSkipFlowFlag { get; set; }
        public abstract class usrSkipFlowFlag : PX.Data.BQL.BqlBool.Field<usrSkipFlowFlag> { }
        #endregion

        #region UsrIsComsData
        [PXDBBool]
        [PXUIField(DisplayName = "Is Coms Data")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsComsData { get; set; }
        public abstract class usrIsComsData : PX.Data.BQL.BqlBool.Field<usrIsComsData> { }
        #endregion

        //2021/11/10 Mantis:0012266
        #region UsrIsTmpPayment
        [PXDBBool]
        [PXUIField(DisplayName = "Is Tmp Payment")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsTmpPayment { get; set; }
        public abstract class usrIsTmpPayment : PX.Data.BQL.BqlBool.Field<usrIsTmpPayment> { }
        #endregion

        //2021/11/10 Mantis:0012266
        #region UsrTmpPaymentTotal
        [PXDecimal]
        [PXUIField(DisplayName = "UsrTmpPaymentTotal",IsReadOnly = true)]
        [PXUnboundDefault]

        public virtual Decimal? UsrTmpPaymentTotal { get; set; }
        public abstract class usrTmpPaymentTotal : PX.Data.BQL.BqlDecimal.Field<usrTmpPaymentTotal> { }
        #endregion

        //2021/11/10 Mantis:0012266
        #region UsrTmpPaymentReleased
        [PXDecimal]
        [PXUIField(DisplayName = "UsrTmpPaymentReleased", IsReadOnly = true)]
        [PXUnboundDefault]
        public virtual Decimal? UsrTmpPaymentReleased { get; set; }
        public abstract class usrTmpPaymentReleased : PX.Data.BQL.BqlDecimal.Field<usrTmpPaymentReleased> { }
        #endregion

        //2021/11/10 Mantis:0012266
        #region UsrTmpPaymentUnReleased
        [PXDecimal]
        [PXUIField(DisplayName = "UsrTmpPaymentUnReleased", IsReadOnly = true)]
        [PXUnboundDefault]

        public virtual Decimal? UsrTmpPaymentUnReleased { get; set; }
        public abstract class usrTmpPaymentUnReleased : PX.Data.BQL.BqlDecimal.Field<usrTmpPaymentUnReleased> { }
        #endregion

        //2022/03/21 計價單保留款Tab
        //保留款比例
        #region UsrRetainagePct
        [PXDBDecimal(0, MinValue = 0, MaxValue = 100)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Retainage Percent", FieldClass = nameof(FeaturesSet.Retainage), IsReadOnly = true)]
        public virtual decimal? UsrRetainagePct { get; set; }
        public abstract class usrRetainagePct : PX.Data.BQL.BqlDecimal.Field<usrRetainagePct> { }
        #endregion

        //保留款金額
        #region UsrRetainageAmt
        [PXDBDecimal]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Retainage Amount")]
        public virtual decimal? UsrRetainageAmt { get; set; }
        public abstract class usrRetainageAmt : PX.Data.BQL.BqlDecimal.Field<usrRetainageAmt> { }
        #endregion

        //是否為啟用保留款，預設PO中計價參數設定 (KGContractPricingRules) RetainageApply
        #region UsrIsRetainageApply
        [PXDBBool]
        [PXUIField(DisplayName = "Is Retainage Applied")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsRetainageApply { get; set; }
        public abstract class usrIsRetainageApply : PX.Data.BQL.BqlBool.Field<usrIsRetainageApply> { }
        #endregion

        //是否為保留款退回文件
        #region UsrIsRetainageDoc
        [PXDBBool]
        [PXUIField(DisplayName = "Is Retainage Doc", Enabled = false)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsRetainageDoc { get; set; }
        public abstract class usrIsRetainageDoc : PX.Data.BQL.BqlBool.Field<usrIsRetainageDoc> { }
        #endregion

        #region UsrRetainageHistType
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Retainage History Type")]
        [RetHistType.List()]
        public virtual string UsrRetainageHistType { get; set; }
        public abstract class usrRetainageHistType : PX.Data.BQL.BqlString.Field<usrRetainageHistType> { }
        #endregion

        #region Unbound Fields

        #region UsrTotalRetainageAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "Billed Amount", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [CalcBilledTranAmt]
        public virtual decimal? UsrTotalRetainageAmt { get; set; }
        public abstract class usrTotalRetainageAmt : PX.Data.BQL.BqlDecimal.Field<usrTotalRetainageAmt> { }
        #endregion

        //已退回保留款金額
        #region UsrRetainageReleased
        [PXDecimal()]
        [PXUIField(DisplayName = "Retainage Released", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrRetainageReleased
        {
            [PXDependsOnFields(typeof(APRegister.docType), typeof(APRegister.refNbr), typeof(APRegisterExt.usrIsRetainageApply))]
            get
            {
                return APInvoiceEntry_Extension.CalcRetainageReleasedAmt(this.UsrIsRetainageApply, Base.DocType, Base.RefNbr);
            }
        }
        public abstract class usrRetainageReleased : PX.Data.BQL.BqlDecimal.Field<usrRetainageReleased> { }
        #endregion

        #region UsrRetainageUnreleasedAmt
        [PXDecimal]
        [PXUIField(DisplayName = "Retainage Unreleased", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Sub<usrRetainageAmt, usrRetainageReleased>))]
        public virtual decimal? UsrRetainageUnreleasedAmt { get; set; }
        public abstract class usrRetainageUnreleasedAmt : PX.Data.BQL.BqlDecimal.Field<usrRetainageUnreleasedAmt> { }
        #endregion

        #endregion
    }

    #region Custom Attributes
    public class CalcBilledTranAmtAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
    {
        public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if ((sender.Current as APInvoice) == null) { return; }

            decimal? billedTotal = 0m;

            ///<remarks> Mantis [0012321] #1</remarks>
            foreach (APInvoice row in sender.Cached)
            {
                // Since bill invoice has prepayment application, adding the condition only calculate current record.
                if (row.RefNbr != (sender.Current as APInvoice).RefNbr) { continue; }

                billedTotal = (row.CuryTaxTotal ?? 0m);

                List<APTran> trans = sender.Graph.Caches[typeof(APTran)].Cached.RowCast<APTran>().ToList();

                trans.RemoveAll(r => r.GetExtension<APTranExt>().UsrValuationType != ValuationTypeStringList.B || r.RefNbr != row.RefNbr);

                billedTotal += trans.Sum(s => s.CuryTranAmt);

                if (trans.Count <= 0)
                {
                    billedTotal += SelectFrom<APTran>.Where<APTran.tranType.IsEqual<@P.AsString>
                                                            .And<APTran.refNbr.IsEqual<@P.AsString>
                                                                 .And<APTranExt.usrValuationType.IsEqual<ValuationTypeStringList.b>>>>
                                                     .AggregateTo<Sum<APTran.curyTranAmt>>.View.Select(sender.Graph, row.DocType, row.RefNbr).TopFirst?.CuryTranAmt ?? 0m;
                }
            }

            e.ReturnValue = billedTotal;
            sender.SetValue<APRegisterExt.usrTotalRetainageAmt>(sender.Current, billedTotal);
        }
    }

    public class RetHistType
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(new[]
                                          {
                                              Pair(Original, nameof(Original)),
                                              Pair(History, nameof(History)),
                                          }) { }
        }

        public const string Original = "O";
        public const string History = "H";

        public class original : PX.Data.BQL.BqlString.Constant<original>
        {
            public original() : base(Original) { }
        }
        public class history : PX.Data.BQL.BqlString.Constant<history>
        {
            public history() : base(History) { }
        }
    }
    #endregion
}