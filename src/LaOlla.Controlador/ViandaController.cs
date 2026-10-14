using LaOlla.Modelo;

namespace LaOlla.Controlador;

/// <summary>
/// Daily batch management. R1: the day starts capped at 150 units and the
/// system never overpromises — quota above the cap is rejected here.
/// </summary>
public sealed class ViandaController
{
    public const int CupoMaximo = 150;

    private readonly ViandaRepository _viandas;
    private readonly ReservaRepository _reservas;

    public ViandaController() : this(new ViandaRepository(), new ReservaRepository()) { }

    internal ViandaController(ViandaRepository viandas, ReservaRepository reservas)
    {
        _viandas = viandas;
        _reservas = reservas;
    }

    public Resultado CargarVianda(DateOnly fecha, string nombreMenu, int cupo)
    {
        if (string.IsNullOrWhiteSpace(nombreMenu))
            return Resultado.Fallo("Enter the menu name for the day.");
        if (cupo <= 0)
            return Resultado.Fallo("Quota must be greater than zero.");
        if (cupo > CupoMaximo)
            return Resultado.Fallo($"Quota cannot exceed {CupoMaximo} units per day.");

        _viandas.Guardar(new Vianda
        {
            Fecha = fecha,
            NombreMenu = nombreMenu.Trim(),
            Cupo = cupo,
        });
        return Resultado.Exito($"Daily batch loaded: {nombreMenu.Trim()} for {fecha:yyyy-MM-dd} with quota {cupo}.");
    }

    public Vianda? ObtenerVianda(DateOnly fecha) => _viandas.ObtenerPorFecha(fecha);

    /// <summary>
    /// Remaining quota = batch quota minus occupied reservations.
    /// </summary>
    public int CupoDisponible(DateOnly fecha)
    {
        Vianda? vianda = _viandas.ObtenerPorFecha(fecha);
        if (vianda is null)
            return 0;
        return Math.Max(0, vianda.Cupo - _reservas.ContarOcupadas(fecha));
    }
}
