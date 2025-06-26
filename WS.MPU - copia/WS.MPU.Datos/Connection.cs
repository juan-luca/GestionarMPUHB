using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using IBM.Data.DB2;

namespace WS.MPUHB.Datos
{
    public class Connection : IDisposable
    {
        private readonly DB2Connection conn;
        public Connection()
        {
            var cs = ConfigurationManager.ConnectionStrings["MPUHBDB2"].ConnectionString;
            conn = new DB2Connection(cs);
            conn.Open();
        }

        public int ExecuteNonQuery(string spName, Dictionary<string, object> parameters)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                foreach (var p in parameters)
                    cmd.Parameters.Add(new DB2Parameter(p.Key, p.Value ?? DBNull.Value));
                return cmd.ExecuteNonQuery();
            }
        }

        public DB2DataReader ExecuteSP(string spName, Dictionary<string, object> parameters)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            foreach (var p in parameters)
                cmd.Parameters.Add(new DB2Parameter(p.Key, p.Value ?? DBNull.Value));
            return cmd.ExecuteReader();
        }

        public void Dispose()
        {
            conn?.Close();
            conn?.Dispose();
        }
    }
}
