using Microsoft.AspNetCore.Mvc;
using Serilog;
using HostalIslaAzul.Application.Services;
using HostalIslaAzul.Application.Dtos;
using System;
using System.Threading.Tasks;

namespace HostalIslaAzul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ReservaService _reservaService;
        private readonly AmaDeLlavesService _amaDeLlavesService;

        public ReservasController(ReservaService reservaService, AmaDeLlavesService amaDeLlavesService)
        {
            _reservaService = reservaService;
            _amaDeLlavesService = amaDeLlavesService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearReserva([FromBody] ReservaDto dto)
        {
            try
            {
                var reserva = await _reservaService.CrearReservaAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { id = reserva.Id }, reserva);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarReserva(int id, [FromBody] ReservaDto dto)
        {
            try
            {
                var reserva = await _reservaService.ModificarReservaAsync(id, dto);
                return Ok(reserva);
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

        [HttpPost("{id}/cancelar")]
        public async Task<IActionResult> CancelarReserva(int id, [FromBody] string motivoCancelacion)
        {
            try
            {
                await _reservaService.CancelarReservaAsync(id, motivoCancelacion);
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

        [HttpPost("{id}/cambiar-habitacion")]
        public async Task<IActionResult> CambiarHabitacion(int id, [FromBody] string nuevoNumeroHabitacion)
        {
            try
            {
                await _reservaService.CambiarHabitacionAsync(id, nuevoNumeroHabitacion);
                return NoContent();
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

        [HttpPost("{id}/registrar-llegada")]
        public async Task<IActionResult> RegistrarLlegada(int id)
        {
            try
            {
                await _reservaService.RegistrarLlegadaAsync(id);
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

        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerReservasActivasAsync([FromQuery] DateTime? fecha)
        {
            try
            {
                var reservas = await _reservaService.ObtenerReservasActivasAsync(fecha);
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _reservaService.ObtenerTodasReservasAsync();
            return Ok(reservas);
        }
        

            
  }
}