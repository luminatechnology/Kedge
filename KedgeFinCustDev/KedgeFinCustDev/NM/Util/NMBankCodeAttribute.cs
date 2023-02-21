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
    //所有銀行分行代碼
    public class NMBankCodeAttribute : PXSelectorAttribute
    {
        public NMBankCodeAttribute()
            : base(
                  typeof(Search2<SegmentValue.value,
                     InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                    And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
                    Where<Segment.dimensionID, Equal<nMBank>,
                    And<Segment.segmentID, Equal<segmentIDPart1>,
                    And<SegmentValue.active, Equal<True>>>>>),
                typeof(SegmentValue.value),
                typeof(SegmentValue.descr))
        {
            DescriptionField = typeof(SegmentValue.descr);
        }

        public const string NMBank = "NMBank";
        public const string SegmentID_PART1 = "1";

        public class nMBank : Constant<string>
        {
            public nMBank() : base(NMBank) { }
        }
        public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }

    }
}
