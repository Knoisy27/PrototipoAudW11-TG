using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Win32;
using System.Reflection;
using System.Security.Principal;
using System.Net;

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

        public void EstablecerComentario(string comentario)
        {
            Comentario = comentario;
            //Console.WriteLine($"Estableciendo comentario: {comentario}");
        }
        // ------------------------- LOGICA PARA ESTABLECER EL COMENTARIO DE CADA METODO





        // LOGICA PARA ESTABLECER EL VALOR DE CADA METODO -------------------------
        public string Valor;

        public void EstablecerValor(string valor)
        {
            Valor = valor;
            //Console.WriteLine($"Estableciendo comentario: {comentario}");
        }

        public void EstablecerValor(int valor)
        {
            Valor = valor.ToString();
            //Console.WriteLine($"Estableciendo comentario: {comentario}");
        }

        public void EstablecerValor(object valor)
        {
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


        private void EstablecerColor(bool condicion, int rowIndex)
        {

            DataGridViewCellStyle estilo = dgvRec.Rows[rowIndex].DefaultCellStyle;
            estilo.BackColor = condicion ? Color.LightGreen : Color.LightCoral;

        }

        public void ColorFilas()
        {

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




        // FUNCION AUXILIAR PARA SEPARAR POR LA COMA DESDE UN STRING -------------------------
        public bool VerificarValores(string cadena, string valoresRequeridos)
        {
            // Dividir la cadena de valores requeridos en substrings basados en la coma como delimitador
            string[] valoresArray = valoresRequeridos.Split(',');
            // Dividir la cadena original en substrings basados en la coma como delimitador
            string[] substrings = cadena.Split(',');
            // Verificar que la cantidad de substrings sea igual a la cantidad de valores requeridos
            if (substrings.Length != valoresArray.Length)            {
                return false; // Si la cantidad de substrings no coincide, retornar false
            }
            // Verificar si todos los valores requeridos están presentes en los substrings
            foreach (string valorRequerido in valoresArray)
            {
                bool encontrado = false;
                foreach (string substring in substrings)
                {
                    if (valorRequerido.Trim() == substring.Trim()) // Trim() para eliminar espacios en blanco alrededor
                    {
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    return false; // Si un valor requerido no se encuentra, retornar false
                }
            }
            return true; // Si todos los valores requeridos están presentes y no hay extras, retornar true
        }
        // ------------------------- FUNCION AUXILIAR PARA SEPARAR POR LA COMA DESDE UN STRING




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



        // 1.2.1 ACCOUNT LOCKOUT DURATION -------------------------
        public void Account_lockout_duration()
        {
            int valor = ObtenerValorConfiguracion("LockoutDuration"); ;
            EstablecerComentario("LockoutDuration");
            if (configuraciones.ContainsKey("LockoutDuration"))
            {
                if (valor >= 15)
                {
                    EstablecerValor(valor);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstablecerValor(valor);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }

        }
        // ------------------------- 1.2.1 ACCOUNT LOCKOUT DURATION

        // 1.2.2 ACCOUNT LOCKOUT THRESHOLD -------------------------
        public void Account_lockout_threshold()
        {
            int valor = ObtenerValorConfiguracion("LockoutBadCount"); ;
            EstablecerComentario("LockoutBadCount");
            if (configuraciones.ContainsKey("LockoutBadCount"))
            {
                if (valor <= 5 && valor != 0)
                {
                    EstablecerValor(valor);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstablecerValor(valor);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.2.2 ACCOUNT LOCKOUT THRESHOLD

        // 1.2.3 ALLOW ADMINISTRATOR ACCOUNT LOCKOUT -------------------------
        public void Allow_Administrator_account_lockout()
        {
            int valor = ObtenerValorConfiguracion("AllowAdministratorLockout"); ;
            EstablecerComentario("AllowAdministratorLockout");
            if (configuraciones.ContainsKey("AllowAdministratorLockout"))
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
        // ------------------------- 1.2.3 ALLOW ADMINISTRATOR ACCOUNT LOCKOUT

        // 1.2.4 RESET ACCOUNT LOCKOUT COUNTER AFTER -------------------------
        public void Reset_account_lockout_counter_after()
        {
            int valor = ObtenerValorConfiguracion("ResetLockoutCount"); ;
            EstablecerComentario("ResetLockoutCount");
            if (configuraciones.ContainsKey("ResetLockoutCount"))
            {
                if (valor >= 15)
                {
                    EstablecerValor(valor);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstablecerValor(valor);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.2.4 RESET ACCOUNT LOCKOUT COUNTER AFTER

        // 2.2.1 ACCESS CREDENTIAL MANAGER AS A TRUSTED CALLER -------------------------
        public void Access_Credential_Manager_as_a_trusted_caller()
        {
            string clave = "SeTrustedCredManAccessPrivilege";
            EstablecerComentario("SeTrustedCredManAccessPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstablecerValor(valor);
                condicionMetodos.Add(false);
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.1 ACCESS CREDENTIAL MANAGER AS A TRUSTED CALLER

        // 2.2.2 ACCESS THIS COMPUTER FROM THE NETWORK -------------------------
        public void Access_this_computer_from_the_network()
        {
            string clave = "SeNetworkLogonRight";
            EstablecerComentario("SeNetworkLogonRight");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos ="*S-1-5-32-544,*S-1-5-14";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else 
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.2 ACCESS THIS COMPUTER FROM THE NETWORK

        // 2.2.3 ACT AS PART OF THE OPERATING SYSTEM -------------------------
        public void Act_as_part_of_the_operating_system()
        {
            string clave = "SeTcbPrivilege";
            EstablecerComentario("SeTcbPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstablecerValor(valor);
                condicionMetodos.Add(false);
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.3 ACT AS PART OF THE OPERATING SYSTEM

        // 2.2.4 ADJUST MEMORY QUOTAS FOR A PROCESS -------------------------
        public void Adjust_memory_quotas_for_a_process()
        {
            string clave = "SeIncreaseQuotaPrivilege";
            EstablecerComentario("SeIncreaseQuotaPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-19,*S-1-5-20";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.4 ADJUST MEMORY QUOTAS FOR A PROCESS

        // 2.2.5 ALLOW LOG ON LOCALLY -------------------------
        public void Allow_log_on_locally()
        {
            string clave = "SeInteractiveLogonRight";
            EstablecerComentario("SeInteractiveLogonRight");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-32-545";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.5 ALLOW LOG ON LOCALLY


        // 2.2.6 ALLOW LOG ON THROUGH REMOTE DESKTOP SERVICES -------------------------
        public void Allow_log_on_through_Remote_Desktop_Services()
        {
            string clave = "SeRemoteInteractiveLogonRight";
            EstablecerComentario("SeRemoteInteractiveLogonRight");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-32-555";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.6 ALLOW LOG ON THROUGH REMOTE DESKTOP SERVICES


        // 2.2.7 BACK UP FILES AND DIRECTORIES -------------------------
        public void Back_up_files_and_directories()
        {
            string clave = "SeRestorePrivilege";
            EstablecerComentario("SeRestorePrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.7 BACK UP FILES AND DIRECTORIES


        // 2.2.8 CHANGE THE SYSTEM TIME -------------------------
        public void Change_the_system_time()
        {
            string clave = "SeSystemtimePrivilege";
            EstablecerComentario("SeSystemtimePrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-19";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.8 CHANGE THE SYSTEM TIME


        // 2.2.9 CHANGE THE TIME ZONE -------------------------
        public void Change_the_time_zone()
        {
            string clave = "SeTimeZonePrivilege";
            EstablecerComentario("SeTimeZonePrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-19,*S-1-5-32-544,*S-1-5-32-545";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.9 CHANGE THE TIME ZONE


        // 2.2.10 CREATE A PAGEFILE -------------------------
        public void Create_a_pagefile()
        {
            string clave = "SeCreatePagefilePrivilege";
            EstablecerComentario("SeCreatePagefilePrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.10 CREATE A PAGEFILE

        // 2.2.11 CREATE A TOKEN OBJECT' IS SET TO 'NO ONE' -------------------------
        public void Create_a_token_object_is_set_to_No_One()
        {
            string clave = "SeCreateTokenPrivilege";
            EstablecerComentario("SeCreateTokenPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstablecerValor(valor);
                condicionMetodos.Add(false);
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.11 CREATE A TOKEN OBJECT' IS SET TO 'NO ONE'


        // 2.2.12 CREATE GLOBAL OBJECTS -------------------------
        public void Create_global_objects()
        {
            string clave = "SeCreateGlobalPrivilege";
            EstablecerComentario("SeCreateGlobalPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-19,*S-1-5-20,*S-1-5-32-544,*S-1-5-6";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.12 CREATE GLOBAL OBJECTS


        // 2.2.13 CREATE PERMANENT SHARED OBJECTS -------------------------
        public void Create_permanent_shared_objects()
        {
            string clave = "SeCreatePermanentPrivilege";
            EstablecerComentario("SeCreatePermanentPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstablecerValor(valor);
                condicionMetodos.Add(false);
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.13 CREATE PERMANENT SHARED OBJECTS


        // 2.2.14 CREATE SYMBOLIC LINKS -------------------------
        public void Create_symbolic_links()
        {
            string clave = "SeCreateSymbolicLinkPrivilege";
            EstablecerComentario("SeCreateSymbolicLinkPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-83-0";
                string valoresRequeridos2 = "*S-1-5-32-544";
                string valoresRequeridos3 = "*S-1-5-83-0";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else if (VerificarValores(valor, valoresRequeridos2))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos2);
                }
                else if (VerificarValores(valor, valoresRequeridos3))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos3);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.14 CREATE SYMBOLIC LINKS


        // 2.2.15 DEBUG PROGRAMS -------------------------
        public void Debug_programs()
        {
            string clave = "SeDebugPrivilege";
            EstablecerComentario("SeDebugPrivilege");

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstablecerValor(valoresRequeridos);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstablecerValor(valor);
                }
            }
            else
            {
                EstablecerValor("Ninguno");
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.15 DEBUG PROGRAMS

        






    }
}
