using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Win32;
using System.Reflection;
using System.Security.Principal;
using System.Net;
using System.Drawing.Printing;

namespace PrototipoAuditoriaWin11
{
    internal class Logica
    {
        private Panel panelDinamicoResultados;
        private DataGridView dgvRec;
        private Dictionary<string, string> configuraciones;
        public List<bool> condicionMetodos = new List<bool>();
        private List<Configuracion> configuracionesPendientes = new List<Configuracion>();

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
                        Console.WriteLine($"{clave} {valor}");

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





        private void EstConfig(string politica, string clave, string valor, string recomendacion) 
        {
            Configuracion configuracion = new Configuracion(politica, clave, valor, recomendacion);
            configuracionesPendientes.Add(configuracion);
        }





        // 1.1.1 ENFORCE PASSWORD HISTORY -------------------------
        public void Analizar_Enforce_Password_History()
        {
            string politica = "Exigir historial de contraseñas";
            string clave = "PasswordHistorySize";
            string recomendacion = "Comentario_Recomendacion_Aqui";
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey(clave))
            {
                int valor = Convert.ToInt32(configuraciones[clave]);
                if (valor >= 24)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }

                // Crear instancia de Configuracion y agregarla a la lista
                EstConfig(politica, clave, valor.ToString(), recomendacion);

            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.1 ENFORCE PASSWORD HISTORY





        // 1.1.2 MAXIMUM PASSWORD AGE -------------------------
        public void Analizar_Maximum_Password_Age()
        {
            string clave = "MaximumPasswordAge";
            string politica = "Exigir historial de contraseñas";
            string recomendacion = "Comentario_Recomendacion_Aqui";
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey(clave))
            {
                int valor = Convert.ToInt32(configuraciones[clave]);
                if (valor > 0 && valor <= 365)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }

