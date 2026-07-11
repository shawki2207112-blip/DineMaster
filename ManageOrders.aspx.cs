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



       