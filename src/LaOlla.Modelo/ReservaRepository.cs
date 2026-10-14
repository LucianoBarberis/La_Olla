using Microsoft.Data.SqlClient;

namespace LaOlla.Modelo;

/// <summary>
/// Outcome of the atomic reserve operation (read quota + insert in one transaction).
/// </summary>
public enum ReservaTxOutcome
{
    Ok,
    SinVianda,
    Duplicada,
    CupoAgotado,
}

/// <summary>
/// Connected ADO.NET persistence for <see cref="Reserva"/>.
/// Quota-sensitive writes run read-quota + insert inside a single transaction
/// so the counter can never overpromise under concurrency.
/// </summary>
public sealed class ReservaRepository
{
    public Reserva? Obtener(DateOnly fecha, string dni)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT Fecha, Dni, NombreMenu, Estado FROM Reserva WHERE Fecha = @fecha AND Dni = @dni;",
            conn);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@dni", dni);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;
        return LeerReserva(reader);
    }

    public int ContarOcupadas(DateOnly fecha)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM Reserva WHERE Fecha = @fecha AND Estado <> @cancelada;",
            conn);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@cancelada", EstadoReserva.Cancelada);
        return (int)cmd.ExecuteScalar()!;
    }

    public List<Reserva> ListarPorFecha(DateOnly fecha)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT Fecha, Dni, NombreMenu, Estado FROM Reserva WHERE Fecha = @fecha ORDER BY Dni;",
            conn);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        using var reader = cmd.ExecuteReader();
        var list = new List<Reserva>();
        while (reader.Read())
            list.Add(LeerReserva(reader));
        return list;
    }

    public List<Reserva> ListarPorDni(string dni)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT Fecha, Dni, NombreMenu, Estado FROM Reserva WHERE Dni = @dni ORDER BY Fecha;",
            conn);
        cmd.Parameters.AddWithValue("@dni", dni);
        using var reader = cmd.ExecuteReader();
        var list = new List<Reserva>();
        while (reader.Read())
            list.Add(LeerReserva(reader));
        return list;
    }

    /// <summary>
    /// Atomic reserve: re-checks batch, duplicate and quota inside one transaction,
    /// then inserts or reactivates a cancelled row. Returns the outcome so the controller can message the counter.
    /// </summary>
    public ReservaTxOutcome ReservarAtomico(DateOnly fecha, string dni, string nombreMenu)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        int? cupo = LeerCupo(conn, tx, fecha);
        if (cupo is null)
        {
            tx.Rollback();
            return ReservaTxOutcome.SinVianda;
        }

        if (ExisteReserva(conn, tx, fecha, dni))
        {
            tx.Rollback();
            return ReservaTxOutcome.Duplicada;
        }

        if (ContarOcupadas(conn, tx, fecha) >= cupo.Value)
        {
            tx.Rollback();
            return ReservaTxOutcome.CupoAgotado;
        }

        if (ExisteCancelada(conn, tx, fecha, dni))
        {
            using var update = new SqlCommand(
                "UPDATE Reserva SET NombreMenu = @menu, Estado = @estado WHERE Fecha = @fecha AND Dni = @dni;",
                conn, tx);
            update.Parameters.AddWithValue("@menu", nombreMenu);
            update.Parameters.AddWithValue("@estado", EstadoReserva.NoEntregado);
            update.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
            update.Parameters.AddWithValue("@dni", dni);
            update.ExecuteNonQuery();
        }
        else
        {
            using var insert = new SqlCommand(
                "INSERT INTO Reserva (Fecha, Dni, NombreMenu, Estado) VALUES (@fecha, @dni, @menu, @estado);",
                conn, tx);
            insert.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
            insert.Parameters.AddWithValue("@dni", dni);
            insert.Parameters.AddWithValue("@menu", nombreMenu);
            insert.Parameters.AddWithValue("@estado", EstadoReserva.NoEntregado);
            insert.ExecuteNonQuery();
        }

        tx.Commit();
        return ReservaTxOutcome.Ok;
    }

    public void CambiarEstado(DateOnly fecha, string dni, string nuevoEstado)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "UPDATE Reserva SET Estado = @estado WHERE Fecha = @fecha AND Dni = @dni;",
            conn);
        cmd.Parameters.AddWithValue("@estado", nuevoEstado);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@dni", dni);
        if (cmd.ExecuteNonQuery() == 0)
            throw new InvalidOperationException("Reservation not found for the given date and DNI.");
    }

    /// <summary>
    /// Marks every still-pending reservation of the date as No retirado.
    /// Returns the number of rows affected.
    /// </summary>
    public int MarcarNoRetirados(DateOnly fecha)
    {
        using var conn = Db.CreateConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "UPDATE Reserva SET Estado = @nuevo WHERE Fecha = @fecha AND Estado = @pendiente;",
            conn);
        cmd.Parameters.AddWithValue("@nuevo", EstadoReserva.NoRetirado);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@pendiente", EstadoReserva.NoEntregado);
        return cmd.ExecuteNonQuery();
    }

    private static Reserva LeerReserva(SqlDataReader reader) => new()
    {
        Fecha = DateOnly.FromDateTime(reader.GetDateTime(0)),
        Dni = reader.GetString(1),
        NombreMenu = reader.GetString(2),
        Estado = reader.GetString(3),
    };

    private static int? LeerCupo(SqlConnection conn, SqlTransaction tx, DateOnly fecha)
    {
        using var cmd = new SqlCommand("SELECT Cupo FROM Vianda WHERE Fecha = @fecha;", conn, tx);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        var value = cmd.ExecuteScalar();
        return value is null or DBNull ? null : (int)value;
    }

    private static bool ExisteReserva(SqlConnection conn, SqlTransaction tx, DateOnly fecha, string dni)
    {
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM Reserva WHERE Fecha = @fecha AND Dni = @dni AND Estado <> @cancelada;",
            conn, tx);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@dni", dni);
        cmd.Parameters.AddWithValue("@cancelada", EstadoReserva.Cancelada);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static bool ExisteCancelada(SqlConnection conn, SqlTransaction tx, DateOnly fecha, string dni)
    {
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM Reserva WHERE Fecha = @fecha AND Dni = @dni AND Estado = @cancelada;",
            conn, tx);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@dni", dni);
        cmd.Parameters.AddWithValue("@cancelada", EstadoReserva.Cancelada);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static int ContarOcupadas(SqlConnection conn, SqlTransaction tx, DateOnly fecha)
    {
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM Reserva WHERE Fecha = @fecha AND Estado <> @cancelada;",
            conn, tx);
        cmd.Parameters.AddWithValue("@fecha", fecha.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@cancelada", EstadoReserva.Cancelada);
        return (int)cmd.ExecuteScalar()!;
    }
}
