using Microsoft.Data.SqlClient;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;
using System.Data;

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
                SqlCommand cmd = new SqlCommand("spValidarAcceso", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Correo", correo);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        string storedHash = dr["Contrasena"].ToString()!;
                        if (PasswordHelper.VerifyPassword(clave, storedHash))
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
            }
            return obj;
        }

        public bool ExisteCorreo(string correo)
        {
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spExisteCorreo", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Correo", correo);
                cn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public Usuario ObtenerPorCorreo(string correo)
        {
            Usuario obj = null;
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spObtenerUsuarioPorCorreo", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Correo", correo);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Usuario
                        {
                            UsuarioID = (int)dr["UsuarioID"],
                            Nombre = dr["Nombre"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            Rol = dr["Rol"].ToString()!
                        };
                    }
                }
            }
            return obj;
        }

        public void RegistrarUsuario(Usuario user)
        {
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spRegistrarUsuario", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Nombre", user.Nombre);
                cmd.Parameters.AddWithValue("@Correo", user.Correo);
                cmd.Parameters.AddWithValue("@Contrasena", PasswordHelper.HashPassword(user.Contrasena));
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
