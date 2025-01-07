using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    public class AsesorController : Controller
    {
        private readonly MDbContext _context;

        public AsesorController(MDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ActivarAsesor(string DNI, int idUsuario)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                //Verificar Datos Enviados
                if (string.IsNullOrEmpty(DNI))
                {
                    return Json(new { success = false, message = "Debe ingresar el DNI del asesor" });
                }
                if (idUsuario == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor" });
                }

                var asesorParaActivar = _context.usuarios.FirstOrDefault(u => u.Dni == DNI && u.IdUsuario == idUsuario);
                if (asesorParaActivar == null)
                {
                    return Json(new { success = false, message = "No se encontró el asesor" });
                }
                if (asesorParaActivar.Estado == "ACTIVO")
                {
                    return Json(new { success = false, message = "El asesor ya se encuentra activo" });
                }
                asesorParaActivar.Estado = "ACTIVO";
                _context.SaveChanges();
                return Json(new { success = true, message = "Asesor activado correctamente" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un error al activar el asesor" });
                throw;
            }
        }

        [HttpPost]
        public IActionResult DesactivarAsesor(string DNI, int idUsuario)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                //Verificar Datos Enviados
                if (string.IsNullOrEmpty(DNI))
                {
                    return Json(new { success = false, message = "Debe ingresar el DNI del asesor" });
                }
                if (idUsuario == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor" });
                }

                var asesorParaDesactivar = _context.usuarios.FirstOrDefault(u => u.Dni == DNI && u.IdUsuario == idUsuario);

                if (asesorParaDesactivar == null)
                {
                    return Json(new { success = false, message = "No se encontró el asesor" });
                }
                if (asesorParaDesactivar.Estado == "INACTIVO")
                {
                    return Json(new { success = false, message = "El asesor ya se encuentra inactivo" });
                }

                asesorParaDesactivar.Estado = "INACTIVO";
                _context.SaveChanges();
                return Json(new { success = true, message = "Asesor desactivado correctamente" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un error al desactivar el asesor" });
                throw;
            }
        }

        [HttpPost]
        public IActionResult GuardarCambiosAsignaciones(List<AsignacionesDTO> AsignacionesEnviadas, int AsesorCambioID)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            try
            {
                if (AsignacionesEnviadas == null)
                {
                    return Json(new { success = false, message = "Llene al menos un campo de la seccion Modificar Clientes Asignados" });
                }

                if (AsesorCambioID == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor al que se asignarán los clientes" });
                }

                bool cambiosRealizados = false;
                foreach (var asignacion in AsignacionesEnviadas)
                {
                    if (asignacion.Modificaciones == 0)
                    {
                        continue;
                    }
                    var clientesAModificar = _context.clientes_asignados
                        .Where(ca => ca.IdUsuarioV == asignacion.IdUsuario
                                && ca.TipificacionMayorPeso == null
                                && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month)
                        .Take(asignacion.Modificaciones)
                        .ToList();
                    if (clientesAModificar.Count < asignacion.Modificaciones)
                    {
                        return Json(new { success = false, message = $"No hay suficientes clientes disponibles para asignar. Solo hay {clientesAModificar.Count} clientes disponibles. Tal asignacion ha sido Obviada" });
                    }

                    foreach (var cliente in clientesAModificar)
                    {
                        cliente.IdUsuarioV = AsesorCambioID;
                        cliente.FechaAsignacionVendedor = DateTime.Now;
                    }
                    cambiosRealizados = true;

                    _context.SaveChanges();
                }
                if (!cambiosRealizados)
                {
                    return Json(new { success = false, message = "No se realizaron modificaciones. No hay clientes para asignar." });
                }

                return Json(new { success = true, message = "Se han modificado los clientes asignados al asesor" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Llene al menos un campo de la seccion Modificar Clientes Asignados" });
                throw;
            }
        }

        [HttpPost]
        public IActionResult AgregarNuevoAsesor([FromBody] Usuario nuevoUsuario)
        {
            if (nuevoUsuario == null)
            {
                return Json(new { success = false, message = "El usuario no puede ser nulo." });
            }
            try
            {
                var usuarioExistente = _context.usuarios.FirstOrDefault(u => u.Dni == nuevoUsuario.Dni);
                if (usuarioExistente != null)
                {
                    return Json(new { success = false, message = "El DNI ya está registrado en la base de datos." });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Los datos enviados no son válidos" });
                }

                if (string.IsNullOrEmpty(nuevoUsuario.Dni) || nuevoUsuario.Dni.Length < 8)
                {
                    return Json(new { success = false, message = "El DNI debe contener al menos 8 dígitos." });
                }

                int? idsupervisoractual = HttpContext.Session.GetInt32("UsuarioId");

                if (idsupervisoractual == null)
                {
                    return Json(new { success = false, message = "El ID Supervisor a asignar automaticamente es invalido. Comunicarse con Soporte Tecnico." });
                }
                else
                {
                    var supervisorData = _context.usuarios.AsNoTracking().FirstOrDefault(u => u.IdUsuario == idsupervisoractual);
                    if (supervisorData == null)
                    {
                        return Json(new { success = false, message = "No se encontró el supervisor actual." });
                    }
                    nuevoUsuario.IDUSUARIOSUP = idsupervisoractual ?? 0;
                    nuevoUsuario.RESPONSABLESUP = supervisorData.NombresCompletos;
                }

                nuevoUsuario.NombresCompletos = nuevoUsuario.NombresCompletos?.ToUpper();
                nuevoUsuario.Rol = nuevoUsuario.Rol?.ToUpper();
                nuevoUsuario.Departamento = nuevoUsuario.Departamento?.ToUpper();
                nuevoUsuario.Provincia = nuevoUsuario.Provincia?.ToUpper();
                nuevoUsuario.Distrito = nuevoUsuario.Distrito?.ToUpper();
                nuevoUsuario.REGION = nuevoUsuario.REGION?.ToUpper();

                nuevoUsuario.FechaRegistro = DateTime.Now;
                nuevoUsuario.Estado = "ACTIVO";
                _context.usuarios.Add(nuevoUsuario);
                _context.SaveChanges();
                return Json(new { success = true, message = $"Se ha agregado al nuevo Usuario {nuevoUsuario.NombresCompletos} con el Rol {nuevoUsuario.Rol}" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Ha ocurrido un error inesperado" });
            }
        }

        [HttpGet]
        public IActionResult ObtenerNumDeClientesPorTipificacion (string tipificacionDetalle, int idAsesorBuscar)
        {
            try
            {
                var clientesNumDB = (from ca in _context.clientes_asignados
                                    join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                    where ca.IdUsuarioV == idAsesorBuscar
                                        && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                        && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                    select new
                                    {
                                        IdCliente = ce.IdCliente,
                                        TipificacionesGenerales = new
                                        {
                                            UltimaTip1 = ce.UltimaTipificacionTelefono1,
                                            UltimaTip2 = ce.UltimaTipificacionTelefono2,
                                            UltimaTip3 = ce.UltimaTipificacionTelefono3,
                                            UltimaTip4 = ce.UltimaTipificacionTelefono4,
                                            UltimaTip5 = ce.UltimaTipificacionTelefono5
                                        }
                                    }).ToList();

                // Consulta para las tipificaciones de números personales
                var clientesNumPers = (from ca in _context.clientes_asignados
                                        join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                        join ta in _context.telefonos_agregados on ce.IdCliente equals ta.IdCliente
                                        where ca.IdUsuarioV == idAsesorBuscar
                                            && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                            && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                        select new
                                        {
                                            IdCliente = ce.IdCliente,
                                            TipificacionesPersonales = ta.UltimaTipificacion 
                                        }).ToList();

                var NumClientesAsignados = (from cndb in clientesNumDB
                                        join cnp in clientesNumPers on cndb.IdCliente equals cnp.IdCliente into cnpGroup
                                        from cnp in cnpGroup.DefaultIfEmpty()  // Left join
                                        where cndb.TipificacionesGenerales.UltimaTip1 == tipificacionDetalle
                                            || cndb.TipificacionesGenerales.UltimaTip2 == tipificacionDetalle
                                            || cndb.TipificacionesGenerales.UltimaTip3 == tipificacionDetalle
                                            || cndb.TipificacionesGenerales.UltimaTip4 == tipificacionDetalle
                                            || cndb.TipificacionesGenerales.UltimaTip5 == tipificacionDetalle
                                            || (cnp != null && cnp.TipificacionesPersonales == tipificacionDetalle) // Condición para verificar null en cnp
                                        select new { cndb, cnp })
                                        .ToList();
                return Json(new {success = true, numClientes = NumClientesAsignados.Count()});
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Ha ocurrido un error inesperado" });
            }
        }

        [HttpPost]
        public IActionResult ModificarAsignacionesPorTipificaciones()
        {
            try
            {
                return Json(new { success = false, message = "Ha ocurrido un error inesperado" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un error inesperado" });
                throw;
            }
        }
    }
}