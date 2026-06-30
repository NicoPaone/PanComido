using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.PlatoCasosDeUso
{
    public class ModificarPlatoCasoDeUso
    {
        private readonly IPlatoRepositorio _platoRepositorio;

        public ModificarPlatoCasoDeUso(IPlatoRepositorio platoRepositorio)
        {
            _platoRepositorio = platoRepositorio;
        }

        public async Task EjecutarAsync(int restauranteId, Plato platoModificado)
        {

            var platoExistente = await _platoRepositorio.ObtenerPorIdAsync(platoModificado.Id, restauranteId);
            if (platoExistente == null)
            {
                throw new ArgumentException("El plato que intenta modificar no existe o no pertenece al restaurante.");
            }

            platoExistente.Nombre = platoModificado.Nombre;
            platoExistente.Descripcion = platoModificado.Descripcion;
            platoExistente.PrecioVentaFinal = platoModificado.PrecioVentaFinal;
            platoExistente.TiempoPreparacionBase = platoModificado.TiempoPreparacionBase;
            platoExistente.TipoPlatoId = platoModificado.TipoPlatoId;
            platoExistente.CategoriaPlatoId = platoModificado.CategoriaPlatoId;
            platoExistente.UrlImagen = platoModificado.UrlImagen;
            platoExistente.EsVisibleEnCarta = platoModificado.EsVisibleEnCarta;

            platoExistente.Restricciones = platoModificado.Restricciones;
            platoExistente.Ingredientes = platoModificado.Ingredientes;

            await _platoRepositorio.ActualizarAsync(platoExistente);
        }
    }
}
