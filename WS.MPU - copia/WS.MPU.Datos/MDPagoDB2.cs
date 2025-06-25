using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2;
using WS.MPU.GestionarMPU.Datos.Models;
using System.Configuration;
using log4net;
using WS.MPU.Negocio;
namespace WS.MPU.GestionarMPU.Datos
{

    public class MDPagoDB2
    {
        private static ILog log = LogManager.GetLogger(typeof(MDPagoDB2).Name);
        private static string nombreServicio = "MDPago";

        public static List<MDPagoDesc> BuscarMediosDP(short? P_C_BCO, short? P_C_AGE, short? P_C_PCIA, string P_D_LOCA, short? P_C_POSTAL, DateTime? F_Presentacion)
        {
            List<MDPagoDesc> mdpago = new List<MDPagoDesc>();
            List<string> Parameters = new List<string>();

            DB2Connection oDB2Connection = null;
            DB2Command oDBCommand = new DB2Command();
            try
            {

                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {
                    // Conexion.obtenerSchema() + ".LAOLListadoMDPagoEXISTENTE", oDB2Connection);
                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;
                    oDBCommand.CommandText = ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LAOL_MDPAGEPAG_TRAER";
                    oDBCommand.Parameters.Add("P_C_BCO", DB2Type.SmallInt, 7, "P_C_BCO");
                    oDBCommand.Parameters[0].Value = P_C_BCO;
                    Parameters.Add(P_C_BCO.ToString());

                    oDBCommand.Parameters.Add("P_C_AGE", DB2Type.SmallInt, 7, "P_C_AGE");
                    oDBCommand.Parameters[1].Value = P_C_AGE;
                    Parameters.Add(P_C_AGE.ToString());

                    oDBCommand.Parameters.Add("P_C_PCIA", DB2Type.SmallInt, 7, "P_C_PCIA");
                    oDBCommand.Parameters[2].Value = P_C_PCIA;
                    Parameters.Add(P_C_PCIA.ToString());

                    oDBCommand.Parameters.Add("P_D_LOCA", DB2Type.VarChar, 60, "P_D_LOCA");
                    oDBCommand.Parameters[3].Value = P_D_LOCA;
                    Parameters.Add(P_D_LOCA.ToString());

                    oDBCommand.Parameters.Add("P_C_POSTAL", DB2Type.SmallInt, 7, "P_C_POSTAL");
                    oDBCommand.Parameters[4].Value = P_C_POSTAL;
                    Parameters.Add(P_C_POSTAL.ToString());

                    oDBCommand.Parameters.Add("F_PRESENTA", DB2Type.Date, 7, "F_PRESENTA");
                    oDBCommand.Parameters[5].Value = F_Presentacion.HasValue ? F_Presentacion.Value.Date : DateTime.MinValue;
                    Parameters.Add(F_Presentacion.ToString());

                    AdministradorDeLog.LogTexto(string.Format("Se llama a LAOL_MDPAGEPAG_TRAER Parametros: P_C_BCO: {0} - P_C_AGE: {1} - P_C_PCIA: {2} - P_D_LOCA: {3} - P_C_POSTAL: {4} ", Parameters.ToArray()), System.Diagnostics.TraceEventType.Information);

                    oDB2Connection.Open();
                    DB2DataReader myReader;
                    myReader = oDBCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        MDPagoDesc mdpd = new MDPagoDesc();

                        mdpd.Calle = myReader["DOM_CALLE"].Equals(DBNull.Value) ? null : myReader["DOM_CALLE"].ToString();
                        mdpd.Cod_Agencia = myReader["C_AGE"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_AGE"].ToString());
                        mdpd.Cod_AgentePag = myReader["C_TIPOAGPA"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_TIPOAGPA"].ToString());
                        mdpd.Cod_Banco = myReader["C_BCO"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_BCO"].ToString());
                        mdpd.Cod_Postal = myReader["C_POSTAL"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_POSTAL"].ToString());
                        mdpd.Cod_Prov = myReader["C_PCIA"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_PCIA"].ToString());
                        mdpd.Desc_Banco = myReader["D_BCO"].Equals(DBNull.Value) ? null : myReader["D_BCO"].ToString();
                        mdpd.Desc_Agencia = myReader["D_AGE"].Equals(DBNull.Value) ? null : myReader["D_AGE"].ToString();
                        mdpd.Dv_Ubic = myReader["DV_UBICAC"].Equals(DBNull.Value) ? 0 : decimal.Parse(myReader["DV_UBICAC"].ToString());
                        mdpd.Localidad = myReader["DOM_LOCA"].Equals(DBNull.Value) ? null : myReader["DOM_LOCA"].ToString();
                        mdpd.Numero = myReader["DOM_NRO"].Equals(DBNull.Value) ? null : myReader["DOM_NRO"].ToString();
                        mdpd.Prioridad = myReader["PRIORIDAD"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["PRIORIDAD"].ToString());

                        mdpago.Add(mdpd);
                    }
                    return mdpago;
                }
            }
            catch (DB2Exception dex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, dex.Source, dex.Message, Environment.NewLine, dex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("No se pueden consultar los datos. Permiso denegado.");
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
            finally
            {
                if (oDB2Connection.State != ConnectionState.Closed)
                {
                    oDB2Connection.Close();
                }

                oDB2Connection.Dispose();
                oDB2Connection = null;

                oDBCommand.Dispose();
                oDBCommand = null;
            }

        }

        public static bool ExisteHistorial(long cuil)
        {
            bool historial = false;

            DB2Connection oDB2Connection = null;
            DB2Command oDBCommand = new DB2Command();
            try
            {
                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {

                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;
                    oDBCommand.CommandText =
                    ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LAOL_MEDPAGO_HIST_EXIST";
                    oDBCommand.Parameters.Add("P_CUIL", DB2Type.Decimal, 11, "P_CUIL");
                    oDBCommand.Parameters[0].Value = cuil;

                    oDBCommand.Parameters.Add("P_EXISTENTE", DB2Type.SmallInt, 6, "P_EXISTENTE");
                    oDBCommand.Parameters[1].Direction = ParameterDirection.Output;

                    AdministradorDeLog.LogTexto(string.Format("Se llama a LAOL_MEDPAGO_HIST_EXIST Parametros: CUIL: {0} ", cuil), System.Diagnostics.TraceEventType.Information);

                    oDB2Connection.Open();
                    DB2DataReader myReader;
                    myReader = oDBCommand.ExecuteReader();


                    historial = (short)oDBCommand.Parameters["P_EXISTENTE"].Value == 1;

                    return historial;
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
            finally
            {
                if (oDB2Connection.State != ConnectionState.Closed)
                {
                    oDB2Connection.Close();
                }

                oDB2Connection.Dispose();
                oDB2Connection = null;

                oDBCommand.Dispose();
                oDBCommand = null;
            }
        }



        public static MDPagoVig TraerMDPVigente(long cuil)
        {
            MDPagoVig mdp = new MDPagoVig();

            DB2Connection oDB2Connection = null;
            DB2Command oDBCommand = new DB2Command();
            try
            {
                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {
                    // Conexion.obtenerSchema() + ".LAOLListadoMDPagoEXISTENTE", oDB2Connection);
                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;
                    oDBCommand.CommandText =
                    ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LAOL_MDPVIGENTE_TRAER";
                    oDBCommand.Parameters.Add("P_CUIL", DB2Type.Decimal, 11, "P_CUIL");
                    oDBCommand.Parameters[0].Value = cuil;

                    AdministradorDeLog.LogTexto(string.Format("Se llama a LAOL_MDPVIGENTE_TRAER Parametros: CUIL: {0} ", cuil), System.Diagnostics.TraceEventType.Information);

                    oDB2Connection.Open();
                    DB2DataReader myReader;
                    myReader = oDBCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        //   mdp.Historial = true;
                        mdp.Tramite = myReader["ID_TRAMITE"].Equals(DBNull.Value) ? 0 : Int64.Parse(myReader["ID_TRAMITE"].ToString());
                        mdp.CBUInicio = myReader["CBU_1"].Equals(DBNull.Value) ? null : myReader["CBU_1"].ToString();
                        mdp.CBUFinal = myReader["CBU_2"].Equals(DBNull.Value) ? null : myReader["CBU_2"].ToString();
                        mdp.CodBanco = myReader["C_BCO"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_BCO"].ToString());
                        mdp.CodAgencia = myReader["C_AGE"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_AGE"].ToString());
                        mdp.CodigoPostal = (myReader["CODIGO_POSTAL"].Equals(DBNull.Value) ? null : myReader["CODIGO_POSTAL"].ToString());
                        mdp.DesBanco = (myReader["D_BCO"].Equals(DBNull.Value) ? null : myReader["D_BCO"].ToString());
                        mdp.DesAgencia = (myReader["D_AGE"].Equals(DBNull.Value) ? null : myReader["D_AGE"].ToString());

                        if (myReader["PE_DESDE"].Equals(DBNull.Value))
                        {
                            mdp.PeDesde = new DateTime();
                        }
                        else
                        {
                            if (DateTime.TryParseExact(myReader["PE_DESDE"].ToString(), "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                            {
                                mdp.PeDesde = dt;
                            }
                            else
                            {
                                mdp.PeDesde = new DateTime();
                            }
                        }
                        if (!myReader["PE_HASTA"].Equals(DBNull.Value))
                        {
                            if (DateTime.TryParseExact(myReader["PE_HASTA"].ToString(), "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                            {
                                mdp.PeHasta = dt;
                            }
                        }

                        mdp.DomCalle = myReader["DOM_CALLE"].Equals(DBNull.Value) ? null : myReader["DOM_CALLE"].ToString();
                        mdp.ModoPago = myReader["C_MODO_PAGO"].Equals(DBNull.Value) ? (short)0 : Int16.Parse(myReader["C_MODO_PAGO"].ToString());
                    }
                    return mdp;
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
            finally
            {
                if (oDB2Connection.State != ConnectionState.Closed)
                {
                    oDB2Connection.Close();
                }

                oDB2Connection.Dispose();
                oDB2Connection = null;

                oDBCommand.Dispose();
                oDBCommand = null;
            }
        }

        public static AltaResponse Insertar(MDPago BDpago)
        {
            int myReader;
            AltaResponse respuesta = new AltaResponse();
            respuesta.Mensajes = new List<string>();
            List<string> Parameters = new List<string>();

            AdministradorDeLog.LogTexto(string.Format("Llega a Insertar: " +
                        "P_UDAI: {0} - P_OPE_TRAMITE: {1} - P_F_PRESENTACION: {2} - P_CUIL: {3} - " +
                        "P_CUIT: {4} - P_CBU_INICIO: {5} - P_CBU_FINAL: {6} - P_C_BANCO: {7} - " +
                        "P_C_AGENCIA: {8} - P_C_CUENTA_JUDICIAL: {9} ", BDpago.Udai, BDpago.OperadorTramite, BDpago.FechaPrensentacion, BDpago.Cuil
                        , BDpago.Cuit, BDpago.CBU_Inicio, BDpago.CBU_Final, BDpago.Cod_Banco, BDpago.Cod_Agencia, BDpago.Cod_Cuenta_Judicial), System.Diagnostics.TraceEventType.Information);

            if (BDpago.Cod_Agencia == null)
                if (Convert.ToDouble(BDpago.CBU_Inicio) < 100000)
                {
                    AdministradorDeLog.LogTexto("ERROR EN EL CBU DETECTADO", System.Diagnostics.TraceEventType.Information);
                    throw new Exception("CBU No VALIDO");
                };

            try
            {
                DB2Connection oDB2Connection = null;
                DB2Command oDBCommand = Conexion.ObtenerDB2Command();
                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {
                    // new DB2Command(Conexion.obtenerSchema() + ".LAOL_ListadoMDPago_INSERT", oDB2Connection);
                    oDBCommand.CommandText = ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LAOL_MEDPAGO_INSERT";
                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;

                    oDBCommand.Parameters.Add("P_NRO_TRAMITE", DB2Type.BigInt, 7, "P_NRO_TRAMITE");
                    oDBCommand.Parameters[0].Direction = ParameterDirection.InputOutput;
                    oDBCommand.Parameters[0].Value = 0;

                    oDBCommand.Parameters.Add("P_UDAI", DB2Type.Decimal, 9, "P_UDAI");
                    oDBCommand.Parameters[1].Value = Decimal.Parse(BDpago.Udai.ToString());
                    Parameters.Add(BDpago.Udai.ToString());

                    oDBCommand.Parameters.Add("P_OPE_TRAMITE", DB2Type.Char, 8, "P_OPE_TRAMITE");
                    oDBCommand.Parameters[2].Value = BDpago.OperadorTramite;
                    Parameters.Add(BDpago.OperadorTramite);

                    oDBCommand.Parameters.Add("P_F_PRESENTACION", DB2Type.Date, 10, "P_F_PRESENTACION");
                    oDBCommand.Parameters[3].Value = BDpago.FechaPrensentacion.Date;
                    Parameters.Add(BDpago.FechaPrensentacion.Date.ToString());

                    oDBCommand.Parameters.Add("P_CUIL", DB2Type.Decimal, 11, "P_CUIL");
                    oDBCommand.Parameters[4].Value = BDpago.Cuil;
                    Parameters.Add(BDpago.Cuil.ToString());

                    oDBCommand.Parameters.Add("P_CUIT", DB2Type.Decimal, 11, "P_CUIT");
                    oDBCommand.Parameters[5].Value = BDpago.Cuit;
                    Parameters.Add(BDpago.Cuil.ToString());

                    oDBCommand.Parameters.Add("P_CBU_INICIO", DB2Type.Decimal, 8, "P_CBU_INICIO");
                    oDBCommand.Parameters[6].Value = Convert.ToDecimal(BDpago.CBU_Inicio);
                    Parameters.Add(BDpago.CBU_Inicio);

                    oDBCommand.Parameters.Add("P_CBU_FINAL", DB2Type.Decimal, 14, "P_CBU_FINAL");
                    oDBCommand.Parameters[7].Value = Convert.ToDecimal(BDpago.CBU_Final);
                    Parameters.Add(BDpago.CBU_Final);

                    oDBCommand.Parameters.Add("P_C_BANCO", DB2Type.SmallInt, 7, "P_C_BANCO");
                    oDBCommand.Parameters[8].Value = BDpago.Cod_Banco;
                    Parameters.Add(BDpago.Cod_Banco.ToString());

                    oDBCommand.Parameters.Add("P_C_AGENCIA", DB2Type.SmallInt, 7, "P_C_AGENCIA");
                    oDBCommand.Parameters[9].Value = BDpago.Cod_Agencia;
                    Parameters.Add(BDpago.Cod_Agencia.ToString());

                    oDBCommand.Parameters.Add("P_C_CUENTA_JUDICIAL", DB2Type.Decimal, 9, "P_C_CUENTA_JUDICIAL");
                    oDBCommand.Parameters[10].Value = BDpago.Cod_Cuenta_Judicial;
                    Parameters.Add(BDpago.Cod_Cuenta_Judicial.ToString());

                    oDBCommand.Parameters.Add("P_ERROR", DB2Type.VarChar, 60, "P_ERROR");
                    oDBCommand.Parameters[11].Direction = ParameterDirection.InputOutput;
                    oDBCommand.Parameters[11].Value = "";

                    oDBCommand.Parameters.Add("P_PE_INICIO_PAGO", DB2Type.Decimal, 6, "P_PE_INICIO_PAGO");
                    oDBCommand.Parameters[12].Direction = ParameterDirection.Output;

                    oDBCommand.Parameters.Add("P_PE_FIN_PAGO", DB2Type.Decimal, 6, "P_PE_FIN_PAGO");
                    oDBCommand.Parameters[13].Direction = ParameterDirection.Output;

                    log = AdministradorDeLog.LogTexto(string.Format("Se llama a LAOL_MEDPAGO_INSERT Parametros: " +
                        "P_UDAI: {0} - P_OPE_TRAMITE: {1} - P_F_PRESENTACION: {2} - P_CUIL: {3} - " +
                        "P_CUIT: {4} - P_CBU_INICIO: {5} - P_CBU_FINAL: {6} - P_C_BANCO: {7} - " +
                        "P_C_AGENCIA: {8} - P_C_CUENTA_JUDICIAL: {9} ", Parameters.ToArray()), System.Diagnostics.TraceEventType.Information);

                    try
                    {
                        oDB2Connection.Open();
                        myReader = oDBCommand.ExecuteNonQuery();

                        respuesta.ID_tramite = oDBCommand.Parameters[0].Value.Equals(DBNull.Value) ? 0 : int.Parse(oDBCommand.Parameters[0].Value.ToString());
                        respuesta.Mensajes.Add(oDBCommand.Parameters[11].Value.ToString());
                        respuesta.FechaInicioPago = oDBCommand.Parameters[12].Value.Equals(DBNull.Value) ? 0 : decimal.Parse(oDBCommand.Parameters[12].Value.ToString());
                        respuesta.FechaFinPago = oDBCommand.Parameters[13].Value.Equals(DBNull.Value) ? 0 : decimal.Parse(oDBCommand.Parameters[13].Value.ToString());
                        ListadoMDPago mdp = TraerMedioDePagoXID(respuesta.ID_tramite.Value);
                        respuesta.Nro_tramite = mdp.NroTramite;

                        var datos = new LogTramite
                        {
                            CUIL = BDpago.Cuil,
                            NumeroDeTramite = respuesta.ID_tramite.Value,
                            FechaDeCarga = BDpago.FechaPrensentacion,
                        };

                        var auditoria = new LoggerTramites(datos);
                        auditoria.LogTramite(datos.CUIL.ToString(), nombreServicio, "MDPagoDB2.Insertar", "LAMODO_PAGODT", BDpago.Udai, BDpago.OperadorTramite, BDpago.IpOrigen, TipoAction.AGREGAR, log);

                        return respuesta;
                    }
                    catch (DB2Exception dex)
                    {
                        AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, dex.Source, dex.Message, Environment.NewLine, dex.StackTrace), System.Diagnostics.TraceEventType.Error);
                        throw new Exception("Hemos tenido un error! intente nuevamente mas tarde.");
                    }
                    catch (Exception ex)
                    {
                        AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                        return new AltaResponse();
                    }
                    finally
                    {
                        if (oDB2Connection.State != ConnectionState.Closed)
                        {
                            oDB2Connection.Close();
                        }

                        oDB2Connection.Dispose();
                        oDB2Connection = null;

                        oDBCommand.Dispose();
                        oDBCommand = null;
                    }
                }
            }
            catch (DB2Exception dex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, dex.Source, dex.Message, Environment.NewLine, dex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw dex;
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
        }

        /// <summary>
        /// Trae el historial de trámites de bocas de pago para un cuil
        /// </summary>
        /// <param name="cuil"></param>
        /// <returns>Lista de entidad de bocas de pago</returns>
        public static List<ListadoMDPago> BuscarListaMDPago(decimal? cuil, AuditoriaConsulta auditoria)
        {
            List<ListadoMDPago> Lst = new List<ListadoMDPago>();

            DB2Connection oDB2Connection = null;
            DB2Command oDBCommand = new DB2Command();
            try
            {
                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {
                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;
                    oDBCommand.CommandText = ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LAOL_MEDPAGO_LISTA";
                    oDBCommand.Parameters.Add("P_CUIL", DB2Type.Decimal, 11, "P_CUIL");
                    oDBCommand.Parameters[0].Value = cuil;

                    AdministradorDeLog.LogTexto(string.Format("Se llama a LAOL_MEDPAGO_LISTA Parametros: CUIL: {0} ", cuil), System.Diagnostics.TraceEventType.Information);

                    oDB2Connection.Open();
                    DB2DataReader myReader;
                    myReader = oDBCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        ListadoMDPago p = new ListadoMDPago();
                        p.IdTramite = myReader["TRAMITE"].Equals(DBNull.Value) ? 0 : Int64.Parse(myReader["TRAMITE"].ToString());
                        p.NroTramite = myReader["NROTRAMITE"].Equals(DBNull.Value) ? 0 : Int64.Parse(myReader["NROTRAMITE"].ToString());
                        p.CodigoUdai = myReader["UDAI"].Equals(DBNull.Value) ? null : myReader["UDAI"].ToString();
                        p.DescripcionUdai = myReader["OFICINA"].Equals(DBNull.Value) ? string.Empty : myReader["OFICINA"].ToString();
                        p.OperadorTramite = myReader["OPERADOR"].Equals(DBNull.Value) ? string.Empty : myReader["OPERADOR"].ToString();
                        p.FechaPresentacion = myReader["FECHAPRESENTACION"].Equals(DBNull.Value) ? new DateTime(1900, 1, 1) : DateTime.Parse(myReader["FECHAPRESENTACION"].ToString());
                        p.CodigoEstado = myReader["IDESTADO"].Equals(DBNull.Value) ? 0 : Int16.Parse(myReader["IDESTADO"].ToString());
                        p.DescripcionEstado = myReader["DESCRIPCIONESTADO"].Equals(DBNull.Value) ? string.Empty : myReader["DESCRIPCIONESTADO"].ToString();
                        p.ModoDePago = myReader["MODODEPAGO"].Equals(DBNull.Value) ? string.Empty : myReader["MODODEPAGO"].ToString();
                        p.CBU = myReader["CBU"].Equals(DBNull.Value) ? string.Empty : myReader["CBU"].ToString();
                        var pDesde = myReader["PERIODODESDE"].Equals(DBNull.Value) ? string.Empty : myReader["PERIODODESDE"].ToString();
                        var pHasta = myReader["PERIODOHASTA"].Equals(DBNull.Value) ? string.Empty : myReader["PERIODOHASTA"].ToString();
                        p.PeriodoDesde = (pDesde == string.Empty || pDesde == "0") ? string.Empty : pDesde.Substring(4) + "/" + pDesde.Substring(0, 4);
                        p.PeriodoHasta = (pHasta == string.Empty || pHasta == "0") ? string.Empty : pHasta.Substring(4) + "/" + pHasta.Substring(0, 4);
                        p.CuentaJudicial = myReader["CUENTAJUDICIAL"].Equals(DBNull.Value) ? string.Empty : myReader["CUENTAJUDICIAL"].ToString();
                        p.Banco = myReader["DES_BANCO"].Equals(DBNull.Value) ? string.Empty : myReader["DES_BANCO"].ToString();
                        p.Agencia = myReader["DES_AGENCIA"].Equals(DBNull.Value) ? string.Empty : myReader["DES_AGENCIA"].ToString();
                        p.DomicilioAgencia = myReader["DOMICILIO_AGE"].Equals(DBNull.Value) ? string.Empty : myReader["DOMICILIO_AGE"].ToString();
                        p.LocalidadAgencia = myReader["LOCALIDAD_AGE"].Equals(DBNull.Value) ? string.Empty : myReader["LOCALIDAD_AGE"].ToString();
                        p.CodigoPostal = myReader["CODIGO_POSTAL"].Equals(DBNull.Value) ? string.Empty : myReader["CODIGO_POSTAL"].ToString();
                        p.CodigoProvicia = myReader["CODIGO_PROVINCIA"].Equals(DBNull.Value) ? (short?)null : short.Parse(myReader["CODIGO_PROVINCIA"].ToString());
                        p.Provincia = myReader["D_PROVINCIA"].Equals(DBNull.Value) ? string.Empty : myReader["D_PROVINCIA"].ToString();
                        p.CodigoBanco = myReader["CODIGO_BANCO"].Equals(DBNull.Value) ? (short?)null : short.Parse(myReader["CODIGO_BANCO"].ToString());
                        p.CodigoAgencia = myReader["CODIGO_AGENCIA"].Equals(DBNull.Value) ? (short?)null : short.Parse(myReader["CODIGO_AGENCIA"].ToString());
                        p.FechaCarga = myReader["TIME_INICIO"].Equals(DBNull.Value) ? (DateTime?)null : DateTime.Parse(myReader["TIME_INICIO"].ToString()).Date;

                        Lst.Add(p);
                    }

                    var logger = new LoggerTramites(cuil.ToString());
                    logger.LogTramite(cuil.ToString(), "MDPagoDB2", "MDPagoDB2.BuscarListaMDPago", "LAMODO_PAGODT", auditoria.UDAI, auditoria.Operador, auditoria.IpOrigen, LoggingCuna.TipoAction.CONSULTAR, log);

                    return Lst;
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
            finally
            {
                if (oDB2Connection.State != ConnectionState.Closed)
                {
                    oDB2Connection.Close();
                }

                oDB2Connection.Dispose();
                oDB2Connection = null;

                oDBCommand.Dispose();
                oDBCommand = null;
            }

        }

        public static ListadoMDPago TraerMedioDePagoXID(long id_tramite)
        {
            ListadoMDPago p = new ListadoMDPago();

            DB2Connection oDB2Connection = null;
            DB2Command oDBCommand = new DB2Command();
            try
            {
                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {
                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;
                    oDBCommand.CommandText = ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LAOL_MDP_XID";
                    oDBCommand.Parameters.Add("P_ID_TRAMITE", DB2Type.Decimal, 11, "P_ID_TRAMITE");
                    oDBCommand.Parameters[0].Value = id_tramite;

                    AdministradorDeLog.LogTexto(string.Format("Se llama a LAOL_MEDPAGO_LISTA Parametros: IdTramite: {0} ", id_tramite.ToString()), System.Diagnostics.TraceEventType.Information);

                    oDB2Connection.Open();
                    DB2DataReader myReader;
                    myReader = oDBCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        p.IdTramite = myReader["TRAMITE"].Equals(DBNull.Value) ? 0 : Int64.Parse(myReader["TRAMITE"].ToString());
                        p.NroTramite = myReader["NROTRAMITE"].Equals(DBNull.Value) ? 0 : Int64.Parse(myReader["NROTRAMITE"].ToString());
                        p.CodigoUdai = myReader["UDAI"].Equals(DBNull.Value) ? null : myReader["UDAI"].ToString();
                        p.DescripcionUdai = myReader["OFICINA"].Equals(DBNull.Value) ? string.Empty : myReader["OFICINA"].ToString();
                        p.OperadorTramite = myReader["OPERADOR"].Equals(DBNull.Value) ? string.Empty : myReader["OPERADOR"].ToString();
                        p.FechaPresentacion = myReader["FECHAPRESENTACION"].Equals(DBNull.Value) ? new DateTime(1900, 1, 1) : DateTime.Parse(myReader["FECHAPRESENTACION"].ToString());
                        p.CodigoEstado = myReader["IDESTADO"].Equals(DBNull.Value) ? 0 : Int16.Parse(myReader["IDESTADO"].ToString());
                        p.DescripcionEstado = myReader["DESCRIPCIONESTADO"].Equals(DBNull.Value) ? string.Empty : myReader["DESCRIPCIONESTADO"].ToString();
                        p.ModoDePago = myReader["MODODEPAGO"].Equals(DBNull.Value) ? string.Empty : myReader["MODODEPAGO"].ToString();
                        p.CBU = myReader["CBU"].Equals(DBNull.Value) ? string.Empty : myReader["CBU"].ToString();
                        var pDesde = myReader["PERIODODESDE"].Equals(DBNull.Value) ? string.Empty : myReader["PERIODODESDE"].ToString();
                        var pHasta = myReader["PERIODOHASTA"].Equals(DBNull.Value) ? string.Empty : myReader["PERIODOHASTA"].ToString();
                        p.PeriodoDesde = (pDesde == string.Empty || pDesde == "0") ? string.Empty : pDesde.Substring(4) + "/" + pDesde.Substring(0, 4);
                        p.PeriodoHasta = (pHasta == string.Empty || pHasta == "0") ? string.Empty : pHasta.Substring(4) + "/" + pDesde.Substring(0, 4);
                        p.CuentaJudicial = myReader["CUENTAJUDICIAL"].Equals(DBNull.Value) ? string.Empty : myReader["CUENTAJUDICIAL"].ToString();
                        p.Banco = myReader["DES_BANCO"].Equals(DBNull.Value) ? string.Empty : myReader["DES_BANCO"].ToString();
                        p.Agencia = myReader["DES_AGENCIA"].Equals(DBNull.Value) ? string.Empty : myReader["DES_AGENCIA"].ToString();
                        p.DomicilioAgencia = myReader["DOMICILIO_AGE"].Equals(DBNull.Value) ? string.Empty : myReader["DOMICILIO_AGE"].ToString();
                        p.LocalidadAgencia = myReader["LOCALIDAD_AGE"].Equals(DBNull.Value) ? string.Empty : myReader["LOCALIDAD_AGE"].ToString();
                        p.CodigoPostal = myReader["CODIGO_POSTAL"].Equals(DBNull.Value) ? string.Empty : myReader["CODIGO_POSTAL"].ToString();
                        p.CodigoProvicia = myReader["CODIGO_PROVINCIA"].Equals(DBNull.Value) ? (short?)null : short.Parse(myReader["CODIGO_PROVINCIA"].ToString());
                        p.Provincia = myReader["D_PROVINCIA"].Equals(DBNull.Value) ? string.Empty : myReader["D_PROVINCIA"].ToString();
                        p.CodigoBanco = myReader["CODIGO_BANCO"].Equals(DBNull.Value) ? (short?)null : short.Parse(myReader["CODIGO_BANCO"].ToString());
                        p.CodigoAgencia = myReader["CODIGO_AGENCIA"].Equals(DBNull.Value) ? (short?)null : short.Parse(myReader["CODIGO_AGENCIA"].ToString());
                        p.FechaCarga = myReader["TIME_INICIO"].Equals(DBNull.Value) ? (DateTime?)null : DateTime.Parse(myReader["TIME_INICIO"].ToString()).Date;
                    }
                    return p;
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
            finally
            {
                if (oDB2Connection.State != ConnectionState.Closed)
                {
                    oDB2Connection.Close();
                }

                oDB2Connection.Dispose();
                oDB2Connection = null;

                oDBCommand.Dispose();
                oDBCommand = null;
            }
        }

        /// <summary>
        /// Consulta por todos los CUILes que tiene asociado un CBU y valida que si encuentra otro sea Cónyuge, Conviviente o Conviviente Previsonal del CUIL ingresado
        /// </summary>
        /// <param name="CBU1"></param>
        /// <param name="CBU2"></param>
        /// <returns></returns>
        public static List<string> PreexistenciaCBU(string CBU1, string CBU2)
        {

            List<string> respueta = new List<string>();
            DB2Connection oDB2Connection = null;
            DB2Command oDBCommand = new DB2Command();
            try
            {
                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {
                    // Conexion.obtenerSchema() + ".LAOLListadoMDPagoEXISTENTE", oDB2Connection);
                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;
                    oDBCommand.CommandText =
                    ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LAOL_PREEXISTENCIA_CBU";
                    oDBCommand.Parameters.Add("P_CBU_INICIO", DB2Type.Decimal, 8, "P_CBU_INICIO");
                    oDBCommand.Parameters[0].Value = Decimal.Parse(CBU1);
                    oDBCommand.Parameters.Add("P_CBU_FIN", DB2Type.Decimal, 14, "P_CBU_FIN");
                    oDBCommand.Parameters[1].Value = Decimal.Parse(CBU2);

                    AdministradorDeLog.LogTexto(string.Format("Se llama a LAOL_PREEXISTENCIA_CBU Parametros: P_CBU_INICIO: {0} - P_CBU_FIN: {1}", Decimal.Parse(CBU1), Decimal.Parse(CBU2)), System.Diagnostics.TraceEventType.Information);

                    oDB2Connection.Open();
                    DB2DataReader myReader;
                    myReader = oDBCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        respueta.Add(myReader["CUIL"].ToString());
                    }
                    return respueta;
                }
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
            finally
            {
                if (oDB2Connection.State != ConnectionState.Closed)
                {
                    oDB2Connection.Close();
                }

                oDB2Connection.Dispose();
                oDB2Connection = null;

                oDBCommand.Dispose();
                oDBCommand = null;
            }
        }

        public static List<ListadoBancoInhib> ListadoCodBancoInhibido()
        {
            List<ListadoBancoInhib> codBanc = new List<ListadoBancoInhib>();


            DB2Connection oDB2Connection = null;
            DB2Command oDBCommand = new DB2Command();
            try
            {
                using (oDB2Connection = Conexion.ObtenerConnexionDB2())
                {
                    oDBCommand.CommandType = CommandType.StoredProcedure;
                    oDBCommand.Connection = oDB2Connection;
                    oDBCommand.CommandText = ConfigurationManager.AppSettings["SchemaDB2"].ToString() + ".LASP_COD_BANCOINHIB_LISTA";


                    AdministradorDeLog.LogTexto(string.Format("Se llama a LASP_COD_BANCOINHIB_LISTA"), System.Diagnostics.TraceEventType.Information);

                    oDB2Connection.Open();
                    DB2DataReader myReader;
                    myReader = oDBCommand.ExecuteReader();
                    while (myReader.Read())
                    {

                        var bancos = new ListadoBancoInhib
                        {
                            codBanInhib = Convert.ToInt16(myReader["C_BANCO"]).ToString("D3"), //myReader["C_BANCO"].ToString(), //Convert.ToInt16(myReader["C_BANCO"])
                            descripBanco = myReader["D_BANCO"].ToString()
                        };
                        codBanc.Add(bancos);

                    }
                    return codBanc;
                }
            }
            catch (DB2Exception dex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, dex.Source, dex.Message, Environment.NewLine, dex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw new Exception("No se pueden consultar los datos. Permiso denegado.");
            }
            catch (Exception ex)
            {
                AdministradorDeLog.LogTexto(string.Format("{0} - Error:{1}->{2} {3}StackTrace: {4}", MethodBase.GetCurrentMethod().Name, ex.Source, ex.Message, Environment.NewLine, ex.StackTrace), System.Diagnostics.TraceEventType.Error);
                throw ex;
            }
            finally
            {
                if (oDB2Connection.State != ConnectionState.Closed)
                {
                    oDB2Connection.Close();
                }

                oDB2Connection.Dispose();
                oDB2Connection = null;

                oDBCommand.Dispose();
                oDBCommand = null;
            }
        }
    }
}

