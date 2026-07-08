using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class ObtenerInsumoPorIdCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IUltimoPrecioCompraInsumoServicio _ultimoPrecioCompraServicio;

        public ObtenerInsumoPorIdCasoDeUso(IInsumoRepositorio insumoRepositorio, IUltimoPrecioCompraInsumoServicio ultimoPrecioCompraServicio)
        {
            _insumoRepositorio = insumoRepositorio;
            _ultimoPrecioCompraServicio = ultimoPrecioCompraServicio;
        }

        public async Task<Insumo> EjecutarAsync(int insumoId, int restauranteId)
        {
            Insumo insumo = await _insumoRepositorio.ObtenerPorIdAsync(insumoId, restauranteId);
            if (insumo == null)
            {
                throw new KeyNotFoundException("El insumo no existe o no pertenece al restaurante.");
            }

            insumo.CostoCalculado = _ultimoPrecioCompraServicio.ObtenerUltimoPrecioCompraRecibido(insumo.PedidoInsumos);

            return insumo;
        }
    }
}