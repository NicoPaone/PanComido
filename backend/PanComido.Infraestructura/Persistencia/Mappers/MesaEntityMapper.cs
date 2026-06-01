using PanComido.Dominio.Entidades.Enums;
using PanComido.Infraestructura.Persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class MesaEntityMapper
    {
        public DOM.Mesa? paraDominio(EF.Mesa mesaEF)
      {
         if (mesaEF == null)
                return null;

            return new DOM.Mesa
            {
                Id = mesaEF.Id,
                GrillaId = mesaEF.GrillaId, 
                DimensionMesaId = mesaEF.DimensionMesaId,
                CantPersonasMax = mesaEF.CantPersonasMax,
                EstadoMesa = (DOM.Enums.EstadoMesa)mesaEF.EstadoMesaId,
                Numero = mesaEF.Numero
            };
        }

        public EF.Mesa paraEntidad(DOM.Mesa mesaDominio)
        {
            return new EF.Mesa
            {
                Id = mesaDominio.Id,
                GrillaId = mesaDominio.GrillaId,
                DimensionMesaId = mesaDominio.DimensionMesaId,
                CantPersonasMax = mesaDominio.CantPersonasMax,
                EstadoMesaId = (int)mesaDominio.EstadoMesa,
                Numero = mesaDominio.Numero
            };
        }
      public DOM.MesaConPosiciones? paraDominioCompleto(EF.Mesa mesaEF)
      {
         if (mesaEF == null)
            return null;

         return new DOM.MesaConPosiciones
         {
            Id = mesaEF.Id,
            Numero = mesaEF.Numero,
            CantPersonasMax = mesaEF.CantPersonasMax,
            EstadoMesa = (DOM.Enums.EstadoMesa)mesaEF.EstadoMesaId,
            PosicionXInicio = mesaEF.PosicionXInicio,
            PosicionXFin = mesaEF.PosicionXFin,
            PosicionYInicio = mesaEF.PosicionYInicio,
            PosicionYFin = mesaEF.PosicionYFin,
            DimensionMesaId = mesaEF.DimensionMesaId,
            Forma = mesaEF.DimensionMesa?.Forma ?? "cuadrada",
         };
      }
   }
}
