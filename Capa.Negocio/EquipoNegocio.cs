using Capa.Datos;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Negocio
{
    public class EquipoNegocio
    {
        private readonly EquipoDAO _dao;

        public EquipoNegocio(IConfiguration config)
        {
            _dao = new EquipoDAO(config);
        }
        public List<Equipo> ObtenerDisponibles() => _dao.ListarDisponibles();
        public string NuevoEquipo(Equipo reg) => _dao.Registrar(reg);
        public Equipo ObtenerPorId(int id) => _dao.BuscarPorId(id);

        public string EditarEquipo(Equipo reg) => _dao.Actualizar(reg);
        public List<Equipo> ObtenerTodo() => _dao.ListarTodo();
        public List<Equipo> FiltrarEquipos(string tipo) => _dao.ListarPorTipo(tipo);
        public string EliminarEquipo(int id) => _dao.Eliminar(id);
        public Dictionary<string, object> ObtenerKPIs() => _dao.ObtenerKPIs();
        public List<Dictionary<string, object>> ObtenerActividadReciente() => _dao.ListarActividadReciente();
    }
}
