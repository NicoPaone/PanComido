using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.Dashboard
{
    public class AplicarDescuentoCasoDeUso
    {
        private readonly IArticuloRepositorio _articuloRepositorio;
        private readonly ISugerenciaIARepositorio _sugerenciaIARepositorio;
        private readonly IPlatoAnalisisRepositorio _platoAnalisisRepositorio;

        public AplicarDescuentoCasoDeUso(
            IArticuloRepositorio articuloRepositorio,
            ISugerenciaIARepositorio sugerenciaIARepositorio,
            IPlatoAnalisisRepositorio platoAnalisisRepositorio)
        {
            _articuloRepositorio = articuloRepositorio;
            _sugerenciaIARepositorio = sugerenciaIARepositorio;
            _platoAnalisisRepositorio = platoAnalisisRepositorio;
        }

        public async Task<AplicarDescuentoResultado?> EjecutarAsync(int restauranteId, int platoId, decimal porcentajeDescuento)
        {
            var articulo = await _articuloRepositorio.ObtenerDetalleAsync(restauranteId, platoId);
            var plato = articulo as Plato;
            if (plato == null)
            {
                return null;
            }

            // 1. Modificar precio en base de datos
            decimal precioOriginal = plato.PrecioVentaFinal ?? 0m;
            decimal descuento = precioOriginal * (porcentajeDescuento / 100);
            decimal precioNuevo = precioOriginal - descuento;

            plato.PrecioVentaFinal = precioNuevo;
            await _articuloRepositorio.ActualizarAsync(plato);

            // 2. Calcular costo dinámico de ingredientes
            decimal costoPreparacion = 0;
            if (plato.Ingredientes != null)
            {
                foreach (var item in plato.Ingredientes)
                {
                    decimal ultimoPrecioCompra = await _platoAnalisisRepositorio.ObtenerUltimoPrecioCompraInsumoAsync(item.InsumoId);
                    costoPreparacion += item.Cantidad * (ultimoPrecioCompra > 0 ? ultimoPrecioCompra : 0);
                }
            }

            // 3. Actualizar estado de sugerencia en IA
            var sugerenciaIa = await _sugerenciaIARepositorio.ObtenerSugerenciaIAAsync(restauranteId);
            if (sugerenciaIa != null && sugerenciaIa.PlatosAnalisis != null)
            {
                var analisisPlato = sugerenciaIa.PlatosAnalisis.FirstOrDefault(p => p.PlatoId == platoId);
                if (analisisPlato != null)
                {
                    var sugDescuento = analisisPlato.Sugerencias.FirstOrDefault(s => s.Tipo == "descuento");
                    if (sugDescuento != null)
                    {
                        sugDescuento.Aplicada = true;
                    }
                    await _sugerenciaIARepositorio.GuardarSugerenciaIAAsync(restauranteId, sugerenciaIa);
                }
            }

            // 4. Calcular nuevo margen
            decimal nuevoMargen = precioNuevo > 0 ? ((precioNuevo - costoPreparacion) / precioNuevo) * 100 : 0m;

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
