using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
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
            ValidarSuperposicion(mesas);

            var idsActivos = await _mesaRepositorio.ObtenerIdsMesasActivasAsync(restauranteId);
            var idsRecibidos = mesas.Where(m => m.Id > 0).Select(m => m.Id).ToList();
            var idsAEliminar = idsActivos.Except(idsRecibidos).ToList();

            await ValidarComandasActivasAsync(idsAEliminar);
            await ValidarMozosAsignadosAsync(idsAEliminar);

            await _mesaRepositorio.GuardarMapaMasivoAsync(restauranteId, mesas);
        }

        private void ValidarSuperposicion(List<MesaMapaDominio> mesas)
        {
            for (int i = 0; i < mesas.Count; i++)
            {
                for (int j = i + 1; j < mesas.Count; j++)
                {
                    var m1 = mesas[i];
                    var m2 = mesas[j];

                    bool superponenX = m1.PosicionXInicio < m2.PosicionXFin && m1.PosicionXFin > m2.PosicionXInicio;
                    bool superponenY = m1.PosicionYInicio < m2.PosicionYFin && m1.PosicionYFin > m2.PosicionYInicio;

                    if (superponenX && superponenY)
                        throw new InvalidOperationException($"Las mesas {m1.Numero} y {m2.Numero} están superpuestas. Por favor separalas.");
                }
            }
        }

        private async Task ValidarComandasActivasAsync(List<int> idsAEliminar)
        {
            if (!idsAEliminar.Any()) return;

            if (await _mesaRepositorio.TieneComandasActivasAsync(idsAEliminar))
                throw new InvalidOperationException("No se puede guardar el mapa: se intentó eliminar una mesa que tiene una comanda activa.");
        }

        private async Task ValidarMozosAsignadosAsync(List<int> idsAEliminar)
        {
            if (!idsAEliminar.Any()) return;

            if (await _mesaRepositorio.TieneMozosAsignadosAsync(idsAEliminar))
                throw new InvalidOperationException("No se puede guardar el mapa: se intentó eliminar una mesa que tiene mozos asignados. Desasigná a los mozos antes de eliminarla.");
        }
    }
}
