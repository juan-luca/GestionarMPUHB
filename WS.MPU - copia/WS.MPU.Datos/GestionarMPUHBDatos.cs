using System;
using System.Collections.Generic;
using IBM.Data.DB2;
using WS.MPUHB.Datos.Models;
using log4net;

namespace WS.MPUHB.Datos
{
    public class GestionarMPUHBDatos
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GestionarMPUHBDatos));

        public Resultado InsertarMPUdesdeHB(decimal cuil, decimal cuit, short banco, short agencia, string ipOrigen)
        {
            try
            {
                using (var cn = new Connection())
                using (var reader = cn.ExecuteSP("PFSP_MPUHB_INSERT", new Dictionary<string, object>
                {
                    {"P_CUIL", cuil},
                    {"P_CUIT", cuit},
                    {"P_C_BANCO", banco},
                    {"P_C_AGENCIA", agencia},
                    {"P_IP_ORIGEN", ipOrigen}
                }))
                {
                    if (reader.Read())
                    {
                        return new Resultado
                        {
                            Codigo = Convert.ToInt16(reader["C_ERROR"]),
                            Mensaje = reader["D_ERROR"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error al insertar MPU desde HB", ex);
                return new Resultado { Codigo = -1, Mensaje = ex.Message };
            }
            return new Resultado { Codigo = -1, Mensaje = "No se recibió resultado del SP" };
        }
    }
}
