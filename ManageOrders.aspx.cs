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


        }



       