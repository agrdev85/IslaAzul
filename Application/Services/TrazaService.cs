using Microsoft.Extensions.Logging;
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
    
    public interface ITrazaService
    {
        Task<(List<TrazaDto> Data, int Total)> ObtenerTrazasAsync(string busqueda, string tablaAfectada, int pagina, int tamanoPagina, string ordenarPor, bool descendente);
    }

    public class TrazaService : ITrazaService
    {
        private readonly HostalDbContext _context; 
        private readonly ILogger<TrazaService> _logger;

        public TrazaService(HostalDbContext context, ILogger<TrazaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(List<TrazaDto> Data, int Total)> ObtenerTrazasAsync(
            string busqueda = "",
            string tablaAfectada = "",
            int pagina = 1,
            int tamanoPagina = 15,
            string ordenarPor = "Id",
            bool descendente = true)
        {
            try
            {
                var query = _context.Trazas.AsQueryable();

                // Filtro por búsqueda
                if (!string.IsNullOrEmpty(busqueda))
                {
                    busqueda = busqueda.ToLower();
                    query = query.Where(t =>
                        t.Operacion.ToLower().Contains(busqueda) ||
                        t.TablaAfectada.ToLower().Contains(busqueda) ||
                        t.Detalles.ToLower().Contains(busqueda));
                }

                // Filtro por tabla afectada
                if (!string.IsNullOrEmpty(tablaAfectada))
                {
                    query = query.Where(t => t.TablaAfectada == tablaAfectada);
                }

                
                int total = await query.CountAsync();

                
                query = ordenarPor switch
                {
                    "Operacion" => descendente ? query.OrderByDescending(t => t.Operacion) : query.OrderBy(t => t.Operacion),
                    "TablaAfectada" => descendente ? query.OrderByDescending(t => t.TablaAfectada) : query.OrderBy(t => t.TablaAfectada),
                    "RegistroId" => descendente ? query.OrderByDescending(t => t.RegistroId) : query.OrderBy(t => t.RegistroId),
                    "Fecha" => descendente ? query.OrderByDescending(t => t.Fecha) : query.OrderBy(t => t.Fecha),
                    "Detalles" => descendente ? query.OrderByDescending(t => t.Detalles) : query.OrderBy(t => t.Detalles),
                    _ => descendente ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id)
                };

                
                var trazas = await query
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .Select(t => new TrazaDto
                    {
                        Id = t.Id,
                        Operacion = t.Operacion,
                        TablaAfectada = t.TablaAfectada,
                        RegistroId = t.RegistroId,
                        Fecha = t.Fecha,
                        Detalles = t.Detalles
                    })
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} trazas con los parámetros: busqueda={Busqueda}, tablaAfectada={TablaAfectada}, pagina={Pagina}, tamanoPagina={TamanoPagina}", trazas.Count, busqueda, tablaAfectada, pagina, tamanoPagina);

                return (trazas, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener trazas");
                throw;
            }
        }
    }

    public class TrazaDto
    {
        public int Id { get; set; }
        public string Operacion { get; set; }
        public string TablaAfectada { get; set; }
        public string RegistroId { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalles { get; set; }
    }
}