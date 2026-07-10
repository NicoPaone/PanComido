using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IComandaRepositorio
    {
        Task<List<Comanda>> ObtenerComandasActivasParaCocinaAsync(int restauranteId);

        Task<int> CrearAsync(Comanda comandaDominio);
        Task<Comanda?> ModificarEstadoComandaAsync(int comandaId, int estadoId);
        Task<Comanda?> ObtenerComandaPorIdMesaAsync(int mesaId);
        Task<List<Comanda>> ObtenerComandasActivasPorMozoAsync(int restauranteId, int mozoId);
        Task<Comanda?> ObtenerComandaPorIdAsync(int comandaId);
        Task MarcarItemsEntregadosAsync(int comandaId, List<int> articuloComandaIds);
        Task ActualizarAsync(Comanda comanda);
        Task ActualizarComandaParaPagoAsync(Comanda comanda);
        Task<List<VentaReporteDetalle>> ObtenerReporteVentasPorPeriodoAsync(int restauranteId, DateTime desde, DateTime hasta);
    }
}