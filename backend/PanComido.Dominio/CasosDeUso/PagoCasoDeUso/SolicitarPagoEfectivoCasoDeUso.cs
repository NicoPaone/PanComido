using Microsoft.Extensions.Logging;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.CasosDeUso.PagoCasoDeUso
{
    public class SolicitarPagoEfectivoCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;
        private readonly ICrearLlamadoServicio _crearLlamadoServicio;
        private readonly ILogger<SolicitarPagoEfectivoCasoDeUso> _logger;


        public SolicitarPagoEfectivoCasoDeUso(IComandaRepositorio comandaRepositorio, ICrearLlamadoServicio crearLlamadoServicio, ILogger<SolicitarPagoEfectivoCasoDeUso> logger)
        {
            _comandaRepositorio = comandaRepositorio;
            _crearLlamadoServicio = crearLlamadoServicio;
            _logger = logger;
        }

        public async Task<Llamado> EjecutarAsync(int comandaId, int restauranteId)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null || comanda.RestauranteId != restauranteId)
            {
                _logger.LogWarning("Comanda no encontrada para solicitud de pago efectivo. ComandaId: {ComandaId}, RestauranteId: {RestauranteId}", comandaId, restauranteId);
                throw new KeyNotFoundException("Comanda no encontrada para el restaurante especificado.");
            }

            if (comanda.Estado != EstadoComanda.EnEspera)
            {
                _logger.LogWarning("Intento de solicitar pago efectivo en estado inválido. ComandaId: {ComandaId}, Estado: {Estado}", comandaId, comanda.Estado);
                throw new ArgumentException("La comanda no está esperando pago.");
            }

            Llamado llamadoGuardado = await _crearLlamadoServicio.CrearYNotificarAsync(comanda.MozoId, comanda.MesaId, comanda.NumeroDeMesa, CategoriaLlamado.Pago, "El comensal ha solicitado el pago en efectivo.");

            _logger.LogInformation("Pago efectivo solicitado. ComandaId: {ComandaId}, MesaId: {MesaId}, MozoId: {MozoId}", comandaId, comanda.MesaId, comanda.MozoId);
            return llamadoGuardado;
        }
    }
}
