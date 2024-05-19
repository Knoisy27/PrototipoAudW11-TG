using System.Drawing;
using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    partial class FormVentanaPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVentanaPrincipal));
            this.panelTop = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnMax = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAnalizar = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnHerramientas = new System.Windows.Forms.Button();
            this.panelHerramientasSub = new System.Windows.Forms.Panel();
            this.btnRegedit = new System.Windows.Forms.Button();
            this.btnGpedit = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.txtEspecs = new System.Windows.Forms.TextBox();
            this.panelTop.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelHerramientasSub.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.AllowDrop = true;
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.panelTop.Controls.Add(this.tableLayoutPanel1);
            this.panelTop.Controls.Add(this.label1);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnCerrar, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnMin, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnMax, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // btnCerrar
            // 
            resources.ApplyResources(this.btnCerrar, "btnCerrar");
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.btnCerrar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnMin
            // 
            resources.ApplyResources(this.btnMin, "btnMin");
            this.btnMin.FlatAppearance.BorderSize = 0;
            this.btnMin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(75)))), ((int)(((byte)(174)))));
            this.btnMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(55)))), ((int)(((byte)(129)))));
            this.btnMin.ForeColor = System.Drawing.Color.White;
            this.btnMin.Name = "btnMin";
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnMax
            // 
            resources.ApplyResources(this.btnMax, "btnMax");
            this.btnMax.FlatAppearance.BorderSize = 0;
            this.btnMax.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(75)))), ((int)(((byte)(174)))));
            this.btnMax.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(55)))), ((int)(((byte)(129)))));
            this.btnMax.ForeColor = System.Drawing.Color.White;
            this.btnMax.Name = "btnMax";
            this.btnMax.UseVisualStyleBackColor = true;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // btnAnalizar
            // 
            resources.ApplyResources(this.btnAnalizar, "btnAnalizar");
            this.btnAnalizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.btnAnalizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAnalizar.FlatAppearance.BorderSize = 0;
            this.btnAnalizar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this.btnAnalizar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnAnalizar.ForeColor = System.Drawing.Color.White;
            this.btnAnalizar.Name = "btnAnalizar";
            this.btnAnalizar.UseVisualStyleBackColor = false;
            this.btnAnalizar.Click += new System.EventHandler(this.BtnAnalizar_Click);
            this.btnAnalizar.MouseLeave += new System.EventHandler(this.btnAnalizar_MouseLeave);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.panel2.Controls.Add(this.btnHerramientas);
            this.panel2.Controls.Add(this.panelHerramientasSub);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.btnAnalizar);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // btnHerramientas
            // 
            resources.ApplyResources(this.btnHerramientas, "btnHerramientas");
            this.btnHerramientas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.btnHerramientas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHerramientas.FlatAppearance.BorderSize = 0;
            this.btnHerramientas.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this.btnHerramientas.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnHerramientas.ForeColor = System.Drawing.Color.White;
            this.btnHerramientas.Name = "btnHerramientas";
            this.btnHerramientas.UseVisualStyleBackColor = false;
            this.btnHerramientas.Click += new System.EventHandler(this.btnHerramientas_Click);
            // 
            // panelHerramientasSub
            // 
            this.panelHerramientasSub.Controls.Add(this.btnRegedit);
            this.panelHerramientasSub.Controls.Add(this.btnGpedit);
            resources.ApplyResources(this.panelHerramientasSub, "panelHerramientasSub");
            this.panelHerramientasSub.Name = "panelHerramientasSub";
            // 
            // btnRegedit
            // 
            resources.ApplyResources(this.btnRegedit, "btnRegedit");
            this.btnRegedit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.btnRegedit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegedit.FlatAppearance.BorderSize = 0;
            this.btnRegedit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this.btnRegedit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnRegedit.ForeColor = System.Drawing.Color.White;
            this.btnRegedit.Name = "btnRegedit";
            this.btnRegedit.UseVisualStyleBackColor = false;
            this.btnRegedit.Click += new System.EventHandler(this.btnRegedit_Click);
            // 
            // btnGpedit
            // 
            resources.ApplyResources(this.btnGpedit, "btnGpedit");
            this.btnGpedit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.btnGpedit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGpedit.FlatAppearance.BorderSize = 0;
            this.btnGpedit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.SkyBlue;
            this.btnGpedit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnGpedit.ForeColor = System.Drawing.Color.White;
            this.btnGpedit.Name = "btnGpedit";
            this.btnGpedit.UseVisualStyleBackColor = false;
            this.btnGpedit.Click += new System.EventHandler(this.btnGpedit_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // txtEspecs
            // 
            resources.ApplyResources(this.txtEspecs, "txtEspecs");
            this.txtEspecs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(241)))));
            this.txtEspecs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEspecs.Name = "txtEspecs";
            this.txtEspecs.ReadOnly = true;
            // 
            // FormVentanaPrincipal
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(241)))));
            this.Controls.Add(this.txtEspecs);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.pictureBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormVentanaPrincipal";
            this.Opacity = 0.99D;
            this.SizeChanged += new System.EventHandler(this.FormVentanaPrincipal_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.FormVentanaPrincipal_VisibleChanged);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelHerramientasSub.ResumeLayout(false);
            this.panelHerramientasSub.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Button btnMax;
        private System.Windows.Forms.Button btnAnalizar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private PictureBox pictureBox1;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnHerramientas;
        private Panel panelHerramientasSub;
        private Button btnRegedit;
        private Button btnGpedit;
        private PictureBox pictureBox2;
        private TextBox txtEspecs;
    }
}

