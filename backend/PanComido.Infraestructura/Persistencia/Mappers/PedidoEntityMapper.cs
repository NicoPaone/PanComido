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
                Fecha = efPedido.Fecha,
                Estado = efPedido.EstadoPedido.Descripcion,
                ItemsInsumo = efPedido.PedidoInsumos.Select(pi => new DOM.PedidoInsumo
                {
                    InsumoId = pi.InsumoId,
                    NombreInsumo = pi.Insumo.IdArticuloNavigation.Nombre,
                    Cantidad = pi.Cantidad,
                    PrecioCompra = pi.PrecioCompra
                }).ToList()
            };
        }
    }
}
