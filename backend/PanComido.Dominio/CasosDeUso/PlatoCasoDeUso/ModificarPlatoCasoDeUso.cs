using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Linq;

namespace PanComido.Dominio.CasosDeUso.PlatoCasosDeUso
{
    public class ModificarPlatoCasoDeUso
    {
        private readonly IPlatoRepositorio _platoRepositorio;
        private readonly IImagenServicio _servicioImagen;
        private readonly INormalizadorNombreServicio _normalizadorNombreServicio;

        public ModificarPlatoCasoDeUso(IPlatoRepositorio platoRepositorio, IImagenServicio servicioImagen, INormalizadorNombreServicio normalizadorNombreServicio)
        {
            _platoRepositorio = platoRepositorio;
            _servicioImagen = servicioImagen;
            _normalizadorNombreServicio = normalizadorNombreServicio;
        }

        public async Task<Plato> EjecutarAsync(int restauranteId, Plato platoModificado, string carpetaCloudinary, Stream stream, string nombreImagen)
        {

            var platoExistente = await _platoRepositorio.ObtenerPorIdAsync(platoModificado.Id, restauranteId);
            if (platoExistente == null)
            {
                throw new ArgumentException("El plato que intenta modificar no existe o no pertenece al restaurante.");
            }

            if (string.IsNullOrWhiteSpace(platoModificado.Nombre))
            {
                throw new ArgumentException("El nombre del plato no puede estar vacío.");
            }

            platoModificado.Nombre = _normalizadorNombreServicio.Normalizar(platoModificado.Nombre);

            bool elNombreCambio = !string.Equals(platoModificado.Nombre, platoExistente.Nombre, StringComparison.OrdinalIgnoreCase);
            if (elNombreCambio && await _platoRepositorio.ExistePlatoConNombreAsync(restauranteId, platoModificado.Nombre))
            {
                throw new ArgumentException($"Ya existe un plato con el nombre '{platoModificado.Nombre}' en el restaurante.");
            }

            if (platoModificado.Ingredientes != null && platoModificado.Ingredientes.Any(i => i.Cantidad <= 0))
            {
                throw new ArgumentException("La cantidad de cada ingrediente debe ser mayor que cero.");
            }

            if (platoModificado.Ingredientes != null &&
                platoModificado.Ingredientes.Select(i => i.InsumoId).Distinct().Count() != platoModificado.Ingredientes.Count)
            {
                throw new ArgumentException("No se puede repetir el mismo insumo en los ingredientes del plato.");
            }

            platoExistente.Nombre = platoModificado.Nombre;
            platoExistente.Descripcion = platoModificado.Descripcion;
            platoExistente.PrecioVentaFinal = platoModificado.PrecioVentaFinal;
            platoExistente.TiempoPreparacionBase = platoModificado.TiempoPreparacionBase;
            platoExistente.TipoPlatoId = platoModificado.TipoPlatoId;
            platoExistente.CategoriaPlatoId = platoModificado.CategoriaPlatoId;
            platoExistente.EsVisibleEnCarta = platoModificado.EsVisibleEnCarta;

            platoExistente.Restricciones = platoModificado.Restricciones;
            platoExistente.Ingredientes = platoModificado.Ingredientes;
            platoExistente.EsPrecioManual = platoModificado.EsPrecioManual;

            if (stream != null && !string.IsNullOrEmpty(nombreImagen))
            {
                platoExistente.UrlImagen = await _servicioImagen.SubirImagenAsync(stream, nombreImagen, carpetaCloudinary);
            }

            await _platoRepositorio.ActualizarAsync(platoExistente);

            return platoExistente;
        }
    }
}
