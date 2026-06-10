using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarMetodosDePagoCasoDeUso
    {
        private readonly IMetodoDePagoRepositorio _metodoDePagoRepositorio;

        public ActualizarMetodosDePagoCasoDeUso(IMetodoDePagoRepositorio metodoDePagoRepositorio) 
        {
            _metodoDePagoRepositorio = metodoDePagoRepositorio;
        }

        public async Task EjecutarAsync(int restauranteId, List<MetodoDePago> metodosDePago)
        {
            await _metodoDePagoRepositorio.ActualizarEstadoAsync(restauranteId, metodosDePago);
        }
    }
}
