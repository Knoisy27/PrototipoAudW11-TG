using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace PrototipoAuditoriaWin11
{
    public partial class FormVentanaPrincipal : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private Logica logica;
        public VentanaAnalisis ventanaAnalisis;
        private ControlRecomendaciones controlRecomendaciones;
        private ControlGrafRec controlGrafRec;

        public FormVentanaPrincipal()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            MostrarEspecificacionesEquipo();

            ventanaAnalisis = new VentanaAnalisis();
            controlRecomendaciones = new ControlRecomendaciones();
            controlGrafRec = new ControlGrafRec();
            logica = new Logica(ventanaAnalisis.DgvResultados);

            MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;

            // Agregar VentanaAnalisis al formulario principal
            ventanaAnalisis.Dock = DockStyle.Fill;
            this.Controls.Add(ventanaAnalisis);

            ventanaAnalisis.VisibleChanged += FormVentanaPrincipal_VisibleChanged;
            ventanaAnalisis.ControlRecomendacionesVisibleChanged += VentanaAnalisis_ControlRecomendacionesVisibleChanged;
            ventanaAnalisis.ControlGrafRecVisibleChanged += VentanaAnalisis_ControlGrafRecVisibleChanged;
        }

        // FUNCIONALIDAD PARA REDIMENSIONAR LA VENTANA DESDE LOS BORDES -------------------------
        private const int cGrip = 16;
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = PointToClient(pos);

                if (pos.X <= cGrip && pos.Y <= cGrip)
                    m.Result = (IntPtr)HTTOPLEFT;
                else if (pos.X >= ClientSize.Width - cGrip && pos.Y <= cGrip)
                    m.Result = (IntPtr)HTTOPRIGHT;
                else if (pos.X <= cGrip && pos.Y >= ClientSize.Height - cGrip)
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (pos.X >= ClientSize.Width - cGrip && pos.Y >= ClientSize.Height - cGrip)
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                else if (pos.X <= cGrip)
                    m.Result = (IntPtr)HTLEFT;
                else if (pos.X >= ClientSize.Width - cGrip)
                    m.Result = (IntPtr)HTRIGHT;
                else if (pos.Y <= cGrip)
                    m.Result = (IntPtr)HTTOP;
                else if (pos.Y >= ClientSize.Height - cGrip)
                    m.Result = (IntPtr)HTBOTTOM;
            }
        }
        // ------------------------- FUNCIONALIDAD PARA REDIMENSIONAR LA VENTANA DESDE LOS BORDES


        // BOTONES PANEL TOP -------------------------
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
            AjustarVentanaAnalisis();
            AjustarControlRecomendaciones();
            AjustarControlGrafRec();
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        // ------------------------- BOTONES PANEL TOP




        // FUNCIONALIDADES DE MOVER LA VENTANA CON EL PANEL TOP -------------------------
        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            // Cuando se hace clic en el panel, registra la posición actual del mouse.
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            // Si el mouse está presionado, mueve el formulario según la diferencia de posición.
            if (mouseDown)
            {
                this.Location = new Point(
                    this.Location.X - lastLocation.X + e.X,
                    this.Location.Y - lastLocation.Y + e.Y);
                this.Update();
            }
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            // Cuando se suelta el mouse, marca que no está presionado.
            mouseDown = false;
        }
        // ------------------------- FUNCIONALIDADES DE MOVER LA VENTANA CON EL PANEL TOP



        // MOSTRAR ESPECIFICACIONES -------------------------
        private void MostrarEspecificacionesEquipo()
        {
            string especificaciones = "";

            // Nombre del sistema operativo
            especificaciones += "Sistema operativo: " + Environment.OSVersion.VersionString + Environment.NewLine;

            // Arquitectura del sistema operativo
            especificaciones += "Arquitectura del sistema operativo: " + (Environment.Is64BitOperatingSystem ? "64 bits" : "32 bits") + Environment.NewLine;

            // Versión del sistema operativo
            especificaciones += "Versión del sistema operativo: " + Environment.OSVersion.Version.ToString() + Environment.NewLine;

            // Nombre del equipo
            especificaciones += "Nombre del equipo: " + Environment.MachineName + Environment.NewLine;

            // Nombre del usuario
            especificaciones += "Nombre del usuario: " + Environment.UserName + Environment.NewLine;

            // Nombre del dominio del usuario
            especificaciones += "Nombre del dominio del usuario: " + Environment.UserDomainName /*+ Environment.NewLine*/;

            // Agregar las especificaciones al TextBox
            txtEspecs.Text = especificaciones;
        }
        // ------------------------- MOSTRAR ESPECIFICACIONES


        // BOTÓN ANALIZAR -------------------------
        private void btnAnalizar_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }
        private void BtnAnalizar_Click(object sender, EventArgs e)
        {
            if (!ventanaAnalisis.Visible) 
            {
                ventanaAnalisis.BringToFront();
                ventanaAnalisis.Show();

                Program.VerificarArchivoCFG();
                logica.LimpiarDatosCFG();

                // Obtener todos los métodos de la clase Logica que comienzan con el prefijo "Analizar_"
                var metodos = typeof(Logica).GetMethods()
                    .Where(m => m.Name.StartsWith("Analizar_"))
                    .ToList();

                foreach (var metodo in metodos)
                {
                    try
                    {
                        // Invocar el método dinámicamente
                        metodo.Invoke(logica, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al ejecutar el método {metodo.Name}: {ex.Message}");
                    }
                }

                // Agregar todas las configuraciones pendientes al DataGridView
                logica.AgregarConfiguracionesPendientes();
                logica.ColorFilas();
                logica.OrdenarDataGridViewPorNombre();
            }               
            
        }
        // ------------------------- BOTÓN ANALIZAR


        // FUNCIONES PARA HACER RESPONSIVE EL PROGRAMA -------------------------
        private void AjustarVentanaAnalisis()
        {
            if (ventanaAnalisis != null && ventanaAnalisis.Visible)
            {
                int anchoVentanaPrincipal = this.ClientSize.Width;
                int altoVentanaPrincipal = this.ClientSize.Height;

                ventanaAnalisis.Size = new Size(anchoVentanaPrincipal - 165, altoVentanaPrincipal - 40);
            }
        }


        private void AjustarControlRecomendaciones() 
        {
            if (ventanaAnalisis != null && ventanaAnalisis.Visible)
            {
                int anchoVentanaPrincipal = this.ClientSize.Width;
                int altoVentanaPrincipal = this.ClientSize.Height;

                // Ajustar el tamaño de ControlGrafRecomendaciones dentro de VentanaAnalisis
                if (ventanaAnalisis.controlRecomendaciones != null && ventanaAnalisis.controlRecomendaciones.Visible)
                {
                    ventanaAnalisis.controlRecomendaciones.Size = new Size(anchoVentanaPrincipal - 165, altoVentanaPrincipal - 40);
                    Console.WriteLine($"ControlGrafRecomendaciones Size: {ventanaAnalisis.controlRecomendaciones.Size}");
                }
            }
        }

        private void AjustarControlGrafRec()
        {
            if (ventanaAnalisis != null && ventanaAnalisis.Visible)
            {
                int anchoVentanaPrincipal = this.ClientSize.Width;
                int altoVentanaPrincipal = this.ClientSize.Height;

                // Ajustar el tamaño de ControlGrafRec dentro de VentanaAnalisis
                if (ventanaAnalisis.controlGrafRec != null && ventanaAnalisis.controlGrafRec.Visible)
                {
                    ventanaAnalisis.controlGrafRec.Size = new Size(anchoVentanaPrincipal - 165, altoVentanaPrincipal - 40);
                    Console.WriteLine($"ControlGrafRec Size: {ventanaAnalisis.controlGrafRec.Size}");
                }
            }
        }

        private void VentanaAnalisis_ControlRecomendacionesVisibleChanged(object sender, EventArgs e)
        {
            AjustarControlRecomendaciones();
        }
        
        private void VentanaAnalisis_ControlGrafRecVisibleChanged(object sender, EventArgs e)
        {
            AjustarControlGrafRec();
        }

        private void FormVentanaPrincipal_VisibleChanged(object sender, EventArgs e)
        {
            if (ventanaAnalisis.Visible)
            {
                AjustarVentanaAnalisis();

            }
        }
        private void FormVentanaPrincipal_SizeChanged(object sender, EventArgs e)
        {
            AjustarVentanaAnalisis();
            AjustarControlRecomendaciones();
            AjustarControlGrafRec();
        }
        // ------------------------- FUNCIONES PARA HACER RESPONSIVE EL PROGRAMA


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("https://github.com/Knoisy27/PrototipoAudW11-TG");
        }

        private void btnHerramientas_Click(object sender, EventArgs e)
        {
            panelHerramientasSub.Visible = (panelHerramientasSub.Visible == true) ? false : true;
        }


        private void btnGpedit_Click(object sender, EventArgs e)
        {
            try
            {
                Process gpedit = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"C:\WINDOWS\System32\gpedit.msc",
                        UseShellExecute = true,
                        Verb = "runas" // Establece el verbo a "runas" para ejecutar como administrador
                    }
                };
                gpedit.Start();
            }
            catch
            {
                string msg = "No se encuentra la herramienta gpedit.msc";
                string title = @"C:\WINDOWS\System32\gpedit.msc";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(msg, title, buttons, MessageBoxIcon.Error);
            }
        }

        private void btnRegedit_Click(object sender, EventArgs e)
        {
            try
            {
                //ExecuteCommand(@"%windir%\\System32\\gpedit.msc");
                Process gpedit = new Process();
                gpedit.StartInfo.FileName = @"C:\WINDOWS\regedit.exe";
                gpedit.Start();
            }
            catch
            {
                string msg = "No se encuentra la herramienta regedit.exe\n";
                string title = @"C:\WINDOWS\regedit.exe";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                MessageBox.Show(msg, title, buttons, MessageBoxIcon.Error);
            }
        }

        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("https://github.com/Knoisy27/PrototipoAudW11-TG");
        }
    }
}
