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

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Abrimos un scope para resolver el caso de uso scoped dentro del singleton del HostedService
                using var scope = _serviceProvider.CreateScope();
                
                var expirarCasoDeUso = scope.ServiceProvider.GetRequiredService<IExpirarTurnosVencidosCasoDeUso>();

                // Delegamos toda la lógica de reglas de negocio, EF y notificaciones al Dominio/CasosDeUso
                await expirarCasoDeUso.EjecutarAsync();
            }
        }
    }
}
