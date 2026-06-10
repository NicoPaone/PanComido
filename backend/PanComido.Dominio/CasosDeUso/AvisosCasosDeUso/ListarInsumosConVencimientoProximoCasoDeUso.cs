using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.AvisosCasosDeUso
{
    public class ListarInsumosConVencimientoProximoCasoDeUso
    {
        private readonly IInsumoRepositorio _insumoRepositorio;
        private readonly IVencimientosProximosInsumosServicio _vencimientosProximosInsumosServicio;

        public ListarInsumosConVencimientoProximoCasoDeUso(IInsumoRepositorio insumoRepositorio, IVencimientosProximosInsumosServicio vencimientosProximosInsumosServicio)
        {
            _insumoRepositorio = insumoRepositorio;
            _vencimientosProximosInsumosServicio = vencimientosProximosInsumosServicio;
        }

        public async Task<Dictionary<int, List<Lote>>> EjecutarAsync(int restauranteId)
        {
            List<Insumo> insumosConLotes = await _insumoRepositorio.ObtenerInsumosConLotesAsync(restauranteId);

            return _vencimientosProximosInsumosServicio.ObtenerVencimientosProximos(insumosConLotes, 7);
        }
    }
}