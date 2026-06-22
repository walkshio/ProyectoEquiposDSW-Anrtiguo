using Capa.Datos;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Negocio
{
    public class CategoriaNegocio
    {
        private readonly CategoriaDAO _dao;

        public CategoriaNegocio(IConfiguration config)
        {
            _dao = new CategoriaDAO(config);
        }

        public List<Categoria> ObtenerTodas() => _dao.ListarTodo();
    }
}
