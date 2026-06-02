using DOM = PanComido.Dominio.Entidades;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso
{
    public class LlamarMozoCasoDeUso
    {
        private readonly IMozoRepositorio _mozoRepositorio;
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ILlamadoNotificador _llamadoNotificador;

        public LlamarMozoCasoDeUso(IMozoRepositorio mozoRepositorio, ILlamadoRepositorio llamadoRepositorio, ILlamadoNotificador llamadoNotificador)
        {
            _mozoRepositorio = mozoRepositorio;
            _llamadoRepositorio = llamadoRepositorio;
            _llamadoNotificador = llamadoNotificador;
        }

      public async Task<DOM.Llamado> EjecutarAsync(int mesaId, int categoriaLlamadoId, string? descripcion)
      {
         int mozoId = await _mozoRepositorio.ObtenerMozoAsignadoAMesaAsync(mesaId);
         if (mozoId == 0)
            throw new KeyNotFoundException("No se encontro un mozo asignado a esta mesa.");

         var llamado = new DOM.Llamado
         {
            MozoId = mozoId,
            MesaId = mesaId,               
            CategoriaLlamadoId = categoriaLlamadoId,
            Descripcion = descripcion,
            Resuelto = false
         };

         var llamadoGuardado = await _llamadoRepositorio.crearLlamadoAsync(llamado);

         await _llamadoNotificador.NotificarLlamadoAsync(llamadoGuardado);
         return llamadoGuardado;
      }

   }
}
