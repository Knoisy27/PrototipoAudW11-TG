using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    public class Configuracion
    {
        public string Politica { get; set; }
        public string Clave { get; set; }
        public string Valor { get; set; }
        public string Recomendacion { get; set; }

        // Constructor que toma los cuatro argumentos
        public Configuracion(string politica, string clave, string valor, string recomendacion)
        {
            Politica = politica;
            Clave = clave;
            Valor = valor;
            Recomendacion = recomendacion;
        }


    }

}
