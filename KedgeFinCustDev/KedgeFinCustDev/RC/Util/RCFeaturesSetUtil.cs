using PX.Data;
using RC.DAC;
using System;
using System.Reflection;

namespace RC.Util
{
    /**
     * <summary>
     * 系統功能判定
     * </summary>
     * **/
    public class RCFeaturesSetUtil
    {

        /**
         * <summary>
         * 取得RC系統設定
         * </summary>
         * **/
        private static RCFeaturesSet GetRCFeaturesSet(PXGraph graph)
        {
            RCFeaturesSet set = PXSelect<RCFeaturesSet>.Select(graph);
            return set ?? new RCFeaturesSet();
        }

        /**
         * <summary>
         * 環境設定與專案是否同樣為測試環境 OR 正式環境
         * </summary>
         * **/
        public static bool IsActive(PXGraph graph)
        {
            return !((GetRCFeaturesSet(graph).IsTestEnv ?? false) ^ RCFeaturesSetProperties.IS_TEST_ENV);
        }


        /**
        * <summary>
        * 功能是否生效<br/>
        * <paramref name="propertyName"/> 參數名稱 請使用RCFeaturesSetProperites取得
        * </summary>
        * **/
        public static bool IsActiveOP(PXGraph graph, string propertyName)
        {
            RCFeaturesSet set = GetRCFeaturesSet(graph);
            PropertyInfo[] type = set.GetType().GetProperties();
            PropertyInfo info = set.GetType().GetProperty(propertyName);
            if (info == null) return false;
            return ((bool?)info.GetValue(set) ?? false);
        }

        /**
        * <summary>
        * 功能是否生效(已包含環境判斷)<br/>
        * <paramref name="propertyName"/> 參數名稱 請使用RCFeaturesSetProperites取得
        * </summary>
        * **/
        public static bool IsActive(PXGraph graph,string propertyName)
        {
            return IsActive(graph) && IsActiveOP(graph,propertyName);
        }

        /**
         * <summary>
         * 回首頁
         * </summary>
         * **/
        public static void BackToHomePage() {
            //if (!((System.Web.UI.Page)System.Web.HttpContext.Current.CurrentHandler).IsCallback) {
                String url = PX.Common.PXUrl.SiteUrlWithPath() + "/Frames/Default.aspx";
                System.Web.HttpContext.Current.Response.Redirect(url);
            //}
        }
    }
}
