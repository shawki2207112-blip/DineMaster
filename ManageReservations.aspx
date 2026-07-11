<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageReservations.aspx.cs" Inherits="DineMaster.ManageReservations" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Reservation Management</title>

    <link href="CSS/reservation.css" rel="stylesheet" />
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
            <h3>Total Reservations</h3>
            <asp:Label ID="lblTotalReservations" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Active Reservations</h3>
            <asp:Label ID="lblActiveReservations" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Cancelled Reservations</h3>
            <asp:Label ID="lblCancelledReservations" runat="server" Text="0"></asp:Label>
        </div>

        <div class="summary-card">
            <h3>Total Guests</h3>
            <asp:Label ID="lblTotalGuests" runat="server" Text="0"></asp:Label>
        </div>

    </div>

    <div class="container">

        <div class="form-card">

            <h2>Reservation Management</h2>

            <asp:HiddenField ID="hfReservationID" runat="server" />

            <div class="selected-box">
                <asp:Label ID="lblSelectedReservation"
                    runat="server"
                    Text="No reservation selected">
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
                <label>Reservation Date</label>

                <asp:TextBox ID="txtReservationDate"
                    runat="server"
                    CssClass="input"
                    TextMode="Date">
                </asp:TextBox>
            </div>

            <div class="input-group">
                <label>Reservation Time</label>

                <asp:TextBox ID="txtReservationTime"
                    runat="server"
                    CssClass="input"
                    TextMode="Time">
                </asp:TextBox>
            </div>

            <div class="input-group">
                <label>Number of People</label>

                <asp:TextBox ID="txtNumberOfPeople"
                    runat="server"
                    CssClass="input"
                    Text="1">
                </asp:TextBox>
            </div>

            <div class="input-group">
                <label>Status</label>

                <asp:DropDownList ID="ddlStatus"
                    runat="server"
                    CssClass="input">

                    <asp:ListItem>Reserved</asp:ListItem>
                    <asp:ListItem>Completed</asp:ListItem>
                    <asp:ListItem>Cancelled</asp:ListItem>

                </asp:DropDownList>
            </div>

            <div class="button-area">

                <asp:Button ID="btnSaveReservation"
                    runat="server"
                    Text="Save Reservation"
                    CssClass="save-btn"
                    OnClick="btnSaveReservation_Click" />

                <asp:Button ID="btnClear"
                    runat="server"
                    Text="Clear"
                    CssClass="clear-btn"
                    OnClick="btnClear_Click" />

            </div>

            <asp:Button ID="btnCancelReservation"
                runat="server"
                Text="Cancel Selected Reservation"
                CssClass="cancel-btn"
                OnClick="btnCancelReservation_Click" />

            <asp:Label ID="lblMessage"
                runat="server"
                CssClass="message">
            </asp:Label>

        </div>

        <div class="grid-card">

            <h2>Reservation List</h2>

            <div class="filter-box">

                <label>Filter by Status</label>

                <asp:DropDownList ID="ddlFilterStatus"
                    runat="server"
                    CssClass="filter-input">

                    <asp:ListItem Value="All">All</asp:ListItem>
                    <asp:ListItem Value="Reserved">Reserved</asp:ListItem>
                    <asp:ListItem Value="Completed">Completed</asp:ListItem>
                    <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>

                </asp:DropDownList>
                <br></br>

                <asp:Button ID="btnFilter"
                    runat="server"
                    Text="Filter"
                    CssClass="save-btn"
                    OnClick="btnFilter_Click" />

            </div>

            <asp:GridView ID="gvReservations"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="customer-grid"
                DataKeyNames="RESERVATION_ID"
                OnRowCommand="gvReservations_RowCommand">

                <Columns>

                    <asp:BoundField DataField="RESERVATION_ID" HeaderText="ID" />
                    <asp:BoundField DataField="CUSTOMER_NAME" HeaderText="Customer" />
                    <asp:BoundField DataField="TABLE_NUMBER" HeaderText="Table" />
                    <asp:BoundField DataField="STAFF_NAME" HeaderText="Staff" />
                    <asp:BoundField DataField="RESERVATION_DATE" HeaderText="Date" />
                    <asp:BoundField DataField="RESERVATION_TIME" HeaderText="Time" />
                    <asp:BoundField DataField="NUMBER_OF_PEOPLE" HeaderText="People" />
                    <asp:BoundField DataField="STATUS" HeaderText="Status" />

                    <asp:ButtonField
                        Text=" Edit"
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