    using Microsoft.AspNetCore.Mvc;
    using HostalIslaAzul.Application.Services;
    using HostalIslaAzul.Domain.Entities;
    using HostalIslaAzul.Infrastructure;
    using System;
    using System.Threading.Tasks;

 namespace HostalIslaAzul.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientesController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteDto dto)
        {
            try
            {
                var cliente = await _clienteService.CrearClienteAsync(dto);
                return CreatedAtAction(nameof(ObtenerCliente), new { id = cliente.Id }, cliente);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerCliente(int id)
        {
            try
            {
                var cliente = await _clienteService.ObtenerClienteAsync(id);
                if (cliente == null) return NotFound();
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarCliente(int id, [FromBody] ClienteDto dto)
        {
            try
            {
                var cliente = await _clienteService.ModificarClienteAsync(id, dto);
                return Ok(cliente);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            try
            {
                await _clienteService.EliminarClienteAsync(id);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarClientes([FromQuery] string? busqueda, [FromQuery] string? ciFiltro, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 10)
        {
            try
            {
                var clientes = await _clienteService.ListarClientesAsync(busqueda, ciFiltro, pagina, tamanoPagina);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}