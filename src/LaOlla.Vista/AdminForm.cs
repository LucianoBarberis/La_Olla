using LaOlla.Controlador;

namespace LaOlla.Vista;

/// <summary>
/// Counter view: loads the single daily batch, lists reservations, marks
/// deliveries and no-shows. Every button delegates to a controller method and
/// displays its message; no quota or rule logic lives in this form.
/// </summary>
public partial class AdminForm : Form
{
    private readonly ViandaController _viandas = new();
    private readonly ReservaController _reservas = new();

    public AdminForm()
    {
        InitializeComponent();
        dtpFecha.Value = DateTime.Today.AddDays(1);
        ActualizarLista();
    }

    private void BtnCargar_Click(object? sender, EventArgs e)
    {
        Resultado r = _viandas.CargarVianda(
            DateOnly.FromDateTime(dtpFecha.Value), txtMenu.Text, (int)numCupo.Value);
        lblMensaje.Text = r.Mensaje;
        ActualizarLista();
    }

    private void BtnEntregar_Click(object? sender, EventArgs e)
    {
        Resultado r = _reservas.MarcarEntregado(DateOnly.FromDateTime(dtpFecha.Value), txtDni.Text);
        lblMensaje.Text = r.Mensaje;
        ActualizarLista();
    }

    private void BtnNoRetirados_Click(object? sender, EventArgs e)
    {
        Resultado r = _reservas.AplicarNoRetirados(DateOnly.FromDateTime(dtpFecha.Value));
        lblMensaje.Text = r.Mensaje;
        ActualizarLista();
    }

    private void BtnVer_Click(object? sender, EventArgs e) => ActualizarLista();

    private void DtpFecha_ValueChanged(object? sender, EventArgs e) => ActualizarLista();

    private void ActualizarLista()
    {
        DateOnly fecha = DateOnly.FromDateTime(dtpFecha.Value);
        var vianda = _viandas.ObtenerVianda(fecha);
        lblCupo.Text = vianda is null
            ? "No daily batch for that date."
            : $"{vianda.NombreMenu} - available: {_viandas.CupoDisponible(fecha)} / {vianda.Cupo}";

        lstReservas.Items.Clear();
        foreach (var r in _reservas.ReservasDelDia(fecha))
            lstReservas.Items.Add($"{r.Dni} | {r.NombreMenu} | {r.Estado}");
    }
}
