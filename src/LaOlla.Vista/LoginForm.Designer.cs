namespace LaOlla.Vista;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtDni;
    private TextBox txtNombre;
    private Button btnIngresar;
    private Label lblMensaje;
    private Label lblDni;
    private Label lblNombre;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblDni = new Label();
        txtDni = new TextBox();
        lblNombre = new Label();
        txtNombre = new TextBox();
        btnIngresar = new Button();
        lblMensaje = new Label();
        SuspendLayout();
        //
        // lblDni
        //
        lblDni.AutoSize = true;
        lblDni.Location = new Point(24, 24);
        lblDni.Text = "DNI";
        //
        // txtDni
        //
        txtDni.Location = new Point(24, 44);
        txtDni.MaxLength = 8;
        txtDni.Size = new Size(240, 23);
        //
        // lblNombre
        //
        lblNombre.AutoSize = true;
        lblNombre.Location = new Point(24, 80);
        lblNombre.Text = "Name (first visit only)";
        //
        // txtNombre
        //
        txtNombre.Location = new Point(24, 100);
        txtNombre.MaxLength = 80;
        txtNombre.Size = new Size(240, 23);
        //
        // btnIngresar
        //
        btnIngresar.Location = new Point(24, 140);
        btnIngresar.Size = new Size(240, 32);
        btnIngresar.Text = "Enter";
        btnIngresar.Click += BtnIngresar_Click;
        //
        // lblMensaje
        //
        lblMensaje.AutoSize = true;
        lblMensaje.Location = new Point(24, 184);
        lblMensaje.MaximumSize = new Size(240, 0);
        //
        // LoginForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(292, 240);
        Controls.Add(lblDni);
        Controls.Add(txtDni);
        Controls.Add(lblNombre);
        Controls.Add(txtNombre);
        Controls.Add(btnIngresar);
        Controls.Add(lblMensaje);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "La Olla - Enter";
        ResumeLayout(false);
        PerformLayout();
    }
}
