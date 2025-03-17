using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Programa iniciado...");
        Console.OutputEncoding = Encoding.UTF8;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string rutaOrigen = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine($"Ruta de origen: {rutaOrigen}");

        if (!Directory.Exists(rutaOrigen))
        {
            Console.WriteLine("Error: La ruta de archivos no existe.");
            Console.ReadKey(); // Mantener la ventana abierta en caso de error
            return;
        }

        string[] archivos = Directory.GetFiles(rutaOrigen, "*.jrn")
                                     .Where(f => Path.GetFileName(f).ToLower().Contains("control") && !Path.GetFileName(f).ToLower().Contains("error"))
                                     .ToArray();

        if (archivos.Length == 0)
        {
            Console.WriteLine("Error: No hay archivos para procesar.");
            Console.ReadKey(); // Mantener la ventana abierta en caso de error
            return;
        }

        bool huboError = false;

        foreach (string archivo in archivos)
        {
            try
            {
                string contenido = File.ReadAllText(archivo, Encoding.Latin1);
                string[] lineas = contenido.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                int lineasACopiar = Math.Min(10, lineas.Length);

                string nombreArchivo = Path.GetFileName(archivo);
                string nombreBase = nombreArchivo.Replace("CONTROL", "MUESTRA");

                // Quitar "FAC" del nombre si está presente
                nombreBase = Regex.Replace(nombreBase, "_FAC_", "_", RegexOptions.IgnoreCase);

                string rutaDestino = Path.Combine(rutaOrigen, nombreBase);
                int contador = 1;

                // Si el archivo ya existe, agregar un número correlativo
                while (File.Exists(rutaDestino))
                {
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(nombreBase);
                    string extension = Path.GetExtension(nombreBase);
                    rutaDestino = Path.Combine(rutaOrigen, $"{nombreSinExtension}_{contador}{extension}");
                    contador++;
                }
                
                using (StreamWriter sw = new StreamWriter(rutaDestino, false, Encoding.Latin1))
                {
                    for (int i = 0; i < lineasACopiar; i++)
                    {
                        sw.WriteLine(lineas[i]);
                    }

                    if (lineas.Length > 0 && !string.IsNullOrWhiteSpace(lineas[^1]))
                    {
                        sw.WriteLine(); // Agregar una línea vacía solo si la última línea no es vacía
                    }
                }

                Console.WriteLine($"Archivo generado correctamente: {rutaDestino}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando {archivo}: {ex.Message}");
                huboError = true;
            }
        }

        if (huboError)
        {
            Console.WriteLine("Proceso finalizado con errores. Presiona una tecla para salir.");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Proceso finalizado correctamente.");
            Thread.Sleep(3000);
        }
    }
}
