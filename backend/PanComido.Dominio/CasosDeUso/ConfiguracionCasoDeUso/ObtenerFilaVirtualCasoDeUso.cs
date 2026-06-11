using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ObtenerFilaVirtualCasoDeUso
    {
        private readonly IFilaVirtualRepositorio _filaVirualRepositorio;

        public ObtenerFilaVirtualCasoDeUso(IFilaVirtualRepositorio filaVirtualRepositorio)
        {
            _filaVirualRepositorio = filaVirtualRepositorio;
        }

        public async Task<FilaVirtual> EjecutarAsync(int restauranteId)
        {
            return await _filaVirualRepositorio.ObtenerFilaVirtualAsync(restauranteId);
        }
    }
}
