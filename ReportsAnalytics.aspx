<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportsAnalytics.aspx.cs" Inherits="DineMaster.ReportsAnalytics" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Reports & Analytics</title>

    <link href="CSS/report.css" rel="stylesheet" />
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

    <div class="summary-row">

        <div class="summary-card">
            <h3>Sales Count</h3>
            <asp:Label ID="lblTodaySalesCount" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Today's Revenue</h3>
            Tk <asp:Label ID="lblTodayRevenue" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Orders Found</h3>
            <asp:Label ID="lblOrderStatusCount" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Reservations Found</h3>
            <asp:Label ID="lblReservationStatusCount" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Best Seller Staff</h3>
            <asp:Label ID="lblBestSellerStaff" runat="server" Text="-"></asp:Label>
        </div>

    </div>

    <div class="filter-card">

        <h2>Report Filters</h2>

        <div class="filter-row">

            <div class="filter-group">
                <label>Today's Sales Date</label>

                <asp:TextBox ID="txtSalesDate"
                    runat="server"
                    TextMode="Date"
                    CssClass="input">
                </asp:TextBox>
            </div>

            <div class="filter-group">
                <label>Order Status</label>

                <asp:DropDownList ID="ddlOrderStatus"
                    runat="server"
                    CssClass="input">

                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Pending">Pending</asp:ListItem>
                    <asp:ListItem Value="Preparing">Preparing</asp:ListItem>
                    <asp:ListItem Value="Served">Served</asp:ListItem>
                    <asp:ListItem Value="Completed">Completed</asp:ListItem>
                    <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>

                </asp:DropDownList>
            </div>

            <div class="filter-group">
                <label>Reservation Status</label>

                <asp:DropDownList ID="ddlReservationStatus"
                    runat="server"
                    CssClass="input">

                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Reserved">Reserved</asp:ListItem>
                    <asp:ListItem Value="Completed">Completed</asp:ListItem>
                    <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>

                </asp:DropDownList>
            </div>

        </div>

        <asp:Button ID="btnLoadReports"
            runat="server"
            Text="Load Reports"
            CssClass="save-btn"
            OnClick="btnLoadReports_Click" />

        <asp:Label ID="lblMessage"
            runat="server"
            CssClass="message">
        </asp:Label>

    </div>

    <div class="report-card">

        <h2>Today's Sales Report</h2>

        <asp:GridView ID="gvTodaySales"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="report-grid">

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

    <div class="report-card">

        <h2>Order Status Report</h2>

        <asp:GridView ID="gvOrderStatus"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="report-grid">

            <Columns>

                <asp:BoundField DataField="ORDER_ID" HeaderText="Order ID" />
                <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Customer" />
                <asp:BoundField DataField="TABLE_NUMBER" HeaderText="Table" />
                <asp:BoundField DataField="ORDER_DATE" HeaderText="Order Date" />
                <asp:BoundField DataField="TOTAL_AMOUNT" HeaderText="Total Amount" />
                <asp:BoundField DataField="ORDER_STATUS" HeaderText="Status" />

            </Columns>

        </asp:GridView>

    </div>

    <div class="report-card">

        <h2>Reservation Status Report</h2>

        <asp:GridView ID="gvReservationStatus"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="report-grid">

            <Columns>

                <asp:BoundField DataField="RESERVATION_ID" HeaderText="Reservation ID" />
                <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Customer" />
                <asp:BoundField DataField="TABLE_NUMBER" HeaderText="Table" />
                <asp:BoundField DataField="RESERVATION_DATE" HeaderText="Date" />
                <asp:BoundField DataField="RESERVATION_TIME" HeaderText="Time" />
                <asp:BoundField DataField="NUMBER_OF_PEOPLE" HeaderText="People" />
                <asp:BoundField DataField="STATUS" HeaderText="Status" />

            </Columns>

        </asp:GridView>

    </div>

    <div class="report-card">

        <h2>Best Seller Staff</h2>

        <asp:GridView ID="gvBestSellerStaff"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="report-grid">

            <Columns>

                <asp:BoundField DataField="STAFF_ID" HeaderText="Staff ID" />
                <asp:BoundField DataField="STAFF_NAME" HeaderText="Staff Name" />
                <asp:BoundField DataField="TOTAL_BILLS" HeaderText="Bills Generated" />
                <asp:BoundField DataField="TOTAL_REVENUE" HeaderText="Total Revenue" />

            </Columns>

        </asp:GridView>

    </div>

</form>

</body>
</html>