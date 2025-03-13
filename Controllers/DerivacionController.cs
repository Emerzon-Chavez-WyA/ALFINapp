using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Filters;
using ALFINapp.Models;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class DerivacionController : Controller
    {
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        private readonly DBServicesConsultasSupervisores _dBServicesConsultasSupervisores;
        private readonly DBServicesConsultasAdministrador _dBServicesConsultasAdministrador;
        private readonly DBServicesGeneral _dBServicesGeneral;
        public DerivacionController(
            DBServicesDerivacion dBServicesDerivacion,
            DBServicesConsultasSupervisores dBServicesConsultasSupervisores,
            DBServicesGeneral dBServicesGeneral,
            DBServicesConsultasAdministrador dBServicesConsultasAdministrador)
        {
            _dBServicesDerivacion = dBServicesDerivacion;
            _dBServicesConsultasSupervisores = dBServicesConsultasSupervisores;
            _dBServicesGeneral = dBServicesGeneral;
            _dBServicesConsultasAdministrador = dBServicesConsultasAdministrador;
        }

        [HttpGet]
        [PermissionAuthorization("Derivacion", "Derivacion")]
        public async Task<IActionResult> Derivacion()
        {
            var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
            if (UsuarioIdSupervisor == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }
            var rolUsuario = HttpContext.Session.GetInt32("RolUser");
            if (rolUsuario == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }
            ViewData["RolUsuario"] = rolUsuario;
            var getdatosUsuario = await _dBServicesGeneral.GetUserInformation(UsuarioIdSupervisor.Value);
            if (!getdatosUsuario.IsSuccess || getdatosUsuario.Data == null)
            {
                TempData["MessageError"] = getdatosUsuario.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            ViewData["DniUsuario"] = getdatosUsuario.Data.Dni;
            if (rolUsuario == 2)
            {
                var getAsesoresAsignados = await _dBServicesConsultasSupervisores.GetAsesorsFromSupervisor(UsuarioIdSupervisor.Value);
                if (!getAsesoresAsignados.IsSuccess || getAsesoresAsignados.Data == null)
                {
                    TempData["MessageError"] = getAsesoresAsignados.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(getAsesoresAsignados.Data);
                if (!getClientesDerivadosGenerales.IsSuccess)
                {
                    TempData["MessageError"] = getClientesDerivadosGenerales.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                if (getClientesDerivadosGenerales.Data == null)
                {
                    ViewData["Asesores"] = getAsesoresAsignados.Data;
                    return View("Derivacion", getClientesDatosDTO);
                }

                var getInformation = await _dBServicesDerivacion.GetDerivacionInformationAll(getClientesDerivadosGenerales.Data);
                if (!getInformation.IsSuccess)
                {
                    TempData["MessageError"] = getInformation.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }

                if (getInformation.Data == null)
                {
                    TempData["MessageError"] = "Ocurrio un error al obtener la información de las derivaciones.";
                    return RedirectToAction("Redireccionar", "Error");
                }

                getInformation.Data = getInformation.Data.OrderByDescending(a => a.FechaEnvio).ToList();
                ViewData["Asesores"] = getAsesoresAsignados.Data;
                return View("Derivacion", getInformation.Data);
            }
            if (rolUsuario == 3)
            {
                var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (idUsuario == null)
                {
                    TempData["MessageError"] = "No se ha iniciado sesión.";
                    return RedirectToAction("Index", "Home");
                }
                var getDniAsesor = await _dBServicesGeneral.GetUserInformation(idUsuario.Value);
                if (!getDniAsesor.IsSuccess || getDniAsesor.Data == null)
                {
                    TempData["MessageError"] = getDniAsesor.Message;
                    return RedirectToAction("Index", "Home");
                }
                var DniAsesor = getDniAsesor.Data.Dni;
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(new List<Usuario> { new Usuario { Dni = DniAsesor } });
                if (!getClientesDerivadosGenerales.IsSuccess)
                {
                    TempData["MessageError"] = getClientesDerivadosGenerales.Message;
                    return RedirectToAction("Index", "Home");
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                if (getClientesDerivadosGenerales.Data == null)
                {
                    ViewData["Asesores"] = new List<Usuario> { new Usuario { Dni = DniAsesor } };
                    return View("Derivacion", getClientesDatosDTO);
                }
                var getInformation = await _dBServicesDerivacion.GetDerivacionInformationAll(getClientesDerivadosGenerales.Data);
                if (!getInformation.IsSuccess)
                {
                    TempData["MessageError"] = getInformation.Message;
                    return RedirectToAction("Index", "Home");
                }
                if (getInformation.Data == null)
                {
                    TempData["MessageError"] = "Ocurrio un error al obtener la información de las derivaciones.";
                    return RedirectToAction("Index", "Home");
                }
                getInformation.Data = getInformation.Data.OrderByDescending(a => a.FechaEnvio).ToList();
                return View("Derivacion", getInformation.Data);
            }
            if (rolUsuario == 4 || rolUsuario == 1)
            {
                var getAsesoresAsignados = await _dBServicesConsultasAdministrador.ConseguirTodosLosUsuarios();
                if (!getAsesoresAsignados.IsSuccess || getAsesoresAsignados.Data == null)
                {
                    TempData["MessageError"] = getAsesoresAsignados.Message;
                    return RedirectToAction("Index", "Home");
                }
                var todosAsesores = getAsesoresAsignados.Data.Where(a => a.IdRol == 3).ToList();
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(todosAsesores);
                if (!getClientesDerivadosGenerales.IsSuccess || getClientesDerivadosGenerales.Data == null)
                {
                    TempData["MessageError"] = getClientesDerivadosGenerales.Message;
                    return RedirectToAction("Index", "Home");
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                if (getClientesDerivadosGenerales.Data == null)
                {
                    ViewData["Asesores"] = todosAsesores;
                    return View("Derivacion", getClientesDatosDTO);
                }
                var getInformation = await _dBServicesDerivacion.GetDerivacionInformationAll(getClientesDerivadosGenerales.Data);
                if (!getInformation.IsSuccess)
                {
                    TempData["MessageError"] = getInformation.Message;
                    return RedirectToAction("Index", "Home");
                }
                if (getInformation.Data == null)
                {
                    TempData["MessageError"] = "Ocurrio un error al obtener la información de las derivaciones.";
                    return RedirectToAction("Index", "Home");
                }
                getInformation.Data = getInformation.Data.OrderByDescending(a => a.FechaEnvio).ToList();
                ViewData["Asesores"] = todosAsesores;
                return View("Derivacion", getInformation.Data);
            }
            else
            {
                TempData["MessageError"] = "No tiene permisos para acceder a esta vista.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerarDerivacion(DateTime FechaVisitaDerivacion,
                                                                string AgenciaDerivacion,
                                                                string AsesorDerivacion,
                                                                string DNIAsesorDerivacion,
                                                                string TelefonoDerivacion,
                                                                string DNIClienteDerivacion,
                                                                string NombreClienteDerivacion)
        {
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }
            try
            {
                var usuarioinfo = await _dBServicesGeneral.GetUserInformation(idUsuario.Value);
                if (!usuarioinfo.IsSuccess || usuarioinfo.Data == null)
                {
                    return Json(new { success = false, message = usuarioinfo.Message });
                }

                var enviarDerivacion = await _dBServicesDerivacion.GenerarDerivacion(
                    FechaVisitaDerivacion,
                    TelefonoDerivacion,
                    DNIAsesorDerivacion,
                    TelefonoDerivacion,
                    DNIClienteDerivacion,
                    NombreClienteDerivacion);
                if (enviarDerivacion.IsSuccess)
                {
                    return Json(new { success = false, message = enviarDerivacion.Message });
                }
                var verificarDerivacion = await _dBServicesDerivacion.VerificarDerivacionEnviada(DNIClienteDerivacion);
                if (verificarDerivacion.IsSuccess)
                {
                    return Json(new { success = false, message = verificarDerivacion.Message });
                }

                var getDerivacion = await _dBServicesDerivacion.GetDerivacionXDNI(DNIClienteDerivacion);
                if (!getDerivacion.IsSuccess || getDerivacion.data == null)
                {
                    return Json(new { success = false, message = getDerivacion.message });
                }

                var mensaje = $@"
                <div>
                    <div style=""font-size: 12px;"">
                        <span>
                            Estimados <br> Buen día
                        </span>
                    </div>

                    <div style=""margin-top: 20px;"">
                        Desde el <strong>CANAL DE A365</strong> originamos y compartimos un prospecto de cliente <br>
                        interesado en la toma de un crédito en efectivo.
                    </div>

                    <div style=""margin-top: 30px;"">
                        <span style=""background-color: yellow; padding: 10px; border-radius: 5px; 
                                    font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; font-size: 24px;"">
                            <strong>Información del Prospecto del Cliente</strong>
                        </span>
                    </div>

                    <div style=""margin-top: 40px; font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; font-size: 16px;"">
                        <table>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>CANAL TELECAMPO:</strong></td>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">A365</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>CÓDIGO DEL EJECUTIVO:</strong></td>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">{usuarioinfo.Data.Dni}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>CDV ALFIN BANCO:</strong></td>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">{usuarioinfo.Data.NombresCompletos}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px;""><strong>DNI Cliente:</strong></td>
                                <td style=""padding: 10px;"">{DNIClienteDerivacion}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px;""><strong>Nombre Cliente:</strong></td>
                                <td style=""padding: 10px;"">{NombreClienteDerivacion}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px;""><strong>Monto Solicitado (S/.):</strong></td>
                                <td style=""padding: 10px;"">{getDerivacion.data.Oferta}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px;""><strong>Celular:</strong></td>
                                <td style=""padding: 10px;"">{TelefonoDerivacion}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Agencia de Atención:</strong></td>
                                <td style=""padding: 10px; background-color: yellow;"">{TelefonoDerivacion}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Fecha de Visita a Agencia:</strong></td>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">{FechaVisitaDerivacion:yyyy-MM-dd}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);""><strong>Hora de Visita a Agencia:</strong></td>
                                <td style=""padding: 10px; background-color: rgb(226, 226, 226);"">HORARIO DE AGENCIA</td>
                            </tr>
                        </table>
                    </div>
                </div>";
                var enviarEmailDerivacion = await _dBServicesDerivacion.EnviarEmailDeDerivacion(AgenciaDerivacion, mensaje, $"Asunto: Fwd: A365 FFVV CAMPO CLIENTE DNI: {DNIClienteDerivacion} / NOMBRE: {NombreClienteDerivacion}");
                return Json(new { success = true, message = enviarDerivacion.Message + " " + enviarEmailDerivacion.message });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerDerivacionesXAsesor(string DniAsesor)
        {
            try
            {
                var DniAsesorGet = new Usuario
                {
                    Dni = DniAsesor
                };

                var enviarDni = new List<Usuario>
                {
                    DniAsesorGet
                };

                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetClientesDerivadosGenerales(enviarDni);
                if (!getClientesDerivadosGenerales.IsSuccess || getClientesDerivadosGenerales.Data == null)
                {
                    return Json(new { success = false, message = getClientesDerivadosGenerales.Message });
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                foreach (var item in getClientesDerivadosGenerales.Data)
                {
                    var newItem = new GestionDetalleDTO
                    {
                        IdAsignacion = item.IdDerivacion,
                        DocCliente = item.DniCliente ?? string.Empty,
                        Canal = "A365",
                        FechaEnvio = item.FechaDerivacion,
                        FechaGestion = item.FechaDerivacion,
                        Telefono = item.TelefonoCliente,
                        OrigenTelefono = "A365",
                        CodTip = 2,
                        DocAsesor = item.DniAsesor,
                        FueProcesadaLaDerivacion = item.FueProcesado,
                        //DATOS QUE DEBEN SER BUSCADOS POR EL ID ASIGNAICON
                        CodCampaña = item.NombreAgencia,
                        Oferta = 0,
                        CodCanal = "DESCONOCIDO",
                        Origen = "A365",
                        ArchivoOrigen = "SISTEMA INTERNO",
                        FechaCarga = item.FechaDerivacion,
                        IdDerivacion = item.IdDerivacion,
                        IdSupervisor = 0,
                        Supervisor = "DESCONOCIDO",
                        IdDesembolso = 0,
                        TraidoDe = "SISTEMA INTERNO",
                        EstadoDerivacion = item.EstadoDerivacion + " - " + item.FueProcesado,
                        TipoDerivacion = "AUTOMATICA"
                    };
                    getClientesDatosDTO.Add(newItem);
                }
                getClientesDatosDTO = getClientesDatosDTO.OrderByDescending(a => a.FechaEnvio).ToList();
                ViewData["Derivaciones"] = getClientesDatosDTO;
                return PartialView("_DerivacionesAsesor");
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> ObtenerDerivacionesGestion(string DniAsesor)
        {
            try
            {
                var enviarDni = new List<Usuario>();
                var Asesor = new Usuario
                {
                    Dni = DniAsesor.ToString()
                };
                enviarDni.Add(Asesor);
                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(enviarDni);
                if (!getClientesDerivadosGenerales.IsSuccess)
                {
                    return Json(new { success = false, message = getClientesDerivadosGenerales.Message });
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                if (getClientesDerivadosGenerales.Data != null)
                {
                    foreach (var item in getClientesDerivadosGenerales.Data)
                    {
                        var newItem = new GestionDetalleDTO
                        {
                            IdFeedback = item.IdFeedback,
                            IdAsignacion = item.IdAsignacion,
                            CodCanal = item.CodCanal,
                            Canal = item.Canal,
                            DocCliente = item.DocCliente,
                            FechaEnvio = item.FechaEnvio,
                            FechaGestion = item.FechaGestion,
                            HoraGestion = item.HoraGestion,
                            Telefono = item.Telefono,
                            OrigenTelefono = item.OrigenTelefono,
                            CodCampaña = item.CodCampaña,
                            CodTip = item.CodTip,
                            Oferta = item.Oferta,
                            DocAsesor = item.DocAsesor,
                            Origen = item.Origen,
                            ArchivoOrigen = item.ArchivoOrigen,
                            FechaCarga = item.FechaCarga,
                            IdDerivacion = item.IdDerivacion,
                            IdSupervisor = item.IdSupervisor,
                            Supervisor = item.Supervisor,
                            IdDesembolso = item.IdDesembolso,
                            TraidoDe = "DETALLES DE GESTION",
                            EstadoDerivacion = "DERIVACION POR SISTEMA",
                            FueProcesadaLaDerivacion = true,
                        };
                        getClientesDatosDTO.Add(newItem);
                    }
                }
                getClientesDatosDTO = getClientesDatosDTO.OrderByDescending(a => a.FechaGestion).ToList();
                return PartialView("_DerivacionesGestion", getClientesDatosDTO);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerDerivacionesGestionSupervisor(string? DniAsesor = null)
        {
            try
            {
                var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
                if (UsuarioIdSupervisor == null)
                {
                    return Json (new { success = false, message = "No se ha iniciado sesión." });
                }
                var enviarDni = new List<Usuario>();
                var Asesor = new Usuario ();
                
                if (DniAsesor == null)
                {
                    var getAsesoresAsignados = await _dBServicesConsultasSupervisores.GetAsesorsFromSupervisor(UsuarioIdSupervisor.Value);
                    if (!getAsesoresAsignados.IsSuccess || getAsesoresAsignados.Data == null)
                    {
                        return Json (new { success = false, message = getAsesoresAsignados.Message });
                    }
                    enviarDni = getAsesoresAsignados.Data;
                }
                else
                {
                    Asesor.Dni = DniAsesor.ToString();
                    enviarDni.Add(Asesor);
                }

                var getClientesDerivadosGenerales = await _dBServicesDerivacion.GetEntradasBSDialXSupervisor(enviarDni);
                if (!getClientesDerivadosGenerales.IsSuccess)
                {
                    return Json(new { success = false, message = getClientesDerivadosGenerales.Message });
                }
                var getClientesDatosDTO = new List<GestionDetalleDTO>();
                if (getClientesDerivadosGenerales.Data != null)
                {
                    foreach (var item in getClientesDerivadosGenerales.Data)
                    {
                        var newItem = new GestionDetalleDTO
                        {
                            IdFeedback = item.IdFeedback,
                            IdAsignacion = item.IdAsignacion,
                            CodCanal = item.CodCanal,
                            Canal = item.Canal,
                            DocCliente = item.DocCliente,
                            FechaEnvio = item.FechaEnvio,
                            FechaGestion = item.FechaGestion,
                            HoraGestion = item.HoraGestion,
                            Telefono = item.Telefono,
                            OrigenTelefono = item.OrigenTelefono,
                            CodCampaña = item.CodCampaña,
                            CodTip = item.CodTip,
                            Oferta = item.Oferta,
                            DocAsesor = item.DocAsesor,
                            Origen = item.Origen,
                            ArchivoOrigen = item.ArchivoOrigen,
                            FechaCarga = item.FechaCarga,
                            IdDerivacion = item.IdDerivacion,
                            IdSupervisor = item.IdSupervisor,
                            Supervisor = item.Supervisor,
                            IdDesembolso = item.IdDesembolso,
                            TraidoDe = "DETALLES DE GESTION",
                            EstadoDerivacion = "DERIVACION POR SISTEMA",
                            FueProcesadaLaDerivacion = true,
                        };
                        getClientesDatosDTO.Add(newItem);
                    }
                }
                getClientesDatosDTO = getClientesDatosDTO.OrderByDescending(a => a.FechaGestion).ToList();
                return PartialView("_DerivacionesGestion", getClientesDatosDTO);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}