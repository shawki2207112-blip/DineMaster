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

        protected void gvStaff_RowCommand(object sender,
            GridViewCommandEventArgs e)
        {
            int rowIndex =
                Convert.ToInt32(e.CommandArgument);

            int staffID =
                Convert.ToInt32(
                gvStaff.DataKeys[rowIndex].Value);

            if (e.CommandName == "DeleteRow")
            {
                con.Open();

                OracleCommand cmd =
                    new OracleCommand(
                    "DELETE FROM STAFF WHERE staff_id = :staffId",
                    con);

                cmd.BindByName = true;

                cmd.Parameters.Add(":staffId", staffID);

                cmd.ExecuteNonQuery();

                OracleCommand commitCmd =
                    new OracleCommand("COMMIT", con);

                commitCmd.ExecuteNonQuery();

                con.Close();

                LoadStaff();

                lblMessage.Text = "Staff Deleted Successfully";
            }

            if (e.CommandName == "EditRow")
            {
                con.Open();

                OracleCommand cmd =
                    new OracleCommand(
                    "SELECT * FROM STAFF WHERE staff_id = :staffId",
                    con);

                cmd.BindByName = true;

                cmd.Parameters.Add(":staffId", staffID);

                OracleDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    hfStaffID.Value =
                        dr["staff_id"].ToString();

                    txtStaffName.Text =
                        dr["staff_name"].ToString();

                    txtPhone.Text =
                        dr["phone"].ToString();

                    txtSalary.Text =
                        dr["salary"].ToString();

                    txtUsername.Text =
                        dr["username"].ToString();

                    txtPassword.Text =
                        dr["password"].ToString();
                }

                dr.Close();
                con.Close();
            }
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