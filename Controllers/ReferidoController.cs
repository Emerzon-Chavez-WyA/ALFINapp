using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    public class ReferidoController : Controller
    {
        public DBServicesGeneral _dbServicesGeneral;
        public DBServicesReferido _dbServicesReferido;
        public ReferidoController(DBServicesGeneral dbServicesGeneral, DBServicesReferido dbServicesReferido)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;

        }
        [HttpGet]
        public IActionResult Referido()
        {
            return View("Referido");
        }
        [HttpGet]
        public async Task<IActionResult> BuscarDNIReferido(string dniBusqueda)
        {
            var getDNIReferido = await _dbServicesReferido.GetDataFromDNI(dniBusqueda);
            var getBases = await _dbServicesGeneral.GetUAgenciasConNumeros();

            if (getDNIReferido.IsSuccess == false)
            {
                return Json(new { success = false, message = getDNIReferido.Message });
            }

            ViewData["Agencias"] = getBases.data;

            return PartialView("DetalleDNIReferido", getDNIReferido.data);
        }

        public async Task<IActionResult> ReferirCliente(string dniReferir,
                                                                        string fuenteBase,
                                                                        string nombresUsuario,
                                                                        string apellidosUsuario,
                                                                        string nombrescliente,
                                                                        string dniUsuario,
                                                                        string telefono,
                                                                        string agencia,
                                                                        DateTime fechaVisita,
                                                                        string celular,
                                                                        string correo,
                                                                        string cci,
                                                                        string departamento,
                                                                        string ubigeo,
                                                                        string banco)
        {
            var getReferido = await _dbServicesReferido.GetDataParaReferir(dniReferir);
            if (getReferido.IsSuccess == false || getReferido.Data == null)
            {
                return Json(new { success = false, message = getReferido.Message });
            }

            var mandarReferido = await _dbServicesReferido.GuardarClienteReferido(dniReferir, 
                fuenteBase, 
                nombresUsuario, 
                apellidosUsuario, 
                dniUsuario, 
                telefono, 
                agencia, 
                fechaVisita, 
                nombrescliente, 
                getReferido.Data?.OfertaMax ?? 0,
                celular,
                correo,
                cci,
                departamento,
                ubigeo,
                banco);

            if (!mandarReferido.Item1) // Accede al primer valor de la tupla (bool IsSuccess)
            {
                return Json(new { success = false, message = mandarReferido.Item2 }); // Segundo valor de la tupla (string Message)
            }
            var mensaje = $@"
            <h2>REFERIDOS</h2>
            <table>
                <tr>
                    <td>CANAL TELECAMPO</td>
                    <td>A365</td>
                </tr>
                <tr>
                    <td>CODIGO EJECUTIVO</td>
                    <td>{dniUsuario}</td>
                </tr>
                <tr>
                    <td>CDV ALFINBANCO</td>
                    <td>{nombresUsuario} {apellidosUsuario}</td>
                </tr>
                <tr>
                    <td>DNI CLIENTE</td>
                    <td>{dniReferir}</td>
                </tr>
                <tr>
                    <td>NOMBRE CLIENTE</td>
                    <td>{nombrescliente}</td>
                </tr>
                <tr>
                    <td>MONTO SOLICITADO</td>
                    <td>{getReferido.Data?.OfertaMax ?? "No especificada"}</td>
                </tr>
                <tr>
                    <td>CELULAR</td>
                    <td>{telefono}</td>
                </tr>
                <tr>
                    <td>AGENCIA</td>
                    <td>{agencia}</td>
                </tr>
                <tr>
                    <td>FECHA DE VISITA A AGENCIA</td>
                    <td>{fechaVisita}</td>
                </tr>
                <tr>
                    <td>HORA DE VISITA A AGENCIA</td>
                    <td>NO ESPECIFICADO</td>
                </tr>
            </table>";

            var enviarCorreo = await _dbServicesReferido.EnviarCorreoReferido(
                                                                        "rperaltam@grupoa365.com.pe",
                                                                        mensaje,
                                                                        $"REFERIDOS - SISTEMA ALFIN {DateTime.Now.ToString("dd/MM/yyyy")}");

            if (enviarCorreo.IsSuccess == false)
            {
                return Json(new { success = false, message = enviarCorreo.Message });
            }

            return Json(new { success = true, message = getReferido.Message + ". " + enviarCorreo.Message });
        }

        public IActionResult Consulta()
        {
            return View("Consulta");
        }

        public async Task<IActionResult> BuscarReferidosDeDNI(string DNI)
        {
            var getDNIConsulta = await _dbServicesReferido.GetReferidosDelDNI(DNI);

            if (getDNIConsulta.IsSuccess == false)
            {
                return Json(new { success = false, message = getDNIConsulta.Message });
            }

            return PartialView("_BuscarReferidoDeDNI", getDNIConsulta.Data);
        }
    }
}