using System;
using System.Collections.Generic;
using WS.MPU.GestionarMPU.Datos;
using WS.MPU.GestionarMPU.Datos.Models;
using System.Diagnostics;

namespace WS.MPU.Negocio
{
    public class GestionarMPUNeg
    {
        #region Bancos
        public List<DTOBanco> ConsultarBancosPorCercania(string codigoPostal)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            List<DTOBanco> rta = new List<DTOBanco>();
            try
            {
                rta = mpuDatos.ConsultarBancosPorCercania(codigoPostal);
            }
            catch (System.Exception)
            {
                throw;
            }
            return rta;
        }

        // Método para obtener el nombre del banco a través del stored procedure PFSP_NOMBREBANCO
        public string ObtenerNombreBanco(short banco)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            string rta = string.Empty;
            try
            {
                rta = mpuDatos.ObtenerNombreBanco(banco);
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error al obtener el nombre del banco", ex);
            }
            return rta;
        }

        // Método para obtener la información de la agencia a través del stored procedure PFSP_NOMBREAGENCIA
        public DTOAgencia ObtenerNombreAgencia(short banco, short agencia)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            DTOAgencia rta = null;
            try
            {
                rta = mpuDatos.ObtenerNombreAgencia(banco, agencia);
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error al obtener la información de la agencia", ex);
            }
            return rta;
        }

        #endregion


        #region MedioPago

        public DTOMedioPagoVigente ObtenerMedioPagoVigente(string cuil)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            try
            {
                return mpuDatos.ObtenerMedioPagoVigente(cuil);
            }
            catch (Exception ex)
            {
                // Loguear el error
                throw new Exception("Error al obtener el medio de pago vigente", ex);
            }
        }


        public int GuardarMedioPagoUnico(DTOMedioPago medioPago)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            int rta = -1;
            try
            {
                rta = mpuDatos.GuardarMedioPagoUnico(medioPago);
            }
            
            catch (System.Exception ex)
            {
                throw ex;
            }
            return rta;
        }

        public decimal ObtenerApoderado(DTOApoderado apod)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            decimal rta = -1;
            try
            {
                rta = mpuDatos.ObtenerApoderado(apod);
            }

            catch (System.Exception)
            {
                throw;
            }
            return rta;
        }
        public List<DTOMedioPagoDisponible> ListarMPDisponibles(decimal cuil)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            List<DTOMedioPagoDisponible> rta = new List<DTOMedioPagoDisponible>();
            try
            {
                rta = mpuDatos.ListarMPDisponibles(cuil);
            }
            catch (System.Exception)
            {
                throw;
            }
            return rta;
        }

        public List<DTOBanco> TraerBancosFisicos(short codigoPostal)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            List<DTOBanco> rta = new List<DTOBanco>();
            try
            {
                rta = mpuDatos.TraerBancosFisicos(codigoPostal);
            }
            catch (System.Exception)
            {
                throw;
            }
            return rta;
        }

        public List<DTOBanco> ListarBancosVirtuales()
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            List<DTOBanco> rta = new List<DTOBanco>();
            try
            {
                rta = mpuDatos.ListarBancosVirtuales();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return rta;
        }

        public List<DTOBanco> ListarBilleterasVirtuales()
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            List<DTOBanco> rta = new List<DTOBanco>();
            try
            {
                rta = mpuDatos.ListarBilleterasVirtuales();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return rta;
        }
        public List<DTOBanco> ListarBancosCorreo(short codigoPostal)
        {
            GestionarMPUDatos mpuDatos = new GestionarMPUDatos();
            List<DTOBanco> rta = new List<DTOBanco>();
            try
            {
                rta = mpuDatos.TraerBancosCorreo(codigoPostal);
            }
            catch (System.Exception)
            {
                throw;
            }
            return rta;
        }



        #endregion

        #region Validaciones
        public bool ValidarCBUCVU(string tipo, string valor, string cuil)
        {
            // Aquí iría la lógica para invocar el servicio web de COELSA
            // Por ahora, retornamos true como ejemplo
            return true;
        }

        public bool ValidarBeneficiario(string cuil)
        {
            // Aquí iría la lógica para validar si el beneficiario puede usar el servicio
            // (menores de 13 años, residentes en exterior, etc.)
            // Por ahora, retornamos true como ejemplo
            return true;
        }
        #endregion

       
    }
   
}