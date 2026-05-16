using Capa.Datos;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Negocio
{
    public class PrestamoNegocio
    {
        private readonly PrestamoDAO _dao;

        public PrestamoNegocio(IConfiguration config)
        {
            _dao = new PrestamoDAO(config);
        }

        public List<Equipo> ObtenerEquiposDisponibles() => _dao.ListarDisponibles();

        public List<Equipo> ObtenerEquiposPorTipo(string tipo) => _dao.ListarPorTipo(tipo);

        public string RegistrarSolicitud(int equipoID, int usuarioID, DateTime fechaFin)
            => _dao.SolicitarPrestamo(equipoID, usuarioID, fechaFin);

        public List<Prestamo> ObtenerSolicitudesPendientes() => _dao.ListarPendientes();

        public List<Prestamo> ObtenerSolicitudesUsuario(int usuarioID) => _dao.ListarPorUsuario(usuarioID);

        public string AprobarSolicitud(int prestamoID) => _dao.AprobarSolicitud(prestamoID);

        public string RechazarSolicitud(int prestamoID) => _dao.RechazarSolicitud(prestamoID);

        public string EntregarEquipo(int prestamoID)
            => _dao.EntregarEquipo(prestamoID);

        public string DevolverEquipo(int prestamoID, DateTime fecha)
            => _dao.DevolverEquipo(prestamoID, fecha);
        public List<Prestamo> ObtenerEnUso() => _dao.ListarEnUso();
    }
}
