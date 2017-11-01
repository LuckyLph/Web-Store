using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    //<Louis-PhilippeH>
    private string isAdminLoaded;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session["isAdminLoaded"] = isAdminLoaded;
        }
        if (Session["isAdminLoaded"].ToString() == "true")
        {
            menuAdminLink.Visible = true;
        }
    }
    protected void ButtonLogin_Click(object sender, EventArgs e)
    {
        if (TextBoxUser.Text == "Admin" && TextBoxPassWord.Text == "Admin")
        {
            Session["isAdminLoaded"] = "true";
            //changer page pour admin
        }
    }
}
