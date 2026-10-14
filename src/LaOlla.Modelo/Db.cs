using Microsoft.Data.SqlClient;

namespace LaOlla.Modelo;

/// <summary>
/// Connection-string holder and connection factory.
/// The Vista never builds a connection string: it calls <see cref="Configure"/>
/// once at startup (value comes from App.config), then repositories call
/// <see cref="CreateConnection"/>.
/// </summary>
public static class Db
{
    private static string _connectionString = "";

    public static void Configure(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string must not be empty.", nameof(connectionString));
        _connectionString = connectionString;
    }

    public static string ConnectionString =>
        string.IsNullOrWhiteSpace(_connectionString)
            ? throw new InvalidOperationException("Db.Configure must be called at startup with the connection string.")
            : _connectionString;

    public static SqlConnection CreateConnection() => new(ConnectionString);
}
