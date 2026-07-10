using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades.Enums;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class PlatoEntityMapper
    {
        public DOM.Plato CompletarMapeoDominio(EF.Articulo efArticulo)
        {
            return new DOM.Plato
            {
                TiempoPreparacionBase = efArticulo.Plato.TiempoPreparacionBase,
                Destacado = efArticulo.Plato.Destacado,
                Sugerencia = efArticulo.Plato.Sugerencia,
                CategoriaPlatoId = efArticulo.Plato.CategoriaPlatoId,
                TipoPlatoId = efArticulo.Plato.TipoPlatoId,

                Ingredientes = efArticulo.Plato.PlatoIngredientes?.Select(pi => new DOM.PlatoIngrediente
                {
                    InsumoId = pi.IngredienteId,
                    Opcional = pi.Opcional,
                    Cantidad = pi.Cantidad,
                    Insumo = new DOM.Insumo
                    {
                        Id = pi.IngredienteId,
                        Nombre = pi.Ingrediente?.IdInsumoNavigation?.IdArticuloNavigation?.Nombre ?? "Ingrediente sin nombre",
                        PedidoInsumos = pi.Ingrediente?.IdInsumoNavigation?.PedidoInsumos?.Select(pedido => new DOM.PedidoInsumo
                        {
                            InsumoId = pedido.InsumoId,
                            Cantidad = pedido.Cantidad,
                            PrecioCompra = pedido.PrecioCompra,
                            Fecha = pedido.Pedido?.Fecha ?? default,
                            Estado = pedido.Pedido != null ? (EstadoPedido)pedido.Pedido.EstadoPedidoId : default
                        }).ToList() ?? new List<DOM.PedidoInsumo>()
                    }
                }).ToList() ?? new List<DOM.PlatoIngrediente>(),

                // para mostrar en carta
                Categoria = efArticulo.Plato.CategoriaPlato?.Descripcion,
                TipoPlato = efArticulo.Plato.TipoPlato?.Descripcion,

                Restricciones = efArticulo.Plato.Restriccions?
                    .Select(rp => new DOM.Restriccion
                    {
                        Descripcion = rp.Descripcion
                    })
                    .ToList() ?? new List<DOM.Restriccion>() , 
            };
        }

        public EF.Plato CompletarMapeoAEntidad(DOM.Plato platoDominio)
        {
            return new EF.Plato
            {
                TiempoPreparacionBase = platoDominio.TiempoPreparacionBase,
                Destacado = platoDominio.Destacado,
                Sugerencia = platoDominio.Sugerencia,
                CategoriaPlatoId = platoDominio.CategoriaPlatoId,
                TipoPlatoId = platoDominio.TipoPlatoId,

                PlatoIngredientes = platoDominio.Ingredientes?.Select(i => new EF.PlatoIngrediente
                {
                    IngredienteId = i.InsumoId,
                    Opcional = i.Opcional,
                    Cantidad = i.Cantidad
                }).ToList() ?? new List<EF.PlatoIngrediente>()
            };
        }
    }
}
