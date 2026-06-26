using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI.WebControls;

namespace DineMaster
{
    public partial class ManageTables : System.Web.UI.Page
    {
        OracleConnection con = new OracleConnection(
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
            OracleDataAdapter da =
                new OracleDataAdapter(
                "SELECT * FROM RESTAURANT_TABLES ORDER BY TABLE_ID",
                con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTables.DataSource = dt;
            gvTables.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            try
            {
                con.Open();

                if (hfTableID.Value == "")
                {
                    string query =
                    @"INSERT INTO RESTAURANT_TABLES
                (TABLE_ID, TABLE_NUMBER, CAPACITY, STATUS)
                VALUES
                (table_seq.NEXTVAL, :tableNo, :tableCap, :tableStatus)";

                    OracleCommand cmd =
                        new OracleCommand(query, con);

                    cmd.BindByName = true;

                    cmd.Parameters.Add(":tableNo",
                        Convert.ToInt32(txtTableNumber.Text));

                    cmd.Parameters.Add(":tableCap",
                        Convert.ToInt32(txtCapacity.Text));

                    cmd.Parameters.Add(":tableStatus",
                        ddlStatus.SelectedValue);

                    cmd.ExecuteNonQuery();

                    OracleCommand commitCmd =
                        new OracleCommand("COMMIT", con);

                    commitCmd.ExecuteNonQuery();

                    lblMessage.Text = "Table Added Successfully";
                }
                else
                {
                    string query =
                    @"UPDATE RESTAURANT_TABLES
                SET
                TABLE_NUMBER = :tableNo,
                CAPACITY = :tableCap,
                STATUS = :tableStatus
                WHERE TABLE_ID = :tableId";

                    OracleCommand cmd =
                        new OracleCommand(query, con);

                    cmd.BindByName = true;

                    cmd.Parameters.Add(":tableNo",
                        Convert.ToInt32(txtTableNumber.Text));

                    cmd.Parameters.Add(":tableCap",
                        Convert.ToInt32(txtCapacity.Text));

                    cmd.Parameters.Add(":tableStatus",
                        ddlStatus.SelectedValue);

                    cmd.Parameters.Add(":tableId",
                        Convert.ToInt32(hfTableID.Value));

                    cmd.ExecuteNonQuery();

                    OracleCommand commitCmd =
                        new OracleCommand("COMMIT", con);

                    commitCmd.ExecuteNonQuery();

                    lblMessage.Text = "Table Updated Successfully";
                }

                con.Close();

                ClearFields();
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

                OracleCommand cmd =
                    new OracleCommand(
                    "DELETE FROM RESTAURANT_TABLES WHERE TABLE_ID = :tableId",
                    con);

                cmd.BindByName = true;

                cmd.Parameters.Add(":tableId", tableID);

                cmd.ExecuteNonQuery();

                OracleCommand commitCmd =
                    new OracleCommand("COMMIT", con);

                commitCmd.ExecuteNonQuery();

                con.Close();

                LoadTables();

                lblMessage.Text = "Table Deleted Successfully";
            }

            if (e.CommandName == "EditRow")
            {
                lblMessage.Text = "";
                con.Open();

                OracleCommand cmd =
                    new OracleCommand(
                    "SELECT * FROM RESTAURANT_TABLES WHERE TABLE_ID = :tableId",
                    con);

                cmd.BindByName = true;

                cmd.Parameters.Add(":tableId", tableID);

                OracleDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    hfTableID.Value =
                        dr["TABLE_ID"].ToString();

                    txtTableNumber.Text =
                        dr["TABLE_NUMBER"].ToString();

                    txtCapacity.Text =
                        dr["CAPACITY"].ToString();

                    ddlStatus.SelectedValue =
                        dr["STATUS"].ToString();
                }

                dr.Close();
                con.Close();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            ClearFields();
        }

        void ClearFields()
        {
            hfTableID.Value = "";

            txtTableNumber.Text = "";

            txtCapacity.Text = "";

            ddlStatus.SelectedIndex = 0;

        }
    }

}
