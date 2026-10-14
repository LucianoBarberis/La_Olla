using LaOlla.Modelo;

namespace LaOlla.Controlador;

/// <summary>
/// User role resolved by DNI-only login (R6).
/// </summary>
public enum Rol
{
    Admin,
    Estudiante,
}

/// <summary>
/// R6: DNI-only access. DNI 12345678 opens the Admin view,
/// any other valid DNI opens the Estudiante view.
/// </summary>
public sealed class AccesoController
{
    public const string DniAdmin = "12345678";

    private readonly ClienteRepository _clientes;

    public AccesoController() : this(new ClienteRepository()) { }

    internal AccesoController(ClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public (bool Ok, Rol Rol, string Nombre, string Mensaje) Ingresar(string dni, string nombreSiNuevo)
    {
        string limpio = (dni ?? "").Trim();
        if (!EsDniValido(limpio))
            return (false, Rol.Estudiante, "", "Enter a valid DNI (7 or 8 digits).");

        if (limpio == DniAdmin)
            return (true, Rol.Admin, "Mostrador", "Welcome, counter.");

        Cliente? existente = _clientes.ObtenerPorDni(limpio);
        if (existente is not null)
            return (true, Rol.Estudiante, existente.Nombre, $"Welcome, {existente.Nombre}.");

        string nombre = (nombreSiNuevo ?? "").Trim();
        if (nombre.Length == 0)
            return (false, Rol.Estudiante, "", "First-time DNI: enter your name to register.");
        _clientes.Guardar(new Cliente { Dni = limpio, Nombre = nombre });
        return (true, Rol.Estudiante, nombre, $"Welcome, {nombre}.");
    }

    public static bool EsDniValido(string dni) =>
        (dni.Length == 7 || dni.Length == 8) && dni.All(char.IsDigit);
}
