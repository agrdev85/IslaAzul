using HostalIslaAzul.Application.Dtos;
using HostalIslaAzul.Application.Services;
using Microsoft.AspNetCore.Mvc;
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
            _reservaService = reservaService ?? throw new ArgumentNullException(nameof(reservaService));
            _amaDeLlavesService = amaDeLlavesService ?? throw new ArgumentNullException(nameof(amaDeLlavesService));
        }

    
        /// Obtiene todas las reservas.
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reservas = await _reservaService.ObtenerTodasReservasAsync();
                return Ok(new { success = true, data = reservas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al obtener las reservas", error = ex.Message });
            }
        }

        
        /// Obtiene las reservas activas, opcionalmente filtradas por fecha.
        
        [HttpGet("activas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerReservasActivasAsync([FromQuery] DateTime? fecha)
        {
            try
            {
                var reservas = await _reservaService.ObtenerReservasActivasAsync(fecha);
                return Ok(new { success = true, data = reservas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al obtener las reservas activas", error = ex.Message });
            }
        }

        
        /// Crea una nueva reserva.
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearReserva([FromBody] ReservaDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.HabitacionNumero) || dto.ClienteId <= 0)
            {
                return BadRequest(new { success = false, message = "Los datos de la reserva son inválidos." });
            }

            try
            {
                var reserva = await _reservaService.CrearReservaAsync(dto);
                var reservaDto = new ReservaActivaDto
                {
                    Id = reserva.Id,
                    FechaReservacion = reserva.FechaReservacion,
                    FechaEntrada = reserva.FechaEntrada,
                    FechaSalida = reserva.FechaSalida,
                    Importe = reserva.Importe,
                    ClienteNombre = reserva.Cliente?.NombreApellidos ?? "Desconocido",
                    HabitacionNumero = reserva.HabitacionNumero ?? "N/A",
                    EstaElClienteEnHostal = reserva.EstaElClienteEnHostal,
                    EstaCancelada = reserva.EstaCancelada,
                    FechaCancelacion = reserva.FechaCancelacion,
                    MotivoCancelacion = reserva.MotivoCancelacion
                };
                return CreatedAtAction(nameof(GetAll), new { id = reserva.Id }, new { success = true, data = reservaDto });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al crear la reserva", error = ex.Message });
            }
        }

    
        /// Modifica una reserva existente.
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModificarReserva(int id, [FromBody] ReservaDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.HabitacionNumero) || dto.ClienteId <= 0)
            {
                return BadRequest(new { success = false, message = "Los datos de la reserva son inválidos." });
            }

            try
            {
                var reserva = await _reservaService.ModificarReservaAsync(id, dto);
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        reserva.Id,
                        reserva.FechaEntrada,
                        reserva.FechaSalida,
                        reserva.Importe,
                        reserva.ClienteId,
                        reserva.HabitacionNumero,
                        reserva.EstaElClienteEnHostal,
                        reserva.EstaCancelada
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al modificar la reserva", error = ex.Message });
            }
        }

        /// Cancela una reserva existente.

        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelarReserva(int id, [FromBody] string motivoCancelacion)
        {
            if (string.IsNullOrWhiteSpace(motivoCancelacion))
            {
                return BadRequest(new { success = false, message = "El motivo de cancelación es requerido." });
            }

            try
            {
                await _reservaService.CancelarReservaAsync(id, motivoCancelacion);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al cancelar la reserva", error = ex.Message });
            }
        }

    
        /// Cambia la habitación de una reserva.
        
        [HttpPut("{id}/cambiar-habitacion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CambiarHabitacion(int id, [FromBody] string nuevoNumeroHabitacion)
        {
            if (string.IsNullOrWhiteSpace(nuevoNumeroHabitacion))
            {
                return BadRequest(new { success = false, message = "El número de habitación es requerido." });
            }

            try
            {
                await _reservaService.CambiarHabitacionAsync(id, nuevoNumeroHabitacion);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al cambiar la habitación", error = ex.Message });
            }
        }

        /// Registra la llegada de un cliente al hostal.
        
        [HttpPut("{id}/registrar-llegada")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarLlegada(int id)
        {
            try
            {
                await _reservaService.RegistrarLlegadaAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al registrar la llegada", error = ex.Message });
            }
        }

        /// Actualiza las reservas expiradas manualmente.
        
        [HttpPost("actualizar-expiradas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForzarActualizarExpiradas()
        {
            try
            {
                await _reservaService.ActualizarReservasExpiradasAsync();
                return Ok(new { success = true, message = "Reservas expiradas actualizadas manualmente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error al actualizar reservas expiradas", error = ex.Message });
            }
        }
    }
}