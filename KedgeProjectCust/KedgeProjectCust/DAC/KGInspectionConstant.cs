using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL;

namespace Kedge.DAC
{
    #region KGInspectionConstant
    public class KGInspectionConstant
    {
        public const string KGMNINSC = "KGMNINSC";
        public const string KGSHRISK = "KGSHRISK";


        public class kgmninsc : BqlString.Constant<kgmninsc> { public kgmninsc() : base(KGMNINSC) { } }
        public class kgshrisk : BqlString.Constant<kgshrisk> { public kgshrisk() : base(KGSHRISK) { } }


        public const string SegmentID_PART1 = "1";
        public const string SegmentID_PART2 = "2";
        public const string SegmentID_PART3 = "3";
        public class segmentIDPart1 : BqlString.Constant<segmentIDPart1> { public segmentIDPart1() : base(SegmentID_PART1) { } }
        public class segmentIDPart2 : BqlString.Constant<segmentIDPart2> { public segmentIDPart2() : base(SegmentID_PART2) { } }
        public class segmentIDPart3 : BqlString.Constant<segmentIDPart3> { public segmentIDPart3() : base(SegmentID_PART3) { } }

        //Altea Add 2019/09/11 職安衛segmentCD
        public const string KGSHINSC = "KGSHINSC";

        public class kgshinsc : BqlString.Constant<kgshinsc> { public kgshinsc() : base(KGSHINSC) { } }

        public const string SHIN_SegmentID_PART1 = "1";
        public const string SHIN_SegmentID_PART2 = "2";
       
        public class shin_segmentIDPart1 : BqlString.Constant<shin_segmentIDPart1> { public shin_segmentIDPart1() : base(SHIN_SegmentID_PART1) { } }
        public class shin_segmentIDPart2 : BqlString.Constant<shin_segmentIDPart2> { public shin_segmentIDPart2() : base(SHIN_SegmentID_PART2) { } }
        

        public const string NewName = " <NEW>";

        // INItemClassQA
        public const string INItemClassQA = "QA";
        public class inItemClassQA : BqlString.Constant<inItemClassQA> { public inItemClassQA() : base(INItemClassQA) { } }

        //INItemClassSC
        public const string INItemClassSC = "SC";
        public class inItemClassSC : BqlString.Constant<inItemClassSC> { public inItemClassSC() : base(INItemClassSC) { } }

        public class Word
        {
            public const string ro = "RO";
            public const string n = "N";
            public const string a = "A";
            public const string d = "D";
            public const string r = "R";

            public class RO : BqlString.Constant<RO>
            {
                public RO() : base(ro) { }
            }
            public class N : BqlString.Constant<N>
            {
                public N() : base(n) { }
            }
            public class A : BqlString.Constant<A>
            {
                public A() : base(a) { }
            }
            public class D : BqlString.Constant<D>
            {
                public D() : base(d) { }
            }
            public class R : BqlString.Constant<R>
            {
                public R() : base(r) { }
            }

        }
    }
    #endregion

    #region KGSelfInspectionStatuses
    public static class KGSelfInspectionStatuses
    {
	
		public const string HoldName = "Hold";
        public const string LockName = "Lock";
        public const string OpenName = "Open";
        public const string PendingName = "Pending";
        public const string CloseName = "Close";
        public const string Hold = "H";
        public const string Lock = "L";
        public const string Open = "N";
        public const string Pending = "P";
        public const string Close = "C";
        
        public class open : BqlString.Constant<open>
        {
            public open() : base(Open) { }
        }
        public class pending : BqlString.Constant<pending>
        {
            public pending() : base(Pending) { }
        }

        public class close : BqlString.Constant<close>
        {
            public close() : base(Close) { }
        }
        public class hold : BqlString.Constant<hold>
        {
            public hold() : base(Hold) { }
        }
        public class locked : BqlString.Constant<locked>
        {
            public locked() : base(Lock) { }
        }

        public class ListAttribute4Template : PXStringListAttribute
        {
            public ListAttribute4Template()
                : base(
                new string[] { Open,Hold, Close },
                new string[] { OpenName,HoldName, CloseName })
            { }
        }

        public class ListAttribute4Self : PXStringListAttribute
        {
            public ListAttribute4Self()
                : base(
                new string[] { Open, Hold, Close },
                new string[] { OpenName, HoldName, CloseName })
            { }
        }
    }
    #endregion

