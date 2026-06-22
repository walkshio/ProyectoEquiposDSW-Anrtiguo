using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Datos
{
    public class MantenimientoDAO
    {
        private readonly string _cadena;

        public MantenimientoDAO(IConfiguration config)
        {
            _cadena = config.GetConnectionString("DefaultConnection")!;
        }

        public List<Mantenimiento> ListarActivos()
        {
            var lista = new List<Mantenimiento>();
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spListarMantenimientosActivos", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Mantenimiento
                        {
                            MantenimientoID = Convert.ToInt32(dr["MantenimientoID"]),
                            EquipoID = Convert.ToInt32(dr["EquipoID"]),
                            FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                            FechaSalida = dr["FechaSalida"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr["FechaSalida"]),
                            Diagnostico = dr["Diagnostico"]?.ToString() ?? "",
                            Costo = Convert.ToDecimal(dr["Costo"]),
                            Tecnico = dr["Tecnico"]?.ToString() ?? "",
                            Estado = dr["Estado"]?.ToString() ?? "",
                            NombreEquipo = dr["NombreEquipo"]?.ToString() ?? ""
                        });
                    }
                }
            }
            return lista;
        }

        public void CompletarMantenimiento(int id, decimal costo)
        {
            using (SqlConnection cn = new SqlConnection(_cadena))
            {
                SqlCommand cmd = new SqlCommand("spCompletarMantenimiento", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@MantenimientoID", id);
                cmd.Parameters.AddWithValue("@Costo", costo);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
