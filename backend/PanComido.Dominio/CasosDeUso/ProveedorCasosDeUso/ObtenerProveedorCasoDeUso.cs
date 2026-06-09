using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class ObtenerProveedorCasoDeUso
    {
        private readonly IProveedorRepositorio _proveedorRepositorio;

        public ObtenerProveedorCasoDeUso(IProveedorRepositorio proveedorRepositorio)
        {
            _proveedorRepositorio = proveedorRepositorio;
        }

        public async Task<Proveedor> EjecutarAsync(int proveedorId)
        {
            Proveedor proveedorEncontrado = await _proveedorRepositorio.ObtenerProveedorPorIdAsync(proveedorId);
            if (proveedorEncontrado == null) throw new KeyNotFoundException("Proveedor no encontrado");

            return proveedorEncontrado;
        }
    }
}
