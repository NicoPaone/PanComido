using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class InsumoValidacionServicio : IInsumoValidacionServicio
    {
        private readonly ICategoriaInsumoRepositorio _categoriaInsumoRepositorio;
        private readonly IUnidadMedidaRepositorio _unidadMedidaRepositorio;
        private readonly ILogger<InsumoValidacionServicio> _logger;

        public InsumoValidacionServicio(
            ICategoriaInsumoRepositorio categoriaInsumoRepositorio,
            IUnidadMedidaRepositorio unidadMedidaRepositorio,
            ILogger<InsumoValidacionServicio> logger)
        {
            _categoriaInsumoRepositorio = categoriaInsumoRepositorio;
            _unidadMedidaRepositorio = unidadMedidaRepositorio;
            _logger = logger;
        }

        public async Task<CategoriaInsumo> ObtenerYValidarCategoriaAsync(int categoriaId)
        {
            CategoriaInsumo categoria = await _categoriaInsumoRepositorio.ObtenerPorIdAsync(categoriaId);
            if (categoria == null)
            {
                _logger.LogWarning("Rechazo de validación: La categoría con ID {CategoriaId} no existe.", categoriaId);
                throw new ArgumentException("La categoría de insumo seleccionada no existe en el sistema.");
            }
            return categoria;
        }

        public async Task<UnidadMedida> ObtenerYValidarUnidadMedidaAsync(int unidadMedidaId)
        {
            UnidadMedida unidadMedida = await _unidadMedidaRepositorio.ObtenerPorIdAsync(unidadMedidaId);
            if (unidadMedida == null)
            {
                _logger.LogWarning("Rechazo de validación: La unidad de medida con ID {UnidadMedidaId} no existe.", unidadMedidaId);
                throw new ArgumentException("La unidad de medida seleccionada no existe en el sistema.");
            }
            return unidadMedida;
        }
    }
}
