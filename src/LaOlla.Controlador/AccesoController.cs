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
            return (false, Rol.Estudiante, "", "Ingrese un DNI válido (7 u 8 dígitos).");

        if (limpio == DniAdmin)
            return (true, Rol.Admin, "Mostrador", "Bienvenido al mostrador.");

        Cliente? existente = _clientes.ObtenerPorDni(limpio);
        if (existente is not null)
            return (true, Rol.Estudiante, existente.Nombre, $"Bienvenido, {existente.Nombre}.");

        string nombre = (nombreSiNuevo ?? "").Trim();
        if (nombre.Length == 0)
            return (false, Rol.Estudiante, "", "DNI nuevo: ingrese su nombre para registrarse.");
        _clientes.Guardar(new Cliente { Dni = limpio, Nombre = nombre });
        return (true, Rol.Estudiante, nombre, $"Bienvenido, {nombre}.");
    }

    public static bool EsDniValido(string dni) =>
        (dni.Length == 7 || dni.Length == 8) && dni.All(char.IsDigit);
}
