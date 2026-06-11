using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOS.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PanComido.Presentacion.Mappers
{
    public class DashboardMapper
    {
        public List<InsumoPorVencerDto> aListaVencimientosDto(List<Insumo> insumos)
        {
            var listaDto = new List<InsumoPorVencerDto>();

            foreach (var insumo in insumos)
            {
                // Buscamos el lote que generó el vencimiento para extraer su nombre
                var loteCausante = insumo.Lotes
                    .Where(l => l.FechaVencimiento == insumo.Vencimiento)
                    .FirstOrDefault();

                string textoRelativo = string.Empty;
                if (insumo.Vencimiento.HasValue)
                {
                    var dias = insumo.Vencimiento.Value.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber;
                    textoRelativo = dias switch
                    {
                        < 0 => "vencido",
                        0 => "vence hoy",
                        1 => "vence mañana",
                        _ => $"vence en {dias} días"
                    };
                }

                var dtoItem = new InsumoPorVencerDto
                {
                    Nombre = insumo.Nombre,
                    LoteNombre = loteCausante != null ? $"Lote: {loteCausante.Nombre}" : "Lote: General",
                    Fecha = insumo.Vencimiento.HasValue ? insumo.Vencimiento.Value.ToString("dd/MM") : string.Empty,
                    Cantidad = $"{insumo.StockActual} {insumo.UnidadMedida} disponibles",
                    Criticidad = insumo.CriticidadVencimiento.HasValue ? insumo.CriticidadVencimiento.Value.ToString().ToUpper() : "BAJA",
                    Relativo = textoRelativo
                };

                listaDto.Add(dtoItem);
            }

            return listaDto;
        }
    }
}