using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.ComandaCasosDeUso
{
    public class ObtenerComandaActivaPorMesaCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;

        public ObtenerComandaActivaPorMesaCasoDeUso(IComandaRepositorio comandaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
        }

        public async Task<Comanda?> EjecutarAsync(int mesaId)
        {
            return await _comandaRepositorio.ObtenerComandaPorIdMesaAsync(mesaId);
        }
    }
}
