using Capa.Entidades;
using Capa.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace Prestamo.EquiposAPP.Controllers
{
    public class EquipoController : Controller
    {
        private readonly EquipoNegocio _negocio;
        private readonly IWebHostEnvironment _env;

        public EquipoController(IConfiguration config, IWebHostEnvironment env)
        {
            _negocio = new EquipoNegocio(config);
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

        // VISTA CATÁLOGO
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Catalogo(string tipoEquipo)
        {
            if (!UsuarioLogueado())
                return RedirectToAction("Index", "Login");

            // INTEGRACIÓN DE API DÓLAR
            var apiNegocio = new ExternoNegocio();

            // Solo solicitamos el precio del dólar
            var dolar = await apiNegocio.GetDolar();

            // Pasamos solo el dato del dólar a la vista
            ViewBag.Dolar = dolar != null ? dolar.conversion_rate.ToString("0.00") : "N/A";
          

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

            // Filtro de seguridad por Rol
            if (rol != "Administrador")
            {
                lista = lista.Where(e => e.Estado == "Disponible").ToList();
            }

            ViewBag.TipoSeleccionado = tipoEquipo;
            return View(lista);
        }

        public IActionResult Eliminar(int id)
        {
            if (!UsuarioLogueado()) return RedirectToAction("Index", "Login");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Administrador") return RedirectToAction("Catalogo");

            TempData["mensajeEliminar"] = _negocio.EliminarEquipo(id);
            return RedirectToAction("Catalogo");
        }

        public IActionResult Crear()
        {
            if (!UsuarioLogueado()) return RedirectToAction("Index", "Login");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Administrador") return RedirectToAction("Catalogo");

            return View("Crear", new Equipo());
        }

        [HttpPost]
        public IActionResult Crear(Equipo reg, IFormFile? imagenArchivo)
        {
            if (!UsuarioLogueado()) return RedirectToAction("Index", "Login");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Administrador") return RedirectToAction("Catalogo");

            if (string.IsNullOrWhiteSpace(reg.Nombre) || imagenArchivo == null || imagenArchivo.Length == 0)
            {
                ViewBag.mensaje = "⚠️ ERROR: El nombre y la imagen son obligatorios.";
                return View("Crear", reg);
            }

            try
            {
                reg.Imagen = GuardarImagen(imagenArchivo);
                ViewBag.mensaje = _negocio.NuevoEquipo(reg);
                return View("Crear", new Equipo());
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = "Error: " + ex.Message;
                return View("Crear", reg);
            }
        }

        public IActionResult Edit(int id)
        {
            if (!UsuarioLogueado()) return RedirectToAction("Index", "Login");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Administrador") return RedirectToAction("Catalogo");

            var equipo = _negocio.ObtenerPorId(id);
            if (equipo == null) return RedirectToAction("Catalogo");
            return View(equipo);
        }

        [HttpPost]
        public IActionResult Edit(Equipo reg, IFormFile? imagenArchivo)
        {
            if (!UsuarioLogueado()) return RedirectToAction("Index", "Login");
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Administrador") return RedirectToAction("Catalogo");

            try
            {
                var imagenNueva = GuardarImagen(imagenArchivo);
                if (!string.IsNullOrEmpty(imagenNueva))
                    reg.Imagen = imagenNueva;

                string mensaje = _negocio.EditarEquipo(reg);
                if (mensaje.Contains("correctamente"))
                {
                    return RedirectToAction("Catalogo");
                }
                ViewBag.mensaje = mensaje;
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = "Error: " + ex.Message;
            }

            return View(reg);
        }
    }
}