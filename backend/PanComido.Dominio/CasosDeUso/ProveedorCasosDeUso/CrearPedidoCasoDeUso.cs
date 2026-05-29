using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class CrearPedidoCasoDeUso
    {
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly IProveedorRepositorio _proveedorRepositorio;
        private readonly IInsumoRepositorio _insumoRepositorio;

        public CrearPedidoCasoDeUso(
            IPedidoRepositorio pedidoRepositorio,
            IProveedorRepositorio proveedorRepositorio,
            IInsumoRepositorio insumoRepositorio)
        {
            _pedidoRepositorio = pedidoRepositorio;
            _proveedorRepositorio = proveedorRepositorio;
            _insumoRepositorio = insumoRepositorio;
        }

        public async Task<Pedido> EjecutarAsync(Pedido pedido, int restauranteId)
        {
            if (pedido.ItemsInsumo.Select(i => i.InsumoId).Distinct().Count() != pedido.ItemsInsumo.Count)
                throw new Exception("Hay insumos duplicados");

            Proveedor proveedor = await _proveedorRepositorio.ObtenerProveedorPorIdAsync(pedido.ProveedorId);
            if (proveedor == null || proveedor.RestauranteId != restauranteId)
            {
                throw new Exception("Proveedor no encontrado");
            }

            List<Insumo> insumos = await _insumoRepositorio.ObtenerInsumosDelProveedorAsync(proveedor.Id, restauranteId);

            var idsValidos = insumos.Select(i => i.Id).ToHashSet();
            if (pedido.ItemsInsumo.Any(item => !idsValidos.Contains(item.InsumoId)))
                throw new Exception("Hay insumos que no pertenecen al proveedor");

            pedido.Fecha = DateOnly.FromDateTime(DateTime.Now);
            pedido.Estado = "Pendiente";

            return await _pedidoRepositorio.CrearPedidoAsync(pedido);
        }
    }
}
