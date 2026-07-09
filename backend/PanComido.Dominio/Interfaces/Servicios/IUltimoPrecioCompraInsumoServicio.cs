using PanComido.Dominio.Entidades;
using System.Collections.Generic;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IUltimoPrecioCompraInsumoServicio
    {
        decimal ObtenerUltimoPrecioCompraRecibido(List<PedidoInsumo> pedidoInsumos);
    }
}
