using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.CierreCajaCasoDeUso
{
    public class ListarCierresDeCajaCasoDeUso
    {
        private readonly ICierreCajaRepositorio _cierreCajaRepositorio;

        public ListarCierresDeCajaCasoDeUso(ICierreCajaRepositorio cierreCajaRepositorio)
        {
            _cierreCajaRepositorio = cierreCajaRepositorio;
        }

        public async Task<List<Cierre>> EjecutarAsync(int restauranteId)
        {
            var cierres = await _cierreCajaRepositorio.ObtenerCierresDeCajaAsync(restauranteId);

            if (cierres == null)
            {                
                return null;
            }
            return cierres;
        }
    }
}
