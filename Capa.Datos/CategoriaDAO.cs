using Microsoft.Data.SqlClient;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Capa.Datos
{
    public class CategoriaDAO
    {
        private readonly string _cadena;

        public CategoriaDAO(IConfiguration config)
        {
            _cadena = config.GetConnectionString("DefaultConnection")!;
        }

        public List<Categoria> ListarTodo()
        {
            var lista = new List<Categoria>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarCategorias", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Categoria
                        {
                            CategoriaID = Convert.ToInt32(dr["CategoriaID"]),
                            Nombre = dr["Nombre"]?.ToString() ?? "",
                            Descripcion = dr["Descripcion"]?.ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}
