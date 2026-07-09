using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
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
        private readonly INormalizadorNombreServicio _normalizadorNombreServicio;
        private readonly ILogger<ModificarProveedorCasoDeUso> _logger;

        public ModificarProveedorCasoDeUso(IProveedorRepositorio proveedorRepositorio, INormalizadorNombreServicio normalizadorNombreServicio, ILogger<ModificarProveedorCasoDeUso> logger)
        {
            _proveedorRepositorio = proveedorRepositorio;
            _normalizadorNombreServicio = normalizadorNombreServicio;
            _logger = logger;
        }

        public async Task<Proveedor> EjecutarAsync(Proveedor proveedor)
        {
            Proveedor proveedorEncontrado = await _proveedorRepositorio.ObtenerProveedorPorIdAsync(proveedor.Id);
            if (proveedorEncontrado == null)
            {
                _logger.LogWarning("Intento de modificar proveedor inexistente. ProveedorId: {ProveedorId}", proveedor.Id);
                throw new KeyNotFoundException("Proveedor no encontrado.");
            }

            if (string.IsNullOrWhiteSpace(proveedor.Nombre)) throw new ArgumentException("El nombre del proveedor no puede ser nulo o vacío.");

            proveedor.Nombre = _normalizadorNombreServicio.Normalizar(proveedor.Nombre);

            if (proveedor.CategoriaIds == null || proveedor.CategoriaIds.Count == 0) throw new ArgumentException("El proveedor debe tener al menos una categoría asociada.");

            bool nombreExistente = await _proveedorRepositorio.ExisteProveedorConNombreAsync(proveedor.RestauranteId, proveedor.Nombre);
            if (nombreExistente && (proveedor.Nombre != proveedorEncontrado.Nombre))
                throw new ArgumentException($"Ya existe un proveedor con el nombre '{proveedor.Nombre}' en este restaurante.");
            Proveedor proveedorModificado = await _proveedorRepositorio.ModificarProveedorAsync(proveedor);

            _logger.LogInformation("Proveedor modificado. ProveedorId: {ProveedorId}, Nombre: {Nombre}", proveedorModificado.Id, proveedorModificado.Nombre);
            return proveedorModificado;
        }
    }
}
