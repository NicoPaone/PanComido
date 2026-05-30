using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.AvisosCasosDeUso
{
    public class ListarInsumosConVencimientoProximo
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;

        public ListarInsumosConVencimientoProximo(IInsumoRepositorio insumoRepositorio,
                                                    ILoteRepositorio loteRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
            _loteRepositorio = loteRepositorio;
        }

        public async Task<Dictionary<int, List<Lote>>> Ejecutar(int restauranteId)
        {
            List<Insumo> insumosConLotes = await _insumoRepositorio.ObtenerInsumosConLotesAsync(restauranteId);
            
            DateOnly fechaLimite = DateOnly.FromDateTime(DateTime.Now.AddDays(7));

            Dictionary<int, List<Lote>> vencimientosProximos = new Dictionary<int, List<Lote>>();

            foreach (var insumo in insumosConLotes)
            {
                List<Lote> lotesProximos = new();

                foreach (var lote in insumo.Lotes)
                {
                    if (lote.FechaVencimiento <= fechaLimite)
                    {
                        lotesProximos.Add(lote);
                    }
                }
                if (lotesProximos.Any())
                {
                    vencimientosProximos.Add(insumo.Id, lotesProximos);
                }
            }
            return vencimientosProximos;
        }
    }
}