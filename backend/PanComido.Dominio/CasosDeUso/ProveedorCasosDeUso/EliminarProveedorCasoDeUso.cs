using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class EliminarProveedorCasoDeuso
    {
        private readonly IProveedorRepositorio _proveedorRepositorio;

        public EliminarProveedorCasoDeuso(IProveedorRepositorio proveedorRepositorio)
        {
            _proveedorRepositorio = proveedorRepositorio;
        }

        public async Task EjecutarAsync(int proveedorId)
        {
            var proveedor = await _proveedorRepositorio.ObtenerProveedorPorIdAsync(proveedorId);
            if (proveedor == null) throw new KeyNotFoundException("Proveedor no encontrado.");
            await _proveedorRepositorio.EliminarProveedorAsync(proveedorId);
        }
    }
}