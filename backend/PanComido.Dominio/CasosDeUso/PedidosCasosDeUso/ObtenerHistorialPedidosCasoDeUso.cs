using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PedidosCasosDeUso
{
    public class ObtenerHistorialPedidosCasoDeUso
    {
        private readonly IProveedorRepositorio _proveedorRepositorio;
        private readonly IPedidoRepositorio _pedidoRepositorio;

        public ObtenerHistorialPedidosCasoDeUso(IProveedorRepositorio proveedorRepositorio, IPedidoRepositorio pedidoRepositorio)
        {
            _proveedorRepositorio = proveedorRepositorio;
            _pedidoRepositorio = pedidoRepositorio;
        }

        public async Task<List<Pedido>?> EjecutarAsync(int proveedorId)
        {
            var proveedor = await _proveedorRepositorio.ObtenerProveedorPorIdAsync(proveedorId);
            if (proveedor == null) throw new KeyNotFoundException("Proveedor no encontrado");

            return await _pedidoRepositorio.ObtenerPedidosPorProveedorAsync(proveedorId);
        }
    }
}
