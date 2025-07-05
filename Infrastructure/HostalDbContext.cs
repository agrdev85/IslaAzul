using Microsoft.EntityFrameworkCore;
using HostalIslaAzul.Domain.Entities;

namespace HostalIslaAzul.Infrastructure
{
    public class HostalDbContext : DbContext
    {
        public HostalDbContext(DbContextOptions<HostalDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<AmaDeLlaves> AmasDeLlaves { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<HabitacionAmaDeLlaves> HabitacionesAmasDeLlaves { get; set; }
        public DbSet<Traza> Trazas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar clave primaria para Habitacion
            modelBuilder.Entity<Habitacion>()
                .HasKey(h => h.Id);
            modelBuilder.Entity<Habitacion>()
                .HasIndex(h => h.Numero)
                .IsUnique();

            // Configurar clave primaria para AmaDeLlaves
            modelBuilder.Entity<AmaDeLlaves>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<AmaDeLlaves>()
                .HasIndex(a => a.CI)
                .IsUnique();

            // Configurar clave compuesta para HabitacionAmaDeLlaves
            modelBuilder.Entity<HabitacionAmaDeLlaves>()
                .HasKey(h => new { h.AmaDeLlavesId, h.HabitacionId, h.Turno });

            // Configurar relaciones para HabitacionAmaDeLlaves
            modelBuilder.Entity<HabitacionAmaDeLlaves>()
                .HasOne(h => h.Habitacion)
                .WithMany()
                .HasForeignKey(h => h.HabitacionId);

            modelBuilder.Entity<HabitacionAmaDeLlaves>()
                .HasOne(h => h.AmaDeLlaves)
                .WithMany()
                .HasForeignKey(h => h.AmaDeLlavesId);

            // Configurar relaciones para Reserva
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Habitacion)
                .WithMany(h => h.Reservas)
                .HasForeignKey(r => r.HabitacionId);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.ClienteId);

            // Índice para HabitacionNumero en Reserva
            modelBuilder.Entity<Reserva>()
                .HasIndex(r => r.HabitacionNumero);
        }
    }
}