using HostalIslaAzul.Domain.Entities;
using HostalIslaAzul.Infrastructure;
using HostalIslaAzul.Application.Dtos;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HostalIslaAzul.Application.Services
{
    public class ReservaService
    {
        private readonly HostalDbContext _context;
        private const decimal PrecioPorDia = 10m;
        private const decimal DescuentoVip = 0.9m;

        public ReservaService(HostalDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReservaActivaDto>> ObtenerTodasReservasAsync()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .ToListAsync();
            return reservas.Select(r => new ReservaActivaDto
            {
                Id = r.Id,
                FechaReservacion = r.FechaReservacion,
                FechaEntrada = r.FechaEntrada,
                FechaSalida = r.FechaSalida,
                Importe = r.Importe,
                ClienteNombre = r.Cliente?.NombreApellidos ?? "Desconocido",
                HabitacionNumero = r.HabitacionNumero ?? "N/A",
                EstaElClienteEnHostal = r.EstaElClienteEnHostal,
                EstaCancelada = r.EstaCancelada,
                FechaCancelacion = r.FechaCancelacion,
                MotivoCancelacion = r.MotivoCancelacion
            }).ToList();
        }

        public async Task<Reserva> CrearReservaAsync(ReservaDto dto)
        {
            // Validaciones
            if (dto.FechaEntrada < DateTime.Today)
                throw new ArgumentException("La fecha de entrada no puede ser anterior a hoy.");
            if (dto.FechaSalida <= dto.FechaEntrada)
                throw new ArgumentException("La fecha de salida debe ser posterior a la fecha de entrada.");
            var dias = (dto.FechaSalida - dto.FechaEntrada).Days + 1;
            if (dias < 3)
                throw new ArgumentException("La reserva debe ser de al menos 3 días.");

            // Validar formato de HabitacionNumero (0XY)
            if (!Regex.IsMatch(dto.HabitacionNumero, @"^0[1-3][1-5]$"))
                throw new ArgumentException("El número de habitación debe tener el formato 0XY (X: 1-3, Y: 1-5, ej. 011).");

            // Validar habitación disponible
            var habitacion = await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero == dto.HabitacionNumero);
            if (habitacion == null || habitacion.EstaFueraDeServicio)
                throw new ArgumentException("La habitación no está disponible.");

            // Validar conflictos de reservas (considerar EstaElClienteEnHostal)
            var conflicto = await _context.Reservas.AnyAsync(r =>
                r.HabitacionId == habitacion.Id &&
                !r.EstaCancelada &&
                r.EstaElClienteEnHostal && // Solo si el cliente está en el hostal
                r.FechaEntrada <= dto.FechaSalida &&
                r.FechaSalida >= dto.FechaEntrada);
            if (conflicto)
                throw new ArgumentException("La habitación ya está reservada en este período por un cliente presente.");

            // Validar que el cliente no tenga otra reserva en el mismo período
            var conflictoCliente = await _context.Reservas.AnyAsync(r =>
                r.ClienteId == dto.ClienteId &&
                !r.EstaCancelada &&
                r.EstaElClienteEnHostal &&
                r.FechaEntrada <= dto.FechaSalida &&
                r.FechaSalida >= dto.FechaEntrada);
            if (conflictoCliente)
                throw new ArgumentException("El cliente ya tiene una reserva activa en este período.");

            // Calcular importe
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado.");
            var importe = dias * PrecioPorDia * (cliente.EsVip ? DescuentoVip : 1m);

            var reserva = new Reserva
            {
                FechaReservacion = DateTime.UtcNow,
                FechaEntrada = dto.FechaEntrada,
                FechaSalida = dto.FechaSalida,
                Importe = importe,
                ClienteId = dto.ClienteId,
                HabitacionId = habitacion.Id,
                HabitacionNumero = dto.HabitacionNumero,
                EstaElClienteEnHostal = false,
                EstaCancelada = false
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Creación", "Reservas", reserva.Id.ToString(), $"Reserva creada para cliente {cliente.NombreApellidos}");
            return reserva;
        }

        public async Task<Reserva> ModificarReservaAsync(int id, ReservaDto dto)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
                throw new ArgumentException("Reserva no encontrada.");

            // Validaciones
            if (reserva.EstaElClienteEnHostal)
                throw new InvalidOperationException("No se puede modificar una reserva si el cliente ya está en el hostal.");
            if (DateTime.Today > reserva.FechaEntrada)
                throw new InvalidOperationException("No se puede modificar una reserva cuya fecha de entrada ya pasó.");

            if (dto.FechaEntrada < DateTime.Today)
                throw new ArgumentException("La fecha de entrada no puede ser anterior a hoy.");
            if (dto.FechaSalida <= dto.FechaEntrada)
                throw new ArgumentException("La fecha de salida debe ser posterior a la fecha de entrada.");
            var dias = (dto.FechaSalida - dto.FechaEntrada).Days + 1;
            if (dias < 3)
                throw new ArgumentException("La reserva debe ser de al menos 3 días.");

            // Validar formato de HabitacionNumero
            if (!Regex.IsMatch(dto.HabitacionNumero, @"^0[1-3][1-5]$"))
                throw new ArgumentException("El número de habitación debe tener el formato 0XY (X: 1-3, Y: 1-5, ej. 011).");

            // Validar habitación disponible
            var habitacion = await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero == dto.HabitacionNumero);
            if (habitacion == null || habitacion.EstaFueraDeServicio)
                throw new ArgumentException("La habitación no está disponible.");

            // Validar conflictos de reservas
            var conflicto = await _context.Reservas.AnyAsync(r =>
                r.HabitacionId == habitacion.Id &&
                r.Id != id &&
                !r.EstaCancelada &&
                r.EstaElClienteEnHostal &&
                r.FechaEntrada <= dto.FechaSalida &&
                r.FechaSalida >= dto.FechaEntrada);
            if (conflicto)
                throw new ArgumentException("La habitación ya está reservada en este período por un cliente presente.");

            // Validar que el cliente no tenga otra reserva
            var conflictoCliente = await _context.Reservas.AnyAsync(r =>
                r.ClienteId == dto.ClienteId &&
                r.Id != id &&
                !r.EstaCancelada &&
                r.EstaElClienteEnHostal &&
                r.FechaEntrada <= dto.FechaSalida &&
                r.FechaSalida >= dto.FechaEntrada);
            if (conflictoCliente)
                throw new ArgumentException("El cliente ya tiene una reserva activa en este período.");

            // Actualizar campos
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado.");

            reserva.FechaEntrada = dto.FechaEntrada;
            reserva.FechaSalida = dto.FechaSalida;
            reserva.ClienteId = dto.ClienteId;
            reserva.HabitacionId = habitacion.Id;
            reserva.HabitacionNumero = dto.HabitacionNumero;
            reserva.Importe = dias * PrecioPorDia * (cliente.EsVip ? DescuentoVip : 1m);

            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Modificación", "Reservas", reserva.Id.ToString(), $"Reserva modificada para cliente {cliente.NombreApellidos}");
            return reserva;
        }

        public async Task CancelarReservaAsync(int id, string motivoCancelacion)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
                throw new ArgumentException("Reserva no encontrada.");

            if (reserva.EstaElClienteEnHostal)
                throw new InvalidOperationException("No se puede cancelar una reserva si el cliente ya está en el hostal.");
            if (reserva.EstaCancelada)
                throw new InvalidOperationException("La reserva ya está cancelada.");
            if (string.IsNullOrWhiteSpace(motivoCancelacion))
                throw new ArgumentException("El motivo de cancelación es requerido.");

            reserva.EstaCancelada = true;
            reserva.FechaCancelacion = DateTime.UtcNow;
            reserva.MotivoCancelacion = motivoCancelacion;

            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Cancelación", "Reservas", reserva.Id.ToString(), $"Reserva cancelada para cliente {reserva.Cliente.NombreApellidos}. Motivo: {motivoCancelacion}");
        }

        

        public async Task CambiarHabitacionAsync(int reservaId, string nuevoNumeroHabitacion)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == reservaId);
            if (reserva == null)
                throw new ArgumentException("Reserva no encontrada.");

            // Validar formato de HabitacionNumero
            if (!Regex.IsMatch(nuevoNumeroHabitacion, @"^0[1-3][1-5]$"))
                throw new ArgumentException("El número de habitación debe tener el formato 0XY (X: 1-3, Y: 1-5, ej. 011).");

            // Validar habitación disponible
            var habitacion = await _context.Habitaciones
                .FirstOrDefaultAsync(h => h.Numero == nuevoNumeroHabitacion);
            if (habitacion == null || habitacion.EstaFueraDeServicio)
                throw new ArgumentException("La nueva habitación no está disponible.");

            // Validar conflictos de reservas
            var conflicto = await _context.Reservas.AnyAsync(r =>
                r.HabitacionId == habitacion.Id &&
                r.Id != reservaId &&
                !r.EstaCancelada &&
                r.EstaElClienteEnHostal &&
                r.FechaEntrada <= reserva.FechaSalida &&
                r.FechaSalida >= reserva.FechaEntrada);
            if (conflicto)
                throw new ArgumentException("La nueva habitación ya está reservada en este período por un cliente presente.");

            var antiguaHabitacion = reserva.HabitacionNumero;
            reserva.HabitacionId = habitacion.Id;
            reserva.HabitacionNumero = nuevoNumeroHabitacion;

            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Cambio de habitación", "Reservas", reserva.Id.ToString(),
                $"Cambio de habitación de {antiguaHabitacion} a {nuevoNumeroHabitacion} para cliente {reserva.Cliente.NombreApellidos}");
        }

        public async Task RegistrarLlegadaAsync(int reservaId)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == reservaId);
            if (reserva == null)
                throw new ArgumentException("Reserva no encontrada.");

            // Comparar solo la fecha (sin hora)
            if (DateTime.Today != reserva.FechaEntrada.Date)
                throw new InvalidOperationException("La llegada solo puede registrarse el día de entrada de la reserva.");
            if (reserva.EstaCancelada)
                throw new InvalidOperationException("No se puede registrar llegada en una reserva cancelada.");
            if (reserva.EstaElClienteEnHostal)
                throw new InvalidOperationException("El cliente ya está registrado como presente.");

            reserva.EstaElClienteEnHostal = true;

            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Registro de llegada", "Reservas", reserva.Id.ToString(),
                $"Cliente {reserva.Cliente.NombreApellidos} registrado en el hostal.");
        }

        
       public async Task<List<ReservaActivaDto>> ObtenerReservasActivasAsync(DateTime? fecha = null)
        {
            IQueryable<Reserva> query = _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Habitacion)
                .Where(r => !r.EstaCancelada && r.EstaElClienteEnHostal);

            // Si se proporciona una fecha, filtrar por FechaSalida > fecha
            if (fecha.HasValue)
            {
                var fechaActual = fecha.Value;
                query = query.Where(r => r.FechaSalida > fechaActual);
            }

            // Depuración: Verificar los datos antes del filtro de fecha
            var debugQuery = await query.ToListAsync();
            Log.Information("Reservas antes de filtro de fecha: {Count}, Ejemplo: Id={Id}, EstaElClienteEnHostal={EstaElClienteEnHostal}",
                debugQuery.Count, debugQuery.FirstOrDefault()?.Id, debugQuery.FirstOrDefault()?.EstaElClienteEnHostal);

            // Materializar los datos y luego mapear con manejo de nulos
            var reservas = await query.ToListAsync();
            var result = reservas.Select(r => new ReservaActivaDto
            {
                Id = r.Id,
                FechaReservacion = r.FechaReservacion,
                FechaEntrada = r.FechaEntrada,
                FechaSalida = r.FechaSalida,
                Importe = r.Importe,
                ClienteNombre = r.Cliente != null ? r.Cliente.NombreApellidos : "Desconocido",
                HabitacionNumero = r.HabitacionNumero ?? "N/A",
                EstaElClienteEnHostal = r.EstaElClienteEnHostal,
                EstaCancelada = r.EstaCancelada,
                FechaCancelacion = r.FechaCancelacion,
                MotivoCancelacion = r.MotivoCancelacion
            }).ToList();

            // Depuración: Verificar los datos mapeados
            Log.Information("Reservas mapeadas: {Count}, Ejemplo: Id={Id}, EstaElClienteEnHostal={EstaElClienteEnHostal}",
                result.Count, result.FirstOrDefault()?.Id, result.FirstOrDefault()?.EstaElClienteEnHostal);

            return result;
        }

        public async Task ActualizarReservasExpiradasAsync()
        {
            var fechaActual = DateTime.Today;
            var reservasExpiradas = await _context.Reservas
                .Where(r => r.EstaElClienteEnHostal && r.FechaSalida <= fechaActual)
                .ToListAsync();

            if (reservasExpiradas.Any())
            {
                foreach (var reserva in reservasExpiradas)
                {
                    reserva.EstaElClienteEnHostal = false;
                    Log.Information("Reserva {Id} marcada como expirada. Cliente {ClienteNombre} dejó la habitación {HabitacionNumero}. FechaSalida: {FechaSalida}",
                        reserva.Id, reserva.Cliente?.NombreApellidos ?? "Desconocido", reserva.HabitacionNumero, reserva.FechaSalida);
                }
                await _context.SaveChangesAsync();
                await RegistrarTrazaAsync("Expiración", "Reservas", string.Join(",", reservasExpiradas.Select(r => r.Id)),
                    $"Marcadas como expiradas {reservasExpiradas.Count} reservas el {fechaActual}");
            }
            else
            {
                Log.Information("No se encontraron reservas expiradas a las {Time} con fecha actual {FechaActual}", DateTime.Now, fechaActual);
            }
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

    public class ReservaDto
    {
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int ClienteId { get; set; }
        public string HabitacionNumero { get; set; } = string.Empty;
    }

    public class ReservaActivaDto
    {
        public int Id { get; set; }
        public DateTime FechaReservacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public decimal Importe { get; set; }
        public string? ClienteNombre { get; set; }
        public string? HabitacionNumero { get; set; }
        public bool EstaElClienteEnHostal { get; set; }
        public bool EstaCancelada { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
    }
}