using Microsoft.Data.SqlClient;

namespace LaOlla.Modelo;

/// <summary>
/// Connected ADO.NET persistence for <see cref="Cliente"/>.
/// All commands are parametrized; connections stay open the shortest time.
/// </summary>
public sealed class ClienteRepository
{
    public Cliente? ObtenerPorDni(string dni)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT Dni, Nombre FROM Cliente WHERE Dni = @dni;", conn);
        cmd.Parameters.AddWithValue("@dni", dni);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;
        return new Cliente
        {
            Dni = reader.GetString(0),
            Nombre = reader.GetString(1),
        };
    }

    public void Guardar(Cliente cliente)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            """
            IF EXISTS (SELECT 1 FROM Cliente WHERE Dni = @dni)
                UPDATE Cliente SET Nombre = @nombre WHERE Dni = @dni;
            ELSE
                INSERT INTO Cliente (Dni, Nombre) VALUES (@dni, @nombre);
            """, conn);
        cmd.Parameters.AddWithValue("@dni", cliente.Dni);
        cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
        cmd.ExecuteNonQuery();
    }
}
