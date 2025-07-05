namespace HostalIslaAzul.Domain.Entities
{
    public class AmaDeLlaves
    {
        public int Id { get; set; }
        public string NombreApellidos { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public string NumeroTelefonico { get; set; } = string.Empty;
        public List<HabitacionAmaDeLlaves> HabitacionesAtendidas { get; set; } = new();
    }
}