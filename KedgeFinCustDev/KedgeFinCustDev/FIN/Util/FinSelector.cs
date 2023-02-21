using Fin.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using System;

namespace FIN.Util
{
    //Segemt Value : 所得格式代碼
    public class WHTFMTCODESelector : PXSelectorAttribute
    {

        public WHTFMTCODESelector()
    : base(
          typeof(Search2<SegmentValue.value,
             InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
            And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<wHTFMTCODE>,
            And<Segment.segmentID, Equal<segmentIDPart1>,
            And<SegmentValue.active, Equal<True>>>>>),
        typeof(SegmentValue.value),
        typeof(SegmentValue.descr))
        {
            DescriptionField = typeof(SegmentValue.descr);
        }

        public const string WHTFMTCODE = "WHTFMTCODE";
        public class wHTFMTCODE : BqlString.Constant<wHTFMTCODE> { public wHTFMTCODE() : base(WHTFMTCODE) { } }
   
        public const string SegmentID_PART1 = "1";
        public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }
    }

    //Segment Value : 所得註記
    public class WHTFMTSUBCODESelector : PXSelectorAttribute
    {

        public WHTFMTSUBCODESelector()
    : base(
          typeof(Search2<SegmentValue.value,
             InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
            And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<wHTFMTSUBCODE>,
            And<Segment.descr, Equal<Current<TWNWHTTran.wHTFmtCode>>,
            And<SegmentValue.active, Equal<True>>>>>),
        typeof(SegmentValue.value),
        typeof(SegmentValue.descr))
        {
            DescriptionField = typeof(SegmentValue.descr);
        }

        public const string WHTFMTSUBCODE = "WHTFMTSUBCODE";
        public class wHTFMTSUBCODE : BqlString.Constant<wHTFMTSUBCODE> { public wHTFMTSUBCODE() : base(WHTFMTSUBCODE) { } }

        
    }

    //Segment Value : 居住地國或地區代碼
    public class WHTJURISDICTIONSelector : PXSelectorAttribute
    {

        public WHTJURISDICTIONSelector()
    : base(
          typeof(Search2<SegmentValue.value,
             InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
            And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<wHTJURISDICTION>,
            And<Segment.segmentID, Equal<segmentIDPart1>,
            And<SegmentValue.active, Equal<True>>>>>),
        typeof(SegmentValue.value),
        typeof(SegmentValue.descr))
        {
            DescriptionField = typeof(SegmentValue.descr);
        }

        public const string WHTJURISDICTION = "WHTJURISDICTION";
        public class wHTJURISDICTION : BqlString.Constant<wHTJURISDICTION> { public wHTJURISDICTION() : base(WHTJURISDICTION) { } }

        public const string SegmentID_PART1 = "1";
        public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }
    }

    //Segment Value : 租稅協定
    public class WHTTAXAGREEMENTSelector : PXSelectorAttribute
    {

        public WHTTAXAGREEMENTSelector()
    : base(
          typeof(Search2<SegmentValue.value,
             InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
            And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<wHTTAXAGREEMENT>,
            And<Segment.segmentID, Equal<segmentIDPart1>,
            And<SegmentValue.active, Equal<True>>>>>),
        typeof(SegmentValue.value),
        typeof(SegmentValue.descr))
        {
            DescriptionField = typeof(SegmentValue.descr);
        }

        public const string WHTTAXAGREEMENT = "WHTTAXAGREEMENT";
        public class wHTTAXAGREEMENT : BqlString.Constant<wHTTAXAGREEMENT> { public wHTTAXAGREEMENT() : base(WHTTAXAGREEMENT) { } }

        public const string SegmentID_PART1 = "1";
        public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }
    }

    //Segment Value : 扣繳類別
    public class WHTDUTYPECODESelector : PXSelectorAttribute
    {

        public WHTDUTYPECODESelector()
    : base(
          typeof(Search2<SegmentValue.value,
             InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
            And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<wHTDUTYPECODE>,
            And<Segment.segmentID, Equal<segmentIDPart1>,
            And<SegmentValue.active, Equal<True>>>>>),
        typeof(SegmentValue.value),
        typeof(SegmentValue.descr))
        {
            DescriptionField = typeof(SegmentValue.descr);
        }

        public const string WHTDUTYPECODE = "WHTDUTYPECODE";
        public class wHTDUTYPECODE : BqlString.Constant<wHTDUTYPECODE> { public wHTDUTYPECODE() : base(WHTDUTYPECODE) { } }

        public const string SegmentID_PART1 = "1";
        public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }
    }
}
