using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ALFINapp.Services;
using ALFINapp.Models;
using ALFINapp.Filters; // Replace with the correct namespace where DBServicesGeneral is defined

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class TipificacionesController : Controller
    {
        
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesTipificaciones _dbServicesTipificaciones;
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        public TipificacionesController(DBServicesGeneral dbServicesGeneral, DBServicesTipificaciones dbServicesTipificaciones, DBServicesDerivacion dBServicesDerivacion)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
            _dBServicesDerivacion = dBServicesDerivacion;
        }
        
        [HttpPost]
        public async Task<IActionResult> GenerarDerivacion(string agenciaComercial, DateTime FechaVisita, string Telefono, int idBase)
        {
            var getClienteBase = await _dbServicesGeneral.GetBaseClienteFunction(idBase);
            if (getClienteBase.data == null || !getClienteBase.IsSuccess)
            {
                return Json(new { success = false, message = getClienteBase.Message });
            }
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return Json(new { success = false, message = "No se consiguio el id de la sesion, inicie sesion nuevamente." });
            }
            var getAsesor = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (getAsesor.IsSuccess == false || getAsesor.Data == null)
            {
                return Json(new { success = false, message = getAsesor.Message });
            }

            var getClienteEnriquecido = await _dbServicesGeneral.GetClienteEnriquecidoFunction(idBase); 

            if (getClienteEnriquecido.data == null || !getClienteEnriquecido.IsSuccess)
            {
                return Json(new { success = false, message = getClienteEnriquecido.Message });
            }

            var enviarDerivacion = new DerivacionesAsesores
            {
                FechaDerivacion = DateTime.Now,
                DniAsesor = getAsesor.Data?.Dni ?? throw new ArgumentNullException("DniAsesor", "DniAsesor cannot be null"),
                DniCliente = getClienteBase.data?.Dni ?? throw new ArgumentNullException("DniCliente", "DniCliente cannot be null"),
                IdCliente = getClienteEnriquecido.data.IdCliente,
                NombreCliente = getClienteBase.data.XNombre + " " + getClienteBase.data.XAppaterno + " " + getClienteBase.data.XApmaterno,
                TelefonoCliente = Telefono,
                NombreAgencia = agenciaComercial,
                FueProcesado = false,
                EstadoDerivacion = "DERIVACION PENDIENTE"
            };
            var enviarFomularioAsignacion = await _dBServicesDerivacion.GenerarDerivacion
                (enviarDerivacion.FechaDerivacion, 
                enviarDerivacion.NombreAgencia, 
                enviarDerivacion.DniAsesor, 
                enviarDerivacion.TelefonoCliente, 
                enviarDerivacion.DniCliente, 
                enviarDerivacion.NombreCliente);
            if (enviarFomularioAsignacion.IsSuccess == false)
            {
                return Json(new { success = false, message = enviarFomularioAsignacion.Message });
            }
            var derivacionEnviada = await _dBServicesDerivacion.VerificarDerivacionEnviada(enviarDerivacion.DniCliente);
            if (derivacionEnviada.IsSuccess == false)
            {
                return Json(new { success = false, message = derivacionEnviada.Message });
            }
            return Json(new { success = true, message = "La Derivacion se ha enviado correctamente, pero para guardar los cambios debe darle al boton Guardar Tipificaciones" });;
        }
    }
}