using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ActualizarPorcentajesCasoDeUso
    {
        private readonly IPorcentajesCategoriaRepositorio _porcentajesCategoriaRepositorio;

        public ActualizarPorcentajesCasoDeUso(IPorcentajesCategoriaRepositorio porcentajesCategoriaRepositorio)
        {
            _porcentajesCategoriaRepositorio = porcentajesCategoriaRepositorio;
        }

        public async Task<PorcentajesGanancia> EjecutarAsync(int restauranteId, List<PorcentajesCategoria> platos, List<PorcentajesCategoria> bebidas)
        {
            return await _porcentajesCategoriaRepositorio.ActualizarPorcentajesGananciaAsync(restauranteId, platos, bebidas);
        }
    }
}
