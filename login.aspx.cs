using System;
using System.Data.SqlClient;
using System.Configuration;

namespace DineMaster
{
    public partial class Login : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(
            ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString
        );

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                string query = "SELECT staff_id, staff_name, role FROM STAFF WHERE username=@username AND password=@password";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@password", txtPassword.Text);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    // session values
                    Session["StaffID"] = dr["staff_id"].ToString();
                    Session["StaffName"] = dr["staff_name"].ToString();
                    Session["Role"] = dr["role"].ToString();

                    string role = dr["role"].ToString();

                    if (role == "Admin")
                    {
                        Response.Redirect("AdminDashboard.aspx");
                    }
                    else
                    {
                        Response.Redirect("StaffDashboard.aspx");
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid username or password!";
                }

                con.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}