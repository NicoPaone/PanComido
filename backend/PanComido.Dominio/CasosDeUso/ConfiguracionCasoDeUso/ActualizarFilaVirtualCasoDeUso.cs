using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Entidades;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarFilaVirtualCasoDeUSo
    {
        private readonly IFilaVirtualRepositorio _filaVirualRepositorio;

        public ActualizarFilaVirtualCasoDeUSo(IFilaVirtualRepositorio filaVirtualRepositorio)
        {
            _filaVirualRepositorio = filaVirtualRepositorio;
        }

        public async Task<FilaVirtual> EjecutarAsync(int restauranteId, bool habilitada)
        {
            return await _filaVirualRepositorio.ActualizarFilaVirtualAsync(restauranteId, habilitada);
        }
    }
}
