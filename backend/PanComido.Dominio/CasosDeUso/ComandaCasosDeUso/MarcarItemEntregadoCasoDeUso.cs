using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ComandaCasosDeUso
{
    public class MarcarItemEntregadoCasoDeUso
    {
        private readonly IComandaRepositorio _comandaRepositorio;

        public MarcarItemEntregadoCasoDeUso(IComandaRepositorio comandaRepositorio)
        {
            _comandaRepositorio = comandaRepositorio;
        }

        public async Task<Comanda> EjecutarAsync(int comandaId, List<int> articuloComandaIds)
        {
            Comanda comanda = await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null) throw new KeyNotFoundException("Comanda no encontrada");

            foreach(int articuloComandaId in articuloComandaIds)
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
            return await _comandaRepositorio.ObtenerComandaPorIdAsync(comandaId);
        }
    }
}
