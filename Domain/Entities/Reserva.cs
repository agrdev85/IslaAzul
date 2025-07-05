namespace HostalIslaAzul.Domain.Entities
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateTime FechaReservacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal Importe { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!; // EF Core will initialize
        public int HabitacionId { get; set; }
        public Habitacion Habitacion { get; set; } = null!; // EF Core will initialize
        public string HabitacionNumero { get; set; } = string.Empty; // Initialize as empty string
        public bool EstaElClienteEnHostal { get; set; }
        public bool EstaCancelada { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
    }
}