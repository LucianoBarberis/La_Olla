using System.Configuration;
using LaOlla.Modelo;

namespace LaOlla.Vista;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Connection string lives outside the forms: it is read here once
        // from App.config and handed to the Modelo layer.
        string? cadena = ConfigurationManager.ConnectionStrings["LaOlla"]?.ConnectionString;
        if (string.IsNullOrWhiteSpace(cadena))
        {
            MessageBox.Show("Falta la cadena de conexión 'LaOlla' en App.config.",
                "La Olla", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        Db.Configure(cadena);

        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }
}
