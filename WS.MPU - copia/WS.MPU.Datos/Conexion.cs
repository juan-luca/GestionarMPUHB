using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using IBM.Data.DB2;


namespace WS.MPU.GestionarMPU.Datos
{
    public class Conexion
    {
        public static DB2Connection ObtenerConnexionDB2()
        {
            DB2Connection cnn = new DB2Connection(ConfigurationManager.ConnectionStrings["LA_LUPADB2"].ConnectionString);
            return cnn;
        }
        public static string GetSheme()
        {
            return ConfigurationManager.AppSettings.Get("SchemaDB2");
        }
        public static DB2Command ObtenerDB2Command()
        {
            DB2Command cmd = new DB2Command();
            cmd.CommandType = System.Data.CommandType.Text;
            return cmd;
        }

        public static DB2DataReader EjecutarComando(DB2Command cmd)
        {
            //cargar el connexion string desencriptado            
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        public static DB2Command ObtenerCommandDB2(DB2Connection cnn, System.Data.CommandType commandType)
        {
            DB2Command cmd = new DB2Command();
            cmd.Connection = cnn;
            cmd.CommandType = commandType;
            return cmd;
        }
    }
}
