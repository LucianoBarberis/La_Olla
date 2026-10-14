namespace LaOlla.Vista;

partial class EstudianteForm
{
    private System.ComponentModel.IContainer components = null;
    private Label lblTitulo;
    private DateTimePicker dtpFecha;
    private Button btnReservar;
    private Button btnCancelar;
    private Label lblCupo;
    private ListBox lstMias;
    private Label lblMensaje;

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
        lblTitulo = new Label();
        dtpFecha = new DateTimePicker();
        btnReservar = new Button();
        btnCancelar = new Button();
        lblCupo = new Label();
        lstMias = new ListBox();
        lblMensaje = new Label();
        SuspendLayout();
        //
        // lblTitulo
        //
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(16, 12);
        lblTitulo.Text = "Mis reservas";
        //
        // dtpFecha
        //
        dtpFecha.Format = DateTimePickerFormat.Short;
        dtpFecha.Location = new Point(16, 36);
        dtpFecha.Size = new Size(160, 23);
        dtpFecha.ValueChanged += DtpFecha_ValueChanged;
        //
        // btnReservar
        //
        btnReservar.Location = new Point(184, 32);
        btnReservar.Size = new Size(110, 30);
        btnReservar.TabIndex = 1;
        btnReservar.Text = "Reservar";
        btnReservar.Click += BtnReservar_Click;
        //
        // btnCancelar
        //
        btnCancelar.Location = new Point(300, 32);
        btnCancelar.Size = new Size(122, 30);
        btnCancelar.TabIndex = 2;
        btnCancelar.Text = "Cancelar reserva";
        btnCancelar.Click += BtnCancelar_Click;
        //
        // lblCupo
        //
        lblCupo.AutoSize = true;
        lblCupo.Location = new Point(16, 68);
        lblCupo.Text = "";
        //
        // lstMias
        //
        lstMias.Location = new Point(16, 96);
        lstMias.Size = new Size(404, 150);
        //
        // lblMensaje
        //
        lblMensaje.AutoSize = true;
        lblMensaje.Location = new Point(16, 254);
        lblMensaje.MaximumSize = new Size(404, 0);
        //
        // EstudianteForm
        //
        AcceptButton = btnReservar;
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(438, 300);
        Controls.Add(lblTitulo);
        Controls.Add(dtpFecha);
        Controls.Add(btnReservar);
        Controls.Add(btnCancelar);
        Controls.Add(lblCupo);
        Controls.Add(lstMias);
        Controls.Add(lblMensaje);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "La Olla - Estudiante";
        ResumeLayout(false);
        PerformLayout();
    }
}
