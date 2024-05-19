﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PrototipoAuditoriaWin11
{
    public partial class VentanaAnalisis : UserControl
    {
        //public Panel PanelDinamicoResultados => panelDinamicoResultados;
        //private System.Windows.Forms.ToolTip toolTip1;  // Declaración del control ToolTip
        public ControlRecomendaciones controlRecomendaciones { get; private set; }
        public ControlGrafRec controlGrafRec { get; private set; }
        Logica logica;

        public event EventHandler ControlRecomendacionesVisibleChanged;

        public DataGridView DgvResultados => dgvRec;
        public VentanaAnalisis()
        {
            InitializeComponent();
            this.Hide();
            logica = new Logica(/*PanelDinamicoResultados, */DgvResultados);
            //toolTip1 = new System.Windows.Forms.ToolTip();
            controlRecomendaciones = new ControlRecomendaciones();
            controlGrafRec = new ControlGrafRec();

            this.Controls.Add(controlRecomendaciones);
            this.Controls.Add(controlGrafRec); 

            dgvRec.Columns.Add("ColumnaPoliticas", "Política");
            dgvRec.Columns.Add("ColumnaClaves", "Clave");
            dgvRec.Columns.Add("ColumnaValores", "Valor");
            dgvRec.Columns.Add("ColumnaRecomendaciones", "Recomendación");
            dgvRec.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Configurar AutoScroll en PanelDinamicoResultados
            //PanelDinamicoResultados.AutoScroll = true;
            dgvRec.CellMouseEnter += dgvRec_CellMouseEnter;
            // Subscribirse al evento VisibleChanged
            controlRecomendaciones.VisibleChanged += ControlRecomendaciones_VisibleChanged;
        }



        private void btnCerrarVentanaAnalisis_Click(object sender, EventArgs e)
        {

            dgvRec.Rows.Clear();
            // Obtener el formulario contenedor (FormVentanaPrincipal)
            FormVentanaPrincipal formPrincipal = this.Parent as FormVentanaPrincipal;

            // Verificar si el formulario contenedor no es nulo
            if (formPrincipal != null)
            {
                // Cerrar la ventana actual (VentanaAnalisis)
                //formPrincipal.ventanaAnalisis.Visible = false;
                formPrincipal.ventanaAnalisis.Hide();
            }

            //logica.condicionMetodos.Clear();
        }
        private void dgvRec_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = dgvRec.Rows[e.RowIndex].Cells[e.ColumnIndex];

            }
        }

        private void dgvRec_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Dibujar el índice de la fila en el área del encabezado de fila
            string rowIndex = (e.RowIndex + 1).ToString(); // El índice de la fila comienza en 0, por lo que sumamos 1 para obtener el índice desde 1
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dgvRec.RowHeadersWidth, e.RowBounds.Height);

            // Configurar el estilo del texto
            TextRenderer.DrawText(e.Graphics, rowIndex, dgvRec.RowHeadersDefaultCellStyle.Font, headerBounds, dgvRec.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        private void dgvRec_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 3) // Verifica si se hizo clic en la última celda de cualquier fila
            {
                Console.WriteLine("ultima fila");
                string clave = dgvRec.Rows[e.RowIndex].Cells[1].Value.ToString(); // Obtiene el valor de la segunda celda de la fila clickeada
                string informacion = logica.ObtenerInformacionMetodo(clave);
                controlRecomendaciones.MostrarInformacion(informacion);
                controlRecomendaciones.Visible = true;
                controlRecomendaciones.BringToFront();
            }
        }

        private void ControlRecomendaciones_VisibleChanged(object sender, EventArgs e)
        {
            // Propagar el evento hacia afuera
            ControlRecomendacionesVisibleChanged?.Invoke(this, e);
        }

        private void btnEstadisticas_Click(object sender, EventArgs e)
        {
            controlGrafRec.BringToFront();
            controlGrafRec.Show();
            ActualizarGrafico();
        }

        public void ActualizarGrafico()
        {
            // Lógica para actualizar el gráfico
            if (controlGrafRec != null)
            {
                controlGrafRec.ActualizarGrafico(ObtenerDatosParaGrafico());
            }
        }

        private Tuple<int, int> ObtenerDatosParaGrafico()
        {
            int configuradasCorrectamente = 0;
            int configuradasIncorrectamente = 0;

            foreach (DataGridViewRow row in dgvRec.Rows)
            {
                if (row.DefaultCellStyle.BackColor == Color.LightGreen)
                {
                    configuradasCorrectamente++;
                }
                else if (row.DefaultCellStyle.BackColor == Color.LightCoral)
                {
                    configuradasIncorrectamente++;
                }
            }

            return Tuple.Create(configuradasCorrectamente, configuradasIncorrectamente);
        }

    }
}
