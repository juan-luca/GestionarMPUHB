
using System;

namespace WS.MPU.GestionarMPU.Datos.Models
{

  
    
    public class DTOMedioPago
    {
        public decimal Cuil { get; set; }
        public string TipoMedioPago { get; set; }
        public decimal? CbuInicio { get; set; }
        public decimal? CbuFinal { get; set; }
        public short? CBanco { get; set; }
        public short? CAgencia { get; set; }
        public decimal PeInicioPago { get; set; }
        public decimal? Cvu1 { get; set; }
        public decimal? Cvu2 { get; set; }
        public string Alias { get; set; }
        public string IpOrigen { get; set; }
        public decimal? Udai { get; set; }
        public string OpeTramite { get; set; }

        // Este método ayuda a obtener el valor correcto basado en el TipoMedioPago
        public string ObtenerValor()
        {
            switch (TipoMedioPago)
            {
                case "CBU":
                    return $"{CbuInicio}{CbuFinal}";
                case "CVU":
                    return $"{Cvu1}{Cvu2}";
                case "Banco/Agencia":
                    return $"{CBanco}/{CAgencia}";
                case "Alias":
                    return Alias;
                default:
                    return null;
            }
        }
    }

    public class DTOMedioPagoDisponible
    {
        public decimal Cuil { get; set; }
        public decimal PeEmision { get; set; }
        public decimal PeLiquidado { get; set; }
        public short CBanco { get; set; }
        public short CAgencia { get; set; }
        public short CSistema { get; set; }
        public string MRetenido { get; set; }
        public string MPago { get; set; }
        public short CTipoLiq { get; set; }
        public decimal? Cbu1 { get; set; }
        public decimal? Cbu2 { get; set; }
        public string NombreBanco { get; set; }
        public string NombreAgencia { get; set; }
    }



    public class DTOMedioPagoVigente
    {
        public long IdModo { get; set; }
        public decimal Cuil { get; set; }
        public int CModoPago { get; set; }
        public short? CBco { get; set; }
        public short? CAge { get; set; }
        public string Cbu1 { get; set; }
        public string Cbu2 { get; set; }
        public string Cvu1 { get; set; }
        public string Cvu2 { get; set; }
        public string Alias { get; set; }
        public int PeDesde { get; set; }
        public int? PeHasta { get; set; }
        public short CEstado { get; set; }
        public DateTime TimeEstado { get; set; }
        public long IdTramite { get; set; }
        public DateTime FAlta { get; set; }
        public string DAge { get; set; }
        public string IpOrigen { get; set; }
        public int? Udai { get; set; }
        public string OpeTramite { get; set; }
        public DateTime TimeTramite { get; set; }
        public string DBco { get; set; }
        public int? CodigoPostal { get; set; }
        public string DomCalle { get; set; }
    }




}
