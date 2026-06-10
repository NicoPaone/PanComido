using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ObtenerMetodosDePagoCasoDeUso
    {
        private readonly IMetodoDePagoRepositorio _metodoDePagoRepositorio;

        public ObtenerMetodosDePagoCasoDeUso(IMetodoDePagoRepositorio metodoDePagoRepositorio)
        {
            _metodoDePagoRepositorio = metodoDePagoRepositorio;
        }

        public async Task<List<MetodoDePago>> EjecutarAsync(int restauranteId)
        {
            List<MetodoDePago> metodosDePago = await _metodoDePagoRepositorio.ObtenerMetodosDePagoAsync(restauranteId);

            return metodosDePago;
        }
    }
}
