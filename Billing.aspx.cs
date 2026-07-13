using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class Billing : System.Web.UI.Page
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!Session["Role"].ToString().Equals("Staff", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("Login.aspx");
            }

            if (Session["StaffID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                try
                {
                    CreateBillingPLSQLObjects();

                    LoadOrders();
                    LoadBills();
                    LoadOrderItems(0);
                    ClearBillInfo();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                }
            }
        }


        void CreateBillingPLSQLObjects()
        {
            ExecuteDDL(@"
            CREATE OR REPLACE FUNCTION GetBillAmount
            (
                p_order_id IN NUMBER
            )
            RETURN NUMBER
            IS
                v_total ORDERS.total_amount%TYPE;
            BEGIN
                SELECT NVL(total_amount, 0)
                INTO v_total
                FROM ORDERS
                WHERE order_id = p_order_id;

                RETURN v_total;

            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    RETURN 0;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE TRIGGER trg_set_bill_total
            BEFORE INSERT OR UPDATE
            ON BILLS
            FOR EACH ROW
            BEGIN
                :NEW.total_bill := GetBillAmount(:NEW.order_id);

                IF :NEW.bill_date IS NULL THEN
                    :NEW.bill_date := SYSDATE;
                END IF;

                IF :NEW.payment_status IS NULL THEN
                    :NEW.payment_status := 'Paid';
                END IF;

                IF :NEW.total_bill <= 0 THEN
                    RAISE_APPLICATION_ERROR(-20201, 'Cannot generate bill for empty order');
                END IF;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE GenerateBill
            (
             p_order_id IN NUMBER,
            p_staff_id IN NUMBER,
            p_bill_id OUT NUMBER
            )     
            AS
            v_order_status ORDERS.order_status%TYPE;
            v_total ORDERS.total_amount%TYPE;
            v_count NUMBER;
            v_table_id ORDERS.table_id%TYPE;
            BEGIN
            SELECT order_status, table_id
            INTO v_order_status, v_table_id
            FROM ORDERS
            WHERE order_id = p_order_id;

            IF UPPER(v_order_status) = 'CANCELLED' THEN
                 RAISE_APPLICATION_ERROR(-20202, 'Cannot generate bill for cancelled order');
            END IF;

             v_total := GetBillAmount(p_order_id);

             IF v_total <= 0 THEN
                 RAISE_APPLICATION_ERROR(-20203, 'Order has no billable amount');
             END IF;

            SELECT COUNT(*)
            INTO v_count
            FROM BILLS
            WHERE order_id = p_order_id;

             IF v_count > 0 THEN

           SELECT bill_id
           INTO p_bill_id
           FROM BILLS
           WHERE order_id = p_order_id
           AND ROWNUM = 1;

           UPDATE BILLS
           SET staff_id = p_staff_id,
            bill_date = SYSDATE,
             payment_status = 'Paid'
             WHERE bill_id = p_bill_id;

            ELSE

             SELECT BILL_SEQ.NEXTVAL
             INTO p_bill_id
            FROM dual;

        INSERT INTO BILLS
        (
            bill_id,
            order_id,
            staff_id,
            bill_date,
            total_bill,
            payment_status
        )
        VALUES
        (
            p_bill_id,
            p_order_id,
            p_staff_id,
            SYSDATE,
            v_total,
            'Unpaid'
        );

            END IF;

             UPDATE ORDERS
             SET order_status = 'Completed'
             WHERE order_id = p_order_id;

            UPDATE RESTAURANT_TABLES
            SET status = 'Available'
            WHERE table_id = v_table_id;

             COMMIT;
             END;");
        }

        void ExecuteDDL(string sql)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                con.Open();

                OracleCommand cmd = new OracleCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
        }


        void LoadOrders()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    o.order_id,
                    'Order #' || o.order_id || ' - ' || c.customer_name || ' - Tk ' || o.total_amount AS order_info
                  FROM ORDERS o
                  INNER JOIN CUSTOMERS c
                    ON o.customer_id = c.customer_id
                  WHERE UPPER(o.order_status) <> 'CANCELLED'
                  ORDER BY o.order_id DESC";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlOrder.DataSource = dt;
                ddlOrder.DataTextField = "order_info";
                ddlOrder.DataValueField = "order_id";
                ddlOrder.DataBind();

                ddlOrder.Items.Insert(0,
                    new ListItem("-- Select Order --", ""));
            }
        }


        void LoadOrderInfo(int orderID)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    c.customer_name,
                    t.table_number,
                    o.order_status,
                    o.total_amount
                  FROM ORDERS o
                  INNER JOIN CUSTOMERS c
                    ON o.customer_id = c.customer_id
                  INNER JOIN RESTAURANT_TABLES t
                    ON o.table_id = t.table_id
                  WHERE o.order_id = :order_id";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":order_id", OracleDbType.Int32)
                    .Value = orderID;

                con.Open();

                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblCustomerName.Text =
                        dr["customer_name"].ToString();

                    lblTableNumber.Text =
                        dr["table_number"].ToString();

                    lblOrderStatus.Text =
                        dr["order_status"].ToString();

                    lblTotalAmount.Text =
                        dr["total_amount"].ToString();
                }

                dr.Close();
            }
        }


        void LoadOrderItems(int orderID)
        {
            if (orderID == 0)
            {
                gvOrderItems.DataSource = null;
                gvOrderItems.DataBind();
                return;
            }

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    m.item_name AS ITEM_NAME,
                    m.category AS CATEGORY,
                    oi.quantity AS QUANTITY,
                    oi.unit_price AS UNIT_PRICE,
                    oi.subtotal AS SUBTOTAL
                  FROM ORDER_ITEMS oi
                  INNER JOIN MENU_ITEMS m
                    ON oi.item_id = m.item_id
                  WHERE oi.order_id = :order_id
                  ORDER BY oi.order_item_id";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":order_id", OracleDbType.Int32)
                    .Value = orderID;

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvOrderItems.DataSource = dt;
                gvOrderItems.DataBind();
            }
        }


        void LoadBills()
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
                  ORDER BY b.bill_id DESC";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBills.DataSource = dt;
                gvBills.DataBind();
            }
        }


        protected void ddlOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (ddlOrder.SelectedValue == "")
            {
                ClearBillInfo();
                LoadOrderItems(0);
                return;
            }

            int orderID = Convert.ToInt32(ddlOrder.SelectedValue);

            LoadOrderInfo(orderID);
            LoadOrderItems(orderID);
        }

        protected void btnGenerateBill_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            try
            {
                if (ddlOrder.SelectedValue == "")
                {
                    lblMessage.Text = "Please select an order first.";
                    return;
                }

                int orderID = Convert.ToInt32(ddlOrder.SelectedValue);

                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    con.Open();

                    OracleCommand cmd = new OracleCommand("GenerateBill", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("p_order_id", OracleDbType.Int32)
                        .Value = orderID;

                    cmd.Parameters.Add("p_staff_id", OracleDbType.Int32)
                        .Value = Convert.ToInt32(Session["StaffID"]);

                    OracleParameter outputBillID =
                        new OracleParameter("p_bill_id", OracleDbType.Int32);

                    outputBillID.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(outputBillID);

                    cmd.ExecuteNonQuery();

                    hfBillID.Value =
                        outputBillID.Value.ToString();
                }

                lblMessage.Text =
                    "Bill Generated Successfully. Bill ID: " + hfBillID.Value;

                LoadOrderInfo(orderID);
                LoadOrders();
                LoadOrderItems(orderID);
                LoadBills();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }


        protected void btnClear_Click(object sender, EventArgs e)
        {
            hfBillID.Value = "";

            if (ddlOrder.Items.Count > 0)
            {
                ddlOrder.SelectedIndex = 0;
            }

            ClearBillInfo();
            LoadOrderItems(0);

            lblMessage.Text = "";
        }

        void ClearBillInfo()
        {
            lblCustomerName.Text = "-";
            lblTableNumber.Text = "-";
            lblOrderStatus.Text = "-";
            lblTotalAmount.Text = "0";
        }
    }
}