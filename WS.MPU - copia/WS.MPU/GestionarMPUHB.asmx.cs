using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Services;
using log4net;
using WS.MPU;
using WS.MPU.GestionarMPU.Datos.Models;
using WS.MPU.Negocio;
using WS.MPU.GestionarMPU;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;


namespace WS.MPU
{
    /// <summary>
    /// Descripción breve de GestionMPU
    /// </summary>
    [WebService(Namespace = "http://MPU.anses.gov.ar")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class GestionMPU : System.Web.Services.WebService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GestionMPU).Name);

        #region Bancos
        [WebMethod(Description = "Consulta los bancos por cercanía según el código postal.")]
        public List<DTOBanco> ConsultarBancosPorCercania(string codigoPostal)
        {
            List<DTOBanco> resp = new List<DTOBanco>();
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.ConsultarBancosPorCercania(codigoPostal);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                throw;
            }
            return resp;
        }
        #endregion

        #region MedioPago
        
        [WebMethod(Description = "Obtiene el medio de pago vigente para un CUIL.")]
        public DTOMedioPagoVigente ObtenerMedioPagoVigente(string cuil)
        {
            DTOMedioPagoVigente resp = null;
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                //12345678901
                resp = negocio.ObtenerMedioPagoVigente(cuil);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                //throw;
            }
            return resp;
        }

        [WebMethod(Description = "Guarda un nuevo medio de pago único para un beneficiario.")]
        public int GuardarMedioPagoUnico(DTOMedioPago medioPago)
        {
            int resp = -1;
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
               
                // Hardcodeamos un objeto DTOMedioPago con valores coherentes para pruebas
               /* DTOMedioPago medioPago = new DTOMedioPago
                {
                    Cuil = 23111111111,
                    TipoMedioPago = "BilleteraVirtual",
                    PeInicioPago = 202412,
                    IpOrigen = "10.86.30.26",
                    OpeTramite = "MIANSES",
                    CAgencia= 1,
                    CBanco= 834,
                    Udai= 99999995

                };*/
                // Objeto para CBU
                /*DTOMedioPago medioPagoCBU = new DTOMedioPago
                {
                    Cuil = 12345678901,
                    TipoMedioPago = "CBU",
                    CbuInicio = 15645961,
                    CbuFinal = 52648596415,
                    PeInicioPago = 202408,
                    IpOrigen = "192.168.1.100",
                    Udai = 0,
                    OpeTramite = "TEST0001"
                };

                // Objeto para CVU
                DTOMedioPago medioPagoCVU = new DTOMedioPago
                {
                    Cuil = 12345678901,
                    TipoMedioPago = "CVU",
                    CBanco = 7,
                    Cvu1 = 00000123,
                    Cvu2 = 45678901234567,
                    PeInicioPago = 202408,
                    IpOrigen = "192.168.1.101",
                    Udai = 0,
                    OpeTramite = "TEST0002"
                };

                DTOMedioPago medioPagoCV = new DTOMedioPago
                {
                    Cuil = 12345678901,
                    TipoMedioPago = "CVU",
                    Cvu1 = 00000123,
                    Cvu2 = 45678901234567,
                    PeInicioPago = 202408,
                    IpOrigen = "192.168.1.100",
                    Udai = 0,
                    OpeTramite = "TEST0001"
                };


                // Objeto para Banco/Agencia
                DTOMedioPago medioPagoBancoAgencia = new DTOMedioPago
                {
                    Cuil = 12345678901,
                    TipoMedioPago = "Banco/Agencia",
                    CBanco = 7,
                    CAgencia = 23,
                    PeInicioPago = 202408,
                    IpOrigen = "192.168.1.102",
                    Udai = 3,
                    OpeTramite = "TEST0003"
                };

                // Objeto para Alias
                DTOMedioPago medioPagoAlias = new DTOMedioPago
                {
                    Cuil = 12345678901,
                    TipoMedioPago = "Alias",
                    Alias = "mi.alias.banco",
                    PeInicioPago = 202408,
                    IpOrigen = "192.168.1.103",
                    Udai = 4,
                    OpeTramite = "TEST0004"
                };
               */

                if (!negocio.ValidarBeneficiario(medioPago.Cuil.ToString()))
                {
                    throw new Exception("El beneficiario no cumple con los requisitos para usar este servicio.");
                }

                if ((medioPago.TipoMedioPago == "CBU" || medioPago.TipoMedioPago == "CVU") &&
                    !negocio.ValidarCBUCVU(medioPago.TipoMedioPago, medioPago.ObtenerValor(), medioPago.Cuil.ToString()))
                {
                    throw new Exception("El CBU/CVU no es válido o no pertenece al beneficiario.");
                }

                resp = negocio.GuardarMedioPagoUnico(medioPago);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                //throw;
            }

            return resp;
        }
        [WebMethod(Description = "Lista los medios de pago disponibles para un CUIL.")]
        public List<DTOMedioPagoDisponible> ListarMPDisponibles(string cuil)
        {
            List<DTOMedioPagoDisponible> resp = new List<DTOMedioPagoDisponible>();
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                //27026825917
                resp = negocio.ListarMPDisponibles(decimal.Parse(cuil));
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
               //throw;
            }
            return resp;
        }
        [WebMethod(Description = "Consulta los bancos físicos por código postal.")]
        public List<DTOBanco> TraerBancosFisicos(short codigoPostal)
        {
            List<DTOBanco> resp = new List<DTOBanco>();
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.TraerBancosFisicos(codigoPostal);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                //throw;
            }
            return resp;
        }

        [WebMethod(Description = "Lista los bancos virtuales disponibles.")]
        public List<DTOBanco> ListarBancosVirtuales()
        {
            List<DTOBanco> resp = new List<DTOBanco>();
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.ListarBancosVirtuales();
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                //throw;
            }
            return resp;
        }

        [WebMethod(Description = "Lista las billeteras virtuales disponibles.")]
        public List<DTOBanco> ListarBilleterasVirtuales()
        {
            List<DTOBanco> resp = new List<DTOBanco>();
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.ListarBilleterasVirtuales();
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                //throw;
            }
            return resp;
        }


        [WebMethod(Description = "Lista las bancos correo disponibles.")]
        public List<DTOBanco> ListarBancosCorreo(short codigoPostal)
        {
            List<DTOBanco> resp = new List<DTOBanco>();
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.ListarBancosCorreo(codigoPostal);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                //throw;
            }
            return resp;
        }

        [WebMethod(Description = "Obtiene apoderados.")]
        public decimal ObtenerApoderado(DTOApoderado apod)
        {
            decimal resp = -1;
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.ObtenerApoderado(apod);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                //throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                //throw;
            }
            return resp;
        }


        [WebMethod(Description = "Obtiene el nombre del banco a través del stored procedure PFSP_NOMBREBANCO.")]
        public string ObtenerNombreBanco(short banco)
        {
            string resp = string.Empty;
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.ObtenerNombreBanco(banco);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                throw;
            }
            return resp;
        }

        [WebMethod(Description = "Obtiene la información de la agencia a través del stored procedure PFSP_NOMBREAGENCIA.")]
        public DTOAgencia ObtenerNombreAgencia(short banco, short agencia)
        {
            DTOAgencia resp = null;
            GestionarMPUNeg negocio = new GestionarMPUNeg();
            try
            {
                resp = negocio.ObtenerNombreAgencia(banco, agencia);
            }
            catch (System.Data.SqlClient.SqlException exsql)
            {
                log.Fatal(exsql.Message + "***" + exsql.StackTrace);
                throw;
            }
            catch (System.Exception ex)
            {
                log.Error(ex.Message + "***" + ex.StackTrace);
                throw;
            }
            return resp;
        }






        #endregion
    }
}