using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class CrearInsumoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IBodegaRepositorio _bodegaRepositorio;
        private readonly IInsumoValidacionServicio _insumoValidacionServicio;

        private readonly IEstadoStockInsumoServicio _estadoStockInsumoServicio;
        private readonly IImagenServicio _imagenServicio;
        private readonly INormalizadorNombreServicio _normalizadorNombreServicio;

        private readonly ILogger<CrearInsumoCasoDeUso> _logger;

        public CrearInsumoCasoDeUso(IInsumoRepositorio insumoRepositorio,
            ILoteRepositorio loteRepositorio,
            IBodegaRepositorio bodegaRepositorio,
            IInsumoValidacionServicio insumoValidacionServicio,
            IEstadoStockInsumoServicio estadoStockInsumoServicio,
            IImagenServicio imagenServicio,
            INormalizadorNombreServicio normalizadorNombreServicio,
            ILogger<CrearInsumoCasoDeUso> logger)
        {
            _insumoRepositorio = insumoRepositorio;
            _bodegaRepositorio = bodegaRepositorio;
            _insumoValidacionServicio = insumoValidacionServicio;
            _loteRepositorio = loteRepositorio;
            _estadoStockInsumoServicio = estadoStockInsumoServicio;
            _imagenServicio = imagenServicio;
            _normalizadorNombreServicio = normalizadorNombreServicio;
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
            insumo.Nombre = _normalizadorNombreServicio.Normalizar(insumo.Nombre);
            await ValidarNombreDuplicadoAsync(restauranteId, insumo.Nombre);
            await ValidarBodegaAsync(restauranteId, idBodega);

            CategoriaInsumo categoria = await _insumoValidacionServicio.ObtenerYValidarCategoriaAsync(insumo.CategoriaId);
            UnidadMedida unidadMedida = await _insumoValidacionServicio.ObtenerYValidarUnidadMedidaAsync(insumo.UnidadDeMedidaId);
            ValidarImagenSegunTipo(categoria.TipoAplica, stream, nombreImagen);

            Lote loteInicial = CrearLoteInicial(insumo.Nombre, cantidadInicial, idBodega, fechaVencimiento);

            insumo.RestauranteId = restauranteId;
            insumo.Tipo = categoria.TipoAplica;
            insumo.Lotes = new List<Lote> { loteInicial };

            insumo.UrlImagen = categoria.TipoAplica == TipoInsumo.Bebida
                ? await SubirYObtenerUrlDeImagen(stream, nombreImagen, carpetaCloudinary)
                : null;

            insumo.EsVisibleEnCarta = categoria.TipoAplica == TipoInsumo.Bebida && insumo.EsVisibleEnCarta;

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

        private async Task ValidarNombreDuplicadoAsync(int restauranteId, string nombre)
        {
            if (await _insumoRepositorio.ExisteInsumoConNombreAsync(restauranteId, nombre))
            {
                _logger.LogWarning("Rechazo al crear insumo: Ya existe uno con el nombre '{NombreInsumo}' en el restaurante {RestauranteId}.", nombre, restauranteId);
                throw new ArgumentException($"Ya existe un insumo con el nombre '{nombre}' en el restaurante.");
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

        private void ValidarImagenSegunTipo(TipoInsumo tipo, Stream stream, string nombreImagen)
        {
            bool tieneImagen = stream != null && !string.IsNullOrEmpty(nombreImagen);

            if (tipo == TipoInsumo.Bebida && !tieneImagen)
            {
                _logger.LogWarning("Rechazo al crear insumo: las bebidas requieren una imagen.");
                throw new ArgumentException("La imagen es obligatoria para las bebidas.");
            }
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
