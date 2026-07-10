using PanComido.Dominio.Entidades.Enums;
using System.Linq;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class FormularioParaCrearPlatoEntityMapper
    {
        public DOM.TipoPlato paraDominio(EF.TipoPlato ef) => new DOM.TipoPlato { Id = ef.Id, Descripcion = ef.Descripcion };
        public DOM.CategoriaPlato paraDominio(EF.CategoriaPlato ef) => new DOM.CategoriaPlato { Id = ef.Id, Descripcion = ef.Descripcion };
        public DOM.Restriccion paraDominio(EF.Restriccion ef) => new DOM.Restriccion { Id = ef.Id, Descripcion = ef.Descripcion };

        public DOM.Ingrediente paraDominio(EF.Ingrediente ef)
        {
            var insumo = ef.IdInsumoNavigation;
            var articulo = insumo?.IdArticuloNavigation;
            var unidadMedida = insumo?.UnidadMedida;

            return new DOM.Ingrediente
            {
                Id = ef.IdInsumo,
                Nombre = articulo?.Nombre ?? "Sin nombre",
                UnidadMedida = unidadMedida?.Nombre ?? "",
                PedidoInsumos = insumo?.PedidoInsumos?.Select(pi => new DOM.PedidoInsumo
                {
                    InsumoId = pi.InsumoId,
                    Cantidad = pi.Cantidad,
                    PrecioCompra = pi.PrecioCompra,
                    Fecha = pi.Pedido?.Fecha ?? default,
                    Estado = pi.Pedido != null ? (EstadoPedido)pi.Pedido.EstadoPedidoId : default
                }).ToList() ?? new List<DOM.PedidoInsumo>()
            };
        }
    }
}