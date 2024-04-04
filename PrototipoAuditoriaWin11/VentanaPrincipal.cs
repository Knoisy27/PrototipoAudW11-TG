﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            ventanaAnalisis.BringToFront();
            ventanaAnalisis.Visible = true;

            string[] metodos =
                {
                "Enforce_Password_History",
                "Maximum_Password_Age",
                "MinimumPasswordAge",
                "MinimumPasswordLength",
                "Password_must_meet_complexity_requirements",
                "Relax_minimum_password_length_limits",
                "Clear_Text_Password",
            };

            foreach (string metodo in metodos) 
            {
                // Utilizar reflexión para obtener el método por su nombre
                var methodInfo = typeof(Logica).GetMethod(metodo);
                if (methodInfo != null)
                {
                    // Invocar el método dinámicamente
                    methodInfo.Invoke(logica, null);
                    // Agregar el comentario y valor pendiente
                    logica.EjecutarYAgregarComentario(() => { }, logica.Comentario, logica.Valor);
                }
                else
                {
                    Console.WriteLine($"El método {metodo} no fue encontrado en la clase Logica.");
                }
            }

            /*logica.EjecutarYAgregarComentario(logica.Enforce_Password_History, logica.Comentario, logica.Valor);
            logica.EjecutarYAgregarComentario(logica.Maximum_Password_Age, logica.Comentario, logica.Valor);
            logica.EjecutarYAgregarComentario(logica.MinimumPasswordAge, logica.Comentario, logica.Valor);
            logica.EjecutarYAgregarComentario(logica.MinimumPasswordLength, logica.Comentario, logica.Valor);
            logica.EjecutarYAgregarComentario(logica.Password_must_meet_complexity_requirements, logica.Comentario, logica.Valor);
            logica.EjecutarYAgregarComentario(logica.Relax_minimum_password_length_limits, logica.Comentario, logica.Valor);
            logica.EjecutarYAgregarComentario(logica.Clear_Text_Password, logica.Comentario, logica.Valor);
            */
            

            logica.AgregarComentariosPendientes();
            logica.ColorFilas();
        }
    }
}
