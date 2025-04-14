using ALFINapp.API.Models;
using ALFINapp.Application.DTOs;
using ALFINapp.Application.Interfaces.Leads;
using ALFINapp.Domain.Entities;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Models;

namespace ALFINapp.Application.UseCases.Leads
{
    public class UseCaseGetAsignacionLeads : IUseCaseGetAsignacionLeads
    {
        private readonly IRepositoryVendedor _repositoryVendedor;
        private readonly IRepositorySupervisor _repositorySupervisor;
        public UseCaseGetAsignacionLeads(
            IRepositoryVendedor repositoryVendedor,
            IRepositorySupervisor repositorySupervisor)
        {
            _repositoryVendedor = repositoryVendedor;
            _repositorySupervisor = repositorySupervisor;
        }
        public async Task<(bool IsSuccess, string Message, ViewGestionLeads Data)> Execute(
            int usuarioId,
            int rol,
            int intervaloInicio = 0,
            int intervaloFin = 1)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                var usuario = await _repositoryVendedor.GetVendedor(usuarioId);
                if (usuario == null)
                {
                    return (false, "No se encontró el usuario", new ViewGestionLeads());
                }
                var usuarioDTO = new DetallesUsuarioDTO(usuario);
                if (rol == 0)
                {
                    return (false, "No se encontró el rol del usuario", new ViewGestionLeads());
                }
                var clientes = new List<DetalleBaseClienteDTO>();
                if (rol == 3)
                {
                    clientes = await _repositoryVendedor.GetClientesGeneralPaginadoFromVendedor(usuarioId, intervaloInicio, intervaloFin);
                    if (clientes == null)
                    {
                        return (true, "No se encontraron clientes", new ViewGestionLeads());
                    }
                    var convertView = new ViewGestionLeads
                    {
                        ClientesA365 = clientes.Select(c => c.DtoToCliente()).ToList(),
                        Vendedor = usuarioDTO.ToEntityVendedor(),
                        ClientesAlfin = new List<Cliente>(),
                        clientesPendientes = clientes.Count(dc =>
                            (!dc.FechaTipificacionDeMayorPeso.HasValue ||
                            (dc.FechaTipificacionDeMayorPeso.Value.Year != currentYear &&
                            dc.FechaTipificacionDeMayorPeso.Value.Month != currentMonth))),
                        clientesTipificados = clientes.Count(dc =>
                            dc.FechaTipificacionDeMayorPeso.HasValue &&
                            dc.FechaTipificacionDeMayorPeso.Value.Year == currentYear &&
                            dc.FechaTipificacionDeMayorPeso.Value.Month == currentMonth),
                        clientesTotal = clientes.Count()
                    };
                    return (true, "Se encontraron los siguientes clientes", convertView);
                }
                else if (rol == 2)
                {
                    clientes = await _repositorySupervisor.GetClientesGeneralPaginadoFromSupervisor(usuarioId);
                    if (clientes == null)
                    {
                        return (true, "No se encontraron clientes", new ViewGestionLeads());
                    }
                    var destinos = clientes.Select(c => c.Destino).Distinct().ToList();
                    var convertView = new ViewGestionLeads
                    {
                        ClientesA365 = clientes.Select(c => c.DtoToCliente()).ToList(),
                        Supervisor = usuarioDTO.ToEntitySupervisor(),
                        ClientesAlfin = new List<Cliente>(),
                        clientesPendientes = clientes.Count(dc =>
                            (!dc.FechaTipificacionDeMayorPeso.HasValue ||
                            (dc.FechaTipificacionDeMayorPeso.Value.Year != currentYear &&
                            dc.FechaTipificacionDeMayorPeso.Value.Month != currentMonth))),
                        clientesTipificados = clientes.Count(dc =>
                            dc.FechaTipificacionDeMayorPeso.HasValue &&
                            dc.FechaTipificacionDeMayorPeso.Value.Year == currentYear &&
                            dc.FechaTipificacionDeMayorPeso.Value.Month == currentMonth),
                        clientesTotal = clientes.Count(),
                        destinoBases = destinos
                            .Where(destino => destino != null)
                            .Cast<string>()
                            .ToList()
                    };
                    return (true, "Se encontraron los siguientes clientes", convertView);
                }
                else
                {
                    return (false, "No se encontró el rol del usuario", new ViewGestionLeads());
                }

            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewGestionLeads());
            }
        }
    }
}