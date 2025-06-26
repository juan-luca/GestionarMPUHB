using System;
using System.IO;
using System.Diagnostics;
using log4net;
using log4net.Config;

namespace WS.MPUHB
{
    public class AdministradorDeLog
    {
        public static ILog ObtenerLogger(string nombreLogger)
        {
            var ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.Config.xml");
            var archivo = new FileInfo(ruta);
            XmlConfigurator.Configure(archivo);
            return LogManager.GetLogger(nombreLogger);
        }

        public static void LogTexto(string mensaje, TraceEventType tipo)
        {
            var log = ObtenerLogger(typeof(AdministradorDeLog).Namespace);
            switch (tipo)
            {
                case TraceEventType.Error:       log.Error(mensaje); break;
                case TraceEventType.Warning:     log.Warn(mensaje); break;
                case TraceEventType.Information: log.Info(mensaje); break;
                default:                         log.Debug(mensaje); break;
            }
        }
    }
}