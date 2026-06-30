using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IDashboardRepositorio
    {
        Task<TotalesPeriodo> ObtenerTotalesPeriodoAsync(int restauranteId, DateTime desde, DateTime hasta);
        Task<List<VentaAgrupada>> ObtenerVentasAgrupadasAsync(int restauranteId, DateTime desde, DateTime hasta, TipoAgrupacionTiempo tipoAgrupacion);
        Task<List<EstadisticaMozoRaw>> ObtenerEstadisticasMozosRawAsync(int restauranteId, DateTime desde, DateTime hasta);
    }
}