                EstConfig(politica, clave, valor.ToString(), recomendacion);
            }
            else
            {
                Console.WriteLine("La clave MaximumPasswordAge no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.1 MAXIMUM PASSWORD AGE





        // 1.1.3 MINIMUM PASSWORD AGE -------------------------
        public void Analizar_MinimumPasswordAge()
        {
            string politica = "Longitud mínima de la contraseña";
            string clave = "MinimumPasswordAge";
            string recomendacion = "Comentario_Recomendacion_Aqui";
            //GuardarDatosCFG();
            if (configuraciones.ContainsKey("MinimumPasswordAge"))
            {
                int valor = Convert.ToInt32(configuraciones[clave]);
                if (valor > 0)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }

                EstConfig(politica, clave, valor.ToString(), recomendacion);

            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.3 MINIMUM PASSWORD AGE


        // 1.1.4 MINIMUM PASSWORD LENGTH -------------------------
        public void Analizar_MinimumPasswordLength()
        {
            string politica = "Tamaño mínimo de la contraseña";
            string clave = "MinimumPasswordLength";
            string recomendacion = "Comentario_Recomendacion_Aqui";
            if (configuraciones.ContainsKey("MinimumPasswordLength"))
            {
                int valor = Convert.ToInt32(configuraciones[clave]);
                if (valor > 13)
                {
                    condicionMetodos.Add(true);
                }
                else
                {
                    condicionMetodos.Add(false);
                }

                EstConfig(politica, clave, valor.ToString(), recomendacion);
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.4 MINIMUM PASSWORD LENGTH


        // 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS -------------------------
        public void Analizar_Password_must_meet_complexity_requirements()
        {
            string politica = "La contraseña debe cumplir con los requisitos de complegidad";
            string clave = "PasswordComplexity";
            string recomendacion = "Comentario_Recomendacion_Aqui";
            if (configuraciones.ContainsKey(clave))
            {
                int valor = Convert.ToInt32(configuraciones[clave]);
                if (valor == 1)
                {
                    condicionMetodos.Add(true);
                }
                else if (valor == 0)
                {
                    condicionMetodos.Add(false);
                }

                EstConfig(politica, clave, valor.ToString(), recomendacion);
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }
        }
        // ------------------------- 1.1.5 PASSWORD MUST MEET COMPLEXITY REQUIREMENTS


        // 1.1.6 MINUMUM PASSWORD LENGTH LIMITS -------------------------
        public void Analizar_Relax_minimum_password_length_limits()
        {
            // Definir los valores de la política y la recomendación
            string politica = "Reducir los límites de longitud mínima de la contraseña";
            string clave = "MACHINE\\System\\CurrentControlSet\\Control\\SAM\\RelaxMinimumPasswordLengthLimits";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                if (valor == "4,1")
                {
                    condicionMetodos.Add(true);
                }
                else if (valor == "4,0")
                {
                    condicionMetodos.Add(false);
                }

                EstConfig(politica, clave, valor, recomendacion);
            }
            else
            {
                Console.WriteLine("La clave PasswordHistorySize no está definida en el archivo de configuración.");
            }

        }
        // ------------------------- 1.1.6 MINUMUM PASSWORD LENGTH LIMITS -------------------------



        // 1.1.7 STORE PASSWORDS USING REVERSIBLE ENCRYPTION -------------------------
        public void Analizar_Clear_Text_Password()
        {
            
            // Definir la política y la recomendación
            string politica = "Almacenar contraseñas con cifrado reversible";
            string clave = "ClearTextPassword";
            int valor = Convert.ToInt32(configuraciones[clave]);
            string recomendacion = "Comentario_Recomendacion_Aqui";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey(clave))
            {
                // Verificar el valor de la configuración
                if (valor == 1)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                }
                else if (valor == 0)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                // Imprimir un mensaje de advertencia si la clave no está definida
                Console.WriteLine("La clave ClearTextPassword no está definida en el archivo de configuración.");
            }

            EstConfig(politica, clave, valor.ToString(), recomendacion);
        }
        // ------------------------- 1.1.7 STORE PASSWORDS USING REVERSIBLE ENCRYPTION




        // 1.2.1 ACCOUNT LOCKOUT DURATION -------------------------
        public void Analizar_Account_lockout_duration()
        {
            

            // Definir la política y la recomendación
            string politica = "Duración del bloqueo de la cuenta";
            string clave = "LockoutDuration";
            int valor = Convert.ToInt32(configuraciones[clave]);
            string recomendacion = "Comentario_Recomendacion_Aqui";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("LockoutDuration"))
            {
                // Verificar si el valor cumple con la condición
                if (valor >= 15)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                }
                else
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                // Imprimir un mensaje de advertencia si la clave no está definida
                Console.WriteLine("La clave LockoutDuration no está definida en el archivo de configuración.");
            }

            EstConfig(politica, clave, valor.ToString(), recomendacion);
        }
        // ------------------------- 1.2.1 ACCOUNT LOCKOUT DURATION


