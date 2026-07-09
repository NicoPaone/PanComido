using System;
using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Entidades
{
    public class MesaFilaVirtualDto
    {
        public int Id { get; set; }
        public int CantPersonasMax { get; set; }
        public EstadoMesa EstadoMesa { get; set; }
        public DateTime? HoraInicioComandaActiva { get; set; }
    }
}
