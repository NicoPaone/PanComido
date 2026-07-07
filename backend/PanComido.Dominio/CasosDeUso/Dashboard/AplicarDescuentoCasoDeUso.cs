using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class AplicarDescuentoCasoDeUso
    {
        private readonly IArticuloRepositorio _articuloRepositorio;
        private readonly ISugerenciaIARepositorio _sugerenciaIARepositorio;
        private readonly ICalculadorCostoPlatoServicio _calculadorCostoPlatoServicio;
        private readonly IPoliticaDescuentoDashboardServicio _politicaDescuentoDashboardServicio;
        private readonly ITransaccionPersistenciaServicio _transaccionPersistenciaServicio;

        public AplicarDescuentoCasoDeUso(
            IArticuloRepositorio articuloRepositorio,
            ISugerenciaIARepositorio sugerenciaIARepositorio,
            ICalculadorCostoPlatoServicio calculadorCostoPlatoServicio,
            IPoliticaDescuentoDashboardServicio politicaDescuentoDashboardServicio,
            ITransaccionPersistenciaServicio transaccionPersistenciaServicio)
        {
            _articuloRepositorio = articuloRepositorio;
            _sugerenciaIARepositorio = sugerenciaIARepositorio;
            _calculadorCostoPlatoServicio = calculadorCostoPlatoServicio;
            _politicaDescuentoDashboardServicio = politicaDescuentoDashboardServicio;
            _transaccionPersistenciaServicio = transaccionPersistenciaServicio;
        }

        public async Task<AplicarDescuentoResultado?> EjecutarAsync(int restauranteId, int platoId, decimal porcentajeDescuento)
        {
            var politica = await _politicaDescuentoDashboardServicio.ObtenerAsync(restauranteId);

            if (porcentajeDescuento <= 0 || porcentajeDescuento > politica.PorcentajeDescuentoMaximo)
            {
                throw new ArgumentException($"El descuento debe ser mayor a 0 y menor o igual a {politica.PorcentajeDescuentoMaximo:N0}%.");
            }

            var articulo = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, platoId);
            var plato = articulo as Plato;
            if (plato == null)
            {
                return null;
            }

            decimal precioOriginal = plato.PrecioVentaFinal ?? 0m;
            if (precioOriginal <= 0)
            {
                throw new InvalidOperationException("El plato no tiene un precio de venta valido para aplicar descuentos.");
            }

            decimal descuento = precioOriginal * (porcentajeDescuento / 100);
            decimal precioNuevo = precioOriginal - descuento;
            decimal costoPreparacion = await _calculadorCostoPlatoServicio.CalcularCostoAsync(plato);
            decimal nuevoMargen = precioNuevo > 0 ? ((precioNuevo - costoPreparacion) / precioNuevo) * 100 : 0m;

            if (precioNuevo <= 0)
            {
                throw new InvalidOperationException("El descuento deja el precio final en cero o negativo.");
            }

            if (nuevoMargen < politica.MargenMinimoPermitido)
            {
                throw new InvalidOperationException($"El descuento deja un margen de {nuevoMargen:N0}%, menor al minimo permitido de {politica.MargenMinimoPermitido:N0}%.");
            }

            await _transaccionPersistenciaServicio.EjecutarAsync(async () =>
            {
                plato.PrecioVentaFinal = precioNuevo;
                await _articuloRepositorio.ActualizarAsync(plato);

                var sugerenciaIa = await _sugerenciaIARepositorio.ObtenerSugerenciaIAAsync(restauranteId);
                if (sugerenciaIa != null && sugerenciaIa.PlatosAnalisis != null)
                {
                    var analisisPlato = sugerenciaIa.PlatosAnalisis.FirstOrDefault(p => p.PlatoId == platoId);
                    if (analisisPlato != null)
                    {
                        var sugDescuento = analisisPlato.Sugerencias
                            .FirstOrDefault(s => s.Tipo.Equals(PanComido.Dominio.Entidades.Enums.TipoSugerencia.Descuento.ToString(), StringComparison.OrdinalIgnoreCase)
                                              || s.Tipo.Equals("descuento", StringComparison.OrdinalIgnoreCase));
                        if (sugDescuento != null)
                        {
                            sugDescuento.Aplicada = true;
                        }
                        await _sugerenciaIARepositorio.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa);
                    }
                }
            });

            return new AplicarDescuentoResultado
            {
                Mensaje = $"¡Descuento del {porcentajeDescuento:G0}% aplicado exitosamente!",
                PlatoId = platoId,
                PrecioNuevo = precioNuevo,
                Costo = costoPreparacion,
                MargenPctNuevo = $"{nuevoMargen:N0}%"
            };
        }
    }

    public class AplicarDescuentoResultado
    {
        public string Mensaje { get; set; } = string.Empty;
        public int PlatoId { get; set; }
        public decimal PrecioNuevo { get; set; }
        public decimal Costo { get; set; }
        public string MargenPctNuevo { get; set; } = string.Empty;
    }
}
