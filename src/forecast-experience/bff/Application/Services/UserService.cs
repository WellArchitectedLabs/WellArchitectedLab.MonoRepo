using Microsoft.Data.SqlClient;

public class UserService
{
    public void GetUser(string username)
    {
        using var conn = new SqlConnection("Server=myserver;Database=test;");
        conn.Open();

        var cmd = new SqlCommand(
            "SELECT * FROM Users WHERE Name = '" + username + "'",
            conn);

        cmd.ExecuteReader();
    }
}