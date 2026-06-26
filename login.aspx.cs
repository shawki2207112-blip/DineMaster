using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DineMaster
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(
                    ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT staff_id, staff_name, role
                        FROM STAFF
                        WHERE username = :username
                        AND password = :password";

                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":username", txtUsername.Text.Trim());
                        cmd.Parameters.Add(":password", txtPassword.Text.Trim());

                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }
    }
}