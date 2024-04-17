using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    public partial class FormVentanaPrincipal : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private Logica logica;
        public VentanaAnalisis ventanaAnalisis;

        public FormVentanaPrincipal()
        {
            InitializeComponent();

            ventanaAnalisis = new VentanaAnalisis();
            logica = new Logica(ventanaAnalisis.PanelDinamicoResultados, ventanaAnalisis.DgvResultados);
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

            // Agregar VentanaAnalisis al formulario principal
            this.Controls.Add(ventanaAnalisis);


        }




        // BOTONES PANEL TOP -------------------------
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal) this.WindowState = FormWindowState.Maximized;
            else this.WindowState = FormWindowState.Normal;
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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


        private void BtnAnalizar_Click(object sender, EventArgs e)
        {
            if (!ventanaAnalisis.Visible)
            {
                ventanaAnalisis.BringToFront();
                ventanaAnalisis.Visible = true;

                Program.VerificarArchivoCFG();

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
            }
        }


    }
}
