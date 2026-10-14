namespace LaOlla.Modelo;

/// <summary>
/// Single daily batch of viandas. One row per date (Fecha is the primary key).
/// </summary>
public sealed class Vianda
{
    public DateOnly Fecha { get; set; }
    public string NombreMenu { get; set; } = "";
    public int Cupo { get; set; }
}
