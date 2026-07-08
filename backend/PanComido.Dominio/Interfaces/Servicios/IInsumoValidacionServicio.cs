using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IInsumoValidacionServicio
    {
        Task<CategoriaInsumo> ObtenerYValidarCategoriaAsync(int categoriaId);
        Task<UnidadMedida> ObtenerYValidarUnidadMedidaAsync(int unidadMedidaId);
        Task ValidarInsumosDeRecetaBebidaAsync(int restauranteId, List<BebidaPreparadaInsumo> insumos);
    }
}
