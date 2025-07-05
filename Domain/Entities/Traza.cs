namespace HostalIslaAzul.Domain.Entities
{
    public class Traza
    {
        public int Id { get; set; }
        public string Operacion { get; set; } = string.Empty;
        public string TablaAfectada { get; set; } = string.Empty;
        public string RegistroId { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Detalles { get; set; } = string.Empty;
    }
}