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
            this.btnCerrarVentanaAnalisis = new System.Windows.Forms.Button();
            this.panelDinamicoResultados = new System.Windows.Forms.Panel();
            this.dgvRec = new System.Windows.Forms.DataGridView();
            this.tlpRecomendaciones = new System.Windows.Forms.TableLayoutPanel();
            this.panelDinamicoResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRec)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCerrarVentanaAnalisis
            // 
            this.btnCerrarVentanaAnalisis.BackColor = System.Drawing.Color.Transparent;
            this.btnCerrarVentanaAnalisis.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCerrarVentanaAnalisis.FlatAppearance.BorderSize = 0;
            this.btnCerrarVentanaAnalisis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarVentanaAnalisis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCerrarVentanaAnalisis.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCerrarVentanaAnalisis.Location = new System.Drawing.Point(1031, 3);
            this.btnCerrarVentanaAnalisis.Name = "btnCerrarVentanaAnalisis";
            this.btnCerrarVentanaAnalisis.Size = new System.Drawing.Size(36, 37);
            this.btnCerrarVentanaAnalisis.TabIndex = 2;
            this.btnCerrarVentanaAnalisis.Text = "X";
            this.btnCerrarVentanaAnalisis.UseVisualStyleBackColor = false;
            this.btnCerrarVentanaAnalisis.Click += new System.EventHandler(this.btnCerrarVentanaAnalisis_Click);
            // 
            // panelDinamicoResultados
            // 
            this.panelDinamicoResultados.BackColor = System.Drawing.Color.White;
            this.panelDinamicoResultados.Controls.Add(this.dgvRec);
            this.panelDinamicoResultados.Location = new System.Drawing.Point(52, 46);
            this.panelDinamicoResultados.Name = "panelDinamicoResultados";
            this.panelDinamicoResultados.Size = new System.Drawing.Size(948, 560);
            this.panelDinamicoResultados.TabIndex = 3;
            // 
            // dgvRec
            // 
            this.dgvRec.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRec.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRec.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRec.Location = new System.Drawing.Point(0, 0);
            this.dgvRec.Name = "dgvRec";
            this.dgvRec.RowHeadersWidth = 62;
            this.dgvRec.RowTemplate.Height = 28;
            this.dgvRec.Size = new System.Drawing.Size(948, 560);
            this.dgvRec.TabIndex = 0;
            this.dgvRec.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRec_CellMouseEnter);
            // 
            // tlpRecomendaciones
            // 
            this.tlpRecomendaciones.AutoSize = true;
            this.tlpRecomendaciones.BackColor = System.Drawing.Color.Transparent;
            this.tlpRecomendaciones.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpRecomendaciones.ColumnCount = 2;
            this.tlpRecomendaciones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRecomendaciones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpRecomendaciones.Location = new System.Drawing.Point(52, 640);
            this.tlpRecomendaciones.Name = "tlpRecomendaciones";
            this.tlpRecomendaciones.RowCount = 1;
            this.tlpRecomendaciones.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRecomendaciones.Size = new System.Drawing.Size(861, 2);
            this.tlpRecomendaciones.TabIndex = 0;
            // 
            // VentanaAnalisis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(248)))));
            this.Controls.Add(this.tlpRecomendaciones);
            this.Controls.Add(this.panelDinamicoResultados);
            this.Controls.Add(this.btnCerrarVentanaAnalisis);
            this.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Location = new System.Drawing.Point(252, 60);
            this.Name = "VentanaAnalisis";
            this.Size = new System.Drawing.Size(1070, 660);
            this.panelDinamicoResultados.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRec)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCerrarVentanaAnalisis;
        private System.Windows.Forms.Panel panelDinamicoResultados;
        private System.Windows.Forms.TableLayoutPanel tlpRecomendaciones;
        private System.Windows.Forms.DataGridView dgvRec;
    }
}