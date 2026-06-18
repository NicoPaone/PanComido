using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ObtenerPorcentajesCasoDeUso
    {
        private readonly IPorcentajesCategoriaRepositorio _porcentajeCategoriaRepositorio;

        public ObtenerPorcentajesCasoDeUso(IPorcentajesCategoriaRepositorio porcentajesCategoriaRepositorio)
        {
            _porcentajeCategoriaRepositorio = porcentajesCategoriaRepositorio;
        }

        public async Task<PorcentajesGanancia> EjecutarAsync(int restauranteId)
        {
            return await _porcentajeCategoriaRepositorio.ObtenerPorcentajesGananciaAsync(restauranteId);
        }
    }
}
