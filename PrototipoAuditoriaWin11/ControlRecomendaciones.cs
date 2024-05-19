using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    public partial class ControlRecomendaciones : UserControl
    {
        public ControlRecomendaciones()
        {
            InitializeComponent();
            this.Hide();
        }

        public void MostrarInformacion(string informacion)
        {
            tbRecomendaciones.Text = informacion;
        }

        private void btnAceptar_Click(object sender, System.EventArgs e)
        {
            this.Visible = false;
        }
    }
}
