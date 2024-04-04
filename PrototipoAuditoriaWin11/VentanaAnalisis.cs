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
        public Panel PanelDinamicoResultados => panelDinamicoResultados;
        private System.Windows.Forms.ToolTip toolTip1;  // Declaración del control ToolTip
        //Logica logica;



        public DataGridView DgvResultados => dgvRec;
        public VentanaAnalisis()
        {
            InitializeComponent();
            //logica = new Logica(PanelDinamicoResultados, DgvResultados);
            this.Visible = false;
            toolTip1 = new System.Windows.Forms.ToolTip();

            dgvRec.Columns.Add("ColumnaConfiguraciones", "Configuraciones");
            dgvRec.Columns.Add("ColumnaValores", "Valor");
            dgvRec.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Configurar AutoScroll en PanelDinamicoResultados
            PanelDinamicoResultados.AutoScroll = true;
            dgvRec.CellMouseEnter += dgvRec_CellMouseEnter;
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
                formPrincipal.ventanaAnalisis.Visible = false;
            }

            //logica.condicionMetodos.Clear();
        }

        private void dgvRec_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = dgvRec.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // Obtener el texto del tooltip según la celda actual
                string tooltipText = ObtenerTextoTooltipSegunCelda(e.RowIndex, e.ColumnIndex);

                // Configurar el nuevo tooltip
                MostrarToolTip(cell, tooltipText);
            }
        }




        private string ObtenerTextoTooltipSegunCelda(int rowIndex, int columnIndex)
        {
            // Lógica para obtener el texto del tooltip según la celda actual
            // Puedes basarte en los datos de tu DataGridView o en otros criterios
            // En este ejemplo, simplemente estoy devolviendo un texto fijo
            return "Tooltip personalizado para la celda en la fila " + rowIndex + ", columna " + columnIndex;
        }


        private void MostrarToolTip(DataGridViewCell cell, string comentario)
        {
            // Mostrar el comentario en un ToolTip
            System.Windows.Forms.ToolTip tooltip = new System.Windows.Forms.ToolTip();
            tooltip.SetToolTip(dgvRec, comentario);
        }


    }
}
