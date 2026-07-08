using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class ModificarInsumoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;
        private readonly IImagenServicio _imagenServicio;
        private readonly IInsumoValidacionServicio _insumoValidacionServicio;
        private readonly ILogger<ModificarInsumoCasoDeUso> _logger;

        public ModificarInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio,
                                            IEstadoStockInsumoServicio estadoStockInsumoServicio,
                                            IImagenServicio imagenServicio,
                                                        IInsumoValidacionServicio insumoValidacionServicio,
                                            ILogger<ModificarInsumoCasoDeUso> logger)
        {
            _insumoRepositorio = insumoRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _imagenServicio = imagenServicio;
            _insumoValidacionServicio = insumoValidacionServicio;
            _logger = logger;
        }

        public async Task<Insumo> EjecutarAsync(int restauranteId, Insumo insumoModificado, Stream stream, string nombreImagen, string carpetaCloudinary)
        {
            _logger.LogInformation("Iniciando modificación del insumo {InsumoId} para el restaurante {RestauranteId}.", insumoModificado.Id, restauranteId);
            var insumoExistente = await _insumoRepositorio.ObtenerPorIdAsync(insumoModificado.Id, restauranteId);
            CategoriaInsumo categoria = await ValidarExistenciaYCategoria(insumoModificado, insumoExistente);

            UnidadMedida unidadMedida = await _insumoValidacionServicio.ObtenerYValidarUnidadMedidaAsync(insumoModificado.UnidadDeMedidaId);

            ActualizarDatosInsumo(insumoModificado, insumoExistente);

            insumoExistente.EsVisibleEnCarta = categoria.TipoAplica == TipoInsumo.Bebida && insumoModificado.EsVisibleEnCarta;

            await ActualizarImagenSegunTipoAsync(stream, nombreImagen, carpetaCloudinary, insumoExistente, categoria);

            await _insumoRepositorio.ActualizarAsync(insumoExistente);

            insumoExistente.EstadoStock = _estadoStockInsumoServicio.CalcularEstadoStock(insumoExistente.StockActual, insumoExistente.StockMinimo, insumoExistente.StockRecomendado);
            insumoExistente.Categoria = categoria.Descripcion;
            insumoExistente.UnidadMedida = unidadMedida.Nombre;

            _logger.LogInformation("Insumo '{NombreInsumo}' (ID {InsumoId}) modificado exitosamente en el restaurante {RestauranteId}.", insumoExistente.Nombre, insumoExistente.Id, restauranteId);

            return insumoExistente;
        }

        private async Task ActualizarImagenSegunTipoAsync(Stream stream, string nombreImagen, string carpetaCloudinary, Insumo insumoExistente, CategoriaInsumo categoria)
        {
            if (categoria.TipoAplica == TipoInsumo.Bebida)
            {
                if (stream != null && !string.IsNullOrEmpty(nombreImagen))
                {
                    insumoExistente.UrlImagen = await _imagenServicio.SubirImagenAsync(stream, nombreImagen, carpetaCloudinary);
                }
            }
            else
            {
                insumoExistente.UrlImagen = null;
            }
        }

        private static void ActualizarDatosInsumo(Insumo insumoModificado, Insumo insumoExistente)
        {
            insumoExistente.Nombre = insumoModificado.Nombre;
            insumoExistente.Descripcion = insumoModificado.Descripcion;
            insumoExistente.PrecioVentaFinal = insumoModificado.PrecioVentaFinal;
            insumoExistente.CategoriaId = insumoModificado.CategoriaId;
            insumoExistente.UnidadDeMedidaId = insumoModificado.UnidadDeMedidaId;
            insumoExistente.StockMinimo = insumoModificado.StockMinimo;
            insumoExistente.StockRecomendado = insumoModificado.StockRecomendado;
            insumoExistente.EsPrecioManual = insumoModificado.EsPrecioManual;
        }

        private async Task<CategoriaInsumo> ValidarExistenciaYCategoria(Insumo insumoModificado, Insumo insumoExistente)
        {
            if (insumoExistente == null)
            {
                _logger.LogWarning("Rechazo al modificar insumo: El insumo {InsumoId} no existe o no pertenece al restaurante.", insumoModificado.Id);
                throw new ArgumentException($"El insumo que intenta modificar no existe o no pertenece al restaurante.");
            }

            CategoriaInsumo categoria = await _insumoValidacionServicio.ObtenerYValidarCategoriaAsync(insumoModificado.CategoriaId);
            if (categoria.TipoAplica != insumoExistente.Tipo)
            {
                _logger.LogWarning("Rechazo al modificar insumo {InsumoId}: La categoría {CategoriaId} no coincide con el tipo actual del insumo.", insumoExistente.Id, insumoModificado.CategoriaId);
                throw new ArgumentException($"La categoría seleccionada no es válida para el tipo de insumo especificado.");
            }

            return categoria;
        }
    }
}
