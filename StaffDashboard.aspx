<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffDashboard.aspx.cs" Inherits="DineMaster.StaffDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Staff Dashboard</title>
    <link href="CSS/dashboard.css" rel="stylesheet" />
</head>

<body>

<form id="form1" runat="server">

    <div class="header">
        <h1>DineMaster - Staff Dashboard</h1>

        <div class="user-section">
            Welcome,
            <asp:Label ID="lblStaffName" runat="server"></asp:Label>

            <asp:Button ID="btnLogout"
                runat="server"
                Text="Logout"
                CssClass="logout-btn"
                OnClick="btnLogout_Click" />
        </div>
    </div>

    <div class="dashboard-container">

        <div class="card">
            <h2>👥 Customer Management</h2>

            <p>
                Add, update and maintain customer records.
            </p>

            <asp:Button ID="btnCustomers"
                runat="server"
                Text="Manage Customers"
                CssClass="card-btn"
                PostBackUrl="~/ManageCustomers.aspx" />
        </div>

        <div class="card">
            <h2>📅 Table Reservations</h2>

            <p>
                Reserve tables and manage customer bookings.
            </p>

            <asp:Button ID="btnReservations"
                runat="server"
                Text="Manage Reservations"
                CssClass="card-btn"
                PostBackUrl="~/ManageReservations.aspx" />
        </div>

        <div class="card">
            <h2>📝 Order Management</h2>

            <p>
                Create, update and manage customer orders.
            </p>

            <asp:Button ID="btnOrders"
                runat="server"
                Text="Manage Orders"
                CssClass="card-btn"
                PostBackUrl="~/ManageOrders.aspx" />
        </div>

        <div class="card">
            <h2>💳 Billing & Payment</h2>

            <p>
                Generate bills and receive payments.
            </p>

            <asp:Button ID="btnBilling"
                runat="server"
                Text="Billing System"
                CssClass="card-btn"
                PostBackUrl="~/Billing.aspx" />
        </div>

    </div>

</form>

</body>
</html>