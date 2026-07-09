using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Servicios;
using System.Collections.Generic;
using System.Linq;

namespace PanComido.Dominio.Servicios
{
    public class UltimoPrecioCompraInsumoServicio : IUltimoPrecioCompraInsumoServicio
    {
        public decimal ObtenerUltimoPrecioCompraRecibido(List<PedidoInsumo> pedidoInsumos)
        {
            var ultimoPedidoRecibido = pedidoInsumos?
                .Where(pi => pi.Estado == EstadoPedido.Recibido)
                .OrderByDescending(pi => pi.Fecha)
                .FirstOrDefault();

            return ultimoPedidoRecibido?.PrecioCompra ?? 0;
        }
    }
}
