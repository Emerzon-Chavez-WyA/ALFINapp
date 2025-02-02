using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Models
{
    public class ClientesReferidos
    {
        [Key]
        [Column("id_referido")]
        public int IdReferido { get; set; }
        [Column("id_base_cliente_a365")]
        public int? IdBaseClienteA365 { get; set; }
        [Column("id_base_cliente_banco")]
        public int? IdBaseClienteBanco { get; set; }
        [Column("id_supervisor_referido")]
        public int? IdSupervisorReferido { get; set; }
        [Column("nombre_completo_asesor")]
        public string? NombreCompletoAsesor { get; set; }
        [Column("nombre_completo_cliente")]
        public string? NombreCompletoCliente { get; set; }
        [Column("dni_asesor")]
        public string? DniAsesor { get; set; }
        [Column("dni_cliente")]
        public string? DniCliente { get; set; }
        [Column("fecha_referido")]
        public DateTime? FechaReferido { get; set; }
        [Column("traido_de")]
        public string? TraidoDe { get; set; }
        [Column("fue_procesado")]
        public bool? FueProcesado { get; set; }
    }
}