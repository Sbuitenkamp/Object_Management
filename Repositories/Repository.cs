using MySql.Data.MySqlClient;

namespace Object_management.Repositories;

public abstract class Repository
{
    protected MySqlConnection Connection;
    
    private string DbName;
    private string DbHost;
    private string DbUser;
    private string DbPassword;

    protected Repository()
    {
        DbName = "Object_management";
        DbHost = "127.0.0.1";
        DbUser = "Sbuitenkamp";
        DbPassword = "pass123";
    }

    protected void Connect()
    {
        string connectionString = "SERVER=" + DbHost + ";DATABASE=" + DbName + ";USER=" + DbUser + ";PASSWORD=" + DbPassword + ";SSL Mode=None;Allow User Variables=True;";
        Connection = new MySqlConnection(connectionString);
        Connection.Open();
    }

    protected void CloseConnection()
    {
        Connection.Close();
    }
}