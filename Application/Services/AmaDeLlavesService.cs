using HostalIslaAzul.Domain.Entities;
using HostalIslaAzul.Application.Dtos;
using HostalIslaAzul.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HostalIslaAzul.Application.Services
{
    public class AmaDeLlavesService
    {
        private readonly HostalDbContext _context;

        public AmaDeLlavesService(HostalDbContext context)
        {
            _context = context;
        }

        
        public async Task<AmaDeLlaves> CrearAmaDeLlavesAsync(AmaDeLlavesDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreApellidos))
                throw new ArgumentException("El nombre y apellidos son requeridos.");
            if (string.IsNullOrWhiteSpace(dto.CI) || dto.CI.Length != 11)
                throw new ArgumentException("El CI debe tener 11 caracteres.");
            if (await _context.AmasDeLlaves.AnyAsync(a => a.CI == dto.CI))
                throw new ArgumentException("El CI ya está registrado.");
            if (string.IsNullOrWhiteSpace(dto.NumeroTelefonico))
                throw new ArgumentException("El número telefónico es requerido.");

            var ama = new AmaDeLlaves
            {
                NombreApellidos = dto.NombreApellidos,
                CI = dto.CI,
                NumeroTelefonico = dto.NumeroTelefonico
            };

            _context.AmasDeLlaves.Add(ama);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Creación", "AmasDeLlaves", ama.Id.ToString(), $"Ama de llaves creada: {ama.NombreApellidos}");
            return ama;
        }

        public async Task<AmaDeLlaves> ModificarAmaDeLlavesAsync(int id, AmaDeLlavesDto dto)
        {
            var ama = await _context.AmasDeLlaves.FindAsync(id);
            if (ama == null)
                throw new ArgumentException("Ama de llaves no encontrada.");

            if (string.IsNullOrWhiteSpace(dto.NombreApellidos))
                throw new ArgumentException("El nombre y apellidos son requeridos.");
            if (string.IsNullOrWhiteSpace(dto.CI) || dto.CI.Length != 11)
                throw new ArgumentException("El CI debe tener 11 caracteres.");
            if (await _context.AmasDeLlaves.AnyAsync(a => a.CI == dto.CI && a.Id != id))
                throw new ArgumentException("El CI ya está registrado para otra ama de llaves.");
            if (string.IsNullOrWhiteSpace(dto.NumeroTelefonico))
                throw new ArgumentException("El número telefónico es requerido.");

            ama.NombreApellidos = dto.NombreApellidos;
            ama.CI = dto.CI;
            ama.NumeroTelefonico = dto.NumeroTelefonico;

            _context.AmasDeLlaves.Update(ama);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Modificación", "AmasDeLlaves", ama.Id.ToString(), $"Ama de llaves modificada: {ama.NombreApellidos}");
            return ama;
        }

        public async Task EliminarAmaDeLlavesAsync(int id)
        {
            var ama = await _context.AmasDeLlaves.FindAsync(id);
            if (ama == null)
                throw new ArgumentException("Ama de llaves no encontrada.");

            if (await _context.HabitacionesAmasDeLlaves.AnyAsync(h => h.AmaDeLlavesId == id))
                throw new InvalidOperationException("No se puede eliminar una ama de llaves con habitaciones asignadas.");

            _context.AmasDeLlaves.Remove(ama);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Eliminación", "AmasDeLlaves", id.ToString(), $"Ama de llaves eliminada: {ama.NombreApellidos}");
        }

        public async Task<AmaDeLlaves?> ObtenerAmaDeLlavesAsync(int id)
        {
            return await _context.AmasDeLlaves.FindAsync(id);
        }

        public async Task<List<AmaDeLlaves>> ListarAmasDeLlavesAsync(string? busqueda, string? ciFiltro, int pagina, int tamanoPagina)
        {
            var query = _context.AmasDeLlaves.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();
                query = query.Where(a => a.NombreApellidos.ToLower().Contains(busqueda) ||
                                         a.CI.ToLower().Contains(busqueda) ||
                                         a.NumeroTelefonico.ToLower().Contains(busqueda));
            }

            if (!string.IsNullOrWhiteSpace(ciFiltro))
            {
                query = query.Where(a => a.CI == ciFiltro);
            }

            return await query
                .OrderBy(a => a.NombreApellidos)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();
        }

        public async Task AsignarHabitacionAsync(int amaDeLlavesId, string habitacionNumero, string turno)
        {
            if (string.IsNullOrWhiteSpace(habitacionNumero))
                throw new ArgumentException("El número de habitación es requerido.");
            if (amaDeLlavesId <= 0)
                throw new ArgumentException("El ID de la ama de llaves debe ser válido.");
            if (string.IsNullOrWhiteSpace(turno))
                throw new ArgumentException("El turno es requerido.");

            var turnosValidos = new[] { "Mañana", "Tarde", "Noche" };
            if (!turnosValidos.Contains(turno, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"El turno '{turno}' no es válido. Turnos permitidos: {string.Join(", ", turnosValidos)}.");

            Log.Information("Asignando habitación {HabitacionNumero} a ama de llaves {AmaDeLlavesId} en turno de la {Turno}",
                habitacionNumero, amaDeLlavesId, turno);

            var habitacion = await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero == habitacionNumero);
            if (habitacion == null)
            {
                Log.Error("Habitación no encontrada para Numero: {HabitacionNumero}", habitacionNumero);
                throw new ArgumentException($"La habitación {habitacionNumero} no existe.");
            }
            if (habitacion.EstaFueraDeServicio)
                throw new ArgumentException("La habitación no está disponible.");

            var amaDeLlaves = await _context.AmasDeLlaves.FindAsync(amaDeLlavesId);
            if (amaDeLlaves == null)
            {
                Log.Error("Ama de llaves no encontrada para Id: {AmaDeLlavesId}", amaDeLlavesId);
                throw new ArgumentException($"La ama de llaves con ID {amaDeLlavesId} no existe.");
            }

            var asignacionExistente = await _context.HabitacionesAmasDeLlaves
                .AnyAsync(h => h.HabitacionId == habitacion.Id && h.AmaDeLlavesId == amaDeLlavesId && h.Turno == turno);

            if (asignacionExistente)
                throw new InvalidOperationException($"La habitación {habitacionNumero} ya está asignada a la ama de llaves {amaDeLlaves.NombreApellidos} en el turno de la {turno}.");

            var asignacion = new HabitacionAmaDeLlaves
            {
                HabitacionId = habitacion.Id,
                AmaDeLlavesId = amaDeLlavesId,
                Turno = turno
            };

            _context.HabitacionesAmasDeLlaves.Add(asignacion);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex, "Error al guardar la asignación: {Message}", ex.Message);
                throw new InvalidOperationException("Error al guardar la asignación. Verifica los datos y la configuración de la base de datos.");
            }

            await RegistrarTrazaAsync("Asignación", "HabitacionesAmasDeLlaves",
                $"{habitacion.Id}-{amaDeLlavesId}-{turno}",
                $"Habitación {habitacionNumero} asignada a ama de llaves {amaDeLlavesId} en turno {turno}");
        }

        public async Task DesasignarHabitacionAsync(int amaDeLlavesId, string habitacionNumero)
        {
            if (string.IsNullOrWhiteSpace(habitacionNumero))
                throw new ArgumentException("El número de habitación es requerido.");
            if (amaDeLlavesId <= 0)
                throw new ArgumentException("El ID de la ama de llaves debe ser válido.");

            var habitacion = await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero == habitacionNumero);
            if (habitacion == null)
                throw new ArgumentException($"La habitación {habitacionNumero} no existe.");

            var asignacion = await _context.HabitacionesAmasDeLlaves
                .FirstOrDefaultAsync(h => h.HabitacionId == habitacion.Id && h.AmaDeLlavesId == amaDeLlavesId);
            if (asignacion == null)
                throw new ArgumentException($"No existe una asignación para la habitación {habitacionNumero} y la ama de llaves {amaDeLlavesId}");

            _context.HabitacionesAmasDeLlaves.Remove(asignacion);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Desasignación", "HabitacionesAmasDeLlaves",
                $"{habitacion.Id}-{amaDeLlavesId}", $"Habitación {habitacionNumero} desasignada de ama de llaves {amaDeLlavesId}");
        }

        public async Task<HabitacionAmaDeLlaves[]> ObtenerHabitacionesPorAmaDeLlavesAsync(int amaDeLlavesId)
        {
            return await _context.HabitacionesAmasDeLlaves
                .Where(h => h.AmaDeLlavesId == amaDeLlavesId)
                .Include(h => h.Habitacion)
                .ToArrayAsync();
        }

        private async Task RegistrarTrazaAsync(string operacion, string tabla, string registroId, string detalles)
        {
            var traza = new Traza
            {
                Operacion = operacion,
                TablaAfectada = tabla,
                RegistroId = registroId,
                Fecha = DateTime.UtcNow,
                Detalles = detalles
            };
            _context.Trazas.Add(traza);
            await _context.SaveChangesAsync();
            Log.Information("Traza registrada: {Operacion} en {Tabla} (ID: {RegistroId})", operacion, tabla, registroId);
        }
    }
}