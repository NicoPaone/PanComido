using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class ListarProveedorCasoDeUso
    {
        private readonly IProveedorRepositorio _proveedorRepositorio;
        private readonly IPedidoRepositorio _pedidoRepositorio;

        public ListarProveedorCasoDeUso(IProveedorRepositorio proveedorRepositorio, IPedidoRepositorio pedidoRepositorio)
        {
            _proveedorRepositorio = proveedorRepositorio;
            _pedidoRepositorio = pedidoRepositorio;
        }

        public async Task<List<Proveedor>> EjecutarAsync(int restauranteId)
        {
            List<Proveedor> proveedores = await _proveedorRepositorio.ObtenerProveedoresAsync(restauranteId);
            foreach (var proveedor in proveedores)
            {
                proveedor.FechaUltimoPedido = await _pedidoRepositorio.ObtenerFechaUltimoPedidoDeProveedorAsync(proveedor.Id);
            }
            return proveedores.OrderByDescending(p => p.FechaUltimoPedido ?? DateOnly.MinValue).ToList();
        }
    }
}
