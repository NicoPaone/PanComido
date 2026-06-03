using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface ILlamadoRepositorio
    {
        Task<Llamado> crearLlamadoAsync(Llamado llamado);

        Task<bool> ResolverLlamadoAsync(int llamadoId);
        Task<List<Llamado>> ObtenerLlamadosPendientesPorMozoAsync(int llamadoId);
        Task ResolverLlamadoPorMesaYCategoriaAsync(int mesaId, int categoriaLlamadoId);
    }
}
