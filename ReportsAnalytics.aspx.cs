using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class ReportsAnalytics : System.Web.UI.Page
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!Session["Role"].ToString().Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                txtSalesDate.Text = DateTime.Now.ToString("yyyy-MM-dd");


                LoadAllReports();
            }
        }

        protected void btnLoadReports_Click(object sender, EventArgs e)
        {
            LoadAllReports();
        }

        void LoadAllReports()
        {
            lblMessage.Text = "";

            try
            {
                LoadTodayRevenue();
                LoadTodaySalesReport();
                LoadOrderStatusReport();
                LoadReservationStatusReport();
                LoadBestSellerStaff();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        void LoadTodayRevenue()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    COUNT(*) AS SALES_COUNT,
                    NVL(SUM(total_bill), 0) AS TODAY_REVENUE
                  FROM BILLS
                  WHERE TRUNC(bill_date) = :sales_date";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":sales_date", OracleDbType.Date)
                    .Value = Convert.ToDateTime(txtSalesDate.Text);

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    lblTodaySalesCount.Text =
                        dt.Rows[0]["SALES_COUNT"].ToString();

                    lblTodayRevenue.Text =
                        dt.Rows[0]["TODAY_REVENUE"].ToString();
                }
            }
        }


        void LoadTodaySalesReport()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    b.bill_id AS BILL_ID,
                    b.order_id AS ORDER_ID,
                    c.customer_name AS CUSTOMER_NAME,
                    TO_CHAR(b.bill_date, 'DD-MON-YYYY HH:MI AM') AS BILL_DATE,
                    b.total_bill AS TOTAL_BILL,
                    b.payment_status AS PAYMENT_STATUS
                  FROM BILLS b
                  INNER JOIN ORDERS o
                    ON b.order_id = o.order_id
                  INNER JOIN CUSTOMERS c
                    ON o.customer_id = c.customer_id
                  WHERE TRUNC(b.bill_date) = :sales_date
                  ORDER BY b.bill_id DESC";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":sales_date", OracleDbType.Date)
                    .Value = Convert.ToDateTime(txtSalesDate.Text);

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvTodaySales.DataSource = dt;
                gvTodaySales.DataBind();
            }
        }

        void LoadOrderStatusReport()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    o.order_id AS ORDER_ID,
                    c.customer_name AS CUSTOMER_NAME,
                    t.table_number AS TABLE_NUMBER,
                    TO_CHAR(o.order_date, 'DD-MON-YYYY HH:MI AM') AS ORDER_DATE,
                    o.total_amount AS TOTAL_AMOUNT,
                    o.order_status AS ORDER_STATUS
                  FROM ORDERS o
                  INNER JOIN CUSTOMERS c
                    ON o.customer_id = c.customer_id
                  INNER JOIN RESTAURANT_TABLES t
                    ON o.table_id = t.table_id
                  WHERE (:order_status = 'All' OR o.order_status = :order_status)
                  ORDER BY o.order_id DESC";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":order_status", OracleDbType.Varchar2)
                    .Value = ddlOrderStatus.SelectedValue;

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvOrderStatus.DataSource = dt;
                gvOrderStatus.DataBind();

                lblOrderStatusCount.Text = dt.Rows.Count.ToString();
            }
        }

        void LoadReservationStatusReport()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    r.reservation_id AS RESERVATION_ID,
                    c.customer_name AS CUSTOMER_NAME,
                    t.table_number AS TABLE_NUMBER,
                    TO_CHAR(r.reservation_date, 'DD-MON-YYYY') AS RESERVATION_DATE,
                    r.reservation_time AS RESERVATION_TIME,
                    r.number_of_people AS NUMBER_OF_PEOPLE,
                    r.status AS STATUS
                  FROM RESERVATIONS r
                  INNER JOIN CUSTOMERS c
                    ON r.customer_id = c.customer_id
                  INNER JOIN RESTAURANT_TABLES t
                    ON r.table_id = t.table_id
                  WHERE (:reservation_status = 'All' OR r.status = :reservation_status)
                  ORDER BY r.reservation_id DESC";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":reservation_status", OracleDbType.Varchar2)
                    .Value = ddlReservationStatus.SelectedValue;

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvReservationStatus.DataSource = dt;
                gvReservationStatus.DataBind();

                lblReservationStatusCount.Text = dt.Rows.Count.ToString();
            }
        }

        void LoadBestSellerStaff()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    s.staff_id AS STAFF_ID,
                    s.staff_name AS STAFF_NAME,
                    COUNT(b.bill_id) AS TOTAL_BILLS,
                    NVL(SUM(b.total_bill), 0) AS TOTAL_REVENUE
                  FROM BILLS b
                  INNER JOIN STAFF s
                    ON b.staff_id = s.staff_id
                  GROUP BY
                    s.staff_id,
                    s.staff_name
                  ORDER BY TOTAL_REVENUE DESC";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBestSellerStaff.DataSource = dt;
                gvBestSellerStaff.DataBind();

                if (dt.Rows.Count > 0)
                {
                    lblBestSellerStaff.Text =
                        dt.Rows[0]["STAFF_NAME"].ToString();
                }
                else
                {
                    lblBestSellerStaff.Text = "-";
                }
            }
        }
    }
}