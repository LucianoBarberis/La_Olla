namespace LaOlla.Controlador;

/// <summary>
/// Operation outcome returned to the Vista. The Vista only shows
/// <see cref="Mensaje"/> and checks <see cref="Ok"/>; every business
/// decision lives here, never in a Form click handler.
/// </summary>
public sealed class Resultado
{
    public bool Ok { get; init; }
    public string Mensaje { get; init; } = "";

    public static Resultado Exito(string mensaje) => new() { Ok = true, Mensaje = mensaje };
    public static Resultado Fallo(string mensaje) => new() { Ok = false, Mensaje = mensaje };
}
