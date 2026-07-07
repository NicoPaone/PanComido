using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso
{
    public class ObtenerIngredientesParaCrearMiseAndPlace
    {
        private readonly IFormularioPlatoRepositorio _repositorio;
        private readonly ICategoriaInsumoRepositorio _categoriaRepositorio;
        private readonly IUnidadMedidaRepositorio _unidadMedidaRepositorio;
        private readonly IBodegaRepositorio _bodegaRepositorio;

        public ObtenerIngredientesParaCrearMiseAndPlace(
            IFormularioPlatoRepositorio repositorio,
            ICategoriaInsumoRepositorio categoriaRepositorio,
            IUnidadMedidaRepositorio unidadMedidaRepositorio,
            IBodegaRepositorio bodegaRepositorio)
        {
            _repositorio = repositorio;
            _categoriaRepositorio = categoriaRepositorio;
            _unidadMedidaRepositorio = unidadMedidaRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
        }

        public async Task<(List<Ingrediente>, List<CategoriaInsumo>, List<UnidadMedida>, List<Bodega>)> EjecutarAsync(int restauranteId)
        {
            var ingredientes = await ObtenerIngredientesBaseAsync(restauranteId);
            var categorias = await ObtenerCategoriasAsync();
            var unidades = await ObtenerUnidadesMedidaAsync();
            var bodegas = await ObtenerBodegasAsync(restauranteId);

            return (ingredientes, categorias, unidades, bodegas);
        }

        private async Task<List<Ingrediente>> ObtenerIngredientesBaseAsync(int restauranteId)
        {
            return await _repositorio.ObtenerIngredientesBaseAsync(restauranteId);
        }

        private async Task<List<CategoriaInsumo>> ObtenerCategoriasAsync()
        {
            var categorias = await _categoriaRepositorio.ObtenerCategoriasInsumoAsync();
            return categorias
                .Where(c => c.Descripcion.ToLower() != "con alcohol" && c.Descripcion.ToLower() != "sin alcohol")
                .ToList();
        }

        private async Task<List<UnidadMedida>> ObtenerUnidadesMedidaAsync()
        {
            return await _unidadMedidaRepositorio.ObtenerUnidadesDeMedidaAsync();
        }

        private async Task<List<Bodega>> ObtenerBodegasAsync(int restauranteId)
        {
            return await _bodegaRepositorio.ObtenerBodegasAsync(restauranteId);
        }
    }
}
