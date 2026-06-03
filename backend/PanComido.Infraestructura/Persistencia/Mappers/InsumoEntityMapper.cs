using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
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
        private LoteEntityMapper _loteEntityMapper;
        
        public InsumoEntityMapper(LoteEntityMapper loteEntityMapper)
        {
            _loteEntityMapper = loteEntityMapper;
        }

        public DOM.Insumo CompletarMapeoDominio(EF.Articulo efArticulo)
        {
            if (efArticulo == null) return null;

            var domInsumo = new DOM.Insumo
            {
                StockMinimo = efArticulo.Insumo.StockMinimo,
                Tipo = efArticulo.Insumo.CategoriaInsumo != null ? (TipoInsumo)efArticulo.Insumo.CategoriaInsumo.TipoAplica : default,
                Categoria = efArticulo.Insumo.CategoriaInsumo?.Descripcion,
                UnidadMedida = efArticulo.Insumo.UnidadMedida?.Nombre,
                CategoriaId = efArticulo.Insumo.CategoriaInsumoId,
                UnidadDeMedidaId = efArticulo.Insumo.UnidadMedidaId
            };

            if (efArticulo.Insumo.Lotes != null && efArticulo.Insumo.Lotes.Any())
            {
                domInsumo.Lotes = efArticulo.Insumo.Lotes?
                .Select(l => _loteEntityMapper.paraDominio(l))
                .ToList() ?? new List<DOM.Lote>();


                domInsumo.StockActual = domInsumo.Lotes.Sum(l => l.Cantidad);
            }
            if (efArticulo.Insumo.PedidoInsumos != null && efArticulo.Insumo.PedidoInsumos.Any())
            {
                domInsumo.PedidoInsumos = efArticulo.Insumo.PedidoInsumos.Select(pi => new DOM.PedidoInsumo
                {
                    InsumoId = pi.InsumoId,
                    Cantidad = pi.Cantidad,
                    PrecioCompra = pi.PrecioCompra
                }).ToList();
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

                Lotes = insumoDominio.Lotes?
                .Select(l => _loteEntityMapper.paraEntidad(l))
                .ToList() ?? new List<EF.Lote>()
            };

            if (insumoDominio.Tipo == TipoInsumo.Ingrediente)
                efInsumo.Ingrediente = new EF.Ingrediente();

            return efInsumo;
        }
    }
}