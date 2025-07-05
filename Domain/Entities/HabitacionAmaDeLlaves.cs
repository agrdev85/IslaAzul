namespace HostalIslaAzul.Domain.Entities
{
    public class HabitacionAmaDeLlaves
    {
        public int AmaDeLlavesId { get; set; }
        public int HabitacionId { get; set; } // Cambiar a HabitacionId
        public string Turno { get; set; } = string.Empty;
        public AmaDeLlaves AmaDeLlaves { get; set; } = null!;
        public Habitacion Habitacion { get; set; } = null!;
    }
}
