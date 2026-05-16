using Microsoft.Data.SqlClient;
using Capa.Entidades;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Capa.Datos
{
    public class EquipoDAO
    {
        private readonly string _cadena;

        public EquipoDAO(IConfiguration config)
        {
            _cadena = config.GetConnectionString("DefaultConnection")!;
        }

        public List<Equipo> ListarDisponibles()
        {
            var lista = new List<Equipo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListEquiposDisp", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Equipo
                        {
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            Nombre = dr["Nombre"]?.ToString() ?? "",
                            Tipo = dr["Tipo"]?.ToString() ?? "",
                            Estado = dr["Estado"]?.ToString() ?? "",
                            Imagen = dr["Imagen"]?.ToString()
                        });
                    }
                }
            }
            return lista;
        }
        public string Registrar(Equipo reg)
        {
            string mensaje;
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spAddEquipo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", (object)reg.Nombre ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipo", (object)reg.Tipo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Imagen", (object)reg.Imagen ?? DBNull.Value);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "Equipo registrado con éxito.";
                }
                catch (Exception ex) { mensaje = "Error: " + ex.Message; }
            }
            return mensaje;
        }
        
        public Equipo BuscarPorId(int id)
        {
            Equipo obj = null;
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Equipo WHERE EquipoID = @id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Equipo
                        {
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            Nombre = dr["Nombre"].ToString() ?? "",
                            Tipo = dr["Tipo"].ToString() ?? "",
                            Estado = dr["Estado"].ToString() ?? "",
                            Imagen = dr["Imagen"]?.ToString()
                        };
                    }
                }
            }
            return obj;
        }
        public string Actualizar(Equipo reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    string sql = "UPDATE Equipo SET Nombre=@nom, Tipo=@tipo, Estado=@est, Imagen=@img WHERE EquipoID=@id";
                    SqlCommand cmd = new SqlCommand(sql, cn);

                    cmd.Parameters.AddWithValue("@nom", reg.Nombre);
                    cmd.Parameters.AddWithValue("@tipo", reg.Tipo);
                    cmd.Parameters.AddWithValue("@est", reg.Estado);
                    cmd.Parameters.AddWithValue("@img", (object)reg.Imagen ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", reg.EquipoID);

                    cn.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        mensaje = "Equipo actualizado correctamente";
                    else
                        mensaje = "Error: No se encontró el equipo con ID " + reg.EquipoID;
                }
                catch (Exception ex) { mensaje = "Error: " + ex.Message; }
            }
            return mensaje;
        }
        public List<Equipo> ListarTodo()
        {
            var lista = new List<Equipo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Equipo", cn);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Equipo
                        {
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            Nombre = dr["Nombre"]?.ToString() ?? "",
                            Tipo = dr["Tipo"]?.ToString() ?? "",
                            Estado = dr["Estado"]?.ToString() ?? "",
                            Imagen = dr["Imagen"]?.ToString()
                        });
                    }
                }
            }
            return lista;
        }
        public List<Equipo> ListarPorTipo(string tipo)
        {
            var lista = new List<Equipo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                string sql = "SELECT * FROM Equipo WHERE Tipo = @tipo";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Equipo
                        {
                            EquipoID = (int)dr["EquipoID"],
                            Nombre = dr["Nombre"].ToString(),
                            Tipo = dr["Tipo"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            Imagen = dr["Imagen"]?.ToString()
                        });
                    }
                }
            }
            return lista;
        }
        public string Eliminar(int id)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    string sql = "DELETE FROM Equipo WHERE EquipoID = @id";
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();
                    mensaje = filas > 0 ? "Equipo eliminado correctamente." : "No se encontró el equipo.";
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 547) // Código para conflicto de FK
                    {
                        mensaje = "Error: No se puede eliminar el equipo porque tiene préstamos o solicitudes asociadas.";
                    }
                    else
                    {
                        mensaje = "Error en base de datos: " + sqlEx.Message;
                    }
                }
                catch (Exception ex) { mensaje = "Error: " + ex.Message; }
            }
            return mensaje;
        }
    }
}
