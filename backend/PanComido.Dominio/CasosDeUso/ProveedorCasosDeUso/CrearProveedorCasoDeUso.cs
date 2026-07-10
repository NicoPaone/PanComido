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
    public class CrearProveedorCasoDeUso
    {
        private readonly IProveedorRepositorio _proveedorRepositorio;
        private readonly INormalizadorNombreServicio _normalizadorNombreServicio;
        private readonly ILogger<CrearProveedorCasoDeUso> _logger;


        public CrearProveedorCasoDeUso(IProveedorRepositorio proveedorRepositorio, INormalizadorNombreServicio normalizadorNombreServicio, ILogger<CrearProveedorCasoDeUso> logger)
        {
            _proveedorRepositorio = proveedorRepositorio;
            _normalizadorNombreServicio = normalizadorNombreServicio;
            _logger = logger;

        }

        public async Task<Proveedor> EjecutarAsync(Proveedor proveedor)
        {
            if (string.IsNullOrWhiteSpace(proveedor.Nombre)) throw new ArgumentException("El nombre del proveedor no puede ser nulo o vacío.");

            proveedor.Nombre = _normalizadorNombreServicio.Normalizar(proveedor.Nombre);

            if(proveedor.CategoriaIds == null || proveedor.CategoriaIds.Count == 0) throw new ArgumentException("El proveedor debe tener al menos una categoría asociada.");

            if (string.IsNullOrWhiteSpace(proveedor.NumeroTelefonoWsp)) throw new ArgumentException("El teléfono del proveedor es obligatorio.");

            bool nombreExistente = await _proveedorRepositorio.ExisteProveedorConNombreAsync(proveedor.RestauranteId, proveedor.Nombre);
            if (nombreExistente)
                throw new ArgumentException($"Ya existe un proveedor con el nombre '{proveedor.Nombre}' en este restaurante.");

            Proveedor proveedorCreado = await _proveedorRepositorio.CrearProveedorAsync(proveedor);
            _logger.LogInformation("Proveedor creado. ProveedorId: {ProveedorId}, Nombre: {Nombre}, RestauranteId: {RestauranteId}", proveedorCreado.Id, proveedorCreado.Nombre, proveedorCreado.RestauranteId);

            return proveedorCreado;
        }
    }
}
