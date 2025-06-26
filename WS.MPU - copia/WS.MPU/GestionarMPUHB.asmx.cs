using System;
using System.Web.Services;
using log4net;
using WS.MPUHB.Negocio;
using WS.MPUHB.Datos.Models;

namespace WS.MPUHB
{
    [WebService(Namespace = "http://anses.gob.ar/MPUHB")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GestionarMPUHB : WebService
    {
        private readonly GestionarMPUHBNeg negocio = new GestionarMPUHBNeg();
        private static readonly ILog log = LogManager.GetLogger(typeof(GestionarMPUHB).Name);

        [WebMethod]
        public Resultado InsertarMPUdesdeHB(decimal cuil, decimal cuit, short cBanco, short cAgencia)
        {
            try
            {
                // 1) Extraer el CUIT real del token de seguridad (aquí un placeholder).
                //    Reemplazar con tu lógica concreta para leer el JWT o SOAP header.
                decimal cuitToken = ObtenerCuitDesdeToken();

                // 2) Comparar
                if (cuitToken != cuit)
                {
                    // 3) Devolver error de seguridad
                    log.Warn($"Intento con CUIT inválido. Token={cuitToken}, Parámetro={cuit}");
                    return new Resultado
                    {
                        Codigo = 99,
                        Mensaje = "CUIT erróneo"
                    };
                }

                // 4) Si coincide, seguir normalmente
                var ip = Context.Request.UserHostAddress;
                return negocio.InsertarMPUdesdeHB(cuil, cuit, cBanco, cAgencia, ip);
            }
            catch (Exception ex)
            {
                log.Error("Error en WS InsertarMPUdesdeHB", ex);
                return new Resultado { Codigo = -1, Mensaje = ex.Message };
            }
        }

        private decimal ObtenerCuitDesdeToken()
        {
            // Aquí deberías:
            // - Leer el header de seguridad
            // - Extraer el JWT o el XML con el CUIT
            // - Validar firma, caducidad, etc.
            // - Devolver el CUIT contenido en el token
            //
            // Por ahora devolvemos un dummy para que compile:
            return 0m;
        }
    }
}
