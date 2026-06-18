using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Presentacion.DTOs.Mesas
{
    public class MesaSinPosicionesResponseDto
    {
        public int Id { get; set; }
        public int NumeroMesa { get; set; }
        public int CantidadPersonasMax { get; set; }
        public EstadoMesa EstadoMesa { get; set; }
    }
}
