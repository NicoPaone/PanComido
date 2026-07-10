using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PanComido.Presentacion.Servicios
{
    public class MonitorExpiracionFilaBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MonitorExpiracionFilaBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            Console.WriteLine("MonitorExpiracionFilaBackgroundService INICIADO");
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Ejecutando ExpirarTurnosVencidos...");
                    using var scope = _serviceProvider.CreateScope();
                    var expirarCasoDeUso = scope.ServiceProvider.GetRequiredService<IExpirarTurnosVencidosCasoDeUso>();
                    await expirarCasoDeUso.EjecutarAsync();
                    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] ExpirarTurnosVencidos ejecutado OK.");
                }
                catch (Exception ex)
                {
                    // Log silencioso o ignorar, lo importante es que no se rompa el loop
                    Console.WriteLine($"Error en MonitorExpiracionFilaBackgroundService: {ex.Message}");
                }
            }
        }
    }
}
