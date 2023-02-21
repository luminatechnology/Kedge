using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using RCGV.GV.Util;


namespace RCGV.GV.DAC
{

    [Serializable]
    [PXProjection(typeof(Search5<GVGuiBook.guiBookID,
               InnerJoin<GVPeriod, On<GVPeriod.registrationCD, Equal<GVGuiBook.registrationCD>
                   , And<GVPeriod.outActive, Equal<True>
                   , And<GVPeriod.periodMonth, LessEqual<GVGuiBook.endMonth>
                   , And<GVPeriod.periodMonth, GreaterEqual<GVGuiBook.startMonth>>>>>>,
                     Aggregate<GroupBy<GVGuiBook.guiBookID>
                   >>))]
    [PXHidden]
    public class GVGuiBook_GVPeriod : GVGuiBook
    {
        //#region OutActive
        //[PXDBBool(BqlField = typeof(GVPeriod.outActive))]
        //[PXUIField(DisplayName = "Out Active")]
        //public virtual bool? OutActive { get; set; }
        //public abstract class outActive : PX.Data.BQL.BqlBool.Field<outActive> { }
        //#endregion

    }
}