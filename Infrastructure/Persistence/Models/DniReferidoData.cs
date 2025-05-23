using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Infrastructure.Persistence.Models
{
    public class DniReferidoData
    {
        public string? DNI { get; set; }
        public string? NombresCompletos { get; set; }
        public int? IdBaseCliente { get; set; }
        public string? TraidoDe { get; set; }
        public decimal? OfertaMaxima { get; set; }
    }
}