using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso
{
    public class ListarFamiliasTipograficasCasoDeUso
    {
        private readonly IFamiliaTipograficaRepositorio _familiaTipograficaRepositorio;

        public ListarFamiliasTipograficasCasoDeUso(IFamiliaTipograficaRepositorio familiaTipograficaRepositorio)
        {
            _familiaTipograficaRepositorio = familiaTipograficaRepositorio;
        }

        public async Task<List<FamiliaTipografica>> EjecutarAsync()
        {
            return await _familiaTipograficaRepositorio.ListarTipografias();
        }
    }
}
