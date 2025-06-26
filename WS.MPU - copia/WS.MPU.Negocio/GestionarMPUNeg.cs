using WS.MPUHB.Datos;
using WS.MPUHB.Datos.Models;

namespace WS.MPUHB.Negocio
{
    public class GestionarMPUHBNeg
    {
        private readonly GestionarMPUHBDatos datos = new GestionarMPUHBDatos();

        public Resultado InsertarMPUdesdeHB(decimal cuil, decimal cuit, short banco, short agencia, string ip)
        {
            return datos.InsertarMPUdesdeHB(cuil, cuit, banco, agencia, ip);
        }
    }
}
