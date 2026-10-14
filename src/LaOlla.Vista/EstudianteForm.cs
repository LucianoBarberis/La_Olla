using LaOlla.Controlador;

namespace LaOlla.Vista;

/// <summary>
/// Student self-service: reserve within quota, review own reservations, cancel.
/// The form never decides business rules; it calls the controller and shows
/// the returned message plus the quota the controller reports.
/// </summary>
public partial class EstudianteForm : Form
{
    private readonly string _dni;
    private readonly ReservaController _reservas = new();
    private readonly ViandaController _viandas = new();

    public EstudianteForm(string dni, string nombre)
    {
        _dni = dni;
        InitializeComponent();
        lblTitulo.Text = $"Reservas de {nombre} ({dni})";
        dtpFecha.Value = DateTime.Today.AddDays(1);
        ActualizarCupo();
        ActualizarMias();
    }

    private void BtnReservar_Click(object? sender, EventArgs e)
    {
        Resultado r = _reservas.Reservar(DateOnly.FromDateTime(dtpFecha.Value), _dni, "");
        lblMensaje.Text = r.Mensaje;
        ActualizarCupo();
        ActualizarMias();
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        Resultado r = _reservas.Cancelar(DateOnly.FromDateTime(dtpFecha.Value), _dni);
        lblMensaje.Text = r.Mensaje;
        ActualizarCupo();
        ActualizarMias();
    }

    private void DtpFecha_ValueChanged(object? sender, EventArgs e)
    {
        ActualizarCupo();
        ActualizarMias();
    }

    private void ActualizarCupo()
    {
        DateOnly fecha = DateOnly.FromDateTime(dtpFecha.Value);
        var vianda = _viandas.ObtenerVianda(fecha);
        lblCupo.Text = vianda is null
            ? "Sin vianda cargada para esta fecha."
            : $"Cupo disponible: {_viandas.CupoDisponible(fecha)} / {vianda.Cupo} — {vianda.NombreMenu}";
    }

    private void ActualizarMias()
    {
        lstMias.Items.Clear();
        var mias = _reservas.ReservasDe(_dni);
        if (mias.Count == 0)
            lstMias.Items.Add("Sin reservas registradas.");
        else foreach (var r in mias)
            lstMias.Items.Add($"{r.Fecha:yyyy-MM-dd} | {r.NombreMenu} | {r.Estado}");
    }
}
