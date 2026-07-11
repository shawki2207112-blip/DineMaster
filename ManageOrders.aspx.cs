using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class ManageOrders : System.Web.UI.Page
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
                    CreateOrderPLSQLObjects();
                    LoadCustomers();
                    LoadTables();
                    LoadMenuItems();
                    LoadOrders();
                    LoadOrderItems(0);
                    LoadOrderSummary();
                    LoadPopularItems();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                }
            }

        }

        void CreateOrderPLSQLObjects()
        {
            ExecuteDDL(@"
            CREATE OR REPLACE FUNCTION GetOrderTotal
            (
                p_order_id IN NUMBER
            )
            RETURN NUMBER
            IS
                v_total NUMBER;
            BEGIN
                SELECT NVL(SUM(subtotal), 0)
                INTO v_total
                FROM ORDER_ITEMS
                WHERE order_id = p_order_id;

                RETURN v_total;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE TRIGGER trg_order_item_subtotal
            BEFORE INSERT OR UPDATE OF item_id, quantity
            ON ORDER_ITEMS
            FOR EACH ROW
            DECLARE
                v_price MENU_ITEMS.price%TYPE;
            BEGIN
                IF :NEW.quantity IS NULL OR :NEW.quantity <= 0 THEN
                    RAISE_APPLICATION_ERROR(-20001, 'Quantity must be greater than zero');
                END IF;

                SELECT price
                INTO v_price
                FROM MENU_ITEMS
                WHERE item_id = :NEW.item_id;

                :NEW.unit_price := v_price;
                :NEW.subtotal := :NEW.quantity * v_price;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE TRIGGER trg_update_order_total
            AFTER INSERT OR UPDATE OR DELETE
            ON ORDER_ITEMS
            BEGIN
                UPDATE ORDERS
                SET total_amount = GetOrderTotal(order_id);
            END;");


            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE AddOrder
            (
                p_customer_id IN NUMBER,
                p_table_id IN NUMBER,
                p_staff_id IN NUMBER,
                p_order_status IN VARCHAR2,
                p_order_id OUT NUMBER
            )
            AS
                v_table_status RESTAURANT_TABLES.status%TYPE;
            BEGIN
                SELECT status
                INTO v_table_status
                FROM RESTAURANT_TABLES
                WHERE table_id = p_table_id;

                IF UPPER(v_table_status) <> 'AVAILABLE' THEN
                    RAISE_APPLICATION_ERROR(-20002, 'Selected table is not available');
                END IF;

                SELECT ORDER_SEQ.NEXTVAL
                INTO p_order_id
                FROM dual;

                INSERT INTO ORDERS
                (
                    order_id,
                    customer_id,
                    table_id,
                    staff_id,
                    order_date,
                    total_amount,
                    order_status
                )
                VALUES
                (
                    p_order_id,
                    p_customer_id,
                    p_table_id,
                    p_staff_id,
                    SYSDATE,
                    0,
                    p_order_status
                );

                IF UPPER(p_order_status) = 'CANCELLED'
                   OR UPPER(p_order_status) = 'COMPLETED' THEN

                    UPDATE RESTAURANT_TABLES
                    SET status = 'Available'
                    WHERE table_id = p_table_id;

                ELSE

                    UPDATE RESTAURANT_TABLES
                    SET status = 'Occupied'
                    WHERE table_id = p_table_id;

                END IF;

                COMMIT;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE UpdateOrder
            (
                p_order_id IN NUMBER,
                p_customer_id IN NUMBER,
                p_table_id IN NUMBER,
                p_order_status IN VARCHAR2
            )
            AS
                v_old_table_id ORDERS.table_id%TYPE;
                v_new_table_status RESTAURANT_TABLES.status%TYPE;
            BEGIN
                SELECT table_id
                INTO v_old_table_id
                FROM ORDERS
                WHERE order_id = p_order_id;

                IF v_old_table_id <> p_table_id THEN

                    SELECT status
                    INTO v_new_table_status
                    FROM RESTAURANT_TABLES
                    WHERE table_id = p_table_id;

                    IF UPPER(v_new_table_status) <> 'AVAILABLE' THEN
                        RAISE_APPLICATION_ERROR(-20003, 'New selected table is not available');
                    END IF;

                    UPDATE RESTAURANT_TABLES
                    SET status = 'Available'
                    WHERE table_id = v_old_table_id;

                END IF;

                UPDATE ORDERS
                SET customer_id = p_customer_id,
                    table_id = p_table_id,
                    order_status = p_order_status
                WHERE order_id = p_order_id;

                IF UPPER(p_order_status) = 'CANCELLED'
                   OR UPPER(p_order_status) = 'COMPLETED' THEN

                    UPDATE RESTAURANT_TABLES
                    SET status = 'Available'
                    WHERE table_id = p_table_id;

                ELSE

                    UPDATE RESTAURANT_TABLES
                    SET status = 'Occupied'
                    WHERE table_id = p_table_id;

                END IF;

                COMMIT;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE AddOrderItem
            (
                p_order_id IN NUMBER,
                p_item_id IN NUMBER,
                p_quantity IN NUMBER
            )
            AS
                v_count NUMBER;
                v_available MENU_ITEMS.availability%TYPE;
                v_order_status ORDERS.order_status%TYPE;
            BEGIN
                SELECT order_status
                INTO v_order_status
                FROM ORDERS
                WHERE order_id = p_order_id;

                IF UPPER(v_order_status) = 'CANCELLED'
                   OR UPPER(v_order_status) = 'COMPLETED' THEN
                    RAISE_APPLICATION_ERROR(-20004, 'Cannot add item to this order');
                END IF;

                SELECT availability
                INTO v_available
                FROM MENU_ITEMS
                WHERE item_id = p_item_id;

                IF UPPER(v_available) <> 'AVAILABLE' THEN
                    RAISE_APPLICATION_ERROR(-20005, 'Food item is not available');
                END IF;

                SELECT COUNT(*)
                INTO v_count
                FROM ORDER_ITEMS
                WHERE order_id = p_order_id
                AND item_id = p_item_id;

                IF v_count > 0 THEN

                    UPDATE ORDER_ITEMS
                    SET quantity = quantity + p_quantity
                    WHERE order_id = p_order_id
                    AND item_id = p_item_id;

                ELSE

                    INSERT INTO ORDER_ITEMS
                    (
                        order_item_id,
                        order_id,
                        item_id,
                        quantity
                    )
                    VALUES
                    (
                        ORDERITEM_SEQ.NEXTVAL,
                        p_order_id,
                        p_item_id,
                        p_quantity
                    );

                END IF;

                COMMIT;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE CancelOrder
            (
                p_order_id IN NUMBER
            )
            AS
                v_table_id ORDERS.table_id%TYPE;
            BEGIN
                SELECT table_id
                INTO v_table_id
                FROM ORDERS
                WHERE order_id = p_order_id;

                UPDATE ORDERS
                SET order_status = 'Cancelled'
                WHERE order_id = p_order_id;

                UPDATE RESTAURANT_TABLES
                SET status = 'Available'
                WHERE table_id = v_table_id;

                COMMIT;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE DeleteOrder
            (
                p_order_id IN NUMBER
            )
            AS
                v_table_id ORDERS.table_id%TYPE;
            BEGIN
                SELECT table_id
                INTO v_table_id
                FROM ORDERS
                WHERE order_id = p_order_id;

                DELETE FROM BILLS
                WHERE order_id = p_order_id;

                DELETE FROM ORDER_ITEMS
                WHERE order_id = p_order_id;

                DELETE FROM ORDERS
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
        void LoadCustomers()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT customer_id, customer_name
                  FROM CUSTOMERS
                  ORDER BY customer_name";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlCustomer.DataSource = dt;
                ddlCustomer.DataTextField = "customer_name";
                ddlCustomer.DataValueField = "customer_id";
                ddlCustomer.DataBind();

                ddlCustomer.Items.Insert(0,
                    new ListItem("-- Select Customer --", ""));
            }
        }

        void LoadTables(int selectedTableID = 0)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT table_id,
                         'Table ' || table_number || ' - ' || capacity || ' Seats (' || status || ')' AS table_info
                  FROM RESTAURANT_TABLES
                  WHERE UPPER(status) = 'AVAILABLE'
                  OR table_id = :selected_table_id
                  ORDER BY table_number";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":selected_table_id", OracleDbType.Int32)
                    .Value = selectedTableID;

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTable.DataSource = dt;
                ddlTable.DataTextField = "table_info";
                ddlTable.DataValueField = "table_id";
                ddlTable.DataBind();

                ddlTable.Items.Insert(0,
                    new ListItem("-- Select Table --", ""));
            }
        }

        void LoadMenuItems()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT item_id,
                         item_name || ' - Tk ' || price AS item_info
                  FROM MENU_ITEMS
                  WHERE UPPER(availability) = 'AVAILABLE'
                  ORDER BY item_name";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlMenuItem.DataSource = dt;
                ddlMenuItem.DataTextField = "item_info";
                ddlMenuItem.DataValueField = "item_id";
                ddlMenuItem.DataBind();

                ddlMenuItem.Items.Insert(0,
                    new ListItem("-- Select Food Item --", ""));
            }
        }

        void LoadOrders()
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
                  ORDER BY o.order_id DESC";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvOrders.DataSource = dt;
                gvOrders.DataBind();
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
                    oi.order_item_id AS ORDER_ITEM_ID,
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

        void LoadOrderSummary()
        {
            lblTotalOrders.Text =
                GetSingleValue("SELECT COUNT(*) FROM ORDERS");

            lblTotalSales.Text =
                GetSingleValue("SELECT NVL(SUM(total_amount), 0) FROM ORDERS WHERE UPPER(order_status) <> 'CANCELLED'");

            lblTotalItems.Text =
                GetSingleValue("SELECT NVL(SUM(quantity), 0) FROM ORDER_ITEMS");

            lblPendingOrders.Text =
                GetSingleValue("SELECT COUNT(*) FROM ORDERS WHERE UPPER(order_status) = 'PENDING'");
        }

        string GetSingleValue(string query)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                con.Open();

                OracleCommand cmd = new OracleCommand(query, con);

                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    return "0";
                }

                return result.ToString();
            }
        }

        void LoadPopularItems()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    m.item_name AS ITEM_NAME,
                    SUM(oi.quantity) AS TOTAL_QUANTITY,
                    SUM(oi.subtotal) AS TOTAL_SALES
                  FROM ORDER_ITEMS oi
                  INNER JOIN MENU_ITEMS m
                    ON oi.item_id = m.item_id
                  INNER JOIN ORDERS o
                    ON oi.order_id = o.order_id
                  WHERE UPPER(o.order_status) <> 'CANCELLED'
                  GROUP BY m.item_name
                  ORDER BY TOTAL_QUANTITY DESC";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvPopularItems.DataSource = dt;
                gvPopularItems.DataBind();
            }
        }


        protected void btnCreateOrderWithItem_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            try
            {
                if (ddlCustomer.SelectedValue == "")
                {
                    lblMessage.Text = "Please select a customer.";
                    return;
                }

                if (ddlTable.SelectedValue == "")
                {
                    lblMessage.Text = "Please select a table.";
                    return;
                }

                if (ddlMenuItem.SelectedValue == "")
                {
                    lblMessage.Text = "Please select a food item.";
                    return;
                }

                int quantity = Convert.ToInt32(txtQuantity.Text);

                if (quantity <= 0)
                {
                    lblMessage.Text = "Quantity must be greater than zero.";
                    return;
                }

                int selectedTableID = Convert.ToInt32(ddlTable.SelectedValue);
                int orderID = 0;

                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    con.Open();

                    if (hfOrderID.Value == "")
                    {
                        OracleCommand cmd = new OracleCommand("AddOrder", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("p_customer_id", OracleDbType.Int32)
                            .Value = Convert.ToInt32(ddlCustomer.SelectedValue);

                        cmd.Parameters.Add("p_table_id", OracleDbType.Int32)
                            .Value = selectedTableID;

                        cmd.Parameters.Add("p_staff_id", OracleDbType.Int32)
                            .Value = Convert.ToInt32(Session["StaffID"]);

                        cmd.Parameters.Add("p_order_status", OracleDbType.Varchar2)
                            .Value = ddlOrderStatus.SelectedValue;

                        OracleParameter outputOrderID =
                            new OracleParameter("p_order_id", OracleDbType.Int32);

                        outputOrderID.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(outputOrderID);

                        cmd.ExecuteNonQuery();

                        orderID = Convert.ToInt32(outputOrderID.Value.ToString());

                        hfOrderID.Value = orderID.ToString();

                        lblSelectedOrder.Text =
                            "Selected Order #" + orderID;
                    }
                    else
                    {
                        orderID = Convert.ToInt32(hfOrderID.Value);

                        OracleCommand cmd = new OracleCommand("UpdateOrder", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("p_order_id", OracleDbType.Int32)
                            .Value = orderID;

                        cmd.Parameters.Add("p_customer_id", OracleDbType.Int32)
                            .Value = Convert.ToInt32(ddlCustomer.SelectedValue);

                        cmd.Parameters.Add("p_table_id", OracleDbType.Int32)
                            .Value = selectedTableID;

                        cmd.Parameters.Add("p_order_status", OracleDbType.Varchar2)
                            .Value = ddlOrderStatus.SelectedValue;

                        cmd.ExecuteNonQuery();
                    }

                    OracleCommand itemCmd = new OracleCommand("AddOrderItem", con);
                    itemCmd.CommandType = CommandType.StoredProcedure;
                    itemCmd.BindByName = true;

                    itemCmd.Parameters.Add("p_order_id", OracleDbType.Int32)
                        .Value = orderID;

                    itemCmd.Parameters.Add("p_item_id", OracleDbType.Int32)
                        .Value = Convert.ToInt32(ddlMenuItem.SelectedValue);

                    itemCmd.Parameters.Add("p_quantity", OracleDbType.Int32)
                        .Value = quantity;

                    itemCmd.ExecuteNonQuery();
                }

                txtQuantity.Text = "1";
                ddlMenuItem.SelectedIndex = 0;

                lblMessage.Text = "Order and food item saved successfully.";

                LoadTables(selectedTableID);
                ddlTable.SelectedValue = selectedTableID.ToString();

                LoadOrders();
                LoadOrderItems(orderID);
                LoadOrderSummary();
                LoadPopularItems();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

