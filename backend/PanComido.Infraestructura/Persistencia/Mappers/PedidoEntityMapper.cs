using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class PedidoEntityMapper
    {
        public DOM.Pedido paraDominio(EF.Pedido efPedido)
        {
            return new DOM.Pedido
            {
                Id = efPedido.Id,
                ProveedorId = efPedido.ProveedorId,
                ProveedorNombre = efPedido.Proveedor.Nombre,
                ProveedorTelefono = efPedido.Proveedor.NumeroTelefonoWsp,
                Fecha = efPedido.Fecha,
                Estado = efPedido.EstadoPedido.Descripcion,
                ItemsInsumo = efPedido.PedidoInsumos.Select(pi => new DOM.PedidoInsumo
                {
                    InsumoId = pi.InsumoId,
                    NombreInsumo = pi.Insumo.IdArticuloNavigation.Nombre,
                    UnidadMedida = pi.Insumo.UnidadMedida.Nombre,
                    Cantidad = pi.Cantidad,
                    PrecioCompra = pi.PrecioCompra,
                    CategoriaInsumoId = pi.Insumo.CategoriaInsumoId
                }).ToList()
            };
        }

        public EF.Pedido paraEf(DOM.Pedido pedido, int estadoPedidoId)
        {
            return new EF.Pedido
            {
                ProveedorId = pedido.ProveedorId,
                Fecha = pedido.Fecha,
                EstadoPedidoId = estadoPedidoId,
                PedidoInsumos = pedido.ItemsInsumo.Select(item => new EF.PedidoInsumo
                {
                    InsumoId = item.InsumoId,
                    Cantidad = item.Cantidad,
                    PrecioCompra = item.PrecioCompra
                }).ToList()
            };
        }
    }
}
