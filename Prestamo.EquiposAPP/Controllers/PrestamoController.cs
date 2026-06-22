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
        private readonly CategoriaNegocio _catNegocio;
        private readonly IConfiguration _config;

        public PrestamoController(IConfiguration config)
        {
            _negocio = new PrestamoNegocio(config);
            _catNegocio = new CategoriaNegocio(config);
            _config = config;
        }

        private bool UsuarioLogueado()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioNombre"));
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("UsuarioRol") == "Administrador";
        }



        [HttpPost("api/prestamos/solicitar")]
        public IActionResult ApiSolicitar([FromBody] SolicitarRequest request)
        {
            var usuarioIDObj = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioIDObj == null)
            {
                return Unauthorized(new { mensaje = "Sesión no iniciada" });
            }

            if (request == null || request.EquipoID <= 0 || request.FechaFin <= DateTime.Today)
            {
                return BadRequest(new { mensaje = "Datos de solicitud inválidos. La fecha de devolución debe ser futura." });
            }

            try
            {
                int usuarioID = usuarioIDObj.Value;
                string mensaje = _negocio.RegistrarSolicitud(request.EquipoID, usuarioID, request.FechaFin);
                if (mensaje.Contains("exitosamente"))
                {
                    return Ok(new { mensaje = mensaje });
                }
                return BadRequest(new { mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpGet("api/prestamos/usuario")]
        public IActionResult ApiGetMisSolicitudes()
        {
            int? usuarioIDObj = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioIDObj == null)
            {
                return Unauthorized(new { mensaje = "Sesión no iniciada" });
            }
            var solicitudes = _negocio.ObtenerSolicitudesUsuario(usuarioIDObj.Value);
            return Ok(solicitudes);
        }

        [HttpGet("api/prestamos/pendientes")]
        public IActionResult ApiGetPendientes()
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }
            var solicitudes = _negocio.ObtenerSolicitudesPendientes();
            return Ok(solicitudes);
        }

        [HttpGet("api/prestamos/todos")]
        public IActionResult ApiGetTodos()
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }
            var solicitudes = _negocio.ObtenerTodos();
            return Ok(solicitudes);
        }

        [HttpGet("api/prestamos/en-uso")]
        public IActionResult ApiGetEnUso()
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }
            var lista = _negocio.ObtenerEnUso();
            return Ok(lista);
        }

        [HttpPost("api/prestamos/aprobar/{id}")]
        public IActionResult ApiAprobar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }
            var prestamo = _negocio.ObtenerPorId(id);
            string resultado = _negocio.AprobarSolicitud(id);
            if (resultado.Contains("correctamente"))
            {
                if (prestamo != null)
                {
                    try { EmailService.NotificarAprobacion(_config, prestamo); } catch {}
                }
                return Ok(new { mensaje = resultado });
            }
            return BadRequest(new { mensaje = resultado });
        }

        [HttpPost("api/prestamos/rechazar/{id}")]
        public IActionResult ApiRechazar(int id, [FromBody] RejectRequest request)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }
            if (string.IsNullOrWhiteSpace(request?.MotivoRechazo))
            {
                return BadRequest(new { mensaje = "El motivo de rechazo es obligatorio." });
            }
            var prestamo = _negocio.ObtenerPorId(id);
            string resultado = _negocio.RechazarSolicitud(id, request.MotivoRechazo);
            if (resultado.Contains("correctamente"))
            {
                if (prestamo != null)
                {
                    try { EmailService.NotificarRechazo(_config, prestamo, request.MotivoRechazo); } catch {}
                }
                return Ok(new { mensaje = resultado });
            }
            return BadRequest(new { mensaje = resultado });
        }

        [HttpPost("api/prestamos/entregar/{id}")]
        public IActionResult ApiEntregar(int id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }
            string resultado = _negocio.EntregarEquipo(id);
            if (resultado.Contains("exitosamente") || resultado.Contains("correctamente") || resultado.Contains("éxito"))
            {
                return Ok(new { mensaje = resultado });
            }
            return BadRequest(new { mensaje = resultado });
        }

        [HttpPost("api/prestamos/confirmar-entrega/{id}")]
        public IActionResult ApiConfirmarEntrega(int id)
        {
            var usuarioIDObj = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioIDObj == null)
            {
                return Unauthorized(new { mensaje = "Sesión no iniciada" });
            }

            string resultado = _negocio.EntregarEquipo(id);
            if (resultado.Contains("exitosamente") || resultado.Contains("correctamente") || resultado.Contains("éxito"))
            {
                return Ok(new { mensaje = resultado });
            }
            return BadRequest(new { mensaje = resultado });
        }

        [HttpPost("api/prestamos/devolver/{id}")]
        public IActionResult ApiDevolver(int id, [FromBody] DevolverRequest request)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }
            string resultado = _negocio.DevolverEquipo(id, DateTime.Now, request.Incidencia, request.MultaDanio);
            if (resultado.Contains("exitosamente") || resultado.Contains("correctamente") || resultado.Contains("éxito") || resultado.Contains("devuelto"))
            {
                return Ok(new { mensaje = resultado });
            }
            return BadRequest(new { mensaje = resultado });
        }
        [HttpGet("api/prestamos/{id}")]
        public IActionResult ApiGetPrestamo(int id)
        {
            var prestamo = _negocio.ObtenerPorId(id);
            if (prestamo == null)
            {
                return NotFound(new { mensaje = "Préstamo no encontrado" });
            }

            int sessionUsuarioID = HttpContext.Session.GetInt32("UsuarioID") ?? 0;
            string sessionRol = HttpContext.Session.GetString("UsuarioRol") ?? "";

            if (sessionRol != "Administrador" && prestamo.UsuarioID != sessionUsuarioID)
            {
                return Forbid();
            }

            return Ok(prestamo);
        }
    }

    public class SolicitarRequest
    {
        public int EquipoID { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class RejectRequest
    {
        public string MotivoRechazo { get; set; } = string.Empty;
    }

    public class DevolverRequest
    {
        public string? Incidencia { get; set; }
        public decimal MultaDanio { get; set; }
    }
}

