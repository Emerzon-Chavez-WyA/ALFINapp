using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.API.Filters;
using ALFINapp.API.Models;
using ALFINapp.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ReportesController : Controller
    {
        private readonly IUseCaseGetReportesAdministrador _useCaseGetReportesAdministrador;
        private readonly IUseCaseGetReportesAsesor _useCaseGetReportesAsesor;
        private readonly IUseCaseGetReportesSupervisor _useCaseGetReportesSupervisor;
        public ReportesController(
            IUseCaseGetReportesAdministrador useCaseGetReportesAdministrador,
            IUseCaseGetReportesAsesor useCaseGetReportesAsesor,
            IUseCaseGetReportesSupervisor useCaseGetReportesSupervisor)
        {
            _useCaseGetReportesAdministrador = useCaseGetReportesAdministrador;
            _useCaseGetReportesAsesor = useCaseGetReportesAsesor;
            _useCaseGetReportesSupervisor = useCaseGetReportesSupervisor;
        }
        [HttpGet]
        [PermissionAuthorization("Reportes", "Reportes")]
        public async Task<IActionResult> Reportes()
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var reportesAdministrador = await _useCaseGetReportesAdministrador.Execute();
            if (!reportesAdministrador.IsSuccess)
            {
                TempData["MessageError"] = reportesAdministrador.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            var reportes = new ViewReportesGeneral { };
            return View("Reportes", reportesAdministrador.Data);
        }
        [HttpGet]
        public async Task<IActionResult> AsesorReportes(int idAsesor)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _useCaseGetReportesAsesor.Execute(idAsesor);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesAsesor", reportesAdministrador.Data);
        }
        [HttpGet]
        public async Task<IActionResult> SupervisorReportes(int idSupervisor)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _useCaseGetReportesSupervisor.Execute(idSupervisor);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesSupervisor", reportesAdministrador.Data);
        }
    }
}