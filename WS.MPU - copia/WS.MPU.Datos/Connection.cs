using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using IBM.Data.DB2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;


namespace WS.MPU.GestionarMPU.Datos
{
    public class Connection : IDisposable
    {
        private DB2Connection dB2Connection;
        private string ConnectionString = ConfigurationManager.ConnectionStrings["MPUDB2"].ConnectionString;

        //private const string ConnectionString = "Database=SEGSOC_D;User ID=k000010;Password=24aMrDE5;Server=10.8.13.3:50000;CurrentSchema=A2000;Persist Security Info=True";
        //ConfigurationManager.AppSettings.Get("MPUDB2");
        public Connection()
        {
            DefaultSchema = GetScheme();
            dB2Connection = new DB2Connection(ConnectionString);
            dB2Connection.Open();
        }

        public static string GetScheme()
        {
            return ConfigurationManager.AppSettings.Get("SchemaDB2");
        }

        public static DB2Connection ObtenerConnexionDB2()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["MPUDB2"].ConnectionString;
            return new DB2Connection(ConnectionString);
        }

        public DB2DataReader ExecuteQuery(string query)
        {
            DB2Command cmd = new DB2Command(query);
            cmd.Connection = dB2Connection;
            cmd.CommandType = CommandType.Text;
            return cmd.ExecuteReader();
        }

        public DB2DataReader ExecuteSP(string spName)
        {
            return ExecuteSP(spName, new Dictionary<string, object>());
        }

        public int ExecuteNonQuery(string spName, Dictionary<string, object> parameters)
        {
            DB2Command cmd = new DB2Command();
            cmd.Connection = dB2Connection;
            cmd.CommandText = GetSPNameCompleto(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            foreach (var param in parameters)
            {
                cmd.Parameters.Add(new DB2Parameter(param.Key, param.Value));
            }

            return cmd.ExecuteNonQuery();
        }

        public DB2DataReader ExecuteSP(string spName, Dictionary<string, object> parameters)
        {
            DB2Command cmd = new DB2Command();
            cmd.Connection = dB2Connection;
            cmd.CommandText = GetSPNameCompleto(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;

            foreach (var param in parameters)
            {
                cmd.Parameters.Add(new DB2Parameter(param.Key, param.Value));
            }

            return cmd.ExecuteReader();
        }

        private string GetSPNameCompleto(string spName)
        {
            return $"{DefaultSchema}.{spName}";
        }

        public string DefaultSchema { get; set; }

        public void Dispose()
        {
            dB2Connection?.Close();
            dB2Connection?.Dispose();
        }
    }

}
