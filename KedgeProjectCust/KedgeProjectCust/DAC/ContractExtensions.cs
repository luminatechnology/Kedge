using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Discount;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Collections;
using System;
using Kedge.DAC;
using PX.Objects.FS;

namespace PX.Objects.CT
{
    public class ContractExt : PXCacheExtension<PX.Objects.CT.Contract>
    {
        #region UsrSiteArea
        [PXDBString(2,IsUnicode =true)]
        [PXUIField(DisplayName = "SiteArea")]
        //­×§ï§ì INSite.SiteID, INSite.SiteCD   
        /*[PXSelector(typeof(Search<INSite.siteID, Where<INSite.active, Equal<True>,And<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>>>),
                    typeof(INSite.siteCD),
                    SubstituteKey = typeof(INSite.siteCD))]*/

        //2020/05/08 §ï§ìAttribute : AREACODE
        [PXSelectorWithCustomOrderBy(typeof(Search<CSAttributeDetail.valueID,
            Where<CSAttributeDetail.attributeID,Equal<word.aREACODE>>,
            OrderBy<Asc<CSAttributeDetail.sortOrder>>>),
            typeof(CSAttributeDetail.sortOrder),
            typeof(CSAttributeDetail.description),
            SubstituteKey = typeof(CSAttributeDetail.description))]


        public virtual string UsrSiteArea { get; set; }
        public abstract class usrSiteArea : IBqlField { }
        #endregion


    }
}