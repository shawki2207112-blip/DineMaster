<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageOrders.aspx.cs" Inherits="DineMaster.ManageOrders" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Order Management</title>
    <link href="CSS/Order.css" rel="stylesheet" />
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
            <a href="Billing.aspx">Billing</a>
        </div>

    </div>

    <div class="summary-row">

        <div class="summary-card">
            <h3>Total Orders</h3>
            <asp:Label ID="lblTotalOrders" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Total Sales</h3>
            Tk <asp:Label ID="lblTotalSales" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Total Items</h3>
            <asp:Label ID="lblTotalItems" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Pending Orders</h3>
            <asp:Label ID="lblPendingOrders" runat="server" Text="0"></asp:Label>
        </div>

    </div>

    <div class="order-layout" style="width:90%; margin:30px auto; display:flex; flex-direction:row; gap:25px; align-items:flex-start;">

        <div class="form-card" style="width:330px; flex:0 0 330px;">

            <h2>Create / Update Order</h2>

            <asp:HiddenField ID="hfOrderID" runat="server" />

            <div class="selected-box">
                <asp:Label ID="lblSelectedOrder"
                    runat="server"
                    Text="No order selected">
                </asp:Label>
            </div>

            <div class="input-group">
                <label>Customer</label>

                <asp:DropDownList ID="ddlCustomer"
                    runat="server"
                    CssClass="input">
                </asp:DropDownList>
            </div>

            <div class="input-group">
                <label>Table</label>

                <asp:DropDownList ID="ddlTable"
                    runat="server"
                    CssClass="input">
                </asp:DropDownList>
            </div>

            <div class="input-group">
                <label>Order Status</label>

                <asp:DropDownList ID="ddlOrderStatus"
                    runat="server"
                    CssClass="input">

                    <asp:ListItem>Pending</asp:ListItem>
                    <asp:ListItem>Preparing</asp:ListItem>
                    <asp:ListItem>Served</asp:ListItem>
                    <asp:ListItem>Completed</asp:ListItem>
                    <asp:ListItem>Cancelled</asp:ListItem>

                </asp:DropDownList>
            </div>

            <hr />

            <h2>Add Food Item</h2>

            <div class="input-group">
                <label>Food Item</label>

                <asp:DropDownList ID="ddlMenuItem"
                    runat="server"
                    CssClass="input">
                </asp:DropDownList>
            </div>

            <div class="input-group">
                <label>Quantity</label>

                <asp:TextBox ID="txtQuantity"
                    runat="server"
                    CssClass="input"
                    Text="1">
                </asp:TextBox>
            </div>

            <div class="button-area">

                <asp:Button ID="btnCreateOrderWithItem"
                    runat="server"
                    Text="Save Order & Add Item"
                    CssClass="save-btn"
                    OnClick="btnCreateOrderWithItem_Click" />

                <asp:Button ID="btnClear"
                    runat="server"
                    Text="Clear"
                    CssClass="clear-btn"
                    OnClick="btnClear_Click" />

            </div>

            <asp:Button ID="btnCancelOrder"
                runat="server"
                Text="Cancel Selected Order"
                CssClass="cancel-btn"
                OnClick="btnCancelOrder_Click" />

            <asp:Label ID="lblMessage"
                runat="server"
                CssClass="message">
            </asp:Label>

        </div>

        <div class="grid-card" style="flex:1; min-width:0;">

            <h2>Order List</h2>

            <asp:GridView ID="gvOrders"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="order-grid"
                DataKeyNames="ORDER_ID"
                OnRowCommand="gvOrders_RowCommand">

                <Columns>

                    <asp:BoundField DataField="ORDER_ID" HeaderText="Order ID" />
                    <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Customer" />
                    <asp:BoundField DataField="TABLE_NUMBER" HeaderText="Table" />
                    <asp:BoundField DataField="ORDER_DATE" HeaderText="Order Date" />
                    <asp:BoundField DataField="TOTAL_AMOUNT" HeaderText="Total Amount" />
                    <asp:BoundField DataField="ORDER_STATUS" HeaderText="Status" />

                    <asp:ButtonField Text="View / Edit" CommandName="EditRow" />
                    <asp:ButtonField Text="Delete" CommandName="DeleteRow" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

    <div class="report-card">

        <h2>Selected Order Items</h2>

        <asp:GridView ID="gvOrderItems"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="order-grid">

            <Columns>

                <asp:BoundField DataField="ORDER_ITEM_ID" HeaderText="Order Item ID" />
                <asp:BoundField DataField="ITEM_NAME" HeaderText="Food Item" />
                <asp:BoundField DataField="CATEGORY" HeaderText="Category" />
                <asp:BoundField DataField="QUANTITY" HeaderText="Quantity" />
                <asp:BoundField DataField="UNIT_PRICE" HeaderText="Unit Price" />
                <asp:BoundField DataField="SUBTOTAL" HeaderText="Subtotal" />

            </Columns>

        </asp:GridView>

    </div>

    <div class="report-card">

        <h2>Popular Food Items</h2>

        <asp:GridView ID="gvPopularItems"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="order-grid">

            <Columns>

                <asp:BoundField DataField="ITEM_NAME" HeaderText="Food Item" />
                <asp:BoundField DataField="TOTAL_QUANTITY" HeaderText="Total Quantity" />
                <asp:BoundField DataField="TOTAL_SALES" HeaderText="Total Sales" />

            </Columns>

        </asp:GridView>

    </div>

</form>

</body>
</html>