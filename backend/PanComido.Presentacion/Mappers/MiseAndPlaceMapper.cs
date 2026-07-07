using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.MiseAndPlace;
using PanComido.Presentacion.DTOs.Insumos;
using PanComido.Presentacion.DTOs.UnidadesDeMedida;
using System.Collections.Generic;
using System.Linq;
using PanComido.Presentacion.DTOs.Bodegas;

namespace PanComido.Presentacion.Mappers
{
    public class MiseAndPlaceMapper
    {
        public IngredienteMiseAndPlaceResponseDto aDto(Ingrediente ingrediente)
        {
            return new IngredienteMiseAndPlaceResponseDto
            {
                Id = ingrediente.Id,
                Nombre = ingrediente.Nombre,
                UnidadMedida = ingrediente.UnidadMedida,
                CostoUnitario = ingrediente.CostoUnitario
            };
        }

        public List<IngredienteMiseAndPlaceResponseDto> aDtoList(List<Ingrediente> ingredientes)
        {
            if (ingredientes == null) return new List<IngredienteMiseAndPlaceResponseDto>();
            
            return ingredientes.Select(i => aDto(i)).ToList();
        }

        public CategoriaLightDto aDtoCategoria(CategoriaInsumo categoria)
        {
            return new CategoriaLightDto
            {
                Id = categoria.Id,
                Descripcion = categoria.Descripcion
            };
        }

        public UnidadMedidaResponseDto aDtoUnidad(UnidadMedida unidad)
        {
            return new UnidadMedidaResponseDto
            {
                Id = unidad.Id,
                Nombre = unidad.Nombre
            };
        }

        public BodegaLightDto aDtoBodega(Bodega bodega)
        {
            return new BodegaLightDto
            {
                Id = bodega.Id,
                Nombre = bodega.Nombre
            };
        }

        public MiseAndPlaceListadoDto aDtoListado(MiseAndPlaceListadoDominio dominio)
        {
            return new MiseAndPlaceListadoDto
            {
                LoteId = dominio.LoteId,
                ArticuloId = dominio.ArticuloId,
                MiseAndPlaceId = dominio.MiseAndPlaceId,
                Nombre = dominio.Nombre,
                Descripcion = dominio.Descripcion,
                Cantidad = dominio.Cantidad,
                FechaVencimiento = dominio.FechaVencimiento,
                UnidadMedida = dominio.UnidadMedida,
                Categoria = dominio.Categoria,
                Bodega = dominio.Bodega,
                Receta = dominio.Receta.Select(r => new RecetaItemDto
                {
                    IngredienteId = r.IngredienteId,
                    NombreIngrediente = r.NombreIngrediente,
                    Cantidad = r.Cantidad
                }).ToList()
            };
        }
    }
}
