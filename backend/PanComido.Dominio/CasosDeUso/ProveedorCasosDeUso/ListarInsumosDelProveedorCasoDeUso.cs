using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso
{
    public class ListarInsumosDelProveedorCasoDeUso
    {

        private readonly IInsumoRepositorio _insumoRepositorio;

        public ListarInsumosDelProveedorCasoDeUso(IInsumoRepositorio insumoRepositorio)
        {
            _insumoRepositorio = insumoRepositorio;
        }

        public async Task<List<Insumo>> EjecutarAsync(int proveedorId, int restauranteId)
        {
            return await _insumoRepositorio.ObtenerInsumosDelProveedorAsync(proveedorId, restauranteId);
        }
    }
}
