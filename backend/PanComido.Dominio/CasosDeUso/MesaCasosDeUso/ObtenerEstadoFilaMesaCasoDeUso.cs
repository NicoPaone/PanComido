
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso.Resultados;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class ObtenerEstadoFilaMesaCasoDeUso
    {
        private readonly ITurnoFilaRepositorio _turnoFilaRepositorio;
        private readonly IMesaRepositorio _mesaRepositorio;
        private readonly ICacheServicio _cache;

        public ObtenerEstadoFilaMesaCasoDeUso(
            ITurnoFilaRepositorio turnoFilaRepositorio,
            IMesaRepositorio mesaRepositorio,
            ICacheServicio cache)
        {
            _turnoFilaRepositorio = turnoFilaRepositorio;
            _mesaRepositorio = mesaRepositorio;
            _cache = cache;
        }

        public async Task<EstadoFilaMesaResult> EjecutarAsync(int turnoId)
        {
            string cacheKey = $"EstadoFilaMesa_{turnoId}";
            
            if (_cache.TryGetValue(cacheKey, out EstadoFilaMesaResult cachedResult))
            {
                return cachedResult;
            }

            var turno = await _turnoFilaRepositorio.ObtenerPorIdAsync(turnoId);
            if (turno == null) throw new ArgumentException("Turno de fila no encontrado");

            if (turno.Estado == EstadoTurnoMesa.MesaAsignada && turno.FechaHoraAsignacion.HasValue)
            {
                var minutosDesdeAsignacion = (DateTime.UtcNow - turno.FechaHoraAsignacion.Value).TotalMinutes;
                var minutosRestantes = 7 - (int)minutosDesdeAsignacion; // Ajustado a 7 minutos de tolerancia

                var resultadoAsignado = new EstadoFilaMesaResult
                {
                    MesaLista = true,
                    MesaAsignadaId = turno.MesaAsignadaId,
                    MinutosRestantesParaOcupar = minutosRestantes > 0 ? minutosRestantes : 0,
                    TiempoEstimadoVisual = "¡Tu mesa está lista!"
                };
                
                _cache.Set(cacheKey, resultadoAsignado, TimeSpan.FromSeconds(15));
                return resultadoAsignado;
            }

            var turnosAdelante = await _turnoFilaRepositorio.ContarTurnosEnEsperaPreviosAsync(turno.FilaVirtualId, turno.FechaHoraIngreso);
            
            var filaVirtual = await _turnoFilaRepositorio.ObtenerFilaVirtualPorIdAsync(turno.FilaVirtualId);
            int tiempoPromedio = filaVirtual?.TiempoPromedioComidaMinutos ?? 40;

            var todasLasMesas = await _mesaRepositorio.ObtenerMesasParaFilaVirtualAsync(filaVirtual.RestauranteId);

            int tiempoEstimado = 15; // default fallback

            if (todasLasMesas.Any())
            {
                var maxCapacidadRestaurante = todasLasMesas.Max(m => m.CantPersonasMax);
                int capacidadBuscada = Math.Min(turno.CantidadComensales, maxCapacidadRestaurante);

                var mesasAptas = todasLasMesas
                    .Where(m => m.CantPersonasMax >= capacidadBuscada && m.EstadoMesa != EstadoMesa.Deshabilitada)
                    .ToList();
                
                if (!mesasAptas.Any()) mesasAptas = todasLasMesas;

                var tiemposRestantes = new List<int>();

                foreach (var mesa in mesasAptas)
                {
                    int minutosRestantes = 0; 

                    if (mesa.EstadoMesa == EstadoMesa.Ocupada)
                    {
                        minutosRestantes = tiempoPromedio;
                        if (mesa.HoraInicioComandaActiva.HasValue)
                        {
                            var minutosOcupada = (int)(DateTime.UtcNow - mesa.HoraInicioComandaActiva.Value).TotalMinutes;
                            minutosRestantes = tiempoPromedio - minutosOcupada;
                            if (minutosRestantes < 10) minutosRestantes = 10;
                        }
                    }
                    else if (mesa.EstadoMesa == EstadoMesa.Reservada)
                    {
                        minutosRestantes = tiempoPromedio;
                    }

                    tiemposRestantes.Add(minutosRestantes);
                }

                tiemposRestantes.Sort();

                int turnoPosicion = turnosAdelante + 1;
                int indiceMesa = (turnoPosicion - 1) % tiemposRestantes.Count;
                int ciclosCompletos = (turnoPosicion - 1) / tiemposRestantes.Count;

                tiempoEstimado = tiemposRestantes[indiceMesa] + (ciclosCompletos * tiempoPromedio);
            }

            int topeMaximo = 90; // Asumido hardcodeado por ahora
            string tiempoVisual = tiempoEstimado > topeMaximo 
                                  ? $"+{topeMaximo} minutos" 
                                  : $"{tiempoEstimado} minutos";

            var resultado = new EstadoFilaMesaResult
            {
                MesaLista = false,
                NumeroTurno = turno.Numero,
                TurnosAdelante = turnosAdelante,
                TiempoEstimadoMinutos = tiempoEstimado,
                TiempoEstimadoVisual = tiempoVisual
            };

            _cache.Set(cacheKey, resultado, TimeSpan.FromSeconds(15));

            return resultado;
        }
    }
}
