namespace HostalIslaAzul.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string NombreApellidos { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public string NumeroTelefonico { get; set; } = string.Empty;
        public bool EsVip { get; set; } // Para el descuento del 10%
        public List<Reserva> Reservas { get; set; } = new();
    }
}