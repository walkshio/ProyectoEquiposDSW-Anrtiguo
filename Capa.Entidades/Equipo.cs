namespace Capa.Entidades
{
    public class Equipo
    {
        public int EquipoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = "Disponible"; 
        public string? Imagen { get; set; }
    }
}