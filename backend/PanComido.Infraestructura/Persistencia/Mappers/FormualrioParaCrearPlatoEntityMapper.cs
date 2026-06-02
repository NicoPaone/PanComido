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
            // Navegamos por las relaciones que EF Core armó
            var insumo = ef.IdInsumoNavigation;
            var articulo = insumo?.IdArticuloNavigation;
            var unidadMedida = insumo?.UnidadMedida;

            // Extraemos el precio de compra de la tabla intermedia Pedido_Insumo
            decimal costoCalculado = 0m;

            if (insumo != null && insumo.PedidoInsumos != null && insumo.PedidoInsumos.Any())
            {
                // Tomamos el primero de la lista. 
                // En el Repositorio nos aseguraremos de traer esta lista ordenada por el pedido más reciente.
                var ultimoPedidoInsumo = insumo.PedidoInsumos.FirstOrDefault();

                if (ultimoPedidoInsumo != null)
                {
                    costoCalculado = ultimoPedidoInsumo.PrecioCompra;
                }
            }

            return new DOM.Ingrediente
            {
                Id = ef.IdInsumo,
                Nombre = articulo?.Nombre ?? "Sin nombre",
                UnidadMedida = unidadMedida?.Nombre ?? "",
                CostoUnitario = costoCalculado
            };
        }
    }
}