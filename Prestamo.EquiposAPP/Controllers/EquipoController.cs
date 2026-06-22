using Capa.Entidades;
using Capa.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace Prestamo.EquiposAPP.Controllers
{
    public class EquipoController : Controller
    {
        private readonly EquipoNegocio _negocio;
        private readonly CategoriaNegocio _catNegocio;
        private readonly IWebHostEnvironment _env;

        public EquipoController(IConfiguration config, IWebHostEnvironment env)
        {
            _negocio = new EquipoNegocio(config);
            _catNegocio = new CategoriaNegocio(config);
            _env = env;
        }

        private bool UsuarioLogueado()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioNombre"));
        }

        private string? GuardarImagen(IFormFile? imagenArchivo)
        {
            if (imagenArchivo == null || imagenArchivo.Length == 0)
                return null;

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imagenArchivo.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
                throw new InvalidOperationException("Solo se permiten imágenes JPG, JPEG, PNG o WEBP.");

            string carpetaImagenes = Path.Combine(_env.WebRootPath, "imagenes");
            Directory.CreateDirectory(carpetaImagenes);

            string nombreArchivo = $"equipo-{DateTime.Now:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}{extension}";
            string rutaFisica = Path.Combine(carpetaImagenes, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                imagenArchivo.CopyTo(stream);
            }

            return "/imagenes/" + nombreArchivo;
        }



        [HttpGet("api/equipos")]
        public IActionResult ApiGetEquipos(string? tipoEquipo)
        {
            var rol = HttpContext.Session.GetString("UsuarioRol");
            List<Equipo> lista;

            if (!string.IsNullOrEmpty(tipoEquipo) && tipoEquipo != "Todos")
            {
                lista = _negocio.FiltrarEquipos(tipoEquipo);
            }
            else
            {
                lista = _negocio.ObtenerTodo();
            }

            if (rol != "Administrador")
            {
                lista = lista.Where(e => e.Estado == "Disponible").ToList();
            }

            return Ok(new {
                equipos = lista
            });
        }

        [HttpGet("api/equipos/categorias")]
        public IActionResult ApiGetCategorias()
        {
            return Ok(_catNegocio.ObtenerTodas());
        }

        [HttpGet("api/equipos/dashboard")]
        public IActionResult ApiGetDashboard()
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }

            var kpis = _negocio.ObtenerKPIs();
            var actividades = _negocio.ObtenerActividadReciente();

            return Ok(new {
                kpis = kpis,
                actividades = actividades
            });
        }
        [HttpPost("api/equipos")]
        public IActionResult ApiCrearEquipo([FromForm] Equipo reg, [FromForm] IFormFile? imagenArchivo)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(reg.Nombre))
            {
                return BadRequest(new { mensaje = "El nombre del equipo es obligatorio." });
            }

            try
            {
                reg.Imagen = GuardarImagen(imagenArchivo) ?? "";
                string mensaje = _negocio.NuevoEquipo(reg);
                if (mensaje.Contains("correctamente") || mensaje.Contains("éxito"))
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

        [HttpPut("api/equipos/{id}")]
        public IActionResult ApiEditarEquipo(int id, [FromForm] Equipo reg, [FromForm] IFormFile? imagenArchivo)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }

            try
            {
                reg.EquipoID = id;
                var imagenNueva = GuardarImagen(imagenArchivo);
                if (!string.IsNullOrEmpty(imagenNueva))
                {
                    reg.Imagen = imagenNueva;
                }
                else
                {
                    var eqAnterior = _negocio.ObtenerPorId(id);
                    if (eqAnterior != null) reg.Imagen = eqAnterior.Imagen;
                }

                string mensaje = _negocio.EditarEquipo(reg);
                if (mensaje.Contains("correctamente") || mensaje.Contains("éxito"))
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

        [HttpDelete("api/equipos/{id}")]
        public IActionResult ApiEliminarEquipo(int id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Administrador")
            {
                return Forbid();
            }

            string mensaje = _negocio.EliminarEquipo(id);
            if (mensaje.Contains("eliminado") || mensaje.Contains("correctamente") || mensaje.Contains("éxito"))
            {
                return Ok(new { mensaje = mensaje });
            }
            return BadRequest(new { mensaje = mensaje });
        }
    }
}