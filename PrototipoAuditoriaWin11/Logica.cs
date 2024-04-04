using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Win32;

namespace PrototipoAuditoriaWin11
{
    internal class Logica
    {
        private Panel panelDinamicoResultados;
        private DataGridView dgvRec;
        private Dictionary<string, string> configuraciones;
        private List<string> comentariosPendientes = new List<string>();
        private List<string> valoresPendientes = new List<string>();
        public List<bool> condicionMetodos = new List<bool>();




        public Logica(Panel panel, DataGridView dataGridView)
        {
            panelDinamicoResultados = panel;
            dgvRec = dataGridView;
            configuraciones = new Dictionary<string, string>();
            GuardarDatosCFG();
        }





        // LOGICA PARA LEER Y GUARDAR LOS DATOS DEL ARCHIVO SECURITY.CFG -------------------------
        private void GuardarDatosCFG()
        {
            string rutaArchivoConfig = @"C:\Windows\Temp\security.cfg";

            try
            {
                // Lee todas las líneas del archivo
                string[] lineas = System.IO.File.ReadAllLines(rutaArchivoConfig);

                // Procesa cada línea para obtener configuraciones
                foreach (string linea in lineas)
                {
                    // Ignora líneas en blanco o comentarios
                    if (string.IsNullOrWhiteSpace(linea) || linea.Trim().StartsWith(";") || linea.Trim().StartsWith("#"))
                    {
                        continue;
                    }

                    // Divide la línea en clave y valor (si es una configuración válida)
                    string[] partes = linea.Split('=');
                    if (partes.Length == 2)
                    {
                        string clave = partes[0].Trim();
                        string valor = partes[1].Trim();
                        //Console.WriteLine($"{clave} {valor}");

                        // Almacena la configuración en el diccionario
                        configuraciones[clave] = valor;
                        //Console.WriteLine(configuraciones[clave]);
                    }
                }

                //Console.WriteLine(configuraciones);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo de configuración: {ex.Message}");
            }
        }
        // ------------------------- LOGICA PARA LEER Y GUARDAR LOS DATOS DEL ARCHIVO SECURITY.CFG





        // LOGICA PARA ESTABLECER EL COMENTARIO DE CADA METODO -------------------------
        public string Comentario;

        public void EstablecerComentario(string comentario) { 
            Comentario = comentario;
            //Console.WriteLine($"Estableciendo comentario: {comentario}");
        }
        // ------------------------- LOGICA PARA ESTABLECER EL COMENTARIO DE CADA METODO
        



        
        // LOGICA PARA ESTABLECER EL VALOR DE CADA METODO -------------------------
        public string Valor;

        public void EstablecerValor(string valor) { 
            Valor = valor;
            //Console.WriteLine($"Estableciendo comentario: {comentario}");
        }
        
        public void EstablecerValor(int valor) { 
            Valor = valor.ToString();
            //Console.WriteLine($"Estableciendo comentario: {comentario}");
        }
        
        public void EstablecerValor(object valor) { 
            Valor = valor.ToString();
            //Console.WriteLine($"Estableciendo comentario: {comentario}");
        }
        // ------------------------- LOGICA PARA ESTABLECER EL VALOR DE CADA METODO





        private void GenerarFilaDinamica(string comentario, string valor)
        {
            DataGridViewRow row = new DataGridViewRow();

            row.CreateCells(dgvRec);
            row.Cells[0].Value = comentario;
            row.Cells[1].Value = valor;
            
            dgvRec.Rows.Add(row);
            
        }





        // Método común para ejecutar otros métodos y agregar etiquetas
        public void EjecutarYAgregarComentario(Action metodo, String comentario, String valor)
        {
            metodo.Invoke(); // Ejecuta el método proporcionado
            comentariosPendientes.Add(Comentario); // Agrega el comentario a la lista
            valoresPendientes.Add(Valor); // Agrega el valor a la lista
        }





        // Método para agregar todas las etiquetas pendientes
        public void AgregarComentariosPendientes()
        {
            // Suspender las actualizaciones de diseño del DataGridView
            dgvRec.SuspendLayout();

            // Combinar ambas listas usando Zip
            var combinaciones = comentariosPendientes.Zip(valoresPendientes, (c, v) => (Comentario: c, Valor: v));

            foreach (var (Comentario, Valor) in combinaciones)
            {
                GenerarFilaDinamica(Comentario, Valor);

                Console.WriteLine("Entra");
            }

            // Limpiar las listas después de agregar todas las etiquetas
            comentariosPendientes.Clear();
            valoresPendientes.Clear();

            // Reanudar las actualizaciones de diseño del DataGridView
            dgvRec.ResumeLayout();
        }


        private void EstablecerColor(bool condicion, int rowIndex) {

            DataGridViewCellStyle estilo = dgvRec.Rows[rowIndex].DefaultCellStyle;
            estilo.BackColor = condicion ? Color.LightGreen : Color.LightCoral;

        }

