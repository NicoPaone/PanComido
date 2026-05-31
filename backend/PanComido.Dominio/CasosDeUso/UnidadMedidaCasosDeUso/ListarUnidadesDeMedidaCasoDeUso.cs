using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.UnidadMedidaCasosDeUso
{
    public class ListarUnidadesDeMedidaCasoDeUso
    {
            private readonly IUnidadMedidaRepositorio _unidadMedidaRepositorio;
    
            public ListarUnidadesDeMedidaCasoDeUso(IUnidadMedidaRepositorio unidadMedidaRepositorio)
            {
                _unidadMedidaRepositorio = unidadMedidaRepositorio;
            }
    
            public async Task<List<UnidadMedida>> EjecutarAsync()
            {
                return await _unidadMedidaRepositorio.ObtenerUnidadesDeMedidaAsync();
            }
    }
}
