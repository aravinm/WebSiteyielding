<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Registeration.aspx.cs" Inherits="Registeration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        User Registration<br />
        <br />
        Email:<asp:TextBox ID="TextBox1" runat="server" style="margin-left: 32px" Width="220px"></asp:TextBox>
        <br />
        <br />
        Password:<asp:TextBox ID="TextBox2" runat="server" style="margin-left: 10px" Width="213px"></asp:TextBox>
        <br />
        <br />
        Confirmed Password:<asp:TextBox ID="TextBox3" runat="server" style="margin-left: 9px" Width="207px"></asp:TextBox>
        <br />
        <br />
        Name of Member:<asp:TextBox ID="TextBox4" runat="server" style="margin-left: 14px" Width="205px"></asp:TextBox>
        <br />
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" style="margin-left: 84px" Text="Submit" Width="190px" />
        <br />
    
    </div>
    </form>
</body>
</html>
