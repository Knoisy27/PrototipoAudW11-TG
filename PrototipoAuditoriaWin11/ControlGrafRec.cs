using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    public partial class ControlGrafRec : UserControl
    {
        public ControlGrafRec()
        {
            InitializeComponent();
            this.Hide();
        }

        public void ActualizarGrafico(Tuple<int, int> datos)
        {
            int configuradasCorrectamente = datos.Item1;
            int configuradasIncorrectamente = datos.Item2;

            PieSeries seriesCorrectas = new PieSeries
            {
                Title = "Correctas",
                Values = new ChartValues<int> { configuradasCorrectamente },
                Fill = System.Windows.Media.Brushes.Green,
                DataLabels = true,
                LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
            };

            PieSeries seriesIncorrectas = new PieSeries
            {
                Title = "Incorrectas",
                Values = new ChartValues<int> { configuradasIncorrectamente },
                Fill = System.Windows.Media.Brushes.Red,
                DataLabels = true,
                LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation)
            };

            pieChart.Series.Clear();
            pieChart.Series.Add(seriesCorrectas);
            pieChart.Series.Add(seriesIncorrectas);

            pieChart.LegendLocation = LegendLocation.Bottom;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
