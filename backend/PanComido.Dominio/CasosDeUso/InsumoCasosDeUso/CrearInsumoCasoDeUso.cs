using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
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

        private readonly ILogger<CrearInsumoCasoDeUso> _logger;

        public CrearInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio,
            ILoteRepositorio loteRepositorio,
            IBodegaRepositorio bodegaRepositorio,
            IUnidadMedidaRepositorio unidadMedidaRepositorio,
            ICategoriaInsumoRepositorio categoriaInsumoRepositorio,
            IEstadoStockInsumoServicio estadoStockInsumoServicio,
            ILogger<CrearInsumoCasoDeUso> logger)
        {
            _insumoRepositorio = insumoRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
            _unidadMedidaRepositorio = unidadMedidaRepositorio; 
            _categoriaInsumoRepositorio = categoriaInsumoRepositorio;
            _loteRepositorio = loteRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _logger = logger;
        }

        public async Task<Insumo> EjecutarAsync(
            int restauranteId,
            Insumo insumo, 
            int cantidadInicial,
            int idBodega,
            DateOnly fechaVencimiento)
        {
            _logger.LogInformation("Iniciando creación del insumo '{NombreInsumo}' para el restaurante {RestauranteId}. Cantidad inicial: {CantidadInicial}, Bodega destino: {BodegaId}", insumo.Nombre, restauranteId, cantidadInicial, idBodega);

            if (cantidadInicial < insumo.StockMinimo)
            {
                _logger.LogWarning("Rechazo al crear insumo: La cantidad inicial ({CantidadInicial}) es menor al stock mínimo configurado ({StockMinimo}). RestauranteId: {RestauranteId}", cantidadInicial, insumo.StockMinimo, restauranteId);
                throw new ArgumentException($"La cantidad inicial ({cantidadInicial}) no puede ser menor al stock mínimo configurado ({insumo.StockMinimo}).");
            }

            if (fechaVencimiento <= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                _logger.LogWarning("Rechazo al crear insumo: La fecha de vencimiento ingresada ({FechaVencimiento}) no es una fecha futura. RestauranteId: {RestauranteId}", fechaVencimiento, restauranteId);
                throw new ArgumentException("La fecha de vencimiento debe ser una fecha futura.");
            }

            if (!await _bodegaRepositorio.ExisteBodegaEnRestauranteAsync(restauranteId, idBodega))
            {
                _logger.LogWarning("Rechazo al crear insumo: La bodega especificada ({BodegaId}) no existe o no pertenece al Restaurante {RestauranteId}.", idBodega, restauranteId);
                throw new ArgumentException("La bodega destino especificada no es valida o no existe.");
            }

            CategoriaInsumo categoria = await _categoriaInsumoRepositorio.ObtenerPorIdAsync(insumo.CategoriaId);

            if (categoria == null)
            {
                _logger.LogWarning("Rechazo al crear insumo: La categoría con ID {CategoriaId} no existe en el sistema.", insumo.CategoriaId);
                throw new ArgumentException("La categoría de insumo seleccionada no existe en el sistema.");
            }

            UnidadMedida unidadMedida = await _unidadMedidaRepositorio.ObtenerPorIdAsync(insumo.UnidadDeMedidaId);

            if (unidadMedida == null)
            {
                _logger.LogWarning("Rechazo al crear insumo: La unidad de medida con ID {UnidadMedidaId} no existe en el sistema.", insumo.UnidadDeMedidaId);
                throw new ArgumentException("La unidad de medida seleccionada no existe en el sistema.");
            }


            var loteInicial = new Lote
            {
                Nombre = $"Lote {insumo.Nombre} - ({DateOnly.FromDateTime(DateTime.UtcNow)})",
                Cantidad = cantidadInicial,
                BodegaId = idBodega,
                FechaAdquisicion = DateOnly.FromDateTime(DateTime.UtcNow),
                FechaVencimiento = fechaVencimiento
            };

            insumo.RestauranteId = restauranteId;
            insumo.Tipo = categoria.TipoAplica;
            insumo.Lotes = new List<Lote> { loteInicial };

            Insumo insumoCreado = await _insumoRepositorio.CrearAsync(insumo);

            insumoCreado.Categoria = categoria.Descripcion;
            insumoCreado.UnidadMedida = unidadMedida.Nombre;
            insumoCreado.Vencimiento = loteInicial.FechaVencimiento;
            insumoCreado.StockActual = loteInicial.Cantidad;
            insumoCreado.EstadoStock = _estadoStockInsumoServicio.CalcularEstadoStock(insumoCreado.StockActual, insumoCreado.StockMinimo);

            _logger.LogInformation("Insumo '{NombreInsumo}' creado exitosamente con ID {InsumoId} en el restaurante {RestauranteId}, junto con su lote inicial.", insumoCreado.Nombre, insumoCreado.Id, restauranteId);

            return insumoCreado;
        }
    }
    
}
