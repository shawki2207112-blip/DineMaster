<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Billing.aspx.cs" Inherits="DineMaster.Billing" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Billing</title>

    <link href="CSS/bill.css" rel="stylesheet" />
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

    <div class="container">

        <div class="form-card">

            <h2>Generate Bill</h2>

            <asp:HiddenField ID="hfBillID" runat="server" />

            <div class="input-group">
                <label>Select Order</label>

                <asp:DropDownList ID="ddlOrder"
                    runat="server"
                    CssClass="input"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlOrder_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <div class="bill-info-box">

                <p>
                    <strong>Customer:</strong>
                    <asp:Label ID="lblCustomerName" runat="server" Text="-"></asp:Label>
                </p>

                <p>
                    <strong>Table:</strong>
                    <asp:Label ID="lblTableNumber" runat="server" Text="-"></asp:Label>
                </p>

                <p>
                    <strong>Order Status:</strong>
                    <asp:Label ID="lblOrderStatus" runat="server" Text="-"></asp:Label>
                </p>

                <p>
                    <strong>Total Amount:</strong>
                    Tk <asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label>
                </p>

            </div>

            <div class="button-area">

                <asp:Button ID="btnGenerateBill"
                    runat="server"
                    Text="Generate Bill"
                    CssClass="save-btn"
                    OnClick="btnGenerateBill_Click" />

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

        <div class="grid-card">

            <h2>Selected Order Items</h2>

            <asp:GridView ID="gvOrderItems"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="customer-grid">

                <Columns>

                    <asp:BoundField DataField="ITEM_NAME" HeaderText="Food Item" />
                    <asp:BoundField DataField="CATEGORY" HeaderText="Category" />
                    <asp:BoundField DataField="QUANTITY" HeaderText="Quantity" />
                    <asp:BoundField DataField="UNIT_PRICE" HeaderText="Unit Price" />
                    <asp:BoundField DataField="SUBTOTAL" HeaderText="Subtotal" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

    <div class="bill-list-card">

        <h2>Generated Bills</h2>

        <asp:GridView ID="gvBills"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="customer-grid">

            <Columns>

                <asp:BoundField DataField="BILL_ID" HeaderText="Bill ID" />
                <asp:BoundField DataField="ORDER_ID" HeaderText="Order ID" />
                <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Customer" />
                <asp:BoundField DataField="BILL_DATE" HeaderText="Bill Date" />
                <asp:BoundField DataField="TOTAL_BILL" HeaderText="Total Bill" />
                <asp:BoundField DataField="PAYMENT_STATUS" HeaderText="Payment Status" />

            </Columns>

        </asp:GridView>

    </div>

</form>

</body>
</html>