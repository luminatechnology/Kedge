using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class  Page_NM502009 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}
protected void grid_RowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
    {
        bool? value = (bool?)e.Row.Cells["UsrIsAlertVendor"].Value;
        if (value == true)
            e.Row.Style.CssClass = "yellow";
    }
}