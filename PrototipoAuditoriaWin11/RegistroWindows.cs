using Microsoft.Win32;
using System;

namespace PrototipoAuditoriaWin11
{
    internal class RegistroWindows
    {
        private static string[] SepararRutaYClave(string rutaCompleta)
        {
            string[] partes = rutaCompleta.Split(':');
            if (partes.Length != 2)
            {
                throw new ArgumentException("La ruta del registro no es válida.");
            }

            return partes;
        }

        public static bool ExisteClaveRegistro(string rutaCompleta)
        {
            try
            {
                string[] partes = SepararRutaYClave(rutaCompleta);
                string rutaRegistro = partes[0];
                string clave = partes[1];

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(rutaRegistro))
                {
                    if (key != null && key.GetValue(clave) != null)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar la clave del registro: {ex.Message}");
                return false;
            }
        }

        public static string ObtenerValorRegistro(string rutaCompleta)
        {
            try
            {
                string[] partes = SepararRutaYClave(rutaCompleta);
                string rutaRegistro = partes[0];
                string clave = partes[1];

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(rutaRegistro))
                {
                    object valor = key.GetValue(clave);
                    return valor.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el valor del registro: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
