using System;

namespace Capa.Entidades
{
    public class Mantenimiento
    {
        public int MantenimientoID { get; set; }
        public int EquipoID { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public string Tecnico { get; set; } = string.Empty;
        public string Estado { get; set; } = "EnProceso";
        public string NombreEquipo { get; set; } = string.Empty;
    }
}
