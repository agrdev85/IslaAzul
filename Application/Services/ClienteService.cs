using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostalIslaAzul.Domain.Entities;
using HostalIslaAzul.Infrastructure;

namespace HostalIslaAzul.Application.Services
{

    public class ClienteService
    {
        private readonly HostalDbContext _context;

            public ClienteService(HostalDbContext context)
            {
                _context = context;
            }
        
    
        public async Task<Cliente> CrearClienteAsync(ClienteDto dto)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.NombreApellidos))
                throw new ArgumentException("El nombre y apellidos son requeridos.");
            if (string.IsNullOrWhiteSpace(dto.CI) || dto.CI.Length != 11)
                throw new ArgumentException("El CI debe tener 11 caracteres.");
            if (await _context.Clientes.AnyAsync(c => c.CI == dto.CI))
                throw new ArgumentException("El CI ya está registrado.");
            if (string.IsNullOrWhiteSpace(dto.NumeroTelefonico))
                throw new ArgumentException("El número telefónico es requerido.");

            var cliente = new Cliente
            {
                NombreApellidos = dto.NombreApellidos,
                CI = dto.CI,
                NumeroTelefonico = dto.NumeroTelefonico,
                EsVip = dto.EsVip
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Creación", "Clientes", cliente.Id.ToString(), $"Cliente creado: {cliente.NombreApellidos}");

            return cliente;
        }

        public async Task<Cliente> ModificarClienteAsync(int id, ClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado.");

            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.NombreApellidos))
                throw new ArgumentException("El nombre y apellidos son requeridos.");
            if (string.IsNullOrWhiteSpace(dto.CI) || dto.CI.Length != 11)
                throw new ArgumentException("El CI debe tener 11 caracteres.");
            if (await _context.Clientes.AnyAsync(c => c.CI == dto.CI && c.Id != id))
                throw new ArgumentException("El CI ya está registrado para otro cliente.");
            if (string.IsNullOrWhiteSpace(dto.NumeroTelefonico))
                throw new ArgumentException("El número telefónico es requerido.");

            // Actualizar datos
            cliente.NombreApellidos = dto.NombreApellidos;
            cliente.CI = dto.CI;
            cliente.NumeroTelefonico = dto.NumeroTelefonico;
            cliente.EsVip = dto.EsVip;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Modificación", "Clientes", cliente.Id.ToString(), $"Cliente modificado: {cliente.NombreApellidos}");

            return cliente;
        }

        public async Task EliminarClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                throw new ArgumentException("Cliente no encontrado.");

            // Validar que no tenga reservas activas
            if (await _context.Reservas.AnyAsync(r => r.ClienteId == id && !r.EstaCancelada))
                throw new InvalidOperationException("No se puede eliminar un cliente con reservas activas.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            await RegistrarTrazaAsync("Eliminación", "Clientes", id.ToString(), $"Cliente eliminado: {cliente.NombreApellidos}");
        }

        public async Task<Cliente?> ObtenerClienteAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task<List<Cliente>> ListarClientesAsync(string? busqueda, string? ciFiltro, int pagina, int tamanoPagina)
        {
            var query = _context.Clientes.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();
                query = query.Where(c => c.NombreApellidos.ToLower().Contains(busqueda) ||
                                        c.CI.ToLower().Contains(busqueda) ||
                                        c.NumeroTelefonico.ToLower().Contains(busqueda));
            }

            if (!string.IsNullOrWhiteSpace(ciFiltro))
            {
                query = query.Where(c => c.CI == ciFiltro);
            }

            // Paginación
            return await query
                .OrderBy(c => c.NombreApellidos)
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

    public class ClienteDto
    {
        public string NombreApellidos { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public string NumeroTelefonico { get; set; } = string.Empty;
        public bool EsVip { get; set; }
    }

}