using Capa.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace Prestamo.EquiposAPP.Controllers
{
    public class MantenimientoController : Controller
    {
        private readonly MantenimientoNegocio _negocio;

        public MantenimientoController(IConfiguration config)
        {
            _negocio = new MantenimientoNegocio(config);
        }

        private bool EsAdmin()
        {
            var nombre = HttpContext.Session.GetString("UsuarioNombre");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            return !string.IsNullOrEmpty(nombre) && rol == "Administrador";
        }



        [HttpGet("api/mantenimiento")]
        public IActionResult ApiGetMantenimientos()
        {
            var nombre = HttpContext.Session.GetString("UsuarioNombre");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrEmpty(nombre) || rol != "Administrador")
            {
                return Unauthorized(new { mensaje = "No autorizado. Sesión de administrador requerida." });
            }
            try
            {
                var mantenimientos = _negocio.ListarActivos();
                return Ok(mantenimientos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al listar mantenimientos: " + ex.Message });
            }
        }

        [HttpPost("api/mantenimiento/completar/{id}")]
        public IActionResult ApiCompletarMantenimiento(int id, [FromBody] CompletarMantenimientoRequest request)
        {
            var nombre = HttpContext.Session.GetString("UsuarioNombre");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrEmpty(nombre) || rol != "Administrador")
            {
                return Unauthorized(new { mensaje = "No autorizado. Sesión de administrador requerida." });
            }
            if (request == null || request.Costo < 0)
            {
                return BadRequest(new { mensaje = "El costo no puede ser negativo." });
            }
            try
            {
                _negocio.CompletarMantenimiento(id, request.Costo);
                return Ok(new { mensaje = "El mantenimiento ha sido completado exitosamente y el equipo se encuentra disponible." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al completar mantenimiento: " + ex.Message });
            }
        }
    }

    public class CompletarMantenimientoRequest
    {
        public decimal Costo { get; set; }
    }
}

