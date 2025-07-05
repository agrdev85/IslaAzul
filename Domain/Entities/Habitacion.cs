namespace HostalIslaAzul.Domain.Entities
{
    public class Habitacion
    {
        public int Id { get; set; } // Clave primaria
        public string Numero { get; set; } = string.Empty; // Formato 0XY
        public bool EstaFueraDeServicio { get; set; }
        public List<Reserva> Reservas { get; set; } = new();
        public List<HabitacionAmaDeLlaves> AmasDeLlaves { get; set; } = new();
    }
}