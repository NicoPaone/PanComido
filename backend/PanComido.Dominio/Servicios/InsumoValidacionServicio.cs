using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class InsumoValidacionServicio : IInsumoValidacionServicio
    {
        private readonly ICategoriaInsumoRepositorio _categoriaInsumoRepositorio;
        private readonly IUnidadMedidaRepositorio _unidadMedidaRepositorio;
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly ILogger<InsumoValidacionServicio> _logger;

        public InsumoValidacionServicio(
            ICategoriaInsumoRepositorio categoriaInsumoRepositorio,
            IUnidadMedidaRepositorio unidadMedidaRepositorio,
            IInsumoRepositorio insumoRepositorio,
            ILogger<InsumoValidacionServicio> logger)
        {
            _categoriaInsumoRepositorio = categoriaInsumoRepositorio;
            _unidadMedidaRepositorio = unidadMedidaRepositorio;
            _insumoRepositorio = insumoRepositorio;
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

        public async Task ValidarInsumosDeRecetaBebidaAsync(int restauranteId, List<BebidaPreparadaInsumo> insumos)
        {
            foreach (var item in insumos)
            {
                if (item.Cantidad <= 0)
                {
                    _logger.LogWarning("Rechazo de validación: La cantidad del insumo {InsumoId} debe ser mayor que cero.", item.InsumoId);
                    throw new ArgumentException("La cantidad de cada insumo debe ser mayor que cero.");
                }

                Insumo insumo = await _insumoRepositorio.ObtenerPorIdAsync(item.InsumoId, restauranteId);
                if (insumo == null)
                {
                    _logger.LogWarning("Rechazo de validación: El insumo con ID {InsumoId} no existe o no pertenece al restaurante {RestauranteId}.", item.InsumoId, restauranteId);
                    throw new ArgumentException($"El insumo con id {item.InsumoId} no existe o no pertenece al restaurante.");
                }

                if (insumo.Tipo != TipoInsumo.Bebida)
                {
                    _logger.LogWarning("Rechazo de validación: El insumo {InsumoId} ('{NombreInsumo}') no es de tipo Bebida.", item.InsumoId, insumo.Nombre);
                    throw new ArgumentException($"El insumo '{insumo.Nombre}' no es de tipo Bebida y no puede usarse en la receta de una bebida preparada.");
                }
            }
        }
    }
}
