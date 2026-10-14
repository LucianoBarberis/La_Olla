using LaOlla.Modelo;

namespace LaOlla.Controlador;

/// <summary>
/// Reservation use cases. Rules enforced here (the Vista only displays
/// <see cref="Resultado.Mensaje"/>):
/// R1 quota respected, never overpromised ("Cupo agotado" on exhaustion);
/// R2 one reservation = one (Fecha, DNI), quantity 1, window 00:00-12:00;
/// R3 no duplicates for the same (Fecha, DNI);
/// R4 cancel frees quota via state change, never physical delete;
/// R5 states Entregado / No entregado / No retirado (auto after 14:00).
/// </summary>
public sealed class ReservaController
{
    public static readonly TimeOnly HoraLimiteReserva = new(12, 0);
    public static readonly TimeOnly HoraNoRetirado = new(14, 0);

    private readonly ViandaRepository _viandas;
    private readonly ReservaRepository _reservas;
    private readonly ClienteRepository _clientes;
    private readonly Func<DateTime> _reloj;

    public ReservaController()
        : this(new ViandaRepository(), new ReservaRepository(), new ClienteRepository(), () => DateTime.Now)
    {
    }

    internal ReservaController(
        ViandaRepository viandas,
        ReservaRepository reservas,
        ClienteRepository clientes,
        Func<DateTime> reloj)
    {
        _viandas = viandas;
        _reservas = reservas;
        _clientes = clientes;
        _reloj = reloj;
    }

    public Resultado Reservar(DateOnly fecha, string dni, string nombre)
    {
        string limpio = (dni ?? "").Trim();
        if (!AccesoController.EsDniValido(limpio))
            return Resultado.Fallo("Ingrese un DNI válido (7 u 8 dígitos).");

        // R2: reservation window 00:00-12:00 on the reservation day.
        DateTime ahora = _reloj();
        if (DateOnly.FromDateTime(ahora) == fecha && TimeOnly.FromDateTime(ahora) > HoraLimiteReserva)
            return Resultado.Fallo("El horario de reserva finalizó (ventana 00:00-12:00).");

        Vianda? vianda = _viandas.ObtenerPorFecha(fecha);
        if (vianda is null)
            return Resultado.Fallo("Sin vianda cargada para esta fecha.");

        // R3: fast pre-check for a clear message; the transaction re-checks atomically.
        if (_reservas.Obtener(fecha, limpio) is { Estado: not EstadoReserva.Cancelada })
            return Resultado.Fallo("Ese DNI ya tiene una reserva para el día.");

        // R1: fast pre-check for a clear message; the transaction owns the final word.
        if (_reservas.ContarOcupadas(fecha) >= vianda.Cupo)
            return Resultado.Fallo("Cupo agotado.");

        string nombreLimpio = (nombre ?? "").Trim();
        if (_clientes.ObtenerPorDni(limpio) is null)
        {
            if (nombreLimpio.Length == 0)
                return Resultado.Fallo("DNI nuevo: ingrese su nombre para registrarse.");
            _clientes.Guardar(new Cliente { Dni = limpio, Nombre = nombreLimpio });
        }

        ReservaTxOutcome outcome = _reservas.ReservarAtomico(fecha, limpio, vianda.NombreMenu);
        return outcome switch
        {
            ReservaTxOutcome.Ok => Resultado.Exito($"Reserva confirmada: {vianda.NombreMenu}."),
            ReservaTxOutcome.SinVianda => Resultado.Fallo("Sin vianda cargada para esta fecha."),
            ReservaTxOutcome.Duplicada => Resultado.Fallo("Ese DNI ya tiene una reserva para el día."),
            ReservaTxOutcome.CupoAgotado => Resultado.Fallo("Cupo agotado."),
            _ => Resultado.Fallo("No se pudo completar la reserva."),
        };
    }

    /// <summary>
    /// R4: cancel frees quota by moving the reservation to Cancelada (row kept).
    /// </summary>
    public Resultado Cancelar(DateOnly fecha, string dni)
    {
        Reserva? reserva = _reservas.Obtener(fecha, (dni ?? "").Trim());
        if (reserva is null || reserva.Estado == EstadoReserva.Cancelada)
            return Resultado.Fallo("No hay una reserva activa para ese DNI y esa fecha.");
        _reservas.CambiarEstado(fecha, reserva.Dni, EstadoReserva.Cancelada);
        return Resultado.Exito("Reserva cancelada; el cupo quedó liberado.");
    }

    /// <summary>
    /// R5: counter marks a pending reservation as delivered.
    /// </summary>
    public Resultado MarcarEntregado(DateOnly fecha, string dni)
    {
        Reserva? reserva = _reservas.Obtener(fecha, (dni ?? "").Trim());
        if (reserva is null || reserva.Estado == EstadoReserva.Cancelada)
            return Resultado.Fallo("No hay una reserva activa para ese DNI y esa fecha.");
        if (reserva.Estado == EstadoReserva.Entregado)
            return Resultado.Fallo("La reserva ya fue entregada.");
        _reservas.CambiarEstado(fecha, reserva.Dni, EstadoReserva.Entregado);
        return Resultado.Exito("Reserva marcada como entregada.");
    }

    /// <summary>
    /// R5: after 14:00 every still-pending reservation becomes No retirado.
    /// The Vista enables this action only past 14:00; the controller enforces it too.
    /// </summary>
    public Resultado AplicarNoRetirados(DateOnly fecha)
    {
        DateTime ahora = _reloj();
        if (DateOnly.FromDateTime(ahora) == fecha && TimeOnly.FromDateTime(ahora) < HoraNoRetirado)
            return Resultado.Fallo("El marcado de no retirados está disponible después de las 14:00.");
        int filas = _reservas.MarcarNoRetirados(fecha);
        return Resultado.Exito(filas == 0
            ? "Sin reservas pendientes para marcar."
            : $"{filas} reserva(s) marcada(s) como no retirada(s).");
    }

    public List<Reserva> ReservasDelDia(DateOnly fecha) => _reservas.ListarPorFecha(fecha);

    public List<Reserva> ReservasDe(string dni) => _reservas.ListarPorDni((dni ?? "").Trim());
}
