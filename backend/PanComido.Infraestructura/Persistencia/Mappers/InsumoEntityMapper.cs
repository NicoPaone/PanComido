using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Infraestructura.Persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class InsumoEntityMapper
    {
        public DOM.Insumo CompletarMapeoDominio(EF.Articulo efArticulo)
        {
            if (efArticulo == null) return null;

            var domInsumo = new DOM.Insumo
            {
                StockMinimo = efArticulo.Insumo.StockMinimo,
                Tipo = efArticulo.Insumo.CategoriaInsumo != null ? (TipoInsumo)efArticulo.Insumo.CategoriaInsumo.TipoAplica : default,
                Tipo = (TipoInsumo?)efArticulo.Insumo.CategoriaInsumo?.TipoAplica,
                Categoria = efArticulo.Insumo.CategoriaInsumo?.Descripcion,
                UnidadMedida = efArticulo.Insumo.UnidadMedida?.Nombre,
                CategoriaId = efArticulo.Insumo.CategoriaInsumoId,
                UnidadDeMedidaId = efArticulo.Insumo.UnidadMedidaId
            };

            if (efArticulo.Insumo.Lotes != null && efArticulo.Insumo.Lotes.Any())
            {
                domInsumo.Lotes = efArticulo.Insumo.Lotes.Select(l => new DOM.Lote
                {
                    Id = l.Id,
                    Cantidad = l.Cantidad,
                    FechaVencimiento = l.FechaVencimiento.GetValueOrDefault()
                }).ToList();
                
                domInsumo.StockActual = domInsumo.Lotes.Sum(l => l.Cantidad);
            }

            return domInsumo;
        }

        public EF.Insumo CompletarMapeoAEntidad(DOM.Insumo insumoDominio)
        {
            var efInsumo = new EF.Insumo
            {
                CategoriaInsumoId = insumoDominio.CategoriaId,
                UnidadMedidaId = insumoDominio.UnidadDeMedidaId,
                StockMinimo = insumoDominio.StockMinimo,

                Lotes = insumoDominio.Lotes?.Select(l => new EF.Lote
                {
                    Nombre = l.Nombre,
                    Cantidad = l.Cantidad,
                    BodegaId = l.BodegaId,
                    FechaAdquisicion = l.FechaAdquisicion,
                    FechaVencimiento = l.FechaVencimiento
                }).ToList() ?? new List<EF.Lote>()
            };

            if (insumoDominio.Tipo == TipoInsumo.Ingrediente)
                efInsumo.Ingrediente = new EF.Ingrediente();

            return efInsumo;
        }
    }
}