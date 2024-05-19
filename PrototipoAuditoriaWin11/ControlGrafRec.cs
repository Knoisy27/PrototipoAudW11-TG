﻿using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    public partial class ControlGrafRec : UserControl
    {
        public ControlGrafRec()
        {
            InitializeComponent();
            this.Visible = false;
        }

        public void ActualizarGrafico(Tuple<int, int> datos)
        {
            int configuradasCorrectamente = datos.Item1;
            int configuradasIncorrectamente = datos.Item2;

            PieSeries seriesCorrectas = new PieSeries
            {
                Title = "Correctas",
                Values = new ChartValues<int> { configuradasCorrectamente },
                Fill = System.Windows.Media.Brushes.Green
            };

            PieSeries seriesIncorrectas = new PieSeries
            {
                Title = "Incorrectas",
                Values = new ChartValues<int> { configuradasIncorrectamente },
                Fill = System.Windows.Media.Brushes.Red
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
