using System.Collections.Generic;
using Capa.Datos;
using Capa.Entidades;
using Microsoft.Extensions.Configuration;

namespace Capa.Negocio
{
    public class MantenimientoNegocio
    {
        private readonly MantenimientoDAO _dao;

        public MantenimientoNegocio(IConfiguration config)
        {
            _dao = new MantenimientoDAO(config);
        }

        public List<Mantenimiento> ListarActivos() => _dao.ListarActivos();
        public void CompletarMantenimiento(int id, decimal costo) => _dao.CompletarMantenimiento(id, costo);
    }
}
