using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.InsumoCasosDeUso
{
    public class ListarCategoriasDeInsumosCasoDeUso
    {
        private readonly ICategoriaInsumoRepositorio _categoriaInsumoRepositorio;

        public ListarCategoriasDeInsumosCasoDeUso(ICategoriaInsumoRepositorio categoriaInsumoRepositorio)
        {
            _categoriaInsumoRepositorio = categoriaInsumoRepositorio;
        }

        public async Task<List<CategoriaInsumo>> EjecutarAsync()
        {
            return await _categoriaInsumoRepositorio.ObtenerCategoriasInsumoAsync();
        }
    }
}
