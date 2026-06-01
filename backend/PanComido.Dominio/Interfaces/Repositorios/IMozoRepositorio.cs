using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface IMozoRepositorio
    {
        Task<int> ObtenerMozoAsignadoAMesaAsync(int mesaId);
    }
}
