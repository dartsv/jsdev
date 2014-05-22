<%@ Page Language="C#" %>

<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        FormsAuthentication.SignOut();
        Response.Redirect("default.htm");
    }
</script>

<!DOCTYPE HTML>
<html>
<head>
    <title>Log out</title>
</head>
<body>
</body>
</html>
