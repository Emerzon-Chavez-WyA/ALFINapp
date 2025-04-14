using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Domain.Entities;

namespace ALFINapp.API.Models
{
    public class ViewGestionLeads
    {
        public List<Cliente> ClientesA365 { get; set; } = new List<Cliente>();
        public Vendedor Vendedor { get; set; } = new Vendedor();
        public Supervisor Supervisor { get; set; } = new Supervisor();
        public List<Cliente> ClientesAlfin { get; set; } = new List<Cliente>();
        public int clientesPendientes { get; set; }
        public int clientesTipificados { get; set; }
        public int clientesTotal { get; set; }
        public List<string> destinoBases { get; set; } = new List<string>();
    }
}