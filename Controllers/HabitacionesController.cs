using Microsoft.AspNetCore.Mvc;
using HostalIslaAzul.Application.Services;
using System.Threading.Tasks;

namespace HostalIslaAzul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionesController : ControllerBase
    {
        private readonly HabitacionService _habitacionService;

        public HabitacionesController(HabitacionService habitacionService)
        {
            _habitacionService = habitacionService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearHabitacion([FromBody] HabitacionDto dto)
        {
            try
            {
                var habitacion = await _habitacionService.CrearHabitacionAsync(dto);
                return CreatedAtAction(nameof(ObtenerHabitacion), new { id = habitacion.Id }, habitacion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerHabitacion(int id)
        {
            try
            {
                var habitacion = await _habitacionService.ObtenerHabitacionAsync(id);
                if (habitacion == null) return NotFound();
                return Ok(habitacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarHabitacion(int id, [FromBody] HabitacionDto dto)
        {
            try
            {
                var habitacion = await _habitacionService.ModificarHabitacionAsync(id, dto);
                return Ok(habitacion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarHabitacion(int id)
        {
            try
            {
                await _habitacionService.EliminarHabitacionAsync(id);
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
        }

        [HttpPost("{id}/fuera-de-servicio")]
        public async Task<IActionResult> PonerFueraDeServicio(int id)
        {
            try
            {
                await _habitacionService.PonerFueraDeServicioAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarHabitaciones([FromQuery] string? busqueda, [FromQuery] bool? estaFueraDeServicio, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 10)
        {
            try
            {
                var habitaciones = await _habitacionService.ListarHabitacionesAsync(busqueda, estaFueraDeServicio, pagina, tamanoPagina);
                return Ok(habitaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("habitaciones-disponibles")]
        public async Task<IActionResult> ObtenerHabitacionesDisponibles([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var habitaciones = await _habitacionService.ObtenerHabitacionesDisponiblesAsync(fechaInicio, fechaFin);
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

    }
}