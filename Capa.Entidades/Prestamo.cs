namespace Capa.Entidades
{
    public class Prestamo
    {
        public int PrestamoID { get; set; }
        public int DetalleID { get; set; }
        public int EquipoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public decimal Multa { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
        public string TipoEquipo { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string CorreoUsuario { get; set; } = string.Empty;
        public string? MotivoRechazo { get; set; }
        public string? Incidencia { get; set; }
        public decimal MultaDanio { get; set; }
    }
}
