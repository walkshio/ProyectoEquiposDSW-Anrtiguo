using Microsoft.AspNetCore.Mvc;
using Capa.Negocio;

namespace Prestamo.EquiposAPP.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioNegocio _negocio;

        public LoginController(IConfiguration config)
        {
            _negocio = new UsuarioNegocio(config);
        }
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Index(string correo, string clave)
        {
            var user = _negocio.Login(correo, clave);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UsuarioID", user.UsuarioID);
                HttpContext.Session.SetString("UsuarioNombre", user.Nombre);
                HttpContext.Session.SetString("UsuarioRol", user.Rol);

                return RedirectToAction("Catalogo", "Equipo");
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Index");
        }
    }
}
