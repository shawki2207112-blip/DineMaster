<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageStaff.aspx.cs" Inherits="DineMaster.ManageStaff" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Staff Management</title>

    <link href="CSS/staff.css" rel="stylesheet" />
</head>

<body>

<form id="form1" runat="server">

    <!-- Navbar -->

    <div class="navbar">

        <div class="logo">
            🍽 DineMaster
        </div>

        <div class="nav-links">
            <a href="AdminDashboard.aspx">Dashboard</a>
            <a href="ManageMenu.aspx">Menu</a>
            <a href="ManageTables.aspx">Tables</a>
            <a href="ManageStaff.aspx">Staff</a>
            <a href="Reports.aspx">Reports</a>
        </div>

    </div>

    <div class="container">

        <!-- Form -->

        <div class="form-card">

            <h2>Staff Management</h2>

            <asp:HiddenField ID="hfStaffID" runat="server" />

            <div class="input-group">
                <label>Staff Name</label>
                <asp:TextBox ID="txtStaffName"
                    runat="server"
                    CssClass="input" Width="225px" />
            </div>

            <div class="input-group">
                <label>Phone</label>
                <asp:TextBox ID="txtPhone"
                    runat="server"
                    CssClass="input" Width="225px" />
            </div>

            <div class="input-group">
                <label>Salary</label>
                <asp:TextBox ID="txtSalary"
                    runat="server"
                    CssClass="input" Width="225px" />
            </div>

            <div class="input-group">
                <label>Username</label>
                <asp:TextBox ID="txtUsername"
                    runat="server"
                    CssClass="input" Width="225px" />
            </div>

            <div class="input-group">
                <label>Password</label>
                <asp:TextBox ID="txtPassword"
                    runat="server"
                    CssClass="input"
                    TextMode="Password" Width="225px" />
            </div>

            <div class="button-area">

                <asp:Button ID="btnSave"
                    runat="server"
                    Text="Save Staff"
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

        <!-- Grid -->

        <div class="grid-card">

            <h2>Staff List</h2>

            <asp:GridView ID="gvStaff"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="staff-grid"
                DataKeyNames="staff_id"
                OnRowCommand="gvStaff_RowCommand">

                <Columns>

                    <asp:BoundField DataField="staff_id" HeaderText="ID" />
                    <asp:BoundField DataField="staff_name" HeaderText="Name" />
                    <asp:BoundField DataField="phone" HeaderText="Phone" />
                    <asp:BoundField DataField="salary" HeaderText="Salary" />
                    <asp:BoundField DataField="role" HeaderText="Role" />
                    <asp:BoundField DataField="username" HeaderText="Username" />

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

</form>

</body>
</html>