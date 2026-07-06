using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
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
    public class CrearInsumoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IBodegaRepositorio _bodegaRepositorio;
        private readonly IUnidadMedidaRepositorio _unidadMedidaRepositorio;
        private readonly ICategoriaInsumoRepositorio _categoriaInsumoRepositorio;

        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;
        private readonly IImagenServicio _imagenServicio;

        private readonly ILogger<CrearInsumoCasoDeUso> _logger;

        public CrearInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio,
            ILoteRepositorio loteRepositorio,
            IBodegaRepositorio bodegaRepositorio,
            IUnidadMedidaRepositorio unidadMedidaRepositorio,
            ICategoriaInsumoRepositorio categoriaInsumoRepositorio,
            IEstadoStockInsumoServicio estadoStockInsumoServicio,
            IImagenServicio imagenServicio,
            ILogger<CrearInsumoCasoDeUso> logger)
        {
            _insumoRepositorio = insumoRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
            _unidadMedidaRepositorio = unidadMedidaRepositorio;
            _categoriaInsumoRepositorio = categoriaInsumoRepositorio;
            _loteRepositorio = loteRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _imagenServicio = imagenServicio;
            _logger = logger;
        }

        public async Task<Insumo> EjecutarAsync(
            int restauranteId,
            Insumo insumo,
            int cantidadInicial,
            int idBodega,
            DateOnly fechaVencimiento,
            Stream stream, 
            string nombreImagen,
            string carpetaCloudinary)
        {
            _logger.LogInformation("Iniciando creación del insumo '{NombreInsumo}' para el restaurante {RestauranteId}. Cantidad inicial: {CantidadInicial}, Bodega destino: {BodegaId}", insumo.Nombre, restauranteId, cantidadInicial, idBodega);

            ValidarReglasDeNegocio(restauranteId, insumo, cantidadInicial, fechaVencimiento);
            await ValidarBodegaAsync(restauranteId, idBodega);

            CategoriaInsumo categoria = await ObtenerYValidarCategoriaAsync(insumo.CategoriaId);
            UnidadMedida unidadMedida = await ObtenerYValidarUnidadMedidaAsync(insumo.UnidadDeMedidaId);

            Lote loteInicial = CrearLoteInicial(insumo.Nombre, cantidadInicial, idBodega, fechaVencimiento);
            
            insumo.RestauranteId = restauranteId;
            insumo.Tipo = categoria.TipoAplica;
            insumo.Lotes = new List<Lote> { loteInicial };

            insumo.UrlImagen = await SubirYObtenerUrlDeImagen(stream, nombreImagen, carpetaCloudinary);

            Insumo insumoCreado = await _insumoRepositorio.CrearAsync(insumo);

            CompletarDatosDeRespuesta(insumoCreado, categoria, unidadMedida, loteInicial);

            _logger.LogInformation("Insumo '{NombreInsumo}' creado exitosamente con ID {InsumoId} en el restaurante {RestauranteId}, junto con su lote inicial.", insumoCreado.Nombre, insumoCreado.Id, restauranteId);

            return insumoCreado;
        }

        private void ValidarReglasDeNegocio(int restauranteId, Insumo insumo, int cantidadInicial, DateOnly fechaVencimiento)
        {
            if (cantidadInicial < insumo.StockMinimo)
            {
                _logger.LogWarning("Rechazo al crear insumo: La cantidad inicial es menor al stock mínimo configurado. RestauranteId: {RestauranteId}", restauranteId);
                throw new ArgumentException($"La cantidad inicial ({cantidadInicial}) no puede ser menor al stock mínimo configurado ({insumo.StockMinimo}).");
            }

            if (fechaVencimiento <= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                _logger.LogWarning("Rechazo al crear insumo: La fecha de vencimiento ingresada no es una fecha futura. RestauranteId: {RestauranteId}", restauranteId);
                throw new ArgumentException("La fecha de vencimiento debe ser una fecha futura.");
            }
        }

        private async Task ValidarBodegaAsync(int restauranteId, int idBodega)
        {
            if (!await _bodegaRepositorio.ExisteBodegaEnRestauranteAsync(restauranteId, idBodega))
            {
                _logger.LogWarning("Rechazo al crear insumo: La bodega especificada no existe o no pertenece al Restaurante {RestauranteId}.", restauranteId);
                throw new ArgumentException("La bodega destino especificada no es valida o no existe.");
            }
        }

        private async Task<CategoriaInsumo> ObtenerYValidarCategoriaAsync(int categoriaId)
        {
            CategoriaInsumo categoria = await _categoriaInsumoRepositorio.ObtenerPorIdAsync(categoriaId);
            if (categoria == null)
            {
                _logger.LogWarning("Rechazo al crear insumo: La categoría con ID {CategoriaId} no existe.", categoriaId);
                throw new ArgumentException("La categoría de insumo seleccionada no existe en el sistema.");
            }
            return categoria;
        }

        private async Task<UnidadMedida> ObtenerYValidarUnidadMedidaAsync(int unidadMedidaId)
        {
            UnidadMedida unidadMedida = await _unidadMedidaRepositorio.ObtenerPorIdAsync(unidadMedidaId);
            if (unidadMedida == null)
            {
                _logger.LogWarning("Rechazo al crear insumo: La unidad de medida con ID {UnidadMedidaId} no existe.", unidadMedidaId);
                throw new ArgumentException("La unidad de medida seleccionada no existe en el sistema.");
            }
            return unidadMedida;
        }

        private Lote CrearLoteInicial(string nombreInsumo, int cantidadInicial, int idBodega, DateOnly fechaVencimiento)
        {
            return new Lote
            {
                Nombre = $"Lote {nombreInsumo} - ({DateOnly.FromDateTime(DateTime.UtcNow)})",
                Cantidad = cantidadInicial,
                BodegaId = idBodega,
                FechaAdquisicion = DateOnly.FromDateTime(DateTime.UtcNow),
                FechaVencimiento = fechaVencimiento
            };
        }
        private async Task<string> SubirYObtenerUrlDeImagen(Stream stream, string nombreImagen, string carpetaCloudinary)
        {
            string? urlImagen = null;

            if (stream != null && !string.IsNullOrEmpty(nombreImagen))
            {
                urlImagen = await _imagenServicio
                   .SubirImagenAsync(stream, nombreImagen, carpetaCloudinary);
            }

            return urlImagen;
        }
        private void CompletarDatosDeRespuesta(Insumo insumoCreado, CategoriaInsumo categoria, UnidadMedida unidadMedida, Lote loteInicial)
        {
            insumoCreado.Categoria = categoria.Descripcion;
            insumoCreado.UnidadMedida = unidadMedida.Nombre;
            insumoCreado.Vencimiento = loteInicial.FechaVencimiento;
            insumoCreado.StockActual = loteInicial.Cantidad;
            insumoCreado.EstadoStock = _estadoStockInsumoServicio.CalcularEstadoStock(insumoCreado.StockActual, insumoCreado.StockMinimo, insumoCreado.StockRecomendado);
        }
    }
}
