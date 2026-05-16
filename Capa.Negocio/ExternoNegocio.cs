using Capa.Datos;
using Capa.Entidades;


namespace Capa.Negocio
{
    public class ExternoNegocio
    {
        private readonly ExternoDAO _dao;

        public ExternoNegocio()
        {
            _dao = new ExternoDAO();
        }

        public async Task<TipoCambio> GetDolar() => await _dao.ObtenerDolar();
    }
}
