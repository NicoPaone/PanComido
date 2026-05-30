using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Repositorios;

namespace PanComido.Dominio.CasosDeUso.Comanda
{
   public class ModificarEstadoComandaCasoDeUso
   {
      private readonly IComandaRepositorio _comandaRepositorio;


      public ModificarEstadoComandaCasoDeUso(IComandaRepositorio comandaRepositorio)
      {
         _comandaRepositorio = comandaRepositorio;
      }

      public async Task<Entidades.Comanda?> EjecutarAsync(int mesaId, int estadoId)
      {
         Console.Write("Llego????");
         return await _comandaRepositorio.ModificarEstadoComandaAsync(mesaId, estadoId);
      }
   }
}
