using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class ManageMenu : System.Web.UI.Page
    {
        OracleConnection con = new OracleConnection(
            ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMenu();
            }
        }

        void LoadMenu()
        {
            OracleDataAdapter da =
                new OracleDataAdapter("SELECT * FROM MENU_ITEMS", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            gvMenu.DataSource = dt;
            gvMenu.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            con.Open();

            if (hfItemID.Value == "")
            {
                string query =
                @"INSERT INTO MENU_ITEMS
                (item_id, item_name, category, price, availability)
                VALUES
                (menu_seq.NEXTVAL, :name, :category, :price, :availability)";

                OracleCommand cmd = new OracleCommand(query, con);

                cmd.Parameters.Add(":name", txtItemName.Text);
                cmd.Parameters.Add(":category", txtCategory.Text);
                cmd.Parameters.Add(":price", txtPrice.Text);
                cmd.Parameters.Add(":availability", ddlAvailability.SelectedValue);

                cmd.ExecuteNonQuery();

                OracleCommand commitCmd = new OracleCommand("COMMIT", con);
                commitCmd.ExecuteNonQuery();

                lblMessage.Text = "Item Added Successfully";
            }
            else
            {
                string query =
                @"UPDATE MENU_ITEMS
                SET item_name = :name,
                    category = :category,
                    price = :price,
                    availability = :availability
                WHERE item_id = :id";

                OracleCommand cmd = new OracleCommand(query, con);

                cmd.BindByName = true;

                cmd.Parameters.Add(":id", hfItemID.Value);
                cmd.Parameters.Add(":name", txtItemName.Text);
                cmd.Parameters.Add(":category", txtCategory.Text);
                cmd.Parameters.Add(":price", txtPrice.Text);
                cmd.Parameters.Add(":availability", ddlAvailability.SelectedValue);

                cmd.ExecuteNonQuery();

                OracleCommand commitCmd = new OracleCommand("COMMIT", con);
                commitCmd.ExecuteNonQuery();

                lblMessage.Text = "Item Updated Successfully";
            }

            con.Close();

            ClearFields();
            LoadMenu();
        }

        protected void gvMenu_RowCommand(object sender,
            System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int itemID = Convert.ToInt32(gvMenu.DataKeys[rowIndex].Value);

            if (e.CommandName == "DeleteRow")
            {
                lblMessage.Text = "";
                con.Open();

                OracleCommand cmd =
                    new OracleCommand(
                    "DELETE FROM MENU_ITEMS WHERE item_id = :id", con);

                cmd.Parameters.Add(":id", itemID);

                cmd.ExecuteNonQuery();

                OracleCommand commitCmd = new OracleCommand("COMMIT", con);
                commitCmd.ExecuteNonQuery();

                con.Close();

                LoadMenu();
            }

            if (e.CommandName == "EditRow")
            {
                con.Open();
                lblMessage.Text = "";

                OracleCommand cmd =
                    new OracleCommand(
                    "SELECT * FROM MENU_ITEMS WHERE item_id = :id", con);

                cmd.Parameters.Add(":id", itemID);

                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    hfItemID.Value = dr["item_id"].ToString();
                    txtItemName.Text = dr["item_name"].ToString();
                    txtCategory.Text = dr["category"].ToString();
                    txtPrice.Text = dr["price"].ToString();
                    ddlAvailability.SelectedValue = dr["availability"].ToString();
                }

                con.Close();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        void ClearFields()
        {
            hfItemID.Value = "";
            txtItemName.Text = "";
            txtCategory.Text = "";
            txtPrice.Text = "";
            ddlAvailability.SelectedIndex = 0;
            lblMessage.Text = "";
        }
    }
}