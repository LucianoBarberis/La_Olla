namespace LaOlla.Modelo;

/// <summary>
/// One reservation = one (Fecha, Dni) pair, quantity always 1.
/// </summary>
public sealed class Reserva
{
    public DateOnly Fecha { get; set; }
    public string Dni { get; set; } = "";
    public string NombreMenu { get; set; } = "";
    public string Estado { get; set; } = EstadoReserva.NoEntregado;
}
