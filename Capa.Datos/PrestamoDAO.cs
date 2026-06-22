using Microsoft.Data.SqlClient;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Capa.Datos
{
    public class PrestamoDAO
    {
        private readonly string _cadena;

        public PrestamoDAO(IConfiguration config)
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

        public List<Equipo> ListarPorTipo(string tipo)
        {
            var lista = new List<Equipo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarEquiposDispPorTipo", cn)
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

        public string SolicitarPrestamo(int equipoID, int usuarioID, DateTime fechaFin)
        {
            string mensaje;
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spSolicitarPrestamo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EquipoID", equipoID);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "✅ Solicitud de préstamo registrada con éxito. Estado: Pendiente de aprobación.";
                }
                catch (Exception ex)
                {
                    mensaje = "❌ Error: " + ex.Message;
                }
            }
            return mensaje;
        }

        public List<Prestamo> ListarPendientes()
        {
            var lista = new List<Prestamo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarSolicitudesPendientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Prestamo
                        {
                            PrestamoID = Convert.ToInt32(dr["PrestamoID"]),
                            DetalleID = Convert.ToInt32(dr["DetalleID"]),
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            FechaSolicitud = Convert.ToDateTime(dr["FechaSolicitud"]),
                            FechaInicio = dr["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                            FechaDevolucion = dr["FechaDevolucion"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaDevolucion"]),
                            Estado = dr["EstadoDetalle"]?.ToString() ?? "",
                            Multa = dr["Multa"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Multa"]),
                            MotivoRechazo = dr["MotivoRechazo"] == DBNull.Value ? null : dr["MotivoRechazo"].ToString(),
                            Incidencia = dr["Incidencia"] == DBNull.Value ? null : dr["Incidencia"].ToString(),
                            MultaDanio = dr["MultaDanio"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MultaDanio"]),
                            NombreEquipo = dr["NombreEquipo"]?.ToString() ?? "",
                            TipoEquipo = dr["CategoriaEquipo"]?.ToString() ?? "",
                            NombreUsuario = dr["NombreUsuario"]?.ToString() ?? "",
                            CorreoUsuario = dr["CorreoUsuario"]?.ToString() ?? ""
                        });
                    }
                }
            }
            return lista;
        }

        public List<Prestamo> ListarTodos()
        {
            var lista = new List<Prestamo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarTodosPrestamos", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Prestamo
                        {
                            PrestamoID = Convert.ToInt32(dr["PrestamoID"]),
                            DetalleID = Convert.ToInt32(dr["DetalleID"]),
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            FechaSolicitud = Convert.ToDateTime(dr["FechaSolicitud"]),
                            FechaInicio = dr["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                            FechaDevolucion = dr["FechaDevolucion"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaDevolucion"]),
                            Estado = dr["EstadoDetalle"]?.ToString() ?? "",
                            Multa = dr["Multa"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Multa"]),
                            MotivoRechazo = dr["MotivoRechazo"] == DBNull.Value ? null : dr["MotivoRechazo"].ToString(),
                            Incidencia = dr["Incidencia"] == DBNull.Value ? null : dr["Incidencia"].ToString(),
                            MultaDanio = dr["MultaDanio"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MultaDanio"]),
                            NombreEquipo = dr["NombreEquipo"]?.ToString() ?? "",
                            TipoEquipo = dr["CategoriaEquipo"]?.ToString() ?? "",
                            NombreUsuario = dr["NombreUsuario"]?.ToString() ?? "",
                            CorreoUsuario = dr["CorreoUsuario"]?.ToString() ?? ""
                        });
                    }
                }
            }
            return lista;
        }

        public List<Prestamo> ListarPorUsuario(int usuarioID)
        {
            var lista = new List<Prestamo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarSolicitudesUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Prestamo
                        {
                            PrestamoID = Convert.ToInt32(dr["PrestamoID"]),
                            DetalleID = Convert.ToInt32(dr["DetalleID"]),
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            FechaSolicitud = Convert.ToDateTime(dr["FechaSolicitud"]),
                            FechaInicio = dr["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                            FechaDevolucion = dr["FechaDevolucion"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaDevolucion"]),
                            Estado = dr["EstadoDetalle"]?.ToString() ?? "",
                            Multa = dr["Multa"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Multa"]),
                            MotivoRechazo = dr["MotivoRechazo"] == DBNull.Value ? null : dr["MotivoRechazo"].ToString(),
                            Incidencia = dr["Incidencia"] == DBNull.Value ? null : dr["Incidencia"].ToString(),
                            MultaDanio = dr["MultaDanio"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MultaDanio"]),
                            NombreEquipo = dr["NombreEquipo"]?.ToString() ?? "",
                            TipoEquipo = dr["CategoriaEquipo"]?.ToString() ?? "",
                            NombreUsuario = dr["NombreUsuario"]?.ToString() ?? "",
                            CorreoUsuario = dr["CorreoUsuario"]?.ToString() ?? ""
                        });
                    }
                }
            }
            return lista;
        }

        public string AprobarSolicitud(int prestamoID)
        {
            return CambiarEstadoSolicitud(prestamoID, "spAprobarSolicitud", "Solicitud aprobada correctamente.");
        }

        public string RechazarSolicitud(int prestamoID, string motivoRechazo)
        {
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spRechazarSolicitud", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PrestamoID", prestamoID);
                    cmd.Parameters.AddWithValue("@MotivoRechazo", motivoRechazo);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    return "Solicitud rechazada correctamente.";
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }

        private string CambiarEstadoSolicitud(int prestamoID, string procedimiento, string mensajeExito)
        {
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(procedimiento, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PrestamoID", prestamoID);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    return mensajeExito;
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }

        public string EntregarEquipo(int prestamoID)
        {
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spEntregarEquipo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PrestamoID", prestamoID);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    return "Equipo entregado correctamente.";
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }

        public string DevolverEquipo(int prestamoID, DateTime fechaDev, string? incidencia, decimal multaDanio)
        {
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spDevolverEquipo", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PrestamoID", prestamoID);
                    cmd.Parameters.AddWithValue("@FechaDevolucion", fechaDev);
                    cmd.Parameters.AddWithValue("@Incidencia", (object)incidencia ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MultaDanio", multaDanio);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    return "Equipo devuelto correctamente.";
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message;
                }
            }
        }
        public List<Prestamo> ListarEnUso()
        {
            var lista = new List<Prestamo>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarPrestamosEnUso", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                         lista.Add(new Prestamo
                        {
                            PrestamoID = Convert.ToInt32(dr["PrestamoID"]),
                            DetalleID = Convert.ToInt32(dr["DetalleID"]),
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            FechaSolicitud = Convert.ToDateTime(dr["FechaSolicitud"]),
                            FechaInicio = dr["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                            FechaDevolucion = dr["FechaDevolucion"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaDevolucion"]),
                            Estado = dr["EstadoDetalle"]?.ToString() ?? "",
                            Multa = dr["Multa"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Multa"]),
                            MotivoRechazo = null,
                            Incidencia = dr["Incidencia"] == DBNull.Value ? null : dr["Incidencia"].ToString(),
                            MultaDanio = dr["MultaDanio"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MultaDanio"]),
                            NombreEquipo = dr["NombreEquipo"]?.ToString() ?? "",
                            TipoEquipo = dr["CategoriaEquipo"]?.ToString() ?? "",
                            NombreUsuario = dr["NombreUsuario"]?.ToString() ?? "",
                            CorreoUsuario = dr["CorreoUsuario"]?.ToString() ?? ""
                        });
                    }
                }
            }
            return lista;
        }
        public Prestamo BuscarPorId(int id)
        {
            Prestamo obj = null;
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spBuscarPrestamoPorId", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PrestamoID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Prestamo
                        {
                            PrestamoID = Convert.ToInt32(dr["PrestamoID"]),
                            DetalleID = Convert.ToInt32(dr["DetalleID"]),
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            FechaSolicitud = Convert.ToDateTime(dr["FechaSolicitud"]),
                            FechaInicio = dr["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                            FechaDevolucion = dr["FechaDevolucion"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaDevolucion"]),
                            Estado = dr["EstadoDetalle"]?.ToString() ?? "",
                            Multa = dr["Multa"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Multa"]),
                            MotivoRechazo = dr["MotivoRechazo"] == DBNull.Value ? null : dr["MotivoRechazo"].ToString(),
                            Incidencia = dr["Incidencia"] == DBNull.Value ? null : dr["Incidencia"].ToString(),
                            MultaDanio = dr["MultaDanio"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MultaDanio"]),
                            NombreEquipo = dr["NombreEquipo"]?.ToString() ?? "",
                            TipoEquipo = dr["CategoriaEquipo"]?.ToString() ?? "",
                            NombreUsuario = dr["NombreUsuario"]?.ToString() ?? "",
                            CorreoUsuario = dr["CorreoUsuario"]?.ToString() ?? ""
                        };
                    }
                }
            }
            return obj;
        }
    }
}
