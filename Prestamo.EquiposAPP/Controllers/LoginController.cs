using Microsoft.AspNetCore.Mvc;
using Capa.Negocio;
using Capa.Entidades;
using Capa.Datos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Prestamo.EquiposAPP.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioNegocio _negocio;
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _negocio = new UsuarioNegocio(config);
            _config = config;
        }

        [HttpGet("api/auth/test-session")]
        public IActionResult TestSession()
        {
            HttpContext.Session.SetInt32("UsuarioID", 999);
            return Ok(new { message = "Session set" });
        }



        [HttpGet("Login/LoginExterno/{provider}")]
        public IActionResult LoginExterno(string provider)
        {
            var properties = new AuthenticationProperties { RedirectUri = "/Login/CallbackExterno" };
            return Challenge(properties, provider);
        }

        [HttpGet("Login/CallbackExterno")]
        public async Task<IActionResult> CallbackExterno()
        {
            var frontendUrl = _config["FrontendUrl"] ?? "http://localhost:5174";
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                return Redirect($"{frontendUrl}/login?error=auth_failed");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Redirect($"{frontendUrl}/login?error=no_email");
            }

            var user = _negocio.ObtenerPorCorreo(email);
            if (user == null)
            {
                user = new Usuario
                {
                    Nombre = name ?? email,
                    Correo = email,
                    Contrasena = PasswordHelper.HashPassword(Guid.NewGuid().ToString("N")),
                    Rol = "Usuario"
                };
                _negocio.Registrar(user);
                user = _negocio.ObtenerPorCorreo(email);
            }

            HttpContext.Session.SetInt32("UsuarioID", user.UsuarioID);
            HttpContext.Session.SetString("UsuarioNombre", user.Nombre);
            HttpContext.Session.SetString("UsuarioRol", user.Rol);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var userData = new {
                usuarioID = user.UsuarioID,
                nombre = user.Nombre,
                rol = user.Rol
            };
            var dataJson = System.Text.Json.JsonSerializer.Serialize(userData);
            var base64Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dataJson));

            return Redirect($"{frontendUrl}/login?auth=success&data={Uri.EscapeDataString(base64Data)}");
        }

        [HttpGet("api/auth/session")]
        public IActionResult ApiGetSession()
        {
            var usuarioID = HttpContext.Session.GetInt32("UsuarioID");
            var nombre = HttpContext.Session.GetString("UsuarioNombre");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioID == null)
            {
                return Unauthorized(new { mensaje = "No hay sesión activa" });
            }

            return Ok(new {
                usuarioID = usuarioID,
                nombre = nombre,
                rol = rol
            });
        }


        [HttpPost("api/auth/login")]
        public IActionResult ApiLogin([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Correo) || string.IsNullOrEmpty(request.Clave))
            {
                return BadRequest(new { mensaje = "El correo y la contraseña son obligatorios" });
            }

            var user = _negocio.Login(request.Correo, request.Clave);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UsuarioID", user.UsuarioID);
                HttpContext.Session.SetString("UsuarioNombre", user.Nombre);
                HttpContext.Session.SetString("UsuarioRol", user.Rol);

                return Ok(new { 
                    usuarioID = user.UsuarioID, 
                    nombre = user.Nombre, 
                    rol = user.Rol 
                });
            }

            return Unauthorized(new { mensaje = "Credenciales incorrectas" });
        }

        [HttpPost("api/auth/register")]
        public IActionResult ApiRegister([FromBody] Usuario user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Nombre) || string.IsNullOrWhiteSpace(user.Correo) || string.IsNullOrWhiteSpace(user.Contrasena))
            {
                return BadRequest(new { mensaje = "Todos los campos son obligatorios" });
            }

            if (_negocio.ExisteCorreo(user.Correo))
            {
                return BadRequest(new { mensaje = "El correo electrónico ya se encuentra registrado" });
            }

            _negocio.Registrar(user);
            return Ok(new { mensaje = "Usuario registrado exitosamente" });
        }
    }

    public class LoginRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }
}
