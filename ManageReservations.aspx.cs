using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class ManageReservations : System.Web.UI.Page
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
                    CreateReservationPLSQLObjects();

                    LoadCustomers();
                    LoadTables();
                    LoadReservations("All");
                    LoadReservationSummary();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                }
            }
        }


        void CreateReservationPLSQLObjects()
        {
            ExecuteDDL(@"
            CREATE OR REPLACE FUNCTION GetReservationStaffName
            (
                p_staff_id IN NUMBER
            )
            RETURN VARCHAR2
            IS
                v_staff_name STAFF.staff_name%TYPE;
            BEGIN
                SELECT staff_name
                INTO v_staff_name
                FROM STAFF
                WHERE staff_id = p_staff_id;

                RETURN v_staff_name;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    RETURN 'N/A';
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE TRIGGER trg_check_reservation_capacity
            BEFORE INSERT OR UPDATE OF table_id, number_of_people
            ON RESERVATIONS
            FOR EACH ROW
            DECLARE
                v_capacity RESTAURANT_TABLES.capacity%TYPE;
            BEGIN
                SELECT capacity
                INTO v_capacity
                FROM RESTAURANT_TABLES
                WHERE table_id = :NEW.table_id;

                IF :NEW.number_of_people IS NULL OR :NEW.number_of_people <= 0 THEN
                    RAISE_APPLICATION_ERROR(-20101, 'Number of people must be greater than zero');
                END IF;

                IF :NEW.number_of_people > v_capacity THEN
                    RAISE_APPLICATION_ERROR(-20102, 'Number of people exceeds table capacity');
                END IF;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE AddReservation
            (
                p_customer_id IN NUMBER,
                p_table_id IN NUMBER,
                p_staff_id IN NUMBER,
                p_reservation_date IN DATE,
                p_reservation_time IN VARCHAR2,
                p_number_of_people IN NUMBER,
                p_status IN VARCHAR2,
                p_reservation_id OUT NUMBER
            )
            AS
                v_table_status RESTAURANT_TABLES.status%TYPE;
            BEGIN
                SELECT status
                INTO v_table_status
                FROM RESTAURANT_TABLES
                WHERE table_id = p_table_id;

                IF UPPER(v_table_status) <> 'AVAILABLE' THEN
                    RAISE_APPLICATION_ERROR(-20103, 'Selected table is not available');
                END IF;

                SELECT RES_SEQ.NEXTVAL
                INTO p_reservation_id
                FROM dual;

                INSERT INTO RESERVATIONS
                (
                    reservation_id,
                    customer_id,
                    table_id,
                    staff_id,
                    reservation_date,
                    reservation_time,
                    number_of_people,
                    status
                )
                VALUES
                (
                    p_reservation_id,
                    p_customer_id,
                    p_table_id,
                    p_staff_id,
                    p_reservation_date,
                    p_reservation_time,
                    p_number_of_people,
                    p_status
                );

                IF UPPER(p_status) = 'RESERVED' THEN
                    UPDATE RESTAURANT_TABLES
                    SET status = 'Reserved'
                    WHERE table_id = p_table_id;
                ELSE
                    UPDATE RESTAURANT_TABLES
                    SET status = 'Available'
                    WHERE table_id = p_table_id;
                END IF;

                COMMIT;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE UpdateReservation
            (
                p_reservation_id IN NUMBER,
                p_customer_id IN NUMBER,
                p_table_id IN NUMBER,
                p_reservation_date IN DATE,
                p_reservation_time IN VARCHAR2,
                p_number_of_people IN NUMBER,
                p_status IN VARCHAR2
            )
            AS
                v_old_table_id RESERVATIONS.table_id%TYPE;
                v_new_table_status RESTAURANT_TABLES.status%TYPE;
            BEGIN
                SELECT table_id
                INTO v_old_table_id
                FROM RESERVATIONS
                WHERE reservation_id = p_reservation_id;

                IF v_old_table_id <> p_table_id THEN

                    SELECT status
                    INTO v_new_table_status
                    FROM RESTAURANT_TABLES
                    WHERE table_id = p_table_id;

                    IF UPPER(v_new_table_status) <> 'AVAILABLE' THEN
                        RAISE_APPLICATION_ERROR(-20104, 'New selected table is not available');
                    END IF;

                    UPDATE RESTAURANT_TABLES
                    SET status = 'Available'
                    WHERE table_id = v_old_table_id;

                END IF;

                UPDATE RESERVATIONS
                SET customer_id = p_customer_id,
                    table_id = p_table_id,
                    reservation_date = p_reservation_date,
                    reservation_time = p_reservation_time,
                    number_of_people = p_number_of_people,
                    status = p_status
                WHERE reservation_id = p_reservation_id;

                IF UPPER(p_status) = 'RESERVED' THEN
                    UPDATE RESTAURANT_TABLES
                    SET status = 'Reserved'
                    WHERE table_id = p_table_id;
                ELSE
                    UPDATE RESTAURANT_TABLES
                    SET status = 'Available'
                    WHERE table_id = p_table_id;
                END IF;

                COMMIT;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE CancelReservation
            (
                p_reservation_id IN NUMBER
            )
            AS
                v_table_id RESERVATIONS.table_id%TYPE;
            BEGIN
                SELECT table_id
                INTO v_table_id
                FROM RESERVATIONS
                WHERE reservation_id = p_reservation_id;

                UPDATE RESERVATIONS
                SET status = 'Cancelled'
                WHERE reservation_id = p_reservation_id;

                UPDATE RESTAURANT_TABLES
                SET status = 'Available'
                WHERE table_id = v_table_id;

                COMMIT;
            END;");

            ExecuteDDL(@"
            CREATE OR REPLACE PROCEDURE DeleteReservation
            (
                p_reservation_id IN NUMBER
            )
            AS
                v_table_id RESERVATIONS.table_id%TYPE;
            BEGIN
                SELECT table_id
                INTO v_table_id
                FROM RESERVATIONS
                WHERE reservation_id = p_reservation_id;

                DELETE FROM RESERVATIONS
                WHERE reservation_id = p_reservation_id;

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

        // LOAD DROPDOWNS

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



        void LoadReservations(string statusFilter)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    r.reservation_id AS RESERVATION_ID,
                    c.customer_name AS CUSTOMER_NAME,
                    t.table_number AS TABLE_NUMBER,
                    GetReservationStaffName(r.staff_id) AS STAFF_NAME,
                    TO_CHAR(r.reservation_date, 'DD-MON-YYYY') AS RESERVATION_DATE,
                    r.reservation_time AS RESERVATION_TIME,
                    r.number_of_people AS NUMBER_OF_PEOPLE,
                    r.status AS STATUS
                  FROM RESERVATIONS r
                  INNER JOIN CUSTOMERS c
                    ON r.customer_id = c.customer_id
                  INNER JOIN RESTAURANT_TABLES t
                    ON r.table_id = t.table_id
                  WHERE (:status_filter = 'All' OR r.status = :status_filter)
                  ORDER BY r.reservation_id DESC";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":status_filter", OracleDbType.Varchar2)
                    .Value = statusFilter;

                OracleDataAdapter da = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvReservations.DataSource = dt;
                gvReservations.DataBind();
            }
        }

        void LoadReservationSummary()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                string query =
                @"SELECT
                    COUNT(*) AS TOTAL_RESERVATIONS,
                    SUM(CASE WHEN UPPER(status) = 'RESERVED' THEN 1 ELSE 0 END) AS ACTIVE_RESERVATIONS,
                    SUM(CASE WHEN UPPER(status) = 'CANCELLED' THEN 1 ELSE 0 END) AS CANCELLED_RESERVATIONS,
                    NVL(SUM(CASE WHEN UPPER(status) <> 'CANCELLED' THEN number_of_people ELSE 0 END), 0) AS TOTAL_GUESTS
                  FROM RESERVATIONS";

                OracleDataAdapter da = new OracleDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    lblTotalReservations.Text =
                        dt.Rows[0]["TOTAL_RESERVATIONS"].ToString();

                    lblActiveReservations.Text =
                        dt.Rows[0]["ACTIVE_RESERVATIONS"].ToString();

                    lblCancelledReservations.Text =
                        dt.Rows[0]["CANCELLED_RESERVATIONS"].ToString();

                    lblTotalGuests.Text =
                        dt.Rows[0]["TOTAL_GUESTS"].ToString();
                }
            }
        }

       

        
    }
}