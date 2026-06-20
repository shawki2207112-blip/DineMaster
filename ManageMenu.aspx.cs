using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DineMaster
{
    public partial class ManageMenu : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(
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
            SqlDataAdapter da =
                new SqlDataAdapter("SELECT * FROM MENU_ITEMS", con);

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
                (item_name, category, price, availability)
                VALUES
                (@name,@category,@price,@availability)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@name", txtItemName.Text);
                cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@availability", ddlAvailability.SelectedValue);

                cmd.ExecuteNonQuery();

                lblMessage.Text = "Item Added Successfully";
            }
            else
            {
                string query =
                @"UPDATE MENU_ITEMS
                SET item_name=@name,
                    category=@category,
                    price=@price,
                    availability=@availability
                WHERE item_id=@id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@id", hfItemID.Value);
                cmd.Parameters.AddWithValue("@name", txtItemName.Text);
                cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                cmd.Parameters.AddWithValue("@availability", ddlAvailability.SelectedValue);

                cmd.ExecuteNonQuery();

                lblMessage.Text = "Item Updated Successfully";
            }

            con.Close();

            ClearFields();
            LoadMenu();
        }

        protected void gvMenu_RowCommand(object sender,
            System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int rowIndex =
                Convert.ToInt32(e.CommandArgument);

            int itemID =
                Convert.ToInt32(gvMenu.DataKeys[rowIndex].Value);

            if (e.CommandName == "DeleteRow")
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                    "DELETE FROM MENU_ITEMS WHERE item_id=@id",
                    con);

                cmd.Parameters.AddWithValue("@id", itemID);

                cmd.ExecuteNonQuery();

                con.Close();

                LoadMenu();
            }

            if (e.CommandName == "EditRow")
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                    "SELECT * FROM MENU_ITEMS WHERE item_id=@id",
                    con);

                cmd.Parameters.AddWithValue("@id", itemID);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    hfItemID.Value =
                        dr["item_id"].ToString();

                    txtItemName.Text =
                        dr["item_name"].ToString();

                    txtCategory.Text =
                        dr["category"].ToString();

                    txtPrice.Text =
                        dr["price"].ToString();

                    ddlAvailability.SelectedValue =
                        dr["availability"].ToString();
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
        }
    }
}