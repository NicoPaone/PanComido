using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IPorcentajesCategoriaRepositorio
    {
        Task<PorcentajesGanancia> ObtenerPorcentajesGananciaAsync(int restauranteId);
        Task<PorcentajesGanancia> ActualizarPorcentajesGananciaAsync(int restauranteId, List<PorcentajesCategoria> platos, List<PorcentajesCategoria> bebidas);
    }
}
