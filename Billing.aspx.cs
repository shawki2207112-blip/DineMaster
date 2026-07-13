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


        
    }
}