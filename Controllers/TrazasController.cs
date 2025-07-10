using Microsoft.AspNetCore.Mvc;
using HostalIslaAzul.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HostalIslaAzul.Controllers
{
    

    [ApiController]
    [Route("api/[controller]")]
    public class TrazasController : ControllerBase
    {
        private readonly ITrazaService _trazaService;
        private readonly ILogger<TrazasController> _logger;

        public TrazasController(ITrazaService trazaService, ILogger<TrazasController> logger)
        {
            _trazaService = trazaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una lista de trazas con filtros, paginación y ordenamiento.
        /// </summary>
        /// <param name="busqueda">Término de búsqueda para Operación, TablaAfectada o Detalles.</param>
        /// <param name="tablaAfectada">Filtra por la tabla afectada (por ejemplo, Cliente, Habitacion).</param>
        /// <param name="pagina">Número de página (por defecto: 1).</param>
        /// <param name="tamanoPagina">Cantidad de registros por página (por defecto: 15).</param>
        /// <param name="ordenarPor">Campo para ordenar (Id, Operacion, TablaAfectada, RegistroId, Fecha, Detalles).</param>
        /// <param name="descendente">Orden descendente (true) o ascendente (false).</param>
        /// <returns>Lista de trazas y el total de registros.</returns>
        /// <response code="200">Devuelve la lista de trazas y el total de registros.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerTrazas(
            [FromQuery] string busqueda = "",
            [FromQuery] string tablaAfectada = "",
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 15,
            [FromQuery] string ordenarPor = "Id",
            [FromQuery] bool descendente = true)
        {
            try
            {
                var (data, total) = await _trazaService.ObtenerTrazasAsync(busqueda, tablaAfectada, pagina, tamanoPagina, ordenarPor, descendente);
                return Ok(new { data, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener trazas");
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }
    }
}