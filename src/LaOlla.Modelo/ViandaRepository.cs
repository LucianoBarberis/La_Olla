using Microsoft.Data.SqlClient;

namespace LaOlla.Modelo;

/// <summary>
/// Connected ADO.NET persistence for the daily <see cref="Vianda"/> batch.
/// </summary>
public sealed class ViandaRepository
{
    public Vianda? ObtenerPorFecha(DateOnly fecha)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT Fecha, NombreMenu, Cupo FROM Vianda WHERE Fecha = @fecha;", conn);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;
        return new Vianda
        {
            Fecha = DateOnly.FromDateTime(reader.GetDateTime(0)),
            NombreMenu = reader.GetString(1),
            Cupo = reader.GetInt32(2),
        };
    }

    public void Guardar(Vianda vianda)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            """
            IF EXISTS (SELECT 1 FROM Vianda WHERE Fecha = @fecha)
                UPDATE Vianda SET NombreMenu = @menu, Cupo = @cupo WHERE Fecha = @fecha;
            ELSE
                INSERT INTO Vianda (Fecha, NombreMenu, Cupo) VALUES (@fecha, @menu, @cupo);
            """, conn);
        cmd.Parameters.AddWithValue("@fecha", vianda.Fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@menu", vianda.NombreMenu);
        cmd.Parameters.AddWithValue("@cupo", vianda.Cupo);
        cmd.ExecuteNonQuery();
    }
}
