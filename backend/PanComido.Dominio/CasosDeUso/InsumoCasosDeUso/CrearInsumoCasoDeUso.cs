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


        public CrearInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio,
            ILoteRepositorio loteRepositorio,
            IBodegaRepositorio bodegaRepositorio,
            IUnidadMedidaRepositorio unidadMedidaRepositorio,
            ICategoriaInsumoRepositorio categoriaInsumoRepositorio,
            IEstadoStockInsumoServicio estadoStockInsumoServicio)
        {
            _insumoRepositorio = insumoRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
            _unidadMedidaRepositorio = unidadMedidaRepositorio; 
            _categoriaInsumoRepositorio = categoriaInsumoRepositorio;
            _loteRepositorio = loteRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
        }

        public async Task<Insumo> EjecutarAsync(
            int restauranteId,
            Insumo insumo, 
            int cantidadInicial,
            int idBodega,
            DateOnly fechaVencimiento)
        {


            if (cantidadInicial < insumo.StockMinimo)
                throw new ArgumentException($"La cantidad inicial ({cantidadInicial}) no puede ser menor al stock mínimo configurado ({insumo.StockMinimo}).");

            if (fechaVencimiento <= DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("La fecha de vencimiento debe ser una fecha futura.");

            if (!await _bodegaRepositorio.ExisteBodegaEnRestauranteAsync(restauranteId, idBodega))
                throw new ArgumentException("La bodega destino especificada no es valida o no existe.");

            CategoriaInsumo categoria = await _categoriaInsumoRepositorio.ObtenerPorIdAsync(insumo.CategoriaId);

            if (categoria == null)
                throw new ArgumentException("La categoría de insumo seleccionada no existe en el sistema.");

            UnidadMedida unidadMedida = await _unidadMedidaRepositorio.ObtenerPorIdAsync(insumo.UnidadDeMedidaId);

            if (unidadMedida == null)
                throw new ArgumentException("La unidad de medida seleccionada no existe en el sistema.");

            

            var loteInicial = new Lote
            {
                Nombre = $"Lote {insumo.Nombre} - 1",
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

            return insumoCreado;
        }
    }
    
}
