using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Util
{
    /*
     * === 2021/04/20  :0012029 === Althea
     * Add EPDocType: GLB ,"傳票作業"
     * 
     * ===2021/05/17 :Louis口頭 === Althea
     * 暫時把EPDocType :GLB拿掉
     * 
     * ===2021/06/15 :0012094 ===Althea
     * Filter Add : ValuationType = 3(票貼)不需要廠商確認
     * 
     * ===2021/07/07 :0012130 ===Althea
     * Add EPUsrApprovalStatus 
     * 
     * ===2021/07/15 :0012155 === Althea
     * Add EPDocType : GLB ,"傳票作業"
     * 
     * ===2021/07/15 :0012156 === Althea
     * OTH 改成其他申請
     * Add CHG 其他收付款變更
     * 
     * ===2021/10/18 : 0012256 === Althea
     * Add EPDocType : BTN , "金融交易" 
     */
    public class EPStringList
    {
        #region EP VoucherType
        public class EPVoucherType : PXStringListAttribute
        {
            public EPVoucherType() : base(Values, Labels) { }
            public static readonly string[] Values =
                {
                    A,G,S
                };
            public static readonly string[] Labels =
                {
                     "一般（收據）","發票（折抵進項）","薪工（報所得扣繳）"
                 };
            /// <summary>
            /// 一般（收據）
            /// </summary>
            public const string A = "A";
            /// <summary>
            /// 發票（折抵進項）
            /// </summary>
            public const string G = "G";
            /// <summary>
            /// 薪工（報所得扣繳）
            /// </summary>
            public const string S = "S";

            /// <summary>
            /// 一般（收據）
            /// </summary>
            public class a : PX.Data.BQL.BqlString.Constant<a> { public a() : base(A) {; } }

            /// <summary>
            /// 發票（折抵進項）
            /// </summary>
            public class g : PX.Data.BQL.BqlString.Constant<g> { public g() : base(G) {; } }

            /// <summary>
            /// 薪工（報所得扣繳）
            /// </summary>
            public class s : PX.Data.BQL.BqlString.Constant<s> { public s() : base(S) {; } }

           
        }
        #endregion

        #region EP DocType
        public class EPDocType : PXStringListAttribute
        {
            public EPDocType() : base(Values, Labels) { }
            public static readonly string[] Values =
        {
            STD,GUR,RGU,CHG,GLB,OTH,BTN
        };
            public static readonly string[] Labels =
            {
            "一般請款","應付保證票","繳回應收保證票","其他收付款變更","傳票作業","其他申請","金融交易"
        };
            /// <summary>
            /// 一般請款
            /// </summary>
            public const string STD = "STD";
            /// <summary>
            /// 保證票
            /// </summary>
            public const string GUR = "GUR";
            /// <summary>
            /// 退回應收保證票
            /// </summary>
            public const string RGU = "RGU";
            /// <summary>
            /// 其他收付款變更
            /// </summary>
            public const string CHG = "CHG";
            /// <summary>
            /// 傳票作業
            /// </summary>
            public const string GLB = "GLB";
            /// <summary>
            /// 其他申請
            /// </summary>
            public const string OTH = "OTH";
            /// <summary>
            /// 金融交易
            /// </summary>
            public const string BTN = "BTN";

            /// <summary>
            /// 一般請款
            /// </summary>
            public class std : PX.Data.BQL.BqlString.Constant<std> { public std() : base(STD) {; } }

            /// <summary>
            /// 保證票
            /// </summary>
            public class gur : PX.Data.BQL.BqlString.Constant<gur> { public gur() : base(GUR) {; } }

            /// <summary>
            /// 退回應收保證票
            /// </summary>
            public class rgu : PX.Data.BQL.BqlString.Constant<rgu> { public rgu() : base(RGU) {; } }

            //2021/07/15  Mantis: 0012156 OTH -->CHG
            /// <summary>
            /// 其他收付款變更
            /// </summary>
            public class chg : PX.Data.BQL.BqlString.Constant<chg> { public chg() : base(CHG) {; } }

            //2021/04/20 Add Mantis : 0012029
            /// <summary>
            /// 傳票作業
            /// </summary>
            public class glb : PX.Data.BQL.BqlString.Constant<glb> { public glb() : base(GLB) {; } }

            //2021/07/15  Mantis: 0012156 OTH -->其他申請
            /// <summary>
            /// 其他申請
            /// </summary>
            public class oth : PX.Data.BQL.BqlString.Constant<oth> { public oth() : base(OTH) {; } }

            //2021/10/18  Add Mantis: 0012256 
            /// <summary>
            /// 金融交易
            /// </summary>
            public class btn : PX.Data.BQL.BqlString.Constant<btn> { public btn() : base(BTN) {; } }

        }
        #endregion

        #region EP Usr Approval Status
        public class EPUsrApprovalStatus : PXStringListAttribute
        {
            public EPUsrApprovalStatus() : base(Values, Labels) { }
            public static readonly string[] Values =
                {
                    Hold,PendingApproval,Approved,AcctApproved,Rejected,Released
                };
            public static readonly string[] Labels =
                {
                     "擱置","待核准","已核准","特許核准","已拒絕","已放行"
                 };
            /// <summary>
            /// 擱置
            /// </summary>
            public const string Hold = "HL";
            /// <summary>
            /// 待核准
            /// </summary>
            public const string PendingApproval = "PA";
            /// <summary>
            /// 已核准
            /// </summary>
            public const string Approved = "AP";
            /// 特許核准
            /// </summary>
            public const string AcctApproved = "AA";
            /// <summary>
            /// 已拒絕
            /// </summary>
            public const string Rejected = "RJ";
            /// <summary>
            /// 已放行
            /// </summary>
            public const string Released = "RL";

            /// <summary>
            /// 擱置
            /// </summary>
            public class hold : PX.Data.BQL.BqlString.Constant<hold> { public hold() : base(Hold) {; } }

            /// <summary>
            /// 待核准
            /// </summary>
            public class pendingApproval : PX.Data.BQL.BqlString.Constant<pendingApproval> { public pendingApproval() : base(PendingApproval) {; } }

            /// <summary>
            /// 已核准
            /// </summary>
            public class approved : PX.Data.BQL.BqlString.Constant<approved> { public approved() : base(Approved) {; } }
            /// <summary>
            /// 特許核准
            /// </summary>
            public class acctApproved : PX.Data.BQL.BqlString.Constant<acctApproved> { public acctApproved() : base(AcctApproved) {; } }
            /// <summary>
            /// 已拒絕
            /// </summary>
            public class rejected : PX.Data.BQL.BqlString.Constant<rejected> { public rejected() : base(Rejected) {; } }
            /// <summary>
            /// 已放行
            /// </summary>
            public class released : PX.Data.BQL.BqlString.Constant<released> { public released() : base(Released) {; } }
        }
        #endregion

        #region Payment Type For OTH
        public class PaymentType : PXStringListAttribute
        {
            public PaymentType() : base(Values, Labels) { }
            public static readonly string[] Values =
        {
            NMR
           ,NMP
           ,CCR
           ,CCP
           ,TT
        };
            public static readonly string[] Labels =
            {
            "應收票據","應付票據","應收保證票","應付保證票","電匯"
        };
            /// <summary>
            /// 應收票據
            /// </summary>
            public const string NMR = "NMR";
            /// <summary>
            ///  應付票據
            /// </summary>
            public const string NMP = "NMP";
            /// <summary>
            /// 應收保證票
            /// </summary>
            public const string CCR = "CCR";
            /// <summary>
            /// 應付保證票
            /// </summary>
            public const string CCP = "CCP";
            /// <summary>
            /// 電匯
            /// </summary>
            public const string TT = "TT";

            /// <summary>
            /// 應付票據
            /// </summary>
            public class nmr : PX.Data.BQL.BqlString.Constant<nmr> { public nmr() : base(NMR) {; } }

            /// <summary>
            /// 應收票據
            /// </summary>
            public class nmp : PX.Data.BQL.BqlString.Constant<nmp> { public nmp() : base(NMP) {; } }

            /// <summary>
            /// 應收保證票
            /// </summary>
            public class ccr : PX.Data.BQL.BqlString.Constant<ccr> { public ccr() : base(CCR) {; } }

            /// <summary>
            /// 應付保證票
            /// </summary>
            public class ccp : PX.Data.BQL.BqlString.Constant<ccp> { public ccp() : base(CCP) {; } }

            /// <summary>
            /// 電匯
            /// </summary>
            public class tt : PX.Data.BQL.BqlString.Constant<tt> { public tt() : base(TT) {; } }

        }
        #endregion

        #region Modify Type For OTH
        public class ModifyType : PXStringListAttribute
        {
            public ModifyType(Tuple<string, string>[] tuple) : base(tuple) { }
            public ModifyType() : base(Values, Labels) { }
            public static readonly string[] Values =
                {
                 DAT
                ,PMT
                ,TIT
                ,REV
                ,OTH
            };
            public static readonly string[] Labels =
                {
                     "票期變更"
                    ,"付款方式變更"
                    ,"抬頭變更"
                    ,"作廢、退撤票"
                    ,"其他"
            };


            /// <summary>
            /// 票期變更
            /// </summary>
            public const string DAT = "DAT";
            /// <summary>
            ///  付款方式變更
            /// </summary>
            public const string PMT = "PMT";
            /// <summary>
            /// 抬頭變更
            /// </summary>
            public const string TIT = "TIT";
            /// <summary>
            /// 作廢、退撤票
            /// </summary>
            public const string REV = "REV";

            /// <summary>
            /// 其他
            /// </summary>
            public const string OTH = "OTH";


            /// <summary>
            /// 票期變更
            /// </summary>
            public class dat : PX.Data.BQL.BqlString.Constant<dat> { public dat() : base(DAT) {; } }

            /// <summary>
            /// 付款方式變更
            /// </summary>
            public class pmt : PX.Data.BQL.BqlString.Constant<pmt> { public pmt() : base(PMT) {; } }

            /// <summary>
            /// 抬頭變更
            /// </summary>
            public class tit : PX.Data.BQL.BqlString.Constant<tit> { public tit() : base(TIT) {; } }

            /// <summary>
            /// 作廢、退撤票
            /// </summary>
            public class rev : PX.Data.BQL.BqlString.Constant<rev> { public rev() : base(REV) {; } }

            /// <summary>
            /// 其他
            /// </summary>
            public class oth : PX.Data.BQL.BqlString.Constant<oth> { public oth() : base(OTH) {; } }

        }
        #endregion

        #region Modify Type For OTH CCR
        public class ModifyType_CCR : ModifyType
        {

            public ModifyType_CCR() : base(
                new[]
                {
                    Pair(DAT,"票期變更"),
                    Pair(PMT,"付款方式變更"),
                    Pair(TIT,"抬頭變更"),
                    Pair(OTH,"其他")
                }
                )
            { }
        }
        #endregion
    }
}
