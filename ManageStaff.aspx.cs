using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class ManageStaff : System.Web.UI.Page
    {
        OracleConnection con = new OracleConnection(
            ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStaff();
            }
        }

        void LoadStaff()
        {
            OracleDataAdapter da =
                new OracleDataAdapter(
                "SELECT * FROM STAFF WHERE ROLE = 'Staff' ORDER BY STAFF_ID",
                con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvStaff.DataSource = dt;
            gvStaff.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            lblMessage.Text = "";
        }

        void ClearFields()
        {
            hfStaffID.Value = "";
            txtStaffName.Text = "";
            txtPhone.Text = "";
            txtSalary.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
        }
    }
}