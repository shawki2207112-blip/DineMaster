<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="DineMaster.AdminDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Admin Dashboard</title>
    <link href="CSS/dashboard.css" rel="stylesheet" />
</head>
<body>

<form id="form1" runat="server">

    <div class="header">
        <h1>DineMaster - Admin Dashboard</h1>

        <div class="user-section">
            Welcome,
            <asp:Label ID="lblAdminName" runat="server"></asp:Label>

            <asp:Button ID="btnLogout"
                runat="server"
                Text="Logout"
                CssClass="logout-btn"
                OnClick="btnLogout_Click" />
        </div>
    </div>

    <div class="dashboard-container">

        <div class="card">
            <h2>🍽️ Menu Management</h2>
            <p>Add, update and manage food items.</p>

            <asp:Button ID="btnMenu"
                runat="server"
                Text="Manage Menu"
                CssClass="card-btn"
                PostBackUrl="~/ManageMenu.aspx" />
        </div>

        <div class="card">
            <h2>🪑 Table Management</h2>
            <p>Manage restaurant tables and capacity.</p>

            <asp:Button ID="btnTables"
                runat="server"
                Text="Manage Tables"
                CssClass="card-btn"
                PostBackUrl="~/ManageTables.aspx" />
        </div>

        <div class="card">
            <h2>👨‍🍳 Staff Management</h2>
            <p>Add and maintain staff accounts.</p>

            <asp:Button ID="btnStaff"
                runat="server"
                Text="Manage Staff"
                CssClass="card-btn"
                PostBackUrl="~/ManageStaff.aspx" />
        </div>

        <div class="card">
            <h2>📊 Reports</h2>
            <p>View sales and restaurant reports.</p>

            <asp:Button ID="btnReports"
                runat="server"
                Text="View Reports"
                CssClass="card-btn"
                PostBackUrl="~/Reports.aspx" />
        </div>

    </div>

</form>

</body>
</html>