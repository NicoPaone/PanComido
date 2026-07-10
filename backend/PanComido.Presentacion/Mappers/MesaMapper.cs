using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Presentacion.DTOs.Mesas;
using System;
using System.Collections.Generic;
using System.Linq;

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
            TipoElemento = mesa.TipoElemento,
            Color = mesa.Color,
            TextoObjeto = mesa.TextoObjeto,
            DimensionMesa = new DimensionMesaDto
            {
               Id = mesa.DimensionMesaId,
               Forma = mesa.Forma,
            },
            MozosAsignadosIds = mesa.MozosAsignadosIds
         };
      }

      public List<MesaResponseDto> aListaDto(List<MesaConPosiciones> mesas)
      {
         return mesas.Select(aDto).ToList();
      }

        // Mapper especifico para cuando se hace la accion de ocupar mesa

        public MesaSinPosicionesResponseDto aMesaSinPosicionesResponseDto(MesaConPosiciones mesa)
        {
            return new MesaSinPosicionesResponseDto
            {
                Id = mesa.Id,
                NumeroMesa = mesa.Numero,
                CantidadPersonasMax = mesa.CantPersonasMax,
                EstadoMesa = mesa.EstadoMesa
            };
        }

        private EstadoMesa ParsearEstado(string estadoStr)
      {
          // Intentamos parsear. Si falla o viene vacío, lo dejamos Disponible por seguridad.
          if (Enum.TryParse<EstadoMesa>(estadoStr, true, out var estado))
              return estado;
              
          return EstadoMesa.Disponible;
      }

      public MesaMapaDominio aDominio(GuardarMesaRequestDto dto)
      {
          return new MesaMapaDominio
          {
              Id = dto.Id,
              Numero = dto.NumeroMesa,
              CantPersonasMax = dto.CantidadPersonasMax,
              EstadoMesa = ParsearEstado(dto.EstadoMesa),
              PosicionXInicio = dto.PosicionXInicio,
              PosicionXFin = dto.PosicionXFin,
              PosicionYInicio = dto.PosicionYInicio,
              PosicionYFin = dto.PosicionYFin,
              DimensionMesaId = dto.DimensionMesa.Id,
              Forma = dto.DimensionMesa.Forma,
              TipoElemento = dto.TipoElemento,
              Color = dto.Color,
              TextoObjeto = dto.TextoObjeto
          };
      }

      public List<MesaMapaDominio> aListaDominio(List<GuardarMesaRequestDto> dtos)
      {
          return dtos.Select(aDominio).ToList();
      }
   }
}
