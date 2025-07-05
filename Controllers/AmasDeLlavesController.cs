using Microsoft.AspNetCore.Mvc;
using HostalIslaAzul.Application.Services;
using HostalIslaAzul.Application.Dtos;
using HostalIslaAzul.Domain.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HostalIslaAzul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmasDeLlavesController : ControllerBase
    {
        private readonly AmaDeLlavesService _amaDeLlavesService;

        public AmasDeLlavesController(AmaDeLlavesService amaDeLlavesService)
        {
            _amaDeLlavesService = amaDeLlavesService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CrearAmaDeLlaves([FromBody] AmaDeLlavesDto dto)
        {
            try
            {
                var ama = await _amaDeLlavesService.CrearAmaDeLlavesAsync(dto);
                return CreatedAtAction(nameof(ObtenerAmaDeLlaves), new { id = ama.Id }, ama);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerAmaDeLlaves(int id)
        {
            try
            {
                var ama = await _amaDeLlavesService.ObtenerAmaDeLlavesAsync(id);
                if (ama == null)
                    return NotFound("Ama de llaves no encontrada.");
                return Ok(ama);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarAmaDeLlaves(int id, [FromBody] AmaDeLlavesDto dto)
        {
            try
            {
                var ama = await _amaDeLlavesService.ModificarAmaDeLlavesAsync(id, dto);
                return Ok(ama);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAmaDeLlaves(int id)
        {
            try
            {
                await _amaDeLlavesService.EliminarAmaDeLlavesAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarAmasDeLlaves(
            [FromQuery] string? busqueda,
            [FromQuery] string? ciFiltro,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 10)
        {
            try
            {
                var amas = await _amaDeLlavesService.ListarAmasDeLlavesAsync(busqueda, ciFiltro, pagina, tamanoPagina);
                return Ok(amas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("habitaciones-por-ama-de-llaves/{amaDeLlavesId}")]
        public async Task<IActionResult> ObtenerHabitacionesPorAmaDeLlaves(int amaDeLlavesId)
        {
            try
            {
                var habitaciones = await _amaDeLlavesService.ObtenerHabitacionesPorAmaDeLlavesAsync(amaDeLlavesId);
                return Ok(habitaciones);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("asignar-habitacion-ama")]
        public async Task<IActionResult> AsignarHabitacionAma([FromBody] AsignacionHabitacionAmaDto dto)
        {
            try
            {
                await _amaDeLlavesService.AsignarHabitacionAsync(dto.AmaDeLlavesId, dto.HabitacionNumero, dto.Turno);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error en AsignarHabitacionAma: {Message}", ex.Message);
                return StatusCode(500, "Error interno del servidor. Por favor, intenta de nuevo.");
            }
        }

        [HttpPost("desasignar-habitacion")]
        public async Task<IActionResult> DesasignarHabitacion([FromBody] AsignacionHabitacionDto dto)
        {
            try
            {
                await _amaDeLlavesService.DesasignarHabitacionAsync(dto.AmaDeLlavesId, dto.HabitacionNumero);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class AsignacionHabitacionDto
        {
            public int AmaDeLlavesId { get; set; }
            public string HabitacionNumero { get; set; } = string.Empty;
        }
    }
}