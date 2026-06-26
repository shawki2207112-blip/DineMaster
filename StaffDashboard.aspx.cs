using System;

namespace DineMaster
{
    public partial class StaffDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (Session["Role"].ToString() != "Staff")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                lblStaffName.Text =
                    Session["StaffName"].ToString();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("Login.aspx");
        }
    }
}