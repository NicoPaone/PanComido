using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class ModificarProveedorCasoDeUso
    {
        private readonly IProveedorRepositorio _proveedorRepositorio;

        public ModificarProveedorCasoDeUso(IProveedorRepositorio proveedorRepositorio)
        {
            _proveedorRepositorio = proveedorRepositorio;
        }

        public async Task<Proveedor> EjecutarAsync(Proveedor proveedor)
        {
            Proveedor proveedorEncontrado = await _proveedorRepositorio.ObtenerProveedorPorIdAsync(proveedor.Id);
            if (proveedorEncontrado == null) throw new KeyNotFoundException("Proveedor no encontrado.");

            if (string.IsNullOrWhiteSpace(proveedor.Nombre)) throw new ArgumentException("El nombre del proveedor no puede ser nulo o vacío.");

            proveedor.Nombre = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(proveedor.Nombre.Trim().ToLower()); //todas las primeras letras del nombre van a empezar en mayuscula

            if (proveedor.CategoriaIds == null || proveedor.CategoriaIds.Count == 0) throw new ArgumentException("El proveedor debe tener al menos una categoría asociada.");

            bool nombreExistente = await _proveedorRepositorio.ExisteProveedorConNombreAsync(proveedor.RestauranteId, proveedor.Nombre);
            if (nombreExistente && (proveedor.Nombre != proveedorEncontrado.Nombre))
                throw new ArgumentException($"Ya existe un proveedor con el nombre '{proveedor.Nombre}' en este restaurante.");

            return await _proveedorRepositorio.ModificarProveedorAsync(proveedor);
        }
    }
}
