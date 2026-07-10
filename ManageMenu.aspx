<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageMenu.aspx.cs" Inherits="DineMaster.ManageMenu" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Menu Management</title>
    <link href="CSS/menu.css" rel="stylesheet" />
</head>

<body>

<form id="form1" runat="server">

    <div class="navbar">

        <div class="logo">
            🍽 DineMaster
        </div>

        <div class="nav-links">
            <a href="AdminDashboard.aspx">Dashboard</a>
            <a href="ManageMenu.aspx">Menu</a>
            <a href="ManageTables.aspx">Tables</a>
            <a href="ManageStaff.aspx">Staff</a>
            <a href="ReportsAnalytics.aspx">Reports</a>
        </div>

    </div>

    <div class="menu-layout" style="width:90%; margin:30px auto; display:flex; flex-direction:row; gap:25px; align-items:flex-start;">

        <div class="form-card" style="width:320px; flex:0 0 320px;">

            <h2>Add / Update Menu Item</h2>

            <asp:HiddenField ID="hfItemID" runat="server" />

            <div class="input-group">
                <label>Item Name</label>

                <asp:TextBox ID="txtItemName"
                    runat="server"
                    CssClass="input">
                </asp:TextBox>
            </div>

            <div class="input-group">
                <label>Category</label>

                <asp:TextBox ID="txtCategory"
                    runat="server"
                    CssClass="input">
                </asp:TextBox>
            </div>

            <div class="input-group">
                <label>Price</label>

                <asp:TextBox ID="txtPrice"
                    runat="server"
                    CssClass="input">
                </asp:TextBox>
            </div>

            <div class="input-group">
                <label>Availability</label>

                <asp:DropDownList ID="ddlAvailability"
                    runat="server"
                    CssClass="input">

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

            <asp:Label ID="lblMessage"
                runat="server"
                CssClass="message">
            </asp:Label>

        </div>

        <div class="grid-card" style="flex:1; min-width:0;">

            <h2>Menu Items</h2>

            <div class="search-box">

                <asp:TextBox ID="txtSearchItem"
                    runat="server"
                    CssClass="input search-input"
                    Placeholder="Search by item name">
                </asp:TextBox>

                <asp:Button ID="btnSearch"
                    runat="server"
                    Text="Search"
                    CssClass="save-btn"
                    OnClick="btnSearch_Click" />

                <asp:Button ID="btnShowAll"
                    runat="server"
                    Text="Show All"
                    CssClass="clear-btn"
                    OnClick="btnShowAll_Click" />

            </div>

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