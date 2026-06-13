using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    internal class PorcentajesGananciaRepositorio : IPorcentajesCategoriaRepositorio
    {
        public Task<List<PorcentajesGanancia>> ActualizarPorcentajesGananciaAsync(int restauranteId, List<PorcentajesCategoria> platos, List<PorcentajesCategoria> bebidas)
        {
            throw new NotImplementedException();
        }

        public Task<PorcentajesGanancia> ObtenerPorcentajesGananciaAsync(int restauranteId)
        {
            throw new NotImplementedException();
        }
    }
}
