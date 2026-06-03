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
            // Validación de superposición de mesas
            for (int i = 0; i < mesas.Count; i++)
            {
                for (int j = i + 1; j < mesas.Count; j++)
                {
                    var m1 = mesas[i];
                    var m2 = mesas[j];

                    bool superponenX = m1.PosicionXInicio < m2.PosicionXFin && m1.PosicionXFin > m2.PosicionXInicio;
                    bool superponenY = m1.PosicionYInicio < m2.PosicionYFin && m1.PosicionYFin > m2.PosicionYInicio;

                    if (superponenX && superponenY)
                    {
                        throw new System.InvalidOperationException($"Las mesas {m1.Numero} y {m2.Numero} están superpuestas. Por favor separalas.");
                    }
                }
            }

            await _mesaRepositorio.GuardarMapaMasivoAsync(restauranteId, mesas);
        }
    }
}
