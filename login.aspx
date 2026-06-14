<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DineMaster.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>DineMaster Login</title>
    <link href="CSS/login.css" rel="stylesheet" />
</head>

<body>

<form id="form1" runat="server">

    <div class="login-container">

        <div class="login-card">

            <h1 class="title">DineMaster</h1>
          

            <asp:Label ID="lblMessage" runat="server" CssClass="error"></asp:Label>

            <asp:TextBox ID="txtUsername" runat="server" CssClass="input"
                placeholder="Enter Username"></asp:TextBox>

            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                CssClass="input" placeholder="Enter Password"></asp:TextBox>

            <asp:Button ID="btnLogin" runat="server" Text="Login"
                CssClass="btn" OnClick="btnLogin_Click" Width="181px" />

            <p class="footer-text">Welcome to smart restaurant system</p>

        </div>

    </div>

</form>

</body>
</html>