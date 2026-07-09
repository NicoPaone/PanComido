using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IPagoRepositorio
    {
        Task<Pago> CrearPagoAsync(Pago pago);
        Task<Pago?> ObtenerPagoPorExternalReferenceAsync(string externalReference);
        Task<Pago?> ObtenerPagoPorComandaIdAsync(int comandaId);
        Task<Pago?> ConfirmarPagoAsync(string externalReference);
        Task<Pago?> RechazarPagoAsync(string externalReference);
        Task<List<Pago>> ObtenerPagosParaCierreAsync(int restauranteId,
                                                    DateTime horarioApertura,
                                                    DateTime horarioCierre);
        Task<List<Pago>> ObtenerPagosPorCierreIdAsync(int cierreId);
    }
}
