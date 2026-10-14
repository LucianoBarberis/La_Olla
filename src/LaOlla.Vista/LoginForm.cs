using LaOlla.Controlador;

namespace LaOlla.Vista;

/// <summary>
/// R6: DNI-only login. The form collects DNI (+ name on first visit) and the
/// controller decides the role; the form only opens the matching view.
/// </summary>
public partial class LoginForm : Form
{
    private readonly AccesoController _acceso = new();

    public LoginForm()
    {
        InitializeComponent();
        ActiveControl = txtDni;
    }

    private void BtnIngresar_Click(object? sender, EventArgs e)
    {
        var (ok, rol, nombre, mensaje) = _acceso.Ingresar(txtDni.Text, txtNombre.Text);
        lblMensaje.Text = mensaje;
        if (!ok)
            return;

        Hide();
        try
        {
            using Form destino = rol == Rol.Admin
                ? new AdminForm()
                : new EstudianteForm(txtDni.Text.Trim(), nombre);
            destino.ShowDialog();
        }
        finally
        {
            txtDni.Clear();
            txtNombre.Clear();
            lblMensaje.Text = "";
            Show();
        }
    }
}
