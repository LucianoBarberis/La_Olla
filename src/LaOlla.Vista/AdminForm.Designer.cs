namespace LaOlla.Vista;

partial class AdminForm
{
    private System.ComponentModel.IContainer components = null;
    private DateTimePicker dtpFecha;
    private TextBox txtMenu;
    private NumericUpDown numCupo;
    private Button btnCargar;
    private TextBox txtDni;
    private Button btnEntregar;
    private Button btnNoRetirados;
    private Button btnVer;
    private Label lblCupo;
    private ListBox lstReservas;
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
        dtpFecha = new DateTimePicker();
        txtMenu = new TextBox();
        numCupo = new NumericUpDown();
        btnCargar = new Button();
        txtDni = new TextBox();
        btnEntregar = new Button();
        btnNoRetirados = new Button();
        btnVer = new Button();
        lblCupo = new Label();
        lstReservas = new ListBox();
        lblMensaje = new Label();
        ((System.ComponentModel.ISupportInitialize)numCupo).BeginInit();
        SuspendLayout();
        //
        // dtpFecha
        //
        dtpFecha.Format = DateTimePickerFormat.Short;
        dtpFecha.Location = new Point(16, 14);
        dtpFecha.Size = new Size(150, 23);
        dtpFecha.ValueChanged += DtpFecha_ValueChanged;
        //
        // txtMenu
        //
        txtMenu.Location = new Point(176, 14);
        txtMenu.PlaceholderText = "Nombre del menú";
        txtMenu.Size = new Size(180, 23);
        //
        // numCupo
        //
        numCupo.Location = new Point(366, 14);
        numCupo.Maximum = 150;
        numCupo.Minimum = 1;
        numCupo.Size = new Size(70, 23);
        numCupo.Value = 10;
        //
        // btnCargar
        //
        btnCargar.Location = new Point(446, 10);
        btnCargar.Size = new Size(110, 30);
        btnCargar.Text = "Cargar vianda";
        btnCargar.Click += BtnCargar_Click;
        //
        // txtDni
        //
        txtDni.Location = new Point(16, 52);
        txtDni.MaxLength = 8;
        txtDni.PlaceholderText = "DNI para entregar";
        txtDni.Size = new Size(150, 23);
        //
        // btnEntregar
        //
        btnEntregar.Location = new Point(176, 48);
        btnEntregar.Size = new Size(110, 30);
        btnEntregar.Text = "Entregar";
        btnEntregar.Click += BtnEntregar_Click;
        //
        // btnNoRetirados
        //
        btnNoRetirados.Location = new Point(296, 48);
        btnNoRetirados.Size = new Size(140, 30);
        btnNoRetirados.Text = "Marcar no retirados";
        btnNoRetirados.Click += BtnNoRetirados_Click;
        //
        // btnVer
        //
        btnVer.Location = new Point(446, 48);
        btnVer.Size = new Size(110, 30);
        btnVer.Text = "Actualizar";
        btnVer.Click += BtnVer_Click;
        //
        // lblCupo
        //
        lblCupo.AutoSize = true;
        lblCupo.Location = new Point(16, 86);
        lblCupo.Text = "";
        //
        // lstReservas
        //
        lstReservas.Location = new Point(16, 110);
        lstReservas.Size = new Size(540, 170);
        //
        // lblMensaje
        //
        lblMensaje.AutoSize = true;
        lblMensaje.Location = new Point(16, 288);
        lblMensaje.MaximumSize = new Size(540, 0);
        //
        // AdminForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(572, 330);
        Controls.Add(dtpFecha);
        Controls.Add(txtMenu);
        Controls.Add(numCupo);
        Controls.Add(btnCargar);
        Controls.Add(txtDni);
        Controls.Add(btnEntregar);
        Controls.Add(btnNoRetirados);
        Controls.Add(btnVer);
        Controls.Add(lblCupo);
        Controls.Add(lstReservas);
        Controls.Add(lblMensaje);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "La Olla - Mostrador";
        ((System.ComponentModel.ISupportInitialize)numCupo).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
