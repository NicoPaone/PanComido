using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
   public class ListarMesasCasoDeUso
   {
      private readonly IMesaRepositorio _mesaRepositorio;

      public ListarMesasCasoDeUso(IMesaRepositorio mesaRepositorio)
      {
         _mesaRepositorio = mesaRepositorio;
      }
      public async Task<List<MesaConPosiciones>> EjecutarAsync(int restauranteId)
      {
         return await _mesaRepositorio.ObtenerTodasAsync(restauranteId);
      }
   }
}
