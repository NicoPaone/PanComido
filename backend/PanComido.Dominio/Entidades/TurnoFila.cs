using PanComido.Dominio.Entidades.Enums;
using System;

namespace PanComido.Dominio.Entidades
{
    public class TurnoFila
    {
        public int Id { get; set; }
        public int FilaVirtualId { get; set; }
        public int Numero { get; set; }
        public int CantidadComensales { get; set; }
        public DateTime FechaHoraIngreso { get; set; }
        public EstadoTurnoMesa Estado { get; set; } 
        public int? MesaAsignadaId { get; set; }
        public DateTime? FechaHoraAsignacion { get; set; }
        public int? ComandaPreArmadaId { get; set; }
    }
}
