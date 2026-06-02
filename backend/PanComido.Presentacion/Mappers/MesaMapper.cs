using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs.Mesas;

namespace PanComido.Presentacion.Mappers
{
   public class MesaMapper
   {
      public MesaResponseDto aDto(MesaConPosiciones mesa)
      {
         return new MesaResponseDto
         {
            Id = mesa.Id,
            NumeroMesa = mesa.Numero,
            CantidadPersonasMax = mesa.CantPersonasMax,
            EstadoMesa = mesa.EstadoMesa.ToString(),
            PosicionXInicio = mesa.PosicionXInicio,
            PosicionXFin = mesa.PosicionXFin,
            PosicionYInicio = mesa.PosicionYInicio,
            PosicionYFin = mesa.PosicionYFin,
            DimensionMesa = new DimensionMesaDto
            {
               Id = mesa.DimensionMesaId,
               Forma = mesa.Forma,
            }
         };


      }
      public List<MesaResponseDto> aListaDto(List<MesaConPosiciones> mesas)
      {
         return mesas.Select(aDto).ToList();
      }

      }
   }
