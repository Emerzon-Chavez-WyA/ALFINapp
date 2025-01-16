using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;


namespace ALFINapp.Services
{
    public class DBServicesConsultasAsesores
    {
        private readonly MDbContext _context;

        public DBServicesConsultasAsesores(MDbContext context)
        {
            _context = context;
        }
        public async Task<List<DetalleBaseClienteDTO>> DetallesClientesParaVentas(int IdUsuarioVendedor)
        {
            var clientes = await (from bc in _context.base_clientes
                                  join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                  join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                  join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente
                                  where ca.ClienteDesembolso != true
                                        && ca.ClienteRetirado != true
                                        && db.TipoBase == ca.FuenteBase
                                        && ca.IdUsuarioV == IdUsuarioVendedor
                                        && ca.FechaAsignacionVendedor.HasValue
                                        && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                        && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                  group new { db, bc, ca } by db.IdBase into grouped
                                  select new
                                  {
                                      IdBase = grouped.Key,
                                      LatestRecord = grouped.OrderByDescending(x => x.db.FechaCarga).FirstOrDefault()
                                  }).ToListAsync();

            // Mapear los resultados a DTO
            var detallesClientes = clientes.Select(cliente => new DetalleBaseClienteDTO
            {
                Dni = cliente.LatestRecord?.bc.Dni ?? "",
                XAppaterno = cliente.LatestRecord?.bc.XAppaterno ?? "",
                XApmaterno = cliente.LatestRecord?.bc.XApmaterno ?? "",
                XNombre = cliente.LatestRecord?.bc.XNombre ?? "",
                OfertaMax = cliente.LatestRecord?.db.OfertaMax ?? 0,
                Campaña = cliente.LatestRecord?.db.Campaña ?? "",
                IdBase = cliente.IdBase,
                IdAsignacion = cliente.LatestRecord?.ca.IdAsignacion,
                FechaAsignacionVendedor = cliente.LatestRecord?.ca.FechaAsignacionVendedor,
                ComentarioGeneral = cliente.LatestRecord?.ca.ComentarioGeneral,
                TipificacionDeMayorPeso = cliente.LatestRecord?.ca.TipificacionMayorPeso,
                PesoTipificacionMayor = cliente.LatestRecord?.ca.PesoTipificacionMayor,
                FechaTipificacionDeMayorPeso = cliente.LatestRecord?.ca.FechaTipificacionMayorPeso,
            }).ToList();

            return detallesClientes;
        }

        public async Task<object> ObtenerDetallesClientes(int IdAsignacion)
        {
            try
            {
                var clienteDatos = await (from bc in _context.base_clientes
                                          join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                          join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente
                                          where ca.IdAsignacion == IdAsignacion
                                          select new
                                          {
                                              Dni = bc.Dni,
                                              XNombre = bc.XNombre,
                                              XAppaterno = bc.XAppaterno,
                                              XApmaterno = bc.XApmaterno,
                                              IdCliente = ce.IdCliente
                                          }).FirstOrDefaultAsync();

                return clienteDatos;
            }
            catch (Exception ex)
            {
                // Maneja la excepción según corresponda
                throw;
            }
        }


        //ACA ESTAN LAS INSERCIONES A LA DB
        public async Task<bool> AgregarDerivacionParaFormularios(DerivacionesAsesores nuevaDerivacion)
        {
            try
            {
                // Add the new entry to the "derivaciones_asesor" table
                _context.derivaciones_asesores.Add(nuevaDerivacion);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

    }

}