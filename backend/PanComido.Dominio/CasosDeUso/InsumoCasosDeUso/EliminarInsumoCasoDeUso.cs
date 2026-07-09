using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class EliminarInsumoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IPlatoRepositorio _platoRepositorio;
        private readonly IMiseAndPlaceRepositorio _miseAndPlaceRepositorio;
        private readonly IBebidaPreparadaRepositorio _bebidaPreparadaRepositorio;
        private readonly IPedidoRepositorio _pedidoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;

        public EliminarInsumoCasoDeUso(
            IInsumoRepositorio insumoRepositorio,
            IPlatoRepositorio platoRepositorio,
            IMiseAndPlaceRepositorio miseAndPlaceRepositorio,
            IBebidaPreparadaRepositorio bebidaPreparadaRepositorio,
            IPedidoRepositorio pedidoRepositorio,
            ILoteRepositorio loteRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
            _platoRepositorio = platoRepositorio;
            _miseAndPlaceRepositorio = miseAndPlaceRepositorio;
            _bebidaPreparadaRepositorio = bebidaPreparadaRepositorio;
            _pedidoRepositorio = pedidoRepositorio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task EjecutarAsync(int insumoId, int restauranteId)
        {
            // 1. Validar que no tenga stock positivo
            decimal stockActual = await _loteRepositorio.ObtenerStockTotalDeInsumo(insumoId);
            if (stockActual > 0)
            {
                throw new InvalidOperationException("No se puede eliminar el insumo porque actualmente tiene stock disponible en bodega. Debe registrar su consumo o pérdida (stock = 0) antes de eliminarlo.");
            }

            // 2. Validar que no pertenezca a ningún Plato activo
            bool existeEnPlatos = await _platoRepositorio.ExisteInsumoEnPlatosActivosAsync(insumoId);
            if (existeEnPlatos)
            {
                throw new InvalidOperationException("No se puede eliminar el insumo porque forma parte de la receta de un Plato que está activo. Elimine el plato o modifique su receta primero.");
            }

            // 3. Validar que no pertenezca a ninguna Bebida Preparada activa
            bool existeEnBebidas = await _bebidaPreparadaRepositorio.ExisteInsumoEnBebidasActivasAsync(insumoId);
            if (existeEnBebidas)
            {
                throw new InvalidOperationException("No se puede eliminar el insumo porque forma parte de la receta de una Bebida Preparada que está activa.");
            }

            // 4. Validar que no pertenezca a ningún MiseAndPlace (Ingrediente Preparado) activo
            bool existeEnMise = await _miseAndPlaceRepositorio.ExisteInsumoEnMiseAndPlaceActivosAsync(insumoId);
            if (existeEnMise)
            {
                throw new InvalidOperationException("No se puede eliminar el insumo porque forma parte de la receta de un Ingrediente Preparado (Mise and Place) que está activo.");
            }

            // 5. Validar que no haya Pedidos a Proveedores pendientes de recibir
            bool existeEnPedidos = await _pedidoRepositorio.ExisteInsumoEnPedidosPendientesAsync(insumoId);
            if (existeEnPedidos)
            {
                throw new InvalidOperationException("No se puede eliminar el insumo porque existen Pedidos a Proveedores que lo incluyen y que aún no han sido recibidos.");
            }

            var eliminado = await _insumoRepositorio.EliminarAsync(insumoId, restauranteId);
            if (eliminado == null)
            {
                throw new KeyNotFoundException("El insumo no existe o no pertenece al restaurante.");
            }
        }
    }
}