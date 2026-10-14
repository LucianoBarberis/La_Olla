namespace LaOlla.Modelo;

/// <summary>
/// Reservation states. Only <see cref="Cancelada"/> frees quota;
/// every other state occupies one unit of the daily batch.
/// </summary>
public static class EstadoReserva
{
    public const string NoEntregado = "No entregado";
    public const string Entregado = "Entregado";
    public const string NoRetirado = "No retirado";
    public const string Cancelada = "Cancelada";

    public static bool OcupaCupo(string estado) => estado != Cancelada;
}
