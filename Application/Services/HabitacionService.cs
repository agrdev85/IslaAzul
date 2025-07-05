using HostalIslaAzul.Domain.Entities;
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
    public class HabitacionService
    {
        private readonly HostalDbContext _context;

        public HabitacionService(HostalDbContext context)
        {
            _context = context;
        }

        public async Task<Habitacion> CrearHabitacionAsync(HabitacionDto dto)
        {
            // Validaciones
            if (!Regex.IsMatch(dto.Numero, @"^0[1-3][1-5]$"))
                throw new ArgumentException("El número de habitación debe tener el formato 0XY (X: 1-3, Y: 1-5, ej. 011).");
            if (await _context.Habitaciones.AnyAsync(h => h.Numero == dto.Numero))
                throw new ArgumentException("El número de habitación ya está registrado.");

            var habitacion = new Habitacion
            {
                Numero = dto.Numero,
                EstaFueraDeServicio = dto.EstaFueraDeServicio
            };

            _context.Habitaciones.Add(habitacion);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Creación", "Habitaciones", habitacion.Id.ToString(), $"Habitación creada: {habitacion.Numero}");
            return habitacion;
        }

        public async Task<Habitacion> ModificarHabitacionAsync(int id, HabitacionDto dto)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null)
                throw new ArgumentException("Habitación no encontrada.");

            // Validaciones
            if (!Regex.IsMatch(dto.Numero, @"^0[1-3][1-5]$"))
                throw new ArgumentException("El número de habitación debe tener el formato 0XY (X: 1-3, Y: 1-5, ej. 011).");
            if (await _context.Habitaciones.AnyAsync(h => h.Numero == dto.Numero && h.Id != id))
                throw new ArgumentException("El número de habitación ya está registrado para otra habitación.");

            habitacion.Numero = dto.Numero;
            habitacion.EstaFueraDeServicio = dto.EstaFueraDeServicio;

            _context.Habitaciones.Update(habitacion);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Modificación", "Habitaciones", habitacion.Id.ToString(), $"Habitación modificada: {habitacion.Numero}");
            return habitacion;
        }

        public async Task EliminarHabitacionAsync(int id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null)
                throw new ArgumentException("Habitación no encontrada.");

            // Validar que no tenga reservas activas
            if (await _context.Reservas.AnyAsync(r => r.HabitacionNumero == habitacion.Numero && !r.EstaCancelada))
                throw new InvalidOperationException("No se puede eliminar una habitación con reservas activas.");

            _context.Habitaciones.Remove(habitacion);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Eliminación", "Habitaciones", id.ToString(), $"Habitación eliminada: {habitacion.Numero}");
        }

        public async Task PonerFueraDeServicioAsync(int id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) throw new ArgumentException("Habitación no encontrada");

            var reservaActivaConCliente = await _context.Reservas
                .AnyAsync(r => r.HabitacionId == id && r.EstaElClienteEnHostal == true);
            if (reservaActivaConCliente)
                throw new InvalidOperationException("No se puede poner fuera de servicio una habitación con cliente presente");

            habitacion.EstaFueraDeServicio = true;
            await _context.SaveChangesAsync();
            await RegistrarTrazaAsync("Modificación", "Habitaciones", id.ToString(), $"Habitación {habitacion.Numero} puesta fuera de servicio");
        }

        public async Task<Habitacion?> ObtenerHabitacionAsync(int id)
        {
            return await _context.Habitaciones.FindAsync(id);
        }

        public async Task<List<string>> ObtenerHabitacionesDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");

            var habitacionesOcupadas = await _context.Reservas
                .Where(r => !r.EstaCancelada &&
                            r.EstaElClienteEnHostal &&
                            r.FechaEntrada <= fechaFin &&
                            r.FechaSalida >= fechaInicio)
                .Select(r => r.HabitacionId)
                .ToListAsync();

            return await _context.Habitaciones
                .Where(h => !h.EstaFueraDeServicio && !habitacionesOcupadas.Contains(h.Id))
                .Select(h => h.Numero)
                .ToListAsync();
        }

        public async Task<List<Habitacion>> ListarHabitacionesAsync(string? busqueda, bool? estaFueraDeServicio, int pagina, int tamanoPagina)
        {
            var query = _context.Habitaciones.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();
                query = query.Where(h => h.Numero.ToLower().Contains(busqueda));
            }

            if (estaFueraDeServicio.HasValue)
            {
                query = query.Where(h => h.EstaFueraDeServicio == estaFueraDeServicio.Value);
            }

            // Paginación
            return await query
                .OrderBy(h => h.Numero)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();
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

    public class HabitacionDto
    {
        public string Numero { get; set; } = string.Empty;
        public bool EstaFueraDeServicio { get; set; }
    }
}
