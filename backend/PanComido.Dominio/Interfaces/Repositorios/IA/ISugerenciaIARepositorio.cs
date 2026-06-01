using PanComido.Dominio.Entidades.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios.IA
{
    public interface ISugerenciaIARepositorio
    {
        Task<SugerenciaIA?> ObtenerSugerenciaIAAsync(int restauranteId);

        Task GuardarSugerenciaIAAsync(int restauranteId, SugerenciaIA sugerenciaIA);
    }
}
