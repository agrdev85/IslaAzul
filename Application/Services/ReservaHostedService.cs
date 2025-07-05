using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostalIslaAzul.Application.Services
{
    public class ReservaHostedService : IHostedService, IDisposable
    {
        private readonly ReservaService _reservaService;
        private readonly ILogger<ReservaHostedService> _logger;
        private Timer _timer;

        public ReservaHostedService(ReservaService reservaService, ILogger<ReservaHostedService> logger)
        {
            _reservaService = reservaService;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Servicio de reservas iniciado a las {Time}", DateTimeOffset.Now);
            // Iniciar el timer para ejecutarse inmediatamente y luego cada minuto
            _timer = new Timer(ActualizarReservasExpiradas, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void ActualizarReservasExpiradas(object state)
        {
            try
            {
                var ahora = DateTime.Now;
                _logger.LogInformation("Verificando reservas expiradas a las {Time}", ahora);
                await _reservaService.ActualizarReservasExpiradasAsync();

             
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reservas expiradas");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Servicio de reservas detenido a las {Time}", DateTimeOffset.Now);
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}