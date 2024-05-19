using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    partial class VentanaAnalisis
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VentanaAnalisis));
            this.btnCerrarVentanaAnalisis = new System.Windows.Forms.Button();
            this.panelDinamicoResultados = new System.Windows.Forms.Panel();
            this.dgvRec = new System.Windows.Forms.DataGridView();
            this.btnEstadisticas = new System.Windows.Forms.Button();
            this.panelDinamicoResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRec)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCerrarVentanaAnalisis
            // 
            this.btnCerrarVentanaAnalisis.AutoSize = true;
            this.btnCerrarVentanaAnalisis.BackColor = System.Drawing.Color.Transparent;
            this.btnCerrarVentanaAnalisis.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCerrarVentanaAnalisis.FlatAppearance.BorderSize = 0;
            this.btnCerrarVentanaAnalisis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarVentanaAnalisis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCerrarVentanaAnalisis.Image = ((System.Drawing.Image)(resources.GetObject("btnCerrarVentanaAnalisis.Image")));
            this.btnCerrarVentanaAnalisis.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCerrarVentanaAnalisis.Location = new System.Drawing.Point(28, 13);
            this.btnCerrarVentanaAnalisis.Name = "btnCerrarVentanaAnalisis";
            this.btnCerrarVentanaAnalisis.Size = new System.Drawing.Size(38, 38);
            this.btnCerrarVentanaAnalisis.TabIndex = 2;
            this.btnCerrarVentanaAnalisis.UseVisualStyleBackColor = false;
            this.btnCerrarVentanaAnalisis.Click += new System.EventHandler(this.btnCerrarVentanaAnalisis_Click);
            // 
            // panelDinamicoResultados
            // 
            this.panelDinamicoResultados.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDinamicoResultados.BackColor = System.Drawing.Color.White;
            this.panelDinamicoResultados.Controls.Add(this.dgvRec);
            this.panelDinamicoResultados.Location = new System.Drawing.Point(82, 68);
            this.panelDinamicoResultados.Name = "panelDinamicoResultados";
            this.panelDinamicoResultados.Size = new System.Drawing.Size(946, 558);
            this.panelDinamicoResultados.TabIndex = 3;
            // 
            // dgvRec
            // 
            this.dgvRec.AllowUserToAddRows = false;
            this.dgvRec.AllowUserToDeleteRows = false;
            this.dgvRec.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRec.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRec.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvRec.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRec.Location = new System.Drawing.Point(3, 3);
            this.dgvRec.Name = "dgvRec";
            this.dgvRec.ReadOnly = true;
            this.dgvRec.RowHeadersWidth = 30;
            this.dgvRec.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvRec.RowTemplate.Height = 28;
            this.dgvRec.Size = new System.Drawing.Size(940, 552);
            this.dgvRec.TabIndex = 0;
            this.dgvRec.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRec_CellClick);
            this.dgvRec.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRec_CellMouseEnter);
            this.dgvRec.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvRec_RowPostPaint);
            // 
            // btnEstadisticas
            // 
            this.btnEstadisticas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEstadisticas.AutoSize = true;
            this.btnEstadisticas.BackColor = System.Drawing.Color.Ivory;
            this.btnEstadisticas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEstadisticas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstadisticas.Location = new System.Drawing.Point(903, 13);
            this.btnEstadisticas.Name = "btnEstadisticas";
            this.btnEstadisticas.Size = new System.Drawing.Size(122, 38);
            this.btnEstadisticas.TabIndex = 4;
            this.btnEstadisticas.Text = "Estadísticas";
            this.btnEstadisticas.UseVisualStyleBackColor = false;
            this.btnEstadisticas.Click += new System.EventHandler(this.btnEstadisticas_Click);
            // 
            // VentanaAnalisis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(241)))));
            this.Controls.Add(this.btnEstadisticas);
            this.Controls.Add(this.panelDinamicoResultados);
            this.Controls.Add(this.btnCerrarVentanaAnalisis);
            this.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Location = new System.Drawing.Point(251, 61);
            this.Name = "VentanaAnalisis";
            this.Size = new System.Drawing.Size(1068, 658);
            this.panelDinamicoResultados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRec)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCerrarVentanaAnalisis;
        private System.Windows.Forms.Panel panelDinamicoResultados;
        private System.Windows.Forms.DataGridView dgvRec;
        private Button btnEstadisticas;
    }
}