using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class GestionDetalleDTO
    {
        public int IdFeedback { get; set; }
        public int? IdAsignacion { get; set; }
        public string? CodCanal { get; set; }
        public string? Canal { get; set; }
        public string? DocCliente { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaGestion { get; set; }
        public TimeSpan? HoraGestion { get; set; }
        public string? Telefono { get; set; }
        public string? OrigenTelefono { get; set; }
        public string? CodCampaña { get; set; }
        public int CodTip { get; set; }
        public decimal Oferta { get; set; }
        public string? DocAsesor { get; set; }
        public string? Origen { get; set; }
        public string? ArchivoOrigen { get; set; }
        public DateTime? FechaCarga { get; set; }
        public int? IdDerivacion { get; set; }
        public int? IdSupervisor { get; set; }
        public string? Supervisor { get; set; }
        public int? IdDesembolso { get; set; }
        public string? TraidoDe { get; set; }
        public string? EstadoDerivacion { get; set; }
        public string? EstadoDesembolso { get; set; }
        public DateTime? FechaDesembolso { get; set; }
        public string? TipoDerivacion { get; set; }
        public DateTime? FechaDerivacion { get; set; }
        public string? Observacion { get; set; }
    }
}