using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IMetodoDePagoRepositorio
    {
        Task<List<MetodoDePago>> ObtenerMetodosDePagoAsync(int restauranteId);
        Task ActualizarEstadoAsync(int restauranteId, List<MetodoDePago> metodosDePago);
    }
}
