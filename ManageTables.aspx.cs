using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace DineMaster
{
    public partial class ManageTables : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(
            ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTables();
            }
        }

        void LoadTables()
        {
            SqlDataAdapter da =
                new SqlDataAdapter(
                "SELECT * FROM RESTAURANT_TABLES",
                con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTables.DataSource = dt;
            gvTables.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                if (hfTableID.Value == "")
                {
                    SqlCommand cmd =
                        new SqlCommand(
                        @"INSERT INTO RESTAURANT_TABLES
                        (table_number, capacity, status)
                        VALUES
                        (@number, @capacity, @status)", con);

                    cmd.Parameters.AddWithValue("@number",
                        Convert.ToInt32(txtTableNumber.Text));

                    cmd.Parameters.AddWithValue("@capacity",
                        Convert.ToInt32(txtCapacity.Text));

                    cmd.Parameters.AddWithValue("@status",
                        ddlStatus.SelectedValue);

                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Table Added Successfully";
                }
                else
                {
                    SqlCommand cmd =
                        new SqlCommand(
                        @"UPDATE RESTAURANT_TABLES
                        SET
                        table_number=@number,
                        capacity=@capacity,
                        status=@status
                        WHERE table_id=@id", con);

                    cmd.Parameters.AddWithValue("@id",
                        Convert.ToInt32(hfTableID.Value));

                    cmd.Parameters.AddWithValue("@number",
                        Convert.ToInt32(txtTableNumber.Text));

                    cmd.Parameters.AddWithValue("@capacity",
                        Convert.ToInt32(txtCapacity.Text));

                    cmd.Parameters.AddWithValue("@status",
                        ddlStatus.SelectedValue);

                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Table Updated Successfully";
                }

                con.Close();

                LoadTables();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        protected void gvTables_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int rowIndex =
                Convert.ToInt32(e.CommandArgument);

            int tableID =
                Convert.ToInt32(
                gvTables.DataKeys[rowIndex].Value);

            if (e.CommandName == "DeleteRow")
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                    "DELETE FROM RESTAURANT_TABLES WHERE table_id=@id",
                    con);

                cmd.Parameters.AddWithValue("@id", tableID);

                cmd.ExecuteNonQuery();

                con.Close();

                LoadTables();
            }

            if (e.CommandName == "EditRow")
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                    "SELECT * FROM RESTAURANT_TABLES WHERE table_id=@id",
                    con);

                cmd.Parameters.AddWithValue("@id", tableID);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    hfTableID.Value =
                        dr["table_id"].ToString();

                    txtTableNumber.Text =
                        dr["table_number"].ToString();

                    txtCapacity.Text =
                        dr["capacity"].ToString();

                    ddlStatus.SelectedValue =
                        dr["status"].ToString();
                }

                con.Close();
            }
        }

        
    }
}