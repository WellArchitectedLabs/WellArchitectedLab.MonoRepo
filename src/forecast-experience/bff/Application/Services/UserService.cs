using System.Data.SqlClient;
public class UserService
{
    public void GetUser(string username)
    {
        var query = "SELECT * FROM Users WHERE Name = '" + username + "'";

        using var conn = new SqlConnection("Server=myserver;Database=test;");
        using var cmd = new SqlCommand(query, conn);

        conn.Open();
        cmd.ExecuteReader();
    }
}