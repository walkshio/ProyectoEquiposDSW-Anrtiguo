using Microsoft.Data.SqlClient;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Datos
{
    public class UsuarioDAO
    {
        private readonly string _cadena;

        public UsuarioDAO(IConfiguration config)
        {
            _cadena = config.GetConnectionString("DefaultConnection")!;
        }
        public Usuario ValidarAcceso(string correo, string clave)
        {
            Usuario obj = null;
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                string sql = "SELECT UsuarioID, Nombre, Correo, Rol FROM Usuario WHERE Correo = @correo AND Contrasena = @clave";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@correo", correo);
                cmd.Parameters.AddWithValue("@clave", clave);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Usuario
                        {
                            UsuarioID = (int)dr["UsuarioID"],
                            Nombre = dr["Nombre"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Rol = dr["Rol"].ToString()
                        };
                    }
                }
            }
            return obj;
        }
    }
}