        // 1.2.2 ACCOUNT LOCKOUT THRESHOLD -------------------------
        public void Analizar_Account_lockout_threshold()
        {

            // Definir la política y la recomendación
            string politica = "Umbral de bloqueo de cuenta";
            string clave = "LockoutBadCount";
            int valor = Convert.ToInt32(configuraciones[clave]);
            string recomendacion = "Comentario_Recomendacion_Aqui";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("LockoutBadCount"))
            {
                // Verificar si el valor cumple con la condición
                if (valor <= 5 && valor != 0)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                }
                else
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                // Imprimir un mensaje de advertencia si la clave no está definida
                Console.WriteLine("La clave LockoutBadCount no está definida en el archivo de configuración.");
            }

            EstConfig(politica, clave, valor.ToString(), recomendacion);
        }
        // ------------------------- 1.2.2 ACCOUNT LOCKOUT THRESHOLD


        // 1.2.3 ALLOW ADMINISTRATOR ACCOUNT LOCKOUT -------------------------
        public void Analizar_Allow_Administrator_account_lockout()
        {

            // Definir la política y la recomendación
            string politica = "Permitir bloqueo de cuenta de administrador";
            string clave = "AllowAdministratorLockout";
            int valor = Convert.ToInt32(configuraciones[clave]);
            string recomendacion = "Comentario_Recomendacion_Aqui";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("AllowAdministratorLockout"))
            {
                // Verificar si el valor cumple con la condición
                if (valor == 1)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                }
                else if (valor == 0)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                // Imprimir un mensaje de advertencia si la clave no está definida
                Console.WriteLine("La clave AllowAdministratorLockout no está definida en el archivo de configuración.");
            }

            EstConfig(politica, clave, valor.ToString(), recomendacion);
        }
        // ------------------------- 1.2.3 ALLOW ADMINISTRATOR ACCOUNT LOCKOUT


        // 1.2.4 RESET ACCOUNT LOCKOUT COUNTER AFTER -------------------------
        public void Analizar_Reset_account_lockout_counter_after()
        {

            // Definir la política y la recomendación
            string politica = "Restablecer el bloqueo de la cuenta después de";
            string clave = "ResetLockoutCount";
            int valor = Convert.ToInt32(configuraciones[clave]);
            string recomendacion = "Comentario_Recomendacion_Aqui";

            // Verificar si la configuración está presente en el diccionario
            if (configuraciones.ContainsKey("ResetLockoutCount"))
            {
                // Verificar si el valor cumple con la condición
                if (valor >= 15)
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(true);
                }
                else
                {
                    // Establecer el valor y la condición
                    condicionMetodos.Add(false);
                }
            }
            else
            {
                // Imprimir un mensaje de advertencia si la clave no está definida
                Console.WriteLine("La clave ResetLockoutCount no está definida en el archivo de configuración.");
            }

            EstConfig(politica, clave, valor.ToString(), recomendacion);
        }
        // ------------------------- 1.2.4 RESET ACCOUNT LOCKOUT COUNTER AFTER


        // 2.2.1 ACCESS CREDENTIAL MANAGER AS A TRUSTED CALLER -------------------------
        public void Analizar_Access_Credential_Manager_as_a_trusted_caller()
        {
            string politica = "Acceso al Administrador de credenciales como un llamador de confianza";
            string clave = "SeTrustedCredManAccessPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.1 ACCESS CREDENTIAL MANAGER AS A TRUSTED CALLER

        // 2.2.2 ACCESS THIS COMPUTER FROM THE NETWORK -------------------------
        public void Analizar_Access_this_computer_from_the_network()
        {
            
            string politica = "Tener acceso a este equipo desde la red";
            string clave = "SeNetworkLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.2 ACCESS THIS COMPUTER FROM THE NETWORK

        // 2.2.3 ACT AS PART OF THE OPERATING SYSTEM -------------------------
        public void Analizar_Act_as_part_of_the_operating_system()
        {
            
            string politica = "Actuar como parte del sistema operativo";
            string clave = "SeTcbPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.3 ACT AS PART OF THE OPERATING SYSTEM

        // 2.2.4 ADJUST MEMORY QUOTAS FOR A PROCESS -------------------------
        public void Analizar_Adjust_memory_quotas_for_a_process()
        {
            string politica = "Ajustar las cuotas de la memoria para un proceso";
            string clave = "SeIncreaseQuotaPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.4 ADJUST MEMORY QUOTAS FOR A PROCESS

        // 2.2.5 ALLOW LOG ON LOCALLY -------------------------
        public void Analizar_Allow_log_on_locally()
        {
            string politica = "Permitir inicio de sesión local";
            string clave = "SeInteractiveLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.5 ALLOW LOG ON LOCALLY


        // 2.2.6 ALLOW LOG ON THROUGH REMOTE DESKTOP SERVICES -------------------------
        public void Analizar_Allow_log_on_through_Remote_Desktop_Services()
        {
            string politica = "Permitir inicio de sesión a través de Servicios de Escritorio remoto";
            string clave = "SeRemoteInteractiveLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.6 ALLOW LOG ON THROUGH REMOTE DESKTOP SERVICES


        // 2.2.7 BACK UP FILES AND DIRECTORIES -------------------------
        public void Analizar_Back_up_files_and_directories()
        {
            string politica = "Hacer copias de seguridad de archivos y directorios";
            string clave = "SeRestorePrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.7 BACK UP FILES AND DIRECTORIES


        // 2.2.8 CHANGE THE SYSTEM TIME -------------------------
        public void Analizar_Change_the_system_time()
        {
            string politica = "Cambiar la hora del sistema";
            string clave = "SeSystemtimePrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.8 CHANGE THE SYSTEM TIME


        // 2.2.9 CHANGE THE TIME ZONE -------------------------
        public void Analizar_Change_the_time_zone()
        {
            string politica = "Cambiar la zona horaria";
            string clave = "SeTimeZonePrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.9 CHANGE THE TIME ZONE


        // 2.2.10 CREATE A PAGEFILE -------------------------
        public void Analizar_Create_a_pagefile()
        {
            string politica = "Crear un archivo de paginación\r\n";
            string clave = "SeCreatePagefilePrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.10 CREATE A PAGEFILE

        // 2.2.11 CREATE A TOKEN OBJECT' IS SET TO 'NO ONE' -------------------------
        public void Analizar_Create_a_token_object_is_set_to_No_One()
        {
            string politica = "Crear un objeto token\r\n";
            string clave = "SeCreateTokenPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.11 CREATE A TOKEN OBJECT' IS SET TO 'NO ONE'


        // 2.2.12 CREATE GLOBAL OBJECTS -------------------------
        public void Analizar_Create_global_objects()
        {
            string politica = "Crear objetos globales";
            string clave = "SeCreateGlobalPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.12 CREATE GLOBAL OBJECTS


        // 2.2.13 CREATE PERMANENT SHARED OBJECTS -------------------------
        public void Analizar_Create_permanent_shared_objects()
        {
            string politica = "Crear objetos compartidos permanentes";
            string clave = "SeCreatePermanentPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.13 CREATE PERMANENT SHARED OBJECTS


        // 2.2.14 CREATE SYMBOLIC LINKS -------------------------
        public void Analizar_Create_symbolic_links()
        {
            string politica = "Crear vínculos simbólicos";
            string clave = "SeCreateSymbolicLinkPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.14 CREATE SYMBOLIC LINKS


        // 2.2.15 DEBUG PROGRAMS -------------------------
        public void Analizar_Debug_programs()
        {
            string politica = "Depurar programas";
            string clave = "SeDebugPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.15 DEBUG PROGRAMS


        // 2.2.16 DENY ACCESS TO THIS COMPUTER FROM NETWORK -------------------------
        public void Analizar_Deny_access_to_this_computer_from_the_network()
        {
            string politica = "Denegar el acceso desde la red a este equipo\r\n";
            string clave = "SeDenyNetworkLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.16 DENY ACCESS TO THIS COMPUTER FROM NETWORK


        // 2.2.17 DENY LOG ON AS A BATCH JOB -------------------------
        public void Analizar_Deny_log_on_as_a_batch_job()
        {
            string politica = "Denegar el inicio de sesión como trabajo por lotes";
            string clave = "SeDenyBatchLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.17 DENY LOG ON AS A BATCH JOB


        // 2.2.18 DENY LOG ON AS A SERVICE -------------------------
        public void Analizar_Deny_log_on_as_a_service()
        {
            string politica = "Denegar el inicio de sesión como servicio";
            string clave = "SeDenyServiceLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.18 DENY LOG ON AS A SERVICE


        // 2.2.19 DENY LOG ON LOCALLY -------------------------
        public void Analizar_Deny_log_on_locally()
        {
            string politica = "Denegar el inicio de sesión localmente";
            string clave = "SeDenyInteractiveLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.19 DENY LOG ON LOCALLY


        // 2.2.20 DENY LOG ON THROUGH REMOTE DESKTOP SERVICES -------------------------
        public void Analizar_Deny_log_on_through_Remote_Desktop_Services()
        {
            string politica = "Denegar inicio de sesión a través de Servicios de Escritorio remoto";
            string clave = "SeDenyRemoteInteractiveLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.20 DENY LOG ON THROUGH REMOTE DESKTOP SERVICES


        // 2.2.21 ENABLE COMPUTER AND USER ACCOUNTS TO BE TRUSTED FOR DELEGATION -------------------------
        public void Analizar_Enable_computer_and_user_accounts_to_betrusted_for_delegation()
        {
            string politica = "Habilitar confianza con el equipo y las cuentas de usuario para delegación";
            string clave = "SeEnableDelegationPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.21 ENABLE COMPUTER AND USER ACCOUNTS TO BE TRUSTED FOR DELEGATION


        // 2.2.22 FORCE SHUTDOWN FROM A REMOTE SYSTEM -------------------------
        public void Analizar_Force_shutdown_from_a_remote_system()
        {
            string politica = "Forzar cierre desde un sistema remoto";
            string clave = "SeRemoteShutdownPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.22 FORCE SHUTDOWN FROM A REMOTE SYSTEM


        // 2.2.23 GENERATE SECURITY AUDITS -------------------------
        public void Analizar_Generate_security_audits()
        {
            string politica = "Generar auditorías de seguridad";
            string clave = "SeAuditPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.23 GENERATE SECURITY AUDITS


        // 2.2.24 IMPERSONATE A CLIENT AFTER AUTHENTICATION -------------------------
        public void Analizar_Impersonate_a_client_after_authentication()
        {
            string politica = "Suplantar a un cliente tras la autenticación";
            string clave = "SeImpersonatePrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.24 IMPERSONATE A CLIENT AFTER AUTHENTICATION


        // 2.2.25 INCREASE SCHEDULING PRIORITY -------------------------
        public void Analizar_Increase_scheduling_priority()
        {
            string politica = "Aumentar prioridad de programación";
            string clave = "SeIncreaseBasePriorityPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.25 INCREASE SCHEDULING PRIORITY


        // 2.2.26 LOAD AND UNLOAD DEVICE DRIVERS -------------------------
        public void Analizar_Load_and_unload_device_drivers()
        {
            string politica = "Cargar y descargar controladores de dispositivo";
            string clave = "SeLoadDriverPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.26 LOAD AND UNLOAD DEVICE DRIVERS


        // 2.2.27 LOCK PAGES IN MEMORY -------------------------
        public void Analizar_Lock_pages_in_memory()
        {
            string politica = "Bloquear páginas en la memoria";
            string clave = "SeLockMemoryPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.27 LOCK PAGES IN MEMORY


        // 2.2.28 LOG ON AS A BATCH JOB -------------------------
        public void Analizar_Log_on_as_a_batch_job()
        {
            string politica = "Iniciar sesión como proceso por lotes";
            string clave = "SeBatchLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.28 LOG ON AS A BATCH JOB


        // 2.2.29 LOG ON AS A SERVICE -------------------------
        public void Analizar_Log_on_as_a_service()
        {
            string politica = "Iniciar sesión como servicio";
            string clave = "SeBatchLogonRight";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (!configuraciones.ContainsKey(clave))
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
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
            string recomendacion = "Comentario_Recomendacion_Aqui";

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
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(false);
            }
        }
        // ------------------------- 2.2.30 MANAGE AUDITING AND SECURITY LOG


        // 2.2.31 MODIFY AN OBJECT LABEL -------------------------
        public void Analizar_Modify_an_object_label()
        {
            string politica = "Modificar la etiqueta de un objeto";
            string clave = "SeRelabelPrivilege";
            string recomendacion = "Comentario_Recomendacion_Aqui";

            if (configuraciones.ContainsKey(clave))
            {
                string valor = configuraciones[clave];
                EstConfig(politica, clave, valor, recomendacion);
                condicionMetodos.Add(false);
            }
            else
            {
                EstConfig(politica, clave, "Nulo", recomendacion);
                condicionMetodos.Add(true);
            }
        }
        // ------------------------- 2.2.31 MODIFY AN OBJECT LABEL






















    }
}
