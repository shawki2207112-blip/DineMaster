using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class ManageCustomers : System.Web.UI.Page
    {
        OracleConnection con = new OracleConnection(
        ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);


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
                LoadCustomers();
                LoadCustomerAnalytics();
            }
        }

        void LoadCustomers()
        {
            OracleCommand cmd =
                new OracleCommand("GetCustomers", con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "p_cursor",
                OracleDbType.RefCursor,
                ParameterDirection.Output);

            OracleDataAdapter da =
                new OracleDataAdapter(cmd);

            DataTable dt =
                new DataTable();

            da.Fill(dt);

            gvCustomers.DataSource = dt;
            gvCustomers.DataBind();
        }

        void LoadCustomerAnalytics()
        {
            try
            {
                string query = @"
                SELECT c.customer_id,c.customer_name,
                COUNT(o.order_id) AS total_orders,
                CASE
                    WHEN SUM(o.total_amount) IS NULL THEN 0
                    ELSE SUM(o.total_amount)
                END AS total_spent
                FROM CUSTOMERS c
                LEFT JOIN ORDERS o
                ON c.customer_id = o.customer_id
                GROUP BY
                c.customer_id,
                c.customer_name
                ORDER BY
                total_spent DESC";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvCustomerAnalytics.DataSource = dt;
                gvCustomerAnalytics.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.ToString();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                if (hfCustomerID.Value == "")
                {
                    OracleCommand cmd =
                        new OracleCommand(
                        "AddCustomer",
                        con);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "p_name",
                        OracleDbType.Varchar2)
                        .Value =
                        txtCustomerName.Text.Trim();

                    cmd.Parameters.Add(
                        "p_phone",
                        OracleDbType.Varchar2)
                        .Value =
                        txtPhone.Text.Trim();

                    cmd.Parameters.Add(
                        "p_email",
                        OracleDbType.Varchar2)
                        .Value =
                        txtEmail.Text.Trim();

                    cmd.ExecuteNonQuery();

                    lblMessage.Text =
                        "Customer Added Successfully";
                }
                else
                {
                    OracleCommand cmd =
                        new OracleCommand(
                        "UpdateCustomer",
                        con);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "p_id",
                        OracleDbType.Int32)
                        .Value =
                        Convert.ToInt32(
                        hfCustomerID.Value);

                    cmd.Parameters.Add(
                        "p_name",
                        OracleDbType.Varchar2)
                        .Value =
                        txtCustomerName.Text.Trim();

                    cmd.Parameters.Add(
                        "p_phone",
                        OracleDbType.Varchar2)
                        .Value =
                        txtPhone.Text.Trim();

                    cmd.Parameters.Add(
                        "p_email",
                        OracleDbType.Varchar2)
                        .Value =
                        txtEmail.Text.Trim();

                    cmd.ExecuteNonQuery();

                    lblMessage.Text =
                        "Customer Updated Successfully";
                }

                con.Close();

                ClearFields();

                LoadCustomers();
                LoadCustomerAnalytics();
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    ex.Message;

                if (con.State ==
                    ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        protected void gvCustomers_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int rowIndex =
                Convert.ToInt32(
                e.CommandArgument);

            int customerID =
                Convert.ToInt32(
                gvCustomers.DataKeys[rowIndex]
                .Value);

            if (e.CommandName ==
                "DeleteRow")
            {
                try
                {
                    con.Open();

                    OracleCommand cmd =
                        new OracleCommand(
                        "DeleteCustomer",
                        con);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "p_id",
                        OracleDbType.Int32)
                        .Value =
                        customerID;

                    cmd.ExecuteNonQuery();

                    con.Close();

                    lblMessage.Text =
                        "Customer Deleted Successfully";

                    LoadCustomers();
                    LoadCustomerAnalytics();
                }
                catch (Exception ex)
                {
                    lblMessage.Text =
                        ex.Message;
                }
            }

            if (e.CommandName ==
                "EditRow")
            {
                try
                {
                    con.Open();

                    OracleCommand cmd =
                        new OracleCommand(
                        "SELECT * FROM CUSTOMERS WHERE customer_id = :id",
                        con);

                    cmd.BindByName = true;

                    cmd.Parameters.Add(
                        ":id",
                        customerID);

                    OracleDataReader dr =
                        cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        hfCustomerID.Value =
                            dr["customer_id"]
                            .ToString();

                        txtCustomerName.Text =
                            dr["customer_name"]
                            .ToString();

                        txtPhone.Text =
                            dr["phone"]
                            .ToString();

                        txtEmail.Text =
                            dr["email"]
                            .ToString();
                    }

                    dr.Close();
                    con.Close();
                }
                catch (Exception ex)
                {
                    lblMessage.Text =
                        ex.Message;
                }
            }
        }

        protected void btnClear_Click(
            object sender,
            EventArgs e)
        {
            ClearFields();
        }

        void ClearFields()
        {
            hfCustomerID.Value = "";

            txtCustomerName.Text = "";

            txtPhone.Text = "";

            txtEmail.Text = "";

        }

    }


}
