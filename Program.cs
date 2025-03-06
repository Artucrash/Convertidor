using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8; // Permitir caracteres especiales
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Agregar compatibilidad con más codificaciones

        Console.Clear();
        Console.WriteLine("*******************************");
        Console.WriteLine("   Bienvenido al Convertidor   ");
        Console.WriteLine("*******************************");
        Console.WriteLine("Procesando archivos...\n");

        string rutaOrigen = Directory.GetCurrentDirectory(); // Obtener ruta actual donde se ejecuta el .exe

        if (!Directory.Exists(rutaOrigen))
        {
            Console.WriteLine("Error: La ruta de archivos no existe.");
            Console.ReadKey();
            return;
        }

        string[] archivos = Directory.GetFiles(rutaOrigen, "*.jrn");

        if (archivos.Length == 0)
        {
            Console.WriteLine("Error: No hay archivos para procesar.");
            Console.ReadKey();
            return;
        }

        bool huboError = false;

        foreach (string archivo in archivos)
        {
            try
            {
                string contenido = File.ReadAllText(archivo, Encoding.Latin1); // Usar codificación Latin1 para asegurar caracteres especiales
                string[] lineas = contenido.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // Separar líneas por saltos de línea
                int lineasACopiar = Math.Min(10, lineas.Length);

                string nombreArchivo = Path.GetFileName(archivo);
                string nombreDestino = nombreArchivo.Replace("CONTROL", "MUESTRA");
                if (nombreDestino == nombreArchivo)
                {
                    nombreDestino = "MUESTRA_" + nombreArchivo;
                }

                string rutaDestino = Path.Combine(rutaOrigen, nombreDestino);

                using (StreamWriter sw = new StreamWriter(rutaDestino, false, Encoding.Latin1)) // Usar la misma codificación para escritura
                {
                    for (int i = 0; i < lineasACopiar; i++)
                    {
                        sw.WriteLine(lineas[i]);
                    }
                    sw.WriteLine(); // Línea vacía para separar el contenido
                }

                Console.WriteLine($"Archivo generado correctamente: {nombreDestino}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando {archivo}: {ex.Message}");
                huboError = true;
            }
        }

        if (huboError)
        {
            Console.WriteLine("\nProceso finalizado con errores.");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("\nProceso finalizado correctamente.");
            System.Threading.Thread.Sleep(3000); // Esperar 3 segundos antes de cerrar
        }
    }
}
