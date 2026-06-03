using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
    public class CambiarEstadoMesaCasoDeUso
    {
        private readonly IMesaRepositorio _mesaRepositorio;

        public CambiarEstadoMesaCasoDeUso(IMesaRepositorio mesaRepositorio)
        {
            _mesaRepositorio = mesaRepositorio;
        }

        public async Task<MesaConPosiciones> EjecutarAsync(int restauranteId, int mesaId, EstadoMesa nuevoEstado)
        {
            MesaConPosiciones mesa = await _mesaRepositorio.ObtenerPorIdAsync(mesaId, restauranteId);

            if (mesa == null)
                throw new ArgumentException("La mesa no existe o no pertenece al restaurante.");

            mesa.EstadoMesa = nuevoEstado;
            await _mesaRepositorio.ActualizarEstadoAsync(mesaId, nuevoEstado);

            return mesa;
        }
    }
}
