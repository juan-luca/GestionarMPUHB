using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2;
using WS.MPU.GestionarMPU.Datos.Models;
using System.Configuration;
using log4net;
namespace WS.MPU.GestionarMPU.Datos
{
    public class GestionarMPUDatos
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GestionMPU).Name);
        public int GuardarMedioPagoUnico(DTOMedioPago medioPago)
        {
            try
            {

                using (var connection = new Connection())
            {
                    
                    var parameters = new Dictionary<string, object>
                    {
                        {"P_CUIL", medioPago.Cuil},
                                {"P_CBU_INICIO", medioPago.CbuInicio ?? (object)DBNull.Value},
                                {"P_CBU_FINAL", medioPago.CbuFinal ?? (object)DBNull.Value},
                                {"P_C_BANCO", medioPago.CBanco ?? (object)DBNull.Value},
                                {"P_C_AGENCIA", medioPago.CAgencia ?? (object)DBNull.Value},
                                {"P_PE_INICIO_PAGO", medioPago.PeInicioPago},
                                {"P_CVU_1", medioPago.Cvu1 ?? (object)DBNull.Value},
                                {"P_CVU_2", medioPago.Cvu2 ?? (object)DBNull.Value},
                                {"P_ALIAS", medioPago.Alias ?? (object)DBNull.Value},
                                {"P_IP_ORIGEN", medioPago.IpOrigen ?? "0.0.0.0"},
                                {"P_UDAI", medioPago.Udai ?? (object)DBNull.Value},
                                {"P_OPE_TRAMITE", medioPago.OpeTramite ?? "00000000"}
                    };

                connection.ExecuteNonQuery("PFOL_MDPUNICO_INSERT", parameters);
                
                
                return 1;
            }
        }
            catch (System.Exception ex)
            {
                throw ex;
                return -1;
            }
        }
        public decimal ObtenerApoderado(DTOApoderado apod)
        {
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
                {
                    {"P_CUIL", decimal.Parse(apod.cuil)},
                    {"P_PE_PAGO", decimal.Parse(apod.p_pago)}
                };
                
                using (var reader = connection.ExecuteSP("PFSPAPODERADOCUNA_TRAER", parameters))
                {
                    if (reader.Read())
                    {

                        return Convert.ToDecimal(reader["CUIL_APODERADO"]);
                            
                        
                    }
                }
            }
            return -1;
        }

        public List<DTOBanco> ListarBilleterasVirtuales()
        {
            List<DTOBanco> billeteras = new List<DTOBanco>();
            using (var connection = new Connection())
            {
                using (var reader = connection.ExecuteSP("PFSPBILLETERASVIRTUALES_TRAER"))
                {
                    while (reader.Read())
                    {
                        billeteras.Add(new DTOBanco
                        {
                            Codigo = reader["C_BCO"].ToString(),
                            CodigoAgencia = reader["C_AGE"].ToString(),
                            Nombre = reader["D_BCO"].ToString(),
                            NombreAgencia = reader["D_AGE"].ToString(),
                            // Agregar otros campos si son necesarios
                        });
                    }
                }
            }
            return billeteras;
        }


        /*public int GuardarMedioPagoUnico(DTOMedioPago medioPago)
        {
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
                    {
                        {"P_CUIL", medioPago.Cuil},
                        {"P_CBU_INICIO", medioPago.CbuInicio ?? (object)DBNull.Value},
                        {"P_CBU_FINAL", medioPago.CbuFinal ?? (object)DBNull.Value},
                        {"P_C_BANCO", medioPago.CBanco ?? (object)DBNull.Value},
                        {"P_C_AGENCIA", medioPago.CAgencia ?? (object)DBNull.Value},
                        {"P_PE_INICIO_PAGO", medioPago.PeInicioPago},
                        {"P_CVU_1", medioPago.Cvu1 ?? (object)DBNull.Value},
                        {"P_CVU_2", medioPago.Cvu2 ?? (object)DBNull.Value},
                        {"P_ALIAS", medioPago.Alias ?? (object)DBNull.Value},
                        {"P_IP_ORIGEN", medioPago.IpOrigen ?? "0.0.0.0"},
                        {"P_UDAI", medioPago.Udai ?? (object)DBNull.Value},
                        {"P_OPE_TRAMITE", medioPago.OpeTramite ?? "00000000"},
                        {"P_RESULTADO", DBNull.Value}  // Parámetro de salida
                    };

                connection.ExecuteNonQuery("PFOL_MDPUNICO_INSERT", parameters);

                // Obtener el valor de retorno del stored procedure
                int resultado = Convert.ToInt32(parameters["P_RESULTADO"]);

                // Interpretar el resultado
                if (resultado > 0)
                {
                    return 1; // Inserción exitosa
                }
                else
                {
                    return -1; // Inserción fallida
                }
            }
        }*/



        public DTOMedioPagoVigente ObtenerMedioPagoVigente(string cuil)
        {
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
        {
            {"P_CUIL", decimal.Parse(cuil)}
        };
                // trae datos con 12345678901
                using (var reader = connection.ExecuteSP("PFOL_MDPVIGENTE_TRAER", parameters))
                {
                    if (reader.Read())
                    {
                        return new DTOMedioPagoVigente
                        {
                            IdModo = Convert.ToInt64(reader["ID_MODO"]),
                            Cuil = Convert.ToDecimal(reader["CUIL"]),
                            CModoPago = Convert.ToInt32(reader["C_MODO_PAGO"]),
                            CBco = reader["C_BCO"] as short?,
                            CAge = reader["C_AGE"] as short?,
                            Cbu1 = reader["CBU_1"] as string,
                            Cbu2 = reader["CBU_2"] as string,
                            Cvu1 = reader["CVU_1"] as string,
                            Cvu2 = reader["CVU_2"] as string,
                            Alias = reader["ALIAS"] as string,
                            PeDesde = Convert.ToInt32(reader["PE_DESDE"]),
                            PeHasta = reader["PE_HASTA"] as int?,
                            CEstado = Convert.ToInt16(reader["C_ESTADO"]),
                            TimeEstado = Convert.ToDateTime(reader["TIME_ESTADO"]),
                            IdTramite = Convert.ToInt64(reader["ID_TRAMITE"]),
                            FAlta = Convert.ToDateTime(reader["F_ALTA"]),
                            DAge = reader["D_AGE"] as string,
                            IpOrigen = reader["IP_ORIGEN"] as string,
                            Udai = reader["UDAI"] as int?,
                            OpeTramite = reader["OPE_TRAMITE"] as string,
                            TimeTramite = Convert.ToDateTime(reader["TIME_TRAMITE"]),
                            DBco = reader["D_BCO"] as string,
                            CodigoPostal = reader["CODIGO_POSTAL"] as int?,
                            DomCalle = reader["DOM_CALLE"] as string
                        };
                    }
                }
            }
            return null;
        }

        public List<DTOMedioPagoDisponible> ListarMPDisponibles(decimal cuil)
         {
             List<DTOMedioPagoDisponible> mediosPagoDisponibles = new List<DTOMedioPagoDisponible>();
             using (var connection = new Connection())
             {
                 var parameters = new Dictionary<string, object>
                 {
                     {"P_CUIL", cuil}
                 };

                 using (var reader = connection.ExecuteSP("PFSP_MDPTRAER", parameters))
                 {
                     while (reader.Read())
                     {
                         mediosPagoDisponibles.Add(new DTOMedioPagoDisponible
                         {
                             Cuil = GetDecimalValue(reader, "CUIL"),
                             PeEmision = GetDecimalValue(reader, "PE_EMISION"),
                             PeLiquidado = GetDecimalValue(reader, "PE_LIQUIDADO"),
                             CBanco = GetShortValue(reader, "C_BCO"),
                             CAgencia = GetShortValue(reader, "C_AGE"),
                             CSistema = GetShortValue(reader, "C_SISTEMA"),
                             MRetenido = GetStringValue(reader, "M_RETENIDO"),
                             MPago = GetStringValue(reader, "M_PAGO"),
                             CTipoLiq = GetShortValue(reader, "C_TIPOLIQ"),
                             Cbu1 = GetNullableDecimalValue(reader, "CBU_1"),
                             Cbu2 = GetNullableDecimalValue(reader, "CBU_2"),
                             NombreBanco = reader.GetString(reader.GetOrdinal("NOMBREBAN")),
                             NombreAgencia = reader.GetString(reader.GetOrdinal("AGENCIA"))
                         });
                     }
                 }
             }
             return mediosPagoDisponibles;
         }
        // trae con el siguiente 27093098582
        /*public List<DTOMedioPagoDisponible> ListarMPDisponibles(decimal cuil)
        {
            List<DTOMedioPagoDisponible> mediosPago = new List<DTOMedioPagoDisponible>();
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
                {
                    {"P_CUIL", decimal.Parse(cuil)}
                };

                using (var reader = connection.ExecuteSP("PFSP_MDPTRAER", parameters))
                {
                    while (reader.Read())
                    {
                        mediosPago.Add(new DTOMedioPagoDisponible
                        {
                            Cuil = GetDecimalValue(reader, "CUIL"),
                            PeEmision = GetDecimalValue(reader, "PE_EMISION"),
                            PeLiquidado = GetDecimalValue(reader, "PE_LIQUIDADO"),
                            CBanco = GetShortValue(reader, "C_BCO"),
                            CAgencia = GetShortValue(reader, "C_AGE"),
                            CSistema = GetShortValue(reader, "C_SISTEMA"),
                            MRetenido = GetStringValue(reader, "M_RETENIDO"),
                            MPago = GetStringValue(reader, "M_PAGO"),
                            CTipoLiq = GetShortValue(reader, "C_TIPOLIQ"),
                            Cbu1 = GetNullableDecimalValue(reader, "CBU_1"),
                            Cbu2 = GetNullableDecimalValue(reader, "CBU_2"),
                            NombreBanco = reader.GetString(reader.GetOrdinal("NOMBREBAN")),
                            NombreAgencia = reader.GetString(reader.GetOrdinal("AGENCIA"))

                            
                            
                        });
                    }
                }
            }
            return mediosPago;
        }*/

        private string ConcatenarCBU(decimal cbu1, decimal cbu2)
        {
            return $"{cbu1:00000000}{cbu2:0000000000000000}";
        }
    

        //1426
    public List<DTOBanco> TraerBancosFisicos(short codigoPostal)
        {
            List<DTOBanco> bancos = new List<DTOBanco>();
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
                {
                    {"P_C_POSTAL", codigoPostal}
                };

                using (var reader = connection.ExecuteSP("PFSPBCOSFISICOS_TRAER", parameters))
                {
                    while (reader.Read())
                    {
                        bancos.Add(new DTOBanco
                        {
                            Codigo = reader["C_BCO"].ToString(),
                            CodigoAgencia = reader["C_AGE"].ToString(),
                            Nombre = reader["D_BCO"].ToString(),
                            NombreAgencia = reader["D_AGE"].ToString(),
                            Direccion = $"{reader["DOM_CALLE"]} {reader["DOM_NRO"]}, {reader["DOM_LOCA"]}",
                            CodigoPostal = reader["C_POSTAL"].ToString()
                        });
                    }
                }
            }
            return bancos;
        }

        public List<DTOBanco> TraerBancosCorreo(short codigoPostal)
        {
            List<DTOBanco> bancos = new List<DTOBanco>();
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
                {
                    {"P_C_POSTAL", codigoPostal}
                };

                using (var reader = connection.ExecuteSP("PFSPBCOSCORREO_TRAER", parameters))
                {
                    while (reader.Read())
                    {
                        bancos.Add(new DTOBanco
                        {
                            Codigo = reader["C_BCO"].ToString(),
                            CodigoAgencia = reader["C_AGE"].ToString(),
                            Nombre = reader["D_BCO"].ToString(),
                            NombreAgencia = reader["D_AGE"].ToString(),
                            Direccion = $"{reader["DOM_CALLE"]} {reader["DOM_NRO"]}, {reader["DOM_LOCA"]}",
                            CodigoPostal = reader["C_POSTAL"].ToString()

                        });
                    }
                }
            }
            return bancos;
        }

        public List<DTOBanco> ListarBancosVirtuales()
        {
            List<DTOBanco> bancos = new List<DTOBanco>();
            using (var connection = new Connection())
            {
                using (var reader = connection.ExecuteSP("PFSPBCOSVIRTUALES_TRAER"))
                {
                    while (reader.Read())
                    {
                        bancos.Add(new DTOBanco
                        {
                            Codigo = reader["C_BCO"].ToString(),
                            CodigoAgencia = reader["C_AGE"].ToString(),
                            Nombre = reader["D_BCO"].ToString(),
                            NombreAgencia = reader["D_AGE"].ToString(),
                            Direccion = $"{reader["DOM_CALLE"]} {reader["DOM_NRO"]}, {reader["DOM_LOCA"]}",
                            CodigoPostal = reader["C_POSTAL"].ToString()
                        });
                    }
                }
            }
            return bancos;
        }

        /// <summary>
        /// Ejecuta el stored procedure PFSP_NOMBREBANCO para obtener el nombre del banco.
        /// </summary>
        /// <param name="banco">Código del banco (tipo SMALLINT).</param>
        /// <returns>Nombre del banco (valor de D_BCO) o cadena vacía si no se encontró registro.</returns>
        public string ObtenerNombreBanco(short banco)
        {
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
            {
                {"P_BANCO", banco}
            };

                using (var reader = connection.ExecuteSP("PFSP_NOMBREBANCO", parameters))
                {
                    if (reader.Read())
                    {
                        return reader["D_BCO"].ToString();
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Ejecuta el stored procedure PFSP_NOMBREAGENCIA para obtener la información de la agencia.
        /// </summary>
        /// <param name="banco">Código del banco (tipo SMALLINT).</param>
        /// <param name="agencia">Código de la agencia (tipo SMALLINT).</param>
        /// <returns>
        /// Un objeto DTOAgencia con:
        /// - D_AGE: nombre de la agencia
        /// - DOM_CALLE: calle de la agencia
        /// - DOM_NRO: número de la calle
        /// - D_LOCA: localidad de la agencia
        /// Si no se encuentra registro, retorna null.
        /// </returns>
        public DTOAgencia ObtenerNombreAgencia(short banco, short agencia)
        {
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
            {
                {"P_BANCO", banco},
                {"P_AGENCIA", agencia}
            };

                using (var reader = connection.ExecuteSP("PFSP_NOMBREAGENCIA", parameters))
                {
                    if (reader.Read())
                    {
                        return new DTOAgencia
                        {
                            NombreAgencia = reader["D_AGE"].ToString(),
                            Calle = reader["DOM_CALLE"].ToString(),
                            NumeroCalle = reader["DOM_NRO"].ToString(),
                            Localidad = reader["D_LOCA"].ToString()
                        };
                    }
                }
            }
            return null;
        }



        private decimal GetDecimalValue(DB2DataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }

        private decimal? GetNullableDecimalValue(DB2DataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }

        private short GetShortValue(DB2DataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (short)0 : reader.GetInt16(ordinal);
        }

       

        private string GetStringValue(DB2DataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }



        public List<DTOBanco> ConsultarBancosPorCercania(string codigoPostal)
        {
            List<DTOBanco> bancos = new List<DTOBanco>();
            using (var connection = new Connection())
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@CodigoPostal", codigoPostal}
                };

                using (var reader = connection.ExecuteSP("PFSPBCOSFISICOS_TRAER", parameters))
                {
                    while (reader.Read())
                    {
                        bancos.Add(new DTOBanco
                        {
                            Codigo = reader["Codigo"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            Direccion = reader["Direccion"].ToString()
                        });
                    }
                }
            }
            return bancos;
        }

    }
}

