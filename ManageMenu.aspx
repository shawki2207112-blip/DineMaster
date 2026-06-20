<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageMenu.aspx.cs" Inherits="DineMaster.ManageMenu" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Menu Management</title>
    <link href="CSS/menu.css" rel="stylesheet" />
</head>

<body>

<form id="form1" runat="server">

    <!-- Header -->
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

    <!-- Main Container -->
    <div class="container">

        <!-- Form Card -->
        <div class="form-card">

            <h2>Add / Update Menu Item</h2>


            <asp:HiddenField ID="hfItemID" runat="server" />

            <div class="input-group">

                <label>Item Name</label>

                <asp:TextBox ID="txtItemName"
                    runat="server"
                    CssClass="input" Width="220px"></asp:TextBox>

            </div>

            <div class="input-group">

                <label>Category</label>

                <asp:TextBox ID="txtCategory"
                    runat="server"
                    CssClass="input" Width="220px"></asp:TextBox>

            </div>

            <div class="input-group">

                <label>Price</label>

                <asp:TextBox ID="txtPrice"
                    runat="server"
                    CssClass="input" Width="220px"></asp:TextBox>

            </div>

            <div class="input-group">

                <label>Availability</label>

                <asp:DropDownList ID="ddlAvailability"
                    runat="server"
                    CssClass="input" Width="166px">

                    <asp:ListItem>Available</asp:ListItem>
                    <asp:ListItem>Unavailable</asp:ListItem>

                </asp:DropDownList>

            </div>

            <div class="button-area">

                <asp:Button ID="btnSave"
                    runat="server"
                    Text="Save Item"
                    CssClass="save-btn"
                    OnClick="btnSave_Click" />

                <asp:Button ID="btnClear"
                    runat="server"
                    Text="Clear"
                    CssClass="clear-btn"
                    OnClick="btnClear_Click" />

            </div>
            <br />

            <asp:Label ID="lblMessage"
                runat="server"
                CssClass="message">
            </asp:Label>

        </div>

        <!-- Grid Card -->

        <div class="grid-card">

            <h2>Menu Items</h2>

            <asp:GridView ID="gvMenu"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="menu-grid"
                DataKeyNames="item_id"
                OnRowCommand="gvMenu_RowCommand">

                <Columns>

                    <asp:BoundField DataField="item_id" HeaderText="ID" />
                    <asp:BoundField DataField="item_name" HeaderText="Item Name" />
                    <asp:BoundField DataField="category" HeaderText="Category" />
                    <asp:BoundField DataField="price" HeaderText="Price" />
                    <asp:BoundField DataField="availability" HeaderText="Status" />

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