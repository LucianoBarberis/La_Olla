namespace LaOlla.Modelo;

/// <summary>
/// Counter client identified by DNI (primary key).
/// </summary>
public sealed class Cliente
{
    public string Dni { get; set; } = "";
    public string Nombre { get; set; } = "";
}
