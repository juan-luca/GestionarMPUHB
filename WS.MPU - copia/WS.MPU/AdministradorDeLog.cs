using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.IO;

namespace WS.MPU
{
    public class AdministradorDeLog
    {


        /// <summary>
        /// Carga el archivo de configuración de log4net ( log4net.Config.xml )y obtiene el logger.
        /// </summary>
        /// <param name="nombreLogger">nombre del logger a buscar</param>
        /// <returns>logger encontrado</returns>
        public static ILog ObtenerLogger(string nombreLogger)
        {
            string rutaArch;

            rutaArch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.Config.xml");

            FileInfo arch = new FileInfo(rutaArch);
            XmlConfigurator.Configure(arch);
            return LogManager.GetLogger(nombreLogger);
        }

        public static void LogTexto(string mensaje, TraceEventType tipo)
        {
            ILog log = AdministradorDeLog.ObtenerLogger(typeof(AdministradorDeLog).Namespace);
            switch (tipo)
            {
                case TraceEventType.Error:
                    log.Error(mensaje);
                    break;
                case TraceEventType.Information:
                    log.Info(mensaje);
                    break;
                case TraceEventType.Warning:
                    log.Warn(mensaje);
                    break;
                default:
                    log.Debug(mensaje);
                    break;
            }
        }

    }
}