    #region KGMMonthlySelfInspectionEvaluations
    public static class KGMMonthlySelfInspectionEvaluations
    {
        public const string KMI_A = "A";
        public const string KMI_B = "B";
        public const string KMI_C = "C";
        public const string KMI_D = "D";
        public const string KMI_E = "E";
        public const string KMI_F = "F";
        public const string KMI_G = "G";
        public const string KMI_H = "H";
        public const string KMI_I = "I";
        public const string KMI_J = "J";

        public class kmiA : BqlString.Constant<kmiA>
        {
            public kmiA() : base(KMI_A) { }
        }

        public class kmiB : BqlString.Constant<kmiB>
        {
            public kmiB() : base(KMI_B) { }
        }

        public class kmiC : BqlString.Constant<kmiC>
        {
            public kmiC() : base(KMI_C) { }
        }

        public class kmiD : BqlString.Constant<kmiD>
        {
            public kmiD() : base(KMI_D) { }
        }
        public class kmiE : BqlString.Constant<kmiE>
        {
            public kmiE() : base(KMI_E) { }
        }
        public class kmiF : BqlString.Constant<kmiF>
        {
            public kmiF() : base(KMI_F) { }
        }
        public class kmiG : BqlString.Constant<kmiG>
        {
            public kmiG() : base(KMI_G) { }
        }
        public class kmiH : BqlString.Constant<kmiH>
        {
            public kmiH() : base(KMI_H) { }
        }
        public class kmiI : BqlString.Constant<kmiI>
        {
            public kmiI() : base(KMI_I) { }
        }
        public class kmiJ : BqlString.Constant<kmiJ>
        {
            public kmiJ() : base(KMI_J) { }
        }


        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                    new string[] {
                    KMI_A, KMI_B, KMI_C, KMI_D, KMI_E,
                    KMI_F, KMI_G, KMI_H, KMI_I, KMI_J
                },
                new string[] {
                    KMI_A, KMI_B, KMI_C, KMI_D, KMI_E,
                    KMI_F, KMI_G, KMI_H, KMI_I, KMI_J
                })
            { }
        }

        public class ListSimpleAttribute : PXStringListAttribute
        {
            public ListSimpleAttribute()
                : base(
                new string[] {
                    KMI_A, KMI_B, KMI_C, KMI_D, KMI_E,
                    KMI_F, KMI_G, KMI_H, KMI_I, KMI_J
                },
                new string[] {
                    KMI_A, KMI_B, KMI_C, KMI_D, KMI_E,
                    KMI_F, KMI_G, KMI_H, KMI_I, KMI_J
                })
            { }
        }
    }
    #endregion

    #region KGSelfInspectionTestResults
    public static class KGSelfInspectionTestResults
    {

        public const string QualifiedName = "檢查合格";
        public const string DefectName = "有缺失需改正";
        public const string NoItemName = "無此檢查項目";
        public const string Qualified = "1";
        public const string Defect = "2";
        public const string NoItem = "3";

        public class qualified : BqlString.Constant<qualified>
        {
            public qualified() : base(Qualified) { }
        }

        public class defect : BqlString.Constant<defect>
        {
            public defect() : base(Defect) { }
        }
        public class noItem : BqlString.Constant<noItem>
        {
            public noItem() : base(NoItem) { }
        }
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                    new string[] { Qualified, Defect, NoItem },
                    new string[] { QualifiedName, DefectName, NoItemName })
            { }
        }

        public class ListSimpleAttribute : PXStringListAttribute
        {
            public ListSimpleAttribute()
                : base(
                new string[] { Qualified, Defect, NoItem },
                new string[] { QualifiedName, DefectName, NoItemName })
            { }
        }
    }
    #endregion

    #region KGSelfInspectionReviewResults
    public static class KGSelfInspectionReviewResults
    {

        public const string NotReviewName = "未複查";
        public const string ReviewedName = "已複查";

        public const string NotReview = "1";
        public const string Reviewed = "2";


        public class notReview : BqlString.Constant<notReview>
        {
            public notReview() : base(NotReview) { }
        }

        public class reviewed : BqlString.Constant<reviewed>
        {
            public reviewed() : base(Reviewed) { }
        }

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                    new string[] { NotReview, Reviewed },
                    new string[] { NotReviewName, ReviewedName })
            { }
        }

        public class ListSimpleAttribute : PXStringListAttribute
        {
            public ListSimpleAttribute()
                : base(
                    new string[] { Reviewed, Reviewed },
                    new string[] { NotReviewName, ReviewedName })
            { }
        }
    }
    #endregion
}
