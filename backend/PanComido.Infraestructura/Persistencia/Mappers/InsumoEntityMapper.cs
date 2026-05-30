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
        public DOM.Insumo paraDominio(EF.Articulo efArticulo)
        {
            if (efArticulo == null) return null;

            EF.Insumo efInsumo = efArticulo.Insumo
                ?? throw new InvalidOperationException("Articulo no es un insumo");

            return new DOM.Insumo
            {
                Id = efArticulo.Id,
                Nombre = efArticulo.Nombre,
                Descripcion = efArticulo.Descripcion,
                StockMinimo = efInsumo.StockMinimo,
                // El tipo lo da la categoria del insumo
                Tipo = (TipoInsumo)efInsumo.CategoriaInsumo.TipoAplica,
                Categoria = efInsumo.CategoriaInsumo.Descripcion,
                UnidadMedida = efInsumo.UnidadMedida?.Nombre
            };
        }

        public EF.Articulo paraEntidad(DOM.Insumo insumoDominio)
        {
            var efArticulo = new EF.Articulo
            {
                RestauranteId = insumoDominio.RestauranteId,
                Nombre = insumoDominio.Nombre,
                Descripcion = insumoDominio.Descripcion,
                PrecioVentaFinal = insumoDominio.PrecioVentaFinal,

                Insumo = new EF.Insumo
                {
                    CategoriaInsumoId = insumoDominio.CategoriaId,
                    UnidadMedidaId = insumoDominio.UnidadDeMedidaId,
                    StockMinimo = insumoDominio.StockMinimo,

                    Lotes = insumoDominio.Lotes.Select(l => new EF.Lote
                    {
                        Nombre = l.Nombre,
                        Cantidad = l.Cantidad,
                        BodegaId = l.BodegaId,
                        FechaAdquisicion = l.FechaAdquisicion,
                        FechaVencimiento = l.FechaVencimiento
                    }).ToList()
                }
            };

            if (insumoDominio.Tipo == TipoInsumo.Ingrediente)
                efArticulo.Insumo.Ingrediente = new EF.Ingrediente();

            return efArticulo;
        }
    }
}
