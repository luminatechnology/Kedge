using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NM.Util
{
    public class NMSegmentKey : PXSelectorAttribute
    {

        public NMSegmentKey()
           : base(
               typeof(Search<SegmentValue.value,
                       Where<SegmentValue.active, Equal<True>>>),
               typeof(SegmentValue.value),
               typeof(SegmentValue.descr)
               )
        {
            DescriptionField = typeof(SegmentValue.descr);
        }

        /// <summary>
        /// 銀行帳號主檔 銀行帳號類型
        /// </summary>
        public const string NMBankAccountType = "NMBKACCOUNTTYPE";
        /// <summary>
        /// 銀行帳號主檔 銀行帳號類型
        /// </summary>
        public class nMBankAccountType : BqlString.Constant<nMBankAccountType> { public nMBankAccountType() : base(NMBankAccountType) { } }

        /// <summary>
        /// NMTr 付款類型
        /// </summary>
        public const string NMTrPaymentType = "NMTRPAYMENTTYPE";
        /// <summary>
        /// NMTr 付款類型
        /// </summary>
        public class nmTrPaymentType : BqlString.Constant<nmTrPaymentType> { public nmTrPaymentType() : base(NMTrPaymentType) { } }


        public const string SegmentID_PART1 = "1";
        public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }
    }
}
