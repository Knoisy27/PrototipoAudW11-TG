using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Text;

namespace PrototipoAuditoriaWin11
{
    internal class Logica
    {
        //private Panel panelDinamicoResultados;
        private DataGridView dgvRec;
        private Dictionary<string, string> configuraciones;
        public List<bool> condicionMetodos = new List<bool>();
        private List<Configuracion> configuracionesPendientes = new List<Configuracion>();

        public Logica(DataGridView dataGridView)
        {
            dgvRec = dataGridView;
            configuraciones = new Dictionary<string, string>();
            GuardarDatosCFG();
        }



        public void LimpiarDatosCFG() 
        {
            configuraciones.Clear();
            GuardarDatosCFG();
        }

        // LOGICA PARA LEER Y GUARDAR LOS DATOS DEL ARCHIVO SECURITY.CFG -------------------------
        public void GuardarDatosCFG()
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

                    // Divide la línea solo en la primera aparición de '='
                    string[] partes = linea.Split(new char[] { '=' }, 2);

                    if (partes.Length == 2)
                    {
                        string clave = partes[0].Trim();
                        string valor = partes[1].Trim();

                        // Almacena la configuración en el diccionario
                        configuraciones[clave] = valor;
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





        // Método para agregar todas las configuraciones pendientes al DataGridView
        public void AgregarConfiguracionesPendientes()
        {
            // Suspender las actualizaciones de diseño del DataGridView
            dgvRec.SuspendLayout();

            // Agregar cada configuración pendiente como una fila en el DataGridView
            foreach (var configuracion in configuracionesPendientes)
            {
                CrearFila(configuracion);
            }

            // Limpiar la lista después de agregar todas las configuraciones
            configuracionesPendientes.Clear();

            // Reanudar las actualizaciones de diseño del DataGridView
            dgvRec.ResumeLayout();
        }





        // Método para crear una fila en el DataGridView a partir de una configuración
        private void CrearFila(Configuracion configuracion)
        {
            DataGridViewRow row = new DataGridViewRow();

            row.CreateCells(dgvRec);
            row.Cells[0].Value = configuracion.Politica;
            row.Cells[1].Value = configuracion.Clave;
            row.Cells[2].Value = configuracion.Valor;
            row.Cells[3].Value = configuracion.Recomendacion;

            dgvRec.Rows.Add(row);
        }





        private void EstablecerColor(bool condicion, int rowIndex)
        {

            DataGridViewCellStyle estilo = dgvRec.Rows[rowIndex].DefaultCellStyle;
            estilo.BackColor = condicion ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightCoral;

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





        // FUNCION AUXILIAR PARA SEPARAR POR LA COMA DESDE UN STRING -------------------------
        public bool VerificarValores(string cadena, string valoresRequeridos)
        {
            // Dividir la cadena de valores requeridos en substrings basados en la coma como delimitador
            string[] valoresArray = valoresRequeridos.Split(',');
            // Dividir la cadena original en substrings basados en la coma como delimitador
            string[] substrings = cadena.Split(',');
            // Verificar que la cantidad de substrings sea igual a la cantidad de valores requeridos
            if (substrings.Length != valoresArray.Length)
            {
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

        private void EstConfig(string politica, string clave, string valor, string recomendacion)
        {
            Configuracion configuracion = new Configuracion(politica, clave, valor, recomendacion);
            configuracionesPendientes.Add(configuracion);
        }

        public string ObtenerInformacionMetodo(string clave)
        {
            string rutaArchivoConfig = @"C:\\Users\\josel\\source\\repos\\PrototipoAuditoriaWin11\\PrototipoAuditoriaWin11\\recomendaciones.txt";

            StringBuilder informacion = new StringBuilder();
            bool encontrado = false;

            try
            {
                using (StreamReader sr = new StreamReader(rutaArchivoConfig))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        // Buscar la entrada correspondiente al nombre del método
                        if (linea.StartsWith("#" + clave))
                        {
                            encontrado = true;
                            continue;
                        }
                        if (encontrado)
                        {
                            // Leer la descripción hasta encontrar el próximo carácter especial #
                            if (linea.StartsWith("#"))
                            {
                                break;
                            }
                            informacion.AppendLine(linea);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                informacion.Append($"Error al leer el archivo de configuración: {ex.Message}");
            }

            // Si la palabra clave no se encontró, mostrar un mensaje
            if (!encontrado)
            {
                informacion.Append($"No se encontró información para la política {clave}");
            }

            return informacion.ToString();
        }




        // 1.1.1 ENFORCE PASSWORD HISTORY -------------------------
        public void Analizar_Enforce_Password_History()
        {
            string politica = "Exigir historial de contraseñas";
            string clave = "PasswordHistorySize";
            string recomendacion = "24 o más contraseñas";
            if (configuraciones.ContainsKey(clave))
            {
                long valor = Convert.ToInt64(configuraciones[clave]);
                if (valor >= 24)
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }

            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.1.1 ENFORCE PASSWORD HISTORY

        // 1.1.2 MAXIMUM PASSWORD AGE -------------------------
        public void Analizar_Maximum_Password_Age()
        {
            string clave = "MaximumPasswordAge";
            string politica = "Vigencia máxima de la contraseña";
            string recomendacion = "365 días o menos, pero no 0";
            if (configuraciones.ContainsKey(clave))
            {
                long valor = Convert.ToInt64(configuraciones[clave]);
                if (valor > 0 && valor <= 365)
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.1.1 MAXIMUM PASSWORD AGE

        // 1.1.3 MINIMUM PASSWORD AGE -------------------------
        public void Analizar_MinimumPasswordAge()
        {
            string politica = "Vigencia mínima de la contraseña";
            string clave = "MinimumPasswordAge";
            string recomendacion = "1 o más días";
            if (configuraciones.ContainsKey("MinimumPasswordAge"))
            {
                long valor = Convert.ToInt64(configuraciones[clave]);
                if (valor > 0)
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }

            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.1.3 MINIMUM PASSWORD AGE


        // 1.1.4 MINIMUM PASSWORD LENGTH -------------------------
        public void Analizar_MinimumPasswordLength()
        {
            string politica = "Longitud mínima de la contraseña";
            string clave = "MinimumPasswordLength";
            string recomendacion = "14 o más caracteres";
            if (configuraciones.ContainsKey("MinimumPasswordLength"))
            {
                long valor = Convert.ToInt64(configuraciones[clave]);
                if (valor > 13)
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.1.4 MINIMUM PASSWORD LENGTH


        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------
        public void Analizar_Password_must_meet_complexity_requirements()
        {
            string politica = "La contraseña debe cumplir con los requisitos de complegidad";
            string clave = "PasswordComplexity";
            string recomendacion = "Activado";
            if (configuraciones.ContainsKey(clave))
            {
                long valor = Convert.ToInt64(configuraciones[clave]);
                if (valor == 1)
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else if (valor == 0)
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS


        // 1.1.6 MINUMUM PASSWORD LENGTH LIMITS -------------------------
        public void Analizar_Relax_minimum_password_length_limits()
        {
            // Definir los valores de la política y la recomendación
            string politica = "Reducir los límites de longitud mínima de la contraseña";
            string clave = "MACHINE\\System\\CurrentControlSet\\Control\\SAM\\RelaxMinimumPasswordLengthLimits";
            string recomendacion = "Activado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else if (valor == "4,0")
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }

        }
        // ------------------------- 1.1.6 MINUMUM PASSWORD LENGTH LIMITS -------------------------



        // 1.1.7 STORE PASSWORDS USING REVERSIBLE ENCRYPTION -------------------------
        public void Analizar_Clear_Text_Password()
        {

            // Definir la política y la recomendación
            string politica = "Almacenar contraseñas con cifrado reversible";
            string clave = "ClearTextPassword";
            long valor = Convert.ToInt64(configuraciones[clave]);
            string recomendacion = "Desactivado";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey(clave))
            {
                // Verificar el valor de la configuración
                if (valor == 1)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else if (valor == 0)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.1.7 STORE PASSWORDS USING REVERSIBLE ENCRYPTION


        // 1.2.1 ACCOUNT LOCKOUT DURATION -------------------------
        public void Analizar_Account_lockout_duration()
        {


            // Definir la política y la recomendación
            string politica = "Duración del bloqueo de la cuenta";
            string clave = "LockoutDuration";
            long valor = Convert.ToInt64(configuraciones[clave]);
            string recomendacion = "15 o más minutos.";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("LockoutDuration"))
            {
                // Verificar si el valor cumple con la condición
                if (valor >= 15)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.2.1 ACCOUNT LOCKOUT DURATION


        // 1.2.2 ACCOUNT LOCKOUT THRESHOLD -------------------------
        public void Analizar_Account_lockout_threshold()
        {

            // Definir la política y la recomendación
            string politica = "Umbral de bloqueo de cuenta";
            string clave = "LockoutBadCount";
            long valor = Convert.ToInt64(configuraciones[clave]);
            string recomendacion = "5 o menos intentos de inicio de sesión no válidos, pero no 0.";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("LockoutBadCount"))
            {
                // Verificar si el valor cumple con la condición
                if (valor <= 5 && valor != 0)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.2.2 ACCOUNT LOCKOUT THRESHOLD


        // 1.2.3 ALLOW ADMINISTRATOR ACCOUNT LOCKOUT -------------------------
        public void Analizar_Allow_Administrator_account_lockout()
        {

            // Definir la política y la recomendación
            string politica = "Permitir bloqueo de cuenta de administrador";
            string clave = "AllowAdministratorLockout";
            long valor = Convert.ToInt64(configuraciones[clave]);
            string recomendacion = "Activado";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("AllowAdministratorLockout"))
            {
                // Verificar si el valor cumple con la condición
                if (valor == 1)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else if (valor == 0)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.2.3 ALLOW ADMINISTRATOR ACCOUNT LOCKOUT


        // 1.2.4 RESET ACCOUNT LOCKOUT COUNTER AFTER -------------------------
        public void Analizar_Reset_account_lockout_counter_after()
        {

            // Definir la política y la recomendación
            string politica = "Restablecer el bloqueo de la cuenta después de";
            string clave = "ResetLockoutCount";
            long valor = Convert.ToInt64(configuraciones[clave]);
            string recomendacion = "15 o más minutos.";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("ResetLockoutCount"))
            {
                // Verificar si el valor cumple con la condición
                if (valor >= 15)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
                else
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 1.2.4 RESET ACCOUNT LOCKOUT COUNTER AFTER


        // 2.2.1 ACCESS CREDENTIAL MANAGER AS A TRUSTED CALLER -------------------------
        public void Analizar_Access_Credential_Manager_as_a_trusted_caller()
        {
            string politica = "Acceso al Administrador de credenciales como un llamador de confianza";
            string clave = "SeTrustedCredManAccessPrivilege";
            string recomendacion = "Nadie";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.1 ACCESS CREDENTIAL MANAGER AS A TRUSTED CALLER

        // 2.2.2 ACCESS THIS COMPUTER FROM THE NETWORK -------------------------
        public void Analizar_Access_this_computer_from_the_network()
        {

            string politica = "Tener acceso a este equipo desde la red";
            string clave = "SeNetworkLogonRight";
            string recomendacion = "Administradores, Usuarios de Escritorio Remoto.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-14";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.2 ACCESS THIS COMPUTER FROM THE NETWORK

        // 2.2.3 ACT AS PART OF THE OPERATING SYSTEM -------------------------
        public void Analizar_Act_as_part_of_the_operating_system()
        {

            string politica = "Actuar como parte del sistema operativo";
            string clave = "SeTcbPrivilege";
            string recomendacion = "Nadie";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.3 ACT AS PART OF THE OPERATING SYSTEM

        // 2.2.4 ADJUST MEMORY QUOTAS FOR A PROCESS -------------------------
        public void Analizar_Adjust_memory_quotas_for_a_process()
        {
            string politica = "Ajustar las cuotas de la memoria para un proceso";
            string clave = "SeIncreaseQuotaPrivilege";
            string recomendacion = "Administradores, SERVICIO LOCAL, SERVICIO DE RED.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-19,*S-1-5-20";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.4 ADJUST MEMORY QUOTAS FOR A PROCESS

        // 2.2.5 ALLOW LOG ON LOCALLY -------------------------
        public void Analizar_Allow_log_on_locally()
        {
            string politica = "Permitir inicio de sesión local";
            string clave = "SelongeractiveLogonRight";
            string recomendacion = "Administradores, Usuarios.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-32-545";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.5 ALLOW LOG ON LOCALLY


        // 2.2.6 ALLOW LOG ON THROUGH REMOTE DESKTOP SERVICES -------------------------
        public void Analizar_Allow_log_on_through_Remote_Desktop_Services()
        {
            string politica = "Permitir inicio de sesión a través de Servicios de Escritorio remoto";
            string clave = "SeRemoteInteractiveLogonRight";
            string recomendacion = "Administradores, Usuarios de Escritorio Remoto";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-32-555";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.6 ALLOW LOG ON THROUGH REMOTE DESKTOP SERVICES


        // 2.2.7 BACK UP FILES AND DIRECTORIES -------------------------
        public void Analizar_Back_up_files_and_directories()
        {
            string politica = "Hacer copias de seguridad de archivos y directorios";
            string clave = "SeRestorePrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.7 BACK UP FILES AND DIRECTORIES


        // 2.2.8 CHANGE THE SYSTEM TIME -------------------------
        public void Analizar_Change_the_system_time()
        {
            string politica = "Cambiar la hora del sistema";
            string clave = "SeSystemtimePrivilege";
            string recomendacion = "Administradores, SERVICIO LOCAL.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-19";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.8 CHANGE THE SYSTEM TIME


        // 2.2.9 CHANGE THE TIME ZONE -------------------------
        public void Analizar_Change_the_time_zone()
        {
            string politica = "Cambiar la zona horaria";
            string clave = "SeTimeZonePrivilege";
            string recomendacion = "Administrador, SERVICIO LOCAL. Usuarios.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-19,*S-1-5-32-544,*S-1-5-32-545";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.9 CHANGE THE TIME ZONE


        // 2.2.10 CREATE A PAGEFILE -------------------------
        public void Analizar_Create_a_pagefile()
        {
            string politica = "Crear un archivo de paginación\r\n";
            string clave = "SeCreatePagefilePrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.10 CREATE A PAGEFILE

        // 2.2.11 CREATE A TOKEN OBJECT' IS SET TO 'NO ONE' -------------------------
        public void Analizar_Create_a_token_object_is_set_to_No_One()
        {
            string politica = "Crear un objeto token\r\n";
            string clave = "SeCreateTokenPrivilege";
            string recomendacion = "Ninguno";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.11 CREATE A TOKEN OBJECT' IS SET TO 'NO ONE'


        // 2.2.12 CREATE GLOBAL OBJECTS -------------------------
        public void Analizar_Create_global_objects()
        {
            string politica = "Crear objetos globales";
            string clave = "SeCreateGlobalPrivilege";
            string recomendacion = "Administradores, SERVICIO LOCAL, SERVICIO DE RED, SERVICIO.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-19,*S-1-5-20,*S-1-5-32-544,*S-1-5-6";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.12 CREATE GLOBAL OBJECTS


        // 2.2.13 CREATE PERMANENT SHARED OBJECTS -------------------------
        public void Analizar_Create_permanent_shared_objects()
        {
            string politica = "Crear objetos compartidos permanentes";
            string clave = "SeCreatePermanentPrivilege";
            string recomendacion = "Ninguno";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.13 CREATE PERMANENT SHARED OBJECTS


        // 2.2.14 CREATE SYMBOLIC LINKS -------------------------
        public void Analizar_Create_symbolic_links()
        {
            string politica = "Crear vínculos simbólicos";
            string clave = "SeCreateSymbolicLinkPrivilege";
            string recomendacion = @"Administradores y (cuando la función Hyper-V está instalada) NT VIRTUAL MACHINE\Virtual Machines.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-83-0";
                string valoresRequeridos2 = "*S-1-5-32-544";
                string valoresRequeridos3 = "*S-1-5-83-0";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else if (VerificarValores(valor, valoresRequeridos2))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos2, recomendacion);
                }
                else if (VerificarValores(valor, valoresRequeridos3))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos3, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.14 CREATE SYMBOLIC LINKS


        // 2.2.15 DEBUG PROGRAMS -------------------------
        public void Analizar_Debug_programs()
        {
            string politica = "Depurar programas";
            string clave = "SeDebugPrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.15 DEBUG PROGRAMS


        // 2.2.16 DENY ACCESS TO THIS COMPUTER FROM NETWORK -------------------------
        public void Analizar_Deny_access_to_this_computer_from_the_network()
        {
            string politica = "Denegar el acceso desde la red a este equipo\r\n";
            string clave = "SeDenyNetworkLogonRight";
            string recomendacion = "Invitados";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-546";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.16 DENY ACCESS TO THIS COMPUTER FROM NETWORK


        // 2.2.17 DENY LOG ON AS A BATCH JOB -------------------------
        public void Analizar_Deny_log_on_as_a_batch_job()
        {
            string politica = "Denegar el inicio de sesión como trabajo por lotes";
            string clave = "SeDenyBatchLogonRight";
            string recomendacion = "Invitados";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-546";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.17 DENY LOG ON AS A BATCH JOB


        // 2.2.18 DENY LOG ON AS A SERVICE -------------------------
        public void Analizar_Deny_log_on_as_a_service()
        {
            string politica = "Denegar el inicio de sesión como servicio";
            string clave = "SeDenyServiceLogonRight";
            string recomendacion = "Invitados";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-546";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.18 DENY LOG ON AS A SERVICE


        // 2.2.19 DENY LOG ON LOCALLY -------------------------
        public void Analizar_Deny_log_on_locally()
        {
            string politica = "Denegar el inicio de sesión localmente";
            string clave = "SeDenyInteractiveLogonRight";
            string recomendacion = "Invitados";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-546";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.19 DENY LOG ON LOCALLY


        // 2.2.20 DENY LOG ON THROUGH REMOTE DESKTOP SERVICES -------------------------
        public void Analizar_Deny_log_on_through_Remote_Desktop_Services()
        {
            string politica = "Denegar inicio de sesión a través de Servicios de Escritorio remoto";
            string clave = "SeDenyRemoteInteractiveLogonRight";
            string recomendacion = "Invitados";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-546";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.20 DENY LOG ON THROUGH REMOTE DESKTOP SERVICES


        // 2.2.21 ENABLE COMPUTER AND USER ACCOUNTS TO BE TRUSTED FOR DELEGATION -------------------------
        public void Analizar_Enable_computer_and_user_accounts_to_betrusted_for_delegation()
        {
            string politica = "Habilitar confianza con el equipo y las cuentas de usuario para delegación";
            string clave = "SeEnableDelegationPrivilege";
            string recomendacion = "Nadie";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.21 ENABLE COMPUTER AND USER ACCOUNTS TO BE TRUSTED FOR DELEGATION


        // 2.2.22 FORCE SHUTDOWN FROM A REMOTE SYSTEM -------------------------
        public void Analizar_Force_shutdown_from_a_remote_system()
        {
            string politica = "Forzar cierre desde un sistema remoto";
            string clave = "SeRemoteShutdownPrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.22 FORCE SHUTDOWN FROM A REMOTE SYSTEM


        // 2.2.23 GENERATE SECURITY AUDITS -------------------------
        public void Analizar_Generate_security_audits()
        {
            string politica = "Generar auditorías de seguridad";
            string clave = "SeAuditPrivilege";
            string recomendacion = "SERVICIO LOCAL, SERVICIO DE RED.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-19,*S-1-5-20";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.23 GENERATE SECURITY AUDITS


        // 2.2.24 IMPERSONATE A CLIENT AFTER AUTHENTICATION -------------------------
        public void Analizar_Impersonate_a_client_after_authentication()
        {
            string politica = "Suplantar a un cliente tras la autenticación";
            string clave = "SeImpersonatePrivilege";
            string recomendacion = "Administradores, SERVICIO LOCAL, SERVICIO DE RED, SERVICIO. ";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-19,*S-1-5-20,*S-1-5-6";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.24 IMPERSONATE A CLIENT AFTER AUTHENTICATION


        // 2.2.25 INCREASE SCHEDULING PRIORITY -------------------------
        public void Analizar_Increase_scheduling_priority()
        {
            string politica = "Aumentar prioridad de programación";
            string clave = "SeIncreaseBasePriorityPrivilege";
            string recomendacion = "Administradores, Gestor de Ventanas Grupo Gestor de Ventanas.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-90-0";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.25 INCREASE SCHEDULING PRIORITY


        // 2.2.26 LOAD AND UNLOAD DEVICE DRIVERS -------------------------
        public void Analizar_Load_and_unload_device_drivers()
        {
            string politica = "Cargar y descargar controladores de dispositivo";
            string clave = "SeLoadDriverPrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.26 LOAD AND UNLOAD DEVICE DRIVERS


        // 2.2.27 LOCK PAGES IN MEMORY -------------------------
        public void Analizar_Lock_pages_in_memory()
        {
            string politica = "Bloquear páginas en la memoria";
            string clave = "SeLockMemoryPrivilege";
            string recomendacion = "Nadie";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.27 LOCK PAGES IN MEMORY


        // 2.2.28 LOG ON AS A BATCH JOB -------------------------
        public void Analizar_Log_on_as_a_batch_job()
        {
            string politica = "Iniciar sesión como proceso por lotes";
            string clave = "SeBatchLogonRight";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.28 LOG ON AS A BATCH JOB


        // 2.2.29 LOG ON AS A SERVICE -------------------------
        public void Analizar_Log_on_as_a_service()
        {
            string politica = "Iniciar sesión como servicio";
            string clave = "SeBatchLogonRight";
            string recomendacion = @"Nadie (cuando la característica Hyper-V está instalada) NT VIRTUAL MACHINE\Virtual Machines o (cuando se utiliza Windows Defender Application Guard, como en el perfil Next Generation Windows Security) WDAGUtilityAccount. ";

            if (!configuraciones.ContainsKey(clave))
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
            else if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-83-0";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }

            }
        }
        // ------------------------- 2.2.29 LOG ON AS A SERVICE


        // 2.2.30 MANAGE AUDITING AND SECURITY LOG -------------------------
        public void Analizar_Manage_auditing_and_security_log()
        {
            string politica = "Administrar registro de seguridad y auditoría";
            string clave = "SeSecurityPrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.30 MANAGE AUDITING AND SECURITY LOG


        // 2.2.31 MODIFY AN OBJECT LABEL -------------------------
        public void Analizar_Modify_an_object_label()
        {
            string politica = "Modificar la etiqueta de un objeto";
            string clave = "SeRelabelPrivilege";
            string recomendacion = "Nadie";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.31 MODIFY AN OBJECT LABEL


        // 2.2.32 MODIFY FIRMWARE ENVIRONMENT VALUES -------------------------
        public void Analizar_Modify_firmware_environment_values()
        {
            string politica = "Modificar valores de entorno firmware";
            string clave = "SeSystemEnvironmentPrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.32 MODIFY FIRMWARE ENVIRONMENT VALUES


        // 2.2.33 PERFORM VOLUME MAINTENANCE TASKS -------------------------
        public void Analizar_Perform_volume_maintenance_tasks()
        {
            string politica = "Realizar tareas de mantenimiento del volumen";
            string clave = "SeManageVolumePrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.33 PERFORM VOLUME MAINTENANCE TASKS


        // 2.2.34 PROFILE SINGLE PROCESS -------------------------
        public void Analizar_Profile_single_process()
        {
            string politica = "Generar perfiles de un solo proceso\r\n";
            string clave = "SeProfileSingleProcessPrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.34 PROFILE SINGLE PROCESS


        // 2.2.35 PROFILE SYSTEM PERFORMANCE -------------------------
        public void Analizar_Profile_system_performance()
        {
            string politica = "Generar perfiles del rendimiento del sistema";
            string clave = "SeSystemProfilePrivilege";
            string recomendacion = @"Administradores, NT SERVICE\WdiServiceHost.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-80-3139157870-2983391045-3678747466-658725712-1809340420";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                EstConfig(politica, clave, "No definido", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.35 PROFILE SYSTEM PERFORMANCE


        // 2.2.36 REPLACE A PROCESS LEVEL TOKEN -------------------------
        // ------------------------- 2.2.36 REPLACE A PROCESS LEVEL TOKEN


        // 2.2.37 RESTORE FILES AND DIRECTORIES -------------------------
        // ------------------------- 2.2.37 RESTORE FILES AND DIRECTORIES


        // 2.2.38 SHUT DOWN THE SYSTEM -------------------------
        public void Analizar_Shut_down_the_system()
        {
            string politica = "Apagar el sistema";
            string clave = "SeShutdownPrivilege";
            string recomendacion = "Administradores, usuarios";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544,*S-1-5-32-545";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.2.38 SHUT DOWN THE SYSTEM


        // 2.2.39 TAKE OWNERSHIP OF FILES OR OTHER OBJECTS -------------------------
        public void Analizar_Take_ownership_of_files_or_other_objects()
        {
            string politica = "Tomar posesión de archivos y otros objetos";
            string clave = "SeTakeOwnershipPrivilege";
            string recomendacion = "Administradores";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];

                string valoresRequeridos = "*S-1-5-32-544";

                if (VerificarValores(valor, valoresRequeridos))
                {
                    condicionMetodos.Add(true);
                    EstConfig(politica, clave, valoresRequeridos, recomendacion);
                }
                else
                {
                    condicionMetodos.Add(false);
                    EstConfig(politica, clave, valor, recomendacion);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.2.39 TAKE OWNERSHIP OF FILES OR OTHER OBJECTS


        // 2.3.1.1 ACCOUNTS: BLOCK MICROSOFT ACCOUNTS -------------------------
        public void Analizar_Accounts__Block_Microsoft_accounts()
        {
            string politica = "Cuentas: bloquear cuentas de Microsoft";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\NoConnectedUser";
            string recomendacion = "Los usuarios no pueden añadir o iniciar sesión con cuentas de Microsoft";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,3")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.1.1 ACCOUNTS: BLOCK MICROSOFT ACCOUNTS



        // 2.3.1.2 ACCOUNTS: GUEST ACCOUNT STATUS -------------------------
        public void Analizar_Accounts__Guest_account_status()
        {
            string politica = "Cuentas: estado de la cuenta de invitado";
            string clave = "EnableGuestAccount";
            string recomendacion = "Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.1.2 ACCOUNTS: GUEST ACCOUNT STATUS


        // 2.3.1.3 ACCOUNTS: LIMIT LOCAL ACCOUNT USE OF BLANK PASSWORDS TO CONSOLE -------------------------
        public void Analizar_Accounts__Limit_local_account_use_of_blank_passwords_to_console_logon_only()
        {
            string politica = "Cuentas: limitar el uso de cuentas locales con contraseña en blanco solo para iniciar sesión en la consola";
            string clave = "MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\LimitBlankPasswordUse";
            string recomendacion = "Habiltado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.1.3 ACCOUNTS: LIMIT LOCAL ACCOUNT USE OF BLANK PASSWORDS TO CONSOLE


        // 2.3.1.4 ACCOUNTS: RENAME ADMINISTRATOR ACCOUNT -------------------------
        public void Analizar_Accounts__Rename_administrator_account()
        {
            string politica = "Cuentas: cambiar el nombre de la cuenta de administrador";
            string clave = "NewAdministratorName";
            string recomendacion = "Renombrar cuenta Administrador";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "Administrator")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.1.4 ACCOUNTS: RENAME ADMINISTRATOR ACCOUNT


        // 2.3.1.5 ACCOUNTS: RENAME GUEST ACCOUNT -------------------------
        public void Analizar_Accounts__Rename_guest_account()
        {
            string politica = "Cuentas: cambiar el nombre de cuenta de invitado";
            string clave = "NewGuestName";
            string recomendacion = "Renombrar cuenta Invitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "Guest")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.1.5 ACCOUNTS: RENAME GUEST ACCOUNT


        // 2.3.2.1 AUDIT: FORCE AUDIT POLICY SUBCATEGORY SETTINGS (WINDOWS VISTA OR LATER) TO OVERRIDE AUDIT POLICY CATEGORY SETTINGS -------------------------
        public void Analizar_Audit__Force_audit_policy_subcategory_settings__Windows_Vista_or_later__to_override_audit_policy_category_settings()
        {
            string politica = "Auditoría: forzar que la configuración de subcategoría de directiva de auditoría (Windows Vista o posterior) invalide la configuración de categoría de directiva de auditoría.";
            string clave = "MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\SCENoApplyLegacyAuditPolicy";
            string recomendacion = "Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.2.1 AUDIT: FORCE AUDIT POLICY SUBCATEGORY SETTINGS (WINDOWS VISTA OR LATER) TO OVERRIDE AUDIT POLICY CATEGORY SETTINGS


        // 2.3.2.2 AUDIT: SHUT DOWN SYSTEM IMMEDIATELY IF UNABLE TO LOG SECURITY AUDITS -------------------------
        public void Analizar_Audit__Shut_down_system_immediately_if__unable_to_log_security_audits()
        {
            string politica = "Auditoría: apagar el sistema de inmediato si no se pueden registrar las auditorías de seguridad";
            string clave = "MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\CrashOnAuditFail";
            string recomendacion = "Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.2.2 AUDIT: SHUT DOWN SYSTEM IMMEDIATELY IF UNABLE TO LOG SECURITY AUDITS


        // 2.3.4.1 DEVICES: PREVENT USERS FROM INSTALLING PRINTER DRIVERS -------------------------
        public void Analizar_Devices__Prevent_users_from_installing_printer__drivers()
        {
            string politica = "Dispositivos: impedir que los usuarios instalen controladores de impresora cuando se conecten a impresoras compartidas";
            string clave = "MACHINE\\System\\CurrentControlSet\\Control\\Print\\Providers\\LanMan Print Services\\Servers\\AddPrinterDrivers";
            string recomendacion = "Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.4.1 DEVICES: PREVENT USERS FROM INSTALLING PRINTER DRIVERS 


        // 2.3.7.1 INTERACTIVE LOGON: DO NOT REQUIRE CTRL+ALT+DEL -------------------------
        public void Analizar_Interactive_logon__Do_not_require__CTRL__ALT__DEL()
        {
            string politica = "Inicio de sesión interactivo: no requerir Ctrl+Alt+Supr";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\DisableCAD";
            string recomendacion = "Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.1 INTERACTIVE LOGON: DO NOT REQUIRE CTRL+ALT+DEL


        // 2.3.7.2 INTERACTIVE LOGON: DON'T DISPLAY LAST SIGNED-IN -------------------------
        public void Analizar_Interactive_logon__Dont_display_last_signed__in()
        {
            string politica = "Inicio de sesión interactivo: No mostrar último inicio de sesión";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\DontDisplayLastUserName";
            string recomendacion = "Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.2 INTERACTIVE LOGON: DON'T DISPLAY LAST SIGNED-IN


        // 2.3.7.3 INTERACTIVE LOGON: MACHINE ACCOUNT LOCKOUT THRESHOLD -------------------------
        public void Analizar_Interactive_logon__Machine_account_lockout_threshold()
        {
            string politica = "Inicio de sesión interactivo: umbral de la cuenta de la máquina.";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\MaxDevicePasswordFailedAttempts";
            string recomendacion = "10 o menos intentos de inicio de sesión no válidos, pero no 0.";

            if (configuraciones.ContainsKey(clave))
            {
                // Dividir la cadena de entrada en partes basadas en la coma
                string[] partes = configuraciones[clave].Split(',');
                long valor = Convert.ToInt64(partes[1]);
                if (valor > 0 && valor <= 10)
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.3 INTERACTIVE LOGON: MACHINE ACCOUNT LOCKOUT THRESHOLD


        // 2.3.7.4 INTERACTIVE LOGON: MACHINE INACTIVITY LIMIT -------------------------
        public void Analizar_Interactive_logon__Machine_inactivity_limit()
        {
            string politica = "Inicio de sesión interactivo: límite de inactividad de equipo.";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\InactivityTimeoutSecs";
            string recomendacion = "900 segundos o menos, pero no 0.";

            if (configuraciones.ContainsKey(clave))
            {
                string[] partes = configuraciones[clave].Split(',');
                long valor = Convert.ToInt64(partes[1]);
                if (valor > 0 && valor <= 900)
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.4 INTERACTIVE LOGON: MACHINE INACTIVITY LIMIT


        // 2.3.7.5 INTERACTIVE LOGON: MESSAGE TEXT FOR USERS ATTEMPTING TO LOG ON -------------------------
        public void Analizar_Interactive_logon__Message_text_for_users_attempting_to_log_on()
        {
            string politica = "Inicio de sesión interactivo: texto del mensaje para los usuarios que intentan iniciar una sesión";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\LegalNoticeText";
            string recomendacion = "Mostrar un mensaje de advertencia antes de iniciar sesión";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "7," && valor != "7")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.5 INTERACTIVE LOGON: MESSAGE TEXT FOR USERS ATTEMPTING TO LOG ON


        // 2.3.7.6 INTERACTIVE LOGON: MESSAGE TITLE FOR USERS ATTEMPTING TO LOG ON -------------------------
        public void Analizar_Interactive_logon__Message_title_for_users_attempting_to_log_on()
        {
            string politica = "Inicio de sesión interactivo: título del mensaje para los usuarios que intentan iniciar una sesión";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\LegalNoticeCaption";
            string recomendacion = "Mostrar un título de advertencia antes de iniciar sesión";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "1," && valor != "1" && valor != "\"\"")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.6 INTERACTIVE LOGON: MESSAGE TITLE FOR USERS ATTEMPTING TO LOG ON 


        // 2.3.7.7 INTERACTIVE LOGON: PROMPT USER TO CHANGE PASSWORD BEFORE EXPIRATION -------------------------
        public void Analizar_Interactive_logon__Prompt_user_to_change_password_before_expiration()
        {
            string politica = "Inicio de sesión interactivo: pedir al usuario que cambie la contraseña antes de que expire";
            string clave = "MACHINE\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\PasswordExpiryWarning";
            string recomendacion = @"Entre 5 y 14 días.";

            if (configuraciones.ContainsKey(clave))
            {
                string[] partes = configuraciones[clave].Split(',');
                long valor = Convert.ToInt64(partes[1]);
                if (valor >= 5 && valor <= 14)
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.7 INTERACTIVE LOGON: PROMPT USER TO CHANGE PASSWORD BEFORE EXPIRATION 


        // 2.3.7.8 INTERACTIVE LOGON: SMART CARD REMOVAL BEHAVIOR -------------------------
        public void Analizar_Interactive_logon__Smart_card_removal__behavior()
        {
            string politica = "Inicio de sesión interactivo: comportamiento de extracción de tarjeta inteligente";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\ScRemoveOption";
            string recomendacion = @"Bloquear estación de trabajo. Configurar este ajuste para Forzar cierre de sesión o Desconectar si una sesión de Servicios de Escritorio Remoto también se ajusta al punto de referencia. ";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "1,\"0\"")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.7.8 INTERACTIVE LOGON: SMART CARD REMOVAL BEHAVIOR 


        // 2.3.8.1 MICROSOFT NETWORK CLIENT: DIGITALLY SIGN COMMUNICATIONS (ALWAYS) -------------------------
        public void Analizar_Microsoft_network_client__Digitally_sign_communications__always()
        {
            string politica = "Cliente de red de Microsoft: firma digital de comunicaciones (siempre)";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanmanWorkstation\\Parameters\\RequireSecuritySignature";
            string recomendacion = @"Habiltado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.8.1 MICROSOFT NETWORK CLIENT: DIGITALLY SIGN COMMUNICATIONS (ALWAYS) 


        // 2.3.8.2 MICROSOFT NETWORK CLIENT: DIGITALLY SIGN COMMUNICATIONS (IF SERVER AGREES) -------------------------
        public void Analizar_Microsoft_network_client__Digitally_sign_communications__if_server_agrees__()
        {
            string politica = "Cliente de red de Microsoft: firma digital de comunicaciones (si el servidor está de acuerdo)";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanmanWorkstation\\Parameters\\EnableSecuritySignature";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.8.2 MICROSOFT NETWORK CLIENT: DIGITALLY SIGN COMMUNICATIONS (IF SERVER AGREES) 


        // 2.3.8.3 MICROSOFT NETWORK CLIENT: SEND UNENCRYPTED PASSWORD TO THIRD-PARTY SMB SERVERS -------------------------
        public void Analizar_Microsoft_network_client__Send_unencrypted_password_to_third__party_SMB_servers()
        {
            string politica = "Cliente de redes de Microsoft: enviar contraseña sin cifrar para conectar con servidores SMB de terceros\r\n";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanmanWorkstation\\Parameters\\EnableSecuritySignature";
            string recomendacion = @"Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.8.3 MICROSOFT NETWORK CLIENT: SEND UNENCRYPTED PASSWORD TO THIRD-PARTY SMB SERVERS 


        // 2.3.9.1 MICROSOFT NETWORK SERVER: AMOUNT OF IDLE TIME REQUIRED BEFORE SUSPENDING SESSION -------------------------
        public void Analizar_Microsoft_network_server__Amount_of_idle_time_required_before_suspending_session()
        {
            string politica = "Servidor de red Microsoft: tiempo de inactividad requerido antes de suspender la sesión";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\AutoDisconnect";
            string recomendacion = @"15 minutos o menos.";

            if (configuraciones.ContainsKey(clave))
            {
                string[] partes = configuraciones[clave].Split(',');
                long valor = Convert.ToInt64(partes[1]);
                if (valor <= 15)
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.9.1 MICROSOFT NETWORK SERVER: AMOUNT OF IDLE TIME REQUIRED BEFORE SUSPENDING SESSION 


        // 2.3.9.2 MICROSOFT NETWORK SERVER: DIGITALLY SIGN COMMUNICATIONS (ALWAYS) -------------------------
        public void Analizar_Microsoft_network_server__Digitally_sign_communications__always()
        {
            string politica = "Servidor de red de Microsoft: firma digital de comunicaciones (siempre)\r\n";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\RequireSecuritySignature";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.9.2 MICROSOFT NETWORK SERVER: DIGITALLY SIGN COMMUNICATIONS (ALWAYS) 


        // 2.3.9.3 MICROSOFT NETWORK SERVER: DIGITALLY SIGN COMMUNICATIONS (IF CLIENT AGREES) -------------------------
        public void Analizar_Microsoft_network_server__Digitally_sign_communications__if_client_agrees__()
        {
            string politica = "Servidor de red de Microsoft: firma digital de comunicaciones (si el cliente está de acuerdo)";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\EnableSecuritySignature";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.9.3 MICROSOFT NETWORK SERVER: DIGITALLY SIGN COMMUNICATIONS (IF CLIENT AGREES) 


        // 2.3.9.4 MICROSOFT NETWORK SERVER: DISCONNECT CLIENTS WHEN LOGON HOURS EXPIRE -------------------------
        public void Analizar_Microsoft_network_server__Disconnect_clients_when_logon_hours_expire()
        {
            string politica = "Servidor de red Microsoft: desconectar a los clientes cuando expiren las horas de inicio de sesión";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\EnableForcedLogOff";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.9.4 MICROSOFT NETWORK SERVER: DISCONNECT CLIENTS WHEN LOGON HOURS EXPIRE 


        // 2.3.9.5 MICROSOFT NETWORK SERVER: SERVER SPN TARGET NAME VALIDATION LEVEL -------------------------
        public void Analizar_Microsoft_network_server__Server_SPN_target_name_validation_level()
        {
            string politica = "Servidor de red Microsoft: nivel de validación de nombres de destino SPN del servidor\r\n";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\SmbServerNameHardeningLevel";
            string recomendacion = @"Aceptar si lo proporciona el cliente. Configurar este parámetro como Requerido del cliente también se ajusta a la referencia";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.9.5 MICROSOFT NETWORK SERVER: SERVER SPN TARGET NAME VALIDATION LEVEL 


        // 2.3.10.1 NETWORK ACCESS: ALLOW ANONYMOUS SID/NAME TRANSLATION -------------------------
        public void Analizar_Network_access__Allow_anonymous__SID__Name_translation()
        {
            string politica = "Acceso de red: permitir traducción SID-nombre anónima";
            string clave = @"LSAAnonymousNameLookup";
            string recomendacion = @"Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                long valor = Convert.ToInt64(configuraciones[clave]);
                if (valor == 0)
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.1 NETWORK ACCESS: ALLOW ANONYMOUS SID/NAME TRANSLATION 


        // 2.3.10.2 NETWORK ACCESS: DO NOT ALLOW ANONYMOUS ENUMERATION OF SAM ACCOUNTS -------------------------
        public void Analizar_Network_access__Do_not_allow_anonymous__enumeration_of_SAM_accounts()
        {
            string politica = "Acceso a redes: no permitir enumeraciones anónimas de cuentas SAM";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\RestrictAnonymousSAM";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.2 NETWORK ACCESS: DO NOT ALLOW ANONYMOUS ENUMERATION OF SAM ACCOUNTS 


        // 2.3.10.3 NETWORK ACCESS: DO NOT ALLOW ANONYMOUS ENUMERATION OF SAM ACCOUNTS AND SHARES -------------------------
        public void Analizar_Network_access__Do_not_allow_anonymous__enumeration_of_SAM_accounts_and_shares()
        {
            string politica = "Acceso a redes: no permitir enumeraciones anónimas de cuentas y recursos compartidos SAM";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\RestrictAnonymous";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.3 NETWORK ACCESS: DO NOT ALLOW ANONYMOUS ENUMERATION OF SAM ACCOUNTS AND SHARES 


        // 2.3.10.4 NETWORK ACCESS: DO NOT ALLOW STORAGE OF PASSWORDS AND CREDENTIALS FOR NETWORK AUTHENTICATION -------------------------
        public void Analizar_Network_access__Do_not_allow_storage_of_passwords_and_credentials_for_network_authentication()
        {
            string politica = "Acceso a redes: no permitir el almacenamiento de contraseñas y credenciales para la autenticación de la red";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\DisableDomainCreds";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.4 NETWORK ACCESS: DO NOT ALLOW STORAGE OF PASSWORDS AND CREDENTIALS FOR NETWORK AUTHENTICATION 


        // 2.3.10.5 NETWORK ACCESS: LET EVERYONE PERMISSIONS APPLY TO ANONYMOUS USERS -------------------------
        public void Analizar_Network_access__Let_Everyone_permissions_apply_to_anonymous_users()
        {
            string politica = "Acceso a redes: permitir la aplicación de los permisos Todos a los usuarios anónimos";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\EveryoneIncludesAnonymous";
            string recomendacion = @"Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.5 NETWORK ACCESS: LET EVERYONE PERMISSIONS APPLY TO ANONYMOUS USERS 


        // 2.3.10.6 NETWORK ACCESS: NAMED PIPES THAT CAN BE ACCESSED ANONYMOUSLY -------------------------
        public void Analizar_Network_access__Named_Pipes_that_can_be_accessed_anonymously()
        {
            string politica = "Acceso a redes: canalizaciones con nombre accesibles anónimamente";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\NullSessionPipes";
            string recomendacion = @"<en blanco> (es decir, Ninguno)";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "7,")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.6 NETWORK ACCESS: NAMED PIPES THAT CAN BE ACCESSED ANONYMOUSLY 


        // 2.3.10.7 NETWORK ACCESS: REMOTELY ACCESSIBLE REGISTRY PATHS -------------------------
        public void Analizar_Network_access__Remotely_accessible_registry_paths()
        {
            string politica = "Acceso a redes: rutas del Registro accesibles remotamente";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\SecurePipeServers\\Winreg\\AllowedExactPaths\\Machine";
            string recomendacion = @"System\CurrentControlSet\Control\ProductOptions System\CurrentControlSet\Control\Server Applications Software\Microsoft\Windows NT\CurrentVersion ";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                string[] valores = configuraciones[clave].Split(',');
                string subcadena = string.Join(",", valores.Skip(1));

                string valoresRequeridos = "System\\CurrentControlSet\\Control\\ProductOptions,System\\CurrentControlSet\\Control\\Server Applications,Software\\Microsoft\\Windows NT\\CurrentVersion";

                if (VerificarValores(subcadena, valoresRequeridos))
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.7 NETWORK ACCESS: REMOTELY ACCESSIBLE REGISTRY PATHS 


        // 2.3.10.8 NETWORK ACCESS: REMOTELY ACCESSIBLE REGISTRY PATHS AND SUB-PATHS -------------------------
        public void Analizar_Network_access__Remotely_accessible_registry_paths_and_sub__paths()
        {
            string politica = "Acceso de red: rutas y subrutas de Registro accesibles remotamente\r\n";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\SecurePipeServers\\Winreg\\AllowedPaths\\Machine";
            string recomendacion = @"System\CurrentControlSet\Control\Print\Printers System\CurrentControlSet\Services\Eventlog Software\Microsoft\OLAP Server Software\Microsoft\Windows NT\CurrentVersion\Print Software\Microsoft\Windows NT\CurrentVersion\Windows System\CurrentControlSet\Control\ContentIndex System\CurrentControlSet\Control\Terminal Server System\CurrentControlSet\Control\Terminal Server\UserConfig System\CurrentControlSet\Control\Terminal Server\DefaultUserConfiguration Software\Microsoft\Windows NT\CurrentVersion\Perflib System\CurrentControlSet\Services\SysmonLog ";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                string[] valores = configuraciones[clave].Split(',');
                string subcadena = string.Join(",", valores.Skip(1));

                string valoresRequeridos = "System\\CurrentControlSet\\Control\\Print\\Printers,System\\CurrentControlSet\\Services\\Eventlog,Software\\Microsoft\\OLAP Server,Software\\Microsoft\\Windows NT\\CurrentVersion\\Print,Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows,System\\CurrentControlSet\\Control\\ContentIndex,System\\CurrentControlSet\\Control\\Terminal Server,System\\CurrentControlSet\\Control\\Terminal Server\\UserConfig,System\\CurrentControlSet\\Control\\Terminal Server\\DefaultUserConfiguration,Software\\Microsoft\\Windows NT\\CurrentVersion\\Perflib,System\\CurrentControlSet\\Services\\SysmonLog";

                if (VerificarValores(subcadena, valoresRequeridos))
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.8 NETWORK ACCESS: REMOTELY ACCESSIBLE REGISTRY PATHS AND SUB-PATHS 


        // 2.3.10.9 NETWORK ACCESS: RESTRICT ANONYMOUS ACCESS TO NAMED PIPES AND SHARES -------------------------
        public void Analizar_Network_access__Restrict_anonymous__access_to_Named_Pipes_and_Shares()
        {
            string politica = "Acceso a redes: restringir acceso anónimo a canalizaciones con nombre y recursos compartidos";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\RestrictNullSessAccess";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.9 NETWORK ACCESS: RESTRICT ANONYMOUS ACCESS TO NAMED PIPES AND SHARES 


        // 2.3.10.10 NETWORK ACCESS: RESTRICT CLIENTS ALLOWED TO MAKE REMOTE CALLS TO SAM -------------------------
        public void Analizar_Network_access__Restrict_clients_allowed_to_make_remote_calls_to_SAM()
        {
            string politica = "Acceso de red: evitar que los clientes con permiso realicen llamadas remotas a SAM";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\RestrictRemoteSAM";
            string recomendacion = @"Administradores: Acceso Remoto: Permitir";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "1,\"O:BAG:BAD:(A;;RC;;;BA)\"")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.10 NETWORK ACCESS: RESTRICT CLIENTS ALLOWED TO MAKE REMOTE CALLS TO SAM 


        // 2.3.10.11 NETWORK ACCESS: SHARES THAT CAN BE ACCESSED ANONYMOUSLY -------------------------
        public void Analizar_Network_access__Shares_that_can_be_accessed_anonymously()
        {
            string politica = "Acceso a redes: recursos compartidos accesibles anónimamente";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LanManServer\\Parameters\\NullSessionShares";
            string recomendacion = @"<en blanco> (es decir, Ninguno).";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "7,")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.11 NETWORK ACCESS: SHARES THAT CAN BE ACCESSED ANONYMOUSLY 


        // 2.3.10.12 NETWORK ACCESS: SHARING AND SECURITY MODEL FOR LOCAL ACCOUNTS -------------------------
        public void Analizar_Network_access__Sharing_and_security_model_for_local_accounts()
        {
            string politica = "Acceso a redes: modelo de seguridad y uso compartido para cuentas locales";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\ForceGuest";
            string recomendacion = @"Clásico: los usuarios locales se autentican como ellos mismos.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.10.12 NETWORK ACCESS: SHARING AND SECURITY MODEL FOR LOCAL ACCOUNTS 


        // 2.3.11.1 NETWORK SECURITY: ALLOW LOCAL SYSTEM TO USE COMPUTER IDENTITY FOR NTLM -------------------------
        public void Analizar_Network_security__Allow_Local_System_to_use_computer_identity_for_NTLM()
        {
            string politica = "Seguridad de red: permitir que LocalSystem use la identidad del PC para NTLM\r\n";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\UseMachineId";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.1 NETWORK SECURITY: ALLOW LOCAL SYSTEM TO USE COMPUTER IDENTITY FOR NTLM 


        // 2.3.11.2 NETWORK SECURITY: ALLOW LOCALSYSTEM NULL SESSION FALLBACK -------------------------
        public void Analizar_Network_security__Allow_LocalSystem_NULL_session_fallback()
        {
            string politica = "Seguridad de red: permitir retroceso a sesión NULL de LocalSystem ";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\MSV1_0\\allownullsessionfallback";
            string recomendacion = @"Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.2 NETWORK SECURITY: ALLOW LOCALSYSTEM NULL SESSION FALLBACK 


        // 2.3.11.3 NETWORK SECURITY: ALLOW PKU2U AUTHENTICATION REQUESTS TO THIS COMPUTER TO USE ONLINE IDENTITIES -------------------------
        public void Analizar_Network_Security__Allow_PKU2U_authentication_requests_to_this_computer_to_use_online_identities()
        {
            string politica = "Seguridad de red: Permite las solicitudes de autenticación PKU2U a este equipo para usar identidades en línea.";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\pku2u\\AllowOnlineID";
            string recomendacion = @"Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.3 NETWORK SECURITY: ALLOW PKU2U AUTHENTICATION REQUESTS TO THIS COMPUTER TO USE ONLINE IDENTITIES 


        // 2.3.11.4 NETWORK SECURITY: CONFIGURE ENCRYPTION TYPES ALLOWED FOR KERBEROS -------------------------
        public void Analizar_Network_security__Configure_encryption_types_allowed_for_Kerberos()
        {
            string politica = "Seguridad de red: configurar tipos de cifrado permitidos para Kerberos\r\n";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Kerberos\\Parameters\\SupportedEncryptionTypes";
            string recomendacion = @"AES128_HMAC_SHA1, AES256_HMAC_SHA1, Futuros tipos de cifrado.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,2147483640")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.4 NETWORK SECURITY: CONFIGURE ENCRYPTION TYPES ALLOWED FOR KERBEROS 


        // 2.3.11.5 NETWORK SECURITY: DO NOT STORE LAN MANAGER HASH VALUE ON NEXT PASSWORD CHANGE -------------------------
        public void Analizar_Network_security__Do_not_store_LAN_Manager_hash_value_on_next_password_change()
        {
            string politica = "Seguridad de red: no almacenar valor de hash de LAN Manager en el próximo cambio de contraseña";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\NoLMHash";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.5 NETWORK SECURITY: DO NOT STORE LAN MANAGER HASH VALUE ON NEXT PASSWORD CHANGE 


        // 2.3.11.6 NETWORK SECURITY: FORCE LOGOFF WHEN LOGON HOURS EXPIRE -------------------------
        public void Analizar_Network_security__Force_logoff_when_logon_hours_expire()
        {
            string politica = "Seguridad de red: forzar el cierre de sesión cuando expire la hora de inicio de sesión";
            string clave = @"ForceLogoffWhenHourExpire";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                long valor = Convert.ToInt64(configuraciones[clave]);
                if (valor == 1)
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor.ToString(), recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.6 NETWORK SECURITY: FORCE LOGOFF WHEN LOGON HOURS EXPIRE 


        // 2.3.11.7 NETWORK SECURITY: LAN MANAGER AUTHENTICATION LEVEL -------------------------
        public void Analizar_Network_security__LAN_Manager__authentication_level()
        {
            string politica = "Seguridad de red: nivel de autenticación de LAN Manager";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\LmCompatibilityLevel";
            string recomendacion = @"MÁQUINA\NSistema\NActualControlSet\NControl\NLsa\NNNNNivel de compatibilidad con Lm";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,5")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.7 NETWORK SECURITY: LAN MANAGER AUTHENTICATION LEVEL 


        // 2.3.11.8 NETWORK SECURITY: LDAP CLIENT SIGNING REQUIREMENTS -------------------------
        public void Analizar_Network_security__LDAP_client_signing_requirements()
        {
            string politica = "Seguridad de red: requisitos de firma de cliente LDAP";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Services\\LDAP\\LDAPClientIntegrity";
            string recomendacion = @"Negociar firma. Configurar este parámetro en Requerir firma también se ajusta a la referencia.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.8 NETWORK SECURITY: LDAP CLIENT SIGNING REQUIREMENTS 


        // 2.3.11.9 NETWORK SECURITY: MINIMUM SESSION SECURITY FOR NTLM SSP BASED (INCLUDING SECURE RPC) CLIENTS -------------------------
        public void Analizar_Network_security__Minimum_session_security_for_NTLM_SSP_based__including_secure_RPC__clients()
        {
            string politica = "Seguridad de red: seguridad de sesión mínima para clientes NTLM basados en SSP (incluida RPC segura)";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\MSV1_0\\NTLMMinClientSec";
            string recomendacion = @"Requiere seguridad de sesión NTLMv2, Requiere encriptación de 128 bits. ";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,537395200")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.9 NETWORK SECURITY: MINIMUM SESSION SECURITY FOR NTLM SSP BASED (INCLUDING SECURE RPC) CLIENTS 


        // 2.3.11.10 NETWORK SECURITY: MINIMUM SESSION SECURITY FOR NTLM SSP BASED (INCLUDING SECURE RPC) SERVERS -------------------------
        public void Analizar_Network_security__Minimum_session__security_for_NTLM_SSP_based__including_secure_RPC__servers()
        {
            string politica = "Seguridad de red: seguridad de sesión mínima para servidores NTLM basados en SSP (incluida RPC segura)";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Lsa\\MSV1_0\\NTLMMinServerSec";
            string recomendacion = @"Requerir seguridad de sesión NTLMv2, Requerir encriptación de 128 bits.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,537395200")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.11.10 NETWORK SECURITY: MINIMUM SESSION SECURITY FOR NTLM SSP BASED (INCLUDING SECURE RPC) SERVERS 


        // 2.3.14.1 SYSTEM CRYPTOGRAPHY: FORCE STRONG KEY PROTECTION FOR USER KEYS STORED ON THE COMPUTER -------------------------
        public void Analizar_System_cryptography__Force_strong_key_protection_for_user_keys_stored_on_the_computer()
        {
            string politica = "Criptografía de sistema: forzar la protección con claves seguras para las claves de usuario almacenadas en el equipo";
            string clave = @"MACHINE\\Software\\Policies\\Microsoft\\Cryptography\\ForceKeyProtection";
            string recomendacion = @"Se pregunta al usuario cuando se utiliza la llave por primera vez. Configurar esta opción como El usuario debe introducir una contraseña cada vez que utiliza una llave también se ajusta a la norma.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor != "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.14.1 SYSTEM CRYPTOGRAPHY: FORCE STRONG KEY PROTECTION FOR USER KEYS STORED ON THE COMPUTER 


        // 2.3.15.1 SYSTEM OBJECTS: REQUIRE CASE INSENSITIVITY FOR NON-WINDOWS SUBSYSTEMS -------------------------
        public void Analizar_System_objects__Require_case_insensitivity_for_non__Windows_subsystems()
        {
            string politica = "Objetos de sistema: requerir no distinguir mayúsculas de minúsculas para subsistemas que no sean de Windows";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Session Manager\\Kernel\\ObCaseInsensitive";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.15.1 SYSTEM OBJECTS: REQUIRE CASE INSENSITIVITY FOR NON-WINDOWS SUBSYSTEMS 


        // 2.3.15.2 SYSTEM OBJECTS: STRENGTHEN DEFAULT PERMISSIONS OF INTERNAL SYSTEM OBJECTS (E.G. SYMBOLIC LINKS) -------------------------
        public void Analizar_System_objects__Strengthen_default_permissions_of_internal_system_objects__e__g___Symbolic_Links__()
        {
            string politica = "Objetos de sistema: reforzar los permisos predeterminados de los objetos internos del sistema (por ejemplo, vínculos simbólicos)";
            string clave = @"MACHINE\\System\\CurrentControlSet\\Control\\Session Manager\\ProtectionMode";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.15.2 SYSTEM OBJECTS: STRENGTHEN DEFAULT PERMISSIONS OF INTERNAL SYSTEM OBJECTS (E.G. SYMBOLIC LINKS) 


        // 2.3.17.1 USER ACCOUNT CONTROL: ADMIN APPROVAL MODE FOR THE BUILT-IN ADMINISTRATOR ACCOUNT -------------------------
        public void Analizar_User_Account_Control__Admin_Approval_Mode_for_the_Built__in_Administrator_account()
        {
            string politica = "Control de cuentas de usuario: usar Modo de aprobación de administrador para la cuenta predefinida Administrador";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\FilterAdministratorToken";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.1 USER ACCOUNT CONTROL: ADMIN APPROVAL MODE FOR THE BUILT-IN ADMINISTRATOR ACCOUNT 


        // 2.3.17.2 USER ACCOUNT CONTROL: BEHAVIOR OF THE ELEVATION PROMPT FOR ADMINISTRATORS IN ADMIN APPROVAL MODE -------------------------
        public void Analizar_User_Account_Control__Behavior_of_the_elevation_prompt_for_administrators_in_Admin_Approval_Mode()
        {
            string politica = "Control de cuentas de usuario: comportamiento de la petición de elevación para los administradores en Modo de aprobación de administrador\r\n";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\ConsentPromptBehaviorAdmin";
            string recomendacion = @"Solicitar consentimiento en el escritorio seguro. Configurar este parámetro como Solicitar credenciales en el escritorio seguro también se ajusta a la referencia. ";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1" || valor == "4,2")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.2 USER ACCOUNT CONTROL: BEHAVIOR OF THE ELEVATION PROMPT FOR ADMINISTRATORS IN ADMIN APPROVAL MODE 


        // 2.3.17.3 USER ACCOUNT CONTROL: BEHAVIOR OF THE ELEVATION PROMPT FOR STANDARD USERS -------------------------
        public void Analizar_User_Account_Control__Behavior_of_the_elevation_prompt_for_standard_users()
        {
            string politica = "Control de cuentas de usuario: comportamiento de la petición de elevación para los usuarios estándar";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\ConsentPromptBehaviorUser";
            string recomendacion = @" Denegar automáticamente las solicitudes de elevación.";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.3 USER ACCOUNT CONTROL: BEHAVIOR OF THE ELEVATION PROMPT FOR STANDARD USERS 


        // 2.3.17.4 USER ACCOUNT CONTROL: DETECT APPLICATION INSTALLATIONS AND PROMPT FOR ELEVATION -------------------------
        public void Analizar_User_Account_Control__Detect_application_installations_and_prompt_for_elevation()
        {
            string politica = "Control de cuentas de usuario: detectar instalaciones de aplicaciones y pedir confirmación de elevación";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\EnableInstallerDetection";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.4 USER ACCOUNT CONTROL: DETECT APPLICATION INSTALLATIONS AND PROMPT FOR ELEVATION 


        // 2.3.17.5 USER ACCOUNT CONTROL: ONLY ELEVATE UIACCESS APPLICATIONS THAT ARE INSTALLED IN SECURE LOCATIONS -------------------------
        public void Analizar_User_Account_Control__Only_elevate_UIAccess_applications_that_are_installed_in_secure_locations()
        {
            string politica = "Control de cuentas de usuario: elevar solo aplicaciones UIAccess instaladas en ubicaciones seguras";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\EnableSecureUIAPaths";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.5 USER ACCOUNT CONTROL: ONLY ELEVATE UIACCESS APPLICATIONS THAT ARE INSTALLED IN SECURE LOCATIONS 


        // 2.3.17.6 USER ACCOUNT CONTROL: RUN ALL ADMINISTRATORS IN ADMIN APPROVAL MODE -------------------------
        public void Analizar_User_Account_Control__Run_all_administrators_in_Admin_Approval_Mode()
        {
            string politica = "Control de cuentas de usuario: activar el Modo de aprobación de administrador.\r\n";
            string clave = "MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\EnableLUA";
            string recomendacion = "Deshabilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.6 USER ACCOUNT CONTROL: RUN ALL ADMINISTRATORS IN ADMIN APPROVAL MODE 


        // 2.3.17.7 USER ACCOUNT CONTROL: SWITCH TO THE SECURE DESKTOP WHEN PROMPTING FOR ELEVATION -------------------------
        public void Analizar_User_Account_Control__Switch_to_the_secure_desktop_when_prompting_for_elevation()
        {
            string politica = "Control de cuentas de usuario: cambiar al escritorio seguro cuando se pida confirmación de elevación";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\PromptOnSecureDesktop";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.7 USER ACCOUNT CONTROL: SWITCH TO THE SECURE DESKTOP WHEN PROMPTING FOR ELEVATION 


        // 2.3.17.8 USER ACCOUNT CONTROL: VIRTUALIZE FILE AND REGISTRY WRITE FAILURES TO PER-USER LOCATIONS -------------------------
        public void Analizar_User_Account_Control__Virtualize_file_and_registry_write_failures_to_per__user_locations()
        {
            string politica = "Control de cuentas de usuario: virtualizar los errores de escritura de archivo y del Registro a ubicaciones por usuario";
            string clave = @"MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\EnableVirtualization";
            string recomendacion = @"Habilitado";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 2.3.17.8 USER ACCOUNT CONTROL: VIRTUALIZE FILE AND REGISTRY WRITE FAILURES TO PER-USER LOCATIONS 


        // 5.1 BLUETOOTH AUDIO GATEWAY SERVICE -------------------------
        public void Analizar_Bluetooth_Audio_Gateway_Service__BTAGService__()
        {
            string politica = "Servicio de puerta de enlace de audio de Bluetooth";
            string clave = @"SYSTEM\CurrentControlSet\Services\BTAGService:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 5.1 BLUETOOTH AUDIO GATEWAY SERVICE (BTAGSERVICE) 


        // 5.2 BLUETOOTH SUPPORT SERVICE (BTHSERV) -------------------------
        public void Analizar_Bluetooth_Support_Service__bthserv__()
        {
            string politica = "Servicio de compatibilidad con Bluetooth";
            string clave = @"SYSTEM\CurrentControlSet\Services\bthserv:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 5.2 BLUETOOTH SUPPORT SERVICE (BTHSERV) 


        // 5.3 COMPUTER BROWSER (BROWSER) -------------------------  *
        public void Analizar_Computer_Browser__Browser__()
        {
            string politica = "Computer Browser";
            string clave = @"SYSTEM\CurrentControlSet\Services\Browser:Start";
            string recomendacion = @"Desactivado o No instalado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 5.3 COMPUTER BROWSER (BROWSER) 


        // 5.4 DOWNLOADED MAPS MANAGER (MAPSBROKER) -------------------------
        public void Analizar_Downloaded_Maps_Manager__MapsBroker__()
        {
            string politica = "Administrador de mapas descargados";
            string clave = @"SYSTEM\CurrentControlSet\Services\MapsBroker:Start";
            string recomendacion = @"Comentario_Recomendacion_Aqui";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 5.4 DOWNLOADED MAPS MANAGER (MAPSBROKER) 


        // 5.5 GEOLOCATION SERVICE (LFSVC) -------------------------
        public void Analizar_Geolocation_Service__lfsvc__()
        {
            string politica = "Servicio de geolocalización";
            string clave = @"SYSTEM\CurrentControlSet\Services\lfsvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 5.5 GEOLOCATION SERVICE (LFSVC) 


        // 5.6 IIS ADMIN SERVICE (IISADMIN) -------------------------  *
        public void Analizar_IIS_Admin_Service__IISADMIN__()
        {
            string politica = "Servicio de Administración de IIS";
            string clave = @"SYSTEM\CurrentControlSet\Services\IISADMIN:Start";
            string recomendacion = @"Desactivado o No instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No definido", recomendacion);
            }
        }
        // ------------------------- 5.6 IIS ADMIN SERVICE (IISADMIN) 


        // 5.7 INFRARED MONITOR SERVICE (IRMON) -------------------------
        public void Analizar_Infrared_monitor_service__irmon__()
        {
            string politica = "Servicio de Monitor de Infrarrojos";
            string clave = @"SYSTEM\CurrentControlSet\Services\irmon:Start";
            string recomendacion = @"Deshabilitado o No instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.7 INFRARED MONITOR SERVICE (IRMON) 


        // 5.8 INTERNET CONNECTION SHARING (ICS) (SHAREDACCESS) -------------------------
        public void Analizar_Internet_Connection_Sharing__ICS__SharedAccess__()
        {
            string politica = "Conexión compartida a Internet";
            string clave = @"SYSTEM\CurrentControlSet\Services\SharedAccess:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.8 INTERNET CONNECTION SHARING (ICS) (SHAREDACCESS) 


        // 5.9 LINK-LAYER TOPOLOGY DISCOVERY MAPPER (LLTDSVC) -------------------------
        public void Analizar_Link_Layer_Topology_Discovery_Mapper__lltdsvc__()
        {
            string politica = "Asignador de detección de topologías de nivel de vínculo";
            string clave = @"SYSTEM\CurrentControlSet\Services\lltdsvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.9 LINK-LAYER TOPOLOGY DISCOVERY MAPPER (LLTDSVC) 


        // 5.10 LXSSMANAGER (LXSSMANAGER) -------------------------
        public void Analizar_LxssManager__LxssManager__()
        {
            string politica = "LXSSMANAGER";
            string clave = @"SYSTEM\CurrentControlSet\Services\LxssManager:Start";
            string recomendacion = @"Desactivado o No instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.10 LXSSMANAGER (LXSSMANAGER) 


        // 5.11 MICROSOFT FTP SERVICE (FTPSVC) -------------------------
        public void Analizar_Microsoft_FTP_Service__FTPSVC__()
        {
            string politica = "Servicio FTP de Microsof";
            string clave = @"SYSTEM\CurrentControlSet\Services\FTPSVC:Start";
            string recomendacion = @"Desactivado o No instalado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.11 MICROSOFT FTP SERVICE (FTPSVC) 


        // 5.12 MICROSOFT ISCSI INITIATOR SERVICE (MSISCSI) ------------------------- soidfghvisdjfnvijsdfnbvoifdnboifnboifdsgnbogfdinbodifognbfdigjnb
        public void Analizar_Microsoft_iSCSI_Initiator_Service__MSiSCSI__()
        {
            string politica = "Servicio del iniciador iSCSI de Microsoft";
            string clave = @"SYSTEM\CurrentControlSet\Services\MSiSCSI:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.12 MICROSOFT ISCSI INITIATOR SERVICE (MSISCSI) 


        // 5.13 OPENSSH SSH SERVER (SSHD) -------------------------
        public void Analizar_OpenSSH_SSH_Server__sshd__()
        {
            string politica = "OpenSSH Authentication Agent";
            string clave = @"SYSTEM\CurrentControlSet\Services\sshd:Start";
            string recomendacion = @"Desactivado o No instalado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.13 OPENSSH SSH SERVER (SSHD) 


        // 5.14 PEER NAME RESOLUTION PROTOCOL (PNRPSVC) -------------------------
        public void Analizar_Peer_Name_Resolution_Protocol__PNRPsvc__()
        {
            string politica = "Protocolo de resolución de nombres de mismo nivel";
            string clave = @"SYSTEM\CurrentControlSet\Services\PNRPsvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.14 PEER NAME RESOLUTION PROTOCOL (PNRPSVC) 


        // 5.15 PEER NETWORKING GROUPING (P2PSVC) -------------------------
        public void Analizar_Peer_Networking_Grouping__p2psvc__()
        {
            string politica = "Agrupación de red del mismo nivel";
            string clave = @"SYSTEM\CurrentControlSet\Services\p2psvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.15 PEER NETWORKING GROUPING (P2PSVC) 


        // 5.16 PEER NETWORKING IDENTITY MANAGER (P2PIMSVC) -------------------------
        public void Analizar_Peer_Networking_Identity_Manager__p2pimsvc__()
        {
            string politica = "Identity Manager de redes de mismo nivel";
            string clave = @"SYSTEM\CurrentControlSet\Services\p2pimsvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.16 PEER NETWORKING IDENTITY MANAGER (P2PIMSVC) 


        // 5.17 PNRP MACHINE NAME PUBLICATION SERVICE (PNRPAUTOREG) -------------------------
        public void Analizar_PNRP_Machine_Name_Publication_Service__PNRPAutoReg__()
        {
            string politica = "Servicio de publicación de nombres de equipo PNRP";
            string clave = @"SYSTEM\CurrentControlSet\Services\PNRPAutoReg:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.17 PNRP MACHINE NAME PUBLICATION SERVICE (PNRPAUTOREG) 


        // 5.18 PRINT SPOOLER (SPOOLER) -------------------------
        public void Analizar_Print_Spooler__Spooler__()
        {
            string politica = "Cola de impresión";
            string clave = @"SYSTEM\CurrentControlSet\Services\Spooler:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.18 PRINT SPOOLER (SPOOLER) 


        // 5.19 PROBLEM REPORTS AND SOLUTIONS CONTROL PANEL SUPPORT (WERCPLSUPPORT) -------------------------
        public void Analizar_Problem_Reports_and_Solutions_Control_Panel_Support__wercplsupport__()
        {
            string politica = "Soporte técnico del panel de control Informes de problemas";
            string clave = @"SYSTEM\CurrentControlSet\Services\wercplsupport:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.19 PROBLEM REPORTS AND SOLUTIONS CONTROL PANEL SUPPORT (WERCPLSUPPORT) 


        // 5.20 REMOTE ACCESS AUTO CONNECTION MANAGER (RASAUTO) -------------------------
        public void Analizar_Remote_Access_Auto_Connection_Manager__RasAuto__()
        {
            string politica = "Administrador de conexiones automáticas de acceso remoto";
            string clave = @"SYSTEM\CurrentControlSet\Services\RasAuto:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.20 REMOTE ACCESS AUTO CONNECTION MANAGER (RASAUTO) 


        // 5.21 REMOTE DESKTOP CONFIGURATION (SESSIONENV) -------------------------
        public void Analizar_Remote_Desktop_Configuration__SessionEnv__()
        {
            string politica = "Configuración de Escritorio remoto";
            string clave = @"SYSTEM\CurrentControlSet\Services\SessionEnv:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.21 REMOTE DESKTOP CONFIGURATION (SESSIONENV) 


        // 5.22 REMOTE DESKTOP SERVICES (TERMSERVICE) -------------------------
        public void Analizar_Remote_Desktop_Services__TermService__()
        {
            string politica = "Servicios de Escritorio remoto";
            string clave = @"SYSTEM\CurrentControlSet\Services\TermService:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.22 REMOTE DESKTOP SERVICES (TERMSERVICE) 


        // 5.23 REMOTE DESKTOP SERVICES USERMODE PORT REDIRECTOR (UMRDPSERVICE) -------------------------
        public void Analizar_Remote_Desktop_Services_UserMode_Port_Redirector__UmRdpService__()
        {
            string politica = "Redirector de puerto en modo usuario de Servicios de Escritorio remoto";
            string clave = @"SYSTEM\CurrentControlSet\Services\UmRdpService:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.23 REMOTE DESKTOP SERVICES USERMODE PORT REDIRECTOR (UMRDPSERVICE) 


        // 5.24 REMOTE PROCEDURE CALL (RPC) LOCATOR (RPCLOCATOR) -------------------------
        public void Analizar_Remote_Procedure_Call__RPC__Locator__RpcLocator__()
        {
            string politica = "Ubicador de llamada a procedimiento remoto (RPC)";
            string clave = @"SYSTEM\CurrentControlSet\Services\RpcLocator:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.24 REMOTE PROCEDURE CALL (RPC) LOCATOR (RPCLOCATOR) 


        // 5.25 REMOTE REGISTRY (REMOTEREGISTRY) -------------------------
        public void Analizar_Remote_Registry__RemoteRegistry__()
        {
            string politica = "Registro remoto";
            string clave = @"SYSTEM\CurrentControlSet\Services\RemoteRegistry:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.25 REMOTE REGISTRY (REMOTEREGISTRY) 


        // 5.26 ROUTING AND REMOTE ACCESS (REMOTEACCESS) -------------------------
        public void Analizar_Routing_and_Remote_Access__RemoteAccess__()
        {
            string politica = "Enrutamiento y acceso remoto";
            string clave = @"SYSTEM\CurrentControlSet\Services\RemoteAccess:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.26 ROUTING AND REMOTE ACCESS (REMOTEACCESS) 


        // 5.27 SERVER (LANMANSERVER) -------------------------
        public void Analizar_Server__LanmanServer__()
        {
            string politica = "Servidor";
            string clave = @"SYSTEM\CurrentControlSet\Services\LanmanServer:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.27 SERVER (LANMANSERVER) 


        // 5.28 SIMPLE TCP/IP SERVICES (SIMP_TCP) -------------------------
        public void Analizar_Simple_TCP_IP_Services__simptcp__()
        {
            string politica = "Simple TCP/IP Services";
            string clave = @"SYSTEM\CurrentControlSet\Services\simptcp:Start";
            string recomendacion = @"Desactivado o no instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.28 SIMPLE TCP/IP SERVICES (SIMP_TCP) 


        // 5.29 SNMP SERVICE (SNMP) -------------------------
        public void Analizar_SNMP_Service__SNMP__()
        {
            string politica = "Captura de SNMP";
            string clave = @"SYSTEM\CurrentControlSet\Services\SNMP:Start";
            string recomendacion = @"Desactivado o No instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.29 SNMP SERVICE (SNMP) 


        // 5.30 SNMP SERVICE (SNMP) SPECIAL ADMINISTRATION CONSOLE HELPER (SACSVR) -------------------------
        public void Analizar_SNMP_Service__SNMP__Special_Administration_Console_Helper__sacsvr__()
        {
            string politica = "Asistente de consola de administración especial";
            string clave = @"SYSTEM\CurrentControlSet\Services\sacsvr:Start";
            string recomendacion = @"Desactivado o No instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.30 SNMP SERVICE (SNMP) SPECIAL ADMINISTRATION CONSOLE HELPER (SACSVR) 


        // 5.31 SSDP DISCOVERY (SSDPSRV) -------------------------
        public void Analizar_SSDP_Discovery__SSDPSRV__()
        {
            string politica = "Detección SSDP";
            string clave = @"SYSTEM\CurrentControlSet\Services\SSDPSRV:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.31 SSDP DISCOVERY (SSDPSRV) 


        // 5.32 UPNP DEVICE HOST (UPNPHOST) -------------------------
        public void Analizar_UPnP_Device_Host__upnphost__()
        {
            string politica = "Dispositivo host de UPnP";
            string clave = @"SYSTEM\CurrentControlSet\Services\upnphost:Start";
            string recomendacion = @"Desahibilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.32 UPNP DEVICE HOST (UPNPHOST) 


        // 5.33 WEB MANAGEMENT SERVICE (WMSVC) -------------------------
        public void Analizar_Web_Management_Service__WMSvc__()
        {
            string politica = "Servicio de Administración Web";
            string clave = @"SYSTEM\CurrentControlSet\Services\WMSvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.33 WEB MANAGEMENT SERVICE (WMSVC) 


        // 5.34 WINDOWS ERROR REPORTING SERVICE (WERSVC) -------------------------
        public void Analizar_Windows_Error_Reporting_Service__WerSvc__()
        {
            string politica = "Servicio de informes de errores de Windows";
            string clave = @"SYSTEM\CurrentControlSet\Services\WerSvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.34 WINDOWS ERROR REPORTING SERVICE (WERSVC) 


        // 5.35 WINDOWS EVENT COLLECTOR (WECSVC) -------------------------
        public void Analizar_Windows_Event_Collector__Wecsvc__()
        {
            string politica = "Recopilador de eventos de Windows";
            string clave = @"SYSTEM\CurrentControlSet\Services\Wecsvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.35 WINDOWS EVENT COLLECTOR (WECSVC) 


        // 5.36 WINDOWS MEDIA PLAYER NETWORK SHARING SERVICE (WMPNETWORKSVC) -------------------------
        public void Analizar_Windows_Media_Player_Network_Sharing_Service__WMPNetworkSvc__()
        {
            string politica = "Servicio de uso compartido de red del Reproductor de Windows Media";
            string clave = @"SYSTEM\CurrentControlSet\Services\WMPNetworkSvc:Start";
            string recomendacion = @"Desactivado o No instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.36 WINDOWS MEDIA PLAYER NETWORK SHARING SERVICE (WMPNETWORKSVC) 


        // 5.37 WINDOWS MOBILE HOTSPOT SERVICE (ICSSVC) -------------------------
        public void Analizar_Windows_Mobile_Hotspot_Service__icssvc__()
        {
            string politica = "Servicio de zona con cobertura inalámbrica móvil de Windows";
            string clave = @"SYSTEM\CurrentControlSet\Services\icssvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.37 WINDOWS MOBILE HOTSPOT SERVICE (ICSSVC) 


        // 5.38 WINDOWS PUSH NOTIFICATIONS SYSTEM SERVICE (WPNSERVICE) -------------------------
        public void Analizar_Windows_Push_Notifications_System_Service__WpnService__()
        {
            string politica = "Servicio del sistema de notificaciones de inserción de Windows";
            string clave = @"SYSTEM\CurrentControlSet\Services\WpnService:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.38 WINDOWS PUSH NOTIFICATIONS SYSTEM SERVICE (WPNSERVICE) 


        // 5.39 WINDOWS PUSHTOINSTALL SERVICE (PUSHTOINSTALL) -------------------------
        public void Analizar_Windows_PushToInstall_Service__PushToInstall__()
        {
            string politica = "Servicio de Windows PushToInstall";
            string clave = @"SYSTEM\CurrentControlSet\Services\PushToInstall:Start";
            string recomendacion = @"Desahibilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.39 WINDOWS PUSHTOINSTALL SERVICE (PUSHTOINSTALL) 


        // 5.40 WINDOWS REMOTE MANAGEMENT (WSMANAGEMENT) (WINRM) -------------------------
        public void Analizar_Windows_Remote_Management__WSManagement__WinRM__()
        {
            string politica = "Administración remota de Windows (WS-Management)";
            string clave = @"SYSTEM\CurrentControlSet\Services\WinRM:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.40 WINDOWS REMOTE MANAGEMENT (WSMANAGEMENT) (WINRM)


        // 5.41 WORLD WIDE WEB PUBLISHING SERVICE (W3SVC) -------------------------
        public void Analizar_World_Wide_Web_Publishing_Service__W3SVC__()
        {
            string politica = "Servicio de publicación World Wide Web";
            string clave = @"SYSTEM\CurrentControlSet\Services\W3SVC:Start";
            string recomendacion = @"Descativado o No instalado.";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.41 WORLD WIDE WEB PUBLISHING SERVICE (W3SVC)


        // 5.42 XBOX ACCESSORY MANAGEMENT SERVICE (XBOXGIPSVC) -------------------------
        public void Analizar_Xbox_Accessory_Management_Service__XboxGipSvc__()
        {
            string politica = "Xbox Accessory Management Service";
            string clave = @"SYSTEM\CurrentControlSet\Services\XboxGipSvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.42 XBOX ACCESSORY MANAGEMENT SERVICE (XBOXGIPSVC)


        // 5.43 XBOX LIVE AUTH MANAGER (XBLAUTHMANAGER) -------------------------
        public void Analizar_Xbox_Live_Auth_Manager__XblAuthManager__()
        {
            string politica = "Administración de autenticación de Xbox Live";
            string clave = @"SYSTEM\CurrentControlSet\Services\XblAuthManager:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.43 XBOX LIVE AUTH MANAGER (XBLAUTHMANAGER)


        // 5.44 XBOX LIVE GAME SAVE (XBLGAMESAVE) -------------------------
        public void Analizar_Xbox_Live_Game_Save__XblGameSave__()
        {
            string politica = "Partida guardada en Xbox Live";
            string clave = @"SYSTEM\CurrentControlSet\Services\XblGameSave:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "4")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.44 XBOX LIVE GAME SAVE (XBLGAMESAVE)


        // 5.45 XBOX LIVE NETWORKING SERVICE (XBOXNETAPISVC) -------------------------
        public void Analizar_Xbox_Live_Networking_Service__XboxNetApiSvc__()
        {
            string politica = "Servicio de red de Xbox Live";
            string clave = @"SYSTEM\CurrentControlSet\Services\XboxNetApiSvc:Start";
            string recomendacion = @"Deshabilitado";

            if (RegistroWindows.ExisteClaveRegistro(clave))
            {
                string valor = RegistroWindows.ObtenerValorRegistro(clave);
                if (valor == "0")
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(true);
                }
                else
                {
                    EstConfig(politica, clave, valor, recomendacion);
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                condicionMetodos.Add(false);
                EstConfig(politica, clave, "No está definido", recomendacion);
            }
        }
        // ------------------------- 5.45 XBOX LIVE NETWORKING SERVICE (XBOXNETAPISVC)
















































    }
}
