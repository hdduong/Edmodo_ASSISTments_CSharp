<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="connectToASSISTments_1._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connect To Edmdo</title>
</head>
<body>

    <form id="form1" runat="server">
    <div>
    
    </div>
        <h2> Please choose an action: </h2>
        <asp:LinkButton  ID="teacherTutor1" runat="server" OnClick="teacherTutor1_Click" Text="Test Drive" /> <br />
        <asp:LinkButton  ID="teacherReport1" runat="server" OnClick="teacherReport1_Click" Text="Report" /> <br />
        <asp:LinkButton  ID="viewProblem" runat="server" OnClick="goToViewProlem" Text="View Problems" /> <br />
        
    </form>
</body>
</html>
