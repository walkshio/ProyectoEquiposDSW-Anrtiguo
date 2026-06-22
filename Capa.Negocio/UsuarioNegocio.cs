using Capa.Datos;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Negocio
{
    public class UsuarioNegocio
    {
        private readonly UsuarioDAO _dao;

        public UsuarioNegocio(IConfiguration config)
        {
            _dao = new UsuarioDAO(config);
        }
        public Usuario Login(string correo, string clave) => _dao.ValidarAcceso(correo, clave);
        public bool ExisteCorreo(string correo) => _dao.ExisteCorreo(correo);
        public Usuario ObtenerPorCorreo(string correo) => _dao.ObtenerPorCorreo(correo);
        public void Registrar(Usuario user) => _dao.RegistrarUsuario(user);
    }
}
