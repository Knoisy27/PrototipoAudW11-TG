using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrototipoAuditoriaWin11
{
    internal static class Program
    {
        // GENERAR EL ARCHIVO SECURITY.CFG -------------------------
        private static void GenerarArchivoCFG()
        {
            //Console.WriteLine("El archivo no existe. Continúa con tu proceso.");
            string command = @"C:\Windows\System32\secedit";
            string arguments = @"/export /cfg C:\Windows\Temp\security.cfg";

            ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false; // Evita que se abra una nueva ventana de CMD
            startInfo.RedirectStandardOutput = true; // Captura la salida del comando

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                // Lee la salida del comando (opcional)
                string output = process.StandardOutput.ReadToEnd();
                //Console.WriteLine(output);

                process.WaitForExit();
            }
        }

        // ------------------------- GENERAR EL ARCHIVO SECURITY.CFG

        public static void VerificarArchivoCFG() 
        {
            try
            {
                string filePath = @"C:\Windows\Temp\security.cfg";

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    GenerarArchivoCFG();
                    Console.WriteLine("El archivo existe");
                }
                else
                {
                    GenerarArchivoCFG();
                    Console.WriteLine("El archivo existe");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al manejar el archivo: {ex.Message}");
            }
        }
       




        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]

        
        static void Main()
        {
            VerificarArchivoCFG();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormVentanaPrincipal());
        }
    }
}
