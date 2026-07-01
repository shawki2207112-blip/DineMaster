<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageCustomers.aspx.cs" Inherits="DineMaster.ManageCustomers" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Customer Management</title>

<link href="CSS/customer.css" rel="stylesheet" />


</head>

<body>

<form id="form1" runat="server">


<div class="navbar">

    <div class="logo">
        🍽 DineMaster
    </div>

    <div class="nav-links">
        <a href="StaffDashboard.aspx">Dashboard</a>
        <a href="ManageCustomers.aspx">Customers</a>
        <a href="ManageReservations.aspx">Reservations</a>
        <a href="ManageOrders.aspx">Orders</a>
    </div>

</div>

<div class="container">

    <!-- Customer Form -->

    <div class="form-card">

        <h2>Customer Management</h2>

        <asp:HiddenField ID="hfCustomerID" runat="server" />

        <div class="input-group">
            <label>Customer Name</label>

            <asp:TextBox ID="txtCustomerName"
                runat="server"
                CssClass="input">
            </asp:TextBox>
        </div>

        <div class="input-group">
            <label>Phone</label>

            <asp:TextBox ID="txtPhone"
                runat="server"
                CssClass="input">
            </asp:TextBox>
        </div>

        <div class="input-group">
            <label>Email</label>

            <asp:TextBox ID="txtEmail"
                runat="server"
                CssClass="input">
            </asp:TextBox>
        </div>

        <div class="button-area">

            <asp:Button ID="btnSave"
                runat="server"
                Text="Save Customer"
                CssClass="save-btn"
                OnClick="btnSave_Click" />

            <asp:Button ID="btnClear"
                runat="server"
                Text="Clear"
                CssClass="clear-btn"
                OnClick="btnClear_Click" />

        </div>

        <asp:Label ID="lblMessage"
            runat="server"
            CssClass="message">
        </asp:Label>

    </div>

    <!-- Customer Grid -->

    <div class="grid-card">

        <h2>Customer List</h2>

        <asp:GridView ID="gvCustomers"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="customer-grid"
            DataKeyNames="customer_id"
            OnRowCommand="gvCustomers_RowCommand">

            <Columns>

                <asp:BoundField
                    DataField="customer_id"
                    HeaderText="ID" />

                <asp:BoundField
                    DataField="customer_name"
                    HeaderText="Name" />

                <asp:BoundField
                    DataField="phone"
                    HeaderText="Phone" />

                <asp:BoundField
                    DataField="email"
                    HeaderText="Email" />

                <asp:ButtonField
                    Text="Edit"
                    CommandName="EditRow" />

                <asp:ButtonField
                    Text="Delete"
                    CommandName="DeleteRow" />

            </Columns>

        </asp:GridView>

    </div>

</div>

<!-- Analytics Section -->

<div class="analytics-card">

    <h2>Customer Analytics</h2>

    <asp:GridView ID="gvCustomerAnalytics"
        runat="server"
        AutoGenerateColumns="True"
        CssClass="customer-grid">
    </asp:GridView>

</div>


</form>

</body>
</html>
