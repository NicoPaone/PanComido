using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso
{
    public class ListarLlamadosPendientesCasoDeUso
    {
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        public ListarLlamadosPendientesCasoDeUso(ILlamadoRepositorio llamadoRepositorio)
        {
            _llamadoRepositorio = llamadoRepositorio;
        }
        public async Task<List<Llamado>> EjecutarAsync(int mozoId)
        {
            return await _llamadoRepositorio.ObtenerLlamadosPendientesPorMozoAsync(mozoId);
        }
    }
}
