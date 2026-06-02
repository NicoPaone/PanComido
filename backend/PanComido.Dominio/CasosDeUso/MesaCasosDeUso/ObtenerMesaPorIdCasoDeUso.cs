using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.MesaCasosDeUso
{
   public class ObtenerMesaPorIdCasoDeUso
   {
      private readonly IMesaRepositorio _mesaRepositorio;

      public async Task<MesaConPosiciones?> EjecutarAsync(int mesaId, int restauranteId)
      {
         var mesas = await _mesaRepositorio.ObtenerTodasAsync(restauranteId);
         return mesas.FirstOrDefault(m => m.Id == mesaId);
      }

   }
}
