using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ReportesController : Controller
    {
        private readonly IUseCaseGetReportes _useCaseGetReportes;
        private readonly IUseCaseGetReportesAsesor _useCaseGetReportesAsesor;
        private readonly IUseCaseGetReportesSupervisor _useCaseGetReportesSupervisor;
        private readonly IUseCaseGetReportesFechas _useCaseGetReportesFechas;
        private readonly IUseCaseGetReportesMetas _useCaseGetReportesMetas;
        public ReportesController(
            IUseCaseGetReportesAsesor useCaseGetReportesAsesor,
            IUseCaseGetReportesSupervisor useCaseGetReportesSupervisor,
            IUseCaseGetReportesFechas useCaseGetReportesFechas,
            IUseCaseGetReportes useCaseGetReportes,
            IUseCaseGetReportesMetas useCaseGetReportesMetas)
        {
            _useCaseGetReportesAsesor = useCaseGetReportesAsesor;
            _useCaseGetReportesSupervisor = useCaseGetReportesSupervisor;
            _useCaseGetReportesFechas = useCaseGetReportesFechas;
            _useCaseGetReportes = useCaseGetReportes;
            _useCaseGetReportesMetas = useCaseGetReportesMetas;
        }
        [HttpGet]
        [PermissionAuthorization("Reportes", "Reportes")]
        public async Task<IActionResult> Reportes(int? anio = null, int? mes = null)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                TempData["MessageError"] = "Rol no valido.";
                return RedirectToAction("Index", "Home");
            }
            ViewData["RolUser"] = rol.Value;
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                TempData["MessageError"] = "Id de usuario no valido.";
                return RedirectToAction("Index", "Home");
            }
            ViewData["UsuarioId"] = idUsuario.Value;
            var reportesAdministrador = await _useCaseGetReportes.Execute(idUsuario.Value, anio, mes);
            if (!reportesAdministrador.IsSuccess)
            {
                TempData["MessageError"] = reportesAdministrador.Message;
                return RedirectToAction("Redireccionar", "Error");
            }
            return View("Reportes", reportesAdministrador.Data);
        }
        
        [HttpGet]
        public async Task<IActionResult> AsesorReportes(
            int idAsesor,
            int? anio = null,
            int? mes = null)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _useCaseGetReportesAsesor.Execute(
                idAsesor,
                anio ?? DateTime.Now.Year,
                mes ?? DateTime.Now.Month);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesAsesor", reportesAdministrador.Data);
        }
        [HttpGet]
        public async Task<IActionResult> SupervisorReportes(int idSupervisor, int? anio = null, int? mes = null)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var reportesAdministrador = await _useCaseGetReportesSupervisor.Execute(idSupervisor, anio, mes);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesSupervisor", reportesAdministrador.Data);
        }
        [HttpGet]
        public async Task<IActionResult> ReportesFechas(DateTime fecha)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                return Json(new { success = false, message = "Id de usuario no valido." });
            }
            var fechaString = fecha.ToString("yyyy-MM-dd");
            var reportesFechas = await _useCaseGetReportesFechas.Execute(fechaString, idUsuario.Value, rol.Value);
            if (!reportesFechas.IsSuccess)
            {
                return Json(new { success = false, message = reportesFechas.Message });
            }
            return PartialView("_ReportesFechas", reportesFechas.Data);
        }
        [HttpGet]
        public async Task<IActionResult> Metas()
        {
            var id = HttpContext.Session.GetInt32("UsuarioId");
            if (id == null)
            {
                TempData["MessageError"] = "No ha iniciado sesion.";
                return RedirectToAction("Index", "Home");
            }
            var reportes = await _useCaseGetReportesMetas.Execute(id.Value);
            return View("Metas", reportes.Data);
        }
        [HttpGet]
        public async Task<IActionResult> ReportesPorMes(int mes, int año)
        {
            var rol = HttpContext.Session.GetInt32("RolUser");
            if (rol == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión" });
            }
            var idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            if (idUsuario == null)
            {
                return Json(new { success = false, message = "Id de usuario no valido." });
            }
            var reportesAdministrador = await _useCaseGetReportesFechas.Execute(DateTime.Now.ToString(), idUsuario.Value, rol.Value, mes, año);
            if (!reportesAdministrador.IsSuccess)
            {
                return Json(new { success = false, message = reportesAdministrador.Message });
            }
            return PartialView("_ReportesMeses", reportesAdministrador.Data);
        }
    }
}