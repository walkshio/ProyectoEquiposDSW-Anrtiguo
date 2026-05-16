using Microsoft.AspNetCore.Mvc;
using Capa.Negocio;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Prestamo.EquiposAPP.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PrestamoController : Controller
    {
        private readonly PrestamoNegocio _negocio;

        public PrestamoController(IConfiguration config)
        {
            _negocio = new PrestamoNegocio(config);
        }

        private bool UsuarioLogueado()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioNombre"));
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("UsuarioRol") == "Administrador";
        }

        public IActionResult Solicitar(string tipoEquipo)
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            List<Equipo> lista;

            if (!string.IsNullOrEmpty(tipoEquipo) && tipoEquipo != "Todos")
            {
                lista = _negocio.ObtenerEquiposPorTipo(tipoEquipo);
            }
            else
            {
                lista = _negocio.ObtenerEquiposDisponibles();
            }

            ViewBag.TipoSeleccionado = tipoEquipo;
            return View(lista);
        }

        [HttpPost]
        public IActionResult Solicitar(int equipoID, DateTime fechaFin)
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            if (fechaFin <= DateTime.Today)
            {
                TempData["mensaje"] = "La fecha de devolución debe ser posterior a hoy.";
                TempData["tipo"] = "warning";
                return RedirectToAction("Solicitar");
            }

            int usuarioID = HttpContext.Session.GetInt32("UsuarioID") ?? 0;

            if (usuarioID == 0)
            {
                TempData["mensaje"] = "Error: Usuario no identificado.";
                TempData["tipo"] = "danger";
                return RedirectToAction("Index", "Login");
            }

            string resultado = _negocio.RegistrarSolicitud(equipoID, usuarioID, fechaFin);

            if (resultado.Contains("éxito"))
            {
                TempData["mensaje"] = resultado;
                TempData["tipo"] = "success";
            }
            else
            {
                TempData["mensaje"] = resultado;
                TempData["tipo"] = "danger";
            }

            return RedirectToAction("Solicitar");
        }

        public IActionResult Pendientes()
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            if (!EsAdministrador())
                return RedirectToAction("Catalogo", "Equipo");

            var solicitudes = _negocio.ObtenerSolicitudesPendientes();
            return View(solicitudes);
        }

        public IActionResult MisSolicitudes()
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            int usuarioID = HttpContext.Session.GetInt32("UsuarioID") ?? 0;

            if (usuarioID == 0)
                return RedirectToAction("Index", "Login");

            var solicitudes = _negocio.ObtenerSolicitudesUsuario(usuarioID);
            return View(solicitudes);
        }

        public IActionResult Aprobar(int id)
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            if (!EsAdministrador())
                return RedirectToAction("Catalogo", "Equipo");

            string resultado = _negocio.AprobarSolicitud(id);
            TempData["mensaje"] = resultado;
            TempData["tipo"] = resultado.Contains("correctamente") ? "success" : "warning";
            return RedirectToAction("Pendientes");
        }

        public IActionResult Rechazar(int id)
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            if (!EsAdministrador())
                return RedirectToAction("Catalogo", "Equipo");

            string resultado = _negocio.RechazarSolicitud(id);
            TempData["mensaje"] = resultado;
            TempData["tipo"] = resultado.Contains("correctamente") ? "success" : "warning";
            return RedirectToAction("Pendientes");
        }
        public IActionResult Entregar(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Catalogo", "Equipo");

            string resultado = _negocio.EntregarEquipo(id);
            TempData["mensaje"] = resultado;

            return RedirectToAction("Pendientes");
        }

        public IActionResult Devolver(int id)
        {
            if (!EsAdministrador())
                return RedirectToAction("Catalogo", "Equipo");

            string resultado = _negocio.DevolverEquipo(id, DateTime.Now);
            TempData["mensaje"] = resultado;

            return RedirectToAction("Pendientes");
        }
        public IActionResult EnUso()
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            if (!EsAdministrador())
                return RedirectToAction("Catalogo", "Equipo");

            var lista = _negocio.ObtenerEnUso();
            return View("PrestamosEnUso", lista);
        }
        public IActionResult ConfirmarEntrega(int id)
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            string resultado = _negocio.EntregarEquipo(id);

            TempData["mensaje"] = resultado;
            TempData["tipo"] = "success";

            return RedirectToAction("MisSolicitudes");
        }

        public IActionResult NoEntregado()
        {
            TempData["mensaje"] = "Se ha informado del problema. La entrega se realizará pronto.";
            TempData["tipo"] = "warning";

            return RedirectToAction("MisSolicitudes");
        }
    }
}
