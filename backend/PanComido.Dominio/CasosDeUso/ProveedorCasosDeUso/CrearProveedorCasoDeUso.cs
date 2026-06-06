using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class CrearProveedorCasoDeUso
    {
        private readonly IProveedorRepositorio _proveedorRepositorio;

        public CrearProveedorCasoDeUso(IProveedorRepositorio proveedorRepositorio)
        {
            _proveedorRepositorio = proveedorRepositorio;
        }

        public async Task<Proveedor> EjecutarAsync(Proveedor proveedor)
        {
            if (string.IsNullOrWhiteSpace(proveedor.Nombre)) throw new ArgumentException("El nombre del proveedor no puede ser nulo o vacío.");

            proveedor.Nombre = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(proveedor.Nombre.Trim().ToLower());
            
            if(proveedor.CategoriaIds == null || proveedor.CategoriaIds.Count == 0) throw new ArgumentException("El proveedor debe tener al menos una categoría de insumo asociada.");
            
            if (await _proveedorRepositorio.ExisteProveedorConNombreAsync(proveedor.RestauranteId, proveedor.Nombre))
                throw new ArgumentException($"Ya existe un proveedor con el nombre '{proveedor.Nombre}' en este restaurante.");

            return await _proveedorRepositorio.CrearProveedorAsync(proveedor);
        }
    }
}
