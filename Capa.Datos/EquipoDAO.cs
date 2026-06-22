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
                            Tipo = dr["Categoria"]?.ToString() ?? "",
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
                    cmd.Parameters.AddWithValue("@CategoriaID", reg.CategoriaID);
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
                SqlCommand cmd = new SqlCommand("spBuscarEquipoPorId", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@EquipoID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Equipo
                        {
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            Nombre = dr["Nombre"].ToString() ?? "",
                            CategoriaID = Convert.ToInt32(dr["CategoriaID"]),
                            Tipo = dr["Categoria"]?.ToString() ?? "",
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
                    SqlCommand cmd = new SqlCommand("spActualizarEquipo", cn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@EquipoID", reg.EquipoID);
                    cmd.Parameters.AddWithValue("@Nombre", reg.Nombre);
                    cmd.Parameters.AddWithValue("@CategoriaID", reg.CategoriaID);
                    cmd.Parameters.AddWithValue("@Estado", reg.Estado);
                    cmd.Parameters.AddWithValue("@Imagen", (object)reg.Imagen ?? DBNull.Value);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "Equipo actualizado correctamente";
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
                SqlCommand cmd = new SqlCommand("spListarEquipos", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Equipo
                        {
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            Nombre = dr["Nombre"]?.ToString() ?? "",
                            CategoriaID = Convert.ToInt32(dr["CategoriaID"]),
                            Tipo = dr["Categoria"]?.ToString() ?? "",
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
                SqlCommand cmd = new SqlCommand("spListarEquiposPorTipo", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Equipo
                        {
                            EquipoID = (int)dr["EquipoID"],
                            Nombre = dr["Nombre"].ToString(),
                            CategoriaID = (int)dr["CategoriaID"],
                            Tipo = dr["Categoria"]?.ToString() ?? "",
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
                    SqlCommand cmd = new SqlCommand("spEliminarEquipo", cn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@EquipoID", id);
                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();
                    mensaje = filas > 0 ? "Equipo eliminado correctamente." : "No se encontró el equipo.";
                }
                catch (Exception ex) { mensaje = "Error: " + ex.Message; }
            }
            return mensaje;
        }

        public Dictionary<string, object> ObtenerKPIs()
        {
            var kpis = new Dictionary<string, object>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spObtenerKPIs", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        kpis["TotalEquipos"] = Convert.ToInt32(dr["TotalEquipos"]);
                        kpis["Disponibles"] = Convert.ToInt32(dr["Disponibles"]);
                        kpis["Reservados"] = Convert.ToInt32(dr["Reservados"]);
                        kpis["EnUso"] = Convert.ToInt32(dr["EnUso"]);
                        kpis["Mantenimiento"] = Convert.ToInt32(dr["Mantenimiento"]);
                        kpis["TotalMultas"] = Convert.ToDecimal(dr["TotalMultas"]);
                    }
                }
            }
            return kpis;
        }

        public List<Dictionary<string, object>> ListarActividadReciente()
        {
            var lista = new List<Dictionary<string, object>>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarActividadReciente", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var item = new Dictionary<string, object>();
                        item["DetalleID"] = Convert.ToInt32(dr["DetalleID"]);
                        item["Equipo"] = dr["Equipo"]?.ToString() ?? "";
                        item["Usuario"] = dr["Usuario"]?.ToString() ?? "";
                        item["EstadoDetalle"] = dr["EstadoDetalle"]?.ToString() ?? "";
                        item["FechaInicio"] = dr["FechaInicio"] == DBNull.Value ? null : (object)Convert.ToDateTime(dr["FechaInicio"]);
                        item["FechaFin"] = Convert.ToDateTime(dr["FechaFin"]);
                        item["FechaDevolucion"] = dr["FechaDevolucion"] == DBNull.Value ? null : (object)Convert.ToDateTime(dr["FechaDevolucion"]);
                        item["Multa"] = Convert.ToDecimal(dr["Multa"]);
                        item["MultaDanio"] = Convert.ToDecimal(dr["MultaDanio"]);
                        item["Incidencia"] = dr["Incidencia"] == DBNull.Value ? "" : dr["Incidencia"].ToString()!;
                        lista.Add(item);
                    }
                }
            }
            return lista;
        }
    }
}

