using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.AvisosCasosDeUso.IA
{
    public class GenerarSugerenciasPlatoIACasoDeUso
    {
        private readonly ISugerenciaIARepositorio _sugerenciaIARepositorio;
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly ISugerenciaPlatosIAServicio _sugerenciaPlatosIAServicio;
        private readonly IArticuloRepositorio _articulosRepositorio;
        private readonly IVencimientosProximosInsumosServicio _vencimientosProximosInsumosServicio;

        public GenerarSugerenciasPlatoIACasoDeUso(
        ISugerenciaIARepositorio sugerenciaIARepositorio,
        IInsumoRepositorio insumoRepositorio,
        ISugerenciaPlatosIAServicio sugerenciaPlatosIAServicio,
        IArticuloRepositorio articulosRepositorio,
        IVencimientosProximosInsumosServicio vencimientosProximosInsumosServicio)
        {
            _sugerenciaIARepositorio = sugerenciaIARepositorio;
            _insumoRepositorio = insumoRepositorio;
            _sugerenciaPlatosIAServicio = sugerenciaPlatosIAServicio;
            _articulosRepositorio = articulosRepositorio;
            _vencimientosProximosInsumosServicio = vencimientosProximosInsumosServicio;
        }

        public async Task<SugerenciaIA> EjecutarAsync(int restauranteId)
        {
            SugerenciaIA? sugerenciaExistente = await _sugerenciaIARepositorio.ObtenerSugerenciaIAAsync(restauranteId);

            if (sugerenciaExistente != null 
                    && sugerenciaExistente.FechaSugerencia.Date == DateTime.Today
                    && sugerenciaExistente.PlatosSugeridos.Any())
            {
                return sugerenciaExistente;
            }

            List<Insumo> insumosDisponibles = await _insumoRepositorio.ObtenerInsumosConLotesAsync(restauranteId);

            Dictionary<int, List<Lote>> vencimientosProximos = _vencimientosProximosInsumosServicio.ObtenerVencimientosProximos(insumosDisponibles, 7);

            List<Articulo> articulos = await _articulosRepositorio.ObtenerArticulosEnCartaConIngredientesAsync(restauranteId);

            List<string> nombresPlatosExistenes = articulos.OfType<Plato>()
                                                        .Select(p => p.Nombre)
                                                        .ToList();

            SugerenciaIA nuevaSugerencia = await _sugerenciaPlatosIAServicio.GenerarSugerenciasAsync(restauranteId, insumosDisponibles, vencimientosProximos, nombresPlatosExistenes , 5);

            nuevaSugerencia.FechaSugerencia = DateTime.Now;

            await _sugerenciaIARepositorio.GuardarSugerenciaIAAsync(restauranteId, nuevaSugerencia);

            return nuevaSugerencia;
        }
    }
}