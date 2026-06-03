using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

public class MarcarItemsEntregadosCasoDeUso
{
    private readonly IComandaRepositorio _comandaRepositorio;
    private readonly IComandaNotificador _comandaNotificador;
    private readonly IMesaRepositorio _mesaRepositorio;

    public MarcarItemsEntregadosCasoDeUso(
        IComandaRepositorio comandaRepositorio,
        IComandaNotificador comandaNotificador,
        IMesaRepositorio mesaRepositorio)
    {
        _comandaRepositorio = comandaRepositorio;
        _comandaNotificador = comandaNotificador;
        _mesaRepositorio = mesaRepositorio;
    }

    public async Task<Comanda> EjecutarAsync(int comandaId, List<int> articuloComandaIds)
    {
        Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
        if (comanda == null) throw new KeyNotFoundException("Comanda no encontrada");

        foreach (int articuloComandaId in articuloComandaIds)
        {
            if (!comanda.Items.Any(ac => ac.Id == articuloComandaId))
                throw new KeyNotFoundException("Artículo de comanda no encontrado en la comanda especificada.");
            var item = comanda.Items.First(ac => ac.Id == articuloComandaId);
            if (item.Entregado)
                throw new InvalidOperationException("El ítem ya fue entregado.");
        }

        if (comanda.Estado == EstadoComanda.Finalizada)
            throw new InvalidOperationException("La comanda ya está finalizada.");

        await _comandaRepositorio.MarcarItemsEntregadosAsync(comandaId, articuloComandaIds);
        Comanda comandaDespuesDeEntregados = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);

        if (comandaDespuesDeEntregados.Items.All(i => i.Entregado))
        {
            await _comandaRepositorio.ModificarEstadoComandaAsync(comandaDespuesDeEntregados.MesaId, (int)EstadoComanda.EnEspera);
            var comandaFinal = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            var mozoIds = await _mesaRepositorio.ObtenerMozoIdsPorMesaAsync(comandaDespuesDeEntregados.MesaId);
            await _comandaNotificador.NotificarEstadoModificadoAsync(comandaFinal, mozoIds);
            return comandaFinal;
        }

        var mozoIdsParcial = await _mesaRepositorio.ObtenerMozoIdsPorMesaAsync(comandaDespuesDeEntregados.MesaId);
        await _comandaNotificador.NotificarEstadoModificadoAsync(comandaDespuesDeEntregados, mozoIdsParcial);
        return comandaDespuesDeEntregados;
    }
}