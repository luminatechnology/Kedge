using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Util
{
    public class PSStringList
    {
        #region PS PSPaymentCategory
        public class PSPaymentCategory : PXStringListAttribute
        {
            public PSPaymentCategory() : base(
                new[]
                {
                    Pair(ExtProjectPayment,"外案工程款"),
                    Pair(IntProjectPayment,"內案工程款"),
                    Pair(Other,"其他"),
                    Pair(InterestInc,"利息收入"),
                    Pair(RPRedemption,"RP贖回"),
                    Pair(ShortFinancing,"融資短借"),
                    Pair(CollAndPay,"代收付")
                })
            { }

            //Const
            /// <summary>
            /// 外案工程款
            /// </summary>
            public const String ExtProjectPayment = "EPP";
            /// <summary>
            /// 內案工程款
            /// </summary>
            public const String IntProjectPayment = "IPP";
            /// <summary>
            /// 其他
            /// </summary>
            public const String Other = "OTH";
            /// <summary>
            /// 利息收入
            /// </summary>
            public const String InterestInc = "IIC";
            /// <summary>
            /// RP贖回
            /// </summary>
            public const String RPRedemption = "RPR";
            /// <summary>
            /// 融資短借
            /// </summary>
            public const String ShortFinancing = "STF";
            /// <summary>
            /// 代收付
            /// </summary>
            public const String CollAndPay = "CAP";


            public class extProjectPayment : PX.Data.BQL.BqlString.Constant<extProjectPayment> { public extProjectPayment() : base(ExtProjectPayment) {; } }
            public class intProjectPayment : PX.Data.BQL.BqlString.Constant<intProjectPayment> { public intProjectPayment() : base(IntProjectPayment) {; } }
            public class other : PX.Data.BQL.BqlString.Constant<other> { public other() : base(Other) {; } }
            public class interestInc : PX.Data.BQL.BqlString.Constant<interestInc> { public interestInc() : base(InterestInc) {; } }
            public class rPRedemption : PX.Data.BQL.BqlString.Constant<rPRedemption> { public rPRedemption() : base(RPRedemption) {; } }
            public class shortFinancing : PX.Data.BQL.BqlString.Constant<shortFinancing> { public shortFinancing() : base(ShortFinancing) {; } }
            public class collAndPay : PX.Data.BQL.BqlString.Constant<collAndPay> { public collAndPay() : base(CollAndPay) {; } }
        }
        #endregion

        #region Target Type
        public class PSTargetType : PXStringListAttribute
        {
            public PSTargetType() : base(
                new[]
                {
                    Pair(Customer,"客戶"),
                    Pair(Vendor,"供應商"),
                    Pair(Employee,"員工")
                })
            { }

            //Const
            /// <summary>
            /// 客戶
            /// </summary>
            public const String Customer = "C";
            /// <summary>
            /// 供應商
            /// </summary>
            public const String Vendor = "V";
            /// <summary>
            /// 員工
            /// </summary>
            public const String Employee = "E";

            public class customer : PX.Data.BQL.BqlString.Constant<customer> { public customer() : base(Customer) {; } }
            public class vendor : PX.Data.BQL.BqlString.Constant<vendor> { public vendor() : base(Vendor) {; } }
            public class employee : PX.Data.BQL.BqlString.Constant<employee> { public employee() : base(Employee) {; } }
        }
        #endregion

        #region PS Status
        public class PSStatus : PXStringListAttribute
        {
            public PSStatus() : base(
                new[]
                {
                    Pair(Hold,"Hold"),
                    Pair(Open,"Open"),
                    Pair(Released,"Released"),
                    Pair(Voided,"Voided")
                })
            { }

            //Const
            /// <summary>
            /// 擱置
            /// </summary>
            public const String Hold = "H";
            /// <summary>
            /// 提交
            /// </summary>
            public const String Open = "O";
            /// <summary>
            /// 過帳
            /// </summary>
            public const String Released = "R";
            /// <summary>
            /// 作廢
            /// </summary>
            public const String Voided = "V";

            public class hold : PX.Data.BQL.BqlString.Constant<hold> { public hold() : base(Hold) {; } }
            public class open : PX.Data.BQL.BqlString.Constant<open> { public open() : base(Open) {; } }
            public class released : PX.Data.BQL.BqlString.Constant<released> { public released() : base(Released) {; } }
            public class voided : PX.Data.BQL.BqlString.Constant<voided> { public voided() : base(Voided) {; } }
        }
        #endregion

        #region PS Doc Type
        public class PSDocType : PXStringListAttribute
        {
            public PSDocType() : base(
                new[]
                {
                    Pair(Payment,"一般繳款"),
                    Pair(ArGuarCheck,"應收保證票"),
                    Pair(ApRtnGuarCheck,"繳回應付保證票"),
                    Pair(Other,"其他")
                }
                )
            { }

            //Const
            /// <summary>
            /// 一般繳款
            /// </summary>
            public const String Payment = "STD";
            /// <summary>
            /// 繳回應收保證票
            /// </summary>
            public const String ArGuarCheck = "GUR";
            /// <summary>
            /// 申請退回應付保證票
            /// </summary>
            public const String ApRtnGuarCheck = "RGU";
            /// <summary>
            /// 其他
            /// </summary>
            public const String Other = "OTH";

            public class payment : PX.Data.BQL.BqlString.Constant<payment> { public payment() : base(Payment) {; } }
            public class arGuarCheck : PX.Data.BQL.BqlString.Constant<arGuarCheck> { public arGuarCheck() : base(ArGuarCheck) {; } }
            public class apRtnGuarCheck : PX.Data.BQL.BqlString.Constant<apRtnGuarCheck> { public apRtnGuarCheck() : base(ApRtnGuarCheck) {; } }
            public class other : PX.Data.BQL.BqlString.Constant<other> { public other() : base(Other) {; } }
        }
        #endregion
    }
}
