<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageTables.aspx.cs" Inherits="DineMaster.ManageTables" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Table Management</title>

    <link href="CSS/table.css" rel="stylesheet" />
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

        <!-- Form Card -->

        <div class="form-card">

            <h2>Table Management</h2>

            <asp:HiddenField ID="hfTableID" runat="server" />

            <div class="input-group">

                <label>Table Number</label>

                <asp:TextBox ID="txtTableNumber"
                    runat="server"
                    CssClass="input" Width="255px"></asp:TextBox>

            </div>

            <div class="input-group">

                <label>Capacity</label>

                <asp:TextBox ID="txtCapacity"
                    runat="server"
                    CssClass="input" Width="255px"></asp:TextBox>

            </div>

            <div class="input-group">

                <label>Status</label>

                <asp:DropDownList ID="ddlStatus"
                    runat="server"
                    CssClass="input" Width="255px">

                    <asp:ListItem>Available</asp:ListItem>
                    <asp:ListItem>Reserved</asp:ListItem>
                    <asp:ListItem>Occupied</asp:ListItem>

                </asp:DropDownList>

            </div>

            <div class="button-area">

                <asp:Button ID="btnSave"
                    runat="server"
                    Text="Save Table"
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

            <h2>Restaurant Tables</h2>

            <asp:GridView ID="gvTables"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table-grid"
                DataKeyNames="table_id"
                OnRowCommand="gvTables_RowCommand">

                <Columns>

                    <asp:BoundField DataField="table_id"
                        HeaderText="ID" />

                    <asp:BoundField DataField="table_number"
                        HeaderText="Table Number" />

                    <asp:BoundField DataField="capacity"
                        HeaderText="Capacity" />

                    <asp:BoundField DataField="status"
                        HeaderText="Status" />

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