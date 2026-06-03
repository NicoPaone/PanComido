using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class GuardarMapaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;

        public GuardarMapaCasoDeUso(IMesaRepositorio mesaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task EjecutarAsync(int restauranteId, List<MesaMapaDominio> mesas)
        {
            await _mesaRepositorio.GuardarMapaMasivoAsync(restauranteId, mesas);
        }
    }
}