        public void ColorFilas() {

            int indx = 0;

            foreach (var cond in condicionMetodos)
            {
                EstablecerColor(cond, indx);
                indx++;
            }

            // Limpiar la lista despues de pintar las filas
            condicionMetodos.Clear();

        }





        public void MostrarEspecs() 
        {
            
        }





        public int ObtenerValorConfiguracion(string clave)
        {
            if (configuraciones.ContainsKey(clave))
            {
                string valorConfiguracion = configuraciones[clave];
                Console.WriteLine(valorConfiguracion);
                if (Int32.TryParse(valorConfiguracion, out int valorEntero))
                {
                    return valorEntero;
                }
            }
            return -1; // Valor predeterminado o manejo de error
        }






        // 1.1.1 ENFORCE PASSWORD HISTORY -------------------------
        public void Enforce_Password_History()
        {
            int valor = ObtenerValorConfiguracion("PasswordHistorySize"); ;
            EstablecerComentario("PasswordHistorySize");
            EstablecerValor(valor);
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey("PasswordHistorySize"))
            {
                if (valor >= 24)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.1 ENFORCE PASSWORD HISTORY
        

        
        
        
        // 1.1.2 MAXIMUM PASSWORD AGE -------------------------
        public void Maximum_Password_Age()
        {
            int valor = ObtenerValorConfiguracion("MaximumPasswordAge"); ;
            EstablecerComentario("MaximumPasswordAge");
            EstablecerValor(valor);
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey("MaximumPasswordAge"))
            {
                if (valor > 0 && valor <= 365)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave MaximumPasswordAge no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.1 MAXIMUM PASSWORD AGE

        
        
        
        
        // 1.1.3 MINIMUM PASSWORD AGE -------------------------
        public void MinimumPasswordAge()
        {
            int valor = ObtenerValorConfiguracion("MinimumPasswordAge"); ;
            EstablecerComentario("MinimumPasswordAge");
            EstablecerValor(valor);
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey("MinimumPasswordAge"))
            {
                if (valor > 0)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.3 MINIMUM PASSWORD AGE






        // 1.1.4 MINIMUM PASSWORD LENGTH -------------------------
        public void MinimumPasswordLength()
        {
            int valor = ObtenerValorConfiguracion("MinimumPasswordLength"); ;
            EstablecerComentario("MinimumPasswordLength");
            EstablecerValor(valor);
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey("MinimumPasswordLength"))
            {
                if (valor > 13)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.4 MINIMUM PASSWORD LENGTH
        
        
        
        
        
        
        
        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------
        public void Password_must_meet_complexity_requirements()
        {
            int valor = ObtenerValorConfiguracion("PasswordComplexity"); ;
            EstablecerComentario("PasswordComplexity");
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey("PasswordComplexity"))
            {
                if (valor == 1)
                {
                    EstablecerValor(valor + " - Enabled");
                    condicionMetodos.Add(true);
                }
                else if (valor == 0)
                {
                    EstablecerValor(valor + " - Disabled");
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS




        // 1.1.6 MINUMUM PASSWORD LENGTH LIMITS -------------------------
        public void Relax_minimum_password_length_limits() 
        {

            // Ruta del registro a la que deseas acceder
            string rutaRegistro = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\SAM";

            // Nombre del valor que quieres leer
            string nombreValor = "RelaxMinimumPasswordLengthLimits";

            // Intenta leer el valor del registro
            object valor = Registry.GetValue(rutaRegistro, nombreValor, null);

            if (valor != null)
            {
                EstablecerComentario("RelaxMinimumPasswordLengthLimits");
                EstablecerValor(valor);
                condicionMetodos.Add(true);
            }
            else
            {
                EstablecerComentario("RelaxMinimumPasswordLengthLimits");
                EstablecerValor("Undefined");
                condicionMetodos.Add(false);
            }

        }
        // ------------------------- 1.1.6 MINUMUM PASSWORD LENGTH LIMITS -------------------------


        // 1.1.7 STORE PASSWORDS USING REVERSIBLE ENCRYPTION -------------------------
        public void Clear_Text_Password()
        {

            int valor = ObtenerValorConfiguracion("ClearTextPassword"); ;
            EstablecerComentario("ClearTextPassword");
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey("ClearTextPassword"))
            {
                if (valor == 1)
                {
                    EstablecerValor(valor + " - Enabled");
                    condicionMetodos.Add(true);
                }
                else if (valor == 0)
                {
                    EstablecerValor(valor + " - Disabled");
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }

        }
        // ------------------------- 1.1.7 STORE PASSWORDS USING REVERSIBLE ENCRYPTION



        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS


        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------



        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS

        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------

        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS









    }
}
